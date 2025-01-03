using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProtoTable;

namespace GameClient
{
    //部位和左右
    public class FashionItemSelectedType
    {
        public ItemTable.eSubType FashionPart;
        public bool IsLeft;
        public bool NeedBlue = true;
    }

    public class FashionMergeNewItemFrame : ClientFrame
    {
        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/SmithShop/FashionSmithShop/FashionMergeNewItemFrame";
        }

        protected override void _OnOpenFrame()
        {
            base._OnOpenFrame();

            var fashionItemSelectedType = userData as FashionItemSelectedType;

            if (mFashionMergeNewItemView != null && fashionItemSelectedType != null)
            {
                mFashionMergeNewItemView.InitData(fashionItemSelectedType.FashionPart, fashionItemSelectedType.IsLeft , fashionItemSelectedType.NeedBlue);
            }
            
        }

        #region ExtraUIBind
        private FashionMergeNewItemView mFashionMergeNewItemView = null;

        protected override void _bindExUI()
        {
            mFashionMergeNewItemView = mBind.GetCom<FashionMergeNewItemView>("FashionMergeNewItemView");
        }

        protected override void _unbindExUI()
        {
            mFashionMergeNewItemView = null;
        }
        #endregion

    }
}