using UnityEngine.UI;
using ProtoTable;
using UnityEngine;

namespace GameClient
{
    public class CommonMsgBoxCancelOKParams
    {
        public bool closeFrameOnCancelBtnCDEnd;
        public bool showCancelBtnGrayOnCDEnd;

        public CommonMsgBoxCancelOKParams()
        {
            ResetData();
        }

        public void ResetData()
        {
            closeFrameOnCancelBtnCDEnd = true;
            showCancelBtnGrayOnCDEnd = false;
        }
    }

    public class CommonMsgBoxCancelOK : ClientFrame
    {
        bool bIsCountDownTimeOKBtn = false;
        bool bOpenCountDownFunc = false;
        float fCountDownTime = 0.0f;
        float fAddUpTime = 0.0f;
        string CancelBtnOriginText = "";
        string OkBtnOriginText = "";

        float updateThreshold = 0.0f;

        CommonMsgBoxCancelOKParams param = new CommonMsgBoxCancelOKParams();

        CommonMsgOutData outData = new CommonMsgOutData();

        public sealed override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/CommonSystemNotify/CommonMsgBoxCancelOK";
        }

        protected sealed override void _OnOpenFrame()
        {
            if (userData != null)
            {
                outData = userData as CommonMsgOutData;
            }
            
            BindUIEvent();

            if(CancelText != null)
            {
                CancelBtnOriginText = CancelText.text;
            }

            if (OKText != null)
            {
                OkBtnOriginText = OKText.text;
            }

            if(NewbieGuideManager.GetInstance().IsGuidingControl() && outData != null && outData.bExclusiveWithNewbieGuide)
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
            OkBtnOriginText = "";

            if (btOK != null)
            {
                btOK.onClick.RemoveAllListeners();
            }

            if(btCancel != null)
            {
                btCancel.onClick.RemoveAllListeners();
            }         

            if(btOKmini != null)
            {
                btOKmini.onClick.RemoveAllListeners();
            }
           
            if(btCancelmini != null)
            {
                btCancelmini.onClick.RemoveAllListeners();
            } 
            
            if(outData != null)
            {
                outData.ClearData();
            }
			
			if(param != null)
			{
				param.ResetData();
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
            if (msgText == null)
            {
                return;
            }

            Canvas.ForceUpdateCanvases();
            if (msgText.cachedTextGenerator.lineCount <= 1)
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
           
            if(msgText != null)
            {
                msgText.text = str;
            }
           
            if(msgTextmini != null)
            {
                msgTextmini.text = str;
            }

            AdaptUIForMsgContent();
        }

        public void SetCancelBtnText(string str)
        {
            if(CancelText != null)
            {
                CancelText.text = str;
            }
            
            if(CancelTextmini != null)
            {
                CancelTextmini.text = str;
            }
            //重新缓存
            CancelBtnOriginText = str;
        }

        public void SetOkBtnText(string str)
        {
            if(OKText != null)
            {
                OKText.text = str;
            }
          
            if(OKTextmini != null)
            {
                OKTextmini.text = str;
            }

            OkBtnOriginText = str;
        }

        public void SetNotifyDataByTable(CommonTipsDesc NotifyData)
        {
            if (NotifyData != null)
            {
                if (NotifyData.ButtonText != "" && NotifyData.ButtonText != "-" && NotifyData.ButtonText != "0")
                {
                    SetOkBtnText(NotifyData.ButtonText);
                    OkBtnOriginText = NotifyData.ButtonText;
                }

                if (NotifyData.CancelBtnText != "" && NotifyData.CancelBtnText != "-" && NotifyData.CancelBtnText != "0")
                {
                    SetCancelBtnText(NotifyData.CancelBtnText);
                    CancelBtnOriginText = NotifyData.CancelBtnText;
                }
            }
        }

        public void SetCountDownTime(float fTime, bool bIsCountDownTimeOKBtn = false)
        {
            if(fTime > 0.0f)
            {
                this.bIsCountDownTimeOKBtn = bIsCountDownTimeOKBtn;

                if (!this.bIsCountDownTimeOKBtn)
                {
                    //这么处理 只是为了保证改动以前的倒计时 阈值保持不变！
                    if (!_NeedCloseFrameOnCDEnd())
                    {
                        updateThreshold = 0f;
                    }

                    if (_NeedShowCancelBtnGray())
                    {
                        _SetCancelButtonEnable(false);
                    }
                }
                else
                {
                    _SetOkButtonEnabe(false);
                }

                fCountDownTime = fTime;
                bOpenCountDownFunc = true;
            }         
        }

        public void InitMsgBox(CommonMsgBoxCancelOKParams outParam)
        {
            if (outParam == null)
            {
                return;
            }
            this.param = outParam;
        }

        public void AddListener(UnityEngine.Events.UnityAction OnOKCallBack, UnityEngine.Events.UnityAction OnCancelCallBack)
        {
            if (OnOKCallBack != null)
            {
                if(btOK != null)
                {
                    btOK.onClick.AddListener(OnOKCallBack);
                }

                if(btOKmini != null)
                {
                    btOKmini.onClick.AddListener(OnOKCallBack);
                }  
            }

            if(OnCancelCallBack != null)
            {
                if(btCancel != null)
                {
                    btCancel.onClick.AddListener(OnCancelCallBack);
                }
                
                if(btCancelmini != null)
                {
                    btCancelmini.onClick.AddListener(OnCancelCallBack);
                }
            }
        }

        [UIEventHandle("normal/Back/Panel/btOK")]
        void OnClickOK()
        {
            frameMgr.CloseFrame(this);
        }

        [UIEventHandle("normal/Back/Panel/btCancel")]
        void OnClickCancel()
        {
            frameMgr.CloseFrame(this);
        }

        [UIEventHandle("mini/Back/Panel/btOK")]
        void OnClickOKmini()
        {
            frameMgr.CloseFrame(this);
        }

        [UIEventHandle("mini/Back/Panel/btCancel")]
        void OnClickCancelmini()
        {
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

                if (!bIsCountDownTimeOKBtn)
                {
                    if (CancelText != null)
                    {
                        CancelText.text = string.Format("{0}({1}s)", CancelBtnOriginText, iInt);
                    }

                    if (CancelTextmini != null)
                    {
                        CancelTextmini.text = string.Format("{0}({1}s)", CancelBtnOriginText, iInt);
                    }
                }
                else
                {
                    if (OKText != null)
                    {
                        OKText.text = string.Format("{0}({1}s)", OkBtnOriginText, iInt);
                    }

                    if (OKTextmini != null)
                    {
                        OKTextmini.text = string.Format("{0}({1}s)", OkBtnOriginText, iInt);
                    }
                }
                
                fAddUpTime += timeElapsed;

                if(fAddUpTime > 1.0f)
                {
                    fCountDownTime -= 1.0f;
                    fAddUpTime = 0.0f;
                }

                if (!bIsCountDownTimeOKBtn)
                {
                    if (iInt < updateThreshold)
                    {
                        bOpenCountDownFunc = false;

                        if (btCancel != null && btCancel.IsActive())
                        {
                            btCancel.onClick.Invoke();
                        }

                        if (btCancelmini != null && btCancelmini.IsActive())
                        {
                            btCancelmini.onClick.Invoke();
                        }

                        if (_NeedShowCancelBtnGray())
                        {
                            CancelText.text = CancelBtnOriginText;
                            CancelTextmini.text = CancelBtnOriginText;
                            _SetCancelButtonEnable(true);
                        }

                        //CD结束 是否需要关闭界面
                        if (_NeedCloseFrameOnCDEnd())
                        {
                            frameMgr.CloseFrame(this);
                        }
                    }
                    
                }
                else
                {
                    if (iInt <= 0)
                    {
                        bOpenCountDownFunc = false;

                        if (OKText != null)
                        {
                            OKText.text = OkBtnOriginText;
                        }

                        if (OKTextmini != null)
                        {
                            OKTextmini.text = OkBtnOriginText;
                        }

                        _SetOkButtonEnabe(true);
                    }
                }
            }
        }

