using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Protocol;

namespace GameClient
{
    //打开公会界面之后打开的二级界面。这里有些枚举值是不要的，暂时不删除，保证和之前的代码对应上
    public enum EOpenGuildMainFramData
    {
        Invalid = -1,

        OpenBaseInfo = 0,
        OpenMembers,
        OpenBuilding,
        OpenBuildingTable,
        OpenBuildingWelfare,
        OpenBuildingStatue,
        OpenBuildingSkill,
        OpenBuildingShop,
        OpenManor,
        OpenGuildStore,
        OpenGuildCrossManor,
        OpenGuildRedPacket,
        OpenGuildEmblemLevel,//荣耀圣殿
        OpenGuildBenefits,//福利
        OpenGuildActivity,

        Count,
    }

    public class GuildMainFrame : ClientFrame
    {

        private ButtonEx mOpenEnterGuildLvBtn;

        private ButtonEx mMegerGuideBtn;
        private UIGray mGuildMergeGray;
        private GameObject mGuildMergeRedPointGo;

        private ButtonEx mGuildLogBtn;

        private ButtonEx mGuildRedPackageBtn;

        private ButtonEx mGuildListBtn;

        private ButtonEx mActivityCenterBtn;
        private GameObject mActivtiyCenterRedPointGo;

        private Text mEnterGuildLvTxt;

        private Button mGuildMailBtn;

        private Button mGuildManorBtn;
        private Text mGuildManorNameTxt;
        private Text mGuildManorStateTxt;

