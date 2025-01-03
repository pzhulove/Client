using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameClient;

//驱魔师释放玄武
public class Skill3612 : BeSkill
{
    public Skill3612(int sid, int skillLevel) : base(sid, skillLevel) { }

    private int _monsterId = 0;
    private int _skillId = 5031;

    private BeActor _monster = null;

    private IBeEventHandle _monsterSummonHandle = null;
    private IBeEventHandle _monsterDeadHandle = null;
    private IBeEventHandle _rebornHandle = null;

    private BeEvent.BeEventHandleNew _monsterSkillStartCoolDownHandle = null;
    private BeEvent.BeEventHandleNew _monsterSkillEndCoolDownHandle = null;

    public override void OnPostInit()
    {
        base.OnPostInit();
        _monsterId = BattleMain.IsModePvP(battleType) ? 93040031 : 93100031;
    }

    public override void OnStart()
    {
        base.OnStart();
        if (CanManualAttack())
        {
            pressMode = SkillPressMode.PRESS_MANY_TWO;
            _RemoveHandle();
            _RegisterHandle();
        }
    }

    public override bool CanUseSkill()
    {
        if(CanManualAttack() && _monster != null) return false;
        return base.CanUseSkill();
    }

    public override BeSkillManager.SkillCannotUseType GetCannotUseType()
    {
        if (CanManualAttack() && _monster != null) return BeSkillManager.SkillCannotUseType.CAN_NOT_USE;
        return base.GetCannotUseType();
    }

    public override void OnClickAgain()
    {
        base.OnClickAgain();
        if (CanManualAttack())
        {
            _MonsterUseSkill();
        }
    }

    public override void OnCancel()
    {
        base.OnCancel();
        if (CanManualAttack() && _monster == null)
            ResetButtonEffect();
    }

    public override void OnFinish()
    {
        base.OnFinish();
        if (CanManualAttack() && _monster == null)
            ResetButtonEffect();
    }

    private void _RegisterHandle()
    {
        _monsterSummonHandle = owner.RegisterEventNew(BeEventType.onSummon, _OnSummon);
        _rebornHandle = owner.RegisterEventNew(BeEventType.onReborn, _OnReborn);
    }

    private void _OnSummon(BeEvent.BeEventParam args)
    {
        // var actor = args[0] as BeActor;
        var actor = args.m_Obj as BeActor;
        if (actor == null) return;
        if (!actor.GetEntityData().MonsterIDEqual(_monsterId)) return;
        
        _monsterDeadHandle = actor.RegisterEventNew(BeEventType.onDead, _OnSummonDead);
        _monsterSkillStartCoolDownHandle = actor.RegisterEventNew(BeEventType.onSkillCoolDownStart, _OnSkillCoolDownStart);
        _monsterSkillEndCoolDownHandle = actor.RegisterEventNew(BeEventType.onSkillCoolDownFinish, _OnSkillCoolDownFinish);

        _monster = actor;

        skillButtonState = SkillState.WAIT_FOR_NEXT_PRESS;
        ChangeButtonEffect();
    }

    private void _OnReborn(BeEvent.BeEventParam args)
    {
        ResetButtonEffect();
    }

    private void _OnSummonDead(BeEvent.BeEventParam args)
    {
        ResetButtonEffect();
        _monster = null;
    }

    private void _MonsterUseSkill()
    {
        if (_monster == null) return;
        if (!_monster.CanUseSkill(_skillId)) return;
        _monster.UseSkill(_skillId);
        ResetButtonEffect();
    }

    private void _OnSkillCoolDownStart(BeEvent.BeEventParam param)
    {
        var skillId = param.m_Int;
        if (skillId != _skillId) return;
        if (_monster == null) return;

        ResetButtonEffect();
    }

    private void _OnSkillCoolDownFinish(BeEvent.BeEventParam param)
    {
        var skillId = param.m_Int;
        if (skillId != _skillId) return;
        if (_monster == null) return;

        skillButtonState = SkillState.WAIT_FOR_NEXT_PRESS;
        ChangeButtonEffect();
    }

    private bool CanManualAttack()
    {
        if (owner.CurrentBeBattle == null) return false;
        return false;
        // return !owner.CurrentBeBattle.FunctionIsOpen(BattleFlagType.XuanwuManualAttack) && owner.XuanWuManualAttack;
    }

    private void _RemoveHandle()
    {
        if (_monsterSummonHandle != null)
        {
            _monsterSummonHandle.Remove();
            _monsterSummonHandle = null;
        }

        if (_monsterDeadHandle != null)
        {
            _monsterDeadHandle.Remove();
            _monsterDeadHandle = null;
        }

        if(_monsterSkillStartCoolDownHandle != null)
        {
            _monsterSkillStartCoolDownHandle.Remove();
            _monsterSkillStartCoolDownHandle = null;
        }

        if (_monsterSkillEndCoolDownHandle != null)
        {
            _monsterSkillEndCoolDownHandle.Remove();
            _monsterSkillEndCoolDownHandle = null;
        }

        if (_rebornHandle != null)
        {
            _rebornHandle.Remove();
            _rebornHandle = null;
        }
    }

}
