using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using GameClient;
using UnityEngine.UI;
using UnityEngine.Serialization;
using System;

namespace GameClient
{
    sealed class MonthCardActive : ActiveSpecialFrame
    {
        public string ms_month_day_key = "rd";
        public override void OnCreate()
        {
            ActiveManager.GetInstance().onActivityUpdate += _OnActivityUpdate;
        }

        public override void OnDestroy()
        {
            ActiveManager.GetInstance().onActivityUpdate -= _OnActivityUpdate;
        }

        void _OnActivityUpdate(ActiveManager.ActivityData data, ActiveManager.ActivityUpdateType EActivityUpdateType)
        {
            if(EActivityUpdateType != ActiveManager.ActivityUpdateType.AUT_STATUS_CHANGED)
            {
                return;
            }

            if(data != null && 2500 == data.activeItem.ID && data.status == (int)Protocol.TaskStatus.TASK_OVER)
            {
                int iRdValue = ActiveManager.GetInstance().GetActiveItemValue(data.activeItem.ID, "rd");
                if (TimeManager.GetInstance().GetServerTime() + 3 > PlayerBaseData.GetInstance().MonthCardLv || iRdValue < 3)
                {
                    SystemNotifyManager.SystemNotify(3117, _OnClickOk);
                }
            }
        }

        void _OnClickOk()
        {
            VipTabType vipType = VipTabType.PAY;
            GameClient.ClientSystemManager.GetInstance().OpenFrame<VipFrame>(FrameLayer.Middle, vipType);
        }

        public override void OnUpdate()
        {
        }
    }
}