using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 在被击者的位置有概率随机召唤一个实体
/// </summary>
public class Mechanism2084 : BeMechanism
{
    /// <summary>
    /// 实体数据
    /// </summary>
    private struct SummonEntityData
    {
        public int EntityId;
        public int HurtId;
        public int DelayAttackTime;     //第一次延时攻击时间
        public int RepeatAttackTime;    //重复攻击时间间隔
        public int RepeatAttackCount;   //重复攻击次数
    }

    public Mechanism2084(int id, int level) : base(id, level) { }

    private int m_SummonRate = 0; //召唤概率（千分比）
    private List<SummonEntityData> m_SummonDataList = new List<SummonEntityData>();

    public override void OnInit()
    {
        m_SummonDataList.Clear();
        for (int i = 0; i < data.ValueA.Count; i++)
        {
            SummonEntityData summonEntityData = new SummonEntityData();
            summonEntityData.EntityId = TableManager.GetValueFromUnionCell(data.ValueA[i], level);
            summonEntityData.HurtId = TableManager.GetValueFromUnionCell(data.ValueB[i], level);
            summonEntityData.DelayAttackTime = TableManager.GetValueFromUnionCell(data.ValueC[i], level);
            summonEntityData.RepeatAttackTime = TableManager.GetValueFromUnionCell(data.ValueE[i], level);
            summonEntityData.RepeatAttackCount = TableManager.GetValueFromUnionCell(data.ValueF[i], level);
            m_SummonDataList.Add(summonEntityData);
        }

        m_SummonRate = TableManager.GetValueFromUnionCell(data.ValueD[0], level);
    }

    public override void OnStart()
    {
        base.OnStart();
        handleA = OwnerRegisterEventNew(BeEventType.onHitOther, HitOther);
        //handleA = owner.RegisterEvent(BeEventType.onHitOther, HitOther);
    }

    /// <summary>
    /// 攻击到目标
    /// </summary>
    /// <param name="args"></param>
    private void HitOther(GameClient.BeEvent.BeEventParam param)
    {
        //过滤召唤出来的实体攻击
        //if(args.Length > 6)
        //{
            var proj = param.m_Obj2 as BeProjectile;
            if (proj != null)
            {
                if (CheckSummonProjectileId(proj.m_iResID))
                {
                    return;
                }
            }
        //}

        //召唤实体 附带的触发效果攻击
        int hurtId = param.m_Int;
        if (CheckSummonHurtId(hurtId))
        {
            return;
        }
        
        BeActor target = param.m_Obj as BeActor;
        if (target == null)
            return;

        TryCreateEntity(target.GetPosition(), target);
    }

    /// <summary>
    /// 按概率，随机生成怪物，受拥有者召唤数量上限限制
    /// </summary>
    private void TryCreateEntity(VInt3 pos,BeActor target)
    {
        // 根据概率判断是否触发
        int random = FrameRandom.Random((uint)GlobalLogic.VALUE_1000);
        if (m_SummonRate < random)
            return;

        //随机一个实体
        var randomIndex = FrameRandom.Random((uint)m_SummonDataList.Count);
        var entityData = m_SummonDataList[randomIndex];

        owner.AddEntity(entityData.EntityId, pos);
        if (entityData.DelayAttackTime > 0)
        {
            //为了和动画匹配 需要延时造成伤害
            owner.delayCaller.DelayCall(entityData.DelayAttackTime, () => 
            {
                if(owner!=null && !owner.IsDead() && target!=null && !target.IsDead())
                {
                    owner.DoAttackTo(target, entityData.HurtId);
                }
            });

            //延时重复伤害
            if (entityData.RepeatAttackCount>0)
            {
                for(int i=0;i< entityData.RepeatAttackCount; i++)
                {
                    int time = entityData.DelayAttackTime + entityData.RepeatAttackTime * (i + 1);
                    owner.delayCaller.DelayCall(time, ()=> 
                    {
                        if (owner != null && !owner.IsDead() && target != null && !target.IsDead())
                        {
                            owner.DoAttackTo(target, entityData.HurtId);
                        }
                    });
                }
            }
        }
        else
        {
            owner.DoAttackTo(target, entityData.HurtId);
        }
    }

    /// <summary>
    /// 过滤自己召唤出来的实体造成的伤害
    /// </summary>
    private bool CheckSummonProjectileId(int resId)
    {
        for(int i=0;i< m_SummonDataList.Count; i++)
        {
            if(m_SummonDataList[i].EntityId == resId)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 过滤自己触发效果造成伤害
    /// </summary>
    /// <returns></returns>
    private bool CheckSummonHurtId(int hurtId)
    {
        for (int i = 0; i < m_SummonDataList.Count; i++)
        {
            if (m_SummonDataList[i].HurtId == hurtId)
            {
                return true;
            }
        }
        return false;
    }
}

