using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using GameClient;

public class BeBuffManager
{
    protected BeActor actor;
    protected List<BeBuff> buffList = new List<BeBuff>();
    protected List<BeBuff> phaseDeleteBuffList = new List<BeBuff>();
    protected List<BeBuff> finishDeleteBuffList = new List<BeBuff>();
    protected List<BeBuff> finishDeleteAllBuffList = new List<BeBuff>();

    protected Dictionary<int, List<BuffInfoData>> triggerBuffList = new Dictionary<int, List<BuffInfoData>>();

    //protected List<BeBuff> buffListCache = new List<BeBuff>();

    public BeBuffManager(BeActor owner)
    {
        actor = owner;
    }

 

    public FrameRandomImp FrameRandom
    {
        get
        {
#if !SERVER_LOGIC 
            if(actor.FrameRandom == null)
            {
                return BeSkill.randomForTown;
            }

 #endif

            return actor.FrameRandom;
        }
    }
    
    public List<BeBuff> GetBuffList()
    {
        return buffList;
    }

    public Dictionary<int, List<BuffInfoData>> GetTriggerBuffList()
    {
        return triggerBuffList;
    }

    public bool IsAbnormalBuff(BuffType buffType)
    {
        return buffType >= BuffType.FLASH && buffType <= BuffType.CURSE;
    }

    /// <summary>
    /// 程序手动添加Buff信息禁用这个函数
    /// </summary>
    public BeBuff TryAddBuff(int buffInfoID, BeActor source = null, bool buffEffectAni = false, BeActor releaser = null,int externLevel = 0)
	{
		BuffInfoData buffInfo = new BuffInfoData(buffInfoID, externLevel);

        return TryAddBuff(buffInfo, source, buffEffectAni, new VRate(),releaser);
	}

    /// <summary>
    /// 专用于程序手动添加啊Buff信息 要求传入外部等级
    /// </summary>
    public BeBuff TryAddBuffInfo(int buffInfoID, BeActor releaser, int externLevel)
    {
        BuffInfoData buffInfo = new BuffInfoData(buffInfoID, externLevel);

        return TryAddBuff(buffInfo, null, false, new VRate(), releaser);
    }

    public BeBuff TryAddBuff(BuffInfoData info, BeActor source = null, bool buffEffectAni = false, VRate buffAddRate = new VRate(), BeActor releaser = null)
    {
        //对不同类型的怪有不同的buff
        int buffID = info.buffID;
        if (actor != null && actor.IsMonster() && info.mapMonsterTypeBuff != null && info.mapMonsterTypeBuff.Count > 0)
        {
            int monsterType = actor.GetEntityData().type;
            if (info.mapMonsterTypeBuff.ContainsKey(monsterType) && info.mapMonsterTypeBuff[monsterType] > 0)
            {
                buffID = info.mapMonsterTypeBuff[monsterType];
            }
        }

        //对召唤兽进行特殊处理        BuffInfo表里面用10标识召唤兽
        if (actor != null && info.mapMonsterTypeBuff != null && info.mapMonsterTypeBuff.Count > 0)
        {
            if (actor.GetEntityData().isSummonMonster && info.mapMonsterTypeBuff.ContainsKey(Global.SUMMONMONSTERTYPE) && info.mapMonsterTypeBuff[Global.SUMMONMONSTERTYPE] > 0)
            {
                buffID = info.mapMonsterTypeBuff[Global.SUMMONMONSTERTYPE];
            }
        }

        if (source == null)
            source = actor;

        VRate finalBuffAddRate = (int)info.prob;
        if (buffAddRate > 0)
            finalBuffAddRate = buffAddRate;

        if (actor != null)
        {
            /*int[] buffAddRateArr = new int[1];
            buffAddRateArr[0] = finalBuffAddRate.i;
            actor.TriggerEvent(BeEventType.onChangeBuffAttackRate, new object[] { BuffAttachType.BUFFINFO, (int)info.buffInfoID, buffAddRateArr, source });
            finalBuffAddRate = new VRate(buffAddRateArr[0]);*/
            var ret = actor.TriggerEventNew(BeEventType.onChangeBuffAttackRate, new EventParam{m_Int = (int) BuffAttachType.BUFFINFO, m_Int2 = info.buffInfoID, m_Int3 = finalBuffAddRate.i, m_Obj = source});
            finalBuffAddRate = new VRate(ret.m_Int3);
            
            /*int[] buffLevelArray = new int[1];
            buffLevelArray[0] = 0;
            actor.TriggerEvent(BeEventType.onChangeBuffLevel, new object[] { 0, (int)info.buffInfoID, buffLevelArray, source });
            info.level += buffLevelArray[0];
            if (info.level < 1) info.level = 1;
            info.abnormalLevel += buffLevelArray[0];*/
            var ret2 = actor.TriggerEventNew(BeEventType.onChangeBuffLevel, new EventParam{m_Int = 0, m_Int2 = info.buffInfoID, m_Int3 = 0, m_Obj = source});
            info.level += ret2.m_Int3;
            info.abnormalLevel += ret2.m_Int3;
            
            /*int[] buffAttackArray = new int[1];
            buffAttackArray[0] = GlobalLogic.VALUE_1000;
            actor.TriggerEvent(BeEventType.onChangeBuffAttack, new object[] { 0, (int)info.buffInfoID, buffAttackArray, source });
            info.attack *= VFactor.NewVFactor(buffAttackArray[0], GlobalLogic.VALUE_1000);*/
            
            var ret3 = actor.TriggerEventNew(BeEventType.onChangeBuffAttack, new EventParam{m_Int = 0, m_Int2 = info.buffInfoID, m_Int3 = GlobalLogic.VALUE_1000, m_Obj = source});
            info.attack *= VFactor.NewVFactor(ret3.m_Int3, GlobalLogic.VALUE_1000);
        }

        if (info.level < 1) info.level = 1;
        if (info.abnormalLevel < 1) info.abnormalLevel = 1;

        

        return TryAddBuff(buffID, info.duration, info.level, finalBuffAddRate.i, info.attack, buffEffectAni, info.skillIDs, info.abnormalLevel, info.buffInfoID, releaser);
    }
     public BeBuff TryAddBuff(
        int buffID, 
        int buffDuration, 
        int buffLevel = 1)
    {
        return TryAddBuff(buffID,buffDuration,buffLevel,GlobalLogic.VALUE_1000);
    }

