using System;
using System.Collections.Generic;
using Network;
using ProtoTable;
using Protocol;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;

namespace GameClient
{

    public class AuctionNewBuyItemView : MonoBehaviour
    {

        private AuctionNewBuyItemDataModel _buyItemDataModel;
        private int _currentBuyNumber = 1;
        private int _maxBuyNumber = 1;
        private long _ownerMoney = 1;
        private long _totalCostMoney = 1;
        private ItemData _itemData = null;

        [Space(15)] [HeaderAttribute("Text")] [Space(5)]
        [SerializeField] private Text itemNameText;
        [SerializeField] private GameObject itemRoot;

        [Space(15)] [HeaderAttribute("Money")] [Space(10)]
        [SerializeField] private Image singlePriceImage;
        [SerializeField] private Text singlePriceValueText;
        [SerializeField] private Image totalPriceImage;
        [SerializeField] private Text totalPriceValueText;
        [SerializeField] private Image ownerMoneyImage;
        [SerializeField] private Text ownerMoneyValueText;

        [Space(15)][HeaderAttribute("BuyNumber")]
        [Space(10)]
        [SerializeField] private Text buyItemNumberText;
        [SerializeField] private Button itemNumberMaxButton;
        [SerializeField] private UIGray itemNumberMaxButtonGray;
        [SerializeField] private Button itemNumberKeyBoardButton;
        
        [Space(15)] [HeaderAttribute("Button")] [Space(10)]
        [SerializeField] private Button cancelButton;
        [SerializeField] private Button buyButton; 
        [SerializeField] private Button closeButton;

        private void Awake()
        {
            BindUiEvents();
        }

        private void OnDestroy()
        {
            UnBindUiEvents();
            ClearData();
        }

        private void ClearData()
        {
            _buyItemDataModel = null;
            _currentBuyNumber = 1;
            _maxBuyNumber = 1;
            _ownerMoney = 1;
            _totalCostMoney = 1;
            _itemData = null;
        }

        private void BindUiEvents()
        {
            if (closeButton != null)
            {
                closeButton.onClick.RemoveAllListeners();
                closeButton.onClick.AddListener(OnCloseButtonClick);
            }

            if (cancelButton != null)
            {
                cancelButton.onClick.RemoveAllListeners();
                cancelButton.onClick.AddListener(OnCancelButtonClick);
            }

            if (buyButton != null)
            {
                buyButton.onClick.RemoveAllListeners();
                buyButton.onClick.AddListener(OnBuyButtonClick);
            }

            if (itemNumberMaxButton != null)
            {
                itemNumberMaxButton.onClick.RemoveAllListeners();
                itemNumberMaxButton.onClick.AddListener(OnItemNumberMaxButtonClick);
            }

            if (itemNumberKeyBoardButton != null)
            {
                itemNumberKeyBoardButton.onClick.RemoveAllListeners();
                itemNumberKeyBoardButton.onClick.AddListener(OnItemNumberKeyBoardButtonClick);
            }

        }

        private void UnBindUiEvents()
        {
            if (closeButton != null)
                closeButton.onClick.RemoveAllListeners();

            if(cancelButton != null)
                cancelButton.onClick.RemoveAllListeners();

            if(buyButton != null)
                buyButton.onClick.RemoveAllListeners();

            if(itemNumberMaxButton != null)
                itemNumberMaxButton.onClick.RemoveAllListeners();

            if(itemNumberKeyBoardButton != null)
                itemNumberKeyBoardButton.onClick.RemoveAllListeners();
        }

        private void OnEnable()
        {
            //金币更改
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.GoldChanged, OnMoneyChanged);

