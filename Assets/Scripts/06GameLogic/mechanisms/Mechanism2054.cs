using System;
using System.Collections.Generic;
//实体轨迹机制 随机目标点
public class Mechanism2054 : BeMechanism
{
    int flyTime = 0;
    int targetDist = 0;
    int angle = 0;
    VInt moveSpeedZ = VInt.zero;
    int targetRandomRange = 0;
    int entityId = 0;
    VInt offsetX = VInt.zero;
    VInt offsetZ = VInt.zero;
    public Mechanism2054(int mid, int lv) : base(mid, lv) { }
    public override void OnInit()
    {
        base.OnInit();
        entityId = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        angle = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        targetDist = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
        flyTime = TableManager.GetValueFromUnionCell(data.ValueD[0], level);
        moveSpeedZ = TableManager.GetValueFromUnionCell(data.ValueE[0], level);
        targetRandomRange = TableManager.GetValueFromUnionCell(data.ValueF[0], level);
        offsetX = VInt.NewVInt(TableManager.GetValueFromUnionCell(data.ValueG[0], level), GlobalLogic.VALUE_1000);
        offsetZ = VInt.NewVInt(TableManager.GetValueFromUnionCell(data.ValueG[1], level), GlobalLogic.VALUE_1000);
        if(flyTime <= 0)
        {
            Logger.LogErrorFormat("Mechanism id {0} flyTime is invalid data",this.mechianismID);
            flyTime = 100;
        }
    }

    public override void OnStart()
    {
        if (owner == null)
            return;
        _createBullet();
    }
    private void _createBullet()
    {
        var targetPos = GetTargetPosition();
        var projectile = owner.AddEntity(entityId, VInt3.zero) as BeProjectile;
        if (projectile != null)
        {
            var srcPos = _getBirthPos();
            projectile.SetPosition(srcPos);
            var vec = targetPos - srcPos;
            projectile.SetMoveSpeedX(vec.x * 1000 / flyTime);
            projectile.SetMoveSpeedY(vec.y * 1000 / flyTime);
            projectile.SetMoveSpeedZ(moveSpeedZ.i);
            int acc = 2 * (moveSpeedZ.i * flyTime / 1000 + offsetZ.i) * 1000 / flyTime * 1000 / flyTime;
            projectile.SetMoveSpeedZAcc(acc);
            projectile.onCollideDie = true;
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
    private VInt3 GetTargetPosition()
    {
        if (owner.aiManager != null)
        {
            var target = owner.aiManager.aiTarget;
            if (target != null)
            {
                var pos = target.GetPosition();
                pos.z = 0;
                var xOffset = owner.FrameRandom.InRange(-targetRandomRange, targetRandomRange);
                var yOffset = owner.FrameRandom.InRange(-targetRandomRange, targetRandomRange);
                pos.x = pos.x + xOffset;
                pos.y = pos.y + yOffset;
                return pos;
            }
        }
        VFactor fAngle = VFactor.pi * angle / 180;
        VFactor fSinA = IntMath.sin(fAngle.nom, fAngle.den);
        VFactor fCosA = IntMath.cos(fAngle.nom, fAngle.den);
        int yTargetOffset = targetDist * fSinA;
        int xTargetOffset = targetDist * fCosA;
        if(owner.GetFace())
        {
            yTargetOffset = -yTargetOffset;
            xTargetOffset = -xTargetOffset;
        }
        var randomXOffset  = owner.FrameRandom.InRange(-targetRandomRange, targetRandomRange);
        var randomYOffset  = owner.FrameRandom.InRange(-targetRandomRange, targetRandomRange);
        var targetPos = owner.GetPosition();
        targetPos.y = targetPos.y + yTargetOffset + randomYOffset;
        targetPos.x = targetPos.x + xTargetOffset + randomXOffset;
        targetPos.z = 0;
        targetPos = BeAIManager.FindStandPositionNew(targetPos, owner.CurrentBeScene,false,false,40);
        return targetPos;
    }
}
