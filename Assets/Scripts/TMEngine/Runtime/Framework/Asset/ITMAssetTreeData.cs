using System.Collections.Generic;

namespace Tenmove.Runtime
{
    public interface ITMAssetTreeData
    {
        List<Runtime.AssetDesc> GetAssetDescMap();
        List<Runtime.AssetPackageDesc> GetAssetPackageDescMap();
    }
}