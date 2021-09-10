﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BurnOutSharp.ExecutableType.Microsoft;
using BurnOutSharp.FileType;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    // TODO: Figure out how to use path check framework here
    public class XCP : IContentCheck, IPathCheck
    {
        /// <inheritdoc/>
        private List<ContentMatchSet> GetContentMatchSets()
        {
            // TODO: Obtain a sample to find where this string is in a typical executable
            return new List<ContentMatchSet>
            {
                // xcpdrive
                new ContentMatchSet(new byte?[]
                {
                    0x78, 0x63,  0x70, 0x64, 0x72, 0x69, 0x76, 0x65
                }, "XCP"),
            };
        }

        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includeDebug = false)
        {
            // Get the sections from the executable, if possible
            PortableExecutable pex = PortableExecutable.Deserialize(fileContent, 0);
            var sections = pex?.SectionTable;
            if (sections == null)
                return null;

            // Get the .rdata section, if it exists
            var rdataSection = sections.FirstOrDefault(s => Encoding.ASCII.GetString(s.Name).StartsWith(".rdata"));
            if (rdataSection != null)
            {
                int sectionAddr = (int)rdataSection.PointerToRawData;
                int sectionEnd = sectionAddr + (int)rdataSection.VirtualSize;
                var matchers = new List<ContentMatchSet>
                {
                    // XCP.DAT
                    new ContentMatchSet(
                        new ContentMatch(new byte?[] { 0x58, 0x43, 0x50, 0x2E, 0x44, 0x41, 0x54 }, start: sectionAddr, end: sectionEnd),
                    "XCP"),

                    // XCPPlugins.dll
                    new ContentMatchSet(
                        new ContentMatch(new byte?[]
                        {
                            0x58, 0x43, 0x50, 0x50, 0x6C, 0x75, 0x67, 0x69,
                            0x6E, 0x73, 0x2E, 0x64, 0x6C, 0x6C
                        }, start: sectionAddr, end: sectionEnd),
                    "XCP"),

                    // XCPPhoenix.dll
                    new ContentMatchSet(
                        new ContentMatch(new byte?[]
                        {
                            0x58, 0x43, 0x50, 0x50, 0x68, 0x6F, 0x65, 0x6E,
                            0x69, 0x78, 0x2E, 0x64, 0x6C, 0x6C
                        }, start: sectionAddr, end: sectionEnd),
                    "XCP"),
                };

                string match = MatchUtil.GetFirstMatch(file, fileContent, matchers, includeDebug);
                if (!string.IsNullOrWhiteSpace(match))
                    return match;
            }

            var contentMatchSets = GetContentMatchSets();
            if (contentMatchSets != null && contentMatchSets.Any())
                return MatchUtil.GetFirstMatch(file, fileContent, contentMatchSets, includeDebug);

            return null;
        }

        /// <inheritdoc/>
        public ConcurrentQueue<string> CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            var protections = new ConcurrentQueue<string>();

            // TODO: Verify if these are OR or AND
            if (files.Any(f => Path.GetFileName(f).Equals("XCP.DAT", StringComparison.OrdinalIgnoreCase))
                || files.Any(f => Path.GetFileName(f).Equals("ECDPlayerControl.ocx", StringComparison.OrdinalIgnoreCase)))
            {
                string versionDatPath = files.FirstOrDefault(f => Path.GetFileName(f).Equals("VERSION.DAT", StringComparison.OrdinalIgnoreCase));
                if (!string.IsNullOrWhiteSpace(versionDatPath))
                {
                    string xcpVersion = GetDatVersion(versionDatPath);
                    if (!string.IsNullOrWhiteSpace(xcpVersion))
                        protections.Enqueue(xcpVersion);
                }
                else
                {
                    protections.Enqueue("XCP");
                }
            }

            return protections;
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            if (Path.GetFileName(path).Equals("XCP.DAT", StringComparison.OrdinalIgnoreCase)
                || Path.GetFileName(path).Equals("ECDPlayerControl.ocx", StringComparison.OrdinalIgnoreCase))
            {
                return "XCP";
            }

            return null;
        }

        private static string GetDatVersion(string path)
        {
            try
            {
                var xcpIni = new IniFile(path);
                return xcpIni["XCP.VERSION"];
            }
            catch
            {
               return null;
            }
        }
    }
}
