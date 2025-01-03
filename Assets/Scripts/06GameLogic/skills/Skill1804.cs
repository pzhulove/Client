using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameClient;

/*
 阵鬼 噬灵鬼斩
 */

public class Skill1804 : BeSkill
{
    protected IBeEventHandle m_SkillCastHandle = null;
    protected IBeEventHandle m_LastCastHandle = null;
    protected int m_ChargePhaseOne = 180421;                    //蓄力完成第一阶段技能ID
    protected int m_ChargePhaseTwo = 180422;                    //蓄力完成第二阶段技能ID
    protected int m_KazanSkillId = 1521;                        //刀魂之卡赞
    protected int m_SayaSkillId = 1806;                         //冰霜之萨亚
    protected int m_PulimengSkillId = 1805;                     //侵蚀之普戾蒙
    protected int m_LastSkillId = 0;                            //上一次使用的放阵技能ID

    protected int m_ReplaceIdKazan = 180422;                    //卡赞
    protected int m_ReplaceIdSaya = 1804222;                    //萨亚
    protected int m_ReplaceIdPuliemeng = 1804221;                //普戾蒙

    private IBeEventHandle _curFrameHandle = null;
    private string _startFlag = "180401";    //开始标签
    private string _endFlag = "180402";  //结束标签

    private int[] _newPhaseArray = new int[5] { 1804, 180412, 18041, 180422, 180423 };

    public Skill1804(int sid, int skillLevel) : base(sid, skillLevel) { }

    public override void OnInit()
    {
        
    }

    public override void OnPostInit()
    {
        if (m_LastCastHandle != null)
        {
            m_LastCastHandle.Remove();
        }
        m_LastCastHandle = owner.RegisterEventNew(BeEventType.onPreSetSkillAction, (param) =>
        {
            // int[] skillList = (int[])args[0];
            // int skillId = skillList[0];
            int skillId = param.m_Int;
            if (skillId == m_KazanSkillId || skillId == m_SayaSkillId || skillId == m_PulimengSkillId)
            {
                m_LastSkillId = skillId;
            }
        });
    }

    public override void OnStart()
    {
        m_SkillCastHandle = owner.RegisterEventNew(BeEventType.onPreSetSkillAction, (param) => 
        {
            // int[] skillList = (int[])args[0];
            // int skillId = skillList[0];
            int skillId = param.m_Int;
            if (skillId == m_ChargePhaseOne)
            {
                owner.skillController.SetCurrentSkillPhases(_newPhaseArray); 
            }

            if (skillId == m_ChargePhaseTwo && m_LastSkillId != 0)
            {
                if (m_LastSkillId == m_KazanSkillId)
                {
                    param.m_Int = m_ReplaceIdKazan;
                }
                else if (m_LastSkillId == m_SayaSkillId)
                {
                    param.m_Int = m_ReplaceIdSaya;
                }
                else if (m_LastSkillId == m_PulimengSkillId)
                {
                    param.m_Int = m_ReplaceIdPuliemeng;
                }
            }
        });

        _curFrameHandle = owner.RegisterEventNew(BeEventType.onSkillCurFrame, _OnSkillCurFrame);
    }

    public override void OnClickAgain()
    {
        base.OnClickAgain();
        _ResetButtonState();
        owner.SetCurrSkillPhase(new int[5] { 1804, 180412, 18041, 180422, 180423 });
        //直接切换到下一个阶段
        ((BeActorStateGraph)owner.GetStateGraph()).ExecuteNextPhaseSkill();
    }

    private void _OnSkillCurFrame(BeEvent.BeEventParam args)
    {
        // string flag = args[0] as string;
        string flag = args.m_String;
        if(flag == _startFlag)
        {
            pressMode = SkillPressMode.PRESS_MANY_TWO;
            skillButtonState = SkillState.WAIT_FOR_NEXT_PRESS;
            ChangeButtonEffect();
        }
        else if(flag == _endFlag)
        {
            _ResetButtonState();
        }
    }

    public override void OnCancel()
    {
        base.OnCancel();
        _ClearData();
    }

    public override void OnFinish()
    {
        base.OnFinish();
        _ClearData();
    }

    private void _ClearData()
    {
        if (m_SkillCastHandle != null)
        {
            m_SkillCastHandle.Remove();
            m_SkillCastHandle = null;
        }
            
        if (_curFrameHandle != null)
        {
            _curFrameHandle.Remove();
            _curFrameHandle = null;
        }
            
        _ResetButtonState();
    }

    private void _ResetButtonState()
    {
        pressMode = SkillPressMode.NORMAL;
        ResetButtonEffect();
    }
}
