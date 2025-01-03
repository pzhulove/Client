
namespace Tenmove.Runtime.Unity
{
    using System.Collections.Generic;
    using System.IO;
    using Tenmove.Runtime;

    internal partial class UnityGameProfileServer
    {
        private class Serializer : ITMSerializer<AssetPackageDesc>,ITMDeserializer<AssetPackageDesc>
        {
            public void Serialize(AssetPackageDesc serializeObject, Stream stream)
            {
                if (null != serializeObject)
                {
                    if(null != stream)
                        _SerializePackageDesc(serializeObject, new BinaryWriter(stream));
                    else
                        Debugger.LogWarning("Stream can not be empty!");
                }
                else
                    Debugger.LogWarning("Serialize object can not be empty!");
            }
            
            public void Deserialize(Stream stream,ref AssetPackageDesc serializeObject)
            {
                if (null != stream)
                    _DeserializePackageDesc(new BinaryReader(stream),ref serializeObject);
                else
                    Debugger.LogWarning("Stream can not be empty!");
            } 

            private void _SerializePackageDesc(AssetPackageDesc serializeObject,BinaryWriter writer)
            {
                writer.Write(serializeObject.PackageBytesSize);
                writer.Write(serializeObject.PackageName);
                writer.Write(serializeObject.PackageMD5);
                writer.Write(serializeObject.PackageAssets.Count);
                for(int i = 0,icnt = serializeObject.PackageAssets.Count;i<icnt;++i)
                    writer.Write(serializeObject.PackageAssets[i]);
            }

            private void _DeserializePackageDesc(BinaryReader reader,ref AssetPackageDesc serializeObject)
            {
                long packageBytesSize = reader.ReadInt64();
                string packageName = reader.ReadString();
                string packageMD5 = reader.ReadString();
                int assetCount = reader.ReadInt32();
                List<string> assetList = FrameStackList<string>.Acquire();
                for (int i = 0, icnt = assetCount; i < icnt; ++i)
                    assetList.Add(reader.ReadString());

                serializeObject = new AssetPackageDesc(packageName, packageMD5, packageBytesSize, assetList);
                FrameStackList<string>.Recycle(assetList);
            }
        }
    }
}