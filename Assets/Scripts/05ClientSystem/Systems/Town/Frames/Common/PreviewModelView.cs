using ProtoTable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Scripts.UI;

namespace GameClient
{
    public class PreviewModelView : MonoBehaviour,IDisposable
    {
        [SerializeField]private Text mName;
        [SerializeField]private SpriteAniRenderChenghao mSpriteAniRenderChenghao;
        [SerializeField]private GeAvatarRendererEx mGeAvatarRendererEx;
        [SerializeField]private Image mAniImage;
        [SerializeField]private Button mCloseBtn;
        [SerializeField]private Button mLeftArrowBtn;
        [SerializeField]private Button mRightArrowBtn;
        [SerializeField]private ComUIListScript mPreViewItemList;

        [SerializeField]
        private GameObject actorPos = null;
        [SerializeField]
        private GameObject itemTipRoot = null;
        [SerializeField]
        private Button showItemAttr = null;
        [SerializeField]
        private Button closeShowItemAttr = null;
        private int mCurrentSelectIndex = 0;
        private PreViewDataModel mPreViewDataModel = null;
        private bool isInitialize = false;

        /// <summary>
        /// 一般来说 如果Item类型是礼包的类型，预览的名字是MallLimitTimeActivity的FashionName
        /// 但是某些特殊的礼包，要显示的是道具的名称
        /// </summary>
        private List<int> mShowItemNameId = new List<int> { 800001223};
        void MoveOffset(GameObject obj, int ix, int iy)
        {
            if (obj == null)
            {
                return;
            }
            RectTransform objrect = obj.GetComponent<RectTransform>();
            Vector3 endPos = new Vector3();
            endPos = objrect.localPosition;
            endPos.x += ix;
            endPos.y += iy;
            obj.transform.localPosition = endPos;
            return;
        }
        const int offsetX = 400;
        private void Awake()
        {
            if (mCloseBtn != null)
            {
                mCloseBtn.onClick.RemoveAllListeners();
                mCloseBtn.onClick.AddListener(() => 
                {
                    ClientSystemManager.GetInstance().CloseFrame<PreviewModelFrame>();
                });
            }

            if (mLeftArrowBtn != null)
            {
                mLeftArrowBtn.onClick.RemoveAllListeners();
                mLeftArrowBtn.onClick.AddListener(OnLeftArrowClick);
            }

            if (mRightArrowBtn != null)
            {
                mRightArrowBtn.onClick.RemoveAllListeners();
                mRightArrowBtn.onClick.AddListener(OnRightArrowClick);
            }
            showItemAttr.SafeSetOnClickListener(() => 
            {
                ItemData a_item = ItemDataManager.CreateItemDataFromTable(mPreViewDataModel.preViewItemList[mCurrentSelectIndex].itemId);               
                ItemTipData tipData = new ItemTipData();
                tipData.item = a_item;
                tipData.itemSuitObj = EquipSuitDataManager.GetInstance().GetSelfEquipSuitObj(a_item.SuitID);
                tipData.compareItem = null;
                tipData.compareItemSuitObj = null;
                tipData.textAnchor = TextAnchor.MiddleCenter;
                tipData.funcs = null;
                tipData.IsForceCloseModelAvatar = true;              //强制关闭展示ModelAvatar
                ClientSystemManager.GetInstance().OpenFrame<ItemTipFrame>(itemTipRoot, tipData, "ShowAttrTip");
                MoveOffset(actorPos, -offsetX, 0);
                MoveOffset(mName.gameObject, -offsetX, 0);
                MoveOffset(mAniImage.gameObject, -offsetX, 0);
                showItemAttr.CustomActive(false);
                closeShowItemAttr.CustomActive(true);
                mLeftArrowBtn.CustomActive(false);
                mRightArrowBtn.CustomActive(false);
                mPreViewItemList.CustomActive(false);
                mCloseBtn.CustomActive(false);
            });
            closeShowItemAttr.SafeSetOnClickListener(() => 
            {
                ClientSystemManager.GetInstance().CloseFrame("ShowAttrTip");
                MoveOffset(actorPos, offsetX, 0);
                MoveOffset(mName.gameObject, offsetX, 0);
                MoveOffset(mAniImage.gameObject, offsetX, 0);
                showItemAttr.CustomActive(true);
                closeShowItemAttr.CustomActive(false);
//                 mLeftArrowBtn.CustomActive(true);
//                 mRightArrowBtn.CustomActive(true);
                mPreViewItemList.CustomActive(true);
                mCloseBtn.CustomActive(true);

                UpdateViewShowInfo();
            });
        }

