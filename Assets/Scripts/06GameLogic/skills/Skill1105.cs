using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Skill1105 : BeSkill
{

    VRate hitThroughRate = 0;
    List<int> effectSkills = new List<int>();
    IBeEventHandle handler;
    int weaponType = 0;
    public Skill1105(int sid, int skillLevel):base(sid, skillLevel)
    {

    }

    public override void OnInit()
    {
        base.OnInit();

		hitThroughRate = TableManager.GetValueFromUnionCell(skillData.ValueA[0], level);

        effectSkills.Clear();
        for (int i = 0; i < skillData.ValueB.Count; ++i)
        {
            int sid = TableManager.GetValueFromUnionCell(skillData.ValueB[i], level);
            effectSkills.Add(sid);
        }
        weaponType = TableManager.GetValueFromUnionCell(skillData.ValueC[0], level);
    }

    public override void OnPostInit()
    {
        base.OnPostInit();
        RemoveHandle();
        DoEffect();
    }

    void DoEffect()
    {
        if (owner == null || owner.GetWeaponType() != weaponType)
            return;

        if (owner.buffController.HasBuffByID(1400326) != null)
            hitThroughRate = TableManager.GetValueFromUnionCell(skillData.ValueA[0], level) + new VRate(0.06f);

        handler = owner.RegisterEventNew(BeEventType.onAfterGenBullet, (args) =>
        {
            BeProjectile projectile = args.m_Obj as BeProjectile;

            int curSkill = owner.GetCurSkillID();
            if (projectile != null && effectSkills.Contains(curSkill))
            {
                projectile.hitThroughFactor += hitThroughRate;
               // Logger.LogErrorFormat("add projectile hit through rate:{0}", hitThroughRate);
            }
        });
    }

    void RemoveHandle()
    {
        if (handler != null)
        {
            handler.Remove();
            handler = null;
        }
    }

    public override void OnWeaponChange()
    {
        OnPostInit();
    }
}
