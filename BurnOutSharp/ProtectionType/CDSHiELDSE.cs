﻿using System.Collections.Generic;
using System.Linq;
using BurnOutSharp.Matching;

namespace BurnOutSharp.ProtectionType
{
    public class CDSHiELDSE : IContentCheck
    {
        /// <inheritdoc/>
        private List<ContentMatchSet> GetContentMatchSets()
        {
            // TODO: Obtain a sample to find where this string is in a typical executable
            return new List<ContentMatchSet>
            {
                // ~0017.tmp
                new ContentMatchSet(new byte?[] { 0x7E, 0x30, 0x30, 0x31, 0x37, 0x2E, 0x74, 0x6D, 0x70 }, "CDSHiELD SE"),
            };
        }

        /// <inheritdoc/>
        public string CheckContents(string file, byte[] fileContent, bool includeDebug = false)
        {
            var contentMatchSets = GetContentMatchSets();
            if (contentMatchSets != null && contentMatchSets.Any())
                return MatchUtil.GetFirstMatch(file, fileContent, contentMatchSets, includeDebug);

            return null;
        }
    }
}
