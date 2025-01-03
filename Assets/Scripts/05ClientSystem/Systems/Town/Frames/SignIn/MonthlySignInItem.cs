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
using DG.Tweening;


namespace GameClient
{
    using UIItemData = AwardItemData;

    public class MonthlySignInItem : MonoBehaviour
    {
        [SerializeField]
        ComItem comItem = null;

        [SerializeField]
        Image signInMark = null; // 已经签到的勾勾

        [SerializeField]
        Image signInMask = null; // 已经签到的遮罩

        [SerializeField]
        GameObject signInTodayEff = null; // 今天签到特效

        [SerializeField]
        Button btnReward = null; // 查看奖励

        [SerializeField]
        Button btnSignIn = null; // 签到

        [SerializeField]
        Button btnFillCheck = null; // 补签

        [SerializeField]
        GameObject signInEff = null; // 可签到时特效

        [SerializeField]
        DOTweenAnimation signAni = null; // 可签到时动画

        [SerializeField]
        GameObject vipAddUpTipRoot = null;

        [SerializeField]
        Text vipAddUpTip = null; // vip加成提示

        [SerializeField]
        CanvasGroup canvasMain = null; // 已签到时需要设置透明度的东西

        [SerializeField]
        float alphaMain = 0.6f; // 已签到透明度

        [SerializeField]
        float signAniStartScale = 0.3f;

        // Use this for initialization
        void Start()
        {
           
        }

        private void OnDestroy()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        void ShowItemTip(GameObject go, ItemData itemData)
        {
            if (null != itemData)
            {
                ItemTipManager.GetInstance().CloseAll();
                ItemTipManager.GetInstance().ShowTip(itemData);
            }
        }

        void SetComItemData(ComItem comItem, UIItemData uIItemData)
        {
            if(comItem == null || uIItemData == null)
            {
                return;
            }

            ItemData itemData = ItemDataManager.CreateItemDataFromTable(uIItemData.ID);
            if (itemData != null)
            {
                itemData.Count = uIItemData.Num;
                comItem.Setup(itemData, ShowItemTip);
            }

            return;
        }

        string GetColorName(UIItemData uIItemData)
        {
            if(uIItemData == null)
            {
                return "";
            }

            ItemData itemData = ItemDataManager.CreateItemDataFromTable(uIItemData.ID);
            if (itemData != null)
            {
                return itemData.GetColorName();
            }

            return "";
        }

        public void SetUp(object data)
        {
            if(data == null)
            {
                return;
            }

            ActivityDataManager.MonthlySignInItemData monthlySignInItemData = data as ActivityDataManager.MonthlySignInItemData;
            if(monthlySignInItemData == null)
            {
                return;
            }

            btnReward.SafeSetOnClickListener(() => 
            {
                if (monthlySignInItemData.awardItemData != null)
                {
                    ItemData itemData = ItemDataManager.CreateItemDataFromTable(monthlySignInItemData.awardItemData.ID);
                    if (itemData != null)
                    {
                        itemData.Count = monthlySignInItemData.awardItemData.Num;
                        ItemTipManager.GetInstance().CloseAll();
                        ItemTipManager.GetInstance().ShowTip(itemData);
                    }
                }
            });

            btnSignIn.SafeSetOnClickListener(() => 
            {
                ActivityDataManager.GetInstance().SendMonthlySignIn(0); // 传入0表示当天签到
            });

            btnFillCheck.SafeSetOnClickListener(() => 
            {
                //ActivityDataManager.GetInstance().SendMonthlySignIn(monthlySignInItemData.day);

                if(ActivityDataManager.GetInstance().GetFillCheckCount() == 0)
                {
                    SystemNotifyManager.SystemNotify(10044);
                    return;
                }

                ClientSystemManager.GetInstance().OpenFrame<SignFrame>(FrameLayer.Middle,monthlySignInItemData);
            });

            DateTime dateTime = Function.ConvertIntDateTime(TimeManager.GetInstance().GetServerDoubleTime());
            // 签到次数是每天的6点刷新，也就是说每天的6点之前算前一天
            if (dateTime.Hour < 6)
            {
                dateTime = dateTime.AddDays(-1);            
            }

            int nowDay = dateTime.Day;
            signInTodayEff.CustomActive(monthlySignInItemData.day == nowDay && !monthlySignInItemData.signIned);
            signInMark.CustomActive(monthlySignInItemData.signIned);
            signInMask.CustomActive(monthlySignInItemData.signIned || monthlySignInItemData.day < nowDay);

            if(comItem != null && monthlySignInItemData.awardItemData != null)
            {
                ItemData itemData = ItemDataManager.CreateItemDataFromTable(monthlySignInItemData.awardItemData.ID);
                if (itemData != null)
                {
                    itemData.Count = monthlySignInItemData.awardItemData.Num;
                    comItem.Setup(itemData, null);
                }
            }

            vipAddUpTip.SafeSetText(ActivityDataManager.GetInstance().GetVipAddUpText(dateTime.Month,monthlySignInItemData.day));
            vipAddUpTipRoot.CustomActive(ActivityDataManager.GetInstance().GetVipAddUpText(dateTime.Month, monthlySignInItemData.day) != "");
            
            if(monthlySignInItemData.day == nowDay)
            {
                btnSignIn.CustomActive(!monthlySignInItemData.signIned);
                signInEff.CustomActive(!monthlySignInItemData.signIned);
                btnFillCheck.CustomActive(false);

                if (signAni != null)
                {
                    if (!monthlySignInItemData.signIned)
                    {
                        signAni.transform.localScale = Vector3.one * signAniStartScale;
                        signAni.DORestart();
                    }
                    else
                    {
                        signAni.DOPause();
                        signAni.transform.localEulerAngles = Vector3.zero;
                        signAni.transform.localScale = Vector3.one;
                    }
                }

                if (canvasMain != null)
                {
                    canvasMain.alpha = monthlySignInItemData.signIned ? alphaMain : 1; ;
                }

                btnReward.CustomActive(monthlySignInItemData.signIned);
            }
            else if(monthlySignInItemData.day < nowDay)
            {
                if(monthlySignInItemData.signIned)
                {
                    btnSignIn.CustomActive(false);
                    signInEff.CustomActive(false);
                    btnFillCheck.CustomActive(false);
                    if (canvasMain != null)
                    {
                        canvasMain.alpha = alphaMain;
                    }
                    btnReward.CustomActive(true);
                }
                else
                {
                    btnSignIn.CustomActive(false);
                    signInEff.CustomActive(false);
                    btnFillCheck.CustomActive(true);
                    if (canvasMain != null)
                    {
                        canvasMain.alpha = alphaMain;
                    }
                    btnReward.CustomActive(true);
                }

                if (signAni != null)
                {
                    signAni.DOPause();
                    signAni.transform.localEulerAngles = Vector3.zero;
                    signAni.transform.localScale = Vector3.one;
                }
            }
            else
            {
                btnSignIn.CustomActive(false);
                signInEff.CustomActive(false);
                btnFillCheck.CustomActive(false);
                if (canvasMain != null)
                {
                    canvasMain.alpha = 1;
                }
                btnReward.CustomActive(true);

                if (signAni != null)
                {
                    signAni.DOPause();
                    signAni.transform.localEulerAngles = Vector3.zero;
                    signAni.transform.localScale = Vector3.one;
                }
            }

            return;
        }
    }
}


