using UnityEngine;
using System.Collections.Generic;
using GameClient;

/*
reset这个先不改，应该没用到了！！！
 * 双子BOSS合放技能机制 
*/
public class Mechanism60 : BeMechanism
{
    List<IBeEventHandle> handlerList = new List<IBeEventHandle>();
    BeActor boss1;
    BeActor boss2;
    BeActor protector1;
    BeActor protector2;
    BeProjectile fireBall;
    IBeEventHandle handler;

    int boss1ID = 8280031;
    int boss2ID = 8290031;
    int protectorID = 3140011;
    int skill1ID = 5647;
    int skill2ID = 5648;
    int fireBallID = 60317;

    int skillCD;                            //技能CD
    int startHeight;                        //火球初始高度
    int duration;                           //火球持续时间
    int fallSpeed;                          //火球下落速度
    int baseDamageValue;                    //基础伤害值
    int[] damageCoefficient = new int[4];   //伤害系数
    int attackID;                           //触发效果ID

    int timeAcc = 0;
    int damageAcc = 0;
    int damageValue;                        //最终伤害值
    bool canUseSkill = false;

#if !LOGIC_SERVER
    string tipsContent = "";
    BattleUIDungeonDamageBar battleUI;
    int protectHurtTime = 0;
#endif

    public Mechanism60(int mid, int lv) : base(mid, lv) { }
    
    public override void OnInit()
    {
        skillCD = TableManager.GetValueFromUnionCell(data.ValueA[0], level);
        startHeight = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        duration = TableManager.GetValueFromUnionCell(data.ValueC[0], level);
        fallSpeed = TableManager.GetValueFromUnionCell(data.ValueD[0], level);
        baseDamageValue = TableManager.GetValueFromUnionCell(data.ValueE[0], level);
        for (int i = 0; i < data.ValueF.Count; i++)
        {
            damageCoefficient[i] = TableManager.GetValueFromUnionCell(data.ValueF[i], level);
        }
        attackID = TableManager.GetValueFromUnionCell(data.ValueG[0], level);
    }

    public override void OnStart()
    {
        List<BeActor> monsters = new List<BeActor>();
        owner.CurrentBeScene.FindMonsterByID(monsters, boss1ID);
        if (monsters.Count > 0)
        {
            boss1 = monsters[0];
        }
        owner.CurrentBeScene.FindMonsterByID(monsters, boss2ID);
        if (monsters.Count > 0)
        {
            boss2 = monsters[0];
        }

        if (boss1 != null && boss2 != null)
        {
            var handler1 = boss1.RegisterEventNew(BeEventType.onStateChange, (param) =>
            {
                ActionState state = (ActionState)param.m_Int;
                if (state == ActionState.AS_DEAD)
                {
                    owner.DoDead();
                }
                else
                {
                    canUseSkill = CheckSpellCondition(boss1) && CheckSpellCondition(boss2);
                }
            });
            handlerList.Add(handler1);

            var handler2 = boss2.RegisterEventNew(BeEventType.onStateChange, (param) =>
            {
                ActionState state = (ActionState)param.m_Int;
                if (state == ActionState.AS_DEAD)
                {
                    owner.DoDead();
                }
                else
                {
                    canUseSkill = CheckSpellCondition(boss1) && CheckSpellCondition(boss2);
                }
            });
            handlerList.Add(handler2);
        }
        else
        {
            Logger.LogError("boss1 or boss2 do not exist");
        }

        timeAcc = 0;
    }

    bool CheckSpellCondition(BeActor actor)
    {
        if (boss1.IsDead() || boss2.IsDead())
            return false;

        bool flag1 =
            actor.stateController.HasBuffState(BeBuffStateType.FROZEN) ||
            actor.stateController.HasBuffState(BeBuffStateType.STUN) ||
            actor.stateController.HasBuffState(BeBuffStateType.STONE) ||
            actor.stateController.HasBuffState(BeBuffStateType.SLEEP) ||
            actor.stateController.HasBuffState(BeBuffStateType.STRAIN);

        bool flag2 =
            actor.sgGetCurrentState() == (int)ActionState.AS_FALL ||
            actor.sgGetCurrentState() == (int)ActionState.AS_GRABBED ||
            actor.sgGetCurrentState() == (int)ActionState.AS_CASTSKILL ||
            actor.sgGetCurrentState() == (int)ActionState.AS_BIRTH;

        return !flag2 && !flag1 && !actor.IsInPassiveState() && !actor.stateController.WillBeGrab();
    }

    public override void OnUpdate(int deltaTime)
    {
        timeAcc += deltaTime;
        if (timeAcc >= skillCD)
        {
            if (canUseSkill)
            {
                timeAcc = 0;
                canUseSkill = false;
                DoSkill();
            }
        }
#if !LOGIC_SERVER
        if (protectHurtTime > 0)
        {
            protectHurtTime -= deltaTime;
            if (protectHurtTime < 0)
            {
                protectHurtTime = 0;
            }
            if (battleUI != null)
            {
                battleUI.ChangeCountDown(protectHurtTime / 1000);
            }
        }
#endif
    }

