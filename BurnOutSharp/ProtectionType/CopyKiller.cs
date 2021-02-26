﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    public class CopyKiller : IContentCheck, IPathCheck
    {
        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includePosition = false)
        {
            // "Tom Commander"
            byte[] check = new byte[] { 0x54, 0x6F, 0x6D, 0x20, 0x43, 0x6F, 0x6D, 0x6D, 0x61, 0x6E, 0x64, 0x65, 0x72 };
            if (fileContent.Contains(check, out int position))
                return "CopyKiller" + (includePosition ? $" (Index {position})" : string.Empty);

            return null;
        }

        /// <inheritdoc/>
        public string CheckPath(string path, bool isDirectory, IEnumerable<string> files)
        {
            if (isDirectory)
            {
                if (files.Any(f => Path.GetFileName(f).Equals("Autorun.dat", StringComparison.OrdinalIgnoreCase)))
                    return "CopyKiller";
            }
            else
            {
                if (Path.GetFileName(path).Equals("Autorun.dat", StringComparison.OrdinalIgnoreCase))
                    return "CopyKiller";
            }

            return null;
        }
    }
}
