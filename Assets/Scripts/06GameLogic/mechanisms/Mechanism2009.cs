using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 黑色大地房间6的终结机制
/// </summary>
public class Mechanism2009 : BeMechanism
{
    private int buffID = 0;
    private readonly int actionSpeed = 250;
    private int time = 5000;
    private readonly VInt tolerance = VInt.Float2VIntValue(0.5f);
    private List<BeActor> target = new List<BeActor>();
    private int skillID = 0;
    private bool isMoving = false;
    private List<IBeEventHandle> handleList = new List<IBeEventHandle>();
    private readonly int[] monsterIDs = new int[] {31140011, 31130011 , 31150011 , 31160011 , 31170011 };//变身之后的怪物ID
    private VInt3 tmpPos;
    public Mechanism2009(int id, int lv) : base(id, lv)
    { }

    public override void OnInit()
    {
        base.OnInit();
        skillID = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        buffID = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        time = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
    }

    public override void OnReset()
    {
        target.Clear();
        isMoving = false;
        RemoveHandleList();
        tmpPos = VInt3.zero;
    }

    public override void OnStart()
    {
        base.OnStart();
        List<BeActor> list = new List<BeActor>();
        owner.CurrentBeScene.FindMainActor(list);
        for (int i = 0; i < list.Count; i++)
        {
            BeActor actor = list[i];
            IBeEventHandle handle01 = RegistAddHandle(actor);
            IBeEventHandle handle02 =  actor.RegisterEventNew(BeEventType.onSummon, (args) =>
            {
                BeActor monster = args.m_Obj as BeActor;
                if (MonsterIDEqual(monster))
                {
                    IBeEventHandle handle03 = RegistAddHandle(monster);
                    handleList.Add(handle03);
                }
            });
            handleList.Add(handle01);
            handleList.Add(handle02);
        }
    }

    /// <summary>
    /// 如果玩家变身也要生效
    /// </summary>
    /// <param name="monster"></param>
    /// <returns></returns>
    private bool MonsterIDEqual(BeActor monster)
    {
        for (int i = 0; i < monsterIDs.Length; i++)
        {
            if (monster.GetEntityData().MonsterIDEqual(monsterIDs[i]))
                return true;
        }
        return false;
    }

    /// <summary>
    /// 玩家被缠绕之后boss移向玩家
    /// </summary>
    /// <param name="actor"></param>
    /// <returns></returns>
    private IBeEventHandle RegistAddHandle(BeActor actor)
    {
        return actor.RegisterEventNew(BeEventType.onAddBuff, (args) =>
        {
            BeBuff buff = args.m_Obj as BeBuff;
            if (buff.buffID == buffID)
            {
                target.Add(actor);
                if (!isMoving)
                    MoveToTarget(target[0]);
            }
        });
    }

    /// <summary>
    /// 移向被冤魂缠绕的玩家
    /// </summary>
    /// <param name="target"></param>
    private void MoveToTarget(BeActor target)
    {
        isMoving = true;
        if (owner != null && owner.m_pkGeActor != null)
        {
            if (owner.aiManager != null)
                owner.aiManager.Stop();
            tmpPos = target.GetPosition();
            owner.SetFace((target.GetPosition().x-owner.GetPosition().x)<0,true);
            BeMoveTo moveTo = BeMoveTo.Create(owner, time, owner.GetPosition(), target.GetPosition(), false);
            owner.actionManager.RunAction(moveTo);
            owner.m_pkGeActor.ChangeAction("Anim_Walk", 1, true);
        }
    }

    /// <summary>
    /// boss靠近之后使用终结技能
    /// </summary>
    /// <param name="deltaTime"></param>
    public override void OnUpdate(int deltaTime)
    {
        base.OnUpdate(deltaTime);
        if (target.Count <= 0) return;
        if (target[0] == null || target[0].IsDead())
        {
            if (owner.aiManager != null)
                owner.aiManager.Start();
            return;
        }
        if (IsNearTargetPosition() && isMoving)
        {
            isMoving = false;
            owner.UseSkill(skillID);
            target.RemoveAt(0);
            if (target.Count > 0)
            {
                owner.delayCaller.DelayCall(4000, () =>
                {
                    MoveToTarget(target[0]);
                });
            }
            else
            {
                if (owner.aiManager != null)
                    owner.aiManager.Start();
            }
        }

    }

    public bool IsNearTargetPosition()
    {
        int distance = tolerance.i;
        int dist = (owner.GetPosition() - tmpPos).magnitude;
        return dist <= distance;
    }

    public override void OnFinish()
    {
        base.OnFinish();
        RemoveHandleList();
    }

    private void RemoveHandleList()
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
}
