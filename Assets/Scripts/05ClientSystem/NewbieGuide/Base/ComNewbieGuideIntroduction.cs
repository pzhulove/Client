using UnityEngine;
using GameClient;

public class ComNewbieGuideIntroduction : ComNewbieGuideBase
{
    #region vitrual

    public bool bOpenAutoClose = false;
    public float mWaitTime = 10.0f;

    public override void StartInit(params object[] args)
    {
        // 参数列表
        // 0.绑定界面的cs文件的名称
        // 1.绑定控件路径
        // 2.引导文字
        // 3.显示位置
        // 4.文字模板类型
        // 5.坐标微调
        // 6.保存点
        // 7.暂停单局
        // 8.需要高亮显示的节点及其子节点
        // 9.是否开启自动关闭(默认关闭)
        // 10.等待时间(可以不填，默认10秒)

        bOpenAutoClose = false;
        mWaitTime = 10.0f;

        base.StartInit(args);

        if (args != null)
        {
            if (args.Length >= 2)
            {
                mFrameType = args[0] as string;
                mComRoot = args[1] as string;
            }

            if (args.Length >= 3)
            {
                mTextTips = args[2] as string;
            }

            if (args.Length >= 4)
            {
                mAnchor = (eNewbieGuideAnchor)args[3];
            }

            if (args.Length >= 5)
            {
                mTextTipType = (TextTipType)args[4];
            }

            if (args.Length >= 6)
            {
                mLocalPos = (Vector3)args[5];
            }

            if (args.Length >= 7)
            {
                if((eNewbieGuideAgrsName)args[6] == eNewbieGuideAgrsName.SaveBoot)
                {
                    mSendSaveBoot = true;
                }         
            }

            if(args.Length >= 8)
            {
                if ((eNewbieGuideAgrsName)args[7] == eNewbieGuideAgrsName.PauseBattle)
                {
                    mTryPauseBattle = true;
                }
            }

            if (args.Length >= 9)
            {
                if (args[8] as string != "")
                {
                    mHighLightPointPath = args[8] as string;
                }
            }

            if (args.Length >= 10)
            {
                if ((eNewbieGuideAgrsName)args[9] == eNewbieGuideAgrsName.AutoClose)
                {
                    bOpenAutoClose = true;
                }
            }

            if (args.Length >= 11)
            {
                mWaitTime = (float)args[10];
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
            Logger.LogWarningFormat("新手引导报错---点击[Introduction]类型控件，找不到{0}界面,处于Exception状态", mFrameType);
            return GuideState.Exception;
        }

        var SourceRect = ibind.GetComponent<RectTransform>(mComRoot);
        if (SourceRect == null)
        {
            Logger.LogErrorFormat("SourceRect is nil with path {0}", mComRoot);
            return GuideState.Exception;
        }

        var gButton = AddIntroductionTips(SourceRect.gameObject);
        if (mTextTips.Length > 0)
        {
            AddTextTips(gButton, mAnchor, mTextTips, mTextTipType, mLocalPos);
        }

        return GuideState.Normal;
    }

    protected override void _update()
    {
        if(bOpenAutoClose)
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

    }
    #endregion
}
