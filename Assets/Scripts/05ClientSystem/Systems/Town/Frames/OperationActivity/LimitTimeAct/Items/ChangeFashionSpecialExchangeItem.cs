using System.Collections.Generic;
using Protocol;
using UnityEngine;
using UnityEngine.UI;
using ProtoTable;
using System;

namespace GameClient
{
    public class ChangeFashionSpecialExchangeItem : ActivityItemBase
    {
        [SerializeField]
        private Toggle mToggleHead;
        [SerializeField]
        private UIGray mToggleHeadGray;
        [SerializeField]
        private Toggle mToggleClothes;
        [SerializeField]
        private UIGray mToggleClothesGray;
        [SerializeField]
        private Toggle mTogglePants;
        [SerializeField]
        private UIGray mTogglePantsGray;
        [SerializeField]
        private Toggle mToggleChest;
        [SerializeField]
        private UIGray mToggleChestGray;
        [SerializeField]
        private Toggle mToggleWaist;
        [SerializeField]
        private UIGray mToggleWaistGray;
        [SerializeField]
        private Button mPreviewBtn;
        [SerializeField]
        private GameObject mMaterialFashion;
        [SerializeField]
        private GameObject mMaterialBox;
        [SerializeField]
        private GameObject mTargetFashion;
        [SerializeField]
        private Button mGetFashionBtn;
        [SerializeField]
        private Button mGetBoxBtn;
        [SerializeField]
        private Button mExchangeBtn;
        [SerializeField]
        private GameObject mGrayBtn;
        [SerializeField]
        private int mFashionItemId;
        [SerializeField]
        private Text mBoxCount;
        [SerializeField]
        private Text mExchangeBtnText;
        [SerializeField]
        private Text mGetFashionBtnText;