        private void OnLeftArrowClick()
        {
            if (mCurrentSelectIndex < 0 || mCurrentSelectIndex >= mPreViewDataModel.preViewItemList.Count)
            {
                return;
            }

            --mCurrentSelectIndex;
            
            UpdateViewShowInfo();
            
            UpdateAvatar(mPreViewDataModel.preViewItemList[mCurrentSelectIndex]);

            if (mPreViewDataModel.isCreatItem == true)
                SelectElement();
        }

        private void OnRightArrowClick()
        {
            if (mCurrentSelectIndex < 0 || mCurrentSelectIndex >= mPreViewDataModel.preViewItemList.Count)
            {
                return;
            }

            ++mCurrentSelectIndex;
            
            UpdateViewShowInfo();

            UpdateAvatar(mPreViewDataModel.preViewItemList[mCurrentSelectIndex]);

            if (mPreViewDataModel.isCreatItem == true)
                SelectElement();
        }

        private void Update()
        {
            if (isInitialize == true)
            {
                if (mGeAvatarRendererEx != null)
                {
                    while (global::Global.Settings.avatarLightDir.x > 360)
                        global::Global.Settings.avatarLightDir.x -= 360;
                    while (global::Global.Settings.avatarLightDir.x < 0)
                        global::Global.Settings.avatarLightDir.x += 360;

                    while (global::Global.Settings.avatarLightDir.y > 360)
                        global::Global.Settings.avatarLightDir.y -= 360;
                    while (global::Global.Settings.avatarLightDir.y < 0)
                        global::Global.Settings.avatarLightDir.y += 360;

                    while (global::Global.Settings.avatarLightDir.z > 360)
                        global::Global.Settings.avatarLightDir.z -= 360;
                    while (global::Global.Settings.avatarLightDir.z < 0)
                        global::Global.Settings.avatarLightDir.z += 360;

                    mGeAvatarRendererEx.m_LightRot = global::Global.Settings.avatarLightDir;
                }
            }
        }

        public void InitView(PreViewDataModel preViewData)
        {
            mPreViewDataModel = preViewData;
            isInitialize = true;

            if (mPreViewDataModel == null)
            {
                return;
            }

            if (mPreViewDataModel.preViewItemList == null || mPreViewDataModel.preViewItemList.Count <= 0)
            {
                return;
            }

            InitPreViewItemList();

            UpdateViewShowInfo();

            if (mPreViewDataModel.preViewItemList.Count > 0)
            {
                UpdateAvatar(mPreViewDataModel.preViewItemList[mCurrentSelectIndex]);
            }
        }

        #region  PreViewItemList
        private void InitPreViewItemList()
        {
            if (showItemAttr != null)
            {
                showItemAttr.CustomActive(mPreViewDataModel.isCreatItem == true);
            }
            
            if (mPreViewDataModel.isCreatItem == true)
            {
                if (mPreViewItemList != null)
                {
                    mPreViewItemList.Initialize();
                    mPreViewItemList.onBindItem += OnBindItemDelegate;
                    mPreViewItemList.onItemVisiable += OnItemVisiableDelegate;
                    mPreViewItemList.onItemSelected += OnItemSelectedDelegate;
                    mPreViewItemList.onItemChageDisplay += OnItemChangeDisplayDelegate;

                    mPreViewItemList.SetElementAmount(mPreViewDataModel.preViewItemList.Count);
                    SelectElement();
                }
            }
        }

