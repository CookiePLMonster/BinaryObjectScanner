﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    public class SoftLock : IPathCheck
    {
        /// <inheritdoc/>
        public string CheckPath(string path, bool isDirectory, IEnumerable<string> files)
        {
            if (isDirectory)
            {
                // TODO: Verify if these are OR or AND
                if (files.Any(f => Path.GetFileName(f).Equals("SOFTLOCKI.dat", StringComparison.OrdinalIgnoreCase))
                    || files.Any(f => Path.GetFileName(f).Equals("SOFTLOCKC.dat", StringComparison.OrdinalIgnoreCase)))
                {
                    return "SoftLock";
                }
            }
            else
            {
                if (Path.GetFileName(path).Equals("SOFTLOCKI.dat", StringComparison.OrdinalIgnoreCase)
                    || Path.GetFileName(path).Equals("SOFTLOCKC.dat", StringComparison.OrdinalIgnoreCase))
                {
                    return "SoftLock";
                }
            }

            return null;
        }
    }
}
