using System;
using System.Collections.Generic;
using UnityEngine;

//当自身拥有某个BUFF时，添加某个BUFF信息ID，当BUFF消失，移除此BUFF信息ID
class Mechanism113 : BeMechanism
{
    int hasBuffId;
    int buffInfoId;

    int savedBuffPID = -1;

    public Mechanism113(int mid, int lv) : base(mid, lv) { }

    public override void OnReset()
    {
        savedBuffPID = -1;
    }
    public override void OnInit()
    {
        hasBuffId = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        buffInfoId = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
    }

    public override void OnStart()
    {
        handleA = owner.RegisterEventNew(BeEventType.onAddBuff, args =>
        {
            var buff = args.m_Obj as BeBuff;
            if (buff.buffID == hasBuffId)
            {
                var savedBuff = owner.buffController.TryAddBuff(buffInfoId, owner);
                if (savedBuff != null)
                {
                    savedBuffPID = savedBuff.PID;
                }
            }
        });

        handleB = owner.RegisterEventNew(BeEventType.onRemoveBuff, args =>
        {
            var buffId = args.m_Int;
            if (buffId == hasBuffId)
            {
                owner.buffController.RemoveBuffByPID(savedBuffPID);
                savedBuffPID = -1;
            }
        });
    }

    public override void OnFinish()
    {
        if (savedBuffPID != -1)
        {
            owner.buffController.RemoveBuffByPID(savedBuffPID);
            savedBuffPID = -1;
        }
    }
}