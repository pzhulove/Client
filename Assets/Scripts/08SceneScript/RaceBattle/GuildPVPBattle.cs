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
    public class GuildPVPBattle : PVPBattle
    {

        public GuildPVPBattle(BattleType type, eDungeonMode mode, int id) : base(type, mode, id)
        {

        }

		public override void _postCreatePlayer ()
		{
			var players = mDungeonPlayers.GetAllPlayers();
			for(int i=0; i<players.Count; ++i)
			{
				var actor = players[i].playerActor;

				if (actor != null)
				{
					var attribute = players[i].playerActor.GetEntityData();
					var remainHPRate = players[i].playerInfo.remainHp/(float)(GlobalLogic.VALUE_1000);
					remainHPRate = Mathf.Clamp01(remainHPRate);
					attribute.SetHP(IntMath.Float2Int(remainHPRate * attribute.GetMaxHP()));

					actor.m_pkGeActor.SetHPValue(remainHPRate);
				}
			}
		}

        [MessageHandle(WorldGuildBattleRaceEnd.MsgID)]
        void _OnWorldGuildBattleRaceEnd(MsgDATA msg)
        {
            WorldGuildBattleRaceEnd battleRaceEnd = new WorldGuildBattleRaceEnd();
            battleRaceEnd.decode(msg.bytes);

            Logger.LogWarningFormat("ReceivePkEndData {0}\n", ObjectDumper.Dump(battleRaceEnd));

            mDungeonManager.FinishFight();
            //!!
            BattleMain.instance.End();

			#if !LOGIC_SERVER
			if (RecordServer.GetInstance().IsReplayRecord())
			{
				RecordServer.GetInstance().EndRecord("GuildBattleRaceEnd");
			}
			#endif

            bool bWin = (battleRaceEnd.result == 1);
            if (bWin)
            {
                ClientSystemManager.GetInstance().OpenFrame<GuildBattleWinFrame>(FrameLayer.Middle, battleRaceEnd);
            }
            else
            {
                ClientSystemManager.GetInstance().OpenFrame<GuildBattleLoseFrame>(FrameLayer.Middle, battleRaceEnd);
            }
        }
    }
}
