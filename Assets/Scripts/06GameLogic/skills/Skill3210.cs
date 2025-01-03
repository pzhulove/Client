using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//闪电之舞
public class Skill3210 : BeSkill
{
    int number;             //攻击次数
    int intervel;           //攻击间隔
    VInt radius;            //寻找目标半径
    VInt backDis;           //身后中转点距离
    VInt height;            //攻击高度上限
    
    int pose1 = 32103;      //攻击动作
    int pose2 = 32104;      //蹲下动作（中转点用）
    int effectId = 32100;   //触发效果Id

    string effectPath1 = "Effects/Hero_Sanda/Eff_Sanda_shandianzhiwu/Prefab/Eff_Sanda_shandianzhiwu_shunyi";    //长
    string effectPath2 = "Effects/Hero_Sanda/Eff_Sanda_shandianzhiwu/Prefab/Eff_Sanda_shandianzhiwu_shunyi02";  //中
    string effectPath3 = "Effects/Hero_Sanda/Eff_Sanda_shandianzhiwu/Prefab/Eff_Sanda_shandianzhiwu_shunyi03";  //短
    string hitEffectPath = "Effects/Hero_Sanda/Eff_Sanda_shandianzhiwu/Prefab/Eff_Sanda_shandianzhiwu_beiji";   //击中特效

    VInt3 lastPos;          //上次位置
    VInt3 tempPos;          //中转点
    int timer;              //计时器
    int count;              //攻击次数统计
    bool hitOther;          //起手是否攻击到敌人
    GameClient.BeEvent.BeEventHandleNew handle1;
    GameClient.BeEvent.BeEventHandleNew handle2;
    List<BeActor> hitList = new List<BeActor>();//已经被攻击过的敌人列表

    public Skill3210(int sid, int skillLevel) : base(sid, skillLevel) { }
    
    //注意：有成长的填表数值，需要在PostInit里面获取
    public override void OnPostInit()
    {
        number = TableManager.GetValueFromUnionCell(skillData.ValueA[0], level);
        intervel = TableManager.GetValueFromUnionCell(skillData.ValueB[0], level);
        radius = VInt.NewVInt(TableManager.GetValueFromUnionCell(skillData.ValueC[0], level), GlobalLogic.VALUE_1000);
        backDis = radius.i / 2;
        height = VInt.NewVInt(TableManager.GetValueFromUnionCell(skillData.ValueD[0], level), GlobalLogic.VALUE_1000);
    }

    public override void OnStart()
    {
        ModifySkillParam();
        lastPos = owner.GetPosition();
        tempPos = VInt3.zero;
        timer = 0;
        count = number;
        hitOther = false;
        hitList.Clear();

        //handle1 = owner.RegisterEvent(BeEventType.onCollide, _OnCollide);
        handle1 = owner.RegisterEventNew(BeEventType.onCollide, _OnCollide);

#if !LOGIC_SERVER
        handle2 = owner.RegisterEventNew(BeEventType.onChangeHitEffect, (GameClient.BeEvent.BeEventParam beEventParam) =>
        {
            int hurtId = beEventParam.m_Int;
            if (hurtId == effectId)
            {
                beEventParam.m_String = hitEffectPath;
            }
        });
#endif
    }

    private void _OnCollide(GameClient.BeEvent.BeEventParam param)
    {
        var target = param.m_Obj as BeActor;
        if (target != null)
        {
            hitOther = true;
            timer = intervel;
        }
    }

    void ModifySkillParam()
    {
        int r = GlobalLogic.VALUE_1000;
        for (int i = 0; i < owner.MechanismList.Count; i++)
        {
            var m = owner.MechanismList[i] as Mechanism127;
            if (m != null)
            {
                number += m.attackNum;
                r += m.radiusRate;
            }
        }
        if (r != GlobalLogic.VALUE_1000)
        {
            radius = radius.i * VFactor.NewVFactor(r, GlobalLogic.VALUE_1000);
            backDis = radius.i / 2;
        }
    }

    void RestoreSkillParam()
    {
        number = TableManager.GetValueFromUnionCell(skillData.ValueA[0], level);
        radius = VInt.NewVInt(TableManager.GetValueFromUnionCell(skillData.ValueC[0], level), GlobalLogic.VALUE_1000);
        backDis = radius.i / 2;
    }

