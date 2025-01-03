using System;
using System.Collections;
using System.Collections.Generic;
///////删除linq
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using ProtoTable;
using Protocol;

namespace GameClient
{
    public class ActiveChargeTabObject : CachedObject
    {
        protected GameObject goLocal;
        protected GameObject goParent;
        protected GameObject goPrefab;
        protected ActiveChargeFrame THIS;
        protected ActiveManager.ActiveData activeData;
        protected ActiveStatus2GameObject comActiveStatusControl;

        Text m_kLabel;
        Text m_kMarkLabel;
        public Toggle m_kToggle;

        static ActiveChargeTabObject ms_selected;
        public static ActiveChargeTabObject Selected
        {
            get { return ms_selected; }
        }

        public static void Clear()
        {
            ms_selected = null;
        }

        #region TabInit
        public static Regex s_regex_tabinit = new Regex(@"<path=(.+) type=(\w+) value=(.+)>", RegexOptions.Singleline);
        List<RedPointObject> m_akGoRedPoints = null;
        void _Initialize()
        {
            if(goLocal != null)
            {
                comActiveStatusControl = goLocal.GetComponent<ActiveStatus2GameObject>();
                if (comActiveStatusControl != null && activeData != null && activeData.mainItem != null)
                {
                    comActiveStatusControl.ActiveConfigID = activeData.mainItem.ID;
                }

                if (activeData != null && activeData.mainItem != null)
                {
                    m_akGoRedPoints = RedPointObject.Create(activeData.mainItem.RedPointPath, goLocal);
                }

                if (m_akGoRedPoints != null)
                {
                    for (int i = 0; i < m_akGoRedPoints.Count; ++i)
                    {
                        if (m_akGoRedPoints[i].Current != null)
                        {
                            m_akGoRedPoints[i].Current.CustomActive(false);
                        }
                    }

                    if (activeData != null && activeData.mainItem != null)
                    {
                        for (int i = 0; i < m_akGoRedPoints.Count; ++i)
                        {
                            if (m_akGoRedPoints[i].redBinder != null)
                            {
                                m_akGoRedPoints[i].redBinder.iMainId = activeData.mainItem.ID;
                            }
                        }
                    }
                }

                if (activeData != null && activeData.mainItem != null && !string.IsNullOrEmpty(activeData.mainItem.TabInitDesc))
                {
                    var initItems = activeData.mainItem.TabInitDesc.Split(new char[] { '\r', '\n' });
                    for (int i = 0; i < initItems.Length; ++i)
                    {
                        if (!string.IsNullOrEmpty(initItems[i]))
                        {
                            var match = s_regex_tabinit.Match(initItems[i]);
                            if (!string.IsNullOrEmpty(match.Groups[0].Value))
                            {
                                switch (match.Groups[2].Value)
                                {
                                    case "Text":
                                        {
                                            Text text = Utility.FindComponent<Text>(goLocal, match.Groups[1].Value);
                                            if (text != null)
                                            {
                                                text.text = match.Groups[3].Value;
                                            }
                                        }
                                        break;
                                    case "Image":
                                        {
                                            Image image = Utility.FindComponent<Image>(goLocal, match.Groups[1].Value);
                                            if (image != null)
                                            {
                                                // image.sprite = AssetLoader.instance.LoadRes(match.Groups[3].Value, typeof(Sprite)).obj as Sprite;
                                                ETCImageLoader.LoadSprite(ref image, match.Groups[3].Value);
                                                image.SetNativeSize();
                                            }
                                        }
                                        break;
                                }
                            }
                        }
                    }
                }
            }

            InitSpecialTab();
        }


        #endregion

