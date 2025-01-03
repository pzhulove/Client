
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using behaviac;
using GameClient;
using UnityEngine;

public class BattlePreview : Singleton<BattlePreview>
{
    public enum BattlePreviewState
    {
        NOT_START,
        INIT,
        CREATED,
        STOPTING
    }

    private BattlePreviewState mState;
    public BattlePreviewState state
    {
        private set
        {
            Logger.LogProcessFormat("[PREVIEW]state = {0}", value.ToString());
            mState = value;
        }
        get { return mState; }
    }

    DemoBattle battle = null;
    BeActor PlayerActor
    {
        get
        {
            if (battle != null)
            {
                var players = battle.dungeonPlayerManager;
                if (players != null)
                {
                    var player = players.GetMainPlayer();
                    if (player != null)
                    {
                        return player.playerActor;
                    }
                }
            }
            return null;
        }
    }
    
    BeScene CurrentBeScene
    {
        get
        {
            if (battle != null)
            {
                return battle.dungeonManager.GetBeScene();
            }
            return null;
        }
    }
    
    GeCamera BattleCamera
    {
        get { return CurrentBeScene?.currentGeScene?.GetCamera(); }
    }
    IEnumerator createCorutineID = null;


    private enum PreConditionType
    {
        AddChaser = 1,        // 添加炫纹
        AddRune = 2            // 添加刻印
    }
    
    private int m_LoopTimeAcc = 0;    
    public AIInputData m_CurSkillInputData = new AIInputData();
    public ProtoTable.SkillInputTable m_CurSkillConfig;
    private int m_CurSkillId;
    private int CurSkillId
    {
        get { return m_CurSkillId; }
        set
        {
            m_CurSkillId = value;
            m_CurSkillConfig = TableManager.instance.GetTableItem<ProtoTable.SkillInputTable>(value);
            if (m_CurSkillConfig == null)
            {
                UnityEngine.Debug.LogErrorFormat("在技能预览表中未找到技能：{0}", value);
            }
            ParseInputData(m_CurSkillConfig);
        }
    }

    Dictionary<int, List<int>> mAllOwnerSkillIds = new Dictionary<int, List<int>>();
    
    public override void Init()
    {
        base.Init();

        state = BattlePreviewState.NOT_START;
    }

    public void Start(object rt, int skillId)
    {
        if (state == BattlePreviewState.CREATED || state == BattlePreviewState.INIT)
            return;

        state = BattlePreviewState.INIT;
        CurSkillId = skillId;

        var town = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
        if (town == null)
            return;

        var battleMain = BattleMain.OpenBattle(BattleType.Demo, eDungeonMode.LocalFrame, 0, "");
        battle = battleMain.GetBattle() as DemoBattle;
        InputManager.isForceLock = true;
        if (town.Scene != null)
        {
            town.Scene.SetActive(false);
        }
        
        GameFrameWork.instance.SetMainCamera(true);
        GameFrameWork.instance.SetMainCameraRenderTexture(rt as RenderTexture);

        createCorutineID = CreateBattle();
        GameFrameWork.instance.StartCoroutine(createCorutineID);
    }

    public void Stop()
    {
        if (state == BattlePreviewState.NOT_START)
            return;

        if (state == BattlePreviewState.STOPTING)
            return;

        if (state == BattlePreviewState.INIT)
        {
            if (createCorutineID != null)
            {
                GameFrameWork.instance.StopCoroutine(createCorutineID);
                createCorutineID = null;

                Logger.LogErrorFormat("异常:预览还没有创建就进入stop了，这里有可能还在进行create！！！！");
            }
        }

        state = BattlePreviewState.STOPTING;

        InputManager.isForceLock = false;
        GameFrameWork.instance.SetMainCameraRenderTexture(null);

        var town = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
        if (town == null)
            return;

        DestroyBattle();
        
        if (town.Scene != null)
        {
            town.Scene._updateGeCamera();
           // town.Scene.GetCamera().GetController().ResetCamera();
            town.Scene.SetActive(true);
            town._InitializeCameraController();

            town.ResetAction();

          //  if (town.MainPlayer != null && town.MainPlayer.GraphicActor != null)
           ///     town.Scene.AttachCameraTo(town.MainPlayer.GraphicActor);
        }
    }

