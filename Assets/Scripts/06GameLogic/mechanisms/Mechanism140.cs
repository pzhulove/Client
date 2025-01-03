using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//驱魔师白虎机制
public class Mechanism140 : BeMechanism
{
    public Mechanism140(int mid, int lv) : base(mid, lv) { }

    public int[] createCDArr = new int[4] { 1000, 667, 167, 0 }; //发射实体的CD
    protected VInt entitySpeed = 60000;      //实体发射速度
    protected int[] addAttackValueArr = new int[4] { 0, 900, 1200, 1500 };  //伤害增长

    readonly protected int entityId = 63609;         //发射的实体ID
    readonly protected int[] monsterIdArr = new int[2] { 93060031, 93000034 };
    readonly protected int hurtId = 36092;               //白虎子弹造成伤害的触发效果ID

    protected List<BeActor> summonMonsterList = new List<BeActor>();
    protected List<BeActor> deadMonsterList = new List<BeActor>();
    public int curCtrateCD = 1000;     //当前创建白虎子弹实体的CD
    protected int monsterId = 93000034;     //白虎怪物ID
    readonly protected int entityCreateHeight = 8000;    //子弹发射高度
    readonly protected int reboundDis = 5000;    //白虎反弹距离

    readonly protected int[] triggerBuffInfoId = new int[2] {360903, 360902};        //光环BuffInfo (PVE|PVP)
    readonly protected int attackBuffId = 360211;      //白虎Idle特效BuffId

    protected bool startFlag = false;       //满足发射条件 开始变形  但是尚未发射
    protected bool runningFlag = false;     //正在发射实体过程中
    protected bool changeFlag = false;      //是否变成发射状态
    protected bool switchToIdleFlag = false;    //数量少于一只 切换到普通Idle
    protected BeActor launchActor = null;   //当前发射球的白虎
    protected BeActor lastReceiveActor;     //上一只接到球的白虎
    protected BeActor receiveActor = null;  //当前接收球的白虎
    protected int lastReceiveSummonIndex = 0;   //上一只接到球的白虎在召唤列表中的位置
    protected BeProjectile projectile = null;       //创建的实体
    protected bool isCDFlag = false;
    protected int curCreateCD = 0;

    public override void OnReset()
    {
        summonMonsterList.Clear();
        deadMonsterList.Clear();
        startFlag = false;
        runningFlag = false;
        changeFlag = false;
        launchActor = null;
        lastReceiveActor = null;
        receiveActor = null;
        lastReceiveSummonIndex = 0; 
        projectile = null;
        isCDFlag = false;
        curCreateCD = 0;
    }

    public override void OnInit()
    {
        for(int i = 0; i < data.ValueA.Count; i++)
        {
            createCDArr[i] = TableManager.GetValueFromUnionCell(data.ValueA[i],level);
        }
        entitySpeed = VInt.NewVInt(TableManager.GetValueFromUnionCell(data.ValueB[0],level),GlobalLogic.VALUE_1000);
        for(int i = 0; i < data.ValueC.Count; i++)
        {
            addAttackValueArr[i] = TableManager.GetValueFromUnionCell(data.ValueC[i],level);
        }
    }

    public override void OnStart()
    {
        if (owner.CurrentBeScene == null)
            return;
        monsterId = BattleMain.IsModePvP(battleType) ? monsterIdArr[1] : monsterIdArr[0];
        sceneHandleA = owner.CurrentBeScene.RegisterEventNew(BeEventSceneType.onSummon, (args) =>
         {
             BeActor actor = (BeActor)args.m_Obj;
             if (actor != null && actor.GetEntityData().monsterID == monsterId && actor.GetCamp() == owner.GetCamp())
             {
                 summonMonsterList.Add(actor);
                 if (GetAliveMonsterCount() >= 2)
                 {
                     actor.delayCaller.DelayCall(GlobalLogic.VALUE_1000,()=> 
                     {
                         if (actor != null && !actor.IsDead())
                         {
                             PlayActionSingle(actor,changeToAttack,attackIdle);
                         }
                     });
                     AddAttackBuff(actor, true);
                 }
                 else
                 {
                     actor.delayCaller.DelayCall(467, () => 
                     {
                         if (actor != null && !actor.IsDead())
                         {
                             AddTriggerBuff(actor);
                         }
                     });
                 }
                 actor.RegisterEventNew(BeEventType.onDead, eventParam =>
                 {
                     deadMonsterList.Add(actor);
                 });
             }
         });
    }

