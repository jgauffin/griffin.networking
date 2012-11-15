namespace Griffin.Networking.Buffers
{
    public interface IBufferSliceStack
    {
        void Push(IBufferSlice slice);
        IBufferSlice Pop();
    }
}