using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using Protocol;

namespace GameClient
{
    public delegate void OnItemSelect(ulong guid);
    public delegate void OnSetPrice(uint price);
    
    class BlackMarketMerchantTradeItem : MonoBehaviour
    {
        [SerializeField]
        private GameObject mItemParent;

        [SerializeField]
        private Text mName;

        [SerializeField]
        private GameObject mCheckMark;

        private ulong mGUID = 0;
        public ulong GUID
        {
            get { return mGUID; }
            set { mGUID = value; }
        }

        ComItem comItem = null;
        public void OnItemVisiable(ItemData data)
        {
            mGUID = data.GUID;

            if (comItem == null)
            {
                comItem = ComItemManager.Create(mItemParent);
            }

            ItemData itemData = ItemDataManager.GetInstance().GetItem(mGUID);
            if (itemData != null)
            {
                comItem.Setup(itemData, Utility.OnItemClicked);
                mName.text = itemData.GetColorName();
            }
            
        }

        public void OnItemChangeDisplay(bool bSelected)
        {
            mCheckMark.CustomActive(bSelected);
        }
        void OnDestroy()
        {
            mGUID = 0;
            comItem = null;
        }
    }
}

