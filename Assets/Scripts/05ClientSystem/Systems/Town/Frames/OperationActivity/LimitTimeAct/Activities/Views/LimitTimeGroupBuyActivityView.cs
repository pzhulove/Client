using Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProtoTable;

namespace GameClient
{
    public enum StageType
    {
        Frist = 0,
        Second,
        Third,
        Fourth,
    }

    public class LimitTimeGroupBuyDataModel
    {
        /// <summary>
        /// 玩家购礼包买的数量
        /// </summary>
        public int playerJoinNum;

        /// <summary>
        /// 参与人数
        /// </summary>
        public int joinNum;

        /// <summary>
        /// 最大人数
        /// </summary>
        public int maxNum;

        /// <summary>
        /// 全服折扣
        /// </summary>
        public int discount;
    }

    public class LimitTimeGroupBuyActivityView : MonoBehaviour, IDisposable, IGiftPackActivityView
    {

        [SerializeField] private Text mTimeTxt;
        [SerializeField] private Text mBuyCountTxt;
        [SerializeField] private Text mItemNameTxt;
        [SerializeField] private Text mItemPriceTxt;
        [SerializeField] private Button mCheckViewBtn;
        [SerializeField] private Button mItemIconBtn;
        [SerializeField] private Button mBuyBtn;
        [SerializeField] private Image mItemBackgroundImg;
        [SerializeField] private Image mItemIconImg;
        [SerializeField] private Slider mFirstProgress;
        [SerializeField] private Slider mSecondProgress;
        [SerializeField] private Slider mThirdProgress;
        [SerializeField] private Slider mFourthProgress;
        [SerializeField] private Text mFirstNumberText;
        [SerializeField] private Text mSecondNumberText;
        [SerializeField] private Text mThirdNumberText;
        [SerializeField] private Text mFourthNumberText;
        [SerializeField] private Text mFirstDiscountText;
        [SerializeField] private Text mSecondDiscountText;
        [SerializeField] private Text mThirdDiscountText;
        [SerializeField] private Text mDiscountText;
        [SerializeField] private Button mFashionPreviewBtn;

        private float mFirstNumber = 2000;//第一阶段人数
        private float mSecondNumber = 3000;//第二阶段人数
        private float mThirdNumber = 5000;//第三阶段人数
        private float mFourthNumber = 10000;//第四阶段人数

        private float mFirstDiscount;
        private float mSecondDiscount;
        private float mThirdDiscount;

        private ActivityItemBase.OnActivityItemClick<int> mOnItemClick;
        private LimitTimeGiftPackModel mModel;
        private float currentNumber = 0;
        private float currentTotalNumber = 0;

        [SerializeField] private string discountDes = "{0}折";
        [SerializeField] private string numberDes ="{0}张";
        [SerializeField] private string fullServiceDiscountDes = "当前全服折扣:{0}折";

        private int iFashionGiftId = 0;
        public void Init(LimitTimeGiftPackModel model, ActivityItemBase.OnActivityItemClick<int> onItemClick)
        {
            mModel = model;
            mOnItemClick = onItemClick;
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnGASWholeBargainResSuccessed, OnGASWholeBargainResSuccessed);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnSyncWorldMallBuySucceed, OnSyncWorldMallBuySucceed);

