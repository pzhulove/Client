/**
* 命名空间: 
*
* 功 能： 力法 炫纹机制
* 类 名： mechanism2072
* 创建人：Jeffery
* 创建时间：2019-11-27 9:48:54
* Ver 变更日期 负责人 变更内容
* ───────────────────────────────────
* V0.01 2019-11-27
**/

using System;
using System.ComponentModel;
using System.Collections.Generic;
using GameClient;
using UnityEngine;

public class Mechanism2072 : BeMechanism
{
    public static readonly int ChaserMgrID = 5282;
    public Mechanism2072(int id, int lv) : base(id, lv) { }

    /// <summary>
    /// 炫纹类型枚举
    /// </summary>
    public enum ChaserType
    {
        [Description("无")]
        None = 0,

        [Description("光属性炫纹")]
        Light,

        [Description("火属性炫纹")]
        Fire,

        [Description("冰属性炫纹")]
        Ice,

        [Description("暗属性炫纹")]
        Dark,
        
        Max
    }

    /// <summary>
    /// 炫纹大小
    /// </summary>
    public enum ChaseSizeType
    {
        [Description("一般大小")]
        Normal = 0,

        [Description("大炫纹")]
        Big,
        
        Max

       // [Description("超大炫纹")]
       //VeryBig,
    }

    /// <summary>
    /// 炫纹数据结构
    /// </summary>
    public class ChaserData
    {
        public ChaserType type;
        public ChaseSizeType sizeType;
        public int liveTime;
        public VInt3 position;
        public VFactor scale;
        public VInt3 anchorPos;
#if !LOGIC_SERVER
        public GeEffectEx effect;
#endif
    }

    private Vector3[] m_ChaserPosArrEffect =
    {
        new Vector3(-0.752f, 1.811f, -0.38f),
        new Vector3(-0.997f, 1.484f, -0.107f),
        new Vector3(-0.618f, 1.131f, 0.54f),
        new Vector3(-1.018f, 1.166f, -0.201f),
        new Vector3(-0.687f, 0.74f, 0.413f),
        new Vector3(-0.45f, 0.518f, -0.14f),
        new Vector3(-0.799f, 0.265f, 0.093f),
        new Vector3(-0.962f, 0.7f, -0.21f),
        new Vector3(-1.324f, 0.674f, 0.575f),
        new Vector3(-1.245f, 0.627f, -0.481f),
        new Vector3(-1.418f, 1.206f, 0.136f)
    };
    
    /// <summary>
    /// 最大生成炫纹数量(PVE|PVP)
    /// </summary>
    private int[] z_ChaserMaxCount = new int[2];

    /// <summary>
    /// 判定生成大炫纹的连击数(达到combo技能命中生成大炫纹|自动炫纹（公式：A+B*N,填写A|B）)
    /// </summary>
    private int[] z_ComboCheckCountArr = new int[3];

    /// <summary>
    /// 自动炫纹的生成概率(生成超大炫纹的概率|攻击生成额外炫纹的概率|combo生成额外炫纹概率,千分比)
    /// </summary>
    private int[] z_GenerateRateArr = new int[3];
    
    /// <summary>
    /// 暂存装备数据
    /// </summary>
    private int[] z_GenerateRateArr_Equip = new int[3];
    
    /// <summary>
    /// 暂存装备数据-炫纹的生成间隔加成
    /// </summary>
    private int z_ChaserCreateTime_Equip = 0;

    /// <summary>
    /// 自动炫纹生成炫纹时间间隔
    /// </summary>
    private int z_GenerateTimeAcc = 0;
    
    /// <summary>
    /// 炫纹存活时间
    /// </summary>
    private int z_ChaserLiveTime = 0;
    
    /// <summary>
    /// 炫纹缓动系数
    /// </summary>
    private VFactor z_ChaserTweenFactor;

    /// <summary>
    /// 连击数以上生成的所有炫纹的大小
    /// </summary>
    private List<Mechanism2089.ComboScale> m_ComboScaleList = new List<Mechanism2089.ComboScale>();

    /// <summary>
    /// 达到combo后使用m_ComboChaserScale(用于装备配置)
    /// </summary>
    //private int z_ComboScaleCount = 20;

    /// <summary>
    /// 触发生成炫纹的hit
    /// </summary>
    private HashSet<int> z_FilterHitSet = new HashSet<int>();
    /// <summary>
    /// 改变炫纹的大小
    /// </summary>
    private VFactor[] z_ChaserScale = new VFactor[(int)Mechanism2072.ChaserType.Max];
    /// <summary>
    /// 炫纹数据队列
    /// </summary>
    private Queue<ChaserData> m_ChaserQueue;
    private int m_ChaserMaxCount;
    private bool m_AutoChaserFlag = false;
    private ChaserType m_LastAddType = ChaserType.None;
    private int m_CurTimeAcc = 0;
    private int m_CurChaserCount = 0;
    private List<IBeEventHandle> mHandleList = null;
    private BeEvent.BeEventHandleNew mEquipHandle;

