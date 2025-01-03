using UnityEngine.UI;

namespace GameClient
{
    public class CommonConfirmFrame : ClientFrame
    {
        protected override void _OnOpenFrame()
        {
        }

        protected override void _OnCloseFrame()
        {
        }

        public override string GetPrefabPath()
        {
			return "";
        }

        [UIEventHandle("main/Panel/BtnOK")]
        void OnClickOK()
        {
            frameMgr.CloseFrame(this);
        }

        // 设置显示内容
        public void SetMsgContent(string str)
        {
            
        }

        // 设置标题（如果有需求）
        public void SetTitleContent(string str)
        {
            
        }

        public void SetNotifyDataByTable(ProtoTable.CommonTipsDesc NotifyData, string content)
        {
            if(NotifyData != null)
            {
                SetTitleContent(NotifyData.TitleText);
                SetMsgContent(content);

                if (NotifyData.ButtonText != "" && NotifyData.ButtonText != "-" && NotifyData.ButtonText != "0")
                {
                    //OKText.text = NotifyData.ButtonText;
                }
            }            
        }

        public void AddListener(UnityEngine.Events.UnityAction OnOKCallBack)
        {
            if(OnOKCallBack != null)
            {
//                 ButtonOK.onClick.RemoveListener(OnOKCallBack);
//                 ButtonOK.onClick.AddListener(OnOKCallBack);
            }
        }

        public override bool IsNeedUpdate()
        {
            return true;
        }

        protected override void _OnUpdate(float timeElapsed)
        {

        }

//         [UIControl("main/Title")]
//         protected Text TitleText;
// 
//         [UIControl("main/TextPanel/AlertText")]
//         protected Text ContentText;
// 
//         [UIControl("main/Panel/BtnOK")]
//         protected Button ButtonOK;
    }
}
