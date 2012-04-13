using System;

namespace Griffin.Networking.Buffers
{
    /// <summary>
    /// Used to share a sliced buffer.
    /// </summary>
    /// <remarks>
    /// A large buffer can be sliced up into chunks to prevent memory defragmentation. This class is used to manage one of those slices.
    /// </remarks>
    public class BufferSlice : IBufferSlice
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BufferSlice"/> class.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="startOffset">Offset in buffer where the slice starts.</param>
        /// <param name="capacity">Number of bytes allocated for this slice.</param>
        /// <param name="count">Number of bytes written to the buffer (if any)</param>
        public BufferSlice(byte[] buffer, int startOffset, int capacity, int count)
        {
            StartOffset = startOffset;
            Capacity = capacity;
            Count = count;
            Buffer = buffer;
            Position = StartOffset;
        }

        /// <summary>
        /// Gets the slice
        /// </summary>
        public byte[] Buffer { get; private set; }

        /// <summary>
        /// Gets start offset for our buffer slize
        /// </summary>
        public int StartOffset { get; private set; }

        /// <summary>
        /// Gets number of bytes allocated for our slice
        /// </summary>
        public int Capacity { get; private set; }


        private int _offset;

        /// <summary>
        /// Gets current offset in buffer
        /// </summary>
        public int Position
        {
            get { return _offset; }
            set
            {
                if (value < StartOffset)
                    throw new ArgumentOutOfRangeException(value + " cannot be smaller than start offset which is " + StartOffset);
                if (value >= StartOffset + Capacity + 1) //+1 to move past the end character
                    throw new ArgumentOutOfRangeException(value + " cannot be larger than allocated slice (end pos" +
                                                          (StartOffset + Capacity - 1) + ").");

                _offset = value;
            }
        }

        /// <summary>
        /// Gets number of bytes written that are left in the buffer (from <see cref="Offset"/> to the end)
        /// </summary>
        public int RemainingLength
        {
            get { return Count - (Position - StartOffset); }
        }

        /// <summary>
        /// Gets number of bytes written that are left in the buffer (from <see cref="Offset"/> to the end)
        /// </summary>
        public int RemainingCapacity
        {
            get { return Capacity - (Position - StartOffset); }
        }


        /// <summary>
        /// Gets or sets number of bytes written to the buffer.
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Move remaining bytes to the beginning of the buffer
        /// </summary>
        public void Compact()
        {
            // read everything, no need to clear. Just reset indexes.
            if (RemainingLength == 0)
            {
                _offset = StartOffset;
                Count = 0;
                return;
            }

            var remaingingCount = RemainingLength;
            System.Buffer.BlockCopy(Buffer, _offset, Buffer, StartOffset, RemainingLength);
            _offset = StartOffset;
            Count = remaingingCount;
        }
    }
}