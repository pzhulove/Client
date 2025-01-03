using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProtoTable;
using UnityEngine.Assertions;
using Protocol;
using Network;
using System;

namespace GameClient
{
    class GuildManorFrame : ClientFrame
    {
        class TimeInfo
        {
            public int day = 0;
            public int hour = 0;
            public int minute = 0;
            public int second = 0;
        }
        string AttackCityEffectPath = "Effects/Scene_effects/EffectUI/EffUI_chuizi";
        const int TerritoryNum = 10;
        int[] TerritoryIDs = new int[TerritoryNum] {1,2,3,4,5,6,7,21,22,23 };
        const int guildActivityTableID = 1;
        List<TimeInfo> signUpTimeInfos = new List<TimeInfo>();

        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Guild/GuildManor";
        }

        protected sealed override void _OnOpenFrame()
        {
            for (int i = 0; i < TerritoryIDs.Length; ++i)
            {
                int nID = TerritoryIDs[i];
                GuildTerritoryTable tableData = _GetTerritoryTableData(nID);
                Assert.IsNotNull(tableData);

                GameObject obj = Utility.FindGameObject(string.Format("Map/Manor{0}", nID));
                Button btnFunc = Utility.GetComponetInChild<Button>(obj, "Func");
                btnFunc.onClick.RemoveAllListeners();
                btnFunc.onClick.AddListener(() =>
                {
                    GuildDataManager.GetInstance().RequestManorInfo(nID);
                });

                Text nameTerritory = Utility.GetComponetInChild<Text>(obj, "Name/Text");
                nameTerritory.SafeSetText(tableData.Name);

                if (GuildDataManager.GetInstance().HasTargetManor() == false &&
                    GuildDataManager.GetInstance().GetGuildBattleType() == GuildBattleType.GBT_NORMAL && 
                    GuildDataManager.GetInstance().GetGuildBattleState() == EGuildBattleState.Signup &&
                    GuildDataManager.GetInstance().HasPermission(EGuildPermission.StartGuildBattle))
                {
                    Utility.FindGameObject(obj, "ClickToSignup").SetActive(true);
                }
                else
                {
                    Utility.FindGameObject(obj, "ClickToSignup").SetActive(false);
                }

                Utility.FindGameObject(obj, "AlreadySignup").SetActive(
                    nID == GuildDataManager.GetInstance().myGuild.nTargetManorID
                    );

                string name = GuildDataManager.GetInstance().GetManorOwner(nID);

                if(name != "")
                {
                    Utility.GetComponetInChild<Text>(obj, "Owner").text = string.Format("【{0}】", name);
                }
                else
                {
                    Utility.GetComponetInChild<Text>(obj, "Owner").text = "";
                }        
            }

            _UpdateCurrentManaor();
            _UpdateTargetManaor();
            _UpdateJoin();
            _UpdateAttackCity();
            _UpdateShowTitle();
            _UpdateLotteryShow();
            UpdateInspireInfo();

            GuildDataManager.GetInstance().UpdateInspireInfo(ref mInspireLevel, ref mCurAttr, ref mInspireMax, ref mInspire, ref mInspireIcon, ref mInspireCount, ref mEnableInspire, GuildBattleOpenType.GBOT_NORMAL_CHALLENGE);

            _RegisterUIEvent();

            GuildDataManager.GetInstance().SetGuildBattleSignUpRedPoint(false);
            InitSignUpTimeInfos();
        }

        protected sealed override void _OnCloseFrame()
        {
            _UnRegisterUIEvent();
            signUpTimeInfos = null;
        }

