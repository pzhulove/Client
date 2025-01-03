using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using UnityEngine.Assertions;
using Protocol;
using ProtoTable;

namespace GameClient
{
    class MonthCardBuyResultUserData
    {
        public string strResultInfo;
        public UnityEngine.Events.UnityAction okCallBack;
    }

    public class MonthCardBuyResult : ClientFrame
    {
        public static bool bBuyMonthCard = false;

        [UIControl("Text")]
        Text txtResultInfo;

        [UIControl("OK")]
        Button btnOK; 

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Vip/MonthCardBuyResult";
        }

        protected override void _OnOpenFrame()
        {
            bBuyMonthCard = false;

            if (btnOK != null)
            {
                btnOK.onClick.RemoveAllListeners();
                btnOK.onClick.AddListener(() =>
                {
                    frameMgr.CloseFrame(this);
                });
            }

            MonthCardBuyResultUserData data = userData as MonthCardBuyResultUserData;
            if(data != null)
            {
                if (txtResultInfo != null)
                {
                    txtResultInfo.text = data.strResultInfo;
                }

                if (btnOK != null)
                {                    
                    btnOK.onClick.AddListener(data.okCallBack);                    
                }
            }
        }

        protected override void _OnCloseFrame()
        {
            bBuyMonthCard = false;

            if (btnOK != null)
            {
                btnOK.onClick.RemoveAllListeners();
            }
        }
    }

}

