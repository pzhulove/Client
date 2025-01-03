using System.Collections.Generic;
using GameClient;

//团本克西拉 释放技能取消无敌，技能取消恢复无敌
public class Mechanism2081 : BeMechanism
{
    private int buffId = 38;
    private List<int> skillIdList = new List<int>();
    private bool removeBuffFlag = false;
	public Mechanism2081(int mid, int lv):base(mid, lv){ }

    public override void OnInit()
    {
        buffId = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        skillIdList.Clear();
        for(int i = 0; i < data.ValueB.Count; ++i)
        {
            skillIdList.Add(TableManager.GetValueFromUnionCell(data.ValueB[i], level));
        }
        var valueC = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
        removeBuffFlag = 0 == valueC ? false : true;
    }

    public override void OnStart()
    {
        //saveAddBuff = null;
        saveAddBuffPID = 0;
        if(owner != null)
        {
            handleA = owner.RegisterEventNew(BeEventType.onCastSkill, OnSkillStart);
            handleB = owner.RegisterEventNew(BeEventType.onSkillCancel, OnSkillEnd);
            handleC = owner.RegisterEventNew(BeEventType.onCastSkillFinish, OnSkillEnd);
        }
    }

    public override void OnFinish()
    {
        //saveAddBuff = null;
        saveAddBuffPID = 0;
    }
    //private BeBuff saveAddBuff;
    private int saveAddBuffPID;
    private void OnSkillStart(BeEvent.BeEventParam args)
    {
        var skillId = args.m_Int;
        if (skillIdList.Contains(skillId))
        {
            if (removeBuffFlag)
            {
                var buff = owner.buffController.TryAddBuff(buffId, -1, level);
                if (buff != null)
                    saveAddBuffPID = buff.PID;
            }
            else
            {
            owner.buffController.RemoveBuff(buffId);//注意到这个RemoveBuff的逻辑应该是该buffId的所有buff都会被移除
            }
        }
    }

    /// <summary>
    /// 对应恢复buff回调事件
    /// </summary>
    /// <param name="args"></param>
    private void OnSkillEnd(BeEvent.BeEventParam args)
    {
        var skillId = args.m_Int;
        if (skillIdList.Contains(skillId))
        {
            if (removeBuffFlag)
            {
                var buff = owner.buffController.GetBuffByPID(saveAddBuffPID);
                if(null != buff)
                {
                    owner.buffController.RemoveBuff(buff);
                }
            }
            else
            {
            owner.buffController.TryAddBuff(buffId, -1, level);
            }
        }
    }
}

