using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using ProtoTable;
using System;

namespace GameClient
{
    public class MagicCardMergeView : MonoBehaviour
    {
        [SerializeField] private ComUIListScript mMagicCardUIListScript;
        [SerializeField] private ComUIListScript mLowProbabilityUIListScript;
        [SerializeField] private ComUIListScript mHighProbabilityUIListScript;
        [SerializeField] private MergeCardItem mMergeCardItemA;
        [SerializeField] private MergeCardItem mMergeCardItemB;
        [SerializeField] private ComMergeMoneyControl mComMergeMoneyControl;
        [SerializeField] private Button mBtnMergeCard;
        [SerializeField] private Button mBtnOneKeyMergeCard;
        [SerializeField] private GameObject mMergePreviewRoot;
        [SerializeField] private GameObject mMergeDesc;
        [SerializeField] private GameObject mLowRoot;
        [SerializeField] private GameObject mHighRoot;
        [SerializeField] private ComDropDownControl mEnchantmentCardQulityDrop;
        [SerializeField] private ComDropDownControl mEnchantmentCardStageDrop;
        [SerializeField] private int iMoneyID = 600000007;

        private EnchantmentsFunctionData dataMerge = new EnchantmentsFunctionData();
        private List<ItemData> mAllMagicCardItems = new List<ItemData>();
        /// <summary>
        /// 低概率合成附魔卡列表
        /// </summary>
        private List<ItemData> mLowProbabilityMagicCardItemList = new List<ItemData>();
        /// <summary>
        /// 高概率合成附魔卡列表
        /// </summary>
        private List<ItemData> mHighProbabilityMagicCardItemList = new List<ItemData>();

        /// <summary>
        /// 附魔卡品质列表
        /// </summary>
        private List<ComControlData> mEnchantmentCardQulityTabDataList = new List<ComControlData>();
        /// <summary>
        /// 附魔卡阶段列表
        /// </summary>
        private List<ComControlData> mEnchantmentCardStageTabDataList = new List<ComControlData>();

        private int mCurrentSelectedMagicCardQuality = 0;//当前选择的品质
        private int iDefaultEnchantmentCardQuality = 0;//默认品质
        private int iDefaultEnchantmentCardStage = 0;//默认阶段

        private void Awake()
        {
            BindUIEvent();
            InitMagicCardUIListScript();
            InitLowProbabilityUIListScript();
            InitHighProbabilityUIListScript();

            if (mBtnMergeCard != null)
            {
                mBtnMergeCard.onClick.RemoveAllListeners();
                mBtnMergeCard.onClick.AddListener(OnMergeCardClick);
            }

            if (mBtnOneKeyMergeCard != null)
            {
                mBtnOneKeyMergeCard.onClick.RemoveAllListeners();
                mBtnOneKeyMergeCard.onClick.AddListener(OnOneKeyMergeCardClick);
            }
        }

        private void OnDestroy()
        {
            UnBindUIEvent();
            UnInitMagicCardUIListScript();
            UnInitLowProbabilityUIListScript();
            UnInitHighProbabilityUIListScript();
            dataMerge = null;
            mAllMagicCardItems.Clear();
            mCurrentSelectedMagicCardQuality = 0;

            if (mHighProbabilityMagicCardItemList != null)
                mHighProbabilityMagicCardItemList.Clear();

            if (mLowProbabilityMagicCardItemList != null)
                mLowProbabilityMagicCardItemList.Clear();
        }

        public void InitView()
        {
            InitMagicCardMergeView();
        }
        
        public void InitMagicCardMergeView()
        {
            mMergeCardItemA.InitMergeCardItem(OnEmptyClick);
            mMergeCardItemB.InitMergeCardItem(OnEmptyClick);
            mComMergeMoneyControl.SetState(ComMergeMoneyControl.CMMC.CMMC_NOT_ENOUGH);
            mComMergeMoneyControl.SetCost(GetMergeCostMoneyID(null), 0);

            mEnchantmentCardQulityTabDataList = EnchantmentsCardManager.GetInstance().GetEnchantmentCardQualityTabDataList();
            mEnchantmentCardStageTabDataList = EnchantmentsCardManager.GetInstance().GetEnchantmentCardStageTabDataList();

            InitEnchantmentCardQulityDrop();
            InitEnchantmentCardStageDrop();

            LoadAllMagicCard();
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
            LoadAllMagicCard();
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
            LoadAllMagicCard();
        }

