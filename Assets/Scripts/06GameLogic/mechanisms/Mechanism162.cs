using System.Collections.Generic;
using GameClient;
using ProtoTable;
//被击后增加伤害百分比
public class Mechanism162 : BeMechanism
{
    public enum AddDamageType
    {
        NONE,
        PHYSIC = 1, //物理伤害
        MAGIC = 2,  //魔法伤害
        NEAR = 3,   //近战
        FAR  = 4,    //远程
    }
    int addDamageRate = 0;
    int addDamageFixedRate = 0;
    protected List<int> m_AddDamageTypeList = new List<int>(5);

    public Mechanism162(int mid, int lv) : base(mid, lv) { }
    public override void OnInit()
    {
        InitData();
    }

    protected void InitData()
    {
        m_AddDamageTypeList.Clear();
        addDamageRate = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        addDamageFixedRate = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        for (int i = 0; i < data.ValueC.Count; i++)
        {
            m_AddDamageTypeList.Add(TableManager.GetValueFromUnionCell(data.ValueC[i], level));
        }
    }
    public override void OnStart()
    {
        handleA = owner.RegisterEventNew(BeEventType.onBeHitChangeDamage, OnBeHitChangeDamage);
    }
    protected void OnBeHitChangeDamage(BeEvent.BeEventParam args)
    {
        var effectTableData = args.m_Obj as EffectTable;
        if (!CheckCondition(effectTableData))
            return;
        /*var damageRateArray = (int[])args[1];
        damageRateArray[0] += addDamageRate;
        damageRateArray[1] += addDamageFixedRate;*/
        args.m_Int2 += addDamageRate;
        args.m_Int3 += addDamageFixedRate;
    }
    protected bool CheckCondition(EffectTable m_EffectTableData)
    {
        if(null == m_AddDamageTypeList   || 0 == m_AddDamageTypeList.Count)
        {
            return true;
        }
        if (m_EffectTableData == null )
        {
            return true;
        }
        for (int i = 0; i < m_AddDamageTypeList.Count; i++)
        {
            var type = m_AddDamageTypeList[i];
            switch (type)
            {
                case 1:
                    if (m_EffectTableData.DamageType != EffectTable.eDamageType.PHYSIC)
                        return false;
                    break;
                case 2:
                    if (m_EffectTableData.DamageType != EffectTable.eDamageType.MAGIC)
                        return false;
                    break;
                case 3:
                    if (m_EffectTableData.DamageDistanceType != EffectTable.eDamageDistanceType.NEAR)
                        return false;
                    break;
                case 4:
                    if (m_EffectTableData.DamageDistanceType != EffectTable.eDamageDistanceType.FAR)
                        return false;
                    break;
            }
        }
        return true;
    }
}

