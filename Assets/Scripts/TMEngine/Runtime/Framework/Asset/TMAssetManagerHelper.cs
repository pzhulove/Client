
using System.Collections.Generic;

namespace Tenmove.Runtime
{
    /// <summary>
    /// 接口分为以下几组：
    /// 1： 操作一个资源的标签（一个资源可以有多个标签，同样一个标签也可以赋给多个资源，实现资源进行分组操作）：
    /// 
    ///     void AddAssetLable(string assetName, string lable);           // 对一个资源添加标签
    ///     void RemoveAssetLable(string assetName, string lable);        // 移除一个资源的标签
    ///     void GetAssetLables(string assetName, List<string> lables);   // 获取一个资源的所有标签
    ///     void ClearAssetLables(string assetName);                      // 清除一个资源的所有标签
    ///     
    /// 2： 对标签的操作
    /// 
    ///     void DestroyLable(string lable);          // 销毁一个标签组，如果该组资源被lock了，会unlock
    ///     void ClearAllLables();                    // 清除所有资源标签组，并解除所有资源的锁定状态
    ///     void ClearAllLables(AssetLockTime lockTime);  // 清除某种类型的资源标签组，并解除其资源的锁定状态
    ///     
    /// 3： 资源锁定相关（锁定一个资源时，只能针对标签进行锁定，这样可以对一个资源添加多个标签，实现多个地方的锁定。比如
    ///     资源A和资源B，A添加了"Lable1"的标签，B添加了"Lable1"和"Lable2"的标签，当对两个Lable都进行Lock时，资源A，B
    ///     都会被锁定，之后对"Lable1"标签进行Unlock时，资源A被解除锁定，但资源B依然会处于锁定状态）
    ///     
    ///     void LockLable(string lable, AssetLockTime lockTime);              // 锁定一个标签组中的所有资源
    ///     void UnLockLable(string lable);                                     // 解除一个标签组中的所有资源的锁定
    ///     bool IsLableLocked(string lable);                                   // 判断一个标签组是否处于锁定状态
    ///     bool IsAssetLocked(string assetName);                               // 判断一个资源是否处于锁定状态
    ///
    /// </summary>
    public enum AssetLockPhase
    {
        DuringScene,           // 持续一个场景，当GeScene发生改变时自动清除Lock
        DuringGameSystem,      // 持续一个GameSystem，当GameSystem发生改变时自动清除Lock
        DuringGame,            // 持续整个游戏，需手动解除lock
    }

    public static class TMAssetManagerHelper
    {
        private static Dictionary<string, HashSet<string>> m_AssetLables = new Dictionary<string, HashSet<string>>();

        private static Dictionary<string, AssetLockPhase> m_Locklables = new Dictionary<string, AssetLockPhase>();
        private static Dictionary<string, int> m_LockedAssets = new Dictionary<string, int>();

        private static ITMAssetManager m_AssetManager = null;

        private static List<string> m_DelLables = new List<string>();
        private static ITMAssetManager _GetAssetManager()
        {
            if (null == m_AssetManager)
                m_AssetManager = ModuleManager.GetModule<Tenmove.Runtime.ITMAssetManager>();

            return m_AssetManager;
        }

        /// <summary>
        /// 对一个资源添加标签
        /// </summary>
        /// <param name="assetName"> 资源名 </param>
        /// <param name="lable"> 标签名 </param>
        public static void AddAssetLable(string assetName, string lable)
        {
            HashSet<string> assetNames;
            if (!m_AssetLables.TryGetValue(lable, out assetNames))
            {
                assetNames = new HashSet<string>();
                m_AssetLables.Add(lable, assetNames);
            }

            // 如果该资源标签组处于被lock状态，该资源需要被lock
            if(assetNames.Add(assetName) && m_Locklables.ContainsKey(lable))
            {
                LockAsset(assetName);
            }
        }

        /// <summary>
        /// 移除一个资源的标签
        /// </summary>
        /// <param name="assetName"> 资源名 </param>
        /// <param name="lable"> 标签名 </param>
        public static void RemoveAssetLable(string assetName, string lable)
        {
            HashSet<string> assetNames;
            if (m_AssetLables.TryGetValue(lable, out assetNames))
            {
                if (assetNames.Remove(assetName) && m_Locklables.ContainsKey(lable))
                {
                    UnLockAsset(assetName);
                }
            }
        }

        /// <summary>
        /// 获取一个资源的所有标签
        /// </summary>
        /// <param name="assetName"> 资源名 </param>
        /// <param name="lable"> 存放标签名的数组 </param>
        public static void GetAssetLables(string assetName, List<string> lables)
        {
            lables.Clear();
            Dictionary<string, HashSet<string>>.Enumerator itr = m_AssetLables.GetEnumerator();
            while (itr.MoveNext())
            {
                HashSet<string> assetNames = itr.Current.Value;
                if (assetNames.Contains(assetName))
                {
                    lables.Add(itr.Current.Key);
                }
            }
        }

        /// <summary>
        /// 清除一个资源的所有标签
        /// </summary>
        /// <param name="assetName"> 资源名 </param>
        public static void ClearAssetLables(string assetName)
        {
            Dictionary<string, HashSet<string>>.Enumerator itr = m_AssetLables.GetEnumerator();
            while(itr.MoveNext())
            {
                HashSet<string> assetNames = itr.Current.Value;
                if(assetNames.Remove(assetName) && m_Locklables.ContainsKey(itr.Current.Key))
                {
                    UnLockAsset(assetName);
                }
            }
        }

