using ProtoTable;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class SpringFestivalDungeonView : LimitTimeActivityViewCommon
    {
        [SerializeField]
        private Text mTimeTxt;
        [SerializeField]
        private Text mRuleTxt;
        [SerializeField]
        private Button mExchangeShopBtn;
        [SerializeField]
        private Button mReviewBtn;
        [SerializeField]
        private Button mGoToBtn;
        [SerializeField]
        private Button mHelpBtn;
        [SerializeField]
        private Transform mItemsRoot;
        [SerializeField]
        private GameObject mTmpItem;
        [SerializeField]
        private Vector2 mComItemSize = new Vector2(100, 100);
        [SerializeField]
        private GameObject mSpecialTipGo;
        [SerializeField]
        private Button mSpecialTipBtnClick;

        private PreViewDataModel mPreViewData;
        public override void Init(ILimitTimeActivityModel model, ActivityItemBase.OnActivityItemClick<int> onItemClick)
        {
            _InitNode(model);
            _InitItems(model);
            _InitPreviewData(model);
            mExchangeShopBtn.SafeAddOnClickListener(_OnExchangeBtnClick);
            mReviewBtn.SafeAddOnClickListener(_OnRevieBtnClick);
            mGoToBtn.SafeAddOnClickListener(_OnGoToBtnClick);
            mHelpBtn.SafeAddOnClickListener(_OnHelpBtnClick);
            mSpecialTipBtnClick.SafeAddOnClickListener(_OnSpecialTipBtnCloseClick);
            mModel = model;
        }

      

        private void _InitPreviewData(ILimitTimeActivityModel model)
        {
            mPreViewData = new PreViewDataModel();
            mPreViewData.isCreatItem = false;
            mPreViewData.preViewItemList = new List<PreViewItemData>();
            for (int i = 0; i < model.ParamArray.Length; i++)
            {
                ItemData itemData = ItemDataManager.CreateItemDataFromTable((int)model.ParamArray[i]);
                if (itemData.Type == ItemTable.eType.FASHION||
                          (itemData.SubType == (int)ItemTable.eSubType.GiftPackage && itemData.ThirdType == ItemTable.eThirdType.FashionGift) ||
                          itemData.SubType == (int)ItemTable.eSubType.TITLE ||
                          itemData.SubType == (int)ItemTable.eSubType.PetEgg)
                {
                  
                    PreViewItemData preViewItem = new PreViewItemData();
                    preViewItem.activityId = (int)model.Id;
                    preViewItem.itemId = (int)model.ParamArray[i];
                    mPreViewData.preViewItemList.Add(preViewItem);
                }
            }
        }

        protected override void _InitItems(ILimitTimeActivityModel model)
        {
            for (int i = 0; i < model.ParamArray.Length; i++)
            {
                GameObject go = Instantiate(mTmpItem);
                go.transform.SetParent(mItemsRoot);
                go.transform.localScale = Vector3.one;

                SpringFestivalDungeonItem item = go.GetComponent<SpringFestivalDungeonItem>();
                if (item != null)
                {
                    item.Init((int)model.ParamArray[i], mComItemSize,i,_OnSpecialItemClick);
                }
            }
            if (mTmpItem)
            {
                Destroy(mTmpItem);
            }
        }

      

        public override void Dispose()
        {
            mExchangeShopBtn.SafeRemoveOnClickListener(_OnExchangeBtnClick);
            mReviewBtn.SafeRemoveOnClickListener(_OnRevieBtnClick);
            mGoToBtn.SafeRemoveOnClickListener(_OnGoToBtnClick);
            mHelpBtn.SafeRemoveOnClickListener(_OnHelpBtnClick);
            mSpecialTipBtnClick.SafeRemoveOnClickListener(_OnSpecialTipBtnCloseClick);
            mPreViewData = null;
        }

        private void _OnSpecialItemClick(GameObject obj, ItemData item)
        {
            mSpecialTipGo.CustomActive(true);
        }
        private void _OnSpecialTipBtnCloseClick()
        {
            mSpecialTipGo.CustomActive(false);
        }
        private void _OnGoToBtnClick()
        {
            if(mModel!=null)
            {
                if (ClientSystemManager.GetInstance().IsFrameOpen<LimitTimeActivityFrame>())
                {
                    ClientSystemManager.GetInstance().CloseFrame<LimitTimeActivityFrame>();
                }

                ChapterSelectFrame.SetSceneID((int)mModel.Param);
                ClientSystemManager.GetInstance().OpenFrame<ChapterSelectFrame>();
            }
          
        }

        private void _OnRevieBtnClick()
        {
            ClientSystemManager.GetInstance().OpenFrame<PreviewModelFrame>(FrameLayer.Middle, mPreViewData);
        }

        private void _OnExchangeBtnClick()
        {
            //  ClientSystemManager.instance.OpenFrame<MallNewFrame>(FrameLayer.Middle, new MallNewFrameParamData() { MallNewType = MallNewType.ExChangeMall });
            LimitTimeActivityFrame limitTimeActivityFrame = ClientSystemManager.GetInstance().GetFrame(typeof(LimitTimeActivityFrame)) as LimitTimeActivityFrame;
            if (limitTimeActivityFrame != null)
            {
                limitTimeActivityFrame.OpenFrameByActivityId(22880);
            }
        }


        private void _OnHelpBtnClick()
        {
            ClientSystemManager.GetInstance().OpenFrame<SpringFestivalDungeonTipFrame>(FrameLayer.Middle);
        }

        private void _InitNode(ILimitTimeActivityModel model)
        {
            mTimeTxt.SafeSetText(string.Format("{0}~{1}", _TransTimeStampToStr(model.StartTime), _TransTimeStampToStr(model.EndTime)));
            mRuleTxt.SafeSetText(model.RuleDesc);
        }
        private string _TransTimeStampToStr(UInt32 timeStamp)
        {
            System.DateTime time = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            DateTime dt = time.AddSeconds(timeStamp);// unix 总秒数
            return string.Format("{0}年{1}月{2}日{3:HH:mm}", dt.Year, dt.Month, dt.Day, dt);
        }
    }
}
