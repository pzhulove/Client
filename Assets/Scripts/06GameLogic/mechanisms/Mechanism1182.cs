using System.Collections.Generic;
using GameClient;

/// <summary>
/// ���ܴ�ָ����Ž׶ο�ʼִ��
/// </summary>
public class Mechanism1182 : BeMechanism
{
    public Mechanism1182(int mid, int lv) : base(mid, lv) { }

    private struct SkillStartInfo
    {
        public int SkillId;
        public int StartPhase;
    }

    private List<SkillStartInfo> _skillStartInfoList = new List<SkillStartInfo>();

    public override void OnInit()
    {
        base.OnInit();
        _skillStartInfoList.Clear();
        for (int i = 0; i < data.ValueA.Count; i++)
        {
            SkillStartInfo info = new SkillStartInfo();
            info.SkillId = TableManager.GetValueFromUnionCell(data.ValueA[i], level);
            info.StartPhase = TableManager.GetValueFromUnionCell(data.ValueB[i], level);
            _skillStartInfoList.Add(info);
        }
    }

    public override void OnStart()
    {
        base.OnStart();
        _RegisterEvent();
    }

    private void _RegisterEvent()
    {
        handleA = owner.RegisterEventNew(BeEventType.onSkillStart, _OnSkillStar);
    }

    private void _OnSkillStar(BeEvent.BeEventParam param)
    {
        for (int i = 0; i < _skillStartInfoList.Count; i++)
        {
            var info = _skillStartInfoList[i];
            if (info.SkillId != param.m_Int) continue;
            var skill = param.m_Obj as BeSkill;
            if (skill != null)
                skill.SetSkillStartPhase(info.StartPhase);
        }
    }
}