    void DoSkill()
    {
        int playerCount = owner.CurrentBeBattle.dungeonPlayerManager.GetAllPlayers().Count;
        if (playerCount > 0)
        {
            int hard = owner.CurrentBeBattle.dungeonManager.GetDungeonDataManager().id.diffID;
            int coefficient = damageCoefficient[hard];
            VFactor factor = new VFactor(coefficient, GlobalLogic.VALUE_1000);
            damageValue = baseDamageValue * playerCount * (hard + 1) * factor;

            boss1.UseSkill(skill1ID, true);
            boss2.UseSkill(skill2ID, true);

            SummonProtector();

            CreateEntity();

            damageAcc = 0;

#if !LOGIC_SERVER
            SystemNotifyManager.SysDungeonSkillTip(tipsContent, duration / 1000f);
            battleUI = BattleUIHelper.CreateBattleUIComponent<BattleUIDungeonDamageBar>();
            ShowDamageBar(true);
            protectHurtTime = duration;
            if (battleUI != null)
            {
                battleUI.ChangeCountDown(protectHurtTime / 1000);
                battleUI.ChangeDamageData(damageAcc, damageValue);
            }
#endif
        }
    }

    /* 召唤护盾 */
    void SummonProtector()
    {
        object[] summoned = new object[1];

        if (boss1.DoSummon(protectorID, level, ProtoTable.EffectTable.eSummonPosType.ORIGIN, null, 1, 0, 0, 0, 0, false, 0, 0, null, SummonDisplayType.NONE, summoned))
        {
            if (summoned[0] != null)
            {
                protector1 = (BeActor)summoned[0];
                protector1.RegisterEventNew(BeEventType.onHit, args =>
                //protector1.RegisterEvent(BeEventType.onHit, (object[] args) =>
                {
                    ProtectorOnHurt(args.m_Int);
                });
            }
        }

        if (boss2.DoSummon(protectorID, level, ProtoTable.EffectTable.eSummonPosType.ORIGIN, null, 1, 0, 0, 0, 0, false, 0, 0, null, SummonDisplayType.NONE, summoned))
        {
            if (summoned[0] != null)
            {
                protector2 = (BeActor)summoned[0];
                protector2.RegisterEventNew(BeEventType.onHit, args =>
                //protector2.RegisterEvent(BeEventType.onHit, (object[] args) =>
                {
                    ProtectorOnHurt(args.m_Int);
                });
            }
        }
    }

    /* 生成大火球实体 */
    void CreateEntity()
    {
        var player = owner.CurrentBeBattle.dungeonPlayerManager.GetPlayerBySeat(0);
        if (player != null)
        {
            var birthPos = new VInt3(player.playerActor.GetPosition().vector3 + Vector3.up * (startHeight / 10000f));
            fireBall = (BeProjectile)owner.AddEntity(fireBallID, birthPos);

            if (fireBall != null)
            {
                handler = fireBall.RegisterEventNew(BeEventType.onDead, eventParam =>
                {
                    DoAttack();
                    StopBossSkill();
                });

                fireBall.delayCaller.DelayCall(duration, () =>
                {
                    if (fireBall != null && !fireBall.IsDead())
                    {
                        fireBall.SetMoveSpeedZ(fallSpeed);
                    }
                });
            }
        }
    }

    void DoAttack()
    {
#if !LOGIC_SERVER
        ShowDamageBar(false);
#endif
        //对场上所有对方actor造成伤害
        List<BeActor> allTargets = GamePool.ListPool<BeActor>.Get();
        owner.CurrentBeScene.FindTargets(allTargets, owner, VInt.Float2VIntValue(100f));

        for (int i = 0; i < allTargets.Count; i++)
        {
            var target = allTargets[i];
            var hitPos = owner.GetPosition();
            hitPos.z += VInt.one.i;
            owner._onHurtEntity(target, hitPos, attackID);
        }

        GamePool.ListPool<BeActor>.Release(allTargets);
    }

    void ProtectorOnHurt(int value)
    {
        damageAcc += value;
        if (damageAcc >= damageValue)
        {
            StopBossSkill();
            RemoveProjectile();
#if !LOGIC_SERVER
            ShowDamageBar(false);
#endif
        }
        else
        {
#if !LOGIC_SERVER
            if (battleUI != null)
            {
                ShowDamageBar(true);
                battleUI.ChangeDamageData(damageAcc, damageValue);
            }
#endif
        }
    }

    void StopBossSkill()
    {
        if (protector1 != null)
        {
            protector1.GetEntityData().SetHP(-1);
            protector1.DoDead();
        }
        if (protector2 != null)
        {
            protector2.GetEntityData().SetHP(-1);
            protector2.DoDead();
        }

        if (boss1 != null)
        {
            boss1.CancelSkill(skill1ID);
            boss1.sgSwitchStates(new BeStateData((int)ActionState.AS_IDLE));
            boss1.buffController.RemoveBuff(38);
        }
        if (boss2 != null)
        {
            boss2.CancelSkill(skill2ID);
            boss2.sgSwitchStates(new BeStateData((int)ActionState.AS_IDLE));
            boss2.buffController.RemoveBuff(38);
        }
    }

    void ShowDamageBar(bool show)
    {
#if !LOGIC_SERVER
        if (battleUI != null)
        {
            battleUI.ShowDamageBar(show);
        }
#endif
    }

    void RemoveProjectile()
    {
        if (handler != null)
        {
            handler.Remove();
            handler = null;
        }
        if (fireBall != null)
        {
            fireBall.DoDie();
        }
    }

    public override void OnFinish()
    {
        RemoveHandler();
    }

    void RemoveHandler()
    {
        for (int i = 0; i < handlerList.Count; i++)
        {
            handlerList[i].Remove();
            handlerList[i] = null;
        }
        handlerList.Clear();
    }
}
