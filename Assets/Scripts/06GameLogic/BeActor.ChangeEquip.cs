using System.Collections.Generic;
using GameClient;
using Protocol;
using ProtoTable;
using System;

/// <summary>
/// 切换装备用
/// </summary>

public struct EquipSchemeData
{
    /// <summary>
    /// 装备列表
    /// </summary>
    public List<ItemProperty> EquipList;

    /// <summary>
    /// 称号
    /// </summary>
    public ItemProperty Title;

    /// <summary>
    /// 称号ID
    /// </summary>
    public int TitlId;
}

/// <summary>
///用于记录添加装备数据
/// </summary>
public struct RecordEquipAddData
{
    public ItemProperty item;
    public List<int> mechanismIdList;
    public List<int> mechanismBuffIdList;
}


public partial class BeActor
{
    protected EquipSchemeData[] m_ChangeEquipDataList;
    protected int m_CurrentChangeEquipIndex = 0;
    protected int m_SchemeLength = 2;
    List<RecordEquipAddData> m_RecordEquipDataList = new List<RecordEquipAddData>();

    /// <summary>
    /// 初始化装备切换数据
    /// </summary>
    public void InitChangeEquipData(RaceEquip[] equipArr, RaceEquipScheme[] schemeArr)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeActor.InitChangeEquipData"))
        {
#endif
        if (equipArr == null)
        {
            return;
        }

        if (schemeArr == null)
        {
            return;
        }
        
        m_SchemeLength = schemeArr.Length;

        m_ChangeEquipDataList = new EquipSchemeData[m_SchemeLength];

        for (int i = 0; i < m_SchemeLength; i++)
        {
            var scheme = schemeArr[i];
            InitEquipDataByScheme(equipArr, scheme, (int)scheme.id - 1);
        }
#if ENABLE_PROFILER
        }
#endif
    }

    /// <summary>
    /// 设置当前穿戴的装备列表
    /// </summary>
    protected void InitEquipDataByScheme(RaceEquip[] equipArr, RaceEquipScheme scheme, int index)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeActor.InitEquipDataByScheme"))
        {
#endif
        if (scheme == null)
        {
            return;
        }

        bool isWear = false;
        if (scheme.isWear == 1)
        {
            isWear = true;
            m_CurrentChangeEquipIndex = index;
        }

        InitSwitchEquipIcon(m_CurrentChangeEquipIndex);

        if (scheme.equips == null)
        {
            return;
        }

        EquipSchemeData changeEquipData = new EquipSchemeData();
        changeEquipData.EquipList = new List<ItemProperty>();
        changeEquipData.Title = null;
        changeEquipData.TitlId = 0;

        for (int i = 0; i < equipArr.Length; i++)
        {
            var equip = equipArr[i];
            ItemProperty itemProperty = null;

            //称号
            if (equip.uid == scheme.title)
            {
                itemProperty = GetEquip(isWear,equip);
                if (itemProperty != null)
                {
                    changeEquipData.Title = GetEquip(isWear, equip);
                    changeEquipData.TitlId = (int)equip.id;
                    continue;
                }
            }

            //装备
            if (Array.IndexOf(scheme.equips, equip.uid) >= 0)
            {
                itemProperty = GetEquip(isWear, equip);
                if (itemProperty == null)
                {
                    continue;
                }
                else
                {
                    changeEquipData.EquipList.Add(itemProperty);
                }
            }
        }
        m_ChangeEquipDataList[index] = changeEquipData;
#if ENABLE_PROFILER
        }
