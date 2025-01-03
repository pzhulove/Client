using System.Collections;
using System.Collections.Generic;
using GameClient;
using UnityEngine;

/// <summary>
/// 在PVP中召唤兽的属性读取新的怪物的数据
/// </summary>
public class Mechanism1072 : BeMechanism
{
    public Mechanism1072(int id, int level) : base(id, level) { }

    protected List<int> monsterIdOldList = new List<int>();     //老的怪物ID
    protected List<int> monsterIdNewList = new List<int>();     //替换的怪物ID

    public override void OnInit()
    {
        base.OnInit();
        monsterIdOldList.Clear();
        monsterIdNewList.Clear();
        for (int i = 0; i < data.ValueA.Count; i++)
        {
            monsterIdOldList.Add(TableManager.GetValueFromUnionCell(data.ValueA[i], level));
        }

        for (int i = 0; i < data.ValueB.Count; i++)
        {
            monsterIdNewList.Add(TableManager.GetValueFromUnionCell(data.ValueB[i], level));
        }
    }

    public override void OnStart()
    {
        base.OnStart();
        RegisterEvent();
    }

    protected void RegisterEvent()
    {
        //只有召唤师才会生效
        if (owner.GetEntityData().professtion != 33)
            return;
        if (!BattleMain.IsModePvP(battleType))
            return;
        if (owner.CurrentBeScene == null)
            return;
        sceneHandleA = owner.CurrentBeScene.RegisterEventNew(BeEventSceneType.onChangeSummonMonsterAttr, ChangeSummonMonsterAttr);
    }

    /// <summary>
    /// 改变召唤兽的属性
    /// </summary>
    protected void ChangeSummonMonsterAttr(BeEvent.BeEventParam args)
    {
        int index = monsterIdOldList.FindIndex((x) => x == args.m_Int);
        if (index < 0)
            return;
        args.m_Int = monsterIdNewList[index];
    }
}
