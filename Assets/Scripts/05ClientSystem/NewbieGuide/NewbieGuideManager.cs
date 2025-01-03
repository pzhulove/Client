using System.Collections.Generic;
using System;
using UnityEngine;
using Protocol;
using Network;
using ProtoTable;
///////删除linq

namespace GameClient
{
    // 新手引导所有序列化的内容都不要在中间插值

    public enum eNewbieGuideState
    {
        None,
        Guiding,
        Wait,
        Exception,
    }

    public enum eNewbieGuideAgrsName
    {
        None = 0,
        SaveBoot,
        PauseBattle,
        ResumeBattle,
        AutoClose,
    }

    public enum eNewbieGuideCondition
    {
        // 注意!!! 现在的新手引导的条件命名只能往最后加，不要中间插入枚举
        // 所有
        // 通用
        Level,
        MaxLevel,

        // 场景
        SceneLogin,
        SceneTown,
        ScenePkWaitingRoom,
        SceneBattle,
        BattleInitFinished,

        // 打开界面
        ClientSystemTownFrameOpen,
        ChapterChooseFrameOpen,
        QuickEquipFrameOpen,

        // 指定事件
        SpecificEvent,

        // 界面互斥
        MainFrameMutex,

        // 自定义条件
        EquipmentInPackage,
        AchievementFinished,
        SignInFinished,
        BranchMissionAccept,
        DailyMissionFinished,
        DungeonID,
        DungeonAreaID,
        DungeonStartTime,
        ChangeEquipmentExist,
        LearnBufferSkill,
        IsDungeon,
        NewbieGuideID,
        AddNewMissionID,
        FinishedMissionID,
        ChangedJob,
        FinishTalkDialog,
        HaveWhiteEquipment,
        MagicBoxGuide,
        SceneID,
        OnMission,
        ChapterChooseFrameOpenHaveParameter,

        UserDefine,
        // 注意!!! 现在的新手引导的条件命名只能往最后加，不要中间插入枚举
    }

    [LoggerModel("NewbieGuide")]
    public class NewbieGuideManager : Singleton<NewbieGuideManager>
    {
        private const string kNewbieGuideManagerTag = "NewbieGuideManager";

        private GameObject mNewbieGuideManagerObject = null;

        private eNewbieGuideState mState = eNewbieGuideState.None;
        public ComNewbieGuideControl mGuideControl = null;

        bool bStartGuide = false;
        float fGuideStateIntrval = 0.0f;

        struct EventBindData
        {
            public EventBindData(EUIEventID id, ClientEventSystem.UIEventHandler eventHandler)
            {
                this.eventid = id;
                this.eventHandler = eventHandler;
            }

            public EUIEventID eventid;
            public ClientEventSystem.UIEventHandler eventHandler;
        }

        List<EventBindData> bindEventDataList = new List<EventBindData>();

        private Dictionary<NewbieGuideTable.eNewbieGuideTask, NewbieGuideDataUnit> mForceGuideUnits = new Dictionary<NewbieGuideTable.eNewbieGuideTask, NewbieGuideDataUnit>();
        private Dictionary<NewbieGuideTable.eNewbieGuideTask, NewbieGuideDataUnit> mWeakGuideUnits = new Dictionary<NewbieGuideTable.eNewbieGuideTask, NewbieGuideDataUnit>();
        private Dictionary<NewbieGuideTable.eNewbieGuideTask, NewbieGuideDataUnit> mAloneGuideUnits = new Dictionary<NewbieGuideTable.eNewbieGuideTask, NewbieGuideDataUnit>();
        private Dictionary<NewbieGuideTable.eNewbieGuideTask, NewbieGuideDataUnit> mFinishDialogGuideUnits = new Dictionary<NewbieGuideTable.eNewbieGuideTask, NewbieGuideDataUnit>();
        private Dictionary<NewbieGuideTable.eNewbieGuideTask, NewbieGuideDataUnit> mAcceptMissionGuideUnits = new Dictionary<NewbieGuideTable.eNewbieGuideTask, NewbieGuideDataUnit>();
        private Dictionary<NewbieGuideTable.eNewbieGuideTask, NewbieGuideDataUnit> mFinishMissionGuideUnits = new Dictionary<NewbieGuideTable.eNewbieGuideTask, NewbieGuideDataUnit>();
        private Dictionary<NewbieGuideTable.eNewbieGuideTask, NewbieGuideDataUnit> mReceiveMissionRewardGuideUnits = new Dictionary<NewbieGuideTable.eNewbieGuideTask, NewbieGuideDataUnit>();

        public void Load()
        {
            Logger.LogProcessFormat("load");

            _loadEnv();
            _loadEvent();
        }

        public void Unload()
        {
            _unloadEnv();
            _unloadEvent();
            _clearData();
        }

        public void Reset()
        {
            _unloadEvent();
            _loadEvent();

            _clearData();
        }

        private void _loadEnv()
        {
            mNewbieGuideManagerObject = Utility.FindGameObject(kNewbieGuideManagerTag, false);
            if (mNewbieGuideManagerObject == null)
            {
                mNewbieGuideManagerObject = new GameObject(kNewbieGuideManagerTag);
                GameObject.DontDestroyOnLoad(mNewbieGuideManagerObject);
            }
        }

        private void _unloadEnv()
        {
            if (mNewbieGuideManagerObject != null)
            {
                GameObject.Destroy(mNewbieGuideManagerObject);
                mNewbieGuideManagerObject = null;
            }
        }

        private void _clearData()
        {
            bStartGuide = false;
            fGuideStateIntrval = 0.0f;
            mState = eNewbieGuideState.None;

            if (mGuideControl != null)
            {
                mGuideControl.ClearData();
            }

            if (mForceGuideUnits != null)
            {
                mForceGuideUnits.Clear();
            }

            if (mWeakGuideUnits != null)
            {
                mWeakGuideUnits.Clear();
            }

            if (mAloneGuideUnits != null)
            {
                mAloneGuideUnits.Clear();
            }

            if(mFinishDialogGuideUnits != null)
            {
                mFinishDialogGuideUnits.Clear();
            }

            if(mAcceptMissionGuideUnits != null)
            {
                mAcceptMissionGuideUnits.Clear();
            }

            if(mFinishMissionGuideUnits != null)
            {
                mFinishMissionGuideUnits.Clear();
            }

            if(mReceiveMissionRewardGuideUnits != null)
            {
                mReceiveMissionRewardGuideUnits.Clear();
            }
        }

        private void _loadEvent()
        {
            // 初始化进入城镇时的检测
            _rigsterEventHanble(EUIEventID.InitNewbieGuideBootData, OnInitGuideBootData);

            // 正常引导流程的检测
            _rigsterEventHanble(EUIEventID.InitializeTownSystem, CheckAll);
            _rigsterEventHanble(EUIEventID.SystemLoadingCompelete, CheckAll);
            _rigsterEventHanble(EUIEventID.SystemChanged, CheckAll);
            _rigsterEventHanble(EUIEventID.SceneChangedFinish, CheckAll);
            _rigsterEventHanble(EUIEventID.BattleInitFinished, CheckAll);
            _rigsterEventHanble(EUIEventID.LevelChanged, CheckAll);      
            _rigsterEventHanble(EUIEventID.DungeonOnFight, CheckAll);                
            _rigsterEventHanble(EUIEventID.FrameClose, CheckAll);
            _rigsterEventHanble(EUIEventID.FadeInOver, CheckAll);
            _rigsterEventHanble(EUIEventID.CheckAllNewbieGuide,CheckAll);
            //_rigsterEventHanble(EUIEventID.EndNewbieGuideCover, CheckAll);
            _rigsterEventHanble(EUIEventID.FrameOpen, CheckAll);

            // 子类引导的检测
            _rigsterEventHanble(EUIEventID.FinishTalkDialog, OnCheckFinishTalkDialogGuide);
            MissionManager.GetInstance().onUpdateMission += OnCheckAddNewMission; // 必须是手动接取的任务，自动接取和手动接取的任务都会先走onAddNewMission的回调，但是手动接取的任务不会影响新手引导的触发时机
            MissionManager.GetInstance().onDeleteMission += OnCheckFinishMission; 
            //_rigsterEventHanble(EUIEventID.MissionRewardFrameClose, OnCheckAcceptMissionReward);
            _rigsterEventHanble(EUIEventID.LevelChanged, OnCheckAcceptMissionReward);    
            
            // 独立引导的检测 
            _rigsterEventHanble(EUIEventID.GuankaFrameOpen, OnCheckGunakaGuide);
            _rigsterEventHanble(EUIEventID.DungeonRewardFrameOpen, OnCheckDungeonRewardGuide);
            _rigsterEventHanble(EUIEventID.DungeonRewardFrameClose, OnCheckReturnToTownGuide);
            //_rigsterEventHanble(EUIEventID.HpChanged, OnCheckDrugGuide);
            _rigsterEventHanble(EUIEventID.ChangeJobFinishFrameOpen, OnCheckChangeJobSkillGuide); 
            _rigsterEventHanble(EUIEventID.ChangeJobSelectFrameOpen, OnCheckChangeJobSelectGuide);

            // 其他功能的检测
            _rigsterEventHanble(EUIEventID.TaskDialogFrameOpen, CheckPauseState);
            _rigsterEventHanble(EUIEventID.FrameClose, CheckPauseState);

            for (int i = 0; i < bindEventDataList.Count; ++i)
            {
                 var cur = bindEventDataList[i];
                 GlobalEventSystem.GetInstance().RegisterEventHandler(cur.eventid, cur.eventHandler);       
            }         
        }

