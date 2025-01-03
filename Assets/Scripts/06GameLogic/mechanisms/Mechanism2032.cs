using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameClient;

public class Mechanism2032 : BeMechanism
{
    int monsterID = 70720011;
    BeActor backupPlayer = null;
    bool needRestoreDrug = false;
    bool buttonChanged = false;
    GameObject go = null;
    GameObject goParent = null;
    VInt3 offset = new VInt3();
    BeActor monster = null;
    private int xSpeed = 0;
    private int ySpeed = 0;
    private GeCameraControllerScroll cameraCtrl;
  //  private float camerStartOffset = 4.5f;
 //   private float camerYOffset = 0.7f;
 //   private float cameraEndOffset = -9f;
    private List<IBeEventHandle> handList = new List<IBeEventHandle>();
    private readonly Vector3 cameraPos = new Vector3(12f,5.2f,0.25f);
    private int[] skillIDs = null;
    public Mechanism2032(int mid, int lv) : base(mid, lv) { }

    public override void OnInit()
    {
        base.OnInit();
        xSpeed = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        ySpeed = TableManager.GetValueFromUnionCell(data.ValueB[0], level);

      //  camerStartOffset = TableManager.GetValueFromUnionCell(data.ValueC[0], level) / 1000.0f;
      //  cameraEndOffset = TableManager.GetValueFromUnionCell(data.ValueD[0], level) / 1000.0f;
     //   camerYOffset = TableManager.GetValueFromUnionCell(data.ValueE[0], level) / 1000.0f;
        monsterID = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
        skillIDs = new int[data.ValueD.Length];
        for (int i = 0; i < skillIDs.Length; i++)
        {
            skillIDs[i] = TableManager.GetValueFromUnionCell(data.ValueD[i], level);
        }
    }

    public override void OnReset()
    {
        monsterID = 70720011;
        backupPlayer = null;
        needRestoreDrug = false;
        buttonChanged = false;
        go = null;
        goParent = null;
        offset = VInt3.zero;
        monster = null;
        int xSpeed = 0;
        int ySpeed = 0;
        cameraCtrl = null;

        var iter = handList.GetEnumerator();
        while (iter.MoveNext())
        {
            iter.Current.Remove();
        }
        handList.Clear();

        skillIDs = null;
    }

    public override void OnStart()
    {
        base.OnStart();
        RestoreSkill2115();
        InitSystem(false);
#if !LOGIC_SERVER
        if(owner != null && owner.isLocalActor && owner.CurrentBeScene != null)
        {
            owner.CurrentBeScene.TriggerEventNew(BeEventSceneType.OnStartAirBattle);
        }
#endif
        if (owner.isSpecialMonster) return;
        MoveCamera();
        SetPosition();
        RegistSummonMonster();
        owner.DoSummon(monsterID);
    }

    private void RestoreSkill2115()
    {
        if (owner.isSpecialMonster)
        {
            owner.DoDead();
            owner.RegisterEventNew(BeEventType.onRemove, (args) =>
            {
                BeActor actor = owner.GetOwner() as BeActor;
                if (actor != null)
                {
                    actor.AddMechanism(1468);
                }
            });
        }
    }

    private void SetPosition()
    {
        VInt3[] poses = new VInt3[3];
        for (int i = 0; i < poses.Length; ++i)
        {
            poses[i] = owner.CurrentBeBattle.dungeonManager.GetDungeonDataManager().GetAirBattleBornPos(i);
        }

        owner.SetPosition(poses[GetSeat()], true);
    }

    private void InitSystem(bool flag)
    {
#if !LOGIC_SERVER
        var battleUI = BattleUIHelper.GetBattleUIComponent<BattleUISwitchWeaAndEquip>();
        if (owner.isLocalActor && battleUI!=null)
                battleUI.SetWeaponState(flag);
#endif
    }

