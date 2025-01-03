using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoTable;

//魔曲-混乱
public class Skill5568 : BeSkill
{
    protected int m_HurtPlayerBuffId = 547708;
    protected int m_HurtSummonBuffId = 547709;

    BuffInfoTable m_HurtPlayerData = null;
    BuffInfoTable m_HurtSummonData = null;

	public Skill5568(int sid, int skillLevel):base(sid, skillLevel)
	{

	}

    public override void OnInit()
    {
        m_HurtPlayerData = TableManager.instance.GetTableItem<BuffInfoTable>(m_HurtPlayerBuffId);
        m_HurtSummonData = TableManager.instance.GetTableItem<BuffInfoTable>(m_HurtSummonBuffId);
    }

	public override void OnStart()
	{
        _AddBuffToPlayer();
        _AddBuffToSummon();
	}

    public override void OnFinish()
    {
        
    }

    protected void _AddBuffToPlayer()
    {
        List<BattlePlayer> battlePlayerList = owner.CurrentBeBattle.dungeonPlayerManager.GetAllPlayers();
        //给所有玩家上混乱buff
        for (int i = 0; i < battlePlayerList.Count; i++)
        {
            BeActor playerActor = battlePlayerList[i].playerActor;
            playerActor.delayCaller.DelayCall(m_HurtPlayerData.BuffDelay, () =>
            {
                playerActor.buffController.TryAddBuff(m_HurtPlayerBuffId);
            });
        }
    }

    protected void _AddBuffToSummon()
    {
        List<BeActor> summonsAlive = _GetAllAliveSummon();
        if (summonsAlive.Count > 0)
        {
            for (int j = 0; j < summonsAlive.Count; j++)
            {
                //根据每个召唤物的概率添加buff
                int random = FrameRandom.Range100();
                if (random <= 50)
                {
                    BeActor summon = summonsAlive[j];
                    summon.delayCaller.DelayCall(m_HurtSummonData.BuffDelay, () =>
                    {
                        if (!summon.IsDead())
                        {
                            summon.buffController.TryAddBuff(m_HurtSummonBuffId);
                        }
                    });
                }
            }
        }
    }

    //获取某个玩家的召唤物
    protected void _GetSummonsByPlayer(List<BeActor> summonsAlive, BeActor Player)
    {
        List<BeActor> summons = GamePool.ListPool<BeActor>.Get();
        owner.CurrentBeScene.GetSummonBySummoner(summons, Player);

        for (int i = 0; i < summons.Count; ++i)
        {
            if (!summons[i].IsDead())
            {
                summonsAlive.Add(summons[i]);
            }
        }
        GamePool.ListPool<BeActor>.Release(summons);
    }

    protected List<BeActor> _GetAllAliveSummon()
    {
        List<BattlePlayer> battlePlayerList = owner.CurrentBeBattle.dungeonPlayerManager.GetAllPlayers();
        List<BeActor> aliveSummonList = GamePool.ListPool<BeActor>.Get();
        for (int i = 0; i < battlePlayerList.Count; i++)
        {
            _GetSummonsByPlayer(aliveSummonList, battlePlayerList[i].playerActor);
        }
        GamePool.ListPool<BeActor>.Release(aliveSummonList);
        return aliveSummonList;
    }
}
