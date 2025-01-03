using UnityEngine.UI;
using ProtoTable;
using UnityEngine;

namespace GameClient
{
    public class CommonMsgOutData
    {
        public bool bExclusiveWithNewbieGuide = false;

        public void ClearData()
        {
            bExclusiveWithNewbieGuide = false;
        }
    }

    public class CommonMsgBoxOK : ClientFrame
    {
        float fUpdateInterval = 0f;
        float fCloseTime = -1f;

        CommonMsgOutData outData = new CommonMsgOutData();

        protected sealed override void _OnOpenFrame()
        {
            if (userData != null)
            {
                outData = userData as CommonMsgOutData;
            }

            BindUIEvent();
            fUpdateInterval = 0f;

            if (NewbieGuideManager.GetInstance().IsGuidingControl() && outData != null && outData.bExclusiveWithNewbieGuide)
            {
                frameMgr.CloseFrame(this);
            }
        }

        protected sealed override void _OnCloseFrame()
        {
            UnBindUIEvent();

            if(ButtonOK != null)
            {
                ButtonOK.onClick.RemoveAllListeners();
            }
            
            if(ButtonOKmini != null)
            {
                ButtonOKmini.onClick.RemoveAllListeners();
            }
          
            if(outData != null)
            {
                outData.ClearData();
            }
        }

        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/CommonSystemNotify/CommonMsgBoxOK";
        }

        protected void BindUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.CurGuideStart, OnCurGuideStart);
        }

        protected void UnBindUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.CurGuideStart, OnCurGuideStart);
        }

        void OnCurGuideStart(UIEvent iEvent)
        {
            frameMgr.CloseFrame(this);
        }

        [UIEventHandle("normal/Back/Panel/btOK")]
        void OnClickOK()
        {
            frameMgr.CloseFrame(this);
        }

        [UIEventHandle("mini/Back/Panel/btOK")]
        void OnClickOKmini()
        {
            frameMgr.CloseFrame(this);
        }

        void AdaptUIForMsgContent()
        {
            if (ContentText == null)
            {
                return;
            }

            Canvas.ForceUpdateCanvases();
            if (ContentText.cachedTextGenerator.lineCount <= 1)
            {
                if(m_goObjnormal != null)
                {
                    m_goObjnormal.CustomActive(false);
                }
               
                if(m_goObjmini != null)
                {
                    m_goObjmini.CustomActive(true);
                }
            }
        }

        public void SetMsgContent(string str)
        {
            if(m_goObjnormal != null)
            {
                m_goObjnormal.CustomActive(true);
            }
          
            if(m_goObjmini != null)
            {
                m_goObjmini.CustomActive(false);
            }
            
            if(ContentText != null)
            {
                ContentText.text = str;
            }
            
            if(ContentTextmini != null)
            {
                ContentTextmini.text = str;
            }

            AdaptUIForMsgContent();
        }

        public void SetOkBtnText(string str)
        {
            if(ButtonText != null)
            {
                ButtonText.text = str;
            }
            
            if(ButtonTextmini != null)
            {
                ButtonTextmini.text = str;
            }
        }

        public void SetNotifyDataByTable(ProtoTable.CommonTipsDesc NotifyData, string content)
        {
            if(NotifyData != null)
            {
                SetMsgContent(content);

                if (NotifyData.ButtonText != "" && NotifyData.ButtonText != "-" && NotifyData.ButtonText != "0")
                {
                    SetOkBtnText(NotifyData.ButtonText);
                }
            }            
        }

        public void AddListener(UnityEngine.Events.UnityAction OnOKCallBack)
        {
            if(OnOKCallBack != null)
            {
                if(ButtonOK != null)
                {
                    ButtonOK.onClick.AddListener(OnOKCallBack);
                }
                
                if(ButtonOKmini != null)
                {
                    ButtonOKmini.onClick.AddListener(OnOKCallBack);
                } 
            }
        }

        public void SetAutoCloseTime(float CloseTime) // 大于0设置自动关闭时间
        {
            fCloseTime = CloseTime;
        }

        public sealed override bool IsNeedUpdate()
        {
            return true;
        }

        protected sealed override void _OnUpdate(float timeElapsed)
        {
            if(fCloseTime <= 0f)
            {
                return;
            }

            fUpdateInterval += timeElapsed;

            if (fUpdateInterval <= fCloseTime)
            {
                return;
            }

            if(ButtonOK != null && ButtonOK.IsActive())
            {
                ButtonOK.onClick.Invoke();
            }

            if (ButtonOKmini != null && ButtonOKmini.IsActive())
            {
                ButtonOKmini.onClick.Invoke();
            }

            frameMgr.CloseFrame(this);
        }

        [UIControl("normal/Back/Title/Text")]
        protected Text TitleText;

        [UIControl("normal/Back/TextPanel/AlertText")]
        protected Text ContentText;

        [UIControl("normal/Back/Panel/btOK/Text")]
        protected Text ButtonText;

        [UIControl("normal/Back/Panel/btOK")]
        protected Button ButtonOK;

        [UIControl("mini/Back/Title/Text")]
        protected Text TitleTextmini;

        [UIControl("mini/Back/TextPanel/AlertText")]
        protected Text ContentTextmini;

        [UIControl("mini/Back/Panel/btOK/Text")]
        protected Text ButtonTextmini;

        [UIControl("mini/Back/Panel/btOK")]
        protected Button ButtonOKmini;

        [UIObject("normal")]
        GameObject m_goObjnormal;

        [UIObject("mini")]
        GameObject m_goObjmini;
    }
}
