
using System;
using UnityEngine;


namespace Tenmove.Runtime.Unity
{
    internal class UnityAssetByteLoader : AssetByteLoader
    {
        public override Type NativeByteAssetType
        {
            get
            {
                return typeof(TextAsset);
            }
        }

        public override AssetByte LoadAssetByte(object asset)
        {
            TextAsset textAsset = asset as TextAsset;
            if (null != textAsset)
                return new AssetByte(textAsset.bytes);
            else
                return new AssetByte(new byte[0]);
        }
    }
}