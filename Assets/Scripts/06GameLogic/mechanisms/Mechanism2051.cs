using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
/// <summary>
/// 团本机制7，四彩气泡
/// </summary>
public class Mechanism2051 : BeMechanism
{
    private int monsterID = 87030031;
    private int hitAttachCount = 0;
    private int buffID = 570037;
    private int[] hurtIDs = new int[] { 210590, 210592, 210593, 210594 };
    private int baseDamage = 20;
    private int[] buffIDs = new int[] { 570041, 570042, 570043, 570044 };
    private int[] effectBuffIDs = new int[] { 570033, 570034, 570035, 570036 };
    private int curBuffID = -1;
    private int damagePercent = 0;
    private IBeEventHandle handle;
    public Mechanism2051(int mid, int lv) : base(mid, lv)
    {
    }
    public override void OnInit()
    {
        base.OnInit();
        baseDamage = TableManager.GetValueFromUnionCell(data.ValueA[0],level);
        damagePercent = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        var tempMonsterId = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
        if(tempMonsterId != 0)
        {
            monsterID = tempMonsterId;
        }
    }

    public override void OnReset()
    {
        hitAttachCount = 0;
        curBuffID = -1;
        if (handle != null)
        {
            handle.Remove();
            handle = null;
        }
        monsterID = 87030031;
    }

    public override void OnStart()
    {
        base.OnStart();
        BeActor boss = owner.CurrentBeScene.FindMonsterByID(monsterID);
        if (boss != null)
        {
            handleA = boss.RegisterEventNew(BeEventType.onAddBuff, (args) =>
            {
                BeBuff buff = args.m_Obj as BeBuff;
                if (buff != null)
                {
                    if (buff.buffID == buffID)
                    {
                        hitAttachCount = 0;
                    }

                    int index = Array.IndexOf(effectBuffIDs, buff.buffID);
                    if (index != -1)
                    {
                        curBuffID = buffIDs[index];
                    }
                }
            });

            handle = boss.RegisterEventNew(BeEventType.onRemoveBuff, (args) =>
            {
                int buffID = (int)args.m_Int;

                int index = Array.IndexOf(effectBuffIDs, buffID);
                if (index != -1)
                {
                    curBuffID = -1;
                }

            });

            handleC = boss.RegisterEventNew(BeEventType.onBeHitAfterFinalDamage, args =>
            //handleC = boss.RegisterEvent(BeEventType.onBeHitAfterFinalDamage, (args) => 
            {
                if (curBuffID == -1) return;
                BeEntity attacker = args.m_Obj as BeEntity;
                BeEntity owner = attacker.GetOwner();
                if (owner != null)
                {
                    BeActor actor = owner as BeActor;
                    if (actor != null && actor.buffController.HasBuffByID(curBuffID)==null)
                    {
                        //int[] damage = args.m_Int as int[];
                        args.m_Int = args.m_Int * VFactor.NewVFactor(damagePercent, 100);
                    }
                }
            });
        }

        handleB = owner.RegisterEventNew(BeEventType.onAfterCalFirstDamage, args =>
        //handleB = owner.RegisterEvent(BeEventType.onAfterCalFirstDamage, (args) => 
        {
            bool beHit = args.m_Bool;
            if (beHit)
            {
                //int damage = args.m_Int;
                int hurtID = args.m_Int2;
                if (Array.IndexOf(hurtIDs, hurtID) != -1)
                {
                    hitAttachCount++;
                    args.m_Int += baseDamage * hitAttachCount;
                }
            }
        });
        handleD = owner.RegisterEventNew(BeEventType.onBuffBeforePostInit, (args) =>
        {
            BeBuff buff = args.m_Obj as BeBuff;
            if (buff != null)
            {               
                if (Array.IndexOf(buffIDs, buff.buffID) != -1)
                {
                    DeleteBeforeBuff();
                }
            }
        });
    }

    private void DeleteBeforeBuff()
    {
        for (int i = 0; i < buffIDs.Length; i++)
        {
            if (owner.buffController.HasBuffByID(buffIDs[i]) != null)
            {
                owner.buffController.RemoveBuff(buffIDs[i]);
            }
        }
    }

    public override void OnFinish()
    {
        base.OnFinish();
        if (handle != null)
        {
            handle.Remove();
            handle = null;
        }
    }
}
