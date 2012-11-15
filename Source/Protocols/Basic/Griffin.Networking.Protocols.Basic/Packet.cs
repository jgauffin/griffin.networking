using System.IO;

namespace Griffin.Networking.Protocols.Basic
{
    /// <summary>
    /// Binary packet used to transport the information
    /// </summary>
    public class Packet
    {
        /// <summary>
        /// Total header length
        /// </summary>
        public const int HeaderLength = sizeof (int) + 1;

        /// <summary>
        /// Length of the body
        /// </summary>
        public int ContentLength;

        /// <summary>
        /// Stream used when building the body
        /// </summary>
        public Stream Message;

        /// <summary>
        /// Packet version
        /// </summary>
        public byte Version;
    }
}