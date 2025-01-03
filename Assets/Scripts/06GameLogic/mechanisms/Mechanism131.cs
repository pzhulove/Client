using System;
using System.Collections.Generic;
using UnityEngine;
using GameClient;

//克里克移动、吃哥布林机制
class Mechanism131 : BeMechanism
{
    enum StateEnum
    {
        NONE,
        PRE_SKILL,      //开场阶段
        STAY,           //停在小屋边上阶段
        MOVE,           //移动阶段
        ATTACK_SKILL,   //释放全屏吸咬技能阶段
        BEFORE_DEAD     //自爆阶段
    }

    int monsterNum;                             //要吃的哥布林数量
    int stayTime;                               //哥布林在小屋边上的停留时间
    int totalTime;                              //爆炸时间
    int hpReduce;                               //小屋每受到这么多伤害飞出一个哥布林
    int maxNumForOneHit;                        //一次伤害最多飞出的哥布林数量

    readonly int eatSkillId = 5821;
    readonly int attackSkillId = 5822;
    readonly int boomSkill = 5823;
    readonly int deadSkill = 5824;
    readonly int preSkill = 5825;
    readonly int preMoveTime = 1100;                     //克里克开场动作持续时间
    readonly int preStayTime = 800;                      //克里克移动的时候，提前让小屋解除无敌
    readonly int roomId1 = 30450021;
    readonly int roomId2 = 30490021;
    readonly int roomLeadAttackBuffInfo = 568912;        //引导攻击克里克效果
    List<BeActor> roomList = new List<BeActor>();//三个小屋
    int hpReduceTotal;                          //小屋受到伤害的统计

    int singleCount;                            //统计克里克单次停留的时候有没有吃到哥布林
    int monsterCount;                           //统计克里克吃到哥布林的总数
    int mainTimer;
    StateEnum state = StateEnum.NONE;

    VInt3[] pointArray;                         //克里克停留的三个点
    VInt speed;
    int index;
    bool setRoomFlag;                           //用来标记小屋状态能不能切换

    GoblinKingdomFrame uiFrame = null;

    public Mechanism131(int mid, int lv) : base(mid, lv) { }

    public override void OnReset()
    {
        roomList.Clear();
        hpReduceTotal = 0;
        singleCount = 0;
        monsterCount = 0;
        mainTimer = 0;
        state = StateEnum.NONE;
        index = 0;
        setRoomFlag = false;
        uiFrame = null;
    }
    public override void OnInit()
    {
        monsterNum = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        stayTime = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
        speed = VInt.NewVInt(TableManager.GetValueFromUnionCell(data.ValueD[0], level), GlobalLogic.VALUE_1000);
        totalTime = TableManager.GetValueFromUnionCell(data.ValueE[0], level);
        hpReduce = TableManager.GetValueFromUnionCell(data.ValueF[0], level);
        maxNumForOneHit = TableManager.GetValueFromUnionCell(data.ValueG[0], level);

        pointArray = new VInt3[3];
        pointArray[0] = new VInt3(-100000, 60000, 0);
        pointArray[1] = new VInt3(-155000, 60000, 0);
        pointArray[2] = new VInt3(-45000, 60000, 0);
    }

    public override void OnStart()
    {
        index = 0;
        mainTimer = 5000;
        monsterCount = 0;
        singleCount = 0;
        setRoomFlag = false;

        handleA = owner.RegisterEventNew(BeEventType.onAIStart, args =>
        {
            owner.aiManager.Stop();

            owner.UseSkill(preSkill, true);
            state = StateEnum.PRE_SKILL;

#if !LOGIC_SERVER
            uiFrame = ClientSystemManager.instance.OpenFrame<GoblinKingdomFrame>() as GoblinKingdomFrame;
            uiFrame.SetNumText(monsterNum.ToString());
#endif
            UpdateTotalTime(0);
        });

        InitRooms();
    }

    void InitRooms()
    {
        owner.CurrentBeScene.FindMonsterByID(roomList, roomId2);
        var tempList = GamePool.ListPool<BeActor>.Get();
        owner.CurrentBeScene.FindMonsterByID(tempList, roomId1);
        if (tempList[0].GetPosition().x < tempList[1].GetPosition().x)
        {
            roomList.Add(tempList[0]);
            roomList.Add(tempList[1]);
        }
        else
        {
            roomList.Add(tempList[1]);
            roomList.Add(tempList[0]);
        }
        GamePool.ListPool<BeActor>.Release(tempList);

        for (int i = 0; i < roomList.Count; i++)
        {
            var room = roomList[i];
            room.buffController.TryAddBuff((int)GlobalBuff.INVINCIBLE_NO_ALPHA, 999999);

            room.RegisterEventNew(BeEventType.onHPChange, args =>
            {
                int value = args.m_Int;
                hpReduceTotal += value;
                for (int j = 0; j < maxNumForOneHit; j++)
                {
                    if (-hpReduceTotal < hpReduce)
                        break;

                    hpReduceTotal += hpReduce;
#if !LOGIC_SERVER
                    if (roomList.IndexOf(room) == 0)
                        owner.CurrentBeScene.currentGeScene.CreateEffect(1008, room.GetPosition().vec3);
                    else
                        owner.CurrentBeScene.currentGeScene.CreateEffect(1009, room.GetPosition().vec3);
#endif
                    owner.delayCaller.DelayCall(800, () =>
                    {
                        owner.UseSkill(eatSkillId);
                        ++monsterCount;
                        ++singleCount;
                        if (monsterCount == monsterNum)//吃够哥布林数量了
                        {
                            SetCurrentRoomState(false);
                            state = StateEnum.BEFORE_DEAD;
                            mainTimer = 2300;
                            owner.delayCaller.DelayCall(300, () =>
                            {
                                owner.UseSkill(deadSkill, true);
                            });
                        }
                        if (uiFrame != null && monsterCount <= monsterNum)
                            uiFrame.SetNumText((monsterNum - monsterCount).ToString());
                    });
                }
            });
        }
    }

