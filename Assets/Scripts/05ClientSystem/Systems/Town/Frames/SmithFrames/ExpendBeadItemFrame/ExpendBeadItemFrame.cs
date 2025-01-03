using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameClient;
using System;

namespace GameClient
{
    public class ExpendBeadItemData:IComparable<ExpendBeadItemData>
    {
       /// <summary>
       /// 道具ID
       /// </summary>
        public int ItemID;
        /// <summary>
        /// 背包道具总数量
        /// </summary>
        public int TatleCount;//背包道具总数量
        /// <summary>
        /// 消耗道具数量
        /// </summary>
        public int Count;//消耗道具数量
        public ExpendBeadItemData(int itemID,int tatleCount,int count)
        {
            this.ItemID = itemID;
            this.TatleCount = tatleCount;
            this.Count = count;
        }

        public int CompareTo(ExpendBeadItemData other)
        {
            return other.TatleCount - this.TatleCount;
        }
    }
    public class ExpendBeadItemFrame : ClientFrame
    {
        List<ExpendBeadItemData> mDatas = null;
        #region ExtraUIBind
        private ExpendBeadItemView mExpendBeadItemView = null;

        protected sealed override void _bindExUI()
        {
            mExpendBeadItemView = mBind.GetCom<ExpendBeadItemView>("ExpendBeadItemView");
        }

        protected sealed override void _unbindExUI()
        {
            mExpendBeadItemView = null;
        }
        #endregion
        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/SmithShop/ExpendBeadItemFrame/ExpendBeadItemFrame";
        }

        protected sealed override void _OnOpenFrame()
        {
            base._OnOpenFrame();
            mDatas = userData as List<ExpendBeadItemData>;
            if (mDatas != null)
            {
                mExpendBeadItemView.Initialize(this,mDatas, OnSelectItem);
            }
        }

        void OnSelectItem(ExpendBeadItemData data)
        {
            if (data != null)
            {
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnSelectExpendBeadItem, data);
            }
            frameMgr.CloseFrame(this);
        }
        protected sealed override void _OnCloseFrame()
        {
            base._OnCloseFrame();
            mExpendBeadItemView.UnInitialize();
        }
    }
}

