using GameClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 圣骑士觉醒被动机制
/// </summary>
public class Mechanism1076 : BeMechanism
{
    public Mechanism1076(int id, int level) : base(id, level) { }

    protected int[] buffIdArr = new int[2];     //BuffId(祝福刻印|审判刻印)
    protected int[] maxNumArr = new int[2];     //Buff最大数量(祝福刻印|审判刻印)

    protected ClientFrame frame = null;

    protected GeEffectEx[] m_EffectArr = new GeEffectEx[2];     //特效的Prefab(祝福刻印|审判刻印)

    public override void OnInit()
    {
        base.OnInit();
        buffIdArr[0] = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        buffIdArr[1] = TableManager.GetValueFromUnionCell(data.ValueA[1], level);
        maxNumArr[0] = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        maxNumArr[1] = TableManager.GetValueFromUnionCell(data.ValueB[1], level);
    }

    public override void OnReset()
    {
        frame = null;
        m_EffectArr = new GeEffectEx[2];
    }

    public override void OnStart()
    {
        base.OnStart();
        handleA = owner.RegisterEventNew(BeEventType.onAddBuff, RegisterBuffAdd);
        handleB = owner.RegisterEventNew(BeEventType.onRemoveBuff, RegisterBuffRemove);
        handleC = owner.RegisterEventNew(BeEventType.onDead, RegisterDead);
    }

    public override void OnFinish()
    {
        base.OnFinish();
        HideBuffInfo();
}

    /// <summary>
    /// 监听Buff添加
    /// </summary>
    protected void RegisterBuffAdd(BeEvent.BeEventParam args)
    {
        BeBuff buff = (BeBuff)args.m_Obj;
        if (buff == null)
            return;
        int index = GetBuffIndex(buff.buffID);
        if (index == -1)
            return;
        int buffNum = owner.buffController.GetBuffCountByID(buff.buffID);
        SetBuffInfo(index, buffNum, maxNumArr[0]);
    }

    /// <summary>
    /// 监听Buff移除
    /// </summary>
    protected void RegisterBuffRemove(BeEvent.BeEventParam args)
    {
        int buffId = (int)args.m_Int;
        int index = GetBuffIndex(buffId);
        if (index == -1)
            return;
        int buffNum = owner.buffController.GetBuffCountByID(buffId);
        SetBuffInfo(index, buffNum, maxNumArr[0]);
    }

    /// <summary>
    /// 判断是否是指定的buff
    /// </summary>
    protected int GetBuffIndex(int buffId)
    {
        for(int i=0;i< buffIdArr.Length; i++)
        {
            if (buffId == buffIdArr[i])
                return i;
        }
        return -1;
    }

    private void SetBuffInfo(int index, int curNum, int maxNum)
    {
#if !LOGIC_SERVER
        RefreshEffect(index);
        if (owner != null && owner.isLocalActor)
        {
            var battleUI = BattleUIHelper.CreateBattleUIComponent<BattleUIProfession>();;
            if (battleUI != null)
            {
                battleUI.SetShengQiBeiDongBuff(index, curNum, maxNum);
            }
        }
#endif
    }

    private void HideBuffInfo()
    {
#if !LOGIC_SERVER
        if (owner != null && owner.isLocalActor)
        {
            var battleUI = BattleUIHelper.CreateBattleUIComponent<BattleUIProfession>();;
            if (battleUI != null)
            {
                battleUI.SetShengQiBeiDongBuff(0, 0, 0);
            }
        }
#endif
    }


    /// <summary>
    /// 刷新身上的特效
    /// </summary>
    private void RefreshEffect(int index)
    {
#if !LOGIC_SERVER
        if (index == -1)
            return;
        if (m_EffectArr[index] == null)
        {
            int effectInfoId = index == 0 ? 1005 : 1006;
            //更改特效类型防止特效在技能结束的时候被清除
            var effect = owner.m_pkGeActor.CreateEffect(effectInfoId, Vec3.zero, false, 0, EffectTimeType.BUFF);
            if (effect != null)
            {
                m_EffectArr[index] = effect;
            }
        }

        for (int i = 0; i < m_EffectArr.Length; i++)
        {
            if (m_EffectArr[i] == null)
                continue;
            m_EffectArr[i].SetVisible(i == index);
        }
#endif
    }

    /// <summary>
    /// 监听自己死亡
    /// </summary>
    protected void RegisterDead(BeEvent.BeEventParam eventParam)
    {
        owner.buffController.RemoveBuff(buffIdArr[0]);
        owner.buffController.RemoveBuff(buffIdArr[1]);
    }
}