    /// <summary>
    /// 监测的触发效果ID与炫纹类型字典
    /// </summary>
    private Dictionary<int,ChaserType> m_HurtIdDic;
    private HashSet<int> mSkillMarkSet;

    private int mAutoChaserLevel = 0;
    private bool mIsTweenChaser = true; // 炫纹是否用更随效果，解决过门飞回问题
    public override void OnInit()
    {
        base.OnInit();
        z_ChaserMaxCount[0] = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        z_ChaserMaxCount[1] = TableManager.GetValueFromUnionCell(data.ValueA[1], level);

        z_ComboCheckCountArr[0] = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        z_ComboCheckCountArr[1] = TableManager.GetValueFromUnionCell(data.ValueB[1], level);
        z_ComboCheckCountArr[2] = TableManager.GetValueFromUnionCell(data.ValueB[2], level);
        
        z_ChaserLiveTime = TableManager.GetValueFromUnionCell(data.ValueE[0], level);
        
        z_ChaserTweenFactor = VFactor.NewVFactor(TableManager.GetValueFromUnionCell(data.ValueF[0], level), GlobalLogic.VALUE_1000);
        
        for (int i = 0; i < data.ValueG.Count; i++)
        {
            z_FilterHitSet.Add(TableManager.GetValueFromUnionCell(data.ValueG[i], level));
        }
        
        mHandleList = GamePool.ListPool<IBeEventHandle>.Get();
        mHandleList.Clear();
        
        mIsTweenChaser = true;
    }

    public override void OnReset()
    {
        z_ChaserMaxCount = new int[2];
        z_ComboCheckCountArr = new int[3];
        z_GenerateRateArr = new int[3];
        z_GenerateRateArr_Equip = new int[3];
        z_ChaserCreateTime_Equip = 0;
        z_GenerateTimeAcc = 0;
        z_ChaserLiveTime = 0;
        z_ChaserTweenFactor = VFactor.one;
        m_ComboScaleList.Clear();
        z_FilterHitSet.Clear();
        z_ChaserScale = new VFactor[(int)Mechanism2072.ChaserType.Max];
        m_ChaserQueue.Clear();
        m_ChaserMaxCount = 0;
        m_AutoChaserFlag = false;
        m_LastAddType = ChaserType.None;
        m_CurTimeAcc = 0;
        m_CurChaserCount = 0;
        mHandleList.Clear();
        if (mEquipHandle != null)
        {
            mEquipHandle.Remove();
            mEquipHandle = null;
        }
        m_HurtIdDic.Clear();
        mSkillMarkSet.Clear();
        mAutoChaserLevel = 0;
        mIsTweenChaser = true; 
}

    /// <summary>
    /// 用于消耗炫纹添加buff用
    /// </summary>
    /// <param name="list">炫纹列表</param>
    /// <param name="battleType">战斗类型</param>
    /// <param name="owner">主体</param>
    /// <param name="level">技能等级</param>
    public static void AddBuff(List<ChaserData> list, BattleType battleType, BeActor owner, int level)
    {
        int[] buffList;
        if (BattleMain.IsModePvP(battleType))
        {
            int[] buffInfoIDList = {230201, 230211, 230241, 230231, 230221};
            buffList = buffInfoIDList;
        }
        else
        {
            int[] buffInfoIDList = {230200, 230210, 230240, 230230, 230220};
            buffList = buffInfoIDList;
        }

        for (int i = 0; i < list.Count; i++)
        {
            var type = (int)list[i].type;
            if (type < buffList.Length)
                owner.buffController.TryAddBuffInfo(buffList[type], owner, level);
        }
    }
    
    private void OnLevelSwitchStart(BeEvent.BeEventParam args)
    {
        mIsTweenChaser = false;
    }
    
    private void OnLevelSwitchEnd(BeEvent.BeEventParam args)
    {
        mIsTweenChaser = true;
    }

    protected void OnChangeWeapon(BeEvent.BeEventParam param)
    {
        UpdateEquipData();
    }

    protected void OnChangeEquip(BeEvent.BeEventParam param)
    {
        UpdateEquipData();
    }
    
    private void UpdateEquipData()
    {
        m_ChaserMaxCount = BattleMain.IsModePvP(battleType) ? z_ChaserMaxCount[1] : z_ChaserMaxCount[0];
        z_GenerateRateArr_Equip[0] = 0;
        z_GenerateRateArr_Equip[1] = 0;
        z_GenerateRateArr_Equip[2] = 0;
        m_ComboScaleList.Clear();
        for (int i = 0; i < z_ChaserScale.Length; i++)
        {
            z_ChaserScale[i] = VFactor.zero;
        }
        
        List<BeMechanism> mechanismList = owner.MechanismList;
        if (mechanismList == null)
            return;
        for (int i = 0; i < mechanismList.Count; i++)
        {
            if(!mechanismList[i].isRunning)
                continue;
            
            var mechanism = mechanismList[i] as Mechanism2089;
            if (mechanism == null)
                continue;

            m_ChaserMaxCount += mechanism.ChaserMaxCount;
            z_GenerateRateArr_Equip[0] += mechanism.BigChaserCreateProb;
            z_GenerateRateArr_Equip[1] += mechanism.ChaserCreateProb;
            z_GenerateRateArr_Equip[2] += mechanism.ChaserCreateProb;
            m_ComboScaleList.Add(mechanism.ComboChaserScale);
            for (int j = 0; j < z_ChaserScale.Length; j++)
            {
                z_ChaserScale[j] += VFactor.NewVFactor(mechanism.ChaserScale(j), GlobalLogic.VALUE_1000);
            }
        }
    }

