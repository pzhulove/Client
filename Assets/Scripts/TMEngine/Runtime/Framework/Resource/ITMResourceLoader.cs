
namespace Tenmove.Runtime
{
    public enum ResourceLoadMode
    {
        None,
        FetchPackage,
        LoadPackage,
        LoadAsset,
    }

    interface ITMResourceLoader
    {
        void Reset();
        void UnloadPackage(string packagePath);
    }
}

