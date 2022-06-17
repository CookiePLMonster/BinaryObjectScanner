﻿/* vim: set sw=8: -*- Mode: C; tab-width: 8; indent-tabs-mode: t; c-basic-offset: 8 -*- */
/*
 * gsf-infile-MSOLE.c :
 *
 * Copyright (C) 2002-2004 Jody Goldberg (jody@gnome.org)
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of version 2.1 of the GNU Lesser General Public
 * License as published by the Free Software Foundation.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301
 * USA
 */

/*
 * [MS-CFB]: Compound File Binary File Format
 * http://msdn.microsoft.com/en-us/library/dd942138.aspx
 */

using System;
using System.Collections.Generic;
using System.IO;
using static LibGSF.GsfMSOleImpl;
using static LibGSF.GsfUtils;

namespace LibGSF.Input
{
    // TODO: Can this be made internal?
    public class MSOleBAT
    {
        #region Properties

        public uint[] Block { get; set; } = null;

        public uint NumBlocks { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Internal constructor
        /// </summary>
        internal MSOleBAT() { }

        /// <summary>
        /// Walk the linked list of the supplied block allocation table and build up a
        /// table for the list starting in <paramref name="block"/>.
        /// </summary>
        /// <param name="metabat">A meta bat to connect to the raw blocks (small or large)</param>
        /// <param name="size_guess">An optional guess as to how many blocks are in the file</param>
        /// <param name="block">The first block in the list.</param>
        /// <param name="res">Where to store the result.</param>
        /// <returns>True on error.</returns>
        public static bool Create(MSOleBAT metabat, int size_guess, uint block, out MSOleBAT res)
        {
            // NOTE : Only use size as a suggestion, sometimes it is wrong
            List<uint> bat = new List<uint>(size_guess);
            byte[] used = new byte[1 + metabat.NumBlocks / 8];

            while (block < metabat.NumBlocks)
            {
                // Catch cycles in the bat list
                if ((used[block / 8] & (1 << (int)(block & 0x7))) != 0)
                    break;

                used[block / 8] |= (byte)(1 << (int)(block & 0x7));

                bat.Add(block);
                block = metabat.Block[block];
            }

            res = new MSOleBAT
            {
                NumBlocks = (uint)bat.Count,
                Block = bat.ToArray(),
            };

            if (block != BAT_MAGIC_END_OF_CHAIN)
            {
                Console.Error.WriteLine("This OLE2 file is invalid.");
                Console.Error.WriteLine($"The Block Allocation Table for one of the streams had {block} instead of a terminator ({BAT_MAGIC_END_OF_CHAIN}).");
                Console.Error.WriteLine("We might still be able to extract some data, but you'll want to check the file.");
            }

            return false;
        }

        #endregion

        #region Functions

        public void Release()
        {
            if (Block == null)
                return;

            NumBlocks = 0;
            Block = null;
        }

        #endregion
    }

    // TODO: Can this be made internal?
    public class MSOleDirent
    {
        #region Properties

        public string Name { get; set; }

        public GsfMSOleSortingKey Key { get; set; }

        public int Index { get; set; }

        public int Size { get; set; }

        public bool UseSmallBlock { get; set; }

        public uint FirstBlock { get; set; }

        public bool IsDirectory { get; set; }

        public List<MSOleDirent> Children { get; set; } = new List<MSOleDirent>();

        /// <summary>
        /// 16 byte GUID used by some apps
        /// </summary>
        public byte[] ClassID { get; set; } = new byte[16];

        public DateTime? ModTime { get; set; }

        #endregion

        #region Functions

        public void Free()
        {
            Name = null;
            Key = null;

            foreach (MSOleDirent child in Children)
            {
                child.Free();
            }

            Children = null;
            ModTime = null;
        }

        #endregion
    }

    // TODO: Can this be made internal?
    public class MSOleInfo
    {
        #region Classes

        public class MSOleInfoBlock
        {
            public MSOleBAT Bat { get; set; }

            public int Shift { get; set; }

            public int Filter { get; set; }

            public int Size { get; set; }
        }

        #endregion

        #region Properties

        public MSOleInfoBlock BigBlock { get; set; }

        public MSOleInfoBlock SmallBlock { get; set; }

