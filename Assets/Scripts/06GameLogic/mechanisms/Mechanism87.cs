using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 死亡给别人加buff
/// </summary>
public class Mechanism87 : BeMechanism
{


    private int monsterID = 0;

    private int buffInfoID = 0;
    private int batiInfoID = 0;
    private  List<BeActor> monsterList = new List<BeActor>();
    private List<IBeEventHandle> handleList = new List<IBeEventHandle>();
    public Mechanism87(int id, int level) : base(id, level){}

    public override void OnReset()
    {
        monsterList.Clear();
        handleList.Clear();
    }
    public override void OnInit()
    {
        base.OnInit();

        monsterID = TableManager.GetValueFromUnionCell(data.ValueA[0], level);

        buffInfoID = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        batiInfoID = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
    }

    public override void OnStart()
    {
        base.OnStart();
        owner.CurrentBeScene.FindMonsterByID(monsterList, monsterID);
        for (int i = 0; i < monsterList.Count; i++)
        {
            var beHandle = monsterList[i].RegisterEventNew(BeEventType.onAfterDead, (args) => 
            {
                AddBuff();
            });
            handleList.Add(beHandle);
        }
    }

    private void AddBuff()
    {
        owner.CurrentBeScene.FindMonsterByID(monsterList, monsterID);
        if (monsterList.Count == 1)
        {
            monsterList[0].buffController.TryAddBuff(batiInfoID);
        }
        else
        {
            for (int i = 0; i < monsterList.Count; i++)
            {
                monsterList[i].buffController.TryAddBuff(buffInfoID);
            }
        }
    }

    public override void OnFinish()
    {
        base.OnFinish();
        for (int i = 0; i < handleList.Count; i++)
        {
            if (handleList[i] != null)
            {
                handleList[i].Remove();
                handleList[i] = null;
            }           
        }
        handleList.Clear();
    }

}
