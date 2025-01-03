using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class ExpendBeadItem : MonoBehaviour
    {
        [SerializeField]
        private GameObject mItemParent;
        [SerializeField]
        private Text mBeadName;
        [SerializeField]
        private Text mCheckBeadName;
        [SerializeField]
        private Text mBeadArrt;
        [SerializeField]
        private Text mCheckBeadArrt;
        [SerializeField]
        private GameObject mCheckRoot;
        [SerializeField]
        private Text mCount;
        [SerializeField]
        GameObject mHintGo;

        ExpendBeadItemData mSimpleData ;
        public ExpendBeadItemData SimpleData
        {
            get
            {
                return mSimpleData == null ? null : mSimpleData;
            }
        }

        public void OnItemChangeDisplay(bool bSelected)
        {
            mCheckRoot.CustomActive(bSelected);
        }

        public void OnItemVisible(ExpendBeadItemData data)
        {
            mSimpleData = data;
            ItemData mItemData = ItemDataManager.GetInstance().GetCommonItemTableDataByID(data.ItemID);
            if (mItemData != null)
            {
                ComItemNew mComItem = ComItemManager.CreateNew(mItemParent);
                mComItem.Setup(mItemData, Utility.OnItemClicked);
            }
            bool isShow = data.TatleCount > 0 ? true : false;
            mCount.CustomActive(isShow);
            mHintGo.CustomActive(!isShow);
            mCount.text = string.Format("拥有{0}",data.TatleCount);
            mBeadName.text = mCheckBeadName.text = mItemData.GetColorName();
            mBeadArrt.text = mCheckBeadArrt.text = BeadCardManager.GetInstance().GetAttributesDesc(data.ItemID);
        }
    }
}
