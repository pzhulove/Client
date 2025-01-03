using System;
using System.Collections.Generic;
using GameClient;
using UnityEngine;

/// <summary>
/// 1.在机制的持续时间内，将自身所有的小炫纹替换为对应元素的大炫纹（对大炫纹不生效）							
/// 2.并且当机制存在时，后续生成的炫纹也为大炫纹。							
/// 3.PVP中，只会替换对应元素的大炫纹，后续生成的炫纹为大炫纹效果不生效							
/// 正在飞行中的炫纹不管，不会替换							
/// </summary>
/// 
public class Mechanism2093 : BeMechanism
{
    public Mechanism2093(int mid, int lv) : base(mid, lv) { }
    public override void OnInit()
    {
      
    }
    
    public override void OnStart()
    {
        if(owner == null)
            return;
        
        ChangeChaseSize();
        if (!BattleMain.IsModePvP(BattleMain.battleType))
        {
            handleA = owner.RegisterEventNew(BeEventType.onAddChaser, ChangeCreateChaseSize);
        }
    }
    
    private void ChangeChaseSize()
    {
        if(owner == null)
            return;
        
        var mechaism = owner.GetMechanism(Mechanism2072.ChaserMgrID);
        if (mechaism == null)
        {
            Logger.LogErrorFormat("未找到机制2072");
            return;
        }

        var chaseMgr = mechaism as Mechanism2072;
        if (chaseMgr != null)
        {
            chaseMgr.SetChaseSize(Mechanism2072.ChaseSizeType.Big);
        }
    }

    private void ChangeCreateChaseSize(BeEvent.BeEventParam param)
    {
        Mechanism2072.ChaseSizeType size = (Mechanism2072.ChaseSizeType)param.m_Int2;
        if (size != Mechanism2072.ChaseSizeType.Big)
        {
            param.m_Int2 = (int) Mechanism2072.ChaseSizeType.Big;
        }
    }
}

