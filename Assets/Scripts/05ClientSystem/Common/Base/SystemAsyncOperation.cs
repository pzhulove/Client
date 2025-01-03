using System;
using System.Collections.Generic;
///////É¾³ýlinq
using System.Text;
using UnityEngine;

namespace GameClient
{
    public class SystemAsyncOperation : IASyncOperation
    {

        public float GetProgress()
        {
            return Progress;
        }

        bool   m_isError;
        string m_errorMsg;

        string m_progressInfo;

        public  void   SetError(string ErrorMsg)
        {
            m_errorMsg = ErrorMsg;
            m_isError  = true;
        }

        public void   SetProgressInfo(string info)
        {
            m_progressInfo = info;
        }

        public string GetProgressInfo()
        {
            return m_progressInfo;
        }

        public string  GetErrorMessage()
        {
            return m_errorMsg;
        }
        public bool    IsError()
        {
            return m_isError;
        }

        private int   m_currentTask;

        public  int currentTaskIndex
        {
            get{return m_currentTask;}
        }

        public float  Progress
        {
            get
            {
                float taskProgress = 0.0f;
                if (m_taskInfos.Count <= 0)
                {
                    taskProgress = 1.0f;
                }
                else
                {
                    for (int i = 0; i < m_taskInfos.Count; ++i)
                    {
                        TaskInfo info = m_taskInfos[i];
                        if (info.State == ETaskState.Done)
                        {
                            taskProgress += info.Weight;
                        }
                        else if (info.State == ETaskState.Loading)
                        {
                            taskProgress += (info.Weight * info.Progress);
                        }
                        else
                        {
                            break;
                        }
                    }
                    taskProgress /= m_maxProgress;
                }

                return Mathf.Clamp(taskProgress, 0.0f, 1.0f);
            }
        }
        public string Description
        {
            get
            {
                for (int i = 0; i < m_taskInfos.Count; ++i)
                {
                    TaskInfo info = m_taskInfos[i];
                    if (info.State == ETaskState.Loading)
                    {
                        return info.Name;
                    }
                }
                return "";
            }
        }

        public SystemAsyncOperation()
        {

        }


        enum ETaskState
        {
            NotStart = -1,
            Loading,
            Done,
        }

        class TaskInfo
        {
            public TaskInfo(string name, float weight)
            {
                Name = name;
                Weight = weight;
                Progress = 0.0f;
                State = ETaskState.NotStart;
            }

            public string Name;
            public float Weight;
            public float Progress;
            public ETaskState State;
        }

        List<TaskInfo> m_taskInfos = new List<TaskInfo>();
        float m_maxProgress = 0.0f;
        float m_progressDelta = 0.0f;
        public void ReInit()
        {
            m_taskInfos.Clear();
            m_maxProgress = 0.0f;
            m_progressDelta = 0.0f;
            m_currentTask = 0;
            m_isError = false;
            m_errorMsg = "";
        }

        public void AddTask(string name, float weight = 1.0f)
        {
            m_taskInfos.Add(new TaskInfo(name, weight));
            m_maxProgress += weight;
        }

        public void SetProgress(float progress)
        {
            if(currentTaskIndex < 0 || currentTaskIndex >= m_taskInfos.Count)
            {
                Logger.LogErrorFormat("[SystemAsyncOperation Error!!]_SetTaskProgress CurrentTask out range {0}  0-{1}!!", currentTaskIndex, m_taskInfos.Count-1);
            }

            progress = Mathf.Clamp(progress, 0.0f, 1.0f);

            var current = m_taskInfos[currentTaskIndex];

            if(current.State != ETaskState.Loading)
            {
                Logger.LogErrorFormat("[SystemAsyncOperation Error!!]_SetTaskProgress Error State {0} Index {1} Name{2}!!", current.State,currentTaskIndex,current.Name);
            }

            if (progress < current.Progress)
            {
                Logger.LogErrorFormat("[SystemAsyncOperation Error!!]_SetTaskProgress ==> progress error!! Index:{0} Name:{2} progress:{1}", currentTaskIndex, progress,current.Name);
                return;
            }
            m_progressDelta += (progress - current.Progress) * current.Weight;
            current.Progress = progress;
        }

        public void SetTaskDesc(string description)
        {
            bool success = false;
            for (int i = 0; i < m_taskInfos.Count; ++i)
            {
                TaskInfo info = m_taskInfos[i];
                if (info.State == ETaskState.Loading)
                {
                    info.Name = description;

                    success = true;
                    break;
                }
            }

            if (success == false)
            {
                Logger.LogError("_SetTaskDesc Error!!! No Loading Task");
            }
        }

        public void BeginTask(int iIndex)
        {
            if(iIndex >= 0 && iIndex < m_taskInfos.Count)
            {
                TaskInfo info = m_taskInfos[iIndex];
                m_currentTask = iIndex;
                if(info.State != ETaskState.NotStart)
                {
                    Logger.LogErrorFormat("[SystemAsyncOperation Error!!]try begin task: Error State {0} index:{1} Name:{2}!!", info.State,iIndex,info.Name);
                }
                else
                {
                    info.Progress = 0.0f;
                    info.State = ETaskState.Loading;
                }
                
            }
            else
            {
                 Logger.LogErrorFormat("[SystemAsyncOperation Error!!]try begin task: {0} OutRange 0 - {1}, !!", iIndex, m_taskInfos.Count-1);
            }
        }

        public void FinishTask(int iIndex)
        {
            if(iIndex >= 0 && iIndex < m_taskInfos.Count)
            {
                TaskInfo info = m_taskInfos[iIndex];
                m_currentTask = iIndex;
                if(info.State != ETaskState.Loading)
                {
                    Logger.LogErrorFormat("[SystemAsyncOperation Error!!]try finish task: Error State {0} index:{1} Name:{2}!!", info.State,iIndex,info.Name);
                }
                else
                {
                    info.Progress = 1.0f;
                    info.State = ETaskState.Done;
                }
                
            }
            else
            {
                 Logger.LogErrorFormat("[SystemAsyncOperation Error!!]try finish task: {0} OutRange 0 - {1}, !!", iIndex, m_taskInfos.Count-1);
            }
        }
    }
}