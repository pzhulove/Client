using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
随机重置一个携带技能的CD
 */

public class Mechanism50 : BeMechanism
{
    public enum TargetType
    {
        PET_OWER, //宠物的主人
        OWER,   //自己
    }


    protected TargetType targetType = TargetType.PET_OWER;
    protected int minCD = 0;
    protected int maxCD = 0;
    protected List<int> useSkillIdList = new List<int>();
    protected List<int> skillIdList = new List<int>();

    public Mechanism50(int sid, int skillLevel) : base(sid, skillLevel) { }

    public override void OnReset()
    {
        targetType = TargetType.PET_OWER;
        minCD = 0;
        maxCD = 0;
        useSkillIdList.Clear();
        skillIdList.Clear();
    }
    public override void OnInit()
    {
        var v = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        if (v > 0)
            targetType = TargetType.OWER;
        if (data.ValueB.Count > 0)
        {
            minCD = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        }
        if (data.ValueC.Count > 0)
        {
            maxCD = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
        }

        if (data.ValueD.Count > 0)
        {
            for (int i = 0; i < data.ValueD.Count; i++)
            {
                useSkillIdList.Add(TableManager.GetValueFromUnionCell(data.ValueD[i], level));
            }
        }

        if (data.ValueE.Count > 0)
        {
            for (int i = 0; i < data.ValueE.Count; i++)
            {
                skillIdList.Add(TableManager.GetValueFromUnionCell(data.ValueE[i], level));
            }
        }
    }

    public override void OnStart()
    {
        if (owner == null || owner.IsDead())
            return;

        BeActor actor = owner;
        if (targetType == TargetType.PET_OWER && owner.GetEntityData().isPet)
            actor = owner.GetOwner() as BeActor;

        if (actor == null)
            return;

        if (skillIdList.Count >0)
        {
            //指定ID列表的时候
            if (skillIdList.Count > 0)
            {
                for (int i = 0; i < skillIdList.Count; i++)
                {
                    ResetSkillCd(actor, skillIdList[i]);
                }
            }
        }
        else
        {
            List<int> skillIDs = GamePool.ListPool<int>.Get();
            if (GetCanCooldownSkills(actor, minCD, maxCD, skillIDs))
            {
                if (skillIDs.Count > 0)
                {
                    int randSkillID = FrameRandom.InRange(0, skillIDs.Count);
                    ResetSkillCd(actor, skillIDs[randSkillID]);
                }
            }
            GamePool.ListPool<int>.Release(skillIDs);
            owner.buffController.RemoveBuff(1200522);
        }
    }

    //重置技能cd
    protected void ResetSkillCd(BeActor actor, int skillId)
    {
        var skill = actor.GetSkill(skillId);
        if (skill != null)
        {
            skill.ResetCoolDown();
            owner.m_pkGeActor.CreateHeadText(GameClient.HitTextType.SPECIAL_ATTACK, skill.skillData.Icon,
                false, "UIFlatten/Prefabs/Battle_Digit/PlayerInfo_ShowSkillIcon");
            // Logger.LogErrorFormat("RESET SKILL : {0}", skillIDs[randSkillID]);
        }
    }

    public static bool GetCanCooldownSkills(BeActor actor, int minCD, int maxCD, List<int> skillIDs = null)
    {
        bool ret = false;

        if (actor == null)
            return ret;

        if (skillIDs != null)
            skillIDs.Clear();

        var skillDic = actor.GetEntityData().skillLevelInfo;

        int curUsingSkillID = 0;
        if (actor.sgGetCurrentState() == (int)ActionState.AS_CASTSKILL)
            curUsingSkillID = actor.GetCurSkillID();

        var enumerator = skillDic.GetEnumerator();
        while (enumerator.MoveNext())
        {
            int skillID = enumerator.Current.Key;

            var skill = actor.GetSkill(skillID);
            if (skill != null &&
                skill.skillCategory == 3 &&
                !skill.isBuffSkill &&
                skill.GetCurrentCD() >= minCD &&
                skill.CDLeftTime >= minCD &&
                skillID != curUsingSkillID &&
                (skill.GetCurrentCD() - skill.CDLeftTime) >= maxCD)//职业技能
            {
                if (skillIDs != null)
                    skillIDs.Add(skillID);
                ret = true;
            }
        }

        return ret;
    }

    public static bool GetCanReduceCDSkills(BeActor actor,int minCD, List<int> skillIDs = null)
    {
        bool ret = false;

        if (actor == null)
            return ret;

        if (skillIDs != null)
            skillIDs.Clear();

        var skillDic = actor.GetEntityData().skillLevelInfo;

        int curUsingSkillID = 0;
        if (actor.sgGetCurrentState() == (int)ActionState.AS_CASTSKILL)
            curUsingSkillID = actor.GetCurSkillID();

        int maxCDLeftTimeSkillId = 0;
        int maxCDLeftTime = 0;

        var enumerator = skillDic.GetEnumerator();
        while (enumerator.MoveNext())
        {
            int skillID = enumerator.Current.Key;


            var skill = actor.GetSkill(skillID);
            if (skill != null &&
                (skill.skillCategory == 3 || skill.skillCategory == 4 || skill.skillCategory == 6) &&
                skillID != curUsingSkillID &&
                skill.CDLeftTime > 0 &&
                !skill.isBuffSkill)
            {
                if (skill.CDLeftTime > minCD)
                {
                    if (skillIDs != null)
                        skillIDs.Add(skillID);
                    ret = true;
                }
                else
                {
                    //记录剩余CD时间最长的技能ID
                    if (skill.CDLeftTime > maxCDLeftTime)
                    {
                        maxCDLeftTimeSkillId = skillID;
                        maxCDLeftTime = skill.CDLeftTime;
                        ret = true;
                    }
                }
            }
        }

        //如果没有大于最小时间的技能ID
        if (skillIDs != null && skillIDs.Count <= 0 && maxCDLeftTimeSkillId > 0)
        {
            //用第一个Id是负一标识这种特殊情况
            skillIDs.Add(-1);
            skillIDs.Add(maxCDLeftTimeSkillId);
        }

        return ret;
    }

    public override void OnFinish()
    {
        base.OnFinish();
    }
}