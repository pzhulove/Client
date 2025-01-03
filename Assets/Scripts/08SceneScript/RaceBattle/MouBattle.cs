using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;
using UnityEngine.Events;

using Network;
using Protocol;
using DG.Tweening;

/// <summary>
/// Battle类
/// </summary>
namespace GameClient
{
    public class MouBattle : ActivityBattle
    {
        public MouBattle(BattleType type, eDungeonMode mode, int id) : base(type, mode, id)
        {}

        protected override void _onCreateScene(BeEvent.BeEventParam args)
        {
            base._onCreateScene(args);

            if (!mDungeonManager.GetDungeonDataManager().IsBossArea())
            {
                // 打开当前牛头怪数量提示界面
                //ClientSystemManager.instance.OpenFrame<DungeonPreTips>();
            }
        }

        protected override void _onAreaClear(BeEvent.BeEventParam args)
        {
            if (mDungeonManager.GetDungeonDataManager().IsBossArea())
            {
#if !LOGIC_SERVER
		        _sendDungeonKillMonsterReq();
                _sendDungeonRewardReq();
                _sendDungeonBossRewardReq();
#endif
                mDungeonManager.FinishFight();

#if !LOGIC_SERVER
                GameFrameWork.instance.StartCoroutine(_successEnd());
#endif
            }
        }

        protected override void _onPlayerCancelReborn(BattlePlayer player)
        {
#if !LOGIC_SERVER
            GameFrameWork.instance.StartCoroutine(_failEnd());
#endif
        }

        protected IEnumerator _failEnd()
        {
#if !LOGIC_SERVER
            //存储战斗结果
            PveBattleResult = BattleResult.Fail;
#endif
            _openFinishFrame<DungeonCommonFailFrame>();
            _setFinish(false);

            yield return _fireRaceEndOnLocalFrameIter();
            yield return _sendDungeonReportDataIter();

            yield return Yielders.EndOfFrame;

            if (_isNeedSendNet())
            {
                var req = _getDungeonRaceEndReq();
                var res = new SceneDungeonRaceEndRes();
                var msg = new MessageEvents();

                yield return (_sendMsgWithResend<SceneDungeonRaceEndReq, SceneDungeonRaceEndRes>(ServerType.GATE_SERVER, msg, req, res));

                if (msg.IsAllMessageReceived())
                {
                    _setExp(res.killMonsterTotalExp + res.raceEndExp);
                    _setDrop(null);
                }
            }
            else
            {
                _setExp(10);
                _setExp(1000);
                _setDrop(null);
            }
        }

        protected IEnumerator _successEnd()
        {
#if !LOGIC_SERVER
            //存储战斗结果
            PveBattleResult = BattleResult.Success;
#endif
            _openFinishFrame<DungeonMouFinishFrame>();
            _setFinish(true);

            yield return _fireRaceEndOnLocalFrameIter();
            yield return _sendDungeonReportDataIter();

            yield return Yielders.EndOfFrame;

            if (_isNeedSendNet())
            {
                var req = _getDungeonRaceEndReq();
                var res = new SceneDungeonRaceEndRes();
                var msg = new MessageEvents();

                yield return (_sendMsgWithResend<SceneDungeonRaceEndReq, SceneDungeonRaceEndRes>(ServerType.GATE_SERVER, msg, req, res));

                if (msg.IsAllMessageReceived())
                {
                    if (0 != res.hasRaceEndDrop)
                    {
                        yield return _requestRaceEndDrops(res.raceEndDropBaseMulti);
                    }

                    _setExp(res.killMonsterTotalExp + res.raceEndExp);
                    _setDrop(_getAllRewardItems(res).ToArray());
                }
            }
            else
            {
                _setExp(10);
                _setExp(1000);
                _setDrop(null);
            }
        }
    }
}
