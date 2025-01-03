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
using System.Collections;
using DG.Tweening;

namespace GameClient
{
    // 月签到界面
    public class MonthlySignInFrameView : MonoBehaviour
    {
        #region inner define

        #endregion

        #region val
        List<ActivityDataManager.MonthlySignInItemData> itemDataList = null;

        bool bInitScrollBarPos = false;

        #endregion

        #region ui bind

        [SerializeField]
        ComUIListScript itemList = null;

        [SerializeField]
        AccumulativeSignInItem accumulativeSignIn1 = null;

        [SerializeField]
        AccumulativeSignInItem accumulativeSignIn2 = null;

        [SerializeField]
        AccumulativeSignInItem accumulativeSignIn3 = null;

        [SerializeField]
        AccumulativeSignInItem accumulativeSignIn4 = null;

        [SerializeField]
        Text labelTotalSign = null;

        [SerializeField]
        Text labelLivenessCount = null;

        [SerializeField]
        Text labelLiveness = null;

        [SerializeField]
        Slider mSliderProcess = null;

        [SerializeField]
        Image mImageTipBoard = null;

        [SerializeField]
        Text mTextTipBoard = null;

        [SerializeField]
        GameObject mGoSingIn;
        #endregion    

        void Start()
        {
            itemDataList = null;
            InitItems();

            UpdateMonthlySignInItems();
            UpdateMonthlySignInCountInfo();
            UpdateAccumulativeSignInItemInfo();

            BindUIEvent();

            ActivityDataManager.GetInstance().SendMonthlySignInQuery();

            float seconds = 0.0f;
            DateTime dateTime = Function.ConvertIntDateTime(TimeManager.GetInstance().GetServerDoubleTime());
            DateTime dateTimeNext = new DateTime();
            if (dateTime.Hour < 6) // 小于6点钟，则在今天的6点钟刷新一次界面
            {
                dateTimeNext = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 6, 0, 1);                
            }
            else // 大于等于6点钟则在明天的6点钟刷新一次界面
            {
                DateTime dt = dateTime.AddDays(1);
                dateTimeNext = new DateTime(dt.Year, dt.Month, dt.Day, 6, 0, 1);
            }

            TimeSpan timeSpan = dateTimeNext - dateTime;
            seconds = (float)timeSpan.TotalSeconds;

            InvokeMethod.Invoke(this, seconds, () => 
            {
                bInitScrollBarPos = false;
                ActivityDataManager.GetInstance().SendMonthlySignInQuery();
            });
        }

        void OnDestroy()
        {
            itemDataList = null;
            UnBindUIEvent();

            InvokeMethod.RemoveInvokeCall(this);

            bInitScrollBarPos = false;
        }

        #region method

