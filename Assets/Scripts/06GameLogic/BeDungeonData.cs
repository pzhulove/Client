using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using Protocol;
using Network;
using System;
using System.Text;
using System.Diagnostics;

using GameClient;
using ProtoTable;
using System.Runtime.InteropServices;

[LoggerModel("Chapter")]
public class BeStateManager<T> where T : struct
{
    private const int kMaxStackDeep = 10;
    private int mStackDeep = 0;

    #region Property
    private List<KeyValuePair<T, StateNode>> list = new List<KeyValuePair<T, StateNode>>();
    #endregion

    #region Getter & Setter
    private T mLastState = default(T);
    private T mState = default(T);
    private T mItCurState;
    private T mItLastState;

    public T state
    {
        get
        {
            return mState;
        }

        set
        {
            if (!_IsStateEqual(mState, value))
            {
                if (!_IsStateEqual(mLastState, mState))
                {
                    Logger.LogProcessFormat("[StateManager] 从状态 {0} 跳到 {1}", mLastState, mState);
                    //mLastState = mState;
                }

                mState = value;

                Logger.LogProcessFormat("[StateManager] 从状态 {0} 改变到 {1}", mLastState, mState);
            }
        }
    }
    public T lastState
    {
        get
        {
            return mLastState;
        }
    }

    #endregion

    #region State Manager
    protected delegate void Do(T lastState);
    protected delegate void EventDo();

    protected enum eBeStateReentrantType
    {
        /// <summary>
        /// 重入重置
        /// </summary>
        Reset,

        /// <summary>
        /// 重入不重置
        /// </summary>
        Continue,
    }

    protected class BaseEventNode : IEventNode
    {
        public int      time;
        public string   name;
        public EventDo  call;

        private int     alltime = 0;

        public eBeStateReentrantType type = eBeStateReentrantType.Reset;

        public BaseEventNode(string name, int time, EventDo call, eBeStateReentrantType type)
        {
            this.time = time;
            this.name = name;
            this.call = call;
            this.type = type;
        }


        public void Init()
        {
            Logger.LogProcessFormat("[BaseEventNode] 初始化 {0}", name);
            alltime = time;
        }

        public void Reset()
        {
            Logger.LogProcessFormat("[BaseEventNode] 重置 {0}", name);

            time = alltime;
        }

        public void UnInit()
        {
            Logger.LogProcessFormat("[BaseEventNode] 反初始化 {0}", name);

            time = 0;
            call = null;
        }

        public bool Excute()
        {
            try 
            {
                if (null != call)
                {
                    call();

                    Logger.LogProcessFormat("[BaseEventNode] 执行成功 {0}", name);
                }

                time = -1;
                return true;
            }
            catch 
            {
                Logger.LogProcessFormat("[BaseEventNode] 执行失败 {0}", name);
               return false;
            }
        }

        public eBeStateReentrantType  GetReentrantType()
        {
            return type;
        }

        public string GetName()
        {
            return name;
        }

        public bool CanExcute(int delta)
        {
            time -= delta;
            return time < 0;
        }

        public bool CanRemove()
        {
            return time < 0;
        }
    }


    protected interface IEventNode
    {
        eBeStateReentrantType GetReentrantType();

        void Init();

        void UnInit();

        void Reset();

        bool CanExcute(int delta);

        bool CanRemove();

        bool Excute();

        string GetName();
    }
    
    protected interface IStateEventManager
    {
        void Add(IEventNode node);

        void Remove(IEventNode node);

        void Clear();
    }

    protected class StateNode : IStateEventManager
    {
        public Do stateEnter;
        public Do stateExit;

        public List<IEventNode> nodes = new List<IEventNode>();

        public void Add(IEventNode node)
        {
            if (null != node )
            {
                Logger.LogProcessFormat("[BeStateManager] 添加事件节点 {0}", node.GetName());

                node.Init();

                nodes.Add(node);
            }
        }

        public void Remove(IEventNode node)
        {
            if (null != node)
            {
                Logger.LogProcessFormat("[BeStateManager] 删除事件节点 {0}", node.GetName());

                node.UnInit();
                nodes.Remove(node);
            }
        }

        public void Clear()
        {
            if (null != nodes)
            {
                for (int i = 0; i < nodes.Count; ++i)
                {
                    nodes[i].UnInit();
                }

                nodes.Clear();
            }
        }
    }

