using System;
using System.Collections.Generic;
using GameClient;

//玩家战斗伤害数据保存类
public class SkillDamageStorage : DataManager<SkillDamageStorage>
{
    private List<SkillDamageData> skillDataStorageList = new List<SkillDamageData>();   //用于存储战斗数据

    //存储伤害数据
    public void SaveSkillDamageData(List<SkillDamageData> list)
    {
        skillDataStorageList = list;
    }

    //获取伤害数据
    public List<SkillDamageData> GetSkillDamageData()
    {
        return skillDataStorageList;
    }

    //初始化
    public override void Initialize()
    {
        skillDataStorageList.Clear();
    }

    //清除数据
    public override void Clear()
    {
        ResetData();
    }

    //重置数据
    public void ResetData()
    {
        skillDataStorageList.Clear();
    }
}