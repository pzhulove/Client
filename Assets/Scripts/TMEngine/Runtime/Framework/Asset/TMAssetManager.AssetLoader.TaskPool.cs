using System.Collections.Generic;

namespace Tenmove.Runtime
{
    internal partial class AssetManager
    {
        private partial class AssetLoader
        {
            private class TaskPool<T> where T : LoadTaskBase
            {
                private readonly Stack<LoadTaskAgent> m_FreeAgents;
                private readonly LinkedList<LoadTaskAgent> m_WorkingAgents;
                private readonly List<LinkedList<T>> m_WaitingTasks;

                public int LoadAgentBaseCount
                {
                    get;
                    set;
                }

                public int LoadAgentExtraCount
                {
                    get;
                    set;
                }

                /// <summary>
                /// 初始化任务池的新实例。
                /// </summary>
                public TaskPool()
                {
                    m_FreeAgents = new Stack<LoadTaskAgent>();
                    m_WorkingAgents = new LinkedList<LoadTaskAgent>();

                    int totalPriorityNum = (int)AssetLoadPriority.Max_Num;
                    m_WaitingTasks = new List<LinkedList<T>>(totalPriorityNum);
                    for(int i = 0; i < totalPriorityNum; ++i)
                    {
                        m_WaitingTasks.Add(new LinkedList<T>());
                    }
                }

                /// <summary>
                /// 获取任务代理总数量。
                /// </summary>
                public int TotalAgentCount
                {
                    get
                    {
                        return FreeAgentCount + WorkingAgentCount;
                    }
                }

                /// <summary>
                /// 获取可用任务代理数量。
                /// </summary>
                public int FreeAgentCount
                {
                    get
                    {
                        return m_FreeAgents.Count;
                    }
                }

                /// <summary>
                /// 获取工作中任务代理数量。
                /// </summary>
                public int WorkingAgentCount
                {
                    get
                    {
                        return m_WorkingAgents.Count;
                    }
                }

                /// <summary>
                /// 获取等待任务数量。
                /// </summary>
                public int WaitingTaskCount
                {
                    get
                    {
                        int iTotal = 0;
                        for (int i = 0,icnt = Utility.Math.Min(m_WaitingTasks.Count, (int)AssetLoadPriority.Max_Num); i < icnt; ++i)
                        {
                            iTotal += m_WaitingTasks[i].Count;
                        }

                        return iTotal;
                    }
                }

                /// <summary>
                /// 任务池轮询。
                /// </summary>
                /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
                /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
                public void Update(float elapseSeconds, float realElapseSeconds)
                {
                    LinkedListNode<LoadTaskAgent> current = m_WorkingAgents.First;
                    while (current != null)
                    {
                        if (current.Value.Task.IsDone)
                        {
                            LinkedListNode<LoadTaskAgent> next = current.Next;
                            current.Value.Reset();
                            m_FreeAgents.Push(current.Value);
                            m_WorkingAgents.Remove(current);
                            current = next;
                            continue;
                        }

                        current.Value.Update(elapseSeconds, realElapseSeconds);
                        current = current.Next;
                    }

                    int maxTaskCountPerFrame = TotalAgentCount * (WaitingTaskCount / 1000  + 1);
                    while (FreeAgentCount > 0 && WaitingTaskCount > 0 && maxTaskCountPerFrame > 0)
                    {
                        // 从高优先级Task开始处理
                        for(int priority = (int)AssetLoadPriority.Max_Num - 1; priority >= 0; --priority)
                        {
                            if(m_WaitingTasks[priority].Count > 0)
                            {
                                int canUseAgentCount = LoadAgentBaseCount + LoadAgentExtraCount * priority;

                                // 如果当前工作Agent数量高于其可用Agent数量，直接退出while循环。
                                if(m_WorkingAgents.Count >= canUseAgentCount)
                                {
                                    maxTaskCountPerFrame = 0;
                                    break;
                                }

                                LoadTaskAgent agent = m_FreeAgents.Pop();
                                LinkedListNode<LoadTaskAgent> agentNode = m_WorkingAgents.AddLast(agent);

                                T task = m_WaitingTasks[priority].First.Value;
                                m_WaitingTasks[priority].RemoveFirst();
                                agent.Start(task);

                                if (task.IsDone)
                                {
                                    --maxTaskCountPerFrame;
                                    agent.Reset();
                                    m_FreeAgents.Push(agent);
                                    m_WorkingAgents.Remove(agentNode);
                                }
                            }
                        }
                    }
                }

                /// <summary>
                /// 关闭并清理任务池。
                /// </summary>
                public void Shutdown()
                {
                    while (FreeAgentCount > 0)
                    {
                        m_FreeAgents.Pop().Shutdown();
                    }

                    for (LinkedListNode<LoadTaskAgent> it = m_WorkingAgents.First; null != it; it = it.Next)
                    {
                        LoadTaskAgent workingAgent = it.Value;
                        workingAgent.Shutdown();
                    }
                    m_WorkingAgents.Clear();

                    for (int i = 0, icnt = m_WaitingTasks.Count; i < icnt; ++i)
                        m_WaitingTasks[i].Clear();
                }

