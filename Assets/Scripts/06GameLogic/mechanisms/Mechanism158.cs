using System;
using System.Collections.Generic;

class Mechanism158 : BeMechanism
{
    public Mechanism158(int mid, int lv) : base(mid, lv) { }
    bool isLeft = false;
    bool isOldLeft = false;
    public override void OnInit()
    {
        isOldLeft = false;
        
        base.OnInit();
        isLeft = TableManager.GetValueFromUnionCell(data.ValueA[0], level) == 1 ? true : false;
    }
    public override void OnStart()
    {
        if(owner != null)
        {
            isOldLeft = owner.GetFace();
            owner.SetFace(isLeft,true,true);
        }
    }
    public override void OnFinish()
    {
        if (owner != null)
        {
            owner.SetFace(isOldLeft,true,true);
        }
    }
}

