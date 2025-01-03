using UnityEngine;
using System.Collections.Generic;
using Protocol;

namespace GameClient
{
    class SmithFunctionRedBinder : MonoBehaviour
    {
        public enum SmithFunctionType
        {
            SFT_STRENGTH,
            SFT_ADDMAGIC,
            SFT_PEARLINLAY,
            SFT_ADJUST,
            SFT_STRENGTH_SPECIAL,
        }
        public List<SmithFunctionType> m_akFunctionTypes = new List<SmithFunctionType>();
        public GameObject m_goTarget = null;

        ItemData specialItem = null;
        public ItemData SpecialItem
        {
            get
            {
                return specialItem;
            }
            set
            {
                specialItem = value;
            }
        }
        
        // Use this for initialization
        void Start()
        {
            ItemDataManager.GetInstance().onAddNewItem += _OnAddNewItem;
            ItemDataManager.GetInstance().onUpdateItem += _OnUpdateItem;
            ItemDataManager.GetInstance().onRemoveItem += _OnRemoveItem;
            PlayerBaseData.GetInstance().onMoneyChanged += _OnMoneyChanged;
            _Update();
        }

        public void SetCheckFunction(SmithFunctionType eSmithFunctionType)
        {
            m_akFunctionTypes.Clear();
            m_akFunctionTypes.Add(eSmithFunctionType);
            _Update();
        }

        public void AddCheckFunction(SmithFunctionType eSmithFunctionType)
        {
            if(!m_akFunctionTypes.Contains(eSmithFunctionType))
            {
                m_akFunctionTypes.Add(eSmithFunctionType);
            }
            _Update();
        }

        public void ClearCheckFunctions()
        {
            m_akFunctionTypes.Clear();
            _Update();
        }

        bool _CheckStrength()
        {
            if(SpecialItem != null)
            {
                return _CheckItemDataNeedStrengthHint(SpecialItem) && _CheckItemDataCanStrength(SpecialItem);
            }

            var uids = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.WearEquip);
            for (int i = 0; i < uids.Count; ++i)
            {
                var itemData = ItemDataManager.GetInstance().GetItem(uids[i]);
                if(_CheckItemDataNeedStrengthHint(itemData) && _CheckItemDataCanStrength(itemData))
                {
                    return true;
                }
            }
            return false;
        }
        bool _CheckStrengthSpecial()
        {
            int iLowLevel = 9527;

            if(SpecialItem == null)
            {
                var uids = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.WearEquip);
                for (int i = 0; i < uids.Count; ++i)
                {
                    var itemData = ItemDataManager.GetInstance().GetItem(uids[i]);
                    if (itemData != null && itemData.Type != ProtoTable.ItemTable.eType.FUCKTITTLE)
                    {
                        iLowLevel = IntMath.Min(iLowLevel, itemData.StrengthenLevel);
                    }
                }
            }
            else
            {
                if (SpecialItem != null && SpecialItem.Type != ProtoTable.ItemTable.eType.FUCKTITTLE)
                {
                    iLowLevel = IntMath.Min(iLowLevel, SpecialItem.StrengthenLevel);
                }
            }

