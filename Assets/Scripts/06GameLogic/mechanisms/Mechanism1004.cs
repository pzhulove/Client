using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//添加角色抖动
public class Mechanism1004 : BeMechanism
{
    public Mechanism1004(int mid, int lv) : base(mid, lv){ }

    protected int time = 0;     //抖动时间
    protected int xRange = 0;   //x轴范围
    protected int yRange = 0;   //y轴范围 
    protected int mode = 0;     //抖动模式
    protected int speed = 0;    //抖动速度

    public override void OnInit()
    {
        base.OnInit();
        time = TableManager.GetValueFromUnionCell(data.ValueA[0],level);
        xRange = TableManager.GetValueFromUnionCell(data.ValueB[0],level);
        yRange = TableManager.GetValueFromUnionCell(data.ValueC[0],level);
        mode = TableManager.GetValueFromUnionCell(data.ValueD[0], level);
        speed = TableManager.GetValueFromUnionCell(data.ValueE[0],level);
    }

    public override void OnStart()
    {
#if !LOGIC_SERVER
        base.OnStart();
        ShockData shockData = new ShockData();
        shockData.time = time / 1000.0f;
        shockData.xrange = xRange / 1000.0f;
        shockData.yrange = yRange / 1000.0f;
        shockData.mode = mode;
        shockData.speed = speed / 1000.0f;
        owner.AddShock(shockData);
#endif
    }
}
