using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using UnityEngine.Assertions;
using Protocol;
using ProtoTable;
using Network;

namespace GameClient
{
    // 购买通行证界面的item
    public class AdventurerPassCardBuyItem : MonoBehaviour
    {
        [SerializeField] Image lv0 = null;
        [SerializeField] Image lv1 = null;
        [SerializeField] Text fromLv = null;
        [SerializeField] Text toLv = null;
        [SerializeField] Text awardItemNumInfo = null;
        [SerializeField] ComUIListScript awardItems = null;
        [SerializeField] Text moneyNum = null;
        [SerializeField] Image moneyIcon = null;
        [SerializeField] Button btnBuy = null;
        [SerializeField] Image num0 = null;
        [SerializeField] Image num1 = null;
        [SerializeField] GameObject discountRoot = null;
        List<AdventurerPassCardDataManager.ItemInfo> rewardItems = null;

        // Use this for initialization
        void Start()
        {
            InitAwardItems();
        }

        private void OnDestroy()
        {
            
        }

        // Update is called once per frame
        void Update()
        {

        }        

        void InitAwardItems()
        {
            if (awardItems == null)
            {
                return;
            }

            awardItems.Initialize();
            awardItems.onBindItem = (go) =>
            {
                return go;
            };

            awardItems.onItemVisiable = (go) =>
            {
                if (go == null)
                {
                    return;
                }

                ComCommonBind bind = go.GetComponent<ComCommonBind>();
                if (bind == null)
                {
                    return;
                }

                ComItem comItem = bind.GetCom<ComItem>("item");
                if (comItem == null)
                {
                    return;
                }

                if (rewardItems == null)
                {
                    return;
                }

                if (go.m_index >= rewardItems.Count)
                {
                    return;
                }

                ItemData itemData = ItemDataManager.CreateItemDataFromTable(rewardItems[go.m_index].itemID);
                if (itemData == null)
                {
                    return;
                }

                if (itemData.TableData != null && itemData.TableData.ExpireTime > 0)
                {
                    itemData.DeadTimestamp = itemData.TableData.ExpireTime;
                }

                itemData.Count = rewardItems[go.m_index].itemNum;
                comItem.Setup(itemData, (var1, var2) =>
                {
                    ItemTipManager.GetInstance().CloseAll();
                    ItemTipManager.GetInstance().ShowTip(var2);
                });
            };
        }

        void UpdateAwardItems(int buyLv)
        {
            if (awardItems == null)
            {
                return;
            }

            if (rewardItems == null)
            {
                return;
            }

            awardItems.SetElementAmount(rewardItems.Count);
        }

        List<AdventurerPassCardDataManager.ItemInfo> GetUnlockAwardItems(int buyLv)
        {
            List<AdventurerPassCardDataManager.ItemInfo> items = new List<AdventurerPassCardDataManager.ItemInfo>();
            if (items == null)
            {
                return null;
            }

            int maxLv = AdventurerPassCardDataManager.GetInstance().GetAdventurerPassCardMaxLv(AdventurerPassCardDataManager.GetInstance().SeasonID);
            int fromLv = (int)AdventurerPassCardDataManager.GetInstance().CardLv;
            int toLv = Math.Min(maxLv, fromLv + buyLv);

            bool bGotKingPassCard = AdventurerPassCardDataManager.GetInstance().GetPassCardType > AdventurerPassCardDataManager.PassCardType.Normal;

            Dictionary<int, AdventurerPassCardDataManager.AwardItemInfo> awardInfos = AdventurerPassCardDataManager.GetInstance().GetAdventurePassAwardsBySeasonID(AdventurerPassCardDataManager.GetInstance().SeasonID);
            if (awardInfos != null)
            {
                Action<List<AdventurerPassCardDataManager.ItemInfo>> GetAwards = (awards) =>
                {
                    if (awards == null)
                    {
                        return;
                    }

                    for (int i = 0; i < awards.Count; i++)
                    {
                        AdventurerPassCardDataManager.ItemInfo temp = awards[i];
                        if (temp == null)
                        {
                            continue;
                        }

                        AdventurerPassCardDataManager.ItemInfo itemInfo = new AdventurerPassCardDataManager.ItemInfo();
                        if (itemInfo == null)
                        {
                            continue;
                        }

                        itemInfo.itemID = temp.itemID;
                        itemInfo.itemNum = temp.itemNum;
                        itemInfo.highValue = temp.highValue;

                        items.Add(itemInfo);
                    }
                };

                for (int i = fromLv + 1; i <= toLv; i++)
                {
                    if (!awardInfos.ContainsKey(i))
                    {
                        continue;
                    }

                    GetAwards(awardInfos[i].normalAwards);
                    if (bGotKingPassCard)
                    {
                        GetAwards(awardInfos[i].highAwards);
                    }
                }
            }

            items.Sort((a, b) =>
            {
                return 0;
            });

            return items;
        }

