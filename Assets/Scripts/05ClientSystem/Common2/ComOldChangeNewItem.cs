using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProtoTable;

namespace GameClient
{
    public class ComOldChangeNewItem : MonoBehaviour
    {
        public Image icon;
        public Image board;
        public Text count;
        public Button iconBtn;

        List<AwardItemData> oldChangeNewItem = new List<AwardItemData>();

        public void SetItemId(int id)
        {
            var shopItemTable = TableManager.GetInstance().GetTableItem<ProtoTable.ShopItemTable>(id);
            if (shopItemTable != null)
            {
                ShopDataManager.GetInstance()._GetOldChangeNewItem(shopItemTable, oldChangeNewItem);

                for (int i = 0; i < oldChangeNewItem.Count; i++)
                {
                    ItemData oldChangeNewItemData = ItemDataManager.GetInstance().GetCommonItemTableDataByID(oldChangeNewItem[i].ID);
                    oldChangeNewItemData.Count = oldChangeNewItem[i].Num;
                    iconBtn.onClick.RemoveAllListeners();
                    iconBtn.onClick.AddListener(
                        () => { ItemTipManager.GetInstance().ShowTip(oldChangeNewItemData);
                        } );

                    _setBoard(oldChangeNewItemData.Quality);
                    _setIcon(oldChangeNewItemData.Icon);
                    bool bIsCanOldChangeNew = (ShopDataManager.GetInstance()._IsCanOldChangeNew(oldChangeNewItemData, EPackageType.Equip) || ShopDataManager.GetInstance()._IsCanOldChangeNew(oldChangeNewItemData, EPackageType.WearEquip));
                    
                    if (bIsCanOldChangeNew)
                    {
                        count.text = TR.Value("oldchangeNewGreen");
                    }
                    else
                    {
                        count.text = TR.Value("oldchangeNewRed");
                    }
                    
                }
            }

          
        }
        
        private void _setBoard(ItemTable.eColor color)
        {
            if (null == board)
            {
                return;
            }

            if (color == ItemTable.eColor.GREEN)
            {
                Color mColor = new Color(10.0f / 255.0f, 100.0f / 255.0f, 10.0f / 255.0f, 255.0f / 255.0f);
                board.color = mColor;
            }
           
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
