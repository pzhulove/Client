using System;
using System.Collections.Generic;
using Protocol;
using ProtoTable;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class GoblinShopItem : MonoBehaviour
    {
        [Space(5)]
        [HeaderAttribute("NormalContent")]
        [SerializeField]
        private GameObject itemRoot;
        [SerializeField]
        private Text itemName;

        [SerializeField]
        private Text itemLimitTimes;
        [SerializeField]
        private Button buyButton;
        [SerializeField]
        private UIGray buyButtonGray;
        [SerializeField]
        private Button previewButton;

        [Space(10)]
        [HeaderAttribute("PriceInfo")]
        [SerializeField]
        private GameObject priceRoot;

        [SerializeField]
        private Text priceName;
        [SerializeField]
        private Image priceIcon;
        [SerializeField]
        private Text priceValue;
        [SerializeField]
        private Button priceBtn;
        [SerializeField]
        private ComOldChangeNewItem comOldChangeNewItem;

        [Space(10)]
        [HeaderAttribute("OtherInfo")]
        [SerializeField]
        private Text soldOverText;

        [SerializeField]
        private GameObject vipRoot;
        [SerializeField]
        private Text vipText;

        [SerializeField]
        private GameObject itemLockLimitRoot;
        [SerializeField]
        private Button itemLockLimitButton;
        [SerializeField]
        private Text itemLockLimitText;

        [SerializeField]
        private Text vipDiscountText;
        [SerializeField]
        private GameObject canNotBuyMask;
        
        public void SetElementItem(AccountShopItemInfo accountShopItem, AccountShopQueryIndex accountShopQueryIndex)
        {
            if(itemName != null)
            {
                itemName.text = accountShopItem.shopItemName;
            }

            if(itemLimitTimes != null)
            {
                itemLimitTimes.text = string.Format("{0}/{1}", accountShopItem.accountRestBuyNum, accountShopItem.accountLimitBuyNum);
            }
            

            if(itemRoot != null)
            {
                ComItem comitem = itemRoot.GetComponentInChildren<ComItem>();
                if (comitem == null)
                {
                    var comItem = ComItemManager.Create(itemRoot);
                    comitem = comItem;
                }
                ItemData ItemDetailData = ItemDataManager.CreateItemDataFromTable((int)accountShopItem.itemDataId);
                if(ItemDetailData != null)
                {
                    ItemDetailData.Count = (int)accountShopItem.itemNum;
                    comitem.Setup(ItemDetailData, (GameObject Obj, ItemData sitem) => { _OnShowTips(ItemDetailData); });
                }
                
            }

            var itemTableData = TableManager.GetInstance().GetTableItem<ItemTable>((int)accountShopItem.costItems[0].id);
            if(itemTableData != null)
            {
                if(priceIcon != null)
                {
                    ETCImageLoader.LoadSprite(ref priceIcon, itemTableData.Icon);
                }
                if(priceValue != null)
                {
                    priceValue.text = accountShopItem.costItems[0].num.ToString();
                }
                priceBtn.onClick.RemoveAllListeners();
                ItemData ItemDetailData = ItemDataManager.CreateItemDataFromTable((int)accountShopItem.costItems[0].id);
                priceBtn.onClick.AddListener(() => { _OnShowTips(ItemDetailData); });
            }

            if (previewButton != null)
            {
                previewButton.onClick.RemoveAllListeners();
                int tempItemID = (int)accountShopItem.itemDataId;
                previewButton.onClick.AddListener(() =>
                {
                    //显示时装
                    if(accountShopItem.needPreviewFunc == 1)
                    {
                        ClientSystemManager.GetInstance().OpenFrame<PlayerTryOnFrame>(FrameLayer.Middle, tempItemID);
                    }
                    //显示宠物
                    else if(accountShopItem.needPreviewFunc == 2)
                    {
                        var tempItemTableData = TableManager.GetInstance().GetTableItem<ItemTable>(tempItemID);
                        if (tempItemTableData == null)
                        {
                            return;
                        }
                        if (tempItemTableData.SubType == ItemTable.eSubType.GiftPackage)
                        {
                            var giftPackTableData = TableManager.GetInstance().GetTableItem<GiftPackTable>(itemTableData.PackageID);
                            if (giftPackTableData == null)
                            {
                                return;
                            }
                            var giftList = TableManager.GetInstance().GetGiftTableData(giftPackTableData.ID);
                            if (giftList == null)
                                return;

                            for (int i = 0; i < giftList.Count; i++)
                            {
                                var giftTableData = giftList[i];
                                if (giftTableData == null)
                                {
                                    continue;
                                }
                                if (!giftTableData.RecommendJobs.Contains(PlayerBaseData.GetInstance().JobTableID))
                                {
                                    continue;
                                }
                                ItemTable tableData = TableManager.GetInstance().GetTableItem<ItemTable>(giftTableData.ItemID);
                                if (tableData == null)
                                {
                                    continue;
                                }
                                var petTableData = TableManager.GetInstance().GetTable<PetTable>();
                                var enumerator = petTableData.GetEnumerator();
                                while(enumerator.MoveNext())
                                {
                                    var petTableItem = enumerator.Current.Value as PetTable;
                                    if(petTableItem != null && petTableItem.PetEggID == tempItemID)
                                    {
                                        ClientSystemManager.GetInstance().OpenFrame<ShowPetModelFrame>(FrameLayer.Middle, petTableItem.ID);
                                        break;
                                    }
                                }
                                break;
                            }
                        }
                    }
                });
            }
            if (accountShopItem.accountRestBuyNum <= 0)
            {
                buyButtonGray.enabled = true;
            }
            else
            {
                buyButtonGray.enabled = false;
            }

            if (buyButton != null)
            {
                buyButton.onClick.RemoveAllListeners();
                buyButton.onClick.AddListener(() =>
                {
                    if (accountShopItem.accountRestBuyNum <= 0)
                    {
                        SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("goblin_shop_cannot_buy_tips_one"));
                    }
                    else if(AccountShopDataManager.GetInstance().GetSpecialItemNum((int)accountShopItem.costItems[0].id) < accountShopItem.costItems[0].num)
                    {
                        SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("goblin_shop_cannot_buy_tips_two"));
                    }
                    else
                    {
                        AccountShopDataManager.GetInstance().SendWorldAccountShopItemBuyReq(accountShopQueryIndex, accountShopItem.shopItemId, 1);
                    }
                });
            }

            if(accountShopItem.needPreviewFunc > 0)
            {
                previewButton.CustomActive(true);
            }
            else
            {
                previewButton.CustomActive(false);
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
