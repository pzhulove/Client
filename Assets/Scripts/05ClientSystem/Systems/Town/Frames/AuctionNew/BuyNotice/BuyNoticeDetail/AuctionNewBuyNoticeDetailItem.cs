using System;
using System.Collections;
using System.Collections.Generic;
using Protocol;
using ProtoTable;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;


namespace GameClient
{

    public delegate void OnAuctionNewDetailItemClick();

    public class AuctionNewBuyNoticeDetailItem : MonoBehaviour
    {

        private AuctionBaseInfo _auctionBaseInfo;
        private AuctionNewMainTabType _mainTabType;
        private ComItem _detailComItem;
        private float _baseIntervalTime = 0;
        private bool _isTreasure = false;

        private int _itemNoticeCount = 0;  //商品关注的次数
        private bool _isOwnerNotice = false; //自己是否已经关注

        [SerializeField] private GameObject itemRoot;
        [SerializeField] private Text itemName;
        [SerializeField] private Text itemScore;
        [SerializeField] private Text itemPriceLabel;
        [SerializeField] private Image itemCostImage;
        [SerializeField] private Text itemPriceValue;
        [SerializeField] private Text itemCountTime;
        [SerializeField] private CountDownTimeController countDownTimeController;

        [Space(10)] [HeaderAttribute("ItemTime")] [Space(10)]
        [SerializeField] private GameObject itemTimeRoot;
        [SerializeField] private Text itemLeftTimeText;
        [SerializeField] private Text itemTimeInvalidText;

        [Space(10)] [HeaderAttribute("Chat")] [Space(10)]
        [SerializeField] private Button chatButton;

        [Space(10)] [HeaderAttribute("Notice")] [Space(10)]
        [SerializeField] private GameObject noticeButtonRoot;

        [SerializeField] private ComButtonWithCd noticeButtonWithCd;
        //[SerializeField] private Button noticeButton;
        [SerializeField] private Image ownerNoticeImage;
        [SerializeField] private Image ownerNotNoticeImage;
        [SerializeField] private Text noticeNumber;

        [Space(10)]
        [HeaderAttribute("treasureInfo")]
        [SerializeField] private Button itemBuyButton;

        private void Awake()
        {
            BindEvents();
        }

        private void OnDestroy()
        {
            UnBindEvents();
            ResetData();
        }

