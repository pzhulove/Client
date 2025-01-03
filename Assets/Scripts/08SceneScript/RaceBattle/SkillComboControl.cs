using GameClient;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG;
using DG.Tweening;
using ProtoTable;
///////删除linq
using Protocol;

public class SkillComboControl : Singleton<SkillComboControl>
{


    private string effectPath1 = "Effects/UI/Prefab/EffUI_lianzhaoxitong/Prefab/EffUI_lianzhaoxitong_zhenqiancaoyan";
    private string effectPath2 = "Effects/UI/Prefab/EffUI_lianzhaoxitong/Prefab/EffUI_lianzhaoxitong_zhenjiantiaozhan";
    private string effectPath3 = "Effects/UI/Prefab/EffUI_lianzhaoxitong/Prefab/EffUI_lianzhaoxitong_kaishi";
    private int roomID = 0;
    public int monitorSkillID = 0;

    private GameObject effectObj;
    private GameObject tip = null;

    private BeDungeon dungeonData = null;
    private List<ComboData> skillList;
    private BeActor curActor = null;
    private SkillComboFrame frame;
    private ClientSystemBattle systemBattle;
    private TrainingSkillComboBattle battle;
    private InstituteTable institueData = null;
    private InstituteBattleFrame battleFrame = null;
    public bool hasPassed = false;
    private bool hasFinished = false;
    private bool battleEnd = false;
    public void Init(BeDungeon data, int roomID)
    {
        battleEnd = false;
           hasPassed = false;
        hasFinished = false;
           monitorSkillID = 0;
        this.dungeonData = data;
        this.roomID = roomID;

        if (dungeonData == null) return;

        curActor = BattleMain.instance.GetLocalPlayer().playerActor;
        if (curActor == null) return;

        systemBattle = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemBattle;
        if (systemBattle == null) return;

        battleFrame = ClientSystemManager.instance.GetFrame(typeof(InstituteBattleFrame)) as InstituteBattleFrame;
        if (battleFrame == null) return;

        battle = BattleMain.instance.GetBattle() as TrainingSkillComboBattle;
        if (battle == null||battle.teachData==null) return;
        
        institueData = TableManager.instance.GetDataByDungeonID(curActor.professionID, roomID);
        if (institueData == null) return;

        skillList = battle.teachData.datas.ToList();
        if (skillList.Count == 0) return;

        AddBuff();

        if (!ClientSystemManager.instance.IsFrameOpen<SkillComboFrame>())
        {
            frame = ClientSystemManager.instance.OpenFrame<SkillComboFrame>() as SkillComboFrame;
        }

        battle.playerHitCallBack = PlayerHitOther;

        hasPassed = MissionManager.GetInstance().GetState(institueData) == 1;

        if (institueData.Type == 2)
        {
            GameFrameWork.instance.StartCoroutine(CastDunFu());
        }
        else
        {

            if (institueData.DifficultyType == 1)
            {
                frame.InitSkillComboUI(curActor.professionID, roomID);
                battleFrame.SetControlContainer(false);
                GameFrameWork.instance.StartCoroutine(InstitueTeach());
            }
            else
            {
                battleFrame.SetControlContainer(true);
                if (hasPassed)
                {
                    GameFrameWork.instance.StartCoroutine(StartCastSkill());
                }
                else
                {
                    InputManager.instance.SetEnable(false);
                    ClientSystemManager.instance.OpenFrame<ComboSkipFrame>();
                }
            }
        }
    }

    private void AddBuff()
    {
        if (institueData == null)
            return;
        if (curActor == null)
            return;
        for (int i = 0; i < institueData.BuffID.Count; i++)
        {
            curActor.buffController.TryAddBuff(institueData.BuffID[i]);
        }
        
    }

    private void PlayerHitOther(int skillID, int id)
    {
        if (hasFailed)
        {
            ShowTip();
            return;
        }
        BeSkill skill = curActor.GetSkill(skillID);
        if (skill == null) return;
        if (monitorSkillID == 0) return;
        if (skill.comboSkillSourceID != 0)
        {
            ExecuteSkill(id);
        }
        else
        {
            ExecuteSkill(skillID);
        }
    }

