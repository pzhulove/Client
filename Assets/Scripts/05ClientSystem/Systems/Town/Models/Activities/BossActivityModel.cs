using System;
using System.Collections.Generic;
using ActivityLimitTime;
using GameClient;
using Protocol;
using ProtoTable;
using UnityEngine;

namespace GameClient
{
    public struct BossKillModel : ILimitTimeNote
    {

        public uint Id { get; private set; } //活动id

        public uint StartTime { get; private set; }

        public uint EndTime { get; private set; }

        public string RuleDesc { get; private set; }//规则描述

        public string LogoDesc { get; private set; }//logo描述

        public string Desc { get; private set; }//活动描述

        public string LogoPath { get; private set; }//logo资源路径

        public string NoteBgPath { get; private set; }//logo资源路径

        public string NotePrefabPath { get; private set; }//logo资源路径

        public string ItemPath { get; private set; }//item prefab路径

        public OpActivityState State { get; private set; }//活动状态

        public string Name { get; private set; }//活动名

        public List<BossKillMonsterModel> MonsterDatas { get; private set; }

        private ActivityInfo mData;

        public BossKillModel(WorldActivityMonsterRes monsterData,  ActivityInfo msg) : this()
        {
            if (msg == null)
            {
                return;
            }

            var tableData = TableManager.GetInstance().GetTableItem<ActiveMainTable>((int)msg.id);

            if (tableData == null)
            {
                return;
            }

            Id = msg.id;
            Name = tableData.Name;
            RuleDesc = tableData.PurDesc;
            LogoDesc = tableData.ParticularDesc;
            State = (OpActivityState)msg.state;
            Desc = tableData.Desc;
            LogoPath = tableData.BgPath;
            ItemPath = string.IsNullOrEmpty(tableData.templateName) ? "UIFlatten/Prefabs/OperateActivity/LimitTime/Items/BossKillItem" : tableData.templateName;
            MonsterDatas = new List<BossKillMonsterModel>();
            if (monsterData != null && monsterData.monsters != null)
            {
                for (int i = 0; i < monsterData.monsters.Length; ++i)
                {
                    MonsterDatas.Add(new BossKillMonsterModel(monsterData.monsters[i]));
                }
            }

            mData = msg;
            StartTime = msg.startTime;
            EndTime = msg.dueTime;
            NoteBgPath = null;
            NotePrefabPath = tableData.ActiveFrameTabPath;
        }
    }

    public enum MonsterType
    {
        None = 0,
        Monster_Pos = 3,
        Elite_Pos = 4,
        Boss_Pos = 5,
    }

    public struct BossKillMonsterModel
    {
        public uint Id { get; private set; }//任务id
        public string Name { get; private set; }//boss名字
        public MonsterType MonsterType { get; private set; }//怪物类型:普通、精英、Boss
        public bool IsActive { get; private set; }//此Boss是否开启
        public DropItem[] DropList { get; private set; }//奖励数组
        public UInt32 StartTime { get; private set; }
        public UInt32 EndTime { get; private set; }
        public UInt32 RemainNum { get; private set; }
        public UInt32 NextRollStartTime { get; private set; }

        public BossKillMonsterModel(ActivityMonsterInfo msgData) : this()
        {
            if (msgData == null)
            {
                return;
            }
            Name = msgData.name;
            Id = msgData.id;
            MonsterType = (MonsterType)msgData.pointType;
            DropList = msgData.drops;
            StartTime = msgData.startTime;
            EndTime = msgData.endTime;
            RemainNum = msgData.remainNum;
            NextRollStartTime = msgData.nextRollStartTime;
            IsActive = msgData.activity == 1;
        }
    }

	public class BossExchangeModelBase : ILimitTimeNote
	{

		public uint Id { get; private set; } //活动id

		public uint StartTime { get; private set; }

		public uint EndTime { get; private set; }

		public string RuleDesc { get; private set; }//规则描述

		public string LogoDesc { get; private set; }//logo描述

		public string Desc { get; private set; }//活动描述

		public string LogoPath { get; private set; }//logo资源路径

		public string NoteBgPath { get; private set; }//logo资源路径

		public string NotePrefabPath { get; private set; }//logo资源路径

		public string ItemPath { get; private set; }//item prefab路径

		public OpActivityState State { get; private set; }//活动状态

		public string Name { get; private set; }//活动名

        public string[] StrParam { get; set; }