        string GetMoneyIconPath(ProtoTable.ItemTable.eSubType eSubType)
        {
            switch (eSubType)
            {
                case ItemTable.eSubType.POINT:
                case ItemTable.eSubType.BindPOINT:
                case ItemTable.eSubType.GOLD:
                case ItemTable.eSubType.BindGOLD:
                    {
                        int iMoneyID = ItemDataManager.GetInstance().GetMoneyIDByType(eSubType);
                        ItemData itemData = ItemDataManager.GetInstance().GetItemByTableID(iMoneyID);
                        if (itemData == null)
                        {
                            return "";
                        }

                        return itemData.Icon;
                    }

                default:
                    return "";
            }
        }
        const string numberPathFormat = "UI/Image/Packed/p_UI_ZhanLing.png:UI_ZhanLing_DengJi_Number_";
        const string discountNumPathFormat = "UI/Image/Packed/p_UI_ZhanLing.png:UI_ZhanLing_ZheKou_Number_";

        public void SetUp(object data)
        {
            if (data == null)
            {
                return;
            }

            if (!(data is int))
            {
                return;
            }

            int nBuyLv = (int)data;
            AdventurePassBuyLevelTable adventurePassBuyLevelTable = TableManager.GetInstance().GetTableItem<AdventurePassBuyLevelTable>(nBuyLv);
            if (adventurePassBuyLevelTable == null)
            {
                return;
            }
            int tenDigit = nBuyLv / 10;
            int unitsDigit = nBuyLv % 10;     
            Utility.SetImageIcon(lv0.gameObject, string.Format("{0}{1}",numberPathFormat, tenDigit),true);
            Utility.SetImageIcon(lv1.gameObject, string.Format("{0}{1}", numberPathFormat, unitsDigit),true);
            lv0.CustomActive(tenDigit > 0);

            long nToLv = Math.Min(nBuyLv + AdventurerPassCardDataManager.GetInstance().CardLv, AdventurerPassCardDataManager.GetInstance().GetAdventurerPassCardMaxLv(AdventurerPassCardDataManager.GetInstance().SeasonID));
            fromLv.SafeSetText(TR.Value("adventurer_pass_card_from_lv", AdventurerPassCardDataManager.GetInstance().CardLv));
            toLv.SafeSetText(TR.Value("adventurer_pass_card_to_lv",nToLv));

            rewardItems = GetUnlockAwardItems(nBuyLv);
            // 道具奖励按照道具品质从高到低显示；品质一样则按照数量从多到少显示；数量一致则按照道具ID从小到大显示
            rewardItems.Sort((a, b) => 
            {
                if(a == null || b == null)
                {
                    return 0;
                }

                ItemTable itemTableA = TableManager.GetInstance().GetTableItem<ItemTable>(a.itemID);
                ItemTable itemTableB = TableManager.GetInstance().GetTableItem<ItemTable>(b.itemID);
                if(itemTableA == null || itemTableB == null)
                {
                    return 0;
                }

                if(itemTableA.Color != itemTableB.Color)
                {
                    return itemTableB.Color.CompareTo(itemTableA.Color);
                }

                if(a.itemNum != b.itemNum)
                {
                    return b.itemNum.CompareTo(a.itemNum);
                }

                return itemTableA.ID.CompareTo(itemTableB.ID);
            });

            awardItemNumInfo.SafeSetText(TR.Value("adventurer_pass_card_unlock_award", rewardItems != null ? rewardItems.Count : 0));
            InitAwardItems();
            UpdateAwardItems(nBuyLv);

            moneyIcon.SafeSetImage(GetMoneyIconPath(ItemTable.eSubType.POINT));
            if(moneyNum != null)
            {
                moneyNum.SafeSetText(adventurePassBuyLevelTable.PointCost.ToString());
                moneyNum.color = adventurePassBuyLevelTable.PointCost > ItemDataManager.GetInstance().GetOwnedItemCount(ItemDataManager.GetInstance().GetMoneyIDByType(ItemTable.eSubType.POINT)) ? Color.red : Color.green;
            }

            btnBuy.SafeSetOnClickListener(() => 
            {
                int moneyID = ItemDataManager.GetInstance().GetMoneyIDByType(ItemTable.eSubType.POINT);
                CostItemManager.GetInstance().TryCostMoneyDefault(new CostItemManager.CostInfo
                {
                    nMoneyID = moneyID,
                    nCount = adventurePassBuyLevelTable.PointCost,
                },
                () =>
                {
                    LoginToggleMsgBoxOKCancelFrame.TryShowMsgBox(LoginToggleMsgBoxOKCancelFrame.LoginToggleMsgType.AdventurerPassCardBuyLevel,
                   TR.Value("adventurer_pass_card_buy_lv_tip", adventurePassBuyLevelTable.PointCost, nToLv, rewardItems.Count),
                   () =>
                   {
                       AdventurerPassCardDataManager.GetInstance().SendWorldAventurePassBuyLvReq((uint)nBuyLv);
                   },
                   () =>
                   {

                   },
                   TR.Value("adventurer_pass_card_buy_lv_tip_ok"),
                   TR.Value("adventurer_pass_card_buy_lv_tip_cancel"));
                },
                "common_money_cost",
                null);
                return;
                
            });
                        
            btnBuy.SafeSetGray(
                AdventurerPassCardDataManager.GetInstance().CardLv >= AdventurerPassCardDataManager.GetInstance().GetAdventurerPassCardMaxLv(AdventurerPassCardDataManager.GetInstance().SeasonID)
             || nBuyLv + AdventurerPassCardDataManager.GetInstance().CardLv > AdventurerPassCardDataManager.GetInstance().GetAdventurerPassCardMaxLv(AdventurerPassCardDataManager.GetInstance().SeasonID));
            
            const int nLv1 = 1;           
            AdventurePassBuyLevelTable adventurePassBuyLevelTableLv1 = TableManager.GetInstance().GetTableItem<AdventurePassBuyLevelTable>(nLv1);
            if(adventurePassBuyLevelTableLv1 != null)
            {
                float fDiscount = ((float)adventurePassBuyLevelTableLv1.PointCost * nBuyLv) / ((float)adventurePassBuyLevelTable.PointCost);           
                string discount = Math.Round(fDiscount * 10.0f, 1).ToString();

                if(discount.Length == 3)
                {
                    num0.SafeSetImage(string.Format("{0}{1}", discountNumPathFormat, Utility.ToInt(discount[0].ToString())));
                    num1.SafeSetImage(string.Format("{0}{1}", discountNumPathFormat, Utility.ToInt(discount[2].ToString())));
                }

                discountRoot.CustomActive(fDiscount < 1.0f);
            }
        }
    }

}