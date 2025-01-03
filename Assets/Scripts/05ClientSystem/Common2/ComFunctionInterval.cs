using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace GameClient
{
    public class ComFunctionInterval : MonoBehaviour
    {
        public Button button;
        public UIGray gray;
        public Text Express;
        public string formatString;
        public float fInterval = 1.0f;
        public List<GameObject> goDisabled = null;
        float fTickTime = 5.0f;

        void OnTickEnd()
        {
            EnableFunction();
        }

        void OnDestroy()
        {
            InvokeMethod.RemoveInvokeCall(this);
        }

        public void EnableFunction()
        {
            bool bEnable = true;
            if (gray != null)
            gray.enabled = !bEnable;
            if(button != null)
            button.enabled = bEnable;
            if(Express != null)
            {
                Express.enabled = !bEnable;
            }
            if(goDisabled != null)
            {
                for(int i = 0; i < goDisabled.Count; ++i)
                {
                    goDisabled[i].CustomActive(bEnable);
                }
            }
        }

        public void DisableFunction()
        {
            bool bEnable = false;
            if (gray != null)
                gray.enabled = !bEnable;
            if (button != null)
                button.enabled = bEnable;
            if (Express != null)
            {
                Express.enabled = !bEnable;
            }
            if (goDisabled != null)
            {
                for (int i = 0; i < goDisabled.Count; ++i)
                {
                    goDisabled[i].CustomActive(bEnable);
                }
            }
        }

        public void BeginInvoke(float fLastTime)
        {
            fTickTime = fLastTime;
            UpdateText();
            InvokeMethod.RemoveInvokeCall(this);
            InvokeMethod.Invoke(this, fLastTime, OnTickEnd);
            InvokeMethod.Invoke(this, fInterval, FunctionRepeating);
        }

        void FunctionRepeating()
        {
            fTickTime -= fInterval;
            fTickTime = Mathf.Max(0.0f, fTickTime);
            UpdateText();
            if(fTickTime > 0.0f)
            {
                InvokeMethod.Invoke(this, fInterval, FunctionRepeating);
            }
        }

        void UpdateText()
        {
            if (Express != null)
            {
                Express.text = string.Format(formatString, fTickTime);
            }
        }
    }
}