		public BossExchangeModelBase(ActivityInfo msg)
		{
			if (msg == null)
				return;
			var tableData = TableManager.GetInstance().GetTableItem<ActiveMainTable>((int)msg.id);
			if (tableData == null)
				return;

			Id = msg.id;
			Name = tableData.Name;
			Desc = tableData.Desc;
			RuleDesc = tableData.PurDesc;
			LogoDesc = tableData.ParticularDesc;
			NoteBgPath = null;
			NotePrefabPath = tableData.ActiveFrameTabPath;
			StartTime = msg.startTime;
			EndTime = msg.dueTime;
			LogoPath = tableData.BgPath;
			ItemPath = string.IsNullOrEmpty(tableData.templateName) ? "UIFlatten/Prefabs/OperateActivity/LimitTime/Items/BossExchangeItem" : tableData.templateName;
			State = (OpActivityState)msg.state;
		}

	}

	public class BossExchangeModel : BossExchangeModelBase
	{

		public Dictionary<int, BossExchangeTaskModel> ExchangeTasks { get; private set; }

        public BossExchangeModel(ActivityInfo msg) : base(msg)
        {
            if (msg == null)
                return;
            var tableData = TableManager.GetInstance().GetTableItem<ActiveMainTable>((int)msg.id);
            if (tableData == null)
                return;

            ExchangeTasks = new Dictionary<int, BossExchangeTaskModel>();

            var activeTableAllData = TableManager.GetInstance().GetTable<ActiveTable>();
            foreach (var data in activeTableAllData)
            {
                var activeTableData = data.Value as ActiveTable;
                if (activeTableData != null && activeTableData.TemplateID == msg.id)
                {
                    ExchangeTasks.Add(activeTableData.ID, new BossExchangeTaskModel(activeTableData));
                }
            }

        }

        public void UpdateTask(int taskId)
        {
            if (ExchangeTasks == null)
            {
                ExchangeTasks = new Dictionary<int, BossExchangeTaskModel>();
            }

            var activeTableAllData = TableManager.GetInstance().GetTable<ActiveTable>();
            if (activeTableAllData != null && activeTableAllData.ContainsKey(taskId))
            {
                ExchangeTasks[taskId] = new BossExchangeTaskModel(activeTableAllData[taskId] as ActiveTable);
            }
        }
    }

    public struct BossExchangeTaskModel
    {
        public int Id { get; private set; } //任务id

        public Dictionary<int, int> NeedItems { get; private set; } //需要的物品id,数量

        public Dictionary<int, int> ExchangeItems { get; private set; } //兑换的物品的id,数量

        public int RemainCount { get; private set; }//剩余兑换次数

        public TaskStatus Status { get; private set; }// 任务状态

        public int TaskCycleCount { get; private set; }

        public int AccountTotalSubmitLimit { get; private set; }//账号限制总的次数

        public BossExchangeTaskModel(ActiveTable tableData) : this()
        {
            if (tableData == null)
            {
                return;
            }

            SceneNotifyActiveTaskVar data = ActivityDataManager.GetInstance().GetBossTaskData((uint)tableData.ID);
            SceneNotifyActiveTaskStatus stateData = ActivityDataManager.GetInstance().GetBossTaskStatusData((uint) tableData.ID);

            if (data == null || stateData == null)
            {
                return;
            }

            string[] needItems = tableData.ConsumeItems.Split(',');
            string[] exchangeItems = tableData.Awards.Split(',');

            Id = (int)data.taskId;
            RemainCount = tableData.TaskCycleCount - int.Parse(data.val);
            Status = (TaskStatus)stateData.status;
            TaskCycleCount = tableData.TaskCycleCount;
            AccountTotalSubmitLimit = tableData.AccountTotalSubmitLimit;
            NeedItems = new Dictionary<int, int>(needItems.Length);
            for (int i = 0; i < needItems.Length; ++i)
            {
                string[] id_Count = needItems[i].Split('_');
                NeedItems.Add(int.Parse(id_Count[0]), int.Parse(id_Count[1]));
            }

            ExchangeItems = new Dictionary<int, int>(exchangeItems.Length);
            for (int i = 0; i < exchangeItems.Length; ++i)
            {
                string[] id_Count = exchangeItems[i].Split('_');
                ExchangeItems.Add(int.Parse(id_Count[0]), int.Parse(id_Count[1]));
            }
        }
    }

	public class BossQuestModel : BossExchangeModelBase, ILimitTimeActivityModel
	{
		public uint[] ParamArray { get; private set; }
        public uint[] ParamArray2 { get; private set; }
        public string CountParam { get; private set; }
		public List<ILimitTimeActivityTaskDataModel> TaskDatas { get; private set; }

