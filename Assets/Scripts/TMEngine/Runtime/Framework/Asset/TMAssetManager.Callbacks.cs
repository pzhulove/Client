using System;

namespace Tenmove.Runtime
{
    public sealed class AssetInitCompleteEventArgs:BaseEventArgs
    {
        public AssetInitCompleteEventArgs()
        {

        }
    }

    public sealed class VersionListUpdateSuccessEventArgs:BaseEventArgs
    {
        public VersionListUpdateSuccessEventArgs(string storagePath,string downloadURI)
        {
            StoragePath = storagePath;
            DownloadURI = downloadURI;
        }

        public string StoragePath
        {
            get;
            private set;
        }

        public string DownloadURI
        {
            get;
            private set;
        }
    }

    public sealed class VersionListUpdateFailureEventArgs:BaseEventArgs
    {
        public VersionListUpdateFailureEventArgs(string downloadURI,string errorMessage)
        {
            DownloadURI = downloadURI;
            ErrorMessage = errorMessage;
        }

        public string DownloadURI
        {
            get;
            private set;
        }

        public string ErrorMessage
        {
            get;
            private set;
        }
    }

    public sealed class AssetCheckCompleteEventArgs : BaseEventArgs
    {
        public AssetCheckCompleteEventArgs(int removeCount,int updateCount,int updateTotalBytes,int updateTotalZipBytes)
        {
            RemoveCount = removeCount;
            UpdateCount = updateCount;
            UpdateTotalBytes = updateTotalBytes;
            UpdateTotalZipBytes = updateTotalZipBytes;
        }

        public int RemoveCount
        {
            get;
            private set;
        }

        public int UpdateCount
        {
            get;
            private set;
        }

        public int UpdateTotalBytes
        {
            get;
            private set;
        }

        public int UpdateTotalZipBytes
        {
            get;
            private set;
        }
    }

    public sealed class AssetUpdateStartEventArgs:BaseEventArgs
    {
        public AssetUpdateStartEventArgs(string name,string storagePath,string downloadURI,int downloadedBytes,int totalBytes,int retryCount)
        {
            Name = name;
            StoragePath = storagePath;
            DownloadURI = downloadURI;
            DownloadedBytes = downloadedBytes;
            TotalBytes = totalBytes;
            RetryCount = retryCount;
        }

        public string Name
        {
            get;
            private set;
        }

        public string StoragePath
        {
            get;
            private set;
        }

        public string DownloadURI
        {
            get;
            private set;
        }

        public int DownloadedBytes
        {
            get;
            private set;
        }

        public int TotalBytes
        {
            get;
            private set;
        }

        public int RetryCount
        {
            get;
            private set;
        }
    }

    public sealed class AssetUpdateChangedEventArgs : BaseEventArgs
    {
        public AssetUpdateChangedEventArgs(string name, string storagePath, string downloadURI, int downloadedBytes, int totalBytes)
        {
            Name = name;
            StoragePath = storagePath;
            DownloadURI = downloadURI;
            DownloadedBytes = downloadedBytes;
            TotalBytes = totalBytes;
        }

        public string Name
        {
            get;
            private set;
        }

        public string StoragePath
        {
            get;
            private set;
        }

        public string DownloadURI
        {
            get;
            private set;
        }

        public int DownloadedBytes
        {
            get;
            private set;
        }

        public int TotalBytes
        {
            get;
            private set;
        }
    }

    public sealed class AssetUpdateSuccessEventArgs : BaseEventArgs
    {
        public AssetUpdateSuccessEventArgs(string name, string storagePath, string downloadURI, int assetBytes, string assetMD5Sum)
        {
            Name = name;
            StoragePath = storagePath;
            DownloadURI = downloadURI;
            AssetBytes = assetBytes;
            AssetMD5Sum = assetMD5Sum;
        }

        public string Name
        {
            get;
            private set;
        }

        public string StoragePath
        {
            get;
            private set;
        }

        public string DownloadURI
        {
            get;
            private set;
        }

        public int AssetBytes
        {
            get;
            private set;
        }

        public string AssetMD5Sum
        {
            get;
            private set;
        }
    }

    public sealed class AssetUpdateFailureEventArgs:BaseEventArgs
    {
        public AssetUpdateFailureEventArgs(string name,string downloadURI,int retryCount,int targetRetryCount,string errorMessage)
        {
            Name = name;
            DownloadURI = downloadURI;
            RetryCount = retryCount;
            TargetRetryCount = targetRetryCount;
            ErrorMessage = errorMessage;
        }

