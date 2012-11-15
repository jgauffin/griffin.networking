namespace Griffin.Networking.Buffers
{
    /// <summary>
    /// We are a stream which is wrapping a buffer (or a slice)
    /// </summary>
    public interface IBufferStream : IBufferReader, IBufferWriter
    {
    }
}