using System.Collections.Generic;
using System;

/// <summary>
/// 召唤师觉醒被动
/// </summary>
public class Mechanism2038 : BeMechanism
{
    private int skillID;
    private int[] buffIDs;
    private int[] monsterIDs;
    private int skillLevel = 0;
    private List<BeActor> monsterList = new List<BeActor>();
    public Mechanism2038(int mid, int lv) : base(mid, lv)
    {
    }

    public override void OnInit()
    {
        base.OnInit();
        buffIDs = new int[data.ValueA.Length];
        for (int i = 0; i < data.ValueA.Length; i++)
        {
            buffIDs[i] = TableManager.GetValueFromUnionCell(data.ValueA[i], level);
        }
        monsterIDs = new int[data.ValueB.Length];
        for (int i = 0; i < data.ValueB.Length; i++)
        {
            monsterIDs[i] = TableManager.GetValueFromUnionCell(data.ValueB[i], level);
        }
        skillID = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
    }

    public override void OnReset()
    {
        skillLevel = 0;
        monsterList.Clear();
    }

    public override void OnStart()
    {
        monsterList.Clear();
        base.OnStart();
        AddBuffToMonster();
        handleA = owner.RegisterEventNew(BeEventType.onDead, eventParam =>
        {
            for (int i = 0; i < monsterList.Count; i++)
            {
                for (int j = 0; j < buffIDs.Length; j++)
                {
                    monsterList[i].buffController.RemoveBuff(buffIDs[j]);
                }
            }
        });
    }

    private int GetSkillLevel()
    {
        BeActor master = owner.GetOwner() as BeActor;
        BeSkill skill = master.GetSkill(skillID);
        if (skill != null)
            return skill.level;
        return 0;
    }

    private void AddBuffToMonster()
    {
        BeActor master = owner.GetOwner() as BeActor;
        if (master != null)
        {
            handleB = master.RegisterEventNew(BeEventType.onSummon, (args) =>
            {
                BeActor summonMonster = args.m_Obj as BeActor;
                if (summonMonster != null && Array.IndexOf(monsterIDs, summonMonster.GetEntityData().monsterID) != -1)
                {
                    monsterList.Add(summonMonster);
                    for (int i = 0; i < buffIDs.Length; i++)
                    {
                        summonMonster.buffController.TryAddBuff(buffIDs[i], -1, GetSkillLevel());
                    }
                }
            });
        }

        for (int i = 0; i < monsterIDs.Length; i++)
        {
            var list = GamePool.ListPool<BeActor>.Get();
            owner.CurrentBeScene.FindMonsterByID(list, monsterIDs[i], false);
            for (int j = 0; j < list.Count; j++)
            {
                if (list[j].GetOwner() == master)
                {
                    monsterList.Add(list[j]);
                    for (int m = 0; m < buffIDs.Length; m++)
                    {
                        list[j].buffController.TryAddBuff(buffIDs[m], -1, GetSkillLevel());
                    }
                }
            }
            GamePool.ListPool<BeActor>.Release(list);
        }
    }
}
