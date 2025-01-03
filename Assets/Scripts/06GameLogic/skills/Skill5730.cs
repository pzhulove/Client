using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameClient;

//王轿修理技能
class Skill5730 : BeSkill
{
    int monsterId = 30010011;                       //召唤的哥布林ID
    int buffId = 521344;                            //王轿回复Buff
    int skillId = 5678;                             //哥布林修理完成释放的技能
    int summonCount = 6;                            //哥布林召唤数量
    int speed = 40000;                              //王轿移动速度
    int timer;
    int deathCount;                                 //记录有多少哥布林死亡
    bool result;                                    //场上还有哥布林活着的标记
    List<BeActor> monsterList = new List<BeActor>();
    List<IBeEventHandle> handleList = new List<IBeEventHandle>();
    BeActor hitMonster;
    private BeEvent.BeEventHandleNew handle1 = null;

    public Skill5730(int sid, int skillLevel) : base(sid, skillLevel) { }

    public override void OnInit()
    {

    }

    public override void OnStart()
    {
        owner.buffController.TryAddBuff((int)GlobalBuff.DUNFU, GlobalLogic.VALUE_20000);

        deathCount = 0;
    }

    public override void OnEnterPhase(int phase)
    {
        if (owner == null)
            return;

        if (phase == 1)//第一阶段移动到场景中央
        {
            owner.aiManager.StopCurrentCommand();
            owner.aiManager.Stop();

            owner.ChangeRunMode(true);
            owner.ClearMoveSpeed();

            var vSpeed = owner.CurrentBeScene.GetSceneCenterPosition() - owner.GetPosition();
            timer = vSpeed.magnitude * GlobalLogic.VALUE_1000 / speed;
            vSpeed.NormalizeTo(speed);

            owner.SetMoveSpeedX(vSpeed.x);
            owner.SetMoveSpeedY(vSpeed.y);
            owner.SetFace(vSpeed.x < 0);
        }
        else if (phase == 2)//第二阶段停止移动，进入修理状态
        {
            SummonGoblin();
        }
        else if (phase == 3)
        {
            CheckResult();
        }
    }

    VInt3[] GetPoints()
    {
        VInt3[] points = new VInt3[summonCount];
        VInt offset = VInt.NewVInt(GlobalLogic.VALUE_2000, GlobalLogic.VALUE_1000);
        var startPos = owner.GetPosition();
        startPos.x -= offset.i;
        startPos.y -= offset.i / 2;
        for (int i = 0; i < summonCount; i++)
        {
            points[i].x = startPos.x + (offset.i * (i % (summonCount / 2)));
            points[i].y = startPos.y + (offset.i * (i / (summonCount / 2)));
        }

        return points;
    }

    void SummonGoblin()
    {
        var points = GetPoints();
        for (int i = 0; i < summonCount; i++)
        {
            VInt3 point = points[i];
            owner.delayCaller.DelayCall(i * 30, () =>
            {
                object[] summoned = new object[1];
                if (owner.DoSummon(monsterId, level, ProtoTable.EffectTable.eSummonPosType.ORIGIN, null, 1, 0, 0, 0, 0, false, 0, 0, null, SummonDisplayType.NONE, summoned))
                {
                    if (summoned[0] != null)
                    {
                        var monster = (BeActor)summoned[0];
                        monster.SetPosition(point);

                        if (handle1 != null)
                        {
                            handle1.Remove();
                            handle1 = null;
                        }
                        handle1 = monster.RegisterEventNew(BeEventType.onHit, args =>
                        //var handle1 = monster.RegisterEvent(BeEventType.onHit, args =>
                        {
                            hitMonster = monster;
                        });
                        //handleList.Add(handle1);
                        var handle2 = monster.RegisterEventNew(BeEventType.onDead, args =>
                        {
                            if (monster == hitMonster)
                                hitMonster = null;

                            ++deathCount;
                            if (deathCount >= summonCount)
                            {
                                ((BeActorStateGraph)owner.GetStateGraph()).ExecuteNextPhaseSkill();
                            }
                        });
                        handleList.Add(handle2);
                        monsterList.Add(monster);
                    }
                }
            });
        }
    }

    void CheckResult()
    {
        result = false;
        for (int i = 0; i < monsterList.Count; i++)
        {
            if (monsterList[i] != null && !monsterList[i].IsDead())
            {
                result = true;
                var monster = monsterList[i];
                monster.UseSkill(skillId, true);
                monster.delayCaller.DelayCall(GlobalLogic.VALUE_3000, () =>
                {
                    if (monster == hitMonster)
                        monster.m_pkGeActor.SetHPDamage(int.MaxValue);
                    monster.GetEntityData().SetHP(-1);
                    monster.DoDead();
                });
            }
        }
        monsterList.Clear();

        if (result)//场上还有哥布林活着
        {
            owner.buffController.TryAddBuff(buffId, GlobalLogic.VALUE_2000);
        }
    }

    public override void OnUpdate(int iDeltime)
    {
        if (owner == null)
            return;

        if (curPhase == 1)
        {
            timer -= iDeltime;
            if (timer <= 0)
            {
                owner.m_pkGeActor.ChangeAction("Anim_Idle", 0.25f);
                owner.ResetMoveCmd();
                owner.aiManager.Start();
                owner.ChangeRunMode(false);

                ((BeActorStateGraph)owner.GetStateGraph()).ExecuteNextPhaseSkill();
            }
        }
        else if (curPhase == 3 && !result)
        {
            ((BeActorStateGraph)owner.GetStateGraph()).ExecuteNextPhaseSkill();
        }
        else if (curPhase == 4 && result)
        {
            ((BeActorStateGraph)owner.GetStateGraph()).ExecuteNextPhaseSkill();
        }
    }

    public override void OnCancel()
    {
        Release();
    }

    public override void OnFinish()
    {
        Release();
    }

    void Release()
    {
        hitMonster = null;
        monsterList.Clear();
        RemoveBuff();
        for (int i = 0; i < handleList.Count; i++)
        {
            handleList[i].Remove();
            handleList[i] = null;
        }
        handleList.Clear();
    }

    void RemoveBuff()
    {
        var buff = owner.buffController.HasBuffByID((int)GlobalBuff.INVINCIBLE_WHITE);
        if (buff != null)
        {
            owner.buffController.RemoveBuff(buff);
        }
    }
}
