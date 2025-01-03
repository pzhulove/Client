using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;

class InvokeMethod
{
    #region InnerClass
    public class TaskManager : Singleton<TaskManager>
    {
        public interface IInvoke
        {
            bool IsStart();
            void DoInvoke();
        }

        public class Invoke : IInvoke
        {
            public float fStart = 0.0f;
            public float fDelay = 0.0f;
            public UnityAction callback = null;
            public object target = null;
            public bool bNeedRemove = false;

            public Invoke(float fStart, float fDelay, UnityAction callback,object target = null)
            {
                this.fStart = fStart;
                this.fDelay = fDelay;
                this.callback = callback;
                this.target = target;
            }

            public bool IsStart()
            {
                return fStart + fDelay <= Time.time;
            }

            public bool NeedRemove()
            {
                return bNeedRemove;
            }

            public void SetNeedRemove(bool bRemove = true)
            {
                bNeedRemove = bRemove;
            }

            public void DoInvoke()
            {
                if (callback != null)
                {
                    callback.Invoke();
                }
            }
        }

        public class IntervalInvoke
        {
            public static int ms_unique_id = 0;
            public bool bHasBegin = false;
            public int unique_id = 0;
            public float fStart = 0.0f;
            public float fDelay = 0.0f;
            public float fLastTime = 0.0f;
            public float fIntervalTime = 1.0f;
            public int iInvokeTimes = 0;
            public object target = null;
            bool needRemove = false;
            public bool NeedRemove
            {
                get
                {
                    return needRemove;
                }
                set
                {
                    needRemove = value;
                }
            }
            public UnityAction onBegin;
            public UnityAction onInterval;
            public UnityAction onEnd;
            public bool IsStart()
            {
                return fStart + fDelay <= Time.time;
            }

            public bool IsEnd()
            {
                return fStart + fDelay + fLastTime <= Time.time;
            }

            public bool CanInterval()
            {
                return fStart + fDelay + (iInvokeTimes + 1) * fIntervalTime <= Time.time;
            }

            public void DoIntervalInvoke()
            {
                if(onInterval != null)
                {
                    onInterval.Invoke();
                }
            }
            public void DoEndInvoke()
            {
                if(onEnd != null)
                {
                    onEnd.Invoke();
                }
            }
        }

        List<IntervalInvoke> akIntervalInvokeLists = new List<IntervalInvoke>();
        List<Invoke> akInvokeLists = new List<Invoke>();
        List<Invoke> akInvokeNeedDelete = new List<Invoke>();
        bool bUpdate = false;

        List<KeyValuePair<GameClient.IClientFrame, List<UnityAction>>> onGuiPerframeCall = new List<KeyValuePair<GameClient.IClientFrame, List<UnityAction>>>();

        public void Enter()
        {
            if(bUpdate)
            {
                Logger.LogError("Enter when update ?");
            }
            //akInvokeLists.Clear();
            //akInvokeNeedDelete.Clear();
            //onGuiPerframeCall.Clear();
            GameClient.GameFrameWork.instance.onLastUpdate.AddListener(OnLastUpdate);
        }

        public void Exit()
        {
            akInvokeLists.Clear();
            akInvokeNeedDelete.Clear();
            GameClient.GameFrameWork.instance.onLastUpdate.RemoveListener(OnLastUpdate);
            onGuiPerframeCall.Clear();
        }

        public void InvokeCall(float fStart, float fDelyTime, UnityAction callback)
        {
            akInvokeLists.Add(new Invoke(fStart, fDelyTime, callback));
        }

        public int InvokeIntervalCall(object target,float fStart,float fDelayTime,float fInterval,float fLastTime,UnityAction onBegin,UnityAction onInterval,UnityAction onEnd)
        {
            var current = new IntervalInvoke
            {
                unique_id = ++IntervalInvoke.ms_unique_id,
                fStart = fStart,
                fDelay = fDelayTime,
                fIntervalTime = fInterval,
                fLastTime = fLastTime,
                onBegin = onBegin,
                onInterval = onInterval,
                onEnd = onEnd,
                target = target,
            };
            akIntervalInvokeLists.Add(current);
            return current.unique_id;
        }

