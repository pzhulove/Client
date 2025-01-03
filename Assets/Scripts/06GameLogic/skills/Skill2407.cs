/*using System.Collections.Generic;
using DG.Tweening;
using GameClient;
using UnityEngine;

/// <summary>
/// 树人
/// 再次点击时，主角释放技能，在某一帧后隐藏树人，在一定时候后还原树人
/// </summary>
public class Skill2407 : SummonSkill , ISummonSkillUpdate
{
    public Skill2407(int sid, int skillLevel) : base(sid, skillLevel)
    {
    }
    
    private List<IBeEventHandle> mHandles = new List<IBeEventHandle>();
    private int mOwnerTargetSkillId;
    protected int OwnerTargetSkillId => mOwnerTargetSkillId;
    private string m_SkillReleaseFlag;
    private int m_MixBuffID;
    private int m_EndPositionOffsetX;
    private int m_MixStartTime;
    
    
    public override int DisplayCD { 
        get
        {
            if (Monster != null)
            {
                return owner.GetSkill(OwnerTargetSkillId).CDTimeAcc;
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
            if (Monster != null)
            {
                return owner.GetSkill(OwnerTargetSkillId).GetCurrentCD();
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
            if (Monster != null)
            {
                return owner.GetSkill(OwnerTargetSkillId).isCooldown;
            }
            else
            {
                return base.IsDisplayCDing;
            }
        }
    }
    
    public override void OnInit()
    {
        base.OnInit();
        mOwnerTargetSkillId = TableManager.GetValueFromUnionCell(skillData.ValueB[0], level);
        m_SkillReleaseFlag = TableManager.GetValueFromUnionCell(skillData.ValueC[0], level).ToString();
        m_MixBuffID = TableManager.GetValueFromUnionCell(skillData.ValueD[0], level);
        m_EndPositionOffsetX = TableManager.GetValueFromUnionCell(skillData.ValueE[0], level);
    }

    protected override void OnMonsterDead(BeActor monster)
    {
        // 保护:主角死亡时（再掉召唤物死亡）,召唤物可能在合体中，在temp中时，不能移除表现。需要还原后自然移除表现
        if(monster != null)
            owner.CurrentBeScene?.RestoreFromTemp(monster);
    }

    protected override void OnSummon(BeActor monster)
    {
        base.OnSummon(monster);
        if (monster != null)
        {
            Mechanism1505.BindUpdate(monster, this);
        }
    }
    
    public override bool CanUseSkill()
    {
        return base.CanUseSkill() && Monster == null;
    }


    public override void OnClickAgain()
    {
        base.OnClickAgain();

        if (CanMix())
        {
            RegisterEvent();
            SetMixSkillLevel();
            owner.UseSkill(mOwnerTargetSkillId);
            SetLightButtonVisible(false);
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
    
    private void RegisterEvent()
    {
        RemoveHandle();
        mHandles.Add(owner.RegisterEventNew(BeEventType.onSkillCurFrame, OnSkillFrame));
        mHandles.Add(owner.RegisterEventNew(BeEventType.onRemoveBuff, OnRemoveBuff));
    }

    private void OnRemoveBuff(BeEvent.BeEventParam args)
    {
        int buffId = (int) args.m_Int;
        if (buffId == m_MixBuffID)
        {
            EndMix();
        }
    }

    private void OnSkillFrame(BeEvent.BeEventParam args)
    {
        if (args.m_String.Equals(m_SkillReleaseFlag) && owner.GetCurSkillID() == mOwnerTargetSkillId)
        {
            StartMix();
        }
    }

    protected void StartMix()
    {
        if (Monster != null)
        {
#if !LOGIC_SERVER
            FlyEffect(Monster.GetPosition().vector3, owner.GetPosition().vector3 + Vector3.up);
#endif
            
            Monster.CurrentBeScene?.RemoveToTemp(Monster);
            Monster.SetPosition(new VInt3(10000f, 10000f, 0f), true);
            Monster.aiManager.Stop();
            
            if (owner.CurrentBeScene != null)
            {
                m_MixStartTime = owner.CurrentBeScene.GameTime;
            }
            

        }
    }

    protected void EndMix()
    {
        if(Monster == null)
            return;
        int time = 300;
        owner.delayCaller.DelayCall(time, OnFlyEnd);
#if !LOGIC_SERVER
        FlyEffect(owner.GetPosition().vector3 + Vector3.up, GetEndPosition().vector3 + Vector3.up, time / 1000f);
#endif
    }

    private void OnFlyEnd()
    {
        if(Monster == null)
            return;
            
        Monster.aiManager.Start();
        Monster.CurrentBeScene?.RestoreFromTemp(Monster);
        Monster.SetPosition(GetEndPosition());
        
        if (owner.CurrentBeScene != null)
        {
            int time = owner.CurrentBeScene.GameTime - m_MixStartTime;
            var buff = Monster.buffController.HasBuffByID((int) GlobalBuff.LIFE_TIME);
            if (buff != null)
            {
                buff.Update(time);
            }
        }
    }

    protected VInt3 GetEndPosition()
    {
        var curPos = owner.GetPosition();
        curPos.z = 0;
        curPos.x += owner.GetFace() ? -m_EndPositionOffsetX : m_EndPositionOffsetX;
        
        if (owner.CurrentBeScene != null && owner.CurrentBeScene.IsInBlockPlayer(curPos))
            curPos = owner.GetPosition();
        return curPos;
    }

    
    protected virtual bool CanMix()
    {
        return OwnerCanUseSkill() && owner.buffController.HasBuffByID(m_MixBuffID) == null;
    }
    
    private bool OwnerCanUseSkill()
    {
        if (owner.IsDeadOrRemoved() || owner.IsPassiveState())
            return false;

        return owner.CanUseSkill(OwnerTargetSkillId);
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
    
#if !LOGIC_SERVER
    private GeEffectEx mWeaponFlyEffect;
    private void FlyEffect(Vector3 startPos, Vector3 endPos, float time = 0.3f)
    {
        if(Monster == null)
            return;
        
        DestroyEffect();
        mWeaponFlyEffect = owner?.CurrentBeScene?.currentGeScene?.CreateEffect(1029, new Vec3(startPos));
        if (mWeaponFlyEffect != null)
        {
            if(mWeaponFlyEffect.GetRootNode() != null)
            { 
                mWeaponFlyEffect.GetRootNode().gameObject.transform.DOJump(endPos, 0.5f, 1, time).SetEase(Ease.InOutQuad).OnComplete(DestroyEffect);
            }
        }
    }
    
    protected void DestroyEffect()
    {
        if (mWeaponFlyEffect != null)
        {
            owner?.CurrentBeScene?.currentGeScene?.DestroyEffect(mWeaponFlyEffect);
            mWeaponFlyEffect = null;     
        }
    }
#endif

}*/