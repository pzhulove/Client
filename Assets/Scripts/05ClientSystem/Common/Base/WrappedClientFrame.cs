using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;

namespace GameClient
{
    class WrappedClientFrame : ClientFrame
    {
        public class WrappedData
        {
            public WrappedData()
            {

            }
        }
        public class WrappedCachedItemObject<TFrame,TData> : CachedObject 
            where TFrame : WrappedClientFrame,new()
            where TData : WrappedData,new()
        {
            protected GameObject goLocal;
            protected CachedObjectBehavior dataScript;
            protected TData data;

            public virtual void Update()
            {

            }

            public override void OnRecycle()
            {
                if(goLocal != null)
                {
                    goLocal.CustomActive(false);
                }
            }

            public override void Enable()
            {
                if(goLocal != null)
                {
                    goLocal.CustomActive(true);
                }
            }

            public override void Disable()
            {
                if (goLocal != null)
                {
                    goLocal.CustomActive(false);
                }
            }

            public override bool NeedFilter(object[] param)
            {
                return false;
            }

            public override void SetAsLastSibling()
            {
                if(goLocal != null)
                {
                    goLocal.transform.SetAsLastSibling();
                }
            }

            public override void OnDestroy()
            {
                goLocal = null;
                dataScript = null;
                data = null;
            }
        }

        public GameObject gameObject
        {
            get
            {
                return frame;
            }
        }

        //ClientObjectFactory m_kFactory;
        protected override void _OnOpenFrame()
        {
            //m_kFactory = frame.GetComponent<ClientObjectFactory>();
            //if(m_kFactory != null)
            //{
            //    m_kFactory.OnOpenFrame();
            //}
        }

        protected override void _OnCloseFrame()
        {
            //if (m_kFactory != null)
            //{
            //    m_kFactory.OnCloseFrame();
            //}
        }
    }
}