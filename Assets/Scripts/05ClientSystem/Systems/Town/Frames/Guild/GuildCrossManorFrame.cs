using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProtoTable;
using UnityEngine.Assertions;
using Protocol;
using Network;

namespace GameClient
{
    class GuildCrossManorFrame : ClientFrame
    {
        string AttackCityEffectPath = "Effects/Scene_effects/EffectUI/EffUI_chuizi";

        int iAreaNum = 0;

        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Guild/GuildCrossManor";
        }

        protected sealed override void _OnOpenFrame()
        {
            _RegisterUIEvent();

            var tabledata = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_CROSS_MANOR_NUM);
            iAreaNum = tabledata.Value;

            for (int i = 8; i < 8 + iAreaNum; ++i)
            {
                GameObject obj = Utility.FindGameObject(string.Format("Map/Manor{0}", i));
                if (obj == null)
                {
                    continue;
                }

                obj.CustomActive(false);

                int nID = i;
                GuildTerritoryTable tableData = _GetTerritoryTableData(nID);

                if(tableData == null)
                {
                    continue;
                }

                if(tableData.IsOpen != 1)
                {
                    continue;
                }

                obj.CustomActive(true);

                Button btnFunc = Utility.GetComponetInChild<Button>(obj, "Func");
                if(btnFunc!=null)
                {
                    btnFunc.onClick.RemoveAllListeners();
                    btnFunc.onClick.AddListener(() =>
                    {
                        GuildDataManager.GetInstance().RequestManorInfo(nID);
                    });
                }
             

                if(i != 8)
                {
                    Text ManorName = Utility.GetComponetInChild<Text>(obj, "Name/Text");

                    if(ManorName != null)
                    {
                        ManorName.text = tableData.Name;
                    }           
                }

                if (GuildDataManager.GetInstance().HasTargetManor() == false &&
                    GuildDataManager.GetInstance().GetGuildBattleType() == GuildBattleType.GBT_CROSS && 
                    GuildDataManager.GetInstance().GetGuildBattleState() == EGuildBattleState.Signup &&
                    GuildDataManager.GetInstance().HasPermission(EGuildPermission.StartGuildCrossBattle))
                {
                    Utility.FindGameObject(obj, "ClickToSignup").CustomActive(true);
                }
                else
                {
                    Utility.FindGameObject(obj, "ClickToSignup").CustomActive(false);
                }

                Utility.FindGameObject(obj, "AlreadySignup").CustomActive(nID == GuildDataManager.GetInstance().myGuild.nTargetCrossManorID &&
                    GuildDataManager.GetInstance().GetGuildBattleState() >= EGuildBattleState.Signup &&
                    GuildDataManager.GetInstance().GetGuildBattleState() <= EGuildBattleState.Firing);

                GuildTerritoryBaseInfo baseinfo = GuildDataManager.GetInstance().GetGuildTerritoryBaseInfo(nID);
                if(baseinfo != null)
                {
                    if (baseinfo.guildName != "")
                    {
                        Utility.GetComponetInChild<Text>(obj, "Owner").SafeSetText(string.Format("【{0}】", baseinfo.guildName));
                    }
                    else
                    {
                        Utility.GetComponetInChild<Text>(obj, "Owner").SafeSetText("");
                    }

                    if(baseinfo.serverName != "")
                    {
                        Utility.GetComponetInChild<Text>(obj, "ServerName").SafeSetText(string.Format("{0}", baseinfo.serverName));
                    }
                    else
                    {
                        Utility.GetComponetInChild<Text>(obj, "ServerName").SafeSetText("");
                    }
                }
            }

            _UpdateCurrentManaor();
            _UpdateTargetManaor();
            _UpdateJoin();
            _UpdateShowTitle();
            _UpdateLotteryShow();

            GuildDataManager.GetInstance().UpdateInspireInfo(ref mInspireLevel, ref mCurAttr, ref mInspireMax, ref mInspire, ref mInspireIcon, ref mInspireCount, ref mEnableInspire, GuildBattleOpenType.GBOT_CROSS);

