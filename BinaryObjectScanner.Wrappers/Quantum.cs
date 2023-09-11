using System.IO;
using System.Text;

namespace BinaryObjectScanner.Wrappers
{
    public class Quantum : WrapperBase
    {
        #region Descriptive Properties

        /// <inheritdoc/>
        public override string Description => "Quantum Archive";

        #endregion

        #region Pass-Through Properties

        #region Header

        /// <inheritdoc cref="Models.Quantum.Header.Signature"/>
        public string Signature => _archive.Header.Signature;

        /// <inheritdoc cref="Models.Quantum.Header.MajorVersion"/>
        public byte MajorVersion => _archive.Header.MajorVersion;

        /// <inheritdoc cref="Models.Quantum.Header.MinorVersion"/>
        public byte MinorVersion => _archive.Header.MinorVersion;

        /// <inheritdoc cref="Models.Quantum.Header.FileCount"/>
        public ushort FileCount => _archive.Header.FileCount;

        /// <inheritdoc cref="Models.Quantum.Header.TableSize"/>
        public byte TableSize => _archive.Header.TableSize;

        /// <inheritdoc cref="Models.Quantum.Header.CompressionFlags"/>
        public byte CompressionFlags => _archive.Header.CompressionFlags;

        #endregion

        #region File List

        /// <inheritdoc cref="Models.Quantum.Archive.FileList"/>
        public SabreTools.Models.Quantum.FileDescriptor[] FileList => _archive.FileList;

        #endregion

        /// <inheritdoc cref="Models.Quantum.Archive.CompressedDataOffset"/>
        public long CompressedDataOffset => _archive.CompressedDataOffset;

        #endregion

        #region Instance Variables

        /// <summary>
        /// Internal representation of the archive
        /// </summary>
        private SabreTools.Models.Quantum.Archive _archive;

        #endregion

        #region Constructors

        /// <summary>
        /// Private constructor
        /// </summary>
        private Quantum() { }

        /// <summary>
        /// Create a Quantum archive from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the archive</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A Quantum archive wrapper on success, null on failure</returns>
        public static Quantum Create(byte[] data, int offset)
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and use that
            MemoryStream dataStream = new MemoryStream(data, offset, data.Length - offset);
            return Create(dataStream);
        }

        /// <summary>
        /// Create a Quantum archive from a Stream
        /// </summary>
        /// <param name="data">Stream representing the archive</param>
        /// <returns>A Quantum archive wrapper on success, null on failure</returns>
        public static Quantum Create(Stream data)
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            var archive = new SabreTools.Serialization.Streams.Quantum().Deserialize(data);
            if (archive == null)
                return null;

