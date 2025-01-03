using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Skill1100 : BeSkill
{
    List<int> effectSkills = null;
    int attackSpeed = 0;
    int normalAttackSpeedAdd = 0;
    int normalAttackAtttackAdd = 0;

    public Skill1100(int sid, int skillLevel):base(sid, skillLevel){}

    public sealed override void OnPostInit()
    {
        base.OnPostInit();

        DoEffect(true);

        effectSkills = GetEffectSkills(skillData.ValueD, level);
        attackSpeed = TableManager.GetValueFromUnionCell(skillData.ValueA[0], level);

        normalAttackSpeedAdd = TableManager.GetValueFromUnionCell(skillData.ValueB[0], level);
        normalAttackAtttackAdd = TableManager.GetValueFromUnionCell(skillData.ValueC[0], level);
        DoEffect();
    }


    void DoEffect(bool restore=false)
    {
        if (owner.GetEntityData() != null)
        {
            //增加通用攻速
            var attribute = owner.GetEntityData();
            if (attackSpeed != 0)
                attribute.SetAttributeValue(AttributeType.attackSpeed, restore?-attackSpeed:attackSpeed, true);

            //Logger.LogErrorFormat("add actor speed {0}", attackSpeed);
            if (effectSkills == null)
                return;

            for(int i=0; i<effectSkills.Count; ++i)
            {
                BeSkill skill = owner.GetSkill(effectSkills[i]);
                if (skill != null)
                {
                    if (!restore)
                    {
                        skill.skillSpeedFactor += normalAttackSpeedAdd;
                        skill.attackAddRate += normalAttackAtttackAdd;
                    }
                    else
                    {
                        skill.skillSpeedFactor -= normalAttackSpeedAdd;
                        skill.attackAddRate -= normalAttackAtttackAdd;
                    }
                    //Logger.LogErrorFormat("skill:{0} speedFactor:{1} attackAttackRate:{2}", effectSkills[i], skill.skillSpeedFactor, skill.attackAddRate);
                }
            }
        }
    }
}
