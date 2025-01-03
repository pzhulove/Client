using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 场景变色机制
public class Mechanism10021 : BeMechanism
{
    int blackSceneID = -1;
    int delayShowTime = 0;
    int delayHideTime = 0;
    int[] color = new[] {255, 255, 255, 255};
    bool alphaEffect = false; 
    bool IsUnique = false; 
    
    public Mechanism10021(int mid, int lv) : base(mid, lv) { }

    public override void OnReset()
    {

    }
    
    public override void OnInit()
    {
        if (data.ValueA.Count == 4)
        {
            for (int i = 0; i < data.ValueA.Count; i++)
            {
                color[i] = TableManager.GetValueFromUnionCell(data.ValueA[i], level);
            }
        }

        delayShowTime = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        delayHideTime = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
        if (TableManager.GetValueFromUnionCell(data.ValueD[0], level) == 1)
        {
            alphaEffect = true;
        }
        if (TableManager.GetValueFromUnionCell(data.ValueE[0], level) == 1)
        {
            IsUnique = true;
        }
    }

    public override void OnStart()
    {
        _switchSceneEffect(true);
    }

    private void _switchSceneEffect(bool flag)
    {
#if !LOGIC_SERVER
        if (flag)
        {
            blackSceneID = owner.CurrentBeScene.currentGeScene.BlendSceneSceneColor(new Color(color[0]/255,color[1]/255,color[2]/255,color[3]/255), delayShowTime / 1000.0f, alphaEffect, IsUnique);
        }
        else
        {
            owner.CurrentBeScene.currentGeScene.RecoverSceneColor(delayHideTime / 1000.0f, blackSceneID);
        }
#endif
    }

    public override void OnFinish()
    {
        _switchSceneEffect(false);
    }
}
