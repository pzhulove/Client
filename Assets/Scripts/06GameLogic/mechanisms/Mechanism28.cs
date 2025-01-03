using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 老鼠移动 然后重新聚到一起
 */
public class Mechanism28 : BeMechanism
{
    protected enum RebornState
    {
        Start,
        Reborning,
        End,
    }

    protected int m_MonsterId = 2630011;
    protected int m_DelayTime = 5000;           //延时时间
    protected int m_RebornTime = 6000;          //复活时间
    readonly protected string m_Text = "复活中...";
    protected float m_HpRate = 100.0f;          //复活回复血量

    protected RebornState m_RebornState = RebornState.Start;       //皮特复活状态
    protected int m_MonsterCount = 0;           //老鼠数量

    protected List<BeActor> m_MonsterList = new List<BeActor>();        //用于存储怪物
    protected List<VInt3> m_TargetPos = new List<VInt3>();                //用于记录目标位置
    protected bool m_MoveBackFlag = false;

    public Mechanism28(int mid, int lv): base(mid, lv){}

    public override void OnReset()
    {
        m_MonsterCount = 0;
        m_MonsterList.Clear();
        m_TargetPos.Clear();
        m_MoveBackFlag = false;
    }

    public override void OnInit()
    {
        m_RebornState = RebornState.Start;
        m_MonsterId = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        m_DelayTime = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        m_RebornTime = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
        m_HpRate = TableManager.GetValueFromUnionCell(data.ValueD[0], level);
    }

    public override void OnStart()
    {
        owner.StartSpellBar(eDungeonCharactorBar.Buff, m_RebornTime, true, m_Text);
        owner.CurrentBeScene.FindActorById(m_MonsterList, m_MonsterId);
        m_MonsterCount = m_MonsterList.Count;

        BeActor actor = null;
        for (int i = 0; i < m_MonsterList.Count; i++)
        {
            actor = m_MonsterList[i];
            actor.buffController.TryAddBuff((int)GlobalBuff.INVINCIBLE, GlobalLogic.VALUE_1500);
            VInt3 targetPos = _SetFakeMonsetPos(i, actor);
            m_TargetPos.Add(targetPos);
        }

        owner.delayCaller.DelayCall(m_DelayTime, () =>
        {
            _MoveBack();
        });

        owner.delayCaller.DelayCall(m_RebornTime, () =>
        {
            List<BeActor> aliveMonster = GamePool.ListPool<BeActor>.Get();
            owner.CurrentBeScene.FindActorById(aliveMonster, m_MonsterId);
            if (aliveMonster.Count > 0)
            {
                m_RebornState = RebornState.Reborning;
                _RemoveMonster();
                //复活并且回血
                int hpChange = (int)(owner.GetEntityData().battleData.maxHp * m_HpRate / (float)(GlobalLogic.VALUE_1000) * aliveMonster.Count / m_MonsterCount);
                owner.DoHeal(hpChange, true);
                owner.protectManager.SetEnable(true);
                if (owner.HasAction("Getup"))
                {
                    owner.PlayAction("Getup");
                }
                owner.StopSpellBar(eDungeonCharactorBar.Buff);
            }
            else
            {
                owner.StopSpellBar(eDungeonCharactorBar.Buff);
                owner.buffController.RemoveBuff((int)GlobalBuff.INVINCIBLE);
                owner.DoDead();
            }
            GamePool.ListPool<BeActor>.Release(aliveMonster);
        });
    }

    public override void OnFinish()
    {
        m_MonsterList.Clear();
        m_RebornState = RebornState.End;
    }

    //将怪物杀死
    protected void _RemoveMonster()
    {
        for (int i = 0; i < m_MonsterList.Count; i++)
        {
            if (m_MonsterList[i] != null)
            {
                if (!m_MonsterList[i].IsDead())
                {
                    m_MonsterList[i].DoDead();
                }
            }
        }
    }

