using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameClient;
using System;

/// <summary>
///流星气泡
/// </summary>

public class Mechanism2064 : BeMechanism
{
    public Mechanism2064(int mid, int lv) : base(mid, lv) { }

    private int totalTime = 40000;  //总时间
    private int entityResId = 0;    //流星实体ID
    private int[] totalQteCountArr = new int[3];  //总数量
    private int[] skillIdArr = new int[2];  //切换的技能ID(成功|失败)
    private VInt[] radiusArr = new VInt[2];   //判断实体与自己的距离(内圆半径|外圆半径)
    private int reduceTime = 0; //缩减时间(惩罚时间)

    private VInt3[] posArr = new VInt3[3];      //气泡包裹玩家位置
    private int commandSkillId = 21080;  //用于帧同步的技能ID
    private int curTime = 0;    //当前时间
    private int curTotalCount = 0;  //当前需要完成的总数量
    private int curQteCount = 0;    //当前完成QTE的数量

    private int monsterId = 80320011;  //克茜拉美梦-流星气泡怪物
    private bool successFlag = false;   //成功标志
    private List<BeActor> playerActorList = new List<BeActor>();
    private int strainBuffId = 98;      //束缚buffId

#if !LOGIC_SERVER
    private VInt3 localActorPos;
    private Dictionary<int, VInt3> playerPosDic = new Dictionary<int, VInt3>();
    private BeActor localActor = null;  //本地玩家 
#endif
    private TeamDungeonBattleFrame frame = null;
    private List<IBeEventHandle> handleList = new List<IBeEventHandle>();
    private bool finishFlag = false;

    public delegate void Del();
    private Del attackDel = null;

    public override void OnInit()
    {
        base.OnInit();
        totalTime = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        entityResId = TableManager.GetValueFromUnionCell(data.ValueB[0], level);

        totalQteCountArr[0] = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
        totalQteCountArr[1] = TableManager.GetValueFromUnionCell(data.ValueC[1], level);
        totalQteCountArr[2] = TableManager.GetValueFromUnionCell(data.ValueC[2], level);

        skillIdArr[0] = TableManager.GetValueFromUnionCell(data.ValueD[0], level);
        skillIdArr[1] = TableManager.GetValueFromUnionCell(data.ValueD[1], level);

        radiusArr[0] = VInt.NewVInt(TableManager.GetValueFromUnionCell(data.ValueE[0], level), GlobalLogic.VALUE_1000);
        radiusArr[1] = VInt.NewVInt(TableManager.GetValueFromUnionCell(data.ValueE[1], level), GlobalLogic.VALUE_1000);

        reduceTime = TableManager.GetValueFromUnionCell(data.ValueF[0], level);
    }

    public override void OnReset()
    {
        posArr = new VInt3[3];
        curTime = 0;
        curTotalCount = 0;
        curQteCount = 0;

        successFlag = false;
        playerActorList.Clear();

#if !LOGIC_SERVER
        localActorPos = VInt3.zero;
        playerPosDic.Clear();
        localActor = null;
#endif
        frame = null;
        handleList.Clear();
        finishFlag = false;

        attackDel = null;
    }

    public override void OnStart()
    {
        base.OnStart();
        InitStartPos();
        InitPlayerData();
        InitTime();
        HideJoystickAndSkill();
    }

    public override void OnUpdate(int deltaTime)
    {
        base.OnUpdate(deltaTime);
        HideJoystickAndSkill();
        UpdateProgressBar(deltaTime);
    }
    
    public override void OnFinish()
    {
        base.OnFinish();
        JudgeResult();
        ClearHandles();
        RemoveStrainBuff();
        HideJoystickAndSkill(true);
        CloseSpecialFrame();
        ClearMonsters();
    }

    /// <summary>
    /// 初始化玩家初始位置
    /// </summary>
    private void InitStartPos()
    {
        posArr[0] = new VInt3(25000, 30000, 0);
        posArr[1] = new VInt3(-15000, 30000, 0);
        posArr[2] = new VInt3(65000, 30000, 0);
    }

