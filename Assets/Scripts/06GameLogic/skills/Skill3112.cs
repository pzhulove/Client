using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameClient;

/// <summary>
/// 念气师 保护罩技能 再次点击会引爆念气罩 同时给处于念气罩范围内目标添加一个给定时间的无敌Buff
/// </summary>
public class Skill3112 : BeSkill
{
    private int mBuffInfoID = 0;
    private int mMonsterID = 0;
    private int mAddBuffInfoID = 0;
    private bool mIsActive = false;

    protected List<IBeEventHandle> mHandleList = null;
    private BeActor mTargetMonster = null;
    
    public Skill3112(int sid, int skillLevel) : base(sid, skillLevel)
    {
    }

    public override void OnInit()
    {
        base.OnInit();
        mIsActive = false;
        if (!BattleMain.IsModePvP(battleType) && skillData.ValueA.Count > 0 && skillData.ValueB.Count > 0 && skillData.ValueC.Count > 0)
        {
            mIsActive = true;
            mMonsterID = TableManager.GetValueFromUnionCell(skillData.ValueA[0], level);
            mBuffInfoID = TableManager.GetValueFromUnionCell(skillData.ValueB[0], level);
            mAddBuffInfoID = TableManager.GetValueFromUnionCell(skillData.ValueC[0], level);
            
            mHandleList = new List<IBeEventHandle>();
        }
    }
    

    public override void OnStart()
    {
        base.OnStart();
        if (!mIsActive)
            return;

        pressMode = SkillPressMode.NORMAL;
        ResetButtonEffect();
        mTargetMonster = null;
        RemoveHandle();
        mHandleList.Add(owner.RegisterEventNew(BeEventType.onSummon, OnTargetMonsterSummon));
    }

    public override void OnFinish()
    {
        base.OnFinish();
        if (mTargetMonster == null || mTargetMonster.IsDead())
            return;
        
        // 修复快速点击技能按钮时，不能加上小罩子的问题
        // 小罩子的添加需要获取大罩子的范围，但是大罩子的buff会在100秒后才加上(范围buff检测间隔)
        owner.delayCaller.DelayCall(100, SwitchState);
    }

    private void OnTargetMonsterSummon(BeEvent.BeEventParam args)
    {
        var actor = args.m_Obj as BeActor;
        if (!actor.GetEntityData().MonsterIDEqual(mMonsterID))
            return;
        
        mTargetMonster = actor;
        mHandleList.Add(mTargetMonster.RegisterEventNew(BeEventType.onDead, OnMonsterDead));
    }

    private void OnMonsterDead(BeEvent.BeEventParam eventParam)
    {
        mTargetMonster = null;
        ResetButtonEffect();
    }
    
    public override void OnClickAgain()
    {
        base.OnClickAgain();

        if (!mIsActive)
            return;
        
        ResetButtonEffect();
        
        if (mBuffInfoID <= 0 || mMonsterID <= 0)
            return;
        
        if (mTargetMonster == null)
            return;
        
        AddBuff();
        mTargetMonster.DoDead();
    }

    private void AddBuff()
    {
        BuffInfoData buffInfoData = new BuffInfoData(mBuffInfoID);
        buffInfoData = mTargetMonster.buffController.GetTriggerBuff(buffInfoData);
        if (buffInfoData == null)
            return;
        
        var targets = GamePool.ListPool<BeActor>.Get();
        targets.Clear();
        BeUtility.GetAllFriendPlayers(owner, targets);
        targets.RemoveAll(player =>
        {
            var entityPos = player.GetPosition();
            VInt2 point = new VInt2(entityPos.x, entityPos.y);
            VInt3 centerV3 = mTargetMonster.GetPosition();
            VInt2 center = new VInt2(centerV3.x, centerV3.y);
            int distance = (center - point).magnitude;

            if (distance < VInt.NewVInt(buffInfoData.buffRangeRadius, (long) GlobalLogic.VALUE_1000))
            {
                return false;
            }

            return true;
        });
        
        for (int i = 0; i < targets.Count; i++)
        {
            targets[i].buffController.TryAddBuffInfo(mAddBuffInfoID, owner, level);
        }
        GamePool.ListPool<BeActor>.Release(targets);
    }
    
    protected void SwitchState()
    {
        if (!mIsActive)
            return;
        
        // 罩子死了就不用再次可点击了
        if (mTargetMonster == null || mTargetMonster.IsDead())
            return;
        
        pressMode = SkillPressMode.TWO_PRESS_OUT;
        skillButtonState = SkillState.WAIT_FOR_NEXT_PRESS;
        ChangeButtonEffect();
    }
    
    protected void RemoveHandle()
    {
        if (mHandleList == null)
            return;
        
        for (int i = 0; i < mHandleList.Count; i++)
        {
            if (mHandleList[i] == null)
                continue;
            mHandleList[i].Remove();
            mHandleList[i] = null;
        }
        mHandleList.Clear();
    }
}
