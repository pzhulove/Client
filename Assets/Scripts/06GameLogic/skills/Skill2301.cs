using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using GameClient;
using ProtoTable;

/// <summary>
/// 圆舞棍
/// </summary>
public class Skill2301 : BeSkill
{

    protected enum DIR
    {
        LEFT = 0,
        RIGHT = 1,
        COUNT = 2,
    }

    protected string strIndicator = "UIFlatten/Prefabs/Battle_Digit/Indicator_ForwardBack";
    
    protected GameObject objIndicator = null;
    protected GameObject[] objs = new GameObject[2];
    protected Text[] texts = new Text[2];
    protected IBeEventHandle mChangeFaceHandle = null;
    protected string[] strValue = new string[2];

    public Skill2301(int sid, int skillLevel) : base(sid, skillLevel) { }

    public override void OnPostInit()
    {
#if !LOGIC_SERVER
        if (inTown)
            return;

        if (owner == null || owner.m_pkGeActor == null)
            return;

        if (!owner.isLocalActor)
            return;

        if (canSlide)
        {
            ComCommonBind bind = owner.m_pkGeActor.GetForwardBackArrowBind(strIndicator);
            if (bind == null)
                return;
            objIndicator = bind.gameObject;
            for (int i = 0; i < (int)DIR.COUNT; ++i)
            {
                objs[i] = bind.GetGameObject(string.Format("obj{0}", i));
                texts[i] = bind.GetCom<Text>(string.Format("txt{0}", i));
            }
            ChangeDir();                       //默认情况先设置一下
            if (owner != null)
            {
                RemoveHandle();
                mChangeFaceHandle = owner.RegisterEventNew(BeEventType.onChangeFace, (args) =>
                {
                    ChangeDir();
                });
            }
            //strValue = new string[] {"前","后"};
            strValue = new string[] {"",""};
            SetAllArrowActive(false);
        }
        else
        {
            joystickMode = SkillJoystickMode.NONE;                  //设置技能摇杆不显示
        }
#endif
    }
    
    public override void OnInit()
    {
#if !LOGIC_SERVER
        canSlide = SettingManager.GetInstance().GetSlideMode("2301") == InputManager.SlideSetting.SLIDE;
#endif
    }

    public override void OnStart()
    {
#if !LOGIC_SERVER
        SetAllArrowActive(false);
#endif
    }

    public override void OnCancel()
    {
        SetAllArrowActive(false);
    }

    public override void OnFinish()
    {
        SetAllArrowActive(false);
    }

    public override void OnReleaseJoystick()
    {
        //Logger.LogErrorFormat("OnReleaseJoystick");
        SetAllArrowActive(false);
    }

    public override void OnUpdateJoystick(int degree)
    {
#if !LOGIC_SERVER
        if (!owner.isLocalActor)
            return;
        if (!canSlide)
            return;

        var dir = InputManager.GetForwardBack(degree);
        if (dir == InputManager.PressDir.LEFT)
            ShowArrow(DIR.LEFT);
        else 
            ShowArrow(DIR.RIGHT);
#endif
    }

    protected void ShowArrow(DIR dir)
    {
#if !LOGIC_SERVER
        if (!CanUseSkill())
            return;
        if (dir >= DIR.COUNT)
            return;

        for (int i = 0; i < (int)DIR.COUNT; ++i)
        {
            if(i >= objs.Length || i >= texts.Length)
                break;
            
            GameObject arrow = objs[i];
            if (arrow == null)
                continue;

            if ((int)dir == i)
            {
                texts[i].text = strValue[i];
                objs[i].CustomActive(true);
            }
            else
            {
                objs[i].CustomActive(false);
            }
        }
#endif
    }

    protected void SetAllArrowActive(bool active)
    {
#if !LOGIC_SERVER
        if (!owner.isLocalActor)
            return;
        
        for (int i = 0; i < (int)DIR.COUNT; ++i)
        {
            if(i >= objs.Length)
                break;
            
            GameObject arrow = objs[i];
            if (arrow == null)
                continue;

            arrow.CustomActive(active);
        }
#endif
    }

    protected void ChangeDir()
    {
#if !LOGIC_SERVER
        if (owner == null)
            return;
        bool faceLeft = owner.GetFace();
        int xScale = faceLeft ? -1 : 1;
        if (objIndicator != null)
        {
            var scale = objIndicator.transform.localScale;
            scale.x = Mathf.Abs(scale.x) * xScale;
            objIndicator.transform.localScale = scale;     
        }
#endif
    }


    protected void RemoveHandle()
    {
        if (mChangeFaceHandle != null)
        {
            mChangeFaceHandle.Remove();
            mChangeFaceHandle = null;
        }
    }
}
