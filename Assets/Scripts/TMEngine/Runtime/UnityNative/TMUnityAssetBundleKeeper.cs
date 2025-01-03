


using System.Collections.Generic;
using UnityEngine;
using Tenmove.Runtime;

namespace TMEgnine.Runtime.Unity
{
    internal partial class UnityAssetBundleKeeper
    {
        private static UnityAssetBundleKeeper sm_Instance = null;

        private readonly Dictionary<string, RegisterItem> m_AssetBundleRegTable;
        private readonly LinkedList<RegisterItem> m_UpdateRegisterList;
        private uint m_AllocIDCount;

        private UnityAssetBundleKeeper()
        {
            m_AssetBundleRegTable = new Dictionary<string, RegisterItem>();
            m_UpdateRegisterList = new LinkedList<RegisterItem>();
            m_AllocIDCount = 0;
        }

        private static UnityAssetBundleKeeper Instance
        {
            get
            {
                if (null == sm_Instance)
                    sm_Instance = new UnityAssetBundleKeeper();

                return sm_Instance;
            }
        }

        public static void Update()
        {
            Instance._OnUpdate();
        }

        public static AssetBundle LoadAssetBundleSync(string packageFullpath)
        {
            RegisterItem item = Instance._GetRegisterItem(packageFullpath);
            return item.LoadSync();
        }

        public static bool LoadAssetBundleAsync(string packageFullpath, OnAssetBundleLoadSucceed onSuccess, OnAssetBundleLoadFailed onFailure, OnAssetBundleLoadUpdate onUpdate)
        {
            RegisterItem item = Instance._GetRegisterItem(packageFullpath);
            return item.LoadAsync(onSuccess, onFailure, onUpdate);
        }

        public static void UnloadAssetBundle(string packageFullpath)
        {
            RegisterItem item = Instance._GetRegisterItem(packageFullpath);
            item.Unload();
        }

        private RegisterItem _GetRegisterItem(string packageFullpath)
        {
            RegisterItem item = null;
            string packageName = Tenmove.Runtime.Utility.Path.GetFileName(packageFullpath);
            if (!m_AssetBundleRegTable.TryGetValue(packageName, out item))
            {
                item = new RegisterItem(this, m_AllocIDCount++, packageFullpath);
                m_AssetBundleRegTable.Add(packageName, item);
            }

            return item;
        }

        private void _AddUpdateItem(RegisterItem item)
        {
            LinkedListNode<RegisterItem> cur = m_UpdateRegisterList.First;
            while (null != cur)
            {
                if (cur.Value.ID == item.ID)
                    return;

                cur = cur.Next;
            }

            m_UpdateRegisterList.AddLast(item);
        }

        private void _RemoveUpdateItem(RegisterItem item)
        {
            LinkedListNode<RegisterItem> cur = m_UpdateRegisterList.First;
            while (null != cur)
            {
                if (cur.Value.ID == item.ID)
                {
                    m_UpdateRegisterList.Remove(cur);
                    return;
                }

                cur = cur.Next;
            }
        }

        private void _OnUpdate()
        {
            LinkedListNode<RegisterItem> cur = m_UpdateRegisterList.First;
            while (null != cur)
            {
                cur.Value.Update();
                cur = cur.Next;
            }
        }
    }
}