#endif
    }

    /// <summary>
    /// 初始化修炼场切换装备数据
    /// </summary>
    public void InitTrainingPveBattleData()
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeActor.InitTrainingPveBattleData"))
        {
#endif
        if (ItemDataManager.GetInstance() == null)
            return;
        if (EquipPlanDataManager.GetInstance() == null)
            return;
        if (!EquipPlanUtility.IsEquipPlanOpenedByServer())
            return;

        //未启用装备方案里面如果没有数据 则不能切换装备
        int notEnableEquipIndex = EquipPlanDataManager.GetInstance().CurrentSelectedEquipPlanId == 1 ? 2 : 1;
        if (EquipPlanUtility.OnlyGetEquipItemGuidListInEquipPlanByEquipPlanId(notEnableEquipIndex) == null
            && EquipPlanUtility.OnlyGetTitleItemGuidInEquipPlanByEquipPlanId(notEnableEquipIndex) == 0)
            return;

        m_ChangeEquipDataList = new EquipSchemeData[m_SchemeLength];
        m_CurrentChangeEquipIndex = EquipPlanDataManager.GetInstance().CurrentSelectedEquipPlanId - 1;

        InitSwitchEquipIcon(m_CurrentChangeEquipIndex);

        for (int i = 0; i < 2; i++)
        {
            int index = i + 1;
            bool isWear = i == m_CurrentChangeEquipIndex;

            EquipSchemeData changeEquipData = new EquipSchemeData();
            changeEquipData.EquipList = new List<ItemProperty>();
            changeEquipData.Title = null;
            changeEquipData.TitlId = 0;

            List<ulong> equipList = EquipPlanUtility.OnlyGetEquipItemGuidListInEquipPlanByEquipPlanId(index);
            if (equipList != null)
            {
                for (int j = 0; j < equipList.Count; j++)
                {
                    ItemProperty itemProperty = GetEquipInTraningPve(isWear,equipList[j]);
                    changeEquipData.EquipList.Add(itemProperty);
                }
            }

            ulong titleId = EquipPlanUtility.OnlyGetTitleItemGuidInEquipPlanByEquipPlanId(index);
            var titleItem = ItemDataManager.GetInstance().GetItem(titleId);
            if (titleItem != null)
            {
                ItemProperty titleProperty = GetEquipInTraningPve(isWear, titleId);
                changeEquipData.Title = titleProperty;
                changeEquipData.TitlId = (int)titleItem.TableID;
            }

            m_ChangeEquipDataList[i] = changeEquipData;
        }
#if ENABLE_PROFILER
        }
#endif
    }

    /// <summary>
    /// 切换装备(装备序号)
    /// </summary>
    /// <param name="index"></param>
    public void ChangeEquip(int index)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeActor.ChangeEquip"))
        {
#endif
        if (m_ChangeEquipDataList == null)
        {
            Logger.LogErrorFormat("没有初始化切换装备数据,不能切换装备");
            return;
        }

        if (m_CurrentChangeEquipIndex >= m_ChangeEquipDataList.Length || index >= m_ChangeEquipDataList.Length)
        {
            Logger.LogErrorFormat("切换装备传入的序号越界了, index:{0}", m_CurrentChangeEquipIndex);
            return;
        }

        EquipSchemeData oldData = m_ChangeEquipDataList[m_CurrentChangeEquipIndex];
        EquipSchemeData newData = m_ChangeEquipDataList[index];

        m_RecordEquipDataList.Clear();

        //移除旧的装备
        if (oldData.EquipList != null)
        {
            for (int i = 0; i < oldData.EquipList.Count; i++)
            {
                var item = oldData.EquipList[i];
                RemoveOldEquip(item);
            }
        }
        //移除旧的称号
        if (oldData.Title != null)
        {
            RemoveOldEquip(oldData.Title);
        }
        
        //移除套装数据
        RemoveOldEquip(suitProperty);
        
        //移除护甲精通数据
        RemoveOldEquip(masterProperty); 

        //添加新的装备
        if (newData.EquipList != null)
        {
            for (int i = 0; i < newData.EquipList.Count; i++)
            {
                var item = newData.EquipList[i];
                AddNewEquip(item);
            }
        }

        //添加新的称号
        if (newData.Title != null)
        {
            AddNewEquip(newData.Title);
        }
        ChangeTitleUI(newData.TitlId);

        if (equips != null)
        {
            List<int> equipments = new List<int>();
            for (int i = 0; i < equips.Count; ++i)
            {
                equipments.Add(equips[i].itemID);
            }

            //套装
            suitProperty = EquipSuitDataManager.GetInstance().GetEquipSuitPropByIDs(equipments);

            if (attribute != null)
            {
                //护甲精通
                masterProperty = EquipMasterDataManager.GetInstance().GetEquipMasterPropByIDs(attribute.professtion, equipments);
            }
            else
            {
                Logger.LogError("attribute is null");
            }

            if (suitProperty != null)
            {
                AddNewEquip(suitProperty);
            }
            if (masterProperty != null)
            {
                AddNewEquip(masterProperty);
            }   
        }

        //添加装备附带的机制和机制Buff
        RealAddBuffAddMechanisms();

        RefreshDataByChangeEquipOrWeapon();

        //刷新最大血量
        attribute.ChangeMaxHpByResist();
        m_CurrentChangeEquipIndex = index;
            
        TriggerEventNew(BeEventType.onChangeEquipEnd, new EventParam(){ m_Obj = this});
#if ENABLE_PROFILER
        }
