using Network;
using Protocol;
using UnityEngine;
using ProtoTable;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;
using DG.Tweening;
using Scripts.UI;

namespace GameClient
{
    public class Pk3v3CrossWaitingRoom : ClientFrame
    {
        string PlayerInfoElePath = "UIFlatten/Prefabs/Pk3v3/Pk3v3PlayerInfo";
        string StartBtnRedPath = "UI/Image/Packed/p_UI_Common00.png:UI_Tongyong_Anniu_Quren_Xuanzhong_06";
        string StartBtnBluePath= "UI/Image/Packed/p_UI_Common00.png:UI_Tongyong_Anniu_Lanse_01";
        const int MaxPlayerNum = 3;

        PkWaitingRoomData RoomData = new PkWaitingRoomData();

        RoomSlotGroup SelfGroup = RoomSlotGroup.ROOM_SLOT_GROUP_INVALID;
        bool bSelfIsRoomOwner = false;
        bool bMatchLock = false;
        bool bIsMatching = false;

        GameObject[] LeftList = new GameObject[MaxPlayerNum];
        GameObject[] RightList = new GameObject[MaxPlayerNum];

        ComTalk m_miniTalk = null;

        float fTimeAcc = 0.0f;

        int iFirstBattleAwardID = 0;
        int iFiveBattleAwardID = 0;
        int iFirstWinBattleID = 0;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Pk3v3Cross/Pk3v3CrossWaitingRoom";
        }

        void SetAwardItem(GameObject goItem, ItemData itemData)
        {
            if(goItem == null)
            {
                return;
            }

            ComItem comItem = goItem.GetComponent<ComItem>();
            if(comItem == null)
            {
                return;
            }
           
            comItem.Setup(itemData, (var1, var2) =>
            {
                ItemTipManager.GetInstance().CloseAll();
                ItemTipManager.GetInstance().ShowTip(var2);
            });

            return;
        }

        protected override void _OnOpenFrame()
        {
            fTimeAcc = 0.0f;
            if(userData != null)
            {
                RoomData = userData as PkWaitingRoomData;
            }

            {
                Dictionary<int, object> dicts = TableManager.instance.GetTable<ScoreWarRewardTable>();
                if (dicts != null)
                {
                    var iter = dicts.GetEnumerator();
                    while (iter.MoveNext())
                    {
                        ScoreWarRewardTable adt = iter.Current.Value as ScoreWarRewardTable;
                        if (adt == null)
                        {
                            continue;
                        }
                        
                        if(adt.BattleCount == 1 && iFirstBattleAwardID == 0)
                        {
                            iFirstBattleAwardID = adt.RewardId;
                        }
                        if(adt.BattleCount == 5 && iFiveBattleAwardID == 0)
                        {
                            iFiveBattleAwardID = adt.RewardId;
                        }
                        if(adt.WinCount == 1 && iFirstWinBattleID == 0)
                        {
                            iFirstWinBattleID = adt.RewardId;
                        }

                        if(iFirstBattleAwardID > 0 && iFirstWinBattleID > 0 && iFiveBattleAwardID > 0)
                        {
                            break;
                        }
                    }
                }               
            }

            InitInterface();
            BindUIEvent();

            OnPk3v3CrossRewardInfoUpdate(null);

            if(goMatchOK != null)
            {
                goMatchOK.CustomActive(false);
            }

            UpdateBeginButton();

            Pk3v3CrossDataManager.My3v3PkInfo pkInfo = Pk3v3CrossDataManager.GetInstance().PkInfo;
            if (pkInfo != null)
            {
                if (pkInfo.nCurPkCount >= 5)
                {
                    mBeginGray.SetEnable(true);
                    mBtBegin.interactable = false;
                }
            }

            //StartCoroutine(OpenTeamMainFrame());
        }

//         IEnumerator OpenTeamMainFrame()
//         {
//             yield return Yielders.EndOfFrame;
// 
//             ClientSystemManager.GetInstance().OpenFrame<Pk3v3CrossTeamMainFrame>();
//             ClientSystemManager.GetInstance().GetFrame(typeof(Pk3v3CrossTeamMainFrame)).GetFrame().CustomActive(true);
//             UIEventSystem.GetInstance().SendUIEvent(EUIEventID.Pk3v3CrossUpdateMyTeamFrame);
// 
//         }

        protected override void _OnCloseFrame()
        {
            iFirstBattleAwardID = 0;
            iFiveBattleAwardID = 0;
            iFirstWinBattleID = 0;

            fTimeAcc = 0.0f;
            ClearData();
            UnBindUIEvent();

            ClientSystemManager.GetInstance().CloseFrame<Pk3v3PlayerMenuFrame>();

            if (ClientSystemManager.GetInstance().IsFrameOpen<ClientSystemTownFrame>())
            {
                ClientSystemTownFrame Townframe = ClientSystemManager.GetInstance().GetFrame(typeof(ClientSystemTownFrame)) as ClientSystemTownFrame;
                Townframe.SetForbidFadeIn(false);
            }

            ClientSystemManager.GetInstance().CloseFrame<Pk3v3CrossTeamMainFrame>();
            ClientSystemManager.GetInstance().CloseFrame<Pk3v3CrossTeamMainMenuFrame>();           
        }

