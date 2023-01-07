namespace BurnOutSharp.Models.Nitro
{
    /// <summary>
    /// Represents a DS/DSi cart image
    /// </summary>
    public class Cart
    {
        /// <summary>
        /// DS/DSi cart header
        /// </summary>
        public CommonHeader CommonHeader { get; set; }

        /// <summary>
        /// DSi extended cart header
        /// </summary>
        public ExtendedDSiHeader ExtendedDSiHeader { get; set; }

        /// <summary>
        /// Secure area, may be encrypted or decrypted
        /// </summary>
        public byte[] SecureArea { get; set; }
    }
}