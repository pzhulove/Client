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

    //两张图片交替展示
    public class CommonGameObjectTwinkleControl : MonoBehaviour
    {
        
        private bool isShowFirst = false;
        [SerializeField] private float intervalTime = 0.5f;

        [SerializeField] private GameObject firstGameObject;
        [SerializeField] private GameObject secondGameObject;
        
        private void OnDisable()
        {
            StopAllCoroutines();
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }
        
        public void DoTwinkleAction()
        {
            //首先关闭所有的携程，之后再次开启携程
            StopAllCoroutines();

            isShowFirst = true;
            UpdateGameObject(isShowFirst);

            InvokeRepeating("OnDoTwinkleAction", 0, intervalTime);
        }

        public void ResetTwinkleAction()
        {
            CancelInvoke("OnDoTwinkleAction");
        }

        private void OnDoTwinkleAction()
        {
            if (isShowFirst == true)
            {
                isShowFirst = false;
            }
            else
            {
                isShowFirst = true;
            }

            UpdateGameObject(isShowFirst);
            
        }

        private void UpdateGameObject(bool flag)
        {
            CommonUtility.UpdateGameObjectVisible(firstGameObject, flag);
            CommonUtility.UpdateGameObjectVisible(secondGameObject, !flag);
        }
        
    }
}