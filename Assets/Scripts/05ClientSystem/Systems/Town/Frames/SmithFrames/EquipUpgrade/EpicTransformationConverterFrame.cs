using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Scripts.UI;

namespace GameClient
{
    public class EpicTransformationConverterDataModel
    {
        public List<ItemData> converterList;
        public Action<ItemData> onConverterClick;
    }

    /// <summary>
    /// 史诗转化器界面
    /// </summary>
    public class EpicTransformationConverterFrame : ClientFrame
    {
        #region ExtraUIBind
        private ComUIListScript mUIList = null;

        protected sealed override void _bindExUI()
        {
            mUIList = mBind.GetCom<ComUIListScript>("UIList");
        }

        protected sealed override void _unbindExUI()
        {
            mUIList = null;
        }
        #endregion

        private List<ItemData> mConverterList = new List<ItemData>();
        private EpicTransformationConverterDataModel mEpicTransformationConverterDataModel;
        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/SmithShop/EquipUpgrade/EpicTransformationConverterFrame";
        }

        protected sealed override void _OnOpenFrame()
        {
            InitEquipmentUIListScript();

            mEpicTransformationConverterDataModel = userData as EpicTransformationConverterDataModel;

            if (mEpicTransformationConverterDataModel == null)
            {
                return;
            }

            mConverterList = mEpicTransformationConverterDataModel.converterList;

            mUIList.SetElementAmount(mConverterList.Count);
        }

        protected sealed override void _OnCloseFrame()
        {
            UnInitEquipmentUIListScript();
            mConverterList.Clear();
        }

        #region EquipmentUIListScript 
        
        private void InitEquipmentUIListScript()
        {
            if (mUIList != null)
            {
                mUIList.Initialize();
                mUIList.onBindItem += OnBindItemDelegate;
                mUIList.onItemVisiable += OnItemVisiableDelegate;
                mUIList.onItemSelected += OnItemSelectedDelegate;
            }
        }

        private void UnInitEquipmentUIListScript()
        {
            if (mUIList != null)
            {
                mUIList.onBindItem -= OnBindItemDelegate;
                mUIList.onItemVisiable -= OnItemVisiableDelegate;
                mUIList.onItemSelected -= OnItemSelectedDelegate;
            }
        }

        private GrowthExpendItem OnBindItemDelegate(GameObject itemObject)
        {
            return itemObject.GetComponent<GrowthExpendItem>();
        }

        private void OnItemVisiableDelegate(ComUIListElementScript item)
        {
            var converterItem = item.gameObjectBindScript as GrowthExpendItem;
            if (converterItem != null && item.m_index >= 0 && item.m_index < mConverterList.Count)
            {
                converterItem.OnItemVisiable(mConverterList[item.m_index]);
            }
        }

        private void OnItemSelectedDelegate(ComUIListElementScript item)
        {
            var converterItem = item.gameObjectBindScript as GrowthExpendItem;
            if (converterItem != null)
            {
                if (mEpicTransformationConverterDataModel != null &&
                    mEpicTransformationConverterDataModel.onConverterClick != null)
                {
                    mEpicTransformationConverterDataModel.onConverterClick.Invoke(converterItem.ItemData);
                    Close();
                }
            }
        }
        #endregion
    }
}