    protected IStateEventManager _addState(T toState, Do enter, Do exit)
    {
        StateNode node =  new StateNode() { stateEnter = enter, stateExit = exit };
        list.Add(new KeyValuePair<T, StateNode>(toState, node));

        Logger.LogProcessFormat("[BeStateManager] 添加状态 {0}", toState);

        return node;
    }

    protected void _clearAllStates()
    {
        Logger.LogProcessFormat("[BeStateManager] 清除所有状态");

        for (int i = 0; i < list.Count; ++i)
        {
            if (_IsStateEqual(list[i].Key, mItCurState))
            {
                list[i].Value.stateExit(default(T));
            }

            list[i].Value.Clear();
        }

        list.Clear();
    }

    protected void _resetState()
    {
        mLastState = default(T);
        Logger.LogProcessFormat("[BeStateManager] 重置当前状态 {0}, 带有 {1} 个 Event", mLastState, list.Count);

        for (int i = 0; i < list.Count; ++i)
        {
            StateNode node = list[i].Value;
            for (int j = 0; j < node.nodes.Count; ++j)
            {
                node.nodes[j].Reset();
            }
        }
    }

    private bool _IsStateEqual(T fst, T snd)
    {
        return fst.GetHashCode() == snd.GetHashCode();
    }

    protected void _updateState(int delta)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeStateManager._updateState"))
        {
#endif
        if (_IsStateEqual(mLastState, mState))
        {
            for (int i = 0; i < list.Count; ++i)
            {
                if (_IsStateEqual(list[i].Key, mState))
                {
                    List<IEventNode> nodes = list[i].Value.nodes;

                    for (int j = 0; j < nodes.Count; ++j)
                    {
                        if (!nodes[j].CanRemove() && nodes[j].CanExcute(delta))
                        {
                            nodes[j].Excute();

                            Logger.LogProcessFormat("[BeStateManager] 执行 {0} 第 {1} 个 Event, {2}", mState, j, nodes[j].GetName());
                        }
                    }
                    break;
                }
            }
            return;
        }

        mItCurState = default(T);
        mItLastState = default(T);
        mStackDeep = 0;

        while (!_IsStateEqual(mItCurState, mState) && ++mStackDeep < kMaxStackDeep)
        {
            mItCurState = mState;
            mItLastState = mLastState;

            for (int i = 0; i < list.Count; i++)
            {
                if (_IsStateEqual(list[i].Key, mItLastState))
                {
                    if (list[i].Value != null && list[i].Value.stateExit != null)
                    {
                        Logger.LogProcessFormat("[BeStateManager] 从 {0} 退出 到 {1}", mItLastState, mItCurState);
                        list[i].Value.stateExit(mItCurState);
                        break;
                    }
                }
            }

            if (!_IsStateEqual(mItLastState, mLastState))
            {
                Logger.LogErrorFormat("[BeStateManager] 无法再退出函数中修改状态，回退上一个状态至 mLastState {0}", mItLastState);
                mLastState = mItLastState;
            }

            if (!_IsStateEqual(mItCurState, mState))
            {
                Logger.LogErrorFormat("[BeStateManager] 无法再退出函数中修改状态, 回退当前状态 mState  {0}", mItCurState);
                mState = mItCurState;
            }

            mLastState = mItCurState;

            for (int i = 0; i < list.Count; i++)
            {
                if (_IsStateEqual(list[i].Key, mItCurState))
                {
                    if (list[i].Value != null && list[i].Value.stateEnter != null)
                    {
                        List<IEventNode> nodes = list[i].Value.nodes;
                        for (int j = 0; j < nodes.Count; ++j)
                        {
                            if (!nodes[j].CanRemove() && nodes[j].GetReentrantType() == eBeStateReentrantType.Reset)
                            {
                                Logger.LogProcessFormat("[BeStateManager] 重置 {0}", nodes[j].GetName());

                                nodes[j].Reset();
                            }
                        }

                        Logger.LogProcessFormat("[BeStateManager] 从 {0} 进入状态 {1}", mItLastState, mItCurState);
                        list[i].Value.stateEnter(mItLastState);
                        break;
                    }
                }
            }
        }

        Logger.LogProcessFormat("[BeStateManager] 当前栈深度 {0}", mStackDeep);

        if (mStackDeep > kMaxStackDeep)
        {
            Logger.LogError("[BeStateManager] 超过最大栈深度");
        }
#if ENABLE_PROFILER
        }
#endif
    }
    #endregion
}
