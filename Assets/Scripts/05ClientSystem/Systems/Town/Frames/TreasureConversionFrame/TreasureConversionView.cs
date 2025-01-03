using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using ProtoTable;
using Protocol;

namespace GameClient
{
    public enum TreasureConversionTabType
    {
        TCTT_NONE = 0,
        TCTT_PURPLE = 1,//紫色
        TCTT_PINK,      //粉色
    }

    public enum TreasureMaterialType
    {
        None = 0,
        Gold,
        Treasure,
        Both,
    }

    [System.Serializable]
    public class TreasureConversionTabDataModel
    {
        public TreasureConversionTabType tabType;
        public string tabName;
    }

    public class TreasureConversionView : MonoBehaviour
    {
        [SerializeField] private List<TreasureConversionTabDataModel> mTabsList;
        [SerializeField] private GameObject mTabPrefab;
        [SerializeField] private GameObject mTabParent;
        [SerializeField] private ComUIListScript mTreasureUIListScript;
        [SerializeField] private Button mTreasureBtn;
        [SerializeField] private Text mActivityTime;
        [SerializeField] private ScrollRect mTreasureConversionScrollRect;

        [SerializeField] private StateController mStateControl;

        [SerializeField] private Image mCurrentTreasureBackground;
        [SerializeField] private Image mCurrentTreasureIcon;
        [SerializeField] private Button mCurrentTreasureIconBtn;

        [SerializeField] private Image mConversionEffectBackground;
        [SerializeField] private Image mConversionEffectIcon;
        [SerializeField] private Button mConversionEffectIconBtn;

        [SerializeField] private Button mCostTreasureBtn;
        [SerializeField] private Image mCostTreasureBackground;
        [SerializeField] private Image mCostTreasureIcon;
        [SerializeField] private Text mCostTreasureName;
        [SerializeField] private Text mCostTreasureCount;
        [SerializeField] private GameObject mCostTreasureRoot;

        [SerializeField] private GameObject mCostItemRoot;
        [SerializeField] private Image mCostItemBackground;
        [SerializeField] private Image mCostItemIcon;
        [SerializeField] private Text mCostItemCount;
        [SerializeField] private GameObject mCostItemComLink;
        [SerializeField] private Button mCostItemComLinkBtn;
        [SerializeField] private Button mCostItemBtn;
        [SerializeField] private Toggle mReplacMaterialTog;

        private TreasureConversionTabType defaultTreasureConversionTabType = TreasureConversionTabType.TCTT_PURPLE;
        private TreasureConversionTabType currentSelectedTreasureConversionTabType = TreasureConversionTabType.TCTT_NONE;
        private List<ItemData> treasureConversionItemDataList = new List<ItemData>();
        private ItemData mCurrentSelectedTreasure;//当前要转换的宝珠
        private ItemData mConversionEffectItemData;//当前转换效果
        private ItemData mTreasureMaterialItemData;//材料宝珠
        private BeadConvertCostTable mBeadConvertCostTable;
        private TreasureMaterialType treasureMaterialType = TreasureMaterialType.None;
        private void Awake()
        {
            BindAddListener();
            InitTreasureUIListScript();
            PlayerBaseData.GetInstance().onMoneyChanged += OnMoneyChanged;
            ItemDataManager.GetInstance().onAddNewItem += OnAddNewItem;
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnTreasureConversionSuccessed, OnTreasureConversionSuccessed);
        }

