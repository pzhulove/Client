using GameClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//击杀计数机制，统计怪物死亡数量
public class Mechanism2060 : BeMechanism
{
    private int mKillSummonMaxNum;
    private int mMonsterBID;
    private int mSkillBuffID;

    private int mMonsterID;

    private int killNum = 0;

    private IBeEventHandle mKillEventHandle = null;

    private int showHitCount = 0;

    private int mRemoveBuffInfoID;

    private bool triggerEventFlag = false;

    private int clearEntityResID;

    private int clearMonsterTag = 0;

    public Mechanism2060(int mid, int lv) : base(mid, lv)
    {
    }

    public override void OnInit()
    {
        mKillSummonMaxNum = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        mMonsterBID = TableManager.GetValueFromUnionCell(data.ValueA[1], level);
        mSkillBuffID = TableManager.GetValueFromUnionCell(data.ValueA[2], level);

        mMonsterID = TableManager.GetValueFromUnionCell(data.ValueB[0], level);

        showHitCount = TableManager.GetValueFromUnionCell(data.ValueC[0], level);

        mRemoveBuffInfoID = TableManager.GetValueFromUnionCell(data.ValueD[0], level);

        clearEntityResID = TableManager.GetValueFromUnionCell(data.ValueE[0], level);

        clearMonsterTag = TableManager.GetValueFromUnionCell(data.ValueF[0], level);
    }

    public override void OnReset()
    {
        killNum = 0;
        triggerEventFlag = false;
        ClearEventHandle();
    }

    public override void OnStart()
    {
        Reset();
        SetKillInfoText();
        if(owner != null)
        {
            mKillEventHandle = owner.CurrentBeScene.RegisterEventNew(BeEventSceneType.onMonsterDead,(args)=> 
            {
                var monster = args.m_Obj as BeActor;
                if(monster != null && monster.GetEntityData().MonsterIDEqual(mMonsterID))
                {
                    killNum++;
                    if (killNum >= mKillSummonMaxNum && !triggerEventFlag) 
                    {
                        BeUtility.CancelCurrentSkill(owner);

                        BeUtility.ForceMonsterUseSkill(mMonsterBID, mSkillBuffID, owner);

                        ClearEntity(clearEntityResID);

                        if (clearMonsterTag == 0)
                        {
                            BeUtility.DoMonsterDeadById(mMonsterID, owner);
                        }

                        owner.buffController.RemoveBuffByBuffInfoID(mRemoveBuffInfoID);

                        ClearEventHandle();

                        triggerEventFlag = true;
                    }

                    if(!triggerEventFlag && killNum < mKillSummonMaxNum)
                    {
                        SetKillInfoText();
                    }
                }
            });
        }
    }

    public override void OnFinish()
    {
        Reset();
        ClearKillInfoText();
    }

    private void Reset()
    {
        killNum = 0;
        triggerEventFlag = false;
        ClearEventHandle();
    }

    private void ClearEventHandle()
    {
        if(mKillEventHandle != null)
        {
            mKillEventHandle.Remove();
            mKillEventHandle = null;
        }
    }

    TeamDungeonBattleFrame RaidFrame;
    private void SetKillInfoText()
    {
#if !SERVER_LOGIC
        if (RaidFrame == null)
        {
            RaidFrame = ClientSystemManager.instance.OpenFrame<TeamDungeonBattleFrame>(FrameLayer.Middle) as TeamDungeonBattleFrame;
        }

        if (RaidFrame != null)
        {
            RaidFrame.SetNoTimeLimitKillNum(killNum, mKillSummonMaxNum);
        }
#endif
    }

    private void ClearKillInfoText()
    {
#if !SERVER_LOGIC
        if (RaidFrame != null)
        {
            RaidFrame.Close();
            RaidFrame = null;
        }
#endif
    }

    private void ClearEntity(int resID)
    {
        if(owner != null)
        {
            List<BeEntity> entitysList = GamePool.ListPool<BeEntity>.Get();
            owner.CurrentBeScene.GetEntitysByResId(entitysList, resID);
            for(int i = 0; i < entitysList.Count; ++i)
            {
                var entity = entitysList[i] as BeProjectile;
                if (entity == null || entity.IsDead())
                    continue;
                entity.DoDie();
            }
            GamePool.ListPool<BeEntity>.Release(entitysList);
        }
    }
}
