using System;
using System.Collections.Generic;
using ProtoTable;
using Protocol;
using Network;
using System.Text;
using UnityEngine;

namespace GameClient
{
#if ROBOT_TEST
    public enum RunningState
    {
        Ready,
        Wait, //等待开始战斗协议
        Running,
        End,
    }
    public class RunTimeDungeon
    {
        public int dungeonId;   //地下城id
        public float durTime;   //当前状态下持续时间
        public string dungeonName; //地下城房间名称
        public bool isValid;  //是否加入循环执行列表
        public int tryCount;//当发送开始战斗协议后等待时间到了，重试次数
        public RunningState stat; //当前房间执行状态
    }

    /// <summary>
    /// 自动战斗编辑时数据
    /// </summary>
    public class AutoFightTestDataManager : Singleton<AutoFightTestDataManager>
    {
        private bool m_start = false;

        //章节菜单数据
        protected List<AutoFightTest> m_ChapterIdList = new List<AutoFightTest>();
        public List<AutoFightTest> ChapterIdList
        {
            get { return m_ChapterIdList; }
        }

        //章节序号作为索引 对应地下城数据列表
        protected Dictionary<int, List<AutoFightTest>> m_ChapterDataDic = new Dictionary<int, List<AutoFightTest>>();
        public Dictionary<int, List<AutoFightTest>> ChapterDataDic
        {
            get { return m_ChapterDataDic; }
        }

        //当前选中的关卡
        protected Dictionary<int, string> m_SelectDungeonDic = new Dictionary<int, string>();
        public Dictionary<int, string> SelectDungeonDic
        {
            get { return m_SelectDungeonDic; }
            set { m_SelectDungeonDic = value; }
        }

        //当前选中的章节
        protected int m_SelChapterId = -1;
        public int SelChapterId
        {
            get { return m_SelChapterId; }
            set { m_SelChapterId = value; }
        }

        protected string m_LastRunningDungeonName;
        public string LastRunningDungeonName
        {
            get { return m_LastRunningDungeonName; }
            set { m_LastRunningDungeonName = value; }
        }

        public bool IsStart
        {
            get { return m_start; }
            set
            {
                m_start = value;
                if (!m_start)
                {
                    //结束战斗
                    AutoFightRunTime.GetInstance().Clear();
                }
            }
        }

        public override void Init()
        {
            m_Content = StringBuilderCache.Acquire(1024 * 1024);
            InitTableData();
        }

        /// <summary>
        /// 初始化表格数据
        /// </summary>
        protected void InitTableData()
        {
            m_ChapterDataDic.Clear();
            m_ChapterIdList.Clear();

            var tableDic = TableManager.instance.GetTable<AutoFightTest>();
            var enumerator = tableDic.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var table = enumerator.Current.Value as AutoFightTest;
                if (table == null)
                    return;
                var chapterId = table.ChapterId;
                if (table.DungeonId != 0)
                {
                    if (m_ChapterDataDic.ContainsKey(chapterId))
                    {
                        m_ChapterDataDic[chapterId].Add(table);
                    }
                    else
                    {
                        List<AutoFightTest> list = new List<AutoFightTest>();
                        list.Add(table);
                        m_ChapterDataDic.Add(chapterId, list);
                    }
                }
                else
                {
                    m_ChapterIdList.Add(table);
                }
            }
        }
        
        protected string m_AutoFightTestPath = "/AutoFightTest.txt";
        protected StringBuilder m_Content = null;

        protected float m_MaxMemoryInBattle = 0;
        public float MaxMemoryInBattle { get { return m_MaxMemoryInBattle; } set { m_MaxMemoryInBattle = value; } }

        protected int m_CurBattleRunCount = 0;
        public int CurBattleRunCount { get { return m_CurBattleRunCount; } set { m_CurBattleRunCount = value; } }

