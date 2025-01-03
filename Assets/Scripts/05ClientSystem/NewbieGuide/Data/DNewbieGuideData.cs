using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameClient;

public interface IUnitData
{
    NewbieGuideComType getType();
    System.Type objType();
    object[] getArgs();
}

// 参数列表
// 0.绑定界面的cs文件的名称
// 1.绑定控件路径
// 2.引导文字
// 3.显示位置
// 4.文字模板类型
// 5.坐标微调
// 6.保存点
// 7.暂停单局
// 8.需要高亮显示的节点及其子节点

[System.Serializable]
public class GuideButton : IUnitData
{
    public NewbieGuideComType getType()
    {
        return NewbieGuideComType.BUTTON;
    }

    public System.Type objType()
    {
        return typeof(GuideButton);
    }
    public string mFrameType;
    public string mComRoot;
    public string mTextTips;
    public ComNewbieGuideBase.eNewbieGuideAnchor mAnchor;
    public TextTipType mTextTipType;
    public Vector3 mLocalPos;
    public eNewbieGuideAgrsName mSaveBoot;
    public eNewbieGuideAgrsName mPauseBattle;
    public string mHighLightPointPath;

    public object[] getArgs()
    {
        return new object[]
        {
            mFrameType,
            mComRoot,
            mTextTips,
            mAnchor,
            mTextTipType,
            mLocalPos,
            mSaveBoot,
            mPauseBattle,
            mHighLightPointPath
        };
    }
}

// 参数列表
// 0.触发Cover界面关闭的事件EUIEventID的id

[System.Serializable]
public class GuideCover : IUnitData
{
    public NewbieGuideComType getType()
    {
        return NewbieGuideComType.COVER;
    }
    public System.Type objType()
    {
        return typeof(GuideCover);
    }
    public string mFrameType;
    public object[] getArgs()
    {
        return new object[]
        {
            mFrameType
        };
    }
}

[System.Serializable]
public class GuideETCButton : IUnitData
{
    public NewbieGuideComType getType()
    {
        return NewbieGuideComType.ETC_BUTTON;
    }
    public System.Type objType()
    {
        return typeof(GuideETCButton);
    }
    public string mButtonName;
    public ComNewbieGuideBase.eNewbieGuideAnchor mAnchor;
    public string mContent;
    public object[] getArgs()
    {
        return new object[]
        {
            mButtonName,
            mAnchor,
            mContent
        };
    }
}

[System.Serializable]
public class GuideETCJoystick : IUnitData
{
    public NewbieGuideComType getType()
    {
        return NewbieGuideComType.ETC_JOYSTICK;
    }
    public System.Type objType()
    {
        return typeof(GuideETCJoystick);
    }
    public float mPosX;
    public object[] getArgs()
    {
        return new object[]
        {
            mPosX
        };
    }
}

// 参数列表
// 0.绑定界面的cs文件的名称
// 1.绑定控件路径
// 2.引导文字
// 3.显示位置
// 4.文字模板类型
// 5.坐标微调
// 6.保存点
// 7.暂停单局
// 8.需要高亮显示的节点及其子节点
// 9.是否开启自动关闭(默认关闭)
// 10.等待时间(可以不填，默认10秒)
[System.Serializable]
public class GuideIntroduction : IUnitData
{
    public NewbieGuideComType getType()
    {
        return NewbieGuideComType.INTRODUCTION;
    }
    public System.Type objType()
    {
        return typeof(GuideIntroduction);
    }
    public string mFrameType;
    public string mComRoot;
    public string mTextTips;
    public ComNewbieGuideBase.eNewbieGuideAnchor mAnchor;
    public TextTipType mTextTipType;
    public Vector3 mLocalPos;
    public eNewbieGuideAgrsName mSaveBoot;
    public eNewbieGuideAgrsName mPauseBattle;
    public string mHighLightPointPath;

    public eNewbieGuideAgrsName mAutoClose; //false
    public float mWaitTime;  //10.0f