        public override bool IsNeedUpdate()
        {
            return true;
        }

        void GetLeftTime(uint nEndTime,uint nNowTime,ref int nLeftDay,ref int nLeftHour,ref int nLeftMin,ref int nLeftSec)
        {
            nLeftDay = 0;
            nLeftHour = 0;
            nLeftMin = 0;
            nLeftSec = 0;

            if(nEndTime <= nNowTime)
            {
                return;
            }

            uint LeftTime = nEndTime - nNowTime;

            uint Day = LeftTime / (24 * 60 * 60);
            LeftTime -= Day * 24 * 60 * 60;

            uint Hour = LeftTime / (60 * 60);
            LeftTime -= Hour * 60 * 60;

            uint Minute = LeftTime / 60;
            LeftTime -= Minute * 60;

            uint Second = LeftTime;

            nLeftDay = (int)Day;
            nLeftHour = (int)Hour;
            nLeftMin = (int)Minute;
            nLeftSec = (int)Second;

            return;
        }

        void UpdateWarStateTimeLeft()
        {

        }
        
        protected override void _OnUpdate(float timeElapsed)
        {
            ScoreWarStatus state = Pk3v3CrossDataManager.GetInstance().Get3v3CrossWarStatus();
            if (state == ScoreWarStatus.SWS_PREPARE || state == ScoreWarStatus.SWS_BATTLE/* || state == ScoreWarStatus.SWS_WAIT_END*/)
            {
                if (goScoreWarStateTimeInfo != null)
                {
                    goScoreWarStateTimeInfo.CustomActive(true);
                }

                int nLeftDay = 0;
                int nLeftHour = 0;
                int nLeftMin = 0;
                int nLeftSec = 0;

                GetLeftTime(Pk3v3CrossDataManager.GetInstance().Get3v3CrossWarStatusEndTime(), TimeManager.GetInstance().GetServerTime(), ref nLeftDay, ref nLeftHour, ref nLeftMin, ref nLeftSec);
                
                if(state == ScoreWarStatus.SWS_PREPARE)
                {
                    txtTimeInfo.text = string.Format("备战时间  {0}  :  {1}  :  {2}", string.Format("{0}  {1}", nLeftHour/10,nLeftHour%10), string.Format("{0}  {1}", nLeftMin / 10, nLeftMin % 10 ), string.Format("{0}  {1}", nLeftSec / 10, nLeftSec % 10 ));
                }
                else if(state == ScoreWarStatus.SWS_BATTLE)
                {
                    txtTimeInfo.text = string.Format("活动时间  {0}  :  {1}  :  {2}", string.Format("{0}  {1}", nLeftHour / 10, nLeftHour % 10), string.Format("{0}  {1}", nLeftMin / 10, nLeftMin % 10), string.Format("{0}  {1}", nLeftSec / 10, nLeftSec % 10));
                }

            }
            else
            { 
                if (goScoreWarStateTimeInfo != null)
                {
                    goScoreWarStateTimeInfo.CustomActive(false);
                }
            }

            //InvokeMethod.InvokeInterval
        }

        bool IsGetFirstBattleAward()
        {         
            Pk3v3CrossDataManager.My3v3PkInfo kInfo = Pk3v3CrossDataManager.GetInstance().PkInfo;
            for(int i = 0;i < kInfo.arrAwardIDs.Count;i++)
            {
                if(iFirstBattleAwardID == kInfo.arrAwardIDs[i])
                {
                    return true;
                }
            }

            return false;
        }

        bool IsGetFiveBattleAward()
        {       
            Pk3v3CrossDataManager.My3v3PkInfo kInfo = Pk3v3CrossDataManager.GetInstance().PkInfo;
            for (int i = 0; i < kInfo.arrAwardIDs.Count; i++)
            {
                if (iFiveBattleAwardID == kInfo.arrAwardIDs[i])
                {
                    return true;
                }
            }

            return false;
        }

        bool IsGetFirstWinAward()
        {
            Pk3v3CrossDataManager.My3v3PkInfo kInfo = Pk3v3CrossDataManager.GetInstance().PkInfo;
            for (int i = 0; i < kInfo.arrAwardIDs.Count; i++)
            {
                if (iFirstWinBattleID == kInfo.arrAwardIDs[i])
                {
                    return true;
                }
            }

            return false;
        }
        
