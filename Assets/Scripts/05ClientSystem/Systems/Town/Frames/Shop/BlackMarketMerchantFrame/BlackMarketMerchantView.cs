using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using Protocol;
using ProtoTable;

namespace GameClient
{
    class BlackMarketMerchantView : MonoBehaviour
    {
        [SerializeField]
        private Text mPeopleDesc;

        //[SerializeField]
        //private Image mMerchantImage;

        [SerializeField]
        private Button mCloseBtn;

        [SerializeField]
        private ComUIListScript mComUIListScript;

        [SerializeField]
        private Text mTransactionTypeDesc;

        BlackMarketMerchantDataModel mDataModel = null;
        OnApplyTradDelegate mOnApplyTradDelegate = null;
        OnCancelApplyDelegate mOnCancelApplyDelegate = null;

        /// <summary>
        /// 初始化界面展示
        /// </summary>
        /// <param name="mModel">黑市商人数据</param>
        /// <param name="mOnApplyTradDelegate">申请交易回调</param>
        /// <param name="mOnCancelApplyDelegate">取消交易回调</param>
        public void InitView(BlackMarketMerchantDataModel mModel, OnApplyTradDelegate mOnApplyTradDelegate, OnCancelApplyDelegate mOnCancelApplyDelegate)
        {
            mDataModel = mModel;
            this.mOnApplyTradDelegate = mOnApplyTradDelegate;
            this.mOnCancelApplyDelegate = mOnCancelApplyDelegate;
            
            var mBlackMarketTable = TableManager.GetInstance().GetTableItem<BlackMarketTable>((int)mDataModel.mBlackMarketType);
            if (mBlackMarketTable == null)
            {
                Logger.LogErrorFormat("[BlackMarketTable] 黑市商人表中未找到ID id={0}", (int)mDataModel.mBlackMarketType);
            }
            
            UpdatePeopleDesc();
            //商人半身像
            //ETCImageLoader.LoadSprite(ref mMerchantImage, mBlackMarketTable.NpcPortrait);
            //mMerchantImage.SetNativeSize();

            InitComUIListScript();
            InitBtnCloseClick();
            SetElementAmount(mDataModel.mBlackMarketAuctionInfoList.Count);
        }

        public void  RefreshItemInfoList(BlackMarketMerchantDataModel mModel)
        {
            mDataModel = mModel;
            SetElementAmount(mDataModel.mBlackMarketAuctionInfoList.Count);
        }

        void InitComUIListScript()
        {
            if (mComUIListScript)
            {
                mComUIListScript.Initialize();
                mComUIListScript.onBindItem += OnBindItemDelegate;
                mComUIListScript.onItemVisiable += OnItemVisiableDelegate;
            }
        }

        void UnInitComUIListScript()
        {
            if (mComUIListScript)
            {
                mComUIListScript.onBindItem -= OnBindItemDelegate;
                mComUIListScript.onItemVisiable -= OnItemVisiableDelegate;
                mComUIListScript = null;
            }
        }

        void SetElementAmount(int Count)
        {
            if (mComUIListScript)
                mComUIListScript.SetElementAmount(Count);
        }

        BlackMarketMerchantItem OnBindItemDelegate(GameObject itemObject)
        {
            return itemObject.GetComponent<BlackMarketMerchantItem>();
        }

        void OnItemVisiableDelegate(ComUIListElementScript item)
        {
            var mItem = item.gameObjectBindScript as BlackMarketMerchantItem;
            if (mItem != null && item.m_index >= 0 && item.m_index < mDataModel.mBlackMarketAuctionInfoList.Count)
            {
                BlackMarketAuctionInfo mItemInfo = mDataModel.mBlackMarketAuctionInfoList[item.m_index];
                mItem.UpdateBlackMarketMerchantItem(mItemInfo, mDataModel.mBlackMarketType, mOnApplyTradDelegate,mOnCancelApplyDelegate);
            }
        }

        public void UpdatePeopleDesc()
        {
            OpActivityData mOpActivityData = ActivityDataManager.GetInstance()._GetBlackMarketMerchantOpActivityData();
            if (mOpActivityData != null)
            {
                var blackMarketTable = TableManager.GetInstance().GetTableItem<BlackMarketTable>((int)mDataModel.mBlackMarketType);
                if (blackMarketTable != null)
                {
                    var npcTable = TableManager.GetInstance().GetTableItem<NpcTable>(blackMarketTable.NpcID);
                    if (npcTable != null)
                    {
                        mPeopleDesc.text = string.Format("{0}:\n{1}", npcTable.NpcName, mOpActivityData.ruleDesc);
                    }

                    //收购类型描述
                    mTransactionTypeDesc.text = blackMarketTable.TransactionTypeDescribe;
                }

                
            }
        }

        void InitBtnCloseClick()
        {
            mCloseBtn.onClick.RemoveAllListeners();
            mCloseBtn.onClick.AddListener(OnCloseClick);
        }

        void OnCloseClick()
        {
            ClientSystemManager.GetInstance().CloseFrame<BlackMarketMerchantFrame>();
        }

        void OnDestroy()
        {
            mDataModel = null;
            mOnApplyTradDelegate = null;
            mOnCancelApplyDelegate = null;
            UnInitComUIListScript();
        }
    }
}