    public BeBuff TryAddBuff(
        int buffID,
        int buffDuration,
        int buffLevel,
        int buffAddRate,
        int buffAttack = 0,
        bool buffEffectAni = false,
        List<int> skillIDs = null,
        int buffAbnormalLevel = 0,
        int buffInfoId = 0,
        BeActor releaser = null)
    {
        if (buffDuration == -1)
            buffDuration = Int32.MaxValue;

        if(null != actor && null != actor.CurrentBeScene)
        {
            var ret = actor.CurrentBeScene.TriggerEventNew(BeEventSceneType.OnChangeBuff, new EventParam(){m_Obj = actor, m_Int = buffID});
            buffID = ret.m_Int;
        }


        var data = TableManager.GetInstance().GetTableItem<ProtoTable.BuffTable>(buffID);
        if (data == null)
            return null;

        //如果是添加新的删除原来的 需要将原来的Buff全部删除
        if(data.Overlay == (int)BuffOverlayType.OVERLAY_ADDNEW)
        {
            RemoveBuff(buffID);
        }      

        if (data.WorkType == (int)BuffWorkType.DEBUFF && actor.stateController.HasBuffState(BeBuffStateType.INVINCIBLE))
            return null;

        if (buffAbnormalLevel == 0)
            buffAbnormalLevel = buffLevel;
        //概率
        ushort randRate = 0;
        if (buffAddRate < VRate.one)
        {
            randRate = FrameRandom.Range1000();
            if (actor != null && actor.IsProcessRecord())
            {
                actor.GetRecordServer().RecordProcess("[BATTLE]pid:{0}-{1} TryAddBuff:{2}", actor.m_iID, actor.GetName(), buffID);
                actor.GetRecordServer().Mark(0x8779792, new int[]
                {
                    actor.m_iID,buffID
                }, actor.GetName());
                // Mark:0x8779792 PID:{0}-{2} TryAddBuff {1}
            }
        }

        //如果有不能添加异常Buff能力36 则不能添加异常Buff
        if (IsAbnormalBuff((BuffType)(data.Type)) && !actor.stateController.CanAddAbnormalBuff())
        {
            return null;
        }

        // 这里判断异常状态的减益Buff
        if (data.WorkType == (int)BuffWorkType.DEBUFF && IsAbnormalBuff((BuffType)(data.Type)))
        {
            if (!actor.stateController.CanBeHit())
                return null;
            if (!actor.stateController.CanAddAbnormalBuffAbility((BuffType)(data.Type))) return null;
            //宠物不加异常buff
            if (actor != null && actor.GetEntityData() != null && actor.GetEntityData().isPet)
                return null;
            /*int[] buffLevelArray = new int[1];
            buffLevelArray[0] = 0;
            actor.TriggerEvent(BeEventType.OnChangeAbnormalBuffLevel, new object[] { buffLevelArray, data.Type });
            buffAbnormalLevel += buffLevelArray[0];*/
            var ret = actor.TriggerEventNew(BeEventType.OnChangeAbnormalBuffLevel, new EventParam{m_Int = 0, m_Int2 = data.Type});
            buffAbnormalLevel += ret.m_Int;
            
            if (actor.stateController.CanAddAbnormalBuffWithBornAbility((BuffType)(data.Type)) && actor.GetEntityData().CanAddAbnormalState(buffAddRate, buffAbnormalLevel, (BuffType)(data.Type)))
            {

            }
            else
            {
                Logger.LogWarningFormat("{0} try add buff id:{1} workType:{2} randRate:{3} buffAddRate:{4}", actor.GetName(), buffID, data.WorkType, randRate, buffAddRate);
                return null;
            }
        }
        else if (/*data.WorkType != (int)BuffWorkType.DEBUFF && */randRate <= (uint)(buffAddRate))
        {
            //can add
        }
        else
        {
            Logger.LogWarningFormat("{0} try add buff id:{1} workType:{2} randRate:{3} buffAddRate:{4}", actor.GetName(), buffID, data.WorkType, randRate, buffAddRate);
            return null;
        }


        BeBuff buff = HasBuffByID(buffID);  //HasBuffByType((BuffType)data.Type);
        bool alreadyHave = buff != null;

        if (data.Overlay == (int)BuffOverlayType.OVERLAY_DELETE && alreadyHave && !buff.CanRemove())
        {
            Logger.LogWarningFormat("{0} buff overlay then remove", actor.GetName());
            buff.Cancel();
            return null;
        }

        if (data.Overlay == (int)BuffOverlayType.OVERLAY_CANT && alreadyHave && !buff.CanRemove())
        {

            Logger.LogWarningFormat("{0} buff {1} can't overlay", actor.GetName(), buff.buffType);
            return null;
        }

        if (data.Overlay == (int)BuffOverlayType.OVERLAY_TIME && alreadyHave && !buff.CanRemove())
        {
            //DisposeByType((BuffType)data.Type);
            buff.RefreshDuration(buffDuration);
            buff.ResetEffectElapsedTime();
            return buff;
        }

        //伤害叠加，时间不叠加
        if (data.Overlay == (int)BuffOverlayType.OVERLAY_DAMAGE && alreadyHave)
        {
            if (data.OverlayLimit > 0 && buff.overlayCount > data.OverlayLimit)
            {
                Logger.LogWarningFormat("{0} exceed overlay limit, buff id:{1}", actor.GetName(), buff.buffID);
                return buff;
            }

            buff.OverlayDamage(buffAttack, buffDuration);
            return buff;
        }

        //将原来的Buff删除 并且添加新的
        if (data.Overlay == (int)BuffOverlayType.OVERLAY_ADDNEW && alreadyHave && !buff.CanRemove())
        {
            buff.Cancel();
        }

        //同时存在个数检查
        int OverlayLimit = data.OverlayLimit;
        if (OverlayLimit > 0 && data.Overlay != (int)BuffOverlayType.OVERLAY_ADDNEW)
        {
            int count = GetBuffCountByID(buffID);
            if (count >= OverlayLimit)
            {
                // QUEUE 队列buf先进先出,超上限了依然可以加入
                if (data.Overlay == (int)BuffOverlayType.OVERLAY_QUEUE)
                {
                    List<BeBuff> curBuffListQueue = GamePool.ListPool<BeBuff>.Get();
                    GetUnDeadBuff(buffID, curBuffListQueue);
                    //List<BeBuff> curBuffListQueue = GetUnDeadBuff(buffID);

                    int countQueue = curBuffListQueue.Count;
                    if (countQueue > 0 && countQueue >= OverlayLimit)
                    {
                        // 交换时间(2->1/ 3->2 / 3最新)
                        if (countQueue > 1)
                        {
                            for (int j = 0; j < countQueue - 1; j++)
                            {
                                curBuffListQueue[j].CopyRunTime(curBuffListQueue[j + 1]);
                            }
                        }
                        BeBuff lastOne = curBuffListQueue[countQueue - 1];
                        lastOne.RefreshDuration(buffDuration);
                        lastOne.ResetEffectElapsedTime();

                        Logger.LogWarningFormat("{0} cancel in queue:{1} buff id{2}", actor.GetName(), count, buffID);
                        GamePool.ListPool<BeBuff>.Release(curBuffListQueue);
                        return null;
                    }

                    GamePool.ListPool<BeBuff>.Release(curBuffListQueue);
                }
                else
                {
                    Logger.LogWarningFormat("{0} execeed exist num:{1} buff id{2}", actor.GetName(), count, buffID);
                    return null;
                }
            }
            else
            {
                Logger.LogWarningFormat("{0} will add buff {1} with count {2}", actor.GetName(), buffID, count + 1);
            }

        }

        // 处理特效叠加1，
        BeBuff lastTimeBufForHideEffect = null;
        if (data.EffectDisOverlay > 0)
        {
            lastTimeBufForHideEffect = GetLastTimeAddBuff(buffID);
        }

        buff = AddBuff(buffID, buffLevel, buffDuration, buffAttack, buffEffectAni, skillIDs, buffAbnormalLevel,releaser);

        // 处理特效叠,只有buff添加成功才处理: 隐藏最后加的那个
        if (lastTimeBufForHideEffect != null && buff != null)
            lastTimeBufForHideEffect.HideEffect();

        RefreshAbnormalData(buff);
        if (buff != null)
        {
            if (releaser != null && data.IsDelete==1)
            {
                BeEvent.BeEventHandleNew _handle = null;
                _handle = releaser.RegisterEventNew(BeEventType.onDead, eventParam =>
                {
                    if (actor != null)
                    {
                        actor.buffController.RemoveBuff(buffID);
                    }
                    if (_handle != null)
                    {
                        _handle.Remove();
                    }
                });
            }
            buff.buffInfoId = buffInfoId;
            //异常Buff保护
            if (actor != null && actor.protectManager != null)
                actor.protectManager.AddBuff(buff.buffType);

            if (null != actor && data.BuffSpeech > 0)
            {
                actor.ShowSpeechWithID(data.BuffSpeech, false);
            }
        }
        return buff;
    } 

