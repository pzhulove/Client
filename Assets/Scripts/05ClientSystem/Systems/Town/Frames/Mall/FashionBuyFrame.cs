using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LimitTimeGift;
using ProtoTable;
using Protocol;
using Network;

namespace GameClient
{
    
    public class FashionBuyFrame : ClientFrame
    {
        List<MallItemInfo> FashionItem = new List<MallItemInfo>();
        int BuyIndex = -1;
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/LimitTimeGift/FashionBuyFrame";
        }
        protected override void _OnOpenFrame()
        {
            if (userData == null)
            {
                Logger.LogProcessFormat("时装购买页面，传入数据为空!!!!!!");
                return;
            }
            else
            {
                List<MallItemInfo> mallitemInfo = (List<MallItemInfo>)userData;
                if(mallitemInfo.Count!=3)
                {
                    Logger.LogErrorFormat("oh FashionBuyData is error");
                }
                for(int i=0;i<mallitemInfo.Count;i++)
                {
                    FashionItem.Add(mallitemInfo[i]);
                }
            }
            InitData();
            mItemForever.isOn = true;
        }
        protected override void _OnCloseFrame()
        {
            cleardata();
            ClientSystemManager.GetInstance().CloseFrame<FashionBuyFrame>();
        }

        private void InitData()
        {
            BuyIndex = -1;
            mPrice7.text = FashionItem[0].discountprice.ToString();
            mPrice30.text = FashionItem[1].discountprice.ToString();
            mPriceForever.text = FashionItem[2].discountprice.ToString();
        }
        private void cleardata()
        {
            FashionItem.Clear();
            BuyIndex = -1;
        }

        [UIControl("PanelBG/Content/giftContent/awards/Item{0}", typeof(RectTransform), 1)]
        RectTransform[] FashionElement = new RectTransform[5];

        #region ExtraUIBind
        private Toggle mItem7 = null;
        private Toggle mItem30 = null;
        private Toggle mItemForever = null;
        private Button mBuy = null;
        private Text mPrice7 = null;
        private Text mPrice30 = null;
        private Text mPriceForever = null;
        private GameObject mAwards = null;
        private GameObject mget7 = null;
        private GameObject mget30 = null;
        private GameObject mgetforever = null;
        private SetButtonCD mSetButtonCD = null;

        protected override void _bindExUI()
        {
            mItem7 = mBind.GetCom<Toggle>("Item7");
            mItem7.onValueChanged.AddListener(_onItem7ToggleValueChange);
            mItem30 = mBind.GetCom<Toggle>("Item30");
            mItem30.onValueChanged.AddListener(_onItem30ToggleValueChange);
            mItemForever = mBind.GetCom<Toggle>("ItemForever");
            mItemForever.onValueChanged.AddListener(_onItemForeverToggleValueChange);
            mBuy = mBind.GetCom<Button>("Buy");
            mBuy.onClick.AddListener(_onBuyButtonClick);
            mPrice7 = mBind.GetCom<Text>("price7");
            mPrice30 = mBind.GetCom<Text>("price30");
            mPriceForever = mBind.GetCom<Text>("priceForever");
            mAwards = mBind.GetGameObject("awards");
            mget7 = mBind.GetGameObject("get7");
            mget30 = mBind.GetGameObject("get30");
            mgetforever = mBind.GetGameObject("getforever");
            mSetButtonCD = mBind.GetCom<SetButtonCD>("SetButtonCD");

        }

        protected override void _unbindExUI()
        {
            mItem7.onValueChanged.RemoveListener(_onItem7ToggleValueChange);
            mItem7 = null;
            mItem30.onValueChanged.RemoveListener(_onItem30ToggleValueChange);
            mItem30 = null;
            mItemForever.onValueChanged.RemoveListener(_onItemForeverToggleValueChange);
            mItemForever = null;
            mBuy.onClick.RemoveListener(_onBuyButtonClick);
            mBuy = null;
            mPrice7 = null;
            mPrice30 = null;
            mPriceForever = null;
            mAwards = null;
            mget7 = null;
            mget30 = null;
            mgetforever = null;
            mSetButtonCD = null;
        }
        #endregion

        #region Callback
        private void _onItem7ToggleValueChange(bool changed)
        {
            if (changed==true)
            {
                BuyIndex = 0;
                mget7.CustomActive(true);
                mget30.CustomActive(false);
                mgetforever.CustomActive(false);
                var MallitemData = TableManager.GetInstance().GetTableItem<MallItemTable>((int)FashionItem[0].id);
                string[] FashionID = MallitemData.giftpackitems.Split('|');
                for (int i=0;i<5;i++)
                {
                    string[] ID_true = FashionID[i].Split(':');
                    int result_ID = 0;
                    int.TryParse(ID_true[0], out result_ID);
                    ItemData ItemDetailData = ItemDataManager.CreateItemDataFromTable(result_ID);
                    if (FashionElement[i] == null)
                    {
                        Logger.LogErrorFormat("Fashionelement[{0}] is null", i);
                        return;
                    }
                    ComItem comitem = FashionElement[i].GetComponentInChildren<ComItem>();
                    if (comitem == null)
                    {
                        comitem = CreateComItem(FashionElement[i].gameObject);
                    }
                    comitem.Setup(ItemDetailData, (GameObject Obj, ItemData sitem) => { OnShowTipsFromJob(result_ID); });
                }
                
            }
        }
        
