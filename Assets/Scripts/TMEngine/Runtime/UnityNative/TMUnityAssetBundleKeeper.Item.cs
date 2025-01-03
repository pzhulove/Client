using System.Collections.Generic;
using UnityEngine;

namespace TMEgnine.Runtime.Unity
{
    internal delegate void OnAssetBundleLoadSucceed(AssetBundle assetBundle);
    internal delegate void OnAssetBundleLoadFailed(string path);
    internal delegate void OnAssetBundleLoadUpdate(float progress);

    internal partial class UnityAssetBundleKeeper
    {
        public class RegisterItem
        {
            private readonly UnityAssetBundleKeeper m_Keeper;
            private readonly string m_AssetBundlePath;

            private AssetBundle m_AssetBundle;
            private AssetBundleCreateRequest m_AssetBundleCreateRequest;
            private uint m_ID;
            private uint m_RefCount;

            private class AsyncCallback
            {
                private OnAssetBundleLoadSucceed m_OnSucceed;
                private OnAssetBundleLoadFailed m_OnFailed;
                private OnAssetBundleLoadUpdate m_OnUpdate;

                public AsyncCallback(OnAssetBundleLoadSucceed onSuccess, OnAssetBundleLoadFailed onFailure, OnAssetBundleLoadUpdate onUpdate)
                {
                    m_OnSucceed = onSuccess;
                    m_OnFailed = onFailure;
                    m_OnUpdate = onUpdate;
                }

                public void OnSuccess(AssetBundle bundle)
                {
                    if (null != m_OnSucceed)
                        m_OnSucceed(bundle);
                }

                public void OnFailure(string path)
                {
                    if (null != m_OnFailed)
                        m_OnFailed(path);
                }

                public void OnUpdate(float progress)
                {
                    if (null != m_OnUpdate)
                        m_OnUpdate(progress);
                }
            }

            private readonly LinkedList<AsyncCallback> m_CallbackList;

            public RegisterItem(UnityAssetBundleKeeper keeper, uint id, string path)
            {
                if (null == keeper)
                    Debugger.LogWarning("Parameter 'keeper' can not be null!");

                if (~0u == id)
                    Debugger.LogWarning("Parameter 'id' is invalid!");

                if (string.IsNullOrEmpty(path))
                    Debugger.LogWarning("Parameter 'path' can not be null or empty string!");

                m_Keeper = keeper;
                m_ID = id;
                m_AssetBundlePath = path;

                m_AssetBundle = null;
                m_AssetBundleCreateRequest = null;

                m_CallbackList = new LinkedList<AsyncCallback>();
                m_RefCount = 0;
            }

            public uint ID
            {
                get { return m_ID; }
            }

            public bool LoadAsync(OnAssetBundleLoadSucceed onSuccess, OnAssetBundleLoadFailed onFailure, OnAssetBundleLoadUpdate onUpdate)
            {
                if (null != onSuccess)
                {
                    if (null != onFailure)
                    {
                        AsyncCallback ayncCallback = new AsyncCallback(onSuccess, onFailure, onUpdate);
                        if (null == m_AssetBundle)
                        {
                            if (null == m_AssetBundleCreateRequest)
                                m_AssetBundleCreateRequest = AssetBundle.LoadFromFileAsync(m_AssetBundlePath);
                            else
                                Debug.LogWarningFormat("Asset bundle '{0}' is already in asynchronize loading, maybe has a bug somewhere!", m_AssetBundlePath);
                        }
                        else
                            Debug.LogWarningFormat("Asset bundle '{0}' is already loaded, maybe has a bug somewhere!", m_AssetBundlePath);
                        
                        _AddAsyncCallback(ayncCallback);
                        return true;
                    }
                    else
                        Debugger.LogWarning("Parameter 'onFailure' can not be null!");
                }
                else
                    Debugger.LogWarning("Parameter 'onSuccess' can not be null!");

                return false;
            }

