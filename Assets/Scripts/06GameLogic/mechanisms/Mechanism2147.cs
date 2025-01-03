using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * 黑洞
*/
public class Mechanism2147 : BeMechanism {

    VInt radius = 2 * VInt.one.i;
    VInt speed = 2 * VInt.one.i;
    int delayTime = 1200;
    float sizeSpeed = 5f;
    bool sizeChanged = false;
    int cnt = 0;
    bool absorbFlag = false;
    int absorbCheckRadius = 0;
    int absorbBuffID = 0;

    int offsetX = 0;
    int offsetY = 0;

    public Mechanism2147(int mid, int lv) : base(mid, lv)
    {
    }

    public override void OnInit()
    {
        radius = VInt.NewVInt(TableManager.GetValueFromUnionCell(data.ValueA[0], level), GlobalLogic.VALUE_1000);
        speed = VInt.NewVInt(TableManager.GetValueFromUnionCell(data.ValueB[0], level), GlobalLogic.VALUE_1000);
        sizeChanged = TableManager.GetValueFromUnionCell(data.ValueC[0], level) == 1;
        if (data.ValueD.Count > 0)
        {
            absorbFlag = TableManager.GetValueFromUnionCell(data.ValueD[0],level) == 1 ? true:false;
            absorbCheckRadius = TableManager.GetValueFromUnionCell(data.ValueD[1], level);
        }
        if (data.ValueE.Count > 0)
        {
            absorbBuffID = TableManager.GetValueFromUnionCell(data.ValueE[0], level);
        }
        if(data.ValueF.Count > 0)
        {
            offsetX = TableManager.GetValueFromUnionCell(data.ValueF[0], level);
        }
        if(data.ValueG.Count > 0)
        {
            offsetY = TableManager.GetValueFromUnionCell(data.ValueG[0], level);
        }
       
    }

    public override void OnStart()
    {
        cnt = 0;

        var r = GlobalLogic.VALUE_1000;
        var s = GlobalLogic.VALUE_1000;
        for (int i = 0; i < owner.MechanismList.Count; i++)
        {
            var m = owner.MechanismList[i] as Mechanism117;
            if (m != null)
            {
                r += m.radiusRate;
                s += m.speedRate;
            }
        }
        radius = radius.i * VFactor.NewVFactor(r, GlobalLogic.VALUE_1000);
        speed = speed.i * VFactor.NewVFactor(s, GlobalLogic.VALUE_1000);
    }

    public override void OnUpdate(int deltaTime)
    {
        if (owner != null)
        {
            List<BeActor> targets = GamePool.ListPool<BeActor>.Get();
            owner.CurrentBeScene.FindTargets(targets, owner, radius);
            for (int i = 0; i < targets.Count; ++i)
            {
                var target = targets[i];

                if (!target.stateController.CanMove()|| target.stateController.CanNotAbsorbByBlockHole())
                    continue;

                var absorbPos = owner.GetPosition();
            
                //向左
                if (owner.GetFace()) 
                {
                    absorbPos.x = absorbPos.x - offsetX;
                } else
                {
                    absorbPos.x = absorbPos.x + offsetX;
                }
            
                absorbPos.y = absorbPos.y + offsetY;
               
                var pos = target.GetPosition();
                if (!IsNeedAbsorb(absorbPos, pos))
                    continue;
                if (!HaveAbsorbBuff(target))
                    continue;
                VInt2 del = new VInt2(absorbPos.x - pos.x, absorbPos.y - pos.y);
                VInt2 vec = del.NormalizeTo(speed.i);
                //修改抓取
                if (target.stateController.CanMoveX())
                {
                    target.extraSpeed.x = vec.x;
                }

                if (target.stateController.CanMoveY())
                {
                    target.extraSpeed.y = vec.y;
                }

                if (sizeChanged)
                {
                    ChangeSize(target);
                }
            }

            GamePool.ListPool<BeActor>.Release(targets);
        }
    }

    private List<BeActor> actorList = new List<BeActor>();
    private void ChangeSize(BeActor actor)
    {
        if (!actorList.Contains(actor))
        {
            actorList.Add(actor);
        }
        cnt++;
#if !SERVER_LOGIC
        actor.m_pkGeActor.SetActorNodeScale(Mathf.Lerp(1,0,0.03f*cnt*sizeSpeed));
#endif
        VInt3 pos = new VInt3(actor.GetPosition().x, actor.GetPosition().y, VInt.Clamp(new VInt(Mathf.Lerp(0,1,0.03f * cnt)), 0, VInt.one).i);
        actor.SetPosition(pos, true);
    }

    private void ResizeModelScale()
    {
        if (sizeChanged)
        {
            for (int i = 0; i < actorList.Count; i++)
            {
                actorList[i].buffController.TryAddBuff(68, -1);
            }

            owner.CurrentBeScene.DelayCaller.DelayCall(delayTime,()=>
            {
                for (int i = 0; i < actorList.Count; i++)
                {
                    if (actorList[i] == null) continue;
                    actorList[i].buffController.RemoveBuff(68);
                }
#if !SERVER_LOGIC
                for (int i = 0; i < actorList.Count; i++)
                {
                    if (actorList[i] == null || actorList[i].m_pkGeActor==null) continue;
                    actorList[i].m_pkGeActor.ResetActorNodeScale();
                }
#endif
                actorList.Clear();
            });          
           
        }
    }

    public override void OnDead()
    {
        base.OnDead();
        ResizeModelScale();
    }


    public override void OnFinish()
    {
        base.OnFinish();
        ResizeModelScale();

    }

    protected bool IsNeedAbsorb(VInt3 ownerPos, VInt3 targetPos)
    {
        if (!absorbFlag)
            return true;
        if ((targetPos - ownerPos).magnitude <= absorbCheckRadius)
            return false;
        return true;
    }

    //只吸取上有指定Buff的怪物
    protected bool HaveAbsorbBuff(BeActor target)
    {
        if (absorbBuffID == 0)
            return true;
        if (target.buffController.HasBuffByID(absorbBuffID) == null)
            return false;
        return true;
    }
}
