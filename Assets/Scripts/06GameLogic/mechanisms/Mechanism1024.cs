using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 根据场上剩余怪物的数量添加buffinfoID
/// </summary>
public class Mechanism1024 : BeMechanism
{
    int monsterNum = 0;
    int buffID = 0;

    int curTimeAcc = 0;
    readonly int timeAcc = 1000; 

    public Mechanism1024(int mid, int lv) : base(mid, lv) { }

    public override void OnInit()
    {
        base.OnInit();
        monsterNum = TableManager.GetValueFromUnionCell(data.ValueA[0],level);
        buffID = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
    }

    public override void OnReset()
    {
        curTimeAcc = 0;
    } 

    public override void OnUpdate(int deltaTime)
    {
        base.OnUpdate(deltaTime);
        UpdateBuffInfo(deltaTime);
    }

    private void UpdateBuffInfo(int deltaTime)
    {
        if (!CheckCD(deltaTime))
            return;

        if (owner.CurrentBeScene.GetMonsterCount() <= monsterNum)
        {
            owner.buffController.TryAddBuff(buffID, -1, level);
        }

        if (owner.CurrentBeScene.GetMonsterCount() > monsterNum)
        {
            owner.buffController.RemoveBuff(buffID);
        }
    }

    private bool CheckCD(int deltaTime)
    {
        if (curTimeAcc < timeAcc)
        {
            curTimeAcc += deltaTime;
            return false;
        }
        else
        {
            curTimeAcc = 0;
            return true;
        }
    }
}
