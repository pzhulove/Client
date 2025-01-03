using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 自己身上有个Buff时指定触发效果攻击到别人时给自己添加一个Buff信息和给别人添加一个Buff信息
/// </summary>
public class Mechanism151 : BeMechanism
{
    private int buffID = 0;
    private List<int> hurtIDList = new List<int>();
    private int ownerBuffInfoID = 0;
    private int targetBuffInfoId = 0;

    public Mechanism151(int mid, int lv) : base(mid, lv) { }

    public override void OnInit()
    {
        base.OnInit();
        buffID = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        hurtIDList.Clear();
        for (int i = 0; i < data.ValueB.Count; i++)
        {
            hurtIDList.Add(TableManager.GetValueFromUnionCell(data.ValueB[i], level));
        }
        ownerBuffInfoID = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
        targetBuffInfoId = TableManager.GetValueFromUnionCell(data.ValueD[0], level);
    }

    public override void OnStart()
    {
        base.OnStart();
        handleA = owner.RegisterEventNew(BeEventType.onBeforeHit, (args) =>
        //handleA = owner.RegisterEvent(BeEventType.onBeforeHit, (args) =>
        {
            if (owner.buffController.HasBuffByID(buffID) != null)
            {
                BeActor actor = args.m_Obj as BeActor;
                int hurtID = args.m_Int;
                if (actor != null && hurtIDList.Contains(hurtID))
                {
                    //给自己添加的Buff信息
                    if (ownerBuffInfoID != 0)
                        owner.buffController.TryAddBuffInfo(ownerBuffInfoID, owner, level);
                    //给攻击目标添加的Buff信息
                    if (targetBuffInfoId != 0)
                        actor.buffController.TryAddBuffInfo(targetBuffInfoId, owner, level);
                }
            }
        });
    }
}