            GuildDataManager.GetInstance().SetGuildBattleSignUpRedPoint(false);
        }

        protected sealed override void _OnCloseFrame()
        {
            _UnRegisterUIEvent();
        }

        void _RegisterUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildInspireSuccess, _OnInspireSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildSignupSuccess, _OnSignupSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildManorInfoUpdated, _OnManorInfoUpdated);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildBaseInfoUpdated, _OnGuildBaseInfoUpdated);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildBattleStateChanged, _OnGuildBattleStateUpdated);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildManorOwnerUpdated, _OnManorOwnerUpdate);
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
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildLotteryResultRes, _OnGuildLotteryResultRes);
        }

        void _OnInspireSuccess(UIEvent a_event)
        {
            GuildDataManager.GetInstance().UpdateInspireInfo(ref mInspireLevel, ref mCurAttr, ref mInspireMax, ref mInspire, ref mInspireIcon, ref mInspireCount, ref mEnableInspire, GuildBattleOpenType.GBOT_CROSS);
        }

        void _OnSignupSuccess(UIEvent a_event)
        {
            _UpdateTargetManaor();
            _UpdateManor();

            GuildDataManager.GetInstance().UpdateInspireInfo(ref mInspireLevel, ref mCurAttr, ref mInspireMax, ref mInspire, ref mInspireIcon, ref mInspireCount, ref mEnableInspire, GuildBattleOpenType.GBOT_CROSS);

            ClientSystemManager.GetInstance().CloseFrame<GuildCrossManorInfoFrame>();
            SystemNotifyManager.SysNotifyTextAnimation(TR.Value("guild_manor_signup_success"));
        }

        void _OnManorInfoUpdated(UIEvent a_event)
        {
            GuildTerritoryBaseInfo data = a_event.Param1 as GuildTerritoryBaseInfo;
            if (data != null)
            {
                ClientSystemManager.GetInstance().OpenFrame<GuildCrossManorInfoFrame>(FrameLayer.Middle, data);
            }
        }

        void _OnGuildBaseInfoUpdated(UIEvent a_event)
        {
            _UpdateCurrentManaor();
            _UpdateTargetManaor();
            _UpdateManor();

            GuildDataManager.GetInstance().UpdateInspireInfo(ref mInspireLevel, ref mCurAttr, ref mInspireMax, ref mInspire, ref mInspireIcon, ref mInspireCount, ref mEnableInspire, GuildBattleOpenType.GBOT_CROSS);
        }

        void _OnGuildBattleStateUpdated(UIEvent a_event)
        {
            _UpdateJoin();
            _UpdateManor();
            _UpdateLotteryShow();

            GuildDataManager.GetInstance().UpdateInspireInfo(ref mInspireLevel, ref mCurAttr, ref mInspireMax, ref mInspire, ref mInspireIcon, ref mInspireCount, ref mEnableInspire, GuildBattleOpenType.GBOT_CROSS);
        }

        void _OnManorOwnerUpdate(UIEvent a_event)
        {
            _UpdateManor();
        }

        void _OnGuildLotteryResultRes(UIEvent a_event)
        {
            _UpdateLotteryShow();
        }

        void _UpdateCurrentManaor()
        {
            if (GuildDataManager.GetInstance().HasSelfGuild() && GuildDataManager.GetInstance().HasSelfCrossManor())
            {
                GuildTerritoryTable tableData = _GetTerritoryTableData(GuildDataManager.GetInstance().myGuild.nSelfCrossManorID);
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
                GuildTerritoryTable tableData = _GetTerritoryTableData(GuildDataManager.GetInstance().myGuild.nTargetCrossManorID);

                if (tableData != null)
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
                GuildDataManager.GetInstance().GetGuildBattleType() == GuildBattleType.GBT_CROSS &&
                (
                GuildDataManager.GetInstance().GetGuildBattleState() == EGuildBattleState.Preparing ||
                GuildDataManager.GetInstance().GetGuildBattleState() == EGuildBattleState.Firing
                );
            mEnableJoin.SetEnable(canBattle);

            mJoinRedPoint.gameObject.CustomActive(GuildDataManager.GetInstance().GetGuildBattleType() == GuildBattleType.GBT_CROSS && RedPointDataManager.GetInstance().HasRedPoint(ERedPoint.GuildBattleEnter));
        }

        void _UpdateShowTitle()
        {
            GuildTerritoryTable tableData = _GetTerritoryTableData(8);

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
            for (int i = 8; i < 8 + iAreaNum; ++i)
            {
                GameObject obj = Utility.FindGameObject(string.Format("Map/Manor{0}", i), false);
                if (obj == null)
                {
                    continue;
                }

                int nID = i;
                GuildTerritoryTable tableData = _GetTerritoryTableData(nID);

                if(tableData == null)
                {
                    continue;
                }

                if (tableData.IsOpen != 1)
                {
                    obj.CustomActive(false);
                    continue;
                }

                obj.CustomActive(true);

                if (GuildDataManager.GetInstance().HasTargetManor() == false &&
                    GuildDataManager.GetInstance().GetGuildBattleType() == GuildBattleType.GBT_CROSS &&
                    GuildDataManager.GetInstance().GetGuildBattleState() == EGuildBattleState.Signup && 
                    GuildDataManager.GetInstance().HasPermission(EGuildPermission.StartGuildCrossBattle))
                {
                    Utility.FindGameObject(obj, "ClickToSignup").SetActive(true);
                }
                else
                {
                    Utility.FindGameObject(obj, "ClickToSignup").SetActive(false);
                }

                Utility.FindGameObject(obj, "AlreadySignup").SetActive(nID == GuildDataManager.GetInstance().myGuild.nTargetCrossManorID);

                GuildTerritoryBaseInfo baseinfo = GuildDataManager.GetInstance().GetGuildTerritoryBaseInfo(nID);
                if (baseinfo != null)
                {
                    if (baseinfo.guildName != "")
                    {
                        Utility.GetComponetInChild<Text>(obj, "Owner").text = string.Format("【{0}】", baseinfo.guildName);
                    }
                    else
                    {
                        Utility.GetComponetInChild<Text>(obj, "Owner").text = "";
                    }

                    if (baseinfo.serverName != "")
                    {
                        Utility.GetComponetInChild<Text>(obj, "ServerName").text = string.Format("{0}", baseinfo.serverName);
                    }
                    else
                    {
                        Utility.GetComponetInChild<Text>(obj, "ServerName").text = "";
                    }
                }
            }
        }

        GuildTerritoryTable _GetTerritoryTableData(int a_nID, bool a_bShowError = true)
        {
            GuildTerritoryTable tableData = TableManager.GetInstance().GetTableItem<GuildTerritoryTable>(a_nID);
            if (tableData == null)
            {
                return null;
            }
            return tableData;
        }

        #region ExtraUIBind
        private Button mDetail = null;
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

        protected sealed override void _bindExUI()
        {
            mDetail = mBind.GetCom<Button>("Detail");
            mDetail.onClick.AddListener(_onDetailButtonClick);
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
        }

        protected sealed override void _unbindExUI()
        {
            mDetail.onClick.RemoveListener(_onDetailButtonClick);
            mDetail = null;
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
        }
        #endregion

        #region Callback
        private void _onDetailButtonClick()
        {
            ClientSystemManager.GetInstance().OpenFrame<GuildInspireDetailFrame>(FrameLayer.Middle);
        }

        private void _onInspireButtonClick()
        {
            GuildDataManager.GetInstance().SendInspire();
        }

        private void _onJoinButtonClick()
        {
            if(ServerSceneFuncSwitchManager.GetInstance().IsTypeFuncLock(ServiceType.SERVICE_GUILD_CROSS_BATTLE))
            {
                SystemNotifyManager.SysNotifyFloatingEffect("跨服公会战系统目前已关闭");
                return;
            }

            if (TeamDataManager.GetInstance().HasTeam())
            {
                SystemNotifyManager.SystemNotify(1104);
                return;
            }

            ClientSystemTown town = ClientSystemManager.GetInstance().GetCurrentSystem() as ClientSystemTown;
            if (town != null && GuildDataManager.GetInstance().HasSelfGuild())
            {
                int nManorID = GuildDataManager.GetInstance().myGuild.nTargetCrossManorID;

                GuildTerritoryTable manorTable = TableManager.GetInstance().GetTableItem<GuildTerritoryTable>(nManorID);
                if (manorTable != null)
                {
                    if (town.CurrentSceneID != manorTable.SceneID)
                    {
                        town.ChangeScene(manorTable.SceneID, 0, town.CurrentSceneID, 0);
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
        }
        #endregion
    }
}
