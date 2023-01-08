using System;
using System.IO;

namespace BurnOutSharp.Wrappers
{
    public class N3DS : WrapperBase
    {
        #region Pass-Through Properties

        #region Header

        #region Common to all NCSD files

        /// <inheritdoc cref="Models.N3DS.NCSDHeader.RSA2048Signature"/>
        public byte[] RSA2048Signature => _cart.Header.RSA2048Signature;

        /// <inheritdoc cref="Models.N3DS.NCSDHeader.MagicNumber"/>
        public string MagicNumber => _cart.Header.MagicNumber;

        /// <inheritdoc cref="Models.N3DS.NCSDHeader.ImageSizeInMediaUnits"/>
        public uint ImageSizeInMediaUnits => _cart.Header.ImageSizeInMediaUnits;

        /// <inheritdoc cref="Models.N3DS.NCSDHeader.MediaId"/>
        public byte[] MediaId => _cart.Header.MediaId;

        /// <inheritdoc cref="Models.N3DS.NCSDHeader.PartitionsFSType"/>
        public Models.N3DS.FilesystemType PartitionsFSType => _cart.Header.PartitionsFSType;

        /// <inheritdoc cref="Models.N3DS.NCSDHeader.PartitionsCryptType"/>
        public byte[] PartitionsCryptType => _cart.Header.PartitionsCryptType;

        /// <inheritdoc cref="Models.N3DS.NCSDHeader.PartitionsTable"/>
        public Models.N3DS.PartitionTableEntry[] PartitionsTable => _cart.Header.PartitionsTable;

        #endregion

        #region CTR Cart Image (CCI) Specific

        /// <inheritdoc cref="Models.N3DS.NCSDHeader.ExheaderHash"/>
        public byte[] ExheaderHash => _cart.Header.ExheaderHash;

        /// <inheritdoc cref="Models.N3DS.NCSDHeader.AdditionalHeaderSize"/>
        public uint AdditionalHeaderSize => _cart.Header.AdditionalHeaderSize;

        /// <inheritdoc cref="Models.N3DS.NCSDHeader.SectorZeroOffset"/>
        public uint SectorZeroOffset => _cart.Header.SectorZeroOffset;

        /// <inheritdoc cref="Models.N3DS.NCSDHeader.PartitionFlags"/>
        public byte[] PartitionFlags => _cart.Header.PartitionFlags;

        /// <inheritdoc cref="Models.N3DS.NCSDHeader.PartitionIdTable"/>
        public ulong[] PartitionIdTable => _cart.Header.PartitionIdTable;

        /// <inheritdoc cref="Models.N3DS.NCSDHeader.Reserved1"/>
        public byte[] Reserved1 => _cart.Header.Reserved1;

        /// <inheritdoc cref="Models.N3DS.NCSDHeader.Reserved2"/>
        public byte[] Reserved2 => _cart.Header.Reserved2;

        /// <inheritdoc cref="Models.N3DS.NCSDHeader.FirmUpdateByte1"/>
        public byte FirmUpdateByte1 => _cart.Header.FirmUpdateByte1;

        /// <inheritdoc cref="Models.N3DS.NCSDHeader.FirmUpdateByte2"/>
        public byte FirmUpdateByte2 => _cart.Header.FirmUpdateByte2;

        #endregion

        #region Raw NAND Format Specific

        /// <inheritdoc cref="Models.N3DS.NCSDHeader.Unknown"/>
        public byte[] Unknown => _cart.Header.Unknown;

        /// <inheritdoc cref="Models.N3DS.NCSDHeader.EncryptedMBR"/>
        public byte[] EncryptedMBR => _cart.Header.EncryptedMBR;

        #endregion

        #endregion

        #region Card Info Header

        /// <inheritdoc cref="Models.N3DS.CardInfoHeader.WritableAddressMediaUnits"/>
        public uint CIH_WritableAddressMediaUnits => _cart.CardInfoHeader.WritableAddressMediaUnits;

        /// <inheritdoc cref="Models.N3DS.CardInfoHeader.CardInfoBitmask"/>
        public uint CIH_CardInfoBitmask => _cart.CardInfoHeader.CardInfoBitmask;

        /// <inheritdoc cref="Models.N3DS.CardInfoHeader.Reserved1"/>
        public byte[] CIH_Reserved1 => _cart.CardInfoHeader.Reserved1;

        /// <inheritdoc cref="Models.N3DS.CardInfoHeader.FilledSize"/>
        public uint CIH_FilledSize => _cart.CardInfoHeader.FilledSize;

        /// <inheritdoc cref="Models.N3DS.CardInfoHeader.Reserved2"/>
        public byte[] CIH_Reserved2 => _cart.CardInfoHeader.Reserved2;

        /// <inheritdoc cref="Models.N3DS.CardInfoHeader.TitleVersion"/>
        public ushort CIH_TitleVersion => _cart.CardInfoHeader.TitleVersion;

        /// <inheritdoc cref="Models.N3DS.CardInfoHeader.CardRevision"/>
        public ushort CIH_CardRevision => _cart.CardInfoHeader.CardRevision;

        /// <inheritdoc cref="Models.N3DS.CardInfoHeader.Reserved3"/>
        public byte[] CIH_Reserved3 => _cart.CardInfoHeader.Reserved3;

        /// <inheritdoc cref="Models.N3DS.CardInfoHeader.CVerTitleID"/>
        public byte[] CIH_CVerTitleID => _cart.CardInfoHeader.CVerTitleID;

        /// <inheritdoc cref="Models.N3DS.CardInfoHeader.CVerVersionNumber"/>
        public ushort CIH_CVerVersionNumber => _cart.CardInfoHeader.CVerVersionNumber;

        /// <inheritdoc cref="Models.N3DS.CardInfoHeader.Reserved4"/>
        public byte[] CIH_Reserved4 => _cart.CardInfoHeader.Reserved4;

