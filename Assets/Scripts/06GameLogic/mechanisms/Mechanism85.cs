using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 全场伤害机制
/// </summary>
public class Mechanism85 : BeMechanism
{
    

    //private bool hurtSelf = false;
    private int hurtID = 0;
    private int height = -1;
    private bool onlyMainActor = false;

    private bool hurtIDByDis = false;
    private VInt[] dis;
    private int[] hurtIDs;

    public Mechanism85(int mid, int lv) : base(mid, lv){}

    public override void OnReset()
    {
        height = -1;
        dis = null;
        hurtIDs = null;
    }
    public override void OnInit()
    {
        base.OnInit();
        hurtID = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        if(data.ValueB.Length > 0)
        {
            height = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        }
        onlyMainActor = TableManager.GetValueFromUnionCell(data.ValueC[0], level) == 1;


        hurtIDByDis = TableManager.GetValueFromUnionCell(data.ValueD[0], level)==1;
        dis = new VInt[data.ValueE.Count];
        for (int i = 0; i < dis.Length; i++)
        {
            dis[i] = TableManager.GetValueFromUnionCell(data.ValueE[i], level);
        }
        hurtIDs = new int[data.ValueF.Count];
        for (int i = 0; i < hurtIDs.Length; i++)
        {
            hurtIDs[i] = TableManager.GetValueFromUnionCell(data.ValueF[i], level);
        }
    }

    public override void OnStart()
    {
        base.OnStart();
        List<BeEntity> actorList = GamePool.ListPool<BeEntity>.Get();
        owner.CurrentBeScene.GetEntitys2(actorList);
        for (int i = 0; i < actorList.Count; i++)
        {
            BeActor actor = actorList[i] as BeActor;
            if (actor != null && actor.GetEntityData() != null && !actor.GetEntityData().isPet)
            {
                if (actor.GetPID() == owner.GetPID() || actor.IsDead()||actor.GetCamp()==owner.GetCamp()) continue;
                if (onlyMainActor)
                {
                    if (!actor.isMainActor) continue;
                }
                var hitPos = actor.GetPosition();
                if(height != -1)
                {
                    if (actor.m_cpkCurEntityActionFrameData != null &&
                        actor.GetPosition().z <= height)
                    {
                        hitPos.z += VInt.one.i;
                        owner._onHurtEntity(actor, hitPos, hurtID);
                    }
                }
                else
                {
                    hitPos.z += VInt.one.i;
                    if (hurtIDByDis)
                    {
                        hurtID = GetHurtIDByDis(actor);
                    }
                    owner._onHurtEntity(actor, hitPos, hurtID);
                }
                
            }
        }
        GamePool.ListPool<BeEntity>.Release(actorList);
    }
    private int GetHurtIDByDis(BeActor actor)
    {
        int dis = (owner.GetPosition() - actor.GetPosition()).magnitude2D;

        for (int i = 0; i < this.dis.Length; i++)
        {
            if (dis < this.dis[i]) return hurtIDs[i];
        }
        return 0;
    }

}
 