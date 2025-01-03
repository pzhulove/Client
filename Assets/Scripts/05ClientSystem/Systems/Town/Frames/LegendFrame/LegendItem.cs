using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Protocol;
using Network;
using ProtoTable;

namespace GameClient
{
    class LegendItemData
    {
        public ProtoTable.LegendTraceTable methodItem;
        public bool bFirst;
    }

    public enum LegendItemRegexType
    {
        LIRT_INVALID = -1,
        LIRT_SHOP_GOODS,
        LIRT_DUNGENDROP,
        LIRT_MALL_GOODS,
        LIRT_NEWSHOP_GOODS,
        LIRT_COUNT,
    }

    class LegendItem : CachedNormalObject<LegendItemData>
    {
        class NewShopItem
        {
            public int goodsId;
            public int remain;
            public int maxLimit;

            public string GetDesc()
            {
                if (maxLimit == -1)
                {
                    return "∞";
                }
                return remain + "/" + maxLimit;
            }
        }
        NewShopItem itemData = new NewShopItem();

        public Text desc;
        public GameObject goItemParent;
        public Text linkText;
        public Text goDesc;
        public Text goDesc0;
        public Image imgGo;
        public Button btnGo;
        public GameObject goLine;
        List<ComItem> comItems = new List<ComItem>();
        StateController comStateController;
        static Regex[] ms_regexs = new Regex[(int)LegendItemRegexType.LIRT_COUNT] 
        {
            new Regex(@"<type=goods id=(\d+)/>"),
            new Regex(@"<type=dungendropid id=(\d+)/>"),
            new Regex(@"<type=mallgoods id=(\d+) gotomall=([a-zA-Z]+)/>"),
            new Regex(@"<type=newgoods shopid=(\d+) goodsid=(\d+)/>"),
        };
        static string counter_key_pre = "dungeon_daily_";

        UnityEngine.Events.UnityAction callback = null;

        void _OnMallItemBuyRes(MsgDATA msg)
        {
            WorldMallBuyRet res = new WorldMallBuyRet();
            res.decode(msg.bytes);

            if (res.code == (uint)ProtoErrorCode.SUCCESS)
            {
                OnUpdate();
            }
            else
            {
                //统一在ShopNewDataManager处理
                // SystemNotifyManager.SystemNotify((int)res.code);
            }
        }

