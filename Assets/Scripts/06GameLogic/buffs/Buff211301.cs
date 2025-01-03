using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameClient;

public class Buff211301 : BeBuff
{
    VFactor hpReplacePercent;
    VInt hpMpRate = VInt.one;
    VFactor mpRateToDeleteBuff = new VFactor(1,10);
    private VFactor hpReplaceDeltaPercent = VFactor.zero;
    // 只用于力法，减少耗蓝
    private VFactor lifaMPCostRate = VFactor.one;

	IBeEventHandle handler = null;
	IBeEventHandle handler2 = null;
	IBeEventHandle handler3 = null;
	private BeEvent.BeEventHandleNew mEquipHandle;
	

    public Buff211301(int bi, int buffLevel, int buffDuration, int attack = 0) : base(bi, buffLevel, buffDuration, attack)
    {

    }

    public static void BuffPreloadRes(ProtoTable.BuffTable buffTableData)
    {
#if !LOGIC_SERVER
        PreloadManager.PreloadEffectInfo(1004);
#endif
    }
	public override void OnReset()
	{
		hpReplacePercent = VFactor.zero;
		hpMpRate = VInt.one;
		mpRateToDeleteBuff = new VFactor(1,10);
		hpReplaceDeltaPercent = VFactor.zero;
		lifaMPCostRate = VFactor.one;

	}

    public override void OnInit()
    {
		if(BattleMain.IsModePvP(battleType))
		{
			hpReplacePercent = new VFactor(TableManager.GetValueFromUnionCell(buffData.ValueD[0], level),GlobalLogic.VALUE_1000);
			hpMpRate =	VInt.NewVInt(TableManager.GetValueFromUnionCell(buffData.ValueE[0], level),GlobalLogic.VALUE_1000);
			mpRateToDeleteBuff = new VFactor(TableManager.GetValueFromUnionCell(buffData.ValueF[0], level),GlobalLogic.VALUE_1000);
		}
		else {
			hpReplacePercent = new VFactor(TableManager.GetValueFromUnionCell(buffData.ValueA[0], level) ,GlobalLogic.VALUE_1000);
			hpMpRate = VInt.NewVInt(TableManager.GetValueFromUnionCell(buffData.ValueB[0], level),GlobalLogic.VALUE_1000);
			mpRateToDeleteBuff = new VFactor(TableManager.GetValueFromUnionCell(buffData.ValueC[0], level),GlobalLogic.VALUE_1000);	
		}

		// 力法专用
		if (buffData.ValueG.Count > 0)
		{
			lifaMPCostRate = new VFactor(TableManager.GetValueFromUnionCell(buffData.ValueG[0], level),GlobalLogic.VALUE_1000);
		}
    }

    public override void OnStart()
    {
		RemoveHandler();
        handler = owner.RegisterEventNew(BeEventType.onHit, args =>
        //handler = owner.RegisterEvent(BeEventType.onHit, (object[] args) =>
        {
            int hurtValue = args.m_Int;
            owner.m_pkGeActor.CreateEffect(1004, new Vec3(0, 0, 0));
        });

        handler2 = owner.RegisterEventNew(BeEventType.onAfterCalFirstDamage, ShareDamage);
        //handler2 = owner.RegisterEvent(BeEventType.onAfterCalFirstDamage, ShareDamage);

        // 异界装备PVP不生�?
        if (!BattleMain.IsModePvP(battleType))
        {
	        UpdateEquipData();
	        handler3 = owner.RegisterEventNew(BeEventType.OnChangeWeaponEnd, OnChangeWeapon);
	        mEquipHandle = owner.RegisterEventNew(BeEventType.onChangeEquipEnd, OnChangeEquip);
        }
    }
    
    protected void OnChangeWeapon(BeEvent.BeEventParam param)
    {
	    UpdateEquipData();
    }

    protected void OnChangeEquip(BeEvent.BeEventParam param)
    {
	    UpdateEquipData();
    }
    
    private void UpdateEquipData()
    {
	    hpReplaceDeltaPercent = VFactor.zero;
	    List<BeMechanism> mechanismList = owner.MechanismList;
	    if (mechanismList == null)
		    return;
	    for (int i = 0; i < mechanismList.Count; i++)
	    {
		    if(!mechanismList[i].isRunning)
			    continue;
		    
		    var mechanism = mechanismList[i] as Mechanism2094;
		    if (mechanism == null)
			    continue;
		    hpReplaceDeltaPercent += mechanism.HpReplaceDelta;
	    }
    }

    /// <summary>
    /// 魔法盾分担伤�?
    /// </summary>
    /// <param name="args"></param>
    private void ShareDamage(BeEvent.BeEventParam param)
    {
        int hurtId = param.m_Int3;
        if(!BeUtility.NeedShareBySGSH(hurtId,owner))
            return;
        bool isBeHit = param.m_Bool;
        if (!isBeHit)
            return;
        //int[] vals = (int[])args[0];
        int damage = param.m_Int;
        int hpRepInt = DoWork(damage);
        param.m_Int = damage - hpRepInt;
    }

	int  DoWork(int value)
    {
        int hpRep = value * (hpReplacePercent + hpReplaceDeltaPercent);
		int hpRepInt =  hpRep;
		int mpCost = (hpRepInt * hpMpRate.factor);
		if (owner.attribute != null && owner.attribute.professtion == (int) ActorOccupation.Zhandoufashi)
		{
			mpCost *= lifaMPCostRate;
		}

		Logger.LogWarningFormat("魔法护罩: value:{0} hpReplacePercent:{1} hpMpRate:{2}", value, hpReplacePercent, hpMpRate);
		Logger.LogWarningFormat("buff 211301 hpRep:{0} hpRepInt:{1} mpCost:{2}", hpRep, hpRepInt, mpCost);

		if (hpRepInt > 0)
		{
			//owner.DoHeal(hpRepInt, false);
			owner.DoMPChange(-mpCost, false);
		}
      
		//Logger.LogErrorFormat("mp rate:{0}", owner.attribute.GetMPRate());

		if (owner.attribute.GetMPRate() < mpRateToDeleteBuff)
        {
            Cancel();
        }

		return hpRepInt;
    }

	public override void OnFinish ()
	{
		RemoveHandler();
	}

	void RemoveHandler()
	{
		if (handler != null)
		{
			handler.Remove();
			handler = null;
		}

		if (handler2 != null)
		{
			handler2.Remove();
			handler2 = null;
		}
		
		if (handler3 != null)
		{
			handler3.Remove();
			handler3 = null;
		}
		
		if (mEquipHandle != null)
		{
			mEquipHandle.Remove();
			mEquipHandle = null;
		}
	}
}