    public override void OnUpdate(int deltaTime)
    {
        UpdateCreateCD(deltaTime);
        UpdateEntityState();
    }

    #region 创建实体相关

    


    protected bool StartLaunch()
    {
        launchActor = GetFirstMonster();
        if (launchActor == null)
            return false;
        lastReceiveActor = launchActor;
        lastReceiveSummonIndex = GetLaunchSummonIndex(launchActor);
        receiveActor = GetReceiveMonster(lastReceiveActor);
        if (receiveActor == null)
            return false;
        BeActor actor = (BeActor)launchActor.GetOwner();
        RegisterEntityCreate(actor);
        VInt3 createPos = launchActor.GetPosition();
        createPos.z += entityCreateHeight;
        projectile = (BeProjectile)actor.AddEntity(entityId, createPos);
        projectile.dontSetFace = true;
        SetTrail();
        return true;
    }

    protected void UpdateEntityState()
    {
        UpdateCreateCondition();
        UpdateEntityPos();
    }

    protected void EndLaunch()
    {
        projectile.DoDie();
        runningFlag = false;
        StartCD();
    }

    protected void UpdateEntityPos()
    {
        if (!runningFlag)
            return;
        if (receiveActor == null || receiveActor.IsDead())
        {
            SwitchTarget(false);
            return;
        }
        VInt3 targetPos = receiveActor.GetPosition();
        if (projectile != null)
        {
            VInt3 entityPos = projectile.GetPosition();
            if (Mathf.Abs(targetPos.x - entityPos.x) < reboundDis && Mathf.Abs(targetPos.y - entityPos.y) < reboundDis)
                SwitchTarget(true);
        }
    }

    protected void SwitchTarget(bool isCloseToTarget)
    {
        if (launchActor == null || receiveActor == null)
            return;
        if (receiveActor == launchActor)
        {
            EndLaunch();
            return;
        }
        //球接近目标时开始切换
        if (isCloseToTarget)
        {
            BeActor receive = GetReceiveMonster(receiveActor);
            if (receive != null)
            {
                lastReceiveActor = receiveActor;
                receiveActor = receive;
            }
            //已经是最后一只
            else
            {
                //第一只白虎没有死
                if (launchActor != null && !launchActor.IsDead())
                {
                    lastReceiveActor = receiveActor;
                    receiveActor = launchActor;
                }
                else
                {
                    lastReceiveActor = receiveActor;
                    launchActor = GetReceiveMonster(launchActor);
                    receiveActor = launchActor;
                }
            }
        }
        //接收球的白虎死亡
        else
        {
            if (receiveActor != null && receiveActor.IsDead())
            {
                lastReceiveActor = receiveActor;
                receiveActor = GetReceiveMonster(lastReceiveActor);
            }
        }
        if (receiveActor != null)
            SetTrail();
        else
            EndLaunch();
    }

    //设置实体的速度
    protected void SetTrail()
    {
        if (projectile == null || receiveActor == null)
            return;
        var trail = new LinearLogicTrail();
        var startPos = projectile.GetPosition();
        trail.startPoint = startPos;
        var targetPos = receiveActor.GetPosition();
        targetPos.z = trail.startPoint.z;
        trail.endPoint = targetPos;
        trail.moveSpeed = entitySpeed;
        trail.Init();
        owner.CurrentBeBattle.LogicTrailManager.AddLogicTrail(trail);
        projectile.logicTrail = trail;
        projectile.ResetDamageData();
        PlayActionSingle(lastReceiveActor, attack, attackIdle);
        ChangeMonsterFace(lastReceiveActor, receiveActor);
        SetModelRotate(targetPos, startPos);
    }

