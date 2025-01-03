using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public interface ICommonTabToggle
    {
        void Init(CommonTabData tabData, CommonTabToggleOnClick onClick, bool isSelected, int index, int length);
        Toggle GetTog();
        void OnSetRedPoint(bool isFlag);
    }

    public class CommonTabToggleItem : MonoBehaviour, ICommonTabToggle
    {
        [SerializeField] private Text normalTabName;
        [SerializeField] private Text checkTabName;
        [SerializeField] private Image normalTag;
        [SerializeField] private Image checkTag;
        [SerializeField] private Toggle tabTog;
        [SerializeField] private GameObject redPoint;
        [SerializeField] private GameObject lineGo;

        private CommonTabData commonTabData;
        private CommonTabToggleOnClick commonTabToggleOnClick;
        private bool bIsSelected = false;

        private void Awake()
        {
            if (tabTog != null)
            {
                tabTog.onValueChanged.RemoveAllListeners();
                tabTog.onValueChanged.AddListener(OnTabClick);
            }
        }

        private void OnDestroy()
        {
            if (tabTog != null)
            {
                tabTog.onValueChanged.RemoveListener(OnTabClick);
            }

            commonTabData = null;
            commonTabToggleOnClick = null;
            bIsSelected = false;
        }

        public void Init(CommonTabData tabData, CommonTabToggleOnClick onClick, bool isSelected,int index,int length)
        {
            if (tabData == null) return;

            commonTabData = tabData;
            commonTabToggleOnClick = onClick;

            if(normalTabName != null && checkTabName != null)
            {
                normalTabName.text = checkTabName.text = commonTabData.tabName;
            }

            if(commonTabData.normalIconPath != string.Empty)
            {
                if (normalTag != null)
                {
                    normalTag.CustomActive(true);

                    ETCImageLoader.LoadSprite(ref normalTag, commonTabData.normalIconPath);
                }
            }
            else
            {
                if (normalTag != null)
                {
                    normalTag.CustomActive(false);
                }
            }

            if (commonTabData.checkIconPath != string.Empty)
            {
                if (checkTag != null)
                {
                    checkTag.CustomActive(true);

                    ETCImageLoader.LoadSprite(ref checkTag, commonTabData.checkIconPath);
                }
            }
            else
            {
                if (checkTag != null)
                {
                    checkTag.CustomActive(false);
                }
            }

            OnSetRedPoint(false);

            if(index == length - 1)
            {
                if(lineGo != null)
                {
                    lineGo.CustomActive(false);
                }
            }

            if(isSelected)
            {
                if(tabTog != null)
                {
                   tabTog.isOn = true;
                }
            }
        }

        public void OnSetRedPoint(bool isFlag)
        {
            if(redPoint != null)
            {
               redPoint.CustomActive(isFlag);
            }
        }

        private void OnTabClick(bool value)
        {
            if (commonTabData == null)
            {
                return;
            }

            //避免重复点击
            if (bIsSelected == value)
            {
                return;
            }

            bIsSelected = value;

            if(value == true)
            {
                if(commonTabToggleOnClick != null)
                {
                    commonTabToggleOnClick.Invoke(commonTabData);
                }
            }
        }
        

        public Toggle GetTog()
        {
            return tabTog;
        }
    }
}