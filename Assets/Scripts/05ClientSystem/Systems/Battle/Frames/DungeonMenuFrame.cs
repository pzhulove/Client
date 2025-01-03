using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using UnityEngine.UI;
using Protocol;
using Network;
using System.Diagnostics;
using System;
using UnityEngine.Events;
using ProtoTable;

namespace GameClient
{
    enum ELeaderOper
    {
        Again,
        Next,
        Exit,
    }

    public class DungeonMenuFrame : ClientFrame
    {
        enum eDungeonMenu
        {
            None = 0,
            Again,
            Next,
            ChapterSelect,
            BackTown,
        }

        private eDungeonMenu mDungeonState = eDungeonMenu.None;
        private bool returned = false;
        private int mysticalMerchantId = 0;
        const string dungeonMenuPlayerItemPath = "UIFlatten/Prefabs/BattleUI/DungeonMenuPlayerItem";

        #region ExtraUIBind
        private ComCountScript mCounter = null;
		private GameObject mFriendAddRoot = null;
		private Button mOnAgain = null;
		private Button mOnBack = null;
		private GameObject mMissionRoot = null;
		private Text mMissionInfo = null;
		private Text mMissionDesc = null;
		private GameObject mAgain = null;
		private Button mNpcTalkBtn = null;
		private Button mBuinessmanBtn = null;
		private Image mBuinessmanImage = null;
		private Button mMissionBack = null;
		private GameObject mPlayerInformationItemRoot = null;
		private GameObject mPlayerInformation = null;
        [UIObject("r/ButtonRoot/Root/BtnRoot2/Reporter")]
        GameObject m_reportBtn;
        [UIEventHandle("r/ButtonRoot/Root/BtnRoot2/Reporter")]
        void _OnOpenReporter()
        {
#if MG_TEST && !LOGIC_SERVER
            if (RecordServer.instance != null)
            {
                RecordServer.instance.EndRecord("openReporter");
            }
            ClientSystemManager.GetInstance().OpenFrame<PKReporterFrame>(FrameLayer.Middle);
            if(mCounter != null)
            {
                mCounter.PauseCount();
            }
#endif
        }
        protected override void _bindExUI()
		{
			mCounter = mBind.GetCom<ComCountScript>("counter");
			mFriendAddRoot = mBind.GetGameObject("friendAddRoot");
			mOnAgain = mBind.GetCom<Button>("onAgain");
			if (null != mOnAgain)
			{
				mOnAgain.onClick.AddListener(_onOnAgainButtonClick);
			}
			mOnBack = mBind.GetCom<Button>("onBack");
			if (null != mOnBack)
			{
				mOnBack.onClick.AddListener(_onOnBackButtonClick);
			}
            mMissionRoot = mBind.GetGameObject("missionRoot");
			mMissionInfo = mBind.GetCom<Text>("missionInfo");
			mMissionDesc = mBind.GetCom<Text>("missionDesc");
			mAgain = mBind.GetGameObject("Again");
			mNpcTalkBtn = mBind.GetCom<Button>("NpcTalkBtn");
			if (null != mNpcTalkBtn)
			{
				mNpcTalkBtn.onClick.AddListener(_onNpcTalkBtnButtonClick);
			}
			mBuinessmanBtn = mBind.GetCom<Button>("BuinessmanBtn");
			if (null != mBuinessmanBtn)
			{
				mBuinessmanBtn.onClick.AddListener(_onBuinessmanBtnButtonClick);
			}
			mBuinessmanImage = mBind.GetCom<Image>("BuinessmanImage");
			mMissionBack = mBind.GetCom<Button>("missionBack");
			if (null != mMissionBack)
			{
				mMissionBack.onClick.AddListener(_onMissionBackButtonClick);
			}
			mPlayerInformationItemRoot = mBind.GetGameObject("PlayerInformationItemRoot");
			mPlayerInformation = mBind.GetGameObject("PlayerInformation");
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnUploadFileSucc, _OnUpLoadFileSucc);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnUploadFileClose, _OnUpLoadFileClose);
        }
        protected void _OnUpLoadFileSucc(UIEvent a_event)
        {
#if MG_TEST
            if (m_reportBtn != null)
            {
                m_reportBtn.CustomActive(false);
            }
             if(mCounter != null)
            {
                mCounter.ResumeCount();
            }
#endif
        }
        protected void _OnUpLoadFileClose(UIEvent a_event)
        {
#if MG_TEST
             if(mCounter != null)
            {
                mCounter.ResumeCount();
            }
#endif
        }
        protected override void _unbindExUI()
		{
			mCounter = null;
			mFriendAddRoot = null;
			if (null != mOnAgain)
			{
				mOnAgain.onClick.RemoveListener(_onOnAgainButtonClick);
			}
			mOnAgain = null;
			if (null != mOnBack)
			{
				mOnBack.onClick.RemoveListener(_onOnBackButtonClick);
			}
			mOnBack = null;
			mMissionRoot = null;
			mMissionInfo = null;
			mMissionDesc = null;
			mAgain = null;
			if (null != mNpcTalkBtn)
			{
				mNpcTalkBtn.onClick.RemoveListener(_onNpcTalkBtnButtonClick);
			}
			mNpcTalkBtn = null;
			if (null != mBuinessmanBtn)
			{
				mBuinessmanBtn.onClick.RemoveListener(_onBuinessmanBtnButtonClick);
			}
			mBuinessmanBtn = null;
			mBuinessmanImage = null;
			if (null != mMissionBack)
			{
				mMissionBack.onClick.RemoveListener(_onMissionBackButtonClick);
			}
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnUploadFileSucc, _OnUpLoadFileSucc);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnUploadFileClose, _OnUpLoadFileClose);
            mMissionBack = null;
			mPlayerInformationItemRoot = null;
			mPlayerInformation = null;
		}
		#endregion

        #region Callback
        private void _onOnAgainButtonClick()
        {
            /* put your code in here */

            if (mCounter.mLeftTime < 1.2f)
            {
                return;
            }

            _sendSceneDungeonStartRep();
            GameStatisticManager.GetInstance().DoStartSingleBoardDoAgainButton("OnAgain");
        }


        private void _onOnBackButtonClick()
        {
            /* put your code in here */
            _ReturnToTown();
            GameStatisticManager.GetInstance().DoStartUIButton("BattleOnBack");
        }

        private void _onMissionBackButtonClick()
        {
            /* put your code in here */

            int id = BattleMain.instance.GetDungeonManager().GetDungeonDataManager().id.dungeonID;

            if (MissionManager.GetInstance().IsMainTaskDungeon(id))
            {
				int missionId = MissionManager.GetInstance().GetMainTaskMainMission(id);

                MissionManager.SingleMissionInfo value = null;
                MissionManager.GetInstance().taskGroup.TryGetValue((uint)missionId, out value);

                if (value != null)
                {
                    ClientSystemManager.instance.Push2FrameStack(new MissionTraceTargetCmd(missionId));
                }
            }

            _ReturnToTown();
        }
