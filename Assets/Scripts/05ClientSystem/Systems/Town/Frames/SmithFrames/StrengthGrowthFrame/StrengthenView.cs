using ActivityLimitTime;
using Protocol;
using ProtoTable;
using System.Collections;
using System.Collections.Generic;
///////删除linq
using UnityEngine;
using UnityEngine.UI;
using StrengthOperateResult = Utility.StrengthOperateResult;
using Scripts.UI;
using DG.Tweening;

namespace GameClient
{
    class StrengthenView : StrengthGrowthBaseView
    {
        [SerializeField]private Text m_kTextProtectedHint;
        [SerializeField]private GameObject m_goMaterialParent;
        [SerializeField]private GameObject m_goMaterialPrefab;
        [SerializeField]private GameObject m_goMaterialParent1;
        [SerializeField]private GameObject m_goMaterialPrefab1;
        [SerializeField]private GameObject goUnuseProtectedHint;
        [SerializeField]private GameObject goProectedComeLink;
        [SerializeField]private Button m_kBtnStrength;
        [SerializeField]private StateController comStatus;
        [SerializeField]private StateController comProtectedStatus;
        [SerializeField]private UIGray m_kGray;
        [SerializeField]private GameObject goItemParent;
        [SerializeField]private Button m_kBtnStop;
        [SerializeField]private UIGray m_kStrengthContinueGray;
        [SerializeField]private Text m_kTextStrengthContinue;
        [SerializeField]private Text m_kTextStrengthHint;
        [SerializeField]private GameObject m_kFrontGround;
        [SerializeField]private GameObject m_kStrengthBtnRoot;
        [SerializeField]private GeUISwitchButton geUISwitchButtonProtected;
        [SerializeField]private Image imgProtectedIcon;
        [SerializeField]private Image imgProtectedBackground;
        [SerializeField]private Button m_btnProtected;
        [SerializeField]private Button m_btnComLinkStrengthenProtected;
        [SerializeField]private Button m_btnSpecialStrength;
        [SerializeField]private Button m_btnStrength;
        [SerializeField]private Button m_btnStrengthContinue;
        [SerializeField]private Button m_btnStop;
        [SerializeField]private GameObject m_goStrengthenAttributesParent;
        [SerializeField]private GameObject m_goStrengthenAttributesPrefab;
        [SerializeField]private GameObject m_goCostContent;
        [SerializeField]private GameObject m_goProtectedContent;
        [SerializeField]private Text mName;
        [SerializeField]private Text mCurrentStrengthLevel;
        [SerializeField]private Text mTargetStrengthLevel;
        [SerializeField]private StateController m_kComStrengthenEffect;
        [SerializeField]private StateController mStrengthenDeviceStateControl;
        [SerializeField]private CommonTabToggleGroup mCommonTabToggleGroup;

        [Header("新改动")]
        [SerializeField] private GeUISwitchButton mStrengthenImplementGeUISwitchButton;
        [SerializeField] private ComUIListScript mStrengthenImplementUIList;
        [SerializeField] private ComUIListScript mStrengthenStampUIList;
        [SerializeField] private RectTransform mBottomRectTransform;
        /// <summary>
        /// 是否使用强化器
        /// </summary>
        private bool bIsUseStrengthenImplement = false;
        private int iContinueStrengthLevel = 0;

        private ComItemNew m_kCurrent;
        private Color32 colorGray = new Color32(255, 255, 255, 255);
        private ItemData m_kProtectedItemData = null;
        private bool m_bOnStart;
        private StrengthOperateResult m_eStrengthOperateResult = StrengthOperateResult.SOR_HAS_NO_ITEM;
        private int m_id0 = (int)ItemData.IncomeType.IT_UNCOLOR;
        private int m_id1 = (int)ItemData.IncomeType.IT_COLOR;
        private int m_id3 = ItemDataManager.GetInstance().GetMoneyIDByType(ProtoTable.ItemTable.eSubType.BindGOLD);
        private StrengthenCost m_costNeed;
        private int m_iTarget;
        private int m_iContinueTimes = 0;
        private const float delaySendContinueStrengthen = 0.7f;
        private const float delaySendStrengthen = 1.62f;
        private bool m_bUseProtected = false;
        private bool bSpecialStrengthen = false;
        private bool m_bNeedContinueStrengthGoldCostHint = false;
        private bool bContinueStrengthen;
        private int iLastStrengthenTargetLevel = 0;//最终设置的目标等级
        private const int iMaxStrengthenLevel = 10;//最大等级
        private int iMinStrengthenLevel = 0; //最小等级
        private int iStrengthenNextLevel = 0; //下级
        private bool m_bContinueStrengthen
        {
            set
            {
                bContinueStrengthen = value;
                StrengthenDataManager.GetInstance().IsStrengthenContinue = value;
            }
            get
            {
                return bContinueStrengthen;
            }
        }

        private CachedObjectDicManager<int, CostMaterialItem> m_akNeedCostMaterials = new CachedObjectDicManager<int, CostMaterialItem>();
        private CachedObjectDicManager<int, CostMaterialItem> m_akNeedCostMaterials1 = new CachedObjectDicManager<int, CostMaterialItem>();
        private List<StrengthenResult> m_akData = new List<StrengthenResult>();
        private SmithShopNewLinkData linkData;
        private StrengthenGrowthView mStrengthenGrowthView;
        private StrengthenGrowthType mType;
        private ItemData mCurrentSelectItemData;
        private ItemData mCurrentSelectStrengthStampItemData;//选择的强化券
        private ItemData mStrengthenDeviceItemData;//强化器道具
        private void Awake()
        {
            RegisterUIEvent();
        }

        private void OnDestroy()
        {
            UnRegisterUIEvent();
            ClearData();
        }

        private void ClearData()
        {
            _UnInitStrengthenAttribute();
            m_kProtectedItemData = null;
            m_akNeedCostMaterials.DestroyAllObjects();
            m_akNeedCostMaterials1.DestroyAllObjects();
            m_bContinueStrengthen = false;
            m_akData.Clear();
            
            InvokeMethod.RemoveInvokeCall(_DelaySendStrengthen);
            InvokeMethod.RemoveInvokeCall(_ContinueStrengthDelyInvoke);
            InvokeMethod.RemoveInvokeCall(this);

            m_kCurrent = null;
            linkData = null;
            mStrengthenGrowthView = null;
            mCurrentSelectItemData = null;
            iLastStrengthenTargetLevel = 0;
            mStrengthenDeviceItemData = null;
            bIsUseStrengthenImplement = false;
            iContinueStrengthLevel = 0;
            UnStrengthenImplementUIList();
            UnStrengthenStampUIList();
        }

        public sealed override void IniteData(SmithShopNewLinkData linkData, StrengthenGrowthType type, StrengthenGrowthView strengthenGrowthView)
        {
            this.linkData = linkData;
            mType = type;
            mStrengthenGrowthView = strengthenGrowthView;
            InitBaseInfo();
        }

        public sealed override void OnDisableView()
        {
            UnRegisterDelegateHandler();
        }

        public sealed override void OnEnableView()
        {
            RegisterDelegateHandler();
        }

        #region RegisterUIEvent

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

