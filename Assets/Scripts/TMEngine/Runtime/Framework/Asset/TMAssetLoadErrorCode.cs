

namespace Tenmove.Runtime
{
    public enum AssetLoadErrorCode
    {
        OK = 0,

        NotReady,
        NullAsset,
        NotExist,
        PackageError,
        DependencyLoadError,
        TaskTypeError,
        TypeError,
        InvalidParam,
    }
}