    public object[] getArgs()
    {
        return new object[]
        {
            mFrameType,
            mComRoot,
            mTextTips,
            mAnchor,
            mTextTipType,
            mLocalPos,
            mSaveBoot,
            mPauseBattle,
            mHighLightPointPath,
            mAutoClose,
            mWaitTime
        };
    }
}

// 参数列表
// 0.引导文字
// 1.保存点
// 2.需要高亮显示的节点及其子节点
// 3.是否开启自动关闭(默认关闭)
// 4.等待时间(可以不填，默认10秒)
[System.Serializable]
public class GuideIntroduction2 : IUnitData
{
    public NewbieGuideComType getType()
    {
        return NewbieGuideComType.INTRODUCTION2;
    }
    public System.Type objType()
    {
        return typeof(GuideIntroduction2);
    }
    public string mTextTips;
    public eNewbieGuideAgrsName mSaveBoot;
    public string mHighLightPointPath;

    public eNewbieGuideAgrsName mAutoClose; //false
    public float mWaitTime;  //10.0f

    public object[] getArgs()
    {
        return new object[]
        {
            mTextTips,
            mSaveBoot,
            mHighLightPointPath,
            mAutoClose,
            mWaitTime
        };
    }
}

// 0.加载的prefab文件名称
// 1.目标点的挂点路径
// 2.显示的icon路径
// 3.名称
// 4.等待时间
// 5.保存点
// 6.暂停单局
[System.Serializable]
public class GuideNewIconUnlock : IUnitData
{
    public NewbieGuideComType getType()
    {
        return NewbieGuideComType.NEWICON_UNLOCK;
    }
    public System.Type objType()
    {
        return typeof(GuideNewIconUnlock);
    }

    public string loadResFile;
    public string TargetObjPath;
    public string iconPath;
    public string iconName;
    public float waittime;

    public eNewbieGuideAgrsName mSaveBoot;

    public eNewbieGuideAgrsName mTryPauseBattle;

    public object[] getArgs()
    {
        return new object[]
        {
            loadResFile,
            TargetObjPath,
            iconPath,
            iconName,
            waittime,
            mSaveBoot,
            mTryPauseBattle
        };
    }
}

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
[System.Serializable]
public class GuidePassThrough : IUnitData
{
    public NewbieGuideComType getType()
    {
        return NewbieGuideComType.PASS_THROUGH;
    }
    public System.Type objType()
    {
        return typeof(GuidePassThrough);
    }
    public string mFrameType;
    public string mComRoot;

    public eNewbieGuideAgrsName mAutoClose;

    public float mWaitTime;
    public string mShowBindObjName;

    public string mTextTips;
    public ComNewbieGuideBase.eNewbieGuideAnchor mAnchor;

    public TextTipType mTextTipType;

    public eNewbieGuideAgrsName mSaveBoot;

    public object[] getArgs()
    {
        return new object[]
        {
            mFrameType,
            mComRoot,
            mAutoClose,
            mWaitTime,
            mShowBindObjName ,
            mTextTips,
            mAnchor,
            mTextTipType,
            mSaveBoot
        };
    }
}

// 参数列表
// 0.绑定界面的cs文件的名称
// 1.绑定控件路径
// 2.等待时间
// 3.保存点
// 4.暂停单局
// 5.恢复单局
[System.Serializable]
public class GuideStress : IUnitData
{
    public NewbieGuideComType getType()
    {
        return NewbieGuideComType.STRESS;
    }
    public System.Type objType()
    {
        return typeof(GuideStress);
    }
    public string mFrameType;
    public string mComRoot;
    public float mWaitTime;

    public eNewbieGuideAgrsName mSaveBoot;
    public eNewbieGuideAgrsName mTryPauseBattle;

    public eNewbieGuideAgrsName mTryResumeBattle;
    public object[] getArgs()
    {
        return new object[]
        {
            mFrameType,
            mComRoot,
            mWaitTime,
            mSaveBoot,
            mTryPauseBattle,
            mTryResumeBattle
        };
    }
}

