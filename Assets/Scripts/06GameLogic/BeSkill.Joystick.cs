using UnityEngine;

//技能表现类 本地表现相关代码放进这个类
public partial class BeSkill
{
    public VInt2 effectOffset = VInt2.zero;//技能摇杆特效初始位置偏移
    public bool canRemoveJoystick = true;      //技能摇杆不要移除
    public SkillJoystickMode joystickMode = SkillJoystickMode.NONE;     //摇杆移动模式
    private bool _notDisplayLineEffect = false;
    public BeSkill.InnerState innerState = BeSkill.InnerState.NONE;     //瞄准特效当前状态
    public VInt3 effectPos;     //瞄准特效的实时位置

    Vector3 effectSpeed;        //瞄准特效移动速度
    private int _effectRadius;        //瞄准特效移动范围半径
    GeEffectEx targetEffectWarning;     //目标位置预警特效
    GeEffectEx targetEffectLine;        //瞄准镜的线
    GeEffectEx targetEffectGround;      //瞄准镜的底

    int recordVx = 0;       //技能摇杆相对于中心点位置的X方向偏移
    int recordVy = 0;       //技能摇杆相对于中心点位置的Y方向偏移

    public void JoyStickPostInit()
    {
        var skillData = owner.GetSkillActionInfo(skillID);

        if (skillData == null)
            return;

        if (skillData.skillJoystickConfig.mode != joystickMode)
        {
            joystickMode = skillData.skillJoystickConfig.mode;
        }
        
        _notDisplayLineEffect = skillData.skillJoystickConfig.notDisplayLineEffect;

        if (skillData.useSpecialOperation)
        {
            if (skillData.skillJoystickConfig.mode != SkillJoystickMode.FORWARDBACK)
            {
                joystickMode = SkillJoystickMode.SPECIAL;
            }
        }

        if (skillData.useSelectSeatJoystick)
        {
            joystickMode = SkillJoystickMode.SELECTSEAT;
        }

        canRemoveJoystick = !skillData.skillJoystickConfig.dontRemoveJoystick;
    }

    public void JoyStickStart()
    {
        if (InputManager.instance != null && !canRemoveJoystick)
        {
            InputManager.instance.LoadButtonJoystick(0, owner, skillID);
        }

        effectOffset = VInt2.zero;
    }

    public VInt3 GetEffectPos()
    {
        return effectPos;
    }

    public void UpdateJoystick(int vx, int vy)
    {
        recordVx = vx;
        recordVy = vy;
    }

    public void SetJoysticEffectOffset(VInt2 offset)
    {
        effectOffset = offset; 
        if (InputManager.instance == null) return;
        InputManager.instance.SetJoystickEffectOffset(offset);
    }

    //初始化瞄准特效
    private void LoadJoystickUI()
    {
        if (joystickMode != SkillJoystickMode.FREE)
            return;
        innerState = BeSkill.InnerState.CHOOSE_TARGET;
        var skillData = owner.GetSkillActionInfo(skillID);
        if (skillData == null)
            return;
        effectPos = owner.GetPosition();
        effectSpeed = skillData.skillJoystickConfig.effectMoveSpeed;
        _effectRadius = skillData.skillJoystickConfig.effectMoveRadius;

        recordVx = 0;
        recordVy = 0;
        if (targetEffectWarning != null)
        {
            targetEffectWarning.Remove();
        }
        targetEffectWarning = null;
        effectPos.x += effectOffset.x;
        effectPos.y += effectOffset.y;
#if !LOGIC_SERVER
        //只有自己能看到这个特效
        if (Utility.IsStringValid(skillData.skillJoystickConfig.effectName) && owner.isLocalActor)
        {
            targetEffectWarning = owner.CurrentBeScene.currentGeScene.CreateEffect(skillData.skillJoystickConfig.effectName, 10000, effectPos.vec3, 1, 1, true);
            if(!_notDisplayLineEffect)
                targetEffectLine = owner.CurrentBeScene.currentGeScene.CreateEffect(1018, effectPos.vec3);
            targetEffectGround = owner.CurrentBeScene.currentGeScene.CreateEffect(1019, owner.GetPosition().vec3);
            targetEffectGround.SetScale(_effectRadius / 6f);
        }
#endif
    }

    public void EndJoystick()
    {
        if (joystickMode != SkillJoystickMode.FREE)
            return;

        SetInnerState(BeSkill.InnerState.NONE);

        if (!owner.isLocalActor)
            return;
        if (InputManager.instance != null)
            InputManager.instance.RemoveButtonJoystick2(skillID);

        owner.delayCaller.DelayCall(500, () =>
        {
            RemoveJoystickEffect();
        });

        RemoveGroundAndLine();
    }

    public void RemoveJoystickEffect()
    {
        if (joystickMode != SkillJoystickMode.FREE)
            return;
        if (targetEffectWarning != null)
        {
            targetEffectWarning.Remove();
            targetEffectWarning = null;
        }

        RemoveGroundAndLine();
    }

    private void RemoveGroundAndLine()
    {
        if (targetEffectGround != null)
        {
            targetEffectGround.Remove();
            targetEffectGround = null;
        }

        if (targetEffectLine != null)
        {
            targetEffectLine.Remove();
            targetEffectLine = null;
        }
    }

    public void SetInnerState(BeSkill.InnerState sta)
    {
        innerState = sta;
        if (innerState == BeSkill.InnerState.LAUNCH)
            EndJoystick();
    }


    public void UpdateEffectMove(int iDeltime)
    {
        if ((joystickMode == SkillJoystickMode.FREE) && innerState == BeSkill.InnerState.CHOOSE_TARGET && recordVx != 0 && recordVy != 0)
        {
            VInt xmin = (VInt)owner.CurrentBeScene.sceneData.GetLogicXSize().x;
            VInt xmax = (VInt)owner.CurrentBeScene.sceneData.GetLogicXSize().y;
            VInt ymin = (VInt)owner.CurrentBeScene.sceneData.GetLogicZSize().x;
            VInt ymax = (VInt)owner.CurrentBeScene.sceneData.GetLogicZSize().y;

            //!!这里可能有问题
            var curPos = owner.GetPosition();
            VFactor radius = new VFactor(_effectRadius * IntMath.kIntDen, 1000);
            effectPos.x = curPos.x + (recordVx * radius);
            effectPos.y = curPos.y + (recordVy * radius);


            //effectPos.x = Mathf.Clamp(effectPos.x, owner.GetPosition().x - VInt.Float2VIntValue(effectRange.x), owner.GetPosition().x + VInt.Float2VIntValue(effectRange.x));
            //effectPos.y = Mathf.Clamp(effectPos.y, owner.GetPosition().y - VInt.Float2VIntValue(effectRange.y), owner.GetPosition().y + VInt.Float2VIntValue(effectRange.y));

            effectPos.x = Mathf.Clamp(effectPos.x, xmin.i, xmax.i);
            effectPos.y = Mathf.Clamp(effectPos.y, ymin.i, ymax.i);

            Vector3 graphicPos = effectPos.vector3;

            if (targetEffectWarning != null)
                targetEffectWarning.SetPosition(graphicPos);

            if (targetEffectLine != null)
                targetEffectLine.SetPosition(graphicPos);
        }
    }
}