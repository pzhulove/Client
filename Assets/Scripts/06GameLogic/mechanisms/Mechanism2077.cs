using System;
using System.Collections.Generic;
using GameClient;
using UnityEngine;

/// <summary>
/// combo到一定数，增加buff机制
/// 
/// 在达到combo时加buff
/// 当combo断后倒计时逐级回落
/// combo重新达成时，重置倒计时
/// </summary>
public class Mechanism2077 : BeMechanism
{
    private class ComboBuff
    {
        public readonly int mCombo;
        public readonly int mBuffId;
        public int mTimeAcc;
        public bool mIsAdd;

        public ComboBuff(int combo, int buffId)
        {
            mCombo = combo;
            mBuffId = buffId;
            mTimeAcc = 0;
            mIsAdd = false;
        }
        
    }
    
    // 配置数据
    private List<ComboBuff> mComBuffList; // combo/buff列表（排序）
    private int mUndoTime = 0; // buff定时回落时间
    
    // 内部数据
    private bool mIsComboStop = false;
    private int mCurMaxComboIndex = -1;
    private int mCurComboIndex = -1;
#if !LOGIC_SERVER
    private float mComboProgress = 0f;
#endif
    
    public Mechanism2077(int mid, int lv) : base(mid, lv)
    {
    }

    public override void OnInit()
    {
        base.OnInit();
        InitData();
    }

    public override void OnReset()
    {
        mComBuffList.Clear();
        mIsComboStop = false;
        mCurMaxComboIndex = -1;
        mCurComboIndex = -1;
#if !LOGIC_SERVER
        mComboProgress = 0f;
#endif
}

/// <summary>
/// 初始化Combo/buff列表，并排序
/// </summary>
private void InitData()
    {
        if (mComBuffList == null)
        {
            mComBuffList = GamePool.ListPool<ComboBuff>.Get();
            mComBuffList.Clear();     
        }
        
        if (data.ValueA.Count != data.ValueB.Count || data.ValueA.Count != data.ValueC.Count)
        {
            Logger.LogErrorFormat("2077机制配置文件错误:配置数量不统一{0}，{1}，{2}", data.ValueA.Count, data.ValueB.Count, data.ValueC.Count);
            return;
        }

        for (int i = 0; i < data.ValueA.Count; i++)
        {
            int combo = TableManager.GetValueFromUnionCell(data.ValueA[i], level);
            int buffId = !BattleMain.IsModePvP(battleType)?TableManager.GetValueFromUnionCell(data.ValueB[i], level):TableManager.GetValueFromUnionCell(data.ValueC[i], level);
            mComBuffList.Add(new ComboBuff(combo, buffId));
        }
        
        //mComBuffList.Sort((x, y) => x.mCombo.CompareTo(y.mCombo));
        mUndoTime =  TableManager.GetValueFromUnionCell(data.ValueD[0], level);
    }

    public override void OnStart()
    {
        base.OnStart();
        if (owner == null || mComBuffList == null)
            return;

        if (mComBuffList.Count <= 0)
            return;
        
        handleA = owner.RegisterEventNew(BeEventType.onBattleCombo, OnComboFeed);
    }

    /// <summary>
    /// combo增加
    /// </summary>
    /// <param name="args"></param>
    private void OnComboFeed(BeEvent.BeEventParam args)
    {
        if (owner == null || owner.actorData == null || owner.buffController == null)
            return;
        
        int curCombo = owner.actorData.GetCurComboCount();
        if (curCombo <= 0)
        {
            return;
        }

        int buffIndex = GetBuffIndex(curCombo);
        mCurComboIndex = buffIndex;
        if (buffIndex < mCurMaxComboIndex)
        {
            // buff递减
            Logger.LogProcessFormat("斗神意志[递减]:Combo:{0},max:{1},cur:{2}", owner.actorData.GetCurComboCount(), mCurMaxComboIndex, buffIndex);
            mIsComboStop = true;
        }
        else if(buffIndex >= 0)
        {
            // buff递增
            IncreaseBuff(buffIndex);
        }
    }

