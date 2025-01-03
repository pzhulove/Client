using UnityEngine.UI;
using GameClient;

public class ComNewbieGuideToggle : ComNewbieGuideButton
{
    // 参数列表同ComNewbieGuideButton

    #region vitrual
    protected override GuideState _init()
    {
        var frameManager = ClientSystemManager.instance as IClientFrameManager;
        if (!frameManager.IsFrameOpen(mFrameType))
        {
            return GuideState.Wait;
        }

        var ibind = frameManager.GetFrame(mFrameType) as IGameBind;
        if (ibind == null)
        {
            return GuideState.Exception;
        }

        var button = ibind.GetComponent<Toggle>(mComRoot);
        if (button == null)
        {
            if (mGuideControl != null)
            {
                Logger.LogErrorFormat("Toggle is nil with path [{0}], GuideTaskID = {1}, currentIndex = {2}", mComRoot, mGuideControl.GuideTaskID, mGuideControl.currentIndex);
            }
            else
            {
                Logger.LogErrorFormat("Toggle is nil with path [{0}]", mComRoot);
            }

            return GuideState.Exception;
        }

        var gButton = AddButtonTips(button.gameObject, () =>
        {
            button.isOn = true;
            button.onValueChanged.Invoke(true);
        });

        if (mTextTips.Length > 0)
        {
            AddTextTips(gButton, mAnchor, mTextTips, mTextTipType, mLocalPos);
        }

        return GuideState.Normal;
    }
    #endregion
}
