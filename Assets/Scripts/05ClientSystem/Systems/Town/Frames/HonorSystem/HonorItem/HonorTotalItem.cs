using System;
using System.Collections.Generic;
using Scripts.UI;
using UnityEngine;
using UnityEngine.UI;

namespace GameClient
{

    public class HonorTotalItem : MonoBehaviour
    {
        [Space(5)]
        [HeaderAttribute("Text")]
        [SerializeField] private Text activityTotalNumberText;
        [SerializeField] private Text notGetText;

        [Space(10)] [HeaderAttribute("Icon")] [Space(10)]
        [SerializeField] private GameObject newFlag;

        [SerializeField] private Image activityItemIcon;
        [SerializeField] private UIGray activityItemUIGray;

        private PvpNumberStatistics _pvpNumberStatistics;

        private void OnDestroy()
        {
            _pvpNumberStatistics = null;
        }
        

        public void InitItem(PvpNumberStatistics pvpNumberStatistics)
        {
            _pvpNumberStatistics = pvpNumberStatistics;

            if (_pvpNumberStatistics == null)
                return;
            
            InitActivityIcon();
            InitActivityText();
        }

        private void InitActivityIcon()
        {
            //新的标志
            CommonUtility.UpdateGameObjectVisible(newFlag,
                _pvpNumberStatistics.IsNewFlag);

            //todo
            //活动的图标
            if (activityItemIcon != null)
            {
                ETCImageLoader.LoadSprite(ref activityItemIcon, _pvpNumberStatistics.PvpIconPath);
            }

            bool uiGrayFlag = (_pvpNumberStatistics.PvpCount <= 0);
            CommonUtility.UpdateGameObjectUiGray(activityItemUIGray, uiGrayFlag);
            
        }

        private void InitActivityText()
        {
            //暂未获得
            if (_pvpNumberStatistics.PvpCount <= 0)
            {
                CommonUtility.UpdateTextVisible(activityTotalNumberText, false);
                CommonUtility.UpdateTextVisible(notGetText, true);
            }
            else
            {
                CommonUtility.UpdateTextVisible(activityTotalNumberText, true);
                if (activityTotalNumberText != null)
                    activityTotalNumberText.text = _pvpNumberStatistics.PvpCount.ToString();

                CommonUtility.UpdateTextVisible(notGetText, false);
            }
        }

    }
}