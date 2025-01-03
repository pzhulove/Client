using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 释放状态跳过技能阶段
/// </summary>
public class Mechanism1066 : BeMechanism
{
    public Mechanism1066(int mid, int lv) : base(mid, lv) { }

    private int phase = 0;    //

    List<int> phaseList = new List<int>();

    public override void OnInit()
    {
        for(int i= 0; i < data.ValueA.Count; ++i)
        {
            phaseList.Add(TableManager.GetValueFromUnionCell(data.ValueA[i], level));
        }
    }

    public override void OnReset()
    {
        phase = 0;
        phaseList.Clear();
    }

    public override void OnStart()
    {
        handleA = owner.RegisterEventNew(BeEventType.onNextPhaseBeforeExecute, (GameClient.BeEvent.BeEventParam param) => 
        {
            int skillID = param.m_Int;
            if (phaseList.Contains(skillID) && owner.GetCurrentSkill() != null && owner.GetCurrentSkill().buttonState == GameClient.ButtonState.RELEASE) 
            {
                param.m_Bool = true;
                //bool[] executeNextAgain = (bool[])args[0];
                //executeNextAgain[0] = true;
            }
        });
    }
}
