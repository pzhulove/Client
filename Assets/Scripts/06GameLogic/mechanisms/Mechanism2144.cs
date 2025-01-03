using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 残影特效
/// </summary>
public class Mechanism2144 : BeMechanism
{
    private int _shadowLiftTime;
    private int shadowInterval = 200;

#if !LOGIC_SERVER
    private int posIndex = 0;
    private Vec3[] effectPosArr = new Vec3[4];
#endif

    public Mechanism2144(int mid, int lv) : base(mid, lv) { }

    public override void OnInit()
    {
        base.OnInit();
        _shadowLiftTime = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        shadowInterval = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
    }

    public override void OnReset()
    {
#if !LOGIC_SERVER
        posIndex = 0;
        effectPosArr = new Vec3[4];
#endif
        timer = 0;
        shadowTime = 0;
}

private int tmpBuffInfoID;
    public override void OnStart()
    {
        base.OnStart();
    }

    #region 创建残影相关
    private int timer = 0;
    private int shadowTime = 0;

    public override void OnUpdate(int deltaTime)
    {
        base.OnUpdate(deltaTime);
        shadowTime += deltaTime;
        if(owner.sgGetCurrentState() != (int)ActionState.AS_IDLE)
        {
            if (shadowTime >= shadowInterval)
            {
                CreateSnapEffect();
                shadowTime = 0;
            }
        } 
    }

    private void CreateSnapEffect()
    {
#if !LOGIC_SERVER
        VInt3 pos = owner.GetPosition();
        pos.z = owner.GetPosition().z + 10000;
        AddEffectPos(pos.vec3);
        owner.m_pkGeActor.CreateSnapshot(Color.white, _shadowLiftTime / 1000.0f, "Shader/Materials/Snapshot_Sanda_Beidongjuexing.mat");
#endif
    }


    private void AddEffectPos(Vec3 pos)
    {
#if !LOGIC_SERVER
        if (posIndex >= effectPosArr.Length)
            posIndex = 0;
        effectPosArr[posIndex] = pos;
        posIndex++;
#endif
    }

#endregion
}
