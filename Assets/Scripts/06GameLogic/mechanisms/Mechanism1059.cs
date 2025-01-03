using System;
using System.Collections.Generic;
using GameClient;

//当角色某个属性（力量、智力、体力、精神；四选一）大于X点时；每增加X点属性，则给自己添加一个BUFFID（）；
public class Mechanism1059 : BeMechanism
{
    int attrTypeIndex = 0;//(0:力量、1:智力、2:体力、3:精神)
    VInt attrValThreshold = VInt.zero;
    VInt attrValAddVal = VInt.zero;
    int buffId = 0;
    private readonly AttributeType[] attrTypes = new AttributeType[] { AttributeType.baseAtk, AttributeType.baseInt, AttributeType.baseSta, AttributeType.baseSpr};
    public Mechanism1059(int sid, int skillLevel) : base(sid, skillLevel)
    {

    }
    public override void OnInit()
    {
        base.OnInit();
        attrTypeIndex = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        attrValThreshold = VInt.NewVInt(TableManager.GetValueFromUnionCell(data.ValueB[0], level),GlobalLogic.VALUE_10);
        attrValAddVal = VInt.NewVInt(TableManager.GetValueFromUnionCell(data.ValueC[0], level),GlobalLogic.VALUE_10);
        buffId = TableManager.GetValueFromUnionCell(data.ValueD[0], level);

    }
    public override void OnStart()
    {
        base.OnStart();
        if (owner == null ||
            owner.buffController == null ||
            owner.GetEntityData() == null || 
            owner.GetEntityData().battleData == null) return;
        CheckAndDoBuff();
        handleA = owner.RegisterEventNew(BeEventType.onAttrChange, onAttributeChange);
    }

    public override void OnFinish()
    {
        base.OnFinish();
        RemoveBuff();
    }

    private void onAttributeChange(BeEvent.BeEventParam args)
    {
        var changedAttrType = (AttributeType)args.m_Int;
        for (int i = 0; i < attrTypes.Length; i++)
        {
            if(attrTypes[i] == changedAttrType)
            {
                if(i == attrTypeIndex)
                {
                    break;
                }
                else
                    return;
            }

        }
        CheckAndDoBuff();
    }
    private void CheckAndDoBuff()
    {
        if (attrTypeIndex < 0 || attrTypeIndex >= attrTypes.Length || attrValAddVal <= 0) return;
        int value = owner.GetEntityData().GetAttributeValue(attrTypes[attrTypeIndex]);
        if(value > attrValThreshold)
        {
            int beyondValue = value - attrValThreshold.i;
            int buffCount = beyondValue / attrValAddVal.i;
            int hasBuffCount = owner.buffController.GetBuffCountByID(buffId);
            int delBuffCount = Math.Abs(buffCount - hasBuffCount);
            if(buffCount > hasBuffCount)
            {
                for(int i = 0; i < delBuffCount;i++)
                {
                    owner.buffController.TryAddBuff(buffId,-1);
                }
            }
            else if(delBuffCount > 0)
            {
                owner.buffController.RemoveBuff(buffId, delBuffCount);
            }
        }
        else
        {
            owner.buffController.RemoveBuff(buffId);
        }
    }

    /// <summary>
    /// 机制结束时移除Buff
    /// </summary>
    private void RemoveBuff()
    {
        owner.buffController.RemoveBuff(buffId);
    }
}

