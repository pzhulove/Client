using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using GameClient;

/// <summary>
/// 召唤物的技能功能
/// 用于召唤类技能的技能按键可点击功能
/// </summary>
public abstract class SummonSkill : BeSkill
{
    protected SummonSkill(int sid, int skillLevel) : base(sid, skillLevel)
    {
    }

    protected BeActor Monster;
    protected bool CanClickAgain = false;
    private IBeEventHandle mSummonHandle;
    protected int mMonsterID;

    public override void OnInit()
    {
        mMonsterID = TableManager.GetValueFromUnionCell(skillData.ValueA[0], level);
    }

    public override void OnStart()
    {
        _RemoveHandle();
        _RegisterHandle();
    }

    public override void OnFinish()
    {
        base.OnFinish();
        _RemoveHandle();
    }

    /// <summary>
    /// 监听召唤
    /// </summary>
    private void _RegisterHandle()
    {
        mSummonHandle = owner.RegisterEventNew(BeEventType.onSummon, _OnSummon);
    }

    protected void _OnSummon(BeEvent.BeEventParam args)
    {
        var monster = args.m_Obj as BeActor;
        if (monster == null)
            return;
        if (!BeUtility.IsMonsterIDEqual(monster.GetEntityData().monsterID, mMonsterID))
            return;

        CanClickAgain = true;
        Monster = monster;
        Monster.RegisterEventNew(BeEventType.onDead, _OnMonsterDead);
        ResetButtonEffect();
        OnSummon(Monster);
    }

    public override void OnCancel()
    {
        if (!CanClickAgain)
            ResetButtonEffect();
    }

    protected virtual void OnSummon(BeActor monster)
    {
        skillButtonState = SkillState.WAIT_FOR_NEXT_PRESS;
        ChangeButtonEffect();
    }

    protected virtual void OnMonsterDead(BeActor monster) { }


    /// <summary>
    /// 召唤的怪物死亡
    /// </summary>
    private void _OnMonsterDead(BeEvent.BeEventParam eventParam)
    {
        if(Monster == null)
            return;
        
        OnMonsterDead(Monster);
        Monster = null;
        CanClickAgain = false;
        ResetButtonEffect();
    }

    private void _RemoveHandle()
    {
        if (mSummonHandle != null)
        {
            mSummonHandle.Remove();
            mSummonHandle = null;
        }
    }
}

/// <summary>
/// 再次点击技能将召唤物拉到身前指定位置后主角与召唤物同时释放指定技能
/// </summary>
public abstract class MixSummonSkill : SummonSkill, ISummonSkillUpdate
{
    protected MixSummonSkill(int sid, int skillLevel) : base(sid, skillLevel)
    {
    }

    private int mOwnerTargetSkillId;
    protected int OwnerTargetSkillId => mOwnerTargetSkillId;
    
    private int mSummonTargetSkillId;
    protected int SummonTargetSkillId => mSummonTargetSkillId;
    private VInt2 mEndPositionOffset;
    
    protected bool m_IsNeedEndMonsterSkill = true;
    private string m_SkillReleaseFlag;
    private List<IBeEventHandle> mHandles = new List<IBeEventHandle>();
    private bool m_CanMix = false;

    public override void OnInit()
    {
        base.OnInit();
        if (skillData.ValueB.Count == 2)
        {
            mOwnerTargetSkillId = TableManager.GetValueFromUnionCell(skillData.ValueB[0], level);
            mSummonTargetSkillId = TableManager.GetValueFromUnionCell(skillData.ValueB[1], level);    
        }
        mEndPositionOffset = new VInt2(TableManager.GetValueFromUnionCell(skillData.ValueC[0], level), TableManager.GetValueFromUnionCell(skillData.ValueC[1], level));
        if (skillData.ValueD.Length > 0)
        {
            m_SkillReleaseFlag = TableManager.GetValueFromUnionCell(skillData.ValueD[0], level).ToString();
        }
    }

    public override void OnPostInit()
    {
        base.OnPostInit();
        m_CanMix = owner.HasSkill(2403);
    }

    public override int DisplayCD { 
        get
        {
            if (!m_CanMix)
                return base.DisplayCD;
            
            if (Monster != null)
            {
                int ret = 0;
                if (OwnerTargetSkillId > 0)
                {
                    ret = owner.GetSkill(OwnerTargetSkillId).CDTimeAcc;
                }

                if (mSummonTargetSkillId > 0)
                {
                    ret = IntMath.Max(ret, Monster.GetSkill(mSummonTargetSkillId).CDTimeAcc);
                }

                return ret;
            }
            else
            {
                return base.DisplayCD;
            }
        }
    }

    public override int DisplayFullCD
    {
        get
        {
            if (!m_CanMix)
                return base.DisplayFullCD;
            
            if (Monster != null)
            {
                int ret = 0;
                if (OwnerTargetSkillId > 0)
                {
                    ret = owner.GetSkill(OwnerTargetSkillId).GetCurrentCD();
                }

                if (mSummonTargetSkillId > 0)
                {
                    ret = IntMath.Max(ret, Monster.GetSkill(mSummonTargetSkillId).GetCurrentCD());
                }
                return ret;
            }
            else
            {
                return base.DisplayFullCD;
            }
        }
    }

    public override bool IsDisplayCDing
    {
        get
        {
            if (!m_CanMix)
                return base.IsDisplayCDing;
            
            if (Monster != null)
            {
                bool ret = false;
                if (OwnerTargetSkillId > 0)
                {
                    ret = owner.GetSkill(OwnerTargetSkillId).isCooldown;
                }
        
                if (mSummonTargetSkillId > 0)
                {
                    ret = Monster.GetSkill(mSummonTargetSkillId).isCooldown || ret;
                }

                return ret;
            }
            else
            {
                return base.IsDisplayCDing;
            }
        }
    }
    