        private void RegisterUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnStrengthChanged, _OnStrengthChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ItemStrengthenSuccess, _OnItemStrengthenSuccess);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ItemStrengthenFail, _OnItemStrengthenFail);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.BeginContineStrengthen, _OnStartContinueStrengthen);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.EndContineStrengthen, _OnEndContinueStrengthen);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.IntterruptContineStrengthen, _OnInterruptContinueStrengthen);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.StrengthenDelay, _OnStrengthenDelay);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.StrengthenCanceled, _StrengthenCanceled);

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnSpecailStrenghthenStart, OnSpecailStrenghthenStart);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnSpecailStrenghthenCanceled, OnSpecailStrenghthenCanceled);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnSpecailStrenghthenFailed, OnSpecailStrenghthenFailed);

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnStrengthenError, OnStrengthenError);

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnEquipmentListNoEquipment, OnEquipmentListNoEquipment);

            StrengthenGrowthView.mOnStrengthenGrowthEquipItemClick += OnStrengthenGrowthEquipItemClick;

            RegisterDelegateHandler();
            PlayerBaseData.GetInstance().onMoneyChanged += OnMoneyChanged;

            //绑定键盘事件
            //UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnCommonKeyBoardInput, OnCommonKeyBoardInput);
        }

        private void UnRegisterUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnStrengthChanged, _OnStrengthChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ItemStrengthenSuccess, _OnItemStrengthenSuccess);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ItemStrengthenFail, _OnItemStrengthenFail);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.BeginContineStrengthen, _OnStartContinueStrengthen);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.EndContineStrengthen, _OnEndContinueStrengthen);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.IntterruptContineStrengthen, _OnInterruptContinueStrengthen);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.StrengthenDelay, _OnStrengthenDelay);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.StrengthenCanceled, _StrengthenCanceled);

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnSpecailStrenghthenStart, OnSpecailStrenghthenStart);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnSpecailStrenghthenCanceled, OnSpecailStrenghthenCanceled);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnSpecailStrenghthenFailed, OnSpecailStrenghthenFailed);

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnStrengthenError, OnStrengthenError);

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnEquipmentListNoEquipment, OnEquipmentListNoEquipment);

            StrengthenGrowthView.mOnStrengthenGrowthEquipItemClick -= OnStrengthenGrowthEquipItemClick;

            UnRegisterDelegateHandler();
            PlayerBaseData.GetInstance().onMoneyChanged -= OnMoneyChanged;

            //绑定键盘事件
            //UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnCommonKeyBoardInput, OnCommonKeyBoardInput);
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

            if (mCurrentSelectItemData.EquipType != EEquipType.ET_COMMON)
            {
                return false;
            }

            return true;
        }
        
        void _OnStrengthChanged(UIEvent uiEvent)
        {
            if (BCheckIsUpdateFrame() == false)
            {
                return;
            }
            
            if (m_bUseProtected == true)
            {
                int iProtectedNum = ItemDataManager.GetInstance().GetOwnedItemCount((int)ItemData.IncomeType.IT_PROTECTED);
                if (iProtectedNum <= 0)
                {
                    SystemNotifyManager.SysNotifyTextAnimation(TR.Value("failure_coupon"));
                    m_bOnStart = false;
                    geUISwitchButtonProtected.SetSwitch(false);
                    return;
                }
            }

            if (bIsUseStrengthenImplement == true)
            {
                //强化器为空
                if (mStrengthenDeviceItemData == null)
                {
                    m_bOnStart = false;
                    SystemNotifyManager.SysNotifyTextAnimation(TR.Value("strengthen_implement"));
                    return;
                }
            }

            if (m_kGray != null)
                m_kGray.enabled = true;
            if (m_kBtnStrength != null)
                m_kBtnStrength.enabled = false;
            if (m_kStrengthContinueGray != null)
                m_kStrengthContinueGray.enabled = true;
            if (m_btnStrengthContinue != null)
                m_btnStrengthContinue.enabled = false;
            if (m_kFrontGround != null)
                m_kFrontGround.CustomActive(true);

            if (mStrengthenGrowthView != null)
                mStrengthenGrowthView.OnSetMaskRoot(true);

            if (m_kBtnStop != null)
                m_kBtnStop.CustomActive(m_bOnStart || m_bContinueStrengthen);

            if(m_kStrengthBtnRoot != null)
            {
                m_kStrengthBtnRoot.CustomActive(false);
            }

            if (m_kComStrengthenEffect != null)
            {
                m_kComStrengthenEffect.Key = "play";
            }
        }

        void _OnItemStrengthenSuccess(UIEvent uiEvent)
        {
            if (BCheckIsUpdateFrame() == false)
            {
                return;
            }
            m_bOnStart = false;
            mCurrentSelectStrengthStampItemData = null;
            StrengthenResult result = StrengthenDataManager.GetInstance().GetStrengthenResult();
            _OnCloseStrengthEffect(uiEvent);

            {
                if (m_bContinueStrengthen)
                {
                    m_akData.Add(result);

                    if (mCurrentSelectItemData.StrengthenLevel >= iContinueStrengthLevel)
                    {
                        StrengthenResultUserData userData = new StrengthenResultUserData
                        {
                            uiCode = result.code,
                            EquipData = result.EquipData,
                            BrokenItems = result.BrokenItems,
                            bContinue = m_bContinueStrengthen
                        };
                        
                        _OnOpenStrengthenResultFrame(userData);

                        InvokeMethod.Invoke(2.0f, () =>
                        {
                            ClientSystemManager.GetInstance().CloseFrame<StrengthenResultFrame>(null, true);
                            _OnSucceedContinueStrengthen(m_akData);
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
                            bContinue = m_bContinueStrengthen
                        };
                        _OnOpenStrengthenResultFrame(userData);

                        InvokeMethod.Invoke(2.0f, () =>
                        {
                            ClientSystemManager.GetInstance().CloseFrame<StrengthenResultFrame>(null, true);
                            StrengthOperateResult eCurResult = StrengthOperateResult.SOR_OK;
                            _TryOpenNextContinueStrength(ref eCurResult, (StrengthOperateResult eResult, bool bStopByHand) =>
                            {
                                _OnStopContinueStrengthen(eResult, m_akData, bStopByHand);
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
                        bContinue = m_bContinueStrengthen,
                        NeedBeforeAnimation = bSpecialStrengthen
                    };
                    bSpecialStrengthen = false;
                    _OnOpenStrengthenResultFrame(userData);
                    if (m_kStrengthBtnRoot != null)
                    {
                        m_kStrengthBtnRoot.CustomActive(true);
                    }
                }
            }
            //);
        }

        void _OnCloseStrengthEffect(UIEvent uiEvent)
        {
            if (m_kComStrengthenEffect != null)
            {
                m_kComStrengthenEffect.Key = "stop";
            }
            
            if (m_kGray != null)
            {
                m_kGray.enabled = m_bContinueStrengthen;
            }
            if (m_kBtnStrength != null)
            {
                m_kBtnStrength.enabled = !m_bContinueStrengthen;
            }

            if (m_kStrengthContinueGray != null)
                m_kStrengthContinueGray.enabled = m_bContinueStrengthen;
            if (m_btnStrengthContinue != null)
                m_btnStrengthContinue.enabled = !m_bContinueStrengthen;

            m_kFrontGround.CustomActive(false);

            m_kStrengthBtnRoot.CustomActive(true);

            if (mStrengthenGrowthView != null)
                mStrengthenGrowthView.OnSetMaskRoot(false);
        }

        void _OnOpenStrengthenResultFrame(object userData)
        {
            ClientSystemManager.GetInstance().OpenFrame<StrengthenResultFrame>(FrameLayer.Middle, userData);
            mStrengthenGrowthView.RefreshAllEquipments();
        }

        void _OnSucceedContinueStrengthen(List<StrengthenResult> results)
        {
            var datas = new List<StrengthenResult>();
            datas.AddRange(results);
            StrengthenContinueResultsFrame.StrengthenContinueResultsFrameData sendData
                = new StrengthenContinueResultsFrame.StrengthenContinueResultsFrameData
                {
                    bStopByHandle = false,
                    eLastOpResult = StrengthOperateResult.SOR_OK,
                    iTarget = iContinueStrengthLevel,
                    results = datas
                };
            ClientSystemManager.GetInstance().OpenFrame<StrengthenContinueResultsFrame>(FrameLayer.Middle, sendData);

            m_bContinueStrengthen = false;
            _UpdateStrengthenMaterials(mCurrentSelectItemData == null ? null : mCurrentSelectItemData);
        }

        bool _TryOpenNextContinueStrength(ref StrengthOperateResult eResult, UnityEngine.Events.UnityAction<StrengthOperateResult, bool> onCancel)
        {
            if (!m_bContinueStrengthen)
            {
                return false;
            }

            m_bContinueStrengthen = false;
            int iCurLevel = mCurrentSelectItemData.StrengthenLevel;
            if (iCurLevel >= 10)
            {
                return false;
            }

            if (iCurLevel >= iContinueStrengthLevel)
            {
                return false;
            }

            if (bIsUseStrengthenImplement == true)
            {
                //强化器为空
                if (mStrengthenDeviceItemData == null)
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

            if (bIsUseStrengthenImplement == false)
            {
                eResult = Utility.CheckStrengthenItem(mCurrentSelectItemData, false);
                if (eResult != StrengthOperateResult.SOR_OK)
                {
                    if (onCancel != null)
                    {
                        onCancel.Invoke(eResult, false);
                    }
                    return false;
                }

                int iBindGoldId = ItemDataManager.GetInstance().GetMoneyIDByType(ItemTable.eSubType.BindGOLD);
                int iNeedGoldCount = _GetStrengthNeedMoney(mCurrentSelectItemData);
                int iHasBindGoldCount = ItemDataManager.GetInstance().GetOwnedItemCount(iBindGoldId, false);
                bool bBindGoldEnough = iNeedGoldCount <= iHasBindGoldCount;

                if (!m_bNeedContinueStrengthGoldCostHint || bBindGoldEnough)
                {
                    _ConfirmContinueStrength();
                }
                else
                {
                    CostItemManager.GetInstance().TryCostMoneyDefault(new CostItemManager.CostInfo { nMoneyID = iBindGoldId, nCount = iNeedGoldCount },
                        () =>
                        {
                            m_bNeedContinueStrengthGoldCostHint = false;
                            _ConfirmContinueStrength();
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
                _ConfirmContinueStrength();
            }
            
            return true;
        }

        void _ConfirmContinueStrength()
        {
            m_bContinueStrengthen = true;
            ++m_iContinueTimes;
            if (m_kTextStrengthHint != null)
            {
                m_kTextStrengthHint.text = string.Format(TR.Value("strengthen_dynamic_level"), iContinueStrengthLevel, m_iContinueTimes);
            }
            
            var systemValue = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)SystemValueTable.eType2.SVT_EQUIP_STRENGTH_CONTINUOUS_ANIM_TIME_DELAY);
            float delayTime = systemValue == null ? delaySendContinueStrengthen : (float)systemValue.Value / 1000;
            m_kStrengthBtnRoot.CustomActive(false);
            //启动强化特效
            _OnStrengthChanged(null);
            //发送强化消息
            InvokeMethod.RemoveInvokeCall(_ContinueStrengthDelyInvoke);
            InvokeMethod.Invoke(delayTime, _ContinueStrengthDelyInvoke);
        }

        void _ContinueStrengthDelyInvoke()
        {
            if (!m_bContinueStrengthen)
            {
                return;
            }

            if (null == mCurrentSelectItemData)
            {
                return;
            }

            //同时使用一次性强化器和保护券
            if (bIsUseStrengthenImplement == true && m_bUseProtected == true)
            {
                StrengthenDataManager.GetInstance().StrengthenItem(mCurrentSelectItemData, 3, mStrengthenDeviceItemData.GUID);
            }//使用一次性强化器
            else if (bIsUseStrengthenImplement == true)
            {
                StrengthenDataManager.GetInstance().StrengthenItem(mCurrentSelectItemData, 2, mStrengthenDeviceItemData.GUID);
            }
            else if (m_bUseProtected == true)
            {
                StrengthenDataManager.GetInstance().StrengthenItem(mCurrentSelectItemData, 1);
            }
            else
            {
                StrengthenDataManager.GetInstance().StrengthenItem(mCurrentSelectItemData, 0);
            }
        }

        void _OnStopContinueStrengthen(StrengthOperateResult eResult, List<StrengthenResult> results, bool bStopByHand)
        {
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
                        iTarget = iLastStrengthenTargetLevel,
                        results = datas
                    };
                ClientSystemManager.GetInstance().OpenFrame<StrengthenContinueResultsFrame>(FrameLayer.Middle, sendData);
            }
            
            m_bContinueStrengthen = false;
            m_iContinueTimes = 0;
            _UpdateStrengthenMaterials(mCurrentSelectItemData == null ? null : mCurrentSelectItemData);
        }

        void _OnItemStrengthenFail(UIEvent uiEvent)
        {
            if (BCheckIsUpdateFrame() == false)
            {
                return;
            }

            m_bOnStart = false;
            
            _OnCloseStrengthEffect(uiEvent);

            StrengthenResult result = StrengthenDataManager.GetInstance().GetStrengthenResult();
            StrengthenResultUserData userData = new StrengthenResultUserData
            {
                uiCode = result.code,
                EquipData = result.EquipData,
                BrokenItems = result.BrokenItems,
                bContinue = m_bContinueStrengthen
            };

            //如果装备强化破碎，选中破碎装备的下一件
            if (result.code == (uint)ProtoErrorCode.ITEM_STRENTH_FAIL_BROKE)
            {
                StrengthenDataManager.GetInstance().IsEquipmentStrengthBroken = true;
            }

            _OnOpenStrengthenResultFrame(userData);
            
            //装备强化损坏
            if (m_bContinueStrengthen)
            {
                m_akData.Add(result);
                InvokeMethod.Invoke(1.0f, () =>
                {
                    ClientSystemManager.GetInstance().CloseFrame<StrengthenResultFrame>(null, true);
                    StrengthOperateResult eCurResult = StrengthOperateResult.SOR_OK;
                    _TryOpenNextContinueStrength(ref eCurResult, (StrengthOperateResult eResult, bool bStopByHand) =>
                    {
                        _OnStopContinueStrengthen(eResult, m_akData, bStopByHand);
                        m_akData.Clear();
                    });
                });
            }
        }

        void _OnStartContinueStrengthen(UIEvent uiEvent)
        {
            if (BCheckIsUpdateFrame() == false)
            {
                return;
            }
            if (mCurrentSelectItemData == null)
            {
                return;
            }

            if (mCurrentSelectItemData.StrengthenLevel >= 10)
            {
                return;
            }

            m_bNeedContinueStrengthGoldCostHint = true;
            m_bContinueStrengthen = true;
            m_iContinueTimes = 0;
            StrengthOperateResult eCurResult = StrengthOperateResult.SOR_OK;
            _TryOpenNextContinueStrength(ref eCurResult, (StrengthOperateResult eResult, bool bStopByHand) =>
            {
                _OnStopContinueStrengthen(eResult, m_akData, bStopByHand);
                m_akData.Clear();
            });
        }

        void _OnEndContinueStrengthen(UIEvent uiEvent)
        {
            if (BCheckIsUpdateFrame() == false)
            {
                return;
            }
            m_iContinueTimes = 0;
            m_bContinueStrengthen = false;
            _OnCloseStrengthEffect(null);
            InvokeMethod.RemoveInvokeCall(_ContinueStrengthDelyInvoke);
            _UpdateStrengthenMaterials(mCurrentSelectItemData == null ? null : mCurrentSelectItemData);
        }

        void _OnInterruptContinueStrengthen(UIEvent uiEvent)
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
                        iTarget = iContinueStrengthLevel,
                        results = datas
                    };
                ClientSystemManager.GetInstance().OpenFrame<StrengthenContinueResultsFrame>(FrameLayer.Middle, sendData);
            }

            m_bContinueStrengthen = false;
            _OnCloseStrengthEffect(null);
            InvokeMethod.RemoveInvokeCall(_ContinueStrengthDelyInvoke);
            _UpdateStrengthenMaterials(mCurrentSelectItemData == null ? null : mCurrentSelectItemData);
        }

        void _OnStrengthenDelay(UIEvent uiEvent)
        {
            if (BCheckIsUpdateFrame() == false)
            {
                return;
            }
            
            var systemValue = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)SystemValueTable.eType2.SVT_EQUIP_STRENGTH_ANIM_TIME_DELAY);
            float delayTime = systemValue == null ? delaySendStrengthen : (float)systemValue.Value / 1000;
            InvokeMethod.RemoveInvokeCall(_DelaySendStrengthen);
            InvokeMethod.Invoke(delayTime, _DelaySendStrengthen);
        }

        void _StrengthenCanceled(UIEvent uiEvent)
        {
            if (BCheckIsUpdateFrame() == false)
            {
                return;
            }
            m_bOnStart = false;
            _UpdateStrengthenMaterials(mCurrentSelectItemData == null ? null : mCurrentSelectItemData);
        }

        void OnSpecailStrenghthenStart(UIEvent uiEvent)
        {
            if (BCheckIsUpdateFrame() == false)
            {
                return;
            }
            //fixed BUG0003152
            if (null == mCurrentSelectItemData || null == mCurrentSelectStrengthStampItemData)
            {
                return;
            }
            
            m_bOnStart = true;
            bSpecialStrengthen = true;

            var systemValue = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)SystemValueTable.eType2.SVT_EQUIP_STRENGTH_ANIM_TIME_DELAY);
            float delayTime = systemValue == null ? delaySendStrengthen : (float)systemValue.Value / 1000;
            //启动强化特效
            _OnStrengthChanged(null);
            m_kBtnStop.CustomActive(false);

            InvokeMethod.RemoveInvokeCall(OnDelaySendSpecailStrenghthen);
            InvokeMethod.Invoke(delayTime, OnDelaySendSpecailStrenghthen);
        }

        private void OnDelaySendSpecailStrenghthen()
        {
            if (null == mCurrentSelectItemData || null == mCurrentSelectStrengthStampItemData)
            {
                return;
            }

            StrengthenDataManager.GetInstance().StrengthenItem(mCurrentSelectItemData, 0, mCurrentSelectStrengthStampItemData.GUID);
        }

        void OnSpecailStrenghthenCanceled(UIEvent uiEvent)
        {
            if (BCheckIsUpdateFrame() == false)
            {
                return;
            }
            _OnCloseStrengthEffect(null);
            m_bOnStart = false;
        }
        
        void OnSpecailStrenghthenFailed(UIEvent uiEvent)
        {
            if (BCheckIsUpdateFrame() == false)
            {
                return;
            }
            OnSpecailStrenghthenCanceled(null);
            mCurrentSelectStrengthStampItemData = null;
            StrengthenResult result = StrengthenDataManager.GetInstance().GetStrengthenResult();
            StrengthenResultUserData userData = new StrengthenResultUserData
            {
                uiCode = result.code,
                EquipData = result.EquipData,
                BrokenItems = result.BrokenItems,
                bContinue = m_bContinueStrengthen,
                NeedBeforeAnimation = true
            };
            bSpecialStrengthen = false;
            _OnOpenStrengthenResultFrame(userData);
        }

        void OnStrengthenError(UIEvent uiEvent)
        {
            if (BCheckIsUpdateFrame() == false)
            {
                return;
            }
            _StrengthenCanceled(uiEvent);
            OnSpecailStrenghthenCanceled(uiEvent);
        }

        private void OnEquipmentListNoEquipment(UIEvent uiEvent)
        {
            mCurrentSelectItemData = null;
            _UpdateStrengthenMaterials(null);

            if (mName != null)
            {
                mName.text = string.Empty;
            }

            if (mCurrentStrengthLevel != null)
            {
                mCurrentStrengthLevel.text = string.Empty;
            }

            if (mTargetStrengthLevel != null)
            {
                mTargetStrengthLevel.text = string.Empty;
            }
        }
        
        void OnAddNewItem(List<Item> items)
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
                _TryUpdateMaterial((int)itemData.TableID);
            }
            
            if (bNeedRefreshEquipments)
            {
                mStrengthenGrowthView.RefreshAllEquipments();
            }

        }

        void OnRemoveItem(ItemData itemData)
        {
            if (mCurrentSelectItemData != null&& mCurrentSelectItemData.GUID == itemData.GUID)
            {
                if (!m_bOnStart && !m_bContinueStrengthen)
                {
                    mStrengthenGrowthView.RefreshAllEquipments();
                }
                    
            }

            _TryUpdateMaterial((int)itemData.TableID);
        }

        void OnUpdateItem(List<Item> items)
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
                    _TryUpdateMaterial((int)itemData.TableID);
                }
            }

        }

        private void OnMoneyChanged(PlayerBaseData.MoneyBinderType eTarget)
        {
            _UpdateStrengthenMaterials(null == mCurrentSelectItemData ? null : mCurrentSelectItemData);
        }
        #endregion

        #region InitBase 
        private void InitBaseInfo()
        {
            bIsUseStrengthenImplement = false;
            m_kCurrent = ComItemManager.CreateNew(goItemParent);
            
            m_kBtnStop.CustomActive(false);
            
            if(geUISwitchButtonProtected != null)
            {
                geUISwitchButtonProtected.onValueChanged.RemoveAllListeners();
                geUISwitchButtonProtected.onValueChanged.AddListener(_OnUseProtected);
            }
            
            if(mStrengthenImplementGeUISwitchButton != null)
            {
                mStrengthenImplementGeUISwitchButton.onValueChanged.RemoveAllListeners();
                mStrengthenImplementGeUISwitchButton.onValueChanged.AddListener(OnStrengthenImplementGeUISwitchButtonClick);
            }
            
            imgProtectedIcon.color = colorGray;
            m_goMaterialPrefab.CustomActive(false);

            if (m_btnProtected != null)
            {
                m_btnProtected.onClick.RemoveAllListeners();
                m_btnProtected.onClick.AddListener(OnClickProtected);
            }

            if (m_btnComLinkStrengthenProtected != null)
            {
                m_btnComLinkStrengthenProtected.onClick.RemoveAllListeners();
                m_btnComLinkStrengthenProtected.onClick.AddListener(_OnClickLinkToStrengthenProtected);
            }

            if (m_btnSpecialStrength != null)
            {
                m_btnSpecialStrength.onClick.RemoveAllListeners();
                m_btnSpecialStrength.onClick.AddListener(OnClickSpecialStrength);
            }

            if (m_btnStrength != null)
            {
                m_btnStrength.onClick.RemoveAllListeners();
                m_btnStrength.onClick.AddListener(OnClickStrength);
            }

            if (m_btnStrengthContinue != null)
            {
                m_btnStrengthContinue.onClick.RemoveAllListeners();
                m_btnStrengthContinue.onClick.AddListener(OnClickStrengthContinue);
            }

            if (m_btnStop != null)
            {
                m_btnStop.onClick.RemoveAllListeners();
                m_btnStop.onClick.AddListener(OnClickStop);
            }
            
            _InitStrengthenTabs();
            _InitStrengthenAttribute();

            var itemdata = ItemDataManager.CreateItemDataFromTable((int)ItemData.IncomeType.IT_PROTECTED);
            if(itemdata != null)
            {
                if (imgProtectedBackground != null)
                    ETCImageLoader.LoadSprite(ref imgProtectedBackground, itemdata.GetQualityInfo().Background);

                if (imgProtectedIcon != null)
                    ETCImageLoader.LoadSprite(ref imgProtectedIcon, itemdata.Icon);
            }
        }
        
        void OnClickProtected()
        {
            if (m_kProtectedItemData == null)
            {
                m_kProtectedItemData = GameClient.ItemDataManager.GetInstance().GetCommonItemTableDataByID((int)ItemData.IncomeType.IT_PROTECTED);
            }
            if (m_kProtectedItemData != null)
            {
                ItemTipManager.GetInstance().ShowTip(m_kProtectedItemData);
            }
        }

        void _OnClickLinkToStrengthenProtected()
        {
            ItemComeLink.OnLink((int)ItemData.IncomeType.IT_PROTECTED, 0);
        }

        void OnClickSpecialStrength()
        {
            if (m_bOnStart)
            {
                return;
            }
            
            if (mCurrentSelectItemData == null)
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("strengthen_item_not_enough"));
                return;
            }

            if (mCurrentSelectStrengthStampItemData == null)
            {
                var dataList = StrengthenDataManager.GetInstance().GetStrengthenStampList(mCurrentSelectItemData);
                if (dataList.Count > 0)
                {
                    SystemNotifyManager.SysNotifyTextAnimation(TR.Value("strengthen_special_need_protected"));
                }
                else
                {
                    ItemComeLink.OnLink(80, 0, false);
                }
                return;
            }

            int timeLeft;
            bool bStartCountdown;
            mCurrentSelectStrengthStampItemData.GetLimitTimeLeft(out timeLeft, out bStartCountdown);

            //失效了
            if (timeLeft <= 0 && bStartCountdown)
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("failure_item"));
                return;
            }

            StrengthenSpecialConfirmFrameData data = new StrengthenSpecialConfirmFrameData
            {
                equipData = mCurrentSelectItemData,
                itemData = mCurrentSelectStrengthStampItemData
            };
            ClientSystemManager.GetInstance().OpenFrame<StrengthenSpecialConfirmFrame>(FrameLayer.Middle, data);
        }

        void OnClickStrength()
        {
            if (mCurrentSelectItemData == null)
            {
                return;
            }

            if (mCurrentSelectItemData != null && mCurrentSelectItemData.bLocked)
            {
                SystemNotifyManager.SysNotifyFloatingEffect("已加锁的装备无法强化");
                return;
            }

            if (SecurityLockDataManager.GetInstance().CheckSecurityLock())
            {
                return;
            }

            _StrengthenEx();
        }

        void _StrengthenEx()
        {
            ClientSystemManager.GetInstance().CloseFrame<StrengthenResultFrame>(null, true);

            if (m_bOnStart)
            {
                return;
            }

            if (m_bUseProtected == true)
            {
                int iProtectedNum = ItemDataManager.GetInstance().GetOwnedItemCount((int)ItemData.IncomeType.IT_PROTECTED);
                if (iProtectedNum <= 0)
                {
                    SystemNotifyManager.SysNotifyTextAnimation(TR.Value("failure_coupon"));
                    m_bOnStart = false;
                    geUISwitchButtonProtected.SetSwitch(false);
                    return;
                }
            }

            if (bIsUseStrengthenImplement == true)
            {
                //强化器为空
                if (mStrengthenDeviceItemData == null)
                {
                    SystemNotifyManager.SysNotifyTextAnimation(TR.Value("strengthen_implement"));
                    m_bOnStart = false;
                    return;
                }
            }

            _UpdateStrengthenMaterials(mCurrentSelectItemData == null ? null : mCurrentSelectItemData);

            if (bIsUseStrengthenImplement == false)
            {
                if (m_eStrengthOperateResult != StrengthOperateResult.SOR_OK)
                {
                    switch (m_eStrengthOperateResult)
                    {
                        case StrengthOperateResult.SOR_COLOR:
                        case StrengthOperateResult.SOR_GOLD:
                        case StrengthOperateResult.SOR_UNCOLOR:
                            ActivityLimitTimeCombineManager.GetInstance().GiftDataManager.TryShowMallGift(MallGiftPackActivateCond.STRENGEN_NO_RESOURCE, () =>
                            {
                                _OnPopUpComeLink(m_eStrengthOperateResult);
                            });
                            break;
                        default:
                            _OnPopUpComeLink(m_eStrengthOperateResult);
                            break;
                    }
                    return;
                }

                int iBindGoldId = ItemDataManager.GetInstance().GetMoneyIDByType(ItemTable.eSubType.BindGOLD);
                int iNeedCount = _GetStrengthNeedMoney(mCurrentSelectItemData);
                CostItemManager.GetInstance().TryCostMoneyDefault(new CostItemManager.CostInfo { nMoneyID = iBindGoldId, nCount = iNeedCount }, _ConfirmStrength);
            }
            else
            {
                _ConfirmStrength();
            }
        }

        void OnClickStrengthContinue()
        {
            if (mCurrentSelectItemData != null && mCurrentSelectItemData.bLocked)
            {
                SystemNotifyManager.SysNotifyFloatingEffect("已加锁的装备无法强化");
                return;
            }

            ClientSystemManager.GetInstance().CloseFrame<StrengthenResultFrame>(null, true);

            if (SecurityLockDataManager.GetInstance().CheckSecurityLock())
            {
                return;
            }

            if (mCurrentSelectItemData == null)
            {
                return;
            }

            if (m_bContinueStrengthen)
            {
                return;
            }

            if (mCurrentSelectItemData.StrengthenLevel >= 10)
            {
                return;
            }

            if (bIsUseStrengthenImplement == true)
            {
                //强化器为空
                if (mStrengthenDeviceItemData == null)
                {
                    SystemNotifyManager.SysNotifyTextAnimation(TR.Value("strengthen_implement"));
                    return;
                }
            }

            if (bIsUseStrengthenImplement == false)
            {
                var eResult = Utility.CheckStrengthenItem(mCurrentSelectItemData, false);
                if (eResult != StrengthOperateResult.SOR_OK)
                {
                    switch (eResult)
                    {
                        case StrengthOperateResult.SOR_COLOR:
                        case StrengthOperateResult.SOR_GOLD:
                        case StrengthOperateResult.SOR_UNCOLOR:
                            ActivityLimitTimeCombineManager.GetInstance().GiftDataManager.TryShowMallGift(MallGiftPackActivateCond.STRENGEN_NO_RESOURCE, () =>
                            {
                                _OnPopUpComeLink(eResult);
                            });
                            break;
                        default:
                            _OnPopUpComeLink(eResult);
                            break;
                    }
                    return;
                }
            }
            
            m_akData.Clear();

            iContinueStrengthLevel = iLastStrengthenTargetLevel;

            StrengthenConfirmData data = new StrengthenConfirmData
            {
                ItemData = mCurrentSelectItemData,
                UseProtect = false,
                TargetStrengthenLevel = iContinueStrengthLevel
            };

            ClientSystemManager.GetInstance().OpenFrame<StrengthenContinueConfirm>(FrameLayer.Middle, data);
        }

        void OnClickStop()
        {
            if (m_bOnStart)
            {
                InvokeMethod.RemoveInvokeCall(_DelaySendStrengthen);
                m_bOnStart = false;
                _OnCloseStrengthEffect(null);
                _UpdateStrengthenMaterials(mCurrentSelectItemData == null ? null : mCurrentSelectItemData);
            }

            if (m_bContinueStrengthen)
            {
                _OnInterruptContinueStrengthen(null);
            }
            
            mStrengthenGrowthView.RefreshAllEquipments();

            m_kStrengthBtnRoot.CustomActive(true);
        }
        
        private void OnStrengthenStampItemClick(ItemData itemData)
        {
            if (itemData == null)
            {
                return;
            }

            mCurrentSelectStrengthStampItemData = itemData;
            
            var ticketTable = TableManager.GetInstance().GetTableItem<StrengthenTicketTable>(mCurrentSelectStrengthStampItemData.TableData.StrenTicketDataIndex);
            if (ticketTable != null)
            {
                int iTargetLevel = ticketTable.Level;
                _UpdateStrengthenAttribute(mCurrentSelectItemData, iTargetLevel);
            }
        }

        int _GetStrengthNeedMoney(ItemData itemData)
        {
            if (_GetCost(ref itemData, ref m_costNeed))
            {
                return m_costNeed.GoldCost;
            }
            return 0;
        }

        bool _GetCost(ref ItemData data, ref StrengthenCost costNeed)
        {
            if (StrengthenDataManager.GetInstance().GetCost(data.StrengthenLevel, data.LevelLimit, data.Quality, ref costNeed))
            {
                if (data.SubType == (int)ProtoTable.ItemTable.eSubType.WEAPON)
                {
                    float fRadio = 1.0f;
                    var SystemValueTableData = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_STRENGTH_WP_COST_MOD);
                    if (SystemValueTableData != null)
                    {
                        fRadio = SystemValueTableData.Value / 10.0f;
                    }

                    costNeed.ColorCost = Utility.ceil(costNeed.ColorCost * fRadio);
                    costNeed.UnColorCost = Utility.ceil(costNeed.UnColorCost * fRadio);
                    costNeed.GoldCost = Utility.ceil(costNeed.GoldCost * fRadio);
                }
                else if (data.SubType >= (int)ProtoTable.ItemTable.eSubType.HEAD && data.SubType <= (int)ProtoTable.ItemTable.eSubType.BOOT)
                {
                    float fRadio = 1.0f;
                    var SystemValueTableData = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_STRENGTH_DF_COST_MOD);
                    if (SystemValueTableData != null)
                    {
                        fRadio = SystemValueTableData.Value / 10.0f;
                    }

                    costNeed.ColorCost = Utility.ceil(costNeed.ColorCost * fRadio);
                    costNeed.UnColorCost = Utility.ceil(costNeed.UnColorCost * fRadio);
                    costNeed.GoldCost = Utility.ceil(costNeed.GoldCost * fRadio);
                }
                else if (data.SubType >= (int)ProtoTable.ItemTable.eSubType.RING && data.SubType <= (int)ProtoTable.ItemTable.eSubType.BRACELET)
                {
                    float fRadio = 1.0f;
                    var SystemValueTableData = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_STRENGTH_JW_COST_MOD);
                    if (SystemValueTableData != null)
                    {
                        fRadio = SystemValueTableData.Value / 10.0f;
                    }

                    costNeed.ColorCost = Utility.ceil(costNeed.ColorCost * fRadio);
                    costNeed.UnColorCost = Utility.ceil(costNeed.UnColorCost * fRadio);
                    costNeed.GoldCost = Utility.ceil(costNeed.GoldCost * fRadio);
                }
                return true;
            }
            return false;
        }

        void _OnPopUpComeLink(StrengthOperateResult eResult)
        {
            switch (eResult)
            {
                case StrengthOperateResult.SOR_UNCOLOR:
                    {
                        ItemComeLink.OnLink(m_id0, 0, true, CloseSmithShopNewFrame);
                    }
                    break;
                case StrengthOperateResult.SOR_COLOR:
                    {
                        ItemComeLink.OnLink(m_id1, 0, true, CloseSmithShopNewFrame);
                    }
                    break;
                case StrengthOperateResult.SOR_GOLD:
                    {
                        ItemComeLink.OnLink(m_id3, 0, true, CloseSmithShopNewFrame);
                    }
                    break;
                case StrengthOperateResult.SOR_HAS_NO_ITEM:
                    {
                        SystemNotifyManager.SysNotifyTextAnimation(TR.Value("strengthen_item_not_enough"));
                    }
                    break;
            }
        }

        private void CloseSmithShopNewFrame()
        {
            ClientSystemManager.GetInstance().CloseFrame<SmithShopNewFrame>(null, true);
        }

        private void _OnUseProtected(bool bUse)
        {
            if (!bUse)
            {
                m_bUseProtected = bUse;
                _UpdateStrengthenMaterials(mCurrentSelectItemData == null ? null : mCurrentSelectItemData);
                imgProtectedIcon.color = colorGray;
                goUnuseProtectedHint.CustomActive(true);
                return;
            }

            int iProtectedNum = ItemDataManager.GetInstance().GetOwnedItemCount((int)ItemData.IncomeType.IT_PROTECTED);
            if (iProtectedNum <= 0)
            {
                ItemComeLink.OnLink(200000310, 1, true, () =>
                {
                    geUISwitchButtonProtected.SetSwitch(true);
                    imgProtectedIcon.color = Color.white;
                    goUnuseProtectedHint.CustomActive(false);
                    _Ansy2();
                });
                geUISwitchButtonProtected.SetSwitch(false);
                imgProtectedIcon.color = colorGray;
                goUnuseProtectedHint.CustomActive(true);
                return;
            }

            _Ansy2();
        }

        private void _Ansy2()
        {
            if (mCurrentSelectItemData == null)
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("strengthen_item_not_enough"));
                geUISwitchButtonProtected.SetSwitch(false);
                imgProtectedIcon.color = colorGray;
                goUnuseProtectedHint.CustomActive(true);
                return;
            }


            if (mCurrentSelectItemData.StrengthenLevel < 10)
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("strengthen_lv_not_enough"));
                geUISwitchButtonProtected.SetSwitch(false);
                imgProtectedIcon.color = colorGray;
                goUnuseProtectedHint.CustomActive(true);
                return;
            }

            m_bUseProtected = true;
            imgProtectedIcon.color = Color.white;
            goUnuseProtectedHint.CustomActive(false);
            _UpdateStrengthenMaterials(mCurrentSelectItemData);
        }

        #region strengthenTabs
        enum StrengthenType
        {
            [System.ComponentModel.DescriptionAttribute("使用材料")]
            ST_COST_METERIAL = 0,
            [System.ComponentModel.DescriptionAttribute("使用强化券")]
            ST_PROTECTED_COUPON,
            ST_COUNT,
        }

        void _InitStrengthenTabs()
        {
            if(mCommonTabToggleGroup != null)
            {
                if(linkData != null)
                {
                    mCommonTabToggleGroup.InitComTab(OnStrengthenTabChanged, linkData.iSmithShopNewOpenTypeId);
                }
                else
                {
                    mCommonTabToggleGroup.InitComTab(OnStrengthenTabChanged, 0);
                }
            }
        }
        
        StrengthenType strengthenType = StrengthenType.ST_COUNT;

        void OnStrengthenTabChanged(CommonTabData commonTabData)
        {
            if(commonTabData == null)
            {
                return;
            }

            if ((StrengthenType)commonTabData.id == strengthenType)
                return;

            strengthenType = (StrengthenType)commonTabData.id;

            m_goCostContent.CustomActive(strengthenType == StrengthenType.ST_COST_METERIAL);
            m_goProtectedContent.CustomActive(strengthenType == StrengthenType.ST_PROTECTED_COUPON);

            if (strengthenType == StrengthenType.ST_COST_METERIAL)
            {
                _UpdateStrengthenMaterials(mCurrentSelectItemData);
            }
            else
            {
                RefreshStrengthenStampUIList();
            }
        }
        
        #endregion

        #endregion
        
        private void OnStrengthenGrowthEquipItemClick(ItemData itemData, StrengthenGrowthType eStrengthenGrowthType)
        {
            if (itemData == null)
            {
                return;
            }
            

            mCurrentSelectItemData = itemData;

            if (eStrengthenGrowthType == mType)
            {
                _UpdateStrengthenMaterials(itemData);
            }
        }

        void _TryUpdateMaterial(int iTableID)
        {
            var item = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(iTableID);
            if (item != null)
            {
                if (item.SubType == ProtoTable.ItemTable.eSubType.Coupon)
                {
                    RefreshStrengthenStampUIList();
                }
                else if (item.SubType == ProtoTable.ItemTable.eSubType.GOLD ||
                    item.SubType == ProtoTable.ItemTable.eSubType.BindGOLD ||
                    iTableID == (int)ItemData.IncomeType.IT_PROTECTED ||
                    iTableID == (int)ItemData.IncomeType.IT_UNCOLOR ||
                    iTableID == (int)ItemData.IncomeType.IT_COLOR ||
                    item.Type == ItemTable.eType.MATERIAL)
                {
                    _UpdateStrengthenMaterials(null == mCurrentSelectItemData ? null : mCurrentSelectItemData);
                }
            }
        }
   
        #region CouponItems
      
        /// <summary>
        /// 判断强化券是否在适用范围
        /// </summary>
        /// <param name="tableId"></param>
        /// <returns></returns>
        bool StrengthenStampScopeofApplication(int equipLevel, int tableId)
        {
            bool isFlag = false;
            //判断强化券适用范围 不满足弹提示
            var itemTable = TableManager.GetInstance().GetTableItem<ItemTable>(tableId);
            if (itemTable != null)
            {
                if (itemTable.LvAdaptation.Count >= 2)
                {
                    int minLevel = itemTable.LvAdaptation[0];
                    int maxLevel = itemTable.LvAdaptation[1];

                    if (equipLevel >= minLevel && equipLevel <= maxLevel)
                    {
                        isFlag = true;
                    }
                }
                else
                {
                    isFlag = true;
                }
            }

            return isFlag;
        }
        #endregion

        void OnItemClicked(GameObject obj, IItemDataModel item)
        {
            if (item != null)
            {
                ItemTipManager.GetInstance().ShowTip(item as ItemData);
            }
        }
        
        void _UpdateStrengthenMaterials(ItemData itemData)
        {
            // 此函数会被延迟调用，具体见 _OnItemStrengthenSuccess
            // 运行到到这里来的时候，界面可能已经关闭了,m_kCurrent已经为空了
            if (m_kCurrent == null)
            {
                return;
            }
            m_eStrengthOperateResult = StrengthOperateResult.SOR_HAS_NO_ITEM;
            
            if (itemData == null)
            {
                comStatus.Key = "null";
                m_kCurrent.Setup(null, null);
                m_akStrengthenAttributeItems.RecycleAllObject();
                mStrengthenImplementGeUISwitchButton.SetSwitch(false);
                mStrengthenImplementGeUISwitchButton.CustomActive(false);
            }
            else
            {
                //刷新强化券列表
                if(strengthenType == StrengthenType.ST_PROTECTED_COUPON)
                {
                    RefreshStrengthenStampUIList();
                }
                else
                {
                    if (mCurrentSelectStrengthStampItemData != null)
                        mCurrentSelectStrengthStampItemData = null;
                }

                //刷新连续强化信息

                if(itemData.StrengthenLevel >= 0 && itemData.StrengthenLevel < 5)
                {
                    iLastStrengthenTargetLevel = 5;
                }
                else if(itemData.StrengthenLevel >= 5 && itemData.StrengthenLevel < 8)
                {
                    iLastStrengthenTargetLevel = 8;
                }
                else if(itemData.StrengthenLevel >= 8 && itemData.StrengthenLevel < 10)
                {
                    iLastStrengthenTargetLevel = 10;
                }

                if (m_kTextStrengthContinue != null)
                    m_kTextStrengthContinue.text = TR.Value("strengthen_level_desc", iLastStrengthenTargetLevel);

                if (itemData.StrengthenLevel >= 20)
                {
                    mStrengthenImplementGeUISwitchButton.SetSwitch(false);
                    mStrengthenImplementGeUISwitchButton.CustomActive(false);
                }
                else
                {
                    mStrengthenImplementGeUISwitchButton.CustomActive(true);
                }
                
                if(mCurrentSelectStrengthStampItemData != null)
                {
                    var ticketTable = TableManager.GetInstance().GetTableItem<StrengthenTicketTable>(mCurrentSelectStrengthStampItemData.TableData.StrenTicketDataIndex);
                    if (ticketTable != null)
                    {
                        iMinStrengthenLevel = ticketTable.Level;
                    }
                }
                else
                {
                    iMinStrengthenLevel = itemData.StrengthenLevel + 1;
                }
               
                iStrengthenNextLevel = iMinStrengthenLevel;
                _UpdateStrengthenAttribute(itemData, iStrengthenNextLevel);
                m_kCurrent.Setup(itemData, OnItemClicked);
                mName.text = /*string.Format("强化+{0}", itemData.StrengthenLevel)*/itemData.GetColorName();

                if (bIsUseStrengthenImplement == false)
                {
                    if (itemData.StrengthenLevel >= 10)
                    {
                        mBottomRectTransform.anchoredPosition = new Vector2(mBottomRectTransform.anchoredPosition.x, -141);
                        comStatus.Key = "level>=10";
                    }
                    else
                    {
                        mBottomRectTransform.anchoredPosition = new Vector2(mBottomRectTransform.anchoredPosition.x, -296);
                        comStatus.Key = "level<10";
                    }
                }
            }

            UpdateStrengthDeviceItem();

            if (m_bOnStart || m_bContinueStrengthen || itemData == null)
            {
                m_kBtnStrength.enabled = false;
                m_kGray.enabled = true;

                if (m_kStrengthContinueGray != null)
                    m_kStrengthContinueGray.enabled = true;
                if (m_btnStrengthContinue != null)
                    m_btnStrengthContinue.enabled = false;
            }
            else
            {
                m_kBtnStrength.enabled = true;
                m_kGray.enabled = false;

                if (m_kStrengthContinueGray != null)
                    m_kStrengthContinueGray.enabled = false;
                if (m_btnStrengthContinue != null)
                    m_btnStrengthContinue.enabled = true;
            }
            
            int iHasCount0 = ItemDataManager.GetInstance().GetOwnedItemCount(m_id0);
            int iHasCount1 = ItemDataManager.GetInstance().GetOwnedItemCount(m_id1);
            int iHasCount2 = ItemDataManager.GetInstance().GetOwnedItemCount(m_id3);
            int iProtectedNum = ItemDataManager.GetInstance().GetOwnedItemCount((int)ItemData.IncomeType.IT_PROTECTED);

            if (iProtectedNum >= 1)
            {
                m_kTextProtectedHint.text = string.Format(TR.Value("strength_protected_enough"), iProtectedNum);
            }
            else
            {
                m_kTextProtectedHint.text = string.Format(TR.Value("strength_protected_not_enough"), iProtectedNum);
                geUISwitchButtonProtected.SetSwitch(false);
            }

            if (iProtectedNum >= 1 && itemData != null && itemData.StrengthenLevel < 10 && m_bUseProtected)
            {
                geUISwitchButtonProtected.SetSwitch(false);
            }
            
            if (!m_bContinueStrengthen)
            {
                m_kTextStrengthHint.text = TR.Value("strengthen_fixed_level");
            }
            else
            {
                m_kTextStrengthHint.text = string.Format(TR.Value("strengthen_dynamic_level"), iContinueStrengthLevel, m_iContinueTimes);
            }
            
            if (bIsUseStrengthenImplement == false)
            {
                m_akNeedCostMaterials.RecycleAllObject();
                m_akNeedCostMaterials1.RecycleAllObject();
                if (itemData == null)
                {
                    var materials = new int[] { m_id0, m_id1, m_id3 };
                    for (int i = 0; i < materials.Length; ++i)
                    {
                        var materialData = ItemDataManager.GetInstance().GetCommonItemTableDataByID(materials[i]);
                        if (materialData != null)
                        {
                            if (m_akNeedCostMaterials.HasObject(materials[i]))
                            {
                                m_akNeedCostMaterials.RefreshObject(materials[i], new object[] { materialData, true, 0 });
                            }
                            else
                            {
                                m_akNeedCostMaterials.Create(materials[i], new object[] { m_goMaterialParent, m_goMaterialPrefab, materialData, this, true, 0 });
                            }

                            if (m_akNeedCostMaterials1.HasObject(materials[i]))
                            {
                                m_akNeedCostMaterials1.RefreshObject(materials[i], new object[] { materialData, true, 0 });
                            }
                            else
                            {
                                m_akNeedCostMaterials1.Create(materials[i], new object[] { m_goMaterialParent1, m_goMaterialPrefab1, materialData, this, true, 0 });
                            }
                        }
                    }
                    m_akNeedCostMaterials.Filter(null);
                    m_akNeedCostMaterials1.Filter(null);
                }


                if (itemData != null)
                {
                    m_eStrengthOperateResult = StrengthOperateResult.SOR_OK;
                    if (_GetCost(ref itemData, ref m_costNeed))
                    {
                        var materials = new int[] { m_id0, m_id1, m_id3 };
                        var material_counts = new int[] { m_costNeed.UnColorCost, m_costNeed.ColorCost, m_costNeed.GoldCost };
                        for (int i = 0; i < materials.Length; ++i)
                        {
                            var materialData = ItemDataManager.GetInstance().GetCommonItemTableDataByID(materials[i]);
                            if (materialData != null)
                            {
                                if (m_akNeedCostMaterials.HasObject(materials[i]))
                                {
                                    m_akNeedCostMaterials.RefreshObject(materials[i], new object[] { materialData, false, material_counts[i] });
                                }
                                else
                                {
                                    m_akNeedCostMaterials.Create(materials[i], new object[] { m_goMaterialParent, m_goMaterialPrefab, materialData, this, false, material_counts[i] });
                                }

                                if (m_akNeedCostMaterials1.HasObject(materials[i]))
                                {
                                    m_akNeedCostMaterials1.RefreshObject(materials[i], new object[] { materialData, false, material_counts[i] });
                                }
                                else
                                {
                                    m_akNeedCostMaterials1.Create(materials[i], new object[] { m_goMaterialParent1, m_goMaterialPrefab1, materialData, this, false, material_counts[i] });
                                }
                            }
                        }
                        m_akNeedCostMaterials.Filter(null);
                        m_akNeedCostMaterials1.Filter(null);

                        if (m_costNeed.UnColorCost > 0 && m_eStrengthOperateResult == StrengthOperateResult.SOR_OK)
                        {
                            int iHasUnColor = ItemDataManager.GetInstance().GetOwnedItemCount(m_id0);
                            if (iHasUnColor < m_costNeed.UnColorCost)
                            {
                                m_eStrengthOperateResult = StrengthOperateResult.SOR_UNCOLOR;
                            }
                        }
                        if (m_costNeed.ColorCost > 0 && m_eStrengthOperateResult == StrengthOperateResult.SOR_OK)
                        {
                            int iHasColor = ItemDataManager.GetInstance().GetOwnedItemCount(m_id1);
                            if (iHasColor < m_costNeed.ColorCost)
                            {
                                m_eStrengthOperateResult = StrengthOperateResult.SOR_COLOR;
                            }
                        }
                        if (m_costNeed.GoldCost > 0 && m_eStrengthOperateResult == StrengthOperateResult.SOR_OK)
                        {
                            int iHasGold = ItemDataManager.GetInstance().GetOwnedItemCount(m_id3);
                            if (iHasGold < m_costNeed.GoldCost)
                            {
                                m_eStrengthOperateResult = StrengthOperateResult.SOR_GOLD;
                            }
                        }
                    }
                    else
                    {
                        m_eStrengthOperateResult = StrengthOperateResult.SOR_HAS_NO_ITEM;
                    }
                }
                else
                {
                    m_eStrengthOperateResult = StrengthOperateResult.SOR_HAS_NO_ITEM;
                }
            }

            int iHasCount = ItemDataManager.GetInstance().GetOwnedItemCount((int)ItemData.IncomeType.IT_PROTECTED);
            goProectedComeLink.CustomActive(iHasCount <= 0);

            comProtectedStatus.Key = m_bUseProtected ? "use" : "unuse";
            
            goUnuseProtectedHint.CustomActive(!m_bUseProtected);
        }


        #region CostMaterialItem
        public sealed class CostMaterialItem : CachedObject
        {
            GameObject goLocal;
            GameObject goPrefab;
            GameObject goParent;
            GameObject goAcquired;
            Button btnAcquired;
            int iNeedCount;
            bool bForceShow;
            ItemData itemData;
            public ItemData ItemData
            {
                get { return itemData; }
            }
            StrengthenView frame;

            GameObject itemParent;
            Text name;
            Text count;
            ComItem comItem;

            public override void OnDestroy()
            {
                comItem.Setup(null, null);
                comItem = null;
                itemData = null;
            }

            public override void OnCreate(object[] param)
            {
                goParent = param[0] as GameObject;
                goPrefab = param[1] as GameObject;
                itemData = param[2] as ItemData;
                frame = param[3] as StrengthenView;
                bForceShow = (bool)param[4];
                iNeedCount = (int)param[5];
                if (goPrefab == null) return;
                if (goLocal == null)
                {
                    goLocal = GameObject.Instantiate(goPrefab);
                    Utility.AttachTo(goLocal, goParent);

                    name = Utility.FindComponent<Text>(goLocal, "Name");
                    itemParent = Utility.FindGameObject(goLocal, "Item");
                    if(itemParent == null)
                    {
                        comItem = ComItemManager.Create(goLocal);
                    }
                    else
                    {
                        comItem = ComItemManager.Create(itemParent);
                    }
                    comItem.gameObject.transform.SetAsFirstSibling();
                    count = Utility.FindComponent<Text>(goLocal, "Count");
                    goAcquired = Utility.FindChild(goLocal, "ItemComLink");
                    btnAcquired = Utility.FindComponent<Button>(goLocal, "ItemComLink");
                    btnAcquired.onClick.RemoveAllListeners();
                    btnAcquired.onClick.AddListener(() =>
                    {
                        if (null != itemData)
                        {
                            ItemComeLink.OnLink(itemData.TableID, 0, true, () => { ClientSystemManager.GetInstance().CloseFrame<SmithShopNewFrame>(null, true); });
                        }
                    });
                }
                Enable();
                SetAsLastSibling();
                _Update();
            }
            public override void SetAsLastSibling()
            {
                if (goLocal != null)
                {
                    goLocal.transform.SetAsLastSibling();
                }
            }
            public override void OnRecycle()
            {
                if (goLocal != null)
                {
                    goLocal.CustomActive(false);
                }
            }
            public override void OnDecycle(object[] param)
            {
                OnCreate(param);
            }
            public override void OnRefresh(object[] param)
            {
                itemData = param[0] as ItemData;
                bForceShow = (bool)param[1];
                iNeedCount = (int)param[2];
                _Update();
            }
            public override void Enable()
            {
                if (goLocal != null)
                {
                    goLocal.CustomActive(true);
                }
            }

            public void SetAsFirstSibling()
            {
                if (goLocal != null)
                {
                    goLocal.transform.SetAsFirstSibling();
                }
            }

            public override void Disable()
            {
                if (goLocal != null)
                {
                    goLocal.CustomActive(false);
                }
            }
            public override bool NeedFilter(object[] param)
            {
                if (bForceShow)
                {
                    return false;
                }

                return iNeedCount <= 0;
            }

            void OnItemClicked(GameObject obj, ItemData item)
            {
                if (item != null)
                {
                    ItemTipManager.GetInstance().ShowTip(item);
                }
            }

            void _Update()
            {
                comItem.Setup(itemData, OnItemClicked);
                name.text = itemData.GetColorName();
                int iHasCount = ItemDataManager.GetInstance().GetOwnedItemCount((int)itemData.TableID);
                if (itemData.Type == ProtoTable.ItemTable.eType.INCOME)
                {
                    count.text = string.Format("{0}", iNeedCount);
                }
                else
                {
                    count.text = string.Format("{0}/{1}", iHasCount, iNeedCount);
                }

                if (iHasCount < iNeedCount && iNeedCount > 0)
                {
                    count.color = Color.red;
                }
                else
                {
                    count.color = Color.white;
                }
                if (itemData != null)
                {
                    itemData.Count = 1;
                }
                goLocal.name = itemData.TableID.ToString();

                goAcquired.CustomActive(iHasCount < iNeedCount && iNeedCount > 0);

                comItem.SetShowNotEnoughState(goAcquired.activeSelf);
            }
        }
        #endregion

        #region StrengthenAttributes

        sealed class StrengthenAttributeItem : CachedObject
        {
            GameObject goLocal;
            GameObject goPrefab;
            GameObject goParent;
            StrengthenView frame;
            StrengthenAttributeItemData dataA = null;
            StrengthenAttributeItemData dataB = null;

            Text fixedDesc = null;
            Text curValue = null;
            Text nextValue = null;

            public override void OnDestroy()
            {

            }

            public override void OnCreate(object[] param)
            {
                goParent = param[0] as GameObject;
                goPrefab = param[1] as GameObject;
                dataA = param[2] as StrengthenAttributeItemData;
                dataB = param[3] as StrengthenAttributeItemData;
                frame = param[4] as StrengthenView;
                if (goPrefab == null) return;
                if (goLocal == null)
                {
                    goLocal = GameObject.Instantiate(goPrefab);
                    Utility.AttachTo(goLocal, goParent);

                    fixedDesc = Utility.FindComponent<Text>(goLocal, "Prefixed");
                    curValue = Utility.FindComponent<Text>(goLocal, "Value1");
                    nextValue = Utility.FindComponent<Text>(goLocal, "Value2");
                }
                Enable();
                SetAsLastSibling();
                _Update();
            }

            public override void SetAsLastSibling()
            {
                if (goLocal != null)
                {
                    goLocal.transform.SetAsLastSibling();
                }
            }

            public override void OnRecycle()
            {
                if (goLocal != null)
                {
                    goLocal.CustomActive(false);
                }
            }
            public override void OnDecycle(object[] param)
            {
                OnCreate(param);
            }
            public override void OnRefresh(object[] param)
            {
                if (param != null && param.Length == 2)
                {
                    dataA = param[0] as StrengthenAttributeItemData;
                    dataB = param[1] as StrengthenAttributeItemData;
                }
                _Update();
            }
            public override void Enable()
            {
                if (goLocal != null)
                {
                    goLocal.CustomActive(true);
                }
            }

            public void SetAsFirstSibling()
            {
                if (goLocal != null)
                {
                    goLocal.transform.SetAsFirstSibling();
                }
            }

            public override void Disable()
            {
                if (goLocal != null)
                {
                    goLocal.CustomActive(false);
                }
            }
            public override bool NeedFilter(object[] param)
            {
                return false;
            }

            void _Update()
            {
                if (dataA != null && dataB != null)
                {
                    fixedDesc.text = dataA.kDesc;
                    curValue.text = string.Format("+{0}", dataA.ToValueDesc());
                    nextValue.text = string.Format("+{0}", dataB.ToValueDesc());
                }
            }
        }
        CachedObjectListManager<StrengthenAttributeItem> m_akStrengthenAttributeItems = new CachedObjectListManager<StrengthenAttributeItem>();
        void _InitStrengthenAttribute()
        {
            m_goStrengthenAttributesPrefab.CustomActive(false);
        }

        void _UpdateStrengthenAttribute(ItemData itemData,int nextStrengthenLevel)
        {
            if (m_akStrengthenAttributeItems != null)
                m_akStrengthenAttributeItems.RecycleAllObject();
            if (itemData != null)
            {
                if (mCurrentStrengthLevel != null)
                    mCurrentStrengthLevel.text = string.Format("强化+{0}", itemData.StrengthenLevel);
                if (mTargetStrengthLevel != null)
                    mTargetStrengthLevel.text = string.Format("强化+{0}", nextStrengthenLevel);

                //如果是辅助装备
                if (itemData.IsAssistEquip())
                {
                    List<StrengthenAttributeItemData> itemStrengthenAttrA = new List<StrengthenAttributeItemData>();
                    List<StrengthenAttributeItemData> itemStrengthenAttrB = new List<StrengthenAttributeItemData>();
                    float attrAValue = StrengthenDataManager.GetInstance().GetAssistEqStrengthAttrValue(itemData, itemData.StrengthenLevel);
                    float attrBValue = StrengthenDataManager.GetInstance().GetAssistEqStrengthAttrValue(itemData, nextStrengthenLevel);
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

                        itemStrengthenAttrA.Add(dataA);
                        itemStrengthenAttrB.Add(dataB);
                    }

                    if (itemStrengthenAttrA.Count == itemStrengthenAttrB.Count)
                    {
                        for (int i = 0; i < itemStrengthenAttrA.Count; i++)
                        {
                            m_akStrengthenAttributeItems.Create(new object[] { m_goStrengthenAttributesParent, m_goStrengthenAttributesPrefab, itemStrengthenAttrA[i], itemStrengthenAttrB[i], this });
                        }
                    }
                }
                else
                {
                    var itemStrengthenAttrA = ItemStrengthAttribute.Create(itemData.TableID);
                    var itemStrengthenAttrB = ItemStrengthAttribute.Create(itemData.TableID);
                    if (itemStrengthenAttrA != null && itemStrengthenAttrB != null)
                    {
                        itemStrengthenAttrA.SetStrength(itemData.StrengthenLevel);
                        itemStrengthenAttrB.SetStrength(nextStrengthenLevel);

                        if (itemStrengthenAttrA.Attributes.Count == itemStrengthenAttrB.Attributes.Count)
                        {
                            for (int i = 0; i < itemStrengthenAttrA.Attributes.Count; ++i)
                            {
                                m_akStrengthenAttributeItems.Create(new object[] { m_goStrengthenAttributesParent, m_goStrengthenAttributesPrefab, itemStrengthenAttrA.Attributes[i], itemStrengthenAttrB.Attributes[i], this });
                            }
                        }
                    }
                }
            }
        }

        void _UnInitStrengthenAttribute()
        {
            m_akStrengthenAttributeItems.DestroyAllObjects();
        }
        #endregion

        void _ConfirmStrength()
        {
            m_bOnStart = true;
            if (mCurrentSelectItemData.StrengthenLevel < 10)
            {
                _OnStrengthChanged(null);
                _OnStrengthenDelay(null);
            }
            else
            {
                StrengthenConfirmData data = new StrengthenConfirmData
                {
                    ItemData = mCurrentSelectItemData,
                    UseProtect = m_bUseProtected,
                    UseStrengthenImplement = bIsUseStrengthenImplement,
                    StrengthenImplementItem = mStrengthenDeviceItemData
                };
                data.TargetStrengthenLevel = data.ItemData.StrengthenLevel;


                ClientSystemManager.GetInstance().OpenFrame<StrengthenConfirm>(FrameLayer.Middle, data);
            }
        }
        
        void _DelaySendStrengthen()
        {
            if (m_bOnStart)
            {
                if (mCurrentSelectItemData != null)
                {
                    //同时使用一次性强化器和保护券
                    if (bIsUseStrengthenImplement == true && m_bUseProtected == true && mStrengthenDeviceItemData != null)
                    {
                        StrengthenDataManager.GetInstance().StrengthenItem(mCurrentSelectItemData, 3, mStrengthenDeviceItemData.GUID);
                    }//使用一次性强化器
                    else if (bIsUseStrengthenImplement == true && mStrengthenDeviceItemData != null)
                    {
                        StrengthenDataManager.GetInstance().StrengthenItem(mCurrentSelectItemData, 2, mStrengthenDeviceItemData.GUID);
                    }
                    else if (m_bUseProtected == true)
                    {
                        StrengthenDataManager.GetInstance().StrengthenItem(mCurrentSelectItemData, 1);
                    }
                    else
                    {
                        StrengthenDataManager.GetInstance().StrengthenItem(mCurrentSelectItemData, 0);
                    }
                }
            }
        }
        

        #region StrengthenDevice
        
        private void UpdateStrengthDeviceItem()
        {
            if (bIsUseStrengthenImplement == true)
            {
                mStrengthenDeviceStateControl.Key = "true";
                RefreshStrengthenImplementUIList();
            }
            else
            {
                mStrengthenDeviceStateControl.Key = "false";
            }
        }

        #endregion


        #region 新改动

        private void OnStrengthenImplementGeUISwitchButtonClick(bool value)
        {
            if (bIsUseStrengthenImplement == value)
                return;

            bIsUseStrengthenImplement = value;

            if (value == true)
            {
                mStrengthenDeviceStateControl.Key = "true";
                RefreshStrengthenImplementUIList();
            }
            else
            {
                mStrengthenDeviceStateControl.Key = "false";
                _UpdateStrengthenMaterials(mCurrentSelectItemData == null ? null : mCurrentSelectItemData);
            }
        }

        List<ulong> StrengthenImplementItems = new List<ulong>();
        private void RefreshStrengthenImplementUIList()
        {
            StrengthenImplementItems = ItemDataManager.GetInstance().GetItemsByPackageThirdType(EPackageType.Material, ItemTable.eThirdType.DisposableStrengItem);

            if(mStrengthenImplementUIList != null)
            {
                if (mStrengthenImplementUIList.IsInitialised())
                {
                    
                }
                else
                {
                    mStrengthenImplementUIList.Initialize();
                    mStrengthenImplementUIList.onBindItem += OnBindStrengthenImplementItemDelegate;
                    mStrengthenImplementUIList.onItemVisiable += OnStrengthenImplementItemVisiableDelegate;
                    mStrengthenImplementUIList.onItemSelected += OnStrengthenImplementItemSelectedDelegate;
                    mStrengthenImplementUIList.onItemChageDisplay += OnStrengthenImplementItemChangeDisplayDelegate;
                }

                if (bIsUseStrengthenImplement && mStrengthenDeviceItemData != null)
                {
                    bool isFind = false;
                    for (int i = 0; i < StrengthenImplementItems.Count; i++)
                    {
                        var itemData = ItemDataManager.GetInstance().GetItem(StrengthenImplementItems[i]);
                        if (itemData == null)
                        {
                            continue;
                        }

                        if (itemData.GUID != mStrengthenDeviceItemData.GUID)
                        {
                            continue;
                        }

                        isFind = true;
                        break;
                    }

                    if (!isFind)
                    {
                        mStrengthenDeviceItemData = null;
                    }

                    if (mStrengthenDeviceItemData == null)
                    {
                        mStrengthenImplementUIList.ResetSelectedElementIndex();
                    }
                }
                else
                {
                    mStrengthenImplementUIList.ResetSelectedElementIndex();
                }
                
                mStrengthenImplementUIList.SetElementAmount(StrengthenImplementItems.Count);
            }
          
        }

        private void UnStrengthenImplementUIList()
        {
            if(mStrengthenImplementUIList != null && mStrengthenImplementUIList.IsInitialised())
            {
                mStrengthenImplementUIList.onBindItem -= OnBindStrengthenImplementItemDelegate;
                mStrengthenImplementUIList.onItemVisiable -= OnStrengthenImplementItemVisiableDelegate;
                mStrengthenImplementUIList.onItemSelected -= OnStrengthenImplementItemSelectedDelegate;
                mStrengthenImplementUIList.onItemChageDisplay -= OnStrengthenImplementItemChangeDisplayDelegate;
            }
        }

        private CommonImplementItem OnBindStrengthenImplementItemDelegate(GameObject itemObject)
        {
            return itemObject.GetComponent<CommonImplementItem>();
        }

        private void OnStrengthenImplementItemVisiableDelegate(ComUIListElementScript item)
        {
            CommonImplementItem commonImplementItem = item.gameObjectBindScript as CommonImplementItem;
            if(commonImplementItem != null && item.m_index >= 0 && item.m_index < StrengthenImplementItems.Count)
            {
                ulong guid = StrengthenImplementItems[item.m_index];
                commonImplementItem.OnItemVisiable(guid);

                if(mStrengthenDeviceItemData != null)
                {
                    commonImplementItem.OnChangeDisplay(guid == mStrengthenDeviceItemData.GUID);
                }
            }
        }

        private void OnStrengthenImplementItemSelectedDelegate(ComUIListElementScript item)
        {
            CommonImplementItem commonImplementItem = item.gameObjectBindScript as CommonImplementItem;
            if(commonImplementItem != null)
            {
                mStrengthenDeviceItemData = commonImplementItem.ItemData;
            }
        }

        private void OnStrengthenImplementItemChangeDisplayDelegate(ComUIListElementScript item,bool bSelected)
        {
            CommonImplementItem commonImplementItem = item.gameObjectBindScript as CommonImplementItem;
            if (commonImplementItem != null)
                commonImplementItem.OnChangeDisplay(bSelected);
        }

        List<ItemData> StrengthenStampItems = new List<ItemData>();
        private void RefreshStrengthenStampUIList()
        {
            StrengthenStampItems = StrengthenDataManager.GetInstance().GetStrengthenStampList(mCurrentSelectItemData);

            if (mStrengthenStampUIList != null)
            {
                if(mStrengthenStampUIList.IsInitialised())
                {

                }
                else
                {
                    mStrengthenStampUIList.Initialize();
                    mStrengthenStampUIList.onBindItem += OnBindStrengthenStampItemDelegate;
                    mStrengthenStampUIList.onItemVisiable += OnStrengthenStampItemVisiableDelegate;
                    mStrengthenStampUIList.onItemSelected += OnStrengthenStampItemSelectedDelegate;
                    mStrengthenStampUIList.onItemChageDisplay += OnStrengthenStampItemChangeDisplayDelegate;
                }

                mStrengthenStampUIList.ResetSelectedElementIndex();
                mStrengthenStampUIList.SetElementAmount(StrengthenStampItems.Count);

                if (mCurrentSelectStrengthStampItemData != null)
                {
                    bool isFind = false;
                    int index = -1;
                    for (int i = 0; i < StrengthenStampItems.Count; i++)
                    {
                        var data = StrengthenStampItems[i];
                        if (data == null)
                            continue;

                        if (mCurrentSelectStrengthStampItemData.GUID != data.GUID)
                            continue;

                        index = i;
                        isFind = true;
                        break;
                    }

                    if (!isFind)
                    {
                        mCurrentSelectStrengthStampItemData = null;
                    }
                    else
                    {
                        if (!mStrengthenStampUIList.IsElementInScrollArea(index))
                        {
                            mStrengthenStampUIList.EnsureElementVisable(index);
                        }

                        mStrengthenStampUIList.SelectElement(index);
                    }
                }
            }
        }

        private void UnStrengthenStampUIList()
        {
            if(mStrengthenStampUIList != null && mStrengthenStampUIList.IsInitialised())
            {
                mStrengthenStampUIList.onBindItem -= OnBindStrengthenStampItemDelegate;
                mStrengthenStampUIList.onItemVisiable -= OnStrengthenStampItemVisiableDelegate;
                mStrengthenStampUIList.onItemSelected -= OnStrengthenStampItemSelectedDelegate;
                mStrengthenStampUIList.onItemChageDisplay -= OnStrengthenStampItemChangeDisplayDelegate;
            }
        }

        private CommonImplementItem OnBindStrengthenStampItemDelegate(GameObject itemObject)
        {
            return itemObject.GetComponent<CommonImplementItem>();
        }

        private void OnStrengthenStampItemVisiableDelegate(ComUIListElementScript item)
        {
            CommonImplementItem commonImplementItem = item.gameObjectBindScript as CommonImplementItem;
            if(commonImplementItem != null && item.m_index >= 0 && item.m_index < StrengthenStampItems.Count)
            {
                ItemData itemData = StrengthenStampItems[item.m_index];
                commonImplementItem.OnItemVisiable(itemData.GUID);
            }
        }

        private void OnStrengthenStampItemSelectedDelegate(ComUIListElementScript item)
        {
            CommonImplementItem commonImplementItem = item.gameObjectBindScript as CommonImplementItem;
            if(commonImplementItem != null)
            {
                OnStrengthenStampItemClick(commonImplementItem.ItemData);
            }
        }

        private void OnStrengthenStampItemChangeDisplayDelegate(ComUIListElementScript item, bool bSelected)
        {
            CommonImplementItem commonImplementItem = item.gameObjectBindScript as CommonImplementItem;
            if (commonImplementItem != null)
            {
                commonImplementItem.OnChangeDisplay(bSelected);
            }
        }
        #endregion
    }
}