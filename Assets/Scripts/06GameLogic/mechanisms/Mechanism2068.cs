using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 团本正面减伤机制
/// </summary>
public class Mechanism2068: BeMechanism
{
    private VFactor per = VFactor.zero;
    private IBeEventHandle onReduceDamageEvent = null;
    List<int> hurtIDList = new List<int>();//这个存是觉醒技能触发效果表id
    List<int> hurtIDList1 = new List<int>();//这个存非觉醒技能触发效果表id

    public Mechanism2068(int mid, int lv) : base(mid, lv)
    {
    }

    public override void OnInit() 
    {
        per = VFactor.NewVFactor(Mathf.Clamp(100 - TableManager.GetValueFromUnionCell(data.ValueA[0], level), 0, 100), GlobalLogic.VALUE_100);
    }

    public override void OnReset()
    {
        if (onReduceDamageEvent != null)
        {
            onReduceDamageEvent.Remove();
            onReduceDamageEvent = null;
        }
        hurtIDList.Clear();
        hurtIDList1.Clear();
    }

    public override void OnStart()
    {
        ClearEventHandle();
        ResetData();
        onReduceDamageEvent = owner.RegisterEventNew(BeEventType.onBeHitAfterFinalDamage, args =>
        //onReduceDamageEvent = owner.RegisterEvent(BeEventType.onBeHitAfterFinalDamage, (object[] args) => 
        {
            //int[] damage = (int[])args[0];
            VInt3 hitPos = args.m_Vint3;
            int hurtID = args.m_Int2;
            if (isAwakeSkill(hurtID))
            {
                return;
            }

            if (isFrontDamage(hitPos))
            {
                args.m_Int = args.m_Int * per;
            }
        });
    }

    public override void OnFinish()
    {
        ClearEventHandle();
        ResetData();
    }

    private void ClearEventHandle()
    {
        if(onReduceDamageEvent != null)
        {
            onReduceDamageEvent.Remove();
            onReduceDamageEvent = null;
        }
    }

    private void ResetData()
    {
        if(hurtIDList != null)
        {
            hurtIDList.Clear();
        }
        else
        {
            hurtIDList = new List<int>();
        }
        if(hurtIDList1 != null)
        {
            hurtIDList1.Clear();
        }
        else
        {
            hurtIDList1 = new List<int>();
        }
    }

    private bool isFrontDamage(VInt3 hitPos)
    {
        if (!owner.GetFace())
        {
            if (hitPos.x - owner.GetPosition().x >= 0)
            {
                return true;
            }
        }
        else
        {
            if (owner.GetPosition().x - hitPos.x >= 0)
            {
                return true;
            }
        }
        return false;
    }

    //二次缓存
    private bool isAwakeSkill(int hurtID)
    {
        if (hurtIDList == null || hurtIDList1 == null)
        {
            return false;
        }
        if (hurtIDList.Contains(hurtID))
        {
            return true;
        }
        else if (hurtIDList1.Contains(hurtID))
        {
            return false;
        }
        else
        {
            var hurtTable = TableManager.GetInstance().GetTableItem<ProtoTable.EffectTable>(hurtID);
            if (hurtTable == null)
            {
                hurtIDList1.Add(hurtID);
                return false;
            }
            else

            {
                var skillTable = TableManager.GetInstance().GetTableItem<ProtoTable.SkillTable>(hurtTable.SkillID);
                if (skillTable == null)
                {
                    hurtIDList1.Add(hurtID);
                    return false;
                }
                else if (skillTable.SkillCategory == 4)
                {
                    hurtIDList.Add(hurtID);
                    return true;
                }
                else
                {
                    hurtIDList1.Add(hurtID);
                    return false;
                }
            }
        }
    }
}
