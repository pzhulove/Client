


using System.Collections.Generic;

namespace Tenmove.Runtime
{
    public enum BufferUsage
    {
        Read,
        Write,
    }

    public interface ITMNetMessageBufferPool
    {
        ITMByteBlock AcquireBuffer(uint size, BufferUsage usage);
        ITMByteBlock AcquireBuffer(byte[] rawBuffer, BufferUsage usage);
    }
}