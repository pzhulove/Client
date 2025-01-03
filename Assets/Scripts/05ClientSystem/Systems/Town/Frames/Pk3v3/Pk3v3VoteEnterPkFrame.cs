using UnityEngine;
using UnityEngine.UI;
using ProtoTable;
using Protocol;
using Network;
using System.Collections.Generic;

namespace GameClient
{
    class Pk3v3VoteEnterPkFrame : ClientFrame
    {
        string VotePlayerElePath = "UIFlatten/Prefabs/Pk3v3/Pk3v3VotePlayerEle";

        float VoteLastTime = 0.0f;
        float addTime = 0.0f;
        float fSpeed = 0.00225f;

        Dictionary<ulong, GameObject> LeftDict = new Dictionary<ulong, GameObject>();
        Dictionary<ulong, GameObject> RightDict = new Dictionary<ulong, GameObject>();

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Pk3v3/Pk3v3VoteEnterPkFrame";
        }

        protected override void _OnOpenFrame()
        {
            var valuedata = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType2.SVT_ROOM_REJECT_BATTLE_READY_TIME);
            if(valuedata == null)
            {
                Logger.LogError("can not find SVT_ROOM_REJECT_ENTER_TIME in SystemValueTable");
                return;
            }

            VoteLastTime = valuedata.Value;

            mTips.text = TR.Value("Pk3v3EnterBattleTips");
            mMatchtips.text = TR.Value("Pk3v3EnterBattleTips");

            InitInterface();
            BindUIEvent();     

