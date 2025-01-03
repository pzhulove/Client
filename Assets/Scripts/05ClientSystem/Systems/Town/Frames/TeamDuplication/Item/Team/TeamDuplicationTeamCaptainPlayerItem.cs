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

    //团队中小队的队员
    public class TeamDuplicationTeamCaptainPlayerItem : TeamDuplicationBasePlayerItem
    {
        [SerializeField] private UIGray playerUIGray;

        private void OnEnable()
        {
            //角色终止
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationPlayerExpireTimeMessage,
                OnReceiveTeamDuplicationPlayerExpireTimeMessage);
        }

        private void OnDisable()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationPlayerExpireTimeMessage,
                OnReceiveTeamDuplicationPlayerExpireTimeMessage);
        }

        public override void Init(TeamDuplicationPlayerDataModel playerDataModel)
        {
            base.Init(playerDataModel);

            InitPlayerGrayCover();
        }

        //显示角色的GrayCover
        private void InitPlayerGrayCover()
        {
            if (PlayerDataModel == null)
            {
                CommonUtility.UpdateGameObjectUiGray(playerUIGray, false);
                return;
            }

            var playerExpireTime = PlayerDataModel.ExpireTime;
            UpdatePlayerGrayCover(playerExpireTime);
        }

        #region UIEvent
        private void OnReceiveTeamDuplicationPlayerExpireTimeMessage(UIEvent uiEvent)
        {
            if (PlayerDataModel == null)
            {
                CommonUtility.UpdateGameObjectUiGray(playerUIGray, false);
                return;
            }

            if (uiEvent == null || uiEvent.Param1 == null || uiEvent.Param2 == null)
                return;

            ulong playerGuid = (ulong)uiEvent.Param1;
            ulong playerExpireTime = (ulong)uiEvent.Param2;

            if (PlayerDataModel.Guid != playerGuid)
                return;

            UpdatePlayerGrayCover(playerExpireTime);
        }

        //更新GrayCover
        private void UpdatePlayerGrayCover(ulong expireTime)
        {
            if (expireTime == 0)
            {
                CommonUtility.UpdateGameObjectUiGray(playerUIGray, false);
            }
            else
            {
                CommonUtility.UpdateGameObjectUiGray(playerUIGray, true);
            }
        }
        #endregion 

    }
}
