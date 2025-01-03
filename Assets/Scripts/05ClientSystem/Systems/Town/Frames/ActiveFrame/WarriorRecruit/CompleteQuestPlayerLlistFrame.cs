using ProtoTable;
using Scripts.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Protocol;

namespace GameClient
{
    public class CompleteQuestPlayerLlistFrame : ClientFrame
    {
        #region ExtraUIBind
        private Button mBtClose = null;
        private ComUIListScript mUIList = null;

        protected sealed override void _bindExUI()
        {
            mBtClose = mBind.GetCom<Button>("btClose");
            mBtClose.onClick.AddListener(_onBtCloseButtonClick);
            mUIList = mBind.GetCom<ComUIListScript>("UIList");
        }

        protected sealed override void _unbindExUI()
        {
            mBtClose.onClick.RemoveListener(_onBtCloseButtonClick);
            mBtClose = null;
            mUIList = null;
        }
        #endregion

        #region Callback
        private void _onBtCloseButtonClick()
        {
            frameMgr.CloseFrame(this);
        }
        #endregion

        private List<string> recruitPlayerInfoList = new List<string>();

        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Activity/WarriorRecruit/CompleteQuestPlayerLlistFrame";
        }

        protected sealed override void _OnOpenFrame()
        {
            InitUIList();

            recruitPlayerInfoList = userData as List<string>;

            mUIList.SetElementAmount(recruitPlayerInfoList.Count);
        }

        protected sealed override void _OnCloseFrame()
        {
            UnInitUIList();

            recruitPlayerInfoList.Clear();
        }

        private void InitUIList()
        {
            if (mUIList != null)
            {
                mUIList.Initialize();
                mUIList.onBindItem += OnBindItemDelegate;
                mUIList.onItemVisiable += OnItemVisiableDelegate;
            }
        }

        private void UnInitUIList()
        {
            if (mUIList != null)
            {
                mUIList.onBindItem -= OnBindItemDelegate;
                mUIList.onItemVisiable -= OnItemVisiableDelegate;
            }
        }

        private ComCommonBind OnBindItemDelegate(GameObject itemObject)
        {
            return itemObject.GetComponent<ComCommonBind>();
        }

        private void OnItemVisiableDelegate(ComUIListElementScript item)
        {
            var comBind = item.gameObjectBindScript as ComCommonBind;

            if (comBind != null && item.m_index >= 0 && item.m_index < recruitPlayerInfoList.Count)
            {
                Text name = comBind.GetCom<Text>("name");

                string info = recruitPlayerInfoList[item.m_index];
              
                if (name != null)
                {
                    name.text = info;
                }
            }
        }
   
    }
}