using System;
using System.Collections.Generic;
using UnityEngine;

//场景变暗机制
class Mechanism102 : BeMechanism
{
    int darkFactor = 0;
    int delayHideTime = 0;
    int delayShowTime = 0;
    int flag = 0;
    int blackSceneID = -1;
    bool isAlpha = true;
    DelayCallUnitHandle handle;
    public Mechanism102(int mid, int lv) : base(mid, lv) { }

    public override void OnReset()
    {
        blackSceneID = -1;
    }
    public override void OnInit()
    {
        darkFactor = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        delayHideTime = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        delayShowTime = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
        flag = TableManager.GetValueFromUnionCell(data.ValueD[0], level);
        isAlpha = TableManager.GetValueFromUnionCell(data.ValueE[0], level) == 0 ? true : false;
    }

    private void _switchSceneEffect(bool flag)
    {
#if !LOGIC_SERVER
        if (this.flag == 1)
        {
            if (!owner.isLocalActor) return;
        }
        if (flag)
        {
            //场景变暗
            blackSceneID = owner.CurrentBeScene.currentGeScene.BlendSceneSceneColor(Color.white* (darkFactor/1000.0f + 0.005f), delayHideTime / 1000.0f,isAlpha);
            if (darkFactor == 0)
            {
                handle = owner.delayCaller.DelayCall(delayHideTime, () =>
                {
                    //变暗之后隐藏场景
                    owner.CurrentBeScene.currentGeScene.GetSceneObject().CustomActive(false);
                    owner.CurrentBeScene.currentGeScene.GetSceneActorRoot().CustomActive(false);
                });
            }
        }
        else
        {
            //还原场景
            owner.CurrentBeScene.currentGeScene.RecoverSceneColor(delayShowTime/1000.0f, blackSceneID);
            if (darkFactor == 0)
            {
                owner.CurrentBeScene.currentGeScene.GetSceneObject().CustomActive(true);
                owner.CurrentBeScene.currentGeScene.GetSceneActorRoot().CustomActive(true);

            }
        }
        
#endif
    }
    public override void OnStart()
    {
        _switchSceneEffect(true);
    }

    public override void OnFinish()
    {
        _switchSceneEffect(false);
        if (handle.IsValid())
        {
            handle.SetRemove(true);
        }
    }

}