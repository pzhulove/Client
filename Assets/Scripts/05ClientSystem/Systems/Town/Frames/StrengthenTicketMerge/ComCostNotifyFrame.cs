using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace GameClient
{
    public class ComCostNotifyData
    {
        public string strContent;
        public System.Action<bool> delSetNotify;
        public System.Func<bool> delGetNotify;
        public UnityAction delOnOkCallback;
        public UnityAction delOnCancelCallback;
    }

    class ComCostNotifyFrame : ClientFrame
    {
        public static

        ComCostNotifyData data = null;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/StrengthenTicketMerge/ComCostNotify";
        }

        protected override void _OnOpenFrame()
        {
            data = userData as ComCostNotifyData;
            if (data != null)
            {
                if (mAlertText)
                {
                    mAlertText.text = data.strContent;
                }

                if (mCanNotify && data.delGetNotify != null)
                {
                    mCanNotify.isOn = data.delGetNotify();
                }
            }
        }

        protected override void _OnCloseFrame()
        {
            data = null;
        }

        #region ExtraUIBind
        private Text mAlertText = null;
        private Toggle mCanNotify = null;
        private Button mBtCancel = null;
        private Button mBtOK = null;

        protected override void _bindExUI()
        {
            mAlertText = mBind.GetCom<Text>("AlertText");
            mCanNotify = mBind.GetCom<Toggle>("CanNotify");
            if (null != mCanNotify)
            {
                mCanNotify.onValueChanged.AddListener(_onCanNotifyToggleValueChange);
            }
            mBtCancel = mBind.GetCom<Button>("btCancel");
            if (null != mBtCancel)
            {
                mBtCancel.onClick.AddListener(_onBtCancelButtonClick);
            }
            mBtOK = mBind.GetCom<Button>("btOK");
            if (null != mBtOK)
            {
                mBtOK.onClick.AddListener(_onBtOKButtonClick);
            }
        }

        protected override void _unbindExUI()
        {
            mAlertText = null;
            if (null != mCanNotify)
            {
                mCanNotify.onValueChanged.RemoveListener(_onCanNotifyToggleValueChange);
            }
            mCanNotify = null;
            if (null != mBtCancel)
            {
                mBtCancel.onClick.RemoveListener(_onBtCancelButtonClick);
            }
            mBtCancel = null;
            if (null != mBtOK)
            {
                mBtOK.onClick.RemoveListener(_onBtOKButtonClick);
            }
            mBtOK = null;
        }
        #endregion

        #region Callback
        private void _onCanNotifyToggleValueChange(bool changed)
        {
            /* put your code in here */
            if (data != null && data.delSetNotify != null)
            {
                data.delSetNotify(changed);
            }
        }
        private void _onBtCancelButtonClick()
        {
            /* put your code in here */

            if (data != null && data.delOnCancelCallback != null)
            {
                data.delOnCancelCallback();
            }

            frameMgr.CloseFrame(this);
        }
        private void _onBtOKButtonClick()
        {
            /* put your code in here */

            if (data != null && data.delOnOkCallback != null)
            {
                data.delOnOkCallback();
            }

            frameMgr.CloseFrame(this);
        }
        #endregion
    }
}
