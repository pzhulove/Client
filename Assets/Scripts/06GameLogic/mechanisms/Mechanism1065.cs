using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 改变被击特效和伤害冒字的位置的位置
/// </summary>
public class Mechanism1065 : BeMechanism
{
    public Mechanism1065(int mid, int lv) : base(mid, lv) { }

    private VInt3 hitEffectPos = VInt3.zero;
    private VInt3 hitNumberPos = VInt3.zero;

    public override void OnInit()
    {
        base.OnInit();

        hitEffectPos = VInt3.zero;
        hitNumberPos = VInt3.zero;
        
        for(int i = 0; i < data.ValueA.Count; i++)
        {
            hitEffectPos[i] = TableManager.GetValueFromUnionCell(data.ValueA[i], level);
        }

        for(int i = 0; i < data.ValueB.Count; i++)
        {
            hitNumberPos[i] = TableManager.GetValueFromUnionCell(data.ValueB[i], level);
        }
    }

    public override void OnStart()
    {
        base.OnStart();
        handleA = owner.RegisterEventNew(BeEventType.onChangeBeHitEffectPos, args => 
        {
            if(hitEffectPos != VInt3.zero)
            {
                /*var posArr = (VInt3[])args[0];
                posArr[0] += hitEffectPos;*/
                args.m_Vint3 += hitEffectPos;
            }
        });

        handleB = owner.RegisterEventNew(BeEventType.onChangeBeHitNumberPos, args => 
        {
#if !LOGIC_SERVER
            if (hitNumberPos != VInt3.zero)
            {
                /*var posArr = (Vec3[])args[0];
                posArr[0] += hitNumberPos.vec3;*/
                args.m_Vector += hitNumberPos.vector3;
            }
#endif
        });
    }
}
