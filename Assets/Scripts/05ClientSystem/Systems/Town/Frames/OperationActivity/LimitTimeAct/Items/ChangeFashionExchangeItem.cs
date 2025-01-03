using System.Collections.Generic;
using Protocol;
using UnityEngine;
using UnityEngine.UI;
using ProtoTable;

namespace GameClient
{
    public class ChangeFashionExchangeItem : ActivityItemBase
    {
        [SerializeField] private Text mTextExchangeCount;
        [SerializeField] private Image mImageCoinIcon;
        [SerializeField] private Text mTextCoinCount;
        [SerializeField] private Text mTextCoinOwnNum;
        [SerializeField] private GameObject mCoinRoot;
        [SerializeField] private Button mButtonExchange;
        [SerializeField] private Button mButtonExchangeBlue;
        [SerializeField] private RectTransform mRewardItemRoot;
        [SerializeField] private ScrollRect mAwardsScrollRect;
        [SerializeField] GameObject mHasTakenReward;
        [SerializeField] GameObject mNotFinishGO;
        [SerializeField] GameObject mFinishedGO;
        [SerializeField] private Button mLookUpFashionBtn;

        [SerializeField] Vector2 mComItemSize = new Vector2(100f, 100f);
        [SerializeField] private int mScrollCount = 5;//超过多少时才能滑动

        private List<ComItem> mComItems = new List<ComItem>();

