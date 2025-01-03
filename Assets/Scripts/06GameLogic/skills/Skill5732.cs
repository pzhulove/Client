using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//王轿撞击技能
public class Skill5732 : BeSkill
{
    VInt speed;
    int buffId;
    int timer;
    VInt3 startPos;
    bool moveFlag = false;
    bool hitPlayerFlag = false;
    List<IBeEventHandle> handleList = new List<IBeEventHandle>();

    public Skill5732(int sid, int skillLevel) : base(sid, skillLevel) { }
    public override void OnInit()
    {
        speed = VInt.NewVInt(TableManager.GetValueFromUnionCell(skillData.ValueA[0], level), GlobalLogic.VALUE_1000);
    }

    public override void OnStart()
    {
        PlayersRegisterEvent();
    }

    void PlayersRegisterEvent()//监听玩家是否被王轿撞到
    {
        hitPlayerFlag = false;

        var players = owner.CurrentBeBattle.dungeonPlayerManager.GetAllPlayers();
        for (int i = 0; i < players.Count; i++)
        {
            var player = players[i];
            if (player == null)
                return;
            if (player.playerActor == null)
                return;
            if (player.playerActor.IsDead())
                return;

            var handle = player.playerActor.RegisterEventNew(BeEventType.onHit, args =>
            {
                var attacker = args.m_Obj as BeActor;
                if (attacker == owner)
                {
                    hitPlayerFlag = true;
                }
            });
            handleList.Add(handle);
        }
    }

    public override void OnEnterPhase(int phase)
    {
        if (phase == 2)
        {
            MoveFast();
        }
        else if (phase == 3)
        {
            if (hitPlayerFlag)//如果之前撞到玩家了，就跳过此阶段
            {
                ((BeActorStateGraph)owner.GetStateGraph()).ExecuteNextPhaseSkill();
            }
        }
    }

    void MoveFast()
    {
        owner.ChangeRunMode(true);
        owner.ClearMoveSpeed();

        bool left = owner.GetPosition().x < owner.CurrentBeScene.GetSceneCenterPosition().x;
        owner.SetFace(!left);

        startPos = owner.GetPosition();

        int dir = left ? 1 : -1;
        owner.SetMoveSpeedX(speed.i * dir);
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
        RemoveHandles();

        owner.ResetMoveCmd();
        owner.ChangeRunMode(false);
    }

    void RemoveHandles()
    {
        for (int i = 0; i < handleList.Count; i++)
        {
            if (handleList[i] != null)
            {
                handleList[i].Remove();
                handleList[i] = null;
            }
        }
        handleList.Clear();
    }
}
