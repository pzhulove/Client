using Scripts.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class LimitTimeGroupBuyWelfareFrame : ClientFrame
    {
        #region ExtraUIBind
        private ComUIListScript mBottomContent = null;
        private ComUIListScript mMiddleContent = null;
        private ComUIListScript mTopContent = null;
        private Button mClose = null;

        protected sealed override void _bindExUI()
        {
            mBottomContent = mBind.GetCom<ComUIListScript>("BottomContent");
            mMiddleContent = mBind.GetCom<ComUIListScript>("MiddleContent");
            mTopContent = mBind.GetCom<ComUIListScript>("TopContent");
            mClose = mBind.GetCom<Button>("Close");
            mClose.onClick.AddListener(_onCloseButtonClick);
        }

        protected sealed override void _unbindExUI()
        {
            mBottomContent = null;
            mMiddleContent = null;
            mTopContent = null;
            mClose.onClick.RemoveListener(_onCloseButtonClick);
            mClose = null;
        }
        #endregion

        #region Callback
        private void _onCloseButtonClick()
        {
            frameMgr.CloseFrame(this);
        }
        #endregion

        private List<ActivityDataManager.LimitTimeGroupBuyPreviewDataModel> mMayDayList = new List<ActivityDataManager.LimitTimeGroupBuyPreviewDataModel>();
        private List<ActivityDataManager.LimitTimeGroupBuyPreviewDataModel> mGoblinChamberList = new List<ActivityDataManager.LimitTimeGroupBuyPreviewDataModel>();
        private List<ActivityDataManager.LimitTimeGroupBuyPreviewDataModel> mGoblinChamberNewList = new List<ActivityDataManager.LimitTimeGroupBuyPreviewDataModel>();

        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/OperateActivity/LimitTime/Activities/LimitTimeGroupBuyWelfareFrame";
        }

        protected sealed override void _OnOpenFrame()
        {
            InitmTopComUIList();
            InitmMiddleComUIList();
            InitmBottomComUIList();

            OnSetElementAmount();
        }

        protected sealed override void _OnCloseFrame()
        {
            UnInitmTopComUIList();
            UnInitmMiddleComUIList();
            UnInitmBottomComUIList();
        }

        #region Top

        private void InitmTopComUIList()
        {
            if (mTopContent != null)
            {
                mTopContent.Initialize();
                mTopContent.onBindItem += OnTopBindItemDelegate;
                mTopContent.onItemVisiable += OnTopItemVisiableDelegate;
            }
        }

        private void UnInitmTopComUIList()
        {
            if (mTopContent != null)
            {
                mTopContent.onBindItem -= OnTopBindItemDelegate;
                mTopContent.onItemVisiable -= OnTopItemVisiableDelegate;
            }
        }

        private ComCommonBind OnTopBindItemDelegate(GameObject itemObject)
        {
            return itemObject.GetComponent<ComCommonBind>();
        }

        private void OnTopItemVisiableDelegate(ComUIListElementScript item)
        {
            var mBind = item.gameObjectBindScript as ComCommonBind;
            if (mBind != null && item.m_index >= 0 && item.m_index < mMayDayList.Count)
            {
                UpdateTopItemInfo(mBind, mMayDayList[item.m_index]);
            }
        }

        private void UpdateTopItemInfo(ComCommonBind bind, ActivityDataManager.LimitTimeGroupBuyPreviewDataModel data)
        {
            if (bind == null || data == null)
            {
                return;
            }

            Text goblinCoinCount = bind.GetCom<Text>("DijingCount");
            Text price = bind.GetCom<Text>("Price");
            Text itemName = bind.GetCom<Text>("itemName");
            Image itemBackgroud = bind.GetCom<Image>("backgroud");
            Image itemIcon = bind.GetCom<Image>("Icon");
            Button itemIconBtn = bind.GetCom<Button>("IconBtn");

            ItemData itemData = ItemDataManager.CreateItemDataFromTable(data.itemId);
            if (itemData != null)
            {
                if (itemName != null)
                    itemName.text = itemData.GetColorName();

                if (itemBackgroud != null)
                    ETCImageLoader.LoadSprite(ref itemBackgroud, itemData.GetQualityInfo().Background);

                if (itemIcon != null)
                    ETCImageLoader.LoadSprite(ref itemIcon, itemData.Icon);

                if(itemIconBtn != null)
                {
                    itemIconBtn.onClick.RemoveAllListeners();
                    itemIconBtn.onClick.AddListener(() => { ItemTipManager.GetInstance().ShowTip(itemData); });
                }
            }

            if (goblinCoinCount != null)
                goblinCoinCount.text = data.goblinCoin.ToString();
            if (price != null)
                price.text = string.Format("{0}点券", data.price);
        }
        #endregion

        #region Middle

        private void InitmMiddleComUIList()
        {
            if (mMiddleContent != null)
            {
                mMiddleContent.Initialize();
                mMiddleContent.onBindItem += OnMiddleBindItemDelegate;
                mMiddleContent.onItemVisiable += OnMiddleItemVisiableDelegate;
            }
        }

        private void UnInitmMiddleComUIList()
        {
            if (mMiddleContent != null)
            {
                mMiddleContent.onBindItem -= OnMiddleBindItemDelegate;
                mMiddleContent.onItemVisiable -= OnMiddleItemVisiableDelegate;
            }
        }

        private ComCommonBind OnMiddleBindItemDelegate(GameObject itemObject)
        {
            return itemObject.GetComponent<ComCommonBind>();
        }

        private void OnMiddleItemVisiableDelegate(ComUIListElementScript item)
        {
            var comBind = item.gameObjectBindScript as ComCommonBind;
            if (comBind != null && item.m_index >= 0 && item.m_index < mGoblinChamberList.Count)
            {
                UpdateMiddleOrBottomItemInfo(comBind, mGoblinChamberList[item.m_index]);
            }
        }

        private void UpdateMiddleOrBottomItemInfo(ComCommonBind bind, ActivityDataManager.LimitTimeGroupBuyPreviewDataModel data)
        {
            if (bind == null || data == null)
            {
                return;
            }

            Text count = bind.GetCom<Text>("Count");
            Text itemName = bind.GetCom<Text>("itemName");
            Image itemBackground = bind.GetCom<Image>("backgroud");
            Image itemIcon = bind.GetCom<Image>("Icon");
            Button itemIconBtn = bind.GetCom<Button>("IconBtn");

            ItemData itemData = ItemDataManager.CreateItemDataFromTable(data.itemId);
            if (itemData != null)
            {
                if (itemName != null)
                    itemName.text = itemData.GetColorName();

                if (itemBackground != null)
                    ETCImageLoader.LoadSprite(ref itemBackground, itemData.GetQualityInfo().Background);

                if (itemIcon != null)
                    ETCImageLoader.LoadSprite(ref itemIcon, itemData.Icon);

                if(itemIconBtn != null)
                {
                    itemIconBtn.onClick.RemoveAllListeners();
                    itemIconBtn.onClick.AddListener(() => { ItemTipManager.GetInstance().ShowTip(itemData); });
                }
            }

            if (count != null)
                count.text = data.goblinCoin.ToString();
        }
        #endregion

        #region Bottom

        private void InitmBottomComUIList()
        {
            if(mBottomContent != null)
            {
                mBottomContent.Initialize();
                mBottomContent.onBindItem += OnBottomBindItemDelegate;
                mBottomContent.onItemVisiable += OnBottomItemVisiableDelegate;
            }
        }

        private void UnInitmBottomComUIList()
        {
            if(mBottomContent != null)
            {
                mBottomContent.onBindItem -= OnBottomBindItemDelegate;
                mBottomContent.onItemVisiable -= OnBottomItemVisiableDelegate;
            }
        }

        private ComCommonBind OnBottomBindItemDelegate(GameObject itemObject)
        {
            return itemObject.GetComponent<ComCommonBind>();
        }

        private void OnBottomItemVisiableDelegate(ComUIListElementScript item)
        {
            var comBind = item.gameObjectBindScript as ComCommonBind;
            if (comBind != null && item.m_index >= 0 && item.m_index < mGoblinChamberNewList.Count)
            {
                UpdateMiddleOrBottomItemInfo(comBind, mGoblinChamberNewList[item.m_index]);
            }
        }
        #endregion

        private void OnSetElementAmount()
        {
            mMayDayList = ActivityDataManager.GetLimitTimeGroupBuyPrevieDataList(ActivityDataManager.LimitTimeGroupBuyPreviewType.MayDay);
            mTopContent.SetElementAmount(mMayDayList.Count);

            mGoblinChamberList = ActivityDataManager.GetLimitTimeGroupBuyPrevieDataList(ActivityDataManager.LimitTimeGroupBuyPreviewType.GoblinChamber);
            mMiddleContent.SetElementAmount(mGoblinChamberList.Count);

            mGoblinChamberNewList = ActivityDataManager.GetLimitTimeGroupBuyPrevieDataList(ActivityDataManager.LimitTimeGroupBuyPreviewType.GoblinChamberNew);
            mBottomContent.SetElementAmount(mGoblinChamberNewList.Count);
        }
    }
}
