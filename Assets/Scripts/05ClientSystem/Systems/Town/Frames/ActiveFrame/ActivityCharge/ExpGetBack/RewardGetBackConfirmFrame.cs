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
using Network;

namespace GameClient
{
    public class RewardGetBackConfirmFrame : ClientFrame
    {
        private RewardGetBackConfirmFrameView mView;
        private Action mConfirmCB;

        public override string GetPrefabPath()
        {
            return "UIFlatten/Prefabs/Activity/Activities/ExpGetBackConfirmFrame";
        }

        protected override void _OnOpenFrame()
        {
            mConfirmCB = userData as Action;
            if (mConfirmCB == null || frame == null)
            {
                Close();
            }
            else
            {
                mView = frame.GetComponent<RewardGetBackConfirmFrameView>();
                if (mView != null)
                {
                    mView.Init(_OnConfirmClick, _OnCloseClick, _OnValueChanged);
                }
                else
                {
                    Close();
                }
            }
        }

        private void _OnValueChanged(bool value)
        {
            ActiveManager.GetInstance().IsNotifyNormalGetBack = !value;
        }

        private void _OnCloseClick()
        {
            Close();
        }

        private void _OnConfirmClick()
        {
            mConfirmCB?.Invoke();
            Close();
        }


    }
}
