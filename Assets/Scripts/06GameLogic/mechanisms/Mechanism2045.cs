using System;
using System.Collections.Generic;
//技能同步机制
public class Mechanism2045 : BeMechanism
{
    int monsterId = 0;
    int skillId = 0;
    public Mechanism2045(int sid, int skillLevel) : base(sid, skillLevel)
    {

    }
    public override void OnInit()
    {
        base.OnInit();
        monsterId = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        skillId = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
    }
    public override void OnStart()
    {
        base.OnStart();
        List<BeActor> list = GamePool.ListPool<BeActor>.Get();
        owner.CurrentBeScene.FindActorById2(list, monsterId);
        for(int i = 0; i < list.Count;i++)
        {
            var monster = list[i];
            if(monster != null && !monster.IsDead() && monster.GetPID() != owner.GetPID() && monster.CanUseSkill(skillId))
            {
                monster.UseSkill(skillId);
            }
        }
        GamePool.ListPool<BeActor>.Release(list);
    }
}
