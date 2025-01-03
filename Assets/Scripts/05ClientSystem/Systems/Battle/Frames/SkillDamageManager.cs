using System.Collections;
using System.Collections.Generic;

public struct DamageData
{
    public int skillId;    //技能ID
    public long damage;     //技能伤害
}

public struct MonsterExistTime
{
    public int monsterId;   //怪物ID
    public int time;
    public bool isDead;
}

public struct SkillDamageData
{
    public int totalTime;
    public string dungeonName;  //地下城名称
    public List<int> monsterIdList; //怪物ID列表
    public List<string> monsterNameList;    //怪物名称列表
    public List<int> origionSkillIdList;    //记录初始技能ID列表
    public List<int> origionSkillIdUseCount;    //记录初始技能ID使用次数
    public Dictionary<int, List<DamageData>> skillDamageDic; //记录所有的伤害      //此处的ID为怪物ID
    public List<MonsterExistTime> monsterExistTimeList; //记录怪物的存在时间
}

//角色 副本技能造成的伤害统计
public class SkillDamageManager
{
    private BeActor owner;
    private BeScene scene;

    private Dictionary<int, List<int>> skillIdDic = null;   //主角技能ID对应的所有技能ID list对应召唤出来的怪物技能列表
    public SkillDamageData skillDamageData;

    private List<int> recordMonsterIdList = new List<int>();  //记录当前存活的怪物ID 用于秒伤计算

    private bool timingFlag = false;    //计时标志

    #region EEventHandle
    GameClient.BeEvent.BeEventHandleNew onHurtHandle = null;
    IBeEventHandle onSummonHandle = null;
    IBeEventHandle onHurtByAbnormalHandle = null;
    IBeEventHandle onCastSkillHandle = null;
    IBeEventHandle onAreaClearHandle = null;
    IBeEventHandle onSceneMonsterDead = null;

    public SkillDamageManager(BeActor actor)
    {
#if !LOGIC_SERVER
        owner = actor;
#endif
    }

    public void InitData(BeScene sce)
    {
#if !LOGIC_SERVER
        skillIdDic = new Dictionary<int, List<int>>();
        skillDamageData = new SkillDamageData();

        scene = sce;
        RemoveHandle();
        InitHandle();
        InitSkillData();
#endif
    }

    public void Update(int deltaTime)
    {
#if !LOGIC_SERVER
        UpdateTotalTime(deltaTime);
        UpdateMonsterTime(deltaTime);
#endif
    }

    /// <summary>
    /// 刷新总时间
    /// </summary>
    /// <param name="deltaTime"></param>
    private void UpdateTotalTime(int deltaTime)
    {
        if (!timingFlag)
            return;
        if (skillDamageData.totalTime >= int.MaxValue)
            return;
        skillDamageData.totalTime += deltaTime;
    }

    /// <summary>
    /// 刷新怪物存活时间
    /// </summary>
    private void UpdateMonsterTime(int deltaTime)
    {
        if (!timingFlag)
            return;
        if (skillDamageData.monsterExistTimeList == null)
            return;
        for(int i=0;i< skillDamageData.monsterExistTimeList.Count; i++)
        {
            var data = skillDamageData.monsterExistTimeList[i];
            var monsterId = data.monsterId;
            if (!recordMonsterIdList.Contains(monsterId))
                continue;
            if (data.isDead)
                continue;
            if (data.time >= int.MaxValue)
                continue;
            data.time += deltaTime;
            skillDamageData.monsterExistTimeList[i] = data;
        }
    }

    //初始化事件监听
    private void InitHandle()
    {
        if (owner == null || scene == null)
            return;
        onHurtHandle = scene.RegisterEventNew(BeEventSceneType.onDoHurt, (GameClient.BeEvent.BeEventParam beEventParam) =>
        {
            BeActor actor = (BeActor)beEventParam.m_Obj;
            BeActor topOwner = actor.GetTopOwner(actor.GetOwner()) as BeActor;
            if (topOwner != owner)
                return;
            OnHurt((int)beEventParam.m_Int, (BeActor)beEventParam.m_Obj2, (int)beEventParam.m_Int2);
        });

        onSummonHandle = scene.RegisterEventNew(BeEventSceneType.onSummon, (args) =>
        {
            BeActor actor = (BeActor)args.m_Obj2;
            BeActor topOwner = actor.GetTopOwner(actor.GetOwner()) as BeActor;
            if (topOwner != owner)
                return;
            OnSummon((BeActor)args.m_Obj, args.m_Int);
        });

        onHurtByAbnormalHandle = scene.RegisterEventNew(BeEventSceneType.onHurtByAbnormalBuff, (args) =>
        {
            BeActor actor = (BeActor)args.m_Obj;
            if (actor == null)
                return;
            BeActor topOwner = actor.GetTopOwner(actor) as BeActor;
            if (topOwner != owner)
                return;
            OnHurtByAbnormalBuff((int)args.m_Int, (BeActor)args.m_Obj2, args.m_Int2);
        });

        onCastSkillHandle = owner.RegisterEventNew(BeEventType.onCastSkill, args =>
        {
            int skillId = args.m_Int;
            SaveOrigionSkillUseData(skillId);
        });

        onAreaClearHandle = scene.RegisterEventNew(BeEventSceneType.onClear, (args) => 
        {
            timingFlag = false;
        });

        onSceneMonsterDead = scene.RegisterEventNew(BeEventSceneType.onMonsterDead, (args) => 
        {
            SaveMonsterDeadTime((BeActor) args.m_Obj);
        });
    }