            var systemValue = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType3.SVT_BARGAIN_PREVIEW_ITEM_ID); 
            if (systemValue != null)
            {
                iFashionGiftId = systemValue.Value;
            }

            mTimeTxt.SafeSetText(string.Format("{0}~{1}", Function._TransTimeStampToStr(mModel.StartTime), Function._TransTimeStampToStr(mModel.EndTime)));

            mCheckViewBtn.SafeRemoveAllListener();
            mCheckViewBtn.SafeAddOnClickListener(OnCheckViewBtnClick);

            mFashionPreviewBtn.SafeRemoveAllListener();
            mFashionPreviewBtn.SafeAddOnClickListener(OnFashionPreviewBtnClick);

            mBuyBtn.SafeRemoveAllListener();
            mBuyBtn.SafeAddOnClickListener(OnBuyBtnClick);

            InitData();

            mFirstNumberText.SafeSetText(string.Format(numberDes,mFirstNumber));
            mSecondNumberText.SafeSetText(string.Format(numberDes, mSecondNumber));
            mThirdNumberText.SafeSetText(string.Format(numberDes, mThirdNumber));
            mFourthNumberText.SafeSetText(string.Format(numberDes, mFourthNumber));

            mFirstDiscountText.SafeSetText(string.Format(discountDes, mFirstDiscount / 10f));
            mSecondDiscountText.SafeSetText(string.Format(discountDes, mSecondDiscount / 10f));
            mThirdDiscountText.SafeSetText(string.Format(discountDes, mThirdDiscount / 10f));

            InitItems();

            ActivityDataManager.GetInstance().OnSendGASWholeBargainReq();
        }

        public void UpdateData(LimitTimeGiftPackModel model)
        {
            
        }

        private void OnCheckViewBtnClick()
        {
            ClientSystemManager.GetInstance().OpenFrame<LimitTimeGroupBuyWelfareFrame>();
        }

        private void OnBuyBtnClick()
        {
            if (mOnItemClick != null)
            {
                mOnItemClick(0, 0, 0);
            }
        }

        private  void OnFashionPreviewBtnClick()
        {
            ItemData itemData = ItemDataManager.CreateItemDataFromTable(iFashionGiftId);
            if (itemData != null)
            {
                ItemTipManager.GetInstance().ShowTip(itemData);
            }
        }

        private void InitData()
        {
            for (int i = 0; i < ActivityDataManager.mDiscountDataList.Count; i++)
            {
                var data = ActivityDataManager.mDiscountDataList[i];

                if (i == (int)StageType.Frist)
                {
                    mFirstNumber = data.joinNum;
                    mFirstDiscount = data.discount;
                }
                else if (i == (int)StageType.Second)
                {
                    mSecondNumber = data.joinNum;
                    mSecondDiscount = data.discount;
                }
                else if (i == (int)StageType.Third)
                {
                    mThirdNumber = data.joinNum;
                    mThirdDiscount = data.discount;
                }
                else if (i == (int)StageType.Fourth)
                {
                    mFourthNumber = data.joinNum;
                }
            }
        }

        private void InitItems()
        {
            if (mModel.DetailDatas != null && mModel.DetailDatas.Count > 0)
            {
                LimitTimeGiftPackDetailModel model = mModel.DetailDatas[0];

                if (model.mRewards != null && model.mRewards.Length > 0)
                {
                    if (mItemPriceTxt != null)
                    {
                        mItemPriceTxt.text = model.GiftPrice.ToString();
                    }

                    if (mItemNameTxt != null)
                    {
                        mItemNameTxt.text = model.Name;
                    }

                    ItemReward reward = model.mRewards[0];
                    var itemData = ItemDataManager.CreateItemDataFromTable((int)reward.id);
                    itemData.Count = (int)reward.num;
                    
                    if (mItemBackgroundImg != null)
                    {
                        ETCImageLoader.LoadSprite(ref mItemBackgroundImg, itemData.GetQualityInfo().Background);
                    }

                    if (mItemIconImg != null)
                    {
                        ETCImageLoader.LoadSprite(ref mItemIconImg, itemData.Icon);
                    }

                    if (mItemIconBtn != null)
                    {
                        mItemIconBtn.onClick.RemoveAllListeners();
                        mItemIconBtn.onClick.AddListener(()=> { ItemTipManager.GetInstance().ShowTip(itemData); });
                    }
                }
            }
        }

        private void UpdateItemCount(int count)
        {
            if (mBuyCountTxt != null)
            {
                mBuyCountTxt.text = count.ToString();
            }
        }

        private void RefreshFullServiceDes(float discount)
        {
            if (mDiscountText != null)
            {
                mDiscountText.text = string.Format(fullServiceDiscountDes, discount);
            }
        }

        private void UpdateProgress()
        {
            ////第一阶段
            //if ((currentNumber - mFirstNumber) > mFirstNumber)
            //{
            //    mFirstProgress.value = 1;
            //}
            //else
            //{
            //    mFirstProgress.value = currentNumber / mFirstNumber;
            //}

            ////第二阶段
            //if ((currentNumber - mFirstNumber) > mSecondNumber)
            //{
            //    mSecondProgress.value = 1;
            //}
            //else
            //{
            //    float value = currentNumber - mFirstNumber;
            //    mSecondProgress.value = value / mSecondNumber;
            //}

            ////第三阶段
            //if ((currentNumber - mFirstNumber - mSecondNumber) > mThirdNumber)
            //{
            //    mThirdProgress.value = 1;
            //}
            //else
            //{
            //    float value = currentNumber - mFirstNumber - mSecondNumber;
            //    mThirdProgress.value = value / mThirdNumber;
            //}

            ////第四阶段
            //if ((currentNumber - mFirstNumber - mSecondNumber - mThirdNumber) > mFourthNumber)
            //{
            //    mFourthProgress.value = 1;
            //}
            //else
            //{
            //    float value = currentNumber - mFirstNumber - mSecondNumber - mThirdNumber;
            //    mFourthProgress.value = value / mFourthNumber;
            //}

            //特殊处理
            if (mFirstNumber == 0)
            {
                //第一阶段
                if ((currentNumber - mSecondNumber) > mSecondNumber)
                {
                    mFirstProgress.value = 1;
                }
                else
                {
                    mFirstProgress.value = currentNumber / mSecondNumber;
                }

                float differenceValue = mThirdNumber - mSecondNumber;
                //第二阶段
                if ((currentNumber - mSecondNumber) > differenceValue)
                {
                    mSecondProgress.value = 1;
                }
                else
                {
                    float value = currentNumber - mSecondNumber;
                    mSecondProgress.value = value / differenceValue;
                }
            }
            else
            {
                //不需要处理
            }
        }

        private void OnSyncWorldMallBuySucceed(UIEvent uiEvent)
        {
            ActivityDataManager.GetInstance().OnSendGASWholeBargainReq();
        }

        private void OnGASWholeBargainResSuccessed(UIEvent uiEvent)
        {
            if (uiEvent == null)
            {
                return;
            }

            if (uiEvent.Param1 == null)
            {
                return;
            }

            LimitTimeGroupBuyDataModel model = uiEvent.Param1 as LimitTimeGroupBuyDataModel;

            if (model == null)
            {
                return;
            }

            currentNumber = model.joinNum;
            currentTotalNumber = model.maxNum;

            UpdateProgress();
            UpdateItemCount(model.playerJoinNum);
            RefreshFullServiceDes(model.discount / 10.0f);
        }
        

        public void Close()
        {
            Dispose();
            Destroy(gameObject);
        }

        public void Dispose()
        {
            mOnItemClick = null;
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnGASWholeBargainResSuccessed, OnGASWholeBargainResSuccessed);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnSyncWorldMallBuySucceed, OnSyncWorldMallBuySucceed);
        }
    }
}