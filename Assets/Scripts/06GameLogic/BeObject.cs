using System;
using UnityEngine;
using System.Collections;
using GameClient;
using System.IO;

public sealed class BeObjectStateGraph : BeStatesGraph
{
    public BeObject logicObject;

    public override void InitStatesGraph()
    {
        BeStates kStandState = new BeStates(
                (int)ActionState.AS_IDLE,
                (int)AStateTag.AST_NULLTAG,
                (BeStates pkState) =>
                {
                    var currentStage = logicObject.currentStage;

                    if (currentStage > 0)
                    {
                        logicObject.PlayAction(string.Format("Idle{0}", currentStage));
                        logicObject._ReloadBlockData();
                    }
                    else if (currentStage == 0)
                    {
                        logicObject.PlayAction(ActionType.ActionType_IDLE);
                        logicObject._ReloadBlockData();
                    }
                    else
                    {

                    }
                    
                    SetCurrentStatesTimeout(-1);
                }
            );

        AddStates2Graph(kStandState);

        BeStates kDeadState = new BeStates(
                (int)ActionState.AS_DEAD,
                (int)AStateTag.AST_BUSY,
                (BeStates pkState) =>
                {
                    if (logicObject.HasAction(ActionType.ActionType_DEAD))
                    {
                        logicObject.PlayAction(ActionType.ActionType_DEAD);
                        logicObject.PlayDeadEffect();
					    SetCurrentStatesTimeout((int)(logicObject.m_cpkCurEntityActionInfo.fRealFramesTime));
                    }
					else
                    {
					    SetCurrentStatesTimeout(GlobalLogic.VALUE_10);
                    }
                },
            (BeStates pkState) =>
            {
                logicObject.m_iEntityLifeState = (int)EntityLifeState.ELS_CANREMOVE;
            }
        );

        AddStates2Graph(kDeadState);

        BeStates kHurt = new BeStates(
            (int)ActionState.AS_HURT,
            (int)AStateTag.AST_NULLTAG,
            (BeStates pkState) =>
            {
                int currentStage = logicObject.currentStage;
                var actionName = string.Format((string)"Hurt{0}", currentStage);
                if (logicObject.HasAction(actionName))
                {
                    logicObject.PlayAction(actionName);
                    SetCurrentStatesTimeout(logicObject.GetCurrentActionDuration());
                }
                else
                {
                    SetCurrentStatesTimeout(GlobalLogic.VALUE_1);
                }

            },
            (BeStates pkState) =>
            {

                logicObject.sgSwitchStates(new BeStateData() { _State = (int)ActionState.AS_IDLE });

            }
        );

        AddStates2Graph(kHurt);
    }
}


public class BeObject : BeEntity
{
    public int currentStage
    {
        get
        {
            return mCurrentStage; 
        }
        set
        {
            mCurrentStage = value;
        }
    }

    public void SetSplitCount(ProtoTable.UnionCell split)
    {
        mSplit = split;
    }

    public void SetMaxStage(int stage)
    {
        mMaxStage = stage;
    }

    public void SetDeadEffect(string deadEffect)
    {
        if (deadEffect.Length != 0 && deadEffect != "-")
        {
            mDeadEffect = deadEffect;
        }
    }

    protected int mCurrentStage;
    protected int mMaxStage;
    protected int mCurrentDamageCount;
    protected int mDamageCount;

    protected bool markDead = false;
    protected int delayDead = 10;
    protected bool canTakeDamage = true;

    protected int hurtOtherCount = 0;

    protected ProtoTable.UnionCell mSplit;

    protected string mDeadEffect = "";

    protected ProtoTable.FlatBufferArray<string> mBlockPaths;

    public BeObject(int resID, int camp, int id)
        : base(resID, camp, id)
    {
        mDamageCount = 0;
        mCurrentDamageCount = 0;
        mCurrentStage = 0;
    }

    public virtual void Create(int dc = 1)
    {
        BeObjectStateGraph sg = new BeObjectStateGraph();
        sg.InitStatesGraph();
        sg.logicObject = this;
        sg.m_pkEntity = this;

        mDamageCount = dc;
        mCurrentStage = 0;

        base.Create(sg, (int)ActionState.AS_IDLE, true);

        DoInit();
    }

    public void DoInit()
    {
        if (stateController != null)
        {
            stateController.SetAbilityEnable(BeAbilityType.BEGRAB, false);
        }
    }

