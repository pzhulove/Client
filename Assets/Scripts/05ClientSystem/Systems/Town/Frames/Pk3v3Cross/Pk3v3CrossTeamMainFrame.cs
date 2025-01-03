using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using ProtoTable;
using Network;
using Protocol;

namespace GameClient
{
    class Pk3v3CrossTeamMainFrame : ClientFrame
    {
        const int MemberNum = 3;

        List<Pk3v3CrossDataManager.TeamMember> TeamDatas = new List<Pk3v3CrossDataManager.TeamMember>();

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Pk3v3Cross/Pk3v3CrossTeamMain";
        }

        protected override void _OnOpenFrame()
        {

            if (btnBk != null)
            {
                for (int i = 0; i < btnBk.Length; i++)
                {
                    Button btn = btnBk[i];
                    if (btn != null)
                    {
                        btn.onClick.RemoveAllListeners();
                        btn.onClick.AddListener(OnOpenInvite);
                    }
                }
            }


            InitInterface();
            BindUIEvent();          
        }

        protected override void _OnCloseFrame()
        {
            Clear();
        }

        void Clear()
        {
            TeamDatas.Clear();
            UnBindUIEvent();
        }

        void BindUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.Pk3v3CrossUpdateMyTeamFrame, OnUpdateInterface);
        }

        void UnBindUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.Pk3v3CrossUpdateMyTeamFrame, OnUpdateInterface);
        }

        void SendQuitRoomReq()
        {
            WorldQuitRoomReq req = new WorldQuitRoomReq();

            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }

        [UIEventHandle("btQuit")]
        void OnQuit()
        {
            if (ClientSystemManager.GetInstance().IsFrameOpen<PkSeekWaiting>())
            {
                SystemNotifyManager.SysNotifyFloatingEffect("匹配中无法进行该操作，请取消后再试");
                return;
            }

            SendQuitRoomReq();
        }

        [UIEventHandle("layout_players/Player{0}/Icon", typeof(Button), typeof(UnityEngine.Events.UnityAction<int>), 1, MemberNum)]
        void OnClickMember(int iIndex)
        {
            if (iIndex < 0)
            {
                return;
            }

            if(TeamDatas == null || TeamDatas.Count <= 0 || iIndex >= TeamDatas.Count)
            {
                return;
            }

            if(TeamDatas[iIndex].id == PlayerBaseData.GetInstance().RoleID)
            {
                return;
            }

            TeamMenuData menuData = new TeamMenuData();

            menuData.index = (byte)iIndex;
            menuData.memberID = TeamDatas[iIndex].id;
            menuData.name = TeamDatas[iIndex].name;
            menuData.occu = TeamDatas[iIndex].occu;
            menuData.level = TeamDatas[iIndex].level;

            menuData.Pos = new Vector3();

            if (frameMgr.IsFrameOpen<TeamMemberMenuFrame>())
            {
                frameMgr.CloseFrame<TeamMemberMenuFrame>();
            }

            frameMgr.OpenFrame<TeamMemberMenuFrame>(FrameLayer.Middle, menuData);
        }

        [UIEventHandle("layout_players/Player{0}/btOpenTeamMy", typeof(Button), typeof(UnityEngine.Events.UnityAction<int>), 1, MemberNum)]
        void OnOpenTeamMy(int iIndex)
        {
            ClientSystemManager.GetInstance().OpenFrame<Pk3v3CrossMyTeamFrame>();
        }

        [UIEventHandle("MyTeam")]
        void OnOpenMyTeam()
        {
            ClientSystemManager.GetInstance().OpenFrame<Pk3v3CrossMyTeamFrame>();
        }

        //[UIEventHandle("layout_players/Invite0/btnInvite")]     
        void OnOpenInvite()
        {
            Pk3v3CrossDataManager.Team teamData = Pk3v3CrossDataManager.GetInstance().GetMyTeam();
            if (teamData == null)
            {
                return;
            }

            if (teamData.leaderInfo.id == PlayerBaseData.GetInstance().RoleID)
            {
                ScoreWarStatus status = Pk3v3CrossDataManager.GetInstance().Get3v3CrossWarStatus();
                if (status != ScoreWarStatus.SWS_PREPARE && status != ScoreWarStatus.SWS_BATTLE)
                {
                    return;
                }

                Pk3v3CrossDataManager.My3v3PkInfo pkInfo = Pk3v3CrossDataManager.GetInstance().PkInfo;
                if (pkInfo != null && pkInfo.nCurPkCount >= Pk3v3CrossDataManager.MAX_PK_COUNT)
                {
                    SystemNotifyManager.SysNotifyFloatingEffect("挑战次数已达上限，操作失败");
                    return;
                }

                if (ClientSystemManager.GetInstance().IsFrameOpen<PkSeekWaiting>())
                {
                    SystemNotifyManager.SysNotifyFloatingEffect("匹配中无法进行该操作，请取消后再试");
                    return;
                }

                ClientSystemManager.GetInstance().OpenFrame<TeamInvitePlayerListFrame>(FrameLayer.Middle, InviteType.Pk3v3Invite);
            }                
        }


        void InitInterface()
        {
            UpdateInterface();
        }

        void UpdateInterface()
        {
            GetFrame().CustomActive(false);

            if(!Pk3v3CrossDataManager.GetInstance().HasTeam())
            {
                return;
            }
            GetFrame().CustomActive(true);

            Pk3v3CrossDataManager.Team teamData = Pk3v3CrossDataManager.GetInstance().GetMyTeam();
            if (teamData == null)
            {
                return;
            }

            TeamDatas.Clear();

            int iIndex = 0;
            for(int i = 0; i < teamData.members.Length; i++)
            {
                if(teamData.members[i].id <= 0)
                {
                    continue;
                }

                Pk3v3CrossDataManager.TeamMember memeberdata = new Pk3v3CrossDataManager.TeamMember();
                memeberdata = teamData.members[i];

                TeamDatas.Add(memeberdata);

                JobTable jobData = TableManager.GetInstance().GetTableItem<JobTable>(teamData.members[i].occu);
                if (jobData != null)
                {
                    ResTable resData = TableManager.GetInstance().GetTableItem<ResTable>(jobData.Mode);
                    if (resData != null)
                    {
                        //Sprite spr = AssetLoader.instance.LoadRes(resData.IconPath, typeof(Sprite)).obj as Sprite;
                        //if (spr != null)
                        //{
                        //    Icons[iIndex].sprite = spr;
                        //}
                        ETCImageLoader.LoadSprite(ref Icons[iIndex], resData.IconPath);
                    }
                }

                MemberNames[iIndex].text = teamData.members[i].name;
                Levels[iIndex].text = string.Format("Lv.{0}", teamData.members[i].level);

                if(jobData != null)
                {
                    Jobs[iIndex].text = jobData.Name;
                }

                int nSeasonLevel = (int)teamData.members[i].playerSeasonLevel;
                ETCImageLoader.LoadSprite(ref mainSeasonLv[i], SeasonDataManager.GetInstance().GetMainSeasonLevelSmallIcon(nSeasonLevel));          
                ETCImageLoader.LoadSprite(ref subSeasonLv[i], SeasonDataManager.GetInstance().GetSubSeasonLevelIcon(nSeasonLevel));


                //HPTexts[i].text = PlayerBaseData.GetInstance().MaxHP.ToString();

                if (teamData.leaderInfo.id == teamData.members[i].id)
                {
                    LeaderMarks[iIndex].gameObject.SetActive(true);
                }
                else
                {
                    LeaderMarks[iIndex].gameObject.SetActive(false);
                }
                GameObject go = mBind.GetGameObject(string.Format("Player{0}", iIndex));
                if (go != null)
                {
                    go.CustomActive(true);
                }

                iIndex++;
            }

            for(int i = iIndex; i < MemberNum; i++)
            {
                GameObject go = mBind.GetGameObject(string.Format("Player{0}", i));
                if (go != null)
                {
                    go.CustomActive(false);
                }
                //Players[i].gameObject.SetActive(false);
            }

            for(int i = 0;i < MemberNum;i++)
            {
                GameObject go = mBind.GetGameObject(string.Format("goInvite{0}", i));
                if(go != null)
                {
                    go.CustomActive(false);
                }
            }

            for(int i = iIndex;i < MemberNum;i++)
            {
                GameObject go = mBind.GetGameObject(string.Format("goInvite{0}", i));
                if (go != null)
                {
                    go.CustomActive(true);
                }
            }

//             if(imgInvite != null)
//             {
//                 imgInvite.CustomActive(TeamDatas.Count != MemberNum);
//             }
        }

        void OnUpdateInterface(UIEvent iEvent)
        {
            UpdateInterface();
        }


        [UIControl("layout_players/Player{0}/Icon", typeof(Image), 1)]
        protected Image[] Icons = new Image[MemberNum];

        [UIControl("layout_players/Player{0}/Name", typeof(Text), 1)]
        protected Text[] MemberNames = new Text[MemberNum];

        [UIControl("layout_players/Player{0}/Level", typeof(Text), 1)]
        protected Text[] Levels = new Text[MemberNum];

        [UIControl("layout_players/Player{0}/LeaderMark", typeof(Image), 1)]
        protected Image[] LeaderMarks = new Image[MemberNum];

        [UIControl("layout_players/Player{0}/Job", typeof(Text), 1)]
        protected Text[] Jobs = new Text[MemberNum];

        [UIControl("layout_players/Player{0}/PkGroup/Main", typeof(Image), 1)]
        protected Image[] mainSeasonLv = new Image[MemberNum];

        [UIControl("layout_players/Player{0}/PkGroup/Main/Sub", typeof(Image), 1)]
        protected Image[] subSeasonLv = new Image[MemberNum];

        [UIControl("layout_players/Invite{0}/btnBk", typeof(Button), 0)]
        protected Button[] btnBk = new Button[MemberNum];
      
    }
}
