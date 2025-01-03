using System.Collections.Generic;
using UnityEngine;
using ProtoTable;
using System;

namespace GameClient {
    public enum NewBieModifyDataType
    {
        JobID = 0,
        EquipInPackagePos,
        PackageEquipTipsGuidePos,
        ChangedEquipInPackagePos,
        ActorShowEquipPos,
        WelfareID,
        SignInID,
        EntourageID,
        EnchantID,
        EnchantMagicCardID,
        StartPause,
        EndPause,
        AchievementPos,
        BranchMissionPos,
        DailyMissionPos,
        IconPath,
        IconName,
        PreJobSkill,
        PreJobName,
        MagicBoxPos,
        FashionInPackagePos,
    }

    public struct NewbieModifyData
    {
        public int iIndex; // 参数在ComNewbieData()的参数列表args里的索引
        public NewBieModifyDataType ModifyDataType;
    }

	public class ComNewbieData
	{
        private NewbieGuideComType comType;
        public NewbieGuideComType ComType { get {return comType;} }

        private System.Type       comNewbieGuideType;
        public Type ComNewbieGuideType { get {return comNewbieGuideType;} }
       

        public object[] args;
        public List<NewbieModifyData> ModifyDataTypeList = new List<NewbieModifyData>();



        public ComNewbieData(NewbieGuideComType ct, List<NewbieModifyData> ModifyTypeList, params object[] args)
		{
			comType = ct;
            ModifyDataTypeList = ModifyTypeList;

            this.args = args;
		}

        public static ComNewbieData New<T>(List<NewbieModifyData> ModifyTypeList, params object[] args) where T : ComNewbieGuideBase
        {
            var data = new ComNewbieData(NewbieGuideComType.USER_DEFINE,ModifyTypeList,args);
            data.comNewbieGuideType = typeof(T);
            return data;
        }
        public static ComNewbieData New(NewbieGuideComType ct, List<NewbieModifyData> ModifyTypeList, params object[] args)
        {
            return new ComNewbieData(ct,ModifyTypeList,args);
        }
	}

	public class NewbieConditionData
	{
		public eNewbieGuideCondition condition;
		public int[] LimitArgsList;
        public string[] LimitFramesList;

        public delegate bool userConditionFunc();
        public userConditionFunc mComditionFunc;

		public NewbieConditionData(eNewbieGuideCondition c, int[] args = null, string[] LimitFrameArgs = null)
		{
			condition = c;

			if (args == null)
            {
                LimitArgsList = new int[] { };
            }       
			else
            {
                LimitArgsList = args;
            }

            if (LimitFrameArgs == null)
            {
                LimitFramesList = new string[] { };
            }
            else
            {
                LimitFramesList = LimitFrameArgs;
            }
        }

        static public NewbieConditionData NewUserCondition(userConditionFunc func)
        {
            NewbieConditionData data = new NewbieConditionData(
                eNewbieGuideCondition.UserDefine
            );
            data.mComditionFunc = func;
            return data;
        }
	}

	[LoggerModel("NewbieGuide")]
	public class NewbieGuideDataUnit
	{
        public NewbieGuideManager manager = null;

        public NewbieGuideTable.eNewbieGuideType guideType = NewbieGuideTable.eNewbieGuideType.NGT_None;
		public NewbieGuideTable.eNewbieGuideTask taskId;
        public int savePoint;
		public bool finished = false;
        public bool AlreadySend = false;

        public List<ComNewbieData> newbieComList = new List<ComNewbieData>();
        public List<NewbieConditionData> newbieConditionList = new List<NewbieConditionData>();
        public List<object> NeedSaveParamsList = new List<object>();

        public NewbieGuideDataUnit(int tid)
		{
			taskId = (NewbieGuideTable.eNewbieGuideTask)tid;
			savePoint = 0;
			finished = false;
		}

        public void ClearData()
        {
            guideType = NewbieGuideTable.eNewbieGuideType.NGT_None;
            taskId = NewbieGuideTable.eNewbieGuideTask.None;
            savePoint = 0;
            finished = false;
            AlreadySend = false;

            if(newbieComList != null)
            {
                newbieComList.Clear();
            }

            if(newbieConditionList != null)
            {
                newbieConditionList.Clear();
            }

            if(NeedSaveParamsList != null)
            {
                NeedSaveParamsList.Clear();
            }
        } 
			
		public virtual void LoadEvent(){}
		public virtual void Unloadevent(){}

		public void Init()
		{
			LoadEvent();

			InitContent();
			InitCondition();
		}

		public virtual void InitContent(){}
		public virtual void InitCondition(){}

		protected void AddContent(ComNewbieData data)
		{
			if (data != null)
            {
                newbieComList.Add(data);
            }			
		}

		protected void AddCondition(NewbieConditionData condition)
		{
			if (condition != null)
				newbieConditionList.Add(condition);
		}

        public bool CheckAllCondition(UIEvent uiEvent)
		{
			if (finished)
            {
                return false;
            }

            NeedSaveParamsList.Clear();

            if (taskId == NewbieGuideTable.eNewbieGuideTask.AutoFightGuide)
            {
                int llll = 0;
            }

            for (int i = 0; i < newbieConditionList.Count; ++i)
			{
				var con = newbieConditionList[i];

				if (!NewbieGuideConditionUtil.CheckCondition(taskId, uiEvent, con, con.condition, ref NeedSaveParamsList, con.LimitArgsList))
                {
                    //UnityEngine.Debug.LogWarningFormat("Check Condition Failed {0}",con.condition.ToString());
                    return false;
                }
			}

            Logger.LogWarningFormat("新手引导: {0} 条件通过", taskId);

            return true;
		}

        public void Start()
		{
			//Logger.LogErrorFormat("{0} Start", taskId);
			OnStart();
		}

		public void UnitFinish()
		{
			//Logger.LogErrorFormat("{0} Finish", taskId);

			if (!finished)
			{
				finished = true;
				Unloadevent();

				OnFinish();
			}
		}

		protected virtual void OnStart(){}
		protected virtual void OnFinish(){}
	}

}
