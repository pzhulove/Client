using GameClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mechanism81 : BeMechanism
{
    private List<int> pveBuffList = new List<int>();
    private List<int> pvpBuffList = new List<int>();

    private IBeEventHandle changeWeaponHandle;

    public VRate changeWeaponCD = new VRate();
    public Mechanism81(int mid, int lv) : base(mid, lv) {}

    public override void OnReset()
    {
        pveBuffList.Clear();
        pvpBuffList.Clear();
        changeWeaponHandle = null;
        changeWeaponCD = new VRate();
    }

    public override void OnInit()
    {
        pveBuffList.Clear();
        pvpBuffList.Clear();

        changeWeaponCD = new VRate(TableManager.GetValueFromUnionCell(data.ValueA[0], level) / 1000.0f);

        for (int i = 0; i < data.ValueB.Count; i++)
        {
            int value = TableManager.GetValueFromUnionCell(data.ValueB[i], level);
            if (value != 0)
                pveBuffList.Add(value);
        }

        for (int i = 0; i < data.ValueC.Count; i++)
        {
            int value = TableManager.GetValueFromUnionCell(data.ValueC[i], level);
            if (value != 0)
                pvpBuffList.Add(value);
        }
    }

    public override void OnStart()
    {
        changeWeaponHandle = owner.RegisterEventNew(BeEventType.OnChangeWeapon, (args) =>
        {
            ProtoTable.ItemTable oldWeapon = args.m_Obj as ProtoTable.ItemTable;
            ProtoTable.ItemTable newWeapon = args.m_Obj2 as ProtoTable.ItemTable;
            if (oldWeapon == null || newWeapon == null) return;
            if (oldWeapon.ThirdType != newWeapon.ThirdType)
            {
                List<int> buffList = BattleMain.IsModePvP(battleType) ? pvpBuffList : pveBuffList;

                for (int i = 0; i < buffList.Count; i++)
                {
                    BuffInfoData data = new BuffInfoData(pvpBuffList[i], level);
                    owner.buffController.TryAddBuff(data);
                }
            }

        });

    }

    public override void OnFinish()
    {
        base.OnFinish();
        if (changeWeaponHandle != null)
        {
            changeWeaponHandle.Remove();
            changeWeaponHandle = null;
        }
    }

}
