using System.Collections;
using System.Collections.Generic;
using UnityEngine;
///////删除linq
using GameClient;

// 戮蛊 旋转喷射毒液
class Skill5642 : BeSkill
{
    Mechanism52 mechanism52;
    IBeEventHandle handle1;

    int rNumber;                //旋转圈数
    int[] timeArray;            //每圈旋转时间
    int attackId;
    int mechanismId;
    bool executeNext = true;

    public Skill5642(int sid, int skillLevel) : base(sid, skillLevel) { }

    public override void OnInit()
    {
        rNumber = TableManager.GetValueFromUnionCell(skillData.ValueA[0], level);
        timeArray = new int[rNumber];
        for (int i = 0; i < rNumber; i++)
        {
            timeArray[i] = TableManager.GetValueFromUnionCell(skillData.ValueB[i], level);
        }
        attackId = 56422;
        mechanismId = 1089;
    }

    public override void OnStart()
    {
        handle1 = owner.RegisterEventNew(BeEventType.onPreSetSkillAction, (GameClient.BeEvent.BeEventParam param) =>
        {
            var array = GetNewPhaseArray(rNumber);
            owner.skillController.SetCurrentSkillPhases(array);
        });
    }

    private int[] GetNewPhaseArray(int number)
    {
        if (number < 1) number = 1;
        int[] newPhaseArray = new int[5 + (number - 1) * 4];
        newPhaseArray[0] = 56421;
        newPhaseArray[1] = 56422;
        newPhaseArray[2] = 56423;
        newPhaseArray[3] = 56424;
        newPhaseArray[4] = 56425;
        int[] tempArray = new int[] { 56426, 56423, 56424, 56425 };

        int index = 5;
        for (int i = 1; i < number; i++)
        {
            for (int j = 0; j < tempArray.Length; j++)
            {
                newPhaseArray[index++] = tempArray[j];
            }
        }

        return newPhaseArray;
    }

    public override void OnEnterPhase(int phase)
    {
        curPhase = phase;

        if (phase == 4)
        {
            mechanism52 = owner.AddMechanism(mechanismId, level) as Mechanism52;
            if (mechanism52 != null)
            {
                mechanism52.Init();
            }
        }

        if (phase % 4 == 0)
        {
            if (mechanism52 != null)
            {
                mechanism52.rTime = timeArray[phase / 4 - 1];
                mechanism52.Start();
            }
            executeNext = false;
        }
    }

    public override void OnUpdate(int iDeltime)
    {
        if (!executeNext)
        {
            if (mechanism52 != null && mechanism52.rotateEnd)
            {
                (owner.GetStateGraph() as BeActorStateGraph).ExecuteNextPhaseSkill();
                executeNext = true;
            }
        }
    }

    public override void OnCancel()
    {
        if (mechanism52 != null)
        {
            mechanism52.Stop();
            owner.RemoveMechanism(mechanism52.mechianismID);
        }

        RemoveHandle();
    }

    public override void OnFinish()
    {
        if (mechanism52 != null)
        {
            mechanism52.Stop();
            owner.RemoveMechanism(mechanism52.mechianismID);
        }

        RemoveHandle();
    }

    void RemoveHandle()
    {
        if (handle1 != null)
        {
            handle1.Remove();
            handle1 = null;
        }
    }
}
