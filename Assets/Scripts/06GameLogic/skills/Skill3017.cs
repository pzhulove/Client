using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameClient;

//鹰踏
public class Skill3017 : BeSkill
{
    int maxCount;                   //最大攻击次数

    int originalSkillId = 3017;     //鹰踏技能
    int replaceSkillId = 3018;      //鹰踏起跳
    int shortCD = 200;              //添加一个短暂的CD，减少踏空操作
    int upSpeed = 80000;            //弹起速度

    IBeEventHandle handle0;
    IBeEventHandle handle1;
    IBeEventHandle handle2;

    bool canUseFlag = true;         //是否能使用
    int count = 0;
    
    public Skill3017(int sid, int skillLevel) : base(sid, skillLevel) { }

    public override void OnPostInit()
    {
        maxCount = TableManager.GetValueFromUnionCell(skillData.ValueA[0], level);
        count = maxCount;

        //添加监听，在地面的话，把鹰踏技能替换成鹰踏起跳
        if (handle0 == null)
        {
            handle0 = owner.RegisterEventNew(BeEventType.onReplaceSkill, args =>
            {
                if (owner.sgGetCurrentState() == (int)ActionState.AS_JUMP)
                    return;
                if (owner.HasTag((int)AState.ACS_JUMP))
                    return;

                int skillId = args.m_Int;
                if (skillId == originalSkillId)
                {
                    BeSkill replaceSkill = owner.GetSkill(replaceSkillId);
                    if (replaceSkill != null && replaceSkill.CanUseSkill())
                    {
                        args.m_Int = replaceSkillId;
                    }
                }
            });
        }
    }

    public override void OnStart()
    {
        canUseFlag = false;

        //添加监听，鹰踏击中敌人就进短CD，并弹起
        handle1 = owner.RegisterEventNew(BeEventType.onHitOther, args =>
        //handle1 = owner.RegisterEvent(BeEventType.onHitOther, args =>
        {
            var entity = args.m_Obj as BeEntity;
            var skillId = args.m_Int2;
            if (skillId == 3017)
            {
                owner.SetMoveSpeedZ(upSpeed);
                owner.sgSwitchStates(new BeStateData((int)ActionState.AS_JUMP));
                owner.delayCaller.DelayCall(shortCD, () =>
                {
                    canUseFlag = true;
                });
            }
        });

        //添加监听，落地后进入技能CD
        handle2 = owner.RegisterEventNew(BeEventType.onTouchGround, args =>
        {
            StartCD();
        });

        count--;
        if (count > 0)
        {
            SetIgnoreCD(true);
        }
        else
        {
            SetIgnoreCD(false);

            //替换技能阶段
            var phases = new int[3] { 3017, 30173, 30174 };
            owner.skillController.SetCurrentSkillPhases(phases);

            //最后一阶段不能移动
            walk = false;
        }
    }

    public void StartCD()
    {
        if (!isCooldown)
        {
            SetIgnoreCD(false);
            StartCoolDown();
            count = maxCount;
            canUseFlag = true;
        }
    }

    public override bool CanUseSkill()
    {
        if (canUseFlag && base.CanUseSkill())
        {
            //起跳状态下需要设定一个最低释放高度
            if (owner.HasTag((int)AState.ACS_JUMP) || owner.sgGetCurrentState() == (int)ActionState.AS_JUMP)
            {
                var flag = owner.GetPosition().z > GlobalLogic.VALUE_10000;
#if !LOGIC_SERVER
                if (button != null)
                {
                    if (flag)
                        button.AddEffect(ETCButton.eEffectType.onContinue);
                    else
                        button.RemoveEffect(ETCButton.eEffectType.onContinue);
                }
#endif
                return flag;
            }
            return true;
        }
        return false;
    }

    public override void OnCancel()
    {
        Release();
    }

    public override void OnFinish()
    {
        Release();
    }

    void Release()
    {
        if (count <= 0)
        {
            StartCoolDown();
            count = maxCount;
            canUseFlag = true;
        }

        walk = true;

        RemoveButtonEffect();

        RemoveHandle();
    }

    public void RemoveButtonEffect()
    {
#if !LOGIC_SERVER
        if (button != null)
            button.RemoveEffect(ETCButton.eEffectType.onContinue);
#endif
    }

    void RemoveHandle()
    {
        if (handle1 != null)
        {
            handle1.Remove();
            handle1 = null;
        }
        if (handle2 != null)
        {
            handle2.Remove();
            handle2 = null;
        }
    }
}
