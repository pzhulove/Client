

namespace Tenmove.Runtime.Unity
{
    using System;
    using System.Collections.Generic;

    public interface ITMGameProfileAssetPackageDesc
    {
        bool HasContent { get; }
        string PackageName { get; }
        string PackageMD5 { get; }
        long PackageBytesSize { get; }
        List<string> PackageAssets { get; }
    }

    public interface ITMGameProfileAssetPackageDescFiller : ITMGameProfileAssetPackageDesc
    {
        void Fill(string packageName, string md5, long size, IList<string> assets);
    }

    public interface ITMGameProfileAssetPacker
    {
        bool PackAsset(string targetFolder, string packageName, List<string> assetList, ITMGameProfileAssetPackageDescFiller packageDescFiller);
    }

    public interface ITMUnityGameProfileServer
    {
        string ServerIP { get; }
        bool IsServerOnline { get; }
        bool HasConnection { get; }

        ITMGameProfileAssetPackageDesc CurrentPackageDesc { get; }

        void SetAssetPacker(ITMGameProfileAssetPacker assetPacker);
        void StartServer();
        void ShutdownServer();

        NetMessage CreateMessage(Type netMessageType);
        T CreateMessage<T>() where T : NetMessage;
        void SendMessage<T>(T message, string ip = "") where T : NetMessage;
        void RegisterMessageProcessor<T, U>(MessageProcessor<T, U> processor, U userData)
            where T : NetMessage where U : IEquatable<U>;

        void GetConnections(List<ITMNetConnection> connectionsList);
        void PackAssets(List<string> assetList);
        void RefreshPackageDesc();
    }
}