using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GameClient
{
    public class ComMonoBehaviorCallBack : MonoBehaviour
    {
        public UnityEvent onMonoBehaviorEnable;
        public UnityEvent onMonoBehaviorDisable;

        private void OnEnable()
        {
            if(null != onMonoBehaviorEnable)
            {
                onMonoBehaviorEnable.Invoke();
            }
        }

        private void OnDisable()
        {
            if (null != onMonoBehaviorDisable)
            {
                onMonoBehaviorDisable.Invoke();
            }
        }
    }
}