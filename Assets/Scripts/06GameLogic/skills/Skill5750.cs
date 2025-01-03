using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//命运祭坛技能
class Skill5750 : BeSkill
{
    int hoodId = 30180011;
    int altarId = 30190011;
    List<VInt3> positionList = new List<VInt3>();

    public Skill5750(int sid, int skillLevel) : base(sid, skillLevel) { }

    public override void OnEnterPhase(int phase)
    {
        if (phase == 2)
        {
            _findAltarPositions();
            _summonHoods();
        }
    }

    private void _findAltarPositions()//找到祭台位置
    {
        List<BeActor> targets = GamePool.ListPool<BeActor>.Get();
        owner.CurrentBeScene.FindMonsterByID(targets, altarId);
        for (int i = 0; i < targets.Count; i++)
        {
            if (targets[i] != null)
            {
                positionList.Add(targets[i].GetPosition());
            }
        }
        GamePool.ListPool<BeActor>.Release(targets);
    }

    private void _summonHoods()//召唤保护罩
    {
        int playerCount = 0;//活着的玩家数量
        var players = owner.CurrentBeBattle.dungeonPlayerManager.GetAllPlayers();
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].playerActor != null && !players[i].playerActor.IsDead())
            {
                playerCount++;
            }
        }

        if (positionList.Count < playerCount)
            return;

        int rand = owner.FrameRandom.Random((uint)positionList.Count);
        object[] summoned = new object[1];
        for (int i = 0; i < playerCount; i++)
        {
            int index = (i + rand) % positionList.Count;
            if (owner.DoSummon(hoodId, level, ProtoTable.EffectTable.eSummonPosType.ORIGIN, null, 1, 0, 0, 0, 0, false, 0, 0, null, SummonDisplayType.NONE, summoned))
            {
                if (summoned[0] != null)
                {
                    var monster = (BeActor)summoned[0];
                    monster.SetPosition(positionList[index]);
                }
            }
        }
    }

    public override void OnCancel()
    {
        _release();
    }

    public override void OnFinish()
    {
        _release();
    }

    private void _release()
    {
        positionList.Clear();
    }
}
