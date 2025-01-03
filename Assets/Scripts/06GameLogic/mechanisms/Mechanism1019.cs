using System.Collections.Generic;
using ProtoTable;

//增加角色技能攻击力百分比
public class Mechanism1019 : BeMechanism
{
    public Mechanism1019(int mid, int lv) : base(mid, lv) { }

    private VRate skillAttackAddRate = 0;     //技能攻击力增加百分比

    public override void OnInit()
    {
        base.OnInit();
        skillAttackAddRate = new VRate(TableManager.GetValueFromUnionCell(data.ValueA[0], level));
    }

    public override void OnStart()
    {
        base.OnStart();
        AttackAdd(skillAttackAddRate, true);
    }

    public override void OnFinish()
    {
        base.OnFinish();
        AttackAdd(skillAttackAddRate, false);
    }

    private void AttackAdd(VRate rate, bool isAdd = true)
    {
        if (owner == null)
            return;
        Dictionary<int, BeSkill> dic = owner.GetSkills();
        if (dic == null)
            return;
        Dictionary<int, BeSkill>.Enumerator it = dic.GetEnumerator();
        while (it.MoveNext())
        {
            int key = (int)it.Current.Key;
            BeSkill curSkill = (BeSkill)it.Current.Value;
            if (curSkill == null)
                continue;
            if (curSkill.skillCategory == 3 || curSkill.skillCategory == 4)
            {
                if (isAdd)
                    curSkill.attackAddRate += rate;
                if (!isAdd)
                    curSkill.attackAddRate -= rate;
            }
        }
    }
}
