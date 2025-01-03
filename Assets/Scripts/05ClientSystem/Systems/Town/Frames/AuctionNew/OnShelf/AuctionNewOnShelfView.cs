using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using Protocol;
using ProtoTable;
using Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
   
    public class AuctionNewOnShelfView : MonoBehaviour
    {

        private ulong _auctionItemGuid = 0;                 //上架道具的GUID
        private bool _isTreasureItem = false;               //是否为珍品
        private AuctionNewOnShelfItemData _auctionNewOnShelfItemData;       //上架物品道具

        private bool _isTimeOverItem = false;               //是否为过期的物品
        private AuctionBaseInfo _auctionBaseInfo = null;        //过期物品的数据

        private ItemData _onShelfItemData = null;
        private ItemTable _onShelfItemTable = null;

        private ulong _onShelfItemRecommendPrice;        //推荐价格,由表中配置算的
        private int _feeValue;              //手续费 百分比
        private int _depositValue;          //押金
        private int _onShelfItemAveragePrice;          //平均价格，由服务器同步，主要是非珍品。珍品这个字段没有用到

        private int _onShelfItemRecommendPriceByServer;  //推荐价格，由服务器同步

        private int _depositBaseValue;      //押金的基础数值：由系统数值表中读取

        private int _onShelfItemNumber;         //准备上架物品的数量
        private int _onShelfItemSinglePrice;    //准备上架物品的单价
        private int _onShelfItemTotalPrice;     //准备上架物品的总价

        private const int DepositMinValue = 1;          //押金的最小值
        private const int DepositMaxValue = 100000;     //押金的最大数值

        private int _onShelfItemMaxPriceRate = 0;         //价格的最大比例，由表中读取

        private int _onShelfItemMinSinglePrice;         //上架物品的最小单价，由服务器同步
        private int _onShelfItemMaxSinglePrice;         //上架物品的最大单价，由服务器同步

        private int _onShelfItemMinNumber = 1;          //上架物品的最少数量
        private int _onShelfItemMaxNumber = 1;          //上架物品的最大数量

        private int _onShelfItemPricesNumber;   //非珍品的价格区间的价格数量
        private int[] _onShelfItemPricesArray;       //价格介于最值之间，并且有平均价格根据一定的规律获得

        private AuctionNewOnShelfItemKeyBoardInputType _onShelfItemKeyBoardInputType = AuctionNewOnShelfItemKeyBoardInputType.None;


        [Space(5)]
        [HeaderAttribute("base")]
        [Space(5)]
        [SerializeField] private Text titleLabel = null;
        [SerializeField] private Button closeButton;


        [Space(10)] [HeaderAttribute("OtherPlayerOnShelfControl")] [Space(5)]
        [SerializeField] private AuctionNewOtherPlayerControl otherPlayerControl;

        [Space(10)] [HeaderAttribute("ItemCommonInfo")] [Space(5)]
        [SerializeField] private GameObject onShelfItemRoot;
        [SerializeField] private Text onShelfItemName;
        [SerializeField] private GameObject normalButtonRoot;
        [SerializeField] private Button onShelfItemButton;
        [SerializeField] private GameObject timeOverButtonRoot;
        [SerializeField] private Button recoverButton;
        [SerializeField] private Button onShelfAgainButton;

        [Space(10)] [HeaderAttribute("TreasureItem")] [Space(5)] [SerializeField]
        private GameObject treasureItemRoot;

        [SerializeField] private Text treasureFeeTitleLabel;
        [SerializeField] private Text treasureFeeValueLabel;
        [SerializeField] private Text treasureDepositValueLabel;
        [SerializeField] private Text treasureItemNumberLabel;
        [SerializeField] private Text treasureItemPriceInputLabel;
        [SerializeField] private Text treasureItemSinglePriceLabel;
        [SerializeField] private Text treasureItemTotalPriceLabel;
        [SerializeField] private Button treasureItemPriceKeyBoardButton;

        [Space(10)] [HeaderAttribute("NormalItem")] [Space(5)] [SerializeField]
        private GameObject normalItemRoot;
        [SerializeField] private Text normalFeeTitleLabel;
        [SerializeField] private Text normalFeeValueLabel;
        [SerializeField] private Text normalDepositValueLabel;
        [SerializeField] private Text recommendPriceLabel;
        [SerializeField] private Text avaragePriceLabel;
        [SerializeField] private Text normalItemNumberLabel;
        [SerializeField] private Text normalItemSinglePriceLabel;
        [SerializeField] private Text normalItemTotalPriceLabel;
        [Space(10)]
        [HeaderAttribute("ItemButton")]
        [Space(5)]
        [SerializeField] private Button minusNumberButton;
        [SerializeField] private UIGray minusNumberButtonGray;
        [SerializeField] private Button addNumberButton;
        [SerializeField] private UIGray addNumberButtonGray;
        [SerializeField] private Button normalItemNumberKeyBoardButton;
        [SerializeField] private Button minusPriceButton;
        [SerializeField] private UIGray minusPriceButtonGray;
        [SerializeField] private Button addPriceButton;
        [SerializeField] private UIGray addPriceButtonGray;
        [SerializeField] private Button normalItemPriceKeyBoardButton;

        #region InitData
        private void Awake()
        {
            BindEvents();
        }

        private void OnDestroy()
        {
            UnBindEvents();
            ClearData();
        }

        private void BindEvents()
        {
            if (closeButton != null)
            {
                closeButton.onClick.RemoveAllListeners();
                closeButton.onClick.AddListener(OnCloseFrame);
            }

            if (onShelfItemButton != null)
            {
                onShelfItemButton.onClick.RemoveAllListeners();
                onShelfItemButton.onClick.AddListener(OnItemOnShelfButtonClick);
            }

            if (recoverButton != null)
            {
                recoverButton.onClick.RemoveAllListeners();
                recoverButton.onClick.AddListener(OnRecoverButtonClick);
            }

            if (onShelfAgainButton != null)
            {
                onShelfAgainButton.onClick.RemoveAllListeners();
                onShelfAgainButton.onClick.AddListener(OnShelfAgainButtonClick);
            }

            if (addNumberButton != null)
            {
                addNumberButton.onClick.RemoveAllListeners();
                addNumberButton.onClick.AddListener(OnAddNumberButtonClick);
            }

            if (minusNumberButton != null)
            {
                minusNumberButton.onClick.RemoveAllListeners();
                minusNumberButton.onClick.AddListener(OnMinusNumberButtonClick);
            }

            if (addPriceButton != null)
            {
                addPriceButton.onClick.RemoveAllListeners();
                addPriceButton.onClick.AddListener(OnAddPriceButtonClick);
            }

            if (minusPriceButton != null)
            {
                minusPriceButton.onClick.RemoveAllListeners();
                minusPriceButton.onClick.AddListener(OnMinusPriceButtonClick);
            }

            if (treasureItemPriceKeyBoardButton != null)
            {
                treasureItemPriceKeyBoardButton.onClick.RemoveAllListeners();
                treasureItemPriceKeyBoardButton.onClick.AddListener(OnTreasureItemPriceKeyBoardButtonClick);
            }

            if (normalItemNumberKeyBoardButton != null)
            {
                normalItemNumberKeyBoardButton.onClick.RemoveAllListeners();
                normalItemNumberKeyBoardButton.onClick.AddListener(OnNormalItemNumberKeyBoardButtonClick);
            }

            if (normalItemPriceKeyBoardButton != null)
            {
                normalItemPriceKeyBoardButton.onClick.RemoveAllListeners();
                normalItemPriceKeyBoardButton.onClick.AddListener(OnNormalItemPriceKeyBoardButtonClick);
            }

            //请求道具的价格
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnAuctionNewWorldQueryItemPriceResSucceed,
                OnReceiveWorldAuctionQueryItemPriceResSucceed);
            //请求在售和公示商品的回调
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnAuctionNewWorldQueryItemPriceListResSucceed,
                OnReceiveWorldAuctionQueryItemPriceListResSucceed);
            //请求最近销售记录的回调
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnAuctionNewWorldQueryItemTransListResSucceed,
                OnAuctionNewWorldQueryItemTransListResSucceed);

            //绑定键盘事件
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnCommonKeyBoardInput, OnCommonKeyBoardInput);
        }

        private void UnBindEvents()
        {
            if (closeButton != null)
                closeButton.onClick.RemoveAllListeners();
            
            if(onShelfItemButton != null)
                onShelfItemButton.onClick.RemoveAllListeners();

            if(recoverButton != null)
                recoverButton.onClick.RemoveAllListeners();

            if(onShelfAgainButton != null)
                onShelfAgainButton.onClick.RemoveAllListeners();

            if (addNumberButton != null)
                addNumberButton.onClick.RemoveAllListeners();

            if (minusNumberButton != null)
                minusNumberButton.onClick.RemoveAllListeners();

            if (addPriceButton != null)
                addPriceButton.onClick.RemoveAllListeners();

            if (minusPriceButton != null)
                minusPriceButton.onClick.RemoveAllListeners();

            if (treasureItemPriceKeyBoardButton != null)
                treasureItemPriceKeyBoardButton.onClick.RemoveAllListeners();

            if (normalItemNumberKeyBoardButton != null)
                normalItemNumberKeyBoardButton.onClick.RemoveAllListeners();

            if (normalItemPriceKeyBoardButton != null)
                normalItemPriceKeyBoardButton.onClick.RemoveAllListeners();

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnAuctionNewWorldQueryItemPriceResSucceed,
                OnReceiveWorldAuctionQueryItemPriceResSucceed);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnAuctionNewWorldQueryItemPriceListResSucceed,
                OnReceiveWorldAuctionQueryItemPriceListResSucceed);

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnAuctionNewWorldQueryItemTransListResSucceed,
                OnAuctionNewWorldQueryItemTransListResSucceed);

            //解绑键盘事件
            UIEventSystem.GetInstance()
                .UnRegisterEventHandler(EUIEventID.OnCommonKeyBoardInput, OnCommonKeyBoardInput);
        }

        private void ClearData()
        {
            _auctionItemGuid = 0;
            _auctionNewOnShelfItemData = null;
            _isTreasureItem = false;
            _onShelfItemData = null;
            _onShelfItemTable = null;
            _onShelfItemPricesArray = null;

            _onShelfItemKeyBoardInputType = AuctionNewOnShelfItemKeyBoardInputType.None;
            _onShelfItemMaxPriceRate = 0;

            _isTimeOverItem = false;
            _auctionBaseInfo = null;
        }

        #endregion

        #region InitItemView

        //需要传递默认的参数
        public void InitView(AuctionNewOnShelfItemData auctionNewOnShelfItemData)
        {

            InitOnShelfItemData();

            //上架物品数据
            _auctionNewOnShelfItemData = auctionNewOnShelfItemData;

            if (_auctionNewOnShelfItemData == null)
            {
                Logger.LogErrorFormat("AuctionNewOnShelfView InitView auctionNewOnShelfItemData is null");
                return;
            }

            _auctionItemGuid = _auctionNewOnShelfItemData.PackageItemGuid;
            _isTreasureItem = _auctionNewOnShelfItemData.IsTreasure;
            _isTimeOverItem = _auctionNewOnShelfItemData.IsTimeOverItem;
            _auctionBaseInfo = _auctionNewOnShelfItemData.ItemAuctionBaseInfo;      //只在商品过期的时候有意义

            //过期商品: 若基础数据为null，则直接返回
            if (_isTimeOverItem == true)
            {
                if (_auctionBaseInfo == null)
                    return;
            }

            if (_isTimeOverItem == false)
            {
                //正常上架的背包物品
                _onShelfItemData = ItemDataManager.GetInstance().GetItem(_auctionItemGuid);
            }
            else
            {
                //拍卖行架子中的过期商品
                _onShelfItemData = ItemDataManager.CreateItemDataFromTable((int)_auctionBaseInfo.itemTypeId);
                if (_onShelfItemData != null)
                {
                    _onShelfItemData.Count = (int) _auctionBaseInfo.num;
                    _onShelfItemData.StrengthenLevel = (int) _auctionBaseInfo.strengthed;
                    _onShelfItemData.BeadAdditiveAttributeBuffID = (int) _auctionBaseInfo.beadbuffid;
                    AuctionNewUtility.UpdateItemDataByEquipType(_onShelfItemData, _auctionBaseInfo);
                }
            }

            if (_onShelfItemData == null)
            {
                Logger.LogErrorFormat("OnShelfItemData is null isTimeOverIem is {0}, itemGuid is {1}",
                    _isTimeOverItem.ToString(), _auctionItemGuid);
                return;
            }

            _onShelfItemTable = TableManager.GetInstance().GetTableItem<ItemTable>(_onShelfItemData.TableID);
            if (_onShelfItemTable == null)
            {
                Logger.LogErrorFormat("OnShelfItemTable is null and itemTable id is {0}", _onShelfItemData.TableID);
                return;
            }

            if (_onShelfItemTable.AuctionMaxPrice >= 100)
            {
                _onShelfItemMaxPriceRate = _onShelfItemTable.AuctionMaxPrice;
            }
            else
            {
                _onShelfItemMaxPriceRate = 100;
            }

            //更新过期的相关数据
            if (_isTimeOverItem == false)
            {
                //上架的最大数量
                _onShelfItemMaxNumber = AuctionNewUtility.GetItemNumberByGuid(_auctionItemGuid, true);
            }
            else
            {
                //过期道具的数量
                _onShelfItemNumber = (int) _auctionBaseInfo.num;
                _onShelfItemMaxNumber = _onShelfItemNumber;
                _onShelfItemMinNumber = _onShelfItemNumber;
                //如果是珍品，则显示过期商品的价格
                if (_isTreasureItem == true)
                    _onShelfItemSinglePrice = (int) _auctionBaseInfo.price;
            }

            InitBaseView();

            InitOnShelfItemInfo();

            //发送服务器消息，用于请求交易的平均价格，价格的最值，和上架的商品
            AuctionNewDataManager.GetInstance().SendWorldAuctionQueryOnShelfItemPriceReq(_onShelfItemData);
        }

        //上架的数据
        private void InitOnShelfItemData()
        {
            _onShelfItemNumber = 1;
            _onShelfItemSinglePrice = 0;
            _onShelfItemTotalPrice = 0;

            _onShelfItemMinSinglePrice = 0;
            _onShelfItemMaxSinglePrice = 0;
            _onShelfItemRecommendPrice = 0;
            _onShelfItemAveragePrice = 0;
            _onShelfItemRecommendPriceByServer = 0;

            _onShelfItemMinNumber = 1;
            _onShelfItemMaxNumber = 1;

            var depositDataTable = TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType.SVT_AUCTION_DEPOSIT);
            _depositBaseValue = depositDataTable != null ? depositDataTable.Value : 1;
        }

        //界面的基础数据
        private void InitBaseView()
        {
            if (titleLabel != null)
            {
                titleLabel.text = TR.Value(_isTimeOverItem == true 
                    ? "auction_new_time_over_title_label" 
                    : "auction_new_sell_item_title");
            }

            if (otherPlayerControl != null)
                otherPlayerControl.InitOtherPlayerControlBaseView(_isTreasureItem,
                    OnSendWorldAuctionQueryItemPriceListReq,
                    OnSendWorldAuctionQueryItemTransListReq);
        }

        //上架的物品的信息
        private void InitOnShelfItemInfo()
        {
            InitOnShelfItemBaseInfo();
            InitOnShelfItemDetailInfo();
            InitOnShelfButtonRoot();
        }

        //准备上架道具的Item和名字
        private void InitOnShelfItemBaseInfo()
        {
            if (onShelfItemName != null)
            {
                var itemNameStr = AuctionNewUtility.GetQualityColorString(_onShelfItemData.Name,
                    _onShelfItemTable.Color);
                onShelfItemName.text = itemNameStr;
            }

            if (onShelfItemRoot != null)
            {
                var onShelfComItem = onShelfItemRoot.GetComponentInChildren<ComItem>();
                if (onShelfComItem == null)
                    onShelfComItem = ComItemManager.Create(onShelfItemRoot);

                if (onShelfComItem != null)
                {
                    onShelfComItem.Setup(_onShelfItemData, OnShowOnShelfItemData);
                    onShelfComItem.SetShowTreasure(_isTreasureItem);
                }
            }
        }

        //上架物品的详细信息
        private void InitOnShelfItemDetailInfo()
        {
            ResetOnShelfItemRoot();
            //准备上架的物品是珍品
            if (_isTreasureItem == true)
            {
                if (treasureItemRoot != null)
                    treasureItemRoot.CustomActive(true);

                InitOnShelfTreasureItem();
            }
            else
            {
                //准备商家的物品是非珍品
                if (normalItemRoot != null)
                    normalItemRoot.CustomActive(true);
                InitOnShelfNormalItem();
            }
        }

        //手续费和押金
        private void InitOnShelfItemCommonInfo()
        {
            var vipRate = (int)(Utility.GetCurVipLevelPrivilegeData(VipPrivilegeTable.eType.AUTION_VIP_COMMISSION_PRIVILEGE) * 100.0f);
            //珍品和非珍品
            if (_isTreasureItem == true)
            {
                if (treasureFeeTitleLabel != null)
                    treasureFeeTitleLabel.text = string.Format(TR.Value("auction_new_sell_item_noble_fee"),
                        PlayerBaseData.GetInstance().VipLevel);
                if (treasureFeeValueLabel != null)
                    treasureFeeValueLabel.text = string.Format(TR.Value("auction_new_fee_rate"), vipRate);
            }
            else
            {
                if (normalFeeTitleLabel != null)
                    normalFeeTitleLabel.text = string.Format(TR.Value("auction_new_sell_item_noble_fee"),
                        PlayerBaseData.GetInstance().VipLevel);
                if (normalFeeValueLabel != null)
                    normalFeeValueLabel.text = string.Format(TR.Value("auction_new_fee_rate"), vipRate);
            }
            //押金
            UpdateItemDepositInfo();
        }

        private void InitOnShelfTreasureItem()
        {
            InitOnShelfItemCommonInfo();

            if (_isTimeOverItem == true)
            {
                UpdateOnShelfItemInfoByPrice(_onShelfItemSinglePrice);
            }
        }

        private void InitOnShelfNormalItem()
        {
            InitOnShelfItemCommonInfo();

            var strengthLevelRate = 0;
            if (AuctionNewUtility.IsMagicCardItem((uint)_onShelfItemData.TableID) == true)
            {
                strengthLevelRate = AuctionNewUtility.GetMagicCardStrengthenAddition(_onShelfItemData);
            }
            else
            {
                if (_onShelfItemData != null)
                {
                    if (_onShelfItemData.EquipType == EEquipType.ET_REDMARK)
                    {
                        strengthLevelRate = AuctionNewUtility.GetRedEquipStrengthLvAdditionalPriceRate(
                            _onShelfItemData);
                    }
                    else
                    {
                        strengthLevelRate = AuctionNewUtility.GetNormalEquipStrengthLvAdditionalPriceRate(
                            _onShelfItemData.StrengthenLevel);
                    }
                }
            }

            _onShelfItemRecommendPrice = AuctionNewUtility.GetBasePrice((ulong) _onShelfItemTable.RecommendPrice,
                strengthLevelRate);

            //推荐价格
            if (recommendPriceLabel != null)
            {
                //附魔卡，价格由服务器下发
                if (AuctionNewUtility.IsMagicCardItem((uint) _onShelfItemData.TableID) == true)
                {
                    recommendPriceLabel.text = "0";
                }
                else
                {
                    //非附魔卡
                    recommendPriceLabel.text = _onShelfItemRecommendPrice.ToString();
                }
            }
            
            if (avaragePriceLabel != null)
                avaragePriceLabel.text = _onShelfItemAveragePrice.ToString();
            //对于普通道具，初始化的时候更新数量按钮的状态
            UpdateItemNumberInfo();

            if (normalItemPriceKeyBoardButton != null)
            {
                //非珍品的装备和称号可以点击弹出价格键盘
                if (_onShelfItemData.Type == ItemTable.eType.EQUIP || _onShelfItemData.Type == ItemTable.eType.FUCKTITTLE)
                {
                    normalItemPriceKeyBoardButton.enabled = true;
                }
                else
                {
                    normalItemPriceKeyBoardButton.enabled = false;
                }
            }

            //过期的商品改变数量的键盘无效
            if (_isTimeOverItem == true)
            {
                if(normalItemNumberKeyBoardButton != null)
                    normalItemNumberKeyBoardButton.enabled = false;
            }
        }

        private void ResetOnShelfItemRoot()
        {
            if (treasureItemRoot != null)
                treasureItemRoot.CustomActive(false);

            if(normalItemRoot != null)
                normalItemRoot.CustomActive(false);
        }

        private void UpdateOnShelfItemInfo()
        {
            //平均价格
            if (avaragePriceLabel != null)
            {
                avaragePriceLabel.text = _onShelfItemAveragePrice.ToString();
            }

            //更新附魔卡的推荐价格
            if (_onShelfItemData != null 
                && AuctionNewUtility.IsMagicCardItem((uint) _onShelfItemData.TableID) == true)
            {
                //服务器下发的推荐价格大于0
                if (_onShelfItemRecommendPriceByServer > 0)
                {
                    if (recommendPriceLabel != null)
                        recommendPriceLabel.text = _onShelfItemRecommendPriceByServer.ToString();
                }
            }

            //更新价格按钮的状态
            UpdatePriceButtonState();
            UpdateOnShelfItemInfoByPrice(_onShelfItemSinglePrice);
        }

        //根据道具数量的变化，更新相关的数值
        private void UpdateOnShelfItemInfoByItemNumber(int number)
        {
            _onShelfItemNumber = number;
            UpdateOnShelfItemInfoByPrice(_onShelfItemSinglePrice);
        }

        //根据单价的变化，更新相关的数值
        private void UpdateOnShelfItemInfoByPrice(int singlePrice)
        {
            _onShelfItemSinglePrice = singlePrice;
            _onShelfItemTotalPrice = _onShelfItemSinglePrice * _onShelfItemNumber;
            //珍品价格
            if (_isTreasureItem == true)
            {
                //价格输入说明隐藏，显示价格
                CommonUtility.UpdateTextVisible(treasureItemPriceInputLabel, false);
                CommonUtility.UpdateTextVisible(treasureItemSinglePriceLabel, true);
                if (treasureItemSinglePriceLabel != null)
                {
                    treasureItemSinglePriceLabel.text = Utility.ToThousandsSeparator((ulong) _onShelfItemSinglePrice);
                }

                if (treasureItemTotalPriceLabel != null)
                {
                    treasureItemTotalPriceLabel.text = Utility.ToThousandsSeparator((ulong) _onShelfItemTotalPrice);
                }
            }
            else
            {
                //非珍品价格
                if (normalItemSinglePriceLabel != null)
                {
                    normalItemSinglePriceLabel.text = Utility.ToThousandsSeparator((ulong)_onShelfItemSinglePrice);
                }
                if (normalItemTotalPriceLabel != null)
                {
                    normalItemTotalPriceLabel.text = Utility.ToThousandsSeparator((ulong)_onShelfItemTotalPrice);
                }
            }

            UpdateItemDepositInfo();
        }

        //更新押金
        private void UpdateItemDepositInfo()
        {
            _depositValue = Mathf.FloorToInt((float)_onShelfItemTotalPrice * _depositBaseValue / 1000.0f);
            if (_depositValue < DepositMinValue)
            {
                _depositValue = DepositMinValue;
            }

            if (_depositValue > DepositMaxValue)
                _depositValue = DepositMaxValue;

            if (_isTreasureItem == true)
            {
                if (treasureDepositValueLabel != null)
                    treasureDepositValueLabel.text = _depositValue.ToString();
            }
            else
            {
                if (normalDepositValueLabel != null)
                    normalDepositValueLabel.text = _depositValue.ToString();
            }
        }

        //更新按钮
        private void InitOnShelfButtonRoot()
        {
            //过期商品
            if (_isTimeOverItem == true)
            {
                if (normalButtonRoot != null)
                    normalButtonRoot.CustomActive(false);
                if (timeOverButtonRoot != null)
                    timeOverButtonRoot.CustomActive(true);
            }
            else
            {
                if (normalButtonRoot != null)
                    normalButtonRoot.CustomActive(true);
                if (timeOverButtonRoot != null)
                    timeOverButtonRoot.CustomActive(false);
            }
        }

        #endregion

        #region UIEvent

        //收到最近交易道具的数据
        private void OnAuctionNewWorldQueryItemTransListResSucceed(UIEvent uiEvent)
        {
            var worldAuctionQueryItemTransListRes =
                AuctionNewDataManager.GetInstance().GetWorldAuctionQueryItemTransListRes();

            if (worldAuctionQueryItemTransListRes == null)
                return;

            if (_onShelfItemData == null)
                return;

            if (worldAuctionQueryItemTransListRes.itemTypeId != _onShelfItemData.TableID)
                return;

            if (worldAuctionQueryItemTransListRes.strengthen != _onShelfItemData.StrengthenLevel)
                return;

            if (otherPlayerControl != null)
                otherPlayerControl.InitOtherPlayerSellRecordItemList(worldAuctionQueryItemTransListRes.transList);
        }

        //收到在售和公示物品的列表
        private void OnReceiveWorldAuctionQueryItemPriceListResSucceed(UIEvent uiEvent)
        {
            var worldAuctionQueryItemPriceListRes =
                AuctionNewDataManager.GetInstance().GetWorldAuctionQueryItemPriceListRes();

            if(worldAuctionQueryItemPriceListRes == null)
                return;

            if (worldAuctionQueryItemPriceListRes.itemTypeId != _onShelfItemData.TableID)
                return;

            if (worldAuctionQueryItemPriceListRes.strengthen != _onShelfItemData.StrengthenLevel)
                return;

            //1：公示；2：在售
            if (worldAuctionQueryItemPriceListRes.auctionState == (byte)AuctionGoodState.AGS_PUBLIC)
            {
                //更新其他玩家的在售数据
                if (otherPlayerControl != null)
                    otherPlayerControl.InitOtherPlayerOnNoticeItemList(worldAuctionQueryItemPriceListRes.actionItems);
            }
            else if (worldAuctionQueryItemPriceListRes.auctionState == (byte)AuctionGoodState.AGS_ONSALE)
            {
                //更新其他玩家的在售数据
                if (otherPlayerControl != null)
                    otherPlayerControl.InitOtherPlayerOnSaleItemList(worldAuctionQueryItemPriceListRes.actionItems);
            }
        }

        //收到上架物品的价格信息
        private void OnReceiveWorldAuctionQueryItemPriceResSucceed(UIEvent uiEvent)
        {
            var worldAuctionQueryItemPriceRes = AuctionNewDataManager.GetInstance().GetWorldAuctionQueryItemPriceRes();

            //返回的Res不存在
            if (worldAuctionQueryItemPriceRes == null)
            {
                return;
            }

            //ItemTableId 不同
            if (worldAuctionQueryItemPriceRes.itemTypeId != _onShelfItemData.TableID)
            {
                return;
            }

            //ItemStrengthLevel 不同
            if (worldAuctionQueryItemPriceRes.strengthen != _onShelfItemData.StrengthenLevel)
            {
                return;
            }

            //更新其他玩家的在售数据
            if (otherPlayerControl != null)
                otherPlayerControl.InitOtherPlayerOnSaleItemList(worldAuctionQueryItemPriceRes.actionItems);

            //待上架道具价格相关内容
            //Item的平均值和最值
            _onShelfItemAveragePrice = (int)worldAuctionQueryItemPriceRes.visAverPrice;
            
            //推荐价格
            _onShelfItemRecommendPriceByServer = (int) worldAuctionQueryItemPriceRes.recommendPrice;

            //最值
            if (worldAuctionQueryItemPriceRes.minPrice > worldAuctionQueryItemPriceRes.maxPrice)
            {
                _onShelfItemMinSinglePrice = (int)worldAuctionQueryItemPriceRes.maxPrice;
                _onShelfItemMaxSinglePrice = (int)worldAuctionQueryItemPriceRes.minPrice;
            }
            else
            {
                _onShelfItemMinSinglePrice = (int)worldAuctionQueryItemPriceRes.minPrice;
                _onShelfItemMaxSinglePrice = (int)worldAuctionQueryItemPriceRes.maxPrice;
            }

            //非过期的商品
            if (_isTimeOverItem == false)
                _onShelfItemSinglePrice = 0; //珍品默认为0，

            //非珍品需要获得价格区间
            if (_isTreasureItem == false)
            {
                //非珍品单价默认为平均值
                _onShelfItemSinglePrice = _onShelfItemAveragePrice;

                _onShelfItemPricesArray = AuctionNewUtility.GetOnShelfItemPriceArray(_onShelfItemAveragePrice,
                    _onShelfItemMinSinglePrice,
                    _onShelfItemMaxSinglePrice,
                    _onShelfItemMaxPriceRate);
                if (_onShelfItemPricesArray == null || _onShelfItemPricesArray.Length <= 0)
                {
                    Logger.LogErrorFormat("AuctionNewOnShelfView OnShelfItemPrices is null or OnShelfItemPrice length is zero");
                    return;
                }

                _onShelfItemPricesNumber = _onShelfItemPricesArray.Length;

               UpdateOnShelfItemPriceByLastTime();

                //更新在售物品的信息
                UpdateOnShelfItemInfo();
            }
            else
            {
                //珍品, 非过期商品
                if (_isTimeOverItem == false)
                {
                    UpdateOnShelfItemPriceByLastTime(true);
                }
            }
        }

        //如果当前上架的商品和上一次上架商品一样，则更新价格
        private void UpdateOnShelfItemPriceByLastTime(bool isTreasure = false)
        {
            var previewItemPrice = 0;
            var isOnShelfSameItemLastTime = AuctionNewDataManager.GetInstance().
                IsOnShelfSameItemLastTime(_onShelfItemData.TableID,
                    _onShelfItemData.StrengthenLevel,
                    ref previewItemPrice);

            //与上一件商品形同，并且价格处在最值之间
            if (isOnShelfSameItemLastTime == true)
            {
                if (previewItemPrice >= _onShelfItemMinSinglePrice
                    && previewItemPrice <= _onShelfItemMaxSinglePrice)
                {
                    //保存上一次的价格
                    _onShelfItemSinglePrice = previewItemPrice;
                    //珍品的话，显示价格
                    if (isTreasure == true)
                    {
                        UpdateOnShelfItemInfoByPrice(_onShelfItemSinglePrice);
                    }
                }
            }
        }

        #endregion

        #region KeyBoardInput

        private void OnCommonKeyBoardInput(UIEvent uiEvent)
        {
            if (uiEvent == null
                || uiEvent.Param1 == null || uiEvent.Param2 == null)
            {
                Logger.LogErrorFormat("OnReceiveKeyBoardInput Error");
                return;
            }

            var inputType = (CommonKeyBoardInputType) uiEvent.Param1;
            var inputValue = (ulong) uiEvent.Param2;

            if (inputType == CommonKeyBoardInputType.ChangeNumber)
            {
                UpdateValueByUserInput(inputValue);
            }
            else if (inputType == CommonKeyBoardInputType.Finished)
            {
                UpdateValueByFinishInput(inputValue);
                _onShelfItemKeyBoardInputType = AuctionNewOnShelfItemKeyBoardInputType.None;
            }
        }

        //键盘关闭，更新相应的输入，输入应介入最值之间
        private void UpdateValueByFinishInput(ulong keyBoardInputValue)
        {
            var inputValue = (int)keyBoardInputValue;
            switch (_onShelfItemKeyBoardInputType)
            {
                //珍品的价格
                case AuctionNewOnShelfItemKeyBoardInputType.TreasureItemPrice:
                    _onShelfItemSinglePrice =
                        CommonUtility.GetMiddleValue(inputValue, _onShelfItemMinSinglePrice,
                            _onShelfItemMaxSinglePrice);
                    UpdateOnShelfItemInfoByPrice(_onShelfItemSinglePrice);
                    break;
                //非珍品数量
                case AuctionNewOnShelfItemKeyBoardInputType.NormalItemNumber:
                    _onShelfItemNumber = CommonUtility.GetMiddleValue(inputValue,
                        _onShelfItemMinNumber, _onShelfItemMaxNumber);
                    UpdateItemNumberInfo();
                    UpdateOnShelfItemInfoByItemNumber(_onShelfItemNumber);
                    break;
                //非珍品价格
                case AuctionNewOnShelfItemKeyBoardInputType.NormalItemPrice:
                    _onShelfItemSinglePrice = CommonUtility.GetMiddleValue(inputValue,
                        _onShelfItemMinSinglePrice, _onShelfItemMaxSinglePrice);
                    UpdatePriceButtonState();
                    UpdateOnShelfItemInfoByPrice(_onShelfItemSinglePrice);
                    break;
                default:
                    break;
            }
        }

        //键盘输入的数字改变时，进行相应更新
        private void UpdateValueByUserInput(ulong keyBoardInputValue)
        {
            var inputValue = (int) keyBoardInputValue;
            switch (_onShelfItemKeyBoardInputType)
            {
                //珍品的价格
                case AuctionNewOnShelfItemKeyBoardInputType.TreasureItemPrice:
                    _onShelfItemSinglePrice = inputValue;
                    UpdateOnShelfItemInfoByPrice(_onShelfItemSinglePrice);
                    break;
                //非珍品数量
                case AuctionNewOnShelfItemKeyBoardInputType.NormalItemNumber:
                    _onShelfItemNumber = inputValue;
                    UpdateItemNumberInfo();
                    UpdateOnShelfItemInfoByItemNumber(_onShelfItemNumber);
                    break;
                //非珍品价格
                case AuctionNewOnShelfItemKeyBoardInputType.NormalItemPrice:
                    _onShelfItemSinglePrice = inputValue;
                    UpdatePriceButtonState();
                    UpdateOnShelfItemInfoByPrice(_onShelfItemSinglePrice);
                    break;
                default:
                    break;
            }
        }
        
        //非珍品数量的输入键盘
        private void OnNormalItemNumberKeyBoardButtonClick()
        {
            _onShelfItemKeyBoardInputType = AuctionNewOnShelfItemKeyBoardInputType.NormalItemNumber;
            CommonUtility.OnOpenCommonKeyBoardFrame(new Vector3(570, 220, 0), (ulong)_onShelfItemNumber,
                (ulong)_onShelfItemMaxNumber);
        }

        //非珍品价格的输入键盘
        private void OnNormalItemPriceKeyBoardButtonClick()
        {
            _onShelfItemKeyBoardInputType = AuctionNewOnShelfItemKeyBoardInputType.NormalItemPrice;
            CommonUtility.OnOpenCommonKeyBoardFrame(new Vector3(650, 130, 0),
                (ulong) _onShelfItemSinglePrice,
                (ulong) _onShelfItemMaxSinglePrice);
        }

        //珍品价格输入的键盘
        private void OnTreasureItemPriceKeyBoardButtonClick()
        {
            _onShelfItemKeyBoardInputType = AuctionNewOnShelfItemKeyBoardInputType.TreasureItemPrice;
            CommonUtility.OnOpenCommonKeyBoardFrame(new Vector3(650, 130, 0),
                (ulong) _onShelfItemSinglePrice,
                (ulong) _onShelfItemMaxSinglePrice);
        }
        #endregion

        #region ItemPriceAndNumber

        //增加数量
        private void OnAddNumberButtonClick()
        {
            if (_onShelfItemNumber < _onShelfItemMaxNumber)
                _onShelfItemNumber += 1;
            else
            {
                _onShelfItemNumber = _onShelfItemMaxNumber;
            }

            UpdateItemNumberInfo();
            UpdateOnShelfItemInfoByItemNumber(_onShelfItemNumber);
        }

        //减少数量
        private void OnMinusNumberButtonClick()
        {
            if (_onShelfItemNumber > _onShelfItemMinNumber)
            {
                _onShelfItemNumber -= 1;
            }
            else
            {
                _onShelfItemNumber = _onShelfItemMinNumber;
            }

            UpdateItemNumberInfo();
            UpdateOnShelfItemInfoByItemNumber(_onShelfItemNumber);
        }

        //增加价格
        private void OnAddPriceButtonClick()
        {
            if (IsNormalItemPriceArrayInvalid() == true)
                return;

            if (AuctionNewUtility.IsPriceInPriceArray(_onShelfItemPricesArray, _onShelfItemSinglePrice) == true)
            {
                var index = AuctionNewUtility.GetPriceIndexInPriceArray(_onShelfItemPricesArray, _onShelfItemSinglePrice);
                if (index >= _onShelfItemPricesNumber - 1)
                {
                    _onShelfItemSinglePrice = _onShelfItemPricesArray[_onShelfItemPricesNumber - 1];
                }
                else
                {
                    index += 1;
                    _onShelfItemSinglePrice = _onShelfItemPricesArray[index];
                }
            }
            else
            {
                var nextIndex = AuctionNewUtility.GetNextIndexInPriceArray(_onShelfItemPricesArray, _onShelfItemSinglePrice);
                _onShelfItemSinglePrice = _onShelfItemPricesArray[nextIndex];
            }

            UpdatePriceButtonState();
            UpdateOnShelfItemInfoByPrice(_onShelfItemSinglePrice);
        }

        //减少价格
        private void OnMinusPriceButtonClick()
        {
            if (IsNormalItemPriceArrayInvalid() == true)
                return;

            if (AuctionNewUtility.IsPriceInPriceArray(_onShelfItemPricesArray, _onShelfItemSinglePrice) == true)
            {
                var index = AuctionNewUtility.GetPriceIndexInPriceArray(_onShelfItemPricesArray, _onShelfItemSinglePrice);
                if (index <= 0)
                {
                    _onShelfItemSinglePrice = _onShelfItemPricesArray[0];
                }
                else
                {
                    index -= 1;
                    _onShelfItemSinglePrice = _onShelfItemPricesArray[index];
                }
            }
            else
            {
                var preIndex = AuctionNewUtility.GetPreIndexInPriceArray(_onShelfItemPricesArray, _onShelfItemSinglePrice);
                _onShelfItemSinglePrice = _onShelfItemPricesArray[preIndex];
            }

            UpdatePriceButtonState();
            UpdateOnShelfItemInfoByPrice(_onShelfItemSinglePrice);
        }

        //物品的价格是否非法
        private bool IsNormalItemPriceArrayInvalid()
        {
            if (_onShelfItemAveragePrice <= 0 || _onShelfItemPricesArray == null || _onShelfItemPricesArray.Length <= 0)
            {
                return true;
            }

            return false;
        }

        //更新数量的按钮状态 和道具数量的展示
        private void UpdateItemNumberInfo()
        {
            if (normalItemNumberLabel != null)
                normalItemNumberLabel.text = _onShelfItemNumber.ToString();

            //道具数量最小
            if (_onShelfItemNumber <= _onShelfItemMinNumber)
            {
                CommonUtility.UpdateButtonState(minusNumberButton, minusNumberButtonGray, false);
            }
            else
            {
                CommonUtility.UpdateButtonState(minusNumberButton, minusNumberButtonGray, true);
            }

            //道具数量最大
            if (_onShelfItemNumber >= _onShelfItemMaxNumber)
            {
                CommonUtility.UpdateButtonState(addNumberButton, addNumberButtonGray, false);
            }
            else
            {
                CommonUtility.UpdateButtonState(addNumberButton, addNumberButtonGray, true);
            }
        }
        
        //更新价格按钮的状态
        private void UpdatePriceButtonState()
        {
            //价格小于最小值
            if (_onShelfItemSinglePrice <= _onShelfItemMinSinglePrice)
            {
                CommonUtility.UpdateButtonState(minusPriceButton, minusPriceButtonGray, false);
            }
            else
            {
                CommonUtility.UpdateButtonState(minusPriceButton, minusPriceButtonGray, true);
            }

            //价格大于最大值
            if (_onShelfItemSinglePrice >= _onShelfItemMaxSinglePrice)
            {
                CommonUtility.UpdateButtonState(addPriceButton, addPriceButtonGray, false);
            }
            else
            {
                CommonUtility.UpdateButtonState(addPriceButton, addPriceButtonGray, true);
            }
        }
        
        #endregion

        #region OnShelfItem
        private void OnItemOnShelfButtonClick()
        {
            //ItemData is null
            if (_onShelfItemData == null)
            {
                return;
            }

            //ItemTable is null
            if (_onShelfItemTable == null)
            {
                Logger.LogErrorFormat("AuctionSellFrame OnItemOnShelfButtonClick ItemTable is null and tableId is {0}",
                    _onShelfItemData.TableID);
                return;
            }
            
            //安全锁相关
            if (SecurityLockDataManager.GetInstance().CheckSecurityLock())
            {
                return;
            }

            //非过期的物品上架，进行检查
            if (_isTimeOverItem == false)
            {
                //上架数量检查
                if (_auctionNewOnShelfItemData.SelfOnShelfItemNum >= _auctionNewOnShelfItemData.MaxShelfNum)
                {
                    SystemNotifyManager.SystemNotify(1088);
                    return;
                }
            }

            //绑定金币手续费检查
            if ((int)PlayerBaseData.GetInstance().BindGold < _depositValue)
            {
                SystemNotifyManager.SystemNotify(1093);
                return;
            }

            //上架数量是否合法
            if (_onShelfItemNumber < 1)
            {
                SystemNotifyManager.SystemNotify(1095);
                return;
            }

            //上架数量大于背包持有数量
            if (_onShelfItemNumber > _onShelfItemMaxNumber)
            {
                SystemNotifyManager.SystemNotify(1096);
                return;
            }

            //总价格低于1金币
            if (_onShelfItemTotalPrice < 1)
            {
                SystemNotifyManager.SystemNotify(1095);
                return;
            }
            
            //非珍品的时效道具，剩余时间不足2天，不能上架
            //非珍品
            if (_isTreasureItem == false)
            {
                //时效道具
                if (AuctionNewUtility.IsItemOwnerTimeValid(_onShelfItemData.TableID) == true)
                {
                    //过期商品
                    if (_isTimeOverItem == true)
                    {
                        //重新上架的商品，过期商品，使用auctionBaseInfo
                        if (_auctionBaseInfo != null)
                        {
                            //过期商品，不在有效期内，无法上架
                            if (AuctionNewUtility.IsItemInValidTimeInterval(_auctionBaseInfo.itemDueTime) ==
                                false)
                            {
                                //无法重新上架提示
                                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("auction_new_item_not_on_shelf_again_by_time_invalid"));
                                return;
                            }

                            //时间不足2天,无法上架
                            if (AuctionNewUtility.IsTimeItemCanOnShelf(_auctionBaseInfo.itemDueTime) == false)
                            {
                                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("auction_new_item_on_shelf_time_out"));
                                return;
                            }
                        }
                    }
                    else
                    {
                        //非过期商品, 背包中的商品
                        //时间不足2天,无法上架
                        if (AuctionNewUtility.IsTimeItemCanOnShelf((uint) _onShelfItemData.DeadTimestamp) == false)
                        {
                            SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("auction_new_item_on_shelf_time_out"));
                            return;
                        }
                    }
                }
            }


            //过期的珍品重新上架，价格相等的时候，进行相应的提示。
            if (_isTimeOverItem == true
                && _isTreasureItem == true)
            {
                if (_auctionBaseInfo != null && _auctionBaseInfo.price == _onShelfItemSinglePrice)
                {
                    SystemNotifyManager.SystemNotify(10016, OnSendOnShelfReq);
                    return;
                }
            }

            //是否显示物品上架的提示（珍品和非珍品）
            var isShowOnShelfTip = false;
            //珍品
            if (_isTreasureItem == true)
            {
                isShowOnShelfTip = AuctionNewDataManager.GetInstance().IsShowOnShelfTipOfTreasureItem(OnCloseFrame,
                    OnSendOnShelfReq);
            }
            else
            {
                //非珍品
                isShowOnShelfTip = AuctionNewDataManager.GetInstance().IsShowOnShelfTipOfNormalItem(OnCloseFrame,
                    OnSendOnShelfReq);
            }
            //显示Tips，直接返回
            if (isShowOnShelfTip == true)
                return;

            //发送上架消息
            SendOnShelfReq(_onShelfItemData, _onShelfItemTotalPrice, _onShelfItemNumber);

            OnCloseFrame();
        }

        //发送消息，进行上架
        private void OnSendOnShelfReq()
        {
            SendOnShelfReq(_onShelfItemData, _onShelfItemTotalPrice, _onShelfItemNumber);
            OnCloseFrame();
        }

        private void SendOnShelfReq(ItemData itemData, int totalPrice,int itemNumber)
        {
            if (itemData == null)
            {
                Logger.LogErrorFormat("ItemData is null");
                return;
            }

            //正常上架
            if (_isTimeOverItem == false)
            {
                AuctionNewDataManager.GetInstance().SendOnShelfReq(itemData, totalPrice, itemNumber);

            }
            else
            {
                //过期商品重新上架
                AuctionNewDataManager.GetInstance()
                    .SendOnShelfReq(itemData, totalPrice, itemNumber, 1, _auctionItemGuid);
            }
        }

        //过期商品重新上架的取回
        private void OnRecoverButtonClick()
        {
            if (_isTimeOverItem == false)
                return;

            if (_auctionBaseInfo == null)
                return;

            //直接取回
            AuctionNewDataManager.GetInstance().SendDownShelfItemRequest(_auctionItemGuid);
            //关闭界面
            OnCloseFrame();
        }

        //过期商品重新上架
        private void OnShelfAgainButtonClick()
        {
            OnItemOnShelfButtonClick();
        }

        #endregion

        //请求在售或者公示的列表
        private void OnSendWorldAuctionQueryItemPriceListReq(byte auctionState)
        {
            AuctionNewDataManager.GetInstance().SendWorldAuctionQueryItemPriceListReq(auctionState,
                _onShelfItemData);
        }

        //请求最近销售的列表
        private void OnSendWorldAuctionQueryItemTransListReq()
        {
            AuctionNewDataManager.GetInstance().SendWorldAuctionQueryItemTransListReq(_onShelfItemData);
        }

        //显示ItemTipFrame
        private void OnShowOnShelfItemData(GameObject obj, ItemData itemData)
        {
            if (_isTimeOverItem == false)
            {
                AuctionNewDataManager.GetInstance().OnShowItemDetailTipFrame(itemData);
            }
            else
            {
                //过期商品
                AuctionNewDataManager.GetInstance().OnShowItemDetailTipFrame(itemData,
                    _auctionItemGuid);
            }
        }

        private void OnCloseFrame()
        {
            ClientSystemManager.GetInstance().CloseFrame<AuctionNewOnShelfFrame>();
        }
    }
}