﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    public class HexalockAutoLock
    {
        public static string CheckPath(string path, IEnumerable<string> files, bool isDirectory)
        {
            if (isDirectory)
            {
                // TODO: Verify if these are OR or AND
                if (files.Count(f => Path.GetFileName(f).Equals("Start_Here.exe", StringComparison.OrdinalIgnoreCase)) > 0
                    || files.Count(f => Path.GetFileName(f).Equals("HCPSMng.exe", StringComparison.OrdinalIgnoreCase)) > 0
                    || files.Count(f => Path.GetFileName(f).Equals("MFINT.DLL", StringComparison.OrdinalIgnoreCase)) > 0
                    || files.Count(f => Path.GetFileName(f).Equals("MFIMP.DLL", StringComparison.OrdinalIgnoreCase)) > 0)
                {
                    return "Hexalock AutoLock";
                }
            }
            else
            {
                if (Path.GetFileName(path).Equals("Start_Here.exe", StringComparison.OrdinalIgnoreCase)
                    || Path.GetFileName(path).Equals("HCPSMng.exe", StringComparison.OrdinalIgnoreCase)
                    || Path.GetFileName(path).Equals("MFINT.DLL", StringComparison.OrdinalIgnoreCase)
                    || Path.GetFileName(path).Equals("MFIMP.DLL", StringComparison.OrdinalIgnoreCase))
                {
                    return "Hexalock AutoLock";
                }
            }

            return null;
        }
    }
}