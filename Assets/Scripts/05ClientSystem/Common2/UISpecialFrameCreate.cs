using UnityEngine;
using System.Collections;
using System.Reflection;

namespace GameClient
{
    class UISpecialFrameCreate : MonoBehaviour
    {
        void Create<T>(string path) where T : ClientFrame,new()
        {
            var clientFrame = ClientSystemManager.GetInstance().OpenFrame<T>(FrameLayer.Middle);
        }

        private string className = null;
        private string param0 = null;
        public string Param0
        {
            set
            {
                param0 = value;
            }
        }

        public string ClsName
        {
            get
            {
                return ClsName;
            }
            set
            {
                className = value;
                m_bDirty = true;
                MarkDirty();
            }
        }
        bool m_bDirty = false;
        IClientFrame m_clientFrame = null;

        void MarkDirty()
        {
            _CloseFrame();
            if (!string.IsNullOrEmpty(className) && m_bDirty)
            {
                Assembly assembly = Assembly.GetExecutingAssembly(); // 获取当前程序集 
                if (assembly != null)
                {
                    System.Type type = assembly.GetType(className);
                    if (type != null)
                    {
                        m_clientFrame = ClientSystemManager.GetInstance().OpenFrame(gameObject,type,param0);
                    }
                }
                m_bDirty = false;
            }
        }

        void _CloseFrame()
        {
            if(m_clientFrame != null)
            {
                m_clientFrame.Close(this);
                m_clientFrame = null;
            }
        }

        public void OnClose()
        {
            _CloseFrame();
        }

        void OnDestroy()
        {
            _CloseFrame();
        }
    }
}