        private void _onItem30ToggleValueChange(bool changed)
        {
            /* put your code in here */
            if (changed == true)
            {
                BuyIndex = 1;
                mget7.CustomActive(false);
                mget30.CustomActive(true);
                mgetforever.CustomActive(false);
                var MallitemData = TableManager.GetInstance().GetTableItem<MallItemTable>((int)FashionItem[1].id);
                string[] FashionID = MallitemData.giftpackitems.Split('|');
                for (int i = 0; i < 5; i++)
                {
                    string[] ID_true = FashionID[i].Split(':');
                    int result_ID = 0;
                    int.TryParse(ID_true[0], out result_ID);
                    ItemData ItemDetailData = ItemDataManager.CreateItemDataFromTable(result_ID);
                    if (FashionElement[i] == null)
                    {
                        Logger.LogErrorFormat("Fashionelement[{0}] is null", i);
                        return;
                    }
                    ComItem comitem = FashionElement[i].GetComponentInChildren<ComItem>();
                    if (comitem == null)
                    {
                        comitem = CreateComItem(FashionElement[i].gameObject);
                    }
                    comitem.Setup(ItemDetailData, (GameObject Obj, ItemData sitem) => { OnShowTipsFromJob(result_ID); });
                }

            }
        }
        private void _onItemForeverToggleValueChange(bool changed)
        {
            /* put your code in here */
            if (changed == true)
            {
                BuyIndex = 2;
                mget7.CustomActive(false);
                mget30.CustomActive(false);
                mgetforever.CustomActive(true);
                var MallitemData = TableManager.GetInstance().GetTableItem<MallItemTable>((int)FashionItem[2].id);
                string[] FashionID = MallitemData.giftpackitems.Split('|');
                for (int i = 0; i < 5; i++)
                {
                    string[] ID_true = FashionID[i].Split(':');
                    int result_ID = 0;
                    int.TryParse(ID_true[0], out result_ID);
                    ItemData ItemDetailData = ItemDataManager.CreateItemDataFromTable(result_ID);
                    if (FashionElement[i] == null)
                    {
                        Logger.LogErrorFormat("Fashionelement[{0}] is null", i);
                        return;
                    }
                    ComItem comitem = FashionElement[i].GetComponentInChildren<ComItem>();
                    if (comitem == null)
                    {
                        comitem = CreateComItem(FashionElement[i].gameObject);
                    }
                    comitem.Setup(ItemDetailData, (GameObject Obj, ItemData sitem) => { OnShowTipsFromJob(result_ID); });
                }
            }
        }

        void OnShowTipsFromJob(int result)
        {
            ItemData ItemDetailData = ItemDataManager.CreateItemDataFromTable(result);
            ItemTipManager.GetInstance().ShowTip(ItemDetailData);
        }
        private void _onBuyButtonClick()
        {
            if (!mSetButtonCD.BtIsWork)
            {
                return;
            }
            mSetButtonCD.BtIsWork = false;
            CostItemManager.CostInfo costInfo = new CostItemManager.CostInfo();

            
            costInfo.nMoneyID = ItemDataManager.GetInstance().GetMoneyIDByType(ItemTable.eSubType.POINT);
            
            if(BuyIndex==0)
            {
                int result = -1;
                int.TryParse(mPrice7.text, out result);
                costInfo.nCount = result;
            }
            else if (BuyIndex == 1)
            {
                int result = -1;
                int.TryParse(mPrice30.text, out result);
                costInfo.nCount = result;
            }
            if (BuyIndex == 2)
            {
                int result = -1;
                int.TryParse(mPriceForever.text, out result);
                costInfo.nCount = result;
            }
            CostItemManager.GetInstance().TryCostMoneyDefault(costInfo, () =>
            {
                WorldMallBuy req = new WorldMallBuy();

                req.itemId = FashionItem[BuyIndex].id;
                req.num = 1;

                NetManager netMgr = NetManager.Instance();
                netMgr.SendCommand(ServerType.GATE_SERVER, req);

                
            });
            ClientSystemManager.GetInstance().CloseFrame<FashionBuyFrame>();
            /* put your code in here */
            //WorldMallBuy req = new WorldMallBuy();

            //req.itemId = FashionItem[BuyIndex].id;
            //req.num = 1;

            //NetManager netMgr = NetManager.Instance();
            //netMgr.SendCommand(ServerType.GATE_SERVER, req);
        }
        #endregion
    }
}