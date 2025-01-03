using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 场景溶解机制
/// </summary>
public class Mechanism2058 : BeMechanism
{
    private string scenePath = "";
    private int time = 0;
    private int reversTime = 0;
    private float maxValue = 0;
    private int mechanismBuffID = 521665;
    public Mechanism2058(int id, int lv) : base(id, lv)
    { }
    public override void OnInit()
    {
        base.OnInit();
        scenePath = data.StringValueA[0];
        time = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        reversTime = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        maxValue = TableManager.GetValueFromUnionCell(data.ValueC[0], level)/1000.0f;
    }

    public override void OnStart()
    {
        //加载魔幻场景
        owner.buffController.TryAddBuff(mechanismBuffID, -1);
        owner.CurrentBeScene.DelayCaller.DelayCall(1000, () => 
        {
#if !LOGIC_SERVER
            if (owner.CurrentBeScene != null && owner.CurrentBeScene.currentGeScene != null)
            {
                owner.CurrentBeScene.currentGeScene.LoadMagicScene(scenePath, time, reversTime, maxValue);
            }
#endif
        });      

    }

    public override void OnFinish()
    {
        base.OnFinish();
        //移除魔幻场景
        owner.CurrentBeScene.DelayCaller.DelayCall(reversTime, () =>
        {
            owner.buffController.RemoveBuff(mechanismBuffID);
#if !LOGIC_SERVER
            if (owner.CurrentBeScene != null && owner.CurrentBeScene.currentGeScene != null)
            {
                owner.CurrentBeScene.currentGeScene.ReverseMaterialSpecialScene();
            }
#endif
        });
      

    }
}
