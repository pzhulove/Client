using ProtoTable;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameClient
{
    class WeaponLeaseItem : MonoBehaviour, IDisposable
    {
        [SerializeField]
        private GameObject mItemParent;
        [SerializeField]
        private Text mName;
        [SerializeField]
        private Image mTicketIcon;
        [SerializeField]
        private Text mTicketCount;
        [SerializeField]
        private Button mLeaseBtn;
        [SerializeField]
        private GameObject mGoLease;
        [SerializeField]
        private GameObject mGoInTheLease;
        [SerializeField]
        private GameObject mGoReCommend;
        [SerializeField]
        private Text mLeaseBtnIcon;

        public delegate void OnClickLease(GoodsData goodsData);
        private OnClickLease mOnClickLease;
        WeaponLeaseShopFrame frame;
        GoodsData goodsData;
        ComItem mComitem;
        bool isRefresh = false;
        int timer = 0;
        public void UpdateWeaponLeaseItem(WeaponLeaseShopFrame frame,GoodsData goodsData, OnClickLease onClickLease)
        {
            this.frame = frame;
            this.goodsData = goodsData;
            this.mOnClickLease = onClickLease;

            if (mComitem == null)
            {
                mComitem = ComItemManager.Create(mItemParent);
            }

            mComitem.Setup(this.goodsData.ItemData, Utility.OnItemClicked);
            mName.text = this.goodsData.ItemData.GetColorName();
            ETCImageLoader.LoadSprite(ref mTicketIcon, this.goodsData.CostItemData.Icon);
            int iCurCount = ItemDataManager.GetInstance().GetOwnedItemCount((int)this.goodsData.CostItemData.TableID);
            mTicketCount.text = this.goodsData.CostItemCount.Value.ToString();
            mTicketCount.color = iCurCount >= this.goodsData.CostItemCount ? Color.white : Color.red;
            mGoReCommend.CustomActive(ShopDataManager.GetInstance().WeaponLeaseIsRecommendOccu(this.goodsData.ItemData.TableID));
            mLeaseBtn.onClick.RemoveAllListeners();
            mLeaseBtn.onClick.AddListener(() => 
            {
                if (mOnClickLease != null)
                {
                    mOnClickLease.Invoke(this.goodsData);
                }
            });

            RefreshButtonState();
            UpdatemLeaseBtnIcon();
        }

        void RefreshButtonState()
        {
            if (this.goodsData.LeaseEndTimeStamp - TimeManager.GetInstance().GetServerTime() > 0)
            {
                mGoInTheLease.CustomActive(true);
                mGoLease.CustomActive(false);
                mTicketIcon.CustomActive(false);
                mTicketCount.CustomActive(false);
            }
            else
            {
                mGoInTheLease.CustomActive(false);
                mGoLease.CustomActive(true);
                mTicketIcon.CustomActive(true);
                mTicketCount.CustomActive(true);
            }
        }

        void UpdatemLeaseBtnIcon()
        {
            if (this.goodsData.ItemData.SubType == (int)ItemTable.eSubType.ST_WEAPON_LEASE_TICKET)
            {
                mLeaseBtnIcon.SafeSetText("购买");
            }
            else
            {
                mLeaseBtnIcon.SafeSetText("租赁");
            }
        }

        void Update()
        {
            timer += (int)Time.deltaTime;
            if (timer > 10)
            {
                if (goodsData.LeaseEndTimeStamp - TimeManager.GetInstance().GetServerTime() <= 0)
                {
                    isRefresh = true;
                }

                if (isRefresh)
                {
                    RefreshButtonState();
                    isRefresh = false;
                }

                timer = 0;
            }
           
        }
        public void OnDestroy()
        {
            Dispose();
        }

        public void Dispose()
        {
            frame = null;
            goodsData = null;
            mOnClickLease = null;
            if (mComitem != null)
            {
                mComitem.Setup(null, null);
                mComitem = null;
            }

            if (mLeaseBtn != null)
            {
                mLeaseBtn.onClick.RemoveAllListeners();
            }
        }
    }
}

