using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace GameClient
{
    public interface IEnumeratorManager
    {
        /// <summary>
        /// 添加一个csharp原声迭代器iter
        ///
        /// 在在战斗帧同步模式中，
        /// 无法使用WaitForSeconed, WaitForEndOfFrame等继承自YieldInstruction的类
        /// </summary>
        IEnumerator AddEnumerator(IEnumerator iter, int priority = int.MaxValue);

        /// <summary>
        /// 若需要在迭代器中管理一些迭代
        /// 则需要使用这个接口来添加流程
        /// </summary>
        IEnumerator AddEnumerator(IEnumerator iter, IEnumerator root = null);

        void RemoveEnumerator(IEnumerator iter);

        void ClearAllEnumerators();

        bool IsEnumeratorError(IEnumerator iter);

        bool IsEnumeratorRunning(IEnumerator iter);

        object GetEnumeratorCurrent(IEnumerator iter);

        string GetEnumeratorError(IEnumerator iter, bool isPopError = true);

        eEnumError GetEnumeratorErrorType(IEnumerator iter, bool isPopError = false);

        void DumpAllEnumeratorError();

        void UpdateEnumerators();
    }

    public interface IEnumeratorLifeCycle
    {
        void OnAdd();

        void OnRemove();
    }

    public class EnumeratorProcessManager : IEnumeratorManager
    {
        private List<List<Node>> mStackEnumerators = new List<List<Node>>();

        private List<ErrorUnit> mAllErrors = new List<ErrorUnit>();

        private class Node
        {
            public IEnumerator process;
            public int         rootindex;
        }

        private class ErrorUnit : IComparable<ErrorUnit> 
        {
            public IEnumerator rootProcess;
            public string      errorMsg;
            public eEnumError  type     = eEnumError.UnkownError;

            public int         priority = -1;

#region IComparable implementation
            public int CompareTo(ErrorUnit other)
            {
                return other.priority - this.priority;
            }
#endregion
        }

#region IEnumeratorManager implementation
        private void _addErrorMsg(IEnumerator iter, string msg, eEnumError type)
        {
            ErrorUnit unit = new ErrorUnit();

            unit.priority = 0;
            unit.errorMsg = msg;
            unit.type     = type;

            unit.rootProcess = iter;

            mAllErrors.Add(unit);
        }

        public void DumpAllEnumeratorError()
        {
            for (int i = 0; i < mAllErrors.Count; ++i)
            {
                ErrorUnit unit = mAllErrors[i];

                if (null != unit && unit.priority >= 0)
                {
                    Logger.LogError(unit.errorMsg);
                }
            }

            mAllErrors.Clear();
        }

        public eEnumError GetEnumeratorErrorType(IEnumerator iter, bool isPopError = false)
        {
            ErrorUnit unit = mAllErrors.Find(x => { return x.rootProcess == iter; });

            if (null != unit)
            {
                return unit.type;

                if (isPopError)
                {
                    mAllErrors.Remove(unit);
                }
            }

            return eEnumError.UnkownError;
        }

        public string GetEnumeratorError(IEnumerator iter, bool isPopError = true)
        {
            string res = "";

            ErrorUnit unit = mAllErrors.Find(x => { return x.rootProcess == iter; });

            if (null != unit)
            {
                res = unit.errorMsg;

                if (isPopError)
                {
                    mAllErrors.Remove(unit);
                }
            }

            return res;
        }

        public bool IsEnumeratorError(IEnumerator iter)
        {
            for (int i = 0; i < mAllErrors.Count; ++i)
            {
                if (mAllErrors[i].rootProcess == iter)
                {
                    return true;
                }
            }

            return false;
        }

        private List<Node> _findIter(IEnumerator iter)
        {
            if (null != iter)
            {
                for (int i = 0; i < mStackEnumerators.Count; ++i)
                {
                    for (int j = 0; j < mStackEnumerators[i].Count; ++j)
                    {
                        if (iter == mStackEnumerators[i][j].process)
                        {
                            return mStackEnumerators[i];
                        }
                    }
                }
            }

            return null;
        }

        public bool IsEnumeratorRunning(IEnumerator iter)
        {
            List<Node> list = _findIter(iter);
            return null != list;
        }

        public object GetEnumeratorCurrent(IEnumerator iter)
        {
            List<Node> list = _findIter(iter);

            if (null != list && list.Count > 0)
            {
                return list[list.Count - 1].process.Current;
            }

            return null;
        }
            
        public void UpdateEnumerators()
        {
            // TODO 改成达意的变量名
            //bool flag = false;
            List<Node> iters = null;

            for (int i = 0; i < mStackEnumerators.Count; ++i)
            {
                iters = mStackEnumerators[i];
                if (null != iters)
                {
                    int cnt = iters.Count;
                    if (cnt > 0)
                    {
                        int curIdx = cnt - 1;
                        IEnumerator curIter = iters[curIdx].process;
                        int curIterTopIdx = iters[curIdx].rootindex;

                        bool moveNextSuccess = false;

                        if (null != curIter)
                        {
                            try
                            {
                                moveNextSuccess = curIter.MoveNext();
                            }
                            catch(Exception e)
                            {
                                Logger.LogErrorFormat("[Enumerator] 异常捕获 {0}", e.ToString());
                                moveNextSuccess = false;
                            }
                        }

                        if (moveNextSuccess)
                        {
                            //flag = true;
                            if (curIter.Current is IEnumerator)
                            {
                                IEnumerator nextDeepIter = curIter.Current as IEnumerator;

                                Node node = new Node();

                                node.process = nextDeepIter;
                                node.rootindex = curIterTopIdx;

                                iters.Add(node);

                                // push
                                Logger.LogProcessFormat("[Enumerator] {0} 执行中入栈 顶部索引{1} 栈深度{2}", i, curIterTopIdx, iters.Count);
                                //break;
                            }
                            else if(curIter.Current is ICustomEnumError)
                            {
                                ICustomEnumError error = curIter.Current as ICustomEnumError;

                                Logger.LogErrorFormat("[Enumerator] {0} 异常流程 {1}({2}) 顶部索引{3} 栈深度{4}", i, error.GetErrorMsg(), error.GetErrorType(), curIterTopIdx, iters.Count);

                                _addErrorMsg(iters[curIterTopIdx].process, error.GetErrorMsg(), error.GetErrorType());

                                // clear to root
                                iters.RemoveAll(x=>{return x.rootindex >= curIterTopIdx; });
                            }
                        }
                        else 
                        {
                            iters.RemoveAt(curIdx);
                            //cnt--;
                            // pop
                            Logger.LogProcessFormat("[Enumerator] {0} 执行中出栈 顶部索引{1} 栈深度{2}", i, curIterTopIdx, iters.Count);
                        }
                    }
                }
            }

            //if (!flag)
            {
                mStackEnumerators.RemoveAll(x=>
                {
                    return x == null || x.Count <= 0;
                });
            }
        }

        private bool _isContainEnumerator(IEnumerator iter)
        {
            List<Node> list = _findIter(iter);
            return null != list;
        }

        private void _dumpAllEnumerator()
        {
            for (int i = 0; i < mStackEnumerators.Count; ++i)
            {
                Logger.LogProcessFormat("[Enumerator] 第 {0}/{1} 个任务 栈深度{1}", i, mStackEnumerators.Count, mStackEnumerators[i].Count);
            }
        }

        public IEnumerator AddEnumerator(IEnumerator iter, IEnumerator root = null)
        {
            if (null != iter)
            {
                List<Node> list = _findIter(root);

                if (null == list)
                {
                    AddEnumerator(iter, int.MaxValue);
                }
                else
                {
                    int rootidx = 0;

                    for (int i = 0; i < list.Count; ++i)
                    {
                        if (root == list[i].process)
                        {
                            rootidx = i;
                            break;
                        }
                    }

                    list[rootidx].rootindex = rootidx;

                    Node node = new Node();
                    node.process = iter;
                    node.rootindex = list[list.Count - 1].rootindex;
                    list.Add(node);

                    Logger.LogProcessFormat("[Enumerator] 添加 数目{0}, 栈顶索引{1}", list.Count, node.rootindex);
#if UNITY_EDITOR
                    Logger.LogProcessFormat("[Enumerator] {0}, rootindex {1}", node.process.GetType().FullName, node.rootindex);
#endif
                }
            }

            return iter;
        }


        public IEnumerator AddEnumerator(IEnumerator iter, int priority = int.MaxValue)
        {
            if (null != mStackEnumerators && null != iter)
            {
                if (!_isContainEnumerator(iter))
                {
                    List<Node> list = new List<Node>();
                    mStackEnumerators.Add(list);
                    Node node = new Node();
                    node.process = iter;
                    node.rootindex = 0;
                    list.Add(node);

                    Logger.LogProcessFormat("[Enumerator] 添加 数目{0}, 栈顶索引{1}", list.Count, node.rootindex);

#if UNITY_EDITOR
                    Logger.LogProcessFormat("[Enumerator] {0}, rootindex {1}", node.process.GetType().FullName, node.rootindex);
#endif

                    _dumpAllEnumerator();
                }
                else 
                {
                    Logger.LogProcessFormat("[Enumerator] 该迭代流程，已经在其他的子流程中");
                }
            }

            return iter;
        }

        public void RemoveEnumerator(IEnumerator iter)
        {
            List<Node> list = _findIter(iter);
            if (null != list)
            {
                int top = 0;
                for (int i = list.Count - 1; i >= 0; --i)
                {
                    if (list[i].process == iter)
                    {
                        top = list[i].rootindex;
                        break;
                    }
                }

                list.RemoveAll(x=>{ return x.rootindex >= top; });

                IEnumeratorLifeCycle life = iter as IEnumeratorLifeCycle;

                if (null != life)
                {
                    Logger.LogProcessFormat("[Enumerator] 调用到生命周期onRemove");

                    life.OnRemove();
                }

                Logger.LogProcessFormat("[Enumerator] 删除到 {0}", top);

                if (list.Count <= 0)
                {
                    mStackEnumerators.Remove(list);
                }
            }
        }

        public void ClearAllEnumerators()
        {
            for (int i = 0; i < mStackEnumerators.Count; ++i)
            {
                if (null != mStackEnumerators[i])
                {
                    mStackEnumerators[i].Clear();
                    mStackEnumerators[i] = null;
                }
            }
            // TODO invoke the onRemove

            mStackEnumerators.Clear();

            Logger.LogProcessFormat("[Enumerator] 清除所有迭代器");
        }
#endregion

    }
}
