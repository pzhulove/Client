using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using Protocol;
using ProtoTable;

namespace GameClient
{
    class EnchantmentCardUpgradeView : MonoBehaviour
    {
        [SerializeField]private ComUIListScript mEnchantmentCardUIList;
        [SerializeField]private ComUIListScript mEnchantmentViceCardUIList;
        [SerializeField]private GameObject mItemParent;
        [SerializeField]private GameObject mGoldCoinItemParent;
        [SerializeField]private GameObject mGoConsumGoldCoinLink;
        [SerializeField]private Image mEnchantmentCardBg;
        [SerializeField]private Image mEnchantmentCardIcon;
        [SerializeField]private Button mEnchantmentCardBtn;
        [SerializeField]private Button mBtnConsumGoldCoinLink;
        [SerializeField]private Text mEnchantmentCardName;
        [SerializeField]private Text mConsumGoldCoinNum;
        [SerializeField]private Text mEnchantmentCardLevel;
        [SerializeField]private Text mCurrentArrt;
        [SerializeField]private Text mNextLevelArrt;
        [SerializeField]private StateController mLevelIsFullStateControl;//等级是否满级的状态控制器
        [SerializeField]private StateController mIsOnlyUseSameCardStateContrl;//是否只能使用同名卡的状态控制器
        [Header("ViceCard")]
        [SerializeField]private Text mSuccessRateDesc;
        [SerializeField]private Text mSelectedExpendNumberDesc;
        [SerializeField]private Button mUpgradeEnchantmentCardBtn;
        [SerializeField]private List<EnchantmentCardExpendItem> mEnchantmentCardExpendItemList = new List<EnchantmentCardExpendItem>();
        [SerializeField]private ComDropDownControl mEnchantmentCardQulityDrop;
        [SerializeField]private ComDropDownControl mEnchantmentCardStageDrop;
        [SerializeField]private string sUnSelected = "UnSelected"; //未选择副卡
        [SerializeField]private string sSelected = "Selected"; //选择了副卡
        [SerializeField]private string sNoFullLevel = "NoFullLevel";//未满级
        [SerializeField]private string sFullLevel= "FullLevel";//满级
        [SerializeField]private string sNoEnchantmentCard= "NoEnchantmentCard";//无附魔卡
        [SerializeField]private string sLevelDesc = "+{0}";
        [SerializeField]private int iGoldCoinID = 600000001; //金币ID
        [SerializeField]private int iBindGoldCoinID = 600000007;//绑定金币ID
        [SerializeField]private string sSuccessRateDesc = "成功率:{0}";
        [SerializeField]private int iMaxExpendCount;
        private bool bIsDefaultSelectCard = true;
        private bool bIsRefreshViceCardInfo = false;
        /// <summary>
        /// 所有可升级的附魔卡
        /// </summary>
        private List<EnchantmentCardItemDataModel> mEnchantmentCardItems = new List<EnchantmentCardItemDataModel>();
        private EnchantmentCardItemDataModel mCurrentSelectEnchantmentCardItem; //选择的要升级的附魔卡
        private EnchantmentCardViceCardData mCureentSelectEnchantmentCardViceCardItem; // 选择消耗附魔卡（副卡）
        /// <summary>
        /// 用于显示绑定状态描述
        /// </summary>
        private List<ItemData> mViceCardItemDataList = new List<ItemData>();
        /// <summary>
        /// 选择附魔卡副卡的集合
        /// </summary>
        private List<EnchantmentCardViceCardData> mCureentSelectEnchantmentCardViceCardItemList = new List<EnchantmentCardViceCardData>();
        /// <summary>
        /// 所有副卡
        /// </summary>
        private List<EnchantmentCardViceCardData> mEnchantmentCardViceCardDataList = new List<EnchantmentCardViceCardData>();
        private ComItemNew mConsumGoldCoinItem;//展示金币
        private int iCurrentGoldCoinID = 0;//当前消耗的金币ID
        private int iDefaultEnchantmentCardQuality = 0;//默认品质
        private int iDefaultEnchantmentCardStage = 0;//默认阶段
        /// <summary>
        /// 附魔卡品质列表
        /// </summary>
        private List<ComControlData> mEnchantmentCardQulityTabDataList = new List<ComControlData>();
        /// <summary>
        /// 附魔卡阶段列表
        /// </summary>
        private List<ComControlData> mEnchantmentCardStageTabDataList = new List<ComControlData>();
        private void Awake()
        {
            InitEnchantmentCardUIList();
            InitEnchantmentViceCardUIList();
            UpdateGoldCoinItem(iBindGoldCoinID);

            if (mBtnConsumGoldCoinLink != null)
            {
                mBtnConsumGoldCoinLink.onClick.RemoveAllListeners();
                mBtnConsumGoldCoinLink.onClick.AddListener(() =>
                {
                    ItemComeLink.OnLink(iCurrentGoldCoinID, 0);
                });
            }
            
            if (mUpgradeEnchantmentCardBtn != null)
            {
                mUpgradeEnchantmentCardBtn.onClick.RemoveAllListeners();
                mUpgradeEnchantmentCardBtn.onClick.AddListener(()=> 
                {
                    if (mUpgradeEnchantmentCardBtn != null)
                    {
                        mUpgradeEnchantmentCardBtn.enabled = false;
                    }

                    InvokeMethod.Invoke(0.5f, () => 
                    {
                        if (mUpgradeEnchantmentCardBtn != null)
                        {
                            mUpgradeEnchantmentCardBtn.enabled = true;
                        }
                    });

                    OnUpgradeEnchantmentCardClick();
                });
            }

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnEnchantmentCardUpgradeRetun, OnEnchantmentCardUpgradeRetun);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnRemoveExpendEnchantmentCard, OnRemoveExpendEnchantmentCard);
            RegisterDelegateHandler();
            PlayerBaseData.GetInstance().onMoneyChanged += OnMoneyChanged;
        }

        private void OnDestroy()
        {
            UnInitEnchantmentCardUIList();
            UnInitEnchantmentViceCardUIList();
            mEnchantmentCardItems.Clear();
            mConsumGoldCoinItem = null;
            iCurrentGoldCoinID = 0;
            bIsRefreshViceCardInfo = false;

            if (mEnchantmentCardQulityTabDataList != null)
                mEnchantmentCardQulityTabDataList.Clear();

            if (mEnchantmentCardStageTabDataList != null)
                mEnchantmentCardStageTabDataList.Clear();

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnEnchantmentCardUpgradeRetun, OnEnchantmentCardUpgradeRetun);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnRemoveExpendEnchantmentCard, OnRemoveExpendEnchantmentCard);
            UnRegisterDelegateHandler();
            PlayerBaseData.GetInstance().onMoneyChanged -= OnMoneyChanged;
        }

        private void ClearSelectEnchantmentCardViceCardItemList()
        {
            if (mCureentSelectEnchantmentCardViceCardItemList != null)
                mCureentSelectEnchantmentCardViceCardItemList.Clear();
        }

        private void RegisterDelegateHandler()
        {
            ItemDataManager.GetInstance().onAddNewItem += OnAddNewItem;
        }

        private void UnRegisterDelegateHandler()
        {
            ItemDataManager.GetInstance().onAddNewItem -= OnAddNewItem;
        }

        private void OnAddNewItem(List<Item> items)
        {
            for (int i = 0; i < items.Count; i++)
            {
                ItemData data = ItemDataManager.GetInstance().CreateItemDataFromNet(items[i]);
                if (data.SubType != (int)ItemTable.eSubType.EnchantmentsCard)
                {
                    continue;
                }

                LoadAllEnchantmentCardItems(false);
                break;
            }
        }

        private void OnMoneyChanged(PlayerBaseData.MoneyBinderType eMoneyBinderType)
        {
            if(eMoneyBinderType == PlayerBaseData.MoneyBinderType.MBT_BIND_GOLD||
                eMoneyBinderType == PlayerBaseData.MoneyBinderType.MBT_GOLD)
            {
                UpdateGoldCoinItemInfo(mCurrentSelectEnchantmentCardItem);
            }
        }
        
        public void InitView()
        {
            mEnchantmentCardQulityTabDataList = EnchantmentsCardManager.GetInstance().GetEnchantmentCardQualityTabDataList();
            mEnchantmentCardStageTabDataList = EnchantmentsCardManager.GetInstance().GetEnchantmentCardStageTabDataList();

            InitEnchantmentCardQulityDrop();
            InitEnchantmentCardStageDrop();

            LoadAllEnchantmentCardItems(true);
        }

        private void InitEnchantmentCardQulityDrop()
        {
            if (mEnchantmentCardQulityTabDataList != null && mEnchantmentCardQulityTabDataList.Count > 0)
            {
                var emchamtentCardQulityTabData = mEnchantmentCardQulityTabDataList[0];
                for (int i = 0; i < mEnchantmentCardQulityTabDataList.Count; i++)
                {
                    if (iDefaultEnchantmentCardQuality == mEnchantmentCardQulityTabDataList[i].Id)
                    {
                        emchamtentCardQulityTabData = mEnchantmentCardQulityTabDataList[i];
                        break;
                    }
                }

                if (mEnchantmentCardQulityDrop != null)
                {
                    mEnchantmentCardQulityDrop.InitComDropDownControl(emchamtentCardQulityTabData, mEnchantmentCardQulityTabDataList, OnEnchantmentCardQulityDropDownItemClicked);
                }
            }
        }

        private void OnEnchantmentCardQulityDropDownItemClicked(ComControlData comControlData)
        {
            if (comControlData == null)
                return;

            //品质相同，直接返回
            if (iDefaultEnchantmentCardQuality == comControlData.Id)
                return;

            //赋值选中的品质
            iDefaultEnchantmentCardQuality = comControlData.Id;

            //根据选中的品质进行更新 
            LoadAllEnchantmentCardItems(true);
        }

        private void InitEnchantmentCardStageDrop()
        {
            if (mEnchantmentCardStageTabDataList != null && mEnchantmentCardStageTabDataList.Count > 0)
            {
                var emchamtentCardStageTabData = mEnchantmentCardStageTabDataList[0];
                for (int i = 0; i < mEnchantmentCardStageTabDataList.Count; i++)
                {
                    if (iDefaultEnchantmentCardStage == mEnchantmentCardStageTabDataList[i].Id)
                    {
                        emchamtentCardStageTabData = mEnchantmentCardStageTabDataList[i];
                        break;
                    }
                }

                if (mEnchantmentCardStageDrop != null)
                {
                    mEnchantmentCardStageDrop.InitComDropDownControl(emchamtentCardStageTabData, mEnchantmentCardStageTabDataList, OnEnchantmentCardStageDropDownItemClicked);
                }
            }
        }

        private void OnEnchantmentCardStageDropDownItemClicked(ComControlData comControlData)
        {
            if (comControlData == null)
                return;

            //品质相同，直接返回
            if (iDefaultEnchantmentCardStage == comControlData.Id)
                return;

            //赋值选中的品质
            iDefaultEnchantmentCardStage = comControlData.Id;

            //根据选中的品质进行更新 
            LoadAllEnchantmentCardItems(true);
        }

        private void RefreshSelectedExpendNumberDesc()
        {
            if(mSelectedExpendNumberDesc != null)
            {
                mSelectedExpendNumberDesc.text = TR.Value("enchantmentCard_SelectedExpendNumberDesc", mCureentSelectEnchantmentCardViceCardItemList.Count, iMaxExpendCount);
            }
        }
        
        private void OnEnchantmentCardUpgradeRetun(UIEvent uiEvent)
        {
            var retunData = uiEvent.Param1 as EnchantmentCardUpgradeSuccessData;

            if (retunData != null)
            {
                if (mCurrentSelectEnchantmentCardItem != null)
                {
                    if (mCurrentSelectEnchantmentCardItem.mUpgradePrecType == UpgradePrecType.Mounted)
                    {
                        ItemData mEquipItem = ItemDataManager.GetInstance().GetItem(retunData.mEquipGUID);
                        if (mEquipItem != null)
                        {
                            mCurrentSelectEnchantmentCardItem.mEquipItemData = mEquipItem;
                        }
                    }
                    else
                    {
                        ItemData mEnchantmentCardItem = ItemDataManager.GetInstance().GetItem(retunData.mEnchantmentCardGUID);
                        if (mEnchantmentCardItem != null)
                        {
                            mCurrentSelectEnchantmentCardItem.mEnchantmentCardItemData = mEnchantmentCardItem;
                        }
                    }
                
                }
               
                if (mEnchantmentCardUIList != null)
                {
                    mEnchantmentCardUIList.ResetSelectedElementIndex();
                }

                LoadAllEnchantmentCardItems(true);
            }
        }
        
        private void OnRemoveExpendEnchantmentCard(UIEvent uiEvent)
        {
            if (uiEvent == null)
            {
                return;
            }

            int magicId = (int)uiEvent.Param1;

            for (int i = 0; i < mCureentSelectEnchantmentCardViceCardItemList.Count; i++)
            {
                var data = mCureentSelectEnchantmentCardViceCardItemList[i];
                if (data == null)
                {
                    continue;
                }

                ItemData magicCardItemData = data.mViceCardItemData;
                if (magicCardItemData == null)
                    continue;

                
                if (magicCardItemData.TableID != magicId)
                {
                    continue;
                }

                //找到从列表删除
                mCureentSelectEnchantmentCardViceCardItemList.RemoveAt(i);
                break;
            }

            RefreshEnchantmentExpendCardSlot();
            RefreshSelectedExpendNumberDesc();
            RefreshSuccessRateDesc();
            OnSetElementAmount(mEnchantmentCardViceCardDataList);
        }

        /// <summary>
        /// 检查主卡（副卡）的绑定类型
        /// </summary>
        private bool CheckEnchantmentCardBindType()
        {
            if (mCurrentSelectEnchantmentCardItem == null ||
                mCurrentSelectEnchantmentCardItem.mEnchantmentCardItemData == null ||
                mCureentSelectEnchantmentCardViceCardItemList == null ||
                mCureentSelectEnchantmentCardViceCardItemList.Count <= 0
                )
            {
                return false;
            }
            
            if (mCurrentSelectEnchantmentCardItem.mEnchantmentCardItemData.BindAttr == ItemTable.eOwner.ROLEBIND ||
                mCurrentSelectEnchantmentCardItem.mUpgradePrecType == UpgradePrecType.Mounted)
            {
                return false;
            }

            if (mViceCardItemDataList != null)
                mViceCardItemDataList.Clear();

            for (int i = 0; i < mCureentSelectEnchantmentCardViceCardItemList.Count; i++)
            {
                var viceCardData = mCureentSelectEnchantmentCardViceCardItemList[i];
                if (viceCardData == null)
                    continue;

                var viceCardItemData = viceCardData.mViceCardItemData;
                if (viceCardItemData == null)
                    continue;

                if (mCurrentSelectEnchantmentCardItem.mEnchantmentCardItemData.BindAttr != viceCardItemData.BindAttr)
                {
                    mViceCardItemDataList.Add(viceCardItemData);
                }
                else
                {
                    if (mCurrentSelectEnchantmentCardItem.mEnchantmentCardItemData.BindAttr == ItemTable.eOwner.ACCBIND && 
                        viceCardItemData.BindAttr == ItemTable.eOwner.ACCBIND)

                    {
                        mViceCardItemDataList.Add(viceCardItemData);
                    }
                }
            }

            if (mViceCardItemDataList.Count > 0)
                return true;
            
            return false;
        }

        private string GetViceCardBindDesc()
        {
            var kStringBuilder = StringBuilderCache.Acquire();
            for (int i = 0; i < mViceCardItemDataList.Count; i++)
            {
                var viceItem = mViceCardItemDataList[i];
                if (viceItem == null)
                    continue;

                kStringBuilder.Append(GetBindDesc(viceItem.BindAttr));
                if (i != mViceCardItemDataList.Count - 1)
                {
                    kStringBuilder.Append("、");
                }
            }

            kStringBuilder.Append("</color>");
            string ret = kStringBuilder.ToString();
            StringBuilderCache.Release(kStringBuilder);

            return ret;
        }

        /// <summary>
        /// 得到主卡（副卡）绑定描述
        /// </summary>
        /// <returns></returns>
        private string GetBindDesc(ItemTable.eOwner ower)
        {
            string bindDesc = "";
            switch (ower)
            {
                case ItemTable.eOwner.Owner_None:
                case ItemTable.eOwner.NOTBIND:
                    bindDesc = "非绑定";
                    break;
                case ItemTable.eOwner.ROLEBIND:
                    bindDesc = "角色绑定";
                    break;
                case ItemTable.eOwner.ACCBIND:
                    bindDesc = "账号绑定";
                    break;
                default:
                    break;
            }

            return bindDesc;
        }
        
        private void RefreshEnchantmentExpendCardSlot()
        {
            for (int i = 0; i < mEnchantmentCardExpendItemList.Count; i++)
            {
                EnchantmentCardExpendItem expentItem = mEnchantmentCardExpendItemList[i];
                if (expentItem == null)
                    continue;

                if (i < mCureentSelectEnchantmentCardViceCardItemList.Count)
                {
                    EnchantmentCardViceCardData viceCardData = mCureentSelectEnchantmentCardViceCardItemList[i];
                    if(viceCardData != null && viceCardData.mViceCardItemData != null)
                    {
                        expentItem.SetItem(viceCardData.mViceCardItemData);
                    }
                    else
                    {
                        expentItem.SetItem(null);
                    }
                }
                else
                {
                    expentItem.SetItem(null);
                }
            }
        }

        private void UpdateViceCardItemInfo(EnchantmentCardViceCardItem viceCardItem)
        {
            if (viceCardItem == null)
            {
                return;
            }

            if (viceCardItem.ViceCardData == null)
            {
                return;
            }
               
            EnchantmentCardViceCardData viceCardData = viceCardItem.ViceCardData;

            //放入的附魔卡数量大于等于最大材料数量
            if (mCureentSelectEnchantmentCardViceCardItemList.Count >= iMaxExpendCount)
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("enchantmentCard_MaxCountDesc", iMaxExpendCount));
                return;
            }

            int allCount = viceCardData.mViceCardCount;
            int count = 0;
            for (int i = 0; i < mCureentSelectEnchantmentCardViceCardItemList.Count; i++)
            {
                var item = mCureentSelectEnchantmentCardViceCardItemList[i];
                if (item == null)
                {
                    continue;
                }

                ItemData magicCardItemData = item.mViceCardItemData;
                if (magicCardItemData == null)
                    continue;

                if (magicCardItemData.TableID == viceCardData.mViceCardItemData.TableID)
                {
                    count++;
                }
            }

            //如果同样的附魔卡已全部放进去
            if (count >= allCount)
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("enchantmentCard_CostDesc"));
                return;
            }

            if (mCureentSelectEnchantmentCardViceCardItemList != null)
                mCureentSelectEnchantmentCardViceCardItemList.Add(viceCardData);

            if (viceCardItem != null)
                viceCardItem.OnItemChangeDisplay(true);

            RefreshEnchantmentExpendCardSlot();
            RefreshSelectedExpendNumberDesc();
            RefreshSuccessRateDesc();
        }

        private void RefreshSuccessRateDesc()
        {
            int iAllSuccessRate = 0;
            for (int i = 0; i < mCureentSelectEnchantmentCardViceCardItemList.Count; i++)
            {
                var item = mCureentSelectEnchantmentCardViceCardItemList[i];
                if (item == null)
                {
                    continue;
                }

                iAllSuccessRate += item.mAllSuccessRate;
            }

            if (mSuccessRateDesc != null)
            {
                if (iAllSuccessRate != 0)
                    mSuccessRateDesc.text = string.Format(sSuccessRateDesc, EnchantmentsCardManager.GetInstance().GetEnchantmentCardProbabilityDesc(iAllSuccessRate));
                else
                    mSuccessRateDesc.text = string.Empty;
            }
        }

        /// <summary>
        /// 更新消耗金币Item
        /// </summary>
        /// <param name="coinID"></param>
        private void UpdateGoldCoinItem(int coinID)
        {
            ItemData goldCoinItem = ItemDataManager.CreateItemDataFromTable(coinID);
            if (goldCoinItem == null)
            {
                return;
            }

            if (mConsumGoldCoinItem == null)
            {
                mConsumGoldCoinItem = ComItemManager.CreateNew(mGoldCoinItemParent);
            }

            if (mConsumGoldCoinItem != null)
            {
                mConsumGoldCoinItem.Setup(goldCoinItem, Utility.OnItemClicked);
            }
        }

        /// <summary>
        /// 更新消耗金币信息
        /// </summary>
        /// <param name="enchatmentCardItemData"></param>
        private void UpdateGoldCoinItemInfo(EnchantmentCardItemDataModel enchatmentCardItemData)
        {
            if (enchatmentCardItemData == null || enchatmentCardItemData.mConsumableMaterialData == null)
            {
                return;
            }

            ItemData goldCoinItem = ItemDataManager.CreateItemDataFromTable(enchatmentCardItemData.mConsumableMaterialData.ItemID);
            if (goldCoinItem == null)
            {
                return;
            }
            
            int iNeedCount = enchatmentCardItemData.mConsumableMaterialData.Count;
            int iHasCount = ItemDataManager.GetInstance().GetOwnedItemCount(goldCoinItem.TableID);
            if (mConsumGoldCoinNum != null)
            {
                mConsumGoldCoinNum.text = iNeedCount.ToString();
                
                if (iHasCount < iNeedCount && iNeedCount > 0)
                {
                    mConsumGoldCoinNum.color = Color.red;
                }
                else
                {
                    mConsumGoldCoinNum.color = Color.white;
                }
            }

            iCurrentGoldCoinID = goldCoinItem.TableID;

            UpdateGoldCoinItem(iCurrentGoldCoinID);

            mGoConsumGoldCoinLink.gameObject.CustomActive(iHasCount < iNeedCount && iNeedCount > 0);
        }

        /// <summary>
        /// 更新附魔卡道具
        /// </summary>
        /// <param name="enchatmentCardItemData"></param>
        private void UpdateEnchantmentCardItem(EnchantmentCardItemDataModel enchatmentCardItemData)
        {
            if (enchatmentCardItemData == null)
            {
                return;
            }

            ItemData itemData = enchatmentCardItemData.mEnchantmentCardItemData;
            mItemParent.CustomActive(true);

            if (mEnchantmentCardBg != null)
            {
                ETCImageLoader.LoadSprite(ref mEnchantmentCardBg, itemData.GetQualityInfo().Background);
            }

            if (mEnchantmentCardIcon != null)
            {
                ETCImageLoader.LoadSprite(ref mEnchantmentCardIcon, itemData.Icon);
            }

            if (mEnchantmentCardBtn != null)
            {
                mEnchantmentCardBtn.onClick.RemoveAllListeners();
                mEnchantmentCardBtn.onClick.AddListener(() => 
                {
                    ItemTipManager.GetInstance().ShowTip(itemData);
                });
            }

            if (mEnchantmentCardName != null)
            {
                mEnchantmentCardName.text = itemData.GetColorName();
            }
            
            if (mEnchantmentCardLevel != null)
            {
                if (itemData.mPrecEnchantmentCard.iEnchantmentCardLevel > 0)
                {
                    mEnchantmentCardLevel.text = string.Format(sLevelDesc, itemData.mPrecEnchantmentCard.iEnchantmentCardLevel);
                }
                else
                {
                    mEnchantmentCardLevel.text = string.Empty;
                }
            }

            if (mLevelIsFullStateControl != null)
            {
                bool levelIsFull = EnchantmentsCardManager.GetInstance().CheckEnchantmentCardLevelIsFull(enchatmentCardItemData);
                if (levelIsFull)
                {
                    mLevelIsFullStateControl.Key = sFullLevel;
                }
                else
                {
                    mLevelIsFullStateControl.Key = sNoFullLevel;
                }
            }

            if (itemData.mPrecEnchantmentCard != null)
            {
                if (mCurrentArrt != null)
                {
                    mCurrentArrt.text = EnchantmentsCardManager.GetInstance().GetEnchantmentCardAttributesDesc((int)itemData.mPrecEnchantmentCard.iEnchantmentCardID, itemData.mPrecEnchantmentCard.iEnchantmentCardLevel);
                }

                if (mNextLevelArrt != null)
                {
                    mNextLevelArrt.text = EnchantmentsCardManager.GetInstance().GetEnchantmentCardAttributesDesc((int)itemData.mPrecEnchantmentCard.iEnchantmentCardID, itemData.mPrecEnchantmentCard.iEnchantmentCardLevel + 1);
                }
            }
                
        }

        #region EnchantmentCardUIList
        private void InitEnchantmentCardUIList()
        {
            if (mEnchantmentCardUIList != null)
            {
                mEnchantmentCardUIList.Initialize();
                mEnchantmentCardUIList.onBindItem += OnBindItemDelegate;
                mEnchantmentCardUIList.onItemVisiable += OnItemVisiableDelegate;
                mEnchantmentCardUIList.onItemSelected += OnItemSelectedDelegate;
                mEnchantmentCardUIList.onItemChageDisplay += OnItemChangeDisplayDelegate;
            }
        }

        private void UnInitEnchantmentCardUIList()
        {
            if (mEnchantmentCardUIList != null)
            {
                mEnchantmentCardUIList.onBindItem -= OnBindItemDelegate;
                mEnchantmentCardUIList.onItemVisiable -= OnItemVisiableDelegate;
                mEnchantmentCardUIList.onItemSelected -= OnItemSelectedDelegate;
                mEnchantmentCardUIList.onItemChageDisplay -= OnItemChangeDisplayDelegate;
            }
        }

        private EnchantmentCardItem OnBindItemDelegate(GameObject itemObject)
        {
            return itemObject.GetComponent<EnchantmentCardItem>();
        }

        private void OnItemVisiableDelegate(ComUIListElementScript item)
        {
            EnchantmentCardItem enchantmentCardItem = item.gameObjectBindScript as EnchantmentCardItem;
            if (enchantmentCardItem != null && item.m_index >= 0 && item.m_index < mEnchantmentCardItems.Count)
            {
                var enchantmentCardData = mEnchantmentCardItems[item.m_index];
                enchantmentCardItem.OnItemVisiable(enchantmentCardData, OnEnchantmentCardItemClick);

                if(mCurrentSelectEnchantmentCardItem != null)
                {
                    if (mCurrentSelectEnchantmentCardItem.mEnchantmentCardItemData.GUID != enchantmentCardData.mEnchantmentCardItemData.GUID)
                        enchantmentCardItem.OnEnchantmentCardItemChageDisplay(false);
                }
            }
        }
        
        private void OnItemSelectedDelegate(ComUIListElementScript item)
        {
            EnchantmentCardItem enchantmentCardItem = item.gameObjectBindScript as EnchantmentCardItem;
            if (enchantmentCardItem != null)
            {
                enchantmentCardItem.OnSelectEnchantmentCardItemClick();
            }
        }

        private void OnItemChangeDisplayDelegate(ComUIListElementScript item, bool bSelected)
        {
            EnchantmentCardItem enchantmentCardItem = item.gameObjectBindScript as EnchantmentCardItem;
            if (enchantmentCardItem != null)
            {
                enchantmentCardItem.OnEnchantmentCardItemChageDisplay(bSelected);
            }
        }

        /// <summary>
        /// 选择附魔卡
        /// </summary>
        /// <param name="enchantmentCardItemDataModel"></param>
        private void OnEnchantmentCardItemClick(EnchantmentCardItemDataModel enchantmentCardItemDataModel)
        {
            if (enchantmentCardItemDataModel == null || enchantmentCardItemDataModel.mEnchantmentCardItemData == null)
            {
                return;
            }
            
            mCurrentSelectEnchantmentCardItem = enchantmentCardItemDataModel;
            iMaxExpendCount = enchantmentCardItemDataModel.mEnchantmentCardCostMaxNum;
            
            ClearSelectEnchantmentCardViceCardItemList();
            RefreshSelectedExpendNumberDesc();
            RefreshEnchantmentExpendCardSlot();
            RefreshSuccessRateDesc();
            //SetIsOnlyUseSameCardStateContrl(mCurrentSelectEnchantmentCardItem);
            UpdateEnchantmentCardItem(mCurrentSelectEnchantmentCardItem);
            UpdateGoldCoinItemInfo(mCurrentSelectEnchantmentCardItem);
            RefreshEnchantmentViceCardItemList();
        }
        
        private void SetIsOnlyUseSameCardStateContrl(EnchantmentCardItemDataModel enchatmentCardItemData)
        {
            if (enchatmentCardItemData == null || enchatmentCardItemData.mEnchantmentCardItemData == null)
            {
                return;
            }

            bool IsOnlyUseSameCard = EnchantmentsCardManager.GetInstance().CheckMainEnchantmentCardIsOnlyUseSameCard(enchatmentCardItemData.mEnchantmentCardItemData);

            if (mIsOnlyUseSameCardStateContrl != null)
            {
                mIsOnlyUseSameCardStateContrl.Key = IsOnlyUseSameCard == true ? "SameNameCard" : "Normal";
            }
        }

        /// <summary>
        /// 加载所有附魔卡
        /// </summary>
        public void LoadAllEnchantmentCardItems(bool isDefaultSelectCard = true)
        {
            bIsDefaultSelectCard = isDefaultSelectCard;

            if (mEnchantmentCardItems != null)
            {
                mEnchantmentCardItems.Clear();
            }
            
            var items = EnchantmentsCardManager.GetInstance().LoadEnchantmentCardItems();

            for (int i = 0; i < items.Count; i++)
            {
                var data = items[i];
                if (data == null)
                    continue;

                if (data.mEnchantmentCardItemData == null)
                    continue;

                if(iDefaultEnchantmentCardQuality != 0)
                {
                    if (data.mEchantmentCardQuality != iDefaultEnchantmentCardQuality)
                        continue;
                }

                if(iDefaultEnchantmentCardStage != 0)
                {
                    if (data.mEnchantmentCardStage != iDefaultEnchantmentCardStage)
                        continue;
                }
                
                mEnchantmentCardItems.Add(data);
            }
            
            if (mEnchantmentCardUIList != null)
            {
                mEnchantmentCardUIList.SetElementAmount(mEnchantmentCardItems.Count);
            }

            if (mEnchantmentCardItems.Count <= 0)
            {
                if (mLevelIsFullStateControl != null)
                {
                    mLevelIsFullStateControl.Key = sNoEnchantmentCard;
                }
            }

            if (bIsDefaultSelectCard == true)
            {
                TryDefultSelectEnchantmentCardItem();
            }
        }

        /// <summary>
        /// 默认选择一张附魔卡
        /// </summary>
        private void TryDefultSelectEnchantmentCardItem()
        {
            int iSelectedIndex = -1;
            bool isFindEnchantmentCard = false;
            
            if (mCurrentSelectEnchantmentCardItem != null && mCurrentSelectEnchantmentCardItem.mEnchantmentCardItemData != null)
            {
                for (int i = 0; i < mEnchantmentCardItems.Count; i++)
                {
                    if (mCurrentSelectEnchantmentCardItem.mUpgradePrecType == UpgradePrecType.Mounted)
                    {
                        if (mCurrentSelectEnchantmentCardItem.mEquipItemData == null)
                        {
                            continue;
                        }

                        if (mEnchantmentCardItems[i].mEquipItemData == null)
                        {
                            continue;
                        }

                        if (/*mEnchantmentCardItems[i].mEnchantmentCardItemData.TableID == mCurrentSelectEnchantmentCardItem.mEnchantmentCardItemData.TableID&&*/
                            mEnchantmentCardItems[i].mEquipItemData.GUID == mCurrentSelectEnchantmentCardItem.mEquipItemData.GUID)
                        {
                            iSelectedIndex = i;
                            isFindEnchantmentCard = true;
                            break;
                        }
                    }
                    else
                    {
                        if (mEnchantmentCardItems[i].mEnchantmentCardItemData.GUID == mCurrentSelectEnchantmentCardItem.mEnchantmentCardItemData.GUID)
                        {
                            iSelectedIndex = i;
                            isFindEnchantmentCard = true;
                            break;
                        }
                    }
                }

                if (isFindEnchantmentCard == false)
                {
                    iSelectedIndex = 0;
                }
            }
            else
            {
                if (mEnchantmentCardItems.Count > 0)
                {
                    iSelectedIndex = 0;
                }
            }
            
            SetSelectedItem(iSelectedIndex);
        }
        
        private void SetSelectedItem(int iBindIndex)
        {
            if (iBindIndex < 0 || mEnchantmentCardItems.Count < 0)
            {
                return;
            }
            
            if (iBindIndex >= 0 && iBindIndex < mEnchantmentCardItems.Count)
            {
                if (mEnchantmentCardUIList != null)
                {
                    if (!mEnchantmentCardUIList.IsElementInScrollArea(iBindIndex))
                    {
                        mEnchantmentCardUIList.EnsureElementVisable(iBindIndex);
                    }

                    mEnchantmentCardUIList.ResetSelectedElementIndex();
                    mEnchantmentCardUIList.SelectElement(iBindIndex);
                }
            }
            else
            {
                mEnchantmentCardUIList.SelectElement(-1);
            }
        }
        #endregion

        #region EnchantmentViceCardUIList
        
        private void InitEnchantmentViceCardUIList()
        {
            if (mEnchantmentViceCardUIList != null)
            {
                mEnchantmentViceCardUIList.Initialize();
                mEnchantmentViceCardUIList.onBindItem += OnEnchantmentViceCardBindItemDelegate;
                mEnchantmentViceCardUIList.onItemVisiable += OnEnchantmentViceCardItemVisiableDelegate;
            }
        }

        private void UnInitEnchantmentViceCardUIList()
        {
            if (mEnchantmentViceCardUIList != null)
            {
                mEnchantmentViceCardUIList.onBindItem -= OnEnchantmentViceCardBindItemDelegate;
                mEnchantmentViceCardUIList.onItemVisiable -= OnEnchantmentViceCardItemVisiableDelegate;
            }
        }

        private EnchantmentCardViceCardItem OnEnchantmentViceCardBindItemDelegate(GameObject itemObject)
        {
            return itemObject.GetComponent<EnchantmentCardViceCardItem>();
        }

        private void OnEnchantmentViceCardItemVisiableDelegate(ComUIListElementScript item)
        {
            EnchantmentCardViceCardItem viceCardItem = item.gameObjectBindScript as EnchantmentCardViceCardItem;
            if (viceCardItem != null && item.m_index >= 0 && item.m_index < mEnchantmentCardViceCardDataList.Count)
            {
                viceCardItem.OnItemVisiable(mEnchantmentCardViceCardDataList[item.m_index], UpdateViceCardItemInfo, mCureentSelectEnchantmentCardViceCardItemList);
            }
        }
        
        private void RefreshEnchantmentViceCardItemList()
        {
            mEnchantmentCardViceCardDataList.Clear();
            mEnchantmentCardViceCardDataList = EnchantmentsCardManager.GetInstance().GetEnchantmentCardViceCardDatas(mCurrentSelectEnchantmentCardItem);
            mEnchantmentCardViceCardDataList.Sort();

            OnSetElementAmount(mEnchantmentCardViceCardDataList);
        }

        private void OnSetElementAmount(List<EnchantmentCardViceCardData> mEnchantmentCardViceCardDataList)
        {
            if (mEnchantmentViceCardUIList != null)
            {
                mEnchantmentViceCardUIList.SetElementAmount(mEnchantmentCardViceCardDataList.Count);
            }
        }
        #endregion

        #region 升级操作
        private void OnUpgradeEnchantmentCardClick()
        {
            if (mCureentSelectEnchantmentCardViceCardItemList == null ||
                mCureentSelectEnchantmentCardViceCardItemList.Count <= 0)
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("enchantmentCard_ExpendDesc"));
                return;
            }

            bool isFull = ItemDataManager.GetInstance().IsPackageFull(EPackageType.Consumable);
            if (isFull == true)
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("enchantmentCard_PackageFullDesc"));
                return;
            }

            bool bIsHightQuality = false;
            for (int i = 0; i < mCureentSelectEnchantmentCardViceCardItemList.Count; i++)
            {
                var viceCardData = mCureentSelectEnchantmentCardViceCardItemList[i];
                if (viceCardData == null)
                    continue;

                var viceCardItemData = viceCardData.mViceCardItemData;
                if (viceCardItemData == null)
                    continue;

                if(viceCardItemData.Quality >= ItemTable.eColor.PINK)
                {
                    bIsHightQuality = true;
                    break;
                }
            }

            if (bIsHightQuality && EnchantmentsCardManager.GetInstance().IsShowQualityTip == false)
            {
                CommonMsgBoxOkCancelNewParamData comconMsgBoxOkCancelParamData = new CommonMsgBoxOkCancelNewParamData();
                comconMsgBoxOkCancelParamData.ContentLabel = TR.Value("enchantmentCard_QualityDesc");
                comconMsgBoxOkCancelParamData.IsShowNotify = true;
                comconMsgBoxOkCancelParamData.OnCommonMsgBoxToggleClick = OnUpdateQualityIsShow;
                comconMsgBoxOkCancelParamData.LeftButtonText = TR.Value("common_data_cancel");
                comconMsgBoxOkCancelParamData.RightButtonText = TR.Value("common_data_sure_2");
                comconMsgBoxOkCancelParamData.OnRightButtonClickCallBack = ExecuteUpgradeLogic;

                OpenComMsgBoxOkCancelFrame(comconMsgBoxOkCancelParamData);
                return;
            }
            else
            {
                bool bIsHightLevel = false;
                for (int i = 0; i < mCureentSelectEnchantmentCardViceCardItemList.Count; i++)
                {
                    var viceCardData = mCureentSelectEnchantmentCardViceCardItemList[i];
                    if (viceCardData == null)
                        continue;

                    var viceCardItemData = viceCardData.mViceCardItemData;
                    if (viceCardItemData == null)
                        continue;

                    if (viceCardItemData.Quality < ItemTable.eColor.PINK &&
                        viceCardItemData.mPrecEnchantmentCard.iEnchantmentCardLevel >= 1)
                    {
                        bIsHightLevel = true;
                        break;
                    }
                }

                if (bIsHightLevel && EnchantmentsCardManager.GetInstance().IsShowLevelTip == false)
                {
                    CommonMsgBoxOkCancelNewParamData comconMsgBoxOkCancelParamData = new CommonMsgBoxOkCancelNewParamData();
                    comconMsgBoxOkCancelParamData.ContentLabel = TR.Value("enchantmentCard_LevelDesc");
                    comconMsgBoxOkCancelParamData.IsShowNotify = true;
                    comconMsgBoxOkCancelParamData.OnCommonMsgBoxToggleClick = OnUpdateLevelIsShow;
                    comconMsgBoxOkCancelParamData.LeftButtonText = TR.Value("common_data_cancel");
                    comconMsgBoxOkCancelParamData.RightButtonText = TR.Value("common_data_sure_2");
                    comconMsgBoxOkCancelParamData.OnRightButtonClickCallBack = ExecuteUpgradeLogic;

                    OpenComMsgBoxOkCancelFrame(comconMsgBoxOkCancelParamData);
                    return;
                }
            }

            ExecuteUpgradeLogic();
        }

        /// <summary>
        /// 执行升级逻辑
        /// </summary>
        private void ExecuteUpgradeLogic()
        {
            if (mCurrentSelectEnchantmentCardItem.mConsumableMaterialData == null)
            {
                if (mCurrentSelectEnchantmentCardItem.mEquipItemData != null && mCurrentSelectEnchantmentCardItem.mEquipItemData.mPrecEnchantmentCard != null)
                {
                    Logger.LogErrorFormat("EnchantmentCardUpgradeView [ExecuteUpgradeLogic] mCurrentSelectEnchantmentCardItem.mConsumableMaterialData = null, EnchantmentCardId = {0} EnchantmentCardLevel = {1}", mCurrentSelectEnchantmentCardItem.mEquipItemData.mPrecEnchantmentCard.iEnchantmentCardID, mCurrentSelectEnchantmentCardItem.mEquipItemData.mPrecEnchantmentCard.iEnchantmentCardLevel);
                }
                return;
            }

            //需要消耗的金币数量
            int iNeedCount = mCurrentSelectEnchantmentCardItem.mConsumableMaterialData.Count;
            //总金币数量
            int iHasCount = ItemDataManager.GetInstance().GetOwnedItemCount(mCurrentSelectEnchantmentCardItem.mConsumableMaterialData.ItemID);
            if (iHasCount < iNeedCount)
            {
                SystemNotifyManager.SysNotifyTextAnimation("绑定金币不足");
                return;
            }

            int goldCoinCount = ItemDataManager.GetInstance().GetOwnedItemCount(iGoldCoinID, false);
            int bindGoldCoinCount = ItemDataManager.GetInstance().GetOwnedItemCount(iBindGoldCoinID, false);

            //如果选择了本次不再显示金币提示界面
            if (EnchantmentsCardManager.GetInstance().IsNotShowGoldCoinTip == true || bindGoldCoinCount >= iNeedCount)
            {
                OnEnchantmentCardUpgradeCheck();
            }
            else
            {
                CommonMsgBoxOkCancelNewParamData comconMsgBoxOkCancelParamData = new CommonMsgBoxOkCancelNewParamData();
                comconMsgBoxOkCancelParamData.ContentLabel = TR.Value("enchantmentCard_goldCoinDesc", bindGoldCoinCount, iNeedCount - bindGoldCoinCount);
                comconMsgBoxOkCancelParamData.IsShowNotify = true;
                comconMsgBoxOkCancelParamData.OnCommonMsgBoxToggleClick = OnUpdateGoldCoinTipIsShow;
                comconMsgBoxOkCancelParamData.LeftButtonText = TR.Value("common_data_cancel");
                comconMsgBoxOkCancelParamData.RightButtonText = TR.Value("common_data_sure_2");
                comconMsgBoxOkCancelParamData.OnRightButtonClickCallBack = OnEnchantmentCardUpgradeCheck;

                OpenComMsgBoxOkCancelFrame(comconMsgBoxOkCancelParamData);
            }
        }

        //请求附魔卡升级前的检查
        private void OnEnchantmentCardUpgradeCheck()
        {
            if (CheckEnchantmentCardBindType() && EnchantmentsCardManager.GetInstance().IsShowBindTip == false)
            {
                CommonMsgBoxOkCancelNewParamData comconMsgBoxOkCancelParamData = new CommonMsgBoxOkCancelNewParamData();
                comconMsgBoxOkCancelParamData.ContentLabel = TR.Value("enchantmentCard_UpgradeDesc", GetViceCardBindDesc(), GetBindDesc(mCurrentSelectEnchantmentCardItem.mEnchantmentCardItemData.BindAttr));
                comconMsgBoxOkCancelParamData.IsShowNotify = true;
                comconMsgBoxOkCancelParamData.OnCommonMsgBoxToggleClick = OnUpdateBindTipIsShow;
                comconMsgBoxOkCancelParamData.LeftButtonText = TR.Value("common_data_cancel");
                comconMsgBoxOkCancelParamData.RightButtonText = TR.Value("common_data_sure_2");
                comconMsgBoxOkCancelParamData.OnRightButtonClickCallBack = OnSceneMagicCardUpgradeReq;

                OpenComMsgBoxOkCancelFrame(comconMsgBoxOkCancelParamData);
            }
            else
            {
                OnSceneMagicCardUpgradeReq();
            }
        }

        /// <summary>
        /// 升级请求
        /// </summary>
        private void OnSceneMagicCardUpgradeReq()
        {
            EnchantmentsCardManager.GetInstance().OnSceneMagicCardUpgradeReq(mCurrentSelectEnchantmentCardItem, mCureentSelectEnchantmentCardViceCardItemList);
        }

        private void OnUpdateGoldCoinTipIsShow(bool value)
        {
            EnchantmentsCardManager.GetInstance().IsNotShowGoldCoinTip = value;
        }

        private void OnUpdateBindTipIsShow(bool value)
        {
            EnchantmentsCardManager.GetInstance().IsShowBindTip = value;
        }

        private void OnUpdateQualityIsShow(bool value)
        {
            EnchantmentsCardManager.GetInstance().IsShowQualityTip = value;
        }

        private void OnUpdateLevelIsShow(bool value)
        {
            EnchantmentsCardManager.GetInstance().IsShowLevelTip = value;
        }

        private void OpenComMsgBoxOkCancelFrame(CommonMsgBoxOkCancelNewParamData paramData)
        {
            if (ClientSystemManager.GetInstance().IsFrameOpen<AuctionNewMsgBoxOkCancelFrame>() == true)
            {
                ClientSystemManager.GetInstance().CloseFrame<AuctionNewMsgBoxOkCancelFrame>();
            }

            ClientSystemManager.GetInstance().OpenFrame<AuctionNewMsgBoxOkCancelFrame>(FrameLayer.High, paramData);
        }
        #endregion
    }
}
