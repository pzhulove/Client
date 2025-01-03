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

    public class AuctionNewOtherPlayerSellRecordItem : MonoBehaviour
    {
        private AuctionTransaction _auctionTransaction = null;

        private ComItem _comItem;

        [Space(10)]
        [HeaderAttribute("ItemContent")]
        [Space(5)]
        [SerializeField] private GameObject itemPos;
        [SerializeField] private Text itemName;
        [SerializeField] private Text itemPriceValueLabel;

        private void OnDestroy()
        {
            if (_comItem != null)
            {
                ComItemManager.Destroy(_comItem);
                _comItem = null;
            }

            _auctionTransaction = null;
        }

        public void InitItem(AuctionTransaction auctionTransaction)
        {
            _auctionTransaction = auctionTransaction;

            if (_auctionTransaction == null)
                return;

            InitItemView();
        }

        private void InitItemView()
        {
            var itemData = ItemDataManager.CreateItemDataFromTable((int)_auctionTransaction.item_id);
            var itemTable = TableManager.GetInstance().GetTableItem<ItemTable>((int)_auctionTransaction.item_id);
            if (itemData == null || itemTable == null)
            {
                Logger.LogErrorFormat("TreasureItemData is null and itemId is {0}", _auctionTransaction.item_id);
                return;
            }

            itemData.Count = (int)_auctionTransaction.item_num;
            itemData.StrengthenLevel = (int) _auctionTransaction.strength;
            itemData.SubQuality = (int)_auctionTransaction.qualitylv;
            itemData.BeadAdditiveAttributeBuffID = (int) _auctionTransaction.beadbuffId;

            //设置装备的类型
            AuctionNewUtility.UpdateItemDataByEquipType(itemData, _auctionTransaction.equipType,
                _auctionTransaction.enhance_type,
                (int) _auctionTransaction.enhanceNum);

            //镶嵌附魔卡
            if (itemData.SubType != (int)ItemTable.eSubType.EnchantmentsCard)
            {
                itemData.mPrecEnchantmentCard = ItemDataManager.GetInstance().MountMagicCardInItem(
                    _auctionTransaction.mountMagicCardId,
                    _auctionTransaction.mountMagicCardLv);
            }

            //镶嵌宝珠
            if (itemData.SubType != (int) ItemTable.eSubType.Bead)
            {
                itemData.PreciousBeadMountHole = ItemDataManager.GetInstance().MountBeadInItem(
                    _auctionTransaction.mountBeadId,
                    _auctionTransaction.mountBeadBuffId);
            }

            if (itemName != null)
                itemName.text = itemData.GetColorName();


            if (itemPos != null)
            {
                _comItem = itemPos.GetComponentInChildren<ComItem>();
                if (_comItem == null)
                    _comItem = ComItemManager.Create(itemPos);

                if (_comItem != null)
                {
                    _comItem.Setup(itemData, OnShowItemTip);
                    //珍品类型
                    _comItem.SetShowTreasure(true);
                }
            }
            
            if (itemPriceValueLabel != null)
            {
                var singlePrice = (ulong)_auctionTransaction.unit_price;

                itemPriceValueLabel.text = Utility.ToThousandsSeparator(singlePrice);
            }
            
        }
        
        public void OnItemRecycle()
        {
            _auctionTransaction = null;
        }

        private void OnShowItemTip(GameObject obj, ItemData itemData)
        {
            ItemTipManager.GetInstance().ShowTip(itemData);
        }


    }
}