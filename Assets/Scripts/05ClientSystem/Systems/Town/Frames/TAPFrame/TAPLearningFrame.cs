using Protocol;
using ProtoTable;
using Scripts.UI;
using System.Collections;
using System.Collections.Generic;
///////删除linq
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    class TaskData
    {
        public int taskId;
        public string taskName;
        public string taskSchedule;
        public string taskReward;
        public bool taskOpen;
    }

    public class TAPLearningFrame : ClientFrame
    {
        RelationData relationData = null; 
        MasterTaskShareData tempData = new MasterTaskShareData();
        List<TaskData> taskDataList = new List<TaskData>();
        //可上报任务
        List<uint> couldReoprtTasks = new List<uint>();
        //已上报任务
        List<uint> reportedTasks = new List<uint>();
        //已确认任务
        List<uint> submitedTasks = new List<uint>();
        //读表任务
        private List<MissionTable> tableTasks = new List<MissionTable>();
        
        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/TAP/TAPLearning";
        }

        protected sealed override void _OnOpenFrame()
        {
            if (userData != null)
            {
                relationData = (RelationData)userData;
            }
            _RegisterUIEvent();
            GetTableShowTasks();
            _InitData();
            RefreshTaskItemListCount();
        }

        void GetTableShowTasks()
        {
            var missionTable = TableManager.GetInstance().GetTable<MissionTable>();
            if (missionTable != null)
            {
                var enumerator = missionTable.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    var missionItem = enumerator.Current.Value as MissionTable;

                    if (missionItem == null)
                    {
                        continue;
                    }

                    if (missionItem.ID > 8000 && missionItem.ID < 8100)
                    {
                        tableTasks.Add(missionItem);    
                    }
                    
                }
            }
            
        }

        protected sealed override void _OnCloseFrame()
        {
            _ClearData();
            _UnRegisterUIEvent();
        }

        void _RegisterUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnTAPLearningUpdate, _OnTAPLearningUpdate);
            _InitComUIList();
        }

        void _UnRegisterUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnTAPLearningUpdate, _OnTAPLearningUpdate);
            if (mTaskList != null)
            {
                mTaskList.onItemVisiable -= BindTaskItem;
            }
        }

        void _InitComUIList()
        {
            if (null == mTaskList)
            {
                return;
            }
            mTaskList.Initialize();

            mTaskList.onItemVisiable += BindTaskItem;
        }

        void BindTaskItem(ComUIListElementScript item)
        {
            ComCommonBind combind = item.GetComponent<ComCommonBind>();
            if (combind == null)
            {
                return;
            }

            if (item.m_index < 0 || item.m_index >= taskDataList.Count)
            {
                return;
            }

            TaskData taskData = taskDataList[item.m_index];
            if (taskDataList == null)
            {
                return;
            }

            var taskName = combind.GetCom<Text>("LearningName");
            taskName.SafeSetText(taskData.taskName);

            var taskSchedule = combind.GetCom<Text>("Schedule");
            taskSchedule.SafeSetText(taskData.taskSchedule);

            var finish = combind.GetGameObject("finish");

            var notOpen = combind.GetGameObject("Open");
            if (submitedTasks.Contains((uint)taskData.taskId))
            {
                finish.CustomActive(true);
                notOpen.CustomActive(false);
                taskSchedule.CustomActive(false);    
            }
            else
            {
                finish.CustomActive(false);
                taskSchedule.CustomActive(taskData.taskOpen);
                notOpen.CustomActive(!taskData.taskOpen);
            }
            

            var bg = combind.GetCom<Image>("bg");
            if (bg != null)
            {
                int num = item.m_index % 2;
                if (num == 0)
                    bg.color = new Color(46 / 255f, 46 / 255f, 44 / 255f, 1.0f);
                else
                    bg.color = new Color(36 / 255f, 35 / 255f, 33 / 255f, 1.0f);
            }

            var reward = combind.GetCom<Text>("Reward");
            reward.SafeSetText(taskData.taskReward); 
        }


        string GetPupilReward(string reward)
        {
            int awardID = -1;
            int awardCount = -1;
            var tempStr = reward.Split('_');
            if (tempStr.Length != 2)
            {
                return "";
            }
            int.TryParse(tempStr[0], out awardID);
            int.TryParse(tempStr[1], out awardCount);
            if (awardID != -1 && awardCount != -1)
            {
                var itemTableData = TableManager.GetInstance().GetTableItem<ItemTable>(awardID);
                if (itemTableData != null)
                {
                    return awardCount.ToString();
                }
            }

            return "";
        }

        void RefreshTaskItemListCount()
        {
            if (null == mTaskList)
            {
                return;
            }
            mTaskList.SetElementAmount(taskDataList.Count);
        }

        void _InitData()
        {
            if (relationData.tapType == TAPType.Teacher)
            {
                //徒弟进
                //是师父的时候tapType是pupil
                //获取徒弟数据
                tempData = TAPNewDataManager.GetInstance().myTAPData;
                _UpdateLearningData(tempData);
                mLevel.text = string.Format("出师等级：{0}/50",PlayerBaseData.GetInstance().Level);

                if (PlayerBaseData.GetInstance().Level >= 50)
                {
                    mGraduation.CustomActive(true);
                    mGraduationGray.CustomActive(false);
                }
                else
                {
                    mGraduation.CustomActive(false);
                    mGraduationGray.CustomActive(true);
                }
                mOneKeyReporting.CustomActive(true);
                mComfirmReporing.CustomActive(false);

                var reportGray = mOneKeyReporting.GetComponent<UIGray>();
                if (reportGray != null)
                {
                    reportGray.SetEnable(couldReoprtTasks.Count <= 0);
                }
                mOneKeyReporting.interactable = (couldReoprtTasks.Count > 0);
                mReportRedPoint.CustomActive(couldReoprtTasks.Count > 0);

            }
            else
            {
                //师父进！
                tempData = TAPNewDataManager.GetInstance().GetMyPupilData(relationData.uid);
                _UpdateLearningData(tempData);
                mLevel.text = string.Format("出师等级：{0}/50", relationData.level);
                //获取本地任务数据(自己是徒弟)
                if (relationData.level >= 50)
                {
                    mGraduation.CustomActive(true);
                    mGraduationGray.CustomActive(false);
                }
                else
                {
                    mGraduation.CustomActive(false);
                    mGraduationGray.CustomActive(true);
                }
                mOneKeyReporting.CustomActive(false);
                mComfirmReporing.CustomActive(true);

                bool couldSubmit = TAPNewDataManager.GetInstance().HaveCouldSubmitTasks(relationData);
                var confirmGray = mComfirmReporing.GetComponent<UIGray>();
                if (confirmGray != null)
                {
                    confirmGray.SetEnable(!couldSubmit);
                }
                mComfirmReporing.interactable = (couldSubmit);
                mComfirmRedPoint.CustomActive(couldSubmit);

            }
            mTask.SafeSetText(string.Format(TR.Value("tap_finished_mission"), submitedTasks.Count, taskDataList.Count));
            
        }
        

        void _UpdateLearningData(MasterTaskShareData pupilData)
        {
            if(pupilData == null)
            {
                return;
            }

            if (pupilData != null)
            {
                taskDataList = new List<TaskData>();
                couldReoprtTasks = new List<uint>();
                reportedTasks = new List<uint>();
                submitedTasks = new List<uint>();

                var tempMissionList = pupilData.dailyTasks.ToList();

                for (int i = 0; i < tableTasks.Count; i++)
                {
                    var mission = tableTasks[i];
                    TaskData data = new TaskData();
                    data.taskId = mission.ID;
                    data.taskName = mission.TaskName;
                    if (relationData != null)
                    {
                        if (relationData.tapType == TAPType.Teacher)
                            data.taskReward = GetPupilReward(mission.Award);
                        else
                            data.taskReward = GetPupilReward(mission.MissionParam);
                    }

                    bool taskOpen = false;
                    var textData = Utility.GetDailyProveTaskConfig(data.taskId);
                    data.taskSchedule = string.Format("0/{0}",textData.iAftValue);
                    foreach (var info in tempMissionList)
                    {
                        if (info != null)
                        {
                            if (info.taskID == mission.ID)
                            {
                                taskOpen = true;
                                data.taskSchedule = Utility.ParseMissionTextForMissionInfo(info, false, false, true);
                                break;
                            }
                        }
                    }

                    data.taskOpen = taskOpen;
                    taskDataList.Add(data);
                }
                
                for (int i = 0; i < tempMissionList.Count; i++)
                {
                    var info = tempMissionList[i];
                    if (info == null)
                    {
                        continue;
                    }

                    //有report_status这个key
                    bool haveTheKey = false;
                    var stateKey = info.akMissionPairs;
                    //可以上报的任务，
                    if (stateKey.Length == 0)
                    {
                        if (info.status == (int) Protocol.TaskStatus.TASK_FINISHED)
                        {
                            couldReoprtTasks.Add(info.taskID);    
                        }
                    }
                    else
                    {
                        for (int j = 0; j < stateKey.Length; j++)
                        {
                            if (stateKey[j].key == "report_status")
                            {
                                haveTheKey = true;
                                if (info.status == (int)Protocol.TaskStatus.TASK_FINISHED)
                                {
                                    //已上报任务
                                    if (stateKey[j].value == "1")
                                    {
                                        reportedTasks.Add(info.taskID);
                                    }
                                }
                                else if (info.status == (int)Protocol.TaskStatus.TASK_OVER)
                                {
                                    //已确认任务
                                    if (stateKey[j].value == "2")
                                    {
                                        submitedTasks.Add(info.taskID);
                                    }
                                }
                                
                            }
                        }

                        if (!haveTheKey)
                        {
                            if (info.status == (int) Protocol.TaskStatus.TASK_FINISHED)
                            {
                                couldReoprtTasks.Add(info.taskID);    
                            }  
                        }
                    }
                    
                }
            }
        }
        void _ClearData()
        {
            relationData = null;
            taskDataList.Clear();
            couldReoprtTasks.Clear();
            reportedTasks.Clear();
            submitedTasks.Clear();
            tableTasks.Clear();
        }
        

        void _OnTAPLearningUpdate(UIEvent uiEvent)
        {
            if((RelationData)uiEvent.Param1 != null)
            {
                relationData = (RelationData)uiEvent.Param1;
            }
            
            _InitData();
            RefreshTaskItemListCount();
        }

        #region ExtraUIBind
        private Text mLevel = null;
        private Text mTask = null;
		private Button mGraduation = null;
        private Button mGraduationGray = null;
        private Button mOneKeyReporting = null;
        private Button mComfirmReporing = null;
        private ComUIListScript mTaskList = null;
        private RectTransform mReportRedPoint = null;
        private RectTransform mComfirmRedPoint = null;

        protected override void _bindExUI()
		{
			mLevel = mBind.GetCom<Text>("Level");
            mTask = mBind.GetCom<Text>("task");

			mGraduation = mBind.GetCom<Button>("graduation");
			if (null != mGraduation)
			{
				mGraduation.onClick.AddListener(_onGraduationButtonClick);
			}
            mGraduationGray = mBind.GetCom<Button>("graduationGray");
            mGraduationGray.onClick.AddListener(_onGraduationGrayButtonClick);
            mOneKeyReporting = mBind.GetCom<Button>("oneKeyReporting");
            mOneKeyReporting.onClick.AddListener(_onOneKeyReportingButtonClick);
            mOneKeyReporting.onClick.AddListener(_onOneKeyReportingButtonClick);
            mComfirmReporing = mBind.GetCom<Button>("comfirmReporing");
            mComfirmReporing.onClick.AddListener(_onComfirmReporingButtonClick);
            mTaskList = mBind.GetCom<ComUIListScript>("TaskList");
            mReportRedPoint = mBind.GetCom<RectTransform>("reportRedPoint");
            mComfirmRedPoint = mBind.GetCom<RectTransform>("comfirmRedPoint");
        }
		
		protected override void _unbindExUI()
		{
			mLevel = null;
            mTask = null;
			if (null != mGraduation)
			{
				mGraduation.onClick.RemoveListener(_onGraduationButtonClick);
			}
			mGraduation = null;
            mGraduationGray.onClick.RemoveListener(_onGraduationGrayButtonClick);
            mGraduationGray = null;
            mOneKeyReporting.onClick.RemoveListener(_onOneKeyReportingButtonClick);
            mOneKeyReporting = null;
            mComfirmReporing.onClick.RemoveListener(_onComfirmReporingButtonClick);
            mComfirmReporing = null;
            mTaskList = null;
            mReportRedPoint = null;
            mComfirmRedPoint = null;
        }
		#endregion
        #region Callback
        private void _onGraduationButtonClick()
        {
            ClientSystemManager.GetInstance().OpenFrame<TapLeaveMasterFrame>(FrameLayer.Middle,relationData);
        }

        private void _onGraduationGrayButtonClick()
        {
            /* put your code in here */
            SystemNotifyManager.SysNotifyTextAnimation(TR.Value("relaton_leaveMaster_Level"));
        }

        private void _onOneKeyReportingButtonClick()
        {
            TAPNewDataManager.GetInstance().SendReportMission(relationData.uid);
        }

        private void _onComfirmReporingButtonClick()
        {
            /* put your code in here */
            //确认汇报领奖，只有师父有这按钮。徒弟跟师父一起领。
            if (relationData.tapType == TAPType.Pupil)
            {
                TAPNewDataManager.GetInstance().SendSubmitMasterTaskReq(relationData.uid);
            }
        }
        #endregion
    }

}
