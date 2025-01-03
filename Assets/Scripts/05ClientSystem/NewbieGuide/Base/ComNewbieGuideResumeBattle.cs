using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using GameClient;
using System;

public class ComNewbieGuideResumeBattle : ComNewbieGuideBase
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
            dataMgr.ResumeFight(false, "newbie");
        }

        return GuideState.Normal;
    }
    #endregion
}
