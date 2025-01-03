using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GameClient
{
    class ComTAPRedControl : MonoBehaviour
    {
        public UnityEvent onSucceed;
        public UnityEvent onFailed;

        void Awake()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnNewPupilApplyRecieved,_OnNewPupilApplyRecieved);
            _Update();
        }

        void OnDestroy()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnNewPupilApplyRecieved,_OnNewPupilApplyRecieved);
        }

        void _OnNewPupilApplyRecieved(UIEvent uiEvent)
        {
            _Update();
        }

        void _Update()
        {
            UnityEvent action = RelationDataManager.GetInstance().HasNewApply ? onSucceed : onFailed;
            if (null != action)
            {
                action.Invoke();
            }
        }
    }
}