

using System;

namespace Tenmove.Runtime
{
    public abstract class AssetByteLoader
    {
        public abstract Type NativeByteAssetType
        {
            get;
        }

        public abstract AssetByte LoadAssetByte(object asset);
    }
}