            if(frameMgr.IsFrameOpen<Pk3v3CrossWaitingRoom>())
            {
                txtBattle.text = "队长发起决斗";
                txtMatch.text = "队长发起匹配";
            }          
        }

        protected override void _OnCloseFrame()
        {
            ClearData();
            UnBindUIEvent();
        }

        void ClearData()
        {
            VoteLastTime = 0.0f;
            addTime = 0.0f;

            LeftDict.Clear();
            RightDict.Clear();
        }

        void BindUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.Pk3v3RoomSlotInfoUpdate, OnPk3v3RoomSlotInfoUpdate);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.Pk3v3RefuseBeginMatch, OnPk3v3RefuseBeginMatch);
        }

        void UnBindUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.Pk3v3RoomSlotInfoUpdate, OnPk3v3RoomSlotInfoUpdate);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.Pk3v3RefuseBeginMatch, OnPk3v3RefuseBeginMatch);
        }

        void OnPk3v3RoomSlotInfoUpdate(UIEvent iEvent)
        {
            UpdatePlayerInfo();
        }

        void OnPk3v3RefuseBeginMatch(UIEvent iEvent)
        {
            frameMgr.CloseFrame(this);
        }

        void Swap<T>(ref T x, ref T y)
        {
            T temp = x;
            x = y;
            y = temp;
        }

        void ReSortRoomSlotInfo(ref RoomInfo roomInfo)
        {
            if(roomInfo == null)
            {
                return;
            }

            int iLeaderIndex = -1;
            for (int i = 0; i < roomInfo.roomSlotInfos.Length; i++)
            {
                RoomSlotInfo slotInfo = roomInfo.roomSlotInfos[i];
                if (slotInfo.playerId == roomInfo.roomSimpleInfo.ownerId)
                {
                    iLeaderIndex = i;
                    break;
                }
            }

            if (iLeaderIndex != -1 && iLeaderIndex != 0)
            {
                Swap(ref roomInfo.roomSlotInfos[0], ref roomInfo.roomSlotInfos[iLeaderIndex]);
            }            
      
            if(roomInfo.roomSlotInfos.Length > 2)
            {
                if (roomInfo.roomSlotInfos[1].playerId == 0 && roomInfo.roomSlotInfos[2].playerId != 0)
                {
                    Swap(ref roomInfo.roomSlotInfos[1], ref roomInfo.roomSlotInfos[2]);
                }
            }            

            return;
        }

        void InitInterface()
        {
            RoomInfo roomInfo = Pk3v3DataManager.GetInstance().GetRoomInfo();

            if (frameMgr.IsFrameOpen<Pk3v3CrossWaitingRoom>())
            {
                roomInfo = Pk3v3CrossDataManager.GetInstance().GetRoomInfo();
                ReSortRoomSlotInfo(ref roomInfo);
            }

            if (roomInfo == null)
            {
                return; 
            }
            if (roomInfo.roomSimpleInfo.roomType == (byte)RoomType.ROOM_TYPE_THREE_FREE || roomInfo.roomSimpleInfo.roomType == (byte)RoomType.ROOM_TYPE_MELEE)
            {
                mGoEntertainment.CustomActive(true);
                mGoMatching.CustomActive(false);
                mBottomroot.CustomActive(PlayerBaseData.GetInstance().RoleID != roomInfo.roomSimpleInfo.ownerId);
            }
            else if (roomInfo.roomSimpleInfo.roomType == (byte)RoomType.ROOM_TYPE_THREE_MATCH || roomInfo.roomSimpleInfo.roomType == (byte)RoomType.ROOM_TYPE_THREE_SCORE_WAR)
            {
                mGoEntertainment.CustomActive(false);
                mGoMatching.CustomActive(true);
                mMatchbottomroot.CustomActive(PlayerBaseData.GetInstance().RoleID != roomInfo.roomSimpleInfo.ownerId);
            }
            //mPkbg.CustomActive(roomInfo.roomSimpleInfo.roomType == (byte)RoomType.ROOM_TYPE_THREE_FREE);
            //mBottomroot.CustomActive(PlayerBaseData.GetInstance().RoleID != roomInfo.roomSimpleInfo.ownerId);
            //mRightroot.CustomActive(roomInfo.roomSimpleInfo.roomType == (byte)RoomType.ROOM_TYPE_THREE_FREE);

            for(int i = 0; i < roomInfo.roomSlotInfos.Length; i++)
            {
                RoomSlotInfo slotInfo = roomInfo.roomSlotInfos[i];

                GameObject VotePlayerEle = AssetLoader.instance.LoadResAsGameObject(VotePlayerElePath);
                if (VotePlayerEle == null)
                {
                    Logger.LogError("can't create obj in Pk3v3VoteEnterPkFrame");
                    return;
                }

                if (slotInfo.group == (byte)RoomSlotGroup.ROOM_SLOT_GROUP_RED)
                {
                    if (roomInfo.roomSimpleInfo.roomType == (byte)RoomType.ROOM_TYPE_THREE_FREE || roomInfo.roomSimpleInfo.roomType == (byte)RoomType.ROOM_TYPE_MELEE)
                    {
                        VotePlayerEle.transform.SetParent(mLeftroot.transform, false);
                    }
                    else if (roomInfo.roomSimpleInfo.roomType == (byte)RoomType.ROOM_TYPE_THREE_MATCH || roomInfo.roomSimpleInfo.roomType == (byte)RoomType.ROOM_TYPE_THREE_SCORE_WAR)
                    {
                        VotePlayerEle.transform.SetParent(mContentroot.transform, false);
                    }

                    if (slotInfo.playerId > 0)
                    {                        
                        if (LeftDict.ContainsKey(slotInfo.playerId))
                        {
                            LeftDict[slotInfo.playerId] = VotePlayerEle;
                        }
                        else
                        {
                            LeftDict.Add(slotInfo.playerId, VotePlayerEle);
                        }                        
                    }
                    else
                    {            
                        if (LeftDict.ContainsKey((ulong)i))
                        {
                            LeftDict[(ulong)i] = VotePlayerEle;
                        }
                        else
                        {
                            LeftDict.Add((ulong)i, VotePlayerEle);
                        }                        
                    }
                }
                else if(slotInfo.group == (byte)RoomSlotGroup.ROOM_SLOT_GROUP_BLUE)
                {
                    VotePlayerEle.transform.SetParent(mRightroot.transform, false);
                    if (slotInfo.playerId > 0)
                    {
                        if (RightDict.ContainsKey(slotInfo.playerId))
                        {
                            RightDict[slotInfo.playerId] = VotePlayerEle;
                        }
                        else
                        {
                            RightDict.Add(slotInfo.playerId, VotePlayerEle);
                        }
                    }
                    else
                    {
                        if (RightDict.ContainsKey((ulong)i))
                        {
                            RightDict[(ulong)i] = VotePlayerEle;
                        }
                        else
                        {
                            RightDict.Add((ulong)i, VotePlayerEle);
                        }
                    }
                }
                else
                {
                    continue;
                }

                ComCommonBind combind = VotePlayerEle.GetComponent<ComCommonBind>();
                if (combind == null)
                {
                    Logger.LogError("can't find ComCommonBind in VotePlayerElePath");
                    return;
                }

                UpdatePlayerBaseInfo(combind, slotInfo);
            }
        }

        void UpdatePlayerInfo()
        {
            RoomInfo roomInfo = Pk3v3DataManager.GetInstance().GetRoomInfo();
            if(frameMgr.IsFrameOpen<Pk3v3CrossWaitingRoom>())
            {
                roomInfo = Pk3v3CrossDataManager.GetInstance().GetRoomInfo();
                ReSortRoomSlotInfo(ref roomInfo);
            }

            if (roomInfo == null)
            {
                return;
            }

            for (int i = 0; i < roomInfo.roomSlotInfos.Length; i++)
            {
                RoomSlotInfo slotInfo = roomInfo.roomSlotInfos[i];

                if (slotInfo.playerId == 0)
                {
                    continue;
                }

                bool bFind = false;
                GameObject obj = null;

                if(roomInfo.roomSlotInfos[i].group == (byte)RoomSlotGroup.ROOM_SLOT_GROUP_RED)
                {
                    if (LeftDict.TryGetValue(slotInfo.playerId, out obj))
                    {
                        bFind = true;
                    }
                    else
                    {
                        Logger.LogErrorFormat("LeftDict don`t find init PlayerId = {0} when UpdatePlayerInfo", slotInfo.playerId);
                    }
                }
                else if(roomInfo.roomSlotInfos[i].group == (byte)RoomSlotGroup.ROOM_SLOT_GROUP_BLUE)
                {
                    if (RightDict.TryGetValue(slotInfo.playerId, out obj))
                    {
                        bFind = true;
                    }
                    else
                    {
                        Logger.LogErrorFormat("RightDict don`t find init PlayerId = {0} when UpdatePlayerInfo", slotInfo.playerId);
                    }
                }

                if(bFind)
                {
                    ComCommonBind combind = obj.GetComponent<ComCommonBind>();
                    if (combind == null)
                    {
                        Logger.LogError("can't find ComCommonBind in UpdatePlayerInfo");
                        return;
                    }

                    UpdatePlayerBaseInfo(combind, slotInfo);
                }
            }
        }

        void UpdatePlayerBaseInfo(ComCommonBind combind, RoomSlotInfo slotInfo)
        {
            Image Icon = combind.GetCom<Image>("Icon");
            Text name = combind.GetCom<Text>("Name");
            Image sel = combind.GetCom<Image>("Sel");
            UIGray IconGray = combind.GetCom<UIGray>("IconGray");

            if (slotInfo.playerId > 0)
            {
                string Iconpath = "";
                JobTable jobData = TableManager.GetInstance().GetTableItem<JobTable>(slotInfo.playerOccu);
                if (jobData != null)
                {
                    ResTable resData = TableManager.GetInstance().GetTableItem<ResTable>(jobData.Mode);
                    if (resData != null)
                    {
                        Iconpath = resData.IconPath;
                    }
                }

                if (Iconpath != "")
                {
                    ETCImageLoader.LoadSprite(ref Icon, Iconpath);
                }

                name.text = slotInfo.playerName;
                IconGray.SetEnable(!(slotInfo.readyStatus == (byte)RoomSlotReadyStatus.ROOM_SLOT_READY_STATUS_ACCEPT));
                sel.gameObject.CustomActive(slotInfo.readyStatus == (byte)RoomSlotReadyStatus.ROOM_SLOT_READY_STATUS_ACCEPT);

                if (slotInfo.playerId == PlayerBaseData.GetInstance().RoleID)
                {
                    if (slotInfo.readyStatus == (byte)RoomSlotReadyStatus.ROOM_SLOT_READY_STATUS_ACCEPT)
                    {

                        mBottomroot.CustomActive(false);
                        mTips.gameObject.CustomActive(true);

                        mMatchbottomroot.CustomActive(false);
                        mMatchtips.gameObject.CustomActive(true);
                    }
                }
            }
            else
            {
                IconGray.SetEnable(true);
            }
           
        }

        void SendBattleReadyReq(bool agree)
        {
            WorldRoomBattleReadyReq msg = new WorldRoomBattleReadyReq();

            if(agree)
            {
                msg.slotStatus = (byte)RoomSlotReadyStatus.ROOM_SLOT_READY_STATUS_ACCEPT;
            }
            else
            {
                msg.slotStatus = (byte)RoomSlotReadyStatus.ROOM_SLOT_READY_STATUS_REFUSE;
            }    

            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, msg);
        }

        public override bool IsNeedUpdate()
        {
            return true;
        }

        protected override void _OnUpdate(float timeElapsed)
        {
            mEnterSlider.value += fSpeed;
            mMatchSlider.value += fSpeed;

            addTime += timeElapsed;

            if(addTime > 1.0f)
            {
                VoteLastTime -= 1.0f;
                addTime = 0.0f;

                int iInt = (int)(VoteLastTime);

                mRegectText.text = string.Format("拒绝({0}s)", iInt);

                mMatchRegectText.text = string.Format("拒绝({0}s)", iInt);
            }

            if (VoteLastTime < 0.2f)
            {
                frameMgr.CloseFrame(this);
            }
        }

        #region ExtraUIBind
        private GameObject mLeftroot = null;
        private GameObject mRightroot = null;
        private GameObject mBottomroot = null;
        private Button mBtReject = null;
        private Button mBtAgree = null;
        private GameObject mPkbg = null;
        private Text mRegectText = null;
        private UIGray mRejectGray = null;
        private UIGray mAgreeGray = null;
        private Text mMatchtips = null;
        private GameObject mMatchbottomroot = null;
        private GameObject mGoMatching = null;
        private GameObject mGoEntertainment = null;
        private GameObject mContentroot = null;
        private UIGray mMatchAgreeGray = null;
        private UIGray mMatchRejectGray = null;
        private Button mBtmatchAgree = null;
        private Button mBtmatchReject = null;
        private Text mMatchRegectText = null;
        private Slider mMatchSlider = null;
        private Slider mEnterSlider = null;
        private Text mTips = null;
        Text txtBattle = null;
        Text txtMatch = null;

        protected override void _bindExUI()
        {
            mLeftroot = mBind.GetGameObject("leftroot");
            mRightroot = mBind.GetGameObject("rightroot");
            mBottomroot = mBind.GetGameObject("bottomroot");
            mBtReject = mBind.GetCom<Button>("btReject");
            mBtReject.onClick.AddListener(_onBtRejectButtonClick);
            mBtAgree = mBind.GetCom<Button>("btAgree");
            mBtAgree.onClick.AddListener(_onBtAgreeButtonClick);
            mPkbg = mBind.GetGameObject("pkbg");
            mRegectText = mBind.GetCom<Text>("RegectText");
            mRejectGray = mBind.GetCom<UIGray>("RejectGray");
            mAgreeGray = mBind.GetCom<UIGray>("AgreeGray");
            mMatchtips = mBind.GetCom<Text>("matchtips");
            mMatchbottomroot = mBind.GetGameObject("matchbottomroot");
            mGoMatching = mBind.GetGameObject("goMatching");
            mGoEntertainment = mBind.GetGameObject("goEntertainment");
            mContentroot = mBind.GetGameObject("contentroot");
            mMatchAgreeGray = mBind.GetCom<UIGray>("matchAgreeGray");
            mMatchRejectGray = mBind.GetCom<UIGray>("matchRejectGray");
            mBtmatchAgree = mBind.GetCom<Button>("btmatchAgree");
            mBtmatchAgree.onClick.AddListener(_onBtmatchAgreeButtonClick);
            mBtmatchReject = mBind.GetCom<Button>("btmatchReject");
            mBtmatchReject.onClick.AddListener(_onBtmatchRejectButtonClick);
            mMatchRegectText = mBind.GetCom<Text>("matchRegectText");
            mMatchSlider = mBind.GetCom<Slider>("matchSlider");
            mEnterSlider = mBind.GetCom<Slider>("enterSlider");
            mTips = mBind.GetCom<Text>("tips");
            txtBattle = mBind.GetCom<Text>("txtBattle");
            txtMatch = mBind.GetCom<Text>("txtMatch");
        }

        protected override void _unbindExUI()
        {
            mLeftroot = null;
            mRightroot = null;
            mBottomroot = null;
            mBtReject.onClick.RemoveListener(_onBtRejectButtonClick);
            mBtReject = null;
            mBtAgree.onClick.RemoveListener(_onBtAgreeButtonClick);
            mBtAgree = null;
            mPkbg = null;
            mRegectText = null;
            mRejectGray = null;
            mAgreeGray = null;
            mMatchtips = null;
            mMatchbottomroot = null;
            mGoMatching = null;
            mGoEntertainment = null;
            mContentroot = null;
            mMatchAgreeGray = null;
            mMatchRejectGray = null;
            mBtmatchAgree.onClick.RemoveListener(_onBtmatchAgreeButtonClick);
            mBtmatchAgree = null;
            mBtmatchReject.onClick.RemoveListener(_onBtmatchRejectButtonClick);
            mBtmatchReject = null;
            mMatchRegectText = null;
            mTips = null;
            mMatchSlider = null;
            mEnterSlider = null;
            txtBattle = null;
            txtMatch = null;
        }
        #endregion

        #region Callback
        private void _onBtRejectButtonClick()
        {
            OnRejectButtonClick();
        }
        private void _onBtAgreeButtonClick()
        {
            SendBattleReadyReq(true);

            mRejectGray.enabled = true;
            mBtAgree.interactable = false;

            mAgreeGray.enabled = true;
            mBtAgree.interactable = false;
        }
        private void _onBtmatchAgreeButtonClick()
        {
            SendBattleReadyReq(true);

            mMatchRejectGray.enabled = true;
            mBtmatchAgree.interactable = false;

            mMatchAgreeGray.enabled = true;
            mBtmatchAgree.interactable = false;

        }
        private void _onBtmatchRejectButtonClick()
        {
            OnRejectButtonClick();
        }
        #endregion

        void OnRejectButtonClick()
        {
            RoomInfo roomInfo = Pk3v3DataManager.GetInstance().GetRoomInfo();

            if (frameMgr.IsFrameOpen<Pk3v3CrossWaitingRoom>())
            {
                roomInfo = Pk3v3CrossDataManager.GetInstance().GetRoomInfo();
                ReSortRoomSlotInfo(ref roomInfo);
            }

            if (roomInfo == null)
            {
                return;
            }

            if (PlayerBaseData.GetInstance().RoleID == roomInfo.roomSimpleInfo.ownerId)
            {
                SystemNotifyManager.SysNotifyFloatingEffect("房主默认同意进入战斗");
                return;
            }

            SendBattleReadyReq(false);

            frameMgr.CloseFrame(this);
        }
        
    }
}
