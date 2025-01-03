using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameClient;

/*
阵鬼 残影之凯贾机制
 */

public class Mechanism42 : BeMechanism
{
    protected int m_BeHitNumPve = 0;                        //被击次数PVE
    protected int m_BeHitNumPvp = 0;                        //被击次数PVP
    protected int m_ReplaceAttackId = 0;                    //替换的普攻Id
    protected List<int> m_BuffIdList = new List<int>();     //需要移除的BuffId
    protected int m_ReplaceSkillId = 0;                     //上挑替换成的技能ID
    protected List<int> m_RemoveBuffList = new List<int>(); //残影凯贾前冲结束移除Buff
    protected List<int> m_AddBuffInfoList = new List<int>();//前冲时添加的BuffInfo

    protected BeEvent.BeEventHandleNew m_BeHitHandle = null;           //监测被击
    protected IBeEventHandle m_ReplaceSkillHandle = null;    //替换上挑
    readonly protected int m_BackupSkillId = 1503;                   //鬼剑上挑技能ID
    protected BeActor.NormalAttack m_AttackData = new BeActor.NormalAttack();     //备份替换普攻数据
    protected int m_CurrentBeHitNum = 0;                    //当前被击次数
    protected int m_HitNum = 0;
    protected IBeEventHandle m_StateChange = null;           //监听状态切换
    protected bool m_IsRunAddBuffState = false;             //是否处于跑的状态
    protected int m_CurrRunTimeAcc = 0;                     //当前跑动的时间
    readonly protected int m_MaxRunTimeAcc = 300;                    //上前冲无敌Buff的时间

    public Mechanism42(int sid, int skillLevel) : base(sid, skillLevel){}

    public override void OnReset()
    {
        m_BuffIdList.Clear();
        m_RemoveBuffList.Clear();
        m_AddBuffInfoList.Clear();
        m_BeHitHandle = null; 
        m_ReplaceSkillHandle = null;
        m_AttackData = new BeActor.NormalAttack();
        m_CurrentBeHitNum = 0;
        m_HitNum = 0;
        m_StateChange = null;
        m_IsRunAddBuffState = false; 
        m_CurrRunTimeAcc = 0; 
    }

    public sealed override void OnInit()
    {
        m_BeHitNumPve = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        m_BeHitNumPvp = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        m_ReplaceAttackId = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
        for (int i = 0; i < data.ValueD.Count; i++)
        {
            int removeBuffId = TableManager.GetValueFromUnionCell(data.ValueD[i], level);
            m_BuffIdList.Add(removeBuffId);
        }
        m_ReplaceSkillId = TableManager.GetValueFromUnionCell(data.ValueE[0], level);
        if (data.ValueF.Count > 0)
        {
            for (int i = 0; i < data.ValueF.Count; i++)
            {
                int buffId = TableManager.GetValueFromUnionCell(data.ValueF[i],level);
                m_RemoveBuffList.Add(buffId);
            }
        }
        if (data.ValueG.Count > 0)
        {
            for (int i = 0; i < data.ValueG.Count; i++)
            {
                int buffInfoId = TableManager.GetValueFromUnionCell(data.ValueG[i], level);
                m_AddBuffInfoList.Add(buffInfoId);
            }
        }
    }

    public sealed override void OnStart()
    {
        m_HitNum = BattleMain.IsModePvP(battleType) ? m_BeHitNumPvp : m_BeHitNumPve;
        m_CurrentBeHitNum = 0;
        ReplaceAttackId();
        ReplaceSkill();
        if (m_HitNum != 0)
        {
            m_BeHitHandle = owner.RegisterEventNew(BeEventType.onHit, arg =>
            {
                bool needStatis = true;

                int hurtid = arg.m_Int4;
                if (hurtid > 0)
                { 
                    var hurtData = TableManager.GetInstance().GetTableItem<ProtoTable.EffectTable>(hurtid);
                    if (hurtData != null && hurtData.IsFriendDamage > 0)
                    {
                        needStatis = false;
                    }
                }
                    
                if (needStatis)
                {
                    BeHit();
                }
                    
            });
        }

        m_StateChange = owner.RegisterEventNew(BeEventType.onStateChange, (param) => 
        {
            var state = (ActionState)param.m_Int;
            if (state == ActionState.AS_RUN && owner.IsRunning())
            {
                m_IsRunAddBuffState = true;
                m_CurrRunTimeAcc = 0;
            }
            else
            {
                m_IsRunAddBuffState = false;
                for (int i = 0; i < m_RemoveBuffList.Count; i++)
                {
                    owner.buffController.RemoveBuff(m_RemoveBuffList[i]);
                }
            }
        });
    }

    public sealed override void OnUpdate(int deltaTime)
    {
        if (m_IsRunAddBuffState)
        {
            if (m_CurrRunTimeAcc >= m_MaxRunTimeAcc)
            {
                for (int i = 0; i < m_AddBuffInfoList.Count; i++)
                {
                    if (m_AddBuffInfoList[i] != null)
                    {
                        owner.buffController.TryAddBuff(m_AddBuffInfoList[i]);
                    }
                }
                m_IsRunAddBuffState = false;
            }
            else
            {
                m_CurrRunTimeAcc += deltaTime;
            }
        }
    }

    protected void BeHit()
    {
        m_CurrentBeHitNum += 1;
        if (m_CurrentBeHitNum >= m_HitNum)
        {
            RemoveEffect();
        }
    }

    //替换普攻技能
    protected void ReplaceAttackId()
    {
        if (BattleMain.IsModePvP(battleType))
        {
            // 在PVP情况下，冥炎之卡洛 覆盖 残影之骸骨 
            m_AttackData = owner.AddReplaceAttackId(m_ReplaceAttackId, 1);
        }
        else
        {
            // 在PVE情况下，残影之骸骨 覆盖 冥炎之卡洛
            m_AttackData = owner.AddReplaceAttackId(m_ReplaceAttackId, 3);
        }
    }

    //还原普攻技能
    protected void RestoreAttackId()
    {
        owner.RemoveReplaceAttackId(m_AttackData);
    }

    //替换技能Id
    protected void ReplaceSkill()
    {
        m_ReplaceSkillHandle = owner.RegisterEventNew(BeEventType.onPreSetSkillAction, (GameClient.BeEvent.BeEventParam param) =>
        {
            //int[] skillIdList = (int[])args[0];
            //int curSkillId = skillIdList[0];

            if (param.m_Int == m_BackupSkillId)
            {
                param.m_Int = m_ReplaceSkillId;
            }
        });
    }

    //不再监听替换技能
    protected void RemoveSkillReplaceHandle()
    {
        if (m_ReplaceSkillHandle != null)
        {
            m_ReplaceSkillHandle.Remove();
        }
    }

    //移除技能效果
    protected void RemoveEffect()
    {
        RestoreAttackId();
        RemoveSkillReplaceHandle();
        for (int i = 0; i < m_BuffIdList.Count; i++)
        {
            owner.buffController.RemoveBuff(m_BuffIdList[i]);
        } 
    }

    public sealed override void OnFinish()
    {
        if (m_BeHitHandle != null)
        {
            m_BeHitHandle.Remove();
        }

        if (m_StateChange != null)
        {
            m_StateChange.Remove();
        }

        RemoveEffect();
    }
}