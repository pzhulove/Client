using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

//倒计时

public class Buff2000003 : BeBuff
{
    protected string m_CountDownPath = "UIFlatten/Prefabs/BattleUI/DungeonCountDown";

#if !SERVER_LOGIC 

    protected GameObject m_CountDownPrefab = null;

 #endif


    public Buff2000003(int bi, int buffLevel, int buffDuration, int attack = 0) : base(bi, buffLevel, buffDuration, attack)
    {

    }

    public override void OnInit()
    {
 #if !SERVER_LOGIC 

       m_CountDownPrefab = CGameObjectPool.instance.GetGameObject(m_CountDownPath, enResourceType.BattleScene, (uint)GameObjectPoolFlag.ReserveLast);

 #endif

    }

    public override void OnStart()
    {
        _AddCountPrefab();
    }

    protected void _AddCountPrefab()
    {
#if !SERVER_LOGIC 

        GameClient.ShowCountDownComponent countDown = m_CountDownPrefab.GetComponent<GameClient.ShowCountDownComponent>();
        countDown.InitData(3);
        Battle.GeUtility.AttachTo(m_CountDownPrefab, owner.m_pkGeActor.goInfoBar);

 #endif

    }

}
