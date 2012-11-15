using System.Runtime.CompilerServices;

namespace Griffin.Networking.Protocols.Basic
{
    /// <summary>
    /// Lightweight transportation of objects over the wire.
    /// </summary>
    /// <remarks>Uses json.net to serialize/deserialize the objects. The JSON is wrapped by a binary header (byte = version, int = XML size).
    /// <para>Simply construct a client or server using the <see cref="BasicMessageFactory"/></para>
    /// </remarks>
    [CompilerGenerated]
    internal class NamespaceDoc
    {
    }
}