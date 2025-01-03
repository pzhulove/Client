
namespace Tenmove.Runtime
{
    public static partial class Utility
    {
        public static class UUID
        {
            static public uint Create32BitUUID()
            {
                return (uint)System.Guid.NewGuid().GetHashCode();
            }
        }
    }
}
