using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace GameClient
{
    class RoleRecoveryBinder : MonoBehaviour
    {
        public Text recoveryCount;
        // Use this for initialization
        void Start()
        {
            _CheckRedPoint();
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ServerTimeChanged,_CheckRedPoint);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.RoleRecoveryUpdate, _CheckRedPoint);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.RoleDeleteOk, _CheckRedPoint);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.RoleInfoUpdate, _CheckRedPoint);
        }

        void _CheckRedPoint(UIEvent uiEvent)
        {
            _CheckRedPoint();
        }

        void _CheckRedPoint()
        {
            int iCount = 0;
            var roles = ClientApplication.playerinfo.roleinfo;
            for(int i = 0; i < roles.Length; ++i)
            {
                if(RecoveryRoleCachedObject.OnFilter(roles[i]))
                {
                    ++iCount;
                }
            }
            gameObject.CustomActive(iCount > 0);
            if(recoveryCount != null)
            {
                recoveryCount.text = iCount.ToString();
            }
        }

        void OnDestroy()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.RoleRecoveryUpdate, _CheckRedPoint);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ServerTimeChanged, _CheckRedPoint);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.RoleDeleteOk, _CheckRedPoint);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.RoleInfoUpdate, _CheckRedPoint);
        }
    }
}