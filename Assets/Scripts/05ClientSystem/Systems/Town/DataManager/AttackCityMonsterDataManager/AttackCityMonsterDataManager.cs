using System;
using System.Collections;
using System.Collections.Generic;
using Network;
using Protocol;
using ProtoTable;
using UnityEngine;
using UnityEngine.Events;

namespace GameClient
{
    //客户端使用的数据模型，与服务器相同，只是List替换了Array
    public class SceneNpcDataModel
    {
        public UInt32 SceneId;
        public List<SceneNpc> NpcsList = new List<SceneNpc>();
    }

    public enum EOpenTalkFrameType
    {
        Invalid = -1,
        Normal = 0,                     //用户打开怪物对话框
        Mission = 1,                    //任务打开怪物对话框
        ActivityDuplication = 2,        //活动副本打开怪物对话框
        TeamDuplication = 3,            //组队副本打开怪物对话框
    }

    public class AttackCityMonsterDataManager : DataManager<AttackCityMonsterDataManager>
    {

        private List<SceneNpcDataModel> _sceneNpcDataModelList = new List<SceneNpcDataModel>();
        private UInt64 _missionMonsterGuid = 0;             //任务中怪物的Guid；
        private UInt64 _nearestByMonsterGuid = 0;           //最近寻路的怪物Guid
        private EOpenTalkFrameType _eOpenTalkFrameType = EOpenTalkFrameType.Invalid;

        public int _attackCityMonsterBeatTimes = 0;            //攻城怪物的当前次数
        public int _attackCityMonsterTotalTimes = 0;           //攻城怪物的总次数

        private const float NpcPositionCoefficient = 10000.0f;           //服务器坐标和场景坐标之间转化的系数
        private const string AttackCityMonsterStr = "AttackCityMonster";
        private const string NetNpcDataStr = "npc_data";
        private const string NetNpcGuidStr = "npc_guid";
        private const string NetAttackCityMonsterTimesStr = "cm_times";
        //暂时没有用到
        private const string NetNpcDungeonStr = "npc_dun";
        private const string NetNpcSceneStr = "npc_scene";
        private const string NetNpcXStr = "npc_x";
        private const string NetNpcYStr = "npc_y";
        
        public const int LimitLevel = 25;                   //进入怪物攻城关卡的等级限制

        #region InitAndClear
        //攻城怪物的数据管理器，接受并保存攻城怪物的数据
        public override void Initialize()
        {
            BindEvents();

            InitData();
        }

        private void InitData()
        {
            var attackCityMonsterTimesItem = TableManager.GetInstance().GetTableItem<SystemValueTable>(
                (int) SystemValueTable.eType2.SVT_CITY_MONSTER_DAILY_COUNT);
            if (attackCityMonsterTimesItem == null || attackCityMonsterTimesItem.Value <= 0)
            {
                _attackCityMonsterTotalTimes = 0;
            }
            else
            {
                _attackCityMonsterTotalTimes = attackCityMonsterTimesItem.Value;
            }
        }

        public override void Clear()
        {
            UnBindEvents();
            ClearData();
        }

        private void ClearData()
        {
            _attackCityMonsterTotalTimes = 0;
            _attackCityMonsterBeatTimes = 0;
            _nearestByMonsterGuid = 0;
            _missionMonsterGuid = 0;
            _eOpenTalkFrameType = EOpenTalkFrameType.Invalid;
            //清空场景中怪物的数据
            ClearSceneNpcDataModelList();
        }

        private void ClearSceneNpcDataModelList()
        {
            for (var i = 0; i < _sceneNpcDataModelList.Count; i++)
            {
                if(_sceneNpcDataModelList[i] != null)
                    _sceneNpcDataModelList[i].NpcsList.Clear();
            }
            _sceneNpcDataModelList.Clear();
        }
        #endregion

        #region SyncNetData
        private void BindEvents()
        {
            NetProcess.AddMsgHandler(SceneNpcList.MsgID, NetSyncSceneNpcList);
            NetProcess.AddMsgHandler(SceneNpcAdd.MsgID, NetSyncSceneNpcAdd);
            NetProcess.AddMsgHandler(SceneNpcDel.MsgID, NetSyncSceneNpcDel);
            NetProcess.AddMsgHandler(SceneNpcUpdate.MsgID, NetSyncSceneNpcUpdate);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnCountValueChange, OnCountValueChanged);
        }