    /// <summary>
    /// 初始化玩家位置
    /// </summary>
    private void InitPlayerData()
    {
        if (owner.CurrentBeBattle == null)
            return;
        if (owner.CurrentBeBattle.dungeonPlayerManager == null)
            return;
        List<BattlePlayer> list = owner.CurrentBeBattle.dungeonPlayerManager.GetAllPlayers();
        int aliveCount = 0;
        for (int i = 0; i < list.Count; i++)
        {
            var player = list[i];
            if (player == null)
                continue;
            var playerActor = player.playerActor;
            if (playerActor == null)
                continue;
            if (!playerActor.IsDead())
            {
                aliveCount++;
            }
            VInt3 pos = posArr[i];
            playerActor.SetPosition(pos);
            InitSummonMonster(playerActor, pos);
            InitLocalPlayerPos(playerActor);

            var actor = list[i].playerActor;
            if (actor == null)
                continue;
            actor.sgSwitchStates(new BeStateData((int)ActionState.AS_IDLE));
            playerActorList.Add(actor);
            actor.buffController.TryAddBuff(strainBuffId, GlobalLogic.VALUE_100000, 60);

            var handle1 = actor.RegisterEventNew(BeEventType.onSyncDungeonOperation, OnSyncDungeonOperation);
            var handle2 = actor.RegisterEventNew(BeEventType.onHit, RegisterBeHit);
            var handle3 = actor.RegisterEventNew(BeEventType.onMagicGirlMonsterRestore, OnMagicGirlMonsterRestore);

            handleList.Add(handle1);
            handleList.Add(handle2);
            handleList.Add(handle3);
            InitLocalActorAttack(actor);
            actor.SetAttackButtonState(ButtonState.RELEASE);

#if !LOGIC_SERVER
            playerPosDic.Add(playerActor.GetPID(), pos);
#endif
        }
        curTotalCount = totalQteCountArr[aliveCount - 1];
        RefreshCompleteNum();
    }

    /// <summary>
    /// 创建流星召唤怪物
    /// </summary>
    private void InitSummonMonster(BeActor playerActor,VInt3 pos)
    {
        var monster = owner.CurrentBeScene.SummonMonster(monsterId, pos, owner.m_iCamp, owner, false, 60);
        //死亡玩家召唤的怪物暂停AI
        if (playerActor.IsDead() && monster != null)
        {
            monster.aiManager.Stop();
            var handle = playerActor.RegisterEventNew(BeEventType.onReborn, args =>
            {
                var actor = args.m_Obj as BeActor;
                if (actor != null)
                {
                    actor.buffController.RemoveBuff(strainBuffId);
                    actor.buffController.TryAddBuff(strainBuffId, GlobalLogic.VALUE_100000, 60);
                }
                monster.aiManager.Start();
            });
            handleList.Add(handle);
        }
    }

    /// <summary>
    /// 初始化当前时间
    /// </summary>
    private void InitTime()
    {
        curTime = totalTime;
    }

    /// <summary>
    /// 监听玩家被击
    /// </summary>
    /// <param name="args"></param>
    private void RegisterBeHit(BeEvent.BeEventParam args)
    {
        ReduceTime();
    }

    /// <summary>
    /// 召唤师觉醒恢复
    /// </summary>
    /// <param name="args"></param>
    private void OnMagicGirlMonsterRestore(BeEvent.BeEventParam args)
    {
        var actor = (BeActor)args.m_Obj;
        if (actor == null)
            return;
#if !LOGIC_SERVER
        if (actor.isLocalActor)
        {
            localActor = actor;
        }
#endif
        actor.buffController.RemoveBuff(strainBuffId);
        actor.buffController.TryAddBuff(strainBuffId, GlobalLogic.VALUE_100000, 60);
    }

    /// <summary>
    /// 监听地下城同步消息
    /// </summary>
    /// <param name="args"></param>
    private void OnSyncDungeonOperation(BeEvent.BeEventParam args)
    {
        int skillId = args.m_Int;
        int operationActorId = args.m_Int2;
        int pid = args.m_Int3;
        if (skillId != commandSkillId)
            return;
        if (pid == 0)
        {
            //操作失败
            ReduceTime();
            PlayAudio();
            PlayEffect("Effects/UI/Prefab/EffUI_tuanben/Prefab/Eff_tuanben_wanchengdu_jianmiao", operationActorId, true);
            PlayEffect("Effects/Monster_TB02/Prefab/Eff_Monster_TB02_kexila_hongquan", operationActorId);
            PlayUIEffect(false);
        }
        else
        {
            DoProjectilDie(pid);
            UpdateCompleteCount();
            PlayAudio(true);
            PlayEffect("Effects/UI/Prefab/EffUI_tuanben/Prefab/Eff_tuanben_wanchengdu_jiayi", operationActorId,true);
            PlayEffect("Effects/Monster_TB02/Prefab/Eff_Monster_TB02_kexila_lvquan", operationActorId);
            PlayUIEffect(true);
        }
    }

