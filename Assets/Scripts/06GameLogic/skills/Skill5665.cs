using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// 小兵集合技能，第一阶段为集合，第二阶段为放技能

public class Skill5666 : Skill5665
{
    public Skill5666(int sid, int skillLevel) : base(sid, skillLevel) { }
}

public class Skill5667 : Skill5665
{
    public Skill5667(int sid, int skillLevel) : base(sid, skillLevel) { }
}

public class Skill5891 : Skill5665
{
    public Skill5891(int sid, int skillLevel) : base(sid, skillLevel) { }
}

public class Skill5892 : Skill5665
{
    public Skill5892(int sid, int skillLevel) : base(sid, skillLevel) { }
}

public class Skill5893 : Skill5665
{
    public Skill5893(int sid, int skillLevel) : base(sid, skillLevel) { }
}

public class Skill5894 : Skill5665
{
    public Skill5894(int sid, int skillLevel) : base(sid, skillLevel) { }
}

public class Skill5895 : Skill5665
{
    public Skill5895(int sid, int skillLevel) : base(sid, skillLevel) { }
}

public class Skill5665 : BeSkill
{
    int monsterId;
    int skillId;
    int skillCount;
    int skillCd;
    int unitDis = (int)IntMath.kIntDen * 2;
    int speed = (int)IntMath.kIntDen * 4;
    int useSkillNumber = 0;

    List<VInt3> points = new List<VInt3>();
    List<BeActor> monsters = new List<BeActor>();
    List<VInt3> startPoints = new List<VInt3>();
    List<int> moveDistance = new List<int>();
    List<bool> readyFlags = new List<bool>();//用来标志怪物有没有走到指定的点
    int timer;
    int count;

    public Skill5665(int sid, int skillLevel) : base(sid, skillLevel) { }

    public override void OnInit()
    {
        if (skillData != null)
        {
            monsterId = TableManager.GetValueFromUnionCell(skillData.ValueA[0], level);
            skillId = TableManager.GetValueFromUnionCell(skillData.ValueB[0], level);
            skillCount = TableManager.GetValueFromUnionCell(skillData.ValueC[0], level);
            skillCd = TableManager.GetValueFromUnionCell(skillData.ValueD[0], level);
            if (skillData.ValueE.Count > 0)
            {
                var dis = VInt.NewVInt(TableManager.GetValueFromUnionCell(skillData.ValueE[0], level), GlobalLogic.VALUE_1000);
                unitDis = dis.i;
            }
            if (skillData.ValueF.Count > 0)
            {
                int s = TableManager.GetValueFromUnionCell(skillData.ValueF[0], level);
                speed = s * (int)IntMath.kIntDen / GlobalLogic.VALUE_1000;
            }
            if (skillData.ValueG.Count > 0)
            {
                useSkillNumber = TableManager.GetValueFromUnionCell(skillData.ValueG[0], level);
            }
        }
    }

    public override void OnStart()
    {
        DoWork();

        timer = skillCd;
        count = 0;
    }
    
