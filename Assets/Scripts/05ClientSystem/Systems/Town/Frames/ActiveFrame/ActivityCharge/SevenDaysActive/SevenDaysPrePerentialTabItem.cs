using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class SevenDaysPrePerentialTabItem : MonoBehaviour
    {
        [SerializeField] private TextEx mTextNormal = null;
        [SerializeField] private TextEx mTextSelect = null;
        [SerializeField] private CanvasGroup mCanvasNormal = null;
        [SerializeField] private CanvasGroup mCanvasSelect = null;
        [SerializeField] private CanvasGroup mCanvasRedPoint = null;

        public void Init(string name, bool isSelected, bool isShowRedPoint)
        {
            mTextNormal.SafeSetText(name);
            mTextSelect.SafeSetText(name);
            mCanvasNormal.CustomActive(!isSelected);
            mCanvasSelect.CustomActive(isSelected);
            mCanvasRedPoint.CustomActive(isShowRedPoint);
        }
    }
}
