using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameClient;

/*
技能屏幕动画机制
 */

public class Mechanism43 : BeMechanism
{

    protected int m_DelayRemoveTime = 0;  //动画延时删除时间
    protected string m_AniPath;        //鬼影闪动画路径

    public Mechanism43(int sid, int skillLevel) : base(sid, skillLevel)
    {

    }

    public override void OnInit()
    {
        m_AniPath = data.StringValueA[0];
        m_DelayRemoveTime = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
    }

#if !LOGIC_SERVER
    public override void OnStart()
    {
        var battleUI = BattleUIHelper.CreateBattleUIComponent<BattleUIProfession>();;
        if (battleUI != null && owner.isLocalActor)
            battleUI.PlayCharacterAni(owner.GetFace(), m_AniPath, m_DelayRemoveTime);
    }
#endif
}