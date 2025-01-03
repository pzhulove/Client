using System.Collections;
using System.Collections.Generic;
using GameClient;
using UnityEngine;

//抓取不同的对象 设置添加不同的BuffInfo
public class Mechanism1002 : BeMechanism
{
    public Mechanism1002(int mid, int lv) : base(mid, lv){ }

    protected int watchSkillId = 0;     //监听的技能ID
    protected List<int> watchMonsterTypeList = new List<int>();     //监听的怪物类型列表
    protected List<int> addBuffIdList = new List<int>();            //给自己添加不同的BuffId
    protected List<int> addBuffTimeList = new List<int>();          //给自己添加的Buff时间

    public override void OnInit()
    {
        base.OnInit();
        watchSkillId = TableManager.GetValueFromUnionCell(data.ValueA[0],level);
        for(int i = 0; i < data.ValueB.Count; i++)
        {
            watchMonsterTypeList.Add(TableManager.GetValueFromUnionCell(data.ValueB[i], level));
        }

        for(int i = 0; i < data.ValueC.Count; i++)
        {
            addBuffIdList.Add(TableManager.GetValueFromUnionCell(data.ValueC[i], level));
        }

        for(int i = 0; i < data.ValueD.Count; i++)
        {
            addBuffTimeList.Add(TableManager.GetValueFromUnionCell(data.ValueD[i], level));
        }
    }

    public override void OnReset()
    {
        watchMonsterTypeList.Clear();
        addBuffIdList.Clear();
        addBuffTimeList.Clear();
    }

    public override void OnStart()
    {
        base.OnStart();
        handleA = owner.RegisterEventNew(BeEventType.onExcuteGrab, (args) => 
        {
            int skillId = args.m_Int;
            BeActor target = args.m_Obj as BeActor;
            AddBuffId(skillId, target);
        });
    }

    //根据不同的抓取目标添加不同的BuffInfo
    protected void AddBuffId(int skillId,BeActor target)
    {
        if (skillId != watchSkillId)
            return;
        if (target == null)
            return;
        int index = 0;
        int monsterType = 0;
        if (target.GetEntityData().monsterData != null)
        {
            monsterType = (int)target.GetEntityData().monsterData.Type;
            for (int i = 0; i < watchMonsterTypeList.Count; i++)
            {
                int type = watchMonsterTypeList[i];
                if (type == monsterType)
                    index = i;
            }
        }
        BeSkill skill = owner.GetSkill(watchSkillId);
        int skillLevel = 1;
        if (skill != null)
            skillLevel = skill.level;
        owner.buffController.TryAddBuff(addBuffIdList[index], addBuffTimeList[index], skillLevel);
    }
}
