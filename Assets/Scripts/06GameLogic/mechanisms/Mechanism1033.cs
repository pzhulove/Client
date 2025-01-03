using System.Collections;
using System.Collections.Generic;
using GameClient;
using UnityEngine;

/// <summary>
/// 受到一定伤害以后 攻击者或者自己身旁创建一个爆炸实体
/// </summary>
public class Mechanism1033 : BeMechanism
{
    public Mechanism1033(int id, int level) : base(id, level) { }

    private struct BeHitData
    {
        
        public int skillId;   //监听到的技能ID
        public int addRate;    // 增长的千分比
    }
    
    private List<BeHitData> beHitDataList = new List<BeHitData>();  // 被击数据列表
    private List<int> skillIdList = new List<int>();    // 监听的技能ID列表

    private int entityId = 0;   // 创建的实体ID
    private int curHurtValue = 0; // 当前受到的伤害
    private readonly int maxHurtValue = 1000;    // 受到的最大伤害
    private List<int> buffInfoIdList = new List<int>();

    // 每秒减少数值
    private int reduceValue = 1;  
    private int curTimeAcc = 0;
    private int timeAcc = 1000; // 毫秒  

    protected SpellBar spellBar = null; // 能量条

    private int summonTimeAcc = 2000;  //召唤怪物事件间隔
    private int curSummonTime = 0;
    private int summonMonsterId = 0;
    private bool needSummonUpdate = false;

    public override void OnInit()
    {
        for(int i = 0; i < data.ValueA.Count;i++)
        {
            BeHitData beHitData = new BeHitData();
            int skillId = TableManager.GetValueFromUnionCell(data.ValueA[i], level);
            beHitData.skillId = skillId;
            if(i <data.ValueB.Count)
                beHitData.addRate = TableManager.GetValueFromUnionCell(data.ValueB[i],level);
            beHitDataList.Add(beHitData);
            skillIdList.Add(skillId);
        }

        entityId = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
        reduceValue = TableManager.GetValueFromUnionCell(data.ValueD[0],level);

        summonTimeAcc = TableManager.GetValueFromUnionCell(data.ValueE[0], level);
        summonMonsterId = TableManager.GetValueFromUnionCell(data.ValueF[0], level);

        for(int i = 0; i < data.ValueH.Count; i++)
        {
            buffInfoIdList.Add(TableManager.GetValueFromUnionCell(data.ValueH[i], level));
        }
    }

    public override void OnReset()
    {
        beHitDataList.Clear();
        skillIdList.Clear();
        curHurtValue = 0;
        spellBar = null;
        curSummonTime = 0;
        needSummonUpdate = false;
        curTimeAcc = 0;
    }

    public override void OnStart()
    {
        base.OnStart();
        handleA = owner.RegisterEventNew(BeEventType.onHitAfterAddBuff, AddHurtValue);
        //handleA = owner.RegisterEvent(BeEventType.onHitAfterAddBuff, AddHurtValue);
        handleB = owner.RegisterEventNew(BeEventType.onMagicGirlMonsterChange, RegisterMagicGirlChange);
    }

    public override void OnUpdate(int deltaTime)
    {
        base.OnUpdate(deltaTime);
        UpdateReduce(deltaTime);
        UpdateSummon(deltaTime);
    }

    public override void OnFinish()
    {
        base.OnFinish();
        RemoveSpellBar();
    }

    /// <summary>
    /// 每秒减少
    /// </summary>
    private void UpdateReduce(int deltaTime)
    {
        if(curTimeAcc < timeAcc)
        {
            curTimeAcc += deltaTime;
            return;
        }

        curTimeAcc = 0;
        curHurtValue -= reduceValue;

        if (curHurtValue <= 0)
        {
            //owner.buffController.RemoveBuff(attachBuff);
            RemoveAttachBuff();
        }
        else
        {
            RefreshEnergrUI(-reduceValue);
        }
    }

    /// <summary>
    /// 召唤怪物
    /// </summary>
    private void UpdateSummon(int deltaTime)
    {
        if (summonMonsterId == 0)
            return;
        if (!needSummonUpdate)
            return;
        if (curSummonTime >= summonTimeAcc)
        {
            curSummonTime = 0;
            
            var thisAttachBuff = GetAttachBuff();
            if (thisAttachBuff != null && thisAttachBuff.releaser != null)
            {
                CreateMonster();
            }
        }
        else
        {
            curSummonTime += deltaTime;
        }
    }