    protected VInt3 _SetFakeMonsetPos(int index, BeActor actor)
    {
        BeScene scene = owner.CurrentBeScene;
        int rowCount = m_MonsterCount / 2;
        int logicWidth = scene.logicXSize.y - scene.logicXSize.x;
        int logicHeight = scene.logicZSize.y - scene.logicZSize.x;
        int xPos = 0;
        int yPos = 0;
        int num = index + 1;
        if (num % 2 != 0)
        {
            xPos = scene.logicXSize.x + logicWidth * VFactor.NewVFactor((num + 1) / 2, (long)(rowCount + 1));
            yPos = scene.logicZSize.x + logicHeight * VFactor.NewVFactor(1, (long)6);
        }
        else
        {
            xPos = scene.logicXSize.x + logicWidth * VFactor.NewVFactor(num / 2, (long)(rowCount + 1));
            yPos = scene.logicZSize.x + logicHeight * VFactor.NewVFactor(5, (long)6);
        }
        VInt3 actorPos = new VInt3(xPos, yPos, actor.GetPosition().z);
        return actorPos;

    }

    //移动到目标点
    private void _moveTo(BeActor actor, VInt3 targetPos)
    {
        if (actor == null || actor.IsDead())
            return;
        VInt3 mPos = targetPos - actor.GetPosition();
        actor.ResetMoveCmd();

        if (m_MoveBackFlag)
        {
            actor.speedConfig = new VInt3(7 * VInt.one.i, 7 * VInt.one.i, 0);
        }
        else
        {
            actor.speedConfig = new VInt3(4 * VInt.one.i, 4 * VInt.one.i, 0);
        }

        if (mPos.x > VInt.half)
        {
            actor.ModifyMoveCmd((int)CommandMove.COMMAND_MOVE_X, true);
        }
        else if (mPos.x < -VInt.half)
        {
            actor.ModifyMoveCmd((int)CommandMove.COMMAND_MOVE_X_NEG, true);
        }

        if (mPos.y > VInt.half)
        {
            actor.ModifyMoveCmd((int)CommandMove.COMMAND_MOVE_Y, true);
        }
        else if (mPos.y < -VInt.half)
        {
            actor.ModifyMoveCmd((int)CommandMove.COMMAND_MOVE_Y_NEG, true);
        }
    }

    //返回
    protected void _MoveBack()
    {
        List<BeActor> aliveMonster = GamePool.ListPool<BeActor>.Get();
        owner.CurrentBeScene.FindActorById(aliveMonster, m_MonsterId);
        for (int i = 0; i < aliveMonster.Count; i++)
        {
            if (aliveMonster[i] != null)
            {
                if (!aliveMonster[i].IsDead())
                {
                    //返回并且添加无敌buff
                    aliveMonster[i].buffController.TryAddBuff((int)GlobalBuff.INVINCIBLE, 5000);
                }
            }
        }
        m_MoveBackFlag = true;
        GamePool.ListPool<BeActor>.Release(aliveMonster);
    }

    private void _Move(int delta)
    {
        if (m_TargetPos.Count == m_MonsterList.Count)
        {
            for (int i = 0; i < m_MonsterList.Count; i++)
            {
                if (!m_MoveBackFlag)
                {
                    _moveTo(m_MonsterList[i], m_TargetPos[i]);
                }
                else
                {
                    _moveTo(m_MonsterList[i], owner.GetPosition());
                }
            }
        }
    }

    private bool _HaveMonsterAlive()
    {
        if (m_MonsterList.Count <= 0)
            return true;
        for (int i = 0; i < m_MonsterList.Count; i++)
        {
            if (!m_MonsterList[i].IsDead())
            {
                return true;
            }
        }
        return false;
    }

    public override void OnUpdate(int delta)
    {
        base.OnUpdate(delta);
        if (m_RebornState == RebornState.Start)
        {
            if (!_HaveMonsterAlive())
            {
                owner.StopSpellBar(eDungeonCharactorBar.Buff);
                owner.buffController.RemoveBuff((int)GlobalBuff.INVINCIBLE);
                owner.DoDead();
            }
        }
        _Move(delta);
    }
}
