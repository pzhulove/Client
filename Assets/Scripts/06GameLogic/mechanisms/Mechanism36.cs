using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 赛车机制
 */
public class Mechanism36 : BeMechanism
{
    protected int m_ActorId = 0;            //召唤的怪物ID
    protected int m_AreaNum = 0;            //平分的区域数量
    protected int m_SummonWaveNum = 0;      //召唤波数
    protected int m_SummonAreaNum = 0;      //召唤区域数量
    protected int m_SummonNum = 0;          //一个区域一波的数量
    protected int m_SummonTimeDelay = 0;    //每一波的召唤时间间隔
    protected VInt m_EntitySpeed = 0;        //召唤出来的实体的速度

    protected List<BeActor> m_SummonMonsterList = new List<BeActor>();

    readonly protected int m_TimeAcc = 500;          //每一个怪物的召唤时间间隔
    //protected List<VInt3> m_AreaBornPos = new List<VInt3>();  //每个区域的出生点位置

    public Mechanism36(int mid, int lv) : base(mid, lv){}
    
    public override void OnReset()
    {
        m_SummonMonsterList.Clear();

    }
    public override void OnInit()
    {
        m_ActorId = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        m_AreaNum = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        m_SummonWaveNum = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
        m_SummonAreaNum = TableManager.GetValueFromUnionCell(data.ValueD[0], level);
        m_SummonNum = TableManager.GetValueFromUnionCell(data.ValueE[0], level);
        m_SummonTimeDelay = TableManager.GetValueFromUnionCell(data.ValueF[0], level);
        //m_EntitySpeed = TableManager.GetValueFromUnionCell(data.ValueG[0], level);
        m_EntitySpeed = VInt.NewVInt(TableManager.GetValueFromUnionCell(data.ValueG[0], level), GlobalLogic.VALUE_1000);
    }
    public override void OnStart()
    {
        SummonMonster();
    }

    public override void OnUpdate(int deltaTime)
    {
        if (m_SummonMonsterList.Count > 0)
        {
            for (int i = 0; i < m_SummonMonsterList.Count; i++)
            {
                if (m_SummonMonsterList[i] != null)
                {
                    Move(m_SummonMonsterList[i]);
                }
            }
        }
    }

    protected void Move(BeActor actor)
    {
        if (actor.IsDead())
            return;
        actor.ResetMoveCmd();
        actor.speedConfig = new VInt3(m_EntitySpeed.i, m_EntitySpeed.i, 0);
        if (!actor.GetFace())
        {
            actor.ModifyMoveCmd((int)CommandMove.COMMAND_MOVE_X, true);
        }
        else
        {
            actor.ModifyMoveCmd((int)CommandMove.COMMAND_MOVE_X_NEG, true);
        }
    }

    //召唤怪物
    protected void SummonMonster()
    {
        bool ownerFace = owner.GetFace();
        //根据波数设置不同的延时召唤
        for (int i = 0; i < m_SummonWaveNum; i++) 
        {
            int time = i;
            owner.delayCaller.DelayCall((m_SummonTimeDelay + m_SummonTimeDelay * time), () =>
            {
                if (isRunning)
                    SummonOneWave(ownerFace);
            });
        }  
    }

    //召唤一波多个区域
    protected void SummonOneWave(bool ownerFace)
    {
        List<VInt3> selectSummonPos = GetSummonPos(ownerFace);
        for (int i = 0; i < selectSummonPos.Count; i++)
        {
            SummonOneArea(selectSummonPos[i], ownerFace);
        }
    }

    //召唤一个区域
    protected void SummonOneArea(VInt3 bornPos,bool face)
    {
        VInt3 summonPos = owner.GetPosition();
        for (int i = 0; i < m_SummonNum; i++)
        {
            owner.delayCaller.DelayCall(m_TimeAcc * i, () => 
            {
                if (isRunning)
                {
                    BeActor actor = owner.CurrentBeScene.CreateMonster(m_ActorId + level * 100, false, null, level, owner.GetCamp());
                    actor.aiManager.Stop();
                    actor.SetPosition(bornPos, true);
                    actor.SetRestrainPosition(false);
                    actor.stateController.SetAbilityEnable(BeAbilityType.BLOCK, false);
                    actor.SetFace(face);
                    m_SummonMonsterList.Add(actor);
                }
            });
        }
    }

    //获取需要召唤怪物的坐标点
    protected List<VInt3> GetSummonPos(bool face)
    {
        List<VInt3> areaPos = GetAreaBorn(face); 
        int areaPosTotalNum = areaPos.Count;
        List<VInt3> selectAreaPos = new List<VInt3>();
        for (int i = 0; i < m_SummonAreaNum; i++)
        {
            int random = FrameRandom.InRange(0, areaPos.Count);
            selectAreaPos.Add(areaPos[random]);
            areaPos.Remove(areaPos[random]);
        }
        return selectAreaPos;
    }

    //获取怪物的召唤位置
    protected List<VInt3> GetAreaBorn(bool face)
    {
        List<VInt3> areaPosList = new List<VInt3>();
        areaPosList.Clear();
        var scene = owner.CurrentBeScene;
        int logicHeight = scene.logicZSize.y - scene.logicZSize.x;
        int areaSpace = logicHeight/m_AreaNum;
        VInt3 pos = VInt3.zero;
        int x = face? scene.logicXSize.y : scene.logicXSize.x;
        for (int i = 0; i < m_AreaNum; i++)
        {
            if (i == 0)
            {
                pos = new VInt3(x, scene.logicZSize.x + areaSpace /2, owner.GetPosition().z);
            }
            else
            {
                pos = new VInt3(x, scene.logicZSize.x + areaSpace * i + areaSpace / 2, owner.GetPosition().z);
            }
            
            areaPosList.Add(pos);
        }
        return areaPosList;
    }

    public override void OnFinish()
    {
        
    }
}