            var wrapper = new Quantum
            {
                _archive = archive,
                _dataSource = DataSource.Stream,
                _streamData = data,
            };
            return wrapper;
        }

        #endregion

        #region Data

        /// <summary>
        /// Extract all files from the Quantum archive to an output directory
        /// </summary>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if all files extracted, false otherwise</returns>
        public bool ExtractAll(string outputDirectory)
        {
            // If we have no files
            if (FileList == null || FileList.Length == 0)
                return false;

            // Loop through and extract all files to the output
            bool allExtracted = true;
            for (int i = 0; i < FileList.Length; i++)
            {
                allExtracted &= ExtractFile(i, outputDirectory);
            }

            return allExtracted;
        }

        /// <summary>
        /// Extract a file from the Quantum archive to an output directory by index
        /// </summary>
        /// <param name="index">File index to extract</param>
        /// <param name="outputDirectory">Output directory to write to</param>
        /// <returns>True if the file extracted, false otherwise</returns>
        public bool ExtractFile(int index, string outputDirectory)
        {
            // If we have no files
            if (FileCount == 0 || FileList == null || FileList.Length == 0)
                return false;

            // If we have an invalid index
            if (index < 0 || index >= FileList.Length)
                return false;

            // Get the file information
            var fileDescriptor = FileList[index];

            // Read the entire compressed data
            int compressedDataOffset = (int)CompressedDataOffset;
            int compressedDataLength = GetEndOfFile() - compressedDataOffset;
            byte[] compressedData = ReadFromDataSource(compressedDataOffset, compressedDataLength);

            // TODO: Figure out decompression
            // - Single-file archives seem to work
            // - Single-file archives with files that span a window boundary seem to work
            // - The first files in each archive seem to work
            return false;

            // // Setup the decompression state
            // State state = new State();
            // Decompressor.InitState(state, TableSize, CompressionFlags);

            // // Decompress the entire array
            // int decompressedDataLength = (int)FileList.Sum(fd => fd.ExpandedFileSize);
            // byte[] decompressedData = new byte[decompressedDataLength];
            // Decompressor.Decompress(state, compressedData.Length, compressedData, decompressedData.Length, decompressedData);

            // // Read the data
            // int offset = (int)FileList.Take(index).Sum(fd => fd.ExpandedFileSize);
            // byte[] data = new byte[fileDescriptor.ExpandedFileSize];
            // Array.Copy(decompressedData, offset, data, 0, data.Length);

            // // Loop through all files before the current
            // for (int i = 0; i < index; i++)
            // {
            //     // Decompress the next block of data
            //     byte[] tempData = new byte[FileList[i].ExpandedFileSize];
            //     int lastRead = Decompressor.Decompress(state, compressedData.Length, compressedData, tempData.Length, tempData);
            //     compressedData = new ReadOnlySpan<byte>(compressedData, (lastRead), compressedData.Length - (lastRead)).ToArray();
            // }

            // // Read the data
            // byte[] data = new byte[fileDescriptor.ExpandedFileSize];
            // _ = Decompressor.Decompress(state, compressedData.Length, compressedData, data.Length, data);

            // // Create the filename
            // string filename = fileDescriptor.FileName;

            // // If we have an invalid output directory
            // if (string.IsNullOrWhiteSpace(outputDirectory))
            //     return false;

            // // Create the full output path
            // filename = Path.Combine(outputDirectory, filename);

            // // Ensure the output directory is created
            // Directory.CreateDirectory(Path.GetDirectoryName(filename));

            // // Try to write the data
            // try
            // {
            //     // Open the output file for writing
            //     using (Stream fs = File.OpenWrite(filename))
            //     {
            //         fs.Write(data, 0, data.Length);
            //     }
            // }
            // catch
            // {
            //     return false;
            // }

            return true;
        }

        #endregion

        #region Printing

        /// <inheritdoc/>
        public override StringBuilder PrettyPrint()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("Quantum Information:");
            builder.AppendLine("-------------------------");
            builder.AppendLine();

            PrintHeader(builder);
            PrintFileList(builder);
            builder.AppendLine($"  Compressed data offset: {CompressedDataOffset} (0x{CompressedDataOffset:X})");
            builder.AppendLine();

            return builder;
        }

        /// <summary>
        /// Print header information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintHeader(StringBuilder builder)
        {
            builder.AppendLine("  Header Information:");
            builder.AppendLine("  -------------------------");
            builder.AppendLine($"  Signature: {Signature}");
            builder.AppendLine($"  Major version: {MajorVersion} (0x{MajorVersion:X})");
            builder.AppendLine($"  Minor version: {MinorVersion} (0x{MinorVersion:X})");
            builder.AppendLine($"  File count: {FileCount} (0x{FileCount:X})");
            builder.AppendLine($"  Table size: {TableSize} (0x{TableSize:X})");
            builder.AppendLine($"  Compression flags: {CompressionFlags} (0x{CompressionFlags:X})");
            builder.AppendLine();
        }

        /// <summary>
        /// Print file list information
        /// </summary>
        /// <param name="builder">StringBuilder to append information to</param>
        private void PrintFileList(StringBuilder builder)
        {
            builder.AppendLine("  File List Information:");
            builder.AppendLine("  -------------------------");
            if (FileCount == 0 || FileList == null || FileList.Length == 0)
            {
                builder.AppendLine("  No file list items");
            }
            else
            {
                for (int i = 0; i < FileList.Length; i++)
                {
                    var fileDescriptor = FileList[i];
                    builder.AppendLine($"  File Descriptor {i}");
                    builder.AppendLine($"    File name size: {fileDescriptor.FileNameSize} (0x{fileDescriptor.FileNameSize:X})");
                    builder.AppendLine($"    File name: {fileDescriptor.FileName ?? "[NULL]"}");
                    builder.AppendLine($"    Comment field size: {fileDescriptor.CommentFieldSize} (0x{fileDescriptor.CommentFieldSize:X})");
                    builder.AppendLine($"    Comment field: {fileDescriptor.CommentField ?? "[NULL]"}");
                    builder.AppendLine($"    Expanded file size: {fileDescriptor.ExpandedFileSize} (0x{fileDescriptor.ExpandedFileSize:X})");
                    builder.AppendLine($"    File time: {fileDescriptor.FileTime} (0x{fileDescriptor.FileTime:X})");
                    builder.AppendLine($"    File date: {fileDescriptor.FileDate} (0x{fileDescriptor.FileDate:X})");
                    if (fileDescriptor.Unknown != null)
                        builder.AppendLine($"    Unknown (Checksum?): {fileDescriptor.Unknown} (0x{fileDescriptor.Unknown:X})");
                }
            }
            builder.AppendLine();
        }

#if NET6_0_OR_GREATER

        /// <inheritdoc/>
        public override string ExportJSON() =>  System.Text.Json.JsonSerializer.Serialize(_archive, _jsonSerializerOptions);

#endif

        #endregion
    }
}