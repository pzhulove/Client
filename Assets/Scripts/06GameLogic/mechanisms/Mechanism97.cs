using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 随机一个或多个指定怪物ID释放技能
/// </summary>
public class Mechanism97 : BeMechanism
{
    readonly private int protectID = 30230011;
    private int monsterID = 0;
    private int skillID = 0;
    private int cnt = 0;
    public Mechanism97(int mid, int lv) : base(mid, lv){}

    public override void OnInit()
    {
        monsterID = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        skillID = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        cnt = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
    }

    public override void OnStart()
    {
        base.OnStart();
        List<BeActor> monsterList = GamePool.ListPool<BeActor>.Get();
        owner.CurrentBeScene.FindMonsterByID(monsterList, monsterID);
        UseSkill(monsterList,1);
        GamePool.ListPool<BeActor>.Release(monsterList);
    }

    private void UseSkill(List<BeActor> list,int index)
    {
        if (index > cnt||list.Count==0) return;
        BeActor actor = list[FrameRandom.InRange(0, list.Count - 1)];

        List<BeActor> monsterList = GamePool.ListPool<BeActor>.Get();
        owner.CurrentBeScene.FindMonsterByID(monsterList, protectID);
        if (monsterList.Count > 0)
        {
            GamePool.ListPool<BeActor>.Release(monsterList);
            return;
        }
        else
        {
            actor.UseSkill(skillID);
        }
        GamePool.ListPool<BeActor>.Release(monsterList);

        list.Remove(actor);
        index++;
        UseSkill(list,index);
    }

    public override void OnUpdate(int deltaTime)
    {
        base.OnUpdate(deltaTime);
    }

    public override void OnFinish()
    {
        base.OnFinish();
    }
}
