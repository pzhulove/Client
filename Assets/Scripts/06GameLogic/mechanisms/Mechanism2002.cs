using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//召唤出来的怪物跟随召唤者
public class Mechanism2002 : BeMechanism
{
    //攻击目标优先级
    protected enum TargetPriority
    {
        None = 0,
        Normal,     //普通
        Elite,      //精英
        Boss,       //Boss
        Player,     //玩家PK模式用
    }

    public Mechanism2002(int mid, int lv) : base(mid, lv) { }
    
    protected VInt[] moveSpeed = new VInt[2]; //怪物移动速度X轴、Y轴
    protected VInt[] followRadius = new VInt[2] ;//怪物跟随范围X轴、Y轴
    protected VInt attackRadius;   //寻找攻击目标范围
    protected VInt attackXOffset;  //X轴攻击距离
    protected int skillId = 5031;   //释放的技能ID

    protected BeActor summoner = null;      //召唤者
    protected BeActor attackTarget = null;  //攻击目标
    protected bool skillInCD = false;       //技能是否处于CD中
    protected ActionState curActionState = ActionState.AS_NONE;
    protected DelayCallUnitHandle switchIdleHandle;

    public override void OnInit()
    {
        base.OnInit();
        moveSpeed[0] = VInt.NewVInt(TableManager.GetValueFromUnionCell(data.ValueA[0],level),GlobalLogic.VALUE_1000);
        moveSpeed[1] = VInt.NewVInt(TableManager.GetValueFromUnionCell(data.ValueA[1], level), GlobalLogic.VALUE_1000);
        followRadius[0] = VInt.NewVInt(TableManager.GetValueFromUnionCell(data.ValueB[0],level),GlobalLogic.VALUE_1000);
        followRadius[1] = VInt.NewVInt(TableManager.GetValueFromUnionCell(data.ValueB[1], level), GlobalLogic.VALUE_1000);
        attackRadius = VInt.NewVInt(TableManager.GetValueFromUnionCell(data.ValueC[0], level), GlobalLogic.VALUE_1000);
        attackXOffset = VInt.NewVInt(TableManager.GetValueFromUnionCell(data.ValueD[0], level), GlobalLogic.VALUE_1000);
        skillId = TableManager.GetValueFromUnionCell(data.ValueE[0], level);
    }

    public override void OnReset()
    {
        summoner = null;
        attackTarget = null;
        skillInCD = false;
        curActionState = ActionState.AS_NONE;
        switchIdleHandle.SetRemove(true);
    }

    public override void OnStart()
    {
        summoner = owner.GetOwner() as BeActor;
    }

    public override void OnUpdate(int deltaTime)
    {
        if (owner.CurrentBeScene == null)
            return;
        UpdateFindTarget();
        UpdateSkillCD();
        UpdateFollow();
        UpdateGoToTargetAndAttack();
    }

    //寻找目标
    protected void UpdateFindTarget()
    {
        if (attackTarget != null && attackTarget.IsDead())
            attackTarget = null;
        if (!CheckTargetInAttackRadius())
            attackTarget = null;
        if (attackTarget != null)
            return;
        List<BeActor> targetList = GamePool.ListPool<BeActor>.Get();
        owner.CurrentBeScene.FindTargets(targetList,owner, attackRadius);
        if(targetList!=null && targetList.Count > 0)
        {
            TargetPriority type = TargetPriority.Normal;
            for (int i = 0; i < targetList.Count; i++)
            {
                TargetPriority curType = GetPrioritty(targetList[i]);
                if (curType >= type)
                {
                    attackTarget = targetList[i];
                }
            }
        }
        GamePool.ListPool<BeActor>.Release(targetList);
    }

    //检查目标是否还在攻击视野内
    protected bool CheckTargetInAttackRadius()
    {
        if (attackTarget == null)
            return false;
        int dis = Mathf.Abs((attackTarget.GetPosition() - owner.GetPosition()).magnitude);
        return dis <= attackRadius;
    }

    //获取目标的优先级
    protected TargetPriority GetPrioritty(BeActor actor)
    {
        if (!actor.IsMonster())
            return TargetPriority.Player;
        if (actor.IsBoss())
            return TargetPriority.Boss;
        if (actor.GetEntityData().monsterData.Type == ProtoTable.UnitTable.eType.ELITE)
            return TargetPriority.Elite;
        return TargetPriority.Normal;
    }