        private void SelectElement()
        {
            if (mCurrentSelectIndex < 0 || mCurrentSelectIndex >= mPreViewDataModel.preViewItemList.Count)
            {
                return;
            }

            if (mPreViewItemList != null)
                mPreViewItemList.SelectElement(mCurrentSelectIndex);
        }

        private PreviewItem OnBindItemDelegate(GameObject itemObject)
        {
            return itemObject.GetComponent<PreviewItem>();
        }

        private void OnItemVisiableDelegate(ComUIListElementScript item)
        {
            PreviewItem mPreviewItem = item.gameObjectBindScript as PreviewItem;
            if (mPreviewItem != null && item.m_index >= 0 && item.m_index < mPreViewDataModel.preViewItemList.Count)
            {
                mPreviewItem.OnItemVisiable(item.m_index, mPreViewDataModel.preViewItemList[item.m_index]);
            }
        }

        private void OnItemSelectedDelegate(ComUIListElementScript item)
        {
            PreviewItem mPreviewItem = item.gameObjectBindScript as PreviewItem;
            if (mPreviewItem != null)
            {
                mCurrentSelectIndex = mPreviewItem.Index;

                UpdateViewShowInfo();
                UpdateAvatar(mPreViewDataModel.preViewItemList[mCurrentSelectIndex]);
            }
        }

        private void OnItemChangeDisplayDelegate(ComUIListElementScript item, bool bSelected)
        {
            PreviewItem mPreviewItem = item.gameObjectBindScript as PreviewItem;
            if (mPreviewItem != null)
            {
                mPreviewItem.OnItemChangeDisplay(bSelected);
            }
        }

        private void UnInitPreViewItemList()
        {
            if (mPreViewDataModel.isCreatItem == true)
            {
                if (mPreViewItemList != null)
                {
                    mPreViewItemList.onBindItem -= OnBindItemDelegate;
                    mPreViewItemList.onItemVisiable -= OnItemVisiableDelegate;
                    mPreViewItemList.onItemSelected -= OnItemSelectedDelegate;
                    mPreViewItemList.onItemChageDisplay -= OnItemChangeDisplayDelegate;
                }
            }
        }
        #endregion