        #region BindUIEvent 

        private void BindUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnMergeSuccess, OnSlotItemsMergeChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnOneKeyMergeSuccess, OnOneKeyMergeSucceed);
        }

        private void UnBindUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnMergeSuccess, OnSlotItemsMergeChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnOneKeyMergeSuccess, OnOneKeyMergeSucceed);
        }

        private void OnSlotItemsMergeChanged(UIEvent uiEvent)
        {
            dataMerge.leftItem = null;
            dataMerge.rightItem = null;
            mCurrentSelectedMagicCardQuality = 0;
            mMergePreviewRoot.CustomActive(false);
            mMergeDesc.CustomActive(true);
            mMergeCardItemA.Reset();
            mMergeCardItemB.Reset();

            UpdateMoneyInfo();
            LoadAllMagicCard();
        }

        private void OnOneKeyMergeSucceed(UIEvent uiEvent)
        {
            ResetMagicCardMergeSelectedItem();

            mMergeCardItemA.Reset();
            mMergeCardItemB.Reset();

            mMergeCardItemA.UpdateMergeCardItem(dataMerge.leftItem);
            mMergeCardItemB.UpdateMergeCardItem(dataMerge.rightItem);

            UpdateMoneyInfo();
            LoadAllMagicCard();
        }

        //重置附魔卡选中的两个Item
        private void ResetMagicCardMergeSelectedItem()
        {
            if (dataMerge == null)
                return;

            if (dataMerge.leftItem != null)
            {
                //左边Item不存在
                if (ItemDataManager.GetInstance().GetItem(dataMerge.leftItem.GUID) == null)
                {
                    dataMerge.leftItem = null;
                }
            }

            if (dataMerge.rightItem != null)
            {

                var packageRightItemData = ItemDataManager.GetInstance().GetItem(dataMerge.rightItem.GUID);
                //右边Item不存在
                if (packageRightItemData == null)
                {
                    dataMerge.rightItem = null;
                }
                else
                {
                    //如果存在,但只有一个，并且已经装备到左边，则rightItem设置为null
                    if (packageRightItemData.Count <= 1
                        && dataMerge.leftItem != null
                        && dataMerge.leftItem.GUID == packageRightItemData.GUID)
                    {
                        dataMerge.rightItem = null;
                    }
                }
            }
        }
        #endregion

        #region  MagicCardUIListScript

        private void InitMagicCardUIListScript()
        {
            if (mMagicCardUIListScript != null)
            {
                mMagicCardUIListScript.Initialize();
                mMagicCardUIListScript.onBindItem += OnBindItemDelegate;
                mMagicCardUIListScript.onItemVisiable += OnItemVisiableDelegate;
            }
        }

        private void UnInitMagicCardUIListScript()
        {
            if (mMagicCardUIListScript != null)
            {
                mMagicCardUIListScript.onBindItem -= OnBindItemDelegate;
                mMagicCardUIListScript.onItemVisiable -= OnItemVisiableDelegate;
            }
        }

        private MagicCardMergeItemElement OnBindItemDelegate(GameObject itemObject)
        {
            return itemObject.GetComponent<MagicCardMergeItemElement>();
        }

        private void OnItemVisiableDelegate(ComUIListElementScript item)
        {
            var element = item.gameObjectBindScript as MagicCardMergeItemElement;
            if (element != null && item.m_index >= 0 && item.m_index < mAllMagicCardItems.Count)
            {
                element.OnItemVisiable(mAllMagicCardItems[item.m_index], mCurrentSelectedMagicCardQuality, UpdatePutMagicCardInfo, dataMerge);
            }
        }

        public void LoadAllMagicCard()
        {
            if (mAllMagicCardItems == null)
            {
                mAllMagicCardItems = new List<ItemData>();
            }

            mAllMagicCardItems.Clear();

            var itemIds = ItemDataManager.GetInstance().GetItemsByType(ProtoTable.ItemTable.eType.EXPENDABLE);
            for (int i = 0; i < itemIds.Count; i++)
            {
                var itemData = ItemDataManager.GetInstance().GetItem(itemIds[i]);
                if (itemData == null)
                {
                    continue;
                }

                if (itemData.SubType != (int)ProtoTable.ItemTable.eSubType.EnchantmentsCard)
                {
                    continue;
                }

                if (itemData.PackageType == EPackageType.Storage)
                {
                    continue;
                }

                if (itemData.PackageType == EPackageType.RoleStorage)
                {
                    continue;
                }

                if (iDefaultEnchantmentCardQuality != 0)
                {
                    if ((int)itemData.Quality != iDefaultEnchantmentCardQuality)
                        continue;
                }

                if (iDefaultEnchantmentCardStage != 0)
                {
                    if (itemData.EnchantmentCardStage != iDefaultEnchantmentCardStage)
                        continue;
                }

                mAllMagicCardItems.Add(itemData);
            }

            mAllMagicCardItems.Sort(Sort);

            SetElementAmount();
        }

        private void SetElementAmount()
        {
            mMagicCardUIListScript.SetElementAmount(mAllMagicCardItems.Count);
        }

        public int Sort(ItemData left, ItemData right)
        {
            if (left.Quality != right.Quality)
            {
                return (int)right.Quality - (int)left.Quality;
            }

            return right.LevelLimit - left.LevelLimit;
        }

        #endregion

        #region LowProbabilityUIListScript

        private void InitLowProbabilityUIListScript()
        {
            if(mLowProbabilityUIListScript != null)
            {
                mLowProbabilityUIListScript.Initialize();
                mLowProbabilityUIListScript.onBindItem += OnBindLowProbabilityItem;
                mLowProbabilityUIListScript.onItemVisiable += OnLowProbabilityItemVisiable;
            }
        }

        private void UnInitLowProbabilityUIListScript()
        {
            if (mLowProbabilityUIListScript != null)
            {
                mLowProbabilityUIListScript.onBindItem -= OnBindLowProbabilityItem;
                mLowProbabilityUIListScript.onItemVisiable -= OnLowProbabilityItemVisiable;
            }
        }

        private CommonNewItem OnBindLowProbabilityItem(GameObject itemObject)
        {
            return itemObject.GetComponent<CommonNewItem>();
        }

        private void OnLowProbabilityItemVisiable(ComUIListElementScript item)
        {
            var commonNewItem = item.gameObjectBindScript as CommonNewItem;
            if(commonNewItem != null && item.m_index >= 0 && item.m_index < mLowProbabilityMagicCardItemList.Count)
            {
                commonNewItem.InitItem(mLowProbabilityMagicCardItemList[item.m_index].TableID);
            }
        }

        private void OnSetLowElementAmount()
        {
            if (mLowProbabilityUIListScript != null)
                mLowProbabilityUIListScript.SetElementAmount(mLowProbabilityMagicCardItemList.Count);

            if (mLowRoot != null)
                mLowRoot.CustomActive(mLowProbabilityMagicCardItemList.Count > 0 ? true : false);
        }
        #endregion

        #region HighProbabilityUIListScript

        private void InitHighProbabilityUIListScript()
        {
            if (mHighProbabilityUIListScript != null)
            {
                mHighProbabilityUIListScript.Initialize();
                mHighProbabilityUIListScript.onBindItem += OnBindHighProbabilityItem;
                mHighProbabilityUIListScript.onItemVisiable += OnHighProbabilityItemVisiable;
            }
        }

        private void UnInitHighProbabilityUIListScript()
        {
            if (mHighProbabilityUIListScript != null)
            {
                mHighProbabilityUIListScript.onBindItem -= OnBindHighProbabilityItem;
                mHighProbabilityUIListScript.onItemVisiable -= OnHighProbabilityItemVisiable;
            }
        }

        private CommonNewItem OnBindHighProbabilityItem(GameObject itemObject)
        {
            return itemObject.GetComponent<CommonNewItem>();
        }

        private void OnHighProbabilityItemVisiable(ComUIListElementScript item)
        {
            var commonNewItem = item.gameObjectBindScript as CommonNewItem;
            if (commonNewItem != null && item.m_index >= 0 && item.m_index < mHighProbabilityMagicCardItemList.Count)
            {
                commonNewItem.InitItem(mHighProbabilityMagicCardItemList[item.m_index].TableID);
            }
        }

        private void OnSetHighElementAmount()
        {
            if (mHighProbabilityUIListScript != null)
                mHighProbabilityUIListScript.SetElementAmount(mHighProbabilityMagicCardItemList.Count);

            if (mHighRoot != null)
                mHighRoot.CustomActive(mHighProbabilityMagicCardItemList.Count > 0 ? true : false);
        }
        #endregion

        private void UpdatePutMagicCardInfo(ItemData itemData,MagicCardMergeItemElement element)
        {
            if (itemData == null)
            {
                return;
            }

            if (dataMerge.leftItem != null && dataMerge.rightItem != null)
            {
                SystemNotifyManager.SysNotifyTextAnimation("选择数量已达上限");
                return;
            }

            if (mCurrentSelectedMagicCardQuality != 0)
            {
                //放入的附魔卡品质与选中的附魔卡品质不一致
                if (mCurrentSelectedMagicCardQuality != (int)itemData.Quality)
                {
                    SystemNotifyManager.SysNotifyTextAnimation("该附魔卡与已放入的附魔卡品质不同，无法放入");
                    return;
                }
            }

            int allCount = itemData.Count;
            int count = 0;
            if (dataMerge.leftItem != null)
            {
                if (dataMerge.leftItem.GUID == itemData.GUID)
                {
                    count++;
                }
            }

            if (dataMerge.rightItem != null)
            {
                if (dataMerge.rightItem.GUID == itemData.GUID)
                {
                    count++;
                }
            }

            //如果同样的附魔卡已全部放进去
            if (count >= allCount)
            {
                SystemNotifyManager.SysNotifyTextAnimation("放入失败，该附魔卡已全部放入合成区");
                return;
            }

            mCurrentSelectedMagicCardQuality = (int)itemData.Quality;

            bool isCardA = false;

            if (dataMerge.leftItem == null)
            {
                isCardA = true;
            }
           
            if (isCardA == true)
            {
                dataMerge.leftItem = itemData;
            }
            else
            {
                dataMerge.rightItem = itemData;
            }

            mMergeCardItemA.UpdateMergeCardItem(dataMerge.leftItem);
            mMergeCardItemB.UpdateMergeCardItem(dataMerge.rightItem);

            if (element != null)
            {
                element.SetCheckMaskRoot(true);
            }

            if (dataMerge.leftItem != null && dataMerge.rightItem != null)
            {
                mMergePreviewRoot.CustomActive(true);
                mMergeDesc.CustomActive(false);

                GetPreViewBxyList();
            }
            else
            {
                mMergePreviewRoot.CustomActive(false);
                mMergeDesc.CustomActive(true);
            }

            UpdateMoneyInfo();
            SetElementAmount();
        }

        private void UpdateMoneyInfo()
        {
            if (dataMerge.leftItem != null || dataMerge.rightItem != null)
            {
                ItemData item = dataMerge.leftItem != null ? dataMerge.leftItem : dataMerge.rightItem;
                mComMergeMoneyControl.SetState(ComMergeMoneyControl.CMMC.CMMC_ENOUGH);
                int count = GetMergeCardCost(item);
                mComMergeMoneyControl.SetCost(GetMergeCostMoneyID(item), count);
            }
            else
            {
                mComMergeMoneyControl.SetState(ComMergeMoneyControl.CMMC.CMMC_NOT_ENOUGH);
            }
        }

        /// <summary>
        /// 清空槽位数据
        /// </summary>
        /// <param name="isCardA"></param>
        private void OnEmptyClick(bool isCardA)
        {
            if (isCardA == true)
            {
                dataMerge.leftItem = null;
            }
            else
            {
                dataMerge.rightItem = null;
            }

            mMergePreviewRoot.CustomActive(false);
            mMergeDesc.CustomActive(true);

            if (dataMerge.leftItem == null && dataMerge.rightItem == null)
            {
                mCurrentSelectedMagicCardQuality = 0;
            }

            UpdateMoneyInfo();
            SetElementAmount();
        }

        private int GetMergeCostMoneyID(ItemData item)
        {
            if (null != item)
            {
                var magicItem = TableManager.GetInstance().GetTableItem<ProtoTable.MagicCardTable>(item.TableID);
                if (null != magicItem)
                {
                    iMoneyID = magicItem.CostItemId;
                }
            }
            return iMoneyID;
        }

        private int GetMergeCardCost(ItemData left)
        {
            int ret = 0;
            if (null != left)
            {
                var magicItem = TableManager.GetInstance().GetTableItem<ProtoTable.MagicCardTable>(left.TableID);
                if (null != magicItem)
                {
                    ret = magicItem.CostNum;
                }
            }
            return ret;
        }

        #region ButtonClick

        bool m_bInMerge = false;
        private void OnMergeCardClick()
        {
            if (m_bInMerge)
            {
                return;
            }
            m_bInMerge = true;

            OnClickFunctionMerge();

            InvokeMethod.Invoke(this, 0.50f, () =>
            {
                m_bInMerge = false;
            });

        }

        private void OnClickFunctionMerge()
        {
            if (dataMerge != null)
            {
                //是否存在两个附魔卡
                if (dataMerge.leftItem != null && dataMerge.rightItem != null)
                {
                    //品质不同
                    if (dataMerge.leftItem.Quality != dataMerge.rightItem.Quality)
                    {
                        SystemNotifyManager.SystemNotify(1072);
                        return;
                    }

                    //品质大于紫色，进行提示
                    if (dataMerge.leftItem.Quality > ItemTable.eColor.PURPLE)
                    {
                        SystemNotifyManager.SystemNotify(1264, _ConfirmToMergeCard);
                    }
                    else
                    {
                        //品质不大于紫色的时候，存在等级大于等于1的情况，进行提示
                        //附魔卡的等级
                        if (MagicCardMergeUtility.GetMagicCardStrengthLevel(dataMerge.leftItem) >= 1
                            || MagicCardMergeUtility.GetMagicCardStrengthLevel(dataMerge.rightItem) >= 1)
                        {
                            bool isShowMagicCardMergeLevelTip = MagicCardMergeUtility.IsShowMagicCardMergeLevelTip();
                            if (isShowMagicCardMergeLevelTip == true)
                            {
                                MagicCardMergeUtility.OnShowMagicCardMergeLevelTip(_ConfirmToMergeCard,
                                    OnMagicCardMergeLevelTipSetting);
                            }
                            else
                            {
                                _ConfirmToMergeCard();
                            }
                        }
                        else
                        {
                            _ConfirmToMergeCard();
                        }
                    }
                }
                else
                {
                    if (dataMerge.leftItem == null)
                    {
                        SystemNotifyManager.SystemNotify(1070);
                    }
                    else
                    {
                        SystemNotifyManager.SystemNotify(1071);
                    }
                }
            }
        }

        private void _ConfirmToMergeCard()
        {
            CostItemManager.GetInstance().TryCostMoneyDefault(new CostItemManager.CostInfo
            {
                nMoneyID = GetMergeCostMoneyID(dataMerge.leftItem),
                nCount = GetMergeCardCost(dataMerge.leftItem),
            },
            OnConfirmMagicCardBindMethod);
        }

        //判断是否显示绑定方式不一致的提示
        private void OnConfirmMagicCardBindMethod()
        {
            //revert原来的合成方式，无论绑定OR非绑定，都统一合成绑定
            OnFinalSendMagicCardMergeReq();
        }

        //最终进行合成附魔卡
        private void OnFinalSendMagicCardMergeReq()
        {
            //检测背包是否为满
            if (PackageUtility.IsPackageFullByType(EPackageType.Consumable) == true)
            {
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("magic_card_merge_package_is_full"));
                return;
            }

            OnSendMagicCardMergeReq();
        }

        //最终发送合成的消息
        private void OnSendMagicCardMergeReq()
        {
            if (dataMerge == null || dataMerge.leftItem == null || dataMerge.rightItem == null) return;
            EnchantmentsCardManager.GetInstance().SendMergeCard(dataMerge.leftItem.GUID, dataMerge.rightItem.GUID);
        }

        private void OnMagicCardMergeLevelTipSetting(bool value)
        {
            MagicCardMergeDataManager.GetInstance().IsNotShowMagicCardMergeLevelTip = value;
        }

        /// <summary>
        /// 一键合成
        /// </summary>
        private void OnOneKeyMergeCardClick()
        {
            MagicCardMergeUtility.OnOpenMagicCardOneKeyMergeTipFrame(OnMagicCardOneKeyMergeAction);
        }

        private void OnMagicCardOneKeyMergeAction()
        {
            //至少选择一种合成的品质
            if (MagicCardMergeDataManager.GetInstance().IsMagicCardOneKeyMergeUseWhiteCard == false
                && MagicCardMergeDataManager.GetInstance().IsMagicCardOneKeyMergeUseBlueCard == false)
            {
                SystemNotifyManager.SysNotifyFloatingEffect(
                    TR.Value("magic_card_one_key_merge_quality_not_selected"));
                return;
            }

            //背包已满
            if (PackageUtility.IsPackageFullByType(EPackageType.Consumable) == true)
            {
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("magic_card_merge_package_is_full"));
                return;
            }

            int whiteMagicCardNumber = 0;
            int whiteMergeCost = 0;
            int blueMagicCardNumber = 0;
            int blueMergeCost = 0;

            MagicCardMergeUtility.GetMagicCardOneKeyMergeInfo(ref whiteMagicCardNumber,
                ref blueMagicCardNumber,
                ref whiteMergeCost,
                ref blueMergeCost);

            //判断对应品质的附魔卡数量是否大于1，可以用于合成
            if (whiteMagicCardNumber <= 1 && blueMagicCardNumber <= 1)
            {
                SystemNotifyManager.SysNotifyFloatingEffect(
                    TR.Value("magic_card_one_key_merge_item_number_is_not_enough"));
                return;
            }

            #region Gold enough
            //判断金币或者绑金的数量是否可以合成一次
            var ownerMoneyNumber = PlayerBaseData.GetInstance().BindGold;
            if (MagicCardMergeDataManager.GetInstance().IsMagicCardOneKeyMergeUserGold == true)
                ownerMoneyNumber += PlayerBaseData.GetInstance().Gold;

            if (whiteMergeCost > 0)
            {
                //首先合成白色附魔卡，金币不足合成白色的附魔卡一次
                if (ownerMoneyNumber < (ulong)whiteMergeCost)
                {
                    SystemNotifyManager.SysNotifyFloatingEffect(
                        TR.Value("magic_card_merge_bind_gold_less"));
                    return;
                }
            }
            else if (blueMergeCost > 0)
            {
                //白色附魔卡不存在，金币不足合成蓝色的附魔卡一次
                if (ownerMoneyNumber < (ulong)blueMergeCost)
                {
                    SystemNotifyManager.SysNotifyFloatingEffect(
                        TR.Value("magic_card_merge_bind_gold_less"));
                    return;
                }
            }
            #endregion

            var oneKeyMergeTipContent = MagicCardMergeUtility.GetMagicCardOneKeyMergeTipContent(
                whiteMagicCardNumber,
                whiteMergeCost,
                blueMagicCardNumber,
                blueMergeCost);

            MagicCardMergeUtility.OnShowOneKeyMergeTipContent(oneKeyMergeTipContent,
                OnSendMagicCardOneKeyMergeReq);
        }
        
        //发送一键合成的请求
        private void OnSendMagicCardOneKeyMergeReq()
        {
            MagicCardMergeDataManager.GetInstance().SendMagicCardOneKeyMergeReq();
        }

        /// <summary>
        /// 合成预览
        /// </summary>
        private void OnPreViewBtnClick()
        {
            if (ClientSystemManager.GetInstance().IsFrameOpen<MagicCardPreViewFrame>())
            {
                ClientSystemManager.GetInstance().CloseFrame<MagicCardPreViewFrame>();
            }

            ClientSystemManager.GetInstance().OpenFrame<MagicCardPreViewFrame>(FrameLayer.Middle);
        }

        private void GetPreViewBxyList()
        {
            if (mHighProbabilityMagicCardItemList != null)
                mHighProbabilityMagicCardItemList.Clear();

            if (mLowProbabilityMagicCardItemList != null)
                mLowProbabilityMagicCardItemList.Clear();

            

            var mMagicCardTableDic = TableManager.GetInstance().GetTable<MagicCardTable>().GetEnumerator();
            while (mMagicCardTableDic.MoveNext())
            {
                var mMagicCardTable = mMagicCardTableDic.Current.Value as MagicCardTable;

                if (mMagicCardTable.Weight == 0)
                {
                    continue;
                }

                ItemData mItem = null;
                if (dataMerge.leftItem == null && dataMerge.rightItem == null)
                {
                    mItem = ItemDataManager.GetInstance().GetCommonItemTableDataByID(mMagicCardTable.ID);
                }
                else if (dataMerge.leftItem != null && dataMerge.rightItem == null)
                {
                    if (dataMerge.leftItem.Quality == ItemTable.eColor.PURPLE)
                    {
                        if (mMagicCardTable.Color == (int)dataMerge.leftItem.Quality
                            || mMagicCardTable.Color == (int)ItemTable.eColor.PINK)
                        {
                            mItem = ItemDataManager.GetInstance().GetCommonItemTableDataByID(mMagicCardTable.ID);
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else if (dataMerge.leftItem.Quality == ItemTable.eColor.PINK)
                    {
                        if (mMagicCardTable.Color != (int)ItemTable.eColor.PINK)
                        {
                            continue;
                        }

                        mItem = ItemDataManager.GetInstance().GetCommonItemTableDataByID(mMagicCardTable.ID);
                    }
                    else
                    {
                        if (mMagicCardTable.Color == (int)dataMerge.leftItem.Quality
                           || mMagicCardTable.Color == ((int)dataMerge.leftItem.Quality + 1))
                        {
                            mItem = ItemDataManager.GetInstance().GetCommonItemTableDataByID(mMagicCardTable.ID);
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
                else if (dataMerge.leftItem == null && dataMerge.rightItem != null)
                {
                    if (dataMerge.rightItem.Quality == ItemTable.eColor.PURPLE)
                    {
                        if (mMagicCardTable.Color == (int)dataMerge.rightItem.Quality
                            || mMagicCardTable.Color == (int)ItemTable.eColor.PINK)
                        {
                            mItem = ItemDataManager.GetInstance().GetCommonItemTableDataByID(mMagicCardTable.ID);
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else if (dataMerge.rightItem.Quality == ItemTable.eColor.PINK)
                    {
                        if (mMagicCardTable.Color != (int)ItemTable.eColor.PINK)
                        {
                            continue;
                        }

                        mItem = ItemDataManager.GetInstance().GetCommonItemTableDataByID(mMagicCardTable.ID);
                    }
                    else
                    {
                        if (mMagicCardTable.Color == (int)dataMerge.rightItem.Quality
                           || mMagicCardTable.Color == ((int)dataMerge.rightItem.Quality + 1))
                        {
                            mItem = ItemDataManager.GetInstance().GetCommonItemTableDataByID(mMagicCardTable.ID);
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
                else
                {
                    if (dataMerge.leftItem.Quality == ItemTable.eColor.PURPLE)
                    {
                        if (mMagicCardTable.Color == (int)dataMerge.leftItem.Quality
                            || mMagicCardTable.Color == (int)ItemTable.eColor.PINK)
                        {
                            mItem = ItemDataManager.GetInstance().GetCommonItemTableDataByID(mMagicCardTable.ID);
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else if (dataMerge.leftItem.Quality == ItemTable.eColor.PINK)
                    {
                        if (mMagicCardTable.Color != (int)ItemTable.eColor.PINK)
                        {
                            continue;
                        }

                        mItem = ItemDataManager.GetInstance().GetCommonItemTableDataByID(mMagicCardTable.ID);
                    }
                    else
                    {
                        if (mMagicCardTable.Color == (int)dataMerge.leftItem.Quality
                           || mMagicCardTable.Color == ((int)dataMerge.leftItem.Quality + 1))
                        {
                            mItem = ItemDataManager.GetInstance().GetCommonItemTableDataByID(mMagicCardTable.ID);
                        }
                        else
                        {
                            continue;
                        }
                    }
                }

                //跟当前选择的品质一样添加到高概率合成列表，反之添加到低概率合成列表中
                if ((int)mItem.Quality == mCurrentSelectedMagicCardQuality)
                    mHighProbabilityMagicCardItemList.Add(mItem);
                else
                    mLowProbabilityMagicCardItemList.Add(mItem);
            }

            OnSetHighElementAmount();
            OnSetLowElementAmount();
        }
        #endregion
    }
}