using System;
using System.Collections.Generic;
public interface IEntityFilter
{
    bool isUseDefault();
    bool isFit(BeEntity target);
}

public class TrueEntityFilter : IEntityFilter
{
    public static TrueEntityFilter Filter= new TrueEntityFilter();
    
    public bool isUseDefault()
    {
        return true;
    }
    
    public bool isFit(BeEntity target)
    {
        return true;
    }
}

public class BeSummonMonsterIdFilter : IEntityFilter
{
    public List<int> summonMonsterId = null;
    public int ownerId;
    public bool isFit(BeEntity monster)
    {
        if (ownerId == 0 || summonMonsterId == null || monster == null) return false;
        var summoner = monster.GetTopOwner(monster);
        if (summoner != null && summoner.GetPID() == ownerId)
        {
            var iter = summonMonsterId.GetEnumerator();
            while (iter.MoveNext())
            {
                if (monster.GetEntityData().MonsterIDEqual(iter.Current))
                {
                    return true;
                }
            }
            return false;
        }
        return false;
    }
    public bool isUseDefault()
    {
        return true;
    }
}
public class BeBuffStatIDFilter : IEntityFilter
{
    public int statId = -2;
    public bool isFit(BeEntity monster)
    {
        if (statId == -2 || monster == null || monster.stateController == null) return false;
        if (monster.stateController.HasBuffState((BeBuffStateType)statId))
        {
            return true;
        }
        return false;
    }
    public bool isUseDefault()
    {
        return false;
    }
}
public class BeMonsterIDFilter: IEntityFilter
{
    List<int> _monsterids = null;
    public BeMonsterIDFilter(List<int> monsterId)
    {
        _monsterids = monsterId;
    }
    public void Init(List<int> monsterId)
    {
        _monsterids = monsterId;
    }
    public bool isFit(BeEntity monster)
    {
        if (monster == null || monster.GetEntityData() == null) return false;
        for (int i = 0; i < _monsterids.Count; i++)
        {
            if (monster.GetEntityData().MonsterIDEqual(_monsterids[i]))
                return true;
        }
        return false;
    }

    public bool isUseDefault()
    {
        return true;
    }
}

public class BeStateFilter : IEntityFilter
{
    BeBuffStateType stateType;
    public BeStateFilter(BeBuffStateType type)
    {
        stateType = type;
    }

    public bool isFit(BeEntity target)
    {
        BeActor actor = target as BeActor;
        if (actor == null) return false;
        return actor.stateController.HasBuffState(stateType);
    }

    public bool isUseDefault()
    {
        return false;
    }
}

public class BeMechanismFilter : IEntityFilter
{
    int  mechanismIndex;
    public BeMechanismFilter(int id)
    {
        mechanismIndex = id;
    }

    public bool isFit(BeEntity target)
    {
        BeActor actor = target as BeActor;
        if (actor == null) return false;
        BeMechanism mechanism = actor.GetMechanismByIndex(mechanismIndex);
        return mechanism != null;
    }

    public bool isUseDefault()
    {
        return false;
    }
}


public class BeHeightFilter : IEntityFilter
{
    VInt _height;
    public BeHeightFilter(VInt height)
    {
        _height = height;
    }
    public bool isFit(BeEntity target)
    {
        // target.GetPosition().z
        return false;
    }
    public bool isUseDefault()
    {
        return false;
    }
}
public class BeCampFilter : IEntityFilter
{
    int _camp;
    public BeCampFilter(int camp)
    {
        _camp = camp;
    }
    public bool isFit(BeEntity target)
    {
        return target.GetCamp()==_camp;
    }

