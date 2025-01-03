using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 周长深渊正负极机制
/// </summary>
public class Mechanism2013 : BeMechanism {
    readonly int positiveBuffID = 558929;
    readonly int negativeBuffID = 558930;
    readonly int hurtID = 202022;
    readonly int hurtID1 = 202021;
    VInt dis = new VInt(2.5f);
    readonly int effectBuffInfoID = 568933;
    int selfFlag = 0;
    int totalTime = 20000;
    readonly int bossID = 50800031;
    List<BeActor> list = new List<BeActor>();
    public Mechanism2013(int mid, int lv) : base(mid, lv) { }

    public override void OnInit()
    {
        base.OnInit();
        dis = new VInt( TableManager.GetValueFromUnionCell(data.ValueA[0], level)/1000.0f);
        totalTime = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
    }

    public override void OnReset()
    {
        selfFlag = 0;
        list.Clear();
        damageTimer = 0;
    }

    /// <summary>
    /// 根据队友的正负极来造成伤害
    /// </summary>
    public override void OnStart()
    {
        base.OnStart();

        handleA = owner.RegisterEventNew(BeEventType.onAddBuff, (args) => 
        {           
            BeBuff buff = args.m_Obj as BeBuff;
            if (buff.buffID == positiveBuffID)
                selfFlag = 1;
            if (buff.buffID == negativeBuffID)
                selfFlag = -1;
        });
        handleB = owner.RegisterEventNew(BeEventType.onRemoveBuff, (args) =>
        {
            int id = (int)args.m_Int;
            if(id==positiveBuffID||id==negativeBuffID)
               selfFlag = 0;
        });
    }

    int damageTimer = 0;
    public override void OnUpdate(int deltaTime)
    {
        base.OnUpdate(deltaTime);
        if (owner == null || owner.IsDead()) return;

        damageTimer += deltaTime;
        if (damageTimer <= 100) return;
        damageTimer = 0;
        CalcHp();
    }

    /// <summary>
    /// 正负极靠近之后可以抵消，其他的都对队友造成伤害
    /// </summary>
    private void CalcHp()
    {
        List<BeActor> list = GamePool.ListPool<BeActor>.Get();
        owner.CurrentBeScene.FindMainActorInRange(list,owner.GetPosition(),dis);
        if (list.Count < 2)
        {
            owner.StopSpellBar(eDungeonCharactorBar.Progress);
        }
        for (int i = 0; i < list.Count; i++)
        {
            
            if (list[i] == owner) continue ;
            BeBuff posBuff = list[i].buffController.HasBuffByID(positiveBuffID);
            BeBuff negBuff = list[i].buffController.HasBuffByID(negativeBuffID);
            var hitPos = owner.GetPosition();
            hitPos.z += VInt.one.i;
            if (selfFlag == 0)
            {
                if (posBuff != null || negBuff != null)
                {
                    OnHurtEntity(hurtID);
                }
            }
            else if (selfFlag == 1)
            {
                if (posBuff != null)
                {
                    OnHurtEntity( hurtID1);
                }
                else if (negBuff != null)
                {
                    SetSpellBar(list[i]);
                }
                else if (negBuff == null)
                {
                    owner.StopSpellBar(eDungeonCharactorBar.Progress);
                }
            }
            else if (selfFlag == -1)
            {
                if (negBuff != null)
                {
                    OnHurtEntity( hurtID1);
                }
                else if (posBuff != null)
                {
                    SetSpellBar(list[i]);
                }
                else if (posBuff == null)
                {
                    owner.StopSpellBar(eDungeonCharactorBar.Progress);
                }
            }
        }
        GamePool.ListPool<BeActor>.Release(list);
    }

    
    private void OnHurtEntity(int hurtID)
    {             
        List<BeActor> list = GamePool.ListPool<BeActor>.Get();
        owner.CurrentBeScene.FindMonsterByID(list,bossID);
        if (list.Count > 0)
        {
            var hitPos = owner.GetPosition();
            hitPos.z += VInt.one.i;
            list[0]._onHurtEntity(owner, hitPos, hurtID);
        }
        GamePool.ListPool<BeActor>.Release(list);
    }

    private void SetSpellBar(BeActor actor)
    {
        SpellBar bar = null;
        var dur = owner.GetSpellBarDuration(eDungeonCharactorBar.Progress);
        if (dur <= 0)
        {
            bar = owner.StartSpellBar(eDungeonCharactorBar.Progress, totalTime);
        }
        
        if(totalTime-100<=dur)
        {
            owner.buffController.RemoveBuff(selfFlag==1?positiveBuffID:negativeBuffID);
            owner.buffController.TryAddBuff(effectBuffInfoID);
            actor.buffController.RemoveBuff(positiveBuffID );
            actor.buffController.RemoveBuff( negativeBuffID);
            actor.buffController.TryAddBuff(effectBuffInfoID);
        }
    }
}
