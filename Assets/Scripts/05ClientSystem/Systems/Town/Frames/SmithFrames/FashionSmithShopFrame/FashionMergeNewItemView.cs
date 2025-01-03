using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProtoTable;
using Scripts.UI;
using Object = System.Object;
using Network;
using Protocol;

namespace GameClient
{

    public class FashionMergeNewItemView : MonoBehaviour
    {

        [SerializeField] private Text title;
        [SerializeField] private Text description;

        [Space(10)]
        [HeaderAttribute("LinkItem")]
        [SerializeField] private GameObject linkItem;
        [SerializeField] private Button jumpButton;

        [Space(10)]
        [HeaderAttribute("FastBuyItem")]
        [SerializeField] private GameObject fastBuyItem;
        [SerializeField] private Text buyItemMoneyCount;
        [SerializeField] private Button buyItemButton;
        [SerializeField] private Text buyItemName;
        [SerializeField] private GameObject buyItemRoot;
        [SerializeField] private GameObject mIntergralMallInfoRoot;
        [SerializeField] private Text mIntergralInfoText;

        [Space(10)]
        [HeaderAttribute("NormalItemList")]
        [SerializeField] private ComUIListScript fashionItemList;

        [Space(5)]
        [SerializeField] private Button closeButton;

        private ItemTable.eSubType _fashionPart;
        private bool _isLeft;
        private bool _needBlue;
        private List<ItemData> _fashionItemDataList;
        private MallItemInfo _mallItemInfo = null;
        private const string FashionLink = "<type=framename param=2|0|1|{0} value=GameClient.MallNewFrame>";

        private void Awake()
        {
            BindUiEventSystem();
        }

        private void BindUiEventSystem()
        {
            if (closeButton != null)
            {
                closeButton.onClick.RemoveAllListeners();
                closeButton.onClick.AddListener(OnCloseFrame);
            }

            if (jumpButton != null)
            {
                jumpButton.onClick.RemoveAllListeners();
                jumpButton.onClick.AddListener(OnJumpToFashionMall);
            }


            if (fashionItemList != null)
            {
                fashionItemList.Initialize();
                fashionItemList.onBindItem += OnBindItem;
                fashionItemList.onItemVisiable += OnItemVisiable;
                fashionItemList.onItemChageDisplay += OnItemChangeDisplay;
                fashionItemList.onItemSelected += OnItemSelected;
            }
        }

        private void OnDestroy()
        {
            UnBindUiEventSystem();
        }

        private void UnBindUiEventSystem()
        {
            if (closeButton != null)
                closeButton.SafeRemoveAllListener();

            if (jumpButton != null)
            {
                jumpButton.onClick.RemoveAllListeners();
                jumpButton = null;
            }

            if (fashionItemList != null)
            {
                fashionItemList.onBindItem -= OnBindItem;
                fashionItemList.onItemVisiable -= OnItemVisiable;
                fashionItemList.onItemChageDisplay -= OnItemChangeDisplay;
                fashionItemList.onItemSelected -= OnItemSelected;
            }

            buyItemMoneyCount = null;
            if (buyItemButton != null)
            {
                buyItemButton.onClick.RemoveAllListeners();
                buyItemButton = null;
            }
        }

        #region ComUIListScriptBind
        private Object OnBindItem(GameObject go)
        {
            if (go == null)
                return null;

            return go.GetComponent<ComFashionMergeItemEx>();
        }

        private void OnItemVisiable(ComUIListElementScript item)
        {
            if (item == null)
                return;

            var fashionMergeItemEx = item.gameObjectBindScript as ComFashionMergeItemEx;
            if (fashionMergeItemEx == null)
                return;

            if (_fashionItemDataList != null
                && item.m_index >= 0
                && item.m_index < _fashionItemDataList.Count
            )
            {
                fashionMergeItemEx.OnItemVisible(_fashionItemDataList[item.m_index]);
            }
        }

        private void OnItemChangeDisplay(ComUIListElementScript item, bool bSelected)
        {
            if (item == null)
                return;

            var fashionMergeItemEx = item.gameObjectBindScript as ComFashionMergeItemEx;
            if (fashionMergeItemEx == null)
                return;

            fashionMergeItemEx.OnItemChangeDisplay(bSelected);
        }

