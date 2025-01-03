using System.Collections.Generic;

//根据自己当前的体力或者精神，修改自己原来的属性
public class Mechanism1017 : BeMechanism
{
    public enum ChangeAttType
    {
        None = 0,
        HPMaxAdd = 1,   //Hp最大值增加量    体力
        MPMaxAdd = 2,   //Mp最大值增加量
        PhysicalDefenceAdd = 3, //增加的物理防御   体力
        MagicDefenceAdd = 4,    //增加的魔法防御
        AtkAdd = 5, //增加的力量     体力
        IntAdd = 6, //增加的智力
        Hp = 7, //回生命       体力
        Mp = 8,    //回魔力
        PhysicalAttackAdd = 9,  //增加的物理攻击力      体力
        MagicAttackAdd = 10,     //增加的魔法攻击力
        HpRecover = 11,  //Hp恢复量        体力
        MpRecover = 12,  //Mp恢复量
        Count = 13,
    }

    private enum EffectType
    {
        None = 0,
        Sta = 1,    //体力
        Spr = 2,    //精神
    }

    public Mechanism1017(int mid, int lv) : base(mid, lv) { }

    private int typeCount = 0;
    private int[] addValueArr = null;   //增加的值列表
    private int[] coefficientArr = null;//系数列表
    private int[] selfExtraAddRateArr = null;    //自己的额外增加比率列表
    private List<int> coeffSpecialList = new List<int>();//生命源泉特殊系数列表

    private int[] recoverValueArr = null;    //记录增加的值 用于后期恢复
    private BeActor attachBuffReleaser = null;    //Buff添加者

    private int[] equipAddValueArr = null;   //装备增加的属性(固定值)
    private int[] equipAddRateArr = null;   //装备增加的属性（千分比）

    public override void OnInit()
    {
        base.OnInit();
        InitData();
        for (int i = 0; i < data.ValueA.Count; i++)
        {
            int index = TableManager.GetValueFromUnionCell(data.ValueA[i], level) - 1;
            addValueArr[index] = (TableManager.GetValueFromUnionCell(data.ValueB[i], level));
            coefficientArr[index] = (TableManager.GetValueFromUnionCell(data.ValueC[i], level));
            selfExtraAddRateArr[index] = TableManager.GetValueFromUnionCell(data.ValueD[i], level);
        }

        for (int i = 0; i < data.ValueE.Count; i++)
        {
            coeffSpecialList.Add(TableManager.GetValueFromUnionCell(data.ValueE[i], level));
        }
    }

    private void InitData()
    {
        typeCount = (int)ChangeAttType.Count - 1;
        addValueArr = new int[typeCount];
        coefficientArr = new int[typeCount];
        selfExtraAddRateArr = new int[typeCount];
        equipAddValueArr = new int[typeCount];
        equipAddRateArr = new int[typeCount];
    }

    public override void OnStart()
    {
        SetBuffReleaser();
        InitExtraAdd();
        SetEquipDataAdd();
        ChangeAttr();
    }

    public override void OnFinish()
    {
        Recover();
    }

    //设置机制Buff的释放者
    private void SetBuffReleaser()
    {
        /*
        if (attachBuff == null)
            return;
        attachBuffReleaser = attachBuff.releaser;
        */

        var thisAttachBuff = GetAttachBuff();
        if (thisAttachBuff == null)
            return;
        attachBuffReleaser = thisAttachBuff.releaser;
    }

    //对自己释放增长更多
    private void InitExtraAdd()
    {
        for (int i = 0; i < typeCount; i++)
        {
            if (attachBuffReleaser != null && attachBuffReleaser != owner)
                selfExtraAddRateArr[i] = GlobalLogic.VALUE_1000;
        }
    }