    /// <summary>
    /// 实体死亡
    /// </summary>
    private void DoProjectilDie(int pid)
    {
        if (owner.CurrentBeScene == null)
            return;
        if (owner.CurrentBeScene.GetEntityByPID(pid) == null)
            return;
        var projectile = owner.CurrentBeScene.GetEntityByPID(pid) as BeProjectile;
        if (projectile == null)
            return;
        if (projectile.IsDead())
            return;
        projectile.DoDie();
        PlayEffectBase("Effects/Monster_TB02/Prefab/Eff_Monster_TB02_kexila_suipianbaozha", projectile.GetPosition().vec3);
    }

    /// <summary>
    /// 减少时间
    /// </summary>
    private void ReduceTime()
    {
        curTime -= reduceTime;
        AttackFail(reduceTime);
    }

    /// <summary>
    /// 刷新时间读条数据
    /// </summary>
    private void UpdateProgressBar(int deltaTime)
    {
        if (curTime <= 0)
        {
            //进度条结束时 没有完成QTE任务 需要召唤三个流星将气泡中的玩家杀死
            SwitchSkill(false);
        }
        else
        {
            curTime -= deltaTime;
            RefreshProgressUI(deltaTime);
        }
    }

    /// <summary>
    /// 更新QTE操作完成次数
    /// </summary>
    private void UpdateCompleteCount()
    {
        curQteCount++;
        RefreshCompleteNum();

        if (curQteCount >= curTotalCount)
        {
            //达到总数量 召唤一个流星砸向玩家
            successFlag = true;
            SwitchSkill(true);
        }
    }

    /// <summary>
    /// 发送同步操作消息
    /// </summary>
    private void SendSyncOperationCommand(int pid)
    {
#if !LOGIC_SERVER
        if (localActor == null)
            return;
        InputManager.CreateSkillDoattackFrameCommand(commandSkillId, localActor.GetPID(), pid);
#endif
    }

    /// <summary>
    /// 判断最终结果
    /// </summary>
    private void JudgeResult()
    {
        if (successFlag)
            return;
        SwitchSkill(false);
    }

    /// <summary>
    /// 切换技能
    /// </summary>
    private void SwitchSkill(bool isSuccess)
    {
        if (finishFlag)
           return;
        finishFlag = true;
        owner.CancelSkill(owner.GetCurSkillID());
        owner.UseSkill(isSuccess ? skillIdArr[0] : skillIdArr[1], true);
    }

    /// <summary>
    /// 清除召唤流星的怪物
    /// </summary>
    private void ClearMonsters()
    {
        if (owner.CurrentBeScene == null)
            return;
        List<BeActor> list = GamePool.ListPool<BeActor>.Get();
        owner.CurrentBeScene.FindActorById2(list, monsterId);
        for (int i = 0; i < list.Count; i++)
        {
            list[i].DoDead();
        }
        GamePool.ListPool<BeActor>.Release(list);
    }

    /// <summary>
    /// 移除束缚Buff
    /// </summary>
    private void RemoveStrainBuff()
    {
        for (int i = 0; i < playerActorList.Count; i++)
        {
            var actor = playerActorList[i];
            actor.buffController.RemoveBuff(strainBuffId);
            actor.buffController.TryAddBuff(63, 2000, 60);
        }
    }

    /// <summary>
    /// 清除事件
    /// </summary>
    private void ClearHandles()
    {
        for (int i = 0; i < handleList.Count; i++)
        {
            if (handleList[i] != null)
            {
                handleList[i].Remove();
                handleList[i] = null;
            }
        }
        handleList.Clear();
    }

    #region 表现相关

    /// <summary>
    /// 记录本地玩家的初始位置
    /// </summary>
    /// <param name="playerActor"></param>
    private void InitLocalPlayerPos(BeActor playerActor)
    {
#if !LOGIC_SERVER
        if (playerActor.isLocalActor)
        {
            //将判定坐标设为玩家的中心点
            localActorPos = playerActor.GetPosition();
            localActorPos.z += 7900;
        }
#endif
    }