    public void OffsetChaserCreateTime(int time)
    {
        z_ChaserCreateTime_Equip += time;
    }

    public override void OnStart()
    {
        base.OnStart();
        InitData();
    }

    public override void OnUpdate(int deltaTime)
    {
        base.OnUpdate(deltaTime);
        UpdateAddChaser(deltaTime);
        UpdateChaserLive(deltaTime);
        UpdateChaserPosition(deltaTime);
    }

    public override void OnUpdateGraphic(int deltaTime)
    {
#if !LOGIC_SERVER
        UpdateChaserGraphicPos(deltaTime);
#endif
    }

    public override void OnFinish()
    {
        base.OnFinish();
        ClearData();
        RemoveHandle();
/*#if !LOGIC_SERVER
        DestroyHole();
#endif*/
    }

    private void RemoveHandle()
    {
        if (mHandleList != null)
        {
            for (int i = 0; i < mHandleList.Count; i++)
            {
                IBeEventHandle handle = mHandleList[i];
                if (handle != null)
                {
                    handle.Remove();
                    handle = null;
                }
            }
            mHandleList.Clear();
            GamePool.ListPool<IBeEventHandle>.Release(mHandleList);   
            mHandleList = null;
        }

        if (mEquipHandle != null)
        {
            mEquipHandle.Remove();
            mEquipHandle = null;
        }
    }

    /// <summary>
    /// 初始化相关数据
    /// </summary>
    private void InitData()
    {
        m_ChaserQueue = new Queue<ChaserData>();
        m_HurtIdDic = new Dictionary<int, ChaserType>();
        mSkillMarkSet = new HashSet<int>();

        UpdateEquipData();
        m_LastAddType = ChaserType.None;
        RegiserEvent();
        m_CurChaserCount = 0;
        ///触发效果ID列表与对应的炫纹类型关系字典  代码中写死
        m_HurtIdDic.Add(23180, ChaserType.None); //无
        m_HurtIdDic.Add(20100, ChaserType.Light); //光/天击
        m_HurtIdDic.Add(23010, ChaserType.Dark); //暗/圆舞棍
        m_HurtIdDic.Add(20080, ChaserType.Ice); //冰/龙牙
        m_HurtIdDic.Add(20090, ChaserType.Fire); //火/落花掌
        m_HurtIdDic.Add(20091, ChaserType.Fire); //火/落花掌蓄力
#if !LOGIC_SERVER
        InitEffectData();
        //InitHole();
#endif
    }

    /// <summary>
    /// 清除数据
    /// </summary>
    private void ClearData()
    {
        mSkillMarkSet.Clear();
        m_ChaserQueue.Clear();
        m_ChaserQueue = null;
        m_HurtIdDic.Clear();
        m_HurtIdDic = null;
    }

    /// <summary>
    /// 监听事件
    /// </summary>
    private void RegiserEvent()
    {
        if(owner == null)
            return;
        
        if (mHandleList == null)
        {
            mHandleList = GamePool.ListPool<IBeEventHandle>.Get();
        }
        //因为自动炫纹
        handleA = OwnerRegisterEventNew(BeEventType.onHitOther, RegisterSkillHitOther);
        handleB = OwnerRegisterEventNew(BeEventType.onHitOther, RegisterHitOther);
        //mHandleList.Add(owner.RegisterEvent(BeEventType.onHitOther, RegisterSkillHitOther));
        //mHandleList.Add(owner.RegisterEvent(BeEventType.onHitOther, RegisterHitOther));
        mHandleList.Add(owner.RegisterEventNew(BeEventType.onBattleCombo, RegisterComboAddChaser));
        
        mHandleList.Add(owner.RegisterEventNew(BeEventType.onCastSkillFinish, RegisterSkillComplete));
        mHandleList.Add(owner.RegisterEventNew(BeEventType.onSkillCancel, RegisterSkillComplete));
        
        mHandleList.Add(owner.RegisterEventNew(BeEventType.OnChangeWeaponEnd, OnChangeWeapon));

        mEquipHandle = owner.RegisterEventNew(BeEventType.onChangeEquipEnd, OnChangeEquip);
        
        mHandleList.Add(owner.RegisterEventNew(BeEventType.onAfterDead, OnDead));
        // 切门时，主角位置瞬变，炫纹会出现飞回来的效果,需要重新刷新位置
        mHandleList.Add(owner.RegisterEventNew(BeEventType.onStartPassDoor, OnLevelSwitchStart));
        mHandleList.Add(owner.RegisterEventNew(BeEventType.onPassedDoor, OnLevelSwitchEnd));
       // mHandleList.Add(owner.RegisterEvent(BeEventType.onDeadTowerEnterNextLayer, OnLevelSwitch));
    }

