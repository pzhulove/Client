using System;
using System.Collections.Generic;
using UnityEngine;
public class Mechanism159 : BeMechanism
{
    protected int damagePercent = 0;                                //伤害千分比
    protected List<int> buffInfoIdList = new List<int>();       //伤害升高到千分比以上时添加BuffInfo
    protected int damageValue = 0;
    protected VFactor damagePercentFactor = VFactor.zero;           //条件血量千分比
    protected string textInfo = string.Empty;
    protected float textLastTime = 0;
    public Mechanism159(int mid, int lv) : base(mid, lv) { }
    
    public override void OnReset()
    {
        buffInfoIdList.Clear();
        damageValue = 0;
        textInfo = string.Empty;
        textLastTime = 0;
    }
    public override void OnInit()
    {
        damagePercent = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        
        for (int i = 0; i < data.ValueB.Count; i++)
        {
            buffInfoIdList.Add(TableManager.GetValueFromUnionCell(data.ValueB[i], level));
        }
        textLastTime = TableManager.GetValueFromUnionCell(data.ValueC[0], level) / 1000.0f;
        if (data.StringValueA.Length > 0)
        {
            textInfo = data.StringValueA[0];
        }
    }
    public override void OnStart()
    {
        base.OnStart();
        damagePercentFactor = new VFactor(damagePercent, GlobalLogic.VALUE_1000);

        handleA = owner.RegisterEventNew(BeEventType.onHurt,  args =>
        //handleA = owner.RegisterEvent(BeEventType.onHurt, (object[] args) =>
        {
            _CheckDamage(args.m_Int);
        });
        handleB = owner.RegisterEventNew(BeEventType.OnBuffDamage, args =>
        {
            _CheckDamage(args.m_Int);
        });
    }
    private void _CheckDamage(int hurtValue)
    {
        int maxHp = owner.GetEntityData().GetMaxHP();
        damageValue += hurtValue;
        VFactor curPercent = new VFactor(damageValue, maxHp);
        if (curPercent > damagePercentFactor)
        {
            AddBuffInfo();
            damageValue = 0;

#if !LOGIC_SERVER
            if (textInfo.Length > 0)
            {
                GameClient.SystemNotifyManager.SysDungeonSkillTip(textInfo, textLastTime);
            }
#endif
        }
    }
    private void AddBuffInfo()
    {
        for (int i = 0; i < buffInfoIdList.Count; i++)
        {
             owner.buffController.TryAddBuff(buffInfoIdList[i]);
        }
    }
}
