using System.Collections.Generic;
using UnityEngine;
using GameClient;

/// <summary>
/// 力法-炫纹发射
/// 
///1.在攻击成功后，发射炫纹；炫纹对人物攻击命中的最后一个目标造成相对应的打击与效果
///2.炫纹发射所发射的炫纹 为 先入先出/FIFO 栈 形式
///3.只有在人物直接攻击到目标时才可以 使用 炫纹发射 来发射炫纹（炫纹命中除外，炫纹命中不会触发，异常伤害不会触发，仅人物主动技能伤害后触发）
///4.炫纹最多存在7个
///5.技能释放判定时间为0.5s（PVE）0.4s(PVP)
///6.炫纹爆炸是一个范围攻击（可以攻击多个目标）
///7.一旦人物被攻击时则无法发射炫纹（验证霸体时被击会触发）
///8.PVP是0.4s，PVE是1.5s。冷却PVP是1s，PVE是0.3s（时间自调）
///9.炫纹只有在飞出去并且到达目标后死亡后，才会生成新的炫纹（场上不会同时存在8个炫纹，有且只有7个炫纹）
public class Mechanism2079 : BeMechanism
{
    private int mActiveTime = 0;
    private int mActiveTimeAcc = 0;
    private HashSet<int> mHurtIDSet;
    private bool mActiveSkill = false;
    
    public Mechanism2079(int mid, int lv) : base(mid, lv)
    {
    }

    public override void OnInit()
    {
        mActiveTime = BattleMain.IsModePvP(battleType) ? TableManager.GetValueFromUnionCell(data.ValueA[1], level) : TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        mHurtIDSet = new HashSet<int>();
        for (int i = 0; i < data.ValueB.Count; i++)
        {
            mHurtIDSet.Add(TableManager.GetValueFromUnionCell(data.ValueB[i], level));
        }
    }

    public override void OnReset()
    {
        mActiveTimeAcc = 0;
        mActiveSkill = false;
    }

    public override void OnStart()
    {
        // 过滤剔除不在列表中的hurtid
        base.OnStart();

        if (owner == null)
            return;

        ///3.只有在人物直接攻击到目标时才可以 使用 炫纹发射 来发射炫纹（炫纹+
        /// +命中除外，炫纹命中不会触发，异常伤害不会触发，仅人物主动技能伤害后触发）
        //handleA = owner.RegisterEvent(BeEventType.onHitOtherAfterHurt, (object[] args) =>
        //{

        //});
        handleA = OwnerRegisterEventNew(BeEventType.onHitOtherAfterHurt, _OnHitOtherAfterHurt);


        ///7.一旦人物被攻击时则无法发射炫纹（验证霸体时被击会触发）
        handleB = owner.RegisterEventNew(BeEventType.onHurt, args => 
        {
            mActiveSkill = false;
        });
        //handleB = owner.RegisterEvent(BeEventType.onHurt, (object[] args) => { mActiveSkill = false; });
    }

    private void _OnHitOtherAfterHurt(BeEvent.BeEventParam param)
    {
        int hurtid = param.m_Int;
        // 过滤剔除不在列表中的hurtid
        if (!mHurtIDSet.Contains(hurtid))
        {
            return;
        }

        mActiveSkill = true;
        mActiveTimeAcc = 0;
    }

    public override void OnUpdate(int iDeltime)
    {
        base.OnUpdate(iDeltime);

        if (mActiveSkill)
        {
            mActiveTimeAcc += iDeltime;
            if (mActiveTimeAcc >= mActiveTime)
            {
                mActiveSkill = false;
                mActiveTimeAcc = 0;
            }
        }
    }

    public bool IsActive()
    {
        return mActiveSkill;
    }
    
    public void SetActive(bool active)
    {
        mActiveSkill = active;
    }
}