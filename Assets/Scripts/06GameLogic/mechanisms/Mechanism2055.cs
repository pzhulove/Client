using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
/// <summary>
/// 团本机制19-玩家失明
/// </summary>
public class Mechanism2055 : BeMechanism
{
    private int[] monsterIDs = new int[] { 81120011, 81110011 };
    private DelayCallUnitHandle mSummonDelayCallHandle;
    public Mechanism2055(int mid, int lv) : base(mid, lv) { }

    public override void OnInit()
    {
        base.OnInit();
        if (data.ValueA.Count == 2)
        {
            monsterIDs[0] = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
            monsterIDs[1] = TableManager.GetValueFromUnionCell(data.ValueA[1], level);
        }
    }

    public override void OnReset()
    {
        mSummonDelayCallHandle.SetRemove(true);
        monsterIDs = new int[] { 81120011, 81110011 };
    }

    public override void OnStart()
    {
        base.OnStart();
        SetActorVisible(false);
        SetSkillBtn(false);

        RegistSummon();

        ClearDelayHandle();
        mSummonDelayCallHandle = owner.delayCaller.DelayCall(100, () =>
         {
             SummonMonster();
         });
    }

    private void RegistSummon()
    {
        handleB = owner.CurrentBeScene.RegisterEventNew(BeEventSceneType.onSummon, (args) =>
        {
            BeActor monster = (BeActor) args.m_Obj;
            BeActor _owner = (BeActor) args.m_Obj2;
            if (Array.IndexOf(monsterIDs, monster.GetEntityData().monsterID) != -1)
            {
#if !SERVER_LOGIC
                //如果召唤出来的怪物的召唤者不是我就隐藏
                if (_owner != null && _owner.GetPID() != owner.GetPID())
                {
                    if (owner.isLocalActor)
                        monster.m_pkGeActor.HideActor(true);
                }
#endif
                //暗的幻影强制跟踪召唤者
                if (monster != null && monster.GetEntityData().MonsterIDEqual(monsterIDs[1]))
                {
                    if (monster.aiManager != null)
                    {
                        if (_owner != null)
                            monster.aiManager.ForceAssignAiTarget(_owner);
                    }
                }
            }
        });
    }

    private void SummonMonster()
    {      
        //召唤明亮幻影和暗幻影
        for (int i = 0; i < monsterIDs.Length; i++)
        {
            VInt3 randomPos = owner.CurrentBeScene.GetRandomPos();
            if (owner.CurrentBeScene.IsInBlockPlayer(randomPos))
                randomPos = owner.GetPosition();
            owner.CurrentBeScene.SummonMonster(monsterIDs[i], randomPos, 1, owner);
        }     
    }

    public override void OnUpdate(int deltaTime)
    {
        base.OnUpdate(deltaTime);
        if (owner.GetCurrentBtnState() == GameClient.ButtonState.PRESS)
        {
            owner.SetAttackButtonState(GameClient.ButtonState.RELEASE);
        }
    }

    private void SetSkillBtn(bool flag)
    {
        owner.SetAttackButtonState(GameClient.ButtonState.RELEASE);
        if (!owner.CurrentBeBattle.FunctionIsOpen(GameClient.BattleFlagType.Mechanism2055CancelSkill))
        {
            if (flag)
            {
                owner.stateController.SetAbilityEnable(BeAbilityType.CAN_DO_SKILL_CMD, true);
            }
            else
            {
                owner.stateController.SetAbilityEnable(BeAbilityType.CAN_DO_SKILL_CMD, false);
                if (owner.IsCastingSkill()) //录像bug 在由正常切到吃小人场景时 如果人物正在蓄力操作并隐藏按钮则直接卡在技能蓄力阶段
                {
                    var curSkill = owner.GetCurrentSkill();
                    if (curSkill != null && BeUtility.IsActorUseCanChargeSkill(owner))//如果蓄力则取消技能
                    {
                        owner.Locomote(new BeStateData((int)ActionState.AS_IDLE), true);
                    }
                }
            }
        }
#if !SERVER_LOGIC
        if (owner.isLocalActor)
        {
            InputManager.instance.SetVisible(true, flag);

        }
#endif
    }

    private void SetActorVisible(bool flag)
    {
#if !SERVER_LOGIC
        if (owner.isLocalActor)
        {
            List<BeActor> list = GamePool.ListPool<BeActor>.Get();
            owner.CurrentBeScene.GetFilterTarget(list, new BeCampFilter(owner.GetCamp()),false);
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] != null && list[i].m_pkGeActor != null)
                {
                    if (list[i].GetPID() == owner.GetPID()) continue;
                    list[i].m_pkGeActor.HideActor(!flag);
                }
            }
            GamePool.ListPool<BeActor>.Release(list);
        }
#endif
    }

    void ClearDelayHandle()
    {
        mSummonDelayCallHandle.SetRemove(true);
    }

    public override void OnFinish()
    {
        base.OnFinish();
        SetActorVisible(true);
        SetSkillBtn(true);
        ClearDelayHandle();
    }
}
