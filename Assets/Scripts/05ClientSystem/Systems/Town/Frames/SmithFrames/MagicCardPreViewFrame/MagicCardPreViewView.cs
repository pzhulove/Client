using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scripts.UI;

namespace GameClient
{
    public class MagicCardPreViewView : MonoBehaviour
    {
        [SerializeField]
        private ComUIListScript mComUIListScript;

        List<ItemData> mPreViewItemList;
        public void InitView(List<ItemData> data)
        {
            mPreViewItemList = data;
            InitComUIListScript();
        }

        void InitComUIListScript()
        {
            mComUIListScript.Initialize();
            mComUIListScript.onBindItem += OnBindItemDelegate;
            mComUIListScript.onItemVisiable += OnItemVisiableDelegate;
            mComUIListScript.SetElementAmount(mPreViewItemList.Count);
        }

        MagicCardPreViewItem OnBindItemDelegate(GameObject itemObject)
        {
            var mPreViewItem = itemObject.GetComponent<MagicCardPreViewItem>();
            return mPreViewItem;
        }

        void OnItemVisiableDelegate(ComUIListElementScript item)
        {
            MagicCardPreViewItem current = item.gameObjectBindScript as MagicCardPreViewItem;
            if (current != null && item.m_index >= 0 && item.m_index < mPreViewItemList.Count)
            {
                current.InitItem(mPreViewItemList[item.m_index]);
            }
        }

        public void UnInitView()
        {
            mPreViewItemList = null;
        }
    }
}

