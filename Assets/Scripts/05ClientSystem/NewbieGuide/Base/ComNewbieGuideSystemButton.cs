using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using GameClient;
using System;

[LoggerModel("NewbieGuide")]
public class ComNewbieGuideSystemButton : ComNewbieGuideButton
{
    #region vitrual
    protected override GuideState _init()
    {
        var ibind = ClientSystemManager.instance.CurrentSystem as IGameBind;

        if (ibind == null)
        {
            return GuideState.Exception;
        }

        var button = ibind.GetComponent<Button>(mComRoot);
        if (button == null)
        {
            Logger.LogErrorFormat("button is nil with path", mComRoot);
            return GuideState.Exception;
        }

        var gButton = AddButtonTips(button.gameObject, () =>
        {
            button.onClick.Invoke();
        });

        if (mTextTips.Length > 0)
        {
            AddTextTips(gButton, mAnchor, mTextTips);
        }

        return GuideState.Normal;
    }
    #endregion
}
