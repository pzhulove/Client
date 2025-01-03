using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.Events;

namespace GameClient
{

    public enum CommonReplaceType
    {
        CRT_BEAD = 0, //宝珠
        CRT_MAGICCARD,//附魔
        CRT_INSCRIPTIONMOSAIC //铭文镶嵌
    }

    public class CommonReplaceData
    {
        public CommonReplaceType commonReplaceType;
        public ItemData oldItemData;//（可代表装备，也可代表镶嵌的道具）
        public ItemData newItemData;
        public int holeIndex; // 孔索引（比如宝珠，铭文）
        public UnityAction callBack;
    }

    public class CommonReplaceFrame : ClientFrame
    {
        #region ExtraUIBind
        private Button mClose = null;
        private ButtonEx mCloseBtn = null;
        private CommonReplaceView mCommonReplaceView = null;

        protected override void _bindExUI()
        {
            mClose = mBind.GetCom<Button>("close");
            mClose.onClick.AddListener(_onCloseButtonClick);
            mCloseBtn = mBind.GetCom<ButtonEx>("CloseBtn");
            mCloseBtn.onClick.AddListener(_onCloseBtnButtonClick);
            mCommonReplaceView = mBind.GetCom<CommonReplaceView>("CommonReplaceView");
        }

        protected override void _unbindExUI()
        {
            mClose.onClick.RemoveListener(_onCloseButtonClick);
            mClose = null;
            mCloseBtn.onClick.RemoveListener(_onCloseBtnButtonClick);
            mCloseBtn = null;
            mCommonReplaceView = null;
        }
        #endregion

        #region Callback
        private void _onCloseButtonClick()
        {
            OnCloseBtnClick();
        }
        private void _onCloseBtnButtonClick()
        {
            OnCloseBtnClick();
        }

        private void OnCloseBtnClick()
        {
            frameMgr.CloseFrame(this);
        }
        #endregion

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/SmithShopNewFrame/CommonReplaceFrame";
        }

        protected override void _OnOpenFrame()
        {
            CommonReplaceData commonReplaceData = userData as CommonReplaceData;
            if(commonReplaceData != null)
            {
                if (mCommonReplaceView != null)
                    mCommonReplaceView.InitView(commonReplaceData);
            }
        }

        protected override void _OnCloseFrame()
        {
           
        }
    }
}