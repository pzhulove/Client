using UnityEngine.UI;

namespace GameClient
{
    public class BaseMsgBoxOK : ClientFrame
    {
        float fUpdateInterval = 0f;
        float fCloseTime = -1f;

        protected override void _OnOpenFrame()
        {
            if (null != ButtonOK)
            {
                ControlButtonAnimation ctrlAnim = ButtonOK.GetComponent<ControlButtonAnimation>();
                if (null != ctrlAnim)
                    ctrlAnim.enabled = false;
            }

            fUpdateInterval = 0f;
        }

        protected override void _OnCloseFrame()
        {
            if(ButtonOK != null)
                ButtonOK.onClick.RemoveAllListeners();
        }

        public override string GetPrefabPath()
        {
            return "Base/UI/Prefabs/BaseMsgBoxOK";
        }

        [UIEventHandle("normal/Back/Panel/btOK")]
        void OnClickOK()
        {
            //ButtonOK.onClick.Invoke();

            frameMgr.CloseFrame(this);
        }

        public void SetMsgContent(string str)
        {
            if (ContentText != null)
            {
                ContentText.text = str;
            }
        }

        public void SetOKBtnText(string str)
        {
            if(ButtonText != null)
            {
                ButtonText.text = str;
            }         
        }

        public void SetNotifyDataByTable(ProtoTable.CommonTipsDesc NotifyData, string content)
        {
            if(NotifyData != null)
            {
                SetMsgContent(content);

                if (NotifyData.ButtonText != "" && NotifyData.ButtonText != "-" && NotifyData.ButtonText != "0")
                {
                    if (ButtonText != null)
                    {
                        ButtonText.text = NotifyData.ButtonText;
                    }
                }
            }            
        }

        public void AddListener(UnityEngine.Events.UnityAction OnOKCallBack)
        {
            if(OnOKCallBack != null)
            {
                if (ButtonOK != null)
                {
                    ButtonOK.onClick.RemoveListener(OnOKCallBack);
                    ButtonOK.onClick.AddListener(OnOKCallBack);
                }
            }
        }

        public void SetAutoCloseTime(float CloseTime) // 大于0设置自动关闭时间
        {
            fCloseTime = CloseTime;
        }

        public override bool IsNeedUpdate()
        {
            return true;
        }

        protected override void _OnUpdate(float timeElapsed)
        {
//             if(fCloseTime <= 0f)
//             {
//                 return;
//             }
// 
//             fUpdateInterval += timeElapsed;
// 
//             if (fUpdateInterval <= fCloseTime)
//             {
//                 return;
//             }
// 
//             ButtonOK.onClick.Invoke();
//             frameMgr.CloseFrame(this);
        }

        [UIControl("normal/Back/Title/Text")]
        protected Text TitleText;

        [UIControl("normal/Back/TextPanel/AlertText")]
        protected Text ContentText;

        [UIControl("normal/Back/Panel/btOK/Text")]
        protected Text ButtonText;

        [UIControl("normal/Back/Panel/btOK")]
        protected Button ButtonOK;
    }
}
