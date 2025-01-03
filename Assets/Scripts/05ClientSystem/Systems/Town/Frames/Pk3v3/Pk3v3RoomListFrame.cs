using Network;
using Protocol;
using UnityEngine;
using ProtoTable;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;
using Scripts.UI;

namespace GameClient
{
    public class Pk3v3RoomListFrame : ClientFrame
    {
        const string elementBgPath1 = "UI/Image/NewPacked/Juedou_Battle.png:Juedou_Liebiao_Item01";
        const string elementBgPath2 = "UI/Image/NewPacked/Juedou_Battle.png:Juedou_Liebiao_Item02";
        int RoomNumPerPage = 10;
        const float RequestTimeIntrval = 5.0f;

        float TimeIntrval = 0.0f;
        bool IsFliterRequest = false;

        RoomListInfo roomList = new RoomListInfo();

        int CurPage = 0;
        int MaxPage = 0;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Pk3v3/Pk3v3RoomListFrame";
        }

        protected override void _OnOpenFrame()
        {
            InitData();
            SendNormalRoomListReq();
            InitInterface();
            BindUIEvent();
        }

        protected override void _OnCloseFrame()
        {
            ClearData();
            UnBindUIEvent();
        }

        void ClearData()
        {
            TimeIntrval = 0.0f;
            IsFliterRequest = false;
            roomList = null;
            CurPage = 0;
            MaxPage = 0;
        }

