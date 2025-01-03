using System;
using System.Collections.Generic;
using UnityEngine;

//增加或者减少某个技能阶段的最大持续时间
class Mechanism115 : BeMechanism
{
    int skillPhaseId;
    int time;
    int timeRate;
    int relateByLevel = 0;      //随机制等级成长

    //int monsterId = 0;
    int skillPhaseStartTime = 0;     //技能阶段初始时间
    //BeEventHandle monsterHandle = null;
    public Mechanism115(int mid, int lv) : base(mid, lv) { }

    public override void OnReset()
    {
        relateByLevel = 0;
        skillPhaseStartTime = 0;
    }
    public override void OnInit()
    {
        skillPhaseId = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        time = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        timeRate = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
        if (data.ValueD.Count > 0)
            relateByLevel = TableManager.GetValueFromUnionCell(data.ValueD[0], level);
        if (data.ValueE.Count > 0)
            skillPhaseStartTime = TableManager.GetValueFromUnionCell(data.ValueE[0], level);
    }

    public override void OnStart()
    {
        handleA = owner.RegisterEventNew(BeEventType.onChangeSkillTime, param =>
        {
            //var sId = (int)args[0];
            if (param.m_Vint3.x == skillPhaseId)
            {
                //var timeArray = (int[])args[1];
                if (skillPhaseStartTime > 0)
                    param.m_Vint3.y = skillPhaseStartTime;
                param.m_Vint3.y += relateByLevel == 1 ? time * level : time;
                param.m_Vint3.z += relateByLevel == 1 ? timeRate * level : timeRate;
            }
        });
    }
}