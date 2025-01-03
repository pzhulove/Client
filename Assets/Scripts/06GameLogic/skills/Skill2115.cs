using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameClient;

public class Skill2115 : BeSkill {
	int monsterID = 9080031;
	int secondSkillID = 2011;
	int monsterSkillID = 5323;

    GameObject go = null;
    GameObject goParent = null;

    GeEffectEx effect = null;
    GameObject uieffect = null;
    bool needRestoreDrug = false;
    bool buttonChanged = false;

    BeActor backupPlayer = null;

    protected int m_BoomEntityId = 60128;   //卡西利亚斯死亡的时候创建的实体ID

    protected enum State
    {
        SUMMON,
        TRANSFORM,
        FINISH
    }

	IBeEventHandle handler = null;

    protected State state = State.SUMMON;

	public Skill2115(int sid, int skillLevel):base(sid, skillLevel)
	{

	}

	public override void OnInit ()
	{
		monsterID = TableManager.GetValueFromUnionCell(skillData.ValueA[0], level);
		secondSkillID = TableManager.GetValueFromUnionCell(skillData.ValueB[0], level);
		monsterSkillID = TableManager.GetValueFromUnionCell(skillData.ValueC[0], level);
	}


    public void Prepare()
    {
        state = State.SUMMON;
        buttonChanged = false;
        go = null;
        goParent = null;
        effect = null;

        RemoveUIEffect();
        RemoveHandler();
    }
	public override void OnStart ()
	{
        Prepare();

        if (owner.IsDead())
            return;

        handler = owner.RegisterEventNew(BeEventType.onSummon, (args) => {

            if (owner.IsDead())
                return;

            BeActor monster = args.m_Obj as BeActor;
            if (monster == null)
                return;
            if (!monster.GetEntityData().MonsterIDEqual(monsterID) || monster.IsDead())
                return;
            AddMechanism(monster);
            //owner.TriggerEvent(BeEventType.onMagicGirlMonsterChange, new object[] { monster, owner });
            owner.TriggerEventNew(BeEventType.onMagicGirlMonsterChange, new EventParam{m_Obj = monster, m_Obj2 = owner});
            ChangeSummonerFollower(monster);
            monster.buffController.RemoveBuff(2);
            monster.buffController.TryAddBuff((int)GlobalBuff.INVINCIBLE_NO_ALPHA, GlobalLogic.VALUE_1500);
            monster.SetFace(owner.GetFace());
            monster.m_pkGeActor.SetHeadInfoVisible(false);
            monster.m_pkGeActor.SetFootIndicatorVisible(false);
            monster.SetRestrainPosition(false);

            var ownerPos = owner.GetPosition();
            var ownerOriginPos = ownerPos;
            ownerPos.x += owner._getFaceCoff() * VInt.one.i;
            bool faceDir = owner.GetFace();

            //隐藏魔法护盾
            SetMoFaDunBuff(owner);

            //                 monster.delayCaller.DelayCall(3000, ()=>{
            //                     owner.DoHurt(owner.GetEntityData().GetHP() + 1);
            //                 });
            //让怪物先走一秒
            monster.delayCaller.DelayCall(1000, ()=>{

                monster.actionManager.StopAll();

                owner.buffController.TryAddBuff((int)GlobalBuff.INVINCIBLE_NO_ALPHA, -1);

                monster.m_pkGeActor.SetHeadInfoVisible(true);
                monster.m_pkGeActor.SetFootIndicatorVisible(true);
                monster.SetRestrainPosition(true);

                var lifeBuff = monster.buffController.HasBuffByID(12) as Buff12;
                if (lifeBuff != null)
                    lifeBuff.showDisappearEffect = false;

                if (monster.CurrentBeScene.IsInBlockPlayer(monster.GetPosition()))
                {
                    var pos = BeAIManager.FindStandPositionNew(ownerOriginPos, monster.CurrentBeScene, faceDir);
                    monster.SetPosition(pos);
                }

                DoChangeToMonster(monster);
            });

            //怪物死亡或者时间到的时候恢复
            monster.RegisterEventNew(BeEventType.onDead, eventParam =>
            {
                DoRestoreFromMonster(monster);
                CreateBoomEntityId(owner, monster);
            });

        });

	}

