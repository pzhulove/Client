using System.Collections.Generic;
using GameClient;

/// <summary>
/// 堕天斩
/// </summary>
public class Skill1933 : BeSkill
{
    public Skill1933(int sid, int skillLevel) : base(sid, skillLevel) { }

    private string _startComboFlag = "193301";
    
    private List<IBeEventHandle> _handleList = new List<IBeEventHandle>();
    private bool _canComboYinluo = false;
    private int _comboYinluoMechanismId = 0;

    public override void OnInit()
    {
        base.OnInit();
        _comboYinluoMechanismId = TableManager.GetValueFromUnionCell(skillData.ValueA[0], level);
    }

    public override void OnStart()
    {
        base.OnStart();
        _canComboYinluo = false;
        _RemoveHandle();
        _RegisterEvent();
    }

    private void _RegisterEvent()
    {
        handleA = owner.RegisterEventNew(BeEventType.onSkillCurFrame, _OnSkillCurFrame);

        _handleList.Add(owner.RegisterEventNew(BeEventType.onTouchGround, _OnTouchGround));
        _handleList.Add(owner.RegisterEventNew(BeEventType.onStateChangeEnd, _OnStateChangeEnd));
        _handleList.Add(owner.RegisterEventNew(BeEventType.onBeHitAfterFinalDamage, _OnBeHitAfterFinalDamage));
    }

    private void _OnSkillCurFrame(BeEvent.BeEventParam args)
    {
        // string flag = (string) args[0];
        string flag = args.m_String;
        if (flag == _startComboFlag && !_canComboYinluo)
            _ChangeYinluoCanUseFlag(true);
    }

    private void _OnTouchGround(BeEvent.BeEventParam args)
    {
        if (_canComboYinluo)
            _ChangeYinluoCanUseFlag(false);
    }

    private void _OnStateChangeEnd(BeEvent.BeEventParam param)
    {
        ActionState state = (ActionState)param.m_Int;
        if (state == ActionState.AS_HURT || state == ActionState.AS_FALL || state == ActionState.AS_GRABBED)
        {
            if (_canComboYinluo)
                _ChangeYinluoCanUseFlag(false);
        }
    }

    private void _OnBeHitAfterFinalDamage(BeEvent.BeEventParam args)
    {
        if (_canComboYinluo)
            _ChangeYinluoCanUseFlag(false);
    }
    
    
    private void _ChangeYinluoCanUseFlag(bool canCombo)
    {
        owner.TriggerEventNew(BeEventType.onChangeYinluoFlag, new EventParam() {m_Bool = canCombo});
        if (canCombo)
        {
            if (owner.GetMechanism(_comboYinluoMechanismId) == null)
                owner.AddMechanism(_comboYinluoMechanismId, level, MechanismSourceType.NONE, null, GlobalLogic.VALUE_10000);
        }
        else
            owner.RemoveMechanism(_comboYinluoMechanismId);
        _canComboYinluo = canCombo;
    }

    private void _RemoveHandle()
    {
        for (int i = 0; i < _handleList.Count; i++)
        {
            var handle = _handleList[i];
            if (handle != null)
            {
                handle.Remove();
                handle = null;
            }
        }
    }

}