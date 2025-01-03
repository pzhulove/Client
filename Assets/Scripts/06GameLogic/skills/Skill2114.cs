using UnityEngine;
using System.Collections;
using System.Collections.Generic;


//召唤师 精灵献祭技能
public class Skill2114 : BeSkill
{
    VInt diameter;
    List<int> monsterIDs = new List<int>();
    int leiwosi = 9030011;
    int bingnaisi = 9010011;

    Dictionary<int, int> entityMap = new Dictionary<int, int>();
#if !LOGIC_SERVER
    float scale = 1.0f;
#endif
    bool findEffect = false;
    string effectName = "Effects/Hero_Zhaohuanshi/Xianji/Prefab/Eff_xianji_fazhen";

    public Skill2114(int sid, int skillLevel) : base(sid, skillLevel)
    {

    }

    public override bool CanUseSkill()
    {
        return base.CanUseSkill();
    }

    public override void OnPostInit()
    {
        base.OnPostInit();
        monsterIDs.Clear();
        entityMap.Clear();

        if (!BattleMain.IsModePvP(battleType))
        {
            diameter = VInt.NewVInt(TableManager.GetValueFromUnionCell(skillData.ValueA[0], level), GlobalLogic.VALUE_1000);
            var normalDiameter = TableManager.GetValueFromUnionCell(skillData.ValueA[0], 1) / 1000f;
#if !LOGIC_SERVER
            scale = diameter.i / (normalDiameter * 10000);
#endif

            for (int i = 0; i < skillData.ValueB.Count; ++i)
            {
                int mid = TableManager.GetValueFromUnionCell(skillData.ValueB[i], level);
                monsterIDs.Add(mid);

                int eid = TableManager.GetValueFromUnionCell(skillData.ValueC[i], level);

                if (!entityMap.ContainsKey(mid))
                    entityMap.Add(mid, eid);
            }
        }
        else
        {
            diameter = VInt.NewVInt(TableManager.GetValueFromUnionCell(skillData.ValueD[0], level), GlobalLogic.VALUE_1000);
            var normalDiameter = TableManager.GetValueFromUnionCell(skillData.ValueD[0], 1) / 1000f;
#if !LOGIC_SERVER
            scale = diameter.i / (normalDiameter * 10000);
#endif

            for (int i = 0; i < skillData.ValueE.Count; ++i)
            {
                int mid = TableManager.GetValueFromUnionCell(skillData.ValueE[i], level);
                monsterIDs.Add(mid);

                int eid = TableManager.GetValueFromUnionCell(skillData.ValueF[i], level);

                if (!entityMap.ContainsKey(mid))
                    entityMap.Add(mid, eid);
            }
        }
    }

    public override void OnStart()
    {
        findEffect = false;
    }

    public override void OnEnterPhase(int phase)
    {
        if (phase == 2)
        {
            VInt3 pos = owner.GetPosition();
            if (owner.GetFace())
                pos.x -= diameter.i / 2;
            else
                pos.x += diameter.i / 2;

            List<BeActor> summonMonsters = GamePool.ListPool<BeActor>.Get();
            owner.CurrentBeScene.GetSummonBySummoner(summonMonsters, owner);
            for (int i = 0; i < summonMonsters.Count; ++i)
            {
                BeActor monster = summonMonsters[i];
                VInt3 mPos = monster.GetPosition();
                if (!BattleMain.IsModePvP(owner.battleType))
                {
                    if (BeUtility.IsMonsterIDEqualList(monsterIDs, monster.GetEntityData().monsterID))
                    {
                        TriggerExplosionPVE(monster, entityMap[monster.GetEntityData().monsterID]);
                    }
                }
                else
                {
                    if ((pos - mPos).magnitude <= (diameter.i / 2) && BeUtility.IsMonsterIDEqualList(monsterIDs, monster.GetEntityData().monsterID))
                    {
                        TriggerExplosionPVP(monster, entityMap[monster.GetEntityData().monsterID]);
                    }

                }
            }

            GamePool.ListPool<BeActor>.Release(summonMonsters);
        }
    }

    public override void OnUpdate(int iDeltime)
    {
#if !LOGIC_SERVER
        if (!findEffect)
        {
            var effect = owner.m_pkGeActor.FindEffect(effectName);
            if (effect != null)
            {
                findEffect = true;
                effect.SetScale(scale);
            }
        }
#endif
    }

    void TriggerExplosionPVE(BeActor actor, int entityID)
    {
        VInt3 xOffset = new VInt3(owner.GetFace() ? -2.8f : 2.8f, 0, 0);
        VInt3 pos = owner.GetPosition() + xOffset;
        if (owner.CurrentBeScene.IsInBlockPlayer(pos))
        {
            pos = actor.GetPosition();
        }
        actor.SetPosition(pos, true);
        owner.AddEntity(entityID, actor.GetPosition(), GetLevel());
        float value = actor.GetEntityData().GetMaxHP() * 0.55f;
        actor.DoHurt((int)value);
    }

    void TriggerExplosionPVP(BeActor actor, int entityID)
    {
        if (actor.aiManager != null)
            actor.aiManager.Stop();
        actor.Locomote(new BeStateData((int)ActionState.AS_IDLE));

        string actionName = "ExpDead";
        //if (actor.HasAction(actionName))
        {
            actor.m_pkGeActor.SetActorVisible(false);
            bool removeFloat = false;
            if (actor.GetEntityData().MonsterIDEqual(bingnaisi))
            {
                removeFloat = true;
            }
            else if (actor.GetEntityData().MonsterIDEqual(leiwosi))
            {
                removeFloat = true;
                if (actor.aiManager.aiTarget != null)
                {
                    actor.SetPosition(actor.aiManager.aiTarget.GetPosition());
                }
            }

            if (removeFloat && actor.floatingHeight > 0)
            {
                actor.RemoveFloating();
                var pos = actor.GetPosition();
                pos.z = 0;
                actor.SetPosition(pos);
            }
            //actor.PlayAction(actionName);
            //actor.sgSetCurrentStatesTimeout(1500);

            var buff = actor.buffController.HasBuffByID((int)GlobalBuff.LIFE_TIME) as Buff12;
            if (buff != null)
                buff.showDisappearEffect = false;

            owner.AddEntity(entityID, actor.GetPosition(), GetLevel());

            actor.delayCaller.DelayCall(1500, () =>
            {
                actor.DoDead();
            });
        }


    }
}