        /// <summary>
        /// 销毁一个标签组，如果该组资源被lock了，会unlock。
        /// </summary>
        /// <param name="lable"> 标签名 </param>
        public static void DestroyLable(string lable)
        {
            // 如果该资源标签组被Lock了，需要unlock
            UnLockLable(lable);
            m_AssetLables.Remove(lable);
        }

        /// <summary>
        /// 锁定一个标签组中的所有资源
        /// </summary>
        /// <param name="lable"> 标签名 </param>
        public static void LockLable(string lable, AssetLockPhase lockTime)
        {
            AssetLockPhase preLockTime;
            // 如果有设置更长时间的lock，设置为长时间的。
            if(m_Locklables.TryGetValue(lable, out preLockTime))
            {
                if(lockTime > preLockTime)
                {
                    m_Locklables[lable] = lockTime;
                }

                return;
            }

            m_Locklables.Add(lable, lockTime);
            HashSet<string> assetNames;
            if (m_AssetLables.TryGetValue(lable, out assetNames))
            {
                HashSet<string>.Enumerator itr = assetNames.GetEnumerator();
                while (itr.MoveNext())
                {
                    LockAsset(itr.Current);
                }
            }
        }

        /// <summary>
        /// 解除锁定一个标签组中的所有资源
        /// </summary>
        /// <param name="lable"> 标签名 </param>
        public static void UnLockLable(string lable)
        {
            if (m_Locklables.Remove(lable))
            {
                HashSet<string> assetNames;
                if (m_AssetLables.TryGetValue(lable, out assetNames))
                {
                    HashSet<string>.Enumerator itr = assetNames.GetEnumerator();
                    while (itr.MoveNext())
                    {
                        UnLockAsset(itr.Current);
                    }
                }
            }
        }

        /// <summary>
        /// 判断一个资源是否处于锁定状态
        /// </summary>
        /// <param name="assetName"> 资源名 </param>
        /// <returns></returns>
        public static bool IsAssetLocked(string assetName)
        {
            return m_LockedAssets.ContainsKey(assetName);
        }

        /// <summary>
        /// 判断一个标签组是否处于锁定状态
        /// </summary>
        /// <param name="assetName"> 标签组名 </param>
        /// <returns></returns>
        public static bool IsLableLocked(string assetName)
        {
            return m_Locklables.ContainsKey(assetName);
        }

        /// <summary>
        /// 清除所有资源标签组，并解除所有资源的锁定状态
        /// </summary>
        public static void ClearAllLables()
        {
            m_Locklables.Clear();
            m_AssetLables.Clear();

            Dictionary<string, int>.Enumerator itr = m_LockedAssets.GetEnumerator();
            while (itr.MoveNext())
            {
                UnLockAssetImpl(itr.Current.Key);
            }

            m_LockedAssets.Clear();
        }

        public static void ClearAllLables(AssetLockPhase lockTime)
        {
            m_DelLables.Clear();

            var itr = m_Locklables.GetEnumerator();
            while(itr.MoveNext())
            {
                if(itr.Current.Value == lockTime)
                {
                    m_DelLables.Add(itr.Current.Key);
                }
            }

            for(int i = 0; i < m_DelLables.Count; ++i)
            {
                DestroyLable(m_DelLables[i]);
            }

             m_DelLables.Clear();
        }

        /// <summary>
        /// 外部不允许直接对Asset进行lock和unlock，避免不匹配数量的lock和unlock发生，只能对资源标签组进行操作。
        /// </summary>
        /// <param name="assetName"></param>
        private static void LockAsset(string assetName)
        {
            int lockCount;
            if(m_LockedAssets.TryGetValue(assetName, out lockCount))
            {
                lockCount++;

                m_LockedAssets[assetName] = lockCount;
            }
            else
            {
                m_LockedAssets.Add(assetName, 1);
                LockAssetImpl(assetName);
            }
        }

        private static void UnLockAsset(string assetName)
        {
            int lockCount;
            if (m_LockedAssets.TryGetValue(assetName, out lockCount))
            {
                lockCount--;
                if (lockCount > 0)
                {
                    m_LockedAssets[assetName] = lockCount;
                }
                else
                {
                    m_LockedAssets.Remove(assetName);
                    UnLockAssetImpl(assetName);
                }
            }
        }

        private static void LockAssetImpl(string assetName)
        {
            ITMAssetManager assetManager = _GetAssetManager();
            if(assetManager != null)
            {
                assetManager.LockAsset(assetName, true);
            }
        }

        private static void UnLockAssetImpl(string assetName)
        {
            ITMAssetManager assetManager = _GetAssetManager();
            if (assetManager != null)
            {
                assetManager.LockAsset(assetName, false);
            }
        }

        public static void OnBeforeSceneUnload()
        {
            ClearAllLables(AssetLockPhase.DuringScene);
        }

        public static void OnBeforeGameSystemLeave()
        {
            ClearAllLables(AssetLockPhase.DuringScene);
            ClearAllLables(AssetLockPhase.DuringGameSystem);
        }
    }
}
