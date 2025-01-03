using System.Collections;
using System.Collections.Generic;

//抓取的目标替换模型，表现用
public class Mechanism10015 : BeMechanism
{
    int monsterId;

    BeActor target;
    BeActor puppet;

    public Mechanism10015(int mid, int lv) : base(mid, lv) { }

    public override void OnInit()
    {
        monsterId = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
    }

    public override void OnStart()
    {
        if (owner.grabController.grabbedActorList.Count == 0)
        {
            return;
        }

        target = owner.grabController.grabbedActorList[0];
        if (target != null && target.m_pkGeActor != null)
        {
            target.m_pkGeActor.HideActor(true);
            target.m_pkGeActor.SetShadowVisible(target.CurrentBeScene.currentGeScene, false);
        }

        puppet = owner.CurrentBeScene.CreateMonster(monsterId, target.GetPosition());
        if (puppet != null && target != null)
        {
            if (puppet.m_pkGeActor != null)
            {
                puppet.m_pkGeActor.SetHeadInfoVisible(false);
            }

            puppet.stateController.SetAbilityEnable(BeAbilityType.BEHIT, false);
            puppet.stateController.SetAbilityEnable(BeAbilityType.BETARGETED, false);

            var action = target.m_cpkCurEntityActionInfo.moveName;
            if (puppet.HasAction(action))
            {
                puppet.PlayAction(action);
            }

            handleA = target.RegisterEventNew(BeEventType.onPlayAction, param =>
            {
                if (puppet.HasAction(param.m_String))
                {
                    puppet.PlayAction(param.m_String);
                }
            });
        }
    }

    public override void OnUpdate(int deltaTime)
    {
        if (puppet != null)
        {
            puppet.SetFace(target.GetFace());
            puppet.SetPosition(target.GetPosition());
        }
    }

    public override void OnFinish()
    {
        if (target != null && target.m_pkGeActor != null)
        {
            target.m_pkGeActor.HideActor(false);
            target.m_pkGeActor.SetShadowVisible(target.CurrentBeScene.currentGeScene, true);
        }
        if (puppet != null)
        {
            if (puppet.m_pkGeActor != null)
            {
                puppet.m_pkGeActor.HideActor(true);
                puppet.m_pkGeActor.SetShadowVisible(target.CurrentBeScene.currentGeScene, false);
            }
            puppet.buffController.TryAddBuff(66, -1);//添加隐身buff
            puppet.DoDead(true);
            puppet = null;
        }
    }
}
