using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public delegate void OnBxyEmptyClick(bool isBxyA);
    public class BxyItem : MonoBehaviour
    {
        [SerializeField] private GameObject mBxyRoot;
        [SerializeField] private GameObject mItemParent;
        [SerializeField] private Text mBxyName;
        [SerializeField] private Text mBxyAttr;

        [SerializeField] private bool IsBxyA;

        private OnBxyEmptyClick mOnBxyEmptyClick;
        private ComItemNew mComItem;
      
        private void OnDestroy()
        {
            mOnBxyEmptyClick = null;
            mComItem = null;
        }

        public void InitBxyItem(OnBxyEmptyClick onBxyEmptyClick)
        {
            mOnBxyEmptyClick = onBxyEmptyClick;

            if (mComItem == null)
            {
                mComItem = ComItemManager.CreateNew(mItemParent);
            }

            mComItem.Setup(null, null);
        }

        public void UpdateBxyItem(ItemData itemData)
        {
            if (itemData == null)
            {
                return;
            }

            var bxyItemData = ItemDataManager.CreateItemDataFromTable(itemData.TableID);
            if (bxyItemData == null)
            {
                return;
            }

            mBxyRoot.CustomActive(true);

            if (mComItem != null)
            {
                mComItem.Setup(itemData, Utility.OnItemClicked);
            }

            if (mBxyName != null)
            {
                mBxyName.text = bxyItemData.GetColorName();
            }

            if (mBxyAttr != null)
            {
                List<string> texts = itemData.GetComplexAttrDescs();
                string text = "";
                for (int i = 0; i < texts.Count; i++)
                {
                    text = text + texts[i] + "\n";
                }
                mBxyAttr.text = text;
            }
        }

        public void OnBxyEmptyClick()
        {
            Reset();

            if (mOnBxyEmptyClick != null)
            {
                mOnBxyEmptyClick.Invoke(IsBxyA);
            }
        }

        public void Reset()
        {
            mBxyRoot.CustomActive(false);

            if (mComItem != null)
            {
                mComItem.Setup(null, null);
            }

            if (mBxyName != null)
            {
                mBxyName.text = string.Empty;
            }

            if (mBxyAttr != null)
            {
                mBxyAttr.text = string.Empty;
            }
        }
    }
}