using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameClient
{
    public class MsgBoxNewdata
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
        public List<ToggleEvent> ToggleListEvent;
        public FrameLayer eLayer = FrameLayer.Middle;
    }

    public class ToggleEvent
    {
        public ToggleEvent(string tempText, UnityAction tempEvent)
        {
            toggleText = tempText;
            toggleEvent = tempEvent;
        }
        public string toggleText;
        public UnityAction toggleEvent;
    }

    public class CommonNewMessageBox : ClientFrame
    {
        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/SmithShop/CrazyAdjustConfirmFrame";
        }

        protected override void _OnOpenFrame()
        {
            MsgBoxNewdata tempData = userData as MsgBoxNewdata;
            if (mCrazyAdjustConfirmView != null)
            {
                mCrazyAdjustConfirmView.InitData(tempData);
            }
        }

        protected override void _OnCloseFrame()
        {
            if (mCrazyAdjustConfirmView != null)
                mCrazyAdjustConfirmView.Clear();
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

        #region ExtraUIBind
        private CommonNewMessageBoxView mCrazyAdjustConfirmView = null;

        protected override void _bindExUI()
        {
            mCrazyAdjustConfirmView = mBind.GetCom<CommonNewMessageBoxView>("CommonNewMessageBoxView");
        }

        protected override void _unbindExUI()
        {
            mCrazyAdjustConfirmView = null;
        }
        #endregion

        #region PUBLIC MOTHOD
        public static void Notify(string path,
            int iID,
            UnityAction ok = null,
            UnityAction cancel = null,
            List<ToggleEvent> toggleAction = null,
            object[] arg0 = null,
            object[] arg1 = null,
            object[] arg2 = null,
            object[] arg3 = null,
            FrameLayer eLayer = FrameLayer.Middle)
        {
            var comDescItem = TableManager.GetInstance().GetTableItem<ProtoTable.CommonTipsDesc>(iID);
            if (comDescItem == null)
            {
                Logger.LogErrorFormat("can not find common desc id = {0}", iID);
                return;
            }

            if (string.IsNullOrEmpty(path))
            {
                Logger.LogErrorFormat("prefab path is error!");
                return;
            }

            MsgBoxDataType flags = MsgBoxDataType.MBDT_OK;
            if (comDescItem.ShowType == ProtoTable.CommonTipsDesc.eShowType.CT_MSG_BOX_OK)
            {
                flags = MsgBoxDataType.MBDT_OK;
            }
            else if (comDescItem.ShowType == ProtoTable.CommonTipsDesc.eShowType.CT_MSG_BOX_OK_CANCEL)
            {
                flags = MsgBoxDataType.MBDT_OK | MsgBoxDataType.MBDT_CANCEL;
            }
            else if (comDescItem.ShowType == ProtoTable.CommonTipsDesc.eShowType.CT_MSG_BOX_CANCEL_OK)
            {
                flags = MsgBoxDataType.MBDT_OK | MsgBoxDataType.MBDT_CANCEL | MsgBoxDataType.MBDT_REVERSE;
            }

            if (!string.IsNullOrEmpty(comDescItem.TitleText) && !string.Equals("-", comDescItem.TitleText))
            {
                flags |= MsgBoxDataType.MBDT_TITLE;
            }

            var data = new MsgBoxNewdata();
            data.prefab = path;
            data.content = TranslateMsg(comDescItem.Descs, arg0);
            data.title = TranslateMsg(comDescItem.TitleText, arg1);
            data.ok = TranslateMsg(comDescItem.ButtonText, arg2);
            data.cancel = TranslateMsg(comDescItem.CancelBtnText, arg3);
            data.OnOK = ok;
            data.OnCancel = cancel;
            data.ToggleListEvent = toggleAction;
            data.flags = flags;
            data.iID = iID;
            data.eLayer = eLayer;

            Notify(data);
        }

        public static void Notify(MsgBoxNewdata data)
        {
            ClientSystemManager.GetInstance().OpenFrame<CommonNewMessageBox>(data.eLayer, data, "CommonNewMessageBox" + data.iID);
        }
        #endregion
    }
}

