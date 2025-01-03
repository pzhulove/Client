using UnityEngine;
using System.Collections.Generic;

/*
 * 旋转一定角度{ValueA}，可以设置旋转时间{ValueB}，旋转方向{ValueC}，攻击距离{ValueD}，攻击范围角度{ValueE}，攻击间隔{ValueF}，击中时触发效果{ValueG}
*/
public class Mechanism52 : BeMechanism
{
    public int rTime;           //旋转一次的时间
    public int orientation;     //顺时针1，逆时针-1
    int angle;                  //旋转一次的角度
    int distance;               //攻击距离
    int interval;               //攻击间隔
    int attackId;               //击中时触发效果

    public bool rotateEnd;
    VFactor minDot;             //根据攻击范围的角度计算的最小点积
    VFactor angleAcc;
    VInt3 startDir;
    int realDir = 0;            //真正的旋转方向
    int rotateTimer = 0;
    List<BeActor> targets;
    Dictionary<BeActor, int> dicHurtCD;
    int findTargetsTimer = 0;

#if !LOGIC_SERVER
    Transform ownerTrans;
#endif

    public Mechanism52(int mid, int lv) : base(mid, lv) { }

    public override void OnReset()
    {
        rotateEnd = false;
        if (targets != null) targets.Clear();
        if (dicHurtCD != null) dicHurtCD.Clear();
        findTargetsTimer = 0;
    }

    public override void OnInit()
    {
        angle = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        rTime = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        orientation = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
        distance = TableManager.GetValueFromUnionCell(data.ValueD[0], level);
        var angleRange = TableManager.GetValueFromUnionCell(data.ValueE[0], level);
        interval = TableManager.GetValueFromUnionCell(data.ValueF[0], level);
        int hard = owner.CurrentBeBattle.dungeonManager.GetDungeonDataManager().id.diffID;
        attackId = TableManager.GetValueFromUnionCell(data.ValueG[hard], level);

        var radian = VFactor.pi * angleRange / 180;
        minDot = IntMath.cos(radian.nom, radian.den);

        targets = new List<BeActor>();
        dicHurtCD = new Dictionary<BeActor, int>();
    }

    public override void OnStart()
    {
        angleAcc = new VFactor(0);
        if (orientation == 0)
        {
            realDir = FrameRandom.InRange(0, 2) * 2 - 1;
        }
        else
        {
            realDir = orientation;
        }

        startDir = owner.GetFace() ? -VInt3.right : VInt3.right;

        rotateTimer = 0;
        rotateEnd = false;

#if !LOGIC_SERVER
        ownerTrans = owner.m_pkGeActor.GetEntityNode(GeEntity.GeEntityNodeType.Charactor).transform;
#endif
    }

    void FindTargets()
    {
        targets.Clear();
        owner.CurrentBeScene.FindTargets(targets, owner, VInt.Float2VIntValue(100f));

        for (int i = 0; i < targets.Count; i++)
        {
            var target = targets[i];
            if (target != null)
            {
                if (!dicHurtCD.ContainsKey(target))
                {
                    dicHurtCD.Add(target, 0);
                }
            }
        }
    }

    public override void OnUpdate(int deltaTime)
    {
        if (!rotateEnd)
        {
            if (owner != null)
            {
                if (rotateTimer < rTime)
                {
                    rotateTimer += deltaTime;
                    angleAcc = new VFactor(rotateTimer * angle * realDir, rTime);
                    var r = VFactor.pi * angleAcc / 180;
                    var arrow = startDir.RotateY(ref r);    //毒柱的虚拟指向箭头
                    arrow.y = arrow.z;
                    arrow.z = 0;

                    findTargetsTimer -= deltaTime;
                    if (findTargetsTimer <= 0)
                    {
                        FindTargets();
                        findTargetsTimer = 300;
                    }

                    for (int i = 0; i < targets.Count; i++)
                    {
                        var target = targets[i];
                        if (target != null && !target.IsDead())
                        {
                            dicHurtCD[target] += deltaTime;
                            if (dicHurtCD[target] > interval)
                            {
                                var vec = target.GetPosition() - owner.GetPosition();
                                if (vec.magnitude < distance)
                                {
                                    var a = vec.NormalizeTo((int)IntMath.kIntDen);
                                    var b = arrow.NormalizeTo((int)IntMath.kIntDen);
                                    var dot = new VFactor(VInt3.Dot(a, b), GlobalLogic.VALUE_10000 * GlobalLogic.VALUE_10000);
                                    if (dot >= minDot)
                                    {
                                        var hitPos = owner.GetPosition();
                                        hitPos.z += VInt.one.i;
                                        owner._onHurtEntity(target, hitPos, attackId);
                                        dicHurtCD[target] = 0;
                                    }
                                }
                            }
                        }
                    }

#if !LOGIC_SERVER
                    if (ownerTrans != null)
                        ownerTrans.eulerAngles = Vector3.up * angleAcc.single;
#endif
                }
                else
                {
                    rotateEnd = true;
#if !LOGIC_SERVER
                    if (ownerTrans != null)
                        ownerTrans.eulerAngles = Vector3.zero;
#endif
                }
            }
        }
    }

    public void Stop()
    {
        rotateEnd = true;

        if (targets != null)
            targets.Clear();

        if (dicHurtCD != null)
            dicHurtCD.Clear();

#if !LOGIC_SERVER
        if (ownerTrans != null)
            ownerTrans.eulerAngles = Vector3.zero;
#endif
    }

    public override void OnFinish()
    {
        Stop();

        targets = null;
        dicHurtCD = null;
    }
}
