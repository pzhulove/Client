using UnityEngine;
using System.Collections.Generic;
using GameClient;

/*
 * 用于雷龙出海
*/
public class Mechanism70 : BeMechanism
{

    int[] dragonIDs = new int[] { 60280, 60281, 60282 };
    int[] dragonPartNum = new int[] { 1, 6, 1 };//龙的总结数

    BeAIManager.MoveDir2[] dragonPartMoveDir = new BeAIManager.MoveDir2[8];
    BeProjectile head = null;
    BeProjectile tail = null;
    int[] dragonPartTimeAcc = new int[8];

    BeProjectile[] dragonParts = new BeProjectile[8];
    VInt speed = new VInt(3.0f);//移动的速度
    VInt size = new VInt(0.65f);//一节的长度
    VInt pveSpeed = new VInt(3.0f);//移动的速度
    VInt pvpSpeed = new VInt(3.0f);//移动的速度
    VInt height = new VInt(1.3f);//离地面的高度
    VInt3 headLastPos;

    int updateControllTimeAcc = 0;
    int updateControllDuration = 250;//控制的间隔

    int updateMoveOutTimeAcc = 0;
    int updateMoveoutDuration = 200;//检查是否出屏幕的间隔
    List<int> dirRecords = new List<int>();
    List<int> dirTimeRecords = new List<int>();

    public Mechanism70(int mid, int lv) : base(mid, lv) { }

    public Skill3118 skill = null;
    public bool canControl = false;

    bool isFinish = false;

    public override void OnInit()
    {
        base.OnInit();
        pveSpeed = new VInt( TableManager.GetValueFromUnionCell(data.ValueA[0], level)/1000.0f);
        pvpSpeed = new VInt(TableManager.GetValueFromUnionCell(data.ValueB[0], level) / 1000.0f);
        if (data.ValueC.Length > 0)
        {
            updateControllDuration = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
        }

        speed = BattleMain.IsModePvP(owner.battleType) ? pvpSpeed : pveSpeed;
        isFinish = false;
    }

    public override void OnUpdate(int deltaTime)
    {
        if (isFinish)
            return;
        UpdateMove(deltaTime);

        if (!canControl)
            return;

        UpdateControl(deltaTime);
        UpdateMoveOut(deltaTime);
    }

    public void DoStart()
    {
        dirRecords.Clear();
        dirTimeRecords.Clear();

        int index = 0;

        var pos = owner.GetPosition();
        pos.z += height.i;

        int factor = owner.GetFace() ? -1 : 1;

        for (int i = 0; i < dragonPartNum.Length; ++i)
        {
            for (int j = 0; j < dragonPartNum[i]; ++j)
            {
                var newPos = pos;
                var part = owner.AddEntity(dragonIDs[i], newPos) as BeProjectile;

                dragonPartTimeAcc[index] = 0;
                dragonParts[index] = part;
                if (index == 0)
                {
                    dragonPartMoveDir[index] = owner.GetFace() ? BeAIManager.MoveDir2.LEFT : BeAIManager.MoveDir2.RIGHT;
                    part.SetFace(false);
                }
                else
                {
                    dragonPartMoveDir[index] = BeAIManager.MoveDir2.COUNT;
                    dragonPartTimeAcc[index] = 256 * -index;
                }

                index++;
            }
        }

        head = dragonParts[0];
        headLastPos = head.GetPosition();
        tail = dragonParts[dragonParts.Length - 1];

        head.RegisterEventNew(BeEventType.onDead, eventParam =>
        {
            isFinish = true;
        });

        dirTimeRecords.Add(0);
        dirRecords.Add((int)dragonPartMoveDir[0]);

        UpdateMove(0);

        for (int i = 0; i < dragonParts.Length; ++i)
            ChangeRotation(i);

        updateControllTimeAcc = updateControllDuration;

        skill.AttackCamera(head);
    }

    public void SetDead()
    {
        owner.delayCaller.DelayCall(100, ()=>
        {
            for (int i = 0; i < dragonParts.Length; i++)
            {
                if(dragonParts[i]!=null)
                   dragonParts[i].DoDie();
            }
        });
       
    }

