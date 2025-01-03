using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * 怪物血量百分比达到一定值{ValueA}以下，召唤某怪物{ValueB}，召唤位置{ValueC}，怪物朝向{ValueD}
*/
public class Mechanism67 : BeMechanism
{
    VFactor percent;
    int summonID;
    List<VInt3> points;
    List<int> faces;//0-跟召唤者一致 1-面向主要角色 2-左 3-右

    IBeEventHandle handler = null;

    public Mechanism67(int mid, int lv) : base(mid, lv) { }

    public override void OnReset()
    {
        if (points != null)
            points.Clear();
        if (faces != null)
            faces.Clear();
        handler = null;
    }
    public override void OnInit()
    {
        percent = VFactor.NewVFactor(TableManager.GetValueFromUnionCell(data.ValueA[0], level), GlobalLogic.VALUE_100);
        summonID = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        var height = VInt.NewVInt(TableManager.GetValueFromUnionCell(data.ValueE[0], level), GlobalLogic.VALUE_1000);
        points = new List<VInt3>();
        for (int i = 0; i < data.ValueC.Count; i++)
        {
            var x = VInt.NewVInt(data.ValueC[i].fixInitValue, GlobalLogic.VALUE_1000);
            var y = VInt.NewVInt(data.ValueC[i].fixLevelGrow, GlobalLogic.VALUE_1000);
            points.Add(new VInt3(x.i, y.i, height.i));
        }
        faces = new List<int>();
        for (int i = 0; i < data.ValueD.Count; i++)
        {
            faces.Add(TableManager.GetValueFromUnionCell(data.ValueD[i], level));
        }
    }

    public override void OnStart()
    {
        if (owner != null)
        {
            handler = owner.RegisterEventNew(BeEventType.onHPChange, (args) =>
            {
                if (!owner.IsDead())
                {
                    VFactor hpRate = owner.GetEntityData().GetHPRate();
                    if (hpRate < percent)
                    {
                        DoSummon();
                        RemoveHandler();
                    }
                }
            });
        }
    }

    void DoSummon()
    {
        for (int i = 0; i < points.Count; i++)
        {
            int mLevel = owner.GetEntityData().level;
            int id = summonID + mLevel * GlobalLogic.VALUE_100;
            BeUtility.AdjustMonsterDifficulty(ref owner.GetEntityData().monsterID, ref id);
            var actor = owner.CurrentBeScene.SummonMonster(id, points[i], owner.GetCamp(), owner, false, owner.GetEntityData().GetLevel());
            if (actor != null)
            {
                if (faces[i] == 0)
                {
                    actor.SetFace(owner.GetFace());
                }
                else if (faces[i] == 1)
                {
                    var players = owner.CurrentBeBattle.dungeonPlayerManager.GetAllPlayers();
                    int playerIndex = -1;
                    for (int j = 0; j < players.Count; j++)
                    {
                        var a = players[j].playerActor;
                        if (a != null && !a.IsDead())
                        {
                            playerIndex = j;
                            break;
                        }
                    }
                    if (playerIndex >= 0 && playerIndex < players.Count)
                    {
                        actor.SetFace(players[playerIndex].playerActor.GetPosition().x < actor.GetPosition().x);
                    }
                }
                else if (faces[i] == 2)
                {
                    actor.SetFace(true);
                }
                else if (faces[i] == 3)
                {
                    actor.SetFace(false);
                }
            }
        }
    }

    void RemoveHandler()
    {
        if (handler != null)
        {
            handler.Remove();
            handler = null;
        }
    }

    public override void OnFinish()
    {
        RemoveHandler();
    }
}
