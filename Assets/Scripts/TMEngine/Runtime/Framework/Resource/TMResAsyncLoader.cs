using System;

namespace Tenmove.Runtime
{
    public sealed class LoadResourceUpdateEventArgs : BaseEventArgs
    {
        public LoadResourceUpdateEventArgs(ResourceLoadMode mode, float progress)
        {
            Mode = mode;
            Progress = progress;
        }

        public ResourceLoadMode Mode
        {
            get;
            private set;
        }

        public float Progress
        {
            get;
            private set;
        }
    }

    public sealed class LoadResourceCompleteEventArgs : BaseEventArgs
    {
        public LoadResourceCompleteEventArgs(object asset,bool sharedAsset)
        {
            Asset = asset;
            SharedAsset = sharedAsset;
        }

        public object Asset
        {
            get;
            private set;
        }

        public bool SharedAsset
        {
            get;
            private set;
        }
    }

    public sealed class LoadResourceFailedEventArgs : BaseEventArgs
    {
        public LoadResourceFailedEventArgs(AssetLoadErrorCode errorCode, string msg,string resName)
        {
            ErrorCode = errorCode;
            Message = msg;
            ResName = resName;
        }

        public string ResName
        {
            get;
            private set;
        }

        public AssetLoadErrorCode ErrorCode
        {
            get;
            private set;
        }

        public string Message
        {
            get;
            private set;
        }
    }

    public sealed class LoadPackageCompleteEventArgs : BaseEventArgs
    {
        public LoadPackageCompleteEventArgs(object package,string packageName)
        {
            Package = package;
            PackageName = packageName;
        }

        public string PackageName
        {
            get;
            private set;
        }

        public object Package
        {
            get;
            private set;
        }
    }

    public delegate void ResAsyncLoadCallback(string fileURI, byte[] bytes, float duration, string errorMessage);

    public abstract class TMResAsyncLoader : ITMResourceLoader
    {
        public abstract event EventHandler<LoadResourceUpdateEventArgs> UpdateResourceEventHandler;
        public abstract event EventHandler<LoadResourceCompleteEventArgs> LoadResourceCompleteEventHandler;
        public abstract event EventHandler<LoadResourceFailedEventArgs> LoadResourceFailedEventHandler;
        public abstract event EventHandler<LoadPackageCompleteEventArgs> LoadPackageCompleteEventHandler;

        public abstract bool ChangedToSyncLoad { get; }
        
        abstract public bool ForceSyncLoadAsset();

        abstract public void LoadPackage(string fullpath);
        abstract public void LoadAsset(object resource, string assetName,string subResName,Type assetType);
        abstract public void LoadFile(string filepath,bool readWritePath, ResAsyncLoadCallback callback);
        abstract public void UnloadPackage(string packagePath);
        abstract public void Reset();
        abstract public void Update();
    }
}