        private void OnDestroy()
        {
            RemoveAddListener();
            UnInitTreasureUIListScript();
            currentSelectedTreasureConversionTabType = TreasureConversionTabType.TCTT_NONE;
            treasureConversionItemDataList.Clear();
            mCurrentSelectedTreasure = null;
            mConversionEffectItemData = null;
            mTreasureMaterialItemData = null;
            mBeadConvertCostTable = null;
            treasureMaterialType = TreasureMaterialType.None;
            PlayerBaseData.GetInstance().onMoneyChanged -= OnMoneyChanged;
            ItemDataManager.GetInstance().onAddNewItem -= OnAddNewItem;
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnTreasureConversionSuccessed, OnTreasureConversionSuccessed);
        }

        public void InitView()
        {
            InitActivityTimer();
            CreatTabs();
        }

        #region BtnClick
        
        private void BindAddListener()
        {
            if (mReplacMaterialTog != null)
            {
                mReplacMaterialTog.onValueChanged.RemoveAllListeners();
                mReplacMaterialTog.onValueChanged.AddListener(OnReplacMaterialTogClick);
            }

            if (mTreasureBtn != null)
            {
                mTreasureBtn.onClick.RemoveAllListeners();
                mTreasureBtn.onClick.AddListener(OnTreasureBtnClick);
            }

            if (mCurrentTreasureIconBtn != null)
            {
                mCurrentTreasureIconBtn.onClick.RemoveAllListeners();
                mCurrentTreasureIconBtn.onClick.AddListener(OnCurrentTreasureIconBtnClick);
            }

            if (mConversionEffectIconBtn != null)
            {
                mConversionEffectIconBtn.onClick.RemoveAllListeners();
                mConversionEffectIconBtn.onClick.AddListener(OnConversionEffectIconBtnClick);
            }

            if (mCostItemComLinkBtn != null)
            {
                mCostItemComLinkBtn.onClick.RemoveAllListeners();
                mCostItemComLinkBtn.onClick.AddListener(OnCostItemComLinkBtnClick);
            }

            if (mCostItemBtn != null)
            {
                mCostItemBtn.onClick.RemoveAllListeners();
                mCostItemBtn.onClick.AddListener(OnCostItemBtnClick);
            }

            if (mCostTreasureBtn != null)
            {
                mCostTreasureBtn.onClick.RemoveAllListeners();
                mCostTreasureBtn.onClick.AddListener(OnCostTreasureBtnClick);
            }
        }

        private void RemoveAddListener()
        {
            if (mReplacMaterialTog != null)
            {
                mReplacMaterialTog.onValueChanged.RemoveListener(OnReplacMaterialTogClick);
            }

            if (mTreasureBtn != null)
            {
                mTreasureBtn.onClick.RemoveListener(OnTreasureBtnClick);
            }

            if (mCurrentTreasureIconBtn != null)
            {
                mCurrentTreasureIconBtn.onClick.RemoveListener(OnCurrentTreasureIconBtnClick);
            }

            if (mConversionEffectIconBtn != null)
            {
                mConversionEffectIconBtn.onClick.RemoveListener(OnConversionEffectIconBtnClick);
            }

            if (mCostItemComLinkBtn != null)
            {
                mCostItemComLinkBtn.onClick.RemoveListener(OnCostItemComLinkBtnClick);
            }

            if (mCostItemBtn != null)
            {
                mCostItemBtn.onClick.RemoveListener(OnCostItemBtnClick);
            }

            if (mCostTreasureBtn != null)
            {
                mCostTreasureBtn.onClick.RemoveListener(OnCostTreasureBtnClick);
            }
        }

        #endregion

        #region Tabs

        private void CreatTabs()
        {
            for (int i = 0; i < mTabsList.Count; i++)
            {
                var tab = mTabsList[i];
                if (tab == null)
                {
                    continue;
                }

                var tabGameObject = GameObject.Instantiate(mTabPrefab);
                tabGameObject.CustomActive(true);
                Utility.AttachTo(tabGameObject, mTabParent);

                ComCommonBind mBind = tabGameObject.GetComponent<ComCommonBind>();
                if (mBind != null)
                {
                    Toggle tabToggle = mBind.GetCom<Toggle>("tab");
                    Text tabName = mBind.GetCom<Text>("tabName");
                    Text checkName = mBind.GetCom<Text>("checkName");

                    if (tabName != null && checkName != null)
                    {
                        tabName.text = checkName.text = tab.tabName;
                    }

                    if (tabToggle != null)
                    {
                        tabToggle.onValueChanged.RemoveAllListeners();
                        tabToggle.onValueChanged.AddListener((value) => 
                        {
                            if (currentSelectedTreasureConversionTabType == tab.tabType)
                            {
                                return;
                            }

                            if (value)
                            {
                                currentSelectedTreasureConversionTabType = tab.tabType;
                                if (mTreasureUIListScript != null)
                                {
                                    mTreasureUIListScript.ResetSelectedElementIndex();
                                }
                                mCurrentSelectedTreasure = null;
                                mConversionEffectItemData = null;
                                mTreasureMaterialItemData = null;
                                UpdateCurrentTreasure();
                                UpdateConversionEffect();
                                UpdateCostTreasure();
                                RefreshTreasureConversionItemDataList(true);

                                if (mStateControl != null)
                                    mStateControl.Key = "None";
                            }
                        });
                    }

                    if (defaultTreasureConversionTabType == tab.tabType)
                    {
                        if (tabToggle != null)
                            tabToggle.isOn = true;
                    }
                }
            }
        }

        #endregion

        #region TreasureUIListScript

        private void InitTreasureUIListScript()
        {
            if (mTreasureUIListScript != null)
            {
                mTreasureUIListScript.Initialize();
                mTreasureUIListScript.onBindItem += OnBindItemDelegate;
                mTreasureUIListScript.onItemVisiable += OnItemVisiableDelegate;
                mTreasureUIListScript.onItemSelected += OnItemSelectedDelegate;
                mTreasureUIListScript.onItemChageDisplay += OnItemChageDisplayDelegate;
            }
        }

        private void UnInitTreasureUIListScript()
        {
            if (mTreasureUIListScript != null)
            {
                mTreasureUIListScript.onBindItem -= OnBindItemDelegate;
                mTreasureUIListScript.onItemVisiable -= OnItemVisiableDelegate;
                mTreasureUIListScript.onItemSelected -= OnItemSelectedDelegate;
                mTreasureUIListScript.onItemChageDisplay -= OnItemChageDisplayDelegate;
            }
        }

        private TreasureConversionItem OnBindItemDelegate(GameObject itemObject)
        {
            return itemObject.GetComponent<TreasureConversionItem>();
        }

        private void OnItemVisiableDelegate(ComUIListElementScript item)
        {
            var treasureConversionItem = item.gameObjectBindScript as TreasureConversionItem;
            if (treasureConversionItem != null &&
                item.m_index >= 0 && 
                item.m_index < treasureConversionItemDataList.Count)
            {
                treasureConversionItem.OnItemVisiable(treasureConversionItemDataList[item.m_index]);
            }
        }

        private void OnItemSelectedDelegate(ComUIListElementScript item)
        {
            var treasureConversionItem = item.gameObjectBindScript as TreasureConversionItem;
            if (treasureConversionItem != null)
            {
                mCurrentSelectedTreasure = treasureConversionItem.TreasureItemData;
                UpdateFrameInfo();
            }
        }

        private void OnItemChageDisplayDelegate(ComUIListElementScript item, bool bSelected)
        {
            var treasureConversionItem = item.gameObjectBindScript as TreasureConversionItem;
            if (treasureConversionItem != null)
                treasureConversionItem.OnItemChangeDisplay(bSelected);
        }
        #endregion

        #region updateframe 

        private void InitActivityTimer()
        {
            ActivityInfo activityInfo = ActiveManager.GetInstance().GetActivityInfo(TR.Value("TreasureConvert_activity_name"));
            if (activityInfo != null)
            {
               mActivityTime.text = Function.GetDateTimeHMS((int)activityInfo.startTime, (int)activityInfo.dueTime);
            }
        }

        private void UpdateFrameInfo()
        {
            if (mCurrentSelectedTreasure == null)
            {
                return;
            }
            
            mBeadConvertCostTable = GetBeadConvertCostTableData((int)mCurrentSelectedTreasure.Quality);
            if (mBeadConvertCostTable != null)
            {
                mConversionEffectItemData = ItemDataManager.CreateItemDataFromTable(mBeadConvertCostTable.ProduceBeadPreview);

                if (mBeadConvertCostTable.MoneyCostId != 0 && mBeadConvertCostTable.BeadCost != 0)
                {
                    treasureMaterialType = TreasureMaterialType.Both;
                }
                else if (mBeadConvertCostTable.MoneyCostId != 0 && mBeadConvertCostTable.BeadCost == 0)
                {
                    treasureMaterialType = TreasureMaterialType.Gold;
                }
                else if (mBeadConvertCostTable.MoneyCostId == 0 && mBeadConvertCostTable.BeadCost != 0)
                {
                    treasureMaterialType = TreasureMaterialType.Treasure;
                }

                switch (treasureMaterialType)
                {
                    case TreasureMaterialType.Gold:
                        {
                            if (mStateControl != null)
                                mStateControl.Key = "Gold";

                            UpdateCostItem(mBeadConvertCostTable);
                        }
                        break;
                    case TreasureMaterialType.Treasure:
                        {
                            if (mStateControl != null)
                                mStateControl.Key = "Treasure";
                        }
                        break;
                    case TreasureMaterialType.Both:
                        {
                            if (mStateControl != null)
                                mStateControl.Key = "Both";

                            UpdateCostItem(mBeadConvertCostTable);

                            OnReplacMaterialTogClick(mReplacMaterialTog.isOn);
                        }
                        break;
                }
            }

            UpdateCurrentTreasure();
            UpdateConversionEffect();

            if (mTreasureMaterialItemData != null)
            {
                mTreasureMaterialItemData = ItemDataManager.GetInstance().GetItem(mTreasureMaterialItemData.GUID);
                if (mTreasureMaterialItemData.GUID == mCurrentSelectedTreasure.GUID)
                {
                    if (mTreasureMaterialItemData.Count - 1 <= 0)
                    {
                        mTreasureMaterialItemData = null;
                    }
                    else
                    {
                        mTreasureMaterialItemData.ShowCount = mTreasureMaterialItemData.Count - 1;
                    }
                }
                else
                {
                    mTreasureMaterialItemData = ItemDataManager.GetInstance().GetItem(mTreasureMaterialItemData.GUID);

                    mTreasureMaterialItemData.ShowCount = mTreasureMaterialItemData.Count;
                }

                UpdateCostTreasure();
            }
        }

        private void UpdateCurrentTreasure()
        {
            if (mCurrentSelectedTreasure == null)
            {
                if (mCurrentTreasureBackground != null)
                    mCurrentTreasureBackground.CustomActive(false);
            }
            else
            {
                if (mCurrentTreasureBackground != null)
                {
                    mCurrentTreasureBackground.CustomActive(true);

                    ETCImageLoader.LoadSprite(ref mCurrentTreasureBackground, mCurrentSelectedTreasure.GetQualityInfo().Background);
                }
            
                if (mCurrentTreasureIcon != null)
                {
                    ETCImageLoader.LoadSprite(ref mCurrentTreasureIcon, mCurrentSelectedTreasure.Icon);
                }
            }
        }

        private void UpdateConversionEffect()
        {
            if (mConversionEffectItemData == null)
            {
                if (mConversionEffectBackground != null)
                    mConversionEffectBackground.CustomActive(false);
            }
            else
            {
                if (mConversionEffectBackground != null)
                {
                    mConversionEffectBackground.CustomActive(true);

                    ETCImageLoader.LoadSprite(ref mConversionEffectBackground, mConversionEffectItemData.GetQualityInfo().Background);
                }
                  
                if (mConversionEffectIcon != null)
                {
                    ETCImageLoader.LoadSprite(ref mConversionEffectIcon, mConversionEffectItemData.Icon);
                }
            }
        }

        private void UpdateCostItem(BeadConvertCostTable beadConvertCostTable)
        {
            ItemData costItem = ItemDataManager.CreateItemDataFromTable(beadConvertCostTable.MoneyCostId);
            if (costItem == null)
            {
                return;
            }

            if (mCostItemBackground != null)
            {
                ETCImageLoader.LoadSprite(ref mCostItemBackground, costItem.GetQualityInfo().Background);
            }

            if (mCostItemIcon != null)
            {
                ETCImageLoader.LoadSprite(ref mCostItemIcon, costItem.Icon);
            }
            
            RefreshCostItemCount(beadConvertCostTable);
        }

        private void RefreshCostItemCount(BeadConvertCostTable beadConvertCostTable)
        {
            int count = ItemDataManager.GetInstance().GetOwnedItemCount(beadConvertCostTable.MoneyCostId);

            if (count >= beadConvertCostTable.MoneyCostNum)
            {
                mCostItemCount.text = TR.Value("EquipUpgrade_Merial_white", count, beadConvertCostTable.MoneyCostNum);
            }
            else
            {
                mCostItemCount.text = TR.Value("EquipUpgrade_Merial_Red", count, beadConvertCostTable.MoneyCostNum);
            }

            mCostItemComLink.CustomActive(count < beadConvertCostTable.MoneyCostNum);
        }

        private void RefreshTreasureConversionItemDataList(bool setScrollRect = false)
        {
            if (treasureConversionItemDataList == null)
            {
                treasureConversionItemDataList = new List<ItemData>();
            }

            treasureConversionItemDataList.Clear();

            var items = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.Consumable);
            if (items != null)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    ItemData itemData = ItemDataManager.GetInstance().GetItem(items[i]);
                    if (itemData == null)
                    {
                        continue;
                    }

                    if (itemData.SubType != (int)ItemTable.eSubType.Bead)
                    {
                        continue;
                    }

                    if (currentSelectedTreasureConversionTabType == TreasureConversionTabType.TCTT_PURPLE)
                    {
                        if (itemData.Quality != ItemTable.eColor.PURPLE)
                        {
                            continue;
                        }
                    }
                    else if (currentSelectedTreasureConversionTabType == TreasureConversionTabType.TCTT_PINK)
                    {
                        if (itemData.Quality != ItemTable.eColor.PINK)
                        {
                            continue;
                        }
                    }

                    itemData.ShowCount = itemData.Count;

                    treasureConversionItemDataList.Add(itemData);
                }
            }

            treasureConversionItemDataList.Sort((x, y) =>
            {
                return x.TableID - y.TableID;
            });

            if (mTreasureUIListScript != null)
            {
                mTreasureUIListScript.SetElementAmount(treasureConversionItemDataList.Count);
                //mTreasureUIListScript.SelectElement(0);
            }
                
            if (setScrollRect)
            {
                if (mTreasureConversionScrollRect != null)
                {
                    mTreasureConversionScrollRect.verticalNormalizedPosition = 1;
                }
            }

            if (mCurrentSelectedTreasure != null)
            {
                int index = -1;
                for (int i = 0; i < treasureConversionItemDataList.Count; i++)
                {
                    ulong guid = treasureConversionItemDataList[i].GUID;
                    if (guid != mCurrentSelectedTreasure.GUID)
                    {
                        continue;
                    }

                    index = i;
                }

                if (index >= 0 && index < treasureConversionItemDataList.Count)
                {
                    if (!mTreasureUIListScript.IsElementInScrollArea(index))
                    {
                        mTreasureUIListScript.EnsureElementVisable(index);
                    }
                    mTreasureUIListScript.SelectElement(index);
                }
                else
                {
                    mTreasureUIListScript.SelectElement(-1);
                }
            }
        }

        private void OnTreasureChooseCallback(ItemData itemData)
        {
            if (itemData == null)
            {
                return;
            }

            mTreasureMaterialItemData = itemData;

            UpdateCostTreasure();
        }

        private void UpdateCostTreasure()
        {
            if (mTreasureMaterialItemData == null)
            {
                if (mCostTreasureBackground != null)
                    mCostTreasureBackground.CustomActive(false);

                if (mCostTreasureName != null)
                    mCostTreasureName.text = "请选择宝珠";
            }
            else
            {
                if (mCostTreasureBackground != null)
                {
                    mCostTreasureBackground.CustomActive(true);
                    ETCImageLoader.LoadSprite(ref mCostTreasureBackground, mTreasureMaterialItemData.GetQualityInfo().Background);
                }

                if (mCostTreasureIcon != null)
                {
                    ETCImageLoader.LoadSprite(ref mCostTreasureIcon, mTreasureMaterialItemData.Icon);
                }

                if (mCostTreasureName != null)
                {
                    mCostTreasureName.text = mTreasureMaterialItemData.GetColorName();
                }
                
                if (mTreasureMaterialItemData.ShowCount >= mBeadConvertCostTable.BeadCostNum)
                {
                    mCostTreasureCount.text = TR.Value("EquipUpgrade_Merial_white", mTreasureMaterialItemData.ShowCount, mBeadConvertCostTable.BeadCostNum);
                }
                else
                {
                    mCostTreasureCount.text = TR.Value("EquipUpgrade_Merial_Red", mTreasureMaterialItemData.ShowCount, mBeadConvertCostTable.BeadCostNum);
                }
            }
        }
        #endregion

        #region CallBack
        private void OnReplacMaterialTogClick(bool value)
        {
            if (value)
            {
                mCostTreasureRoot.CustomActive(true);
                mCostItemRoot.CustomActive(false);
            }
            else
            {
                mCostTreasureRoot.CustomActive(false);
                mCostItemRoot.CustomActive(true);
            }
        }

        private void OnCurrentTreasureIconBtnClick()
        {
            ShowTip(mCurrentSelectedTreasure);
        }

        private void OnConversionEffectIconBtnClick()
        {
            ShowTip(mConversionEffectItemData);
        }

        private void ShowTip(ItemData itemData)
        {
            if (itemData != null)
            {
                ItemTipManager.GetInstance().ShowTip(itemData);
            }
        }

        private void OnCostItemComLinkBtnClick()
        {
            if (mBeadConvertCostTable != null)
            {
                ItemComeLink.OnLink(mBeadConvertCostTable.MoneyCostId, 0);
            }
        }

        private void OnCostItemBtnClick()
        {
            if (mBeadConvertCostTable != null)
            {
                ItemData itemData = ItemDataManager.CreateItemDataFromTable(mBeadConvertCostTable.MoneyCostId);
                ShowTip(itemData);
            }
        }

        private void OnCostTreasureBtnClick()
        {
            if (mCurrentSelectedTreasure == null)
            {
                return;
            }

            List<ItemData> treasureChooseList = new List<ItemData>();

            var items = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.Consumable);
            if (items != null)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    var itemData = ItemDataManager.GetInstance().GetItem(items[i]);
                    if (itemData == null)
                    {
                        continue;
                    }

                    if (itemData.SubType != (int)ItemTable.eSubType.Bead)
                    {
                        continue;
                    }

                    if (currentSelectedTreasureConversionTabType == TreasureConversionTabType.TCTT_PINK)
                    {
                        if (itemData.Quality != ItemTable.eColor.PINK)
                        {
                            continue;
                        }
                    }
                    else if (currentSelectedTreasureConversionTabType == TreasureConversionTabType.TCTT_PURPLE)
                    {
                        if (itemData.Quality != ItemTable.eColor.PURPLE)
                        {
                            continue;
                        }
                    }

                    itemData.ShowCount = itemData.Count;

                    //如果宝珠是选中的宝珠，判断数量减1数量是否小于等于零，是的话就过滤掉
                    if (itemData.GUID == mCurrentSelectedTreasure.GUID)
                    {
                        if (itemData.Count - 1 <= 0)
                        {
                            continue;
                        }

                        itemData.ShowCount = itemData.Count - 1;
                    }

                    treasureChooseList.Add(itemData);
                }
            }

            if (treasureChooseList.Count <= 0)
            {
                //选中的粉色宝珠
                if (currentSelectedTreasureConversionTabType == TreasureConversionTabType.TCTT_PINK)
                {
                    //弹获取途径
                    ItemComeLink.OnLink(294,0);
                }//紫色宝珠
                else if (currentSelectedTreasureConversionTabType == TreasureConversionTabType.TCTT_PURPLE)
                {//弹获取途径
                    ItemComeLink.OnLink(293, 0);
                }

                return;
            }

            treasureChooseList.Sort((x, y) => { return x.TableID - y.TableID; });

            TreasureChooseParam param = new TreasureChooseParam();
            param.treasureChooseList = treasureChooseList;
            param.treasureChooseCallback += OnTreasureChooseCallback;

            ClientSystemManager.GetInstance().OpenFrame<TreasureChooseFrame>(FrameLayer.Middle, param);
        }

        private void OnTreasureBtnClick()
        {
            //当前选择的转换宝珠为空
            if (mCurrentSelectedTreasure == null)
            {
                return;
            }
            //宝珠转换的表格数据
            if (mBeadConvertCostTable == null)
            {
                return;
            }

            ulong materialGuid = 0;
            //只可以用金币作为
            if (treasureMaterialType == TreasureMaterialType.Gold)
            {
                int totalCount = ItemDataManager.GetInstance().GetOwnedItemCount(mBeadConvertCostTable.MoneyCostId);
                if (totalCount < mBeadConvertCostTable.MoneyCostNum)
                {
                    SystemNotifyManager.SysNotifyFloatingEffect("金币不足，转换失败。");
                    return;
                }
            }//只可以用宝珠作为材料
            else if (treasureMaterialType == TreasureMaterialType.Treasure)
            {
                if (mTreasureMaterialItemData == null)
                {
                    SystemNotifyManager.SysNotifyFloatingEffect("请放入材料宝珠");
                    return;
                }

                materialGuid = mTreasureMaterialItemData.GUID;
            } //可选择金币或者宝珠作为材料
            else if (treasureMaterialType == TreasureMaterialType.Both)
            {
                if(mReplacMaterialTog != null)
                {
                    if (mReplacMaterialTog.isOn == true)
                    {
                        if (mTreasureMaterialItemData == null)
                        {
                            SystemNotifyManager.SysNotifyFloatingEffect("请放入材料宝珠");
                            return;
                        }
                        else
                        {
                            materialGuid = mTreasureMaterialItemData.GUID;
                        }
                    }
                    else
                    {
                        int totalCount = ItemDataManager.GetInstance().GetOwnedItemCount(mBeadConvertCostTable.MoneyCostId);
                        if (totalCount < mBeadConvertCostTable.MoneyCostNum)
                        {
                            SystemNotifyManager.SysNotifyFloatingEffect("金币不足，转换失败。");
                            return;
                        }
                    }
                }
            }

            if (mCurrentSelectedTreasure != null)
            {
                //如果材料宝珠为粉色以上，弹框提示玩家
                if (mCurrentSelectedTreasure.Quality >= ItemTable.eColor.PINK &&
                    BeadCardManager.GetInstance().TreasureConvertTip == false)
                {
                    CommonMsgBoxOkCancelNewParamData comconMsgBoxOkCancelParamData = new CommonMsgBoxOkCancelNewParamData();
                    comconMsgBoxOkCancelParamData.ContentLabel = TR.Value("TreasureConvert_tip");
                    comconMsgBoxOkCancelParamData.IsShowNotify = true;
                    comconMsgBoxOkCancelParamData.OnCommonMsgBoxToggleClick = OnCommonMsgBoxToggleClick;
                    comconMsgBoxOkCancelParamData.LeftButtonText = TR.Value("common_data_cancel");
                    comconMsgBoxOkCancelParamData.RightButtonText = TR.Value("common_data_sure_2");
                    comconMsgBoxOkCancelParamData.OnRightButtonClickCallBack = ()=> 
                    {
                        BeadCardManager.GetInstance().OnSceneBeadConvertReq(mCurrentSelectedTreasure.GUID, materialGuid);
                    };

                    if (ClientSystemManager.GetInstance().IsFrameOpen<AuctionNewMsgBoxOkCancelFrame>() == true)
                    {
                        ClientSystemManager.GetInstance().CloseFrame<AuctionNewMsgBoxOkCancelFrame>();
                    }

                    ClientSystemManager.GetInstance().OpenFrame<AuctionNewMsgBoxOkCancelFrame>(FrameLayer.High, comconMsgBoxOkCancelParamData);
                    return;
                }
            }

            BeadCardManager.GetInstance().OnSceneBeadConvertReq(mCurrentSelectedTreasure.GUID, materialGuid);
        }

        private void OnCommonMsgBoxToggleClick(bool value)
        {
            BeadCardManager.GetInstance().TreasureConvertTip = value;
        }
        #endregion

        /// <summary>
        /// 根据宝珠品质得到宝珠转换消耗合成表格数据
        /// </summary>
        /// <param name="quality"></param>
        /// <returns></returns>
        private BeadConvertCostTable GetBeadConvertCostTableData(int quality)
        {
            var Dicts = TableManager.GetInstance().GetTable<BeadConvertCostTable>();
            if (Dicts != null)
            {
                var iter = Dicts.GetEnumerator();
                while (iter.MoveNext())
                {
                    var table = iter.Current.Value as BeadConvertCostTable;
                    if (table == null)
                    {
                        continue;
                    }

                    if (table.Colour != quality)
                    {
                        continue;
                    }

                    return table;
                }
            }

            return null;
        }

        private void OnAddNewItem(List<Item> items)
        {
            if (items == null)
            {
                return;
            }

            for (int i = 0; i < items.Count; i++)
            {
                ItemData itemData = ItemDataManager.GetInstance().GetItem(items[i].uid);
                if (itemData == null)
                {
                    continue;
                }

                if (itemData.SubType == (int)ProtoTable.ItemTable.eSubType.Bead)
                {
                    RefreshTreasureConversionItemDataList();
                }
            }
        }

        private void OnMoneyChanged(PlayerBaseData.MoneyBinderType eMoneyBinderType)
        {
            if (eMoneyBinderType == PlayerBaseData.MoneyBinderType.MBT_GOLD||
                eMoneyBinderType == PlayerBaseData.MoneyBinderType.MBT_BIND_GOLD)
            {
                if ((treasureMaterialType == TreasureMaterialType.Gold|| treasureMaterialType== TreasureMaterialType.Both ) && mBeadConvertCostTable != null)
                {
                    RefreshCostItemCount(mBeadConvertCostTable);
                }
            }
        }

        private void OnTreasureConversionSuccessed(UIEvent uiEvent)
        {
            if (mTreasureUIListScript != null)
            {
                mTreasureUIListScript.ResetSelectedElementIndex();
            }
            mCurrentSelectedTreasure = null;
            mConversionEffectItemData = null;
            mTreasureMaterialItemData = null;
            UpdateCurrentTreasure();
            UpdateConversionEffect();
            UpdateCostTreasure();
            RefreshTreasureConversionItemDataList(false);

            if (mStateControl != null)
                mStateControl.Key = "None";
        }
    }
}