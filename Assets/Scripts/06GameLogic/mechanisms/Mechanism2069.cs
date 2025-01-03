using Protocol;
using System;
using System.Collections.Generic;
using GameClient;
using UnityEngine;
public class Mechanism2069 : BeMechanism
{
    public Mechanism2069(int mid, int lv) : base(mid, lv) { }
    List<int> monsterIds = new List<int>();
    DelayCallUnitHandle delayHandle;
    string[] sceneName = new string[]
    {
        "1haidi",
        "2meimeng",
        "3emeng"
    };
    VInt3[] monsterBornPos = { new VInt3(15869, 67344, 0), new VInt3(17049, 67783, 0), new VInt3(28400, 200300, 0) };
    int curSceneIndex = 0;
    bool bInited = false;
    int procedureMonsterId = 80620011;
    public override void OnInit()
    {
        monsterIds.Clear();
        if (data.ValueA.Count > 0)
        {
            for (int i = 0; i < data.ValueA.Length; i++)
            {
                monsterIds.Add(TableManager.GetValueFromUnionCell(data.ValueA[i], level));
            }
        }
        int tempMonsterId = TableManager.GetValueFromUnionCell(data.ValueB[0], level);
        if (tempMonsterId != 0)
        {
            procedureMonsterId = tempMonsterId;
        }
    }
    private int preTime = 0;

    public override void OnReset()
    {
        delayHandle.SetRemove(true);
        curSceneIndex = 0;
        bInited = false;
        preTime = 0;
    }

    public override void OnUpdate(int deltaTime)
    {
        base.OnUpdate(deltaTime);
        if(!bInited)
        {
            //for (int i = 0; i < monsterIds.Count; i++)
            //{
            //    var curMonster = owner.CurrentBeScene.FindMonsterByID(monsterIds[i]);
            //    if (curMonster != null && curMonster.stateController != null)
            //    {
            //        curMonster.stateController.SetAbilityEnable(BeAbilityType.BEHIT, false);
            //        curMonster.stateController.SetAbilityEnable(BeAbilityType.BETARGETED, false);
            //        curMonster.stateController.SetAbilityEnable(BeAbilityType.ATTACK, false);
            //        curMonster.stateController.SetAbilityEnable(BeAbilityType.MOVE, false);
            //        curMonster.m_pkGeActor.HideActor(true);
            //        var pos = curMonster.GetPosition();
            //        Logger.LogErrorFormat("pos {0}", pos);
            //    }
            //}
            var procedureMonster = owner.CurrentBeScene.FindMonsterByID(procedureMonsterId);
#if !LOGIC_SERVER
            if (procedureMonster != null && procedureMonster.m_pkGeActor != null)
            {
                procedureMonster.m_pkGeActor.HideActor(true);
            }
#endif
            CreateBossPhaseChange(1);
            bInited = true;
        }
    }
    public override void OnUpdateGraphic(int deltaTime)
    {
#if !LOGIC_SERVER
        if (ReplayServer.GetInstance() != null && ReplayServer.GetInstance().IsReplay()) return;
        if (bInited && curSceneIndex >= 0 && curSceneIndex < 3 &&
            owner != null && owner.CurrentBeBattle != null && owner.CurrentBeBattle.dungeonManager != null &&
            !owner.CurrentBeBattle.dungeonManager.IsFinishFight())
        {
            if (FrameSync.instance != null)
            {
                preTime += deltaTime;
                if (preTime >= 2880) 
                {
                    preTime = 0;
                    TeamCopyPhaseBossInfo req = new TeamCopyPhaseBossInfo();
                    if (ClientApplication.playerinfo != null)
                    {
                        req.raceId = ClientApplication.playerinfo.session;
                    }
                    if (owner != null && owner.CurrentBeBattle != null && owner.CurrentBeBattle.dungeonManager != null)
                    {
                        var mainPlayer = owner.CurrentBeBattle.dungeonPlayerManager.GetMainPlayer();
                        if (mainPlayer != null)
                        {
                            req.roleId = mainPlayer.playerInfo.roleId;
                        }
                    }
                    req.curFrame = FrameSync.instance.curFrame;
                    req.phase = (uint)curSceneIndex + 1;
                    var monster = owner.CurrentBeScene.FindMonsterByID(monsterIds[curSceneIndex]);
                    if (monster != null)
                    {
                        VFactor bossHpRate = new VFactor(monster.attribute.GetHP() * 100, monster.attribute.GetMaxHP());
                        req.bossBloodRate = (uint)bossHpRate.single;
                        Network.NetManager.Instance().SendCommand(Network.ServerType.GATE_SERVER, req);
                    }
                }
            }
        }
#endif
    }

