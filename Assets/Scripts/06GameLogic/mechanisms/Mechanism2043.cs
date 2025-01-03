using System;
using System.Collections.Generic;
using GameClient;

//团本索敌范围增加
public class Mechanism2043: BeMechanism
{
    private int buffIDCondition;
    private VFactor sightAddRate = VFactor.zero;
    private int originSight = 0;
    public Mechanism2043(int sid, int skillLevel) : base(sid, skillLevel)
    {

    }
    public override void OnInit()
    {
        base.OnInit();
        buffIDCondition = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        int intFactor = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        sightAddRate = VFactor.NewVFactor(intFactor, GlobalLogic.VALUE_1000);
      
    }

    public override void OnReset()
    {
        originSight = 0;
    }

    public override void OnStart()
    {
        base.OnStart();
        handleA = owner.RegisterEventNew(BeEventType.onAddBuff, OnAddBuff);
        handleB = owner.RegisterEventNew(BeEventType.onRemoveBuff, OnRemoveBuff);
    }
    private void OnAddBuff(BeEvent.BeEventParam args)
    {
        BeBuff buff = args.m_Obj as BeBuff;
        if (buff == null || buff.buffID != buffIDCondition) return;
        if (owner == null || owner.buffController == null || owner.aiManager == null) return;
        if (owner.buffController.GetBuffCountByID(buff.buffID) > 1)
        {
            return;
        }
        originSight = owner.aiManager.sight;
        var addSight = originSight * (VFactor.one + sightAddRate);
        owner.aiManager.sight = addSight;
    }
    private void OnRemoveBuff(BeEvent.BeEventParam args)
    {
        int buffId = (int)args.m_Int;
        if (buffIDCondition != buffId) return;
        if (owner == null || owner.buffController == null || owner.aiManager == null) return;
        if (owner.buffController.GetBuffCountByID(buffId) > 1)
        {
            return;
        }
        owner.aiManager.sight = originSight;
    }
}
