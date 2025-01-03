using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Skill1107 : BeSkill
{
    public VInt range = new VInt();
    int buffInfoID = 0;
    protected List<BeActor> actorList = new List<BeActor>();
    public Skill1107(int sid, int skillLevel) : base(sid, skillLevel) { }

    public override void OnPostInit()
    {
        handleA = owner.RegisterEventNew(BeEventType.ConfigCommand, (args) =>
        {
            if (skillData.ValueA.Count > 1)
            {
                int buffInfoID = !owner.autoHitConfig ? TableManager.GetValueFromUnionCell(isPVP() ? skillData.ValueA[0] : skillData.ValueA[1], level) :
                     TableManager.GetValueFromUnionCell(skillData.ValueC[0], level);
                if (buffInfoID > 0)
                {
                    if (owner != null)
                    {
                        owner.buffController.RemoveTriggerBuff(buffInfoID);

                        BuffInfoData buffInfo = new BuffInfoData(buffInfoID, level);
                        owner.buffController.AddTriggerBuff(buffInfo);
                    }
                }
            }
        });

        range = VInt.Float2VIntValue(TableManager.GetValueFromUnionCell(skillData.ValueB[0], level) / 1000.0f);
    }

    public override bool CanUseSkill()
    {
        bool attackButton = true;
        if (owner.autoHitConfig)
        {
            attackButton = owner.GetCurrentBtnState() == GameClient.ButtonState.PRESS;
        }

        owner.CurrentBeScene.FindActorInRange(actorList, owner.GetPosition(), range, owner.GetCamp() == 0 ? 1 : 0);
        bool canUse = base.CanUseSkill();
        return canUse && owner.sgGetCurrentState() == (int)ActionState.AS_HURT && (owner.GetPosition().z <= 0) && actorList.Count > 0 && !owner.IsDead() && attackButton && !HaveBuff();
    }

    /// <summary>
    /// 胜利之矛的buff
    /// </summary>
    /// <returns></returns>
    private bool HaveBuff()
    {
        BeBuff buff =   owner.buffController.GetBuffList().Find((x) => { return x.buffID == 370500 || x.buffID == 370501; });
        return buff != null;
    }

    public bool canUseSkill()
    {
        return base.CanUseSkill();
    }

    public bool isPVP()
    {
        return BattleMain.IsModePvP(battleType);
    }
    public override bool CheckSpellCondition(ActionState state)
    {
        return owner.sgGetCurrentState() == (int)ActionState.AS_HURT && (owner.GetPosition().z <= 0);
    }
}

public class Skill1910 : Skill1107
{
    BeEntity actor = null;
    IBeEventHandle handle = null;
    public Skill1910(int sid, int skillLevel) : base(sid, skillLevel) { }

    public override void OnPostInit()
    {
        handleA = owner.RegisterEventNew(BeEventType.ConfigCommand, (args) =>
        {
            if (skillData.ValueA.Count > 1)
            {
                int buffInfoID = !owner.backHitConfig ? TableManager.GetValueFromUnionCell(isPVP() ? skillData.ValueA[0] : skillData.ValueA[1], level) :
                     TableManager.GetValueFromUnionCell(skillData.ValueC[0], level);
                if (buffInfoID > 0)
                {
                    if (owner != null)
                    {
                        owner.buffController.RemoveTriggerBuff(buffInfoID);

                        BuffInfoData buffInfo = new BuffInfoData(buffInfoID, level);
                        owner.buffController.AddTriggerBuff(buffInfo);
                    }
                }
            }
        });
        range = VInt.Float2VIntValue(TableManager.GetValueFromUnionCell(skillData.ValueB[0], level) / 1000.0f);

        RemoveHandle();
        handle = owner.RegisterEventNew(BeEventType.onBackHit, args =>
        //handle = owner.RegisterEvent(BeEventType.onBackHit, (object[] args) =>
        {
            BeEntity entity = args.m_Obj as BeEntity;
            if (entity != null)
            {
                if (entity is BeProjectile)
                {
                    actor = entity.GetOwner();
                }
                else
                {
                    actor = entity;
                }
            }

        });
    }

    public override bool CanUseSkill()
    {
        bool attackButton = true;
        if (owner.backHitConfig)
        {
            attackButton = owner.GetCurrentBtnState() == GameClient.ButtonState.PRESS;

        }

        owner.CurrentBeScene.FindActorInRange(actorList, owner.GetPosition(), range, owner.GetCamp() == 0 ? 1 : 0);
        bool canUse = canUseSkill();
        return canUse && owner.sgGetCurrentState() == (int)ActionState.AS_HURT && (owner.GetPosition().z <= 0) && actorList.Count > 0 && !owner.IsDead() && attackButton;
    }

    void RemoveHandle()
    {
        if (handle != null)
        {
            handle.Remove();
            handle = null;
        }
    }

    public override void OnStart()
    {
        base.OnStart();
        if (actor != null)
        {
            owner.SetFace(owner.GetPosition().x > actor.GetPosition().x, true);
        }
    }


}
