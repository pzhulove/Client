using System.Collections.Generic;
using GameClient;

/// <summary>
/// 自身的宝宝死亡时给自己加指定BUFF
/// </summary>
public class Mechanism1506 : BeMechanism
{
    public Mechanism1506(int mid, int lv) : base(mid, lv)
    {
    }

    private int m_BuffInfoId;
    private List<int> m_SummonMonsterIdList = new List<int>();
    private bool m_IsKillAddBuff;
    
    public override void OnInit()
    {
        base.OnInit();
        m_BuffInfoId = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        for (int i = 0; i < data.ValueB.Count; i++)
        {
            m_SummonMonsterIdList.Add(TableManager.GetValueFromUnionCell(data.ValueB[i], level));
        }

        m_IsKillAddBuff = data.ValueC.Count == 0;
    }

    public override void OnReset()
    {
        m_SummonMonsterIdList.Clear();
        m_IsKillAddBuff = false;
    }

    public override void OnStart()
    {
        if(owner.CurrentBeScene == null)
            return;
        
        if (m_IsKillAddBuff)
        {
            sceneHandleA = owner.CurrentBeScene.RegisterEventNew(BeEventSceneType.onEntityDead, OnEntityRemove);
        }
        else
        {
            sceneHandleA = owner.CurrentBeScene.RegisterEventNew(BeEventSceneType.onEntityRemove, OnEntityRemove);
        }
    }
    
    private void OnEntityRemove(BeEvent.BeEventParam param)
    {
        BeEntity entity = param.m_Obj as BeEntity;
        BeActor actor = entity as BeActor;
        if (actor != null)
        {
            if (IsSummonMonster(actor))
            {
                owner.buffController.TryAddBuffInfo(m_BuffInfoId, actor, level);
            }
        }
    }

    private bool IsSummonMonster(BeActor actor)
    {
        if (!actor.IsSameTopOwner(owner))
            return false;

        if(actor.attribute == null)
            return false;
        
        for (int i = 0; i < m_SummonMonsterIdList.Count; i++)
        {
            int id = m_SummonMonsterIdList[i];
            if (actor.attribute.MonsterIDEqual(id))
            {
                return true;
            }
        }

        return false;
    }
}