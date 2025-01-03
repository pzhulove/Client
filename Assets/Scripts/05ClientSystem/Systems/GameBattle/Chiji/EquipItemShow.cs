using System;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using ProtoTable;
using Protocol;
using Network;
using System;
using System.Collections.Generic;

namespace GameClient
{
    public class EquipItemShow : MonoBehaviour
    {
        public GameObject parentObj = null;
        public EEquipWearSlotType equipSlot = EEquipWearSlotType.EquipInvalid;
        public string slotName = "";

        private ComItem comEquip = null;
        private ulong CurWearEquipGuid = 0;

        private void Start()
        {
            if (comEquip == null)
            {
                comEquip = ComItemManager.Create(parentObj);
                comEquip.SetupSlot(ComItem.ESlotType.Opened, slotName);
            }

            _InitWearEquip();

            ItemDataManager.GetInstance().onUpdateItem += _OnUpdateItem;
        }

        private void OnDestroy()
        {
            CurWearEquipGuid = 0;
            equipSlot = EEquipWearSlotType.EquipInvalid;

            ItemDataManager.GetInstance().onUpdateItem -= _OnUpdateItem;
        }

        private void _OnUpdateItem(List<Item> items)
        {
            if(items == null)
            {
                return;
            }

            for(int i = 0; i < items.Count; i++)
            {
                Item item = items[i];

                if (item == null)
                {
                    continue;
                }

                // 穿装备刷新装备栏
                _UpdateWearEquip(item);
            }
        }

        private void _InitWearEquip()
        {
            if (parentObj == null)
            {
                Logger.LogErrorFormat("parentObj is null in EquipItemShow, check preferb");
                return;
            }

            List<ulong> equipIDs = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.WearEquip);

            for (int i = 0; i < equipIDs.Count; ++i)
            {
                ItemData item = ItemDataManager.GetInstance().GetItem(equipIDs[i]);
                if (item == null)
                {
                    continue;
                }

                if (item.EquipWearSlotType != equipSlot)
                {
                    continue;
                }

                if (comEquip != null)
                {
                    comEquip.Setup(item, _OnWearedItemClicked);
                    CurWearEquipGuid = item.GUID;
                }

                break;
            }
        }

        private void _UpdateWearEquip(Item item)
        {
            if (parentObj == null || comEquip == null || item == null)
            {
                return;
            }

            ItemData itemdata = ItemDataManager.GetInstance().GetItem(item.uid);
            if (itemdata == null)
            {
                return;
            }

            if (itemdata.EquipWearSlotType != equipSlot)
            {
                return;
            }

            if ((EPackageType)item.pack == EPackageType.WearEquip)  // 穿上
            {
                comEquip.Setup(itemdata, _OnWearedItemClicked);
                CurWearEquipGuid = item.uid;
            }
            else if((EPackageType)item.pack == EPackageType.Equip) // 卸下
            {
                if(CurWearEquipGuid != item.uid)
                {
                    return;
                }

                comEquip.Setup(null, null);

                CurWearEquipGuid = 0;
            }
        }

        void _OnWearedItemClicked(GameObject obj, ItemData item)
        {
            if (item == null)
            {
                return;
            }

            List<TipFuncButon> funcs = new List<TipFuncButon>();
            TipFuncButon tempFunc = null;

            // 卸下
            if (item.Type == ItemTable.eType.EQUIP || item.Type == ItemTable.eType.FASHION || item.Type == ItemTable.eType.FUCKTITTLE)
            {
                tempFunc = new TipFuncButonSpecial();

                tempFunc.text = TR.Value("tip_takeoff");
                tempFunc.callback = _OnUnWear;

                funcs.Add(tempFunc);
            }

            ItemTipManager.GetInstance().ShowTip(item, funcs, TextAnchor.MiddleLeft);
        }

        void _OnUnWear(ItemData item, object data)
        {
            if (item != null)
            {
                ItemDataManager.GetInstance().UseItem(item);
                AudioManager.instance.PlaySound(103);
                ItemTipManager.GetInstance().CloseAll();
            }
        }

        private void Update()
        {
        }
    }
}
