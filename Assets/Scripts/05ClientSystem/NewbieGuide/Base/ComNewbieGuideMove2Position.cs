using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using GameClient;
using System;

public class ComNewbieGuideMove2Position : ComNewbieGuideBase
{
    #region vitrual
    public override void StartInit(params object[] args)
    {
        mTryPauseBattleAI = true;
    }

    protected override GuideState _init()
    {
        var main = BattleMain.instance.Main;
        if (main != null)
        {
            var region = main.AddRegion(new DRegionInfo()
            {
                resid = 1,
                position = new Vector3(-12.924f, 0, 6.66f)
            }, null);

            region.triggerRegion = (info, target) =>
            {
                mGuideControl.ControlComplete();
				return true;
            };
        }
        else
        {
            Logger.LogError("main is nil");
        }

        return GuideState.Normal;
    }
    #endregion
}
