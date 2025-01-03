using System.Collections;
using System.Collections.Generic;
using GameClient;

/// <summary>
/// 19年9月装备机制 攻击敌人时标记敌人，当标记层数到达一定数量造成额外伤害
/// </summary>
public class Mechanism1068 : BeMechanism
{
    private int mTagCount = 0;
    private int mUnTagCount = 100;
    private int mHurtId = 0;
    private int mTagBuffDuration = 0;//(ms)
    private int mCDBuffIdDuration = 0;//(ms)
    
    private List<targetUnit> mTargetList = new List<targetUnit>();

    //优化
    private int reduceMul;

    #region Target Interface
    //用于维护目标层数 触发cd等
    class targetUnit
    {
        public int pid;
        public int tagCount;//累计层数
        public int tagDuration; //累计持续时间 到时间直接清零计数，打脏标记
        public int tagCD;   //累计清零后CD

        public bool mDirty;

        public BeEvent.BeEventHandleNew deadEventHandle;
        public void Reset()
        {
            pid = -1;
            tagCount = 0;
            tagDuration = 0;
            tagCD = 0;
            if(deadEventHandle != null)
            {
                deadEventHandle.Remove();
                deadEventHandle = null;
            }
            mDirty = true;
        }
    }

    private targetUnit GetOneTargetUnit()
    {
        if(mTargetList == null)
        {
            mTargetList = new List<targetUnit>();
        }
        targetUnit target;
        for(int i = 0; i < mTargetList.Count; ++i)
        {
            if (mTargetList[i] != null && mTargetList[i].mDirty) 
            {
                return mTargetList[i];
            }
        }
        target = new targetUnit();
        mTargetList.Add(target);
        return target;
    }

    private void UpdateTargetList(int deltaTime)
    {
        if(mTargetList == null)
        {
            return;
        }
        int num = 0;
        for(int i = 0; i < mTargetList.Count; ++i)
        {
            if (mTargetList[i] != null && !mTargetList[i].mDirty)
            {
                num++;
                if(mTargetList[i].tagCD > 0)
                {
                    mTargetList[i].tagCD -= deltaTime;
                    if(mTargetList[i].tagCD < 0)
                    {
                        mTargetList[i].Reset();
                    }
                }
                if(mTargetList[i].tagDuration > 0)
                {
                    mTargetList[i].tagDuration -= deltaTime;
                    if(mTargetList[i].tagDuration < 0)
                    {
                        mTargetList[i].Reset();
                    }
                }
            }
        }
        if (mTargetList.Count - num > reduceMul) //空余元素大于20个时优化一次
        {
            var tempList = new List<targetUnit>(num);

            for (int i = 0; i < mTargetList.Count; ++i)
            {
                if (mTargetList[i] != null && !mTargetList[i].mDirty)
                {
                    tempList.Add(mTargetList[i]);
                }
            }

            mTargetList.Clear();
            mTargetList = tempList;
        }
    }

    private targetUnit GetTargetByPid(int pid)
    {
        if(mTargetList == null)
        {
            return null;
        }
        for (int i = 0; i < mTargetList.Count; ++i)
        {
            if (mTargetList[i] != null && !mTargetList[i].mDirty && mTargetList[i].pid == pid)
            {
                return mTargetList[i];
            }
        }
        return null;
    }
    #endregion

    public Mechanism1068(int mid, int lv) : base(mid, lv) { }

    public override void OnInit()
    {
        mTagCount = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        mUnTagCount = TableManager.GetValueFromUnionCell(data.ValueA[1], level);

        mHurtId = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        
        mTagBuffDuration = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
        mCDBuffIdDuration = TableManager.GetValueFromUnionCell(data.ValueD[0], level);

        var value = TableManager.GetValueFromUnionCell(data.ValueE[0], level);
        if (value == 0)
        {
            reduceMul = 20;
        }
        else
        {
            reduceMul = value;
        }
    }

    public override void OnReset()
    {
        clearTargetList();
    }

    public override void OnStart()
    {
        handleA = owner.RegisterEventNew(BeEventType.onAfterFinalDamageNew, args1 =>
        //handleA = owner.RegisterEvent(BeEventType.onAfterFinalDamageNew, (object[] args1) =>
        {
            var target = args1.m_Obj as BeActor;
            var hurtID = args1.m_Int2;
            if (target == null || hurtID == mHurtId)
            {
                return;
            }
            var targetUnit = GetTargetByPid(target.GetPID());
            if (targetUnit != null) //如果有对象，也表示有buff
            {
                if (targetUnit.tagCD <= 0)
                {
                    targetUnit.tagCount++;
                    if (targetUnit.tagCount >= mUnTagCount)
                    {
                        targetUnit.tagCount = 0;
                        targetUnit.tagDuration = 0;
                        targetUnit.tagCD = mCDBuffIdDuration;
                        owner.DoAttackTo(target, mHurtId, false, true); //触发伤害接口 附加伤害
                    }
                    else //未到达层数 刷新时间
                    {
                        targetUnit.tagDuration = mTagBuffDuration;
                    }
                }
            }
            else //无buff 添加buff 初始化设置值
            {
                targetUnit = GetOneTargetUnit();
                targetUnit.pid = target.GetPID();
                targetUnit.tagCount = 1;//这里暂时不考虑一层就触发伤害
                targetUnit.tagDuration = mTagBuffDuration;
                targetUnit.deadEventHandle = target.RegisterEventNew(BeEventType.onDead, eventParam =>
                {
                    var target1 = eventParam.m_Obj as BeActor;
                    if(target1 != null)
                    {
                        var target1Unit = GetTargetByPid(target1.GetPID());
                        if (target1Unit != null)
                        {
                            target1Unit.Reset();
                        }
                    }
                });
                targetUnit.mDirty = false;
            }
        });
    }

    public override void OnUpdate(int deltaTime)
    {
        UpdateTargetList(deltaTime);
    }

    public override void OnFinish()
    {
        clearTargetList();
    }

    void clearTargetList()
    {
        if (mTargetList != null)
        {
            for (int i = 0; i < mTargetList.Count; ++i)
            {
                if (mTargetList[i] != null)
                {
                    mTargetList[i].Reset();
                }
            }
            mTargetList.Clear();
        }
    }
}
