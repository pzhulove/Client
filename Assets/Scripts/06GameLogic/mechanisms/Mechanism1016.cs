using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 正负极功能的随机添加机制
/// </summary>
public class Mechanism1016 : BeMechanism
{
    List<int> buffList = new List<int>();
    readonly int buffInfoID = 568929;
    readonly int buffInfoID01 = 568930;
    bool onlyMainActor = false;
    public Mechanism1016(int mid, int lv) : base(mid, lv) { }

    public override void OnInit()
    {
        base.OnInit();
        buffList.Clear();
        for (int i = 0; i < data.ValueA.Count; i++)
        {
            buffList.Add(TableManager.GetValueFromUnionCell(data.ValueA[i], level));
        }
        onlyMainActor = TableManager.GetValueFromUnionCell(data.ValueB[0], level) == 1;
    }

    public override void OnStart()
    {
        base.OnStart();
        if (onlyMainActor)
        {
            if (!owner.isMainActor) return;
        }
        BeBuff positiveBuff = owner.buffController.HasBuffByID(558929);
        BeBuff negtiveBuff = owner.buffController.HasBuffByID(558930);
        if (positiveBuff != null)
        {
            owner.buffController.RemoveBuff(558929);
            owner.buffController.TryAddBuff(buffInfoID01);
        }
        else if (negtiveBuff != null)
        {
            owner.buffController.RemoveBuff(558930);
            owner.buffController.TryAddBuff(buffInfoID);
        }
        else
        {

            int index = owner.FrameRandom.InRange(0, buffList.Count);
            owner.buffController.TryAddBuff(buffList[index]);
        }
    }
}
