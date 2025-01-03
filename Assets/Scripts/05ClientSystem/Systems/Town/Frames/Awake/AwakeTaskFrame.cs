using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;
using Protocol;
using ProtoTable;
using System;

namespace GameClient
{
    class AwakeTaskFrame : ClientFrame
    {
        const int MaxTaskNum = 4;

        List<int> TaskList = new List<int>();
       
        int DoingTaskIndex = -1;
        int CurSelTaskIndex = -1;

        ChangeJobState eState = ChangeJobState.DoAwakeJobTask;

        protected override void _OnOpenFrame()
        {
            PlayerBaseData.GetInstance().eChangeJobState = Utility.GetChangeJobState();
            eState = PlayerBaseData.GetInstance().eChangeJobState;

            MissionManager.GetInstance().onDeleteMission += OnFinishTask;

            InitInterface();
        }

        protected override void _OnCloseFrame()
        {
            ClearData();   
        }

        public override string GetPrefabPath()
        {
            return "UI/Prefabs/AwakeTaskFrame";
        }

        void ClearData()
        {
            TaskList.Clear();

            DoingTaskIndex = -1;
            CurSelTaskIndex = -1;

            eState = ChangeJobState.DoAwakeJobTask;

            MissionManager.GetInstance().onDeleteMission -= OnFinishTask;
        }

        [UIEventHandle("btClose")]
        void OnClose()
        {
            frameMgr.CloseFrame(this);
        }

        [UIEventHandle("return_back/btReturn")]
        void OnReturn()
        {
            OnClose();
        }

        [UIEventHandle("TaskList/task{0}", typeof(Toggle), typeof(UnityEngine.Events.UnityAction<int, bool>), 1, MaxTaskNum)]
        void OnSwitchTask(int iIndex, bool value)
        {
            if (iIndex < 0 || !value)
            {
                return;
            }

            CurSelTaskIndex = iIndex;

            UpdateCurTaskInfo();
        }

        [UIEventHandle("btReceiveAward")]
        void OnReceiveAward()
        {
            if (CurSelTaskIndex < 0 || CurSelTaskIndex >= TaskList.Count)
            {
                return;
            }

            if (ClientSystemManager.GetInstance().IsFrameOpen<AwakeAwardFrame>())
            {
                ClientSystemManager.GetInstance().CloseFrame<AwakeAwardFrame>();
            }

            ClientSystemManager.GetInstance().OpenFrame<AwakeAwardFrame>(FrameLayer.Middle, TaskList[CurSelTaskIndex]);

            MissionManager.GetInstance().sendCmdSubmitTask((uint)TaskList[CurSelTaskIndex], TaskSubmitType.TASK_SUBMIT_UI, 0);
        }

        void InitInterface()
        {
            var JobData = TableManager.GetInstance().GetTableItem<JobTable>(PlayerBaseData.GetInstance().JobTableID);
            if (JobData == null)
            {
                return;
            }

            if(eState == ChangeJobState.DoChangeJobTask)
            {
//                 Sprite Icon = AssetLoader.instance.LoadRes("", typeof(Sprite)).obj as Sprite;
//                 if (Icon != null)
//                 {
//                     title.sprite = Icon;
//                 }

                TaskList = Utility.GetChangeJobTaskList();
            }
            else
            {
                TaskList = Utility.GetAwakeTaskList();
            }

            taskIcon.gameObject.AddComponent<UIGray>();
            taskIcon.gameObject.GetComponent<UIGray>().enabled = false;

            for (int i = 0; i < MaxTaskNum; i++)
            {
                if(i < TaskList.Count)
                {
                    MissionManager.SingleMissionInfo kMissionInfo = null;

                    if (MissionManager.GetInstance().taskGroup.TryGetValue((uint)TaskList[i], out kMissionInfo))
                    {
                        task[i].isOn = true;

                        DoingTaskIndex = i;
                        CurSelTaskIndex = i;
                    }
                    else
                    {
                        task[i].isOn = false;
                    }

                    task[i].gameObject.SetActive(true);
                }
                else
                {
                    task[i].isOn = false;
                    task[i].gameObject.SetActive(false);
                }
            }

            for (int i = 0; i < MaxTaskNum; i++)
            {
                if (i < TaskList.Count)
                {
                    if (i < DoingTaskIndex)
                    {
                        finish[i].gameObject.SetActive(true);
                        Tasklock[i].gameObject.SetActive(false);
                        cover[i].gameObject.SetActive(false);
                    }
                    else if (i == DoingTaskIndex)
                    {
                        finish[i].gameObject.SetActive(false);
                        Tasklock[i].gameObject.SetActive(false);
                        cover[i].gameObject.SetActive(false);
                    }
                    else
                    {
                        finish[i].gameObject.SetActive(false);
                        Tasklock[i].gameObject.SetActive(true);
                        cover[i].gameObject.SetActive(true);
                    }
                }
                else
                {
                    finish[i].gameObject.SetActive(false);
                    Tasklock[i].gameObject.SetActive(false);
                    cover[i].gameObject.SetActive(false);
                }
            }

            for (int i = 0; i < MaxTaskNum - 1; i++)
            {
                if(i < DoingTaskIndex)
                {
                    Lines[i].gameObject.SetActive(true);
                }
                else
                {
                    Lines[i].gameObject.SetActive(false);
                }
            }      

            if (JobData.JobPortrayal != "" && JobData.JobPortrayal != "-")
            {
                //Sprite Icon = AssetLoader.instance.LoadRes(JobData.JobPortrayal, typeof(Sprite)).obj as Sprite;
                //if (Icon != null)
                //{
                //    role.sprite = Icon;
                //}
                ETCImageLoader.LoadSprite(ref role, JobData.JobPortrayal);
            }

            if(eState == ChangeJobState.DoChangeJobTask)
            {
                jobName.gameObject.SetActive(false);
            }
            else
            {
                if (JobData.AwakeJobName != "" && JobData.AwakeJobName != "-")
                {
                    //Sprite Icon = AssetLoader.instance.LoadRes(JobData.AwakeJobName, typeof(Sprite)).obj as Sprite;
                    //if (Icon != null)
                    //{
                    //    jobName.sprite = Icon;
                    //}
                    ETCImageLoader.LoadSprite(ref jobName, JobData.AwakeJobName);
                }
            }

            UpdateCurTaskInfo();
        }

