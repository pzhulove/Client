using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameClient;

public partial class BeActorData
{
    protected enum ComboState { None, Combo };
    
    protected ComboState curState = ComboState.None;

    protected BeActor actor;
    protected BeActor owner = null;

    protected BeEvent.BeEventHandleNew handler;
    protected BeEvent.BeEventHandleNew deadHandler;
    protected BeEvent.BeEventHandleNew breakHandle;
    protected BeEvent.BeEventHandleNew blockHandle;
    protected bool enable = false;

    protected int maxCombo = 0;
    protected int curCombo = 0;
    protected int curBreakAction = 0;
    protected int curBlock = 0;
    protected int comboIntervel = 1250;
    protected int timeAcc;

    public bool isSpecialMonster = false;

    protected float comboDamageValue = 0;
    protected uint[] comboMaxDamageList = new uint[3] { 0, 0, 0 };
    public BeActorData(BeActor a)
    {
        actor = a;
        comboIntervel = comboDefaultIntervel;
    }

    public void SetEnable(bool flag)
    {
        enable = flag;

        if (enable)
        {
            ResetData();
        }
    }

    public bool IsEnable()
    {
        return enable;
    }

    
    public void Update(int iDeltaTime)
    {
        UpdateCombo(iDeltaTime);

        if (isSpecialMonster)
        {
            UpdateOwnerCombo(iDeltaTime);
        }
    }

    protected void UpdateCombo(int iDeltaTime)
    {
        if (curState == ComboState.Combo)
        {
            if (timeAcc > 0)
            {
                timeAcc -= iDeltaTime;
            }
            else
            {
                if (owner == null)
                {
                    StopFeed();
                }
                else
                {
                    owner.actorData.StopFeed();
                }
            }
        }
    }

    protected void UpdateOwnerCombo(int iDeltaTime)
    {
        if (owner != null)
        {
            if (owner.actorData.curState == ComboState.Combo)
            {
                if (owner.actorData.timeAcc > 0)
                {
                    owner.actorData.timeAcc -= iDeltaTime;
                }
                else
                {
                    owner.actorData.StopFeed();
                }
            }
        }
    }

    public void RemoveHandle()
    {
        if (handler != null)
        {
            handler.Remove();
            handler = null;
        }
        if (breakHandle != null)
        {
            breakHandle.Remove();
            breakHandle = null;
        }
        if (blockHandle != null)
        {
            blockHandle.Remove();
            blockHandle = null;
        }
        RemoveLogicHandle();
    }

    protected void ResetData()
    {
        curState = ComboState.None;
        maxCombo = 0;
        curCombo = 0;
        timeAcc = 0;
        curBreakAction = 0;
        curBlock = 0;
        comboDamageValue = 0;
        if (actor.GetEntityData() != null && actor.GetEntityData().isSummonMonster)
        {
            var o = actor.GetOwner();
            owner = o as BeActor;
        }

        if (handler != null)
        {
            handler.Remove();
            handler = null;
        }
        handler = actor.RegisterEventNew(BeEventType.onHitOther, args =>
        //handler = actor.RegisterEvent(BeEventType.onHitOther, (object[] args) =>
        {
            float damageValueFactor = 0;
            //if (args.Length > 2 && args[1] != null)
            //{
                var hurtid = args.m_Int;
                var hurtData = TableManager.GetInstance().GetTableItem<ProtoTable.EffectTable>(hurtid);
                if (hurtData != null && (hurtData.IsFriendDamage > 0 || hurtData.HasDamage != 1))
                {
                    return;
                }
                var target = args.m_Obj as BeEntity;
                if (target != null && target.attribute != null)
                {
                    int damageValue = args.m_Int4;
                    if (target.attribute.GetMaxHP() > 0)
                    {
                        damageValueFactor = (float)damageValue / target.attribute.GetMaxHP();
                    }
                }
            //}

            if (owner == null)
            {
                Feed(damageValueFactor);
            }
            else
            {
                owner.actorData.Feed(damageValueFactor);
            }
        });

        if (breakHandle != null)
        {
            breakHandle.Remove();
            breakHandle = null;
        }
        breakHandle = actor.RegisterEventNew(BeEventType.onBreakAction, (args) =>
        {
            curBreakAction++;
        });

        if (blockHandle != null)
        {
            blockHandle.Remove();
            blockHandle = null;
        }

        blockHandle = actor.RegisterEventNew(BeEventType.BlockSuccess, _BlockSuccess);

        //blockHandle = actor.RegisterEventNew(BeEventType.BlockSuccess, (args) =>
        //{
        //    curBlock++;
        //});
        if (actor.battleType == BattleType.ScufflePVP)
        {
            if (deadHandler != null)
            {
                deadHandler.Remove();
                deadHandler = null;
            }
            BeActor beActor = owner == null ? actor : owner;
            deadHandler = beActor.RegisterEventNew(BeEventType.onDead, eventParam =>
            {
                beActor.actorData.StopFeed();
            });
        }

    }