    //检查技能是否处于CD中
    protected void UpdateSkillCD()
    {
        if (owner.GetSkill(skillId) == null)
            skillInCD = true;
        skillInCD = owner.GetSkill(skillId).isCooldown;
    }

    //走向攻击目标并且攻击
    protected void UpdateGoToTargetAndAttack()
    {
        if (attackTarget == null || skillInCD)
            return;
        if (owner.IsCastingSkill())
            return;
        if (CheckInAttackRadius() == 1)
        {
            SetIdle();
            SetFaceTarget(attackTarget.GetPosition());
            owner.UseSkill(skillId);
        }
        else if(CheckInAttackRadius() == 0)
        {
            GoToTarget(attackTarget.GetPosition(), attackXOffset, new VInt(0.1f));
        }
    }
    
    /// <summary>
    /// 检测目标是否处于怪物的攻击目标内
    /// </summary>
    /// <returns>1表示到达攻击位置 0表示还没有到达攻击位置</returns>
    protected int CheckInAttackRadius()
    {
        if (attackTarget == null)
            return -1;
        VInt3 targetPos = attackTarget.GetPosition();

        int sdisx = owner.GetPosition().x - targetPos.x;
        int sdisy = owner.GetPosition().y - targetPos.y;

        int disx = Mathf.Abs(sdisx);
        int disy = Mathf.Abs(sdisy);

        return disx <= attackXOffset && disy <= VInt.zeroDotOne?1:0;
    }

    //移动到目标位置
    protected void GoToTarget(VInt3 targetPos,VInt limitRadiusX, VInt limitRadiusY)
    {
        switchIdleHandle.SetRemove(true);
        int sdisx = owner.GetPosition().x - targetPos.x;
        int sdisy = owner.GetPosition().y - targetPos.y;

        VInt xSpeed = sdisx > 0 ? -moveSpeed[0] : moveSpeed[0];
        VInt ySpeed = sdisy > 0 ? -moveSpeed[1] : moveSpeed[1];

        owner.SetFace(sdisx > 0);

        xSpeed = Math.Abs(sdisx) > limitRadiusX ? xSpeed : 0;
        ySpeed = Math.Abs(sdisy) > limitRadiusY ? ySpeed : 0;

        owner.SetMoveSpeedX(xSpeed);
        owner.SetMoveSpeedY(ySpeed);

        PlayWalk();
    }

    //怪物切换到Idle动作
    protected void SetIdle()
    {
        owner.SetMoveSpeedX(0);
        owner.SetMoveSpeedY(0);
        switchIdleHandle = owner.delayCaller.DelayCall((int)GlobalLogic.VALUE_200, () =>
        {
            PlayIdle();
        });
    }

    //设置怪物朝向攻击目标
    protected void SetFaceTarget(VInt3 targetPos)
    {
        int sdisx = owner.GetPosition().x - targetPos.x;
        int sdisy = owner.GetPosition().y - targetPos.y;
        owner.SetFace(sdisx > 0);
    }


    #region Follow
    //更新跟随逻辑
    public void UpdateFollow()
    {
        if (attackTarget != null && !skillInCD)
            return;
        if (owner.IsCastingSkill())
            return;
        if (!IsInFollowRadius())
        {
            VInt3 summonPos = summoner.GetPosition();
            GoToTarget(summonPos,new VInt(0.1f), new VInt(0.1f));
        }
        else
        {
            SetIdle();
        }
    }

    //是否在跟随范围内
    protected bool IsInFollowRadius()
    {
        if (summoner == null)
            return true;
        VInt3 targetPos = summoner.GetPosition();

        int sdisx = owner.GetPosition().x - targetPos.x;
        int sdisy = owner.GetPosition().y - targetPos.y;

        int disx = Mathf.Abs(sdisx);
        int disy = Mathf.Abs(sdisy);

        return disx < followRadius[0] && disy < followRadius[1];
    }

    protected void PlayIdle()
    {
        if (owner.IsCastingSkill())
            return;
        if (curActionState == ActionState.AS_IDLE)
            return;
        curActionState = ActionState.AS_IDLE;
        string action = "Idle";
        owner.PlayAction(action);
    }

    protected void PlayWalk()
    {
        if (owner.IsCastingSkill())
            return;
        if (curActionState == ActionState.AS_WALK)
            return;
        curActionState = ActionState.AS_WALK;
        string action = "Walk";
        owner.PlayAction(action);
    }
    #endregion
}
