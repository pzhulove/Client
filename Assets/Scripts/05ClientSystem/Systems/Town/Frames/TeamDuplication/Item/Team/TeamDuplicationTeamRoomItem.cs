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

    //
    public class TeamDuplicationTeamRoomItem : MonoBehaviour
    {
        //小队数据
        private TeamDuplicationCaptainDataModel _captainDataModel;
        private bool _isOtherTeam;

        [Space(10)]
        [HeaderAttribute("Text")]
        [Space(5)]
        [SerializeField] private Text troopIndexTitle;

        [Space(10)]
        [HeaderAttribute("TroopRoomPlayerItem")]
        [Space(5)]
        [SerializeField] private TeamDuplicationTeamRoomPlayerItem firstPlayerItem;
        [SerializeField] private TeamDuplicationTeamRoomPlayerItem secondPlayerItem;
        [SerializeField] private TeamDuplicationTeamRoomPlayerItem thirdPlayerItem;

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
            _captainDataModel = null;
            _isOtherTeam = false;
        }



        public void Init(TeamDuplicationCaptainDataModel captainDataModel,
            bool isOtherTeam = false)
        {
            _captainDataModel = captainDataModel;
            _isOtherTeam = isOtherTeam;

            if (_captainDataModel == null)
            {
                Logger.LogErrorFormat("TeamDuplicationTroopTotalItem Init troopDataModel is null");
                return;
            }

            InitItem();
        }

        private void InitItem()
        {

            if (troopIndexTitle != null)
            {
                troopIndexTitle.text = string.Format(TR.Value("team_duplication_troop_room_team_title"),
                    _captainDataModel.CaptainId);
            }

            var firstPlayerSeatId = (_captainDataModel.CaptainId - 1) * 3 + 1;
            var secondPlayerSeatId = (_captainDataModel.CaptainId - 1) * 3 + 2;
            var thirdPlayerSeatId = (_captainDataModel.CaptainId - 1) * 3 + 3;

            if (firstPlayerItem != null)
            {
                if (_captainDataModel.PlayerList != null && _captainDataModel.PlayerList.Count > 0)
                {
                    firstPlayerItem.InitItem(_captainDataModel.PlayerList[0],
                        _isOtherTeam,
                        firstPlayerSeatId);
                }
                else
                {
                    firstPlayerItem.InitItem(null,
                        _isOtherTeam,
                        firstPlayerSeatId);
                }
            }

            if (secondPlayerItem != null)
            {
                if (_captainDataModel.PlayerList != null && _captainDataModel.PlayerList.Count > 1)
                {
                    secondPlayerItem.InitItem(_captainDataModel.PlayerList[1],
                        _isOtherTeam,
                        secondPlayerSeatId);
                }
                else
                {
                    secondPlayerItem.InitItem(null,
                        _isOtherTeam,
                        secondPlayerSeatId);
                }
            }

            if (thirdPlayerItem != null)
            {
                if (_captainDataModel.PlayerList != null && _captainDataModel.PlayerList.Count > 2)
                {
                    thirdPlayerItem.InitItem(_captainDataModel.PlayerList[2],
                        _isOtherTeam,
                        thirdPlayerSeatId);
                }
                else
                {
                    thirdPlayerItem.InitItem(null,
                        _isOtherTeam,
                        thirdPlayerSeatId);
                }
            }
        }

    }
}
