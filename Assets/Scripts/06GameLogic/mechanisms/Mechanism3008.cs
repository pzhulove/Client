using System.Collections.Generic;
using GameClient;

/// <summary>
/// 执行切换动作技能事件时 移除指定特效
/// </summary>
public class Mechanism3008 : BeMechanism
{
    private string[] _removeEffectPathArr;

    public Mechanism3008(int mid, int lv) : base(mid, lv)
    {
    }


    public override void OnInit()
    {
        base.OnInit();
        _removeEffectPathArr = data.StringValueA.ToArray();
    }

    public override void OnStart()
    {
        base.OnStart();
        handleA = OwnerRegisterEventNew(BeEventType.onSkillEventChangeAnimation, _OnSkillEventChangeAnimation);
    }

    private void _OnSkillEventChangeAnimation(BeEvent.BeEventParam param)
    {
        if (owner.m_pkGeActor == null) return;
#if !LOGIC_SERVER
        for (int i = 0; i < _removeEffectPathArr.Length; i++)
        {
            owner.m_pkGeActor.DestroyEffectByName(_removeEffectPathArr[i]);
        }
#endif
    }
}