using System.Collections.Generic;

/// <summary>
/// 魅影守护 和 格挡减伤百分比 
/// </summary>
public class Mechanism1149 : BeMechanism
{
    public Mechanism1149(int id, int level) : base(id, level) { }

    private int _meiyingValue = 0;  //魅影守护修改值
    private int _geDangValue = 0;   //格挡修改值
    private AttackType _attackType = AttackType.NONE;   //伤害类型

    public override void OnInit()
    {
        base.OnInit();
        _meiyingValue = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        _geDangValue = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        _attackType = (AttackType)TableManager.GetValueFromUnionCell(data.ValueC[0], level);
    }

    public override void OnStart()
    {
        base.OnStart();
        _ChangeMeiYingData(false);
        _ChangeGeDangData(false);
    }

    public override void OnFinish()
    {
        base.OnFinish();
        _ChangeMeiYingData(true);
        _ChangeGeDangData(true);
    }

    private void _ChangeMeiYingData(bool isRestore = false)
    {
        if (owner.GetEntityData() == null) return;
        var battleData = owner.GetEntityData().battleData;
        if (battleData == null) return;
        if (_meiyingValue <= 0) return;

        if (isRestore)
        {
            RemoveAddDamage(battleData.reduceMeiyingDamagePercent, _attackType,_meiyingValue);
        }
        else
        {
            battleData.reduceMeiyingDamagePercent.Add(new AddDamageInfo(_meiyingValue, (int)_attackType));
        }
    }

    private void RemoveAddDamage(List<AddDamageInfo> list, AttackType type, int value)
    {
        int index = -1;
        for (int i = 0; i < list.Count; i++)
        {
            var addDamageInfo = list[i];
            if (addDamageInfo.value != value)
                continue;
            if (addDamageInfo.attackType != type)
                continue;
            index = i;
            break;
        }
        if (index >= 0)
            list.RemoveAt(index);
    }

    private void _ChangeGeDangData(bool isRestore = false)
    {
        if (owner.GetEntityData() == null) return;
        var battleData = owner.GetEntityData().battleData;
        if (battleData == null) return;
        if (_geDangValue <= 0) return;

        if (isRestore)
        {
            RemoveAddDamage(battleData.reduceGeDangDamagePercent, _attackType, _geDangValue);
        }
        else
        {
            battleData.reduceGeDangDamagePercent.Add(new AddDamageInfo(_geDangValue, (int)_attackType));
        }
    }

}