    public void Update(float time)
    {
        if (state == BattlePreviewState.CREATED)
        {
            int deltaTime = (int) (time * 1000);
            FrameSync.instance.UpdateFrame();
            
            if (battle != null)
            {
                battle.Update(deltaTime);
            }
            
            UpdateLoopPreview(deltaTime);
        }
    }

    private bool m_isSkillFinish = false;
    private void UpdateLoopPreview(int deltaTime)
    {
        if (m_CurSkillConfig == null)
            return;

        var actor = PlayerActor;
        if (actor != null)
        {
            if (actor.sgGetCurrentState() == (int) ActionState.AS_IDLE && m_isSkillFinish)
            {
                m_LoopTimeAcc += deltaTime;
            }
        }
        if (m_LoopTimeAcc >= m_CurSkillConfig.LoopTime)
        {
            UseSkill(CurSkillId);
        }
    }
    
    public void UseSkill(int skillID)
    {
        if(!CheckSkillPreview(skillID))
           return;

        m_isSkillFinish = false;
        m_LoopTimeAcc = 0;
        CurSkillId = skillID;
        ResetBattle();
        DealPreCondition();
        PlayActorSkill();    
    }

    private bool CheckSkillPreview(int skillId)
    {
        if (state != BattlePreviewState.CREATED)
            return false;
        if (skillId <= 0)
            return false;
        var skillTable = TableManager.instance.GetTableItem<ProtoTable.SkillTable>(skillId);
        
        if (skillTable != null && skillTable.SkillType != ProtoTable.SkillTable.eSkillType.ACTIVE)
        {
            return false;
        }

        return true;
    }

    private void DealPreCondition()
    {
        if (m_CurSkillConfig == null)
            return;
        var condition = m_CurSkillConfig.PreCondition;
        if(condition.Count == 0 || condition[0] == 0)
            return;

        var actor = PlayerActor;
        if(actor == null)
            return;
        
        PreConditionType kind = (PreConditionType) condition[0];
        switch (kind)
        {
            case PreConditionType.AddChaser:
                if (condition.Count >= 2)
                    AddChaser(actor, condition[1]);
                break;
            case PreConditionType.AddRune:
                if (condition.Count >= 2)
                    AddRune(actor, condition[1]);
                break;
        }
    }

    private void AddChaser(BeActor actor, int count)
    {
        if (count <= 0)
            return;
        
        var mgr = actor.GetMechanism<Mechanism2072>(Mechanism2072.ChaserMgrID);
        if(mgr == null)
            return;
        for (int i = 0; i < count; i++)
        {
            mgr.AddChaser(Mechanism2072.ChaserType.Fire, Mechanism2072.ChaseSizeType.Normal);    
        }
    }

    private void AddRune(BeActor actor, int count)
    {
        if (count <= 0)
            return;
        var mgr = actor.GetMechanism<Mechanism22>(1001);
        if(mgr == null)
            return;
        for (int i = 0; i < count; i++)
        {
            mgr.AddRune();
        }
    }
    private void PlayActorSkill()
    {
        var actor = PlayerActor;
        if(actor == null)
            return;

        var ai = actor.aiManager as BeActorAIManager;
        if(ai == null)
            return;

       
        //if (m_CurSkillConfig != null)
        {
            ParseInputData(m_CurSkillConfig);
            ai.DoAction(m_CurSkillInputData);    
        }
    }
    
    
    protected void ParseInputData(ProtoTable.SkillInputTable config)
    {
        m_CurSkillInputData.inputs.Clear();

        var skillIds = GamePool.ListPool<int>.Get();

        var data = config;
        if (data == null)
        {
            if (!skillIds.Contains(CurSkillId))
            {
                skillIds.Add(CurSkillId);
            }

            m_CurSkillInputData.AddInput(CurSkillId, 100);
        }
        else
        {
            var list = GamePool.ListPool<ProtoTable.FlatBufferArray<int>>.Get();
            list.Add(data.Input1);
            list.Add(data.Input2);
            list.Add(data.Input3);
            list.Add(data.Input4);
            list.Add(data.Input5);

            for (var i = 0; i < list.Count; i++)
            {
                var item = list[i];

                behaviac.Input configItem = ParseInputConfigItem(item);
                if (configItem.skillID > 0)
                {
                    var id = configItem.skillID;
                    if (id >= 1000 && !skillIds.Contains(id))
                    {
                        skillIds.Add(id);
                    }

                    m_CurSkillInputData.AddInput(configItem);
                    if (configItem.pressTime > 0)
                    {
                        m_CurSkillInputData.AddInput(configItem.skillID, configItem.pressTime);
                    }
                }
            }

            GamePool.ListPool<ProtoTable.FlatBufferArray<int>>.Release(list);
        }

        if (skillIds.Count > 0)
        {
            var childSkillIds = GamePool.ListPool<int>.Get();

            foreach (var item in skillIds)
            {
                if (mAllOwnerSkillIds.ContainsKey(item))
                {
                    var ids = mAllOwnerSkillIds[item];
                    foreach (var id0 in ids)
                    {
                        if (!skillIds.Contains(id0) && !childSkillIds.Contains(id0))
                        {
                            childSkillIds.Add(id0);
                        }
                    }
                }
            }

            skillIds.AddRange(childSkillIds);

            GamePool.ListPool<int>.Release(childSkillIds);
        }

        if (skillIds.Count > 0)
        {
            foreach (var item in skillIds)
            {
                var skillID = item;
                if (PlayerActor != null && !PlayerActor.HasSkill(skillID))
                    PlayerActor.skillController.LoadOneSkillAndConfig(skillID, 1, PlayerActor.m_iResID);
            }
            if (PlayerActor != null)
                PlayerActor.skillController.PostInitSkills();
        }

        GamePool.ListPool<int>.Release(skillIds);
    }

