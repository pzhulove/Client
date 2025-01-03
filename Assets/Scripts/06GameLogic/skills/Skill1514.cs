using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameClient;

public class Skill1514 : BeSkill
{
    List<int> carrerID = new List<int>();
    int buffID = 0;
    List<int> weaponType = new List<int>();

    protected bool _isYinluo = false;
    private bool _canUseSkillFalg = false;
    private IBeEventHandle _yinluoCanUseHandle = null;

    public Skill1514(int sid, int skillLevel) : base(sid, skillLevel)
    {

    }

    public override void OnInit()
    {
        carrerID.Clear();
        weaponType.Clear();

        for (int i = 0; i < skillData.ValueA.Count; ++i)
        {
            var value = TableManager.GetValueFromUnionCell(skillData.ValueA[i], level);
            if (value > 0)
                carrerID.Add(value);
        }

        buffID = TableManager.GetValueFromUnionCell(skillData.ValueB[0], level);

        for (int i = 0; i < skillData.ValueC.Count; ++i)
        {
            var value = TableManager.GetValueFromUnionCell(skillData.ValueC[i], level);
            if (value > 0)
                weaponType.Add(value);
        }

        _isYinluo = true;
    }

    public override void OnPostInit()
    {
        base.OnPostInit();
        if (_isYinluo)
        {
            _RemoveEvent();
            _RegisterEvent();
        }
    }

    public override void OnStart()
    {
        base.OnStart();
        _SetButtonEffect();
    }

    public override bool NeedCoolDown()
    {
        return false;
    }

    public override bool CanForceUseSkill()
    {
        if (_YinluoTwoPhaseCanUse())
            return true;
        return base.CanForceUseSkill();
    }

    public override void OnEnterPhase(int phase)
    {
        base.OnEnterPhase(phase);
        //除了剑魂，其他职业没有霸体
        if (phase == 2 && carrerID.Contains(owner.professionID) && weaponType.Contains(owner.GetWeaponType()))
        {
            owner.buffController.TryAddBuff(buffID, -1);
        }
    }

    public override void OnCancel()
    {
        base.OnCancel();
        _RemoveButtonEffect();
        RemoveBuff();
    }

    public override void OnFinish()
    {
        StartCoolDown();
        _RemoveButtonEffect();
        RemoveBuff();
    }

    private void _SetButtonEffect()
    {
        if (!_isYinluo) return;
        if (!_YinluoTwoPhaseCanUse()) return;
        ChangeButtonEffect();
    }

    private void _RegisterEvent()
    {
        _yinluoCanUseHandle = owner.RegisterEventNew(BeEventType.onChangeYinluoFlag, _OnChangeYinluoFlag);
    }

    private void _RemoveEvent()
    {
        if (_yinluoCanUseHandle != null)
        {
            _yinluoCanUseHandle.Remove();
            _yinluoCanUseHandle = null;
        }
    }

    private void _OnChangeYinluoFlag(BeEvent.BeEventParam param)
    {
        SetCanUseSkillFlag(param.m_Bool);
    }

    private void RemoveBuff()
    {
        owner.buffController.RemoveBuff(buffID);
    }

    /// <summary>
    /// 银落能否点击触发第二阶段
    /// </summary>
    private bool _YinluoTwoPhaseCanUse()
    {
        if (_canUseSkillFalg && CanUseSkill())
            return true;
        if (owner.IsCastingSkill() && owner.GetCurSkillID() == skillID && buttonState != GameClient.ButtonState.PRESS_AGAIN)
            return true;

        return false;
    }

    /// <summary>
    /// 设置银光落刃特殊情况下能否释放标志
    /// </summary>
    /// <param name="canUse">true表示能释放</param>
    public void SetCanUseSkillFlag(bool canUse)
    {
        if (canUse)
        {
            if (CanUseSkill())
            {
                _canUseSkillFalg = true;
                ChangeButtonEffect();
            }
        }
        else
        {
            _RemoveButtonEffect();
            _canUseSkillFalg = false;
        }
    }

    private void _RemoveButtonEffect()
    {
#if !LOGIC_SERVER
        if (button != null)
        {
            button.RemoveEffect(ETCButton.eEffectType.onContinue);
        }
#endif

    }

}

public class Skill1901 : Skill1514
{
    public Skill1901(int sid, int skillLevel) : base(sid, skillLevel)
    {

    }

    public override void OnInit()
    {
        base.OnInit();
        _isYinluo = false;
    }

}
