using Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{

    public class GuildMergeAskInfoFrame : ClientFrame
    {

        private Button mCloseBtn;

        private Button mRefuseBtn;

        private Button mCancleAgreeBtn;

        private Button mAgreeBtn;

        private Button mConnectLeaderBtn;

        private Text mGuildNameTxt;

        private Text mGuildLeaderTxt;

        private Text mGuildGrandTxt;
        
        private Text mGuildCountTxt;

        private GuildEntry mGuildEntry;

        #region UIBase
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Guild/GuildMergeAskInfoFrame";
        }

        protected override void _bindExUI()
        {
            mGuildNameTxt = mBind.GetCom<Text>("GuildNameTxt");
            mGuildLeaderTxt = mBind.GetCom<Text>("GuildLeaderTxt");
            mGuildGrandTxt = mBind.GetCom<Text>("GuildGrandTxt");
            mGuildCountTxt = mBind.GetCom<Text>("GuildMembersTxt");

            mCloseBtn = mBind.GetCom<Button>("CloseBtn");
            mCancleAgreeBtn = mBind.GetCom<Button>("CancelAgreeBtn");
            mAgreeBtn = mBind.GetCom<Button>("AgreeBtn");
            mConnectLeaderBtn = mBind.GetCom<Button>("ConnactLeaderBtn");
            mRefuseBtn = mBind.GetCom<Button>("RefuseBtn");


            mCloseBtn.SafeAddOnClickListener(OnCloseBtnClick);
            mCancleAgreeBtn.SafeAddOnClickListener(OnCancleAgreeBtnClick);
            mAgreeBtn.SafeAddOnClickListener(OnAgreeBtnClick);
            mConnectLeaderBtn.SafeAddOnClickListener(OnConncetLeaderBtnClick);
            mRefuseBtn.SafeAddOnClickListener(OnRefuseBtnClick);
        }

       

        protected override void _unbindExUI()
        {
            mGuildNameTxt = null;
            mGuildLeaderTxt = null;
            mGuildGrandTxt = null;
            mGuildCountTxt = null;


            mCloseBtn.SafeRemoveOnClickListener(OnCloseBtnClick);
            mCancleAgreeBtn.SafeRemoveOnClickListener(OnCancleAgreeBtnClick);
            mAgreeBtn.SafeRemoveOnClickListener(OnAgreeBtnClick);
            mConnectLeaderBtn.SafeRemoveOnClickListener(OnConncetLeaderBtnClick);
            mRefuseBtn.SafeRemoveOnClickListener(OnRefuseBtnClick);

            mCloseBtn = null;
            mCancleAgreeBtn = null;
            mAgreeBtn = null;
            mConnectLeaderBtn = null;
            mRefuseBtn = null;
        }

        protected override void _OnOpenFrame()
        {
            base._OnOpenFrame();
            if(userData!=null)
            {
                mGuildEntry = (GuildEntry)userData;
                SetData(mGuildEntry);
            }
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.AgreeMerger,_OnAgreeMerger);
        }


        protected override void _OnCloseFrame()
        {
            base._OnCloseFrame();
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.AgreeMerger, _OnAgreeMerger);
        }
        #endregion


        private void SetData(GuildEntry guildEntry)
        {
            if(guildEntry!=null)
            {
                mGuildNameTxt.SafeSetText(guildEntry.name);
                mGuildLeaderTxt.SafeSetText(guildEntry.leaderName);
                mGuildGrandTxt.SafeSetText(guildEntry.level.ToString());
                int curNum = guildEntry.memberNum;
                int totalNum= TableManager.GetInstance().GetTableItem<ProtoTable.GuildTable>(guildEntry.level).MemberNum;
                mGuildCountTxt.SafeSetText(string.Format("{0}/{1}",curNum, totalNum));
                SetBtnActive(guildEntry.isRequested != 0);
            }
          
        }

        private void SetBtnActive(bool isHaveAgreed)
        {
            mRefuseBtn.CustomActive(!isHaveAgreed);
            mAgreeBtn.CustomActive(!isHaveAgreed);

            mCancleAgreeBtn.CustomActive(isHaveAgreed);

        }
        /// <summary>
        /// 联系会长
        /// </summary>
        private void OnConncetLeaderBtnClick()
        {
           if(mGuildEntry!=null)
            {
                var relationData = RelationDataManager.GetInstance().GetRelationByRoleID(mGuildEntry.leaderId);
                //相关数据存在，好友或者陌生人
                if (relationData != null)
                {
                    //直接密聊
                    AuctionNewUtility.OpenChatFrame(relationData);
                    return;
                }

                //相关数据不存在的时候，向服务器请求相关数据，获得数据之后再打开密聊界面
                OtherPlayerInfoManager.GetInstance().SendWatchOnShelfItemOwnerInfo(mGuildEntry.leaderId);
            }
         

        }
        /// <summary>
        /// 同意兼并 兼并申请界面变成已同意状态，按钮变成取消同意
        /// </summary>
        private void OnAgreeBtnClick()
        {
            var commonMsgBoxOkCancelParamData = new CommonMsgBoxOkCancelNewParamData()
            {
                ContentLabel = TR.Value("guildmerge_agree_content"),
                IsShowNotify = false,
                LeftButtonText = TR.Value("guildmerge_agree_no"),
                RightButtonText = TR.Value("guildmerge_agree_ok"),
                OnRightButtonClickCallBack = OnAgree,
            };
            SystemNotifyManager.OpenCommonMsgBoxOkCancelNewFrame(commonMsgBoxOkCancelParamData);
        }
        private void OnAgree()
        {
            if (mGuildEntry == null) return;
            GuildDataManager.GetInstance().AcceptOrRefuseOrCancelMergeRequest(0, mGuildEntry.id);
            ClientSystemManager.GetInstance().CloseFrame<GuildMergeAskInfoFrame>();
        }


        /// <summary>
        /// 拒绝
        /// </summary>
        private void OnRefuseBtnClick()
        {
            if (mGuildEntry == null) return;
            GuildDataManager.GetInstance().AcceptOrRefuseOrCancelMergeRequest(1, mGuildEntry.id);
            ClientSystemManager.GetInstance().CloseFrame<GuildMergeAskInfoFrame>();
        }

        /// <summary>
        /// 取消同意兼并 
        /// </summary>
        private void OnCancleAgreeBtnClick()
        {

            var commonMsgBoxOkCancelParamData = new CommonMsgBoxOkCancelNewParamData()
            {
                ContentLabel = TR.Value("guildmerge_cancelAgree_content"),
                IsShowNotify = false,
                LeftButtonText = TR.Value("guildmerge_cancelAgree_no"),
                RightButtonText = TR.Value("guildmerge_cancelAgree_ok"),
                OnRightButtonClickCallBack = OnCancelAgree,
            };
            SystemNotifyManager.OpenCommonMsgBoxOkCancelNewFrame(commonMsgBoxOkCancelParamData);
           
        }
        /// <summary>
        /// 取消同意兼并 
        /// </summary>
        private void OnCancelAgree()
        {
            if (mGuildEntry == null) return;
            GuildDataManager.GetInstance().AcceptOrRefuseOrCancelMergeRequest(2, mGuildEntry.id);
            ClientSystemManager.GetInstance().CloseFrame<GuildMergeAskInfoFrame>();
        }

      

       
        /// <summary>
        /// 关闭界面
        /// </summary>
        private void OnCloseBtnClick()
        {
            ClientSystemManager.GetInstance().CloseFrame<GuildMergeAskInfoFrame>();
        }



        private void _OnAgreeMerger(UIEvent uiEvent)
        {
            SetBtnActive(true);
        }
    }
}