    //改变
    private void ChangeAttr()
    {
        if (attachBuffReleaser == null || attachBuffReleaser.GetEntityData() == null || attachBuffReleaser.GetEntityData().battleData == null)
            return;
        recoverValueArr = new int[typeCount];
        for (int i = 0; i < typeCount; i++)
        {
            ChangeAttType changeType = (ChangeAttType)(i + 1);
            EffectType type = IsEffectBySta(changeType) ? EffectType.Sta : EffectType.Spr;
            int staSprValue = GetEffectValue(type, attachBuffReleaser);
            VFactor rate = VFactor.zero;
            if (coefficientArr[i] != 0)
                rate = new VFactor(staSprValue, coefficientArr[i]);
            rate += VFactor.one;
            rate *= GetSpecialCoff(i, type);
            rate *= new VFactor(selfExtraAddRateArr[i], GlobalLogic.VALUE_1000);

            int addValue = addValueArr[i];

            //先代入装备加成再进行计算
            addValue = EquipAdd(i, addValue, !IsNeedPriorityCalc(changeType));
            if (rate != 0)
                addValue *= rate;

            //先计算再代入装备加成
            addValue = EquipAdd(i, addValue, IsNeedPriorityCalc(changeType));

            if (addValue == 0)
                continue;
            recoverValueArr[i] = addValue;
            ChangeDataByType(changeType, recoverValueArr[i]);
        }
    }

    //获取特殊系数加成
    private VFactor GetSpecialCoff(int index, EffectType type)
    {
        if (index >= coeffSpecialList.Count)
            return VFactor.one;
        int staSprValue = GetEffectValue(type, owner);
        VFactor factor = VFactor.zero;
        if (coeffSpecialList[index] != 0)
            factor = new VFactor(staSprValue, coeffSpecialList[index]);
        return factor + VFactor.one;
    }

    private int EquipAdd(int index, int value, bool needCalc)
    {
        if (!needCalc)
            return value;
        int changeValue = value;
        VFactor equipAddRate = new VFactor(equipAddRateArr[index], GlobalLogic.VALUE_1000);
        changeValue += changeValue * equipAddRate;
        changeValue += equipAddValueArr[index];
        return changeValue;
    }

    //恢复
    private void Recover()
    {
        if (recoverValueArr == null)
            return;
        for (int i = 0; i < typeCount; i++)
        {
            ChangeAttType type = (ChangeAttType)(i + 1);
            //这几种类型不需要恢复
            if (type == ChangeAttType.Hp || type == ChangeAttType.Mp)
                continue;
            if (recoverValueArr[i] == 0)
                continue;
            ChangeDataByType(type, -recoverValueArr[i]);
        }
    }

    //根据枚举改变数据
    private void ChangeDataByType(ChangeAttType changeType, int addValue)
    {
        if (owner == null || owner.GetEntityData() == null || owner.GetEntityData().battleData == null)
            return;
        BeEntityData entityData = owner.GetEntityData();
        BattleData battleData = owner.GetEntityData().battleData;
        bool isAdd = true;
        switch (changeType)
        {
            case ChangeAttType.HPMaxAdd:
                entityData.ChangeMaxHp(addValue);
                DoSyncHPBar(owner);
                break;
            case ChangeAttType.MPMaxAdd:
                battleData.ChangeMaxMP(addValue);
                DoSyncHPBar(owner);
                break;
            case ChangeAttType.HpRecover:
                entityData.SetAttributeValue(AttributeType.hpRecover, addValue, isAdd);
                battleData.hpRecover = owner.GetEntityData().battleData.fHpRecoer;
                break;
            case ChangeAttType.MpRecover:
                entityData.SetAttributeValue(AttributeType.mpRecover, addValue, isAdd);
                battleData.mpRecover = owner.GetEntityData().battleData.fMpRecover;
                break;
            case ChangeAttType.AtkAdd:
                battleData.SetValue(AttributeType.baseAtk, addValue * GlobalLogic.VALUE_1000, isAdd);
                break;
            case ChangeAttType.IntAdd:
                battleData.SetValue(AttributeType.baseInt, addValue * GlobalLogic.VALUE_1000, isAdd);
                break;
            case ChangeAttType.MagicAttackAdd:
                battleData.SetValue(AttributeType.magicAttack, addValue, isAdd);
                break;
            case ChangeAttType.MagicDefenceAdd:
                battleData.SetValue(AttributeType.magicDefence, addValue, isAdd);
                break;
            case ChangeAttType.PhysicalAttackAdd:
                battleData.SetValue(AttributeType.attack, addValue, isAdd);
                break;
            case ChangeAttType.PhysicalDefenceAdd:
                battleData.SetValue(AttributeType.defence, addValue, isAdd);
                break;
            case ChangeAttType.Hp:
                owner.DoHPChange(addValue, true);
                break;
            case ChangeAttType.Mp:
                owner.DoMPChange(addValue, false);
                break;
        }
    }

