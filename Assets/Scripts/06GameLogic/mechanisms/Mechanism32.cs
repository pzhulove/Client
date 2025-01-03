using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameClient;

/*
 三个巨人召唤
 */
public class Mechanism32 : BeMechanism
{
    protected int[] m_GiantIdArray = new int[3];            //巨人怪物ID列表
    protected int m_ProtectedEffectId = 0;                  //监听的怪物ID
    protected int m_ProtectHurtTime = 0;                    //伤害倒计时
    protected int m_BaseHurt = 0;                           //基础伤害值
    protected int[] m_HurtCoefficient = new int[4];         //伤害系数

    readonly protected int m_BeHitNoDamage = 547727;                 //受到攻击 但是不造成伤害
    protected int m_MaxHurt = 0;                            //最大伤害值
    protected int m_CurrentDamage = 0;                      //当前伤害
    protected bool m_AllGiantDead = false;                  //所有怪物死亡
    protected BeActor m_ProtectActor = null;                //保护罩怪物
    protected bool m_OnHurtCountDownFlag = false;           //正在被伤害标志
    readonly static protected string m_TipContent = "破坏仪式，阻止BOSS破封而出";  

    BattleUIDungeonDamageBar _battleUI = null;

    public Mechanism32(int mid, int lv) : base(mid, lv){}

    public override void OnReset()
    {
        BeUtility.ResetIntArray(m_GiantIdArray);
        BeUtility.ResetIntArray(m_HurtCoefficient);
        m_MaxHurt = 0;  
        m_CurrentDamage = 0;
        m_AllGiantDead = false; 
        m_ProtectActor = null;
        m_OnHurtCountDownFlag = false; 
        _battleUI = null;
    }

    public override void OnInit()
    {
        m_CurrentDamage = 0;
        for (int i = 0; i < data.ValueA.Count; i++)
        {
            var monsterId = TableManager.GetValueFromUnionCell(data.ValueA[i], level);
            m_GiantIdArray[i] = monsterId;
        }
        m_ProtectedEffectId = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        m_ProtectHurtTime = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
        m_BaseHurt = TableManager.GetValueFromUnionCell(data.ValueD[0], level);

        for (int i = 0; i < data.ValueE.Count; i++)
        {
            var coefficient = TableManager.GetValueFromUnionCell(data.ValueE[i], level);
            m_HurtCoefficient[i] = coefficient;
        }
    }

    public override void OnStart()
    {
        m_AllGiantDead = false;
    }

    public override void OnUpdate(int deltaTime)
    {
        if (!m_AllGiantDead)
        {
            if (CheckGiantAllDead())
            {
                m_AllGiantDead = true;
                StartDamage();
            }
        }
        else
        {
            if (m_ProtectHurtTime >= 0)
            {
                m_OnHurtCountDownFlag = true;
                m_ProtectHurtTime -= deltaTime;
#if !LOGIC_SERVER
                if (_battleUI != null)
                {
                    _battleUI.ShowDamageBar(true);
                    _battleUI.ChangeCountDown(m_ProtectHurtTime / 1000);
                }
#endif
            }
            else
            {
                m_OnHurtCountDownFlag = false;
            }

            //伤害倒计时结束
            if (!m_OnHurtCountDownFlag)
            {
                SetProtectDead();
            }
        }

    }

    //检查所有怪物是否死亡
    protected bool CheckGiantAllDead()
    {
        List<BeActor> giantList = new List<BeActor>();
        for (int i = 0; i < m_GiantIdArray.Length; i++)
        {
            owner.CurrentBeScene.FindActorById(giantList, m_GiantIdArray[i]);
            if (giantList.Count > 0 && giantList[0] != null)
            {
                if (!giantList[0].IsDead())
                    return false;
            }
        }
        return true;
    }

    protected void StartDamage()
    {
#if !LOGIC_SERVER
        GameClient.SystemNotifyManager.SysDungeonSkillTip(m_TipContent, m_ProtectHurtTime / 1000f);
        _battleUI = BattleUIHelper.CreateBattleUIComponent<BattleUIDungeonDamageBar>();
#endif
        int playerCount = owner.CurrentBeBattle.dungeonPlayerManager.GetAllPlayers().Count;
        int hard = owner.CurrentBeBattle.dungeonManager.GetDungeonDataManager().id.diffID;     //测试时候临时注释
		int coefficient = 0;
        switch (hard)
        {
            case 0:
                coefficient = m_HurtCoefficient[0];
                break;
            case 1:
                coefficient = m_HurtCoefficient[1];
                break;
            case 2:
                coefficient = m_HurtCoefficient[2];
                break;
            case 3:
                coefficient = m_HurtCoefficient[3];
                break;
        }
        VFactor xiShu = new VFactor(coefficient, GlobalLogic.VALUE_1000);
        m_MaxHurt = m_BaseHurt * playerCount * (hard + 1) * xiShu;
        AddHitNoDamageBuff();
        if (m_ProtectActor != null)
        {
            handleA = m_ProtectActor.RegisterEventNew(BeEventType.onHit, args =>
            //m_ProtectHandle = m_ProtectActor.RegisterEvent(BeEventType.onHit, (object[] args) =>
            {
                ProtectOnHurt(args.m_Int);
            });
        }
    }

    //给法阵添加能检测攻击但是不造成伤害BUFF
    protected void AddHitNoDamageBuff()
    {
        List<BeActor> ProtectedEffect = new List<BeActor>();
        owner.CurrentBeScene.FindActorById(ProtectedEffect, m_ProtectedEffectId);
        if (ProtectedEffect.Count > 0)
        {
            if (ProtectedEffect[0] != null)
            {
                m_ProtectActor = ProtectedEffect[0];
                m_ProtectActor.buffController.TryAddBuff(m_BeHitNoDamage);
            }
        }
    }

    //保护罩受到伤害
    protected void ProtectOnHurt(int value)
    {
        //玩家对boss造成的伤害值 
        m_CurrentDamage += value;
        if (m_CurrentDamage >= m_MaxHurt)
        {
#if !LOGIC_SERVER
            RefreshDamageBar();
            ShowDamageBar(false);
#endif
            SetBossDead();
        }
        else
        {
#if !LOGIC_SERVER
            RefreshDamageBar();
#endif
        }
    }

    /// <summary>
    /// 刷新伤害条数据
    /// </summary>
    protected void RefreshDamageBar()
    {
#if !LOGIC_SERVER
        if (_battleUI == null)
            return;
        ShowDamageBar(true);
        _battleUI.ChangeDamageData(m_CurrentDamage, m_MaxHurt);
#endif
    }

    //设置保护罩死亡
    protected void SetProtectDead()
    {
        if (m_ProtectActor != null)
        {
            m_ProtectActor.DoDead();
#if !LOGIC_SERVER
            ShowDamageBar(false);
#endif
        }
    }

    //设置自己死亡
    protected void SetBossDead()
    {
        owner.buffController.RemoveBuff((int)GlobalBuff.INVINCIBLE);
        owner.DoHurt(int.MaxValue);
    }

    //隐藏伤害进度条
    protected void ShowDamageBar(bool show)
    {
#if !LOGIC_SERVER
        if (_battleUI != null)
        {
            _battleUI.ShowDamageBar(show);
        }
#endif
    }
}