        /// <summary>
        /// 记录战斗相关信息
        /// </summary>
        /// <param name="isStart">战斗开始</param>
        public void RecordBattleData(bool isStart = false)
        {
            if (BattleMain.instance == null)
                return;
            if (BattleMain.instance.GetDungeonManager() == null)
                return;
            if (BattleMain.instance.GetDungeonManager().GetDungeonDataManager() == null)
                return;
            int id = BattleMain.instance.GetDungeonManager().GetDungeonDataManager().id.dungeonID;
            var dungeonTable = BattleMain.instance.GetDungeonManager().GetDungeonDataManager().table;
            if (dungeonTable == null)
                return;
            m_Content.Clear();
            if (isStart)
            {
                m_Content.AppendFormat("{0}Battle Start: DungeonId:{1} Name:{2} ", "\n\n", id, dungeonTable.Name);
            }
            else
            {
                m_Content.AppendFormat("{0}Battle End: DungeonId:{1} Name:{2} ", "\n", id, dungeonTable.Name);
            }
            //string availMemory = PluginManager.instance.GetAvailMemory();
            //string currentProcess = PluginManager.instance.GetCurrentProcessMemory();
            //string monoMemory="0";
            string monoMemory = GetMonoTotalMemory().ToString("0.0") + "M";
            string maxInBattle = m_MaxMemoryInBattle.ToString("0.0");
            //m_Content.AppendFormat("Avail:{0} Current:{1} Mono:{2} MaxInBattle:{3} ", availMemory, currentProcess, monoMemory, maxInBattle);
            m_Content.AppendFormat("monoMemory:{0},MaxInBattle:{1}", monoMemory, maxInBattle);
            m_Content.AppendFormat("Time:{0}", DateTime.Now);
            BeUtility.SaveDataToFile(_GetRootPath() + m_AutoFightTestPath, m_Content.ToString());
        }

        private string _GetRootPath()
        {
            #if !UNITY_EDITOR && UNITY_STANDALONE
            return ".";
            #else
            return Application.persistentDataPath;
            #endif
        }

        /// <summary>
        /// 获取Mono总内存
        /// </summary>
        /// <returns></returns>
        public float GetMonoTotalMemory()
        {
            return GC.GetTotalMemory(false) / (1024.0f * 1024);
        }

