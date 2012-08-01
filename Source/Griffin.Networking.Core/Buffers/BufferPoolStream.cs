using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Griffin.Networking.Buffers
{
    /// <summary>
    /// A stream that will return the buffer to the pool when being disposed.
    /// </summary>
    public class BufferPoolStream : MemoryStream, IPeekable
    {
        private readonly BufferPool _pool;
        private bool _returnedBuffer = false;
        private BufferSlice _slize;

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferPoolStream"/> class.
        /// </summary>
        /// <param name="pool">The pool.</param>
        /// <param name="slice">The slice.</param>
        public BufferPoolStream(BufferPool pool, BufferSlice slice)
            : base(slice.Buffer, slice.StartOffset, slice.Capacity, true, true)
        {
            _slize = slice;
            _pool = pool;
            SetLength(slice.Count);
            Position = slice.Position;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferPoolStream"/> class.
        /// </summary>
        /// <param name="slice">The slice.</param>
        public BufferPoolStream(BufferSlice slice)
            : base(slice.Buffer, slice.StartOffset, slice.Capacity, true, true)
        {
            _slize = slice;
            SetLength(slice.Count);
            Position = slice.Position;
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="T:System.IO.MemoryStream"/> class and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (!_returnedBuffer)
            {
                if (_pool != null)
                    _pool.Push(GetBuffer());
                _returnedBuffer = true;
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Peek at the next byte in the sequence.
        /// </summary>
        /// <returns>Char if not EOF; otherwise <see cref="char.MinValue"/></returns>
        public char Peek()
        {
            if (_slize.RemainingLength <= 0)
                return char.MinValue;

            return (char)_slize.Buffer[_slize.Position + 1];
        }
    }
}
