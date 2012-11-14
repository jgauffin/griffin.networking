using System.Reflection;
using Griffin.Networking.Buffers;

namespace Griffin.Networking.Http.Tests.Msgs
{
    public class MsgLoader
    {
        public static IBufferSlice Load(string nameWithoutExtension)
        {
            using (
                var stream =
                    Assembly.GetExecutingAssembly().GetManifestResourceStream(typeof (MsgLoader).Namespace + "." +
                                                                              nameWithoutExtension + ".txt"))
            {
                var buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
                return new BufferSlice(buffer, 0, buffer.Length);
            }
        }
    }
}