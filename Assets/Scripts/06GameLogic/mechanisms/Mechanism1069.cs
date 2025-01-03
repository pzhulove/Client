using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 19年9月装备机制 对怪打标记 对标记怪打触发效果同时对自身增加buff
/// </summary>
public class Mechanism1069 : BeMechanism
{
    //标记buff
    private int mTagBuffId = 0;
    private int mTagBuffDuration = 0;
    //对怪触发效果表
    private int mTargetEffectTableId = 0;
    //自身buff
    private int mSelfBuffId = 0;
    private int mSelfBuffDuration = 0;
    private int mSelfBuffRate = 0;
    private int mSelfBuffCD = 0;

    private int mSelfBuffCDTimer = 0;
    private bool isSelfBuffInCD = false;
    public Mechanism1069(int mid, int lv) : base(mid, lv) { }

    public override void OnInit()
    {
        mTagBuffId = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        mTagBuffDuration = TableManager.GetValueFromUnionCell(data.ValueA[1], level);

        mTargetEffectTableId = TableManager.GetValueFromUnionCell(data.ValueB[0], level);

        mSelfBuffId = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
        mSelfBuffDuration = TableManager.GetValueFromUnionCell(data.ValueC[1], level);
        mSelfBuffRate = TableManager.GetValueFromUnionCell(data.ValueC[2], level);

        mSelfBuffCD = TableManager.GetValueFromUnionCell(data.ValueD[0], level);
    }

    public override void OnReset()
    {
        mSelfBuffCDTimer = 0;
        isSelfBuffInCD = false;
    }

    public override void OnStart()
    {
        if (owner == null)
        {
            return;
        }

        handleA = owner.RegisterEventNew(BeEventType.onAfterFinalDamageNew, args1 =>
        //handleA = owner.RegisterEvent(BeEventType.onAfterFinalDamageNew, (object[] args1) => 
        {
            var target = args1.m_Obj as BeActor;
            var hurtID = args1.m_Int2;
            if(target == null || hurtID == mTargetEffectTableId)
            {
                return;
            }
            var tagBuff = target.buffController.HasBuffByID(mTagBuffId);
            if(tagBuff == null)
            {
                target.buffController.TryAddBuff(mTagBuffId, mTagBuffDuration, 1, GlobalLogic.VALUE_1000);
            }
            else
            {
                owner.DoAttackTo(target, mTargetEffectTableId, false, true);
                if (!isSelfBuffInCD)//CD限制需求 如果加过buff进入cd
                {
                    if(mSelfBuffRate>= FrameRandom.Range1000())//概率限制需求 有概率对自身加buff
                    {
                        owner.buffController.TryAddBuff(mSelfBuffId, mSelfBuffDuration);
                        isSelfBuffInCD = true;
                    }
                }
            }
        });
    }

    public override void OnUpdate(int deltaTime)
    {
        if (isSelfBuffInCD)
        {
            mSelfBuffCDTimer += deltaTime;
            if (mSelfBuffCDTimer >= mSelfBuffCD)
            {
                mSelfBuffCDTimer = 0;
                isSelfBuffInCD = false;
            }
        }
    }
}