        #endregion

        #region Development Card Info Header

        #region Initial Data

        /// <inheritdoc cref="Models.N3DS.InitialData.CardSeedKeyY"/>
        public byte[] DCIH_ID_CardSeedKeyY => _cart.DevelopmentCardInfoHeader?.InitialData?.CardSeedKeyY;

        /// <inheritdoc cref="Models.N3DS.InitialData.EncryptedCardSeed"/>
        public byte[] DCIH_ID_EncryptedCardSeed => _cart.DevelopmentCardInfoHeader?.InitialData?.EncryptedCardSeed;

        /// <inheritdoc cref="Models.N3DS.InitialData.CardSeedAESMAC"/>
        public byte[] DCIH_ID_CardSeedAESMAC => _cart.DevelopmentCardInfoHeader?.InitialData?.CardSeedAESMAC;

        /// <inheritdoc cref="Models.N3DS.InitialData.CardSeedNonce"/>
        public byte[] DCIH_ID_CardSeedNonce => _cart.DevelopmentCardInfoHeader?.InitialData?.CardSeedNonce;

        /// <inheritdoc cref="Models.N3DS.InitialData.Reserved3"/>
        public byte[] DCIH_ID_Reserved => _cart.DevelopmentCardInfoHeader?.InitialData?.Reserved;

        /// <inheritdoc cref="Models.N3DS.InitialData.BackupHeader"/>
        public Models.N3DS.NCCHHeader DCIH_ID_BackupHeader => _cart.DevelopmentCardInfoHeader?.InitialData?.BackupHeader;

        #endregion

        /// <inheritdoc cref="Models.N3DS.DevelopmentCardInfoHeader.CardDeviceReserved1"/>
        public byte[] DCIH_CardDeviceReserved1 => _cart.DevelopmentCardInfoHeader?.CardDeviceReserved1;

        /// <inheritdoc cref="Models.N3DS.DevelopmentCardInfoHeader.TitleKey"/>
        public byte[] DCIH_TitleKey => _cart.DevelopmentCardInfoHeader?.TitleKey;

        /// <inheritdoc cref="Models.N3DS.DevelopmentCardInfoHeader.CardDeviceReserved2"/>
        public byte[] DCIH_CardDeviceReserved2 => _cart.DevelopmentCardInfoHeader?.CardDeviceReserved2;

        #region Test Data

        /// <inheritdoc cref="Models.N3DS.TestData.Signature"/>
        public byte[] DCIH_TD_Signature => _cart.DevelopmentCardInfoHeader?.TestData?.Signature;

        /// <inheritdoc cref="Models.N3DS.TestData.AscendingByteSequence"/>
        public byte[] DCIH_TD_AscendingByteSequence => _cart.DevelopmentCardInfoHeader?.TestData?.AscendingByteSequence;

        /// <inheritdoc cref="Models.N3DS.TestData.DescendingByteSequence"/>
        public byte[] DCIH_TD_DescendingByteSequence => _cart.DevelopmentCardInfoHeader?.TestData?.DescendingByteSequence;

        /// <inheritdoc cref="Models.N3DS.TestData.Filled00"/>
        public byte[] DCIH_TD_Filled00 => _cart.DevelopmentCardInfoHeader?.TestData?.Filled00;

        /// <inheritdoc cref="Models.N3DS.TestData.FilledFF"/>
        public byte[] DCIH_TD_FilledFF => _cart.DevelopmentCardInfoHeader?.TestData?.FilledFF;

        /// <inheritdoc cref="Models.N3DS.TestData.Filled0F"/>
        public byte[] DCIH_TD_Filled0F => _cart.DevelopmentCardInfoHeader?.TestData?.Filled0F;

        /// <inheritdoc cref="Models.N3DS.TestData.FilledF0"/>
        public byte[] DCIH_TD_FilledF0 => _cart.DevelopmentCardInfoHeader?.TestData?.FilledF0;

        /// <inheritdoc cref="Models.N3DS.TestData.Filled55"/>
        public byte[] DCIH_TD_Filled55 => _cart.DevelopmentCardInfoHeader?.TestData?.Filled55;

        /// <inheritdoc cref="Models.N3DS.TestData.FilledAA"/>
        public byte[] DCIH_TD_FilledAA => _cart.DevelopmentCardInfoHeader?.TestData?.FilledAA;

        /// <inheritdoc cref="Models.N3DS.TestData.FinalByte"/>
        public byte? DCIH_TD_FinalByte => _cart.DevelopmentCardInfoHeader?.TestData?.FinalByte;

        #endregion

        #endregion

        #region Partitions

        /// <inheritdoc cref="Models.N3DS.Cart.Partitions"/>
        public Models.N3DS.NCCHHeader[] Partitions => _cart.Partitions;

        #endregion

        #region Extended Headers

        /// <inheritdoc cref="Models.N3DS.Cart.ExtendedHeaders"/>
        public Models.N3DS.NCCHExtendedHeader[] ExtendedHeaders => _cart.ExtendedHeaders;

        #endregion

        #region ExeFS Headers

        /// <inheritdoc cref="Models.N3DS.Cart.ExeFSHeaders"/>
        public Models.N3DS.ExeFSHeader[] ExeFSHeaders => _cart.ExeFSHeaders;

        #endregion

        #region RomFS Headers

        /// <inheritdoc cref="Models.N3DS.Cart.RomFSHeaders"/>
        public Models.N3DS.RomFSHeader[] RomFSHeaders => _cart.RomFSHeaders;

        #endregion

        #endregion

        #region Instance Variables

        /// <summary>
        /// Internal representation of the cart
        /// </summary>
        private Models.N3DS.Cart _cart;

        #endregion

        #region Constructors

        /// <summary>
        /// Private constructor
        /// </summary>
        private N3DS() { }

