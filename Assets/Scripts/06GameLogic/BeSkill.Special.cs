using GameClient;
using UnityEngine;

//技能-特写逻辑类
public partial class BeSkill
{
    //按第二下立即切换到下一阶段
    protected string startEnterNextFlag;
    protected string endEnterNextFlag;
    private bool enterFlag = false;
    private IBeEventHandle enterNextHandle = null;

    //后跳取消帧标签
    protected string startJumpBackCnacelFlag;
    protected string endJumpBackCnacelFlag;

    private void SpecialSkillStart()
    {
        if (!owner.CurrentBeBattle.HasFlag(BattleFlagType.SkillSpecialBug))
        {
            if (string.IsNullOrEmpty(startEnterNextFlag) && string.IsNullOrEmpty(endEnterNextFlag) &&
                string.IsNullOrEmpty(startJumpBackCnacelFlag) && string.IsNullOrEmpty(endJumpBackCnacelFlag))
                return;
        }
        
        enterFlag = false;
        enterNextHandle = owner.RegisterEventNew(BeEventType.onSkillCurFrame, (args) =>
        //enterNextHandle = owner.RegisterEvent(BeEventType.onSkillCurFrame, (object[] args) =>
        {
            string flag = args.m_String;
            if (flag == startEnterNextFlag)
            {
                enterFlag = true;
                SwitchEnterNextState();
            }
            else if (flag == endEnterNextFlag)
            {
                enterFlag = false;
                ResetButtonEffect();
            }

            //可以按后跳取消
            if (flag == startJumpBackCnacelFlag)
            {
                canPressJumpBackCancel = true;
                ChangeJumpBackImage(false);
            }
            else if (flag == endJumpBackCnacelFlag)
            {
                ChangeJumpBackImage(true);
                canPressJumpBackCancel = false;
            }
        });
    }

    private void SpecialSkillPressAgain()
    {
        if (!skillState.IsRunning())
            return;
        if (enterFlag)
        {
            ResetButtonEffect();
            OnEnterNextPhase(curPhase);
            ((BeActorStateGraph)owner.GetStateGraph()).ExecuteNextPhaseSkill();
        }
    }

    private void SpecialSkillCancel()
    {
        if (owner.CurrentBeBattle.HasFlag(BattleFlagType.SkillSpecialBug))
        {
            if (startEnterNextFlag != null)
            {
                RemoveHandle();
                ResetButtonEffect();
            }
        }
        else
        {
            RemoveHandle();
            if (startEnterNextFlag != null)
            {
                ResetButtonEffect();
            }
            
            if (!string.IsNullOrEmpty(startJumpBackCnacelFlag))
            {
                ChangeJumpBackImage(true);
                canPressJumpBackCancel = false;
            }
        }
    }

    private void SpecialSkillFinish()
    {
        if (owner.CurrentBeBattle.HasFlag(BattleFlagType.SkillSpecialBug))
        {
            if (startEnterNextFlag != null)
            {
                RemoveHandle();
                ResetButtonEffect();
            }
        }
        else
        {
            RemoveHandle();
            if (startEnterNextFlag != null)
            {
                ResetButtonEffect();
            }
            
            if (!string.IsNullOrEmpty(startJumpBackCnacelFlag))
            {
                ChangeJumpBackImage(true);
                canPressJumpBackCancel = false;
            }
        }
    }

    protected void SwitchEnterNextState()
    {
        skillButtonState = SkillState.WAIT_FOR_NEXT_PRESS;
        ChangeButtonEffect();
    }

    private void RemoveHandle()
    {
        if (enterNextHandle != null)
        {
            enterNextHandle.Remove();
            enterNextHandle = null;
        }
    }
}