    void UpdateControl(int delta)
    {
        updateControllTimeAcc += delta;
        if (updateControllTimeAcc < updateControllDuration)
            return;

        updateControllTimeAcc -= updateControllDuration;

        var joystickDir = GetJoystickDir();
        if (dragonPartMoveDir[0] == joystickDir || joystickDir == BeAIManager.MoveDir2.COUNT)
            return;

        int dir1 = ((int)dragonPartMoveDir[0] + 1 + (int)BeAIManager.MoveDir2.COUNT) % (int)BeAIManager.MoveDir2.COUNT;
        int dir2 = ((int)dragonPartMoveDir[0] - 1 + (int)BeAIManager.MoveDir2.COUNT) % (int)BeAIManager.MoveDir2.COUNT;

        int nextDir = dir2;
        if (GetStep(dragonPartMoveDir[0], joystickDir) < GetStep(dragonPartMoveDir[0], joystickDir, false))
            nextDir = dir1;

        //Logger.LogErrorFormat ("target:{0} choose:{1}", joystickDir, (BeAIManager.MoveDir2)nextDir);

        dragonPartMoveDir[0] = (BeAIManager.MoveDir2)nextDir;
        ChangeRotation(0);

        dirRecords.Add(nextDir);
        dirTimeRecords.Add(dragonPartTimeAcc[0]);

        headLastPos = head.GetPosition();
    }

    int GetStep(BeAIManager.MoveDir2 start, BeAIManager.MoveDir2 target, bool isDown = true)
    {
        if (start == target)
            return 0;

        int step = 0;
        int factor = isDown ? 1 : -1;

        while (start != target)
        {
            var tmp = ((int)start + factor + (int)BeAIManager.MoveDir2.COUNT) % (int)BeAIManager.MoveDir2.COUNT;
            start = (BeAIManager.MoveDir2)tmp;

            step++;
        }

        return step;

    }

    void UpdateMoveOut(int delta)
    {
#if !LOGIC_SERVER
        if (tail == null)
            return;

        if (owner.CurrentBeScene == null)
            return;

        updateMoveOutTimeAcc += delta;
        if (updateMoveOutTimeAcc >= updateMoveoutDuration)
            updateMoveOutTimeAcc -= updateMoveoutDuration;
        else
            return;

        //如果不在屏幕内，就停止技能
        if (owner.isLocalActor &&
            (!owner.CurrentBeScene.IsInScreen(tail.GetPosition().vec3) &&
             !owner.CurrentBeScene.IsInScreen(head.GetPosition().vec3)))
        {
            InputManager.CreateStopSkillFrameCommand(skill.skillID);
            tail = null;
        }
#endif
    }

    void UpdateMove(int delta)
    {
        var curPos = head.GetPosition();
        var dis = curPos - headLastPos;
        if (Mathf.Abs(dis.x) >= size || Mathf.Abs(dis.y) >= size)
        {
            dirTimeRecords.Add(dragonPartTimeAcc[0]);
            dirRecords.Add((int)dragonPartMoveDir[0]);
        }

        for (int i = 0; i < dragonParts.Length; ++i)
        {
            dragonPartTimeAcc[i] += delta;

            for (int j = dirTimeRecords.Count - 1; j >= 0; --j)
            {
                if (dragonPartTimeAcc[i] >= dirTimeRecords[j])
                {
                    DoTurn(i, dirRecords[j]);
                    break;
                }
            }
        }

        for (int i = 0; i < dragonParts.Length; ++i)
        {
            var dir = dragonPartMoveDir[i];
            if (dir == BeAIManager.MoveDir2.COUNT)
                continue;

            VInt moveSpeedx = speed.i * BeAIManager.DIR_VALUE3[(int)dir, 0];
            VInt moveSpeedy = speed.i * BeAIManager.DIR_VALUE3[(int)dir, 1];
            dragonParts[i].SetFace(false);

            dragonParts[i].SetMoveSpeedX(moveSpeedx);
            dragonParts[i].SetMoveSpeedY(moveSpeedy);
        }
    }

    void DoTurn(int index, int dir)
    {
        var origin = dragonPartMoveDir[index];
        dragonPartMoveDir[index] = (BeAIManager.MoveDir2)dir;
        if (origin != dragonPartMoveDir[index])
            ChangeRotation(index);
    }

    void ChangeRotation(int index)
    {
#if !LOGIC_SERVER
        int angle = (int)dragonPartMoveDir[index] * 45;
        angle = 360 - angle;

        dragonParts[index].m_pkGeActor.GetEntityNode(GeEntity.GeEntityNodeType.Actor).transform.localRotation = Quaternion.AngleAxis(angle, Vector3.up);
#endif
    }

    BeAIManager.MoveDir2 GetJoystickDir()
    {
        BeAIManager.MoveDir2 ret = BeAIManager.MoveDir2.COUNT;
        if (owner.IsInMoveDirection())
        {
            int degree = owner.GetJoystickDegree();

            ret = InputManager.GetDir8(degree);
        }

        return ret;
    }
}
