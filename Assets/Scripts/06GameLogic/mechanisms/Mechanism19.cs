using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameClient;
/*
 * 
*/

public class Mechanism19 : BeMechanism {

	int delay = 3000;
	static string text = "复活中";
	int monsterID = 102;
	int skillID = 5449;
    int tipId = 0;

    protected bool m_StopRebornFlag = false;    //如果监听的怪物死亡 则自己停止复活 并且关闭复活读条和关卡技能提示条(暂时只改了变现 逻辑上没有做处理)
    protected bool m_RebornFlag = false;

    public Mechanism19(int mid, int lv):base(mid, lv){}

    public override void OnReset()
    {
        m_StopRebornFlag = false;
        m_RebornFlag = false;
    }

	public override void OnInit ()
	{
		delay = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
		monsterID = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
		skillID = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
        tipId = TableManager.GetValueFromUnionCell(data.ValueD[0], level);
        m_StopRebornFlag = TableManager.GetValueFromUnionCell(data.ValueE[0], level) == 0 ? false : true;
    }

	public override void OnStart ()
	{
		handleA = owner.RegisterEventNew(BeEventType.onDead, eventParam =>
        {
			if (IsTargetAlive(monsterID))
			{
                m_RebornFlag = true;
                var bar = owner.StartSpellBar(eDungeonCharactorBar.Progress, delay, true, text);
                if(bar != null)
                {
                    bar.alwaysRefreshUI = true;
                }

                GameClient.UIEventSystem.GetInstance().SendUIEvent(GameClient.EUIEventID.BattleDoubleBossTips, delay,tipId);

				owner.delayCaller.DelayCall(delay, ()=>{

					if (IsTargetAlive(monsterID))
					{
                        m_RebornFlag = false;
                        owner.Reborn();
						if (owner.CanUseSkill(skillID))
							owner.UseSkill(skillID, true);
					}
					else {
						owner.SetLifeState(EntityLifeState.ELS_CANREMOVE);
					}

				});
			}

		});

        handleB = owner.RegisterEventNew(BeEventType.onMarkRemove, (GameClient.BeEvent.BeEventParam param) => {

			if (IsTargetAlive(monsterID))
			{
                param.m_Bool = false;
			}
		});

        RegisterTargetMonsterDead();
    }

    /// <summary>
    /// 监听目标怪物死亡
    /// </summary>
    protected void RegisterTargetMonsterDead()
    {
#if !LOGIC_SERVER
        if (!m_StopRebornFlag)
            return;
        if (owner.CurrentBeScene == null)
            return;
        sceneHandleA = owner.CurrentBeScene.RegisterEventNew(BeEventSceneType.onMonsterDead, RegisterDeadEvent);
#endif
    }

    /// <summary>
    /// 监听死亡事件
    /// </summary>
    protected void RegisterDeadEvent(BeEvent.BeEventParam args)
    {
#if !LOGIC_SERVER
        var actor = (BeActor)args.m_Obj;
        if (actor == null)
            return;
        if (actor.GetEntityData().monsterID != monsterID)
            return;
        RemoveEffect();
#endif
    }

    /// <summary>
    /// 清除读条和场景滚动条Tips
    /// </summary>
    protected void RemoveEffect()
    {
#if !LOGIC_SERVER
        if (!m_RebornFlag)
            return;
        SystemNotifyManager.ClearDungeonSkillTip();
        owner.StopSpellBar(eDungeonCharactorBar.Progress);
#endif
    }

	bool IsTargetAlive(int monsterID)
	{
		List<BeActor> results = GamePool.ListPool<BeActor>.Get();

		owner.CurrentBeScene.FindMonsterByID(results, monsterID);
		bool ret = results.Count > 0;

		GamePool.ListPool<BeActor>.Release(results);

		return ret;
	}

    public override void OnFinish()
    {
        RemoveEffect();
    }
}
