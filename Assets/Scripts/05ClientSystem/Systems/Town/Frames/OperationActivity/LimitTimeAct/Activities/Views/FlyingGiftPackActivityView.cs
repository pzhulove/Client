using System;
using System.Collections.Generic;
using Protocol;
using ProtoTable;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameClient
{
    public sealed class FlyingGiftPackActivityView : MonoBehaviour, IDisposable, IGiftPackActivityView
    {
        [SerializeField] protected ActivityNote mNote;
        [SerializeField] protected List<GameObject> mItems1;
        [SerializeField] protected List<GameObject> mItems2;
        [SerializeField] protected Button mMallBuyBtn1;
        [SerializeField] protected Button mMallBuyBtn2;
        [SerializeField] protected UIGray mMallBuyGray1;
        [SerializeField] protected UIGray mMallBuyGray2;
        [SerializeField] protected List<int> mGiftItems1;
        [SerializeField] protected List<int> mGiftItems2;

        private ActivityItemBase.OnActivityItemClick<int> mOnItemClick;
        private LimitTimeGiftPackModel mModel;
        private int gift1AccountRestBuyNum = 0;//礼包1账号剩余购买次数
        private int gift2AccountRestBuyNum = 0;//礼包2账号剩余购买次数
        private void Awake()
        {
            if (mMallBuyBtn1 != null)
            {
                mMallBuyBtn1.onClick.RemoveAllListeners();
                mMallBuyBtn1.onClick.AddListener(() => 
                {
                    if (mOnItemClick != null)
                    {
                        mOnItemClick(0,0,0);
                    }
                });
            }

            if (mMallBuyBtn2 != null)
            {
                mMallBuyBtn2.onClick.RemoveAllListeners();
                mMallBuyBtn2.onClick.AddListener(() =>
                {
                    if (mOnItemClick != null)
                    {
                        mOnItemClick(1, 0, 0);
                    }
                });
            }

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnSyncWorldMallBuySucceed, OnSyncWorldMallBuySucceed);
        }

        private void OnDestroy()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnSyncWorldMallBuySucceed, OnSyncWorldMallBuySucceed);
        }

        public void Init(LimitTimeGiftPackModel model, ActivityItemBase.OnActivityItemClick<int> onItemClick)
        {
            if (model.Id == 0)
            {
                Logger.LogError("LimitTimeActivityModel data is empty");
                return;
            }

            mModel = model;
            mOnItemClick = onItemClick;
            mNote.Init(model,false,GetComponent<ComCommonBind>());
            InitItems();
        }

        private void UpdateBuyBtnState()
        {
            if (mMallBuyBtn1 != null)
            {
                mMallBuyBtn1.enabled = gift1AccountRestBuyNum > 0;
            }
            
            if (mMallBuyGray1 != null)
            {
                mMallBuyGray1.enabled = !(gift1AccountRestBuyNum > 0);
            }

            if (mMallBuyBtn2 != null)
            {
                mMallBuyBtn2.enabled = gift2AccountRestBuyNum > 0;
            }

            if (mMallBuyGray2 != null)
            {
                mMallBuyGray2.enabled = !(gift2AccountRestBuyNum > 0);
            }
        }

        private void InitItems()
        {
            for (int i = 0; i < mGiftItems1.Count; i++)
            {
                mItems1[i].CustomActive(true);
                ItemData itemData = ItemDataManager.CreateItemDataFromTable(mGiftItems1[i]);
                if (itemData == null)
                {
                    continue;
                }

                ComItem comItem = ComItemManager.Create(mItems1[i]);
                comItem.Setup(itemData, Utility.OnItemClicked);
            }

            for (int i = 0; i < mGiftItems2.Count; i++)
            {
                mItems2[i].CustomActive(true);
                ItemData itemData = ItemDataManager.CreateItemDataFromTable(mGiftItems2[i]);
                if (itemData == null)
                {
                    continue;
                }

                ComItem comItem = ComItemManager.Create(mItems2[i]);
                comItem.Setup(itemData, Utility.OnItemClicked);
            }
        }

        private void UpdateItemAccountRestBuyNum()
        {
            if (mModel.DetailDatas.Count >= 2)
            {
                gift1AccountRestBuyNum = (int)mModel.DetailDatas[0].AccountRestBuyNum;
                gift2AccountRestBuyNum = (int)mModel.DetailDatas[1].AccountRestBuyNum;
            }
        }

        public void UpdateData(LimitTimeGiftPackModel model)
        {
            mModel = model;

            UpdateItemAccountRestBuyNum();
            UpdateBuyBtnState();
        }

        private void OnSyncWorldMallBuySucceed(UIEvent uiEvent)
        {
            if (uiEvent == null)
                return;

            if (uiEvent.Param1 == null || uiEvent.Param2 == null)
                return;

            var itemId = (UInt32)uiEvent.Param1;
            //var leftLimitNumber = (int)uiEvent.Param2;

            int index = -1;
            for (int i = 0; i < mModel.DetailDatas.Count; i++)
            {
                if (itemId != mModel.DetailDatas[i].Id)
                {
                    continue;
                }

                index = i;
                break;
            }

            if (index == 0)
            {
                gift1AccountRestBuyNum = (int)uiEvent.Param3;
            }
            else if (index == 1)
            {
                gift2AccountRestBuyNum = (int)uiEvent.Param3;
            }
            
            UpdateBuyBtnState();
        }

        public void Dispose()
        {
            if (mNote != null)
                mNote.Dispose();
        }

        public void Close()
        {
            Dispose();
            Destroy(gameObject);
        }
    }
}
