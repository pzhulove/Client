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
    public class Pk3v3WaitingRoom : ClientFrame
    {
        string PlayerInfoElePath = "UIFlatten/Prefabs/Pk3v3/Pk3v3PlayerInfo";
        string StartBtnRedPath = "UI/Image/Packed/p_UI_Common00.png:UI_Tongyong_Anniu_Quren_Xuanzhong_06";
        string StartBtnBluePath= "UI/Image/Packed/p_UI_Common00.png:UI_Tongyong_Anniu_Lanse_01";
        const int MaxPlayerNum = 3;
        const int MaxMeleePlayerNum = 2;

        PkWaitingRoomData RoomData = new PkWaitingRoomData();

        RoomSlotGroup SelfGroup = RoomSlotGroup.ROOM_SLOT_GROUP_INVALID;
        bool bSelfIsRoomOwner = false;
        bool bMatchLock = false;

        GameObject[] LeftList = new GameObject[MaxPlayerNum];
        GameObject[] RightList = new GameObject[MaxPlayerNum];

        ComTalk m_miniTalk = null;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Pk3v3/Pk3v3WaitingRoom";
        }

        protected override void _OnOpenFrame()
        {
            Pk3v3CrossDataManager.GetInstance()._UnBindNetMsg();

            if (userData != null)
            {
                RoomData = userData as PkWaitingRoomData;
            }
            // 乱斗模式只有两个栏位
            RoomInfo roomInfo = Pk3v3DataManager.GetInstance().GetRoomInfo();
            if(roomInfo != null)
            {
                if(roomInfo.roomSimpleInfo.roomType == (byte)RoomType.ROOM_TYPE_MELEE)
                {
                    LeftList = new GameObject[MaxMeleePlayerNum];
                    RightList = new GameObject[MaxMeleePlayerNum];
                }
                else
                {
                    LeftList = new GameObject[MaxPlayerNum];
                    RightList = new GameObject[MaxPlayerNum];
                }
            }

            InitInterface();
            BindUIEvent();
        }

        protected override void _OnCloseFrame()
        {
            Pk3v3CrossDataManager.GetInstance()._BindNetMsg();

            ClearData();
            UnBindUIEvent();

            ClientSystemManager.GetInstance().CloseFrame<Pk3v3PlayerMenuFrame>();

            if (ClientSystemManager.GetInstance().IsFrameOpen<ClientSystemTownFrame>())
            {
                ClientSystemTownFrame Townframe = ClientSystemManager.GetInstance().GetFrame(typeof(ClientSystemTownFrame)) as ClientSystemTownFrame;
                Townframe.SetForbidFadeIn(false);
            }
        }

        void ClearData()
        {
            RoomData.Clear();
            SelfGroup = RoomSlotGroup.ROOM_SLOT_GROUP_INVALID;
            bSelfIsRoomOwner = false;
            bMatchLock = false;

            for (int i = 0; i < LeftList.Length; i++)
            {
                LeftList[i] = null;
            }

            for (int i = 0; i < RightList.Length; i++)
            {
                RightList[i] = null;
            }

            if (m_miniTalk != null)
            {
                ComTalk.Recycle();
                m_miniTalk = null;
            }
        
            _UnInitVoiceTalk();
        }

        protected void BindUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.Pk3v3RoomSimpleInfoUpdate, OnPk3v3RoomSimpleInfoUpdate);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.Pk3v3RoomSlotInfoUpdate, OnPk3v3RoomSlotInfoUpdate);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.Pk3v3KickOut, OnPk3v3KickOut);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.Pk3v3BeginMatch, OnBeginMatch);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.Pk3v3BeginMatchRes, OnBeginMatchRes);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.Pk3v3CancelMatch, OnCancelMatch);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.Pk3v3CancelMatchRes, OnCancelMatchRes);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.Set3v3RoomName, OnSetName);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.Set3v3RoomPassword, onSetPassword);
        }

        protected void UnBindUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.Pk3v3RoomSimpleInfoUpdate, OnPk3v3RoomSimpleInfoUpdate);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.Pk3v3RoomSlotInfoUpdate, OnPk3v3RoomSlotInfoUpdate);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.Pk3v3KickOut, OnPk3v3KickOut);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.Pk3v3BeginMatch, OnBeginMatch);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.Pk3v3BeginMatchRes, OnBeginMatchRes);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.Pk3v3CancelMatch, OnCancelMatch);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.Pk3v3CancelMatchRes, OnCancelMatchRes);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.Set3v3RoomName, OnSetName);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.Set3v3RoomPassword, onSetPassword);
        }

        void OnPk3v3RoomSimpleInfoUpdate(UIEvent iEvent)
        {
            UpdatePlayerList();
            SetRoomPassword();
        }

        void OnPk3v3RoomSlotInfoUpdate(UIEvent iEvent)
        {
            UpdatePlayerList();
        }

        void OnPk3v3KickOut(UIEvent iEvent)
        {
            SwitchSceneToTown();
        }

        void OnBeginMatch(UIEvent iEvent)
        {
            RoomInfo roomInfo = Pk3v3DataManager.GetInstance().GetRoomInfo();

            if(roomInfo == null)
            {
                return;
            }

            PkSeekWaitingData data = new PkSeekWaitingData();
            data.roomtype = PkRoomType.Pk3v3;

            ClientSystemManager.GetInstance().OpenFrame<PkSeekWaiting>(FrameLayer.Middle, data);

            if (roomInfo.roomSimpleInfo.roomType == (byte)RoomType.ROOM_TYPE_THREE_MATCH)
            {
                ShowCancelText(roomInfo);
                ETCImageLoader.LoadSprite(ref mBtStartImage, StartBtnBluePath);
                mRight.CustomActive(roomInfo.roomSimpleInfo.roomStatus == (byte)RoomStatus.ROOM_STATUS_MATCH);
            }

            mBtBegin.gameObject.CustomActive(roomInfo.roomSimpleInfo.ownerId == PlayerBaseData.GetInstance().RoleID || (roomInfo.roomSimpleInfo.ownerId != PlayerBaseData.GetInstance().RoleID && roomInfo.roomSimpleInfo.roomStatus == (byte)RoomStatus.ROOM_STATUS_MATCH));
        }

        void OnBeginMatchRes(UIEvent iEvent)
        {
            bMatchLock = false;
        }

        void OnCancelMatch(UIEvent iEvent)
        {
            RoomInfo roomInfo = Pk3v3DataManager.GetInstance().GetRoomInfo();

            if (roomInfo == null)
            {
                return;
            }

            ClientSystemManager.GetInstance().CloseFrame<PkSeekWaiting>();

            if (roomInfo.roomSimpleInfo.roomType == (byte)RoomType.ROOM_TYPE_THREE_MATCH)
            {
                ShowBeginText(roomInfo);
                ETCImageLoader.LoadSprite(ref mBtStartImage, StartBtnRedPath);
                mRight.CustomActive(roomInfo.roomSimpleInfo.roomStatus == (byte)RoomStatus.ROOM_STATUS_MATCH);
            }

            mBtBegin.gameObject.CustomActive(roomInfo.roomSimpleInfo.ownerId == PlayerBaseData.GetInstance().RoleID || (roomInfo.roomSimpleInfo.ownerId != PlayerBaseData.GetInstance().RoleID && roomInfo.roomSimpleInfo.roomStatus == (byte)RoomStatus.ROOM_STATUS_MATCH));
        }

        void OnCancelMatchRes(UIEvent iEvent)
        {
            bMatchLock = false;
        }

        void OnSetName(UIEvent iEvent)
        {
            string roomName = (string)iEvent.Param1;
            if (roomName != null && roomName != "")
            {
                mRoomName.text = roomName;
            }     
        }

        void onSetPassword(UIEvent iEvent)
        {
            SetRoomPassword();
        }

        void OnClickLeftIcon(int iIndex)
        {
            if (Pk3v3DataManager.GetInstance().CheckRoomIsMatching())
            {
                return;
            }

            TryOpenPlayerMenuFrame(iIndex, RoomSlotGroup.ROOM_SLOT_GROUP_RED);
        }

        void OnClickLeftChangePos(int iIndex)
        {
            if (Pk3v3DataManager.GetInstance().CheckRoomIsMatching())
            {
                return;
            }

            TryChangePos(iIndex, RoomSlotGroup.ROOM_SLOT_GROUP_RED);
        }

        void OnClickLeftClosePos(int iIndex)
        {
            if (Pk3v3DataManager.GetInstance().CheckRoomIsMatching())
            {
                return;
            }

            TryClosePos(iIndex, RoomSlotGroup.ROOM_SLOT_GROUP_RED);
        }

        void OnClickRightIcon(int iIndex)
        {
            if (Pk3v3DataManager.GetInstance().CheckRoomIsMatching())
            {
                return;
            }

            TryOpenPlayerMenuFrame(iIndex, RoomSlotGroup.ROOM_SLOT_GROUP_BLUE);
        }

        void OnClickRightChangePos(int iIndex)
        {
            if (Pk3v3DataManager.GetInstance().CheckRoomIsMatching())
            {
                return;
            }

            TryChangePos(iIndex, RoomSlotGroup.ROOM_SLOT_GROUP_BLUE);
        }

        void OnClickRightClosePos(int iIndex)
        {
            if (Pk3v3DataManager.GetInstance().CheckRoomIsMatching())
            {
                return;
            }

            TryClosePos(iIndex, RoomSlotGroup.ROOM_SLOT_GROUP_BLUE);
        }

        void InitInterface()
        {
            InitPlayerList();
            _InitTalk();
            _InitVoiceTalk();
        }

        void InitPlayerList()
        {
            RoomInfo roomInfo = Pk3v3DataManager.GetInstance().GetRoomInfo();

            if (roomInfo == null)
            {
                Logger.LogError("roomInfo is null in [InitPlayerList]");
                return;
            }

            ShowBeginText(roomInfo);

            CheckSelfInfo();

            mRight.CustomActive(roomInfo.roomSimpleInfo.roomType == (byte)RoomType.ROOM_TYPE_THREE_FREE || roomInfo.roomSimpleInfo.roomType == (byte)RoomType.ROOM_TYPE_MELEE);
            mBtSetting.gameObject.CustomActive(roomInfo.roomSimpleInfo.ownerId == PlayerBaseData.GetInstance().RoleID);
            mBtBegin.gameObject.CustomActive(roomInfo.roomSimpleInfo.ownerId == PlayerBaseData.GetInstance().RoleID);
            //mRankBg.CustomActive(roomInfo.roomSimpleInfo.roomType == (byte)RoomType.ROOM_TYPE_THREE_MATCH);
            //mAmuseBg.gameObject.CustomActive(roomInfo.roomSimpleInfo.roomType == (byte)RoomType.ROOM_TYPE_THREE_FREE || roomInfo.roomSimpleInfo.roomType == (byte)RoomType.ROOM_TYPE_MELEE);
            mMatchTypeImg.gameObject.CustomActive(roomInfo.roomSimpleInfo.roomType == (byte)RoomType.ROOM_TYPE_MELEE);
            mAmuseTypeImg.gameObject.CustomActive(roomInfo.roomSimpleInfo.roomType == (byte)RoomType.ROOM_TYPE_THREE_FREE);

            InitLeftPlayerList();
            InitRightPlayerList();

            mRoomName.text = roomInfo.roomSimpleInfo.name;
            mRoomId.text = roomInfo.roomSimpleInfo.id.ToString();

            SetRoomPassword();

            
        }

        void _InitTalk()
        {
            m_miniTalk = ComTalk.Create(mTalkRoot);
        }

        void InitLeftPlayerList()
        {
            RoomInfo roomInfo = Pk3v3DataManager.GetInstance().GetRoomInfo();

            if (roomInfo == null)
            {
                Logger.LogError("roomInfo is null in [InitLeftPlayerList]");
                return;
            }

            for (int i = 0; i < LeftList.Length; i++)
            {
                GameObject PlayerInfoEle = AssetLoader.instance.LoadResAsGameObject(PlayerInfoElePath);
                if (PlayerInfoEle == null)
                {
                    Logger.LogError("can't create left obj in pk3v3WaitinRoom");
                    return;
                }

                PlayerInfoEle.transform.SetParent(mLeft.transform, false);
                LeftList[i] = PlayerInfoEle;

                ComCommonBind combind = PlayerInfoEle.GetComponent<ComCommonBind>();
                if (combind == null)
                {
                    Logger.LogError("can't find ComCommonBind in PlayerInfoElePath");
                    return;
                }

                UpdatePlayerBaseInfo(combind, roomInfo, i, RoomSlotGroup.ROOM_SLOT_GROUP_RED);
            }
        }

        void InitRightPlayerList()
        {
            RoomInfo roomInfo = Pk3v3DataManager.GetInstance().GetRoomInfo();

            if (roomInfo == null)
            {
                Logger.LogError("roomInfo is null in [InitRightPlayerList]");
                return;
            }

            if(roomInfo.roomSimpleInfo.roomType == (byte)RoomType.ROOM_TYPE_THREE_MATCH)
            {
                return;
            }

            for (int i = 0; i < RightList.Length; i++)
            {
                GameObject PlayerInfoEle = AssetLoader.instance.LoadResAsGameObject(PlayerInfoElePath);
                if (PlayerInfoEle == null)
                {
                    Logger.LogError("can't create right obj in pk3v3WaitinRoom");
                    return;
                }

                PlayerInfoEle.transform.SetParent(mRight.transform, false);
                RightList[i] = PlayerInfoEle;

                ComCommonBind combind = PlayerInfoEle.GetComponent<ComCommonBind>();
                if (combind == null)
                {
                    Logger.LogError("can't find ComCommonBind in PlayerInfoElePath");
                    return;
                }

                UpdatePlayerBaseInfo(combind, roomInfo, i, RoomSlotGroup.ROOM_SLOT_GROUP_BLUE);
            }
        }

        void UpdatePlayerList()
        {
            RoomInfo roomInfo = Pk3v3DataManager.GetInstance().GetRoomInfo();

            if(roomInfo == null)
            {
                Logger.LogError("roomInfo is null in [UpdatePlayerList]");
                return;
            }

            CheckSelfInfo();

            for (int i = 0; i < LeftList.Length; i++)
            {
                ComCommonBind combind = LeftList[i].GetComponent<ComCommonBind>();
                if (combind == null)
                {
                    Logger.LogError("can't find ComCommonBind in PlayerInfoElePath");
                    return;
                }

                UpdatePlayerBaseInfo(combind, roomInfo, i, RoomSlotGroup.ROOM_SLOT_GROUP_RED);
            }

            if (roomInfo.roomSimpleInfo.roomType == (byte)RoomType.ROOM_TYPE_THREE_FREE || roomInfo.roomSimpleInfo.roomType == (byte)RoomType.ROOM_TYPE_MELEE)
            {
                for (int i = 0; i < RightList.Length; i++)
                {
                    ComCommonBind combind = RightList[i].GetComponent<ComCommonBind>();
                    if (combind == null)
                    {
                        Logger.LogError("can't find ComCommonBind in PlayerInfoElePath");
                        return;
                    }

                    UpdatePlayerBaseInfo(combind, roomInfo, i, RoomSlotGroup.ROOM_SLOT_GROUP_BLUE);
                }
            }

            mBtSetting.gameObject.CustomActive(roomInfo.roomSimpleInfo.ownerId == PlayerBaseData.GetInstance().RoleID);
            mBtBegin.gameObject.CustomActive(roomInfo.roomSimpleInfo.ownerId == PlayerBaseData.GetInstance().RoleID || (roomInfo.roomSimpleInfo.ownerId != PlayerBaseData.GetInstance().RoleID && roomInfo.roomSimpleInfo.roomStatus == (byte)RoomStatus.ROOM_STATUS_MATCH));
            mRoomName.text = roomInfo.roomSimpleInfo.name;
        }

        void UpdatePlayerBaseInfo(ComCommonBind combind, RoomInfo roomInfo, int iIndex, RoomSlotGroup group)
        {         
            Image Icon = combind.GetCom<Image>("Icon");
            Text name = combind.GetCom<Text>("Name");
            GameObject nameRoot = combind.GetGameObject("nameRoot");
            Text Lv = combind.GetCom<Text>("Lv"); 
            Image LvBack = combind.GetCom<Image>("LvBack");
            Image RoomOwner = combind.GetCom<Image>("RoomOwner");
            Image LockImg = combind.GetCom<Image>("Lock");
            GameObject WhatObj = combind.GetGameObject("What");
            UIGray IconGray = combind.GetCom<UIGray>("IconGray");
            UIGray btIconGray = combind.GetCom<UIGray>("btIconGray");

            Button btIcon = combind.GetCom<Button>("btIcon");
            Button btChangePos = combind.GetCom<Button>("ChangePos");
            Button btClosePos = combind.GetCom<Button>("ClosePos");

            ReplaceHeadPortraitFrame mReplaceHeadPortraitFrame = combind.GetCom<ReplaceHeadPortraitFrame>("ReplaceHeadPortraitFrame");

            bool bIsPlayer = false;
            bool bIsRoomOwner = false;
            bool bLock = false;
            bool bOffLine = false;
            bool bIsSameGroupWithSelf = false;

            for (int j = 0; j < roomInfo.roomSlotInfos.Length; j++)
            {
                if (roomInfo.roomSlotInfos[j].group != (byte)group)
                {
                    continue;
                }

                if (roomInfo.roomSlotInfos[j].index != iIndex)
                {
                    continue;
                }

                if (roomInfo.roomSlotInfos[j].playerId == roomInfo.roomSimpleInfo.ownerId)
                {
                    bIsRoomOwner = true;
                }

                if (roomInfo.roomSlotInfos[j].status == (byte)RoomSlotStatus.ROOM_SLOT_STATUS_CLOSE)
                {
                    bLock = true;
                }
                else if(roomInfo.roomSlotInfos[j].status == (byte)RoomSlotStatus.ROOM_SLOT_STATUS_OFFLINE)
                {
                    bOffLine = true;
                }

                if(roomInfo.roomSlotInfos[j].group == (byte)SelfGroup)
                {
                    bIsSameGroupWithSelf = true;
                }

                if (roomInfo.roomSlotInfos[j].playerId != 0)
                {
                    bIsPlayer = true;

                    LoadSprite(ref Icon, roomInfo.roomSlotInfos[j].playerOccu);

                    Lv.text = roomInfo.roomSlotInfos[j].playerLevel.ToString();
                    name.text = roomInfo.roomSlotInfos[j].playerName;
                }

                if (mReplaceHeadPortraitFrame != null)
                {
                    if (roomInfo.roomSlotInfos[j].playerLabelInfo.headFrame != 0)
                    {
                        mReplaceHeadPortraitFrame.ReplacePhotoFrame((int)roomInfo.roomSlotInfos[j].playerLabelInfo.headFrame);
                    }
                    else
                    {
                        mReplaceHeadPortraitFrame.ReplacePhotoFrame(HeadPortraitFrameDataManager.iDefaultHeadPortraitID);
                    }
                }

                break;
            }

            Icon.gameObject.CustomActive(bIsPlayer);
            name.gameObject.CustomActive(bIsPlayer);
            nameRoot.CustomActive(bIsPlayer);
            LvBack.gameObject.CustomActive(bIsPlayer);
            RoomOwner.gameObject.CustomActive(bIsRoomOwner);
            LockImg.gameObject.CustomActive(bLock);
            IconGray.SetEnable(bOffLine);
            btIconGray.SetEnable(bOffLine);
            WhatObj.gameObject.CustomActive(group == RoomSlotGroup.ROOM_SLOT_GROUP_BLUE && roomInfo.roomSimpleInfo.roomType == (byte)RoomType.ROOM_TYPE_THREE_MATCH);
            //btChangePos.gameObject.CustomActive(!bIsPlayer && !bLock && !bSelfIsRoomOwner && !bIsSameGroupWithSelf);
            btChangePos.gameObject.CustomActive(!bIsPlayer && !bLock && !bSelfIsRoomOwner);
            //btClosePos.gameObject.CustomActive(!bIsPlayer && bSelfIsRoomOwner && bIsSameGroupWithSelf);
            btClosePos.gameObject.CustomActive(false);

            btIcon.onClick.RemoveAllListeners();
            int iIdex = iIndex;

            btChangePos.onClick.RemoveAllListeners();
            int iIdx = iIndex;

            btClosePos.onClick.RemoveAllListeners();
            int idx = iIndex;

            if (group == RoomSlotGroup.ROOM_SLOT_GROUP_RED)
            {
                btIcon.onClick.AddListener(() => { OnClickLeftIcon(iIdex); });
                btChangePos.onClick.AddListener(() => { OnClickLeftChangePos(iIdx); });
                btClosePos.onClick.AddListener(() => { OnClickLeftClosePos(idx); });
            }
            else if(group == RoomSlotGroup.ROOM_SLOT_GROUP_BLUE)
            {
                btIcon.onClick.AddListener(() => { OnClickRightIcon(iIdex); });
                btChangePos.onClick.AddListener(() => { OnClickRightChangePos(iIdx); });
                btClosePos.onClick.AddListener(() => { OnClickRightClosePos(idx); });
            }          
        }

        void CheckSelfInfo()
        {
            RoomInfo roomInfo = Pk3v3DataManager.GetInstance().GetRoomInfo();

            if(roomInfo == null)
            {
                return;
            }

            bSelfIsRoomOwner = roomInfo.roomSimpleInfo.ownerId == PlayerBaseData.GetInstance().RoleID;

            for (int i = 0; i < roomInfo.roomSlotInfos.Length; i++)
            {
                if (roomInfo.roomSlotInfos[i].playerId == PlayerBaseData.GetInstance().RoleID)
                {
                    SelfGroup = (RoomSlotGroup)roomInfo.roomSlotInfos[i].group;
                    break;
                }
            }
        }

        void TryOpenPlayerMenuFrame(int iIdex, RoomSlotGroup slotGroup)
        {
            RoomInfo roomInfo = Pk3v3DataManager.GetInstance().GetRoomInfo();
            RoomSlotInfo slotinfo = null;

            bool bIsSelf = false;
            for (int i = 0; i < roomInfo.roomSlotInfos.Length; i++)
            {
                if (roomInfo.roomSlotInfos[i].group != (byte)slotGroup)
                {
                    continue;
                }

                if (roomInfo.roomSlotInfos[i].index != iIdex)
                {
                    continue;
                }

                if (roomInfo.roomSlotInfos[i].playerId == PlayerBaseData.GetInstance().RoleID)
                {
                    bIsSelf = true;
                    break;
                }

                slotinfo = roomInfo.roomSlotInfos[i];

                break;
            }

            if (slotinfo == null)
            {
                return;
            }

            bool bIngnore = false;
            if (slotinfo.status == (byte)RoomSlotStatus.ROOM_SLOT_STATUS_CLOSE)
            {
                //if (bSelfIsRoomOwner && slotinfo.group != (byte)SelfGroup)
                if (bSelfIsRoomOwner)
                {
                    bIngnore = true;
                }
            }
            else
            {
                bIngnore = true;
            }

            if (!bIngnore)
            {
                return;
            }

            if (bIsSelf)
            {
                return;
            }

            if (PlayerBaseData.GetInstance().RoleID != roomInfo.roomSimpleInfo.ownerId && slotinfo.playerId == 0)
            {
                return;
            }

            if (ClientSystemManager.GetInstance().IsFrameOpen<Pk3v3PlayerMenuFrame>())
            {
                ClientSystemManager.GetInstance().CloseFrame<Pk3v3PlayerMenuFrame>();
            }
            else
            {
                if(bSelfIsRoomOwner && slotinfo.status == (byte)RoomSlotStatus.ROOM_SLOT_STATUS_CLOSE)
                {
                    Pk3v3DataManager.GetInstance().SendClosePosReq(slotinfo.group, slotinfo.index);
                }
                else
                {
                    Pk3v3PlayerMenuFrame frm = ClientSystemManager.GetInstance().OpenFrame<Pk3v3PlayerMenuFrame>(FrameLayer.Middle, slotinfo) as Pk3v3PlayerMenuFrame;

                    if (slotGroup == RoomSlotGroup.ROOM_SLOT_GROUP_RED)
                    {
                        frm.SetPosition(LeftList[iIdex].GetComponent<RectTransform>().position);
                    }
                    else
                    {
                        frm.SetPosition(RightList[iIdex].GetComponent<RectTransform>().position);
                    }
                }        
            }         
        }

        void TryChangePos(int iIdex, RoomSlotGroup slotGroup)
        {
            RoomInfo roomInfo = Pk3v3DataManager.GetInstance().GetRoomInfo();

            if (roomInfo == null)
            {
                return;
            }

            for (int i = 0; i < roomInfo.roomSlotInfos.Length; i++)
            {
                if (roomInfo.roomSlotInfos[i].group != (byte)slotGroup)
                {
                    continue;
                }

                if (roomInfo.roomSlotInfos[i].index != iIdex)
                {
                    continue;
                }

                if (roomInfo.roomSlotInfos[i].playerId != 0)
                {
                    continue;
                }

                if (roomInfo.roomSlotInfos[i].group == (byte)SelfGroup)
                {
                    //continue;
                }

                Pk3v3DataManager.GetInstance().SendPk3v3ChangePosReq(roomInfo.roomSimpleInfo.id, roomInfo.roomSlotInfos[i]);

                break;
            }
        }

        void TryClosePos(int iIdex, RoomSlotGroup slotGroup)
        {
            RoomInfo roomInfo = Pk3v3DataManager.GetInstance().GetRoomInfo();

            if (roomInfo == null)
            {
                return;
            }

            if (roomInfo.roomSimpleInfo.ownerId != PlayerBaseData.GetInstance().RoleID)
            {
                return;
            }

            for (int i = 0; i < roomInfo.roomSlotInfos.Length; i++)
            {
                if (roomInfo.roomSlotInfos[i].group != (byte)slotGroup)
                {
                    continue;
                }

                if (roomInfo.roomSlotInfos[i].index != iIdex)
                {
                    continue;
                }

                if(roomInfo.roomSlotInfos[i].playerId != 0)
                {
                    continue;
                }

                if(roomInfo.roomSlotInfos[i].group != (byte)SelfGroup)
                {
                    continue;
                }

                Pk3v3DataManager.GetInstance().SendClosePosReq(roomInfo.roomSlotInfos[i].group, roomInfo.roomSlotInfos[i].index);

                break;
            }
        }

        void LoadSprite(ref Image Icon, int Occu)
        {
            string path = "";

            JobTable jobData = TableManager.GetInstance().GetTableItem<JobTable>(Occu);
            if (jobData != null)
            {
                ResTable resData = TableManager.GetInstance().GetTableItem<ResTable>(jobData.Mode);
                if (resData != null)
                {
                    path = resData.IconPath;
                }
            }

            if (path != "")
            {
                ETCImageLoader.LoadSprite(ref Icon, path);
            }
        }

        void SwitchSceneToTown()
        {
            ClientSystemTown systemTown = ClientSystemManager.GetInstance().GetCurrentSystem() as ClientSystemTown;
            if (systemTown == null)
            {
                Logger.LogError("Current System is not Town from Pk3v3WaitingRoom");
                return;
            }

            GameFrameWork.instance.StartCoroutine(systemTown._NetSyncChangeScene(new SceneParams
            {
                currSceneID = RoomData.CurrentSceneID,
                currDoorID = 0,
                targetSceneID = RoomData.TargetTownSceneID,
                targetDoorID = 0,
            }, true));

            frameMgr.CloseFrame(this);
        }

        /// <summary>
        /// 用于刷新房间密码的显示
        /// </summary>
        private void SetRoomPassword()
        {
            RoomInfo roomInfo = Pk3v3DataManager.GetInstance().GetRoomInfo();

            if (roomInfo == null)
            {
                Logger.LogError("roomInfo is null in [InitPlayerList]");
                return;
            }

            if (roomInfo.roomSimpleInfo.isPassword > 0 && roomInfo.roomSimpleInfo.ownerId == PlayerBaseData.GetInstance().RoleID)
            {
                SetActiveRoomPassword(true);
            }
            else
            {
                SetActiveRoomPassword(false);
            }
        }

        private void SetActiveRoomPassword(bool isOpen)
        {
            if(isOpen)
            {
                mRoomPasswordGO.CustomActive(true);
                mRoomPasswordText.text = Pk3v3DataManager.GetInstance().PassWord;
            }
            else
            {
                mRoomPasswordGO.CustomActive(false);
            }
        }

        [MessageHandle(WorldQuitRoomRes.MsgID)]
        void OnQuitRoomRes(MsgDATA msg)
        {
            WorldQuitRoomRes res = new WorldQuitRoomRes();
            res.decode(msg.bytes);

            if (res.result != (uint)ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int)res.result);
            }

            Pk3v3DataManager.GetInstance().ClearRoomInfo();

            SwitchSceneToTown();
        }

        void SendQuitRoomReq()
        {
            WorldQuitRoomReq req = new WorldQuitRoomReq();

            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }

        void SendBeginGameReq()
        {
            WorldRoomBattleStartReq req = new WorldRoomBattleStartReq();

            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }

        void SendCancelGameReq()
        {
            WorldRoomBattleCancelReq req = new WorldRoomBattleCancelReq();

            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }

        void ShowBeginText(RoomInfo roomInfo)
        {
            if (roomInfo.roomSimpleInfo.roomType == (byte)RoomType.ROOM_TYPE_THREE_FREE || roomInfo.roomSimpleInfo.roomType == (byte)RoomType.ROOM_TYPE_MELEE)
            {
                mBtMatchText.text = "开始决斗";
            }
            else
            {
                mBtMatchText.text = "开始匹配";
            }
        }

        void ShowCancelText(RoomInfo roomInfo)
        {
            if (roomInfo.roomSimpleInfo.roomType == (byte)RoomType.ROOM_TYPE_THREE_FREE || roomInfo.roomSimpleInfo.roomType == (byte)RoomType.ROOM_TYPE_MELEE)
            {
                mBtMatchText.text = "取消决斗";
            }
            else
            {
                mBtMatchText.text = "取消匹配";
            }
        }

        #region VoiceSDK

        private ComVoiceTalk mComVoiceTalk = null;
 
        void _InitVoiceTalk()
        {
            ComVoiceTalk.LocalCacheData localData = new ComVoiceTalk.LocalCacheData();            
            ComVoiceTalk.ComVoiceTalkType talkType = ComVoiceTalk.ComVoiceTalkType.Pk3v3Room;
            if(mComVoiceTalk == null)
            {
                mComVoiceTalk = ComVoiceTalk.Create(mTalkVoiceRoot, localData, talkType);
            }
            if(mComVoiceTalk != null)
            {
                string pk3v3VoiceChannelId = TryGetVoiceSDKChannalId();
                mComVoiceTalk.AddGameSceneId(pk3v3VoiceChannelId);
            }
        }

        void _UnInitVoiceTalk()
        {
            if(mComVoiceTalk != null)
            {
                ComVoiceTalk.ForceDestroy();
            }
        }

        string TryGetVoiceSDKChannalId()
        {
            string voiceChannelId = "";
            RoomInfo roomInfo = Pk3v3DataManager.GetInstance().GetRoomInfo();
            if (roomInfo == null)
            {
                return voiceChannelId;
            }
            uint serverId = ClientApplication.adminServer.id;
            int bCount = roomInfo.roomSimpleInfo.id.ToString().Length;
            if (bCount <= 0)
                return voiceChannelId;
            Logger.LogProcessFormat("TryGetVoiceSDKChannalId 3v3 room id is = {0}, Count = {1} , serverId = {2}", roomInfo.roomSimpleInfo.id, bCount,serverId);
            voiceChannelId = (serverId * Math.Pow(10,bCount) + roomInfo.roomSimpleInfo.id).ToString();
            Logger.LogProcessFormat("TryGetVoiceSDKChannalId 3v3 room id is = {0}", voiceChannelId);
            return voiceChannelId;
        }
        #endregion

        #region ExtraUIBind
        private Button mBtClose = null;
        private Button mBtMenu = null;
        private Button mBtInviteFriends = null;
        private Button mBtBegin = null;
        private GameObject mTalkRoot = null;
        private Button mBtSetting = null;
        private GameObject mLeft = null;
        private GameObject mRight = null;
        private Text mRoomName = null;
        private Text mRoomId = null;
        private Image mMatchTypeImg = null;
        private Image mAmuseTypeImg = null;
        private Text mBtMatchText = null;
        private GameObject mRankBg = null;
        private Image mBtStartImage = null;
        private GameObject mAmuseBg = null;
        private Text mRoomPasswordText = null;
        private GameObject mRoomPasswordGO = null;
        private GameObject mTalkVoiceRoot = null;

        protected override void _bindExUI()
        {
            mBtClose = mBind.GetCom<Button>("btClose");
            mBtClose.onClick.AddListener(_onBtCloseButtonClick);
            mBtMenu = mBind.GetCom<Button>("btMenu");
            mBtMenu.onClick.AddListener(_onBtMenuButtonClick);
            mBtMenu.CustomActive(false);
            mBtInviteFriends = mBind.GetCom<Button>("btInviteFriends");
            mBtInviteFriends.onClick.AddListener(_onBtInviteFriendsButtonClick);
            mBtBegin = mBind.GetCom<Button>("btBegin");
            mBtBegin.onClick.AddListener(_onBtBeginButtonClick);
            mTalkRoot = mBind.GetGameObject("TalkRoot");
            mBtSetting = mBind.GetCom<Button>("btSetting");
            mBtSetting.onClick.AddListener(_onBtSettingButtonClick);
            mLeft = mBind.GetGameObject("left");
            mRight = mBind.GetGameObject("right");
            mRoomName = mBind.GetCom<Text>("RoomName");
            mRoomId = mBind.GetCom<Text>("RoomId");
            mMatchTypeImg = mBind.GetCom<Image>("MatchTypeImg");
            mAmuseTypeImg = mBind.GetCom<Image>("AmuseTypeImg");
            mBtMatchText = mBind.GetCom<Text>("btMatchText");
            mRankBg = mBind.GetGameObject("RankBg");
            mBtStartImage = mBind.GetCom<Image>("btStartImage");
            mAmuseBg = mBind.GetGameObject("AmuseBg");
            mRoomPasswordText = mBind.GetCom<Text>("RoomPasswordText");
            mRoomPasswordGO = mBind.GetGameObject("RoomPasswordGO");
            mTalkVoiceRoot = mBind.GetGameObject("TalkVoiceRoot");
        }

        protected override void _unbindExUI()
        {
            mBtClose.onClick.RemoveListener(_onBtCloseButtonClick);
            mBtClose = null;
            mBtMenu.onClick.RemoveListener(_onBtMenuButtonClick);
            mBtMenu = null;
            mBtInviteFriends.onClick.RemoveListener(_onBtInviteFriendsButtonClick);
            mBtInviteFriends = null;
            mBtBegin.onClick.RemoveListener(_onBtBeginButtonClick);
            mBtBegin = null;
            mTalkRoot = null;
            mBtSetting.onClick.RemoveListener(_onBtSettingButtonClick);
            mBtSetting = null;
            mLeft = null;
            mRight = null;
            mRoomName = null;
            mRoomId = null;
            mMatchTypeImg = null;
            mAmuseTypeImg = null;
            mBtMatchText = null;
            mRankBg = null;
            mBtStartImage = null;
            mAmuseBg = null;
            mRoomPasswordText = null;
            mRoomPasswordGO = null;
            mTalkVoiceRoot = null;
        }
        #endregion

        #region Callback
        private void _onBtCloseButtonClick()
        {
            if (Pk3v3DataManager.GetInstance().CheckRoomIsMatching())
            {
                return;
            }

            SendQuitRoomReq();
        }

        private void _onBtMenuButtonClick()
        {
            if (Pk3v3DataManager.GetInstance().CheckRoomIsMatching())
            {
                return;
            }

            ClientSystemManager.GetInstance().OpenFrame<PkMenuFrame>(FrameLayer.Middle);
        }

        private void _onBtInviteFriendsButtonClick()
        {
            if (Pk3v3DataManager.GetInstance().CheckRoomIsMatching())
            {
                return;
            }

            if (ClientSystemManager.GetInstance().IsFrameOpen<TeamInvitePlayerListFrame>())
            {
                return;
            }

            ClientSystemManager.GetInstance().OpenFrame<TeamInvitePlayerListFrame>(FrameLayer.Middle, InviteType.Pk3v3Invite);
        }

        private void _onBtBeginButtonClick()
        {
            RoomInfo roomInfo = Pk3v3DataManager.GetInstance().GetRoomInfo();
            if(roomInfo == null || roomInfo.roomSlotInfos == null)
            {
                return;
            }

            if (roomInfo.roomSimpleInfo.roomStatus == (byte)RoomStatus.ROOM_STATUS_OPEN)
            {
                if (PlayerBaseData.GetInstance().RoleID != roomInfo.roomSimpleInfo.ownerId)
                {
                    SystemNotifyManager.SysNotifyFloatingEffect("你不是房主,无法开始游戏");
                    return;
                }

                for (int i = 0; i < roomInfo.roomSlotInfos.Length; i++)
                {
                    if (roomInfo.roomSlotInfos[i].playerId > 0 && roomInfo.roomSlotInfos[i].status == (byte)RoomSlotStatus.ROOM_SLOT_STATUS_OFFLINE)
                    {
                        SystemNotifyManager.SystemNotify(9216);
                        return;
                    }
                }
                if (roomInfo.roomSimpleInfo.roomType == (byte)RoomType.ROOM_TYPE_MELEE)
                {
                    int iBlueCount = 0;
                    int iRedCount = 0;
                    for (int i = 0; i < roomInfo.roomSlotInfos.Length; i++)
                    {
                        if (roomInfo.roomSlotInfos[i].group == (byte)RoomSlotGroup.ROOM_SLOT_GROUP_BLUE && roomInfo.roomSlotInfos[i].playerId > 0)
                        {
                            iBlueCount++;
            }

                        if (roomInfo.roomSlotInfos[i].group == (byte)RoomSlotGroup.ROOM_SLOT_GROUP_RED && roomInfo.roomSlotInfos[i].playerId > 0)
                        {
                            iRedCount++;
                        }
                    }
                    if (!(iBlueCount >= 2 && iRedCount >= 2))
                    {
                        SystemNotifyManager.SystemNotify(9934);
                        return;
                    }
                }
            }
            if(bMatchLock)
            {
                return;
            }

            bMatchLock = true;

            if (roomInfo.roomSimpleInfo.roomStatus == (byte)RoomStatus.ROOM_STATUS_OPEN)
            {
                SendBeginGameReq();
            }
            else if (roomInfo.roomSimpleInfo.roomStatus == (byte)RoomStatus.ROOM_STATUS_MATCH)
            {
                SendCancelGameReq();
            }
            else
            {
                Logger.LogErrorFormat("Pk3v3 begin state is error, roomstate = {0}", roomInfo.roomSimpleInfo.roomStatus);
            }
        }

        private void _onBtSettingButtonClick()
        {
            if(Pk3v3DataManager.GetInstance().CheckRoomIsMatching())
            {
                return;
            }

            if (ClientSystemManager.GetInstance().IsFrameOpen<Pk3v3RoomSettingFrame>())
            {
                return;
            }

            ClientSystemManager.GetInstance().OpenFrame<Pk3v3RoomSettingFrame>(FrameLayer.Middle);
        }

        private void _onPvp3v3PlayerBtnButtonClick()
        {
            /* put your code in here */
            if (VoiceSDK.SDKVoiceManager.GetInstance().IsPlayVoiceEnabled == false)
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("voice_sdk_play_not_enabled"));
                return;
            }
            VoiceSDK.SDKVoiceManager.GetInstance().ControlRealVociePlayer();
        }
        private void _onPvp3v3MicRoomBtnButtonClick()
        {
            /* put your code in here */
            if (VoiceSDK.SDKVoiceManager.GetInstance().IsRecordVoiceEnabled == false)
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("voice_sdk_record_not_enabled"));
                return;
            }
            VoiceSDK.SDKVoiceManager.GetInstance().ControlRealVoiceMic();
        }
        #endregion
    }
}
