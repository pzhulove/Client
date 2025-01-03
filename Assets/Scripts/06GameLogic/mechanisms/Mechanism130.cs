using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 能量护盾机制
/// </summary>
public class Mechanism130 : BeMechanism
{
    public Mechanism130(int mid, int lv) : base(mid, lv) { }

    protected int absorbDamageRate = 0;
    protected int absorbDamageMax = 0;
    protected int buffId = 0;

    protected int curDamage = 0;

    public override void OnReset()
    {
        curDamage = 0;
    }
    public override void OnInit()
    {
        base.OnInit();
        absorbDamageRate = TableManager.GetValueFromUnionCell(data.ValueA[0],level);
        absorbDamageMax = TableManager.GetValueFromUnionCell(data.ValueB[0],level);
        buffId = TableManager.GetValueFromUnionCell(data.ValueC[0],level);
    }

    public override void OnStart()
    {
        base.OnStart();
        curDamage = 0;
        StartEnergyUI(absorbDamageMax);
        handleA = owner.RegisterEventNew(BeEventType.onChangeHurtValue, args =>
        //handleA = owner.RegisterEvent(BeEventType.onChangeHurtValue, (object[] args) => 
        {
            //int[] hurtValueArr = (int[])args[0];
            ChangeHurtValue(args);
        });
    }

    protected void ChangeHurtValue(GameClient.BeEvent.BeEventParam param)
    {
        VFactor absorbRate = new VFactor(absorbDamageRate, GlobalLogic.VALUE_1000);
        int canAbsorb = param.m_Int * absorbRate;
        if(curDamage + canAbsorb >= absorbDamageMax)
        {
            canAbsorb = absorbDamageMax - curDamage;
            owner.buffController.RemoveBuff(buffId);
        }
        param.m_Int -= canAbsorb;
        curDamage += canAbsorb;
        RefreshEnergrUI(absorbDamageMax - curDamage);
    }

    public override void OnFinish()
    {
        base.OnFinish();
        StopEnergyUI();
    }

    #region 进度条相关
    protected ComDungeonCharactorBarEnergy energyBar = null;
    protected void StartEnergyUI(int maxValue)
    {
#if !LOGIC_SERVER
        GameObject barObj = AssetLoader.instance.LoadResAsGameObject("UIFlatten/Prefabs/BattleUI/DungeonBar/DungeonCharactorHeadEnergy");
        energyBar = barObj.GetComponent<ComDungeonCharactorBarEnergy>();
        owner.m_pkGeActor.CreateBarRoot();
        GameObject barsRoot = owner.m_pkGeActor.mBarsRoot.hRoot;
        Utility.AttachTo(barObj,barsRoot);
        energyBar.InitData(maxValue);
#endif
    }

    protected void RefreshEnergrUI(int value)
    {
#if !LOGIC_SERVER
        if (energyBar == null)
            return;
        energyBar.RefreshData(value);
#endif
    }

    protected void StopEnergyUI()
    {
#if !LOGIC_SERVER
        if (energyBar == null)
            return;
        GameObject obj =  energyBar.GetGameObject();
        GameObject.Destroy(obj);
#endif
    }
    #endregion
}
