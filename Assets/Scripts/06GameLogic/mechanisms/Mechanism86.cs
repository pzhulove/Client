using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 异界1第一关三个哥布林三连击机制
/// </summary>
public class Mechanism86 : BeMechanism
{

    private IBeEventHandle handle;

    private BeActor target;
    private int index = 0;

    private int skillID = 0;

    private int delayRunTime = 0;
    private int delayWaitTime = 0;
    private int actionSpeed = 250;

    readonly private int monsterID = 30000021;

    private VInt3 tmpPos;

    List<BeActor> monsterList = new List<BeActor>();
    readonly private int speed = 10;

    Dictionary<int, bool> readyFlag = new Dictionary<int, bool>();
    public Mechanism86(int id, int lv) : base(id, lv){}

    public override void OnReset()
    {
        handle = null;
        target = null;
        index = 0;
        skillID = 0;
        tmpPos = VInt3.zero;
        if(monsterList != null)
        {
            monsterList.Clear();
        }
        if(readyFlag != null)
        {
            readyFlag.Clear();
        }
    }

    public override void OnInit()
    {
        base.OnInit();
        monsterList = new List<BeActor>();
        readyFlag.Clear();
        skillID = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        delayRunTime = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        delayWaitTime = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
        actionSpeed = TableManager.GetValueFromUnionCell(data.ValueD[0], level);
    }

    /// <summary>
    /// 监听是否给敌人上了眩晕BUFF
    /// </summary>
    public override void OnStart()
    {
        base.OnStart();
        handle = owner.RegisterEventNew(BeEventType.OnAddBuffToOthers, (args) =>
        {
            target = args.m_Obj as BeActor;
            BeBuff buff = args.m_Obj2 as BeBuff;
            if (buff != null && buff.buffType == BuffType.STUN)
            {
                if (target != null && target.isMainActor)
                {
                    owner.CurrentBeScene.FindMonsterByID(monsterList, monsterID);
                    monsterList.RemoveAll(x => { return x.GetPID() == owner.GetPID() || x.IsDead(); });
                    readyFlag.Clear();
                    index = 0;
                    tmpPos = target.GetPosition();
                    for (int i = 0; i < monsterList.Count; i++)
                    {
                        DoWork(monsterList[i]);
                    }
                }
            }
        });
    }

    /// <summary>
    /// 跑过去进行攻击
    /// </summary>
    /// <param name="monster"></param>
    void DoWork(BeActor monster)
    {
        //如果怪物在放技能，则强制中断技能
        if (monster == null || monster.IsDead())
        {
            return;
        }
        monster.aiManager.StopCurrentCommand();
        monster.aiManager.Stop();
        if (monster.IsCastingSkill())
        {
            monster.Locomote(new BeStateData((int)ActionState.AS_IDLE), true);
        }
        monster.m_pkGeActor.ShowHeadDialog(TR.Value("monsterSpeech"), false, false, false, false, delayWaitTime / 1000.0f, false);
        //等待怪物切换状态机，延迟一帧执行
        
        monster.delayCaller.DelayCall(delayWaitTime + index * delayRunTime, () =>
            {
                if (monster != null
            && target != null
            && !monster.IsDead()
            && !monster.IsInPassiveState()
            && CanRunState(monster))
                {
                    readyFlag[monster.GetPID()] = false;
                    monster.ClearMoveSpeed();
                    VInt3 vSpeed = (tmpPos - monster.GetPosition()).NormalizeTo(speed * IntMath.Float2IntWithFixed(1.0f));
                    monster.SetMoveSpeedX(vSpeed.x);
                    monster.SetMoveSpeedY(vSpeed.y);
                    monster.SetFace(vSpeed.x < 0);

                    monster.m_pkGeActor.ChangeAction("Anim_Walk", actionSpeed / 1000.0f, true);

                }
                else
                {
                    SetMonsterState(monster, true);
                }

            });
        index++;
    }

    private bool CanRunState(BeActor monster)
    {
        return monster.sgGetCurrentState() == (int)ActionState.AS_IDLE ||
                 monster.sgGetCurrentState() == (int)ActionState.AS_WALK ||
                 monster.sgGetCurrentState() == (int)ActionState.AS_RUN;
    }

    public override void OnUpdate(int deltaTime)
    {
        base.OnUpdate(deltaTime);
        if (monsterList == null) return;

        for (int i = 0; i < monsterList.Count; i++)
        {

            if (monsterList[i].IsDead())
                continue;

            if (!readyFlag.ContainsKey(monsterList[i].GetPID()))
                continue;

            if (readyFlag[monsterList[i].GetPID()])
                continue;

            if (monsterList[i].IsInPassiveState())
            {
                SetMonsterState(monsterList[i], true);
                continue;
            }
            if (IsNearTargetPosition(monsterList[i]))
            {
                SetMonsterState(monsterList[i]);
            }

        }
    }

    private void SetMonsterState(BeActor monster, bool isBreak = false)
    {
        readyFlag[monster.GetPID()] = true;
        monster.aiManager.Start();
        monster.ChangeRunMode(false);
        monster.m_pkGeActor.ChangeAction("Anim_Idle", 0.25f);
        if (isBreak) return;
        if (monster.CanUseSkill(skillID))
        {
            monster.UseSkill(skillID);
        }
    }

    public override void OnFinish()
    {
        target = null;
        readyFlag.Clear();
        if (monsterList != null)
        {
            monsterList.Clear();
            monsterList = null;
        }
        if (handle != null)
        {
            handle.Remove();
            handle = null;
        }
    }

    public bool IsNearTargetPosition(BeActor monster)
    {
        int distance = VInt.Float2VIntValue(1.0f);
        VInt3 pos = monster.GetPosition();
        return (((Mathf.Abs(tmpPos.x - pos.x)) <= distance) && (Mathf.Abs(tmpPos.y - pos.y) <= distance));
    }
}
