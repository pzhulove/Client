using System.Collections.Generic;
using UnityEngine;
using GameClient;


/// <summary>
/// 将怪物吸到嘴边并且吞噬的机制
/// </summary>
public class Mechanism2059 : BeMechanism
{
    public Mechanism2059(int id, int lv) : base(id, lv) { }

    private int[] monsterIdArr = new int[2];  //怪物ID(触手|尸体)
    private int absorbMaxCount = 0; //吸入尸体怪物最大数量
    private VInt3 targetPos = VInt3.zero;   //目标点位置
    private int absorbSpeed = 0;   //吸引速度
    private int absorbAcc = 0;  //吸引加速度
    private int hurtId = 0; //伤害触发效果ID
    private int delayTime = 0;  //机制延时生效时间
    
    private int useSkillId = 21074; //虚弱状态下释放的技能Id

    private int curAbsorbSpeed = 0; //当前的吸引速度
    private bool absorbFlag = true;

    private TeamDungeonBattleFrame frame = null;
    private int curDelayTime = 0;   //当前延时时间
    private int runingTime = 0;    //机制存在时间
    private int[] offsetArr = new int[5];
    private List<BeActor> monsterList = new List<BeActor>();    //死亡的触角怪物
    private int monsterDead = 0;    //怪物死亡数量

    public override void OnInit()
    {
        base.OnInit();
        monsterIdArr[0] = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        monsterIdArr[1] = TableManager.GetValueFromUnionCell(data.ValueA[1], level);
        absorbMaxCount = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        for (int i = 0; i < data.ValueC.Count; i++)
        {
            targetPos[i] = TableManager.GetValueFromUnionCell(data.ValueC[i], level);
        }
        absorbSpeed = TableManager.GetValueFromUnionCell(data.ValueD[0], level);
        absorbAcc = TableManager.GetValueFromUnionCell(data.ValueE[0], level);
        hurtId = TableManager.GetValueFromUnionCell(data.ValueF[0], level);
        delayTime = TableManager.GetValueFromUnionCell(data.ValueG[0], level);
    }

    public override void OnReset()
    {
        targetPos = VInt3.zero;
        curAbsorbSpeed = 0;
        absorbFlag = true;
        frame = null;
        curDelayTime = 0;
        runingTime = 0;
        offsetArr = new int[5];
        monsterList.Clear();
        monsterDead = 0;
    }

    public override void OnStart()
    {
        base.OnStart();
        InitData();
    }

    public override void OnUpdate(int deltaTime)
    {
        base.OnUpdate(deltaTime);
        if (!CheckStart(deltaTime))
            return;
        runingTime += deltaTime;
        UpdateAbsorbPlayers();
        UpdateAbsorbDeadBody();
        UpdateAbsorbSpeed();
        CheckStart(deltaTime);
    }

    /// <summary>
    /// 延时生效
    /// </summary>
    private bool CheckStart(int deltaTime)
    {
        if (curDelayTime >= delayTime)
            return true;
        curDelayTime += deltaTime;
        return false;
    }

    public override void OnFinish()
    {
        base.OnFinish();
        SetMonsterRemove();
        CloseFrame();
    }

    private void InitData()
    {
        curDelayTime = 0;
        runingTime = 0;
        monsterDead = 0;
        RefreshCompleteCount(0);
        for(int i = 0; i < offsetArr.Length; i++)
        {
            int offset = FrameRandom.InRange(-1000, 1000);
            offsetArr[i] = offset;
        }
        monsterList.Clear();
        if (owner.CurrentBeScene != null)
        {
            sceneHandleA = owner.CurrentBeScene.RegisterEventNew(BeEventSceneType.onSummon,(args)=> 
            {
                BeActor actor = (BeActor) args.m_Obj;
                if (actor != null && actor.GetEntityData().MonsterIDEqual(monsterIdArr[1]))
                {
                    monsterDead++;
                    RefreshCompleteCount(monsterDead);
                }
            });
        }
    }

    /// <summary>
    /// 根据加速度更新吸引速度
    /// </summary>
    private void UpdateAbsorbSpeed()
    {
        curAbsorbSpeed = absorbSpeed + runingTime * absorbAcc;
    }