        private void UnBindEvents()
        {
            NetProcess.RemoveMsgHandler(SceneNpcList.MsgID, NetSyncSceneNpcList);
            NetProcess.RemoveMsgHandler(SceneNpcAdd.MsgID, NetSyncSceneNpcAdd);
            NetProcess.RemoveMsgHandler(SceneNpcDel.MsgID, NetSyncSceneNpcDel);
            NetProcess.RemoveMsgHandler(SceneNpcUpdate.MsgID, NetSyncSceneNpcUpdate);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnCountValueChange, OnCountValueChanged);
        }
        
        //同步服务器的SceneNpcList数据，需要清空当前的数据
        private void NetSyncSceneNpcList(MsgDATA msgData)
        {
            if (msgData == null)
            {
                Logger.LogErrorFormat("SyncSceneNpcList ==> msgData is null");
                return;
            }

            var sceneNpcList = new SceneNpcList();
            sceneNpcList.decode(msgData.bytes);

            //清空旧的数据
            ClearSceneNpcDataModelList();

            //添加新的数据
            for (var i = 0; i < sceneNpcList.infoes.Length; i++)
            {
                var sceneNpcInfo = sceneNpcList.infoes[i];
                //创建SceneNpcDataModel数据结构，并进行赋值

                var sceneNpcDataModel = CreateSceneNpcDataModel(sceneNpcInfo);

                _sceneNpcDataModelList.Add(sceneNpcDataModel);
            }

            //发送UI事件
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.SyncAttackCityMonsterList);
        }

        //同步服务器的SceneNpcAdd数据，在当前数据的基础上增加
        private void NetSyncSceneNpcAdd(MsgDATA msgData)
        {
            if (msgData == null)
            {
                Logger.LogErrorFormat("syncSceneNpcAdd ==> msgData is null");
                return;
            }

            var sceneNpcAdd = new SceneNpcAdd();
            sceneNpcAdd.decode(msgData.bytes);

            //没有数据，不需要同步和UI的出发
            if(sceneNpcAdd.data == null || sceneNpcAdd.data.Length <= 0)
                return;

            //更新数据
            for (var i = 0; i < sceneNpcAdd.data.Length; i++)
            {
                var sceneNpcInfo = sceneNpcAdd.data[i];
                var sceneNpcDataModel = GetSceneNpcDataModel(sceneNpcInfo.sceneId);
                //如果某一个场景的DataModel不存在，则新创建一个
                if (sceneNpcDataModel == null)
                {
                    sceneNpcDataModel = CreateSceneNpcDataModel(sceneNpcInfo);
                    _sceneNpcDataModelList.Add(sceneNpcDataModel);
                }
                else
                {
                    //如果存在，将该场景中同步的npc，添加进来
                    for (var j = 0; j < sceneNpcInfo.npcs.Length; j++)
                    {
                        sceneNpcDataModel.NpcsList.Add(sceneNpcInfo.npcs[j]);
                    }
                }
            }

            //发送UI事件
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.SyncAttackCityMonsterAdd);
        }

        //同步服务器的SceneNpcDel数据，在当前数据的基础上删除
        private void NetSyncSceneNpcDel(MsgDATA msgData)
        {
            if (msgData == null)
            {
                Logger.LogErrorFormat("SyncSceneNpcDel ==> msgData is null");
                return;
            }

            var sceneNpcDel = new SceneNpcDel();
            sceneNpcDel.decode(msgData.bytes);

            if (sceneNpcDel.guids == null || sceneNpcDel.guids.Length <= 0)
                return;

            for (var i = 0; i < sceneNpcDel.guids.Length; i++)
            {
                RemoveSceneNpcDataModel(sceneNpcDel.guids[i]);
            }
            
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.SyncAttackCityMonsterDel);

        }

        //同步服务器的SceneNpcUpdate数据，在当前数据的基础上，更新SceneNpc的数据
        private void NetSyncSceneNpcUpdate(MsgDATA msgData)
        {
            if (msgData == null)
            {
                Logger.LogErrorFormat("SyncSceneNpcUpdate ==> msgData is null");
                return;
            }

            var sceneNpcUpdate = new SceneNpcUpdate();
            sceneNpcUpdate.decode(msgData.bytes);
            
            if(sceneNpcUpdate.data == null 
               || sceneNpcUpdate.data.sceneId <= 0 
               || sceneNpcUpdate.data.npcs == null
               || sceneNpcUpdate.data.npcs.Length <= 0)
                return;

            var sceneNpcDataModel = GetSceneNpcDataModel(sceneNpcUpdate.data.sceneId);
            if(sceneNpcDataModel == null)
                return;

            for (var i = 0; i < sceneNpcUpdate.data.npcs.Length; i++)
            {
                var curSceneNpc = sceneNpcUpdate.data.npcs[i];
                if(curSceneNpc == null)
                    continue;

                for (var j = 0; j < sceneNpcDataModel.NpcsList.Count; j++)
                {
                    var curDataModel = sceneNpcDataModel.NpcsList[j];
                    if(curDataModel == null)
                        continue;
                    if (curSceneNpc.guid == curDataModel.guid)
                    {
                        sceneNpcDataModel.NpcsList[j] = curSceneNpc;
                        continue;
                    }
                }
            }
            
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.SyncAttackCityMonsterUpdate);
        }

        //由服务器的数据模型，创建客户端数据模型
        private SceneNpcDataModel CreateSceneNpcDataModel(SceneNpcInfo sceneNpcInfo)
        {
            var sceneNpcDataModel = new SceneNpcDataModel();
            sceneNpcDataModel.SceneId = sceneNpcInfo.sceneId;
            for (var i = 0; i < sceneNpcInfo.npcs.Length; i++)
            {
                sceneNpcDataModel.NpcsList.Add(sceneNpcInfo.npcs[i]);
            }

            return sceneNpcDataModel;
        }

        //根据SceneID，得到当前场景的SceneNpcDataModel
        private SceneNpcDataModel GetSceneNpcDataModel(uint sceneId)
        {
            for (var i = 0; i < _sceneNpcDataModelList.Count; i++)
            {
                if (_sceneNpcDataModelList[i].SceneId == sceneId)
                    return _sceneNpcDataModelList[i];
            }

            return null;
        }

        //删除一个SceneNpc
        private void RemoveSceneNpcDataModel(UInt64 guid)
        {
            for (var i = 0; i < _sceneNpcDataModelList.Count; i++)
            {
                if(_sceneNpcDataModelList[i].NpcsList.Count <= 0)
                    continue;

                for (var j = 0; j < _sceneNpcDataModelList[i].NpcsList.Count; j++)
                {
                    if (_sceneNpcDataModelList[i].NpcsList[j].guid == guid)
                    {
                        _sceneNpcDataModelList[i].NpcsList.RemoveAt(j);
                        return;
                    }
                }
            }
        }
               
        #endregion

        #region FindPathToMonster

        //展示攻城怪物的对话框
        public void ShowAttackCityMonsterDialogFrame(UInt64 npcGuid)
        {
            ClientSystemManager.GetInstance().OpenFrame<AttackCityMonsterTalkFrame>(FrameLayer.Middle, npcGuid);
        }

        /// <summary>
        /// 由组队中进入查找最近攻城怪物的流程
        /// </summary>
        public void EnterFindPathProcessByTeamDuplication()
        {
            if (IsExistActivityTypeCityMonster() == false)
            {
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("monster_attack_city_no_activity_monster"));
                return;
            }

            //关闭对应的Frame
            if (ClientSystemManager.GetInstance().IsFrameOpen<TeamListFrame>() == true)
            {
                ClientSystemManager.GetInstance().CloseFrame<TeamListFrame>();
            }
            ClientSystemManager.GetInstance().CloseFrame<TeamMyFrame>();

            //自动寻路到最近的怪物
            CommandMoveToNearestByAttackCityMonster();
            SetOpenTalkFrameType(EOpenTalkFrameType.TeamDuplication);
        }

        /// <summary>
        /// 由活动副本进入查找最近攻城怪物的流程
        /// </summary>
        public void EnterFindPathProcessByActivityDuplication()
        {
            if (IsExistActivityTypeCityMonster() == false)
            {
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("monster_attack_city_no_activity_monster"));
                return;
            }

            //关闭对应的Frame
            if (ClientSystemManager.GetInstance().IsFrameOpen<ActivityDungeonFrame>() == true)
                ClientSystemManager.GetInstance().CloseFrame<ActivityDungeonFrame>();

            CommandMoveToNearestByAttackCityMonster();
            SetOpenTalkFrameType(EOpenTalkFrameType.ActivityDuplication);
        }

        public void EnterFindPathProcessByMissionContent(Dictionary<string, string> content,
            UnityAction onSucceed = null,
            UnityAction onFailed = null)
        {
            if (content == null || content.Count <= 0)
            {
                Logger.LogErrorFormat("EnterAttackCityMonsterByMissionContent content is null");
                return;
            }

            string npcGuid;
            if (content.TryGetValue(NetNpcGuidStr, out npcGuid) == true)
            {
                var guid = UInt64.Parse(npcGuid);
                if (guid > 0)
                {
                    EnterFindPathProcessByMonsterGuid(guid,
                        onSucceed,
                        onFailed);
                    return;
                }
            }
        }

        /// <summary>
        /// 通过任务进入到寻路特定怪物的流程
        /// </summary>
        private void EnterFindPathProcessByMonsterGuid(UInt64 monsterGuid,
            UnityAction onSucceed = null,
            UnityAction onFailed = null)
        {
            //当前正在查找
            if (monsterGuid == _missionMonsterGuid)
            {
                return;
            }

            if (IsSceneDataContainNpcData(monsterGuid) == false)
            {
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("monster_attack_city_no_task_monster"));
                return;
            }

            _missionMonsterGuid = monsterGuid;
            SetOpenTalkFrameType(EOpenTalkFrameType.Mission);

            CommandMoveToAttackCityMonsterByGuid(monsterGuid);

        }

        /// <summary>
        /// 走到最近的攻城怪物
        /// </summary>
        private void CommandMoveToNearestByAttackCityMonster()
        {
            //正在进行寻路到最近的怪物中,并且这个怪物存在，则直接返回，避免一直在不同的怪物中寻路
            //(玩家一直点击)
            if (_nearestByMonsterGuid != 0 && IsSceneDataContainNpcData(_nearestByMonsterGuid) == true)
                return;

            _nearestByMonsterGuid = 0;
            GetNearestByAttackCityMonsterGuid(ref _nearestByMonsterGuid);

            if (_nearestByMonsterGuid == 0)
            {
                return;
            }
            else
            {
                CommandMoveToAttackCityMonsterByGuid(_nearestByMonsterGuid);
            }
        }

        //朝着某个攻城怪物进行自动寻路，所有自动寻路到怪物的调用都汇到这里
        private void CommandMoveToAttackCityMonsterByGuid(UInt64 npcGuid,
            UnityAction onSucceed = null,
            UnityAction onFailed = null)
        {
            var sceneNpc = GetSceneNpcByNpcGuid(npcGuid);
            if (sceneNpc == null)
            {
                Logger.LogErrorFormat("The sceneNpc is not exist and npcGuid is {0}", npcGuid);
                return;
            }

            var sceneId = GetSceneIdByNpcGuid(npcGuid);
            if (sceneId == 0)
            {
                Logger.LogErrorFormat("Scene is not exist and npcGuid is {0}", npcGuid);
                return;
            }

            var npcId = (int)sceneNpc.id;
            var npcItem = TableManager.GetInstance().GetTableItem<NpcTable>(npcId);
            if (npcItem == null)
            {
                Logger.LogErrorFormat("npcItem is null or npcItem is not AttackCityMonster, monsterid is {0}", sceneNpc.id);
                return;
            }

            var clientSystemTown = ClientSystemManager.GetInstance().GetCurrentSystem() as ClientSystemTown;
            if (clientSystemTown == null)
            {
                Logger.LogErrorFormat("ClientSystemTown is null");
                return;
            }
            if (clientSystemTown.MainPlayer == null)
            {
                Logger.LogErrorFormat("MainPlayer in clientSystemTown is null");
                return;
            }

            Parser.NpcParser.OnClickLink(npcGuid,
                npcId,
                ESceneActorType.AttackCityMonster,
                () =>
                {
                    if (onSucceed != null)
                        onSucceed.Invoke();

                    GetInstance().ShowAttackCityMonsterDialogFrame(npcGuid);
                    _nearestByMonsterGuid = 0;
                    _missionMonsterGuid = 0;
                },
                () =>
                {
                    if (onFailed != null)
                        onFailed.Invoke();

                    _nearestByMonsterGuid = 0;
                    _missionMonsterGuid = 0;
                });
        }
        #endregion

        #region Helper

        public EOpenTalkFrameType GetOpenTalkFrameType()
        {
            return _eOpenTalkFrameType;
        }

        public void SetOpenTalkFrameType(EOpenTalkFrameType openTalkFrameType)
        {
            _eOpenTalkFrameType = openTalkFrameType;
        }

        public void ResetOpenTalkFrameType()
        {
            _eOpenTalkFrameType = EOpenTalkFrameType.Invalid;
        }

        //由场景ID，获得这个场景中的Npc数据
        public List<SceneNpc> GetSceneNpcsListBySceneId(int sceneId)
        {
            for (var i = 0; i < _sceneNpcDataModelList.Count; i++)
            {
                if (_sceneNpcDataModelList[i].SceneId == sceneId)
                    return _sceneNpcDataModelList[i].NpcsList;
            }
            return null;
        }

        //当前场景中是否包含活动类型的攻城怪物，用于最近怪物的查找
        private List<SceneNpc> GetActivityMonsterDataModelBySceneId(int sceneId)
        {
            var sceneNpcsDataModel = new List<SceneNpc>();
            for (var i = 0; i < _sceneNpcDataModelList.Count; i++)
            {
                if((int)_sceneNpcDataModelList[i].SceneId != sceneId)
                    continue;
                for (var j = 0; j < _sceneNpcDataModelList[i].NpcsList.Count; j++)
                {
                    if ((CityMonsterGenerate.eMonsterType) _sceneNpcDataModelList[i].NpcsList[j].funcType
                        == CityMonsterGenerate.eMonsterType.Activity)
                    {
                        sceneNpcsDataModel.Add(_sceneNpcDataModelList[i].NpcsList[j]);
                    }
                }
            }

            return sceneNpcsDataModel;
        }

        //由Npc的guid获得这个Npc所在的场景ID
        public UInt32 GetSceneIdByNpcGuid(UInt64 guid)
        {
            for (var i = 0; i < _sceneNpcDataModelList.Count; i++)
            {
                for (var j = 0; j < _sceneNpcDataModelList[i].NpcsList.Count; j++)
                {
                    if (_sceneNpcDataModelList[i].NpcsList[j].guid == guid)
                        return _sceneNpcDataModelList[i].SceneId;
                }
            }
            return 0;
        }

        //得到怪物的SceneNpc信息
        public SceneNpc GetSceneNpcByNpcGuid(UInt64 guid)
        {
            for (var i = 0; i < _sceneNpcDataModelList.Count; i++)
            {
                for (var j = 0; j < _sceneNpcDataModelList[i].NpcsList.Count; j++)
                {
                    if (_sceneNpcDataModelList[i].NpcsList[j].guid == guid)
                        return _sceneNpcDataModelList[i].NpcsList[j];
                }
            }
            return null;
        }

        //随机得到最近攻城怪物ID
        //用于组队和活动查找
        //任务中固定的怪物ID，不需要查找
        private void GetNearestByAttackCityMonsterGuid(ref UInt64 monsterGuid)
        {
            //传进来的monsterId默认为0;

            if(_sceneNpcDataModelList == null || _sceneNpcDataModelList.Count <= 0)
                return;
            
            var clientSystemTown = ClientSystemManager.GetInstance().GetCurrentSystem() as ClientSystemTown;
            if (clientSystemTown == null)
                return;

            if (clientSystemTown.MainPlayer == null)
                return;

            //当前场景中是否包含活动类型的攻城怪物
            var activityMonsterDataModelList = GetActivityMonsterDataModelBySceneId(clientSystemTown.CurrentSceneID);
            if (activityMonsterDataModelList != null && activityMonsterDataModelList.Count > 0)
            {
                monsterGuid = GetSceneNpcGuidByRandom(activityMonsterDataModelList);
                return;
            }
            
            //如果当前场景不存在怪物，找到包含怪物的最近场景
            var nearestBySceneId = 0;
            List<Vector3> nearestByPathList = null;
            for (var i = 0; i < _sceneNpcDataModelList.Count; i++)
            {
                //需要查找的场景中不存在怪物
                var sceneNpcDataModel = _sceneNpcDataModelList[i];
                if(sceneNpcDataModel == null 
                   || sceneNpcDataModel.NpcsList == null 
                   || sceneNpcDataModel.NpcsList.Count <= 0)
                    continue;

                //需要查找的场景中是否存在活动类型的怪物
                activityMonsterDataModelList = GetActivityMonsterDataModelBySceneId((int)sceneNpcDataModel.SceneId);
                if(activityMonsterDataModelList == null || activityMonsterDataModelList.Count <= 0)
                    continue;

                //当前场景中存在活动类型的怪物， 找到最近的一个
                var curSceneId = sceneNpcDataModel.SceneId;
                var path = clientSystemTown.GetMovePath((int)curSceneId);
                if (nearestByPathList == null)
                {
                    nearestByPathList = path;
                    nearestBySceneId = (int)curSceneId;
                }
                else
                {
                    if (path.Count > 0 && path.Count < nearestByPathList.Count)
                    {
                        nearestByPathList = path;
                        nearestBySceneId = (int)curSceneId;
                    }
                }
            }

            //不存在包含怪物的相邻场景，直接返回
            if(nearestByPathList == null || nearestByPathList.Count <= 0)
                return;
            //相邻场景存在，找到场景中活动类型的怪物列表
            activityMonsterDataModelList = GetActivityMonsterDataModelBySceneId(nearestBySceneId);
            if (activityMonsterDataModelList != null && activityMonsterDataModelList.Count > 0)
            {
                monsterGuid = GetSceneNpcGuidByRandom(activityMonsterDataModelList);
                return;
            }
        }

        //在怪物列表中随机选择一个怪物
        private UInt64 GetSceneNpcGuidByRandom(List<SceneNpc> sceneNpcList)
        {
            var count = sceneNpcList.Count;
            var index = UnityEngine.Random.Range(0, count);
            if (index >= count)
                index = count - 1;
            if (index < 0)
                index = 0;

            var sceneNpc = sceneNpcList[index];
            if (sceneNpc == null)
                return 0;
            else
                return sceneNpc.guid;
        }

        //将服务器的怪物位置转换为当前场景的位置坐标
        public Vector3 GetAttackCityMonsterScenePosition(NpcPos npcPos)
        {
            //将服务器传过来的位置坐标npcPos(x,y)转化为场景中实际的位置moveData.position(x,y,z);
            return new Vector3((float)npcPos.x / NpcPositionCoefficient,
                0,
                (float)npcPos.y / NpcPositionCoefficient);
        }

        //相应场景中是否包含对应怪物
        //sceneId = 0的时候，在全部场景中查找
        //sceneId != 0的时候，在某个特定场景中查找
        public bool IsSceneDataContainNpcData(UInt64 guid, UInt32 sceneId = 0)
        {

            for (var i = 0; i < _sceneNpcDataModelList.Count; i++)
            {
                //查找特定场景中的是否包含guid
                if (sceneId != 0 && _sceneNpcDataModelList[i].SceneId != sceneId)
                    continue;

                for (var j = 0; j < _sceneNpcDataModelList[i].NpcsList.Count; j++)
                {
                    if (_sceneNpcDataModelList[i].NpcsList[j].guid == guid)
                        return true;
                }
            }
            return false;
        }

        //场景中是否包含活动类型的攻城怪物
        private bool IsExistActivityTypeCityMonster()
        {
            if (_sceneNpcDataModelList == null || _sceneNpcDataModelList.Count <= 0)
                return false;

            for (var i = 0; i < _sceneNpcDataModelList.Count; i++)
            {
                if (_sceneNpcDataModelList[i].NpcsList != null
                    && _sceneNpcDataModelList[i].NpcsList.Count > 0)
                {
                    for (var j = 0; j < _sceneNpcDataModelList[i].NpcsList.Count; j++)
                    {
                        if ((CityMonsterGenerate.eMonsterType) _sceneNpcDataModelList[i].NpcsList[j].funcType
                            == CityMonsterGenerate.eMonsterType.Activity)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        //判断字符串是否为怪物攻城字符串，用于判断活动副本
        public bool IsAttackCityMonsterStr(string judgeStr)
        {
            if (string.IsNullOrEmpty(judgeStr) == true)
                return false;
            if (judgeStr == AttackCityMonsterStr)
                return true;
            return false;
        }

        public string GetMissionNpcName(Dictionary<string, string> content)
        {
            if (content == null || content.Count <= 0)
                return "";

            string npcData;
            if (content.TryGetValue(NetNpcDataStr, out npcData) == true)
            {
                var npcId = int.Parse(npcData);
                if (npcId <= 0)
                    return "";

                var npcItem = TableManager.GetInstance().GetTableItem<NpcTable>(npcId);
                if (npcItem == null)
                    return "";

                //"<color=#13ff82>名字</color>"
                var npcName = string.Format("<color={0}>{1}</color>",
                    TR.Value("parse_color_npc"),
                    npcItem.NpcName);
                return npcName;
            }
            return "";
        }
        #endregion

        #region AttackCityMonsterBeatTimes

        private void OnCountValueChanged(UIEvent uiEvent)
        {
            if (uiEvent == null)
                return;

            var countKeyStr = uiEvent.Param1 as string;
            if (string.IsNullOrEmpty(countKeyStr) == true)
                return;

            if (string.Equals(countKeyStr, NetAttackCityMonsterTimesStr) == false)
                return;

            var curBeatTime = CountDataManager.GetInstance().GetCount(countKeyStr);
            _attackCityMonsterBeatTimes = curBeatTime;
            ShowAttackCityMonsterBeatTimes();
        }

        private void ShowAttackCityMonsterBeatTimes()
        {
            if(_attackCityMonsterTotalTimes <= 0)
                return;

            if(_attackCityMonsterBeatTimes <= 0)
                return;

            var systemBattle = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemBattle;
            //战斗系统，战斗刚刚结束,在战斗系统中，才显示
            if (systemBattle != null)
            {
                string resultStr = null;
                if (_attackCityMonsterBeatTimes < _attackCityMonsterTotalTimes)
                {
                    resultStr = string.Format(TR.Value("monster_attack_city_less_ten_times"),
                        _attackCityMonsterBeatTimes, _attackCityMonsterTotalTimes);
                }
                else
                {
                    resultStr = string.Format(TR.Value("monster_attack_city_more_ten_times"),
                        _attackCityMonsterTotalTimes);
                }

                if (string.IsNullOrEmpty(resultStr) == true)
                    return;
                //飘字
                SystemNotifyManager.SysNotifyFloatingEffect(resultStr);
            }
        }

        public bool IsAlreadyFinishTotalBeatTimes()
        {
            if (_attackCityMonsterTotalTimes <= 0)
                return false;

            if (_attackCityMonsterBeatTimes < _attackCityMonsterTotalTimes)
                return false;

            return true;
        }

        public int GetLeftBeatTimes()
        {
            int leftBeatTimes = _attackCityMonsterTotalTimes - _attackCityMonsterBeatTimes;
            if (leftBeatTimes <= 0)
                return 0;

            return leftBeatTimes;
        }

        #endregion
    }
}