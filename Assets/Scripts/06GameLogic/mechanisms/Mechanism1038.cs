using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 暴击别人根据概率从Buff信息列表中选择一个添加
/// </summary>
public class Mechanism1038 : BeMechanism
{
    public struct AddBuffInfoData
    {
        public int buffInfoId;  //添加的BuffInfoId
        public int addRate; //添加的概率
        public int priorityLevel;   //优先级
    }

    public Mechanism1038(int id, int level) : base(id, level) { }

    private List<AddBuffInfoData> buffInfoList = new List<AddBuffInfoData>();

    public override void OnInit()
    {
        base.OnInit();
        for(int i = 0; i < data.ValueA.Count; i++)
        {
            AddBuffInfoData buffInfoData = new AddBuffInfoData();
            buffInfoData.buffInfoId = TableManager.GetValueFromUnionCell(data.ValueA[i],level);
            buffInfoData.addRate = TableManager.GetValueFromUnionCell(data.ValueB[i], level);
            buffInfoData.priorityLevel = TableManager.GetValueFromUnionCell(data.ValueC[i], level);
            buffInfoList.Add(buffInfoData);
        }
    }

    public override void OnReset()
    {
        buffInfoList.Clear();
    }

    public override void OnStart()
    {
        base.OnStart();
        handleA = owner.RegisterEventNew(BeEventType.onHitCriticalBeforDamage, args =>
        //handleA = owner.RegisterEvent(BeEventType.onHitCriticalBeforDamage, (object[] args) =>
        {
            AddBuffInfo();
        });
    }

    private void AddBuffInfo()
    {
        int addBuffInfoId = 0;
        int addPriority = -1;
        for (int i = 0; i < buffInfoList.Count; i++)
        {
            AddBuffInfoData buffInfoData = buffInfoList[i];
            int rate = FrameRandom.Random((uint)GlobalLogic.VALUE_1000);
            if (buffInfoData.addRate >= rate && buffInfoData.priorityLevel > addPriority)
            {
                addBuffInfoId = buffInfoData.buffInfoId;
                addPriority = buffInfoData.priorityLevel;
            }
        }
        if (addBuffInfoId != 0)
            owner.buffController.TryAddBuffInfo(addBuffInfoId, owner, level);
    }
}
