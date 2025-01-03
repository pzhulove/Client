using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//在每个玩家位置创建一个实体
public class Mechanism1042 : BeMechanism
{
    private int entityId = 0;   //召唤的实体ID
    protected bool m_CreateOnGround = false;    //创建在地面

    public Mechanism1042(int id, int level) : base(id, level) { }

    public override void OnInit()
    {
        base.OnInit();
        entityId = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        m_CreateOnGround = TableManager.GetValueFromUnionCell(data.ValueB[0], level) == 0 ? false : true;
    }


    public override void OnStart()
    {
        base.OnStart();
        CreateEntity();
    }

    /// <summary>
    /// 在玩家位置创建实体
    /// </summary>
    private void CreateEntity()
    {
        if (owner.CurrentBeBattle == null)
            return;
        List<BattlePlayer> list = owner.CurrentBeBattle.dungeonPlayerManager.GetAllPlayers();
        if (list != null)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].playerActor == null)
                    continue;
                var pos = list[i].playerActor.GetPosition();
                if (m_CreateOnGround)
                {
                    pos.z = 0;
                }
                owner.AddEntity(entityId, pos);
            }
        }
    }
}
