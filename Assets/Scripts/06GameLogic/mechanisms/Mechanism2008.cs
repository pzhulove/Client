using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 黑色大地四神兽解除连接机制，给隐形怪上的机制
/// </summary>
public class Mechanism2008 : BeMechanism
{
    private readonly int[] monsterIDs = new int[4] { 30830011, 30840011, 30850011, 30860011 };//四个石像的ID
    private readonly int[] buffIDs = new int[4] { 521736, 521737, 521738, 521739 };//每个石像添加的buff
    private int[] calcDamage = new int[4] { 0, 0, 0, 0 };

    private readonly int bossID = 30770021;
    private readonly int buffID = 521740;
    private int totalDamage = 0;
    private int time = 0;
    private List<BeActor> actorList = new List<BeActor>();
    private BeActor boss = null;
    public Mechanism2008(int mid, int lv) : base(mid, lv) { }

    public override void OnInit()
    {
        base.OnInit();
        totalDamage = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        time = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
    }

    public override void OnReset()
    {
        calcDamage = new int[4] { 0, 0, 0, 0 };
        actorList.Clear();
        boss = null;
        tmplist.Clear();
        timer = 0;
        flag = false;
    }

    private List<BeActor> tmplist = new List<BeActor>();
    public override void OnStart()
    {
        base.OnStart();
        for (int i = 0; i < monsterIDs.Length; i++)
        {
            int index = i;
            FindMonster(monsterIDs[index], index);
        }
        boss = owner.CurrentBeScene.FindMonsterByID(bossID);
        flag = true;
        tmplist.AddRange(actorList);
    }

    private int timer = 0;
    private bool flag = false;
    public override void OnUpdate(int deltaTime)
    {
        base.OnUpdate(deltaTime);
        if (!flag) return;
        timer += deltaTime;
        if (timer >= time)
        {
            timer = 0;
            BeActor actor = SelectTarget();
            if (actor != null)
            {
                AddBuff(actor);
            }
        }
    }

    /// <summary>
    /// 添加buff
    /// </summary>
    /// <param name="actor"></param>
    private void AddBuff(BeActor actor)
    {
        for (int i = 0; i < monsterIDs.Length; i++)
        {
            if (actor.GetEntityData().MonsterIDEqual(monsterIDs[i]))
            {
                actor.buffController.TryAddBuff(buffIDs[i], -1);
            }
        }
    }

    /// <summary>
    /// 随机选择一个石像给boss加buff
    /// </summary>
    /// <returns></returns>
    private BeActor SelectTarget()
    {
        int index = owner.FrameRandom.InRange(0, tmplist.Count);
        BeActor target = tmplist[index];
        tmplist.Remove(target);
        if (HaveBuff(target))
        {
            return SelectTarget();
        }
        if (tmplist.Count <= 0)
        {
            if (boss != null)
            {
                boss.buffController.TryAddBuff(buffID, -1);
            }
            tmplist.AddRange(actorList);
            flag = false;
        }
        return target;
    }

    /// <summary>
    /// 判断boss是否有buff
    /// </summary>
    /// <param name="actor"></param>
    /// <returns></returns>
    private bool HaveBuff(BeActor actor)
    {
        for (int i = 0; i < buffIDs.Length; i++)
        {
            if (actor.buffController.HasBuffByID(buffIDs[i]) != null)
                return true;
        }
        return false;
    }

    /// <summary>
    /// 监听每个石像的伤害
    /// </summary>
    /// <param name="id"></param>
    /// <param name="index"></param>
    private void FindMonster(int id, int index)
    {
        BeActor actor = owner.CurrentBeScene.FindMonsterByID(id);
        if (actor != null)
        {
            actorList.Add(actor);
            RegistDamageEvent(actor, index);
        }
    }

    /// <summary>
    /// 当伤害总量达到一定数的时候，解除所有链接
    /// </summary>
    /// <param name="actor"></param>
    /// <param name="index"></param>
    private void RegistDamageEvent(BeActor actor, int index)
    {
        int tmp = index;
        handleA = actor.RegisterEventNew(BeEventType.onHit, args =>
        //BeEventHandle handle = actor.RegisterEvent(BeEventType.onHit, (args) =>
        {
            int damage = args.m_Int;
            calcDamage[tmp] += damage;
            if (calcDamage[tmp] >= totalDamage)
            {
                ClearAllChain();
            }
        });
        //handleList.Add(handle);
    }

    /// <summary>
    /// 移除链接
    /// </summary>
    private void ClearAllChain()
    {
        timer = 0;
        flag = true;
        if (boss != null)
        {
            boss.buffController.RemoveBuff(buffID);
        }
        tmplist.Clear();
        tmplist.AddRange(actorList);

        for (int i = 0; i < calcDamage.Length; i++)
        {
            calcDamage[i] = 0;
        }
        for (int i = 0; i < actorList.Count; i++)
        {
            BeActor actor = actorList[i];
            if (actor != null && !actor.IsDead())
            {
                for (int j = 0; j < buffIDs.Length; j++)
                {
                    actor.buffController.RemoveBuff(buffIDs[j]);
                }
            }
        }
    }
}
