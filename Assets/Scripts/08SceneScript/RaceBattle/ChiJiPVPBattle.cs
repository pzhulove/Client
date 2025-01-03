using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Protocol;
using Network;
namespace GameClient
{
    public class ChiJiPVPBattle : PVPBattle
    {
        public ChiJiPVPBattle(BattleType type, eDungeonMode mode, int id) : base(type, mode, id) { }
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
                    if (this.recordServer != null && recordServer.IsProcessRecord())
                    {
                        this.recordServer.RecordProcess(string.Format("[ChijiPVP] PID:{0} HP:{1} rate:{2} MaxHP:{3}", players[i].playerActor.GetPID(), attribute.GetHP(), remainHPRate, attribute.GetMaxHP()));
                        this.recordServer.MarkInt(0x7777778, players[i].playerActor.GetPID(), attribute.GetHP(), (int)(remainHPRate * 10000), attribute.GetMaxHP());
                        // Mark:0x7777778 [ChijiPVP] PID:{0} HP:{1} rate:{2} MaxHP:{3}

                    }
                }
            }
        }

        [MessageHandle(SceneMatchPkRaceEnd.MsgID)]
        void _OnReceiveChijiPkEndData(MsgDATA msg)
        {
            SceneMatchPkRaceEnd pkEndData = new SceneMatchPkRaceEnd();
            pkEndData.decode(msg.bytes);

            Logger.LogWarningFormat("ReceivePkEndData {0}\n", ObjectDumper.Dump(pkEndData));

#if !LOGIC_SERVER
            if (RecordServer.GetInstance().IsReplayRecord())
            {
                RecordServer.GetInstance().RecordResult(pkEndData);
            }
#endif
            //mDungeonManager.FinishFight();
            BattleMain.instance.End();

#if !LOGIC_SERVER
            if (RecordServer.GetInstance().IsReplayRecord())
            {
                RecordServer.GetInstance().EndRecord("PkEnd");
            }
#endif
            //ClientSystemManager.instance.OpenFrame<PKBattleResultFrame>(FrameLayer.Middle, pkEndData);
            ChijiDataManager.GetInstance().PkEndData = pkEndData;

            if (ClientSystemManager.instance.TargetSystem != null)
            {
                ChijiDataManager.GetInstance().GuardForPkEndData = true;
                return;
            }

            // 失败退出到城镇要清掉吃鸡相关数据
            ChijiDataManager.GetInstance().ResponseBattleEnd();
        }
    }
}