            public AssetBundle LoadSync()
            {
                if (null == m_AssetBundle)
                {
                    AssetBundle assetBundle = null;
                    if (null != m_AssetBundleCreateRequest)
                    {
                        assetBundle = m_AssetBundleCreateRequest.assetBundle;
                        m_AssetBundleCreateRequest = null;
                    }
                    else
                        assetBundle = AssetBundle.LoadFromFile(m_AssetBundlePath);

                    if (null != assetBundle)
                    {
                        m_AssetBundle = assetBundle;
                        _InvokeOnSuccess(assetBundle);
                    }
                    else
                    {
                        Debug.LogWarningFormat("Can not load asset bundle from file '{0}' which is not a valid asset bundle resource!", m_AssetBundlePath);
                        _InvokeOnFailure(m_AssetBundlePath);
                        return null;
                    }
                }

                ++m_RefCount;
                return m_AssetBundle;
            }

            public void Unload()
            {
                if (m_RefCount > 0)
                {
                    --m_RefCount;
                    if (0 == m_RefCount)
                    {
                        if (null != m_AssetBundle)
                        {
                            m_AssetBundle.Unload(true);
                            m_AssetBundle = null;
                        }
                        else
                            Debug.LogWarningFormat("Asset bundle '{0}' is null already, must has a bug somewhere!", m_AssetBundlePath);
                    }
                }
                else
                    Debug.LogWarningFormat("Asset bundle '{0}' reference count is already equal to zero, must has bug somewhere!", m_AssetBundlePath);
            }

            public void Update()
            {
                if (null != m_AssetBundleCreateRequest)
                {
                    if (m_AssetBundleCreateRequest.isDone)
                    {
                        AssetBundle assetBundle = m_AssetBundleCreateRequest.assetBundle;
                        m_AssetBundleCreateRequest = null;

                        if (null != assetBundle)
                        {
                            m_AssetBundle = assetBundle;
                            _InvokeOnSuccess(assetBundle);
                        }
                        else
                            _InvokeOnFailure(m_AssetBundlePath);
                    }
                    else
                    {
                        _InvokeAsyncUpdate(m_AssetBundleCreateRequest.progress);
                    }
                }
                else
                {
                    if(m_CallbackList.Count > 0)
                    {
                        if(null != m_AssetBundle)
                            _InvokeOnSuccess(m_AssetBundle);
                        else
                            _InvokeOnFailure(m_AssetBundlePath);
                    }
                }
            }

            private void _AddAsyncCallback(AsyncCallback asyncCallback)
            {
                ++m_RefCount;
                m_CallbackList.AddLast(asyncCallback);
                m_Keeper._AddUpdateItem(this);
            }

            private void _InvokeOnSuccess(AssetBundle assetBundle)
            {
                LinkedListNode<AsyncCallback> cur = m_CallbackList.First;
                LinkedListNode<AsyncCallback> next = null; ;
                while (null != cur)
                {
                    next = cur.Next;
                    cur.Value.OnSuccess(assetBundle);

                    m_CallbackList.Remove(cur);
                    cur = next;
                }
                
                m_Keeper._RemoveUpdateItem(this);
            }

            private void _InvokeOnFailure(string path)
            {
                LinkedListNode<AsyncCallback> cur = m_CallbackList.First;
                LinkedListNode<AsyncCallback> next = null; ;
                while (null != cur)
                {
                    next = cur.Next;
                    --m_RefCount;

                    cur.Value.OnFailure(path);
                    m_CallbackList.Remove(cur);
                    cur = next;
                }
                m_Keeper._RemoveUpdateItem(this);
            }

            private void _InvokeAsyncUpdate(float progress)
            {
                LinkedListNode<AsyncCallback> cur = m_CallbackList.First;
                while (null != cur)
                {
                    cur.Value.OnUpdate(progress);
                    cur = cur.Next;
                }
            }
        }
    }
}