    private BeBuff AddBuff(int buffID, int buffLevel, int buffDuration, int buffAttack = 0, bool buffEffectAni = false, List<int> skillIDs = null, int abnormalLevel=0, BeActor releaser = null)
    {
        if (!actor.stateController.CanAddBuff())
        {
            return null;
        }

        //被抓取状态下 不能上 眩晕 冰冻 石化 睡眠四种异常Buff
        if (actor.sgGetCurrentState() == (int)ActionState.AS_GRABBED)
        {
            if (buffID == 3 || buffID == 8 || buffID == 4 || buffID == 7)
            {
                return null;
            }
        }

        //如果自己正在抓取并且是异常Buff 则不能上Buff
        ProtoTable.BuffTable buffData = TableManager.GetInstance().GetTableItem<ProtoTable.BuffTable>(buffID);
        if (buffData != null)
        {
            if (actor.stateController.IsGrabbing() && IsAbnormalBuff((BuffType)buffData.Type))
            {
                return null;
            }
        }

        //死亡状态下不能上异常buff
        if(actor.IsDead()&&IsAbnormalBuff((BuffType)buffData.Type))
        {
            return null;
        }

        /*int[] array = new int[] { 0 };
        actor.TriggerEvent(BeEventType.BuffCanAdd, new object[] { array, buffID });
        if (array[0] == 1)
        {
            return null;
        }*/
        
        var ret = actor.TriggerEventNew(BeEventType.BuffCanAdd, new EventParam(){m_Int = 0, m_Int2 = buffID, m_Int3 = buffLevel});
        if (ret.m_Int == 1)
        {
            return null;
        }

        buffLevel = ret.m_Int3;
        abnormalLevel = ret.m_Int3;

        BeBuff buff = null;

        //对独立覆盖类型的非感电Buff进行特殊处理（因为感电Buff进行了特写）
        bool isAbnormal = (buffData.Type != (int)BuffType.FLASH && buffData.Overlay == (int)BuffOverlayType.OVERLAY_ALONE);
        int buffPID = 0;
       
       //town里不用bebuff pool，另外验证服务器pool也是开的，因为pool隶属于basebattle
        bool isTown = actor.CurrentBeBattle == null;

        if (isTown)
        {
            buff = BeBuffFactory.CreateBuff(buffID, buffLevel, buffDuration, buffAttack, buffEffectAni, isAbnormal);
        }                 
        else
        {
            buffPID = actor.CurrentBeBattle.BuffPool.GenID();
            buff = actor.CurrentBeBattle.BuffPool.GetBuff(buffID, buffLevel, buffDuration, buffAttack, buffEffectAni, isAbnormal);
        }

        buff.PID = buffPID;
        buff.owner = actor;
        buff.SetBuffReleaser(releaser);
        buff.skillIDs = skillIDs;
        buff.abnormalLevel = abnormalLevel;
        actor.TriggerEventNew(BeEventType.onBuffBeforePostInit, new EventParam{ m_Obj = buff });
        buff.PostInit();

        if (!buff.CanAdd(actor))
        {
            if (!isTown) 
                actor.CurrentBeBattle.BuffPool.PutBuff(buff);

            return null;
        }
        
        buffList.Add(buff);

        buff.Start();
        
        //actor.TriggerEvent(BeEventType.onAddBuff, new object[] { buff });
        actor.TriggerEventNew(BeEventType.onAddBuff, new EventParam(){m_Obj = buff});
        if(actor.CurrentBeScene != null)
            actor.CurrentBeScene.TriggerEventNew(BeEventSceneType.OnAddBuff, new EventParam(){m_Obj = buff, m_Obj2 = actor});

        if (actor != null && actor.IsProcessRecord())
        {
            if (actor != null)
            {
                actor.GetRecordServer().RecordProcess("PID:{0}-{1} ADD BUFF {2} {3}", actor.m_iID, actor.GetName(), buff.buffID, actor.GetInfo());
                actor.GetRecordServer().Mark(0x8779793, new int[]
               {
                   actor.m_iID, buff.buffID, actor.GetPosition().x, actor.GetPosition().y,
                   actor.GetPosition().z, actor.moveXSpeed.i, actor.moveYSpeed.i, actor.moveZSpeed.i,
                    actor.GetFace() ? 1: 0, actor.attribute.GetHP(),  actor.attribute.GetMP(),actor.GetAllStatTag(),
                    actor.attribute.battleData.attack
               }, actor.GetName());
                // Mark:0x8779793 PID:{0}-{13} AddBuff {1} Pos:({2},{3},{4}),Speed:({5},{6},{7}),Face:{8},Hp:{9},Mp:{10},Flag:{11},attack:{12}
            }
        }

        Logger.LogWarningFormat("{0} addbuff id:{1}, level:{2} duration:{3} name:{4}", actor.GetName(), buffID, buffLevel, buffDuration, buff.buffData.Name);
        return buff;
    }

