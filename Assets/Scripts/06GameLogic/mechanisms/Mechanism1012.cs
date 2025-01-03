using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameClient;

public class Mechanism1012 : BeMechanism
{
    private List<int> skillIDList = new List<int>();

    int monsterID = 9080031;
    BeActor backupPlayer = null;
    bool needRestoreDrug = false;
    bool buttonChanged = false;
    GameObject go = null;
    GameObject goParent = null;
    BeActor monster = null;
    bool isTimeUp = false;
    int backUpAttackID = 0;
    public Mechanism1012(int mid, int lv) : base(mid, lv) { }

    public override void OnInit()
    {
        base.OnInit();
        skillIDList.Clear();
        int skillCount = data.ValueA.Count;
        for (int i = 0; i < skillCount; ++i)
        {
            var skillID = TableManager.GetValueFromUnionCell(data.ValueB[i], 1);
            if (skillID > 0)
                skillIDList.Add(skillID);
        }

        monsterID = TableManager.GetValueFromUnionCell(data.ValueA[0], level);

    }

    public override void OnReset()
    {
        backupPlayer = null;
        needRestoreDrug = false;
        buttonChanged = false;
        go = null;
        goParent = null;
        isTimeUp = false;
        backUpAttackID = 0;
    }

    public override void OnStart()
    {
        base.OnStart();
        RegistSummonMonster();
        owner.DoSummon(monsterID);
    }

    private void RegistSummonMonster()
    {
        handleA = owner.RegisterEventNew(BeEventType.onSummon, (args) =>
        {
            BeActor monster = args.m_Obj as BeActor;
            if (monster == null)
                return;
            if (!monster.GetEntityData().MonsterIDEqual(monsterID) || monster.IsDead())
                return;
            this.monster = monster;
            monster.stateController.SetAbilityEnable(BeAbilityType.CANATTACKFRIEND, false);
            owner.CancelSkill(owner.GetCurSkillID());
            monster.SetFace(owner.GetFace());
            monster.m_pkGeActor.SetHeadInfoVisible(false);
            monster.m_pkGeActor.SetFootIndicatorVisible(false);
            monster.SetPosition(owner.GetPosition(), true);
            monster.actionManager.StopAll();
            owner.buffController.TryAddBuff((int)GlobalBuff.INVINCIBLE_NO_ALPHA, -1);
            owner.buffController.TryAddBuff(820004, -1, 1);//变身无敌 
            DoChangeToMonster(monster);

            var thisAttachBuff = GetAttachBuff();

            if (thisAttachBuff != null)
            {
                monster.CurrentBeScene.DelayCaller.DelayCall(thisAttachBuff.duration, () =>
                {
                    //owner.buffController.RemoveBuff(attachBuff);
                    RemoveAttachBuff();
                });
            }
            monster.RegisterEventNew(BeEventType.onDead, eventParam =>
            {
                if (!isTimeUp)
                {
                    //owner.buffController.RemoveBuff(attachBuff);
                    RemoveAttachBuff();
                    owner.DoDead();
                }

                monster.m_pkGeActor.SetActorVisible(false);
                owner.SetPosition(monster.GetPosition(), true);
                owner.SetFace(true);
                owner.actionManager.StopAll();

                monster.spirit = null;

                owner.CurrentBeScene.RestoreFromTemp(owner);
#if !LOGIC_SERVER
                if (go != null)
                    go.CustomActive(true);
                RestoreButtons();

                if (owner.pet != null && owner.pet.m_pkGeActor != null)
                    owner.pet.m_pkGeActor.SetActorVisible(true);
#endif
                RestoreOwner();
                owner.pauseAI = monster.pauseAI;
                if (monster.aiManager != null && owner.aiManager != null)
                    owner.aiManager.isAutoFight = monster.aiManager.isAutoFight;

            });

            monster.RegisterEventNew(BeEventType.onBeHitAfterFinalDamage, obj =>
            //monster.RegisterEvent(BeEventType.onBeHitAfterFinalDamage, (obj) =>
            {
                //int[] damage = obj[0] as int[];
                if (backupPlayer != null)
                {
                    backupPlayer.DoHPChange(-obj.m_Int, false);                   
                }
            });

        });
    }

    void DoChangeToMonster(BeActor monster)
    {

        SwitchToMonster(monster);

#if !LOGIC_SERVER

        go = owner.m_pkGeActor.GetEntityNode(GeEntity.GeEntityNodeType.Root);
        go.CustomActive(false);

#endif
        owner.CurrentBeScene.DelayCaller.DelayCall(0, () => {
            owner.buffController.RemoveBuff(820004);//从BeScene里删除时移除变身无敌buff
        owner.CurrentBeScene.RemoveToTemp(owner);
        });

        monster.isSpecialMonster = true;
    }

