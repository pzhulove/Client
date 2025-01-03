using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 团本水人——喷泉技能 在每一个喷泉只召唤一个水球，召唤位置为攻击目标位置
/// 该机制不再通用
/// </summary>
public class Mechanism1044 : BeMechanism
{
    public Mechanism1044(int id, int level) : base(id, level) { }

    private int effectID = 0;     //触发效果表id
    private IBeEventHandle bulletEventHandle;
    private IBeEventHandle summonEventHandle;
    private List<IBeEventHandle> hurtEventHandle = new List<IBeEventHandle>();
    private int summonID = 0;
    private int bulletID = 0;

    private List<int> bulletPidList = new List<int>();
    public override void OnInit()
    {
        effectID = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        summonID = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        bulletID = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
    }

    public override void OnReset()
    {
        ClearEventHandle();
    }

    public override void OnStart()
    {
        ClearEventHandle();
        if (bulletPidList != null)
        {
            bulletPidList.Clear();
        }
        else
        {
            bulletPidList = new List<int>();
        }
        bulletEventHandle = owner.RegisterEventNew(BeEventType.onAfterGenBullet, (args) =>
        {
            BeProjectile bullet = args.m_Obj as BeProjectile;
            if (bullet == null || bullet.m_iResID != bulletID)
            {
                return;
            }

            handleA = OwnerRegisterEventNew(BeEventType.onHitOther, args1 =>
            //hurtEventHandle.Add(bullet.RegisterEvent(BeEventType.onHitOther, (object[] args1) =>
           {
               if (bulletPidList.Contains(bullet.GetPID()))
               {
                   return;
               }
               if (args1 .m_Int == effectID)
               {
                   BeActor target = args1.m_Obj as BeActor;
                   if (target != null)
                   {
                       summonEventHandle = owner.RegisterEventNew(BeEventType.onSummon, (args2) =>
                       {
                           BeActor monster = args2.m_Obj as BeActor;
                           if (!monster.GetEntityData().MonsterIDEqual(summonID))
                           {
                               return;
                           }
                           if (monster != null)
                                   {
                               monster.SetPosition(target.GetPosition(), true);
                                   }
                           if (summonEventHandle != null)
                           {
                               summonEventHandle.Remove();
                               summonEventHandle = null;
                           }
                       });
                       owner.DoSummon(summonID,owner.GetEntityData().level);
                       bulletPidList.Add(bullet.GetPID());
                   }
               }
           });
        });
    }

    public override void OnFinish()
    {
        if(bulletPidList != null)
        {
            bulletPidList.Clear();
        }
        ClearEventHandle();
    }

    private void ClearEventHandle()
    {
        if(bulletEventHandle != null)
        {
            bulletEventHandle.Remove();
            bulletEventHandle = null;
        }
        if(summonEventHandle != null)
        {
            summonEventHandle.Remove();
            summonEventHandle = null;
        }
        if (hurtEventHandle != null)
        {
            for (int i = 0; i < hurtEventHandle.Count; ++i)
            {
                if (hurtEventHandle[i] != null)
                {
                    hurtEventHandle[i].Remove();
                    hurtEventHandle[i] = null;
                }
            }
        }
    }
}
