using System;
using System.Collections.Generic;
using GameClient;

public struct ModeSelectItemInfo
{
    public string IconPath;
    public string Name;
}

/// <summary>
/// 模式选择摇杆接口
/// </summary>
interface IModeSelectSkill
{
    /// <summary>
    ///  摇杆显示的相关信息
    /// </summary>
    /// <returns></returns>
    ModeSelectItemInfo[] GetModeItemInfos();
    
    /// <summary>
    /// 每个模式选择之间的角度间隔
    /// </summary>
    /// <returns></returns>
    int GetDegree();
    
    /// <summary>
    /// 当选择模式完成后的回调
    /// </summary>
    /// <param name="index"></param>
    void OnSelectMode(int index);

    /// <summary>
    /// 是否能在技能CD时呼出摇杆
    /// </summary>
    /// <returns></returns>
    bool CanJoystickOnCD();
}

public class Skill1400 : BeSkill, IModeSelectSkill, ISummonSkillUpdate
{
    public Skill1400(int sid, int skillLevel) : base(sid, skillLevel)
    {
    }

    private ModeSelectItemInfo[] m_ModeList = 
    {
        new ModeSelectItemInfo(){IconPath = String.Empty, Name = "步枪"}, 
        new ModeSelectItemInfo(){IconPath = String.Empty, Name = "霰弹枪"}, 
        new ModeSelectItemInfo(){IconPath = String.Empty, Name = "防御"}, 
    };

    public ModeSelectItemInfo[] GetModeItemInfos()
    {
        return m_ModeList;
    }

    public int GetDegree()
    {
        return 60;
    }

    public void OnSelectMode(int index)
    {
        if(index > (int) AttackMode.Count)
            return;
        
        SwitchEnemy((AttackMode) index + 1);
    }

    public bool CanJoystickOnCD()
    {
        return true;
    }


    public enum AttackMode
    {
        Pistol,    // 手枪 默认模式
        
        Rifle,   // 步枪
        Shotgun, // 霰弹枪
        Defense, // 防御
        
        Count
    }
    private AttackMode m_CurAttackMode = AttackMode.Pistol;
    public AttackMode CurAttackMode => m_CurAttackMode;

    private VInt3[] m_OffsetPos = new VInt3[(int) AttackMode.Count];
    private bool[] m_AnchorMask = new bool[2];
    private int m_Duration;
    private int m_StartTime;
    private bool m_IsLock = true;
    private List<Mechanism3004> m_EnemyList = new List<Mechanism3004>();
    public override void OnInit()
    {
        base.OnInit();
        m_OffsetPos[(int) AttackMode.Pistol] = new VInt3(TableManager.GetValueFromUnionCell(skillData.ValueA[0], level), TableManager.GetValueFromUnionCell(skillData.ValueA[1], level), 0);
        m_OffsetPos[(int) AttackMode.Rifle] = new VInt3(TableManager.GetValueFromUnionCell(skillData.ValueB[0], level), TableManager.GetValueFromUnionCell(skillData.ValueB[1], level), 0);
        m_OffsetPos[(int) AttackMode.Shotgun] = new VInt3(TableManager.GetValueFromUnionCell(skillData.ValueC[0], level), TableManager.GetValueFromUnionCell(skillData.ValueC[1], level), 0);
        m_OffsetPos[(int) AttackMode.Defense] = new VInt3(TableManager.GetValueFromUnionCell(skillData.ValueD[0], level), TableManager.GetValueFromUnionCell(skillData.ValueD[1], level), 0);
        
        m_Duration = TableManager.GetValueFromUnionCell(skillData.ValueE[0], level);
        joystickMode = SkillJoystickMode.NONE;
    }

    public VInt3 GetAnchorPosition(int id)
    {
        var pos = m_OffsetPos[(int) m_CurAttackMode];
        if (id == 1)
        {
            pos.y = -pos.y;
        }

        return pos;
    }

    public int GetAnchorId(Mechanism3004 enemy)
    {
        Mechanism1505.BindUpdate(enemy.owner, this);
        for (int i = 0; i < m_AnchorMask.Length; i++)
        {
            if (!m_AnchorMask[i])
            {
                m_EnemyList.Add(enemy);
                m_AnchorMask[i] = true;
                ModeUpdate();
                return i;
            }
        }

        Logger.LogError("Mechanism3004 out of enemy limit");
        return 0;
    }

    public void ReleaseAnchorId(int id, Mechanism3004 enemy)
    {
        if(id >= m_AnchorMask.Length)
            return;

        m_EnemyList.Remove(enemy);
        m_AnchorMask[id] = false;
        ModeUpdate();
    }

    private void SwitchEnemy(AttackMode mode)
    {
        if(!m_IsLock)
            return;
            
        if (owner.CurrentBeScene != null) 
            m_StartTime = owner.CurrentBeScene.GameTime;
        
        m_CurAttackMode = mode;
        for (int i = 0; i < m_EnemyList.Count; i++)
        {
            var enemy = m_EnemyList[i];
            enemy.UpdateAttackMode();
        }
    }

    private void ModeUpdate()
    {
        if (m_CurAttackMode != AttackMode.Pistol)
        {
            joystickMode = SkillJoystickMode.NONE;
            SetLightButtonVisible(false);
        }
        else
        {
            if (m_EnemyList.Count > 0)
            {
                joystickMode = SkillJoystickMode.MODESELECT;
                SetLightButtonVisible(true);
            }
            else
            {
                joystickMode = SkillJoystickMode.NONE;
                SetLightButtonVisible(false);
            }    
        }
    }

    public void SetLockEnemy(bool isLock)
    {
        m_IsLock = isLock;
        for (int i = 0; i < m_EnemyList.Count; i++)
        {
            m_EnemyList[i].SetLockEnemy(isLock);
        }
    }
    
    public void OnSummonUpdate(int deltaTime)
    {
        if (m_CurAttackMode == AttackMode.Pistol)
            return;

        if (m_Duration <= 0)
            return;
        
        if (owner.CurrentBeScene != null)
        {
            if (owner.CurrentBeScene.GameTime - m_StartTime >= m_Duration)
            {
                SwitchEnemy(AttackMode.Pistol);
            }
        }
    }

    public void OnSummonUpdateFinish()
    {
    }
}
