using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
阵鬼召唤机制
 */

public class Mechanism39 : BeMechanism
{
    protected int m_SummonTimeAcc = 0;                          //召唤时间间隔
    protected List<int> m_SummonNumList = new List<int>();      //召唤数量区间
    protected VInt m_Radius = 3700;                             //法阵范围半径
    protected int m_SummonEntityId = 0;                         //召唤实体ID
    protected int m_AttachBuffIdPve = 0;                        //Pve附着监听BuffId
    protected int m_AttachBuffIdPvp = 0;                        //Pvp附着监听BuffId
    protected int m_AttachMaxNum = 0;                           //最大附着数量
    protected VInt m_SummonRadius = 500;                        //在目标怪物周围召唤瘟疫的半径

    readonly protected int m_SummonMax = 10;                             //瘟疫最大召唤数量
    protected int m_CurrentTimeAcc = 0;                         //当前的时间间隔

    public Mechanism39(int mid, int lv) : base(mid, lv) {}

    public override void OnReset()
    {
        m_SummonNumList.Clear();
        m_CurrentTimeAcc = 0; 
    }

    public override void OnInit()
    {
        m_SummonTimeAcc = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        if (data.ValueB.Count > 0)
        {
            for (int i = 0; i < data.ValueB.Count; i++)
            {
                int num = TableManager.GetValueFromUnionCell(data.ValueB[i], level);
                m_SummonNumList.Add(num);
            }
        }
        m_Radius = VInt.NewVInt(TableManager.GetValueFromUnionCell(data.ValueC[0], level), GlobalLogic.VALUE_1000);
        m_SummonEntityId = TableManager.GetValueFromUnionCell(data.ValueD[0], level);
        m_AttachBuffIdPve = TableManager.GetValueFromUnionCell(data.ValueE[0], level);
        m_AttachBuffIdPvp = TableManager.GetValueFromUnionCell(data.ValueE[1], level);
        m_AttachMaxNum = TableManager.GetValueFromUnionCell(data.ValueF[0], level);
        m_SummonRadius = VInt.NewVInt(TableManager.GetValueFromUnionCell(data.ValueG[0], level), GlobalLogic.VALUE_1000);
    }

    public override void OnStart()
    {
        var r = GlobalLogic.VALUE_1000;
        for (int i = 0; i < owner.MechanismList.Count; i++)
        {
            var m = owner.MechanismList[i] as Mechanism121;
            if (m != null)
                r += m.radiusRate;
        }
        m_Radius = m_Radius.i * VFactor.NewVFactor(r, GlobalLogic.VALUE_1000);

        SelectActorSummon();
    }

    public override void OnUpdate(int deltaTime)
    {
        base.OnUpdate(deltaTime);
        if (m_CurrentTimeAcc >= m_SummonTimeAcc)
        {
            m_CurrentTimeAcc = 0;
            SelectActorSummon();
        }
        else
        {
            m_CurrentTimeAcc += deltaTime;
        }
    }

    //获取瘟疫怪物数量
    protected int GetEntityCount()
    {
        List<BeEntity> entityList = owner.CurrentBeScene.GetEntities();
        int count = 0;
        if (entityList.Count > 0)
        {
            for (int i = 0; i < entityList.Count; i++)
            {
                if (entityList[i].m_iResID == m_SummonEntityId && entityList[i].GetOwner() == owner)
                {
                    count++;
                }
            }
        }
        return count;
    }

    //在范围内选择怪物 在怪物旁边召唤
    protected void SelectActorSummon()
    {
        if (GetEntityCount() >= m_SummonMax)
            return;
        List<BeActor> rangeActorList = GamePool.ListPool<BeActor>.Get();
        owner.CurrentBeScene.FindActorInRange(rangeActorList, owner.GetPosition(), m_Radius);
        for (int i = 0; i < rangeActorList.Count; i++)
        {
            int attachBuffId = BattleMain.IsModePvP(battleType) ? m_AttachBuffIdPvp : m_AttachBuffIdPve;
            int buffCount = rangeActorList[i].buffController.GetBuffCountByID(attachBuffId);
            if (buffCount < m_AttachMaxNum && rangeActorList[i].GetCamp()!=owner.GetCamp())
            {
                SummonMonster(rangeActorList[i]);
                break;
            }
        }
        GamePool.ListPool<BeActor>.Release(rangeActorList);
    }

    //在范围内怪物旁边召唤邪魂
    protected void SummonMonster(BeActor actor)
    {
        int summonNum = 0;
        if (m_SummonNumList.Count == 1)
        {
            summonNum = m_SummonNumList[0];
        }
        else
        {
            summonNum = FrameRandom.InRange(m_SummonNumList[0], m_SummonNumList[1]+1);
        }
        for (int i = 0; i < summonNum; i++)
        {
            VInt3 pos = owner.CurrentBeScene.GetLogicPosInRange(actor, m_SummonRadius.i);
            owner.AddEntity(m_SummonEntityId, pos, level);
        } 
    }

    public override void OnFinish()
    {
        List<BeEntity> allEntity = owner.CurrentBeScene.GetEntities();
        if (allEntity.Count <= 0)
            return;
        for (int i = 0; i < allEntity.Count; i++)
        {
            if (allEntity[i].m_iResID == m_SummonEntityId && allEntity[i]!=null)
            {
                allEntity[i].OnRemove();
            }
        }
            
    }
}