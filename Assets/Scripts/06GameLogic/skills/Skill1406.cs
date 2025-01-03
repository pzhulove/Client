/*
using System.Collections;
using System.Collections.Generic;
using GameClient;

/// <summary>
/// 1.场上会出现一个向尼尔狙击的透明瞄准镜。这个敌我双方是可见的
/// 2.这个瞄准镜会始终跟随敌人
/// 3.瞄准镜的出现随机在敌人附近周围
/// 4.如果再次点击技能按键，指定怪物会发动攻击。（敌人要处于瞄准镜内环才会受到攻击）
/// </summary>
public class Skill1406 : SummonSkill
{
    public Skill1406(int sid, int skillLevel) : base(sid, skillLevel){ }
    protected int MechanismId = 6201;
    protected bool NewAnim = true;

    public override void OnInit()
    {
        base.OnInit();
        if (skillData.ValueB.Count > 0)
        {
            NewAnim = false;
        }
    }

    protected override void OnSummon(BeActor monster)
    {
        if (!NewAnim)
            base.OnSummon(monster);
    }

    public override void OnClickAgain()
    {
        if (NewAnim)
            return;
        
        DoAttack();
    }

    public void DoAttack()
    {
        if (NewAnim)
        {
            var mechanism = GetMonsterMechanism<Mechanism1147>(6215);
            if (mechanism == null)
                return;
        
            mechanism.ClickAttack();
            ResetButtonEffect();
        }
        else
        {
            var mechanism = GetMonsterMechanism<Mechanism1123>(MechanismId);
            if (mechanism == null)
                return;
        
            mechanism.ClickAttack();
            ResetButtonEffect();
        }
    }

    public void DoRealAttack(int bulletNum,int pid)
    {
        if (bulletNum <= 0)
            return;

        if (!NewAnim)
        {
            var mechanism = GetMonsterMechanism<Mechanism1123>(MechanismId);
            if (mechanism == null)
                return;
            mechanism.DoRealAttack(pid);
        }
        else
        {
            var mechanism = GetMonsterMechanism<Mechanism1147>(6215);
            if (mechanism == null)
                return;
            mechanism.DoRealAttack(pid);
        }
        
    }

    /// <summary>
    /// 点亮技能按键
    /// </summary>
    public void LightSkillButton()
    {
        if (NewAnim)
            return;
        
        if (!CanClickAgain)
            return;
        
        skillButtonState = SkillState.WAIT_FOR_NEXT_PRESS;
        ChangeButtonEffect();
    }
    
    //同步枪口位置
    public void DoSyncSight(int x, int z)
    {
        if (NewAnim)
        {
            var mechanism = GetMonsterMechanism<Mechanism1147>(6215);
            if (mechanism == null)
                return;
        
            mechanism.DoSyncSight(x, z);
        }
        else
        {
            var mechanism = GetMonsterMechanism<Mechanism1123>(MechanismId);
            if (mechanism == null)
                return;
        
            mechanism.DoSyncSight(x, z);
        }

    }
}
*/