// 参数列表
// 0.对话id
// 1.保存点
// 2.暂停单局
[System.Serializable]
public class GuideTalkDialog : IUnitData
{
    public NewbieGuideComType getType()
    {
        return NewbieGuideComType.TALK_DIALOG;
    }
    public System.Type objType()
    {
        return typeof(GuideTalkDialog);
    }
    public int id = -1;
    public eNewbieGuideAgrsName mSaveBoot;
    public eNewbieGuideAgrsName mTryPauseBattle;
    public object[] getArgs()
    {
        return new object[]
        {
            id,
            mSaveBoot,
            mTryPauseBattle
        };
    }
}

// 参数列表(可以只填参数0,不需要绑定任何界面)
// 0.等待时间(一定要是浮点型数据,不然会报错,e.g : 4f)
// 1.是否穿透
// 2.保存点
// 3.暂停单局
// 4.恢复单局
[System.Serializable]
public class GuideWait : IUnitData
{
    public NewbieGuideComType getType()
    {
        return NewbieGuideComType.WAIT;
    }
    public System.Type objType()
    {
        return typeof(GuideWait);
    }

    public float mWaitTime;
    public bool mPathThorugh = false;
    public eNewbieGuideAgrsName mSaveBoot;
    public eNewbieGuideAgrsName mPauseBattle;
    public eNewbieGuideAgrsName mTryResumeBattle;

    public object[] getArgs()
    {
        return new object[]
        {
            mWaitTime,
            mPathThorugh,
            mSaveBoot,
            mPauseBattle,
            mTryResumeBattle
        };
    }
}

// 0.特效所在的预制体路径
// 1.保存点
// 2.等待时间
[System.Serializable]
public class GuidePlayEffect: IUnitData
{
    public NewbieGuideComType getType()
    {
        return NewbieGuideComType.PLAY_EFFECT;
    }
    public System.Type objType()
    {
        return typeof(GuidePlayEffect);
    }

    public string loadResFile;
    public eNewbieGuideAgrsName mSaveBoot;
    public float mWaitTime;

    public object[] getArgs()
    {
        return new object[]
        {
            loadResFile,
            mSaveBoot,
            mWaitTime
        };
    }
}

[System.Serializable]
public struct ModifyData
{
    public int iIndex; // 参数在ComNewbieData()的参数列表args里的索引
    public NewBieModifyDataType ModifyDataType;
}


[System.Serializable]
public struct NewbieDataUnitData
{
    public NewbieDataUnitData(string name = null)
    {
        stepName = name;
        type = NewbieGuideComType.NULL;
        modifyData = null;
        buttonGuide = null;
        coverGuide = null;
        etcButtonGuide = null;
        etcJoystickGuide = null;
        introductionGuide = null;
        introduction2Guide = null;
        newiconUnlockGuide = null;
        passThroughGuide = null;
        stressGuide = null;
        talkDialogGuide = null;
        waitGuide = null;
        playEffectGuide = null;
    }

    public string stepName;


    public NewbieGuideComType type;
    public ModifyData[] modifyData;

