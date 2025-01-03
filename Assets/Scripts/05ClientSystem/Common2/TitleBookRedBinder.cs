using UnityEngine;
using System.Collections;

namespace GameClient
{
    class TitleBookRedBinder : MonoBehaviour
    {
        public GameObject goTarget;
        // Use this for initialization
        void Start()
        {
            if(goTarget == null)
            {
                goTarget = gameObject;
            }

            _UpdateRedPoint();
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.RedPointChanged, _OnRedPointChanged);
        }

        void OnDestroy()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.RedPointChanged, _OnRedPointChanged);
        }

        void _OnAddTittle(ulong uid)
        {
            _UpdateRedPoint();
        }

        void _OnRemoveTitle(ulong uid)
        {
            _UpdateRedPoint();
        }

        void _OnUpdateTitle(ulong uid)
        {
            _UpdateRedPoint();
        }

        void _OnTittleMarkChanged(TittleComeType eTittleComeType)
        {
            _UpdateRedPoint();
        }

        void _OnRedPointChanged(UIEvent uiEvent)
        {
            _UpdateRedPoint();
        }

        void _UpdateRedPoint()
        {
            goTarget.CustomActive(ItemDataManager.GetInstance().IsPackageHasNew(EPackageType.Title));
        }
    }
}