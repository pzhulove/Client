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
using System.Collections;
using DG.Tweening;

namespace GameClient
{
    // 公会地下城拍卖界面
    public class GuildDungeonAuctionFrame : ClientFrame
    {
        #region inner define

        // 界面类型 不同类型显示的拍卖的道具不一样
        public enum FrameType
        {
            GuildAuction, // 公会拍卖
            WorldAuction, // 世界拍卖
        }
        #endregion

        #region val
        List<GuildDataManager.GuildAuctionItemData> itemDataList = null;

        private static FrameType frameType = FrameType.GuildAuction;
        private object requestGuildAuctionInfoObj = null;
        private const float requestInterval = 3.0f;
        private bool bToggleInit = true;

        #endregion

        #region ui bind
        ComUIListScript itemList = null;
        Text textTitle = null;
        Text textDesc = null;
        GameObject Help1 = null;
        GameObject Help2 = null;
        Button Close = null;
        Toggle togGuildAuctionItems = null;
        Toggle togWorldAuctionItems = null;
        GameObject worldAuctionRedPoint = null;
        GameObject guildAuctionRedPoint = null;

        #endregion

        #region override
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Guild/GuildDungeonAuction";
        }

        protected override void _OnOpenFrame()
        {
            bToggleInit = true;
            
            itemDataList = null;
            InitItems();
            BindUIEvent();

            FrameType eType = FrameType.GuildAuction;
            if (userData != null && userData is FrameType)
            {
                eType = (FrameType)userData;
            }
            if(eType == FrameType.GuildAuction)
            {
                togGuildAuctionItems.SafeSetToggleOnState(true);
                togWorldAuctionItems.SafeSetToggleOnState(false);
            }
            else if(eType == FrameType.WorldAuction)
            {
                togGuildAuctionItems.SafeSetToggleOnState(false);
                togWorldAuctionItems.SafeSetToggleOnState(true);
            }            

               

            requestGuildAuctionInfoObj = new object();
            InvokeMethod.InvokeInterval(requestGuildAuctionInfoObj,0.0f,requestInterval, float.MaxValue, null, RequestGuildAuctionItem, null);
            RequestGuildAuctionItem();

            bToggleInit = false;
        }

        protected override void _OnCloseFrame()
        {
            bToggleInit = true;

            itemDataList = null;
            frameType = FrameType.GuildAuction;            

            UnBindUIEvent();

            InvokeMethod.RmoveInvokeIntervalCall(requestGuildAuctionInfoObj);
            requestGuildAuctionInfoObj = null;
        }

        protected override void _bindExUI()
        {
            itemList = mBind.GetCom<ComUIListScript>("itemList");
            textTitle = mBind.GetCom<Text>("textTitle");
            textDesc = mBind.GetCom<Text>("textDesc");

            Help1 = mBind.GetGameObject("Help1");
            Help2 = mBind.GetGameObject("Help2");

            Close = mBind.GetCom<Button>("Close");
            Close.SafeRemoveAllListener();
            Close.SafeAddOnClickListener(() => 
            {
                frameMgr.CloseFrame(this);
            });

            togGuildAuctionItems = mBind.GetCom<Toggle>("togGuildAuctionItems");
            togGuildAuctionItems.SafeAddOnValueChangedListener((bool bValue) => 
            {
                if(bValue)
                {
                    SetFrameType(FrameType.GuildAuction);
                }                
            });

            togWorldAuctionItems = mBind.GetCom<Toggle>("togWorldAuctionItems");
            togWorldAuctionItems.SafeAddOnValueChangedListener((bool bValue) => 
            {
                if(bValue)
                {
                    SetFrameType(FrameType.WorldAuction);
                }
            });

            worldAuctionRedPoint = mBind.GetGameObject("worldAuctionRedPoint");
            guildAuctionRedPoint = mBind.GetGameObject("guildAuctionRedPoint");
        }

        protected override void _unbindExUI()
        {
            itemList = null;
            textTitle = null;
            Help1 = null;
            Help2 = null;
            textDesc = null;
            Close = null;

            togGuildAuctionItems = null;
            togWorldAuctionItems = null;

            worldAuctionRedPoint = null;
            guildAuctionRedPoint = null;
        }

        #endregion 

        #region method

