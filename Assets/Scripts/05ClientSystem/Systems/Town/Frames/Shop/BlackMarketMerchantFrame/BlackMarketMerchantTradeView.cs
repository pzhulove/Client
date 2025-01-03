using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using Protocol;
using UnityEngine.Events;
using System;
using ProtoTable;

namespace GameClient
{
    class BlackMarketMerchantTradeView : MonoBehaviour
    {
        [HeaderAttribute("状态控制器")]
        [SerializeField]
        private StateController mStateControl;

        [HeaderAttribute("固定价格状态")]
        [SerializeField]
        private string mBmtFixedPriceState;

        [HeaderAttribute("竞拍价格状态")]
        [SerializeField]
        private string mBmtAuctionPriceState;

        [SerializeField]
        private Button mCloseBtn;

        [SerializeField]
        private Text mPrice;

        [SerializeField]
        private Text mRecommendPrice;

        [SerializeField]
        private Text mInputPrice;

        [SerializeField]
        private Button mInputBtn;

        [SerializeField]
        private Button mConFirmBtn;

        [SerializeField]
        private ComUIListScript mComUIListScript;

        [SerializeField]
        private GameObject mGoRecommendedRoot;

        ApplyTradData mApplyTradData = null;
        OnItemSelect mOnItemSelect = null;
        OnSetPrice mOnSetPrice = null;
        UnityAction mOnConfirmClick;
        List<ItemData> mAllEquips = new List<ItemData>();
        private UInt32 iMinPrice = 0; //价格下限
        private UInt32 iMaxPrice = 0;//价格上限
        private UInt32 iInputNumber = 0; // 输入价格
        private bool ReenterPrice = true; //重新输入价格
        void Awake()
        {
            if (mInputBtn)
            {
                mInputBtn.onClick.RemoveAllListeners();
                mInputBtn.onClick.AddListener(() =>
                {
                    ClientSystemManager.GetInstance().OpenFrame<VirtualKeyboardFrame>(FrameLayer.Middle);
                    ReenterPrice = true;
                });
            }

            if (mConFirmBtn)
            {
                mConFirmBtn.onClick.RemoveAllListeners();
                mConFirmBtn.onClick.AddListener(() =>
                {
                    if (mOnConfirmClick != null)
                    {
                        mOnConfirmClick.Invoke();
                    }
                });
            }

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ChangeNum, OnChangeNumber);
        }
        
        void OnChangeNumber(UIEvent uiEvent)
        {
            var type = (ChangeNumType)uiEvent.Param1;
            if (type == ChangeNumType.EnSure)
            {
                iInputNumber = Clamp(iInputNumber, iMinPrice, iMaxPrice);
                UpdateInputPriceText(iInputNumber);

                if (mOnSetPrice != null)
                {
                    mOnSetPrice.Invoke(iInputNumber);
                }
            }
            else
            {
                UInt32 mCureentNumber = iInputNumber;
                if (type == ChangeNumType.Add)
                {
                    int addNum = (int)uiEvent.Param2;
                    mCureentNumber = AddInputNumber(mCureentNumber, (UInt32)addNum);
                }
                else if (type == ChangeNumType.BackSpace)
                {
                    mCureentNumber = BackSpaceInputNumber(mCureentNumber);
                }

                if (mCureentNumber <= 0 || mCureentNumber.ToString().Length > 9)
                {
                    //非法输入
                    return;
                }

                iInputNumber = Math.Min(mCureentNumber, iMaxPrice);

                UpdateInputPriceText(iInputNumber);
            }
        }

        UInt32 AddInputNumber(UInt32 currentNum, UInt32 addNum)
        {
            UInt32 currentUserInput = currentNum;

            if (addNum < 0 || addNum > 9)
            {
                Logger.LogErrorFormat("传入数字不合法，请控制在0-9之间");
                return currentUserInput;
            }

            //重新输入价格
            if (ReenterPrice)
            {
                if (addNum != 0)
                {
                    currentUserInput = addNum;
                    ReenterPrice = false;
                }
                else
                {
                    currentUserInput = 1;
                    ReenterPrice = true;
                }
            }
            else
            {
                currentUserInput = currentUserInput * 10 + (UInt32)addNum;
            }
            
            if (currentUserInput < 1)
            {
                Logger.LogErrorFormat("userInputPrice is error");
            }

            return currentUserInput;
        }

        UInt32 BackSpaceInputNumber(UInt32 currentNum)
        {
            if (currentNum < 10)
            {
                currentNum = 1;
            }
            else
            {
                UInt32 tempCurNum = currentNum / 10;
                currentNum = tempCurNum;
            }

            return currentNum;
        }

        UInt32 Clamp(UInt32 value, UInt32 min, UInt32 max)
        {
            return Math.Min(Math.Max(value, min), max);
        }

