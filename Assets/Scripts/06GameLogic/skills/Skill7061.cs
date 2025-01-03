using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoTable;
using System;

//蜘蛛不能在出生点周围挖洞
public class Skill7061 : BeSkill
{
    protected VInt3 m_SpiderBornPoint = new VInt3();          //蜘蛛出生点
    protected VInt m_TrapRadius =  VInt.one.i * 5;                       //陷阱半径

    public Skill7061(int sid, int skillLevel): base(sid, skillLevel)
	{
        
	}

    public override void OnInit()
    {
        m_TrapRadius = TableManager.GetValueFromUnionCell(skillData.ValueA[0], level);
    }

	public override void OnStart ()
	{

        m_SpiderBornPoint = owner.CurrentBeBattle.dungeonManager.GetDungeonDataManager().GetBirthPosition();
    }

    public override bool CanUseSkill()
    {
        VInt3 pos = owner.GetPosition();
        int absX = Math.Abs(pos.x-m_SpiderBornPoint.x);
        int absY =Math.Abs(pos.y-m_SpiderBornPoint.y);
        int distance = IntMath.Sqrt(absX * absX + absY * absY);
        if (distance <= m_TrapRadius)
        {
            //在出生点范围内不能挖陷阱
            return false;
        }
        return true;
    }
}
