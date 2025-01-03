using System;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using ProtoTable;
using Protocol;
using Network;
using System;
using System.Collections.Generic;

namespace GameClient
{
    public class ChijiRoomData
    {
        public CitySceneTable.eSceneSubType SceneSubType = CitySceneTable.eSceneSubType.NULL;
        public int CurrentSceneID = 0;
        public int TargetTownSceneID = 0;

        public void Clear()
        {
            SceneSubType = CitySceneTable.eSceneSubType.NULL;
            CurrentSceneID = 0;
            TargetTownSceneID = 0;
        }
    }

    public class ChijiWaitingRoomFrame : ClientFrame
    {
        private ChijiRoomData roomData = new ChijiRoomData();
        private uint StartServerTime = 0;
        private float SettlementTime = 0.0f;

        //摇杆相关
        private bool mIsStopMoveFunction = false;
        private bool mLastJoyStickFizzyCheck = false;
        private Vector2 mLastJoyStickPosition = Vector2.zero;
        float m_sin60 = 0.8660254f;
        float m_sin45 = 0.7071067f;
        float m_sin30 = 0.5f;

        InputManager _inputManager;
        ComTalk mComTalk;
        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Chiji/ChijiWaitingRoomFrame";
        }
        void InitData()
        {
            // 这里可以加一些表格数据的初始化，比如需要拿系统数值表的一些数据，可以在这里加代码
            mLastJoyStickPosition = Vector2.zero;
            mLastJoyStickFizzyCheck = false;
            mIsStopMoveFunction = false;
        }