        public void RmoveInvokeIntervalCall(object target)
        {
            for(int i = 0; i < akIntervalInvokeLists.Count; ++i)
            {
                if(akIntervalInvokeLists[i].target == target)
                {
                    akIntervalInvokeLists[i].NeedRemove = true;
                }
            }
        }

        public void RmoveInvokeIntervalCall(int unique_id)
        {
            for (int i = 0; i < akIntervalInvokeLists.Count; ++i)
            {
                if (akIntervalInvokeLists[i].unique_id == unique_id)
                {
                    akIntervalInvokeLists[i].NeedRemove = true;
                    break;
                }
            }
        }

        public void InvokeCall(object target,float fStart, float fDelyTime, UnityAction callback)
        {
            akInvokeLists.Add(new Invoke(fStart, fDelyTime, callback, target));
        }

        public void RmoveInvokeCall(object target)
        {
            for(int i = 0; i < akInvokeLists.Count; ++i)
            {
                if(akInvokeLists[i].target == target)
                {
                    akInvokeLists[i].SetNeedRemove(true);
                }
            }
        }

        public void RmoveInvokeCall(UnityAction callback)
        {
            for (int i = 0; i < akInvokeLists.Count; ++i)
            {
                if (akInvokeLists[i].callback == callback)
                {
                    akInvokeLists[i].SetNeedRemove(true);
                }
            }
        }

        public void AddUniqueInvoke(ref Invoke invoke)
        {
            bool bFind = false;
            for(int i = 0; i < akInvokeNeedDelete.Count; ++i)
            {
                if(akInvokeNeedDelete[i] == invoke)
                {
                    bFind = true;
                }
            }
            if(!bFind)
            {
                akInvokeNeedDelete.Add(invoke);
            }
        }

        public void RemoveUniqueInvoke(ref Invoke invoke)
        {
            akInvokeNeedDelete.Remove(invoke);
        }

        public void AddPerFrameCall(GameClient.IClientFrame clientFrame,UnityAction callback)
        {
            for(int i = 0; i < onGuiPerframeCall.Count; ++i)
            {
                if(onGuiPerframeCall[i].Key == clientFrame)
                {
                    onGuiPerframeCall[i].Value.Add(callback);
                    return;
                }
            }

            var callbackList = new List<UnityAction>();
            callbackList.Add(callback);
            onGuiPerframeCall.Add(new KeyValuePair<GameClient.IClientFrame, List<UnityAction>>(clientFrame, callbackList));
        }

        public void RemovePerFrameCall(GameClient.IClientFrame clientFrame)
        {
            for (int i = 0; i < onGuiPerframeCall.Count; ++i)
            {
                if (onGuiPerframeCall[i].Key == clientFrame)
                {
                    onGuiPerframeCall.RemoveAt(i);
                    return;
                }
            }
        }

        public void Update()
        {
            bUpdate = true;
            for (int i = 0; i < akInvokeLists.Count; ++i)
            {
                if (akInvokeLists[i] != null)
                {
                    if (akInvokeLists[i].IsStart() && !akInvokeLists[i].NeedRemove())
                    {
                        akInvokeLists[i].DoInvoke();
                        akInvokeLists[i].SetNeedRemove();
                    }
                }
            }

            for (int i = 0; i < akInvokeLists.Count; ++i)
            {
                if(akInvokeLists[i] == null || akInvokeLists[i].NeedRemove())
                {
                    akInvokeLists.RemoveAt(i--);
                }
            }

            for (int i = 0; i < akInvokeNeedDelete.Count; ++i)
            {
                var current = akInvokeNeedDelete[i];
                if (current.IsStart())
                {
                    akInvokeNeedDelete.RemoveAt(i--);
                    Logger.LogProcessFormat("InvokeFading --------->");
                    current.DoInvoke();
                }
            }

            UpdateIntervalInvoke();
            bUpdate = false;
        }