    void FindNextTarget()
    {
        var list = GetTargets();
        //没有攻击目标
        if (list.Count == 0)
        {
            count = 0;
            return;
        }
        //只找到1个攻击对象，并且上次攻击的就是这个对象，则移动到一个中转点
        else if (list.Count == 1 && hitList.Count >= 1 && list[0].Equals(hitList[hitList.Count - 1]))
        {
            if (count == 1)
                return;

            var targetPos = list[0].GetPosition();
            targetPos.z = 0;
            var vec = lastPos - targetPos;
            VInt3 pos;
            if (tempPos == VInt3.zero || (tempPos - owner.GetPosition()).magnitude > radius.i)
                pos = owner.GetPosition() + vec.NormalizeTo(backDis.i);
            else
                pos = tempPos;
            lastPos = owner.GetPosition();
            owner.SetPosition(pos);
            tempPos = pos;

            owner.SetFace(pos.x > lastPos.x);

            hitList.Clear();

            ((BeActorStateGraph)owner.GetStateGraph()).ExecuteSkill(pose2);

            CreateTrailEffect(pos, lastPos, 3800);
        }
        else
        {
            BeActor tempActor = null;
            int tempIndex = int.MaxValue;

            for (int i = 0; i < list.Count; i++)
            {
                int index = hitList.IndexOf(list[i]);
                //如果攻击目标在已经被攻击过的敌人列表里
                if (index != -1)
                {
                    if (index < tempIndex)
                    {
                        tempIndex = index;
                    }
                }
                else
                {
                    tempActor = list[i];
                    break;
                }
            }
            if (tempActor == null)
            {
                if (hitList.Count > tempIndex)
                {
                    tempActor = hitList[tempIndex];
                    hitList.RemoveAt(tempIndex);
                }
                else
                {
                    return;
                }
            }

            RefleshHitList();
            hitList.Add(tempActor);
            
            var hitPos = tempActor.GetPosition();
            //计算攻击目标左边和右边的点
            var leftPos = tempActor.GetPosition();
            leftPos.z = 0;
            leftPos.x -= 7000;
            var leftDis = (owner.GetPosition() - leftPos).magnitude;
            var rightPos = tempActor.GetPosition();
            rightPos.z = 0;
            rightPos.x += 7000;
            var rightDis = (owner.GetPosition() - rightPos).magnitude;
            //选择离玩家近的
            lastPos = owner.GetPosition();
            if (leftDis < rightDis)
            {
                hitPos.x -= GlobalLogic.VALUE_1000;
                owner.SetPosition(leftPos);
                owner.SetFace(false);
                CreateTrailEffect(leftPos, lastPos, 6600);
            }
            else
            {
                hitPos.x += GlobalLogic.VALUE_1000;
                owner.SetPosition(rightPos);
                owner.SetFace(true);
                CreateTrailEffect(rightPos, lastPos, 6600);
            }
            hitPos.z = VInt.one.i;
            owner._onHurtEntity(tempActor, hitPos, effectId);

            ((BeActorStateGraph)owner.GetStateGraph()).ExecuteSkill(pose1);
        }
    }

    void RefleshHitList()
    {
        //清理尸体
        for (int i = hitList.Count - 1; i >= 0; i--)
        {
            if (hitList[i].IsDead())
            {
                hitList.RemoveAt(i);
            }
        }
    }

    List<BeActor> GetTargets()
    {
        var list = new List<BeActor>();

        var targets = GamePool.ListPool<BeActor>.Get();
        owner.CurrentBeScene.FindTargets(targets, owner, radius);
        for (int i = 0; i < targets.Count; i++)
        {
            var target = targets[i];
            if (target != null && 
                !target.IsDead() && 
                target.GetPosition().z < height.i &&
                !target.stateController.HasBuffState(BeBuffStateType.INVINCIBLE) &&
                !owner.CurrentBeScene.IsInBlockPlayer(target.GetPosition()))
            {
                list.Add(target);
            }
        }
        GamePool.ListPool<BeActor>.Release(targets);

        //按距离排序
        list.Sort((a, b) =>
        {
            var dis1 = (owner.GetPosition() - a.GetPosition()).magnitude;
            var dis2 = (owner.GetPosition() - b.GetPosition()).magnitude;
            if (dis1 != dis2)
                return dis1.CompareTo(dis2);
            else
                return a.GetPID().CompareTo(b.GetPID());
        });

        return list;
    }

    void CreateTrailEffect(VInt3 start, VInt3 end, int zOffset)
    {
#if !LOGIC_SERVER
        if (count == number)
            return;
        
        start.z += zOffset;
        end.z += zOffset;
        start.y -= 3300;
        end.y -= 3300;

        var vec = end.vector3 - start.vector3;
        var angle = Vector3.Angle(Vector3.left, vec);
        var cross = Vector3.Cross(Vector3.left, vec);

        var y = cross.normalized.y;
        if (y != 0) angle *= y;

        //特效至少要带一点角度才好看
        if (80 < angle && angle <= 90)
            angle = 80;
        else if (90 < angle && angle < 100)
            angle = 100;
        else if (-90 <= angle && angle < -80)
            angle = -80;
        else if (-100 < angle && angle < -90)
            angle = -100;

        //不同的移动距离用不同长度的特效
        string path;
        var dis = (start - end).magnitude;
        if (dis > 32000)
            path = effectPath1;
        else if (dis > 16000)
            path = effectPath2;
        else
            path = effectPath3;

        var effect = owner.CurrentBeScene.currentGeScene.CreateEffect(path, 0.2f, start.vec3);
        effect.GetRootNode().transform.eulerAngles = Vector3.up * angle;
#endif
    }

    public override void OnUpdate(int iDeltime)
    {
        if (!hitOther)
            return;
        if (count <= 0)
            return;

        timer += iDeltime;
        if (timer >= intervel)
        {
            FindNextTarget();
            timer = 0;
            count--;
        }
    }

    public override void OnCancel()
    {
        Release();
    }

    public override void OnFinish()
    {
        Release();
    }

    void Release()
    {
        var pos = owner.GetPosition();
        if (owner.CurrentBeScene.IsInBlockPlayer(pos))
        {
            pos = BeAIManager.FindStandPositionNew(owner.GetPosition(), owner.CurrentBeScene, owner.GetFace(), false, 50);
            owner.SetPosition(pos);
        }
        RestoreSkillParam();
        RemoveHandles();
    }

    void RemoveHandles()
    {
        if (handle1 != null)
        {
            handle1.Remove();
            handle1 = null;
        }
        if (handle2 != null)
        {
            handle2.Remove();
            handle2 = null;
        }
    }
}
