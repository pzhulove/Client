using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;

namespace GameClient
{
    public delegate void OnSelectedJobClick(int jobId);
    public class JobItem : MonoBehaviour
    {
        [SerializeField] private int iMaxJobCount = 2;
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private GameObject parentRoot;
        [SerializeField] private GameObject prefab;
 
        private OnSelectedJobClick onSelectedJobClick;
        private ChangeJobData changeJobData;
        private bool bIsSelected = false;

        private List<GameObject> goList = new List<GameObject>();
        private void Awake()
        {

        }

        private void OnDestroy()
        {
            onSelectedJobClick = null;
            changeJobData = null;
            bIsSelected = false;
            goList.Clear();
        }

        public void OnItemVisiable(int index,ChangeJobData changeJobData, OnSelectedJobClick onSelectedJobClick,bool bIsSelected)
        {
            if(changeJobData == null || onSelectedJobClick == null)
            {
                return;
            }

            this.changeJobData = changeJobData;
            this.onSelectedJobClick = onSelectedJobClick;
            this.bIsSelected = bIsSelected;

            //设置偏移量
            //if(index % 2 == 0)
            //{
            //    rectTransform.anchoredPosition = new Vector3(102.0f, rectTransform.anchoredPosition.y);
            //}
            
            for (int i = 0; i < iMaxJobCount; i++)
            {
                if(i < goList.Count)
                {
                    GameObject go = goList[i];
                    OnItemVisiable(i, go);
                }
                else
                {
                    GameObject go = GameObject.Instantiate(prefab);
                    go.CustomActive(true);
                    Utility.AttachTo(go, parentRoot);
                    OnItemVisiable(i, go);
                    goList.Add(go);
                }
            }
        }

        private void OnItemVisiable(int index,GameObject go)
        {
            ChangeJobNewItem changeJobNewItem = go.GetComponent<ChangeJobNewItem>();
            if (changeJobNewItem != null)
            {
                if (index < changeJobData.changeJobList.Count)
                {
                    int jobId = changeJobData.changeJobList[index];

                    if (bIsSelected)
                    {
                        changeJobNewItem.OnItemVisiable(jobId, onSelectedJobClick, index == 0);
                    }
                    else
                    {
                        changeJobNewItem.OnItemVisiable(jobId, onSelectedJobClick, false);
                    }

                    //changeJobNewItem.UpdateCheckMarkGo();
                }
                else
                {
                    //未开放
                    changeJobNewItem.OnItemVisiable(0, null, false);
                }
            }
        }
    }
}
