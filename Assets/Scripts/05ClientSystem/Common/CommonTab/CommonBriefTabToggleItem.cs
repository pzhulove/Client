using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{
    public class CommonBriefTabToggleItem : MonoBehaviour
    {
        [SerializeField] private Text normalTabName;
        [SerializeField] private Text checkTabName;
        [SerializeField] private GameObject redPoint;
        [SerializeField] private GameObject normalTab;
        [SerializeField] private GameObject checkTab;

        private CommonBriefTabData commonTabData;
        private bool bIsSelected = false;

        private void Awake()
        {

        }

        private void OnDestroy()
        {
            commonTabData = null;
            bIsSelected = false;
        }

        public void Init(CommonBriefTabData tabData, bool isSelected,int index,int length, bool isShowRedPoint)
        {
            if (tabData == null) return;

            commonTabData = tabData;

            if(normalTabName != null && checkTabName != null)
            {
                normalTabName.text = checkTabName.text = commonTabData.tabName;
            }

            OnSetRedPoint(isShowRedPoint);
            SelectToggle(isSelected);
        }

        public void SelectToggle(bool isSelect)
        {
            normalTab.CustomActive(!isSelect);
            checkTab.CustomActive(isSelect);
        }

        public void OnSetRedPoint(bool isFlag)
        {
            if(redPoint != null)
            {
               redPoint.CustomActive(isFlag);
            }
        }
    }
}