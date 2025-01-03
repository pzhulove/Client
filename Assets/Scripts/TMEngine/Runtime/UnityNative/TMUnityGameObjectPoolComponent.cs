using System.Collections.Generic;

using UnityEngine;

namespace Tenmove.Runtime.Unity
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Tenmove/GameObjectPoolModule")]
    public class TMUnityGameObjectPoolComponent : TMModuleComponent
    {
        private ITMUnityGameObjectPool m_UnityGameObjectPool = null;

        public int GameObjectAssetCount
        {
            get { return m_UnityGameObjectPool.GameObjectPoolCount; }
        }

        public void GetAllPoolInfo(ref List<UnityGameObjectPoolInfo> poolInfoList)
        {
            m_UnityGameObjectPool.GetAllPoolInfo(ref poolInfoList);
        }

        /// <summary>
        /// 游戏框架组件初始化。
        /// </summary>
        protected sealed override void Awake()
        {
            base.Awake();

            m_UnityGameObjectPool = ModuleManager.GetModule<ITMUnityGameObjectPool>();
            if (m_UnityGameObjectPool == null)
            {
                Debugger.LogError("Unity game-object pool is invalid!");
                return;
            }

            m_UnityGameObjectPool.SetPoolRootNode(gameObject);
        }

        private void Start()
        {
        }
    }
}