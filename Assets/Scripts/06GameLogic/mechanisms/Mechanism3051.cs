using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 冰刃护体
/// </summary>
public class Mechanism3051 : BeMechanism
{
    public Mechanism3051(int mid, int lv) : base(mid, lv)
    {
    }

    private int m_EntityCount;
    private int m_EntityResID;
    private List<BeProjectile> m_ProjectileList;
    
    VInt dis = new VInt(2f);//实体旋转的当前半径
    VInt pi = VInt.zero;//实体旋转当前的角度
    VFactor v0 = VFactor.zero;//实体旋转的初始角度
    VInt rSpeed = new VInt(0.12f);//实体当前旋转的速度
    
    public override void OnInit()
    {
        base.OnInit();
        m_EntityCount = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        m_EntityResID = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        dis = new VInt(TableManager.GetValueFromUnionCell(data.ValueC[0], level) * 10);
        var speed = TableManager.GetValueFromUnionCell(data.ValueD[0], level);
        rSpeed = new VInt(3141.6f / (15f  * speed)); 
    }

    public override void OnStart()
    {
        m_ProjectileList = GamePool.ListPool<BeProjectile>.Get();
        base.OnStart();
        for (int i = 0; i < m_EntityCount; i++)
        {
            var entity = owner.AddEntity(m_EntityResID, VInt3.zero, level) as BeProjectile;
            entity.SetType(ProjectType.TIMING, 0);
            m_ProjectileList.Add(entity);
        }
        v0 = (VFactor.pi * 2) / m_ProjectileList.Count;
        StartRotate();
    }

    public override void OnUpdate(int deltaTime)
    {
        base.OnUpdate(deltaTime);
        StartRotate();
    }

    public override void OnFinish()
    {
        base.OnFinish();
        for (int i = 0; i < m_ProjectileList.Count; i++)
        {
            m_ProjectileList[i]?.DoDie();
        }
        GamePool.ListPool<BeProjectile>.Release(m_ProjectileList);
    }

    /// <summary>
    /// 实体转圈
    /// </summary>
    void StartRotate()
    {
        for (int i = 0; i < m_ProjectileList.Count; ++i)
        {
            var radian = pi.factor + v0 * i;
            VInt z = dis.i * IntMath.cos(radian.nom, radian.den);
            VInt x = dis.i * IntMath.sin(radian.nom, radian.den);
            m_ProjectileList[i].SetPosition(new VInt3(x.i + owner.GetPosition().x, z.i + owner.GetPosition().y, VInt.one.i));
#if !LOGIC_SERVER
            m_ProjectileList[i].m_pkGeActor.SetRotation(Quaternion.AngleAxis( 90 - Vector2.SignedAngle(new Vector2(x.scalar, z.scalar ), Vector2.right), Vector3.down));
#endif
        }
        pi += rSpeed;
    }
}