        private GuildBattleType mGuildBattleType;
        public static void OpenLinkFrame(string a_strParam)
        {
            if (!GuildDataManager.GetInstance().HasSelfGuild())
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("guild_have_no_guild"));
                return;
            }
            string[] arrParams = a_strParam.Split('|');
            if (arrParams.Length > 0)
            {
                int value = 0;
                int.TryParse(arrParams[0], out value);
                EOpenGuildMainFramData data = (EOpenGuildMainFramData)value;
                ClientSystemManager.GetInstance().OpenFrame<GuildMainFrame>(FrameLayer.Middle, data);
            }

        }
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Guild/GuildMainFrame";
        }

        protected override void _bindExUI()
        {
            mOpenEnterGuildLvBtn = mBind.GetCom<ButtonEx>("EnterGuildeLvBtn");
            mOpenEnterGuildLvBtn.SafeAddOnClickListener(_OnOpenEnterGuildLvBtnClick);

            mMegerGuideBtn = mBind.GetCom<ButtonEx>("MegerGuildBtn");
            if (mMegerGuideBtn != null)
            {
                mGuildMergeGray = mMegerGuideBtn.GetComponent<UIGray>();
            }
            mMegerGuideBtn.SafeAddOnClickListener(_OnMegerGuideBtnClick);

            mGuildLogBtn = mBind.GetCom<ButtonEx>("GuildLogBtn");
            mGuildLogBtn.SafeAddOnClickListener(_OnGuildLogBtnClick);

            mGuildRedPackageBtn = mBind.GetCom<ButtonEx>("GuildRedPackageBtn");
            mGuildRedPackageBtn.SafeAddOnClickListener(_OnGuildRedPackageBtnClick);

            mGuildListBtn = mBind.GetCom<ButtonEx>("GuildListBtn");
            mGuildListBtn.SafeAddOnClickListener(_OnGuildListBtnClick);

            mActivityCenterBtn = mBind.GetCom<ButtonEx>("ActivityCenterBtn");
            mActivityCenterBtn.SafeAddOnClickListener(_OnActivityCenterBtnClick);

            mGuildMergeRedPointGo = mBind.GetGameObject("GuildMergeRedPoint");

            mActivtiyCenterRedPointGo = mBind.GetGameObject("ActivtiyCenterRedPoint");

            mEnterGuildLvTxt= mBind.GetCom<Text>("EnterGuildLvTxt");

            mGuildMailBtn = mBind.GetCom<Button>("GuildMailBtn");
            mGuildMailBtn.SafeAddOnClickListener(_OnGuildMailBtnClick);

            mGuildManorBtn = mBind.GetCom<Button>("GuildManorBtn");
            mGuildManorBtn.SafeAddOnClickListener(_OnGuildManorBtnClick);

            mGuildManorNameTxt = mBind.GetCom<Text>("GuildManorNameTxt");
            mGuildManorStateTxt = mBind.GetCom<Text>("GuildManorStateTxt");
        }
        
        protected override void _unbindExUI()
        {
            mOpenEnterGuildLvBtn.SafeRemoveOnClickListener(_OnOpenEnterGuildLvBtnClick);
            mOpenEnterGuildLvBtn = null;

            mMegerGuideBtn.SafeRemoveOnClickListener(_OnMegerGuideBtnClick);
            mMegerGuideBtn = null;

            mGuildLogBtn.SafeRemoveOnClickListener(_OnGuildLogBtnClick);
            mGuildLogBtn = null;

            mGuildRedPackageBtn.SafeRemoveOnClickListener(_OnGuildRedPackageBtnClick);
            mGuildRedPackageBtn = null;

            mGuildListBtn.SafeRemoveOnClickListener(_OnGuildListBtnClick);
            mGuildListBtn = null;

            mActivityCenterBtn.SafeRemoveOnClickListener(_OnActivityCenterBtnClick);
            mActivityCenterBtn = null;

            mGuildMergeGray = null;

            mGuildMergeRedPointGo = null;

            mActivtiyCenterRedPointGo = null;

            mEnterGuildLvTxt = null;

            mGuildMailBtn.SafeRemoveOnClickListener(_OnGuildMailBtnClick);
            mGuildMailBtn = null;

            mGuildManorBtn.SafeRemoveOnClickListener(_OnGuildManorBtnClick);
            mGuildManorBtn = null;

            mGuildManorNameTxt = null;
            mGuildManorStateTxt = null;
        }

        protected override void _OnOpenFrame()
        {
            if(userData!=null)
            {
                EOpenGuildMainFramData data = (EOpenGuildMainFramData)userData;
                switch (data)
                {
                    case EOpenGuildMainFramData.OpenMembers:
                        OpenGuildMemberFrameData d = new OpenGuildMemberFrameData();
                        ClientSystemManager.GetInstance().OpenFrame<GuildMemberFrame>(FrameLayer.Middle, d);
                        break;
                    case EOpenGuildMainFramData.OpenManor:
                        ClientSystemManager.GetInstance().OpenFrame<GuildActivityFrame>(FrameLayer.Middle);
                        break;
                    case EOpenGuildMainFramData.OpenGuildStore:
                        ClientSystemManager.GetInstance().OpenFrame<GuildStoreFrame>(FrameLayer.Middle);
                        break;
                    case EOpenGuildMainFramData.OpenGuildCrossManor:
                        ClientSystemManager.GetInstance().OpenFrame<GuildActivityFrame>(FrameLayer.Middle);
                        break;
                    case EOpenGuildMainFramData.OpenGuildRedPacket:
                        ClientSystemManager.GetInstance().OpenFrame<GuildRedPacketFrame>(FrameLayer.Middle);
                        break;
                    case EOpenGuildMainFramData.OpenGuildEmblemLevel:
                        ClientSystemManager.GetInstance().OpenFrame<GuildEmblemUpFrame>(FrameLayer.Middle);
                        break;
                    case EOpenGuildMainFramData.OpenGuildActivity:
                        ClientSystemManager.GetInstance().OpenFrame<GuildActivityFrame>(FrameLayer.Middle);
                        break;
                    case EOpenGuildMainFramData.OpenBuildingShop:
                        int nShopLevel = GuildDataManager.GetInstance().myGuild.dictBuildings[GuildBuildingType.SHOP].nLevel;
                        ProtoTable.GuildBuildingTable buildingTable = TableManager.GetInstance().GetTableItem<ProtoTable.GuildBuildingTable>(nShopLevel);
                        if (buildingTable != null)
                        {
                            ShopNewDataManager.GetInstance().OpenShopNewFrame(buildingTable.ShopId);
                        }
                        break;
                }
            }
            _BindUIEvt();
            _UpdateSetJoinLvInfo();
            _UpdatePermission();
            _UpdateRedPoint();
            _UpdateEnterGuildLv();
            _UpdateGuildManorInfo();
        }
        protected override void _OnCloseFrame()
        {
            _UnBindUIEvt();
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.GuildMainFrameClose);
            RedPointDataManager.GetInstance().NotifyRedPointChanged(ERedPoint.GuildMain);
        }
        
        private void _BindUIEvt()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildRequestDismissSuccess, _OnRequestDismissGuildSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildRequestCancelDismissSuccess, _OnRequestCancelDismissGuildSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.RedPointChanged, _OnRedPointChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildJoinLvUpdate,_OnGuildEnterLvUpdate);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildBattleStateChanged, _OnGuildBattleStateUpdated);
        }

       
        private void _UnBindUIEvt()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildRequestDismissSuccess, _OnRequestDismissGuildSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildRequestCancelDismissSuccess, _OnRequestCancelDismissGuildSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.RedPointChanged, _OnRedPointChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildJoinLvUpdate, _OnGuildEnterLvUpdate);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildBattleStateChanged, _OnGuildBattleStateUpdated);
        }

        #region btn callback
        private void _OnGuildListBtnClick()
        {
            ClientSystemManager.GetInstance().OpenFrame<GuildListFrame>(FrameLayer.Middle);
        }

        private void _OnGuildRedPackageBtnClick()
        {
            ClientSystemManager.GetInstance().OpenFrame<GuildRedPacketFrame>(FrameLayer.Middle);
        }

        private void _OnActivityCenterBtnClick()
        {
            ClientSystemManager.GetInstance().OpenFrame<GuildActivityFrame>(FrameLayer.Middle);
        }

        private void _OnGuildLogBtnClick()
        {
            ClientSystemManager.GetInstance().OpenFrame<GuildJournalFrame>(FrameLayer.Middle);
        }

        private void _OnMegerGuideBtnClick()
        {
            GuildMyData guildMyDat = GuildDataManager.GetInstance().myGuild;
            if (guildMyDat != null)
            {
                if (guildMyDat.prosperity == (uint)EGuildProsperityState.Mid)
                {
                    RedPointDataManager.GetInstance().ClearRedPoint(ERedPoint.GuildMerger);
                    ClientSystemManager.GetInstance().OpenFrame<GuildListFrame>(FrameLayer.Middle, EShowGuildType.CanMerged);
                }
                else
                {
                    SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("guildmerge_lackselect"));
                }

            }
        }

        private void _OnOpenEnterGuildLvBtnClick()
        {
            if (!GuildDataManager.GetInstance().HasPermission(EGuildPermission.ChangeJoinLv))
            {
                return;
            }
            ClientSystemManager.GetInstance().OpenFrame<GuildSetJoinLvFrame>();
        }
        private void _OnGuildMailBtnClick()
        {
            GuildCommonModifyData data = new GuildCommonModifyData();
            data.bHasCost = false;
            data.nMaxWords = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_GUILD_MAIL_MAX_WORLDS).Value;
            data.onOkClicked = (string a_strValue) =>
            {
                GuildDataManager.GetInstance().SendMail(a_strValue);
            };
            data.strTitle = TR.Value("guild_send_mail");
            data.strEmptyDesc = TR.Value("guild_send_mail_desc");
            data.eMode = EGuildCommonModifyMode.Long;
            frameMgr.OpenFrame<GuildCommonModifyFrame>(FrameLayer.Middle, data);
        }

        /// <summary>
        /// 公会领地 跨服公会领地
        /// </summary>
        private void _OnGuildManorBtnClick()
        {
            if(mGuildBattleType==GuildBattleType.GBT_NORMAL)
            {
                ClientSystemManager.GetInstance().OpenFrame<GuildManorFrame>();
            }
            else if (mGuildBattleType == GuildBattleType.GBT_CROSS)
            {
                ClientSystemManager.GetInstance().OpenFrame<GuildCrossManorFrame>();
            }
        }
        #endregion

        #region UI event
        private void _OnRequestDismissGuildSuccess(UIEvent a_event)
        {
            _UpdatePermission();
        }

        private void _OnRequestCancelDismissGuildSuccess(UIEvent a_event)
        {
            _UpdatePermission();
        }

        private void _OnRedPointChanged(UIEvent uiEvent)
        {
            _UpdateRedPoint();
        }


        private void _OnGuildEnterLvUpdate(UIEvent uiEvent)
        {
            _UpdateEnterGuildLv();
        }

        private void _OnGuildBattleStateUpdated(UIEvent uiEvent)
        {
            _UpdateGuildManorInfo();
        }

        #endregion

        private void _UpdateSetJoinLvInfo()
        {
            mOpenEnterGuildLvBtn.CustomActive(GuildDataManager.GetInstance().HasPermission(EGuildPermission.ChangeJoinLv));

        }
        private void _UpdateRedPoint()
        {
            mGuildMergeRedPointGo.CustomActive(RedPointDataManager.GetInstance().HasRedPoint(ERedPoint.GuildMerger));
            mActivtiyCenterRedPointGo.CustomActive(RedPointDataManager.GetInstance().HasRedPoint(ERedPoint.GuildManor)
               || RedPointDataManager.GetInstance().HasRedPoint(ERedPoint.GuildCrossManor)
               || RedPointDataManager.GetInstance().HasRedPoint(ERedPoint.GuildTerrDayReward));
        }
        
        private void _UpdateEnterGuildLv()
        {
            if (!GuildDataManager.GetInstance().HasSelfGuild()) return;
            mEnterGuildLvTxt.SafeSetText(GuildDataManager.GetInstance().myGuild.nJoinLevel.ToString());
        }

        private void _UpdatePermission()
        {
            if (ServerSceneFuncSwitchManager.GetInstance().IsServiceTypeSwitchOpen(ServiceType.SERVICR_GUILDMERGER))
            {
                if (GuildDataManager.GetInstance().IsCanGuildMerger())
                {
                    mMegerGuideBtn.CustomActive(GuildDataManager.GetInstance().HasPermission(EGuildPermission.GuildMeger));
                }
                else
                {
                    mMegerGuideBtn.CustomActive(false);
                }
            }
            else
            {
                mMegerGuideBtn.CustomActive(false);
            }

            if (mGuildMergeGray != null)
            {
                if (GuildDataManager.GetInstance().HasSelfGuild())
                {
                    GuildMyData guildMyData = GuildDataManager.GetInstance().myGuild;
                    if (guildMyData.prosperity != (uint)EGuildProsperityState.Mid)
                    {
                        mGuildMergeGray.enabled = true;
                    }
                    else
                    {
                        mGuildMergeGray.enabled = false;
                    }
                }
            }
        }


        private void _UpdateGuildManorInfo()
        {
            mGuildBattleType = GuildDataManager.GetInstance().GetGuildBattleType();
            EGuildBattleState guildBattleState= GuildDataManager.GetInstance().GetGuildBattleState();

            if (guildBattleState == EGuildBattleState.Invalid)
            {
                mGuildManorBtn.CustomActive(false);
            }
            else
            {
                mGuildManorBtn.CustomActive(true);
            }

            if (mGuildBattleType == GuildBattleType.GBT_NORMAL)
            {
                mGuildManorNameTxt.SafeSetText(TR.Value("GuildManorName"));
            }
            else if(mGuildBattleType == GuildBattleType.GBT_CROSS)
            {
                mGuildManorNameTxt.SafeSetText(TR.Value("GuildCrossManorName"));
            }
            

            if (guildBattleState == EGuildBattleState.Signup)
            {
                mGuildManorStateTxt.SafeSetText(TR.Value("GuildSingUp"));
            }
            else if (guildBattleState == EGuildBattleState.Preparing)
            {
                mGuildManorStateTxt.SafeSetText(TR.Value("GuildPreparing"));
            }
            else if (guildBattleState == EGuildBattleState.Firing)
            {
                mGuildManorStateTxt.SafeSetText(TR.Value("GuildFiring"));
            }
            else if (guildBattleState == EGuildBattleState.LuckyDraw)
            {
                mGuildManorStateTxt.SafeSetText(TR.Value("GuildLuckyDraw"));
            }
            
        }


    }
}
