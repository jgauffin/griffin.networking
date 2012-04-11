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
    public class BufferPoolStream : MemoryStream
    {
        private readonly BufferPool _pool;
        private bool _returnedBuffer = false;

        public BufferPoolStream(BufferPool pool)
            : base(pool.Pop())
        {
            _pool = pool;
        }

        protected override void Dispose(bool disposing)
        {
            if(!_returnedBuffer)
            {
                _pool.Push(GetBuffer());
                _returnedBuffer = true;
            }

            base.Dispose(disposing);
        }
    }
}
