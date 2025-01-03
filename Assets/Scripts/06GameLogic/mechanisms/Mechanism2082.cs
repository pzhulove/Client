using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameClient;

/*
 * 机制描述:当自身拥有指定机制时，减少指定异常状态（BUFF表类型）伤害（千分比）；
 * 
 * 注意：该机制的减免效果是叠乘算法，如：100血的伤害，被减免2次50%还会有25点伤害（100*0.5*0.5）
*/

public class Mechanism2082 : BeMechanism
{
    private Dictionary<int, CrypticInt32> mBuffDic = new Dictionary<int, CrypticInt32>();

    public Mechanism2082(int mid, int lv):base(mid, lv)
    {
    }

    public override void OnInit()
    {
        if (data.ValueA.Count != data.ValueB.Count)
        {
            return;
        }

        mBuffDic.Clear();
        for (int i = 0; i < data.ValueA.Count; i++)
        {
            int buffType = TableManager.GetValueFromUnionCell(data.ValueA[i], level);
            int precent = TableManager.GetValueFromUnionCell(data.ValueB[i], level);
            
            mBuffDic.Add(buffType, precent);
        }
    }

    public override void OnReset()
    {
        mBuffDic.Clear();
    }

    public override void OnStart ()
    {
        if (owner == null)
            return;
        
        handleA = owner.RegisterEventNew(BeEventType.AbnormalBuffHurt, args =>
        {
            BeBuff buff = (BeBuff) args.m_Obj;

            int percent = GetBuffDamagePercent(buff);
            if (percent < 0)
                return;
            
            int damage = args.m_Int;
            damage = damage * (VFactor.one - VFactor.NewVFactor(percent, GlobalLogic.VALUE_1000));
            args.m_Int = damage;
        });
    }

    CrypticInt32 GetBuffDamagePercent(BeBuff buff)
    {
        for (int i = 0; i < buff.buffData.StateChange.Count; i++)
        {
            int state = buff.buffData.StateChange[i];

            if (mBuffDic.ContainsKey(state))
            {
                return mBuffDic[state];
            }
        }

        return -1;
    }
}