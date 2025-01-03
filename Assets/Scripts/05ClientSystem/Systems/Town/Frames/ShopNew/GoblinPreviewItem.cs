using System.Collections;
using Protocol;
using ProtoTable;
using Scripts.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class GoblinPreviewItem : MonoBehaviour
    {
        [SerializeField]
        private Text mName;
        [SerializeField]
        private Text mScoreNum;
        [SerializeField]
        private GameObject itemRoot;

        public void InitUI(MallItemInfo mallItemInfo)
        {
            mName.text = string.Format("购买{0}", mallItemInfo.giftName);
            if (mallItemInfo.buyGotInfos != null && mallItemInfo.buyGotInfos.Length != 0)
            {
                mScoreNum.text = mallItemInfo.buyGotInfos[0].buyGotNum.ToString();
            }
            ComItem comitem = itemRoot.GetComponentInChildren<ComItem>();
            if (comitem == null)
            {
                var comItem = ComItemManager.Create(itemRoot);
                comitem = comItem;
            }
            if (mallItemInfo.giftItems != null && mallItemInfo.giftItems.Length != 0)
            {
                ItemData ItemDetailData = ItemDataManager.CreateItemDataFromTable((int)mallItemInfo.giftItems[0].id);
                if (ItemDetailData != null)
                {
                    ItemDetailData.Count = 1;
                    comitem.Setup(ItemDetailData, (GameObject Obj, ItemData sitem) => { _OnShowTips(ItemDetailData); });
                }
            }
                

        }

        void _OnShowTips(ItemData result)
        {
            if (result == null)
            {
                return;
            }
            ItemTipManager.GetInstance().ShowTip(result);
        }
    }

}