using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//哥布林王的掷骰子技能（继承草人技能）

public class Skill5758 : Skill2112
{
    int[] skillList;
    int[] entityList;
    int curIndex;

    int lastSkillIndex = -1;
    
    public Skill5758(int sid, int skillLevel) : base(sid, skillLevel) { }

    public override void OnInit()
    {
        distancex = VInt.NewVInt(TableManager.GetValueFromUnionCell(skillData.ValueA[0], level), GlobalLogic.VALUE_1000);
        distancey = VInt.NewVInt(TableManager.GetValueFromUnionCell(skillData.ValueA[1], level), GlobalLogic.VALUE_1000);

        skillList = new int[skillData.ValueB.Count];
        for (int i = 0; i < skillData.ValueB.Count; i++)
        {
            skillList[i] = TableManager.GetValueFromUnionCell(skillData.ValueB[i], level);
        }

        entityList = new int[skillData.ValueC.Count];
        for (int i = 0; i < skillData.ValueC.Count; i++)
        {
            entityList[i] = TableManager.GetValueFromUnionCell(skillData.ValueC[i], level);
        }

        // effectInfoId = 1028;
        effectPath = "Effects/Monster_Renzhe/Prefab/Eff_renzhe_yanwu";
        isCreatePoppet = false;
    }

    public override bool CheckSpellCondition(ActionState state)
    {
        bool flag =
            owner.stateController.HasBuffState(BeBuffStateType.FROZEN) ||
            owner.stateController.HasBuffState(BeBuffStateType.STUN) ||
            owner.stateController.HasBuffState(BeBuffStateType.STONE) ||
            owner.stateController.HasBuffState(BeBuffStateType.SLEEP) ||
            owner.stateController.HasBuffState(BeBuffStateType.STRAIN);

        bool flag2 =
            owner.sgGetCurrentState() == (int)ActionState.AS_HURT ||
            owner.sgGetCurrentState() == (int)ActionState.AS_GRABBED ||
            owner.sgGetCurrentState() == (int)ActionState.AS_CASTSKILL ||
            owner.sgGetCurrentState() == (int)ActionState.AS_BIRTH ||
            owner.sgGetCurrentState() == (int)ActionState.AS_JUMPBACK;

        return !flag && !flag2 && !owner.IsInPassiveState() && !owner.stateController.WillBeGrab();
    }

    public override void OnEnterPhase(int phase)
    {
        if (phase == 3)//掷骰子阶段（随机选择技能）
        {
            curIndex = GetNextSkillIndex();
            var id = entityList[curIndex];
            CreateHeadEntity(id);
        }
    }
    
    void CreateHeadEntity(int id)
    {

        var players = owner.CurrentBeBattle.dungeonPlayerManager.GetAllPlayers();
        for (int i = 0; i < players.Count; i++)
        {
            var player = players[i];
            if (player != null && player.playerActor != null && !player.playerActor.IsDead())
            {
                var entity = player.playerActor.AddEntity(id, player.playerActor.GetPosition());
                if (entity == null || entity.m_pkGeActor == null) continue;
#if !LOGIC_SERVER
                var parent = player.playerActor.m_pkGeActor.GetAttachNode("[actor]OrignBuff");
                var entityRoot = entity.m_pkGeActor.GetEntityNode(GeEntity.GeEntityNodeType.Root);
                if (parent != null && entityRoot != null)
                {
                    Battle.GeUtility.AttachTo(entityRoot, parent, false);
                    entityRoot.transform.localPosition = Vector3.zero;
                }
#endif
            }
        }

    }

    int GetNextSkillIndex()
    {
        if (skillList.Length == 0)
            return -1;

        int index = -1;
        for (int i = 0; i < 100; i++)//最多循环100次，保证不会死循环
        {
            index = FrameRandom.Random((uint)skillList.Length);
            if (index != lastSkillIndex)
            {
                break;
            }
        }
        lastSkillIndex = index;

        return index;
    }

    public override void OnFinish()
    {
        var skillId = skillList[curIndex];
        owner.delayCaller.DelayCall(30, () =>
        {
            owner.UseSkill(skillId);
        });
    }
}