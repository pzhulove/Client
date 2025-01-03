using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
///////删除linq
using ProtoTable;
using Scripts.UI;

namespace GameClient
{
    class MissionFrameNewData
    {
        public int iFirstFilter;
        public bool bCycle;
    }

    class MissionFrameNew : ClientFrame
    {
        public static void OpenLinkFrame(string strParam)
        {
            try
            {
                var tokens = strParam.Split('|');
                if (tokens.Length == 1)
                {
                    MissionFrameNewData data = new MissionFrameNewData();
                    data.iFirstFilter = int.Parse(tokens[0]);
                    data.bCycle = false;
                    GameClient.ClientSystemManager.GetInstance().CloseFrame<MissionFrameNew>();
                    GameClient.ClientSystemManager.GetInstance().OpenFrame<MissionFrameNew>(FrameLayer.Middle, data);
                }
                else if (tokens.Length == 2)
                {
                    MissionFrameNewData data = new MissionFrameNewData();
                    data.iFirstFilter = int.Parse(tokens[0]);
                    data.bCycle = (bool)(int.Parse(tokens[1]) == 1);
                    GameClient.ClientSystemManager.GetInstance().CloseFrame<MissionFrameNew>();
                    GameClient.ClientSystemManager.GetInstance().OpenFrame<MissionFrameNew>(FrameLayer.Middle, data);
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e.ToString());
            }
        }
        private ComItem skillComItem = null;
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Mission/MissionFrame.prefab";
        }

        [UIControl("Content/Describe/LevelHint", typeof(Text))]
        Text levelHint;

