using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//哥布林修理王轿技能
public class Skill5671 : BeSkill
{
    int monsterId = 30200011;
    int speed = (int)IntMath.kIntDen;
    int standDis = 15000;
    int timer;

    public Skill5671(int sid, int skillLevel) : base(sid, skillLevel) { }
    public override void OnInit()
    {
        //int s = TableManager.GetValueFromUnionCell(skillData.ValueA[0], level);
        //speed = s * (int)IntMath.kIntDen / GlobalLogic.VALUE_1000;
        speed = 20000;
    }

    public override void OnStart()
    {
        owner.aiManager.StopCurrentCommand();
        owner.aiManager.Stop();
    }

    public override void OnEnterPhase(int phase)
    {
        if (phase == 1)
        {
            TraceTarget();
        }
        else if (phase == 2)
        {
            owner.ResetMoveCmd();
            owner.ChangeRunMode(false);
        }
    }

    void TraceTarget()
    {
        if (owner == null || owner.IsDead())
            return;

        List<BeActor> monsters = GamePool.ListPool<BeActor>.Get();
        owner.CurrentBeScene.FindMonsterByID(monsters, monsterId);
        if (monsters.Count > 0)
        {
            owner.ChangeRunMode(true);
            owner.ClearMoveSpeed();

            VInt3 vec = monsters[0].GetPosition() - owner.GetPosition();
            if (vec.magnitude > standDis)
            {
                VInt3 temp = vec;
                temp.NormalizeTo(standDis);
                vec = vec - temp;
                timer = vec.magnitude * GlobalLogic.VALUE_1000 / speed;
                VInt3 vSpeed = vec.NormalizeTo(speed);
                owner.SetMoveSpeedX(vSpeed.x);
                owner.SetMoveSpeedY(vSpeed.y);
                owner.SetFace(vSpeed.x < 0);
            }
            else
            {
                timer = 0;
            }
        }
        GamePool.ListPool<BeActor>.Release(monsters);
    }

    public override void OnUpdate(int iDeltime)
    {
        if (curPhase == 1)
        {
            timer -= iDeltime;
            if (timer <= 0)
            {
                ((BeActorStateGraph)owner.GetStateGraph()).ExecuteNextPhaseSkill();
            }
        }
    }

    public override void OnCancel()
    {
        ResetMove();
    }

    public override void OnFinish()
    {
        ResetMove();
    }

    void ResetMove()
    {
        owner.ResetMoveCmd();
        owner.ChangeRunMode(false);
        owner.aiManager.Start();
    }
}
