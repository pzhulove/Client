using GameClient;
using UnityEngine.UI;
using UnityEngine;

public class ComNewbieGuidePassThrough : ComNewbieGuideBase
{
    public string mShowBindObjName = "";
    public bool mOpenAutoClose = false;
    public float mWaitTime = 0.0f;

    #region vitrual
    public override void StartInit(params object[] args)
    {
        // 参数列表(可以只填参数0,1)
        // 0.绑定界面的cs文件的名称
        // 1.绑定点击穿透的控件路径
        // 2.是否开启自动关闭
        // 3.等待时间(一定要是浮点型数据,不然会报错,e.g : 4f)
        // 4.绑定显示的obj路径
        // 5.引导文字
        // 6.显示位置
        // 7.文字模板类型
        // 8.保存点


        mWaitTime = 0.0f;

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
                if ((eNewbieGuideAgrsName)args[2] == eNewbieGuideAgrsName.AutoClose)
                {
                    mOpenAutoClose = true;
                }
            }

            if (args.Length >= 4)
            {
                mWaitTime = (float)args[3];
            }

            if (args.Length >= 5)
            {
                mShowBindObjName = args[4] as string;
            }

            if (args.Length >= 6)
            {
                mTextTips = args[5] as string;
            }

            if (args.Length >= 7)
            {
                mAnchor = (eNewbieGuideAnchor)args[6];
            }

            if (args.Length >= 8)
            {
                mTextTipType = (TextTipType)args[7];
            }

            if (args.Length >= 9)
            {
                if ((eNewbieGuideAgrsName)args[8] == eNewbieGuideAgrsName.SaveBoot)
                {
                    mSendSaveBoot = true;
                }
            }
        }
    }

    protected override GuideState _init()
    {
        var frameManager = ClientSystemManager.instance as IClientFrameManager;
        if (!frameManager.IsFrameOpen(mFrameType))
        {
            Logger.LogWarningFormat("新手引导报错---点击[PassThrough]类型控件，找不到{0}界面，处于wait状态", mFrameType);
            return GuideState.Wait;
        }

        var ibind = frameManager.GetFrame(mFrameType) as IGameBind;
        if (ibind == null)
        {
            Logger.LogWarningFormat("新手引导报错---点击[PassThrough]类型控件，找不到{0}界面，处于Exception状态", mFrameType);
            return GuideState.Exception;
        }

        var ClickRect = ibind.GetComponent<Button>(mComRoot);
        if (ClickRect == null)
        {
            Logger.LogErrorFormat("ClickRect is nil with path : {0}", mComRoot);
            return GuideState.Exception;
        }

        GameObject ShowSourceRect = null;
        if (mShowBindObjName != "")
        {
            ShowSourceRect = ibind.GetComponent<RectTransform>(mShowBindObjName).gameObject;
        }

        var gShowAreaButton = AddPassThroughTips(ClickRect.gameObject, ShowSourceRect, () =>
        {
            ClickRect.onClick.Invoke();
        });

        if (mTextTips.Length > 0)
        {
            AddTextTips(gShowAreaButton, mAnchor, mTextTips, mTextTipType, mLocalPos);
        }

        return GuideState.Normal;
    }

    protected override void _update()
    {
        if(mOpenAutoClose)
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