    /// <summary>
    /// 黑色大地怨念值机制
    /// </summary>
    private void AddMechanism(BeActor monster)
    {
        Mechanism2004 backupMechanism = owner.GetMechanism(5300) as Mechanism2004;
        if (backupMechanism != null)
        {
            Mechanism2004 mechanism = monster.AddMechanism(5300) as Mechanism2004;
            if (mechanism != null)
            {
                mechanism.CopyMechanims(backupMechanism.GetResentmentValue(), backupMechanism.IsBetray());
            }
        }
    }

    private void RestoreMechanism(BeActor monster)
    {
        Mechanism2004 backupMechanism = owner.GetMechanism(5300) as Mechanism2004;
        if (backupMechanism != null)
        {
            Mechanism2004 mechanism = monster.GetMechanism(5300) as Mechanism2004;
            if (mechanism != null)
            {
                mechanism.CopyMechanims(backupMechanism.GetResentmentValue(), backupMechanism.IsBetray());
                mechanism.OnChangeResentmentValue(backupMechanism.GetResentmentValue());
            }
        }
    }


    void DoChangeToMonster(BeActor monster)
    {
        if (state != State.SUMMON)
            return;

        if (owner.IsDead())
            return;

       // Logger.LogErrorFormat("before SwitchToMonster!!!!");
        SwitchToMonster(monster);

        //Logger.LogErrorFormat("after SwitchToMonster!!!!");

        var pos = owner.GetPosition();
        pos.y -= VInt.Float2VIntValue(0.5f);
        owner.SetPosition(pos, true);

#if !LOGIC_SERVER

        //变成卡西利亚斯以后强制清除失明效果
        if (owner.isLocalActor && owner.CurrentBeScene != null)
        {
            owner.CurrentBeScene.StopBlindMask();
        }

        //特殊表现
        owner.m_pkGeActor.ChangeAction("Anim_Zhaohuan_juexing_Idle", 1.0f, true);
        effect = owner.m_pkGeActor.CreateEffect(1015, Vec3.zero);

        AddUIEffect();

        go = owner.m_pkGeActor.GetEntityNode(GeEntity.GeEntityNodeType.Root);
        var parent = monster.m_pkGeActor.GetEntityNode(GeEntity.GeEntityNodeType.Root);

        goParent = go.transform.parent.gameObject;

        if (go != null && goParent != null)
        {
            Battle.GeUtility.AttachTo(go, parent);
        }

        monster.spirit = owner.m_pkGeActor;
#endif

        monster.RegisterEventNew(BeEventType.onChangeFace, (args2) =>
        {

            SetSpiritPos(monster.GetFace(), go);
        });

        SetSpiritPos(monster.GetFace(), go);

        //owner.CurrentBeScene.GetEntities().Remove(owner);
        owner.CurrentBeScene.RemoveToTemp(owner);

        if (owner.isLocalActor)
        {
            monster.actorData.isSpecialMonster = true;
        }

    //    Logger.LogErrorFormat("333333333333!!!!");
    }

