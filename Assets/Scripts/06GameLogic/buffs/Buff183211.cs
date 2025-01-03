using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameClient;

//柔化肌肉-PVP
class Buff183212 : Buff183211
{
    private int initCD = 10000;
    private bool canuse = false;
    private bool in_cd = true;
    public Buff183212(int bi, int buffLevel, int buffDuration, int attack = 0) : base(bi, buffLevel, buffDuration, attack) { }

    int acc = 0;
    int acc_pre = 0;

    public override void OnReset()
    {
        initCD = 10000;
        canuse = false;
        in_cd = true;
        acc = 0;
        acc_pre = 0;
    }
    public override void OnUpdate(int delta)
    {
        if (owner.battleType == BattleType.Training)
        {
            base.OnUpdate(delta);
            return;
        }
        if (in_cd)
        {
            acc_pre += delta;
            if (acc_pre <= 4000) return;
        }
#if !LOGIC_SERVER
        if (owner != null && owner.isLocalActor)
        {
            if (battleUI == null)
            {
                battleUI = BattleUIHelper.CreateBattleUIComponent<BattleUIProfession>();
                if (battleUI != null)
                {
                    battleUI.SetMuscleShiftActive(true);
                }
            }
        }
#endif
        if (in_cd)
        {
            acc += delta;
            if (acc < initCD)
            {
                if (battleUI != null)
                    battleUI.SetMuscleShiftCD((float)acc / initCD);
            }
            else
            {
                canuse = true;
                if (battleUI != null)
                    battleUI.SetMuscleShiftCount(count);
                in_cd = false;
            }
        }
        else
        {
            base.OnUpdate(delta);
        }
    }
    public override bool CanUseSkill(int curSkillId, int skillId)
    {
        if (owner.battleType == BattleType.Training)
            canuse = true;
        return base.CanUseSkill(curSkillId, skillId) && canuse;
    }
}

//柔化肌肉-PVE
class Buff183211 : BeBuff
{
    protected int maxCount;
    int intervel;
    List<int> skillList = new List<int>();

    protected BattleUIProfession battleUI = null;
    IBeEventHandle handle;
    protected int count;
    int timer;
    bool decreaseFlag;

    public Buff183211(int bi, int buffLevel, int buffDuration, int attack = 0) : base(bi, buffLevel, buffDuration, attack) { }

    public override void OnReset()
    {
        maxCount = 0;
        intervel = 0;
        skillList.Clear();
        battleUI = null;
        count = 0;
        timer = 0;
        decreaseFlag = false;
    }

    public override void OnInit()
    {
        maxCount = TableManager.GetValueFromUnionCell(buffData.ValueA[0], level);
        intervel = TableManager.GetValueFromUnionCell(buffData.ValueB[0], level);
        for (int i = 0; i < buffData.ValueC.Count; i++)
        {
            skillList.Add(TableManager.GetValueFromUnionCell(buffData.ValueC[i], level));
        }
    }

    public override void OnStart()
    {
        count = maxCount;
        timer = 0;
        decreaseFlag = false;

        handle = owner.RegisterEventNew(BeEventType.onCastSkillFinish, param =>
        {
            if (decreaseFlag)
            {
                owner.buffController.RemoveBuffByBuffInfoID(183206);
                owner.buffController.RemoveBuffByBuffInfoID(1832060);
                decreaseFlag = false;
            }
        });
    }

    public override void OnUpdate(int delta)
    {
#if !LOGIC_SERVER
        if (owner != null && owner.isLocalActor)
        {
            if (battleUI == null)
            {
                battleUI = BattleUIHelper.CreateBattleUIComponent<BattleUIProfession>();;
                if (battleUI != null)
                {
                    battleUI.SetMuscleShiftActive(true);
                    battleUI.SetMuscleShiftCount(count);
                }
            }
        }
#endif

        if (count < maxCount)
        {
            timer += delta;

            if (battleUI != null)
                battleUI.SetMuscleShiftCD((float)timer / intervel);

            if (timer >= intervel)
            {
                Increase();
                timer = 0;
            }
        }
    }

    public void Increase()
    {
        if (count < maxCount)
        {
            ++count;
            if (battleUI != null)
                battleUI.SetMuscleShiftCount(count);
        }
    }

    public void Decrease(int curSkillId, int skillId)
    {
        if (count > 0)
        {
            --count;
            decreaseFlag = true;
            if (battleUI != null)
                battleUI.SetMuscleShiftCount(count);
        }
    }

    public override void OnFinish()
    {
        if (battleUI != null)
            battleUI.SetMuscleShiftActive(false);

        if (handle != null)
        {
            handle.Remove();
            handle = null;
        }

        skillList.Clear();
    }

    public virtual bool CanUseSkill(int curSkillId, int skillId)
    {
        return count > 0 &&
            curSkillId != skillId && 
            skillList.Contains(curSkillId) && 
            skillList.Contains(skillId) &&
            (owner.GetPosition().z <= 0 || owner.GetPosition().z > 0 && skillId == 3113);
    }
}
