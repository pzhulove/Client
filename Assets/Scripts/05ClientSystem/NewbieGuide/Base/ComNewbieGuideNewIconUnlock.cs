using UnityEngine.UI;
using GameClient;
using UnityEngine;

public class ComNewbieGuideNewIconUnlock : ComNewbieGuideBase
{
    public float mWaitTime = 0.0f;
    public string LoadResFile = "";
    public string TargetRootPath = "";
    public string IconPath = "";
    public string IconName = "";

    #region vitrual
    public override void StartInit(params object[] args)
    {
        // 参数列表
        // 0.加载的prefab文件名称
        // 1.目标点挂点路径
        // 2.显示的icon路径
        // 3.名称
        // 4.等待时间
        // 5.保存点
        // 6.暂停单局

        base.StartInit(args);

        if (args != null)
        {
            if (args.Length >= 1)
            {
                LoadResFile = args[0] as string;
            }

            if(args.Length >= 2)
            {
                TargetRootPath = args[1] as string;
            }

            if(args.Length >= 3)
            {
                IconPath = args[2] as string;
            }

            if (args.Length >= 4)
            {
                IconName = args[3] as string;
            }

            if (args.Length >= 5)
            {
                mWaitTime = (float)args[4];
            }

            if (args.Length >= 6)
            {
                if ((eNewbieGuideAgrsName)args[5] == eNewbieGuideAgrsName.SaveBoot)
                {
                    mSendSaveBoot = true;
                }
            }

            if (args.Length >= 7)
            {
                if ((eNewbieGuideAgrsName)args[6] == eNewbieGuideAgrsName.PauseBattle)
                {
                    mTryPauseBattle = true;
                }
            }
        }
    }

    protected override GuideState _init()
    {
        if(!AddNewIconUnlock(LoadResFile, TargetRootPath, IconPath, IconName))
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
