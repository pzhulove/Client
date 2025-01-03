using System;
using System.Collections.Generic;
using Protocol;
using UnityEngine;
using UnityEngine.UI;
using ProtoTable;

namespace GameClient
{
    public class ChristmasSnowmanItem2 : ActivityItemBase
    {
        [SerializeField]
        private Button mButton;
        [SerializeField]
        private Button mTipButton;
        [SerializeField]
        private Image mFinishImage;
        [SerializeField]
        private Image mUnFinishImag;
        [SerializeField]
        private Image mOverImag;
        [SerializeField]
        ItemData mItemData;
        int mPetId = 0;
        public override sealed void UpdateData(ILimitTimeActivityTaskDataModel data)
        {
            switch (data.State)
            {
                case OpActTaskState.OATS_INIT:
                case OpActTaskState.OATS_UNFINISH:
                    mUnFinishImag.CustomActive(true);
                    mFinishImage.CustomActive(false);
                    mOverImag.CustomActive(false);
                    break;
                case OpActTaskState.OATS_FINISHED:
                    mUnFinishImag.CustomActive(false);
                    mFinishImage.CustomActive(true);
                    mOverImag.CustomActive(false);
                    break;
                case OpActTaskState.OATS_FAILED:
                    break;
                case OpActTaskState.OATS_SUBMITTING:
                    break;
                case OpActTaskState.OATS_OVER:
                    mUnFinishImag.CustomActive(false);
                    mFinishImage.CustomActive(false);
                    mOverImag.CustomActive(true);
                    break;
                default:
                    break;
            }

            PetBtnOnClick(data);
        }

        protected override sealed void OnInit(ILimitTimeActivityTaskDataModel data)
        {
            if (data.AwardDataList.Count > 0)
            {
                int itemId = (int)data.AwardDataList[0].id;
                int num = (int)data.AwardDataList[0].num;
                mItemData = ItemDataManager.GetInstance().GetCommonItemTableDataByID(itemId);
            }

            PetBtnOnClick(data);

            if (mTipButton != null)
            {
                mTipButton.onClick.RemoveAllListeners();
                mTipButton.onClick.AddListener(() => { ItemTipManager.GetInstance().ShowTip(mItemData); });
            }

            GetPetID();
        }

        public override sealed void Dispose()
        {
            base.Dispose();
            if (mButton != null)
            {
                mButton.onClick.RemoveAllListeners();
            }

            if (mTipButton != null)
            {
                mTipButton.onClick.RemoveAllListeners();
            }
            mItemData = null;
        }
        
        void GetPetID()
        {
            var mPetDic = TableManager.GetInstance().GetTable<PetTable>().GetEnumerator();
            while (mPetDic.MoveNext())
            {
                var mPetTable = mPetDic.Current.Value as PetTable;
                if (mPetTable.PetEggID != mItemData.TableID)
                {
                    continue;
                }

                mPetId = mPetTable.ID;
                return;
            }
        }

        void PetBtnOnClick(ILimitTimeActivityTaskDataModel data)
        {
            if (mButton != null)
            {
                mButton.onClick.RemoveAllListeners();
                mButton.onClick.AddListener(() =>
                {
                    switch (data.State)
                    {
                        case OpActTaskState.OATS_INIT:
                        case OpActTaskState.OATS_UNFINISH:
                        case OpActTaskState.OATS_OVER:
                            OpenShowPetModelFrame();
                            break;
                        case OpActTaskState.OATS_FINISHED:
                            _OnItemClick();
                            break;
                        case OpActTaskState.OATS_FAILED:
                            break;
                        case OpActTaskState.OATS_SUBMITTING:
                            break;

                    }

                });
            }
        }

        void OpenShowPetModelFrame()
        {
            ClientSystemManager.GetInstance().OpenFrame<ShowPetModelFrame>(FrameLayer.Middle, mPetId);
        }
    }
}
