using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using GameClient;
using System;

public class ComNewbieGuideETCJoystick : ComNewbieGuideBase
{
    public float mPosX;

    #region vitrual
    public override void StartInit(params object[] args)
    {
        base.StartInit(args);

        mPosX = -1;
        mTryPauseBattleAI = true;

        if (args != null)
        {
            if (args.Length >= 1)
            {
                mPosX = (float)args[0];
            }
        }
    }

    protected override GuideState _init()
    {
        var frameManager = ClientSystemManager.instance as IClientFrameManager;

        var etcjoystick = Utility.FindGameObject(ClientSystemManager.instance.MiddleLayer, "ETCJoystick(Clone)");
        if (etcjoystick == null)
        {
            return GuideState.Exception;
        }

        var com = etcjoystick.GetComponent<ETCJoystick>();
        if (com == null || !com.isActiveAndEnabled)
        {
            Logger.LogProcessFormat("button is not enable with name {0}");
            return GuideState.Exception;
        }

        var goButton = AddDragTips(etcjoystick,
            move =>
        {
            com.onMove.Invoke(move);
        },
            () =>
        {
            com.onTouchUp.Invoke();
        },
            eNewbieGuideAnchor.Center);

        AddTextTips(goButton, eNewbieGuideAnchor.Top, "拖动摇杆进行移动");

        var main = BattleMain.instance.Main;
        if (main != null)
        {
            var region = main.AddRegion(new DRegionInfo()
            {
                resid = 13,
                regiontype = DRegionInfo.RegionType.Rectangle,
                rect = new Vector2(2, 20),
                position = new Vector3(mPosX, 0, 6.66f)
            }, null);

            region.active = false;
            region.activeEffect = false;

            region.active = true;
            region.activeEffect = true;

            bool isTrigger = false;

            region.triggerRegion = (info, target) =>
            {
                if (!isTrigger)
                {
                    isTrigger = true;
                    com.onTouchUp.Invoke();
                    mGuideControl.ControlComplete();
                }

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
