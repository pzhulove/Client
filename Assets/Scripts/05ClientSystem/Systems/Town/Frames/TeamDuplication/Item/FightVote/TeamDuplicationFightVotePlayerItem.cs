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

    //投票的角色Item
    public class TeamDuplicationFightVotePlayerItem : TeamDuplicationBasePlayerItem
    {
        private TeamDuplicationFightVoteType _fightVoteType;
        [SerializeField] private GameObject headGrayCover;

        [Space(10)] [HeaderAttribute("VoteResult")] [Space(10)]
        [SerializeField] private GameObject voteResultRoot;
        [SerializeField] private GameObject agreeRoot;
        [SerializeField] private GameObject refuseRoot;

        private void OnEnable()
        {
            //战斗开始投票同意
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationFightStartVoteAgreeMessage,
                OnReceiveTeamDuplicationFightStartVoteAgreeMessage);

            //战斗结束投票同意
            UIEventSystem.GetInstance().RegisterEventHandler(
                EUIEventID.OnReceiveTeamDuplicationFightEndVoteAgreeMessage,
                OnReceiveTeamDuplicationFightEndVoteAgreeMessage);

            //战斗结束投票拒绝
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationFightEndVoteRefuseMessage,
                OnReceiveTeamDuplicationFightEndVoteRefuseMessage);
        }

        private void OnDisable()
        {
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationFightStartVoteAgreeMessage,
                OnReceiveTeamDuplicationFightStartVoteAgreeMessage);

            UIEventSystem.GetInstance().UnRegisterEventHandler(
                EUIEventID.OnReceiveTeamDuplicationFightEndVoteAgreeMessage,
                OnReceiveTeamDuplicationFightEndVoteAgreeMessage);

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnReceiveTeamDuplicationFightEndVoteRefuseMessage,
                OnReceiveTeamDuplicationFightEndVoteRefuseMessage);
        }

        //默认为战斗开始
        public void InitVotePlayerItem(TeamDuplicationPlayerDataModel playerDataModel,
            TeamDuplicationFightVoteType fightVoteType = TeamDuplicationFightVoteType.FightStartVote)
        {
            _fightVoteType = fightVoteType;

            base.Init(playerDataModel);

            if (playerDataModel == null)
                return;

            var playerGuid = playerDataModel.Guid;
            if (_fightVoteType == TeamDuplicationFightVoteType.FightEndVote)
            {
                //战斗结束投票
                bool isAgreeVote = TeamDuplicationUtility.IsPlayerAlreadyAgreeFightEndVote(playerGuid);
                bool isRefuseVote = TeamDuplicationUtility.IsPlayerAlreadyRefuseFightEndVote(playerGuid);
                //还没有投票
                if (isAgreeVote == false && isRefuseVote == false)
                {
                    UpdateHeadGray(false);
                    CommonUtility.UpdateGameObjectVisible(voteResultRoot, false);
                }
                else
                {
                    //已经投票
                    CommonUtility.UpdateGameObjectVisible(voteResultRoot, true);
                    if (isAgreeVote == true)
                    {
                        CommonUtility.UpdateGameObjectVisible(agreeRoot, true);
                        CommonUtility.UpdateGameObjectVisible(refuseRoot, false);
                    }
                    else
                    {
                        CommonUtility.UpdateGameObjectVisible(agreeRoot, false);
                        CommonUtility.UpdateGameObjectVisible(refuseRoot, true);
                    }
                }
            }
            else
            {
                //战斗开始投票
                var isAgreeVote = TeamDuplicationUtility.IsPlayerAlreadyAgreeFightStartVote(playerGuid);
                UpdateHeadGray(isAgreeVote);
                CommonUtility.UpdateGameObjectVisible(voteResultRoot, false);
            }
        }

        //战斗开始投票
        private void OnReceiveTeamDuplicationFightStartVoteAgreeMessage(UIEvent uiEvent)
        {
            //当前类型为战斗结束投票类型
            if (_fightVoteType == TeamDuplicationFightVoteType.FightEndVote)
                return;

            if (PlayerDataModel == null)
                return;

            if (uiEvent == null || uiEvent.Param1 == null)
                return;

            var voteAgreePlayerId = (ulong)uiEvent.Param1;

            //同意的角色不是自己,不进行操作
            if (voteAgreePlayerId != PlayerDataModel.Guid)
                return;

            UpdateHeadGray(true);
            CommonUtility.UpdateGameObjectVisible(voteResultRoot, false);
        }

        //战斗结束投票(同意)
        private void OnReceiveTeamDuplicationFightEndVoteAgreeMessage(UIEvent uiEvent)
        {
            //当前类型不为战斗结束投票类型
            if (_fightVoteType != TeamDuplicationFightVoteType.FightEndVote)
                return;


            if (PlayerDataModel == null)
                return;

            if (uiEvent == null || uiEvent.Param1 == null)
                return;

            var voteAgreePlayerId = (ulong)uiEvent.Param1;

            //同意的角色不是自己,不进行操作
            if (voteAgreePlayerId != PlayerDataModel.Guid)
                return;

            UpdateHeadGray(true);
            CommonUtility.UpdateGameObjectVisible(voteResultRoot, true);
            CommonUtility.UpdateGameObjectVisible(agreeRoot, true);
            CommonUtility.UpdateGameObjectVisible(refuseRoot, false);
        }

        //战斗结束投票（拒绝）
        private void OnReceiveTeamDuplicationFightEndVoteRefuseMessage(UIEvent uiEvent)
        {
            //当前类型不为战斗结束投票类型
            if (_fightVoteType != TeamDuplicationFightVoteType.FightEndVote)
                return;

            if (PlayerDataModel == null)
                return;

            if (uiEvent == null || uiEvent.Param1 == null)
                return;

            var voteRefusePlayerId = (ulong)uiEvent.Param1;

            //同意的角色不是自己,不进行操作
            if (voteRefusePlayerId != PlayerDataModel.Guid)
                return;

            UpdateHeadGray(true);
            CommonUtility.UpdateGameObjectVisible(voteResultRoot, true);
            CommonUtility.UpdateGameObjectVisible(agreeRoot, false);
            CommonUtility.UpdateGameObjectVisible(refuseRoot, true);
        }


        //收到UI消息，进行数据更新
        private void UpdateVotePlayerItem(UIEvent uiEvent)
        {
            if (PlayerDataModel == null)
                return;

            if (uiEvent == null || uiEvent.Param1 == null)
                return;

            var voteAgreePlayerId = (ulong)uiEvent.Param1;

            //同意的角色不是自己,不进行操作
            if (voteAgreePlayerId != PlayerDataModel.Guid)
                return;

            UpdateHeadGray(true);
        }


        public void UpdateHeadGray(bool flag)
        {
            CommonUtility.UpdateGameObjectVisible(headGrayCover, !flag);
        }

        public void Reset()
        {
            PlayerDataModel = null;
            _fightVoteType = TeamDuplicationFightVoteType.None;
        }

    }
}
