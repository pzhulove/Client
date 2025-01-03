using System;
using System.Collections.Generic;
//在被攻击的敌人的位置产生一个友方实体
public class Mechanism1053 : BeMechanism
{
    public Mechanism1053(int mid, int lv) : base(mid, lv)
    {

    }

    int entityId = 0;
    protected int triggerNum = 0;   //机制事件最大触发次数

    protected int curTriggerNum = 0;    //当前机制触发次数

    public override void OnInit()
    {
        curTriggerNum = 0;
        entityId = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        triggerNum = TableManager.GetValueFromUnionCell(data.ValueB[0],level);
        if(triggerNum == 0)
        {
            //默认只能执行一次
            triggerNum = 1;
        }
    }
    public override void OnStart()
    {
        if (owner == null) return;
        curTriggerNum = 0;
        handleA = owner.RegisterEventNew(BeEventType.onBeforeHit, onAttackTarget);
        //handleA = owner.RegisterEvent(BeEventType.onBeforeHit,onAttackTarget);
    }
    private void onAttackTarget(GameClient.BeEvent.BeEventParam param)
    {
        if (owner == null || owner.CurrentBeScene == null) return;
        BeActor target = param.m_Obj as BeActor;
        if (target == null)
            return;
        if (curTriggerNum >= triggerNum && triggerNum != 0)
            return;
        curTriggerNum++;
        var projectile = owner.AddEntity(entityId, VInt3.zero) as BeProjectile;
        if(projectile != null)
        {
            projectile.SetPosition(target.GetPosition());
        }
    }
}

