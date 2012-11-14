using System;
using System.Collections.Generic;
using System.IO;
using Griffin.Networking.Buffers;
using Griffin.Networking.Messaging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Griffin.Networking.Protocols.Basic
{
    /// <summary>
    /// Builds messages from incoming data
    /// </summary>
    public class BasicMessageBuilder : IMessageBuilder
    {
        private readonly byte[] _header = new byte[Packet.HeaderLength];
        private readonly Queue<Packet> _messages = new Queue<Packet>();
        private int _bytesLeft = Packet.HeaderLength;
        private Packet _packet;
        private Func<IBufferReader, bool> _parserMethod;

        /// <summary>
        /// Initializes a new instance of the <see cref="BasicMessageBuilder" /> class.
        /// </summary>
        public BasicMessageBuilder()
        {
            _parserMethod = ReadHeaderBytes;
        }

        #region IMessageBuilder Members

        /// <summary>
        /// Append more bytes to your message building
        /// </summary>
        /// <param name="reader">Contains bytes which was received from the other end</param>
        /// <returns><c>true</c> if a complete message has been built; otherwise <c>false</c>.</returns>
        /// <remarks>You must handle/read everything which is available in the buffer</remarks>
        public bool Append(IBufferReader reader)
        {
            while (_parserMethod(reader))
            {
            }

            return _messages.Count > 0;
        }

        /// <summary>
        /// Try to dequeue a message
        /// </summary>
        /// <param name="message">Message that the builder has built.</param>
        /// <returns><c>true</c> if a message was available; otherwise <c>false</c>.</returns>
        public bool TryDequeue(out object message)
        {
            if (_messages.Count == 0)
            {
                message = null;
                return false;
            }

            var packet = _messages.Dequeue();

            using (var reader = new StreamReader(packet.Message))
            {
                var str = reader.ReadToEnd();
                message = JsonConvert.DeserializeObject(str, new JsonSerializerSettings(){});
            }

            return true;
        }

        #endregion

        /// <summary>
        /// Read all header bytes from the incoming buffer 
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns><c>false</c> if we have not received all bytes yet; otherwise <c>true</c>.</returns>
        protected virtual bool ReadHeaderBytes(IBufferReader stream)
        {
            var bytesLeftInStream = stream.Count - stream.Position;
            var bytesToCopy = bytesLeftInStream < _bytesLeft
                                  ? bytesLeftInStream
                                  : _bytesLeft;

            stream.Read(_header, 0, bytesToCopy);

            _bytesLeft -= bytesToCopy;
            if (_bytesLeft > 0)
                return false;

            _packet = CreatePacket(_header);

            _bytesLeft = _packet.ContentLength;
            _parserMethod = ReadBodyBytes;
            return true;
        }

        /// <summary>
        /// Read all body bytes from the incoming buffer
        /// </summary>
        /// <param name="reader">Contains received bytes</param>
        /// <returns><c>false</c> if we have not received all bytes yet; otherwise <c>true</c>.</returns>
        protected virtual bool ReadBodyBytes(IBufferReader reader)
        {
            var bytesLeftInStream = reader.Count - reader.Position;
            var bytesToCopy = bytesLeftInStream < _bytesLeft
                                  ? bytesLeftInStream
                                  : _bytesLeft;


            reader.CopyTo(_packet.Message, bytesToCopy);

            _bytesLeft -= bytesToCopy;
            if (_bytesLeft > 0)
                return false;

            _packet.Message.Position = 0;
            _messages.Enqueue(_packet);
            _packet = null;

            _bytesLeft = Packet.HeaderLength;
            _parserMethod = ReadHeaderBytes;
            return true;
        }

        /// <summary>
        /// Create a new packet from the header bytes.
        /// </summary>
        /// <param name="header">Header bytes</param>
        /// <returns>Created packet (filled with info)</returns>
        /// <remarks>The packet bytes are described in the <see cref="Packet"/> class doc.</remarks>
        protected virtual Packet CreatePacket(byte[] header)
        {
            var message = new Packet
                {
                    Version = _header[0],
                    ContentLength = BitConverter.ToInt32(header, 1)
                };

            if (message.Version <= 0)
                throw new InvalidDataException(string.Format(
                    "Received '{0}' as version. Must be larger or equal to 1.", message.Version));
            if (message.ContentLength <= 0)
                throw new InvalidDataException(string.Format(
                    "Got invalid content length: '{0}', expected 1 or larger.", message.ContentLength));

            message.Message = new MemoryStream(message.ContentLength);
            return message;
        }
    }

    public class Builder : JsonConverter
    {
        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter"/> to write to.</param><param name="value">The value.</param><param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader"/> to read from.</param><param name="objectType">Type of the object.</param><param name="existingValue">The existing value of object being read.</param><param name="serializer">The calling serializer.</param>
        /// <returns>
        /// The object value.
        /// </returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            reader.re
            // Load JObject from stream
            JObject jObject = JObject.Load(reader);

            // Create target object based on JObject
            T target = Create(objectType, jObject);

            // Populate the object properties
            serializer.Populate(jObject.CreateReader(), target);

            return target;
        }

        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>
        /// <c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanConvert(Type objectType)
        {
            return true;
        }
    }
}