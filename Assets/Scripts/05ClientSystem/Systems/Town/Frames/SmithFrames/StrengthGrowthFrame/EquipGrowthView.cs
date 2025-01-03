using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using ProtoTable;
using StrengthOperateResult = Utility.StrengthOperateResult;
///////删除linq
using Protocol;

namespace GameClient
{
    enum GrowthType
    {
        [System.ComponentModel.DescriptionAttribute("使用材料")]
        ST_COST_METERIAL = 0,
        [System.ComponentModel.DescriptionAttribute("使用激化券")]
        ST_PROTECTED_COUPON,
        ST_COUNT,
    }

    class EquipGrowthView : StrengthGrowthBaseView
    {
        [SerializeField] private GameObject mBottomRoot;
        [SerializeField] private GameObject mProtectedContentRoot;
        [SerializeField] private GameObject mEquipItemParent;
        [SerializeField] private GameObject mFrontGroundRoot;
        [SerializeField] private GameObject mGrowthProtectedStampItemComLinkRoot;

        [SerializeField] private GameObject mGrowthProtectedStampItemComLinkRoot1;

        [SerializeField] private Text mEquipName;
        [SerializeField] private Text mCurrentGrowthLevel;
        [SerializeField] private Text mNextGrowthLevel;
        [SerializeField] private Text mGrowthHintText;
        [SerializeField] private Text mGrowthProtectedStampItemCount;

        [SerializeField] private Text mGrowthProtectedStampItemCount1;

        [SerializeField] private ComUIListScript mAttributesComUIListScript;
        [SerializeField] private ComUIListScript mCostMaterialComUIListScript;
        [SerializeField] private StateController mGrowthProtectStateController;
        [SerializeField] private StateController mGrowthProtectStateController1;
        [SerializeField] private StateController mSelectEquipStateContrl;
        [SerializeField] private StateController mGrowthStateContrl;
        [SerializeField] private Button mGrowthBtn;
        [SerializeField] private Button mGrowthSpecialBtn;
        [SerializeField] private Button mStopBtn;
        [SerializeField] private Button mGrowthProtectedStampItemComLinkBtn;
        [SerializeField] private Button mGrowthProtectedStampItemBtn;

        [SerializeField] private Button mGrowthProtectedStampItemComLinkBtn1;
        [SerializeField] private Button mGrowthProtectedStampItemBtn1;

        [SerializeField] private RectTransform mCostMaterialScrollViewRect;
        [SerializeField] private Image mGrowthProtectStampIconBg;
        [SerializeField] private Image mGrowthProtectStampIcon;

        [SerializeField] private Image mGrowthProtectStampIconBg1;
        [SerializeField] private Image mGrowthProtectStampIcon1;

        [SerializeField] private StateController m_kComStrengthenEffect;
        [SerializeField] private StateController mGrowthDeviceStateControl;
        [SerializeField] private CommonTabToggleGroup mCommonTabToggleGroup;

        [Header("新改动")]
        [SerializeField] private GeUISwitchButton mGrowthImplementGeUISwitchButton;
        [SerializeField] private ComUIListScript mGrowthImplementUIList;
        [SerializeField] private ComUIListScript mGrowthStampUIList;
        [SerializeField] private Button mGrowthContinueBtn;
        [SerializeField] private Text mGrowtContinueText;
        [SerializeField] private GeUISwitchButton mGrowthStampGeUISwitchButton;
        [SerializeField] private GeUISwitchButton mGrowthStampGeUISwitchButton1;
        [SerializeField] private RectTransform mBottomRectTransform;

        /// <summary>
        /// 是否使用激化器
        /// </summary>
        private bool bIsUseGrowthImplement = false;
        private GrowthType growthType = GrowthType.ST_COUNT;
        private int iContinueGrowthLevel = 0;//连续激化等级

        private const int iMaxGrowthLevel = 10;//最大等级
        private const float delaySendGrowth = 1.62f;
        private const float delaySendContinueGrowth = 0.7f;
        private SmithShopNewLinkData mLinkData;
        private StrengthenGrowthType mStrengthenGrowthType;
        private StrengthenGrowthView mStrengthenGrowthView;
        private int iMinGrowthLevel = 0; //最小等级
        private int iGrowthNextLevel = 0; //下一等级
        private int iLastGrowTargetLevel = 0; //最终设置的目标等级
        private int iContinueTimes = 0;//连续强化次数
        private ComItemNew mEquipItem;//装备
        private bool mIsUseProtectStamp = false; //是否使用保护券
        private bool mIsUseProtectStamp1 = false; //是否使用保留券
        private bool bOnStart; //单级增幅
        private bool bSpecialGrowth = false; //使用增幅券的标志位
        private bool bContinueGrowth; //连续强化的标志位
        private List<StrengthenAttributeItemData> mItemGrowthAttrA;
        private List<StrengthenAttributeItemData> mItemGrowthAttrB;
        private List<ItemSimpleData> mGrowthCosts;
        private ItemData mCurrentSelectItemData;
        private List<StrengthenResult> m_akData = new List<StrengthenResult>();
        private List<ItemData> mGrowthStampList = new List<ItemData>();
        private ItemData mGrowthStampItemData = null;
        private bool m_bNeedContinueGrowthGoldCostHint = false;
        private ItemData mGrowthDeviceItemData;//强化器道具
        public sealed override void IniteData(SmithShopNewLinkData linkData, StrengthenGrowthType type, StrengthenGrowthView strengthenGrowthView)
        {
            mLinkData = linkData;
            mStrengthenGrowthType = type;
            mStrengthenGrowthView = strengthenGrowthView;
            
            InitGrowthTab();

            InitGrowthProtectStampBGAndIcon();

            InitGrowthProtectStampBGAndIcon1();

            mSelectEquipStateContrl.Key = "NoEquip";
        }

        public sealed override void OnDisableView()
        {
            UnRegisterDelegateHandler();
        }

        public sealed override void OnEnableView()
        {
            RegisterDelegateHandler();
        }

        private void InitGrowthTab()
        {
            if(mCommonTabToggleGroup != null)
            {
                mCommonTabToggleGroup.InitComTab(OnGrowthTabChanged, mLinkData == null ? 0 : mLinkData.iSmithShopNewOpenTypeId);
            }
        }

        private void OnGrowthTabChanged(CommonTabData commonTabData)
        {
            if (commonTabData == null)
                return;

            if ((GrowthType)commonTabData.id == growthType)
                return;

            growthType = (GrowthType)commonTabData.id;

            mBottomRoot.CustomActive(growthType == GrowthType.ST_COST_METERIAL);
            mProtectedContentRoot.CustomActive(growthType == GrowthType.ST_PROTECTED_COUPON);

            if(growthType == GrowthType.ST_COST_METERIAL)
            {
                OnStrengthenGrowthEquipItemClick(mCurrentSelectItemData, mStrengthenGrowthType);
            }
            else
            {
                RefreshGrowthStampUIList();
            }
        }

        /// <summary>
        /// 初始化增幅保护券BG和Icon
        /// </summary>
        private void InitGrowthProtectStampBGAndIcon()
        {
            ItemData itemData = ItemDataManager.CreateItemDataFromTable((int)ItemData.IncomeType.IT_GROWTHPROTECTED);
            if (itemData != null)
            {
                if (mGrowthProtectStampIconBg != null)
                {
                    ETCImageLoader.LoadSprite(ref mGrowthProtectStampIconBg, itemData.GetQualityInfo().Background);
                }

                if (mGrowthProtectStampIcon != null)
                {
                    ETCImageLoader.LoadSprite(ref mGrowthProtectStampIcon, itemData.Icon);
                }
            }
        }

        private void InitGrowthProtectStampBGAndIcon1()
        {
            ItemData itemData = ItemDataManager.CreateItemDataFromTable(900000082);
            if (itemData != null)
            {
                if (mGrowthProtectStampIconBg1 != null)
                {
                    ETCImageLoader.LoadSprite(ref mGrowthProtectStampIconBg1, itemData.GetQualityInfo().Background);
                }

                if (mGrowthProtectStampIcon1 != null)
                {
                    ETCImageLoader.LoadSprite(ref mGrowthProtectStampIcon1, itemData.Icon);
                }
            }
        }
        
        private void ClearData()
        {
            mLinkData = null;
            mStrengthenGrowthType = StrengthenGrowthType.SGT_Strengthen;
            mStrengthenGrowthView = null;
            iMinGrowthLevel = 0;
            iGrowthNextLevel = 0;
            iLastGrowTargetLevel = 0;
            mEquipItem = null;
            mIsUseProtectStamp = false;
            mIsUseProtectStamp1 = false;
            if (mItemGrowthAttrA != null)
            {
                mItemGrowthAttrA.Clear();
            }

            if (mItemGrowthAttrB != null)
            {
                mItemGrowthAttrB.Clear();
            }

            if (mGrowthCosts != null)
            {
                mGrowthCosts.Clear();
            }

            if (m_akData != null)
            {
                m_akData.Clear();
            }

            if (mGrowthStampList != null)
            {
                mGrowthStampList.Clear();
            }
            mCurrentSelectItemData = null;
            mGrowthStampItemData = null;
            mGrowthDeviceItemData = null;
            bIsUseGrowthImplement = false;
            growthType = GrowthType.ST_COUNT;
            iContinueGrowthLevel = 0;
            UnGrowthStampUIList();
            UnGrowthImplementUIList();
        }

