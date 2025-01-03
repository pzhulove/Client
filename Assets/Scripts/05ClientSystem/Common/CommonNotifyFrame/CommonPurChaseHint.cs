using UnityEngine.UI;
using ProtoTable;

namespace GameClient
{
    class CommonMsgData
    {
        public string ok;
        public string cancel;
        public string msg;
        public delegate void OnOk();
        public OnOk onClickOk;
        public delegate void OnCancel();
        public OnCancel onClickCancel;
    }

    class CommonPurChaseHintFrame : ClientFrame
    {
        public static void Open(string msg,string ok = "确定",string cancel = "取消", CommonMsgData.OnOk onOk = null,CommonMsgData.OnCancel onCancel = null)
        {
            var data = new CommonMsgData
            {
                msg = msg,
                ok = ok,
                cancel = cancel,
                onClickOk = onOk,
                onClickCancel = onCancel,
            };
            ClientSystemManager.GetInstance().OpenFrame<CommonPurChaseHintFrame>(FrameLayer.High, data);
        }

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/MsgBox/SpecialMsgBoxOKCancel";
        }

        CommonMsgData data = null;
        protected override void _OnOpenFrame()
        {
            data = userData as CommonMsgData;
            OKText.text = data.ok;
            CancelText.text = data.cancel;
            msgText.text = data.msg;
        }

        protected override void _OnCloseFrame()
        {
            data = null;
        }

        [UIEventHandle("Back/Panel/btOK")]
        void OnClickOK()
        {
            if(data.onClickOk != null)
            {
                data.onClickOk.Invoke();
                data.onClickOk = null;
            }
            frameMgr.CloseFrame(this);
        }

        [UIEventHandle("Back/Panel/btCancel")]
        void OnClickCancel()
        {
            if (data.onClickCancel != null)
            {
                data.onClickCancel.Invoke();
                data.onClickCancel = null;
            }
            frameMgr.CloseFrame(this);
        }

        [UIControl("Back/TextPanel/AlertText")]
        protected Text msgText;

        [UIControl("Back/Panel/btOK/Text")]
        protected Text OKText;

        [UIControl("Back/Panel/btCancel/Text")]
        protected Text CancelText;

        [UIControl("Back/Panel/btOK")]
        protected Button btOK;

        [UIControl("Back/Panel/btCancel")]
        protected Button btCancel;
    }
}
