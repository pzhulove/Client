using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
    点名机制
 */
public class Mechanism33 : BeMechanism
{
    protected int m_CallNameBuff = 0;           //点名Buff
    protected int m_Num = 0;                    //点名数量

    protected List<BattlePlayer> m_TargetList = new List<BattlePlayer>();     //攻击目标玩家

    public Mechanism33(int mid, int lv) : base(mid, lv)
    {
        
    }

    public override void OnReset()
    {
        m_TargetList.Clear();
    }
    public override void OnInit()
    {
        m_CallNameBuff = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        m_Num = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
    }

    public override void OnStart()
    {
        AddBuff();
    }

    //添加点名标记buff
    protected void AddBuff()
    {
        List<BeActor> selectPlayerList = new List<BeActor>();
        List<BattlePlayer> playerList =  owner.CurrentBeBattle.dungeonPlayerManager.GetAllPlayers();
        if(playerList.Count>m_Num)
        {
            for (int i = 0; i < m_Num; i++)
            {
                int random = FrameRandom.InRange(0, playerList.Count);
                if (random<playerList.Count)
                {
                    selectPlayerList.Add(playerList[random].playerActor);
                }
            }
        }
        else
        {
            for (int j = 0; j < playerList.Count; j++)
            {
                selectPlayerList.Add(playerList[j].playerActor);
            }
        }

        for (int i = 0; i < selectPlayerList.Count; i++)
        {
            var actor = selectPlayerList[i];
            actor.buffController.TryAddBuff(m_CallNameBuff);
        }
    }
}
