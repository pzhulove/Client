using GameClient;
using UnityEngine;

/// <summary>
/// 药剂-进化药剂机制
/// 当释放选点药剂后，召唤物（自身）直接寻路前往该点，并在达到一定距离后，释放技能
/// </summary>
public class Mechanism3015 : BeMechanism
{
    public Mechanism3015(int mid, int lv) : base(mid, lv)
    {
    }

    private BeActor m_Actor;
    private VInt3 m_TargetPos;
    private bool m_GotoUseSkill = false;
    
    private int m_TargetResID;
    private VInt2 m_Distance;
    private VInt2 m_RemoveBuffDistance;
    private int m_SkillID;
    private int[] m_BuffList;
    public override void OnInit()
    {
        base.OnInit();
        m_TargetResID = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        m_Distance = new VInt2(TableManager.GetValueFromUnionCell(data.ValueB[0], level), TableManager.GetValueFromUnionCell(data.ValueB[1], level));
        m_SkillID = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
        m_BuffList = new int[data.ValueD.Length];
        for (int i = 0; i < data.ValueD.Length; i++)
        {
            m_BuffList[i] = TableManager.GetValueFromUnionCell(data.ValueD[i], level);
        }
    }

    public override void OnReset()
    {
        base.OnReset();
        m_Actor = null;
        m_TargetPos = VInt3.zero;
        m_GotoUseSkill = false;
        m_BuffList = null;
    }

    public override void OnStart()
    {
        base.OnStart();
        m_Actor = GetTopOwner();
        if (m_Actor != null)
        {
            handleA = m_Actor.RegisterEventNew(BeEventType.onOwnerAfterGenBullet, onAfterGenBullet);
        }
        handleB = owner.RegisterEventNew(BeEventType.onAIMoveEnd, OnAIMoveEnd);
    }

    private void OnAIMoveEnd(BeEvent.BeEventParam param)
    {
        if (m_GotoUseSkill)
        {
            UseSkill();
        }
    }

    private void onAfterGenBullet(BeEvent.BeEventParam args)
    {
        BeProjectile entity = args.m_Obj as BeProjectile;
        if (entity != null && entity.m_iResID == m_TargetResID)
        {
            
            m_TargetPos = entity.GetPosition();
            if (owner.sgGetCurrentState() == (int) ActionState.AS_CASTSKILL)
            {
                owner.Locomote(new BeStateData((int)ActionState.AS_IDLE), true);
            }

            owner.SetDefaultAIRun(false);
            if (owner.aiManager != null)
            {
                var ai = owner.aiManager as BeActorAIManager;
                if (ai != null)
                {
                    ai.StopCurrentCommand();
                    ai.DoDestination((int) BeAIManager.DestinationType.GO_TO_TARGET3, entity, true);
                }
            }

            for (int i = 0; i < m_BuffList.Length; i++)
            {
                owner.buffController.TryAddBuff(m_BuffList[i], -1, level);
            }
            m_GotoUseSkill = true;
        }
    }

    public override void OnUpdate(int deltaTime)
    {
        if (!m_GotoUseSkill)
            return;
        
        var dis = (m_TargetPos - owner.GetPosition());
        if (Mathf.Abs(dis.x) < m_Distance.x && Mathf.Abs(dis.y) < m_Distance.y)
        {
            UseSkill();
        }
    }

    private void UseSkill()
    {
        for (int i = 0; i < m_BuffList.Length; i++)
        {
            owner.buffController.RemoveBuff(m_BuffList[i]);
        }
            
        m_GotoUseSkill = false;
        owner.SetFace(owner.GetPosition().x > m_TargetPos.x, true, true);
        owner.UseSkill(m_SkillID, true);
        owner.SetDefaultAIRun(true);
    }
}