    /// <summary>
    /// 监听伤害
    /// </summary>
    /// <param name="args"></param>
    private void AddHurtValue(BeEvent.BeEventParam param)
    {
        var skillId = param.m_Int2;
        if (!skillIdList.Contains(skillId))
            return;
        if (curHurtValue >= 1000)
            return;
        int addRate = GetAddRateById(skillId);
        curHurtValue += addRate;
        RefreshEnergrUI(curHurtValue);

        if (curHurtValue >= maxHurtValue)
        {
            BeActor attacker = param.m_Obj as BeActor;

            //创建实体
            AddEntity(attacker);

            //添加Buff信息
            AddBuffInfo();

            //能量条满
            EneryFull();
        }
    }

    /// <summary>
    /// 能量条满了
    /// </summary>
    private void EneryFull()
    {
        if (summonMonsterId != 0)
        {
            needSummonUpdate = true;
            CreateMonster();
        }
        else
        {
            ResetDamgeRecord();
            RemoveSpellBar();
        }
    }

    /// <summary>
    /// 创建一个实体
    /// </summary>
    private void AddEntity(BeActor attacker)
    {
        if (entityId == 0)
            return;
        if (attacker == null)
            return;
        attacker.AddEntity(entityId, owner.GetPosition());
    }

    /// <summary>
    /// 添加一个Buff信息
    /// </summary>
    private void AddBuffInfo()
    {
        if (buffInfoIdList.Count <= 0)
            return;
        for (int i = 0; i < buffInfoIdList.Count; i++)
        {
            BeActor releaser = null;
            var thisAttachBuff = GetAttachBuff();
            if (thisAttachBuff != null)
            {
                releaser = thisAttachBuff.releaser;
            }
            owner.buffController.TryAddBuff(buffInfoIdList[i],null,false, releaser);
        }
    }

    /// <summary>
    /// 监听召唤师觉醒变身
    /// </summary>
    /// <param name="args"></param>
    private void RegisterMagicGirlChange(BeEvent.BeEventParam args)
    {
        var thisAttachBuff = GetAttachBuff();
        if (thisAttachBuff != null)
            thisAttachBuff.Finish();
    }

    /// <summary>
    /// 创建怪物
    /// </summary>
    private void CreateMonster()
    {
        var thisAttachBuff = GetAttachBuff();
        if (thisAttachBuff != null && thisAttachBuff.releaser != null)
        {
            int level = thisAttachBuff.releaser.GetEntityData().level;
            int monsterNewId = owner.GenNewMonsterID(summonMonsterId,level);
            thisAttachBuff.releaser.CurrentBeScene.SummonMonster(monsterNewId, owner.GetPosition(), thisAttachBuff.releaser.GetCamp());
        }
    }

    /// <summary>
    /// 获取增加比率通过技能ID
    /// </summary>
    /// <returns></returns>
    private int GetAddRateById(int skillId)
    {
        for(int i = 0; i < beHitDataList.Count; i++)
        {
            if (beHitDataList[i].skillId == skillId)
                return beHitDataList[i].addRate;
        }
        return 0;
    }

    /// <summary>
    /// 重置伤害统计
    /// </summary>
    private void ResetDamgeRecord()
    {
        curTimeAcc = 0;
        curHurtValue = 0;
    }

    protected void RefreshEnergrUI(int value)
    {
#if !LOGIC_SERVER
        SpellBar bar = null;
        var dur = owner.GetSpellBarDuration(eDungeonCharactorBar.MonsterEnergyBar);
        if (dur <= 0)
        {
            string content = "";
            if (data.StringValueA.Count > 0)
            {
                content = data.StringValueA[0];
            }
            bar = owner.StartSpellBar(eDungeonCharactorBar.MonsterEnergyBar, maxHurtValue, true, content);
            bar.autoAcc = false;
            bar.reverse = false;
            bar.autodelete = false;
        }
        owner.AddSpellBarProgress(eDungeonCharactorBar.MonsterEnergyBar, new VFactor(value, 1000));
#endif
    }

    private void RemoveSpellBar()
    {
#if !LOGIC_SERVER
        owner.StopSpellBar(eDungeonCharactorBar.MonsterEnergyBar);
#endif
    }
}
