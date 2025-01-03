using System;
using System.Collections.Generic;
using ActivityLimitTime;
using GameClient;
using Protocol;
using UnityEngine;

namespace GameClient
{
    public interface ILimitTimeActivityModel: ILimitTimeNote
    {
        uint Id { get;  } //活动id

        string Desc { get;  }//活动描述

        string ItemPath { get; }//item prefab路径

        string ActivityPrefafPath { get; }//活动的路径

        OpActivityState State { get;  }//活动状态

        string Name { get;  }//活动名

        UInt32 Param { get; }//扩展参数

        UInt32[] ParamArray { get; } //扩展参数

        UInt32[] ParamArray2 { get; } //扩展参数2

        string CountParam { get; }

        string[] StrParam { get; }     //扩展参数

        List<ILimitTimeActivityTaskDataModel> TaskDatas { get;}

        void UpdateTask(int taskId);//更新任务

        void SortTaskByState();//任务按状态排序
    }

    public abstract class LimitTimeActivityModelBase
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

        public OpActivityState State { get; set; }//活动状态

        public string Name { get; private set; }//活动名

        public UInt32 Param { get; private set; }//扩展参数

        public UInt32[] ParamArray { get; private set; } //扩展参数数组

        public UInt32[] ParamArray2 { get; private set; } //扩展参数数组2

        public string CountParam { get; set; } // count的拓展参数

        public string[] StrParam { get; set; }    //paramStr的扩展参数

        public List<ILimitTimeActivityTaskDataModel> TaskDatas { get; private set; }

        protected bool mIsSortByState = false;

        public string ActivityPrefafPath { get; private set; } //活动预制体对应的路径 (字符串为空时兼容以前按扩展参数方式处理)

        protected LimitTimeActivityModelBase(OpActivityData msg, string itemPath, string logoPath = null, string noteBgPath = null, string notePrefabPath = null)
        {
            if (msg == null)
            {
                return;
            }
            Id = msg.dataId;
            Name = msg.name;
            RuleDesc = msg.ruleDesc;
            LogoDesc = msg.logoDesc.Replace("|", "\n");
            State = (OpActivityState)msg.state;
            Desc = msg.desc;
            if(!string.IsNullOrEmpty(logoPath))//这里兼容之前的代码，之前有些活动是把bgpath给传给logopath
            {
                LogoPath = logoPath;
            }
            else
            {
                LogoPath = msg.logoPath;
            }

            ItemPath = itemPath;
            StartTime = msg.startTime;
            EndTime = msg.endTime;
            NoteBgPath = noteBgPath;
            NotePrefabPath = notePrefabPath;
            Param = msg.parm;
            ParamArray = msg.parm2;
            ParamArray2 = msg.parm3;
            CountParam = msg.countParam;
            StrParam = msg.strParams;
            TaskDatas = new List<ILimitTimeActivityTaskDataModel>();
            ActivityPrefafPath = msg.prefabPath;
        }

        public void SortTaskByState()
        {
            mIsSortByState = true;
            if (TaskDatas == null)
            {
                return;
            }

            List<ILimitTimeActivityTaskDataModel> overList = new List<ILimitTimeActivityTaskDataModel>();
            List<ILimitTimeActivityTaskDataModel> notOverList = new List<ILimitTimeActivityTaskDataModel>();

            for (int i = 0; i < TaskDatas.Count; ++i)
            {
                if (TaskDatas[i].State == OpActTaskState.OATS_OVER)
                {
                    overList.Add(TaskDatas[i]);
                }
                else
                {
                    notOverList.Add(TaskDatas[i]);
                }
            }

            TaskDatas.Clear();
            TaskDatas.AddRange(notOverList);
            TaskDatas.AddRange(overList);
        }

