using System.Collections.Generic;
using FlatBuffers;
using GameClient;
using Spine;

/// <summary>
/// 强制技能连招机制
/// 当前置技能完成后，后置技能处于CD中，则强制释放后置技能
/// </summary>
public class Mechanism1514 : BeMechanism
{
    public Mechanism1514(int mid, int lv) : base(mid, lv)
    {
    }

    private int m_PreSkill;
    private int m_PostSkill;
    private bool isCombo = false;
    public override void OnInit()
    {
        base.OnInit();
        isCombo = false;
        m_PreSkill = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        m_PostSkill = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
    }

    public override void OnStart()
    {
        base.OnStart();
        handleA = owner.RegisterEventNew(BeEventType.onCastSkillFinish, OnSkillFinish);
    }

    private void OnSkillFinish(BeEvent.BeEventParam param)
    {
        if (param.m_Int != m_PreSkill)
            return;

        if (owner.skillController.IsSkillCoolDown(m_PostSkill))
        {
            isCombo = true;
        }
    }

    public override void OnUpdate(int deltaTime)
    {
        base.OnUpdate(deltaTime);
        if (isCombo)
        {
            owner.skillController.ResetSkillCoolDown(m_PostSkill);
            owner.UseSkill(m_PostSkill, true);
        } 
        isCombo = false;
    }
}