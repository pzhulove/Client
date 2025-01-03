using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 变身(怪物使用)
*/
public class Mechanism27 : BeMechanism {

    List<int> buffInfoIds = new List<int>();
    
    List<int> removeBuffIds = new List<int>();
    List<int> removeBuffInfoIds = new List<int>();
    
    List<int> finishAddbuffInfoIds = new List<int>();
    int selectedMonsterId = 0;

    public Mechanism27(int mid, int lv):base(mid, lv)
	{
    }

    public override void OnReset()
    {
        buffInfoIds.Clear();
        removeBuffIds.Clear();
        removeBuffInfoIds.Clear();
        finishAddbuffInfoIds.Clear();
        selectedMonsterId = 0;
    }
	public override void OnInit ()
	{
        int countBuff = data.ValueB.Count;
        for (int i = 0; i < countBuff; ++i)
        {
            var buffId = TableManager.GetValueFromUnionCell(data.ValueB[i], 1);
            if (buffId > 0)
                buffInfoIds.Add(buffId);
        }

        for (int i = 0; i < data.ValueC.Count; ++i)
        {
            removeBuffIds.Add(TableManager.GetValueFromUnionCell(data.ValueC[i], level));
        }
        
        for (int i = 0; i < data.ValueD.Count; ++i)
        {
            removeBuffInfoIds.Add(TableManager.GetValueFromUnionCell(data.ValueD[i], level));
        }
        
        for (int i = 0; i < data.ValueE.Count; ++i)
        {
            finishAddbuffInfoIds.Add(TableManager.GetValueFromUnionCell(data.ValueE[i], level));
        }
    }

    public static int GetMonsterId(int masterIdOfOwner, ProtoTable.MechanismTable data)
    {
        if (data == null)
            return 0;

        int selectIdx = 0;
        if (masterIdOfOwner > 0)
            selectIdx = masterIdOfOwner % 10 - 1; // ( 1~4 )- 1

        int countMid = data.ValueA.Count;
        if (selectIdx < 0 || selectIdx > countMid - 1)
            return 0;

        int mid = TableManager.GetValueFromUnionCell(data.ValueA[selectIdx], 1);
        return mid;
    }

    public override void OnStart()
    {
        if (owner == null)
            return;

        // 适配怪物难度(规则尾号)
        var entityData = owner.GetEntityData();
        if (entityData == null)
            return;
        if (owner.IsDead()) return;
        selectedMonsterId = GetMonsterId(entityData.monsterID, data);
        ProtoTable.UnitTable unitData = TableManager.GetInstance().GetTableItem<ProtoTable.UnitTable>(selectedMonsterId);
        if (unitData == null)
            return;

        entityData.overHeadHeight = unitData.overHeadHeight / (float)(GlobalLogic.VALUE_1000);
        entityData.buffOriginHeight = unitData.buffOriginHeight / (float)(GlobalLogic.VALUE_1000);

        // 概率释放被击技能支持
        entityData.hitID = unitData.HitSkillID;
        entityData.hitIDRand = unitData.HitSkillRand;

        owner.ChangeModel(unitData, false);

        // BUFF
        BeBuffManager buffController = owner.buffController;
        if (buffController == null)
            return;

        RemoveBuff();
        
        int countBuff = buffInfoIds.Count;
        for (int i = 0; i < countBuff; ++i)
        {
            int buffInfoId = buffInfoIds[i];
            //if (buffController.HasBuffByID(buffInfoId) == null)
            buffController.TryAddBuff(buffInfoId);
        }
    }

    public override void OnFinish()
    {
        if (owner == null)
            return;
        if (owner.IsDead()) return;

        owner.TriggerEventNew(BeEventType.onChangeModelFinish);

        ProtoTable.UnitTable unitData = TableManager.GetInstance().GetTableItem<ProtoTable.UnitTable>(selectedMonsterId);
        if (unitData == null)
            return;

        owner.ChangeSkill(unitData);
        
        for (int i = 0; i < finishAddbuffInfoIds.Count; ++i)
        {
            BuffInfoData buffInfo = new BuffInfoData(finishAddbuffInfoIds[i], level);
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


    private void RemoveBuff()
    {
        for (int i = 0; i < removeBuffIds.Count; i++)
        {
            owner.buffController.RemoveBuff(removeBuffIds[i]);
        }
        
        for (int i = 0; i < removeBuffInfoIds.Count; i++)
        {
            owner.buffController.RemoveBuffByBuffInfoID(removeBuffInfoIds[i]);
        }
    }
}