    public void AddBuffInfoByID(int buffInfoID, int level = 0, bool isTown = false)
    {
        BuffInfoData buffInfo = new BuffInfoData(buffInfoID, level);
        if (buffInfo != null)
        {
            switch (buffInfo.condition)
            {
                case BuffCondition.NONE:
                    {
                        // 直接触发
                        TriggerBuffInfo(buffInfo);
                    }
                    break;
                case BuffCondition.ENTERBATTLE:
                    {
                        // 进入战斗触发
                        // 在战斗中直接触发
                        // 在城镇中不触发
                        if (!isTown)
                        {
                            TriggerBuffInfo(buffInfo);
                        }
                    }
                    break;
                default:
                    {
                        // 加入等待触发的列表
                        AddTriggerBuff(buffInfo);
                    }
                    break;
            }
        }
    }

    public BeBuff AddBuffForSkill(int id, int level, int duration, List<int> skillIDs)
    {
        if (duration == -1)
            duration = Int32.MaxValue;

        if(null != actor && null != actor.CurrentBeScene)
        {
            var ret = actor.CurrentBeScene.TriggerEventNew(BeEventSceneType.OnChangeBuff, new EventParam(){m_Obj = actor, m_Int = id});
            id = ret.m_Int;
        }

        BeBuff buff = AddBuff(id, level, duration, 0, false, skillIDs);

        Logger.LogWarningFormat("{0} addbuff id:{1}, level:{2} duration:{3} name:{4}", actor.GetName(), id, level, duration, buff.buffData.Name);

        return buff;
    }
    public BeBuff HasBuffByID(int bid)
    {
        if (bid > 0)
        {
            for (int i = 0; i < buffList.Count; ++i)
            {
                var curBuff = buffList[i];
                if (curBuff != null && !curBuff.CanRemove() && curBuff.buffID == bid)
                    return curBuff;

            }
        }

        return null;
    }

    //驱散buff
    public void DispelBuff(BuffWorkType buffWorkType, int num = Int32.MaxValue)
    {
        for (int i = 0; i < buffList.Count && i < num; ++i)
        {
            var buff = buffList[i];
            if (buff != null && !buff.CanRemove() && buff.CanDispel() && buff.buffWorkType == buffWorkType)
            {
                buff.Cancel();
            }
        }
    }

    public void DisposeByID(int bid)
    {
        if (bid > 0)
        {
            for (int i = 0; i < buffList.Count; ++i)
            {
                if (buffList[i] != null && !buffList[i].CanRemove() && buffList[i].buffID == bid)
                    buffList[i].Cancel();
            }
        }
    }

    public BeBuff HasBuffByType(BuffType bt)
    {
        //foreach(var buff in buffList)

        for (int i = 0; i < buffList.Count; ++i)
        {
            var buff = buffList[i];
            if (!buff.CanRemove() && buff.buffType == bt)
                return buff;
        }

        return null;
    }

    public void DisposeByType(BuffType bt)
    {
        //foreach(var buff in buffList)
        for (int i = 0; i < buffList.Count; ++i)
        {
            var buff = buffList[i];
            if (buff.buffType == bt)
            {
                buff.Cancel();
            }
        }
    }

    private bool CheckCanRemoveBuff(BeBuff buff)
    {
        return buff.CanRemove();
    }

