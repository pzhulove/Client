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
    public class NorthBattle : ActivityBattle
    {
        public NorthBattle(BattleType type, eDungeonMode mode, int id) : base(type, mode, id)
        {}

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

        private IEnumerator _failEnd()
        {
            _openFinishFrame<DungeonCommonFailFrame>();

            _setFinish(false);

            yield return _fireRaceEndOnLocalFrameIter();
            yield return _sendDungeonReportDataIter();

            if (_isNeedSendNet())
            {
                var req = _getDungeonRaceEndReq();
                var res = new SceneDungeonRaceEndRes();
                var msg = new MessageEvents();

                yield return (_sendMsgWithResend<SceneDungeonRaceEndReq, SceneDungeonRaceEndRes>(ServerType.GATE_SERVER, msg, req, res));

                if (msg.IsAllMessageReceived())
                {
                    _setExp(res.killMonsterTotalExp + res.raceEndExp);
                    if (res.hasRaceEndDrop == 1)
                    {
                        _setDrop(null);
                    }
                }
            }
            else
            {
                _setExp(10);
                _setExp(1000);
                _setDrop(null);
            }
        }

        private IEnumerator _successEnd()
        {
            _openFinishFrame<DungeonNorthFinishFrame>();
            _setFinish(true);

            yield return _fireRaceEndOnLocalFrameIter();
            yield return _sendDungeonReportDataIter();

            if (_isNeedSendNet())
            {
                var req = _getDungeonRaceEndReq();
                var res = new SceneDungeonRaceEndRes();
                var msg = new MessageEvents();

                yield return (_sendMsgWithResend<SceneDungeonRaceEndReq, SceneDungeonRaceEndRes>(ServerType.GATE_SERVER, msg, req, res));

                if (msg.IsAllMessageReceived())
                {
                    if (res.hasRaceEndDrop != 0)
                    {
                        yield return (_endDropItemsIter(ChapterNorthFrame.sMuti));
                    }

                    _setExp(res.raceEndExp + res.killMonsterTotalExp);
                }
            }
            else
            {
                _setDrop(new ComItemList.Items[0]);
            }
        }

        /// <summary>
        /// 结算掉落
        /// </summary>
        /// <param name="multi"></param>
        /// <returns></returns>
        protected IEnumerator _endDropItemsIter(int multi)
        {
            if (!_isNeedSendNet())
            {
                _setDrop(new ComItemList.Items[0]);
                yield break;
            }

            var msg = new MessageEvents();
            var req = new SceneDungeonEndDropReq();
            var res = new SceneDungeonEndDropRes();

            req.multi = (byte)multi;

            yield return (MessageUtility.Wait<SceneDungeonEndDropReq, SceneDungeonEndDropRes>(ServerType.GATE_SERVER, msg, req, res, true, 10));

            if (msg.IsAllMessageReceived())
            {
                if (res.multi == 0)
                {
                    Logger.LogError("fail to get the resutle drop item");
                    _setDrop(new ComItemList.Items[0]);
                    yield break;
                }

                ChapterNorthFrame.sMuti = res.multi;

                _setDrop(_convertComItemList(res.items));
            }
        }
    }
}
