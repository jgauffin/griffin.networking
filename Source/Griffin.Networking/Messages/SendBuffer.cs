using System;

namespace Griffin.Networking.Messages
{
    /// <summary>
    /// Send a byte[] buffer
    /// </summary>
    public class SendBuffer : IPipelineMessage
    {
        public SendBuffer(byte[] buffer, int offset, int count)
        {
            if (buffer == null) throw new ArgumentNullException("buffer");
            if (buffer.Length < count)
                throw new ArgumentOutOfRangeException("count", string.Format("Count {0} is larger than buffer length {1}.", count, buffer.Length));
            if (buffer.Length < offset+count)
                throw new ArgumentException(string.Format("Offset+Count ({0}+{1}) is past end of buffer.", offset, count));

            Buffer = buffer;
            Offset = offset;
            Count = count;
        }

        public byte[] Buffer { get; protected set; }
        public int Offset { get; protected set; }
        public int Count { get; protected set; }
    }
}