        private void _unloadEvent()
        {
            for(int i = 0; i < bindEventDataList.Count; ++i)
            {
                 var cur = bindEventDataList[i];
                 GlobalEventSystem.GetInstance().UnRegisterEventHandler(cur.eventid, cur.eventHandler);       
            }

            bindEventDataList.Clear();

            MissionManager.GetInstance().onAddNewMission -= OnCheckAddNewMission;
            MissionManager.GetInstance().onDeleteMission -= OnCheckFinishMission;
        }

        private void _rigsterEventHanble(EUIEventID id, ClientEventSystem.UIEventHandler eventHandler)
        {
            bindEventDataList.Add(new EventBindData(id, eventHandler));
        }

        private void _loadGuide()
        {
            List<int> TableNewbieGuideOrderList = TableManager.GetInstance().GetNewbieGuideOrderList();
            if (TableNewbieGuideOrderList == null)
            {
                SystemNotifyManager.SysNotifyMsgBoxOK("新手引导表格获取引导顺序为空");
                return;
            }

            for (int i = 0; i < TableNewbieGuideOrderList.Count; i++)
            {
                _loadOne(TableNewbieGuideOrderList[i]);
            }
        }

        private void _loadOne(int taskID)
        {
            NewbieGuideTable tabledata = TableManager.GetInstance().GetTableItem<NewbieGuideTable>(taskID);
            if (tabledata == null)
            {
                return;
            }

            if(CheckHasGuided(tabledata))
            {
                return;
            }

            var unit = NewbieGuideDataManager.instance.GetData(tabledata,(NewbieGuideTable.eNewbieGuideTask)taskID);
            if (unit == null)
            {
                return;
            }

            unit.manager = this;
            unit.guideType = tabledata.NewbieGuideType;

            if(tabledata.GuideSubType == NewbieGuideTable.eGuideSubType.GST_ALONE)
            {
                mAloneGuideUnits.Add((NewbieGuideTable.eNewbieGuideTask)taskID, unit);
            }
            else if(tabledata.GuideSubType == NewbieGuideTable.eGuideSubType.GST_FINISH_TALK_DIALOG)
            {
                mFinishDialogGuideUnits.Add((NewbieGuideTable.eNewbieGuideTask)taskID, unit);
            }
            else if(tabledata.GuideSubType == NewbieGuideTable.eGuideSubType.GST_ACCEPT_MISSION)
            {
                mAcceptMissionGuideUnits.Add((NewbieGuideTable.eNewbieGuideTask)taskID, unit);
            }
            else if(tabledata.GuideSubType == NewbieGuideTable.eGuideSubType.GST_FINISH_MISSION)
            {
                mFinishMissionGuideUnits.Add((NewbieGuideTable.eNewbieGuideTask)taskID, unit);
            }
            else if(tabledata.GuideSubType == NewbieGuideTable.eGuideSubType.GST_RECEIVE_MISSION_AWARD)
            {
                mReceiveMissionRewardGuideUnits.Add((NewbieGuideTable.eNewbieGuideTask)taskID, unit);
            }
            else
            {
                if(tabledata.NewbieGuideType == NewbieGuideTable.eNewbieGuideType.NGT_FORCE)
                {
                    mForceGuideUnits.Add((NewbieGuideTable.eNewbieGuideTask)taskID, unit);
                }
                else
                {
                    mWeakGuideUnits.Add((NewbieGuideTable.eNewbieGuideTask)taskID, unit);
                }
            }          
        }

        void OnInitGuideBootData(UIEvent uiEvent)
        {
            _clearData();
            _loadGuide();

            CheckAll(uiEvent);
        }

        void CheckAll(UIEvent uiEvent)
        {
            if (!CheckAllGuideCondition(uiEvent))
            {
                return;
            }

            if (TryForceGuide(uiEvent))
            {
                return;
            }

            TryWeakGuide(uiEvent);
        }

        void OnCheckFinishTalkDialogGuide(UIEvent uiEvent)
        {
            if (!CheckAllGuideCondition(uiEvent))
            {
                return;
            }

            TryFinishTalkDialogGuide(uiEvent);
        }

        void OnCheckAddNewMission(UInt32 iTaskID)
        {
            if (iTaskID == 2352)
            {
                int kkkkk = 0;
            }

            if (!MissionManager.GetInstance().IsAcceptMission(iTaskID))
            {
                return;
            }
            
            UIEvent uiEvent = new UIEvent();

            uiEvent.EventID = EUIEventID.AddNewMission;
            uiEvent.Param1 = (int)iTaskID;

            if (!CheckAllGuideCondition(uiEvent))
            {
                return;
            }

            TryAddNewMissionGuide(uiEvent);
        }

        void OnCheckFinishMission(UInt32 iTaskID)
        {
            UIEvent uiEvent = new UIEvent();

            uiEvent.EventID = EUIEventID.MissionRewardFrameClose;
            uiEvent.Param1 = (int)iTaskID;

            OnCheckAcceptMissionReward(uiEvent);
        }

        void OnCheckAcceptMissionReward(UIEvent uiEvent)
        {
            if (!CheckAllGuideCondition(uiEvent))
            {
                return;
            }

            if(uiEvent.EventID == EUIEventID.MissionRewardFrameClose)
            {
                PlayerBaseData.GetInstance().GuideFinishMission = (int)uiEvent.Param1;
            }

            TryReceiveMissionRewardGuide(uiEvent);
        }

        void OnCheckGunakaGuide(UIEvent uiEvent)
        {
            NewbieGuideTable tabledata = TableManager.GetInstance().GetTableItem<NewbieGuideTable>((int)NewbieGuideTable.eNewbieGuideTask.GuanKaGuide);
            if (tabledata != null)
            {
                TryAloneGuide(tabledata, uiEvent);
            }

            NewbieGuideTable tabledata2 = TableManager.GetInstance().GetTableItem<NewbieGuideTable>((int)NewbieGuideTable.eNewbieGuideTask.ExchangeShopGuide);
            if (tabledata2 != null)
            {
                TryAloneGuide(tabledata2, uiEvent);
            }

            NewbieGuideTable tabledata3 = TableManager.GetInstance().GetTableItem<NewbieGuideTable>((int)NewbieGuideTable.eNewbieGuideTask.YiJieOpenGuide);
            if (tabledata3 != null)
            {
                TryAloneGuide(tabledata3, uiEvent);
            }
        }

        void OnCheckDungeonRewardGuide(UIEvent uiEvent)
        {
            NewbieGuideTable tabledata = TableManager.GetInstance().GetTableItem<NewbieGuideTable>((int)NewbieGuideTable.eNewbieGuideTask.DungeonRewardGuide);
            if (tabledata == null)
            {
                return;
            }

            TryAloneGuide(tabledata, uiEvent);
        }

        void OnCheckReturnToTownGuide(UIEvent uiEvent)
        {
            NewbieGuideTable tabledata = TableManager.GetInstance().GetTableItem<NewbieGuideTable>((int)NewbieGuideTable.eNewbieGuideTask.ReturnToTownGuide);
            if (tabledata == null)
            {
                return;
            }

            TryAloneGuide(tabledata, uiEvent);
        }

        void OnCheckDrugGuide(UIEvent uiEvent)
        {
            NewbieGuideTable tabledata = TableManager.GetInstance().GetTableItem<NewbieGuideTable>((int)NewbieGuideTable.eNewbieGuideTask.DrugGuide);
            if (tabledata == null)
            {
                return;
            }

            TryAloneGuide(tabledata, uiEvent);
        }

        void OnCheckChangeJobSkillGuide(UIEvent uiEvent)
        {
            NewbieGuideTable tabledata = TableManager.GetInstance().GetTableItem<NewbieGuideTable>((int)NewbieGuideTable.eNewbieGuideTask.ChangeJobSkillGuide);
            if (tabledata == null)
            {
                return;
            }

            TryAloneGuide(tabledata, uiEvent);
        }

        void OnCheckChangeJobSelectGuide(UIEvent uiEvent)
        {
            NewbieGuideTable tabledata = TableManager.GetInstance().GetTableItem<NewbieGuideTable>((int)NewbieGuideTable.eNewbieGuideTask.ChangeJobChoiceGuide);
            if (tabledata == null)
            {
                return;
            }

            TryAloneGuide(tabledata, uiEvent);
        }

        void CheckPauseState(UIEvent uiEvent)
        {
            if (mState < eNewbieGuideState.Guiding)
            {
                return;
            }

            if (uiEvent.EventID == EUIEventID.TaskDialogFrameOpen)
            {
                SetPauseState(true);
            }
            else if (uiEvent.EventID == EUIEventID.FrameClose && uiEvent.Param1 as string == "TaskDialogFrame")
            {
                SetPauseState(false);
            }
        }

