using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public delegate void OnSinanEmptyClick();
    public class SinanItem : MonoBehaviour
    {
        [SerializeField] private GameObject mSinanRoot;
        [SerializeField] private GameObject mItemParent;
        [SerializeField] private Text mSinanName;
        [SerializeField] private Text mSinanAttr;

        private OnSinanEmptyClick mOnSinanEmptyClick;
        private ComItemNew mComItem;
      
        private void OnDestroy()
        {
            mOnSinanEmptyClick = null;
            mComItem = null;
        }

        public void InitSinanItem(OnSinanEmptyClick onSinanEmptyClick)
        {
            mOnSinanEmptyClick = onSinanEmptyClick;

            if (mComItem == null)
            {
                mComItem = ComItemManager.CreateNew(mItemParent);
            }

            mComItem.Setup(null, null);
        }

        public void UpdateSinanItem(ItemData itemData)
        {
            if (itemData == null)
            {
                return;
            }

            var sinanItemData = ItemDataManager.CreateItemDataFromTable(itemData.TableID);
            if (sinanItemData == null)
            {
                return;
            }

            mSinanRoot.CustomActive(true);

            if (mComItem != null)
            {
                mComItem.Setup(itemData, Utility.OnItemClicked);
            }

            if (mSinanName != null)
            {
                mSinanName.text = sinanItemData.GetColorName();
            }

            if (mSinanAttr != null)
            {
                List<string> texts = itemData.GetSinanBuffDescs();
                string text = "";
                for (int i = 0; i < texts.Count; i++)
                {
                    text = text + texts[i] + "\n";
                }
                mSinanAttr.text = text;
            }
        }

        public void OnSinanEmptyClick()
        {
            Reset();

            if (mOnSinanEmptyClick != null)
            {
                mOnSinanEmptyClick.Invoke();
            }
        }

        public void Reset()
        {
            mSinanRoot.CustomActive(false);

            if (mComItem != null)
            {
                mComItem.Setup(null, null);
            }

            if (mSinanName != null)
            {
                mSinanName.text = string.Empty;
            }

            if (mSinanAttr != null)
            {
                mSinanAttr.text = string.Empty;
            }
        }
    }
}