        void InitJoystick()
        {
            _inputManager = new InputManager();
            _inputManager.LoadJoystick(SettingManager.GetInstance().GetJoystickMode());
            _inputManager.SetJoyStickMoveCallback(_OnJoyStickMove);
            _inputManager.SetJoyStickMoveEndCallback(_OnJoyStickStop);
        }
        void _OnJoyStickStop()
        {
            if (ChijiDataManager.GetInstance().IsMainPlayerDead) return;
            mIsStopMoveFunction = false;
            mLastJoyStickFizzyCheck = false;

            ClientSystemGameBattle cursystem = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemGameBattle;

            if (cursystem != null && cursystem.MainPlayer != null)
            {
                if (cursystem.MainPlayer.MoveState == BeFighterMain.EMoveState.AutoMoving)
                {
                    return;
                }

                cursystem.MainPlayer.CommandStopMove();
            }
        }
        void _OnJoyStickMove(Vector2 pos)
        {

            var cursystem = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemGameBattle;
            if (cursystem.MainPlayer == null || ChijiDataManager.GetInstance().IsMainPlayerDead)
            {
                return;
            }

            if (pos == Vector2.zero)
            {
                return;
            }

            if (mLastJoyStickFizzyCheck)
            {
                Vector2 distance = mLastJoyStickPosition - pos;

                if (distance.magnitude < 0.438f)
                {
                    return;
                }

                mLastJoyStickFizzyCheck = false;
            }

            mLastJoyStickPosition = pos;

            if (mIsStopMoveFunction)
            {
                return;
            }

            Vector2 dir = pos.normalized;
            if (dir.x > m_sin60)
            {
                dir.x = 1.0f;
                dir.y = 0.0f;
            }
            else if (dir.x > m_sin30 && dir.x <= m_sin60)
            {
                dir.x = m_sin45;
                if (dir.y > 0)
                {
                    dir.y = m_sin45;
                }
                else
                {
                    dir.y = -m_sin45;
                }
            }
            else if (dir.x > -m_sin30 && dir.x <= m_sin30)
            {
                dir.x = 0;
                if (dir.y > 0)
                {
                    dir.y = 1.0f;
                }
                else
                {
                    dir.y = -1.0f;
                }
            }
            else if (dir.x > -m_sin60 && dir.x <= -m_sin30)
            {
                dir.x = -m_sin45;
                if (dir.y > 0)
                {
                    dir.y = m_sin45;
                }
                else
                {
                    dir.y = -m_sin45;
                }
            }
            else if (dir.x <= -m_sin60)
            {
                dir.x = -1.0f;
                dir.y = 0.0f;
            }

            Vector3 dirVec3 = new Vector3(dir.x, 0.0f, dir.y);




            if (cursystem.MainPlayer.ActorData.MoveData.TargetDirection != dirVec3)
            {
                cursystem.MainPlayer.CommandMoveForward(dirVec3);
            }
        }
        protected sealed override void _OnOpenFrame()
        {
#if MG_TEST || UNITY_EDITOR
            mBtLog.CustomActive(true);
#else
            mBtLog.CustomActive(false);
#endif

            StartServerTime = TimeManager.GetInstance().GetServerTime();

            ChijiDataManager.GetInstance().IsMatching = false;
            ChijiDataManager.GetInstance().SwitchingTownToPrepare = false;
            ChijiDataManager.GetInstance().SwitchingChijiSceneToPrepare = false;

            if (ClientSystemManager.GetInstance().IsFrameOpen<MapItemFrame>())
            {
                ClientSystemManager.GetInstance().CloseFrame<MapItemFrame>();
            }

            ClientSystemManager.GetInstance().CloseFrame<ChijiSkillChooseFrame>();
            ItemTipManager.GetInstance().CloseAll();

            InitData();
            InitJoystick();
            _BindUIEvent();

            if (userData != null)
            {
                roomData = userData as ChijiRoomData;
                if (roomData != null)
                {
                    if (mtxtDesc != null)
                    {
                        mtxtDesc.text = "开始匹配";
                    }
                }
            }

            _InitInterface();
            _InitTalk();

            if (ChijiDataManager.GetInstance().GuardForSettlement)
            {
                //ChijiDataManager.GetInstance().OpenSettlementFrame();
                //ChijiDataManager.GetInstance().GuardForSettlement = false;
            }

            // 应服务器要求，初始化的时候发送一次取消匹配，为了让服务器触发一些数据的发送
            BattleEnrollReq req = new BattleEnrollReq
            {
                isMatch = 0u,
                accId = ClientApplication.playerinfo.accid,
                roleId = PlayerBaseData.GetInstance().RoleID,
                playerName = PlayerBaseData.GetInstance().Name,
                playerOccu = (byte)PlayerBaseData.GetInstance().JobTableID
            };

            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }

        protected sealed override void _OnCloseFrame()
        {
            if (ClientSystemManager.GetInstance().IsFrameOpen<ClientSystemTownFrame>())
            {
                ClientSystemTownFrame Townframe = ClientSystemManager.GetInstance().GetFrame(typeof(ClientSystemTownFrame)) as ClientSystemTownFrame;
                Townframe.SetForbidFadeIn(false);
            }

            if (ClientSystemManager.GetInstance().IsFrameOpen<RanklistFrame>())
            {
                ClientSystemManager.GetInstance().CloseFrame<RanklistFrame>();
            }

            if (ClientSystemManager.GetInstance().IsFrameOpen<ChijiHelpFrame>())
            {
                ClientSystemManager.GetInstance().CloseFrame<ChijiHelpFrame>();
            }

            if (ClientSystemManager.GetInstance().IsFrameOpen<ChijiSeekWaitingFrame>())
            {
                ClientSystemManager.GetInstance().CloseFrame<ChijiSeekWaitingFrame>();
            }

            ClientSystemManager.GetInstance().CloseFrame<ChatFrame>();

            if (ClientSystemManager.GetInstance().IsFrameOpen<RelationMenuFram>())
            {
                ClientSystemManager.GetInstance().CloseFrame<RelationMenuFram>();
            }

            _UnBindUIEvent();
            UnloadInput();
            _ClearData();
        }
        void UnloadInput()
        {
            if (_inputManager != null)
            {
                _inputManager.Unload();
                _inputManager = null;
            }
        }
        private void _ClearData()
        {
            roomData.Clear();

            mLastJoyStickFizzyCheck = false;
            mLastJoyStickPosition = Vector2.zero;
            mIsStopMoveFunction = false;
            _inputManager = null;
            StartServerTime = 0;
            SettlementTime = 0.0f;
            if (mComTalk != null)
            {
                ComTalk.Recycle();
                mComTalk = null;
            }
        }

