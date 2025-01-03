using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 判断场上是否存在指定ID的怪物 存在给自己添加一个BuffA没有给自己添加一个BuffB
/// </summary>
public class Mechanism1032 : BeMechanism
{
    public Mechanism1032(int id, int level) : base(id, level) { }

    protected int monsterId = 0;    //怪物ID
    protected int[] buffInfoIdArr = new int[2]; //需要添加的Buff信息ID

    public override void OnInit()
    {
        monsterId = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        buffInfoIdArr[0] = TableManager.GetValueFromUnionCell(data.ValueB[0],level);
        buffInfoIdArr[1] = TableManager.GetValueFromUnionCell(data.ValueB[1],level);
    }

    public override void OnStart()
    {
        base.OnStart();
        CheckMonsterExist();
    }

    /// <summary>
    /// 检查场上是否有指定ID的怪物存在
    /// </summary>
    private void CheckMonsterExist()
    {
        List<BeActor> list = GamePool.ListPool<BeActor>.Get();
        owner.CurrentBeScene.FindActorById2(list, monsterId);
        if (list.Count > 0)
        {
            owner.buffController.TryAddBuff(buffInfoIdArr[0]);
        }
        else
        {
            owner.buffController.TryAddBuff(buffInfoIdArr[1]);
        }
        GamePool.ListPool<BeActor>.Release(list);
    }
}
