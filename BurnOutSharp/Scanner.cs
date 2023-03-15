﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BinaryObjectScanner.FileType;
using BinaryObjectScanner.Utilities;
using BinaryObjectScanner.Wrappers;
using static BinaryObjectScanner.Utilities.Dictionary;

namespace BurnOutSharp
{
    public class Scanner
    {
        #region Options

        /// <inheritdoc cref="Options.ScanArchives"/>
        public bool ScanArchives => options?.ScanArchives ?? false;

        /// <inheritdoc cref="Options.ScanContents"/>
        public bool ScanContents => options?.ScanContents ?? false;

        /// <inheritdoc cref="Options.ScanPaths"/>
        public bool ScanPackers => options?.ScanPackers ?? false;

        /// <inheritdoc cref="Options.ScanArchives"/>
        public bool ScanPaths => options?.ScanPaths ?? false;

        /// <inheritdoc cref="Options.IncludeDebug"/>
        public bool IncludeDebug => options?.IncludeDebug ?? false;

        /// <summary>
        /// Options object for configuration
        /// </summary>
        private readonly Options options;

        #endregion

        /// <summary>
        /// Optional progress callback during scanning
        /// </summary>
        private readonly IProgress<ProtectionProgress> fileProgress;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="scanArchives">Enable scanning archive contents</param>
        /// <param name="scanContents">Enable including content detections in output</param>
        /// <param name="scanPackers">Enable including packers in output</param>
        /// <param name="scanPaths">Enable including path detections in output</param>
        /// <param name="includeDebug">Enable including debug information</param>
        /// <param name="fileProgress">Optional progress callback</param>
        public Scanner(bool scanArchives, bool scanContents, bool scanPackers, bool scanPaths, bool includeDebug, IProgress<ProtectionProgress> fileProgress = null)
        {
            this.options = new Options
            {
                ScanArchives = scanArchives,
                ScanContents = scanContents,
                ScanPackers = scanPackers,
                ScanPaths = scanPaths,
                IncludeDebug = includeDebug,
            };

            this.fileProgress = fileProgress;

            // Register the codepages
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        #region Scanning

        /// <summary>
        /// Scan a single path and get all found protections
        /// </summary>
        /// <param name="path">Path to scan</param>
        /// <returns>Dictionary of list of strings representing the found protections</returns>
        public ConcurrentDictionary<string, ConcurrentQueue<string>> GetProtections(string path)
        {
            return GetProtections(new List<string> { path });
        }

        /// <summary>
        /// Scan the list of paths and get all found protections
        /// </summary>
        /// <returns>Dictionary of list of strings representing the found protections</returns>
        public ConcurrentDictionary<string, ConcurrentQueue<string>> GetProtections(List<string> paths)
        {
            // If we have no paths, we can't scan
            if (paths == null || !paths.Any())
                return null;

            // Set a starting starting time for debug output
            DateTime startTime = DateTime.UtcNow;

            // Checkpoint
            this.fileProgress?.Report(new ProtectionProgress(null, 0, null));

            // Temp variables for reporting
            string tempFilePath = Path.GetTempPath();
            string tempFilePathWithGuid = Path.Combine(tempFilePath, Guid.NewGuid().ToString());

            // Loop through each path and get the returned values
            var protections = new ConcurrentDictionary<string, ConcurrentQueue<string>>();
            foreach (string path in paths)
            {
                // Directories scan each internal file individually
                if (Directory.Exists(path))
                {
                    // Enumerate all files at first for easier access
                    var files = Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories).ToList();

                    // Scan for path-detectable protections
                    if (ScanPaths)
                    {
                        var directoryPathProtections = Handler.HandlePathChecks(path, files);
                        AppendToDictionary(protections, directoryPathProtections);
                    }

                    // Scan each file in directory separately
                    for (int i = 0; i < files.Count; i++)
                    {
                        // Get the current file
                        string file = files.ElementAt(i);

                        // Get the reportable file name
                        string reportableFileName = file;
                        if (reportableFileName.StartsWith(tempFilePath))
                            reportableFileName = reportableFileName.Substring(tempFilePathWithGuid.Length);

                        // Checkpoint
                        this.fileProgress?.Report(new ProtectionProgress(reportableFileName, i / (float)files.Count, "Checking file" + (file != reportableFileName ? " from archive" : string.Empty)));

                        // Scan for path-detectable protections
                        if (ScanPaths)
                        {
                            var filePathProtections = Handler.HandlePathChecks(file, files: null);
                            AppendToDictionary(protections, filePathProtections);
                        }

                        // Scan for content-detectable protections
                        var fileProtections = GetInternalProtections(file);
                        if (fileProtections != null && fileProtections.Any())
                        {
                            foreach (string key in fileProtections.Keys)
                            {
                                if (!protections.ContainsKey(key))
                                    protections[key] = new ConcurrentQueue<string>();

                                protections[key].AddRange(fileProtections[key]);
                            }
                        }

                        // Checkpoint
                        protections.TryGetValue(file, out ConcurrentQueue<string> fullProtectionList);
                        string fullProtection = (fullProtectionList != null && fullProtectionList.Any() ? string.Join(", ", fullProtectionList) : null);
                        this.fileProgress?.Report(new ProtectionProgress(reportableFileName, (i + 1) / (float)files.Count, fullProtection ?? string.Empty));
                    }
                }

                // Scan a single file by itself
                else if (File.Exists(path))
                {
                    // Get the reportable file name
                    string reportableFileName = path;
                    if (reportableFileName.StartsWith(tempFilePath))
                        reportableFileName = reportableFileName.Substring(tempFilePathWithGuid.Length);

                    // Checkpoint
                    this.fileProgress?.Report(new ProtectionProgress(reportableFileName, 0, "Checking file" + (path != reportableFileName ? " from archive" : string.Empty)));

                    // Scan for path-detectable protections
                    if (ScanPaths)
                    {
                        var filePathProtections = Handler.HandlePathChecks(path, files: null);
                        AppendToDictionary(protections, filePathProtections);
                    }

                    // Scan for content-detectable protections
                    var fileProtections = GetInternalProtections(path);
                    if (fileProtections != null && fileProtections.Any())
                    {
                        foreach (string key in fileProtections.Keys)
                        {
                            if (!protections.ContainsKey(key))
                                protections[key] = new ConcurrentQueue<string>();

                            protections[key].AddRange(fileProtections[key]);
                        }
                    }

                    // Checkpoint
                    protections.TryGetValue(path, out ConcurrentQueue<string> fullProtectionList);
                    string fullProtection = (fullProtectionList != null && fullProtectionList.Any() ? string.Join(", ", fullProtectionList) : null);
                    this.fileProgress?.Report(new ProtectionProgress(reportableFileName, 1, fullProtection ?? string.Empty));
                }

                // Throw on an invalid path
                else
                {
                    Console.WriteLine($"{path} is not a directory or file, skipping...");
                    //throw new FileNotFoundException($"{path} is not a directory or file, skipping...");
                }
            }

            // Clear out any empty keys
            ClearEmptyKeys(protections);

            // If we're in debug, output the elasped time to console
            if (IncludeDebug)
                Console.WriteLine($"Time elapsed: {DateTime.UtcNow.Subtract(startTime)}");

            return protections;
        }