        private void _InitInterface()
        {
            if (mtxtDesc != null)
            {
                mtxtDesc.text = "开始匹配";
            }

            ClientSystemManager.GetInstance().CloseFrame<ChijiSeekWaitingFrame>();

            if (mNum != null)
            {
                //mNum.text = ChijiDataManager.GetInstance().GetPrepareScenePlayerNum().ToString();

               // ClientSystemTown curTown = ClientSystemManager.instance.CurrentSystem as ClientSystemTown;
                ClientSystemGameBattle curTown = ClientSystemManager.instance.CurrentSystem as ClientSystemGameBattle;
                if (curTown != null && curTown.OtherFighters != null)
                {
                    mNum.text = string.Format("{0}", curTown.OtherFighters.GetFightCount() + 1);
                }
            }

            if (mRobot != null)
            {
#if UNITY_EDITOR
                mRobot.CustomActive(true);
#else
                mRobot.CustomActive(false);
#endif
            }

            _UpdateReward();

            UpdateHonorSystemExpValue();

            UpdateIntegralRankBtn();
        }

        private void _InitTalk()
        {
            mComTalk = ComTalk.Create(mTalkParent);
        }

        private void _BindUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.UpdateChijiPrepareScenePlayerNum, _OnUpdatePlayerNum);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ChijiBestRank, _OnUpdateBestRank);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.DisConnect, _OnDisConnect);

            //counter值改变
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnCountValueChange, OnCountValueChangeChanged);

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ActivityUpdate, OnActivityUpdate);
        }

        private void _UnBindUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.UpdateChijiPrepareScenePlayerNum, _OnUpdatePlayerNum);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ChijiBestRank, _OnUpdateBestRank);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.DisConnect, _OnDisConnect);

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnCountValueChange, OnCountValueChangeChanged);

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ActivityUpdate, OnActivityUpdate);
        }

        private void _OnUpdatePlayerNum(UIEvent iEvent)
        {
            if (mNum != null)
            {
                mNum.text = ChijiDataManager.GetInstance().GetPrepareScenePlayerNum().ToString();
            }
        }

        private void _OnUpdateBestRank(UIEvent iEvent)
        {
            _UpdateReward();
        }

        private void _OnDisConnect(UIEvent iEvent)
        {
            ChijiDataManager.GetInstance().IsMatching = false;

            if (mtxtDesc != null) mtxtDesc.text = "开始匹配";

            ChijiDataManager.GetInstance().SwitchingPrepareToChijiScene = false;

            ClientSystemManager.GetInstance().CloseFrame<ChijiSeekWaitingFrame>();
        }

        private void _UpdateReward()
        {
            if (mBestRank != null)
            {
                mBestRank.text = ChijiDataManager.GetInstance().BestRank.ToString();
            }

            if (mItemRoot != null)
            {
                int AwardBoxID = _CalBestRankAward(ChijiDataManager.GetInstance().BestRank);
                ComItem AwardItem = mItemRoot.GetComponentInChildren<ComItem>();

                ItemData ItemDetailData = ItemDataManager.CreateItemDataFromTable(AwardBoxID);
                if (ItemDetailData == null)
                {
                    if (AwardItem != null)
                    {
                        AwardItem.CustomActive(false);
                    }
                }
                else
                {
                    if (AwardItem == null)
                    {
                        AwardItem = CreateComItem(mItemRoot);
                    }

                    AwardItem.CustomActive(true);
                    AwardItem.Setup(ItemDetailData, _ShowItemTip);
                }
            }
        }

        private void _ShowItemTip(GameObject go, ItemData itemData)
        {
            if (null != itemData)
            {
                ItemTipManager.GetInstance().ShowTip(itemData);
            }
        }

        private int _CalBestRankAward(int BestRank)
        {
            var table = TableManager.GetInstance().GetTable<ChijiRewardTable>();

            if (table != null)
            {
                var emu = table.GetEnumerator();

                while (emu.MoveNext())
                {
                    var data = emu.Current.Value as ChijiRewardTable;

                    if (data == null)
                    {
                        continue;
                    }

                    if (BestRank > data.MinRank || BestRank < data.MaxRank)
                    {
                        continue;
                    }

                    return data.RewardBoxID;
                }
            }

            return 0;
        }

        public sealed override bool IsNeedUpdate()
        {
            return true;
        }

        protected sealed override void _OnUpdate(float timeElapsed)
        {
            if (_inputManager != null)
            {
                _inputManager.SingleUpdate(0);
            }

            // 延迟显示结算界面
            if (ChijiDataManager.GetInstance().GuardForSettlement)
            {
                SettlementTime += timeElapsed;

                if (SettlementTime > 0.3f)
                {
                    SettlementTime = 0.0f;

                    if (TimeManager.GetInstance().GetServerTime() - StartServerTime >= 1)
                    {
                        ChijiDataManager.GetInstance().OpenSettlementFrame();
                        ChijiDataManager.GetInstance().GuardForSettlement = false;
                    }
                }
            }
        }

