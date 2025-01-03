using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class SevenDaysTargetTabItem : MonoBehaviour
    {
        [SerializeField] private CanvasGroup mCanvasNormal;
        [SerializeField] private CanvasGroup mCanvasCheck;
        [SerializeField] private CanvasGroup mCanvasLock;
        [SerializeField] private CanvasGroup mCanvasRedPoint;
        [SerializeField] private TextEx mTextNormal;
        [SerializeField] private TextEx mTextCheck;

        public void Init(bool isCheck, bool isLock, int index, bool isShowRedPoint)
        {
            mCanvasNormal.CustomActive(!isCheck);
            mCanvasCheck.CustomActive(isCheck);
            mCanvasLock.CustomActive(isLock);
            if (isCheck)
            {
                mTextCheck.SafeSetText(string.Format("第{0}天", index));
            }
            else
            {
                mTextNormal.SafeSetText(string.Format("第{0}天", index));
            }

            mCanvasRedPoint.CustomActive(isShowRedPoint);
        }
    }
}
