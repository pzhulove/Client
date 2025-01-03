using UnityEngine.UI;
using ProtoTable;
using UnityEngine;

namespace GameClient
{
    public class CommonMsgBoxOKCancel : ClientFrame
    {
//         public delegate void OnResponseOK();
//         public OnResponseOK responseOK;
// 
//         public delegate void OnResponseCancel();
//         public OnResponseCancel responseCancel;

        bool bOpenCountDownFunc = false;
        float fCountDownTime = 0.0f;
        float fAddUpTime = 0.0f;
        string CancelBtnOriginText = "";

        CommonMsgOutData outData = new CommonMsgOutData();

        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/CommonSystemNotify/CommonMsgBoxOKCancel";
        }

        protected sealed override void _OnOpenFrame()
        {
            if (userData != null)
            {
                outData = userData as CommonMsgOutData;
            }

            BindUIEvent();

            if (CancelText != null)
            {
                CancelBtnOriginText = CancelText.text;
            }

            if (m_togCanNotify != null)
            {
                m_togCanNotify.onValueChanged.RemoveAllListeners();
                m_togCanNotify.isOn = !PlayerBaseData.GetInstance().isNotify;
                m_togCanNotify.onValueChanged.AddListener(var =>
                {
                    PlayerBaseData.GetInstance().isNotify = !var;
                });
            }

            if (m_togCanNotifymini != null)
            {
                m_togCanNotifymini.onValueChanged.RemoveAllListeners();
                m_togCanNotifymini.isOn = !PlayerBaseData.GetInstance().isNotify;
                m_togCanNotifymini.onValueChanged.AddListener(var =>
                {
                    PlayerBaseData.GetInstance().isNotify = !var;
                });
            }
           
            if (NewbieGuideManager.GetInstance().IsGuidingControl() && outData != null && outData.bExclusiveWithNewbieGuide)
            {
                frameMgr.CloseFrame(this);
            }
        }

        protected sealed override void _OnCloseFrame()
        {
            UnBindUIEvent();
            ClearData();
        }

        void ClearData()
        {
            bOpenCountDownFunc = false;
            fCountDownTime = 0.0f;
            fAddUpTime = 0.0f;
            CancelBtnOriginText = "";

            if (btOK != null)
            {
                btOK.onClick.RemoveAllListeners();
            }
            
            if (btCancel != null)
            {
                btCancel.onClick.RemoveAllListeners();
            }

            if (btOKmini != null)
            {
                btOKmini.onClick.RemoveAllListeners();
            }
            
            if (btCancelmini != null)
            {
                btCancelmini.onClick.RemoveAllListeners();
            }
            
            if (outData != null)
            {
                outData.ClearData();
            }
           
        }

