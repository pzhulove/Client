using System.Collections;
using System.Collections.Generic;
using GameClient;

/// <summary>
/// 团本联锁之音机制
/// </summary>
public class Mechanism2039 : BeMechanism
{
    private int buffDuration = -1;
    private string chainEffect = string.Empty;
    private VInt chainMaxDistance = VInt.one.i * 5;
    private int buffID;
    private int breakTime;

    //new 
    private BeEvent.BeEventHandleNew bossDeadHandle;
    private BeEvent.BeEventHandleNew firstDeadHandle;

    int chainTimes = 3;
    public Mechanism2039(int mid, int lv) : base(mid, lv)
    {
    }

    class ChainUnit
    {
        public BeActor self;
        public IBeEventHandle handle;
        public IBeEventHandle monsterChangeHandle;
        public IBeEventHandle monsterRestoreHandle;
        public IBeEventHandle fakeRebornHandle;
        public byte chainNum;
    }

    List<ChainUnit> chainList = new List<ChainUnit>();
    List<BeActor> allTeamPlayersList = new List<BeActor>();

    public override void OnInit()
    {
        chainEffect = data.StringValueA[0];
        chainMaxDistance = VInt.NewVInt(TableManager.GetValueFromUnionCell(data.ValueA[0], level), GlobalLogic.VALUE_1000);
        buffID = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        buffDuration = TableManager.GetValueFromUnionCell(data.ValueB[1], level);
        breakTime = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
    }

    public override void OnStart()
    {
        Clear();
        allTeamPlayersList.Clear();
        chainTimes = 3;
     
        if (owner.CurrentBeBattle != null && owner.CurrentBeBattle.dungeonPlayerManager != null)
        {
            var dungeonPlayers = owner.CurrentBeBattle.dungeonPlayerManager.GetAllPlayers();
            if(dungeonPlayers == null)
            {
                return;
            }
            for(int i = 0; i < dungeonPlayers.Count; ++i)
            {
                if (dungeonPlayers == null || dungeonPlayers[i].playerActor == null) 
                {
                    return;
                }
                if(owner.GetPID() != dungeonPlayers[i].playerActor.GetPID())
                {
                    allTeamPlayersList.Add(dungeonPlayers[i].playerActor);
                }
            }
        }

        var target = GetPlayerInDistance(owner);
        BeActor toActor = null;
        if (target != null)
        {
            CreateChainEffect(owner, target);
            bossDeadHandle = owner.RegisterEventNew(BeEventType.onDead, eventParam => 
            {
                ClearChainEffect(owner);
                ClearNewHandle();
            });
            firstDeadHandle = target.RegisterEventNew(BeEventType.onDead, eventParam =>
            {
                ClearChainEffect(owner);
                ClearNewHandle();
            });
            owner.delayCaller.DelayCall(breakTime, () => 
            {
                ClearChainEffect(owner);
                ClearNewHandle();
            });

            toActor = GetPlayerInDistance(target);
        }
        if( target != null && toActor != null)
        {
            chainTimes--;
            AddChain(target, toActor);

            ChainNext();
        }
    }

    public override void OnUpdate(int deltaTime)
    {
        UpdateActorDistance();
    }

    private void AddChain(BeActor fromActor,BeActor toActor)
    {
        //Effect logic
        CreateChainEffect(fromActor, toActor);

        //Mechainsm logic
        var fromUnit = getUnitInChain(fromActor);
        var toUnit = getUnitInChain(toActor);

        AddUnitEventHandle(fromUnit);
        AddUnitEventHandle(toUnit);

        AddDeBuff(fromUnit);
        AddDeBuff(toUnit);
    }

    private void ChainNext()
    {
        if (chainList.Count < 1 || chainTimes <= 0) 
        {
            return;
        }
        var target = GetPlayerInDistance(chainList[chainList.Count - 1].self);
        if(target != null)
        {
            chainTimes--;
            AddChain(chainList[chainList.Count - 1].self, target);

            ChainNext();
        }
    }

    public override void OnFinish()
    {
        chainTimes = 3;
        Clear();
        allTeamPlayersList.Clear();
    }

    //不会有乱序问题 直接return了
    BeActor GetPlayerInDistance(BeActor baseActor)
    {
        for (int i = 0; i < allTeamPlayersList.Count; ++i) 
        {
            if (IsInRange(baseActor, allTeamPlayersList[i])) 
            {
                var player = allTeamPlayersList[i];
                if (!player.stateController.CanBeHit() || player.IsDead()) 
                {
                    continue;
                } 
                allTeamPlayersList.Remove(allTeamPlayersList[i]);
                return player;
            }
        }
        return null;
    }

    //private void GetCurrentTargets(List<BeActor> list)
    //{
    //    list.Clear();
    //    for(int i = 0; i < chainList.Count; ++i)
    //    {
    //        list.Add(chainList[i].self);
    //    }
    //}

    void AddDeBuff(ChainUnit unit)
    {
        if(unit.self != null && unit.chainNum == 0)
        {
            unit.self.buffController.TryAddBuff(buffID, buffDuration);
        }
        unit.chainNum++;
    }