        public override void Initialize()
        {
            desc = Utility.FindComponent<Text>(goLocal, "Text");
            goItemParent = Utility.FindChild(goLocal, "Items");
            linkText = Utility.FindComponent<Text>(goLocal, "LinkText");
            goDesc = Utility.FindComponent<Text>(goLocal, "Go/Text");
            goDesc0 = Utility.FindComponent<Text>(goLocal, "Over/Text");
            imgGo = Utility.FindComponent<Image>(goLocal, "Go");
            btnGo = Utility.FindComponent<Button>(goLocal, "Go");
            goLine = Utility.FindChild(goLocal, "Line");
            comStateController = Utility.FindComponent<StateController>(goLocal, "LinkText");

            NetProcess.AddMsgHandler(WorldMallBuyRet.MsgID, _OnMallItemBuyRes);

            _RecycleAllComItems();

            btnGo.onClick.AddListener(_OnClickGo);

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ShopBuyGoodsSuccess, _RereshAllGoods);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ShopRefreshSuccess, _RebuildAllObjects);
        }

        void _RereshAllGoods(UIEvent uiEvent)
        {
            OnUpdate();
        }

        void _RebuildAllObjects(UIEvent uiEvent)
        {
            OnUpdate();
        }

        void _RecycleAllComItems()
        {
            for(int i = 0; i < comItems.Count; ++i)
            {
                ComItemManager.Destroy(comItems[i]);
            }
            comItems.Clear();
        }

        void _LoadComItems()
        {
            for (int i = 0; i < Value.methodItem.ItemIds.Count && i < Value.methodItem.ItemCounts.Count; ++i)
            {
                var comItem = ComItemManager.Create(goItemParent);
                if (null != comItem)
                {
                    var itemData = ItemDataManager.CreateItemDataFromTable(Value.methodItem.ItemIds[i]);
                    if (null != itemData)
                    {
                        itemData.Count = Value.methodItem.ItemCounts[i];
                    }
                    comItem.Setup(itemData, (GameObject obj, ItemData item) =>
                    {
                        ItemTipManager.GetInstance().ShowTip(itemData);
                    });
                    comItems.Add(comItem);
                }
            }
        }

        void _OnClickGo()
        {
            if (null != Value && null != Value.methodItem)
            {
                if(null != callback)
                {
                    callback();
                }
                else
                {
                    ActiveManager.GetInstance().OnClickLinkInfo(Value.methodItem.LinkInfo);
                }
            }
        }
        void SetGoodsLimit(int cur, int max)
        {

        }
        void _OnMatchSucceed(Match match,string orgText, LegendItemRegexType eMatchType, int isShowNumber)
        {
            switch(eMatchType)
            {
                case LegendItemRegexType.LIRT_SHOP_GOODS:
                    {

                        int iGoodId = 0;

                        if (int.TryParse(match.Groups[1].Value, out iGoodId))
                        {
                            var shopItem = TableManager.GetInstance().GetTableItem<ProtoTable.ShopItemTable>(iGoodId);
                            
                            if (null != shopItem)
                            {
                                var shopData = ShopDataManager.GetInstance().GetGoodsDataFromShop(shopItem.ShopID);
                                    
                                if (null != shopData)
                                {
                                    linkText.text = _GetShopDataLimitDesc(iGoodId, shopItem.ShopID);
                                    return;
                                }
                                else
                                {
                                    linkText.text = string.Empty;

                                    ShopDataManager.GetInstance().OpenShop(shopItem.ShopID,
                                        0, -1,
                                        () =>
                                        {
                                            linkText.text = _GetShopDataLimitDesc(iGoodId, shopItem.ShopID);
                                        },
                                        null,
                                        ShopFrame.ShopFrameMode.SFM_QUERY_NON_FRAME,
                                        0);
                                    
                                }
                            }
                        }
                    }
                    break;
                case LegendItemRegexType.LIRT_DUNGENDROP:
                    {
                        int iDungenDropID = 0;
                        if(int.TryParse(match.Groups[1].Value,out iDungenDropID))
                        {
                            var dungenDropItem = TableManager.GetInstance().GetTableItem<ProtoTable.DungeonDailyDropTable>(iDungenDropID);
                            if(null != dungenDropItem)
                            {
                                System.Text.StringBuilder builder = StringBuilderCache.Acquire();
                                builder.Append(counter_key_pre);
                                builder.Append(iDungenDropID);
                                string queryKey = builder.ToString();
                                StringBuilderCache.Release(builder);

                                int iPre = 0;
                                int iAft = dungenDropItem.DailyLimit;
                                var counterInfo = CountDataManager.GetInstance().GetCountInfo(queryKey);
                                if(null != counterInfo)
                                {
                                    iPre = (int)counterInfo.value;
                                }
                                iPre = IntMath.Max(0, iPre);
                                iPre = IntMath.Min(iPre, iAft);
                                linkText.text = string.Format("{0}/{1}", iPre, iAft);

                                if (linkText != null)
                                {
                                    //显示次数
                                    if (isShowNumber == 1)
                                    {
                                        linkText.gameObject.CustomActive(true);
                                    }
                                    else
                                    {
                                        linkText.gameObject.CustomActive(false);
                                    }
                                }

                                //显示次数
                                if (isShowNumber == 1)
                                {
                                    if (iPre >= iAft)
                                    {
                                        comStateController.Key = "over";
                                    }
                                    else
                                    {
                                        comStateController.Key = "normal";
                                    }
                                }
                                else
                                {
                                    comStateController.Key = "normal";
                                }
                            }
                        }
                    }
                    break;
                case LegendItemRegexType.LIRT_MALL_GOODS:
                    {
                        linkText.gameObject.CustomActive(true);
                        bool gotoMall = false;

                        if (!bool.TryParse(match.Groups[2].Value, out gotoMall))
                        {
                            Logger.LogError("_OnMatchSucceed gotoMall invaild");
                            return;
                        }

                        int iGoodId = 0;
                        if (int.TryParse(match.Groups[1].Value, out iGoodId))
                        {
                            var mallItem = TableManager.GetInstance().GetTableItem<ProtoTable.MallItemTable>(iGoodId);
                            if(null == mallItem)
                            {
                                break;
                            }

                            int iMallTypeID = -1;
                            ProtoTable.MallTypeTable MallData = null;
                            var mallTypeTable = TableManager.GetInstance().GetTable<ProtoTable.MallTypeTable>();
                            var enumerator = mallTypeTable.GetEnumerator();
                            while(enumerator.MoveNext())
                            {
                                var tempData = enumerator.Current.Value as ProtoTable.MallTypeTable;
                                //类型关联，而不是类型和ID相关联
                                if(tempData.MoneyID == mallItem.moneytype &&
                                    mallItem.type == (int)tempData.MallType)
                                {
                                    MallData = tempData;
                                    break;
                                }
                            }

                            int iMallSubTypeIndex = 0;
                            int JobID = 0;
                            if (null != MallData)
                            {
                                WorldMallQueryItemReq req = new WorldMallQueryItemReq();

                                if (MallData.MoneyID != 0)
                                {
                                    req.moneyType = (byte)MallData.MoneyID;
                                }

                                if (MallData.MallType == MallTypeTable.eMallType.SN_HOT)
                                {
                                    req.tagType = 1;
                                }
                                else
                                {
                                    req.tagType = 0;
                                    req.type = (byte)MallData.MallType;

                                    if (MallData.MallSubType.Count > 0 && iMallSubTypeIndex < MallData.MallSubType.Count && MallData.MallSubType[iMallSubTypeIndex] != 0)
                                    {
                                        req.subType = (byte)MallData.MallSubType[iMallSubTypeIndex];
                                    }

                                    if (MallData.ClassifyJob == 1 && JobID > 0)
                                    {
                                        req.occu = (byte)JobID;
                                    }
                                }

                                NetManager netMgr = NetManager.Instance();
                                netMgr.SendCommand(ServerType.GATE_SERVER, req);

                                WaitNetMessageManager.GetInstance().Wait<WorldMallQueryItemRet>(msgRet=>
                                {
                                    MallItemInfo mallItemInfo = null;
                                    for (int i = 0; i < msgRet.items.Length; ++i)
                                    {
                                        if (msgRet.items[i].itemid == mallItem.itemid)
                                        {
                                            mallItemInfo = msgRet.items[i];
                                            break;
                                        }
                                    }

                                    if (null != mallItemInfo)
                                    {
                                        linkText.text = _GetMallDataLimitDesc(mallItem, mallItemInfo);
                                        if (!gotoMall)
                                        {
                                            callback = () =>
                                            {
                                                ClientSystemManager.GetInstance().OpenFrame<MallBuyFrame>(FrameLayer.Middle, mallItemInfo);
                                            };
                                        }
                                    }
                                });
                            }
                        }
                    }
                    break;
                case LegendItemRegexType.LIRT_NEWSHOP_GOODS:
                    {
                        int shopId = 0;
                        int newGoodsId = 0;

                        if (!int.TryParse(match.Groups[1].Value, out shopId))
                        {
                            return;
                        }
                        if (!int.TryParse(match.Groups[2].Value, out newGoodsId))
                        {
                            return;
                        }
                        itemData.goodsId = newGoodsId;
                        linkText.text = "0/0";

                        NetProcess.AddMsgHandler(SceneShopSync.MsgID, SceneShopSyncSuc);
                        UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ShopNewBuyGoodsSuccess, NewShopBuyRetSuc);

                        var req = new SceneShopQuery();
                        req.shopId = (byte)shopId;
                        req.cache = (byte)0;

                        NetManager netMgr = NetManager.Instance();
                        netMgr.SendCommand(ServerType.GATE_SERVER, req);
                    }


                    break;
            }
        }

        void SceneShopSyncSuc(MsgDATA msg)
        {
            CustomDecoder.ProtoShop msgRet;
            var pos = 0;

            if (CustomDecoder.DecodeShop(out msgRet, msg.bytes, ref pos, msg.bytes.Length) == false)
            {
                Logger.LogErrorFormat("Open ShopNewFrame OnSyncShopItem Decode is error");
                return;
            }
            if (msgRet == null)
            {
                Logger.LogErrorFormat("Open ShopNewFrame OnSyncShopItem Decode msgRet is null");
                return;
            }
            

            for (var i = 0; i < msgRet.shopItemList.Count; i++)
            {
                var protoShopItem = msgRet.shopItemList[i];
                if (protoShopItem.shopItemId != itemData.goodsId)
                {
                    continue;
                }
                UpdateText(protoShopItem.restNum,protoShopItem.limiteNum);
            }


        }

        void NewShopBuyRetSuc(UIEvent p)
        {
            var goodsId = (int)p.Param1;
            var remain = (ushort)p.Param2;
            if (goodsId != itemData.goodsId)
            {
                return;
            }

            UpdateText(remain);

        }

        
        void UpdateText(int remain, int maxlimit = -2)
        {
            if (remain == -1)
            {
                return;
            }
            if (maxlimit != -2)
            {
                itemData.maxLimit = maxlimit;
            }
            itemData.remain = remain;
            linkText.text = itemData.GetDesc();
        }

        void _ParseLinkText(string orgText, int isShowNumber)
        {
            comStateController.Key = "normal";
            linkText.text = string.Empty;
            callback = null;

            if (string.IsNullOrEmpty(orgText))
            {
                return;
            }

            for(int i = 0; i < (int)LegendItemRegexType.LIRT_COUNT; ++i)
            {
                var match = ms_regexs[i].Match(orgText);
                if(match.Success)
                {
                    _OnMatchSucceed(match, orgText, (LegendItemRegexType)i, isShowNumber);
                    break;
                }
            }
        }



        string _GetShopDataLimitDesc(int iGoodID,int iShopID)
        {
            var shopData = ShopDataManager.GetInstance().GetGoodsDataFromShop(iShopID);
            if(null != shopData)
            {
                var goodsData = shopData.Goods.Find(x =>
                {
                    int iId = x.ID.HasValue ? x.ID.Value : 0;
                    return iId == iGoodID;
                });

                if(null == goodsData)
                {
                    return string.Empty;
                }

                var shopItem = TableManager.GetInstance().GetTableItem<ProtoTable.ShopItemTable>(iGoodID);
                if(null == shopItem)
                {
                    return string.Empty;
                }

                if(shopItem.NumLimite < 0)
                {
                    return string.Empty;
                }

                int iAft = shopItem.NumLimite;
                int iPre = goodsData.LimitBuyTimes;
                iPre = iAft - iPre;
                iPre = IntMath.Max(0, iPre);
                iPre = IntMath.Min(iPre, iAft);
                if(iPre >= iAft)
                {
                    comStateController.Key = "over";
                }
                else
                {
                    comStateController.Key = "normal";
                }

                return string.Format("{0}/{1}", iPre, iAft);
            }

            return string.Empty;
        }

        string _GetMallDataLimitDesc(ProtoTable.MallItemTable mallItem, MallItemInfo mallItemInfo)
        {
            if(null != mallItem && null != mallItemInfo)
            {
                bool bIsDaily = false;
                int limitNum = Utility.GetLeftLimitNum(mallItemInfo, ref bIsDaily);
                int iPre = mallItem.limitnum - limitNum;
                int iAft = mallItem.limitnum;
                iPre = IntMath.Max(0, iPre);
                iPre = IntMath.Min(iPre, iAft);
                if (iPre >= iAft)
                {
                    comStateController.Key = "over";
                }
                else
                {
                    comStateController.Key = "normal";
                }
                return string.Format("{0}/{1}", iPre, iAft);
            }
            return string.Empty;
        }

        public override void UnInitialize()
        {
            if(null != btnGo)
            {
                btnGo.onClick.RemoveAllListeners();
                btnGo = null;
            }

            _RecycleAllComItems();

            NetProcess.RemoveMsgHandler(WorldMallBuyRet.MsgID, _OnMallItemBuyRes);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ShopBuyGoodsSuccess, _RereshAllGoods);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ShopRefreshSuccess, _RebuildAllObjects);
            NetProcess.RemoveMsgHandler(SceneShopSync.MsgID, SceneShopSyncSuc);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ShopNewBuyGoodsSuccess, NewShopBuyRetSuc);

        }

        public override void OnUpdate()
        {
            if(null != Value && null != Value.methodItem)
            {
                desc.text = Value.methodItem.Title;
                _RecycleAllComItems();
                _LoadComItems();
                _ParseLinkText(Value.methodItem.KeyValueDesc, Value.methodItem.IsShowNumber);
                if(Value.methodItem.ActionDesc.Count > 0)
                    goDesc.text = Value.methodItem.ActionDesc[0];
                if (Value.methodItem.ActionDesc.Count > 1)
                    goDesc0.text = Value.methodItem.ActionDesc[1];
                goLine.CustomActive(true);
                // imgGo.sprite = AssetLoader.instance.LoadRes(Value.methodItem.Icons[0], typeof(Sprite)).obj as Sprite;
                ETCImageLoader.LoadSprite(ref imgGo, Value.methodItem.Icons[0]);
            }
        }
    }
}