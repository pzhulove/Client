using System.Collections.Generic;
using UnityEngine;
using System;
using GameClient;

/// <summary>
/// 血气之怒 技能特写
/// </summary>
public class Skill1605 : BeSkill
{
    public Skill1605(int sid, int skillLevel) : base(sid, skillLevel)
    {

    }

    protected int[] m_ResIdArr = new int[2] { 60259, 60203 };
    protected VInt3 m_Offset = new VInt3(15500,0,10000);//实体位置偏移

    public override void OnStart()
    {
        RegisterEvent();
    }

    /// <summary>
    /// 监听事件
    /// </summary>
    protected void RegisterEvent()
    {
        handleA = owner.RegisterEventNew(BeEventType.onAfterGenBullet, OnAfterGenBullet);
    }

    /// <summary>
    /// 监听实体创建
    /// </summary>
    protected void OnAfterGenBullet(BeEvent.BeEventParam args)
    {
        BeProjectile projectile = args.m_Obj as BeProjectile;
        if (projectile == null)
            return;
        if (Array.IndexOf(m_ResIdArr, projectile.m_iResID) < 0)
            return;
        var pos = owner.GetPosition();
        pos.x += owner.GetFace() ? -m_Offset.x : m_Offset.x;
        pos.z += m_Offset.z;
        projectile.SetPosition(pos);
    }
}
