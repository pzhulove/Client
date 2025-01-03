using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Experimental.UIElements;

/*
龙虎啸机制
 */

public class Mechanism38 : BeMechanism
{
    protected int m_ReplaceNormalAttackId = 0;          //替换普攻的技能Id
    protected int m_BackupAttackId = 0;                 //备份原来的普攻Id
    protected int m_SkillId = 0;                        //替换的技能Id
    protected int m_BackupSkillId = 0;                  //被替换的技能Id
    protected int m_PveBuffId = 0;                      //Pve添加的BuffId
    protected int m_PvpBuffId = 0;                      //Pvp添加的BuffId

    IBeEventHandle m_ReplaceSkillHandle = null;          //替换技能
    IBeEventHandle m_OwnerDeadHandle = null;             //监听自己死亡

    public Mechanism38(int mid, int lv) : base(mid, lv){}

    public override void OnInit()
    {
        m_ReplaceNormalAttackId = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        m_BackupSkillId = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        m_SkillId = TableManager.GetValueFromUnionCell(data.ValueB[1], level);
        m_PveBuffId = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
        m_PvpBuffId = TableManager.GetValueFromUnionCell(data.ValueD[0], level);
    }

    public override void OnReset()
    {
        m_BackupAttackId = 0;
        m_ReplaceSkillHandle = null;
        m_OwnerDeadHandle = null;
    }

	public override void OnStart ()
	{
        DoEffect();

        if (m_OwnerDeadHandle != null)
        {
            m_OwnerDeadHandle.Remove();
        }
        m_OwnerDeadHandle = owner.RegisterEventNew(BeEventType.onDead, eventParam =>
        {
            RemoveEffect();
        });
	}

    //添加技能效果
    protected void DoEffect()
    {
        ReplaceAttackId();
        AddBuff();
        ReplaceSkill();
    }

    //移除技能效果
    protected void RemoveEffect()
    {
        ReplaceAttackId(true);
        AddBuff(true);
        ReplaceSkill(true);
    }

    //替换普攻技能
    protected void ReplaceAttackId(bool restore = false)
    {
        if (!restore)
        {
            m_BackupAttackId = owner.GetEntityData().normalAttackID;
            owner.GetEntityData().normalAttackID = m_ReplaceNormalAttackId;
        }
        else
        {
            owner.GetEntityData().normalAttackID = m_BackupAttackId;
        }
    }

    //替换技能Id
    protected void ReplaceSkill(bool restore = false)
    {
        if (!restore)
        {
            RemoveReplaceSkillHandle();
            m_ReplaceSkillHandle = owner.RegisterEventNew(BeEventType.onPreSetSkillAction, (GameClient.BeEvent.BeEventParam param) =>
            {
                //int[] skillIdList = (int[])args[0];
                //int curSkillId = skillIdList[0];

                if (param.m_Int == m_BackupSkillId)
                {
                    param.m_Int = m_SkillId;
                }
            });
        }
        else
        {
            RemoveReplaceSkillHandle();
        }
    }

    protected void RemoveReplaceSkillHandle()
    {
        if (m_ReplaceSkillHandle != null)
        {
            m_ReplaceSkillHandle.Remove();
        }
    }

    protected void AddBuff(bool remove = false)
    {
        int addBuffId = 0;
        if (!BattleMain.IsModePvP(battleType))
        {
            addBuffId = m_PveBuffId;
        }
        else
        {
            addBuffId = m_PvpBuffId;
        }
        if (!remove)
        {
            owner.buffController.TryAddBuff(addBuffId, -1, level);
        }
        else
        {
            if (owner.buffController.HasBuffByID(addBuffId) != null)
            {
                owner.buffController.RemoveBuff(addBuffId);
            }
        }
    }

    public override void OnFinish()
    {
        RemoveEffect();
    }
}