using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//式神：白虎
public class Skill3609 : BeSkill
{
    protected int[] monsterCountLimit = new int[2] { 4, 3};                //召唤数量上限(Pve|Pvp)
    protected VInt summonOffset = 2000;                  //白虎召唤位置偏移
    protected VInt summonDisLimit = 2000;                //召唤最小距离判定
    
    protected int[] monsterIdArr = new int[2] { 93060031, 93000034 };       //召唤怪物ID（Pve|Pvp）
    protected int mechanismMonsterId = 93030031;    //机制挂载怪物ID
    protected string mechanismFrame = "360901";     //机制挂载怪物帧标志

    public Skill3609(int sid, int skillLevel) : base(sid, skillLevel) { }

    public override void OnInit()
    {
        base.OnInit();
        monsterCountLimit[0] = TableManager.GetValueFromUnionCell(skillData.ValueA[0], level);
        monsterCountLimit[1] = TableManager.GetValueFromUnionCell(skillData.ValueA[1],level);
        summonOffset = VInt.NewVInt(TableManager.GetValueFromUnionCell(skillData.ValueB[0], level),GlobalLogic.VALUE_1000);
        summonDisLimit = VInt.NewVInt(TableManager.GetValueFromUnionCell(skillData.ValueC[0], level),GlobalLogic.VALUE_1000);
    }

    public override void OnStart()
    {
        handleA = owner.RegisterEventNew(BeEventType.onSkillCurFrame, (args) =>
        //handleA = owner.RegisterEvent(BeEventType.onSkillCurFrame, (object[] args) =>
        {
            string flag = args.m_String;
            if (flag == mechanismFrame)
            {
                DoSummon();
            }
        });
    }

    public override bool CanUseSkill()
    {
        BeSkillManager.SkillCannotUseType type = GetSkillUseType();
        if (type != BeSkillManager.SkillCannotUseType.CAN_USE)
            return false;
        return base.CanUseSkill();
    }

    public override BeSkillManager.SkillCannotUseType GetCannotUseType()
    {
        BeSkillManager.SkillCannotUseType type = GetSkillUseType();
        if (type != BeSkillManager.SkillCannotUseType.CAN_USE)
            return type;
        return base.GetCannotUseType();
    }

    //获取技能不能释放的原因
    protected BeSkillManager.SkillCannotUseType GetSkillUseType()
    {
        BeSkillManager.SkillCannotUseType type = BeSkillManager.SkillCannotUseType.CAN_USE;
        List<BeActor> monsterList = GamePool.ListPool<BeActor>.Get();
        int monsterId = BattleMain.IsModePvP(battleType) ? monsterIdArr[1] : monsterIdArr[0];
        owner.CurrentBeScene.FindMonsterByIDAndCamp(monsterList, monsterId, owner.GetCamp());
        int limitCount = BattleMain.IsModePvP(battleType) ? monsterCountLimit[1] : monsterCountLimit[0];
        if (monsterList.Count >= limitCount)
            type = BeSkillManager.SkillCannotUseType.MONSTER_COUNT_LIMITM;
        VInt offset = owner.GetFace() ? -summonOffset : summonOffset;
        VInt3 summonPos = owner.GetPosition();
        summonPos.x += offset.i;
        for (int i = 0; i < monsterList.Count; i++)
        {
            int dis = (monsterList[i].GetPosition() - summonPos).magnitude;
            if (dis <= summonDisLimit)
            {
                type = BeSkillManager.SkillCannotUseType.MONSTER_DIS_LIMITM;
                break;
            }
        }
        GamePool.ListPool<BeActor>.Release(monsterList);
        return type;
    }

    //召唤白虎机制挂载怪物
    protected void DoSummon()
    {
        int count = owner.CurrentBeScene.GetMonsterCountByIdCamp(mechanismMonsterId,owner.GetCamp());
        if (count < 1)
            owner.DoSummon(mechanismMonsterId);
    }
   
}
