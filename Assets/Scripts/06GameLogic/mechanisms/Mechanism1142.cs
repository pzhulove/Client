/*using System.Collections.Generic;
using GameClient;

/// <summary>
/// 警卫员机制
/// 1）警卫员不会离开玩家太远的距离。当警卫员距离玩家距离超过6000码时，警卫员会返回玩家身边；（这个参数希望由策划进行设置）（参考“命令·保护”Skill2104）
/// 2）当附近有敌人时，会主动攻击敌人。
/// 3）当近身攻击不满足条件释放时，会尽量和敌人保持同一直线后进行远程攻击。
/// 4）攻击目标依次为：有集火标记的单位＞范围内最近的敌方单位
///
/// </summary>
public class Mechanism1142 : LockAttackMechanism
{
    private int _entityId = 64016;
    private BeActor m_AITarget;
    
    public Mechanism1142(int mid, int lv) : base(mid, lv) { }

    protected override int LockMarkMechanismID => 6211;

    public override void OnStart()
    {
        base.OnStart();
        InitEvent();

        SetFollowActor();
        UpdateAITarget();
    }

    // 强制更随
    private void SetFollowActor()
    {
        if (owner.aiManager == null)
            return;
        
        owner.aiManager.SetForceFollow(true);
        owner.aiManager.followTarget = GetTopOwner();
    }
    
    private void InitEvent()
    {
        var actor = owner.GetTopOwner(owner);
        if(actor == null)
            return;
        
        sceneHandleA = owner.CurrentBeScene.RegisterEvent(BeEventSceneType.onMonsterDead, OnMonsterDead);        
    }

    private void OnMonsterDead(object[] args)
    {
        if(m_AITarget == null)
            return;
        
        if(args[0] == null)
            return;
        
        var monster = args[0] as BeActor;
        if (monster == m_AITarget)
        {
            m_AITarget = null;
            UpdateAITarget();            
        }
    }



    protected override void OnForceTargetChange()
    {
        UpdateAITarget();
    }

    private void UpdateAITarget()
    {
        if (owner.aiManager == null)
            return;
        
        m_AITarget = FindTarget();
        if (m_AITarget == null)
        {
            owner.aiManager.targetUnchange = false;
        }
        else
        {
            owner.aiManager.SetTarget(m_AITarget, true);
            owner.aiManager.ResetAction();
            owner.aiManager.ResetDestinationSelect();
            
        }
    }

    private BeActor FindTarget()
    {
        // 集火攻击目标优先
        return ForceTarget;
    }
}*/