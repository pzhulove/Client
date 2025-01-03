using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 指定ID的怪物进入自己范围会给对方添加一个Buff信息
/// </summary>
public class Mechanism1047 : BeMechanism
{
    public Mechanism1047(int id, int level) : base(id, level) { }

    private int monsterId = 0;  //怪物ID
    private VInt radius = 0; //半径范围
    private int buffInfoId = 0; //给对方添加的Buff信息

    private readonly int timeAcc = 500;  //更新频率
    private bool addBuffFlag = false;
    

    public override void OnInit()
    {
        base.OnInit();
        monsterId = TableManager.GetValueFromUnionCell(data.ValueA[0],level);
        radius = VInt.NewVInt(TableManager.GetValueFromUnionCell(data.ValueB[0],level),GlobalLogic.VALUE_1000);
        buffInfoId = TableManager.GetValueFromUnionCell(data.ValueC[0],level);
        addBuffFlag = false;
    }

    public override void OnStart()
    {
        base.OnStart();
        InitTimeAcc(timeAcc);
    }

    public override void OnUpdateTimeAcc()
    {
        if (addBuffFlag)
            return;
        base.OnUpdateTimeAcc();
        List<BeActor> list = GamePool.ListPool<BeActor>.Get();
        owner.CurrentBeScene.FindActorInRange(list,owner.GetPosition(), radius,owner.GetCamp(), monsterId);
        if (list.Count > 0)
        {
            addBuffFlag = true;
            list[0].buffController.TryAddBuffInfo(buffInfoId,owner,level);
            owner.DoDead();
        }
        GamePool.ListPool<BeActor>.Release(list);
    }
}