        private void OnItemSelected(ComUIListElementScript item)
        {
            if (item == null)
                return;       

            var fashionMergeItemEx = item.gameObjectBindScript as ComFashionMergeItemEx;
            if(fashionMergeItemEx == null)
            {
                return;
            }

            if(fashionMergeItemEx.ItemData == null)
            {
                return;
            }

            if(fashionMergeItemEx.ItemData.bFashionItemLocked)
            {
                if(fashionItemList != null)
                {
                    fashionItemList.SelectElement(-1);
                }

                SystemNotifyManager.SystemNotify(1000107);
                return;
            }

            //发送UI事件
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnFashionNormalItemSelected, fashionMergeItemEx);
            OnCloseFrame();
        }
        #endregion

        #region InitData
        public void InitData(ItemTable.eSubType fashionPart, bool isLeft,bool needBlueFashion)
        {
            _fashionPart = fashionPart;
            _isLeft = isLeft;
            _needBlue = needBlueFashion;
            InitTitle();
            InitItemList();
        }

        private void InitTitle()
        {
            if (description != null)
                description.text = TR.Value("fashion_merge_new_item_description");

            switch (_fashionPart)
            {
                case ItemTable.eSubType.FASHION_HAIR:
                    title.text = TR.Value("fashion_merge_new_item_hair");
                    break;
                case ItemTable.eSubType.FASHION_HEAD:
                    title.text = TR.Value("fashion_merge_new_item_head");
                    break;
                case ItemTable.eSubType.FASHION_SASH:
                    title.text = TR.Value("fashion_merge_new_item_sash");
                    break;
                case ItemTable.eSubType.FASHION_CHEST:
                    title.text = TR.Value("fashion_merge_new_item_chest");
                    break;
                case ItemTable.eSubType.FASHION_LEG:
                    title.text = TR.Value("fashion_merge_new_item_leg");
                    break;
                case ItemTable.eSubType.FASHION_EPAULET:
                    title.text = TR.Value("fashion_merge_new_item_epaulet");
                    break;
            }
        }
        
        private void InitItemList()
        {
            if (_fashionItemDataList == null)
                _fashionItemDataList = new List<ItemData>();
            _fashionItemDataList.Clear();

            ComFashionMergeItemEx.LoadAllEquipments(ref _fashionItemDataList, _fashionPart, _isLeft,_needBlue);
            fashionItemList.SetElementAmount(_fashionItemDataList.Count);

            if (_fashionItemDataList != null && _fashionItemDataList.Count > 0)
            {
                linkItem.CustomActive(false);
                fastBuyItem.CustomActive(false);
            }
            else
            {
                linkItem.CustomActive(true);
                InitFastBuyItem();
            }
        }

        private void InitFastBuyItem()
        {
            fastBuyItem.CustomActive(true);

            buyItemButton.onClick.RemoveAllListeners();
            var jobTabData = TableManager.GetInstance().GetTableItem<JobTable>(PlayerBaseData.GetInstance().JobTableID);
            if(jobTabData == null)
                return;

            var mallItemTableData =
                TableManager.GetInstance().GetTableItem<MallItemTable>(jobTabData.FashionMergeFastBuyID);
            if(mallItemTableData == null)
                return;

            var itemId = GetFastBuyItemId(mallItemTableData);
            var itemTableData = TableManager.GetInstance().GetTableItem<ItemTable>(itemId);
            if(itemTableData == null)
                return;

            var itemDetailData = ItemDataManager.CreateItemDataFromTable(itemTableData.ID);
            if (itemDetailData != null)
            {
                var comItem = buyItemRoot.GetComponentInChildren<ComItem>();
                if (comItem == null)
                {
                    comItem = ComItemManager.Create(buyItemRoot);
                }
                comItem.Setup(itemDetailData,OnShowTips);
            }

            buyItemName.text = itemTableData.Name;


            var msg = new WorldGetMallItemByItemIdReq
            {
                itemId = (uint) itemId
            };
            var netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, msg);