        //string errorString = "";
        bool CheckAllGuideCondition(UIEvent uiEvent)
        {          
            if(!_CheckBaseCondition(uiEvent))
            {
                return false;
            }

            ClientSystemTown systemTown = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;

            if (systemTown != null)
            {
//                 if (PlayerBaseData.GetInstance().bLevelUpChange == true)
//                 {
//                     //errorString = "bLevelUpChange"; 
//                     return false;
//                 }

                CitySceneTable scenedata = TableManager.GetInstance().GetTableItem<CitySceneTable>(systemTown.CurrentSceneID);
                if (scenedata != null)
                {
                    ClientSystemTownFrame Maintownframe = ClientSystemManager.GetInstance().GetFrame(typeof(ClientSystemTownFrame)) as ClientSystemTownFrame;

                    if (Maintownframe == null)
                    {
                        //errorString = "ClientSystemTownFrame is null";
                        return false;
                    }

                    if (scenedata.SceneType != CitySceneTable.eSceneType.PK_PREPARE && Maintownframe.GetState() > EFrameState.Open)
                    {
                        if (uiEvent.EventID == EUIEventID.FrameClose)
                        {
                            string str = uiEvent.Param1 as string;

                            if (str != "TaskDialogFrame")
                            {
                                return false;
                            }
                        }
                        else if (uiEvent.EventID != EUIEventID.MissionRewardFrameClose && uiEvent.EventID != EUIEventID.FinishTalkDialog)
                        {
                            //errorString = " ClientSystemTownFrame not open";
                            return false;
                        }             
                    }
                }
            }

            if(uiEvent.EventID == EUIEventID.FrameClose)
            {
                string str = uiEvent.Param1 as string;
                if (str == "" || str == "WaitNetMessageFrame" || str == "FadingFrame")
                {
                    //errorString = str + " - Normal Frame Close";
                    return false;
                }
            }

            if (ClientSystemManager.GetInstance().IsFrameOpen<LevelUpNotify>())
            {
                //errorString = "FunctionUnlockFrame NextOpenShowFrame LevelUpNotify";
                return false;
            }

            if (ClientSystemManager.GetInstance().IsFrameOpen<ChapterSelectFrame>())
            {
                if (GetNextGuide(NewbieGuideTable.eNewbieGuideType.NGT_FORCE) != NewbieGuideTable.eNewbieGuideTask.GuanKaGuide)
                {
                    //errorString = "ChapterSelectFrame open";
                    return false;
                }
            }

            if (mState == eNewbieGuideState.Wait)
            {
                if (mGuideControl == null)
                {
                    SetGuideState(eNewbieGuideState.None);
                }
                else
                {
                    if (mGuideControl.curState == ComNewbieGuideControl.ControlState.Finish)
                    {
                        SetGuideState(eNewbieGuideState.None);
                    }
                    else
                    {
                        NewbieGuideTable data = TableManager.GetInstance().GetTableItem<NewbieGuideTable>((int)mGuideControl.GuideTaskID);
                        if (data == null)
                        {
                            //errorString = "NewbieGuideTable is null";
                            SetGuideState(eNewbieGuideState.None);
                            return false;
                        }

                        if(CheckHasGuided(data))
                        {
                            ManagerFinishGuide(mGuideControl.GuideTaskID);
                            return false;
                        }

                        if(mGuideControl.GetCurGuideCom() == null)
                        {
                            ManagerFinishGuide(mGuideControl.GuideTaskID);
                            return false;
                        }

                        if (CanGuide(data, data.NewbieGuideType, uiEvent))
                        {
                            return DoGuide(data);
                        }

                        //errorString = "Can not Guide";
                        return false;
                    }
                }
            }
            else if (mState == eNewbieGuideState.Guiding)
            {
                CheckCurCoverGuide(uiEvent);

                if (uiEvent.EventID == EUIEventID.SystemChanged && mGuideControl != null && 
                    mGuideControl.curState != ComNewbieGuideControl.ControlState.Finish)
                {
                    SetGuideState(eNewbieGuideState.None);
                    return true;
                }

                 //errorString = "eNewbieGuideState.Guiding";
                return false;
            }

            return true;
        }

        bool _CheckBaseCondition(UIEvent uiEvent)
        {
            if (!Global.Settings.isGuide)
            {
                //errorString = "Guide is close";
                return false;
            }

            // 如果是使用秘药飞升升级的，那么就不弹各种升级表现了
            if (PlayerBaseData.GetInstance().IsFlyUpState)
            {
                //mState = eNewbieGuideState.None;
                return false;
            }

            RoleInfo roleInfo = ClientApplication.playerinfo.GetSelectRoleBaseInfoByLogin();
            //Logger.LogErrorFormat("111111111111111111{0}", roleInfo.playerLabelInfo.noviceGuideChooseFlag);
            //Logger.LogErrorFormat("222222222222222222{0}", NewbieGuideDataManager.GetInstance().GetRoleNewbieguideState((int)roleInfo.roleId));
            if (roleInfo != null)
            {
                if (roleInfo.playerLabelInfo.noviceGuideChooseFlag == (int)NoviceGuideChooseFlag.NGCF_PASS 
                    || NewbieGuideDataManager.GetInstance().GetRoleNewbieguideState((int)roleInfo.roleId) == NoviceGuideChooseFlag.NGCF_PASS)
                {
                    return false;
                }
            }
            

            if (mNewbieGuideManagerObject == null)
            {
                //errorString = "mNewbieGuideManagerObject is null";
                return false;
            }

            if (!PlayerBaseData.GetInstance().bInitializeTownSystem)
            {
                //errorString = "bInitializeTownSystem is false";
                return false;
            }

            if (BattleMain.IsModePvP(BattleMain.battleType) || BattleMain.IsModeMultiplayer(BattleMain.mode))
            {
                //errorString = "IsModePvP || IsModeMultiplayer";
                return false;
            }

            if(IsGuiding())
            {
                return false;
            }

            if (ClientSystemManager.GetInstance().IsFrameOpen<TaskDialogFrame>())
            {
                if (uiEvent.EventID == EUIEventID.FrameClose)
                {
                    string str = uiEvent.Param1 as string;

                    if (str != "TaskDialogFrame")
                    {
                        return false;
                    }
                }
                else if (uiEvent.EventID != EUIEventID.MissionRewardFrameClose && uiEvent.EventID != EUIEventID.FinishTalkDialog)
                {
                    return false;
                }
            }

            return true;
        }

        bool TryForceGuide(UIEvent uiEvent)
        {
            IEnumerator<NewbieGuideTable.eNewbieGuideTask> iter = mForceGuideUnits.Keys.GetEnumerator();
            return FindGuide(iter, uiEvent);
        }

        bool TryWeakGuide(UIEvent uiEvent)
        {
            IEnumerator<NewbieGuideTable.eNewbieGuideTask> iter = mWeakGuideUnits.Keys.GetEnumerator();
            return FindGuide(iter, uiEvent);
        }

        bool TryFinishTalkDialogGuide(UIEvent uiEvent)
        {
            IEnumerator<NewbieGuideTable.eNewbieGuideTask> iter = mFinishDialogGuideUnits.Keys.GetEnumerator();
            return FindGuide(iter, uiEvent);
        }

        bool TryAddNewMissionGuide(UIEvent uiEvent)
        {
            IEnumerator<NewbieGuideTable.eNewbieGuideTask> iter = mAcceptMissionGuideUnits.Keys.GetEnumerator();
            return FindGuide(iter, uiEvent);
        }

        bool TryFinishMissionGuide(UIEvent uiEvent)
        {
            IEnumerator<NewbieGuideTable.eNewbieGuideTask> iter = mFinishMissionGuideUnits.Keys.GetEnumerator();
            return FindGuide(iter, uiEvent);
        }

        bool TryReceiveMissionRewardGuide(UIEvent uiEvent)
        {
            IEnumerator<NewbieGuideTable.eNewbieGuideTask> iter = mReceiveMissionRewardGuideUnits.Keys.GetEnumerator();
            return FindGuide(iter, uiEvent);
        }

        bool TryAloneGuide(NewbieGuideTable tabledata, UIEvent uiEvent)
        {
            if (CheckHasGuided(tabledata))
            {
                return false;
            }

            if (!_CheckBaseCondition(uiEvent))
            {
                return false;
            }

            if (CanGuide(tabledata, tabledata.NewbieGuideType, uiEvent))
            {
                DoGuide(tabledata);
            }

            return false;
        }

        private bool FindGuide(IEnumerator<NewbieGuideTable.eNewbieGuideTask> iter, UIEvent uiEvent)
        {
            while (iter.MoveNext())
            {
                var key = iter.Current;

                NewbieGuideTable tabledata = TableManager.GetInstance().GetTableItem<NewbieGuideTable>((int)key);
                if (tabledata == null)
                {
                    continue;
                }



                if (CheckHasGuided(tabledata))
                {
                    continue;
                }

                if (CanGuide(tabledata, tabledata.NewbieGuideType, uiEvent))
                {
                    return DoGuide(tabledata);
                }
            }

            return false;
        }