    //移除事件监听
    private void RemoveHandle()
    {
        if (onHurtHandle != null)
        {
            onHurtHandle.Remove();
            onHurtHandle = null;
        }

        if (onSummonHandle != null)
        {
            onSummonHandle.Remove();
            onSummonHandle = null;
        }

        if (onHurtByAbnormalHandle != null)
        {
            onHurtByAbnormalHandle.Remove();
            onHurtByAbnormalHandle = null;
        }

        if (onCastSkillHandle != null)
        {
            onCastSkillHandle.Remove();
            onCastSkillHandle = null;
        }

        if (onAreaClearHandle != null)
        {
            onAreaClearHandle.Remove();
            onAreaClearHandle = null;
        }

        if (onSceneMonsterDead != null)
        {
            onSceneMonsterDead.Remove();
            onSceneMonsterDead = null;
        }
    }
    #endregion

    private void InitSkillData()
    {
        if (owner == null)
            return;
        List<int> list = GetActorSkillList(owner);
        for (int i = 0; i < list.Count; i++)
        {
            int skillId = list[i];
            int sourceId = BeUtility.GetComboSkillId(owner,skillId);
            List<int> skillList = new List<int>();
            if (!skillList.Contains(skillId))
                skillList.Add(skillId);
            if (!skillList.Contains(sourceId))
                skillList.Add(sourceId);
            skillIdDic.Add(skillId, skillList);
        }
    }

    //造成伤害时
    private void OnHurt(int hurtValue, BeActor target, int skillId)
    {
        SaveAllData(hurtValue, target, skillId);
    }

    //召唤时
    private void OnSummon(BeActor monster, int skillId)
    {
        if (monster == null)
            return;
        int originId = GetOriginSkillId(skillId);
        if (originId == 0)
            return;
        List<int> monsterSkillIdList = GetActorSkillList(monster);
        skillIdDic[originId].AddRange(monsterSkillIdList);
    }

    //异常Buff造成伤害时
    private void OnHurtByAbnormalBuff(int hurtValue, BeActor target, int skillId)
    {
        SaveAllData(hurtValue, target, skillId);
    }

    //获取一个怪物所有的技能ID
    private List<int> GetActorSkillList(BeActor actor)
    {
        List<int> list = new List<int>();
        Dictionary<int, BeSkill>.Enumerator it = actor.GetSkills().GetEnumerator();
        while (it.MoveNext())
        {
            int id = (int)it.Current.Key;
            list.Add(id);
        }
        return list;
    }