    private void OnDead(BeEvent.BeEventParam param)
    {
        // 死亡重置数据
        mSkillMarkSet.Clear();
        m_ChaserQueue.Clear();
        m_CurTimeAcc = 0;
        m_CurChaserCount = 0;
        m_AutoChaserFlag = false;
        m_LastAddType = ChaserType.None;
        mIsTweenChaser = true;
    }

    /// <summary>
    /// 监听指定技能命中(生成炫纹)
    /// </summary>
    private void RegisterSkillHitOther(BeEvent.BeEventParam param)
    {
        int skillId = param.m_Int2;
        if(CheckSkillMark(skillId))
            return;
        
        int hurtId = param.m_Int;
        if (!m_HurtIdDic.ContainsKey(hurtId))
        {
            return;
        }
        
        ChaserType type = m_HurtIdDic[hurtId];

        // 开关控制
        if(owner.IsChaserSwitch((int) type))
            return;
        
        MarkSkill(skillId, true);
        
        ChaseSizeType sizeType = ChaseSizeType.Normal;

        if(owner == null || owner.actorData == null)
            return;
        
        int curComboCount = owner.actorData.GetCurComboCount();
        if (curComboCount >= z_ComboCheckCountArr[0])
        {
            sizeType = ChaseSizeType.Big;
        }
        AddChaser(type, sizeType);
    }

    /// <summary>
    /// 监听攻击命中(学习了自动炫纹以后生效)
    /// </summary>
    /// <param name="args"></param>
    private void RegisterHitOther(BeEvent.BeEventParam param)
    {
        if (!m_AutoChaserFlag)
        {
            return;
        }
        
        // 特定hurt才能生成炫纹
        int hurtId = param.m_Int;
        if (!z_FilterHitSet.Contains(hurtId))
            return;
        
        int skillId = param.m_Int2;
        if(CheckSkillMark(skillId))
            return;
        MarkSkill(skillId, true);
        
        RateAddChaser(z_GenerateRateArr[1]);
    }

    private void RegisterSkillComplete(BeEvent.BeEventParam args)
    {
        int skillId = args.m_Int;
        MarkSkill(skillId, false);
    }

    private void MarkSkill(int skillId, bool mark)
    {
        if (mark)
        {
            if (!mSkillMarkSet.Contains(skillId))
            {
                mSkillMarkSet.Add(skillId);
            }
        }
        else
        {
            if (mSkillMarkSet.Contains(skillId))
            {
                mSkillMarkSet.Remove(skillId);
            }
        }
    }

    private bool CheckSkillMark(int skillId)
    {
        return mSkillMarkSet.Contains(skillId);
    }
    
    /// <summary>
    /// 炫纹跟随
    /// </summary>
    private void UpdateChaserPosition(int deltaTime)
    {
        var enumerator = m_ChaserQueue.GetEnumerator();
        while (enumerator.MoveNext())
        {
            var chaser = enumerator.Current;
            if (mIsTweenChaser)
            {
                chaser.position = VInt3.Lerp(chaser.position, GetAnchorWorldPos(chaser.anchorPos), z_ChaserTweenFactor);
            }
            else
            {
                chaser.position = GetAnchorWorldPos(chaser.anchorPos);
            }
        }
        enumerator.Dispose();
    }

    /// <summary>
    /// 刷新炫纹生存时间
    /// </summary>
    /// <param name="deltaTime"></param>
    private void UpdateChaserLive(int deltaTime)
    {
        var enumerator = m_ChaserQueue.GetEnumerator();
        while (enumerator.MoveNext())
        {
            var chaser = enumerator.Current;
            chaser.liveTime += deltaTime;
        }
        enumerator.Dispose();
        
        int count = 0;
        do
        {
            if (m_ChaserQueue.Count <= 0)
                break;
            
            ChaserData lastData = m_ChaserQueue.Peek();
            if (lastData != null && lastData.liveTime >= z_ChaserLiveTime)
            {
                m_ChaserQueue.Dequeue();
                count++;
#if !LOGIC_SERVER
                if (lastData.effect != null && owner != null && owner.m_pkGeActor != null)
                {
                    owner.m_pkGeActor.DestroyEffect(lastData.effect);
                }
#endif
                //Logger.LogProcessFormat("炫纹死亡:炫纹时长{0},策划时长:{1}", lastData.liveTime, z_ChaserLiveTime);
            }
            else
            {
                break;
            }
        } while (true);

        if (count > 0)
        {
            ChangeChaserEffectPos();
            _ReduceChaserCount(count);    
        }
    }
    
