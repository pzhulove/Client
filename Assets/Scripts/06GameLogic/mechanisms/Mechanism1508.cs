using System.Collections.Generic;
using GameClient;

/// <summary>
/// 给自己的指定召唤物加BuffInfo
/// </summary>
public class Mechanism1508 : BeMechanism
{
    public Mechanism1508(int mid, int lv) : base(mid, lv)
    {
    }
    
    private List<int> m_SummonMonsterIdList = new List<int>();
    private int m_BuffInfoId;
    
    public override void OnInit()
    {
        base.OnInit();
        m_SummonMonsterIdList.Clear();
        for (int i = 0; i < data.ValueA.Count; i++)
        {
            m_SummonMonsterIdList.Add(TableManager.GetValueFromUnionCell(data.ValueA[i], level));
        }
        
        m_BuffInfoId = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
    }

    public override void OnStart()
    {
        if(owner.CurrentBeScene == null)
            return;
        
        List<BeActor> list = GamePool.ListPool<BeActor>.Get();
        owner.CurrentBeScene.FindTargets(list, owner, true, new BeMonsterIDFilter(m_SummonMonsterIdList));
        for(int i = 0; i < list.Count;i++)
        {
            var curActor = list[i];
            if (curActor != null && curActor.buffController != null)
            {
                if (curActor.IsSameTopOwner(owner))
                {
                    curActor.buffController.TryAddBuffInfo(m_BuffInfoId, owner, level);
                }
            }
        }
        GamePool.ListPool<BeActor>.Release(list);
    }
}