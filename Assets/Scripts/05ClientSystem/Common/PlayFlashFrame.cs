using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using UnityEngine;

namespace GameClient
{
    class PlayFlashFrame : ClientFrame
    {
        enum ePlayFlash
        {
            None,
            Playing
        }

        private const string kPlayFlashFrameKey = "kPlayFlashFrameKey";

        //[UIControl("Comic", typeof(Animator))]
        //private Animator mAnimator;

        private ePlayFlash mFlashState;
        private float mDelayColseTime;

        public override bool IsNeedUpdate()
        {
            return true;
        }

        protected override void _OnOpenFrame()
        {
            if (_canPlay())
            {
                mFlashState = ePlayFlash.Playing;
                mDelayColseTime = _getTime();
                _save();
            }
            else
            {
                Close();
            }
        }

        private string _getKey()
        {
            var str = PlayerLocalSetting.GetValue("AccountDefault");
            return string.Format("{0}{1}", str, kPlayFlashFrameKey);
        }

        private bool _canPlay()
        {
            //var obj = PlayerLocalSetting.GetValue(_getKey());
            //if (obj != null)
            //{
            //    return (bool)obj;
            //}

            return true;
        }

        private void _save()
        {
            //PlayerLocalSetting.SetValue(_getKey(), false);
            //PlayerLocalSetting.SaveConfig();
        }

        protected override void _OnCloseFrame()
        {
            mFlashState = ePlayFlash.None;
            mDelayColseTime = 0.0f;
        }

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Comic/Comic1";
        }

        private float _getTime()
        {
            return 0.0f;
            //var infos = mAnimator.GetCurrentAnimatorClipInfo(0);
            //var t = 0.0f;
            //foreach (var item in infos)
            //{
            //    t += item.clip.length;
            //}
            //return t;
        }

        [UIEventHandle("Skip")]
        private void _onSkip()
        {
            mFlashState = ePlayFlash.None;
            frameMgr.CloseFrame(this);
        }

        //protected override void _OnUpdate(float timeElapsed)
        //{
        //    if (mFlashState == ePlayFlash.Playing)
        //    {
        //        if (mDelayColseTime >= 0.0f)
        //        {
        //            mDelayColseTime -= timeElapsed;
        //        }
        //        else
        //        {
        //            Close();
        //        }
        //    }
        //}
    }
}
