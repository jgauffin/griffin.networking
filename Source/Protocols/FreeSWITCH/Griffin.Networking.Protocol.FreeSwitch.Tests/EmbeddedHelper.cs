using System;
using System.IO;
using System.Reflection;

namespace Griffin.Networking.Protocol.FreeSwitch.Tests
{
    public static class EmbeddedHelper
    {
        public static string ReadAllText(this Assembly assembly, string fileName)
        {
            var name = typeof(EmbeddedHelper).Namespace + ".Messages." + fileName;

            using (var stream = assembly.GetManifestResourceStream(name))
            {
                if (stream == null)
                    throw new InvalidOperationException("Failed to find: " + name);
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
            
        }

        public static byte[] ReadAsBytes(this Assembly assembly, string fileName)
        {
            var name = typeof (EmbeddedHelper).Namespace + ".Messages." + fileName;
            using (var stream = assembly.GetManifestResourceStream(name))
            {
                if(stream == null)
                    throw new InvalidOperationException("Failed to find: " + name);
                var buffer = new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);
                return buffer;
            }

        }
    }
}