    private int GetSeat()
    {
        List<BattlePlayer> list = owner.CurrentBeBattle.dungeonPlayerManager.GetAllPlayers();
        for (int i = 0; i < list.Count; i++)
        {
            if (owner.GetPID() == list[i].playerActor.GetPID())
            {
                return list[i].playerInfo.seat;
            }
        }
        return 0;
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
            monster.buffController.TryAddBuff((int)GlobalBuff.INVINCIBLE_NO_ALPHA, -1);
            monster.SetFace(false);
            monster.SetPosition(owner.GetPosition(), true);
            monster.actionManager.StopAll();
            owner.SetFace(false, true);
            owner.buffController.TryAddBuff((int)GlobalBuff.INVINCIBLE_NO_ALPHA, -1);
            owner.buffController.TryAddBuff(820004, -1, 1);//变身无敌 
            DoChangeToMonster(monster);
            var handle1 = OwnerRegisterEventNew(BeEventType.OnChangeSpeed, _OnChangeSpeed);
            //var handle1 = monster.RegisterEvent(BeEventType.OnChangeSpeed, (obj) =>
            //{

            //    VInt[] array = obj[0] as VInt[];

            //    if (array[1] < 0)
            //    {
            //        array[0] = xSpeed;
            //        array[1] = -ySpeed;
            //    }
            //    else if (array[1] > 0)
            //    {
            //        array[0] = -xSpeed;
            //        array[1] = ySpeed;
            //    }
            //    else
            //    {
            //        array[0] = 0;
            //    }


            //});
            handList.Add(handle1);

            var handle2 = monster.RegisterEventNew(BeEventType.onYInBlock, (obj) =>
            {
                monster.SetMoveSpeedX(0);
                monster.SetMoveSpeedY(0);
                obj.m_Bool = true;
            });
            handList.Add(handle2);
            var handle3 = monster.RegisterEventNew(BeEventType.onChangeFace, (obj) =>
            {
                monster.SetFace(false, true);
            });
            handList.Add(handle3);
        });

        //        handleB = owner.CurrentBeScene.RegisterEvent(BeEventSceneType.onExit, (args) => 
        //        {
        //#if !LOGIC_SERVER
        //            BeUtility.ResetCamera();           
        //#endif
        //        });
    }

    private void _OnChangeSpeed(BeEvent.BeEventParam param)
    {
        // marked by ckm
        // if (param.m_Vint2 < 0)
        // {
        //     param.m_Vint = xSpeed;
        //     param.m_Vint2 = -ySpeed;
        // }
        // else if (param.m_Vint2 > 0)
        // {
        //     param.m_Vint = -xSpeed;
        //     param.m_Vint2 = ySpeed;
        // }
        // else
        // {
        //     // param.m_Vint = 0;
        // }
    }

    private void MoveCamera()
    {
#if !LOGIC_SERVER
        if (owner != null && owner.isLocalActor)
        {
            BeUtility.ResetCamera();
            owner.CurrentBeScene.DelayCaller.DelayCall(300, () =>
            {
                cameraCtrl = owner.CurrentBeScene.currentGeScene.GetCamera().GetController();
                //cameraCtrl.MoveCamera(camerStartOffset, 0.1f);
                //Camera.main.transform.localPosition = new Vector3(0, camerYOffset, 0);
                cameraCtrl.SetPause(true);
                cameraCtrl.SetCameraPosition(cameraPos);
            });
        }
#endif
    }

    public void EndAirBattle(int delayTime)
    {
        owner.CurrentBeScene.DelayCaller.DelayCall(delayTime, () =>
        {
            DoRestoreFromMonster(monster);
            owner.stateController.SetAbilityEnable(BeAbilityType.CAN_DO_SKILL_CMD, false);
            VInt3 pos = owner.GetPosition();
            BeMoveTo moveTo = BeMoveTo.Create(owner, 500, owner.GetPosition(), (new VInt3(-7f, owner.GetPosition().fy, 3.0f) + pos) * 0.5f, true);
            owner.actionManager.RunAction(moveTo);
            owner.CurrentBeScene.DelayCaller.DelayCall(500, () =>
            {
                BeMoveTo moveTo1 = BeMoveTo.Create(owner, 500, owner.GetPosition(), new VInt3(-7f, owner.GetPosition().fy, 0f), true);
                owner.actionManager.RunAction(moveTo1);
            });
            owner.CurrentBeScene.DelayCaller.DelayCall(1000, () =>
            {
                owner.stateController.SetAbilityEnable(BeAbilityType.CAN_DO_SKILL_CMD, true);
            });
            BeUtility.ResetCamera();
#if !LOGIC_SERVER
            if (owner != null && owner.isLocalActor)
            {
                //  cameraCtrl.MoveCamera(cameraEndOffset, 0.5f);
                owner.m_pkGeActor.ChangeAction("Anim_Houtiao", 0.4f, false, true, true);
                owner.CurrentBeScene.TriggerEventNew(BeEventSceneType.OnEndAirBattle);
                InitSystem(true);
            }
#endif

        });
    }



    void DoChangeToMonster(BeActor monster)
    {
        if(owner.CurrentBeBattle != null)
        {
            //Fix:漫游切门后接枪，切了技能动作。导致上炮台都动作一直卡在技能动作上
            if (owner.sgGetCurrentState() == (int) ActionState.AS_CASTSKILL)
            {
                BeUtility.CancelCurrentSkill(owner);
            }
        }
        SwitchToMonster(monster);

#if !LOGIC_SERVER
        go = owner.m_pkGeActor.GetEntityNode(GeEntity.GeEntityNodeType.Root);
        var parent = monster.m_pkGeActor.GetEntityNode(GeEntity.GeEntityNodeType.Root);

        goParent = go.transform.parent.gameObject;

        if (go != null && goParent != null)
        {
            Battle.GeUtility.AttachTo(go, parent);
        }
        go.transform.localPosition = new Vector3(-0.4f, 1.36f, 0);
#endif
        owner.ResetMoveCmd();
        owner.CurrentBeScene.DelayCaller.DelayCall(0, () => {
            owner.buffController.RemoveBuff(820004);//从BeScene里删除时移除变身无敌buff
        owner.CurrentBeScene.RemoveToTemp(owner);
        });

        if (owner.isLocalActor)
        {
            monster.actorData.isSpecialMonster = true;
        }

    }

    public void SwitchToMonster(BeActor monster)
    {

        backupPlayer = owner;

        var battlePlayer = GetBattlePlayer(owner);

        if (battlePlayer != null)
        {
            monster.isMainActor = owner.isMainActor;
            monster.isLocalActor = owner.isLocalActor;
            monster.pauseAI = owner.pauseAI;
            monster.aiManager.isAutoFight = owner.aiManager.isAutoFight;

            var slotMap = new Dictionary<int, int>();
            slotMap.Add(1, skillIDs[0]);
            slotMap.Add(2, -1);
            slotMap.Add(3, -1);
            slotMap.Add(4, skillIDs[1]);

            monster.skillController.skillSlotMap = slotMap;

            battlePlayer.playerActor = monster;


#if !LOGIC_SERVER
            monster.m_pkGeActor.SetActorVisible(true);
            monster.m_pkGeActor.SetHPBarVisible(false);
            if (monster.m_pkGeActor.goInfoBar != null)
                monster.m_pkGeActor.goInfoBar.SetActive(false);
            ChangeButtons(monster);
#endif
            backupPlayer.SetAttackButtonState(GameClient.ButtonState.RELEASE);
            monster.SetAttackButtonState(GameClient.ButtonState.RELEASE);
#if !LOGIC_SERVER
            if (owner.pet != null)
                owner.pet.m_pkGeActor.SetActorVisible(false);
#endif
            handleB = monster.RegisterEventNew(BeEventType.RaidBattleChangeMonster, RegisterDoublePress);
        }
    }

    /// <summary>
    /// 监听双击跑帧配置数据
    /// </summary>
    private void RegisterDoublePress(BeEvent.BeEventParam args)
    {
        args.m_Obj = backupPlayer;
    }


    void ChangeButtons(BeActor monster)
    {
#if !LOGIC_SERVER
        if (!backupPlayer.isLocalActor)
            return;

        if (buttonChanged)
            return;

        InputManager.instance.ReloadButtons(backupPlayer.GetEntityData().professtion, monster, monster.skillController.skillSlotMap, true);

        // owner.CurrentBeBattle.dungeonManager.GetGeScene().AttachCameraTo(monster.m_pkGeActor);

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
        SwitchBack(monster);
        // owner.SetPosition(monster.GetPosition(), true);
        owner.SetFace(true);
        owner.actionManager.StopAll();

        monster.spirit = null;

        owner.CurrentBeScene.RestoreFromTemp(owner);


#if !LOGIC_SERVER
        if (go != null && goParent != null)
        {
            Battle.GeUtility.AttachTo(go, goParent);
        }
#endif
        RestoreOwner();
        owner.pauseAI = monster.pauseAI;
        owner.aiManager.isAutoFight = monster.aiManager.isAutoFight;

#if !LOGIC_SERVER
        RestoreButtons();

        if (owner.pet != null)
            owner.pet.m_pkGeActor.SetActorVisible(true);
#endif

    }

    public void SwitchBack(BeActor monster)
    {
        var battlePlayer = GetBattlePlayer(monster);
        if (battlePlayer != null)
        {
            monster.isMainActor = false;
            monster.isLocalActor = false;

            var actor = battlePlayer.playerActor;

            //var backPos = actor.GetPosition();
            //backPos.z = 0;

            // backupPlayer.SetPosition(backPos);
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
    }

    void RestoreButtons()
    {
#if !LOGIC_SERVER
        if (!backupPlayer.isLocalActor)
            return;

        if (!buttonChanged)
            return;

        buttonChanged = false;

        InputManager.instance.ReloadButtons(backupPlayer.GetEntityData().professtion, backupPlayer, null);

        //     backupPlayer.CurrentBeBattle.dungeonManager.GetGeScene().AttachCameraTo(backupPlayer.m_pkGeActor);


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
        var iter = handList.GetEnumerator();
        while (iter.MoveNext())
        {
            iter.Current.Remove();
        }
        handList.Clear();
    }
}
