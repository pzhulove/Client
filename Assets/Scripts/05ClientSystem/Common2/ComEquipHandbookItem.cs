using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProtoTable;

namespace GameClient
{
    public class ComEquipHandbookItem : MonoBehaviour
    {
        public Image icon;
        public Image board;
        public Text  name;
        public Image arrowState;
        public GameObject carry;
        public GameObject gostate;
        public ComCommonBind bind;
        public Text level;

        public void SetItemId(int id)
        {
            ItemTable table = TableManager.instance.GetTableItem<ItemTable>(id);

            if (null == table)
            {
                return ;
            }

            ItemData data = ItemDataManager.GetInstance().GetCommonItemTableDataByID(table.ID);

            _setName(data.GetColorName(),data.LevelLimit);
            _setBoard(ItemData.GetQualityInfo(table.Color).Background);
            _setIcon(table.Icon);
        }

        public void SetItemState(EquipHandbookEquipItemData data)
        {
            ItemTable table = TableManager.GetInstance().GetTableItem<ItemTable>(data.id);

            if (null == table)
            {
                return;
            }

            if (EquipHandbookDataManager.GetInstance().playerEquipData.Count <= 0)
            {
                carry.CustomActive(false);
            }

            for (int i = 0; i < EquipHandbookDataManager.GetInstance().playerEquipData.Count; i++)
            {
                ItemTable playTable = TableManager.GetInstance().GetTableItem<ItemTable>(EquipHandbookDataManager.GetInstance().playerEquipData[i].id);

                if (playTable == null)
                {
                    continue;
                }

                if (table.ID == playTable.ID)
                {
                    carry.CustomActive(true);
                    arrowState.CustomActive(false);
                    return;
                }

                if (table.SubType != playTable.SubType)
                {
                    carry.CustomActive(false);
                    bind.GetSprite("greenArrow", ref arrowState);
                    continue;
                }
                else
                {
                    carry.CustomActive(false);
                    arrowState.CustomActive(true);
                    if (data.baseScore >= EquipHandbookDataManager.GetInstance().playerEquipData[i].baseScore)
                    {
                        bind.GetSprite("greenArrow", ref arrowState);
                    }
                    else
                    {
                        bind.GetSprite("redArrow", ref arrowState);
                    }
                    break;
                }
            }
            
        }

        private void _setName(string itemName,int itemLevel)
        {
            if (null == name)
            {
                return;
            }

            name.text = itemName;
            level.text =string.Format("Lv.{0}", itemLevel);
        }

        private void _setBoard(string path)
        {
            if (null == board)
            {
                return;
            }

            ETCImageLoader.LoadSprite(ref board, path);
            board.type = Image.Type.Sliced;
        }

        private void _setIcon(string path)
        {
            if (null == icon)
            {
                return;
            }

            ETCImageLoader.LoadSprite(ref icon, path);
        }

    }
}