        [UIEventHandle("BG/Title/Close")]
        void OnClickClose()
        {
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.SwitchToMission);
            frameMgr.CloseFrame(this);
        }

        public void OnFilterZeroChanged(FilterZeroType eFilterZeroType)
        {
            m_akFilterZeros.GetObject(eFilterZeroType).toggle.isOn = true;
        }

        #region _Filter0
        public enum FilterZeroType
        {
            //             [System.ComponentModel.Description("未接受")]
            //             FZT_UNACCEPTED = 0,
            [System.ComponentModel.Description("已完成")]
            FZT_FINISHED = -1, 

            [System.ComponentModel.Description("已领取")]
            FZT_ACCEPTED = 0,
            [System.ComponentModel.Description("称号任务")]
            FZT_TITLE_TASK,
            FZT_COUNT,
        }

        class FilterZero : CachedObject
        {
            GameObject goLocal;
            GameObject goPrefab;
            GameObject goParent;
            FilterZeroType eCurrent;
            MissionFrameNew THIS;

            GameObject goRedPoint;
            Text normalText;
            Text checkText;

            public Toggle toggle;

            public override void OnDestroy()
            {
                goLocal = null;
                goPrefab = null;
                goParent = null;
                eCurrent = FilterZeroType.FZT_COUNT;
                THIS = null;
                goRedPoint = null;
                normalText = null;
                checkText = null;
                if (toggle != null)
                {
                    toggle.onValueChanged.RemoveAllListeners();
                    toggle = null;
                }
            }

            public override void OnCreate(object[] param)
            {
                goParent = param[0] as GameObject;
                goPrefab = param[1] as GameObject;
                eCurrent = (FilterZeroType)param[2];
                THIS = param[3] as MissionFrameNew;
                if (goLocal == null)
                {
                    goLocal = GameObject.Instantiate(goPrefab);
                    Utility.AttachTo(goLocal, goParent);

                    normalText = Utility.FindComponent<Text>(goLocal, "Text");
                    checkText = Utility.FindComponent<Text>(goLocal, "CheckMark/Text");
                    goRedPoint = Utility.FindChild(goLocal, "RedPoint");

                    toggle = goLocal.GetComponent<Toggle>();
                    toggle.onValueChanged.RemoveAllListeners();
                    toggle.onValueChanged.AddListener((bool bValue) =>
                    {
                        if (bValue)
                        {
                            OnSelected();
                        }
                    });
                }
                Enable();
                _Update();
            }
            public override void OnRecycle()
            {
                if (goLocal != null)
                {
                    goLocal.CustomActive(false);
                }
            }
            public override void OnDecycle(object[] param)
            {
                OnCreate(param);
            }
            public override void OnRefresh(object[] param)
            {
                _Update();
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
            public override bool NeedFilter(object[] param)
            {
                return false;
            }
            public void SetRedPointOn(bool bOn)
            {
                goRedPoint.CustomActive(bOn);
            }

            void _Update()
            {
                normalText.text = checkText.text = Utility.GetEnumDescription(eCurrent);
                goLocal.name = eCurrent.ToString();
            }

            void OnSelected()
            {
                THIS._OnFilterChanged0(eCurrent);
            }
        }
        CachedObjectDicManager<FilterZeroType, FilterZero> m_akFilterZeros = new CachedObjectDicManager<FilterZeroType, FilterZero>();

        void _InitFilter0()
        {
            m_akFilterZeros.Clear();
            GameObject goParent = Utility.FindChild(frame,"Content/HorizenFilter");
            GameObject goPrefab = Utility.FindChild(goParent, "Filter");
            goPrefab.CustomActive(false);
            m_eFilterZeroType = FilterZeroType.FZT_COUNT;

            for (int i = 0; i < (int)FilterZeroType.FZT_COUNT; ++i)
            {
                m_akFilterZeros.Create((FilterZeroType)i, new object[] { goParent, goPrefab, (FilterZeroType)i, this });
            }
        }

        void _OnFilterChanged0(FilterZeroType eCurrent)
        {
            m_eFilterZeroType = eCurrent;
            m_kMainMissionList.Filter(m_eFilterZeroType);

            //_SetDefaultMainMission(m_eFilterZeroType);
        }
        FilterZeroType m_eFilterZeroType;
        #endregion
        #region _Filter1
        public enum FilterFirstType
        {
            [System.ComponentModel.Description("任务手册")]
            FFT_MAIN_OR_BRANCH = 0,
            [System.ComponentModel.Description("每日")]
            FFT_DAILY,
            [System.ComponentModel.Description("成就")]
            FFT_ACHIEVEMENT,
            FFT_COUNT,
        }

        class FilterFirst : CachedObject
        {
            GameObject goLocal;
            GameObject goPrefab;
            GameObject goParent;
            FilterFirstType eCurrent;
            MissionFrameNew THIS;

            GameObject goRedPoint;
            Text normalText;
            Text checkText;
            FunctionUnLock.eFuncType eFuncType = FunctionUnLock.eFuncType.None;

            public Toggle toggle;

            public override void OnDestroy()
            {
                goLocal = null;
                goPrefab = null;
                goParent = null;
                eCurrent = FilterFirstType.FFT_COUNT;
                THIS = null;
                goRedPoint = null;
                normalText = null;
                checkText = null;
                eFuncType = FunctionUnLock.eFuncType.None;

                if (toggle != null)
                {
                    toggle.onValueChanged.RemoveAllListeners();
                    toggle = null;
                }
            }

            public override void OnCreate(object[] param)
            {
                goParent = param[0] as GameObject;
                goPrefab = param[1] as GameObject;
                eCurrent = (FilterFirstType)param[2];
                THIS = param[3] as MissionFrameNew;
                if (goLocal == null)
                {
                    goLocal = GameObject.Instantiate(goPrefab);
                    Utility.AttachTo(goLocal, goParent);

                    goRedPoint = Utility.FindChild(goLocal, "RedPoint");
                    normalText = Utility.FindComponent<Text>(goLocal, "Text");
                    checkText = Utility.FindComponent<Text>(goLocal, "CheckMark/Text");

                    toggle = goLocal.GetComponent<Toggle>();
                    toggle.onValueChanged.RemoveAllListeners();
                    toggle.onValueChanged.AddListener((bool bValue) =>
                    {
                        if (bValue)
                        {
                            OnSelected();
                        }
                    });
                }
                Enable();
                _Update();
            }
            public override void OnRecycle()
            {
                if (goLocal != null)
                {
                    goLocal.CustomActive(false);
                }
            }
            public override void OnDecycle(object[] param)
            {
                OnCreate(param);
            }
            public override void OnRefresh(object[] param)
            {
                _Update();
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

            public override bool NeedFilter(object[] param)
            {
                if (eFuncType == FunctionUnLock.eFuncType.None)
                {
                    return false;
                }

                if(eFuncType == FunctionUnLock.eFuncType.DailyTask)
                {
                    return true;
                }

                var FuncUnlockdata = TableManager.GetInstance().GetTableItem<FunctionUnLock>((int)eFuncType);
                if (FuncUnlockdata == null)
                {
                    return true;
                }

                if (PlayerBaseData.GetInstance().Level < FuncUnlockdata.FinishLevel)
                {
                    return true;
                }

                return false;
            }

            void _Update()
            {
                normalText.text = checkText.text = Utility.GetEnumDescription(eCurrent);
                goLocal.name = eCurrent.ToString();

                eFuncType = FunctionUnLock.eFuncType.None;
                if (eCurrent == FilterFirstType.FFT_ACHIEVEMENT)
                {
                    eFuncType = FunctionUnLock.eFuncType.Achievement;
                }
                else if (eCurrent == FilterFirstType.FFT_DAILY)
                {
                    eFuncType = FunctionUnLock.eFuncType.DailyTask;
                }
            }

            void OnSelected()
            {
                THIS._OnFilterChanged1(eCurrent);
            }

            public void SetRedPointOn(bool bOn)
            {
                goRedPoint.CustomActive(bOn);
            }
        }
        CachedObjectDicManager<FilterFirstType, FilterFirst> m_akFilterFirsts = new CachedObjectDicManager<FilterFirstType, FilterFirst>();

        void _InitFilter1()
        {
            m_akFilterFirsts.Clear();
            GameObject goParent = Utility.FindChild(frame,"VerticalFilter");
            GameObject goPrefab = Utility.FindChild(goParent, "Filter");
            goPrefab.CustomActive(false);

            for (int i = 0; i < (int)FilterFirstType.FFT_COUNT; ++i)
            {
                if(i != (int)FilterFirstType.FFT_MAIN_OR_BRANCH)
                {
                    continue;
                }
                var current = m_akFilterFirsts.Create((FilterFirstType)i, new object[] { goParent, goPrefab, (FilterFirstType)i, this });
                if (current != null)
                {
                    m_akFilterFirsts.FilterObject((FilterFirstType)i, null);
                }
            }
        }

        GameObject goContent;
        GameObject goAchievementContent;
        GameObject goMainDescribe;
        void _OnFilterChanged1(FilterFirstType eCurrent)
        {
            m_eFilterFirstType = eCurrent;
            _InitializeTab(m_eFilterFirstType);
            goContent.CustomActive(m_eFilterFirstType == FilterFirstType.FFT_MAIN_OR_BRANCH);
            goAchievementContent.CustomActive(m_eFilterFirstType == FilterFirstType.FFT_ACHIEVEMENT);
        }
        FilterFirstType m_eFilterFirstType;
        #endregion

        #region MissionItemObject
        public void OnMissionSelected(MissionManager.SingleMissionInfo value)
        {
            m_kCurrent = value;

            goDescribe.CustomActive(m_kCurrent != null);
            levelHint.CustomActive(m_kCurrent != null && !MissionManager.GetInstance().IsLevelFit(value.missionItem.ID));

            if (m_kCurrent != null)
            {
                levelHint.text = TR.Value("mission_level_hint", value.missionItem.MinPlayerLv);
                kCurrentName.text = MissionManager.GetInstance().GetMissionName((uint)value.missionItem.ID);
                kCurrentDesc.SetText(Utility.ParseMissionText(m_kCurrent.missionItem.ID));
                // kCurrentIcon.sprite = AssetLoader.instance.LoadRes(Utility.GetMissionIcon(m_kCurrent.missionItem.TaskType), typeof(Sprite)).obj as Sprite;
                ETCImageLoader.LoadSprite(ref kCurrentIcon, Utility.GetMissionIcon(m_kCurrent.missionItem.TaskType));

                //设置循环任务刷新消耗
                if (value.missionItem.TaskType == ProtoTable.MissionTable.eTaskType.TT_CYCLE)
                {
                    var SystemValueTableData = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_CYCLE_TASK_REFRESH_FREE_COUNT);
                    int iFreeTimes = SystemValueTableData.Value;
                    int iRefreshTimes = CountDataManager.GetInstance().GetCount(CounterKeys.COUNTER_CYCLE_REFRESH_TIMES);
                    if (iFreeTimes > iRefreshTimes)
                    {
                        m_kCycleOrgHint.CustomActive(true);
                        m_goCostHint.CustomActive(false);
                    }
                    else
                    {
                        m_kCycleOrgHint.CustomActive(false);
                        m_goCostHint.CustomActive(true);
                        int iBindPointID = (int)ItemDataManager.GetInstance().GetMoneyIDByType(ProtoTable.ItemTable.eSubType.BindPOINT);
                        int iOwnedCount = ItemDataManager.GetInstance().GetOwnedItemCount(iBindPointID);
                        SystemValueTableData = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_CYCLE_TASK_REFRESH_CONSUME);
                        int iNeedCost = 0;
                        if (SystemValueTableData != null)
                        {
                            iNeedCost = SystemValueTableData.Value;
                        }
                        if (iOwnedCount >= iNeedCost)
                        {
                            m_kRefreshPre.color = m_kRefreshCount.color = Color.white;
                        }
                        else
                        {
                            m_kRefreshPre.color = m_kRefreshCount.color = Color.red;
                        }
                        m_kRefreshCount.text = iNeedCost.ToString();

                        var item = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(iBindPointID);
                        if (item != null)
                        {
                            // m_kRefreshCoin.sprite = AssetLoader.instance.LoadRes(item.Icon, typeof(Sprite)).obj as Sprite;
                            ETCImageLoader.LoadSprite(ref m_kRefreshCoin, item.Icon);
                        }
                    }
                }
                btnFastFinish.CustomActive(false);
                if (m_kCurrent.missionItem.FinishRightNowLevel != 0)
                {
                    if (m_kCurrent.missionItem.FinishRightNowLevel <= PlayerBaseData.GetInstance().Level && m_kCurrent.status == (int)Protocol.TaskStatus.TASK_UNFINISH)
                    {
                        btnFastFinish.CustomActive(true);
                        mFastFinishCount.text = m_kCurrent.missionItem.FinishRightNowItemNum.ToString();
                        bool notEnough = m_kCurrent.missionItem.FinishRightNowItemNum > ItemDataManager.GetInstance().GetOwnedItemCount(m_kCurrent.missionItem.FinishRightNowItemType,false);
                        mFastFinishCount.color = notEnough ? Color.red : Color.white;
                        var itemTableData = TableManager.GetInstance().GetTableItem<ItemTable>(m_kCurrent.missionItem.FinishRightNowItemType);
                        if (itemTableData != null)
                        {
                            ETCImageLoader.LoadSprite(ref mFastFinishIcon, itemTableData.Icon);
                        }
                    }
                }
                //设置安扭
                if (m_kCurrent.missionItem.TaskType == ProtoTable.MissionTable.eTaskType.TT_MAIN || m_kCurrent.missionItem.TaskType == ProtoTable.MissionTable.eTaskType.TT_BRANCH ||
                    m_kCurrent.missionItem.TaskType == ProtoTable.MissionTable.eTaskType.TT_CYCLE || m_kCurrent.missionItem.TaskType == ProtoTable.MissionTable.eTaskType.TT_AWAKEN)
                {
                    goBtnGiveUp.CustomActive(m_kCurrent.missionItem.TaskType != ProtoTable.MissionTable.eTaskType.TT_CHANGEJOB &&
                        m_kCurrent.missionItem.TaskType != ProtoTable.MissionTable.eTaskType.TT_CYCLE &&
    m_kCurrent.status != (int)Protocol.TaskStatus.TASK_FINISHED &&
    m_kCurrent.status != (int)Protocol.TaskStatus.TASK_INIT);
                    goBtnComplete.CustomActive(m_kCurrent.status == (int)Protocol.TaskStatus.TASK_FINISHED);
                    goBtnTrace.CustomActive(true);
                    btnTrace.enabled = true;
                    comTrace.enabled = false;
                    goBtnAccept.CustomActive(m_kCurrent.status == (int)Protocol.TaskStatus.TASK_INIT);
                    btnAccept.enabled = true;
                    comAccept.enabled = false;
                    goBtnAward.CustomActive(false);
                    goBtnRefresh.CustomActive(m_kCurrent.missionItem.TaskType == ProtoTable.MissionTable.eTaskType.TT_CYCLE);
                    comGrayGiveUp.enabled = m_kCurrent.missionItem.TaskType == MissionTable.eTaskType.TT_MAIN;
                }
                else if(m_kCurrent.missionItem.TaskType == ProtoTable.MissionTable.eTaskType.TT_TITLE)
                {
                    goBtnGiveUp.CustomActive(m_kCurrent.status != (int)Protocol.TaskStatus.TASK_FINISHED &&
                        m_kCurrent.status != (int)Protocol.TaskStatus.TASK_INIT);
                    goBtnComplete.CustomActive(m_kCurrent.status == (int)Protocol.TaskStatus.TASK_FINISHED);

                    int iActLv = m_kCurrent.missionItem.MinPlayerLv;
                    bool bCanAccept = iActLv <= PlayerBaseData.GetInstance().Level;
                    goBtnAccept.CustomActive(m_kCurrent.status == (int)Protocol.TaskStatus.TASK_INIT);
                    btnAccept.enabled = bCanAccept;
                    comAccept.enabled = !btnAccept.enabled;

                    goBtnTrace.CustomActive(true);
                    bool bCanTrace = true;
                    if(m_kCurrent.status == (int)Protocol.TaskStatus.TASK_INIT && !bCanAccept)
                    {
                        bCanTrace = false;
                    }
                    btnTrace.enabled = bCanTrace;
                    comTrace.enabled = !btnTrace.enabled;

                    goBtnAward.CustomActive(false);
                    goBtnRefresh.CustomActive(false);
                    comGrayGiveUp.enabled = false;
                }
                else if (m_kCurrent.missionItem.TaskType == ProtoTable.MissionTable.eTaskType.TT_ACHIEVEMENT)
                {
                    goBtnGiveUp.CustomActive(false);
                    goBtnTrace.CustomActive(false);
                    btnTrace.enabled = true;
                    comTrace.enabled = false;
                    goBtnAccept.CustomActive(false);
                    btnAccept.enabled = true;
                    comAccept.enabled = false;
                    goBtnComplete.CustomActive(false);
                    goBtnAward.CustomActive(true);

                    comGray.enabled = m_kCurrent.status != (int)Protocol.TaskStatus.TASK_FINISHED;
                    comGrayGiveUp.enabled = false;
                }
                else
                {
                    goBtnGiveUp.CustomActive(false);
                    goBtnTrace.CustomActive(false);
                    btnTrace.enabled = true;
                    comTrace.enabled = false;
                    goBtnAccept.CustomActive(false);
                    btnAccept.enabled = true;
                    comAccept.enabled = false;
                    goBtnComplete.CustomActive(false);
                    goBtnRefresh.CustomActive(false);
                    goBtnAward.CustomActive(true);

                    comGray.enabled = m_kCurrent.status != (int)Protocol.TaskStatus.TASK_FINISHED;
                    comGrayGiveUp.enabled = false;
                }

                //设置奖励
                List<ComItem> akCachedObject = new List<ComItem>();
                for (int i = 0; i < goAwardArray.transform.childCount; ++i)
                {
                    var child = goAwardArray.transform.GetChild(i);
                    if (child != null)
                    {
                        var comItem = child.GetComponent<ComItem>();
                        if (comItem != null)
                        {
                            comItem.Setup(null, null);
                            comItem.gameObject.CustomActive(false);
                            akCachedObject.Add(comItem);
                        }
                    }
                }

                var awards = MissionManager.GetInstance().GetMissionAwards(m_kCurrent.missionItem.ID);
                for (int i = 0; i < awards.Count; ++i)
                {
                    var award = awards[i];
                    ProtoTable.ItemTable itemInfo = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(award.ID);
                    if (itemInfo != null)
                    {
                        var ItemData = GameClient.ItemDataManager.CreateItemDataFromTable(award.ID);
                        if (ItemData != null)
                        {
                            ComItem current = null;
                            if (akCachedObject.Count > 0)
                            {
                                current = akCachedObject[0];
                                akCachedObject.RemoveAt(0);
                            }
                            else
                            {
                                current = CreateComItem(goAwardArray);
                            }

                            ItemData.Count = award.Num;
                            ItemData.StrengthenLevel = award.StrengthenLevel;
                            ItemData.EquipType = (EEquipType)award.EquipType;

                            if (current != null)
                            {
                                current.gameObject.CustomActive(true);
                                current.Setup(ItemData, _OnAwardItemClicked);
                            }
                        }
                    }
                }
                //这个对技能槽解锁任务进行特殊处理
               
                var tabledata2 = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)SystemValueTable.eType2.SVT_12_SKILL_SLOT_TASK_ID);
                if (tabledata2 == null) return;

                if (m_kCurrent.taskID == (uint)tabledata2.Value)
                {
                    var table = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType3.SVT_12_SKILL_ITEM_ID);
                    if (table == null) return;
                    ItemData skillItemData = GameClient.ItemDataManager.CreateItemDataFromTable(table.Value);
                    if (skillComItem == null)
                    {
                        if (skillItemData != null)
                        {
                            skillComItem = CreateComItem(goAwardArray);
                            skillComItem.Setup(skillItemData, _OnAwardItemClicked);
                            skillComItem.gameObject.CustomActive(true);
                        }
                    }
                    else
                    {
                        skillComItem.Setup(skillItemData, _OnAwardItemClicked);
                        skillComItem.gameObject.CustomActive(true);
                    }
                    
                }
                else
                {
                    if (skillComItem != null)
                    {
                        skillComItem.Setup(null, null);
                        skillComItem.gameObject.CustomActive(false);
                    }
                }
            }
        }
        void _OnAwardItemClicked(GameObject obj, ItemData item)
        {
            ItemTipManager.GetInstance().ShowTip(item);
        }
        GameObject goParent;
        GameObject goPrefab;

        GameObject goDescribe;
        Text kCurrentName;
        Image kCurrentIcon;
        LinkParse kCurrentDesc;
        GameObject goAwardArray;

        GameObject goBtnGiveUp;
        GameObject goBtnComplete;
        GameObject goBtnTrace;
        GameObject goBtnAccept;
        GameObject goBtnAward;
        GameObject goBtnRefresh;
        UIGray comGrayGiveUp;
        UIGray comGray;
        [UIControl("Content/Describe/BtnTrace", typeof(UIGray))]
        UIGray comTrace;
        [UIControl("Content/Describe/BtnTrace", typeof(Button))]
        Button btnTrace;
        [UIControl("Content/Describe/BtnAccept", typeof(UIGray))]
        UIGray comAccept;
        [UIControl("Content/Describe/BtnAccept", typeof(Button))]
        Button btnAccept;
        [UIControl("Content/Describe/BtnFastFinish", typeof(Button))]
        Button btnFastFinish;

        Text m_kCycleOrgHint;

        GameObject m_goCostHint;
        Text m_kRefreshPre;
        Text m_kRefreshCount;
        Image m_kRefreshCoin;
        Image mFastFinishIcon;
        Text mFastFinishCount;

        void _InitItemObjects()
        {
            goParent = Utility.FindChild(frame,"Content/MissionArray/ScrollView/ViewPort/Content");
            goPrefab = Utility.FindChild(goParent, "ItemObject");
            goPrefab.CustomActive(false);

            goDescribe = Utility.FindChild(frame,"Content/Describe");
            kCurrentName = Utility.FindComponent<Text>(goDescribe, "tittle/Text");
            kCurrentIcon = Utility.FindComponent<Image>(goDescribe, "tittle/typeIcon");
            kCurrentDesc = Utility.FindComponent<LinkParse>(goDescribe, "MissionDescribe/ViewPort/Content");
            goAwardArray = Utility.FindChild(goDescribe, "MissionAward/AwardArray");

            goBtnGiveUp = Utility.FindChild(goDescribe, "BtnGiveUp");
            goBtnComplete = Utility.FindChild(goDescribe, "BtnComplete");
            goBtnTrace = Utility.FindChild(goDescribe, "BtnTrace");
            goBtnAccept = Utility.FindChild(goDescribe, "BtnAccept");
            goBtnAward = Utility.FindChild(goDescribe, "BtnAward");
            goBtnRefresh = Utility.FindChild(goDescribe, "BtnRefresh");
            comGray = Utility.FindComponent<UIGray>(goDescribe, "BtnAward");
            comGrayGiveUp = Utility.FindComponent<UIGray>(goDescribe, "BtnGiveUp");

            m_kCycleOrgHint = Utility.FindComponent<Text>(frame,"Content/Describe/BtnRefresh/renwutishi");
            m_goCostHint = Utility.FindChild(frame,"Content/Describe/BtnRefresh/Hint");
            m_kRefreshPre = Utility.FindComponent<Text>(frame,"Content/Describe/BtnRefresh/Hint/refresh");
            m_kRefreshCount = Utility.FindComponent<Text>(frame,"Content/Describe/BtnRefresh/Hint/Count");
            m_kRefreshCoin = Utility.FindComponent<Image>(frame,"Content/Describe/BtnRefresh/Hint/Image");
            mFastFinishIcon = Utility.FindComponent<Image>(btnFastFinish.gameObject,"Hint/Image");
            mFastFinishCount = Utility.FindComponent<Text>(btnFastFinish.gameObject, "Hint/Count");
        }
        #endregion

        MissionManager.SingleMissionInfo m_kCurrent;

        MissionFrameNewData data = new MissionFrameNewData();

        FilterFirstType GetDefaultFirstType(int iFirstFilter)
        {
            FilterFirstType eCurrentFirstType = (FilterFirstType)iFirstFilter;
            if(eCurrentFirstType == FilterFirstType.FFT_DAILY)
            {
                var FuncUnlockdata = TableManager.GetInstance().GetTableItem<FunctionUnLock>((int)FunctionUnLock.eFuncType.DailyTask);
                if (FuncUnlockdata != null && PlayerBaseData.GetInstance().Level >= FuncUnlockdata.FinishLevel)
                {
                    return eCurrentFirstType;
                }
                eCurrentFirstType = FilterFirstType.FFT_MAIN_OR_BRANCH;
            }
            else if(eCurrentFirstType == FilterFirstType.FFT_ACHIEVEMENT)
            {
                var FuncUnlockdata = TableManager.GetInstance().GetTableItem<FunctionUnLock>((int)FunctionUnLock.eFuncType.Achievement);
                if (FuncUnlockdata != null && PlayerBaseData.GetInstance().Level >= FuncUnlockdata.FinishLevel)
                {
                    return eCurrentFirstType;
                }
                eCurrentFirstType = FilterFirstType.FFT_MAIN_OR_BRANCH;
            }
            else
            {
                eCurrentFirstType = FilterFirstType.FFT_MAIN_OR_BRANCH;
            }

            return eCurrentFirstType;
        }

        MainMissionList m_kMainMissionList = new MainMissionList();
        AchievementMissionList m_kAchievementList = new AchievementMissionList();
        DailyMissionList m_kDailyMissionList = new DailyMissionList();

        Toggle finished = null;
        ComUIListScript MissionArray = null;
        List<GameClient.MissionManager.SingleMissionInfo> finishedMissions = new List<MissionManager.SingleMissionInfo>();

        GameObject MissionDescribe = null;
        ComUIListScript talkInfo = null;
        List<object> talkInfoComUIListDatas = new List<object>();

        protected override void _OnOpenFrame()
        {
            SetIsNeedClearWhenChangeScene(true);

            data = userData as MissionFrameNewData;
            goContent = Utility.FindChild(frame, "Content");
            goMainDescribe = Utility.FindChild(frame,"Content/Describe");
            goAchievementContent = Utility.FindChild(frame,"AchievementContent");
            goMainDescribe.CustomActive(false);

            _InitItemObjects();
            _InitFilter0();
            _InitFilter1();

            _CheckMainMission(false);
            _CheckAchievementMission(false);
            _InitDefaultMission();
            PlayerBaseData.GetInstance().onLevelChanged += OnLevelChanged;
            PlayerBaseData.GetInstance().onMoneyChanged += OnMoneyChanged;

            finishedMissions = new List<MissionManager.SingleMissionInfo>();
            CalcFinishedMissionDatas();

            if(finished != null)
            {
                finished.gameObject.GetComponent<RectTransform>().SetAsLastSibling();
            }

            InitTalkInfoComUIList();
        }

        void CalcFinishedMissionDatas()
        {
            var ids = MissionManager.GetInstance().finisedTaskIDs;
            if(ids == null)
            {
                return;
            }

            finishedMissions = new List<MissionManager.SingleMissionInfo>();
            if(finishedMissions == null)
            {
                return;
            }

            for(int i = 0;i < ids.Count;i++)
            {
                MissionManager.SingleMissionInfo missionInfo = new MissionManager.SingleMissionInfo();
                if(missionInfo == null)
                {
                    continue;
                }

                missionInfo.taskID = ids[i];
                missionInfo.status = (byte)Protocol.TaskStatus.TASK_OVER;
                missionInfo.missionItem = TableManager.GetInstance().GetTableItem<MissionTable>((int)ids[i]);

                finishedMissions.Add(missionInfo);
            }
        }
        
        ComMainMissionScript _OnBindItemDelegate(GameObject itemObject)
        {
            return itemObject.GetComponent<ComMainMissionScript>();
        }       

        void _OnItemVisiableDelegate(ComUIListElementScript item)
        {
            if (item != null && item.m_index >= 0 && item.m_index < finishedMissions.Count)
            {
                var current = item.gameObjectBindScript as ComMainMissionScript;
                if (current != null)
                {
                    current.OnVisible(finishedMissions[item.m_index], this);
                }
            }
        }

        void _OnItemSelectedDelegate(ComUIListElementScript item)
        {
            if (item != null)
            {
                var current = item.gameObjectBindScript as ComMainMissionScript;
                if (current != null)
                {                   
                    this.OnMissionSelected(current.Value);
                }

                UpdateTalkInfoComUIList(current.Value);
            }
        }

        void _OnItemChangeDisplayDelegate(ComUIListElementScript item, bool bSelected)
        {
            if (item != null)
            {
                var comMainScript = item.gameObjectBindScript as ComMainMissionScript;
                if (comMainScript != null)
                {
                    comMainScript.OnDisplayChange(bSelected);
                }
            }
        }

        protected override void _bindExUI()
        {
            finished = mBind.GetCom<Toggle>("finished");
            finished.SafeSetOnValueChangedListener((value) =>
            {
                var comUIListScript = MissionArray;
                if(comUIListScript == null)
                {
                    return;
                }

                if(finishedMissions == null)
                {
                    return;
                }

                if (value)
                {
                    comUIListScript.Initialize();

                    comUIListScript.onBindItem += _OnBindItemDelegate;
                    comUIListScript.onItemVisiable += _OnItemVisiableDelegate;
                    comUIListScript.onItemSelected += _OnItemSelectedDelegate;
                    comUIListScript.onItemChageDisplay += _OnItemChangeDisplayDelegate;

                    comUIListScript.SetElementAmount(finishedMissions.Count);

                    m_kMainMissionList.SetCurFilterType(FilterZeroType.FZT_FINISHED);
                }
                else
                {
                    comUIListScript.onBindItem -= _OnBindItemDelegate;
                    comUIListScript.onItemVisiable -= _OnItemVisiableDelegate;
                    comUIListScript.onItemSelected -= _OnItemSelectedDelegate;
                    comUIListScript.onItemChageDisplay -= _OnItemChangeDisplayDelegate;
                }

                talkInfo.CustomActive(value);
                // talkInfo.CustomActive(false);
            });

            MissionArray = mBind.GetCom<ComUIListScript>("MissionArray");

            MissionDescribe = mBind.GetGameObject("MissionDescribe");
            talkInfo = mBind.GetCom<ComUIListScript>("talkInfo");
        }

        protected override void _unbindExUI()
        {
            finished = null;
            MissionArray = null;
            MissionDescribe = null;
            talkInfo = null;
        }

        void InitTalkInfoComUIList()
        {
            if (talkInfo == null)
            {
                return;
            }

            talkInfo.Initialize();
            talkInfo.onBindItem = (go) =>
            {
                return go;
            };

            talkInfo.onItemVisiable = (go) =>
            {
                if (go == null)
                {
                    return;
                }

                if (talkInfoComUIListDatas == null)
                {
                    return;
                }

                ComUIListTemplateItem comUIListItem = go.GetComponent<ComUIListTemplateItem>();
                if (comUIListItem == null)
                {
                    return;
                }

                if (go.m_index >= 0 && go.m_index < talkInfoComUIListDatas.Count)
                {
                    comUIListItem.SetUp(talkInfoComUIListDatas[go.m_index]);
                }
            };
        }

        void CalcTalkInfoComUIListDatas(MissionManager.SingleMissionInfo missionInfo)
        {
            if(missionInfo == null)
            {
                return;
            }

            if(missionInfo.missionItem == null)
            {
                return;
            }           

            talkInfoComUIListDatas = new List<object>();
            if (talkInfoComUIListDatas == null)
            {
                return;
            }

            {
                var table = TableManager.GetInstance().GetTableItem<TalkTable>(missionInfo.missionItem.BefTaskDlgID);
                int count = 0;
                while (table != null && count++ <= 100)
                {
                    if(!string.IsNullOrEmpty(table.TalkText))
                    {
                        talkInfoComUIListDatas.Add(table.ID);
                    }
                    
                    table = TableManager.GetInstance().GetTableItem<TalkTable>(table.NextID);
                }
            }

            {
                var table = TableManager.GetInstance().GetTableItem<TalkTable>(missionInfo.missionItem.AftTaskDlgID);
                int count = 0;
                while (table != null && count++ <= 100)
                {
                    if (!string.IsNullOrEmpty(table.TalkText))
                    {
                        talkInfoComUIListDatas.Add(table.ID);
                    }

                    table = TableManager.GetInstance().GetTableItem<TalkTable>(table.NextID);
                }
            }
        }

        void UpdateTalkInfoComUIList(MissionManager.SingleMissionInfo missionInfo)
        {
            if(missionInfo == null)
            {
                return;
            }

            if (talkInfo == null)
            {
                return;
            }

            CalcTalkInfoComUIListDatas(missionInfo);
            if (talkInfoComUIListDatas == null)
            {
                return;
            }

            talkInfo.SetElementAmount(talkInfoComUIListDatas.Count);
        }

        void _InitializeTab(FilterFirstType eFilterFirstType)
        {
            switch(eFilterFirstType)
            {
                case FilterFirstType.FFT_MAIN_OR_BRANCH:
                    {
                        if (!m_kMainMissionList.Initialized)
                        {
                            if (!data.bCycle)
                            {
                                m_kMainMissionList.Initialize(this, Utility.FindChild(frame, "Content/MissionArray"), _CheckMainMission, FilterZeroType.FZT_COUNT, false, false);
                            }
                            else
                            {
                                m_kMainMissionList.Initialize(this, Utility.FindChild(frame, "Content/MissionArray"), _CheckMainMission, FilterZeroType.FZT_ACCEPTED, true, true);
                            }
                        }
                    }
                    break;
                case FilterFirstType.FFT_ACHIEVEMENT:
                    {
                        if (!m_kAchievementList.Initialized)
                            m_kAchievementList.Initialize(this, Utility.FindChild(frame, "AchievementContent"), _CheckAchievementMission);
                    }
                    break;
            }
        }

        void _InitDefaultMission()
        {
            if(data == null)
            {
                data = new MissionFrameNewData();
                data.iFirstFilter = (int)FilterFirstType.FFT_MAIN_OR_BRANCH;
            }
            else
            {
                if(data.iFirstFilter != (int)FilterFirstType.FFT_MAIN_OR_BRANCH)
                {
                    data.iFirstFilter = (int)FilterFirstType.FFT_MAIN_OR_BRANCH;
                    Logger.LogErrorFormat("MissionDaily and MissionAchievement has been removed from this frame , please link other frame !!!");
                }
            }

            m_akFilterFirsts.GetObject((FilterFirstType)data.iFirstFilter).toggle.isOn = true;
        }

        void _CheckMainMission(bool bCheck)
        {
            var firstFilter = m_akFilterFirsts.GetObject(FilterFirstType.FFT_MAIN_OR_BRANCH);
            if (firstFilter != null)
            {
                var missions = MissionManager.GetInstance().taskGroup.Values.ToList();
                bool bHasRedPoint = false;
                for(int i = 0; i < missions.Count; ++i)
                {
                    var x = missions[i];
                    if(null == x || null == x.missionItem)
                    {
                        continue;
                    }
                    
                    if(x.status != (int)Protocol.TaskStatus.TASK_FINISHED)
                    {
                        continue;
                    }

                    if (!(x.missionItem.TaskType == MissionTable.eTaskType.TT_MAIN ||
                    x.missionItem.TaskType == MissionTable.eTaskType.TT_CYCLE ||
                    x.missionItem.TaskType == MissionTable.eTaskType.TT_BRANCH ||
                    x.missionItem.TaskType == MissionTable.eTaskType.TT_TITLE))
                    {
                        continue;
                    }

                    bHasRedPoint = true;
                    break;
                }
                firstFilter.SetRedPointOn(bHasRedPoint);
            }

            var filterAchievement = m_akFilterFirsts.GetObject(FilterFirstType.FFT_ACHIEVEMENT);
            if(null != filterAchievement)
            {
                var missions = MissionManager.GetInstance().taskGroup.Values.ToList();
                bool bHasRedPoint = false;
                for (int i = 0; i < missions.Count; ++i)
                {
                    var x = missions[i];
                    if (null == x || null == x.missionItem)
                    {
                        continue;
                    }

                    if (x.status != (int)Protocol.TaskStatus.TASK_FINISHED)
                    {
                        continue;
                    }

                    if (!(x.missionItem.TaskType == MissionTable.eTaskType.TT_ACHIEVEMENT &&
                x.missionItem.SubType == MissionTable.eSubType.Daily_Null))
                    {
                        continue;
                    }

                    bHasRedPoint = true;
                    break;
                }
                filterAchievement.SetRedPointOn(bHasRedPoint);
            }

            var zeroFilter = m_akFilterZeros.GetObject(FilterZeroType.FZT_ACCEPTED);
            if (zeroFilter != null)
            {
                var missions = MissionManager.GetInstance().taskGroup.Values.ToList();
                bool bHasRedPoint = false;
                for (int i = 0; i < missions.Count; ++i)
                {
                    var x = missions[i];
                    if (null == x || null == x.missionItem)
                    {
                        continue;
                    }

                    if (x.status != (int)Protocol.TaskStatus.TASK_FINISHED)
                    {
                        continue;
                    }

                    if (!(x.missionItem.TaskType == MissionTable.eTaskType.TT_MAIN ||
                    x.missionItem.TaskType == MissionTable.eTaskType.TT_CYCLE ||
                    x.missionItem.TaskType == MissionTable.eTaskType.TT_BRANCH))
                    {
                        continue;
                    }

                    bHasRedPoint = true;
                    break;
                }

                zeroFilter.SetRedPointOn(bHasRedPoint);
            }

            var zeroFilterTitle = m_akFilterZeros.GetObject(FilterZeroType.FZT_TITLE_TASK);
            if (zeroFilterTitle != null)
            {
                var missions = MissionManager.GetInstance().taskGroup.Values.ToList();
                bool bHasRedPoint = false;
                for (int i = 0; i < missions.Count; ++i)
                {
                    var x = missions[i];
                    if (null == x || null == x.missionItem)
                    {
                        continue;
                    }

                    if (x.status != (int)Protocol.TaskStatus.TASK_FINISHED)
                    {
                        continue;
                    }

                    if (!(x.missionItem.TaskType == MissionTable.eTaskType.TT_TITLE))
                    {
                        continue;
                    }

                    bHasRedPoint = true;
                    break;
                }

                zeroFilterTitle.SetRedPointOn(bHasRedPoint);
            }
        }

        void _CheckDailyMission(bool bCheck)
        {
            var firstFilter = m_akFilterFirsts.GetObject(FilterFirstType.FFT_DAILY);
            if (firstFilter != null)
            {
                bool bHasFinishedMission = bCheck;

                if (!bHasFinishedMission)
                {
                    var scoreObjects = m_akMissionScoreItems.ActiveObjects.Values.ToList();
                    for (int i = 0; i < scoreObjects.Count; ++i)
                    {
                        if (scoreObjects[i] != null && scoreObjects[i].CanAcquire)
                        {
                            bHasFinishedMission = true;
                            break;
                        }
                    }
                }
                firstFilter.SetRedPointOn(bHasFinishedMission);
            }
        }

        void _CheckAchievementMission(bool bCheck)
        {
            var filterAchievement = m_akFilterFirsts.GetObject(FilterFirstType.FFT_ACHIEVEMENT);
            if (null != filterAchievement)
            {
                var missions = MissionManager.GetInstance().taskGroup.Values.ToList();
                bool bHasRedPoint = false;
                for (int i = 0; i < missions.Count; ++i)
                {
                    var x = missions[i];
                    if (null == x || null == x.missionItem)
                    {
                        continue;
                    }

                    if (x.status != (int)Protocol.TaskStatus.TASK_FINISHED)
                    {
                        continue;
                    }

                    if (!(x.missionItem.TaskType == MissionTable.eTaskType.TT_ACHIEVEMENT &&
                x.missionItem.SubType == MissionTable.eSubType.Daily_Null))
                    {
                        continue;
                    }

                    bHasRedPoint = true;
                    break;
                }
                filterAchievement.SetRedPointOn(bHasRedPoint);
            }
        }

        protected override void _OnCloseFrame()
        {
            //m_kDailyMissionList.UnInitialize();
            m_kAchievementList.UnInitialize();
            m_kMainMissionList.UnInitialize();

            PlayerBaseData.GetInstance().onLevelChanged -= OnLevelChanged;
            PlayerBaseData.GetInstance().onMoneyChanged -= OnMoneyChanged;

            m_akFilterZeros.DestroyAllObjects();
            //
            m_akFilterFirsts.DestroyAllObjects();
            goContent = null;
            goMainDescribe = null;
            //
            goParent = null;
            goPrefab = null;
            goDescribe = null;
            kCurrentName = null;
            kCurrentIcon = null;
            kCurrentDesc = null;
            goAwardArray = null;
            goBtnGiveUp = null;
            goBtnComplete = null;
            goBtnTrace = null;
            goBtnAccept = null;
            goBtnAward = null;
            goBtnRefresh = null;
            comGray = null;
            m_kCycleOrgHint = null;
            m_goCostHint = null;
            m_kRefreshPre = null;
            m_kRefreshCount = null;
            m_kRefreshCoin = null;
            //
            m_akMissionScoreItems.DestroyAllObjects();
            goMissionScoreItemParent = null;
            goMissionScoreItemPrefabs = null;
            btnFastFinish = null;
            mFastFinishCount = null;
            mFastFinishIcon = null;
            skillComItem = null;

            finishedMissions = null;
        }
        #region uiEvent


        [UIEventHandle("Content/Describe/BtnGiveUp")]
        void OnGiveUpMission()
        {
            if (m_kCurrent != null &&
                m_kCurrent.missionItem.TaskType != ProtoTable.MissionTable.eTaskType.TT_ACHIEVEMENT &&
                m_kCurrent.missionItem.TaskType != ProtoTable.MissionTable.eTaskType.TT_DIALY)
            {
                MissionManager.GetInstance().sendCmdAbandomTask((uint)m_kCurrent.missionItem.ID);
            }
        }

        [UIEventHandle("Content/Describe/BtnComplete")]
        void OnCompleteMission()
        {
            if (m_kCurrent != null &&
                m_kCurrent.missionItem.TaskType != ProtoTable.MissionTable.eTaskType.TT_ACHIEVEMENT &&
                m_kCurrent.missionItem.TaskType != ProtoTable.MissionTable.eTaskType.TT_DIALY)
            {
                if( MissionManager.GetInstance().OnExecuteSubmitTask(m_kCurrent.missionItem.ID) )
                {
                    Close();
                }
            }
        }

        [UIEventHandle("Content/Describe/BtnAward")]
        void OnAward()
        {
            if (m_kCurrent != null &&
                (m_kCurrent.missionItem.TaskType == ProtoTable.MissionTable.eTaskType.TT_ACHIEVEMENT ||
                m_kCurrent.missionItem.TaskType == ProtoTable.MissionTable.eTaskType.TT_DIALY))
            {
                MissionManager.GetInstance().sendCmdSubmitTask((UInt32)m_kCurrent.missionItem.ID, Protocol.TaskSubmitType.TASK_SUBMIT_AUTO, 0);
            }
        }

        [UIEventHandle("Content/Describe/BtnTrace")]
        void OnTraceMission()
        {
            if (m_kCurrent != null &&
                m_kCurrent.missionItem.TaskType != ProtoTable.MissionTable.eTaskType.TT_ACHIEVEMENT &&
                m_kCurrent.missionItem.TaskType != ProtoTable.MissionTable.eTaskType.TT_DIALY)
            {
                if (m_kCurrent.missionItem.LinkParam == 1)
                {
                    frameMgr.CloseFrame(this, true);
                }
                MissionManager.GetInstance().AutoTraceTask(m_kCurrent.missionItem.ID);
            }
        }

        [UIEventHandle("Content/Describe/BtnAccept")]
        void OnAcceptMission()
        {
            if (m_kCurrent == null)
            {
                return;
            }

            if(!MissionManager.GetInstance().IsLevelFit(m_kCurrent.missionItem.ID))
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("mission_accept_need_level", m_kCurrent.missionItem.MinPlayerLv));
                return;
            }

            if (m_kCurrent.missionItem.TaskType != ProtoTable.MissionTable.eTaskType.TT_ACHIEVEMENT &&
                m_kCurrent.missionItem.TaskType != ProtoTable.MissionTable.eTaskType.TT_DIALY)
            {
                MissionManager.GetInstance().OnExecuteAcceptTask(m_kCurrent.missionItem.ID);
            }
            else if (m_kCurrent.missionItem.TaskType == ProtoTable.MissionTable.eTaskType.TT_ACHIEVEMENT)
            {
                MissionManager.GetInstance().sendCmdSubmitTask((UInt32)m_kCurrent.missionItem.ID, Protocol.TaskSubmitType.TASK_SUBMIT_AUTO, 0);
            }
            else
            {
                MissionManager.GetInstance().sendCmdSubmitTask((UInt32)m_kCurrent.missionItem.ID, Protocol.TaskSubmitType.TASK_SUBMIT_AUTO, 0);
            }
        }

        [UIEventHandle("Content/Describe/BtnFastFinish")]
        void OnFastFinishMission()
        {
            if (m_kCurrent == null)
            {
                return;
            }
            if (m_kCurrent.missionItem == null)
            {
                return;
            }
            if (!MissionManager.GetInstance().IsLevelFit(m_kCurrent.missionItem.ID))
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("mission_accept_need_level", m_kCurrent.missionItem.MinPlayerLv));
                return;
            }
            int moneyID = m_kCurrent.missionItem.FinishRightNowItemType;
            int price = m_kCurrent.missionItem.FinishRightNowItemNum;
            CostItemManager.GetInstance().TryCostMoneyDefault(new CostItemManager.CostInfo
            {
                nMoneyID = moneyID,
                nCount = price,
            },
            () =>
            {
            MissionManager.GetInstance().sendUnFinishTask((UInt32)m_kCurrent.missionItem.ID, Protocol.TaskSubmitType.TASK_SUBMIT_RIGHTNOW, 0);
            },
            "common_money_cost",
            null);            
        }
        [UIEventHandle("Content/Describe/BtnRefresh")]
        void OnRefresh()
        {
            if (m_kCurrent != null)
            {
                if (m_kCurrent.missionItem.TaskType == ProtoTable.MissionTable.eTaskType.TT_CYCLE)
                {
                    var SystemValueTableData = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_CYCLE_TASK_REFRESH_FREE_COUNT);
                    int iFreeTimes = SystemValueTableData.Value;
                    int iRefreshTimes = CountDataManager.GetInstance().GetCount(CounterKeys.COUNTER_CYCLE_REFRESH_TIMES);
                    if (iFreeTimes <= iRefreshTimes)
                    {
                        int iBindPointID = (int)ItemDataManager.GetInstance().GetMoneyIDByType(ProtoTable.ItemTable.eSubType.BindPOINT);
                        var item = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(iBindPointID);
                        if (item == null)
                        {
                            Logger.LogErrorFormat("bindpoint id is error can not find in table ItemTable!");
                            return;
                        }
                        int iOwnedCount = ItemDataManager.GetInstance().GetOwnedItemCount(iBindPointID,false);
                        SystemValueTableData = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_CYCLE_TASK_REFRESH_CONSUME);
                        int iNeedCost = 0;
                        if (SystemValueTableData != null)
                        {
                            iNeedCost = SystemValueTableData.Value;
                        }
                        else
                        {
                            Logger.LogErrorFormat("error! ProtoTable.SystemValueTable.eType.SVT_CYCLE_TASK_REFRESH_CONSUME can not find in table ProtoTable.SystemValueTable !!");
                            return;
                        }
                        if (iOwnedCount < iNeedCost)
                        {
                            CostItemManager.CostInfo costInfo = new CostItemManager.CostInfo();

                            costInfo.nMoneyID = iBindPointID;
                            costInfo.nCount = iNeedCost;
                            CostItemManager.GetInstance().TryCostMoneyDefault(costInfo,()=> { MissionManager.GetInstance().sendCmdRefreshTask(); });
                            //string value = string.Format(TR.Value("cycle_task_refresh_failed"), item.Name);
                            //SystemNotifyManager.SysNotifyTextAnimation(value);
                            
                            return;
                        }
                    }
                    MissionManager.GetInstance().sendCmdRefreshTask();
                }
            }
        }
        #endregion
        #region OnDeleteMission
        void OnLevelChanged(int iPre, int iCur)
        {
            m_akFilterFirsts.Filter(null);
        }
        private void OnMoneyChanged(PlayerBaseData.MoneyBinderType eTarget)
        {
            OnMissionSelected(m_kCurrent);
        }
        #endregion
        #region OnUpdateMission
        int _SortedItem(MissionManager.SingleMissionInfo left, MissionManager.SingleMissionInfo right)
        {
            if (left.status != right.status)
            {
                if (left.status == (int)Protocol.TaskStatus.TASK_OVER)
                {
                    return 1;
                }

                if (right.status == (int)Protocol.TaskStatus.TASK_OVER)
                {
                    return -1;
                }

                return (int)right.status - (int)left.status;
            }

            if (left.missionItem.SortID != right.missionItem.SortID)
            {
                return left.missionItem.SortID - right.missionItem.SortID;
            }

            if (left.taskID != right.taskID)
            {
                return left.taskID < right.taskID ? -1 : 1;
            }

            return 0;
        }
        #endregion

        #region MissionScoreData
        class MissionScoreItem : CachedObject
        {
            GameObject goLocal;
            GameObject goPrefab;
            GameObject goParent;
            MissionManager.MissionScoreData data;
            MissionFrameNew THIS;

            DG.Tweening.DOTweenAnimation tween;
            Text score;
            Image image;
            GameObject goEffect;
            Button button;
            MissionScoreRedBinder comBinder;

            public override void OnDestroy()
            {
                goLocal = null;
                goPrefab = null;
                goParent = null;
                data = null;
                THIS = null;
                score = null;
                image = null;

                if (button != null)
                {
                    button.onClick.RemoveAllListeners();
                    button = null;
                }
                comBinder = null;
                tween = null;
            }

            public override void OnCreate(object[] param)
            {
                goParent = param[0] as GameObject;
                goPrefab = param[1] as GameObject;
                data = param[2] as MissionManager.MissionScoreData;
                THIS = param[3] as MissionFrameNew;

                if (goLocal == null)
                {
                    goLocal = GameObject.Instantiate(goPrefab);
                    Utility.AttachTo(goLocal, goParent);

                    score = Utility.FindComponent<Text>(goLocal, "Text");
                    //image = goLocal.GetComponent<Image>();
                    image = Utility.FindComponent<Image>(goLocal, "Bg/Icon");
                    button = goLocal.GetComponent<Button>();
                    comBinder = Utility.FindComponent<MissionScoreRedBinder>(goLocal, "Bg/RedPoint");
                    goEffect = Utility.FindChild(goLocal, "Bg/Baoxiangbeijing");
                    tween = Utility.FindComponent<DG.Tweening.DOTweenAnimation>(goLocal, "Bg/Icon");

                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(_OnOpenChest);
                }
                Enable();
                _Update();
            }

            void _OnOpenChest()
            {
                MissionScoreAwardFrame.Open(data.missionScoreItem.ID);
            }

            public override void OnRecycle()
            {
                if (goLocal != null)
                {
                    goLocal.CustomActive(false);
                }
            }
            public override void OnDecycle(object[] param)
            {
                OnCreate(param);
            }
            public override void OnRefresh(object[] param)
            {
                if (param != null && param.Length > 0)
                {
                    data = param[0] as MissionManager.MissionScoreData;
                }
                _Update();
            }

            public override void SetAsLastSibling()
            {
                if (goLocal != null)
                {
                    goLocal.transform.SetAsLastSibling();
                }
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
            public override bool NeedFilter(object[] param)
            {
                return false;
            }

            public bool CanAcquire
            {
                get
                {
                    bool bCanAcquire = data.missionScoreItem.Score <= MissionManager.GetInstance().Score && !MissionManager.GetInstance().AcquiredChestIDs.Contains(data.missionScoreItem.ID);
                    return bCanAcquire;
                }
            }

            void _Update()
            {
                if (data != null)
                {
                    score.text = data.missionScoreItem.Name;
                    data.bOpen = data.missionScoreItem.Score <= MissionManager.GetInstance().Score && MissionManager.GetInstance().AcquiredChestIDs.Contains(data.missionScoreItem.ID);
                    // image.sprite = data.GetIcon();
                    data.GetIcon(ref image);
                    var bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(goParent.transform);
                    goLocal.transform.localPosition = new Vector3(1800.0f * data.fPostion, goLocal.transform.localPosition.y, 0.0f);
                    comBinder.LinkID = data.missionScoreItem.ID;
                    goEffect.CustomActive(data.missionScoreItem.Score <= MissionManager.GetInstance().Score && !MissionManager.GetInstance().AcquiredChestIDs.Contains(data.missionScoreItem.ID));
                    if (goEffect.activeSelf)
                    {
                        tween.DOPlay();
                    }
                    else
                    {
                        tween.DOPause();
                    }
                }
            }
        }
        CachedObjectDicManager<int, MissionScoreItem> m_akMissionScoreItems = new CachedObjectDicManager<int, MissionScoreItem>();
        GameObject goMissionScoreItemParent;
        GameObject goMissionScoreItemPrefabs;
        void _LoadMissionScoreItems()
        {
            if (goMissionScoreItemParent == null)
            {
                goMissionScoreItemParent = Utility.FindChild(frame,"DailyContent/ScoreBar/process/Boxes");
                goMissionScoreItemPrefabs = Utility.FindChild(frame,"DailyContent/ScoreBar/process/Boxes/Box");
                goMissionScoreItemPrefabs.CustomActive(false);
            }
            m_akMissionScoreItems.RecycleAllObject();
            var datas = MissionManager.GetInstance().MissionScoreDatas;
            for (int i = 0; i < datas.Count; ++i)
            {
                var current = m_akMissionScoreItems.Create(datas[i].missionScoreItem.ID, new object[] { goMissionScoreItemParent, goMissionScoreItemPrefabs, datas[i], this });
                if (current != null)
                {
                    current.SetAsLastSibling();
                }
            }
        }
        Text m_kScore;
        Slider m_kSlider;
        void _InitMissionScore()
        {
            m_kScore = Utility.FindComponent<Text>(frame,"DailyContent/ScoreBar/Score");
            m_kSlider = Utility.FindComponent<Slider>(frame,"DailyContent/ScoreBar/process/back");
        }
        void _UpdateMissionScore()
        {
            m_kScore.text = string.Format(TR.Value("mission_score"), MissionManager.GetInstance().Score);
            float fValue = MissionManager.GetInstance().Score * 1.0f / MissionManager.GetInstance().MaxScore;
            fValue = Mathf.Clamp01(fValue);
            m_kSlider.value = fValue;
        }
        void _UnInitMissionScore()
        {
            m_kScore = null;
        }

        void OnDailyScoreChanged(int score)
        {
            _UpdateMissionScore();
            m_akMissionScoreItems.RefreshAllObjects(null);

            _CheckDailyMission(m_kDailyMissionList.CheckRedPoint());
        }

        void OnChestIdsChanged()
        {
            _UpdateMissionScore();
            m_akMissionScoreItems.RefreshAllObjects(null);
            _CheckDailyMission(m_kDailyMissionList.CheckRedPoint());
        }
        #endregion
    }
}
