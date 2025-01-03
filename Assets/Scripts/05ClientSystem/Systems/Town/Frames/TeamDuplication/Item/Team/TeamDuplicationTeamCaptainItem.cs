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

    //团本中小队的Item
    public class TeamDuplicationTeamCaptainItem : MonoBehaviour
    {

        //小队索引的地址
        private const string captainIndexPath = "UI/Image/Packed/p_UI_Tuanben.png:UI_Tuanben_Shuzi_{0}";
        //待机
        private const string captainStatusIdle = "UI/Image/Packed/p_UI_Tuanben.png:UI_Tuanben_Daiji_Zi";
        //准备
        private const string captainStatusPrepare = "UI/Image/Packed/p_UI_Tuanben.png:UI_Tuanben_Beizhan_Zi";
        //战斗
        private const string captainStatusFighting = "UI/Image/Packed/p_UI_Tuanben.png:UI_Tuanben_Zhandou_Zi";

        private TeamDuplicationCaptainDataModel _captainDataModel;

        [Space(10)] [HeaderAttribute("Text")] [Space(5)] [SerializeField]
        private Image captainIndexImage;
        [SerializeField] private Image captainStatusImage;

        [Space(10)]
        [HeaderAttribute("PlayerItem")]
        [Space(5)]
        [SerializeField] private TeamDuplicationTeamCaptainPlayerItem firstPlayerItem;
        [SerializeField] private TeamDuplicationTeamCaptainPlayerItem secondPlayerItem;
        [SerializeField] private TeamDuplicationTeamCaptainPlayerItem thirdPlayerItem;

        private void Awake()
        {

        }

        private void OnEnable()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationCaptainNotifyMessage,
                OnReceiveTeamDuplicationCaptainNotifyMessage);
        }

        private void OnDisable()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationCaptainNotifyMessage,
                OnReceiveTeamDuplicationCaptainNotifyMessage);
        }

        private void OnDestroy()
        {
            _captainDataModel = null;
        }

        public void Init(TeamDuplicationCaptainDataModel captainDataModel)
        {
            _captainDataModel = captainDataModel;

            if (_captainDataModel == null)
            {
                Logger.LogErrorFormat("TeamDuplicationTroopTotalItem Init troopDataModel is null");
                return;
            }
            InitItem();
        }

        private void InitItem()
        {
            InitCaptainImage();

            UpdateCaptainStatus();

            UpdateTeamCaptainPlayerItem();
        }

        private void InitCaptainImage()
        {
            if (captainIndexImage == null)
                return;

            var curCaptainIndexPath = string.Format(captainIndexPath, _captainDataModel.CaptainId);

            ETCImageLoader.LoadSprite(ref captainIndexImage, curCaptainIndexPath);
        }

        private void UpdateCaptainStatus()
        {
            if (captainStatusImage == null)
                return;

            var statusPath = captainStatusIdle;
            var captainStatus = (TeamCopySquadStatus) _captainDataModel.CaptainStatus;
            if (captainStatus == TeamCopySquadStatus.TEAM_COPY_SQUAD_STATUS_PREPARE)
            {
                statusPath = captainStatusPrepare;
            }
            else if (captainStatus == TeamCopySquadStatus.TEAM_COPY_SQUAD_STATUS_BATTLE)
            {
                statusPath = captainStatusFighting;
            }

            ETCImageLoader.LoadSprite(ref captainStatusImage, statusPath);
        }

        private void UpdateTeamCaptainPlayerItem()
        {
            if (firstPlayerItem != null)
            {
                if (_captainDataModel.PlayerList != null && _captainDataModel.PlayerList.Count > 0)
                {
                    firstPlayerItem.Init(_captainDataModel.PlayerList[0]);
                }
                else
                {
                    firstPlayerItem.Init(null);
                }
            }

            if (secondPlayerItem != null)
            {
                if (_captainDataModel.PlayerList != null && _captainDataModel.PlayerList.Count > 1)
                {
                    secondPlayerItem.Init(_captainDataModel.PlayerList[1]);
                }
                else
                {
                    secondPlayerItem.Init(null);
                }
            }

            if (thirdPlayerItem != null)
            {
                if (_captainDataModel.PlayerList != null && _captainDataModel.PlayerList.Count > 2)
                {
                    thirdPlayerItem.Init(_captainDataModel.PlayerList[2]);
                }
                else
                {
                    thirdPlayerItem.Init(null);
                }
            }

        }

        #region UIEvent
        private void OnReceiveTeamDuplicationCaptainNotifyMessage(UIEvent uiEvent)
        {
            if (_captainDataModel == null)
                return;

            if (uiEvent == null || uiEvent.Param1 == null)
                return;

            var captainId = (int)uiEvent.Param1;
            if (_captainDataModel.CaptainId != captainId)
                return;

            //更新队伍的状态
            UpdateCaptainStatus();
        }

        #endregion 

    }
}
