using UnityEngine;
using UnityEngine.UI;
using Protocol;
using Network;

namespace GameClient
{
    class TeamMenuData
    {
        public byte index;
        public ulong memberID;
        public string name;
        public byte occu;
        public ushort level;
        public Vector3 Pos;
    }

    class TeamMemberMenuFrame : ClientFrame
    {
        TeamMenuData uiData = new TeamMenuData();
        bool isLeader = false;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Team/TeamMemberMenu";
        }

        protected override void _OnOpenFrame()
        {
            uiData = userData as TeamMenuData;
            InitInterface();
        }

        protected override void _OnCloseFrame()
        {
            Clear();
        }

        void Clear()
        {
            isLeader = false;
        }

        [UIEventHandle("btClose")]
        void OnClose()
        {
            frameMgr.CloseFrame(this);
        }

        [UIEventHandle("back/funcs/PrivateTalk/Button")]
        void OnPrivateTalk()
        {
            RelationData data = new RelationData();

            data.uid = uiData.memberID;
            data.name = uiData.name;
            data.occu = uiData.occu;
            data.level = uiData.level;

            RelationDataManager.GetInstance().OnPrivateChat(data);

            OnClose();
        }

        [UIEventHandle("back/funcs/WatchInfo/Button")]
        void OnWatchInfo()
        {
            WorldQueryPlayerReq kCmd = new WorldQueryPlayerReq();
            kCmd.roleId = uiData.memberID;
            kCmd.name = "";
            OtherPlayerInfoManager.GetInstance().QueryPlayerType = WorldQueryPlayerType.WQPT_WATCH_PLAYER_INTO;
            NetManager.instance.SendCommand(ServerType.GATE_SERVER, kCmd);

            OnClose();
        }

        [UIEventHandle("back/funcs/MakeFriend/Button")]
        void OnMakeFriend()
        {
            if (uiData.memberID == 0)
            {
                return;
            }

            var relationData = RelationDataManager.GetInstance().GetRelationByRoleID(uiData.memberID);
            if (relationData != null && relationData.type == (byte)RelationType.RELATION_FRIEND)
            {
                SystemNotifyManager.SysNotifyFloatingEffect("该玩家已经是你的好友了");
                return;
            }

            SceneRequest req = new SceneRequest();

            req.type = (byte)RequestType.RequestFriend;
            req.target = uiData.memberID;
            req.targetName = "";
            req.param = 0;

            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);

