using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//式神 殇 特殊机制
public class Mechanism2001 : BeMechanism
{
    public Mechanism2001(int mid, int lv) : base(mid, lv) { }

    protected int effectId = 0;
    protected List<int> radiusList = new List<int>();    //距离范围
    protected List<int> addXForceList = new List<int>();    //对应的范围添加的X轴推力 
    protected List<int> addFloatXForceList = new List<int>();    //对应的范围添加的X轴推力(浮空状态)

    protected List<int> targetList = new List<int>();
    protected List<GameClient.BeEvent.BeEventHandleNew> handleList = new List<GameClient.BeEvent.BeEventHandleNew>();

    public override void OnInit()
    {
        base.OnInit();
        effectId = TableManager.GetValueFromUnionCell(data.ValueA[0],level);

        for (int i = 0; i < data.ValueB.Count; i++)
        {
            radiusList.Add(TableManager.GetValueFromUnionCell(data.ValueB[i], level));
        }

        for (int i = 0; i < data.ValueC.Count; i++)
        {
            addXForceList.Add(TableManager.GetValueFromUnionCell(data.ValueC[i], level));
        }

        for(int i = 0; i < data.ValueD.Count; i++)
        {
            addFloatXForceList.Add(TableManager.GetValueFromUnionCell(data.ValueD[i],level));
        }

    }

    public override void OnReset()
    {
        radiusList.Clear();
        addXForceList.Clear();
        addFloatXForceList.Clear();
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
            if (target != null && !targetList.Contains(target.GetPID()) && effectId == hurtid)
            {
                targetList.Add(target.GetPID());

                var handle1 = target.RegisterEventNew(BeEventType.onChangeXRate, (GameClient.BeEvent.BeEventParam param) =>
                {
                    if(effectId == param.m_Vint3.x)
                    {
                        param.m_Vint3.y = GetXForce(target, false);
                    }
                    //int id = (int)args1[0];
                    //if (effectId == id)
                    //{
                    //    var xForceArray = (int[])args1[1];
                    //    xForceArray[0] = GetXForce(target,false);
                    //}
                });
                handleList.Add(handle1);

                var handle2 = target.RegisterEventNew(BeEventType.onChangeFloatXForce, (GameClient.BeEvent.BeEventParam param) =>
                {
                    if(effectId == param.m_Vint3.x)
                    {
                        param.m_Vint3.y = GetXForce(target, true);
                    }
                    //int id = (int)args2[0];
                    //if (effectId == id)
                    //{
                    //    var xForceFloatingArray = (int[])args2[1];
                    //    xForceFloatingArray[0] = GetXForce(target,true);
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

    //获取对应距离的X轴推力
    protected int GetXForce(BeEntity pkEntity,bool isFloat)
    {
        int xForce = 0;
        if (pkEntity == null)
            return xForce;
        int dis = (pkEntity.GetPosition() - owner.GetPosition()).magnitude;
        int disAbs = Mathf.Abs(dis);
        for (int i=0;i< radiusList.Count; i++)
        {
            int front = i == 0 ? 0 : radiusList[i - 1] * GlobalLogic.VALUE_10;
            if (disAbs > front && disAbs <= radiusList[i] * GlobalLogic.VALUE_10)
            {
                if (!isFloat)
                    xForce = addXForceList[i];
                else
                    xForce = addFloatXForceList[i];
            }
        }
        return dis > 0 ? xForce : -xForce;
    }
}
