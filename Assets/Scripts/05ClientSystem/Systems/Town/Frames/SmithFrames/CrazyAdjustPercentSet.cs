using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using UnityEngine.Assertions;
using Protocol;
using ProtoTable;
using Network;
using Protocol;


namespace GameClient
{
    // 疯狂洗练设置百分比控制脚本
    public class CrazyAdjustPercentSet : MonoBehaviour
    {
        [SerializeField]
        Slider slider = null;

        [SerializeField]
        Text txtPercent = null;

        [SerializeField]
        Button btnSub = null;

        [SerializeField]
        Button btnAdd = null;

        [SerializeField]
        Text sliderValue = null;

        [SerializeField]
        Image fill1 = null;

        [SerializeField]
        Image fill2 = null;

        const int nMin = 60;
        const int nMax = 100;
        const int step = 10;

        [HideInInspector]
        public static int curTargetQuality = 0;

        [HideInInspector]
        public static int initTargetQuality = 0;

        // Use this for initialization
        void Start()
        {
            float fill1Width = 0;
            float fill2Width = 0;

            if(fill1 != null && fill2 != null)
            {
                fill1Width = fill1.rectTransform.sizeDelta.x;
                fill2Width = fill2.rectTransform.sizeDelta.x;
            }

            if(slider != null)
            {
                slider.SafeSetValueChangeListener((val) => 
                {
                    float value = ((int)val) / 10 * 10;  
                    txtPercent.SafeSetText(string.Format("{0}%", (int)value));
                    slider.value = value;
                    curTargetQuality = (int)slider.value;

                    sliderValue.SafeSetText(string.Format("{0}%", (int)value));

                    UpdateSubAndAddBtn();

                    const float min1 = 80;
                    const float max1 = 90;

                    const float min2 = 90;
                    const float max2 = 100;

                    if(fill1 != null && fill2 != null)
                    {
                        float percent1 = (value - min1) / (max1 - min1);
                        float percent2 = (value - min2) / (max2 - min2);
                        percent1 = Mathf.Clamp(percent1, 0.0f, 1.0f);
                        percent2 = Mathf.Clamp(percent2, 0.0f, 1.0f);

                        fill1.rectTransform.sizeDelta = new Vector2(percent1 * fill1Width, fill1.rectTransform.sizeDelta.y);
                        fill2.rectTransform.sizeDelta = new Vector2(percent2 * fill2Width, fill2.rectTransform.sizeDelta.y);
                    }
                });

                btnSub.SafeSetOnClickListener(() =>
                {
                    float value = slider.value;
                    value -= step;
                    if (value <= nMin)
                    {
                        value = nMin;                 
                    }
         
                    slider.value = value;
                });

                btnAdd.SafeSetOnClickListener(() => 
                {
                    float value = slider.value;
                    value += step;
                    if (value >= nMax)
                    {
                        value = nMax;                 
                    }
              
                    slider.value = value;
                });
            }

            SetPercent(nMax);
            SetPercent(initTargetQuality);
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void SetPercent(int percent)
        {
            if(slider == null)
            {
                return;
            }

            int value = (percent / 10) * 10;
            value = Math.Min(nMax, value);
            value = Math.Max(nMin, value);

            slider.value = value;      
        }

        public int GetPercent()
        {
            if(slider == null)
            {
                return 0;
            }

            return (int)slider.value;
        }

        void UpdateSubAndAddBtn()
        {
            if(slider == null)
            {
                return;
            }

            btnSub.SafeSetGray(false);
            btnAdd.SafeSetGray(false);

            int value = (int)slider.value;
            if(value <= nMin)
            {
                btnSub.SafeSetGray(true);
                btnAdd.SafeSetGray(false);
            }
            else if(value >= nMax)
            {
                btnSub.SafeSetGray(false);
                btnAdd.SafeSetGray(true);
            }
            else
            {
                btnSub.SafeSetGray(false);
                btnAdd.SafeSetGray(false);
            }
        }
    }
}