    List<VInt3> GetPoints(int pointCount)
    {
        List<VInt3> points = new List<VInt3>();

        if (pointCount == 0)
        {
            return points;
        }

        var startPos = owner.GetPosition();
        startPos.x += owner.GetFace() ? (int)IntMath.kIntDen : -(int)IntMath.kIntDen;
        startPos = BeAIManager.FindStandPosition(startPos, owner.CurrentBeScene, owner.GetFace());
        if (owner.CurrentBeScene.IsInBlockPlayer(startPos))
        {
            startPos = owner.GetPosition();
            if (owner.CurrentBeScene.IsInBlockPlayer(startPos))//如果起点都在阻挡里面的极端情况
            {
                for (int i = 0; i < pointCount; ++i)
                {
                    points.Add(startPos);
                }
                return points;
            }
        }

        if (pointCount == 1)
        {
            points.Add(startPos);
            return points;
        }

        int maxLoopTime = 100;

        var topPos = startPos;
        for (int i = 0; i < maxLoopTime; ++i)
        {
            var pos = topPos;
            pos.y += GlobalLogic.VALUE_1000;
            if (!owner.CurrentBeScene.IsInBlockPlayer(pos))
            {
                topPos = pos;
            }
            else
            {
                break;
            }
        }
        if (topPos != startPos)
        {
            topPos.y -= GlobalLogic.VALUE_500;//找到的边界的点往里收一些，避免小兵走不到这个点
        }

        var bottomPos = startPos;
        for (int i = 0; i < maxLoopTime; ++i)
        {
            var pos = bottomPos;
            pos.y -= GlobalLogic.VALUE_1000;
            if (!owner.CurrentBeScene.IsInBlockPlayer(pos))
            {
                bottomPos = pos;
            }
            else
            {
                break;
            }
        }
        if (bottomPos != startPos)
        {
            bottomPos.y += GlobalLogic.VALUE_500;//找到的边界的点往里收一些，避免小兵走不到这个点
        }

        int width = topPos.y - bottomPos.y;//计算总宽度
        int unit = width / (pointCount - 1);

        if (unit <= unitDis)//排不下
        {
            for (int i = 0; i < pointCount; i++)
            {
                var pos = bottomPos;
                pos.y += unit * i;
                points.Add(pos);
            }
        }
        else//排得下
        {
            var halfWidth = unitDis * (pointCount - 1) / 2;
            var top = startPos;
            top.y += halfWidth;
            var bottom = startPos;
            bottom.y -= halfWidth;
            if (!owner.CurrentBeScene.IsInBlockPlayer(top) && !owner.CurrentBeScene.IsInBlockPlayer(bottom))
            {
                for (int i = 0; i < pointCount; i++)
                {
                    var pos = bottom;
                    pos.y += unitDis * i;
                    points.Add(pos);
                }
            }
            else if (owner.CurrentBeScene.IsInBlockPlayer(top))
            {
                for (int i = 0; i < pointCount; i++)
                {
                    var pos = topPos;
                    pos.y -= unitDis * i;
                    points.Add(pos);
                }
            }
            else
            {
                for (int i = 0; i < pointCount; i++)
                {
                    var pos = bottomPos;
                    pos.y += unitDis * i;
                    points.Add(pos);
                }
            }
        }

        return points;
    }

    void MonstersUseSkill()
    {
        var count = monsters.Count;
        if (useSkillNumber > 0)
        {
            count = Mathf.Min(useSkillNumber, monsters.Count);
            if (monsters.Count > 1)
            {
                int num = monsters.Count * 2;
                for (int i = 0; i < num; i++)
                {
                    int index = FrameRandom.InRange(1, monsters.Count);
                    var temp = monsters[0];
                    monsters[0] = monsters[index];
                    monsters[index] = temp;
                }
            }
        }
        for (int i = 0; i < count; ++i)
        {
            var monster = monsters[i];
            if (monster != null 
            && !monster.IsDead()
            && !monster.IsInPassiveState())
            {
                monster.UseSkill(skillId, true);
            }
        }
    }