        public override void OnCreate(object[] param)
        {
            if(param.Length >= 1)
            {
                goParent = param[0] as GameObject;
            }

            if(param.Length >= 2)
            {
                goPrefab = param[1] as GameObject;
            }

            if(param.Length >= 3)
            {
                activeData = param[2] as ActiveManager.ActiveData;
            }
            
            if(param.Length >= 4)
            {
                THIS = param[3] as ActiveChargeFrame;
            }

            if (goLocal == null)
            {
                if(goPrefab != null)
                {
                    goLocal = GameObject.Instantiate(goPrefab);
                }

                if(goLocal != null && goParent != null)
                {
                    Utility.AttachTo(goLocal, goParent);
                }

                if(goLocal != null)
                {
                    m_kLabel = Utility.FindComponent<Text>(goLocal, "Image/Label", false);
                    m_kMarkLabel = Utility.FindComponent<Text>(goLocal, "Checkmark/Label", false);

                    m_kToggle = goLocal.GetComponent<Toggle>();
                    if (m_kToggle != null)
                    {
                        m_kToggle.onValueChanged.RemoveAllListeners();
                        m_kToggle.onValueChanged.AddListener((bool bValue) =>
                        {
                            if (m_kLabel != null)
                                m_kLabel.transform.parent.gameObject.CustomActive(!bValue);
                            if (m_kMarkLabel != null)
                                m_kMarkLabel.transform.parent.gameObject.CustomActive(bValue);
                            if (bValue)
                            {
                                OnSelected();
                            }
                        });
                    }
                }

                //TO ADD CODE
                _Initialize();
            }

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ActivitySpecialRedPointNotify, _OnSignalRedPoint);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ActivitySpecialRedPointNotify, _OnSignalRedPoint);

