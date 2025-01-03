using UnityEngine;
using System.Collections.Generic;

//羽毛传递Buff

public class Buff2000020 : BeBuff
{
    public Buff2000020(int bi, int buffLevel, int buffDuration, int attack = 0) : base(bi, buffLevel, buffDuration, attack)
    {

    }

    private int totalTime = 25000;      //整体爆炸时间
    private int monsterId = 60011;   //召唤的怪物ID
    private VInt radius = VInt.zero; //查找目标范围

    protected int curTotalTime = 0;   //当前的整体时间
    private readonly int checkTargetTime = 1000; //检测玩家时间间隔
    private int curCheckTargetTime = 0;

    private string countDownPath = "UIFlatten/Prefabs/BattleUI/FeatherBuffText";
    private GameObject countDownPrefab = null;
    protected bool passFlag = false;

    //对召唤师觉醒做特殊处理
    private int magicMonsterId = 9080031;
    private IBeEventHandle changeHandle = null;
    private IBeEventHandle restoreHandle = null;

    private List<BeActor> outRangeList = new List<BeActor>();

    public override void OnReset()
    {
        countDownPrefab = null;
        passFlag = false;
        outRangeList.Clear();
    }

    public override void OnInit()
    {
        base.OnInit();
        totalTime = TableManager.GetValueFromUnionCell(buffData.ValueA[0], level);
        monsterId = TableManager.GetValueFromUnionCell(buffData.ValueB[0], level);
        radius = VInt.NewVInt(TableManager.GetValueFromUnionCell(buffData.ValueC[0], level), GlobalLogic.VALUE_1000);
    }

    public override void OnStart()
    {
        base.OnStart();
        curTotalTime = totalTime;
        curCheckTargetTime = checkTargetTime;
        CreateCountDownPrefab();
        JudgeMagicGirl();
        JudgeMagicGirlMonster();
    }

    /// <summary>
    /// 对召唤师觉醒做特殊处理
    /// </summary>
    private void JudgeMagicGirl()
    {
        if (owner.professionID != 33)
            return;
        changeHandle = owner.RegisterEventNew(BeEventType.onMagicGirlMonsterChange, args =>
        {
            BeActor monster = args.m_Obj as BeActor;
            if (monster != null)
            {
                PassBuff(monster, GetLeftTime());
                Finish();
            }
        });
    }

    /// <summary>
    /// 对召唤师觉醒怪物做特殊处理
    /// </summary>
    private void JudgeMagicGirlMonster()
    {
        if (!owner.GetEntityData().MonsterIDEqual(magicMonsterId))
            return;
        restoreHandle = owner.RegisterEventNew(BeEventType.onMagicGirlMonsterRestore, args => 
        {
            BeActor magicGirl = args.m_Obj as BeActor;
            if(magicGirl != null)
            {
                PassBuff(magicGirl,GetLeftTime());
                Finish();
            }
        });
    }

    public void SetTotalTime(int time)
    {
        curTotalTime = time;
    }

    public override void OnUpdate(int deltaTime)
    {
        base.OnUpdate(deltaTime);
        UpdateTotalTime(deltaTime);
        UpdateCheckTarget(deltaTime);
    }

    /// <summary>
    /// 刷新找目标
    /// </summary>
    private void UpdateCheckTarget(int deltaTime)
    {
        if (curCheckTargetTime <= 0)
        {
            curCheckTargetTime = checkTargetTime;
            List<BeEntity> list = GamePool.ListPool<BeEntity>.Get();
            owner.CurrentBeScene.GetEntitys2(list);
            if (list != null)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    var actor = list[i] as BeActor;
                    if (actor == null || actor.IsDead() || actor.IsRemoved())
                        continue;
                    if (actor == owner)
                        continue;
                    if (!(actor.IsBoss() || actor.professionID != 0))
                        continue;

                    if ((actor.GetPosition() - owner.GetPosition()).magnitude > radius)
                    {
                        UpdateOutRangeTarget(actor);
                        continue;
                    }

                    //如果没有离开过自己的范围 则不能传递
                    if (!outRangeList.Contains(actor))
                        continue;

                    PassBuff(actor,duration);
                    if (passFlag)
                    {
                        break;
                    }
                }
            }

