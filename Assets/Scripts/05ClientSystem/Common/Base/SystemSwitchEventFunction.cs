using System;
using System.Collections;
using System.Collections.Generic;
///////删除linq
using System.Text;
using Network;
using Protocol;
using UnityEngine;
using UnityEngine.UI;
using System.Reflection;


namespace GameClient
{
    public delegate void SystemUnityAction();
    public enum SystemEventType
    {
        SYSETM_EVENT_START = 1000,
        SYSETM_EVENT_SELECT_ROLE,
        SYSTEM_EVENT_ON_SWITCH_FAILED,
    }

    public class SystemSwitchEventFunction
    {
        public static void OnEventSelectRole()
        {
            ClientSystemManager.GetInstance().CloseFrame<UserAgreementFrame>();
            ClientSystemManager.GetInstance().CloseFrame<PublishContentFrame>();
            ClientSystemLoginUtility.StartLoginAfterVerify();
            ClientSystemLogin.mSwitchRole = false;
            SystemSwitchEventManager.GetInstance().RemoveEvent(SystemEventType.SYSTEM_EVENT_ON_SWITCH_FAILED);
        }

        public static void OnEventSwitchFailed()
        {
            SystemSwitchEventManager.GetInstance().RemoveEvent(SystemEventType.SYSETM_EVENT_SELECT_ROLE);
            ClientSystemLogin.mSwitchRole = false;
        }
    }
}