    public void SwitchToMonster(BeActor monster)
    {

        backupPlayer = owner;

        var battlePlayer = GetBattlePlayer(owner);

        if (battlePlayer != null)
        {
            if (monster.attribute != null && backupPlayer.attribute != null)
            {
                monster.attribute.SetMaxHP(backupPlayer.attribute.GetMaxHP());
                monster.attribute.SetHP(backupPlayer.attribute.GetHP());
                backUpAttackID = backupPlayer.attribute.normalAttackID;
                monster.attribute.normalAttackID = skillIDList[0];
            }
            monster.isMainActor = owner.isMainActor;
            monster.isLocalActor = owner.isLocalActor;
            monster.pauseAI = owner.pauseAI;
            if(monster.aiManager!=null&&owner.aiManager!=null)
               monster.aiManager.isAutoFight = owner.aiManager.isAutoFight;

            var slotMap = new Dictionary<int, int>();
            slotMap.Add(1, skillIDList[0]);
            slotMap.Add(2, -1);
            slotMap.Add(3, -1);

            monster.skillController.skillSlotMap = slotMap;

            battlePlayer.playerActor = monster;
            if(monster.m_pkGeActor!=null)
               monster.m_pkGeActor.SetActorVisible(true);
            backupPlayer.SetAttackButtonState(GameClient.ButtonState.RELEASE);
            monster.SetAttackButtonState(GameClient.ButtonState.RELEASE);
            monster.sgSwitchStates(new BeStateData((int)ActionState.AS_IDLE));
#if !LOGIC_SERVER
            ChangeButtons(monster);
#endif

#if !LOGIC_SERVER
            if (owner.pet != null && owner.pet.m_pkGeActor!=null)
                owner.pet.m_pkGeActor.SetActorVisible(false);
#endif
        }
    }

    void ChangeButtons(BeActor monster)
    {
#if !LOGIC_SERVER
        if (!backupPlayer.isLocalActor)
            return;

        if (buttonChanged)
            return;

        InputManager.instance.SetButtonStateActive(0);

        owner.CurrentBeBattle.dungeonManager.GetGeScene().AttachCameraTo(monster.m_pkGeActor);

        var battleUI = BattleUIHelper.GetBattleUIComponent<BattleUIDrug>();
        if (battleUI != null && battleUI.IsDrugVisible())
        {
            battleUI.SetDrugVisible(false);
            needRestoreDrug = true;
        }

        backupPlayer.m_pkGeActor.isSyncHPMP = false;

        buttonChanged = true;
#endif
    }

    BattlePlayer GetBattlePlayer(BeActor actor)
    {
        BattlePlayer battlePlayer = null;

        var players = owner.CurrentBeBattle.dungeonPlayerManager.GetAllPlayers();
        for (int i = 0; i < players.Count; ++i)
        {
            if (players[i].playerActor == actor)
            {
                battlePlayer = players[i];
                break;
            }
        }

        return battlePlayer;
    }

    void DoRestoreFromMonster(BeActor monster)
    {
        isTimeUp = true;
        SwitchBack(monster);
        monster.DoDead();      
    }

    public void SwitchBack(BeActor monster)
    {
        var battlePlayer = GetBattlePlayer(monster);
        if (battlePlayer != null)
        {
            monster.isMainActor = false;
            monster.isLocalActor = false;

            var actor = battlePlayer.playerActor;

            var backPos = actor.GetPosition();
            backPos.z = 0;
            if(backupPlayer.attribute!=null)
              backupPlayer.attribute.normalAttackID = backUpAttackID;
            backupPlayer.SetPosition(backPos);
            backupPlayer.m_pkGeActor.SetActorVisible(true);
            backupPlayer.Reset();
            backupPlayer.SetAttackButtonState(GameClient.ButtonState.RELEASE);
            monster.SetAttackButtonState(GameClient.ButtonState.RELEASE);
            backupPlayer.sgSwitchStates(new BeStateData((int)ActionState.AS_IDLE));
            battlePlayer.playerActor = backupPlayer;
        }
    }

    void RestoreOwner()
    {
        owner.buffController.RemoveBuff((int)GlobalBuff.INVINCIBLE_NO_ALPHA);
        owner.GetStateGraph().ResetStateTag((int)ActionState.AS_IDLE);
        owner.GetStateGraph().ResetCurrentStateTag();
        owner.sgSwitchStates(new BeStateData((int)ActionState.AS_IDLE));

        owner.SetScale(VInt.one);
    }

    void RestoreButtons()
    {
#if !LOGIC_SERVER
        if (!backupPlayer.isLocalActor)
            return;

        if (!buttonChanged)
            return;

        buttonChanged = false;

        InputManager.instance.ResetButtonState();

        backupPlayer.CurrentBeBattle.dungeonManager.GetGeScene().AttachCameraTo(backupPlayer.m_pkGeActor);


        if (needRestoreDrug)
        {
            var battleUI = BattleUIHelper.GetBattleUIComponent<BattleUIDrug>();
            if (battleUI != null)
            {
                battleUI.SetDrugVisible(true);
                needRestoreDrug = false;
            }
        }
        backupPlayer.m_pkGeActor.isSyncHPMP = true;
#endif
    }


    public override void OnFinish()
    {
        base.OnFinish();
        DoRestoreFromMonster(monster);
    }
}
