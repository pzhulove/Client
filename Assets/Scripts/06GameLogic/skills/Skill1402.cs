/*
 using GameClient;
using System;
using System.Collections.Generic;


/// <summary>
/// 路霸
/// </summary>
public class Skill1402 : BeSkill
{
    public Skill1402(int sid, int skillLevel) : base(sid, skillLevel)
    {
    }

    protected int MonsterId;
    protected BeActor Monster;
    protected BeEvent.BeEventHandleNew MonsterDeadHandle;
    protected BeEvent.BeEventHandleNew _skillStartHandle;
    protected List<BeEventHandle> _handleList;

    private readonly int _saoSheSkillId = 1419;
    private readonly int _changeBulletSkillId = 1416;
    protected bool CanClickAgain = false;


    public override void OnInit()
    {
        base.OnInit();
        MonsterId = 94150031;
        if(_handleList == null)
            _handleList = new List<BeEventHandle>();
    }

    public override void OnStart()
    {
        _RemoveHandle();
        _RegisterHandle();
    }

    public override void OnClickAgain()
    {
        base.OnClickAgain();
        _SwitchToSaoShe();
    }

    /// <summary>
    /// 监听召唤
    /// </summary>
    private void _RegisterHandle()
    {

        if (owner.CurrentBeScene == null)
            return;
        _handleList.Add(owner.CurrentBeScene.RegisterEvent(BeEventSceneType.onSummon, _OnSummon));
        MonsterDeadHandle = owner.CurrentBeScene.RegisterEventNew(BeEventSceneType.onEntityRemove, _OnMonsterDead);
    }

    private void _OnSummon(object[] args)
    {
        var monster = args[0] as BeActor;
        if (monster == null)
            return;
        if (!monster.GetEntityData().MonsterIDEqual(MonsterId))
            return;
        if (monster.GetOwner() != owner)
            return;
        CanClickAgain = true;
        Monster = monster;

        _skillStartHandle = monster.RegisterEventNew(BeEventType.onSkillStart, _OnSkillStart);
        _handleList.Add(monster.RegisterEvent(BeEventType.onSkillCancel, _OnSkillFinish));
        _handleList.Add(monster.RegisterEvent(BeEventType.onCastSkillFinish, _OnSkillFinish));

        skillButtonState = SkillState.WAIT_FOR_NEXT_PRESS;
        ChangeButtonEffect();
    }

    /// <summary>
    /// 召唤的怪物死亡
    /// </summary>
    private void _OnMonsterDead(BeEvent.BeEventParam param)
    {
        if (Monster == null)
            return;
        var actor = param.m_Obj as BeActor;
        if (actor == null)
            return;
        if (Monster == actor)
        {
            CanClickAgain = false;
            ResetButtonEffect();
        }
    }

    private void _RemoveHandle()
    {
        for (int i = 0; i < _handleList.Count; i++)
        {
            var handle = _handleList[i];
            if (handle == null)
                continue;
            handle.Remove();
            handle = null;
        }
        _handleList.Clear();

        if (MonsterDeadHandle != null)
        {
            MonsterDeadHandle.Remove();
            MonsterDeadHandle = null;
        }

        if (_skillStartHandle != null)
        {
            _skillStartHandle.Remove();
            _skillStartHandle = null;
        }
    }


    /// <summary>
    /// 切换到扫射技能
    /// </summary>
    private void _SwitchToSaoShe()
    {
        if (Monster == null)
            return;
        if (!Monster.CanUseSkill(_saoSheSkillId))
            return;
        Monster.UseSkill(_saoSheSkillId);
    }

    /// <summary>
    /// 监听技能开始
    /// </summary>
    /// <param name="param"></param>
    private void _OnSkillStart(BeEvent.BeEventParam param)
    {
        int skillId = param.m_Int;
        if (skillId != _changeBulletSkillId)
            return;
        ResetButtonEffect();
    }

    private void _OnSkillFinish(object[] args)
    {
        int skillId = (int)args[0];
        if (skillId != _changeBulletSkillId)
            return;
        if (!CanClickAgain)
            return;
        skillButtonState = SkillState.WAIT_FOR_NEXT_PRESS;
        ChangeButtonEffect();
    }
}
*/