    public void CreateBossPhaseChange(int phaseIndex)
    {
#if !LOGIC_SERVER
        if (FrameSync.instance != null)
        {
            GameClient.BossPhaseChange cmd = new GameClient.BossPhaseChange
            {
                phase = phaseIndex,
            };

            FrameSync.instance.FireFrameCommand(cmd, true);
        }
        
#endif
        if (owner.GetRecordServer() != null && owner.IsProcessRecord())
        {
            owner.GetRecordServer().RecordProcess(string.Format("BOSS阶段上报 阶段:{0}", phaseIndex));
            owner.GetRecordServer().MarkInt(0x7777777, phaseIndex);
            // Mark:0x7777777 BOSS阶段上报 阶段:{0}
        }
    }
    public override void OnStart()
    {
        base.OnStart();
        if (owner == null || owner.CurrentBeScene == null) return;
        handleA = owner.CurrentBeScene.RegisterEventNew(BeEventSceneType.onMonsterDead,onMonsterDead);
    }
    public override void OnFinish()
    {
        base.OnFinish();
        delayHandle.SetRemove(true);
    }
    void onMonsterDead(BeEvent.BeEventParam args)
    {
        if (owner == null || owner.CurrentBeScene == null) return;
        var monster = args.m_Obj as BeActor;
        if (monster == null) return;
        for(int i = 0; i < monsterIds.Count;i++)
        {
            if(monster.GetEntityData() == null )
            {
                return;
            }
            if (!monster.GetEntityData().MonsterIDEqual(monsterIds[i])) continue;

            if (curSceneIndex + 1 >= sceneName.Length)
            {
                var procedureMonster = owner.CurrentBeScene.FindMonsterByID(procedureMonsterId);
                if(procedureMonster != null)
                {
                    procedureMonster.DoDead();
                }
                return;
            }

#if !LOGIC_SERVER
            if (owner.CurrentBeScene.currentGeScene != null)
            {
                var sgo = owner.CurrentBeScene.currentGeScene.GetSceneObject();
                if (sgo != null && sgo.transform != null)
                {
                    var curScene = sgo.transform.Find(sceneName[curSceneIndex]);
                    if (curScene != null && curScene.gameObject != null) 
                    {
                        curScene.gameObject.CustomActive(false);
                    }
                }
            }
#endif
            curSceneIndex++;
            if (curSceneIndex >= sceneName.Length)
            {
                return;
            }

            CreateBossPhaseChange(curSceneIndex + 1);
#if !LOGIC_SERVER
            if (owner.CurrentBeScene.currentGeScene != null)
            {
                if (curSceneIndex == 1)
                {
                    owner.CurrentBeScene.currentGeScene.CreateEffect(1022, Vec3.zero);

                    if (owner.CurrentBeBattle != null)
                        owner.CurrentBeBattle.PlaySound(5004);
                    if(!owner.CurrentBeBattle.HasFlag(GameClient.BattleFlagType.BossTranform_Disable_GC))
                    {
                        AssetGabageCollector.instance.ClearUnusedAsset();
                    }
                }
                else if (curSceneIndex == 2)
                {
                    owner.CurrentBeScene.currentGeScene.CreateEffect(1023, Vec3.zero);
                    
                    if (owner.CurrentBeBattle != null)
                        owner.CurrentBeBattle.PlaySound(5003);
                    if (!owner.CurrentBeBattle.HasFlag(GameClient.BattleFlagType.BossTranform_Disable_GC))
                    {
                        AssetGabageCollector.instance.ClearUnusedAsset();
                    }
                }
            }
            if(monster.m_pkGeActor != null)
                monster.m_pkGeActor.HideActor(true);
#endif
            
            delayHandle = owner.CurrentBeScene.DelayCaller.DelayCall(1500, () =>
            {
                if (owner == null || owner.CurrentBeScene == null) return;
#if !LOGIC_SERVER
                if (owner.CurrentBeScene.currentGeScene != null)
                {
                    var sgo = owner.CurrentBeScene.currentGeScene.GetSceneObject();
                    if (sgo != null && sgo.transform != null)
                    {
                        var nextScene = sgo.transform.Find(sceneName[curSceneIndex]);
                        if (nextScene != null && nextScene.gameObject != null)
                        {
                            nextScene.gameObject.CustomActive(true);
                        }
                    }
                }
#endif
                var boss = owner.CurrentBeScene.CreateMonster(monsterIds[curSceneIndex] + owner.GetEntityData().GetLevel() * 100, monsterBornPos[curSceneIndex]);
                if(boss != null)
                {
                    //boss.SetPosition(monsterBornPos[curSceneIndex]);
                    boss.StartAI(null, false);
                }
                //                var nextMonster = owner.CurrentBeScene.FindMonsterByID(monsterIds[curSceneIndex]);
                //                if (nextMonster != null && nextMonster.stateController != null)
                //                {
                //                    //if (curSceneIndex + 1 >= monsterIds.Count)
                //                    //{
                //                    //    nextMonster.stateController.SetAbilityEnable(BeAbilityType.BEHIT,true);
                //                    //    nextMonster.stateController.SetAbilityEnable(BeAbilityType.BETARGETED,true);
                //                    //    nextMonster.stateController.SetAbilityEnable(BeAbilityType.ATTACK, true);

                //                    //}
                //                    //else
                //                    //{
                //                    //    nextMonster.stateController.SetAbilityEnable(BeAbilityType.BEHIT, true);
                //                    //    nextMonster.stateController.SetAbilityEnable(BeAbilityType.BETARGETED, true);
                //                    //    nextMonster.stateController.SetAbilityEnable(BeAbilityType.ATTACK, true);
                //                    //    nextMonster.stateController.SetAbilityEnable(BeAbilityType.MOVE, true);
                //                    //}
                //#if !LOGIC_SERVER
                //                    if(nextMonster.m_pkGeActor != null)
                //                        nextMonster.m_pkGeActor.HideActor(false);
                //#endif
                //                }
            });
        }
    }
}