            var datas = ItemDataManager.GetInstance().GetItemsByPackageSubType(EPackageType.Material, ProtoTable.ItemTable.eSubType.Coupon);
            if (datas != null)
            {
                for (int i = 0; i < datas.Count; ++i)
                {
                    var itemData = ItemDataManager.GetInstance().GetItem(datas[i]);
                    if (itemData == null)
                    {
                        continue;
                    }

                    var ticketItem = TableManager.GetInstance().GetTableItem<ProtoTable.StrengthenTicketTable>((int)itemData.TableID);
                    if (ticketItem != null)
                    {
                        if(ticketItem.Level > iLowLevel)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
        bool _CheckItemDataCanStrength(ItemData itemData)
        {
            if (itemData != null && itemData.Type != ProtoTable.ItemTable.eType.FUCKTITTLE)
            {
                StrengthenCost costNeed = new StrengthenCost();
                if (StrengthenDataManager.GetInstance().GetCost(itemData.StrengthenLevel, itemData.LevelLimit, itemData.Quality, ref costNeed))
                {
                    int m_id0 = (int)ItemData.IncomeType.IT_UNCOLOR;
                    int m_id1 = (int)ItemData.IncomeType.IT_COLOR;
                    int m_id3 = ItemDataManager.GetInstance().GetMoneyIDByType(ProtoTable.ItemTable.eSubType.BindGOLD);

                    if (itemData.SubType == (int)ProtoTable.ItemTable.eSubType.WEAPON)
                    {
                        float fRadio = 1.0f;
                        var SystemValueTableData = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_STRENGTH_WP_COST_MOD);
                        if (SystemValueTableData != null)
                        {
                            fRadio = SystemValueTableData.Value / 10.0f;
                        }

                        costNeed.ColorCost = Utility.ceil(costNeed.ColorCost * fRadio);
                        costNeed.UnColorCost = Utility.ceil(costNeed.UnColorCost * fRadio);
                        costNeed.GoldCost = Utility.ceil(costNeed.GoldCost * fRadio);
                    }
                    else if (itemData.SubType >= (int)ProtoTable.ItemTable.eSubType.HEAD && itemData.SubType <= (int)ProtoTable.ItemTable.eSubType.BOOT)
                    {
                        float fRadio = 1.0f;
                        var SystemValueTableData = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_STRENGTH_DF_COST_MOD);
                        if (SystemValueTableData != null)
                        {
                            fRadio = SystemValueTableData.Value / 10.0f;
                        }

                        costNeed.ColorCost = Utility.ceil(costNeed.ColorCost * fRadio);
                        costNeed.UnColorCost = Utility.ceil(costNeed.UnColorCost * fRadio);
                        costNeed.GoldCost = Utility.ceil(costNeed.GoldCost * fRadio);
                    }
                    else if (itemData.SubType >= (int)ProtoTable.ItemTable.eSubType.RING && itemData.SubType <= (int)ProtoTable.ItemTable.eSubType.BRACELET)
                    {
                        float fRadio = 1.0f;
                        var SystemValueTableData = TableManager.GetInstance().GetTableItem<ProtoTable.SystemValueTable>((int)ProtoTable.SystemValueTable.eType.SVT_STRENGTH_JW_COST_MOD);
                        if (SystemValueTableData != null)
                        {
                            fRadio = SystemValueTableData.Value / 10.0f;
                        }

                        costNeed.ColorCost = Utility.ceil(costNeed.ColorCost * fRadio);
                        costNeed.UnColorCost = Utility.ceil(costNeed.UnColorCost * fRadio);
                        costNeed.GoldCost = Utility.ceil(costNeed.GoldCost * fRadio);
                    }

                    if (costNeed.UnColorCost > 0)
                    {
                        int iHasUnColor = ItemDataManager.GetInstance().GetOwnedItemCount(m_id0);
                        if (iHasUnColor < costNeed.UnColorCost)
                        {
                            return false;
                        }
                    }
                    if (costNeed.ColorCost > 0)
                    {
                        int iHasColor = ItemDataManager.GetInstance().GetOwnedItemCount(m_id1);
                        if (iHasColor < costNeed.ColorCost)
                        {
                            return false;
                        }
                    }
                    if (costNeed.GoldCost > 0)
                    {
                        int iHasGold = ItemDataManager.GetInstance().GetOwnedItemCount(m_id3);
                        if (iHasGold < costNeed.GoldCost)
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }
            return false;
        }
        bool _CheckItemDataNeedStrengthHint(ItemData itemData)
        {
            if(itemData == null || itemData.StrengthenLevel >= 10)
            {
                return false;
            }
            return true;
        }

        bool _CheckAddMagic()
        {
            var itemids = ItemDataManager.GetInstance().GetItemsByType(ProtoTable.ItemTable.eType.EXPENDABLE);
            if(itemids.Count <= 0)
            {
                return false;
            }

            var uids = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.WearEquip);
            if(uids.Count <= 0)
            {
                return false;
            }

            List<ItemData> cards = new List<ItemData>();
            for (int i = 0; i < itemids.Count; ++i)
            {
                var itemData = _TryAddMagicCard(itemids[i]);
                if (itemData != null)
                {
                    cards.Add(itemData);
                }
            }

            if (specialItem == null)
            {
                for (int i = 0; i < uids.Count; ++i)
                {
                    var itemData = ItemDataManager.GetInstance().GetItem(uids[i]);
                    if (itemData != null && itemData.Type != ProtoTable.ItemTable.eType.FUCKTITTLE)
                    {
                        if (itemData.mPrecEnchantmentCard == null)
                        {
                            continue;
                        }

                        if (itemData.mPrecEnchantmentCard.iEnchantmentCardID == 0)
                        {
                            for (int j = 0; j < cards.Count; ++j)
                            {
                                if (cards[j] == null) continue;
                                var magicItem = TableManager.GetInstance().GetTableItem<ProtoTable.MagicCardTable>(cards[j].TableID);
                                if (magicItem != null && magicItem.Parts.Contains((int)itemData.EquipWearSlotType))
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (specialItem != null && specialItem.Type != ProtoTable.ItemTable.eType.FUCKTITTLE)
                {
                    if (specialItem.mPrecEnchantmentCard != null)
                    {
                        if (specialItem.mPrecEnchantmentCard.iEnchantmentCardID == 0)
                        {
                            for (int j = 0; j < cards.Count; ++j)
                            {
                                if (cards[j] == null) continue;
                                var magicItem = TableManager.GetInstance().GetTableItem<ProtoTable.MagicCardTable>(cards[j].TableID);
                                if (magicItem != null && magicItem.Parts.Contains((int)specialItem.EquipWearSlotType))
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }

        bool _CheckAdjustQuality()
        {
            int iCostSealID = int.Parse(TR.Value("ItemKeyZBPJTZX"));
            int iHasCount = ItemDataManager.GetInstance().GetOwnedItemCount((int)iCostSealID);
            if(iHasCount < 1)
            {
                return false;
            }

            var uids = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.WearEquip);
            if (uids.Count <= 0)
            {
                return false;
            }

            if(SpecialItem == null)
            {
                for (int i = 0; i < uids.Count; ++i)
                {
                    var itemData = ItemDataManager.GetInstance().GetItem(uids[i]);
                    if (itemData != null && itemData.Type != ProtoTable.ItemTable.eType.FUCKTITTLE)
                    {
                        if (itemData.SubQuality <= 60)
                        {
                            return true;
                        }
                    }
                }
            }
            else
            {
                if (SpecialItem != null && SpecialItem.Type != ProtoTable.ItemTable.eType.FUCKTITTLE)
                {
                    if (SpecialItem.SubQuality <= 60)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        ItemData _TryAddMagicCard(ulong guid)
        {
            ItemData itemData = ItemDataManager.GetInstance().GetItem(guid);
            if (itemData != null && itemData.Type == ProtoTable.ItemTable.eType.EXPENDABLE &&
                itemData.SubType == (int)ProtoTable.ItemTable.eSubType.EnchantmentsCard)
            {
                return itemData;
            }
            return null;
        }

        bool _CheckOk(SmithFunctionType eSmithFunctionType)
        {
            if (eSmithFunctionType == SmithFunctionType.SFT_STRENGTH)
            {
                return _CheckStrengthSpecial() || _CheckStrength();
            }
            else if(eSmithFunctionType == SmithFunctionType.SFT_ADDMAGIC)
            {
                return _CheckAddMagic();
            }
            else if(eSmithFunctionType == SmithFunctionType.SFT_ADJUST)
            {
                return _CheckAdjustQuality();
            }
            else if(eSmithFunctionType == SmithFunctionType.SFT_STRENGTH_SPECIAL)
            {
                return _CheckStrengthSpecial();
            }

            return false;
        }

        void _Update()
        {
            if(m_goTarget == null)
            {
                return;
            }

            bool bResultOk = false;
            for(int i = 0;!bResultOk && i < m_akFunctionTypes.Count; ++i)
            {
                bResultOk = _CheckOk(m_akFunctionTypes[i]);
            }

            bResultOk = false;

            m_goTarget.CustomActive(bResultOk);
        }

        void _OnAddNewItem(List<Item> items)
        {
            _Update();
        }

        void _OnRemoveItem(ItemData data)
        {
            _Update();
        }

        void _OnUpdateItem(List<Item> items)
        {
            _Update();
        }

        void _OnMoneyChanged(PlayerBaseData.MoneyBinderType eMoneyBinderType)
        {
            _Update();
        }

        void OnEnable()
        {
            _Update();
        }

        void OnDestroy()
        {
            ItemDataManager.GetInstance().onAddNewItem -= _OnAddNewItem;
            ItemDataManager.GetInstance().onUpdateItem -= _OnUpdateItem;
            ItemDataManager.GetInstance().onRemoveItem -= _OnRemoveItem;
            PlayerBaseData.GetInstance().onMoneyChanged -= _OnMoneyChanged;
        }
    }
}