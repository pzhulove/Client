using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * 机制描述:召唤师召唤兽存在时间增加50%
召唤师召唤兽存在时间增加50%
*/

public class Mechanism2 : BeMechanism
{
    int addFixTime;
    int addPercentTime;
    List<int> specifyMonsterIDs = new List<int>();
    bool relateByLevel = false;     //时间随等级成长

    public Mechanism2(int mid, int lv) : base(mid, lv) { }

    public override void OnReset()
    {
        addFixTime = 0;
        addPercentTime = 0;
        specifyMonsterIDs.Clear();
        relateByLevel = false;
    }
    public override void OnInit()
    {
        addFixTime = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        addPercentTime = TableManager.GetValueFromUnionCell(data.ValueB[0], level);

        for (int i = 0; i < data.ValueC.Count; ++i)
        {
            var mid = TableManager.GetValueFromUnionCell(data.ValueC[i], level);

            mid = BeUtility.GetOnlyMonsterID(mid);
            if (mid > 0)
                specifyMonsterIDs.Add(mid);
        }
        if (data.ValueD.Count > 0 && TableManager.GetValueFromUnionCell(data.ValueD[0],level) == 1)
            relateByLevel = true;
    }

    public override void OnStart()
    {
        if (owner != null)
        {
            handleA = owner.RegisterEventNew(BeEventType.onChangeSummonLifeTime, (args) =>
            {
                BeActor summon = args.m_Obj as BeActor;
                if (summon != null)
                {
                    if (specifyMonsterIDs.Count <= 0 || specifyMonsterIDs.Contains(summon.GetEntityData().simpleMonsterID))
                    {

                        args.m_Int += relateByLevel ? level * addFixTime : addFixTime;
                        args.m_Int2 += relateByLevel ? level * addPercentTime : addPercentTime;
                    }
                }
            });
        }
    }
}
