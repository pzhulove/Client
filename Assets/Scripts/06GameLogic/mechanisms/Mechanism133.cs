using System;
using System.Collections.Generic;
using UnityEngine;

//怪物或者角色增加携带技能
class Mechanism133 : BeMechanism
{
    public Mechanism133(int mid, int lv) : base(mid, lv) { }

    protected List<int> skillIdList = new List<int>();
    protected Dictionary<int,int> replaceSkillIdDic = new Dictionary<int, int>();  //替换的技能列表
    protected Dictionary<int, int> backUpSkillDic = new Dictionary<int, int>(); //备份的技能列表

    public override void OnReset()
    {
        skillIdList.Clear();
        replaceSkillIdDic.Clear();
        backUpSkillDic.Clear();
    }
    public override void OnInit()
    {
        for(int i = 0; i < data.ValueA.Count; i++)
        {
            skillIdList.Add(TableManager.GetValueFromUnionCell(data.ValueA[i],level));
        }

        for (int i = 0; i < data.ValueB.Count; i++)
        {
            int replaceId = TableManager.GetValueFromUnionCell(data.ValueB[i], level);
            if (!replaceSkillIdDic.ContainsKey(replaceId))
                replaceSkillIdDic.Add(replaceId, level);
        }
    }

    public override void OnStart()
    {
        base.OnStart();
        AddSkill();
        RecordBackupSkills();
        ReplaceSkills();
    }

    public override void OnFinish()
    {
        base.OnFinish();
        RestoreAddSkills();
        ReplaceSkills(false);
    }

    /// <summary>
    /// 增加技能
    /// </summary>
    private void AddSkill()
    {
        if (skillIdList.Count <= 0)
            return;
        for (int i = 0; i < skillIdList.Count; i++)
        {
            AddSkill(skillIdList[i],level);
        }
        RefreshAISkillList();
    }

    /// <summary>
    /// 恢复增加的技能列表
    /// </summary>
    private void RestoreAddSkills()
    {
        if (skillIdList.Count <= 0)
            return;
        for (int i = 0; i < skillIdList.Count; i++)
        {
            RemoveSkillById(skillIdList[i]);
        }
        RefreshAISkillList();
    }

    /// <summary>
    /// 替换技能列表
    /// </summary>
    private void ReplaceSkills(bool isReplace = true)
    {
        if (replaceSkillIdDic.Count <= 0)
            return;
        Dictionary<int, int> oldData = isReplace ?  backUpSkillDic : replaceSkillIdDic;
        Dictionary<int, int> newData = isReplace ?  replaceSkillIdDic : backUpSkillDic;

        //移除不需要的
        Dictionary<int, int>.Enumerator enumeratorCur = oldData.GetEnumerator();
        while (enumeratorCur.MoveNext())
        {
            int skillId = enumeratorCur.Current.Key;
            if (!newData.ContainsKey(skillId))
            {
                RemoveSkillById(skillId);
            }
        }

        //添加新的
        Dictionary<int, int>.Enumerator enumeratorBackUp = newData.GetEnumerator();
        while (enumeratorBackUp.MoveNext())
        {
            int skillId = enumeratorBackUp.Current.Key;
            int lelve = enumeratorBackUp.Current.Value;
            if (!oldData.ContainsKey(skillId))
            {
                AddSkill(skillId, lelve);
            }
        }
        RefreshAISkillList();
    }

    /// <summary>
    /// 记录备份技能
    /// </summary>
    private void RecordBackupSkills()
    {
        var skills = owner.GetSkills();
        Dictionary<int, BeSkill>.Enumerator enumerator = skills.GetEnumerator();
        while (enumerator.MoveNext())
        {
            int skillId = enumerator.Current.Key;
            BeSkill skill = enumerator.Current.Value;
            if(!backUpSkillDic.ContainsKey(skillId))
                backUpSkillDic.Add(skillId, skill.level);
        }
    }

    /// <summary>
    /// 添加技能
    /// </summary>
    private void AddSkill(int skillId,int skillLevel = 1)
    {
        owner.skillController.LoadOneSkill(skillId, skillLevel);
        BeSkill skill = owner.GetSkill(skillId);
        if (skill != null)
        {
            skill.StartInitCD(BattleMain.IsModePvP(battleType));
        }
    }

    /// <summary>
    /// 移除技能
    /// </summary>
    /// <param name="skillId"></param>
    private void RemoveSkillById(int skillId)
    {
        owner.skillController.RemoveSkill(skillId);
    }

    /// <summary>
    /// 刷新怪物AI技能
    /// </summary>
    private void RefreshAISkillList()
    {
        owner.aiManager.ReloadSkillInfos(owner.GetEntityData().monsterData);
    }
}