        void _RegisterUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildInspireSuccess, _OnInspireSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildSignupSuccess, _OnSignupSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildManorInfoUpdated, _OnManorInfoUpdated);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildBaseInfoUpdated, _OnGuildBaseInfoUpdated);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildBattleStateChanged, _OnGuildBattleStateUpdated);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildManorOwnerUpdated, _OnManorOwnerUpdate);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildAttackCityInfoUpdate, _OnGuildAttackCityInfoUpdate);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildLotteryResultRes, _OnGuildLotteryResultRes);
        }

        void _UnRegisterUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildInspireSuccess, _OnInspireSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildSignupSuccess, _OnSignupSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildManorInfoUpdated, _OnManorInfoUpdated);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildBaseInfoUpdated, _OnGuildBaseInfoUpdated);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildBattleStateChanged, _OnGuildBattleStateUpdated);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildManorOwnerUpdated, _OnManorOwnerUpdate);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildAttackCityInfoUpdate, _OnGuildAttackCityInfoUpdate);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildLotteryResultRes, _OnGuildLotteryResultRes);
        }

        void _OnInspireSuccess(UIEvent a_event)
        {
            GuildDataManager.GetInstance().UpdateInspireInfo(ref mInspireLevel, ref mCurAttr, ref mInspireMax, ref mInspire, ref mInspireIcon, ref mInspireCount, ref mEnableInspire, GuildBattleOpenType.GBOT_NORMAL_CHALLENGE);
        }

        void _OnSignupSuccess(UIEvent a_event)
        {
            _UpdateTargetManaor();
            _UpdateManor();

            UpdateInspireInfo();
            GuildDataManager.GetInstance().UpdateInspireInfo(ref mInspireLevel, ref mCurAttr, ref mInspireMax, ref mInspire, ref mInspireIcon, ref mInspireCount, ref mEnableInspire, GuildBattleOpenType.GBOT_NORMAL_CHALLENGE);

            ClientSystemManager.GetInstance().CloseFrame<GuildManorInfoFrame>();
            SystemNotifyManager.SysNotifyTextAnimation(TR.Value("guild_manor_signup_success"));
        }

        void _OnManorInfoUpdated(UIEvent a_event)
        {
            GuildTerritoryBaseInfo data = a_event.Param1 as GuildTerritoryBaseInfo;
            if (data != null)
            {
                ClientSystemManager.GetInstance().OpenFrame<GuildManorInfoFrame>(FrameLayer.Middle, data);
            }
        }

        void _OnGuildBaseInfoUpdated(UIEvent a_event)
        {
            _UpdateCurrentManaor();
            _UpdateTargetManaor();
            _UpdateManor();
            UpdateInspireInfo();

            GuildDataManager.GetInstance().UpdateInspireInfo(ref mInspireLevel, ref mCurAttr, ref mInspireMax, ref mInspire, ref mInspireIcon, ref mInspireCount, ref mEnableInspire, GuildBattleOpenType.GBOT_NORMAL_CHALLENGE);
        }

        void _OnGuildBattleStateUpdated(UIEvent a_event)
        {
            _UpdateJoin();
            _UpdateAttackCity();
            _UpdateManor();
            _UpdateLotteryShow();

            GuildDataManager.GetInstance().UpdateInspireInfo(ref mInspireLevel, ref mCurAttr, ref mInspireMax, ref mInspire, ref mInspireIcon, ref mInspireCount, ref mEnableInspire, GuildBattleOpenType.GBOT_NORMAL_CHALLENGE);
            UpdateInspireInfo();
        }
        void UpdateInspireInfo()
        {
            inspireRoot.CustomActive(GuildDataManager.GetInstance().HasSelfGuild() && GuildDataManager.GetInstance().HasTargetManor());
        }

        void _OnManorOwnerUpdate(UIEvent a_event)
        {
            _UpdateManor();
        }

        void _OnGuildAttackCityInfoUpdate(UIEvent a_event)
        {
            _UpdateAttackCity();
        }

        void _OnGuildLotteryResultRes(UIEvent a_event)
        {
            _UpdateLotteryShow();
        }

        void _UpdateCurrentManaor()
        {
            if (GuildDataManager.GetInstance().HasSelfGuild() && GuildDataManager.GetInstance().HasSelfManor())
            {
                GuildTerritoryTable tableData = _GetTerritoryTableData(GuildDataManager.GetInstance().myGuild.nSelfManorID);
                Assert.IsNotNull(tableData);
                mCurManorName.text = tableData.Name;
            }
            else
            {
                mCurManorName.text = TR.Value("guild_manor_none_manor");
            }
        }

        void _UpdateTargetManaor()
        {
            if (GuildDataManager.GetInstance().HasSelfGuild() && GuildDataManager.GetInstance().HasTargetManor())
            {
                GuildTerritoryTable tableData = _GetTerritoryTableData(GuildDataManager.GetInstance().myGuild.nTargetManorID);

                if(tableData != null)
                {
                    mTargetManorName.text = tableData.Name;
                }
                else
                {
                    mTargetManorName.text = TR.Value("guild_manor_none_manor");
                }
            }
            else
            {
                mTargetManorName.text = TR.Value("guild_manor_none_manor");
            }
        }

        void _UpdateJoin()
        {
            bool canBattle = GuildDataManager.GetInstance().HasTargetManor() &&
                 (GuildDataManager.GetInstance().GetGuildBattleType() == GuildBattleType.GBT_NORMAL || GuildDataManager.GetInstance().GetGuildBattleType() == GuildBattleType.GBT_CHALLENGE) &&
                 (GuildDataManager.GetInstance().GetGuildBattleState() == EGuildBattleState.Preparing || GuildDataManager.GetInstance().GetGuildBattleState() == EGuildBattleState.Firing);

            mEnableJoin.SetEnable(canBattle);
            mJoinRedPoint.gameObject.CustomActive((GuildDataManager.GetInstance().GetGuildBattleType() == GuildBattleType.GBT_NORMAL || GuildDataManager.GetInstance().GetGuildBattleType() == GuildBattleType.GBT_CHALLENGE) && RedPointDataManager.GetInstance().HasRedPoint(ERedPoint.GuildBattleEnter));
        }

        void _UpdateAttackCity()
        {
            if(GuildDataManager.GetInstance().GetGuildBattleType() == GuildBattleType.GBT_CHALLENGE)
            {
                EGuildBattleState state = GuildDataManager.GetInstance().GetGuildBattleState();

                if (state > EGuildBattleState.Invalid && state < EGuildBattleState.Firing)
                {
                    GuildAttackCityData AttackData = GuildDataManager.GetInstance().GetAttackCityData();

//                     if (state == EGuildBattleState.Signup && AttackData.info.terrId <= 0)
//                     {
//                         mAttackCity.gameObject.CustomActive(false);
//                     }
//                     else if (state == EGuildBattleState.Preparing && AttackData.enrollGuildId <= 0)
//                     {
//                         mAttackCity.gameObject.CustomActive(false);
//                     }
//                     else
//                     {
                         mAttackCity.gameObject.CustomActive(true);
//                     }            
                }
                else
                {
                    mAttackCity.gameObject.CustomActive(false);
                }   
            }
            else
            {
                mAttackCity.gameObject.CustomActive(false);
            }
        }

        void _UpdateShowTitle()
        {
            GuildTerritoryTable tableData = _GetTerritoryTableData(1);

            if(tableData == null)
            {
                return;
            }

            if(tableData.LeaderReward.Count > 0)
            {
                if (tableData.LeaderReward[0] != "-" && tableData.LeaderReward[0] != "")
                {
                    string[] values = tableData.LeaderReward[0].Split('_');

                    if(values.Length >= 2)
                    {
                        ItemData itemData = ItemDataManager.CreateItemDataFromTable(int.Parse(values[0]));
                        itemData.Count = int.Parse(values[1]);

                        ComItem comItem = CreateComItem(mLeaderRewardPos);

                        comItem.Setup(itemData, (var1, var2) =>
                        {
                            ItemTipManager.GetInstance().ShowTip(var2);
                        });
                    }
                }             
            }

            if (tableData.MemberReward.Count > 0)
            {
                if (tableData.MemberReward[0] != "-" && tableData.MemberReward[0] != "")
                {
                    string[] values = tableData.MemberReward[0].Split('_');

                    if(values.Length >= 2)
                    {
                        ItemData itemData = ItemDataManager.CreateItemDataFromTable(int.Parse(values[0]));
                        itemData.Count = int.Parse(values[1]);

                        ComItem comItem = CreateComItem(mMemberRewardPos);

                        comItem.Setup(itemData, (var1, var2) =>
                        {
                            ItemTipManager.GetInstance().ShowTip(var2);
                        });
                    }
                }
            }
        }

        void _UpdateLotteryShow()
        {
//             if(GuildDataManager.GetInstance().GetGuildBattleState() == EGuildBattleState.LuckyDraw)
//             {
//                 if(GuildDataManager.GetInstance().HasGuildBattleLotteryed())
//                 {
//                     mLottery.gameObject.CustomActive(true);
//                 }
//                 else
//                 {
//                     mLottery.gameObject.CustomActive(true);
//                 }
//             }
//             else
//             {
//                 mLottery.gameObject.CustomActive(false);
//             }
        }

        void _UpdateManor()
        {
            for (int i = 0; i < TerritoryIDs.Length; ++i)
            {
                int nID = TerritoryIDs[i];
                GuildTerritoryTable tableData = _GetTerritoryTableData(nID);
                Assert.IsNotNull(tableData);

                GameObject obj = Utility.FindGameObject(string.Format("Map/Manor{0}", nID));
                if (GuildDataManager.GetInstance().HasTargetManor() == false &&
                    GuildDataManager.GetInstance().GetGuildBattleType() == GuildBattleType.GBT_NORMAL &&
                    GuildDataManager.GetInstance().GetGuildBattleState() == EGuildBattleState.Signup && 
                    GuildDataManager.GetInstance().HasPermission(EGuildPermission.StartGuildBattle))
                {
                    Utility.FindGameObject(obj, "ClickToSignup").SetActive(true);
                }
                else
                {
                    Utility.FindGameObject(obj, "ClickToSignup").SetActive(false);
                }

                Utility.FindGameObject(obj, "AlreadySignup").SetActive(
                    nID == GuildDataManager.GetInstance().myGuild.nTargetManorID
                    );

                string name = GuildDataManager.GetInstance().GetManorOwner(nID);

                if(name != "")
                {
                    Utility.GetComponetInChild<Text>(obj, "Owner").text = string.Format("【{0}】", name);
                }
                else
                {
                    Utility.GetComponetInChild<Text>(obj, "Owner").text = "";
                }              
            }
        }

        GuildTerritoryTable _GetTerritoryTableData(int a_nID, bool a_bShowError = true)
        {
            GuildTerritoryTable tableData = TableManager.GetInstance().GetTableItem<ProtoTable.GuildTerritoryTable>(a_nID);
            if (tableData == null)
            {
                return null;
            }
            return tableData;
        }

        #region ExtraUIBind
        private Button mDetail = null;
        private Button mAttackCity = null;
        private Text mInspireLevel = null;
        private Text mCurManorName = null;
        private Text mTargetManorName = null;
        private Text mCurAttr = null;
        private Button mInspire = null;
        private Button mJoin = null;
        private ComButtonEnbale mEnableInspire = null;
        private ComButtonEnbale mEnableJoin = null;
        private GameObject mInspireMax = null;
        private Image mJoinRedPoint = null;
        private Image mInspireIcon = null;
        private Text mInspireCount = null;
        private GameObject mLeaderRewardPos = null;
        private GameObject mMemberRewardPos = null;
        private Button mStatue = null;
        private Button mViceStatue = null;
        private Button rankList = null;
        private Button dailyAward = null;
        private GameObject dailyAwardRedPoint = null;
        private Text leftTime = null;
        private GameObject leftTimeRoot = null;
        private Button statue = null;
        private GameObject inspireRoot = null;

        protected override void _bindExUI()
        {
            mDetail = mBind.GetCom<Button>("Detail");
            mDetail.onClick.AddListener(_onDetailButtonClick);
            mAttackCity = mBind.GetCom<Button>("AttackCity");
            mAttackCity.onClick.AddListener(_onAttackCityButtonClick);
            mInspireLevel = mBind.GetCom<Text>("InspireLevel");
            mCurManorName = mBind.GetCom<Text>("CurManorName");
            mTargetManorName = mBind.GetCom<Text>("TargetManorName");
            mCurAttr = mBind.GetCom<Text>("CurAttr");
            mInspire = mBind.GetCom<Button>("Inspire");
            mInspire.onClick.AddListener(_onInspireButtonClick);
            mJoin = mBind.GetCom<Button>("Join");
            mJoin.onClick.AddListener(_onJoinButtonClick);
            mEnableInspire = mBind.GetCom<ComButtonEnbale>("EnableInspire");
            mEnableJoin = mBind.GetCom<ComButtonEnbale>("EnableJoin");
            mInspireMax = mBind.GetGameObject("InspireMax");
            mJoinRedPoint = mBind.GetCom<Image>("JoinRedPoint");
            mInspireIcon = mBind.GetCom<Image>("InspireIcon");
            mInspireCount = mBind.GetCom<Text>("InspireCount");
            mLeaderRewardPos = mBind.GetGameObject("LeaderRewardPos");
            mMemberRewardPos = mBind.GetGameObject("MemberRewardPos");
            mStatue = mBind.GetCom<Button>("Statue");
            mStatue.onClick.AddListener(_onStatueButtonClick);
            mViceStatue = mBind.GetCom<Button>("ViceStatue");
            mViceStatue.onClick.AddListener(_onViceStatueButtonClick);
            rankList = mBind.GetCom<Button>("rankList");
            rankList.SafeSetOnClickListener(() => 
            {
                GuildDataManager.GetInstance().RequestGuildManorWeekRanklist();
            });
            dailyAward = mBind.GetCom<Button>("dailyAward");
            dailyAward.SafeSetOnClickListener(() =>
            {
                GuildDataManager.GetInstance().SendWorldGuildGetTerrDayRewardReq();
            });
            dailyAwardRedPoint = mBind.GetGameObject("dailyAwardRedPoint");
            leftTime = mBind.GetCom<Text>("leftTime");
            leftTimeRoot = mBind.GetGameObject("leftTimeRoot");
            statue = mBind.GetCom<Button>("statue");
            statue.SafeSetOnClickListener(() =>
            {
                ClientSystemManager.GetInstance().OpenFrame<GuildManorOwnerAttrAddUpShowFrame>();
            });
            inspireRoot = mBind.GetGameObject("inspireRoot");
        }

        protected override void _unbindExUI()
        {
            mDetail.onClick.RemoveListener(_onDetailButtonClick);
            mDetail = null;
            mAttackCity.onClick.RemoveListener(_onAttackCityButtonClick);
            mAttackCity = null;
            mInspireLevel = null;
            mCurManorName = null;
            mTargetManorName = null;
            mCurAttr = null;
            mInspire.onClick.RemoveListener(_onInspireButtonClick);
            mInspire = null;
            mJoin.onClick.RemoveListener(_onJoinButtonClick);
            mJoin = null;
            mEnableInspire = null;
            mEnableJoin = null;
            mInspireMax = null;
            mJoinRedPoint = null;
            mInspireIcon = null;
            mInspireCount = null;
            mLeaderRewardPos = null;
            mMemberRewardPos = null;
            mStatue.onClick.RemoveListener(_onStatueButtonClick);
            mStatue = null;
            mViceStatue.onClick.RemoveListener(_onViceStatueButtonClick);
            mViceStatue = null;
            rankList = null;
            dailyAward = null;
            dailyAwardRedPoint = null;
            leftTime = null;
            leftTimeRoot = null;
            statue = null;
            inspireRoot = null;
        }
        #endregion
        int GetActivityNextOpenStamp()
        {
            if(signUpTimeInfos == null)
            {
                return 0;
            }

            DateTime now = TimeUtility.GetDateTimeByTimeStamp((int)TimeManager.GetInstance().GetServerTime());
            DateTime temp = now;
            for (int i = 0; i <= 7; i++)
            {
                temp = now.AddDays(i);
                TimeInfo timeInfo = signUpTimeInfos.Find((t) => 
                {
                    return temp.DayOfWeek == (DayOfWeek)(t.day % 7);
                });
                if(timeInfo != null)
                {
                    DateTime end = new DateTime(temp.Year, temp.Month, temp.Day, timeInfo.hour, timeInfo.minute, timeInfo.second);
                    if (Function.ConvertDateTimeInt(end) >= (double)TimeManager.GetInstance().GetServerTime())
                    {
                    return (int)Function.ConvertDateTimeInt(end);
                    }                        
                }
            }
            return 0;
        }
        string GetLeftTime(int end,int now)
        {
            int LeftTime = end - now;
            if (LeftTime < 0)
                LeftTime = 0;
            int Day = LeftTime / (24 * 60 * 60);
            LeftTime -= Day * 24 * 60 * 60;
            int Hour = LeftTime / (60 * 60);
            LeftTime -= Hour * 60 * 60;
            int Minute = LeftTime / 60;
            LeftTime -= Minute * 60;
            if(Day > 0)
            {
                return TR.Value("guild_battle_activity_time_format1", Day, Hour);
            }
            else
            {
                return TR.Value("guild_battle_activity_time_format2", Hour,Minute,LeftTime);
            }
            return string.Empty;
        }
        void InitSignUpTimeInfos()
        {
            signUpTimeInfos = new List<TimeInfo>();
            if(signUpTimeInfos == null)
            {
                return;
            }
            Dictionary<int, object> dicts = TableManager.instance.GetTable<GuildBattleTimeTable>();
            if (dicts != null)
            {
                var iter = dicts.GetEnumerator();
                while (iter.MoveNext())
                {
                    GuildBattleTimeTable adt = iter.Current.Value as GuildBattleTimeTable;
                    if (adt == null)
                    {
                        continue;
                    }
                    if(!(adt.Type == GuildBattleTimeTable.eType.GBT_NORMAL && adt.Status == GuildBattleTimeTable.eStatus.GBS_ENROLL && adt.IsOpen == 1))
                    {
                        continue;
                    }
                    TimeInfo timeInfo = new TimeInfo();
                    if(timeInfo == null)
                    {
                        continue;
                    }
                    timeInfo.day = adt.Week;
                    var times = adt.Time.Split(':');
                    if (times == null)
                    {
                        continue;
                    }
                    if (times.Length != 3)
                    {
                        continue;
                    }
                    timeInfo.hour = Utility.ToInt(times[0]);
                    timeInfo.minute = Utility.ToInt(times[1]);
                    timeInfo.second = Utility.ToInt(times[2]);
                    signUpTimeInfos.Add(timeInfo);
                }
            }
            signUpTimeInfos.Sort((a, b) => 
            {
                if(a == null || b == null)
                {
                    return 0;
                }
                return a.day.CompareTo(b.day);
            });
        }   
        void UpdateActivtyLeftTimeTip()
        {
            int activityTimeStamp = 0;

            var state = GuildDataManager.GetInstance().GetGuildBattleState();

            if (GuildDataManager.GetInstance().GetGuildBattleType() == GuildBattleType.GBT_CROSS 
                || GuildDataManager.GetInstance().GetGuildBattleType() == GuildBattleType.GBT_CHALLENGE)
            {
                state = EGuildBattleState.Invalid;
            }

            if (state == EGuildBattleState.Invalid)
            {
                activityTimeStamp = GetActivityNextOpenStamp();
                //activityTimeStamp = (int)GuildDataManager.GetInstance().GetGuildBattleStateEndTime();
                leftTime.SafeSetText(TR.Value("guild_battle_activity_open_time_tip", GetLeftTime(activityTimeStamp, (int)TimeManager.GetInstance().GetServerTime())));
            }
            else if (state == EGuildBattleState.Signup)
            {
                activityTimeStamp = (int)GuildDataManager.GetInstance().GetGuildBattleStateEndTime();
                leftTime.SafeSetText(TR.Value("guild_battle_activity_sign_up_tip", GetLeftTime((int)GuildDataManager.GetInstance().GetGuildBattleStateEndTime(), (int)TimeManager.GetInstance().GetServerTime())));
            }
            else if (state == EGuildBattleState.Preparing)
            {
                activityTimeStamp = (int)GuildDataManager.GetInstance().GetGuildBattleStateEndTime();
                leftTime.SafeSetText(TR.Value("guild_battle_activity_prepare_tip", GetLeftTime((int)GuildDataManager.GetInstance().GetGuildBattleStateEndTime(), (int)TimeManager.GetInstance().GetServerTime())));
            }
            else if (state == EGuildBattleState.Firing)
            {
                activityTimeStamp = (int)GuildDataManager.GetInstance().GetGuildBattleStateEndTime();
                leftTime.SafeSetText(TR.Value("guild_battle_activity_firing_tip", GetLeftTime((int)GuildDataManager.GetInstance().GetGuildBattleStateEndTime(), (int)TimeManager.GetInstance().GetServerTime())));
            }            
           
            leftTimeRoot.CustomActive(activityTimeStamp > 0);
        }
        public override bool IsNeedUpdate()
        {
            return true;
        }
        protected override void _OnUpdate(float timeElapsed)
        {
            UpdateActivtyLeftTimeTip();
        }

        #region Callback
        private void _onDetailButtonClick()
        {
            ClientSystemManager.GetInstance().OpenFrame<GuildInspireDetailFrame>(FrameLayer.Middle);
        }

        private void _onAttackCityButtonClick()
        {
            int iTerritoryID = 1;
            ClientSystemManager.GetInstance().OpenFrame<GuildAttackCityFrame>(FrameLayer.Middle, iTerritoryID);
        }

        private void _onInspireButtonClick()
        {
            GuildDataManager.GetInstance().SendInspire();
        }

        private void _onJoinButtonClick()
        {
            if (TeamDataManager.GetInstance().HasTeam())
            {
                SystemNotifyManager.SystemNotify(1104);
                return;
            }

            ClientSystemTown town = ClientSystemManager.GetInstance().GetCurrentSystem() as ClientSystemTown;
            if (town != null && GuildDataManager.GetInstance().HasSelfGuild())
            {
                int nManorID = GuildDataManager.GetInstance().myGuild.nTargetManorID;
                GuildTerritoryTable manorTable = TableManager.GetInstance().GetTableItem<GuildTerritoryTable>(nManorID);
                if (manorTable != null)
                {
                    if (town.CurrentSceneID != manorTable.SceneID)
                    {
                        town.ChangeScene(manorTable.SceneID, 0,town.CurrentSceneID, 0);
                        ClientSystemManager.GetInstance().CloseFrame<PkWaitingRoom>();
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildCloseMainFrame);
                        GuildDataManager.GetInstance().SetGuildBattleEnterRedPoint(false);
                    }
                    else
                    {
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildCloseMainFrame);
                    }
                }
            }
            frameMgr.CloseFrame<GuildManorFrame>();
        }

        private void _onStatueButtonClick()
        {
            if(ClientSystemManager.GetInstance().IsFrameOpen<GuildStatueFrame>())
            {
                ClientSystemManager.GetInstance().CloseFrame<GuildStatueFrame>();
            }

            ClientSystemManager.GetInstance().OpenFrame<GuildStatueFrame>(FrameLayer.Middle, StatueType.TownStatue);
        }

        private void _onViceStatueButtonClick()
        {
            if (ClientSystemManager.GetInstance().IsFrameOpen<GuildStatueFrame>())
            {
                ClientSystemManager.GetInstance().CloseFrame<GuildStatueFrame>();
            }

            ClientSystemManager.GetInstance().OpenFrame<GuildStatueFrame>(FrameLayer.Middle, StatueType.ViceTownStatue);
        }
        #endregion
    }
}
