using UnityEngine;
using System.Collections;

public class Buff110801 : BeBuff
{
    int weaponType = 0;
    IBeEventHandle handler = null;

    public Buff110801(int bi, int buffLevel, int buffDuration, int attack = 0) : base(bi, buffLevel, buffDuration, attack)
    {

    }

    public override void OnReset()
    {
        weaponType = 0;
    }

    public override bool CanAdd(BeActor target)
    {
        return (target.GetWeaponType() == weaponType);
    }

    public override void OnInit()
    {
        weaponType = TableManager.GetValueFromUnionCell(buffData.ValueA[0], level);
    }

    public override void OnStart()
    {
        RemoveHandler();

        handler = owner.RegisterEventNew(BeEventType.onHitCritical, args =>
        //handler = owner.RegisterEvent(BeEventType.onHitCritical, (object[] args) =>
        {
            VInt3 pos = args.m_Vint3;

            //Logger.LogErrorFormat("listen onHitCritical!!!!!!!");

            owner.CurrentBeScene.currentGeScene.CreateEffect(1020, pos.vec3, owner.GetFace());
        });

    }

    public override void OnFinish()
    {
        RemoveHandler();
    }

    void RemoveHandler()
    {
        if (handler != null)
        {
            handler.Remove();
            handler = null;
        }
    }
}