    /// <summary>
    /// 发射炫纹以后重新调整所有炫纹的位置
    /// </summary>
    private void ChangeChaserEffectPos()
    {
        int index = 0;
        var enumerator = m_ChaserQueue.GetEnumerator();
        while (enumerator.MoveNext())
        {
            var data = enumerator.Current;
            if (index < m_ChaserPosArrEffect.Length)
            {
                data.anchorPos = new VInt3(m_ChaserPosArrEffect[index]);
            }
            index++;
        }
        enumerator.Dispose();
    }
    
    /// <summary>
    /// 每隔一段时间生成炫纹
    /// </summary>
    /// <param name="deltaTime">更新的频率</param>
    private void UpdateAddChaser(int deltaTime)
    {
        if (!m_AutoChaserFlag)
        {
            return;
        }

        int finalCreateTime = z_GenerateTimeAcc + z_ChaserCreateTime_Equip;
        
        //炫纹生成间隔保护，最少1秒
        if (finalCreateTime <= 1000)
        {
            finalCreateTime = 1000;
        }
        
        if (m_CurTimeAcc < finalCreateTime)
        {
            m_CurTimeAcc += deltaTime;
            return;
        }

        m_CurTimeAcc -= finalCreateTime;

        ChaserType type = GetChaserType();
        if (type == ChaserType.Max)
            return;
        
        ChaseSizeType sizeType = ChaseSizeType.Normal;

        int random = FrameRandom.Range1000();
        if (random <= z_GenerateRateArr[0])
        {
            sizeType = ChaseSizeType.Big;
        }

        AddChaser(type, sizeType);
    }

    /// <summary>
    /// 连击生成炫纹
    /// </summary>
    private void RegisterComboAddChaser(BeEvent.BeEventParam args)
    {
        if (!m_AutoChaserFlag)
        {
            return;
        }
        if(owner == null || owner.actorData == null)
            return;
        
        int curComboCount = owner.actorData.GetCurComboCount();
        if ((curComboCount - z_ComboCheckCountArr[1]) % z_ComboCheckCountArr[2] != 0)
        {
            //Logger.LogProcessFormat("当前连接数不满足自动炫纹条件:当前连击数{0},策划配置连击数:{1}", curComboCount, z_ComboCheckCountArr[1]);
            return;
        }
        RateAddChaser(z_GenerateRateArr[2]);
    }

    /// <summary>
    /// 自动炫纹时 概率生成炫纹
    /// </summary>
    private void RateAddChaser(int rate)
    {
        int random = FrameRandom.Range1000();
        if (random > rate)
        {
            Logger.LogProcessFormat("概率不满足，不生成炫纹:随机概率:{0},策划配置概率:{1}", random, rate);
            return;
        }

        ChaserType type = GetChaserType();
        if (type == ChaserType.Max)
            return;
        
        ChaseSizeType sizeType = ChaseSizeType.Normal;
        AddChaser(type, sizeType);
    }

    /// <summary>
    /// 给外部增加炫纹专用
    /// 如机制2092
    /// </summary>
    /// <param name="type"></param>
    /// <param name="sizeType"></param>
    /// <param name="isExtra"></param>
    public void AddChaserByExternal(ChaserType type, ChaseSizeType sizeType, bool isExtra = false)
    {
        if (owner.IsChaserSwitch((int) type))
        {
            type = GetPriorityChaserType();
        }

        if(type == ChaserType.Max)
            return;
        
        AddChaser(type, sizeType, isExtra);
    }
    
    /// <summary>
    /// 增加炫纹
    /// 炫纹共有四种产生途径,1:指定技能命中目标后生成,2:开启自动炫纹以后,每隔一段时间产生一个.3:开启自动炫纹以后，攻击命中有概率产生.4:开启自动炫纹以后，连击达到一定值有概率产生
    /// </summary>
    /// <param name="type">炫纹类型</param>
    /// <param name="sizeType">炫纹大小</param>
    /// <param name="isExtra">用于机制2092,额外炫纹不会改变最后炫纹种类</param>
    public void AddChaser(ChaserType type, ChaseSizeType sizeType, bool isExtra = false)
    {
        if (m_CurChaserCount >= m_ChaserMaxCount)
        {
            //Logger.LogProcessFormat("当前生成炫纹数量已达最大值:{0}", m_ChaserMaxCount);
            return;
        }
        
        //var eventParam = DataStructPool.EventParamPool.Get();
        //eventParam.m_Int = (int)type;
        //eventParam.m_Int2 = (int)sizeType;
        //owner.TriggerEventNew(BeEventType.onAddChaser, eventParam);
        //type = (ChaserType) eventParam.m_Int;
        //sizeType = (ChaseSizeType) eventParam.m_Int2;
        //DataStructPool.EventParamPool.Release(eventParam);

        var param = owner.TriggerEventNew(BeEventType.onAddChaser, new EventParam(){ m_Int = (int)type, m_Int2 = (int)sizeType });
        type = (ChaserType)param.m_Int;
        sizeType = (ChaseSizeType)param.m_Int2;

        //Logger.LogProcessFormat("生成炫纹,Type:{0},sizeType:{1}", type, sizeType);
        if (!isExtra)
        {
            m_LastAddType = type;
        }
        ChaserData data = new ChaserData();
        data.type = type;
        data.sizeType = sizeType;
        data.liveTime = 0;
        data.scale = GetChaserScale(type);
        int addIndex = m_ChaserQueue.Count;
        data.anchorPos = new VInt3(m_ChaserPosArrEffect[addIndex]);
        data.position = GetAnchorWorldPos(data.anchorPos);
#if !LOGIC_SERVER
        if (NeedCreateEffect())
        {
            var effect = AddChaserEffect(data, addIndex);
            data.effect = effect;
            if (effect != null && m_IsChaserVisable == false)
            {
                data.effect.SetVisible(m_IsChaserVisable);    
            }
        }
#endif
        m_CurChaserCount++;
        m_ChaserQueue.Enqueue(data);
    }

