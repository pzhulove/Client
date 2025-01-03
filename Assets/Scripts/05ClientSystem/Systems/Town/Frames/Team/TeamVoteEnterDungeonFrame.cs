using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using ProtoTable;
using Protocol;
using Network;

namespace GameClient
{
    class TeamVoteEnterDungeonFrame : ClientFrame
    {
        const int MemberNum = 3;
        float MaxWaitTime = RejectTime; // 给进度条用的时间
        float fAddUpTime = 0.0f;
        float fSpeed = 0.00225f;

        const float AgreeTime = 3.0f;
        const float RejectTime = 10.0f;
        const float HoldMaxTime = 15.0f;
        float MaxWaitTime2 = RejectTime; // 给拒绝和同意按钮用的
        float TimeRemain = 0.0f;
        bool bFlag = false;
        bool bUpdate = true;
        bool bWaitAutoAgree = false;
        int iDungeonID = 0;
        float AgreeAddTime = 0.0f; // 记录点击同意后的时长

        enum UpdateMode
        {
            AUTO_TEAMMETE_REJECT,
            AUTO_TEAMMETE_AGREE,
            AUTO_TEAMMETE_AGREE_WAITLOADING,
            AUTO_TEAMLEADER,
            AUTO_AGREE_HOLDMAX
        };

        UpdateMode mUpdateMode;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Team/TeamVoteEnterDungeonFrame";
        }

        protected override void _OnOpenFrame()
        {
            bFlag = false;
            iDungeonID = (int)userData;

            InitInterface();
            BindUIEvent();

			#if DEBUG_SETTING
			if (Global.Settings.IsTestTeam())
			{
				ClientSystemManager.GetInstance().delayCaller.DelayCall(2000, ()=>{
					OnAgree();
				});
			}
            #endif
        }

        protected override void _OnCloseFrame()
        {
            bFlag = false;
            Clear();
            UnBindUIEvent();
        }

        void Clear()
        {
            MaxWaitTime = RejectTime;
            fAddUpTime = 0.0f;
            Process.value = 0.0f;

            iDungeonID = 0;
            AgreeAddTime = 0.0f;
        }

        void BindUIEvent()
        {
        }

        void UnBindUIEvent()
        {
        }

        void _OnReject()
        {
            if (bFlag)
            {
                return;
            }
            bFlag = true;
            bUpdate = false;

            WorldTeamReportVoteChoice msg = new WorldTeamReportVoteChoice();
            msg.agree = 0;

            RejectText.text = string.Format("拒绝");
            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, msg);

