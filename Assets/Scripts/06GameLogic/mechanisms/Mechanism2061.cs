using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//召唤计数机制，统计summon数量
public class Mechanism2061 : BeMechanism
{
    private string m_MonsterInfoPath = "UIFlatten/Prefabs/BattleUI/DungeonMonsterInfo";
    private string m_InfoTextString = "";

#if !SERVER_LOGIC

    private GameObject m_MonsterInfoPrefab = null;
    private UnityEngine.UI.Text m_InfoText = null;
#endif

    private int mSummonMaxNum;
    private int mMonsterAID;
    private int mSkillBuffID;

    private int mMonsterID;

    private int mSummonNum = 0;
    
    private bool triggerEventFlag = false;

    private IBeEventHandle mSummonEventHandle = null;
    private IBeEventHandle mKillEventHandle = null;

    private int showMonsterInfo = 0;

    private int mRemoveBuffInfoID;

    private int clearEntityResID;

    private int clearMonsterTag = 0;

    public Mechanism2061(int mid, int lv) : base(mid, lv)
    {
    }

    public override void OnInit()
    {
        m_InfoTextString = data.StringValueA[0];

        mSummonMaxNum = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        mMonsterAID = TableManager.GetValueFromUnionCell(data.ValueA[1], level);
        mSkillBuffID = TableManager.GetValueFromUnionCell(data.ValueA[2], level);

        mMonsterID = TableManager.GetValueFromUnionCell(data.ValueB[0], level);

        showMonsterInfo = TableManager.GetValueFromUnionCell(data.ValueC[0], level);

        mRemoveBuffInfoID = TableManager.GetValueFromUnionCell(data.ValueD[0], level);

        clearEntityResID = TableManager.GetValueFromUnionCell(data.ValueE[0], level);
        clearMonsterTag = TableManager.GetValueFromUnionCell(data.ValueF[0], level);
    }

    public override void OnReset()
    {
#if !SERVER_LOGIC
        m_MonsterInfoPrefab = null;
        m_InfoText = null;
#endif

        mSummonNum = 0;
        triggerEventFlag = false;

        Reset();
}

public override void OnStart()
    {
        Reset();

        if(owner != null)
        {
            mSummonEventHandle = owner.RegisterEventNew(BeEventType.onSummon, (args) => 
            {
                BeActor monster = args.m_Obj as BeActor;
                if(monster != null && monster.GetEntityData().MonsterIDEqual(mMonsterID))
                {
                    mSummonNum++;

                    if(mSummonNum >= mSummonMaxNum && !triggerEventFlag)
                    {
                        ShowHeadDialog(string.Empty, true);

                        //执行指挥操作并且清除所有召唤物
                        BeUtility.CancelCurrentSkill(owner);

                        BeUtility.ForceMonsterUseSkill(mMonsterAID, mSkillBuffID, owner);

                        ClearEntity(clearEntityResID);

                        if (clearMonsterTag == 0)
                        {
                            BeUtility.DoMonsterDeadById(mMonsterID, owner);
                        }

                        owner.buffController.RemoveBuffByBuffInfoID(mRemoveBuffInfoID);

                        ClearEventHandle();

                        triggerEventFlag = true;
                    }

                    if (!triggerEventFlag && mSummonNum < mSummonMaxNum)
                    {
                        ShowHeadDialog(string.Format(m_InfoTextString, (mSummonMaxNum - mSummonNum)));
                    }
                }
            });

            mKillEventHandle = owner.CurrentBeScene.RegisterEventNew(BeEventSceneType.onMonsterDead, (args) => 
            {
                var monster = args.m_Obj as BeActor;
                if(monster != null && monster.GetEntityData().MonsterIDEqual(mMonsterID))
                {
                    mSummonNum--;
                }
            });
        }
    }

    public override void OnFinish()
    {
        Reset();
    }

    private void Reset()
    {
        mSummonNum = 0;
        triggerEventFlag = false;
        ClearEventHandle();
        ShowHeadDialog(string.Empty, true);
    }

    private void ClearEventHandle()
    {
        if(mSummonEventHandle != null)
        {
            mSummonEventHandle.Remove();
            mSummonEventHandle = null;
        }
        if(mKillEventHandle != null)
        {
            mKillEventHandle.Remove();
            mKillEventHandle = null;
        }
    }

    private void ClearEntity(int resID)
    {
        if (owner != null)
        {
            List<BeEntity> entitysList = GamePool.ListPool<BeEntity>.Get();
            owner.CurrentBeScene.GetEntitysByResId(entitysList, resID);
            for (int i = 0; i < entitysList.Count; ++i)
            {
                var entity = entitysList[i] as BeProjectile;
                if (entity == null || entity.IsDead())
                    continue;
                entity.DoDie();
            }
            GamePool.ListPool<BeEntity>.Release(entitysList);
        }
    }

    private void ShowHeadDialog(string text,bool hide = false)
    {
#if !SERVER_LOGIC
        if(owner != null && owner.m_pkGeActor != null)
        {
            owner.m_pkGeActor.ShowHeadDialog(text, hide);
        }
#endif
    }
}
