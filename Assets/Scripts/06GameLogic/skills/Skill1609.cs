using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameClient;
using ProtoTable;

public class Skill1609 : BeSkill
{
    IBeEventHandle handler;
	IBeEventHandle handler2;
    int replaceNormalAttackID;
	int backupAttackID;
	//int healValue;
	int healBuffID;
    int buffID;
	int hpReduce;
    bool started = false;
	static string effect = "Effects/Hero_Kuangzhan/Xuezhikuangbaoerdao/Prefab/Eff_xuezhikuangbao_xueqiu";

    protected IBeEventHandle m_HandleComboHandle = null;
    protected int m_RegisterComboSkillId = 1601;
    protected int m_ReplaceComboSkillId = 1623;
    protected int m_ShixueBuffIdPve = 162209;

    public Skill1609(int sid, int skillLevel) : base(sid, skillLevel)
    {
        
    }

    public static void SkillPreloadRes(SkillTable tableData)
    {
#if !LOGIC_SERVER
        PreloadManager.PreloadPrefab(effect);
#endif
    }

    public override void OnInit()
    {
        replaceNormalAttackID = TableManager.GetValueFromUnionCell(skillData.ValueA[0], level);
		healBuffID = TableManager.GetValueFromUnionCell (skillData.ValueB[0], level);
        buffID = TableManager.GetValueFromUnionCell(skillData.ValueD[0], level);

        //这里也重新获取下
        hpReduce = (int)(TableManager.GetValueFromUnionCell(skillData.ValueE[0], level) / 10);
    }

    public override void OnPostInit()
    {
        base.OnPostInit();
        hpReduce = (int)(TableManager.GetValueFromUnionCell(skillData.ValueE[0], level) / 10);

        if (!BattleMain.IsModePvP(battleType))
        {
            RemoveHandle();
            RegisterHandle();
        }   
    }

    public override void OnStart()
    {
        if (!started)
        {
            started = true;
            DoEffect();
        }
        else
        {
            started = false;
            Restore();
        }
    }

    /// <summary>
    /// 监听事件
    /// </summary>
    protected void RegisterHandle()
    {
        m_HandleComboHandle = owner.RegisterEventNew(BeEventType.onReplaceComboSkill, OnReplaceComboSkill);
    }

    /// <summary>
    /// 移除事件
    /// </summary>
    protected void RemoveHandle()
    {
        if (m_HandleComboHandle != null)
        {
            m_HandleComboHandle.Remove();
            m_HandleComboHandle = null;
        }
    }

    /// <summary>
    /// 如果是嗜血状态下  则二刀流第1段Combo第五段
    /// </summary>
    protected void OnReplaceComboSkill(BeEvent.BeEventParam args)
    {
        if (owner.buffController.HasBuffByID(m_ShixueBuffIdPve) == null)
            return;
        if (args.m_Int != m_RegisterComboSkillId)
            return;
        args.m_Int = m_ReplaceComboSkillId;
    }

    void DoEffect()
    {
        if (owner != null)
        {
            var r = GlobalLogic.VALUE_1000;
            for (int i = 0; i < owner.MechanismList.Count; i++)
            {
                var m = owner.MechanismList[i] as Mechanism123;
                if (m != null)
                    r += m.hpRate;
            }
            //每10秒扣血
            hpReduce = (int)(TableManager.GetValueFromUnionCell(skillData.ValueE[0], level) * VFactor.NewVFactor(r, GlobalLogic.VALUE_10000));

            //替换普攻
            backupAttackID = owner.GetEntityData().normalAttackID;
            owner.GetEntityData().normalAttackID = replaceNormalAttackID;
			owner.GetEntityData().ChangeHPReduce(hpReduce);

            AddBuff();
            AddSkillBuff();

            //死亡回血
            handler = owner.RegisterEventNew(BeEventType.onKill, args =>
            //handler = owner.RegisterEvent(BeEventType.onKill, (object[] args) =>
            {
                BeActor target = args.m_Obj as BeActor;
                if (target != null && target.buffController.HasBuffByType(BuffType.BLEEDING) != null)
                {
                    target.RegisterEventNew(BeEventType.onRemove, (args2) =>
                    {
#if !SERVER_LOGIC 

                        var startPos = target.GetPosition();
                        var curPos = owner.GetPosition();

                        Vector3 startV = new Vector3(0, 0, 0);
                        var trail = TrailManager.AddParabolaTrail(
                            target.GetGePosition() + Global.Settings.offset,
                            owner,
                            startV,
                            Vector3.zero,
									effect);

 #endif


						AddHealBuff();
								/*
                        if (trail != null)
                        {
                            trail.SetReachCallBack(() =>
                            {
								AddHealBuff();
                            });
                        }
                        */
                    }
                    );
                }
            }
            );

			//血少取消技能
			handler2 = owner.RegisterEventNew(BeEventType.onHPChange, (args3) => {
				if (owner.GetEntityData().GetHP() <= hpReduce)
				{
					PressAgainCancel();
					//OnCancel();
				}
			});

        }
    }

    void AddSkillBuff(bool remove=false)
    {
        for (int i = 0; i < skillData.ValueC.Count; ++i)
        {
            int skillID = skillData.ValueC[i].fixInitValue;
            int buffID = skillData.ValueC[i].fixLevelGrow;

            if (!remove)
            {
                int buffLevel = level;
                owner.buffController.AddBuffForSkill(buffID, buffLevel, -1, new List<int> { skillID });
            }
            else
            {
                int buffLevel = level;
                owner.buffController.RemoveBuff(buffID);
            }
        }
    }

    void AddBuff(bool remove = false)
    {
        if (remove)
        {
            owner.buffController.RemoveBuff(buffID);
        }
        else
        {
            int buffLevel = level;
            owner.buffController.TryAddBuff(buffID, -1, buffLevel);
        }
    }

	void AddHealBuff()
	{
        int buffID = BattleMain.IsModePvP(owner.battleType) ? 162210 : 162209;
        if (owner.buffController.HasBuffByID(buffID) != null)
        {
            BeSkill skill = owner.GetSkill(1622);
            if (skill != null)
            {
                int healBuff = BattleMain.IsModePvP(owner.battleType) ? 162204 : 162203;
                owner.buffController.TryAddBuff(healBuff, GlobalLogic.VALUE_1000, skill.level);
            }
        }
        owner.buffController.TryAddBuff(healBuffID, GlobalLogic.VALUE_1000, level);
    }

    void Restore()
    {
        if (owner != null)
        {
            owner.GetEntityData().normalAttackID = backupAttackID;
			owner.GetEntityData().ChangeHPReduce(-hpReduce);

            if (handler != null)
            {
                handler.Remove();
                handler = null;
            }

			if (handler2 != null)
			{
				handler2.Remove();
				handler2 = null;
			}

            AddBuff(true);
            AddSkillBuff(true);
        }
    }

	public override void OnCancel ()
	{
		if (started)
		{
			started = false;
			Restore();

		}
	}
}
