using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 新的铁锤怪物的技能和异变章鱼的技能相同
/// </summary>
public class Skill21436  : Skill20179
{
    public Skill21436(int sid, int skillLevel) : base(sid, skillLevel) { }
}

/// <summary>
/// 章鱼旋转技能
/// </summary>
public class Skill20179 : BeSkill
{
    public Skill20179(int sid, int skillLevel) : base(sid, skillLevel){ }

    private VInt speedX = 70000;    //X轴速度
    private VInt speedY = 70000;    //Y轴速度

    public override void OnInit()
    {
        base.OnInit();
        speedX = TableManager.GetValueFromUnionCell(skillData.ValueA[0], level);
        speedY = TableManager.GetValueFromUnionCell(skillData.ValueB[0], level);
    }

    public override void OnEnterPhase(int phase)
    {
        base.OnEnterPhase(phase);
        if (phase == 2)
        {
            owner.SetMoveSpeedX(owner.GetFace() ? -speedX : speedX);
            owner.SetMoveSpeedY(owner.GetFace() ? -speedY : speedY);
        }
    }
}
