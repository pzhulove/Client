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
using Network;

namespace GameClient
{
    using TeamBeInvteInfo = NewTeamInviteList;

    public class TeamBeInvitedFrame : ClientFrame
    {
        TeamBeInvteInfo teamBeInvteInfo = null;

        #region val
        bool bTryOpenNewInvite = false;
        const float fMaxTime = 25.0f;
        const float fHangUpTimeOut = 30.0f;
        float fFrameOpenTime = 0.0f;
        #endregion

        #region ui bind       
        private Button BtnClose = null;
        private Image Icon = null;
        private Text Name = null;
        private Text Level = null;
        private Text Target = null;
        private Button reject = null;
        private Button hangUp = null;
        private Button agree = null;
        private Text hangUpText = null;
        private DOTweenAnimation animation1 = null;
        private DOTweenAnimation animation2 = null;
        private ReplaceHeadPortraitFrame mReplaceHeadPortraitFrame = null;
        private GameObject returnPlayer = null;
        private GameObject myFriend = null;
        private GameObject myGuild = null;
        #endregion

        #region override
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Team/TeamBeInvitedFrame";
        }

        protected override void _OnOpenFrame()
        {
            fFrameOpenTime = Time.realtimeSinceStartup;

            BindUIEvent();

            teamBeInvteInfo = null;
            List<NewTeamInviteList> InviteTeamList = TeamDataManager.GetInstance().GetInviteTeamList();
            if(InviteTeamList != null && InviteTeamList.Count > 0)
            {
                teamBeInvteInfo = InviteTeamList[0];
            }

            bTryOpenNewInvite = false;
            UpadateInviteInfo();

            InvokeMethod.InvokeInterval(this, 0.0f, 1.0f, fHangUpTimeOut, 
            () => 
            {                
                hangUpText.SafeSetText(TR.Value("hang_up_tick", (int)(fHangUpTimeOut)));
            },
            () => 
            {             
                int nTimeLeft = (int)fHangUpTimeOut - Utility.ceil(Time.realtimeSinceStartup - fFrameOpenTime);
                nTimeLeft = Math.Max(nTimeLeft, 0);               
                hangUpText.SafeSetText(TR.Value("hang_up_tick", nTimeLeft));

            }, 
            () => 
            {                
                OnHangUp();
            });
        }

        protected override void _OnCloseFrame()
        {
            InvokeMethod.RmoveInvokeIntervalCall(this);
            teamBeInvteInfo = null;
            UnBindUIEvent();

            if(bTryOpenNewInvite && Time.realtimeSinceStartup - fFrameOpenTime <= fMaxTime)
            {
                StopCoroutine(OpenNewTeamBeInviteFrame());
                StartCoroutine(OpenNewTeamBeInviteFrame());                
            }

            fFrameOpenTime = 0.0f;
            bTryOpenNewInvite = false;
        }

        protected override void _bindExUI()
        {
            BtnClose = mBind.GetCom<Button>("BtnClose");
            BtnClose.SafeRemoveAllListener();
            BtnClose.SafeAddOnClickListener(() => 
            {
                OnHangUp();
            });

            Icon = mBind.GetCom<Image>("Icon");
            Name = mBind.GetCom<Text>("Name");
            Level = mBind.GetCom<Text>("Level");
            Target = mBind.GetCom<Text>("Target");

            reject = mBind.GetCom<Button>("reject");
            reject.SafeRemoveAllListener();
            reject.SafeAddOnClickListener(() => 
            {
                OnReject();

                bTryOpenNewInvite = true;
                frameMgr.CloseFrame(this);
            });

            hangUp = mBind.GetCom<Button>("hangUp");
            hangUp.SafeRemoveAllListener();
            hangUp.SafeAddOnClickListener(() => 
            {
                OnHangUp();
            });

            agree = mBind.GetCom<Button>("agree");
            agree.SafeRemoveAllListener();
            agree.SafeAddOnClickListener(() => 
            {
                OnAgree();

                frameMgr.CloseFrame(this);
                bTryOpenNewInvite = false;                
            });

            hangUpText = mBind.GetCom<Text>("hangUpText");

            animation1 = mBind.GetCom<DOTweenAnimation>("animation1");
            animation2 = mBind.GetCom<DOTweenAnimation>("animation2");
            mReplaceHeadPortraitFrame = mBind.GetCom<ReplaceHeadPortraitFrame>("PictureFrame");
            returnPlayer = mBind.GetGameObject("returnPlayer");
            myFriend = mBind.GetGameObject("myFriend");
            myGuild = mBind.GetGameObject("myGuild");
    }