    /// <summary>
    /// 计算炫纹大小
    /// combo大小系数，装备指定属性炫纹大小系数
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private VFactor GetChaserScale(ChaserType type)
    {
        var scale = VFactor.zero;
        if (owner != null && owner.actorData != null)
        {
            // 达到一定combo，放大炫纹系数，加法叠加
            int curCombo = owner.actorData.GetCurComboCount();
            for (int i = 0; i < m_ComboScaleList.Count; i++)
            {
                Mechanism2089.ComboScale comboScale = m_ComboScaleList[i];
                if (curCombo >= comboScale.combo)
                {
                    scale += comboScale.scale;
                }
            }
        }

        var scaleFactor = VFactor.zero;
        if ((int) type < z_ChaserScale.Length)
        {
            scaleFactor = z_ChaserScale[(int)type];
        }

        scale += scaleFactor;
        return scale;
    }

    private VInt3 GetAnchorWorldPos(VInt3 pos)
    {
        if(owner == null)
            return VInt3.zero;
        
        pos.x = owner.GetFace() ? pos.x * -1 : pos.x;
        return owner.GetPosition() + pos;
    }
    
    /// <summary>
    /// 发射炫纹
    /// </summary>
    /// <param name="launchCount">发射炫纹数量</param>
    /// <param name="list">发射炫纹列表,用于返回列表数据</param>
    public void LaunchChaser(int launchCount, List<ChaserData> list)
    {
        if (list == null)
        {
            Logger.LogProcessFormat("list is null");
            return;
        }
        list.Clear();
        int curChaserCount = m_ChaserQueue.Count;

        for (int i = 0; i < curChaserCount; i++)
        {
            if (i >= launchCount)
            {
                break;
            }
            var data = m_ChaserQueue.Dequeue();
            list.Add(data);
            Logger.LogProcessFormat("发射炫纹类型:{0},剩余数量:{1}", data.type, m_ChaserQueue.Count);
        }
        ChangeChaserEffectPos();
    }

    /// <summary>
    /// 生成一个炫纹数据
    /// </summary>
    /// <returns>返回生成的炫纹数据</returns>
    private ChaserType GetChaserType()
    {
        if (m_LastAddType != ChaserType.None)
        {
            return m_LastAddType;
        }
        // 炫纹开关，优先级顺序:无 暗 火 光 冰
        return GetPriorityChaserType();
    }

    /// <summary>
    /// 得到按优先级顺序的备选炫纹类型
    /// 都关闭为max
    /// </summary>
    /// <returns></returns>
    private ChaserType GetPriorityChaserType()
    {
        ChaserType[] typeList = {ChaserType.None, ChaserType.Dark, ChaserType.Fire, ChaserType.Light, ChaserType.Ice};
        for (int i = 0; i < typeList.Length; i++)
        {
            if (!owner.IsChaserSwitch((int) typeList[i]))
            {
                return typeList[i];
            }
        }
        return ChaserType.Max;
    }

    /// <summary>
    /// 返回当前炫纹的数量
    /// </summary>
    /// <returns>当前炫纹的数量</returns>
    public int GetChaserCount()
    {
        if (m_ChaserQueue == null)
            return 0;
        
        return m_ChaserQueue.Count;
    }

    /// <summary>
    /// 设置自动炫纹开关
    /// </summary>
    /// <param name="flag">true为开 false为关</param>
    public void SetAutoChaserFlag(bool flag, int level)
    {
        m_AutoChaserFlag = flag;

        if (m_AutoChaserFlag)
        {
            mAutoChaserLevel = level;
            UpdateAutoChaserData();
            //开启自动炫纹后立即生成炫纹
            m_CurTimeAcc = z_GenerateTimeAcc + z_ChaserCreateTime_Equip;
        }
    }