                /// <summary>
                /// 增加任务代理。
                /// </summary>
                /// <param name="agent">要增加的任务代理。</param>
                public void AddAgent(LoadTaskAgent agent)
                {
                    if (agent == null)
                    {
                        throw new EngineException("Task agent is invalid.");
                    }

                    agent.Initialize();
                    m_FreeAgents.Push(agent);
                }

                public void ClearFreeAgent()
                {
                    while (FreeAgentCount > 0)
                    {
                        m_FreeAgents.Pop().Shutdown();
                    }

                    LoadAgentBaseCount = 0;
                    LoadAgentExtraCount = 0;
                }

                /// <summary>
                /// 增加任务。
                /// </summary>
                /// <param name="task">要增加的任务。</param>
                public void AddTask(T task, int priorityLevel)
                {
                    int realPriority = priorityLevel;
                    if (realPriority < 0)
                        realPriority = 0;
                    else if (realPriority >= (int)AssetLoadPriority.Max_Num)
                        realPriority = (int)AssetLoadPriority.Max_Num - 1;

                    // 如果传入的Priority超过了最高Priority，则添加到最高Priority队列头部。否则，添加到尾部。
                    if (priorityLevel > realPriority)
                        m_WaitingTasks[realPriority].AddFirst(task);
                    else
                        m_WaitingTasks[realPriority].AddLast(task);
                }

                /// <summary>
                /// 移除任务。
                /// </summary>
                /// <param name="serialId">要移除任务的序列编号。</param>
                /// <returns>被移除的任务。</returns>
                public LoadTaskBase RemoveTask(int taskID)
                {
                    for (int priority = (int)AssetLoadPriority.Max_Num - 1; priority >= 0; --priority)
                    {
                        for (LinkedListNode<T> it = m_WaitingTasks[priority].First; null != it; it = it.Next)
                        {
                            T waitingTask = it.Value;

                            if (waitingTask.TaskID == taskID)
                            {
                                m_WaitingTasks[priority].Remove(it);
                                return waitingTask;
                            }
                        }
                    }

                    for (LinkedListNode<LoadTaskAgent> it = m_WorkingAgents.First; null != it; it = it.Next)
                    {
                        LoadTaskAgent workingAgent = it.Value;
                        if (workingAgent.Task.TaskID == taskID)
                        {
                            LoadTaskBase task = workingAgent.Task;
                            workingAgent.Reset();
                            m_FreeAgents.Push(workingAgent);
                            m_WorkingAgents.Remove(workingAgent);
                            return task;
                        }
                    }

                    return default(LoadTaskBase);
                }

                public bool RemoveTaskGroup(int groupId)
                {
                    // 主Task是否被成功移除，主Task是依赖DependencyTask的，所以如果Waiting队列中有，肯定有一个是主Task。
                    bool bMainTaskRemoved = false;

                    for (int priority = (int)AssetLoadPriority.Max_Num - 1; priority >= 0; --priority)
                    {
                        LinkedListNode<T> waitingTaskNode = m_WaitingTasks[priority].First;
                        LinkedListNode<T> nextNode;

                        while (waitingTaskNode != null)
                        {
                            nextNode = waitingTaskNode.Next;
                            if (waitingTaskNode.Value.TaskGroupID == groupId)
                            {
                                //waitingTaskNode.Value.Removed = true;
                                bMainTaskRemoved = true;

                                m_WaitingTasks[priority].Remove(waitingTaskNode);
                            }

                            waitingTaskNode = nextNode;
                        }

                    }

                    // 已经触发的Task需要做完，不能删除，但可以标记为RequestAbort。
                    for (LinkedListNode<LoadTaskAgent> it = m_WorkingAgents.First; null != it; it = it.Next)
                    {
                        LoadTaskAgent workingAgent = it.Value;
                        if (workingAgent.Task.TaskGroupID == groupId)
                        {
                            /// Mark abort
                        }
                    }

                    return bMainTaskRemoved;
                }

                /// <summary>
                /// 移除所有任务。
                /// </summary>
                public void RemoveAllTasks()
                {
                    for(int i = 0,icnt = m_WaitingTasks.Count;i<icnt;++i)
                        m_WaitingTasks[i].Clear();

                    for (LinkedListNode<LoadTaskAgent> it = m_WorkingAgents.First; null != it; it = it.Next)
                    {
                        LoadTaskAgent workingAgent = it.Value;

                        workingAgent.Reset();
                        m_FreeAgents.Push(workingAgent);
                    }
                    m_WorkingAgents.Clear();
                }

                public void BeginCleanRuningTask()
                {

                }

                public bool EndCleanRuningTask()
                {
                    return true;
                }
            }
        }
    }
}