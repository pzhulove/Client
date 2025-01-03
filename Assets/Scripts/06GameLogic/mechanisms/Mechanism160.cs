using System;
using System.Collections.Generic;
using UnityEngine;
public class Mechanism160 : BeMechanism
{
    int monsterCount;
    int useSkillID = 0;
    List<int> skillIDs = new List<int>();
    int durTime = 0;
    //int interval = 200;
    bool bValid = false;
    public Mechanism160(int mid, int lv) : base(mid, lv) { }
    
    public override void OnReset()
    {
        skillIDs.Clear();
        durTime = 0;
        bValid = false;
    }
    public override void OnInit()
    {
        monsterCount = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        useSkillID = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        for (int i = 0; i < data.ValueC.Length; i++)
        {
            skillIDs.Add(TableManager.GetValueFromUnionCell(data.ValueC[i], level));
        }
    }
    public override void OnStart()
    {

    }
    public override void OnUpdate(int deltaTime)
    {
        base.OnUpdate(deltaTime);
        if (bValid) return;
        if (owner == null) return;
        durTime += deltaTime;
        int curMonsterCount = 0;
        var entities = owner.CurrentBeScene.GetEntities();
        for (int i = 0; i < entities.Count; i++)
        {
            var curActor = entities[i] as BeActor;
            if (curActor != null && !curActor.IsDead() &&
                curActor.IsMonster() && !curActor.IsSkillMonster())
            {
                curMonsterCount++;
            }
        }
        if(curMonsterCount < monsterCount)
        {
            bValid = true;
            var usedSkill = owner.GetSkill(useSkillID);
            if(usedSkill != null)
            {
                int leftCD = usedSkill.LeftInitCd;
                usedSkill.AccTimeCD(leftCD);
                owner.UseSkill(useSkillID,true);
                for (int i = 0; i < skillIDs.Count; i++)
                {
                    var beEffectedSkill = owner.GetSkill(skillIDs[i]);
                    if(beEffectedSkill != null)
                    {
                        int accValue = beEffectedSkill.CDTimeAcc;
                        if(accValue > 0 && leftCD > 0)
                        {
                            beEffectedSkill.AccTimeCD(leftCD);
                        }
                    }
                }
            }
        }
    }
}
