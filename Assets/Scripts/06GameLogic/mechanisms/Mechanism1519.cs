using System.Collections.Generic;
using GameClient;

/// <summary>
/// 在指定攻击时，将自己向攻击者位置拖拽一定%距离
/// </summary>
public class Mechanism1519 : BeMechanism
{
    public Mechanism1519(int mid, int lv) : base(mid, lv) { }

    private int m_Duration;
    private int m_NearestDistance;
    private VFactor m_Distance;
    private List<int> m_TargetHurtId = new List<int>();
    public override void OnInit()
    {
        base.OnInit();
        for (int i = 0; i < data.ValueA.Count; i++)
        {
            m_TargetHurtId.Add(TableManager.GetValueFromUnionCell(data.ValueA[0], level));
        }

        m_Distance = VFactor.NewVFactor(TableManager.GetValueFromUnionCell(data.ValueB[0], level), GlobalLogic.VALUE_1000);
        m_Duration = TableManager.GetValueFromUnionCell(data.ValueB[1], level);

        if (data.ValueC.Length > 0)
        {
            m_NearestDistance = TableManager.GetValueFromUnionCell(data.ValueC[0], level) * 10;
        }
    }

    public override void OnReset()
    {
        base.OnReset();
        Clear();
    }

    public override void OnStart()
    {
        base.OnStart();
        handleA = owner.RegisterEventNew(BeEventType.onHit, OnHit);
    }

    private void OnHit(BeEvent.BeEventParam param)
    {
        var hurtId = param.m_Int4;
        if(!m_TargetHurtId.Contains(hurtId))
            return;
        
        var attacker = param.m_Obj as BeActor;
        DoMoveToAttacker(attacker);
    }

    private void DoMoveToAttacker(BeActor attacker)
    {
        var centerPos = attacker.GetPosition();
        var pos = owner.GetPosition();
        //if((centerPos - pos).magnitude < m_NearestDistance)
        //    return;
        
        var endPos = pos;
        if(m_NearestDistance > 0)
            endPos = centerPos + (pos - centerPos).NormalizeTo(m_NearestDistance);
        VInt3 del = new VInt3(endPos.x - pos.x, endPos.y - pos.y, 0);
        del = del * m_Distance;
        owner.actionManager.RunAction(BeMoveBy.Create(owner, m_Duration, owner.GetPosition(), del, false,null,true));
    }

    public override void OnFinish()
    {
        base.OnFinish();
        Clear();
    }

    private void Clear()
    {
        m_Distance = VFactor.zero;
        m_Duration = 0;
        m_TargetHurtId.Clear();
        m_NearestDistance = 0;
    }
}
 