    void RemoveDeBuff(ChainUnit unit,bool bForce= false)
    {
        if (unit.self != null) 
        {
            unit.chainNum--;
            if (bForce || unit.chainNum == 0)
            {
                //移除unit.self上的buff
                unit.self.buffController.RemoveBuff(buffID);
                ClearUnitEventHandle(unit);
                chainList.Remove(unit);
            }
        }
    }

    ChainUnit getUnitInChain(BeActor target)
    {
        for(int i = 0; i < chainList.Count; ++i)
        {
            if(chainList[i].self.GetPID() == target.GetPID())
            {
                return chainList[i];
            }
        }

        var targetUnit = new ChainUnit();
        targetUnit.self = target;
        chainList.Add(targetUnit);
        return targetUnit;
    }

    void RemoveUnitInChain(ChainUnit unit, bool removeBuff = true)
    {
        if (unit == null)
        {
            //Logger.LogError("removeUnitChain unit is null");
            return;
        }
        int index = chainList.IndexOf(unit);

        if (index < 0 || index >= chainList.Count)
        {
            //Logger.LogError("removeUnitChain index is invalid");
            return;
        }

        ChainUnit selfActor = chainList[index];
        ChainUnit otherActor1;
        ChainUnit otherActor2;

        if (index == 0)
        {
            otherActor2 = chainList[index + 1];
            //effect 
            ClearChainEffect(selfActor.self);

            //removeBuff
            if (removeBuff)
            {
                RemoveDeBuff(otherActor2);
            }
        }
        else if (index == chainList.Count - 1)
        {
            otherActor1 = chainList[index - 1];
            //effect
            ClearChainEffect(otherActor1.self);

            //removeBuff
            if (removeBuff)
            {
                RemoveDeBuff(otherActor1);
            }
        }
        else if (index != -1) 
        {
            otherActor1 = chainList[index - 1];
            otherActor2 = chainList[index + 1];
            //effect
            ClearChainEffect(selfActor.self);
            ClearChainEffect(otherActor1.self);
            //removeBuff
            if (removeBuff)
            {
                RemoveDeBuff(otherActor2);
                RemoveDeBuff(otherActor1);
            }
        }
        //removeBuff
        if (removeBuff)
        {
            RemoveDeBuff(selfActor, true);
        }
    }

    void ReplaceActorInUnit(ChainUnit unit, BeActor toActor)
    {
        //处理完原来actor
        ClearUnitEventHandle(unit);
        if(unit.self != null)
        {
            unit.self.buffController.RemoveBuff(buffID);
        }
        //处理当前actor
        unit.self = toActor;
        if (unit.self != null)
        {
            unit.self.buffController.TryAddBuff(buffID, buffDuration);
        }
        AddUnitEventHandle(unit);
        if (unit == null)
        {
            //Logger.LogError("removeUnitChain unit is null");
            return;
        }
        int index = chainList.IndexOf(unit);

        if (index < 0 || index >= chainList.Count)
        {
            //Logger.LogError("removeUnitChain index is invalid");
            return;
        }

        ChainUnit selfActor = chainList[index];
        ChainUnit otherActor1;
        ChainUnit otherActor2;

        if (index == 0)
        {
            otherActor2 = chainList[index + 1];
            //effect 
            CreateChainEffect(selfActor.self, otherActor2.self);
        }
        else if (index == chainList.Count - 1)
        {
            otherActor1 = chainList[index - 1];
            //effect
            CreateChainEffect(otherActor1.self,selfActor.self);
        }
        else if (index != -1)
        {
            otherActor1 = chainList[index - 1];
            otherActor2 = chainList[index + 1];
            //effect
            CreateChainEffect(otherActor1.self,selfActor.self);
            CreateChainEffect(selfActor.self, otherActor2.self);
        }
    }

    void ClearUnitEventHandle(ChainUnit unit)
    {
        if(unit != null)
        {
            if (unit.handle != null)
            {
                unit.handle.Remove();
                unit.handle = null;
            }
            if(unit.monsterChangeHandle != null)
            {
                unit.monsterChangeHandle.Remove();
                unit.monsterChangeHandle = null;
            }
            if(unit.monsterRestoreHandle != null)
            {
                unit.monsterRestoreHandle.Remove();
                unit.monsterRestoreHandle = null;
            }
            if(unit.fakeRebornHandle != null)
            {
                unit.fakeRebornHandle.Remove();
                unit.fakeRebornHandle = null;
            }
        }
    }

    ChainUnit OnlyGetUnitByActor(BeEntity entity)
    {
        if(chainList == null)
        {
            return null;
        }
        for (int i = 0; i < chainList.Count; ++i)
        {
            if (chainList[i].self.GetPID() == entity.GetPID())
            {
                return chainList[i];
            }
        }
        return null;
    }