        protected virtual int _CompareTask(ILimitTimeActivityTaskDataModel task1, ILimitTimeActivityTaskDataModel task2)
        {
            if (task1.State == task2.State)
            {
                return task1.DataId < task2.DataId ? -1 : 1;
            }

            if (task1.State == OpActTaskState.OATS_OVER)
            {
                return 1;
            }
            else if (task1.State == OpActTaskState.OATS_FINISHED)
            {
                return -1;
            }

            return -1;

        }
    }

    public class LimitTimeActivityModel : LimitTimeActivityModelBase, ILimitTimeActivityModel
    {

        private OpActivityData mData;

        public LimitTimeActivityModel(OpActivityData msg, string itemPath, string logoPath = null, string noteBgPath = null, string notePrefabPath = null)
            : base(msg, itemPath, logoPath, noteBgPath, notePrefabPath)
        {
            if (msg == null)
            {
                return;
            }
            string[] SkillDesc = msg.taskDesc.Split('|');
            for (int i = 0; i < msg.tasks.Length; ++i)
            {
                var data = msg.tasks[i];
                if (data == null)
                {
                    continue;
                }
                
                var task = ActivityDataManager.GetInstance().GetLimitTimeTaskData(data.dataid);
                string desc = "";
                if (i < SkillDesc.Length)
                {
                    desc = SkillDesc[i];
                }
                TaskDatas.Add(new LimitTimeActivityTaskDataModel((OpActivityTmpType)msg.tmpType, msg.tasks[i], task, desc));
            }

            mData = msg;
        }

        /// <summary>
        /// 更新任务.
        /// </summary>
        /// <param name="taskId"></param>
        public void UpdateTask(int taskId)
        {
            if (mData == null)
            {
                return;
            }
            bool isContain = false;
            var task = ActivityDataManager.GetInstance().GetLimitTimeTaskData((uint)taskId);
            OpActTaskData taskData = new OpActTaskData();
            for (int i = 0; i < mData.tasks.Length; ++i)
            {
                if (mData.tasks[i].dataid == taskId)
                {
                    taskData = mData.tasks[i];
                }
            }

            for (int i = 0; i < TaskDatas.Count; ++i)
            {
                if (TaskDatas[i].DataId == taskId)
                {
                    //如果数据层没有 说明要任务已经没有了
                    if (task != null)
                    {
                        TaskDatas[i] = new LimitTimeActivityTaskDataModel((OpActivityTmpType)mData.tmpType, taskData, task, TaskDatas[i].Desc);
                    }
                    else
                    {
                        TaskDatas.RemoveAt(i);
                    }
                    isContain = true;
                    break;
                }
            }

            //如果是新数据 则加入
            if (!isContain && mData != null && mData.tasks != null)
            {
                string[] SkillDesc = mData.taskDesc.Split('|');
                for (int i = 0; i < mData.tasks.Length; ++i)
                {
                    if (mData.tasks[i].dataid == taskId)
                    {
                        string desc = "";
                        if (i < SkillDesc.Length)
                        {
                            desc = SkillDesc[i];
                        }
                        TaskDatas.Add(new LimitTimeActivityTaskDataModel((OpActivityTmpType)mData.tmpType, mData.tasks[i], task, desc));
                        break;
                    }
                }
            }

            if (mIsSortByState)
            {
                SortTaskByState();
            }
        }
    }

    public interface ILimitTimeActivityTaskDataModel
    {
        uint DataId { get; }//任务id

        OpActTaskState State { get;  }//任务状态

        string Desc { get; }//任务描述

        uint DoneNum { get;}//完成数

        uint TotalNum { get; }//需完成的总数

        List<uint> ParamNums { get;  } //扩展参数

       List<uint> ParamNums2 { get;  } //拓展参数2

        List<CounterItem> CountParamNums { get;  }//count的扩展参数

        List<OpTaskReward> AwardDataList { get; }//奖励数组
        string taskName { get; }//任务名字

        List<string> ParamProgress { get; }//任务变量进度用的参数

        List<OpActTaskParam> ParamProgressList { get;  } //任务进度的参数，在这里作为key值来查询

         UInt16 PlayerLevelLimit { get; } //开启等级限制(玩家等级)

        int AccountDailySubmitLimit { get; }//账户每日领奖限制次数
        int AccountTotalSubmitLimit { get; }//账户总领奖限制次数
        int AccountWeeklySubmitLimit { get; } //账户每周领奖限制次数

        int CantAccept { get;}//接受类型0：可接1：不可接
        int EventType { get;}//事件类型 11：通关地下城
        int SubType { get;}//事件子类型
    }

    public class LimitTimeActivityTaskDataModel: ILimitTimeActivityTaskDataModel
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

        public List<string> ParamProgress { get; private set; }//任务变量进度用的参数

        public List<OpActTaskParam> ParamProgressList{ get; private set; } //任务进度的参数，在这里作为key值来查询

        public UInt16 PlayerLevelLimit{ get;private set; } //开启等级限制(玩家等级)
        public int AccountDailySubmitLimit { get; private set; }//账户每日领奖限制次数
        public int AccountTotalSubmitLimit { get; private set; }//账户总领奖限制次数
        public int AccountWeeklySubmitLimit { get; private set; }//账户每周领奖限制次数
        public int CantAccept { get; private set; }//接受类型0：可接1：不可接
        public int EventType { get; private set; }//事件类型 11：通关地下城
        public int SubType { get; private set; }//事件子类型
        
        public LimitTimeActivityTaskDataModel(OpActivityTmpType tmpType, OpActTaskData taskData, OpActTask task, string desc) 
        {
            DataId = taskData.dataid;
            if (task != null)
            {
                State = (OpActTaskState)task.state;
                DoneNum = task.curNum;
                ParamProgressList = new List<OpActTaskParam>();
                for(int i = 0;i<task.parms.Length;i++)
                {
                    ParamProgressList.Add(task.parms[i]);
                }
            }
            Desc = desc;
            TotalNum = taskData.completeNum;
            ParamNums = new List<uint>(taskData.variables.Length);
            for (int i = 0; i < taskData.variables.Length; ++i)
            {
                ParamNums.Add(taskData.variables[i]);
            }

            ParamNums2 = new List<uint>(taskData.variables2.Length);
            for(int i = 0;i<taskData.variables2.Length;++i)
            {
                ParamNums2.Add(taskData.variables2[i]);
            }

            CountParamNums = new List<CounterItem>(taskData.counters.Length);
            for(int i = 0;i<taskData.counters.Length;i++)
            {
                CountParamNums.Add(taskData.counters[i]);
            }

            AwardDataList = new List<OpTaskReward>(taskData.rewards.Length);

            for (int i = 0; i < taskData.rewards.Length; ++i)
            {
                AwardDataList.Add(taskData.rewards[i]);
            }

            IActivityDetailDesc actDetailDesc = null;
            switch (tmpType)
            {
                case OpActivityTmpType.OAT_DAY_COST_ITEM:
                case OpActivityTmpType.OAT_COST_ITEM:
                    actDetailDesc = new ActDetailItemDesc();
                    break;
                case OpActivityTmpType.OAT_COMPLETE_DUNG:
                case OpActivityTmpType.OAT_DAY_COMPLETE_DUNG:
                    actDetailDesc = new ActDetailDungeonDesc();
                    break;
                default:
                    actDetailDesc = new ActDetailBaseDesc();
                    break;
            }

            int pNumCount = ParamNums.Count;
            int[] intValues = new int[pNumCount + 1];
            intValues[0] = (int)TotalNum;
            for (int i = 1; i <= pNumCount; i++)
            {
                intValues[i] = (int)ParamNums[i - 1];
            }
            
            if (actDetailDesc != null)
            {
                Desc = actDetailDesc.FormatActivityDesc(Desc, intValues);
            }
            taskName = taskData.taskName;
            ParamProgress = new List<string>();
            for (int i = 0;i<taskData.varProgressName.Length;i++)
            {
                ParamProgress.Add(taskData.varProgressName[i]);
            }
            PlayerLevelLimit = taskData.playerLevelLimit;
            AccountDailySubmitLimit = (int)taskData.accountDailySubmitLimit;
            AccountTotalSubmitLimit = (int)taskData.accountTotalSubmitLimit;
            AccountWeeklySubmitLimit = (int)taskData.accountWeeklySubmitLimit;
            CantAccept = (int)taskData.cantAccept;
            EventType = (int)taskData.eventType;
            SubType = (int)taskData.subType;
        }
    }

	public class ActDetailItemDesc : ActDetailBaseDesc
	{
		public override string FormatActivityDesc(string descFormat, params int[] values)
		{
			object[] opaParams = null;
			if (values != null)
			{
				int pNumCount = values.Length;
				opaParams = new object[pNumCount];
				for (int i = 0; i < pNumCount; i++)
				{
					opaParams[i] = values[i];
				}

				if (pNumCount > 1)
				{
					var itemData = ItemDataManager.CreateItemDataFromTable((int)opaParams[1]);
					string itemDataName = itemData.Name;
					var opaParam = opaParams[0];
					opaParams[0] = itemDataName;
					opaParams[1] = opaParam;
				}
			}
			return string.Format(descFormat, opaParams);
		}
	}
	public class ActDetailBaseDesc : IActivityDetailDesc
	{
		public virtual string FormatActivityDesc(string descFormat, params int[] values)
		{
			object[] opaParams = null;
			if (values != null)
			{
				int pNumCount = values.Length;
				opaParams = new object[pNumCount];
				for (int i = 0; i < pNumCount; i++)
				{
					opaParams[i] = values[i];
				}
			}
			return string.Format(descFormat, opaParams);
		}
	}

	public class ActDetailDungeonDesc : ActDetailBaseDesc
	{
		public override string FormatActivityDesc(string descFormat, params int[] values)
		{
			object[] opaParams = null;
			if (values != null)
			{
				int pNumCount = values.Length;
				opaParams = new object[pNumCount];
				for (int i = 0; i < pNumCount; i++)
				{
					opaParams[i] = values[i];
				}
				if (pNumCount > 1)
				{
					ProtoTable.DungeonTable dta = TableManager.instance.GetTableItem<ProtoTable.DungeonTable>((int)opaParams[1]);
					if (null != dta)
					{
						string dunName = dta.Name;
						var opaParam = opaParams[0];
						opaParams[0] = dunName;
						opaParams[1] = opaParam;
					}
				}
			}
			return string.Format(descFormat, opaParams);
		}
	}


	public interface IActivityDetailDesc
	{
		string FormatActivityDesc(string format, params int[] values);
	}


	public sealed class LevelFightShowActivityDataModel : LimitTimeActivityModelBase, ILimitTimeActivityModel
    {
        public void UpdateTask(int taskId)
        {
            
        }

        public LevelFightShowActivityDataModel(OpActivityData msg, string itemPath, BaseSortList records, string logoPath = null, string noteBgPath = null, string notePrefabPath = null)
            : base(msg, itemPath, logoPath, noteBgPath, notePrefabPath)
        {
            if (msg != null && msg.tasks != null)
            {
                string[] SkillDesc = msg.taskDesc.Split('|');

                for (int i = 0; i < msg.tasks.Length; ++i)
                {
                    string desc = "";
                    if (i < SkillDesc.Length)
                    {
                        desc = SkillDesc[i];
                    }

                    TaskDatas.Add(new LevelFightActivityRankDataModel((OpActivityTmpType)msg.tmpType, msg.tasks[i], null, desc, "", 0));
                }
            }
        }

        public void UpdateRecords(BaseSortList records)
        {
            if (TaskDatas == null)
            {
                return;
            }

            for (int i = 0; i < records.entries.Count; ++i)
            {
                if (i >= TaskDatas.Count)
                {
                    return;
                }
                ((LevelFightActivityRankDataModel) TaskDatas[i]).UpdateData(records.entries[i].name, records.entries[i].ranking);
            }
        }
    }

    public sealed class LevelFightActivityRankDataModel : LimitTimeActivityTaskDataModel
    {
        public string Name { get; private set; }//玩家姓名
        public uint Rank { get; private set; }//排名

        public LevelFightActivityRankDataModel(OpActivityTmpType tmpType, OpActTaskData taskData, OpActTask task, string desc, string name, uint rank)
            : base (tmpType, taskData, task, desc)
        {
            Name = name;
            Rank = rank;
        }

        public void UpdateData(string name, uint rank)
        {
            Name = name;
            Rank = rank;
        }

    }

}