    public void PlayDeadEffect()
    {
        if (mDeadEffect.Length > 0)
        {
            this.m_pkGeActor.ChangeSurface(mDeadEffect, 0, true, false);
        }
    }

    public override bool OnDamage()
    {
        if (!canTakeDamage)
            return true;

        ++mCurrentDamageCount;

        if (mCurrentDamageCount >= mDamageCount)
        { 
            if (delayDead > 0 && !markDead)
            {
                markDead = true;
                delayCaller.DelayCall(delayDead, () =>
                {
                    DoDead();
                });
            }
        }
        else
        {
            var count = TableManager.GetValueFromUnionCell(mSplit, mCurrentStage + 1);

            if (mCurrentDamageCount >= count)
            {
                ++mCurrentStage;

                if (mCurrentStage < mMaxStage)
                {
                    sgSwitchStates(new BeStateData() { _State = (int)ActionState.AS_HURT });
                }
            }

            TriggerEventNew(BeEventType.onHit,new EventParam() { });
            //TriggerEvent(BeEventType.onHit);
        }

        return true;
    }

    public void SetDamageCount(int count)
    {
        mDamageCount = count;
    }
    
    public void SetBlockPaths(ProtoTable.FlatBufferArray<string> blockPaths)
    {
        mBlockPaths = blockPaths;
        if (mBlockPaths.Length != mMaxStage - 1)
        {
            Logger.LogWarningFormat("DestrucTable BlockPaths warning value : {0}", this.m_iResID);
        }
    }

    public override bool Update(int iDeltaTime)
    {
        base.Update(iDeltaTime);
        return true;
    }

	public void ForceDoDead()
	{
        bool isForce = false;
        TriggerEventNew(BeEventType.onDead, new EventParam() { m_Bool = true, m_Bool2 = isForce });
        sgSwitchStates(new BeStateData((int)ActionState.AS_DEAD));
	}

    public override void DoDead(bool isForce = false)
    {
        if (HasAction(ActionType.ActionType_DEAD))
        {
            TriggerEventNew(BeEventType.onDead, new EventParam() { m_Bool = true, m_Bool2 = isForce });
            sgSwitchStates(new BeStateData((int)ActionState.AS_DEAD));
        }
    }

    public void SetCanBeBreak(bool flag)
    {
        canTakeDamage = flag;
    }

    public override void onHitEntity(BeEntity pkEntity, VInt3 pos, int hurtID = 0, AttackResult result = AttackResult.MISS, int finalDamage = 0)
    {
        if (hurtOtherCount >= 1)
            return;

		int skillID = 0;
		var data = TableManager.GetInstance().GetTableItem<ProtoTable.EffectTable>(hurtID);
		if (data != null)
			skillID = data.SkillID;

        TriggerEventNew(BeEventType.onHitOther,new EventParam() { m_Obj = pkEntity, m_Int = hurtID, m_Int2 = skillID, m_Vint3 = pos, m_Int3 = 0, m_Int4 = 0,m_Obj2 = this });
		//TriggerEvent(BeEventType.onHitOther, new object[] { pkEntity, hurtID, skillID, pos ,0,0});

        hurtOtherCount++;
    }
    
    public void _ReloadBlockData()
    {
        if (currentStage >= 0 && mBlockPaths != null && mBlockPaths.Length > currentStage)
        {
            string modelDataRes = mBlockPaths[currentStage];
            if (Utility.IsStringValid(modelDataRes))
            {
                DModelData modelData = null;
#if USE_FB
                FBModelDataSerializer.LoadFBModelData(Path.Combine(Utility.kRawDataPrefix, Path.ChangeExtension(modelDataRes, Utility.kRawDataExtension)), out modelData);
#else
                modelData = AssetLoader.instance.LoadRes(modelDataRes, typeof(DModelData)).obj as DModelData;
#endif
                if (null != modelData)
                {
                    SetBlockLayer(false);
                    m_nBlockWidth = modelData.blockGridChunk.gridWidth;
                    m_nBlockHeight = modelData.blockGridChunk.gridHeight;
                    m_byteBlockData = new byte[modelData.blockGridChunk.gridBlockData.Length];
                    Array.Copy(modelData.blockGridChunk.gridBlockData,m_byteBlockData, modelData.blockGridChunk.gridBlockData.Length);
                    SetBlockLayer(true);
                }
                else
                {
                    Logger.LogErrorFormat("the blockPath {0} is not exist", modelDataRes);
                }
            }
        }
    }
}
