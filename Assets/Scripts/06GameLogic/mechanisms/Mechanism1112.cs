using GameClient;

//“蓄能反击”:
//带有机制的怪物只要受到伤害的时候会在头顶出现进度条，进度条随着受到伤害的增加而增长。
//当受到到指定数值的伤害后（进度条满积满）会强制让怪物释放一个技能。
public class Mechanism1112 : BeMechanism
{
    int m_maxHurtValue = 0;   //最大伤害数值
    int m_spellSkillID = 0;    //待释放的技能id
    int m_curHurtValue = 0;    //当前累积的伤害数值
    bool m_isTimeRelated = false;  //是否随时间衰减伤害值
    int m_reduceHurtValue = 0;      //周期衰减伤害值单位
    int m_durTime = 0;               //当前累积时间
    bool m_needSpellSkill = false;   //是否满足条件待释放中
    public Mechanism1112(int mid, int lv) : base(mid, lv)
    {
    }
    public override void OnInit()
    {
        m_maxHurtValue = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        m_spellSkillID = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        m_isTimeRelated = TableManager.GetValueFromUnionCell(data.ValueC[0], level) != 0 ? true:false;
        m_reduceHurtValue = TableManager.GetValueFromUnionCell(data.ValueD[0], level);
    }

    public override void OnReset()
    {
        m_curHurtValue = 0;
        m_durTime = 0;
        m_needSpellSkill = false;
    }

    public override void OnStart()
    {
        handleA = owner.RegisterEventNew(BeEventType.onHurt, _onBeHit);
        //handleA = owner.RegisterEvent(BeEventType.onHurt, _onBeHit);
    }

    public override void OnUpdate(int deltaTime)
    {
        base.OnUpdate(deltaTime);
        //当累积伤害值刚满足时，因其他情况导致不能释放技能，那么一直尝试释放技能
        if (m_needSpellSkill)
        {
            _tryUseSkill();
            return;
        }

        if (!m_isTimeRelated) return;

        //和时间关联的时候做伤害值递减
        if (m_curHurtValue <= 0) return;

        m_durTime += deltaTime;
        if (m_durTime < 200) return;

        m_durTime -= 200;
        int oldValue = m_curHurtValue;
        m_curHurtValue -= m_reduceHurtValue;

        if (m_curHurtValue < 0)
        {
            m_curHurtValue = 0;
        }
        if (m_curHurtValue <= 0)
        {
            owner.StopSpellBar(eDungeonCharactorBar.Progress);
        }
        else
        {
            _updateProgress(m_curHurtValue - oldValue);
        }
    }

    private void _onBeHit(BeEvent.BeEventParam param)
    {
        if (m_needSpellSkill) return;
        int hurtValue = param.m_Int;
        if (hurtValue <= 0) return;
        m_curHurtValue += hurtValue;
        _updateProgress(hurtValue);
        if (m_curHurtValue >= m_maxHurtValue)
        {
            m_needSpellSkill = true;
            _tryUseSkill();
        }
    }

    private bool _canUseSkill()
    {

        if (owner != null && (owner.GetStateGraph() != null &&
            (owner.GetStateGraph().CurrentStateHasTag((int)AStateTag.AST_CONTROLED) || (
           owner.GetStateGraph().CurrentStateHasTag((int)AStateTag.AST_BUSY)) && !owner.HasTag((int)AState.ACS_JUMP)
           )))
        {
            return false;
        }
        return true;
    }

    private void _updateProgress(int addValue)
    {
        SpellBar bar = null;
        var dur = owner.GetSpellBarDuration(eDungeonCharactorBar.Progress);
        if (dur <= 0)
        {
            string content = "";
            if (data.StringValueA.Count > 0)
            {
                content = data.StringValueA[0];
            }
            bar = owner.StartSpellBar(eDungeonCharactorBar.Progress, m_maxHurtValue, true, content);
            bar.autoAcc = false;
            bar.reverse = false;
            bar.autodelete = false;
        }
        owner.AddSpellBarProgress(eDungeonCharactorBar.Progress, new VFactor(addValue, m_maxHurtValue));
    }
    private void _tryUseSkill()
    {
        if(owner!= null  && !owner.IsDead() && _canUseSkill() && owner.CanUseSkill(m_spellSkillID))
        {
            if(owner.UseSkill(m_spellSkillID))
            {
                m_curHurtValue = 0;
                owner.StopSpellBar(eDungeonCharactorBar.Progress);
                m_needSpellSkill = false;
                m_durTime = 0;
            }
        }
    }
}