        /// <summary>
        /// Get the content-detectable protections associated with a single path
        /// </summary>
        /// <param name="file">Path to the file to scan</param>
        /// <returns>Dictionary of list of strings representing the found protections</returns>
        private ConcurrentDictionary<string, ConcurrentQueue<string>> GetInternalProtections(string file)
        {
            // Quick sanity check before continuing
            if (!File.Exists(file))
                return null;

            // Open the file and begin scanning
            try
            {
                using (FileStream fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    return GetInternalProtections(file, fs);
                }
            }
            catch (Exception ex)
            {
                if (IncludeDebug) Console.WriteLine(ex);

                var protections = new ConcurrentDictionary<string, ConcurrentQueue<string>>();
                AppendToDictionary(protections, file, IncludeDebug ? ex.ToString() : "[Exception opening file, please try again]");
                ClearEmptyKeys(protections);
                return protections;
            }
        }

        /// <summary>
        /// Get the content-detectable protections associated with a single path
        /// </summary>
        /// <param name="fileName">Name of the source file of the stream, for tracking</param>
        /// <param name="stream">Stream to scan the contents of</param>
        /// <returns>Dictionary of list of strings representing the found protections</returns>
        private ConcurrentDictionary<string, ConcurrentQueue<string>> GetInternalProtections(string fileName, Stream stream)
        {
            // Quick sanity check before continuing
            if (stream == null || !stream.CanRead || !stream.CanSeek)
                return null;

            // Initialize the protections found
            var protections = new ConcurrentDictionary<string, ConcurrentQueue<string>>();

            // Get the extension for certain checks
            string extension = Path.GetExtension(fileName).ToLower().TrimStart('.');

            // Open the file and begin scanning
            try
            {
                // Get the first 16 bytes for matching
                byte[] magic = new byte[16];
                try
                {
                    stream.Read(magic, 0, 16);
                    stream.Seek(0, SeekOrigin.Begin);
                }
                catch (Exception ex)
                {
                    if (IncludeDebug) Console.WriteLine(ex);

                    return null;
                }

                // Get the file type either from magic number or extension
                SupportedFileType fileType = FileTypes.GetFileType(magic);
                if (fileType == SupportedFileType.UNKNOWN)
                    fileType = FileTypes.GetFileType(extension);

                // If we still got unknown, just return null
                if (fileType == SupportedFileType.UNKNOWN)
                    return null;

                #region Non-Archive File Types

                // Create a detectable for the given file type
                var detectable = Factory.CreateDetectable(fileType);

                // If we're scanning file contents
                if (detectable != null && ScanContents)
                {
                    // If we have an executable, it needs to bypass normal handling
                    // TODO: Write custom executable handling
                    if (detectable is Executable)
                    {
                        ProcessExecutable(fileName, stream, protections);
                    }

                    // Otherwise, use the default implementation
                    else
                    {
                        var subProtections = Handler.HandleDetectable(detectable, fileName, stream, IncludeDebug);
                        if (subProtections != null)
                            AppendToDictionary(protections, fileName, subProtections);
                    }

                    string subProtection = detectable.Detect(stream, fileName, IncludeDebug);
                    if (!string.IsNullOrWhiteSpace(subProtection))
                    {
                        // If we have an indicator of multiple protections
                        if (subProtection.Contains(';'))
                        {
                            var splitProtections = subProtection.Split(';');
                            AppendToDictionary(protections, fileName, splitProtections);
                        }
                        else
                        {
                            AppendToDictionary(protections, fileName, subProtection);
                        }
                    }
                }

                #endregion

                #region Archive File Types

                // Create an extractable for the given file type
                var extractable = Factory.CreateExtractable(fileType);

                // If we're scanning archives
                if (extractable != null && ScanArchives)
                {
                    // We only want to process non-executables
                    if (!(extractable is Executable))
                    {
                        var subProtections = Handler.HandleExtractable(extractable, fileName, stream, this);
                        if (subProtections != null)
                            AppendToDictionary(protections, subProtections);
                    }
                }

                #endregion
            }
            catch (Exception ex)
            {
                if (IncludeDebug) Console.WriteLine(ex);
                AppendToDictionary(protections, fileName, IncludeDebug ? ex.ToString() : "[Exception opening file, please try again]");
            }

            // Clear out any empty keys
            ClearEmptyKeys(protections);

            return protections;
        }

