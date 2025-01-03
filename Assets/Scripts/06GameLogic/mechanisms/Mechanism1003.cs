using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//增加或者减少触发效果表ID的X轴推力大小（固定值和千分比）
public class Mechanism1003 : BeMechanism
{
    public Mechanism1003(int mid, int lv) : base(mid, lv){ }

    protected List<int> effectIdList = new List<int>();
    protected int xForce;
    protected int xForceRate;
    protected int xForceFloating;
    protected int xForceFloatingRate;

    protected List<int> targetList = new List<int>();
    protected List<GameClient.BeEvent.BeEventHandleNew> handleList = new List<GameClient.BeEvent.BeEventHandleNew>();

    public override void OnInit()
    {
        for(int i = 0; i < data.ValueA.Count; i++)
        {
            effectIdList.Add(TableManager.GetValueFromUnionCell(data.ValueA[i], level));
        }
        xForce = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        xForceRate = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
        xForceFloating = TableManager.GetValueFromUnionCell(data.ValueD[0], level);
        xForceFloatingRate = TableManager.GetValueFromUnionCell(data.ValueE[0], level);
    }

    public override void OnReset()
    {
        effectIdList.Clear();
        targetList.Clear();
        RemoveHandle();
    }

    public override void OnStart()
    {
        handleA = OwnerRegisterEventNew(BeEventType.onHitOther, args =>
        //handleA = owner.RegisterEvent(BeEventType.onHitOther, args =>
        {
            var target = args.m_Obj as BeActor;
            var hurtid = args.m_Int;
            if (target != null && !targetList.Contains(target.GetPID()) && effectIdList.Contains(hurtid))
            {
                targetList.Add(target.GetPID());

                var handle1 = target.RegisterEventNew(BeEventType.onChangeXRate, (GameClient.BeEvent.BeEventParam param) =>
                {
                    int id = param.m_Vint3.x;
                    if (effectIdList.Contains(id))
                    {
                        param.m_Vint3.y += xForce;
                        param.m_Vint3.z += xForceRate;
                    }
                    //int id = (int)args1[0];
                    //if (effectIdList.Contains(id))
                    //{
                    //    var xForceArray = (int[])args1[1];
                    //    xForceArray[0] += xForce;
                    //    xForceArray[1] += xForceRate;
                    //}
                });
                handleList.Add(handle1);

                var handle2 = target.RegisterEventNew(BeEventType.onChangeFloatXForce, (GameClient.BeEvent.BeEventParam param) =>
                {
                    if (effectIdList.Contains(param.m_Vint3.x))
                    {
                        param.m_Vint3.y += xForceFloating;
                        param.m_Vint3.z += xForceFloatingRate;
                    }
                    //int id = (int)args2[0];
                    //if (effectIdList.Contains(id))
                    //{
                    //    var xForceFloatingArray = (int[])args2[1];
                    //    xForceFloatingArray[0] += xForceFloating;
                    //    xForceFloatingArray[1] += xForceFloatingRate;
                    //}
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