    private void ExecuteSkill(int skillID)
    {
        if (skillID == monitorSkillID)
        {
            if (currentItem == null) return;
            currentItem.StopComboCD();
            index++;
            if (index >= frame.GetComboItemList().Count)
            {
                if (hasFinished) return;
                hasFinished = true;
                if(battleFrame != null)
                    battleFrame.SetBtnEnable(false);
                SystemNotifyManager.SysDungeonSkillTip("挑战完成", 3);              
                ClientSystemManager.instance.delayCaller.DelayCall(2000, () =>
                {
                    battle.EndInstitueTrain();
                });


                return;
            }
            currentItem = frame.GetShowItem(index);
            currentItem.StartComboCD();
            SetMonitorSkillID(currentItem.skillID);
        }
    }

    #region 深渊引导
    public void StartHellGuide(BeDungeon dungeon)
    {
        int roomID = dungeon.GetDungeonDataManager().CurrentAreaID();
        dungeonData = dungeon;
        GameFrameWork.instance.StartCoroutine(StartGuide(roomID));
    }

    private IEnumerator StartGuide(int id)
    {
        if (id == 23101)
        {
            return new ProcessUnit().Append(_WaitForSeconds(0.2f)).Append(_waitForDialog(50002)).Append(_ShowDungeonDesFrame(id)).Sequence();
        }
        else if (id == 23131)
        {
            return new ProcessUnit().Append(_WaitForSeconds(0.2f)).Append(_ShowDungeonDesFrame(id)).Sequence();
        }
        else
        {
            return new ProcessUnit().Sequence();
        }

    }
    IEnumerator _WaitForSeconds(float time)
    {
        yield return Yielders.GetWaitForSeconds(time);
    }
    IEnumerator _ShowDungeonDesFrame(int roomID)
    {
        ClientSystemManager.instance.OpenFrame<DungeonInfoFrame>(FrameLayer.Middle, roomID);
        yield break;
    }

    #endregion

    /// <summary>
    /// 返回实战阶段
    /// </summary>
    public void RestartPracticeFight()
    {
        if (institueData != null && institueData.Type == 2)
        {
            GameFrameWork.instance.StartCoroutine(PracticeDunFu());
        }
        else
        {
            GameFrameWork.instance.StartCoroutine(ReturnPractice());
        }

    }

    private IEnumerator ReturnPractice()
    {
        return new ProcessUnit()
            .Append(RecreateEntity())
            .Append(StartPractice())
            .Sequence();
    }

    /// <summary>
    /// 返回教学阶段
    /// </summary>
    /// <returns></returns>
    public void RestartTeachFight()
    {
        if (institueData != null && institueData.Type == 2)
        {
            GameFrameWork.instance.StartCoroutine(RestartTeachDunFu());
        }
        else
        {
            GameFrameWork.instance.StartCoroutine(Restart());
        }

    }

    private IEnumerator Restart()
    {
        if(InputManager.instance!=null)
           InputManager.instance.SetEnable(false);
        if(dungeonData!=null)
           dungeonData.ResumeFight();
        DestroyEffect();
        return new ProcessUnit()
                  .Append(RecreateEntity())
                  .Append(_WaitForSeconds(0.3f))
                  .Append(RestartTeach())
                  .Sequence();
    }

    private IEnumerator RestartTeach()
    {

        if (institueData.DifficultyType == 1)
        {
            return new ProcessUnit()
                .Append(InitUI(true))
                .Append(FreazeMonsters(-1, true, false))
                .Append(StartToTeachFight())
                .Append(ReturnPractice())
               .Sequence();
        }
        else
        {
            SetBattleFrame(1);
            return new ProcessUnit()
                .Append(InitData())
                .Append(InitUI(false))
                .Append(LoadTeachEffect(false))
               .Append(StartSkill(0))
               .Sequence();
        }


    }

    private IEnumerator InitUI(bool showArrow)
    {
        if(curActor != null)
            frame.InitSkillComboUI(curActor.professionID, roomID, showArrow);
        yield break;
    }

    private IEnumerator InitData()
    {
        hasFailed = false;
        InputManager.instance.SetVisible(false);
        battle.UseSkillCallBack = null;
        if(battleFrame != null)
            battleFrame.SetControlContainer(true);
        yield break;
    }

    public void SetEndBattle()
    {
        battleEnd = true;
    }