        private void UpdateAvatar(PreViewItemData preViewItem)
        {
            if (preViewItem == null)
            {
                return;
            }

            if (mSpriteAniRenderChenghao != null)
            {
                mSpriteAniRenderChenghao.gameObject.CustomActive(false);
            }
            
            var itemTable = TableManager.GetInstance().GetTableItem<ItemTable>(preViewItem.itemId);
            if (itemTable == null)
            {
                Logger.LogErrorFormat("can not find ItemTable with id:{0}", preViewItem.itemId);
            }
            else
            {
                if (itemTable.SubType == ItemTable.eSubType.GiftPackage)
                {
                    
                    var giftPackTable = TableManager.GetInstance().GetTableItem<GiftPackTable>(itemTable.PackageID);
                    if (giftPackTable == null)
                    {
                        return;
                    }

                    //如果是整套预览，脱掉自身穿戴的时装
                    if (giftPackTable.ShowAvatarModelType == GiftPackTable.eShowAvatarModelType.Complete)
                    {
                        InitAvatar(true);
                    }
                    else
                    {
                        InitAvatar();
                    }

                    var giftList = TableManager.GetInstance().GetGiftTableData(giftPackTable.ID); ;

                    bool haveInitAvartar = false;
                    for (int i = 0; i < giftList.Count; i++)
                    {
                        var giftTableData = giftList[i];
                        ItemTable tableData = TableManager.GetInstance().GetTableItem<ItemTable>(giftTableData.ItemID);
                        if (giftTableData == null|| tableData == null)
                        {
                            continue;
                        }
                        if(tableData.SubType != ItemTable.eSubType.PetEgg)
                        {
                            if (!giftTableData.RecommendJobs.Contains(PlayerBaseData.GetInstance().JobTableID))
                            {
                                continue;
                            }
                        }
                       
                        if (!haveInitAvartar)
                        {
                            haveInitAvartar = true;
                            RefreshAvatar(giftTableData.ItemID);
                        }
                        else
                        {
                            if (tableData == null)
                            {
                                continue;
                            }
                            if (tableData.SubType == ItemTable.eSubType.FASHION_HAIR)//添加礼包里面翅膀的预览
                            {
                                PlayerBaseData.GetInstance().AvatarEquipWing(mGeAvatarRendererEx, tableData.ID);
                            }
                            else if (tableData.SubType == ItemTable.eSubType.FASHION_WEAPON)//时装武器
                            {
                                PlayerBaseData.GetInstance().AvatarEquipWeapon(mGeAvatarRendererEx, PlayerBaseData.GetInstance().JobTableID, tableData.ID);
                            }
                            else if (tableData.SubType == ItemTable.eSubType.GiftPackage)//礼品里面配置的是一个礼包
                            {
                                var GiftPackTable = TableManager.GetInstance().GetTableItem<GiftPackTable>(tableData.ID);
                                if (GiftPackTable == null)
                                {
                                    return;
                                }

                                var gifts = TableManager.GetInstance().GetGiftTableData(GiftPackTable.ID);

                                for (int j = 0; j < gifts.Count; j++)
                                {
                                    var giftTableData2= gifts[j];
                                    if (giftTableData2 == null)
                                    {
                                        continue;
                                    }
                                    if (!giftTableData2.RecommendJobs.Contains(PlayerBaseData.GetInstance().JobTableID))
                                    {
                                        continue;
                                    }
                                    ItemTable tableData2 = TableManager.GetInstance().GetTableItem<ItemTable>(giftTableData2.ItemID);
                                    if (tableData2.SubType == ItemTable.eSubType.FASHION_HAIR)//添加礼包里面翅膀的预览
                                    {
                                        PlayerBaseData.GetInstance().AvatarEquipWing(mGeAvatarRendererEx, tableData2.ID);
                                    }
                                }
                            }
                            else
                            {
                                EFashionWearSlotType slotType = GetEquipSlotType(tableData);
                                PlayerBaseData.GetInstance().AvatarEquipPart(mGeAvatarRendererEx, slotType, tableData.ID);
                            }
                         
                        }
                    }
                    mGeAvatarRendererEx.ChangeAction("Anim_Show_Idle", 1f, true);
                    if (mShowItemNameId != null&&mShowItemNameId.Contains(itemTable.ID))
                    {
                        SetName(itemTable.Name, itemTable.Color);
                    }
                    else
                    {
                        var mallLimitTimeActivity = TableManager.GetInstance().GetTableItem<MallLimitTimeActivity>(preViewItem.activityId);
                        if (mallLimitTimeActivity != null)
                        {
                            SetName(mallLimitTimeActivity.FashionName, itemTable.Color);
                        }
                        else
                        {
                            SetName(itemTable.Name, itemTable.Color);
                        }
                    }
                   
                }
                else if (itemTable.SubType == ItemTable.eSubType.TITLE)
                {
                    InitAvatar();
                    SetNameTitleImage(itemTable);
                    SetName(itemTable.Name, itemTable.Color);
                }
                else if (itemTable.SubType == ItemTable.eSubType.PetEgg)
                {
                    int mPetId = GetPetID(itemTable);

                    PlayerUtility.LoadPetAvatarRenderEx(mPetId, mGeAvatarRendererEx);

                    PetTable petTable = TableManager.GetInstance().GetTableItem<PetTable>(mPetId);
                    if (petTable == null)
                    {
                        Logger.LogErrorFormat("can not find PetTable with id:{0}", mPetId);
                    }
                    else
                    {
                        SetName(petTable.Name, (ItemTable.eColor)petTable.Quality);
                    }
                }
                else
                {
                    InitAvatar();

                    RefreshAvatar(itemTable.ID);

                    SetName(itemTable.Name, itemTable.Color);
                }
            }
        }