        private void OnEnable()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnAuctionNewReceiveNoticeReqSucceed,
                OnReceiveNoticeReqSucceed);
        }

        private void OnDisable()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnAuctionNewReceiveNoticeReqSucceed,
                OnReceiveNoticeReqSucceed);
        }

        private void BindEvents()
        {
            if (itemBuyButton != null)
            {
                itemBuyButton.onClick.RemoveAllListeners();
                itemBuyButton.onClick.AddListener(OnDetailItemBuyButtonClick);
            }

            if (chatButton != null)
            {
                chatButton.onClick.RemoveAllListeners();
                chatButton.onClick.AddListener(OnChatButtonClick);
            }

            if (noticeButtonWithCd != null)
            {
                noticeButtonWithCd.SetButtonListener(OnNoticeButtonClick);
            }
        }

        private void UnBindEvents()
        {
            if (itemBuyButton != null)
                itemBuyButton.onClick.RemoveAllListeners();

            if(chatButton != null)
                chatButton.onClick.RemoveAllListeners();

            if (noticeButtonWithCd != null)
                noticeButtonWithCd.ResetButtonListener();
        }

        private void ResetData()
        {
            _auctionBaseInfo = null;
            _mainTabType = AuctionNewMainTabType.None;
            _isTreasure = false;
            _isOwnerNotice = false;
            _itemNoticeCount = 0;
        }

        public void InitItem(AuctionNewMainTabType mainTabType,
            AuctionBaseInfo auctionBaseInfo)
        {
            _baseIntervalTime = 0;
            _mainTabType = mainTabType;
            _auctionBaseInfo = auctionBaseInfo;

            if (_auctionBaseInfo == null)
            {
                Logger.LogError("BuyNoticeDetail InitItem auctionBaseInfo is null");
                return;
            }

            _isTreasure = _auctionBaseInfo.isTreas == 1 ? true : false;

            //_isOwnerNotice = false;
            //_itemNoticeCount = 0;

            InitItemView();
        }

        private void InitItemView()
        {

            var itemData = ItemDataManager.CreateItemDataFromTable((int)_auctionBaseInfo.itemTypeId);
            var itemTable = TableManager.GetInstance().GetTableItem<ItemTable>((int)_auctionBaseInfo.itemTypeId);
            if (itemData == null || itemTable == null)
            {
                Logger.LogErrorFormat("ItemData or ItemTable is null and itemTypeid is {0}",
                    _auctionBaseInfo.itemTypeId);
                return;
            }

            itemData.Count = (int)_auctionBaseInfo.num;
            itemData.StrengthenLevel = (int) _auctionBaseInfo.strengthed;
            //设置装备的类型和增幅的类型
            AuctionNewUtility.UpdateItemDataByEquipType(itemData,
                _auctionBaseInfo);
            
            if (itemName != null)
                itemName.text = AuctionNewUtility.GetQualityColorString(itemData.Name, itemTable.Color);

            if (itemRoot != null)
            {
                _detailComItem = itemRoot.GetComponentInChildren<ComItem>();
                if (_detailComItem == null)
                    _detailComItem = ComItemManager.Create(itemRoot);

                if (_detailComItem != null)
                {
                    _detailComItem.Setup(itemData, OnShowDetailItemTip);
                    _detailComItem.SetShowTreasure(_isTreasure);
                }
            }
            
            //评分
            if (itemScore != null)
            {
                if (_auctionBaseInfo.itemScore <= 0)
                {
                    itemScore.gameObject.CustomActive(false);
                }
                else
                {
                    itemScore.gameObject.CustomActive(true);
                    itemScore.text = string.Format(TR.Value("auction_new_itemDetail_score_value"),
                        _auctionBaseInfo.itemScore);
                }
            }

            //ItemTimeInvalid
            InitItemTime();
            
            if (itemPriceLabel != null)
                itemPriceLabel.text = TR.Value("auction_new_sell_item_single_price");

            if (itemPriceValue != null)
            {
                var singlePrice = (ulong) _auctionBaseInfo.price;
                if (_auctionBaseInfo.num > 0)
                    singlePrice = (ulong) _auctionBaseInfo.price / (ulong) _auctionBaseInfo.num;

                itemPriceValue.text = Utility.GetShowPrice(singlePrice);
                itemPriceValue.text = Utility.ToThousandsSeparator(singlePrice);
            }
            
            if (itemCountTime != null)
            {
                //购买页面
                if (_mainTabType != AuctionNewMainTabType.AuctionNoticeType)
                {
                    itemCountTime.gameObject.CustomActive(false);
                }
                else
                {
                    //公示页面
                    itemCountTime.gameObject.CustomActive(true);
                    itemCountTime.text = CountDownTimeUtility.GetCountDownTimeByHourMinute(
                        _auctionBaseInfo.publicEndTime,
                        TimeManager.GetInstance().GetServerTime());

                    if (countDownTimeController != null)
                    {
                        countDownTimeController.EndTime = _auctionBaseInfo.publicEndTime;
                        countDownTimeController.InitCountDownTimeController();
                    }
                }
            }
            InitItemTreasureInfo();
        }

        private void InitItemTime()
        {
            CommonUtility.UpdateGameObjectVisible(itemTimeRoot, false);

            bool isItemOwnerTimeValid = AuctionNewUtility.IsItemOwnerTimeValid((int)_auctionBaseInfo.itemTypeId);
            //非时效道具
            if (isItemOwnerTimeValid == false)
                return;
            
            //时效道具
            CommonUtility.UpdateGameObjectVisible(itemTimeRoot, true);

            bool isItemInValidTimeInterval = AuctionNewUtility.IsItemInValidTimeInterval(_auctionBaseInfo.itemDueTime);
            if (isItemInValidTimeInterval == true)
            {
                //有效期内
                CommonUtility.UpdateTextVisible(itemTimeInvalidText, false);

                CommonUtility.UpdateTextVisible(itemLeftTimeText, true);
                if (itemLeftTimeText != null)
                {
                    var leftTimeStr = AuctionNewUtility.GetTimeValidItemLeftTimeStr(_auctionBaseInfo.itemDueTime);
                    itemLeftTimeText.text = leftTimeStr;
                }
            }
            else
            {
                CommonUtility.UpdateTextVisible(itemLeftTimeText, false);
                CommonUtility.UpdateTextVisible(itemTimeInvalidText, true);
            }
        }


        //如果这个道具是珍品的话，初始化珍品相关的东西：聊天按钮和关注按钮
        private void InitItemTreasureInfo()
        {
            if (_isTreasure == false)
            {
                UpdateTreasureRelationButton(false);
                return;
            }
            else
            {
                UpdateTreasureRelationButton(true);
                UpdateNoticeState();
            }

            //noticeButton 重置
            if(noticeButtonWithCd != null)
                noticeButtonWithCd.Reset();
        }

        public void OnItemRecycle()
        {
            ResetData();
            if (_mainTabType == AuctionNewMainTabType.AuctionNoticeType)
            {
                if (countDownTimeController != null)
                {
                    countDownTimeController.ResetCountDownTimeController();
                }
            }
        }

        private void OnDetailItemBuyButtonClick()
        {

            if(_auctionBaseInfo == null)
                return;
            
            //公示商品无法购买
            if (_mainTabType == AuctionNewMainTabType.AuctionNoticeType)
            {
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("auction_new_onSale_notice_can_not_buy"));
                return;
            }
            else
            {
                //购买界面，进行购买具体的商品
                var itemData = TableManager.GetInstance().GetTableItem<ItemTable>((int) _auctionBaseInfo.itemTypeId);
                if (itemData == null)
                {
                    Logger.LogErrorFormat("BuyNoticeDetailItem itemData is null and itemTypeId is {0}",
                        _auctionBaseInfo.itemTypeId);
                    return;
                }

                AuctionNewUtility.OnOpenAuctionNewBuyItemFrame(_auctionBaseInfo);

                //if(ClientSystemManager.GetInstance().IsFrameOpen<AuctionBuyFrame>() == true)
                //    ClientSystemManager.GetInstance().CloseFrame<AuctionBuyFrame>();

                //var buyItemData = new BuyItemData();
                //buyItemData.SetItemDataByAuction(_auctionBaseInfo);
                //ClientSystemManager.GetInstance().OpenFrame<AuctionBuyFrame>(FrameLayer.Middle, buyItemData);
            }
        }

        private void OnShowDetailItemTip(GameObject obj, ItemData itemData)
        {
            AuctionNewDataManager.GetInstance().OnShowItemDetailTipFrame(itemData,
                _auctionBaseInfo.guid);
        }

        //关注和聊天按钮是否显示
        private void UpdateTreasureRelationButton(bool flag)
        {
            if (chatButton != null)
                chatButton.gameObject.CustomActive(flag);


            //关注按钮
            if (noticeButtonRoot != null)
                noticeButtonRoot.CustomActive(flag);

        }

        private void OnChatButtonClick()
        {
            if (_auctionBaseInfo == null)
            {
                Logger.LogErrorFormat("AuctionBaseInfo is null");
                return;
            }
            
            AuctionNewUtility.ChatWithOnShelfItemOwner(_auctionBaseInfo.owner);
        }

        #region NoticeRelation
        //发送关注消息
        private void OnNoticeButtonClick()
        {
            if (_auctionBaseInfo == null)
            {
                Logger.LogErrorFormat("The AuctionBaseInfo is null");
                return;
            }
            AuctionNewDataManager.GetInstance().OnSendWorldActionNoticeReq(_auctionBaseInfo.guid);
        }

        //关注消息的服务器返回
        private void OnReceiveNoticeReqSucceed(UIEvent uiEvent)
        {
            if(uiEvent == null)
                return;

            //自身道具的guid与需要关注的guid不一致，返回
            var noticeItemGuid = (ulong) uiEvent.Param1;
            if(_auctionBaseInfo == null 
               || _auctionBaseInfo.guid != noticeItemGuid)
                return;

            ShowNoticeResultEffect();            
            UpdateNoticeState();
        }

        //关注操作飘字
        private void ShowNoticeResultEffect()
        {
            //关注成功
            if (_auctionBaseInfo.attent == 1)
            {
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("auction_new_notice_succeed"));
            }
            else
            {
                //取消关注
                SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("auction_new_notice_cancel"));
            }
        }

        //自己是否关注，以及关注的数量
        private void UpdateNoticeState()
        {
            if (_auctionBaseInfo == null)
            {
                Logger.LogErrorFormat("AuctionBaseInfo is null");
                return;
            }

            //0表示自己没有关注，1表示关注
            bool isNotice = (int)_auctionBaseInfo.attent != 0;
            UpdateNoticeFlag(isNotice);

            if (noticeNumber != null)
                noticeNumber.text = _auctionBaseInfo.attentNum.ToString();
        }

        //自己是否关注
        private void UpdateNoticeFlag(bool flag)
        {
            if (ownerNoticeImage != null)
                ownerNoticeImage.gameObject.CustomActive(flag);

            if (ownerNotNoticeImage != null)
                ownerNotNoticeImage.gameObject.CustomActive(!flag);
        }
        #endregion

    }
}