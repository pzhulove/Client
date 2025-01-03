using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using GameClient;
using System;

public class ComNewbieGuideETCButton : ComNewbieGuideBase
{
    public string mButtonName;
    public string mContent = "";

    #region vitrual
    public override void StartInit(params object[] args)
    {
        base.StartInit(args);

        mButtonName = "";
        mContent = "";

        if (args != null)
        {
            if (args.Length >= 2)
            {
                mButtonName = args[0] as string;
                mAnchor = (eNewbieGuideAnchor)args[1];
            }

            if (args.Length >= 3)
            {
                mContent = args[2] as string;
            }
        }
    }

    protected override GuideState _init()
    {
        var frameManager = ClientSystemManager.instance as IClientFrameManager;

        var etcbutton = Utility.FindGameObject(ClientSystemManager.instance.MiddleLayer, InputManager.GetEtcSkillRoot());
        if (etcbutton == null)
        {
            return GuideState.Exception;
        }

        var button = Utility.FindGameObject(etcbutton, mButtonName);
        if(button == null)
        {
            return GuideState.Exception;
        }

        var etcButtonCom = button.GetComponent<ETCButton>();
        if(etcButtonCom == null)
        {
            return GuideState.Exception;
        }

        if (!etcButtonCom.isActiveAndEnabled)
        {
            return GuideState.Exception;
        }

        var goButton = AddTouchedTips(button, () =>
        {
            etcButtonCom.onDown.Invoke();
        },
            () =>
        {
            etcButtonCom.onUp.Invoke();
        });

        if (mContent.Length > 0)
        {
            AddTextTips(goButton, mAnchor, mContent);
        }

        return GuideState.Normal;
    }
    #endregion
}
