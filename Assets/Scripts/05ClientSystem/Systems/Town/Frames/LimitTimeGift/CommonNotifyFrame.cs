using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using GameClient;
using UnityEngine.Events;

namespace LimitTimeGift
{
    public class CommonNotifyData
    {
        public string contentStr;
        public UnityAction onClickOkCallback;
        public UnityAction onClickCancelCallback;
        public ClientFrame ownerFrame;
    }
    public class CommonNotifyFrame : ClientFrame
    {
        private Text notifyContent;
        private Button btnCancel;
        private Button btnOk;

        private UnityAction addedClickOkCB;
        private UnityAction addedClickCancelCB;
        private ClientFrame thisframeOwner;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/LimitTimeGift/CommonNotifyFrame";
        }

        protected override void _bindExUI()
        {
            notifyContent = mBind.GetCom<Text>("notifyContent");
            btnCancel = mBind.GetCom<Button>("btnCancel");
            if (btnCancel)
            {
                btnCancel.onClick.RemoveListener(OnCancelBtnClick);
                btnCancel.onClick.AddListener(OnCancelBtnClick);
            }
            btnOk = mBind.GetCom<Button>("btnOk");
            if (btnOk)
            {
                btnOk.onClick.RemoveListener(OnOkBtnClick);
                btnOk.onClick.AddListener(OnOkBtnClick);
            }
        }
        protected override void _unbindExUI()
        {
            notifyContent = null;
            btnCancel = null;
            btnOk = null;
        }

        protected override void _OnOpenFrame()
        {
            InitCacheData();
            var data = this.userData as CommonNotifyData;
            if (notifyContent && data!=null)
            {
                notifyContent.text = data.contentStr;
                addedClickOkCB = data.onClickOkCallback;
                addedClickCancelCB = data.onClickCancelCallback;
                thisframeOwner = data.ownerFrame;
            }
        }

        protected override void _OnCloseFrame()
        {
        }

        void InitCacheData()
        {
            addedClickOkCB = null;
            addedClickCancelCB = null;
            thisframeOwner = null;
        }

        void OnCancelBtnClick()
        {
            ClientSystemManager.instance.CloseFrame(this,true);
            if (addedClickCancelCB != null)
                addedClickCancelCB();
        }

        void OnOkBtnClick()
        {
            ClientSystemManager.instance.CloseFrame(this, false);
            if (thisframeOwner != null)
            {
                ClientSystemManager.instance.CloseFrame(thisframeOwner);
            }
            if (addedClickOkCB != null)
                addedClickOkCB();
        }
    }

}