#endif
    }

    /// <summary>
    /// 切换装备完成以后刷新角色数据
    /// </summary>
    protected void RefreshDataByChangeEquipOrWeapon()
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeActor.RefreshDataByChangeEquipOrWeapon"))
        {
#endif
        Dictionary<int, CrypticInt32> skillLevelBeforeChange = new Dictionary<int, CrypticInt32>(attribute.skillLevelInfo);
        Dictionary<int, CrypticInt32> skillLevelAfterChange = new Dictionary<int, CrypticInt32>(attribute.skillLevelInfo);

        //技能等级变化的处理
        var enumerator = skillLevelBeforeChange.GetEnumerator();
        while (enumerator.MoveNext())
        {
            int skillID = enumerator.Current.Key;
            int skillLevelBefore = enumerator.Current.Value;
            int skillLevelAfter = 0;
            if (skillLevelAfterChange.ContainsKey(skillID))
            {
                skillLevelAfter = skillLevelAfterChange[skillID];
            }

            if (skillLevelBefore != skillLevelAfter)
            {
                var skill = GetSkill(skillID);
                if (skill != null)
                {
                    skill.OnInit();
                    if (skill.skillData != null && skill.skillData.SkillType == SkillTable.eSkillType.PASSIVE)
                    {
                        skill.OnPostInit();
                    }
                    skill.DealLevelChange();
                }
            }
        }

        //处理技能阶段
        if (IsCastingSkill())
        {
            skillController.SetSkillPhases(GetCurSkillID());
        }

        //刷新技能
        var enumerator2 = skillController.GetSkills().GetEnumerator();
        while (enumerator2.MoveNext())
        {
            BeSkill skill = enumerator2.Current.Value;
            if (skill != null)
            {
                skill.DealEquipChange();
            }
        }

        attribute.ChangeMaxHpByResist();
        attribute.battleData.RefreshMpInfo();

#if !LOGIC_SERVER
        if (m_pkGeActor != null)
        {
            if(m_pkGeActor.mCurHpBar != null)
            {
                m_pkGeActor.mCurHpBar.InitResistMagic(attribute.GetResistMagic(), this);
            }

            m_pkGeActor.SyncHPBar();
        }
#endif
#if ENABLE_PROFILER
        }
#endif
    }

    /// <summary>
    /// 刷新称号UI相关
    /// </summary>
    protected void ChangeTitleUI(int newTitleId)
    {
#if !LOGIC_SERVER

        m_pkGeActor.OnTittleChanged(newTitleId);
#endif
    }

    /// <summary>
    /// 移除老的装备
    /// </summary>
    protected void RemoveOldEquip(ItemProperty item)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeActor.RemoveOldEquip"))
        {
#endif
        if (item == null)
            return;
        if (BattleMain.IsModePvP(battleType))
        {
            RemoveMechanisms(item.attachPVPMechanismIDs);
            RemoveMechanismBuffs(item.attachPVPBuffIDs, item);
        }
        else
        {
            RemoveMechanisms(item.attachMechanismIDs);
            RemoveMechanismBuffs(item.attachBuffIDs, item);
        }
        attribute.RemoveEquip(item);
        attribute.RemoveEquipment(item);
#if ENABLE_PROFILER
        }
