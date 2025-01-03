using UnityEngine;
using GameClient;

public class ComNewbieGuideIntroduction2 : ComNewbieGuideBase
{
    #region vitrual

    public bool bOpenAutoClose = false;
    public float mWaitTime = 10.0f;

    public override void StartInit(params object[] args)
    {
        // 参数列表
        // 0.引导文字
        // 1.保存点
        // 2.需要高亮显示的节点及其子节点
        // 3.是否开启自动关闭(默认关闭)
        // 4.等待时间(可以不填，默认10秒)

        bOpenAutoClose = false;
        mWaitTime = 10.0f;

        base.StartInit(args);

        if (args != null)
        {
            if (args.Length >= 1)
            {
                mTextTips = args[0] as string;
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
                if (args[2] as string != "")
                {
                    mHighLightPointPath = args[2] as string;
                }
            }

            if (args.Length >= 4)
            {
                if ((eNewbieGuideAgrsName)args[3] == eNewbieGuideAgrsName.AutoClose)
                {
                    bOpenAutoClose = true;
                }
            }

            if (args.Length >= 5)
            {
                mWaitTime = (float)args[4];
            }
        }
    }

    protected override GuideState _init()
    {
        AddIntroductionTips2();

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
