using System;
using System.Collections.Generic;
// 分裂机制
public class Mechanism2044 : BeMechanism
{
    int monsterId = 0;
    int dist = 0;
    VFactor hpRate = VFactor.zero;
    public Mechanism2044(int sid, int skillLevel) : base(sid, skillLevel)
    {
         
    }
    public override void OnInit()
    {
        base.OnInit();
        monsterId = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        dist = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        int hp = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
        hpRate = VFactor.NewVFactor(hp, GlobalLogic.VALUE_1000);
    }
    public override void OnStart()
    {
        base.OnStart();
        if (owner == null || owner.CurrentBeScene == null) return;
        BeActor cloneMonster = owner.CurrentBeScene.CreateMonster(monsterId + owner.GetEntityData().GetLevel() * 100, true, null, level, owner.GetCamp(), owner);
        if (cloneMonster == null) return;
        var ownerPos = owner.GetPosition();
        var curHP = owner.attribute.GetHP();
        var monsterHP = curHP * hpRate;
        var newPos = new VInt3(ownerPos.x + dist, ownerPos.y, ownerPos.z);
        if(owner.CurrentBeScene.IsInBlockPlayer(newPos))
        {
            newPos = BeAIManager.FindStandPositionNew(newPos, owner.CurrentBeScene,false, false, 40);
        }
        cloneMonster.SetPosition(newPos);
        cloneMonster.attribute.SetHP(monsterHP);
#if !LOGIC_SERVER
        if(cloneMonster.m_pkGeActor != null)
        {
            cloneMonster.m_pkGeActor.isSyncHPMP = true;
            cloneMonster.m_pkGeActor.SyncHPBar();
            cloneMonster.m_pkGeActor.isSyncHPMP = false;
        }
#endif   
        cloneMonster.StartAI(null);
     //   Logger.LogErrorFormat("clonetmonster {0} hp {1} maxhp {2} hprate {3}", cloneMonster.GetPID(), cloneMonster.attribute.GetHP(), cloneMonster.attribute.GetMaxHP(), cloneMonster.attribute.GetHPRate());
        cloneMonster = owner.CurrentBeScene.CreateMonster(monsterId + owner.GetEntityData().GetLevel() * 100, true, null, level, owner.GetCamp(), owner);
        if (cloneMonster == null) return;
        newPos = new VInt3(ownerPos.x - dist, ownerPos.y, ownerPos.z);
        if (owner.CurrentBeScene.IsInBlockPlayer(newPos))
        {
            newPos = BeAIManager.FindStandPositionNew(newPos, owner.CurrentBeScene,true, false, 40);
        }
        cloneMonster.SetPosition(newPos);
        cloneMonster.attribute.SetHP(monsterHP);
        cloneMonster.StartAI(null);
#if !LOGIC_SERVER
        if (cloneMonster.m_pkGeActor != null)
        {
            cloneMonster.m_pkGeActor.isSyncHPMP = true;
            cloneMonster.m_pkGeActor.SyncHPBar();
            cloneMonster.m_pkGeActor.isSyncHPMP = false;
        }
#endif
        //  Logger.LogErrorFormat("clonetmonster {0} hp {1} maxhp {2} hprate {3}", cloneMonster.GetPID(), cloneMonster.attribute.GetHP(), cloneMonster.attribute.GetMaxHP(), cloneMonster.attribute.GetHPRate());
        owner.attribute.SetHP(-1);
        owner.DoDead(true);
        if (owner.m_pkGeActor != null)
        {
            owner.m_pkGeActor.SetActorVisible(false);
        }
    }
}