    private void CreateHPBar()
    {
        if (curActor == null || curActor.m_pkGeActor == null) return;
        curActor.m_pkGeActor.CreateHPBarCharactor(ClientApplication.playerinfo.seat);
    }

    private void DestroyHPBar()
    {
        if (curActor == null || curActor.m_pkGeActor == null) return;
        curActor.m_pkGeActor.DestroySelfHPBar();
    }

    #region 武研院进阶连招引导

    #region 主动释放
    public IEnumerator StartCastSkill()
    {
        if (hasPassed)
        {
            frame.InitSkillComboUI(curActor.professionID, roomID, true);
            return new ProcessUnit()
               .Append(StartPractice()).Sequence();
        }
        else
        {
            frame.InitSkillComboUI(curActor.professionID, roomID, false);
            SetBattleFrame(1);
            InputManager.instance.SetVisible(false);
            return new ProcessUnit()
                .Append(LoadTeachEffect(false))
                .Append(StartSkill(0)).Sequence();
        }
    }


    private IEnumerator StartSkill(int index)
    {

        if (battleEnd) yield break;
        if (index >= skillList.Count)
        {
            yield return ReturnPractice();
            yield break;
        }

        float skillTime = 0;
        float moveTime = 0;
        float idleTime = 0;
        int moveDir = 0;
        int skillID = 0;
        ComboData item = battle.GetComboData(index);
        if (item != null)
        {
            skillTime = item.skillTime / 1000.0f;
            moveTime = item.moveTime / 1000.0f;
            idleTime = item.idleTime / 1000.0f;
            moveDir = item.moveDir;
        }
        skillID = skillList[index].skillID;


        if (moveTime > 0)
        {
            FireMoveCommand((short)moveDir, moveTime);
            yield return new WaitForSeconds(moveTime);
            InputManager.instance.FireStopCommand();
            yield return new WaitForSeconds(idleTime);
        }
        if (curActor == null) yield break;
        BeSkill skill = curActor.GetSkill(skillID);
        if (skill.comboSkillSourceID != 0)
            skillID = skill.comboSkillSourceID;

        curActor.SetFace(item.faceRight != 1, true, true);
        frame.ShowComboItemEffect(item.skillGroupID);
        InputManager.instance.CreateSkillFrameCommand(skillID);
        InputManager.instance.CreateSkillFrameCommand(skillID, new SkillFrameCommand.SkillFrameData{isUp = true});
        float preTime = Time.realtimeSinceStartup;
        yield return new WaitForSeconds(skillTime);
        index++;
        yield return StartSkill(index);
    }

    private DelayCallUnitHandle delayUnit;
    private void FireMoveCommand(short nDegree, float moveTime)
    {
        if (delayUnit.IsValid())
        {
            delayUnit.SetRemove(true);
        }
        int time = 0;
        bool runing = true;
        delayUnit = ClientSystemManager.instance.delayCaller.RepeatCall(30, () =>
        {
            if (!runing) return;
            time += 30;
            if (time > ((moveTime * 1000) - 30))
            {
                if (delayUnit.IsValid())
                {
                    delayUnit.SetRemove(true);
                }
                runing = false;
                return;
            }
            GameClient.MoveFrameCommand cmd = new GameClient.MoveFrameCommand
            {
                degree = nDegree,
                run = true
            };
            FrameSync.instance.FireFrameCommand(cmd);

            FrameSync.instance.bInRunMode = true;
            FrameSync.instance.nDegree = nDegree;

        }, 999, true);


    }


    #endregion
    #endregion

    #region 武研院初级连招引导

    private IEnumerator InstitueTeach()
    {
        if (hasPassed)
        {
            return new ProcessUnit()
                                 .Append(StartPractice())
                                 .Sequence();
        }
        else
        {
            return new ProcessUnit()
                  .Append(FreazeMonsters(-1, true, false))
                  .Append(StartToTeachFight())
                  .Append(ReturnPractice())
                  .Sequence();
        }

    }

