using System;
using System.Diagnostics;
using UnityEngine;


namespace Tenmove.Runtime.Unity
{
    internal sealed partial class UnityGameObjectPool
    {
        internal sealed class Object : Recyclable
        {
            private string m_Name;
            private GameObject m_GameObject;
            private int m_InstanceID;
            //private TMUnityGameObjectComponent m_Component;
            private Vector3 m_OriginScale;

            public Object()
            {
            }

            public bool Fill(string name, GameObject go)
            {
                if (null == go)
                {
                    Debugger.LogError("Game object can not be null!");
                    return false;
                }

                if (string.IsNullOrEmpty(name))
                {
                    Debugger.LogError("Name can not be null or empty string!");
                    return false;
                }

                m_Name = name;
                m_GameObject = go;
                m_InstanceID = go.GetInstanceID();
                m_OriginScale = go.transform.localScale;
                //m_Component = m_GameObject.GetComponent<TMUnityGameObjectComponent>();
                //if (null == m_Component)
                //    m_Component = m_GameObject.AddComponent<TMUnityGameObjectComponent>();

                return true;
            }

            public sealed override void OnReuse()
            {
                base.OnReuse();

                if(null != m_GameObject)
                {
                    //m_GameObject.SetActive(true);
                    m_GameObject.transform.localPosition = Vector3.zero;
                    m_GameObject.transform.localRotation = Quaternion.identity;
                    m_GameObject.transform.localScale = m_OriginScale;

                    /// StackTrace stackTrace = new StackTrace(true);
                    /// for (int i = 0, icnt = stackTrace.FrameCount; i < icnt; ++i)
                    /// {
                    ///     StackFrame curFrame = stackTrace.GetFrame(i);
                    ///     if (null == curFrame) continue;
                    ///     m_Component.PrefabKey = string.Format("{0} {1}(File:{2})\n\n",
                    ///         m_Component.PrefabKey,
                    ///         curFrame.GetMethod().ToString(),
                    ///         Tenmove.Runtime.Utility.Path.GetFileName(curFrame.GetFileName()));
                    /// }
                }
                else
                    Debugger.LogWarning("Game object named '{0}' is already be destroyed!", Name);
            }

            public sealed override void OnRecycle()
            {
                base.OnRecycle();
                if(null != m_GameObject)
                    m_GameObject.SetActive(false);
                else
                    Debugger.LogWarning("Game object named '{0}' is already be destroyed!", Name);

            }

            public sealed override void OnRelease()
            {
                if (null != m_GameObject)
                    GameObject.Destroy(m_GameObject);
            }

            public string Name
            {
                get { return m_Name; }
            }

            public GameObject GameObject
            {
                get { return m_GameObject; }
            }

            public sealed override bool IsValid
            {
                get { return null != m_GameObject; }
            }

            public int InstanceID
            {
                get { return m_InstanceID; }
            }
        }
    }
}