    //设置白虎子弹的旋转角度
    protected void SetModelRotate(VInt3 targetPos, VInt3 startPos)
    {
#if !LOGIC_SERVER
        if (projectile == null)
            return;
        //计算物体在朝向某个向量后的正前方
        Vector3 forwardDir = targetPos.vector3 - startPos.vector3;
        //计算朝向这个正前方时的物体四元数值
        if(forwardDir != Vector3.zero)
        {
            Quaternion lookAtRot = Quaternion.LookRotation(forwardDir);
            //把四元数值转换成角度
            Vector3 resultEuler = lookAtRot.eulerAngles;
            float rotateY = resultEuler.y - 90;
            projectile.m_pkGeActor.GetEntityNode(GeEntity.GeEntityNodeType.Root).transform.localRotation = Quaternion.Euler(0, rotateY, 0);
        }
#endif
    }

    //检测第一个实体发射的条件
    protected void UpdateCreateCondition()
    {
        if (runningFlag || isCDFlag)
            return;
        int count = GetAliveMonsterCount();

        if (!startFlag && changeFlag && count >= 2)           //不是第一次变形发射
        {
            runningFlag = true;
            StartLaunch();
        }
        else if (!changeFlag && count >= 2)     //第一次变形发射
        {
            changeFlag = true;
            startFlag = true;
            RemoveTriggerBuff();
            PlayAction(changeToAttack, attackIdle);
            owner.delayCaller.DelayCall(ChangeToAttackTime, () =>
            {
                bool result = StartLaunch();
                if (result)
                {
                    startFlag = false;
                    runningFlag = true;
                }
            });
        }
        else if(changeFlag && count < 2)        //数量不够变回最初的idle
        {
            changeFlag = false;
            List<BeActor> list = GamePool.ListPool<BeActor>.Get();
            GetAliveMonsterList(list);
            if (list.Count > 0)
            {
                AddTriggerBuff(list[0]);
                AddAttackBuff(list[0], false);
            }
            GamePool.ListPool<BeActor>.Release(list);
            PlayAction(changeToIdle, idle);
        }
    }

