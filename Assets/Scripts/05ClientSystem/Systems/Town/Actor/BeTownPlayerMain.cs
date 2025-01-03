using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using Network;
using Protocol;
using ProtoTable;

namespace GameClient
{

    class BeTownPlayerMain : BeTownPlayer
    {
        public enum EMoveState
        {
            Invalid = -1,
            Idle = 0,
            Moveing,
            AutoMoving,
        }

        static public MoveStateChangedEvent OnMoveStateChanged { get; set; }
        static public AutoMoveSuccessEvent OnAutoMoveSuccess { get; set; }
        static public AutoMoveFailEvent OnAutoMoveFail { get; set; }
        static public MoveingEvent OnMoveing { get; set; }
        static public AutoMoveingEvent OnAutoMoving { get; set; }
        public EMoveState MoveState { get; set; }

        bool m_bEnable = true;
        TaskGuideModelArrow comTaskGuideModelArrow = null;

        static UnityEngine.Coroutine ms_autoMoveCoroutine;

        public BeTownPlayerMain(BeTownPlayerData data, ClientSystemTown systemTown)
            :base(data, systemTown)
        {
            if (OnAutoMoveSuccess == null)
            {
                OnAutoMoveSuccess = new AutoMoveSuccessEvent();
            }
            
            if (OnMoveStateChanged == null)
            {
                OnMoveStateChanged = new MoveStateChangedEvent();
            }
            
            if (OnAutoMoveFail == null)
            {
                OnAutoMoveFail = new AutoMoveFailEvent();
            }
            
            if (OnMoveing == null)
            {
                OnMoveing = new MoveingEvent();
            }
            
            if (OnAutoMoving == null)
            {
                OnAutoMoving = new AutoMoveingEvent();
            }
            
            MoveState = EMoveState.Idle;
        }

        void _BeginTrace(Int32 iNpcID, Vector3 target)
        {
            if (comTaskGuideModelArrow != null)
            {
                comTaskGuideModelArrow.BeginTrace(iNpcID, target);
            }
        }

        void _EndTrace()
        {
            if(comTaskGuideModelArrow != null)
            {
                comTaskGuideModelArrow.EndTrace();
            }
        }

       

        public sealed override void InitGeActor(GeSceneEx geScene)
        {
            base.InitGeActor(geScene);

            if (_geActor != null)
            {
                bool IsIn3v3Room = false;

                ClientSystemTown systemTown = ClientSystemManager.GetInstance().GetCurrentSystem() as ClientSystemTown;
                if (systemTown != null)
                {
                    CitySceneTable scenedata = TableManager.GetInstance().GetTableItem<CitySceneTable>(systemTown.CurrentSceneID);
                    if (scenedata != null)
                    {
                        if (scenedata.SceneSubType == CitySceneTable.eSceneSubType.Pk3v3)
                        {
                            IsIn3v3Room = true;
                        }
                    }
                }

                if (IsIn3v3Room)
                {
                    RoomInfo roominfo = Pk3v3DataManager.GetInstance().GetRoomInfo();

                    if(roominfo == null || roominfo.roomSlotInfos == null)
                    {
                        //_geActor.CreateFootIndicator("Effects/Common/Sfx/Jiaodi/Prefab/Eff_jiaodidingwei_guo");
                        CreateFootIndicator();
                    }
                    else
                    {
                        bool bFind = false;

                        for (int i = 0; i < roominfo.roomSlotInfos.Length; i++)
                        {
                            if (roominfo.roomSlotInfos[i].playerId == ActorData.GUID)
                            {
                                if (roominfo.roomSlotInfos[i].group == (byte)RoomSlotGroup.ROOM_SLOT_GROUP_RED)
                                {
                                    CreateFootIndicator(FootEffectType.RED);
                                   // _geActor.CreateFootIndicator("Effects/Common/Sfx/Jiaodi/Prefab/Eff_jiaodidingwei_hong"); 
                                }
                                else
                                {
                                    CreateFootIndicator(FootEffectType.BULE);
                                    //_geActor.CreateFootIndicator("Effects/Common/Sfx/Jiaodi/Prefab/Eff_jiaodidingwei_lan");
                                }

                                bFind = true;

                                break;
                            }
                        }

                        if(!bFind)
                        {
                            //_geActor.CreateFootIndicator("Effects/Common/Sfx/Jiaodi/Prefab/Eff_jiaodidingwei_guo");
                            CreateFootIndicator();
                        }
                    }
                }
                else
                {
                    // _geActor.CreateFootIndicator("Effects/Common/Sfx/Jiaodi/Prefab/Eff_jiaodidingwei_guo");
                    CreateFootIndicator();
                }       

                // 已不用
                // GameObject goCharactor = _geActor.GetEntityNode(GeEntity.GeEntityNodeType.Charactor);
                // if(goCharactor != null)
                // {
                //     GameObject goParent = Utility.FindChild(goCharactor,"GuideModelArrow");
                //     if (goParent == null)
                //     {
				// 		goParent = AssetLoader.instance.LoadResAsGameObject("UIFlatten/Prefabs/BattleUI/GuideModelArrow");
                //         if(goParent != null)
                //         {
                //             Utility.AttachTo(goParent.gameObject, goCharactor);
                //             goParent.name = "GuideModelArrow";
                //             comTaskGuideModelArrow = goParent.gameObject.AddComponent<TaskGuideModelArrow>();
                //             if (comTaskGuideModelArrow != null)
                //             {
                //                 comTaskGuideModelArrow.Initialize(this);
                //             }
                //         }
                //     }
                // }

				UpdateEquip();
                UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.AvatarChanged, UpdateAvatar);
            }
        }
        
