using GameClient;

/// <summary>
/// 当实体死亡时，自身如果处于指定技能，则直接进去最后技能阶段
/// </summary>
public class Mechanism3011 : BeMechanism
{
    public Mechanism3011(int mid, int lv) : base(mid, lv)
    {
    }

    private int m_EntityId;
    private int m_SkillId;
    public override void OnInit() 
    {
        base.OnInit();
        m_EntityId = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        m_SkillId = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
    }

    public override void OnReset()
    {
        base.OnReset();
        m_EntityId = 0;
        m_SkillId = 0;
    }

    public override void OnStart()
    {
        base.OnStart();
        handleA = owner.CurrentBeScene.RegisterEventNew(BeEventSceneType.onEntityRemove, OnRemoveEntity);
    }

    private void OnRemoveEntity(BeEvent.BeEventParam param)
    {
        BeProjectile projectile = param.m_Obj as BeProjectile;
        if (projectile != null)
        {
            if (projectile.m_iResID == m_EntityId && owner.IsSameTopOwner(projectile))
            {
                if (owner.IsCastingSkill() && owner.GetCurSkillID() == m_SkillId)
                {
                    ((BeActorStateGraph) owner.GetStateGraph()).ExecuteEndPhase();
                }
            }
        }
    }
}
