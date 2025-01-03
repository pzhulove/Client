using System;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using ProtoTable;
using Protocol;
using Network;
using System;
using System.Collections.Generic;

namespace GameClient
{
    public class ComChijiShowPkReadyTime : MonoBehaviour
    {
        public GameObject root = null;
        public Text ShowTime = null;
        public int LastTime = 3;

        private bool bIsInReadyState = false;
        private float TimeIntrval = 0.0f;
        private uint StartTime = 0;

        private void Start()
        {
            _BindUIEvent();
        }

        private void OnDestroy()
        {
            _UnBindUIEvent();
            _Clear();
        }

        private void _Clear()
        {
            bIsInReadyState = false;
            TimeIntrval = 0.0f;
            StartTime = 0;
        }

        private void _BindUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ChijiPkReady, _OnChijiPkReady);
        }

        private void _UnBindUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ChijiPkReady, _OnChijiPkReady);
        }

        private void _OnChijiPkReady(UIEvent iEvent)
        {
            StartTime = TimeManager.GetInstance().GetServerTime();
            bIsInReadyState = true;
            root.CustomActive(true);
        }

        private void Update()
        {
            if(bIsInReadyState)
            {
                TimeIntrval += Time.deltaTime;

                if(TimeIntrval > 0.2f)
                {
                    TimeIntrval = 0.0f;

                    int PassedTime = (int)(TimeManager.GetInstance().GetServerTime() - StartTime);

                    if(LastTime >= PassedTime)
                    {
                        if (ShowTime != null)
                        {
                            ShowTime.text = (LastTime - PassedTime).ToString();
                        }
                    }
                    else
                    {
                        root.CustomActive(false);
                        StartTime = 0;
                        bIsInReadyState = false;

                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ChijiPkReadyFinish);
                    }
                }
            }    
        }
    }
}
