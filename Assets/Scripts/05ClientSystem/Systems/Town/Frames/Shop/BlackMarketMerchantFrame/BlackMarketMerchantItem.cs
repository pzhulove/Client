using Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    /// <summary>
    /// 申请交易数据
    /// </summary>
    public class ApplyTradData
    {
        public BlackMarketAuctionInfo mInfo;//回购道具信息
        public BlackMarketType mMerchantType;//商人类型
    }

    public delegate void OnApplyTradDelegate(ApplyTradData data);
    public delegate void OnCancelApplyDelegate(BlackMarketAuctionInfo info);

    class BlackMarketMerchantItem : MonoBehaviour
    {
        [HeaderAttribute("状态控制器")]
        [SerializeField]
        private StateController mStateContrl;

        [HeaderAttribute("背包里无收购道具")]
        [SerializeField]
        private string mBackpackNoacQuisitionProps;

        [Space(2)]
        [HeaderAttribute("可申请交易")]
        [SerializeField]
        private string mCanApplyFor;

        [Space(2)]
        [HeaderAttribute("取消申请")]
        [SerializeField]
        private string mCancelApplyFor;

        [Space(2)]
        [HeaderAttribute("固定价格取消申请")]
        [SerializeField]
        private string mFixedPriceCancelApplyFor;

        [Space(2)]
        [HeaderAttribute("已申请")]
        [SerializeField]
        private string mHaveApplied;

        [Space(2)]
        [HeaderAttribute("已交易")]
        [SerializeField]
        private string mHaveBeenTrading;

        [Space(5)]
        [SerializeField]
        private Text mName;
        [SerializeField]
        private GameObject mItemParent;
        [SerializeField]
        private Image mTickIcon;
        [SerializeField]
        private Text mPrice;
        [SerializeField]
        private Image mQuotedPriceIcon;//报价Icon
        [SerializeField]
        private Text mQuotedPrice;
        [SerializeField]
        private Button mApplyTradBtn;
        [SerializeField]
        private Button mCancelApplyBtn;

        [HeaderAttribute("已申请的名字")]
        [SerializeField]
        private Text mHaveAppliedName;
        [HeaderAttribute("已交易的名字")]
        [SerializeField]
        private Text mHaveBeenTradName;

        BlackMarketAuctionInfo mItemInfo;
        BlackMarketType mBlackMarketMerchantType;
        private OnApplyTradDelegate mOnApplyTradDelegate;
        private OnCancelApplyDelegate mOnCancelApplyDelegate;

        ComItem mComItem = null;
        void Awake()
        {
            mApplyTradBtn.onClick.RemoveAllListeners();
            mApplyTradBtn.onClick.AddListener(OnApplyTradClick);

            mCancelApplyBtn.onClick.RemoveAllListeners();
            mCancelApplyBtn.onClick.AddListener(OnCancelApplyClick);
        }

        public void UpdateBlackMarketMerchantItem(BlackMarketAuctionInfo mItemInfo, BlackMarketType type, OnApplyTradDelegate mOnApplyTradDelegate, OnCancelApplyDelegate mOnCancelApplyDelegate)
        {
            this.mItemInfo = mItemInfo;
            mBlackMarketMerchantType = type;
            this.mOnApplyTradDelegate = mOnApplyTradDelegate;
            this.mOnCancelApplyDelegate = mOnCancelApplyDelegate;
            InitItemInfo();
            UpdateState();
        }

        void InitItemInfo()
        {
            if (mItemInfo == null)
            {
                Logger.LogErrorFormat("黑市商人回购道具信息[BlackMarketBackBuyItemInfo] 为空");
                return;
            }

            ItemData mItemData = ItemDataManager.GetInstance().GetCommonItemTableDataByID((int)mItemInfo.back_buy_item_id);
            if (mItemData == null)
            {
                Logger.LogErrorFormat("脚本[BlackMarketMerchantItem] 中 [InitItemInfo]函数创建道具Itemdata为空");
            }

            if (mComItem == null)
            {
                mComItem = ComItemManager.Create(mItemParent);
            }

            mComItem.Setup(mItemData, Utility.OnItemClicked);

            mName.text = mItemData.GetColorName();

            if (mItemInfo.back_buy_type == (byte)BlackMarketType.BmtFixedPrice)
            {
                mPrice.text = mItemInfo.price.ToString();
            }
            else
            {
                if (mItemInfo.recommend_price > 0)
                {
                    mPrice.text = BlackMarketMerchantDataManager.GetInstance().SwichQuestionMarkCharacter(mItemInfo.recommend_price.ToString().Length);
                }
                else
                {
                    mPrice.text = BlackMarketMerchantDataManager.GetInstance().SwichQuestionMarkCharacter(mItemInfo.price_upper_limit.ToString().Length);
                }
            }

            mQuotedPrice.text = mItemInfo.auction_price.ToString();

            mHaveAppliedName.text = mHaveBeenTradName.text = string.Format("({0})", mItemInfo.auctioner_name);
        }

        void UpdateState()
        {
            switch ((BlackMarketAuctionState)mItemInfo.state)
            {
                case BlackMarketAuctionState.BmaisInvalid:
                    break;
                case BlackMarketAuctionState.BmaisCanAuction:

                    bool isFind = BlackMarketMerchantDataManager.GetInstance().FindBackPackBuyBackItem((int)mItemInfo.back_buy_item_id);
                    if (isFind)
                    {
                        mStateContrl.Key = mCanApplyFor;
                    }
                    else
                    {
                        mStateContrl.Key = mBackpackNoacQuisitionProps;
                    }

                    break;
                case BlackMarketAuctionState.BmaisApplyed:

                    //如果竞拍人是自己
                    if (PlayerBaseData.GetInstance().RoleID == mItemInfo.auctioner_guid)
                    {
                        if (mItemInfo.back_buy_type == (byte)BlackMarketType.BmtFixedPrice)
                        {
                            mStateContrl.Key = mFixedPriceCancelApplyFor;
                        }
                        else
                        {
                            mStateContrl.Key = mCancelApplyFor;
                        }
                    }
                    else
                    {
                        mStateContrl.Key = mHaveApplied;
                    }

                    break;
                case BlackMarketAuctionState.BmaisTransed:

                    mStateContrl.Key = mHaveBeenTrading;

                    break;
                case BlackMarketAuctionState.BmaisMax:
                    break;
                default:
                    break;
            }
        }

        void OnApplyTradClick()
        {
            ApplyTradData data = new ApplyTradData();
            data.mInfo = mItemInfo;
            data.mMerchantType = mBlackMarketMerchantType;

            if (mOnApplyTradDelegate != null)
            {
                mOnApplyTradDelegate.Invoke(data);
            }
        }

        void OnCancelApplyClick()
        {
            if (mOnCancelApplyDelegate != null)
            {
                mOnCancelApplyDelegate.Invoke(mItemInfo);
            }
        }

        void OnDestroy()
        {
            mItemInfo = null;
            mBlackMarketMerchantType = BlackMarketType.BmtInvalid;
            mOnApplyTradDelegate = null;
            mOnCancelApplyDelegate = null;
        }
    }
}
