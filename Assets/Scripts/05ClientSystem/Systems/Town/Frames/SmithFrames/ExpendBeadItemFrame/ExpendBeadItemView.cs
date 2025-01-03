using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scripts.UI;
using UnityEngine.UI;

namespace GameClient
{
    public class ExpendBeadItemView : MonoBehaviour
    {
        [SerializeField]
        private ComUIListScript mComUIListScript;
        [SerializeField]
        private Button mCloseBtn;

        List<ExpendBeadItemData> mItemSimpleDatas = null;
        public delegate void OnItemSelected(ExpendBeadItemData itemData);
        public OnItemSelected onItemSelected;
        ClientFrame clientFrame;
        public void Initialize(ClientFrame clientFrame,List<ExpendBeadItemData> datas, OnItemSelected onItemSelected)
        {
            this.clientFrame = clientFrame;
            mItemSimpleDatas = datas;
            this.onItemSelected = onItemSelected;
            mComUIListScript.Initialize();
            mComUIListScript.onBindItem += _OnBindItemDelegate;
            mComUIListScript.onItemVisiable += _OnItemVisiableDelegate;
            mComUIListScript.onItemSelected += _OnItemSelectedDelegate;
            mComUIListScript.onItemChageDisplay += _OnItemChangeDisplayDelegate;

            mComUIListScript.SetElementAmount(mItemSimpleDatas.Count);
            mCloseBtn.onClick.RemoveAllListeners();
            mCloseBtn.onClick.AddListener(() =>
            {
                (this.clientFrame as ExpendBeadItemFrame).Close();
            });
        }

        ExpendBeadItem _OnBindItemDelegate(GameObject itemObject)
        {
            ExpendBeadItem mExpendBeadItem = itemObject.GetComponent<ExpendBeadItem>();
            return mExpendBeadItem;
        }
        void _OnItemVisiableDelegate(ComUIListElementScript item)
        {
            var current = item.gameObjectBindScript as ExpendBeadItem;
            if (current != null && item.m_index >= 0 && item.m_index < mItemSimpleDatas.Count)
            {
                current.OnItemVisible(mItemSimpleDatas[item.m_index]);
            }
        }

        void _OnItemSelectedDelegate(ComUIListElementScript item)
        {
            var current = item.gameObjectBindScript as ExpendBeadItem;
            ExpendBeadItemData data = current == null ? null : current.SimpleData;
            if (onItemSelected != null)
            {
                onItemSelected.Invoke(data);
            }
        }

        void _OnItemChangeDisplayDelegate(ComUIListElementScript item, bool bSelected)
        {
            var current = item.gameObjectBindScript as ExpendBeadItem;
            if (current != null)
            {
                current.OnItemChangeDisplay(bSelected);
            }
        }

        public void UnInitialize()
        {
            if (mComUIListScript != null)
            {
                mComUIListScript.onBindItem -= _OnBindItemDelegate;
                mComUIListScript.onItemVisiable -= _OnItemVisiableDelegate;
                mComUIListScript.onItemSelected -= _OnItemSelectedDelegate;
                mComUIListScript.onItemChageDisplay -= _OnItemChangeDisplayDelegate;
                mComUIListScript = null;
            }
            this.onItemSelected -= onItemSelected;
            mItemSimpleDatas.Clear();
            this.clientFrame = null;
            mCloseBtn.onClick.RemoveAllListeners();
        }
    }
}
