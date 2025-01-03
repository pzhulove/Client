using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace GameClient
{
    public enum MsgBoxDataType
    {
        MBDT_OK = (1 << 0),
        MBDT_CANCEL = (1 << 1),
        MBDT_TITLE = (1 << 2),
        MBDT_REVERSE = (1 << 3),
    }

    public class MsgBoxData
    {
        public int iID;
        public string prefab;
        public string title;
        public string content;
        public string ok;
        public string cancel;
        public MsgBoxDataType flags;
        public UnityAction OnOK;
        public UnityAction OnCancel;
    };

    public class CommonMessageBox : ClientFrame
    {
        [UIControl("Title/titletext", typeof(Text))]
        Text title;
        [UIControl("Content", typeof(Text))]
        Text con;
        [UIControl("OK/Image", typeof(Text))]
        Text ok;
        [UIControl("CANCEL/Image", typeof(Text))]
        Text cancel;
        [UIControl("OK", typeof(Button))]
        Button OK;
        [UIControl("CANCEL", typeof(Button))]
        Button CANCEL;
        [UIObject("Title")]
        GameObject goTitle;

        MsgBoxData data = null;
        MsgBoxData Value
        {
            get
            {
                return data;
            }
            set
            {
                data = value;
            }
        }

        bool IsReverse
        {
            get
            {
                return _HasFlag(MsgBoxDataType.MBDT_REVERSE);
            }
        }

        bool _HasFlag(MsgBoxDataType eFlag)
        {
            return (eFlag & Value.flags) == eFlag;
        }

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/SmithShop/CrazyAdjustFinish";
        }

        protected override void _OnOpenFrame()
        {
            Value = userData as MsgBoxData;

            title.text = Value.title;
            con.text = Value.content;
            if (IsReverse)
            {
                ok.text = Value.cancel;
                cancel.text = Value.ok;
            }
            else
            {
                ok.text = Value.ok;
                cancel.text = Value.cancel;
            }

            goTitle.CustomActive(_HasFlag(MsgBoxDataType.MBDT_TITLE));
        }

        protected override void _OnCloseFrame()
        {
            Value = null;
        }

        [UIEventHandle("OK")]
        void OnClickOK()
        {
            if (IsReverse)
            {
                if (Value.OnCancel != null)
                {
                    Value.OnCancel();
                }
            }
            else
            {
                if (Value.OnOK != null)
                {
                    Value.OnOK();
                }
            }

            Close();
        }

        [UIEventHandle("CANCEL")]
        void OnClickCancel()
        {
            if (IsReverse)
            {
                if(Value.OnOK != null)
                {
                    Value.OnOK();
                }
            }
            else
            {
                if (Value.OnCancel != null)
                {
                    Value.OnCancel();
                }
            }

            Close();
        }

        [UIEventHandle("Title/closeicon")]
        void OnClickCancel2()
        {
            OnClickCancel();
        }

        private static string TranslateMsg(string des, params object[] args)
        {
            string FinalStr = des;

            if (args != null)
            {
                string descs = System.Text.RegularExpressions.Regex.Replace(FinalStr, "\\{[\\w]*\\:([\\d]*)\\}", "{$1}");
                FinalStr = string.Format(descs, args);
            }

            FinalStr = FinalStr.Replace("\\n", "\n");

            return FinalStr;
        }

        public static void Notify(int iID,
            string path,
            object[] arg0 = null,
            UnityAction ok = null,
            UnityAction cancel = null, 
            object[] arg1 = null, 
            object[] arg2 = null, 
            object[] arg3 = null)
        {
            var comDescItem = TableManager.GetInstance().GetTableItem<ProtoTable.CommonTipsDesc>(iID);
            if(comDescItem == null)
            {
                Logger.LogErrorFormat("can not find common desc id = {0}", iID);
                return;
            }

            if(string.IsNullOrEmpty(path))
            {
                Logger.LogErrorFormat("prefab path is error!");
                return;
            }

            MsgBoxDataType flags = MsgBoxDataType.MBDT_OK;
            if(comDescItem.ShowType == ProtoTable.CommonTipsDesc.eShowType.CT_MSG_BOX_OK)
            {
                flags = MsgBoxDataType.MBDT_OK;
            }
            else if(comDescItem.ShowType == ProtoTable.CommonTipsDesc.eShowType.CT_MSG_BOX_OK_CANCEL)
            {
                flags = MsgBoxDataType.MBDT_OK | MsgBoxDataType.MBDT_CANCEL;
            }
            else if(comDescItem.ShowType == ProtoTable.CommonTipsDesc.eShowType.CT_MSG_BOX_CANCEL_OK)
            {
                flags = MsgBoxDataType.MBDT_OK | MsgBoxDataType.MBDT_CANCEL | MsgBoxDataType.MBDT_REVERSE;
            }

            if(!string.IsNullOrEmpty(comDescItem.TitleText) && !string.Equals("-", comDescItem.TitleText))
            {
                flags |= MsgBoxDataType.MBDT_TITLE;
            }

            var data = new MsgBoxData();
            data.prefab = path;
            data.content = TranslateMsg(comDescItem.Descs, arg0);
            data.title = TranslateMsg(comDescItem.TitleText, arg1);
            data.ok = TranslateMsg(comDescItem.ButtonText, arg2);
            data.cancel = TranslateMsg(comDescItem.CancelBtnText, arg3);
            data.OnOK = ok;
            data.OnCancel = cancel;
            data.flags = flags;
            data.iID = iID;

            Notify(data);
        }

        public static void Notify(MsgBoxData data)
        {
            ClientSystemManager.GetInstance().OpenFrame<CommonMessageBox>(FrameLayer.Middle, data, "CommonMessageBox" + data.iID);
        }
    }
}