        private void Awake()
        {
            RegisterUIEventHandle();
            InitAttributesUIList();
            InitCostMaterialUIListScript();
            
            if(mGrowthStampGeUISwitchButton != null)
            {
                mGrowthStampGeUISwitchButton.onValueChanged.RemoveAllListeners();
                mGrowthStampGeUISwitchButton.onValueChanged.AddListener(OnGrowthStampGeUISwitchButton);
            }

            if(mGrowthStampGeUISwitchButton1 != null)
            {
                mGrowthStampGeUISwitchButton1.onValueChanged.RemoveAllListeners();
                mGrowthStampGeUISwitchButton1.onValueChanged.AddListener(OnGrowthStampGeUISwitchButton1);
            }

            if (mGrowthImplementGeUISwitchButton != null)
            {
                mGrowthImplementGeUISwitchButton.onValueChanged.RemoveAllListeners();
                mGrowthImplementGeUISwitchButton.onValueChanged.AddListener(OnGrowthImplementClick);
            }
            
            if (mGrowthBtn != null)
            {
                mGrowthBtn.onClick.RemoveAllListeners();
                mGrowthBtn.onClick.AddListener(OnGrowthClick);
            }

            if(mGrowthContinueBtn != null)
            {
                mGrowthContinueBtn.onClick.RemoveAllListeners();
                mGrowthContinueBtn.onClick.AddListener(OnGrowthContinueClick);
            }

            if (mStopBtn != null)
            {
                mStopBtn.onClick.RemoveAllListeners();
                mStopBtn.onClick.AddListener(OnClickStop);
            }

            if (mGrowthProtectedStampItemComLinkBtn != null)
            {
                mGrowthProtectedStampItemComLinkBtn.onClick.RemoveAllListeners();
                mGrowthProtectedStampItemComLinkBtn.onClick.AddListener(OnGrowthProtectedStampItemComLinkClick);
            }

            if (mGrowthProtectedStampItemComLinkBtn1 != null)
            {
                mGrowthProtectedStampItemComLinkBtn1.onClick.RemoveAllListeners();
                mGrowthProtectedStampItemComLinkBtn1.onClick.AddListener(OnGrowthProtectedStampItemComLinkClick1);
            }

            if (mGrowthSpecialBtn != null)
            {
                mGrowthSpecialBtn.onClick.RemoveAllListeners();
                mGrowthSpecialBtn.onClick.AddListener(OnGrowthSpecialBtnClick);
            }

            if (mGrowthProtectedStampItemBtn != null)
            {
                mGrowthProtectedStampItemBtn.onClick.RemoveAllListeners();
                mGrowthProtectedStampItemBtn.onClick.AddListener(() =>
                {
                    var itemData = ItemDataManager.CreateItemDataFromTable((int)ItemData.IncomeType.IT_GROWTHPROTECTED);
                    if (itemData != null)
                    {
                        ItemTipManager.GetInstance().ShowTip(itemData);
                    }
                });
            }

            if (mGrowthProtectedStampItemBtn1 != null)
            {
                mGrowthProtectedStampItemBtn1.onClick.RemoveAllListeners();
                mGrowthProtectedStampItemBtn1.onClick.AddListener(() =>
                {
                    var itemData = ItemDataManager.CreateItemDataFromTable(900000082);
                    if (itemData != null)
                    {
                        ItemTipManager.GetInstance().ShowTip(itemData);
                    }
                });
            }
        }

        #region Auction

        private void OnGrowthStampGeUISwitchButton(bool value)
        {
            if (mIsUseProtectStamp == value)
                return;

            mIsUseProtectStamp = value;
            if (value == true)
            {
                //增幅保护券数量
                int iProtectedNum = ItemDataManager.GetInstance().GetOwnedItemCount((int)ItemData.IncomeType.IT_GROWTHPROTECTED);
                if (iProtectedNum <= 0)
                {
                    ItemComeLink.OnLink((int)ItemData.IncomeType.IT_GROWTHPROTECTED, 0, false);
                    mGrowthStampGeUISwitchButton.SetSwitch(false);
                    mIsUseProtectStamp = false;
                    return;
                }

                if (mGrowthProtectStateController != null)
                {
                    mGrowthProtectStateController.Key = "UseProtected";
                }
            }
            else
            {
                if (mGrowthProtectStateController != null)
                {
                    mGrowthProtectStateController.Key = "UnUseProtected";
                }
            }
        }

        private void OnGrowthStampGeUISwitchButton1(bool value)
        {
            if (mIsUseProtectStamp1 == value)
                return;

            mIsUseProtectStamp1 = value;
            if (value == true)
            {
                //增幅保留券数量
                int iProtectedNum = ItemDataManager.GetInstance().GetOwnedItemCount(900000082);
                if (iProtectedNum <= 0)
                {
                    ItemComeLink.OnLink(900000082, 0, false);
                    mGrowthStampGeUISwitchButton1.SetSwitch(false);
                    mIsUseProtectStamp1 = false;
                    return;
                }

                if (mGrowthProtectStateController1 != null)
                {
                    mGrowthProtectStateController1.Key = "UseProtected";
                }
            }
            else
            {
                if (mGrowthProtectStateController1 != null)
                {
                    mGrowthProtectStateController1.Key = "UnUseProtected";
                }
            }
        }

        //用增幅券增幅
        private void OnGrowthSpecialBtnClick()
        {
            if (bOnStart)
            {
                return;
            }

            if (mCurrentSelectItemData == null)
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("growth_item_not_enough"));
                return;
            }

            if (mGrowthStampItemData == null)
            {
                var growthStamps = EquipGrowthDataManager.GetInstance().GetGrowthStampList(mCurrentSelectItemData);
                if (growthStamps != null)
                {
                    if (growthStamps.Count > 0)
                    {
                        SystemNotifyManager.SysNotifyTextAnimation(TR.Value("growth_special_need_protected"));
                    }
                    else
                    {
                        ItemComeLink.OnLink(245, 0);
                    }
                }
                return;
            }

            int timeLeft;
            bool bStartCountdown;
            mGrowthStampItemData.GetLimitTimeLeft(out timeLeft, out bStartCountdown);

