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
    public class PackageExpandView : MonoBehaviour
    {
        [SerializeField] private Text mTextCost;
        [SerializeField] private Image mImageIcon;
        private Action mOnConfirmClick;
        private Action mOnCancelClick;

        public void Init(int cost, string icon, Action onConfrimClick, Action onCancelClick)
        {
            mOnConfirmClick = onConfrimClick;
            mOnCancelClick = onCancelClick;
            mTextCost.SafeSetText(cost.ToString());
            ETCImageLoader.LoadSprite(ref mImageIcon, icon);
        }

        public void OnConfirmClick()
        {
            mOnConfirmClick?.Invoke();
        }

        public void OnCloseClick()
        {
            mOnCancelClick?.Invoke();
        }

	}

}
