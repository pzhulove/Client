using System.Collections.Generic;

/// <summary>
/// 机制初始化的时候  如果机制拥有者有抓取到目标 则在目标位置创建一个实体
/// </summary>
public class Mechanism1120 : BeMechanism
{
    public Mechanism1120(int mid, int lv) : base(mid, lv) { }

    protected int m_EntityId = 0;

    public override void OnInit()
    {
        base.OnInit();
        m_EntityId = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
    }

    public override void OnStart()
    {
        base.OnStart();
        CreateEntity();
    }

    protected void CreateEntity()
    {
        List<BeActor> list = GamePool.ListPool<BeActor>.Get();
        owner.grabController.GetGrabTargetList(list);
        if (list.Count <= 0)
        {
            GamePool.ListPool<BeActor>.Release(list);
            return;
        }
        for(int i=0;i< list.Count; i++)
        {
            owner.AddEntity(m_EntityId, list[i].GetPosition(),level);
        }
        GamePool.ListPool<BeActor>.Release(list);
        
    }
}