    public void UpdateBuff(int deltaTime)
    {
        bool bDirty = false;
        for (int i = 0; i < buffList.Count; ++i)
        {
            BeBuff buff = buffList[i];
            if (CheckCanRemoveBuff(buff))
            {
                bDirty = true;
            }

            buffList[i].Update(deltaTime);
        }

        if (bDirty)
            _RemoveBuff();


        UpdateRangeTriggerBuff(deltaTime);
        UpdateBuffInfo(deltaTime);
    }

    void _RemoveBuff()
    {
        //buffList.RemoveAll(CheckCanRemoveBuff);

        var tmpList = GamePool.ListPool<BeBuff>.Get();

        foreach (var buff in buffList)
        {
            if (buff.CanRemove())
            {
                tmpList.Add(buff);
            }
        }

        buffList.RemoveAll(CheckCanRemoveBuff);
        if(actor.CurrentBeBattle != null)
        {
            foreach (var buff in tmpList)
            {
                actor.CurrentBeBattle.BuffPool.PutBuff(buff);
            }
        }

        _RemoveFromList(phaseDeleteBuffList, tmpList);
        _RemoveFromList(finishDeleteBuffList, tmpList);
        _RemoveFromList(finishDeleteAllBuffList, tmpList);

        GamePool.ListPool<BeBuff>.Release(tmpList);
    }

    void _RemoveFromList(List<BeBuff> list, List<BeBuff> alreadyDead)
    {
        if (list == null || alreadyDead == null)
        {
            return;
        }

        list.RemoveAll(item => { return alreadyDead.Contains(item); });
    }

    public void RemoveBuff(BeBuff buff)
    {
        for (int i = 0; i < buffList.Count; ++i)
        {
            if (buffList[i] == buff && !buffList[i].CanRemove())
            {
                buffList[i].Cancel();
            }
        }
        //buffList.RemoveAll(CheckCanRemoveBuff);

        _RemoveBuff();
    }

    public void RemoveBuff(int buffID, int num = 0, int buffInfoId = 0)
    {
        BeBuff buff = HasBuffByID(buffID);
        if (buff == null)
            return;

        int count = 0;
        for (int i = 0; i < buffList.Count; ++i)
        {
            if (buffList[i].buffID == buffID && !buffList[i].CanRemove())
            {
				//精确查找到BuffInfo添加的那个Buff
                if (buffInfoId != 0 && buffList[i].buffInfoId != buffInfoId)
                    continue;
                buffList[i].Cancel();
                if (num > 0)
                {
                    count++;
                    if (count >= num)
                        break;
                }
            }
        }

        //buffList.RemoveAll(CheckCanRemoveBuff);
        _RemoveBuff();
    }

    public void RemoveBuffByBuffInfoID(int buffInfoID)
    {
        if (buffInfoID <= 0)
            return;
        BuffInfoData buffInfo = new BuffInfoData(buffInfoID);
        if (buffInfo.condition <= BuffCondition.NONE)
        {
            if (buffInfo.condition == BuffCondition.ENTERBATTLE || buffInfo.condition == BuffCondition.NONE)
                RemoveBuff(buffInfo.buffID, 1);
        }
        else
        {
            RemoveTriggerBuff(buffInfoID);
        }
    }

    public void RemoveBuffByPID(int buffPID)
    {
        for (int i = buffList.Count-1; i >= 0; --i)
        {
            var buff = buffList[i];
            if (!buff.CanRemove() && buff.PID == buffPID)
            {
                buff.Cancel();
            }
        }
    }

    public BeBuff GetBuffByPID(int buffPID)
    {
        for (int i = buffList.Count-1; i >= 0; --i)
        {
            var buff = buffList[i];

            if (!buff.CanRemove() && buff.PID == buffPID)
            {
                return buff;
            }
        }
        return null;
    }

    public void AddPhaseDelete(BeBuff buff)
    {
        phaseDeleteBuffList.Add(buff);
    }

    public void ClearPhaseDelete()
    {
        for (int i = 0; i < phaseDeleteBuffList.Count; ++i)
        {
            var buff = phaseDeleteBuffList[i];
            if (buff != null && !buff.CanRemove())
                buff.Cancel();
        }

        phaseDeleteBuffList.Clear();
    }

    public void AddFinishDelete(BeBuff buff)
    {
        finishDeleteBuffList.Add(buff);
    }

    public void ClearFinishDelete()
    {
        for (int i = 0; i < finishDeleteBuffList.Count; ++i)
        {
            var buff = finishDeleteBuffList[i];
            if (buff != null && !buff.CanRemove() && buff.buffID == 1)
                buff.Cancel();
        }

        finishDeleteBuffList.Clear();
    }

    public void AddFinishDeleteAll(BeBuff buff)
    {
        finishDeleteAllBuffList.Add(buff);
    }

    public void ClearFinishDeleteAll()
    {
        for (int i = 0; i < finishDeleteAllBuffList.Count; ++i)
        {
            var buff = finishDeleteAllBuffList[i];
            if (buff != null && !buff.CanRemove())
                buff.Cancel();
        }

        finishDeleteAllBuffList.Clear();
    }

    public int GetBuffCountByType(BuffType buffType)
    {
        int count = 0;

        for (int i = 0; i < buffList.Count; ++i)
        {
            if (!buffList[i].CanRemove() && buffList[i].buffType == buffType)
                count++;
        }

        return count;
    }

    public int GetBuffCountByID(int buffID)
    {
        int count = 0;

        for (int i = 0; i < buffList.Count; ++i)
        {
            if (!buffList[i].CanRemove() && buffList[i].buffID == buffID)
                count++;
        }

        return count;
    }

    public void GetUnDeadBuff(int buffID, List<BeBuff> buffListCache)
    {
        if (buffListCache == null)
            return;
        
        buffListCache.Clear();
        for (int i = 0; i < buffList.Count; ++i)
        {
            var curBuff = buffList[i];
            if (curBuff.buffID == buffID && !curBuff.CanRemove())
                buffListCache.Add(buffList[i]);
        }
    }

