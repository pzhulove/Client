using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using UnityEngine.Assertions;
using Protocol;
using ProtoTable;
using System.Collections;
using DG.Tweening;
using System.IO;

namespace GameClient
{
    public class GetUltimateChallengeBufTipFrame : ClientFrame
    {       

        [UIObject("EffUI_gongxihuode_guangyun")]
        GameObject m_effGuanYun;

        [UIObject("EffUI_gongxihuode_xingguang")]
        GameObject m_effXingGuang;

        [UIControl("btnExit")]
        Button m_btnExit;  

        [UIObject("Desc")]
        GameObject goDesc = null;

        [UIObject("BG (3)")]
        GameObject goBg3 = null;

        ComBufItem bufItem0 = null;

        float fFrameCreateTime = Time.realtimeSinceStartup;
        const float fInterval1 = 0.2f;
        const float fInterval2 = 0.1f;        
        const float fDelayTime = 0.5f;
        float fTimeElapsed = 0.0f;
        float fTimeElapsedDelay = 0.0f;
        float fInterval = 0.0f;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Tip/GetUltimateChallengeBufTip";
        } 

        protected override void _OnOpenFrame()
        {
            BindUIEvent();

            fFrameCreateTime = Time.realtimeSinceStartup;
            fTimeElapsed = 0.0f;
            fTimeElapsedDelay = 0.0f;
 
            fInterval = fInterval1;        

            if (m_effXingGuang != null)
            {
                m_effXingGuang.CustomActive(false);
            }

            if (m_effGuanYun != null)
            {
                m_effGuanYun.CustomActive(false);
            }

            if(m_btnExit != null)
            {
                m_btnExit.onClick.RemoveAllListeners();
                m_btnExit.onClick.AddListener(() => 
                {
                    if(Time.realtimeSinceStartup - fFrameCreateTime < 1.0f)
                    {
                        return;
                    }

                    frameMgr.CloseFrame(this);     
                });
            }

            InvokeMethod.Invoke(this, fDelayTime, () => 
            {
                
            });

            bufItem0.CustomActive(false);
        }

        void _UpdateEffect()
        {
            if (m_effXingGuang != null)
            {
                m_effXingGuang.CustomActive(true);
            }

            if (m_effGuanYun != null)
            {
                m_effGuanYun.CustomActive(true);
            }

            if (bufItem0 != null)
            {
                bufItem0.SetBufData(ActivityDataManager.GetInstance().GetUltimateChallengeInspireBufID(), ActivityDataManager.GetInstance().GetUltimateChallengeInspireBufLv());
                bufItem0.CustomActive(true);
            }
            return;
        }

        protected override void _OnCloseFrame()
        {   
            InvokeMethod.RemoveInvokeCall(this);           

            UnBindUIEvent();

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.RefreshInspireBufSuccess);
        }

        public override bool IsNeedUpdate()
        {
            return true;
        }

        protected override void _OnUpdate(float timeElapsed)
        {
            fTimeElapsedDelay += timeElapsed;
            fTimeElapsed += timeElapsed;   

            if(fTimeElapsedDelay >= fDelayTime)
            {
                if (fTimeElapsed >= fInterval)
                {
                    fTimeElapsed = 0.0f;
                    _UpdateEffect();
                }
            }       

            if (goDesc != null && goBg3 != null)
            {
                Vector3 vecPos1 = goDesc.transform.localPosition;
                Vector3 vecPos2 = goBg3.transform.localPosition;

                //vecPos1 = vecPos2;
                vecPos1.y = vecPos2.y - goBg3.transform.GetComponent<RectTransform>().sizeDelta.y - 40;

                goDesc.transform.localPosition = vecPos1;
            }           
        }

        protected override void _bindExUI()
        {
            bufItem0 = mBind.GetCom<ComBufItem>("bufItem0");
        }

        protected override void _unbindExUI()
        {
            bufItem0 = null;
        }


        #region set sibling on specialframe

        void BindUIEvent()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.FirstPayFrameOpen, _OnFirstPayFrameOpen);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.SecondPayFrameOpen, _OnSecondPayFrameOpen);
        }

        void UnBindUIEvent()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.FirstPayFrameOpen, _OnFirstPayFrameOpen);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.SecondPayFrameOpen, _OnSecondPayFrameOpen);
        }

        void _OnFirstPayFrameOpen(UIEvent uiEvent)
        {
            var firstPayFrame = ClientSystemManager.GetInstance().GetFrame(typeof(FirstPayFrame)) as FirstPayFrame;
            if (firstPayFrame == null)
            {
                return;
            }
            var firstPayframeObj = firstPayFrame.GetFrame();
            if (firstPayFrame != null && firstPayframeObj != null)
            {
                this.frame.transform.SetSiblingIndex(firstPayframeObj.transform.GetSiblingIndex() + 1);
            }
        }
        void _OnSecondPayFrameOpen(UIEvent uiEvent)
        {
            var secondPayFrame = ClientSystemManager.GetInstance().GetFrame(typeof(SecondPayFrame)) as SecondPayFrame;
            if (secondPayFrame == null)
            {
                return;
            }
            var secondPayframeObj = secondPayFrame.GetFrame();
            if (secondPayFrame != null && secondPayframeObj != null)
            {
                this.frame.transform.SetSiblingIndex(secondPayframeObj.transform.GetSiblingIndex() + 1);
            }
        }

        #endregion
    }
}