            frameMgr.CloseFrame(this);
        }
        [UIEventHandle("middle/reject")]
        void OnReject()
        {
            _OnReject();
            TeamDataManager.GetInstance().IsAutoAgree = false;
        }

        [UIEventHandle("middle/agree")]
        void OnAgree()
        {
            // http://192.168.2.253:8080/browse/ALD-2219 http://192.168.2.253:8080/browse/ALD-2220
            // 正在城镇中切换场景的时候 即使是点击了同意，也应该发送拒绝消息 (解决玩家卡loading和卡战斗场景的bug)
            // add by qxy 2019-11-29
            var systemTown = ClientSystemManager.instance.CurrentSystem as ClientSystemTown;
            if (systemTown != null && systemTown.GetTownSceneSwitchState()) 
            {
                _OnReject();
                return;
            }

            if (bFlag)
            {
                return;
            }
            bFlag = true;
            bUpdate = false;

            WorldTeamReportVoteChoice msg = new WorldTeamReportVoteChoice();
            msg.agree = 1;

            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, msg);

            btAgree.GetComponent<UIGray>().enabled = true;
            btAgree.interactable = false;

            btReject.GetComponent<UIGray>().enabled = true;
            btReject.interactable = false;
            AgreeText.text = string.Format("同意");

            mUpdateMode = UpdateMode.AUTO_AGREE_HOLDMAX;
            TimeRemain = HoldMaxTime;
            fAddUpTime = 0.0f;
            AgreeAddTime = 0.0f;
        }

        void InitInterface()
        {
            UIGray Agreegray = btAgree.gameObject.AddComponent<UIGray>();
            Agreegray.enabled = false;

            UIGray Rejectgray = btReject.gameObject.AddComponent<UIGray>();
            Rejectgray.enabled = false;

            DungeonTable data = TableManager.GetInstance().GetTableItem<DungeonTable>(iDungeonID);
            if (data == null)
            {
                return;
            }

            if (TeamDataManager.GetInstance().IsTeamLeader())
            {
                btReject.gameObject.SetActive(false);
                btAgree.gameObject.SetActive(false);
                mTeamLeaderTips.SetActive(true);
            }
            else
            {
                btReject.gameObject.SetActive(true);
                btAgree.gameObject.SetActive(true);

                mTeamLeaderTips.SetActive(false);
            }


            for (int i = 0; i < MemberNum; i++)
            {
                UIGray gray = members[i].gameObject.AddComponent<UIGray>();
                gray.enabled = true;
            }

            DungeonName.text = data.Name;
            if(txtTeamTargetInfo != null)
            {
                //若果是堕落深渊 难度显示“王者”
                if (data.SubType == DungeonTable.eSubType.S_LIMIT_TIME_HELL
                    || data.SubType == DungeonTable.eSubType.S_WEEK_HELL
                    || data.SubType == DungeonTable.eSubType.S_WEEK_HELL_ENTRY
                    || data.SubType == DungeonTable.eSubType.S_LIMIT_TIME__FREE_HELL)
                {
                    txtTeamTargetInfo.text = string.Format("{0}({1})", data.Name, TeamDataManager.GetInstance().GeDungeontHardName((byte)DungeonTable.eHard.KING));
                }
                else
                {
                    txtTeamTargetInfo.text = string.Format("{0}({1})", data.Name, TeamDataManager.GetInstance().GeDungeontHardName((byte)data.Hard));
                }
            }
            if(togAutoAgree != null)
            {
                togAutoAgree.isOn = TeamDataManager.GetInstance().IsAutoAgree;
            }
            if(togAutoAgree != null)
            {
                togAutoAgree.CustomActive(!TeamDataManager.GetInstance().IsTeamLeader());
            }

            mUpdateMode = UpdateMode.AUTO_TEAMMETE_REJECT;
            TimeRemain = RejectTime;


            fAddUpTime = 0.0f;

            //队长模式
            if (TeamDataManager.GetInstance().IsTeamLeader())
            {
                mUpdateMode = UpdateMode.AUTO_TEAMLEADER;
                TimeRemain = RejectTime;
            }
            //队员模式
            else
            {
                if (TeamDataManager.GetInstance().IsAutoAgree)
                {
                    MaxWaitTime = AgreeTime;

#if UNITY_EDITOR
                    if(_isSystemInLoading() && LeanTween.instance.useVoteEnterLoadingProtect)
#else
                    if (_isSystemInLoading())
#endif

                    {
                        mUpdateMode = UpdateMode.AUTO_TEAMMETE_AGREE_WAITLOADING;
                        TimeRemain = RejectTime;
                    }
                    else
                    {
                        mUpdateMode = UpdateMode.AUTO_TEAMMETE_AGREE;
                        TimeRemain = AgreeTime;
                    }
                }
            }
            
            bUpdate = true;
            UpdateInterface();
        }

        void UpdateInterface()
        {
            Team myTeam = TeamDataManager.GetInstance().GetMyTeam();
            if (myTeam == null)
            {
                return;
            }

            for (int i = 0; i < MemberNum; i++)
            {
                if(i < myTeam.members.Length && myTeam.members[i].id != 0)
                {
                    UIGray cgom = members[i].gameObject.GetComponent<UIGray>();
                    cgom.enabled = false;

                    JobTable data = TableManager.GetInstance().GetTableItem<JobTable>(myTeam.members[i].occu);
                    if(data == null)
                    {
                        continue;
                    }

                    //Sprite spr = AssetLoader.instance.LoadRes(data.JobHalfBody, typeof(Sprite)).obj as Sprite;
                    //if(spr != null)
                    //{
                    //    icons[i].sprite = spr;
                    //}
                    ETCImageLoader.LoadSprite(ref icons[i], data.JobHalfBody);

                    Lvs[i].text = string.Format("Lv.{0}", myTeam.members[i].level);
                    Names[i].text = myTeam.members[i].name;
                    Occus[i].text = data.Name;

                    if(myTeam.members[i].id == myTeam.leaderInfo.id)
                    {
                        UIGray com = members[i].gameObject.GetComponent<UIGray>();
                        com.enabled = false;
                    }
                    else
                    {
                        UIGray ucom = members[i].gameObject.GetComponent<UIGray>();
                        ucom.enabled = true;
                    }
                    members[i].gameObject.SetActive(true);
                    
                }
                else
                {
                    members[i].gameObject.SetActive(false);
                }
            }
        }

        public void UpdateMemVoteState(WorldTeamVoteChoiceNotify NotifyData)
        {
            Team myTeam = TeamDataManager.GetInstance().GetMyTeam();
            if (myTeam == null)
            {
                return;
            }

            for (int i = 0; i < myTeam.members.Length; i++)
            {
                if (myTeam.members[i].id == NotifyData.roleId && NotifyData.roleId != 0)
                {
                    if (NotifyData.agree == 0)
                    {
                        SystemNotifyManager.SysNotifyFloatingEffect(string.Format(TR.Value("rejectEnterDungeon"), myTeam.members[i].name));
                    }
                    else
                    {
                        UIGray com = members[i].gameObject.GetComponent<UIGray>();
                        com.enabled = false;
                    }

                    break;
                }
            }

            if (NotifyData.agree == 0)
            {
                frameMgr.CloseFrame(this);
            }
        }

        public override bool IsNeedUpdate()
        {
            return true;
        }


        protected bool _isSystemInLoading()
        {
            return ClientSystemManager.instance.isSwitchSystemLoading && ClientSystemManager.instance.TargetSystem != null;
        }

        protected void ResetTimeRemain(float time)
        {
            TimeRemain = time;
            fAddUpTime = 0.0f;
        }

        protected override void _OnUpdate(float timeElapsed)
        {
            Process.value += fSpeed;

            if (bUpdate == false)
            {
                if (mUpdateMode == UpdateMode.AUTO_AGREE_HOLDMAX)
                {
                    AgreeAddTime += timeElapsed;

                    if (AgreeAddTime > HoldMaxTime)
                    {
                        frameMgr.CloseFrame(this);
                    }
                }

                return;
            }

            fAddUpTime += timeElapsed;

            //更新倒计时
            if (fAddUpTime > 1.0f)
            {
                TimeRemain -= 1.0f;
                MaxWaitTime -= 1.0f;
                fAddUpTime = 0.0f;
            }

            switch (mUpdateMode)
            {
                case UpdateMode.AUTO_TEAMMETE_AGREE:
                    {
                        int iInt = (int)(TimeRemain);
                        if (iInt >= 0)
                        {
                            RejectText.text = string.Format("拒绝");
                            AgreeText.text = string.Format("同意({0}s)", iInt);
                        }

                        if (TimeRemain < 0.05f)
                        {
                           OnAgree();
                        }
                    }
                    break;
                case UpdateMode.AUTO_TEAMMETE_REJECT:
                    {
                        int iInt = (int)(TimeRemain);

                        if (iInt >= 0)
                        {
                            RejectText.text = string.Format("拒绝({0}s)", iInt);
                            AgreeText.text = string.Format("同意");
                        }

                        if (TimeRemain < 0.05f)
                        {
                            _OnReject();
                        }
                    }
                    break;
                case UpdateMode.AUTO_TEAMMETE_AGREE_WAITLOADING:
                    {
                        if (TimeRemain < 0.05f)
                        {
                            _OnReject();
                        }

                        if (_isSystemInLoading() == false)
                        {
                            TimeRemain = AgreeTime; 
                            fAddUpTime  = 0.0f;
                            mUpdateMode = UpdateMode.AUTO_TEAMMETE_AGREE;
                        }
                    }
                    break;
                case UpdateMode.AUTO_AGREE_HOLDMAX:
                    {
                        // 冲哥改的流程里，这里是进不来的,因为点击同意后，bUpdate == false，直接return了
                        if (TimeRemain < 0.05f)
                        {
                            frameMgr.CloseFrame(this);
                        }
                    }
                    break;
                case UpdateMode.AUTO_TEAMLEADER:
                default:
                    {
                        if (TimeRemain < 0.05f)
                        {
                            if (!TeamDataManager.GetInstance().IsTeamLeader())
                            {
                                _OnReject();
                            }
                            else
                            {
                                frameMgr.CloseFrame(this);
                            }
                        }
                    }
                    break;
            }
        }

        [UIControl("middle/DungeonName")]
        Text DungeonName;

        [UIControl("middle/memberroot/mem{0}", typeof(Image), 1)]
        Image[] members = new Image[MemberNum];

        [UIControl("middle/memberroot/mem{0}/Mask/OccuHead", typeof(Image), 1)]
        Image[] icons = new Image[MemberNum];

        [UIControl("middle/memberroot/mem{0}/Lv", typeof(Text), 1)]
        Text[] Lvs = new Text[MemberNum];

        [UIControl("middle/memberroot/mem{0}/nameback/Name", typeof(Text), 1)]
        Text[] Names = new Text[MemberNum];

        [UIControl("middle/memberroot/mem{0}/nameback/Occu", typeof(Text), 1)]
        Text[] Occus = new Text[MemberNum];

        [UIControl("middle/reject")]
        Button btReject;

        [UIControl("middle/reject/Text")]
        Text RejectText;

        [UIControl("middle/agree/Text")]
        Text AgreeText;
        [UIControl("middle/agree")]
        Button btAgree;

        [UIControl("middle/process", typeof(Slider))]
        Slider Process;

