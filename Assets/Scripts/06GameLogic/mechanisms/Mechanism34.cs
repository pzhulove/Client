using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 集火机制
 */
public class Mechanism34 : BeMechanism
{
    protected int m_MonsterId = 0;                              //攻击的怪物ID
    protected int m_CallNameBuffId = 0;                         //点名BuffId
    protected List<int> m_BuffInfoList = new List<int>();       //需要添加的BuffInfoId
    protected int m_ForcusFireTime = 0;                         //集火时间
    protected List<int> m_RemoveBuffIdList = new List<int>();   //需要移除的BuffId
    protected int m_FocusFireNum = 0;                           //集火玩家数量

    protected int m_ListenCurrent = 0;                          //当前监听时间
    readonly protected int m_ListenInterval = 100;                      //监听时间间隔
    protected bool m_IsFocusFire = false;                       //正处于集火状态

    public Mechanism34(int mid, int lv) : base(mid, lv){}

    public override void OnReset()
    {
        m_BuffInfoList.Clear();
        m_RemoveBuffIdList.Clear();
        m_ListenCurrent = 0;
        m_IsFocusFire = false;
    }

    public override void OnInit()
    {
        m_MonsterId = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        m_CallNameBuffId = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        for (int i = 0; i < data.ValueC.Count; i++)
        {
            int buffInfoId = TableManager.GetValueFromUnionCell(data.ValueC[i],level);
            if (!m_BuffInfoList.Contains(buffInfoId))
            {
                m_BuffInfoList.Add(buffInfoId);
            }
        }

        m_ForcusFireTime = TableManager.GetValueFromUnionCell(data.ValueD[0], level);
        for (int j = 0; j < data.ValueE.Count; j++)
        {
            int buffId = TableManager.GetValueFromUnionCell(data.ValueE[j], level);
            if (!m_RemoveBuffIdList.Contains(buffId))
                m_RemoveBuffIdList.Add(buffId);
        }

        m_FocusFireNum = TableManager.GetValueFromUnionCell(data.ValueF[0],level);
    }

    public override void OnUpdate(int deltaTime)
    {
        if (m_ListenCurrent >= m_ListenInterval)
        {
            m_ListenCurrent = 0;
            if (CheckFocusFire() && !m_IsFocusFire)
            {
                m_IsFocusFire = true;
                StartFocusFire();
            }
        }
        else
        {
            m_ListenCurrent += deltaTime;
        }
    }

    //检测集火
    protected bool CheckFocusFire()
    {
        List<BeActor> callNamePlayer = new List<BeActor>();
        callNamePlayer = GetCallNamePlayer();
        return callNamePlayer.Count >= m_FocusFireNum;
    }

    //开始集火
    protected void StartFocusFire()
    {
        SetMonsterTarget();
        owner.delayCaller.DelayCall(m_ForcusFireTime, () =>
        {
            m_IsFocusFire = false;
            RemoveTarget();
            RemoveBuff();
        });
    }

    //设置怪物集火目标
    protected void SetMonsterTarget()
    {
        List<BeActor> playerList = new List<BeActor>();
        playerList = GetCallNamePlayer();

        List<BeActor> monsterList = new List<BeActor>();
        owner.CurrentBeScene.FindActorById(monsterList, m_MonsterId);
        for (int j = 0; j < monsterList.Count; j++)
        {
            var monster = monsterList[j];
            AddBuffInfoToMonster(monster);
            monster.aiManager.SetTarget(GetDistance(playerList,monster),true);
        }
    }

    //给攻击给添加Buff
    protected void AddBuffInfoToMonster(BeActor actor)
    {
        for (int i = 0; i < m_BuffInfoList.Count; i++)
        {
            actor.buffController.TryAddBuff(m_BuffInfoList[i]);
        }
    }

    //清除攻击目标锁定
    protected void RemoveTarget()
    {
        List<BeActor> monsterList = new List<BeActor>();
        owner.CurrentBeScene.FindActorById(monsterList, m_MonsterId);
        for (int j = 0; j < monsterList.Count; j++)
        {
            var monster = monsterList[j];
            monster.aiManager.SetTarget(null);
        }
    }

    //清除集火相关Buff
    protected void RemoveBuff()
    {
        //移除攻击怪物身上的buff
        List<BeActor> monsterList = new List<BeActor>();
        owner.CurrentBeScene.FindActorById(monsterList,m_MonsterId);
        for (int i = 0; i < monsterList.Count; i++)
        {
            for (int j = 0; j < m_RemoveBuffIdList.Count; j++)
            {
                if (monsterList[i].buffController.HasBuffByID(m_RemoveBuffIdList[j]) != null)
                {
                    monsterList[i].buffController.RemoveBuff(m_RemoveBuffIdList[j]);
                }
            }   
        }

        //移除玩家身上的点名buff
        List<BeActor> callNamePlayer = GetCallNamePlayer();
        for (int i = 0; i < callNamePlayer.Count; i++)
        {
            var actor = callNamePlayer[i];
            if (actor.buffController.HasBuffByID(m_CallNameBuffId) != null)
            {
                actor.buffController.RemoveBuff(m_CallNameBuffId);
            }
        }
    }

    //获取距离攻击者最新的目标
    protected BeActor GetDistance(List<BeActor> playerList, BeActor attack)
    {
        int distance = 0;
        BeActor target = null;
        for (int i = 0; i < playerList.Count; i++)
        {
            BeActor actor = playerList[i];
            VInt3 targetPos = actor.GetPosition();
            VInt3 attackPos = attack.GetPosition();
            //int xAbs = Mathf.Abs(targetPos.y - attackPos.y);
            //int yAbs = Mathf.Abs(targetPos.x - attackPos.x);
            //float currentDistance = Mathf.Sqrt(Mathf.Pow((targetPos.x - attackPos.x), 2) + Mathf.Pow((targetPos.y - attackPos.y), 2));
            int dis = (targetPos - attackPos).magnitude;
            if (i == 0)
                distance = dis;
            if (dis <= distance)
            {
                distance = dis;
                target = actor;
            }
        }
        return target;
    }

    //获取点名玩家
    protected List<BeActor> GetCallNamePlayer()
    {
        List<BattlePlayer> playerList = new List<BattlePlayer>();
        playerList = owner.CurrentBeBattle.dungeonPlayerManager.GetAllPlayers();

        List<BeActor> callNamePlayer = new List<BeActor>();
        for (int i = 0; i < playerList.Count; i++)
        {
            var actor = playerList[i].playerActor;
            if (actor.buffController.HasBuffByID(m_CallNameBuffId) != null)
            {
                if (!callNamePlayer.Contains(actor))
                {
                    callNamePlayer.Add(actor);
                }
            }
        }
        return callNamePlayer;
    }
}