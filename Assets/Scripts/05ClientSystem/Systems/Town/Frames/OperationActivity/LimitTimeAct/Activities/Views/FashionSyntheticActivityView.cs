using System;
using System.Collections.Generic;
using Protocol;
using ProtoTable;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;

namespace GameClient
{
    public sealed class FashionSyntheticActivityView : MonoBehaviour, IDisposable, IGiftPackActivityView
    {
        [SerializeField] private Text mActivityTimer;
        [SerializeField] private ComUIListScript mItemUIListScript;
        [SerializeField] private Button mBtnGo;

        private ActivityItemBase.OnActivityItemClick<int> mOnItemClick;
        private LimitTimeGiftPackModel mModel;
        private void Awake()
        {
            InitUIListScription();
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnSyncWorldMallBuySucceed, OnSyncWorldMallBuySucceed);
        }

        private void OnDestroy()
        {
            UnInitUIListScription();
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnSyncWorldMallBuySucceed, OnSyncWorldMallBuySucceed);
        }

        #region UIListScription

        private void InitUIListScription()
        {
            if (mItemUIListScript != null)
            {
                mItemUIListScript.Initialize();
                mItemUIListScript.onBindItem += OnBindItemDelegate;
                mItemUIListScript.onItemVisiable += OnItemVisiableDelegate;
            }
        }

        private void UnInitUIListScription()
        {
            if (mItemUIListScript != null)
            {
                mItemUIListScript.onBindItem -= OnBindItemDelegate;
                mItemUIListScript.onItemVisiable -= OnItemVisiableDelegate;
            }
        }

        private FashionSyntheticActivityItem OnBindItemDelegate(GameObject itemObject)
        {
            return itemObject.GetComponent<FashionSyntheticActivityItem>();
        }

        private void OnItemVisiableDelegate(ComUIListElementScript item)
        {
            var fashionSyntheticItem = item.gameObjectBindScript as FashionSyntheticActivityItem;
            if (fashionSyntheticItem != null && item.m_index >= 0 && item.m_index < mModel.DetailDatas.Count)
            {
                fashionSyntheticItem.OnItemVisiable(mModel.DetailDatas[item.m_index], item.m_index, mOnItemClick);
            }

        }

        private void OnSetElementAmount(LimitTimeGiftPackModel model)
        {
            if (mItemUIListScript != null)
            {
                mItemUIListScript.SetElementAmount(model.DetailDatas.Count);
            }
        }

        #endregion

        public void Init(LimitTimeGiftPackModel model, ActivityItemBase.OnActivityItemClick<int> onItemClick)
        {
            if (model.Id == 0)
            {
                Logger.LogError("LimitTimeActivityModel data is empty");
                return;
            }

            mModel = model;
            mOnItemClick = onItemClick;

            mActivityTimer.SafeSetText(string.Format("{0}~{1}", Function._TransTimeStampToStr(mModel.StartTime), Function._TransTimeStampToStr(model.EndTime)));

            mBtnGo.SafeRemoveAllListener();
            mBtnGo.SafeAddOnClickListener(OnGoBtnClick);

            OnSetElementAmount(mModel);
        }

        private void OnGoBtnClick()
        {
            FashionMergeNewFrame.OpenLinkFrame(string.Format("1|0|{0}|{1}|{2}", (int)FashionMergeManager.GetInstance().FashionType, (int)FashionMergeManager.GetInstance().FashionPart, 0));

            if (ClientSystemManager.GetInstance().IsFrameOpen<LimitTimeActivityFrame>())
            {
                ClientSystemManager.GetInstance().CloseFrame<LimitTimeActivityFrame>();
            }
        }
        
        public void UpdateData(LimitTimeGiftPackModel model)
        {
            mModel = model;

            OnSetElementAmount(mModel);
        }

        private void OnSyncWorldMallBuySucceed(UIEvent uiEvent)
        {
            if (uiEvent == null)
                return;

            if (uiEvent.Param1 == null || uiEvent.Param2 == null)
                return;

            var itemId = (UInt32)uiEvent.Param1;

            bool isFind = false;
            for (int i = 0; i < mModel.DetailDatas.Count; i++)
            {
                if (itemId != mModel.DetailDatas[i].Id)
                {
                    continue;
                }

                isFind = true;
                break;
            }
            
            if (isFind)
            {
                OnSetElementAmount(mModel);
            }
        }

        public void Dispose()
        {
            mOnItemClick = null;
        }

        public void Close()
        {
            Dispose();
            Destroy(gameObject);
        }
    }
}
