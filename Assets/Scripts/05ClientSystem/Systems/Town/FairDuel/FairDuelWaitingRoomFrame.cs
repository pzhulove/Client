using Network;
using Protocol;
using ProtoTable;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    
    public class FairDueliRoomData
    {
        public CitySceneTable.eSceneSubType SceneSubType = CitySceneTable.eSceneSubType.NULL;
        public int CurrentSceneID = 0;
        public int TargetTownSceneID = 0;

        public void Clear()
        {
            SceneSubType = CitySceneTable.eSceneSubType.NULL;
            CurrentSceneID = 0;
            TargetTownSceneID = 0;
        }
    }
    public class FairDuelWaitingRoomFrame : ClientFrame
    {
        private Button mCloseBtn;
        private Button mFightBtn;
        private Button mRuleDetailBtn;
        private Button mSkillBtn;
        private Button mFriendPkBtn;
        private FairDueliRoomData mFairDueliRoomData = new FairDueliRoomData();
        private ComTalk mComTalk;
        private GameObject mComTalkParent;
        private bool mIsSetSkillBar;
        private Text mFightBtnTxt;
        private bool mIsSeeking = false;

      
        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/FairDuel/FairDuelWaitingRoomFrame";
        }
        protected override void _OnOpenFrame()
        {
        

            if (userData!=null)
            {
                mFairDueliRoomData = (FairDueliRoomData)userData;

            }
            if (mComTalkParent!=null)
            {
                mComTalk = ComTalk.Create(mComTalkParent);
            }
            SkillDataManager.GetInstance().SendSetSkillConfigReq(0);

            BindEvt();
        }

       

        protected override void _OnCloseFrame()
        {
            if (ClientSystemManager.GetInstance().IsFrameOpen<ClientSystemTownFrame>())
            {
                ClientSystemTownFrame Townframe = ClientSystemManager.GetInstance().GetFrame(typeof(ClientSystemTownFrame)) as ClientSystemTownFrame;
                Townframe.SetForbidFadeIn(false);
            }

            ClearData();
            UnBind();


        }
        private void BindEvt()
        {
            NetProcess.AddMsgHandler(WorldMatchStartRes.MsgID, _OnReciveWorldMatchStartResRes);
            NetProcess.AddMsgHandler(WorldMatchCancelRes.MsgID, _OnReciveWorldMatchCancelRes);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PkMatchStartSuccess, _OnPkMatchStartSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PkMatchCancelSuccess, _OnPkMatchStartFail);
        }

        private void UnBind()
        {
            NetProcess.AddMsgHandler(WorldMatchStartRes.MsgID, _OnReciveWorldMatchStartResRes);
            NetProcess.AddMsgHandler(WorldMatchCancelRes.MsgID, _OnReciveWorldMatchCancelRes);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PkMatchStartSuccess, _OnPkMatchStartSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PkMatchCancelSuccess, _OnPkMatchStartFail);
        }

     

        private void ClearData()
        {
            if(mFairDueliRoomData!=null)
            {
                mFairDueliRoomData.Clear();
            }
           
            if(mComTalk!=null)
            {
                ComTalk.Recycle();
                mComTalk = null;
            }
            mIsSeeking = false;
        }

        #region ExtraUIBind
        protected override void _bindExUI()
        {
            mCloseBtn = mBind.GetCom<Button>("btClose");
            if(mCloseBtn!=null)
            {
                mCloseBtn.onClick.AddListener(OnCloseBtnClick);
            }
            mFightBtn = mBind.GetCom<Button>("btBegin");
            if(mFightBtn!=null)
            {
                mFightBtn.onClick.AddListener(OnFightBtnClick);
            }
            mRuleDetailBtn = mBind.GetCom<Button>("RuleBtn");
            if(mRuleDetailBtn!=null)
            {
                mRuleDetailBtn.onClick.AddListener(OnRuleDetailBtnClick);
            }
            mComTalkParent = mBind.GetGameObject("TalkParent");

            mSkillBtn = mBind.GetCom<Button>("SkillBtn");
            if(mSkillBtn!=null)
            {
                mSkillBtn.onClick.AddListener(OnSkillBtnClick);
            }
            mFriendPkBtn = mBind.GetCom<Button>("FriendPKBtn");
            if(mFriendPkBtn!=null)
            {
                mFriendPkBtn.onClick.AddListener(OnFriendPKBtnClick);
            }
            mFightBtnTxt = mBind.GetCom<Text>("Begintxt");
        }

     
        protected override void _unbindExUI()
        {
            if (mCloseBtn != null)
            {
                mCloseBtn.onClick.RemoveListener(OnCloseBtnClick);
                mCloseBtn = null;
            }
            if (mFightBtn != null)
            {
                mFightBtn.onClick.RemoveListener(OnFightBtnClick);
                mFightBtn = null;
            }
            if (mRuleDetailBtn != null)
            {
                mRuleDetailBtn.onClick.RemoveListener(OnRuleDetailBtnClick);
                mRuleDetailBtn = null;
            }
            if(mComTalkParent!=null)
            {
                mComTalkParent = null;
            }
            if (mSkillBtn != null)
            {
                mSkillBtn.onClick.RemoveListener(OnSkillBtnClick);
                mSkillBtn = null;
            }
            if (mFriendPkBtn != null)
            {
                mFriendPkBtn.onClick.RemoveListener(OnFriendPKBtnClick);
                mFriendPkBtn = null;
            }
            if(mFightBtnTxt!=null)
            {
                mFightBtnTxt = null;
            }
        }
        #endregion

        #region 按钮点击事件
        /// <summary>
        /// 详细规则
        /// </summary>
        private void OnRuleDetailBtnClick()
        {
            ClientSystemManager.GetInstance().OpenFrame<FairDuelHelpFrame>(FrameLayer.Middle);
        }
        /// <summary>
        /// 开始匹配
        /// </summary>
        private void OnFightBtnClick()
        {
            if(!SkillDataManager.GetInstance().IsHaveSetFairDueSkillBar)
            {
                //弹出是否设置技能栏的提示
                var commonMsgBoxOkCancelParamData = new CommonMsgBoxOkCancelNewParamData()
                {
                    ContentLabel = TR.Value("fairduel_setskillBar_content"),
                    IsShowNotify = false,
                    LeftButtonText = TR.Value("fairduel_setskillBar_cancel"),
                    RightButtonText = TR.Value("fairduel_setskillBar_ok"),
                    OnRightButtonClickCallBack = OnOpenFairSkillFrame,
                };
                SystemNotifyManager.OpenCommonMsgBoxOkCancelNewFrame(commonMsgBoxOkCancelParamData);
            }
            else//直接匹配
            {
                if(!mIsSeeking)
                {
                    WorldMatchStartReq req = new WorldMatchStartReq();
                    req.type = (byte)PkType.PK_EQUAL_1V1;

                    NetManager netMgr = NetManager.Instance();
                    netMgr.SendCommand(ServerType.GATE_SERVER, req);
                }
                else
                {
                    WorldMatchCancelReq req = new WorldMatchCancelReq();

                    NetManager netMgr = NetManager.Instance();
                    netMgr.SendCommand(ServerType.GATE_SERVER, req);
                }
             
            }
        }

        private void OnOpenFairSkillFrame()
        {
            SkillDataManager.GetInstance().SendSetSkillConfigReq(1);

            SkillFrameParam frameParam = new SkillFrameParam();
            frameParam.frameType = SkillFrameType.FairDuel;

            ClientSystemManager.GetInstance().OpenFrame<SkillFrame>(FrameLayer.Middle, frameParam);
        }

        /// <summary>
        /// 好有PK
        /// </summary>
        private void OnFriendPKBtnClick()
        {
            if (mIsSeeking)
            {
                SystemNotifyManager.SystemNotify(4004);
                return;
            }
            ClientSystemManager.GetInstance().OpenFrame<PkFriendsFrame>(FrameLayer.Middle, RequestType.Request_Equal_PK);
        }

        /// <summary>
        /// 关闭按钮
        /// </summary>
        private void OnCloseBtnClick()
        {
         
            if (mIsSeeking)
            {
                SystemNotifyManager.SystemNotify(4004);
                return;
            }
           
            ClientSystemTown systemTown = ClientSystemManager.GetInstance().GetCurrentSystem() as ClientSystemTown;
            if (systemTown == null)
            {
                Logger.LogError("Current System is not systemTown!!!");
                return;
            }
            GameFrameWork.instance.StartCoroutine(systemTown._NetSyncChangeScene(new SceneParams
            {
                currSceneID = mFairDueliRoomData.CurrentSceneID,
                currDoorID = 0,
                targetSceneID = mFairDueliRoomData.TargetTownSceneID,
                targetDoorID = 0,
            }, false));
            frameMgr.CloseFrame(this);
         

        }

        /// <summary>
        /// 打开技能面板
        /// </summary>
        private void OnSkillBtnClick()
        {
            SkillFrameParam frameParam = new SkillFrameParam();
            frameParam.frameType = SkillFrameType.FairDuel;

            ClientSystemManager.GetInstance().OpenFrame<SkillFrame>(FrameLayer.Middle, frameParam);
        }
        #endregion

        private void _OnReciveWorldMatchStartResRes(MsgDATA data)
        {
            //Debug.Log("_OnReciveWorldMatchStartResRes");
            if (data == null)
            {
                return;
            }
            WorldMatchStartRes res = new WorldMatchStartRes();
            res.decode(data.bytes);

            if (res.result != 0)
            {
                SystemNotifyManager.SystemNotify((int)res.result);

                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PkMatchFailed);
                return;
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PkMatchStartSuccess);

        }
        private void _OnReciveWorldMatchCancelRes(MsgDATA data)
        {
            //Debug.Log("_OnReciveWorldMatchCancelRes");
            if (data == null)
            {
                return;
            }
            WorldMatchCancelRes res = new WorldMatchCancelRes();
            res.decode(data.bytes);

            if (res.result != 0)
            {
                SystemNotifyManager.SystemNotify((int)res.result);

                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PkMatchCancelFailed);
                return;
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PkMatchCancelSuccess);

        }

        private void _OnPkMatchStartSuccess(UIEvent uiEvent)
        {
           
            PkSeekWaitingData waitingData = new PkSeekWaitingData();
            waitingData.roomtype = PkRoomType.TraditionPk;
            mIsSeeking = true;
            mFightBtnTxt.text = "取消匹配";
            ClientSystemManager.GetInstance().OpenFrame<PkSeekWaiting>(FrameLayer.Middle, waitingData);
        }

        private void _OnPkMatchStartFail(UIEvent uiEvent)
        {
            ClientSystemManager.GetInstance().CloseFrame<PkSeekWaiting>();
            mIsSeeking = false;
            mFightBtnTxt.text = "开始匹配";
        }

    }
}