            WaitNetMessageManager.GetInstance().Wait<WorldGetMallItemByItemIdRes>(OnWorldGetMallItemByItemIdRes,
                false);

        }

        private void OnWorldGetMallItemByItemIdRes(WorldGetMallItemByItemIdRes msgRet)
        {
            if(msgRet == null)
                return;

            _mallItemInfo = msgRet.mallItem;
            if(_mallItemInfo == null)
                return;
            if(buyItemMoneyCount != null)
            {
                buyItemMoneyCount.text = _mallItemInfo.discountprice.ToString();
            }
            if(buyItemButton != null)
            {
                buyItemButton.onClick.RemoveAllListeners();
                buyItemButton.onClick.AddListener(OnFastBuyItemClick);
            }

            if (mIntergralMallInfoRoot != null)
            {
                mIntergralMallInfoRoot.CustomActive(_mallItemInfo.multiple > 0);
            }

            if (mIntergralInfoText != null)
            {
                int price = MallNewUtility.GetTicketConvertIntergalNumnber((int)_mallItemInfo.discountprice) * _mallItemInfo.multiple;
                string mContent = string.Empty;
                if (_mallItemInfo.multiple <= 1)
                {
                    mContent = TR.Value("mall_buy_intergral_single_multiple_desc", price);
                }
                else
                {
                    mContent = TR.Value("mall_buy_intergral_many_multiple_desc", price, _mallItemInfo.multiple);
                }

                mIntergralInfoText.text = mContent;
            }
        }
        #endregion

        #region FastBuyItem
        private void OnFastBuyItemClick()
        {
            var costInfo = new CostItemManager.CostInfo();
            if (_mallItemInfo.moneytype == (int)ItemTable.eSubType.BindPOINT)
            {
                costInfo.nMoneyID = ItemDataManager.GetInstance().GetMoneyIDByType(ItemTable.eSubType.BindPOINT);
            }
            else
            {
                costInfo.nMoneyID = ItemDataManager.GetInstance().GetMoneyIDByType(ItemTable.eSubType.POINT);
            }
            costInfo.nCount = Utility.GetMallRealPrice(_mallItemInfo);
            var notifyCount = string.Format(TR.Value("fashion_merge_new_buy_item"), costInfo.nCount);
            if (_mallItemInfo.multiple > 0)
            {
                int price = MallNewUtility.GetTicketConvertIntergalNumnber(costInfo.nCount) * _mallItemInfo.multiple;
                string mContent = string.Empty;
                if (_mallItemInfo.multiple <= 1)
                {
                    mContent = TR.Value("mall_fast_buy_intergral_single_multiple_desc", price);
                }
                else
                {
                    mContent = TR.Value("mall_fast_buy_intergral_many_multiple_desc", price, _mallItemInfo.multiple,string.Empty);
                }

                notifyCount += mContent;
            }

            SystemNotifyManager.SysNotifyMsgBoxOkCancel(notifyCount, () =>
            {
                var reqId = _mallItemInfo.id;
                CostItemManager.GetInstance().TryCostMoneyDefault(costInfo, () =>
                {
                    if (_mallItemInfo.multiple > 0)
                    {
                        string content = string.Empty;
                        //积分商城积分等于上限值
                        if ((int)PlayerBaseData.GetInstance().IntergralMallTicket == MallNewUtility.GetIntergralMallTicketUpper() &&
                             MallNewDataManager.GetInstance().bItemMallIntergralMallScoreIsEqual == false)
                        {
                            content = TR.Value("mall_buy_intergral_mall_score_equal_upper_value_desc");

                            MallNewUtility.CommonIntergralMallPopupWindow(content, MallNewUtility.ItemMallIntergralMallScoreIsEqual, () => { OnSendWorldMallBuy(reqId); });
                        }
                        else
                        {
                            int ticketConvertScoreNumber = MallNewUtility.GetTicketConvertIntergalNumnber(costInfo.nCount) * _mallItemInfo.multiple;

                            int allIntergralScore = (int)PlayerBaseData.GetInstance().IntergralMallTicket + ticketConvertScoreNumber;

                            //购买道具后商城积分超出上限值
                            if (allIntergralScore > MallNewUtility.GetIntergralMallTicketUpper() &&
                               (int)PlayerBaseData.GetInstance().IntergralMallTicket != MallNewUtility.GetIntergralMallTicketUpper() &&
                                MallNewDataManager.GetInstance().bItemMallIntergralMallScoreIsExceed == false)
                            {
                                content = TR.Value("mall_buy_intergral_mall_score_exceed_upper_value_desc",
                                                   (int)PlayerBaseData.GetInstance().IntergralMallTicket,
                                                   MallNewUtility.GetIntergralMallTicketUpper(),
                                                   MallNewUtility.GetIntergralMallTicketUpper() - (int)PlayerBaseData.GetInstance().IntergralMallTicket);

                                MallNewUtility.CommonIntergralMallPopupWindow(content, MallNewUtility.ItemMallIntergralMallScoreIsExceed, () => { OnSendWorldMallBuy(reqId); });
                            }
                            else
                            {//未超出
                                OnSendWorldMallBuy(reqId);
                            }
                        }
                    }
                    else
                    {
                        OnSendWorldMallBuy(reqId);
                    }
                });
            });
        }

        private void OnSendWorldMallBuy(uint reqId)
        {
            var req = new WorldMallBuy
            {
                itemId = reqId,
                num = 1,
            };

            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, req);
            WaitNetMessageManager.GetInstance().Wait<WorldMallBuyRet>(ret =>
            {
                if (ret.mallitemid == reqId)
                {
                    //发送UI事件
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnFashionFastItemBuyFinished);

                    OnCloseFrame();
                }
            }, false);
        }

        private int GetFastBuyItemId(MallItemTable mallItemTableData)
        {
            string strItemId = "-1";
            int itemId = -1;

            try
            {
                switch (_fashionPart)
                {
                    case ItemTable.eSubType.FASHION_HEAD:
                        strItemId = mallItemTableData.giftpackitems.Split('|')[0].Split(':')[0];
                        //1
                        break;
                    case ItemTable.eSubType.FASHION_SASH:
                        strItemId = mallItemTableData.giftpackitems.Split('|')[4].Split(':')[0];
                        //5
                        break;
                    case ItemTable.eSubType.FASHION_CHEST:
                        strItemId = mallItemTableData.giftpackitems.Split('|')[1].Split(':')[0];
                        //2
                        break;
                    case ItemTable.eSubType.FASHION_LEG:
                        strItemId = mallItemTableData.giftpackitems.Split('|')[3].Split(':')[0];
                        //4
                        break;
                    case ItemTable.eSubType.FASHION_EPAULET:
                        strItemId = mallItemTableData.giftpackitems.Split('|')[2].Split(':')[0];
                        //3
                        break;
                }

                int.TryParse(strItemId, out itemId);
            }
            catch (Exception e)
            {
                Logger.LogErrorFormat("{0}", e.ToString());
                return -1;
            }

            return itemId;
        }
        #endregion

        #region LinkJump
        private void OnJumpToFashionMall()
        {
            var fashionMallLink = GetFashionMallLink();
            ActiveManager.GetInstance().OnClickLinkInfo(fashionMallLink);
            OnCloseFrame();
        }

        private string GetFashionMallLink()
        {
            var linkInfo = string.Empty;
            var slot = GetSlotIdBySubType(FashionMergeManager.GetInstance().FashionPart);
            if (slot >= 0 && slot < 5)
            {
                linkInfo = string.Format(FashionLink, slot);
            }
            return linkInfo;
        }

        private int GetSlotIdBySubType(ItemTable.eSubType eSubType)
        {
            var slotId = -1;
            switch (eSubType)
            {
                case ItemTable.eSubType.FASHION_HAIR:
                {
                    slotId = 5;
                }
                    break;
                case ItemTable.eSubType.FASHION_HEAD:
                {
                    slotId = 0;
                }
                    break;
                case ItemTable.eSubType.FASHION_SASH:
                {
                    slotId = 4;
                }
                    break;
                case ItemTable.eSubType.FASHION_CHEST:
                {
                    slotId = 1;
                }
                    break;
                case ItemTable.eSubType.FASHION_LEG:
                {
                    slotId = 3;
                }
                    break;
                case ItemTable.eSubType.FASHION_EPAULET:
                {
                    slotId = 2;
                }
                    break;
            }
            return slotId;
        }
        #endregion

        private void OnShowTips(GameObject go, ItemData itemData)
        {
            if (itemData == null)
                return;

            ItemTipManager.GetInstance().ShowTip(itemData);
        }

        private void OnCloseFrame()
        {
            ClientSystemManager.GetInstance().CloseFrame<FashionMergeNewItemFrame>();
        }

    }
}