        void UpdateAwardInfo(int iAwardID,Func<bool> isGet,Func<bool> canGet,Button btnChest, Image goImageGet,GameObject goAni)
        {
            goAni.CustomActive(false);

//             isGet = () => { return false; };
//             canGet = () => { return true; };

            if (isGet()) // 已经领取
            {
                btnChest.onClick.RemoveAllListeners();
                btnChest.interactable = false;
                goImageGet.CustomActive(true);
                btnChest.GetComponent<Image>().raycastTarget = false;
                UIGray gray = btnChest.gameObject.GetComponent<UIGray>();
                if (gray != null)
                {
                    gray.SetEnable(true);
                }

                btnChest.gameObject.GetComponent<DOTweenAnimation>().DOPause();
            }
            else if (canGet()) // 可以领取,此时点击宝箱就直接领取奖励
            {
                btnChest.onClick.RemoveAllListeners();
                btnChest.onClick.AddListener(() =>
                {
                    SceneScoreWarRewardReq req = new SceneScoreWarRewardReq();
                    req.rewardId = (byte)iAwardID;

                    NetManager netMgr = NetManager.Instance();
                    netMgr.SendCommand(ServerType.GATE_SERVER, req);
                });
                btnChest.interactable = true;
                goImageGet.CustomActive(false);
                btnChest.GetComponent<Image>().raycastTarget = true;
                UIGray gray = btnChest.gameObject.GetComponent<UIGray>();
                if (gray != null)
                {
                    gray.SetEnable(false);
                }

                goAni.CustomActive(true);          
                btnChest.gameObject.GetComponent<DOTweenAnimation>().DORestart();               
            }
            else // 不能领取，此时点击宝箱是查看奖励道具
            {
                btnChest.onClick.RemoveAllListeners();
                btnChest.onClick.AddListener(() =>
                {
                    AwardRankItem data = new AwardRankItem();

                    ScoreWarRewardTable tableItem = TableManager.GetInstance().GetTableItem<ProtoTable.ScoreWarRewardTable>(iAwardID);
                    if (tableItem != null)
                    {
                        for (int i = 0; i < tableItem.ItemReward.Count; i++)
                        {
                            string strReward = tableItem.ItemRewardArray(i);
                            string[] reward = strReward.Split('_');
                            if (reward.Length >= 2)
                            {
                                int id = int.Parse(reward[0]);
                                int iCount = int.Parse(reward[1]);
                                ItemData itemData = ItemDataManager.CreateItemDataFromTable(id);
                                itemData.Count = iCount;
                                if (itemData != null)
                                {
                                    //data.arrItems.Add(itemData);

                                    ItemTipManager.GetInstance().CloseAll();
                                    ItemTipManager.GetInstance().ShowTip(itemData);
                                }
                            }
                        }
                    }

                    //frameMgr.OpenFrame<AwardShowFrame>(FrameLayer.Middle, data);
                });

                btnChest.interactable = true;
                goImageGet.CustomActive(false);
                btnChest.GetComponent<Image>().raycastTarget = true;
                UIGray gray = btnChest.gameObject.GetComponent<UIGray>();
                if (gray != null)
                {
                    gray.SetEnable(false);
                }

                btnChest.gameObject.GetComponent<DOTweenAnimation>().DOPause();
            }
        }

        void UpdateFirstBattleAwardChest(UIEvent uiEvent)
        {
            if (IsGetFirstBattleAward())
            {
                gofirstBattleAward.CustomActive(false);
            }
            else
            {
                gofirstBattleAward.CustomActive(true);
            }

            UpdateAwardInfo(iFirstBattleAwardID, IsGetFirstBattleAward, () => 
            {
                Pk3v3CrossDataManager.My3v3PkInfo kInfo = Pk3v3CrossDataManager.GetInstance().PkInfo;
                return kInfo.nCurPkCount >= 1;
            }, 
            btnFirstBattleChest, 
            goFirstBattleGet,
            mBind.GetGameObject("goFirstBattleAni"));

            return;
        }

        void UpdateFiveBattleAwardChest(UIEvent uiEvent)
        { 
            if (!IsGetFirstBattleAward())
            {
                gofiveBattleAward.CustomActive(false);
            }
            else
            {
                gofiveBattleAward.CustomActive(true);

                UpdateAwardInfo(iFiveBattleAwardID, IsGetFiveBattleAward, () =>
                {
                    Pk3v3CrossDataManager.My3v3PkInfo kInfo = Pk3v3CrossDataManager.GetInstance().PkInfo;
                    return kInfo.nCurPkCount == 5;
                },           
                btnFiveBattleChest,           
                goFiveBattleGet,
                mBind.GetGameObject("goFiveBattleAni"));

                return;
            }
        }

        void UpdateFirstWinAwardChest(UIEvent uiEvent)
        {
            UpdateAwardInfo(iFirstWinBattleID, IsGetFirstWinAward, () =>
            {
                Pk3v3CrossDataManager.My3v3PkInfo kInfo = Pk3v3CrossDataManager.GetInstance().PkInfo;
                return kInfo.nWinCount >= 1;
            },           
            btnFirstWinChest,           
            goFirstWinGet,
            mBind.GetGameObject("goFirstWinAni"));
            return;           
        }

        void ClearData()
        {
            RoomData.Clear();
            SelfGroup = RoomSlotGroup.ROOM_SLOT_GROUP_INVALID;
            bSelfIsRoomOwner = false;
            bMatchLock = false;
            bIsMatching = false;

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
            

            bHasTeam = false;
            _UnInitVoiceTalk();
        }

