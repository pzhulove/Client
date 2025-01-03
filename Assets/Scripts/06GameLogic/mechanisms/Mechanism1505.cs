/// <summary>
/// 用于召唤类技能的接口类
/// </summary>
public interface ISummonSkillUpdate
{
    void OnSummonUpdate(int deltaTime);

    void OnSummonUpdateFinish();
}

/// <summary>
/// 用于召唤类技能的Update
/// </summary>
public class Mechanism1505 : BeMechanism
{
    public Mechanism1505(int mid, int lv) : base(mid, lv) { }

    public static readonly int MechanismID = 6224;
    private int mOwnerSkillId = 0;
    private ISummonSkillUpdate mOwnerSkill;

    /// <summary>
    /// 召唤物添加机制，刷新主角技能
    /// </summary>
    /// <param name="monster">召唤物</param>
    /// <param name="skill">刷新的对应技能</param>
    public static void BindUpdate(BeActor monster, ISummonSkillUpdate skill)
    {
        if (monster == null || skill == null)
            return;
        
        var mech = monster.AddMechanism(MechanismID) as Mechanism1505;
        if (mech != null)
        {
            mech.InitSkill(skill);
        }
    }
    
    public override void OnInit()
    {
        base.OnInit();
        
        if(data.ValueA.Count > 0)
            mOwnerSkillId = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
    }

    public override void OnReset()
    {
        mOwnerSkillId = 0;
        mOwnerSkill = null;
    }

    public override void OnStart()
    {
        base.OnStart();
        if(mOwnerSkillId <= 0)
            return;
        
        var topOwner = GetTopOwner();
        if (topOwner != null)
        {
            var skill = topOwner.GetSkill(mOwnerSkillId);
            if (skill != null)
            {
                mOwnerSkill = skill as ISummonSkillUpdate;
            }
        }
    }

    public void InitSkill(ISummonSkillUpdate skill)
    {
        mOwnerSkill = skill;
    }
    
    public override void OnUpdate(int deltaTime)
    {
        if (mOwnerSkill != null)
        {
            mOwnerSkill.OnSummonUpdate(deltaTime);
        }
    }

    public override void OnFinish()
    {
        if (mOwnerSkill != null)
        {
            mOwnerSkill.OnSummonUpdateFinish();
            mOwnerSkill = null;
        }
        
    }
}

