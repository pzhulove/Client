using System;
using System.Collections.Generic;
using GameClient;
//团本 水炮
public class Mechanism2052 : BeMechanism
{
    int monsterId = 0;
    int resID = 0;
    public Mechanism2052(int mid, int lv) : base(mid, lv)
    {
    }
    public override void OnInit()
    {
        base.OnInit();
        monsterId = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        resID = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
    }

    public override void OnStart()
    {
        base.OnStart();
        handleA = owner.RegisterEventNew(BeEventType.onAfterGenBullet, onGenProjectile);
    }
    void onGenProjectile(BeEvent.BeEventParam eventParam)
    {
        var entity = eventParam.m_Obj as BeProjectile;
        if (entity != null)
        {
            entity.RegisterEventNew(BeEventType.onDead, onProjectileDead);
        }
    }
    void onProjectileDead(BeEvent.BeEventParam eventParam)
    {
        if (owner == null || owner.CurrentBeScene == null) return;
        var entity = eventParam.m_Obj as BeProjectile;
        if (entity != null && entity.m_iResID == resID)
        {
            var pos = entity.GetPosition();
            pos =  BeAIManager.FindStandPositionNew(pos, owner.CurrentBeScene, !entity.GetFace(), false, 40);
            var monster =  owner.CurrentBeScene.SummonMonster(monsterId + level * 100, pos, owner.GetCamp(), owner);
        }
    }
}
