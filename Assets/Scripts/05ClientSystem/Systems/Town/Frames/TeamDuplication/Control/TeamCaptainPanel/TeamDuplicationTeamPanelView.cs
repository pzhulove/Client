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

    //团队控制器
    public class TeamDuplicationTeamPanelView : TeamDuplicationTeamCaptainPanelBaseView
    {

        private TeamDuplicationTeamDataModel _teamDataModel = null;

        [Space(15)]
        [HeaderAttribute("ComUIListEx")]
        [Space(5)]
        [SerializeField]
        private ComUIListScriptEx captainItemList;

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
            if (captainItemList != null)
            {
                captainItemList.Initialize();
                captainItemList.onItemVisiable += OnItemVisible;
            }
        }

        private void UnBindUiEvents()
        {
            if (captainItemList != null)
            {
                captainItemList.onItemVisiable -= OnItemVisible;
            }
        }

        private void ClearData()
        {
            _teamDataModel = null;
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
            UpdateCaptainItemList();
        }

        public override void OnEnableView()
        {
            UpdateCaptainItemList();
        }

        private void OnReceiveTeamDuplicationTeamDataMessage(UIEvent uiEvent)
        {
            UpdateCaptainItemList();
        }

        private void UpdateCaptainItemList()
        {
            _teamDataModel = TeamDuplicationDataManager.GetInstance().GetTeamDuplicationTeamDataModel();
            if (captainItemList != null)
            {
                if (_teamDataModel == null ||
                    _teamDataModel.CaptainDataModelList == null
                    || _teamDataModel.CaptainDataModelList.Count <= 0)
                    captainItemList.SetElementAmount(0);
                else
                {
                    captainItemList.SetElementAmount(_teamDataModel.CaptainDataModelList.Count);
                }
            }
        }

        private void OnItemVisible(ComUIListElementScript item)
        {
            if (item == null)
                return;

            if (_teamDataModel == null
                || _teamDataModel.CaptainDataModelList == null
                || _teamDataModel.CaptainDataModelList.Count <= 0)
                return;

            if (item.m_index >= _teamDataModel.CaptainDataModelList.Count)
                return;

            TeamDuplicationTeamCaptainItem teamCaptainItem = item.GetComponent<TeamDuplicationTeamCaptainItem>();
            TeamDuplicationCaptainDataModel captainDataModel = _teamDataModel.CaptainDataModelList[item.m_index];

            if (teamCaptainItem != null && captainDataModel != null)
            {
                teamCaptainItem.Init(captainDataModel);
            }
        }

        //////更新列表
    }
}