    private void _BlockSuccess(BeEvent.BeEventParam param)
    {
        curBlock++;
    }

    public void Feed(float damgeValue)
    {
        curState = ComboState.Combo;
        timeAcc = comboIntervel;
        curCombo++;
        comboDamageValue += damgeValue;
        if (curCombo > maxCombo)
        {
            maxCombo = curCombo;
        }
#if !LOGIC_SERVER
        if (actor.isLocalActor || owner != null && owner.isLocalActor)
        {
            var battleUI = BattleUIHelper.GetBattleUIComponent<BattleUICommon>();
            if (battleUI != null)
                battleUI.FeedCombo(curCombo);
        }
#endif
    }
    private uint GetDamageScore(float damageFactor, int comboCount)
    {
#if !LOGIC_SERVER
        if (actor == null) return 0;
        if (actor.battleType != BattleType.MutiPlayer) return 0;
        int damage = (int)(damageFactor * 100);
        return TableManager.instance.GetComboScore(actor.professionID, Mathf.Min(damage, 100), comboCount);
#else
        return 0;
#endif
    }
    public void StopFeed()
    {
        curState = ComboState.None;
        uint score = GetDamageScore(comboDamageValue, curCombo);
#if UNITY_EDITOR
        if (actor.battleType == BattleType.MutiPlayer)
            Logger.LogWarningFormat("名字：{0}  本次Combo：{1},本次伤害百分比：{2},本次得分：{3}", actor.GetName(), curCombo, (int)(comboDamageValue * 100), score);
#endif
        timeAcc = 0;
        curCombo = 0;
        
        if (actor.battleType == BattleType.MutiPlayer)
        {
            for (int i = 0; i < comboMaxDamageList.Length; i++)
            {
                if (comboMaxDamageList[i] <= score)
                {
                    for (int j = comboMaxDamageList.Length - 1; j > i; j--)
                    {
                        comboMaxDamageList[j] = comboMaxDamageList[j - 1];
                    }
                    comboMaxDamageList[i] = score;
                    break;
                }
            }
            comboDamageValue = 0;
        }
#if !LOGIC_SERVER
        if (actor.isLocalActor || owner != null && owner.isLocalActor)
        {
            var battleUI = BattleUIHelper.GetBattleUIComponent<BattleUICommon>();
            if (battleUI != null)
                battleUI.ResetCombo();
        }
#endif
    }

    public int GetMaxCombo()
    {
        return maxCombo;
    }
    /// <summary>
    /// 根据combo数计算得分
    /// </summary>
    /// <returns></returns>
    public uint GetDamageScore()
    {
        uint totalScore = 0;
        for (int i = 0; i < comboMaxDamageList.Length; i++)
        {
            if (i == 0)
            {
                totalScore += comboMaxDamageList[i];
            }
            if (i == 1)
            {
                totalScore = totalScore + (uint)(0.5f * comboMaxDamageList[i]);
            }
            if (i == 2)
            {
                totalScore = totalScore + (uint)(0.3f * comboMaxDamageList[i]);
            }
        }
        return (uint)Mathf.Clamp(totalScore, 0, 450);
    }
    /// <summary>
    /// 根据格挡和破招计算分数
    /// </summary>
    /// <returns></returns>
    public uint GetExtraScore()
    {
        uint curBreakScore = 0;
        uint curBlockScore = 0;

        curBreakScore = (uint)Mathf.Clamp(curBreakAction * 10, 0,30);
        curBlockScore = (uint)Mathf.Clamp(curBlock * 10, 0, 20);
        return curBreakScore + curBlockScore;
    }

}