        protected void BindUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.Pk3v3InviteRoomListUpdate, OnInviteRoomListUpdate);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.Pk3v3RefreshRoomList, OnRefreshRoomList);
        }

        protected void UnBindUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.Pk3v3InviteRoomListUpdate, OnInviteRoomListUpdate);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.Pk3v3RefreshRoomList, OnRefreshRoomList);
        }

        void OnInviteRoomListUpdate(UIEvent iEvent)
        {
            UpdateInviteList();
        }

        void OnRefreshRoomList(UIEvent iEvent)
        {
            SendRoomListReq();
        }

        void OnJoinRoom(int iIndex)
        {
            if (roomList == null || roomList.rooms == null)
            {
                return;
            }

            if(iIndex < 0 || iIndex >= roomList.rooms.Length)
            {
                return;
            }

            if(roomList.rooms[iIndex].playerSize >= roomList.rooms[iIndex].playerMaxSize)
            {
                SystemNotifyManager.SysNotifyFloatingEffect("房间人数已满");
                return;
            }

            if(roomList.rooms[iIndex].limitPlayerLevel > PlayerBaseData.GetInstance().Level)
            {
                SystemNotifyManager.SysNotifyFloatingEffect("等级不足");
                return;
            }

            if (roomList.rooms[iIndex].limitPlayerSeasonLevel > SeasonDataManager.GetInstance().seasonLevel)
            {
                SystemNotifyManager.SysNotifyFloatingEffect("段位不足");
                return;
            }
           
            if ((RoomStatus)roomList.rooms[iIndex].roomStatus != RoomStatus.ROOM_STATUS_OPEN)
            {
                SystemNotifyManager.SysNotifyFloatingEffect("该房间已进入比赛");
                return;
            }

            if (roomList.rooms[iIndex].isPassword > 0)
            {
                if(ClientSystemManager.GetInstance().IsFrameOpen<Pk3v3CheckPasswordFrame>())
                {
                    ClientSystemManager.GetInstance().CloseFrame<Pk3v3CheckPasswordFrame>();
                }

                ClientSystemManager.GetInstance().OpenFrame<Pk3v3CheckPasswordFrame>(FrameLayer.Middle, roomList.rooms[iIndex]);

                return;
            }

            Pk3v3DataManager.SendJoinRoomReq(roomList.rooms[iIndex].id, (RoomType)roomList.rooms[iIndex].roomType);
        }

        void InitInterface()
        {         
            InitRoomScrollListBind();
            UpdateInviteList();
        }

        void InitData()
        {
            var systemValueTableData = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_3V3_ROOM_LIMIT_NUM);
            if(systemValueTableData != null)
            {
                RoomNumPerPage = systemValueTableData.Value;
            }
        }

        void InitRoomScrollListBind()
        {
            mUIListScript.Initialize();

            mUIListScript.onItemVisiable = (item) =>
            {
                if (item.m_index >= 0)
                {
                    UpdateRoomScrollListBind(item);
                }
            };

            mUIListScript.OnItemRecycle = (item) =>
            {
                ComCommonBind combind = item.GetComponent<ComCommonBind>();
                if (combind == null)
                {
                    return;
                }

                Button btJoin = combind.GetCom<Button>("join");
                btJoin.onClick.RemoveAllListeners();
            };
        }

        void UpdateRoomScrollListBind(ComUIListElementScript item)
        {
            ComCommonBind combind = item.GetComponent<ComCommonBind>();
            if (combind == null)
            {
                return;
            }

            if (item.m_index < 0 || item.m_index >= roomList.rooms.Length)
            {
                return;
            }

            RoomSimpleInfo roominfo = roomList.rooms[item.m_index];

            Text id = combind.GetCom<Text>("id");
            Text RoomName = combind.GetCom<Text>("RoomName");
            Text state = combind.GetCom<Text>("state");
            Text rank = combind.GetCom<Text>("rank");
            Text lv = combind.GetCom<Text>("lv");
            Text roomtype = combind.GetCom<Text>("roomtype");
            Text num = combind.GetCom<Text>("num");
            Button btJoin = combind.GetCom<Button>("join");
            UIGray joinGray = combind.GetCom<UIGray>("JoinUIGray");
            Image Lock = combind.GetCom<Image>("Lock");
            Image bg = combind.GetCom<Image>("bg");

            int index = item.m_index % 2;
            if(bg != null)
            {
                if(index == 0)
                {
                    ETCImageLoader.LoadSprite(ref bg, elementBgPath1);
                }
                else
                {
                    ETCImageLoader.LoadSprite(ref bg, elementBgPath2);
                }
            }

            id.text = roominfo.id.ToString();
            Lock.gameObject.CustomActive(roominfo.isPassword > 0);
            RoomName.text = roominfo.name;
            state.text = Pk3v3DataManager.GetInstance().GetRoomState((RoomStatus)roominfo.roomStatus);

            if(SeasonDataManager.GetInstance().seasonLevel >= roominfo.limitPlayerSeasonLevel)
            {
                rank.text = string.Format("<color=#ffffffff>{0}</color>", SeasonDataManager.GetInstance().GetSimpleRankName((int)roominfo.limitPlayerSeasonLevel));
            }
            else
            {
                rank.text = string.Format("<color=#1ED355FF>{0}</color>", SeasonDataManager.GetInstance().GetSimpleRankName((int)roominfo.limitPlayerSeasonLevel));
            }      

            if(PlayerBaseData.GetInstance().Level >= roominfo.limitPlayerLevel)
            {
                lv.text = string.Format("<color=#ffffffff>{0}</color>", roominfo.limitPlayerLevel);
            }
            else
            {
                lv.text = string.Format("<color=#f0cd0dff>{0}</color>", roominfo.limitPlayerLevel);
            }
           
            roomtype.text = Pk3v3DataManager.GetInstance().GetRoomType((RoomType)roominfo.roomType);

            if(roominfo.playerSize < roominfo.playerMaxSize)
            {
                num.text = string.Format("<color=#ffffffff>{0}/{1}</color>", roominfo.playerSize, roominfo.playerMaxSize);
            }
            else
            {
                num.text = string.Format("<color=#f0cd0dff>{0}/{1}</color>", roominfo.playerSize, roominfo.playerMaxSize);
            }           

            if(joinGray != null)
            {
                if((RoomStatus)roominfo.roomStatus == RoomStatus.ROOM_STATUS_BATTLE || roominfo.playerSize >= roominfo.playerMaxSize || 
                    roominfo.limitPlayerLevel > PlayerBaseData.GetInstance().Level || roominfo.limitPlayerSeasonLevel > SeasonDataManager.GetInstance().seasonLevel)
                {
                    joinGray.enabled = true;
                    btJoin.interactable = false;
                }
                else
                {
                    joinGray.enabled = false;
                    btJoin.interactable = true;
                }
            }
            
            btJoin.onClick.RemoveAllListeners();
            int iIndex = item.m_index;
            btJoin.onClick.AddListener(() => { OnJoinRoom(iIndex); });
        }

        void RefreshPetItemListCount()
        {
            mUIListScript.SetElementAmount(roomList.rooms.Length);
        }

        void UpdateInviteList()
        {
            List<WorldSyncRoomInviteInfo> InviteRoomList = Pk3v3DataManager.GetInstance().GetInviteRoomList();

            mInviteImg.gameObject.CustomActive(InviteRoomList.Count > 0);
            mIniteNum.gameObject.CustomActive(InviteRoomList.Count > 0);
            mIniteNum.text = InviteRoomList.Count.ToString();
        }

        void UpdatePage()
        {
            UIGray uiLeftgray = mBtLeft.GetComponent<UIGray>();
            UIGray uiRightgray = mBtRight.GetComponent<UIGray>();

            uiLeftgray.enabled = false;
            uiRightgray.enabled = false;

            if (MaxPage <= 1)
            {
                uiLeftgray.enabled = true;
                uiRightgray.enabled = true;
            }
            else
            {
                if(CurPage >= (MaxPage - 1))
                {
                    uiRightgray.enabled = true;
                }
                else if(CurPage <= 0)
                {
                    uiLeftgray.enabled = true;
                }
            }

            mShowPage.text = string.Format("{0}/{1}", CurPage + 1, MaxPage);
        }

        void SendRoomListReq()
        {
            if(CurPage < 0)
            {
                CurPage = 0;
            }

            if (!IsFliterRequest)
            {
                SendNormalRoomListReq();
            }
            else
            {
                SendFliterRoomListReq();
            }
        }

        public override bool IsNeedUpdate()
        {
            return true;
        }

        protected override void _OnUpdate(float timeElapsed)
        {
            TimeIntrval += timeElapsed;

            if(TimeIntrval >= RequestTimeIntrval)
            {
                TimeIntrval = 0.0f;

                SendRoomListReq();
            }
        }

        [MessageHandle(WorldRoomListRes.MsgID)]
        void OnRoomListRes(MsgDATA msg)
        {
            WorldRoomListRes res = new WorldRoomListRes();
            res.decode(msg.bytes);

            if (res.result != (uint)ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int)res.result);
                return;
            }

            roomList = res.roomList;

            MaxPage = (int)res.roomList.total / RoomNumPerPage;
            if (res.roomList.total % RoomNumPerPage != 0)
            {
                MaxPage += 1;
            }

            if(CurPage > (MaxPage - 1))
            {
                CurPage = MaxPage - 1;
            }

            if(CurPage < 0)
            {
                CurPage = -1;
            }

            RefreshPetItemListCount();
            UpdatePage();
        }

        void SendNormalRoomListReq()
        {
            WorldRoomListReq req = new WorldRoomListReq();

            req.count = (uint)RoomNumPerPage;
            req.startIndex = (uint)(CurPage * RoomNumPerPage);
            req.roomType = (byte)RoomType.ROOM_TYPE_THREE_FREE;

            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }

        void SendFliterRoomListReq()
        {
            WorldRoomListReq req = new WorldRoomListReq();

            req.count = (uint)RoomNumPerPage;
            req.startIndex = 0;
            req.roomType = (byte)RoomType.ROOM_TYPE_THREE_FREE;

            req.limitPlayerLevel = PlayerBaseData.GetInstance().Level;
            req.limitPlayerSeasonLevel = (uint)SeasonDataManager.GetInstance().seasonLevel;
            req.roomStatus = (byte)RoomStatus.ROOM_STATUS_OPEN;
            req.isPassword = 0;

            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }

        #region ExtraUIBind
        private Button mBtClose = null;
        private ComUIListScript mUIListScript = null;
        private Button mBtInviteInfo = null;
        private Button mBtCreateRoom = null;
        private Button mBtAmusementRoom = null;
        private Button mBtRankRoom = null;
        private Button mBtLeft = null;
        private Button mBtRight = null;
        private Text mShowPage = null;
        private Toggle mConditionFliter = null;
        private Image mInviteImg = null;
        private Text mIniteNum = null;

        protected override void _bindExUI()
        {
            mBtClose = mBind.GetCom<Button>("btClose");
            mBtClose.onClick.AddListener(_onBtCloseButtonClick);
            mUIListScript = mBind.GetCom<ComUIListScript>("UIListScript");
            mBtInviteInfo = mBind.GetCom<Button>("btInviteInfo");
            mBtInviteInfo.onClick.AddListener(_onBtInviteInfoButtonClick);
            mBtCreateRoom = mBind.GetCom<Button>("btCreateRoom");
            mBtCreateRoom.onClick.AddListener(_onBtCreateRoomButtonClick);
            mBtAmusementRoom = mBind.GetCom<Button>("btAmusementRoom");
            mBtAmusementRoom.onClick.AddListener(_onBtAmusementRoomButtonClick);
            mBtRankRoom = mBind.GetCom<Button>("btRankRoom");
            mBtRankRoom.onClick.AddListener(_onBtRankRoomButtonClick);
            mBtLeft = mBind.GetCom<Button>("btLeft");
            mBtLeft.onClick.AddListener(_onBtLeftButtonClick);
            mBtRight = mBind.GetCom<Button>("btRight");
            mBtRight.onClick.AddListener(_onBtRightButtonClick);
            mShowPage = mBind.GetCom<Text>("ShowPage");
            mConditionFliter = mBind.GetCom<Toggle>("ConditionFliter");
            mConditionFliter.onValueChanged.AddListener(_onConditionFliterToggleValueChange);
            mInviteImg = mBind.GetCom<Image>("InviteImg");
            mIniteNum = mBind.GetCom<Text>("IniteNum");
        }

        protected override void _unbindExUI()
        {
            mBtClose.onClick.RemoveListener(_onBtCloseButtonClick);
            mBtClose = null;
            mUIListScript = null;
            mBtInviteInfo.onClick.RemoveListener(_onBtInviteInfoButtonClick);
            mBtInviteInfo = null;
            mBtCreateRoom.onClick.RemoveListener(_onBtCreateRoomButtonClick);
            mBtCreateRoom = null;
            mBtAmusementRoom.onClick.RemoveListener(_onBtAmusementRoomButtonClick);
            mBtAmusementRoom = null;
            mBtRankRoom.onClick.RemoveListener(_onBtRankRoomButtonClick);
            mBtRankRoom = null;
            mBtLeft.onClick.RemoveListener(_onBtLeftButtonClick);
            mBtLeft = null;
            mBtRight.onClick.RemoveListener(_onBtRightButtonClick);
            mBtRight = null;
            mShowPage = null;
            mConditionFliter.onValueChanged.RemoveListener(_onConditionFliterToggleValueChange);
            mConditionFliter = null;
            mInviteImg = null;
            mIniteNum = null;
        }
        #endregion

        #region Callback
        private void _onBtCloseButtonClick()
        {
            frameMgr.CloseFrame(this);
        }
        private void _onBtInviteInfoButtonClick()
        {
            ClientSystemManager.instance.OpenFrame<TeamBeInvitedListFrame>(FrameLayer.Middle, InviteType.Pk3v3Invite);
        }
        private void _onBtCreateRoomButtonClick()
        {
            if (ClientSystemManager.GetInstance().IsFrameOpen<Pk3v3TypeChooseFrame>())
            {
                return;
            }

            ClientSystemManager.GetInstance().OpenFrame<Pk3v3TypeChooseFrame>(FrameLayer.Middle);
            //Pk3v3DataManager.GetInstance().SendCreateRoomReq(RoomType.ROOM_TYPE_THREE_FREE);
        }
        private void _onBtAmusementRoomButtonClick()
        {
            Pk3v3DataManager.SendJoinRoomReq(0, RoomType.ROOM_TYPE_THREE_FREE);
        }
        private void _onBtRankRoomButtonClick()
        {
            if (ServerSceneFuncSwitchManager.GetInstance().IsTypeFuncLock(ServiceType.SERVICE_3v3_TUMBLE))
            {
                SystemNotifyManager.SystemNotify(5);
                return;
            }
            Pk3v3DataManager.SendJoinRoomReq(0, RoomType.ROOM_TYPE_MELEE);
        }
        private void _onBtLeftButtonClick()
        {
            if(CurPage <= 0)
            {
                return;
            }

            CurPage--;

            SendRoomListReq();
            UpdatePage();
        }
        private void _onBtRightButtonClick()
        {
            if (CurPage >= (MaxPage - 1))
            {
                return;
            }

            CurPage++;            

            SendRoomListReq();
            UpdatePage();
        }
        private void _onConditionFliterToggleValueChange(bool changed)
        {
            IsFliterRequest = changed;
            SendRoomListReq();
        }
        #endregion
    }
}
