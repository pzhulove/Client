using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

namespace GameClient
{
    public class ComEffectTransparentSolve : MonoBehaviour
    {
        static string ms_invalid_type = "WaitNetMessageFrame";
        public UnityEvent onEnable;
        public UnityEvent onDisable;
        [HideInInspector]
        public string[] types = new string[0];
        ClientFrameBinder comBinder = null;
        bool bInited = false;
        Type mFrameType = null;

        void Awake()
        {
            GlobalEventSystem.GetInstance().RegisterEventHandler(EUIEventID.FrameOpen, _OnFrameOpen);
            GlobalEventSystem.GetInstance().RegisterEventHandler(EUIEventID.FrameClose, _OnFrameClose);
        }

        void Start()
        {
            if(!bInited)
            {
                comBinder = GetComponentInParent<ClientFrameBinder>();
                if(null != comBinder)
                {
                    mFrameType = comBinder.GetFrameType();
                }
                else
                {
                    mFrameType = null;
                }
                bInited = true;
            }
            _Check();
        }

        void OnDestroy()
        {
            GlobalEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.FrameOpen, _OnFrameOpen);
            GlobalEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.FrameClose, _OnFrameClose);
        }

        int _FindIndex(Type type)
        {
            if(null == mFrameType)
            {
                return -1;
            }

            if(type.Name == ms_invalid_type)
            {
                return -1;
            }

            if (null != type)
            {
                for (int i = 0; i < types.Length; ++i)
                {
                    if(types[i] == type.Name && types[i] != mFrameType.Name)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        void _OnFrameOpen(UIEvent uiEvent)
        {
            var type = uiEvent.Param2 as Type;
            if (null != type)
            {
                int iFindIndex = _FindIndex(type);
                if (-1 != iFindIndex)
                {
                    _Check();
                }
            }
        }

        void _OnFrameClose(UIEvent uiEvent)
        {
            var type = uiEvent.Param2 as Type;
            if (null != type)
            {
                int iFindIndex = _FindIndex(type);
                if (-1 != iFindIndex)
                {
                    _Check();
                }
            }
        }

        void _Check()
        {
            bool bOpen = false;
            for(int i = 0; i < types.Length && !bOpen; ++i)
            {
                bOpen = (!string.IsNullOrEmpty(types[i])) && ClientSystemManager.GetInstance().HasActiveFrame(types[i]);
            }
            var action = (!bOpen) ? onEnable : onDisable;
            if (null != action)
            {
                action.Invoke();
            }
        }
    }
}
