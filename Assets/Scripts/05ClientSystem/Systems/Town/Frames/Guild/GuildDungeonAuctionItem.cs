using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using UnityEngine.Assertions;
using Protocol;
using ProtoTable;
using DG.Tweening;


namespace GameClient
{
    using UIItemData = AwardItemData;

    public class GuildDungeonAuctionItem : MonoBehaviour
    {
        [SerializeField]
        ComItem item0 = null;

        [SerializeField]
        Text txtItem0Name = null;

        [SerializeField]
        ComItem item1 = null;

        [SerializeField]
        Text txtItem1Name = null;

        [SerializeField]
        Text txtLeftTime = null;

        [SerializeField]
        Text txtItemState = null;

        [SerializeField]
        Text txtBuyNowPrice = null;     // 一口价

        [SerializeField]
        Button btnBuy = null;  

        [SerializeField]
        Text txtCurBiddingPrice = null; // 当前竞价

        [SerializeField]
        Button btnBidding = null;       // 竞价按钮

        [SerializeField]
        Text txtBiddingBtnText = null;  // 下一次竞价

        [SerializeField]
        Image myBidFlag = null;         // 我出价标志

        [SerializeField]
        GameObject jia = null;

        [SerializeField]
        GameObject itemInfo1 = null;

        GuildDataManager.GuildAuctionItemData auctionItemData = null;

        ulong guid = 0;

        // Use this for initialization
        void Start()
        {
            //auctionItemData = null;

            btnBuy.SafeSetOnClickListener(() => 
            {
                ulong guidTemp = guid;
                GuildDataManager.GuildAuctionItemData auctionItemDataTemp = GetGuildAuctionItemDataByGUID(guidTemp);

                if(auctionItemDataTemp != null && txtItem0Name != null && txtItem1Name != null)
                {
                    PopUpCostMoneyMsgBox(TR.Value("auction_buy_now_confirm", auctionItemDataTemp.buyNowPrice, txtItem0Name.text, txtItem1Name.text),
                            MoneyType.MT_POINT,
                            auctionItemDataTemp.buyNowPrice,
                            () =>
                            {
                                GuildDataManager.GetInstance().SendWorldGuildAuctionFixReq(guidTemp);
                            });                           
                }
                
            });

            btnBidding.SafeSetOnClickListener(() => 
            {
                ulong guidTemp = guid;
                GuildDataManager.GuildAuctionItemData auctionItemDataTemp = GetGuildAuctionItemDataByGUID(guidTemp);

                if (auctionItemDataTemp != null)
                {
                    if (auctionItemDataTemp.nextBiddingPrice >= auctionItemDataTemp.buyNowPrice) // 竞拍价超过了一口价，提示玩家直接一口价购买
                    {
                        PopUpCostMoneyMsgBox(TR.Value("auction_bidding_greater_than_buy_now_price", auctionItemDataTemp.buyNowPrice),
                            MoneyType.MT_POINT,
                            auctionItemDataTemp.buyNowPrice,
                            () =>
                            {
                                GuildDataManager.GetInstance().SendWorldGuildAuctionFixReq(guidTemp);
                            });
                    }
                    else
                    {
                        PopUpCostMoneyMsgBox(TR.Value("auction_bidding_confirm", auctionItemDataTemp.nextBiddingPrice),
                            MoneyType.MT_POINT,
                            auctionItemDataTemp.nextBiddingPrice,
                            () =>
                            {
                                GuildDataManager.GetInstance().SendWorldGuildAuctionBidReq(guidTemp, auctionItemDataTemp.nextBiddingPrice);
                            });                       
                    }                    
                }
            });
        }

        private void OnDestroy()
        {
            //auctionItemData = null;
        }

        // Update is called once per frame
        void Update()
        {
            UdpateItemTimeLeftOrState();
        }

        void ShowItemTip(GameObject go, ItemData itemData)
        {
            if (null != itemData)
            {
                ItemTipManager.GetInstance().CloseAll();
                ItemTipManager.GetInstance().ShowTip(itemData);
            }
        }

        void SetComItemData(ComItem comItem, UIItemData uIItemData)
        {
            if(comItem == null || uIItemData == null)
            {
                return;
            }

            ItemData itemData = ItemDataManager.CreateItemDataFromTable(uIItemData.ID);
            if (itemData != null)
            {
                itemData.Count = uIItemData.Num;
                comItem.Setup(itemData, ShowItemTip);
            }

            return;
        }

        string GetColorName(UIItemData uIItemData)
        {
            if(uIItemData == null)
            {
                return "";
            }

            ItemData itemData = ItemDataManager.CreateItemDataFromTable(uIItemData.ID);
            if (itemData != null)
            {
                return itemData.GetColorName();
            }

            return "";
        }

        // 货币类型，和ItemTable.eSubType中的货币类型值一样
        enum MoneyType
        {
            MT_POINT = ItemTable.eSubType.POINT,
            MT_BIND_POINT = ItemTable.eSubType.BindPOINT,
            MT_GOLD= ItemTable.eSubType.GOLD,
            MT_BIND_GOLD = ItemTable.eSubType.BindGOLD,
        }