        protected override void _unbindExUI()
        {
            BtnClose = null;
            Icon = null;
            Name = null;
            Level = null;
            Target = null;
            reject = null;
            hangUp = null;
            agree = null;
            hangUpText = null;
            animation1 = null;
            animation2 = null;
            mReplaceHeadPortraitFrame = null;
            returnPlayer = null;
            myFriend = null;
            myGuild = null;
        }

        #endregion 

        #region method

        void BindUIEvent()
        {
            //UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.RefreshVerifyPwdErrorCount,_OnRefreshVerifyPwdErrorCount);
        }

        void UnBindUIEvent()
        {
            //UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.RefreshVerifyPwdErrorCount,_OnRefreshVerifyPwdErrorCount);
        }

        void UpadateInviteInfo()
        {
            if(teamBeInvteInfo == null)
            {
                return;
            }

            JobTable jobData = TableManager.GetInstance().GetTableItem<JobTable>(teamBeInvteInfo.baseinfo.masterInfo.occu);
            if (jobData != null)
            {
                ResTable resData = TableManager.GetInstance().GetTableItem<ResTable>(jobData.Mode);
                if (resData != null)
                {
                    Icon.SafeSetImage(resData.IconPath);                   
                }
            }

            Name.SafeSetText(teamBeInvteInfo.baseinfo.masterInfo.name);
            Level.SafeSetText(string.Format("Lv{0}", teamBeInvteInfo.baseinfo.masterInfo.level));

            TeamDungeonTable table = TableManager.GetInstance().GetTableItem<TeamDungeonTable>((int)teamBeInvteInfo.baseinfo.target);
            if (table != null)
            {
                Target.SafeSetText(TR.Value("other_player_invite_you", table.Name));
            }

            //设置头像框
            if (mReplaceHeadPortraitFrame != null)
            {
                if (teamBeInvteInfo.baseinfo != null && teamBeInvteInfo.baseinfo.masterInfo != null && teamBeInvteInfo.baseinfo.masterInfo.playerLabelInfo != null)
                {
                    if (teamBeInvteInfo.baseinfo.masterInfo.playerLabelInfo.headFrame != 0)
                    {
                        mReplaceHeadPortraitFrame.ReplacePhotoFrame((int)teamBeInvteInfo.baseinfo.masterInfo.playerLabelInfo.headFrame);
                    }
                    else
                    {
                        mReplaceHeadPortraitFrame.ReplacePhotoFrame(HeadPortraitFrameDataManager.iDefaultHeadPortraitID);
                    }
                }
            }
          
            returnPlayer.CustomActive(teamBeInvteInfo.baseinfo.masterInfo.playerLabelInfo.returnStatus == 1);
            RelationData relationData = null;
            bool isMyFriend = RelationDataManager.GetInstance().FindPlayerIsRelation(teamBeInvteInfo.baseinfo.masterInfo.id, ref relationData);
            myFriend.CustomActive(isMyFriend);
            myGuild.CustomActive(GuildDataManager.GetInstance().IsSameGuild(teamBeInvteInfo.baseinfo.masterInfo.playerLabelInfo.guildId));
            return;
        }

        IEnumerator OpenNewTeamBeInviteFrame()
        {
            // 等两帧
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();

            List<NewTeamInviteList> InviteTeamList = TeamDataManager.GetInstance().GetInviteTeamList();
            if(InviteTeamList != null && InviteTeamList.Count > 0)
            {
                ClientSystemManager.GetInstance().OpenFrame<TeamBeInvitedFrame>();
            }

            yield return 0;
        }