        /// <summary>
        /// 数据更新
        /// </summary>
        /// <param name="data">新的数据</param>
        public override void UpdateData(ILimitTimeActivityTaskDataModel data)
        {
            switch (data.State)
            {
                case OpActTaskState.OATS_OVER:
                    mNotFinishGO.CustomActive(false);
                    mHasTakenReward.CustomActive(true);
                    mFinishedGO.CustomActive(false);
                    break;
                case OpActTaskState.OATS_FINISHED:
                    mFinishedGO.CustomActive(true);
                    mNotFinishGO.CustomActive(false);
                    mHasTakenReward.CustomActive(false);
                    break;
                case OpActTaskState.OATS_UNFINISH:
                    mFinishedGO.CustomActive(false);
                    mNotFinishGO.CustomActive(true);
                    mHasTakenReward.CustomActive(false);
                    break;
            }

            if (data.State == OpActTaskState.OATS_OVER)
            {
                mTextExchangeCount.SafeSetText(string.Format(TR.Value("activity_coin_exchange_item_exchange_count"), 0, data.TotalNum));
            }
            else
            {
                mTextExchangeCount.SafeSetText(string.Format(TR.Value("activity_coin_exchange_item_exchange_count"), data.DoneNum, data.TotalNum));
            }

            //int coinNum = CountDataManager.GetInstance().GetCount(string.Format("{0}{1}", mActivityId, CounterKeys.COUNTER_ACTIVITY_FATIGUE_COIN_NUM));
            //var paramNumList = data.ParamNums;
            //if (paramNumList.Count == 0)
            //{
            //    return;
            //}
            //mTextCoinCount.SafeSetText(string.Format("/{0}", paramNumList[0]));
            //mTextCoinOwnNum.SafeSetText(coinNum.ToString());
            //if (coinNum < paramNumList[0])
            //{
            //    mTextCoinOwnNum.color = Color.red;
            //}
            //else
            //{
            //    mTextCoinOwnNum.color = Color.green;
            //}

            int itemID = -1;
            int count = -1;
            int coinNum = -1;
            if (data.ParamNums.Count > 1)
            {
                //是普通道具
                itemID = (int)data.ParamNums[0];
                count = (int)data.ParamNums[1];
                

                coinNum = ItemDataManager.GetInstance().GetOwnedItemCount(itemID);
                
            }
            else if(data.CountParamNums.Count != 0)
            {
                //不是道具，是走count的积分
                itemID = (int)data.CountParamNums[0].currencyId;
                count = (int)data.CountParamNums[0].value;

                coinNum = CountDataManager.GetInstance().GetCount(data.CountParamNums[0].name);
            }
            var itemTableItem = TableManager.GetInstance().GetTableItem<ItemTable>(itemID);
            if (itemTableItem == null)
            {
                Logger.LogErrorFormat("扩展参数{0}在道具表中无法被找到");
                return;
            }
            ComItem comitem = mCoinRoot.GetComponentInChildren<ComItem>();
            if (comitem == null)
            {
                var comItem = ComItemManager.Create(mCoinRoot);//可以这样写吗需要确认
                comitem = comItem;
            }
            ItemData ItemDetailData = ItemDataManager.CreateItemDataFromTable(itemID);
            if (null == ItemDetailData)
            {
                return;
            }
            ItemDetailData.Count = 1;
            comitem.Setup(ItemDetailData, (GameObject Obj, ItemData sitem) => { _OnShowTips(ItemDetailData); });

            mTextCoinCount.SafeSetText(string.Format("/{0}", count));
            mTextCoinOwnNum.SafeSetText(coinNum.ToString());
            if (coinNum < count)
            {
                mTextCoinOwnNum.color = Color.red;
            }
            else
            {
                mTextCoinOwnNum.color = Color.green;
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            if (mComItems != null)
            {
                for (int i = this.mComItems.Count - 1; i >= 0; --i)
                {
                    ComItemManager.Destroy(mComItems[i]);
                }
                mComItems.Clear();
            }
            mButtonExchange.SafeRemoveOnClickListener(_OnItemClick);
            mButtonExchangeBlue.SafeRemoveOnClickListener(_OnItemClick);
            mLookUpFashionBtn.SafeRemoveAllListener();
        }

        protected override void OnInit(ILimitTimeActivityTaskDataModel data)
        {
            if (data != null && data.AwardDataList != null)
            {
                var tempItemTableData = TableManager.GetInstance().GetTableItem<ItemTable>((int)data.AwardDataList[0].id);
                if(tempItemTableData.SubType != ItemTable.eSubType.GiftPackage)
                {
                    mLookUpFashionBtn.CustomActive(false);
                }
                else
                {
                    int rewardFirstId = (int)data.AwardDataList[0].id;
                    mLookUpFashionBtn.CustomActive(true);
                    mLookUpFashionBtn.SafeRemoveAllListener();
                    mLookUpFashionBtn.SafeAddOnClickListener(() =>
                    {
                        _ShowAvartar(rewardFirstId);
                    });
                }
                for (int i = 0; i < data.AwardDataList.Count; ++i)
                {
                    var comItem = ComItemManager.Create(mRewardItemRoot.gameObject);
                    if (comItem != null)
                    {
                        ItemData item = ItemDataManager.CreateItemDataFromTable((int)data.AwardDataList[i].id);
                        item.Count = (int)data.AwardDataList[i].num;
                        comItem.Setup(item, Utility.OnItemClicked);
                        mComItems.Add(comItem);
                        (comItem.transform as RectTransform).sizeDelta = mComItemSize;
                    }
                }
                mAwardsScrollRect.enabled = data.AwardDataList.Count > mScrollCount;
            }
            mButtonExchange.SafeAddOnClickListener(_OnItemClick);
            mButtonExchangeBlue.SafeAddOnClickListener(_OnItemClick);
        }
        void _OnShowTips(ItemData result)
        {
            if (result == null)
            {
                return;
            }
            ItemTipManager.GetInstance().ShowTip(result);
        }

        void _ShowAvartar(int id)
        {
            if (ClientSystemManager.GetInstance().IsFrameOpen<PlayerTryOnFrame>())
            {
                var tryOnFrame = ClientSystemManager.GetInstance().GetFrame(typeof(PlayerTryOnFrame)) as PlayerTryOnFrame;
                if (tryOnFrame != null)
                {
                    tryOnFrame.Reset(id);
                }
            }
            else
            {
                ClientSystemManager.GetInstance().OpenFrame<PlayerTryOnFrame>(FrameLayer.Middle, id);
            }
        }
    }
}