    public bool isUseDefault()
    {
        return false;
    }
}
public class BeTargetEntityTypeFilter:IEntityFilter
{
    public BeAIManager.TargetEntityType targetEntityType;
    public bool isBoss = false;
    public bool isFit(BeEntity target)
    {
        var targetActor = target as BeActor;
        if (target == null) return false;
        if (targetEntityType == BeAIManager.TargetEntityType.DEFAULT)
        {
            if(isBoss)
            {
                return targetActor.IsMonster() && targetActor.IsBoss();
            }
            return targetActor.IsMonster() && targetActor.attribute.autoFightNeedAttackFirst;
        }
        else if (targetEntityType == BeAIManager.TargetEntityType.PLAYER)
        {
            if (target.CurrentBeBattle == null ||
                target.CurrentBeBattle.dungeonPlayerManager == null ||
                target.CurrentBeBattle.dungeonPlayerManager.GetAllPlayers() == null)
            {
                return false;
            }
            List<BattlePlayer> players = target.CurrentBeBattle.dungeonPlayerManager.GetAllPlayers();
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].playerActor == null) continue;
                if (players[i].playerActor.GetPID() == target.GetPID())
                {
                    return true;
                }
            }
        }
        else if (targetEntityType == BeAIManager.TargetEntityType.MONSTER)
        {
            if(isBoss)
            {
                return targetActor.IsMonster() && targetActor.IsBoss();
            }
            return targetActor.IsMonster();
        }
        else if (targetEntityType == BeAIManager.TargetEntityType.SUMMON)
        {
            return targetActor.GetEntityData().isSummonMonster;
        }
        return false;
    }

    public bool isUseDefault()
    {
        return true;
    }
}

/// <summary>
/// 行为树过滤器
/// </summary>
public class BTActorFilter : IEntityFilter
{
    private behaviac.Filter m_filter;
    private BeActor m_self;
    public BTActorFilter(behaviac.Filter filter, BeActor self)
    {
        m_filter = filter;
        m_self = self;
    }
    public bool isUseDefault()
    {
        return true;
    }

    public bool isFit(BeEntity target)
    {
        var actor = target as BeActor;
        if (actor == null)
            return false;

        return AgentBase.FilterActor(actor, m_self, m_filter);
    }
}

/// <summary>
/// 移动管理那边的过滤器
/// </summary>
public class BeActionActorFilter : IEntityFilter
{
    private bool isSpecialBuff = false; //是否是冰冻 石化Buff
    private bool isBati = false;    //是否有霸体
    private bool isAbsorb = false;  //是否被抓取
    private bool isGeDang = false;  //是否格挡
    private bool isFallgroundOrFloat = false;   //是否到底或者浮空

    public void Init(bool special = false,bool bati = false,bool absorb = false,bool geDang = false,bool fallOrFloat = false)
    {
        isSpecialBuff = special;
        isBati = bati;
        isAbsorb = absorb;
        isGeDang = geDang;
        isFallgroundOrFloat = fallOrFloat;
    }

    public bool isFit(BeEntity target)
    {
        BeActor actor = (BeActor)target;
        if (actor == null || actor.IsDead())
            return false;
        if (actor.grabController.isAbsorb)
            return false;
        //不能瞬移霸体怪物
        if (actor.buffController == null || actor.buffController.HaveBatiBuff() || actor.buffController.HasBuffByID((int)GlobalBuff.GEDANG) != null)
            return false;
        if (actor.stateController == null || !actor.stateController.HasBornAbility(BeAbilityType.FALLGROUND) || !actor.stateController.HasBornAbility(BeAbilityType.FLOAT))
            return false;
        if(isSpecialBuff && (actor.stateController.HasBuffState(BeBuffStateType.FROZEN) || actor.stateController.HasBuffState(BeBuffStateType.STONE)))
            return false;
        return true;
    }

    public bool isUseDefault()
    {
        return true;
    }
}

/// <summary>
/// 获取召唤师的召唤兽
/// </summary>
public class BeMagicGirlSummonMonsterFilter : IEntityFilter
{
    int ownerPId = -1;

