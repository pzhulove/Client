using UnityEngine;
using System.Collections;
using GameClient;
using UnityEngine.UI;
using Protocol;
using Network;
using System;

namespace _Settings
{
    public class CDKResponseMsgCN
    {
        public const string CDK_RECEIVE_SUCC = "已兑换奖励，请在邮箱查收";
        public const string CDK_CODE_WRONG = "礼包卡不正确，请重新输入";
        public const string CDK_CODE_OUTDATE = "您的礼包卡已超出使用有效期";
        public const string CDK_CODE_USED = "您的礼包卡已被使用，无法再次使用";
        public const string CDK_CODE_INVALID = "您输入的礼包卡无效，请核实礼包卡兑换条件";
    }
    public class CDKRewardSettings : SettingsBindUI
    {
        private enum CDKCodeMsgType
        {
            Succ = 0,
            Wrong,
            OutDate,
            Used,
            Invalid
        }

        private Button pasteBtn;
        private Button inputBtn;
        private InputField inputText;

        public CDKRewardSettings(GameObject root, ClientFrame frame)
            : base(root, frame)
        {

        }

        protected override string GetCurrGameObjectPath()
        {
            return "UIRoot/UI2DRoot/Middle/SettingPanel/Panel/Contents/cdkCharge";
        }

        protected override void InitBind()
        {
            pasteBtn = mBind.GetCom<Button>("PasteBtn");
            pasteBtn.onClick.AddListener(OnPasteBtnClick);
            inputBtn = mBind.GetCom<Button>("InputBtn");
            inputBtn.onClick.AddListener(OnInputBtnClick);
            inputText = mBind.GetCom<InputField>("InputField");
        }

        protected override void UnInitBind()
        {
            if(pasteBtn)
                 pasteBtn.onClick.RemoveListener(OnPasteBtnClick);
            pasteBtn = null;
            if(inputBtn)
                 inputBtn.onClick.RemoveListener(OnInputBtnClick);
            inputBtn = null;
            inputText = null;
        }

        protected override void OnShowOut()
        {
           
        }

        protected override void OnHideIn()
        {
            if (inputText)
                inputText.text = "";
        }


        void OnPasteBtnClick()
        {
            inputText.text = SDKInterface.Instance.GetClipboardText();
        }

        void OnInputBtnClick()
        {
            if (!string.IsNullOrEmpty(inputText.text))
            {
                string code = inputText.text;
                code = code.Replace("\r", "");
                code = code.Replace("\n", "");
                code = code.Replace("\t", "");
                code = code.Replace(" ", "");
                CDKManager.GetInstance().SendCDKcode(code.Trim());
            }
        }

        void OnCDKReturn(CDKData cdkData)
        {
            int cdkMsg = (int)cdkData.CDKReturnCode;
            Logger.LogProcessFormat("[CDK Return Code Int] - {0}", cdkMsg);
            switch (cdkMsg)
            {
                case (int)CDKCodeMsgType.Succ:
                    SystemNotifyManager.SysNotifyTextAnimation(CDKResponseMsgCN.CDK_RECEIVE_SUCC);
                    break;
                case (int)CDKCodeMsgType.Wrong:
                    SystemNotifyManager.SysNotifyTextAnimation(CDKResponseMsgCN.CDK_CODE_WRONG);
                    break;
                case (int)CDKCodeMsgType.Used:
                    SystemNotifyManager.SysNotifyTextAnimation(CDKResponseMsgCN.CDK_CODE_USED);
                    break;
                case (int)CDKCodeMsgType.OutDate:
                    SystemNotifyManager.SysNotifyTextAnimation(CDKResponseMsgCN.CDK_CODE_OUTDATE);
                    break;
                case (int)CDKCodeMsgType.Invalid:
                    SystemNotifyManager.SysNotifyTextAnimation(CDKResponseMsgCN.CDK_CODE_INVALID);
                    break;
            }
        }
    }

    public class CDKManager : Singleton<CDKManager>
    {
        public override void Init()
        {
            //NetProcess.AddMsgHandler(SceneCdkRes.MsgID, OnSyncCDKReturn);
        }

        public override void UnInit()
        {
            //NetProcess.RemoveMsgHandler(SceneCdkRes.MsgID, OnSyncCDKReturn);
        }

        private CDKData NetData2LocalData(SceneCdkRes data)
        {
            CDKData cdkData = new CDKData();
            cdkData.CDKReturnCode = data.code;
            return cdkData;
        }

        public void SendCDKcode(string cdkCode)
        {
            SceneCdkReq reqCDK = new SceneCdkReq();
            reqCDK.cdk = cdkCode;
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, reqCDK);
        }

        /*
        private void OnSyncCDKReturn(MsgDATA msg)
        {
            int pos = 0;
            SceneCdkRes msgCDK = new SceneCdkRes();
            msgCDK.decode(msg.bytes, ref pos);

            CDKData cdkData = NetData2LocalData(msgCDK);
            ReceiveCDKData(cdkData);
        }

        public event System.Action<CDKData> CDKReturnHandler;
        public void ReceiveCDKData(CDKData cdkData)
        {
            if (CDKReturnHandler != null)
                CDKReturnHandler(cdkData);
        }
         * */
    }

    public class CDKData
    {
        public UInt32 CDKReturnCode;
    }

}
