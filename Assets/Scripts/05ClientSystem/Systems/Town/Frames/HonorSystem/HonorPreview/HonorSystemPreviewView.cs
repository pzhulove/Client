using System;
using System.Collections.Generic;
using Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{

    public class HonorSystemPreviewView : MonoBehaviour
    {

        private List<PreviewLevelItemDataModel> _previewLevelItemDataModelList;

        [Space(10)] [HeaderAttribute("PreviewLevelItemList")] [Space(10)]
        [SerializeField] private ComUIListScriptEx previewLevelItemList;


        private void Awake()
        {
            BindEvents();
        }

        private void OnDestroy()
        {
            UnBindEvents();
            ClearData();
        }

        private void ClearData()
        {
            if (_previewLevelItemDataModelList != null)
            {
                for (var i = 0; i < _previewLevelItemDataModelList.Count; i++)
                {
                    var levelItemDataModel = _previewLevelItemDataModelList[i];
                    if(levelItemDataModel == null
                       || levelItemDataModel.UnLockShopItemList == null
                       || levelItemDataModel.UnLockShopItemList.Count <= 0)
                        continue;

                    levelItemDataModel.UnLockShopItemList.Clear();
                }
                _previewLevelItemDataModelList.Clear();
                _previewLevelItemDataModelList = null;
            }
        }

        #region PreviewLevelItemList

        private void BindEvents()
        {

            if (previewLevelItemList != null)
            {
                previewLevelItemList.Initialize();
                previewLevelItemList.onItemVisiable += OnPreviewLevelItemVisible;
                previewLevelItemList.OnItemRecycle += OnPreviewLevelItemRecycle;
            }
        }

        private void UnBindEvents()
        {
            if (previewLevelItemList != null)
            {
                previewLevelItemList.onItemVisiable -= OnPreviewLevelItemVisible;
                previewLevelItemList.OnItemRecycle -= OnPreviewLevelItemRecycle;
            }
        }

        private void OnPreviewLevelItemVisible(ComUIListElementScript item)
        {
            if (item == null)
                return;

            if (_previewLevelItemDataModelList == null || _previewLevelItemDataModelList.Count <= 0)
                return;

            if (item.m_index < 0 || item.m_index >= _previewLevelItemDataModelList.Count)
                return;

            var previewLevelItemDataModel = _previewLevelItemDataModelList[item.m_index];
            var previewLevelItem = item.GetComponent<HonorSystemPreviewItem>();

            if (previewLevelItem != null
                && previewLevelItemDataModel != null)
                previewLevelItem.InitPreviewItem(previewLevelItemDataModel);
        }

        private void OnPreviewLevelItemRecycle(ComUIListElementScript item)
        {
            if (item == null)
                return;

            var previewLevelItem = item.GetComponent<HonorSystemPreviewItem>();
            if(previewLevelItem != null)
                previewLevelItem.OnRecycleItem();
        }

        #endregion


        public void InitView()
        {
            InitPreviewLevelData();

            InitPreviewLevelItemList();
        }

        //获得等级数据
        private void InitPreviewLevelData()
        {
            _previewLevelItemDataModelList = HonorSystemUtility.GetPreviewLevelItemDataModelList();
        }

        private void InitPreviewLevelItemList()
        {
            if (previewLevelItemList == null)
                return;

            var count = 0;
            if (_previewLevelItemDataModelList != null)
                count = _previewLevelItemDataModelList.Count;

            previewLevelItemList.SetElementAmount(count);

            var index = (int)HonorSystemDataManager.GetInstance().PlayerHonorLevel;
            previewLevelItemList.MoveItemToFirstPosition(index);
        }

    }
}