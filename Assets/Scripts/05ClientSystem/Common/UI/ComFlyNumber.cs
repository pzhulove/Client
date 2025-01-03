using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

namespace GameClient
{
    public class ComFlyNumber : MonoBehaviour 
    {
        public float mDelayTime = 0.0f;
        public float mTime      = 0.5f;

        public Text mNumber;

        private int mLastNumber = 0;

        private void _updateNumber(int num)
        {
            if (null != mNumber)
            {
                mNumber.text = string.Format("{0}", num);
            }
        }

        public void SetNumber(int finalnum)
        {
            var dt = DOTween.To(
                    () => mLastNumber,
                    r => _updateNumber(r),
                    finalnum,
                    mTime);
            dt.SetDelay(mDelayTime);
        }
    }
}