        public uint Param { get; private set; }

        public string ActivityPrefafPath { get; private set; }//活动的路径(这个活动预制体的路径是给运营活动表里面的活动用的)
        public BossQuestModel(ActivityInfo msg) : base(msg)
		{
			TaskDatas = new List<ILimitTimeActivityTaskDataModel>();
			var activeTableAllData = TableManager.GetInstance().GetTable<ActiveTable>();
			foreach (var data in activeTableAllData)
			{
				var activeTableData = data.Value as ActiveTable;
				if (activeTableData != null && activeTableData.TemplateID == msg.id)
				{
					TaskDatas.Add(new BossQuestTaskDataModel(activeTableData));
				}
			}
		}

		public void UpdateTask(int taskId)
		{
			if (TaskDatas == null)
			{
				TaskDatas = new List<ILimitTimeActivityTaskDataModel>();
			}

			var activeTableAllData = TableManager.GetInstance().GetTable<ActiveTable>();
			if (activeTableAllData != null && activeTableAllData.ContainsKey(taskId))
			{
				for (int i = 0; i < TaskDatas.Count; ++i)
				{
					if (TaskDatas[i].DataId == taskId)
					{
						TaskDatas[i] = new BossQuestTaskDataModel(activeTableAllData[taskId] as ActiveTable);
						break;
					}
				}
			}
		}

		public void SortTaskByState()
		{
		}
	}

	public class BossQuestTaskDataModel : ILimitTimeActivityTaskDataModel
	{
		public uint DataId { get; private set; }//任务id

		public OpActTaskState State { get; private set; }//任务状态

		public string Desc { get; private set; }//任务描述

		public uint DoneNum { get; private set; }//完成数

		public uint TotalNum { get; private set; }//需完成的总数

		public List<uint> ParamNums { get; private set; } //扩展参数

        public List<uint> ParamNums2 { get; private set; } //拓展参数2

        public List<CounterItem> CountParamNums { get; private set; }//count的扩展参数

        public List<OpTaskReward> AwardDataList { get; private set; }//奖励数组

        public string taskName { get; private set; } //任务名字

        public List<string> ParamProgress { get; private set; } //任务变量进度参数

        public List<OpActTaskParam> ParamProgressList { get; private set; } //任务进度的参数，在这里作为key值来查询

        public UInt16 PlayerLevelLimit { get; private set; } //开启等级限制(玩家等级)
        public int AccountDailySubmitLimit { get; private set; }//账户每日领奖限制次数
        public int AccountTotalSubmitLimit { get; private set; }//账户总领奖限制次数
        public int AccountWeeklySubmitLimit { get; private set; }//账户每周领奖限制次数
        public int CantAccept
        {
            get; private set;
        }

        public int EventType
        {
            get; private set;
        }

        public int SubType
        {
            get; private set;
        }

        public BossQuestTaskDataModel(ActiveTable tableData)
		{
			DataId = (uint)tableData.ID;
			SceneNotifyActiveTaskStatus stateData = ActivityDataManager.GetInstance().GetBossTaskStatusData(DataId);
			if (stateData == null)
			{
				return;
			}

			switch ((TaskStatus)stateData.status)
			{
				case TaskStatus.TASK_INIT:
					State = OpActTaskState.OATS_INIT;
					break;
				case TaskStatus.TASK_UNFINISH:
					State = OpActTaskState.OATS_UNFINISH;
					break;
				case TaskStatus.TASK_FINISHED:
					State = OpActTaskState.OATS_FINISHED;
					break;
				case TaskStatus.TASK_FAILED:
					State = OpActTaskState.OATS_FAILED;
					break;
				case TaskStatus.TASK_SUBMITTING:
					State = OpActTaskState.OATS_SUBMITTING;
					break;
				case TaskStatus.TASK_OVER:
					State = OpActTaskState.OATS_OVER;
					break;
			}

			Desc = string.Format(TR.Value("activity_charge_rebate_task_desc"), tableData.Param1);

			string[] awardItems = tableData.Awards.Split(',');

			AwardDataList = new List<OpTaskReward>();
			for (int i = 0; i < awardItems.Length; ++i)
			{
				string[] id_Count = awardItems[i].Split('_');
				OpTaskReward item = new OpTaskReward
				{
					id = uint.Parse(id_Count[0]),
					num = uint.Parse(id_Count[1])
				};
				AwardDataList.Add(item);
			}
		}
	}
}