/*
using System.Collections.Generic;
using GameClient;

/// <summary>
/// 急速射击技能
/// 在指定帧时将警卫员瞬移到上下位置，类似僚机，释放指定技能
/// 与主角同面向，结束时警卫员自动战斗
///
/// 配置：A：帧事件
///       B：警卫员资源ID
///       C：警卫员释放指定技能ID
///       D：警卫员排队位置（基于主角,(X|Y)）
/// </summary>
public class Skill1409 : BeSkill
{
    private string m_StartMassFlag = "140900";        // 帧事件ID
    private int m_ArmsID = 64000;                     // 警卫员资源ID
    private int m_ArmSkillId = 1421;                  // 警卫员指定技能ID  
    
    private int m_LineUpOffsetY = 20000;               // 偏移位置Y
    private int m_LineUpOffsetX = 12500;               // 偏移位置X

    private List<BeActor> m_ArmsList = new List<BeActor>();
    public Skill1409(int sid, int skillLevel) : base(sid, skillLevel){ }

    public override void OnInit()
    {
        base.OnInit();
        if (skillData.ValueA.Count > 0)
        {
            m_StartMassFlag = TableManager.GetValueFromUnionCell(skillData.ValueA[0], level).ToString();
        }

        if (skillData.ValueB.Count > 0)
        {
            m_ArmsID = TableManager.GetValueFromUnionCell(skillData.ValueB[0], level);
        }

        
        if (skillData.ValueC.Count > 0)
        {
            m_ArmSkillId = TableManager.GetValueFromUnionCell(skillData.ValueC[0], level);
        }
        
        if (skillData.ValueD.Count == 2)
        {
            m_LineUpOffsetX = TableManager.GetValueFromUnionCell(skillData.ValueD[0], level);
            m_LineUpOffsetY = TableManager.GetValueFromUnionCell(skillData.ValueD[1], level);
        }
    }

    public override void OnStart()
    {
        base.OnStart();
        _RegisterHandle();
    }

    public override void OnFinish()
    {
        ResetArms();
    }
    
    public override void OnCancel()
    {
        ResetArms();
    }
    
    /// <summary>
    /// 重置警卫员
    /// </summary>
    private void ResetArms()
    {
        for (int i = 0; i < m_ArmsList.Count; i++)
        {
            var arm = m_ArmsList[i];
            if(arm.IsDeadOrRemoved())
                continue;
            SetArmAiActive(arm, true);
        }
        
        m_ArmsList.Clear();
    }

    private void _RegisterHandle()
    {
        handleA = owner.RegisterEventNew(BeEventType.onSkillCurFrame, OnStartLineUp);
        //handleA = owner.RegisterEvent(BeEventType.onSkillCurFrame, OnStartLineUp);
    }

    
    /// <summary>
    /// 开始将警卫员集结
    /// </summary>
    /// <param name="args"></param>
    private void OnStartLineUp(BeEvent.BeEventParam param)
    {
        string flag = param.m_String;
        
        if(!flag.Equals(m_StartMassFlag))
            return;

        LineUpArms();
    }

    /// <summary>
    /// 集结警卫员
    /// </summary>
    private void LineUpArms()
    {
        m_ArmsList.Clear();
        GetArms(m_ArmsList);
        if(m_ArmsList.Count <= 0)
            return;

        SetLineUpPosition(m_ArmsList);
        for (int i = 0; i < m_ArmsList.Count; i++)
        {
            var arm = m_ArmsList[i];
            SetArmAiActive(arm, false);
            
            if(arm.CanUseSkill(m_ArmSkillId))
                arm.UseSkill(m_ArmSkillId);
            
            arm.SetFace(owner.GetFace());
        }
    }


    
    /// <summary>
    /// 获得自己召唤的警卫员,排除受控单位
    /// </summary>
    /// <param name="result"></param>
    private void GetArms(List<BeActor> result)
    {
        if(owner.CurrentBeScene == null)
            return;
        
        if(result == null)
            return;
        
        result.Clear();
        
        owner.CurrentBeScene.FindSummonMonster(result, owner);
        result.RemoveAll((item) => item.GetTopOwner(item) != owner || item.m_iResID != m_ArmsID);
        CancelArmSkill();
        m_ArmsList.RemoveAll((item) => item.IsInPassiveState());
    }

    
    /// <summary>
    /// 取消正在释放技能
    /// </summary>
    private void CancelArmSkill()
    {
        for (int i = 0; i < m_ArmsList.Count; i++)
        {
            var arm = m_ArmsList[i];
            if (arm.IsCastingSkill())
            {
                arm.CancelSkill(arm.GetCurSkillID());
                arm.sgSwitchStates(new BeStateData((int)ActionState.AS_IDLE));    
            }
        }
    }

    /// <summary>
    /// 警卫员AI开关
    /// </summary>
    /// <param name="arm"></param>
    /// <param name="active"></param>
    private void SetArmAiActive(BeActor arm, bool active)
    {
        if(arm == null)
            return;

        if (arm.aiManager != null)
        {
            if (active)
            {
                arm.aiManager.Start();
            }
            else
            {
                arm.aiManager.Stop();
            }
        }
    }

    /// <summary>
    /// 计算士兵排列落点（最多2个士兵）
    /// 算法：正常情况，2个士兵排在主角上下2个非阻挡点
    /// 当2个士兵时。查找最接近的板边位置
    /// 当1个士兵时。放在2个点中非阻挡点，优先上面
    /// </summary>
    /// <returns></returns>
    private void SetLineUpPosition(List<BeActor> arms)
    {
        if(arms.Count <= 0 || owner.CurrentBeScene == null)
            return;
        
        if (arms.Count == 1)
        {
            VInt3 pos = GetPosition(true);
            if (owner.CurrentBeScene.IsInBlockPlayer(pos))
            {
                pos = GetPosition(false);
                if (owner.CurrentBeScene.IsInBlockPlayer(pos))
                {
                    arms[0].SetPosition(GetNearestPosition(pos));
                }
                else
                {
                    arms[0].SetPosition(pos);
                }
            }
            else
            {
                arms[0].SetPosition(pos);
            }
        }
        else if (arms.Count == 2)
        {
            for (int i = 0; i < arms.Count; i++)
            {
                VInt3 pos = GetPosition(i == 0);
                if (owner.CurrentBeScene.IsInBlockPlayer(pos))
                {
                    arms[i].SetPosition(GetNearestPosition(pos));
                }
                else
                {
                    arms[i].SetPosition(pos);
                }       
            }
        }
    }

    /// <summary>
    /// 获得排队目标点
    /// </summary>
    /// <param name="isUp"></param>
    /// <returns></returns>
    private VInt3 GetPosition(bool isUp)
    {
        return owner.GetPosition() + new VInt3(owner.GetFace() ? m_LineUpOffsetX : -m_LineUpOffsetX, isUp ? m_LineUpOffsetY : -m_LineUpOffsetY, 0);
    }

    /// <summary>
    /// 获得最近的非板边点，如果找不到取主角位置
    /// </summary>
    /// <param name="centerPos"></param>
    /// <returns></returns>
    private VInt3 GetNearestPosition(VInt3 centerPos)
    {
        if (owner.CurrentBeScene == null)
            return owner.GetPosition();
        
        var pos = BeAIManager.FindStandPositionNew(centerPos, owner.CurrentBeScene);
        //if (owner.CurrentBeScene.IsInBlockPlayer(pos)) 规避换算误差
        if(pos == centerPos)
        {
            return owner.GetPosition();
        }

        return pos;
    }
    
    
}
*/

