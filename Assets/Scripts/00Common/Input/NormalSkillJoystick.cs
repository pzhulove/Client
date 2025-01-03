#if !LOGIC_SERVER
using GameClient;

public class NormalSkillJoystick : SkillJoystick
{
    public override string PrefabPath => "UIFlatten/Prefabs/ETCInput/HGJoystick";

    private short recordVx, recordVy;       //摇杆偏移中心点的X轴或Y轴位置
    private short recordJoystickDegree;     //摇杆偏移角度

    public override void OnStart()
    {
        base.OnStart();
        recordVx = recordVy = 0;
    }

    public override void OnMove(int vx, int vy, int degree)
    {
        base.OnMove(vx, vy, degree);
        if (m_Skill != null)
        {
            if(m_Skill.joystickMode == SkillJoystickMode.FREE)
            {
                if (m_Skill.innerState == BeSkill.InnerState.CHOOSE_TARGET)
                {
                    if (recordVx != vx || recordVy != vy)
                    {
                        recordVx = (short)vx;
                        recordVy = (short)vy;

                        //InputManager.instance.CreateSkillFrameCommand(m_Skill.skillID, (vx + GlobalLogic.VALUE_1000) * GlobalLogic.VALUE_10000 + vy + GlobalLogic.VALUE_1000);
                        InputManager.instance.CreateSkillFrameCommand(m_Skill.skillID, new SkillFrameCommand.SkillFrameData
                        {
                            type = SkillFrameCommand.SkillFrameDataType.Joystick_Position, 
                            data1 = (uint) (vx + GlobalLogic.VALUE_1000), 
                            data2 = (uint) (vy + GlobalLogic.VALUE_1000)
                        });;
                    }
                }
            }  
            
            if (recordJoystickDegree != degree)
            {
                recordJoystickDegree = (short)degree;
                //InputManager.instance.CreateSkillFrameCommand(m_Skill.skillID, 100000000 + degree);
                InputManager.instance.CreateSkillFrameCommand(m_Skill.skillID, new SkillFrameCommand.SkillFrameData
                {
                    type = SkillFrameCommand.SkillFrameDataType.Joystick_Degree, 
                    data1 = (uint) degree
                });
            }
        }
    }

    public override void OnUpdate(int v)
    {
        base.OnUpdate(v);
        if (m_Skill != null)
        {
            if (m_JoystickCtrl != null && m_JoystickCtrl.canRemoveJoystick != m_Skill.canRemoveJoystick)
            {
                m_JoystickCtrl.canRemoveJoystick = m_Skill.canRemoveJoystick;
            }

            if(m_Skill.joystickMode == SkillJoystickMode.FREE)
            {
                if (m_Skill.innerState == BeSkill.InnerState.LAUNCH)
                {
                    RemoveButtonJoystick(m_Skill.skillID);
                    ReleaseCaptureButton(m_Skill.button);
                }
            }
        }
    }

    public override void OnRelease(int degree, bool hasDrag, float duration)
    {
        base.OnRelease(degree, hasDrag, duration);
        SpecialSkillRelease(m_Skill, duration, hasDrag, degree);
    }
    
    //特殊技能释放流程
    private void SpecialSkillRelease(BeSkill skill, float duration, bool hasDrag, int degree)
    {
        if (skill == null)
            return;

        //用于技能特殊操作 例如金刚碎
        if (skill.joystickMode == SkillJoystickMode.SPECIAL)
        {
            int specialChoice = 0;

            if (duration >= 0.05f && hasDrag)
            {
                int tmp = degree;
                if (tmp >= 45 && tmp < 135)
                    specialChoice = 1;
                else if (tmp >= 135 && tmp < 225)
                    specialChoice = 0;
                else if (tmp >= 225 && tmp < 315)
                    specialChoice = 2;
                else
                    specialChoice = 0;
            }

            //InputManager.instance.CreateSkillFrameCommand(m_Skill.skillID, specialChoice + 2);
            InputManager.instance.CreateSkillFrameCommand(m_Skill.skillID, new SkillFrameCommand.SkillFrameData{type = SkillFrameCommand.SkillFrameDataType.Joystick_SpecialChoice, data1 = (uint) specialChoice});
        }
        else if (skill.joystickMode == SkillJoystickMode.FORWARDBACK)
        {
            int choice = 0;
            if (duration >= 0.05f && hasDrag)
            {
                int tmp = degree;
                if (tmp >= 90 && tmp < 270)
                    choice = 1;
                else
                    choice = 0;
            }

            if (!m_Owner.GetFace())
            {
                if (choice == 1)
                {
                    choice = 0;
                }
                else if (choice == 0)
                {
                    choice = 1;
                }
            }

            //InputManager.instance.CreateSkillFrameCommand(m_Skill.skillID, choice + 10);
            InputManager.instance.CreateSkillFrameCommand(m_Skill.skillID, new SkillFrameCommand.SkillFrameData{type = SkillFrameCommand.SkillFrameDataType.Joystick_ForwardBackChoice, data1 = (uint) choice});
        }
    }
}
#endif