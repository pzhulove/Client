using System;
using System.Collections.Generic;
using UnityEngine;

//每隔一段时间给场上的怪物添加BUFF信息（不重复添加）
class Mechanism134 : BeMechanism
{
    int interval;
    int[] buffInfoArray;

    int timer;
    List<BeActor> targetList = new List<BeActor>();

    public Mechanism134(int mid, int lv) : base(mid, lv) { }

    public override void OnReset()
    {
        timer = 0;
        targetList.Clear();
    }
    public override void OnInit()
    {
        interval = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        buffInfoArray = new int[data.ValueB.Length];
        for (int i = 0; i < data.ValueB.Length; i++)
            buffInfoArray[i] = TableManager.GetValueFromUnionCell(data.ValueB[i], level);
    }

    public override void OnStart()
    {
        timer = 0;
    }

    public override void OnUpdate(int deltaTime)
    {
        timer += deltaTime;
        if (timer >= interval)
        {
            timer -= interval;
            AddBuffInfoToMonsters();
        }
    }

    void AddBuffInfoToMonsters()
    {
        var targets = GamePool.ListPool<BeActor>.Get();
        owner.CurrentBeScene.FindTargets(targets, owner, VInt.Float2VIntValue(100f));
        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i] != null && !targets[i].IsDead() && !targetList.Contains(targets[i]))
            {
                for (int j = 0; j < buffInfoArray.Length; j++)
                {
                    targets[i].buffController.TryAddBuff(buffInfoArray[j], owner);
                }
                targetList.Add(targets[i]);
            }
        }
        GamePool.ListPool<BeActor>.Release(targets);
    }

    public override void OnFinish()
    {
        for (int i = 0; i < targetList.Count; i++)
        {
            for (int j = 0; j < buffInfoArray.Length; j++)
            {
                targetList[i].buffController.RemoveBuffByBuffInfoID(buffInfoArray[j]);
            }
        }
        targetList.Clear();
    }
}