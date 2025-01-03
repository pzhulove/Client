using UnityEngine.UI;
using System.Collections;
using UnityEngine;
using System;

namespace GameClient
{
    public class SignFrame : ClientFrame
    {
        string[] hints = new string[]
            {
                "还可以补签: <color=#00fe0cff>{0}</color>次",
                "还可以补签: <color=#FF0000FF>{0}</color>次",
                "还可以补签: <color=#00ff00FF>{0}</color>次",
                "还可以补签: <color=#00ff00FF>{0}</color>次",
                "免费补签: <color=#00fe0cff>{0}</color>次",
                "免费补签: <color=#00ff00FF>{0}</color>次",
                "免费补签: <color=#FF0000FF>{0}</color>次",
                "免费补签: <color=#00ff00FF>{0}</color>次",
            };

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Activity/SignFrame";
        }

        Text count0;
        Text count1;
        Text count2;

        int m_iSIRp;
        int m_iRpFree;
        int m_iOnceCost;
        Text m_kOnce;
        Text m_kAll;
        UIGray m_kGrayOnce;
        UIGray m_kGrayAll;
        Button m_kBtnOnce;
        Button m_kBtnAll;
        Image moneyIcon;
        Image moneyIcon1;
        int m_iCostDefault = 20;

        ComItem Item0 = null;
        CanvasGroup mCanvasActive = null;
        CanvasGroup mCanvasVipDouble = null;
        Text mTextVipDouble = null;

        ActivityDataManager.MonthlySignInItemData monthlySignInItemData = null;
        protected override void _OnOpenFrame()
        {
            monthlySignInItemData = null;
            if(userData != null && userData is ActivityDataManager.MonthlySignInItemData)
            {
                monthlySignInItemData = (ActivityDataManager.MonthlySignInItemData)userData;
            }
  
            CalcSignInCountInfo();
           
            m_iOnceCost = _GetSignOnceCost();
            int iIndex = 0;

            //m_kHint.text = string.Format(hints[iIndex], m_iSIRp);
            //m_kHint2.text = string.Format(hints[iIndex + 4], m_iRpFree);

            int iOnceCost = m_iRpFree > 0 ? 0 : m_iOnceCost;

            ActivityDataManager.MonthlySignInCountInfo monthlySignInCountInfo = ActivityDataManager.GetInstance().GetMonthlySignInCountInfo();
            if (monthlySignInCountInfo != null)
            {                
                count0.SafeSetText(TR.Value("count_info", monthlySignInCountInfo.noFree + monthlySignInCountInfo.free));
                count1.SafeSetText(TR.Value("count_info", monthlySignInCountInfo.free));
                count2.SafeSetText(TR.Value("count_info", monthlySignInCountInfo.activite));

                m_kOnce = Utility.FindComponent<Text>(frame, "OnceHint/Text");
                if (monthlySignInCountInfo.activite > 0)
                {
                    mCanvasActive.CustomActive(true);
                    moneyIcon.CustomActiveAlpha(false);
                    m_kOnce.CustomActive(false);
                }
                else
                {
                    mCanvasActive.CustomActive(false);
                    moneyIcon.CustomActiveAlpha(true);
                    m_kOnce.CustomActive(true);
                    m_kOnce.text = string.Format(TR.Value("sign_once_cost"), iOnceCost);
                }
            }

            m_kAll = Utility.FindComponent<Text>(frame, "AllHint/Text");
            int iAllCost = (m_iSIRp - m_iRpFree) * m_iOnceCost;
            iAllCost = Mathf.Max(0, iAllCost);
            m_kAll.text = string.Format(TR.Value("sign_once_cost"), iAllCost);

            m_kGrayOnce = Utility.FindComponent<UIGray>(frame, "BtnOnce");
            m_kGrayAll = Utility.FindComponent<UIGray>(frame, "BtnAll");
            m_kBtnOnce = Utility.FindComponent<Button>(frame, "BtnOnce");
            m_kBtnAll = Utility.FindComponent<Button>(frame, "BtnAll");

            int iMoneyID = _GetSignMoneyID();
            int iTicket = ItemDataManager.GetInstance().GetOwnedItemCount(iMoneyID);
            var item = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(iMoneyID);
            if(null != item)
            {
                // moneyIcon.sprite = moneyIcon1.sprite = AssetLoader.instance.LoadRes(item.Icon, typeof(Sprite)).obj as Sprite;
                ETCImageLoader.LoadSprite(ref moneyIcon, item.Icon);
            }

            m_kGrayOnce.enabled = iOnceCost > iTicket;
            m_kBtnOnce.enabled = !m_kGrayOnce.enabled;
            m_kOnce.color = m_kBtnOnce.enabled ? Color.green : Color.red;

            m_kGrayAll.enabled = iAllCost > iTicket;
            m_kBtnAll.enabled = !m_kGrayAll.enabled;
            m_kAll.color = m_kBtnAll.enabled ? Color.green : Color.red;

            if(monthlySignInItemData != null)
            {
                if (Item0 != null && monthlySignInItemData.awardItemData != null)
                {
                    ItemData itemData = ItemDataManager.CreateItemDataFromTable(monthlySignInItemData.awardItemData.ID);
                    if (itemData != null)
                    {
                        itemData.Count = monthlySignInItemData.awardItemData.Num;
                        Item0.Setup(itemData, (var1,var2) => 
                        {
                            ItemTipManager.GetInstance().CloseAll();
                            ItemTipManager.GetInstance().ShowTip(itemData);
                        });
                    }
                }
            }

            DateTime dateTime = Function.ConvertIntDateTime(TimeManager.GetInstance().GetServerDoubleTime());
            // 签到次数是每天的6点刷新，也就是说每天的6点之前算前一天
            if (dateTime.Hour < 6)
            {
                dateTime = dateTime.AddDays(-1);
            }
            mTextVipDouble.SafeSetText(ActivityDataManager.GetInstance().GetVipAddUpText(dateTime.Month, monthlySignInItemData.day));
            mCanvasVipDouble.CustomActive(ActivityDataManager.GetInstance().GetVipAddUpText(dateTime.Month, monthlySignInItemData.day) != "");
        }

