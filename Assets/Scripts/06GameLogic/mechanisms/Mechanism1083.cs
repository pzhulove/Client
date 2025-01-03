using System;
using System.Collections.Generic;

//20191025火属性攻击不破冰机制
public class Mechanism1083 : BeMechanism
{
    private int buffInfoID;
    private int attachEffectID;
    private bool finishFrozenFlag = true;

    public bool Flag
    {
        get { return finishFrozenFlag; }
    }
    
    public Mechanism1083(int mid, int lv) : base(mid, lv) { }

    public override void OnInit()
    {
        buffInfoID = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        attachEffectID = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        var temp = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
        if(temp == 0)
        {
            finishFrozenFlag = true;
        }
        else
        {
            finishFrozenFlag = false;
        }
    }

    public override void OnStart()
    {
        if(owner != null)
        {
            if(buffInfoID != 0)
            {
                handleA = owner.RegisterEventNew(BeEventType.onBeforeHit, OnBeforeHitEvent);
                //handleA = owner.RegisterEvent(BeEventType.onBeforeHit, OnBeforeHitEvent);
            }
            if(attachEffectID != 0)
            {
                handleB = owner.RegisterEventNew(BeEventType.onAfterFinalDamageNew, OnAfterFinalDamageNewEvent);
                //handleB = owner.RegisterEvent(BeEventType.onAfterFinalDamageNew, OnAfterFinalDamageNewEvent);
            }
        }
    }

    //异常攻击函数
    private void OnBeforeHitEvent(GameClient.BeEvent.BeEventParam param)
    {
        //if (args != null && args.Length >= 3)
        //{
            var hurtId = param.m_Int;
            if(hurtId == attachEffectID)//防止递归
            {
                return;
            }
            var attackElementType = owner.GetEntityData().GetAttackElementType(hurtId);
            var target = param.m_Obj as BeActor;

            if (target != null && attackElementType == MagicElementType.FIRE &&
                target.stateController.HasBuffState(BeBuffStateType.FROZEN))
            {
                owner.buffController.TryAddBuffInfo(buffInfoID, owner, level);
            }
        //}
    }

    //附加伤害函数
    private void OnAfterFinalDamageNewEvent(GameClient.BeEvent.BeEventParam param)
    {
        int hurtId = param.m_Int2;
        if(hurtId == attachEffectID)//防止递归
        {
            return;
        }
        var attackElementType = owner.GetEntityData().GetAttackElementType(hurtId);
        BeActor target = param.m_Obj as BeActor;
        if (target != null && attackElementType == MagicElementType.FIRE &&
                target.stateController.HasBuffState(BeBuffStateType.FROZEN))
        {
            owner.DoAttackTo(target, attachEffectID, false, true);
        }
    }
}