        void OnHangUp()
        {
            // 这里做个容错处理，防止界面关闭不了导致流程卡死的问题
            try
            {
                GameObject btn = ComTalk.ms_comTalk.GetTeamInvitedBtn();
                if (btn == null || !btn.activeInHierarchy)
                {
                    frameMgr.CloseFrame(this);
                    bTryOpenNewInvite = false;
                    return;
                }
        
                animation1.endValueV3 = new Vector3(btn.transform.position.x, btn.transform.position.y);
                animation1.tween = animation1.transform.DOMove(animation1.endValueV3, animation1.duration).SetDelay(animation1.delay);               

                if (animation1.onComplete == null)
                {
                    animation1.onComplete = new UnityEngine.Events.UnityEvent();                 
                }

                animation1.hasOnComplete = true;
                animation1.onComplete.AddListener(() =>
                {
                    frameMgr.CloseFrame(this);
                    bTryOpenNewInvite = false;
                });

                animation1.DOPlay();
                animation2.DOPlay();
            }
            catch(System.Exception e)
            {
                //Logger.LogErrorFormat(string.Format("[被邀请界面]启动挂起动画异常，异常为{0}", e.ToString()));

                frameMgr.CloseFrame(this);
                bTryOpenNewInvite = false;
            }
            
            return;
        }

        void OnReject()
        {
            List<NewTeamInviteList> InviteTeamList = TeamDataManager.GetInstance().GetInviteTeamList();
            if (InviteTeamList == null || InviteTeamList.Count == 0)
            {
                return;
            }

            if (teamBeInvteInfo == null)
            {
                return;
            }

            int iIndex = GetInviteIndex(teamBeInvteInfo);
            if (iIndex < 0 || iIndex >= InviteTeamList.Count)
            {
                return;
            }

            NetManager netMgr = NetManager.Instance();
            SceneReply msg = new SceneReply();

            msg.result = 0;
            msg.type = (byte)RequestType.InviteTeam;
            msg.requester = teamBeInvteInfo.baseinfo.teamId;

            netMgr.SendCommand(ServerType.GATE_SERVER, msg);

            InviteTeamList.RemoveAt(iIndex);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.TeamNewInviteNoticeUpdate);
        }

        void OnAgree()
        {
            List<NewTeamInviteList> InviteTeamList = TeamDataManager.GetInstance().GetInviteTeamList();
            if(InviteTeamList == null || InviteTeamList.Count == 0)
            {
                return;
            }

            if(teamBeInvteInfo == null)
            {
                return;
            }

            int iIndex = GetInviteIndex(teamBeInvteInfo);
            if (iIndex < 0 || iIndex >= InviteTeamList.Count)
            {
                return;
            }

            NetManager netMgr = NetManager.Instance();
            SceneReply msg = new SceneReply();

            msg.result = 1;
            msg.type = (byte)RequestType.InviteTeam;
            msg.requester = teamBeInvteInfo.baseinfo.teamId;

            netMgr.SendCommand(ServerType.GATE_SERVER, msg);

            InviteTeamList.RemoveAt(iIndex);

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.TeamNewInviteNoticeUpdate);   
        }

        int GetInviteIndex(TeamBeInvteInfo info)
        {
            if(info == null)
            {
                return -1;
            }

            List<NewTeamInviteList> InviteTeamList = TeamDataManager.GetInstance().GetInviteTeamList();
            if(InviteTeamList == null)
            {
                return -1;
            }

            for(int i = 0;i < InviteTeamList.Count;i++)
            {
                NewTeamInviteList inviteInfo = InviteTeamList[i];
                if(inviteInfo.baseinfo.masterInfo.id == info.baseinfo.masterInfo.id && inviteInfo.baseinfo.target == info.baseinfo.target)
                {
                    return i;
                }
            }

            return -1;
        }

        #endregion

        #region ui event

        //         void _OnRefreshVerifyPwdErrorCount(UIEvent uiEvent)
        //         {
        //             SecurityLockData data = SecurityLockDataManager.GetInstance().GetSecurityLockData();
        //             if(txtErrorCount != null)
        //             {
        //                 txtErrorCount.text = TR.Value("verifyPwdFailedCount", maxErrorCount, data.verifyPwdFailedCount);
        //             }
        // 
        //             if(data.verifyPwdFailedCount >= maxErrorCount)
        //             {
        //                 InvokeMethod.RmoveInvokeIntervalCall(this);
        // 
        //                 if(btnOK != null)
        //                 {
        //                     UIGray gray = btnOK.gameObject.SafeAddComponent<UIGray>();
        //                     if (gray != null)
        //                     {
        //                         gray.SetEnable(true);
        //                     }
        // 
        //                     btnOK.interactable = false;
        //                     btnOK.image.raycastTarget = false;
        //                 }
        // 
        //                 if (txtOk != null)
        //                 {
        //                     txtOk.text = string.Format("{0}", strOk);
        //                     iCount = 0;
        //                 }
        //             }
        //         }

        #endregion
    }
}
