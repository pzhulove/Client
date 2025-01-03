using Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public enum EquipAdjustTabType
    {
        EAT_METERIAL = 0, //材料
        EAT_PERFECT,
        EAT_COUNT
    }

    public class EquipAdjustView : MonoBehaviour, ISmithShopNewView
    {
        [SerializeField] private ComFunctionAdjust comFunctionAdjust;
        [SerializeField] private GameObject goAdjustComeLink;
        [SerializeField] private GameObject goItemParent;
        [SerializeField] private GameObject goCostItemParent;
        [SerializeField] private GameObject goBaptizeVolumeItemParent;
        [SerializeField] private GameObject goAdjustFunctionCommonEquipment;

        [SerializeField] private RectTransform goEquipmentsTransfom;

        [SerializeField] private Slider m_kEquipmentGradeSlider;
        [SerializeField] private Text m_kEquipmentName;
        [SerializeField] private Text m_kQualityLevel;
        [SerializeField] private Text m_kCostAdjustCount;
        [SerializeField] private Text m_kCostAdjustItemName;
        [SerializeField] private Text m_kPerfectScrollsCount;
        [SerializeField] private Text m_kPerfectScrollsDes;
        [SerializeField] private Text m_kPerfectScrollsItemName;
        [SerializeField] private Text m_kPerfectRollText;

        [SerializeField] private GameObject mBtnAdjustCrazyGo;
        [SerializeField] private GameObject mBtnAdjustOnceGo;
        [SerializeField] private GameObject mToggleRootGo;
        [SerializeField] private GameObject mBaptizeNolumeRootGo;
        [SerializeField] private GameObject mCostParentGo;
        [SerializeField] private GameObject mBG2Go;
        [SerializeField] private GameObject mBG3Go;

        [SerializeField] private CommonTabToggleGroup mCommonTabToggleGroup;

        [SerializeField] private UIGray m_kPerfectScrollsBtnGray;
        [SerializeField] private Button m_kPerfectScrollsBtn;
        [SerializeField] private Button m_kItemComLinkBtn;

        [SerializeField] private int m_iCostAdjustID = 200110002;
        [SerializeField] private int m_perfectScrollsID = 200000329;

        private ComItem m_kComAdjust;
        private ComItem m_kCostAdjustItem;
        private ComItem m_kPerfectScrollsComItem;
        private ComSealEquipmentList m_kComSealEquipmentList = new ComSealEquipmentList();
        private ItemData mCurrentBaptizeItem = null;
        private EquipAdjustTabType EEquipAdjustTabType = EquipAdjustTabType.EAT_COUNT;

        private void Awake()
        {
            RegisterEventHander();

            if (m_kPerfectScrollsBtn != null)
            {
                m_kPerfectScrollsBtn.onClick.RemoveAllListeners();
                m_kPerfectScrollsBtn.onClick.AddListener(OnDetermineWashsPracticeClick);
            }

            if (m_kItemComLinkBtn != null)
            {
                m_kItemComLinkBtn.onClick.RemoveAllListeners();
                m_kItemComLinkBtn.onClick.AddListener(OnAdjustItemComeLink);
            }
        }

        private void OnDestroy()
        {
            UnRegisterEventHander();

            m_kComAdjust = null;
            m_kCostAdjustItem = null;
            m_kPerfectScrollsComItem = null;
            mCurrentBaptizeItem = null;

            m_kComSealEquipmentList.UnInitialize();
            PlayerBaseData.GetInstance().IsSelectedPerfectWashingRollTab = false;
            EEquipAdjustTabType = EquipAdjustTabType.EAT_COUNT;
        }

        private void OnDetermineWashsPracticeClick()
        {
            ClientSystemManager.GetInstance().OpenFrame<PerfectWashRollConfirm>(FrameLayer.Middle);
        }

        private void OnAdjustItemComeLink()
        {
            ItemComeLink.OnLink(m_iCostAdjustID, 0);
        }
        
        public void InitView(SmithShopNewLinkData linkData)
        {
            InitInterface(linkData);
        }

        public void OnEnableView()
        {
            if (comFunctionAdjust != null)
            {
                comFunctionAdjust.RegisterDelegateHandler();
            }
            RegisterDelegateHandler();
            m_kComSealEquipmentList.RefreshAllEquipments(SmithShopNewTabType.SSNTT_ADJUST);
            comFunctionAdjust.StopEffect();
        }

        public void OnDisableView()
        {
            if (comFunctionAdjust != null)
            {
                comFunctionAdjust.UnRegisterDelegateHandler();
            }
            UnRegisterDelegateHandler();
        }

        #region Event(Delegate)

        private void RegisterDelegateHandler()
        {
            ItemDataManager.GetInstance().onAddNewItem += OnAddNewItem;
            ItemDataManager.GetInstance().onRemoveItem += OnRemoveItem;
            ItemDataManager.GetInstance().onUpdateItem += OnUpdateItem;
        }

        private void UnRegisterDelegateHandler()
        {
            ItemDataManager.GetInstance().onAddNewItem -= OnAddNewItem;
            ItemDataManager.GetInstance().onRemoveItem -= OnRemoveItem;
            ItemDataManager.GetInstance().onUpdateItem -= OnUpdateItem;
        }

        private void RegisterEventHander()
        {
            RegisterDelegateHandler();

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ContinueProcessFinish, OnAdjustQualityCrazyFinish);
        }

        private void UnRegisterEventHander()
        {
            UnRegisterDelegateHandler();

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ContinueProcessFinish, OnAdjustQualityCrazyFinish);
        }

        private void OnAdjustQualityCrazyFinish(UIEvent uiEvent)
        {
           if(m_kComSealEquipmentList != null)
            {
                OnAdjustQualityChanged(m_kComSealEquipmentList.Selected);
            }
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

                if (itemData.Type == ProtoTable.ItemTable.eType.EQUIP &&
                       (itemData.PackageType == EPackageType.Equip || itemData.PackageType == EPackageType.WearEquip))
                {
                    //common
                     m_kComSealEquipmentList.RefreshAllEquipments(SmithShopNewTabType.SSNTT_ADJUST);
                }
                
                OnAdjustQualityChanged(m_kComAdjust.ItemData);
            }
        }

        private void OnRemoveItem(ItemData itemData)
        {
            if (itemData == null)
            {
                return;
            }

            if (m_kComSealEquipmentList.Initilized)
            {
                m_kComSealEquipmentList.RefreshAllEquipments(SmithShopNewTabType.SSNTT_ADJUST);
            }

            if (m_iCostAdjustID == (int)itemData.TableID ||
                m_perfectScrollsID == (int)itemData.TableID)
            {
                OnAdjustQualityChanged(m_kComAdjust.ItemData);
            }
        }

        private void OnUpdateItem(List<Item> items)
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

                if (mCurrentBaptizeItem != null && mCurrentBaptizeItem.GUID == itemData.GUID)
                {
                    OnAdjustItemSelected(itemData);
                }

                //qualitychanged
                if (m_kComAdjust != null && m_kComAdjust.ItemData != null && m_kComAdjust.ItemData.GUID == itemData.GUID)
                {
                    OnAdjustQualityChanged(itemData);
                }

                if (m_iCostAdjustID == itemData.TableID && m_kComAdjust != null || m_perfectScrollsID == itemData.TableID)
                {
                    OnAdjustQualityChanged(m_kComAdjust.ItemData);
                }
            }
        }

        #endregion

        #region  Interface

        private void InitInterface(SmithShopNewLinkData linkData)
        {
            if (m_kComAdjust == null)
            {
                m_kComAdjust = ComItemManager.Create(goItemParent);
            }

            m_kComAdjust.Setup(null, null);

            if (m_kCostAdjustItem == null)
            {
                m_kCostAdjustItem = ComItemManager.Create(goCostItemParent);
            }

            ItemData adjustData = ItemDataManager.CreateItemDataFromTable(m_iCostAdjustID);
            if (adjustData != null)
            {
                m_kCostAdjustItem.Setup(adjustData, Utility.OnItemClicked);

                if (m_kCostAdjustItemName != null)
                {
                    m_kCostAdjustItemName.text = adjustData.GetColorName();
                }
            }

            m_kCostAdjustItem.transform.SetAsFirstSibling();

            if (comFunctionAdjust != null)
            {
                comFunctionAdjust.Initialize();
            }

            if (m_kPerfectScrollsComItem == null)
            {
                m_kPerfectScrollsComItem = ComItemManager.Create(goBaptizeVolumeItemParent);
            }

            ItemData perfectScrollsData = ItemDataManager.CreateItemDataFromTable(m_perfectScrollsID);
            if (perfectScrollsData != null)
            {
                m_kPerfectScrollsComItem.Setup(perfectScrollsData, Utility.OnItemClicked);

                if (m_kPerfectScrollsItemName != null)
                {
                    m_kPerfectScrollsItemName.text = perfectScrollsData.GetColorName();
                }
            }
           
            m_kPerfectScrollsComItem.transform.SetAsFirstSibling();

            InitTab();

            if (m_kComSealEquipmentList != null && !m_kComSealEquipmentList.Initilized)
            {
                m_kComSealEquipmentList.Initialize(goAdjustFunctionCommonEquipment, OnAdjustItemSelected, SmithShopNewTabType.SSNTT_ADJUST, linkData);
            }
        }

        private void InitTab()
        {
            if(mCommonTabToggleGroup != null)
            {
                mCommonTabToggleGroup.InitComTab(OnTabChanged, PlayerBaseData.GetInstance().IsSelectedPerfectWashingRollTab == true ? (int)EquipAdjustTabType.EAT_PERFECT : (int)EquipAdjustTabType.EAT_METERIAL);
            }
        }

        private void OnTabChanged(CommonTabData commonTabData)
        {
            if (commonTabData == null)
                return;

            if (EEquipAdjustTabType == (EquipAdjustTabType)commonTabData.id)
                return;

            EEquipAdjustTabType = (EquipAdjustTabType)commonTabData.id;

            mBtnAdjustCrazyGo.CustomActive(EEquipAdjustTabType == EquipAdjustTabType.EAT_METERIAL);
            mBtnAdjustOnceGo.CustomActive(EEquipAdjustTabType == EquipAdjustTabType.EAT_METERIAL);
            mToggleRootGo.CustomActive(EEquipAdjustTabType == EquipAdjustTabType.EAT_METERIAL);
            mCostParentGo.CustomActive(EEquipAdjustTabType == EquipAdjustTabType.EAT_METERIAL);
            mBG2Go.CustomActive(EEquipAdjustTabType == EquipAdjustTabType.EAT_METERIAL);
            mBaptizeNolumeRootGo.CustomActive(EEquipAdjustTabType == EquipAdjustTabType.EAT_PERFECT);
            mBG3Go.CustomActive(EEquipAdjustTabType == EquipAdjustTabType.EAT_PERFECT);
        }

        private void OnAdjustQualityChanged(ItemData itemData)
        {
            m_kComAdjust.Setup(itemData, Utility.OnItemClicked);

            m_kEquipmentName.text = itemData.GetColorName();

            if(m_kEquipmentGradeSlider != null)
            {
                m_kEquipmentGradeSlider.value = itemData.SubQuality / 100.0f;
            }

            int iHasCount = ItemDataManager.GetInstance().GetOwnedItemCount(m_iCostAdjustID);
            int iCostCount = 1;

            m_kCostAdjustCount.enabled = itemData != null;
            m_kCostAdjustCount.text = string.Format("{0}/{1}", iHasCount, iCostCount);
            m_kCostAdjustCount.color = iHasCount < iCostCount ? Color.red : Color.white;
            goAdjustComeLink.CustomActive(iHasCount < iCostCount);
            m_kCostAdjustItem.SetShowNotEnoughState(iHasCount < iCostCount);

            int iHasPerfectScrollsCount = ItemDataManager.GetInstance().GetOwnedItemCount(m_perfectScrollsID);
            m_kPerfectScrollsCount.enabled = itemData != null;
            m_kPerfectScrollsCount.text = string.Format("{0}/{1}", iHasPerfectScrollsCount, iCostCount);
            m_kPerfectScrollsCount.color = iHasPerfectScrollsCount < iCostCount ? Color.red : Color.white;
            m_kPerfectScrollsComItem.SetShowNotEnoughState(iHasPerfectScrollsCount < iCostCount);
            if (itemData != null)
            {
                m_kPerfectScrollsBtnGray.enabled = iHasPerfectScrollsCount <= 0 || itemData.SubQuality >= 100;
                m_kPerfectScrollsBtn.enabled = iHasPerfectScrollsCount > 0 && itemData.SubQuality < 100;
                if (itemData.SubQuality >= 100)
                {
                    m_kPerfectRollText.text = TR.Value("perfectbaptize_dono");
                }
                else
                {
                    m_kPerfectRollText.text = TR.Value("perfectbaptize_sure");
                }
            }
            
            m_kPerfectScrollsDes.text = TR.Value("ItemKeyPerfectScrollsDes");

            string levelText = "";
            if (itemData != null)
            {
                levelText = itemData.GetEquipmentGradeDesc();
            }
            else
            {
                levelText = TR.Value("tip_grade_high_most_1", 100);
            }

            m_kQualityLevel.text = levelText;
        }

        private void OnAdjustItemSelected(ItemData itemData)
        {
            if (itemData == null)
                return;

            mCurrentBaptizeItem = itemData;

            OnAdjustQualityChanged(itemData);
            if (comFunctionAdjust != null)
            {
                comFunctionAdjust.SetUIData(itemData);
            }
        }
        #endregion

        
    }
}