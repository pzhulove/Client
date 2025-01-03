

namespace Tenmove.Runtime
{
    public static partial class Utility
    {
        public static class Handle
        {
            public static readonly uint InvalidHandle = ~0u;

            static public uint AllocHandle(byte handleType, ref uint handleAllocCount)
            {
                if (handleAllocCount + 1 >= uint.MaxValue >> 8)
                    handleAllocCount = 0;
                return (handleAllocCount++) | (((uint)handleType & 0xff) << 24);
            }

            static public byte GetHandleType(uint handle)
            {
                return (byte)((handle >> 24) & 0xff);
            }

            static public uint InvalidTypeHandle(byte handleType)
            {
                return (((uint)handleType & 0xff) << 24) | 0x00ffffff;
            }
        }
    }
}