#endregion

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/BattleUI/DungeonMenu";
        }

        protected override bool _isLoadFromPool()
        {
            return true;
        }

        private void _onNpcTalkBtnButtonClick()
        {
            mNpcTalkBtn.CustomActive(false);
            GotoBuinessmanShop();
        }
        private void _onBuinessmanBtnButtonClick()
        {
            GotoBuinessmanShop();
        }

        protected override void _OnOpenFrame()
        {
            mDungeonState = eDungeonMenu.None;
            returned = false;
            mIsSendMessage = false;
            //mysticalMerchantId = 1;
            mysticalMerchantId = ShopDataManager.GetInstance().MysticalMerchantID;
            if (m_reportBtn != null)
            {
#if !MG_TEST
                m_reportBtn.CustomActive(false);
#else
                m_reportBtn.CustomActive(true);
#endif
            }
#if ROBOT_TEST
            mysticalMerchantId = -1;
#endif
            if (mysticalMerchantId == -1)
            {
                //修改团队副本倒计时
                //int /*count*/ = GetCountNum(mCounter.mLeftTime);
                int count = mCounter.mLeftTime;

                mCounter.StartCount(() =>
                {
                    if (mMissionRoot.activeSelf)
                    {
                        if (NewbieGuideManager.GetInstance().GetCurTaskID() == ProtoTable.NewbieGuideTable.eNewbieGuideTask.ReturnToTownGuide)
                        {
                            NewbieGuideManager.GetInstance().ManagerFinishGuide(NewbieGuideManager.GetInstance().GetCurTaskID());
                        }

                        _onMissionBackButtonClick();
                    }
                    else
                    {
                        _ReturnToTown();
                    }
                }, count);
            }
            else if (BattleMain.instance.GetDungeonManager().GetDungeonDataManager().table.SubType != DungeonTable.eSubType.S_RAID_DUNGEON) 
            {
                ShowBattleNpc();
            }

            _loadFriend();

            _updateMainMissionInfo();

            _updatePlayerAgain();

            _InitPlayerInformation();

            if(BattleMain.instance != null &&                
                BattleMain.instance.GetDungeonManager() != null && 
                BattleMain.instance.GetDungeonManager().GetDungeonDataManager() != null &&
                BattleMain.instance.GetDungeonManager().GetDungeonDataManager().id != null)           
            {
                int id = BattleMain.instance.GetDungeonManager().GetDungeonDataManager().id.dungeonID;
                if (GuildDataManager.GetInstance().IsGuildDungeonMap(id))
                {
                    mOnAgain.CustomActive(false);
				}
            }
            
            int dungeonID = BattleMain.instance.GetDungeonManager().GetDungeonDataManager().id.dungeonID;
            if (ChallengeUtility.isYunShangChangAn(dungeonID))
            {
                mOnAgain.CustomActive(false);
            }
        }

        protected override void _OnUpdate(float timeElapsed)
        {
            if (mysticalMerchantId != -1)
            {
                CheckDistance();
                UpdateTalkBtnPos(timeElapsed);
            }
        }

        public override bool IsNeedUpdate()
        {
            return true;
        }

        private void AddDialog(int id, UnityAction action)
        {
            var task = new TaskDialogFrame.OnDialogOver();
            if (action != null)
            {
                task.AddListener(action);
            }
            MissionManager.GetInstance().CreateDialogFrame(id, 0, task);
            DeviceVibrateManager.GetInstance().TriggerDeviceVibrateByType(VibrateSwitchType.MysteryShop);
        }

        private void _updatePlayerAgain()
        {
            bool isShow = _isShowAgainButton();
            mAgain.CustomActive(isShow);

            if (isShow && !_isCanAgainEnterDungeon())
            {
                mAgain.SafeAddComponent<UIGray>();
            }
        }

        private void _InitPlayerInformation()
        {
            if (BattleMain.instance == null)
                return;
            if (BattleMain.instance.GetPlayerManager() == null)
                return;
            List<BattlePlayer> playerList = BattleMain.instance.GetPlayerManager().GetAllPlayers();

            if (playerList.Count > 3)
            {
                Logger.LogErrorFormat("战斗结算伤害统计界面显示角色个数异常：playerList count = {0}", playerList.Count);
            } 

            List<BattlePlayer> list = new List<BattlePlayer>();
            for(int i = 0;i<playerList.Count;i++)
            {
                list.Add(playerList[i]);
            }
            list.Sort(SortList);
            long maxScore = 0;
            long totalScore = 0;
            int highestIndex = -1;
            for (int i = 0; i < list.Count; i++)
            {
                string name = list[i].playerInfo.name;
                BeEntity entity = list[i].playerActor.GetTopOwner(list[i].playerActor);
                //BeEntity entity = list[i].playerActor.GetOwner();
                if(entity == null)
                {
                    continue;
                }
                long damage = entity.GetEntityData().battleData.GetTotalDamage();
                if (damage > maxScore)
                {
                    maxScore = damage;
                    highestIndex = i;
                }
                totalScore += damage;
            }

            for (int i = 0; i < list.Count; i++)
            {
                bool isHighestScore = false;
                if(i == highestIndex && list.Count > 1)
                {
                    isHighestScore = true;
                }
                else
                {
                    isHighestScore = false;
                }
                GameObject dungeonMenuPlayerItemGO = AssetLoader.instance.LoadResAsGameObject(dungeonMenuPlayerItemPath);
                var dungeonMenuPlayerItem = dungeonMenuPlayerItemGO.GetComponent<DungeonMenuPlayerItem>();
                if (dungeonMenuPlayerItem != null)
                {
                    dungeonMenuPlayerItem.InitPlayerItem(list[i], maxScore, totalScore, isHighestScore);
                    Utility.AttachTo(dungeonMenuPlayerItemGO, mPlayerInformationItemRoot);
                }
            }
        }

        private int SortList(BattlePlayer a, BattlePlayer b)
        {
            if (a == null || b == null)
                return 0;
            if (a.playerActor == null || b.playerActor == null)
                return 0;
            if (a.playerActor.GetEntityData() == null || b.playerActor.GetEntityData() == null)
                return 0;
            if (a.playerActor.GetEntityData().battleData == null || b.playerActor.GetEntityData().battleData == null)
                return 0;
            long aDamage = a.playerActor.GetEntityData().battleData.GetTotalDamage();
            long bDamage = b.playerActor.GetEntityData().battleData.GetTotalDamage();
            if(aDamage > bDamage)
            {
                return -1;
            }
            else if(aDamage < bDamage)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
        private bool _isCanAgainEnterDungeon()
        {
            int dungeonID = BattleMain.instance.GetDungeonManager().GetDungeonDataManager().id.dungeonID;
            dungeonID = ChapterUtility.GetOriginDungeonId(dungeonID);

            if (!ChapterUtility.GetDungeonCanEnter(dungeonID, false, true, false))
            {
                return false;
            }

            if (!ChapterUtility.IsCanComsumeFatigue(dungeonID))
            {
                return false;
            }
            if (!DungeonUtility.IsWeekHellDungeonCanAgain(dungeonID)) return false;
            return true;
        }

        private bool _isShowAgainButton()
        {
            if (null == BattleMain.instance)
            {
                return false;
            }

            /// 剧情关卡
            DungeonID id = BattleMain.instance.GetDungeonManager().GetDungeonDataManager().id;
            if (id.prestoryID > 0)
            {
                return false;
            }

            if (id.dungeonIDWithOutDiff == 720000)
            {
                return false;
            }
            if (id.dungeonIDWithOutDiff == 600000)
            {
                return false;//打完远古引导副本，打完结算后，隐藏按钮
            }
            if (BattleMain.instance.GetDungeonManager().GetDungeonDataManager().table.SubType == DungeonTable.eSubType.S_CITYMONSTER ||
                 BattleMain.instance.GetDungeonManager().GetDungeonDataManager().table.SubType == DungeonTable.eSubType.S_GUILD_DUNGEON ||
                 BattleMain.instance.GetDungeonManager().GetDungeonDataManager().table.SubType == DungeonTable.eSubType.S_RAID_DUNGEON ||
                 BattleMain.instance.GetDungeonManager().GetDungeonDataManager().table.SubType == DungeonTable.eSubType.S_TREASUREMAP) 
            {
                return false;
            }
            if (!DungeonUtility.IsWeekHellDungeonCanAgain(id.dungeonID)) return false;
            /// 非单机模式
            if (BattleMain.instance.GetBattle().GetMode() != eDungeonMode.LocalFrame)
            {
                return false;
            }

            return true;
        }


        protected override void _OnCloseFrame()
        {
        }

        private void _bindEvent()
        {
        }

        private void _unbindEvent()
        {
        }

        private bool _isMyFriend(ulong id)
        {
            RelationData relation = RelationDataManager.GetInstance().GetRelationByRoleID(id);
            return null != relation && (relation.IsFriend() || relation.IsMater() || relation.IsDisciple());
        }

        private void _loadFriend()
        {
            //团队副本不显示添加好友
            if (CheckRaidBattle())
                return;
            mFriendAddRoot.SetActive(false);

            string unit = mBind.GetPrefabPath("unit");

            mBind.ClearCacheBinds(unit);

            List<BattlePlayer> allPlayer = BattleMain.instance.GetPlayerManager().GetAllPlayers();
            BattlePlayer mainPlayer = BattleMain.instance.GetPlayerManager().GetMainPlayer();

            for (int i = 0 ; i < allPlayer.Count; ++i)
            {
                ulong roleID = allPlayer[i].playerInfo.roleId;

                Logger.LogProcessFormat("[地下城菜单] {0} 是否是好友 {1}", roleID, _isMyFriend(roleID));

                if (roleID != mainPlayer.playerInfo.roleId && !_isMyFriend(roleID))
                {
                    ComCommonBind bind = mBind.LoadExtraBind(unit);
                    if (null != bind)
                    {
                        mFriendAddRoot.SetActive(true);

                        Utility.AttachTo(bind.gameObject, mFriendAddRoot);

                        mFriendAddRoot.transform.rectTransform().sizeDelta = new Vector2(mFriendAddRoot.transform.rectTransform().sizeDelta.x, mFriendAddRoot.transform.rectTransform().sizeDelta.y + bind.rectTransform().rect.height);

                        Image icon = bind.GetCom<Image>("icon");
                        Text name = bind.GetCom<Text>("name");
                        Button onAdd = bind.GetCom<Button>("onAdd");
                        GameObject addRoot = bind.GetGameObject("addRoot");
                        GameObject hasAddRoot = bind.GetGameObject("hasAddRoot");

                        // icon.sprite = TeamUtility.GetSpriteByOccu(allPlayer[i].playerInfo.occupation);
                        TeamUtility.GetSpriteByOccu(ref icon, allPlayer[i].playerInfo.occupation);
                        name.text = allPlayer[i].playerInfo.name;

                        addRoot.SetActive(true);
                        hasAddRoot.SetActive(false);

                        onAdd.onClick.RemoveAllListeners();
                        onAdd.onClick.AddListener(()=>
                        {
                            RelationDataManager.GetInstance().AddFriendByID(roleID);
                            addRoot.SetActive(false);
                            hasAddRoot.SetActive(true);
                        });
                    }
                }
            }
        }

        void _ReturnToTown()
        {
            mDungeonState = eDungeonMenu.BackTown;

            GameStatisticManager.instance.DoStatInBattleEx(StatInBattleType.CLICK_RETURN,
                    BattleMain.instance.GetDungeonManager().GetDungeonDataManager().id.dungeonID,
                    BattleMain.instance.GetDungeonManager().GetDungeonDataManager().CurrentAreaID(),
                    "");

			if (returned)
				return;

			returned = true;

            ClientSystemManager.instance.SwitchSystem<ClientSystemTown>();
            ShopDataManager.GetInstance().MysticalMerchantID = -1;
        }

        private void _updateMainMissionInfo()
        {
            int id = BattleMain.instance.GetDungeonManager().GetDungeonDataManager().id.dungeonID;

            mMissionRoot.SetActive(false);

            if (MissionManager.GetInstance().IsMainTaskDungeon(id))
            {
				int missionId = MissionManager.GetInstance().GetMainTaskMainMission(id);

                MissionManager.SingleMissionInfo value;
                MissionManager.GetInstance().taskGroup.TryGetValue((uint)missionId, out value);

                if (value != null)
                {
                    mMissionRoot.SetActive(true);
                    mMissionInfo.text = MissionManager.GetInstance().GetMissionName((uint)missionId) + MissionManager.GetInstance().GetMissionNameAppendBystatus(value.status,value.missionItem.ID);

                    //ClientSystemManager.instance.Push2FrameStack(new MissionTraceTargetCmd(missionId));
                }
            }
        }

        private bool mIsSendMessage = false;

        private void _sendSceneDungeonStartRep()
        {
            int dungeonID = BattleMain.instance.GetDungeonManager().GetDungeonDataManager().id.dungeonID;
            dungeonID = ChapterUtility.GetOriginDungeonId(dungeonID);

            if (!ChapterUtility.IsCanComsumeFatigue(dungeonID))
            {
                return;
            }

            if (!ChapterUtility.GetDungeonCanEnter(dungeonID, true, true, false))
            {
                return;
            }

#if !MG_TEST_EXTENT
            NetManager.instance.ClearReSendData();
#endif

#if MG_TEST_EXTENT
            NetManager.instance.ResetResend();
#endif
            GameFrameWork.instance.StartCoroutine(_sendSceneDungeonStart(dungeonID));
            ShopDataManager.GetInstance().MysticalMerchantID = -1;
        }

        private IEnumerator _sendSceneDungeonStart(int dungeonID)
        {
            if (!mIsSendMessage)
            {
                mDungeonState = eDungeonMenu.Again;

                SceneDungeonStartReq req = new SceneDungeonStartReq();
                req.dungeonId = (uint)dungeonID;
                req.isRestart = 1;

                var msg = new MessageEvents();
                var res = new SceneDungeonStartRes();

                mIsSendMessage = true;

                yield return (MessageUtility.Wait<SceneDungeonStartReq, SceneDungeonStartRes>(ServerType.GATE_SERVER, msg, req, res, true, 5));

                mIsSendMessage = false;
            }
        }

#region 神秘商人Npc相关
        protected int borderDis = 2;                //与场景边缘的距离
        protected float showBtnDis = 5;             //显示交谈按钮的距离
        protected Vector3 npcPos = Vector3.zero;    //Npc的位置坐标
        protected BeActor localActor = null;        //本地玩家

        protected void ShowBattleNpc()
        {
            HideCounter();
            var mysticalMerchantData = TableManager.instance.GetTableItem<MysticalMerchantTable>(mysticalMerchantId);
            if (mysticalMerchantData == null)
                return;
            var npcData = TableManager.instance.GetTableItem<NpcTable>(mysticalMerchantData.ShopNpcID);
            if (npcData == null)
                return;
            SetTalkBtnImage(npcData);
            if (npcData.NpcTalk.Length > 0)
            {
                ShowDialog(int.Parse(npcData.NpcTalk[0]));
            }
            CreateNpc(npcData);
        }

        protected void SetTalkBtnImage(NpcTable npcData)
        {
            ETCImageLoader.LoadSprite(ref mBuinessmanImage, npcData.FunctionIcon);
        }

        protected void HideCounter()
        {
            mBuinessmanBtn.CustomActive(true);
            mCounter.CustomActive(false);
        }

        protected void ShowDialog(int talkId)
        {
            var dialog = TableManager.instance.GetTableItem<TalkTable>(talkId);
            if (dialog == null)
                return;
            ClientSystemManager.GetInstance().delayCaller.DelayCall(100, () =>
            {
                // 打开对话
                AddDialog(talkId, () =>
                {
                    GotoBuinessmanShop();
                    MysticalMerchantDungeonTypeTigger();
                });
            });
        }

        protected void CreateNpc(NpcTable npcData)
        {
            var mainPlayer = BattleMain.instance.GetPlayerManager().GetMainPlayer();
            var actor = mainPlayer.playerActor;
            GeActorEx geActor = actor.CurrentBeScene.currentGeScene.CreateActor(npcData.ResID, 0, 0, true, true, true, false);
            geActor.CreateInfoBar(npcData.NpcName, PlayerInfoColor.TOWN_NPC, 0);
            npcPos = GetNpcPosition(actor, actor.CurrentBeScene);
            localActor = actor;
            geActor.SetPosition(npcPos);
        }

        protected Vector3 GetNpcPosition(BeActor localActor,BeScene scene)
        {
            VInt3 pos = scene.GetPosInXAxis(localActor);
            return pos.vector3;
        }

        protected void CheckDistance()
        {
            if (localActor == null)
                return;
            Vector3 pos = localActor.GetPosition().vector3;
            bool showFlag = Vector3.Distance(pos, npcPos) <= showBtnDis;
            mNpcTalkBtn.CustomActive(showFlag);
        }
        
        protected void UpdateTalkBtnPos(float deltaTime)
        {
            if (!mNpcTalkBtn.gameObject.activeInHierarchy)
                return;
            Vector2 localPos = Vector3.zero;
            if (Camera.main == null)
                return;
            Vector3 screenPos= Camera.main.WorldToScreenPoint(npcPos);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(frame.transform as RectTransform, screenPos, ClientSystemManager.GetInstance().UICamera, out localPos);
            mNpcTalkBtn.transform.localPosition = localPos;
        }

        protected void GotoBuinessmanShop()
        {
            ShopDataManager.GetInstance().OpenMysteryShopFrame();
        }

        /// <summary>
        /// 添加每个类型的地下城触发神秘商人的次数埋点
        /// </summary>
        protected void MysticalMerchantDungeonTypeTigger()
        {
            int dungeonID = BattleMain.instance.GetDungeonManager().GetDungeonDataManager().id.dungeonID;
            var tab = TableManager.GetInstance().GetTableItem<DungeonTable>(dungeonID);
            if (tab == null)
            {
                return;
            }
            var mysticalMerchantData = TableManager.instance.GetTableItem<MysticalMerchantTable>(mysticalMerchantId);
            if (mysticalMerchantData == null)
            {
                return;
            }
            var shopData = TableManager.GetInstance().GetTableItem<ShopTable>(mysticalMerchantData.ShopId);
            if (shopData == null)
            {
                return;
            }
            string dungeonName = GameStatisticManager.GetInstance().DungeonName((GameStatisticManager.DungeonsType)(int)tab.SubType);
            string shopName = shopData.ShopName;

            GameStatisticManager.GetInstance().DoStartMysticalMerchantDungeon(dungeonName,shopName);
        }
#endregion

        /// <summary>
        /// 检查是否是团队副本
        /// </summary>
        /// <returns></returns>
        private bool CheckRaidBattle()
        {
            if (BattleMain.battleType == BattleType.RaidPVE)
                return true;
            return false;
        }

        /// <summary>
        /// 根据副本类型获取倒计时时间
        /// </summary>
        /// <returns></returns>
        private int GetCountNum(int originCount)
        {
            int count = originCount;
            switch (BattleMain.battleType)
            {
                case BattleType.RaidPVE:
                    count = 5;
                    break;
            }
            return count;
        }

    }
}

