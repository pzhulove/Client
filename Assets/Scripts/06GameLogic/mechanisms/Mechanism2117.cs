using System;
using System.Collections.Generic;
using GameClient;
using UnityEngine;

/// <summary>
/// PK场 浮空保护伤害减免机制
/// </summary>
public class Mechanism2117 : BeMechanism
{
    public Mechanism2117(int mid, int lv) : base(mid, lv)
    {
    }

    private VFactor m_FallPreProtect;
    private VFactor m_FallPostProtect;
    public override void OnInit()
    {
        base.OnInit();
        m_FallPreProtect = VFactor.NewVFactor(TableManager.GetValueFromUnionCell(data.ValueA[0], level), GlobalLogic.VALUE_1000);
        m_FallPostProtect = VFactor.NewVFactor(TableManager.GetValueFromUnionCell(data.ValueB[0], level), GlobalLogic.VALUE_1000);
    }

    public override void OnStart()
    {
        base.OnStart();
        handleA = owner.RegisterEventNew(BeEventType.onChangeHurtValue, OnHurt);
    }

    private void OnHurt(BeEvent.BeEventParam args)
    {
        if(!Enable())
            return;
        
        // int[] arr = (int[]) args[0];
        // int damage = arr[0];
        int damage = args.m_Int;

        var factor = GetFallProtectHurtFactor();
        //Debug.LogError("OnHurt:" + damage + "->" + damage * factor + "   " + factor.single);
        // arr[0] = damage * factor;
        args.m_Int = damage * factor;
        
        // 附加伤害显示处理
        // if(args.m_Int2 == null)
        //     return;
        // var attachHurt = args[1] as List<int>;
        // if(attachHurt == null)
        //     return;
        // for (int i = 0; i < attachHurt.Count; i++)
        // {
        //     attachHurt[i] *= factor;
        // }
        // args[1] = attachHurt;
    }

    private bool Enable()
    {   
        if (owner.CurrentBeBattle == null) 
            return false;
        // if (owner.CurrentBeBattle.FunctionIsOpen(BattleFlagType.PKFallProtectHurtRate))
        //     return false;
        
        if (!owner.isMainActor)
            return false;
        
        if (owner.protectManager == null || !owner.protectManager.IsEnable())
            return false;

        return BattleMain.IsProtectFloat(battleType);
    }
    
    private VFactor GetFallProtectHurtFactor()
    {
        VFactor ret = VFactor.one;
        var protect = owner.protectManager;
        if (protect == null)
            return ret;

        if (protect.FallHurtCounting)
        {
            if (protect.FallHurtEffect > 0)
            {
                // 打过了浮空条
                ret = m_FallPostProtect;
            }
            else
            {
                // 还没打到浮空条
                ret = m_FallPreProtect;
            }
        }

        return ret;
    }
}
