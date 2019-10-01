﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BurnOutSharp.ProtectionType
{
    public class IndyVCD
    {
        public static string CheckPath(string path, IEnumerable<string> files, bool isDirectory)
        {
            if (isDirectory)
            {
                // TODO: Verify if these are OR or AND
                if (files.Count(f => Path.GetFileName(f).Equals("INDYVCD.AX", StringComparison.OrdinalIgnoreCase)) > 0
                    || files.Count(f => Path.GetFileName(f).Equals("INDYMP3.idt", StringComparison.OrdinalIgnoreCase)) > 0)
                {
                    return "IndyVCD";
                }
            }
            else
            {
                if (Path.GetFileName(path).Equals("INDYVCD.AX", StringComparison.OrdinalIgnoreCase)
                    || Path.GetFileName(path).Equals("INDYMP3.idt", StringComparison.OrdinalIgnoreCase))
                {
                    return "IndyVCD";
                }
            }

            return null;
        }
    }
}