    void DoRestoreFromMonster(BeActor monster)
    {
        if (state == State.TRANSFORM)
        {
            state = State.FINISH;

            //Logger.LogErrorFormat("44444444444444444!!!!");

            monster.m_pkGeActor.SetActorVisible(false);

            SwitchBack(monster);

            //Logger.LogErrorFormat("555555555555555555!!!!");

            owner.actionManager.StopAll();

            monster.spirit = null;

            //             var allEntites = owner.CurrentBeScene.GetEntities();
            //             if (!allEntites.Contains(owner))
            //                 allEntites.Add(owner);

            owner.CurrentBeScene.RestoreFromTemp(owner);
            RestoreMechanism(monster);
            //monster.TriggerEvent(BeEventType.onMagicGirlMonsterRestore, new object[] { owner,monster });
            monster.TriggerEventNew(BeEventType.onMagicGirlMonsterRestore, new EventParam{m_Obj = owner, m_Obj2 = monster});
            ChangeSummonerFollower(owner);
#if !LOGIC_SERVER
            if (go != null && goParent != null)
            {
                Battle.GeUtility.AttachTo(go, goParent);
            }
            if (effect != null)
            {
                owner.m_pkGeActor.DestroyEffect(effect);
                effect = null;
            }

            RemoveUIEffect();

            //从卡西利亚斯变成召唤兽以后强制清除失明效果
            if (owner.isLocalActor && owner.CurrentBeScene != null)
            {
                owner.CurrentBeScene.StopBlindMask();
            }
#endif
            owner.GetStateGraph().SetCurrentStateTag((int)AStateTag.AST_BUSY | (int)AStateTag.AST_CONTROLED);
            owner.GetStateGraph().ResetCurrentStateTime();
            int time = owner.PlayAction("Qyzh_Kaxiliyasi_07");
            if (monster.GetMechanism(1468) != null)
            {
                time = 0;
            }
            owner.GetStateGraph().SetCurrentStatesTimeout(time);

           // Logger.LogErrorFormat("66666666666666666666!!!!");

            owner.delayCaller.DelayCall(time, () =>
            {
               // Logger.LogErrorFormat("777777777777777!!!! time:{0}", time);


                RestoreOwner();
                if (owner.CurrentBeScene.IsInBlockPlayer(owner.GetPosition()))
                {
                    var pos = BeAIManager.FindStandPositionNew(owner.GetPosition(), owner.CurrentBeScene, owner.GetFace(), false, 30);

                    //还是找不到
                    if (pos == owner.GetPosition() && !owner.CurrentBeScene.IsInBlockPlayer(monster.savedPosition))
                    {
                        monster.SetPosition(monster.savedPosition);
                    }
                    else
                    {
                        pos = BeAIManager.FindStandPositionNew(monster.savedPosition, owner.CurrentBeScene, owner.GetFace(), false, 30);
                        owner.SetPosition(pos);
                    }
                }

            });

            owner.pauseAI = monster.pauseAI;
            owner.aiManager.isAutoFight = monster.aiManager.isAutoFight;
        }
        else
        {
            //如果没召唤出来就死了
            if (state == State.SUMMON)
                RestoreOwner();
        }
    }


    void AddUIEffect()
    {
        if (owner.isLocalActor)
        {
            RemoveUIEffect();
            uieffect = AssetLoader.instance.LoadResAsGameObject("Effects/UI/Prefab/EffUI_Zhaohuanshi_juexing/Prefab/EffUI_Zhaohuanshi_juexing_pinmu");
            if (uieffect != null)
                Utility.AttachTo(uieffect, GameClient.ClientSystemManager.instance.GetLayer(GameClient.FrameLayer.TopMost));
        }
    }

    void RemoveUIEffect()
    {
        if (owner.isLocalActor && uieffect != null)
        {
            GameObject.Destroy(uieffect);
            uieffect = null;
        }
    }

