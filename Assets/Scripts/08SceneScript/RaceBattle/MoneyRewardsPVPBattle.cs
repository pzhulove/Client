using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
//using ProtoBuf;
using System;
using Protocol;
using Network;

namespace GameClient
{
    public class MoneyRewardsPVPBattle : PVPBattle
    {

        public MoneyRewardsPVPBattle(BattleType type, eDungeonMode mode, int id) : base(type, mode, id)
        {

        }

        public override void _postCreatePlayer()
        {
            var players = mDungeonPlayers.GetAllPlayers();
            for (int i = 0; i < players.Count; ++i)
            {
                var actor = players[i].playerActor;

                if (actor != null)
                {
                    var attribute = players[i].playerActor.GetEntityData();
                    var remainHPRate = players[i].playerInfo.remainHp / (float)(GlobalLogic.VALUE_1000);
                    remainHPRate = Mathf.Clamp01(remainHPRate);
                    attribute.SetHP(IntMath.Float2Int(remainHPRate * attribute.GetMaxHP()));

                    actor.m_pkGeActor.SetHPValue(remainHPRate);
                }
            }
        }

        [MessageHandle(WorldPremiumLeagueRaceEnd.MsgID)]
        void OnRecvWorldPremiumLeagueRaceEnd(MsgDATA msg)
        {
            WorldPremiumLeagueRaceEnd _raceResult = new WorldPremiumLeagueRaceEnd();
            _raceResult.decode(msg.bytes);

            Logger.LogWarningFormat("<color=#00ff00>Receive WorldPremiumLeagueRaceEnd !!!<color>");

            mDungeonManager.FinishFight();
            //!!
            BattleMain.instance.End();

#if !LOGIC_SERVER
            if (RecordServer.GetInstance().IsReplayRecord())
            {
                RecordServer.GetInstance().EndRecord("LeagueRaceEnd");
            }
#endif

            bool bWin = (_raceResult.result == 1);
            if (bWin)
            {
                ClientSystemManager.GetInstance().OpenFrame<MoneyRewardsWinFrame>(FrameLayer.Middle, _raceResult);
            }
            else
            {
                ClientSystemManager.GetInstance().OpenFrame<MoneyRewardsLoseFrame>(FrameLayer.Middle, _raceResult);
            }
#if !LOGIC_SERVER
            if (ReplayServer.GetInstance().IsReplay())
            {
                ReplayServer.GetInstance().EndReplay(true,"LeagueRaceEnd Replay");
            }
#endif
        }
    }
}