    public override void OnUpdate(int deltaTime)
    {
        if (owner == null)
            return;
        if (owner.IsDead())
            return;
        if (state == StateEnum.NONE)
            return;

        if (state == StateEnum.PRE_SKILL)
        {
            mainTimer -= deltaTime;
            if (mainTimer <= 0)
            {
                StartMove();
            }
        }
        else if (state == StateEnum.MOVE)
        {
            mainTimer -= deltaTime;
            if (mainTimer <= preStayTime)
            {
                SetCurrentRoomState(true);
                setRoomFlag = true;
            }
            if (mainTimer <= 0)
            {
                StopMove();
                mainTimer = 0;
            }
        }
        else if (state == StateEnum.STAY)
        {
            mainTimer += deltaTime;
            if (mainTimer + preMoveTime >= stayTime)//让小屋进入无敌
            {
                SetCurrentRoomState(false);
                setRoomFlag = true;
            }
            if (mainTimer >= stayTime)
            {
                if (singleCount > 0)
                {
                    StartMove();
                }
                else
                {
                    owner.UseSkill(attackSkillId, true);
                    state = StateEnum.ATTACK_SKILL;
                    mainTimer = 3000;
                }
            }
        }
        else if (state == StateEnum.ATTACK_SKILL)
        {
            mainTimer -= deltaTime;
            if (mainTimer <= 0)
            {
                StartMove();
            }
        }
        else if (state == StateEnum.BEFORE_DEAD)
        {
            mainTimer -= deltaTime;
            if (mainTimer <= 0)
            {
                owner.DoDead();
            }
        }

        if (state != StateEnum.BEFORE_DEAD && state != StateEnum.PRE_SKILL)
            UpdateTotalTime(deltaTime);
    }

    void SetCurrentRoomState(bool state)
    {
        if (setRoomFlag)
            return;

        if (state)//小屋解除无敌
        {
            roomList[index].buffController.RemoveBuff((int)GlobalBuff.INVINCIBLE_NO_ALPHA);
            roomList[index].buffController.TryAddBuff(roomLeadAttackBuffInfo);
        }
        else//小屋进入无敌
        {
            roomList[index].buffController.TryAddBuff((int)GlobalBuff.INVINCIBLE_NO_ALPHA, 999999);
        }
    }

    void UpdateTotalTime(int deltaTime)
    {
        totalTime -= deltaTime;
        if (totalTime > 0)
        {
            if (uiFrame != null)
            {
                int m = totalTime / 60000;
                int s = (totalTime % 60000) / 1000;
                int ms = (totalTime % 1000) / 10;
                string text = string.Format("{0}.{1:D2}.{2:D2}", m, s, ms);
                uiFrame.SetTimeText(text);
            }
        }
        else//总时间到了，没吃够哥布林数量
        {
            state = StateEnum.BEFORE_DEAD;
            mainTimer = 2500;
            owner.UseSkill(boomSkill, true);
        }
    }

    void RandNextIndex()
    {
        int rand = owner.FrameRandom.Random(2) + 1;
        index = (index + rand) % pointArray.Length;
    }

    void MoveToTarget()
    {
        owner.ChangeRunMode(true);
        owner.ClearMoveSpeed();

        var vSpeed = pointArray[index] - owner.GetPosition();
        mainTimer = vSpeed.magnitude * GlobalLogic.VALUE_1000 / speed.i;
        vSpeed.NormalizeTo(speed.i);

        owner.SetMoveSpeedX(vSpeed.x);
        owner.SetMoveSpeedY(vSpeed.y);
        owner.SetFace(vSpeed.x < 0);

        owner.m_pkGeActor.ChangeAction("Anim_Walk", 0.25f, true);
    }

    void StartMove()
    {
        RandNextIndex();
        MoveToTarget();
        state = StateEnum.MOVE;
        setRoomFlag = false;
    }

    void StopMove()
    {
        owner.SetPosition(pointArray[index]);
        owner.SetFace(false);
        owner.ResetMoveCmd();
        owner.m_pkGeActor.ChangeAction("Anim_Stayopen02", 0.25f, true);
        state = StateEnum.STAY;
        setRoomFlag = false;
        singleCount = 0;
        hpReduceTotal = 0;
#if !LOGIC_SERVER
        GeEffectEx effect = owner.m_pkGeActor.CreateEffect(1007, Vec3.zero);
        if (effect != null)
        {
            var pos = effect.GetPosition();
            pos.y += 2.3f;
            effect.SetPosition(pos);
        }
#endif
    }

    public override void OnFinish()
    {
#if !LOGIC_SERVER
        if (uiFrame != null)
        {
            uiFrame.Close();
            uiFrame = null;
        }
#endif
        for (int i = 0; i < roomList.Count; i++)
        {
            roomList[i].GetEntityData().SetHP(-1);
            roomList[i].DoDead();
        }
        roomList.Clear();
    }
}