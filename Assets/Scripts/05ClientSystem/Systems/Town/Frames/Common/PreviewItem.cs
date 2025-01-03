using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProtoTable;

namespace GameClient
{
    public class PreviewItem : MonoBehaviour
    {
        [SerializeField]private Image mBg;
        [SerializeField]private Image mIcon;
        [SerializeField]private Text mName;
        [SerializeField]private GameObject mSelectGo;

        private int iIndex = 0;
        private PreViewItemData mPreviewItemData = null;
        public int Index { get { return iIndex; } }
        
        private void OnDestroy()
        {
            iIndex = 0;
            mPreviewItemData = null;
        }

        public void OnItemVisiable(int index,PreViewItemData previewItemData)
        {
            iIndex = index;
            mPreviewItemData = previewItemData;
            
            ItemData itemData = ItemDataManager.CreateItemDataFromTable(mPreviewItemData.itemId);
            if (itemData != null)
            {
                ETCImageLoader.LoadSprite(ref mBg, itemData.GetQualityInfo().Background);
            }

            //如果是宠物蛋
            if (itemData.SubType == (int)ItemTable.eSubType.PetEgg)
            {
                int petID = Utility.GetPetID(itemData.TableID);
                var petTable = TableManager.GetInstance().GetTableItem<PetTable>(petID);
                if (petTable != null)
                {
                    ETCImageLoader.LoadSprite(ref mIcon, petTable.IconPath);
                }
            }
            else
            {
                ETCImageLoader.LoadSprite(ref mIcon, itemData.Icon);
            }

            if (mName != null)
            {
                mName.text = itemData.GetColorName();
            }
        }

        public void OnItemChangeDisplay(bool bSelected)
        {
            if (mSelectGo != null)
            {
                mSelectGo.gameObject.CustomActive(bSelected);
            }
        }
    }
}
