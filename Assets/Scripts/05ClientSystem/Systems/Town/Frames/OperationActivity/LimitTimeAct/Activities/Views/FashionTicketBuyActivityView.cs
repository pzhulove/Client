using Network;
using Protocol;
using ProtoTable;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class FashionTicketBuyActivityView : LimitTimeActivityViewCommon
    {
        //item的根节点
        [SerializeField]
        private GameObject mBuyItemRoot = null;

        [SerializeField]
        private string mItemPath = null;

        public delegate void BuyCallBack(uint mallItemId);
        private BuyCallBack mBuyCallBack;
        private List<ComItem> mComItems = new List<ComItem>();

        public  void SetBuyCallBack(BuyCallBack callback)
        {
            mBuyCallBack = callback;
        }
        public override void Init(ILimitTimeActivityModel model, ActivityItemBase.OnActivityItemClick<int> onItemClick)
        {
            if (model == null)
            {
                Logger.LogError("LimitTimeActivityModel data is empty");
                return;
            }

            mOnItemClick = onItemClick;
            //_InitItems(model);
            mNote.Init(model, true, GetComponent<ComCommonBind>());

            for (int i = 0; i < model.ParamArray.Length; i++)
            {
                int mallItemTableID = (int)model.ParamArray[i];
                var mallItemTableData = TableManager.GetInstance().GetTableItem<MallItemTable>(mallItemTableID);
                ItemTable.eSubType moneyType = (ItemTable.eSubType)mallItemTableData.moneytype;
                if (mallItemTableData != null && mItemPath != null)
                {
                    GameObject ticketItemGO = AssetLoader.instance.LoadResAsGameObject(mItemPath);
                    var mBind = ticketItemGO.GetComponent<ComCommonBind>();
                    if (mBind != null)
                    {
                        var m_ItemRoot = mBind.GetGameObject("ItemRoot");
                        var m_PriceImage = mBind.GetCom<Image>("PriceImage");
                        var m_Price = mBind.GetCom<Text>("Price");
                        var m_Buy = mBind.GetCom<Button>("Buy");
                        var m_Name = mBind.GetCom<Text>("Name");

                        //icon
                        string tempFirstItemData = mallItemTableData.giftpackitems.Split('|')[0];
                        string[] tempItemData = tempFirstItemData.Split(':');
                        if (tempItemData.Length != 2)
                        {
                            continue;
                        }
                        int itemID = -1;
                        int itemCount = -1;
                        int.TryParse(tempItemData[0], out itemID);
                        int.TryParse(tempItemData[1], out itemCount);
                        var itemTableItem = TableManager.GetInstance().GetTableItem<ItemTable>(itemID);
                        if (itemTableItem == null)
                        {
                            return;
                        }
                        ComItem comitem = m_ItemRoot.GetComponentInChildren<ComItem>();
                        if (comitem == null)
                        {
                            var comItem = ComItemManager.Create(m_ItemRoot);//可以这样写吗需要确认
                            comitem = comItem;
                            mComItems.Add(comitem);
                        }
                        ItemData ItemDetailData = ItemDataManager.CreateItemDataFromTable(itemID);
                        if (null == ItemDetailData)
                        {
                            return;
                        }
                        ItemDetailData.Count = itemCount;
                        comitem.Setup(ItemDetailData, (GameObject Obj, ItemData sitem) => { _OnShowTips(ItemDetailData); });

                        //Btn
                        m_Buy.onClick.RemoveAllListeners();
                        m_Buy.onClick.AddListener(() =>
                        {
                            if (mBuyCallBack != null)
                            {
                                mBuyCallBack((uint)mallItemTableID);
                            }
                        });

                        //name
                        m_Name.text = mallItemTableData.giftpackname;

                        //price
                        m_Price.text = mallItemTableData.price.ToString();
                        int mMoneyID = -1;
                        if (moneyType == ItemTable.eSubType.BindPOINT)
                        {
                            mMoneyID = ItemDataManager.GetInstance().GetMoneyIDByType(ItemTable.eSubType.BindPOINT);
                        }
                        else if (moneyType == ItemTable.eSubType.POINT)
                        {
                            mMoneyID = ItemDataManager.GetInstance().GetMoneyIDByType(ItemTable.eSubType.POINT);
                        }
                        var itemTableData = TableManager.GetInstance().GetTableItem<ItemTable>(mMoneyID);
                        if (itemTableData != null)
                        {
                            ETCImageLoader.LoadSprite(ref m_PriceImage, itemTableData.Icon);
                        }
                    }
                    Utility.AttachTo(ticketItemGO, mBuyItemRoot);
                }
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            if (mComItems != null)
            {
                for (int i = this.mComItems.Count - 1; i >= 0; --i)
                {
                    ComItemManager.Destroy(mComItems[i]);
                }
                mComItems.Clear();
            }
        }

        void _OnShowTips(ItemData result)
        {
            if (result == null)
            {
                return;
            }
            ItemTipManager.GetInstance().ShowTip(result);
        }
    }
}
