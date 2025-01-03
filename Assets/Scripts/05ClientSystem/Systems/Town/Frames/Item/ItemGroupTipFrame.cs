using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using UnityEngine.Assertions;
using Protocol;

namespace GameClient
{
    class ItemGroupTipFrame : ClientFrame
    {
        [UIControl("Name")]
        Text m_labName;

        [UIControl("Items")]
        ComUIListScript m_comItemList;

        ItemData m_packItem = null;
        List<ItemData> m_arrItems = new List<ItemData>();

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Tip/ItemGroupTip";
        }

        protected override void _OnOpenFrame()
        {
            m_packItem = userData as ItemData;
            m_arrItems = new List<ItemData>();

            ProtoTable.GiftPackTable table = TableManager.GetInstance().GetTableItem<ProtoTable.GiftPackTable>(m_packItem.PackID);
            if (table != null)
            {
                var giftDataList = TableManager.GetInstance().GetGiftTableData(table.ID);
                for (int i = 0; i < giftDataList.Count; ++i)
                {
                    ProtoTable.GiftTable giftTable = giftDataList[i];
                    if (giftTable == null)
                        continue;
                    
                    ItemData item = ItemDataManager.CreateItemDataFromTable(giftTable.ItemID);
                    item.Count = giftTable.ItemCount;
                    m_arrItems.Add(item);
                }
            }


            m_comItemList.Initialize();

            m_comItemList.onBindItem = var =>
            {
                return CreateComItem(Utility.FindGameObject(var, "Item"));
            };

            m_comItemList.onItemVisiable = var =>
            {
                if (m_arrItems != null)
                {
                    if (var.m_index >= 0 && var.m_index < m_arrItems.Count)
                    {
                        ComItem comItem = var.gameObjectBindScript as ComItem;
                        comItem.Setup(m_arrItems[var.m_index], (var1, var2) =>
                        {
                            ItemTipManager.GetInstance().ShowTip(var2, null, TextAnchor.MiddleCenter, true);
                        });

                        Utility.GetComponetInChild<Text>(var.gameObject, "Name").text = m_arrItems[var.m_index].GetColorName();
                    }
                }
            };

            if (m_packItem != null)
            {
                m_labName.text = m_packItem.Name;
            }
            else
            {
                m_labName.text = string.Empty;
            }

            if (m_arrItems != null)
            {
                m_comItemList.SetElementAmount(m_arrItems.Count);
            }
            else
            {
                m_comItemList.SetElementAmount(0);
            }
        }

        protected override void _OnCloseFrame()
        {
            m_packItem = null;
            m_arrItems.Clear();
        }
    }
}