        public string Name
        {
            get;
            private set;
        }

        public string DownloadURI
        {
            get;
            private set;
        }

        public int RetryCount
        {
            get;
            private set;
        }

        public int TargetRetryCount
        {
            get;
            private set;
        }

        public string ErrorMessage
        {
            get;
            private set;
        }
    }

    public sealed class AssetUpdateFinishEventArgs : BaseEventArgs
    {
        public AssetUpdateFinishEventArgs()
        {

        }
    }

    internal sealed partial class AssetManager
    {
        private EventHandler<AssetInitCompleteEventArgs> m_AssetInitCompleteEventHandler;
        private EventHandler<VersionListUpdateSuccessEventArgs> m_VersionListUpdateSuccessEventHandler;
        private EventHandler<VersionListUpdateFailureEventArgs> m_VersionListUpdateFailureEventHandler;
        private EventHandler<AssetCheckCompleteEventArgs> m_AssetCheckCompleteEventHandler;
        private EventHandler<AssetUpdateStartEventArgs> m_AssetUpdateStartEventHandler;
        private EventHandler<AssetUpdateChangedEventArgs> m_AssetUpdateChangedEventHandler;
        private EventHandler<AssetUpdateFailureEventArgs> m_AssetUpdateFailureEventHandler;
        private EventHandler<AssetUpdateSuccessEventArgs> m_AssetUpdateSuccessEventHandler;
        private EventHandler<AssetUpdateFinishEventArgs> m_AssetUpdateFinishEventHandler;

        public event EventHandler<AssetInitCompleteEventArgs> AssetInitComplete
        {
            add
            {
                m_AssetInitCompleteEventHandler += value;
            }
            remove
            {
                m_AssetInitCompleteEventHandler -= value;
            }
        }

        private void _OnAssetInitComplete()
        {

        }

        public event EventHandler<VersionListUpdateSuccessEventArgs> VersionListUpdateSuccess
        {
            add
            {
                m_VersionListUpdateSuccessEventHandler += value;
            }
            remove
            {
                m_VersionListUpdateSuccessEventHandler -= value;
            }
        }

        private void _OnVersionListUpdateSuccess()
        {

        }

        public event EventHandler<VersionListUpdateFailureEventArgs> VersionListUpdateFailure
        {
            add
            {
                m_VersionListUpdateFailureEventHandler += value;
            }
            remove
            {
                m_VersionListUpdateFailureEventHandler -= value;
            }
        }

        private void _OnVersionListUpdateFailure()
        {

        }

        public event EventHandler<AssetCheckCompleteEventArgs> AssetCheckComplete
        {
            add
            {
                m_AssetCheckCompleteEventHandler += value;
            }
            remove
            {
                m_AssetCheckCompleteEventHandler -= value;
            }
        }

        private void _OnAssetCheckComplete()
        {

        }

        public event EventHandler<AssetUpdateStartEventArgs> AssetUpdateStart
        {
            add
            {
                m_AssetUpdateStartEventHandler += value;
            }
            remove
            {
                m_AssetUpdateStartEventHandler -= value;
            }
        }

        private void _OnAssetUpdateStart()
        {

        }

        public event EventHandler<AssetUpdateChangedEventArgs> AssetUpdateChanged
        {
            add
            {
                m_AssetUpdateChangedEventHandler += value;
            }
            remove
            {
                m_AssetUpdateChangedEventHandler -= value;
            }
        }

        private void _OnAssetUpdateChanged()
        {

        }

        public event EventHandler<AssetUpdateSuccessEventArgs> AssetUpdateSuccess
        {
            add
            {
                m_AssetUpdateSuccessEventHandler += value;
            }
            remove
            {
                m_AssetUpdateSuccessEventHandler -= value;
            }
        }

        private void _OnAssetUpdateSuccess()
        {

        }

        public event EventHandler<AssetUpdateFailureEventArgs> AssetUpdateFailure
        {
            add
            {
                m_AssetUpdateFailureEventHandler += value;
            }
            remove
            {
                m_AssetUpdateFailureEventHandler -= value;
            }
        }

        private void _OnAssetUpdateFailure()
        {

        }

        public event EventHandler<AssetUpdateFinishEventArgs> AssetUpdateFinish
        {
            add
            {
                m_AssetUpdateFinishEventHandler += value;
            }
            remove
            {
                m_AssetUpdateFinishEventHandler -= value;
            }
        }

        private void _OnAssetUpdateFinish()
        {

        }
    }
}

