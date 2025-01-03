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

    //投票的Item
    public class TeamDuplicationFightVoteItem : MonoBehaviour
    {
        private TeamDuplicationCaptainDataModel _captainDataModel;
        private TeamDuplicationFightVoteType _fightVoteType;            //战斗开始，战斗结束

        [Space(10)]
        [HeaderAttribute("Text")]
        [Space(5)]
        [SerializeField] private Text troopIndexTitle;

        [Space(10)]
        [HeaderAttribute("FightVotePlayer")]
        [Space(5)]
        [SerializeField] private TeamDuplicationFightVotePlayerItem firstPlayerItem;
        [SerializeField] private TeamDuplicationFightVotePlayerItem secondPlayerItem;
        [SerializeField] private TeamDuplicationFightVotePlayerItem thirdPlayerItem;

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
            _fightVoteType = TeamDuplicationFightVoteType.None;
        }


        //默认类型为战斗开始
        public void Init(TeamDuplicationCaptainDataModel captainDataModel,
            TeamDuplicationFightVoteType fightVoteType = TeamDuplicationFightVoteType.FightStartVote)
        {
            _captainDataModel = captainDataModel;
            _fightVoteType = fightVoteType;

            if (_captainDataModel == null)
            {
                Logger.LogErrorFormat("TeamDuplicationTroopTotalItem Init troopDataModel is null");
                return;
            }

            InitItem();
        }

        private void InitItem()
        {
            InitCaptainLabel();

            InitVotePlayerItem();
        }

        private void InitCaptainLabel()
        {
            if (troopIndexTitle != null)
                troopIndexTitle.text = _captainDataModel.CaptainId.ToString();
        }

        private void InitVotePlayerItem()
        {
            if (firstPlayerItem != null)
            {
                if (_captainDataModel.PlayerList != null && _captainDataModel.PlayerList.Count > 0)
                {
                    firstPlayerItem.InitVotePlayerItem(_captainDataModel.PlayerList[0],
                        _fightVoteType);
                }
                else
                {
                    firstPlayerItem.InitVotePlayerItem(null,
                        _fightVoteType);
                }
            }

            if (secondPlayerItem != null)
            {
                if (_captainDataModel.PlayerList != null && _captainDataModel.PlayerList.Count > 1)
                {
                    secondPlayerItem.InitVotePlayerItem(_captainDataModel.PlayerList[1],
                        _fightVoteType);
                }
                else
                {
                    secondPlayerItem.InitVotePlayerItem(null,
                        _fightVoteType);
                }
            }

            if (thirdPlayerItem != null)
            {
                if (_captainDataModel.PlayerList != null && _captainDataModel.PlayerList.Count > 2)
                {
                    thirdPlayerItem.InitVotePlayerItem(_captainDataModel.PlayerList[2],
                        _fightVoteType);
                }
                else
                {
                    thirdPlayerItem.InitVotePlayerItem(null,
                        _fightVoteType);
                }
            }
        }

        public void Reset()
        {
            if(firstPlayerItem != null)
                firstPlayerItem.Reset();

            if(secondPlayerItem != null)
                secondPlayerItem.Reset();

            if(thirdPlayerItem != null)
                thirdPlayerItem.Reset();
        }
        
    }
}