        public void InitView(ApplyTradData mData, OnItemSelect onItemSelect, OnSetPrice onSetPrice, UnityAction OnConFirmClick)
        {
            mApplyTradData = mData;
            mOnItemSelect = onItemSelect;
            mOnSetPrice = onSetPrice;
            mOnConfirmClick = OnConFirmClick;
            iMinPrice = mApplyTradData.mInfo.price_lower_limit;
            iMaxPrice = mApplyTradData.mInfo.price_upper_limit;

            InitInfo();
            UpdateStateShow();
            InitComUIListScript();
            InitBtnCloseClick();
        }

        void UpdateStateShow()
        {
            if (mApplyTradData.mInfo.back_buy_type == (uint)BlackMarketType.BmtFixedPrice)
            {
                mStateControl.Key = mBmtFixedPriceState;
            }
            else
            {
                mStateControl.Key = mBmtAuctionPriceState;
            }
        }

        void InitInfo()
        {
            //固定价格
            mPrice.text = mApplyTradData.mInfo.price.ToString();

            //推荐价格
            mRecommendPrice.text = mApplyTradData.mInfo.recommend_price.ToString();

            //推荐价格大于零显示
            mGoRecommendedRoot.CustomActive(mApplyTradData.mInfo.recommend_price > 0);
        }

        void InitComUIListScript()
        {
            if (mComUIListScript)
            {
                mComUIListScript.Initialize();
                mComUIListScript.onBindItem += OnBindItemDelegate;
                mComUIListScript.onItemVisiable += OnItemVisiableDelegate;
                mComUIListScript.onItemSelected += OnItemSelectedDelegate;
                mComUIListScript.onItemChageDisplay += OnItemChangeDisplayDelegate;

                LoadAllEquip();
            }
        }

        void UnInitComUIListScript()
        {
            if (mComUIListScript)
            {
                mComUIListScript.onBindItem -= OnBindItemDelegate;
                mComUIListScript.onItemVisiable -= OnItemVisiableDelegate;
                mComUIListScript.onItemSelected -= OnItemSelectedDelegate;
                mComUIListScript.onItemChageDisplay -= OnItemChangeDisplayDelegate;
                mComUIListScript = null;
            }
        }

        void LoadAllEquip()
        {
            mAllEquips = BlackMarketMerchantDataManager.GetInstance().GetBackPackAllItem((int)mApplyTradData.mInfo.back_buy_item_id);
            SetElementAmount(mAllEquips.Count);
        }

        void SetElementAmount(int Count)
        {
            if (mComUIListScript)
            {
                mComUIListScript.SetElementAmount(Count);
            }
        }

        BlackMarketMerchantTradeItem OnBindItemDelegate(GameObject itemObject)
        {
            return itemObject.GetComponent<BlackMarketMerchantTradeItem>();
        }

        void OnItemVisiableDelegate(ComUIListElementScript item)
        {
            var current = item.gameObjectBindScript as BlackMarketMerchantTradeItem;
            if (current != null)
            {
                if (item.m_index >= 0 && item.m_index < mAllEquips.Count)
                {
                    current.OnItemVisiable(mAllEquips[item.m_index]);
                }
            }
        }

        void OnItemSelectedDelegate(ComUIListElementScript item)
        {
            var current = item.gameObjectBindScript as BlackMarketMerchantTradeItem;
            if (current != null)
            {
                if (mOnItemSelect != null)
                {
                    mOnItemSelect.Invoke(current.GUID);
                }
            }
        }

        void OnItemChangeDisplayDelegate(ComUIListElementScript item, bool bSelected)
        {
            var current = item.gameObjectBindScript as BlackMarketMerchantTradeItem;
            if (current != null)
            {
                current.OnItemChangeDisplay(bSelected);
            }
        }
        
        //更新输入价格文本
        void UpdateInputPriceText(UInt32 price)
        {
            mInputPrice.text = price.ToString();
        }

        void InitBtnCloseClick()
        {
            mCloseBtn.onClick.RemoveAllListeners();
            mCloseBtn.onClick.AddListener(OnCloseClick);
        }

        void OnCloseClick()
        {
            ClientSystemManager.GetInstance().CloseFrame<BlackMarketMerchantTradeFrame>();
        }

        void OnDestroy()
        {
            mApplyTradData = null;
            mOnItemSelect = null;
            mOnSetPrice = null;
            mOnConfirmClick = null;
            mAllEquips.Clear();
            iMinPrice = 0; //价格下限
            iMaxPrice = 0;//价格上限
            iInputNumber = 0; // 输入价格
            UnInitComUIListScript();
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ChangeNum, OnChangeNumber);
        }
    }
}

