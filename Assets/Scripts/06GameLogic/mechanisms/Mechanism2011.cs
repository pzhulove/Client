using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 黑色大地boss关卡变身机制
/// </summary>
public class Mechanism2011 : BeMechanism
{
    List<BeActor> list = new List<BeActor>();
    private int[] buffInfoList = new int[2];
    private int buffInfoID;
    public Mechanism2011(int mid, int lv) : base(mid, lv)
    {

    }

    public override void OnInit()
    {
        base.OnInit();
        buffInfoList = new int[data.ValueA.Count];
        for (int i = 0; i < data.ValueA.Count; i++)
        {
            buffInfoList[i] = TableManager.GetValueFromUnionCell(data.ValueA[i], level);
        }
        buffInfoID = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
    }

    public override void OnReset()
    {
        list.Clear();
    }

    /// <summary>
    /// 通过给玩家上不同的buff信息变身不同的怪物
    /// </summary>
    public override void OnStart()
    {
        base.OnStart();
        owner.CurrentBeScene.FindMainActor(list);
        if (list.Count > 1)
        {
            int index = owner.FrameRandom.InRange(0, list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                if (IsJueXingSkill(list[i]) || list[i].isSpecialMonster) continue;
                if (i == index)
                {
                    list[i].buffController.TryAddBuff(buffInfoList[0]);
                }
                else
                {
                    list[i].buffController.TryAddBuff(buffInfoList[1]);
                }
            }
        }
        else if (list.Count == 1)
        {
            if (IsJueXingSkill(list[0]) || list[0].isSpecialMonster) return;
            list[0].buffController.TryAddBuff(buffInfoID);
        }
    }

    private bool IsJueXingSkill(BeActor actor)
    {
        BeSkill skill = actor.skillController.GetCurrentSkill();
        if (skill != null)
        {
            if (skill.skillData.SkillCategory == 4)
                return true;
        }
        return false;
    }

    public override void OnFinish()
    {
        base.OnFinish();
    }
}