            //失效了
            if (timeLeft <= 0 && bStartCountdown)
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("failure_item"));
                return;
            }

            StrengthenSpecialConfirmFrameData data = new StrengthenSpecialConfirmFrameData
            {
                equipData = mCurrentSelectItemData,
                itemData = mGrowthStampItemData
            };
            ClientSystemManager.GetInstance().OpenFrame<StrengthenSpecialConfirmFrame>(FrameLayer.Middle, data);
        }

        #endregion

        private void OnDestroy()
        {
            ClearData();
            UnRegisterUIEventHandle();
            UnInitAttributesUIList();
            UnInitCostMaterialUIListScript();
        }

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

        private void RegisterUIEventHandle()
        {
            //绑定键盘事件
            //UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnCommonKeyBoardInput, OnCommonKeyBoardInput);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.BeginContineStrengthen, OnStartContinueGrowth);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.EndContineStrengthen, OnEndContinueGrowth);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnStrengthChanged, OnGrowthChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.StrengthenDelay, OnGrowthDelay);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.IntterruptContineStrengthen, OnInterruptContinueGrowth);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.StrengthenCanceled, GrowthCanceled);

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ItemGrowthSuccess, OnItemGrowthSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ItemGrowthFail, OnItemGrowthFail);

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnSpecailStrenghthenStart, OnSpecailGrowthStart);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnSpecailStrenghthenCanceled, OnSpecailGrowthCanceled);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnSpecailGrowthFailed, OnSpecailGrowthFailed);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnStrengthenError, OnGrowthError);
            StrengthenGrowthView.mOnStrengthenGrowthEquipItemClick += OnStrengthenGrowthEquipItemClick;

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnEquipmentListNoEquipment, OnEquipmentListNoEquipment);

            RegisterDelegateHandler();
            PlayerBaseData.GetInstance().onMoneyChanged += OnMoneyChanged;
        }

        private void UnRegisterUIEventHandle()
        { //绑定键盘事件
            //UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnCommonKeyBoardInput, OnCommonKeyBoardInput);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.BeginContineStrengthen, OnStartContinueGrowth);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.EndContineStrengthen, OnEndContinueGrowth);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnStrengthChanged, OnGrowthChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.StrengthenDelay, OnGrowthDelay);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.IntterruptContineStrengthen, OnInterruptContinueGrowth);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.StrengthenCanceled, GrowthCanceled);

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ItemGrowthSuccess, OnItemGrowthSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ItemGrowthFail, OnItemGrowthFail);

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnSpecailStrenghthenStart, OnSpecailGrowthStart);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnSpecailStrenghthenCanceled, OnSpecailGrowthCanceled);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnSpecailGrowthFailed, OnSpecailGrowthFailed);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnStrengthenError, OnGrowthError);
            StrengthenGrowthView.mOnStrengthenGrowthEquipItemClick -= OnStrengthenGrowthEquipItemClick;

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnEquipmentListNoEquipment, OnEquipmentListNoEquipment);

            UnRegisterDelegateHandler();
            PlayerBaseData.GetInstance().onMoneyChanged -= OnMoneyChanged;
        }

        #region UIEvent 

        private void OnAddNewItem(List<Item> items)
        {
            bool bNeedRefreshEquipments = false;

            for (int i = 0; i < items.Count; ++i)
            {
                ItemData itemData = ItemDataManager.GetInstance().GetItem(items[i].uid);
                if (itemData == null)
                {
                    continue;
                }

                if (itemData.Type == ProtoTable.ItemTable.eType.EQUIP &&
                        (itemData.PackageType == EPackageType.Equip || itemData.PackageType == EPackageType.WearEquip))
                {
                    bNeedRefreshEquipments = true;
                }

                TryUpdateMaterial(itemData.TableID);
            }

            if (bNeedRefreshEquipments)
            {
                mStrengthenGrowthView.RefreshAllEquipments();
            }
        }

        private void OnUpdateItem(List<Item> items)
        {
            if (items == null)
            {
                return;
            }

            for (int i = 0; i < items.Count; ++i)
            {
                if (items[i] == null)
                {
                    continue;
                }
                ItemData itemData = ItemDataManager.GetInstance().GetItem(items[i].uid);
                if (itemData != null)
                {
                    TryUpdateMaterial((int)itemData.TableID);
                }
            }
        }

        private void OnMoneyChanged(PlayerBaseData.MoneyBinderType eTarget)
        {
            UpdateCostMaterial(mCurrentSelectItemData);
        }

        private void OnRemoveItem(ItemData itemData)
        {
            if (mCurrentSelectItemData != null && mCurrentSelectItemData.GUID == itemData.GUID)
            {
                if (!bOnStart && !bContinueGrowth)
                {
                    mStrengthenGrowthView.RefreshAllEquipments();
                }
            }

            TryUpdateMaterial(itemData.TableID);
        }
        
        private void OnStartContinueGrowth(UIEvent uiEvent)
        {
            if (BCheckIsUpdateFrame() == false)
            {
                return;
            }
            
            //开始连续激化
            bContinueGrowth = true;
            m_bNeedContinueGrowthGoldCostHint = true;
            iContinueTimes = 0;
            StrengthOperateResult eCurResult = StrengthOperateResult.SOR_OK;
            TryOpenNextContinueGrowth(ref eCurResult, (StrengthOperateResult eResult, bool bStopByHand) =>
            {
                OnStopContinueGrowth(eResult, m_akData, bStopByHand);
                m_akData.Clear();
            });
        }

        private void OnEndContinueGrowth(UIEvent uiEvent)
        {
            if (BCheckIsUpdateFrame() == false)
            {
                return;
            }
            iContinueTimes = 0;
            //取消连续激化
            bContinueGrowth = false;
            OnCloseGrowthEffect(null);
            InvokeMethod.RemoveInvokeCall(ContinueGrowthDelyInvoke);
        }

        private void OnGrowthChanged(UIEvent uiEvent)
        {
            if (BCheckIsUpdateFrame() == false)
            {
                return;
            }

            if (mIsUseProtectStamp == true)
            {
                int iProtectedNum = ItemDataManager.GetInstance().GetOwnedItemCount((int)ItemData.IncomeType.IT_GROWTHPROTECTED);
                if (iProtectedNum <= 0)
                {
                    SystemNotifyManager.SysNotifyTextAnimation(TR.Value("failure_coupon"));
                    bOnStart = false;
                    mGrowthStampGeUISwitchButton.SetSwitch(false);
                    return;
                }
            }

            if (mIsUseProtectStamp1 == true)
            {
                int iProtectedNum = ItemDataManager.GetInstance().GetOwnedItemCount(900000082);
                if (iProtectedNum <= 0)
                {
                    SystemNotifyManager.SysNotifyTextAnimation(TR.Value("failure_coupon1"));
                    bOnStart = false;
                    mGrowthStampGeUISwitchButton1.SetSwitch(false);
                    return;
                }
            }

            if (bIsUseGrowthImplement == true)
            {
                //强化器为空
                if (mGrowthDeviceItemData == null)
                {
                    bOnStart = false;
                    SystemNotifyManager.SysNotifyTextAnimation(TR.Value("strengthen_implement"));
                    return;
                }
            }

            mGrowthStateContrl.Key = "Start";

            if (mStrengthenGrowthView != null)
                mStrengthenGrowthView.OnSetMaskRoot(true);

            mStopBtn.CustomActive(bOnStart || bContinueGrowth);

            if (m_kComStrengthenEffect != null)
            {
                m_kComStrengthenEffect.Key = "play";
            }
        }

        private void OnGrowthDelay(UIEvent uiEvent)
        {
            if (BCheckIsUpdateFrame() == false)
            {
                return;
            }
            var systemValue = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)SystemValueTable.eType2.SVT_EQUIP_STRENGTH_ANIM_TIME_DELAY);
            float delayTime = systemValue == null ? delaySendGrowth : (float)systemValue.Value / 1000;
            InvokeMethod.RemoveInvokeCall(DelaySendGrowth);
            InvokeMethod.Invoke(delayTime, DelaySendGrowth);
        }

        private void OnCloseGrowthEffect(UIEvent uiEvent)
        {
            if (m_kComStrengthenEffect != null)
            {
                m_kComStrengthenEffect.Key = "stop";
            }

            if (mGrowthStateContrl != null)
            {
                mGrowthStateContrl.Key = "NotStart";
            }

            if (mStrengthenGrowthView != null)
                mStrengthenGrowthView.OnSetMaskRoot(false);
        }

        private void OnInterruptContinueGrowth(UIEvent uiEvent)
        {
            if (BCheckIsUpdateFrame() == false)
            {
                return;
            }

            if (m_akData.Count > 0)
            {
                var datas = new List<StrengthenResult>();
                datas.AddRange(m_akData);
                m_akData.Clear();

                StrengthenContinueResultsFrame.StrengthenContinueResultsFrameData sendData
                    = new StrengthenContinueResultsFrame.StrengthenContinueResultsFrameData
                    {
                        bStopByHandle = true,
                        eLastOpResult = StrengthOperateResult.SOR_OK,
                        iTarget = iContinueGrowthLevel,
                        results = datas
                    };
                ClientSystemManager.GetInstance().OpenFrame<StrengthenContinueResultsFrame>(FrameLayer.Middle, sendData);
            }

            //手动停掉激化
            bContinueGrowth = false;
            OnCloseGrowthEffect(null);
            InvokeMethod.RemoveInvokeCall(ContinueGrowthDelyInvoke);
        }

        private void GrowthCanceled(UIEvent uiEvent)
        {
            if (BCheckIsUpdateFrame() == false)
            {
                return;
            }
            bOnStart = false;
        }

        private void OnItemGrowthSuccess(UIEvent uiEvent)
        {
            if (BCheckIsUpdateFrame() == false)
            {
                return;
            }
            bOnStart = false;
            mGrowthStampItemData = null;
            StrengthenResult result = EquipGrowthDataManager.GetInstance().GetGrowthResult();
            
            if (bContinueGrowth)
            {
                m_akData.Add(result);

                if (mCurrentSelectItemData.StrengthenLevel >= iContinueGrowthLevel)
                {
                    StrengthenResultUserData userData = new StrengthenResultUserData
                    {
                        uiCode = result.code,
                        EquipData = result.EquipData,
                        BrokenItems = result.BrokenItems,
                        bContinue = bContinueGrowth
                    };

                    OnOpenStrengthenResultFrame(userData);

                    InvokeMethod.Invoke(2.0f, () =>
                    {
                        ClientSystemManager.GetInstance().CloseFrame<StrengthenResultFrame>(null, true);
                        OnSucceedContinueGrowth(m_akData);
                        m_akData.Clear();

                    });
                }
                else
                {
                    StrengthenResultUserData userData = new StrengthenResultUserData
                    {
                        uiCode = result.code,
                        EquipData = result.EquipData,
                        BrokenItems = result.BrokenItems,
                        bContinue = bContinueGrowth
                    };
                    OnOpenStrengthenResultFrame(userData);

                    InvokeMethod.Invoke(2.0f, () =>
                    {
                        ClientSystemManager.GetInstance().CloseFrame<StrengthenResultFrame>(null, true);
                        StrengthOperateResult eCurResult = StrengthOperateResult.SOR_OK;
                        TryOpenNextContinueGrowth(ref eCurResult, (StrengthOperateResult eResult, bool bStopByHand) =>
                        {
                            OnStopContinueGrowth(eResult, m_akData, bStopByHand);
                            m_akData.Clear();
                        });
                    });
                }
            }
            else
            {
                StrengthenResultUserData userData = new StrengthenResultUserData
                {
                    uiCode = result.code,
                    EquipData = result.EquipData,
                    BrokenItems = result.BrokenItems,
                    bContinue = bContinueGrowth,
                    NeedBeforeAnimation = bSpecialGrowth
                };
                bSpecialGrowth = false;
                iContinueGrowthLevel = 0;
                OnOpenStrengthenResultFrame(userData);
            }

            OnCloseGrowthEffect(uiEvent);
        }

        private void OnItemGrowthFail(UIEvent uiEvent)
        {

            bOnStart = false;
            
            StrengthenResult result = EquipGrowthDataManager.GetInstance().GetGrowthResult();
            StrengthenResultUserData userData = new StrengthenResultUserData
            {
                uiCode = result.code,
                EquipData = result.EquipData,
                BrokenItems = result.BrokenItems,
                bContinue = bContinueGrowth
            };

            //如果装备强化破碎，选中破碎装备的下一件
            if (result.code == (uint)ProtoErrorCode.ITEM_STRENTH_FAIL_BROKE)
            {
                EquipGrowthDataManager.GetInstance().IsEquipmentIntensifyBroken = true;
            }

            OnOpenStrengthenResultFrame(userData);

            //装备强化损坏
            if (bContinueGrowth)
            {
                m_akData.Add(result);
                InvokeMethod.Invoke(1.0f, () =>
                {
                    ClientSystemManager.GetInstance().CloseFrame<StrengthenResultFrame>(null, true);
                    StrengthOperateResult eCurResult = StrengthOperateResult.SOR_OK;
                    TryOpenNextContinueGrowth(ref eCurResult, (StrengthOperateResult eResult, bool bStopByHand) =>
                    {
                        OnStopContinueGrowth(eResult, m_akData, bStopByHand);
                        m_akData.Clear();
                    });
                });
            }

            OnCloseGrowthEffect(uiEvent);
        }

        private void OnSpecailGrowthStart(UIEvent uiEvent)
        {
            if (BCheckIsUpdateFrame() == false)
            {
                return;
            }
            
            bOnStart = true;
            bSpecialGrowth = true;

            var systemValue = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)SystemValueTable.eType2.SVT_EQUIP_STRENGTH_ANIM_TIME_DELAY);
            float delayTime = systemValue == null ? delaySendGrowth : (float)systemValue.Value / 1000;
            //启动强化特效
            OnGrowthChanged(null);
            mStopBtn.CustomActive(false);

            InvokeMethod.RemoveInvokeCall(OnDelaySendSpecailGrowth);
            InvokeMethod.Invoke(delayTime, OnDelaySendSpecailGrowth);
        }

        private void OnDelaySendSpecailGrowth()
        {
            if (null == mCurrentSelectItemData || null == mGrowthStampItemData)
            {
                return;
            }

            EquipGrowthDataManager.GetInstance().SceneEquipEnhanceUpgrade(mCurrentSelectItemData, 0, mGrowthStampItemData.GUID);
        }


        private void OnSpecailGrowthCanceled(UIEvent uiEvent)
        {
            if (BCheckIsUpdateFrame() == false)
            {
                return;
            }

            OnCloseGrowthEffect(null);
            bOnStart = false;

        }

        private void OnSpecailGrowthFailed(UIEvent uiEvent)
        {
            if (BCheckIsUpdateFrame() == false)
            {
                return;
            }

            OnSpecailGrowthCanceled(null);
            mGrowthStampItemData = null;
            StrengthenResult result = EquipGrowthDataManager.GetInstance().GetGrowthResult();
            StrengthenResultUserData userData = new StrengthenResultUserData
            {
                uiCode = result.code,
                EquipData = result.EquipData,
                BrokenItems = result.BrokenItems,
                bContinue = bContinueGrowth,
                NeedBeforeAnimation = true
            };
            //激化券失败
            bContinueGrowth = false;
            ClientSystemManager.GetInstance().OpenFrame<StrengthenResultFrame>(FrameLayer.Middle, userData);
            mStrengthenGrowthView.RefreshAllEquipments();
        }

        private void OnGrowthError(UIEvent uiEvent)
        {
            if (BCheckIsUpdateFrame() == false)
            {
                return;
            }

            GrowthCanceled(uiEvent);
            OnSpecailGrowthCanceled(uiEvent);
        }

        private void OnEquipmentListNoEquipment(UIEvent uiEvent)
        {
            mCurrentSelectItemData = null;
            mGrowthStampItemData = null;
            if (mEquipItem != null)
                mEquipItem.Setup(null, null);

            if (mEquipName != null)
            {
                mEquipName.text = string.Empty;
            }

            if (mCurrentGrowthLevel != null)
            {
                mCurrentGrowthLevel.text = string.Empty;
            }

            if (mNextGrowthLevel != null)
            {
                mNextGrowthLevel.text = string.Empty;
            }
            
            if (mAttributesComUIListScript != null)
            {
                mAttributesComUIListScript.SetElementAmount(0);
            }

            if (mCostMaterialComUIListScript != null)
            {
                mCostMaterialComUIListScript.SetElementAmount(0);
            }

            if (mSelectEquipStateContrl != null)
                mSelectEquipStateContrl.Key = "NoEquip";

            mGrowthImplementGeUISwitchButton.SetSwitch(false);
            mGrowthImplementGeUISwitchButton.CustomActive(false);
        }
        #endregion

        #region 增幅 

        private void OnSucceedContinueGrowth(List<StrengthenResult> results)
        {
            if (results == null)
            {
                return;
            }

            var datas = new List<StrengthenResult>();
            datas.AddRange(results);
            StrengthenContinueResultsFrame.StrengthenContinueResultsFrameData sendData
                = new StrengthenContinueResultsFrame.StrengthenContinueResultsFrameData
                {
                    bStopByHandle = false,
                    eLastOpResult = StrengthOperateResult.SOR_OK,
                    iTarget = iContinueGrowthLevel,
                    results = datas
                };
            ClientSystemManager.GetInstance().OpenFrame<StrengthenContinueResultsFrame>(FrameLayer.Middle, sendData);

            //激化到目标等级连续激化的值设为false
            bContinueGrowth = false;
            if (mFrontGroundRoot != null)
                mFrontGroundRoot.CustomActive(false);

            if (mGrowthStateContrl != null)
                mGrowthStateContrl.Key = "NotStart";

            if (mStrengthenGrowthView != null)
                mStrengthenGrowthView.OnSetMaskRoot(false);

            iContinueGrowthLevel = 0;

            if (mStrengthenGrowthView != null)
                mStrengthenGrowthView.RefreshAllEquipments();
        }

        private void OnOpenStrengthenResultFrame(object userData)
        {
            ClientSystemManager.GetInstance().OpenFrame<StrengthenResultFrame>(FrameLayer.Middle, userData);
            mStrengthenGrowthView.RefreshAllEquipments();
        }

        /// <summary>
        /// 检查装备类型是否可以刷新界面
        /// </summary>
        /// <returns></returns>
        private bool BCheckIsUpdateFrame()
        {
            if (mCurrentSelectItemData == null)
            {
                return false;
            }

            if (mCurrentSelectItemData.EquipType != EEquipType.ET_REDMARK)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 增幅按钮
        /// </summary>
        private void OnGrowthClick()
        {
            if (mCurrentSelectItemData == null)
            {
                return;
            }

            if (mCurrentSelectItemData != null && mCurrentSelectItemData.bLocked)
            {
                SystemNotifyManager.SysNotifyFloatingEffect("已加锁的装备无法激化");
                return;
            }


            if (SecurityLockDataManager.GetInstance().CheckSecurityLock())
            {
                return;
            }

            ClientSystemManager.GetInstance().CloseFrame<StrengthenResultFrame>(null, true);
            //增幅
            GrowthEx();
        }

        private bool CheckMaterial()
        {
            var growthCostList = EquipGrowthDataManager.GetInstance().GetGrowthCosts(mCurrentSelectItemData);
            for (int i = 0; i < growthCostList.Count; i++)
            {
                var item = growthCostList[i];
                int iHasCount = ItemDataManager.GetInstance().GetOwnedItemCount(item.ItemID);
                int iNeedCount = item.Count;
                if (iHasCount < iNeedCount)
                {
                    ItemComeLink.OnLink(item.ItemID, 0, true);
                    return true;
                }
            }

            return false;
        }

        private int GetGrowthNeedMonet(ItemData itemData)
        {
            int iBindGoldfNeedCount = 0;
            var growthCostList = EquipGrowthDataManager.GetInstance().GetGrowthCosts(itemData);
            for (int i = 0; i < growthCostList.Count; i++)
            {
                var item = growthCostList[i];
                ItemTable itemTable = TableManager.GetInstance().GetTableItem<ItemTable>(item.ItemID);
                if (itemTable == null)
                {
                    continue;
                }

                if (itemTable.SubType != ItemTable.eSubType.BindGOLD)
                {
                    continue;
                }

                iBindGoldfNeedCount = item.Count;
                break;
            }
            return iBindGoldfNeedCount;
        }

        private void GrowthEx()
        {
            if (bOnStart)
            {
                return;
            }

            if (bIsUseGrowthImplement == false)
            {
                if (CheckMaterial())
                {
                    return;
                }
            }

            if (mIsUseProtectStamp == true)
            {
                int iProtectedNum = ItemDataManager.GetInstance().GetOwnedItemCount((int)ItemData.IncomeType.IT_GROWTHPROTECTED);
                if (iProtectedNum <= 0)
                {
                    SystemNotifyManager.SysNotifyTextAnimation(TR.Value("failure_coupon"));
                    bOnStart = false;
                    mGrowthStampGeUISwitchButton.SetSwitch(false);
                    return;
                }
            }

            if (mIsUseProtectStamp1 == true)
            {
                int iProtectedNum = ItemDataManager.GetInstance().GetOwnedItemCount(900000082);
                if (iProtectedNum <= 0)
                {
                    SystemNotifyManager.SysNotifyTextAnimation(TR.Value("failure_coupon1"));
                    bOnStart = false;
                    mGrowthStampGeUISwitchButton1.SetSwitch(false);
                    return;
                }
            }

            if (bIsUseGrowthImplement == true)
            {
                //强化器为空
                if (mGrowthDeviceItemData == null)
                {
                    SystemNotifyManager.SysNotifyTextAnimation(TR.Value("growth_implement"));
                    bOnStart = false;
                    return;
                }
            }

            mGrowthHintText.text = TR.Value("growth_fixed_level");

            if (bIsUseGrowthImplement == false)
            {
                int iBindGoldId = ItemDataManager.GetInstance().GetMoneyIDByType(ItemTable.eSubType.BindGOLD);
                int iBindGoldfNeedCount = GetGrowthNeedMonet(mCurrentSelectItemData);

                CostItemManager.GetInstance().TryCostMoneyDefault(new CostItemManager.CostInfo { nMoneyID = iBindGoldId, nCount = iBindGoldfNeedCount }, ConfirmGrowth);
            }
            else
            {
                ConfirmGrowth();
            }
        }


        private void ConfirmGrowth()
        {
            bOnStart = true;
            if (mCurrentSelectItemData.StrengthenLevel < 10)
            {
                OnGrowthChanged(null);
                OnGrowthDelay(null);
            }
            else
            {
                StrengthenConfirmData data = new StrengthenConfirmData
                {
                    ItemData = mCurrentSelectItemData,
                    UseProtect = mIsUseProtectStamp,
                    UseStrengthenImplement = bIsUseGrowthImplement,
                    StrengthenImplementItem = mGrowthDeviceItemData
                };
                data.TargetStrengthenLevel = data.ItemData.StrengthenLevel;


                ClientSystemManager.GetInstance().OpenFrame<StrengthenConfirm>(FrameLayer.Middle, data);
            }
        }

        private void DelaySendGrowth()
        {
            if (bOnStart)
            {
                if (mCurrentSelectItemData != null)
                {
                    //同时使用一次性强化器和保护券
                    if (bIsUseGrowthImplement == true && mIsUseProtectStamp1 == true)
                    {
                        EquipGrowthDataManager.GetInstance().SceneEquipEnhanceUpgrade(mCurrentSelectItemData, 4, mGrowthDeviceItemData.GUID);
                    }
                    else if (bIsUseGrowthImplement == true && mIsUseProtectStamp == true)
                    {
                        EquipGrowthDataManager.GetInstance().SceneEquipEnhanceUpgrade(mCurrentSelectItemData, 3, mGrowthDeviceItemData.GUID);
                    }//使用一次性强化器
                    else if (bIsUseGrowthImplement == true)
                    {
                        EquipGrowthDataManager.GetInstance().SceneEquipEnhanceUpgrade(mCurrentSelectItemData, 2, mGrowthDeviceItemData.GUID);
                    }
                    else if (mIsUseProtectStamp1 == true)
                    {
                        EquipGrowthDataManager.GetInstance().SceneEquipEnhanceUpgrade(mCurrentSelectItemData, 5);
                    }
                    else if (mIsUseProtectStamp == true)
                    {
                        EquipGrowthDataManager.GetInstance().SceneEquipEnhanceUpgrade(mCurrentSelectItemData, 1);
                    }
                    else
                    {
                        EquipGrowthDataManager.GetInstance().SceneEquipEnhanceUpgrade(mCurrentSelectItemData, 0);
                    }
                }
            }
        }

        /// <summary>
        /// 连续强化
        /// </summary>
        private void OnGrowthContinueClick()
        {
            if (bContinueGrowth)
            {
                return;
            }

            if (bIsUseGrowthImplement == true)
            {
                //激化器为空
                if (mGrowthDeviceItemData == null)
                {
                    SystemNotifyManager.SysNotifyTextAnimation(TR.Value("strengthen_implement"));
                    return;
                }
            }

            if (bIsUseGrowthImplement == false)
            {
                if (CheckMaterial())
                {
                    return;
                }
            }

            mFrontGroundRoot.CustomActive(true);
            mStopBtn.CustomActive(false);

            m_akData.Clear();

            iContinueGrowthLevel = iLastGrowTargetLevel;

            StrengthenConfirmData data = new StrengthenConfirmData
            {
                ItemData = mCurrentSelectItemData,
                UseProtect = false,
                TargetStrengthenLevel = iContinueGrowthLevel
            };

            ClientSystemManager.GetInstance().OpenFrame<StrengthenContinueConfirm>(FrameLayer.Middle, data);
        }

        private bool TryOpenNextContinueGrowth(ref StrengthOperateResult eResult, UnityEngine.Events.UnityAction<StrengthOperateResult, bool> onCancel)
        {
            if (mCurrentSelectItemData == null)
            {
                return false;
            }

            if (!bContinueGrowth)
            {
                return false;
            }

            //尝试下次激化
            bContinueGrowth = false;
            int iCurLevel = mCurrentSelectItemData.StrengthenLevel;
            if (iCurLevel >= 10)
            {
                return false;
            }

            if (iCurLevel >= iContinueGrowthLevel)
            {
                return false;
            }

            if (bIsUseGrowthImplement == true)
            {
                //强化器为空
                if (mGrowthDeviceItemData == null)
                {
                    eResult = StrengthOperateResult.SOR_Strengthen_Implement;
                    if (eResult != StrengthOperateResult.SOR_OK)
                    {
                        if (onCancel != null)
                        {
                            onCancel.Invoke(eResult, false);
                        }
                        return false;
                    }
                }
            }

            if (bIsUseGrowthImplement == false)
            {
                eResult = Utility.CheckGrowthItem(mCurrentSelectItemData, false);
                if (eResult != StrengthOperateResult.SOR_OK)
                {
                    if (onCancel != null)
                    {
                        onCancel.Invoke(eResult, false);
                    }
                    return false;
                }

                int iBindGoldId = ItemDataManager.GetInstance().GetMoneyIDByType(ItemTable.eSubType.BindGOLD);
                int iNeedGoldCount = GetGrowthNeedMonet(mCurrentSelectItemData);
                int iHasBindGoldCount = ItemDataManager.GetInstance().GetOwnedItemCount(iBindGoldId, false);
                bool bBindGoldEnough = iNeedGoldCount <= iHasBindGoldCount;

                if (!m_bNeedContinueGrowthGoldCostHint || bBindGoldEnough)
                {
                    ConfirmContinueGrowth();
                }
                else
                {
                    CostItemManager.GetInstance().TryCostMoneyDefault(new CostItemManager.CostInfo { nMoneyID = iBindGoldId, nCount = iNeedGoldCount },
                       () =>
                       {
                           m_bNeedContinueGrowthGoldCostHint = false;
                           ConfirmContinueGrowth();
                       },
                    "common_money_cost",
                    () =>
                    {
                        if (onCancel != null)
                        {
                            onCancel.Invoke(StrengthOperateResult.SOR_OK, true);
                        }
                    });
                }
            }
            else
            {
                ConfirmContinueGrowth();
            }

            return true;
        }

        private void ConfirmContinueGrowth()
        {
            //开始下次激化
            bContinueGrowth = true;
            ++iContinueTimes;
            mGrowthHintText.text = TR.Value("growth_dynamic_level", iContinueGrowthLevel, iContinueTimes);
            var systemValue = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)SystemValueTable.eType2.SVT_EQUIP_STRENGTH_CONTINUOUS_ANIM_TIME_DELAY);
            float delayTime = systemValue == null ? delaySendContinueGrowth : (float)systemValue.Value / 1000;
            //启动强化特效
            OnGrowthChanged(null);
            //发送强化消息
            InvokeMethod.RemoveInvokeCall(ContinueGrowthDelyInvoke);
            InvokeMethod.Invoke(delayTime, ContinueGrowthDelyInvoke);
        }

        private void ContinueGrowthDelyInvoke()
        {
            if(bIsUseGrowthImplement && mGrowthDeviceItemData == null)
            {
                StrengthOperateResult eCurResult = StrengthOperateResult.SOR_OK;
                TryOpenNextContinueGrowth(ref eCurResult, (StrengthOperateResult eResult, bool bStopByHand) =>
                {
                    OnStopContinueGrowth(eResult, m_akData, bStopByHand);
                    m_akData.Clear();
                });

                return;
            }

            //同时使用一次性强化器和保护券
            if (bIsUseGrowthImplement == true && mIsUseProtectStamp1 == true)
            {
                EquipGrowthDataManager.GetInstance().SceneEquipEnhanceUpgrade(mCurrentSelectItemData, 4, mGrowthDeviceItemData.GUID);
            }
            else if (bIsUseGrowthImplement == true && mIsUseProtectStamp == true)
            {
                EquipGrowthDataManager.GetInstance().SceneEquipEnhanceUpgrade(mCurrentSelectItemData, 3, mGrowthDeviceItemData.GUID);
            }//使用一次性强化器
            else if (bIsUseGrowthImplement == true)
            {
                EquipGrowthDataManager.GetInstance().SceneEquipEnhanceUpgrade(mCurrentSelectItemData, 2, mGrowthDeviceItemData.GUID);
            }
            else if (mIsUseProtectStamp1 == true)
            {
                EquipGrowthDataManager.GetInstance().SceneEquipEnhanceUpgrade(mCurrentSelectItemData, 5);
            }
            else if (mIsUseProtectStamp == true)
            {
                EquipGrowthDataManager.GetInstance().SceneEquipEnhanceUpgrade(mCurrentSelectItemData, 1);
            }
            else
            {
                EquipGrowthDataManager.GetInstance().SceneEquipEnhanceUpgrade(mCurrentSelectItemData, 0);
            }
        }

        private void OnStopContinueGrowth(StrengthOperateResult eResult, List<StrengthenResult> results, bool bStopByHand)
        {
            //激化材料不足
            bContinueGrowth = false;
            iContinueTimes = 0;

            OnCloseGrowthEffect(null);
            var datas = new List<StrengthenResult>();
            datas.AddRange(results);
            if (datas.Count <= 0)
            {
                Utility.OnPopupStrengthenResultMsg(eResult);
            }
            else
            {
                StrengthenContinueResultsFrame.StrengthenContinueResultsFrameData sendData
                   = new StrengthenContinueResultsFrame.StrengthenContinueResultsFrameData
                   {
                       bStopByHandle = bStopByHand,
                       eLastOpResult = eResult,
                       iTarget = iContinueGrowthLevel,
                       results = datas
                   };
                ClientSystemManager.GetInstance().OpenFrame<StrengthenContinueResultsFrame>(FrameLayer.Middle, sendData);
            }

            iContinueGrowthLevel = 0;
            mStrengthenGrowthView.RefreshAllEquipments();
        }

        private void OnClickStop()
        {
            if (bOnStart)
            {
                InvokeMethod.RemoveInvokeCall(DelaySendGrowth);
                bOnStart = false;
                OnCloseGrowthEffect(null);
            }

            if (bContinueGrowth)
            {
                OnInterruptContinueGrowth(null);
            }

            iContinueGrowthLevel = 0;
            mStrengthenGrowthView.RefreshAllEquipments();
        }

        /// <summary>
        /// 增幅保护券跳转链接
        /// </summary>
        private void OnGrowthProtectedStampItemComLinkClick()
        {
            //增幅保护券
            ItemComeLink.OnLink((int)ItemData.IncomeType.IT_GROWTHPROTECTED, 0);
        }

        /// <summary>
        /// 增幅保护券跳转链接
        /// </summary>
        private void OnGrowthProtectedStampItemComLinkClick1()
        {
            //增幅保护券
            ItemComeLink.OnLink(900000082, 0);
        }
        #endregion


        private void OnStrengthenGrowthEquipItemClick(ItemData itemData, StrengthenGrowthType eStrengthenGrowthType)
        {
            if (itemData == null)
            {
                return;
            }

            mCurrentSelectItemData = itemData;

            if (eStrengthenGrowthType == mStrengthenGrowthType)
            {
                //刷新强化券列表
                if (growthType == GrowthType.ST_PROTECTED_COUPON)
                {
                    RefreshGrowthStampUIList();
                }
                else
                {
                    if (mGrowthStampItemData != null)
                        mGrowthStampItemData = null;
                }

                UpdateGrowthContinueText(itemData);

                mGrowthStampGeUISwitchButton.SetSwitch(false);

                mGrowthStampGeUISwitchButton1.SetSwitch(false);

                if (itemData.StrengthenLevel >= 10)
                {
                    mBottomRectTransform.anchoredPosition = new Vector2(mBottomRectTransform.anchoredPosition.x, 0);
                    mSelectEquipStateContrl.Key = "level>=10";
                }
                else
                {
                    mBottomRectTransform.anchoredPosition = new Vector2(mBottomRectTransform.anchoredPosition.x, -283);
                    mSelectEquipStateContrl.Key = "level<10";
                }

                if (mGrowthStampItemData != null)
                {
                    var ticketTable = TableManager.GetInstance().GetTableItem<StrengthenTicketTable>(mGrowthStampItemData.TableData.StrenTicketDataIndex);
                    if (ticketTable != null)
                    {
                        iMinGrowthLevel = ticketTable.Level;
                    }
                }
                else
                {
                    iMinGrowthLevel = itemData.StrengthenLevel + 1;
                }


                iGrowthNextLevel = iMinGrowthLevel;
                UpdateEquipItem(itemData);
                UpdateAttributes(itemData, iGrowthNextLevel);
                UpdateCostMaterial(itemData);
                UpdateGrowthProtectedStamp();
                UpdateGrowthProtectedStamp1();
                UpdateGrowthDeviceItem();

                if (itemData.StrengthenLevel >= 20)
                {
                    mGrowthImplementGeUISwitchButton.SetSwitch(false);
                    mGrowthImplementGeUISwitchButton.CustomActive(false);
                }
                else
                {
                    mGrowthImplementGeUISwitchButton.CustomActive(true);
                }

            }
        }

        private void UpdateGrowthContinueText(ItemData itemData)
        {
            if (itemData == null)
                return;

            if (itemData.StrengthenLevel >= 0 && itemData.StrengthenLevel < 5)
            {
                iLastGrowTargetLevel = 5;
            }
            else if (itemData.StrengthenLevel >= 5 && itemData.StrengthenLevel < 8)
            {
                iLastGrowTargetLevel = 8;
            }
            else if (itemData.StrengthenLevel >= 8 && itemData.StrengthenLevel < 10)
            {
                iLastGrowTargetLevel = 10;
            }

            if (mGrowtContinueText != null)
                mGrowtContinueText.text = TR.Value("growth_level_desc", iLastGrowTargetLevel);
        }

        /// <summary>
        /// 更新装备信息
        /// </summary>
        /// <param name="itemData"></param>
        private void UpdateEquipItem(ItemData itemData)
        {
            if (mEquipItem == null)
            {
                mEquipItem = ComItemManager.CreateNew(mEquipItemParent);
            }

            mEquipItem.Setup(itemData,Utility.OnItemClicked);

            if (mEquipName != null)
            {
                mEquipName.text = /*TR.Value("growth_strengthlevel_desc", itemData.StrengthenLevel)*/itemData.GetColorName();
            }
        }

        /// <summary>
        /// 更新装备增幅属性
        /// </summary>
        /// <param name="itemData"></param>
        /// <param name="iTargetLevel"></param>
        private void UpdateAttributes(ItemData itemData,int iTargetLevel)
        {
            if (mItemGrowthAttrA == null)
            {
                mItemGrowthAttrA = new List<StrengthenAttributeItemData>();
            }
            else
            {
                mItemGrowthAttrA.Clear();
            }

            if (mItemGrowthAttrB == null)
            {
                mItemGrowthAttrB = new List<StrengthenAttributeItemData>();
            }
            else
            {
                mItemGrowthAttrB.Clear();
            }


            if (itemData != null)
            {
                if (mCurrentGrowthLevel != null)
                {
                    mCurrentGrowthLevel.text = TR.Value("growth_strengthlevel_desc", itemData.StrengthenLevel.ToString());
                }

                if (mNextGrowthLevel != null)
                {
                    mNextGrowthLevel.text = TR.Value("growth_target_strengthlevel_desc", iTargetLevel.ToString());
                }
                
                StrengthenAttributeItemData attrDataA = new StrengthenAttributeItemData();
                attrDataA.kDesc = EquipGrowthDataManager.GetInstance().GetGrowthAttrDesc(itemData.GrowthAttrType);
                attrDataA.iCurValue = itemData.GrowthAttrNum ;
                attrDataA.valueFormat = "{0}";

                mItemGrowthAttrA.Add(attrDataA);

                StrengthenAttributeItemData attrDataB = new StrengthenAttributeItemData();
                attrDataB.kDesc = EquipGrowthDataManager.GetInstance().GetGrowthAttrDesc(itemData.GrowthAttrType);
                attrDataB.iCurValue = EquipGrowthDataManager.GetInstance().GetGrowthAttributeValue(itemData, iTargetLevel);
                attrDataB.valueFormat = "{0}";

                mItemGrowthAttrB.Add(attrDataB);

                //如果是辅助装备
                if (itemData.IsAssistEquip())
                {
                    float attrAValue = StrengthenDataManager.GetInstance().GetAssistEqStrengthAttrValue(itemData, itemData.StrengthenLevel);
                    float attrBValue = StrengthenDataManager.GetInstance().GetAssistEqStrengthAttrValue(itemData, itemData.StrengthenLevel + 1);
                    for (int i = (int)EEquipProp.Strenth; i <= (int)EEquipProp.Stamina; i++)
                    {
                        StrengthenAttributeItemData dataA = new StrengthenAttributeItemData();
                        StrengthenAttributeItemData dataB = new StrengthenAttributeItemData();
                        if (i == (int)EEquipProp.Strenth)
                        {
                            dataA.kDesc = dataB.kDesc = TR.Value("auxiliary_equipment_attr_strength");
                            dataA.valueFormat = dataB.valueFormat = "{0}";

                            dataA.iCurValue = attrAValue;
                            dataB.iCurValue = attrBValue;
                        }
                        else if (i == (int)EEquipProp.Intellect)
                        {
                            dataA.kDesc = dataB.kDesc = TR.Value("auxiliary_equipment_attr_intelligence");
                            dataA.valueFormat = dataB.valueFormat = "{0}";

                            dataA.iCurValue = attrAValue;
                            dataB.iCurValue = attrBValue;
                        }
                        else if (i == (int)EEquipProp.Stamina)
                        {
                            dataA.kDesc = dataB.kDesc = TR.Value("auxiliary_equipment_attr_stamina");
                            dataA.valueFormat = dataB.valueFormat = "{0}";

                            dataA.iCurValue = attrAValue;
                            dataB.iCurValue = attrBValue;
                        }
                        else if (i == (int)EEquipProp.Spirit)
                        {
                            dataA.kDesc = dataB.kDesc = TR.Value("auxiliary_equipment_attr_spirit");
                            dataA.valueFormat = dataB.valueFormat = "{0}";

                            dataA.iCurValue = attrAValue;
                            dataB.iCurValue = attrBValue;
                        }

                        mItemGrowthAttrA.Add(dataA);
                        mItemGrowthAttrB.Add(dataB);
                    }
                }
                else
                {
                    var itemStrengthenAttrA = ItemStrengthAttribute.Create(itemData.TableID);
                    var itemStrengthenAttrB = ItemStrengthAttribute.Create(itemData.TableID);
                    if (itemStrengthenAttrA != null && itemStrengthenAttrB != null)
                    {
                        itemStrengthenAttrA.SetStrength(itemData.StrengthenLevel);
                        itemStrengthenAttrB.SetStrength(iTargetLevel);

                        mItemGrowthAttrA.AddRange(itemStrengthenAttrA.Attributes);
                        mItemGrowthAttrB.AddRange(itemStrengthenAttrB.Attributes);
                    }
                }

                if (mItemGrowthAttrA != null && mItemGrowthAttrB != null)
                {
                    if (mAttributesComUIListScript != null)
                        mAttributesComUIListScript.SetElementAmount(mItemGrowthAttrA.Count);
                }
            }
        }

        /// <summary>
        /// 更新消耗材料
        /// </summary>
        /// <param name="itemData"></param>
        private void UpdateCostMaterial(ItemData itemData)
        {
            if (mGrowthCosts != null)
            {
                mGrowthCosts.Clear();
            }
            else
            {
                mGrowthCosts = new List<ItemSimpleData>();
            }

            var list = EquipGrowthDataManager.GetInstance().GetGrowthCosts(itemData);
            mGrowthCosts.AddRange(list);

            if (mCostMaterialComUIListScript != null)
            {
                mCostMaterialComUIListScript.SetElementAmount(mGrowthCosts.Count);
            }

            UpdateGrowthProtectedStamp();
            UpdateGrowthProtectedStamp1();
        }
        
        /// <summary>
        /// 更新增幅保护券信息
        /// </summary>
        private void UpdateGrowthProtectedStamp()
        {
            int iProtectedNum = ItemDataManager.GetInstance().GetOwnedItemCount((int)ItemData.IncomeType.IT_GROWTHPROTECTED);
            if (iProtectedNum >= 1)
            {
                mGrowthProtectedStampItemCount.text = string.Format(TR.Value("strength_protected_enough"), iProtectedNum);
            }
            else
            {
                mGrowthProtectedStampItemCount.text = string.Format(TR.Value("strength_protected_not_enough"), iProtectedNum);
            }

            mGrowthProtectedStampItemComLinkRoot.CustomActive(iProtectedNum < 1);
        }

        /// <summary>
        /// 更新增幅保留券信息
        /// </summary>
        private void UpdateGrowthProtectedStamp1()
        {
            int iProtectedNum = ItemDataManager.GetInstance().GetOwnedItemCount(900000082);
            if (iProtectedNum >= 1)
            {
                mGrowthProtectedStampItemCount1.text = string.Format(TR.Value("strength_protected_enough"), iProtectedNum);
            }
            else
            {
                mGrowthProtectedStampItemCount1.text = string.Format(TR.Value("strength_protected_not_enough"), iProtectedNum);
            }

            mGrowthProtectedStampItemComLinkRoot1.CustomActive(iProtectedNum < 1);
        }
        
        private void TryUpdateMaterial(int iTableID)
        {
            var item = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(iTableID);
            if (item != null)
            {
                if (item.SubType == ItemTable.eSubType.GOLD ||
                    item.SubType == ItemTable.eSubType.BindGOLD ||
                    item.SubType == ItemTable.eSubType.ST_ZENGFU_PROTECT ||
                    item.SubType == ItemTable.eSubType.ST_ZENGFU_MATERIAL||
                    item.SubType == ItemTable.eSubType.ST_ZENGFU_AMPLIFICATION)
                {
                    UpdateCostMaterial(mCurrentSelectItemData);
                }
                else if (item.ThirdType == ItemTable.eThirdType.DisposableIncreaseItem)
                {
                    UpdateGrowthDeviceItem();
                }
            }
        }

        #region Attributes

        private void InitAttributesUIList()
        {
            if (mAttributesComUIListScript != null)
            {
                mAttributesComUIListScript.Initialize();
                mAttributesComUIListScript.onBindItem += OnBindAttrbutesItemDelegate;
                mAttributesComUIListScript.onItemVisiable += OnAttrbutesItemVisiableDelegate;
            }
        }

        private void UnInitAttributesUIList()
        {
            if (mAttributesComUIListScript != null)
            {
                mAttributesComUIListScript.onBindItem -= OnBindAttrbutesItemDelegate;
                mAttributesComUIListScript.onItemVisiable -= OnAttrbutesItemVisiableDelegate;
            }
        }

        private ComCommonBind OnBindAttrbutesItemDelegate(GameObject itemObject)
        {
            return itemObject.GetComponent<ComCommonBind>();
        }

        private void OnAttrbutesItemVisiableDelegate(ComUIListElementScript item)
        {
            var mBind = item.gameObjectBindScript as ComCommonBind;
            if (mBind != null && item.m_index >= 0 && item.m_index < mItemGrowthAttrA.Count && mItemGrowthAttrA.Count == mItemGrowthAttrB.Count)
            {
                Text mPrefixed = mBind.GetCom<Text>("Prefixed");
                Text curValue = mBind.GetCom<Text>("Value1");
                Text nextValue = mBind.GetCom<Text>("Value2");
                
                if (mPrefixed != null)
                {
                    mPrefixed.text = mItemGrowthAttrA[item.m_index].kDesc;
                }

                if (curValue != null)
                {
                    curValue.text = string.Format("+{0}", mItemGrowthAttrA[item.m_index].ToValueDesc());
                }

                if (nextValue != null)
                {
                    nextValue.text = string.Format("+{0}", mItemGrowthAttrB[item.m_index].ToValueDesc());
                }
            }
        }

        #endregion
        #region CostMaterial

        private void InitCostMaterialUIListScript()
        {
            if (mCostMaterialComUIListScript != null)
            {
                mCostMaterialComUIListScript.Initialize();
                mCostMaterialComUIListScript.onBindItem += OnCostMaterialBindItemDelegate;
                mCostMaterialComUIListScript.onItemVisiable += OnCostMaterialItemVisiableDelegate;
            }
        }

        private void UnInitCostMaterialUIListScript()
        {
            if (mCostMaterialComUIListScript != null)
            {
                mCostMaterialComUIListScript.onBindItem -= OnCostMaterialBindItemDelegate;
                mCostMaterialComUIListScript.onItemVisiable -= OnCostMaterialItemVisiableDelegate;
            }
        }

        private EquipUpgradeCostItem OnCostMaterialBindItemDelegate(GameObject itemObject)
        {
            return itemObject.GetComponent<EquipUpgradeCostItem>();
        }

        private void OnCostMaterialItemVisiableDelegate(ComUIListElementScript item)
        {
            var costItem = item.gameObjectBindScript as EquipUpgradeCostItem;
            if (costItem != null && item.m_index >= 0 && item.m_index < mGrowthCosts.Count)
            {
                costItem.OnItemVisiable(mGrowthCosts[item.m_index]);
            }
        }
        #endregion

        #region  GrowthStamps
        private void OnGrowthStampClcik(ItemData itemData)
        {
           if (itemData == null)
           {
                return;
           }
            mGrowthStampItemData = itemData;
           
            var ticketTable = TableManager.GetInstance().GetTableItem<StrengthenTicketTable>(mGrowthStampItemData.TableData.StrenTicketDataIndex);
            if (ticketTable != null)
            {
                int iTargetLevel = ticketTable.Level;
                UpdateAttributes(mCurrentSelectItemData, iTargetLevel);
            }
        }
        
        List<ItemData> GrowthStampItems = new List<ItemData>();
        private void RefreshGrowthStampUIList()
        {
            GrowthStampItems = EquipGrowthDataManager.GetInstance().GetGrowthStampList(mCurrentSelectItemData);

            if (mGrowthStampUIList != null)
            {
                if (mGrowthStampUIList.IsInitialised())
                {

                }
                else
                {
                    mGrowthStampUIList.Initialize();
                    mGrowthStampUIList.onBindItem += OnBindGrowthStampItemDelegate;
                    mGrowthStampUIList.onItemVisiable += OnGrowthStampItemVisiableDelegate;
                    mGrowthStampUIList.onItemSelected += OnGrowthStampItemSelectedDelegate;
                    mGrowthStampUIList.onItemChageDisplay += OnGrowthStampItemChangeDisplayDelegate;
                }

                mGrowthStampUIList.ResetSelectedElementIndex();
                mGrowthStampUIList.SetElementAmount(GrowthStampItems.Count);

                if (mGrowthStampItemData != null)
                {
                    bool isFind = false;
                    int index = -1;
                    for (int i = 0; i < GrowthStampItems.Count; i++)
                    {
                        var data = GrowthStampItems[i];
                        if (data == null)
                            continue;

                        if (mGrowthStampItemData.GUID != data.GUID)
                            continue;

                        index = i;
                        isFind = true;
                        break;
                    }

                    if (!isFind)
                    {
                        mGrowthStampItemData = null;
                    }
                    else
                    {
                        if (!mGrowthStampUIList.IsElementInScrollArea(index))
                        {
                            mGrowthStampUIList.EnsureElementVisable(index);
                        }

                        mGrowthStampUIList.SelectElement(index);
                    }
                }
            }
        }

        private void UnGrowthStampUIList()
        {
            if (mGrowthStampUIList != null)
            {
                if (mGrowthStampUIList.IsInitialised())
                {
                    mGrowthStampUIList.onBindItem -= OnBindGrowthStampItemDelegate;
                    mGrowthStampUIList.onItemVisiable -= OnGrowthStampItemVisiableDelegate;
                    mGrowthStampUIList.onItemSelected -= OnGrowthStampItemSelectedDelegate;
                    mGrowthStampUIList.onItemChageDisplay -= OnGrowthStampItemChangeDisplayDelegate;
                }
            }
        }

        private CommonImplementItem OnBindGrowthStampItemDelegate(GameObject itemObject)
        {
            return itemObject.GetComponent<CommonImplementItem>();
        }

        private void OnGrowthStampItemVisiableDelegate(ComUIListElementScript item)
        {
            CommonImplementItem commonImplementItem = item.gameObjectBindScript as CommonImplementItem;
            if (commonImplementItem != null && item.m_index >= 0 && item.m_index < GrowthStampItems.Count)
            {
                ItemData itemData = GrowthStampItems[item.m_index];
                commonImplementItem.OnItemVisiable(itemData.GUID);
            }
        }

        private void OnGrowthStampItemSelectedDelegate(ComUIListElementScript item)
        {
            CommonImplementItem commonImplementItem = item.gameObjectBindScript as CommonImplementItem;
            if (commonImplementItem != null)
            {
                OnGrowthStampClcik(commonImplementItem.ItemData);
            }
        }

        private void OnGrowthStampItemChangeDisplayDelegate(ComUIListElementScript item, bool bSelected)
        {
            CommonImplementItem commonImplementItem = item.gameObjectBindScript as CommonImplementItem;
            if (commonImplementItem != null)
            {
                commonImplementItem.OnChangeDisplay(bSelected);
            }
        }

        #endregion

        #region GrowthDevice

        private void OnGrowthImplementClick(bool value)
        {
            if (bIsUseGrowthImplement == value)
                return;

            bIsUseGrowthImplement = value;

            if (value == true)
            {
                mGrowthDeviceStateControl.Key = "true";
                RefreshGrowthImplementUIList();
            }
            else
            {
                mGrowthDeviceStateControl.Key = "false";
                OnStrengthenGrowthEquipItemClick(mCurrentSelectItemData, mStrengthenGrowthType);
            }
        }

        List<ulong> GrowthImplementItems = new List<ulong>();
        private void RefreshGrowthImplementUIList()
        {
            GrowthImplementItems = ItemDataManager.GetInstance().GetItemsByPackageThirdType(EPackageType.Material, ItemTable.eThirdType.DisposableIncreaseItem);

            if (mGrowthImplementUIList != null)
            {
                if (mGrowthImplementUIList.IsInitialised())
                {

                }
                else
                {
                    mGrowthImplementUIList.Initialize();
                    mGrowthImplementUIList.onBindItem += OnBindGrowthImplementItemDelegate;
                    mGrowthImplementUIList.onItemVisiable += OnGrowthImplementItemVisiableDelegate;
                    mGrowthImplementUIList.onItemSelected += OnGrowthImplementItemSelectedDelegate;
                    mGrowthImplementUIList.onItemChageDisplay += OnGrowthImplementItemChangeDisplayDelegate;
                }

                if (bIsUseGrowthImplement && mGrowthDeviceItemData != null)
                {
                    bool isFind = false;
                    for (int i = 0; i < GrowthImplementItems.Count; i++)
                    {
                        var itemData = ItemDataManager.GetInstance().GetItem(GrowthImplementItems[i]);
                        if (itemData == null)
                        {
                            continue;
                        }

                        if (itemData.GUID != mGrowthDeviceItemData.GUID)
                        {
                            continue;
                        }

                        isFind = true;
                        break;
                    }

                    if (!isFind)
                    {
                        mGrowthDeviceItemData = null;
                    }

                    if (mGrowthDeviceItemData == null)
                    {
                        mGrowthImplementUIList.ResetSelectedElementIndex();
                    }
                }
                else
                {
                    mGrowthImplementUIList.ResetSelectedElementIndex();
                }

                mGrowthImplementUIList.SetElementAmount(GrowthImplementItems.Count);
            }

        }

        private void UnGrowthImplementUIList()
        {
            if (mGrowthImplementUIList != null)
            {
                if (mGrowthImplementUIList.IsInitialised())
                {
                    mGrowthImplementUIList.onBindItem -= OnBindGrowthImplementItemDelegate;
                    mGrowthImplementUIList.onItemVisiable -= OnGrowthImplementItemVisiableDelegate;
                    mGrowthImplementUIList.onItemSelected -= OnGrowthImplementItemSelectedDelegate;
                    mGrowthImplementUIList.onItemChageDisplay -= OnGrowthImplementItemChangeDisplayDelegate;
                }
            }
        }

        private CommonImplementItem OnBindGrowthImplementItemDelegate(GameObject itemObject)
        {
            return itemObject.GetComponent<CommonImplementItem>();
        }

        private void OnGrowthImplementItemVisiableDelegate(ComUIListElementScript item)
        {
            CommonImplementItem commonImplementItem = item.gameObjectBindScript as CommonImplementItem;
            if (commonImplementItem != null && item.m_index >= 0 && item.m_index < GrowthImplementItems.Count)
            {
                ulong guid = GrowthImplementItems[item.m_index];
                commonImplementItem.OnItemVisiable(guid);

                if (mGrowthDeviceItemData != null)
                {
                    commonImplementItem.OnChangeDisplay(guid == mGrowthDeviceItemData.GUID);
                }
            }
        }

        private void OnGrowthImplementItemSelectedDelegate(ComUIListElementScript item)
        {
            CommonImplementItem commonImplementItem = item.gameObjectBindScript as CommonImplementItem;
            if (commonImplementItem != null)
            {
                mGrowthDeviceItemData = commonImplementItem.ItemData;
            }
        }

        private void OnGrowthImplementItemChangeDisplayDelegate(ComUIListElementScript item, bool bSelected)
        {
            CommonImplementItem commonImplementItem = item.gameObjectBindScript as CommonImplementItem;
            if (commonImplementItem != null)
                commonImplementItem.OnChangeDisplay(bSelected);
        }
        
        private void UpdateGrowthDeviceItem()
        {
            if (bIsUseGrowthImplement == true)
            {
                mGrowthDeviceStateControl.Key = "true";
                RefreshGrowthImplementUIList();
            }
            else
            {
                mGrowthDeviceStateControl.Key = "false";
            }
        }

        #endregion
    }
}