        void UpdateCurTaskInfo()
        {
            if(CurSelTaskIndex < 0 || CurSelTaskIndex >= TaskList.Count)
            {
                return;
            }

            MissionTable MissionItemData = TableManager.GetInstance().GetTableItem<MissionTable>(TaskList[CurSelTaskIndex]);
            if(MissionItemData == null)
            {
                return;
            }

            taskname.text = MissionItemData.TaskName;

            if(CurSelTaskIndex < DoingTaskIndex)
            {
                taskstate.text = "完成";
                taskIcon.gameObject.GetComponent<UIGray>().enabled = false;
                btReceiveAward.gameObject.SetActive(false);
            }
            else if(CurSelTaskIndex == DoingTaskIndex)
            {
                MissionManager.SingleMissionInfo kMissionInfo = null;
                MissionManager.GetInstance().taskGroup.TryGetValue((uint)TaskList[CurSelTaskIndex], out kMissionInfo);
          
                if (kMissionInfo != null && kMissionInfo.status == (int)TaskStatus.TASK_FINISHED)
                {
                    taskIcon.gameObject.GetComponent<UIGray>().enabled = false;
                    btReceiveAward.gameObject.SetActive(true);

                    taskstate.text = "可提交";
                }
                else
                {
                    taskIcon.gameObject.GetComponent<UIGray>().enabled = true;
                    btReceiveAward.gameObject.SetActive(false);

                    taskstate.text = "进行中";
                }
            }
            else
            {
                taskstate.text = "未解锁";
                taskIcon.gameObject.GetComponent<UIGray>().enabled = true;
                btReceiveAward.gameObject.SetActive(false);
            }

            taskDes.text = Utility.ParseMissionText(MissionItemData.ID, true);
        }

        public void OnFinishTask(UInt32 iMissionID)
        {
            if(eState == ChangeJobState.DoChangeJobTask)
            {
                if (!Utility.IsChangeJobTask(iMissionID))
                {
                    return;
                }
            }
            else
            {
                if (!Utility.IsAwakeTask(iMissionID))
                {
                    return;
                }
            }

            if(iMissionID != TaskList[CurSelTaskIndex])
            {
                return;
            }

            OnClose();
        }

        [UIControl("titleback/Title")]
        protected Image title;

        [UIControl("role")]
        protected Image role;

        [UIControl("jobback/jobName")]
        protected Image jobName;

        [UIControl("TaskDes/taskname")]
        protected Text taskname;

        [UIControl("TaskDes/taskstate")]
        protected Text taskstate;

        [UIControl("TaskDes/taskIcon")]
        protected Image taskIcon;

        [UIControl("TaskDes/taskDes")]
        protected Text taskDes;

        [UIControl("TaskList/line{0}", typeof(Image), 1)]
        protected Image[] Lines = new Image[MaxTaskNum - 1];

        [UIControl("TaskList/task{0}", typeof(Toggle), 1)]
        protected Toggle[] task = new Toggle[MaxTaskNum];

        [UIControl("TaskList/task{0}/finish", typeof(Image), 1)]
        protected Image[] finish = new Image[MaxTaskNum];

        [UIControl("TaskList/task{0}/cover", typeof(Image), 1)]
        protected Image[] cover = new Image[MaxTaskNum];

        [UIControl("TaskList/task{0}/lock", typeof(Image), 1)]
        protected Image[] Tasklock = new Image[MaxTaskNum];

        [UIControl("btReceiveAward")]
        protected Button btReceiveAward;
    }
}