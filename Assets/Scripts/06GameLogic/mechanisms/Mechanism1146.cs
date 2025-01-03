using System.Collections.Generic;
using GameClient;

/// <summary>
/// 抵挡正面指定实体伤害
/// </summary>
public class Mechanism1146 : BeMechanism
{
    public Mechanism1146(int mid, int lv) : base(mid, lv)
    {
    }

    public override void OnStart()
    {
        base.OnStart();
        handleA = owner.RegisterEventNew(BeEventType.onBeHitAfterFinalDamage, OnBeHit);
        //handleA = owner.RegisterEvent(BeEventType.onBeHitAfterFinalDamage, OnBeHit);
    }

    private void OnBeHit(BeEvent.BeEventParam param)
    {
        BeEntity entity = param.m_Obj as BeEntity;
        BeProjectile projectile = entity as BeProjectile;
        if(projectile == null)
            return;
        int hurtId = param.m_Int2;

        if (IsFront(param.m_Vint3) && CanBlock(projectile, hurtId))
        {
            projectile.DoDie();
        }
    }

    private bool IsFront(VInt3 hitPos)
    {
        return !owner.GetFace() == hitPos.x - owner.GetPosition().x >= 0;
    }
    
    private bool CanBlock(BeProjectile proj, int hurtId)
    {
        if (proj == null)
            return false;

        if (proj.totoalHitCount == 1)
            return true;
        
        var hurtData = TableManager.GetInstance().GetTableItem<ProtoTable.EffectTable>(hurtId);
        if (hurtData.RepeatAttackInterval.Count > 0)
            return true;

        return false;
    }
}