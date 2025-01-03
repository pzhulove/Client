using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 弹药改良
/// </summary>
public class Skill1301 : BeSkill
{
    protected List<int> mWeaponTypeList = new List<int>();                          //武器类型
    protected List<int> mReplaceAttackIdList = new List<int>();                     //替换的普攻ID 
    protected List<int> mEffectWeaponTypeList = new List<int>();                    //会触发触发效果的武器类型
    protected int[] mPveBuffIdArr = new int[2];                                     //pve对应的Buff数组
    protected int[] mPvpBuffIdArr = new int[2];                                     //pvp对应的Buff数组

    protected BeActor.NormalAttack mAttackData = new BeActor.NormalAttack();
    protected IBeEventHandle mRebornHandle = null;               


    public Skill1301(int sid, int skillLevel):base(sid, skillLevel){}

    public override void OnInit()
    {
        mWeaponTypeList.Clear();
        mReplaceAttackIdList.Clear();
        mEffectWeaponTypeList.Clear();


        if (skillData.ValueA.Count > 0)
        {
            for(int i = 0; i < skillData.ValueA.Count; i++)
            {
                mWeaponTypeList.Add(TableManager.GetValueFromUnionCell(skillData.ValueA[i], level));
            }
        }

        
        if (skillData.ValueB.Count > 0)
        {
            for(int i = 0; i < skillData.ValueB.Count; i++)
            {
                mReplaceAttackIdList.Add(TableManager.GetValueFromUnionCell(skillData.ValueB[i],level));
            }
        }

        
        for (int i = 0; i < skillData.ValueC.Count; i++)
        {
            mEffectWeaponTypeList.Add(TableManager.GetValueFromUnionCell(skillData.ValueC[i], level));
        }

        for(int i = 0; i < skillData.ValueD.Count; i++)
        {
            mPveBuffIdArr[i] = TableManager.GetValueFromUnionCell(skillData.ValueD[i], level);
        }

        for(int i = 0; i < skillData.ValueE.Count; i++)
        {
            mPvpBuffIdArr[i] = TableManager.GetValueFromUnionCell(skillData.ValueE[i], level);
        }
    }

    public override void OnPostInit()
    {
        RemoveHandle();
        mRebornHandle = owner.RegisterEventNew(BeEventType.onReborn, args => 
        {
            UseEffectId();
            ReplaceNormalAttack();
        });

        RestoreAttackId();
        ReplaceNormalAttack();
        UseEffectId();
    }

    public override void OnWeaponChange()
    {
        OnPostInit();
    }

    protected void ReplaceNormalAttack()
    {
        int weaponType = owner.GetWeaponType();
        for (int i = 0; i < mReplaceAttackIdList.Count; i++)
        {
            if (weaponType == mWeaponTypeList[i])
            {
                ReplaceAttackId(mReplaceAttackIdList[i]);
            }
        }
    }

    protected void UseEffectId()
    {
        int[] buffIdArr = BattleMain.IsModePvP(battleType) ? mPvpBuffIdArr : mPveBuffIdArr;
        AddBuff(buffIdArr,false);
        if (mEffectWeaponTypeList.Count <= 0)
            return;
        if (!mEffectWeaponTypeList.Contains(owner.GetWeaponType()))
            return;
        AddBuff(buffIdArr);
    }

    protected void AddBuff(int[] buffIdArr,bool isAdd = true)
    {
        for(int i=0;i< buffIdArr.Length; i++)
        {
            int buffId = buffIdArr[i];
            if (isAdd)
            {
                owner.buffController.TryAddBuff(buffId,int.MaxValue,level);
            }
            else
            {
                owner.buffController.RemoveBuff(buffId);
            }
        }
    }

    public override void OnCancel()
    {
        RestoreAttackId();
    }

    public override void OnFinish()
    {
        RestoreAttackId();
    }

    //替换普攻技能
    protected void ReplaceAttackId(int replaceAttackId)
    {
        mAttackData = owner.AddReplaceAttackId(replaceAttackId, 1);
    }

    //还原普攻技能
    protected void RestoreAttackId()
    {
        owner.RemoveReplaceAttackId(mAttackData);
    }

    protected void RemoveHandle()
    {
        if (mRebornHandle != null)
        {
            mRebornHandle.Remove();
            mRebornHandle = null;
        }
    }
}
