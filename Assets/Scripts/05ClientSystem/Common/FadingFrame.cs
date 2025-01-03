using System;
using System.Collections.Generic;
///////删除linq
using System.Text;

namespace GameClient
{
    class FadingFrame : ClientFrame, ITownFadingFrame
    {
        ComFadeEffect m_fadeEffect;

        public int CurrentProgress
        {
            get { return -1; }
        }

        protected override void _OnOpenFrame()
        {
            m_fadeEffect = frame.GetComponent<ComFadeEffect>();
        }

        protected override void _OnCloseFrame()
        {
            m_fadeEffect = null;
        }

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Common/FadingFrame";
        }

        public void FadingIn(float time = 1.0f)
        {
            if (m_fadeEffect != null)
            {
                m_state = EFrameState.FadeIn;
                m_fadeEffect.FadeInTime = time;
                m_fadeEffect.OnFadeIn.AddListener(() => { m_state = EFrameState.Open; });
                m_fadeEffect.FadeIn();
            }
        }

        public bool IsOpened()
        {
            return m_state == EFrameState.Open;
        }

        public bool IsClosed()
        {
            return m_state == EFrameState.Close;
        }
        public void FadingOut(float time = 1.0f)
        {
            if (m_fadeEffect != null)
            {
                m_state = EFrameState.FadeOut;
                m_fadeEffect.FadeOutTime = time;
                m_fadeEffect.OnFadeOut.AddListener(() => 
                { 
                    frameMgr.CloseFrame(this);
                });
                m_fadeEffect.FadeOut();
            }
        }
    }
}