    protected override void OnSummon(BeActor monster)
    {
        // 不学技能不能合体
        if (!m_CanMix)
            return;
        
        base.OnSummon(monster);
        
        if (monster != null)
        {
            Mechanism1505.BindUpdate(monster, this);
        }
    }
    
    public override void OnClickAgain()
    {
        base.OnClickAgain();

        if (CanMix())
        {
            RegisterEvent();
            StartMix();
        }
    }
    
    protected virtual bool CanMix()
    {
        return OwnerCanUseSkill() && MonsterCanUseSkill();
    }

    private void RegisterEvent()
    {
        RemoveHandle();
        mHandles.Add(owner.RegisterEventNew(BeEventType.onCastSkillFinish, OnSkillFinish));
        mHandles.Add(owner.RegisterEventNew(BeEventType.onSkillCancel, OnSkillCancel));
        mHandles.Add(owner.RegisterEventNew(BeEventType.onSkillCurFrame, OnSkillFrame));
    }

    protected virtual VInt3 GetMixPosition()
    {
        var curPos = owner.GetPosition();
        curPos.x += owner.GetFace() ? -mEndPositionOffset.x : mEndPositionOffset.x;
        curPos.y += mEndPositionOffset.y;
        curPos.z = 0;
        if (owner.CurrentBeScene != null && owner.CurrentBeScene.IsInBlockPlayer(curPos))
            curPos = owner.GetPosition();
        return curPos;
    }
    
    protected virtual void StartMix()
    {
        if(Monster == null)
            return;

        m_IsNeedEndMonsterSkill = true;

        if (mOwnerTargetSkillId > 0)
        {
            SetMixSkillLevel();
            owner.UseSkill(mOwnerTargetSkillId);
        }
        
        Monster.SetPosition(GetMixPosition());
        Monster.SetFace(owner.GetFace());
        Monster.aiManager?.StopCurrentCommand();
        if(mSummonTargetSkillId > 0)
            Monster.UseSkill(mSummonTargetSkillId, true);
    }

    protected virtual void EndMix(){ }

    private void OnSkillCancel(BeEvent.BeEventParam args)
    {
        if (args.m_Int == mOwnerTargetSkillId)
        {
            if (m_IsNeedEndMonsterSkill)
            {
                if (Monster != null && !Monster.IsDeadOrRemoved() && Monster.GetCurSkillID() == mSummonTargetSkillId)
                {
                    Monster.Locomote(new BeStateData((int)ActionState.AS_IDLE), true);
                }
            }
            EndMix();
        }
    }
    
    private void OnSkillFinish(BeEvent.BeEventParam args)
    {
        if (args.m_Int == mOwnerTargetSkillId)
        {
            EndMix();
        }
    }

    private void OnSkillFrame(BeEvent.BeEventParam args)
    {
        if (args.m_String.Equals(m_SkillReleaseFlag) && owner.GetCurSkillID() == mOwnerTargetSkillId)
        {
            m_IsNeedEndMonsterSkill = false;
        }
    }

    
    private void RemoveHandle()
    {
        for (int i = 0; i < mHandles.Count; i++)
        {
            mHandles[i].Remove();
        }

        mHandles.Clear();
    }
    
    
    private bool OwnerCanUseSkill()
    {
        if (OwnerTargetSkillId == 0)
            return true;
        
        if (owner.IsDeadOrRemoved() || owner.IsPassiveState())
            return false;

        return owner.CanUseSkill(OwnerTargetSkillId);
    }
    
    private bool MonsterCanUseSkill()
    {
        if (SummonTargetSkillId == 0)
            return true;
        
        if (Monster == null)
            return false;
        
        if (Monster.IsDeadOrRemoved() || Monster.IsPassiveState())
            return false;
        
        return Monster.CanUseSkill(SummonTargetSkillId);
    }

    public void OnSummonUpdate(int deltaTime)
    {
        SetLightButtonVisible(CanMix());
    }

    public void OnSummonUpdateFinish()
    {
        SetLightButtonVisible(false);
    }
    
    private void SetMixSkillLevel()
    {
        if (mOwnerTargetSkillId > 0)
        {
            var skill = owner.GetSkill(mOwnerTargetSkillId);
            if (skill != null)
            {
                skill.level = level;
            }
        }
    }
}

/// <summary>
/// 兔子
/// </summary>
public class Skill2400 : MixSummonSkill
{
    public Skill2400(int sid, int skillLevel) : base(sid, skillLevel)
    {
    }
}

/// <summary>
/// 追猎
/// </summary>
public class Skill2412 : MixSummonSkill
{
    public Skill2412(int sid, int skillLevel) : base(sid, skillLevel)
    {
    }
}

/// <summary>
/// 树人
/// </summary>
public class Skill2407 : MixSummonSkill
{
    public Skill2407(int sid, int skillLevel) : base(sid, skillLevel)
    {
    }
}

/// <summary>
/// 狼
/// </summary>
public class Skill2402 : MixSummonSkill
{
    public Skill2402(int sid, int skillLevel) : base(sid, skillLevel)
    {
    }

    protected override void StartMix()
    {
        base.StartMix();
        SetActorVisible(false);
    }

    protected override void EndMix()
    {
        base.EndMix();
        SetActorVisible(true);
    }
    
        
    private void SetActorVisible(bool visible)
    {
#if !LOGIC_SERVER
        if (Monster != null && Monster.m_pkGeActor != null)
        {
            Monster.m_pkGeActor.SetFootIndicatorVisible(visible);
            Monster.m_pkGeActor.SetHeadInfoVisible(visible);
            Monster.m_pkGeActor.SetHPBarVisible(visible);
        }
#endif
    }
}