        private void _SetCancelButtonEnable(bool bEnable)
        {
            if (_NeedShowCancelBtnGray())
            {
                btCancel.enabled = bEnable;
                btCancelGray.enabled = !bEnable;

                btCancelmini.enabled = bEnable;
                btCancelminiGray.enabled = !bEnable;
            }
        }

        private void _SetOkButtonEnabe(bool bEnable)
        {
            if (btOK != null)
            {
                btOK.enabled = bEnable;
            }

            if (btOKGray != null)
            {
                btOKGray.enabled = !bEnable;
            }

            if (btOKmini != null)
            {
                btOKmini.enabled = bEnable;
            }

            if (btOKminiGray != null)
            {
                btOKminiGray.enabled = !bEnable;
            }
        }

        private bool _NeedCloseFrameOnCDEnd()
        {
            if (param == null)
            {
                return false;
            }
            return param.closeFrameOnCancelBtnCDEnd;
        }

        private bool _NeedShowCancelBtnGray()
        {
            if (param == null)
            {
                return false;
            }
            return param.showCancelBtnGrayOnCDEnd;
        }

        [UIControl("normal/Back/TextPanel/AlertText")]
        protected Text msgText;

        [UIControl("normal/Back/Panel/btOK/Text")]
        protected Text OKText;

        [UIControl("normal/Back/Panel/btCancel/Text")]
        protected Text CancelText;

        [UIControl("normal/Back/Panel/btOK")]
        protected Button btOK;

        [UIControl("normal/Back/Panel/btOK")]
        protected UIGray btOKGray;

        [UIControl("normal/Back/Panel/btCancel")]
        protected Button btCancel;

        [UIControl("normal/Back/Panel/btCancel")]
        protected UIGray btCancelGray;

        [UIControl("mini/Back/TextPanel/AlertText")]
        protected Text msgTextmini;

        [UIControl("mini/Back/Panel/btOK/Text")]
        protected Text OKTextmini;

        [UIControl("mini/Back/Panel/btCancel/Text")]
        protected Text CancelTextmini;

        [UIControl("mini/Back/Panel/btOK")]
        protected Button btOKmini;

        [UIControl("mini/Back/Panel/btOK")]
        protected UIGray btOKminiGray;

        [UIControl("mini/Back/Panel/btCancel")]
        protected Button btCancelmini;

        [UIControl("mini/Back/Panel/btCancel")]
        protected UIGray btCancelminiGray;

        [UIObject("normal")]
        GameObject m_goObjnormal;

        [UIObject("mini")]
        GameObject m_goObjmini;
    }
}
