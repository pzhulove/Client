using System;
using System.Collections.Generic;
using GameClient;
using UnityEngine;

//爆裂玫瑰（nvdaqiang）被动觉醒
public class Mechanism2071 : BeMechanism
{
    private List<int> calcuSkillIdList = new List<int>();
    private List<int> gainSkillIdList = new List<int>();
    private Dictionary<int, int> skillIdBuffInfoIdDic = new Dictionary<int, int>();
    private Dictionary<int, int> skillIdBuffIdDic = new Dictionary<int, int>();
    private int calcuNum;//累计次数，达到则增伤
    private int overLoadBuffId;//增伤时用于显示的特效buff

    public Mechanism2071(int mid, int lv) : base(mid, lv) { }

    public override void OnInit()
    {
        //ValueA
        if (calcuSkillIdList == null) 
        {
            calcuSkillIdList = new List<int>();
        }
        else
        {
            calcuSkillIdList.Clear();
        }
        for(int i = 0; i < data.ValueA.Count; ++i)
        {
            calcuSkillIdList.Add(TableManager.GetValueFromUnionCell(data.ValueA[i], level));
        }
        //ValueB
        if (gainSkillIdList == null)
        {
            gainSkillIdList = new List<int>();
        }
        else
        {
            gainSkillIdList.Clear();
        }
        for (int i = 0; i < data.ValueB.Count; ++i)
        {
            gainSkillIdList.Add(TableManager.GetValueFromUnionCell(data.ValueB[i], level));
        }
        calcuNum = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
        overLoadBuffId = TableManager.GetValueFromUnionCell(data.ValueD[0], level);

        skillIdBuffInfoIdDic.Clear();
        int min = Math.Min(data.ValueB.Count, data.ValueE.Count);
        for(int i = 0; i < min; ++i)
        {
            skillIdBuffInfoIdDic.Add(gainSkillIdList[i], TableManager.GetValueFromUnionCell(data.ValueE[i], level));
        }

        skillIdBuffIdDic.Clear();
        min = Math.Min(data.ValueB.Count, data.ValueF.Count);
        for (int i = 0; i < min; ++i)
        {
            skillIdBuffIdDic.Add(gainSkillIdList[i], TableManager.GetValueFromUnionCell(data.ValueF[i], level));
        }
    }

    public override void OnReset()
    {
        InitUITimer = 0;
        useSkillTimes = 0;
        addDamageSkillId.Clear();
        if(useSkillHandle != null)
        {
            useSkillHandle.Remove();
            useSkillHandle = null;
        }
        unInitUI = true;
    }

    public override void OnStart()
    {
        useSkillTimes = 0;
        addDamageSkillId.Clear();

        useSkillHandle = owner.RegisterEventNew(BeEventType.onCastSkill, OnCastSkillEvent);

        InitNvDaQiangEnergyBar(calcuNum);
    }

    private const int InitUIMaxTime = 1000;
    private int InitUITimer;

    public override void OnUpdate(int deltaTime)
    {
        if (unInitUI)
        {
            InitUITimer += deltaTime;
            if (InitUITimer >= InitUIMaxTime)
            {
                InitUITimer = 0;
                InitNvDaQiangEnergyBar(calcuNum);
            }
        }
    }

    public override void OnFinish()
    {
        useSkillTimes = 0;
        addDamageSkillId.Clear();

        if(useSkillHandle != null)
        {
            useSkillHandle.Remove();
            useSkillHandle = null;
        }
    }


    int useSkillTimes = 0;
    List<int> addDamageSkillId = new List<int>();

    IBeEventHandle useSkillHandle;
    
    private void OnCastSkillEvent(BeEvent.BeEventParam args)
    {
        if(args != null)
        {
            var skillID = args.m_Int;
            if(addDamageSkillId.Contains(skillID))
            {
                //再一次释放增伤技能 取消这个增伤buff
                if (skillIdBuffIdDic.ContainsKey(skillID))
                {
                    owner.buffController.RemoveBuff(skillIdBuffIdDic[skillID]);
                }
                addDamageSkillId.Remove(skillID);
            }
            if (useSkillTimes >= calcuNum && gainSkillIdList.Contains(skillID)) 
            {
                addDamageSkillId.Add(skillID);//因为已经满足条件 记录下当前增伤技能id
                useSkillTimes = 0;
                SetNvDaQiangEnergyBar(useSkillTimes);//随着useSikillTimes改变 做UI变化
                owner.buffController.RemoveBuff(overLoadBuffId);//去掉超频buff
                if (skillIdBuffInfoIdDic.ContainsKey(skillID))
                {
                    owner.buffController.TryAddBuffInfo(skillIdBuffInfoIdDic[skillID], owner, level);//添加增伤buff
                }
                return;
            }
            if (calcuSkillIdList.Contains(skillID))
            {
                useSkillTimes++;
                SetNvDaQiangEnergyBar(useSkillTimes);
                if (useSkillTimes >= calcuNum) //进入超载状态
                {
                    owner.buffController.TryAddBuff(overLoadBuffId, -1, level);
                }
            }
        }
    }

    private bool unInitUI = true;

    private void InitNvDaQiangEnergyBar(int n)
    {
#if !LOGIC_SERVER
        if (!unInitUI)
        {
            return;
        }

        if (owner != null && owner.isLocalActor)
        {
            var battleUI = GameClient.BattleUIHelper.CreateBattleUIComponent<GameClient.BattleUIProfession>();
            if (battleUI != null)
            {
                battleUI.InitNvDaQiangEnergyBar(n);
                unInitUI = false;
            }
        }
#endif
    }
    
    private void SetNvDaQiangEnergyBar(int times)
    {
#if !LOGIC_SERVER
        if (unInitUI)
        {
            return;
        }
        if (owner != null && owner.isLocalActor)
        {
            var battleUI = GameClient.BattleUIHelper.CreateBattleUIComponent<GameClient.BattleUIProfession>();
            if (battleUI != null)
            {
                battleUI.SetNvDaQiangEnergyBar(times);
            }
        }
#endif
    }
}