        private bool CanGuide(NewbieGuideTable tabledata, NewbieGuideTable.eNewbieGuideType eGuideType, UIEvent uiEvent)
        {
            var unit = GetUnit(tabledata);
            if (unit == null)
            {
                return false;
            }

            if (unit.guideType != eGuideType)
            {
                return false;
            }
#if APPLE_STORE
            //add by mjx for IOS AppStore
            if (tabledata.NewbieGuideTask == NewbieGuideTable.eNewbieGuideTask.WelfareGuide)
            {
                if (IOSFunctionSwitchManager.GetInstance().IsFunctionClosed(ProtoTable.IOSFuncSwitchTable.eType.SEVEN_AWARDS))
                {
                    return false;
                }
            }
#endif
            return unit.CheckAllCondition(uiEvent);
        }

        private bool DoGuide(NewbieGuideTable tabledata)
        {
            var unit = GetUnit(tabledata);

            if (unit == null)
            {
                return false;
            }

            //临时代码 测试
            ClientSystemManager.instance.CloseFrame<TeamListFrame>();

            if (tabledata.ID != (int)NewbieGuideTable.eNewbieGuideTask.GuanKaGuide && tabledata.ID != (int)NewbieGuideTable.eNewbieGuideTask.AutoTraceGuide2)
            {
                BeTownPlayerMain.CommandStopAutoMove();

                Logger.LogWarningFormat("新手引导{0}触发--停止自动寻路", (NewbieGuideTable.eNewbieGuideTask)tabledata.ID);
            }

            if (tabledata.ID == (int)NewbieGuideTable.eNewbieGuideTask.MagicPotGuide)
            {
                ClientSystemTownFrame Townframe = ClientSystemManager.GetInstance().GetFrame(typeof(ClientSystemTownFrame)) as ClientSystemTownFrame;
                if (Townframe != null)
                {
                    Townframe.ExtendTopRightBtn();
                }
            }
           
            SetGuideState(eNewbieGuideState.Guiding);
            bStartGuide = true;
            fGuideStateIntrval = 0.0f;

            if (tabledata.CloseFrames == true)
            {
                ClientSystemManager.instance.ForceClearFrame("");
            }

            _deleteCurGuideControl();
            _AddCurGuideControl((NewbieGuideTable.eNewbieGuideTask)tabledata.ID, unit);

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.CurGuideStart, (NewbieGuideTable.eNewbieGuideTask)tabledata.ID);

            return true;
        }

        void _AddCurGuideControl(NewbieGuideTable.eNewbieGuideTask task, NewbieGuideDataUnit unit)
        {
            ComNewbieGuideControl com = null;

            if (unit.guideType == NewbieGuideTable.eNewbieGuideType.NGT_FORCE)
            {
                com = mNewbieGuideManagerObject.AddComponent<ComNewbieGuideControl>();
            }
            else
            {
                com = mNewbieGuideManagerObject.AddComponent<ComNewbieGuideWeakControl>();
            }

            com.guideManager = this;
            com.GuideTaskID = task;
            com.SetUnit(unit);

            mGuideControl = com;
        }

        void _deleteCurGuideControl()
        {
            if (null != mGuideControl)
            {
                mGuideControl.FinishCurGuideControl();
                GameObject.Destroy(mGuideControl);
                mGuideControl = null;
            }
        }

        private void CheckCurCoverGuide(UIEvent uiEvent)
        {
            if (mGuideControl == null)
            {
                return;
            }

            NewbieGuideDataUnit ControlGuideUnit = mGuideControl.GetControlUnit();

            if (ControlGuideUnit == null || ControlGuideUnit.newbieComList == null)
            {
                return;
            }

            int iIndex = mGuideControl.currentIndex;
            if (iIndex < 0 || iIndex >= ControlGuideUnit.newbieComList.Count)
            {
                return;
            }

            if (ControlGuideUnit.newbieComList[iIndex].ComType != NewbieGuideComType.COVER)
            {
                return;
            }

            if (ControlGuideUnit.newbieComList[iIndex].args == null || ControlGuideUnit.newbieComList[iIndex].args.Length < 1)
            {
                return;
            }

            if ((EUIEventID)ControlGuideUnit.newbieComList[iIndex].args[0] != uiEvent.EventID)
            {
                return;
            }

            mGuideControl.ControlComplete();
        }

        public void ManagerFinishGuide(NewbieGuideTable.eNewbieGuideTask task)
        {
            NewbieGuideTable tabledata = TableManager.GetInstance().GetTableItem<NewbieGuideTable>((int)task);
            if (tabledata == null)
            {
                return;
            }

            RemoveUnit(tabledata);
            _deleteCurGuideControl();

            if (!CheckHasGuided(tabledata))
            {
                SendSaveBoot(task);
            }

            bStartGuide = false;
            fGuideStateIntrval = 0.0f;

            SetGuideState(eNewbieGuideState.None);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.CurGuideFinish, task);

