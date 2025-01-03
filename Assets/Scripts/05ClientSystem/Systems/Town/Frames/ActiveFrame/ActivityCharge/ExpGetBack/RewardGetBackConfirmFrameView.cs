using DG.Tweening;
using ProtoTable;
using Scripts.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameClient
{
    public class RewardGetBackConfirmFrameView : MonoBehaviour
    {
        private Action mOnConfrimClicked;
        private Action mOnCloseClicked;
        private Action<bool> mOnValueChanged;

        public void Init(Action onConfrimClick, Action onCloseClick, Action<bool> onValueChanged)
        {
            mOnConfrimClicked = onConfrimClick;
            mOnCloseClicked = onCloseClick;
            mOnValueChanged = onValueChanged;
        }

        public void OnValueChanged(bool value)
        {
            mOnValueChanged?.Invoke(value);
        }

        public void OnConfirmClick()
        {
            mOnConfrimClicked?.Invoke();
        }

        public void OnCloseClick()
        {
            mOnCloseClicked?.Invoke();
        }
    }
}