#endif
    }

    /// <summary>
    /// 添加新的装备
    /// </summary>
    protected void AddNewEquip(ItemProperty item)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeActor.AddNewEquip"))
        {
#endif
        if (item == null)
            return;
        RecordEquipAddData data = new RecordEquipAddData();
        data.item = item;
        data.mechanismIdList = new List<int>();
        data.mechanismBuffIdList = new List<int>();
        data.mechanismIdList.AddRange(BattleMain.IsModePvP(battleType) ? item.attachPVPMechanismIDs : item.attachMechanismIDs);
        data.mechanismBuffIdList.AddRange(BattleMain.IsModePvP(battleType) ? item.attachPVPBuffIDs : item.attachBuffIDs);
        m_RecordEquipDataList.Add(data);
        attribute.AddEquipment(item);
        attribute.AddEquip(item);
#if ENABLE_PROFILER
        }
#endif
    }

    /// <summary>
    /// 真正的添加装备携带的Buff和机制
    /// </summary>
    protected void RealAddBuffAddMechanisms()
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeActor.RealAddBuffAddMechanisms"))
        {
#endif
        for(int i=0;i< m_RecordEquipDataList.Count; i++)
        {
            var data = m_RecordEquipDataList[i];
            LoadMechanisms(data.mechanismIdList, 0, MechanismSourceType.EQUIP);
        }

        for (int i = 0; i < m_RecordEquipDataList.Count; i++)
        {
            var data = m_RecordEquipDataList[i];
            LoadMechanismBuffs(data.mechanismBuffIdList, 0, false, data.item, true);
        }
#if ENABLE_PROFILER
        }
#endif
    }

    /// <summary>
    /// 获取装备方案数量
    /// </summary>
    /// <returns>获取当前方案总数量</returns>
    public int GetSchemeCount()
    {
        if (m_ChangeEquipDataList == null)
            return 0;
        return m_ChangeEquipDataList.Length;
    }

    /// <summary>
    /// 获取当前的装备方案
    /// </summary>
    /// <returns></returns>
    public int GetCurrentSchemeIndex()
    {
        return m_CurrentChangeEquipIndex;
    }

    /// <summary>
    /// 初始化切换按钮图标
    /// </summary>
    protected void InitSwitchEquipIcon(int index)
    {
#if !LOGIC_SERVER
        var battleUI = BattleUIHelper.GetBattleUIComponent<BattleUISwitchWeaAndEquip>();
        if (battleUI == null)
            return;
        battleUI.InitSwitchEquipIcon(m_CurrentChangeEquipIndex);
#endif
    }

    /// <summary>
    /// 获取道具装备
    /// </summary>
    /// <returns></returns>
    protected ItemProperty GetEquip(bool isWear,RaceEquip equip)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeActor.GetEquip"))
        {
#endif
        if (isWear)
        {
            return attribute.GetWearEquipByGUID(equip.uid);
        }
        return BattlePlayer.GetEquip(equip, BattleMain.IsModePvP(battleType));
#if ENABLE_PROFILER
        }
#endif
    }

    /// <summary>
    /// d修炼场中获取装备
    /// </summary>
    protected ItemProperty GetEquipInTraningPve(bool isWear,ulong guid)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeActor.GetEquipInTraningPve"))
        {
#endif
        if (isWear)
        {
            return attribute.GetWearEquipByGUID(guid);
        }

        ItemData item = ItemDataManager.GetInstance().GetItem(guid);
        if (item != null)
        {
            ItemProperty ip = item.GetBattleProperty();
            ip.itemID = (int)item.TableID;
            ip.guid = item.GUID;
            return ip;
        }
        return null;
#if ENABLE_PROFILER
        }
#endif
    }
}
