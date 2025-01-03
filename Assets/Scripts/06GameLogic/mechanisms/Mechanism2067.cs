using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 地下城阶段切换同步服务器，
/// </summary>
public class Mechanism2067: BeMechanism
{
    private int phase = 0;
    
    public Mechanism2067(int mid, int lv) : base(mid, lv)
    {
    }

    public override void OnInit() 
    {
        phase = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
    }

    public override void OnStart()
    {
#if !LOGIC_SERVER
        InputManager.CreateBossPhaseChange(phase);
#endif
    }
}