    #region 教学阶段
    private IEnumerator StartToTeachFight()
    {
        hasFailed = false;
        SetBattleFrame(1);

        SkillComboItem.timeOutCallBack = () =>
        {
            if (institueData.DifficultyType == 2)
                ShowTip();
            if (institueData.DifficultyType == 1)
                RestartTeachFight();
        };
        InputManager.instance.SetVisible(false, true);
        InputManager.instance.SetEnable(false);
        return new ProcessUnit()
            .Append(LoadTeachEffect())
             .Append(_Guide2())
             .Sequence();
    }

    private void SetBattleFrame(int type)
    {
        if (battleFrame == null) return;
        if (type == 1)//演示教学阶段
        {
            SetMonitorSkillID(0);
            DestroyHPBar();
            battleFrame.SetEffectState(false);
            battleFrame.SetTitle(0);
            battleFrame.SetBtnState(false);
        }
        else
        {
            battleFrame.ResetTimeScale();
            battleFrame.SetControlContainer(false);
            battleFrame.SetEffectState(false);
            battleFrame.SetTitle(1);
            battleFrame.SetBtnState(true);
        }
    }

    private void DestroyEffect()
    {
        DestroyEffectObj();
        DestroyTip();
    }

    #endregion

    #region 实战试炼




    private IEnumerator RecreateEntity()
    {
        if(battle!=null && BattleMain.instance != null && BattleMain.instance.GetLocalPlayer() != null)
        {
            battle.RecreatePlayer();
            curActor = BattleMain.instance.GetLocalPlayer().playerActor;
            AddBuff();
            BeUtility.ResetCamera();
        }     
        yield break;
    }

    private bool hasFailed = false;

    /// <summary>
    ///  
    /// </summary>
    /// <returns></returns>
    private IEnumerator StartPractice()
    {
        ResetPracticeUI();
        dungeonData.ResumeFight();
        SkillComboItem.timeOutCallBack = () =>
        {
            ShowTip();
        };

        yield return LoadPracticeEffect();
        WaitInputSkill();
    }
    private void ResetPracticeUI()
    {
        hasFailed = false;
        CreateHPBar();
        DestroyEffect();
        if(frame != null)
            frame.InitSkillComboUI(curActor.professionID, roomID);

        SetBattleFrame(2);
        if (InputManager.instance != null)
            InputManager.instance.SetEnable(false);

    }


    int index = 0;
    bool flag = false;
    private void WaitInputSkill()
    {
        flag = false;
        index = 0;

        battle.UseSkillCallBack = (id) =>
        {
            if (index >= skillList.Count) return;
            if (index == 0)
            {
                if (flag) return;
                flag = true;
                currentItem = frame.GetShowItem(0);
                if (currentItem != null)
                {
                    currentItem.StartComboCD();
                    SetMonitorSkillID(currentItem.skillID);
                }
            }
            if (curActor == null) return;
            BeSkill skill = curActor.GetSkill(id);
            if (skill != null && skill.isBuffSkill)
            {
                ExecuteSkill(id);
            }
        };


    }

    SkillComboItem currentItem = null;
    private void SetMonitorSkillID(int skillID)
    {
        monitorSkillID = skillID;
    }

    public void ShowTip()
    {
        if(battleFrame != null)
            battleFrame.SetEffectState(true);
        hasFailed = true;
        SkillComboItem.timeOutCallBack = null;
        if (tip != null)
        {
            return;
        }
        tip = SystemNotifyManager.SysDungeonSkillTip("挑战失败，请点击重置", 3);       
    }

    #endregion
    #endregion

    #region 新手关卡连招引导

    private IEnumerator _waitForDialog(int id)
    {
        if (id <= 0) yield break;
        dungeonData.PauseFight();

        bool isFinish = false;

        var task = new GameClient.TaskDialogFrame.OnDialogOver();
        task.AddListener(() =>
        {
            isFinish = true;
        });
        MissionManager.GetInstance().CreateDialogFrame(id, 0, task);

        while (!isFinish)
        {
            yield return Yielders.EndOfFrame;
        }

        dungeonData.ResumeFight();
    }


    #endregion

    #region 实现技能连招
    private IEnumerator _Guide2()
    {
        return new ProcessUnit()
             .Append(StartSkillCombo(0))
             .Append(DestroyEffectobj())
             .Append(UnFreazeMonsters())
             .Sequence();
    }

