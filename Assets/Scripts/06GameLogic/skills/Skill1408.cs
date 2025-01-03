/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
/// <summary>
/// 1.机械狗在召唤出来后，会主动追击敌人（AI处理）
/// 2.机械狗在接近敌人多少范围内，并持续2秒，则会自爆
/// 3.如果玩家再次点技能按键，则立即自爆
/// </summary>
public class Skill1408 : SummonSkillButton
{
    public Skill1408(int sid, int skillLevel) : base(sid, skillLevel)
    { }

    public override void OnInit()
    {
        base.OnInit();
        MonsterId = 94190031;
    }

    public override void OnClickAgain()
    {
        SetMonsterDead();
    }
    
    /// <summary>
    /// 设置机械犬死亡
    /// </summary>
    protected void SetMonsterDead()
    {
        if (Monster == null)
            return;
        Monster.DoDead();
    }
}
*/
