using System;
using System.Collections.Generic;
using Scripts.UI;
using UnityEngine;
using UnityEngine.UI;
using ProtoTable;
using Protocol;

namespace GameClient
{

    public class HonorTotalController : MonoBehaviour
    {
        
        [Space(5)]
        [HeaderAttribute("TotalItemList")]
        [Space(5)]
        [SerializeField] private ComUIListScript totalActivityItemList;

        private List<PvpNumberStatistics> totalActivityDataModelList;

        private void Awake()
        {
            BindEvents();
        }

        private void OnDestroy()
        {
            UnBindEvents();
            ClearData();
        }

        private void BindEvents()
        {
            if (totalActivityItemList != null)
            {
                totalActivityItemList.Initialize();
                totalActivityItemList.onItemVisiable += OnTotalActivityItemVisible;
            }
        }

        private void UnBindEvents()
        {
            if (totalActivityItemList != null)
                totalActivityItemList.onItemVisiable -= OnTotalActivityItemVisible;
        }

        private void ClearData()
        {
            totalActivityDataModelList = null;
        }


        public void InitHonorTotalController()
        {
            //活动总计
            InitHonorTotalActivityItemList();
        }
        

        private void InitHonorTotalActivityItemList()
        {
            if (totalActivityItemList == null)
                return;

            var totalActivityItemNumber = 0;

            //总计活动的数据
            totalActivityDataModelList = HonorSystemUtility.GetPvpNumberStaticsListByDateType(
                    HONOR_DATE_TYPE.HONOR_DATE_TYPE_TOTAL);

            if (totalActivityDataModelList != null && totalActivityDataModelList.Count > 0)
                totalActivityItemNumber = totalActivityDataModelList.Count;

            totalActivityItemList.SetElementAmount(totalActivityItemNumber);

        }

        private void OnTotalActivityItemVisible(ComUIListElementScript item)
        {
            if (totalActivityItemList == null)
                return;

            if (item == null)
                return;

            if (totalActivityDataModelList == null || totalActivityDataModelList.Count <= 0)
                return;

            if (item.m_index < 0 || item.m_index >= totalActivityDataModelList.Count)
                return;

            var totalActivityDataModel = totalActivityDataModelList[item.m_index];
            var totalActivityItem = item.GetComponent<HonorTotalItem>();
            if (totalActivityItem != null
                && totalActivityDataModel != null)
            {
                totalActivityItem.InitItem(totalActivityDataModel);
            }
        }
    }
}