        public long MaxBlock { get; set; }

        /// <summary>
        /// Transition between small and big blocks
        /// </summary>
        public uint Threshold { get; set; }

        public uint SBatStart { get; set; }

        public uint NumSbat { get; set; }

        public MSOleDirent RootDir { get; set; }

        public GsfInput SmallBlockFile { get; set; }

        public int RefCount { get; set; }

        #endregion

        #region Functions

        public void Unref()
        {
            if (RefCount-- != 1)
                return;

            BigBlock.Bat.Release();
            SmallBlock.Bat.Release();
            if (RootDir != null)
            {
                RootDir.Free();
                RootDir = null;
            }

            if (SmallBlockFile != null)
                SmallBlockFile = null;
        }

        public MSOleInfo Ref()
        {
            RefCount++;
            return this;
        }

        #endregion
    }

    public class GsfInfileMSOle : GsfInfile
    {
        #region Properties

        public GsfInput Input { get; private set; } = null;

        public MSOleInfo Info { get; private set; } = null;

        public MSOleDirent DirectoryEntry { get; private set; }

        public MSOleBAT Bat { get; private set; }

        public long CurBlock { get; private set; } = BAT_MAGIC_UNUSED;

        /// <remarks>Actually `{ byte[] Buf, long BufSize }`</remarks>
        public byte[] Stream { get; private set; }

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Private constructor
        /// </summary>
        private GsfInfileMSOle() { }