#region ExtraUIBind
        private Button mBtClose = null;
        private Button mBtnEnrollChiji = null;
        private Button mRobot = null;
        private Text mtxtDesc = null;
        private Text mNum = null;
        private Button mRuleBtn = null;
        private Button mIntegralRankBtn = null;
        private GameObject mTalkParent = null;
        private Button mShopBtn = null;
        private Text mBestRank = null;
        private GameObject mItemRoot = null;
        private Button mBtPreview = null;
        private Text mHonorExpValueLabel = null;
        private Button mBtLog = null;

        protected sealed override void _bindExUI()
        {
            mBtClose = mBind.GetCom<Button>("btClose");
            if (null != mBtClose)
            {
                mBtClose.onClick.AddListener(_onBtCloseButtonClick);
            }
            mBtnEnrollChiji = mBind.GetCom<Button>("btnEnrollChiji");
            if (null != mBtnEnrollChiji)
            {
                mBtnEnrollChiji.onClick.AddListener(_onBtnEnrollChijiButtonClick);
            }
            mtxtDesc = mBind.GetCom<Text>("txtEnrollDes");
            mRobot = mBind.GetCom<Button>("btRobot");
            if(mRobot != null)
            {
                mRobot.onClick.AddListener(_onBtnAddRobotButtonClick);
            }
            mNum = mBind.GetCom<Text>("num");
            mRuleBtn = mBind.GetCom<Button>("RuleBtn");
            if (null != mRuleBtn)
            {
                mRuleBtn.onClick.AddListener(_onRuleButtonClick);
            }
            mIntegralRankBtn = mBind.GetCom<Button>("IntegralRankBtn");
            if (null != mIntegralRankBtn)
            {
                mIntegralRankBtn.onClick.AddListener(_onIntegralRankButtonClick);
            }
            mTalkParent = mBind.GetGameObject("TalkParent");

            mShopBtn = mBind.GetCom<Button>("Shop");
            if (mShopBtn != null)
            {
                mShopBtn.onClick.AddListener(_OnShopButtonClick);
            }

            mBestRank = mBind.GetCom<Text>("BestRank");
            mItemRoot = mBind.GetGameObject("itemRoot");
            mBtPreview = mBind.GetCom<Button>("btPreview");
            mBtPreview.onClick.AddListener(_onBtPreviewButtonClick);

            mHonorExpValueLabel = mBind.GetCom<Text>("honorExpValueLabel");
            mBtLog = mBind.GetCom<Button>("btLog");
            mBtLog.onClick.AddListener(_onBtLogButtonClick);
        }

        protected sealed override void _unbindExUI()
        {
            if (null != mBtClose)
            {
                mBtClose.onClick.RemoveListener(_onBtCloseButtonClick);
            }
            mBtClose = null;
            if (null != mBtnEnrollChiji)
            {
                mBtnEnrollChiji.onClick.RemoveListener(_onBtnEnrollChijiButtonClick);
            }
            mBtnEnrollChiji = null;

            if (mRobot != null)
            {
                mRobot.onClick.RemoveListener(_onBtnAddRobotButtonClick);
            }
            mNum = null;
            if (null != mRuleBtn)
            {
                mRuleBtn.onClick.RemoveListener(_onRuleButtonClick);
            }
            mRuleBtn = null;
            if (null != mIntegralRankBtn)
            {
                mIntegralRankBtn.onClick.RemoveListener(_onIntegralRankButtonClick);
            }
            mIntegralRankBtn = null;
            mTalkParent = null;

            if (mShopBtn != null)
            {
                mShopBtn.onClick.RemoveListener(_OnShopButtonClick);
            }
            mShopBtn = null;

            mBestRank = null;
            mItemRoot = null;
            mBtPreview.onClick.RemoveListener(_onBtPreviewButtonClick);
            mBtPreview = null;

            mHonorExpValueLabel = null;

            mBtLog.onClick.RemoveListener(_onBtLogButtonClick);
            mBtLog = null;
        }
