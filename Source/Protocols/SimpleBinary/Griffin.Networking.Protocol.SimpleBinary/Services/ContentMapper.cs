using System;
using System.Collections.Generic;
using System.Linq;

namespace Griffin.Networking.SimpleBinary.Services
{
    /// <summary>
    /// Used to map between objects (packets) and their content id
    /// </summary>
    /// <seealso cref="SimpleHeader"/>
    public class ContentMapper
    {
        private readonly List<ContentMapping> _mappings = new List<ContentMapping>();

        /// <summary>
        /// Gets the type of the packet.
        /// </summary>
        /// <param name="contentId">The content id.</param>
        /// <returns>Type to create for the packet if found; otherwise <c>null</c>.</returns>
        public Type GetPacketType(byte contentId)
        {
            return _mappings.Where(x => x.ContentId == contentId).Select(x => x.PacketType).FirstOrDefault();
        }

        /// <summary>
        /// Gets the content id.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns>Content id byte</returns>
        /// <remarks>The content type must have been mapped.</remarks>
        /// <example>
        /// <code>
        /// var info = new FileInfoPacket("abraham.txt", 40);
        /// var contentId = mapper.GetContentId(info);
        /// </code>
        /// </example>
        public byte GetContentId(object content)
        {
            var mapping = _mappings.FirstOrDefault(x => x.PacketType == content.GetType());
            if (mapping == null)
                throw new InvalidOperationException(string.Format("Failed to find a mapping for '{0}'.",
                                                                  content.GetType()));

            return mapping.ContentId;
        }

        /// <summary>
        /// Maps the specified content id.
        /// </summary>
        /// <param name="contentId">The content id.</param>
        /// <param name="type">The type.</param>
        /// <example>
        /// <code>
        /// mapper.Map(10, typeof(FileInfoPacket));
        /// </code>
        /// </example>
        public void Map(byte contentId, Type type)
        {
            if (type == null) throw new ArgumentNullException("type");
            _mappings.Add(new ContentMapping
                              {
                                  ContentId = contentId,
                                  PacketType = type
                              });
        }

        #region Nested type: ContentMapping

        public class ContentMapping
        {
            public byte ContentId { get; set; }
            public Type PacketType { get; set; }
        }

        #endregion
    }
}