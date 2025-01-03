using ProtoTable;
using System.Collections.Generic;
using GameClient;

//光之复仇 普攻或者技能命中目标(主动)或者受到魔法伤害(被动) 在目标位置概率召唤一道闪电
public class Mechanism2015 : BeMechanism
{
    public Mechanism2015(int mid, int lv) : base(mid, lv) { }

    private int[] entityIdArr = new int[2];       //光之复仇闪电实体ID(攻击|被击)
    private int[] summonRateArr = new int[3] { 1000, 1000 , 1000};   //召唤闪电概率 千分比(主动、魔法、物理)
    private List<int> skillIdList = new List<int>();    //监听的技能ID列表 
    
    private int curSummonRayCount = 0;  //当前主动攻击的落雷次数
    private readonly int timeAcc = 66;
    private int curTimeAcc = 0;

    public static void MechanismPreloadRes(MechanismTable tableData)
    {
#if !LOGIC_SERVER
        PreloadManager.PreloadResID(TableManager.GetValueFromUnionCell(tableData.ValueA[0],1), null, null);
        PreloadManager.PreloadResID(TableManager.GetValueFromUnionCell(tableData.ValueA[1], 1), null, null);
#endif
    }

    public override void OnInit()
    {
        base.OnInit();
        entityIdArr[0] = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        entityIdArr[1] = TableManager.GetValueFromUnionCell(data.ValueA[1], level);
        summonRateArr[0] = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        summonRateArr[1] = TableManager.GetValueFromUnionCell(data.ValueB[1], level);
        summonRateArr[2] = TableManager.GetValueFromUnionCell(data.ValueB[2], level);
        for (int i = 0; i < data.ValueC.Count; i++)
        {
            skillIdList.Add(TableManager.GetValueFromUnionCell(data.ValueC[i], level));
        }
    }

    public override void OnReset()
    {
        skillIdList.Clear();
        curSummonRayCount = 0;
        curTimeAcc = 0;
    }

    public override void OnStart()
    {
        SetEquipAdd();
        handleA = OwnerRegisterEventNew(BeEventType.onHitOther, args =>
        //handleA = owner.RegisterEvent(BeEventType.onHitOther, (object[] args) =>
        {
            int skillId = args.m_Int2;
            BeActor target = args.m_Obj as BeActor;
            CheckAttackHit(skillId, target);
        });

        handleB = owner.RegisterEventNew(BeEventType.onBeHitAfterFinalDamage, args =>
        //handleB = owner.RegisterEvent(BeEventType.onBeHitAfterFinalDamage, (object[] args) =>
        {
            bool triggerFlash = args.m_Bool2;
            if (!triggerFlash)
                return;
            int hurtId = args.m_Int2;
            if (hurtId == 37034)        //光之复仇的伤害不会再触发
                return;
            BeActor attacker = args.m_Obj as BeActor;
            EffectTable effectData = TableManager.instance.GetTableItem<EffectTable>(hurtId);
            CheckBeHit(effectData, attacker);
        });
    }

    public override void OnUpdate(int deltaTime)
    {
        base.OnUpdate(deltaTime);
        UpdateTimeAcc(deltaTime);
    }

    /// <summary>
    /// 刷新释放CD时间
    /// </summary>
    /// <param name="deltaTime"></param>
    private void UpdateTimeAcc(int deltaTime)
    {
        if (curSummonRayCount == 0)
            return;
        if (curTimeAcc >= timeAcc)
        {
            curTimeAcc = 0;
            curSummonRayCount = 0;
        }
        else
            curTimeAcc += deltaTime;
    }

    //检测攻击命中
    private void CheckAttackHit(int skillId, BeActor actor)
    {
        if (owner == null || owner.GetEntityData() == null)
            return;
        if (curSummonRayCount >= 3)
            return;
        curSummonRayCount += 1;

        int sourceId = BeUtility.GetComboSkillId(owner, skillId);

        if (skillIdList.Contains(skillId) || sourceId == owner.GetEntityData().normalAttackID)
        {
            SummonRay(summonRateArr[0], actor, skillId, entityIdArr[0]);
        }
    }

    //检测被击是否属于魔法伤害
    private void CheckBeHit(EffectTable effectData, BeActor actor)
    {
        if (effectData == null)
            return;
        if (effectData.DamageType == EffectTable.eDamageType.MAGIC)
        {
            SummonRay(summonRateArr[1], actor, effectData.SkillID, entityIdArr[1]);
        }
        else if (effectData.DamageType == EffectTable.eDamageType.PHYSIC)
        {
            SummonRay(summonRateArr[2], actor, effectData.SkillID, entityIdArr[1]);
        }
    }

    //召唤闪电
    private void SummonRay(int rate, BeActor target, int skillId,int entityId)
    {
        if (target == null)
            return;
        int ran = FrameRandom.InRange(0, GlobalLogic.VALUE_1000);
        if (ran > rate)
            return;
        VInt3 summonPos = target.GetPosition();
        if (skillId == 3705)
        {
            //对胜利之矛进行特殊处理
            owner.delayCaller.DelayCall(66, () =>
            {
                if (target != null && !target.IsDead())
                    summonPos = target.GetPosition();
                AddEntity(entityId, summonPos);
            });
        }
        else
            AddEntity(entityId, summonPos);
    }

    private void AddEntity(int entityId,VInt3 pos)
    {
        var thisAttachBuff = GetAttachBuff();

        if (thisAttachBuff == null)
            return;
        if (thisAttachBuff.releaser == null || thisAttachBuff.releaser.IsDead())
            return;
        thisAttachBuff.releaser.AddEntity(entityId, pos);
    }

    //装备加成计算
    private void SetEquipAdd()
    {
        List<BeMechanism> list = owner.MechanismList;
        if (list == null)
            return;
        for (int i = 0; i < list.Count; i++)
        {
            var mechanism = list[i] as Mechanism2023;
            if (mechanism == null)
                continue;
            summonRateArr[0] += mechanism.addEntityRate;
            summonRateArr[1] += mechanism.addEntityRate;
        }
    }
}
