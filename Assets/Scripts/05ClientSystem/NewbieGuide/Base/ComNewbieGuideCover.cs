using UnityEngine;
using GameClient;

public class ComNewbieGuideCover : ComNewbieGuideBase
{
    #region vitrual
    public override void StartInit(params object[] args)
    {
        // 参数列表
        // 0.触发Cover界面关闭的事件EUIEventID的id

        base.StartInit(args);

        if (args != null)
        {
            if (args.Length >= 1)
            {
                mFrameType = args[0] as string;
            }
        }
    }

    protected override GuideState _init()
    {
        if (!AddCover())
        {
            return GuideState.Exception;
        }

        return GuideState.Normal;
    }
    #endregion
}