            GamePool.ListPool<BeEntity>.Release(list);

            if (passFlag)
            {
                Finish();
            }
        }
        else
        {
            curCheckTargetTime -= deltaTime;
        }
    }

    /// <summary>
    /// 维护范围外目标列表
    /// </summary>
    /// <param name="entity"></param>
    private void UpdateOutRangeTarget(BeActor actor)
    {
        if (outRangeList.Contains(actor))
            return;
        outRangeList.Add(actor);
    }

    /// <summary>
    /// 创建倒计时
    /// </summary>
    private void CreateCountDownPrefab()
    {
#if !SERVER_LOGIC
        countDownPrefab = CGameObjectPool.instance.GetGameObject(countDownPath, enResourceType.BattleScene, (uint)GameObjectPoolFlag.ReserveLast);
        if (countDownPrefab != null)
        {
            GameClient.ShowCountDownComponent countDown = countDownPrefab.GetComponent<GameClient.ShowCountDownComponent>();
            countDown.InitData((int)(GetLeftTime() / 1000.0f));
            if (owner.m_pkGeActor != null)
            {
                Battle.GeUtility.AttachTo(countDownPrefab, owner.m_pkGeActor.goInfoBar);
                Vector3 pos = owner.m_pkGeActor.buffOriginLocalPosition;
                if (owner.IsBoss())
                    pos.y += 50;
                countDownPrefab.transform.localPosition = pos;
            }
        }
#endif
    }

    /// <summary>
    /// 刷新整个时间
    /// </summary>
    private void UpdateTotalTime(int deltaTime)
    {
        if (curTotalTime <= 0)
        {
            Finish();
        }
        else
        {
            curTotalTime -= deltaTime;
        }
    }

    /// <summary>
    /// 创建爆炸实体
    /// </summary>
    protected void CreateBoomEntity(int summonID)
    {
        if (owner.IsDead())
            return;
        BeActor boss = GetBoss();
        if (boss == null)
            return;
        int monsterIdNew = boss.GenNewMonsterID(summonID, boss.GetEntityData().level);
        var monster = boss.CurrentBeScene.SummonMonster(monsterIdNew, owner.GetPosition(), boss.GetCamp());
        if (owner.IsBoss() && monster != null)
        {
            monster.stateController.SetAbilityEnable(BeAbilityType.ATTACK_FRIEND_ENEMY, false);
        }
        
    }

    protected BeActor GetBoss()
    {
        BeActor actor = null;
        List<BeActor> list = GamePool.ListPool<BeActor>.Get();
        owner.CurrentBeScene.FindBoss(list);
        if (list.Count > 0)
            actor = list[0];
        GamePool.ListPool<BeActor>.Release(list);
        return actor;
    }

    /// <summary>
    /// 传递Buff
    /// </summary>
    protected virtual void PassBuff(BeActor target,int time)
    {
        var buff = target.buffController.TryAddBuff(2000020, time, level) as Buff2000020;
        if (buff != null)
        {
            passFlag = true;
            buff.SetTotalTime(curTotalTime);
        }
    }

    public override void OnFinish()
    {
        base.OnFinish();
        if (!passFlag)
        {
            CreateBoomEntity(monsterId);
            BeActor boss = GetBoss();
            if (boss == null)
                return;
            boss.Locomote(new BeStateData((int)ActionState.AS_IDLE), true);
        }

#if !SERVER_LOGIC
        if (countDownPrefab != null)
        {
            GameObject.Destroy(countDownPrefab);
        }
#endif
        outRangeList.Clear();
        RemoveHandle();
    }

    private void RemoveHandle()
    {
        if (changeHandle != null)
        {
            changeHandle.Remove();
            changeHandle = null;
        }
        if (restoreHandle != null)
        {
            restoreHandle.Remove();
            restoreHandle = null;
        }
    }
}
