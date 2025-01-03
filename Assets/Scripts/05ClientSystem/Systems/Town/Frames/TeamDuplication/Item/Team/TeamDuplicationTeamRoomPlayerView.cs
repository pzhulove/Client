using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ProtoTable;
using Protocol;
using Scripts.UI;
using UnityEngine.EventSystems;

namespace GameClient
{

    //角色的具体View
    public class TeamDuplicationTeamRoomPlayerView : TeamDuplicationBasePlayerItem
    {
        private bool _isOtherTeam = false;          //其他队伍的详情信息

        [Space(25)]
        [HeaderAttribute("GrayPlayer")]
        [Space(10)]
        [SerializeField] private UIGray playerUIGray;
        [SerializeField] private CountDownTimeController playerGrayCountDownTimeController;

        [Space(15)]
        [HeaderAttribute("OwnerPlayerRoot")]
        [Space(5)]
        [SerializeField] private GameObject teamLeaderRoot;
        [SerializeField] private GameObject captainerRoot;
        [SerializeField] private GameObject goldRoot;
        [SerializeField] private Button playerItemButton;

        [Space(15)]
        [HeaderAttribute("Detail")]
        [Space(5)]
        [SerializeField] private Text playerNameText;
        [SerializeField] private Text playerEquipmentScoreText;
        [SerializeField] private Text playerProfessionText;
        [SerializeField] private Text playerLevelText;

        [Space(15)]
        [HeaderAttribute("Ticket")]
        [Space(5)]
        [SerializeField] private GameObject notEnoughTicketRoot;

        protected override void Awake()
        {
            BindUiEvents();

            base.Awake();
        }

        protected override void OnDestroy()
        {
            UnBindUiEvents();
            ClearData();

            base.OnDestroy();
        }

        private void ClearData()
        {
            _isOtherTeam = false;
        }

        private void BindUiEvents()
        {
            if (playerItemButton != null)
            {
                playerItemButton.onClick.RemoveAllListeners();
                playerItemButton.onClick.AddListener(OnPlayerItemButtonClick);
            }
        }

        private void UnBindUiEvents()
        {
            if (playerItemButton != null)
                playerItemButton.onClick.RemoveAllListeners();
        }

        //playerDataModel不为null
        public void InitItem(TeamDuplicationPlayerDataModel playerDataModel,
            bool isOtherTeam = false)
        {
            _isOtherTeam = isOtherTeam;

            base.Init(playerDataModel);

            InitTeamRoomPlayerView();
        }

        private void InitTeamRoomPlayerView()
        {
            UpdatePlayerTypeFlag();

            UpdatePlayerDetailInfo();

            UpdatePlayerTicketInfo();

            InitPlayerGrayCover();
        }

        //显示角色的GrayCover
        private void InitPlayerGrayCover()
        {
            if (PlayerDataModel == null)
            {
                ResetPlayerGrayView();
                return;
            }

            //过期时间
            var playerExpireTime = PlayerDataModel.ExpireTime;
            UpdatePlayerGrayView(playerExpireTime);
        }

        private void ResetPlayerTypeFlag()
        {
            CommonUtility.UpdateGameObjectVisible(teamLeaderRoot, false);
            CommonUtility.UpdateGameObjectVisible(captainerRoot, false);
            CommonUtility.UpdateGameObjectVisible(goldRoot, false);
        }

        private void UpdatePlayerTypeFlag()
        {
            ResetPlayerTypeFlag();

            if (PlayerDataModel.IsTeamLeader == true)
            {
                //团长
                CommonUtility.UpdateGameObjectVisible(teamLeaderRoot, true);
            }
            else
            {
                //金主
                if (PlayerDataModel.IsGoldOwner == true)
                {
                    CommonUtility.UpdateGameObjectVisible(goldRoot, true);
                }
                else
                {
                    //队长
                    if (PlayerDataModel.IsCaptain == true)
                    {
                        CommonUtility.UpdateGameObjectVisible(captainerRoot, true);
                    }
                }
            }
        }

        private void UpdatePlayerDetailInfo()
        {
            if (playerNameText != null)
                playerNameText.text = PlayerDataModel.Name;

            if (playerEquipmentScoreText != null)
                playerEquipmentScoreText.text = PlayerDataModel.EquipmentScore.ToString();

            if (playerProfessionText != null)
                playerProfessionText.text = TeamDuplicationUtility.GetJobName(PlayerDataModel.ProfessionId);

            if (playerLevelText != null)
                playerLevelText.text = string.Format(TR.Value("team_duplication_troop_room_player_level"),
                    PlayerDataModel.Level);
        }

