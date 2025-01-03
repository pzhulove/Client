
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using XUPorterJSON;

namespace Tenmove.Runtime.Unity
{
    public class AssetGabageCollector 
    {
        private readonly ITMAssetManager m_AssetManager;

        private AsyncOperation m_UnloadAsync;
        private bool m_IsUnloadingAssets;
        private long m_PauseStamp;
        
        private event EventHandler<PurgePoolEventArgs> m_OnPurgePoolEventHandler;
        private event EventHandler<UnloadUnusedAssetEventArgs> m_OnUnloadUnusedAssetEventHandler;
        private event Action<bool> m_OnUnloadUnusedAssetStateChanged;



        public AssetGabageCollector()
        {
            m_AssetManager = ModuleManager.GetModule<ITMAssetManager>();
            m_IsUnloadingAssets = false;
            m_UnloadAsync = null;
            m_PauseStamp = 0;
        }

        public string ReadOnlyPath
        {
            get { return m_AssetManager.ReadOnlyPath; }
        }

        public string ReadWritePath
        {
            get { return m_AssetManager.ReadWritePath; }
        }

        public event EventHandler<PurgePoolEventArgs> OnPurgePoolEventHandler
        {
            add { m_OnPurgePoolEventHandler += value; }
            remove { m_OnPurgePoolEventHandler = value; }
        }

        public event EventHandler<UnloadUnusedAssetEventArgs> OnUnloadUnusedAssetEventHandler
        {
            add { m_OnUnloadUnusedAssetEventHandler += value; }
            remove { m_OnUnloadUnusedAssetEventHandler = value; }
        }

        public event Action<bool> OnUnloadUnusedAssetStateChanged
        {
            add { m_OnUnloadUnusedAssetStateChanged += value; }
            remove { m_OnUnloadUnusedAssetStateChanged = value; }
        }

        public bool Init()
        {
            ITMAssetManager assetManager = ModuleManager.GetModule<ITMAssetManager>();
            OnUnloadUnusedAssetEventHandler += assetManager.OnUnloadUnusedAsset;
            OnUnloadUnusedAssetStateChanged += assetManager.OnUnloadAssetStateChanged;

            ITMUnityGameObjectPool gameObjectPool = ModuleManager.GetModule<ITMUnityGameObjectPool>();
            OnPurgePoolEventHandler += gameObjectPool.OnPurgePool;
            OnUnloadUnusedAssetStateChanged += gameObjectPool.OnUnloadAssetStateChanged;

            return true;
        }
        
        public IEnumerator ClearUnusedAssetAsync(HashSet<string> reserveAssets)
        {
            /// 首先释放池中的资源
            if (null != m_OnPurgePoolEventHandler)
                m_OnPurgePoolEventHandler(this, new PurgePoolEventArgs(reserveAssets));


            /// 这里需要等一帧，GameObjectPool中的GameObject调用Destroy后，不会马上被销毁，导致AssetPool中不能释放资源。
            yield return null;

            /// 然后通知资源管理器释放未使用的资源
            if (null != m_OnUnloadUnusedAssetEventHandler)
                m_OnUnloadUnusedAssetEventHandler(this, new UnloadUnusedAssetEventArgs());

            if (null == m_UnloadAsync)
            {
                m_UnloadAsync = Resources.UnloadUnusedAssets();
                if (null != m_UnloadAsync)
                {
                    //Debug.LogError("Unload unused assets pause asset async-loading!");
                    _ChangeUnloadUnusedAssetState(true);
                    m_PauseStamp = Utility.Time.GetTicksNow();
                }
            }
        }

        public void Collect(int generation)
        {
            GC.Collect(generation);
        }

        public void Update()
        {
            if (null != m_UnloadAsync)
            {
                if (m_UnloadAsync.isDone)
                {
//                    Debug.LogError("Unload unused assets has finished! resume asset async-loading!");
                    GC.Collect();
                    _ChangeUnloadUnusedAssetState(false);
                    m_UnloadAsync = null;
                }

                if(Utility.Time.TicksToSeconds(Utility.Time.GetTicksNow() - m_PauseStamp) > 10)
                {
                    Debug.LogError("Pausing asset async-loading more than 10 second must has bug, Auto resume!");
                    _ChangeUnloadUnusedAssetState(false);
                    m_UnloadAsync = null;
                }
            }
        }

        public void Deinit()
        {
            ITMAssetManager assetManager = ModuleManager.GetModule<ITMAssetManager>();
            OnUnloadUnusedAssetEventHandler -= assetManager.OnUnloadUnusedAsset;
            OnUnloadUnusedAssetStateChanged -= assetManager.OnUnloadAssetStateChanged;

            ITMUnityGameObjectPool gameObjectPool = ModuleManager.GetModule<ITMUnityGameObjectPool>();
            OnPurgePoolEventHandler -= gameObjectPool.OnPurgePool;
            OnUnloadUnusedAssetStateChanged -= gameObjectPool.OnUnloadAssetStateChanged;
        }

        private void _ChangeUnloadUnusedAssetState(bool isUnloadingAsset)
        {
            m_IsUnloadingAssets = isUnloadingAsset;
            if (null != m_OnUnloadUnusedAssetStateChanged)
                m_OnUnloadUnusedAssetStateChanged(m_IsUnloadingAssets);
        }
    }

}
