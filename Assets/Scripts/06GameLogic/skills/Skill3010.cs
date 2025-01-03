using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoTable;

//念气波
public class Skill3010 : BeSkill
{
    protected int m_XuNianPaoPhaseId = 301012;          //蓄念炮释放的时候使用的ID
    protected int m_XuNianPaoId = 3111;                 //蓄念炮表里面配置的Id 目前只用于获取CD时间
    IBeEventHandle m_XuNianPaoHandle = null;             //监听蓄念炮技能的释放

    public Skill3010(int sid, int skillLevel): base(sid, skillLevel)
	{
        
	}

	public override void OnStart ()
    {
        m_XuNianPaoHandle = owner.RegisterEventNew(BeEventType.onPreSetSkillAction, (GameClient.BeEvent.BeEventParam param) =>
        {
            //int[] skillIdList = (int[])args[0];
            if (param.m_Int == m_XuNianPaoPhaseId)
            {
                var skill = owner.GetSkill(m_XuNianPaoId);
                if (skill != null)
                {
                    if (BattleMain.IsChijiNeedReplaceSkillId(skill.skillID,battleType))
                    {
                        var chijiSkillMapTable = TableManager.instance.GetTableItem<ChijiSkillMapTable>(skill.skillID);
                        SetChargeCdTime(TableManager.GetValueFromUnionCell(chijiSkillMapTable.RefreshTimePVP, level));
                    }
                    else
                    {
                        if (!BattleMain.IsModePvP(battleType))
                            SetChargeCdTime(TableManager.GetValueFromUnionCell(skill.skillData.RefreshTime, level));
                        else
                            SetChargeCdTime(TableManager.GetValueFromUnionCell(skill.skillData.RefreshTimePVP, level));
                    }
                }
            }
        });
	}

    public override void OnCancel()
    {
        base.OnCancel();
        RemoveBuff();
    }

    private void RemoveBuff()
    {
        owner.buffController.RemoveBuff(1);
    }

    public override void OnFinish()
    {
        RemoveBuff();
        if (m_XuNianPaoHandle != null)
        {
            m_XuNianPaoHandle.Remove();
        }
    }
}