    void AddUnitEventHandle(ChainUnit unit)
    {
        if (unit != null)
        {
            if (unit.handle == null)
            {
                unit.handle = unit.self.RegisterEventNew(BeEventType.onDead, eventParam =>
                {
                    var entity = eventParam.m_Obj as BeEntity;
                    if (entity != null)
                    {
                        var temp = OnlyGetUnitByActor(entity);
                        RemoveUnitInChain(temp);
                    }
                });
            }
            if (unit.monsterChangeHandle == null)
            {
                unit.monsterChangeHandle = unit.self.RegisterEventNew(BeEventType.onMagicGirlMonsterChange, args => 
                {
                    BeActor monster = args.m_Obj as BeActor;
                    BeEntity actor = args.m_Obj2 as BeEntity;
                    var temp = OnlyGetUnitByActor(actor);
                    if (monster != null && temp != null)
                    {
                        RemoveUnitInChain(temp, false);
                        ReplaceActorInUnit(temp, monster);
                    }
                });
            }
            if (unit.monsterRestoreHandle == null)
            {
                unit.monsterRestoreHandle = unit.self.RegisterEventNew(BeEventType.onMagicGirlMonsterRestore, args =>
                {
                    BeActor actor = args.m_Obj as BeActor;
                    BeEntity monster = args.m_Obj2 as BeEntity;
                    var temp = OnlyGetUnitByActor(monster);
                    if (actor != null && temp != null)
                    {
                        RemoveUnitInChain(temp, false);
                        ReplaceActorInUnit(temp, actor);
                    }
                });
            }
            if(unit.fakeRebornHandle == null)
            {
                unit.fakeRebornHandle = unit.self.RegisterEventNew(BeEventType.OnFakeReborn, (args) =>
                {
                    var entity = args.m_Obj as BeEntity;
                    if (entity != null)
                    {
                        var temp = OnlyGetUnitByActor(entity);
                        RemoveUnitInChain(temp);
                    }
                });
            }
        }
    }

    private void Clear()
    {
        List<ChainUnit> temp = new List<ChainUnit>();
        temp.AddRange(chainList);
        for(int i= 0; i < temp.Count; ++i)
        {
            //remove effect
            ClearChainEffect(temp[i].self);

            ClearUnitEventHandle(temp[i]);

            //remove removeDebuff
            if (i != temp.Count - 1) 
            {
                RemoveDeBuff(temp[i + 1]);
            }
            RemoveDeBuff(temp[i]);
        }

        if (owner.CurrentBeBattle != null && owner.CurrentBeBattle.dungeonPlayerManager != null)
        {
            var dungeonPlayers = owner.CurrentBeBattle.dungeonPlayerManager.GetAllPlayers();
            if (dungeonPlayers != null)
            {
                for (int i = 0; i < dungeonPlayers.Count; ++i)
                {
                    if (dungeonPlayers[i].playerActor != null)
                    {
                        dungeonPlayers[i].playerActor.buffController.RemoveBuff(buffID);
                    }
                }
            }
        }

        chainList.Clear();

        ClearNewHandle();
    }

    void ClearNewHandle()
    {
        if (bossDeadHandle != null)
        {
            bossDeadHandle.Remove();
            bossDeadHandle = null;
        }
        if (firstDeadHandle != null)
        {
            firstDeadHandle.Remove();
            firstDeadHandle = null;
        }
    }

    void UpdateActorDistance()
    {
        for (int i = 0; i < chainList.Count - 1; ++i) 
        {
            if (chainList.Count == 0 || i <= -1 || i >= chainList.Count - 1) 
            {
                return;
            }

            if (!IsInRange(chainList[i].self, chainList[i + 1].self))    // 如果大于 chainMaxDistance 执行断开逻辑
            {
                //effect 
                ClearChainEffect(chainList[i].self);

                //removeBuff
                var fromChainUnit = chainList[i];
                var toChainUnit = chainList[i + 1];
                
                RemoveDeBuff(fromChainUnit);
                RemoveDeBuff(toChainUnit);

                if (fromChainUnit.chainNum == 0) 
                {
                    i--;
                }

                if(toChainUnit.chainNum == 0)
                {
                    i--;
                }

                //if (chainList[i].chainNum == 0) 
                //{
                //    chainList.Remove(chainList[i]);
                //    i--;
                //}

                //if (chainList[i + 1].chainNum == 0) 
                //{
                //    chainList.Remove(chainList[i + 1]);
                //    i--;
                //}
            }
        }
    }

    bool IsInRange(BeActor player1,BeActor player2)
    {
        if(player1.GetDistance(player2 ) > chainMaxDistance)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// 创建特效连线
    /// </summary>
    private void CreateChainEffect(BeActor fromActor,BeActor toActor)
    {
#if !LOGIC_SERVER
        if (fromActor == null || toActor == null || fromActor.m_pkGeActor == null)  
            return;
        fromActor.m_pkGeActor.CreateChainEffect(toActor, chainEffect);
#endif
    }

    /// <summary>
    /// 清除特效连线
    /// </summary>
    private void ClearChainEffect(BeActor actor)
    {
#if !LOGIC_SERVER
        if (actor == null || actor.m_pkGeActor == null) 
            return;
        actor.m_pkGeActor.ClearChainEffect();
#endif
    }
}
