using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;

namespace GameClient
{
    public class ItemTipsFuncOtherButton : MonoBehaviour
    {
        [SerializeField]private ComUIListScript mFuncItemUIList;
        [SerializeField]private GameObject mTabItemRoot;
        [SerializeField]private GameObject mDownArrow;
        [SerializeField]private GameObject mUpArrow;
        [SerializeField]private Text mDesc;
        [SerializeField]private string mStrMore = "更多";
        [SerializeField]private string mStrPackUp = "收起";

        List<TipFuncButon> mFuncInfos = null;
        ItemData mItem = null;
        int mTipIndex = 0;

        private void Awake()
        {
            InitFuncItemUIList();
        }

        public void Initialize(ItemData item, List<TipFuncButon> funcInfos, int nTipIndex)
        {
            mItem = item;
            mFuncInfos = new List<TipFuncButon>();
            mFuncInfos = funcInfos;
            mTipIndex = nTipIndex;

            if(mFuncItemUIList != null)
            mFuncItemUIList.SetElementAmount(mFuncInfos.Count);
        }

        public void SetTabItemRoot()
        {
            mTabItemRoot.CustomActive(!mTabItemRoot.activeSelf);

            if (!mTabItemRoot.activeSelf)
            {
                mDesc.text = mStrPackUp;
                mUpArrow.CustomActive(true);
                mDownArrow.CustomActive(false);
            }
            else
            {
                mDesc.text = mStrMore;
                mUpArrow.CustomActive(false);
                mDownArrow.CustomActive(true);
            }
        }

        private void InitFuncItemUIList()
        {
            if (mFuncItemUIList != null)
            {
                mFuncItemUIList.Initialize();
                mFuncItemUIList.onBindItem += OnBindItemDelegate;
                mFuncItemUIList.onItemVisiable += OnItemVisiableDelegate;
            }
        }

        private void UnInitFuncItemUIList()
        {
            if (mFuncItemUIList != null)
            {
                mFuncItemUIList.onItemVisiable -= OnItemVisiableDelegate;
            }

            mFuncItemUIList = null;
        }

        private ComCommonBind OnBindItemDelegate(GameObject itemObject)
        {
            return itemObject.GetComponent<ComCommonBind>();
        }

        private void OnItemVisiableDelegate(ComUIListElementScript item)
        {
            ComCommonBind mBind = item.gameObjectBindScript as ComCommonBind;
            if (mBind == null) return;

            if (item.m_index >= 0 && item.m_index < mFuncInfos.Count)
            {
                TipFuncButon tipFuncBtn = mFuncInfos[item.m_index];
                Text mName = mBind.GetCom<Text>("name");
                Button btn = mBind.GetCom<Button>("TabItemBtn");

                if (mName != null)
                {
                    mName.text = tipFuncBtn.text;
                }
                
                if (btn != null)
                {
                    btn.onClick.RemoveAllListeners();
                    btn.onClick.AddListener(()=>
                    {
                        tipFuncBtn.callback(mItem, mTipIndex);
                    });
                }
            }
        }

        private void OnDestroy()
        {
            mFuncInfos = null;
            mItem = null;
            mTipIndex = 0;
            UnInitFuncItemUIList();
        }
    }
}