    private behaviac.Input ParseInputConfigItem(ProtoTable.FlatBufferArray<int> item)
    {
        var data = new behaviac.Input();
        for (int i = 0; i < item.Count; i++)
        {
            int v = item[i];
            switch (i)
            {
                case 0:
                    data.skillID = v;
                    break;
                case 1:
                    data.specialChoice = v;
                    break;
                case 2:
                    data.pressTime = v;
                    break;
                case 3:
                    data.delay = v;
                    break;
            }
        }
        return data;
    }
    private void DestroyBattle()
    {
        if (battle != null)
        {
            BattleMain.CloseBattle(false);
            CGameObjectPool.instance.ClearAll();
            battle = null;
        }

        state = BattlePreviewState.NOT_START;



    }

    private IEnumerator CreateBattle()
    {
        yield return battle.Start(null);
        state = BattlePreviewState.CREATED;
        createCorutineID = null;

        var player = PlayerActor;
        if (player != null)
        {
            player.RegisterEventNew(BeEventType.onCastSkillFinish, param => m_isSkillFinish = true);
            while (player.sgGetCurrentState() == (int) ActionState.AS_BIRTH) { yield return null; }
        }
        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.BattlePreviewCreateFinish);
        UseSkill(CurSkillId);
    }

    private void ResetBattle()
    {
        ResetCamera();
        ResetPlayer(PlayerActor);
        ResetScene();
        ResetMonster();
    }

    private void ResetCamera()
    {
        if (BattleCamera == null)
            return;

        //BattleCamera.SetOrthographicSize();
    }

    private void ResetPlayer(BeActor actor)
    {
        if (actor == null)
            return;
        
        actor.SetFace(false, true);
        VInt3 pos = new VInt3(-1f, 0, 0);
        if (m_CurSkillConfig != null)
        {
            if (m_CurSkillConfig.PositionOffset.Count == 2)
            {
                pos = new VInt3((float)m_CurSkillConfig.PositionOffset[0] / GlobalLogic.VALUE_1000, (float)m_CurSkillConfig.PositionOffset[1] / GlobalLogic.VALUE_1000, 0);
            }

        }
        actor.SetPosition(pos, true);    
        actor.buffController.RemoveAllBuff();
        actor.Locomote(new BeStateData((int)ActionState.AS_IDLE), true);
        actor.ResetSkillCoolDown();
        actor.RemoveAllMechanism();
        actor.ClearMoveSpeed();
        actor.m_pkGeActor?.Clear();
        if(actor.sgGetCurrentState() != (int) ActionState.AS_IDLE)
            actor.sgForceSwitchState(new BeStateData((int) ActionState.AS_IDLE));
    }

    private void ResetScene()
    {
        var scene = CurrentBeScene;
        if (scene == null)
            return;
#if !LOGIC_SERVER
        scene.currentGeScene?.DestroyAllEffect();
#endif
        scene.RemoveAllActorWithout(PlayerActor);
        scene.RemoveAllProjectiles();
    }

    struct MonsterInfo
    {
        public int id;
        public VInt3 offsetPos;
        public bool faceLeft;
        public int skillId;
        public int skillDelay;
    }
    void ResetMonster()
    {
        if (m_CurSkillConfig == null)
            return;

        var scene = CurrentBeScene;
        if(scene == null)
            return;
                
        var list = GamePool.ListPool<ProtoTable.FlatBufferArray<int>>.Get();
        list.Add(m_CurSkillConfig.MonsterInfo1);
        list.Add(m_CurSkillConfig.MonsterInfo2);
        list.Add(m_CurSkillConfig.MonsterInfo3);
        list.Add(m_CurSkillConfig.MonsterInfo4);
        list.Add(m_CurSkillConfig.MonsterInfo5);
        list.Add(m_CurSkillConfig.MonsterInfo6);

        for (var i = 0; i < list.Count; i++)
        {
            var item = list[i];

            MonsterInfo monsterInfo = ParseMonsterInfo(item);
            if(monsterInfo.id <= 0)
                continue;

            var monster = scene.CreateMonster(monsterInfo.id);
            if (monster == null)
            {
                Logger.LogErrorFormat("无法创建 怪物ID：{0}， 请检查表 技能ID：{}", monsterInfo.id, CurSkillId);
                continue;
            }
            monster.m_pkGeActor.SetFootIndicatorVisible(false);
            monster.m_pkGeActor.SetHeadInfoVisible(false);
            monster.m_pkGeActor.RemoveHPBar();
            VInt3 offset = new VInt3((float)monsterInfo.offsetPos.x / GlobalLogic.VALUE_1000, (float)monsterInfo.offsetPos.y / GlobalLogic.VALUE_1000, 0);
            monster.SetPosition(offset, true);
            monster.SetFace(monsterInfo.faceLeft, true);
            monster.GetEntityData().SetMaxHP(int.MaxValue);
            monster.GetEntityData().SetHP(int.MaxValue);
            monster.pauseAI = true;
            if (monsterInfo.skillId > 0)
            {
                if (monsterInfo.skillDelay > 0)
                {
                    monster.delayCaller.DelayCall(monsterInfo.skillDelay, () => monster.UseSkill(monsterInfo.skillId, true));    
                }
                else
                {
                    monster.UseSkill(monsterInfo.skillId, true);
                }
                
            }
            
        }
        GamePool.ListPool<ProtoTable.FlatBufferArray<int>>.Release(list);
    }


    private MonsterInfo ParseMonsterInfo(ProtoTable.FlatBufferArray<int> info)
    {
        MonsterInfo ret = new MonsterInfo();
        for (int i = 0; i < info.Count; i++)
        {
            int v = info[i];
            switch (i)
            {
                case 0 :
                    ret.id = v;
                    break;
                case 1 :
                    ret.offsetPos.x = v;
                    break;
                case 2 :
                    ret.offsetPos.y = v;
                    break;
                case 3 :
                    ret.faceLeft = v == 0;
                    break;
                case 4 :
                    ret.skillId = v;
                    break;
                case 5 :
                    ret.skillDelay = v;
                    break;
            }
        }

        return ret;
    }

    void InitOwnerSkillIds()
    {
        if (mAllOwnerSkillIds.Count > 0)
        {
            return;
        }

        var table = TableManager.GetInstance().GetTable<ProtoTable.SkillTable>();

        foreach(var item in table)
        {
            int skillId = item.Key;
            var data = item.Value as ProtoTable.SkillTable;
            int ownerSkillId = data.MasterSkillID;
            if (ownerSkillId <= 0)
            {
                continue;
            }

            if (!mAllOwnerSkillIds.ContainsKey(ownerSkillId))
            {
                mAllOwnerSkillIds.Add(ownerSkillId, new List<int>());
            }
            if (mAllOwnerSkillIds.ContainsKey(ownerSkillId))
            {
                mAllOwnerSkillIds[ownerSkillId].Add(skillId);
            }
        }
    }
}