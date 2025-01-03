using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// 交叉射击
/// </summary>
public class Skill1300 : BeSkill
{
    protected int mStreSilverBuffId = 0;                                //强化银弹BuffId
    protected int mIceBuffId = 0;                                       //冰冻弹BuffId
    protected int mFireBuffId = 0;                                      //爆炎弹BuffId

    protected int mOriginalPhaseId = 1300;                              //正常阶段ID
    protected int mReplacePhaseIdSilver = 13002;                        //银弹替换阶段ID
    protected int mReplacePhaseIdFire = 13003;                          //爆炎弹替换阶段ID
    protected int mReplacePhaseIdIce = 13004;                           //冰冻弹替换阶段ID

    protected int mUsedBuffId = 0;                                      //记录最后一次使用的特殊弹Buff 

    protected int mXForceCutRate = 0;                                   //x轴力随距离减少系数
    protected List<int> effectIdList = new List<int>();                 //这个技能加上buff后的所有推力触发效果
    protected List<int> targetList = new List<int>();
    protected int mXPosition = 0;
    protected bool mFace = false;
    protected int mStartXDis = 1000;                                    //x轴力改变起始距离

    protected IBeEventHandle mBuffAddHandle = null;                      //监听Buff添加
    protected IBeEventHandle mBuffFinishHandle = null;                   //监听Buff结束
    protected IBeEventHandle mBuffRemoveHandle = null;                   //监听Buff移除
    protected IBeEventHandle mPreSkillPhaseHandle = null;                //监听技能阶段   
    protected IBeEventHandle mRebornHandle = null;                       //监听复活     
    protected List<IBeEventHandle> mChangeXRateHandle = new List<IBeEventHandle>(); //监听X轴推力改变
    protected IBeEventHandle mHitHandle = null;
    protected IBeEventHandle mAfterGenBulletHandle = null;

    public Skill1300(int sid, int skillLevel) : base(sid, skillLevel)
    {

    }

    public override void OnPostInit()
    {
        mXForceCutRate = TableManager.GetValueFromUnionCell(skillData.ValueD[0], level);
    }

    public override void OnInit()
    {
        mStreSilverBuffId = TableManager.GetValueFromUnionCell(skillData.ValueA[0], level);
        mIceBuffId = TableManager.GetValueFromUnionCell(skillData.ValueB[0], level);
        mFireBuffId = TableManager.GetValueFromUnionCell(skillData.ValueC[0], level);
        mXForceCutRate = TableManager.GetValueFromUnionCell(skillData.ValueD[0], level);
        for (int i = 0; i < skillData.ValueE.Count; ++i)
        {
            effectIdList.Add(TableManager.GetValueFromUnionCell(skillData.ValueE[i], level));
        }
        mStartXDis = TableManager.GetValueFromUnionCell(skillData.ValueG[0], level);

        RemoveHandle();
        mBuffAddHandle = owner.RegisterEventNew(BeEventType.onAddBuff, (args) =>
        {
            BeBuff buff = (BeBuff)args.m_Obj;
            if (null != buff)
            {
                if(mStreSilverBuffId == buff.buffID || mFireBuffId == buff.buffID || mIceBuffId == buff.buffID)
                {
                    mUsedBuffId = buff.buffID;
                }
            }
        });

        mBuffFinishHandle = owner.RegisterEventNew(BeEventType.onBuffFinish, (args) =>
        {
            int buffId = (int)args.m_Int;
            if (buffId == mUsedBuffId)
            {
                mUsedBuffId = 0;
            }
        });

        mBuffRemoveHandle = owner.RegisterEventNew(BeEventType.onRemoveBuff, (args) =>
        {
            int buffId = (int)args.m_Int;
            if (buffId == mUsedBuffId)
            {
                mUsedBuffId = 0;
            }
        });

        mPreSkillPhaseHandle = owner.RegisterEventNew(BeEventType.onPreSetSkillAction, (GameClient.BeEvent.BeEventParam param) =>
        {
            //int[] skillPhaseList = (int[])args[0];
            if (param.m_Int == mOriginalPhaseId)
            {
                if (mUsedBuffId != 0)
                {
                    if (mStreSilverBuffId == mUsedBuffId)
                    {
                        param.m_Int = mReplacePhaseIdSilver;
                    }
                    else if (mFireBuffId == mUsedBuffId)
                    {
                        param.m_Int = mReplacePhaseIdFire;
                    }
                    else if (mIceBuffId == mUsedBuffId)
                    {
                        param.m_Int = mReplacePhaseIdIce;
                    }
                }
            }
        });

        mRebornHandle = owner.RegisterEventNew(BeEventType.onReborn, args =>
        {
            mUsedBuffId = 0;                    //重置
        });

    }

