using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using GameClient;
using System;

public class ComNewbieGuidePauseBattle : ComNewbieGuideBase
{
    #region vitrual
    protected override GuideState _init()
    {
        var dataMgr = BattleMain.instance.GetDungeonManager();
        if (null == dataMgr)
        {
            Logger.LogError("main is nil");
        }
        else
        {
            dataMgr.PauseFight(false, "newbie");
        }

        return GuideState.Normal;
    }
    #endregion
}
