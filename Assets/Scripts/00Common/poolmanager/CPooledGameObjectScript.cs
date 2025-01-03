using System;
using UnityEngine;

public class CPooledGameObjectScript : MonoBehaviour
{
    public Vector3 m_defaultScale;
    public bool m_isInit;
    public IPooledMonoBehaviour[] m_pooledMonoBehaviours;
    public string m_prefabKey;

    public bool m_IsOriginInVisible = false;
    public bool m_IsRecycled = false;
    
    /// public string m_DebugInfo = null;
}

