﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    public class Bitpool : IPathCheck
    {
        /// <inheritdoc/>
        public string CheckPath(string path, bool isDirectory, IEnumerable<string> files)
        {
            if (isDirectory)
            {
                if (files.Any(f => Path.GetFileName(f).Equals("bitpool.rsc", StringComparison.OrdinalIgnoreCase)))
                    return "Bitpool";
            }
            else
            {
                if (Path.GetFileName(path).Equals("bitpool.rsc", StringComparison.OrdinalIgnoreCase))
                    return "Bitpool";
            }

            return null;
        }
    }
}
