using Google.ProtocolBuffers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DFZMochiInferno
{
    internal class PbFiles
    {
        private static byte[] buf = new byte[1048576];

        public static List<T> LoadPbFilesFromAssembly<T>(Func<T> constructor, string type) where T : Message
        {
            var assembly = Assembly.GetExecutingAssembly();

            List<T> list = new List<T>();

            foreach (var name in assembly.GetManifestResourceNames())
            {
                if (!name.Contains('_' + type))
                {
                    continue;
                }
                int num;
                using (var stream = assembly.GetManifestResourceStream(name))
                {
                    num = stream.Read(buf, 0, buf.Length);
                }
                list.AddRange(PbFile.ReadPbList<T>(constructor, buf, 0, num).ToList<T>());
            }

            return list;
        }
    }
}