    public BeBuff GetLastTimeAddBuff(int buffID)
    {
        //buffListCache.Clear();
        BeBuff curBuff = null;
        for (int i = buffList.Count - 1; i > -1; --i)
        {
            curBuff = buffList[i];
            if (curBuff.buffID == buffID && !curBuff.CanRemove())
                return curBuff;
        }

        return null;
    }

    public BeBuff GetBuffByType(BuffType buffType)
    {

        for (int i = 0; i < buffList.Count; ++i)
        {
            if (!buffList[i].CanRemove() && buffList[i].buffType == buffType)
                return buffList[i];
        }

        return null;
    }

    public void RemoveInPassiveBuff()
    {
        for (int i = 0; i < buffList.Count; ++i)
        {
            var buff = buffList[i];
            if (buff != null && !buff.CanRemove() && !buff.IsPassive() && buff.duration < Int32.MaxValue)
            {
                //Logger.LogErrorFormat("RemoveInPassiveBuff:{0}", buff.buffID);
                buff.Cancel();
            }
        }
    }

    public void RemoveDebuff()
    {
        for (int i = 0; i < buffList.Count; ++i)
        {
            var buff = buffList[i];
            if (buff != null && !buff.CanRemove() && !buff.IsPassive() && buff.duration < Int32.MaxValue && buff.buffWorkType == BuffWorkType.DEBUFF)
                buff.Cancel();
        }
    }
    //该接口请慎重斟酌使用（递归的风险）
    public void RemoveAllBuff(bool forceRemove=false)
    {
        for (int i = 0; i < buffList.Count; ++i)
        {
            var buff = buffList[i];
            if (buff != null && !buff.CanRemove())
                buff.Cancel();
        }

        if (forceRemove)
            _RemoveBuff();
    }

    public bool AddTriggerBuff(int buffInfoID,int level = 0)
    {
        if (buffInfoID <= 0)
            return false;

        BuffInfoData buffInfo = new BuffInfoData(buffInfoID, level);
        return AddTriggerBuff(buffInfo);
    }


    public bool AddTriggerBuff(BuffInfoData buffInfo)
    {
        if (buffInfo == null)
            return false;

        if (buffInfo.condition <= BuffCondition.NONE)
        {
            Logger.LogWarningFormat("buffInfo:{0} has condition:{1}", buffInfo.buffInfoID, buffInfo.condition);
            return false;
        }

        int key = (int)buffInfo.condition;

        if (!triggerBuffList.ContainsKey(key))
        {
            List<BuffInfoData> buffs = new List<BuffInfoData>();
            triggerBuffList.Add(key, buffs);
        }

        triggerBuffList[key].Add(buffInfo);
        buffInfo.OnAdd(actor);

        return true;
    }

    public void RemoveTriggerBuff(int buffInfoID)
    {
        if (buffInfoID <= 0)
            return;

        BuffInfoData buffInfo = new BuffInfoData(buffInfoID);
        if (buffInfo.condition <= BuffCondition.NONE)
            return;

        int key = (int)buffInfo.condition;
        if (!triggerBuffList.ContainsKey(key))
            return;

        List<BuffInfoData> buffInfos = triggerBuffList[key];
        buffInfos.RemoveAll(x =>
        {
            if (x.buffInfoID == buffInfoID)
            {
                x.DoRelease();
                x.OnRemove(actor);
                return true;
            }
            return false;
        });
    }

    public BuffInfoData GetTriggerBuff(BuffInfoData buffInfo)
    {
        if (buffInfo == null)
            return null;

        int key = (int)buffInfo.condition;
        if (!triggerBuffList.ContainsKey(key))
            return null;

        var buffInfos = triggerBuffList[key];
        for (int i=0; i< buffInfos.Count; ++i)
        {
            if (buffInfos[i].buffInfoID == buffInfo.buffInfoID)
                return buffInfos[i];
        }

        return null;
    }