        public void UpdatePlayerTicketInfo()
        {
            if (notEnoughTicketRoot == null)
                return;

            if (_isOtherTeam == true)
            {
                //其他团队
                CommonUtility.UpdateGameObjectVisible(notEnoughTicketRoot, false);
            }
            else
            {
                //战斗开始之后 不显示门票不足的标志
                if (TeamDuplicationUtility.IsTeamDuplicationFightStart() == true)
                {
                    CommonUtility.UpdateGameObjectVisible(notEnoughTicketRoot, false);
                }
                else
                {
                    //战斗开始之前，根据门票的数量展示门票不足
                    CommonUtility.UpdateGameObjectVisible(notEnoughTicketRoot, !PlayerDataModel.TicketIsEnough);
                }
            }
        }

        #region PlayerExpireTime

        //更新GrayCoverView
        public void UpdatePlayerGrayView(ulong expireTime)
        {
            if (expireTime == 0)
            {
                ResetPlayerGrayView();
            }
            else
            {
                //处在离线状态
                CommonUtility.UpdateGameObjectUiGray(playerUIGray, true);

                //其他队伍详情，不显示倒计时
                if (_isOtherTeam == true)
                {
                    UpdatePlayerGrayCountDownTimeController(false);
                }
                else
                {
                    //自己队伍，战斗开始前不显示倒计时，战斗开始后才显示倒计时
                    if (TeamDuplicationUtility.IsTeamDuplicationFightStart() == false)
                    {
                        UpdatePlayerGrayCountDownTimeController(false);
                    }
                    else
                    {
                        UpdatePlayerGrayCountDownTimeController(true, expireTime);
                    }
                }
            }
        }

        //更新离线倒计时
        private void UpdatePlayerGrayCountDownTimeController(bool flag, ulong expireTime = 0)
        {
            if (playerGrayCountDownTimeController == null)
                return;

            if (flag == false)
            {
                playerGrayCountDownTimeController.ResetCountDownTimeController();
                CommonUtility.UpdateGameObjectVisible(playerGrayCountDownTimeController.gameObject, false);
            }
            else
            {
                CommonUtility.UpdateGameObjectVisible(playerGrayCountDownTimeController.gameObject, true);
                playerGrayCountDownTimeController.EndTime = (uint)expireTime;
                playerGrayCountDownTimeController.InitCountDownTimeController();
            }
        }

        //重置离线信息
        public void ResetPlayerGrayView()
        {
            CommonUtility.UpdateGameObjectUiGray(playerUIGray, false);
            UpdatePlayerGrayCountDownTimeController(false);
        }

        #endregion

        #region ButtonClick
        private void OnPlayerItemButtonClick()
        {
            if (PlayerDataModel == null || PlayerDataModel.Guid == 0)
            {
                Logger.LogErrorFormat("OnPlayerItemButtonClick PlayerDataModel is Error");
                return;
            }

            //其他队伍.点击头像，显示用户详情
            if (_isOtherTeam == true)
            {
                OtherPlayerInfoManager.GetInstance().SendWatchOtherPlayerInfo(
                    PlayerDataModel.Guid,
                    (uint) QueryPlayerType.QPT_TEAM_COPY,
                    (uint) PlayerDataModel.ZoneId);
                return;
            }

            //自己，直接返回
            if (PlayerDataModel.Guid == PlayerBaseData.GetInstance().RoleID)
                return;

            //自己队伍的数据
            var ownerPlayerDataModel = TeamDuplicationUtility.GetOwnerPlayerDataModel();
            if (ownerPlayerDataModel == null || ownerPlayerDataModel.Guid == 0)
                return;

            //按钮的屏幕坐标
            Vector3 clickScreenPosition = Vector2.zero;
            var playerItemButtonRtf = playerItemButton.transform as RectTransform;

            //localPosition 局部坐标， position UI的全局坐标。由按钮的全局坐标获得屏幕坐标
            if (playerItemButtonRtf != null
                && ClientSystemManager.GetInstance().UICamera != null)
                clickScreenPosition = ClientSystemManager.GetInstance().UICamera.WorldToScreenPoint(
                    playerItemButtonRtf.position);

            TeamDuplicationUtility.OnOpenTeamDuplicationTeamPermissionFrame(ownerPlayerDataModel,
                PlayerDataModel,
                clickScreenPosition);

        }
        #endregion 

    }
}
