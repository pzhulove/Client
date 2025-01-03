using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuffAttachType
{
    NONE = -1,
    EFFECTTABLE = 0,            //触发效果表
    BUFFINFO = 1,               //Buff信息表
}

/// <summary>
/// 更改Buff附加概率
/// </summary>
public class Mechanism93 : BeMechanism
{
    protected BuffAttachType buffAttachType = BuffAttachType.NONE;
    protected List<int> effectOrBuffIdList = new List<int>();
    protected int attachRate = 0;
    
    protected List<int> targetList = new List<int>();
    protected List<IBeEventHandle> handleList = new List<IBeEventHandle>();

    public Mechanism93(int id, int level) : base(id, level) { }
    public override void OnReset()
    {
        buffAttachType = BuffAttachType.NONE;
        effectOrBuffIdList.Clear();
        attachRate = 0;
        targetList.Clear();
        handleList.Clear();
    }
    public override void OnInit()
    {
        buffAttachType = (BuffAttachType)TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        for(int i = 0; i < data.ValueB.Count; i++)
        {
            effectOrBuffIdList.Add(TableManager.GetValueFromUnionCell(data.ValueB[i], level));
        }
        attachRate = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
    }

    public override void OnStart()
    {
        if (buffAttachType == BuffAttachType.EFFECTTABLE)
        {
            handleA = owner.RegisterEventNew(BeEventType.onChangeBuffAttackRate, args =>
            {
                var attachType = (BuffAttachType)args.m_Int;
                int id = args.m_Int2;
                if (attachType == buffAttachType && effectOrBuffIdList.Contains(id))
                {
                    args.m_Int3 = ChangeBuffAttachRate(args.m_Int3);
                }
            });
        }
        else if (buffAttachType == BuffAttachType.BUFFINFO)
        {
            handleA = owner.RegisterEventNew(BeEventType.onBeforeHit, args =>
            //handleA = owner.RegisterEvent(BeEventType.onBeforeHit, args =>
            {
                var target = args.m_Obj as BeActor;
                if (target != null && !targetList.Contains(target.GetPID()))
                {
                    var handle = target.RegisterEventNew(BeEventType.onChangeBuffAttackRate, args1 =>
                    {
                        var attachType = (BuffAttachType)args1.m_Int;
                        int id = args1.m_Int2;
                        var source = (BeActor)args1.m_Obj;
                        if (attachType == buffAttachType && effectOrBuffIdList.Contains(id) && owner.GetPID() == source.GetPID())
                        {
                            args1.m_Int3 = ChangeBuffAttachRate(args1.m_Int3);
                        }
                    });
                    targetList.Add(target.GetPID());
                    handleList.Add(handle);
                }
            });
        }
    }

    protected int ChangeBuffAttachRate(int curRate)
    {
        int attachBuffRate = curRate + attachRate;
        attachBuffRate = attachBuffRate > 1000 ? 1000 : attachBuffRate;
        attachBuffRate = attachBuffRate < 0 ? 0 : attachBuffRate;
        return attachBuffRate;
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
