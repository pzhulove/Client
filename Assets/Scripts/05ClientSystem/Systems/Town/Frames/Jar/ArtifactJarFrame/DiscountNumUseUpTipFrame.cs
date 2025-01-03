using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

namespace GameClient
{
    class DiscountNumUseUpTipFrame : ClientFrame
    {
        #region var
        UnityAction okCallBack = null;

        #endregion

        #region ui bind
        private Text m_labContent = null;
        private Toggle m_togCanNotify = null;
        private Button m_btnOk = null;
        #endregion

        #region override
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/ArtifactJar/DiscountNumUseUpTip";
        }

        protected override void _OnOpenFrame()
        {
            okCallBack = null;

            if(userData != null && userData is UnityAction)
            {
                okCallBack = (UnityAction)userData;
            }

            InitUI();
        }

        protected override void _OnCloseFrame()
        {
            okCallBack = null;
        }

        protected override void _bindExUI()
        {
            m_labContent = mBind.GetCom<Text>("tip");

            m_togCanNotify = mBind.GetCom<Toggle>("CanNotify");
            m_togCanNotify.SafeAddOnValueChangedListener(var =>
            {
                ArtifactDataManager.GetInstance().isNotNotify = var;
            });

            m_btnOk = mBind.GetCom<Button>("OK");
            m_btnOk.SafeRemoveAllListener();
            m_btnOk.SafeAddOnClickListener(() =>
            {
                if(okCallBack != null)
                {
                    okCallBack();
                }

                frameMgr.CloseFrame(this);
            });
        }

        protected override void _unbindExUI()
        {
            m_labContent = null;
            m_togCanNotify = null;
            m_btnOk = null;
        }
        #endregion


        #region method
        private void InitUI()
        {
            m_labContent.SafeSetText(TR.Value("DiscountNumUseUpTip"));
            m_togCanNotify.SafeSetToggleOnState(ArtifactDataManager.GetInstance().isNotNotify);
        }
        #endregion
    }
}