    //获取第一只创建的白虎(第一白虎的召唤者发射实体)
    protected BeActor GetFirstMonster()
    {
        BeActor actor = null;
        List<BeActor> list = GamePool.ListPool<BeActor>.Get();
        GetAliveMonsterList(list);
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] != null && !list[i].IsDead())
            {
                actor = list[i];
                break;
            }
        }
        GamePool.ListPool<BeActor>.Release(list);
        return actor;
    }

    //获取实体发射者在召唤列表中的位置
    protected int GetLaunchSummonIndex(BeActor actor)
    {
        int index = 0;
        for (int i = 0; i < summonMonsterList.Count; i++)
        {
            if (actor == summonMonsterList[i])
            {
                index = i;
                break;
            }
        }
        return index;
    }

    //获取接收的白虎
    protected BeActor GetReceiveMonster(BeActor lastReceive)
    {
        BeActor actor = null;
        int index = -1;
        for (int i = 0; i < summonMonsterList.Count; i++)
        {
            if (summonMonsterList[i] == lastReceive)
                index = i;
            if (index != -1 && i > index)
            {
                actor = summonMonsterList[i];
                break;
            }
        }
        return actor;
    }

    //假如在实体运行过程中 接收实体的白虎死亡 同时发射实体的白虎也死了
    protected BeActor GetLaunchMonster()
    {
        BeActor actor = null;
        for (int i = lastReceiveSummonIndex; i < summonMonsterList.Count; i++)
        {
            if (summonMonsterList[i] != null && !summonMonsterList[i].IsDead())
            {
                actor = summonMonsterList[i];
                break;
            }
        }
        return actor;
    }

    //获取存活的白虎数量
    protected int GetAliveMonsterCount()
    {
        int count = 0;
        List<BeActor> list = GamePool.ListPool<BeActor>.Get();
        GetAliveMonsterList(list);
        count = list.Count;
        GamePool.ListPool<BeActor>.Release(list);
        return count;
    }

    //获取当前存活的白虎
    protected void GetAliveMonsterList(List<BeActor> monsterList)
    {
        for (int i = 0; i < summonMonsterList.Count; i++)
        {
            BeActor actor = summonMonsterList[i];
            if (!deadMonsterList.Contains(actor))
                monsterList.Add(actor);
        }
    }

    //监听白虎子弹创建
    protected void RegisterEntityCreate(BeActor creater)
    {
        if (handleB != null)
        {
            handleB.Remove();
            handleB = null;
        }
        handleB = creater.RegisterEventNew(BeEventType.onChangeDamage, args =>
        {
            int effectId = args.m_Int;
            if (effectId != hurtId)
                return;
            int aliveMonsterCount = GetAliveMonsterCount();
            if (aliveMonsterCount < 2 || aliveMonsterCount > 5)
                return;
            int addAttackRate = addAttackValueArr[aliveMonsterCount - 2];

            /*int[] damageArray = (int[])args[1];
            damageArray[0] += addAttackRate;
            damageArray[1] += addAttackRate;*/
            args.m_Int2 += addAttackRate;
            args.m_Int3 += addAttackRate;
        });
    }

    //改变白虎的朝向
    protected void ChangeMonsterFace(BeActor launcher, BeActor receiver)
    {
        if (launcher == null || receiver == null)
            return;
        int xDis = launcher.GetPosition().x - receiver.GetPosition().x;
        launcher.SetFace(xDis > 0, true, true);
    }

    //添加光环BuffInfo
    protected void AddTriggerBuff(BeActor actor)
    {
        int buffInfoId = BattleMain.IsModePvP(battleType) ? triggerBuffInfoId[1] : triggerBuffInfoId[0];
        BuffInfoData buffInfo = new BuffInfoData(buffInfoId, actor.GetEntityData().level);
        actor.buffController.AddTriggerBuff(buffInfo);
    }

    //删除光环BuffInfo
    protected void RemoveTriggerBuff()
    {
        int buffInfoId = BattleMain.IsModePvP(battleType) ? triggerBuffInfoId[1] : triggerBuffInfoId[0];
        List<BeActor> aliveList = GamePool.ListPool<BeActor>.Get();
        GetAliveMonsterList(aliveList);
        for(int i = 0; i < aliveList.Count; i++)
        {
            aliveList[i].buffController.RemoveTriggerBuff(buffInfoId);
            AddAttackBuff(aliveList[i],true);
        }
        GamePool.ListPool<BeActor>.Release(aliveList);
    }

    //添加攻击特效BuffId
    protected void AddAttackBuff(BeActor actor, bool isAdd = false)
    {
        if (isAdd)
        {
            actor.buffController.TryAddBuff(attackBuffId, int.MaxValue);
        }
        else
        {
            BeBuff buff = actor.buffController.HasBuffByID(attackBuffId);
            if (buff != null)
                actor.buffController.RemoveBuff(buff);
        }
    }

    #endregion

    #region CD相关
       protected void StartCD()
    {
        isCDFlag = true;
        int monsterCount = GetAliveMonsterCount();
        if (monsterCount <= 5 && monsterCount > 1)
        {
            if (monsterCount > 1)
                curCtrateCD = createCDArr[monsterCount - 2];
        }
        curCreateCD = curCtrateCD;
    }

    protected void UpdateCreateCD(int deltaTime)
    {
        if (!isCDFlag)
            return;
        if (curCreateCD > 0)
            curCreateCD -= deltaTime;
        else
        {
            curCreateCD = curCtrateCD;
            isCDFlag = false;
        }
    }
    #endregion

    #region 白虎播放动作相关
    readonly static protected string idle = "Idle";
    readonly static protected string attack = "Attack";
    readonly static protected string changeToAttack = "ChangeToAttack";
    readonly static protected string attackIdle = "AttackIdle";
    readonly static protected string changeToIdle = "ChangeToIdle";
	readonly protected int ChangeToAttackTime = 1500;

    protected void PlayAction(string curAction, string delayAction)
    {
        List<BeActor> list = GamePool.ListPool<BeActor>.Get();
        GetAliveMonsterList(list);
        if (list != null)
        {
            for (int i = 0; i < list.Count; i++)
            {
                BeActor actor = list[i];
                PlayActionSingle(actor, curAction, delayAction);
            }
        }
        GamePool.ListPool<BeActor>.Release(list);
    }

    protected void PlayActionSingle(BeActor actor, string curAction, string delayAction)
    {
        int time = actor.PlayAction(curAction);
        actor.delayCaller.DelayCall(time, () =>
        {
            if (actor != null && !actor.IsDead())
                actor.PlayAction(delayAction);
        });
    }
    #endregion
}
