using Network;
using Protocol;
using UnityEngine;
using ProtoTable;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;

namespace GameClient
{
    public class Pk3v3PlayerMenuFrame : ClientFrame
    {
        RoomSlotInfo slotinfo = null;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Pk3v3/Pk3v3Menu";
        }

        protected override void _OnOpenFrame()
        {
            if(userData != null)
            {
                slotinfo = (RoomSlotInfo)userData;
            }

            InitInterface();
            BindUIEvent();

           

            if (Pk3v3DataManager.GetInstance().isNotify)
            {
                RefreshChangePosState();
            }
        }

        protected override void _OnCloseFrame()
        {
            ClearData();
            UnBindUIEvent();
        }

        void ClearData()
        {
            slotinfo = null;
        }

        protected override void _OnUpdate(float timeElapsed)
        {
            RefreshChangePosState();
        }
        public override bool IsNeedUpdate()
        {
            return Pk3v3DataManager.GetInstance().isNotify;
        }
        void RefreshChangePosState()
        {
            mChangePosGray.enabled = true;
            mChangePos.interactable = false;

            int iInt = Pk3v3DataManager.GetInstance().iInt;
            mChangePosTimer.text = string.Format("{0}s", iInt);

            if (iInt < 0.05)
            {
                mChangePosGray.enabled = false;
                mChangePosTimer.text = "";
                Pk3v3DataManager.GetInstance().isNotify = false;
                mChangePos.interactable = true;
            }
        }

        public void SetPosition(Vector3 pos)
        {
            Vector3 newpos = new Vector3();
            newpos = pos;

            newpos.y -= 60.0f;

            mContentRect.position = newpos;
        }

