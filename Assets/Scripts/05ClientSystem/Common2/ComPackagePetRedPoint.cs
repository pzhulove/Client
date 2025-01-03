using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace GameClient
{
    public class ComPackagePetRedPoint : MonoBehaviour
    {
        public UnityEvent onTrue;
        public UnityEvent onFalse;

        private EUIEventID[] waitOnEventList = new EUIEventID[]
        {
            EUIEventID.ItemNotifyGet,
            EUIEventID.ItemNotifyRemoved,
            EUIEventID.ItemCountChanged,
            EUIEventID.ItemNewStateChanged,
            EUIEventID.ItemUseSuccess,

            EUIEventID.PetInfoInited,
            EUIEventID.PetSlotChanged,
            EUIEventID.PetFeedSuccess,
            EUIEventID.PetItemsInfoUpdate,

            EUIEventID.PetGoldFeedClick,
            EUIEventID.PetTabClick,
        };

        private void _onEventCom(UIEvent ui)
        {
            var current = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemGameBattle;
            if (current != null)
                return;

            bool changed = PetDataManager.GetInstance().IsNeedShowOnUsePetsRedPoint()
                        || PetDataManager.GetInstance().IsNeedShowPetEggRedPoint()
                        || PetDataManager.GetInstance().IsNeedShowPetRedPoint(); 

            if (changed)
            {
                if (null != onTrue) onTrue.Invoke();
            }
            else
            {
                if (null != onFalse) onFalse.Invoke();
            }
        }

        private void Awake()
        {
            _onEventCom(null);

            for (int i = 0; i < waitOnEventList.Length; ++i)
            {
                UIEventSystem.GetInstance().RegisterEventHandler(waitOnEventList[i], _onEventCom);
            }
        }

        private void OnDestroy()
        {
            for (int i = 0; i < waitOnEventList.Length; ++i)
            {
                UIEventSystem.GetInstance().UnRegisterEventHandler(waitOnEventList[i], _onEventCom);
            }
        }
    }
}
