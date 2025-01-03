using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//寸拳
public class Skill3206 : BeSkill
{
    int delay;
    VInt offsetX;
    VInt offsetZ;

    IBeEventHandle handle1;
    IBeEventHandle handle2;
    int entityId = 60506;
    string frameFlag = "320601";

    public Skill3206(int sid, int skillLevel) : base(sid, skillLevel) { }

    public override void OnInit()
    {
        delay = TableManager.GetValueFromUnionCell(skillData.ValueA[0], level);
        offsetX = VInt.NewVInt(TableManager.GetValueFromUnionCell(skillData.ValueB[0], level), GlobalLogic.VALUE_1000);
        offsetZ = VInt.NewVInt(TableManager.GetValueFromUnionCell(skillData.ValueB[1], level), GlobalLogic.VALUE_1000);
    }

    public override void OnStart()
    {
        handle1 = owner.RegisterEventNew(BeEventType.onHitOther, args =>
        {
            RemoveHandles();
            owner.delayCaller.DelayCall(delay, () =>
            {
                CreateEntity();
                owner.AddShock(new ShockData(0.5f, 40f, 0.01f, 0, 0));
            });
        });
        handle2 = owner.RegisterEventNew(BeEventType.onSkillCurFrame, args =>
        {
            // string flag = (string)args[0];
            string flag = args.m_String;
            if (flag.Equals(frameFlag))
            {
                owner.AddShock(new ShockData(0.9f, 40f, 0.01f, 0, 0));
            }
        });
    }

    void CreateEntity()
    {
        if (owner != null && !owner.IsDead())
        {
            var pos = owner.GetPosition();
            pos.x += (owner.GetFace() ? -1 : 1) * offsetX.i;
            pos.z += offsetZ.i;
            owner.AddEntity(entityId, pos);
        }
    }

    public override void OnCancel()
    {
        RemoveHandles();
    }

    public override void OnFinish()
    {
        RemoveHandles();
    }

    void RemoveHandles()
    {
        if (handle1 != null)
        {
            handle1.Remove();
            handle1 = null;
        }
        if (handle2 != null)
        {
            handle2.Remove();
            handle2 = null;
        }
    }
}