        // 弹框提示玩家消耗单一货币进行某种操作
        void PopUpCostMoneyMsgBox(string msgContent,MoneyType moneyType,ulong nCount,Action action)
        {
            if(string.IsNullOrEmpty(msgContent))
            {
                return;
            }

            if(action == null)
            {
                return;
            }

            SystemNotifyManager.SysNotifyMsgBoxOkCancel(msgContent, () =>
            {
                CostItemManager.CostInfo costInfo = new CostItemManager.CostInfo
                {
                    nMoneyID = ItemDataManager.GetInstance().GetMoneyIDByType((ItemTable.eSubType)(moneyType)),
                    nCount = (int)nCount
                };

                if (costInfo != null)
                {
                    CostItemManager.GetInstance().TryCostMoneyDefault(costInfo, () =>
                    {
                        action();
                    });
                }
            });

            return;
        }

        void UdpateItemTimeLeftOrState()
        {      
            auctionItemData = GetGuildAuctionItemDataByGUID(guid);
            if(auctionItemData == null)
            {
                return;
            }

            if (auctionItemData.auctionItemState == GuildDataManager.AuctionItemState.Prepare)
            {
                txtLeftTime.CustomActive(true);
                txtItemState.CustomActive(false);
                txtLeftTime.SafeSetText(TR.Value("auction_prepare", Function.GetLeftTime((int)auctionItemData.statusEndStamp, (int)TimeManager.GetInstance().GetServerTime(), ShowtimeType.OnlineGift)));
            }
            else if (auctionItemData.auctionItemState == GuildDataManager.AuctionItemState.InAuction)
            {
                txtLeftTime.CustomActive(true);
                txtItemState.CustomActive(false);
                txtLeftTime.SafeSetText(Function.GetLeftTime((int)auctionItemData.statusEndStamp, (int)TimeManager.GetInstance().GetServerTime(), ShowtimeType.OnlineGift));
            }           
            else if (auctionItemData.auctionItemState == GuildDataManager.AuctionItemState.SoldOut)
            {
                txtItemState.CustomActive(true);
                txtLeftTime.CustomActive(false);
                txtItemState.SafeSetText(TR.Value("auction_sold_out"));
            }
            else if (auctionItemData.auctionItemState == GuildDataManager.AuctionItemState.AbortiveAuction)
            {
                txtItemState.CustomActive(true);
                txtLeftTime.CustomActive(false);
                txtItemState.SafeSetText(TR.Value("auction_abortive"));
            }

            return;
        }

        GuildDataManager.GuildAuctionItemData GetGuildAuctionItemDataByGUID(ulong GUID)
        {
            List<GuildDataManager.GuildAuctionItemData> guildAuctionItemDatas = null;
            if(GuildDungeonAuctionFrame.GetAuctionFrameType() == GuildDungeonAuctionFrame.FrameType.GuildAuction)
            {
                guildAuctionItemDatas = GuildDataManager.GetInstance().GetGuildAuctionItemDatasForGuildAuction();
            }
            else if(GuildDungeonAuctionFrame.GetAuctionFrameType() == GuildDungeonAuctionFrame.FrameType.WorldAuction)
            {
                guildAuctionItemDatas = GuildDataManager.GetInstance().GetGuildAuctionItemDatasForWorldAuction();
            }

            if(guildAuctionItemDatas != null)
            {
                for(int i = 0;i < guildAuctionItemDatas.Count;i++)
                {
                    if(guildAuctionItemDatas[i].guid == GUID)
                    {
                        return guildAuctionItemDatas[i];
                    }
                }
            }

            return null;
        }

        public void SetUp(object data)
        {
            if(data == null)
            {
                return;
            }

            if(!(data is GuildDataManager.GuildAuctionItemData))
            {
                return;
            }

            GuildDataManager.GuildAuctionItemData guildAuctionItemData = data as GuildDataManager.GuildAuctionItemData;       
            auctionItemData = guildAuctionItemData;
            guid = auctionItemData.guid;

            SetComItemData(item0, guildAuctionItemData.itemData0);
            SetComItemData(item1, guildAuctionItemData.itemData1);

            txtItem0Name.SafeSetText(GetColorName(guildAuctionItemData.itemData0));
            txtItem1Name.SafeSetText(GetColorName(guildAuctionItemData.itemData1));

            UdpateItemTimeLeftOrState();

            txtBuyNowPrice.SafeSetText(TR.Value("auction_buy_now_price", guildAuctionItemData.buyNowPrice));

            if (guildAuctionItemData.curbiddingPrice == 0)
            {
                txtCurBiddingPrice.SafeSetText(TR.Value("auction_no_price"));
                txtBiddingBtnText.SafeSetText(TR.Value("auction_first_bidding", guildAuctionItemData.nextBiddingPrice));
            }
            else
            {
                txtCurBiddingPrice.SafeSetText(TR.Value("auction_cur_bidding_price", guildAuctionItemData.curbiddingPrice));
                txtBiddingBtnText.SafeSetText(TR.Value("auction_next_bidding_price", guildAuctionItemData.nextBiddingPrice));
            }

            if (guildAuctionItemData.auctionItemState == GuildDataManager.AuctionItemState.InAuction)
            {
                btnBuy.SafeSetGray(false);
                btnBidding.SafeSetGray(false);
            }
            else
            {
                btnBuy.SafeSetGray(true);
                btnBidding.SafeSetGray(true);
            }

            myBidFlag.CustomActive(guildAuctionItemData.bidRoleId == PlayerBaseData.GetInstance().RoleID);

            if(guildAuctionItemData.itemData1 != null)
            {
                jia.CustomActive(guildAuctionItemData.itemData1.ID != 0);
                itemInfo1.CustomActive(guildAuctionItemData.itemData1.ID != 0);
            }            

            return;
        }
    }
}


