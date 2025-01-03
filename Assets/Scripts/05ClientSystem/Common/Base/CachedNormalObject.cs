using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace GameClient
{
    class CachedSelectedObject<T,F> : CachedObject where T : class,new() where F : class,new()
    {
        protected GameObject goLocal;
        public static CachedSelectedObject<T, F> Selected = null;
        public delegate void OnSelectedDelegate(T data);
        public OnSelectedDelegate onSelected;
        public GameObject goPrefab;
        public GameObject goParent;
        protected Toggle toggle;
        protected T data;
        protected bool bUseTemplate = false;
        public T Value
        {
            get
            {
                return data;
            }
        }
        public bool isOn
        {
            get
            {
                return toggle != null ? toggle.isOn : false;
            }
        }

        public static void Clear()
        {
            if(Selected != null)
            {
                Selected.OnDisplayChanged(false);
                Selected = null;
            }
        }

        public void OnSelected()
        {
            if(Selected != this)
            {
                if (Selected != null)
                {
                    Selected.OnDisplayChanged(false);
                }

                Selected = this;
                if (toggle.isOn != true)
                {
                    toggle.isOn = true;
                }
                Selected.OnDisplayChanged(true);

                if (onSelected != null)
                {
                    onSelected.Invoke(Value);
                }
            }
        }

        public override void OnCreate(object[] param)
        {
            goParent = param[0] as GameObject;
            goPrefab = param[1] as GameObject;
            data = param[2] as T;
            onSelected = param[3] as OnSelectedDelegate;
            if(param.Length > 4)
            {
                bUseTemplate = (bool)param[4];
            }

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
            toggle = goLocal.GetComponent<Toggle>();
            if(toggle == null)
            {
                toggle = goLocal.GetComponentInChildren<Toggle>();
            }
            if (toggle != null)
            {
                toggle.onValueChanged.RemoveAllListeners();
                toggle.onValueChanged.AddListener((bool bValue) =>
                {
                    if (bValue && onSelected != null)
                    {
                        if (Selected != this)
                        {
                            if (Selected != null)
                            {
                                Selected.OnDisplayChanged(false);
                            }
                            Selected = this;
                        }
                        Selected.OnDisplayChanged(true);
                        onSelected.Invoke(data);
                    }
                });
            }
            Initialize();
            Enable();
            SetAsLastSibling();
            OnUpdate();
        }

        public override void OnDestroy()
        {
            UnInitialize();

            onSelected = null;
            if (toggle != null)
            {
                toggle.onValueChanged.RemoveAllListeners();
                toggle = null;
            }
            goLocal = null;
            data = null;
            goPrefab = null;
            goParent = null;
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
            if (Selected == this)
            {
                Clear();
            }
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

        public virtual void OnDisplayChanged(bool bShow)
        {

        }

		public virtual void OnFrameUpdate()
		{
			
		}
    }
}