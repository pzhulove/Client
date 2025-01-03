using GameClient;
using UnityEngine.UI;
using UnityEngine;

public class ComNewbieGuideShowImage : ComNewbieGuideBase
{
    public string mShowImagePath = "";

    #region vitrual
    public override void StartInit(params object[] args)
    {
        // 参数列表
        // 0.图片路径
        // 1.保存点
        // 2.暂停单局

        base.StartInit(args);

        mProtectTime = 2.2f;

        if (args != null)
        {
            if (args.Length >= 1)
            {
                mShowImagePath = args[0] as string;
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
        if(mShowImagePath.Length <= 0)
        {
            return GuideState.Exception;
        }

        if (!AddShowImage(mShowImagePath))
        {
            return GuideState.Exception;
        }

        return GuideState.Normal;
    }
    #endregion
}
