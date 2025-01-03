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

    //BasePlayerItem
    public class TeamDuplicationBasePlayerItem : MonoBehaviour
    {

        protected TeamDuplicationPlayerDataModel PlayerDataModel = null;

        [Space(15)]
        [HeaderAttribute("HeadIcon")]
        [Space(5)]
        [SerializeField] private Image headIcon;

        [SerializeField] private Image headFrameIcon;

        protected virtual void Awake()
        {

        }

        protected virtual void OnDestroy()
        {
            PlayerDataModel = null;
        }

        public virtual void Init(TeamDuplicationPlayerDataModel playerDataModel)
        {
            PlayerDataModel = playerDataModel;

            InitHead();
        }

        private void InitHead()
        {
            if (PlayerDataModel == null || PlayerDataModel.Guid == 0
                                         || PlayerDataModel.ProfessionId == 0)
            {
                ResetHead();
                return;
            }

            string headIconPath = "";
            var jobData = TableManager.GetInstance().GetTableItem<JobTable>(PlayerDataModel.ProfessionId);
            if (jobData != null)
            {
                var resData = TableManager.GetInstance().GetTableItem<ResTable>(jobData.Mode);
                if (resData != null)
                    headIconPath = resData.IconPath;
            }

            if (headIcon != null && string.IsNullOrEmpty(headIconPath) == false)
            {
                ETCImageLoader.LoadSprite(ref headIcon, headIconPath);
                CommonUtility.UpdateImageVisible(headIcon, true);
            }

            string headFramePath = HeadPortraitFrameDataManager.GetHeadPortraitFramePath(
                PlayerDataModel.HeadFrameId);
            if (headFrameIcon != null && string.IsNullOrEmpty(headFramePath) == false)
            {
                ETCImageLoader.LoadSprite(ref headFrameIcon, headFramePath);
            }
        }

        //重置头像
        private void ResetHead()
        {
            if (headIcon != null)
            {
                headIcon.sprite = null;
                headIcon.material = null;
            }
            CommonUtility.UpdateImageVisible(headIcon, false);
        }

        //得到角色的Guid
        public ulong GetPlayerGuid()
        {
            if (PlayerDataModel == null)
                return 0;

            return PlayerDataModel.Guid;
        }

    }
}
