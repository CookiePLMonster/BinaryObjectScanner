﻿using System;

namespace BurnOutSharp.Models.NewExecutable
{
    [Flags]
    public enum HeaderFlag : ushort
    {
        NOAUTODATA = 0x0000,

        /// <summary>
        /// Shared automatic data segment
        /// </summary>
        SINGLEDATA = 0x0001,

        /// <summary>
        /// Instanced automatic data segment
        /// </summary>
        MULTIPLEDATA = 0x0002,

        /// <summary>
        /// Errors detected at link time, module will not load
        /// </summary>
        ErrorsDetectedAtLinkTime = 0x2000,

        /// <summary>
        /// Library module.
        /// The SS:SP information is invalid, CS:IP points
        /// to an initialization procedure that is called
        /// with AX equal to the module handle. This
        /// initialization procedure must perform a far
        /// return to the caller, with AX not equal to
        /// zero to indicate success, or AX equal to zero
        /// to indicate failure to initialize. DS is set
        /// to the library's data segment if the
        /// SINGLEDATA flag is set. Otherwise, DS is set
        /// to the caller's data segment.
        /// </summary>
        /// <remarks>
        /// A program or DLL can only contain dynamic
        /// links to executable files that have this
        /// library module flag set. One program cannot
        /// dynamic-link to another program.
        /// </remarks>
        LibraryModule = 0x8000,
    }

    public enum OperatingSystem : byte
    {
        WINDOWS = 0x02,
    }
}
