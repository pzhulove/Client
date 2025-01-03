using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using GameClient;
using System;

public class ComNewbieGuideWait : ComNewbieGuideBase
{
    public float mWaitTime = 0.0f;
    public bool mbPathThrough = false;

    #region vitrual
    public override void StartInit(params object[] args)
    {
        // 参数列表(可以只填参数0,不需要绑定任何界面)
        // 0.等待时间(一定要是浮点型数据,不然会报错,e.g : 4f)
        // 1.是否穿透
        // 2.保存点
        // 3.暂停单局
        // 4.恢复单局

        mWaitTime = 0.0f;
        mbPathThrough = false;

        base.StartInit(args);

        if (args != null)
        {
            if (args.Length >= 1)
            {
                mWaitTime = (float)args[0];
            }

            if (args.Length >= 2)
            {
                mbPathThrough = (bool)args[1];
            }

            if (args.Length >= 3)
            {
                if ((eNewbieGuideAgrsName)args[2] == eNewbieGuideAgrsName.SaveBoot)
                {
                    mSendSaveBoot = true;
                }
            }

            if (args.Length >= 4)
            {
                if ((eNewbieGuideAgrsName)args[3] == eNewbieGuideAgrsName.PauseBattle)
                {
                    mTryPauseBattle = true;
                }
            }

            if (args.Length >= 5)
            {
                if ((eNewbieGuideAgrsName)args[4] == eNewbieGuideAgrsName.ResumeBattle)
                {
                    mTryResumeBattle = true;
                }
            }
        }
    }

    protected override GuideState _init()
    {
        if(mFrameType == "" || mComRoot == "")
        {
            AddWaitTips(null, mbPathThrough);
            return GuideState.Normal;
        }
        else
        {
            var frameManager = ClientSystemManager.instance as IClientFrameManager;
            if (!frameManager.IsFrameOpen(mFrameType))
            {
                Logger.LogWarningFormat("新手引导---创建[wait]类型引导,未打开[0]界面,处于wait状态", mFrameType);
                return GuideState.Wait;
            }

            var ibind = frameManager.GetFrame(mFrameType) as IGameBind;
            if (ibind == null)
            {
                Logger.LogWarningFormat("新手引导---创建[wait]类型引导,未找到[0]界面的IGameBind,处于Exception状态", mFrameType);
                return GuideState.Exception;
            }

            var SourceRect = ibind.GetComponent<RectTransform>(mComRoot);
            if (SourceRect == null)
            {
                Logger.LogErrorFormat("SourceRect is nil with path {0}", mComRoot);
                return GuideState.Exception;
            }

            var gButton = AddWaitTips(SourceRect.gameObject, mbPathThrough);

            if (mTextTips.Length > 0)
            {
                AddTextTips(gButton, mAnchor, mTextTips, mTextTipType, mLocalPos);
            }

            return GuideState.Normal;
        }
    }

    protected override void _update()
    {
        if (mWaitTime > 0.0f)
        {
            mWaitTime -= Time.deltaTime;
        }
        else
        {
            if(mGuideControl != null)
            {
                mGuideControl.ControlComplete();
            }
        }
    }
    #endregion
}
