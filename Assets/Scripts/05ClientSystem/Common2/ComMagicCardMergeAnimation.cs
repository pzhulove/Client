using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

namespace GameClient
{
    [System.Serializable]
    public class MergeStep
    {
        public UnityEvent onEvent;
        public float time;
        public int stepId = 0;
    }

    class ComMagicCardMergeAnimation : MonoBehaviour
    {
        public List<MergeStep> events = new List<MergeStep>();
        public float eventsLength
        {
            get
            {
                float length = 0.0f;
                if(null != events)
                {
                    for(int i = 0; i < events.Count; ++i)
                    {
                        if(null != events[i])
                        length += events[i].time;
                    }
                }
                return length;
            }
        }

        public void AppendEvent(MergeStep mergeStep)
        {
            var find = events.Find(x => { return mergeStep.stepId == x.stepId; });
            if(null == find)
            {
                events.Add(mergeStep);
            }
            else
            {
                events.Remove(find);
                events.Add(mergeStep);
            }
        }

        // Use this for initialization
        void Start()
        {

        }

        public void TriggerEvents()
        {
            InvokeMethod.RemoveInvokeCall(this);
            float time = 0.0f;
            for(int i = 0; i < events.Count; ++i)
            {
                var curEvent = events[i];
                if(null != curEvent && null != curEvent.onEvent)
                {
                    InvokeMethod.Invoke(this,time,()=>
                    {
                        curEvent.onEvent.Invoke();
                    });
                    time += curEvent.time;
                }
            }
        }
    }
}