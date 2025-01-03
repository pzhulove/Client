using System;
using System.Collections.Generic;
using UnityEngine;
using GameClient;

//被指定出发效果命中n次后 强制释放某个技能
public class Mechanism1098 : BeMechanism
{
    private List<int> mEffectId = new List<int>();
    private int mTotalNum = 10;
    private int mSkillId = 0;
    private string mBossMsg;
    protected int mBeHitNum = 0;

    public Mechanism1098(int mid, int lv) : base(mid, lv) { }

    public override void OnInit()
    {
        mBossMsg = data.StringValueA[0];

        mEffectId.Clear();
        for(int i = 0; i < data.ValueA.Count; ++i)
        {
            mEffectId.Add(TableManager.GetValueFromUnionCell(data.ValueA[i], level));
        }
        
        mTotalNum = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        mSkillId = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
    }

    public override void OnReset()
    {
        mEffectId.Clear();
        mBossMsg = "";
        mBeHitNum = 0;
    }

    public override void OnStart()
    {
        mBeHitNum = 0;
        if (owner.CurrentBeBattle != null && owner.CurrentBeBattle.dungeonPlayerManager != null)
        {
            var dungeonPlayers = owner.CurrentBeBattle.dungeonPlayerManager.GetAllPlayers();
            if(dungeonPlayers != null)
            {
                if (dungeonPlayers.Count > 0 && dungeonPlayers[0].playerActor != null)
                {
                    handleA = dungeonPlayers[0].playerActor.RegisterEventNew(BeEventType.onHit, OnBeHitEvent);
                    //handleA = dungeonPlayers[0].playerActor.RegisterEvent(BeEventType.onHit, OnBeHitEvent);
                }
                if(dungeonPlayers.Count > 1 && dungeonPlayers[1].playerActor != null)
                {
                    handleB = dungeonPlayers[1].playerActor.RegisterEventNew(BeEventType.onHit, OnBeHitEvent);
                    //handleB = dungeonPlayers[1].playerActor.RegisterEvent(BeEventType.onHit, OnBeHitEvent);
                }
                if(dungeonPlayers.Count > 2 && dungeonPlayers[2].playerActor != null)
                {
                    handleC = dungeonPlayers[2].playerActor.RegisterEventNew(BeEventType.onHit, OnBeHitEvent);
                    //handleC = dungeonPlayers[2].playerActor.RegisterEvent(BeEventType.onHit, OnBeHitEvent);
                }
            }
            
            ShowHeadDialog(mTotalNum);
        }
            
        
    }

    public override void OnFinish()
    {
        ShowHeadDialog(0, true);
    }

    private void OnBeHitEvent(BeEvent.BeEventParam param)
    {
        //if(args.Length >= 7)
        //{
            int hurtId = param.m_Int4;
            if (mEffectId.Contains(hurtId)) //判断出发效果
            {
                mBeHitNum++;
                ShowHeadDialog(mTotalNum - mBeHitNum);
                if(mBeHitNum >= mTotalNum)//触发累计事件
                {
                    ShowHeadDialog(0, true);
                    //取消当前并释放指定技能
                    BeUtility.CancelCurrentSkill(owner);
                    var skill = owner.GetSkill(mSkillId);
                    if(skill != null)
                    {
                        skill.ResetCoolDown();
                        owner.UseSkill(mSkillId, true);
                    }
                    owner.RemoveMechanism(this);
                }
            }
        //}
    }
    //非验证服务器用显示消息函数
    private void ShowHeadDialog(int num, bool hide = false)
    {
#if !SERVER_LOGIC
        float lifeTime = 3.5f;
        if (!hide)
        {
            lifeTime = 999f;
        }
        if (string.IsNullOrEmpty(mBossMsg))
        {
            mBossMsg = "剩余{0}";
        }
        if (owner != null && owner.m_pkGeActor != null)
        {
            owner.m_pkGeActor.ShowHeadDialog(string.Format(mBossMsg, num), hide, false, false, false, lifeTime, false);
        }
#endif
    }
}

