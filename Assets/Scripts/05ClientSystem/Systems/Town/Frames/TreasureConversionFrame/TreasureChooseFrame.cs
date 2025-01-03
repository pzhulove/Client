using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Scripts.UI;

namespace GameClient
{
    public class TreasureChooseParam
    {
        public List<ItemData> treasureChooseList;
        public Action<ItemData> treasureChooseCallback;
    }
    public class TreasureChooseFrame : ClientFrame
    {
        #region ExtraUIBind
        private ComUIListScript mScrollView = null;

        protected sealed override void _bindExUI()
        {
            mScrollView = mBind.GetCom<ComUIListScript>("ScrollView");
        }

        protected sealed override void _unbindExUI()
        {
            mScrollView = null;
        }
        #endregion

        private TreasureChooseParam param;
        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/TreasureConversionFrame/TreasureChooseFrame";
        }

        protected sealed override void _OnOpenFrame()
        {
            param = userData as TreasureChooseParam;
            if (param == null)
            {
                return;
            }

            InitComUIListScription();

            if (param.treasureChooseList != null)
            {
                mScrollView.SetElementAmount(param.treasureChooseList.Count);
            }
        }

        protected sealed override void _OnCloseFrame()
        {
            UnInitComUIListScription();
            param = null;
        }

        private void InitComUIListScription()
        {
            if (mScrollView != null)
            {
                mScrollView.Initialize();
                mScrollView.onBindItem += OnBindItemDelegate;
                mScrollView.onItemVisiable += OnItemVisiableDelegate;
                mScrollView.onItemSelected += OnItemSelectedDelegate;
            }
        }

        private void UnInitComUIListScription()
        {
            if (mScrollView != null)
            {
                mScrollView.onBindItem -= OnBindItemDelegate;
                mScrollView.onItemVisiable -= OnItemVisiableDelegate;
                mScrollView.onItemSelected -= OnItemSelectedDelegate;
            }
        }

        private TreasureConversionItem OnBindItemDelegate(GameObject itemObject)
        {
            return itemObject.GetComponent<TreasureConversionItem>();
        }

        private void OnItemVisiableDelegate(ComUIListElementScript item)
        {
            var treasureConversionItem = item.gameObjectBindScript as TreasureConversionItem;
            if (treasureConversionItem != null && item.m_index >= 0 &&item.m_index < param.treasureChooseList.Count)
            {
                treasureConversionItem.OnItemVisiable(param.treasureChooseList[item.m_index]);
            }
        }

        private void OnItemSelectedDelegate(ComUIListElementScript item)
        {
            var treasureConversionItem = item.gameObjectBindScript as TreasureConversionItem;
            if (treasureConversionItem != null)
            {
                if (param.treasureChooseCallback != null)
                {
                    param.treasureChooseCallback.Invoke(treasureConversionItem.TreasureItemData);
                }

                Close();
            }
        }
    }
}