        private void RefreshAvatar(int id)
        {
            ItemTable itemTable = TableManager.GetInstance().GetTableItem<ItemTable>(id);
            if (itemTable == null)
            {
                Logger.LogErrorFormat("can not find ItemTable with id:{0}", id);
            }
            else
            {
              
                if (itemTable.SubType == ItemTable.eSubType.FASHION_HAIR)
                {
                    PlayerBaseData.GetInstance().AvatarEquipWing(mGeAvatarRendererEx, itemTable.ID);
                }
                else if (itemTable.SubType == ItemTable.eSubType.FASHION_AURAS)
                {
                    PlayerBaseData.GetInstance().AvatarEquipHalo(mGeAvatarRendererEx, itemTable.ID);
                }
                else if (itemTable.SubType == ItemTable.eSubType.WEAPON)
                {
                    PlayerBaseData.GetInstance().AvatarEquipWeapon(mGeAvatarRendererEx, PlayerBaseData.GetInstance().JobTableID, itemTable.ID);
                }
                else if (itemTable.SubType == ItemTable.eSubType.FASHION_WEAPON)
                {
                    PlayerBaseData.GetInstance().AvatarEquipWeapon(mGeAvatarRendererEx, PlayerBaseData.GetInstance().JobTableID, itemTable.ID);
                }
                else if (itemTable.SubType == ItemTable.eSubType.PetEgg)
                {
                    int mPetId = GetPetID(itemTable);

                    PlayerUtility.LoadPetAvatarRenderEx(mPetId, mGeAvatarRendererEx);
                    
                    PetTable petTable = TableManager.GetInstance().GetTableItem<PetTable>(mPetId);
                    if (petTable == null)
                    {
                        Logger.LogErrorFormat("can not find PetTable with id:{0}", mPetId);
                    }
                    else
                    {
                        SetName(petTable.Name, (ItemTable.eColor)petTable.Quality);
                    }
                }
                else
                {
                    EFashionWearSlotType slotType = GetEquipSlotType(itemTable);
                    PlayerBaseData.GetInstance().AvatarEquipPart(mGeAvatarRendererEx, slotType, itemTable.ID);
                }

                mGeAvatarRendererEx.ChangeAction("Anim_Show_Idle", 1f, true);
            }
        }

        private void InitAvatar(bool isTakeOffWearFashion = false)
        {
            int jobID = PlayerBaseData.GetInstance().JobTableID;

            JobTable jobTable = TableManager.GetInstance().GetTableItem<JobTable>(jobID);
            if (jobTable == null)
            {
                Logger.LogErrorFormat("can not find JobTable with id:{0}", jobID);
            }
            else
            {
                ResTable resTable = TableManager.GetInstance().GetTableItem<ResTable>(jobTable.Mode);
                if (resTable == null)
                {
                    Logger.LogErrorFormat("can not find ResTable with id:{0}", jobTable.Mode);
                }
                else
                {
                    mGeAvatarRendererEx.ClearAvatar();
                    mGeAvatarRendererEx.LoadAvatar(resTable.ModelPath);

                    if (jobID == PlayerBaseData.GetInstance().JobTableID)
                    {
                        if (isTakeOffWearFashion)
                        {
                            PlayerBaseData.GetInstance().AvatarEquipFromPreviewCompleteFashion(mGeAvatarRendererEx);
                        }
                        else
                        {
                            PlayerBaseData.GetInstance().AvatarEquipFromCurrentEquiped(mGeAvatarRendererEx);
                        }
                    }

                    mGeAvatarRendererEx.AttachAvatar("Aureole", "Effects/Scene_effects/Effectui/EffUI_chuangjue_fazhen_JS", "[actor]Orign", false);
                    mGeAvatarRendererEx.SuitAvatar();
                }
            }
        }

