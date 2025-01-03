using UnityEngine.UI;

namespace GameClient
{
    public class BaseMsgBoxOKCancel : ClientFrame
    {
        public delegate void OnResponseOK();
        public OnResponseOK responseOK;

        public delegate void OnResponseCancel();
        public OnResponseCancel responseCancel;

        protected sealed override void _OnOpenFrame()
        {
        }

        protected sealed override void _OnCloseFrame()
        {
            if (btOK != null)
            {
                btOK.onClick.RemoveAllListeners();
            }
            
            if (btCancel != null)
            {
                btCancel.onClick.RemoveAllListeners();
            }
        }

        public sealed override string GetPrefabPath()
        {
			return "Base/UI/Prefabs/BaseMsgBoxOKCancel";
        }

        public void SetMsgContent(string str)
        {
            if (msgText != null)
            {
                msgText.text = str;
            }
        }

        public void SetOkBtnText(string str)
        {
            if (OKText != null)
            {
                OKText.text = str;
            }
        }

        public void SetCancelBtnText(string str)
        {
            if (CancelText != null)
            {
                CancelText.text = str;
            }
        }

        public void SetNotifyDataByTable(ProtoTable.CommonTipsDesc NotifyData, string content)
        {
            if (NotifyData != null)
            {
                SetMsgContent(content);

                if (NotifyData.ButtonText != "" && NotifyData.ButtonText != "-" && NotifyData.ButtonText != "0")
                {
                    SetOkBtnText(NotifyData.ButtonText);
                }

                if (NotifyData.CancelBtnText != "" && NotifyData.CancelBtnText != "-" && NotifyData.CancelBtnText != "0")
                {
                    SetCancelBtnText(NotifyData.CancelBtnText);
                }
            }
        }

        public void AddListener(UnityEngine.Events.UnityAction OnOKCallBack, UnityEngine.Events.UnityAction OnCancelCallBack)
        {
            if (OnOKCallBack != null)
            {
                if (btOK != null)
                {
                    btOK.onClick.RemoveListener(OnOKCallBack);
                    btOK.onClick.AddListener(OnOKCallBack);
                }
            }

            if(OnCancelCallBack != null)
            {
                if (btCancel != null)
                {
                    btCancel.onClick.RemoveListener(OnCancelCallBack);
                    btCancel.onClick.AddListener(OnCancelCallBack);
                }
            }
        }

        [UIEventHandle("normal/Back/Panel/btOK")]
        void OnClickOK()
        {
            if(responseOK != null)
            {
                responseOK();
            }

            responseOK = null;
            frameMgr.CloseFrame(this);
        }

        [UIEventHandle("normal/Back/Panel/btCancel")]
        void OnClickCancel()
        {
            if (responseCancel != null)
            {
                responseCancel();
            }

            responseCancel = null;
            frameMgr.CloseFrame(this);
        }

        [UIControl("normal/Back/TextPanel/AlertText")]
        protected Text msgText;

        [UIControl("normal/Back/Panel/btOK/Text")]
        protected Text OKText;

        [UIControl("normal/Back/Panel/btCancel/Text")]
        protected Text CancelText;

        [UIControl("normal/Back/Panel/btOK")]
        protected Button btOK;

        [UIControl("normal/Back/Panel/btCancel")]
        protected Button btCancel;
    }
}