            OnClose();
        }

        [UIEventHandle("back/funcs/KickOutTeam/Button")]
        void OnKickOutTeam()
        {
            if(!isLeader)
            {
                return;
            }

            if (ClientSystemManager.GetInstance().IsFrameOpen<PkSeekWaiting>())
            {
                SystemNotifyManager.SysNotifyFloatingEffect("匹配中无法进行该操作，请取消后再试");
                return;
            }

            Pk3v3CrossDataManager.Team myTeam = Pk3v3CrossDataManager.GetInstance().GetMyTeam();
            if (myTeam != null && myTeam.leaderInfo.id == PlayerBaseData.GetInstance().RoleID)
            {
                WorldKickOutRoomReq req = new WorldKickOutRoomReq();

                req.playerId = uiData.memberID;

                NetManager netMgr = NetManager.Instance();
                netMgr.SendCommand(ServerType.GATE_SERVER, req);

                OnClose();
                return;
            }

            TeamDataManager.GetInstance().KickTeamMember(uiData.memberID);

            OnClose();
        }

        [UIEventHandle("back/funcs/ChangeLeader/Button")]
        void OnChangeLeader()
        {
            if (!isLeader)
            {
                return;
            }

            if (ClientSystemManager.GetInstance().IsFrameOpen<PkSeekWaiting>())
            {
                SystemNotifyManager.SysNotifyFloatingEffect("匹配中无法进行该操作，请取消后再试");
                return;
            }

            Pk3v3CrossDataManager.Team myTeam = Pk3v3CrossDataManager.GetInstance().GetMyTeam();
            if (myTeam != null && myTeam.leaderInfo.id == PlayerBaseData.GetInstance().RoleID)
            {
                WorldChangeRoomOwnerReq req = new WorldChangeRoomOwnerReq();

                req.playerId = uiData.memberID;

                NetManager netMgr = NetManager.Instance();
                netMgr.SendCommand(ServerType.GATE_SERVER, req);

                OnClose();
                return;
            }

            TeamDataManager.GetInstance().ChangeTeamLeader(uiData.memberID);

            OnClose();
        }

        [UIEventHandle("back/funcs/BtnReport/Button")]
        void OnReport()
        {
            if (uiData.memberID == 0)
            {
                return;
            }
            InformantInfo info = new InformantInfo();
            info.roleId = uiData.memberID.ToString();
            info.roleName = uiData.name;
            info.roleLevel = uiData.level.ToString();
            info.jobId = uiData.occu.ToString();
            info.jobName = BaseWebViewManager.GetInstance().GetJobNameByJobId((int)uiData.occu);

            BaseWebViewManager.GetInstance().TryOpenReportFrame(info);
            frameMgr.CloseFrame(this);
        }

        void InitInterface()
        {
            isLeader = TeamDataManager.GetInstance().IsTeamLeader();

            Pk3v3CrossDataManager.Team myTeam = Pk3v3CrossDataManager.GetInstance().GetMyTeam();
            if(myTeam != null && myTeam.leaderInfo.id == PlayerBaseData.GetInstance().RoleID)
            {
                isLeader = true;
            }
         
            if(!isLeader)
            {
                btKickOutTeam.gameObject.SetActive(false);
                btChangeLeader.gameObject.SetActive(false);
            }

            if (frameMgr.IsFrameOpen<Pk3v3CrossWaitingRoom>())
            {
                btMakeFriend.CustomActive(false);
            }

            btReport.CustomActive(BaseWebViewManager.GetInstance().IsReportFuncOpen());

            SetupFramePosition(uiData.Pos);
        }

        void SetupFramePosition(Vector3 pos)
        {
            //frame.transform.position = pos;

            //             RectTransform rectParent = frame.GetComponent<RectTransform>();
            //             RectTransform rectContent = m_content.GetComponent<RectTransform>();
            //             LayoutRebuilder.ForceRebuildLayoutImmediate(rectContent);
            //             float xMin = 0.0f;
            //             float xMax = rectParent.rect.size.x - rectContent.rect.size.x;
            //             float yMin = rectContent.rect.size.y - rectParent.rect.size.y;
            //             float yMax = 0.0f;
            // 
            //             Vector2 localPos;
            //             bool success = RectTransformUtility.ScreenPointToLocalPointInRectangle(rectParent, pos, ClientSystemManager.GetInstance().UICamera, out localPos);
            //             if (success)
            //             {
            //                 if (localPos.x < xMin)
            //                 {
            //                     localPos.x = xMin;
            //                 }
            //                 else if (localPos.x > xMax)
            //                 {
            //                     localPos.x = localPos.x - rectContent.rect.size.x;
            //                 }
            // 
            //                 if (localPos.y < yMin)
            //                 {
            //                     localPos.y = localPos.y + rectContent.rect.size.y;
            //                 }
            //                 else if (localPos.y > yMax)
            //                 {
            //                     localPos.y = yMax;
            //                 }
            // 
            //                 rectContent.anchoredPosition = localPos;
            //             }
        }

//         void _OnChangeLeaderClicked(ulong id)
//         {
//             TeamDataManager.GetInstance().ChangeTeamLeader(id);
//         }

        [UIObject("back/funcs/KickOutTeam")]
        GameObject btKickOutTeam;

        [UIObject("back/funcs/ChangeLeader")]
        GameObject btChangeLeader;

        [UIObject("back/funcs/MakeFriend")]
        GameObject btMakeFriend;

        [UIObject("back/funcs/BtnReport")]
        GameObject btReport;
    }
}
