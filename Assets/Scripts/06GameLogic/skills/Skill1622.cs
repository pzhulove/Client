
using ProtoTable;
using System.Collections.Generic;
using GameClient;
using UnityEngine;

public class Skill1622 : BeSkill
{

    int buffInfoID = 162205;//嗜血BUFFInfo
    int buffID = 162209;//嗜血BUFF
    int mechanismID = 999;//替换技能机制
    int buffInfoID1 = 162217;
    IBeEventHandle addBuffHandle = null;
    IBeEventHandle removeBuffHandle = null;
    IBeEventHandle hpChangeHandle = null;
    IBeEventHandle triggerHandle = null;
    DelayCallUnitHandle delayCall;
    bool flag = false;
    public Skill1622(int sid, int skillLevel) : base(sid, skillLevel)
    {

    }

    public static void SkillPreloadRes(SkillTable tableData)
    {
#if !LOGIC_SERVER
        if (BattleMain.instance == null) return;

        int buffInfoID = BattleMain.IsModePvP(BattleMain.battleType) ? 162206 : 162205;
        int buffInfoID1 = BattleMain.IsModePvP(BattleMain.battleType) ? 162218 : 162217;

        PreloadManager.PreloadBuffInfoID(buffInfoID, null, null);
        PreloadManager.PreloadBuffInfoID(buffInfoID1, null, null);
#endif
    }

    public override void OnStart()
    {
        flag = false;
        RemoveBuffHandle();
        buffInfoID = BattleMain.IsModePvP(owner.battleType) ? 162206 : 162205;
        buffInfoID1 = BattleMain.IsModePvP(owner.battleType) ? 162218 : 162217;
        buffID = BattleMain.IsModePvP(owner.battleType) ? 162210 : 162209;
        base.OnStart();
        addBuffHandle = owner.RegisterEventNew(BeEventType.onAddBuff, (args) =>
        {
            BeBuff buff = args.m_Obj as BeBuff;
            if (buff.buffID == this.buffID)
            {
                BeMechanism mechanism = owner.GetMechanism(mechanismID);
                if (mechanism != null)
                {
                    owner.RemoveMechanism(mechanismID);
                }
                owner.AddMechanism(mechanismID);

                BuffInfoData buffInfo = new BuffInfoData(BattleMain.IsModePvP(owner.battleType) ? 162210 : 162209, level);
                BuffInfoData buffInfo1 = new BuffInfoData(BattleMain.IsModePvP(owner.battleType) ? 162212 : 162211, level);
                owner.buffController.TryAddBuff(buffInfo);
                owner.buffController.TryAddBuff(buffInfo1);
            }
        });

        removeBuffHandle = owner.RegisterEventNew(BeEventType.onRemoveBuff, (args) =>
        {
            int id = (int)args.m_Int;
            if (id == this.buffID)
            {
                owner.RemoveMechanism(mechanismID);
                if (owner.sgGetCurrentState() == (int)ActionState.AS_IDLE)
                {
                    owner.Locomote(new BeStateData((int)ActionState.AS_IDLE));
                }
            }
        });



        hpChangeHandle = owner.RegisterEventNew(BeEventType.OnBuffHpChange, (args) =>
        {
            int id = args.m_Int;
            if (id == 162101 || id == 162102 || id == 162215 || id == 162216)
            {
                int maxHp = owner.GetEntityData().GetMaxHP();
                if (owner.GetEntityData().GetHPRate() < VFactor.NewVFactor(10, 1000))
                {
                    owner.GetEntityData().SetHP(maxHp * VFactor.NewVFactor(10, 1000));
                    owner.SetIsDead(false);
                    owner.m_pkGeActor.SyncHPBar();
                }

                if (owner.GetEntityData().GetHPRate() < VFactor.NewVFactor(5, 100))
                {
                    ((BeActorStateGraph)owner.GetStateGraph()).ExecuteNextPhaseSkill();
                }
            }
        });

        triggerHandle = owner.RegisterEventNew(BeEventType.OnReleaseButtonTrigger, (args) =>
        {
            if (flag || BattleMain.IsModePvP(owner.battleType)) return;
            flag = true;
            if (pressDuration > 500)
            {
                ((BeActorStateGraph)owner.GetStateGraph()).ExecuteNextPhaseSkill();
            }
            else
            {
                delayCall = owner.delayCaller.DelayCall(250, () =>
                {
                    ((BeActorStateGraph)owner.GetStateGraph()).ExecuteNextPhaseSkill();
                });
            }
        });

    }

    public override bool CanUseSkill()
    {
        if (base.CanUseSkill())
        {
            int curHp = owner.GetEntityData().GetHP();
            int maxHp = owner.GetEntityData().GetMaxHP();
            return VFactor.NewVFactor(curHp, maxHp) >= VFactor.NewVFactor(100, 1000);
        }
        return false;
    }

    public override BeSkillManager.SkillCannotUseType GetCannotUseType()
    {
        int curHp = owner.GetEntityData().GetHP();
        int maxHp = owner.GetEntityData().GetMaxHP();
        if (VFactor.NewVFactor(curHp, maxHp) < VFactor.NewVFactor(100, 1000))
        {
            return BeSkillManager.SkillCannotUseType.NO_HP;
        }
            

        return base.GetCannotUseType();
    }

    public override void OnCancel()
    {
        base.OnCancel();
        RemoveHandle();
    }

    public override void OnFinish()
    {
        base.OnFinish();

        RemoveHandle();
    }

    private void RemoveBuffHandle()
    {
        if (addBuffHandle != null)
        {
            addBuffHandle.Remove();
            addBuffHandle = null;
        }
        if (removeBuffHandle != null)
        {
            removeBuffHandle.Remove();
            removeBuffHandle = null;
        }
    }

    private void RemoveHandle()
    {
        if (owner.buffController.HasBuffByID(buffID) == null)
        {
            BuffInfoData buffInfo = new BuffInfoData(buffInfoID, level);
            owner.buffController.TryAddBuff(buffInfo);
        }

        BuffInfoData buffInfo1 = new BuffInfoData(buffInfoID1, level);
        owner.buffController.TryAddBuff(buffInfo1);
        if (hpChangeHandle != null)
        {
            hpChangeHandle.Remove();
            hpChangeHandle = null;
        }
        if (triggerHandle != null)
        {
            triggerHandle.Remove();
            triggerHandle = null;
        }
        delayCall.SetRemove(true);
    }
}
