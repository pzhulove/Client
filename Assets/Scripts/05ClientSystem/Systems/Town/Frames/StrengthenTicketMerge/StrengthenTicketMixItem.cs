using System;
using System.Collections;
using System.Collections.Generic;
using GameClient;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class StrengthenTicketMixItem : MonoBehaviour
    {
        [SerializeField] private ComItemNew mItem;
        [SerializeField] private Text mTextName;
        [SerializeField] private Text mTextCount;
        [SerializeField] private Image mImgSelect;
        private int mSelectCount = 0;
        private int mOwnerCount = 0;
        public void OnInit(StrengthenTicketFuseItemData data, int itemId1, int itemId2)
        {
            var itemData = ItemDataManager.CreateItemDataFromTable(data.ticketItemData.TableID);
            mItem.Setup(itemData);
            mTextName.SafeSetText(itemData.Name, itemData.TableData.Color);
            mSelectCount = 0;
            mOwnerCount = data.ticketItemData.Count;
            if (itemId1 == data.ticketItemData.TableID)
                ++mSelectCount;
            if (itemId2 == data.ticketItemData.TableID)
                ++mSelectCount;
            mImgSelect.CustomActiveAlpha(mSelectCount > 0);
            if (mSelectCount > 0)
            {
                mTextCount.SafeSetText(TR.Value("strengthen_merge_mix_item_use_count", mSelectCount, mOwnerCount));
            }
            else
            {
                if (mOwnerCount == 1)
                    mTextCount.SafeSetText("");
                else
                    mTextCount.SafeSetText(mOwnerCount.ToString());
            }
        }

        //数量富余就可以继续点
        public bool IsCountEnough()
        {
            return (mOwnerCount - mSelectCount > 0);
        }
    }
}
