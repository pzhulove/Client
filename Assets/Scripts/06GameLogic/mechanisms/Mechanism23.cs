using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameClient;

/*
 * 杀意波动
*/

public class Mechanism23 : BeMechanism
{
    protected string effectName = "";           //特效路径
    VInt radius = VInt.one.i * 2;               //范围半径
    protected int hurtID = 0;                   //触发效果ID
    protected int friendBuffID = 0;             //友军添加Buff
    protected int enemyBuffID = 0;              //敌军添加Buff
    protected int hurtCheckInterval = 0;        //伤害时间间隔
    protected bool normalHit = false;           //走正常攻击流程
    protected VInt minPos = 0;
    protected VInt maxPos = 0;
    int timeAcc = 0;
    readonly int checkInterval = 200;
    int timeAcc2 = 0;
    bool onlyHurtOnce = false;                  //只造成一次伤害
    List<BeActor> attackActorList = new List<BeActor>();       //伤害列表
    protected List<BeActor> inRangers = new List<BeActor>();

    public Mechanism23(int mid, int lv) : base(mid, lv){}

    public override void OnReset()
    {
        effectName = "";
        radius = VInt.one.i * 2;
        hurtID = 0;
        friendBuffID = 0;  
        enemyBuffID = 0;  
        hurtCheckInterval = 0;
        normalHit = false; 
        minPos = 0;
        maxPos = 0;
        timeAcc = 0;
        timeAcc2 = 0;
        onlyHurtOnce = false; 
        attackActorList.Clear();
        inRangers.Clear();
    }

    public static void MechanismPreloadRes(ProtoTable.MechanismTable tableData)
    {
#if !LOGIC_SERVER
        PreloadManager.PreloadEffectID(TableManager.GetValueFromUnionCell(tableData.ValueB[0], 1), null, null);
#endif
    }

    public override void OnInit()
    {
        if (data.StringValueA.Count > 0)
        {
            effectName = data.StringValueA[0];
        }
        if (data.ValueA.Count > 0)
        {
            radius = VInt.NewVInt(TableManager.GetValueFromUnionCell(data.ValueA[0], level), GlobalLogic.VALUE_1000);
        }
        if (data.ValueB.Count > 0)
        {
            hurtID = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        }
        if (data.ValueC.Count > 0)
        {
            friendBuffID = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
        }
        if (data.ValueD.Count > 0)
        {
            enemyBuffID = TableManager.GetValueFromUnionCell(data.ValueD[0], level);
        }
        if (data.ValueE.Count > 0)
        {
            hurtCheckInterval = TableManager.GetValueFromUnionCell(data.ValueE[0], level);
        }
        if (data.ValueF.Count > 0)
        {
            int value = TableManager.GetValueFromUnionCell(data.ValueF[0], level);
            if (value > 0)
            {
                normalHit = true;
            }
        }

        if (data.ValueG.Count > 1)
        {
            minPos = VInt.Float2VIntValue(TableManager.GetValueFromUnionCell(data.ValueG[0], level) / 1000.0f);
            maxPos = VInt.Float2VIntValue(TableManager.GetValueFromUnionCell(data.ValueG[1], level) / 1000.0f);
        }

        if (data.ValueH.Count > 0)
            onlyHurtOnce = TableManager.GetValueFromUnionCell(data.ValueH[0], level) == 1 ? true : false;
    }

    public override void OnStart()
    {
        attackActorList.Clear();
        int r1 = GlobalLogic.VALUE_1000;
        int r2 = GlobalLogic.VALUE_1000;
        int intervalRate = GlobalLogic.VALUE_1000;

        for (int i = 0; i < owner.MechanismList.Count; i++)
        {
            var m = owner.MechanismList[i] as Mechanism116;
            if (m == null)
                continue;
            if (m.impactMechanismIdList.Contains(mechianismID))
                continue;
            r1 += m.radiusRate;
            if (m.isChangeEffectSize)
                r2 += m.radiusRate;
            intervalRate += m.intervalRate;
        }

        if (r1 == GlobalLogic.VALUE_1000 && r2 == GlobalLogic.VALUE_1000 && intervalRate == GlobalLogic.VALUE_1000)
        {
            var actor = owner.GetOwner() as BeActor;
            if (actor != null)
            {
                for (int i = 0; i < actor.MechanismList.Count; i++)
                {
                    var m = actor.MechanismList[i] as Mechanism116;
                    if (m == null)
                        continue;
                    if (!m.impactMechanismIdList.Contains(mechianismID))
                        continue;
                    r1 += m.radiusRate;
                    if (m.isChangeEffectSize)
                        r2 += m.radiusRate;
                    intervalRate += m.intervalRate;
                }
            }
        }

        radius = radius.i * VFactor.NewVFactor(r1, GlobalLogic.VALUE_1000);
        VInt effectRadius = radius.i * VFactor.NewVFactor(r2, GlobalLogic.VALUE_1000);
        hurtCheckInterval *= VFactor.NewVFactor(intervalRate, GlobalLogic.VALUE_1000);

#if !LOGIC_SERVER
        if (!string.IsNullOrEmpty(effectName))
        {
            //设置特效大小
            if (data.ValueA[0].valueType != ProtoTable.UnionCellType.union_fix)
            {
                var eff = owner.m_pkGeActor.FindEffect(effectName);
                if (eff != null)
                {
                    float initV = TableManager.GetValueFromUnionCell(data.ValueA[0], 1) / (float)(GlobalLogic.VALUE_1000);
                    float add = (effectRadius.scalar - initV) / initV;

                    eff.SetScale(1f + add);
                }
            }
        }
#endif
        CheckRange();
        CheckAttack();
    }

