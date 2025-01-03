using UnityEngine.UI;
using Scripts.UI;
using UnityEngine;
using ProtoTable;
using Protocol;
using Network;
using System;
using System.Collections.Generic;

namespace GameClient
{
    class FeedPetFrame : ClientFrame
    {
        private PetInfo curPetInfo;
        private ItemData PetFoodItemData;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Pet/FeedPetFrame";
        }
        
        protected override void _OnOpenFrame()
        {
            base._OnOpenFrame();

            if (userData == null)
            {
                Logger.LogError("can`t get userData");
                return;
            }
            curPetInfo = new PetInfo();
            PetDataManager.GetInstance().SetPetData(curPetInfo, userData as PetInfo);
            InitData();
            InitInterFace();
        }


        protected void BindUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.PetFeedSuccess, OnPetFeedSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.UpdatePetFoodNum, OnPetFeedSuccess);
        }

        protected void UnBindUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.PetFeedSuccess, OnPetFeedSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.UpdatePetFoodNum, OnPetFeedSuccess);
        }

        protected override void _OnCloseFrame()
        {
            base._OnCloseFrame();
            ClearData();
        }

        void InitData()
        {
            // curPetInfo = new PetInfo();
            // List<PetInfo> OnUsePetItemList = PetDataManager.GetInstance().GetOnUsePetList();
            // PetDataManager.GetInstance().SetPetData(curPetInfo, OnUsePetItemList[index]);
        }

        void ClearData()
        {
            curPetInfo = null;
            PetFoodItemData = null;
            UnBindUIEvent();
        }

        void InitInterFace()
        {
            if (curPetInfo == null)
                return;
            PetDataManager.GetInstance().SetPetItemData(mPetItem, curPetInfo, PlayerBaseData.GetInstance().JobTableID);

            PetTable petData = TableManager.GetInstance().GetTableItem<PetTable>((int)curPetInfo.dataId);
            if (petData == null)
            {
                return;
            }

            if (mPetName != null)
                mPetName.text = PetDataManager.GetInstance().GetColorName(petData.Name, petData.Quality);

            string sTypeIcon = PetDataManager.GetInstance().GetPetTypeIconPath(petData.PetType);
            if (!string.IsNullOrEmpty(sTypeIcon) && sTypeIcon != "-")
            {
                ETCImageLoader.LoadSprite(ref mTypeIcon, sTypeIcon);
            }

            mTypeName.SafeSetText(PetDataManager.GetInstance().GetPetTypeDesc(petData.PetType));

            SystemValueTable SatietyData = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType.SVT_PET_HUNGER_MAX_NUM);
            var MaxSatietyNum = SatietyData.Value;
            mSatietyNum.SafeSetText(string.Format("{0}/{1}", curPetInfo.hunger, MaxSatietyNum));


            UpdatePetFoodItemData();
            SetPetFoodComItem();
            BindUIEvent();
        }

        void OnPetFeedSuccess(UIEvent iEvent)
        {
            if (curPetInfo != null)
            {
                SystemValueTable SatietyData = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType.SVT_PET_HUNGER_MAX_NUM);
                var MaxSatietyNum = SatietyData.Value;
                mSatietyNum.SafeSetText(string.Format("{0}/{1}", curPetInfo.hunger, MaxSatietyNum));
            }

            UpdatePetFoodItemData();
            SetPetFoodComItem();
        }

        void UpdatePetFoodItemData()
        {
            int PetFoodTableID = 0;
            int iCount = PetDataManager.GetInstance().GetPetFoodNum(ref PetFoodTableID);

            if (iCount > 0)
            {
                PetFoodItemData = ItemDataManager.CreateItemDataFromTable(PetFoodTableID);
                PetFoodItemData.Count = 1;

                mItemNum.text = string.Format("{0}/1", iCount);

                ItemData foodData = TableManager.GetInstance().GetTableItem<ItemData>(PetFoodTableID);
                if (foodData != null)
                {
                    var value = string.Format("<color={0}>{1}</color>", foodData.Quality.ToString().ToLower(), foodData.Name);
                    mFoodName.SafeSetText(value);
                    mFoodDesc.SafeSetText(foodData.Description);
                    mFoodName.CustomActive(true);
                    mFoodDesc.CustomActive(true);
                }
            }
            else
            {
                mItemNum.text = "";
                mFoodName.CustomActive(false);
                mFoodDesc.CustomActive(false);
            }

        }

        void OnPetFoodItemTips()
        {
            if (PetFoodItemData == null)
            {
                return;
            }

            ItemTipManager.GetInstance().ShowTip(PetFoodItemData);
        }

        void SetPetFoodComItem()
        {
            int PetFoodTableID = 0;
            int iCount = PetDataManager.GetInstance().GetPetFoodNum(ref PetFoodTableID);

            ComItem ShowItem = mAddBtn.GetComponentInChildren<ComItem>();

            if (ShowItem == null)
            {
                ShowItem = CreateComItem(mAddBtn.gameObject);
            }

            if (iCount <= 0)
            {
                ShowItem.gameObject.CustomActive(false);
            }
            else
            {
                ShowItem.gameObject.CustomActive(true);
            }

            ShowItem.Setup(PetFoodItemData, (GameObject Obj, ItemData sitem) => { OnPetFoodItemTips(); });
        }

        #region ExtraUIBind
        private GameObject mPetItem = null;
        private Text mPetName = null;
        private Image mTypeIcon = null;
        private Text mTypeName = null;
        private Text mSatietyNum = null;
        private Button mAddBtn = null;
        private Button mFeed = null;
        private Text mFoodName = null;
        private Text mFoodDesc = null;
        private Text mItemNum = null;
        private Button mCloseBtn = null;

        protected override void _bindExUI()
        {
            mPetItem = mBind.GetGameObject("PetItem");
            mPetName = mBind.GetCom<Text>("PetName");
            mTypeIcon = mBind.GetCom<Image>("TypeIcon");
            mTypeName = mBind.GetCom<Text>("TypeName");
            mSatietyNum = mBind.GetCom<Text>("SatietyNum");
            mAddBtn = mBind.GetCom<Button>("AddBtn");
            mAddBtn.onClick.AddListener(_onAddBtnButtonClick);
            mFeed = mBind.GetCom<Button>("feed");
            mFeed.onClick.AddListener(_onFeedButtonClick);
            mFoodName = mBind.GetCom<Text>("FoodName");
            mFoodDesc = mBind.GetCom<Text>("FoodDesc");
            mItemNum = mBind.GetCom<Text>("itemNum");
            mCloseBtn = mBind.GetCom<Button>("CloseBtn");
            mCloseBtn.onClick.AddListener(_onCloseBtnButtonClick);
        }

        protected override void _unbindExUI()
        {
            mPetItem = null;
            mPetName = null;
            mTypeIcon = null;
            mTypeName = null;
            mSatietyNum = null;
            mAddBtn.onClick.RemoveListener(_onAddBtnButtonClick);
            mAddBtn = null;
            mFeed.onClick.RemoveListener(_onFeedButtonClick);
            mFeed = null;
            mFoodName = null;
            mFoodDesc = null;
            mItemNum = null;
            mCloseBtn.onClick.RemoveListener(_onCloseBtnButtonClick);
            mCloseBtn = null;
        }
        #endregion

        #region Callback
        private void _onAddBtnButtonClick()
        {
            int PetFoodTableID = 0;
            int iCount = PetDataManager.GetInstance().GetPetFoodNum(ref PetFoodTableID);

            if (iCount <= 0)
            {
                ItemComeLink.OnLink(811000004, 0);
                Close();
            }

        }
        private void _onFeedButtonClick()
        {
            if (PetFoodItemData == null)
            {
                SystemNotifyManager.SysNotifyFloatingEffect("宠物口粮不足");
                return;
            }

            if (PetFoodItemData.Count <= 0)
            {
                SystemNotifyManager.SysNotifyFloatingEffect("宠物口粮不足");
                return;
            }

            PetDataManager.GetInstance().SendFeedPetReq(PetFeedTable.eType.PET_FEED_ITEM, curPetInfo.id);

        }

        private void _onCloseBtnButtonClick()
        {
            Close();

        }
        #endregion
    }
}
