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

    //小队控制器
    public class TeamDuplicationCaptainPanelView : TeamDuplicationTeamCaptainPanelBaseView
    {

        private TeamDuplicationCaptainDataModel _playerDataModel = null;

        [Space(15)]
        [HeaderAttribute("ComUIList")]
        [Space(5)]
        [SerializeField]
        private ComUIListScriptEx playerItemList;

        private void Awake()
        {
            BindUiEvents();
        }

        private void OnDestroy()
        {
            UnBindUiEvents();
            ClearData();
        }

        private void BindUiEvents()
        {
            if (playerItemList != null)
            {
                playerItemList.Initialize();
                playerItemList.onItemVisiable += OnItemVisible;
            }
        }

        private void UnBindUiEvents()
        {
            if (playerItemList != null)
            {
                playerItemList.onItemVisiable -= OnItemVisible;
            }
        }

        private void ClearData()
        {
            _playerDataModel = null;
        }

        private void OnEnable()
        {
            //注册事件进行刷新
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationTeamDataMessage,
                OnReceiveTeamDuplicationTeamDataMessage);
        }

        private void OnDisable()
        {
            //注册事件进行刷新
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationTeamDataMessage,
                OnReceiveTeamDuplicationTeamDataMessage);
        }

        public override void Init()
        {
            UpdateTroopList();
        }

        public override void OnEnableView()
        {
            UpdateTroopList();
        }

        private void OnReceiveTeamDuplicationTeamDataMessage(UIEvent uiEvent)
        {
            UpdateTroopList();
        }

        private void UpdateTroopList()
        {
            _playerDataModel = TeamDuplicationUtility.GetTeamDuplicationCaptainDataModelByPlayerGuid(
                PlayerBaseData.GetInstance().RoleID);

            if (playerItemList != null)
            {
                if (_playerDataModel != null)
                {
                    if (_playerDataModel.PlayerList == null || _playerDataModel.PlayerList.Count <= 0)
                        playerItemList.SetElementAmount(0);
                    else
                    {
                        playerItemList.SetElementAmount(_playerDataModel.PlayerList.Count);
                    }
                }
                else
                {
                    playerItemList.SetElementAmount(0);
                }
            }
        }

        private void OnItemVisible(ComUIListElementScript item)
        {
            if (item == null)
                return;

            if (_playerDataModel.PlayerList == null
                || _playerDataModel.PlayerList.Count <= 0)
                return;

            if (item.m_index >= _playerDataModel.PlayerList.Count)
                return;

            TeamDuplicationCaptainPlayerItem captainPlayerItem = item.GetComponent<TeamDuplicationCaptainPlayerItem>();
            TeamDuplicationPlayerDataModel playerData = _playerDataModel.PlayerList[item.m_index];

            if (captainPlayerItem != null)
            {
                captainPlayerItem.Init(playerData);
            }
        }

        //////更新列表

    }
}
