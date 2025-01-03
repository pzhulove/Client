using System;
using System.Collections.Generic;
using GameClient;

/// <summary>
/// 力法装备机制
/// 装备改变炫纹生成逻辑相关机制
///
/// 1.改炫纹机制的炫纹数量上限
/// 2.改自动炫纹机制的生成额外炫纹的概率（技能命中/连击数都加）
/// 3.改变连击数20以上生成的所有炫纹的大小（表现与逻辑）
/// 4.改变炫纹的大小（表现与逻辑）
/// 5.偃月刀实体数
/// </summary>
public class Mechanism2089 : BeMechanism
{
    public struct ComboScale
    {
        public int combo;
        public VFactor scale;
    }
    private int m_ChaserMaxCount = 0;        // 改炫纹机制的炫纹数量上限
    private int m_ChaserCreateProb = 0;      // 改自动炫纹机制的生成额外炫纹的概率
    private int m_BigChaserCreateProb = 0;      // 改自动炫纹机制的定时生成大炫纹的概率
    private ComboScale m_ComboChaserScale;      // 改变连击数(m_ComboScaleCount)以上生成的所有炫纹的大小
    private int[] m_ChaserScale = new int[5];  // 改变炫纹的大小
    private int m_knifeCount = 0;
    private int m_ChaserCreateTime = 0;
    private bool m_isOffsetCreateTime = false;

    public int ChaserMaxCount
    {
        get { return m_ChaserMaxCount; }
    }

    public int ChaserCreateProb
    {
        get { return m_ChaserCreateProb; }
    }

    public int BigChaserCreateProb
    {
        get { return m_BigChaserCreateProb; }
    }

    public ComboScale ComboChaserScale
    {
        get { return m_ComboChaserScale; }
    }

    public int KnifeCount
    {
        get { return m_knifeCount; }
    }

    

    public int ChaserScale(int i)
    {
        if (i >= m_ChaserScale.Length)
        {
            return 0;
        }
        
        return m_ChaserScale[i];
    }

    public Mechanism2089(int id, int level) : base(id, level) { }

    public override void OnInit()
    {
        if (data.ValueA.Count > 0)
        {
            m_ChaserMaxCount = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        }
        
        if (data.ValueB.Count > 0)
        {
            m_ChaserCreateProb = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        }
        
        if (data.ValueC.Count > 0)
        {
            m_ComboChaserScale.combo = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
            m_ComboChaserScale.scale = VFactor.NewVFactor(TableManager.GetValueFromUnionCell(data.ValueC[1], level), GlobalLogic.VALUE_1000);
        }
        else
        {
            m_ComboChaserScale.combo = -1;
            m_ComboChaserScale.scale = VFactor.zero;
        }
        
        if (data.ValueD.Count > 0)
        {
            for (int i = 0; i < data.ValueD.Count; i++)
            {
                m_ChaserScale[i] = TableManager.GetValueFromUnionCell(data.ValueD[i], level);
            }
        }
        else
        {
            for (int i = 0; i < 5; i++)
            {
                m_ChaserScale[i] = 0;
            }
        }

        if (data.ValueE.Count > 0)
        {
            m_knifeCount = TableManager.GetValueFromUnionCell(data.ValueE[0], level);
        }
        
        if (data.ValueF.Count > 0)
        {
            m_BigChaserCreateProb = TableManager.GetValueFromUnionCell(data.ValueF[0], level);
        }

        if (data.ValueG.Count > 0)
        {
            m_ChaserCreateTime = TableManager.GetValueFromUnionCell(data.ValueG[0], level);
        }
    }

    public override void OnReset()
    {
        m_ChaserMaxCount = 0;
        m_ChaserCreateProb = 0;
        m_BigChaserCreateProb = 0;
        m_ChaserScale = new int[5];
        m_knifeCount = 0;
        m_ChaserCreateTime = 0;
        m_isOffsetCreateTime = false;
    }

    public override void OnStart()
    {
        base.OnStart();

        GetChaserMgr();
        if (mChaserMgr == null)
            return;
        
        mChaserMgr.OffsetChaserCreateTime(m_ChaserCreateTime);
        m_isOffsetCreateTime = true;
    }

    public override void OnFinish()
    {
        base.OnFinish();

        GetChaserMgr();
        if (mChaserMgr == null)
            return;

        if (m_isOffsetCreateTime)
        {
            mChaserMgr.OffsetChaserCreateTime(-m_ChaserCreateTime);
            m_isOffsetCreateTime = false;
        }
    }

    private Mechanism2072 mChaserMgr = null;
    private void GetChaserMgr()
    {
        if(owner == null)
            return ;

        if (mChaserMgr != null)
            return;
      
        var baseMech = owner.GetMechanism(Mechanism2072.ChaserMgrID);
        if (baseMech == null)
            return;
        
        var mechanism = baseMech as Mechanism2072;
        mChaserMgr = mechanism;
    }
}