            Enable();
            _UpdateItem();
        }

        void OnSelected()
        {
            ms_selected = this;
            THIS.SetTarget(activeData);
            
            if (activeData.iActiveID == ActiveManager.activityId[0])
            {
                ActiveManager.GetInstance().WelfareTABEnergyRedPointFlag = true;
            }

            if (activeData.iActiveID == ActiveManager.activityId[1])
            {
                ActiveManager.GetInstance().WelfareTABRewardRedPointFlag = true;
            }
            _UpdateItem();
        }

        void _OnSignalRedPoint(UIEvent uiEvent)
        {
            UIEventSpecialRedPointNotify uiParam = uiEvent as UIEventSpecialRedPointNotify;
            if (uiParam != null && uiParam.iMainId == activeData.iActiveID)
            {
                OnSignalRedPoint(activeData, uiParam.prefabKey);
            }
        }

        void OnSignalRedPoint(ActiveManager.ActiveData activeData, string prefabKey)
        {
            for (int i = 0; i < activeData.akChildItems.Count; ++i)
            {
                var curChildItem = activeData.akChildItems[i].activeItem;
                if (curChildItem.DoesWorkToRedPoint == 1 &&
                    curChildItem.RedPointWorkMode == 1 &&
                    prefabKey == curChildItem.PrefabKey)
                {
                    ActiveManager.GetInstance().SignalRedPoint(activeData.akChildItems[i]);
                }
            }
        }

        public override void OnDestroy()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ActivitySpecialRedPointNotify, _OnSignalRedPoint);
            DestroySpecialTab();
        }

        public override void OnRecycle()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ActivitySpecialRedPointNotify, _OnSignalRedPoint);
            Disable();
        }
        public override void Enable()
        {
            if (goLocal != null)
            {
                goLocal.CustomActive(true);
            }
        }

        public override void Disable()
        {
            if (goLocal != null)
            {
                goLocal.CustomActive(false);
            }
        }
        public override void OnDecycle(object[] param) { OnCreate(param); }
        public override void OnRefresh(object[] param) { OnCreate(param); }
        public override bool NeedFilter(object[] param) { return false; }

        void _UpdateItem()
        {
            if (!string.IsNullOrEmpty(activeData.mainItem.Name))
            {
                if (m_kLabel != null)
                {
                    m_kLabel.text = activeData.mainItem.Name;
                }
                if (m_kMarkLabel != null)
                {
                    m_kMarkLabel.text = activeData.mainItem.Name;
                }
            }

            if (m_akGoRedPoints != null)
            {
                for (int i = 0; i < m_akGoRedPoints.Count; ++i)
                {
                  
                    if (activeData.iActiveID == ActiveManager.activityId[0] && ActiveManager.GetInstance().WelfareTABEnergyRedPointFlag|| 
                        activeData.iActiveID == ActiveManager.activityId[1] && ActiveManager.GetInstance().WelfareTABRewardRedPointFlag)
                    {
                        m_akGoRedPoints[i].Current.CustomActive(false);
                    }
                    else
                    {
                        bool bShowRedPoint = ActiveManager.GetInstance().CheckHasFinishedChildItem(activeData, m_akGoRedPoints[i].Keys);

                        if (activeData.iActiveID == ActiveManager.activityId[0] &&bShowRedPoint ==false)
                        {
                            ActiveManager.GetInstance().WelfareTABEnergyRedPointFlag = true;
                        }

                        if (activeData.iActiveID == ActiveManager.activityId[1] && bShowRedPoint ==false)
                        {
                            ActiveManager.GetInstance().WelfareTABRewardRedPointFlag = true;
                        }
                        m_akGoRedPoints[i].Current.CustomActive(bShowRedPoint);
                    }
                   
                }
            }

            if (comActiveStatusControl != null)
            {
                comActiveStatusControl.ActiveConfigID = activeData.mainItem.ID;
            }

            DealWithSpecialTabRedPoint();
        }
        
        private void DestroySpecialTab()
        {
            if (activeData != null)
            {
                if (activeData.iActiveID == FinancialPlanDataManager.GetInstance().ActivityTemplateId)
                {
                    UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.FinancialPlanRedPointTips, DealWithSpecialTabRedPoint);
                }
                if (activeData.iActiveID == DailyChargeRaffleDataManager.ACTIVITY_TEMPLATE_ID)
                {
                    UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.DailyChargeRedPointChanged, DealWithSpecialTabRedPoint);
                }

                //新人周签到和活动周签到的红点提示
                if (activeData.iActiveID == WeekSignInDataManager.NewPlayerWeekSignInOpActTypeId)
                {
                    UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnNewPlayerWeekSignInRedPointChanged,
                        DealWithSpecialTabRedPoint);
                }

                if (activeData.iActiveID == WeekSignInDataManager.ActivityWeekSignInOpActTypeId)
                {
                    UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnActivityWeekSignInRedPointChanged,
                        DealWithSpecialTabRedPoint);
                }

                //月卡翻牌奖励
                if (activeData.iActiveID == PayManager.MONTH_CARD_TEMPLATE_ID)
                {
                    UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnMonthCardRewardRedPointReset, DealWithSpecialTabRedPoint);
                }

                // 新版月签到
                if (activeData.iActiveID == ActiveChargeFrame.monthlySignInActiveID)
                {
                    UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnMonthlySignInRedPointReset, DealWithSpecialTabRedPoint);
                }

                //勇士招募
                if (activeData.iActiveID == WarriorRecruitDataManager.GetInstance().warriorRecruitActiveID)
                {
                    UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.WarriorRecruitReceiveRewardSuccessed, DealWithSpecialTabRedPoint);
                    UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.WarriorRecruitQueryTaskSuccessed, DealWithSpecialTabRedPoint);
                    UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.CounterChanged, DealWithSpecialTabRedPoint);
                }
            }
        }

        private void InitSpecialTab()
        {
            if (activeData != null)
            {
                if (activeData.iActiveID == FinancialPlanDataManager.GetInstance().ActivityTemplateId)
                {
                    UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.FinancialPlanRedPointTips, DealWithSpecialTabRedPoint);
                }
                if (activeData.iActiveID == DailyChargeRaffleDataManager.ACTIVITY_TEMPLATE_ID)
                {
                    UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.DailyChargeRedPointChanged, DealWithSpecialTabRedPoint);
                }

                //新人周签到和活动周签到的红点提示
                if (activeData.iActiveID == WeekSignInDataManager.NewPlayerWeekSignInOpActTypeId)
                {
                    UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnNewPlayerWeekSignInRedPointChanged,
                        DealWithSpecialTabRedPoint);
                }

                if (activeData.iActiveID == WeekSignInDataManager.ActivityWeekSignInOpActTypeId)
                {
                    UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnActivityWeekSignInRedPointChanged,
                        DealWithSpecialTabRedPoint);
                }

                //月卡暂存箱
                if (activeData.iActiveID == PayManager.MONTH_CARD_TEMPLATE_ID)
                {
                    UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnMonthCardRewardRedPointReset, DealWithSpecialTabRedPoint);
                }

                // 新版月签到
                if (activeData.iActiveID == ActiveChargeFrame.monthlySignInActiveID)
                {
                    UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnMonthlySignInRedPointReset, DealWithSpecialTabRedPoint);
                }

                //勇士招募
                if (activeData.iActiveID == WarriorRecruitDataManager.GetInstance().warriorRecruitActiveID)
                {
                    UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.WarriorRecruitReceiveRewardSuccessed, DealWithSpecialTabRedPoint);
                    UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.WarriorRecruitQueryTaskSuccessed, DealWithSpecialTabRedPoint);
                    UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.CounterChanged, DealWithSpecialTabRedPoint);
                }
            }
        }

        private void DealWithSpecialTabRedPoint(UIEvent data = null)
        {
            OnDealWithSpecialTab();
        }

        private void OnDealWithSpecialTab()
        {
            if (activeData != null)
            {
                if (activeData.iActiveID == FinancialPlanDataManager.GetInstance().ActivityTemplateId)
                {
                    if (m_akGoRedPoints != null)
                    {
                        for (int i = 0; i < m_akGoRedPoints.Count; i++)
                        {
                            bool isShowRedPoint = FinancialPlanDataManager.GetInstance().IsShowRedPoint();
                            m_akGoRedPoints[i].Current.CustomActive(isShowRedPoint);
                        }
                    }
                }
                else if (activeData.iActiveID == DailyChargeRaffleDataManager.ACTIVITY_TEMPLATE_ID)
                {
                    if (m_akGoRedPoints != null)
                    {
                        for (int i = 0; i < m_akGoRedPoints.Count; i++)
                        {
                            bool isShowRedPoint = DailyChargeRaffleDataManager.GetInstance().IsRedPointShow();
                            m_akGoRedPoints[i].Current.CustomActive(isShowRedPoint);
                        }
                    }
                }
                else if (activeData.iActiveID == WeekSignInDataManager.ActivityWeekSignInOpActTypeId)
                {
                    if (m_akGoRedPoints != null)
                    {
                        for (var i = 0; i < m_akGoRedPoints.Count; i++)
                        {
                            var isShowRedPoint =
                                WeekSignInUtility.IsWeekSignInRedPointVisibleByWeekSignInType(WeekSignInType
                                    .ActivityWeekSignIn);
                            m_akGoRedPoints[i].Current.CustomActive(isShowRedPoint);
                        }
                    }
                }
                else if (activeData.iActiveID == WeekSignInDataManager.NewPlayerWeekSignInOpActTypeId)
                {
                    if (m_akGoRedPoints != null)
                    {
                        for (var i = 0; i < m_akGoRedPoints.Count; i++)
                        {
                            var isShowRedPoint = WeekSignInUtility.IsWeekSignInRedPointVisibleByWeekSignInType(
                                WeekSignInType.NewPlayerWeekSignIn);
                            m_akGoRedPoints[i].Current.CustomActive(isShowRedPoint);
                        }
                    }
                }
                else if (activeData.iActiveID == PayManager.MONTH_CARD_TEMPLATE_ID)
                {
                    if (m_akGoRedPoints != null)
                    {
                        for (var i = 0; i < m_akGoRedPoints.Count; i++)
                        {
                            bool bShowRedPoint = ActiveManager.GetInstance().CheckHasFinishedChildItem(activeData, m_akGoRedPoints[i].Keys);
                            var isShowRedPoint = MonthCardRewardLockersDataManager.GetInstance().IsRedPointShow() || bShowRedPoint;
                            m_akGoRedPoints[i].Current.CustomActive(isShowRedPoint);
                        }
                    }
                }
                else if (activeData.iActiveID == ActiveChargeFrame.monthlySignInActiveID)
                {
                    if (m_akGoRedPoints != null)
                    {
                        // 可进行签到，可免费补签，可领取累计签到奖励时，按钮显示小红点；领取奖励后小红点消失
                        var isShowRedPoint = ActivityDataManager.GetInstance().IsShowSignInRedPoint();
                        for (var i = 0; i < m_akGoRedPoints.Count; i++)
                        {                           
                            m_akGoRedPoints[i].Current.CustomActive(isShowRedPoint);
                        }
                    }
                }
                else if (activeData.iActiveID == WarriorRecruitDataManager.GetInstance().warriorRecruitActiveID)
                {
                    if (m_akGoRedPoints != null)
                    {
                        var isShowRedPoint = WarriorRecruitDataManager.GetInstance().IsRedPointShow();
                        for (int i = 0; i < m_akGoRedPoints.Count; i++)
                        {
                            var redPoint = m_akGoRedPoints[i];
                            if (redPoint == null)
                            {
                                continue;
                            }

                            if (redPoint.Current == null)
                            {
                                continue;
                            }

                            redPoint.Current.CustomActive(isShowRedPoint);
                        }
                    }
                }
            }
        }

    }

}
