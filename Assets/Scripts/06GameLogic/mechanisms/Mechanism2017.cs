using System.Collections.Generic;
using ProtoTable;
using GameClient;

//圣光守护 为自己和队友提供一个护盾
public class Mechanism2017 : BeMechanism
{
    public Mechanism2017(int mid, int lv) : base(mid, lv) { }

    private int hurtMaxLimit = 0;      //承担的伤害上限
    private List<int> skillIdList = new List<int>();    //能被抓取的技能ID列表

    private int totalHurtValue = 0;     //承担的总的伤害数值

    public override void OnInit()
    {
        base.OnInit();
        hurtMaxLimit = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        for (int i = 0; i < data.ValueB.Count; i++)
        {
            skillIdList.Add(TableManager.GetValueFromUnionCell(data.ValueB[i], level));
        }
    }

    public override void OnReset()
    {
        skillIdList.Clear();
        totalHurtValue = 0;
    }

    public override void OnStart()
    {
        base.OnStart();
        SetEquipDataAdd();
        RegisterHurtEvent();
    }

    private void RegisterHurtEvent()
    {
        if (owner == null)
            return;
        handleA = owner.RegisterEventNew(BeEventType.onBeHitAfterFinalDamage, args =>
        //handleA = owner.RegisterEvent(BeEventType.onBeHitAfterFinalDamage, (object[] args) =>
        {
            int hurtId = args.m_Int2;
            EffectTable hurtData = TableManager.instance.GetTableItem<EffectTable>(hurtId);
            //int[] damageArr = (int[])args[0];
            //bool[] absorbDamage = (bool[])args[2];
            AbsorbHurt(args,hurtData);
        });

        handleB = owner.RegisterEventNew(BeEventType.OnJudgeGrab, (args) =>
        {
            int skillId = (int)args.m_Int;
            if (!skillIdList.Contains(skillId))
            {
                args.m_Bool = false;
            }
        });
    }

    //护盾吸收伤害
    private void AbsorbHurt(GameClient.BeEvent.BeEventParam param, EffectTable hurtData)
    {
        if (hurtData.DamageType == EffectTable.eDamageType.MAGIC)
            return;
        if (param.m_Int == 0)
            return;
        totalHurtValue += param.m_Int;
        param.m_Int = 0;
        param.m_Bool = true;
        ShowHitNumber();
        if (totalHurtValue >= hurtMaxLimit)
            RemoveAll();
    }

    //吸收伤害的同时 给自己一个伤害为0的飘字
    private void ShowHitNumber()
    {
#if !LOGIC_SERVER
        var battleUI = BattleUIHelper.GetBattleUIComponent<BattleUICommon>();
        if (battleUI == null)
            return;
        if (battleUI.comShowHit == null)
            return;
        battleUI.comShowHit.ShowHitNumber(0, null, owner.m_iID, owner.GetGePosition(PositionType.OVERHEAD), owner.GetFace(), GameClient.HitTextType.NORMAL, null, owner);
#endif
    }

    //能量吸收满了  盾破碎
    private void RemoveAll()
    {
        //if (attachBuff != null)
        //    owner.buffController.RemoveBuff(attachBuff);

        RemoveAttachBuff();
    }

    //设置装备加成
    private void SetEquipDataAdd()
    {
        List<BeMechanism> mechanismList = owner.MechanismList;
        if (mechanismList == null)
            return;
        for (int i = 0; i < mechanismList.Count; i++)
        {
            var mechanism = mechanismList[i] as Mechanism2030;
            if (mechanism == null)
                continue;
            hurtMaxLimit *=VFactor.one + VFactor.NewVFactor(mechanism.hurtMaxLimitAddRate,GlobalLogic.VALUE_1000);
        }
    }
}