    public bool isFit(BeEntity target)
    {
        if (target.IsDead())
            return false;
        if (ownerPId > 0 && target.GetPID() != ownerPId)
            return false;
        if (!target.IsSummonMonster())
            return false;
        return true;
    }

    public bool isUseDefault()
    {
        return true;
    }
}
public class BeAbilityEnable : IEntityFilter
{
    public int abType = -1;
    public bool isFit(BeEntity target)
    {
        BeActor actor = target as BeActor;
        if (actor == null || actor.stateController == null)
            return false;

        if(abType <= -1 || abType >= (int)BeAbilityType.COUNT)
        {
            return false;
        }
        return actor.stateController.HasAbility((BeAbilityType)abType);
    }
    public bool isUseDefault()
    {
        return false;
    }
}

/// <summary>
/// 获取所有敌方阵营怪物 非玩家阵营
/// </summary>
public class BeAllEnemyMonsterFilter : IEntityFilter
{
    public bool containSkillMonster = true;

    public bool isFit(BeEntity target)
    {
        BeActor actor = target as BeActor;
        if (actor == null)
            return false;
        if (!actor.IsMonster())
            return false;
        if (actor.m_iCamp == (int)ProtoTable.UnitTable.eCamp.C_HERO)
            return false;
        if (actor.GetEntityData() == null)
            return false;
        if (!containSkillMonster && actor.GetEntityData().type == (int)ProtoTable.UnitTable.eType.SKILL_MONSTER)
            return false;
        return true;
    }

    public bool isUseDefault()
    {
        return true;
    }
}

/// <summary>
/// 获取所有敌方阵营怪物 非玩家阵营
/// </summary>
public class BeGetRangeFriendNotSummon : IEntityFilter
{
    public bool isFit(BeEntity target)
    {
        BeActor actor = target as BeActor;
        if (actor == null)
            return false;
        if (actor.IsSummonMonster())
            return false;
        return true;
    }

    public bool isUseDefault()
    {
        return true;
    }
}


/// <summary>
/// 获取内圆环内的目标
/// </summary>
public class BeGetConcentricCircleTarget : IEntityFilter
{
    public BeActor m_Owner;
    public VInt2 m_OwnerPosXY;
    public VInt m_MinCircleRadius;  //内圆半径
    public VInt m_MaxCircleRadius;  //外圆半径

    public static VInt LargeMaxCirleRadius = VInt.NewVInt(999999, GlobalLogic.VALUE_1000);

    public bool isFit(BeEntity target)
    {
        BeActor actor = target as BeActor;
        if (actor == null)
            return false;
        if (actor.IsDead())
            return false;
        if (m_Owner.IsDead() || m_Owner == actor || m_Owner.m_iCamp == actor.m_iCamp)
            return false;
        if (actor.IsSkillMonster())
            return false;
        if(!actor.stateController.CanBeTargeted())
            return false;
        VInt2 targetPosXY = new VInt2(actor.GetPosition().x, actor.GetPosition().y);
        int dis = (targetPosXY - m_OwnerPosXY).magnitude;
        if (dis < m_MinCircleRadius.i || dis > m_MaxCircleRadius.i)
            return false;
        return true;
    }

    public bool isUseDefault()
    {
        return false;
    }
}

/// <summary>
/// 敌方actor
/// </summary>
public class BeActorFilter : IEntityFilter
{
    private BeActor m_owner;
    public BeActorFilter(BeActor owner):base()
    {
        m_owner = owner;
    }
    public bool isFit(BeEntity target)
    {
        BeActor actor = target as BeActor;
        if (actor == null)
            return false;
        
        if (actor.IsDeadOrRemoved())
            return false;

        if(actor.m_iCamp == m_owner.m_iCamp)
            return false;
        
        if (actor.IsSkillMonster())
            return false;
        
        if (!actor.stateController.CanBeTargeted())
            return false;
        
        return true;
    }
    public bool isUseDefault()
    {
        return true;
    }
}
