using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using UnityEngine.Events;

namespace GameClient
{
    //间隔一定的时间，触发一下
    public class CommonTimeRefreshControl : MonoBehaviour
    {

        [SerializeField] private float intervalTime = 0.0f;

        private Action _invokeAction = null;
        private float _tempTime = 0.0f;

        public void SetInvokeAction(Action invokeAction)
        {
            _invokeAction = invokeAction;
            _tempTime = 0.0f;
        }

        public void ResetInvokeAction()
        {
            _invokeAction = null;
            _tempTime = 0.0f;
        }

        private void Update()
        {
            if (_invokeAction == null)
                return;

            if (intervalTime <= 0.0f)
                return;

            _tempTime += Time.deltaTime;
            if (_tempTime >= intervalTime)
            {
                _tempTime = 0.0f;
                if (_invokeAction != null)
                    _invokeAction();
            }
        }

    }

}
