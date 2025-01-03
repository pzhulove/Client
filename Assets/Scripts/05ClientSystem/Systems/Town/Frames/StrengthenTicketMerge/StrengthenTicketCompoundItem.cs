using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class StrengthenTicketCompoundItem : MonoBehaviour
    {
        [SerializeField] private ComItemNew mItem;
        [SerializeField] private Text mTextName;
        [SerializeField] private Image mImgSelect;
        [SerializeField] private Image mImgUnSelect;

        public void OnInit(StrengthenTicketMaterialMergeModel itemModel, bool isSelect)
        {
            ItemData itemData = ItemDataManager.CreateItemDataFromTable(itemModel.displayItemTableId);
            mItem.Setup(itemData, null, true);
            mTextName.SafeSetText(itemModel.name, itemData.TableData.Color);
            mImgSelect.CustomActiveAlpha(isSelect);
            mImgUnSelect.CustomActiveAlpha(!isSelect);
        }
    }
}
