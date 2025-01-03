using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 黑色大地房间三--boss的破防机制
/// </summary>
public class Mechanism2006 : BeMechanism
{
    private int hitNum = 0; //被击次数
    private int hitMax = 0;//达到次数之后召唤灵魂
    private int summonID = 0;//召唤ID
    private bool mStarted = true;
    readonly int intervel = GlobalLogic.VALUE_500;//每隔0.5秒检测是否还有灵魂存在
    private readonly VInt tolerance = VInt.Float2VIntValue(0.1f);
    private List<BeActor> summonList = new List<BeActor>();
    private VInt3[] pos = new VInt3[3];
    private bool[] readyFlag = new bool[3];
    private int buffID = 0;//怪物的高减伤BUFF
    private bool startFocus = false;
    private readonly int buffInfoId = 568936;
    private bool flag = false;
    private float radius = 3;
    private VInt3 centerOffset = VInt3.zero;
    public Mechanism2006(int mid, int lv) : base(mid, lv) { }

    public override void OnInit()
    {
        base.OnInit();
        hitMax = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        summonID = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        buffID = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
        radius = TableManager.GetValueFromUnionCell(data.ValueD[0], level)/1000.0f;
        centerOffset = new VInt3( TableManager.GetValueFromUnionCell(data.ValueE[0], level)/1000.0f, TableManager.GetValueFromUnionCell(data.ValueE[1], level) / 1000.0f, 0);
    }

    public override void OnReset()
    {
        hitNum = 0;
        hitMax = 0;
        summonID = 0;
        mStarted = true;
        summonList.Clear();
        pos = new VInt3[3];
        readyFlag = new bool[3];
        startFocus = false;
        flag = false;
    }

    public override void OnStart()
    {
        base.OnStart();
        SetMonsterPos();
        InitReadyFlag();
        hitNum = 0;
        summonList.Clear();
        flag = false;
        handleA = OwnerRegisterEventNew(BeEventType.onHit, args =>
        //handleA = owner.RegisterEvent(BeEventType.onHit, (args) =>
        {
            //被击一定次数后召唤出三个小灵魂
            hitNum++;
            if (hitNum >= hitMax && !flag)
            {
                flag = true;
                owner.DoSummon(summonID,60,ProtoTable.EffectTable.eSummonPosType.ORIGIN,null,3);              
            }
        });

        handleB = owner.RegisterEventNew(BeEventType.onSummon, (args) => 
        {          

            //召唤出的灵魂被打一定血量问题的时候给boss加个buff
            BeActor actor = args.m_Obj as BeActor;

            actor.RegisterEventNew(BeEventType.onHit, obj =>
            //actor.RegisterEvent(BeEventType.onHit, (obj) => 
            {
                int damage = obj.m_Int;
                BeActor target = obj.m_Obj as BeActor;
                if (damage >= actor.GetEntityData().GetHP() && target!=null)
                {
                    target.buffController.TryAddBuff(buffInfoId);
                }
            });

            if (actor.aiManager != null)
                actor.aiManager.Stop();
            summonList.Add(actor);
            startFocus = summonList.Count == 3;
        });


        handleC = owner.RegisterEventNew(BeEventType.onAddBuff, (args) => 
        {
            //添加固定buff之后重新开始新的一个轮回
            BeBuff buff = args.m_Obj as BeBuff;
            if (buff.buffID == buffID)
            {
                InitReadyFlag();
                hitNum = 0;
                summonList.Clear();
                flag = false;
            }
        });
       
    }

    private void InitReadyFlag()
    {
        for (int i = 0; i < readyFlag.Length; i++)
        {
            readyFlag[i] = false;
        }
    }

    /// <summary>
    /// 设置灵魂的位置
    /// </summary>
    private void SetMonsterPos()
    {
        VInt3 center = owner.CurrentBeScene.GetSceneCenterPosition()+ centerOffset;

        pos[0] = center + VInt3.right*-radius;
        pos[1] = center + VInt3.right* radius;
        pos[2] = center + VInt3.up* -radius;
    }

    private int timer = 0;
    public override void OnUpdate(int deltaTime)
    {
        base.OnUpdate(deltaTime);
        if (startFocus)
        {
            timer += deltaTime;
            if (timer >= intervel)
            {             
                if (IsAllMonsterDead())
                {
                    owner.buffController.RemoveBuff(buffID);
                    startFocus = false;
                    summonList.Clear();
                }

            }
        }
        MoveMonster();
    }

    /// <summary>
    /// 将灵魂移动到固定位置
    /// </summary>
    private void MoveMonster()
    {
        if (summonList.Count>0)
        {
            for (int i = 0; i < summonList.Count; i++)
            {
                if (!readyFlag[i])
                {
                    BeActor monster = summonList[i];
                    if (monster == null || monster.IsDead()) continue;
                    var vec = pos[i] - monster.GetPosition();
                    if (vec.magnitude <= tolerance.i)
                    {
                        monster.SetMoveSpeedX(VInt.zero);
                        monster.SetMoveSpeedY(VInt.zero);
                    }
                    else
                    {

                        VInt speed = vec.magnitude;
                        vec.NormalizeTo(speed.i);
                        monster.SetMoveSpeedX(vec.x);
                        monster.SetMoveSpeedY(vec.y);

                    }
                    if (IsNearTargetPosition(monster.GetPosition(), pos[i]))
                    {
                        readyFlag[i] = true;
                    }
                }
            }
        }
    }

    private bool IsNearTargetPosition(VInt3 pos, VInt3 targetPos)
    {
        int distance = tolerance.i;
        int dist = (pos - targetPos).magnitude;
        return dist <= distance;
    }

    public override void OnFinish()
    {
        base.OnFinish();
        startFocus = false;
    }

    /// <summary>
    /// 召唤的灵魂是否死完了
    /// </summary>
    /// <returns></returns>
    private bool  IsAllMonsterDead()
    {
        for (int i = 0; i < summonList.Count; i++)
        {
            BeActor monster = summonList[i];
            if (monster == null) continue;
            if (!monster.IsDead())
            {
                return false;
            }
        }
        return true;
    }

}
