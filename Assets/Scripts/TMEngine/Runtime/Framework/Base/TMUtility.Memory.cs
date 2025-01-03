
using System.IO;

namespace Tenmove.Runtime
{
    public static partial class Utility
    {
        public static class Memory
        {
            static public Stream OpenStream(int capcity)
            {
                return new MemoryStream(capcity);
            }

            static public Stream OpenStream(byte[] buffer)
            {
                return new MemoryStream(buffer, 0, buffer.Length);
            }

            static public Stream OpenStream(byte[] buffer, int index, int count)
            {
                return new MemoryStream(buffer, index, count);
            }
        }
    }
}