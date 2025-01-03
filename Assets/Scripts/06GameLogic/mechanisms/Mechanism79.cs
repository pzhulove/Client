using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mechanism79 : BeMechanism
{
    int height;

    public Mechanism79(int sid, int skillLevel) : base(sid, skillLevel) { }
    
    public override void OnInit()
    {
        height = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
    }

    public override void OnStart()
    {
        owner.SetFloating(VInt.NewVInt(height, GlobalLogic.VALUE_1000), false);
        if (height <= 0)
        {
            owner.RemoveFloating();
        }
    }
    
}
