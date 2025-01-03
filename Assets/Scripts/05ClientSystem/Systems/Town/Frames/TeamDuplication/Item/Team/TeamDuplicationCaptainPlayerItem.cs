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

    //小队中的队员
    public class TeamDuplicationCaptainPlayerItem : MonoBehaviour
    {
        private TeamDuplicationPlayerDataModel _playerDataModel;
        private TeamDuplicationChatBubbleViewControl _chatBubbleViewControl;

        [Space(10)] [HeaderAttribute("TroopTotalItem")] [Space(5)]
        [SerializeField] private GameObject playerInfoRoot;
        [SerializeField] private Text playerNameText;
        [SerializeField] private Text playerProfessionText;
        [SerializeField] private GameObject noPlayerInfoRoot;
        [SerializeField] private TeamDuplicationBasePlayerItem basePlayerItem;

        [Space(25)] [HeaderAttribute("ChatContent")] [Space(10)]
        [SerializeField] private GameObject chatRoot;

        [Space(25)]
        [HeaderAttribute("GrayPlayer")]
        [Space(10)]
        [SerializeField] private UIGray playerUIGrayRoot;


        private void OnEnable()
        {
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationChatContentMessage,
                OnReceiveTeamDuplicationChatBubbleContentMessage);

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationPlayerExpireTimeMessage,
                OnReceiveTeamDuplicationPlayerExpireTimeMessage);
        }

        private void OnDisable()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationChatContentMessage,
                OnReceiveTeamDuplicationChatBubbleContentMessage);

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationPlayerExpireTimeMessage,
                OnReceiveTeamDuplicationPlayerExpireTimeMessage);

            //重置
            ResetChatBubbleViewControl();
        }

        private void OnDestroy()
        {
            _playerDataModel = null;
            _chatBubbleViewControl = null;
        }

        public void Init(TeamDuplicationPlayerDataModel playerDataModel)
        {
            _playerDataModel = playerDataModel;

            InitItem();
        }

        private void InitItem()
        {
            if (basePlayerItem != null)
            {
                basePlayerItem.Init(_playerDataModel);
            }

            UpdatePlayerNameAndProfession();

            InitPlayerGrayCover();

            //更新聊天气泡的内容
            UpdateChatBubbleViewControl();
        }

        //显示角色的GrayCover
        private void InitPlayerGrayCover()
        {
            if (_playerDataModel == null)
            {
                CommonUtility.UpdateGameObjectUiGray(playerUIGrayRoot, false);
                return;
            }

            var playerExpireTime = _playerDataModel.ExpireTime;
            UpdatePlayerGrayCover(playerExpireTime);
        }


        private void UpdatePlayerNameAndProfession()
        {
            if (_playerDataModel == null)
            {
                CommonUtility.UpdateGameObjectVisible(noPlayerInfoRoot, true);
                CommonUtility.UpdateGameObjectVisible(playerInfoRoot, false);
            }
            else
            {
                CommonUtility.UpdateGameObjectVisible(noPlayerInfoRoot, false);
                CommonUtility.UpdateGameObjectVisible(playerInfoRoot, true);

                if (_playerDataModel != null)
                {
                    if (playerNameText != null)
                        playerNameText.text = _playerDataModel.Name;

                    if (playerProfessionText != null)
                        playerProfessionText.text = TeamDuplicationUtility.GetJobName(_playerDataModel.ProfessionId);
                }
            }
        }

        #region ChatContent

        private void OnReceiveTeamDuplicationChatBubbleContentMessage(UIEvent uiEvent)
        {
            if (_playerDataModel == null)
                return;

            if (uiEvent == null || uiEvent.Param1 == null || uiEvent.Param2 == null)
                return;

            var guid = (ulong) uiEvent.Param1;
            if (guid != _playerDataModel.Guid)
                return;

            var chatContent = (string) uiEvent.Param2;

            ShowTeamDuplicationChatBubbleContent(guid, chatContent);
        }

        private void ShowTeamDuplicationChatBubbleContent(ulong guid, string chatContent)
        {
            if (_chatBubbleViewControl == null)
            {
                _chatBubbleViewControl = TeamDuplicationUtility.LoadTeamDuplicationChatBubbleViewControl(chatRoot);
                if (_chatBubbleViewControl != null)
                {
                    //设置旋转
                    _chatBubbleViewControl.SetChatBgRotate();
                }
            }

            if (_chatBubbleViewControl == null)
                return;

            _chatBubbleViewControl.SetChatPlayerGuid(guid);
            _chatBubbleViewControl.SetMessage(chatContent);
        }

        private void UpdateChatBubbleViewControl()
        {
            if (_chatBubbleViewControl == null)
                return;

            //角色不存在
            if (_playerDataModel == null)
            {
                ResetChatBubbleViewControl();
                return;
            }

            //角色已经更换
            if(_playerDataModel.Guid != _chatBubbleViewControl.GetChatPlayerGuid())
                ResetChatBubbleViewControl();
        }

        private void ResetChatBubbleViewControl()
        {
            if (_chatBubbleViewControl == null)
                return;

            _chatBubbleViewControl.ShowRoot(false);
        }

        #endregion

        #region UIEvent

        private void OnReceiveTeamDuplicationPlayerExpireTimeMessage(UIEvent uiEvent)
        {
            if (_playerDataModel == null)
            {
                CommonUtility.UpdateGameObjectUiGray(playerUIGrayRoot, false);
                return;
            }

            if (uiEvent == null || uiEvent.Param1 == null || uiEvent.Param2 == null)
                return;

            ulong playerGuid = (ulong)uiEvent.Param1;
            ulong playerExpireTime = (ulong)uiEvent.Param2;

            if (_playerDataModel.Guid != playerGuid)
                return;

            UpdatePlayerGrayCover(playerExpireTime);
        }

        //更新GrayCover
        private void UpdatePlayerGrayCover(ulong expireTime)
        {
            if (expireTime == 0)
            {
                CommonUtility.UpdateGameObjectUiGray(playerUIGrayRoot, false);
            }
            else
            {
                CommonUtility.UpdateGameObjectUiGray(playerUIGrayRoot, true);
            }
        }
        #endregion

    }
}
