using System;
using System.Collections;
using System.Collections.Generic;
using GameClient;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class ActivityCommonInfo : MonoBehaviour
    {
        [SerializeField] private Text mTextTime;
        [SerializeField] private Text mTextRule;

        public void OnInit(ILimitTimeActivityModel data)
        {
            if (null != data)
            {
                mTextTime.SafeSetText(string.Format("{0}~{1}", _TransTimeStampToStr(data.StartTime), _TransTimeStampToStr(data.EndTime)));
                mTextRule.SafeSetText(data.RuleDesc.Replace('|', '\n'));
            }
        }

        private string _TransTimeStampToStr(UInt32 timeStamp)
        {
            System.DateTime time = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            DateTime dt = time.AddSeconds(timeStamp);// unix 总秒数
            
            return string.Format("{0}年{1}月{2}日{3:HH:mm}", dt.Year, dt.Month, dt.Day, dt);
            //return string.Format("{0}月{1}日{2:HH:mm}", dt.Month, dt.Day, dt);
        }
    }
}
