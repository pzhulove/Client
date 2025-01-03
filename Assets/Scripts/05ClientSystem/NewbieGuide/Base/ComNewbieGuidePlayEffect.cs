using UnityEngine.UI;
using GameClient;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class ComNewbieGuidePlayEffect : ComNewbieGuideBase
{
    public string LoadResFile = "";
    public float mWaitTime = 0.0f;

    #region vitrual
    public override void StartInit(params object[] args)
    {
        // 参数列表
        // 0.特效所在预制体的路径
        // 1.保存点

        base.StartInit(args);

        if (args != null)
        {
            if (args.Length >= 1)
            {
                LoadResFile = args[0] as string;
            }

            if (args.Length >= 2)
            {
                if((eNewbieGuideAgrsName)args[1] == eNewbieGuideAgrsName.SaveBoot)
                {
                    mSendSaveBoot = true;
                }          
            }

            if (args.Length >= 3)
            {
                mWaitTime = (float)args[2];
            }
        }
    }

    protected override GuideState _init()
    {
        if (!AddEffect(LoadResFile))
        {
            return GuideState.Exception;
        }

        return GuideState.Normal;
    }

    protected override void _update()
    {
        if (mWaitTime > 0.0f)
        {
            mWaitTime -= Time.deltaTime;
        }
        else
        {
            if (mGuideControl != null)
            {
                mGuideControl.ControlComplete();
            }
        }
    }

    #endregion
}