    void DoWork()
    {
        owner.buffController.TryAddBuff(1, GlobalLogic.VALUE_20000);

        monsters.Clear();
        points.Clear();
        startPoints.Clear();
        moveDistance.Clear();
        readyFlags.Clear();
        owner.CurrentBeScene.FindMonsterByID(monsters, monsterId);
        points = GetPoints(monsters.Count);

        //如果怪物在放技能，则强制中断技能
        for (int i = 0; i < points.Count && i < monsters.Count; i++)
        {
            var monster = monsters[i];
            if (monster == null || monster.IsDead())
            {
                startPoints.Add(VInt3.zero);
                moveDistance.Add(0);
                readyFlags.Add(false);
            }
            else
            {
                startPoints.Add(monster.GetPosition());
                var vec = monster.GetPosition() - points[i];
                moveDistance.Add(vec.magnitude);

                monster.aiManager.StopCurrentCommand();
                monster.aiManager.Stop();

                if (monster.IsCastingSkill())
                {
                    monster.Locomote(new BeStateData((int)ActionState.AS_IDLE), true);
                }
                readyFlags.Add(true);
            }
        }

        //等待怪物切换状态机，延迟一帧执行
        owner.delayCaller.DelayCall(30, () =>
        {
            for (int i = 0; i < points.Count && i < monsters.Count; ++i)
            {
                var monster = monsters[i];
                if (monster != null
                && !monster.IsDead()
                && !monster.IsInPassiveState())
                {
                    monster.ChangeRunMode(true);
                    monster.ClearMoveSpeed();

                    VInt3  vSpeed = (points[i] - monster.GetPosition()).NormalizeTo(speed);
                    monster.SetMoveSpeedX(vSpeed.x);
                    monster.SetMoveSpeedY(vSpeed.y);
                    monster.SetFace(vSpeed.x < 0);

                    monster.m_pkGeActor.ChangeAction("Anim_Walk", 0.6f, true);

                    monster.buffController.TryAddBuff(1, GlobalLogic.VALUE_20000);
                }
            }
        });
    }

    void SetMonsterIdle(int index)
    {
        monsters[index].m_pkGeActor.ChangeAction("Anim_Idle", 0.25f);
        monsters[index].SetFace(owner.GetFace());
        monsters[index].SetPosition(points[index]);
        monsters[index].ResetMoveCmd();
        readyFlags[index] = false;
    }

    public override void OnUpdate(int iDeltime)
    {
        if (curPhase == 2) //集合阶段
        {
            for (int i = 0; i < points.Count && i < monsters.Count; ++i)
            {
                if (!readyFlags[i])
                {
                    continue;
                }

                if (monsters[i] == null)
                {
                    continue;
                }

                if (monsters[i].IsDead())
                {
                    readyFlags[i] = false;
                    continue;
                }

                if (monsters[i].IsInPassiveState())
                {
                    monsters[i].buffController.RemoveBuff(1);
                    readyFlags[i] = false;
                    continue;
                }

                var vec = monsters[i].GetPosition() - points[i];
                if (vec.magnitude < GlobalLogic.VALUE_3000)
                {
                    SetMonsterIdle(i);
                }
                else
                {
                    vec = monsters[i].GetPosition() - startPoints[i];
                    if (vec.magnitude >= moveDistance[i])
                    {
                        SetMonsterIdle(i);
                    }
                }
            }

            if (_checkReady())
            {
                ((BeActorStateGraph)owner.GetStateGraph()).ExecuteNextPhaseSkill();
            }
        }

        else if (curPhase == 3) //集体放技能
        {
            if (count < skillCount)
            {
                timer += iDeltime;
                if (timer >= skillCd)
                {
                    timer = 0;
                    ++count;

                    MonstersUseSkill();
                }
            }
            else
            {
                timer += iDeltime;
                if (timer >= GlobalLogic.VALUE_1000)
                {
                    ((BeActorStateGraph)owner.GetStateGraph()).ExecuteNextPhaseSkill();
                }
            }
        }
    }

    private bool _checkReady()
    {
        for (int i = 0; i < readyFlags.Count; ++i)
        {
            if (readyFlags[i])
            {
                return false;
            }
        }
        return true;
    }

    public override void OnCancel()
    {
        _resetMonsters();
    }

    public override void OnFinish()
    {
        _resetMonsters();
    }

    private void _resetMonsters()
    {
        owner.buffController.RemoveBuff(1);

        for (int i = 0; i < points.Count && i < monsters.Count; ++i)
        {
            var monster = monsters[i];
            if (monster != null && !monster.IsDead())
            {
                monster.buffController.RemoveBuff(1);
                monster.aiManager.Start();
                monster.ChangeRunMode(false);
            }
        }

        points.Clear();
        monsters.Clear();
        readyFlags.Clear();
    }
}
