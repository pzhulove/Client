using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using GameClient;
using System;

public class ComNewbieGuideStress : ComNewbieGuideBase
{
    public bool bWait = true;
    public float mWaitTime = 2.0f;

    #region vitrual
    public override void StartInit(params object[] args)
    {
        // 参数列表
        // 0.绑定界面的cs文件的名称
        // 1.绑定控件路径
        // 2.是否等待
        // 3.等待时间
        // 4.保存点
        // 5.暂停单局
        // 6.恢复单局

        bWait = true;
        mWaitTime = 2.0f;

        base.StartInit(args);

        if (args != null)
        {
            if (args.Length >= 1)
            {
                mFrameType = args[0] as string;
            }

            if (args.Length >= 2)
            {
                mComRoot = args[1] as string;
            }

            if (args.Length >= 3)
            {
                bWait = (bool)args[2];
            }

            if (args.Length >= 4)
            {
                mWaitTime = (float)args[3];
            }

            if (args.Length >= 5)
            {
                if ((eNewbieGuideAgrsName)args[4] == eNewbieGuideAgrsName.SaveBoot)
                {
                    mSendSaveBoot = true;
                }
            }

            if (args.Length >= 6)
            {
                if ((eNewbieGuideAgrsName)args[5] == eNewbieGuideAgrsName.PauseBattle)
                {
                    mTryPauseBattle = true;
                }
            }

            if(args.Length >= 7)
            {
                if ((eNewbieGuideAgrsName)args[6] == eNewbieGuideAgrsName.ResumeBattle)
                {
                    mTryResumeBattle = true;
                }
            }
        }
    }

    protected override GuideState _init()
    {
        IGameBind ibind = null;

        if (mFrameType == "ClientSystemBattle")
        {
            ibind = ClientSystemManager.instance.CurrentSystem as IGameBind;
        }
        else
        {
            var frameManager = ClientSystemManager.instance as IClientFrameManager;

            if (frameManager.IsFrameOpen(mFrameType))
            {
                ibind = frameManager.GetFrame(mFrameType) as IGameBind;
            }
        }

        if (ibind == null)
        {
            return GuideState.Wait;
        }

        var button = ibind.GetComponent<Button>(mComRoot);
        if (button == null)
        {
            Logger.LogErrorFormat("button is nil with path {0}", mComRoot);
            return GuideState.Exception;
        }

        if (!AddStress(button.gameObject))
        {
            return GuideState.Exception;
        }
       
        return GuideState.Normal;
    }

    protected override void _update()
    {
        if(bWait)
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
    }
    #endregion
}
