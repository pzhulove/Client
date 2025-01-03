using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameClient;

/*
 * 指挥某个ID的怪物释放某种技能
*/
public class Mechanism8 : BeMechanism {

	int monsterID;
	int skillID;
	//int buffID;
    bool registerFlag = false;
    bool isEnemy = true;
    bool buffRelateLevel = false;
	public Mechanism8(int mid, int lv):base(mid, lv)
	{
	}

	public override void OnInit ()
	{
		monsterID = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
		skillID = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        isEnemy = TableManager.GetValueFromUnionCell(data.ValueE[0], level) == 0 ? true : false;
        registerFlag = TableManager.GetValueFromUnionCell(data.ValueD[0], level) == 0 ? false : true;
        buffRelateLevel = TableManager.GetValueFromUnionCell(data.ValueF[0], level) == 0 ? false : true;
    }

	public override void OnStart ()
	{
        RegisterMonsterSummon();
        if (owner != null)
		{
			List<BeActor> monsters = GamePool.ListPool<BeActor>.Get();
            if (monsterID == 80620011)
            {
                owner.CurrentBeScene.FindMonsterByID(monsters, monsterID, isEnemy);
                owner.CurrentBeScene.FindMonsterByID1(monsters, 80820011, isEnemy);
            }
            else
            {
                owner.CurrentBeScene.FindMonsterByID(monsters, monsterID, isEnemy);
            }
			for(int i=0; i<monsters.Count; ++i)
			{
				var sidetick = monsters[i];

				if (sidetick == null || sidetick.IsDead())
					continue;
                AddBuffInfo(sidetick);
                UseSkill(sidetick);
			}

			GamePool.ListPool<BeActor>.Release(monsters);
		}
	}

    /// <summary>
    /// 监听怪物创建
    /// </summary>
    private void RegisterMonsterSummon()
    {
        if (!registerFlag)
            return;
        if (owner == null || owner.CurrentBeScene == null)
            return;
        handleA = owner.CurrentBeScene.RegisterEventNew(BeEventSceneType.onCreateMonster, SummonMonster);
    }

    /// <summary>
    /// 监听事件
    /// </summary>
    /// <param name="args"></param>
    private void SummonMonster(BeEvent.BeEventParam args)
    {
        var actor = (BeActor) args.m_Obj;
        if (actor == null)
            return;
        if (monsterID == 80620011)
        {
            if (!actor.GetEntityData().MonsterIDEqual(monsterID) && !actor.GetEntityData().MonsterIDEqual(80820011))
                return;
        }
        else
        {
            if (!actor.GetEntityData().MonsterIDEqual(monsterID))
                return;
        }
        UseSkill(actor);
        AddBuffInfo(actor);

    }

    /// <summary>
    /// 使用技能
    /// </summary>
    private void UseSkill(BeActor monster)
    {
        if (skillID > 0 && (!monster.IsInPassiveState() || monster.IsCastingSkill()))
        {
            var skill = monster.GetSkill(skillID);
            if (skill != null)
            {
                skill.ResetCoolDown();
                monster.UseSkill(skillID, true);
            }
        }
    }

    /// <summary>
    /// 添加Buff信息
    /// </summary>
    private void AddBuffInfo(BeActor monster)
    {
        if (data.ValueC.Count > 0 && TableManager.GetValueFromUnionCell(data.ValueC[0], level) > 0)
        {
            for (int j = 0; j < data.ValueC.Count; ++j)
            {
                int buffInfoID = TableManager.GetValueFromUnionCell(data.ValueC[j], level);
                if (!buffRelateLevel)
                {
                    monster.buffController.TryAddBuff(buffInfoID);
                }
                else
                {
                    monster.buffController.TryAddBuffInfo(buffInfoID, owner, level);
                }
            }
        }
    }
}
