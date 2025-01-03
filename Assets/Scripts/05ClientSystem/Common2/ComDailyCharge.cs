using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class ComDailyCharge : MonoBehaviour
    {
        public int iTemplateID = 8500;
        public string formatSting = string.Empty;
        public string formatSting2 = string.Empty;
        public string linkString = string.Empty;
        public Text content;
        public StateController state;

        void Awake()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnDayChargeChanged, _OnDayChargeChanged);
        }

        void _OnDayChargeChanged(UIEvent uiEvent)
        {
            _UpdateValue();
        }

        void _UpdateValue()
        {
            if (null != content)
            {
                string value = string.Empty;
                int delta = 0;
                var activityData = getChargeItem((int)PlayerBaseData.GetInstance().dayChargeNum);
                if(null != activityData)
                {
                    delta = activityData.activeItem.Param1 - (int)PlayerBaseData.GetInstance().dayChargeNum;
                    value = string.Format(formatSting, PlayerBaseData.GetInstance().dayChargeNum, delta, activityData.activeItem.Param0);
                    if(null != state)
                    {
                        state.Key = "status_0";
                    }
                }
                else
                {
                    value = string.Format(formatSting2, PlayerBaseData.GetInstance().dayChargeNum, delta);
                    if (null != state)
                    {
                        state.Key = "status_1";
                    }
                }
                content.text = value;
            }
        }

        ActiveManager.ActivityData getChargeItem(int iValue)
        {
            var activeData = ActiveManager.GetInstance().GetActiveData(iTemplateID);
            if (null != activeData)
            {
                for(int i = 0; i < activeData.akChildItems.Count; ++i)
                {
                    var childItem = activeData.akChildItems[i];
                    if(null != childItem && iValue < childItem.activeItem.Param1)
                    {
                        return childItem;
                    }
                }
            }
            return null;
        }

        // Use this for initialization
        void Start()
        {
            _UpdateValue();
        }

        void OnDestroy()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnDayChargeChanged, _OnDayChargeChanged);
        }

        public void OnClickGo()
        {
            if(!string.IsNullOrEmpty(linkString))
            {
                ActiveManager.GetInstance().OnClickLinkInfo(linkString);
            }
        }
    }
}