using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class StrengthenTicketCompoundNeedItem : MonoBehaviour
    {
        [SerializeField] private ComItemNew mItem;
        [SerializeField] private Text mTextCount;
        [SerializeField] private int mFastBuyItemId = 330000213;

        public void OnInit(ItemSimpleData itemSData)
        {
            int ownerCount = ItemDataManager.GetInstance().GetOwnedItemCount(itemSData.ItemID, false);
            var itemData = ItemDataManager.CreateItemDataFromTable(itemSData.ItemID);
            mItem.Setup(itemData, (obj, item)=>{
                if (mFastBuyItemId == item.TableID)
                    StrengthenTicketMergeDataManager.GetInstance().ReqFastMallBuy(mFastBuyItemId);
                else 
                    ItemComeLink.OnLink(itemSData.ItemID, 0, false, null, false, true);
            });
            mTextCount.SafeSetText(_FormatCustomComItemCount(ownerCount, itemSData.Count));
        }

        private string _FormatCustomComItemCount(int ownCount, int needCount)
        {
            string ownCountStr = ownCount.ToString();
            string needCountStr = needCount.ToString();
            if (ownCount < needCount)
            {
                ownCountStr = string.Format(TR.Value("strengthen_merge_desc_less_color_format"), ownCountStr);
            }
            else
            {
                ownCountStr = string.Format(TR.Value("strengthen_merge_desc_normal_color_format"), ownCountStr);
            }
            needCountStr = string.Format(TR.Value("strengthen_merge_desc_normal_color_format"), needCount.ToString());
            return string.Format(TR.Value("strengthen_merge_material_count_format"), ownCountStr, needCountStr);
        }
    }
}
