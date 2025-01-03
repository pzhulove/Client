using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using LimitTimeGift;
using System.Collections.Generic;
using ActivityLimitTime;
using Protocol;
using Network;
using Scripts.UI;
using UnityEngine.Events;
using ProtoTable;

namespace GameClient
{
    public delegate void OnBuyClickDelegate(MallItemInfo info);
    public class LimitTimePetGiftFrameView : MonoBehaviour
    {
        [SerializeField]private ComUIListScript mItemsUIList;
        [SerializeField]private Button mCloseBtn;
        [SerializeField]private SimpleTimer mSimpleTimer;

        List<MallItemInfo> mData;
        OnBuyClickDelegate mOnBuyClickDelegate;
        public void InitData(List<MallItemInfo> data, OnBuyClickDelegate onBuyClickDelegate)
        {
            mData = data;
            mOnBuyClickDelegate = onBuyClickDelegate;
            if (mCloseBtn)
            {
                mCloseBtn.onClick.RemoveAllListeners();
                mCloseBtn.onClick.AddListener(OnCloseClick);
            }
            
            if (mData.Count > 1)
            {
                uint endTime = (uint)Mathf.Max(mData[0].endtime, mData[1].endtime);
                InitSimpleTime(endTime);
            }
            else
            {
                InitSimpleTime(mData[0].endtime);
            }
           
            InitComUIListScript();
        }

        public void RefreshItemInfo(List<MallItemInfo> data)
        {
            mData = new List<MallItemInfo>();
            mData = data;

            SetElementAmount();
        }

        void InitComUIListScript()
        {
            mItemsUIList.Initialize();
            mItemsUIList.onBindItem += OnBindItemDelegate;
            mItemsUIList.onItemVisiable += OnItemVisiableDelegate;

            SetElementAmount();
        }

        ComCommonBind OnBindItemDelegate(GameObject itemObject)
        {
            ComCommonBind mItemBind = itemObject.GetComponent<ComCommonBind>();
            if (mItemBind != null)
            {
                return mItemBind;
            }

            return null;
        }

        void OnItemVisiableDelegate(ComUIListElementScript item)
        {
            ComCommonBind mBind = item.gameObjectBindScript as ComCommonBind;
            if (mBind != null && item.m_index >= 0 && item.m_index < mData.Count)
            {
                var info = mData[item.m_index];
                int mItemId = 0;
                if (info.giftItems.Length > 0)
                {
                    mItemId = (int)info.giftItems[0].id;
                }

                Button mItemTipsBtn = mBind.GetCom<Button>("TipsBtn");
                Text mPrice = mBind.GetCom<Text>("price");
                Button mBuyBtn = mBind.GetCom<Button>("Buy");
                UIGray mBuyGray = mBind.GetCom<UIGray>("Gray");
                Image mBg = mBind.GetCom<Image>("BG");
                Image mIcon = mBind.GetCom<Image>("Icon");
                
                ItemData mItemData = ItemDataManager.GetInstance().GetCommonItemTableDataByID(mItemId);
                if (mItemData != null)
                {
                    ETCImageLoader.LoadSprite(ref mBg, mItemData.GetQualityInfo().Background);
                }
                
                int petID = GetPetID(mItemId);
                PetTable petTable = TableManager.GetInstance().GetTableItem<PetTable>(petID);
                if (petTable != null)
                {
                    ETCImageLoader.LoadSprite(ref mIcon, petTable.IconPath);
                }

                if (mItemTipsBtn != null)
                {
                    mItemTipsBtn.onClick.RemoveAllListeners();
                    mItemTipsBtn.onClick.AddListener(() => 
                    {
                        if (mItemData != null)
                        {
                            ItemTipManager.GetInstance().ShowTip(mItemData);
                        }
                    });
                }


                if (mPrice != null)
                {
                    mPrice.text = ((int)info.discountprice).ToString();
                }

                if (mBuyBtn != null)
                {
                    mBuyBtn.onClick.RemoveAllListeners();
                    mBuyBtn.onClick.AddListener(() => 
                    {
                        if (mOnBuyClickDelegate != null)
                        {
                            mOnBuyClickDelegate.Invoke(info);
                        }
                    });
                }

                if (mBuyBtn != null && mBuyGray != null)
                {
                    mBuyBtn.image.raycastTarget = info.limittotalnum > 0;
                    mBuyGray.enabled = !(info.limittotalnum > 0);
                }
                
            }
        }

        void SetElementAmount()
        {
            mItemsUIList.SetElementAmount(mData.Count);
        }

        void InitSimpleTime(uint endTime)
        {
            int time = (int)(endTime - TimeManager.GetInstance().GetServerTime());
            if (mSimpleTimer)
            {
                mSimpleTimer.SetCountdown(time);
                mSimpleTimer.StartTimer();
            }
        }

        void OnCloseClick()
        {
            if (ClientSystemManager.GetInstance().IsFrameOpen<LimitTimePetGiftFrame>())
            {
                ClientSystemManager.GetInstance().CloseFrame<LimitTimePetGiftFrame>();
            }
        }

        private int GetPetID(int petEggID)
        {
            int mPetId = 0;
            var mPetDic = TableManager.GetInstance().GetTable<PetTable>().GetEnumerator();
            while (mPetDic.MoveNext())
            {
                var mPetTable = mPetDic.Current.Value as PetTable;
                if (mPetTable.PetEggID != petEggID)
                {
                    continue;
                }

                mPetId = mPetTable.ID;
                return mPetId;
            }

            return mPetId;
        }

        void OnDestroy()
        {
            if (mSimpleTimer)
            {
                mSimpleTimer.StopTimer();
                mSimpleTimer = null;
            }

            if (mItemsUIList)
            {
                mItemsUIList.onBindItem -= OnBindItemDelegate;
                mItemsUIList.onItemVisiable -= OnItemVisiableDelegate;
                mItemsUIList = null;
            }

            mData = null;
            mOnBuyClickDelegate = null;
        }
    }
}