        void BindUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnGuildDungeonAuctionItemsUpdate, _OnUpdateItems);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnGuildDungeonAuctionAddNewItem, _OnGuildDungeonAuctionAddNewItem);
        }

        void UnBindUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnGuildDungeonAuctionItemsUpdate, _OnUpdateItems);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnGuildDungeonAuctionAddNewItem, _OnGuildDungeonAuctionAddNewItem);
        }

        public static void RequestGuildAuctionItem()
        {
            if (frameType == FrameType.GuildAuction)
            {
                GuildDataManager.GetInstance().SendWorldGuildAuctionItemReq(GuildAuctionType.G_AUCTION_GUILD);
            }
            else if (frameType == FrameType.WorldAuction)
            {
                GuildDataManager.GetInstance().SendWorldGuildAuctionItemReq(GuildAuctionType.G_AUCTION_WORLD);
            }

            return;
        }

        public static FrameType GetAuctionFrameType()
        {
            return frameType;
        }

        void SetFrameType(FrameType eType)
        {
            frameType = eType;

            InitTitleAndHelp();
            UpdateItems();

            RequestGuildAuctionItem(); // 切换页签的时候也立即请求一次数据


            if(eType == FrameType.GuildAuction)
            {
                if(!bToggleInit)
                {
                    GuildDataManager.GetInstance().HaveNewGuildAuctonItem = false;
                }                        
            }
            else if(eType == FrameType.WorldAuction)
            {
                if(!bToggleInit)
                {
                    GuildDataManager.GetInstance().HaveNewWorldAuctonItem = false;
                }                                
            }

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnGuildDungeonAuctionAddNewItem);
        }

        void InitItems()
        {
            if(itemList == null)
            {
                return;
            }

            itemList.Initialize();

            itemList.onBindItem = (GameObject go) =>
            {
                GuildDungeonAuctionItem item = null;
                if (go)
                {
                    item = go.GetComponent<GuildDungeonAuctionItem>();
                }
               
                return item;
            };


            itemList.onItemVisiable = (var1) =>
            {
                UpdateAuctionItem(var1);

            };

            itemList.OnItemUpdate = (var1) =>
            {
                UpdateAuctionItem(var1);
            };
        }

        void UpdateAuctionItem(ComUIListElementScript var1)
        {
            if (var1 == null)
            {
                return;
            }

            int iIndex = var1.m_index;
            if (iIndex >= 0 && itemDataList != null && iIndex < itemDataList.Count)
            {
                GuildDungeonAuctionItem item = var1.gameObjectBindScript as GuildDungeonAuctionItem;
                if (item != null)
                {
                    item.SetUp(itemDataList[iIndex]);
                }
            }
        }

        void InitTitleAndHelp()
        {
            if(frameType == FrameType.GuildAuction)
            {
                Help1.CustomActive(true);
                Help2.CustomActive(false);

                //textTitle.SafeSetText(TR.Value("auction_guild_title"));
                textDesc.SafeSetText(TR.Value("auction_guild_desc"));
            }
            else if(frameType == FrameType.WorldAuction)
            {
                Help1.CustomActive(false);
                Help2.CustomActive(true);

                //textTitle.SafeSetText(TR.Value("auction_world_title"));
                textDesc.SafeSetText(TR.Value("auction_world_desc"));
            }
        }

        void CalItemDataList()
        {
            itemDataList = null;

            if(frameType == FrameType.GuildAuction)
            {
                itemDataList = GuildDataManager.GetInstance().GetGuildAuctionItemDatasForGuildAuction();
            }
            else if(frameType == FrameType.WorldAuction)
            {
                itemDataList = GuildDataManager.GetInstance().GetGuildAuctionItemDatasForWorldAuction();
            }            

            return;
        }

        void UpdateItems()
        {
            if(itemList == null)
            {
                return;
            }

            CalItemDataList();

            if(itemDataList != null)
            {
                itemList.UpdateElementAmount(itemDataList.Count);
            }
        }

        #endregion

        #region ui event
        void _OnUpdateItems(UIEvent uiEvent)
        {
            if(uiEvent.Param1 == null || !(uiEvent.Param1 is GuildAuctionType))
            {
                return;
            }

            GuildAuctionType guildAuctionType = (GuildAuctionType)uiEvent.Param1;
            if((guildAuctionType == GuildAuctionType.G_AUCTION_GUILD && GetAuctionFrameType() == FrameType.GuildAuction) ||
                (guildAuctionType == GuildAuctionType.G_AUCTION_WORLD && GetAuctionFrameType() == FrameType.WorldAuction))
            {
                UpdateItems();
            }

            return;
        }
        void _OnGuildDungeonAuctionAddNewItem(UIEvent uiEvent)
        {
            guildAuctionRedPoint.CustomActive(GuildDataManager.GetInstance().HaveNewGuildAuctonItem);
            worldAuctionRedPoint.CustomActive(GuildDataManager.GetInstance().HaveNewWorldAuctonItem);
        }
        #endregion
    }
}
