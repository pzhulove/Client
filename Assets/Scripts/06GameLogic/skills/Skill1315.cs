using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill1315 : BeSkill
{
    protected IBeEventHandle m_PassDoorHandle = null;                       //监听过门
    protected IBeEventHandle m_PassTowerHandle = null;                      //监听爬塔
    protected IBeEventHandle m_MonsterDeadHandle = null;
    private int entityID = 60515;
    private int monterID = 90000031;
    private int delayFindMonster = 3000;
    private bool hasPassed = false;   //是否已经过门
    public Skill1315(int sid, int skillLevel) : base(sid, skillLevel)
    { }

    public override void OnPostInit()
    {

    }

    public override void OnStart()
    {
        hasPassed = false;
        RemoveHandle();
        m_PassDoorHandle = owner.RegisterEventNew(BeEventType.onStartPassDoor, (args) =>
        {
            ClearDeadHandle();
        });

        m_PassTowerHandle = owner.RegisterEventNew(BeEventType.onDeadTowerEnterNextLayer, (args) =>
        {
            ClearDeadHandle();
        });

        owner.delayCaller.DelayCall(delayFindMonster, () =>
        {
            if (hasPassed) return;
            List<BeActor> list = GamePool.ListPool<BeActor>.Get();
            owner.CurrentBeScene.FindActorById2(list, monterID);
            if (list.Count > 0)
            {
                BeActor actor = list[0];
                if (actor != null)
                {
                    m_MonsterDeadHandle = actor.RegisterEventNew(BeEventType.onDead, eventParam =>
                    {
                        owner.AddEntity(entityID, actor.GetPosition());
                    });
                }
               
            }
            GamePool.ListPool<BeActor>.Release(list);
        });
    }

    protected void ClearDeadHandle()
    {
        hasPassed = true;
        if (m_MonsterDeadHandle != null)
        {
            m_MonsterDeadHandle.Remove();
            m_MonsterDeadHandle = null;
        }
    }

    protected void RemoveHandle()
    {
        if (m_MonsterDeadHandle != null)
        {
            m_MonsterDeadHandle.Remove();
            m_MonsterDeadHandle = null;
        }

        if (m_PassDoorHandle != null)
        {
            m_PassDoorHandle.Remove();
            m_PassDoorHandle = null;
        }

        if (m_PassTowerHandle != null)
        {
            m_PassTowerHandle.Remove();
            m_PassTowerHandle = null;
        }
    }
}