	void SetSpiritPos(bool faceLeft, GameObject target)
	{
#if !LOGIC_SERVER
        if (target == null)
			return;

		var offset = faceLeft?0.4f:-0.4f;

		target.transform.localPosition = new Vector3(offset, 1.59f, -0.2f);
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

    
    public void SwitchToMonster(BeActor monster)
    {
        if (state == State.TRANSFORM)
            return;

        backupPlayer = owner;

        var battlePlayer = GetBattlePlayer(owner);

        if (battlePlayer != null)
        {
            monster.isMainActor = owner.isMainActor;
            monster.isLocalActor = owner.isLocalActor;
            monster.pauseAI = owner.pauseAI;
            monster.aiManager.isAutoFight = owner.aiManager.isAutoFight;
            //    monster.aiManager.aiType = BeAIManager.AIType.NOATTACK;
            monster.isSpecialMonster = true;
            var slotMap = new Dictionary<int, int>();
            slotMap.Add(1, 5321);
            slotMap.Add(2, -1);
            slotMap.Add(3, -1);
            slotMap.Add(4, 5322);
            slotMap.Add(5, 5323);
            slotMap.Add(6, 5324);

            monster.skillController.skillSlotMap = slotMap;

            var backPos = backupPlayer.GetPosition();
            backPos.z = 0;

            battlePlayer.playerActor = monster;

            backupPlayer.SetRestrainPosition(false);
            monster.m_pkGeActor.SetActorVisible(true);

#if !LOGIC_SERVER
            backupPlayer.m_pkGeActor.SetFootIndicatorVisible(false);
            backupPlayer.m_pkGeActor.SetUIVisible(false);

            ChangeButtons(monster);
#endif
            backupPlayer.SetAttackButtonState(GameClient.ButtonState.RELEASE);
            monster.SetAttackButtonState(GameClient.ButtonState.RELEASE);
            monster.sgSwitchStates(new BeStateData((int)ActionState.AS_IDLE));
            monster.SetRestrainPosition(true);

#if !LOGIC_SERVER
            if (owner.pet != null)
            {
                if (owner.pet.m_pkGeActor != null)
                {
                    owner.pet.m_pkGeActor.SetActorVisible(false);
                }
                else
                {
                    Logger.LogErrorFormat("SwitchToMonster  pet.m_pkGeActor is null ,pet ResID:{0},MonsterID:{1}", owner.pet.m_iResID, owner.pet.GetEntityData().monsterID);
                }
            }
#endif
        }

        state = State.TRANSFORM;     
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

            backupPlayer.SetRestrainPosition(true);
            backupPlayer.SetPosition(backPos);
            backupPlayer.m_pkGeActor.SetActorVisible(true);
            backupPlayer.Reset();

#if !LOGIC_SERVER

            if (backupPlayer.m_pkGeActor != null)
            {
                backupPlayer.m_pkGeActor.SetFootIndicatorVisible(true);
                backupPlayer.m_pkGeActor.SetUIVisible(true);

                RestoreButtons();

                if (backupPlayer.pet != null)
                {
                    if (backupPlayer.pet.m_pkGeActor != null)
                    {
                        backupPlayer.pet.m_pkGeActor.SetActorVisible(true);
                    }
                    else
                    {
                        Logger.LogErrorFormat("SwitchBack  pet.m_pkGeActor is null ,pet ResID:{0},MonsterID:{1}", backupPlayer.pet.m_iResID, backupPlayer.pet.GetEntityData().monsterID);
                    }
                }
            }
            else
            {
                Logger.LogErrorFormat("SwitchBack,backupPlayer.m_pkGeActor is null");
            }
#endif
            //按钮状态需要延时一帧重置，因为原来召唤师的技能按钮出现会延迟一帧，如果这边状态重置以后再按住普攻按钮不放，会导致召唤师一直在释放普攻
            if(owner.IsDead())
            {
                ResetAttackBtnState(backupPlayer, monster);
            }
            else
            {
                owner.delayCaller.DelayCall(1000, () =>
                {
                    ResetAttackBtnState(backupPlayer, monster);
                });
            }
            
            backupPlayer.sgSwitchStates(new BeStateData((int)ActionState.AS_IDLE));
            battlePlayer.playerActor = backupPlayer;
        }

        state = State.FINISH;
    }

    /// <summary>
    /// 重置普攻按钮状态
    /// </summary>
    protected void ResetAttackBtnState(BeActor backUp, BeActor current)
    {
        if (backUp != null)
        {
            backUp.SetAttackButtonState(GameClient.ButtonState.RELEASE);
        }

        if (current != null)
        {
            current.SetAttackButtonState(GameClient.ButtonState.RELEASE);
        }
    }


    void ChangeButtons(BeActor monster)
    {
#if !LOGIC_SERVER
        if (!backupPlayer.isLocalActor)
            return;

        if (buttonChanged)
            return;

        InputManager.instance.ReloadButtons(backupPlayer.GetEntityData().professtion, monster, monster.skillController.skillSlotMap, true);

        owner.CurrentBeBattle.dungeonManager.GetGeScene().AttachCameraTo(monster.m_pkGeActor);

        var battleUI = BattleUIHelper.GetBattleUIComponent<BattleUIDrug>();
        if (battleUI != null)
        {
            if (battleUI.IsDrugVisible())
            {
                battleUI.SetDrugVisible(false);
                needRestoreDrug = true;
            }

        }

        backupPlayer.m_pkGeActor.isSyncHPMP = false;

        buttonChanged = true;
#endif
    }

