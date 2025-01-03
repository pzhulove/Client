using UnityEngine;
using GameClient;

public class ComNewbieGuideOpenEyes : ComNewbieGuideBase
{
    public float mWaitTime = 3.30f;

    #region vitrual
    public override void StartInit(params object[] args)
    {
        // 参数列表(可以只填参数0,不需要绑定某个界面)
        // 0.等待时间(一定要是浮点型数据,不然会报错,e.g : 4f)
        // 1.保存点
        // 2.暂停单局

        mWaitTime = 4.0f;

        base.StartInit(args);

        if (args != null)
        {
            if (args.Length >= 1)
            {
                mWaitTime = (float)args[0];
            }

            if (args.Length >= 2)
            {
                if ((eNewbieGuideAgrsName)args[1] == eNewbieGuideAgrsName.SaveBoot)
                {
                    mSendSaveBoot = true;
                }
            }

            if (args.Length >= 3)
            {
                if ((eNewbieGuideAgrsName)args[2] == eNewbieGuideAgrsName.PauseBattle)
                {
                    mTryPauseBattle = true;
                }
            }
        }
    }

    protected override GuideState _init()
    {
        if (!AddOpenEyes())
        {
            Logger.LogWarningFormat("新手引导报错---点击[OpenEyes]类型控件，找不到{0}界面", mFrameType);
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
            if(mGuideControl != null)
            {
                mGuideControl.ControlComplete();
            }
        }
    }
    #endregion
}