    /// <summary>
    /// Buff型技能，只在添加buff时确定配置参数，在buffing切换装备，不会改变buff的配置参数
    /// </summary>
    public void UpdateAutoChaserData()
    {
        z_GenerateTimeAcc = TableManager.GetValueFromUnionCell(data.ValueD[0], mAutoChaserLevel);

        if (BattleMain.IsModePvP(battleType))
        {
            z_GenerateRateArr[0] = TableManager.GetValueFromUnionCell(data.ValueH[0], mAutoChaserLevel) + z_GenerateRateArr_Equip[0];
            z_GenerateRateArr[1] = TableManager.GetValueFromUnionCell(data.ValueH[1], mAutoChaserLevel) + z_GenerateRateArr_Equip[1];
            z_GenerateRateArr[2] = TableManager.GetValueFromUnionCell(data.ValueH[2], mAutoChaserLevel) + z_GenerateRateArr_Equip[2];
        }
        else
        {
            z_GenerateRateArr[0] = TableManager.GetValueFromUnionCell(data.ValueC[0], mAutoChaserLevel) + z_GenerateRateArr_Equip[0];
            z_GenerateRateArr[1] = TableManager.GetValueFromUnionCell(data.ValueC[1], mAutoChaserLevel) + z_GenerateRateArr_Equip[1];
            z_GenerateRateArr[2] = TableManager.GetValueFromUnionCell(data.ValueC[2], mAutoChaserLevel) + z_GenerateRateArr_Equip[2];
        }
    }

    
    /// <summary>
    /// 改变所有炫纹大小，不包含发射中的炫纹
    /// </summary>
    /// <param name="size"></param>
    public void SetChaseSize(ChaseSizeType size)
    {
        var enumerator = m_ChaserQueue.GetEnumerator();
        while (enumerator.MoveNext())
        {
            var chaser = enumerator.Current;
            chaser.sizeType = size;
#if !LOGIC_SERVER
            SetEffectScale(chaser, chaser.effect);
#endif
        }
        enumerator.Dispose();
    }
    

    /// <summary>
    /// 发射的炫纹实体死亡后 减少当前炫纹数量 用于控制场上最多只能存在指定个数的炫纹
    /// </summary>
    /// <param name="count">减少的炫纹数量</param>
    private void _ReduceChaserCount(int count)
    {
        if (m_CurChaserCount < count)
        {
            Logger.LogProcessFormat("当前炫纹数量小于减少数量,当前数量:{0},减少数量：{1}", m_CurChaserCount, count);
            m_CurChaserCount = 0;
            return;
        }
        
        m_CurChaserCount -= count;
    }

    
    /// <summary>
    /// 主动消耗炫纹，会发消耗炫纹通知
    /// 炫纹自然消亡不会走这里
    /// </summary>
    /// <param name="count"></param>
    public void ReduceChaserCount(int count)
    {
        //var eventParam = DataStructPool.EventParamPool.Get();
        //eventParam.m_Int = count;
        //owner.TriggerEventNew(BeEventType.onRemoveChaser, eventParam);
        //DataStructPool.EventParamPool.Release(eventParam);

        owner.TriggerEventNew(BeEventType.onRemoveChaser, new EventParam(){ m_Int  = count});
        _ReduceChaserCount(count);
    }
    
    ///炫纹表现相关
#if !LOGIC_SERVER
    /// <summary>
    /// 炫纹特效的偏移位置数组
    /// </summary>
   
    private string[] m_ChaserPathArr = new string[5];

    private bool m_IsChaserVisable = true;

/*    private string m_ChaserHolePath = "Effects/Hero_Lifa/Eff_Lifa_XWRH/Prefab/Eff_Lifa_XWRH_guanghuan";
    private GeEffectEx m_HoleEffect = null;*/

    /// <summary>
    /// 初始化表现相关的数据
    /// </summary>
    private void InitEffectData()
    {
        // 需要同步修改机制2092
        m_ChaserPathArr[0] = "Effects/Hero_Lifa/Eff_Lifa_XWRH/Prefab/Eff_Lifa_XWRH_wu";
        m_ChaserPathArr[1] = "Effects/Hero_Lifa/Eff_Lifa_XWRH/Prefab/Eff_Lifa_XWRH_guang";
        m_ChaserPathArr[2] = "Effects/Hero_Lifa/Eff_Lifa_XWRH/Prefab/Eff_Lifa_XWRH_fire";
        m_ChaserPathArr[3] = "Effects/Hero_Lifa/Eff_Lifa_XWRH/Prefab/Eff_Lifa_XWRH_ice";
        m_ChaserPathArr[4] = "Effects/Hero_Lifa/Eff_Lifa_XWRH/Prefab/Eff_Lifa_XWRH_an";

        if (owner != null && owner.CurrentBeScene != null)
        {
            mHandleList.Add(owner.CurrentBeScene.RegisterEventNew(BeEventSceneType.OnStartAirBattle, OnStartAirBattle)); 
            mHandleList.Add(owner.CurrentBeScene.RegisterEventNew(BeEventSceneType.OnEndAirBattle, OnEndAirBattle)); 
        }
    }

