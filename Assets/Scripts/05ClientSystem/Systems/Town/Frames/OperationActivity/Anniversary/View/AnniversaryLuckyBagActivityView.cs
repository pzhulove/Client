using Protocol;
using ProtoTable;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class AnniversaryLuckyBagActivityView : MonoBehaviour, IGiftPackActivityView
    {
        [SerializeField]
        private Button mBuyBtn;

        [SerializeField]
        private List<Transform> mItemParent;

        [SerializeField]
        private List<Text> mItemNameTxt;

        [SerializeField]
        private Text mGiftNameTxt;

        [SerializeField]
        private Transform mGiftParent;

        private ActivityItemBase.OnActivityItemClick<int> mOnItemClick;

        [SerializeField]
        private Text mTimeTxt;

        [SerializeField]
        private Text mRuleDesTxt;

        [SerializeField]
        private Text mLimitAccountTxt;


        private int accountRestBuyNum = 0;

        [SerializeField]
        private Vector2 mComItemSize = new Vector2(90, 90);
        private LimitTimeGiftPackModel mModle;


        private readonly List<int> mRequestedGiftPackIds = new List<int>();

        void Awake()
        {
            mBuyBtn.SafeRemoveAllListener();
            mBuyBtn.SafeAddOnClickListener(_OnBuyClick);

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnSyncWorldMallBuySucceed, OnSyncWorldMallBuySucceed);
        }


        void OnDestroy()
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
            mTimeTxt.SafeSetText(string.Format("{0}~{1}", _TransTimeStampToStr(model.StartTime), _TransTimeStampToStr(model.EndTime)));
            mRuleDesTxt.SafeSetText(model.RuleDesc);
            mOnItemClick = onItemClick;
            accountRestBuyNum = 0;
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GetGiftData, _OnGetGiftData);
            UpdateMyData(model);
            UpdateAccountResNum(-1);
            UpdateBuyButton();


        }



        public void Close()
        {
            mRequestedGiftPackIds.Clear();
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GetGiftData, _OnGetGiftData);
            Destroy(gameObject);
        }



        public void UpdateData(LimitTimeGiftPackModel model)
        {
            UpdateMyData(model);
            UpdateAccountResNum(-1);
            UpdateBuyButton();
        }

        private void UpdateMyData(LimitTimeGiftPackModel model)
        {
            mModle = model;
            if (model.DetailDatas.Count > 0)
            {

                for (int i = 0; i < model.DetailDatas.Count; i++)
                {
                    for (int j = 0; j < model.DetailDatas[i].mRewards.Length; j++)//礼包的内容
                    {

                        GiftPackDataManager.GetInstance().GetGiftPackItem((int)model.DetailDatas[i].mRewards[j].id);
                        ItemData itemData = ItemDataManager.CreateItemDataFromTable((int)model.DetailDatas[i].mRewards[j].id);
                        if (!mRequestedGiftPackIds.Contains(itemData.TableID))
                        {
                            mRequestedGiftPackIds.Add(itemData.TableID);
                        }
                      
                    }

                }
               

            }
        }
        private void _OnGetGiftData(UIEvent param)
        {
            if (param == null || param.Param1 == null)
            {
                Logger.LogError("礼包数据为空");
                return;
            }

            GiftPackSyncInfo data = param.Param1 as GiftPackSyncInfo;
            if (!mRequestedGiftPackIds.Contains((int)data.id))
            {
                return;
            }
            PreViewDataModel preViewDataModel = new PreViewDataModel();
            preViewDataModel.isCreatItem = false;
            preViewDataModel.preViewItemList = new List<PreViewItemData>();

            if (mItemParent.Count < data.gifts.Length&& mItemNameTxt.Count<data.gifts.Length)
            {
                Logger.LogError("礼包的数量大于ComItem位置的数量，请添加新的位置");
                return;
            }
            if (data != null)
            {
                for (int i = 0; i < data.gifts.Length; ++i)
                {
                    GiftPackItemData giftTable = GiftPackDataManager.GetGiftDataFromNet(data.gifts[i]);
                    if (giftTable.ItemID > 0)
                    {
                        for (int j = 0; j < mItemParent[i].childCount; j++)
                        {
                            Destroy(mItemParent[i].GetChild(j).gameObject);
                        }
                        ComItem comItem = ComItemManager.Create(mItemParent[i].gameObject);
                        if (giftTable.ItemID > 0 && comItem != null)
                        {
                            ItemData itemData = ItemDataManager.CreateItemDataFromTable((int)data.gifts[i].itemId);
                            itemData.Count = (int)data.gifts[i].itemNum;
                            comItem.Setup(itemData, Utility.OnItemClicked);
                            (comItem.transform as RectTransform).sizeDelta = mComItemSize;

                            mItemNameTxt[i].SafeSetText(itemData.Name);
                        }
                        
                    }
                  
                }
            
            }
            if (mRequestedGiftPackIds.Count > 0)
            {
                for (int i = 0; i < mGiftParent.childCount; i++)
                {
                    Destroy(mGiftParent.GetChild(i).gameObject);
                }
                ComItem comItem = ComItemManager.Create(mGiftParent.gameObject);
                if (mRequestedGiftPackIds[0] > 0 && comItem != null)
                {
                    ItemData itemData = ItemDataManager.CreateItemDataFromTable(mRequestedGiftPackIds[0]);
                    itemData.Count = 1;
                    comItem.Setup(itemData, Utility.OnItemClicked);
                    (comItem.transform as RectTransform).sizeDelta = mComItemSize;
                    mGiftNameTxt.SafeSetText(itemData.Name);
                }
            }
        }

        private void UpdateAccountResNum(int accountNum = -1)
        {
            if (mModle.DetailDatas != null && mModle.DetailDatas.Count >= 1)
            {
                if (accountNum == -1)
                {
                    accountRestBuyNum = (int)mModle.DetailDatas[0].AccountRestBuyNum;
                }
                else
                {
                    accountRestBuyNum = accountNum;
                }

                int totalNum = (int)mModle.DetailDatas[0].AccountLimitBuyNum;
                switch (mModle.DetailDatas[0].AccountRefreshType)
                {
                    case (int)RefreshType.REFRESH_TYPE_PER_DAY:
                        mLimitAccountTxt.SafeSetText(string.Format(TR.Value("count_limittime_mall_limit_number_today"), accountRestBuyNum, totalNum));
                        break;
                    case (int)RefreshType.REFRESH_TYPE_PER_WEEK:
                        mLimitAccountTxt.SafeSetText(string.Format(TR.Value("count_limittime_mall_limit_number_week"), accountRestBuyNum, totalNum));
                        break;
                    case (int)RefreshType.REFRESH_TYPE_PER_MONTH:
                        mLimitAccountTxt.SafeSetText(string.Format(TR.Value("count_limittime_mall_limit_number_month"), accountRestBuyNum, totalNum));
                        break;
                    case (int)RefreshType.REFRESH_TYPE_NONE:
                        mLimitAccountTxt.SafeSetText(string.Format(TR.Value("count_limittime_mall_limit_number_everyday"), accountRestBuyNum, totalNum));
                        break;
                }
            }
        }
        private void OnSyncWorldMallBuySucceed(UIEvent uiEvent)
        {
            if (uiEvent == null)
                return;

            if (uiEvent.Param1 == null || uiEvent.Param2 == null || uiEvent.Param3 == null)
                return;
            if (mModle.DetailDatas == null || mModle.DetailDatas.Count < 1) return;
            UInt32 itemId = (UInt32)uiEvent.Param1;
            int resLimitNum = (int)uiEvent.Param3;
            if (itemId == mModle.DetailDatas[0].Id)
            {
                accountRestBuyNum = resLimitNum;
                UpdateAccountResNum(accountRestBuyNum);
                UpdateBuyButton();
                ActivityDataManager.GetInstance().RequestMallGiftData(MallTypeTable.eMallType.SN_GRATITUDE_LUCKYBAG);
            }

        }



        private void UpdateBuyButton()
        {
            bool ret = accountRestBuyNum > 0;
            if (mBuyBtn != null)
            {
                mBuyBtn.enabled = ret;
                mBuyBtn.GetComponent<UIGray>().enabled = !ret;
            }



        }
        private void _OnBuyClick()
        {
            if (mOnItemClick != null)
            {
                mOnItemClick(0, 0, 0);
            }
        }

        private string _TransTimeStampToStr(UInt32 timeStamp)
        {
            System.DateTime time = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            DateTime dt = time.AddSeconds(timeStamp);// unix 总秒数
            return string.Format("{0}月{1}日{2:HH:mm}", dt.Month, dt.Day, dt);
        }
    }
}
