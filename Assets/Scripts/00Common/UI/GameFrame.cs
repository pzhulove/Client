using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using UnityEngine.Assertions;
using Protocol;
using ProtoTable;
using System.Collections;
using DG.Tweening;
using Network;

namespace GameClient
{
    // 增加了安全的delaycall和intervalCall处理的frame
    // 增加安全的ui event的注册和反注册处理
    public class GameFrame : ClientFrame
    {
        private List<object> delayCallObjs = new List<object>();
        private List<object> intervalCallObjs = new List<object>();

        class UIEventBindInfo
        {
            public EUIEventID eventID = EUIEventID.Invalid;
            public ClientEventSystem.UIEventHandler eventHandler = null;
        }
        private List<UIEventBindInfo> eventBindInfos = new List<UIEventBindInfo>();

        protected sealed override void _OnOpenFrame()
        {
            delayCallObjs = new List<object>();
            intervalCallObjs = new List<object>();
            eventBindInfos = new List<UIEventBindInfo>();

            OnBindUIEvent();

            OnOpenFrame();
        }

        protected sealed override void _OnCloseFrame()
        {
            ClearAllDelayCall();
            ClearAllIntervalCall();

            delayCallObjs = null;
            intervalCallObjs = null;

            OnUnBindUIEvent();

            UnBindAllUIEvent();
            eventBindInfos = null;

            OnCloseFrame();
        }

        protected virtual void OnOpenFrame()
        {

        }

        protected virtual void OnCloseFrame()
        {

        }

        protected virtual void OnBindUIEvent()
        {

        }

        protected virtual void OnUnBindUIEvent()
        {

        }

        protected object AddDelayCall(float delay, UnityEngine.Events.UnityAction action)
        {
            if (action == null)
            {
                return null;
            }

            if (delayCallObjs == null)
            {
                return null;
            }

            object obj = new object();
            if (obj == null)
            {
                return null;
            }
            delayCallObjs.Add(obj);
            InvokeMethod.TaskManager.GetInstance().InvokeCall(obj, Time.time, delay, action);

            return obj;
        }

        protected void DelDelayCall(object obj)
        {
            if (obj == null)
            {
                return;
            }

            if (delayCallObjs == null)
            {
                return;
            }

            InvokeMethod.TaskManager.GetInstance().RmoveInvokeCall(obj);
            delayCallObjs.Remove(obj);
            return;
        }

        protected void DelDelayCall(UnityEngine.Events.UnityAction action)
        {
            InvokeMethod.TaskManager.GetInstance().RmoveInvokeCall(action);
        }

        protected object AddIntervalCall(UnityEngine.Events.UnityAction action, float interval, float duration = float.MaxValue, float delay = 0.0f)
        {
            if (action == null)
            {
                return null;
            }

            if (intervalCallObjs == null)
            {
                return null;
            }

            object obj = new object();
            if (obj == null)
            {
                return null;
            }

            intervalCallObjs.Add(obj);
            InvokeMethod.TaskManager.GetInstance().InvokeIntervalCall(obj, Time.time, delay, interval, duration, null, action, null);
            return obj;
        }

        protected void DelIntervalCall(object obj)
        {
            if (obj == null)
            {
                return;
            }

            if (intervalCallObjs == null)
            {
                return;
            }

            InvokeMethod.TaskManager.GetInstance().RmoveInvokeIntervalCall(obj);
            intervalCallObjs.Remove(obj);

            return;
        }

        protected void DelIntervalCall(UnityEngine.Events.UnityAction action)
        {

        }

        private void ClearAllDelayCall()
        {
            if (delayCallObjs == null)
            {
                return;
            }

            for (int i = 0; i < delayCallObjs.Count; i++)
            {
                InvokeMethod.TaskManager.GetInstance().RmoveInvokeCall(delayCallObjs[i]);
            }
            delayCallObjs.Clear();
        }

        private void ClearAllIntervalCall()
        {
            if (intervalCallObjs == null)
            {
                return;
            }

            for (int i = 0; i < intervalCallObjs.Count; i++)
            {
                InvokeMethod.TaskManager.GetInstance().RmoveInvokeIntervalCall(intervalCallObjs[i]);
            }
            intervalCallObjs.Clear();
        }

        protected void BindUIEvent(EUIEventID id, ClientEventSystem.UIEventHandler handler)
        {
            if(eventBindInfos == null)
            {
                return;
            }

            if(handler == null)
            {
                return;
            }    

            eventBindInfos.Add(new UIEventBindInfo() { eventID = id,eventHandler = handler});
            UIEventSystem.GetInstance().RegisterEventHandler(id, handler);
        }

        protected void UnBindUIEvent(EUIEventID id, ClientEventSystem.UIEventHandler handler)
        {
            if(eventBindInfos == null)
            {
                return;
            }

            if(handler == null)
            {
                return;
            }

            eventBindInfos.RemoveAll((info) => { return (info != null && info.eventID == id && info.eventHandler == handler); });
            UIEventSystem.GetInstance().UnRegisterEventHandler(id, handler);
        }

        void UnBindAllUIEvent()
        {
            if(eventBindInfos == null)
            {
                return;
            }

            for(int i = 0;i < eventBindInfos.Count;i++)
            {
                UIEventBindInfo info = eventBindInfos[i];
                if(info == null)
                {
                    continue;
                }

                UIEventSystem.GetInstance().UnRegisterEventHandler(info.eventID, info.eventHandler);
            }
            eventBindInfos.Clear();
        }

        protected void UpdateControlState(UIState state)
        {
            LiteStateControl.RefreshControlsState(this, state);
        }
    }

    // 测试使用GameFrame
    public class TestGameFrame : GameFrame
    {
        protected override void OnOpenFrame()
        {
            AddDelayCall(5.0f, () => { });
            AddIntervalCall(() => { }, 1.0f);
        }

        protected override void OnCloseFrame()
        {
         
        }

        protected override void OnBindUIEvent()
        {
            BindUIEvent(EUIEventID.Invalid, null);
        }

        protected override void OnUnBindUIEvent()
        {
            UnBindUIEvent(EUIEventID.Invalid, null);
        }
    }
}