        protected void BindUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.Pk3v3RoomInfoUpdate, OnPk3v3RoomInfoUpdate);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.Pk3v3RoomSimpleInfoUpdate, OnPk3v3RoomSimpleInfoUpdate);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.Pk3v3RoomSlotInfoUpdate, OnPk3v3RoomSlotInfoUpdate);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.Pk3v3KickOut, OnPk3v3KickOut);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.Pk3v3BeginMatch, OnBeginMatch);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.Pk3v3BeginMatchRes, OnBeginMatchRes);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.Pk3v3CancelMatch, OnCancelMatch);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.Pk3v3CancelMatchRes, OnCancelMatchRes);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.Set3v3RoomName, OnSetName);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.Set3v3RoomPassword, onSetPassword);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.Pk3v3CrossPkAwardInfoUpdate, OnPk3v3CrossRewardInfoUpdate);      
        }

        protected void UnBindUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.Pk3v3RoomInfoUpdate, OnPk3v3RoomInfoUpdate);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.Pk3v3RoomSimpleInfoUpdate, OnPk3v3RoomSimpleInfoUpdate);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.Pk3v3RoomSlotInfoUpdate, OnPk3v3RoomSlotInfoUpdate);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.Pk3v3KickOut, OnPk3v3KickOut);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.Pk3v3BeginMatch, OnBeginMatch);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.Pk3v3BeginMatchRes, OnBeginMatchRes);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.Pk3v3CancelMatch, OnCancelMatch);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.Pk3v3CancelMatchRes, OnCancelMatchRes);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.Set3v3RoomName, OnSetName);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.Set3v3RoomPassword, onSetPassword);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.Pk3v3CrossPkAwardInfoUpdate, OnPk3v3CrossRewardInfoUpdate);          
        }       

        void OnPk3v3CrossRewardInfoUpdate(UIEvent iEvent)
        {
            Pk3v3CrossDataManager.My3v3PkInfo pkInfo = Pk3v3CrossDataManager.GetInstance().PkInfo;
            if(pkInfo == null)
            {
                return;
            }

            txtPkCountInfo.text = string.Format("剩余比赛次数: {0}/{1}", Pk3v3CrossDataManager.MAX_PK_COUNT - pkInfo.nCurPkCount, 5);

            UpdateFirstBattleAwardChest(null);
            UpdateFiveBattleAwardChest(null);
            UpdateFirstWinAwardChest(null);
          

            if (pkInfo.nCurPkCount >= 5)
            {
                mBeginGray.SetEnable(true);
                mBtBegin.interactable = false;
            }

            return;
            
        }

        void OnPk3v3RoomInfoUpdate(UIEvent iEvent)
        {
            //UIEventSystem.GetInstance().SendUIEvent(EUIEventID.Pk3v3CrossRoomInfoUpdate);
            //frameMgr.CloseFrame(this);

            UpdateTeamMainMenuFrame();
        }

        void OnPk3v3RoomSimpleInfoUpdate(UIEvent iEvent)
        {
            //UIEventSystem.GetInstance().SendUIEvent(EUIEventID.Pk3v3CrossRoomInfoUpdate);
            //UpdatePlayerList();
            //SetRoomPassword();

            UpdateTeamMainMenuFrame();

            UpdateBeginButton();
        }

        void OnPk3v3RoomSlotInfoUpdate(UIEvent iEvent)
        {
            //UIEventSystem.GetInstance().SendUIEvent(EUIEventID.Pk3v3CrossRoomSlotInfoUpdate);

            //UpdatePlayerList();

            UpdateTeamMainMenuFrame();

            UpdateBeginButton();
        }

        void OnPk3v3KickOut(UIEvent iEvent)
        {
            Pk3v3CrossDataManager.GetInstance().ClearRoomInfo();

            UpdateTeamMainMenuFrame();

            UpdateBeginButton();

            frameMgr.CloseFrame<Pk3v3CrossMyTeamFrame>();
            //SwitchSceneToTown();
        }

        void OnBeginMatch(UIEvent iEvent)
        {
            bIsMatching = true;

            RoomInfo roomInfo = Pk3v3CrossDataManager.GetInstance().GetRoomInfo();

            UpdateTeamMainMenuFrame();

            UpdateBeginButton();

            if (roomInfo == null)
            {
                return;
            }

            PkSeekWaitingData data = new PkSeekWaitingData();
            data.roomtype = PkRoomType.Pk3v3Cross;

            ClientSystemManager.GetInstance().OpenFrame<PkSeekWaiting>(FrameLayer.Middle, data);

            if(roomInfo.roomSimpleInfo != null && roomInfo.roomSimpleInfo.id > 0)
            {
                if (roomInfo.roomSimpleInfo.roomType == (byte)RoomType.ROOM_TYPE_THREE_SCORE_WAR)
                {
                    ShowCancelText(roomInfo);
                    //ETCImageLoader.LoadSprite(ref mBtStartImage, StartBtnBluePath);
                    mRight.CustomActive(roomInfo.roomSimpleInfo.roomStatus == (byte)RoomStatus.ROOM_STATUS_MATCH);
                }

                bool bFlag = roomInfo.roomSimpleInfo.ownerId == PlayerBaseData.GetInstance().RoleID || (roomInfo.roomSimpleInfo.ownerId != PlayerBaseData.GetInstance().RoleID && roomInfo.roomSimpleInfo.roomStatus == (byte)RoomStatus.ROOM_STATUS_MATCH);
                //btnBeginEnable.SetEnable(!bFlag);
                mBtBegin.gameObject.CustomActive(true);
            }
            else
            {
                mBtMatchText.text = "取消匹配";
                //ETCImageLoader.LoadSprite(ref mBtStartImage, StartBtnBluePath);
            }      
        }

        void OnBeginMatchRes(UIEvent iEvent)
        {
            bMatchLock = false;
        }

        void OnCancelMatch(UIEvent iEvent)
        {
            bIsMatching = false;

            RoomInfo roomInfo = Pk3v3CrossDataManager.GetInstance().GetRoomInfo();

            UpdateTeamMainMenuFrame();

            UpdateBeginButton();

            if (roomInfo == null)
            {
                return;
            }

            ClientSystemManager.GetInstance().CloseFrame<PkSeekWaiting>();

            if (roomInfo.roomSimpleInfo != null && roomInfo.roomSimpleInfo.id > 0)
            {
                if (roomInfo.roomSimpleInfo.roomType == (byte)RoomType.ROOM_TYPE_THREE_SCORE_WAR)
                {
                    ShowBeginText(roomInfo);
                    //ETCImageLoader.LoadSprite(ref mBtStartImage, StartBtnRedPath);
                    mRight.CustomActive(roomInfo.roomSimpleInfo.roomStatus == (byte)RoomStatus.ROOM_STATUS_MATCH);
                }

                //mBtBegin.gameObject.CustomActive(roomInfo.roomSimpleInfo.ownerId == PlayerBaseData.GetInstance().RoleID || (roomInfo.roomSimpleInfo.ownerId != PlayerBaseData.GetInstance().RoleID && roomInfo.roomSimpleInfo.roomStatus == (byte)RoomStatus.ROOM_STATUS_MATCH));

                bool bFlag = roomInfo.roomSimpleInfo.ownerId == PlayerBaseData.GetInstance().RoleID || (roomInfo.roomSimpleInfo.ownerId != PlayerBaseData.GetInstance().RoleID && roomInfo.roomSimpleInfo.roomStatus == (byte)RoomStatus.ROOM_STATUS_MATCH);
                //btnBeginEnable.SetEnable(!bFlag);
                mBtBegin.gameObject.CustomActive(true);
            }
            else
            {
                mBtMatchText.text = "开始匹配";
                //ETCImageLoader.LoadSprite(ref mBtStartImage, StartBtnRedPath);
            }           
        }

        void OnCancelMatchRes(UIEvent iEvent)
        {
            //Logger.LogErrorFormat("OnCancelMatchRes,bMatchLock = {0}", bMatchLock);

            bMatchLock = false;
        }

        void OnSetName(UIEvent iEvent)
        {
            string roomName = (string)iEvent.Param1;
            if (roomName != null && roomName != "")
            {
                mRoomName.text = roomName;
            }

            mRoomName.text = "3v3积分争霸赛";
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

            mRight.CustomActive(roomInfo.roomSimpleInfo.roomType == (byte)RoomType.ROOM_TYPE_THREE_FREE);
            //mBtSetting.gameObject.CustomActive(roomInfo.roomSimpleInfo.ownerId == PlayerBaseData.GetInstance().RoleID);
            //mBtBegin.gameObject.CustomActive(roomInfo.roomSimpleInfo.ownerId == PlayerBaseData.GetInstance().RoleID);
            mRankBg.CustomActive(roomInfo.roomSimpleInfo.roomType == (byte)RoomType.ROOM_TYPE_THREE_MATCH);
            mAmuseBg.gameObject.CustomActive(roomInfo.roomSimpleInfo.roomType == (byte)RoomType.ROOM_TYPE_THREE_FREE);
            mMatchTypeImg.gameObject.CustomActive(roomInfo.roomSimpleInfo.roomType == (byte)RoomType.ROOM_TYPE_THREE_MATCH);
            mAmuseTypeImg.gameObject.CustomActive(roomInfo.roomSimpleInfo.roomType == (byte)RoomType.ROOM_TYPE_THREE_FREE);

            InitLeftPlayerList();
            InitRightPlayerList();

            mRoomName.text = roomInfo.roomSimpleInfo.name;
            mRoomId.text = roomInfo.roomSimpleInfo.id.ToString();

            SetRoomPassword();

            mRoomName.text = "3v3积分争霸赛";
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

        void UpdateTeamMainMenuFrame()
        {
            if (Pk3v3CrossDataManager.GetInstance().HasTeam())
            {
                ClientSystemManager.GetInstance().CloseFrame<Pk3v3CrossTeamMainMenuFrame>();
            }
            else
            {
                ClientSystemManager.GetInstance().OpenFrame<Pk3v3CrossTeamMainMenuFrame>(FrameLayer.Bottom);
                ClientSystemManager.GetInstance().GetFrame(typeof(Pk3v3CrossTeamMainMenuFrame)).GetFrame().CustomActive(true);
            }
        }

        void UpdateBeginButton()
        {
            RoomInfo roomInfo = Pk3v3CrossDataManager.GetInstance().GetRoomInfo();
            ScoreWarStatus state = Pk3v3CrossDataManager.GetInstance().Get3v3CrossWarStatus();

            mBeginGray.SetEnable(false);
            mBtBegin.interactable = true;
            if (state != ScoreWarStatus.SWS_BATTLE || (roomInfo != null && roomInfo.roomSimpleInfo.id > 0 && roomInfo.roomSimpleInfo.ownerId != PlayerBaseData.GetInstance().RoleID))
            {
                mBeginGray.SetEnable(true);
                mBtBegin.interactable = false;
            }

            mBtBegin.CustomActive(true);

            if(!bHasTeam && Pk3v3CrossDataManager.GetInstance().HasTeam())
            {
                bHasTeam = true;
                _InitVoiceTalk();
            }
            else if(bHasTeam && !Pk3v3CrossDataManager.GetInstance().HasTeam())
            {
                bHasTeam = false;
                _UnInitVoiceTalk();
            }

            Pk3v3CrossDataManager.My3v3PkInfo pkInfo = Pk3v3CrossDataManager.GetInstance().PkInfo;
            if (pkInfo != null)
            {
                if (pkInfo.nCurPkCount >= 5)
                {
                    mBeginGray.SetEnable(true);
                    mBtBegin.interactable = false;
                }
            }
        }


        void UpdatePlayerList()
        {
            RoomInfo roomInfo = Pk3v3DataManager.GetInstance().GetRoomInfo();
     
            if (roomInfo == null)
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

            if (roomInfo.roomSimpleInfo.roomType == (byte)RoomType.ROOM_TYPE_THREE_FREE)
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

            //mBtSetting.gameObject.CustomActive(roomInfo.roomSimpleInfo.ownerId == PlayerBaseData.GetInstance().RoleID);
            //mBtBegin.gameObject.CustomActive(roomInfo.roomSimpleInfo.ownerId == PlayerBaseData.GetInstance().RoleID || (roomInfo.roomSimpleInfo.ownerId != PlayerBaseData.GetInstance().RoleID && roomInfo.roomSimpleInfo.roomStatus == (byte)RoomStatus.ROOM_STATUS_MATCH));

            bool bFlag = roomInfo.roomSimpleInfo.ownerId == PlayerBaseData.GetInstance().RoleID || (roomInfo.roomSimpleInfo.ownerId != PlayerBaseData.GetInstance().RoleID && roomInfo.roomSimpleInfo.roomStatus == (byte)RoomStatus.ROOM_STATUS_MATCH);
            //btnBeginEnable.SetEnable(!bFlag);
            mBtBegin.gameObject.CustomActive(true);
            mRoomName.text = roomInfo.roomSimpleInfo.name;

            mRoomName.text = "3v3积分争霸赛";
        }

        void UpdatePlayerBaseInfo(ComCommonBind combind, RoomInfo roomInfo, int iIndex, RoomSlotGroup group)
        {         
            Image Icon = combind.GetCom<Image>("Icon");
            Text name = combind.GetCom<Text>("Name");
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

                break;
            }

            Icon.gameObject.CustomActive(bIsPlayer);
            name.gameObject.CustomActive(bIsPlayer);
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
            RoomInfo roomInfo = Pk3v3CrossDataManager.GetInstance().GetRoomInfo();

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
                mRoomPasswordText.text = Pk3v3CrossDataManager.GetInstance().PassWord;
            }
            else
            {
                mRoomPasswordGO.CustomActive(false);
            }
        }

        [MessageHandle(SceneSyncScoreWarInfo.MsgID)]
        void OnWorldSyncScoreWarInfo(MsgDATA msg)
        {

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

            Pk3v3CrossDataManager.GetInstance().ClearRoomInfo();

            UpdateTeamMainMenuFrame();

            UpdateBeginButton();

            frameMgr.CloseFrame<Pk3v3CrossMyTeamFrame>();

            //SwitchSceneToTown();
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

        void SendBeginOnePersonMatchGameReq()
        {
            WorldMatchStartReq req = new WorldMatchStartReq();
            req.type = (byte)PkType.Pk_3V3_War_Score;

            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }

        void SendCancelOnePersonMatchGameReq()
        {
            WorldMatchCancelReq req = new WorldMatchCancelReq();

            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }

        void ShowBeginText(RoomInfo roomInfo)
        {
            if (roomInfo.roomSimpleInfo.roomType == (byte)RoomType.ROOM_TYPE_THREE_FREE)
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
            if (roomInfo.roomSimpleInfo.roomType == (byte)RoomType.ROOM_TYPE_THREE_FREE)
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
        bool bHasTeam = false;
        void _InitVoiceTalk()
        {
            ComVoiceTalk.LocalCacheData localData = new ComVoiceTalk.LocalCacheData();            
            ComVoiceTalk.ComVoiceTalkType talkType = ComVoiceTalk.ComVoiceTalkType.Pk3v3Room;
            bool otherSwitch = Pk3v3CrossDataManager.GetInstance().HasTeam();
            if(otherSwitch)
            {
                if(mComVoiceTalk == null)
                {
                    mComVoiceTalk = ComVoiceTalk.Create(mTalkVoiceRoot, localData, talkType, otherSwitch);
                }
                if(mComVoiceTalk != null)
                {
                    string pk3v3VoiceChannelId = TryGetVoiceSDKChannalId();
                    mComVoiceTalk.AddGameSceneId(pk3v3VoiceChannelId);
                }
            }
        }

        void _UnInitVoiceTalk()
        {
            if (mComVoiceTalk != null)
            {
                ComVoiceTalk.ForceDestroy();
            }
        }

        string TryGetVoiceSDKChannalId()
        {
            string voiceChannelId = "";
            RoomInfo roomInfo = Pk3v3CrossDataManager.GetInstance().GetRoomInfo();
            if (roomInfo == null)
            {
                return voiceChannelId;
            }
            uint serverId = ClientApplication.adminServer.id;
            int bCount = roomInfo.roomSimpleInfo.id.ToString().Length;
            if (bCount <= 0)
                return voiceChannelId;
            //Logger.LogProcessFormat("TryGetVoiceSDKChannalId 3v3 room id is = {0}, Count = {1} , serverId = {2}", roomInfo.roomSimpleInfo.id, bCount,serverId);
            voiceChannelId = (serverId * Math.Pow(10,bCount) + roomInfo.roomSimpleInfo.id).ToString();
            //Logger.LogProcessFormat("TryGetVoiceSDKChannalId 3v3 room id is = {0}", voiceChannelId);
            return voiceChannelId;
        }
        #endregion

        #region ExtraUIBind
        private ComUIMultListScript test = null;
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
        private Text txtPkCountInfo = null;
        GameObject goBattleTimeState = null;      
        List<Text> arrNumbers = new List<Text>();

        Button btnFirstBattleChest = null;
        Image goFirstBattleGet = null;
        Button btnFiveBattleChest = null;
        Image goFiveBattleGet = null;
        Button btnFirstWinChest = null;
        Image goFirstWinGet = null;

        GameObject gofiveBattleAward = null;
        GameObject gofirstBattleAward = null;

        GameObject goMatchOK = null;

        GameObject goScoreWarStateTimeInfo = null;
        Text txtTimeInfo = null;

        UIGray mBeginGray = null;

        private GameObject mTalkVoiceRoot = null;


        [UIEventHandleAttribute("btnBag")]
        void _OnClickOpenBagFrame()
        {
            if (bIsMatching || frameMgr.IsFrameOpen<PkSeekWaiting>() || Pk3v3CrossDataManager.GetInstance().CheckRoomIsMatching())
            {
                SystemNotifyManager.SysNotifyMsgBoxOK("正在匹配，无法进行该操作");
                return;
            }

            frameMgr.OpenFrame<PackageNewFrame>(FrameLayer.Middle);
        }

        [UIEventHandleAttribute("btnSkill")]
        void _OnClickOpenSkillFrame()
        {
            if (bIsMatching || frameMgr.IsFrameOpen<PkSeekWaiting>() || Pk3v3CrossDataManager.GetInstance().CheckRoomIsMatching())
            {
                SystemNotifyManager.SysNotifyMsgBoxOK("正在匹配，无法进行该操作");
                return;
            }

            frameMgr.OpenFrame<SkillFrame>(FrameLayer.Middle);
        }

        [UIEventHandleAttribute("btnTeamList")]
        void _OnClickOpenTeamListFrame()
        {
            if(Pk3v3CrossDataManager.GetInstance().HasTeam())
            {
                frameMgr.OpenFrame<Pk3v3CrossMyTeamFrame>(FrameLayer.Middle);
            }
            else
            {
                frameMgr.OpenFrame<Pk3v3CrossTeamListFrame>(FrameLayer.Middle);
            }
        }

        [UIEventHandleAttribute("btnRank")]
        void _OnClickOpenRankFrame()
        {
            frameMgr.OpenFrame<Pk3v3CrossRankListFrame>(FrameLayer.Middle);
        }


        [MessageHandle(SceneScoreWarRewardRes.MsgID)]
        void OnGetPkAwardRes(MsgDATA msg)
        {
            SceneScoreWarRewardRes res = new SceneScoreWarRewardRes();
            res.decode(msg.bytes);

            if (res.result != (uint)ProtoErrorCode.SUCCESS)
            {
                SystemNotifyManager.SystemNotify((int)res.result);
            }

            return;
        }

        protected override void _bindExUI()
        {
            test = mBind.GetCom<ComUIMultListScript>("test111");

            test.Initialize();
            test.OnSelectPrefab = (int index) =>
            {
                return index % 2;
            };
            test.SetElementAmount(5);

            mBtClose = mBind.GetCom<Button>("btClose");
            mBtClose.onClick.AddListener(_onBtCloseButtonClick);
            mBtMenu = mBind.GetCom<Button>("btMenu");
            mBtMenu.onClick.AddListener(_onBtMenuButtonClick);
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

            txtPkCountInfo = mBind.GetCom<Text>("PkCountInfo");
            goBattleTimeState = mBind.GetGameObject("goBattleTimeState");

            goScoreWarStateTimeInfo = mBind.GetGameObject("goScoreWarStateTimeInfo");
            txtTimeInfo = mBind.GetCom<Text>("txtTimeInfo");

            for (int i = 0;i < 6;i++)
            {
                Text txtNumber = mBind.GetCom<Text>(string.Format("txtTime{0}", i));
                if (txtNumber != null)
                {
                    arrNumbers.Add(txtNumber);
                }
            }

            btnFirstBattleChest = mBind.GetCom<Button>("btnFirstBattleChest");
            btnFirstWinChest = mBind.GetCom<Button>("btnFirstWinChest");
            btnFiveBattleChest = mBind.GetCom<Button>("btnFiveBattleChest");
            goFirstBattleGet = mBind.GetCom<Image>("goFirstBattleGet");
            goFiveBattleGet = mBind.GetCom<Image>("goFiveBattleGet");
            goFirstWinGet = mBind.GetCom<Image>("goFirstWinGet");

            gofiveBattleAward = mBind.GetGameObject("gofiveBattleAward");
            gofirstBattleAward = mBind.GetGameObject("gofirstBattleAward");
            goMatchOK = mBind.GetGameObject("goMatchOK");

            mBeginGray = mBind.GetCom<UIGray>("btnBeginGray");

            DOTweenAnimation ani = mBind.GetCom<DOTweenAnimation>("firstWinDotween");

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

            txtPkCountInfo = null;
            goBattleTimeState = null;
            arrNumbers.Clear();

            btnFirstBattleChest = null;
            btnFirstWinChest = null;
            btnFiveBattleChest = null;
            goFirstBattleGet = null;
            goFiveBattleGet = null;
            goFirstWinGet = null;
            gofiveBattleAward = null;
            gofirstBattleAward = null;
            goMatchOK = null;
            mBeginGray = null;

            goScoreWarStateTimeInfo = null;
            txtTimeInfo = null;

            mTalkVoiceRoot = null;
        }
        #endregion
      
        #region Callback
        private void _onBtCloseButtonClick()
        {
            if (bIsMatching || frameMgr.IsFrameOpen<PkSeekWaiting>() || Pk3v3CrossDataManager.GetInstance().CheckRoomIsMatching())
            {
                SystemNotifyManager.SysNotifyFloatingEffect("匹配中无法进行该操作，请取消后再试");
                return;
            }
           

            RoomInfo roomInfo = Pk3v3CrossDataManager.GetInstance().GetRoomInfo();
            if (roomInfo != null && roomInfo.roomSimpleInfo.id > 0)
            {
                SystemNotifyManager.SysNotifyMsgBoxOkCancel("退出活动场景会自动离开积分赛队伍，是否确认退出？", () =>
                {
                    RoomInfo roomInfo1 = Pk3v3CrossDataManager.GetInstance().GetRoomInfo();

                    if (roomInfo1 != null && roomInfo1.roomSimpleInfo.id > 0)
                    {
                        SendQuitRoomReq();
                    }

                    SwitchSceneToTown();
                });
            }
            else
            {
                SwitchSceneToTown();
            }           

            return;

//             if(bIsMatching)
//             {
//                 SystemNotifyManager.SysNotifyFloatingEffect("正在匹配中,无法进行操作");
//                 return;
//             }
// 
//             if (frameMgr.IsFrameOpen<PkSeekWaiting>())
//             {
//                 SystemNotifyManager.SysNotifyFloatingEffect("正在匹配中,无法进行操作");
//                 return;
//             }
// 
//             if (Pk3v3CrossDataManager.GetInstance().CheckRoomIsMatching())
//             {
//                 return;
//             }
// 
//             RoomInfo roomInfo = Pk3v3CrossDataManager.GetInstance().GetRoomInfo();
// 
//             if (roomInfo != null && roomInfo.roomSimpleInfo.id > 0)
//             {
//                 SendQuitRoomReq();     
//             }
// 
//             SwitchSceneToTown();
        }

        private void _onBtMenuButtonClick()
        {
            if (Pk3v3CrossDataManager.GetInstance().CheckRoomIsMatching())
            {
                return;
            }

            ClientSystemManager.GetInstance().OpenFrame<PkMenuFrame>(FrameLayer.Middle);
        }

        private void _onBtInviteFriendsButtonClick()
        {
            if (Pk3v3CrossDataManager.GetInstance().CheckRoomIsMatching())
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
            RoomInfo roomInfo = Pk3v3CrossDataManager.GetInstance().GetRoomInfo();
            if(roomInfo == null || roomInfo.roomSlotInfos == null)
            {
                return;
            }

            if(roomInfo.roomSimpleInfo != null && roomInfo.roomSimpleInfo.id > 0)
            {
                if (roomInfo.roomSimpleInfo.roomStatus == (byte)RoomStatus.ROOM_STATUS_OPEN)
                {
                    if (PlayerBaseData.GetInstance().RoleID != roomInfo.roomSimpleInfo.ownerId)
                    {
                        SystemNotifyManager.SysNotifyFloatingEffect("你不是队长,无法开始游戏");
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
                }

                if (roomInfo.roomSimpleInfo.roomStatus == (byte)RoomStatus.ROOM_STATUS_READY)
                {
                    return;
                }

                if (bMatchLock)
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
                    //Logger.LogErrorFormat("SendCancelGameReq,bMatchLock = {0}", bMatchLock);
                    SendCancelGameReq();
                }
                else
                {
                    Logger.LogErrorFormat("Pk3v3 begin state is error, roomstate = {0}", roomInfo.roomSimpleInfo.roomStatus);
                }
            }
            else
            {
                if (bMatchLock)
                {
                    return;
                }

                bMatchLock = true;

                if (!Pk3v3CrossDataManager.GetInstance().IsMathcing())
                {
                    SendBeginOnePersonMatchGameReq();
                }
                else
                {
                    SendCancelOnePersonMatchGameReq();
                }                
            }
        }

        private void _onBtSettingButtonClick()
        {
            if(Pk3v3CrossDataManager.GetInstance().CheckRoomIsMatching())
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