    /// <summary>
    /// 吸引玩家
    /// </summary>
    private void UpdateAbsorbPlayers()
    {
        if (!absorbFlag)
            return;
        if (owner.CurrentBeBattle == null || owner.CurrentBeBattle.dungeonPlayerManager == null)
            return;
        var battlePlayerList = owner.CurrentBeBattle.dungeonPlayerManager.GetAllPlayers();
        for (int i = 0; i < battlePlayerList.Count; i++)
        {
            if (battlePlayerList[i] == null)
                continue;
            BeActor actor = battlePlayerList[i].playerActor;
            if (actor == null)
                continue;
            if (actor.IsDead())
                continue;
            //靠近场景边缘 给玩家造成一次最大值的伤害
            if (GetDis(actor, targetPos) < GlobalLogic.VALUE_5000)
            {
                ClearExtraSpeed(actor);
                if (actor.stateController.CanBeHit())
                {
#if !LOGIC_SERVER
                    if (actor.isLocalActor)
                    {
                        if (actor.CurrentBeBattle != null)
                            actor.CurrentBeBattle.PlaySound(5016);
                    }
#endif
                    owner.DoAttackTo(actor, hurtId);
                }
            }
            else
            {
                AbsorbTarget(actor, targetPos);
            }
        }
    }

    /// <summary>
    /// 吸引怪物尸体
    /// </summary>
    private void UpdateAbsorbDeadBody()
    {
        if (!absorbFlag)
            return;
        List<BeActor> list = GamePool.ListPool<BeActor>.Get();
        var pos = targetPos;
        pos.x -= 20000;     //章鱼触角的X轴偏移
        owner.CurrentBeScene.FindMonsterByIDAndCamp(list, monsterIdArr[1], owner.GetCamp());
        for (int i = 0; i < list.Count; i++)
        {
            var actor = list[i];
            if (GetDis(list[i], pos) <= GlobalLogic.VALUE_5000)
            {
                if (!monsterList.Contains(actor))
                {
                    monsterList.Add(actor);
                    ClearExtraSpeed(actor);
                    SetMonsterActorXOffset();
                }
            }
            else
            {
                AbsorbTarget(actor, pos);
            }
        }
        GamePool.ListPool<BeActor>.Release(list);

        if (monsterList.Count >= absorbMaxCount)
        {
            //吸入了足够的尸体
            absorbFlag = false;
            owner.UseSkill(useSkillId, true);
        }
    }

    /// <summary>
    /// 获取自己与目标的距离
    /// </summary>
    /// <returns></returns>
    private int GetDis(BeActor actor,VInt3 pos)
    {
        return (pos - actor.GetPosition()).magnitude;
    }

    /// <summary>
    /// 吸引目标
    /// </summary>
    private void AbsorbTarget(BeActor actor, VInt3 pos)
    {
        if (!actor.stateController.CanMove())
            return;
        if (actor.stateController.CanNotAbsorbByBlockHole())
            return;
        var playerPos = actor.GetPosition();
        VInt3 del = new VInt3(pos.x - playerPos.x, pos.y - playerPos.y, pos.z - playerPos.z);
        VInt3 delNor = del.NormalizeTo(curAbsorbSpeed);
        if (actor.stateController.CanMoveX())
        {
            actor.extraSpeed.x = delNor.x;
        }

        if (actor.stateController.CanMoveY())
        {
            actor.extraSpeed.y = delNor.y;
        }
           
        actor.extraSpeed.z = delNor.z;
    }

    /// <summary>
    /// 清除速度
    /// </summary>
    private void ClearExtraSpeed(BeActor actor)
    {
        actor.extraSpeed = VInt3.zero;
    }

    /// <summary>
    /// 移除怪物
    /// </summary>
    private void SetMonsterRemove()
    {
        //移除触手
        for(int i = 0; i < monsterList.Count; i++)
        {
            var actor = monsterList[i];
            if (actor == null || actor.IsDead())
                continue;
            actor.DoDead();
        }
        SetMonsterActorXOffset(true);
    }

    /// <summary>
    /// 设置怪物模型随机偏移
    /// </summary>
    private void SetMonsterActorXOffset(bool isRestore = false)
    {
#if !LOGIC_SERVER
        for(int i = 0; i < offsetArr.Length; i++)
        {
            if (i >= monsterList.Count)
                continue;
            var monster = monsterList[i];
            if (monster == null)
                continue;
            if (monster.m_pkGeActor == null)
                continue;
            var root = monster.m_pkGeActor.GetEntityNode(GeEntity.GeEntityNodeType.Actor);
            if (root == null)
                continue;
            if (isRestore)
            {
                root.transform.localPosition = Vector3.zero;
            }
            else
            {
                root.transform.localPosition = new Vector3(offsetArr[i] / 1000.0f, 0, 0);
            }
        }
#endif
    }

    private void RefreshCompleteCount(int curCount)
    {
#if !LOGIC_SERVER
        if (frame == null)
        {
            frame = ClientSystemManager.instance.OpenFrame<TeamDungeonBattleFrame>(FrameLayer.Middle) as TeamDungeonBattleFrame;
        }

        if (frame != null)
        {
            frame.ShowCompelteCount(curCount, absorbMaxCount);
        }
#endif
    }

    private void CloseFrame()
    {
#if !LOGIC_SERVER
        if (frame == null)
            return;
        frame.Close();
        frame = null;
#endif
    }
}