        /// <summary>
        /// 检查各种物品的数量
        /// </summary>
        public void CheckItemCountInAutoFightTest()
        {
            //ChatManager.GetInstance().SendChat(ChatType.CT_WORLD, string.Format("!!uplevel num={0}", 60));
            //ChatManager.GetInstance().SendChat(ChatType.CT_WORLD, "!!fatask");

            //加深渊票
            int shenyuanCount = ItemDataManager.GetInstance().GetItemCount(200000004);
            if (shenyuanCount < 50)
            {
                ChatManager.GetInstance().SendChat(ChatType.CT_WORLD, string.Format("!!additem id={0} num={1}", 200000004, 999));
            }

            //加远古票
            int yuanguCount = ItemDataManager.GetInstance().GetItemCount(200000003);
            if (yuanguCount < 50)
            {
                ChatManager.GetInstance().SendChat(ChatType.CT_WORLD, string.Format("!!additem id={0} num={1}", 200000003, 999));
            }

            //加无色晶体
            int wuseCount = ItemDataManager.GetInstance().GetItemCount(300000106);
            if (wuseCount < 50)
            {
                ChatManager.GetInstance().SendChat(ChatType.CT_WORLD, string.Format("!!additem id={0} num={1}", 300000106, 999));
            }

            //加疲劳
            int pilaoCount = PlayerBaseData.GetInstance().fatigue;
            if (pilaoCount < 20)
            {
                ChatManager.GetInstance().SendChat(ChatType.CT_WORLD, string.Format("!!addfatigue num={0}", 100));
            }

            //加复活币
            int fuhuoCount = ItemDataManager.GetInstance().GetItemCount(600000006);
            if (fuhuoCount < 20)
            {
                ChatManager.GetInstance().SendChat(ChatType.CT_WORLD, string.Format("!!additem id={0} num={1}", 600000006, 999));
            }

            //回血药
            int hpCount = ItemDataManager.GetInstance().GetItemCount(20000100);
            if (hpCount < 20)
            {
                ChatManager.GetInstance().SendChat(ChatType.CT_WORLD, string.Format("!!additem id={0} num={1}", 20000100, 999));
            }
        }
    }

    //自动战斗运行时
    public class AutoFightRunTime : Singleton<AutoFightRunTime>
    {
        private List<RunTimeDungeon> m_RunTimeDungeonCmd = new List<RunTimeDungeon>();
        private int m_curCmdIndex = -1;

        /// <summary>
        /// 生成自动战斗相关数据
        /// </summary>
        public void BuildRunTimeData(Dictionary<int, string> dungeonIdDic)
        {
            var enumerator = dungeonIdDic.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var current = enumerator.Current;

                var dungeonData = new RunTimeDungeon();
                dungeonData.dungeonId = current.Key;
                dungeonData.stat = RunningState.Ready;
                dungeonData.durTime = 0;
                dungeonData.isValid = true;

                dungeonData.dungeonName = current.Value;
                m_RunTimeDungeonCmd.Add(dungeonData);

            }
        }

        public void Clear()
        {
            m_RunTimeDungeonCmd.Clear();
            m_curCmdIndex = -1;
        }
        public string GetOutputMsg()
        {
            if (m_RunTimeDungeonCmd.Count <= 0) return "暂无";
            if (m_curCmdIndex == -1) return "暂无_1";
            var cmd = m_RunTimeDungeonCmd[m_curCmdIndex];
            return string.Format("Index :{0} Stat :{1} DungoneId : {2} Name {3} Time {4} TryCount {5}",
                            m_curCmdIndex,
                            cmd.stat,
                            cmd.dungeonId,
                            cmd.dungeonName,
                            cmd.durTime,
                            cmd.tryCount);

        }
        public void Update(float deltaTime)
        {
            if (m_RunTimeDungeonCmd.Count <= 0) return;
            if (m_curCmdIndex == -1)
            {
                m_curCmdIndex = 0;
            }
            var cmd = m_RunTimeDungeonCmd[m_curCmdIndex];
            if (cmd == null) return;
            if (!cmd.isValid)
            {
                m_curCmdIndex++;
                m_curCmdIndex %= m_RunTimeDungeonCmd.Count;
                return;
            }
            if (cmd.stat == RunningState.Ready)
            {
                SendCommand(cmd);
                cmd.stat = RunningState.Wait;
                cmd.durTime = 0;
                cmd.tryCount = 0;
            }
            else if (cmd.stat == RunningState.Wait)
            {
                cmd.durTime += deltaTime;
                if (cmd.durTime >= 60.0f)
                {
                    cmd.tryCount++;
                    cmd.durTime = 0;
                    if (cmd.tryCount > 5)
                    {
                        cmd.stat = RunningState.End;
                    }
                    else
                    {
                        SendCommand(cmd);
                    }
                }
            }
            else if (cmd.stat == RunningState.End)
            {
                cmd.durTime += deltaTime;
                if (cmd.durTime >= 10.0f)
                {
                    cmd.stat = RunningState.Ready;
                    m_curCmdIndex++;
                    m_curCmdIndex %= m_RunTimeDungeonCmd.Count;
                }
            }
        }

        public void OnDungeonStart()
        {
            if (m_curCmdIndex == -1 || m_RunTimeDungeonCmd.Count <= 0) return;
            var cmd = m_RunTimeDungeonCmd[m_curCmdIndex];
            cmd.stat = RunningState.Running;
        }

        public void OnReturnToTown()
        {
            if (m_curCmdIndex == -1 || m_RunTimeDungeonCmd.Count <= 0) return;
            var cmd = m_RunTimeDungeonCmd[m_curCmdIndex];
            cmd.stat = RunningState.End;
            cmd.durTime = 0;
        }

        /// <summary>
        /// 请求进入战斗
        /// </summary>
        protected void SendCommand(RunTimeDungeon cmd)
        {
            //var table = GetTeamDungeonData(cmd.dungeonId);
            //if (table == null)
            //    return;
            //if (table.Type == 1)
            //{
            //    SendTeamDungeonCommand(cmd.dungeonId, cmd.dungeonName);
            //}
            //else
            //{
            //    SendNormalDungeonCommand(cmd.dungeonId, cmd.dungeonName);
            //}
            SendNormalDungeonCommand(cmd.dungeonId, cmd.dungeonName);
        }

        /// <summary>
        /// 发送普通副本命令
        /// </summary>
        protected void SendNormalDungeonCommand(int dungeonId, string dungeonName)
        {
            AutoFightTestDataManager.instance.CurBattleRunCount++;
            SceneDungeonStartReq req = new SceneDungeonStartReq();
            AutoFightTestDataManager.instance.LastRunningDungeonName = dungeonName;
            req.dungeonId = (uint)dungeonId;
            SystemNotifyManager.SysNotifyFloatingEffect(dungeonName);
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
        }

        /// <summary>
        /// 发送团本开战命令
        /// </summary>
        //protected void SendTeamDungeonCommand(int pointId, string dungeonName)
        //{
        //    TeamCopyStartChallengeReq startChallengeReq = new TeamCopyStartChallengeReq();

        //    startChallengeReq.fieldId = (uint)pointId;

        //    var netMgr = NetManager.Instance();
        //    if (netMgr != null)
        //        netMgr.SendCommand(ServerType.GATE_SERVER, startChallengeReq);
        //}

        /// <summary>
        /// 是否是团本副本ID
        /// </summary>
        //protected AutoFightTest GetTeamDungeonData(int id)
        //{
        //    var tableDic = TableManager.instance.GetTable<AutoFightTest>();
        //    var enumerator = tableDic.GetEnumerator();
        //    while (enumerator.MoveNext())
        //    {
        //        var table = enumerator.Current.Value as AutoFightTest;
        //        if (table == null)
        //            continue;
        //        if (table.DungeonId == id)
        //            return table;
        //        continue;
        //    }
        //    return null;
        //}


    }
#endif
}