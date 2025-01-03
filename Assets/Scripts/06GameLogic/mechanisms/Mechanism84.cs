using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 抛物线子弹机制
public class Mechanism84 : BeMechanism
{
    int entityId;
    int duration;
    VInt offsetX;
    VInt offsetZ;
    VInt moveSpeedZ;
    
    public Mechanism84(int sid, int skillLevel) : base(sid, skillLevel) { }
    public override void OnInit()
    {
        entityId = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        duration = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        offsetX = VInt.NewVInt(TableManager.GetValueFromUnionCell(data.ValueC[0], level), GlobalLogic.VALUE_1000);
        offsetZ = VInt.NewVInt(TableManager.GetValueFromUnionCell(data.ValueC[1], level), GlobalLogic.VALUE_1000);
        moveSpeedZ = VInt.NewVInt(TableManager.GetValueFromUnionCell(data.ValueD[0], level), GlobalLogic.VALUE_1000);
    }

    public override void OnStart()
    {
        if (owner == null)
            return;

        _createBullet();
    }

    private void _createBullet()
    {
        var target = owner.aiManager.aiTarget;
        if (target != null)
        {
            var projectile = owner.AddEntity(entityId, VInt3.zero) as BeProjectile;
            if (projectile != null)
            {
                projectile.SetFace(owner.GetFace());
                var birthPos = _getBirthPos();
                projectile.SetPosition(birthPos);

                var vec = target.GetPosition() - birthPos;
                projectile.SetMoveSpeedX(vec.x * 1000 / duration);
                projectile.SetMoveSpeedY(vec.y * 1000 / duration);
                projectile.SetMoveSpeedZ(moveSpeedZ.i);
                int acc = 2 * (moveSpeedZ.i * duration / 1000 + offsetZ.i) * 1000 / duration * 1000 / duration;
                projectile.SetMoveSpeedZAcc(acc);

                if (target.m_pkGeActor != null)
                    target.m_pkGeActor.CreateEffect(8, target.GetPosition().vec3, false, duration / 1000f);//预警圈
            }
        }
    }

    private VInt3 _getBirthPos()
    {
        int x = owner.GetPosition().x + (owner.GetFace() ? -offsetX.i : offsetX.i);
        int y = owner.GetPosition().y;
        int z = owner.GetPosition().z + offsetZ.i;
        var pos = new VInt3(x, y, z);
        return pos;
    }
}
