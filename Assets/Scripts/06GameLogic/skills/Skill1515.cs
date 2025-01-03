using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameClient;

public class Skill1515 : BeSkill
{
	IBeEventHandle handle = null;
	List<int> buffIDs = new List<int>();
    BeEvent.BeEventHandleNew block = null;
    int duration = 200;
    int dis = 500;
    List<int> weaponType = new List<int>();
    List<int> carrerID = new List<int>();

    int pveBuffID = 0;
    int pvpBuffID = 0;

    public bool isAutoBlock = false;

    private Mechanism73 mechanism;
    public Skill1515(int sid, int skillLevel):base(sid, skillLevel)
    {

    }

	public override void OnInit ()
	{
        buffIDs.Clear();
		for(int i=0; i<skillData.ValueA.Count; ++i)
		{
			var value = TableManager.GetValueFromUnionCell(isPVP() ? skillData.ValueB[i]: skillData.ValueA[i], level);
			if (value > 0)
				buffIDs.Add(value);
		}

        if (isPVP())
        {
            if (skillData.ValueD.Count > 1)
            {
                duration = TableManager.GetValueFromUnionCell(skillData.ValueD[0], level);
                dis = TableManager.GetValueFromUnionCell(skillData.ValueD[1], level);
            }
        }
        else
        {
            if (skillData.ValueC.Count > 1)
            {
                duration = TableManager.GetValueFromUnionCell(skillData.ValueC[0], level);
                dis = TableManager.GetValueFromUnionCell(skillData.ValueC[1], level);
            }
        }


        weaponType.Clear();
        for (int i = 0; i < skillData.ValueE.Count; ++i)
        {
            var value = TableManager.GetValueFromUnionCell(skillData.ValueE[i], level);
            if (value > 0)
                weaponType.Add(value);
        }

        carrerID.Clear();
        for (int i = 0; i < skillData.ValueF.Count; ++i)
        {
            var value = TableManager.GetValueFromUnionCell(skillData.ValueF[i], level);
            if (value > 0)
                carrerID.Add(value);
        }

        if (skillData.ValueG.Count > 1)
        {
            pveBuffID = TableManager.GetValueFromUnionCell(skillData.ValueG[0], level);
            pvpBuffID = TableManager.GetValueFromUnionCell(skillData.ValueG[1], level);
        }
    }
    public override void OnPostInit()
    {
        base.OnPostInit();
        if (isPVP())
        {
            if (skillData.ValueD.Count > 1)
            {
                duration = TableManager.GetValueFromUnionCell(skillData.ValueD[0], level);
                dis = TableManager.GetValueFromUnionCell(skillData.ValueD[1], level);
            }
        }
        else
        {
            if (skillData.ValueC.Count > 1)
            {
                duration = TableManager.GetValueFromUnionCell(skillData.ValueC[0], level);
                dis = TableManager.GetValueFromUnionCell(skillData.ValueC[1], level);
            }
        }
    }

    private bool isPVP()
    {
        return BattleMain.IsModePvP(battleType);
    }

	public override void OnStart()
	{
        mechanism = (isPVP() ? owner.GetMechanism(5062) : owner.GetMechanism(5061))as Mechanism73;

        AddBuff();

		RemoveHandle();
        //handle = owner.RegisterEvent(BeEventType.onBackHit, (object[] args)=>
        handle = owner.RegisterEventNew(BeEventType.onBackHit, args =>
        {
            owner.Locomote(new BeStateData((int)ActionState.AS_IDLE), true);
        });

        block = owner.RegisterEventNew(BeEventType.BlockSuccess, _BlockSuccess);

        //block = owner.RegisterEvent(BeEventType.BlockSuccess, (object[] args) =>
        //   {
        //       int[] vals = (int[])args[0];
        //       if (isAutoBlock && mechanism!=null)
        //       {
        //           vals[0] = mechanism.Duration;
        //           int flag = owner.GetFace() ? 1 : -1;
        //           vals[1] = flag * VInt.Float2VIntValue( (float)mechanism.Distance/1000);
        //       }
        //       else
        //       {
        //           vals[0] = duration;
        //           int flag = owner.GetFace() ? 1 : -1;
        //           vals[1] = flag * VInt.Float2VIntValue((float)dis/1000);
        //       }

        //       if (owner != null && carrerID.Contains(owner.GetEntityData().professtion)&&weaponType.Contains(owner.GetWeaponType()))
        //       {
        //           BuffInfoData data = new BuffInfoData(isPVP() ? pvpBuffID : pveBuffID,level);
        //           owner.buffController.TryAddBuff(data);
        //       }
        //   });
    }

    private void _BlockSuccess(BeEvent.BeEventParam param)
    {
        if (isAutoBlock && mechanism != null)
        {
            param.m_Int = mechanism.Duration;
            int flag = owner.GetFace() ? 1 : -1;
            param.m_Int2 = flag * VInt.Float2VIntValue((float)mechanism.Distance / 1000);
        }
        else
        {
            param.m_Int = duration;
            int flag = owner.GetFace() ? 1 : -1;
            param.m_Int2 = flag * VInt.Float2VIntValue((float)dis / 1000);
        }

        if (owner != null && carrerID.Contains(owner.GetEntityData().professtion) && weaponType.Contains(owner.GetWeaponType()))
        {
            BuffInfoData data = new BuffInfoData(isPVP() ? pvpBuffID : pveBuffID, level);
            owner.buffController.TryAddBuff(data);
        }
    }

    int timer = 0;
    public override void OnUpdate(int iDeltime)
    {
        base.OnUpdate(iDeltime);       
        if (isAutoBlock && mechanism!=null)
        {
            timer += iDeltime;
            if (timer >= mechanism.Duration)
            {
                StopMoveByAction();
                owner.Locomote(new BeStateData((int)ActionState.AS_IDLE), true);
                isAutoBlock = false;
                timer = 0;
            }
        }
    }

    private void StopMoveByAction()
    {
        BeMoveBy action = null;
        List<BeAction> list = owner.actionManager.GetActionList();
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] is BeMoveBy)
                action = list[i] as BeMoveBy;
        }
        if (action != null)
        {
            action.Stop();
        }
    }

    public override void OnFinish ()
	{
		//Logger.LogErrorFormat("skill1515 OnFinish");
		RemoveBuff();
		RemoveHandle();
	}

	public override void OnCancel ()
	{
		//Logger.LogErrorFormat("skill1515 OnCancel");
		RemoveBuff();
		RemoveHandle();
	}

	void AddBuff()
	{
		owner.buffController.TryAddBuff((int)GlobalBuff.GEDANG, int.MaxValue);
		for(int i = 0; i<buffIDs.Count; ++i)
		{
			owner.buffController.TryAddBuff(buffIDs[i], int.MaxValue, level);
		}
	}

	void RemoveBuff()
	{
		owner.buffController.RemoveBuff((int)GlobalBuff.GEDANG);
		for(int i = 0; i<buffIDs.Count; ++i)
		{
			owner.buffController.RemoveBuff(buffIDs[i]);
		}
	}

	void RemoveHandle()
	{
		if (handle != null)
		{
			handle.Remove();
			handle = null;
		}
        if (block != null)
        {
            block.Remove();
            block = null;
        }
    }



}
