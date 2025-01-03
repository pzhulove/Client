using System;
using System.Collections.Generic;
using LimitTimeGift;
using Protocol;
using ProtoTable;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{

    public class MallNewPropertyMallElementItem : MonoBehaviour
    {

        private MallItemInfo _mallItemInfo = null;

        private bool _isPlayerLimit = false;        //角色限购
        private string _playerLimitDescriptionStr = ""; //角色限制的描述
        private int _playerLimitItemLeftNumber = 0;     //角色限购的剩余次数

        private bool _isAccountLimit = false;       //账号限购
        private string _accountLimitDescriptionStr = "";    //账号限制的描述
        private int _accountLimitItemLeftNumber = 0;        //账号限购的剩余次数
        private bool _isUpdate = false; //积分时间戳更新

        private ComItem _comItem = null;

        [HeaderAttribute("Text")]
        [SerializeField] private Text nameText;
        [SerializeField] private Text playerLimitText;            //角色限购的描述
        [SerializeField] private Text accountLimitText;     //账号限购的描述
        [SerializeField] private Text curPriceText;
        // [SerializeField] private Text finishText;
        [SerializeField] private Text intergralLimtText;
        [SerializeField] private GameObject mObjEndTime;
        [SerializeField] private SimpleTimer mEndTimer;

        [HeaderAttribute("Image")]
        [SerializeField] private Image costIcon;
        [SerializeField] private Image intergralMultiple;

        [HeaderAttribute("GameObject")]
        [SerializeField] private GameObject itemRoot;
        [SerializeField] private GameObject limitRoot;
        [SerializeField] private GameObject costRoot;
        [SerializeField] private GameObject finishRoot;
        [SerializeField] private GameObject intergralRoot;
        [SerializeField] private GameObject intergralFlagRoot;
        [SerializeField] private GameObject singleRoot;
        [SerializeField] private GameObject multiplePlictityRoot;

        // [HeaderAttribute("Button")]
        // [SerializeField] private Button buyButton;
        // [SerializeField] private UIGray buyButtonGray;

        #region Awake InitData
        private void Awake()
        {
            ClearData();
            // BindUiEventSystem();
        }

        // private void BindUiEventSystem()
        // {
        //     if (buyButton != null)
        //     {
        //         buyButton.onClick.RemoveAllListeners();
        //         buyButton.onClick.AddListener(OnButtonClickCallBack);
        //     }
        // }

        private void OnDestroy()
        {
            if(_comItem != null)
                ComItemManager.Destroy(_comItem);

            // UnBindUiEventSystem();
            ClearData();
        }

        // private void UnBindUiEventSystem()
        // {
        //     if (buyButton != null)
        //     {
        //         buyButton.onClick.RemoveAllListeners();
        //     }
        // }

        private void ClearData()
        {
            _mallItemInfo = null;

            _isPlayerLimit = false;
            _playerLimitDescriptionStr = "";
            _playerLimitItemLeftNumber = 0;

            _isAccountLimit = false;
            _accountLimitDescriptionStr = "";
            _accountLimitItemLeftNumber = 0;
            _isUpdate = false;
        }

        private void Update()
        {
            if (_mallItemInfo != null && _mallItemInfo.multipleEndTime > 0)
            {
                _isUpdate = true;
            }

            if (_isUpdate == true)
            {
                int time = (int)(_mallItemInfo.multipleEndTime - TimeManager.GetInstance().GetServerTime());
                if (time <= 0)
                {
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnSendQueryMallItemInfo, _mallItemInfo.itemid);
                    _isUpdate = false;
                }
            }
        }
        #endregion

        #region Enable SyncData

        private void OnEnable()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnSyncWorldMallBuySucceed,
                OnSyncWorldMallBuySucceed);

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnSendQueryMallItemInfo, ReqQueryMallItemInfo);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnQueryMallItenInfoSuccess, OnQueryMallItenInfoSuccess);
        }

        private void OnDisable()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnSyncWorldMallBuySucceed,
                OnSyncWorldMallBuySucceed);

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnSendQueryMallItemInfo, ReqQueryMallItemInfo);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnQueryMallItenInfoSuccess, OnQueryMallItenInfoSuccess);
        }

        #endregion


        public void InitData(MallItemInfo mallItemInfo)
        {
            //清空数据
            ClearData();

            _mallItemInfo = mallItemInfo;
            if (_mallItemInfo == null)
                return;

            InitElementView();
        }

        [SerializeField] private GameObject mObjSelect;
        public void SetSelect(bool isSelect)
        {
            if (null != mObjSelect)
            {
                mObjSelect.CustomActive(isSelect);
            }
        }

        private void InitElementView()
        {
            InitElementLimitData();

            InitElementItem();
            InitElementBuyContent();
        }

        //初始化商品限购的数据
        private void InitElementLimitData()
        {
            //获得购买情况：是否限购，以及限购次数
            var isMallItemLimitBuy = IsMallItemLimitBuy();
            if (isMallItemLimitBuy == true)
            {
                //角色限购的标志
                if ((ELimitiTimeGiftDataLimitType) _mallItemInfo.limit == ELimitiTimeGiftDataLimitType.None)
                {
                    _isPlayerLimit = false;
                }
                else
                {
                    _isPlayerLimit = true;
                    _playerLimitDescriptionStr = TR.Value("mall_new_limit_player_day_limit");
                    switch ((ELimitiTimeGiftDataLimitType) _mallItemInfo.limit)
                    {
                        case ELimitiTimeGiftDataLimitType.Refresh:
                            _playerLimitDescriptionStr = TR.Value("mall_new_limit_player_day_limit");
                            _playerLimitItemLeftNumber =
                                _mallItemInfo.limitnum -
                                CountDataManager.GetInstance().GetCount(_mallItemInfo.id.ToString());
                            break;
                        case ELimitiTimeGiftDataLimitType.Week:
                            _playerLimitDescriptionStr = TR.Value("mall_new_limit_player_week_limit");
                            _playerLimitItemLeftNumber =
                                _mallItemInfo.limitnum -
                                CountDataManager.GetInstance().GetCount(_mallItemInfo.id.ToString());
                            break;
                        case ELimitiTimeGiftDataLimitType.NotRefresh:
                            _playerLimitDescriptionStr = TR.Value("mall_new_limit_player_forever_limit");
                            _playerLimitItemLeftNumber =
                                _mallItemInfo.limittotalnum -
                                CountDataManager.GetInstance().GetCount(_mallItemInfo.id.ToString());
                            break;
                    }

                    if (_playerLimitItemLeftNumber <= 0)
                        _playerLimitItemLeftNumber = 0;
                }

                //账号限购
                if (_mallItemInfo.accountLimitBuyNum <= 0)
                {
                    _isAccountLimit = false;
                }
                else
                {
                    _isAccountLimit = true;
                    _accountLimitItemLeftNumber = (int)_mallItemInfo.accountRestBuyNum;
                    _accountLimitDescriptionStr = TR.Value("mall_new_limit_account_day_limit");

                    switch ((RefreshType) _mallItemInfo.accountRefreshType)
                    {
                        case RefreshType.REFRESH_TYPE_NONE:
                            _accountLimitDescriptionStr = TR.Value("mall_new_limit_account_forever_limit");
                            break;
                        case RefreshType.REFRESH_TYPE_PER_DAY:
                            _accountLimitDescriptionStr = TR.Value("mall_new_limit_account_day_limit");
                            break;
                        case RefreshType.REFRESH_TYPE_PER_WEEK:
                            _accountLimitDescriptionStr = TR.Value("mall_new_limit_account_week_limit");
                            break;
                        case RefreshType.REFRESH_TYPE_PER_MONTH:
                            _accountLimitDescriptionStr = TR.Value("mall_new_limit_account_month_limit");
                            break;
                    }
                }
            }
        }

        //初始化Item相关内容
        private void InitElementItem()
        {
            var itemId = (int)_mallItemInfo.itemid;

            var itemTable = TableManager.GetInstance().GetTableItem<ItemTable>(itemId);
            if (itemTable == null)
            {
                Logger.LogErrorFormat("The itemTable is null and itemId is {0}", itemId);
                return;
            }

            var itemData = ItemDataManager.CreateItemDataFromTable(itemId);
            if (itemData == null)
            {
                Logger.LogErrorFormat("The ItemData is null and itemId is {0}", itemId);
                return;
            }

            if (itemData.SubType != (int) ItemTable.eSubType.GOLD
                && itemData.SubType != (int) ItemTable.eSubType.BindGOLD)
            {
                itemData.Count = (int) _mallItemInfo.itemnum;
            }

            _comItem = itemRoot.GetComponentInChildren<ComItem>();
            if (_comItem == null)
            {
                _comItem = ComItemManager.Create(itemRoot);
            }
            _comItem.Setup(itemData, ShowItemTip);

            if (nameText != null)
            {
                if (itemData.SubType == (int) ItemTable.eSubType.GOLD
                    || itemData.SubType == (int) ItemTable.eSubType.BindGOLD)
                {
                    nameText.text = Utility.GetShowPrice((UInt64) _mallItemInfo.itemnum, true) + itemData.Name;
                }
                else
                {
                    nameText.text = PetDataManager.GetInstance()
                        .GetColorName(itemTable.Name, (PetTable.eQuality) itemTable.Color);
                }
            }
            //结束时间
            mObjEndTime.CustomActive(0 < (int)_mallItemInfo.endtime);
            if (0 < _mallItemInfo.endtime)
            {
                mEndTimer.SetCountdown((int)(_mallItemInfo.endtime - TimeManager.GetInstance().GetServerTime()));
            }

        }

        private void InitElementBuyContent()
        {
            if(costIcon != null)
            {
                var costItemTable = MallNewDataManager.GetInstance().GetCostItemTableByCostType(_mallItemInfo.moneytype);
                if (costItemTable != null)
                {
                    ETCImageLoader.LoadSprite(ref costIcon, costItemTable.Icon);
                }
                else
                {
                    Logger.LogErrorFormat("CostItemTable is null and moneyType is {0}", _mallItemInfo.moneytype);
                }
            }

            curPriceText.text = Utility.GetMallRealPrice(_mallItemInfo).ToString();

            UpdateElementBuyContent();
        }

        /// <summary>
        /// 初始化积分信息
        /// </summary>
        private void UpdateIntergralInfo()
        {
            intergralFlagRoot.CustomActive(_mallItemInfo.multiple > 0);
            intergralRoot.CustomActive(_mallItemInfo.multipleEndTime > 0);
            singleRoot.CustomActive(_mallItemInfo.multiple == 1);
            multiplePlictityRoot.CustomActive(_mallItemInfo.multiple > 1);

            if (intergralMultiple != null)
            {
                ETCImageLoader.LoadSprite(ref intergralMultiple, TR.Value("mall_new_limit_item_left_intergral_multiple_sprit_path", _mallItemInfo.multiple));
            }

            intergralLimtText.text = TR.Value("mall_new_limit_item_left_intergral_multiple_limit", _mallItemInfo.multiple, Function.SetShowTimeDay((int)_mallItemInfo.multipleEndTime));
        }

        //更新Item购买的限制条件：
        //1:是否限制购买，如果非限制否买的话，可以正常无限次的购买
        //2:限制购买：还剩多少次
        //3：已经购买完成，不能购买
        private void UpdateElementBuyContent()
        {
            UpdateIntergralInfo();

            //非限购
            if (IsLimitItem() == false)
            {
                // UpdateBuyButton(true);
                costRoot.CustomActive(true);
                limitRoot.CustomActive(false);
                finishRoot.CustomActive(false);
                return;
            }
            
            //不可购买
            if ((_isPlayerLimit == true && _playerLimitItemLeftNumber <= 0)
                || (_isAccountLimit == true && _accountLimitItemLeftNumber <= 0))
            {

                // UpdateBuyButton(false);
                costRoot.CustomActive(false);
                limitRoot.CustomActive(true);
                finishRoot.CustomActive(true);
                // finishText.text = TR.Value("mall_new_limit_item_left_number_zero");
            }
            else
            {
                //可以购买
                finishRoot.CustomActive(false);
                //存在限购，并且还可以购买
                // UpdateBuyButton(true);
                costRoot.CustomActive(true);
                limitRoot.CustomActive(true);
            }
            if (playerLimitText != null)
            {
                if (_isPlayerLimit == true)
                {
                    CommonUtility.UpdateTextVisible(playerLimitText, true);
                    playerLimitText.text = string.Format(_playerLimitDescriptionStr, _playerLimitItemLeftNumber);
                }
                else
                {
                    CommonUtility.UpdateTextVisible(playerLimitText, false);
                }
            }

            if (accountLimitText != null)
            {
                if (_isAccountLimit == true)
                {
                    CommonUtility.UpdateTextVisible(accountLimitText, true);
                    accountLimitText.text = string.Format(_accountLimitDescriptionStr, _accountLimitItemLeftNumber);
                }
                else
                {
                    CommonUtility.UpdateTextVisible(accountLimitText, false);
                }
            }
        }

        // private void UpdateBuyButton(bool flag)
        // {
        //     if (buyButtonGray != null)
        //     {
        //         buyButtonGray.enabled = !flag;
        //     }

        //     if (buyButton != null)
        //         buyButton.interactable = flag;
        // }

        // private void OnButtonClickCallBack()
        // {
        //     if(_mallItemInfo == null)
        //         return;

        //     if (SecurityLockDataManager.GetInstance().CheckSecurityLock())
        //     {
        //         return;
        //     }

        //     OpenMallBuyFrame();
        // }

        #region UIEvent

        private void OnSyncWorldMallBuySucceed(UIEvent uiEvent)
        {
            if (uiEvent == null)
                return;

            if (uiEvent.Param1 == null
                || uiEvent.Param2 == null
                || uiEvent.Param3 == null)
                return;

            //是否为同一个商品
            if (_mallItemInfo == null)
                return;

            var id = (UInt32)uiEvent.Param1;

            if (_mallItemInfo.id != id)
                return;

            if (IsLimitItem() == false)
                return;

            _playerLimitItemLeftNumber = (int)uiEvent.Param2;
            _accountLimitItemLeftNumber = (int)uiEvent.Param3;

            UpdateElementBuyContent();
        }

        private void ReqQueryMallItemInfo(UIEvent uiEvent)
        {
            //是否为同一个商品
            if (_mallItemInfo == null)
                return;

            var itemId = (UInt32)uiEvent.Param1;

            if (_mallItemInfo.itemid != itemId)
                return;

            MallNewDataManager.GetInstance().ReqQueryMallItemInfo((int)_mallItemInfo.itemid);
        }

        private void OnQueryMallItenInfoSuccess(UIEvent uiEvent)
        {
            _mallItemInfo = MallNewDataManager.GetInstance().QueryMallItemInfo;
            UpdateIntergralInfo();
        }

        #endregion

        #region Helper
        //打开购买界面
        // private void OpenMallBuyFrame()
        // {
        //     if (ClientSystemManager.GetInstance().IsFrameOpen<MallBuyFrame>() == true)
        //         ClientSystemManager.GetInstance().CloseFrame<MallBuyFrame>();

        //     Utility.DoStartFrameOperation("MallNewPropertyMallElementItem", string.Format("BuyItem/{0}", _mallItemInfo.itemid));
        //     ClientSystemManager.GetInstance().OpenFrame<MallBuyFrame>(FrameLayer.Middle, _mallItemInfo);
        // }

        private void ShowItemTip(GameObject go, ItemData itemData)
        {
            Utility.DoStartFrameOperation("MallNewPropertyMallElementItem",string.Format("DetailItem/{0}",itemData.PackID));
            ItemTipManager.GetInstance().ShowTip(itemData);
        }

        //商店中商品类型：推荐，消耗品，材料，金币，道具，功能，兑换， 药品
        private bool IsMallItemLimitBuy()
        {
            if (_mallItemInfo == null)
                return false;

            if (_mallItemInfo.type == (byte)MallTypeTable.eMallType.SN_RECOMMEND
                || _mallItemInfo.type == (byte)MallTypeTable.eMallType.SN_COST
                || _mallItemInfo.type == (byte)MallTypeTable.eMallType.SN_MATERIAL
                || _mallItemInfo.type == (byte)MallTypeTable.eMallType.SN_GOLD
                || _mallItemInfo.type == (byte)MallTypeTable.eMallType.SN_ITEM
                || _mallItemInfo.type == (byte)MallTypeTable.eMallType.SN_FUNCTION
                || _mallItemInfo.type == (byte)MallTypeTable.eMallType.SN_EXCHANGE
                || _mallItemInfo.type == (byte)MallTypeTable.eMallType.SN_MEDICINE
                || _mallItemInfo.type == (byte)MallTypeTable.eMallType.SN_MALL_POINT_ITEM)
            {
                return true;
            }

            return false;
        }

        private bool IsLimitItem()
        {
            //角色限制
            if (_isPlayerLimit == true)
                return true;

            //账号限制
            if (_isAccountLimit == true)
                return true;

            return false;
        }
        #endregion

    }
}