    private IEnumerator StartSkillCombo(int index)
    {
        InputManager.needJoystickOnTouch = false;
        if (index >= skillList.Count)
            yield break;

        InputManager.instance.SetEnable(true);
        SetAllSkillBtnState(false);
        dungeonData.PauseFight();
        SkillComboItem comboItem = frame.GetShowItem(index);
        int skillID = 0;
        if (comboItem != null)
        {
            skillID = comboItem.skillID;
            if (index != 0)
                comboItem.StartComboCD();
            comboItem.SelectCurrent();
        }

        bool bInput = false;
        ETCButton btn = GetSkillBtn(skillID);
        if (btn != null)
        {
            effectObj = LoadUIEffect(btn.gameObject, BaseNewbieGuideBattle.buttonTips);
            effectObj.transform.localPosition = Vector3.zero;
            btn.activated = true;
        }


        battle.UseSkillCallBack = (id) =>
        {
            if (index == 0)
                comboItem.StartComboCD();
            if (bInput) return;
            if (id == skillID || (id == (int)SpecialSkillID.JUMP_BACK && skillID == 9990))
            {
                SetAllSkillBtnState(false);
                InputManager.instance.CreateSkillFrameCommand(skillID, new SkillFrameCommand.SkillFrameData{isUp = true});
                index++;
                DestroyTip();
                DestroyEffectObj();
                bInput = true;
                dungeonData.ResumeFight();

                if (comboItem != null)
                {

                    comboItem.StopComboCD();

                }
            }
        };

        while (!bInput)
        {
            yield return Yielders.EndOfFrame;
        }

        float waitTime = 0;

        waitTime = skillList[index - 1].skillTime / 1000.0f;

        if (index >= skillList.Count)
        {
            SetAllSkillBtnState(true);
        }
        yield return Yielders.GetWaitForSeconds(waitTime);
        yield return StartSkillCombo(index);
    }


    private IEnumerator DestroyEffectobj()
    {
        DestroyEffectObj();
        yield break;
    }

    private IEnumerator FreazeMonsters(int id = -1, bool isMonster = true, bool modifyMonsterData = true)
    {

        var entityList = dungeonData.GetBeScene().GetFullEntities();
        for (int i = 0; i < entityList.Count; ++i)
        {
            var current = entityList[i] as BeActor;

            bool flag = false;

            if (isMonster)
            {
                if (current != null && current.IsMonster() && (id == -1 ? true : id == current.GetEntityData().monsterID))
                {
                    flag = true;
                }
            }
            else
            {
                if (current != null && current.GetCamp() == (int)ProtoTable.UnitTable.eCamp.C_HERO && (id == -1 ? true : id == current.GetEntityData().monsterID))
                {
                    BeEntity player = dungeonData.GetDungeonPlayerDataManager().GetMainPlayer().playerActor;
                    if (current != player)
                    {
                        flag = true;
                    }
                }
            }

            if (flag)
            {
                current.buffController.TryAddBuff(68, -1);
                current.hasAI = false;
                current.Reset();
                if (modifyMonsterData)
                {
                    current.GetEntityData().battleData.dodge = 0;
                }
            }
        }
        yield break;
    }

    private IEnumerator UnFreazeMonsters(int id = -1, bool isMonster = true)
    {
        var entityList = dungeonData.GetBeScene().GetFullEntities();
        for (int i = 0; i < entityList.Count; ++i)
        {
            var current = entityList[i] as BeActor;

            bool flag = false;

            if (isMonster)
            {
                if (current != null && current.IsMonster() && (id == -1 ? true : id == current.GetEntityData().monsterID))
                {
                    flag = true;
                }
            }
            else
            {
                if (current != null && current.GetCamp() == (int)ProtoTable.UnitTable.eCamp.C_HERO && (id == -1 ? true : id == current.GetEntityData().monsterID))
                {
                    BeEntity player = dungeonData.GetDungeonPlayerDataManager().GetMainPlayer().playerActor;
                    if (current != player)
                    {
                        flag = true;
                    }
                }
            }

            if (flag)
            {
                current.buffController.RemoveBuff(68);
                current.hasAI = true;
                current.Reset();
                current.StartAI(null);
            }
        }

        yield break;
    }

    #endregion
    #region 蹲伏，抓取

