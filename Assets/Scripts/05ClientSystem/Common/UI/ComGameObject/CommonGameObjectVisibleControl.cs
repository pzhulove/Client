using System;
using System.Collections;
using System.Collections.Generic;
using Protocol;
using ProtoTable;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;


namespace GameClient
{

    //展示一段时间间隔，自动隐藏
    public class CommonGameObjectVisibleControl : MonoBehaviour
    {

        private float _intervalTime = 0.0f;
        
        //需要展示的时间
        [SerializeField] private float visibleTime = 2.0f;

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }

        public void SetVisibleTime(float showTime)
        {
            visibleTime = showTime;
        }

        public void SetVisibleControl()
        {
            if (visibleTime <= 0.0f)
                return;

            //首先关闭所有的携程，之后再次开启携程
            StopAllCoroutines();

            _intervalTime = visibleTime;
            StartCoroutine(StartCountDown());
        }

        private IEnumerator StartCountDown()
        {
            while (_intervalTime > 0.0f)
            {
                _intervalTime -= 0.5f;
                yield return new WaitForSeconds(0.5f);
            }

            _intervalTime = 0.0f;
            transform.gameObject.CustomActive(false);
        }
        
    }
}