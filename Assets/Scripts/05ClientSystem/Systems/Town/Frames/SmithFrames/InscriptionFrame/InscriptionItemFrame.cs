using Scripts.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GameClient
{
    public class InscriptionItemDataModel
    {
        public int InscriptionHoleColor;
        public UnityAction<ItemData> OnSelectedClick;
    }

    public class InscriptionItemFrame : ClientFrame
    {
        InscriptionItemDataModel mDataModel;
        private List<ItemData> mInscriptionItemDataList = new List<ItemData>();
        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/SmithShop/InscriptionFrame/InscriptionItemFrame";
        }

        protected sealed override void _OnOpenFrame()
        {
            InitUIList();
            mDataModel = userData as InscriptionItemDataModel;
            if (mDataModel != null)
            {
                mInscriptionItemDataList = InscriptionMosaicDataManager.GetInstance().GetCanMosaicInscription(mDataModel.InscriptionHoleColor);
            }

            mUIList.SetElementAmount(mInscriptionItemDataList.Count);
        }

        protected sealed override void _OnCloseFrame()
        {
            UnInitUIList();
            mDataModel = null;
            mInscriptionItemDataList.Clear();
        }

        private void InitUIList()
        {
            if (mUIList != null)
            {
                mUIList.Initialize();
                mUIList.onBindItem += OnBindItemDelegate;
                mUIList.onItemVisiable += OnItemVisiableDelegate;
                mUIList.onItemSelected += OnItemSelectedDelegate;
                mUIList.onItemChageDisplay += OnItemChangeDisplayDelegate;
            }
        }

        private void UnInitUIList()
        {
            if (mUIList != null)
            {
                mUIList.onBindItem -= OnBindItemDelegate;
                mUIList.onItemVisiable -= OnItemVisiableDelegate;
                mUIList.onItemSelected -= OnItemSelectedDelegate;
                mUIList.onItemChageDisplay -= OnItemChangeDisplayDelegate;
            }
        }

        private InscriptionItemElement OnBindItemDelegate(GameObject itemObject)
        {
            return itemObject.GetComponent<InscriptionItemElement>();
        }

        private void OnItemVisiableDelegate(ComUIListElementScript item)
        {
            var itemElement = item.gameObjectBindScript as InscriptionItemElement;
            if (itemElement != null && item.m_index >= 0 && item.m_index < mInscriptionItemDataList.Count)
            {
                itemElement.OnItemVisiable(mInscriptionItemDataList[item.m_index]);
            }
        }

        private void OnItemSelectedDelegate(ComUIListElementScript item)
        {
            var itemElement = item.gameObjectBindScript as InscriptionItemElement;
            if (itemElement != null)
            {
                if (mDataModel.OnSelectedClick != null)
                {
                    mDataModel.OnSelectedClick(itemElement.CurrentItemData);
                }

                Close();
            }
        }

        private void OnItemChangeDisplayDelegate(ComUIListElementScript item,bool isSelected)
        {
            var itemElement = item.gameObjectBindScript as InscriptionItemElement;
            if (itemElement != null)
                itemElement.OnItemChangeDisplay(isSelected);
        }

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
    }
}