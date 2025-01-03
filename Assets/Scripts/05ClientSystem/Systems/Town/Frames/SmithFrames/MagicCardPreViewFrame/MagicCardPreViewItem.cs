using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace GameClient
{
    public class MagicCardPreViewItem : MonoBehaviour, IDisposable
    {
        [SerializeField]
        private GameObject mItemParent;
        [SerializeField]
        private Text mName;

        ComItemNew mComItem;
        public void InitItem(ItemData item)
        {
            if (mComItem == null)
            {
                mComItem = ComItemManager.CreateNew(mItemParent);
            }

            mComItem.Setup(item, Utility.OnItemClicked);
            mName.text = item.GetColorName();
        }

        public void Dispose()
        {
            if (mComItem != null)
            {
                mComItem.Setup(null, null);
                mComItem = null;
            }
        }
    }
}
