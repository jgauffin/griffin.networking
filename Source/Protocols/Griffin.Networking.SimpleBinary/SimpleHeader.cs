using System.Runtime.Serialization;
using Griffin.Networking.SimpleBinary.Handlers;
using Griffin.Networking.SimpleBinary.Messages;

namespace Griffin.Networking.SimpleBinary
{
    /// <summary>
    /// Header used to transport information
    /// </summary>
    /// <remarks><para>Header of format <c>byte</c> (version) <c>byte</c> (contentId) <c>int</c> (content length)</para>
    /// <para>the version identifies the header version, so that we can improve it later of if we would like. the current version is <c>1</c>.</para>
    /// <para>ContentId is used to identify the content type. Typically one id per data type. Ids 1-10 is reserved for the system</para>
    /// <para>Content length is the number of bytes for the content/body in this packet.</para>
    /// </remarks>
    /// <seealso cref="ReceivedHeader"/>
    /// <seealso cref="ResponseEncoder"/>
    public class SimpleHeader
    {
        /// <summary>
        /// Gets or sets version of this 
        /// </summary>
        [DataMember(Order = 1)]
        public byte Version { get; set; }

        /// <summary>
        /// Gets or sets an id which identifies the type of content.
        /// </summary>
        /// <remarks>1-10 is reserved by other simple handlers.</remarks>
        [DataMember(Order = 2)]
        public byte ContentId { get; set; }

        /// <summary>
        /// Gets or sets length of the content in this packet.
        /// </summary>
        [DataMember(Order = 3)]
        public int ContentLength { get; set; }
    }
}