        protected override void _bindExUI()
        {
            Item0 = mBind.GetCom<ComItem>("Item0");
            mCanvasActive = mBind.GetCom<CanvasGroup>("ActiveSign");
            mTextVipDouble = mBind.GetCom<Text>("VipText");
            mCanvasVipDouble = mBind.GetCom<CanvasGroup>("vipDouble");
            count0 = mBind.GetCom<Text>("count0");
            count1 = mBind.GetCom<Text>("count1");
            count2 = mBind.GetCom<Text>("count2");
            moneyIcon = mBind.GetCom<Image>("moneyIcon");
            moneyIcon1 = mBind.GetCom<Image>("moneyIcon1");
        }

        protected override void _unbindExUI()
        {
            Item0 = null;
            mCanvasActive = null;
            mTextVipDouble = null;
            mCanvasVipDouble = null;
            count0 = null;
            count1 = null;
            count2 = null;
            moneyIcon = null;
            moneyIcon1 = null;
        }

        void CalcSignInCountInfo()
        {
            ActivityDataManager.MonthlySignInCountInfo monthlySignInCountInfo = ActivityDataManager.GetInstance().GetMonthlySignInCountInfo();
            if (monthlySignInCountInfo != null)
            {
                m_iSIRp = monthlySignInCountInfo.noFree + monthlySignInCountInfo.free + monthlySignInCountInfo.activite;
                m_iRpFree = monthlySignInCountInfo.free + monthlySignInCountInfo.activite;
                if (m_iRpFree < 0)
                {
                    m_iRpFree = 0;
                }
            }
        }

        int _GetSignMoneyID()
        {
            int iMoneyID = ItemDataManager.GetInstance().GetMoneyIDByType(ProtoTable.ItemTable.eSubType.BindPOINT);
            var systemValue = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_RP_SIGHIN_COST_ITEMID);
            if(null != systemValue)
            {
                iMoneyID = systemValue.Value;
            }
            return iMoneyID;
        }

        int _GetSignOnceCost()
        {
            int iCost = m_iCostDefault;
            var functionData = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_RP_SIGHIN_COST_NUM);
            if (null != functionData)
            {
                iCost = functionData.Value;
            }
            return iCost;
        }

        protected override void _OnCloseFrame()
        {
            monthlySignInItemData = null;
        }
        
//         [UIEventHandle("bg/close")]
//         void OnClickClose()
//         {
//             frameMgr.CloseFrame(this);
//         }

        [UIEventHandle("BtnOnce")]
        void OnSignOnce()
        {
            if (ActivityDataManager.GetInstance().GetFillCheckCount() == 0)
            {
                SystemNotifyManager.SystemNotify(10044);
                return;
            }

            CalcSignInCountInfo();

            m_iOnceCost = _GetSignOnceCost();
            int iOnceCost = m_iRpFree > 0 ? 0 : m_iOnceCost;
            int iCostId = _GetSignMoneyID();

            if(iOnceCost <= 0)
            {
                //ActiveManager.GetInstance().SendSceneActiveTaskSubmitRp(3000, true);
                if(monthlySignInItemData != null)
                {
                    ActivityDataManager.GetInstance().SendMonthlySignIn(monthlySignInItemData.day);
                }
                
                frameMgr.CloseFrame(this);
            }
            else
            {
                CostItemManager.GetInstance().TryCostMoneyDefault(new CostItemManager.CostInfo {  nMoneyID = iCostId, nCount = iOnceCost },
                    ()=>
                    {
                        //ActiveManager.GetInstance().SendSceneActiveTaskSubmitRp(3000, true);
                        if (monthlySignInItemData != null)
                        {
                            ActivityDataManager.GetInstance().SendMonthlySignIn(monthlySignInItemData.day);
                        }
                        frameMgr.CloseFrame(this);
                    });
            }
        }

//         [UIEventHandle("BtnAll")]
//         void OnSignAll()
//         {
//             if (ActivityDataManager.GetInstance().GetFillCheckCount() == 0)
//             {
//                 SystemNotifyManager.SystemNotify(10044);
//                 return;
//             }
// 
//             CalcSignInCountInfo();
// 
//             m_iOnceCost = _GetSignOnceCost();
// 
//             int iAllCost = (m_iSIRp - m_iRpFree) * m_iOnceCost;
//             iAllCost = Mathf.Max(0, iAllCost);
// 
//             int iCostId = _GetSignMoneyID();
// 
//             if (iAllCost <= 0)
//             {
//                 //ActiveManager.GetInstance().SendSceneActiveTaskSubmitRp(3000, false);
//                 ActivityDataManager.GetInstance().SendMonthlySignIn(curDay,true);
//                 frameMgr.CloseFrame(this);
//             }
//             else
//             {
//                 CostItemManager.GetInstance().TryCostMoneyDefault(new CostItemManager.CostInfo { nMoneyID = iCostId, nCount = iAllCost },
//                     () =>
//                     {
//                         //ActiveManager.GetInstance().SendSceneActiveTaskSubmitRp(3000, false);
//                         ActivityDataManager.GetInstance().SendMonthlySignIn(curDay,true);
//                         frameMgr.CloseFrame(this);
//                     });
//             }
//         }
    }
}