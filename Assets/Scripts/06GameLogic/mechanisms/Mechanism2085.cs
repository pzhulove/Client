using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
阵鬼 残影之凯贾机制 过滤子弹

残影之凯贾配置文件中制作上了释放实体
通过该机制过滤子弹的发射
条件：
该子弹只有在PVE下，开启了冥炎之卡洛时，才能放出子弹。

注意：该机制如果跟随Buff上，当buff结束时，机制移除，但普攻替换却不是马上还原的（等当前技能动画完成），
    因此buff上的机制不能控制子弹过滤。所有将过滤机制移到全局生命周期下
 */

public class Mechanism2085 : BeMechanism
{
    protected HashSet<int> m_launchBullet = new HashSet<int>();

    public Mechanism2085(int sid, int skillLevel) : base(sid, skillLevel)
    {
        
    }

    public sealed override void OnInit()
    {
        m_launchBullet.Clear();
        for (int i = 0; i < data.ValueA.Count; i++)
        {
            m_launchBullet.Add(TableManager.GetValueFromUnionCell(data.ValueA[i], level));
        }   
    }

    public sealed override void OnStart()
    {
        handleA = owner.RegisterEventNew(BeEventType.onBeforeGenBullet, args =>
        {
            if (!owner.IsCastingSkill()) 
                return;
            
            int resID = args.m_Int;
            if (IsLaunch(resID))
            {
                return;
            }

            args.m_Bool = false;
        });
    }
    
    private bool IsLaunch(int resID)
    {
        // 是指定的子弹
        if (IsTargetBullet(resID))   
        {
            // PVP 都不能发出子弹
            if (BattleMain.IsModePvP(battleType))
            {
                return false;
            }

            // 在没有 冥炎之卡洛
            if (owner.GetMechanism(1061) == null)
            {
                return false;
            }
            return true;
        }
        
        return true;
    }

    private bool IsTargetBullet(int resID)
    {
        return m_launchBullet.Contains(resID);
    }
}