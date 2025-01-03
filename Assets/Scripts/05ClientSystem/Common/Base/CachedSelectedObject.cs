using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace GameClient
{
    class CachedNormalObject<T> : CachedObject where T : class,new()
    {
        private bool bCreate = false;
        protected GameObject goLocal;
        public GameObject goPrefab;
        public GameObject goParent;
        protected T data;
        protected bool bUseTemplate = false;
        public T Value
        {
            get
            {
                return data;
            }
        }

        public override void OnCreate(object[] param)
        {
            goParent = param[0] as GameObject;
            goPrefab = param[1] as GameObject;
            data = param[2] as T;
            bUseTemplate = (bool)param[3];

            if (goLocal == null)
            {
                if(bUseTemplate)
                {
                    goLocal = goPrefab;
                }
                else
                {
                    goLocal = GameObject.Instantiate(goPrefab) as GameObject;
                }
                Utility.AttachTo(goLocal, goParent);
            }

            if(!bCreate)
            {
                Initialize();
                bCreate = true;
            }
            Enable();
            SetAsLastSibling();
            OnUpdate();
        }

        public override void OnDestroy()
        {
            UnInitialize();
            goLocal = null;
            data = null;
            goPrefab = null;
            goParent = null;
            bCreate = false;
        }

        public override void Enable()
        {
            if (goLocal != null)
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

        public override void SetAsLastSibling()
        {
            if (goLocal != null)
            {
                goLocal.transform.SetAsLastSibling();
            }
        }

        public void SetSiblingIndex(int iIndex)
        {
            if(null != goLocal)
            {
                goLocal.transform.SetSiblingIndex(iIndex);
            }
        }

        public override void OnRefresh(object[] param)
        {
            if(param != null && param.Length > 0)
            {
                data = param[0] as T;
            }
            OnUpdate();
        }

        public override void OnRecycle()
        {
            Disable();
        }

        public override void OnDecycle(object[] param)
        {
            OnCreate(param);
        }

        public virtual void Initialize()
        {

        }

        public virtual void UnInitialize()
        {

        }

        public virtual void OnUpdate()
        {

        }
    }
}