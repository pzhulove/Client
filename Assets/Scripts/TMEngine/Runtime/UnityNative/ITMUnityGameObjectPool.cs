using UnityEngine;
using System.Collections.Generic;

namespace Tenmove.Runtime.Unity
{
    public enum GameObjectUsage
    {
        Default,
        UI,
        Scene,
        Actor,
        Spark,
    }

    /// <summary>
    /// 注意！ReserveAssetKeys是要保留的资源名称
    /// </summary>
    public sealed class PurgePoolEventArgs : BaseEventArgs
    {
        public PurgePoolEventArgs(HashSet<string> reserveAssetKeys)
        {
            ReserveAssetKeys = reserveAssetKeys;
        }

        public HashSet<string> ReserveAssetKeys
        {
            private set;
            get;
        }
    }

    public struct UnityGameObjectPoolInfo
    {
        public UnityGameObjectPoolInfo(string prefabRes,GameObjectUsage usage,int reserveCount,
            float expireTime,int priority,int unusedObjectCount,int usingObjectCount,
            int acquireCount,int recycleCount,int createCount,int releaseCount)
        {
            m_PrefabResPath= prefabRes;
            m_ObjectUsage = usage;
            m_ReserveCount = reserveCount;
            m_ExpireTime = expireTime;
            m_Priority = priority;
            m_UnusedObjectCount = unusedObjectCount;
            m_UsingObjectCount = usingObjectCount;
            m_AcquireCount = acquireCount;
            m_RecycleCount = recycleCount;
            m_CreateCount = createCount;
            m_ReleaseCount = releaseCount;
        }

        public string           m_PrefabResPath;
        public GameObjectUsage   m_ObjectUsage;
        public int              m_ReserveCount;
        public float            m_ExpireTime;
        public int              m_Priority;
        public int              m_UnusedObjectCount;
        public int              m_UsingObjectCount;
        public int              m_AcquireCount;
        public int              m_RecycleCount;
        public int              m_CreateCount;
        public int              m_ReleaseCount;
    }

    public interface ITMUnityGameObjectPool
    {
        /// <summary>
        /// 获取所有GameObject池子数
        /// </summary>
        /// <returns>所有GameObject池子数</returns>
        int GameObjectPoolCount
        {
            get;
        }

        /// <summary>
        /// 设置池子根节点
        /// </summary>
        /// <param name="root">设置池子根节点</param>
        void SetPoolRootNode(GameObject root);

        /// <summary>
        /// 设置对应资源的对象池相关参数
        /// </summary>
        /// <param name="prefabRes">目标GameObject的资源路径</param>
        /// <param name="objectUsage">目标GameObject的资源使用方式</param>
        /// <param name="reserouceCount">目标GameObject预保留数量</param>
        /// <param name="expireTime">目标GameObject的过期时间</param>
        /// <param name="priority">目标GameObject的加载优先级</param>
        void SetObjectPoolDesc(string prefabRes, GameObjectUsage objectUsage, int reserveCount, float expireTime, int priority);
 
        /// <summary>
        /// 同步获取目标资源类型的GameObject
        /// </summary>
        /// <param name="prefabRes">目标GameObject的资源路径</param>
        /// <param name="flag">请求标志</param>
        /// <returns>目标资源路径对应的GameObject</returns>
        GameObject AcquireGameObjectSync(string prefabRes, uint flag);

        /// <summary>
        /// 异步获取目标资源类型的GameObject
        /// </summary>
        /// <param name="prefabRes">目标GameObject的资源路径</param>
        /// <param name="userData">自定义数据</param>
        /// <param name="callbacks">异步完成回调</param>
        /// <param name="flag">请求标志</param>
        /// <returns>本次请求ID</returns>
        int AcquireGameObjectAsync(string prefabRes, object userData, AssetLoadCallbacks<object> callbacks, uint flag);

        /// <summary>
        /// 回收使用完毕的GameObject
        /// </summary>
        /// <param name="gameObject"></param>
        void RecycleGameObject(GameObject gameObject);

        /// <summary>
        /// 清理对象池
        /// </summary>
        /// <param name="clearAll">是否清理所有的对象 true表示不预留任何对象，false保留预留数量的对象</param>
        void PurgePool(HashSet<string> reserveAssets,bool clearAll);

        /// <summary>
        /// 获取所有目标GameObject的池子的信息
        /// </summary>
        /// <param name="poolInfoList">所有的池子信息</param>
        void GetAllPoolInfo(ref List<UnityGameObjectPoolInfo> poolInfoList);

        /// <summary>
        /// 当GC管理器准备清理对象池的注册回调
        /// </summary>
        /// <param name="sender">事件发送者</param>
        /// <param name="args">事件参数</param>
        void OnPurgePool(object sender, PurgePoolEventArgs args);
        void OnUnloadAssetStateChanged(bool isPurgingAsset);
    }
}