using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProtoTable;

namespace GameClient
{
    public class HalloweenOnlineItem : ActivityItemBase
    {
        [SerializeField] private Button mReceiveBtn;
        [SerializeField] private Button mPetPreviewBtn;
        [SerializeField] private GameObject mHasTakenGo;
        [SerializeField] private GameObject mNotFinishedGo;
        [SerializeField] private GameObject mContent;
        [SerializeField] private Text mTextDescription;
        [SerializeField] private Text mTextProgress;
        [SerializeField] Vector2 mComItemSize = new Vector2(100f, 100f);
        [SerializeField] private ScrollRect mAwardsScrollRect;
        [SerializeField] private int mScrollCount = 5;//超过多少时才能滑动

        private List<ComItem> mComItems = new List<ComItem>();
        private ILimitTimeActivityTaskDataModel mData;
        public sealed override void UpdateData(ILimitTimeActivityTaskDataModel data)
        {
            switch (data.State)
            {
                case Protocol.OpActTaskState.OATS_INIT:
                case Protocol.OpActTaskState.OATS_UNFINISH:
                    {
                        mNotFinishedGo.CustomActive(true);
                        mHasTakenGo.CustomActive(false);
                        mReceiveBtn.CustomActive(false);
                    }
                    break;
                case Protocol.OpActTaskState.OATS_FINISHED:
                    {
                        mNotFinishedGo.CustomActive(false);
                        mHasTakenGo.CustomActive(false);
                        mReceiveBtn.CustomActive(true);
                    }
                    break;
                case Protocol.OpActTaskState.OATS_FAILED:
                    break;
                case Protocol.OpActTaskState.OATS_SUBMITTING:
                    break;
                case Protocol.OpActTaskState.OATS_OVER:
                    {
                        mNotFinishedGo.CustomActive(false);
                        mHasTakenGo.CustomActive(true);
                        mReceiveBtn.CustomActive(false);
                    }
                    break;
                default:
                    break;
            }

            mTextDescription.SafeSetText(data.Desc);
            mTextProgress.SafeSetText(string.Format("{0}/{1}", data.DoneNum, data.TotalNum));
        }

        protected sealed override void OnInit(ILimitTimeActivityTaskDataModel data)
        {
            mData = data;
            mPetPreviewBtn.CustomActive(false);
            if (data != null && data.AwardDataList != null)
            {
                for (int i = 0; i < data.AwardDataList.Count; ++i)
                {
                    var comItem = ComItemManager.Create(mContent.gameObject);
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

                if (data.AwardDataList.Count > 0)
                {
                    ItemData itemData = ItemDataManager.CreateItemDataFromTable((int)data.AwardDataList[0].id);
                    if (itemData != null)
                    {
                        mPetPreviewBtn.CustomActive(itemData.SubType == (int)ItemTable.eSubType.PetEgg);
                    }
                }
            }

            if (mReceiveBtn != null)
            {
                mReceiveBtn.onClick.RemoveAllListeners();
                mReceiveBtn.onClick.AddListener(_OnItemClick);
            }

            if (mPetPreviewBtn != null)
            {
                mPetPreviewBtn.onClick.RemoveAllListeners();
                mPetPreviewBtn.onClick.AddListener(OnPetPreviewBtnClick);
            }
        }

        public sealed override void Dispose()
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
        }

        private void OnPetPreviewBtnClick()
        {
            if (mData.AwardDataList.Count > 0)
            {
                ItemData itemData = ItemDataManager.CreateItemDataFromTable((int)mData.AwardDataList[0].id);
                if (itemData != null)
                {
                    PreViewItemData preViewItemData = new PreViewItemData();
                    preViewItemData.itemId = itemData.TableID;

                    PreViewDataModel preViewData = new PreViewDataModel();
                    preViewData.preViewItemList.Add(preViewItemData);

                    ClientSystemManager.GetInstance().OpenFrame<PreviewModelFrame>(FrameLayer.Middle, preViewData);
                }
            }
        }
    }
}