#region ExtraUIBind
        private GameObject mTeamLeaderTips = null;
        private Text txtTeamTargetInfo = null;
        private Toggle togAutoAgree = null;

        protected override void _bindExUI()
        {
            mTeamLeaderTips = mBind.GetGameObject("teamLeaderTips");
            txtTeamTargetInfo = mBind.GetCom<Text>("txtTeamTargetInfo");
            togAutoAgree = mBind.GetCom<Toggle>("togAutoAgree");
            if(togAutoAgree != null)
            {
                togAutoAgree.onValueChanged.RemoveAllListeners();
                togAutoAgree.onValueChanged.AddListener((bool bIsOn) => 
                {
                    if(!bIsOn && TeamDataManager.GetInstance().IsAutoAgree)
                    {
                        OnReject();
                    }
                    else if(bIsOn && !TeamDataManager.GetInstance().IsAutoAgree)
                    {
                        mUpdateMode = UpdateMode.AUTO_TEAMMETE_AGREE;
                        TimeRemain = AgreeTime;
                        fAddUpTime = 0.0f;

                        if(MaxWaitTime > 0.0f)
                        {
                            fAddUpTime = 0.0f;
                            MaxWaitTime = AgreeTime;
                        }                                           
                        RejectText.text = string.Format("拒绝");
                        AgreeText.text = string.Format("同意");
                    }
                    TeamDataManager.GetInstance().IsAutoAgree = bIsOn;
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.TeamInfoUpdateSuccess);
                });
            }
        }

        protected override void _unbindExUI()
        {
            mTeamLeaderTips = null;
            txtTeamTargetInfo = null;
            togAutoAgree = null;
        }
#endregion   
    }
}