    /// <summary>
    /// 初始化本地玩家操作
    /// </summary>
    private void InitLocalActorAttack(BeActor actor)
    {
#if !LOGIC_SERVER
        if (actor.isLocalActor)
        {
            localActor = actor;
            if (frame == null)
            {
                frame = ClientSystemManager.instance.OpenFrame<TeamDungeonBattleFrame>(FrameLayer.Middle) as TeamDungeonBattleFrame;
            }

            if (frame != null)
            {
                frame.InitLiuXingData(totalTime);
                attackDel = OnDungeonAttack;
                frame.SetLiuXingAttackBtn(attackDel);
                frame.SetLiuXingCompleteNum(0, curTotalCount);
            }
        }
#endif
    }

    /// <summary>
    /// 监听本地玩家操作
    /// </summary>
    private void OnDungeonAttack()
    {
#if !LOGIC_SERVER
        if (localActor == null || localActor.IsDead())
            return;
        bool successFlag = false;
        List<BeEntity> list = GamePool.ListPool<BeEntity>.Get();
        owner.CurrentBeScene.GetEntitys2(list);
        for (int i = 0; i < list.Count; i++)
        {
            BeProjectile projectile = list[i] as BeProjectile;
            if (projectile == null)
                continue;
            if (projectile.m_iResID != entityResId)
                continue;
            int dis = (projectile.GetPosition() - localActorPos).magnitude;
            if (dis >= radiusArr[0] && dis <= radiusArr[1])
            {
                //同时右多个流星处于范围内的时候只销毁一个流星
                SendSyncOperationCommand(projectile.GetPID());
                successFlag = true;
                break;
            }
        }

        //操作失败同步消息
        if (!successFlag)
        {
            SendSyncOperationCommand(0);
        }
        GamePool.ListPool<BeEntity>.Release(list);
#endif
    }

    /// <summary>
    /// 设置摇杆和技能按钮
    /// </summary>
    private void HideJoystickAndSkill(bool isRestore = false)
    {
#if !LOGIC_SERVER
        InputManager.instance.SetVisible(isRestore);
#endif
    }

    /// <summary>
    /// 关闭特殊UI界面
    /// </summary>
    private void CloseSpecialFrame()
    {
#if !LOGIC_SERVER
        if (frame == null)
            return;
        frame.Close();
        frame = null;
#endif
    }

    /// <summary>
    /// 刷新时间读条UI
    /// </summary>
    private void RefreshProgressUI(int reduceTime)
    {
#if !LOGIC_SERVER
        //刷新进度条UI
        if (frame != null)
        {
            frame.RefreshLiuXingTime(reduceTime);
        }
#endif
    }

    /// <summary>
    /// 操作失败
    /// </summary>
    private void AttackFail(int reduceTime)
    {
#if !LOGIC_SERVER
        //刷新进度条UI
        if (frame != null)
        {
            frame.AttackFial(reduceTime);
        }
#endif
    }

    /// <summary>
    /// 刷新完成次数
    /// </summary>
    private void RefreshCompleteNum()
    {
#if !LOGIC_SERVER
        if (frame != null)
        {
            frame.SetLiuXingCompleteNum(curQteCount, curTotalCount);
        }
#endif
    }

    /// <summary>
    /// 播放成功和失败的特效
    /// </summary>
    private void PlayEffect(string path,int operationPid,bool offsetFlag = false)
    {
#if !LOGIC_SERVER
        if (!playerPosDic.ContainsKey(operationPid))
            return;
        VInt3 pos = playerPosDic[operationPid];
        pos.z += 7900;
        if (offsetFlag)
        {
            pos.y -= 25000;
            pos.z += 11000;
        }
        PlayEffectBase(path, pos.vec3);
#endif
    }

    private void PlayAudio(bool isSuccess = false)
    {
#if !LOGIC_SERVER
        int audioId = isSuccess ? 5014 : 5015;
        owner.CurrentBeBattle.PlaySound(audioId);

#endif
    }

    /// <summary>
    /// 播放特效基础函数
    /// </summary>
    private void PlayEffectBase(string path,Vec3 pos)
    {
#if !LOGIC_SERVER
        if (owner.CurrentBeScene == null)
            return;
        if (owner.CurrentBeScene.currentGeScene == null)
            return;
        owner.CurrentBeScene.currentGeScene.CreateEffect(path, 0.267f, pos);
#endif
    }

    /// <summary>
    /// 播放UI特效
    /// </summary>
    private void PlayUIEffect(bool isSuccess)
    {
#if !LOGIC_SERVER
        if (frame == null)
            return;
        frame.PlayAttackResultEffect(isSuccess);
#endif
    }

    #endregion
}