        /// <summary>
        /// Create a 3DS cart image from a byte array and offset
        /// </summary>
        /// <param name="data">Byte array representing the archive</param>
        /// <param name="offset">Offset within the array to parse</param>
        /// <returns>A 3DS cart image wrapper on success, null on failure</returns>
        public static N3DS Create(byte[] data, int offset)
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and use that
            MemoryStream dataStream = new MemoryStream(data, offset, data.Length - offset);
            return Create(dataStream);
        }

        /// <summary>
        /// Create a 3DS cart image from a Stream
        /// </summary>
        /// <param name="data">Stream representing the archive</param>
        /// <returns>A 3DS cart image wrapper on success, null on failure</returns>
        public static N3DS Create(Stream data)
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            var archive = Builders.N3DS.ParseCart(data);
            if (archive == null)
                return null;

            var wrapper = new N3DS
            {
                _cart = archive,
                _dataSource = DataSource.Stream,
                _streamData = data,
            };
            return wrapper;
        }

        #endregion

        #region Printing

        /// <inheritdoc/>
        public override void Print()
        {
            Console.WriteLine("3DS Cart Information:");
            Console.WriteLine("-------------------------");
            Console.WriteLine();

            PrintNCSDHeader();
            PrintCardInfoHeader();
            PrintDevelopmentCardInfoHeader();
            PrintPartitions();
            PrintExtendedHeaders();
            PrintExeFSHeaders();
            PrintRomFSHeaders();
        }

        /// <summary>
        /// Print NCSD header information
        /// </summary>
        private void PrintNCSDHeader()
        {
            Console.WriteLine("  NCSD Header Information:");
            Console.WriteLine("  -------------------------");
            Console.WriteLine($"  RSA-2048 SHA-256 signature: {BitConverter.ToString(RSA2048Signature).Replace('-', ' ')}");
            Console.WriteLine($"  Magic number: {MagicNumber}");
            Console.WriteLine($"  Image size in media units: {ImageSizeInMediaUnits}");
            Console.WriteLine($"  Media ID: {BitConverter.ToString(MediaId).Replace('-', ' ')}");
            Console.WriteLine($"  Partitions filesystem type: {PartitionsFSType}");
            Console.WriteLine($"  Partitions crypt type: {BitConverter.ToString(PartitionsCryptType).Replace('-', ' ')}");
            Console.WriteLine();

            Console.WriteLine($"  Partition table:");
            Console.WriteLine("  -------------------------");
            for (int i = 0; i < PartitionsTable.Length; i++)
            {
                var partitionTableEntry = PartitionsTable[i];
                Console.WriteLine($"  Partition table entry {i}");
                Console.WriteLine($"    Offset: {partitionTableEntry.Offset}");
                Console.WriteLine($"    Length: {partitionTableEntry.Length}");
            }
            Console.WriteLine();

            // If we have a cart image
            if (PartitionsFSType == Models.N3DS.FilesystemType.Normal || PartitionsFSType == Models.N3DS.FilesystemType.None)
            {
                Console.WriteLine($"  Exheader SHA-256 hash: {BitConverter.ToString(ExheaderHash).Replace('-', ' ')}");
                Console.WriteLine($"  Additional header size: {AdditionalHeaderSize}");
                Console.WriteLine($"  Sector zero offset: {SectorZeroOffset}");
                Console.WriteLine($"  Partition flags: {BitConverter.ToString(PartitionFlags).Replace('-', ' ')}");
                Console.WriteLine();

                Console.WriteLine($"  Partition ID table:");
                Console.WriteLine("  -------------------------");
                for (int i = 0; i < PartitionIdTable.Length; i++)
                {
                    Console.WriteLine($"  Partition {i} ID: {PartitionIdTable[i]}");
                }
                Console.WriteLine();

                Console.WriteLine($"  Reserved 1: {BitConverter.ToString(Reserved1).Replace('-', ' ')}");
                Console.WriteLine($"  Reserved 2: {BitConverter.ToString(Reserved2).Replace('-', ' ')}");
                Console.WriteLine($"  Firmware update byte 1: {FirmUpdateByte1}");
                Console.WriteLine($"  Firmware update byte 2: {FirmUpdateByte2}");
            }

            // If we have a firmware image
            else if (PartitionsFSType == Models.N3DS.FilesystemType.FIRM)
            {
                Console.WriteLine($"  Unknown: {BitConverter.ToString(Unknown).Replace('-', ' ')}");
                Console.WriteLine($"  Encrypted MBR: {BitConverter.ToString(EncryptedMBR).Replace('-', ' ')}");
            }

            Console.WriteLine();
        }

        /// <summary>
        /// Print card info header information
        /// </summary>
        private void PrintCardInfoHeader()
        {
            Console.WriteLine("  Card Info Header Information:");
            Console.WriteLine("  -------------------------");
            Console.WriteLine($"  Writable address in media units: {CIH_WritableAddressMediaUnits}");
            Console.WriteLine($"  Card info bitmask: {CIH_CardInfoBitmask}");
            Console.WriteLine($"  Reserved 1: {BitConverter.ToString(CIH_Reserved1).Replace('-', ' ')}");
            Console.WriteLine($"  Filled size of cartridge: {CIH_FilledSize}");
            Console.WriteLine($"  Reserved 2: {BitConverter.ToString(CIH_Reserved2).Replace('-', ' ')}");
            Console.WriteLine($"  Title version: {CIH_TitleVersion}");
            Console.WriteLine($"  Card revision: {CIH_CardRevision}");
            Console.WriteLine($"  Reserved 3: {BitConverter.ToString(CIH_Reserved3).Replace('-', ' ')}");
            Console.WriteLine($"  Title ID of CVer in included update partition: {BitConverter.ToString(CIH_CVerTitleID).Replace('-', ' ')}");
            Console.WriteLine($"  Version number of CVer in included update partition: {CIH_CVerVersionNumber}");
            Console.WriteLine($"  Reserved 4: {BitConverter.ToString(CIH_Reserved4).Replace('-', ' ')}");
            Console.WriteLine();
        }

        /// <summary>
        /// Print development card info header information
        /// </summary>
        private void PrintDevelopmentCardInfoHeader()
        {
            Console.WriteLine("  Development Card Info Header Information:");
            Console.WriteLine("  -------------------------");
            if (_cart.DevelopmentCardInfoHeader == null)
            {
                Console.WriteLine("  No development card info header");
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine("  Initial Data:");
                Console.WriteLine("  -------------------------");
                Console.WriteLine($"  Card seed keyY: {BitConverter.ToString(DCIH_ID_CardSeedKeyY).Replace('-', ' ')}");
                Console.WriteLine($"  Encrypted card seed: {BitConverter.ToString(DCIH_ID_EncryptedCardSeed).Replace('-', ' ')}");
                Console.WriteLine($"  Card seed AES-MAC: {BitConverter.ToString(DCIH_ID_CardSeedAESMAC).Replace('-', ' ')}");
                Console.WriteLine($"  Card seed nonce: {BitConverter.ToString(DCIH_ID_CardSeedNonce).Replace('-', ' ')}");
                Console.WriteLine($"  Reserved: {BitConverter.ToString(DCIH_ID_Reserved).Replace('-', ' ')}");
                Console.WriteLine();

                Console.WriteLine("    Backup Header:");
                Console.WriteLine("    -------------------------");
                Console.WriteLine($"    Magic ID: {DCIH_ID_BackupHeader.MagicID}");
                Console.WriteLine($"    Content size in media units: {DCIH_ID_BackupHeader.ContentSizeInMediaUnits}");
                Console.WriteLine($"    Partition ID: {DCIH_ID_BackupHeader.PartitionId}");
                Console.WriteLine($"    Maker code: {DCIH_ID_BackupHeader.MakerCode}");
                Console.WriteLine($"    Version: {DCIH_ID_BackupHeader.Version}");
                Console.WriteLine($"    Verification hash: {DCIH_ID_BackupHeader.VerificationHash}");
                Console.WriteLine($"    Program ID: {BitConverter.ToString(DCIH_ID_BackupHeader.ProgramId).Replace('-', ' ')}");
                Console.WriteLine($"    Reserved 1: {BitConverter.ToString(DCIH_ID_BackupHeader.Reserved1).Replace('-', ' ')}");
                Console.WriteLine($"    Logo region SHA-256 hash: {BitConverter.ToString(DCIH_ID_BackupHeader.LogoRegionHash).Replace('-', ' ')}");
                Console.WriteLine($"    Product code: {DCIH_ID_BackupHeader.ProductCode}");
                Console.WriteLine($"    Extended header SHA-256 hash: {BitConverter.ToString(DCIH_ID_BackupHeader.ExtendedHeaderHash).Replace('-', ' ')}");
                Console.WriteLine($"    Extended header size in bytes: {DCIH_ID_BackupHeader.ExtendedHeaderSizeInBytes}");
                Console.WriteLine($"    Reserved 2: {BitConverter.ToString(DCIH_ID_BackupHeader.Reserved2).Replace('-', ' ')}");
                Console.WriteLine($"    Flags: {DCIH_ID_BackupHeader.Flags}");
                Console.WriteLine($"    Plain region offset, in media units: {DCIH_ID_BackupHeader.PlainRegionOffsetInMediaUnits}");
                Console.WriteLine($"    Plain region size, in media units: {DCIH_ID_BackupHeader.PlainRegionSizeInMediaUnits}");
                Console.WriteLine($"    Logo region offset, in media units: {DCIH_ID_BackupHeader.LogoRegionOffsetInMediaUnits}");
                Console.WriteLine($"    Logo region size, in media units: {DCIH_ID_BackupHeader.LogoRegionSizeInMediaUnits}");
                Console.WriteLine($"    ExeFS offset, in media units: {DCIH_ID_BackupHeader.ExeFSOffsetInMediaUnits}");
                Console.WriteLine($"    ExeFS size, in media units: {DCIH_ID_BackupHeader.ExeFSSizeInMediaUnits}");
                Console.WriteLine($"    ExeFS hash region size, in media units: {DCIH_ID_BackupHeader.ExeFSHashRegionSizeInMediaUnits}");
                Console.WriteLine($"    Reserved 3: {BitConverter.ToString(DCIH_ID_BackupHeader.Reserved3).Replace('-', ' ')}");
                Console.WriteLine($"    RomFS offset, in media units: {DCIH_ID_BackupHeader.RomFSOffsetInMediaUnits}");
                Console.WriteLine($"    RomFS size, in media units: {DCIH_ID_BackupHeader.RomFSSizeInMediaUnits}");
                Console.WriteLine($"    RomFS hash region size, in media units: {DCIH_ID_BackupHeader.RomFSHashRegionSizeInMediaUnits}");
                Console.WriteLine($"    Reserved 4: {BitConverter.ToString(DCIH_ID_BackupHeader.Reserved4).Replace('-', ' ')}");
                Console.WriteLine($"    ExeFS superblock SHA-256 hash: {BitConverter.ToString(DCIH_ID_BackupHeader.ExeFSSuperblockHash).Replace('-', ' ')}");
                Console.WriteLine($"    RomFS superblock SHA-256 hash: {BitConverter.ToString(DCIH_ID_BackupHeader.RomFSSuperblockHash).Replace('-', ' ')}");

                // TODO: Print Backup Header?
                Console.WriteLine();

                Console.WriteLine($"  Card device reserved 1: {BitConverter.ToString(DCIH_CardDeviceReserved1).Replace('-', ' ')}");
                Console.WriteLine($"  Title key: {BitConverter.ToString(DCIH_TitleKey).Replace('-', ' ')}");
                Console.WriteLine($"  Card device reserved 2: {BitConverter.ToString(DCIH_CardDeviceReserved2).Replace('-', ' ')}");
                Console.WriteLine();

                Console.WriteLine("  Test Data:");
                Console.WriteLine("  -------------------------");
                Console.WriteLine($"  Signature: {BitConverter.ToString(DCIH_TD_Signature).Replace('-', ' ')}");
                Console.WriteLine($"  Ascending byte sequence: {BitConverter.ToString(DCIH_TD_AscendingByteSequence).Replace('-', ' ')}");
                Console.WriteLine($"  Descending byte sequence: {BitConverter.ToString(DCIH_TD_DescendingByteSequence).Replace('-', ' ')}");
                Console.WriteLine($"  Filled with 00: {BitConverter.ToString(DCIH_TD_Filled00).Replace('-', ' ')}");
                Console.WriteLine($"  Filled with FF: {BitConverter.ToString(DCIH_TD_FilledFF).Replace('-', ' ')}");
                Console.WriteLine($"  Filled with 0F: {BitConverter.ToString(DCIH_TD_Filled0F).Replace('-', ' ')}");
                Console.WriteLine($"  Filled with F0: {BitConverter.ToString(DCIH_TD_FilledF0).Replace('-', ' ')}");
                Console.WriteLine($"  Filled with 55: {BitConverter.ToString(DCIH_TD_Filled55).Replace('-', ' ')}");
                Console.WriteLine($"  Filled with AA: {BitConverter.ToString(DCIH_TD_FilledAA).Replace('-', ' ')}");
                Console.WriteLine($"  Final byte: {DCIH_TD_FinalByte}");
            }

            Console.WriteLine();
        }

        /// <summary>
        /// Print NCCH partition header information
        /// </summary>
        private void PrintPartitions()
        {
            Console.WriteLine("  NCCH Partition Header Information:");
            Console.WriteLine("  -------------------------");
            if (Partitions == null || Partitions.Length == 0)
            {
                Console.WriteLine("  No NCCH partition headers");
            }
            else
            {
                for (int i = 0; i < Partitions.Length; i++)
                {
                    var partitionHeader = Partitions[i];
                    Console.WriteLine($"  NCCH Partition Header {i}");
                    if (partitionHeader.MagicID == string.Empty)
                    {
                        Console.WriteLine($"    Empty partition, no data can be parsed");
                    }
                    else if (partitionHeader.MagicID != Models.N3DS.Constants.NCCHMagicNumber)
                    {
                        Console.WriteLine($"    Unrecognized partition data, no data can be parsed");
                    }
                    else
                    {
                        Console.WriteLine($"    RSA-2048 SHA-256 signature: {BitConverter.ToString(partitionHeader.RSA2048Signature).Replace('-', ' ')}");
                        Console.WriteLine($"    Magic ID: {partitionHeader.MagicID}");
                        Console.WriteLine($"    Content size in media units: {partitionHeader.ContentSizeInMediaUnits}");
                        Console.WriteLine($"    Partition ID: {partitionHeader.PartitionId}");
                        Console.WriteLine($"    Maker code: {partitionHeader.MakerCode}");
                        Console.WriteLine($"    Version: {partitionHeader.Version}");
                        Console.WriteLine($"    Verification hash: {partitionHeader.VerificationHash}");
                        Console.WriteLine($"    Program ID: {BitConverter.ToString(partitionHeader.ProgramId).Replace('-', ' ')}");
                        Console.WriteLine($"    Reserved 1: {BitConverter.ToString(partitionHeader.Reserved1).Replace('-', ' ')}");
                        Console.WriteLine($"    Logo region SHA-256 hash: {BitConverter.ToString(partitionHeader.LogoRegionHash).Replace('-', ' ')}");
                        Console.WriteLine($"    Product code: {partitionHeader.ProductCode}");
                        Console.WriteLine($"    Extended header SHA-256 hash: {BitConverter.ToString(partitionHeader.ExtendedHeaderHash).Replace('-', ' ')}");
                        Console.WriteLine($"    Extended header size in bytes: {partitionHeader.ExtendedHeaderSizeInBytes}");
                        Console.WriteLine($"    Reserved 2: {BitConverter.ToString(partitionHeader.Reserved2).Replace('-', ' ')}");
                        Console.WriteLine("    Flags:");
                        Console.WriteLine($"      Reserved 0: {partitionHeader.Flags.Reserved0}");
                        Console.WriteLine($"      Reserved 1: {partitionHeader.Flags.Reserved1}");
                        Console.WriteLine($"      Reserved 2: {partitionHeader.Flags.Reserved2}");
                        Console.WriteLine($"      Crypto method: {partitionHeader.Flags.CryptoMethod}");
                        Console.WriteLine($"      Content platform: {partitionHeader.Flags.ContentPlatform}");
                        Console.WriteLine($"      Content type: {partitionHeader.Flags.MediaPlatformIndex}");
                        Console.WriteLine($"      Content unit size: {partitionHeader.Flags.ContentUnitSize}");
                        Console.WriteLine($"      Bitmasks: {partitionHeader.Flags.BitMasks}");
                        Console.WriteLine($"    Plain region offset, in media units: {partitionHeader.PlainRegionOffsetInMediaUnits}");
                        Console.WriteLine($"    Plain region size, in media units: {partitionHeader.PlainRegionSizeInMediaUnits}");
                        Console.WriteLine($"    Logo region offset, in media units: {partitionHeader.LogoRegionOffsetInMediaUnits}");
                        Console.WriteLine($"    Logo region size, in media units: {partitionHeader.LogoRegionSizeInMediaUnits}");
                        Console.WriteLine($"    ExeFS offset, in media units: {partitionHeader.ExeFSOffsetInMediaUnits}");
                        Console.WriteLine($"    ExeFS size, in media units: {partitionHeader.ExeFSSizeInMediaUnits}");
                        Console.WriteLine($"    ExeFS hash region size, in media units: {partitionHeader.ExeFSHashRegionSizeInMediaUnits}");
                        Console.WriteLine($"    Reserved 3: {BitConverter.ToString(partitionHeader.Reserved3).Replace('-', ' ')}");
                        Console.WriteLine($"    RomFS offset, in media units: {partitionHeader.RomFSOffsetInMediaUnits}");
                        Console.WriteLine($"    RomFS size, in media units: {partitionHeader.RomFSSizeInMediaUnits}");
                        Console.WriteLine($"    RomFS hash region size, in media units: {partitionHeader.RomFSHashRegionSizeInMediaUnits}");
                        Console.WriteLine($"    Reserved 4: {BitConverter.ToString(partitionHeader.Reserved4).Replace('-', ' ')}");
                        Console.WriteLine($"    ExeFS superblock SHA-256 hash: {BitConverter.ToString(partitionHeader.ExeFSSuperblockHash).Replace('-', ' ')}");
                        Console.WriteLine($"    RomFS superblock SHA-256 hash: {BitConverter.ToString(partitionHeader.RomFSSuperblockHash).Replace('-', ' ')}");
                    }
                }
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Print NCCH extended header information
        /// </summary>
        private void PrintExtendedHeaders()
        {
            Console.WriteLine("  NCCH Extended Header Information:");
            Console.WriteLine("  -------------------------");
            if (ExtendedHeaders == null || ExtendedHeaders.Length == 0)
            {
                Console.WriteLine("  No NCCH extended headers");
            }
            else
            {
                for (int i = 0; i < ExtendedHeaders.Length; i++)
                {
                    var extendedHeader = ExtendedHeaders[i];
                    Console.WriteLine($"  NCCH Extended Header {i}");
                    if (extendedHeader == null)
                    {
                        Console.WriteLine($"    Unrecognized partition data, no data can be parsed");
                    }
                    else
                    {
                        Console.WriteLine($"    System control info:");
                        Console.WriteLine($"      Application title: {extendedHeader.SCI.ApplicationTitle}");
                        Console.WriteLine($"      Reserved 1: {BitConverter.ToString(extendedHeader.SCI.Reserved1).Replace('-', ' ')}");
                        Console.WriteLine($"      Flag: {extendedHeader.SCI.Flag}");
                        Console.WriteLine($"      Remaster version: {extendedHeader.SCI.RemasterVersion}");

                        Console.WriteLine($"      Text code set info:");
                        Console.WriteLine($"        Address: {extendedHeader.SCI.TextCodeSetInfo.Address}");
                        Console.WriteLine($"        Physical region size (in page-multiples): {extendedHeader.SCI.TextCodeSetInfo.PhysicalRegionSizeInPages}");
                        Console.WriteLine($"        Size (in bytes): {extendedHeader.SCI.TextCodeSetInfo.SizeInBytes}");

                        Console.WriteLine($"      Stack size: {extendedHeader.SCI.StackSize}");

                        Console.WriteLine($"      Read-only code set info:");
                        Console.WriteLine($"        Address: {extendedHeader.SCI.ReadOnlyCodeSetInfo.Address}");
                        Console.WriteLine($"        Physical region size (in page-multiples): {extendedHeader.SCI.ReadOnlyCodeSetInfo.PhysicalRegionSizeInPages}");
                        Console.WriteLine($"        Size (in bytes): {extendedHeader.SCI.ReadOnlyCodeSetInfo.SizeInBytes}");

                        Console.WriteLine($"      Reserved 2: {BitConverter.ToString(extendedHeader.SCI.Reserved2).Replace('-', newChar: ' ')}");

                        Console.WriteLine($"      Data code set info:");
                        Console.WriteLine($"        Address: {extendedHeader.SCI.DataCodeSetInfo.Address}");
                        Console.WriteLine($"        Physical region size (in page-multiples): {extendedHeader.SCI.DataCodeSetInfo.PhysicalRegionSizeInPages}");
                        Console.WriteLine($"        Size (in bytes): {extendedHeader.SCI.DataCodeSetInfo.SizeInBytes}");

                        Console.WriteLine($"      BSS size: {extendedHeader.SCI.BSSSize}");
                        Console.WriteLine($"      Dependency module list: {string.Join(", ", extendedHeader.SCI.DependencyModuleList)}");

                        Console.WriteLine($"      System info:");
                        Console.WriteLine($"        SaveData size: {extendedHeader.SCI.SystemInfo.SaveDataSize}");
                        Console.WriteLine($"        Jump ID: {extendedHeader.SCI.SystemInfo.JumpID}");
                        Console.WriteLine($"        Reserved: {BitConverter.ToString(extendedHeader.SCI.SystemInfo.Reserved).Replace('-', newChar: ' ')}");

                        Console.WriteLine($"    Access control info:");
                        Console.WriteLine($"      ARM11 local system capabilities:");
                        Console.WriteLine($"        Program ID: {extendedHeader.ACI.ARM11LocalSystemCapabilities.ProgramID}");
                        Console.WriteLine($"        Core version: {extendedHeader.ACI.ARM11LocalSystemCapabilities.CoreVersion}");
                        Console.WriteLine($"        Flag 1: {extendedHeader.ACI.ARM11LocalSystemCapabilities.Flag1}");
                        Console.WriteLine($"        Flag 2: {extendedHeader.ACI.ARM11LocalSystemCapabilities.Flag2}");
                        Console.WriteLine($"        Flag 0: {extendedHeader.ACI.ARM11LocalSystemCapabilities.Flag0}");
                        Console.WriteLine($"        Priority: {extendedHeader.ACI.ARM11LocalSystemCapabilities.Priority}");
                        Console.WriteLine($"        Resource limit descriptors: {string.Join(", ", extendedHeader.ACI.ARM11LocalSystemCapabilities.ResourceLimitDescriptors)}");

                        Console.WriteLine($"        Storage info:");
                        Console.WriteLine($"          Extdata ID: {extendedHeader.ACI.ARM11LocalSystemCapabilities.StorageInfo.ExtdataID}");
                        Console.WriteLine($"          System savedata IDs: {BitConverter.ToString(extendedHeader.ACI.ARM11LocalSystemCapabilities.StorageInfo.SystemSavedataIDs).Replace('-', newChar: ' ')}");
                        Console.WriteLine($"          Storage accessible unique IDs: {BitConverter.ToString(extendedHeader.ACI.ARM11LocalSystemCapabilities.StorageInfo.StorageAccessibleUniqueIDs).Replace('-', newChar: ' ')}");
                        Console.WriteLine($"          File system access info: {BitConverter.ToString(extendedHeader.ACI.ARM11LocalSystemCapabilities.StorageInfo.FileSystemAccessInfo).Replace('-', newChar: ' ')}");
                        Console.WriteLine($"          Other attributes: {extendedHeader.ACI.ARM11LocalSystemCapabilities.StorageInfo.OtherAttributes}");

                        Console.WriteLine($"        Service access control: {string.Join(", ", extendedHeader.ACI.ARM11LocalSystemCapabilities.ServiceAccessControl)}");
                        Console.WriteLine($"        Extended service access control: {string.Join(", ", extendedHeader.ACI.ARM11LocalSystemCapabilities.ExtendedServiceAccessControl)}");
                        Console.WriteLine($"        Reserved: {BitConverter.ToString(extendedHeader.ACI.ARM11LocalSystemCapabilities.Reserved).Replace('-', newChar: ' ')}");
                        Console.WriteLine($"        Resource limit cateogry: {extendedHeader.ACI.ARM11LocalSystemCapabilities.ResourceLimitCategory}");

                        Console.WriteLine($"      ARM11 kernel capabilities:");
                        Console.WriteLine($"        Descriptors: {string.Join(", ", extendedHeader.ACI.ARM11KernelCapabilities.Descriptors)}");
                        Console.WriteLine($"        Reserved: {BitConverter.ToString(extendedHeader.ACI.ARM11KernelCapabilities.Reserved).Replace('-', newChar: ' ')}");

                        Console.WriteLine($"      ARM9 access control:");
                        Console.WriteLine($"        Descriptors: {BitConverter.ToString(extendedHeader.ACI.ARM9AccessControl.Descriptors).Replace('-', newChar: ' ')}");
                        Console.WriteLine($"        Descriptor version: {extendedHeader.ACI.ARM9AccessControl.DescriptorVersion}");

                        Console.WriteLine($"    AccessDec signature (RSA-2048-SHA256): {BitConverter.ToString(extendedHeader.AccessDescSignature).Replace('-', ' ')}");
                        Console.WriteLine($"    NCCH HDR RSA-2048 public key: {BitConverter.ToString(extendedHeader.NCCHHDRPublicKey).Replace('-', ' ')}");


                        Console.WriteLine($"    Access control info (for limitations of first ACI):");
                        Console.WriteLine($"      ARM11 local system capabilities:");
                        Console.WriteLine($"        Program ID: {extendedHeader.ACIForLimitations.ARM11LocalSystemCapabilities.ProgramID}");
                        Console.WriteLine($"        Core version: {extendedHeader.ACIForLimitations.ARM11LocalSystemCapabilities.CoreVersion}");
                        Console.WriteLine($"        Flag 1: {extendedHeader.ACIForLimitations.ARM11LocalSystemCapabilities.Flag1}");
                        Console.WriteLine($"        Flag 2: {extendedHeader.ACIForLimitations.ARM11LocalSystemCapabilities.Flag2}");
                        Console.WriteLine($"        Flag 0: {extendedHeader.ACIForLimitations.ARM11LocalSystemCapabilities.Flag0}");
                        Console.WriteLine($"        Priority: {extendedHeader.ACIForLimitations.ARM11LocalSystemCapabilities.Priority}");
                        Console.WriteLine($"        Resource limit descriptors: {string.Join(", ", extendedHeader.ACIForLimitations.ARM11LocalSystemCapabilities.ResourceLimitDescriptors)}");

                        Console.WriteLine($"        Storage info:");
                        Console.WriteLine($"          Extdata ID: {extendedHeader.ACIForLimitations.ARM11LocalSystemCapabilities.StorageInfo.ExtdataID}");
                        Console.WriteLine($"          System savedata IDs: {BitConverter.ToString(extendedHeader.ACIForLimitations.ARM11LocalSystemCapabilities.StorageInfo.SystemSavedataIDs).Replace('-', newChar: ' ')}");
                        Console.WriteLine($"          Storage accessible unique IDs: {BitConverter.ToString(extendedHeader.ACIForLimitations.ARM11LocalSystemCapabilities.StorageInfo.StorageAccessibleUniqueIDs).Replace('-', newChar: ' ')}");
                        Console.WriteLine($"          File system access info: {BitConverter.ToString(extendedHeader.ACIForLimitations.ARM11LocalSystemCapabilities.StorageInfo.FileSystemAccessInfo).Replace('-', newChar: ' ')}");
                        Console.WriteLine($"          Other attributes: {extendedHeader.ACIForLimitations.ARM11LocalSystemCapabilities.StorageInfo.OtherAttributes}");

                        Console.WriteLine($"        Service access control: {string.Join(", ", extendedHeader.ACIForLimitations.ARM11LocalSystemCapabilities.ServiceAccessControl)}");
                        Console.WriteLine($"        Extended service access control: {string.Join(", ", extendedHeader.ACIForLimitations.ARM11LocalSystemCapabilities.ExtendedServiceAccessControl)}");
                        Console.WriteLine($"        Reserved: {BitConverter.ToString(extendedHeader.ACIForLimitations.ARM11LocalSystemCapabilities.Reserved).Replace('-', newChar: ' ')}");
                        Console.WriteLine($"        Resource limit cateogry: {extendedHeader.ACIForLimitations.ARM11LocalSystemCapabilities.ResourceLimitCategory}");

                        Console.WriteLine($"      ARM11 kernel capabilities:");
                        Console.WriteLine($"        Descriptors: {string.Join(", ", extendedHeader.ACIForLimitations.ARM11KernelCapabilities.Descriptors)}");
                        Console.WriteLine($"        Reserved: {BitConverter.ToString(extendedHeader.ACIForLimitations.ARM11KernelCapabilities.Reserved).Replace('-', newChar: ' ')}");

                        Console.WriteLine($"      ARM9 access control:");
                        Console.WriteLine($"        Descriptors: {BitConverter.ToString(extendedHeader.ACIForLimitations.ARM9AccessControl.Descriptors).Replace('-', newChar: ' ')}");
                        Console.WriteLine($"        Descriptor version: {extendedHeader.ACIForLimitations.ARM9AccessControl.DescriptorVersion}");
                    }
                }
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Print ExeFS header information
        /// </summary>
        private void PrintExeFSHeaders()
        {
            Console.WriteLine("  ExeFS Header Information:");
            Console.WriteLine("  -------------------------");
            if (ExeFSHeaders == null || ExeFSHeaders.Length == 0)
            {
                Console.WriteLine("  No ExeFS headers");
            }
            else
            {
                for (int i = 0; i < ExeFSHeaders.Length; i++)
                {
                    var exeFSHeader = ExeFSHeaders[i];
                    Console.WriteLine($"  ExeFS Header {i}");
                    if (exeFSHeader == null)
                    {
                        Console.WriteLine($"    Unrecognized partition data, no data can be parsed");
                    }
                    else
                    {
                        Console.WriteLine($"    File headers:");
                        for (int j = 0; j < exeFSHeader.FileHeaders.Length; j++)
                        {
                            var fileHeader = exeFSHeader.FileHeaders[j];
                            Console.WriteLine(value: $"    File Header {j}");
                            Console.WriteLine(value: $"      File name: {fileHeader.FileName}");
                            Console.WriteLine(value: $"      File offset: {fileHeader.FileOffset}");
                            Console.WriteLine(value: $"      File size: {fileHeader.FileSize}");
                        }

                        Console.WriteLine(value: $"    Reserved: {BitConverter.ToString(exeFSHeader.Reserved).Replace('-', ' ')}");
                        
                        Console.WriteLine($"    File hashes:");
                        for (int j = 0; j < exeFSHeader.FileHashes.Length; j++)
                        {
                            var fileHash = exeFSHeader.FileHashes[j];
                            Console.WriteLine(value: $"    File Hash {j}");
                            Console.WriteLine(value: $"      SHA-256: {BitConverter.ToString(fileHash).Replace('-', ' ')}");
                        }
                    }
                }
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Print RomFS header information
        /// </summary>
        private void PrintRomFSHeaders()
        {
            Console.WriteLine("  RomFS Header Information:");
            Console.WriteLine("  -------------------------");
            if (RomFSHeaders == null || RomFSHeaders.Length == 0)
            {
                Console.WriteLine("  No RomFS headers");
            }
            else
            {
                for (int i = 0; i < RomFSHeaders.Length; i++)
                {
                    var romFSHeader = RomFSHeaders[i];
                    Console.WriteLine($"  RomFS Header {i}");
                    if (romFSHeader == null)
                    {
                        Console.WriteLine($"    Unrecognized RomFS data, no data can be parsed");
                    }
                    else
                    {
                        Console.WriteLine(value: $"    Magic string: {romFSHeader.MagicString}");
                        Console.WriteLine(value: $"    Magic number: {romFSHeader.MagicNumber}");
                        Console.WriteLine(value: $"    Master hash size: {romFSHeader.MasterHashSize}");
                        Console.WriteLine(value: $"    Level 1 logical offset: {romFSHeader.Level1LogicalOffset}");
                        Console.WriteLine(value: $"    Level 1 hashdata size: {romFSHeader.Level1HashdataSize}");
                        Console.WriteLine(value: $"    Level 1 block size: {romFSHeader.Level1BlockSizeLog2}");
                        Console.WriteLine(value: $"    Reserved 1: {BitConverter.ToString(romFSHeader.Reserved1).Replace('-', ' ')}");
                        Console.WriteLine(value: $"    Level 2 logical offset: {romFSHeader.Level2LogicalOffset}");
                        Console.WriteLine(value: $"    Level 2 hashdata size: {romFSHeader.Level2HashdataSize}");
                        Console.WriteLine(value: $"    Level 2 block size: {romFSHeader.Level2BlockSizeLog2}");
                        Console.WriteLine(value: $"    Reserved 2: {BitConverter.ToString(romFSHeader.Reserved2).Replace('-', ' ')}");
                        Console.WriteLine(value: $"    Level 3 logical offset: {romFSHeader.Level3LogicalOffset}");
                        Console.WriteLine(value: $"    Level 3 hashdata size: {romFSHeader.Level3HashdataSize}");
                        Console.WriteLine(value: $"    Level 3 block size: {romFSHeader.Level3BlockSizeLog2}");
                        Console.WriteLine(value: $"    Reserved 3: {BitConverter.ToString(romFSHeader.Reserved3).Replace('-', ' ')}");
                        Console.WriteLine(value: $"    Reserved 4: {BitConverter.ToString(romFSHeader.Reserved4).Replace('-', ' ')}");
                        Console.WriteLine(value: $"    Optional info size: {romFSHeader.OptionalInfoSize}");
                    }
                }
            }
            Console.WriteLine();
        }

        #endregion
    }
}