        private List<ComItem> mComItems = new List<ComItem>();
        private ulong mCurSelectFashionUid = 0;
        private int mCurFashionPart = -1;
        private int mCurSelectTaskId = 0;
        ILimitTimeActivityModel thisMode = null;
        const string yellowBtnImagePath = "UI/Image/Packed/p_UI_Common00.png:UI_Tongyong_Anniu_Quren_Xuanzhong_01";
        const string blueBtnImagePath = "UI/Image/Packed/p_UI_Common00.png:UI_Tongyong_Anniu_Quren_Xuanzhong_02";
        public override void InitFromMode(ILimitTimeActivityModel data, OnActivityItemClick<int> onItemClick)
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnFashionNormalItemSelected, OnFashionNormalItemSelected);

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnFashionTicketBuyFinished, updateItem);

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnCountValueChange, updateItem);
            mActivityId = data.Id;
            mOnItemClick = onItemClick;
            thisMode = data;
            for (int i = 0;i< data.TaskDatas.Count;i++)
            {
                InitTask(data.TaskDatas[i]);
                
            }
            int haveNotFashionIndex = -1;
            for (int i = 0;i<data.TaskDatas.Count;i++)
            {
                //if (ItemDataManager.GetInstance().GetOwnedItemCount((int)data.TaskDatas[i].AwardDataList[0].id) == 0)
                //{
                    
                //    haveNotFashionIndex = (int)ProtoTable.ItemTable.eSubType.FASHION_HEAD;
                //    break;
                //}
                if(data.TaskDatas[i].State != OpActTaskState.OATS_OVER)
                {
                    haveNotFashionIndex = (int)data.TaskDatas[i].ParamNums2[0];
                    break;
                }
            }
            switch (haveNotFashionIndex)
            {
                case ((int)ProtoTable.ItemTable.eSubType.FASHION_HEAD):
                    mToggleHead.isOn = true;
                    break;
                case ((int)ProtoTable.ItemTable.eSubType.FASHION_CHEST):
                    mToggleClothes.isOn = true;
                    break;
                case ((int)ProtoTable.ItemTable.eSubType.FASHION_LEG):
                    mTogglePants.isOn = true;
                    break;
                case ((int)ProtoTable.ItemTable.eSubType.FASHION_EPAULET):
                    mToggleChest.isOn = true;
                    break;
                case ((int)ProtoTable.ItemTable.eSubType.FASHION_SASH):
                    mToggleWaist.isOn = true;
                    break;
                default:
                    mToggleHead.isOn = true;
                    break;
            }
            mPreviewBtn.SafeAddOnClickListener(()=>
            {
                _ShowAvartar(mFashionItemId);
            });
            
        }
        /// <summary>
        /// 数据更新
        /// </summary>
        /// <param name="data">新的数据</param>
        public override void UpdateData(ILimitTimeActivityTaskDataModel data)
        {
            for(int i = 0;i<thisMode.TaskDatas.Count;i++)
            {
                if(thisMode.TaskDatas[i].DataId == data.DataId)
                {
                    thisMode.TaskDatas[i] = data;
                }
            }
            if(mCurFashionPart == (int)data.ParamNums2[0])
            {
                UpdateMiddleUI((int)data.DataId);
                UpdateTitle(data);
            }
        }

        protected override void OnInit(ILimitTimeActivityTaskDataModel data)
        {

        }

        public override void Dispose()
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
            mPreviewBtn.SafeRemoveAllListener();
            mGetFashionBtn.SafeRemoveAllListener();
            mGetBoxBtn.SafeRemoveAllListener();
            mExchangeBtn.SafeRemoveOnClickListener(_OnItemClick);
            //mButtonExchange.SafeRemoveOnClickListener(_OnItemClick);
            //mButtonExchangeBlue.SafeRemoveOnClickListener(_OnItemClick);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnFashionNormalItemSelected, OnFashionNormalItemSelected);

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnFashionTicketBuyFinished, updateItem);

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnCountValueChange, updateItem);
        }
        void _OnShowTips(ItemData result)
        {
            if (result == null)
            {
                return;
            }
            ItemTipManager.GetInstance().ShowTip(result);
        }

        void UpdateMiddleUI(int taskId)
        {
            ILimitTimeActivityTaskDataModel data = null;
            for (int i = 0; i < thisMode.TaskDatas.Count;i++)
            {
                if(thisMode.TaskDatas[i].DataId == taskId)
                {
                    data = thisMode.TaskDatas[i];
                }
            }
            if(data == null)
            {
                return;
            }
            mId = data.DataId;
            mCurSelectFashionUid = 0;
            //List<int> idlist = ItemDataManager.GetInstance().GetItemListFromSubType((ProtoTable.ItemTable.eSubType)data.ParamNums2[0]);
            mCurFashionPart = (int)data.ParamNums2[0];
            mCurSelectTaskId = taskId;
            List<ItemData> fashionList = ItemDataManager.GetInstance().GetItemDataListBySubType((int)data.ParamNums2[0], EPackageType.Fashion);
            if(fashionList == null)
            {
                return;
            }
            ItemData tempItemData = null;
            for(int i = 0;i< fashionList.Count;i++)
            {
                var itemTableData = TableManager.GetInstance().GetTableItem<ItemTable>(fashionList[i].TableID);
                if(itemTableData != null)
                {
                    if(itemTableData.Color != ItemTable.eColor.PURPLE)
                    {
                        continue;
                    }
                    if(itemTableData.TimeLeft != 0)
                    {
                        continue;
                    }
                    tempItemData = fashionList[i];
                    mCurSelectFashionUid = fashionList[i].GUID;
                    break;
                }
            }
            if(tempItemData != null)
            {
                SetMaterialFashion(tempItemData);
                mGetFashionBtnText.text = "替换";
            }
            else
            {
                SetMaterialFashion(null);
                mGetFashionBtnText.text = "获取";
                //var jobTabData = TableManager.GetInstance().GetTableItem<JobTable>(PlayerBaseData.GetInstance().JobTableID);
                //if (jobTabData == null)
                //    return;

                //var mallItemTableData =
                //    TableManager.GetInstance().GetTableItem<MallItemTable>(jobTabData.FashionMergeFastBuyID);
                //if (mallItemTableData == null)
                //    return;

                //var itemId = GetFastBuyItemId(mallItemTableData);
                //var jobTempItemData = ItemDataManager.CreateItemDataFromTable(itemId);
                //SetMaterialFashion(jobTempItemData);
            }

            ComItem comBoxItem = mMaterialBox.GetComponentInChildren<ComItem>();
            if (comBoxItem == null)
            {
                var comItem = ComItemManager.Create(mMaterialBox);//可以这样写吗需要确认
                comBoxItem = comItem;
                mComItems.Add(comBoxItem);
            }
            ItemData BoxItemDetailData = ItemDataManager.CreateItemDataFromTable((int)data.ParamNums[0]);
            if (null == BoxItemDetailData)
            {
                return;
            }
            string tempBoxNum;
            if (ItemDataManager.GetInstance().GetOwnedItemCount((int)data.ParamNums[0]) < (int)data.ParamNums[1])
            {
                tempBoxNum = string.Format("<color=red>{0}</color>", ItemDataManager.GetInstance().GetOwnedItemCount((int)data.ParamNums[0]));
            }
            else
            {
                tempBoxNum = string.Format("<color=green>{0}</color>", ItemDataManager.GetInstance().GetOwnedItemCount((int)data.ParamNums[0]));
            }
            mBoxCount.text = string.Format("{0}/{1}", tempBoxNum, (int)data.ParamNums[1]);
            BoxItemDetailData.Count = 1;
            comBoxItem.Setup(BoxItemDetailData, (GameObject Obj, ItemData sitem) => { _OnShowTips(BoxItemDetailData); });


            ComItem comFashionItem = mTargetFashion.GetComponentInChildren<ComItem>();
            if (comFashionItem == null)
            {
                var comItem = ComItemManager.Create(mTargetFashion);//可以这样写吗需要确认
                comFashionItem = comItem;
                mComItems.Add(comFashionItem);
            }
            int tempItemId = getItemIdInGiftPack((int)data.AwardDataList[0].id);
            ItemData FashionItemDetailData;
            if (tempItemId == -1)
            {
                FashionItemDetailData = ItemDataManager.CreateItemDataFromTable((int)data.AwardDataList[0].id);
            }
            else
            {
                FashionItemDetailData = ItemDataManager.CreateItemDataFromTable(tempItemId);
            }
            if (null == FashionItemDetailData)
            {
                return;
            }
            FashionItemDetailData.Count = (int)data.AwardDataList[0].num;
            comFashionItem.Setup(FashionItemDetailData, (GameObject Obj, ItemData sitem) => { _OnShowTips(FashionItemDetailData); });

            if(data.State == OpActTaskState.OATS_OVER)
            {
                mGrayBtn.CustomActive(true);
            }
            else
            {
                mGrayBtn.CustomActive(false);
            }
            mExchangeBtn.SafeAddOnClickListener(_OnItemClick);


            mGetFashionBtn.SafeAddOnClickListener(AddMergeFashionOnClick);
            mGetBoxBtn.SafeAddOnClickListener(()=>
            {
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ActivityLimitTimeToggleChange, 10100);
            });

            if (data.State == OpActTaskState.OATS_FINISHED)
            {
                var tempImage = mExchangeBtn.GetComponent<Image>();
                if(tempImage != null)
                {
                    ETCImageLoader.LoadSprite(ref tempImage, yellowBtnImagePath);
                }
            }
            else
            {
                var tempImage = mExchangeBtn.GetComponent<Image>();
                if (tempImage != null)
                {
                    ETCImageLoader.LoadSprite(ref tempImage, blueBtnImagePath);
                }
            }
                string tempDoneNum;
            if(data.DoneNum < data.TotalNum)
            {
                tempDoneNum = string.Format("<color=red>{0}</color>", data.DoneNum);
            }
            else
            {
                tempDoneNum = string.Format("<color=white>{0}</color>", data.DoneNum);
            }
            mExchangeBtnText.SafeSetText(string.Format("兑换（{0}/{1}）", tempDoneNum, data.TotalNum));
        }

        void SetMaterialFashion(ItemData itemData)
        {
            if(itemData != null)
            { 
                ComItem comitem = mMaterialFashion.GetComponentInChildren<ComItem>();
                if (comitem == null)
                {
                    var comItem = ComItemManager.Create(mMaterialFashion);//可以这样写吗需要确认
                    comitem = comItem;
                    mComItems.Add(comitem);
                }
                ItemData ItemDetailData = itemData;
                if (null == ItemDetailData)
                {
                    return;
                }
                ItemDetailData.Count = 1;
                comitem.Setup(ItemDetailData, (GameObject Obj, ItemData sitem) => { _OnShowTips(ItemDetailData); });
                mCurSelectFashionUid = itemData.GUID;
            }
        }
        public void AddMergeFashionOnClick()
        {


            var fashionItemSelectedType = new FashionItemSelectedType
            {
                FashionPart = (ProtoTable.ItemTable.eSubType)mCurFashionPart,
                IsLeft = true,
                NeedBlue = false,
            };
            ClientSystemManager.GetInstance()
                .OpenFrame<FashionMergeNewItemFrame>(FrameLayer.Middle, fashionItemSelectedType);
        }
        protected override void _OnItemClick()
        {
            if(mOnItemClick != null)
            {
                mOnItemClick((int)mId, 0, mCurSelectFashionUid);
            }
        }
        void _ShowAvartar(int id)
        {
            if (ClientSystemManager.GetInstance().IsFrameOpen<PlayerTryOnFrame>())
            {
                var tryOnFrame = ClientSystemManager.GetInstance().GetFrame(typeof(PlayerTryOnFrame)) as PlayerTryOnFrame;
                if (tryOnFrame != null)
                {
                    tryOnFrame.Reset(id);
                }
            }
            else
            {
                ClientSystemManager.GetInstance().OpenFrame<PlayerTryOnFrame>(FrameLayer.Middle, id);
            }
        }

        int getItemIdInGiftPack(int id)
        {
            var itemTableData = TableManager.GetInstance().GetTableItem<ItemTable>(id);
            if (itemTableData == null)
            {
                return -1;
            }

            var giftPackTable = TableManager.GetInstance().GetTableItem<GiftPackTable>(itemTableData.PackageID);
            if (giftPackTable == null)
            {
                return -1;
            }
            var giftDataList = TableManager.GetInstance().GetGiftTableData(giftPackTable.ID);
            for (int i = 0; i < giftDataList.Count; i++)
            {
                var giftTableData = giftDataList[i];
                if (!giftTableData.RecommendJobs.Contains(PlayerBaseData.GetInstance().JobTableID))
                {
                    continue;
                }
                return giftTableData.ItemID;
            }
            return -1;
        }
        void InitTask(ILimitTimeActivityTaskDataModel data)
        {
            if (data != null && data.AwardDataList != null)
            {
                switch (data.ParamNums2[0])
                {
                    case ((int)ProtoTable.ItemTable.eSubType.FASHION_HEAD):
                        //头
                        var comHeadItem = ComItemManager.Create(mToggleHead.gameObject);
                        if (comHeadItem != null)
                        {
                            int tempItemId = getItemIdInGiftPack((int)data.AwardDataList[0].id);
                            ItemData item;
                            if(tempItemId == -1)
                            {
                                item = ItemDataManager.CreateItemDataFromTable((int)data.AwardDataList[0].id);
                            }
                            else
                            {
                                item = ItemDataManager.CreateItemDataFromTable(tempItemId);
                            }
                            item.Count = (int)data.AwardDataList[0].num;
                            comHeadItem.Setup(item, null);
                            mComItems.Add(comHeadItem);
                        }
                        mToggleHead.onValueChanged.RemoveAllListeners();
                        mToggleHead.onValueChanged.AddListener((value) =>
                        {
                            if(value)
                            {
                                UpdateMiddleUI((int)data.DataId);
                            }
                        });

                        if(data.State == OpActTaskState.OATS_OVER)
                        {
                            mToggleHeadGray.enabled = false;
                        }
                        else
                        {
                            mToggleHeadGray.enabled = true;
                        }
                        break;
                    case ((int)ProtoTable.ItemTable.eSubType.FASHION_CHEST):
                        //衣服
                        var comClothesItem = ComItemManager.Create(mToggleClothes.gameObject);
                        if (comClothesItem != null)
                        {

                            int tempItemId = getItemIdInGiftPack((int)data.AwardDataList[0].id);
                            ItemData item;
                            if (tempItemId == -1)
                            {
                                item = ItemDataManager.CreateItemDataFromTable((int)data.AwardDataList[0].id);
                            }
                            else
                            {
                                item = ItemDataManager.CreateItemDataFromTable(tempItemId);
                            }
                            item.Count = (int)data.AwardDataList[0].num;
                            comClothesItem.Setup(item, null);
                            mComItems.Add(comClothesItem);
                        }
                        mToggleClothes.onValueChanged.RemoveAllListeners();
                        mToggleClothes.onValueChanged.AddListener((value) =>
                        {
                            if (value)
                            {
                                UpdateMiddleUI((int)data.DataId);
                            }
                        });
                        if (data.State == OpActTaskState.OATS_OVER)
                        {
                            mToggleClothesGray.enabled = false;
                        }
                        else
                        {
                            mToggleClothesGray.enabled = true;
                        }
                        break;
                    case ((int)ProtoTable.ItemTable.eSubType.FASHION_LEG):
                        //裤子
                        var comPantsItem = ComItemManager.Create(mTogglePants.gameObject);
                        if (comPantsItem != null)
                        {
                            int tempItemId = getItemIdInGiftPack((int)data.AwardDataList[0].id);
                            ItemData item;
                            if (tempItemId == -1)
                            {
                                item = ItemDataManager.CreateItemDataFromTable((int)data.AwardDataList[0].id);
                            }
                            else
                            {
                                item = ItemDataManager.CreateItemDataFromTable(tempItemId);
                            }
                            item.Count = (int)data.AwardDataList[0].num;
                            comPantsItem.Setup(item, null);
                            mComItems.Add(comPantsItem);
                        }
                        mTogglePants.onValueChanged.RemoveAllListeners();
                        mTogglePants.onValueChanged.AddListener((value) =>
                        {
                            if (value)
                            {
                                UpdateMiddleUI((int)data.DataId);
                            }
                        });
                        if (data.State == OpActTaskState.OATS_OVER)
                        {
                            mTogglePantsGray.enabled = false;
                        }
                        else
                        {
                            mTogglePantsGray.enabled = true;
                        }
                        break;
                    case ((int)ProtoTable.ItemTable.eSubType.FASHION_EPAULET):
                        //胸饰
                        var comChestItem = ComItemManager.Create(mToggleChest.gameObject);
                        if (comChestItem != null)
                        {
                            int tempItemId = getItemIdInGiftPack((int)data.AwardDataList[0].id);
                            ItemData item;
                            if (tempItemId == -1)
                            {
                                item = ItemDataManager.CreateItemDataFromTable((int)data.AwardDataList[0].id);
                            }
                            else
                            {
                                item = ItemDataManager.CreateItemDataFromTable(tempItemId);
                            }
                            item.Count = (int)data.AwardDataList[0].num;
                            comChestItem.Setup(item, null);
                            mComItems.Add(comChestItem);
                        }
                        mToggleChest.onValueChanged.RemoveAllListeners();
                        mToggleChest.onValueChanged.AddListener((value) =>
                        {
                            if (value)
                            {
                                UpdateMiddleUI((int)data.DataId);
                            }
                        });
                        if (data.State == OpActTaskState.OATS_OVER)
                        {
                            mToggleChestGray.enabled = false;
                        }
                        else
                        {
                            mToggleChestGray.enabled = true;
                        }
                        break;
                    case ((int)ProtoTable.ItemTable.eSubType.FASHION_SASH):
                        //腰
                        var comWaistItem = ComItemManager.Create(mToggleWaist.gameObject);
                        if (comWaistItem != null)
                        {
                            int tempItemId = getItemIdInGiftPack((int)data.AwardDataList[0].id);
                            ItemData item;
                            if (tempItemId == -1)
                            {
                                item = ItemDataManager.CreateItemDataFromTable((int)data.AwardDataList[0].id);
                            }
                            else
                            {
                                item = ItemDataManager.CreateItemDataFromTable(tempItemId);
                            }
                            item.Count = (int)data.AwardDataList[0].num;
                            comWaistItem.Setup(item, null);
                            mComItems.Add(comWaistItem);
                        }
                        mToggleWaist.onValueChanged.RemoveAllListeners();
                        mToggleWaist.onValueChanged.AddListener((value) =>
                        {
                            if (value)
                            {
                                UpdateMiddleUI((int)data.DataId);
                            }
                        });
                        if (data.State == OpActTaskState.OATS_OVER)
                        {
                            mToggleWaistGray.enabled = false;
                        }
                        else
                        {
                            mToggleWaistGray.enabled = true;
                        }
                        break;
                }
            }
        }
        void UpdateTitle(ILimitTimeActivityTaskDataModel data)
        {
            if (data != null && data.AwardDataList != null)
            {
                switch (data.ParamNums2[0])
                {
                    case ((int)ProtoTable.ItemTable.eSubType.FASHION_HEAD):
                        //头
                        if (data.State == OpActTaskState.OATS_OVER)
                        {
                            mToggleHeadGray.enabled = false;
                        }
                        else
                        {
                            mToggleHeadGray.enabled = true;
                        }
                        break;
                    case ((int)ProtoTable.ItemTable.eSubType.FASHION_CHEST):
                        //衣服
                        if (data.State == OpActTaskState.OATS_OVER)
                        {
                            mToggleClothesGray.enabled = false;
                        }
                        else
                        {
                            mToggleClothesGray.enabled = true;
                        }
                        break;
                    case ((int)ProtoTable.ItemTable.eSubType.FASHION_LEG):
                        //裤子
                        if (data.State == OpActTaskState.OATS_OVER)
                        {
                            mTogglePantsGray.enabled = false;
                        }
                        else
                        {
                            mTogglePantsGray.enabled = true;
                        }
                        break;
                    case ((int)ProtoTable.ItemTable.eSubType.FASHION_EPAULET):
                        //胸饰
                        if (data.State == OpActTaskState.OATS_OVER)
                        {
                            mToggleChestGray.enabled = false;
                        }
                        else
                        {
                            mToggleChestGray.enabled = true;
                        }
                        break;
                    case ((int)ProtoTable.ItemTable.eSubType.FASHION_SASH):
                        //腰
                        if (data.State == OpActTaskState.OATS_OVER)
                        {
                            mToggleWaistGray.enabled = false;
                        }
                        else
                        {
                            mToggleWaistGray.enabled = true;
                        }
                        break;
                }
            }
        }
        private int GetFastBuyItemId(MallItemTable mallItemTableData)
        {
            string strItemId = "-1";
            int itemId = -1;

            try
            {
                switch ((ItemTable.eSubType)mCurFashionPart)
                {
                    case ItemTable.eSubType.FASHION_HEAD:
                        strItemId = mallItemTableData.giftpackitems.Split('|')[0].Split(':')[0];
                        //1
                        break;
                    case ItemTable.eSubType.FASHION_SASH:
                        strItemId = mallItemTableData.giftpackitems.Split('|')[4].Split(':')[0];
                        //5
                        break;
                    case ItemTable.eSubType.FASHION_CHEST:
                        strItemId = mallItemTableData.giftpackitems.Split('|')[1].Split(':')[0];
                        //2
                        break;
                    case ItemTable.eSubType.FASHION_LEG:
                        strItemId = mallItemTableData.giftpackitems.Split('|')[3].Split(':')[0];
                        //4
                        break;
                    case ItemTable.eSubType.FASHION_EPAULET:
                        strItemId = mallItemTableData.giftpackitems.Split('|')[2].Split(':')[0];
                        //3
                        break;
                }

                int.TryParse(strItemId, out itemId);
            }
            catch (Exception e)
            {
                Logger.LogErrorFormat("{0}", e.ToString());
                return -1;
            }

            return itemId;
        }
        private void OnFashionNormalItemSelected(UIEvent uiEvent)
        {
            if (uiEvent == null)
                return;

            var script = uiEvent.Param1 as ComFashionMergeItemEx;
            SetMaterialFashion(script.ItemData);
        }

        private void updateItem(UIEvent uiEvent)
        {
            UpdateMiddleUI(mCurSelectTaskId);
        }
    }
}