    private IEnumerator CastDunFu()
    {

        if (hasPassed)
        {
            if(battleFrame != null)
                battleFrame.SetControlContainer(false);
            frame.InitSkillComboUI(curActor.professionID, roomID, true);
            return new ProcessUnit()
            .Append(StartDunFuPractice())
            .Append(ResetUI())
            .Append(CastSkill())
            .Sequence();
        }
        else
        {
            if (battleFrame != null)
                battleFrame.SetControlContainer(true);
            frame.InitSkillComboUI(curActor.professionID, roomID, false);
            SetBattleFrame(1);
            InputManager.instance.SetVisible(false);
            return new ProcessUnit()
                .Append(LoadTeachEffect())
                .Append(TeachCastSkill())
                .Append(SetAttackButtonState())
                .Append(_WaitForSeconds(1))
                .Append(PracticeDunFu())
                .Sequence();
        }
    }

    private IEnumerator SetAttackButtonState()
    {
        if (curActor != null)
            curActor.SetAttackButtonState(ButtonState.PRESS);
        yield return Yielders.GetWaitForSeconds(2);
        if (curActor != null)
            curActor.SetAttackButtonState(ButtonState.RELEASE);
    }

    private IEnumerator PracticeDunFu()
    {
        return new ProcessUnit()
           .Append(RecreateEntity())
           .Append(StartDunFuPractice())
           .Append(ResetUI())
           .Append(CastSkill())
           .Sequence();
    }

    private IEnumerator RestartTeachDunFu()
    {
        if (battleFrame != null)
            battleFrame.SetControlContainer(true);
        DestroyEffect();
        frame.InitSkillComboUI(curActor.professionID, roomID, false);
        SetBattleFrame(1);
        return new ProcessUnit()
            .Append(RecreateEntity())
            .Append(HideInputmanager())
            .Append(LoadTeachEffect())
            .Append(TeachCastSkill())
            .Append(SetAttackButtonState())
            .Append(_WaitForSeconds(2))
            .Append(PracticeDunFu())
            .Sequence();
    }

    private IEnumerator HideInputmanager()
    {
        InputManager.instance.SetVisible(false);
        yield break;
    }

    private IEnumerator ResetUI()
    {
        ResetPracticeUI();
        dungeonData.ResumeFight();
        yield return LoadPracticeEffect();
    }

    private IEnumerator StartDunFuPractice()
    {
        if(battleFrame!=null)
           battleFrame.SetControlContainer(false);

        if (InputManager.instance != null)
        {
            InputManager.instance.SetEnable(true);
            if(InputManager.instance.joystick != null)
                InputManager.instance.joystick.visible = false;
        }
        if (curActor != null)
        {
            curActor.RegisterEventNew(BeEventType.onStateChange, (GameClient.BeEvent.BeEventParam param) =>
            {
                ActionState state = (ActionState)param.m_Int;
                if (state == ActionState.AS_GETUP)
                {
                    if (curActor!=null && curActor.GetCurrentBtnState() == ButtonState.PRESS)
                    {
                        if (frame != null)
                        {
                            SkillComboItem item = frame.GetShowItem(0);
                            if (item != null)
                                item.StopComboCD();
                        }
                        SkillComboItem.timeOutCallBack = null;
                        SystemNotifyManager.SysDungeonSkillTip("挑战完成", 3);
                        ClientSystemManager.instance.delayCaller.DelayCall(2000, () =>
                        {
                            if(battle!=null)
                               battle.EndInstitueTrain();
                        });
                        if (battleFrame != null)
                            battleFrame.SetBtnEnable(false);
                    }
                    else
                    {
                        ShowTip();
                    }
                }
            });
        }
        yield break;
    }

    private IEnumerator CastSkill()
    {
        BeActor actor = GetTargetPlayer();
        if (actor != null)
            actor.UseSkill(5900);
        SkillComboItem.timeOutCallBack = () =>
        {
            ShowTip();
        };
        SkillComboItem item = frame.GetShowItem(0);
        if (item != null)
            item.StartComboCD();
        yield return new WaitForSeconds(2);
        InputManager.instance.SetEnable(true);
    }

    private IEnumerator TeachCastSkill()
    {
        BeActor actor = GetTargetPlayer();
        if (actor != null)
            actor.UseSkill(5900);

        yield return new WaitForSeconds(2);
    }

