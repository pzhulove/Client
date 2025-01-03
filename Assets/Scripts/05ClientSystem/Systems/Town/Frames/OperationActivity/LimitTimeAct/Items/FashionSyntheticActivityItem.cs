using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class FashionSyntheticActivityItem : MonoBehaviour
    {
        [SerializeField] private GameObject mItemParent;
        [SerializeField] private Text mPrice;
        [SerializeField] private Text mLimit;
        [SerializeField] private Button mBuyBtn;
        [SerializeField] private UIGray mBuyGray;
        [SerializeField] private Text mName;

        private LimitTimeGiftPackDetailModel limitTimeGiftPackDetailModel;
        private int index = 0;
        private ActivityItemBase.OnActivityItemClick<int> onItemClick;
        private ComItem comItem;

        public void OnItemVisiable(LimitTimeGiftPackDetailModel model,int index, ActivityItemBase.OnActivityItemClick<int> onItemClick)
        {
            limitTimeGiftPackDetailModel = model;
            this.index = index;
            this.onItemClick = onItemClick;

            if (comItem == null)
            {
                comItem = ComItemManager.Create(mItemParent);
            }

            if (model.mRewards.Length > 0)
            {
                ItemData itemData = ItemDataManager.CreateItemDataFromTable((int)model.mRewards[0].id);
                if (itemData != null)
                {
                    itemData.Count = (int)model.mRewards[0].num;

                    comItem.Setup(itemData, Utility.OnItemClicked);

                    mName.SafeSetText(itemData.GetColorName());
                }
            }
            
            mPrice.SafeSetText(string.Format("{0}点券", model.GiftPrice));
            mLimit.SafeSetText(TR.Value("mall_new_limit_player_forever_limit", model.LimitPurchaseNum -
                CountDataManager.GetInstance().GetCount(model.Id.ToString())));

            mBuyBtn.SafeRemoveAllListener();
            mBuyBtn.SafeAddOnClickListener(OnBuyBtnClick);

            if (mBuyGray != null)
            {
                mBuyGray.enabled = !(model.LimitPurchaseNum -
                CountDataManager.GetInstance().GetCount(model.Id.ToString()) > 0);
            }

            if (mBuyBtn != null)
            {
                mBuyBtn.image.raycastTarget = model.LimitPurchaseNum -
                CountDataManager.GetInstance().GetCount(model.Id.ToString()) > 0;
            }
        }

        private void OnBuyBtnClick()
        {
            if (onItemClick != null)
            {
                onItemClick.Invoke(index, 0, 0);
            }
        }

        private void OnDestroy()
        {
            index = 0;
            onItemClick = null;
            comItem = null;
        }
    }
}