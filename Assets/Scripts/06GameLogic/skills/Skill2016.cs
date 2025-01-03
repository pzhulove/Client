
using System.Collections.Generic;
using GameClient;

/// <summary>
/// 空中杰克爆弹
/// </summary>
public class Skill2016 : BeSkill
{
    protected int _registerSkillId = 2016;
    protected int _airReplaceSkillId = 2017;

    protected List<IBeEventHandle> _handleList = new List<IBeEventHandle>();

    public Skill2016(int sid, int skillLevel):base(sid, skillLevel)
    {

    }

    public override void OnPostInit()
    {
        base.OnPostInit();
        _RemoveHandleList();
        _RegisterEvent();
    }

    public override bool CanForceUseSkill()
    {
        bool flag = base.CanForceUseSkill();
        if (flag) return flag;
        if (isCooldown) return flag;
        return _CanComboSkill();
    }

    protected void _RegisterEvent()
    {
        _handleList.Add(owner.RegisterEventNew(BeEventType.onReplaceSkill, _OnReplaceSkill));
    }

    protected void _OnReplaceSkill(BeEvent.BeEventParam param)
    {
        if (param.m_Int != _registerSkillId) return;
        if (owner.GetPosition().z <= 0) return;
        param.m_Int = _airReplaceSkillId;
    }

    protected bool _CanComboSkill()
    {
        var comboSkillID = owner.skillController.CheckComboSkill();
        return comboSkillID > 0;
    }

    protected void _RemoveHandleList()
    {
        for (int i = 0; i < _handleList.Count; i++)
        {
            if (_handleList[i] != null)
            {
                _handleList[i].Remove();
                _handleList[i] = null;
            }
        }
        _handleList.Clear();
    }
}
