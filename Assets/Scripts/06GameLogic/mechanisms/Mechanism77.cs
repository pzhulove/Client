using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mechanism77 : BeMechanism
{
    string effectPath;
    int monsterId;
    int pointCount;
    int summonLimit;
    int unitDis;

    public Mechanism77(int sid, int skillLevel) : base(sid, skillLevel) { }

    public override void OnInit()
    {
        effectPath = data.StringValueA[0];
        monsterId = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        pointCount = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        summonLimit = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
        var dis = VInt.NewVInt(TableManager.GetValueFromUnionCell(data.ValueD[0], level), GlobalLogic.VALUE_1000);
        unitDis = dis.i;
    }

    public override void OnStart()
    {
        List<BeActor> list = new List<BeActor>();
        owner.CurrentBeScene.FindActorById(list, monsterId);
        int summonCount = IntMath.Min(pointCount, summonLimit - list.Count);
        if (summonCount > 0)
        {
            var points = GetPoints();
            object[] summoned = new object[summonCount];
            for (int i = 0; i < summonCount; i++)
            {
                if (owner.DoSummon(monsterId, level, ProtoTable.EffectTable.eSummonPosType.ORIGIN, null, 1, 0, 0, 0, 0, false, 0, 0, null, SummonDisplayType.NONE, summoned))
                {
                    if (summoned[0] != null)
                    {
                        var monster = (BeActor)summoned[0];
                        monster.SetPosition(points[i]);

                        var effPos = points[i].vec3;
                        owner.CurrentBeScene.currentGeScene.CreateEffect(effectPath, 0, effPos);
                    }
                }
            }
        }
    }

    VInt3[] GetPoints()
    {
        var startPos = owner.GetPosition();

        VInt3[] nodes = new VInt3[pointCount * 2 + 1];
        for (int i = 0; i < pointCount; ++i)
        {
            nodes[i * 2] = startPos;
            nodes[i * 2].y -= (i + 1) * unitDis;
            nodes[i * 2 + 1] = startPos;
            nodes[i * 2 + 1].y += (i + 1) * unitDis;
        }
        nodes[pointCount * 2] = startPos;
        
        VInt3[] points = new VInt3[pointCount];
        int index = 0;
        for (int i = 0; i < pointCount; ++i)
        {
            for (int j = 0; j < nodes.Length; j++)
            {
                if (index >= pointCount)
                    break;
                if (!owner.CurrentBeScene.IsInBlockPlayer(nodes[j]))
                {
                    points[index++] = nodes[j];
                }
            }
        }
        for (; index < pointCount; ++index)//填不满就用起点补上
        {
            points[index] = startPos;
        }

        return points;
    }
}
