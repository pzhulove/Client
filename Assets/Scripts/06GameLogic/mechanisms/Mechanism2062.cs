using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameClient;

/// <summary>
/// 选择一个玩家并冲过去
/// </summary>
public class Mechanism2062 : BeMechanism
{
    public Mechanism2062(int mid, int lv) : base(mid, lv) { }

    private int speed = 500000; //冲向目标的速度
    private int[] hurtIdArr = new int[2] { 212050, 212051 };   //对路上的敌人和控制住的敌人造成一次伤害
    private int buffId = 521644; //控制BuffId

    private BeActor target = null;  //选择的目标
    private string chainEffect = "Effects/Monster_HMZD_zhenshou/Prefab/Eff_hmzd_zhenshou_shouhulian";
    private int registerSkillPhaseId = 212054;   //技能第四阶段ID
    private bool hurtFlag = false;
    private int checkHurtId = 212052;   //检测碰撞的触发效果ID

    public override void OnInit()
    {
        base.OnInit();
        speed = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        hurtIdArr[0] = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        hurtIdArr[1] = TableManager.GetValueFromUnionCell(data.ValueB[1], level);
        buffId = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
    }

    public override void OnReset()
    {
        target = null;
        hurtFlag = false;
    }

    public override void OnStart()
    {
        base.OnStart();
        target = SelectOnePlayerByRamdom();
        ControlTarget();
        CreateChainEffect();
        handleA = OwnerRegisterEventNew(BeEventType.onCollide, RegisteHurtPlayer);
        //handleA = owner.RegisterEvent(BeEventType.onCollide, RegisteHurtPlayer);
        handleB = owner.RegisterEventNew(BeEventType.onPreSetSkillAction, RegisterSkill);
    }

    public override void OnFinish()
    {
        base.OnFinish();
        if (!hurtFlag)
        {
            //机制结束的时候自己停止移动 避免没有碰到玩家停不下来
            StopMoving();
        }
    }

    /// <summary>
    /// 控制住目标
    /// </summary>
    private void ControlTarget()
    {
        if (target == null)
            return;
        target.buffController.TryAddBuff(buffId, GlobalLogic.VALUE_10000, 60);
    }

    /// <summary>
    /// 冲向目标
    /// </summary>
    private void SetOwnerSpeed()
    {
        if (target == null)
            return;
        var ownerPos = owner.GetPosition();
        var targetPos = target.GetPosition();
        VInt3 dis = new VInt3(targetPos.x - ownerPos.x, targetPos.y - ownerPos.y, targetPos.z - ownerPos.z);
        var del = dis.NormalizeTo(speed);
        owner.SetMoveSpeedX(del.x);
        owner.SetMoveSpeedY(del.y);
        owner.SetMoveSpeedZ(del.z);
    }

    /// <summary>
    /// 监听碰到玩家
    /// </summary>
    private void RegisteHurtPlayer(BeEvent.BeEventParam param)
    {
        if (hurtFlag)
            return;
        BeActor actor = param.m_Obj as BeActor;
        int hurtId = param.m_Int;
        if (hurtId != checkHurtId)
            return;
        if (actor == null)
            return;
        if (!actor.isMainActor)
            return;
        if (actor.stateController.CanBeHit())
        {
            if (target != null && actor.GetPID() == target.GetPID())
            {
                owner.DoAttackTo(actor, hurtIdArr[1]);
            }
            else
            {
                owner.DoAttackTo(actor, hurtIdArr[0]);
            }
            
        }
        hurtFlag = true;
        StopMoving();
    }

    /// <summary>
    /// 监听技能释放
    /// </summary>
    /// <param name="args"></param>
    private void RegisterSkill(GameClient.BeEvent.BeEventParam param)
    {
        //int[] skillArr = (int[])args[0];
        if (param.m_Int != registerSkillPhaseId)
            return;
        SetOwnerSpeed();
    }

    /// <summary>
    /// 停止移动
    /// </summary>
    private void StopMoving()
    {
        owner.ClearMoveSpeed();
        if(target != null)
        {
            target.buffController.RemoveBuff(buffId);
        }

        if (owner.GetStateGraph() != null)
        {
            ((BeActorStateGraph)owner.GetStateGraph()).ExecuteNextPhaseSkill();
        }
        
        ClearChainEffect();
    }

    /// <summary>
    /// 随机选择一个玩家
    /// </summary>
    private BeActor SelectOnePlayerByRamdom()
    {
        if (owner.CurrentBeBattle == null)
            return null;
        if (owner.CurrentBeBattle.dungeonPlayerManager == null)
            return null;
        BeActor actor = null;
        List<BattlePlayer> list = GamePool.ListPool<BattlePlayer>.Get();
        var battleList = owner.CurrentBeBattle.dungeonPlayerManager.GetAllPlayers();
        for (int i = 0; i < battleList.Count; ++i)
        {
            if (!owner.CurrentBeBattle.dungeonPlayerManager.IsPlayerDeadByBattlePlayer(battleList[i]))
            {
                list.Add(battleList[i]);
            }
        }
        if (list != null && list.Count > 0)
        {
            int random = FrameRandom.InRange(0, list.Count);
            actor = list[random].playerActor;
        }
        GamePool.ListPool<BattlePlayer>.Release(list);
        return actor;
    }

    /// <summary>
    /// 创建特效连线
    /// </summary>
    private void CreateChainEffect()
    {
#if !LOGIC_SERVER
        if (target == null)
            return;
        owner.m_pkGeActor.CreateChainEffect(target, chainEffect);
#endif
    }

    /// <summary>
    /// 清除特效连线
    /// </summary>
    private void ClearChainEffect()
    {
#if !LOGIC_SERVER
        owner.m_pkGeActor.ClearChainEffect();
#endif
    }
}
