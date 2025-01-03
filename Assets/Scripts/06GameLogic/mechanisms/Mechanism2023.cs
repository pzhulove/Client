using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//光之复仇装备机制
public class Mechanism2023: BeMechanism
{
    public Mechanism2023(int sid, int skillLevel) : base(sid, skillLevel){}

    public int addEntityRate = 0;      //增加创建实体概率（千分比）

    public override void OnInit()
    {
        base.OnInit();
        addEntityRate = TableManager.GetValueFromUnionCell(data.ValueA[0],level);
    }
}
