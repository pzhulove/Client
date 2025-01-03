using System.Collections.Generic;
using GameClient;
using ProtoTable;
using UnityEngine;

#region 生命源泉
public class Skill3712 : Skill3703
{
    public Skill3712(int sid, int skillLevel) : base(sid, skillLevel) { }

    private int[] useCountMax = new int[2];   //技能最大使用次数(PVE|PVP)

    private int curUseCount = 0;    //技能当前使用次数
    private int maxCount = 0;    //当前技能最大的使用次数
    private IBeEventHandle rebornHandle = null;
    private IBeEventHandle deadTowerHandle = null;
    private IBeEventHandle switch3v3NextHandle = null;
    private IBeEventHandle trainingPveResetSkillCDHandle = null;

    private string _skillIconPath = "UI/Image/NewPacked/NewBattle/Battle_OccuSkillIcon_Shengqishi.png:Paladin_icon-42";

    public static void SkillPreloadRes(ProtoTable.SkillTable tableData)
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

        useCountMax[0] = TableManager.GetValueFromUnionCell(skillData.ValueC[0], level);
        useCountMax[1] = TableManager.GetValueFromUnionCell(skillData.ValueC[1], level);

        maxCount = BattleMain.IsModePvP(battleType) ? useCountMax[1] : useCountMax[0];
        //延时100ms是为了确保这个时候战斗UI界面已经加载
        owner.delayCaller.DelayCall(100,()=> 
        {
            ResetUseCount(false);
        });
        addBuffTag = "371201";
    }

    public override void OnStart()
    {
        base.OnStart();

        curUseCount++;
        SetUseCount();

        RemoveHandles();
        rebornHandle = owner.RegisterEventNew(BeEventType.onReborn, args =>
        {
            ResetUseCount();
        });

        if (owner.CurrentBeScene != null)
        {
            deadTowerHandle = owner.CurrentBeScene.RegisterEventNew(BeEventSceneType.onDeadTowerPassFiveLayer, (args) =>
            {
                ResetUseCount();
            });

            switch3v3NextHandle = owner.CurrentBeScene.RegisterEventNew(BeEventSceneType.on3v3SwitchNext, (args) =>
            {
                ResetUseCount();
            });
        }
        
        trainingPveResetSkillCDHandle = owner.RegisterEventNew(BeEventType.onTrainingPveResetSkillCD,args => 
        {
            ResetUseCount();
        });
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
        if (curUseCount >= maxCount)
            return false;
        return base.CanUseSkill();
    }

    public override BeSkillManager.SkillCannotUseType GetCannotUseType()
    {
        if (curUseCount >= maxCount)
            return BeSkillManager.SkillCannotUseType.CAN_NOT_USE;
        return base.GetCannotUseType();
    }

    private void SetUseCount()
    {
#if !LOGIC_SERVER
        if (owner == null || !owner.isLocalActor)
            return;
        var battleUI = BattleUIHelper.CreateBattleUIComponent<BattleUIProfession>();;
        if (battleUI == null)
            return;
        battleUI.SetSkillUseCount(skillID, maxCount - curUseCount, _skillIconPath);
#endif
    }

    //移除复活时间监听
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
        if (switch3v3NextHandle != null)
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
}
#endregion

#region 光之复仇
public class Skill3703 : BeSkill
{
    public Skill3703(int sid, int skillLevel) : base(sid, skillLevel) { }

    private List<int> buffInfoListPve = new List<int>();        //Pve添加的buff信息列表
    private List<int> buffInfoListPvp = new List<int>();        //Pvp添加的buff信息列表

    protected string addBuffTag = null;       //添加Buff信息的标签

    public static void SkillPreloadRes(SkillTable tableData)
    {
#if !LOGIC_SERVER
        if (BattleMain.instance == null) return;

        if (BattleMain.IsModePvP(BattleMain.battleType))
        {
            for (int i = 0; i < tableData.ValueB.Count; i++)
            {
                PreloadManager.PreloadBuffInfoID(TableManager.GetValueFromUnionCell(tableData.ValueB[i], 1), null, null);
            }
        }
        else
        {
            for (int i = 0; i < tableData.ValueA.Count; i++)
            {
                PreloadManager.PreloadBuffInfoID(TableManager.GetValueFromUnionCell(tableData.ValueA[i], 1), null, null);
            }
        }
#endif
    }

    public override void OnInit()
    {
        base.OnInit();

        buffInfoListPve.Clear();
        buffInfoListPvp.Clear();

        for (int i = 0; i < skillData.ValueA.Count; i++)
        {
            buffInfoListPve.Add(TableManager.GetValueFromUnionCell(skillData.ValueA[i], level));
        }

        for (int i = 0; i < skillData.ValueB.Count; i++)
        {
            buffInfoListPvp.Add(TableManager.GetValueFromUnionCell(skillData.ValueB[i], level));
        }
        addBuffTag = "370301";
    }

    public override void OnStart()
    {
        handleA = owner.RegisterEventNew(BeEventType.onSkillCurFrame, (args) =>
        //handleA = owner.RegisterEvent(BeEventType.onSkillCurFrame, (object[] args) => 
        {
            string flag = args.m_String;
            if(flag == addBuffTag)
            {
                AddBuffInfo();
            }
        });
    }

    //添加Buff信息
    private void AddBuffInfo()
    {
        BeActor actor = owner;
        if (joystickSelectActor != null && !joystickSelectActor.IsDead())
            actor = joystickSelectActor;
        RealAddBuffInfo(actor);
    }

    private void RealAddBuffInfo(BeActor actor)
    {
        if (actor == null || actor.IsDead())
            return;
        List<int> buffInfoList = BattleMain.IsModePvP(battleType) ? buffInfoListPvp : buffInfoListPve;
        for (int i = 0; i < buffInfoList.Count; i++)
        {
            BuffInfoData data = new BuffInfoData(buffInfoList[i], level);
            actor.buffController.TryAddBuff(data, null, false, new VRate(), owner);
        }
    }
    #endregion
}
