using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProtoTable;
using Protocol;
using Scripts.UI;

namespace GameClient
{
    
    //头像控制器
    public class TeamDuplicationHeadControl : MonoBehaviour
    {

        [Space(15)] [HeaderAttribute("HeadIcon")] [Space(5)]
        [SerializeField] private Image headIcon;

        [SerializeField] private Image headFrameIcon;

        [Space(15)] [HeaderAttribute("Text")] [Space(5)] [SerializeField]
        private Text nameText;

        [SerializeField] private Text professionText;

        private void Awake()
        {

        }

        private void OnEnable()
        {

        }

        private void OnDisable()
        {

        }

        private void OnDestroy()
        {

        }

        public void Init()
        {

            InitControlData();

            InitControl();
        }

        private void InitControlData()
        {

        }

        private void InitControl()
        {
            if (nameText != null)
                nameText.text = PlayerBaseData.GetInstance().Name;

            if (professionText != null)
                professionText.text = TeamDuplicationUtility.GetJobName(PlayerBaseData.GetInstance().JobTableID);
            
            InitHead();
        }

        private void InitHead()
        {
            string headIconPath = "";
            var jobData = TableManager.GetInstance().GetTableItem<JobTable>(PlayerBaseData.GetInstance().JobTableID);
            if (jobData != null)
            {
                var resData = TableManager.GetInstance().GetTableItem<ResTable>(jobData.Mode);
                if (resData != null)
                    headIconPath = resData.IconPath;
            }

            if (headIcon != null && string.IsNullOrEmpty(headIconPath) == false)
            {
                ETCImageLoader.LoadSprite(ref headIcon, headIconPath);
            }

            var headFrameId = HeadPortraitFrameDataManager.iDefaultHeadPortraitID;
            if (HeadPortraitFrameDataManager.WearHeadPortraitFrameID > 0)
            {
                headFrameId = HeadPortraitFrameDataManager.WearHeadPortraitFrameID;
            }

            string headFramePath = HeadPortraitFrameDataManager.GetHeadPortraitFramePath(headFrameId);

            if (headFrameIcon != null && string.IsNullOrEmpty(headFramePath) == false)
            {
                ETCImageLoader.LoadSprite(ref headFrameIcon, headFramePath);
            }
        }

    }
}
