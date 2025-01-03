using System;
using System.Collections.Generic;
///////删除linq
using System.Text;

using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Protocol;
using Network;

namespace GameClient
{
    class SplashLoadingFrame : ClientFrame
    {
        protected bool _loadingDone = false;
        public bool fadeFinish
        {
            get { return _loadingDone; }
        }

        [UIControl("background")]
        Image _background;

        public override bool IsNeedUpdate()
        {
            return true;
        }

        protected override void _OnOpenFrame()
        {
            _loadingDone = false;
               Color col = _background.color;
            col.a = 0.0f;
            _background.color = col;
            m_state = EFrameState.FadeIn;
            //GameObject.DontDestroyOnLoad(frame);
        }

        protected override void _OnCloseFrame()
        {

        }

        protected override void _OnUpdate(float timeElapsed)
        {
            if (_background == null) return;

            Color col = _background.color;

            if (m_state == EFrameState.FadeIn)
            {
                col.a += 0.07f;
                if (col.a > 1.0f)
                {
                    col.a = 1.0f;
                }
                _background.color = col;

                if (_background.color.a >= 1.0f)
                {
                    m_state = EFrameState.Open;
                }
            }
            else if (m_state == EFrameState.Open)
            {
                float progress = ClientSystemManager.GetInstance().SwitchProgress;
                if ( progress >= 1.0f)
                {
                    col.a -= 0.1f;
                    if (col.a < 0.0f)
                    {
                        col.a = 0.0f;
                    }
                    _background.color = col;

                    if (_background.color.a <= 0.0f)
                    {
                        _loadingDone = true;
                        frameMgr.CloseFrame(this);
                    }
                }
            }
        }

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Common/SplashLoading";
        }
    }
}