        protected void BindUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.Pk3v3VoteEnterBattle, OnPk3v3VoteEnterBattle);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.CurGuideStart, OnCurGuideStart);
        }

        protected void UnBindUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.Pk3v3VoteEnterBattle, OnPk3v3VoteEnterBattle);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.CurGuideStart, OnCurGuideStart);
        }

        void OnPk3v3VoteEnterBattle(UIEvent iEvent)
        {
            frameMgr.CloseFrame(this);
        }

        void OnCurGuideStart(UIEvent iEvent)
        {
            frameMgr.CloseFrame(this);
        }

        void AdaptUIForMsgContent()
        {
            if(msgText == null)
            {
                return;
            }

            Canvas.ForceUpdateCanvases();
            if(msgText.cachedTextGenerator.lineCount <= 1)
            {
                if (m_goObjnormal != null)
                {
                    m_goObjnormal.CustomActive(false);
                }
                
                if (m_goObjmini != null)
                {
                    m_goObjmini.CustomActive(true);
                }
                
                return;
            }
        }

        public void SetMsgContent(string str)
        {
            if (m_goObjnormal != null)
                m_goObjnormal.CustomActive(true);

            if(m_goObjmini != null)
                m_goObjmini.CustomActive(false);

            if (msgText != null)
            {
                msgText.text = str;
            }
            else
            {
                Logger.LogErrorFormat("[msgText] is null, str = {0}", str);
            }

            if (msgTextmini != null)
                msgTextmini.text = str;

            AdaptUIForMsgContent();        
        }

        public void SetOkBtnText(string str)
        {
            if (OKText != null)
            {
                OKText.text = str;
            }
            
            if (OKTextmini != null)
            {
                OKTextmini.text = str;
            }
           
        }

        public void SetCancelBtnText(string str)
        {
            if (CancelText != null)
            {
                CancelText.text = str;
            }
            
            if (CancelTextmini != null)
            {
                CancelTextmini.text = str;
            }
           
        }

        public void SetbNotify(bool bFlag)
        {
            if (m_goCanNotifyObj != null)
            {
                m_goCanNotifyObj.CustomActive(bFlag);
            }
            
            if (m_goCanNotifyObjmini != null)
            {
                m_goCanNotifyObjmini.CustomActive(bFlag);
            }
            
        }

        public void SetNotifyDataByTable(CommonTipsDesc NotifyData)
        {
            if (NotifyData != null)
            {
                if (NotifyData.ButtonText != "" && NotifyData.ButtonText != "-" && NotifyData.ButtonText != "0")
                {
                    SetOkBtnText(NotifyData.ButtonText);
                }

                if (NotifyData.CancelBtnText != "" && NotifyData.CancelBtnText != "-" && NotifyData.CancelBtnText != "0")
                {
                    SetCancelBtnText(NotifyData.CancelBtnText);
                    CancelBtnOriginText = NotifyData.CancelBtnText;
                }
            }
        }

        public void SetCountDownTime(float fTime)
        {
            if(fTime > 0.0f)
            {
                fCountDownTime = fTime;
                bOpenCountDownFunc = true;
            }         
        }

        public void AddListener(UnityEngine.Events.UnityAction OnOKCallBack, UnityEngine.Events.UnityAction OnCancelCallBack)
        {
            if (OnOKCallBack != null)
            {
                if (btOK != null)
                {
                    btOK.onClick.AddListener(OnOKCallBack);
                }
                
                if (btOKmini != null)
                {
                    btOKmini.onClick.AddListener(OnOKCallBack);
                }
                
            }

            if(OnCancelCallBack != null)
            {
                if (btCancel != null)
                {
                    btCancel.onClick.AddListener(OnCancelCallBack);
                }
                
                if (btCancelmini != null)
                {
                    btCancelmini.onClick.AddListener(OnCancelCallBack);
                }
               
            }
        }

        [UIEventHandle("normal/Back/Panel/btOK")]
        void OnClickOK()
        {
//             if(responseOK != null)
//             {
//                 responseOK();
//             }
// 
//             responseOK = null;
            frameMgr.CloseFrame(this);
        }

        [UIEventHandle("normal/Back/Panel/btCancel")]
        void OnClickCancel()
        {
//             if (responseCancel != null)
//             {
//                 responseCancel();
//             }
// 
//             responseCancel = null;
            frameMgr.CloseFrame(this);
        }

        [UIEventHandle("mini/Back/Panel/btOK")]
        void OnClickOKmini()
        {
            //             if(responseOK != null)
            //             {
            //                 responseOK();
            //             }
            // 
            //             responseOK = null;
            frameMgr.CloseFrame(this);
        }

        [UIEventHandle("mini/Back/Panel/btCancel")]
        void OnClickCancelmini()
        {
            //             if (responseCancel != null)
            //             {
            //                 responseCancel();
            //             }
            // 
            //             responseCancel = null;
            frameMgr.CloseFrame(this);
        }

        public sealed override bool IsNeedUpdate()
        {
            return true;
        }

        protected sealed override void _OnUpdate(float timeElapsed)
        {
            if(bOpenCountDownFunc)
            {
                int iInt = (int)(fCountDownTime);

                if (CancelText != null)
                {
                    CancelText.text = string.Format("{0}({1}s)", CancelBtnOriginText, iInt);
                }

                if (CancelTextmini != null)
                {
                    CancelTextmini.text = string.Format("{0}({1}s)", CancelBtnOriginText, iInt);
                }

                fAddUpTime += timeElapsed;

                if(fAddUpTime > 1.0f)
                {
                    fCountDownTime -= 1.0f;
                    fAddUpTime = 0.0f; 
                }

                if(iInt < 0.0f)
                {
                    bOpenCountDownFunc = false;

                    if(btCancel != null && btCancel.IsActive())
                    {
                        btCancel.onClick.Invoke();
                    }

                    if(btCancelmini != null && btCancelmini.IsActive())
                    {
                        btCancelmini.onClick.Invoke();
                    }

                    frameMgr.CloseFrame(this);                  
                }
            }
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

        [UIControl("normal/CanNotify")]
        Toggle m_togCanNotify;

        [UIObject("normal/CanNotify")]
        GameObject m_goCanNotifyObj;

        [UIControl("mini/Back/TextPanel/AlertText")]
        protected Text msgTextmini;

        [UIControl("mini/Back/Panel/btOK/Text")]
        protected Text OKTextmini;

        [UIControl("mini/Back/Panel/btCancel/Text")]
        protected Text CancelTextmini;

        [UIControl("mini/Back/Panel/btOK")]
        protected Button btOKmini;

        [UIControl("mini/Back/Panel/btCancel")]
        protected Button btCancelmini;

        [UIControl("mini/CanNotify")]
        Toggle m_togCanNotifymini;

        [UIObject("mini/CanNotify")]
        GameObject m_goCanNotifyObjmini;

        [UIObject("normal")]
        GameObject m_goObjnormal;

        [UIObject("mini")]
        GameObject m_goObjmini;
    }
}
