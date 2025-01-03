using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public enum InscriptionOperationType
    {
        InscriptionFracture = 0,// 铭文碎裂
        InscriptionPick,//铭文摘取
    }

    /// <summary>
    /// 摘取铭文数据
    /// </summary>
    public class InscriptionExtractItemData
    {
        /// <summary>
        /// 装备
        /// </summary>
        public ItemData equipmentItem;
        /// <summary>
        /// 铭文
        /// </summary>
        public ItemData inscriptionItem;
        /// <summary>
        /// 孔索引
        /// </summary>
        public int index;
    }

    public class InscriptionOperationFrame : ClientFrame
    {
        #region ExtraUIBind
        private InscriptionOperationView mInscriptionOperationView = null;
        private ButtonEx mClose = null;

        protected override void _bindExUI()
        {
            mInscriptionOperationView = mBind.GetCom<InscriptionOperationView>("InscriptionOperationView");
            mClose = mBind.GetCom<ButtonEx>("Close");
            mClose.onClick.AddListener(_onCloseButtonClick);
        }

        protected override void _unbindExUI()
        {
            mInscriptionOperationView = null;
            mClose.onClick.RemoveListener(_onCloseButtonClick);
            mClose = null;
        }
        #endregion

        #region Callback
        private void _onCloseButtonClick()
        {
            frameMgr.CloseFrame(this);
        }
        #endregion

        private InscriptionExtractItemData mInscriptionExtractItemData;
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/SmithShop/InscriptionFrame/InscriptionOperationFrame";
        }

        protected override void _OnOpenFrame()
        {
            if(userData != null)
            {
                mInscriptionExtractItemData = userData as InscriptionExtractItemData;
                if(mInscriptionExtractItemData != null)
                {
                    if(mInscriptionOperationView != null)
                    {
                        mInscriptionOperationView.InitView(mInscriptionExtractItemData);
                    }
                }
            }
        }

        protected override void _OnCloseFrame()
        {
            mInscriptionExtractItemData = null;
        }
    }
}
