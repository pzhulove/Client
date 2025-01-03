using System;
using System.Collections.Generic;

//技能摇杆表现相关
public partial class InputManager
{
#if !LOGIC_SERVER
    /// <summary>
    /// 摇杆样式
    /// </summary>
    enum JoystickStyle
    {
        Normal,                 // 普通
        PlayerSelect,           // 选人  
        ModeSelect,             // 选模式
        ActionSelect,           // 选技能
    }
    
    private Dictionary<int, ISkillJoystick> m_JoystickDic = new Dictionary<int, ISkillJoystick>();
    private ISkillJoystick m_CurJoystick ;
    public void LoadButtonJoystick(int slotPressed, BeActor actor, int skillID, bool isUp = false,
        SkillJoystickMode mode = SkillJoystickMode.NONE)
    {
        if (isUp)
            return;
        if (!actor.isLocalActor)
            return; 
        
        EnableSkillButton(false);
        m_CurJoystick = LoadJoystick(actor, skillID, mode);
    }
    
    private ISkillJoystick LoadJoystick(BeActor actor, int skillId, SkillJoystickMode mode)
    {
        ISkillJoystick skillJoystick = null;
        var style = GetJoystickStyle(mode);
        if(!m_JoystickDic.TryGetValue((int) style, out skillJoystick))
        {
            string name = style + "SkillJoystick";
            Type type = TypeTable.GetType(name);
            if (type != null)
                skillJoystick = (ISkillJoystick) Activator.CreateInstance(type);
            else
                skillJoystick = new NormalSkillJoystick();
            
            if (skillJoystick != null)
            {
                m_JoystickDic.Add((int) style, skillJoystick);
                skillJoystick.Create(mode);
            }
        }

        if (skillJoystick != null)
        {
            skillJoystick.Start(actor, actor.GetSkill(skillId));
        }
        return skillJoystick;
    }

    private JoystickStyle GetJoystickStyle(SkillJoystickMode mode)
    {
        JoystickStyle ret = JoystickStyle.Normal;
        switch (mode)
        {
            case SkillJoystickMode.NONE:
            case SkillJoystickMode.FREE:
            case SkillJoystickMode.SPECIAL:
            case SkillJoystickMode.DIRECTION:
            case SkillJoystickMode.FORWARDBACK:
                ret = JoystickStyle.Normal;
                break;
            case SkillJoystickMode.SELECTSEAT:
                ret = JoystickStyle.PlayerSelect;
                break;
            case SkillJoystickMode.MODESELECT:
                ret = JoystickStyle.ModeSelect;
                break;
            case SkillJoystickMode.ACTIONSELECT:
                ret = JoystickStyle.ActionSelect;
                break;
        }

        return ret;
    }
    
    public void SetJoystickEffectOffset(VInt2 offset)
    {
        if (m_CurJoystick == null) return;
            m_CurJoystick.SetDefaultOffset(offset);
    }
    
    public void DisableSkillJoystick()
    {
        if (m_CurJoystick == null) 
            return;
        m_CurJoystick.End(false);
    }
    
    //根据ID移除技能摇杆2
    public void RemoveButtonJoystick2(int skillID)
    {
        if (m_CurJoystick == null) 
            return;
        m_CurJoystick.End();
    }
    
    //练习场重置联系时移除技能摇杆
    public void ResetSkillJoystick()
    {
        if (m_CurJoystick != null)
        {
            m_CurJoystick.End(false);
            m_CurJoystick.Clear();
            m_CurJoystick = null;    
        }
        m_JoystickDic.Clear();
    }

    private void _ClearSkillJoystick()
    {
        if (m_CurJoystick != null)
        {
            m_CurJoystick.Clear();
            m_CurJoystick = null;
        }
        m_JoystickDic.Clear();
    }
#else
    public void LoadButtonJoystick(int slotPressed, BeActor actor, int skillID, bool isUp = false, SkillJoystickMode mode = SkillJoystickMode.NONE){}
    public void SetJoystickEffectOffset(VInt2 offset){}
    public void RemoveButtonJoystick2(int skillID){}
    public void ResetSkillJoystick(){}
    private void _ClearSkillJoystick() {}
#endif
}