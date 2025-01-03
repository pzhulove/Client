using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;

namespace GameClient
{
    public class TopUpPushView : MonoBehaviour
    {
        [SerializeField]private SimpleTimer mSimpleTimer;
        [SerializeField]private Button mCloseBtn;
        [SerializeField]private ComUIListScript mItemUIListScript;
        [SerializeField]private GameObject mUpArrowGo;

        TopUpPushDataModel mTopUpPushDataModel;
        OnBuyClick mOnBuyClick;
        void Awake()
        {
            InitItemUIListScript();

            if (mCloseBtn)
            {
                mCloseBtn.onClick.RemoveAllListeners();
                mCloseBtn.onClick.AddListener(() =>
                {
                    ClientSystemManager.GetInstance().CloseFrame<TopUpPushFrame>();
                });
            }
        }

        void OnDestroy()
        {
            UnItemUIListScript();
            mTopUpPushDataModel = null;
            mOnBuyClick = null;
        }

        /// <summary>
        /// 初始化界面信息
        /// </summary>
        /// <param name="topUpPushDataModel">充值推送数据</param>
        /// <param name="callBack">购买事件</param>
        public void InitView(TopUpPushDataModel topUpPushDataModel,OnBuyClick callBack)
        {
            if (topUpPushDataModel == null)
            {
                return;
            }

            mTopUpPushDataModel = topUpPushDataModel;
            mOnBuyClick = callBack;

            SetSimpleTimer();
            SetElementAmount(mTopUpPushDataModel.mItems.Count);
            mUpArrowGo.CustomActive(mTopUpPushDataModel.mItems.Count > 3);
        }

        /// <summary>
        /// 设置时间
        /// </summary>
        private void SetSimpleTimer()
        {
            int timer = (int)(mTopUpPushDataModel.validTimesTamp - TimeManager.GetInstance().GetServerTime());

            if (mSimpleTimer)
            {
                mSimpleTimer.SetCountdown(timer);
                mSimpleTimer.StartTimer();
            }
        }

        private void SetElementAmount(int Count)
        {
            mItemUIListScript.SetElementAmount(Count);
        }

        /// <summary>
        /// 刷新道具列表
        /// </summary>
        /// <param name="topUpPushDataModel"></param>
        public void RefreshItems(TopUpPushDataModel topUpPushDataModel)
        {
            mTopUpPushDataModel = topUpPushDataModel;
            SetElementAmount(mTopUpPushDataModel.mItems.Count);
        }

        private void InitItemUIListScript()
        {
            mItemUIListScript.Initialize();
            mItemUIListScript.onBindItem += OnBindItemDelegate;
            mItemUIListScript.onItemVisiable += OnItemVisiableDelegate;
        }

        private void UnItemUIListScript()
        {
            if (mItemUIListScript)
            {
                mItemUIListScript.onBindItem -= OnBindItemDelegate;
                mItemUIListScript.onItemVisiable -= OnItemVisiableDelegate;
                mItemUIListScript = null;
            }
        }

        private TopUpPushItem OnBindItemDelegate(GameObject itemObject)
        {
            return itemObject.GetComponent<TopUpPushItem>();
        }

        private void OnItemVisiableDelegate(ComUIListElementScript item)
        {
            TopUpPushItem mTopUpPushItem = item.gameObjectBindScript as TopUpPushItem;

            if (mTopUpPushItem != null && item.m_index >= 0 && item.m_index < mTopUpPushDataModel.mItems.Count)
            {
                mTopUpPushItem.OnItemVisiable(mTopUpPushDataModel.mItems[item.m_index], mOnBuyClick);
            }
        }
    }
}

