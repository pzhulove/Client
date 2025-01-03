using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;

namespace GameClient
{
    public delegate void OnBuyClick(TopUpPushItemData pushItemData);
    public class TopUpPushItem : MonoBehaviour
    {
        [SerializeField] private GameObject mItemParent;
        [SerializeField] private Text mName;
        [SerializeField] private Text mPrice;
        [SerializeField] private Text mDiscountPrice;
        [SerializeField] private Text mLimitBuyCount;
        [SerializeField] private Button mBuyBtn;
        [SerializeField] private UIGray mBuyGray;

        TopUpPushItemData topUpPushItemData;
        private OnBuyClick mOnBuyClick;

        public void OnItemVisiable(TopUpPushItemData topUpPushItemData, OnBuyClick callBack)
        {
            this.topUpPushItemData = topUpPushItemData;
            mOnBuyClick = callBack;

            ComItem comItem = ComItemManager.Create(mItemParent);

            ItemData itemData = ItemDataManager.GetInstance().GetCommonItemTableDataByID(this.topUpPushItemData.itemId);
            itemData.Count = this.topUpPushItemData.itemCount;

            comItem.Setup(itemData, Utility.OnItemClicked);

            //名字
            mName.text = itemData.GetColorName();

            //原价
            mPrice.text = this.topUpPushItemData.price.ToString();

            //折扣价
            mDiscountPrice.text = this.topUpPushItemData.disCountPrice.ToString();

            //剩余次数
            int iResidueDegree = this.topUpPushItemData.maxTimes - this.topUpPushItemData.buyTimes;

            //限购次数
            mLimitBuyCount.text = string.Format("限购次数:{0}/{1}", iResidueDegree, this.topUpPushItemData.maxTimes);

            mBuyBtn.onClick.RemoveAllListeners();
            mBuyBtn.onClick.AddListener(OnBuyClick);

            mBuyGray.enabled = iResidueDegree <= 0;
        }

        void OnBuyClick()
        {
            if (mOnBuyClick != null)
            {
                mOnBuyClick.Invoke(topUpPushItemData);
            }
        }

        void OnDestroy()
        {
            topUpPushItemData = null;
            if (mOnBuyClick != null)
            {
                mOnBuyClick = null;
            }

            if (mBuyBtn != null)
            {
                mBuyBtn.onClick.RemoveListener(OnBuyClick);
            }

            mBuyBtn = null;
        }
    }
}

