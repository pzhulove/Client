using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GameClient
{
    [System.Serializable]
    public class DelayAction
    {
        public float time;
        public UnityEvent action;
    }

    public class ComDelayInvoke : MonoBehaviour
    {
        public DelayAction[] actions = new DelayAction[0];
        bool mDirty = false;

        public void ClearAllInvokes()
        {
            InvokeMethod.RemoveInvokeCall(this);
            mDirty = false;
        }

        public void AddAllInvokes()
        {
            mDirty = true;
        }

        void OnDestroy()
        {
            ClearAllInvokes();
        }

        void Update()
        {
            if(!mDirty)
            {
                return;
            }

            InvokeMethod.RemoveInvokeCall(this);

            for (int i = 0; i < actions.Length; ++i)
            {
                if (null != actions[i])
                {
                    var action = actions[i].action;
                    InvokeMethod.Invoke(this, actions[i].time, () =>
                    {
                        if (null != action)
                        {
                            action.Invoke();
                        }
                    });
                }
            }

            mDirty = false;
        }
    }
}