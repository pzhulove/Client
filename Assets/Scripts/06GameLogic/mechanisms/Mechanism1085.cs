using System;
using System.Collections.Generic;
using GameClient;

//20191025医疗波装备机制
public class Mechanism1085 : BeMechanism
{
    private string chainEffect;
    private int chainMaxCount;
    private int buffInfoID;
    private int effectID;
    private VInt chainMaxDistance = VInt.one.i * 5;

    private readonly int attackDuration = GlobalLogic.VALUE_300;

    List<ChainTarget> doAttackTarget = new List<ChainTarget>();
    List<BeActor> effectTarget = new List<BeActor>();
    private int chainCount = 0;
    List<BeEntity> mEntitys = new List<BeEntity>();

    class ChainTarget
    {
        public BeActor target;
        public BeEvent.BeEventHandleNew handle;
        public bool isDead;
    }

    public Mechanism1085(int mid, int lv) : base(mid, lv) { }

    public override void OnInit()
    {
        chainEffect = data.StringValueA[0];
        chainMaxCount = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        buffInfoID = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        effectID = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
    }

    public override void OnReset()
    {
        doAttackTarget.Clear();
        effectTarget.Clear();
        chainCount = 0;
        mEntitys.Clear();
        startChain = false;
        fromActor = null;
        toActor = null;
        chainTimer = 0;
    }

    public override void OnStart()
    {
        chainCount = 0;
        if(mEntitys == null)
        {
            mEntitys = new List<BeEntity>();
        }
        else
        {
            mEntitys.Clear();
        }
        if (owner != null)
        {
            doAttackTarget.Clear();
            effectTarget.Clear();
            owner.CurrentBeScene.GetEntitys2(mEntitys);

            effectTarget.Add(owner);
            var target = FindNearestRangeTarget(owner.GetPosition(), chainMaxDistance, effectTarget);
            if(target != null)
            {
                startChain = true;
                fromActor = owner;
                toActor = target;
            }
            
        }
    }

    bool startChain = false;
    BeActor fromActor = null;
    BeActor toActor = null;
    int chainTimer = 0;
    public override void OnUpdate(int deltaTime)
    {
        
        if (startChain) 
        {
            chainTimer -= deltaTime;
            if(chainTimer > 0)
            {
                return;
            }
            if (fromActor != null && toActor != null)
            {
                Chain(fromActor, toActor);
                fromActor = toActor ;
                toActor = FindNearestRangeTarget(fromActor.GetPosition(), chainMaxDistance, effectTarget);
                chainTimer = attackDuration;
            }
        }
    }

    public override void OnFinish()
    {
        chainCount = 0;
        Clear();
    }

    private void Chain(BeActor fromActor,BeActor toActor)
    {
        chainCount++;
        if (chainCount > chainMaxCount)
        {
            return;
        }
        //链结特效
        CreateChainEffect(fromActor, toActor);
        effectTarget.Add(toActor);
        
        if (owner.GetCamp() == toActor.GetCamp())
        {
            toActor.buffController.TryAddBuffInfo(buffInfoID, owner, 0);
        }
        else
        {
            ChainTarget unit = new ChainTarget();
            unit.target = toActor;
            unit.isDead = false;

            //保证目标在链接阶段不死亡
            unit.handle = toActor.RegisterEventNew(BeEventType.onDead, eventParam => {

                var deadActor = eventParam.m_Obj as BeActor;
                SetUnitDead(deadActor);

                bool isForce = eventParam.m_Bool2;
                if (!isForce)
                {
                    eventParam.m_Bool = false;
                }
            });
            doAttackTarget.Add(unit);
            //造成伤害
            var hitPos = toActor.GetPosition();
            hitPos.z += VInt.one.i;
            owner._onHurtEntity(toActor, hitPos, effectID);
        }
    }

    /// <summary>
    /// 创建特效连线
    /// </summary>
    private void CreateChainEffect(BeActor fromActor, BeActor toActor)
    {
#if !LOGIC_SERVER
        if (fromActor == null || toActor == null || fromActor.m_pkGeActor == null)
            return;
        fromActor.m_pkGeActor.CreateChainEffect(toActor, chainEffect);
#endif
    }

    /// <summary>
    /// 清除特效连线
    /// </summary>
    private void ClearChainEffect(BeActor actor)
    {
#if !LOGIC_SERVER
        if (actor == null || actor.m_pkGeActor == null)
            return;
        actor.m_pkGeActor.ClearChainEffect();
#endif
    }

    private void SetUnitDead(BeActor actor)
    {
        if(doAttackTarget != null)
        {
            for(int i = 0; i < doAttackTarget.Count; ++i)
            {
                if (doAttackTarget[i].target.GetPID() == actor.GetPID())
                {
                    doAttackTarget[i].isDead = true;
                }
            }
        }
    }

    protected void Clear()
    {
        for(int i = 0; i < effectTarget.Count; ++i)
        {
            ClearChainEffect(effectTarget[i]);
        }
        effectTarget.Clear();
        for(int i = 0; i < doAttackTarget.Count; ++i)//此躺for循环有continue，请勿随意修改执行顺序
        {
            if (doAttackTarget[i].handle != null)
            {
                doAttackTarget[i].handle.Remove();
            }
            if (doAttackTarget[i].target != null)
            {
                if (doAttackTarget[i].target.GetLifeState() != (int)EntityLifeState.ELS_ALIVE)
                    continue;
                //浮空不直接死
                if (doAttackTarget[i].target.sgGetCurrentState() == (int)ActionState.AS_BUSY && doAttackTarget[i].target.GetPosition().z > 0)
                    continue;
                if (doAttackTarget[i].isDead == true || doAttackTarget[i].target.IsDead() || (doAttackTarget[i].target.GetEntityData() != null && doAttackTarget[i].target.GetEntityData().GetHP() <= 0))
                    doAttackTarget[i].target.DoDead();
            }
        }
        doAttackTarget.Clear();
    }

    private BeActor FindNearestRangeTarget(VInt3 pos, VInt radius, List<BeActor> inList = null)
    {
        BeActor target = null;
        int minY = int.MaxValue;
        int minX = int.MaxValue;

        VInt2 center = new VInt2(pos.x, pos.y);

        for (int i = 0; i < mEntitys.Count; ++i)
        {
            if ((mEntitys[i] as BeActor) == null || mEntitys[i].IsDead())
                continue;

            if ((mEntitys[i] as BeActor).IsSkillMonster()) //技能实现的怪物
                continue;

            if ((mEntitys[i] as BeActor).IsSummonMonster()) //召唤的怪物
                continue;

            if (!mEntitys[i].stateController.CanBeTargeted() && (mEntitys[i] as BeActor).GetCamp() != owner.GetCamp()) //只有地方区分是否可被攻击
                continue;

            if (inList != null && inList.Contains((mEntitys[i] as BeActor)))
                continue;

            var entityPos = mEntitys[i].GetPosition();
            VInt2 point = new VInt2(entityPos.x, entityPos.y);

            int distance = (center - point).magnitude;
            if (distance <= radius && Math.Abs(center.y - point.y) <= minY)
            {
                if (Math.Abs(center.y - point.y) != minY ||
                    Math.Abs(center.y - point.y) == minY && Math.Abs(center.x - point.x) < minX) 
                {
                    minY = Math.Abs(center.y - point.y);
                    minX = Math.Abs(center.x - point.x);
                    target = mEntitys[i] as BeActor;
                }
            }
        }

        return target;
    }
}

