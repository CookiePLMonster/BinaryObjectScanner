﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    public class SafeDiscLite : IPathCheck
    {
        /// <inheritdoc/>
        public string CheckPath(string path, bool isDirectory, IEnumerable<string> files)
        {
            if (isDirectory)
            {
                if (files.Any(f => Path.GetFileName(f).Equals("00000001.LT1", StringComparison.OrdinalIgnoreCase)))
                    return "SafeDisc Lite";
            }
            else
            {
                if (Path.GetFileName(path).Equals("00000001.LT1", StringComparison.OrdinalIgnoreCase))
                    return "SafeDisc Lite";
            }

            return null;
        }
    }
}
