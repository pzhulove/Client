
//圣骑士自然恢复

using GameClient;
using ProtoTable;
using UnityEngine;

public class Skill3723 : Skill3715
{
    public Skill3723(int sid, int skillLevel) : base(sid, skillLevel) { }
    public override void OnInit()
    {
        base.OnInit();
        cancelSetMonsterDead = false;
    }
}

//快速愈合
public class Skill3701 : Skill3715
{
    public Skill3701(int sid, int skillLevel) : base(sid, skillLevel) { }
    public override void OnInit()
    {
        base.OnInit();
        cancelSetMonsterDead = false;
    }
}

//圣愈之风
public class Skill3715 : BeSkill
{
    public Skill3715(int sid, int skillLevel) : base(sid, skillLevel) { }

    private int[] useCountMax = new int[2];   //技能最大使用次数(PVE|PVP)
    private int[] monsterIdArr = new int[2];   //护盾怪物ID（PVE|PVP）

    private int curUseCount = 0;    //技能当前使用次数
    private int maxCount = 0;   //当前技能最大使用次数

    public bool cancelSetMonsterDead = true;
    private BeActor monster = null;
    private IBeEventHandle rebornHandle = null;
    private IBeEventHandle deadTowerHandle = null;
    private IBeEventHandle switch3v3NextHandle = null;
    private IBeEventHandle trainingPveResetSkillCDHandle = null;

    private bool pveNeedCheckFlag = true;       //Pve是否需要检测技能使用次数

    private string _skillIconPath = "UI/Image/NewPacked/NewBattle/Battle_OccuSkillIcon_Shengqishi.png:Paladin_icon-45";

    public static void SkillPreloadRes(SkillTable tableData)
    {
#if !LOGIC_SERVER
        PreloadManager.PreloadPrefab("UIFlatten/Prefabs/BattleUI/BattleUIComponent/BattleUIProfession");
#endif
    }

    public override void OnInit()
    {
        base.OnInit();
        OnPostInit();
    }

    public override void OnPostInit()
    {
        base.OnPostInit();

        useCountMax[0] = TableManager.GetValueFromUnionCell(skillData.ValueA[0], level);
        useCountMax[1] = TableManager.GetValueFromUnionCell(skillData.ValueA[1], level);

        maxCount = BattleMain.IsModePvP(battleType) ? useCountMax[1] : useCountMax[0];

        if (skillData.ValueB.Count > 0)
        {
            monsterIdArr[0] = TableManager.GetValueFromUnionCell(skillData.ValueB[0], level);
            monsterIdArr[1] = TableManager.GetValueFromUnionCell(skillData.ValueB[1], level);
        }

        if (owner != null && 
            owner.CurrentBeBattle != null && 
            !owner.CurrentBeBattle.HasFlag(BattleFlagType.SkillSpecialBug))
        {
            canPressJumpBackCancel = false;
        }
        startJumpBackCnacelFlag = "371501";
        endJumpBackCnacelFlag = "371502";

        if (!BattleMain.IsModePvP(battleType))
            pveNeedCheckFlag = false;

        //延时100ms是为了确保这个时候战斗UI界面已经加载
        owner.delayCaller.DelayCall(100, () =>
        {
            ResetUseCount(false);
        });
    }

    public override void OnStart()
    {
        base.OnStart();
        curUseCount++;
        SetUseCount();
        monster = null;

        RemoveHandles();
        rebornHandle = owner.RegisterEventNew(BeEventType.onReborn, args =>
        {
            ResetUseCount();
        });

        if (owner.CurrentBeScene != null)
        {
            deadTowerHandle = owner.CurrentBeScene.RegisterEventNew(BeEventSceneType.onDeadTowerPassFiveLayer,(args)=>
            {
                ResetUseCount();
            });

            switch3v3NextHandle = owner.CurrentBeScene.RegisterEventNew(BeEventSceneType.on3v3SwitchNext, (args) =>
            {
                ResetUseCount();
            });
        }
        
        trainingPveResetSkillCDHandle = owner.RegisterEventNew(BeEventType.onTrainingPveResetSkillCD, args =>
        {
            ResetUseCount();
        });

        if (cancelSetMonsterDead)
            RegisterSummon();
    }

    private void ResetUseCount(bool resetCurCount = true)
    {
        if (resetCurCount)
        {
            curUseCount = 0;
        }
        SetUseCount();
    }

    public override bool CanUseSkill()
    {
        if (pveNeedCheckFlag && curUseCount >= maxCount)
            return false;
        return base.CanUseSkill();
    }

    public override BeSkillManager.SkillCannotUseType GetCannotUseType()
    {
        if (pveNeedCheckFlag && curUseCount >= maxCount)
            return BeSkillManager.SkillCannotUseType.CAN_NOT_USE;
        return base.GetCannotUseType();
    }

    public override void OnCancel()
    {
        base.OnCancel();
        if (cancelSetMonsterDead)
            SetMonsterDead();
    }

    private void RegisterSummon()
    {
        int monsterId = BattleMain.IsModePvP(battleType) ? monsterIdArr[1] : monsterIdArr[0];
        handleB = owner.RegisterEventNew(BeEventType.onSummon, (args) =>
        {
            BeActor actor = args.m_Obj as BeActor;
            if (actor != null && actor.GetEntityData().MonsterIDEqual(monsterId))
            {
                monster = actor;
                handleC = actor.RegisterEventNew(BeEventType.onDead, eventParam =>
                {
                    owner.Locomote(new BeStateData((int)ActionState.AS_IDLE), true);
                });
            }
        });
    }

    private void RemoveHandles()
    {
        if (rebornHandle != null)
        {
            rebornHandle.Remove();
            rebornHandle = null;
        }
        if (deadTowerHandle != null)
        {
            deadTowerHandle.Remove();
            deadTowerHandle = null;
        }
        if(switch3v3NextHandle != null)
        {
            switch3v3NextHandle.Remove();
            switch3v3NextHandle = null;
        }
        if(trainingPveResetSkillCDHandle != null)
        {
            trainingPveResetSkillCDHandle.Remove();
            trainingPveResetSkillCDHandle = null;
        }
    }

    private void SetMonsterDead()
    {
        if (monster == null || monster.IsDead())
            return;
        monster.DoDead();
#if !LOGIC_SERVER
        monster.m_pkGeActor.SetActorVisible(false);
#endif
    }

    private void SetUseCount()
    {
        if (!pveNeedCheckFlag)
            return;
#if !LOGIC_SERVER
        if (owner != null && owner.isLocalActor)
        {
            var battleUI = BattleUIHelper.CreateBattleUIComponent<BattleUIProfession>();;
            if (battleUI != null)
                battleUI.SetSkillUseCount(skillID, maxCount - curUseCount, _skillIconPath);
        }
#endif
    }
}
