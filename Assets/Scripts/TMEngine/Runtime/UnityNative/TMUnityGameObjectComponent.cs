using System;
using UnityEngine;


namespace Tenmove.Runtime.Unity
{
    public class TMUnityGameObjectComponent : MonoBehaviour
    {
//         [System.NonSerialized]
//         private TMUnityGameObjectManager.Object m_OwnerObject;

        [SerializeField]
        private Vector3 m_DefaultScale;
        [SerializeField]
        private bool m_IsInit;
        [SerializeField]
        private string m_PrefabKey;

        [SerializeField]
        private bool m_IsOriginInVisible = false;
        [System.NonSerialized]
        private bool m_IsRecycled = false;

        public Vector3 DefaultScale
        {
            get { return m_DefaultScale; }
            internal set { m_DefaultScale = value; }
        }

        public bool IsInit
        {
            get { return m_IsInit; }
            internal set { m_IsInit = value; }
        }

        public string PrefabKey
        {
            get { return m_PrefabKey; }
            internal set { m_PrefabKey = value; }
        }

        public bool IsOriginInVisible
        {
            get { return m_IsOriginInVisible; }
            internal set { m_IsOriginInVisible = value; }
        }

        public bool IsRecycled
        {
            get { return m_IsRecycled; }
            internal set { m_IsRecycled = value; }
        }

        /// private void OnDestroy()
        /// {
        ///     Debugger.LogError("Game object '{0}' will be destroy!",gameObject.name);
        /// }
    }
}

