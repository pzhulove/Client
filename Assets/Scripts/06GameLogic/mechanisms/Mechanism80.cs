using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameClient;

public class Mechanism80 : BeMechanism {

    private int jobID;
    private int weaponType;
    private List<int> buffInfoList = new List<int>();
    private IBeEventHandle handle = null;
    public Mechanism80(int mid, int lv): base(mid, lv)
    {
        //canRemove = false;
    }
    
    public override void OnReset()
    {
        buffInfoList.Clear();
        handle = null;
    }

    public override void OnInit()
    {
        jobID = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        weaponType = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        for (int i = 0; i < data.ValueC.Count; i++)
        {
            buffInfoList.Add(TableManager.GetValueFromUnionCell(data.ValueC[i], level));
        }
    }
    
    public override void OnStart()
    {
        base.OnStart();

        handle = owner.RegisterEventNew(BeEventType.OnChangeWeapon, OnChangeWeapon);

        handleA = owner.RegisterEventNew(BeEventType.onChangeEquipEnd, OnChangeEquip);
		
		SetTriggerBuff();
    }

    protected void OnChangeWeapon(BeEvent.BeEventParam param)
    {
        SetTriggerBuff();
    }

    protected void OnChangeEquip(BeEvent.BeEventParam param)
    {
        SetTriggerBuff();
    }

    private void SetTriggerBuff()
    {
        RemoveTriggerBuff();
        int type = owner.GetWeaponType();
        int tmpWeaponType = type == 0 ? owner.GetDefaultWeaponType() : type;
        if (owner.professionID != 0 && weaponType != 0)
        {           
            if (owner.professionID == jobID && weaponType == tmpWeaponType)
                AddTriggerBuff();
        }
        else if (owner.professionID == 0 && weaponType != 0)
        {
            if (weaponType == tmpWeaponType)
            {
                AddTriggerBuff();
            }
        }
        else if (weaponType == 0 && owner.professionID != 0)
        {
            if (jobID == owner.professionID)
            {
                AddTriggerBuff();
            }
        }
    }

    public override void OnFinish()
    {
        base.OnFinish();
        if (handle != null)
        {
            handle.Remove();
            handle = null;
        }
    }

    private void AddTriggerBuff()
    {
        for(int i=0;i< buffInfoList.Count; i++)
        {
            BuffInfoData buffInfo = new BuffInfoData(buffInfoList[i], level);
            if (buffInfo.condition <= BuffCondition.NONE)
            {
                owner.buffController.TryAddBuff(buffInfo);
            }
            else
            {
                owner.buffController.AddTriggerBuff(buffInfo);
            }
        }
    }

    private void RemoveTriggerBuff()
    {
        for(int i=0;i< buffInfoList.Count; i++)
        {
            var data = TableManager.GetInstance().GetTableItem<ProtoTable.BuffInfoTable>(buffInfoList[i]);
            if (data != null)
            {
                if (data.BuffCondition <= (int)BuffCondition.NONE)
                {
                    owner.buffController.RemoveBuff(data.BuffID);
                }
                else
                {
                    owner.buffController.RemoveTriggerBuff(buffInfoList[i]);
                }
            }
        }
    }
}