    private BeActor GetTargetPlayer()
    {
        if (curActor == null) return null;
        List<BeEntity> list = curActor.CurrentBeScene.GetEntities();
        for (int i = 0; i < list.Count; i++)
        {
            BeActor actor = list[i] as BeActor;
            if (actor != null && !actor.isLocalActor)
                return actor;
        }
        return null;
    }

    #endregion
    #region 工具方法
    private ETCButton GetSkillBtn(int skillID)
    {
        ETCButton btn = InputManager.instance.GetETCButton(skillID);
        if (btn != null)
        {
            return btn;
        }
        else if (skillID == 9990)
        {
            return InputManager.instance.GetSpecialETCButton(SpecialSkillID.JUMP_BACK);
        }
        else
        {
            Logger.LogErrorFormat("该职业没有技能ID{0}", skillID);
        }
        return null;
    }

    private GameObject effect1;
    private GameObject effect2;
    private GameObject effect3;
    private IEnumerator LoadTeachEffect(bool enable = true)
    {
        if(battleFrame != null)
            battleFrame.SetBtnEnable(false);
        effect1 = LoadUIEffect(ClientSystemManager.instance.GetLayer(FrameLayer.Top), effectPath1);
        yield return new WaitForSeconds(1.5f);
        if (effect1 != null)
        {
            GameObject.Destroy(effect1);
            effect1 = null;
        }
        if(battleFrame != null)
            battleFrame.SetBtnEnable(true);
        AddBuffForEnemy();
        yield return new WaitForSeconds(1);

    }

    private IEnumerator LoadPracticeEffect()
    {
        if (battleFrame != null)
        {
            battleFrame.SetBtnEnable(false);
            effect2 = LoadUIEffect(battleFrame.GetFrame(), effectPath2);
        }
        yield return new WaitForSeconds(1.5f);
        if (effect2 != null)
        {
            GameObject.Destroy(effect2);
            effect2 = null;
        }
        yield return LoadStartEffect();
        yield return new WaitForSeconds(1);
        AddBuffForEnemy();

        if (effect3 != null)
        {
            GameObject.Destroy(effect3);
            effect3 = null;
        }
        if(battleFrame != null)
            battleFrame.SetBtnEnable(true);
        InputManager.instance.SetEnable(true);
    }

    private void AddBuffForEnemy()
    {
        BeActor actor = GetTargetPlayer();
        if (actor != null && institueData != null && institueData.EnemyBuffID.Count > 0)
        {
            int buffID = institueData.EnemyBuffID[0];
            if (actor.buffController.HasBuffByID(buffID) != null)
            {
                actor.buffController.RemoveBuff(buffID);
            }
            actor.buffController.TryAddBuff(buffID, -1);
        }
        if (institueData != null && !string.IsNullOrEmpty(institueData.Tip))
            SystemNotifyManager.SysDungeonSkillTip(institueData.Tip, 10);
    }


    private IEnumerator LoadStartEffect()
    {
        if (effect3 != null)
        {
            GameObject.Destroy(effect3);
            effect3 = null;
        }
        if(battleFrame != null)
            effect3 = LoadUIEffect(battleFrame.GetFrame(), effectPath3);
        yield break;
    }


    public GameObject LoadUIEffect(GameObject parent, string path, bool bKeep = true)
    {
        var go = AssetLoader.instance.LoadResAsGameObject(path);
        if (bKeep)
        {
            Utility.AttachTo(go, parent);
        }
        else
        {
            Utility.AttachTo(go, parent.transform.parent.gameObject, false);
            var p = parent.GetComponent<RectTransform>();
            var l = go.GetComponent<RectTransform>();
            l.position = p.position;
        }
        go.transform.SetAsLastSibling();
        return go;
    }

    private void SetAllSkillBtnState(bool flag)
    {
        InputManager.instance.joystick.activated = flag;
        InputManager.instance.EnableSkillButton(flag);
        GetSkillBtn(9990).activated = flag;
    }

    private void DestroyEffectObj()
    {
        if (effectObj != null)
        {
            GameObject.Destroy(effectObj);
            effectObj = null;
        }
    }
    private void DestroyTip()
    {
        if (tip != null)
        {
            GameObject.Destroy(tip);
            tip = null;
        }
    }
    #endregion
}
