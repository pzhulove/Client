using System.Collections;
using System.Collections.Generic;
using GameClient;
using UnityEngine;

/// <summary>
/// 异常Buff类
/// </summary>

//记录异常Buff数据
public struct AbnormalBuffData
{
    public bool isFinish;                  //当前异常Buff是否已经结束
    public bool isFirst;                   //是第一个添加的异常Buff
    public int curAbnormalTimeAcc;         //当前异常Buff的更新频率
    public int lastDamageAcc;              //最后一次造成异常伤害的时间
}


public class BeAbnormalBuff : BeBuff
{
    protected int abnormalAttack = 0;
    //protected object[] interParams = new object[] { null, new int[] {0} };

    public override void OnReset()
    {
        abnormalAttack = 0;
    }
    public BeAbnormalBuff(int bi, int buffLevel, int buffDuration, int attack = 0, bool buffEffectAni = true) : base(bi, buffLevel, buffDuration, attack, buffEffectAni)
    {
    }

    public override void OnUpdate(int delta)
    {
        base.OnUpdate(delta);
        UpdateAbnormalDamage(delta);
    }

    public override void OnFinish()
    {
        base.OnFinish();
        ChangeAbnormalData();
    }

    public override int GetAloneAbnormalDamage()
    {
        if (abnormalAttack > 0)
            return abnormalAttack;
        abnormalAttack = GetAbnromalDamage(buffAttack, duration);
        return abnormalAttack;
    }

    //Buff移除时改变异常伤害Buff数据
    private void ChangeAbnormalData()
    {
        if (abnormalBuffData.isFinish || !abnormalBuffData.isFirst)
            return;
        abnormalBuffData.isFinish = true;
        abnormalBuffData.isFirst = false;
        BeBuff buff = owner.buffController.GetBuffButSelf(this, buffID);
        if (buff != null)
        {
            buff.abnormalBuffData.isFirst = true;
        }
        BeBuff firstAbnormalBuff = owner.buffController.GetFirstAbnormalBuff(buffID);
        if (firstAbnormalBuff == null)
            return;
        if (abnormalBuffData.lastDamageAcc != 0)
        {
            firstAbnormalBuff.abnormalBuffData.curAbnormalTimeAcc = duration - abnormalBuffData.lastDamageAcc;          //Buff时间内有造成伤害
        }
        else
        {
            firstAbnormalBuff.abnormalBuffData.curAbnormalTimeAcc = abnormalBuffData.curAbnormalTimeAcc;   //Buff时间内没有造成伤害
        }
    }

 
    //更新异常buff伤害
    private void UpdateAbnormalDamage(int delta)
    {
        if (!abnormalBuffData.isFirst)
            return;
        if (buffData.TriggerInterval <= 0)
            return;
        if (abnormalBuffData.curAbnormalTimeAcc >= buffData.TriggerInterval)
        {
            int damage = owner.buffController.GetAbnormalDamage(buffID);
            if (damage <= 0)
                return;

            /*interParams[0] = this;
            var subParams = interParams[1] as int[];
            subParams[0] = damage;
            owner.TriggerEvent(BeEventType.AbnormalBuffHurt, interParams);
            damage = subParams[0];*/
            
            var ret = owner.TriggerEventNew(BeEventType.AbnormalBuffHurt, new EventParam(){m_Obj = this,m_Int = damage});
            damage = ret.m_Int;
            
            if(owner.CurrentBeScene!=null)
                owner.CurrentBeScene.TriggerEventNew(BeEventSceneType.onHurtByAbnormalBuff, new EventParam(){m_Int = damage, m_Obj = releaser, m_Obj2 = owner, m_Int2 = skillId});
                //owner.CurrentBeScene.TriggerEvent(BeEventSceneType.onHurtByAbnormalBuff, new object[] { damage,releaser, owner, skillId});
            DoBuffAttack(damage);
            abnormalBuffData.curAbnormalTimeAcc = 0;
            if (duration > runTime)
                abnormalBuffData.lastDamageAcc = runTime;
        }
        else
        {
            abnormalBuffData.curAbnormalTimeAcc += delta;
        }
    }
}
