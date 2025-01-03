using UnityEngine;
using System.Collections.Generic;

/*
 * 靠近后释放某技能
*/
public sealed class Mechanism76 : BeMechanism
{
    enum eCalcType
    {
        CalcXY = 0,
        CalcX,
        CalcY
    }

    VInt distance;                              //出发距离
    eCalcType type;                             //检测类型
    int skillId;                                //释放的技能
    List<int> listBuffId = new List<int>();     //结束要移除的buff
    int totalTime;                                   //机制持续时间

    readonly bool isForceUse = true;
    readonly int interval = 250;                         //检测距离时间间隔
    int timer;
    bool usedSkill;

    public Mechanism76(int mid, int lv) : base(mid, lv) { }

    public override void OnReset()
    {
        listBuffId.Clear();
        timer = 0;
        usedSkill = false;
    }

    public override void OnInit()
    {
        distance = VInt.NewVInt(TableManager.GetValueFromUnionCell(data.ValueA[0], level), GlobalLogic.VALUE_1000);
        type = (eCalcType)TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        skillId = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
        for (int i = 0; i < data.ValueD.Count; i++)
        {
            var buffId = TableManager.GetValueFromUnionCell(data.ValueD[i], level);
            listBuffId.Add(buffId);
        }
        totalTime = TableManager.GetValueFromUnionCell(data.ValueE[0], level);
    }

    public override void OnStart()
    {
        timer = 0;
        usedSkill = false;
    }

    public override void OnUpdate(int deltaTime)
    {
        if (owner != null)
        {
            if (!usedSkill)
            {
                if (owner.aiManager.IsRunning())
                {
                    owner.aiManager.Stop();
                }

                totalTime -= deltaTime;
                if (totalTime <= 0)
                {
                    UseSkill(null);
                    return;
                }

                timer += deltaTime;
                if (timer >= interval)
                {
                    timer = 0;
                    List<BeActor> targets = GamePool.ListPool<BeActor>.Get();
                    owner.CurrentBeScene.FindTargets(targets, owner, VInt.Float2VIntValue(100f));
                    for (int i = 0; i < targets.Count; i++)
                    {
                        var target = targets[i];
                        if (target != null && !target.IsDead())
                        {
                            if (type == eCalcType.CalcXY)
                            {
                                var vec = target.GetPosition() - owner.GetPosition();
                                if (vec.magnitude < distance.i)
                                {
                                    UseSkill(target);
                                    break;
                                }
                            }
                            else if (type == eCalcType.CalcX)
                            {
                                int x = Mathf.Abs(target.GetPosition().x - owner.GetPosition().x);
                                if (x < distance.i)
                                {
                                    UseSkill(target);
                                    break;
                                }
                            }
                            else if (type == eCalcType.CalcY)
                            {
                                int y = Mathf.Abs(target.GetPosition().y - owner.GetPosition().y);
                                if (y < distance.i)
                                {
                                    UseSkill(target);
                                    break;
                                }
                            }
                        }
                    }
                    GamePool.ListPool<BeActor>.Release(targets);
                }
            }
        }
    }

    void UseSkill(BeActor target)
    {
        usedSkill = owner.UseSkill(skillId, isForceUse);
        if (usedSkill)
        {
            if (owner.aiManager.state == BeAIManager.State.STOP)
            {
                owner.aiManager.Start();
            }
            if (target != null)
            {
                owner.SetFace(target.GetPosition().x < owner.GetPosition().x);
            }
            else
            {
                var players = owner.CurrentBeBattle.dungeonPlayerManager.GetAllPlayers();
                int playerIndex = -1;
                for (int i = 0; i < players.Count; i++)
                {
                    var actor = players[i].playerActor;
                    if (actor != null && !actor.IsDead())
                    {
                        playerIndex = i;
                        break;
                    }
                }
                if (playerIndex >= 0 && playerIndex < players.Count)
                {
                    owner.SetFace(players[playerIndex].playerActor.GetPosition().x < owner.GetPosition().x);
                }
            }
            RemoveBuffs();
        }
    }

    void RemoveBuffs()
    {
        for (int i = 0; i < listBuffId.Count; i++)
        {
            owner.buffController.RemoveBuff(listBuffId[i]);
        }
        listBuffId.Clear();
    }

}
