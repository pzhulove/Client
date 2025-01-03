using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scripts.UI;
using UnityEngine.UI;

namespace GameClient
{
    public class RandomPropertiesView : MonoBehaviour
    {
        [SerializeField]
        private ComUIListScript mComUIListScript;
        [SerializeField]
        private Button mCloseBtn;

        List<int> mDatas = null;
        ClientFrame clientFrame;
        public void Initialize(ClientFrame clientFrame,List<int> datas)
        {
            this.clientFrame = clientFrame;
            mDatas = datas;
            mComUIListScript.Initialize();
            mComUIListScript.onBindItem += _OnBindItemDelegate;
            mComUIListScript.onItemVisiable += _OnItemVisiableDelegate;

            mComUIListScript.SetElementAmount(mDatas.Count);
            mCloseBtn.onClick.RemoveAllListeners();
            mCloseBtn.onClick.AddListener(() =>
            {
                (this.clientFrame as RandomPropertiesFrame).Close();
            });
        }

        RandomPropertiesItem _OnBindItemDelegate(GameObject itemObject)
        {
            RandomPropertiesItem mRandomPropertiesItem = itemObject.GetComponent<RandomPropertiesItem>();
            return mRandomPropertiesItem;
        }
        void _OnItemVisiableDelegate(ComUIListElementScript item)
        {
            var current = item.gameObjectBindScript as RandomPropertiesItem;
            if (current != null && item.m_index >= 0 && item.m_index < mDatas.Count)
            {
                current.OnItemVisible(mDatas[item.m_index]);
            }
        }
        
        public void UnInitialize()
        {
            if (mComUIListScript != null)
            {
                mComUIListScript.onBindItem -= _OnBindItemDelegate;
                mComUIListScript.onItemVisiable -= _OnItemVisiableDelegate;
                mComUIListScript = null;
            }
            mDatas.Clear();
            this.clientFrame = null;
            mCloseBtn.onClick.RemoveAllListeners();
        }
    }
}