        protected void BindUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.Pk3v3PlayerLeave, OnPlayerLeave);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.Pk3v3VoteEnterBattle, OnPk3v3VoteEnterBattle);
        }

        protected void UnBindUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.Pk3v3PlayerLeave, OnPlayerLeave);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.Pk3v3VoteEnterBattle, OnPk3v3VoteEnterBattle);
        }

        void OnPlayerLeave(UIEvent iEvent)
        {
            byte bGroup = (byte)iEvent.Param1;
            byte bIndex = (byte)iEvent.Param2;

            if(slotinfo.group == bGroup && slotinfo.index == bIndex)
            {
                frameMgr.CloseFrame(this);
            }
        }

        void OnPk3v3VoteEnterBattle(UIEvent iEvent)
        {
            frameMgr.CloseFrame(this);
        }

        void InitInterface()
        {
            RoomInfo roomInfo = Pk3v3DataManager.GetInstance().GetRoomInfo();

            if (roomInfo == null)
            {
                Logger.LogError("roomInfo is null from Pk3v3PlayerMenuFrame");
                return;
            }

            if (slotinfo == null)
            {
                Logger.LogError("pk3v3 slotinfo is null");
                return;
            }

            bool bShowChangePos = true;
//             for(int i = 0; i < roomInfo.roomSlotInfos.Length; i++)
//             {
//                 if(roomInfo.roomSlotInfos[i].playerId == PlayerBaseData.GetInstance().RoleID)
//                 {
//                     if(roomInfo.roomSlotInfos[i].group != slotinfo.group)
//                     {
//                         bShowChangePos = true;
//                     }
// 
//                     break;
//                 }
//             }

            mChangePos.gameObject.CustomActive(bShowChangePos);
            mLookInfo.gameObject.CustomActive(slotinfo.playerId != 0);
            mAddFriend.gameObject.CustomActive(slotinfo.playerId != 0);
            mTransfer.gameObject.CustomActive(roomInfo.roomSimpleInfo.ownerId == PlayerBaseData.GetInstance().RoleID && slotinfo.playerId != 0);
            mKickOutRoom.gameObject.CustomActive(roomInfo.roomSimpleInfo.ownerId == PlayerBaseData.GetInstance().RoleID && slotinfo.playerId != 0);
            mClosePos.gameObject.CustomActive(roomInfo.roomSimpleInfo.ownerId == PlayerBaseData.GetInstance().RoleID);

            int iZoneID = 0;
            ClientSystemTown town = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
            if(town != null)
            {
                BeTownPlayer player = town.GetTownPlayer(slotinfo.playerId);
                if(player != null)
                {
                    iZoneID = player.GetPlayerZoneID();
                }
            }
            mLookInfo.gameObject.CustomActive(false);
            mAddFriend.gameObject.CustomActive(slotinfo.playerId != 0 && iZoneID > 0 && iZoneID == PlayerBaseData.GetInstance().ZoneID);
        }

        void SendLookInfoReq()
        {
            WorldQueryPlayerReq kCmd = new WorldQueryPlayerReq();

            kCmd.roleId = slotinfo.playerId;
            kCmd.name = "";
            OtherPlayerInfoManager.GetInstance().QueryPlayerType = WorldQueryPlayerType.WQPT_WATCH_PLAYER_INTO;

            NetManager.instance.SendCommand(ServerType.GATE_SERVER, kCmd);
        }

        void SendAddFriendReq()
        {
            SceneRequest req = new SceneRequest();

            req.type = (byte)RequestType.RequestFriend;
            req.target = slotinfo.playerId;
            req.targetName = "";
            req.param = 0;

            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }

        void SendTransferPosReq()
        {
            WorldChangeRoomOwnerReq req = new WorldChangeRoomOwnerReq();

            req.playerId = slotinfo.playerId;

            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }

        void SendKickOutRoomReq()
        {
            WorldKickOutRoomReq req = new WorldKickOutRoomReq();

            req.playerId = slotinfo.playerId;

            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }

        #region ExtraUIBind
        private Button mLookInfo = null;
        private Button mChangePos = null;
        private Button mAddFriend = null;
        private Button mTransfer = null;
        private Button mKickOutRoom = null;
        private Button mClosePos = null;
        private RectTransform mContentRect = null;
        private Button mBtClose = null;
        private Text mChangePosTimer = null;
        private UIGray mChangePosGray = null;

        protected override void _bindExUI()
        {
            mLookInfo = mBind.GetCom<Button>("LookInfo");
            mLookInfo.onClick.AddListener(_onLookInfoButtonClick);
            mChangePos = mBind.GetCom<Button>("ChangePos");
            mChangePos.onClick.AddListener(_onChangePosButtonClick);
            mAddFriend = mBind.GetCom<Button>("AddFriend");
            mAddFriend.onClick.AddListener(_onAddFriendButtonClick);
            mTransfer = mBind.GetCom<Button>("Transfer");
            mTransfer.onClick.AddListener(_onTransferButtonClick);
            mKickOutRoom = mBind.GetCom<Button>("KickOutRoom");
            mKickOutRoom.onClick.AddListener(_onKickOutRoomButtonClick);
            mClosePos = mBind.GetCom<Button>("ClosePos");
            mClosePos.onClick.AddListener(_onClosePosButtonClick);
            mContentRect = mBind.GetCom<RectTransform>("ContentRect");
            mBtClose = mBind.GetCom<Button>("btClose");
            mBtClose.onClick.AddListener(_onBtCloseButtonClick);
            mChangePosTimer = mBind.GetCom<Text>("ChangePosTimer");
            mChangePosGray = mBind.GetCom<UIGray>("ChangePosGray");
        }

        protected override void _unbindExUI()
        {
            mLookInfo.onClick.RemoveListener(_onLookInfoButtonClick);
            mLookInfo = null;
            mChangePos.onClick.RemoveListener(_onChangePosButtonClick);
            mChangePos = null;
            mAddFriend.onClick.RemoveListener(_onAddFriendButtonClick);
            mAddFriend = null;
            mTransfer.onClick.RemoveListener(_onTransferButtonClick);
            mTransfer = null;
            mKickOutRoom.onClick.RemoveListener(_onKickOutRoomButtonClick);
            mKickOutRoom = null;
            mClosePos.onClick.RemoveListener(_onClosePosButtonClick);
            mClosePos = null;
            mContentRect = null;
            mBtClose.onClick.RemoveListener(_onBtCloseButtonClick);
            mBtClose = null;
            mChangePosTimer = null;
            mChangePosGray = null;
        }
        #endregion

        #region Callback
        private void _onLookInfoButtonClick()
        {
            if (slotinfo.playerId == 0)
            {
                SystemNotifyManager.SystemNotify(3406004);
                return;
            }

            SendLookInfoReq();

            frameMgr.CloseFrame(this);
        }
        private void _onChangePosButtonClick()
        {
            if (slotinfo == null)
            {
                return;
            }

            if (Pk3v3DataManager.GetInstance().CheckRoomIsMatching())
            {
                return;
            }

            RoomInfo roomInfo = Pk3v3DataManager.GetInstance().GetRoomInfo();

            if (roomInfo == null)
            {
                return;
            }

//             for (int i = 0; i < roomInfo.roomSlotInfos.Length; i++)
//             {
//                 if (roomInfo.roomSlotInfos[i].playerId == PlayerBaseData.GetInstance().RoleID)
//                 {
//                     if (roomInfo.roomSlotInfos[i].group == slotinfo.group)
//                     {
//                         return;
//                     }
//                 }
//             }

            Pk3v3DataManager.GetInstance().SendPk3v3ChangePosReq(roomInfo.roomSimpleInfo.id, slotinfo);

            if (slotinfo.playerId > 0)
            {
                var data = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_ROOM_CHANGEPOS_PROTECT_TIME);
                if (data != null)
                {
                    float fChangePosLocationTime = data.Value;
                    Pk3v3DataManager.GetInstance().SetCountDownTime(fChangePosLocationTime);
                }
            }

            frameMgr.CloseFrame(this);
        }
        private void _onAddFriendButtonClick()
        {
            if (slotinfo.playerId == 0)
            {
                SystemNotifyManager.SystemNotify(3406004);
                return;
            }

            SendAddFriendReq();

            frameMgr.CloseFrame(this);
        }
        private void _onTransferButtonClick()
        {
            if (Pk3v3DataManager.GetInstance().CheckRoomIsMatching())
            {
                return;
            }

            RoomInfo roomInfo = Pk3v3DataManager.GetInstance().GetRoomInfo();

            if (roomInfo == null)
            {
                return;
            }

            if (slotinfo.playerId == 0)
            {
                SystemNotifyManager.SystemNotify(3406004);
                return;
            }

            if (PlayerBaseData.GetInstance().RoleID != roomInfo.roomSimpleInfo.ownerId)
            {
                return;
            }

            SendTransferPosReq();

            frameMgr.CloseFrame(this);
        }
        private void _onKickOutRoomButtonClick()
        {
            if (Pk3v3DataManager.GetInstance().CheckRoomIsMatching())
            {
                return;
            }

            RoomInfo roomInfo = Pk3v3DataManager.GetInstance().GetRoomInfo();

            if (roomInfo == null)
            {
                return;
            }

            if (slotinfo.playerId == 0)
            {
                SystemNotifyManager.SystemNotify(3406004);
                return;
            }

            if (PlayerBaseData.GetInstance().RoleID != roomInfo.roomSimpleInfo.ownerId)
            {
                return;
            }

            SendKickOutRoomReq();

            frameMgr.CloseFrame(this);
        }
        private void _onClosePosButtonClick()
        {
            if (Pk3v3DataManager.GetInstance().CheckRoomIsMatching())
            {
                return;
            }

            RoomInfo roomInfo = Pk3v3DataManager.GetInstance().GetRoomInfo();

            if (roomInfo == null)
            {
                return;
            }

            if (slotinfo.playerId != 0)
            {
                //return;
            }

            if(PlayerBaseData.GetInstance().RoleID != roomInfo.roomSimpleInfo.ownerId)
            {
                return;
            }

            Pk3v3DataManager.GetInstance().SendClosePosReq(slotinfo.group, slotinfo.index);

            frameMgr.CloseFrame(this);
        }

        private void _onBtCloseButtonClick()
        {
            frameMgr.CloseFrame(this);
        }
        #endregion
    }
}
