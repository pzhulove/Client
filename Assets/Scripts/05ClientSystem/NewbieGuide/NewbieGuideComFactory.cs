using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;
using GameClient;

[LoggerModel("NewbieGuide")]
public class NewbieGuideComFactory
{
    public static ComNewbieGuideBase _addComponent<T>(GameObject go, params object[] args) where T : ComNewbieGuideBase
    {
        var comp = go.AddComponent<T>() as T;
        if (comp != null)
        {
            comp.StartInit(args);
        }
        else
        {
            Logger.LogError("comp is nil");
        }

        return comp;
    }

    public static ComNewbieGuideBase _addComponent(GameObject go,Type type, params object[] args)
    {
        if(go == null)
        {
            Logger.LogError("NewbieGuideComFactory go is null!");
        }

        if( type == null 
        /*|| typeof(ComNewbieGuideBase).IsAssignableFrom(type) == false
        &*/
         )
        {
             Logger.LogError("NewbieGuideComFactory Type must Drive From ComNewbieGuideBase!");
        }

        var comp = go.AddComponent(type) as ComNewbieGuideBase;
        if (comp != null)
        {
            comp.StartInit(args);
        }
        else
        {
            Logger.LogError("comp is nil");
        }

        return comp;
    }

	public static ComNewbieGuideBase AddNewbieCom(GameObject go, ComNewbieData data)
    {
        if (go == null)
        {
            return null;
        }

		switch(data.ComType)
		{
            case NewbieGuideComType.USER_DEFINE:
                {
                    return _addComponent(go, data.ComNewbieGuideType, data.args);
                }
            case NewbieGuideComType.BUTTON:
                {
                    return _addComponent<ComNewbieGuideButton>(go, data.args);
                }
            case NewbieGuideComType.ETC_BUTTON:
                {
                    return _addComponent<ComNewbieGuideETCButton>(go, data.args);
                }
            case NewbieGuideComType.ETC_JOYSTICK:
                {
                    return _addComponent<ComNewbieGuideETCJoystick>(go, data.args);
                }
            case NewbieGuideComType.MOVE_2_POS:
                {
                    return _addComponent<ComNewbieGuideMove2Position>(go, data.args);
                }
            case NewbieGuideComType.PAUSE_BATTLE:
                {
                    return _addComponent<ComNewbieGuidePauseBattle>(go, data.args);
                }
            case NewbieGuideComType.RESUME_BATTLE:
                {
                    return _addComponent<ComNewbieGuideResumeBattle>(go, data.args);
                }
            case NewbieGuideComType.SYSTEM_BUTTON:
                {
                    return _addComponent<ComNewbieGuideSystemButton>(go, data.args);
                }
            case NewbieGuideComType.TALK_DIALOG:
                {
                    return _addComponent<ComNewbieGuideTalkDialog>(go, data.args);
                }
            case NewbieGuideComType.TOGGLE:
                {
                    return _addComponent<ComNewbieGuideToggle>(go, data.args);
                }
            case NewbieGuideComType.WAIT:
                {
                    return _addComponent<ComNewbieGuideWait>(go, data.args);
                }
            case NewbieGuideComType.INTRODUCTION:
                {
                    return _addComponent<ComNewbieGuideIntroduction>(go, data.args);
                }
            case NewbieGuideComType.INTRODUCTION2:
                {
                    return _addComponent<ComNewbieGuideIntroduction2>(go, data.args);
                }
            case NewbieGuideComType.COVER:
                {
                    return _addComponent<ComNewbieGuideCover>(go, data.args);
                }
            case NewbieGuideComType.PASS_THROUGH:
                {
                    return _addComponent<ComNewbieGuidePassThrough>(go, data.args);
                }
            case NewbieGuideComType.SHOW_IMAGE:
                {
                    return _addComponent<ComNewbieGuideShowImage>(go, data.args);
                }
            case NewbieGuideComType.STRESS:
                {
                    return _addComponent<ComNewbieGuideStress>(go, data.args);
                }
            case NewbieGuideComType.OPEN_EYES:
                {
                    return _addComponent<ComNewbieGuideOpenEyes>(go, data.args);
                }
            case NewbieGuideComType.NEWICON_UNLOCK:
                {
                    return _addComponent<ComNewbieGuideNewIconUnlock>(go, data.args);
                }
            case NewbieGuideComType.BATTLEDRUGDRAG:
                {
                    return _addComponent<ComNewbieGuideBattleDrugDrag>(go, data.args);
                }
            case NewbieGuideComType.PLAY_EFFECT:
                {
                    return _addComponent<ComNewbieGuidePlayEffect>(go, data.args);
                }
        }

		return null;
    }
}
