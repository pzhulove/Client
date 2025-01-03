using UnityEngine;
using System.Collections;
using GameClient;

public class Skill2522 : BeSkill
{
    public Skill2522(int sid, int skillLevel) : base(sid, skillLevel) { }

    protected int effectId = 25221;   //女漫游的走射伤害ID
    protected int attachEffectId = 25240;     //附加触发效果ID
    protected string curFrameFlag = "252201";     //标记ID
    protected int skillId = 2524;     //女漫游银弹技能ID

    protected int leftBulletNum = 0;   //当前剩余的银弹次数
    
    public override void OnStart()
    {
        base.OnStart();
        leftBulletNum = GetBulletNum();
        handleA = owner.RegisterEventNew(BeEventType.onSkillCurFrame, OnSkillCurFrame);
        //handleA = owner.RegisterEvent(BeEventType.onSkillCurFrame, OnSkillCurFrame);

        handleB = owner.RegisterEventNew(BeEventType.onAfterFinalDamageNew, OnAfterFinalDamageNew);
        //handleB = owner.RegisterEvent(BeEventType.onAfterFinalDamageNew, OnAfterFinalDamageNew);
    }

    /// <summary>
    /// 获取剩余银弹次数
    /// </summary>
    /// <returns></returns>
    protected virtual  int GetBulletNum()
    {
        Skill2524 skill = (Skill2524)owner.GetSkill(skillId);
        if (skill == null)
            return 0;
        return skill.GetLeftBulletNum();
    }

    /// <summary>
    /// 设置银弹数量
    /// </summary>
    protected virtual void SetSilverBulletCount()
    {
        Skill2524 skill = (Skill2524)owner.GetSkill(skillId);
        if (skill == null)
            return;
        skill.ConsumBulletCount();
    }
    /// <summary>
    /// 技能触发某一帧 响应函数
    /// </summary>
    /// <param name="args"></param>
    protected virtual void OnSkillCurFrame(BeEvent.BeEventParam param)
    {
        string flag = param.m_String;
        if (flag == curFrameFlag && leftBulletNum > 0)
        {
            leftBulletNum--;
            SetSilverBulletCount();
        }
    }

    /// <summary>
    /// 附加伤害函数
    /// </summary>
    /// <param name="args"></param>
    protected virtual void OnAfterFinalDamageNew(GameClient.BeEvent.BeEventParam param)
    {
        int id = param.m_Int2;
        BeActor target = param.m_Obj as BeActor;
        if (id == effectId && target != null && leftBulletNum > 0)
        {
            owner.DoAttackTo(target, attachEffectId, false, true);
        }
    }
}
