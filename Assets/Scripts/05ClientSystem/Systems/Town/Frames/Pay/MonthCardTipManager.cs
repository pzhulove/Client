using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameClient
{
    public class MonthCardTipManager : Singleton<MonthCardTipManager>
    {
        string keyName = "-";

        public override void Init()
        {
        }

        public void SetPlayerKey(ulong roleID)
        {
            keyName = "HasShowMonthCardTip_" + roleID;
        }

        public void TryOpenMonthCardTipFrameByCond(ulong roleID)
        {
            SetPlayerKey(roleID);

            if (ConfigHasOpened())
                return;
            if (PlayerBaseData.GetInstance().Level == 1)
                return;
            var fatigueValue = (int)PlayerBaseData.GetInstance().fatigue;
            var monthCardLv = (int)PlayerBaseData.GetInstance().MonthCardLv;
            //xzl
            var nowTime = (int)TimeManager.GetInstance().GetServerTime();//(int)AdsPush.AdsPushServerDataManager.GetInstance().TransNowDateToStamp();
            var monthLastTime = monthCardLv - nowTime;
            if (monthLastTime <= 0 && fatigueValue == 0)
            {
                if (ClientSystemManager.GetInstance().IsFrameOpen<MonthCardTipFrame>() == false)
                {
                    ClientSystemManager.GetInstance().OpenFrame<MonthCardTipFrame>(FrameLayer.Middle);
                }
                SetConfig(keyName, 1);
            }
            else
            {
                SetConfig(keyName, 0);
            }
        }
        bool ConfigHasOpened()
        {
            if (PlayerPrefs.HasKey(keyName) == false)
            {
                SetConfig(keyName, 0);
                return false;
            }
            return PlayerPrefs.GetInt(keyName) >= 1 ? true : false;
        }

        void SetConfig(string keyName, int value)
        {
            PlayerPrefs.SetInt(keyName, value);
        }

        public void SetTrueConfig(ulong roleID)
        {
            SetPlayerKey(roleID);
            SetConfig(keyName, 1);
        }
    }
}