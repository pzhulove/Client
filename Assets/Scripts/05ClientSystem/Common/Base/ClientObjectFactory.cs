using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class ClientObjectFactory : MonoBehaviour
{
    [Serializable]
    public class FrameBinder
    {
        public GameObject goLocal;
        public string varName;
        public CachedObjectBehavior.UIBinder.BinderType eBinderType = CachedObjectBehavior.UIBinder.BinderType.BT_INVALID;
    }

    [Serializable]
    public class FrameBinderGroup
    {
        public FrameBinder[] m_akFrameBinders = null;
        public string Tag = null;
    }

    public FrameBinderGroup[] m_akBinderGroups = null;
    public CachedObjectBehavior[] m_akObjects = null;
    public string m_kPrefabPath = null;

    public void OnOpenFrame()
    {
        for(int i = 0; i < m_akObjects.Length; ++i)
        {
            if(m_akObjects[i] != null && m_akObjects[i].IsOpenCreate)
            {
                m_akObjects[i].Create();
            }
        }
    }
    public void OnCloseFrame()
    {

    }
}