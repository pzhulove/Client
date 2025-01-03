using FlatBuffers;
using UnityEngine;

/// <summary>
/// 机制结束的时候 将角色瞬移到之前保存的不在阻挡的位置
/// </summary>
public class Mechanism1121 : BeMechanism
{
    protected VInt3 m_RecordNotInBlockPos;
    protected bool m_IsInBlock = false;

    public Mechanism1121(int mid, int lv) : base(mid, lv) { }

    public override void OnReset()
    {
        m_RecordNotInBlockPos = VInt3.zero;
        m_IsInBlock = false;
    }

    public override void OnStart()
    {
        base.OnStart();
        m_IsInBlock = false;
        RecordPos();
    }

    public override void OnUpdate(int deltaTime)
    {
        base.OnUpdate(deltaTime);
        RecordPos();
    }

    public override void OnFinish()
    {
        base.OnFinish();
        RestorePos();
    }

    protected void RecordPos()
    {
        if (m_IsInBlock)
            return;
        if (owner.CurrentBeScene == null)
            return;
        var pos = owner.GetPosition();
        if (owner.CurrentBeScene.IsInBlockPlayer(pos))
        {
            m_IsInBlock = true;
            return;
        }
        m_RecordNotInBlockPos = pos;
    }

    protected void RestorePos()
    {
        if (owner.CurrentBeScene == null)
            return;
        //如果机制结束时 自己不在阻挡里面则不作处理
        if (!owner.CurrentBeScene.IsInBlockPlayer(owner.GetPosition()))
            return;
        if (owner.CurrentBeScene.IsInBlockPlayer(m_RecordNotInBlockPos))
        {
            Logger.LogErrorFormat("发生严重错误 记录的坐标处于阻挡中:{0}", m_RecordNotInBlockPos);
            var newPos = BeAIManager.FindStandPositionNew(m_RecordNotInBlockPos, owner.CurrentBeScene, false, false, 50);
            owner.SetPosition(newPos);
            return;
        }
        owner.SetPosition(m_RecordNotInBlockPos);
    }
}

