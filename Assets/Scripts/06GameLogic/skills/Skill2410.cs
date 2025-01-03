using System.Collections.Generic;


/// <summary>
/// 催眠技能
///
/// 配置
/// A:在一定时间后可再次点击
/// B:给召唤物的上buffInfo(绑定机制3000)
/// </summary>
public class Skill2410 : BeSkill
{
    private int mClickAgainTime;
    private int mClickAgainTimeAcc;
    private int mSleepBuffInfoID;
    
    public Skill2410(int sid, int skillLevel) : base(sid, skillLevel)
    {
    }

    public override void OnInit()
    {
        base.OnInit();
        mClickAgainTime = TableManager.GetValueFromUnionCell(skillData.ValueA[0], level);
        mSleepBuffInfoID = TableManager.GetValueFromUnionCell(skillData.ValueB[0], level);
    }

    public override void OnStart()
    {
        base.OnStart();
        mClickAgainTimeAcc = 0;
        
        List<BeActor> summonList = GamePool.ListPool<BeActor>.Get();
        owner.CurrentBeScene?.FindSummonMonster(summonList, owner);
        for (var i = 0; i < summonList.Count; i++)
        {
            var summon = summonList[i];
            summon.buffController.TryAddBuffInfo(mSleepBuffInfoID, owner, level);
        }

        GamePool.ListPool<BeActor>.Release(summonList);
    }

    public override void OnUpdate(int iDeltime)
    {
        base.OnUpdate(iDeltime);
        mClickAgainTimeAcc += iDeltime;
        if (mClickAgainTimeAcc >= mClickAgainTime)
        {
            skillButtonState = SkillState.WAIT_FOR_NEXT_PRESS;
            ChangeButtonEffect();
        }
    }

    public override void OnClickAgain()
    {
        base.OnClickAgain();
        
        List<BeActor> summonList = GamePool.ListPool<BeActor>.Get();
        owner.CurrentBeScene?.FindSummonMonster(summonList, owner);
        for (var i = 0; i < summonList.Count; i++)
        {
            var summon = summonList[i];
            summon.buffController.RemoveBuffByBuffInfoID(mSleepBuffInfoID);
        }

        GamePool.ListPool<BeActor>.Release(summonList);
    }
}