            //绑定键盘事件
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnCommonKeyBoardInput, OnCommonKeyBoardInput);
        }

        private void OnDisable()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.GoldChanged, OnMoneyChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnCommonKeyBoardInput, OnCommonKeyBoardInput);
        }

        public void Init(AuctionNewBuyItemDataModel buyItemDataModel)
        {
            _buyItemDataModel = buyItemDataModel;
            if (_buyItemDataModel == null)
                return;

            _itemData = ItemDataManager.CreateItemDataFromTable(_buyItemDataModel.ItemTypeId);
            if (_itemData == null || _itemData.TableData == null)
                return;

            InitData();
            InitView();
        }

        private void InitData()
        {
            _currentBuyNumber = 1;

            UpdateOwnerMoneyAndMaxNumber();
        }

        private void UpdateOwnerMoneyAndMaxNumber()
        {
            _ownerMoney = ItemDataManager.GetInstance().GetOwnedItemCount(_buyItemDataModel.MoneyTypeId, false);

            var totalOwnerMoneyWithEqual =
                ItemDataManager.GetInstance().GetOwnedItemCount(_buyItemDataModel.MoneyTypeId, false);

            if (_buyItemDataModel.SinglePrice <= 0)
            {
                _maxBuyNumber = 1;
            }
            else
            {
                _maxBuyNumber = (int)((long)totalOwnerMoneyWithEqual / _buyItemDataModel.SinglePrice);
            }

            //限制购买的数量
            if (_buyItemDataModel.Number > 0
                && _buyItemDataModel.Number < _maxBuyNumber)
                _maxBuyNumber = _buyItemDataModel.Number;
        }


        private void InitView()
        {
            InitItemInfo();
            InitPriceImage();
            InitSinglePrice();

            UpdateTotalPrice();
            UpdateOwnerMoney();
            UpdateBuyItemNumber();

            UpdateItemNumberMaxButtonState();
        }

        private void InitItemInfo()
        {
            if (itemRoot != null)
            {
                //数量强化等级
                _itemData.Count = _buyItemDataModel.Number;
                _itemData.StrengthenLevel = _buyItemDataModel.StrengthLevel;
                //增幅相关
                AuctionNewUtility.UpdateItemDataByEquipType(_itemData,
                    _buyItemDataModel.EquipType,
                    _buyItemDataModel.EnhanceType,
                    _buyItemDataModel.EnhanceNum);
                //已经交易次数
                _itemData.ItemTradeNumber = (int)_buyItemDataModel.ItemTradeNumber;

                var comItem = ComItemManager.Create(itemRoot);
                if (comItem != null)
                {
                    comItem.Setup(_itemData, ShowItemTipFrame);
                    comItem.SetShowTreasure(_buyItemDataModel.IsTreasure == 1 ? true : false);
                }
            }

            if (itemNameText != null)
                itemNameText.text = AuctionNewUtility.GetQualityColorString(_itemData.Name,
                    _itemData.TableData.Color);
        }

        private void InitPriceImage()
        {
            var moneyImagePath = ItemDataManager.GetInstance().GetOwnedItemIconPath(_buyItemDataModel.MoneyTypeId);

            if (singlePriceImage != null)
                ETCImageLoader.LoadSprite(ref singlePriceImage, moneyImagePath);

            if (totalPriceImage != null)
                ETCImageLoader.LoadSprite(ref totalPriceImage, moneyImagePath);

            if (ownerMoneyImage != null)
                ETCImageLoader.LoadSprite(ref ownerMoneyImage, moneyImagePath);
        }

        private void InitSinglePrice()
        {
            if (singlePriceValueText != null)
                singlePriceValueText.text = _buyItemDataModel.SinglePrice.ToString();
        }

        private void UpdateTotalPrice()
        {
            if (_buyItemDataModel == null)
                return;

            if (totalPriceValueText == null)
                return;

            var totalPriceValue = _buyItemDataModel.SinglePrice * (long)_currentBuyNumber;
            totalPriceValueText.text = totalPriceValue.ToString();

            UpdateOwnerMoney();
        }

        private void UpdateOwnerMoney()
        {
            if (_buyItemDataModel == null)
                return;

            if (ownerMoneyValueText == null)
                return;

            ownerMoneyValueText.text = _ownerMoney.ToString();

            var totalCostValue = _buyItemDataModel.SinglePrice * (long)_currentBuyNumber;

            if (totalCostValue > _ownerMoney)
            {
                ownerMoneyValueText.color = Color.red;
            }
            else
            {
                ownerMoneyValueText.color = Color.white;
            }

        }

        private void UpdateBuyItemNumber()
        {
            if (_buyItemDataModel == null)
                return;

            if (buyItemNumberText == null)
                return;

            buyItemNumberText.text = _currentBuyNumber.ToString();

        }

        private void OnItemNumberMaxButtonClick()
        {
            if (_buyItemDataModel == null)
                return;

            _currentBuyNumber = _maxBuyNumber;

            UpdateItemNumberMaxButtonState();

            UpdateTotalPrice();
            UpdateBuyItemNumber();
        }

        private void UpdateItemNumberMaxButtonState()
        {
            if (_buyItemDataModel == null)
                return;

            if (_currentBuyNumber < _maxBuyNumber)
            {
                CommonUtility.UpdateButtonState(itemNumberMaxButton, itemNumberMaxButtonGray, true);
            }
            else
            {
                CommonUtility.UpdateButtonState(itemNumberMaxButton, itemNumberMaxButtonGray, false);
            }
        }

        #region UIEvent

        //金币数量更改
        private void OnMoneyChanged(UIEvent uiEvent)
        {
            if (_buyItemDataModel == null)
                return;

            //更新金币数量
            UpdateOwnerMoneyAndMaxNumber();

            //更新UI
            UpdateOwnerMoney();
            UpdateItemNumberMaxButtonState();
        }

        //键盘输入，购买数量更改
        private void OnCommonKeyBoardInput(UIEvent uiEvent)
        {
            if (_buyItemDataModel == null)
                return;

            if (uiEvent == null
                || uiEvent.Param1 == null || uiEvent.Param2 == null)
            {
                return;
            }

            var inputType = (CommonKeyBoardInputType)uiEvent.Param1;
            _currentBuyNumber = (int)((ulong)uiEvent.Param2);
            
            //购买数量进行限制
            if (_currentBuyNumber <= 0)
            {
                _currentBuyNumber = 1;
            }
            else if (_currentBuyNumber >= _maxBuyNumber)
            {
                _currentBuyNumber = _maxBuyNumber;
            }

            UpdateTotalPrice();

            UpdateBuyItemNumber();
            UpdateItemNumberMaxButtonState();
        }

        #endregion

        #region BuyItem
        private void OnBuyButtonClick()
        {
            if (SecurityLockDataManager.GetInstance().CheckSecurityLock())
            {
                return;
            }

            //交易限制
            var isItemTradeLimit = ItemDataUtility.IsItemTradeLimitBuyNumber(_itemData);
            if (isItemTradeLimit == true)
            {
                var itemTradeLeftTime = ItemDataUtility.GetItemTradeLeftTime(_itemData);
                if (itemTradeLeftTime <= 0)
                {
                    //飘字
                    SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("auction_new_item__trade_number_zero"));
                    return;
                }
                else
                {
                    //弹窗提示
                    var contentStr = string.Format(TR.Value("auction_new_item_on_buy_with_trade_number"),
                        itemTradeLeftTime);

                    AuctionNewUtility.OnShowItemTradeLimitFrame(contentStr,
                        OnBuyItem);
                    return;
                }
            }

            OnBuyItem();
        }

        private void OnBuyItem()
        {
            CostItemManager.CostInfo costInfo = new CostItemManager.CostInfo();

            costInfo.nMoneyID = _buyItemDataModel.MoneyTypeId;
            costInfo.nCount = (int)_buyItemDataModel.SinglePrice * _currentBuyNumber;

            CostItemManager.GetInstance().TryCostMoneyDefault(costInfo, () =>
            {
                SendBuyItemReq();
                OnCloseFrame();
            });
        }


        private void SendBuyItemReq()
        {
            NetManager netMgr = NetManager.Instance();

            var isTreasureRushBuy =
                AuctionNewUtility.IsAuctionNewTreasureRushBuy(_buyItemDataModel.IsTreasure, _buyItemDataModel.PublicEndTime);

            //珍品是否抢购：1：珍品在抢购期间；2：非再次上架。
            //IsAgainOnSale = 1 只能按照正常的上架逻辑
            if (isTreasureRushBuy == true && _buyItemDataModel.IsAgainOnSale == 0)
            {
                var req = new WorldAuctionRusyBuy
                {
                    id = _buyItemDataModel.Guid,
                    num = (uint)_currentBuyNumber,
                };
                netMgr.SendCommand(ServerType.GATE_SERVER, req);
            }
            else
            {
                //正常购买
                var req = new WorldAuctionBuy
                {
                    id = _buyItemDataModel.Guid,
                    num = (uint)_currentBuyNumber,
                };

                netMgr.SendCommand(ServerType.GATE_SERVER, req);
            }
        }
        #endregion 

        private void OnItemNumberKeyBoardButtonClick()
        {
            CommonUtility.OnOpenCommonKeyBoardFrame(new Vector3(680, 116, 0),
                (ulong)_currentBuyNumber,
                (ulong)_maxBuyNumber);
        }

        private void OnCloseButtonClick()
        {
            OnCloseFrame();
        }

        private void OnCancelButtonClick()
        {
            OnCloseFrame();
        }

        private void OnCloseFrame()
        {
            AuctionNewUtility.OnCloseAuctionNewBuyItemFrame();
        }

        private void ShowItemTipFrame(GameObject go, ItemData itemData)
        {
            if (_buyItemDataModel == null)
                return;

            AuctionNewDataManager.GetInstance().OnShowItemDetailTipFrame(itemData, _buyItemDataModel.Guid);
        }

    }
}