        private void UpdateViewShowInfo()
        {
            if (mPreViewDataModel.preViewItemList.Count > 1 && mCurrentSelectIndex == 0)
            {
                mLeftArrowBtn.gameObject.CustomActive(false);
                mRightArrowBtn.gameObject.CustomActive(true);
            }
            else if (mPreViewDataModel.preViewItemList.Count > 1 && mCurrentSelectIndex == mPreViewDataModel.preViewItemList.Count - 1)
            {
                mLeftArrowBtn.gameObject.CustomActive(true);
                mRightArrowBtn.gameObject.CustomActive(false);
            }
            else if (mPreViewDataModel.preViewItemList.Count < 2)
            {
                mLeftArrowBtn.gameObject.CustomActive(false);
                mRightArrowBtn.gameObject.CustomActive(false);
            }
            else
            {
                mLeftArrowBtn.gameObject.CustomActive(true);
                mRightArrowBtn.gameObject.CustomActive(true);
            }
        }

        private EFashionWearSlotType GetEquipSlotType(ItemTable ItemTableData)
        {
            if (ItemTableData.SubType == ItemTable.eSubType.FASHION_HEAD)
            {
                return EFashionWearSlotType.Head; // 头饰
            }
            else if (ItemTableData.SubType == ItemTable.eSubType.FASHION_CHEST)
            {
                return EFashionWearSlotType.UpperBody; // 上装
            }
            else if (ItemTableData.SubType == ItemTable.eSubType.FASHION_EPAULET)
            {
                return EFashionWearSlotType.Chest; // 胸饰
            }
            else if (ItemTableData.SubType == ItemTable.eSubType.FASHION_LEG)
            {
                return EFashionWearSlotType.LowerBody; // 下装
            }
            else if (ItemTableData.SubType == ItemTable.eSubType.FASHION_SASH)
            {
                return EFashionWearSlotType.Waist; // 腰饰
            }
            else
            {
                return EFashionWearSlotType.Invalid;
            }
        }

        private int GetPetID(ItemTable itemTable)
        {
            int mPetId = 0;
            var mPetDic = TableManager.GetInstance().GetTable<PetTable>().GetEnumerator();
            while (mPetDic.MoveNext())
            {
                var mPetTable = mPetDic.Current.Value as PetTable;
                if (mPetTable.PetEggID != itemTable.ID)
                {
                    continue;
                }

                mPetId = mPetTable.ID;
                return mPetId;
            }

            return mPetId;
        }

        private void SetName(string name, ItemTable.eColor color)
        {
            if (mName != null)
            {
                mName.text = PetDataManager.GetInstance().GetColorName(name, (PetTable.eQuality)color);
            }
        }

        private void SetNameTitleImage(ItemTable itemTable)
        {
            if (itemTable != null && itemTable.Path2.Count == 4)
            {
                if (mSpriteAniRenderChenghao != null)
                {
                    mSpriteAniRenderChenghao.gameObject.CustomActive(true);
                    mSpriteAniRenderChenghao.Reset(itemTable.Path2[0], itemTable.Path2[1], int.Parse(itemTable.Path2[2]), float.Parse(itemTable.Path2[3]), itemTable.ModelPath);
                    if (mAniImage != null)
                    {
                        mAniImage.enabled = true;
                    }
                }
            }
        }

        public void Dispose()
        {
            UnInitPreViewItemList();
            mPreViewDataModel = null;
            mCurrentSelectIndex = 0;
            isInitialize = false;

            if (mCloseBtn != null)
            {
                mCloseBtn.onClick.RemoveAllListeners();
            }

            if (mLeftArrowBtn != null)
            {
                mLeftArrowBtn.onClick.RemoveAllListeners();
            }

            if (mRightArrowBtn != null)
            {
                mRightArrowBtn.onClick.RemoveAllListeners();
            }
        }

        private void OnDestroy()
        {
            Dispose();
        }
    }
}