        void BindUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnUpdateMonthlySignInCountInfo, _OnUpdateMonthlySignInCountInfo);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnUpdateMonthlySignInItemInfo, _OnUpdateMonthlySignInItemInfo);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnUpdateAccumulativeSignInItemInfo, _OnUpdateAccumulativeSignInItemInfo);
        }

        void UnBindUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnUpdateMonthlySignInCountInfo, _OnUpdateMonthlySignInCountInfo);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnUpdateMonthlySignInItemInfo, _OnUpdateMonthlySignInItemInfo);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnUpdateAccumulativeSignInItemInfo, _OnUpdateAccumulativeSignInItemInfo);
        }

        void InitItems()
        {
            if (itemList == null)
            {
                return;
            }

            itemList.Initialize();

            itemList.onBindItem = (GameObject go) =>
            {
                MonthlySignInItem item = null;
                if (go)
                {
                    item = go.GetComponent<MonthlySignInItem>();
                }

                return item;
            };


            itemList.onItemVisiable = (var1) =>
            {
                UpdateSignInItem(var1);

            };       
        }

        void UpdateSignInItem(ComUIListElementScript var1)
        {
            if (var1 == null)
            {
                return;
            }

            int iIndex = var1.m_index;
            if (iIndex >= 0 && itemDataList != null && iIndex < itemDataList.Count)
            {
                MonthlySignInItem item = var1.gameObjectBindScript as MonthlySignInItem;
                if (item != null)
                {
                    item.SetUp(itemDataList[iIndex]);
                }
            }
        }

        void CalItemDataList()
        {
            itemDataList = null;
            itemDataList = ActivityDataManager.GetInstance().GetMonthlySignInItemDatas();

            return;
        }

        void UpdateMonthlySignInItems()
        {
            if (itemList == null)
            {
                return;
            }

            CalItemDataList();

            if (itemDataList != null)
            {
                itemList.SetElementAmount(0);
                itemList.SetElementAmount(itemDataList.Count);
            }            
        }

        public void OnSignInClick()
        {
            ActivityDataManager.GetInstance().SendMonthlySignIn(0);
        }

        void UpdateMonthlySignInCountInfo()
        {
            ActivityDataManager.MonthlySignInCountInfo monthlySignInCountInfo = ActivityDataManager.GetInstance().GetMonthlySignInCountInfo();
            if (monthlySignInCountInfo == null)
            {
                return;
            }

            DateTime dateTime = Function.ConvertIntDateTime(TimeManager.GetInstance().GetServerDoubleTime());
            if(dateTime.Hour < 6)
            {
                dateTime = dateTime.AddDays(-1);
            }

            int dayNum = ActivityDataManager.GetMonthDayNum(dateTime.Year, dateTime.Month);

            string labelText = "本月签到： " + ActivityDataManager.GetInstance().GetHasSignInCount().ToString() + string.Format(TR.Value("daily_sign_in_max_day"), dayNum.ToString());
            labelTotalSign.SafeSetText(labelText);

            string labelText1 = "剩余普通补签 " + (monthlySignInCountInfo.noFree + monthlySignInCountInfo.free).ToString() + " 次        活跃度补签 " + monthlySignInCountInfo.activite.ToString() + " 次";
            labelLivenessCount.SafeSetText(labelText1);

            string labelText2 = "今日活跃度： " + MissionManager.GetInstance().Score.ToString();
            labelLiveness.SafeSetText(labelText2);

            //签到按钮
            bool isCanSignIn = ActivityDataManager.GetInstance().CanSignInToday();
            mGoSingIn.CustomActive(isCanSignIn);
            mTextTipBoard.SafeSetText(string.Format(TR.Value("daily_sign_in_actvie_count"), monthlySignInCountInfo.activite));
            mImageTipBoard.CustomActive(monthlySignInCountInfo.activite > 0);
        }

        void UpdateAccumulativeSignInItemInfo()
        {
            List<AccumulativeSignInItem> accumulativeSignInItems = new List<AccumulativeSignInItem>();
            if (accumulativeSignInItems == null)
            {
                return;
            }

            accumulativeSignInItems.Add(accumulativeSignIn1);
            accumulativeSignInItems.Add(accumulativeSignIn2);
            accumulativeSignInItems.Add(accumulativeSignIn3);
            accumulativeSignInItems.Add(accumulativeSignIn4);

            List<ActivityDataManager.AccumulativeSignInItemData> accumulativeSignInItemDatas = ActivityDataManager.GetInstance().GetAccumulativeSignInItemDatas();
            if (accumulativeSignInItemDatas == null)
            {
                return;
            }

            for (int i = 0; i < accumulativeSignInItems.Count && i < accumulativeSignInItemDatas.Count; i++)
            {
                accumulativeSignInItems[i].SetUp(accumulativeSignInItemDatas[i]);
            }

            // 这里开始计算进度条的值
            // 这些魔术值是通过编辑器来的。。。
            float[,] sections = new float[3,2] 
            {
                { 0.135f,0.292f },
                { 0.427f,0.575f },
                { 0.710f,0.875f },
            };

            if (mSliderProcess != null)
            {
                mSliderProcess.value = 0.0f;

                int index = -1;
                int signInCount = ActivityDataManager.GetInstance().GetHasSignInCount();
                for (int i = 0; i < accumulativeSignInItemDatas.Count; i++)
                {
                    ActivityDataManager.AccumulativeSignInItemData accumulativeSignInItemData = accumulativeSignInItemDatas[i];
                    if (accumulativeSignInItemData == null)
                    {
                        continue;
                    }

                    if (signInCount >= accumulativeSignInItemData.accumulativeDay)
                    {
                        index++;
                    }
                }

                if (index < 0)
                {
                    mSliderProcess.value = 0.0f;
                }
                else if (index >= sections.GetLength(0))
                {
                    mSliderProcess.value = sections[2, 1];
                }
                else
                {
                    float start = sections[index, 0];
                    float end = sections[index, 1];

                    mSliderProcess.value = start + (end - start) * ((float)(signInCount - accumulativeSignInItemDatas[index].accumulativeDay) / (float)(accumulativeSignInItemDatas[index + 1].accumulativeDay - accumulativeSignInItemDatas[index].accumulativeDay));
                }
            }
        }

        #endregion

        #region ui event
        void _OnUpdateMonthlySignInItemInfo(UIEvent uiEvent)
        {
            UpdateMonthlySignInItems();

            if(!bInitScrollBarPos)
            {
                bInitScrollBarPos = true;

                if(itemList != null)
                {
                    DateTime dateTime = Function.ConvertIntDateTime(TimeManager.GetInstance().GetServerDoubleTime());
                    if (dateTime.Hour < 6)
                    {
                        dateTime = dateTime.AddDays(-1);
                    }

                    if (dateTime.Day > 21)
                    {
                        itemList.m_scrollRect.verticalNormalizedPosition = 0.0f;
                    }
                    else
                    {
                        itemList.m_scrollRect.verticalNormalizedPosition = 1.0f;
                    }
                }               
            }

            bool isCanSignIn = ActivityDataManager.GetInstance().CanSignInToday();
            mGoSingIn.CustomActive(isCanSignIn);

            ActivityDataManager.MonthlySignInCountInfo monthlySignInCountInfo = ActivityDataManager.GetInstance().GetMonthlySignInCountInfo();
            if (monthlySignInCountInfo != null)
            {
                mImageTipBoard.CustomActive(monthlySignInCountInfo.activite > 0);
                mTextTipBoard.SafeSetText(string.Format(TR.Value("daily_sign_in_actvie_count"), monthlySignInCountInfo.activite));
            }
        }

        void _OnUpdateMonthlySignInCountInfo(UIEvent uiEvent)
        {
            UpdateMonthlySignInCountInfo();
        }

        void _OnUpdateAccumulativeSignInItemInfo(UIEvent uiEvent)
        {
            UpdateAccumulativeSignInItemInfo();
        }

        #endregion
    }
}