    //获取对应的主角的技能ID
    private int GetOriginSkillId(int id)
    {
        Dictionary<int, List<int>>.Enumerator it = skillIdDic.GetEnumerator();
        while (it.MoveNext())
        {
            int skillId = it.Current.Key;
            List<int> list = it.Current.Value;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] == id)
                    return skillId;
            }
        }
        return 0;
    }

    //保存技能伤害统计所有数据
    private void SaveAllData(int hurtValue, BeActor target, int skillId)
    {
        //针对修罗觉醒特殊处理 这样处理很不好 后期建议修改修罗觉醒
        if (skillId == 1719)
            skillId = 1500;
        int originId = GetOriginSkillId(skillId);
        if (originId == 0)
            return;
        if (originId == 9998)       //主角复活冲击波的伤害不统计
            return;
        int sourceId = BeUtility.GetComboSkillId(owner, originId);
        int monsterId = target.GetEntityData().monsterID;
        string monsterName = target.GetEntityData().name;
        SaveMonsterData(monsterId,monsterName);
        if (sourceId == 1712)
            sourceId = 1500;
        if (sourceId == 3100 || sourceId == 3101)   //格斗家 猫拳特殊处理
            sourceId = 3000;
        SaveDamageData(sourceId, hurtValue, monsterId);
        SaveMonsterBeHitTime(target.GetPID(), monsterId, target);
    }

    /// <summary>
    /// 保存怪物被击时候的时间
    /// </summary>
    private void SaveMonsterBeHitTime(int pid, int monsterId,BeActor target)
    {
        if (!CheckTargetIsMeetCondition(target))
            return;
        if (skillDamageData.monsterExistTimeList == null)
        {
            skillDamageData.monsterExistTimeList = new List<MonsterExistTime>();
        }

        if (recordMonsterIdList.Contains(monsterId))
            return;
        MonsterExistTime data = new MonsterExistTime();
        data.monsterId = monsterId;
        skillDamageData.monsterExistTimeList.Add(data);
        recordMonsterIdList.Add(monsterId);
    }

    /// <summary>
    /// 检测攻击目标是否满足条件
    /// </summary>
    private bool CheckTargetIsMeetCondition(BeActor actor)
    {
        if (!actor.IsMonster())
            return false;
        if (actor.GetEntityData().monsterData == null)
            return false;
        if (actor.GetEntityData().monsterData.Type != ProtoTable.UnitTable.eType.BOSS
            && actor.GetEntityData().monsterData.Type != ProtoTable.UnitTable.eType.ELITE)
            return false;
        return true;
    }

    /// <summary>
    /// 保存怪物死亡时间
    /// </summary>
    private void SaveMonsterDeadTime(BeActor actor)
    {
        if (actor == null)
            return;
        var monsterId = actor.GetEntityData().monsterID;
        if (!recordMonsterIdList.Contains(monsterId))
            return;
        recordMonsterIdList.Remove(monsterId);
    }

    /// <summary>
    /// 获取指定ID的怪物存活总时间
    /// </summary>
    /// <param name="monsterId"></param>
    public double GetMonsterTime(int monsterId, SkillDamageData data)
    {
        if (monsterId == 0)
            return data.totalTime;
        if (data.monsterExistTimeList == null || data.monsterExistTimeList.Count <= 0)
            return 0;
        for (int i = 0; i < data.monsterExistTimeList.Count; i++)
        {
            var timeData = data.monsterExistTimeList[i];
            if(timeData.monsterId == monsterId)
            {
                return timeData.time;
            }
        }
        return 0;
    }

    //存储技能使用次数
    private void SaveOrigionSkillUseData(int skillId)
    {
        int sourceId = BeUtility.GetComboSkillId(owner, skillId);
        if (skillId != sourceId)
            return;
        if (sourceId == 1712)
            sourceId = 1500;
        int index = UseSkillIndex(sourceId);
        if (index != -1)
            skillDamageData.origionSkillIdUseCount[index] += 1;
    }

    //该技能是否是第一次使用
    private int UseSkillIndex(int skillId)
    {
        if (skillDamageData.origionSkillIdList == null)
            skillDamageData.origionSkillIdList = new List<int>();
        if (skillDamageData.origionSkillIdUseCount == null)
            skillDamageData.origionSkillIdUseCount = new List<int>();
        int index = _FindIndex(skillDamageData.origionSkillIdList,skillId);
        if (index == -1)
        {
            skillDamageData.origionSkillIdList.Add(skillId);
            skillDamageData.origionSkillIdUseCount.Add(1);
            return index;
        }
        return index;
    }
    private int _FindIndex(List<int> findList,int skillID)
    {
        for(int i = 0; i < findList.Count;i++)
        {
           if(findList[i] == skillID)
            {
                return i;
            }
        }
        return -1;
    }

    //存储怪物数据
    private void SaveMonsterData(int monsterId,string monsterName)
    {
        if (skillDamageData.monsterIdList == null)
        {
            skillDamageData.monsterIdList = new List<int>();
            skillDamageData.monsterIdList.Add(0);       //添加默认
        }
            
        bool bFinded = skillDamageData.monsterIdList.Contains(monsterId);
        if (bFinded)
            return;
        skillDamageData.monsterIdList.Add(monsterId);
        if (skillDamageData.monsterNameList == null)
        {
            skillDamageData.monsterNameList = new List<string>();
            skillDamageData.monsterNameList.Add("所有");
        }
        if (monsterName.Length > 8)
            monsterName.Substring(0,8);
        skillDamageData.monsterNameList.Add(monsterName);
    }

    //存储伤害数据
    private void SaveDamageData(int skillId,long damage,int monsterId)
    {
        //检测该技能是不是第一次释放
        UseSkillIndex(skillId);

        if (skillDamageData.skillDamageDic == null)
            skillDamageData.skillDamageDic = new Dictionary<int, List<DamageData>>();
        if (!skillDamageData.skillDamageDic.ContainsKey(monsterId))
            skillDamageData.skillDamageDic.Add(monsterId,new List<DamageData>());
        List<DamageData> damageDataList = skillDamageData.skillDamageDic[monsterId];
        bool isExit = false;
        for(int i = 0; i < damageDataList.Count; i++)
        {
            DamageData data = damageDataList[i];
            if (data.skillId == skillId)
            {
                data.damage += damage;
                damageDataList[i] = data;
                isExit = true;
                break;
            }
        }

        if (!isExit)
        {
            DamageData data = new DamageData();
            data.skillId = skillId;
            data.damage = damage;
            damageDataList.Add(data);
        }

        if (!timingFlag)
        {
            timingFlag = true;
        }
    }

    //保存技能伤害数据到本地磁盘
    public void SaveSkillDamageData(BeActor actor, string dungeonName)
    {
#if !LOGIC_SERVER
        if (actor.skillDamageManager == null)
            return;

        SkillDamageData current = actor.skillDamageManager.skillDamageData;
        current.dungeonName = dungeonName;
        if (current.skillDamageDic == null || current.skillDamageDic.Count <= 0)
            return;

        List<SkillDamageData> origion = GetSkillDamageData();
        List<SkillDamageData> newData = new List<SkillDamageData>();

        newData.Add(current);
        if (origion != null)
        {
            for (int i = 0; i < origion.Count; i++)
            {
                if (newData.Count >= 3)
                    break;
                newData.Add(origion[i]);
            }
        }
        SkillDamageStorage.GetInstance().SaveSkillDamageData(newData);
#endif
    }

    //获取之前保存的技能伤害数据
    public List<SkillDamageData> GetSkillDamageData()
    {
#if !LOGIC_SERVER
        return SkillDamageStorage.GetInstance().GetSkillDamageData();
#else
            return null;
#endif
    }

    //清除数据
    public void ClearSkillDamageData()
    {
#if !LOGIC_SERVER
        SkillDamageStorage.GetInstance().Clear();
#endif
    }

    //获取技能伤害百分比
    public string GetSkillDamagePercent(int skillId, int monsterId, SkillDamageData data)
    {
        long skillDamage = GetSkilDamage(skillId, monsterId, data);
        long totalDamage = GetTotalDamage(monsterId, data);
        VFactor factor = new VFactor(skillDamage, totalDamage);
        string result = (factor.single * 100).ToString("0.0") + "%";
        return result;
    }

    //获取单个技能 针对特定怪物造成的伤害
    public long GetSkilDamage(int skillId, int monsterId, SkillDamageData data)
    {
        long damage = 0;
        if (monsterId != 0 && !data.skillDamageDic.ContainsKey(monsterId))
            return damage;
        Dictionary<int, List<DamageData>> skillDamageDic = data.skillDamageDic;
        Dictionary<int, List<DamageData>>.Enumerator it = skillDamageDic.GetEnumerator();
        while (it.MoveNext())
        {
            int key = (int)it.Current.Key;
            List<DamageData> value = (List<DamageData>)it.Current.Value;
			
            if (monsterId != 0 && key != monsterId)
            	continue;
            for (int i = 0; i < value.Count; i++)
            {
                if (value[i].skillId == skillId)
                    damage += value[i].damage;
            }
        }
        return damage;
    }

    //获取所有技能打出的总的伤害
    public long GetTotalDamage(int monsterId, SkillDamageData data)
    {
        long totalDamage = 0;
        if (data.skillDamageDic == null)
            return totalDamage;
        Dictionary<int, List<DamageData>> skillDamageDic = data.skillDamageDic;
        Dictionary<int, List<DamageData>>.Enumerator it = skillDamageDic.GetEnumerator();
        while (it.MoveNext())
        {
            int key = (int)it.Current.Key;
            List<DamageData> value = (List<DamageData>)it.Current.Value;
            for (int i = 0; i < value.Count; i++)
            {
                if (monsterId != 0 && key != monsterId)
                    continue;
                totalDamage += value[i].damage;
            }
        }
        return totalDamage;
    }

    //停止计时
    public void SetTimingFlag(bool isOpen)
    {
        timingFlag = isOpen;
    }
}