    /*
	 * ATTACK - target 为攻击对象
	 * BEHIT - target 为传人的攻击者
	 * RELEASE_SKILL - target 为null
	 * RANGE - 无
	*/
    public void TriggerBuffs(BuffCondition condition, BeActor target = null, object var1 = null)
    {
        List<BuffInfoData> buffs = null;
        if (triggerBuffList.ContainsKey((int)condition))
            buffs = triggerBuffList[(int)condition];
        if (buffs == null)
            return;

        Logger.LogWarningFormat("name:{0} triggers buff at condition:{1}", actor.GetName(), condition);

        for (int i = 0; i < buffs.Count; ++i)
        {
            var buffInfo = buffs[i];

            TriggerBuffInfo(buffInfo, target, var1);
        }
    }
    //object[] addTriggerBuffParam = new object[] { null };
    public void TriggerBuffInfo(BuffInfoData buffInfo, BeActor target = null, object var1 = null)
    {
        if (buffInfo == null)
            return;

        if (buffInfo.condition == BuffCondition.RELEASE_SEPCIFY_SKILL ||
            buffInfo.condition == BuffCondition.RELEASE_SEPCIFY_SKILL_HIT
            || buffInfo.condition == BuffCondition.RELEASE_SEPCIFY_SKILL_COMPLETE)
        {
            int skillID = 0;
            if (var1 != null)
                skillID = (int)var1;
            if (!buffInfo.ContainSkillID(skillID))
                return;
            if (actor != null && actor.attribute != null &&
                actor.attribute.monsterData != null &&
                actor.attribute.monsterData.MonsterMode != buffInfo.monsterMode)
            {
                return;
            }
        }

        /*int[] radiusArray = new int[1];
        radiusArray[0] = GlobalLogic.VALUE_1000;
        actor.TriggerEvent(BeEventType.onChangeBuffTargetRadius, new object[] { (int)buffInfo.buffInfoID, radiusArray });
        buffInfo.buffTargetRangeRadius *= VFactor.NewVFactor(radiusArray[0], GlobalLogic.VALUE_1000);*/
        
        var eventData = actor.TriggerEventNew(BeEventType.onChangeBuffTargetRadius, new EventParam(){m_Int = buffInfo.buffInfoID, m_Int2 = GlobalLogic.VALUE_1000});
        buffInfo.buffTargetRangeRadius *= VFactor.NewVFactor(eventData.m_Int2, GlobalLogic.VALUE_1000);

        bool isNeedAddByMonsterMode = true;
        if (target != null && target.attribute != null &&
               target.attribute.monsterData != null)
        {
            if (buffInfo.monsterMode != 0 && target.attribute.monsterData.MonsterMode != buffInfo.monsterMode)
            {
                isNeedAddByMonsterMode = false;
            }
        }

        List<BeActor> targets = GamePool.ListPool<BeActor>.Get();

        if (buffInfo.target == BuffTarget.RANGE_ENEMY)
        {
            actor.CurrentBeScene.FindTargets(targets, actor, VInt.NewVInt(buffInfo.buffTargetRangeRadius,(long)GlobalLogic.VALUE_1000));
        }
        else if (buffInfo.target == BuffTarget.RANGE_FRIEND)
        {
            actor.CurrentBeScene.FindTargets(targets, actor, VInt.NewVInt(buffInfo.buffTargetRangeRadius,(long)GlobalLogic.VALUE_1000), true);
        }
        else if(buffInfo.target == BuffTarget.RANGE_FRIEND_ADNSELF)
        {
            actor.CurrentBeScene.FindTargets(targets, actor, VInt.NewVInt(buffInfo.buffTargetRangeRadius, (long)GlobalLogic.VALUE_1000), true);
        } 
        else if (buffInfo.target == BuffTarget.ENEMY)
        {
            if (target != null && target.GetCamp() != actor.GetCamp() && isNeedAddByMonsterMode)
                targets.Add(target);
        }
        else if (buffInfo.target == BuffTarget.SELF || buffInfo.target == BuffTarget.SKILL)
        {
            if (buffInfo.target == BuffTarget.SKILL || isNeedAddByMonsterMode)
                targets.Add(actor);
        }
        //查找友方玩家
        else if (buffInfo.target == BuffTarget.RANGE_FRIENDHERO)
        {
            BeUtility.GetAllFriendPlayers(actor, targets);
        }
        else if (buffInfo.target == BuffTarget.RANGE_ENEMYHERO)
        {
            BeUtility.GetAllEnemyPlayers(actor, targets);
        }
        else if (buffInfo.target == BuffTarget.RANGE_FRIEND_NOTSUMMON)
        {
            BeGetRangeFriendNotSummon filter = new BeGetRangeFriendNotSummon();
            actor.CurrentBeScene.FindTargets(targets, actor, VInt.NewVInt(buffInfo.buffTargetRangeRadius, (long)GlobalLogic.VALUE_1000), true, filter);
        }
        else if(buffInfo.target == BuffTarget.OUT_OF_RANGE_ENEMY)
        {
            BeGetConcentricCircleTarget filter = new BeGetConcentricCircleTarget();
            filter.m_Owner = actor;
            filter.m_OwnerPosXY = new VInt2(actor.GetPosition().x, actor.GetPosition().y);
            filter.m_MinCircleRadius = VInt.NewVInt(buffInfo.buffTargetRangeRadius, GlobalLogic.VALUE_1000);
            filter.m_MaxCircleRadius = BeGetConcentricCircleTarget.LargeMaxCirleRadius;
            actor.CurrentBeScene.GetFilterTarget(targets, filter);
        }
        else if (buffInfo.target == BuffTarget.RANGE_OWNER)
        {
            var topOwner = actor.GetTopOwner(actor) as BeActor;
            if (topOwner != null && topOwner.GetPID() != actor.GetPID())
            {
                if(topOwner.GetDistance(actor) < VInt.NewVInt(buffInfo.buffTargetRangeRadius, GlobalLogic.VALUE_1000))
                    targets.Add(topOwner);
            }
        }
        //CD中不再触发
        if (buffInfo.IsCD())
        {
            Logger.LogProcessFormat("buffinfo:{0} not triggerd with CD!!!!", buffInfo.buffInfoID);
            //return;
        }
        else if (targets != null)
        {
            VRate buffAddRate = VRate.zero;
            if (buffInfo.target == BuffTarget.RANGE_ENEMY || buffInfo.target == BuffTarget.RANGE_FRIEND || buffInfo.target == BuffTarget.RANGE_FRIEND_ADNSELF 
                || buffInfo.target == BuffTarget.RANGE_FRIENDHERO || buffInfo.target == BuffTarget.RANGE_ENEMYHERO || buffInfo.target == BuffTarget.RANGE_FRIEND_NOTSUMMON
                || buffInfo.target == BuffTarget.OUT_OF_RANGE_ENEMY|| buffInfo.target == BuffTarget.RANGE_OWNER) 
            {
                if (buffInfo.prob >= GlobalLogic.VALUE_1000)
                    buffAddRate = VRate.one;
                else
                {
                    int randRate = FrameRandom.Range1000();
                    if (randRate <= buffInfo.prob)
                        buffAddRate = VRate.one;
                    else
                    {
                        GamePool.ListPool<BeActor>.Release(targets);
                        return;
                    }  
                }
            }

            //bool triggered = false;
            bool triggeredSucceed = false;//判定是否添加成功标志位
            Logger.LogProcessFormat("buffinfo:{0} trigered:{1}", buffInfo.buffInfoID, buffInfo.CD);
            for (int j = 0; j < targets.Count; ++j)
            {
                if (targets[j] != null)
                {
                    //triggered = true;

                    var realTarget = targets[j];
                    if (buffInfo.delay > 0)
                    {
                        realTarget.delayCaller.DelayCall(buffInfo.delay, () =>
                        {
                            RealAddBuff(realTarget, buffInfo, buffAddRate);
                        });
                    }
                    else
                        triggeredSucceed |= RealAddBuff(realTarget, buffInfo, buffAddRate);//策划需求 只要有一个buffInfo添加成功则判定为buffInfo触发成功
                }
            }
            if (triggeredSucceed)
            {
                //addTriggerBuffParam[0] = buffInfo;
                //actor.TriggerEvent(BeEventType.onAddTriggerBuff, addTriggerBuffParam);
                
                actor.TriggerEventNew(BeEventType.onAddTriggerBuff, new EventParam(){m_Obj = buffInfo});
            }

            //if (triggered)
            //	buffInfo.StartCD();
        }

        GamePool.ListPool<BeActor>.Release(targets);
    }

