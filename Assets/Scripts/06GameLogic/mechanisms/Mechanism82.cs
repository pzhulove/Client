using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 天黑机制
public class Mechanism82 : BeMechanism
{
    List<int> buffList = new List<int>();
    VInt distance;

    List<BeActor> playerList = new List<BeActor>();
    List<BeActor> targetList = new List<BeActor>();
    List<IBeEventHandle> handleList = new List<IBeEventHandle>();
    int timer;
    readonly int interval = 150;                             //检测距离时间间隔
    int blackSceneID = -1;

    public Mechanism82(int mid, int lv) : base(mid, lv) { }

    public override void OnReset()
    {
        buffList.Clear();
        distance = VInt.zero;
        playerList.Clear();
        targetList.Clear();
        handleList.Clear();
        timer = 0;
        blackSceneID = -1;
    }
    public override void OnInit()
    {
        for (int i = 0; i < data.ValueA.Count; i++)
        {
            buffList.Add(TableManager.GetValueFromUnionCell(data.ValueA[i], level));
        }
        buffList.Add(21);//加入隐身buff
        distance = VInt.NewVInt(TableManager.GetValueFromUnionCell(data.ValueB[0], level), GlobalLogic.VALUE_1000);
    }

    public override void OnStart()
    {
        timer = 0;
        _initPlayerList();
        _switchSceneEffect(true);
    }

    private void _switchSceneEffect(bool flag)
    {
#if !LOGIC_SERVER
        if (flag)
        {
            blackSceneID = owner.CurrentBeScene.currentGeScene.BlendSceneSceneColor(Color.white*0.3f,0.5f);
        }
        else
        {
            owner.CurrentBeScene.currentGeScene.RecoverSceneColor(0.5f, blackSceneID);
        }
#endif
    }

    private void _initPlayerList()
    {
        var players = owner.CurrentBeBattle.dungeonPlayerManager.GetAllPlayers();
        if (players == null)
            return;
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i] != null)
            {
                var actor = players[i].playerActor;
                playerList.Add(actor);
                _addEffect(actor);
                var handle = actor.RegisterEventNew(BeEventType.onReborn, args =>
                {
                    _addEffect(actor);
                });
                handleList.Add(handle);
            }
        }
        playerList.Add(owner);
        _addEffect(owner);
    }

    private void _addEffect(BeActor actor)
    {
        if (actor.IsDead()) return;
        actor.m_pkGeActor.CreateEffect(1011, Vec3.zero, false, 0, EffectTimeType.BUFF);
    }

    private bool _checkInDistance(BeActor monster)
    {
        if (monster == null)
            return false;
        if (monster.IsDead())
            return false;

        for (int i = 0; i < playerList.Count; i++)
        {
            if (playerList[i] != null && !playerList[i].IsDead())
            {
                var vec = monster.GetPosition() - playerList[i].GetPosition();
                if (vec.magnitude < distance.i)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void _addBuffs(BeActor monster)
    {
        if (monster != null)
        {
            for (int i = 0; i < buffList.Count; i++)
            {
                var buff = monster.buffController.HasBuffByID(buffList[i]);
                if (buff == null)
                {
                    monster.buffController.TryAddBuff(buffList[i], GlobalLogic.VALUE_100000 * 10);
                }
            }
            {
                var buff = monster.buffController.HasBuffByID(42);
                if (buff != null)
                {
                    monster.buffController.RemoveBuff(buff);
                }
            }
        }
    }

    private void _removeBuffs(BeActor monster)
    {
        if (monster != null)
        {
            for (int i = 0; i < buffList.Count; i++)
            {
                var buff = monster.buffController.HasBuffByID(buffList[i]);
                if (buff != null)
                {
                    monster.buffController.RemoveBuff(buff);
                }
            }
            {
                var buff = monster.buffController.HasBuffByID(42);
                if (buff == null)
                {
                    monster.buffController.TryAddBuff(42, GlobalLogic.VALUE_1500);//添加渐显buff
                }
            }
            if (monster.m_pkGeActor != null && monster.m_cpkCurEntityActionInfo != null)
            {
                monster.m_pkGeActor.ChangeAction(monster.m_cpkCurEntityActionInfo.actionName, 0.25f, true);
            }
        }
    }

    private void _removeAllBuffs()
    {
        for (int i = 0; i < targetList.Count; i++)
        {
            _removeBuffs(targetList[i]);
        }
    }

    public override void OnUpdate(int deltaTime)
    {
        if (owner == null)
            return;

        if (owner.IsDead())
            return;

        timer += deltaTime;
        if (timer >= interval)
        {
            timer = 0;

            owner.CurrentBeScene.FindTargets(targetList, owner, VInt.Float2VIntValue(100f), true);
            for (int i = 0; i < targetList.Count; i++)
            {
                if (targetList[i] == null)
                    continue;
                if (targetList[i] == owner)
                    continue;
                if (targetList[i].IsDead())
                    continue;

                if (_checkInDistance(targetList[i]))
                {
                    _removeBuffs(targetList[i]);
                }
                else
                {
                    _addBuffs(targetList[i]);
                }
            }
        }
    }

    private void _removeAllEffects()
    {
        for (int i = 0; i < playerList.Count; i++)
        {
            if (playerList[i] != null && !playerList[i].IsDead())
            {
                var effect = playerList[i].m_pkGeActor.FindEffectById(1011);
                if (effect != null)
                {
                    playerList[i].m_pkGeActor.DestroyEffect(effect);
                }
            }
        }
    }

    private void _removeHandles()
    {
        for (int i = 0; i < handleList.Count; i++)
        {
            var handle = handleList[i];
            if (handle != null)
            {
                handle.Remove();
                handle = null;
            }
        }
        handleList.Clear();
    }

    public override void OnFinish()
    {
        _removeAllBuffs();
        _removeAllEffects();
        _removeHandles();
        _switchSceneEffect(false);

        targetList.Clear();
        buffList.Clear();
        playerList.Clear();
    }
}