    private void OnStartAirBattle(BeEvent.BeEventParam args)
    {
        SetChaserVisable(false);
    }
    
    private void OnEndAirBattle(BeEvent.BeEventParam args)
    {
        SetChaserVisable(true);
    }

/*    private void InitHole()
    {
        if(owner == null)
            return;
        if(owner.m_pkGeActor == null)
            return;
        
        m_HoleEffect = owner.m_pkGeActor.CreateEffect(m_ChaserHolePath, "[actor]Orign", 999999999, Vec3.zero, 1, 1, true,false, EffectTimeType.BUFF);
        if (m_HoleEffect != null)
        {
            m_HoleEffect.SetLocalPosition(new Vector3(-0.2f, 1f, 0));
        }
    }*/

/*    private void DestroyHole()
    {
        if (m_HoleEffect != null && owner != null && owner.m_pkGeActor != null)
        {
            owner.m_pkGeActor.DestroyEffect(m_HoleEffect);
        }
    }*/
    

    /// <summary>
    /// 炫纹跟随(渲染)
    /// </summary>
    private void UpdateChaserGraphicPos(int deltaTime)
    {
        var enumerator = m_ChaserQueue.GetEnumerator();
        while (enumerator.MoveNext())
        {
            var chaser = enumerator.Current;
            if (chaser != null && chaser.effect != null)
            {
                chaser.effect.SetPosition(chaser.position.vector3);
            }
        }
        enumerator.Dispose();
    }
    
    /// <summary>
    /// 创建炫纹表现相关
    /// </summary>
    /// <param name="data">新增炫纹数据</param>
    /// <param name="index">所在队列的序号</param>
    private GeEffectEx AddChaserEffect(ChaserData data, int index)
    {
        if (index >= m_ChaserPosArrEffect.Length)
        {
            Logger.LogProcessFormat("炫纹所在序号错误，超过最大限制,当前序号:{0},最大序号:{1}", index, m_ChaserPosArrEffect.Length);
            return null;
        }

        if (owner == null || owner.m_pkGeActor == null)
            return null;
        
        string path = GetChaserPath(data.type);
        GeEffectEx effect = owner.m_pkGeActor.CreateEffect(path, "[actor]Orign", 999999999, Vec3.zero, 1, 1, true,false, EffectTimeType.BUFF);
        if (effect != null)
        {
            Battle.GeUtility.AttachTo(effect.GetRootNode(), owner.m_pkGeActor.GetEntityNode(GeEntity.GeEntityNodeType.Root));
            effect.SetPosition(data.position.vector3);
            SetEffectScale(data, effect);
            return effect;
        }

        return null;
    }

    /// <summary>
    /// 获取炫纹对应的特效路径
    /// </summary>
    /// <param name="type">炫纹类型</param>
    /// <param name="sizeType">炫纹大小类型</param>
    /// <returns>返回炫纹特效路径</returns>
    private string GetChaserPath(ChaserType type)
    {
        string path = m_ChaserPathArr[0];
        switch (type)
        {
            case ChaserType.Light:
                path = m_ChaserPathArr[1];
                break;
            case ChaserType.Fire:
                path = m_ChaserPathArr[2];
                break;
            case ChaserType.Ice:
                path = m_ChaserPathArr[3];
                break;
            case ChaserType.Dark:
                path = m_ChaserPathArr[4];
                break;
        }
        return path;
    }

    /// <summary>
    /// 设置炫纹大小
    /// </summary>
    private void SetEffectScale(ChaserData data, GeEffectEx effect)
    {
        if (effect == null)
        {
            Logger.LogProcessFormat("创建出来的炫纹特效是空的");
            return;
        }

        float scale = 1;
        switch (data.sizeType)
        {
            case ChaseSizeType.Big:
                scale = 1.3f;
                break;
            /*case ChaseSizeType.VeryBig:
                scale = 2f;
                break;*/
        }

        scale += data.scale.single;
        effect.SetScale(scale, scale, scale);
    }

    private void SetChaserVisable(bool visable)
    {
        m_IsChaserVisable = visable;
        var enumerator = m_ChaserQueue.GetEnumerator();
        while (enumerator.MoveNext())
        {
            var chaser = enumerator.Current;
            if (chaser.effect != null)
            {
                chaser.effect.SetVisible(visable);
            }
        }
        enumerator.Dispose();
    }
	

    /// <summary>
    /// 检查是否需要创建炫纹特效
    /// </summary>
    /// <returns></returns>
    protected bool NeedCreateEffect()
    {
        if (BattleMain.IsModePvP(battleType))
            return true;
        if (owner.isLocalActor)
            return true;
        if (SettingManager.instance == null)
            return true;
        return SettingManager.instance.GetCommmonSet(SettingManager.STR_SKILLEFFECTDISPLAY) == SettingManager.SetCommonType.Open;
	}
#endif
}