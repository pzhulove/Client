

using System;
using System.Collections.Generic;
using Tenmove.Runtime;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Tenmove.Editor.Unity
{
    public partial class UnityEditorBase
    {
        private class FrameTrigger
        {

            private static readonly uint InvalidTriggerHandle = ~0u;

            private interface ITMFrameTriggerTask
            {
                uint ID { get; }
                bool CheckTrigger(uint frameNow);
            }

            private struct FrameTriggerTask<T, U> : ITMFrameTriggerTask
            {
                private readonly uint m_TriggerID;
                private readonly uint m_TriggerAfter;
                private readonly T m_Initiator;
                private readonly U m_UserData;
                private readonly Function<T, U> m_TriggerCallback;
                private readonly bool m_IsLoopTrigger;
                private uint m_StartFrame;

                public FrameTriggerTask(uint triggerID, uint frameNow, uint triggerAfter, T initiator, U userData, Function<T, U> callback, bool isLoop = false)
                {
                    Runtime.Debugger.Assert(null != callback, "Parameter 'callback' can not be null!");

                    m_TriggerID = triggerID;
                    m_StartFrame = frameNow;
                    m_TriggerAfter = triggerAfter;
                    m_TriggerCallback = callback;
                    m_Initiator = initiator;
                    m_UserData = userData;
                    m_IsLoopTrigger = isLoop;
                }

                public uint ID { get { return m_TriggerID; } }

                public bool CheckTrigger(uint frameNow)
                {
                    if (frameNow >= m_StartFrame + m_TriggerAfter)
                    {
                        m_TriggerCallback(m_Initiator, m_UserData);
                        if (m_IsLoopTrigger)
                        {
                            m_StartFrame = frameNow;
                            return false;
                        }
                        else
                            return true;
                    }
                    else
                        return false;
                }
            }

            private readonly LinkedList<ITMFrameTriggerTask> m_TriggerTaskList;
            private uint m_FrameTriggerCount;
            private byte m_FrameTriggerHandleType;

            public FrameTrigger()
            {
                m_TriggerTaskList = new LinkedList<ITMFrameTriggerTask>();
                m_FrameTriggerHandleType = 0xbf;
                m_FrameTriggerCount = 0;
            }

            public uint AddTrigger<T, U>(uint curFrame, uint triggerAfterFrame, T initiator, U userData, Function<T, U> callback, bool isLoop = false)
            {
                uint handle = Runtime.Utility.Handle.AllocHandle(m_FrameTriggerHandleType, ref m_FrameTriggerCount);
                FrameTriggerTask<T, U> task = new FrameTriggerTask<T, U>(handle, curFrame, triggerAfterFrame, initiator, userData, callback, isLoop);
                m_TriggerTaskList.AddLast(task);
                return handle;
            }

            public void RemoveTrigger(uint handle)
            {
                LinkedListNode<ITMFrameTriggerTask> cur = m_TriggerTaskList.First;
                while (null != cur)
                {
                    if (cur.Value.ID == handle)
                    {
                        m_TriggerTaskList.Remove(cur);
                        break;
                    }
                    cur = cur.Next;
                }
            }

            public void Shutdown()
            {
                m_TriggerTaskList.Clear();
            }

            public void Update(uint curFrame)
            {
                LinkedListNode<ITMFrameTriggerTask> cur = m_TriggerTaskList.First, next = null;
                while (null != cur)
                {
                    next = cur.Next;
                    if (cur.Value.CheckTrigger(curFrame))
                        m_TriggerTaskList.Remove(cur);
                    cur = next;
                }
            }
        }
    }
}