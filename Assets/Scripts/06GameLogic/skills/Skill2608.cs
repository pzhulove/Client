using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill2608 : BeSkill
{
    List<int> bulletResIdList = new List<int>();
    protected int m_EffectOffset = 2000;            //技能摇杆特效位置偏移

    IBeEventHandle handler;
    public Skill2608(int sid, int skillLevel) : base(sid, skillLevel)
    {
    }

    public override void OnInit()
    {
        bulletResIdList.Clear();
        for(int i = 0; i < skillData.ValueB.Count; ++i)
        {
            bulletResIdList.Add(TableManager.GetValueFromUnionCell(skillData.ValueB[i], level));
        }
        m_EffectOffset = TableManager.GetValueFromUnionCell(skillData.ValueA[0], level);
    }

    public override void OnStart()
    {
        var offset = VInt2.zero;
        offset.x = owner.GetFace() ? -m_EffectOffset * GlobalLogic.VALUE_10 : m_EffectOffset * GlobalLogic.VALUE_10;
        SetJoysticEffectOffset(offset);

        RemoveHandle();

        if (owner != null)
        {
            handler = owner.RegisterEventNew(BeEventType.onAfterGenBullet, (args) =>
            {
                SetInnerState(InnerState.LAUNCH);

                BeProjectile project = args.m_Obj as BeProjectile;
                if (project != null && bulletResIdList.Contains(project.m_iResID))
                {
                    VInt3 projectPos = project.GetPosition();
                    if (owner.GetFace())
                        projectPos.x = effectPos.x;
                    else
                        projectPos.x = effectPos.x;
                    projectPos.y = effectPos.y;

                    project.SetPosition(projectPos);
                    project.SetFace(owner.GetFace(), true);
                    RemoveHandle();
                }
            });
        }
    }

    public override void OnFinish()
    {
        RemoveHandle();
    }

    public override void OnCancel()
    {
        RemoveHandle();
    }

    void RemoveHandle()
    {
        if (handler != null)
        {
            handler.Remove();
            handler = null;
        }
    }
}