    public override void OnUpdate(int deltaTime)
    {
        if (hurtCheckInterval > 0)
        {
            UpdateCheckRange(deltaTime);
            UpdateCheckHurt(deltaTime);
        }
    }

    void UpdateCheckHurt(int deltaTime)
    {
        timeAcc2 += deltaTime;
        if (timeAcc2 > hurtCheckInterval)
        {
            timeAcc2 -= hurtCheckInterval;
            CheckAttack();
        }
    }

    void UpdateCheckRange(int deltaTime)
    {
        timeAcc += deltaTime;
        if (timeAcc > checkInterval)
        {
            timeAcc -= checkInterval;

            CheckRange();
        }
    }

    protected void CheckAttack()
    {
        for (int i = 0; i < inRangers.Count; ++i)
        {
            var actor = inRangers[i];
            DoAttack(actor);
        }
    }

    void CheckRange()
    {
        if (owner.CurrentBeScene == null) return;
        List<BeActor> targets = GamePool.ListPool<BeActor>.Get();

        var pos = owner.GetPosition();

        pos.z = 0;

        owner.CurrentBeScene.FindActorInRange(targets, pos, radius);
        FilterHeight(targets);
        for (int i = 0; i < inRangers.Count; ++i)
        {
            if (!targets.Contains(inRangers[i]))
            {
                OutRange(inRangers[i]);
                inRangers.RemoveAt(i);
                i--;
            }
        }

        for (int i = 0; i < targets.Count; ++i)
        {
            if (!inRangers.Contains(targets[i]))
            {
                inRangers.Add(targets[i]);
                InRange(targets[i]);
            }
        }
        GamePool.ListPool<BeActor>.Release(targets);
    }

    private void FilterHeight(List<BeActor> list)
    {
        if (minPos == 0 || maxPos == 0) return;
        list.RemoveAll(x =>
        {
            if (x.m_cpkCurEntityActionFrameData == null || x.m_cpkCurEntityActionFrameData.pDefenseData == null || x.m_cpkCurEntityActionFrameData.pDefenseData.vBox.Count <= 0)
            {
                return true;
            }
            VInt yMax = x.m_cpkCurEntityActionFrameData.pDefenseData.vBox[0].vBox._max.y + x.GetPosition().z;
            VInt yMin = x.m_cpkCurEntityActionFrameData.pDefenseData.vBox[0].vBox._min.y + x.GetPosition().z;
            return !(yMin <= maxPos && yMax >= minPos);
        });
    }


    //改变攻击时间间隔
    public void SetAttackTimeAcc(VFactor rate)
    {
        VFactor rrate = new VFactor(rate.den, rate.nom);
        hurtCheckInterval = (rrate * hurtCheckInterval).integer;
    }

    //造成伤害
    protected void DoAttack(BeActor actor)
    {
        if (onlyHurtOnce)
        {
            if (attackActorList.Contains(actor))
                return;
            else
                attackActorList.Add(actor);
        }

        if (!actor.IsDead() && actor.GetCamp() != owner.GetCamp() && actor.GetLifeState() == (int)EntityLifeState.ELS_ALIVE)
        {
            BeEntity attackActor = owner.GetOwner();
            if (actor.stateController.CanBeHit())
            {
                bool triggerFlash = normalHit ? true : false;
                AttackResult result = attackActor.DoAttackTo(actor, hurtID, triggerFlash);
                if (normalHit && result != AttackResult.MISS)
                {
                    var hurtData = TableManager.GetInstance().GetTableItem<ProtoTable.EffectTable>(hurtID);
                    if (hurtData != null)
                    {
                        if (hurtData.HitGrab && !actor.IsGrabed())
                            return;
                        if (actor.OnDamage())
                            return;
                        if (hurtData.HasDamage == 0)
                            return;
                        int skillLevel = 1;
                        BeEntityData data = attackActor.GetEntityData();
                        if (data != null)
                        {
                            skillLevel = data.GetSkillLevel(hurtData.SkillID);
                        }

                        var hitPos = actor.GetPosition();
                        hitPos.z += VInt.one.i;
                        owner.DealHit(hurtID, actor, hitPos, hurtData, skillLevel, false);
                    }
                }
            }
        }
    }

    public override void OnFinish()
    {
        for (int i = 0; i < inRangers.Count; ++i)
        {
            OutRange(inRangers[i]);
        }

        inRangers.Clear();
    }

    void InRange(BeActor actor)
    {
        //友
        if (actor.GetCamp() == owner.GetCamp())
        {
            if (friendBuffID != 0)
                actor.buffController.TryAddBuff(friendBuffID, int.MaxValue, level);
        }
        //敌
        else
        {
            if (enemyBuffID != 0)
                actor.buffController.TryAddBuff(enemyBuffID, int.MaxValue, level);
        }
    }

    void OutRange(BeActor actor)
    {
        //友
        if (actor.GetCamp() == owner.GetCamp())
            actor.buffController.RemoveBuff(friendBuffID);
        //敌
        else
        {
            actor.buffController.RemoveBuff(enemyBuffID);
        }
    }
}
