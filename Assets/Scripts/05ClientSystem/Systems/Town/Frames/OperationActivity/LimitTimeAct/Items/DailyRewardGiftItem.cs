using System.Collections;
using System.Collections.Generic;
//using BehaviorDesigner.Runtime.Tasks.Basic.UnityGameObject;
using ProtoTable;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class DailyRewardGiftItem : MonoBehaviour,IDailyRewardGiftItem
    {

        [SerializeField] private Text mTextDescription;
        [SerializeField] private Image mImageBg;
        [SerializeField] private RectTransform mItemRoot;
        [SerializeField] Vector2 mComItemSize = new Vector2(100, 100);
        private List<ComItem> mComItems = new List<ComItem>();

        public void Init(List<DailyRewardDetailData> giftList)
        {
            if (giftList.Count <= 0)
            {
                Logger.LogError("gift 数组数量为0");
                return;
            }

            Dispose();
            for (int i = 0; i < giftList.Count; ++i)
            {
                var comItem = ComItemManager.Create(this.mItemRoot.gameObject);
                if (comItem != null)
                {
                    ItemData data = ItemDataManager.CreateItemDataFromTable((int)giftList[i].mSimpleData.ItemID);
                    data.Count = giftList[i].mSimpleData.Count;
                    comItem.Setup(data, OnItemClicked);
                    (comItem.transform as RectTransform).sizeDelta = this.mComItemSize;
                    mComItems.Add(comItem);
                }
            }

            this.mTextDescription.SafeSetText(giftList[0].Desc);
        }

        public void Dispose()
        {
            for (int i = this.mComItems.Count - 1; i >= 0; --i)
            {
                ComItemManager.Destroy(mComItems[i]);
            }
            mComItems.Clear();
        }

        void OnDestroy()
        {
            Dispose();
        }

        void OnItemClicked(GameObject obj, ItemData item)
        {
            if (null != item)
            {
                ItemTipManager.GetInstance().ShowTip(item);
            }
        }

    }
}