using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

//玩家接触蜘蛛陷阱

public class Buff521211 : BeBuff
{

    protected VRate m_DoHurtToOwner = VRate.half;//0.5f;     //掉血百分比 
    protected int m_HurtTime = 1000;            //延时多久掉血

    public Buff521211(int bi, int buffLevel, int buffDuration, int attack = 0): base(bi, buffLevel, buffDuration, attack)
    {

    }

    public override void OnReset()
    {
        m_DoHurtToOwner = VRate.half;
        m_HurtTime = 1000;
    }

    public override void OnInit()
    {
        m_DoHurtToOwner = TableManager.GetValueFromUnionCell(buffData.ValueA[0], level);
        m_HurtTime = TableManager.GetValueFromUnionCell(buffData.ValueB[0], level);
    }

    public override void OnStart()
    {
        DoFunction();
    }

    //执行功能
    protected void DoFunction()
    {
        owner.SetTag((int)AState.ACS_FALL, true);
        owner.SetTag((int)AState.AST_DROPSCENE, true);       //设置玩家可以从地面掉到场景下方
        owner.moveZSpeed = 1;
        owner.buffController.TryAddBuff((int)GlobalBuff.INVINCIBLE);
        owner.delayCaller.DelayCall(m_HurtTime,() =>
        {
            if (owner.buffController.HasBuffByID((int)GlobalBuff.INVINCIBLE)!=null)
            {
                owner.buffController.RemoveBuff((int)GlobalBuff.INVINCIBLE);
            }
            owner.moveZSpeed = 0;
            int maxHP = owner.GetEntityData().GetMaxHP();
            owner.DoHurt(maxHP * m_DoHurtToOwner.factor);       //1秒后让玩家掉血
            VInt3 pos = owner.CurrentBeBattle.dungeonManager.GetDungeonDataManager().GetBirthPosition();
            owner.SetTag((int)AState.ACS_FALL, false);
            owner.SetTag((int)AState.AST_DROPSCENE, false);
            owner.SetPosition(pos);                                             //将玩家拉回出生点
            owner.buffController.TryAddBuff((int)GlobalBuff.INVINCIBLE,1000);   //加上一个无敌buff  持续1秒
        });
    }
}