        #endregion

        #region Executable Handling

        /// <summary>
        /// Process scanning for an Executable type
        /// </summary>
        /// <param name="fileName">Name of the source file of the stream, for tracking</param>
        /// <param name="stream">Stream to scan the contents of</param>
        /// <param name="protections">Current set of protections to append to</param>
        /// <remarks>
        /// Ideally, we wouldn't need to circumvent the proper handling of file types just for Executable,
        /// but due to the complexity of scanning, this is not currently possible.
        /// </remarks>
        private void ProcessExecutable(string fileName, Stream stream, ConcurrentDictionary<string, ConcurrentQueue<string>> protections)
        {
            // Try to create a wrapper for the proper executable type
            var wrapper = WrapperFactory.CreateExecutableWrapper(stream);
            if (wrapper == null)
                return;

            // Only use generic content checks if we're in debug mode
            if (IncludeDebug)
            {
                var subProtections = Handler.HandleContentChecks(fileName, stream, this);
                if (subProtections != null)
                    AppendToDictionary(protections, fileName, subProtections);
            }

            if (wrapper is MSDOS)
            {
                // No-op until protection classes implmented
            }
            else if (wrapper is LinearExecutable lex)
            {
                var subProtections = Handler.HandleLinearExecutableChecks(fileName, stream, lex, this);
                if (subProtections != null)
                    AppendToDictionary(protections, subProtections);
            }
            else if (wrapper is NewExecutable nex)
            {
                var subProtections = Handler.HandleNewExecutableChecks(fileName, stream, nex, this);
                if (subProtections != null)
                    AppendToDictionary(protections, subProtections);
            }
            else if (wrapper is PortableExecutable pex)
            {
                var subProtections = Handler.HandlePortableExecutableChecks(fileName, stream, pex, this);
                if (subProtections != null)
                    AppendToDictionary(protections, subProtections);
            }
        }

        #endregion
    }
}
