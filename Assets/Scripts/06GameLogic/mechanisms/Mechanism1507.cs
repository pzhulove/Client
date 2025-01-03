using System.Collections.Generic;
using GameClient;

/// <summary>
/// 宝宝击杀总数达到一定值加指定BuffInfo
/// </summary>
public class Mechanism1507 : BeMechanism
{
    public Mechanism1507(int mid, int lv) : base(mid, lv)
    {
    }

    private struct KillCountBuffInfo
    {
        public int KillCount;
        public int BuffInfoId;
    }
    
    private List<int> m_SummonMonsterIdList = new List<int>();
    private List<KillCountBuffInfo> m_AddBuffList = new List<KillCountBuffInfo>();
    private int m_CurSkillCount;
    private int m_PreBuffInfo;

    public int CurSkillCount
    {
        get => m_CurSkillCount;
        set
        {
            m_CurSkillCount = value; 
            TriggerAddBuffInfo();
        }
    }

    public override void OnInit()
    {
        base.OnInit();
        for (int i = 0; i < data.ValueA.Count; i++)
        {
            m_SummonMonsterIdList.Add(TableManager.GetValueFromUnionCell(data.ValueA[i], level));
        }

        for (int i = 0; i < data.ValueB.Count && i < data.ValueC.Count; i++)
        {
            int count = TableManager.GetValueFromUnionCell(data.ValueB[i], level);
            int id = TableManager.GetValueFromUnionCell(data.ValueC[i], level);
            m_AddBuffList.Add(new KillCountBuffInfo{KillCount = count, BuffInfoId = id});
        }

        m_CurSkillCount = 0;
    }

    public override void OnReset()
    {
        m_SummonMonsterIdList.Clear();
        m_AddBuffList.Clear();
        m_PreBuffInfo = 0;
    }

    public override void OnStart()
    {
        if (owner.CurrentBeScene != null)
        {
            sceneHandleA = owner.CurrentBeScene.RegisterEventNew(BeEventSceneType.onKill, OnKill);
        }
    }

    private void OnKill(BeEvent.BeEventParam param)
    {
        BeActor attacker = param.m_Obj2 as BeActor;
        if (attacker != null)
        {
            if (IsSummonMonster(attacker))
            {
                CurSkillCount++;
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

    private void TriggerAddBuffInfo()
    {
        for (int i = 0; i < m_AddBuffList.Count; i++)
        {
            var curLevel = m_AddBuffList[i];
            if (CurSkillCount >= curLevel.KillCount)
            {
                if (m_PreBuffInfo != curLevel.BuffInfoId)
                {
                    owner.buffController.RemoveBuffByBuffInfoID(m_PreBuffInfo);
                }
                owner.buffController.TryAddBuffInfo(curLevel.BuffInfoId, owner, level);
                m_PreBuffInfo = curLevel.BuffInfoId;
            }
        }
    }
}