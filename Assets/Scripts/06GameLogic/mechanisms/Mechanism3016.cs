using GameClient;
using UnityEngine;

/// <summary>
/// 明王-剑气UI
/// </summary>
public class Mechanism3016 : BeMechanism
{
    public Mechanism3016(int mid, int lv) : base(mid, lv)
    {
    }

    public const int EnergyMgrID = 2140;
    
    private int m_CurEnergy = 0;
    private int m_EnergyMax;
    private int m_UseDuration;

    public int CurEnergy => m_CurEnergy;
    public int MaxEnergy => m_EnergyMax;

    private enum EnergyState
    {
        Collect,
        Use
    }
    private EnergyState m_CurState = EnergyState.Collect;
    private int m_UseDurationAcc;

    public override void OnInit()
    {
        base.OnInit();
        m_EnergyMax = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        m_UseDuration = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
    }

    public override void OnStart()
    {
        base.OnStart();
#if !LOGIC_SERVER
        EnergyActive(true);
        SetEnergy(0);
#endif
        handleA = owner.RegisterEventNew(BeEventType.onMingWangSetEnergy, OnSetEnergy);
        handleB = owner.RegisterEventNew(BeEventType.onMingWangUseEnergy, OnUseEnergy);
    }

    private void OnUseEnergy(BeEvent.BeEventParam param)
    {
        UseEnergy();
    }

    public override void OnFinish()
    {
        base.OnFinish();
#if !LOGIC_SERVER
        EnergyActive(false);
#endif
    }

    public override void OnReset()
    {
        base.OnReset();
        
        m_CurEnergy = 0;
        m_EnergyMax = 0;
        m_UseDuration = 0; 
        m_CurState = EnergyState.Collect;
        m_UseDurationAcc = 0;
#if !LOGIC_SERVER
        m_BattleUI = null;
#endif
    }

    private void OnSetEnergy(BeEvent.BeEventParam param)
    {
        if(m_CurState == EnergyState.Collect)
            SetEnergy(param.m_Int);
    }

    public void SetEnergy(int v)
    {
        var temp = m_CurEnergy;
        m_CurEnergy = IntMath.Min(v, m_EnergyMax) ;
#if !LOGIC_SERVER
        AddEnergyUI(temp, m_CurEnergy);
#endif
    }

    public bool CanUse()
    {
        return m_CurState == EnergyState.Collect && m_CurEnergy >= m_EnergyMax;
    }

    public void UseEnergy()
    {
        if (!CanUse())
            return;

        m_CurEnergy = 0;
        m_CurState = EnergyState.Use;
        m_UseDurationAcc = m_UseDuration;
    }

    public override void OnUpdate(int deltaTime)
    {
        base.OnUpdate(deltaTime);
        if (m_CurState == EnergyState.Use)
        {
            m_UseDurationAcc -= deltaTime;
#if !LOGIC_SERVER
            DownEnergyUI();
#endif
            if (m_UseDurationAcc <= 0)
                m_CurState = EnergyState.Collect;
        }
    }

#if !LOGIC_SERVER
    private BattleUIProfession m_BattleUI;
    public BattleUIProfession BattleUI
    {
        get
        {
            if (m_BattleUI == null)
            {
                m_BattleUI = BattleUIHelper.CreateBattleUIComponent<BattleUIProfession>();
            }
            return m_BattleUI;
        }
    }

    private void EnergyActive(bool active)
    {
        if (!owner.isLocalActor)
            return;

        var ui = BattleUI;
        if(ui == null)
            return;
        
        ui.JianQiActive(active);
    }
    private void AddEnergyUI(int form, int to)
    {
        if (!owner.isLocalActor)
            return;

        var ui = BattleUI;
        if(ui == null)
            return;
        
        ui.SetJianqi(to, m_EnergyMax);
    }
    
    private void DownEnergyUI()
    {
        if (!owner.isLocalActor)
            return;

        var ui = BattleUI;
        if(ui == null)
            return;

        Mathf.Max(m_UseDurationAcc, 0);
        ui.SetJianqiDown(m_UseDurationAcc, m_UseDuration);
    }
#endif
}