        void UpdateIntervalInvoke()
        {
            for (int i = 0; i < akIntervalInvokeLists.Count; ++i)
            {
                if (akIntervalInvokeLists[i] != null)
                {
                    if(akIntervalInvokeLists[i].NeedRemove)
                    {
                        continue;
                    }

                    if (akIntervalInvokeLists[i].IsStart())
                    {
                        if(!akIntervalInvokeLists[i].bHasBegin)
                        {
                            akIntervalInvokeLists[i].bHasBegin = true;
                            if(akIntervalInvokeLists[i].onBegin != null)
                            {
                                akIntervalInvokeLists[i].onBegin.Invoke();
                            }
                            continue;
                        }

                        if (akIntervalInvokeLists[i].CanInterval())
                        {
                            if (akIntervalInvokeLists[i].onInterval != null)
                            {
                                akIntervalInvokeLists[i].onInterval.Invoke();
                                akIntervalInvokeLists[i].iInvokeTimes += 1;
                            }
                        }

                        if (akIntervalInvokeLists[i].IsEnd())
                        {
                            if(akIntervalInvokeLists[i].onEnd != null)
                            {
                                akIntervalInvokeLists[i].onEnd.Invoke();
                            }
                            akIntervalInvokeLists[i].NeedRemove = true;
                        }
                    }
                }
            }

            for(int i = 0; i < akIntervalInvokeLists.Count; ++i)
            {
                if(akIntervalInvokeLists[i] == null || akIntervalInvokeLists[i].NeedRemove)
                {
                    akIntervalInvokeLists.RemoveAt(i--);
                }
            }
        }

        public void OnLastUpdate()
        {
            while(onGuiPerframeCall.Count > 0)
            {
                var curCallbackList = onGuiPerframeCall[0].Value;
                if(curCallbackList.Count > 0)
                {
                    var callback = curCallbackList[0];
                    curCallbackList.RemoveAt(0);
                    if(callback != null)
                    {
                        callback.Invoke();
                    }
                }

                if(curCallbackList.Count == 0)
                {
                    onGuiPerframeCall.RemoveAt(0);
                }
            }
        }
    }
    #endregion

    public static void Enter()
    {
        Logger.LogProcessFormat("[InvokeMethod] Enter");
        TaskManager.GetInstance().Enter();
    }

    public static void Exit()
    {
        TaskManager.GetInstance().Exit();
    }

    public static void Update()
    {
        TaskManager.GetInstance().Update();
    }

    public static void Invoke(float fDelyTime, UnityAction callback)
    {
        TaskManager.GetInstance().InvokeCall(Time.time, fDelyTime, callback);
    }

    public static void InvokeInterval(object target,float fDelayTime,float fIntervalTime,float fLastTime, UnityAction onBegin,UnityAction onUpdate,UnityAction onEnd)
    {
        TaskManager.GetInstance().InvokeIntervalCall(target, Time.time, fDelayTime, fIntervalTime, fLastTime, onBegin, onUpdate, onEnd);
    }

    public static void RmoveInvokeIntervalCall(object target)
    {
        TaskManager.GetInstance().RmoveInvokeIntervalCall(target);
    }

    public static void RmoveInvokeIntervalCall(int unique_id)
    {
        TaskManager.GetInstance().RmoveInvokeIntervalCall(unique_id);
    }

    public static void Invoke(object target, float fDelyTime, UnityAction callback)
    {
        TaskManager.GetInstance().InvokeCall(target,Time.time, fDelyTime, callback);
    }

    public static void RemoveInvokeCall(object target)
    {
        TaskManager.GetInstance().RmoveInvokeCall(target);
    }

    public static void RemoveInvokeCall(UnityAction callback)
    {
        TaskManager.GetInstance().RmoveInvokeCall(callback);
    }

    public static void AddUniqueInvoke(TaskManager.Invoke invoke)
    {
        TaskManager.GetInstance().AddUniqueInvoke(ref invoke);
    }

    public static void RemoveUniqueInvoke(TaskManager.Invoke invoke)
    {
        TaskManager.GetInstance().RemoveUniqueInvoke(ref invoke);
    }

    public static void AddPerFrameCall(GameClient.IClientFrame clientFrame,UnityAction callback)
    {
        TaskManager.GetInstance().AddPerFrameCall(clientFrame,callback);
    }

    public static void RemovePerFrameCall(GameClient.IClientFrame clientFrame)
    {
        TaskManager.GetInstance().RemovePerFrameCall(clientFrame);
    }
}