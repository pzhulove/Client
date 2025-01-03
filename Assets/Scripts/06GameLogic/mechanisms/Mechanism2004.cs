using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mechanism2004 : BeMechanism
{
    private int resentmentValue; //当前的怨念值
    private readonly int addIntervalTime = 2000;//增加怨念值的间隔时间
    private readonly int minusIntervalTime = 600;//减少怨念值的间隔时间
    private readonly int timeAddValue = 1;//定时增加
    private readonly int hitAddValue = 10;//怪物击中增加
    private readonly int buffMinusValue = 100;//吃地上BUFF减少
    private readonly int timeMinusValue = 1;//定时减少
    private readonly int hitMinusValue = 5;//被玩家攻击减少
    private int buffID;
    private bool isBetray = false;
    private readonly int resentmentMax = 100;
    private bool updateFlag = true;
    private List<IBeEventHandle> handList = new List<IBeEventHandle>();
    private readonly int effectBuffID = 521871;
    private List<int> roomIDList = new List<int>();
    private bool showTip = true;
    private int[] reduceDamagePrecents = new int[4];
    public Mechanism2004(int mid, int lv) : base(mid, lv) { }

    public override void OnInit()
    {
        base.OnInit();
        if (data.ValueA.Length >= 4) //reduceDamagePrecents必须有四个值
        {
            for(int i = 0; i < reduceDamagePrecents.Length; ++i)
            {
                reduceDamagePrecents[i] = TableManager.GetValueFromUnionCell(data.ValueA[i], level);
            }
        }
    }

    public override void OnReset()
    {
        resentmentValue = 0;
        buffID = 0;
        isBetray = false;
        updateFlag = true;
        RemoveHandle();
        roomIDList.Clear();
        showTip = true;
        reduceDamagePrecents = new int[4];
        betrayCnt = 0;
        betrayTotal = 0;
        timer = 0;
        totalTime = 0;
        tipTime = 0;
        tempPause = false;
        bar = null;
    }

    public override void OnStart()
    {
        betrayTotal = 0;
        betrayCnt = 0;
        updateFlag = true;
        base.OnStart();
        handleA = OwnerRegisterEventNew(BeEventType.onHit, args =>
        //handleA = owner.RegisterEvent(BeEventType.onHit, (args) =>
        {
            if (isBetray)
            {
                //int[] damage = args[0] as int[];
                BeEntity entity = args.m_Obj as BeEntity;
                if (entity.IsSummonMonster())
                    return;
                BeEntity topOwner = entity.GetOwner();
                if (topOwner.IsSummonMonster()) return;
                if (topOwner != null)
                {
                    BeActor actor = topOwner as BeActor;
                    if (actor != null && actor.isMainActor)
                    {
                        OnChangeResentmentValue(-hitMinusValue);
#if !LOGIC_SERVER
                        if (showTip && owner.isLocalActor)
                        {
                            GameClient.SystemNotifyManager.SysDungeonSkillTip("被队友攻击可有效降低背叛状态下的怨念值", 3);
                            showTip = false;
                        }
#endif
                    }
                }
            }
        });

        handleB = owner.RegisterEventNew(BeEventType.onAfterFinalDamageNew, args =>
        //handleB = owner.RegisterEvent(BeEventType.onAfterFinalDamageNew, (args) =>
        {
            //int[] vals = args[0] as int[];
            BeActor actor = args.m_Obj as BeActor;
            if (actor != null)
            {
                if (actor.GetEntityData().isSummonMonster)
                    return;
                BeActor topOwner = actor.GetOwner() as BeActor;
                if (topOwner != null && topOwner.isMainActor)
                {
                    Mechanism2004 mechanism = topOwner.GetMechanism(5300) as Mechanism2004;
                    if (mechanism != null)
                    {
                        if (mechanism.IsBetray())
                        {
                            args.m_Int = 0;
                        }

                        Mechanism2004 ownerMechanism = owner.GetMechanism(5300) as Mechanism2004;
                        if (ownerMechanism != null && ownerMechanism.IsBetray() && !mechanism.IsBetray())
                        {
                            if (owner != null && owner.CurrentBeBattle != null && owner.CurrentBeBattle.dungeonManager != null &&
                                owner.CurrentBeBattle.dungeonManager.GetDungeonDataManager() != null)
                            {
                                var diff = owner.CurrentBeBattle.dungeonManager.GetDungeonDataManager().id.diffID;
                                VFactor v = VFactor.zero;
                                if (diff == 0)
                                {
                                    v = new VFactor(reduceDamagePrecents[0], 100);
                                }
                                else if (diff == 1)
                                {
                                    v = new VFactor(reduceDamagePrecents[1], 100);
                                }
                                else if (diff == 2)
                                {
                                    v = new VFactor(reduceDamagePrecents[2], 100);
                                }
                                else if (diff == 3)
                                {
                                    v = new VFactor(reduceDamagePrecents[3], 100);
                                }
                                args.m_Int *= v;
                            }
                        }
                    }


                }
            }
        });


        handleC = owner.RegisterEventNew(BeEventType.onAddBuff, (args) =>
        {
            BeBuff buff = args.m_Obj as BeBuff;
            if (buff.buffID == buffID)
            {
                OnChangeResentmentValue(-buffMinusValue);
            }
        });

        //玩家身上有特殊buff时，就不能上某个ID的buff
        var handle = owner.RegisterEventNew(BeEventType.BuffCanAdd, (args) =>
        {
            int id = args.m_Int2;
            if ((id == 521727 || id == 521728 || id == 521729) && HaveBuff(id))
            {
                args.m_Int = 1;
            }
        });

        //怪物给玩家上一个变身BUFF，并且自己上一个召唤BUFF
      var  handle1 = owner.RegisterEventNew(BeEventType.onBuffBeforePostInit, (args) =>
        {
            BeBuff buff = args.m_Obj as BeBuff;
            if (buff != null)
            {
                if (buff.buffID == 521826 || buff.buffID == 521847 || buff.buffID == 521828)
                {
                    List<BeActor> list = GamePool.ListPool<BeActor>.Get();
                    owner.CurrentBeScene.FindMonsterByID(list, 30780031);
                    for (int i = 0; i < list.Count; i++)
                    {
                        int id = 0;
                        switch (buff.buffID)
                        {
                            case 521826:
                                id = 521844;
                                break;
                            case 521847:
                                id = 521845;
                                break;
                            case 521828:
                                id = 521846;
                                break;
                            default:
                                break;
                        }
                        list[i].buffController.TryAddBuff(id, 100);
                    }
                    GamePool.ListPool<BeActor>.Release(list);
                }
            }
        });

        var handle2 = owner.RegisterEventNew(BeEventType.onDead, eventParam => 
        {
            OnChangeResentmentValue(-resentmentValue);
        });

        var handle3 = owner.RegisterEventNew(BeEventType.onRemoveBuff, (args) =>
        {
            int buffID = (int)args.m_Int;
            if (buffID == 521828)
            {
                OnChangeResentmentValue(-resentmentValue);
            }
        });
        var handle4 = owner.RegisterEventNew(BeEventType.OnBeforePassDoor, (args) =>
        {
#if !SERVER_LOGIC
            if (owner.isLocalActor && owner.CurrentBeBattle != null 
            && owner.CurrentBeBattle.dungeonManager != null 
            && owner.CurrentBeBattle.dungeonManager.GetDungeonDataManager() != null)
            {
                int areaID = owner.CurrentBeBattle.dungeonManager.GetDungeonDataManager().CurrentAreaID();
                if (roomIDList.Contains(areaID)) return;
                roomIDList.Add(areaID);
                GameStatisticManager.instance.SendBatrayCount(betrayCnt.ToString(),string.Format("running_{0}", areaID));
                betrayCnt = 0;
            }
#endif
        });
        handList.Add(handle1);
        handList.Add(handle2);
        handList.Add(handle3);
        handList.Add(handle4);
        handList.Add(handle);
    }

    public int betrayCnt = 0;
    public int betrayTotal = 0;
    private bool HaveBuff(int id)
    {
        if (owner.buffController != null)
        {
            return owner.buffController.HasBuffByID(521727) != null ||
                   owner.buffController.HasBuffByID(521728) != null ||
                   owner.buffController.HasBuffByID(521729) != null;
        }
        return false;
    }

    public void CopyMechanims(int value, bool isBetray)
    {
        resentmentValue = value;
        this.isBetray = isBetray;
    }

    int timer = 0;
    public override void OnUpdate(int deltaTime)
    {
        UpdateShowTip(deltaTime);

        base.OnUpdate(deltaTime);
        if (!updateFlag) return;
        if (!isBetray)
        {
            timer += deltaTime;
            if (timer >= addIntervalTime)
            {
              //  OnChangeResentmentValue(timeAddValue);
                timer = 0;
            }
        }
        else
        {
            timer += deltaTime;
            if (timer >= minusIntervalTime)
            {
                OnChangeResentmentValue(-timeMinusValue);
                timer = 0;
            }
        }

       
    }

    private int totalTime = 6000;
    private int tipTime = 0;
    private void UpdateShowTip(int delta)
    {
        if (!showTip)
        {
            tipTime += delta;
            if (tipTime > totalTime)
            {
                showTip = true;
                tipTime = 0;
            }
        }
    }


    public bool IsBetray()
    {
        return isBetray;
    }

    public void OnChangeResentmentValue(int value)
    {
        if (owner.IsDead() && value > 0) return;
        if (resentmentValue >= resentmentMax && isBetray && value > 0) return;
        if (resentmentValue <= 0 && value < 0) return;
        if (tempPause) return;
        ShowResentmentValue(value);
        this.resentmentValue += value;
        if (resentmentValue >= resentmentMax)
        {
            resentmentValue = resentmentMax;
            tempPause = true;
#if !LOGIC_SERVER
            if (owner.isLocalActor)
                GameClient.SystemNotifyManager.SysDungeonSkillTip("即将背叛，注意不要误伤队友", 3);
#endif
            owner.CurrentBeScene.DelayCaller.DelayCall(3000, () =>
            {
                tempPause = false;
                isBetray = true;
                timer = 0;
                owner.buffController.TryAddBuff(effectBuffID, -1);
                 owner.stateController.ResetAttackAbility();
                owner.stateController.SetAbilityEnable(BeAbilityType.ATTACK_FRIEND_ENEMY, false);
            });

        }
        else if (resentmentValue <= 0)
        {
            resentmentValue = 0;
            isBetray = false;
            timer = 0;
            owner.buffController.RemoveBuff(effectBuffID);
            owner.stateController.ResetAttackAbility();
            owner.stateController.SetAbilityEnable(BeAbilityType.ATTACK_FRIEND_ENEMY, true);
        }

        if (isBetray)
        {
            RefreshEnergrUI(value);
        }
        else
        {
            RemoveSpellBar();
        }
    }

    private void ShowResentmentValue(int value)
    {
#if !SERVER_LOGIC
        if (value == 1 || value == -1) return;
        ComCharactorHPBar hpBar = owner.m_pkGeActor.mCurHpBar as ComCharactorHPBar;
        if (hpBar != null)
        {
            hpBar.ShowResentmentChange(value);
        }
#endif
    }

    private bool tempPause = false;
    SpellBar bar = null;

    protected void RefreshEnergrUI(int value)
    {
#if !LOGIC_SERVER
        var dur = owner.GetSpellBarDuration(eDungeonCharactorBar.MonsterEnergyBar);
        if (dur <= 0)
        {
            bar = owner.StartSpellBar(eDungeonCharactorBar.MonsterEnergyBar, resentmentMax, true, "", true);
            bar.autoAcc = false;
            bar.reverseAutoAcc = false;
            bar.autodelete = false;
        }
        owner.AddSpellBarProgress(eDungeonCharactorBar.MonsterEnergyBar, (float)value / resentmentMax);
#endif
    }

    private void RemoveSpellBar()
    {
#if !LOGIC_SERVER
        if(bar!=null)
        {
          owner.StopSpellBar(eDungeonCharactorBar.MonsterEnergyBar);
          bar = null;
        }
#endif
    }

    public int GetResentmentValue()
    {
        return resentmentValue;
    }

    public void SetUpdateFlag(bool flag)
    {
        updateFlag = flag;
    }

    private void RemoveHandle()
    {
        for (int i = 0; i < handList.Count; i++)
        {
            if (handList[i] !=null)
            {
                handList[i].Remove();
                handList[i] = null;
            }
        }
        handList.Clear();
    }

    public override void OnFinish()
    {
        base.OnFinish();
        RemoveHandle();
    }

}
