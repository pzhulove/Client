using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// 数据统计
/// </summary>
public class DungeonStatistics
{
    /// <summary>
    /// 从开始战斗到怪物清除的时间
    /// </summary>
    public int areaFightTime = 0;
    /// <summary>
    /// 从怪物清除到去下一个区域的时间
    /// </summary>
    public int areaClearTime = 0;

    /// <summary>
    /// 最后访问的帧
    /// </summary>
    public uint lastVisitFrame = uint.MaxValue;

    private List<DungeonHurtData> mMaxHurtDatas = new List<DungeonHurtData>();
    private UInt64 mSumDamage = 0;

    public UInt64 GetSumDamage()
    {
        return mSumDamage;
    }

    public void AddHurtData(int skillId, int hurtId, int damage)
    {
        if (damage >= 0)
        {
            mSumDamage += (UInt64)damage;
            Logger.LogProcessFormat("[战斗] 伤害总数 {0}", mSumDamage);
        }
        else
        {
            Logger.LogError("[战斗] 伤害数据为负数");
        }

        if (null == mMaxHurtDatas)
        {
            Logger.LogError("[战斗] 伤害数据为空");
            return ;
        }

        if (mMaxHurtDatas.Count <= 0)
        {
            DungeonHurtData data = new DungeonHurtData(skillId, hurtId, damage);
            mMaxHurtDatas.Add(data);
            return ;
        }

        DungeonHurtData savedData = mMaxHurtDatas[0];

        if (null == savedData)
        {
            Logger.LogError("[战斗] 保存数据为空");
            return ;
        }

        if (damage > savedData.damage)
        {
            DungeonHurtData maxData = GetMaxHurtData();

            Logger.LogProcessFormat("[战斗] 伤害更新 :SID {0}->{1}, HID:{2}->{3}, DAMAGE:{4}->{5}",
                    maxData.skillId, skillId,
                    maxData.hurtId,  hurtId,
                    maxData.skillId, skillId);

            maxData.skillId = skillId;
            maxData.hurtId  = hurtId;
            maxData.damage  = damage;
        }
    }

    public DungeonHurtData GetMaxHurtData()
    {
        if (null == mMaxHurtDatas || mMaxHurtDatas.Count <= 0)
        {
            return new DungeonHurtData();
        }

        // TODO heapsort
        
        int maxIndex = 0;
        int maxDamage = -1;

        for (int i = 0; i < mMaxHurtDatas.Count; ++i)
        {
            if (null == mMaxHurtDatas[i])
            {
                continue;
            }

            if (mMaxHurtDatas[i].damage > maxDamage)
            {
                maxDamage = mMaxHurtDatas[i].damage;
                maxIndex = i;
            }
        }

        return mMaxHurtDatas[maxIndex];
    }
}

public class DungeonHurtData
{
    public DungeonHurtData()
    {
        this.skillId = 0;
        this.hurtId  = 0;
        this.damage  = 0;
    }

    public DungeonHurtData(int skillId, int hurtId, int damage)
    {
        this.skillId = skillId;
        this.hurtId  = hurtId;
        this.damage  = damage;
    }

    //public int playerSeat { get; set; }

    public int skillId    { get; set; }
    public int hurtId     { get; set; }
    public int damage     { get; set; }
}