    //根据类型获取体力值或者精力值
    private int GetEffectValue(EffectType effectType, BeActor sourceActor)
    {
        if (sourceActor == null || sourceActor.IsDead() || sourceActor.GetEntityData() == null)
            return 0;
        BattleData battleData = sourceActor.GetEntityData().battleData;
        if (battleData == null)
            return 0;
        int value = 0;
        if (effectType == EffectType.Sta)
            value = battleData.sta * VFactor.NewVFactor(GlobalLogic.VALUE_1, GlobalLogic.VALUE_1000);
        else if (effectType == EffectType.Spr)
            value = battleData.spr * VFactor.NewVFactor(GlobalLogic.VALUE_1, GlobalLogic.VALUE_1000);
        return value;
    }

    //同步血条
    protected void DoSyncHPBar(BeActor actor)
    {
#if !LOGIC_SERVER

        if (actor != null && actor.m_pkGeActor != null)
            actor.m_pkGeActor.SyncHPBar();
#endif
    }

    //是否需要先代入公式计算再修改数值 
    private bool IsNeedPriorityCalc(ChangeAttType type)
    {
        if (type == ChangeAttType.HPMaxAdd || type == ChangeAttType.MPMaxAdd || type == ChangeAttType.HpRecover || type == ChangeAttType.MpRecover)
            return true;
        return false;
    }

    //是否是受体力影响
    private bool IsEffectBySta(ChangeAttType type)
    {
        if (type == ChangeAttType.HPMaxAdd || type == ChangeAttType.PhysicalDefenceAdd || type == ChangeAttType.AtkAdd
            || type == ChangeAttType.Hp || type == ChangeAttType.PhysicalAttackAdd || type == ChangeAttType.HpRecover)
            return true;
        return false;
    }


    //设置装备加成
    private void SetEquipDataAdd()
    {
        BeActor attachBuffReleaser = GetAttachBuffReleaser();
        if (attachBuffReleaser == null)
            return;
        List<BeMechanism> mechanismList = attachBuffReleaser.MechanismList;
        if (mechanismList == null)
            return;
        //每次使用机制前装备加成数据重置
        equipAddValueArr = new int[typeCount];
        equipAddRateArr = new int[typeCount];
        for (int i = 0; i < mechanismList.Count; i++)
        {
            var mechanism = mechanismList[i] as Mechanism2027;
            if (mechanism == null )
                continue;
            if ((attachBuffReleaser.sgGetCurrentState() == (int)ActionState.AS_CASTSKILL && mechanism.IsContainSkillID(attachBuffReleaser.GetCurSkillID())))
            {
                for (int j = 0; j < typeCount; j++)
                {
                    equipAddValueArr[j] += mechanism.addValueArr[j];
                    equipAddRateArr[j] += mechanism.addRateArr[j];
                    if (selfExtraAddRateArr[j] == 0)
                        selfExtraAddRateArr[j] += mechanism.selfExtraAddRateArr[j];
                    if (coefficientArr[j] == 0)
                        coefficientArr[j] += mechanism.coefficientArr[j];
                }
            }
        }
    }
    public int[] GetRecoverValueArr()
    {
        return recoverValueArr;
    }
}
