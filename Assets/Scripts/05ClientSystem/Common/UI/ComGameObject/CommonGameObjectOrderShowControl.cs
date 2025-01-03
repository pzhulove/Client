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

    //图片顺序展示
    public class CommonGameObjectOrderShowControl : MonoBehaviour
    {

        private float _intervalTime = 0.3f;
        private int _orderIndex = 0;
        private Action _orderShowFinishAction = null;

        [SerializeField] private float totalShowTime = 1.0f;
        [SerializeField] private List<GameObject> goList = new List<GameObject>();


        private void OnDestroy()
        {
            _orderShowFinishAction = null;
            StopAllCoroutines();
        }

        public void Init(Action orderShowFinishAction)
        {
            StopAllCoroutines();

            if (goList == null)
                return;

            var goCount = goList.Count;
            if (goCount <= 0)
                return;

            if (totalShowTime <= 0)
                return;

            _intervalTime = totalShowTime / (float) goCount;
            _orderShowFinishAction = orderShowFinishAction;
            _orderIndex = 0;

            ResetGameObject();

            DoGameObjectOrderShow();
        }

        private void DoGameObjectOrderShow()
        {
            InvokeRepeating("OnDoGameObjectOrderShow", 0, _intervalTime);

        }

        public void OnDoGameObjectOrderShow()
        {
            if (_orderIndex < goList.Count)
            {
                CommonUtility.UpdateGameObjectVisible(goList[_orderIndex], true);
            }

            _orderIndex = _orderIndex + 1;

            if (_orderIndex >= goList.Count)
            {
                FinishGameObjectOrderShow();

                if (_orderShowFinishAction != null)
                    _orderShowFinishAction();
            }
        }

        public void FinishGameObjectOrderShow()
        {
            CancelInvoke("OnDoGameObjectOrderShow");
        }

        private void ResetGameObject()
        {
            for (var i = 0; i < goList.Count; i++)
            {
                CommonUtility.UpdateGameObjectVisible(goList[i], false);
            }
        }

    }
}