    bool RealAddBuff(BeActor realTarget, BuffInfoData buffInfo, VRate buffAddRate)
    {
        bool triggerSucceed = false;
        //特殊,宠物放技能
        if (buffInfo.buffID == -1)
        {
            realTarget.TriggerEventNew(BeEventType.onPetSkill);
            return triggerSucceed;
        }


        BeBuff buff = realTarget.buffController.TryAddBuff(buffInfo, actor, false, buffAddRate,actor);
        if (buff != null)
        {
            triggerSucceed = true;
            if (!buffInfo.IsCD())
                buffInfo.StartCD();
        }
        return triggerSucceed;
    }

    public void UpdateRangeTriggerBuff(int delta)
    {
        if (!triggerBuffList.ContainsKey((int)BuffCondition.RANGE))
            return;

        List<BuffInfoData> buffs = triggerBuffList[(int)BuffCondition.RANGE];
        if (buffs == null)
            return;

        for (int i = 0; i < buffs.Count; ++i)
        {
            buffs[i].UpdateCheckRange(delta, actor);
        }
    }

    public void RemoveRangerTriggerBuff()
    {
        if (!triggerBuffList.ContainsKey((int)BuffCondition.RANGE))
            return;

        List<BuffInfoData> buffs = triggerBuffList[(int)BuffCondition.RANGE];
        if (buffs == null)
            return;

        for (int i = 0; i < buffs.Count; ++i)
        {
            buffs[i].DoRelease();
        }
    }

    public void UpdateBuffInfo(int delta)
    {
        for (int i = (int)BuffCondition.ATTACK; i < (int)BuffCondition.COUNT; ++i)
        {
            if (!triggerBuffList.ContainsKey(i))
                continue;

            var buffs = triggerBuffList[i];
            if (buffs == null)
                continue;

            for (int j = 0; j < buffs.Count; ++j)
            {
                buffs[j].Update(delta);
            }
        }
    }

    //判断角色有没有霸体Buff
    public bool HaveBatiBuff()
    {

        for (int i = 0; i < buffList.Count; i++)
        {
            BeBuff buff = buffList[i];
            if (buff == null)
                continue;
            if(buff.CanRemove())
            {
                continue;
            }
            if (buff.buffType == BuffType.BATI)
                return true;
        }
        return false;
    }

    //判断角色有没有忽略顿帧的霸体
    public bool HaveBatiNoPauseBuff()
    {
        for (int i = 0; i < buffList.Count; i++)
        {
            BeBuff buff = buffList[i];
            if (buff == null)
                continue;
            if (buff.CanRemove())
            {
                continue;
            }
            if (buff.buffType == BuffType.BATI_NO_PAUSE)
                return true;
        }
        return false;
    }

    //移除所有的异常Buff
    public void RemoveAllAbnormalBuff()
    {
        //倒序删除
        bool needRemove = false;
        for (int i = buffList.Count -1;i >=0; i--)
        {
            BeBuff buff = buffList[i];
            if (buff == null || buff.CanRemove())
                continue;
            if (IsAbnormalBuff(buff.buffType))
            {
                buff.Cancel();
                needRemove = true;
            }
        }
        if(needRemove)
        {
            //buffList.RemoveAll(CheckCanRemoveBuff);
            _RemoveBuff();
        }
    }

    #region 异常伤害Buff新的伤害机制相关

    //刷新独立覆盖类型异常Buff数据 
    //在添加Buff的地方处理，本来可以在BuffManager里面处理，但是由于TryAddff时候Buff.Releaser没有赋值，所以改为在复制的地方处理
    public void RefreshAbnormalData(BeBuff buff)
    {
        if (buff == null || buff.buffData.Overlay != (int)BuffOverlayType.OVERLAY_ALONE)
            return;
        //独立叠加 用于异常buff伤害叠加
        BeBuff firstAbnormalBuff = GetFirstAbnormalBuff(buff.buffID);
        //如果之前存在
        if (firstAbnormalBuff==null)
            buff.abnormalBuffData.isFirst = true;
    }

    //获取相同ID用于造成伤害的异常Buff
    public BeBuff GetFirstAbnormalBuff(int buffId)
    {
        if (buffId > 0)
        {
            for (int i = 0; i < buffList.Count; ++i)
            {
                if (buffList[i] != null && buffList[i].buffID == buffId && buffList[i].abnormalBuffData.isFirst)
                    return buffList[i];
            }
        }
        return null;
    }

    //获取一个和自己相同ID的Buff
    public BeBuff GetBuffButSelf(BeBuff buff,int buffId)
    {
        for (int i = 0; i < buffList.Count; ++i)
        {
            if (buffList[i] != null && buffList[i].buffID == buffId && buffList[i] != buff)
                return buffList[i];
        }
        return null;
    }

    //获取异常Buff伤害的数值
    public int GetAbnormalDamage(int buffId)
    {
        int damage = 0;
        for (int i = 0; i < buffList.Count; i++)
        {
            if (buffList[i] == null || buffList[i].buffID != buffId)
                continue;
            damage += buffList[i].GetAloneAbnormalDamage();
        }
        return damage;
    }
    #endregion

    /// <summary>
    /// 玩家复活的时候通知Buff
    /// </summary>
    public void RefreshBuffStateOnReborn()
    {
        for (int i = 0; i < buffList.Count; i++)
        {
            var buff = buffList[i];
            buff.OnOwnerReborn();
        }
    }
}