#endregion

#region Callback
        private void _onBtCloseButtonClick()
        {
            if(ChijiDataManager.GetInstance().IsMatching)
            {
                SystemNotifyManager.SystemNotify(4200006);
                return;
            }

            ChijiDataManager.GetInstance().SwitchingPrepareToTown = true;

            //ChijiDataManager.GetInstance().SwitchingPrepareToChijiScene = false;

            //BattleEnrollReq req = new BattleEnrollReq
            //{
            //    isMatch = 0,
            //    accId = ClientApplication.playerinfo.accid,
            //    roleId = PlayerBaseData.GetInstance().RoleID,
            //    playerName = PlayerBaseData.GetInstance().Name,
            //    playerOccu = (byte)PlayerBaseData.GetInstance().JobTableID
            //};

            //NetManager netMgr = NetManager.Instance();
            //netMgr.SendCommand(ServerType.GATE_SERVER, req);

            //GameFrameWork.instance.StartCoroutine(systemTown._NetSyncChangeScene(new SceneParams
            //{
            //    currSceneID = roomData.CurrentSceneID,
            //    currDoorID = 0,
            //    targetSceneID = roomData.TargetTownSceneID,
            //    targetDoorID = 0,
            //}, true));

            ClientSystemGameBattle systemTown = ClientSystemManager.GetInstance().GetCurrentSystem() as ClientSystemGameBattle;
            if (systemTown == null)
            {
                Logger.LogError("Current System is not GameBattle!!!");
                return;
            }
            BattleEnrollReq req = new BattleEnrollReq
            {
                isMatch = 0,
                accId = ClientApplication.playerinfo.accid,
                roleId = PlayerBaseData.GetInstance().RoleID,
                playerName = PlayerBaseData.GetInstance().Name,
                playerOccu = (byte)PlayerBaseData.GetInstance().JobTableID
            };

            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);
            ClientSystemManager.GetInstance().SwitchSystem<ClientSystemTown>();
            frameMgr.CloseFrame(this);
        }

        private void _onBtnEnrollChijiButtonClick()
        {
            if (TeamDataManager.GetInstance().HasTeam())
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("Chiji_has_team_Cannotmatch"));
                return;
            }

            if (!ChijiDataManager.GetInstance().MainFrameChijiButtonIsShow())
            {
                if (ChijiDataManager.GetInstance().IsMatching)
                {
                    SendBattleEnrollReq();
                }
                else
                {
                    SystemNotifyManager.SysNotifyTextAnimation(TR.Value("Chiji_Activity_Close"));
                }
                return;
            }

            SendBattleEnrollReq();
        }

        private void SendBattleEnrollReq()
        {
            BattleEnrollReq req = new BattleEnrollReq
            {
                isMatch = !ChijiDataManager.GetInstance().IsMatching ? 1u : 0u,
                accId = ClientApplication.playerinfo.accid,
                roleId = PlayerBaseData.GetInstance().RoleID,
                playerName = PlayerBaseData.GetInstance().Name,
                playerOccu = (byte)PlayerBaseData.GetInstance().JobTableID
            };

            ChijiDataManager.GetInstance().IsMatching = !ChijiDataManager.GetInstance().IsMatching;

            if (ChijiDataManager.GetInstance().IsMatching)
            {
                mtxtDesc.text = "取消匹配";
                ChijiDataManager.GetInstance().SwitchingPrepareToChijiScene = true;
            }
            else
            {
                mtxtDesc.text = "开始匹配";
                ChijiDataManager.GetInstance().SwitchingPrepareToChijiScene = false;
            }

            // 这里对时序又要求，要先置变量mStartSwitchBetweenChijiAndTown的状态，再发协议
            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }

        private void _onBtnAddRobotButtonClick()
        {
            var req = new BattleServerAddPkRobot();
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER,req);
        }

        private void _onRuleButtonClick()
        {
            ClientSystemManager.GetInstance().OpenFrame<ChijiHelpFrame>(FrameLayer.Middle);
        }

        private void _onIntegralRankButtonClick()
        {
            if (ChijiDataManager.GetInstance().IsMatching)
            {
                SystemNotifyManager.SystemNotify(4200006);
                return;
            }

            ClientSystemManager.GetInstance().OpenFrame<RanklistFrame>(FrameLayer.Middle, SortListType.SORTLIST_CHIJI_SCORE);
        }

        private void _OnShopButtonClick()
        {
            ShopNewDataManager.GetInstance().OpenShopNewFrame(29);
        }

        private void _onBtPreviewButtonClick()
        {
            ClientSystemManager.GetInstance().OpenFrame<ChijiRewardPreviewFrame>(FrameLayer.Middle);
        }

        private void _onBtLogButtonClick()
        {
            ClientSystemManager.GetInstance().OpenFrame<ChijiBugReportFrame>();
        }
        #endregion

        #region HonorSystemExpValue

        private void OnCountValueChangeChanged(UIEvent uiEvent)
        {
            if (uiEvent == null || uiEvent.Param1 == null)
                return;

            string countKey = (string) uiEvent.Param1;

            if(string.Equals(countKey, HonorSystemDataManager.GetInstance().ChiJiHonorExpCounterStr) == true)
                UpdateHonorSystemExpValue();
        }

        private void OnActivityUpdate(UIEvent a_event)
        {
            var activityId = (uint)a_event.Param1;
            var chijiActivityIdList = ChijiDataManager.GetInstance().ChijiActivityIDs.ToList<int>();
            if (chijiActivityIdList != null && chijiActivityIdList.Contains((int)activityId))
            {
                UpdateIntegralRankBtn();
            }
        }

        private void UpdateHonorSystemExpValue()
        {
            if (mHonorExpValueLabel == null)
                return;

            var honorExpValueStr = HonorSystemUtility.GetThisWeekHonorExpStrInChiJi();
            mHonorExpValueLabel.text = honorExpValueStr;
        }

        private void UpdateIntegralRankBtn()
        {
            mIntegralRankBtn.CustomActive(ChijiDataManager.GetInstance().MainFrameChijiButtonIsShow());
        }

        #endregion
    }
}