    private void IncreaseBuff(int buffIndex)
    {
        if(mComBuffList == null)
            return;
        
        if (buffIndex < 0 || buffIndex >= mComBuffList.Count)
            return;
        
        var comboBuffData = mComBuffList[buffIndex];
        if (comboBuffData == null)
            return;
        
        // 重置倒计时时间
        comboBuffData.mTimeAcc = 0;
        Logger.LogProcessFormat("斗神意志[重置]:Combo:{0},level:{1},Buff:{2}", owner.actorData.GetCurComboCount(), buffIndex, comboBuffData.mBuffId);
        
        if (comboBuffData.mIsAdd)
            return;
        
        Logger.LogProcessFormat("斗神意志[添加]:Combo:{0},level:{1},Buff:{2}", owner.actorData.GetCurComboCount(), buffIndex, comboBuffData.mBuffId);
        if(owner == null || owner.buffController == null)
            return;
        
        var buff = owner.buffController.TryAddBuff(comboBuffData.mBuffId, -1, level);
        if (buff == null)
            return;

        if (mCurMaxComboIndex >= 0 && mCurMaxComboIndex < mComBuffList.Count)
        {
            owner.buffController.RemoveBuff(mComBuffList[mCurMaxComboIndex].mBuffId);
        }
        
        mIsComboStop = false;
        mCurMaxComboIndex = buffIndex;
        comboBuffData.mIsAdd = true;
#if !LOGIC_SERVER
        UpdateBuffInfo(mCurMaxComboIndex, 1);
#endif
    }
    
    public override void OnUpdate(int deltaTime)
    {
        base.OnUpdate(deltaTime);
        UpdateUndoBuff(deltaTime);
    }
    
#if !LOGIC_SERVER
    public override void OnUpdateGraphic(int deltaTime)
    {
        base.OnUpdateGraphic(deltaTime);
        // 3帧刷一次
        if(Time.frameCount % 3 == 0)
            UpdateBuffInfo(mCurMaxComboIndex, mComboProgress);
    }
#endif

    /// <summary>
    /// 刷新buff回落
    /// </summary>
    private void UpdateUndoBuff(int deltaTime)
    {
#if !LOGIC_SERVER
        mComboProgress = 1;
#endif
        if (!mIsComboStop)
            return;
        
        if (mCurComboIndex >= mCurMaxComboIndex)
        {
            Logger.LogProcessFormat("斗神意志[递减结束]:max:{0}, cur:{1}", mCurComboIndex, mCurMaxComboIndex);
            mIsComboStop = false;
            return;
        }
        
        if (mCurMaxComboIndex < 0)
            return;

        if(mCurMaxComboIndex >= mComBuffList.Count)
            return;
        
        var comboBuff = mComBuffList[mCurMaxComboIndex];
        if (comboBuff == null)
            return;

        comboBuff.mTimeAcc += deltaTime;
#if !LOGIC_SERVER
        mComboProgress = (mUndoTime - comboBuff.mTimeAcc) / (float)mUndoTime;
#endif
        if (comboBuff.mTimeAcc >= mUndoTime)
        {
            UndoBuff(comboBuff);
        }
    }

    /// <summary>
    /// 回落buff
    /// </summary>
    private void UndoBuff(ComboBuff comboBuff)
    {
        if (mCurMaxComboIndex < 0)
            return;
        
        if (owner == null || owner.actorData == null || owner.buffController == null)
            return;
        
        owner.buffController.RemoveBuff(comboBuff.mBuffId);
        comboBuff.mIsAdd = false;
        Logger.LogProcessFormat("斗神意志[回落]:level:{0},Buff:{1}", mCurMaxComboIndex, comboBuff.mBuffId);
        mCurMaxComboIndex--;
        if (mCurMaxComboIndex >= 0 && mCurMaxComboIndex < mComBuffList.Count)
        {
            owner.buffController.TryAddBuff(mComBuffList[mCurMaxComboIndex].mBuffId, -1, level);
        }
#if !LOGIC_SERVER
        UpdateBuffInfo(mCurMaxComboIndex, 1);
#endif
        if (mCurMaxComboIndex < 0)
            mIsComboStop = false;
    }
    
    /// <summary>
    /// 获取当先combo的buff
    /// </summary>
    /// <param name="combo">当前combo</param>
    /// <returns>列表序号</returns>
    private int GetBuffIndex(int combo)
    {
        int ret = -1;

        if (mComBuffList == null)
            return ret;
        
        for (int i = 0; i < mComBuffList.Count; i++)
        {
            if (mComBuffList[i].mCombo <= combo)
            {
                ret = i;
            }
            else
            {
                break;
            }
        }
        
        return ret;
    }
    
#if !LOGIC_SERVER
    /// <summary>
    /// 刷新UI，Buff数量与可见
    /// </summary>
    /// <param name="level"></param>
    public void UpdateBuffInfo(int index, float progress)
    {
        if (owner != null && owner.isLocalActor)
        {
            var battleUI = BattleUIHelper.CreateBattleUIComponent<BattleUIProfession>();;
            if (battleUI != null)
            {
                battleUI.SetComboBuffNum(index + 1, progress);
            }
        }
    }
#endif

    public override void OnFinish()
    {
        base.OnFinish();
        if (mComBuffList != null)
        {
            GamePool.ListPool<ComboBuff>.Release(mComBuffList);
            mComBuffList = null;
        }
    }
}