#if !LOGIC_SERVER
using UnityEngine;
using static GameClient.SkillFrameCommand;

public interface ISkillJoystick
{
    void Create(SkillJoystickMode mode);
    void Start(BeActor actor, BeSkill skill);
    void End(bool sendCmd = true); 
    void SetDefaultOffset(VInt2 offset);
    void Clear();
}

public abstract class SkillJoystick : ISkillJoystick
{
    private float startTime = 0;        //开始时间
    protected BeSkill m_Skill;
    protected BeActor m_Owner;
    protected SkillJoystickMode m_Mode;
    protected GameObject m_JoystickGo;
    protected HGJoyStick m_JoystickCtrl;
    public abstract string PrefabPath { get; }

    public void Create(SkillJoystickMode mode)
    {
        if (m_JoystickGo != null)
        {
            Object.Destroy(m_JoystickGo);
            m_JoystickGo = null;
        }

        m_JoystickGo = AssetLoader.instance.LoadRes(PrefabPath).obj as GameObject;
        if (m_JoystickGo != null)
        {
            m_JoystickCtrl = m_JoystickGo.GetComponent<HGJoyStick>();
            m_JoystickGo.SetActive(false);
        }
        OnCreate();
    }
    
    public void Start(BeActor actor, BeSkill skill)
    {
        if (actor == null || skill == null)
            return;
        
        SetDefaultOffset(VInt2.zero);
        startTime = Time.realtimeSinceStartup; 
        m_Owner = actor;
        m_Skill = skill;
        if (m_JoystickCtrl != null)
        {
            m_JoystickCtrl.canRemoveJoystick = true;
            m_JoystickCtrl.onMove = OnMove;
            m_JoystickCtrl.onUpdate = OnUpdate;
            m_JoystickCtrl.onRelease = _OnRelease;
        }

        if (m_JoystickGo != null && m_Skill != null)
        {
            m_JoystickGo.SetActive(true);
            Utility.AttachTo(m_JoystickGo, m_Skill.button.transform.parent.gameObject);
            m_JoystickGo.transform.position = m_Skill.button.transform.position;
        }
        OnStart();
    }

    public void End(bool sendCmd)
    {
        if(m_JoystickGo != null && !m_JoystickGo.activeSelf)
            return;
        if (sendCmd)
        {
            InputManager.instance.CreateSkillFrameCommand(m_Skill.skillID, new SkillFrameData{isUp = true});
        }
        DisableSkillJoystick();
    }

    public void SetDefaultOffset(VInt2 offset)
    {
        if (m_JoystickCtrl != null)
        {
            m_JoystickCtrl.DefaultOffset = offset;
        }
    }

    public void Clear()
    {
        m_Owner = null;
        m_Skill = null;
        if (m_JoystickCtrl != null)
        {
            m_JoystickCtrl.onMove = null;
            m_JoystickCtrl.onRelease = null;
            m_JoystickCtrl.onUpdate = null;
            m_JoystickCtrl = null;
        }

        if (m_JoystickGo != null)
        {
            Object.Destroy(m_JoystickGo);
            m_JoystickGo = null;
        }

        OnClear();
    }

    public void _OnRelease(int degree, bool hasDrag)
    {
        float duration = Time.realtimeSinceStartup - startTime;
        if (m_Skill != null)
        {
            if (m_Skill.canRemoveJoystick)
            {
                RemoveButtonJoystick(m_Skill.skillID);
                m_Skill.ReleaseJoystick();    
            }
            else
            {
                UpButtonJoystick(m_Skill.skillID);
            }
            ReleaseCaptureButton(m_Skill.button);
        }

        OnRelease(degree, hasDrag, duration);
    }
    
    protected void RemoveButtonJoystick(int skillID)
    {
        InputManager.instance.CreateSkillFrameCommand(skillID, new SkillFrameData{isUp = true});
        DisableSkillJoystick();
    }
    
    protected void UpButtonJoystick(int skillID)
    {
        InputManager.instance.CreateSkillFrameCommand(skillID, new SkillFrameData{type = SkillFrameDataType.Joystick_Release});
    }
    
    public void DisableSkillJoystick()
    {
        if(m_JoystickGo != null)
            m_JoystickGo.SetActive(false);
        if (m_JoystickCtrl != null)
        {
            m_JoystickCtrl.onMove = null;
            m_JoystickCtrl.onRelease = null;
            m_JoystickCtrl.onUpdate = null;
        }
        InputManager.instance.EnableSkillButton(true);
    }
    
    protected void ReleaseCaptureButton(ETCButton button)
    {
        var ctrl = button.GetComponent<ControlButtonAnimationInside>();
        if (ctrl != null)
        {
            ctrl.skillup();
        }
    }
    
    public virtual void OnCreate(){}
    public virtual void OnStart(){}
    public virtual void OnClear(){}
    public virtual void OnMove(int vx, int vy, int degree) {}
    public virtual void OnUpdate(int v){}
    public virtual void OnRelease(int degree, bool hasDrag, float duration) { }
}
#endif