        public void CreatePropertyRiseEffect(string content)
        {
            _geActor.CreatePropertyRiseEffect(content);
        }

        private void UpdateAvatar(UIEvent data = null)
        {
            UpdateEquip();
        }
        
        public sealed override void Dispose()
        {
            base.Dispose();

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.AvatarChanged, UpdateAvatar);
        }

        public void UpdateEquip()
		{

            var avatar = PlayerBaseData.GetInstance().avatar;
            var jobID = PlayerBaseData.GetInstance().JobTableID;
            if (avatar != null)
            {
                PlayerBaseData.GetInstance().AvatarEquipFromItems(null,
                    avatar.equipItemIds,
                    jobID,
                    (int) (avatar.weaponStrengthen),
                    _geActor,
                    false,
                    avatar.isShoWeapon);
            }
            else
            {
                Logger.LogErrorFormat("avatar is null in BeTownPlayerMain UpdateEquip");
            }

            

            /*
			//武器替换
			{
				int wid = 0;
				int strengthenLevel = 0;
				ulong uwid = ItemDataManager.GetInstance().GetWearEquipBySlotType(EEquipWearSlotType.EquipWeapon);
				if (uwid > 0){
					ItemData itemData = ItemDataManager.GetInstance().GetItem(uwid);
                    if (itemData != null)
                    {
                        wid = (int)itemData.TableID;
                        strengthenLevel = (int)itemData.StrengthenLevel;
                    }
				}
				EquipWeapon(wid, strengthenLevel);
			}


			//时装
			for(int i=1; i<(int)EFashionWearSlotType.Max; ++i)
			{
				EFashionWearSlotType slotType = (EFashionWearSlotType)i;

				int wid = 0;
				ulong uwid = ItemDataManager.GetInstance().GetFashionWearEquipBySlotType(slotType);
				if (uwid > 0)
				{
					ItemData itemData = ItemDataManager.GetInstance().GetItem(uwid);
                    if(itemData != null)
					    wid = (int)itemData.TableID;
				}

				EquipFasion(slotType, wid>0, wid);
			}
            */
        }

        public void SetEnable(bool bEnable)
        {
            m_bEnable = bEnable;
        }

        #region MissionLink

        public void CreateMissionLink(UInt32 iMissionID)
        {
            var linkedDoorList = new List<Vector3>();
            var iNpcID = 0;

            MissionManager.SingleMissionInfo outValue = null;
            if (!MissionManager.GetInstance().taskGroup.TryGetValue(iMissionID, out outValue))
            {
                return;
            }

            var missionItem = TableManager.GetInstance().GetTableItem<ProtoTable.MissionTable>((Int32)iMissionID);
            if (missionItem == null)
            {
                return;
            }

            if (outValue.status == (int)Protocol.TaskStatus.TASK_INIT)
            {
                if (missionItem.AcceptType == ProtoTable.MissionTable.eAcceptType.ACT_NPC)
                {
                    ProtoTable.NpcTable table = TableManager.GetInstance().GetTableItem<ProtoTable.NpcTable>(missionItem.MissionTakeNpc);
                    if (table != null)
                    {
                        BeTownNPC townNpc = _systemTown.GetTownNpcByNpcId(missionItem.MissionTakeNpc);
                        if (townNpc != null)
                        {
                            linkedDoorList.Add(townNpc.ActorData.MoveData.Position);
                        }
                        else
                        {
                            linkedDoorList = GetNpcMovePath(missionItem.MissionTakeNpc);
                        }
                        iNpcID = missionItem.MissionTakeNpc;
                    }
                }
            }
            else if (outValue.status == (int)Protocol.TaskStatus.TASK_UNFINISH)
            {
                int sceneID = _systemTown.GetDungenSceneID(missionItem.MapID);
                linkedDoorList = _systemTown.GetMovePath(sceneID);
            }
            else if (outValue.status == (int)Protocol.TaskStatus.TASK_FINISHED)
            {
                if (missionItem.FinishType == ProtoTable.MissionTable.eFinishType.FINISH_TYPE_NPC)
                {
                    ProtoTable.NpcTable table = TableManager.GetInstance().GetTableItem<ProtoTable.NpcTable>(missionItem.MissionFinishNpc);
                    if (table != null)
                    {
                        BeTownNPC townNpc = _systemTown.GetTownNpcByNpcId(missionItem.MissionFinishNpc);
                        if (townNpc != null)
                        {
                            linkedDoorList.Add(townNpc.ActorData.MoveData.Position);
                        }
                        else
                        {
                            linkedDoorList = GetNpcMovePath(missionItem.MissionFinishNpc);
                        }
                        iNpcID = missionItem.MissionFinishNpc;
                    }
                }
            }

            //如果不需要引导
            if (linkedDoorList.Count == 0)
            {
                _EndTrace();
                return;
            }

            _BeginTrace(iNpcID, linkedDoorList[0]);
        }

        ///  <summary>
        /// 到达某个Npc的路径 
        ///  </summary>
        /// <param name="npcId"></param>
        /// <returns></returns>
        private List<Vector3> GetNpcMovePath(int npcId)
        {
            var table = TableManager.GetInstance().GetTableItem<NpcTable>(npcId);
            if (table == null)
                return null;

            var path = new List<Vector3>();
            for (var i = 0; i < table.MapID.Count; ++i)
            {
                var newPath = _systemTown.GetMovePath(table.MapID[i]);
                if (i == 0)
                {
                    path = newPath;
                }
                else
                {
                    if (newPath.Count < path.Count)
                    {
                        path = newPath;
                    }
                }
            }
            return path;
        }
        #endregion


        #region AutoMove

        private static bool _isAutoMoveSucceed = true;
        //自动寻路结束
        public static void CommandStopAutoMove()
        {
            if (ms_autoMoveCoroutine != null)
            {
                GameFrameWork.instance.StopCoroutine(ms_autoMoveCoroutine);
                ms_autoMoveCoroutine = null;

                Logger.LogProcessFormat("AutoMoveFail (interrupted) ==> move state EMoveState.AutoMove");
                if (BeTownPlayerMain.OnAutoMoveFail != null)
                {
                    BeTownPlayerMain.OnAutoMoveFail.Invoke();
                }
            }

            //自动寻路结束，寻路的数据情况
            SceneMapDataManager.GetInstance().ResetSceneMapPathDataModelList();
        }

        #region AutoMoveToPlayer : NPC or Monster

        //移动到场景中的某个物体
        public void CommandAutoMoveToSceneActor(object sceneActorId,
            ESceneActorType sceneActorType = ESceneActorType.Npc)
        {
            if (m_bEnable == false)
                return;
            CommandStopAutoMove();

            //角色所在的场景Id
            var targetSceneId = GetSceneActorTargetSceneId(sceneActorId,
                sceneActorType);
            if (targetSceneId <= 0)
                return;

            //角色在目标场景中的位置
            var targetPos = Vector3.zero;
            var isGetTargetPos = GetSceneActorTargetPosition(targetSceneId,
                sceneActorId,
                ref targetPos,
                sceneActorType);

            if (isGetTargetPos == false)
                return;

            targetPos = SceneMapUtility.FindValidPosition(targetSceneId, targetPos);

            CommandAutoMoveToTargetPos(targetSceneId, targetPos);
        }

        //得到对象所在的场景Id
        private int GetSceneActorTargetSceneId(object sceneActorId,
            ESceneActorType sceneActorType = ESceneActorType.Npc)
        {
            var targetSceneId = 0;
            switch (sceneActorType)
            {
                case ESceneActorType.Npc:
                    var npcId = (int)sceneActorId;
                    targetSceneId = GetNpcTargetSceneId(npcId);
                    break;
                case ESceneActorType.AttackCityMonster:
                    var attackCityMonsterGuid = (ulong)sceneActorId;
                    targetSceneId = (int)AttackCityMonsterDataManager.GetInstance()
                        .GetSceneIdByNpcGuid(attackCityMonsterGuid);
                    break;
            }

            return targetSceneId;
        }

        //得到某个Npc所在的场景Id
        private int GetNpcTargetSceneId(int npcId)
        {
            var table = TableManager.GetInstance().GetTableItem<NpcTable>(npcId);
            if (table == null)
                return 0;

            var targetSceneId = 0;
            var path = new List<Vector3>();

            for (var i = 0; i < table.MapID.Count; i++)
            {
                var newSceneId = table.MapID[i];
                var newPath = _systemTown.GetMovePath(table.MapID[i]);
                if (i == 0)
                {
                    path = newPath;
                    targetSceneId = newSceneId;
                }
                else
                {
                    if (newPath.Count < path.Count)
                    {
                        path = newPath;
                        targetSceneId = newSceneId;
                    }
                }
            }

            return targetSceneId;
        }

        //是否存在目标点
        private bool GetSceneActorTargetPosition(int targetSceneId,
            object sceneActorId,
            ref Vector3 targetPos,
            ESceneActorType sceneActorType = ESceneActorType.Npc)
        {
            var gridInfo = SceneMapDataManager.GetInstance().GetGridInfoBySceneId(targetSceneId);
            if (gridInfo == null)
                return false;

            if (sceneActorType == ESceneActorType.Npc)
            {
                var sceneData = SceneMapDataManager.GetInstance().GetSceneDataBySceneId(targetSceneId);
                var npcId = (int)sceneActorId;
                var npcInfo = SceneMapUtility.GetNpcInfoByNpcId(npcId, sceneData);

                if (npcInfo == null)
                    return false;

                targetPos = SceneMapUtility.GetValidTargetPosition(gridInfo,
                    npcInfo.GetEntityInfo().GetPosition(),
                    npcInfo.GetMinFindRange(),
                    npcInfo.GetMaxFindRange());

                return true;
            }
            else if (sceneActorType == ESceneActorType.AttackCityMonster)
            {
                var attackCityMonsterGuid = (ulong)sceneActorId;
                var sceneNpcInfo = AttackCityMonsterDataManager.GetInstance()
                    .GetSceneNpcByNpcGuid(attackCityMonsterGuid);
                if (sceneNpcInfo == null)
                    return false;

                var attackCityMonsterPosition = AttackCityMonsterDataManager.GetInstance()
                    .GetAttackCityMonsterScenePosition(sceneNpcInfo.pos);

                var minFindRange = new Vector2(0.5f, 0.5f);
                var maxFindRange = new Vector2(2.0f, 2.0f);

                targetPos = SceneMapUtility.GetValidTargetPosition(gridInfo,
                    attackCityMonsterPosition,
                    minFindRange,
                    maxFindRange);
                return true;
            }

            return false;
        }

        #endregion

        #region AutoMoveToTargetPos

        //移动到某个目标点
        public void CommandAutoMoveToTargetPos(int targetSceneId, 
            Vector3 targetPos,
            bool isShowLastScenePath = true)
        {
            if (m_bEnable == false)
                return;
            CommandStopAutoMove();

            //得到路径
            var movePath = SceneMapDataManager.GetInstance().GetSceneMapMovePath(targetSceneId, targetPos,
                true,
                isShowLastScenePath);

            ms_autoMoveCoroutine = GameFrameWork.instance.StartCoroutine(AsyncAutoMoveToTargetPos(_systemTown,
                movePath,
                targetPos));
        }

        private static IEnumerator AsyncAutoMoveToTargetPos(ClientSystemTown systemTown,
            List<Vector3> doorList,
            Vector3 targetPos)
        {
            _isAutoMoveSucceed = true;

            //不同的场景，需要先跨场景寻路
            if (doorList != null && doorList.Count > 0)
            {
                yield return AsyncAutoMoveCrossScene(systemTown, doorList);
            }

            if (_isAutoMoveSucceed == false)
            {
                OnAutoMoveFail.Invoke();
                ms_autoMoveCoroutine = null;
                _isAutoMoveSucceed = true;
                yield break;
            }

            yield return AsyncAutoMoveToTargetPos(systemTown, targetPos);

            if (_isAutoMoveSucceed == false)
            {
                OnAutoMoveFail.Invoke();
            }
            else
            {
                OnAutoMoveSuccess.Invoke();
            }

            ms_autoMoveCoroutine = null;
            _isAutoMoveSucceed = true;
        }

        private static IEnumerator AsyncAutoMoveCrossScene(ClientSystemTown systemTown, List<Vector3> doorList)
        {
            _isAutoMoveSucceed = true;

            if (_CheckSystemValid(systemTown) == false)
            {
                _isAutoMoveSucceed = false;
                yield break;
            }

            while (doorList.Count > 0)
            {
                if (_CheckSystemValid(systemTown) == false)
                {
                    _isAutoMoveSucceed = false;
                    yield break;
                }

                Vector3 targetPos = doorList[0];
                doorList.RemoveAt(0);
                systemTown.MainPlayer.CommandMoveTo(targetPos);

                if (systemTown.MainPlayer.MoveState != EMoveState.AutoMoving)
                {
                    _isAutoMoveSucceed = false;
                    yield break;
                }

                int oldSceneId = systemTown.CurrentSceneID;
                while (oldSceneId == systemTown.CurrentSceneID)
                {
                    if (_CheckSystemValid(systemTown) == false)
                    {
                        _isAutoMoveSucceed = false;
                        yield break;
                    }

                    if (systemTown.MainPlayer.MoveState == EMoveState.AutoMoving)
                    {
                        yield return Yielders.EndOfFrame;
                    }
                    else if (systemTown.MainPlayer.MoveState == EMoveState.Moveing)
                    {
                        _isAutoMoveSucceed = false;
                        yield break;
                    }
                    else
                    {
                        if (systemTown.MainPlayer._IsInSameGrid(targetPos, systemTown.MainPlayer.ActorData.MoveData.Position) == false)
                        {
                            _isAutoMoveSucceed = false;
                            yield break;
                        }
                        else
                        {
                            yield return Yielders.EndOfFrame;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 同场景寻路到特定位置
        /// </summary>
        /// <param name="a_systemTown"></param>
        /// <param name="a_pos"></param>
        /// <returns></returns>
        private static IEnumerator AsyncAutoMoveToTargetPos(ClientSystemTown a_systemTown, Vector3 a_pos)
        {
            _isAutoMoveSucceed = true;

            Logger.LogProcessFormat("当前地图寻路 >>> 当前场景{0} ", a_systemTown.CurrentSceneID);
            if (_CheckSystemValid(a_systemTown) == false)
            {
                Logger.LogProcessFormat("当前地图寻路 >>> 检测失败");
                _isAutoMoveSucceed = false;
                yield break;
            }
            a_systemTown.MainPlayer.CommandMoveTo(a_pos);

            while (true)
            {
                if (_CheckSystemValid(a_systemTown) == false)
                {
                    Logger.LogProcessFormat("当前地图寻路 >>> 检测失败");
                    _isAutoMoveSucceed = false;
                    yield break;
                }
                if (a_systemTown.MainPlayer.MoveState == EMoveState.AutoMoving)
                {
                    yield return Yielders.EndOfFrame;
                }
                else if (a_systemTown.MainPlayer.MoveState == EMoveState.Moveing)
                {
                    Logger.LogProcessFormat("AutoMoveFail (interrupted) ==> move state EMoveState.Moveing");
                    _isAutoMoveSucceed = false;
                    yield break;
                }
                else
                {
                    if (a_systemTown.MainPlayer._IsInSameGrid(a_pos, a_systemTown.MainPlayer.ActorData.MoveData.Position))
                    {
                        Logger.LogProcessFormat("当前地图寻路 >>> 移动成功");
                        _isAutoMoveSucceed = true;
                    }
                    else
                    {
                        Logger.LogProcessFormat("AutoMoveFail (interrupted) ==> move state EMoveState.Idle");
                        _isAutoMoveSucceed = false;
                    }
                    yield break;
                }
            }
        }

        #endregion

        #region AutoMoveToScene
        
        //移动到某个地下城
        public void CommandMoveToDungeon(int dungeonId)
        {
            var targetSceneId = _systemTown.GetDungenSceneID(dungeonId);

            if (targetSceneId <= 0)
                return;

            CommandMoveToScene(targetSceneId);
        }

        //移动到某个场景中
        public void CommandMoveToScene(int targetSceneId)
        {
            if (m_bEnable == false)
            {
                return;
            }
            CommandStopAutoMove();

            if (_systemTown.CurrentSceneID == targetSceneId)
                return;

            //得到路径
            var movePath = SceneMapDataManager.GetInstance().GetSceneMapMovePath(targetSceneId, Vector3.zero,
                true,
                false);

            if (movePath.Count > 0)
            {
                ms_autoMoveCoroutine = GameFrameWork.instance.StartCoroutine(AsyncAutoMoveToTargetScene(_systemTown, movePath));
            }
            else
            {
                SystemNotifyManager.SysNotifyMsgBoxOK(TR.Value("common_cannot_auto_move"));
            }
        }

        private static IEnumerator AsyncAutoMoveToTargetScene(ClientSystemTown systemTown, List<Vector3> doorList)
        {
            // 跨地图
            while (doorList.Count > 0)
            {
                if (_CheckSystemValid(systemTown) == false)
                {
                    OnAutoMoveFail.Invoke();
                    ms_autoMoveCoroutine = null;
                    yield break;
                }

                var targetPos = doorList[0];
                doorList.RemoveAt(0);
                systemTown.MainPlayer.CommandMoveTo(targetPos);

                if (systemTown.MainPlayer.MoveState != EMoveState.AutoMoving)
                {
                    OnAutoMoveFail.Invoke();
                    ms_autoMoveCoroutine = null;
                    yield break;
                }

                var oldSceneId = systemTown.CurrentSceneID;
                while (oldSceneId == systemTown.CurrentSceneID)
                {
                    if (_CheckSystemValid(systemTown) == false)
                    {
                        OnAutoMoveFail.Invoke();
                        ms_autoMoveCoroutine = null;
                        yield break;
                    }

                    if (systemTown.MainPlayer.MoveState == EMoveState.AutoMoving)
                    {
                        yield return Yielders.EndOfFrame;
                    }
                    else if (systemTown.MainPlayer.MoveState == EMoveState.Moveing)
                    {
                        OnAutoMoveFail.Invoke();
                        ms_autoMoveCoroutine = null;
                        yield break;
                    }
                    else
                    {
                        if (systemTown.MainPlayer._IsInSameGrid(targetPos, systemTown.MainPlayer.ActorData.MoveData.Position))
                        {
                            yield return Yielders.EndOfFrame;
                        }
                        else
                        {
                            OnAutoMoveFail.Invoke();
                            ms_autoMoveCoroutine = null;
                            yield break;
                        }
                    }
                }
            }

            ms_autoMoveCoroutine = null;
            OnAutoMoveSuccess.Invoke();
        }
        #endregion

        #endregion

        #region CommandMoveRelation

        public sealed override void CommandMoveForward(Vector3 targetDirection)
        {
            if (m_bEnable == false)
            {
                return;
            }

            targetDirection.y = 0.0f;
            Logger.LogFormat("Command Move Forward ==> target dir:({0},{1},{2})", targetDirection.x, targetDirection.y, targetDirection.z);
            base.CommandMoveForward(targetDirection);
            _UpdateMoveState();
        }

        public sealed override void CommandDirectMoveTo(Vector3 targetPosition)
        {
            if (m_bEnable == false)
            {
                return;
            }

            targetPosition.y = 0.0f;
            Logger.LogProcessFormat("Command direct move to ==> target pos:({0},{1},{2})", targetPosition.x, targetPosition.y, targetPosition.z);
            base.CommandDirectMoveTo(targetPosition);
            _UpdateMoveState();
        }

        public sealed override void CommandMoveTo(Vector3 targetPosition)
        {
            if (m_bEnable == false)
            {
                Logger.LogProcessFormat("Command move to ==> target pos:({0},{1},{2})  主角处于无效状态", targetPosition.x, targetPosition.y, targetPosition.z);
                return;
            }

            targetPosition.y = 0.0f;
            Logger.LogProcessFormat("Command move to ==> target pos:({0},{1},{2})", targetPosition.x, targetPosition.y, targetPosition.z);
            base.CommandMoveTo(targetPosition);
            _UpdateMoveState();
        }

        public sealed override void CommandStopMove()
        {
            if (m_bEnable == false)
            {
                return;
            }

            Logger.LogProcessFormat("Command stop move");
            base.CommandStopMove();
            _UpdateMoveState();
        }
        
        #endregion

        public sealed override void UpdateMove(float timeElapsed)
        {
            if (m_bEnable == false)
            {
                return;
            }

            if (_systemTown.IsNet)
            {
                _SyncMove();
            }

            base.UpdateMove(timeElapsed);

            if (MoveState == EMoveState.Moveing)
            {
                OnMoveing.Invoke(ActorData.MoveData.Position);
            }
            else if (MoveState == EMoveState.AutoMoving)
            {
                OnAutoMoving.Invoke(ActorData.MoveData.Position);
            }
        }

        public sealed override void CreatePet(int a_nPetID)
        {
            base.CreatePet(a_nPetID);

            if (m_townPet != null)
            {
                m_townPet.SetDialogEnable(true);
            }
        }

        public class MoveStateChangedEvent : UnityEvent<EMoveState, EMoveState> { }
        public class AutoMoveSuccessEvent : UnityEvent { }
        public class AutoMoveFailEvent : UnityEvent { }
        public class MoveingEvent : UnityEvent<Vector3> { }
        public class AutoMoveingEvent : UnityEvent<Vector3> { }

        protected static bool _CheckSystemValid(ClientSystemTown a_system)
        {
            if (a_system == null)
            {
                Logger.LogProcessFormat("systemTown is null! stop!!");
                return false;
            }
            if (a_system.MainPlayer == null)
            {
                Logger.LogProcessFormat("main player is null! stop!!");
                return false;
            }

            return true;
        }

        protected EMoveState _CalculateMoveState()
        {
            EMoveState newMoveState = EMoveState.Idle;
            if (ActorData.MoveData.MoveType == EActorMoveType.Invalid)
            {
                newMoveState = EMoveState.Idle;
            }
            else if (ActorData.MoveData.MoveType == EActorMoveType.TargetDir)
            {
                newMoveState = EMoveState.Moveing;
            }
            else if (ActorData.MoveData.MoveType == EActorMoveType.TargetPos)
            {
                newMoveState = EMoveState.AutoMoving;
            }
            return newMoveState;
        }

        protected void _UpdateMoveState()
        {
            EMoveState newMoveState = _CalculateMoveState();
            if (MoveState != newMoveState)
            {
                EMoveState oldMoveState = MoveState;
                MoveState = newMoveState;
                Logger.LogProcessFormat("移动状态发生变化 >>> 老的状态:{0} 新的状态:{1}", oldMoveState.ToString(), MoveState.ToString());
                OnMoveStateChanged.Invoke(oldMoveState, MoveState);
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PlayerMoveStateChanged, oldMoveState, MoveState);

                Logger.LogProcessFormat("移动状态发生变化 >>> 老的状态:{0} 新的状态:{1}", oldMoveState.ToString(), MoveState.ToString());
            }
        }

        protected Vector3 m_moveDir = Vector3.zero;
        protected Vector3 m_oldMoveDir = Vector3.zero;
        protected Vector3 m_oldTargetPos = Vector3.zero;
        protected EActorMoveType m_oldMoveType = EActorMoveType.Invalid;
        protected float m_syncElapsed = 0.0f;
        protected float m_axisScale = 10000.0f;
        protected void _SyncMove()
        {

            ActorMoveData moveData = ActorData.MoveData;
            if (m_oldMoveType != moveData.MoveType)
            {
                if (moveData.MoveType == EActorMoveType.TargetDir)
                {
                    m_oldMoveDir = moveData.TargetDirection;
                    m_oldTargetPos = Vector3.zero;
                    m_moveDir = moveData.TargetDirection;
                }
                else if (moveData.MoveType == EActorMoveType.TargetPos)
                {
                    m_oldMoveDir = Vector3.zero;
                    m_oldTargetPos = moveData.TargetPosition;
                    m_moveDir = (moveData.TargetPosition - moveData.Position).normalized;
                }
                else
                {
                    m_oldMoveDir = Vector3.zero;
                    m_oldTargetPos = Vector3.zero;
                    m_moveDir = Vector3.zero;
                }

                m_oldMoveType = moveData.MoveType;
                _SendSyncMoveMessage();
            }
            else
            {
                if (moveData.MoveType != EActorMoveType.Invalid)
                {
                    bool bSended = false;
                    if (moveData.MoveType == EActorMoveType.TargetDir && m_oldMoveDir != moveData.TargetDirection)
                    {
                        m_oldMoveDir = moveData.TargetDirection;
                        m_moveDir = moveData.TargetDirection;
                        _SendSyncMoveMessage();
                        bSended = true;
                    }
                    else if (moveData.MoveType == EActorMoveType.TargetPos && m_oldTargetPos != moveData.TargetPosition)
                    {
                        m_oldTargetPos = moveData.TargetPosition;
                        Vector3 newDir = (moveData.TargetPosition - moveData.Position).normalized;
                        if (m_moveDir != newDir)
                        {
                            m_moveDir = newDir;
                            _SendSyncMoveMessage();
                            bSended = true;
                        }
                    }

                    if (bSended == false)
                    {
                        m_syncElapsed += Time.deltaTime;
                        if (m_syncElapsed > 0.2f)
                        {
                            _SendSyncMoveMessage();
                            m_syncElapsed = 0.0f;
                        }
                    }
                }
            }
        }

        protected void _SendSyncMoveMessage()
        {
            SceneMoveRequire req = new SceneMoveRequire
            {
                dir = new SceneDir
                {
                    x = (short)(m_moveDir.x * m_axisScale),
                    y = (short)(m_moveDir.z * m_axisScale),
                    faceRight = (byte)(ActorData.MoveData.FaceRight ? 1 : 0)
                },
                pos = new ScenePosition
                {
                    x = (uint)(ActorData.MoveData.ServerPosition.x * m_axisScale),
                    y = (uint)(ActorData.MoveData.ServerPosition.z * m_axisScale)
                }
            };

            var current = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
            if (current != null)
            {
                req.clientSceneId = (uint)current.CurrentSceneID;
            }

            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);

            Logger.LogFormat(
                "send move dir:({0},{1}) face right:{4} pos:({2},{3})",
                req.dir.x, req.dir.y,
                ActorData.MoveData.Position.x,
                ActorData.MoveData.Position.z,
                req.dir.faceRight
                );
        }

        protected bool _IsInSameGrid(Vector3 posA, Vector3 posB)
        {
            PathFinding.Grid gridA = new PathFinding.Grid(_systemTown.GridInfo, posA);
            PathFinding.Grid gridB = new PathFinding.Grid(_systemTown.GridInfo, posB);
            return gridA.X == gridB.X && gridA.Y == gridB.Y;
        }

        //-1表示获得数值的时候失败，需要特殊处理
        //其他数值表示能够获得主角的附魔值
        public int OwnerResistMagicValue()
        {

            var resistMagicValue = BeUtility.GetMainActorResist();
            return resistMagicValue;

            //var mainPlayerAttribute = BeUtility.GetMainPlayerActorAttribute();
            //if (mainPlayerAttribute == null)
            //    return -1;

            //var resistMagicValue = mainPlayerAttribute.resistMagic;

            //return (int) resistMagicValue;

            //var resistMagicName = "resistMagic";
            //var fieldInfo = mainPlayerAttribute.GetType().GetField(resistMagicName);
            //if (fieldInfo == null)
            //    return -1;

            //var resistMagicValue = (float)fieldInfo.GetValue(mainPlayerAttribute);
            //return (int)resistMagicValue;
        }

        //同步主角抗魔值，同步的同时保存主角抗魔值
        public void SyncResistMagicValue()
        {
            var resistMagicValue = OwnerResistMagicValue();
            if(resistMagicValue == -1)
                return;

            //保存主角的附魔值
            if (PlayerBaseData.GetInstance() != null)
            {
				//抗魔值保持不变，不用同步
                if (PlayerBaseData.GetInstance().ResistMagicValue == resistMagicValue)
                    return;

                PlayerBaseData.GetInstance().ResistMagicValue = resistMagicValue;
            }
            
            SceneSyncResistMagicReq req = new SceneSyncResistMagicReq
            {
                resist_magic = (uint) resistMagicValue,
            };

            if (NetManager.Instance() != null)
                NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);

        }

        #region OnAutoMove

        //点击地图自动寻路的场景和目标点
        public static int OnAutoMoveTargetSceneId = -1;
        public static Vector3 OnAutoMoveTargetScenePos = Vector3.zero;

        public static void ResetOnAutoMoveTargetData()
        {
            OnAutoMoveTargetSceneId = -1;
            OnAutoMoveTargetScenePos = Vector3.zero;
            SceneMapDataManager.GetInstance().ResetSceneMapPathDataModelList();
        }

        public static void SetOnAutoMoveTargetData(int sceneId, Vector3 targetPos)
        {
            OnAutoMoveTargetSceneId = sceneId;
            OnAutoMoveTargetScenePos = targetPos;
        }

        #endregion

    }
}