    void ClearData()
    {
        buttonGuide = null;
        coverGuide = null;
        etcButtonGuide = null;
        etcJoystickGuide = null;
        introductionGuide = null;
        introduction2Guide = null;
        newiconUnlockGuide = null;
        passThroughGuide = null;
        stressGuide = null;
        talkDialogGuide = null;
        waitGuide = null;
        playEffectGuide = null;
    }
    IUnitData SetType(NewbieGuideComType type)
    {
        ClearData();
        switch (type)
        {
            case NewbieGuideComType.TOGGLE:
            case NewbieGuideComType.BUTTON:
                {
                    buttonGuide = new GuideButton();
                    return buttonGuide;
                }
            case NewbieGuideComType.COVER:
                {
                    coverGuide = new GuideCover();
                    return coverGuide;
                }
            case NewbieGuideComType.ETC_BUTTON:
                {
                    etcButtonGuide = new GuideETCButton();
                    return etcButtonGuide;
                }
            case NewbieGuideComType.ETC_JOYSTICK:
                {
                    etcJoystickGuide = new GuideETCJoystick();
                    return etcJoystickGuide;
                }
            case NewbieGuideComType.INTRODUCTION:
                {
                    introductionGuide = new GuideIntroduction();
                    return introductionGuide;
                }
            case NewbieGuideComType.INTRODUCTION2:
                {
                    introduction2Guide = new GuideIntroduction2();
                    return introduction2Guide;
                }
            case NewbieGuideComType.NEWICON_UNLOCK:
                {
                    newiconUnlockGuide = new GuideNewIconUnlock();
                    return newiconUnlockGuide;
                }
            case NewbieGuideComType.PASS_THROUGH:
                {
                    passThroughGuide = new GuidePassThrough();
                    return passThroughGuide;
                }
            case NewbieGuideComType.STRESS:
                {
                    stressGuide = new GuideStress();
                    return stressGuide;
                }
            case NewbieGuideComType.TALK_DIALOG:
                {
                    talkDialogGuide = new GuideTalkDialog();
                    return talkDialogGuide;
                }
            case NewbieGuideComType.WAIT:
                {
                    waitGuide = new GuideWait();
                    return waitGuide;
                }
            case NewbieGuideComType.PLAY_EFFECT:
                {
                    playEffectGuide = new GuidePlayEffect();
                    return playEffectGuide;
                }
        }

        return null;
    }

    public IUnitData GetData()
    {
        switch (type)
        {
            case NewbieGuideComType.TOGGLE:
            case NewbieGuideComType.BUTTON:
                {
                    return buttonGuide;
                }
            case NewbieGuideComType.COVER:
                {
                    return coverGuide;
                }
            case NewbieGuideComType.ETC_BUTTON:
                {
                    return etcButtonGuide;
                }
            case NewbieGuideComType.ETC_JOYSTICK:
                {
                    return etcJoystickGuide;
                }
            case NewbieGuideComType.INTRODUCTION:
                {
                    return introductionGuide;
                }
            case NewbieGuideComType.INTRODUCTION2:
                {
                    return introduction2Guide;
                }
            case NewbieGuideComType.NEWICON_UNLOCK:
                {
                    return newiconUnlockGuide;
                }
            case NewbieGuideComType.PASS_THROUGH:
                {
                    return passThroughGuide;
                }
            case NewbieGuideComType.STRESS:
                {
                    return stressGuide;
                }
            case NewbieGuideComType.TALK_DIALOG:
                {
                    return talkDialogGuide;
                }
            case NewbieGuideComType.WAIT:
                {
                    return waitGuide;
                }
            case NewbieGuideComType.PLAY_EFFECT:
                {
                    return playEffectGuide;
                }
        }

        return null;
    }

    public void ChangeType(NewbieGuideComType type)
    {
        this.Type = type;
        SetType(type);
    }

    public GuideButton buttonGuide;
    public GuideCover coverGuide;
    public GuideETCButton etcButtonGuide;
    public GuideETCJoystick etcJoystickGuide;
    public GuideIntroduction introductionGuide;

    public GuideIntroduction2 introduction2Guide;

    public GuideNewIconUnlock newiconUnlockGuide;
    public GuidePassThrough passThroughGuide;
    public GuideStress stressGuide;

    public GuideTalkDialog talkDialogGuide;
    public GuideWait waitGuide;
    public GuidePlayEffect playEffectGuide;

    public NewbieGuideComType Type
    {
        get
        {
            return type;
        }
        set
        {
            type = value;
        }
    }
}

[System.Serializable]
public struct NewbieConditionData
{
    public eNewbieGuideCondition condition;
    public int[] LimitArgsList;
    public string[] LimitFramesList;
}

public class DNewbieGuideData : ScriptableObject
{
    public string GuideName;
    public NewbieDataUnitData[] UnitData = new NewbieDataUnitData[0];

    public NewbieConditionData[] ConditionData = new NewbieConditionData[0];

}