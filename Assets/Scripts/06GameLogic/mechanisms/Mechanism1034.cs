using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameClient;

/// <summary>
/// 在被击者的位置有概率召唤一个怪物
/// </summary>
public class Mechanism1034 : BeMechanism
{
    public Mechanism1034(int id, int level) : base(id, level) { }

    private int monsterId = 0;  //召唤的怪物ID
    private int summonRate = 0; //召唤概率（千分比）
    private int summonNumLimit = 0; //召唤数量限制

    public override void OnInit()
    {
        monsterId = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        summonRate = TableManager.GetValueFromUnionCell(data.ValueB[0],level);
        summonNumLimit = TableManager.GetValueFromUnionCell(data.ValueC[0],level);
    }

    public override void OnStart()
    {
        base.OnStart();
        handleA = OwnerRegisterEventNew(BeEventType.onHitOther, HitOther);
        //handleA = owner.RegisterEvent(BeEventType.onHitOther, HitOther);
    }

    /// <summary>
    /// 攻击到目标
    /// </summary>
    /// <param name="args"></param>
    private void HitOther(BeEvent.BeEventParam param)
    {
        BeActor target = param.m_Obj as BeActor;
        if (target == null)
            return;
        int random = FrameRandom.Random((uint)GlobalLogic.VALUE_1000);
        if (summonRate < random)
            return;
        int summonId = owner.GenNewMonsterID(monsterId, level);
        int monsterCount = 0;
        List<BeActor> list = GamePool.ListPool<BeActor>.Get();
        owner.CurrentBeScene.FindActorById2(list, summonId);
        monsterCount = list.Count;
        GamePool.ListPool<BeActor>.Release(list);
        if (summonNumLimit!=0 && monsterCount >= summonNumLimit)
            return;
        owner.CurrentBeScene.SummonMonster(summonId, target.GetPosition(), owner.m_iCamp);
    }
}
