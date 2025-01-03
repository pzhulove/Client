


namespace Tenmove.Runtime.Unity
{
    using System;

    public delegate void AssetLoadCallback<TAsset, TUserData>(uint requestID, string assetPath, TAsset assetObject, TUserData userData);

    public interface ITMUnityGameProfiler
    {
        uint InvalidAssetHandle { get; }

        void RegisterMessageProcessor<T, U>(MessageProcessor<T, U> processor, U userData)
            where T : NetMessage where U : IEquatable<U>;

        uint LoadAsset<TAsset, TUserData>(string assetPath, TUserData userData, AssetLoadCallback<TAsset, TUserData> callback) where TAsset : class;
    }
}