    public override void OnStart()
    {
        mXPosition = owner.GetPosition().x;
        mFace = owner.GetFace();
        mAfterGenBulletHandle = owner.RegisterEventNew(BeEventType.onAfterGenBullet, arg2 =>
        {
            BeProjectile projectile = arg2.m_Obj as BeProjectile;
            if (projectile != null) 
            {
                mHitHandle = projectile.RegisterEventNew(BeEventType.onHitOther, args =>
                {
                    var target = args.m_Obj as BeActor;
                    var hurtId = (int)args.m_Int;
                    if (target != null && effectIdList.Contains(hurtId) && !targetList.Contains(target.GetPID()))
                    {
                        targetList.Add(target.GetPID());

                        mChangeXRateHandle.Add(target.RegisterEventNew(BeEventType.onChangeXRate, (GameClient.BeEvent.BeEventParam param) =>
                        {
                            if (effectIdList.Contains(param.m_Vint3.x)) 
                            {
                                //var xForceArray = (int[])arg1[1];
                                int targetXPosition = target.GetPosition().x;
                                if (Math.Abs(targetXPosition - mXPosition) <= mStartXDis)
                                {
                                    return;
                                }
                                param.m_Vint3.y += (!mFace ? 1 : -1) * mXForceCutRate * (targetXPosition - mXPosition) / 1000;
                                //if (!mFace)
                                //{
                                //    xForceArray[0] = Mathf.Max(0, xForceArray[0]);
                                //}
                                //else
                                //{
                                //    xForceArray[0] = Mathf.Min(0, xForceArray[0]);
                                //}
                            }
                        }));
                    }
                });
            }
        });
    }

    public override void OnCancel()
    {
        ClearNewHandleEvent();
        targetList.Clear();
    }

    public override void OnFinish()
    {
        ClearNewHandleEvent();
        targetList.Clear();
    }

    protected void RemoveHandle()
    {
        if (mBuffAddHandle != null)
        {
            mBuffAddHandle.Remove();
            mBuffAddHandle = null;
        }

        if (mBuffRemoveHandle != null)
        {
            mBuffRemoveHandle.Remove();
            mBuffRemoveHandle = null;
        }

        if (mPreSkillPhaseHandle != null)
        {
            mPreSkillPhaseHandle.Remove();
            mPreSkillPhaseHandle = null;
        }

        if (mRebornHandle != null)
        {
            mRebornHandle.Remove();
            mRebornHandle = null;
        }

        if (mBuffFinishHandle != null)
        {
            mBuffFinishHandle.Remove();
            mBuffFinishHandle = null;
        }
    }

    private void ClearNewHandleEvent()
    {
        if (mHitHandle != null)
        {
            mHitHandle.Remove();
            mHitHandle = null;
        }

        for (int i = 0; i < mChangeXRateHandle.Count; ++i)
        {
            mChangeXRateHandle[i].Remove();
            mChangeXRateHandle[i] = null;
        }
        mChangeXRateHandle.Clear();

        if(mAfterGenBulletHandle != null)
        {
            mAfterGenBulletHandle.Remove();
            mAfterGenBulletHandle = null;
        }
    }
}