    void RestoreButtons()
    {
#if !LOGIC_SERVER
        owner.delayCaller.DelayCall(1000, () =>
        {
            SetMoFaDunBuff(backupPlayer, false);
        });

        if (!backupPlayer.isLocalActor)
            return;

        if (!buttonChanged)
            return;

        buttonChanged = false;

        if (owner.IsDead())
            InputManager.instance.ReloadButtons(backupPlayer.GetEntityData().professtion, backupPlayer, null);
        else
        {
            owner.delayCaller.DelayCall(1000, () =>
            {
                InputManager.instance.ReloadButtons(backupPlayer.GetEntityData().professtion, backupPlayer, null);
                ShowMofadunBtnEffect(backupPlayer);
            });
        }

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

    void RestoreOwner()
    {
        owner.buffController.RemoveBuff((int)GlobalBuff.INVINCIBLE_NO_ALPHA);
        owner.GetStateGraph().ResetStateTag((int)ActionState.AS_IDLE);
        owner.GetStateGraph().ResetCurrentStateTag();
        owner.sgSwitchStates(new BeStateData((int)ActionState.AS_IDLE));

        owner.SetScale(VInt.one);

#if !LOGIC_SERVER
        RestoreButtons();
#endif
    }


    void Restore()
	{
        RemoveUIEffect();
		RemoveHandler();
	}

	void RemoveHandler()
	{
		if (handler != null)
		{
			handler.Remove();
			handler = null;
		}
	}

	public override void OnCancel ()
	{
		Restore();
	}

    /// <summary>
    /// 改变召唤兽的跟随目标
    /// </summary>
    private void ChangeSummonerFollower(BeActor target)
    {
        List<BeActor> list = GamePool.ListPool<BeActor>.Get();
        owner.CurrentBeScene.GetSummonBySummoner(list, owner);
        for (int i=0;i< list.Count; i++)
        {
            var actor = list[i];
            if(actor!=null && !actor.IsDead() && actor.aiManager!=null)
            {
                actor.aiManager.followTarget = target;
            }
        }
        GamePool.ListPool<BeActor>.Release(list);
    }
    
    /// <summary>
    /// 觉醒变身的时候魔法盾Buff暂时隐藏
    /// </summary>
    private const int m_MoFaDunBuffId = 211301;
    private const int m_MoFaDunSkillId = 2113;
    private void SetMoFaDunBuff(BeActor actor, bool isHide = true)
    {
#if !LOGIC_SERVER
        if (actor == null)
            return;
        BeBuff buff = actor.buffController.HasBuffByID(m_MoFaDunBuffId);
        if (buff == null)
            return;
        GeEffectEx buffEffect = buff.GetEffectEx();
        if (buffEffect == null)
            return;
        buffEffect.SetVisible(!isHide);
#endif
    }

    /// <summary>
    /// 重新显示魔法盾技能按钮特效
    /// </summary>
    private void ShowMofadunBtnEffect(BeActor actor)
    {
#if !LOGIC_SERVER
		if(actor==null)
		return;
        BeSkill skill = actor.GetSkill(m_MoFaDunSkillId);
        BeBuff buff = actor.buffController.HasBuffByID(m_MoFaDunBuffId);
        if (skill != null && buff != null)
        {
            if (skill.button != null)
            {
                skill.button.AddEffect(ETCButton.eEffectType.onContinue, true);
            }
        }
#endif
    }

    /// <summary>
    /// 创建实体Id
    /// </summary>
    private void CreateBoomEntityId(BeActor actor,BeActor monster)
    {
        if (actor == null)
            return;
        if (monster == null)
            return;
        VInt3 pos = monster.GetPosition();
        pos.z += GlobalLogic.VALUE_10000;
        actor.AddEntity(m_BoomEntityId, pos, level);
    }
}
