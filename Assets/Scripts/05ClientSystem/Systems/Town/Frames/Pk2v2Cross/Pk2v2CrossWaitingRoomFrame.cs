using System;
using System.Collections.Generic;
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
    // Pk2v2CrossWaitingRoom
    public class Pk2v2CrossWaitingRoomFrame : GameFrame
    {
        #region inner define

        #endregion

        #region val
        List<object> testComUIListDatas = new List<object>();
        ComTalk m_miniTalk = null;

        PkWaitingRoomData RoomData = new PkWaitingRoomData();

        bool bMatchLock = false;
        bool bIsMatching = false;

        int iFirstBattleAwardID = 0;
        int iFiveBattleAwardID = 0;
        int iTenBattleAwardID = 0;
        int iFirstWinBattleID = 0;       

        #endregion

        #region ui bind
        ComUIListScript testComUIList = null;
        Text testTxt = null;
        Button btClose = null;
        Image testImg = null;
        Slider testSlider = null;
        Toggle testToggle = null;
        GameObject testGo = null;
        GameObject mTalkRoot = null;
        Text mRoomName = null;
        Text mBtMatchText = null;
        UIGray mBeginGray = null;
        Button mBtBegin = null;
        Button mBtnRank = null;

        Button btnFirstBattleChest = null;
        Image goFirstBattleGet = null;
        Button btnFiveBattleChest = null;
        Image goFiveBattleGet = null;
        Button btnFirstWinChest = null;
        Image goFirstWinGet = null;
        Button btnTenBattleChest = null;
        Image goTenBattleGet = null;

        GameObject gofiveBattleAward = null;
        GameObject gofirstBattleAward = null;
        GameObject gotenBattleAward = null;

        Text txtPkCountInfo = null;

        GameObject goScoreWarStateTimeInfo = null;
        Text txtTimeInfo = null;

        #endregion

        #region override

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Pk2v2Cross/Pk2v2CrossWaitingRoom";
        }

        protected override void OnOpenFrame()
        {
            InitTestComUIList();
            UpdateTestComUIList();

            if (userData != null)
            {
                RoomData = userData as PkWaitingRoomData;
            }

            _InitTalk();

            BindUIEvent();

            bMatchLock = false;
            bIsMatching = false;

            UpdateBeginButton();

            {
                Dictionary<int, object> dicts = TableManager.instance.GetTable<ScoreWar2v2RewardTable>();
                if (dicts != null)
                {
                    var iter = dicts.GetEnumerator();
                    while (iter.MoveNext())
                    {
                        ScoreWar2v2RewardTable adt = iter.Current.Value as ScoreWar2v2RewardTable;
                        if (adt == null)
                        {
                            continue;
                        }

                        if (adt.BattleCount == 1 && iFirstBattleAwardID == 0)
                        {
                            iFirstBattleAwardID = adt.RewardId;
                        }
                        if (adt.BattleCount == 5 && iFiveBattleAwardID == 0)
                        {
                            iFiveBattleAwardID = adt.RewardId;
                        }
                        if (adt.BattleCount == 10 && iTenBattleAwardID == 0)
                        {
                            iTenBattleAwardID = adt.RewardId;
                        }
                        if (adt.WinCount == 1 && iFirstWinBattleID == 0)
                        {
                            iFirstWinBattleID = adt.RewardId;
                        }

                        if (iFirstBattleAwardID > 0 && iFirstWinBattleID > 0 && iFiveBattleAwardID > 0 && iTenBattleAwardID > 0)
                        {
                            break;
                        }
                    }
                }
            }

//             Pk2v2CrossDataManager.My2v2PkInfo pkInfo = Pk2v2CrossDataManager.GetInstance().PkInfo;
//             pkInfo.nWinCount = (byte)GameDebug.instance.ints[0];
//             pkInfo.nCurPkCount = GameDebug.instance.ints[1];
// 
//             pkInfo.arrAwardIDs.Clear();
//             pkInfo.arrAwardIDs.AddRange(GameDebug.instance.uints);

            OnPk2v2CrossRewardInfoUpdate(null);
        }

        protected override void OnCloseFrame()
        {
            iFirstBattleAwardID = 0;
            iFiveBattleAwardID = 0;
            iTenBattleAwardID = 0;
            iFirstWinBattleID = 0;

            UnBindUIEvent();

            if(RoomData != null)
            {
                RoomData.Clear();
            }
            RoomData = null;
            bMatchLock = false;
            bIsMatching = false;

            if (ClientSystemManager.GetInstance().IsFrameOpen<ClientSystemTownFrame>())
            {
                ClientSystemTownFrame Townframe = ClientSystemManager.GetInstance().GetFrame(typeof(ClientSystemTownFrame)) as ClientSystemTownFrame;
                Townframe.SetForbidFadeIn(false);
            }

            if (m_miniTalk != null)
            {
                ComTalk.Recycle();
                m_miniTalk = null;
            }
        }

        protected override void _bindExUI()
        {
            testComUIList = mBind.GetCom<ComUIListScript>("testComUIList");
            testTxt = mBind.GetCom<Text>("testTxt");

            btClose = mBind.GetCom<Button>("btClose");
            btClose.SafeSetOnClickListener(() => 
            {
                if (bIsMatching || frameMgr.IsFrameOpen<PkSeekWaiting>() || Pk2v2CrossDataManager.GetInstance().IsMathcing())
                {
                    SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("2v2melee_score_war_is_matching"));
                    return;
                }

                SwitchSceneToTown();

                return;
            });

            testImg = mBind.GetCom<Image>("testImg");

            testSlider = mBind.GetCom<Slider>("testSlider");
            testSlider.SafeSetValueChangeListener((value) => 
            {

            });

            testToggle = mBind.GetCom<Toggle>("testToggle");
            testToggle.SafeSetOnValueChangedListener((value) => 
            {

            });

            testGo = mBind.GetGameObject("testGo");

            mTalkRoot = mBind.GetGameObject("TalkRoot");
            mRoomName = mBind.GetCom<Text>("RoomName");

            mBtMatchText = mBind.GetCom<Text>("btMatchText");
            mBeginGray = mBind.GetCom<UIGray>("btnBeginGray");
            mBtBegin = mBind.GetCom<Button>("btBegin");
            mBtBegin.SafeSetOnClickListener(() => 
            {
                if (bMatchLock)
                {
                    return;
                }

                bMatchLock = true;

                if (!Pk2v2CrossDataManager.GetInstance().IsMathcing())
                {
                    SendBeginOnePersonMatchGameReq();
                }
                else
                {
                    SendCancelOnePersonMatchGameReq();
                }                
            });

            mBtnRank = mBind.GetCom<Button>("btnRank");
            mBtnRank.SafeSetOnClickListener(() => 
            {
                frameMgr.OpenFrame<Pk2v2CrossRankListFrame>(FrameLayer.Middle);
            });

            btnFirstBattleChest = mBind.GetCom<Button>("btnFirstBattleChest");
            btnFirstWinChest = mBind.GetCom<Button>("btnFirstWinChest");
            btnFiveBattleChest = mBind.GetCom<Button>("btnFiveBattleChest");
            btnTenBattleChest = mBind.GetCom<Button>("btnTenBattleChest");
            goFirstBattleGet = mBind.GetCom<Image>("goFirstBattleGet");
            goFiveBattleGet = mBind.GetCom<Image>("goFiveBattleGet");
            goFirstWinGet = mBind.GetCom<Image>("goFirstWinGet");
            goTenBattleGet = mBind.GetCom<Image>("goTenBattleGet");

            gofiveBattleAward = mBind.GetGameObject("gofiveBattleAward");
            gofirstBattleAward = mBind.GetGameObject("gofirstBattleAward");
            gotenBattleAward = mBind.GetGameObject("gotenBattleAward");

            txtPkCountInfo = mBind.GetCom<Text>("PkCountInfo");

            goScoreWarStateTimeInfo = mBind.GetGameObject("goScoreWarStateTimeInfo");
            txtTimeInfo = mBind.GetCom<Text>("txtTimeInfo");
        }

        protected override void _unbindExUI()
        {
            testComUIList = null;

            testTxt = null;
            btClose = null;
            testImg = null;
            testSlider = null;
            testToggle = null;
            testGo = null;
            mTalkRoot = null;
            mRoomName = null;

            mBtMatchText = null;
            mBeginGray = null;
            mBtBegin = null;
            mBtnRank = null;

            btnFirstBattleChest = null;
            btnFirstWinChest = null;
            btnFiveBattleChest = null;
            btnTenBattleChest = null;
            goFirstBattleGet = null;
            goFiveBattleGet = null;
            goTenBattleGet = null;
            goFirstWinGet = null;
            gofiveBattleAward = null;
            gotenBattleAward = null;
            gofirstBattleAward = null;
            txtPkCountInfo = null;
            goScoreWarStateTimeInfo = null;
            txtTimeInfo = null;
        }

        public override bool IsNeedUpdate()
        {
            return true;
        }

        protected override void _OnUpdate(float timeElapsed)
        {
            ScoreWar2V2Status state = Pk2v2CrossDataManager.GetInstance().Get2v2CrossWarStatus();
            if (state == ScoreWar2V2Status.SWS_2V2_PREPARE || state == ScoreWar2V2Status.SWS_2V2_BATTLE/* || state == ScoreWarStatus.SWS_WAIT_END*/)
            {
                if (goScoreWarStateTimeInfo != null)
                {
                    goScoreWarStateTimeInfo.CustomActive(true);
                }

                int nLeftDay = 0;
                int nLeftHour = 0;
                int nLeftMin = 0;
                int nLeftSec = 0;

                GetLeftTime(Pk2v2CrossDataManager.GetInstance().Get2v2CrossWarStatusEndTime(), TimeManager.GetInstance().GetServerTime(), ref nLeftDay, ref nLeftHour, ref nLeftMin, ref nLeftSec);

                if (state == ScoreWar2V2Status.SWS_2V2_PREPARE)
                {
                    txtTimeInfo.text = TR.Value("2v2melee_score_war_prepare_time_info", string.Format("{0:00}", nLeftHour), string.Format("{0:00}", nLeftMin), string.Format("{0:00}", nLeftSec));
                }
                else if (state == ScoreWar2V2Status.SWS_2V2_BATTLE)
                {
                    txtTimeInfo.text = TR.Value("2v2melee_score_war_pking_time_info", string.Format("{0:00}", nLeftHour), string.Format("{0:00}", nLeftMin), string.Format("{0:00}", nLeftSec));
                }

            }
            else
            {
                if (goScoreWarStateTimeInfo != null)
                {
                    goScoreWarStateTimeInfo.CustomActive(false);
                }
            }
        }

        #endregion

        #region method

        void BindUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnCountValueChange, _OnOnCountValueChange);

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.Pk2v2CrossBeginMatch, OnBeginMatch);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.Pk2v2CrossBeginMatchRes, OnBeginMatchRes);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.Pk2v2CrossCancelMatch, OnCancelMatch);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.Pk2v2CrossCancelMatchRes, OnCancelMatchRes);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.Pk2v2CrossPkAwardInfoUpdate, OnPk2v2CrossRewardInfoUpdate);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PK2V2CrossStatusUpdate, OnPK2V2CrossStatusUpdate);          
        }

        void UnBindUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnCountValueChange, _OnOnCountValueChange);

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.Pk2v2CrossBeginMatch, OnBeginMatch);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.Pk2v2CrossBeginMatchRes, OnBeginMatchRes);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.Pk2v2CrossCancelMatch, OnCancelMatch);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.Pk2v2CrossCancelMatchRes, OnCancelMatchRes);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.Pk2v2CrossPkAwardInfoUpdate, OnPk2v2CrossRewardInfoUpdate);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PK2V2CrossStatusUpdate, OnPK2V2CrossStatusUpdate); 
        }

        void _InitTalk()
        {
            m_miniTalk = ComTalk.Create(mTalkRoot);
        }

        void SendBeginOnePersonMatchGameReq()
        {
            WorldMatchStartReq req = new WorldMatchStartReq();
            req.type = (byte)PkType.PK_2V2_ACTIVITY;

            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }

        void SendCancelOnePersonMatchGameReq()
        {
            WorldMatchCancelReq req = new WorldMatchCancelReq();

            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }

        bool IsGetFirstBattleAward()
        {
            Pk2v2CrossDataManager.My2v2PkInfo kInfo = Pk2v2CrossDataManager.GetInstance().PkInfo;
            for (int i = 0; i < kInfo.arrAwardIDs.Count; i++)
            {
                if (iFirstBattleAwardID == kInfo.arrAwardIDs[i])
                {
                    return true;
                }
            }

            return false;
        }

        bool IsGetFiveBattleAward()
        {
            Pk2v2CrossDataManager.My2v2PkInfo kInfo = Pk2v2CrossDataManager.GetInstance().PkInfo;
            for (int i = 0; i < kInfo.arrAwardIDs.Count; i++)
            {
                if (iFiveBattleAwardID == kInfo.arrAwardIDs[i])
                {
                    return true;
                }
            }

            return false;
        }

        bool IsGetTenBattleAward()
        {
            Pk2v2CrossDataManager.My2v2PkInfo kInfo = Pk2v2CrossDataManager.GetInstance().PkInfo;
            for (int i = 0; i < kInfo.arrAwardIDs.Count; i++)
            {
                if (iTenBattleAwardID == kInfo.arrAwardIDs[i])
                {
                    return true;
                }
            }

            return false;
        }

        bool IsGetFirstWinAward()
        {
            Pk2v2CrossDataManager.My2v2PkInfo kInfo = Pk2v2CrossDataManager.GetInstance().PkInfo;
            for (int i = 0; i < kInfo.arrAwardIDs.Count; i++)
            {
                if (iFirstWinBattleID == kInfo.arrAwardIDs[i])
                {
                    return true;
                }
            }

            return false;
        }

        void UpdateAwardInfo(int iAwardID, Func<bool> isGet, Func<bool> canGet, Button btnChest, Image goImageGet, GameObject goAni)
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
                    Scene2V2ScoreWarRewardReq req = new Scene2V2ScoreWarRewardReq();
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

                    ScoreWar2v2RewardTable tableItem = TableManager.GetInstance().GetTableItem<ProtoTable.ScoreWar2v2RewardTable>(iAwardID);
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
                Pk2v2CrossDataManager.My2v2PkInfo kInfo = Pk2v2CrossDataManager.GetInstance().PkInfo;
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
                    Pk2v2CrossDataManager.My2v2PkInfo kInfo = Pk2v2CrossDataManager.GetInstance().PkInfo;
                    return kInfo.nCurPkCount >= 5;
                },
                btnFiveBattleChest,
                goFiveBattleGet,
                mBind.GetGameObject("goFiveBattleAni"));

                return;
            }
        }

        void UpdateTenBattleAwardChest(UIEvent uiEvent)
        {
            if (!IsGetFiveBattleAward())
            {
                gotenBattleAward.CustomActive(false);
            }
            else
            {
                gotenBattleAward.CustomActive(true);

                UpdateAwardInfo(iTenBattleAwardID, IsGetTenBattleAward, () =>
                {
                    Pk2v2CrossDataManager.My2v2PkInfo kInfo = Pk2v2CrossDataManager.GetInstance().PkInfo;
                    return kInfo.nCurPkCount >= 10;
                },
                btnTenBattleChest,
                goTenBattleGet,
                mBind.GetGameObject("goTenBattleAni"));

                return;
            }
        }

        void UpdateFirstWinAwardChest(UIEvent uiEvent)
        {
            UpdateAwardInfo(iFirstWinBattleID, IsGetFirstWinAward, () =>
            {
                Pk2v2CrossDataManager.My2v2PkInfo kInfo = Pk2v2CrossDataManager.GetInstance().PkInfo;
                return kInfo.nWinCount >= 1;
            },
            btnFirstWinChest,
            goFirstWinGet,
            mBind.GetGameObject("goFirstWinAni"));
            return;
        }

        void UpdateBeginButton()
        {
            ScoreWar2V2Status state = Pk2v2CrossDataManager.GetInstance().Get2v2CrossWarStatus();

            mBeginGray.SetEnable(false);
            mBtBegin.interactable = true;           

            mBtBegin.CustomActive(true);

            Pk2v2CrossDataManager.My2v2PkInfo pkInfo = Pk2v2CrossDataManager.GetInstance().PkInfo;
            if (pkInfo != null)
            {
                if (pkInfo.nCurPkCount >= Pk2v2CrossDataManager.MAX_PK_COUNT || state != ScoreWar2V2Status.SWS_2V2_BATTLE)
                {
                    mBeginGray.SetEnable(true);
                    mBtBegin.interactable = false;
                }
            }
        }

        void GetLeftTime(uint nEndTime, uint nNowTime, ref int nLeftDay, ref int nLeftHour, ref int nLeftMin, ref int nLeftSec)
        {
            nLeftDay = 0;
            nLeftHour = 0;
            nLeftMin = 0;
            nLeftSec = 0;

            if (nEndTime <= nNowTime)
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

        void InitTestComUIList()
        {
            if(testComUIList == null)
            {
                return;
            }

            testComUIList.Initialize();
            testComUIList.onBindItem = (go) => 
            {
                return go;
            };

            testComUIList.onItemVisiable = (go) => 
            {
                if(go == null)
                {
                    return;
                }

                if(testComUIListDatas == null)
                {
                    return;
                }

                ComUIListTemplateItem comUIListItem = go.GetComponent<ComUIListTemplateItem>();
                if(comUIListItem == null)
                {
                    return;
                }

                if(go.m_index >= 0 && go.m_index < testComUIListDatas.Count)
                {
                    comUIListItem.SetUp(testComUIListDatas[go.m_index]);
                }                
            };          
        }

        void CalcTestComUIListDatas()
        {
            testComUIListDatas = new List<object>();
            if(testComUIListDatas == null)
            {
                return;
            }
        }

        void UpdateTestComUIList()
        {
            if(testComUIList == null)
            {
                return;
            }

            CalcTestComUIListDatas();
            if(testComUIListDatas == null)
            {
                return;
            }

            testComUIList.SetElementAmount(testComUIListDatas.Count);            
        }

        void SwitchSceneToTown()
        {
            if(RoomData == null)
            {
                return;
            }

            ClientSystemTown systemTown = ClientSystemManager.GetInstance().GetCurrentSystem() as ClientSystemTown;
            if (systemTown == null)
            {
                Logger.LogError("Current System is not Town from Pk2v2WaitingRoom");
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
        #endregion

        #region ui event

        void _OnOnCountValueChange(UIEvent uiEvent)
        {            
            return;
        }

        void OnBeginMatch(UIEvent iEvent)
        {
            bIsMatching = true;
            UpdateBeginButton();         

            PkSeekWaitingData data = new PkSeekWaitingData();
            data.roomtype = PkRoomType.Pk2v2Cross;

            ClientSystemManager.GetInstance().OpenFrame<PkSeekWaiting>(FrameLayer.Middle, data);

            mBtMatchText.text = TR.Value("2v2melee_score_war_cancel_match");
        }

        void OnBeginMatchRes(UIEvent iEvent)
        {
            bMatchLock = false;
        }

        void OnCancelMatch(UIEvent iEvent)
        {
            bIsMatching = false;
            UpdateBeginButton();

            ClientSystemManager.GetInstance().CloseFrame<PkSeekWaiting>();
            mBtMatchText.text = TR.Value("2v2melee_score_war_start_match");
        }

        void OnCancelMatchRes(UIEvent iEvent)
        {
            //Logger.LogErrorFormat("OnCancelMatchRes,bMatchLock = {0}", bMatchLock);

            bMatchLock = false;
        }

        void OnPK2V2CrossStatusUpdate(UIEvent iEvent)
        {
            UpdateBeginButton();
        }

        void OnPk2v2CrossRewardInfoUpdate(UIEvent iEvent)
        {
            Pk2v2CrossDataManager.My2v2PkInfo pkInfo = Pk2v2CrossDataManager.GetInstance().PkInfo;
            if (pkInfo == null)
            {
                return;
            }

            txtPkCountInfo.text = TR.Value("2v2melee_score_war_pk_count_info", Pk2v2CrossDataManager.MAX_PK_COUNT - pkInfo.nCurPkCount, Pk2v2CrossDataManager.MAX_PK_COUNT);

            UpdateFirstBattleAwardChest(null);
            UpdateFiveBattleAwardChest(null);
            //UpdateTenBattleAwardChest(null);
            UpdateFirstWinAwardChest(null);

            ScoreWar2V2Status state = Pk2v2CrossDataManager.GetInstance().Get2v2CrossWarStatus();
            if (pkInfo.nCurPkCount >= Pk2v2CrossDataManager.MAX_PK_COUNT || state != ScoreWar2V2Status.SWS_2V2_BATTLE)
            {
                mBeginGray.SetEnable(true);
                mBtBegin.interactable = false;
            }

            return;
        }       

        #endregion
    }
}