            //CheckAll(uiEvent);
            //TryGuide(NewbieGuideType.ALL, uiEvent);
        }

        bool CheckHasGuided(NewbieGuideTable tabledata)
        {
            if (tabledata.IsOpen == 0)
            {
                return true;
            }

            if (tabledata.NewbieGuideType == NewbieGuideTable.eNewbieGuideType.NGT_FORCE)
            {
                if (tabledata.Order <= PlayerBaseData.GetInstance().NewbieGuideCurSaveOrder)
                {
                    return true;
                }
            }
            else
            {
                for (int i = 0; i < PlayerBaseData.GetInstance().NewbieGuideWeakGuideList.Count; i++)
                {
                    if (PlayerBaseData.GetInstance().NewbieGuideWeakGuideList[i] == tabledata.ID)
                    {
                        return true;
                    }
                }
            }

            return false;       
        }

        public void SendSaveBoot(NewbieGuideTable.eNewbieGuideTask task)
        {
            //return;

            NewbieGuideTable data = TableManager.GetInstance().GetTableItem<NewbieGuideTable>((int)task);
            if(data == null)
            {
                return;
            }

            NetManager netMgr = NetManager.Instance();

            if (data.NewbieGuideType == NewbieGuideTable.eNewbieGuideType.NGT_FORCE)
            {
                SceneNotifyNewBoot req = new SceneNotifyNewBoot();
                req.id = (UInt32)task;

                netMgr.SendCommand(ServerType.GATE_SERVER, req);
            }
            else
            {
                SceneNotifyBootFlag req = new SceneNotifyBootFlag();
                req.id = (UInt32)task;

                netMgr.SendCommand(ServerType.GATE_SERVER, req);
            }
        }

        public void OnUpdate(float timeElapsed)
        {
//             if(bStartGuide)
//             {
//                 fGuideStateIntrval += timeElapsed;
// 
//                 if (fGuideStateIntrval > 180.0f)
//                 {
//                     bStartGuide = false;
//                     fGuideStateIntrval = 0.0f;
// 
//                     if (mGuideControl != null)
//                     {
//                         ManagerFinishGuide(mGuideControl.GuideTaskID);
//                     }
// 
//                     mState = eNewbieGuideState.None;
//                 }
//             }
         
        }

        public void SetGuideState(eNewbieGuideState estate)
        {
            mState = estate;
        }

        public void SetPauseState(bool bPause)
        {
            if (mGuideControl == null)
            {
                return;
            }

            if (GetCurGuideType() == NewbieGuideComType.TALK_DIALOG)
            {
                return;
            }

            ComNewbieGuideBase ComGuideBase = mGuideControl.GetCurGuideCom();
            if (ComGuideBase == null)
            {
                return;
            }

            if (bPause)
            {
                ComGuideBase.SetShow(false);
            }
            else
            {
                ComGuideBase.SetShow(true);
            }
        }

        public bool IsGuidingControl()
        {
            if (mGuideControl == null)
            {
                return false;
            }         

            return (mState == eNewbieGuideState.Guiding);
        }
        public bool IsHavingWhiteGuide()
        {
            //var itemids = ItemDataManager.GetInstance().GetItemsByType(ProtoTable.ItemTable.eType.EQUIP);
            var itemids = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.Equip);
            if (itemids == null)
            {
                Logger.LogErrorFormat("itemids is null");
                return false;
            }
            for(int i=0;i<itemids.Count;i++)
            {
                if(IsWhiteItem(itemids[i]))
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsWhiteItem(ulong id)
        {
            ItemData itemData = ItemDataManager.GetInstance().GetItem(id);
            if (itemData == null) return false;
            if(itemData.Quality== ProtoTable.ItemTable.eColor.WHITE)
            {
                return true;
            }
            return false;
        }
        public bool IsGuiding()
        {
            if (mGuideControl == null)
            {
                return false;
            }

            if (mGuideControl.GetCurGuideCom() == null)
            {
                return false;
            }

            return (mState == eNewbieGuideState.Guiding);
        }

        public bool IsGuidingTask(NewbieGuideTable.eNewbieGuideTask eTask)
        {
            if (mGuideControl == null)
            {
                return false;
            }

            if (mGuideControl.GuideTaskID != eTask)
            {
                return false;
            }

            if(mGuideControl.GetCurGuideCom() == null)
            {
                return false;
            }

            return true;
        }

        public bool IsGuidingTaskByCondition(eNewbieGuideCondition eCondition)
        {
            if (mGuideControl == null)
            {
                return false;
            }

            if (mState < eNewbieGuideState.Guiding)
            {
                return false;
            }

            if (mGuideControl.GuideTaskID <= 0)
            {
                return false;
            }

            NewbieGuideDataUnit unit = mGuideControl.GetControlUnit();

            if (unit == null)
            {
                return false;
            }

            if (unit.newbieConditionList == null || unit.newbieConditionList.Count <= 0)
            {
                return false;
            }

            for (int i = 0; i < unit.newbieConditionList.Count; i++)
            {
                if (unit.newbieConditionList[i].condition == eCondition)
                {
                    return true;
                }
            }

            return false;
        }

        public NewbieGuideTable.eNewbieGuideTask GetNextGuide(NewbieGuideTable.eNewbieGuideType eGuideType = NewbieGuideTable.eNewbieGuideType.NGT_FORCE)
        {
            List<int> TableNewbieGuideOrderList = TableManager.GetInstance().GetNewbieGuideOrderList();

            if (TableNewbieGuideOrderList == null || TableNewbieGuideOrderList.Count <= 0)
            {
                return NewbieGuideTable.eNewbieGuideTask.None;
            }

            if (eGuideType == NewbieGuideTable.eNewbieGuideType.NGT_FORCE)
            {
                bool bFindSavePoint = false;

                for (int i = 0; i < TableNewbieGuideOrderList.Count; i++)
                {
                    var Tabledata = TableManager.GetInstance().GetTableItem<NewbieGuideTable>(TableNewbieGuideOrderList[i]);

                    if (Tabledata == null)
                    {
                        continue;
                    }

                    if (Tabledata.NewbieGuideType != NewbieGuideTable.eNewbieGuideType.NGT_FORCE)
                    {
                        continue;
                    }

                    if (TableNewbieGuideOrderList[i] == PlayerBaseData.GetInstance().NewbieGuideSaveBoot)
                    {
                        bFindSavePoint = true;

                        for (int j = i + 1; j < TableNewbieGuideOrderList.Count; j++)
                        {
                            var Forcedata = TableManager.GetInstance().GetTableItem<NewbieGuideTable>(TableNewbieGuideOrderList[j]);

                            if (Forcedata == null || Forcedata.NewbieGuideType != NewbieGuideTable.eNewbieGuideType.NGT_FORCE)
                            {
                                continue;
                            }

                            return (NewbieGuideTable.eNewbieGuideTask)TableNewbieGuideOrderList[j];
                        }

                        return NewbieGuideTable.eNewbieGuideTask.None;
                    }
                }

                if(!bFindSavePoint)
                {
                    return (NewbieGuideTable.eNewbieGuideTask)TableNewbieGuideOrderList[0];
                }
            }
            else
            {
                for (int i = 0; i < TableNewbieGuideOrderList.Count; i++)
                {
                    var Tabledata = TableManager.GetInstance().GetTableItem<NewbieGuideTable>(TableNewbieGuideOrderList[i]);

                    if (Tabledata == null)
                    {
                        continue;
                    }

                    if (Tabledata.NewbieGuideType != NewbieGuideTable.eNewbieGuideType.NGT_WEAK)
                    {
                        continue;
                    }

                    bool bFind = false;
                    for (int j = 0; j < PlayerBaseData.GetInstance().NewbieGuideWeakGuideList.Count; j++)
                    {
                        if (TableNewbieGuideOrderList[i] == PlayerBaseData.GetInstance().NewbieGuideWeakGuideList[j])
                        {
                            bFind = true;
                            break;
                        }
                    }

                    if (bFind)
                    {
                        continue;
                    }

                    return (NewbieGuideTable.eNewbieGuideTask)TableNewbieGuideOrderList[i];
                }
            }

            return NewbieGuideTable.eNewbieGuideTask.None;
        }

        public void ManagerWait()
        {
            SetGuideState(eNewbieGuideState.Wait);
        }

        public void ManagerException()
        {
            SetGuideState(eNewbieGuideState.Exception);

            if (mGuideControl == null)
            {
                SetGuideState(eNewbieGuideState.None);
            }
            else
            {
                ManagerFinishGuide(mGuideControl.GuideTaskID);
            }
        }

        public NewbieGuideTable.eNewbieGuideTask GetCurTaskID()
        {
            if (mGuideControl == null)
            {
                return NewbieGuideTable.eNewbieGuideTask.None;
            }

            return mGuideControl.GuideTaskID;
        }

        public void DoGuideByEditor(NewbieGuideTable tabledata)
        {
            _loadEnv();

            NewbieGuideDataUnit unit = NewbieGuideDataManager.instance.GetData(tabledata, (NewbieGuideTable.eNewbieGuideTask)tabledata.ID);

            if (unit == null)
            {
                Logger.LogErrorFormat("无法创建引导---[NewbieGuideDataUnit unit]创建失败:{0}", (NewbieGuideTable.eNewbieGuideTask)tabledata.ID);

                return;
            }

            unit.manager = this;

            ClientSystemManager.instance.CloseFrame<TeamListFrame>();

            if (tabledata.ID != (int)NewbieGuideTable.eNewbieGuideTask.GuanKaGuide && tabledata.ID != (int)NewbieGuideTable.eNewbieGuideTask.AutoTraceGuide2)
            {
                BeTownPlayerMain.CommandStopAutoMove();

                Logger.LogWarningFormat("新手引导{0}触发--停止自动寻路", (NewbieGuideTable.eNewbieGuideTask)tabledata.ID);
            }

            SetGuideState(eNewbieGuideState.Guiding);

            if (tabledata.CloseFrames == true)
            {
                ClientSystemManager.instance.ForceClearFrame("");
            }

            _deleteCurGuideControl();
            _AddCurGuideControl((NewbieGuideTable.eNewbieGuideTask)tabledata.ID, unit);

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.CurGuideStart, (NewbieGuideTable.eNewbieGuideTask)tabledata.ID);
        }

        void RemoveUnit(NewbieGuideTable tabledata)
        {
            NewbieGuideDataUnit unit = null;

            if (tabledata.GuideSubType == NewbieGuideTable.eGuideSubType.GST_ALONE)
            {
                unit = GetAloneUnit((NewbieGuideTable.eNewbieGuideTask)tabledata.ID);

                if (unit != null)
                {
                    mAloneGuideUnits.Remove((NewbieGuideTable.eNewbieGuideTask)tabledata.ID);
                }
            }
            else if(tabledata.GuideSubType == NewbieGuideTable.eGuideSubType.GST_FINISH_TALK_DIALOG)
            {
                unit = GetFinishDialogUnit((NewbieGuideTable.eNewbieGuideTask)tabledata.ID);

                if (unit != null)
                {
                    mFinishDialogGuideUnits.Remove((NewbieGuideTable.eNewbieGuideTask)tabledata.ID);
                }
            }
            else if(tabledata.GuideSubType == NewbieGuideTable.eGuideSubType.GST_ACCEPT_MISSION)
            {
                unit = GetAcceptMissionUnit((NewbieGuideTable.eNewbieGuideTask)tabledata.ID);

                if (unit != null)
                {
                    mAcceptMissionGuideUnits.Remove((NewbieGuideTable.eNewbieGuideTask)tabledata.ID);
                }
            }
            else if(tabledata.GuideSubType == NewbieGuideTable.eGuideSubType.GST_FINISH_MISSION)
            {
                unit = GetFinishMissionUnit((NewbieGuideTable.eNewbieGuideTask)tabledata.ID);

                if (unit != null)
                {
                    mFinishMissionGuideUnits.Remove((NewbieGuideTable.eNewbieGuideTask)tabledata.ID);
                }
            }
            else if(tabledata.GuideSubType == NewbieGuideTable.eGuideSubType.GST_RECEIVE_MISSION_AWARD)
            {
                unit = GetReceiveMissionAwardUnit((NewbieGuideTable.eNewbieGuideTask)tabledata.ID);

                if (unit != null)
                {
                    mReceiveMissionRewardGuideUnits.Remove((NewbieGuideTable.eNewbieGuideTask)tabledata.ID);
                }
            }
            else
            {
                if (tabledata.NewbieGuideType == NewbieGuideTable.eNewbieGuideType.NGT_FORCE)
                {
                    unit = GetForceUnit((NewbieGuideTable.eNewbieGuideTask)tabledata.ID);

                    if (unit != null)
                    {
                        mForceGuideUnits.Remove((NewbieGuideTable.eNewbieGuideTask)tabledata.ID);
                    }
                }
                else
                {
                    unit = GetWeakUnit((NewbieGuideTable.eNewbieGuideTask)tabledata.ID);

                    if (unit != null)
                    {
                        mWeakGuideUnits.Remove((NewbieGuideTable.eNewbieGuideTask)tabledata.ID);
                    }
                }
            }
        }

        public NewbieGuideDataUnit GetUnit(NewbieGuideTable tabledata)
        {
            NewbieGuideDataUnit unit = null;

            if (tabledata.GuideSubType == NewbieGuideTable.eGuideSubType.GST_ALONE)
            {
                unit = GetAloneUnit((NewbieGuideTable.eNewbieGuideTask)tabledata.ID);
            }
            else if (tabledata.GuideSubType == NewbieGuideTable.eGuideSubType.GST_FINISH_TALK_DIALOG)
            {
                unit = GetFinishDialogUnit((NewbieGuideTable.eNewbieGuideTask)tabledata.ID);
            }
            else if(tabledata.GuideSubType == NewbieGuideTable.eGuideSubType.GST_ACCEPT_MISSION)
            {
                unit = GetAcceptMissionUnit((NewbieGuideTable.eNewbieGuideTask)tabledata.ID);
            }
            else if(tabledata.GuideSubType == NewbieGuideTable.eGuideSubType.GST_FINISH_MISSION)
            {
                unit = GetFinishMissionUnit((NewbieGuideTable.eNewbieGuideTask)tabledata.ID);
            }
            else if(tabledata.GuideSubType == NewbieGuideTable.eGuideSubType.GST_RECEIVE_MISSION_AWARD)
            {
                unit = GetReceiveMissionAwardUnit((NewbieGuideTable.eNewbieGuideTask)tabledata.ID);
            }
            else
            {
                if (tabledata.NewbieGuideType == NewbieGuideTable.eNewbieGuideType.NGT_FORCE)
                {
                    unit = GetForceUnit((NewbieGuideTable.eNewbieGuideTask)tabledata.ID);
                }
                else
                {
                    unit = GetWeakUnit((NewbieGuideTable.eNewbieGuideTask)tabledata.ID);
                }
            }

            return unit;
        }

        NewbieGuideDataUnit GetForceUnit(NewbieGuideTable.eNewbieGuideTask task)
        {
            if (mForceGuideUnits.ContainsKey(task))
            {
                return mForceGuideUnits[task];
            }

            return null;
        }

        NewbieGuideDataUnit GetWeakUnit(NewbieGuideTable.eNewbieGuideTask task)
        {
            if (mWeakGuideUnits.ContainsKey(task))
            {
                return mWeakGuideUnits[task];
            }

            return null;
        }

        NewbieGuideDataUnit GetAloneUnit(NewbieGuideTable.eNewbieGuideTask task)
        {
            if (mAloneGuideUnits.ContainsKey(task))
            {
                return mAloneGuideUnits[task];
            }

            return null;
        }

        NewbieGuideDataUnit GetFinishDialogUnit(NewbieGuideTable.eNewbieGuideTask task)
        {
            if (mFinishDialogGuideUnits.ContainsKey(task))
            {
                return mFinishDialogGuideUnits[task];
            }

            return null;
        }

        NewbieGuideDataUnit GetAcceptMissionUnit(NewbieGuideTable.eNewbieGuideTask task)
        {
            if (mAcceptMissionGuideUnits.ContainsKey(task))
            {
                return mAcceptMissionGuideUnits[task];
            }

            return null;
        }

        NewbieGuideDataUnit GetFinishMissionUnit(NewbieGuideTable.eNewbieGuideTask task)
        {
            if (mFinishMissionGuideUnits.ContainsKey(task))
            {
                return mFinishMissionGuideUnits[task];
            }

            return null;
        }

        NewbieGuideDataUnit GetReceiveMissionAwardUnit(NewbieGuideTable.eNewbieGuideTask task)
        {
            if (mReceiveMissionRewardGuideUnits.ContainsKey(task))
            {
                return mReceiveMissionRewardGuideUnits[task];
            }

            return null;
        }

        public NewbieGuideComType GetCurGuideType()
        {
            if (GetCurTaskID() == NewbieGuideTable.eNewbieGuideTask.None)
            {
                return NewbieGuideComType.USER_DEFINE;
            }

            NewbieGuideTable tabledata = TableManager.GetInstance().GetTableItem<NewbieGuideTable>((int)GetCurTaskID());
            if(tabledata == null)
            {
                return NewbieGuideComType.USER_DEFINE;
            }

            NewbieGuideDataUnit CurUnit = GetUnit(tabledata);
               
            if (CurUnit == null)
            {
                return NewbieGuideComType.USER_DEFINE;
            }

            if (CurUnit.newbieComList == null || CurUnit.newbieComList.Count <= 0 || CurUnit.savePoint < 0 || CurUnit.savePoint >= CurUnit.newbieComList.Count)
            {
                return NewbieGuideComType.USER_DEFINE;
            }

            return CurUnit.newbieComList[CurUnit.savePoint].ComType;
        }

        public void CleanWeakGuide()
        {
            if (mNewbieGuideManagerObject != null)
            {
                var com = mNewbieGuideManagerObject.GetComponent<ComNewbieGuideWeakControl>();

                if (com != null)
                {
                    com.FinishCurGuideControl();

                    SetGuideState(eNewbieGuideState.None);
                }
            }
        }

        public void Save()
        {
        }

        #region notuse
        //         public bool IsInitFirstFight()
        //         {
        //             return false;
        //         }

        // 目前没有通过关闭界面来达到跳过引导的需求了，但是这段代码的功能是正常的，暂时不要删除
        //         void OnSkipGuideByFrameClose(UIEvent uiEvent)
        //         {
        //             if (!PlayerBaseData.GetInstance().bInitializeTownSystem)
        //             {
        //                 return;
        //             }
        // 
        //             if (!IsGuiding())
        //             {
        //                 return;
        //             }
        // 
        //             if (mGuideControl == null)
        //             {
        //                 return;
        //             }
        // 
        //             NewbieGuideDataUnit ControlGuideUnit = mGuideControl.GetControlUnit();
        //             if (ControlGuideUnit == null)
        //             {
        //                 return;
        //             }
        // 
        //             int iIndex = mGuideControl.GetCurIndex();
        // 
        //             if (ControlGuideUnit.newbieComList == null || iIndex < 0 || iIndex >= ControlGuideUnit.newbieComList.Count)
        //             {
        //                 return;
        //             }
        // 
        //             if (ControlGuideUnit.newbieComList[iIndex].args == null || ControlGuideUnit.newbieComList[iIndex].args.Length <= 0)
        //             {
        //                 return;
        //             }
        // 
        //             if (!(ControlGuideUnit.newbieComList[iIndex].args[0] is string))
        //             {
        //                 return;
        //             }
        // 
        //             if ((string)ControlGuideUnit.newbieComList[iIndex].args[0] != (string)uiEvent.Param1)
        //             {
        //                 return;
        //             }
        // 
        //             Logger.LogFormat("新手引导流程有问题，关闭界面跳过该引导，流程id = {0}, 小步骤id = {1}", ControlGuideUnit.taskId, iIndex);
        // 
        //             ManagerFinishGuide(ControlGuideUnit.taskId);
        //         }
        #endregion
    }

    #region condition
    public class NewbieGuideConditionUnit
    {
    }

    public class NewbieGuideCondition
    {
        public List<NewbieGuideConditionUnit> mConditionList = new List<NewbieGuideConditionUnit>();
    }

    public class NewbieGuideConditionUtil
    {
        private static bool _checkArgsLimit(int count, params int[] args)
        {
            if (args == null)
            {
                Logger.LogError("args is nil");
                return false;
            }

            if (args.Length >= count)
            {
                return true;
            }

            // Logger.LogErrorFormat("args is not valid with count {0}, require {1} args", args.Length, count);

            return false;
        }

        public static bool CheckCondition(NewbieGuideTable.eNewbieGuideTask taskId, UIEvent uiEvent, NewbieConditionData data, eNewbieGuideCondition type, ref List<object> NeedSaveParamList, params int[] LimitArgs)
        {
            //Logger.LogProcessFormat("check condition with type {0}", type);

            switch (type)
            {
                case eNewbieGuideCondition.UserDefine:
                    {
                        if (data != null && data.mComditionFunc != null)
                        {
                            return data.mComditionFunc();
                        }
                        break;
                    }
                case eNewbieGuideCondition.Level:
                    {
                        if (_checkArgsLimit(1, LimitArgs))
                        {
                            var level = PlayerBaseData.GetInstance().Level;
                            return LimitArgs[0] <= level;
                        }

                        break;
                    }
                case eNewbieGuideCondition.MaxLevel:
                    {
                        if (_checkArgsLimit(1, LimitArgs))
                        {
                            var level = PlayerBaseData.GetInstance().Level;
                            return level <= LimitArgs[0];
                        }

                        break;
                    }
                case eNewbieGuideCondition.SceneLogin:
                    {
                        return ClientSystemManager.instance.CurrentSystem is ClientSystemLogin;
                    }
                case eNewbieGuideCondition.SceneTown:
                    {
                        return (ClientSystemManager.instance.CurrentSystem is ClientSystemTown && !ClientSystemManager.instance.bIsInPkWaitingRoom);
                    }
                case eNewbieGuideCondition.ScenePkWaitingRoom:
                    {
                        return (ClientSystemManager.instance.CurrentSystem is ClientSystemTown && ClientSystemManager.instance.bIsInPkWaitingRoom);
                    }
                case eNewbieGuideCondition.SceneBattle:
                    {
                        return ClientSystemManager.instance.CurrentSystem is ClientSystemBattle;
                    }
                case eNewbieGuideCondition.IsDungeon:
                    {
                        if (BattleMain.instance != null)
                        {
                            if (BattleMain.battleType == BattleType.Dungeon)
                            {
                                return true;
                            }
                        }

                        break;
                    }
                case eNewbieGuideCondition.BattleInitFinished:
                    {
                        if (BattleMain.instance != null)
                        {
                            IDungeonManager manager = BattleMain.instance.GetDungeonManager();

                            if(manager != null)
                            {
                                BeScene scene = manager.GetBeScene();

                                if(scene != null)
                                {
                                    if(scene.state == BeSceneState.onFight)
                                    {
                                        return true;
                                    }
                                }
                            }
                        }

                        break;
                    }
                case eNewbieGuideCondition.ClientSystemTownFrameOpen:
                    {
                        return ClientSystemManager.instance.IsFrameOpen<ClientSystemTownFrame>();
                    }
                case eNewbieGuideCondition.HaveWhiteEquipment:
                    {
                        return NewbieGuideManager.GetInstance().IsHavingWhiteGuide();
                    }
                case eNewbieGuideCondition.ChapterChooseFrameOpen:
                    {
                        return ClientSystemManager.instance.IsFrameOpen<ChapterSelectFrame>();
                    }
                case eNewbieGuideCondition.ChapterChooseFrameOpenHaveParameter:
                    {
                        if (_checkArgsLimit(1, LimitArgs))
                        {
                            if (ClientSystemManager.instance.IsFrameOpen<ChapterSelectFrame>())
                            {
                                ChapterSelectFrame chapterSelectFrame = ClientSystemManager.instance.GetFrame(typeof(ChapterSelectFrame)) as ChapterSelectFrame;
                                if(chapterSelectFrame != null)
                                {
                                    return chapterSelectFrame._GetChapterIndex() == LimitArgs[0];
                                }
                                else
                                {
                                    return false;
                                }
                            }
                            return false;
                        }
                        return false;
                    }
                case eNewbieGuideCondition.QuickEquipFrameOpen:
                    {
                        return ClientSystemManager.instance.IsFrameOpen<EquipmentChangedFrame>();
                    }
                case eNewbieGuideCondition.SpecificEvent:
                    {
                        if (_checkArgsLimit(1, LimitArgs))
                        {
                            return LimitArgs[0] == (int)uiEvent.EventID;
                        }

                        break;
                    }
                case eNewbieGuideCondition.MainFrameMutex:
                    {
                        ClientSystemTownFrame Townframe = ClientSystemManager.GetInstance().GetFrame(typeof(ClientSystemTownFrame)) as ClientSystemTownFrame;
                        if (Townframe == null)
                        {
                            break;
                        }

                        if (Townframe.IsHidden())
                        {
                            break;
                        }

                        if (!ClientSystemManager.GetInstance().IsMainPrefabTop())
                        {
                            break;
                        }

                        return true;
                    }
                case eNewbieGuideCondition.EquipmentInPackage:
                    {
                        List<ulong> EquipmentIDs = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.Equip);

                        if (EquipmentIDs == null || EquipmentIDs.Count <= 0 || NeedSaveParamList == null)
                        {
                            break;
                        }

                        List<ItemData> Temps = new List<ItemData>();

                        for (int i = 0; i < EquipmentIDs.Count; i++)
                        {
                            ItemData item = ItemDataManager.GetInstance().GetItem(EquipmentIDs[i]);

                            if (item == null)
                            {
                                continue;
                            }

                            if (PlayerBaseData.GetInstance().Level < item.LevelLimit)
                            {
                                continue;
                            }

                            if (taskId == NewbieGuideTable.eNewbieGuideTask.ForgeGuide)
                            {
                                if (item.EquipWearSlotType != EEquipWearSlotType.EquipWeapon)
                                {
                                    continue;
                                }
                            }

                            if (!item.IsOccupationFit())
                            {
                                continue;
                            }

                            if (item.Quality < ItemTable.eColor.BLUE)
                            {
                                continue;
                            }

                            Temps.Add(item);
                        }

                        for (int i = 0; i < Temps.Count; i++)
                        {
                            for (int j = i + 1; j < Temps.Count; j++)
                            {
                                if (Temps[j].Quality > Temps[i].Quality)
                                {
                                    ItemData item = Temps[i];
                                    Temps[i] = Temps[j];
                                    Temps[j] = item;
                                }
                            }
                        }

                        if (Temps.Count <= 0)
                        {
                            break;
                        }

                        NeedSaveParamList.Add(Temps[0].GUID);

                        return true;
                    }

                case eNewbieGuideCondition.MagicBoxGuide:
                    {
                        List<ulong> ConsumableIDs = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.Consumable);

                        if (ConsumableIDs == null || ConsumableIDs.Count <= 0 || NeedSaveParamList == null)
                        {
                            break;
                        }

                        List<ItemData> Temps = new List<ItemData>();

                        for (int i = 0; i < ConsumableIDs.Count; i++)
                        {
                            ItemData item = ItemDataManager.GetInstance().GetItem(ConsumableIDs[i]);

                            if (item == null)
                            {
                                continue;
                            }

                            if (PlayerBaseData.GetInstance().Level < item.LevelLimit)
                            {
                                continue;
                            }

                            if(item.SubType != (int)ItemTable.eSubType.MagicBox)
                            {
                                continue;
                            }

                            NeedSaveParamList.Add(item.GUID);

                            break;
                        }

                        return true;
                    }

                case eNewbieGuideCondition.ChangeEquipmentExist:
                    {
                        List<ulong> EquipmentIDs = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.Equip);
                        if (EquipmentIDs == null || EquipmentIDs.Count <= 0 || NeedSaveParamList == null)
                        {
                            break;
                        }

                        for (int i = 0; i < (int)EEquipWearSlotType.EquipMax; i++)
                        {
                            if ((EEquipWearSlotType)i != EEquipWearSlotType.EquipWeapon)
                            {
                                continue;
                            }

                            ulong WearEuipID = ItemDataManager.GetInstance().GetWearEquipBySlotType((EEquipWearSlotType)i);

                            if (WearEuipID == 0)
                            {
                                break;
                            }

                            bool bFind = false;
                            for (int j = 0; j < EquipmentIDs.Count; j++)
                            {
                                ItemData Packageitem = ItemDataManager.GetInstance().GetItem(EquipmentIDs[j]);

                                if (Packageitem == null)
                                {
                                    continue;
                                }

                                if (Packageitem.EquipWearSlotType != (EEquipWearSlotType)i)
                                {
                                    continue;
                                }

                                if (!Packageitem.IsOccupationFit())
                                {
                                    continue;
                                }

                                if (PlayerBaseData.GetInstance().Level < Packageitem.LevelLimit)
                                {
                                    continue;
                                }

                                if (!Packageitem.IsBetterThanEquip())
                                {
                                    continue;
                                }

                                if ((int)Packageitem.Quality < (int)ItemTable.eColor.PURPLE)
                                {
                                    continue;
                                }

                                NeedSaveParamList.Add(Packageitem.GUID);
                                NeedSaveParamList.Add(WearEuipID);

                                bFind = true;

                                break;
                            }

                            if (bFind)
                            {
                                return true;
                            }
                        }

                        break;
                    }
                case eNewbieGuideCondition.AchievementFinished:
                    {
                        var mainIDs = MissionManager.GetInstance().GetTaskByType(MissionTable.eTaskType.TT_ACHIEVEMENT, TaskStatus.TASK_FINISHED);
                        if (mainIDs == null || mainIDs.Count <= 0 || NeedSaveParamList == null)
                        {
                            break;
                        }

                        NeedSaveParamList.Add(mainIDs[0].taskID);

                        return true;
                    }
                case eNewbieGuideCondition.BranchMissionAccept:
                    {
                        var mainIDs = MissionManager.GetInstance().GetTaskByType(MissionTable.eTaskType.TT_BRANCH, TaskStatus.TASK_INIT);
                        if (mainIDs == null || mainIDs.Count <= 0 || NeedSaveParamList == null)
                        {
                            break;
                        }

                        NeedSaveParamList.Add(mainIDs[0].taskID);

                        return true;
                    }
                case eNewbieGuideCondition.DailyMissionFinished:
                    {
                        int iMissionID = 0;

                        if (!_checkArgsLimit(1, LimitArgs))
                        {
                            break;
                        }

                        iMissionID = LimitArgs[0];

                        List<MissionManager.SingleMissionInfo> MissionInfos = MissionManager.GetInstance().GetTaskByType(MissionTable.eTaskType.TT_DIALY, TaskStatus.TASK_FINISHED);
                        //if (MissionInfos == null || MissionInfos.Count <= 0 || NeedSaveParamList == null)
                        if (MissionInfos == null || MissionInfos.Count <= 0)
                        {
                            break;
                        }

                        for (int i = 0; i < MissionInfos.Count; i++)
                        {
                            if (MissionInfos[i].missionItem.SubType != MissionTable.eSubType.Daily_Task)
                            {
                                continue;
                            }

                            if(MissionInfos[i].missionItem.ID != iMissionID)
                            {
                                continue;
                            }

                            //NeedSaveParamList.Add(MissionInfos[i].taskID);

                            return true;
                        }

                        break;
                    }
                case eNewbieGuideCondition.SignInFinished:
                    {
                        var activeData = ActiveManager.GetInstance().GetActiveData(3000);
                        if (activeData == null || NeedSaveParamList == null)
                        {
                            break;
                        }

                        if (activeData.akChildItems == null || activeData.akChildItems.Count <= 0)
                        {
                            break;
                        }

                        for (int i = 0; i < activeData.akChildItems.Count; i++)
                        {
                            if (activeData.akChildItems[i].status == 2)
                            {
                                NeedSaveParamList.Add(activeData.akChildItems[i].ID);
                                return true;
                            }
                        }

                        break;
                    }
                case eNewbieGuideCondition.DungeonID:
                    {
                        if (_checkArgsLimit(1, LimitArgs))
                        {
                            var dugeonid = BattleDataManager.GetInstance().BattleInfo.dungeonId;
                            return LimitArgs[0] == dugeonid;
                        }

                        break;
                    }
                case eNewbieGuideCondition.SceneID:
                    {
                        if (_checkArgsLimit(1, LimitArgs))
                        {
                            var areaId = BattleDataManager.GetInstance().BattleInfo.areaId;
                            return LimitArgs[0] == areaId;
                        }

                        break;
                    }
                case eNewbieGuideCondition.DungeonStartTime:
                    {
                        if (_checkArgsLimit(1, LimitArgs))
                        {
                            int startTime = BattleMain.instance.GetDungeonStatistics().AllFightTime(false);
                            return startTime >= LimitArgs[0];
                        }

                        break;
                    }
                case eNewbieGuideCondition.DungeonAreaID:
                    {
                        if (_checkArgsLimit(1, LimitArgs))
                        {
                            var mng = BattleMain.instance.GetDungeonManager();

                            int iAreaId = mng.GetDungeonDataManager().CurrentAreaID();
                            return iAreaId == LimitArgs[0];
                        }

                        break;
                    }
                case eNewbieGuideCondition.LearnBufferSkill:
                    {
                        if (NeedSaveParamList == null)
                        {
                            break;
                        }

                        List<Skill> skillList = SkillDataManager.GetInstance().GetSkillList(false);

                        for (int i = 0; i < skillList.Count; i++)
                        {
                            SkillTable tblData = TableManager.GetInstance().GetTableItem<SkillTable>(skillList[i].id);
                            if(tblData == null || tblData.IsBuff != 1)
                            {
                                continue;
                            }

                            NeedSaveParamList.Add((int)skillList[i].id);
                            return true;
                        }

                        break;
                    }
                case eNewbieGuideCondition.NewbieGuideID:
                    {
                        if (_checkArgsLimit(1, LimitArgs))
                        {
                            NewbieGuideTable tabledata = TableManager.GetInstance().GetTableItem<NewbieGuideTable>(LimitArgs[0]);
                            if (tabledata == null || tabledata.IsOpen == 0)
                            {
                                return false;
                            }

                            if (tabledata.NewbieGuideType == NewbieGuideTable.eNewbieGuideType.NGT_FORCE && tabledata.Order <= PlayerBaseData.GetInstance().NewbieGuideCurSaveOrder)
                            {
                                return true;
                            }
                            else if(tabledata.NewbieGuideType == NewbieGuideTable.eNewbieGuideType.NGT_WEAK)
                            {
                                for(int i = 0; i < PlayerBaseData.GetInstance().NewbieGuideWeakGuideList.Count; i++)
                                {
                                    if(PlayerBaseData.GetInstance().NewbieGuideWeakGuideList[i] == LimitArgs[0])
                                    {
                                        return true;
                                    }
                                }

                                return false;
                            }
                        }

                        break;
                    }
                case eNewbieGuideCondition.AddNewMissionID:
                    {
                        if(uiEvent.EventID != EUIEventID.AddNewMission)
                        {
                            break;
                        }

                        if (_checkArgsLimit(1, LimitArgs))
                        {
                            if (LimitArgs[0] == (int)uiEvent.Param1)
                            {
                                return true;
                            }
                        }

                        break;
                    }
                case eNewbieGuideCondition.FinishedMissionID:
                    {
                        if(uiEvent.EventID != EUIEventID.MissionRewardFrameClose && uiEvent.EventID != EUIEventID.LevelChanged)
                        {
                            break;
                        }

                        if (_checkArgsLimit(1, LimitArgs))
                        {
                            if (LimitArgs[0] == PlayerBaseData.GetInstance().GuideFinishMission)
                            {
                                return true;
                            }
                        }

                        break;
                    }
                case eNewbieGuideCondition.FinishTalkDialog:
                    {
                        if (uiEvent.EventID != EUIEventID.FinishTalkDialog)
                        {
                            break;
                        }

                        if (_checkArgsLimit(1, LimitArgs))
                        {
                            if (LimitArgs[0] == (int)uiEvent.Param1)
                            {
                                return true;
                            }
                        }

                        break;
                    }
                case eNewbieGuideCondition.ChangedJob:
                    {
                        JobTable jobData = TableManager.GetInstance().GetTableItem<JobTable>(PlayerBaseData.GetInstance().JobTableID);

                        if(jobData == null)
                        {
                            break;
                        }

                        if(jobData.JobType == 1)
                        {
                            return true;
                        }

                        break;
                    }
                case eNewbieGuideCondition.OnMission:
                    {
                        if (_checkArgsLimit(1, LimitArgs))
                        {
                            if (MissionManager.GetInstance().IsAcceptMission((uint)LimitArgs[0]))
                            {
                                return true;
                            }
                            return false;
                        }
                        break;
                    }
                default:
                    {
                        //Logger.LogErrorFormat("not dealed type with {0}", type);
                        break;
                    }
            }

            return false;
        }
    }
    #endregion

    [LoggerModel("NewbieGuide")]
    public class NewbieGuideDataManager : Singleton<NewbieGuideDataManager>
    {
        private Dictionary<int, NoviceGuideChooseFlag> roleNewbieGuideStateDic = new Dictionary<int, NoviceGuideChooseFlag>();
        public NoviceGuideChooseFlag GetRoleNewbieguideState(int id)
        {
            if(roleNewbieGuideStateDic.ContainsKey(id))
            {
                return roleNewbieGuideStateDic[id];
            }
            else
            {
                return NoviceGuideChooseFlag.NGCF_INIT;
            }
        }

        public void SetRoleNewbieguideState(int id,NoviceGuideChooseFlag state)
        {

            if(roleNewbieGuideStateDic.ContainsKey(id))
            {
                roleNewbieGuideStateDic[id] = state;
            }
            else
            {
                roleNewbieGuideStateDic.Add(id, state);
            }
        }
        public NewbieGuideDataUnit GetData(NewbieGuideTable resTable, NewbieGuideTable.eNewbieGuideTask taskID)
        {
            NewbieGuideDataUnit data = null;

            if (false == string.IsNullOrEmpty(resTable.ScriptDataPath))
            {
                var scriptData = AssetLoader.instance.LoadRes(resTable.ScriptDataPath, typeof(DNewbieGuideData), false).obj as DNewbieGuideData;

                if (scriptData != null)
                {
                    NewbieScriptDataGuide scriptGuide = new NewbieScriptDataGuide(resTable.ID);
                    scriptGuide.LoadScriptData(scriptData);
                    data = scriptGuide;
                }
            }

            if (data == null)
            {
                string typeName = "GameClient.Newbie" + taskID.ToString();

                if (typeName != null)
                {
                    var type = Type.GetType(typeName);

                    if (type == null)
                    {
                        Logger.LogErrorFormat("NewbieGuideManager eNewbieGuideTask [{0}] is wrong, please check!", typeName);
                    }

                    data = (NewbieGuideDataUnit)Activator.CreateInstance(type, (int)taskID);
                }

            }

            if (data != null)
            {
                data.Init();
            }

            return data;
        }

        private string _getForceGuideKey()
        {
            return string.Format("{0}:{1}", PlayerBaseData.GetInstance().RoleID, "NewbieForceGuideProcess");
        }

        private string _getRecordKey(NewbieGuideTable.eNewbieGuideTask taskID)
        {
            var accountDefault = PlayerLocalSetting.GetValue("AccountDefault");

            if (accountDefault == null)
            {
                return "";
            }

            return string.Format("{0}:{1}", PlayerLocalSetting.GetValue(accountDefault as string), taskID);
        }

        public NewbieGuideTable.eNewbieGuideTask GetLocalForceGuideProcess()
        {
            var obj = PlayerLocalSetting.GetValue(_getForceGuideKey());

            if (obj == null)
            {
                return NewbieGuideTable.eNewbieGuideTask.None;
            }

            string sObj = string.Format("{0}", obj);
            int iObj = int.Parse(sObj);

            return (NewbieGuideTable.eNewbieGuideTask)iObj;
        }

        public bool GetRecordLocalProcess(NewbieGuideTable.eNewbieGuideTask taskID)
        {
            // 这个是按账号记录的
            var obj = PlayerLocalSetting.GetValue(_getRecordKey(taskID));

            if (obj == null)
            {
                return false;
            }

            return (bool)obj;
        }

        public void SetLocalForceGuideProcess(NewbieGuideTable.eNewbieGuideTask taskID)
        {
            int iTaskID = (int)taskID;

            string sTaskId = iTaskID.ToString();

            PlayerLocalSetting.SetValue(_getForceGuideKey(), sTaskId);
            PlayerLocalSetting.SaveConfig();
        }

        public void RecordLocalProcess(NewbieGuideTable.eNewbieGuideTask taskID, bool state)
        {
            PlayerLocalSetting.SetValue(_getRecordKey(taskID), state);
            PlayerLocalSetting.SaveConfig();
        }
    }
}