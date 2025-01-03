using System;
using System.Collections.Generic;
using System.Collections;
///////删除linq
using System.Text;
using Network;
using Protocol;
using UnityEngine;
using UnityEngine.UI;
using System.ComponentModel;
using System.Reflection;

namespace GameClient
{
    class ClientSystemTestBase
    {
//        protected IEnumerator _wait_for_system<T>() where T : ClientSystem
//        {
//            while (!(ClientSystemManager.instance.CurrentSystem is T))
//            {
//                yield return Yielders.EndOfFrame;
//            }
//        }
//
//        protected IEnumerator _wait_for_frame_open(Type type)
//        {
//            while (!(ClientSystemManager.instance.IsFrameOpen(type)))
//            {
//                yield return Yielders.EndOfFrame;
//            }
//        }
//
//        protected T _get_frame<T>() where T : ClientFrame
//        {
//            return ClientSystemManager.instance.GetFrame(typeof(T)) as T;
//        }
//
//        protected T _get_system<T>() where T : ClientSystem
//        {
//            return ClientSystemManager.instance.CurrentSystem as T;
//        }
//
//        protected void _set_input(object obj, string name, string val)
//        {
//            Type type = obj.GetType();
//
//            try
//            {
//                FieldInfo info = type.GetField(name, BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);
//                InputField field = info.GetValue(obj) as InputField;
//                field.text = val;
//            }
//            catch (Exception e)
//            {
//                throw new Exception(string.Format("设置输入{0}:{1}出错 {2}", name, val, e.ToString()));
//            }
//        }
//
//        protected void _call_methmod(object obj, string name)
//        {
//            Type type = obj.GetType();
//            try
//            {
//                type.InvokeMember(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod | BindingFlags.Instance, null, obj, new object[] {});
//            }
//            catch (Exception e)
//            {
//                throw new Exception(string.Format("调用方法{0}出错 {1}", name, e.ToString()));
//            }
//        }
//    }
//
//    class ClientSystemLoginTest : ClientSystemTestBase
//    {
//        private IEnumerator _login(string acc, string psw)
//        {
//            ClientSystemLogin login = _get_system<ClientSystemLogin>(); 
//
//            _set_input(login, "account", acc);
//
//            _set_input(login, "password", psw);
//
//            yield return Yielders.EndOfFrame;
//
//            _call_methmod(login, "Login");
//        }
//
//        private IEnumerator _create_role(string name, int type)
//        {
//            yield return _wait_for_frame_open(typeof(CreateActorFrame));
//
//            var frame = _get_frame<CreateActorFrame>();
//            frame.OnClickCreate();
//
//            yield return Yielders.EndOfFrame;
//        }
//
//        private IEnumerator _select_role(int idx)
//        {
//            var res = new GateSendRoleInfo();
//            var msg = new MessageEvents();
//
//            yield return MessageUtility.Wait<GateSendRoleInfo, GateSendRoleInfo>(
//                    ServerType.GATE_SERVER,
//                    msg,
//                    null,
//                    res
//                    );
//
//            if (res.roles.Length == 0)
//            {
//                yield return _create_role("", 0);
//            }
//
//            yield return _wait_for_frame_open(typeof(SelectRoleFrame));
//
//            var selectFrame = _get_frame<SelectRoleFrame>();
//
//            yield return Yielders.EndOfFrame;
//
//            _call_methmod(selectFrame, "OnEnterGame");
//        }
//
//        public IEnumerator Login(string acc, string psw)
//        {
//            while (true)
//            {
//                yield return _wait_for_system<ClientSystemLogin>();
//                yield return _login(acc, psw);
//                yield return _select_role(0);
//                yield return _wait_for_system<ClientSystemTown>();
//            }
//        }
//    }
//
//    /// <summary>
//    /// 测试代码，暂定测试任务流程，测试普通关卡
//    /// </summary>
//    [LoggerModel("TestBattleNormal")]
//    class ClientSystemBattleTest : ClientSystemTestBase
//    {
//        private enum eExceptionWarning
//        {
//            [Description("无法进入副本")]
//            CAN_NOT_ENTER_DUNGON,
//
//            [Description("AreaID重复")]
//            AREA_REPLEAT,
//
//            [Description("多个开始Area")]
//            AREA_MUTI_START,
//
//            [Description("任务失败")]
//            MISSION_FAILED,
//        }
//
//        private System.IO.Stream mProcessStream;
//        private System.IO.Stream mErrorStream;
//
//        public ClientSystemBattleTest()
//        {
//        }
//
//        public ClientSystemBattleTest(System.IO.Stream process, System.IO.Stream error)
//        {
//            mProcessStream = process;
//            mErrorStream = error;
//        }
//
//        private void _logError(string str, params object[] args)
//        {
//            Logger.LogErrorFormat(str, args);
//
//            if (null != mErrorStream)
//            {
//            }
//        }
//
//        private void _logProcess(string str, params object[] args)
//        {
//            Logger.LogProcessFormat(str, args);
//
//            if (null != mProcessStream)
//            {
//
//            }
//        }
//
//        private IEnumerator _changeScene(int currSceneID, int currDoorID, int targetSceneID, int targetDoorID)
//        {
//            SceneNotifyEnterScene notify = new SceneNotifyEnterScene();
//            ScenePlayerChangeSceneReq changeScene = new ScenePlayerChangeSceneReq();
//            
//            changeScene.curDoorId = (uint)currDoorID;
//            changeScene.dstSceneId = (uint)targetSceneID;
//            changeScene.dstDoorId = (uint)targetDoorID;
//
//            var rmsg = new MessageEvents();
//
//            yield return (MessageUtility.Wait<ScenePlayerChangeSceneReq, SceneNotifyEnterScene>(ServerType.GATE_SERVER, rmsg, changeScene, notify));
//        }
//
//        /// <summary>
//        /// 进入选择关卡场景
//        /// </summary>
//        /// <returns></returns>
//        private IEnumerator _chapterSelectEnter()
//        {
//            _logProcess("进入关卡选择场景");
// 
//            yield return (_changeScene(6001, 2, 6002, 1));
//        }
//
//        /// <summary>
//        /// 退出选择关卡场景
//        /// </summary>
//        /// <returns></returns>
//        private IEnumerator _chapterSelectExit()
//        {
//            _logProcess("退出关卡选择场景");
//
//            yield return Yielders.EndOfFrame;
//            ClientSystemManager.instance.SwitchSystem<ClientSystemTown>();
//
//            yield return _debug_wait_system_town();
//        }
//
//        /// <summary>
//        /// 进入战斗流程
//        /// </summary>
//        /// <param name="dungeonID"></param>
//        /// <returns></returns>
//        private IEnumerator _battleEnter(int dungeonID, bool ishell = false)
//        {
//            _logProcess("请求进入战斗");
//
//            DungeonID id = new DungeonID(dungeonID);
//
//            SceneDungeonStartRes res = new SceneDungeonStartRes();
//            SceneDungeonStartReq req = new SceneDungeonStartReq();
//            req.dungeonId = (uint)(id.dungeonID);
//            var msg = new MessageEvents();
//
//            yield return MessageUtility.Wait<SceneDungeonStartReq, SceneDungeonStartRes>(ServerType.GATE_SERVER, msg, req, res);
//
//            if (res.result != 0)
//            {
//                Logger.LogErrorCode(res.result);
//                // TODO send a email to duanduan
//                // need dump the ChapterInfo & DungeonInfo
//                Logger.LogWarning(ObjectDumper.Dump(BattleDataManager.GetInstance().ChapterInfo));
//                throw new Exception(eExceptionWarning.CAN_NOT_ENTER_DUNGON.GetDescription());
//            }
//            else
//            {
//                _logProcess("初始化战斗数据");
//                //BattleDataManager.GetInstance().ConvertDungeonBattleInfo(res);
//                //BattleDataManager.GetInstance().InitSoloDungeon(res);
//            }
//        }
//
//        private IEnumerator _battleBegin()
//        {
//            yield break;
//        }
//
//        private void _checkMonsterData(int monsterID)
//        {
//            var monsterLevel = monsterID % 10000 / 100 % 100;
//            var originMonsterID = monsterID;
//            monsterID = monsterID / 10000 * 10000 + monsterID % 100;
//
//            var monsterData = TableManager.instance.GetTableItem<ProtoTable.UnitTable>(monsterID);
//            
//            if (null != monsterData)
//            {
//                _logProcess("ID {0}({1}), 怪物 {2}, 等级 {3}, 经验 {4}, 掉落 {5}", originMonsterID, monsterID, monsterData.Name, monsterLevel, monsterData.Exp, monsterData.DropID);
//            }
//            else 
//            {
//                _logError("怪物表中没有怪物 {0}", monsterID);
//            }
//        }
//
//        private IEnumerator _battleKillMonsters(int areaID)
//        {
//            var areaList = BattleDataManager.GetInstance().BattleInfo.areas;
//            var item = areaList.Find(x => { return x.id == areaID; });
//
//            if (null == item)
//            {
//                _logError("无法找到区域ID {0}", areaID);
//            }
//            else
//            {
//                List<uint> monsterList = new List<uint>();
//
//                _logProcess("杀死怪物");
//
//                for (int i = 0; i < item.monsters.Count; ++i)
//                {
//                    monsterList.Add((uint)item.monsters[i].id);
//                    _checkMonsterData(item.monsters[i].typeId);
//                }
//
//                SceneDungeonKillMonsterReq req = new SceneDungeonKillMonsterReq();
//                SceneDungeonKillMonsterRes res = new SceneDungeonKillMonsterRes();
//                var msg = new MessageEvents();
//                req.monsterIds = monsterList.ToArray();
//
//                yield return MessageUtility.Wait<SceneDungeonKillMonsterReq, SceneDungeonKillMonsterRes>(ServerType.GATE_SERVER, msg, req, res);
//            }
//
//            yield break;
//        }
//
//        private IEnumerator _battleChangeArea(int nextAreaID)
//        {
//            var areaList = BattleDataManager.GetInstance().BattleInfo.areas;
//
//            BattleDataManager.GetInstance().BattleInfo.areaId = nextAreaID;
//
//            var idx = areaList.Find(x => { return x.id == nextAreaID; });
//
//            if (null == idx)
//            {
//                _logError("无法找到区域ID {0}", nextAreaID);
//            }
//            else
//            {
//                _logProcess("跳转到区域 {0}", nextAreaID);
//
//                var msg = new SceneDungeonEnterNextAreaReq();
//                msg.areaId = (uint)(nextAreaID);
//                var rsg = new MessageEvents();
//
//                yield return MessageUtility.Wait<SceneDungeonEnterNextAreaReq, SceneDungeonEnterNextAreaReq>(ServerType.GATE_SERVER, rsg, msg, null);
//            }
//
//            yield break;
//        }
//
//        private void _checkItemData(int itemID, int num)
//        {
//            var itemData = TableManager.instance.GetTableItem<ProtoTable.ItemTable>(itemID);
//            if (null != itemData)
//            {
//                _logProcess("ID {0}, 道具 {1}, 数量 {2}", itemID, itemData.Name, num);
//            }
//            else 
//            {
//                _logError("掉落道具没有 {0}", itemID);
//            }
//        }
//
//        private IEnumerator _battlePickDrops(int areaID)
//        {
//
//            var areaList = BattleDataManager.GetInstance().BattleInfo.areas;
//            var item = areaList.Find(x => { return x.id == areaID; });
//
//            if (null == item)
//            {
//                _logError("无法找到区域ID {0}", areaID);
//            }
//            else
//            {
//                List<uint> drops = new List<uint>();
//
//                _logProcess("拣取所有掉落");
//
//                for (int i = 0; i < item.monsters.Count; ++i)
//                {
//                    for (int j = 0; j < item.monsters[i].dropItems.Count; ++j)
//                    {
//                        var dropNode = item.monsters[i].dropItems[j];
//
//                        drops.Add((uint)dropNode.id);
//
//                        _checkItemData(dropNode.typeId, dropNode.num);
//                    }
//                }
//
//
//                var req = new SceneDungeonRewardReq();
//                var res = new SceneDungeonRewardRes();
//
//                req.dropItemIds = drops.ToArray();
//
//                var rmsg = new MessageEvents();
//                yield return (MessageUtility.Wait<SceneDungeonRewardReq, SceneDungeonRewardRes>(ServerType.GATE_SERVER, rmsg, req, res));
//            }
//
//            yield break;
//        }
//
//        private IEnumerator _battleEnd()
//        {
//            var res = new SceneDungeonRaceEndRes();
//            var req = new SceneDungeonRaceEndReq();
//            req.beHitCount = 100;
//            req.usedTime = 1000;
//            req.score = (byte)1;
//            var msg = new MessageEvents();
//            yield return (MessageUtility.Wait<SceneDungeonRaceEndReq, SceneDungeonRaceEndRes>(ServerType.GATE_SERVER, msg, req, res));
//        }
//
//        private IEnumerator _battleOpenChest(int missionID)
//        {
//            _logProcess("开始翻牌 {0}", missionID);
//
//            var req = new SceneDungeonOpenChestReq();
//            req.pos = 1;
//
//            var res = new SceneDungeonOpenChestRes();
//            var msg = new MessageEvents();
//
//            yield return MessageUtility.Wait<SceneDungeonOpenChestReq, SceneDungeonOpenChestRes>(ServerType.GATE_SERVER, msg, req, res);
//        }
//        
//        /// <summary>
//        /// 接取任务
//        /// </summary>
//        /// <param name="missionID"></param>
//        /// <returns></returns>
//        private IEnumerator _pickupMission(int missionID)
//        {
//            _logProcess("接取任务 {0}", missionID);
//
//            var missionTable = TableManager.instance.GetTableItem<ProtoTable.MissionTable>(missionID);
//            SceneAcceptTaskReq kCmd = new SceneAcceptTaskReq();
//            kCmd.taskID = (uint)missionID;
//            kCmd.npcID = (uint)missionTable.MissionTakeNpc;
//            kCmd.acceptType = (byte)(missionTable.AcceptType);
//            var msg = new MessageEvents();
//
//            yield return (MessageUtility.Wait<SceneAcceptTaskReq, SceneAcceptTaskReq>(ServerType.GATE_SERVER, msg, kCmd, null));
//
//            //NetManager.Instance().SendCommand<SceneAcceptTaskReq>(ServerType.GATE_SERVER, kCmd);
//            //MissionManager.GetInstance().sendCmdAcceptTask((UInt32)missionID, (TaskSubmitType)(missionTable.FinishType), (uint)missionTable.MissionTakeNpc);
//            yield return Yielders.GetWaitForSeconds(0.2f);
//
//            MissionManager.GetInstance().CloseAllDialog();
//        }
//
//
//        /// <summary>
//        /// 提交任务
//        /// </summary>
//        /// <param name="missionID"></param>
//        /// <returns></returns>
//        private IEnumerator _submmitMission(int missionID)
//        {
//            _logProcess("提交任务 {0}", missionID);
//
//            var missionTable = TableManager.instance.GetTableItem<ProtoTable.MissionTable>(missionID);
//
//            SceneSubmitTaskReq kCmd = new SceneSubmitTaskReq();
//            kCmd.taskID = (UInt32)missionID;
//            kCmd.npcID = (uint)missionTable.MissionFinishNpc;
//            kCmd.submitType = (byte)((Protocol.TaskSubmitType)missionTable.FinishType);
//            var msg  = new MessageEvents();
//
//            yield return (MessageUtility.Wait<SceneSubmitTaskReq, SceneSubmitTaskReq>(ServerType.GATE_SERVER, msg, kCmd, null));
//
//            yield return Yielders.GetWaitForSeconds(0.2f);
//            MissionManager.GetInstance().CloseAllDialog();
//        }
//
//        private IEnumerator _checkLevel(int level)
//        {
//            _logProcess("需求等级 {0}, 当前等级 {1}", level, PlayerBaseData.GetInstance().Level);
//
//            if (PlayerBaseData.GetInstance().Level < level)
//            {
//                _logProcess("升级至 {0}", level);
//
//                var expTable = TableManager.instance.GetTableItem<ProtoTable.ExpTable>(level);
//
//                yield return (_addExp((ulong)expTable.TotalExp - PlayerBaseData.GetInstance().Exp + 1));
//            }
//
//            while (PlayerBaseData.GetInstance().Level < level)
//            {
//                yield return Yielders.EndOfFrame;
//            }
//        }
//
//        private IEnumerator _checkFigure()
//        {
//            _logProcess("当前疲劳 {0}", PlayerBaseData.GetInstance().fatigue);
//
//            if (PlayerBaseData.GetInstance().fatigue <= 0)
//            {
//                int count = 100;
//                _logProcess("增加疲劳 {0}", count);
//                yield return _addFigure(count);
//            }
//
//            while (PlayerBaseData.GetInstance().fatigue <= 0)
//            {
//                yield return Yielders.EndOfFrame;
//            }
//        }
//
//        private string _get_jpg_name(int idx)
//        {
//            int areaID = BattleMain.instance.GetDungeonManager().GetDungeonDataManager().CurrentAreaID();
//            int dungeonID = BattleMain.instance.GetDungeonManager().GetDungeonDataManager().table.ID;
//
//            return string.Format("{0}_{1}_{2}.jpg", dungeonID, idx, areaID);
//        }
//
//        private IEnumerator _debug_close_dialog()
//        {
//            yield return Yielders.GetWaitForSeconds(1.0f);
//
//            if (ClientSystemManager.instance.IsFrameOpen(typeof(GameClient.TaskDialogFrame)))
//            {
//                IGameBind bind = ClientSystemManager.instance.GetFrame(typeof(GameClient.TaskDialogFrame)) as IGameBind;
//
//                if (null != bind)
//                {
//                    UnityEngine.UI.Button button = bind.GetComponent<Button>("BtnStepOver");
//
//                    if (null != button)
//                    {
//                        button.onClick.Invoke();
//                    }
//                }
//
//                yield return Yielders.EndOfFrame;
//
//            }
//
//            yield return Yielders.EndOfFrame;
//        }
//
//        private IEnumerator _debug_start_fight()
//        {
//            yield return _debug_wait_system();
//
//            int idx = 0;
//
//            while (true)
//            {
//                yield return _debug_close_dialog();
//
//                yield return _debug_wait_for_state(BeSceneState.onFight);
//
//                yield return _debug_kill_all_monster();
//
//                Application.CaptureScreenshot(_get_jpg_name(idx++), 0);  
//                
//                if (!BattleMain.instance.GetDungeonManager().GetDungeonDataManager().IsBossArea())
//                {
//                    yield return _debug_wait_for_state(BeSceneState.onClear);
//
//                    yield return _debug_auto_move_next();
//                }
//                else 
//                {
//                    yield return _debug_wait_for_frame(typeof(DungeonMenuFrame));
//                    yield break;
//                }
//            }
//        }
//
//        private IEnumerator _debug_wait_system()
//        {
//            while (BattleMain.instance == null)
//            {
//                yield return Yielders.EndOfFrame;
//            }
//        }
//
//        private IEnumerator _debug_wait_system_town()
//        {
//            while (ClientSystemManager.instance.CurrentSystem == null || !(ClientSystemManager.instance.CurrentSystem is ClientSystemTown))
//            {
//                yield return Yielders.EndOfFrame;
//            }
//        }
//
//        private IEnumerator _debug_kill_all_monster()
//        {
//            BattleMain.instance.GetDungeonManager().GetBeScene().ClearAllCharacter();
//            yield return Yielders.EndOfFrame;
//        }
//
//        private IEnumerator _debug_auto_move_next()
//        {
//            if (!BattleMain.instance.GetDungeonManager().GetDungeonDataManager().IsBossArea())
//            {
//                Vector3 pos = BattleMain.instance.GetDungeonManager().GetDungeonDataManager().CurrentGuideDoorPosition();
//
//                BeActor mainPlayer = BattleMain.instance.GetPlayerManager().GetMainPlayer().playerActor;
//                mainPlayer.SetPosition(new Vec3(pos));
//                yield return Yielders.EndOfFrame;
//
//                yield return _debug_wait_for_state_change(BeSceneState.onClear);
//            }
//
//            yield return Yielders.EndOfFrame;
//        }
//
//        private IEnumerator _debug_wait_for_state_change(BeSceneState state)
//        {
//            while (BattleMain.instance.GetDungeonManager().GetBeScene() == null || BattleMain.instance.GetDungeonManager().GetBeScene().state == state)
//            {
//                yield return Yielders.EndOfFrame;
//            }
//        }
//
//        private IEnumerator _debug_wait_for_frame(Type type)
//        {
//            while (!ClientSystemManager.instance.IsFrameOpen(type))
//            {
//                yield return Yielders.EndOfFrame;
//            }
//        }
//
//
//        private IEnumerator _debug_wait_for_state(BeSceneState state)
//        {
//            while (BattleMain.instance.GetDungeonManager().GetBeScene() == null || BattleMain.instance.GetDungeonManager().GetBeScene().state != state)
//            {
//                yield return Yielders.EndOfFrame;
//            }
//        }
//
//        public IEnumerator TestDungeon(int dungeonID)
//        {
//            var dungeonData = TableManager.instance.GetTableItem<ProtoTable.DungeonTable>(dungeonID);
//
//            if (null != dungeonData)
//            {
//                yield return _chapterSelectEnter();
//
//                yield return _checkFigure();
//
//                yield return _battleEnter(dungeonID);
//
//                yield return _debug_start_fight();
//
//                yield return _chapterSelectExit();
//            }
//
//            yield break;
//        }
//
//        /// <summary>
//        /// 测试
//        /// </summary>
//        public IEnumerator TestMission(int missionID)
//        {
//            var missionData = TableManager.instance.GetTableItem<ProtoTable.MissionTable>(missionID);
//            if (null != missionData)
//            {
//                yield return _checkLevel(missionData.MinPlayerLv);
//
//                yield return _checkFigure();
//
//                yield return _pickupMission(missionID);
//
//                yield return TestDungeon(missionData.MapID);
//
//                yield return _submmitMission(missionID);
//
//                if (!_isMissionFinish(missionID))
//                {
//                    _logError("任务执行失败 {0}", missionID);
//                    throw new Exception(eExceptionWarning.MISSION_FAILED.GetDescription() + " " + missionID);
//                }
//            }
//        }
//
//        private bool _isMissionFinish(int missionID)
//        {
//            var state = MissionManager.GetInstance().GetMissionStatus((UInt32)missionID);
//            return state != TaskStatus.TASK_UNFINISH;
//        }
//
//        private IEnumerator _sendGM(string msg)
//        {
//            SceneChat gm = new SceneChat();
//            gm.channel = 0;
//            gm.targetId = 0;
//            gm.word = string.Format("!!{0}", msg);
//
//            var rmsg = new MessageEvents();
//            yield return (MessageUtility.Wait<SceneChat, SceneChat>(ServerType.GATE_SERVER , rmsg, gm, null));
//            yield return Yielders.GetWaitForSeconds(0.2f);
//        }
//
//        private IEnumerator _addItem(int item, int num)
//        {
//            yield return _sendGM(string.Format("additem id={0} num={1}", item, num));
//        }
//
//        private IEnumerator _addExp(ulong exp)
//        {
//            yield return _sendGM(string.Format("addexp num={0}", exp));
//        }
//
//        private IEnumerator _addFigure(int count)
//        {
//            yield return _sendGM(string.Format("addfatigue num={0}", count));
//        }
//
//
//        public IEnumerator TestChapter()
//        {
//            yield return (_addItem(400000002, 1));
//
//            for (int i = 1; i <= 30; i++)
//            {
//                yield return (TestMission(2000+i));
//            }
//        }
//
//        public IEnumerator TestChapterFollowMain(int startMissionID)
//        {
//            //yield return (_addItem(400000002, 1));
//            //
//            //
//            
//            List<int> missionList = new List<int>();
//
//            //int startMissionID = MissionManager.GetInstance().GetMainTask();
//
//            int finalMission = 2246; 
//
//            ProtoTable.MissionTable missionData = null;
//            do
//            {
//                missionData = TableManager.instance.GetTableItem<ProtoTable.MissionTable>(finalMission);
//                missionList.Add(finalMission);
//                finalMission = missionData.PreTaskID;
//            }
//            while (finalMission > 0);
//            missionList.Reverse();
//
//            var startTime = DateTime.Now;
//
//            int startIdx = -1;
//            while (startIdx < missionList.Count && missionList[++startIdx] != startMissionID) { ; }
//
//            _logProcess("开始测试主线任务流程 idx {0}", startIdx);
//            _logProcess("开始测试主线任务流程 id {0}", missionList[startIdx]);
//
//            for (int i = startIdx; startIdx < missionList.Count; ++i)
//            {
//
//                missionData = TableManager.instance.GetTableItem<ProtoTable.MissionTable>(missionList[i]);
//                if (null != missionData)
//                {
//                    yield return (TestMission(missionList[i]));
//                }
//
//                _logProcess("用时 {0}", (DateTime.Now - startTime).TotalSeconds);
//
//                //if (missionList[i] == toMissionID)
//                //{
//                //    break;
//                //}
//            }
//
//            var endTime = DateTime.Now;
//
//            _logProcess("开始时间 {0}, 结束时间 {1}, 经历时间 {2}", startTime.ToLocalTime(), endTime.ToLocalTime(), (endTime - startTime).TotalSeconds);
//        }
    }
}
