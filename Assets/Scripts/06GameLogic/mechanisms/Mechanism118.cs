using System.Collections;
using System.Collections.Generic;
using GameClient;
using UnityEngine;

public class Mechanism118 : BeMechanism
{
    protected int checkSkillId = 0;
    protected List<int> replaceComboSkillIdList = new List<int>();
    protected int replaceStartFrame = -1;
    protected int comboSourceId = 0;
    protected List<int> curReplaceList = new List<int>();

    protected bool canComboFlag = false;

    public Mechanism118(int mid, int lv) : base(mid, lv) {}

    public override void OnReset()
    {
        replaceComboSkillIdList.Clear();
        curReplaceList.Clear();
        canComboFlag = false;
    }
    public override void OnInit()
    {
        base.OnInit();
        checkSkillId = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        if (data.ValueB.Count > 0)
        {
            for(int i = 0; i < data.ValueB.Count; i++)
            {
                replaceComboSkillIdList.Add(TableManager.GetValueFromUnionCell(data.ValueB[i], level));
            }
        }
        replaceStartFrame = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
        comboSourceId = TableManager.GetValueFromUnionCell(data.ValueD[0],level);
    }

    public override void OnStart()
    {
        base.OnStart();
        SetComboResourceId();
        handleA = owner.RegisterEventNew(BeEventType.onReplaceComboSkill, (args) =>
        {
            if (!canComboFlag)
                return;
            BeSkill curSkill = owner.GetCurrentSkill();
            if (curSkill == null || curSkill.comboSkillSourceID != comboSourceId)
                return;
            
            if (curReplaceList.Count > 0)
            {
                args.m_Int = curReplaceList[0];
            }
            if (replaceStartFrame > 0)
                args.m_Int2 = replaceStartFrame;
        });

        handleB = owner.RegisterEventNew(BeEventType.OnChangeWeaponEnd, args => 
        {
            SetComboResourceId();
        });

        handleC = owner.RegisterEventNew(BeEventType.onCastSkill, args =>
        {
            int castSkillID = args.m_Int;
            if (castSkillID == checkSkillId)
            {
                curReplaceList.Clear();
                curReplaceList.AddRange(replaceComboSkillIdList); 
                canComboFlag = true;
            }

            if(curReplaceList.Contains(castSkillID))
            {
                curReplaceList.Remove(castSkillID);
            }

            if (curReplaceList.Count <= 0)
            {
                canComboFlag = false;
            }
        });
    }

    protected void SetComboResourceId()
    {
        if (replaceComboSkillIdList.Count <= 0)
            return;
        for (int i = 0; i < replaceComboSkillIdList.Count; i++)
        {
            BeSkill skill = owner.GetSkill(replaceComboSkillIdList[i]);
            if (skill != null)
            {
                skill.comboSkillSourceID = comboSourceId;
            }
        }
    }
}
