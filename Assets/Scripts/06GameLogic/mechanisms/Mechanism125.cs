using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 邪光波动阵机制
/// </summary>
public class Mechanism125 : BeMechanism
{
    protected string effectName = "";           //特效路径
    VInt radius = VInt.one.i * 2;               //范围半径
    protected int hurtID = 0;                   //触发效果ID
    protected int hurtCheckInterval = 0;        //伤害时间间隔
    int timeAcc2 = 0;
    protected List<BeActor> inRangers = new List<BeActor>();
    int buffInfoID = 0;
    int magicHurtID = 0;
    int delayTime = 0;
    readonly int maxEffectNum = 7;
    readonly int controlBuffInfoPVE = 1704202;
    readonly int controlBuffInfoPVP = 1704203;

    bool isPVP = false;

    int cnt = 0;
    int time = 0;
    bool flag = false;
    public Mechanism125(int mid, int lv) : base(mid, lv) {}

    public override void OnReset()
    {
        effectName = "";
        radius = VInt.one.i * 2;
        hurtID = 0; 
        hurtCheckInterval = 0;
        timeAcc2 = 0;
        inRangers.Clear();
        buffInfoID = 0;
        magicHurtID = 0;
        delayTime = 0;
        isPVP = false;
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
            hurtCheckInterval = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
        }

        if (data.ValueD.Count > 0)
        {
            buffInfoID = TableManager.GetValueFromUnionCell(data.ValueD[0], level);
        }

        if (data.ValueE.Count > 0)
        {
            magicHurtID = TableManager.GetValueFromUnionCell(data.ValueE[0], level);
        }

        if (data.ValueF.Count > 0)
        {
            delayTime = TableManager.GetValueFromUnionCell(data.ValueF[0], level);
        }
    }

    public override void OnStart()
    {
        cnt = 0;
        time = 0;
        flag = false;
        isPVP = BattleMain.IsModePvP(owner.battleType);
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
                    float add = (radius.scalar - initV) / initV;

                    eff.SetScale(1f + add);
                }
            }
        }
#endif
        CheckRange();
        CheckAttack(true);
    }

    
    public override void OnUpdate(int deltaTime)
    {
        if (hurtCheckInterval > 0)
        {
            UpdateCheckHurt(deltaTime);
        }

        if (!flag)
        {
            time += deltaTime;
            if (time >= delayTime)
            {
                OnHurtEntity();
                flag = true;
            }
        }
    }

    /// <summary>
    ///添加控制BUFF
    /// </summary>
    private void OnHurtEntity()
    {
        for (int i = 0; i < inRangers.Count; ++i)
        {
            var actor = inRangers[i];
            var hitPos = actor.GetPosition();
            hitPos.z += VInt.one.i;
            if (!actor.IsDead() && actor.GetCamp() != owner.GetCamp() && actor.GetLifeState() == (int)EntityLifeState.ELS_ALIVE)
            {
               
                actor.buffController.RemoveBuff(isPVP?170422:170421);
                BeEntity o = owner.GetOwner();
                if (o != null)
                {
                    o._onHurtEntity(actor, hitPos, magicHurtID);
                }
                             
            }
        }
    }

    /// <summary>
    /// 攻击间隔
    /// </summary>
    /// <param name="deltaTime"></param>
    void UpdateCheckHurt(int deltaTime)
    {
        timeAcc2 += deltaTime;
        if (timeAcc2 > hurtCheckInterval)
        {
            timeAcc2 -= hurtCheckInterval;
            CheckAttack();
        }
    }

    protected void CheckAttack(bool showEffect = false)
    {
        for (int i = 0; i < inRangers.Count; ++i)
        {
            var actor = inRangers[i];
            DoAttack(actor, showEffect);
        }
    }

    void CheckRange()
    {
        var pos = owner.GetPosition();
        pos.z = 0;
        owner.CurrentBeScene.FindActorInRange(inRangers, pos, radius);
        inRangers.RemoveAll(x =>
        {
            return !x.stateController.CanBeHit() || !x.stateController.CanBeTargeted();
        });
    }

    //造成伤害
    protected void DoAttack(BeActor actor, bool showEffect = false)
    {
        if (!actor.IsDead() && actor.GetCamp() != owner.GetCamp() && actor.GetLifeState() == (int)EntityLifeState.ELS_ALIVE)
        {
            if (showEffect)
            {
                if (cnt < maxEffectNum)
                {
                    cnt++;
                    actor.buffController.TryAddBuff(isPVP ? controlBuffInfoPVP : controlBuffInfoPVE);
                }
                actor.buffController.TryAddBuff(buffInfoID, owner);
            }
            BeEntity attackActor = owner.GetOwner();
            if (actor.stateController.CanBeHit())
            {
                attackActor.DoAttackTo(actor, hurtID);

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
                    BeEntityData data = owner.GetEntityData();
                    if (data != null)
                    {
                        skillLevel = data.GetSkillLevel(hurtData.SkillID);
                    }
                    var hitPos = actor.GetPosition();
                    hitPos.z += VInt.one.i;
                    attackActor.DealHit(hurtID, actor, hitPos, hurtData, skillLevel, false);
                }

            }
        }
    }

    public override void OnFinish()
    {
        inRangers.Clear();
    }


}
