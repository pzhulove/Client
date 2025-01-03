using System;
using System.Collections.Generic;
using UnityEngine;

//增加或者减少触发效果表ID的浮空力大小（固定值和千分比）
class Mechanism106 : BeMechanism
{
    int effectId;
    int yForce;
    int yForceRate;
    int yForceFloating;
    int yForceFloatingRate;
    
    protected List<int> targetList = new List<int>();
    protected List<GameClient.BeEvent.BeEventHandleNew> handleList = new List<GameClient.BeEvent.BeEventHandleNew>();

    public Mechanism106(int mid, int lv) : base(mid, lv) { }

    public override void OnReset()
    {
        targetList.Clear();
        handleList.Clear();
    }
    public override void OnInit()
    {
        effectId = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        yForce = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        yForceRate = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
        yForceFloating = TableManager.GetValueFromUnionCell(data.ValueD[0], level);
        yForceFloatingRate = TableManager.GetValueFromUnionCell(data.ValueE[0], level);
    }

    public override void OnStart()
    {
        handleA = OwnerRegisterEventNew(BeEventType.onHitOther, args =>
        //handleA = owner.RegisterEvent(BeEventType.onHitOther, args =>
        {
            var target = args.m_Obj as BeActor;
            var hurtid = args.m_Int;
            if (target != null && !targetList.Contains(target.GetPID()) && effectId == hurtid)
            {
                targetList.Add(target.GetPID());

                var handle1 = target.RegisterEventNew(BeEventType.onChangeFloatingRate, (GameClient.BeEvent.BeEventParam param) =>
                {
                    //int id = (int)args1[0];
                    if (effectId == param.m_Vint3.x)
                    {
                        //var yForceArray = (int[])args1[1];
                        //yForceArray[0] += yForce;
                        //yForceArray[1] += yForceRate;
                        param.m_Vint3.y += yForce;
                        param.m_Vint3.z += yForceRate;
                    }
                });
                handleList.Add(handle1);

                var handle2 = target.RegisterEventNew(BeEventType.onChangeFloatYForce, (GameClient.BeEvent.BeEventParam param) =>
                {
                    //int id = (int)args2[0];
                    if (effectId == param.m_Vint3.x)
                    {
                        //var yForceFloatingArray = (int[])args2[1];
                        //yForceFloatingArray[0] += yForceFloating;
                        //yForceFloatingArray[1] += yForceFloatingRate;
                        param.m_Vint3.y += yForceFloating;
                        param.m_Vint3.z += yForceFloatingRate;
                    }
                });
                handleList.Add(handle2);
            }
        });
    }

    public override void OnFinish()
    {
        targetList.Clear();
        RemoveHandle();
    }

    protected void RemoveHandle()
    {
        for (int i = 0; i < handleList.Count; i++)
        {
            handleList[i].Remove();
            handleList[i] = null;
        }
        handleList.Clear();
    }
}