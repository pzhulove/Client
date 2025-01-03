using GameClient;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class GuildeBaseInfoView : MonoBehaviour
    {
        [SerializeField]
        private Text mLvTxt;

        [SerializeField]
        private Text mNameTxt;

        [SerializeField]
        private Text mNoticeContentTxt;

        [SerializeField]
        private Button mModifyNoticeBtn;

        [SerializeField]
        private Text mPresidentTxt;

        [SerializeField]
        private Text mConstrcutionMoneyTxt;

        [SerializeField]
        private Text mMemberNumTxt;

        [SerializeField]
        private Text mLastWeekProsTxt;

        [SerializeField]
        private Text mThisWeekProsTxt;

        [SerializeField]
        private Button mOpenMemberListBtn;

        [SerializeField]
        private Button mEnterSceneBtn;

        [SerializeField]
        private GameObject mGuildListRedPointGo;

        //[SerializeField]
        //private Button mGuildTipsBtn;

        private void Awake()
        {
            _BindEvt();
            _Init();
        }

        private void OnDestroy()
        {
            _UnBindEvt();
        }
        private void _Init()
        {
            if (GuildDataManager.GetInstance().HasSelfGuild())
            {
                GuildMyData guildMy = GuildDataManager.GetInstance().myGuild;
                mLvTxt.SafeSetText(guildMy.nLevel.ToString());
                mConstrcutionMoneyTxt.SafeSetText(guildMy.nFund.ToString());
                mPresidentTxt.SafeSetText(TR.Value("guild_leader", guildMy.leaderData.strName));
                mMemberNumTxt.SafeSetText(string.Format("{0}/{1}", guildMy.nMemberCount, guildMy.nMemberMaxCount));
                mNameTxt.SafeSetText(guildMy.strName);
                mLastWeekProsTxt.SafeSetText(guildMy.lastWeekFunValue.ToString());
                mThisWeekProsTxt.SafeSetText(guildMy.thisWeekFunValue.ToString());

                _UpdateNotice();
                _UpdateRedPoint();
                _UpdateGuildTips();
            }
            else
            {
                Logger.LogError("不存在公会");
            }
        }

        private void _BindEvt()
        {
            mOpenMemberListBtn.SafeAddOnClickListener(_OnOpenMemberListBtnClick);
            mEnterSceneBtn.SafeAddOnClickListener(_OnEnterSceneBtnClick);
            mModifyNoticeBtn.SafeAddOnClickListener(_OnModifNoticeBtnClick);
            //mGuildTipsBtn.SafeAddOnClickListener(_OnGuildTipsBtnClick);

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildChangeNoticeSuccess, _OnChangeNoticeSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.RedPointChanged, _OnRedPointChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GuildDungeonSyncActivityState, _OnSyncActivityState);
        }

       
        private void _UnBindEvt()
        {
            mOpenMemberListBtn.SafeRemoveOnClickListener(_OnOpenMemberListBtnClick);
            mEnterSceneBtn.SafeRemoveOnClickListener(_OnEnterSceneBtnClick);
            mModifyNoticeBtn.SafeRemoveOnClickListener(_OnModifNoticeBtnClick);
            //mGuildTipsBtn.SafeRemoveOnClickListener(_OnGuildTipsBtnClick);

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildChangeNoticeSuccess, _OnChangeNoticeSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.RedPointChanged, _OnRedPointChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GuildDungeonSyncActivityState, _OnSyncActivityState);
        }

     
        private void _UpdateNotice()
        {
            if (GuildDataManager.GetInstance().HasSelfGuild())
            {
                if (string.IsNullOrEmpty(GuildDataManager.GetInstance().myGuild.strNotice))
                {
                    mNoticeContentTxt.SafeSetText(TR.Value("guild_no_notice"));
                }
                else
                {
                    mNoticeContentTxt.SafeSetText(GuildDataManager.GetInstance().myGuild.strNotice);
                }
            }
        }

        private void _OnChangeNoticeSuccess(UIEvent a_event)
        {
            _UpdateNotice();
            ClientSystemManager.GetInstance().CloseFrame<GuildCommonModifyFrame>();
        }
        private void _OnRedPointChanged(UIEvent uiEvent)
        {
            _UpdateRedPoint();
        }

        private void _OnSyncActivityState(UIEvent uiEvent)
        {
            _UpdateGuildTips();
        }

        #region Btn callback
        private void _OnEnterSceneBtnClick()
        {
            GuildDataManager.GetInstance().SwitchToGuildScene();
            if (ClientSystemManager.GetInstance().IsFrameOpen<GuildMainFrame>())
            {
                ClientSystemManager.GetInstance().CloseFrame<GuildMainFrame>();
            }
        }
        private void _OnOpenMemberListBtnClick()
        {
            OpenGuildMemberFrameData data = new OpenGuildMemberFrameData();
            ClientSystemManager.GetInstance().OpenFrame<GuildMemberFrame>(FrameLayer.Middle, data);
        }

        private void _OnModifNoticeBtnClick()
        {
            GuildCommonModifyData data = new GuildCommonModifyData();
            data.bHasCost = false;
            data.nMaxWords = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_GUILD_NOTICE_MAX_WORDS).Value;
            data.onOkClicked = (string a_strValue) =>
            {
                GuildDataManager.GetInstance().ChangeNotice(a_strValue);
            };
            data.strTitle = TR.Value("guild_change_notice");
            data.strEmptyDesc = TR.Value("guild_change_notice_desc");
            data.strDefultContent = GuildDataManager.GetInstance().myGuild.strNotice;
            data.eMode = EGuildCommonModifyMode.Long;
            ClientSystemManager.GetInstance().OpenFrame<GuildCommonModifyFrame>(FrameLayer.Middle, data);
        }

        private void _OnGuildTipsBtnClick()
        {
            //mGuildTipsBtn.CustomActive(false);
        }

        #endregion


        private void _UpdateRedPoint()
        {
            mGuildListRedPointGo.CustomActive(RedPointDataManager.GetInstance().HasRedPoint(ERedPoint.GuildMember));
        }
        private void _UpdateGuildTips()
        {
            //mGuildTipsBtn.CustomActive(GuildDataManager.GetInstance().IsGuildDungeonActivityOpen());
        }

    }
}
