﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    public class ByteShield : IPathCheck
    {
        /// <inheritdoc/>
        public string CheckDirectoryPath(string path, IEnumerable<string> files)
        {
            if (files.Any(f => Path.GetFileName(f).Equals("Byteshield.dll", StringComparison.OrdinalIgnoreCase))
                || files.Any(f => Path.GetExtension(f).Trim('.').Equals("bbz", StringComparison.OrdinalIgnoreCase)))
            {
                return "ByteShield";
            }

            return null;
        }

        /// <inheritdoc/>
        public string CheckFilePath(string path)
        {
            if (Path.GetFileName(path).Equals("Byteshield.dll", StringComparison.OrdinalIgnoreCase)
                || Path.GetExtension(path).Trim('.').Equals("bbz", StringComparison.OrdinalIgnoreCase))
            {
                return "ByteShield";
            }
            
            return null;
        }
    }
}