        /// <summary>
        /// Opens the root directory of an MS OLE file.
        /// </summary>
        /// <param name="source">GsfInput</param>
        /// <param name="err">Optional place to store an error</param>
        /// <returns>The new ole file handler</returns>
        /// <remarks>This adds a reference to <paramref name="source"/>.</remarks>
        public static GsfInfile Create(GsfInput source, ref Exception err)
        {
            if (source == null)
                return null;

            GsfInfileMSOle ole = new GsfInfileMSOle
            {
                Input = GsfInputProxy.Create(source),
                Size = 0,
            };

            long calling_pos = source.CurrentOffset;
            if (ole.InitInfo(ref err))
            {
                // We do this so other kinds of archives can be tried.
                source.Seek(calling_pos, SeekOrigin.Begin);
                return null;
            }

            return ole;
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~GsfInfileMSOle()
        {
            if (Input != null)
                Input = null;

            if (Info != null && Info.SmallBlockFile != this)
            {
                Info.Unref();
                Info = null;
            }

            Bat.Release();
            Stream = null;
        }

        #endregion

        #region Functions

        /// <inheritdoc/>
        protected override GsfInput DupImpl(ref Exception err) => (Container as GsfInfileMSOle)?.CreateChild(DirectoryEntry, ref err);

        /// <inheritdoc/>
        protected override byte[] ReadImpl(int num_bytes, byte[] optional_buffer, int optional_buffer_ptr)
        {
            // Small block files are preload
            if (DirectoryEntry != null && DirectoryEntry.UseSmallBlock)
            {
                if (optional_buffer != null)
                {
                    Array.Copy(Stream, CurrentOffset, optional_buffer, optional_buffer_ptr, num_bytes);
                    return optional_buffer;
                }

                byte[] buffer = new byte[num_bytes];
                Array.Copy(Stream, CurrentOffset, buffer, 0, num_bytes);
                return buffer;
            }

            // GsfInput guarantees that num_bytes > 0
            long first_block = (CurrentOffset >> Info.BigBlock.Shift);
            long last_block = ((CurrentOffset + num_bytes - 1) >> Info.BigBlock.Shift);
            long offset = CurrentOffset & Info.BigBlock.Filter;

            if (last_block >= Bat.NumBlocks)
                return null;

            // Optimization: are all the raw blocks contiguous?
            long i = first_block;
            long raw_block = Bat.Block[i];

            //if (FALSE && first_block != last_block)
            //    Console.Error.WriteLine($"Check if {first_block}-{last_block} of {Bat.NumBlocks} are contiguous.");

            while (++i <= last_block && ++raw_block == Bat.Block[i]);

            if (i > last_block)
            {
                if (!SeekBlock(Bat.Block[first_block], offset))
                    return null;

                CurBlock = last_block;
                return Input.Read(num_bytes, optional_buffer, optional_buffer_ptr);
            }

            // Damn, we need to copy it block by block
            if (optional_buffer == null)
            {
                if (Stream.Length < num_bytes)
                    Stream = new byte[num_bytes];

                optional_buffer = Stream;
                optional_buffer_ptr = 0;
            }

            int ptr = optional_buffer_ptr; // optional_buffer[0]
            int count;
            for (i = first_block; i <= last_block; i++, ptr += count, num_bytes -= count)
            {
                count = (int)(Info.BigBlock.Size - offset);
                if (count > num_bytes)
                    count = num_bytes;

                if (!SeekBlock(Bat.Block[i], offset))
                    return null;

                if (Input.Read(count, optional_buffer, ptr) == null)
                    return null;

                offset = 0;
            }

            CurBlock = BAT_MAGIC_UNUSED;

            return optional_buffer;
        }

        /// <inheritdoc/>
        protected override bool SeekImpl(long offset, SeekOrigin whence)
        {
            CurBlock = BAT_MAGIC_UNUSED;
            return false;
        }

        /// <inheritdoc/>
        public override GsfInput ChildByIndex(int i, ref Exception error)
        {
            foreach (MSOleDirent dirent in DirectoryEntry.Children)
            {
                if (i-- <= 0)
                    return CreateChild(dirent, ref error);
            }

            return null;
        }

        /// <inheritdoc/>
        public override string NameByIndex(int i)
        {
            foreach (MSOleDirent dirent in DirectoryEntry.Children)
            {
                if (i-- <= 0)
                    return dirent.Name;
            }

            return null;
        }

        /// <inheritdoc/>
        public override GsfInput ChildByName(string name, ref Exception error)
        {
            foreach (MSOleDirent dirent in DirectoryEntry.Children)
            {
                if (dirent.Name != null && dirent.Name == name)
                    return CreateChild(dirent, ref error);
            }

            return null;
        }

        /// <inheritdoc/>
        public override int NumChildren()
        {
            if (DirectoryEntry == null)
                return -1;

            if (!DirectoryEntry.IsDirectory)
                return -1;

            return DirectoryEntry.Children.Count;
        }

        /// <summary>
        /// Retrieves the 16 byte indentifier (often a GUID in MS Windows apps)
        /// stored within the directory associated with @ole and stores it in <paramref name="res"/>.
        /// </summary>
        /// <param name="res">16 byte identifier (often a GUID in MS Windows apps)</param>
        /// <returns>True on success</returns>
        public bool GetClassID(byte[] res)
        {
            if (DirectoryEntry == null)
                return false;

            Array.Copy(DirectoryEntry.ClassID, res, DirectoryEntry.ClassID.Length);
            return true;
        }

        #endregion

        #region Utilities

        /// <returns>false on error.</returns>
        private bool SeekBlock(uint block, long offset)
        {
            if (block >= Info.MaxBlock)
                return false;

            // OLE_HEADER_SIZE is fixed at 512, but the sector containing the
            // header is padded out to BigBlock.Size (sector size) when BigBlock.Size > 512.
            if (Input.Seek(Math.Max(MSOleHeader.OLE_HEADER_SIZE, Info.BigBlock.Size) + (block << Info.BigBlock.Shift) + offset, SeekOrigin.Begin))
                return false;

            return true;
        }

        /// <summary>
        /// Read a block of data from the underlying input.
        /// </summary>
        /// <param name="block">Block number</param>
        /// <param name="buffer">Optionally null</param>
        /// <returns>Pointer to the buffer or null if there is an error or 0 bytes are requested.</returns>
        /// <remarks>Be really anal.</remarks>
        private byte[] GetBlock(uint block, byte[] buffer)
        {
            if (!SeekBlock(block, 0))
                return null;

            return Input.Read(Info.BigBlock.Size, buffer);
        }

        /// <summary>
        /// A small utility routine to read a set of references to bat blocks
        /// either from the OLE header, or a meta-bat block.
        /// </summary>
        /// <returns>A pointer to the element after the last position filled</returns>
        private int? ReadMetabat(uint[] bats, int batsPtr, uint max_bat, uint[] metabat, uint metabat_end)
        {
            for (int metabatPtr = 0; metabatPtr < metabat_end; metabatPtr++)
            {
                if (metabat[metabatPtr] != BAT_MAGIC_UNUSED)
                {
                    byte[] bat = GetBlock(metabat[metabatPtr], null);
                    if (bat == null)
                        return null;

                    int end = Info.BigBlock.Size;
                    for (int batPtr = 0; batPtr < end; batPtr += BAT_INDEX_SIZE, batsPtr++)
                    {
                        bats[batsPtr] = GSF_LE_GET_GUINT32(bat, batPtr);
                        if (bats[batsPtr] >= max_bat && bats[batsPtr] < BAT_MAGIC_METABAT)
                        {
                            Console.Error.WriteLine($"Invalid metabat item {bats[batsPtr]}");
                            return null;
                        }
                    }
                }
                else
                {
                    // Looks like something in the wild sometimes creates
                    // 'unused' entries in the metabat.  Let's assume that
                    // corresponds to lots of unused blocks
                    // http://bugzilla.gnome.org/show_bug.cgi?id=336858
                    uint i = (uint)(Info.BigBlock.Size / BAT_INDEX_SIZE);
                    while (i-- > 0)
                    {
                        bats[batsPtr++] = BAT_MAGIC_UNUSED;
                    }
                }
            }

            return batsPtr;
        }

        /// <summary>
        /// Copy some some raw data into an array of uint.
        /// </summary>
        private static void GetUnsignedInts(uint[] dst, byte[] src, int srcPtr, int num_bytes)
        {
            for (int dstPtr = 0; (num_bytes -= BAT_INDEX_SIZE) >= 0; srcPtr += BAT_INDEX_SIZE)
            {
                dst[dstPtr++] = GSF_LE_GET_GUINT32(src, srcPtr);
            }
        }

        private GsfInput GetSmallBlockFile()
        {
            if (Info.SmallBlockFile != null)
                return Info.SmallBlockFile;

            Exception err = null;
            Info.SmallBlockFile = CreateChild(Info.RootDir, ref err);
            if (Info.SmallBlockFile == null)
                return null;

            // Avoid creating a circular reference
            if (Info.SmallBlockFile is GsfInfileMSOle)
                (Info.SmallBlockFile as GsfInfileMSOle).Info.Unref();

            if (Info.SmallBlock.Bat.Block != null)
                return null;

            if (MSOleBAT.Create(Info.BigBlock.Bat, (int)Info.NumSbat, Info.SBatStart, out MSOleBAT meta_sbat))
                return null;

            Info.SmallBlock.Bat.NumBlocks = (uint)(meta_sbat.NumBlocks * (Info.BigBlock.Size / BAT_INDEX_SIZE));
            Info.SmallBlock.Bat.Block = new uint[Info.SmallBlock.Bat.NumBlocks];
            ReadMetabat(Info.SmallBlock.Bat.Block, 0, Info.SmallBlock.Bat.NumBlocks, meta_sbat.Block, meta_sbat.NumBlocks);
            meta_sbat.Release();

            return Info.SmallBlockFile;
        }

        private static int DirectoryEntryCompare(MSOleDirent a, MSOleDirent b) => SortingKeyCompare(a.Key, b.Key);

        private static DateTime? DateTimeFromFileTime(ulong ft) => ft == 0 ? (DateTime?)null : DateTime.FromFileTime((long)ft);

        /// <summary>
        /// Parse dirent number <paramref name="entry"/> and recursively handle its siblings and children.
        /// parent is optional.
        private MSOleDirent CreateDirectoryEntry(uint entry, MSOleDirent parent, bool[] seen_before)
        {
            if (entry >= MSOleDirectoryEntry.DIRENT_MAGIC_END)
                return null;

            if (entry > uint.MaxValue / MSOleDirectoryEntry.DIRENT_SIZE)
                return null;

            uint block = ((entry * MSOleDirectoryEntry.DIRENT_SIZE) >> Info.BigBlock.Shift);
            if (block >= Bat.NumBlocks)
                return null;

            if (seen_before[entry])
                return null;

            seen_before[entry] = true;

            byte[] data = GetBlock(Bat.Block[block], null);
            if (data == null)
                return null;

            int dataPtr = 0; // data[0]
            dataPtr += (int)((MSOleDirectoryEntry.DIRENT_SIZE * entry) % Info.BigBlock.Size);

            Exception err = null;
            MSOleDirectoryEntry directoryEntry = MSOleDirectoryEntry.Create(data, dataPtr, ref err);
            if (err != null)
            {
                Console.Error.WriteLine(err.Message);
                return null;
            }

            if (parent == null && directoryEntry.TYPE_FLAG != DIRENT_TYPE.DIRENT_TYPE_ROOTDIR)
            {
                // See bug 346118.
                Console.Error.WriteLine("Root directory is not marked as such.");
                directoryEntry.TYPE_FLAG = DIRENT_TYPE.DIRENT_TYPE_ROOTDIR;
            }

            // It looks like directory (and root directory) sizes are sometimes bogus
            if (!(directoryEntry.TYPE_FLAG == DIRENT_TYPE.DIRENT_TYPE_DIR || directoryEntry.TYPE_FLAG == DIRENT_TYPE.DIRENT_TYPE_ROOTDIR || directoryEntry.FILE_SIZE <= (uint)Input.Size))
                return null;

            MSOleDirent dirent = new MSOleDirent
            {
                Index = (int)entry,
                Size = (int)directoryEntry.FILE_SIZE,
                ModTime = DateTimeFromFileTime(directoryEntry.MODIFY_TIME),

                // Store the class id which is 16 byte identifier used by some apps
                ClassID = directoryEntry.CLSID,

                // Root dir is always big block
                UseSmallBlock = parent != null && (directoryEntry.FILE_SIZE < Info.Threshold),
                FirstBlock = directoryEntry.FIRSTBLOCK,
                IsDirectory = directoryEntry.IS_DIRECTORY,
                Children = new List<MSOleDirent>(),
            };

            dirent.Name = directoryEntry.NAME_STRING.TrimEnd('\0');

            // Be really anal in the face of screwups
            if (dirent.Name == null)
                dirent.Name = string.Empty;

            dirent.Key = GsfMSOleSortingKey.Create(dirent.Name);

            if (parent != null)
            {
                parent.Children.Add(dirent);
                parent.Children.Sort(DirectoryEntryCompare);
            }

            // NOTE : These links are a tree, not a linked list
            CreateDirectoryEntry(directoryEntry.PREV, parent, seen_before);
            CreateDirectoryEntry(directoryEntry.NEXT, parent, seen_before);

            if (dirent.IsDirectory)
                CreateDirectoryEntry(directoryEntry.CHILD, dirent, seen_before);
            else if (directoryEntry.CHILD != MSOleDirectoryEntry.DIRENT_MAGIC_END)
                Console.Error.WriteLine("A non directory stream with children ?");

            return dirent;
        }

        /// <summary>
        /// Utility routine to _partially_ replicate a file.  It does NOT copy the bat
        /// blocks, or init the dirent.
        /// </summary>
        private GsfInfileMSOle PartiallyDuplicate(ref Exception err)
        {
            GsfInput input = Input.Duplicate(ref err);
            if (input == null)
            {
                err = new Exception("Failed to duplicate input stream");
                return null;
            }

            GsfInfileMSOle dst = new GsfInfileMSOle
            {
                Input = input,
                Info = Info.Ref(),
                Stream = new byte[0],
            };

            return dst;
        }

        /// <summary>
        /// Read an OLE header and do some sanity checking
        /// along the way.
        /// </summary>
        /// <returns>True on error setting <paramref name="err"/> if it is supplied.</returns>
        private bool InitInfo(ref Exception err)
        {
            // Seek to the header
            if (Input.Seek(0, SeekOrigin.Begin))
            {
                err = new Exception("Cannot seek to header");
                return true;
            }

            byte[] header = Input.Read(MSOleHeader.OLE_HEADER_SIZE, null);
            if (header == null)
            {
                err = new Exception("Header could not be read");
                return true;
            }

            MSOleHeader headerImpl = MSOleHeader.Create(header, 0, ref err);
            if (headerImpl == null)
                return true;

            // Some sanity checks
            // 1) There should always be at least 1 BAT block
            // 2) It makes no sense to have a block larger than 2^31 for now.
            //    Maybe relax this later, but not much.
            if (6 > headerImpl.BB_SHIFT || headerImpl.BB_SHIFT >= 31 || headerImpl.SB_SHIFT > headerImpl.BB_SHIFT || (Input.Size >> headerImpl.BB_SHIFT) < 1)
            {
                err = new Exception("Unreasonable block sizes");
                return true;
            }

            MSOleInfo info = new MSOleInfo
            {
                BigBlock = new MSOleInfo.MSOleInfoBlock { Bat = new MSOleBAT() },
                SmallBlock = new MSOleInfo.MSOleInfoBlock { Bat = new MSOleBAT() },
            };

            info.RefCount           = 1;
            info.BigBlock.Shift     = headerImpl.BB_SHIFT;
            info.BigBlock.Size      = headerImpl.BB_SIZE;
            info.BigBlock.Filter    = headerImpl.BB_FILTER;
            info.SmallBlock.Shift   = headerImpl.SB_SHIFT;
            info.SmallBlock.Size    = headerImpl.SB_SIZE;
            info.SmallBlock.Filter  = headerImpl.SB_FILTER;
            info.Threshold          = headerImpl.THRESHOLD;
            info.SBatStart          = headerImpl.SBAT_START;
            info.NumSbat            = headerImpl.NUM_SBAT;
            info.MaxBlock = (Input.Size - MSOleHeader.OLE_HEADER_SIZE + info.BigBlock.Size - 1) / info.BigBlock.Size;
            info.SmallBlockFile = null;

            Info = info;

            if (info.NumSbat == 0 && info.SBatStart != BAT_MAGIC_END_OF_CHAIN && info.SBatStart != BAT_MAGIC_UNUSED)
                Console.Error.WriteLine("There are not supposed to be any blocks in the small block allocation table, yet there is a link to some.  Ignoring it.");

            uint[] metabat = null;
            uint last;
            int? ptr = null;

            // Very rough heuristic, just in case
            uint num_bat = headerImpl.NUM_BAT;
            if (num_bat < info.MaxBlock && headerImpl.NUM_SBAT < info.MaxBlock)
            {
                info.BigBlock.Bat.NumBlocks = (uint)(num_bat * (headerImpl.BB_SIZE / BAT_INDEX_SIZE));
                info.BigBlock.Bat.Block     = new uint[info.BigBlock.Bat.NumBlocks];

                metabat = new uint[Math.Max(headerImpl.BB_SIZE, MSOleHeader.OLE_HEADER_SIZE)];

                // Reading the elements invalidates this memory, make copy
                GetUnsignedInts(metabat, header, OLE_HEADER_START_BAT, MSOleHeader.OLE_HEADER_SIZE - OLE_HEADER_START_BAT);
                last = num_bat;
                if (last > OLE_HEADER_METABAT_SIZE)
                    last = OLE_HEADER_METABAT_SIZE;

                ptr = ReadMetabat(info.BigBlock.Bat.Block, 0, info.BigBlock.Bat.NumBlocks, metabat, last);
                num_bat -= last;
            }

            uint metabat_block = headerImpl.METABAT_BLOCK;
            uint num_metabat = headerImpl.NUM_METABAT;

            last = (uint)((info.BigBlock.Size - BAT_INDEX_SIZE) / BAT_INDEX_SIZE);
            while (ptr != null && num_metabat-- > 0)
            {
                byte[] tmp = GetBlock(metabat_block, null);
                if (tmp == null)
                {
                    ptr = null;
                    break;
                }

                // Reading the elements invalidates this memory, make copy
                GetUnsignedInts(metabat, tmp, 0, info.BigBlock.Size);

                if (num_metabat == 0)
                {
                    if (last < num_bat)
                    {
                        // There should be less that a full metabat block remaining
                        ptr = null;
                        break;
                    }

                    last = num_bat;
                }
                else if (num_metabat > 0)
                {
                    metabat_block = metabat[last];
                    if (num_bat < last)
                    {
                        // ::num_bat and ::num_metabat are
                        // inconsistent.  There are too many metabats
                        // for the bat count in the header.
                        ptr = null;
                        break;
                    }

                    num_bat -= last;
                }

                ptr = ReadMetabat(info.BigBlock.Bat.Block, ptr.Value, info.BigBlock.Bat.NumBlocks, metabat, last);
            }

            bool fail = (ptr == null);

            if (fail)
            {
                err = new Exception("Inconsistent block allocation table");
                return true;
            }

            // Read the directory's bat, we do not know the size
            if (MSOleBAT.Create(Info.BigBlock.Bat, 0, headerImpl.DIRENT_START, out MSOleBAT tempBat))
            {
                err = new Exception("Problems making block allocation table");
                return true;
            }

            Bat = tempBat;

            // Read the directory
            bool[] seen_before = new bool[(Bat.NumBlocks << info.BigBlock.Shift) * MSOleDirectoryEntry.DIRENT_SIZE + 1];
            DirectoryEntry = info.RootDir = CreateDirectoryEntry(0, null, seen_before);
            if (DirectoryEntry == null)
            {
                err = new Exception("Problems reading directory");
                return true;
            }

            // The spec says to ignore modtime for root object.  That doesn't
            // keep files from actually have a modtime there.
            ModTime = DirectoryEntry.ModTime;

            return false;
        }

        internal static int SortingKeyCompare(GsfMSOleSortingKey a, GsfMSOleSortingKey b)
        {
            long diff;

            // According to the docs length is more important than lexical order
            if (a.Length != b.Length)
                diff = a.Length - b.Length;
            else
                diff = a.Name.CompareTo(b.Name);

            // Note, that diff might not fit "int"
            return diff > 0 ? +1 : (diff < 0 ? -1 : 0);
        }

        private GsfInput CreateChild(MSOleDirent dirent, ref Exception err)
        {
            GsfInfileMSOle child = PartiallyDuplicate(ref err);
            if (child == null)
                return null;

            child.DirectoryEntry = dirent;
            child.Size = dirent.Size;
            child.ModTime = dirent.ModTime;

            // The root dirent defines the small block file
            if (dirent.Index != 0)
            {
                child.Name = dirent.Name;
                child.Container = this;

                if (dirent.IsDirectory)
                {
                    // Be wary.  It seems as if some implementations pretend that the
                    // directories contain data
                    child.Size = 0;
                    return child;
                }
            }

            MSOleInfo info = Info;

            MSOleBAT metabat;
            int size_guess;
            GsfInput sb_file = null;

            // Build the bat
            if (dirent.UseSmallBlock)
            {
                metabat = info.SmallBlock.Bat;
                size_guess = dirent.Size >> info.SmallBlock.Shift;
                sb_file = GetSmallBlockFile();
                if (sb_file == null)
                {
                    err = new Exception("Failed to access child");
                    return null;
                }
            }
            else
            {
                metabat = info.BigBlock.Bat;
                size_guess = dirent.Size >> info.BigBlock.Shift;
            }

            if (MSOleBAT.Create(metabat, size_guess + 1, dirent.FirstBlock, out MSOleBAT tempBat))
                return null;

            child.Bat = tempBat;

            if (dirent.UseSmallBlock)
            {
                if (sb_file == null)
                    return null;

                int remaining = dirent.Size;
                child.Stream = new byte[remaining];

                for (uint i = 0; remaining > 0 && i < child.Bat.NumBlocks; i++, remaining -= info.SmallBlock.Size)
                {
                    if (sb_file.Seek(child.Bat.Block[i] << info.SmallBlock.Shift, SeekOrigin.Begin)
                        || sb_file.Read(Math.Min(remaining, info.SmallBlock.Size), child.Stream, (int)(i << info.SmallBlock.Shift)) == null)
                    {
                        Console.Error.WriteLine($"Failure reading block {i} for '{dirent.Name}'");
                        err = new Exception("Failure reading block");
                        return null;
                    }
                }

                if (remaining > 0)
                {
                    err = new Exception("Insufficient blocks");
                    Console.Error.WriteLine($"Small-block file '{dirent.Name}' has insufficient blocks ({child.Bat.NumBlocks}) for the stated size ({dirent.Size})");
                    return null;
                }
            }

            return child;
        }

        #endregion
    }
}
