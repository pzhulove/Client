using System;
using System.Collections.Generic;
using UnityEngine;

//将敌人瞬移到角色身前
class Mechanism103 : BeMechanism
{
    string effectPath1;
    string effectPath2;
    int[] buffArray;
    int delay;
    VInt distance;

    BeActor player;
   // List<BeBuff> buffList = new List<BeBuff>();
    List<int> buffPIDList = new List<int>();

    public Mechanism103(int mid, int lv) : base(mid, lv) { }

    public override void OnReset()
    {
        player = null;
        buffPIDList.Clear();
    }
    public override void OnInit()
    {
        effectPath1 = data.StringValueA[0];
        effectPath2 = data.StringValueA[1];
        buffArray = new int[data.ValueA.Length];
        for (int i = 0; i < data.ValueA.Length; i++)
            buffArray[i] = TableManager.GetValueFromUnionCell(data.ValueA[i], level);
        delay = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        distance = VInt.NewVInt(TableManager.GetValueFromUnionCell(data.ValueC[0], level), GlobalLogic.VALUE_1000);
    }

    public override void OnStart()
    {
        var players = owner.CurrentBeBattle.dungeonPlayerManager.GetAllPlayers();
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].playerActor != null && !players[i].playerActor.IsDead())
            {
                player = players[i].playerActor;
                DoWork();

                break;
            }
        }
    }

    void DoWork()
    {
        if (player == null)
            return;

        AddBuffs();

        player.m_pkGeActor.CreateEffect(effectPath1, "[actor]Body", 0, Vec3.zero);

        player.delayCaller.DelayCall(delay, () =>
        {
            var pos = owner.GetPosition();
            pos.x += owner.GetFace() ? -distance.i : distance.i;

            if (player == null)
            {
                return;
            }

            if (player.CurrentBeScene.IsInBlockPlayer(pos))
            {
                var transpos = BeAIManager.FindStandPositionNew(pos, player.CurrentBeScene,!owner.GetFace(), false, 40);
                player.SetPosition(transpos);
            }
            else
            {
                player.SetPosition(pos);
            }
            RemoveBuffs();
#if !LOGIC_SERVER
            var bodyNode = player.m_pkGeActor.GetAttachNode("[actor]Body");
            if(bodyNode != null)
                pos.z = (new VInt3(bodyNode.transform.position)).z;
            player.CurrentBeScene.currentGeScene.CreateEffect(effectPath2, 0, pos.vec3);
#endif
        });
    }

    void AddBuffs()
    {
        if (player == null)
            return;

        for (int i = 0; i < buffArray.Length; i++)
        {
            var buff = player.buffController.TryAddBuff(buffArray[i], GlobalLogic.VALUE_100000);
            if (buff != null)
                //buffList.Add(buff);
                buffPIDList.Add(buff.PID);
        }
    }

    void RemoveBuffs()
    {
        if (player == null)
            return;

        for (int i = 0; i < buffPIDList.Count; i++)
        {
            player.buffController.RemoveBuffByPID(buffPIDList[i]);
        }
        buffPIDList.Clear();
    }

    public override void OnFinish()
    {
        RemoveBuffs();
    }
}