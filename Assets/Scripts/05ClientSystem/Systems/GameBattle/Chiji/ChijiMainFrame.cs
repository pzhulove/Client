using System;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using ProtoTable;
using Protocol;
using Network;
using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace GameClient
{
    public class ChijiMainFrame : ClientFrame
    {
        private ChijiRoomData roomData = new ChijiRoomData();
        private ChijiMapFrame m_mapFrame = null;

        public static string mStagename = "";

        private int LastTime = 30; // 持续时间
        private int LeftTime = 0; // 剩余时间
        private float fTimeIntrval = 0.0f;
        private uint StartServerTime = 0;
        private float KillInfoShowTimeIntrval = 0.0f;

        private float SettlementTime = 0.0f;
        private GameObject mDeadTips = null;
        private bool bRefreshPickUpState = false;
        private bool bPickUpFinish = false;
        private bool bWaitPickUpMsg = false;
        private int PickUpItemLastTime = 4;// 捡取道具持续时间
        private float fLocalTime = 0.0f;
        private float DelayHideTime = 0.0f;

        private PickUpType picktype = PickUpType.None;
        private ChiJiSkill[] skillList = null;
        private UInt32[] equipList = null;

        //毒圈路径
        Vector2 center = Vector2.zero;
        float radius = 0.0f;
        float durTime = 0.0f;
        float shrinkTime = 0.0f;
        Vector2 lastCenter = Vector2.zero;
        float lastRadius = 0.0f;

        //摇杆相关
        private bool mIsStopMoveFunction = false;
        private bool mLastJoyStickFizzyCheck = false;
        private Vector2 mLastJoyStickPosition = Vector2.zero;
        float m_sin60 = 0.8660254f;
        float m_sin45 = 0.7071067f;
        float m_sin30 = 0.5f;
        InputManager _inputManager;

        // 攻击范围特效
        GeEffectEx mAttackAreaEffect = null;

        private ChijiPlayerHeadPortraitInfoView mChijiPlayerHeadPortraitInfoView = null;
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Chiji/ChijiMainFrame";
        }

        void InitData()
        {
            // 这里可以加一些表格数据的初始化，比如需要拿系统数值表的一些数据，可以在这里加代码
            mLastJoyStickPosition = Vector2.zero;
            mLastJoyStickFizzyCheck = false;
            mIsStopMoveFunction = false;
        }

        protected override void _OnOpenFrame()
        {
            ChijiDataManager.GetInstance().SwitchingPrepareToChijiScene = false;

#if MG_TEST || UNITY_EDITOR
            mBtReport.CustomActive(true);
#else
            mBtReport.CustomActive(false);
#endif

            InitData();
            InitJoystick();
            _BindUIEvent();
            if (userData != null)
            {
                roomData = userData as ChijiRoomData;
            }

            if(ChijiDataManager.GetInstance().CurBattleStage < ChiJiTimeTable.eBattleStage.BS_PUT_ITEM_1)
            {
                ChiJiTimeTable tablData = TableManager.GetInstance().GetTableItem<ChiJiTimeTable>((int)ChiJiTimeTable.eBattleStage.BS_PUT_ITEM_1);
                if (tablData != null)
                {
                    LastTime = tablData.StartTime;
                }

                LeftTime = LastTime;
            }
            StartServerTime = TimeManager.GetInstance().GetServerTime();

            _LoadChijiPlayerHeadPortraitInfoView();
            _InitInterface();
            if(ChijiDataManager.GetInstance().GuardForPickUpOtherPlayerItems)
            {
                ChijiDataManager.GetInstance().GuardForPickUpOtherPlayerItems = false;
                ClientSystemManager.GetInstance().OpenFrame<PlayerItemFrame>(FrameLayer.Middle);
            }
            if (PlayerBaseData.GetInstance().BuffMgr.HasBuffByID(402000003))
            {
                ShowPoisionEffect(true);
            }

            _InitPoisonData();
        }

        protected override void _OnCloseFrame()
        {
            ClientSystemManager.GetInstance().CloseFrame<SelectOccupationFrame>();
            ClientSystemManager.GetInstance().CloseFrame<SelectMapAreaFrame>();
            ClientSystemManager.GetInstance().CloseFrame<MapItemFrame>();
            ClientSystemManager.GetInstance().CloseFrame<PlayerItemFrame>();
            ClientSystemManager.GetInstance().CloseFrame<ChijiFullMapFrame>();
            ClientSystemManager.GetInstance().CloseFrame<ChijiSkillFrame>();
            ClientSystemManager.GetInstance().CloseFrame<PackageNewFrame>();
            ClientSystemManager.GetInstance().CloseFrame<ChijiNpcDialogFrame>();
            ClientSystemManager.GetInstance().CloseFrame<ChijiHandInEquipmentFrame>();

            if (ClientSystemManager.GetInstance().IsFrameOpen<ClientSystemTownFrame>())
            {
                ClientSystemTownFrame Townframe = ClientSystemManager.GetInstance().GetFrame(typeof(ClientSystemTownFrame)) as ClientSystemTownFrame;
                Townframe.SetForbidFadeIn(false);
            }
            if(mDeadTips != null)
            {
                mDeadTips.CustomActive(false);
            }
            _UnBindUIEvent();
            _UnloadInput();
            _ClearData();

            if (_chijiShopView != null)
            {
                Object.Destroy(_chijiShopView.gameObject);
                _chijiShopView = null;
            }
        }

        private void _ClearData()
        {
            if (roomData != null)
            {
                roomData.Clear();
            }
            
            if(m_mapFrame != null)
            {
                m_mapFrame = null;
            }
            mLastJoyStickFizzyCheck = false;
            mLastJoyStickPosition = Vector2.zero;
            mIsStopMoveFunction = false;
            _inputManager = null;

            LastTime = 60;
            fTimeIntrval = 0.0f;
            StartServerTime = 0;
            KillInfoShowTimeIntrval = 0.0f;
            fLocalTime = 0.0f;
            DelayHideTime = 0.0f;
            picktype = PickUpType.None;

            if (skillList != null)
            {
                skillList = null;
            }

            if(equipList != null)
            {
                equipList = null;
            }

            _ClearChijiMap();

            mChijiPlayerHeadPortraitInfoView = null;
            SettlementTime = 0.0f;

            center = Vector2.zero;
            radius = 0.0f;
            durTime = 0.0f;
            shrinkTime = 0.0f;
            lastCenter = Vector2.zero;
            lastRadius = 0.0f;
            bRefreshPickUpState = false;
            bPickUpFinish = false;
            bWaitPickUpMsg = false;

            if (mAttackAreaEffect != null)
            {
                mAttackAreaEffect = null;
            }
        }

        void _InitInterface()
        {
            ChiJiTimeTable timeData = TableManager.GetInstance().GetTableItem<ChiJiTimeTable>((int)ChijiDataManager.GetInstance().CurBattleStage);

            DisplayAttribute attribute = BeUtility.GetMainPlayerActorAttribute(true, true);
            
            if (mChijiPlayerHeadPortraitInfoView != null)
            {
                mChijiPlayerHeadPortraitInfoView.InitView(attribute);
            }

            if (mSurvivePlayerNum != null)
            {
                mSurvivePlayerNum.text = ChijiDataManager.GetInstance().SurvivePlayerNum.ToString();
            }

            _InitChijiMap();

            if(ChijiDataManager.GetInstance().RecentKillPlayerID != 0)
            {
                KillInfoShowTimeIntrval = 0.0f;
                _UpdateShowKillInfo();
            }

            if (mShowKill != null)
            {
                mShowKill.text = ChijiDataManager.GetInstance().KillNum.ToString();
                //mShowKill.CustomActive(ChijiDataManager.GetInstance().KillNum > 0);
            }

            if (timeData != null)
            {
                mStagename = timeData.StageTip;
            }

             mTime.text = mStagename + LeftTime.ToString();

            _UpdateShowDeathPlayerInfo();

            if (ChijiDataManager.GetInstance().IsReadyPk)
            {
                mPkText.text = "取消挑战";
            }
            else
            {
                mPkText.text = "挑战";
            }

            mPickUpProcess.CustomActive(false);//进度条默认隐藏
        }

        void _BindUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ChijiBattleStageChanged, _OnStageChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.JobIDReset, _OnResetAttr);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ChijiPlayerDead, _OnPlayerDead);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PoisonStatChange, _OnPlayerPoisonStatChange);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.BlueCircleChange, _OnBlueCircleChange);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.StartOpenChijiItem, _OnStartOpenChijiItem);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.FinishOpenChijiItem, _OnFinishOpenChijiItem);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.CancelOpenChijiItem, _OnCancelOpenChijiItem);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OpenChijiSkillChooseFrame, _OnOpenChijiSkillChooseFrame);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ChijiPkReady, _OnChijiPkReady);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ChijiPKButtonChange, _OnPKButtonChange);
        }

        void _UnBindUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ChijiBattleStageChanged, _OnStageChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.JobIDReset, _OnResetAttr);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ChijiPlayerDead, _OnPlayerDead);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PoisonStatChange, _OnPlayerPoisonStatChange);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.BlueCircleChange, _OnBlueCircleChange);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.StartOpenChijiItem, _OnStartOpenChijiItem);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.FinishOpenChijiItem, _OnFinishOpenChijiItem);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.CancelOpenChijiItem, _OnCancelOpenChijiItem);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OpenChijiSkillChooseFrame, _OnOpenChijiSkillChooseFrame);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ChijiPkReady, _OnChijiPkReady);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ChijiPKButtonChange, _OnPKButtonChange);
        }

        void _OnStageChanged(UIEvent iEvent)
        {
            ChiJiTimeTable timeData = TableManager.GetInstance().GetTableItem<ChiJiTimeTable>((int)ChijiDataManager.GetInstance().CurBattleStage);

            if (timeData != null && timeData.StageTip != "")
            {
                mStagename = timeData.StageTip;
            }
        }

        void _OnResetAttr(UIEvent iEvent)
        {
            DisplayAttribute attribute = BeUtility.GetMainPlayerActorAttribute(true, true);

            if (mChijiPlayerHeadPortraitInfoView != null)
            {
                mChijiPlayerHeadPortraitInfoView.InitView(attribute);
            }
        }

        void _OnPlayerDead(UIEvent iEvent)
        {
            if(mSurvivePlayerNum != null)
            {
                mSurvivePlayerNum.text = ChijiDataManager.GetInstance().SurvivePlayerNum.ToString();
            }

            if (ChijiDataManager.GetInstance().RecentKillPlayerID != 0)
            {
                KillInfoShowTimeIntrval = 0.0f;
                _UpdateShowKillInfo();
            }

            if (mShowKill != null)
            {
                mShowKill.text = ChijiDataManager.GetInstance().KillNum.ToString();
                //mShowKill.CustomActive(ChijiDataManager.GetInstance().KillNum > 0);
            }

            _UpdateShowDeathPlayerInfo();
        }

        void _OnPlayerPoisonStatChange(UIEvent iEvent)
        {
            var isInPoison = (bool)iEvent.Param1;
            ShowPoisionEffect(isInPoison);
        }

        void _OnBlueCircleChange(UIEvent iEvent)
        {
            lastCenter = center;
            lastRadius = radius;
            center = (Vector2)iEvent.Param1;
            radius = (float)iEvent.Param2;
            durTime = (float)iEvent.Param3;
            shrinkTime = (float)iEvent.Param4;
            if (m_mapFrame != null)
            {
                m_mapFrame.SetWhiteCircle(center, radius);
                m_mapFrame.SetBlueCircle(center, radius, durTime, shrinkTime);
            }

            if (ClientSystemManager.GetInstance().IsFrameOpen<ChijiFullMapFrame>())
            {
                var frame = ClientSystemManager.GetInstance().GetFrame(typeof(ChijiFullMapFrame)) as ChijiFullMapFrame;
                if (frame != null && frame.MapFrame != null)
                {
                    frame.MapFrame.SetWhiteCircle(center, radius);
                    frame.MapFrame.SetBlueCircle(center, radius, durTime, shrinkTime);
                }
            }
        }

        void _OnStartOpenChijiItem(UIEvent iEvent)
        {
            if (mPickUpProcess.IsActive())
            {
                return;
            }

            PickUpItemLastTime = (int)((UInt32)iEvent.Param1 - TimeManager.GetInstance().GetServerTime());
            int pickUpTime = (int)iEvent.Param2;

            if (PickUpItemLastTime > pickUpTime)
            {
                PickUpItemLastTime = pickUpTime;
            }

            Logger.LogErrorFormat("TimeLeft = {0}", PickUpItemLastTime);

            mPickUpProcess.value = 0;
            fLocalTime = 0.0f;
            DelayHideTime = 0.0f;

            bRefreshPickUpState = true;
            bPickUpFinish = false;
            bWaitPickUpMsg = false;
            mPickUpProcess.CustomActive(true);
        }

        void  _OnFinishOpenChijiItem(UIEvent iEvent)
        {
            if (!mPickUpProcess.IsActive())
            {
                return;
            }

            if (mPickUpProcess != null)
            {
                mPickUpProcess.value = 1.0f;
            }

            bPickUpFinish = true;
            DelayHideTime = fLocalTime; // 记一下结束的时间点
        }

        void _OnCancelOpenChijiItem(UIEvent iEvent)
        {
            if (!mPickUpProcess.IsActive())
            {
                return;
            }

            bRefreshPickUpState = false;
            mPickUpProcess.CustomActive(false);
        }

        void _OnOpenChijiSkillChooseFrame(UIEvent iEvent)
        {
            picktype = (PickUpType)iEvent.Param1;

            ChijiSkillChooseFrame.pickUpType = picktype;

            if(picktype == PickUpType.PickUpSkill)
            {
                skillList = iEvent.Param2 as ChiJiSkill[];
            }
            else if(picktype == PickUpType.PickUpItem)
            {
                equipList = iEvent.Param2 as UInt32[];
            }

            if(bWaitPickUpMsg)
            {
                bWaitPickUpMsg = false;

                if (picktype == PickUpType.PickUpSkill)
                {
                    ClientSystemManager.GetInstance().OpenFrame<ChijiSkillChooseFrame>(FrameLayer.Middle, skillList);
                }
                else if (picktype == PickUpType.PickUpItem)
                {
                    ClientSystemManager.GetInstance().OpenFrame<ChijiSkillChooseFrame>(FrameLayer.Middle, equipList);
                }

                picktype = PickUpType.None;
            }
        }

        void _OnChijiPkReady(UIEvent iEvent)
        {
            mPkText.text = "挑战";
            ClientSystemManager.GetInstance().CloseFrame<ChijiSkillFrame>();
            if (mChallengeTipRoot != null)
            {
                mChallengeTipRoot.CustomActive(false);
            }
        }

        /// <summary>
        /// 加载玩家头像预制体
        /// </summary>
        private void _LoadChijiPlayerHeadPortraitInfoView()
        {
            var uiPrefabWrapper = mPlayerHeadPortraitInfoRoot.GetComponent<UIPrefabWrapper>();
            if (uiPrefabWrapper != null)
            {
                GameObject prefab = uiPrefabWrapper.LoadUIPrefab();
                if (prefab != null)
                {
                    prefab.transform.SetParent(mPlayerHeadPortraitInfoRoot.transform, false);

                    if (mChijiPlayerHeadPortraitInfoView == null)
                    {
                        mChijiPlayerHeadPortraitInfoView = prefab.GetComponent<ChijiPlayerHeadPortraitInfoView>();
                    }
                }
            }
        }

        private void _UpdateShowKillInfo()
        {
            if(mKillInfo != null)
            {
                if (ChijiDataManager.GetInstance().RecentKillPlayerID == PlayerBaseData.GetInstance().RoleID)
                {
                    mKillInfo.text = "你 淘汰了 你自己";
                }
                else
                {
                    JoinPlayerInfo Deadinfo = ChijiDataManager.GetInstance().JoinPlayerInfoList.Find(x => { return x.playerId == ChijiDataManager.GetInstance().RecentKillPlayerID; });

                    if (Deadinfo != null)
                    {
                        mKillInfo.text = string.Format("你 淘汰了 {0}", Deadinfo.playerName);
                    }
                }

                mKillInfo.CustomActive(true);
            }

            if(mKills != null)
            {
                mKills.text = ChijiDataManager.GetInstance().KillNum.ToString();
            }
        }

        private void _UpdateShowDeathPlayerInfo()
        {
            if(mDeathPlayer0 != null)
            {
                if(ChijiDataManager.GetInstance().ShowDeathPlayerList.Count >= 1)
                {
                    if (mDeathplayer0Bg != null)
                    {
                        mDeathplayer0Bg.CustomActive(true);
                    }

                    mDeathPlayer0.text = _ShowDeathInfo(ChijiDataManager.GetInstance().ShowDeathPlayerList[0]);
                }
            }

            if(mDeathPlayer1 != null)
            {
                if (ChijiDataManager.GetInstance().ShowDeathPlayerList.Count >= 2)
                {
                    if (mDeathplayer1Bg != null)
                    {
                        mDeathplayer1Bg.CustomActive(true);
                    }

                    mDeathPlayer1.text = _ShowDeathInfo(ChijiDataManager.GetInstance().ShowDeathPlayerList[1]);
                }
            }

            if(mDeathPlayer2 != null)
            {
                if (ChijiDataManager.GetInstance().ShowDeathPlayerList.Count >= 3)
                {
                    if (mDeathplayer2Bg != null)
                    {
                        mDeathplayer2Bg.CustomActive(true);
                    }

                    mDeathPlayer2.text = _ShowDeathInfo(ChijiDataManager.GetInstance().ShowDeathPlayerList[2]);
                }       
            }
        }

        private string _ShowDeathInfo(PlayerDeathReason data)
        {
            string content = "";

            if(data == null)
            {
                return content;
            }

            if(data.playerID == PlayerBaseData.GetInstance().RoleID)
            {
                if(data.killerID == 0)
                {
                    content = string.Format("<color=#FF0025FF>你 在安全区外阵亡</color>");
                }
                else
                {
                    JoinPlayerInfo killerinfo = ChijiDataManager.GetInstance().JoinPlayerInfoList.Find( x => { return x.playerId == data.killerID; });

                    if(killerinfo != null)
                    {
                        content = string.Format("<color=#FF0025FF>{0} 淘汰了 你</color>", killerinfo.playerName);
                    }
                }
            }
            else
            {
                JoinPlayerInfo Deadinfo = ChijiDataManager.GetInstance().JoinPlayerInfoList.Find(x => { return x.playerId == data.playerID; });

                if (data.killerID == PlayerBaseData.GetInstance().RoleID)
                {
                    if (Deadinfo != null)
                    {
                        content = string.Format("<color=#3F9AFFFF>你 淘汰了 {0}</color>", Deadinfo.playerName);
                    }
                }
                else
                {
                    if(data.killerID != 0)
                    {
                        JoinPlayerInfo killerinfo = ChijiDataManager.GetInstance().JoinPlayerInfoList.Find(x => { return x.playerId == data.killerID; });

                        if(killerinfo != null)
                        {
                            content = string.Format("<color=#FFFFFFFF>{0} 淘汰了 {1}</color>", killerinfo.playerName, Deadinfo.playerName);
                        }
                    }
                    else
                    {
                        if (Deadinfo != null)
                        {
                            content = string.Format("<color=#FFFFFFFF>{0} 在安全区外阵亡</color>",  Deadinfo.playerName);
                        }
                    }
                }
            }

            return content;
        }

        private void _OnLeaveChiji()
        {
            ClientSystemGameBattle systemChiji = ClientSystemManager.GetInstance().GetCurrentSystem() as ClientSystemGameBattle;
            if (systemChiji == null)
            {
                Logger.LogError("Current System is not Game Battle!!![_OnLeaveChiji]");
                return;
            }

            GameFrameWork.instance.StartCoroutine(systemChiji._NetSyncChangeScene(
                        new SceneParams
                        {
                            currSceneID = systemChiji.CurrentSceneID,
                            currDoorID = 0,
                            targetSceneID = 10101,
                            targetDoorID = 0,
                        }));

            ChijiDataManager.GetInstance().SwitchingChijiSceneToPrepare = true;

            frameMgr.CloseFrame(this);
        }

        #region 小地图相关
        private void _ClearChijiMap()
        {
            frameMgr.CloseFrame<ChijiMapFrame>();
            m_mapFrame = null;
        }

        private void _InitChijiMap()
        {
            m_mapFrame = frameMgr.OpenFrame<ChijiMapFrame>(mMapContent) as ChijiMapFrame;

            m_mapFrame.SetScale(new Vector2(0.8f, 0.8f));
            mMapContent.GetComponent<RectTransform>().sizeDelta = m_mapFrame.GetSize();

            _UpdateChijiMap();
        }

        private void _UpdateChijiMap()
        {
            // 更新小地图当前显示区域
            if (m_mapFrame != null && mMapscrollRect != null)
            {
                Vector2 pos = m_mapFrame.GetPlayerMainPos();
                Vector2 contentSize = m_mapFrame.GetSize();
                Vector2 viewportSize = mMapscrollRect.viewport.rect.size;

                mMapscrollRect.normalizedPosition = new Vector2(
                    (pos.x - viewportSize.x * 0.5f) / (contentSize.x - viewportSize.x),
                    (pos.y - viewportSize.y * 0.5f) / (contentSize.y - viewportSize.y)
                    );
            }
        }
        #endregion

        #region 毒圈相关
        private void _InitPoisonData()
        {
            center = ChijiDataManager.GetInstance().PoisonRing.center;
            radius = ChijiDataManager.GetInstance().PoisonRing.radius;
            durTime = ChijiDataManager.GetInstance().PoisonRing.durTime;
            shrinkTime = ChijiDataManager.GetInstance().PoisonRing.shrinkTime;
            lastCenter = ChijiDataManager.GetInstance().PoisonRing.lastCenter;
            lastRadius = ChijiDataManager.GetInstance().PoisonRing.lastRadius;
            var nextStageCenter = ChijiDataManager.GetInstance().PoisonRing.nextStageCenter;
            var nextStageRadius = ChijiDataManager.GetInstance().PoisonRing.nextStageRadius;
            if (m_mapFrame != null)
            {
                m_mapFrame.ResetSourceCircle(lastCenter, lastRadius);
                m_mapFrame.SetWhiteCircle(center, radius);
                m_mapFrame.SetBlueCircle(center, radius, durTime, shrinkTime);
            }
        }

        public void ShowPoisionEffect(bool isShow)
        {
            if (null != mDeadTips)
            {
                mDeadTips.CustomActive(isShow);
            }
        }
        #endregion

        #region 摇杆相关
        void InitJoystick()
        {
            _inputManager = new InputManager();
            _inputManager.LoadJoystick(SettingManager.GetInstance().GetJoystickMode());
            GameObject joyStick = _inputManager.GetJoyStick();
            if (joyStick != null)
            {
                Utility.AttachTo(joyStick, GameClient.ClientSystemManager.instance.BottomLayer);
                joyStick.transform.SetAsFirstSibling();
            }
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
            if (cursystem == null || cursystem.MainPlayer == null || ChijiDataManager.GetInstance().IsMainPlayerDead)
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

            BeBaseActorData actor = cursystem.MainPlayer.ActorData;
            if(actor != null)
            {
                ActorMoveData move = actor.MoveData;

                if (move.TargetDirection != dirVec3)
                {
                    cursystem.MainPlayer.CommandMoveForward(dirVec3);
                }
            }
        }

        void _UnloadInput()
        {
            if (_inputManager != null)
            {
                _inputManager.Unload();
                _inputManager = null;
            }
        }
        #endregion

        #region 刷新相关
        public override bool IsNeedUpdate()
        {
            return true;
        }

        protected override void _OnUpdate(float timeElapsed)
        {
            durTime += timeElapsed;

            // 摇杆
            if (_inputManager != null)
            {
                _inputManager.SingleUpdate(0);
            }

            // 显示倒计时
            if (ChijiDataManager.GetInstance().CurBattleStage < ChiJiTimeTable.eBattleStage.BS_PUT_ITEM_1)
            {
                if (LeftTime >= 0)
                {
                    fTimeIntrval += timeElapsed;

                    if (fTimeIntrval >= 0.4f)
                    {
                        fTimeIntrval = 0.0f;

                        uint PassedTime = TimeManager.GetInstance().GetServerTime() - StartServerTime;

                        LeftTime = LastTime - (int)PassedTime;

                        if (mTime != null)
                        {
                            mTime.text = mStagename + Function.GetLeftTime((int)StartServerTime + LastTime, (int)TimeManager.GetInstance().GetServerTime(), ShowtimeType.OnlineGift);
                        }
                    }
                }
            }

            // 显示捡取道具进度条
            if(bRefreshPickUpState)
            {
                if(mPickUpProcess != null)
                {
                    if (bPickUpFinish)
                    {
                        if (fLocalTime - DelayHideTime >= 0.2f)
                        {
                            bRefreshPickUpState = false;
                            bPickUpFinish = false;
                            mPickUpProcess.CustomActive(false);

                            fLocalTime = 0.0f;
                            DelayHideTime = 0.0f;

                            ClientSystemManager.GetInstance().CloseFrame<ChijiSkillChooseFrame>();

                            if (picktype == PickUpType.PickUpSkill)
                            {
                                ClientSystemManager.GetInstance().OpenFrame<ChijiSkillChooseFrame>(FrameLayer.Middle, skillList);
                            }
                            else if(picktype == PickUpType.PickUpItem)
                            {
                                ClientSystemManager.GetInstance().OpenFrame<ChijiSkillChooseFrame>(FrameLayer.Middle, equipList);
                            }
                            else
                            {
                                bWaitPickUpMsg = true;
                            }

                            picktype = PickUpType.None;
                        }
                    }
                    else
                    {
                        mPickUpProcess.value = fLocalTime / PickUpItemLastTime;
                    }
                }

                fLocalTime += (timeElapsed * 1.35f); // 这个速度是按1秒的进度计算的，后面如果不同道具的时间不同，那么时间越长的速度要越慢
            }

            // 击杀信息显示一定时间后关闭
            if (ChijiDataManager.GetInstance().RecentKillPlayerID != 0)
            {
                KillInfoShowTimeIntrval += timeElapsed;

                if (KillInfoShowTimeIntrval >= 4.0f)
                {
                    mKillInfo.CustomActive(false);

                    KillInfoShowTimeIntrval = 0.0f;
                    ChijiDataManager.GetInstance().RecentKillPlayerID = 0;
                }
            }

            // 延迟显示结算界面
            if (ChijiDataManager.GetInstance().GuardForSettlement)
            {
                SettlementTime += timeElapsed;

                if (SettlementTime >= 0.4f)
                {
                    SettlementTime = 0.0f;

                    if (TimeManager.GetInstance().GetServerTime() - StartServerTime >= 2.4f)
                    {
                        ChijiDataManager.GetInstance().OpenSettlementFrame();

                        ChijiDataManager.GetInstance().GuardForSettlement = false;
                    }
                }
            }

            _UpdateChijiMap();
        }
        #endregion

        #region ExtraUIBind
        private Button mBtClose = null;
        private Button mBtBag = null;
        private Button mBtSkill = null;
        private Text mTime = null;
        private ScrollRect mMapscrollRect = null;
        private GameObject mMapContent = null;
        private Button mBtMap = null;
        private Text mSurvivePlayerNum = null;
        private GameObject mPlayerHeadPortraitInfoRoot = null;
        private Text mDeathPlayer0 = null;
        private Text mDeathPlayer1 = null;
        private Text mDeathPlayer2 = null;
        private Text mKillInfo = null;
        private Text mKills = null;
        private Text mShowKill = null;
        private Button mBtPk = null;
        private Text mPkText = null;
        private Slider mPickUpProcess = null;
        private GameObject mChallengeTipRoot = null;
        private Button mBtReport = null;
        private Button mShopButton = null;
        private GameObject mShopViewRoot = null;
        private GameObject mDeathplayer2Bg = null;
        private GameObject mDeathplayer1Bg = null;
        private GameObject mDeathplayer0Bg = null;

        protected override void _bindExUI()
        {
            mBtClose = mBind.GetCom<Button>("btClose");
            if (null != mBtClose)
            {
                mBtClose.onClick.AddListener(_onBtCloseButtonClick);
            }
            mBtBag = mBind.GetCom<Button>("btBag");
            if (null != mBtBag)
            {
                mBtBag.onClick.AddListener(_onBtBagButtonClick);
            }
            mBtSkill = mBind.GetCom<Button>("btSkill");
            if (null != mBtSkill)
            {
                mBtSkill.onClick.AddListener(_onBtSkillButtonClick);
            }
            mTime = mBind.GetCom<Text>("Time");
            mMapscrollRect = mBind.GetCom<ScrollRect>("MapscrollRect");
            mMapContent = mBind.GetGameObject("MapContent");
            mBtMap = mBind.GetCom<Button>("btMap");
            if (null != mBtMap)
            {
                mBtMap.onClick.AddListener(_onBtMapButtonClick);
            }
            mSurvivePlayerNum = mBind.GetCom<Text>("SurvivePlayerNum");
            mPlayerHeadPortraitInfoRoot = mBind.GetGameObject("HeadRoot");
            mDeadTips = mBind.GetGameObject("Canxue");
            mDeathPlayer0 = mBind.GetCom<Text>("DeathPlayer0");
            mDeathPlayer1 = mBind.GetCom<Text>("DeathPlayer1");
            mDeathPlayer2 = mBind.GetCom<Text>("DeathPlayer2");
            mKillInfo = mBind.GetCom<Text>("KillInfo");
            mKills = mBind.GetCom<Text>("kills");
            mShowKill = mBind.GetCom<Text>("ShowKill");
            mBtPk = mBind.GetCom<Button>("btPk");
            if (null != mBtPk)
            {
                mBtPk.onClick.AddListener(_onBtPkButtonClick);
            }
            mPkText = mBind.GetCom<Text>("pkText");
            mPickUpProcess = mBind.GetCom<Slider>("PickUpProcess");
            mChallengeTipRoot = mBind.GetGameObject("ChallengeTipRoot");
            mBtReport = mBind.GetCom<Button>("btReport");
            mBtReport.onClick.AddListener(_onBtReportButtonClick);
            mShopButton = mBind.GetCom<Button>("shopButton");
            if (mShopButton != null)
            {
                mShopButton.onClick.AddListener(OnShopButtonClicked);
            }

            mShopViewRoot = mBind.GetGameObject("shopViewRoot");

            mDeathplayer2Bg = mBind.GetGameObject("Deathplayer2Bg");
            mDeathplayer1Bg = mBind.GetGameObject("Deathplayer1Bg");
            mDeathplayer0Bg = mBind.GetGameObject("Deathplayer0Bg");

        }


        protected override void _unbindExUI()
        {
            if (null != mBtClose)
            {
                mBtClose.onClick.RemoveListener(_onBtCloseButtonClick);
            }
            mBtClose = null;
            if (null != mBtBag)
            {
                mBtBag.onClick.RemoveListener(_onBtBagButtonClick);
            }
            mBtBag = null;
            if (null != mBtSkill)
            {
                mBtSkill.onClick.RemoveListener(_onBtSkillButtonClick);
            }
            mBtSkill = null;
            mTime = null;
            mMapscrollRect = null;
            mMapContent = null;
            if (null != mBtMap)
            {
                mBtMap.onClick.RemoveListener(_onBtMapButtonClick);
            }
            mBtMap = null;
            mSurvivePlayerNum = null;
            mPlayerHeadPortraitInfoRoot = null;
            mDeadTips = null;
            mDeathPlayer0 = null;
            mDeathPlayer1 = null;
            mDeathPlayer2 = null;
            mKillInfo = null;
            mKills = null;
            mShowKill = null;
            if (null != mBtPk)
            {
                mBtPk.onClick.RemoveListener(_onBtPkButtonClick);
            }
            mBtPk = null;
            mPkText = null;
            mPickUpProcess = null;
            mChallengeTipRoot = null;
            mBtReport.onClick.RemoveListener(_onBtReportButtonClick);
            mBtReport = null;

            if (mShopButton != null)
            {
                mShopButton.onClick.RemoveListener(OnShopButtonClicked);
                mShopButton = null;
            }

            mShopViewRoot = null;
            mDeathplayer2Bg = null;
            mDeathplayer1Bg = null;
            mDeathplayer0Bg = null;

        }
        #endregion

        #region Callback
        private void _onBtCloseButtonClick()
        {
            SystemNotifyManager.SystemNotify(4200007, _OnLeaveChiji);
        }

        private void _onBtBagButtonClick()
        {
            ClientSystemManager.GetInstance().OpenFrame<PackageNewFrame>(FrameLayer.Middle);
        }

        private void _onBtSkillButtonClick()
        {
            ClientSystemManager.GetInstance().OpenFrame<ChijiSkillFrame>(FrameLayer.Middle);
        }

        private void _onBtMapButtonClick()
        {
           var frame = ClientSystemManager.GetInstance().OpenFrame<ChijiFullMapFrame>(FrameLayer.Middle) as ChijiFullMapFrame;
            if(frame != null && frame.MapFrame != null)
            {
                frame.MapFrame.ResetSourceCircle(lastCenter, lastRadius);
                frame.MapFrame.SetWhiteCircle(center, radius);
                frame.MapFrame.SetBlueCircle(center, radius, durTime, shrinkTime);
            }
        }

        private void _onBtPkButtonClick()
        {
            ChijiDataManager.GetInstance().IsReadyPk = !ChijiDataManager.GetInstance().IsReadyPk;
            _OnPKButtonChange(null);

        }

        void _OnPKButtonChange(UIEvent iEvent)
        {
            // 取消选择技能操作
            ClientSystemGameBattle systemChiji = ClientSystemManager.GetInstance().GetCurrentSystem() as ClientSystemGameBattle;
            //if (ChijiDataManager.GetInstance().IsReadyPk && systemChiji != null && systemChiji.MainPlayer != null && systemChiji.MainPlayer.Skill != null)
            {
                //var skill = systemChiji.MainPlayer.Skill;
                //if (skill.SkillState == BeFightSkill.eSkillState.Operation)
                {
                    //skill.CancelSkill();
                }
            }
            
            if (ChijiDataManager.GetInstance().IsReadyPk)
            {
                mPkText.text = "取消挑战";

                if (mChallengeTipRoot != null)
                {
                    mChallengeTipRoot.CustomActive(true);
                }

                if (systemChiji != null && systemChiji.MainPlayer != null)
                {
                    if(mAttackAreaEffect == null)
                    {
                        mAttackAreaEffect = systemChiji.MainPlayer.CreateAttackAreaEffect();
                        mAttackAreaEffect.SetScale(1.5f);
                    }
                    else
                    {
                        mAttackAreaEffect.SetVisible(true);
                    }
                }
            }
            else
            {
                mPkText.text = "挑战";

                if (mChallengeTipRoot != null)
                {
                    mChallengeTipRoot.CustomActive(false);
                }

                if(mAttackAreaEffect != null)
                {
                    mAttackAreaEffect.SetVisible(false);
                }
            }
        }

        private void _onBtReportButtonClick()
        {
            ClientSystemManager.GetInstance().OpenFrame<ChijiBugReportFrame>();
        }
        #endregion

        #region ChijiShopView

        private ChijiShopView _chijiShopView;

        private void OnShopButtonClicked()
        {
            if (_chijiShopView == null)
            {
                var chijiShopViewPrefab = CommonUtility.LoadGameObject(mShopViewRoot);
                if (chijiShopViewPrefab != null)
                    _chijiShopView = chijiShopViewPrefab.GetComponent<ChijiShopView>();

                if(_chijiShopView != null)
                    _chijiShopView.InitShopView();
            }
            else
            {
                CommonUtility.UpdateGameObjectVisible(_chijiShopView.gameObject, true);
                _chijiShopView.OnEnableShopView();
            }
        }

        #endregion 

    }
}
