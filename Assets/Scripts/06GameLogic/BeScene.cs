using System;
using System.IO;
using System.Collections.Generic;
using Protocol;
using System.ComponentModel;
using GameClient;
using UnityEngine;
using Tenmove.Runtime.Unity;

public enum BeSceneState
{
    onNone = 0,
    onPause,
    onCreate,

    onReady,
    onFight,
    onClear,
    onBulletTime,
    onFinish,
}

public enum LevelDifficulty
{
    /// <summary>
    /// 简单
    /// </summary>
    NORMAL = 1,
    /// <summary>
    /// 冒险
    /// </summary>
    ADVANTURE,
    /// <summary>
    /// 勇士
    /// </summary>
    WARRIOR,
    /// <summary>
    /// 王者
    /// </summary>
    KING,
    COUNT
}

public enum MonsterAttributeType
{
    /// <summary>
    /// 普通
    /// </summary>
    NORMAL = 1,
    /// <summary>
    /// 精英
    /// </summary>
    ELITE,
    /// <summary>
    /// BOSS
    /// </summary>
    BOSS,

    HELL = 5,//深渊
    ACTIVITY = 6,//远古
    /// <summary>
    /// 随从
    /// </summary>
	ACCOMPANY = 7,
    COUNT
}

public enum MonsterMode
{
    NORMAL = 1, //普通
    HELL,       //深渊
    ANCIENT,        //远古
    DEADTOWER,
    SUMMON_PVE, //召唤兽pve
    SUMMON_PVP, //召唤兽pvp
    COUNT
}



public class MonsterIDData
{
    public int mid;
    public int level;
    public int type;
    public int difficulty;

    public MonsterIDData(int oldId)
    {
        if (oldId.ToString().Length < 7)
        {
            mid = oldId;
            level = 1;
            type = 1;
            difficulty = 1;
        }
        else
        {
            int sid = oldId;

            level = sid % GlobalLogic.VALUE_10000 / GlobalLogic.VALUE_100;
            type = sid % GlobalLogic.VALUE_100 / GlobalLogic.VALUE_10;
            difficulty = sid % GlobalLogic.VALUE_10;


            mid = oldId - level * GlobalLogic.VALUE_100;

            //Logger.LogErrorFormat("mid:{0}, {1},{2},{3},{4}", oldId, mid, level, type, difficulty);

            level = Math.Max(1, level);

        }
    }

    public int GenMonsterID(int level)
    {
        this.level = level;

        return mid += level * GlobalLogic.VALUE_100;
    }
    public void SetID(int id)
    {
        _decodeID(id);
    }
    private void _decodeID(int id)
    {
        var tid = id;
        difficulty = tid % GlobalLogic.VALUE_10;
        tid /= GlobalLogic.VALUE_10;

        type = tid % GlobalLogic.VALUE_10;
        tid /= GlobalLogic.VALUE_10;

        level = tid % GlobalLogic.VALUE_100;
        tid /= GlobalLogic.VALUE_100;

        mid = tid;
    }
    public int GenFullMonsterID(int level)
    {
        int resid = mid;
        resid *= GlobalLogic.VALUE_100;
        resid += level % GlobalLogic.VALUE_100;

        resid *= GlobalLogic.VALUE_10;
        resid += type % GlobalLogic.VALUE_10;

        resid *= GlobalLogic.VALUE_10;
        resid += difficulty % GlobalLogic.VALUE_10;
        this.level = level;
        return resid;
    }
}

/// <summary>
/// 单个房间的管理
/// </summary>
[LoggerModel("Chapter")]
public class BeScene : BeStateManager<BeSceneState>
{
    protected ISceneData mSceneData;
    public int logicWidth;
    public int logicHeight;
    public VInt2 logicZSize;
    public VInt2 logicXSize;
    public VInt2 logicGrild;

    public byte[] mBlockInfo = null;
    public byte[] mEventAreaInfo = null;

    private bool mIsBossScene;
    private bool mIsDoorOpened;
    private bool isTraceDoor = true;

    //add by wangchong, 首领预告
    public bool isUseBossShow = true;

    protected List<BeEntity> mEntitys = new List<BeEntity>();
    protected List<BeEntity> mPendingArray = new List<BeEntity>();
    protected Dictionary<int, BeEntity> mEntitysMap = new Dictionary<int, BeEntity>();

    protected List<BeEntity> mDeadBody = new List<BeEntity>();
    protected List<BeEntity> mTempSaveEntitys = new List<BeEntity>();

    protected List<BeRegionBase> mRegions = new List<BeRegionBase>();
    protected List<BeRegionBase> mPendingRegion = new List<BeRegionBase>();
    private List<BeRegionBase> mDoorList = new List<BeRegionBase>();

    protected DelayCaller mDelayCaller = new DelayCaller();

    protected GeSceneEx mCurrentGeScene;

    protected List<GeActorEx> mDecoratorList = new List<GeActorEx>();

    protected bool mIsTickAI = true;

    protected IList<int> taskMonsters = null;

    protected BeRegionBase traceDoor = null;
    protected VInt3 traceDoorPos = VInt3.zero;

    bool isBlind = false;
    bool bossDead = false;

    protected bool isBattleDataError = false;
    protected int checkBattleDataErrorAcc = 0;

    SimpleTimer2 simpleTimer = new SimpleTimer2();
    public SimpleTimer pkTimer = null;
#if LOGIC_SERVER
    protected BeActionFrameMgr mActionFrameMgr = new BeActionFrameMgr();
    protected SkillFileListCache mSkillFileCache = new SkillFileListCache();
#endif

    protected BeEventManager m_EventManager;

    public BaseBattle mBattle
    {
        get; set;
    }

    public FrameRandomImp FrameRandom
    {
        get
        {
            return mBattle.FrameRandom;
        }
    }

    public TrailManagerImp TrailManager
    {
        get
        {
            return mBattle.TrailManager;
        }
    }

    public BeProjectilePoolImp BeProjectilePool
    {
        get
        {
            return mBattle.BeProjectilePool;
        }
    }

    public BeActionFrameMgr ActionFrameMgr
    {
        get
        {
#if LOGIC_SERVER
            return mActionFrameMgr;
#else
            return null;
#endif
        }
    }
    public SkillFileListCache SkillFileCache
    {
        get
        {
#if LOGIC_SERVER
            return mSkillFileCache;
#else
            return null;
#endif
        }
    }

    public BeAICommandPoolImp BeAICommandPool
    {
        get
        {
            return mBattle.BeAICommandPool;
        }
    }

    public BeBuffPoolImp BeBuffPool
    {
        get {return mBattle.BuffPool;}
    }

    public BeMechanismPoolImp MechanismPool
    {
        get {return mBattle.MechanismPool;}
    }

    public bool IsBossDead()
    {
        return mIsBossScene && bossDead;
    }

    public int singleBloodBarCount
    {
        set; get;
    }
    protected int mDurtime = 0;
    public int GameTime { get { return mDurtime; } }
    #region Getter && Setter
    public bool isTickAI
    {
        set
        {
            mIsTickAI = value;
        }
    }

    public DelayCaller DelayCaller
    {
        get
        {
            if (mDelayCaller == null)
            {
                Logger.LogError("delayCaller is nil");
            }
            return mDelayCaller;
        }
    }

    public ISceneData sceneData
    {
        get
        {
            return mSceneData;
        }
    }

    public GeSceneEx currentGeScene
    {
        get
        {
            if (mCurrentGeScene == null)
            {
                Logger.LogError("currentGeScene is nil");
            }
            return mCurrentGeScene;
        }
    }
    #endregion

    protected BeScene()
    {
        state = BeSceneState.onCreate;

        mIsBossScene = false;
        mIsDoorOpened = false;

        checkAcc = 0;

        mPrePauseState = BeSceneState.onNone;

        _initSceneExData();
    }

    public bool IsShowTransportDoorCount()
    {
        return simpleTimer.IsCount();
    }

    int mCurTransportTimerID = 0;
    public int curTransportTimerId { get { return mCurTransportTimerID; } }
    public void StopTransportDoorCount(int timerId = -1)
    {
        if (timerId != -1 && timerId != mCurTransportTimerID) return;
        simpleTimer.StopTimer();
        var battleUI = BattleUIHelper.GetBattleUIComponent<BattleUIPve>();
        if (battleUI != null)
            battleUI.ShowDigit(false);
    }

    public void StartTransportDoorCount(int countSecond, SimpleTimer2.TimeUpCallBack timeupCallback)
    {
        var battleUI = BattleUIHelper.GetBattleUIComponent<BattleUIPve>();
        if (battleUI != null)
        {
            battleUI.ShowDigit(true);
            battleUI.SetCountDigit(countSecond);
        }
        mCurTransportTimerID++;
        simpleTimer.SetCountdown(countSecond);
        simpleTimer.secondCallBack = (int second) =>
        {
	        if (battleUI != null)
			{
                battleUI.SetCountDigit(countSecond - second);
				// Logger.LogErrorFormat("show dight:{0} time:{1}", (countSecond - second), Time.realtimeSinceStartup);
			}
        };

        simpleTimer.timeupCallBack = () =>
        {
            timeupCallback();
#if !SERVER_LOGIC

            if (battleUI != null)
                battleUI.ShowDigit(false);

#endif

        };

        simpleTimer.StartTimer();
    }

    public void StartPKCountDown(int countSecond, SimpleTimer2.TimeUpCallBack timeupCallback)
    {
        //simpleTimer.mR = mBattle.recordServer;
        simpleTimer.timeupCallBack = timeupCallback;
        simpleTimer.SetCountdown(countSecond);
        simpleTimer.StartTimer();
    }

    public void SetTraceDoor(bool flag)
    {
        isTraceDoor = flag;
    }

    public void Reset()
    {
        _initSceneExData();
    }

    private void _initSceneExData()
    {
        // 单条血条最大值
        singleBloodBarCount = -1;
    }

    #region Event



    public bool IsBossSceneClear()
    {
        return mIsBossScene && state >= BeSceneState.onClear;
    }

    private void _onClearEvent()
    {
        if (mIsBossScene == false && null == mHell)
        {
            _setTransportDoor(true);
        }

        state = BeSceneState.onClear;

        Logger.LogProcessFormat("Trigger the onClear event");
        TriggerEventNew(BeEventSceneType.onClear);

        traceDoor = null;
        traceDoorPos = VInt3.zero;
        if (!mIsBossScene && isTraceDoor)
            ChooseADoorToChance();
    }

    #endregion

    #region 新的事件管理机制

    public void UnRegisterAll()
    {
        ClearEventAllNew();
    }
    
    /// <summary>
    /// 注册事件
    /// </summary>
    /// <param name="type">事件类型</param>
    /// <param name="function">事件回调函数</param>
    public BeEvent.BeEventHandleNew RegisterEventNew(BeEventSceneType eventType, BeEvent.BeEventHandleNew.Function function)
    {
        if (m_EventManager == null)
        {
            m_EventManager = new BeEventManager(0);
        }
        return m_EventManager.RegisterEvent((int)eventType, function);
    }

    /// <summary>
    /// 触发事件(New)
    /// </summary>
    /// 使用示例
    public EventParam TriggerEventNew(BeEventSceneType eventType, EventParam eventParam = new EventParam())
    {
        if (m_EventManager == null)
            return eventParam;
        return m_EventManager.TriggerEvent((int)eventType, eventParam);
    }
    
    /// <summary>
    /// 清除所有事件
    /// </summary>
    public void ClearEventAllNew()
    {
        if (m_EventManager == null)
            return;
        m_EventManager.ClearAll();
    }

    public void RemoveEventNew(BeEvent.BeEventHandleNew handle)
    {
        if (m_EventManager == null)
            return;
        m_EventManager.RemoveHandle(handle);
    }

    public void UnRegisterEventNew(BeEventSceneType type, BeEvent.BeEventHandleNew.Function del)
    {
        if (m_EventManager == null)
            return;
        m_EventManager.RemoveHandle((int) type, del);
    }

    public void RemoveHandle(IBeEventHandle handle)
    {
        if (null == handle)
        {
            return;
        }

        RemoveEventNew(handle as BeEvent.BeEventHandleNew);
    }

    #endregion

    #region State

    public void LoadTaskMonsters(int taskID)
    {
        if (taskID <= 0)
            return;

        var taskData = TableManager.GetInstance().GetTableItem<ProtoTable.MissionTable>(taskID);
        if (taskData != null)
        {
            taskMonsters = taskData.MonsterIDs;
        }
    }

    public bool IsTaskMonster(int monsterID)
    {
        if (taskMonsters != null && monsterID > 0)
        {
            return taskMonsters.Contains(BeUtility.GetOnlyMonsterID(monsterID));
        }

        return false;
    }
#if !LOGIC_SERVER
    double bossDeadTimeStamp = -1;
#endif
    private void _initState()
    {
        {
            GeEffectEx effect = null;
            IStateEventManager mgr = _addState(
                BeSceneState.onBulletTime,
                (BeSceneState fromState) =>
                {
#if !LOGIC_SERVER
                    Time.timeScale = Global.Settings.bullteScale;

                    var mainPlayer = mBattle.dungeonPlayerManager.GetMainPlayer();
                    if (null != mainPlayer)
                    {
                        int effectInfoId = 2;
                        effect = mainPlayer.playerActor.m_pkGeActor.CreateEffect(effectInfoId, mainPlayer.playerActor.GetPosition().vec3);
                    }
                    bossDeadTimeStamp = Utility.GetTimeStamp();
#endif
                },
                (BeSceneState toState) =>
                {
#if !LOGIC_SERVER
                    Time.timeScale = 1.0f;

                    if (effect != null)
                    {
                        effect.Remove();
                        effect = null;
                    }
#endif
                }
            );

            //!! 子弹时间！！
            BaseEventNode changeFight = new BaseEventNode(
                    "bullte2fight",
                    VInt.Float2VIntValue(Global.Settings.bullteTime * Global.Settings.bullteScale),
                    ()=>{ state = BeSceneState.onFight; },
                    eBeStateReentrantType.Continue);
 


            mgr.Add(changeFight);
        }

        {
            IStateEventManager mgr = _addState(
                BeSceneState.onReady,
                (BeSceneState fromState) =>
                {
                    for (int i = 0; i < mEntitys.Count; ++i)
                    {
                        var item = mEntitys[i] as BeActor;
                        if (item != null && item.aiManager != null && item.aiManager.scenarioAgent != null)
                        {
                            item.StartAI(null);
                        }
                    }
                },
                (BeSceneState toState) =>
                {
                    for (int i = 0; i < mEntitys.Count; ++i)
                    {
                        var item = mEntitys[i] as BeActor;
                        if (item != null)
                        {
                            if (item.m_iCamp == (int)ProtoTable.UnitTable.eCamp.C_ENEMY && !item.isMainActor)
                            {
                                item.StartAI(null);
                            }
                            //公会本 己方大炮开启AI
                            else if(item.m_iCamp == (int)ProtoTable.UnitTable.eCamp.C_HERO && mBattle.GetBattleType() == BattleType.GuildPVE)
                            {
                                item.StartAI(null);
                            }
                        }
                    }
                    TriggerEventNew(BeEventSceneType.AfterOnReady);
                }
            );

            BaseEventNode changeFight = new BaseEventNode(
                    "onready2fight",
                GlobalLogic.VALUE_1,
                    () => { state = BeSceneState.onFight; },
                    eBeStateReentrantType.Reset
                    );
            mgr.Add(changeFight);
        }

        {
            _addState(BeSceneState.onFight,
                (BeSceneState from) =>
                {
                    GlobalEventSystem.GetInstance().SendUIEvent(EUIEventID.DungeonOnFight);
                },
                (BeSceneState to) =>
                {
                }
            );
        }

        {
            _addState(BeSceneState.onFinish,
                (BeSceneState from) =>
                {
                    if (mBattle == null || mBattle.dungeonPlayerManager == null) return;

                    if (BattleMain.IsModePVP3V3(mBattle.GetBattleType()))
                    {
                        var pvp3v3battle = mBattle as PVP3V3Battle;
                        if (pvp3v3battle == null) return;
                        var actor = pvp3v3battle.GetWinActor();
                        if (actor != null)
                        {
                            actor.winHandler = BeUtility.ShowWin(actor, actor.winHandler);
                        }
                    }
                    else if ((mBattle as PVPBattle) != null)
                    {
                        var pvpBattle = mBattle as PVPBattle;
                        if(pvpBattle != null)
                            pvpBattle.OnPlayWinAction(ePVPBattleEndType.onAllEnemyDied);
                    }
                    else
                    {
#if !LOGIC_SERVER
                        if( mBattle.dungeonPlayerManager.GetMainPlayer() == null) return;

                        var actor = mBattle.dungeonPlayerManager.GetMainPlayer().playerActor;
                        if (actor != null && mBattle.GetBattleType() != BattleType.NewbieGuide)
                        {
                            actor.winHandler = BeUtility.ShowWin(actor, actor.winHandler);
                        }
#endif
                    }


                },
                (BeSceneState to) =>
                {
                });
        }
    }

    public void OnReady()
    {
        if (isUseBossShow)
        {
            DelayCaller.DelayCall(100, () =>
            {
                DoBossShow();
            });
        }

        FinishTraceDoor();
    }

    public void DoBossShow()
    {
        if (mIsBossScene)
        {
            if (mBattle.GetBattleType() == BattleType.DeadTown ||
                mBattle.GetBattleType() == BattleType.ChampionMatch ||
                mBattle.GetBattleType() == BattleType.TreasureMap)
            {
                return;
            }

#if !LOGIC_SERVER
            var battleUI = BattleUIHelper.GetBattleUIComponent<BattleUIPve>();
            if (battleUI != null)
                battleUI.ShowBossWarning();
#endif
        }
    }

#endregion

#region Generate ID
    protected int sID = 0;
    protected int _genID()
    {
        return ++sID;
    }
    public void ClearSID()
    {
        sID = 0;
    }
    
    protected uint m_AttackProcessID = 0;
    public uint GenAttackProcessID()
    {
        return ++m_AttackProcessID;
    }
#endregion

#region Scene Create
    public static BeScene CreateScene(string path,BaseBattle battle_)
    {
        // FIX
        // 这里会有问题
        // 会在下一个BeScene创建的时候，再次执行上个BeScene的Unload
        var sCurrentBeScene = new BeScene();
        sCurrentBeScene.mBattle = battle_;
        sCurrentBeScene.Load(path);
        sCurrentBeScene.mDurtime = 0;

#if !LOGIC_SERVER


        if (null != BattleMain.instance)
        {
            if (BattleMain.battleType == BattleType.Dungeon)
            {
                sCurrentBeScene.LoadTaskMonsters(MissionManager.GetInstance().GetMainTask(BattleDataManager.GetInstance().BattleInfo.dungeonId));
            }
        }

#endif


        return sCurrentBeScene;
    }

    
    private void _originEntityRemoveAll()
    {
        _EntityRemoveAll(true, item =>
        {
            if (item.m_iCamp == (int)ProtoTable.UnitTable.eCamp.C_HERO)
            {
                return _isNeedRemoveByBeEntity(item);
            }
            else
            {
                item.ClearEvent();

                if (BeClientSwitch.FunctionIsOpen(ClientSwitchType.OnRemove))
                {
                    //过门的时候怪物调用死亡流程
                    if ((item as BeActor) != null)
                    {
                        item.OnDead();
                    }
                }

                item.OnRemove();
                return true;
            }
        });
    }

    public void Pvp3v3EntityRemoveAll()
    {
        _EntityRemoveAll(true, item =>
        {
            if (item.m_iCamp == (int)ProtoTable.UnitTable.eCamp.C_ENEMY ||
                item.m_iCamp == (int)ProtoTable.UnitTable.eCamp.C_HERO)
            {
                return _isNeedRemoveByBeEntity(item);
            }
            else
            {
                item.ClearEvent();
                item.OnRemove();
                return true;
            }
        });
    }

    private bool _isNeedRemoveByBeEntity(BeEntity item)
    {
        if (null == item)
        {
            return true;
        }

        //召唤兽不清除
        BeActor actor = item as BeActor;
        if (actor != null && !actor.IsDead())
        {
            if (actor.GetEntityData().isPet)
                return false;

            //为了解决炫纹法师在过门禁止输入的时候AI释放豪龙破军会导致自己不受阻挡影响 然后被移除的bug
            bool flag = false;
            if (mBattle != null && !mBattle.FunctionIsOpen(BattleFlagType.EntityRemoveInPassDoor))
            {
                flag = actor.GetEntityData().isSummonMonster && !actor.stateController.HasBornAbility(BeAbilityType.MOVE) ||
                    !actor.stateController.HasBornAbility(BeAbilityType.BLOCK) && !actor.stateController.NotRemoveTransDoor();
            }
            else
            {
                flag = actor.GetEntityData().isSummonMonster && !actor.stateController.HasBornAbility(BeAbilityType.MOVE) ||
                    !actor.stateController.BlockByScene() && !actor.stateController.NotRemoveTransDoor();
            }

            if (flag)
            {
                //item.TriggerEvent(BeEventType.onDead);
                //int[] vals = new int[1];
                //vals[0] = 0;
                //item.TriggerEvent(BeEventType.onDead, new object[] { vals ,false });
                bool isForce = false;
                item.TriggerEventNew(BeEventType.onDead, new EventParam() { m_Bool = true, m_Bool2 = isForce });
                item.SetIsDead(true);

                item.ClearEvent();
                item.OnDead();
                item.OnRemove();
                return true;
            }
        }

        if ((item as BeProjectile) != null || (item as BeObject) != null)
        {
            item.ClearEvent();
            item.OnRemove();
            return true;
        }

        return false;
    }

    bool mIsDayTime = false;
    public void SetDayTime(bool isDayTime)
    {
        mIsDayTime = isDayTime;
    }
    public bool IsDayTime()
    {
        return mIsDayTime;
    }

    public void ReloadScene(ISceneData data)
    {
        if (data == null)
        {
            Logger.LogError("data is nil");
            return;
        }

        mSceneData = data;
       
        mCurrentGeScene.ReloadSceneWithSameScene(mSceneData);
      
        _setBlockInfo();
        _setEventAreaInfo();

        _RemoveRegion(true,item =>
        {
            item.Remove();
            return true;
        });

        mDoorList.Clear();

        mDecoratorList.RemoveAll(item =>
        {
            //item.Destroy();
#if !LOGIC_SERVER

           mCurrentGeScene.DestroyActor(item);

#endif

            return true;
        });

        _resetState();

        mIsBossScene = false;
        mIsDoorOpened = false;

        mPrePauseState = BeSceneState.onNone;

        state = BeSceneState.onCreate;
    }

    public void ReloadScene(string path)
    {
        _originEntityRemoveAll();
        LoadSceneData(path, true);
        ReloadScene(mSceneData);
        mDurtime = 0;
#if !SERVER_LOGIC
        GeWeatherManager.instance.ChangeWeather(sceneData.GetWeatherMode());
#endif
    }

    private bool _loadSceneData(string path)
    {
        mSceneData = mBattle.dungeonManager.GetDungeonDataManager().CurrentSceneData();

        if (null != mSceneData)
        {
            logicWidth = sceneData.GetLogicX();
            logicHeight = sceneData.GetLogicZ();

            logicXSize = new VInt2(sceneData.GetLogicXSize().x,sceneData.GetLogicXSize().y);
            logicZSize = new VInt2(sceneData.GetLogicZSize().x,sceneData.GetLogicZSize().y);

            logicGrild = new VInt2(sceneData.GetGridSize().x,sceneData.GetGridSize().y);
            return true;
        }
        else
        {
            Logger.LogErrorFormat("can't load with path \"{0}\"!", path);
            return false;
        }
    }

    private void _setBlockInfo()
    {
        mBlockInfo = new byte[mSceneData.GetBlockLayerLength()];
        if(mSceneData.GetBlockLayerLength() > 0)
            Array.Copy(mSceneData.GetRawBlockLayer(), mBlockInfo, mSceneData.GetBlockLayerLength());
    }

    private void _setEventAreaInfo()
    {
        var eventLayer = mSceneData.GetRawEventAreaLayer();
        if (null == eventLayer)
        {
            return;
        }

        mEventAreaInfo = new byte[eventLayer.Length];
        if (mEventAreaInfo.Length > 0)
            Array.Copy(eventLayer, mEventAreaInfo, mEventAreaInfo.Length);
    }

    public void LoadSceneData(string path,bool isReload = false)
    {
        if (_loadSceneData(path))
        {
            if (!isReload)
            {
                _setBlockInfo();
                _setEventAreaInfo();
            }
        }
    }

    private void _loadDynSceneSetting()
    {
#if !LOGIC_SERVER

  if (null == sceneData)
            return;

        var ligitMap = AssetLoader.instance.LoadRes(sceneData.GetLightmapsettingsPath(),typeof(DynSceneSetting)).obj as DynSceneSetting;
        sceneData.SetLightmapsettings(ligitMap);

        if (sceneData.GetLightmapsettings() != null)
        {
            sceneData.GetLightmapsettings().Apply();
        }

#endif

    }

    public void Load(string path)
    {
        Logger.LogWarningFormat("Load Scene  Path: {0}", path);

        _initState();

        LoadSceneData(path);
        AttachGeScene();
    }

    public void AttachGeScene()
    {
        DettachGeScene();

        _loadDynSceneSetting();

        mCurrentGeScene = new GeSceneEx();

#if !SERVER_LOGIC
        mCurrentGeScene.LoadScene(sceneData);
        GeWeatherManager.instance.ChangeWeather(sceneData.GetWeatherMode());
#endif
    }

    public void DettachGeScene()
    {
        if (mCurrentGeScene != null)
        {
            bool needGC = true;
            if (mBattle != null && mBattle.GetBattleType() == BattleType.Demo)
                needGC = false;

            mCurrentGeScene.UnloadScene(needGC);
            mCurrentGeScene = null;
        }
    }

    private void DettachEntity()
    {
    }

    public List<BeEntity> GetSaveTempList()
    {
        return mTempSaveEntitys;
    }
    /// <summary>
    /// 如果在主逻辑的Update里 请慎重调用
    /// 考虑到List.Remove 却未对遍历时Index做修正，参考UpdateEntities
    /// </summary>
    /// <param name="actor"></param>
    public void RemoveToTemp(BeEntity actor)
    {
        mEntitys.Remove(actor);
        mEntitysMap.Remove(actor.GetPID());
        mTempSaveEntitys.Add(actor);
    }

    public void RestoreFromTemp(BeEntity actor)
    {
        if (!mEntitys.Contains(actor))
        {
            mEntitys.Add(actor);
        }

        if (!mEntitysMap.ContainsKey(actor.GetPID()))
        {
            mEntitysMap.Add(actor.GetPID(), actor);
        }
        mTempSaveEntitys.Remove(actor);
    }

    public void ClearTempSavedEntitys()
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeScene.ClearTempSavedEntitys"))
        {
#endif
        mTempSaveEntitys.RemoveAll(x =>
        {
            x.ClearEvent();
            x.OnRemove(true);
            return true;
        });
#if ENABLE_PROFILER
        }
#endif
    }

    public void ClearBossDeadBody()
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeScene.ClearBossDeadBody"))
        {
#endif
        mDeadBody.RemoveAll(x =>
        {
            x.ClearEvent();
            x.OnRemove(true);
            return true;
        });
#if ENABLE_PROFILER
        }
#endif
    }

    public void ClearAllEntity(bool removeMechanismFlag = false)
    {
        mHell = null;
        //ForceUpdatePendingArray();
        _EntityRemoveAll(true, x =>
         {
             BeActor actor = x as BeActor;
             if(removeMechanismFlag && actor != null)
             {
                 actor.RemoveAllMechanism();
             }
             x.ClearEvent();
             x.OnRemove(true);
             return true;
         });
    }

    public void Unload()
    {
        mPrePauseState = BeSceneState.onNone;

        ClearBossDeadBody();
        ClearTempSavedEntitys();
        ClearAllEntity();

        _clearAllStates();

        UnRegisterAll();

        if (null != DelayCaller)
            DelayCaller.Clear();

        //BeAICommandPoolImp.GetInstance().Clear();
        //BeProjectilePoolImp.GetInstance().Clear();
        BeBuffPool.Clear();
        BeAICommandPool.Clear();
        BeProjectilePool.Clear();
        MechanismPool.Clear();
        
        CPoolManager.GetInstance().Clear();

#if !SERVER_LOGIC
        DettachGeScene();
        GeAnimatInstPool.instance.ClearAll();

#endif


        //if (!DebugSettings.instance.EnableActionFrameCache)
#if LOGIC_SERVER
        if (mActionFrameMgr != null)
        {
            mActionFrameMgr.Clear();
        }
        if(mSkillFileCache != null)
        {
            mSkillFileCache.Clear();
        }
#else
        BeActionFrameMgr.Clear();
        //if (!DebugSettings.instance.EnableDSkillDataCache)
        BeActionFrameMgr.ClearSkillObjectCache();
        SkillFileListCache.Clear();
#endif


        mSceneData = null;
    }

#endregion

#region Create Entity
    private void _setTransportDoor(bool state)
    {
        if (mIsDoorOpened != state)
        {
            mIsDoorOpened = state;

            for (int i = 0; i < mDoorList.Count; ++i)
            {
                BeRegionTransportDoor door = mDoorList[i] as BeRegionTransportDoor;
                if (door != null)
                {
                    if (door.isEggDoor && state) continue;
                    door.active = state;
                    door.activeEffect = state;
                }
            }

            TriggerEventNew(BeEventSceneType.onDoorStateChange, new EventParam(){m_Bool = state});
            //TriggerEvent(BeEventSceneType.onDoorStateChange, new object[] { state });
        }
    }

    public void OpenEggDoor()
    {
        for (int i = 0; i < mDoorList.Count; ++i)
        {
            BeRegionTransportDoor door = mDoorList[i] as BeRegionTransportDoor;
            if (door != null && door.isEggDoor)
            {
                door.active = true;
                door.activeEffect = true;
            }
        }
    }
    public List<BeRegionBase> GetDoorList()
    {
        return mDoorList;
    }


#region Basic
    public BeActor AddActor(int iResID, int iCamp, int iID = -1, bool useCube = false)
    {
        if (iID == -1)
        {
            iID = _genID();
        }

        BeActor actor = new BeActor(iResID, iCamp, iID);
        mCheckMonsterDirtyFlag = true;
        mPendingArray.Add(actor);
        mEntitysMap.Add(iID, actor);
        actor.SetBeScene(this);
        actor.Create(0, useCube);
        return actor;
    }

    public BeProjectile AddProjectile(int iResID, int iCamp, int type, int value, int triggerSkillLevel = 1, BeEntity owner = null, int iID = -1, bool useCube = false)
    {
        if (iID == -1)
        {
            iID = _genID();
        }

        BeProjectile projectile = BeProjectilePool.GetProjectile(iResID, iCamp, (int)iID, useCube);
        if (owner != null)
            projectile.SetOwner(owner);
        projectile.triggerSkillLevel = triggerSkillLevel;
        projectile.SetType((ProjectType)type, value);
        mPendingArray.Add(projectile);
        mEntitysMap.Add(iID, projectile);
        projectile.SetBeScene(this);
        projectile.Create(0, useCube);
        TriggerEventNew(BeEventSceneType.onAddEntity,new EventParam(){m_Obj = projectile});
        //TriggerEvent(BeEventSceneType.onAddEntity,new object[] { projectile });

        return projectile;
    }


    public BeObject AddLogicObject(int resID, int camp = 2)
    {
        var resData = TableManager.GetInstance().GetTableItem<ProtoTable.ResTable>(resID);
        if (resData != null)
        {
            BeObject logicObject = new BeObject(resID, camp, _genID());

            mPendingArray.Add(logicObject);
            mEntitysMap.Add(logicObject.GetPID(), logicObject);

            logicObject.SetBeScene(this);
            logicObject.Create(1);

            logicObject.RegisterEventNew(BeEventType.onDead, eventParam =>
            {
                logicObject.SetBlockLayer(false);
            });

            return logicObject;
        }

        return null;
    }

    public BeRegion AddRegion(ISceneRegionInfoData info, BeRegionBase.TriggerTarget target)
    {
        BeRegion region = new BeRegion();
        region.SetBeScene(this);
        region.Create(info);
        region.SetPosition(new VInt3(info.GetEntityInfo().GetPosition()));
        region.SetScale((VInt)info.GetEntityInfo().GetScale());

        region.triggerTarget = target;

        if (region != null)
            mPendingRegion.Add(region);

        return region;
    }

    public BeRegionDropItem AddDropItem(ISceneRegionInfoData info, Battle.DungeonDropItem dropItem, VInt3 originPos,bool needAdd = true, bool needTargetList = true)
    {
        if (needAdd)
        {
            var list = BattleDataManager.GetInstance().BattleInfo.dropItemIds;
            var idx = list.BinarySearch(dropItem.id);
            if (idx < 0)
            {
                BattleDataManager.GetInstance().BattleInfo.dropItemIds.Insert(~idx, dropItem.id);
                BattleDataManager.GetInstance().BattleInfo.dropCacheItemIds.Add(dropItem);
            }
            else
            {
                Logger.LogErrorFormat("already pick up the item with id {0}", dropItem.id);
            }
        }

        var region = new BeRegionDropItem();
        region.SetBeScene(this);
        region.SetDropItem(dropItem);
        //region.SetPickedList(BattleDataManager.GetInstance().BattleInfo.pickedItems);
        region.Create(info);
        region.SetScale((VInt)info.GetEntityInfo().GetScale());
        //region.SetPosition(new Vec3(info.position));
        region.SetPosition(originPos);
        if (needTargetList)
        {
            region.triggerTarget = _regionTargetList;
        }
        else
        {
            region.triggerTarget = _defaultRegionTargetList;
        }

        region.StartTrail(originPos, new VInt3(info.GetEntityInfo().GetPosition()));


        if (region != null)
        {
            mPendingRegion.Add(region);
        }

        return region;
    }

    public void ForcePickUpDropItem(BeActor actor)
    {
        for (int i = 0; i < mRegions.Count; ++i)
        {
            var region = mRegions[i] as BeRegionDropItem;
            if (null != region)
            {
                region.ForceTriggerEnter(actor);
            }
        }
    }

    //判断是否处于传送门内 并且状态是可以传送
    public bool CheckInDoorRange(BeActor actor)
    {
        bool isRange = false;
        for (int i = 0; i < mDoorList.Count; ++i)
        {
            BeRegionTransportDoor door = mDoorList[i] as BeRegionTransportDoor;

            if (null != door)
            {
                if (door.CheckInDoorRange(actor))
                {
                    isRange = true;
                }
            }
        }
        return isRange;
    }

    private List<BattlePlayer> _regionTargetList()
    {
        List<BattlePlayer> list = new List<BattlePlayer>();
        // TODO 这里要改
#if !SERVER_LOGIC

        var mainPlayer = mBattle.dungeonPlayerManager.GetMainPlayer();
        list.Add(mainPlayer);

#endif

        return list;
    }

    List<BattlePlayer> defaultTargetList = null;
    private List<BattlePlayer> _defaultRegionTargetList()
    {
        if (defaultTargetList == null)
        {
            defaultTargetList = new List<BattlePlayer>();
        }
        return defaultTargetList;
    }

    public BeRegionTransportDoor AddTransportDoor(ISceneRegionInfoData info, BeRegionBase.TriggerTarget target, BeRegionBase.TriggerRegion callback = null, bool isboss = false, bool isvisited = false, TransportDoorType doorType = TransportDoorType.None, bool isEggDoor = false, string materialPath = "")
    {
        BeRegionTransportDoor region = new BeRegionTransportDoor();

        region.SetBeScene(this);
        region.Create(info, isboss, doorType);
        region.SetPosition(new VInt3(info.GetEntityInfo().GetPosition()));
        region.SetScale((VInt)info.GetEntityInfo().GetScale());
        region.SetVisited(isvisited);
        region.SetDoorType(doorType);
        region.isEggDoor = isEggDoor;
        region.triggerTarget = target;
        region.triggerRegion = callback;

#if UNITY_EDITOR && !SERVER_LOGIC
        if (!Application.isPlaying)
        {
            DEntityInfo entityInfo = info.GetEntityInfo() as DEntityInfo;
            var root = region.geActor.GetEntityNode(GeActorEx.GeEntityNodeType.Root);

            region.currentBeScene.currentGeScene.AddToColorDescList(root);

            DEntityInfoComponent com = null;
            com = root.GetComponent<DEntityInfoComponent>();
            if( com == null)
            {
                com = root.AddComponent<DEntityInfoComponent>();
            }
            entityInfo.obj = root;
            com.info = entityInfo;
        }
#endif

#if !SERVER_LOGIC

        if (!string.IsNullOrEmpty(materialPath))
        {
            var root = region.geActor.GetEntityNode(GeActorEx.GeEntityNodeType.Root);
            var materialReplacer = root.AddComponent<MaterialReplacerComponent>();
            var path = materialPath.Remove(materialPath.LastIndexOf("."));
            path = path.Replace("Assets/Resources/","");
            var material = AssetLoader.instance.LoadRes(path, typeof(Material)).obj as Material;
            materialReplacer.SetDoorMaterial(material);
        }
#endif

        if (mBattle.recordServer.IsProcessRecord())
        {
             mBattle.recordServer.RecordProcess("[Craete-Doors] {0} {1}", region.position, doorType);
             mBattle.recordServer.MarkInt(0x8779799, new int[]
             {
                    region.position.x,
                    region.position.y,
                    region.position.z,
                    (int)doorType
             });
            // Mark:0x8779799 Create-Doors position:({0},{1},{2}) doorType: {3}

        }

        mDoorList.Add(region);
        mPendingRegion.Add(region);

        return region;
    }

    public void SetBossTransportDoorEffectState(bool isopen)
    {
        for (int i = 0; i < mDoorList.Count; ++i)
        {
            BeRegionTransportDoor door = mDoorList[i] as BeRegionTransportDoor;

            if (null != door && door.IsBoss())
            {
                door.activeEffect = isopen;
            }
        }
    }

    public void SetTransportDoorEnable(TransportDoorType type, bool isenable)
    {
        for (int i = 0; i < mDoorList.Count; ++i)
        {
            BeRegionTransportDoor door = mDoorList[i] as BeRegionTransportDoor;
            if (null != door)
            {
                ISceneTransportDoorData doorData = door.regionInfo as ISceneTransportDoorData;

                if (null != doorData && doorData.GetDoortype() == type)
                {
                    door.activeEffect = isenable;
                    door.active = isenable;
                    break;
                }
            }
        }
    }

#endregion

#region Logic
    public void CreateLogic()
    {
        for (int i = 0; i < sceneData.GetDestructibleInfoLength(); i++)
        {
            ISceneEntityInfoData item = sceneData.GetRegionInfo(i).GetEntityInfo();
            CreateDestruct(item.GetResid(), new VInt3(item.GetPosition()), item.GetColor(), new VInt(item.GetScale()));
        }
    }

#endregion

#region birth & dead
    private void _birthMonster(BeActor actor, ProtoTable.UnitTable.eType type)
    {
        if (actor.attribute.type != (int)ProtoTable.UnitTable.eType.SKILL_MONSTER)
            _setTransportDoor(false);

        if (actor.m_iCamp == (int)ProtoTable.UnitTable.eCamp.C_ENEMY)
        {
            switch (type)
            {
                case ProtoTable.UnitTable.eType.MONSTER:
                case ProtoTable.UnitTable.eType.ELITE:
                    //++mSceneExData.enemyAlive;
                    break;
                case ProtoTable.UnitTable.eType.BOSS:
                    {
                        //++mSceneExData.enemyAlive;

                        actor.RegisterEventNew(BeEventType.onHurt, args =>
                        //actor.RegisterEvent(BeEventType.onHurt, (object[] args) =>
                        {
                            CheckBossDead(actor);
                        });

                        actor.RegisterEventNew(BeEventType.OnBuffDamage, args =>
                        {
                            CheckBossDead(actor);
                        });

                        actor.RegisterEventNew(BeEventType.onSpecialDead, (args) => 
                        {
                            CheckBossDead(actor);
                        });
                    }
                    break;
            }

            //Logger.LogProcessFormat("birth the monster {0}, alive {1}", actor.GetName(), mSceneExData.enemyAlive);
        }
    }

    private bool isBossDeaded = false;
    private void CheckBossDead(BeActor actor)
    {      
        if (actor.GetLifeState() == (int)EntityLifeState.ELS_ALIVE)
        {
            var data = actor.GetEntityData();
            if (mIsBossScene && data != null && data.type == (int)ProtoTable.UnitTable.eType.BOSS && data.battleData != null)
            {
                if (!isBossDeaded && data.GetHP() <= 0)
                {
                    if ( state < BeSceneState.onBulletTime && _isAllBossDead())
                    {
                        isBossDeaded = true;
                        if (mBattle.GetBattleType() != BattleType.DeadTown)
                            state = BeSceneState.onBulletTime;

                        _deadBoss(actor);
                    }
                }
            }
        }
    }

    public bool _isAllBossDead()
    {
        for (int i = 0; i < mEntitys.Count; ++i)
        {
            var mons = mEntitys[i] as BeActor;
            if (mons != null && mons.IsBoss() && !mons.IsDead())
            {
                return false;
                /*
                var data = mons.GetEntityData();
                if (data != null && data.type == (int)ProtoTable.UnitTable.eType.BOSS && data.battleData != null)
                {
					if (data.GetHP() > 0)
                    {
                        return false;
                    }
                }*/
            }
        }

        return true;
    }

    public bool isAllEnemyDead()
    {
        var list = GamePool.ListPool<BeEntity>.Get();
        list.AddRange(mEntitys);
        list.AddRange(mPendingArray);
        bool ret = true;
        for (int i = 0; i < list.Count; ++i)
        {
            var mons = list[i] as BeActor;
            if (mons != null)
            {
                var data = mons.GetEntityData();

                if (data != null &&
                    mons.m_iCamp == (int)ProtoTable.UnitTable.eCamp.C_ENEMY &&
                    data.type != (int)ProtoTable.UnitTable.eType.SKILL_MONSTER &&
                    data.type != (int)ProtoTable.UnitTable.eType.EGG &&
                    (data.type == (int)ProtoTable.UnitTable.eType.BOSS ||
                    data.type == (int)ProtoTable.UnitTable.eType.ELITE ||
                    data.type == (int)ProtoTable.UnitTable.eType.MONSTER)
                    && data.battleData != null)
                {
                    if (!mons.IsDead())
                    {
                       // Logger.LogErrorFormat("monster {0} is not dead {1}", mons.GetName(), mons.GetPID());
                       ret = false;
                       break;
                    }

                    if (mons.GetLifeState() < (int)EntityLifeState.ELS_CANREMOVE)
                    {
                      //  Logger.LogErrorFormat("monster {0} ELS_CANREMOVE {1}", mons.GetName(), mons.GetPID());
                        ret = false;
                        break;
                    }

                }
            }
        }
        GamePool.ListPool<BeEntity>.Release(list);

        return ret;
    }


    private void _deadMonster(BeActor actor)
    {
        if (_isEnemyMonster(actor))
        {
            TriggerEventNew(BeEventSceneType.onMonsterDead, new EventParam(){m_Obj = actor});
            //TriggerEvent(BeEventSceneType.onMonsterDead, new object[] { actor });
        }
        else
        {
            if (actor == null) return;
            var data = actor.GetEntityData();
            if (data != null && data.type == (int)ProtoTable.UnitTable.eType.EGG)
            {
                TriggerEventNew(BeEventSceneType.onEggDead);
            }
        }
    }

    private bool _isEnemyMonster(BeActor actor)
    {
        if (null == actor)
        {
            return false;
        }

        BeEntityData data = actor.GetEntityData();

        if (null == data)
        {
            return false;
        }

        if (actor.m_iCamp == (int)ProtoTable.UnitTable.eCamp.C_ENEMY &&
                data.type != (int)ProtoTable.UnitTable.eType.SKILL_MONSTER &&
                data.type != (int)ProtoTable.UnitTable.eType.EGG)
        {
            return true;
        }

        return false;
    }

    private void _removeMonster(BeActor actor)
    {
        if (_isEnemyMonster(actor))
        {
            TriggerEventNew(BeEventSceneType.onMonsterRemoved);
        }
    }

    private void _removeDestruct()
    {
        TriggerEventNew(BeEventSceneType.onMonsterRemoved);
    }

    private int checkMosnterClearInterval = GlobalLogic.VALUE_1000;
    private int checkAcc = 0;
    private bool mCheckMonsterDirtyFlag = false;
    private void UpdateCheckMosnterClear(int delta)
    {
        if (state == BeSceneState.onFinish ||
            state == BeSceneState.onNone ||
            state == BeSceneState.onReady ||
            state == BeSceneState.onPause)
        {
            return;
        }

        checkAcc += delta;
        if (mCheckMonsterDirtyFlag || checkAcc >= checkMosnterClearInterval)
        {
            mCheckMonsterDirtyFlag = false;
            checkAcc = 0;
            CheckMonsterClear();
        }
    }

    private void CheckMonsterClear()
    {
        if (isAllEnemyDead())
        {
            if (state != BeSceneState.onPause &&
                state != BeSceneState.onClear)
            {
                _onClearEvent();
            }
        }
        else
        {
            if (state != BeSceneState.onFight &&
                state != BeSceneState.onPause &&
                state != BeSceneState.onBulletTime)
            {
                state = BeSceneState.onFight;
                _setTransportDoor(false);
            }
        }
    }

    private Vec3 mgeDeadBossPositionGE = Vec3.zero;

    public Vec3 GeGDeadBossPosition()
    {
        return mgeDeadBossPositionGE;
    }

    private void _deadBoss(BeActor actor)
    {
        TriggerEventNew(BeEventSceneType.onBossDead);
        var isNeedClear = mIsBossScene && (actor.GetEntityData().type == (int)ProtoTable.UnitTable.eType.BOSS);
        if (isNeedClear)
        {
            bossDead = true;
            mgeDeadBossPositionGE = actor.GetGePosition(PositionType.ORIGIN);
            ClearMonsters(actor);
            if(actor != null && actor.CurrentBeBattle != null && !actor.CurrentBeBattle.HasFlag(BattleFlagType.PendingArray_Dont_Remove))
            {
                ClearPendingMonster(actor);
            }
        }
    }
    private void _MakeMonsterDead(List<BeEntity> entityList,BeActor actor)
    {
        for (int j = 0; j < entityList.Count; j++)
        {
            var leftItem = entityList[j] as BeActor;
            if (leftItem != null &&
                !leftItem.IsDead() &&
                (actor == null || actor != null && actor != leftItem) &&
                leftItem.m_iCamp == (int)ProtoTable.UnitTable.eCamp.C_ENEMY &&
                leftItem.GetEntityData().type != (int)ProtoTable.UnitTable.eType.SKILL_MONSTER &&
                leftItem.GetEntityData().type != (int)ProtoTable.UnitTable.eType.EGG
            )
            {
                leftItem.GetEntityData().SetHP(-1);
                leftItem.JudgeDead();
                leftItem.DoDead();
            }
        }
    }
    private void ClearPendingMonster(BeActor actor)
    {
        _MakeMonsterDead(mPendingArray,actor);
    }
    private void ClearMonsters(BeActor actor)
    {
        _MakeMonsterDead(mEntitys, actor);
    }

    // 游戏开始更新之前使用
    public BeActor FindAPendingMonster()
    {
        for (int j = 0; j < mPendingArray.Count; j++)
        {
            var leftItem = mPendingArray[j] as BeActor;
            if (leftItem != null &&
                leftItem.m_iCamp == (int)ProtoTable.UnitTable.eCamp.C_ENEMY &&
                leftItem.GetEntityData().type != (int)ProtoTable.UnitTable.eType.SKILL_MONSTER &&
                leftItem.GetEntityData().type != (int)ProtoTable.UnitTable.eType.EGG
            )
            {
                return leftItem;
            }
        }
        return null;
    }

#endregion

#region Monster

    private void _createMonsterUI(BeActor actor, ProtoTable.UnitTable monsterData, BeActor owner = null, bool isSummon = false)
    {
#if !LOGIC_SERVER

        if (monsterData.Type == ProtoTable.UnitTable.eType.SKILL_MONSTER && !monsterData.ShowName)
            return;
		bool enemy = true;
		if (BattleMain.IsModePvP(mBattle.GetBattleType()))
		{
			var localPlayer = BattleMain.instance.GetPlayerManager().GetMainPlayer().playerActor;
			if (owner != null && localPlayer == owner)
				enemy = false;
		}
		else {
			if (actor.m_iCamp <= 0){
				enemy = false;
			}
		}

        actor.m_pkGeActor.CreateHPBarMonster(monsterData.Type, actor.GetEntityData().name, Color.white, singleBloodBarCount, enemy);

        if (monsterData.MonsterTitle > 0)
        {
            actor.m_pkGeActor.AddTittleComponent(monsterData.MonsterTitle, monsterData.Name, 0, "", 0, 0, PlayerInfoColor.BOSS);
            actor.m_pkGeActor.CreateInfoBar(monsterData.Name, PlayerInfoColor.BOSS, 0);
        }

        switch (monsterData.Type)
        {
            case ProtoTable.UnitTable.eType.BOSS:
                actor.m_pkGeActor.CreateMonsterInfoBar(monsterData.Name, PlayerInfoColor.BOSS);
                if (!isSummon && monsterData.ShowFootBar != 4) 
                	actor.m_pkGeActor.CreateMonsterLoop();
                break;
            case ProtoTable.UnitTable.eType.ELITE:
                actor.m_pkGeActor.CreateMonsterInfoBar(monsterData.Name, PlayerInfoColor.ELITE_MONSTER);
                {
                    if (Utility.IsStringValid(monsterData.FootEffectName))
                    {
                        actor.m_pkGeActor.CreateFootIndicator(monsterData.FootEffectName);
                    }
                }
                break;
            case ProtoTable.UnitTable.eType.MONSTER:
                {
                    if (monsterData.ShowName)
                    {
                        actor.m_pkGeActor.CreateMonsterInfoBar(monsterData.Name, PlayerInfoColor.ELITE_MONSTER);
                        actor.m_pkGeActor.SetHeadInfoVisible(false);
                    }

                    if (Utility.IsStringValid(monsterData.FootEffectName))
                    {
                        actor.m_pkGeActor.CreateFootIndicator(monsterData.FootEffectName);

                    }
                }
                break;
            case ProtoTable.UnitTable.eType.SKILL_MONSTER:
                if (monsterData.ShowName)
                {
                    actor.m_pkGeActor.CreateMonsterInfoBar(monsterData.Name, PlayerInfoColor.TOWN_NPC);
                    actor.m_pkGeActor.SetHeadInfoVisible(false);
                }

                if (!monsterData.ShowHPBar)
                    actor.m_pkGeActor.SetHPBarVisible(false);

                break;
        }

#endif

    }


    public BeActor CreateMonster(int monsterID, bool isSummon = false, List<int> prefix = null, int externalSkillLevel = 0, int camp = -1, BeActor owner = null, bool useCube = false)
    {
        MonsterIDData mdata = new MonsterIDData(monsterID);

        var monsterData = TableManager.GetInstance().GetTableItem<ProtoTable.UnitTable>(mdata.mid);
        if (monsterData == null)
        {
            monsterData = TableManager.GetInstance().GetTableItem<ProtoTable.UnitTable>(monsterID);
        }

        if (monsterData != null)
        {
            int resid = monsterData.Mode;
            if (camp == -1)
                camp = (int)monsterData.Camp;

            BeActor actor = AddActor(resid, camp, -1, useCube);
            Logger.LogProcessFormat("[创建怪物] 开始 最终创建 怪物 {0}, {1}, {2}", monsterID, isSummon, camp);

            Dictionary<int, int> skillInfo = new Dictionary<int, int>();
            int skillLevel = mdata.level;

            if (externalSkillLevel > 0)
            {
                skillLevel = externalSkillLevel;
            }

            if (monsterData.AttackSkillID != 0)
            {
                skillInfo.Add(monsterData.AttackSkillID, skillLevel);
            }

            for (int i = 0; i < monsterData.SkillIDs.Count; ++i)
            {
                var sid = monsterData.SkillIDs[i];
                if (sid != 0 && !skillInfo.ContainsKey(sid))
                {
                    var skillData = TableManager.GetInstance().GetTableItem<ProtoTable.SkillTable>(sid);
                    //开放了等级才能携带这个技能
                    if (skillData != null &&
                        (mdata.level >= skillData.LevelLimit ||
                            mdata.type == (int)ProtoTable.UnitTable.eType.ACCOMPANY ||
                            monsterData.APCIsSpecialConfig >= 1))
                    {
                        skillInfo.Add(sid, skillLevel);

                        //APC技能等级设置上限
                        if (monsterData.APCIsSpecialConfig > 0 && skillData.TopLevel > 0)
                        {
                            skillLevel = Math.Min(skillLevel, skillData.TopLevel);
                        }
                    }


                }
            }

            actor.SetDefaultHitSFX(monsterData.DefaultAttackHitSFXID);

            // actor.defaultWeaponTag = monsterData.AIWeaponTag;
            actor.SetDefaultWeapenTag(monsterData.AIWeaponTag);
            //APC换武器
            if (monsterData.APCWeaponRes > 0)
            {
                actor.ReplaceWeapon(monsterData.APCWeaponRes, monsterData.AIWeaponTag, monsterData.APCWeaponStrengthLevel);
            }

            if (owner != null)
            {
                owner.TriggerEventNew(BeEventType.OnBeforeInitData, new EventParam() {m_Obj = actor});
            }
            
            actor.InitData(mdata.level, 0, skillInfo, monsterData.APCIsSpecialConfig > 0 ? "apc" : "");
            actor.beHitEffect = monsterData.Hurt;

            actor.runSpeed = new VInt3(Global.Settings.monsterRunSpeed);
            actor.walkSpeed = new VInt3(Global.Settings.monsterWalkSpeed);

            var entityData = actor.GetEntityData();
            if (entityData != null)
            {
                //!! 这里注意
                entityData.weight = VInt.Float2VIntValue(Global.Settings.gravity) * VFactor.NewVFactor(monsterData.Weight,(long)GlobalLogic.VALUE_100);
                entityData.exp = monsterData.Exp;
                entityData.type = (int)monsterData.Type;
                entityData.skillMonsterCanBeAttack = monsterData.SkillMonsterCanBeAttack > 0;
                entityData.autoFightNeedAttackFirst = monsterData.AutoFightNeedAttackFirst > 0;
                entityData.getupIDRand = monsterData.GetupSkillRand;
                entityData.getupID = monsterData.GetupSkillID;
                entityData.hitIDRand = monsterData.HitSkillRand;
                entityData.hitID = monsterData.HitSkillID;
                entityData.camp = camp;
                
                entityData.normalAttackID = monsterData.AttackSkillID;

                actor.walkSpeed = actor.walkSpeed * new VFactor(monsterData.WalkSpeed,GlobalLogic.VALUE_100);
                actor.runSpeed = actor.runSpeed * new VFactor(monsterData.WalkSpeed,GlobalLogic.VALUE_100);

                entityData.isMonster = true;
                entityData.monsterID = monsterData.ID;
                entityData.monsterData = monsterData;
                //表现用
                entityData.overHeadHeight = monsterData.overHeadHeight / (float)(GlobalLogic.VALUE_1000);
                entityData.buffOriginHeight = monsterData.buffOriginHeight / (float)(GlobalLogic.VALUE_1000);
                entityData.simpleMonsterID = BeUtility.GetOnlyMonsterID(monsterData.ID);
                entityData.enhancedRadius = monsterData.enhanceRadius * (int)(IntMath.kIntDen / GlobalLogic.VALUE_1000);
                entityData.monsterIDData = mdata;
                entityData.name = monsterData.Name;

                entityData.isSpecialAPC = monsterData.APCIsSpecialConfig > 0;
                entityData.isShowSkillSpeech = true;

                entityData.walkAnimationSpeedPercent = monsterData.WalkAnimationSpeedPerent;
                entityData.height = monsterData.Height;
            }

            if (monsterData.FloatValue > 0)
                actor.floatingHeight = VInt.NewVInt(monsterData.FloatValue,1000);

            InitMonsterAttribute(actor.GetEntityData(), monsterData, monsterData.MonsterMode, mdata.difficulty, mdata.type, mdata.level);
            actor.SetMoveSpeedZAcc(actor.GetEntityData().weight);
            
            //根据玩家的抗魔值调整召唤兽的相关属性
            if (owner != null)
                actor.attribute.SetResistMagic(owner.attribute.GetResistMagic());

            FixUnitAttribute(actor.GetEntityData(), monsterData, camp);
            ChangeMonsterAbility(actor, monsterData);
            SetMonsterAiInfo(actor, monsterData);
            SetMonsterProtect(actor, monsterData);

            AddBornBuff(actor, monsterData.BornBuff, monsterData.BornBuff2, monsterData.BornMechanism);

            if (prefix != null && prefix.Count > 0)
            {
                DealPrefix(actor, prefix, monsterData);
            }

            _createMonsterUI(actor, monsterData, owner, isSummon);

            if(null != actor.m_pkGeActor)
            {
                if (ProtoTable.UnitTable.eType.BOSS == monsterData.Type)
                    actor.m_pkGeActor.SetMaterial("HeroGo/Surface/General_Rim_Flash");
                if (!actor.IsSkillMonster() && monsterData.ShowFootBar != 3)                                                 //技能实现的怪物不需要影子                                                                                                                             //技能实现的怪物不需要影子
                    actor.m_pkGeActor.AddSimpleShadow(Vector3.one);
            }

            actor.walkSpeeches =(monsterData.WalkSpeech);
            actor.attackSpeeches = (monsterData.AttackSpeech);
            actor.birthSpeeches = (monsterData.BirthSpeech);

 

            if (entityData.type == (int)ProtoTable.UnitTable.eType.MONSTER || 
				actor.walkSpeeches.Count > 0 && actor.walkSpeeches[0] > 0 ||
				actor.attackSpeeches.Count > 0 && actor.attackSpeeches[0] > 0 ||
				actor.birthSpeeches.Count > 0 && actor.birthSpeeches[0] > 0
			)
            {
				actor.RegisterEventNew(BeEventType.onStateChange, (param) =>
					{
						ActionState state = (ActionState)param.m_Int;

						if (state != ActionState.AS_BIRTH && !actor.IsCastingSkill() && actor.GetStateGraph().CurrentStateHasTag((int)AStateTag.AST_BUSY | (int)AStateTag.AST_CONTROLED | (int)AStateTag.AST_LOCKZ))
						{
#if !SERVER_LOGIC

							actor.m_pkGeActor.ShowHeadDialog("", true);

#endif

						}
						else
						{
							actor.ShowSpeech(state);
						}
					});
            }

 


            if (monsterData.GetupBati > 0)
            {
                int batiDuration = monsterData.GetupBati;
                actor.RegisterEventNew(BeEventType.onStateChange, (param) =>
                {
                    ActionState state = (ActionState)param.m_Int;
                    if (state == ActionState.AS_GETUP)
                    {
                        if (FrameRandom.Range1000() <= (uint)(Global.Settings.monsterGetupBatiFactor * GlobalLogic.VALUE_1000))
                            actor.buffController.TryAddBuff((int)GlobalBuff.GETUP_BATI, batiDuration);
                    }
                });
            }

            int modelScale = TableManager.GetValueFromUnionCell(monsterData.Scale, actor.GetEntityData().level);
            if (modelScale != GlobalLogic.VALUE_100)
            {
                var scaleFactor = new VFactor(modelScale,GlobalLogic.VALUE_100);
                actor.SetScale(actor.GetScale().i * scaleFactor);
            }

            int existTime = TableManager.GetValueFromUnionCell(monsterData.ExistDuration, actor.GetEntityData().level);
            if (BattleMain.IsModePvP(mBattle.GetBattleType()))
            {
                existTime = TableManager.GetValueFromUnionCell(monsterData.PVPExistDuration, actor.GetEntityData().level);
            }

            if (owner != null)
            {
                /*var lifeTimeArray = new int[2];
                lifeTimeArray[0] = existTime;
                lifeTimeArray[1] = GlobalLogic.VALUE_1000;
                owner.TriggerEvent(BeEventType.onChangeSummonLifeTime, new object[] { actor, lifeTimeArray });
                existTime = lifeTimeArray[0] * VFactor.NewVFactor(lifeTimeArray[1], GlobalLogic.VALUE_1000);*/
                var eventData = owner.TriggerEventNew(BeEventType.onChangeSummonLifeTime, new EventParam(){m_Obj = actor, m_Int = existTime, m_Int2 = GlobalLogic.VALUE_1000});
                existTime = eventData.m_Int * VFactor.NewVFactor(eventData.m_Int2, GlobalLogic.VALUE_1000);
            }

            if (existTime > 0)
            {
                actor.buffController.TryAddBuff((int)GlobalBuff.LIFE_TIME, existTime);
            }
#if !LOGIC_SERVER

			if (monsterData.ShowName)
				actor.m_pkGeActor.SetHeadInfoVisible(true);

#endif

            //if (monsterData.BornAI == ProtoTable.UnitTable.eBornAI.Start)
            //{
            //    actor.StartAI(null);
            //}

            /*
            if (monsterData.ShowName)
                actor.m_pkGeActor.SetHeadInfoVisible(false);
            if (monsterData.ShowFootBar)
                actor.m_pkGeActor.SetFootIndicatorVisible(false);
            if (monsterData.ShowHPBar)
                actor.m_pkGeActor.RemoveHPBar();
                */

            TriggerEventNew(BeEventSceneType.onCreateMonster, new EventParam(){m_Obj = actor});
            //TriggerEvent(BeEventSceneType.onCreateMonster, new object[] { actor });
            if (actor != null &&
                actor.GetEntityData() != null &&
                actor.GetEntityData().battleData != null)
            {
                actor.GetEntityData().battleData.initDefence = actor.GetEntityData().battleData.defence;
                actor.GetEntityData().battleData.initMagicDefence = actor.GetEntityData().battleData.magicDefence;
            }
            if (this.state >= BeSceneState.onBulletTime)
            {
                actor.DoDead();
            }
            return actor;
        }
        else
        {
            Logger.LogErrorFormat("UnitTable can't find item with ID : {0}", monsterID);
        }

        return null;
    }

    public Dictionary<int, int> GetMonsterSkillInfo(int monsterID, int externalSkillLevel = 0)
    {
        Dictionary<int, int> skillInfo = new Dictionary<int, int>();

        MonsterIDData mdata = new MonsterIDData(monsterID);
        var monsterData = TableManager.GetInstance().GetTableItem<ProtoTable.UnitTable>(mdata.mid);
        if (monsterData == null)
            monsterData = TableManager.GetInstance().GetTableItem<ProtoTable.UnitTable>(monsterID);

        if (monsterData == null)
            return skillInfo;

        int resid = monsterData.Mode;
        int skillLevel = mdata.level;

        if (externalSkillLevel > 0)
            skillLevel = externalSkillLevel;

        if (monsterData.AttackSkillID != 0)
            skillInfo.Add(monsterData.AttackSkillID, skillLevel);

        for (int i = 0; i < monsterData.SkillIDs.Count; ++i)
        {
            var sid = monsterData.SkillIDs[i];
            if (sid != 0 && !skillInfo.ContainsKey(sid))
            {
                var skillData = TableManager.GetInstance().GetTableItem<ProtoTable.SkillTable>(sid);
                //开放了等级才能携带这个技能
                if (skillData != null &&
                    (mdata.level >= skillData.LevelLimit ||
                        mdata.type == (int)ProtoTable.UnitTable.eType.ACCOMPANY ||
                        monsterData.APCIsSpecialConfig >= 1))
                {
                    skillInfo.Add(sid, skillLevel);

                    //APC技能等级设置上限
                    if (monsterData.APCIsSpecialConfig > 0 && skillData.TopLevel > 0)
                    {
                        skillLevel = Math.Min(skillLevel, skillData.TopLevel);
                    }
                }


            }
        }

        return skillInfo;
    }

    enum PrefixType
    {
        LOW = 0,
        HIGH
    }

    public void DealPrefix(BeActor monster, List<int> prefix, ProtoTable.UnitTable monsterData)
    {
        List<string> lowPrefix = new List<string>();
        string hightPrefix = null;

        for (int i = 0; i < prefix.Count; ++i)
        {
            if (prefix[i] <= 0)
                continue;
            var data = TableManager.GetInstance().GetTableItem<ProtoTable.MonsterPrefixTable>(prefix[i]);
            if (data == null)
                continue;

            //添加真正的buff
            for (int j = 0; j < data.BufferInfoID.Count; ++j)
            {
                int buffInfoID = data.BufferInfoID[j];
                if (buffInfoID > 0)
                {
                    BuffInfoData buffInfo = new BuffInfoData(buffInfoID, monster.GetEntityData().level);
                    if (buffInfo.condition <= BuffCondition.NONE)
                        monster.buffController.TryAddBuff(buffInfo);
                    else
                        monster.buffController.AddTriggerBuff(buffInfo);
                }
            }

            if (data.type == (int)PrefixType.HIGH)
                hightPrefix = data.Name;
            else
                lowPrefix.Add(data.Name);
        }

        string info = "";
        for (int i = 0; i < lowPrefix.Count; ++i)
        {
            info += string.Format("<color=white>{0}\n</color>", lowPrefix[i]);
        }
        if (hightPrefix == null)
            hightPrefix = "";
        if (hightPrefix != null)
        {
            info += string.Format("<color=#8918FFFF>{0}{1}</color>", hightPrefix, monsterData.Name);

            if (monster.GetEntityData() != null)
            {
                monster.GetEntityData().name = string.Format("{0}{1}", hightPrefix, monsterData.Name);
            }
        }
        monster.m_pkGeActor.CreateMonsterInfoBar(info, PlayerInfoColor.PREFIX_MONSTER);

#if !LOGIC_SERVER

        if (Utility.IsStringValid(monsterData.PrefixEffect))
        {
            monster.m_pkGeActor.CreateEffect(monsterData.PrefixEffect, "[actor]Orign", 999999, new Vec3(0, 0, 0), 1, 1, true);
        }

#endif

    }

    public BeActor SummonAccompany(int accompanyID, VInt3 pos, int camp, BeActor owner)
    {
        BeActor actor = SummonMonster(accompanyID, pos, camp, owner, false, 0, false,0,true);
        return actor;
    }

    public BeActor SummonMonster(int monsterID, VInt3 pos, int camp, BeActor owner = null, bool related = false, int summonMonsterSkillLevel = 0, bool isShowBlood = true, int originSummonId = 0, bool forceDisplayModel = false, bool isSameFace = false)
    {
        if (monsterID <= 0)
            return null;

        bool useCube = false;

#if !LOGIC_SERVER
        if (!BattleMain.IsModePvP(mBattle.GetBattleType()) && camp == 0 && owner != null && !forceDisplayModel)
        {
            var data = TableManager.instance.GetTableItem<ProtoTable.SwitchClientFunctionTable>(63);
            if (!data.ValueD.Contains(originSummonId))
            {
                //队友
                if (!owner.isLocalActor)
                {
                    if (SettingManager.instance.GetCommmonSet(SettingManager.STR_SUMMONDISPLAY) == SettingManager.SetCommonType.Close)
                    {
                        useCube = true;
                    }
                }
            }
        }
#endif


        BeActor monster = CreateMonster(monsterID, true, null, summonMonsterSkillLevel, camp, owner, useCube);
        if (monster == null || monster != null && monster.GetEntityData() == null)
            return null;
        monster.m_iCamp = camp;
        monster.GetEntityData().isSummonMonster = true;
        var monsterData = TableManager.GetInstance().GetTableItem<ProtoTable.UnitTable>(monster.GetEntityData().monsterID);
        if (monsterData != null)
        {
            if (monsterData.BornAI == ProtoTable.UnitTable.eBornAI.Start)
            {
                monster.StartAI(null, monster.GetEntityData().type != (int)ProtoTable.UnitTable.eType.ACCOMPANY);
            }
        }
        monster.SetPosition(pos, true);
        if (owner != null)
        {
            if (isSameFace)
            {
                monster.SetFace(owner.GetFace());               //设置召唤出来的怪物朝向和召唤者的朝向一致
            }
            else
            {
                monster.SetFace(!owner.GetFace());               //设置召唤出来的怪物朝向和召唤者的朝向一致
            }

        }

        if (!isShowBlood)
        {
            monster.m_pkGeActor.RemoveHPBarMonster();
        }

        
        if (monsterData != null)
        {
            if (monsterData.FloatValue > 0)
                monster.SetFloating((int)(monsterData.FloatValue * IntMath.kIntDen / GlobalLogic.VALUE_1000), false);

            //设置脚底光环
#if !LOGIC_SERVER

            if ((monster.GetEntityData().type != (int)ProtoTable.UnitTable.eType.SKILL_MONSTER &&
            monster.GetEntityData().type != (int)ProtoTable.UnitTable.eType.EGG) && monsterData.ShowFootBar != 5 || monsterData.ShowFootBar == 1)
            {
                if (BattleMain.IsModePvP(mBattle.GetBattleType()))
                {
                    //蓝色
                    if (owner != null && owner == BattleMain.instance.GetLocalPlayer().playerActor 
                        || (BattleMain.battleType==BattleType.ScufflePVP && owner.m_iCamp == BattleMain.instance.GetLocalPlayer().playerActor.m_iCamp))
                    {
                        monster.m_pkGeActor.CreateFootIndicator("Effects/Hero_Zhaohuanshi/Bingnaisi/Prefab/Eff_Zhaohuanbingnaisi_zhaohuan_02");
                    }
                    //红色
                    else if (owner != null && owner == BattleMain.instance.GetLocalTargetPlayer().playerActor)
                    {
                        monster.m_pkGeActor.CreateFootIndicator("Effects/Hero_Zhaohuanshi/Bingnaisi/Prefab/Eff_Zhaohuanbingnaisi_zhaohuan_03");
                    }
                }
                else
                {
                    //蓝色
                    if (monsterData.ShowFootBar != 2 && monsterData.ShowFootBar != 3)
                        monster.m_pkGeActor.CreateFootIndicator("Effects/Hero_Zhaohuanshi/Bingnaisi/Prefab/Eff_Zhaohuanbingnaisi_zhaohuan_02");
                }
            }

#endif

        }

        if (owner != null)
        {
            //APC怪物，角色
            if (related)
            {
                AdjustSummonMonsterAttribute(owner, monster);
                BeEvent.BeEventHandleNew handle = owner.RegisterEventNew(BeEventType.onDead, eventParam =>
                {
                    if (!monster.IsDead())
                        monster.DoDead();
                });

                monster.RegisterEventNew(BeEventType.onDead, eventParam =>
                {
                    handle.Remove();
                });
            }

            monster.SetOwner(owner);

            if (monster.aiManager.followDistance > 0)
            {
                monster.aiManager.followTarget = owner;
            }

            if (!BattleMain.IsModePvP(mBattle.GetBattleType()) && owner.isLocalActor ||
                BattleMain.IsModePvP(mBattle.GetBattleType()) && owner.isMainActor)
            {
                monster.UseActorData();
            }

#if !SERVER_LOGIC

            if (monster.GetEntityData().type != (int)ProtoTable.UnitTable.eType.SKILL_MONSTER
             && monster.GetEntityData().type != (int)ProtoTable.UnitTable.eType.EGG)
            {
                //只对主角显示等级
                if (owner == BattleMain.instance.GetLocalPlayer().playerActor)
                {
                    string info = string.Format("Lv.{0} {1}", monster.GetEntityData().level, monsterData.Name);
                    monster.m_pkGeActor.CreateMonsterInfoBar(info, PlayerInfoColor.SUMMON_MONSTER);
                }
            }

#endif


        }

        if (monster.m_iCamp != (int)ProtoTable.UnitTable.eCamp.C_HERO)
        {
            _birthMonster(monster, (ProtoTable.UnitTable.eType)monster.GetEntityData().type);
        }

        if(owner != null)
            TriggerEventNew(BeEventSceneType.onSummon, new EventParam(){m_Obj = monster, m_Obj2 = owner, m_Int = owner.GetCurSkillID() });
            //TriggerEvent(BeEventSceneType.onSummon, new object[] { monster,owner, owner.m_iCurSkillID });


        if (owner != null)
        {
            if (BattleMain.IsModePvP(mBattle.GetBattleType()))
            {
                //怪物HP暂时不受天平印象
                //monster.GetEntityData().AdjustHPForPvP(owner.GetEntityData().hpScale);
            }
        }

        //添加无敌buff
        if (monster.GetEntityData().type != (int)ProtoTable.UnitTable.eType.SKILL_MONSTER
        && monster.GetEntityData().type != (int)ProtoTable.UnitTable.eType.EGG)
        {
            monster.buffController.TryAddBuff((int)GlobalBuff.INVINCIBLE, GlobalLogic.VALUE_1000);
        }

        if (mBattle != null && mBattle.recordServer != null && mBattle.recordServer.IsProcessRecord())
        {
            mBattle.recordServer.RecordProcess("[BATTLE]{0} summon {1}", (owner != null ? owner.m_iID.ToString() : ""), monsterID);
            mBattle.recordServer.MarkInt(0x8779800, new int[]
            {
                owner != null ? owner.m_iID : 0,
                monsterID
            });
            // Mark:0x8779800 PID: {0} summon monsterID: {1}
        }

        return monster;
    }

    public BeActor DuplicateMonster(BeActor monster)
    {
        return DuplicateMonster(monster,VFactor.one);
    }
    public BeActor DuplicateMonster(BeActor monster, VFactor percent, int maxNum = 0)
    {
        if (monster == null)
            return null;

        if (monster.GetEntityData() == null)
            return null;

        if (!monster.stateController.CanDuplicate())
            return null;

        var entity = monster.GetEntityData();
        int monsterID = entity.monsterID + entity.level * GlobalLogic.VALUE_100;

        if (maxNum > 0)
        {
            int existNum = monster.CurrentBeScene.GetSummonCountByID(entity.monsterID, monster);
            if (existNum >= maxNum)
                return null;
        }

        BeActor dupliateMonster = CreateMonster(monsterID, false, null, 0, monster.m_iCamp);
        var duplicateEntity = dupliateMonster.GetEntityData();

        duplicateEntity.isSummonMonster = true;

        dupliateMonster.SetOwner(monster);
        dupliateMonster.SetPosition(monster.GetPosition());
        dupliateMonster.StartAI(null);
        dupliateMonster.stateController.SetAbilityEnable(BeAbilityType.DUPLICATE, false);

        int newHP = (entity.GetMaxHP() * percent);
        //duplicateEntity.battleData.hp = ;
        //duplicateEntity.battleData.maxHp = (int)(entity.battleData.maxHp * percent);
        duplicateEntity.SetHP(newHP);
        duplicateEntity.SetMaxHP(newHP);
        duplicateEntity.battleData.attack = (entity.battleData.attack * percent);
        duplicateEntity.battleData.magicAttack = (entity.battleData.magicAttack * percent);

        if (null != dupliateMonster.m_pkGeActor)
        {
            dupliateMonster.m_pkGeActor.ResetHPBar();
        }

        return dupliateMonster;
    }

    public void AdjustSummonMonsterAttribute(BeActor owner, BeActor monster)
    {
        if (owner == null || monster == null)
            return;

        if (monster.attribute.monsterData.MonsterMode == (int)MonsterMode.SUMMON_PVE)
        {
            //召唤师的召唤兽PVP属性读取新怪物的
            var unitDataNew = GetSummonMonsterAttr(monster.attribute.monsterData.ID);
            if (unitDataNew == null)
            {
                unitDataNew = monster.attribute.monsterData;
            }

            monster.attribute.SetAttributeValue(AttributeType.attack, owner.attribute.GetAttributeValue(AttributeType.magicAttack) * VFactor.NewVFactor(TableManager.instance.GetMonsterTableProperty(AttributeType.magicAttack, unitDataNew), GlobalLogic.VALUE_100000));
            monster.attribute.SetAttributeValue(AttributeType.magicAttack, owner.attribute.GetAttributeValue(AttributeType.magicAttack) * VFactor.NewVFactor(TableManager.instance.GetMonsterTableProperty(AttributeType.magicAttack, unitDataNew), GlobalLogic.VALUE_100000));
            monster.attribute.SetAttributeValue(AttributeType.ignoreDefAttackAdd, owner.attribute.GetAttributeValue(AttributeType.ignoreDefMagicAttackAdd) * VFactor.NewVFactor(TableManager.instance.GetMonsterTableProperty(AttributeType.ignoreDefMagicAttackAdd, unitDataNew), GlobalLogic.VALUE_100000));
            monster.attribute.SetAttributeValue(AttributeType.ignoreDefMagicAttackAdd, owner.attribute.GetAttributeValue(AttributeType.ignoreDefMagicAttackAdd) * VFactor.NewVFactor(TableManager.instance.GetMonsterTableProperty(AttributeType.ignoreDefMagicAttackAdd, unitDataNew), GlobalLogic.VALUE_100000));
            monster.attribute.SetAttributeValue(AttributeType.baseAtk, owner.attribute.GetAttributeValue(AttributeType.baseInt) * VFactor.NewVFactor(TableManager.instance.GetMonsterTableProperty(AttributeType.baseInt, unitDataNew), GlobalLogic.VALUE_100000));
            monster.attribute.SetAttributeValue(AttributeType.baseInt, owner.attribute.GetAttributeValue(AttributeType.baseInt) * VFactor.NewVFactor(TableManager.instance.GetMonsterTableProperty(AttributeType.baseInt, unitDataNew), GlobalLogic.VALUE_100000));
            monster.attribute.SetAttributeValue(AttributeType.baseSta, owner.attribute.GetAttributeValue(AttributeType.baseSta) * VFactor.NewVFactor(TableManager.instance.GetMonsterTableProperty(AttributeType.baseSta, unitDataNew), GlobalLogic.VALUE_100000));
            monster.attribute.SetAttributeValue(AttributeType.baseSpr, owner.attribute.GetAttributeValue(AttributeType.baseSpr) * VFactor.NewVFactor(TableManager.instance.GetMonsterTableProperty(AttributeType.baseSpr, unitDataNew), GlobalLogic.VALUE_100000));
            monster.attribute.SetAttributeValue(AttributeType.ciriticalAttack, owner.attribute.GetAttributeValue(AttributeType.ciriticalAttack) * VFactor.NewVFactor(TableManager.instance.GetMonsterTableProperty(AttributeType.ciriticalAttack, unitDataNew), GlobalLogic.VALUE_100000));
            monster.attribute.SetAttributeValue(AttributeType.ciriticalMagicAttack, owner.attribute.GetAttributeValue(AttributeType.ciriticalMagicAttack) * VFactor.NewVFactor(TableManager.instance.GetMonsterTableProperty(AttributeType.ciriticalMagicAttack, unitDataNew), GlobalLogic.VALUE_100000));

            //var monsterMaxHp = owner.attribute.battleData._maxHp * VFactor.NewVFactor(TableManager.instance.GetMonsterTableProperty(AttributeType.maxHp, unitDataNew), GlobalLogic.VALUE_100000);
            monster.attribute.ChangeMaxHpByResist();
            monster.attribute.battleData.RefreshMpInfo();
            if (BattleMain.IsModePvP(mBattle.GetBattleType()))//PVP
            {
                var data = TableManager.GetInstance().GetTableItem<ProtoTable.PkHPProfessionAdjustTable>(monster.attribute.simpleMonsterID);
                if (data != null)
                {
                    if (mBattle.PkRaceType == (int)Protocol.RaceType.ScoreWar)
                    {
                        monster.attribute.AdjustHPForPvP(VRate.Factor(data.factor_3v3).single);
                    }
                    else if (mBattle.PkRaceType == (int)Protocol.RaceType.ChiJi)
                    {
                        monster.attribute.AdjustHPForPvP(VRate.Factor(data.factor_chiji).single);
                    }
                    else
                    {
                        monster.attribute.AdjustHPForPvP(VRate.Factor(data.factor).single);
                    }
                }
            }

#if !LOGIC_SERVER
            if (monster != null && monster.m_pkGeActor != null)
                monster.m_pkGeActor.SyncHPBar();
#endif

            for (int i = 1; i < (int)MagicElementType.MAX; i++)
                monster.attribute.battleData.magicElementsAttack[i] = owner.attribute.battleData.magicElementsAttack[i] * VRate.Factor(GlobalLogic.VALUE_1000);

            if (monster.attribute.simpleMonsterID == 900 || monster.attribute.simpleMonsterID == 905 || monster.attribute.simpleMonsterID == 909)//哥布林、花妖、黑骑士特殊处理
            {
                for (int i = 1; i < (int)MagicElementType.MAX; i++)
                    monster.attribute.battleData.magicELements[i] = owner.attribute.battleData.magicELements[i];
            }
        }
        else
        {
            monster.attribute.SetAttributeValue(AttributeType.attack, owner.attribute.GetAttributeValue(AttributeType.magicAttack));
            monster.attribute.SetAttributeValue(AttributeType.magicAttack, owner.attribute.GetAttributeValue(AttributeType.magicAttack));
            monster.attribute.SetAttributeValue(AttributeType.baseAtk, owner.attribute.GetAttributeValue(AttributeType.baseInt));
            monster.attribute.SetAttributeValue(AttributeType.baseInt, owner.attribute.GetAttributeValue(AttributeType.baseInt));
        }
    }

    public BeActor CreateRemoteMonster(Battle.DungeonMonster remoteData, VInt3 pos, VFactor scale, ISceneEntityInfoData localData = null)
    {
        bool isDead = false;

        BeActor monster = CreateMonster(remoteData.typeId, false, remoteData.prefixes);

        if (null == monster)
        {
            return null;
        }

        bool faceLeft = false;

        ISceneMonsterInfoData monsterData = localData as ISceneMonsterInfoData;
        if (null != monsterData)
        {
            faceLeft = monsterData.GetIsFaceLeft();

            if (monsterData.GetMonsterInfoTableID() > 0)
            {
                var tbData = TableManager.instance.GetTableItem<ProtoTable.MonsterInstanceInfoTable>(monsterData.GetMonsterInfoTableID());
                if (null != tbData)
                {
                    AddBornBuff(monster, tbData.BornBuffIDs, tbData.BornBuffInfoIDs, tbData.BornMechismIDs);
                }
            }

            var ai = monster.aiManager;
            if (null != ai)
            {
                var sight = monsterData.GetSight();
                if (sight != 0)
                {
                    ai.sight = sight;
                    ai.chaseSight = sight * GlobalLogic.VALUE_3;
                }

                var groupIndex = monsterData.GetGroupIndex();
                monster.GetEntityData().groupID = groupIndex;

                var aiActionPath = monsterData.GetAIActionPath();
                if (Utility.IsStringValid(aiActionPath))
                {
                    ai.actionAgent = ai.InitAgent(aiActionPath);
                }
                
                var aiScenarioPath = monsterData.GetAIScenarioPath();
                if (Utility.IsStringValid(aiScenarioPath))
                {
                    ai.scenarioAgent = ai.InitAgent(aiScenarioPath);
                }
            }
        }

        monster.SetPosition(pos, true);
        monster.SetScale(monster.GetScale().i * scale);
        monster.SetFace(faceLeft);
        

        if (monster.floatingHeight > 0)
        {
            monster.SetFloating(monster.floatingHeight, true);
        }

        if (state == BeSceneState.onFinish)
        {
            monster.DoDead();
            monster.sgForceSwitchState(new BeStateData((int)ActionState.AS_FALLGROUND));

            _dropItem(remoteData, monster);
            BattleDataManager.GetInstance().BattleInfo.KillMonster(remoteData.id);
        }
        else
        {
            monster.RegisterEventNew(BeEventType.onDead, eventParam =>
            {
                if (!isDead)
                {
                    isDead = true;
                    remoteData.removed = true;

                    // 掉落
                    _dropItem(remoteData, monster);

                    // onDead 怪物
                    BattleDataManager.GetInstance().BattleInfo.KillMonster(remoteData.id);
                }
            });

            // 设置跟随区域
            _birthMonster(monster, (ProtoTable.UnitTable.eType)monster.GetEntityData().type);
        }

        if (IsTaskMonster(remoteData.typeId))
        {
            monster.m_pkGeActor.SetTaskMonster(monster.attribute.name);
        }

        return monster;
    }

    public BeActor CreateMonster(int monsterId,VInt3 pos)
    {
        BeActor monster = CreateMonster(monsterId);

        if (null == monster)
        {
            return null;
        }
        monster.SetPosition(pos, true);
        if (monster.floatingHeight > 0)
            monster.SetFloating(monster.floatingHeight, true);

        if (state == BeSceneState.onFinish)
        {
            monster.DoDead();
            monster.sgForceSwitchState(new BeStateData((int)ActionState.AS_FALLGROUND));
        }
        else
        {
            // 设置跟随区域
            _birthMonster(monster, (ProtoTable.UnitTable.eType)monster.GetEntityData().type);
        }

        if (IsTaskMonster(monsterId))
        {
            monster.m_pkGeActor.SetTaskMonster(monster.attribute.name);
        }

        return monster;
    }
    public BeActor CreateRemoteMonster(Battle.DungeonMonster remoteData, ISceneEntityInfoData localData)
    {
        return CreateRemoteMonster(remoteData, new VInt3(localData.GetPosition()), VFactor.NewVFactorF(localData.GetScale(),1000), localData);
    }

    private void _dropItem(Battle.DungeonMonster remoteData, BeEntity actor)
    {
        /*
		for(int i=0; i<1; ++i)
		{
			Battle.DungeonDropItem item = new Battle.DungeonDropItem();
			item.id = 0;

			item.num = UnityEngine.Random.Range(1,10) * 100 + 1;
			item.typeId = Global.GOLD_ITEM_ID;
			
			remoteData.dropItems.Add(item);
		}*/


        if (remoteData.dropItems.Count > 0)
        {
            DropItems(remoteData.dropItems, actor.GetPosition());
        }
    }

    public void DropItems(List<Battle.DungeonDropItem> dropList, VInt3 epos, bool needClear = true, bool needTargetList = true, List<BeRegionDropItem> retList = null)
    {
#if !LOGIC_SERVER
        int xrange = logicWidth;
        int yrange = logicHeight;

        epos.z = 0;
        float radius = 1.2f * 2;
        for (int i = 0; i < dropList.Count; ++i)
        {
            var dropItem = dropList[i];
            Vec3 pos = epos.vec3;
            VInt3 dpos = epos;

            if (true || i > 0)
            {
                pos.x = pos.x + UnityEngine.Random.Range(-radius, radius);
                pos.y = pos.y + UnityEngine.Random.Range(-radius, radius);
                pos.z = 0;

                pos.x = Mathf.Clamp(pos.x, logicXSize.fx, logicXSize.fy);
                pos.y = Mathf.Clamp(pos.y, logicZSize.fx, logicZSize.fy);


                dpos = BeAIManager.FindStandPosition(
                    new VInt3(pos), this, i % 2 == 0);
            }

            var newItem = AddDropItem(DungeonUtility.CreateSceneRegionInfoData(4, dpos.vector3), dropItem, epos, needClear, needTargetList);
            if (newItem != null && retList != null)
            {
                retList.Add(newItem);
            }
        }
        if(needClear)
            dropList.Clear();
#endif
    }

    private void _dropMonster(Battle.DungeonMonster monster, ISceneEntityInfoData info)
    {
        if (null != monster.summonerMonsters)
        {
            for (int i = 0; i < monster.summonerMonsters.Count; ++i)
            {
                var newMonster = monster.summonerMonsters[i];
                var actor = CreateRemoteMonster(newMonster, info);

                if (actor.GetEntityData().type != (int)ProtoTable.UnitTable.eType.EGG
                && actor.GetEntityData().type != (int)ProtoTable.UnitTable.eType.SKILL_MONSTER)
                {
                    actor.buffController.TryAddBuff((int)GlobalBuff.INVINCIBLE, GlobalLogic.VALUE_1000);
                }
            }

            monster.summonerMonsters = null;

            if (state != BeSceneState.onFinish)
            {
                state = BeSceneState.onReady;
            }
        }
    }

    public BeObject CreateDestruct(int resid, VInt3 pos, Color color, VInt scale)
    {
        var dataItem = TableManager.instance.GetTableItem<ProtoTable.DestrucTable>(resid);
        if (null != dataItem)
        {
            var destruct = AddLogicObject(dataItem.Mode);
            if (null != destruct)
            {
                destruct.SetPosition(pos);
                destruct.SetScale(scale);
                destruct.SetBlockLayer(true);
                destruct.SetSplitCount(dataItem.IdleSplitCount);
                destruct.SetMaxStage(dataItem.IdleCount + 1);
                destruct.SetDeadEffect(dataItem.DeadEffect);
                destruct.SetDamageCount(dataItem.DestructHitCount);
                destruct.SetBlockPaths(dataItem.BlockPaths);
                destruct.beHitEffect = dataItem.Hurt;

#if !LOGIC_SERVER
                destruct.m_pkGeActor.SetDyeColor(color, destruct.m_pkGeActor.renderObject);
#endif

                if (!dataItem.IsDestruct)
                {
                    destruct.SetCanBeBreak(false);
                }

                return destruct;
            }
            else
            {
                Logger.LogErrorFormat("don't contain the id with {0}, Model {1}", resid, dataItem.Mode);
            }
        }

        return null;
    }

    public BeObject CreateDestruct(ISceneEntityInfoData info)
    {
        return CreateDestruct(info.GetResid(), new VInt3(info.GetPosition()), info.GetColor(), VInt.Float2VIntValue(info.GetScale()));
    }

    private BeObject _createRemoteDestruct(Battle.DungeonMonster monster, ISceneEntityInfoData info)
    {
        if (null != monster && monster.id > 0)
        {
            var destruct = CreateDestruct(info);
            if (null != destruct && null != monster)
            {
                destruct.RegisterEventNew(BeEventType.onDead, eventParam =>
                {
                    monster.id = -1;
                    //掉落
                    _dropItem(monster, destruct);

                    if (monster.summonerMonsters != null && monster.summonerMonsters.Count > 0)
                    {
                        _setTransportDoor(false);
                    }
                });

                if (null != monster.summonerMonsters && monster.summonerMonsters.Count > 0)
                {
                    destruct.RegisterEventNew(BeEventType.onRemove, args =>
                    {
                        // 爆出来的怪物
                        _dropMonster(monster, info);
                    });
                }

                return destruct;
            }
            else
            {
                Logger.LogWarningFormat("don't contain the id with {0}", info.GetResid());
            }
        }

        return null;
    }

    public int CreateMonsterList(IList<Battle.DungeonMonster> monsters, bool isBoss, VInt3 birthPos, bool needClear = true)
    {
        mIsBossScene = isBoss;

        int monsterCreatedCount = 0;
        //localList = sceneData._monsterinfo;
        IList<Battle.DungeonMonster> remoteList = monsters;

        if (remoteList == null)
            return 0;

        Logger.LogProcessFormat("[创建怪物] 本地怪物数目{0}, 服务器怪物数目{1}", sceneData.GetMonsterInfoLength(), remoteList.Count);

        //Logger.Log(ObjectDumper.Dump(localList));
        Logger.Log(ObjectDumper.Dump(remoteList));

        for (int i = 0; i < remoteList.Count; ++i)
        {
            var remoteData = remoteList[i];

            ISceneEntityInfoData localData = null;

            for (int k = 0; k < sceneData.GetMonsterInfoLength(); ++k)
            {
                ISceneMonsterInfoData monsterInfoData = sceneData.GetMonsterInfo(k);

                monsterInfoData.SetMonsterID(monsterInfoData.GetEntityInfo().GetResid());

                if (k == remoteData.pointId % GlobalLogic.VALUE_100)
                {
                    localData = monsterInfoData.GetEntityInfo();
                    break;
                }
            }

            if (null != localData)
            {
                if (!remoteData.removed)
                {
                    Logger.LogProcessFormat("[创建怪物] 创建 {0}", remoteData.typeId);

					remoteData.monsterType = localData.GetType();

                    //var monster = CreateRemoteMonster(remoteData, localData, VInt.Float2VIntValue(localData.GetPosition().x) > birthPos.x);
                    var monster = CreateRemoteMonster(remoteData, localData);

                    if (monster != null)
                    {
                        if (monster.GetRecordServer().IsProcessRecord())
                        {
                            monster.GetRecordServer().RecordProcess("[CreateBirthPos] create hell monster {0} {1} dataPos{2} monsterPos{3}", monster.GetName(), monster.m_iID, localData.GetPosition(), monster.GetPosition());
                            monster.GetRecordServer().MarkString(0x8779801, monster.GetName(), monster.m_iID.ToString(), localData.GetPosition().ToString(), monster.GetPosition());
                            // Mark:0x8779801 create hell monster PID:{1}-{0} dataPos{2} monsterPos{3}
                        }

                        if (!monster.IsSkillMonster())
                        {
                            monsterCreatedCount++;
                        }
                    }
                }
            }
        }

        // 检查此处剩余怪物的id
        for (int i = 0; i < remoteList.Count; ++i)
        {
            if (remoteList[i].summonerId <= 0)
            {
                Logger.LogWarningFormat("local data is different with remote data, id {0}, resid {1}", remoteList[i].id, remoteList[i].typeId);
            }
        }

        // 创建影响过关的可破坏物
        // 

        if (needClear && monsterCreatedCount <= 0)
        {
            _onClearEvent();
            _updateState(0);
        }

        return monsterCreatedCount;
    }
    public void ClearEventAndState()
    {
        _onClearEvent();
        _updateState(0);
    }

    public int CreateMonsterList(IList<Battle.DungeonMonster> monsters, bool isBoss, VInt3 birthPos, ref BeActor boss)
    {
        mIsBossScene = isBoss;
        boss = null;
        int monsterCreatedCount = 0;
        //localList = sceneData._monsterinfo;
        IList<Battle.DungeonMonster> remoteList = monsters;

        Logger.LogProcessFormat("[创建怪物] 本地怪物数目{0}, 服务器怪物数目{1}", sceneData.GetMonsterInfoLength(), remoteList.Count);

        //Logger.Log(ObjectDumper.Dump(localList));
        Logger.Log(ObjectDumper.Dump(remoteList));

        for (int i = 0; i < remoteList.Count; ++i)
        {
            var remoteData = remoteList[i];

            ISceneEntityInfoData localData = null;

            for (int k = 0; k < sceneData.GetMonsterInfoLength(); ++k)
            {
                ISceneMonsterInfoData monsterInfoData = sceneData.GetMonsterInfo(k);

                monsterInfoData.SetMonsterID(monsterInfoData.GetEntityInfo().GetResid());

                if (k == remoteData.pointId % GlobalLogic.VALUE_100)
                {
                    localData = monsterInfoData.GetEntityInfo();
                    break;
                }
            }

            if (null != localData)
            {
                if (!remoteData.removed)
                {
                    Logger.LogProcessFormat("[创建怪物] 创建 {0}", remoteData.typeId);

                    remoteData.monsterType = localData.GetType();

                    //var monster = CreateRemoteMonster(remoteData, localData, VInt.Float2VIntValue(localData.GetPosition().x) > birthPos.x);
                    var monster = CreateRemoteMonster(remoteData, localData);
                    if (monster != null && monster.IsBoss())
                    {
                        boss = monster;
                    }
                    if (monster != null && monster.GetRecordServer().IsProcessRecord())
                    {
                        monster.GetRecordServer().RecordProcess("[CreateBirthPos] create hell monster {0} {1} dataPos{2} monsterPos{3}", monster.GetName(), monster.m_iID, localData.GetPosition(), monster.GetPosition());
                        monster.GetRecordServer().MarkString(0x8779802, monster.GetName(), monster.m_iID.ToString(), localData.GetPosition().ToString(), monster.GetPosition());
                        // Mark:0x8779802 create hell monster {0} {1} dataPos{2} monsterPos{3}
                    }

                    if (monster != null && !monster.IsSkillMonster())
                    {
                        monsterCreatedCount++;
                    }
                }
            }
        }

        // 检查此处剩余怪物的id
        for (int i = 0; i < remoteList.Count; ++i)
        {
            if (remoteList[i].summonerId <= 0)
            {
                Logger.LogWarningFormat("local data is different with remote data, id {0}, resid {1}", remoteList[i].id, remoteList[i].typeId);
            }
        }

        // 创建影响过关的可破坏物
        // 

        if (monsterCreatedCount <= 0)
        {
            _onClearEvent();
            _updateState(0);
        }

        return monsterCreatedCount;
    }
    public void CreateDestructList2(List<Battle.DungeonMonster> destructs)
    {
        Logger.LogProcessFormat("area {0}", ObjectDumper.Dump(destructs));

        if (null == sceneData)
        {
            return;
        }

        List<Battle.DungeonMonster> remoteList = destructs;
        int offset = sceneData.GetMonsterInfoLength();

        for (int i = 0; i < sceneData.GetDestructibleInfoLength(); ++i)
        {
            var findItem = remoteList.Find(x =>
            {
                return (x.pointId % GlobalLogic.VALUE_100) == (offset + i) && x.typeId == sceneData.GetDestructibleInfo(i).GetEntityInfo().GetResid();
            });

            if (null != findItem)
            {
                // create remote
                _createRemoteDestruct(findItem, sceneData.GetDestructibleInfo(i).GetEntityInfo());
            }
        }
    }

    public void SetMonsterListTarget(BeActor target)
    {
        for (int i = 0; i < mEntitys.Count; i++)
        {
            var entity = mEntitys[i] as BeActor;
            if (entity != null && entity.hasAI && entity.m_iCamp != (int)ProtoTable.UnitTable.eCamp.C_HERO)
            {
                entity.UpdateAITarget(target);
            }
        }
    }

    public void CreateMonsterLocal(BeActor target)
    {
        if (target == null)
        {
            Logger.LogError("target is nil");
            return;
        }
#if DEBUG_SETTING
        if (Global.Settings.isCreateMonsterLocal)
        {
            //var monsterList = sceneData._monsterinfo;
            for (int i = 0; i < sceneData.GetMonsterInfoLength(); i++)
            {
                int resid = sceneData.GetMonsterInfo(i).GetEntityInfo().GetResid();
                var pos = sceneData.GetMonsterInfo(i).GetEntityInfo().GetPosition();
                var enemy = CreateMonster(resid);
                enemy.StartAI(target);
                enemy.SetPosition(new VInt3(pos));
                enemy.SetScale((VInt)sceneData.GetMonsterInfo(i).GetEntityInfo().GetScale());
            }
        }
#endif
    }
#endregion

#region Player
    private void _createCharacterUI(BeActor actor, int playerSeat)
    {
        if (actor.hasHP)
        {
            switch (BattleMain.battleType)
            {
                case BattleType.MutiPlayer:
                case BattleType.GuildPVP:
                case BattleType.MoneyRewardsPVP:
                case BattleType.ChijiPVP:
                    break;
                case BattleType.Single:
                case BattleType.Dungeon:
                case BattleType.Mou:
                case BattleType.FinalTestBattle:
                case BattleType.DeadTown:
                case BattleType.North:
                case BattleType.NewbieGuide:
                case BattleType.Hell:
                case BattleType.YuanGu:
                case BattleType.GoldRush:
                case BattleType.ChampionMatch:
                case BattleType.GuildPVE:
                case BattleType.TrainingPVE:
                case BattleType.RaidPVE:
                case BattleType.TreasureMap:
                case BattleType.BattlegroundPVE:
                case BattleType.InputSetting:
                case BattleType.ChangeOccu:
                    {
                        actor.m_pkGeActor.CreateHPBarCharactor(playerSeat);
                    }
                    break;
            }
        }
    }

    public BeActor CreateCharacter(
        bool isLocalActor,
        int professtionID,
        int level,
        int camp,
        Dictionary<int, int> skillInfo,
        List<ItemProperty> equips = null,
        List<Battle.DungeonBuff> buffList = null,
        int playerSeat = 0,
        string name = "",
        int strengthenLevel = 0,
        List<BuffInfoData> rankBuffList = null,//段位
        PetData petData = null,
        List<ItemProperty> sideEquips = null,
        Dictionary<int, string> avatar = null,
        bool isShowWeapon = false,
        bool isAIRobot = false
    )
    {
        var jobData = TableManager.GetInstance().GetTableItem<ProtoTable.JobTable>(professtionID);
        BeActor actor = AddActor(jobData.Mode, camp);
        if (actor != null)
        {

            //actor.defaultWeaponTag = jobData.DefaultWeaponTag;
            actor.SetDefaultWeapenTag(jobData.DefaultWeaponTag);
            actor.SetDefaultWeapenType(jobData.DefaultWeaponType);
            actor.isLocalActor = isLocalActor;

            actor.InitData(
                level,
                jobData.FightID,
                skillInfo,
                "Data/AI/Hero_Skill_AI",
                professtionID,
                equips,
                buffList,
                strengthenLevel,
                rankBuffList,
                petData,
                sideEquips,
                avatar,
                isShowWeapon,
                isAIRobot
            );
           // actor.professionID = professtionID;
            actor.boxRadius =  VInt.NewVInt(jobData.DefaultBoxRadius,GlobalLogic.VALUE_1000);

            actor.walkAction = Global.Settings.walkAction;
            actor.runAction = Global.Settings.runAction;
            actor.walkSpeed = new VInt3(Global.Settings.walkSpeed);
            actor.runSpeed = new VInt3(Global.Settings.runSpeed);
            actor.walkSpeedFactor =  Global.Settings.walkAniFactor;
            actor.runSpeedFactor = Global.Settings.runAniFactor;

            if (petData != null && petData.id > 0)
            {

            }

            var entityData = actor.GetEntityData();
            if (entityData != null)
            {
                entityData.weight = jobData.Weight * (int)IntMath.kIntDen;
                entityData.type = (int)ProtoTable.UnitTable.eType.HERO;
                //if (entityData.normalAttackID <= 0)
                //    entityData.normalAttackID = jobData.NormalAttackID;
                entityData.jumpAttackID = jobData.JumpAttackID;
                entityData.runAttackID = jobData.RunAttackID;
                entityData.jumpAttackCount = jobData.JumpAttackNum;
                entityData.name = name;
                entityData.height = jobData.Height;


                actor.RegisterEventNew(BeEventType.onTouchGround, eventParam =>
                {
                    actor.jumpAttackUseCount = 0;
                });

                actor.RegisterEventNew(BeEventType.onCastSkill, args =>
                {
                    if (actor.GetCurSkillID() == actor.GetEntityData().jumpAttackID)
                    {
                        actor.jumpAttackUseCount++;
                    }
                });
            }

            actor.hitEffect = jobData.DefaultHitEffect;

            _createCharacterUI(actor, playerSeat);

            return actor;
        }
        return null;
    }
#endregion

#region Region
    public void CreateRegions(BeRegionBase.TriggerTarget target, List<int> regions)
    {
        if (target == null)
        {
            target = _regionTargetList;
        }

        for (int i = 0; i < sceneData.GetRegionInfoLength(); i++)
        {
            ISceneRegionInfoData regionInfo = sceneData.GetRegionInfo(i);

            if (!regions.Contains(regionInfo.GetEntityInfo().GetGlobalid()))
            {
                var region = AddRegion(regionInfo, target);
                if (region != null)
                {
                    region.triggerRegion = (ISceneRegionInfoData info2, BeRegionTarget target2) =>
                    {
                        regions.Add(info2.GetEntityInfo().GetGlobalid());
                        return true;
                    };
                }
            }
        }
    }

    public void CreateDynamicRegion(BeRegionBase.TriggerTarget target, CustomSceneRegionInfo regionInfo, List<int> regions)
    {
        if (target == null)
        {
            target = _regionTargetList;
        }
        if (!regions.Contains(regionInfo.GetGlobalid()))
        {
            var region = AddCustomRegion(regionInfo, target);
            if (region != null)
            {
                region.triggerRegion = (ISceneRegionInfoData info2, BeRegionTarget target2) =>
                {
                    regions.Add(info2.GetEntityInfo().GetGlobalid());
                    return true;
                };
            }
        }
    }
    public BeRegion AddCustomRegion(CustomSceneRegionInfo info, BeRegionBase.TriggerTarget target)
    {
        BeRegion region = new BeRegion();
        region.SetBeScene(this);
        region.Create(info);
        region.SetPosition(info.GetLogicPosition());
        region.SetScale((VInt)info.GetEntityInfo().GetScale());

        region.triggerTarget = target;

        if (region != null)
            mPendingRegion.Add(region);

        return region;
    }
    public void onEnterEndRoom()
    {
        _setTransportDoor(true);
    }
    /*
    public void CreateTransportDoor(bool[] connect, BeRegionBase.TriggerTarget target, BeRegionBase.TriggerRegion callback = null, bool[] isboss = null)
    {
        if (connect.Length < 4)
        {
            Logger.LogError("require bool[4]");
            return;
        }

        if (isboss == null)
        {
            isboss = new bool[4] { false, false, false, false };
        }

        //var door = mSceneData._transportdoor;
        for (int i = 0; i < sceneData.GetTransportDoorLength(); i++)
        {
            ISceneTransportDoorData curDoor = sceneData.GetTransportDoor(i);

            if (connect[(int)curDoor.GetDoortype()])
            {
                AddTransportDoor(curDoor.GetRegionInfo(), target, callback, isboss[(int)curDoor.GetDoortype()]);
            }
        }
    }
	*/
    #endregion

    #region Decorator
    public void CreateDecorator()
    {
        for (int i = 0; i < sceneData.GetDecoratorInfoLenth(); i++)
        {
            ISceneDecoratorInfoData decoratorInfo = sceneData.GetDecoratorInfo(i);
            var actor = mCurrentGeScene.CreateActor(decoratorInfo.GetEntityInfo().GetResid(), 0, 0, true);
            if (actor != null)
            {
                mDecoratorList.Add(actor);
                actor.SetPosition(decoratorInfo.GetEntityInfo().GetPosition());
                actor.SetScaleV3(decoratorInfo.GetLocalScale());
                actor.SetRotation(decoratorInfo.GetRotation());
#if !LOGIC_SERVER
                actor.SetDyeColor(decoratorInfo.GetEntityInfo().GetColor(), actor.renderObject);
#endif
                SetBlockLayer(decoratorInfo.GetEntityInfo().GetResid(),
                    new VInt3(decoratorInfo.GetEntityInfo().GetPosition().x,
                             decoratorInfo.GetEntityInfo().GetPosition().z,
                             decoratorInfo.GetEntityInfo().GetPosition().y), true);
            }
        }
    }
#endregion

#endregion

#region FixUnityAttribute


    public void InitMonsterAttribute(BeEntityData attribute, ProtoTable.UnitTable monsterData, int mode, int difficulty, int type, int level)
    {
        if (mode == (int)MonsterMode.SUMMON_PVE)
        {
            type = (int)monsterData.Type;
            if (BattleMain.IsModePvP(mBattle.GetBattleType()))
                mode = (int)MonsterMode.SUMMON_PVP;
        }

        var data = (ProtoTable.MonsterAttributeTable)TableManager.GetInstance().GetMonsterAttribute(mode, difficulty, type, level);
        if (data != null)
        {
            for (int i = 0; i < (int)AttributeType.hpGrow; ++i)
            {
                AttributeType at = (AttributeType)i;
                int value = TableManager.GetInstance().GetMonsterAttributeTableProperty(at, data); //attribute.GetProperty(at, data.GetType(), data);
                if (value != 0)
                {
                    attribute.SetAttributeValue(at, value);
                }
            }
        }
    }
    //
    public void FixUnitAttribute(BeEntityData attribute, ProtoTable.UnitTable fixUnitData, int camp)
    {

        ProtoTable.UnitTable unitData = null;

        //召唤师的召唤兽PVP属性读取新怪物的
        var unitDataNew = GetSummonMonsterAttr(fixUnitData.ID);
        if(unitDataNew != null)
        {
            unitData = unitDataNew;
        }
        else
        {
            unitData = fixUnitData;
        }

        //属性攻击效果，属性，属抗
        attribute.SetMagicElementTypes(unitData.Elements);
        ProtoTable.UnionCell[] elementAttacks = new ProtoTable.UnionCell[]
        {
            unitData.LightAttack, unitData.FireAttack, unitData.IceAttack, unitData.DarkAttack
        };

        ProtoTable.UnionCell[] elementDefences = new ProtoTable.UnionCell[]
        {
                    unitData.LightDefence, unitData.FireDefence, unitData.IceDefence, unitData.DarkDefence
        };

        for(int i=1; i<(int)MagicElementType.MAX; ++i)
        {
            attribute.battleData.magicElementsAttack[i]     += TableManager.GetValueFromUnionCell(elementAttacks[i - 1], attribute.level);
            attribute.battleData.magicElementsDefence[i]    += TableManager.GetValueFromUnionCell(elementDefences[i - 1], attribute.level);
        }

        //异抗
        ProtoTable.UnionCell[] cells = new ProtoTable.UnionCell[]
        {
           unitData.abnormalResist1,
           unitData.abnormalResist2,
           unitData.abnormalResist3,
           unitData.abnormalResist4,
           unitData.abnormalResist5,
           unitData.abnormalResist6,
           unitData.abnormalResist7,
           unitData.abnormalResist8,
           unitData.abnormalResist9,
           unitData.abnormalResist10,
           unitData.abnormalResist11,
           unitData.abnormalResist12,
           unitData.abnormalResist13,
        };

        for (int i = 0; i < Global.ABNORMAL_COUNT; ++i)
        {
            int value = TableManager.GetValueFromUnionCell(cells[i], attribute.level);
            if (value != 0)
                attribute.battleData.abnormalResists[i] += value;
        }


        //attribute.battleData.DebugPrint();
        for (int i = 0; i < (int)AttributeType.hpGrow; ++i)
        {
            AttributeType at = (AttributeType)i;

            int value = TableManager.GetInstance().GetMonsterTableProperty(at, unitData);//attribute.GetProperty(at, unitData.GetType(), unitData);
                                                                                         //1000 * 100
            if (value != GlobalLogic.VALUE_100000)
            {
                int curValue = attribute.GetAttributeValue(at);
                int newValue = curValue * VFactor.NewVFactor(value, (long)(GlobalLogic.VALUE_1000 * GlobalLogic.VALUE_100));
                attribute.SetAttributeValue(at, newValue);
            }
        }
        attribute.PostInit();


        //怪物血量固定值
        int maxFixHp = TableManager.GetValueFromUnionCell(unitData.maxFixHp, attribute.level);
        if (maxFixHp > 0)
        {
            //attribute.battleData.hp = unitData.maxFixHp;
            //attribute.battleData.maxHp = unitData.maxFixHp;
            //固定血量的怪物体力不受外界影响
            if (attribute.battleData != null)
            {
                attribute.battleData.SetNeedChangeSta(false);
            }
            
            attribute.SetHP(maxFixHp);
            attribute.SetMaxHP(maxFixHp);
        }


        //组队难度调整
        var dungeonID = mBattle.dungeonManager.GetDungeonDataManager().id.dungeonID;
        int playerNum = mBattle.dungeonPlayerManager.GetAllPlayers().Count;

        if (camp != 0)
        {
            var difficultyAdjustData = (ProtoTable.DungeonDifficultyAdjustTable)TableManager.GetInstance().GetDungeonDifficultyAdjustInfo(dungeonID, playerNum);
            if (difficultyAdjustData != null)
            {
                VFactor hpFactor = new VFactor(difficultyAdjustData.MonsterHPFactor,GlobalLogic.VALUE_1000);
                VFactor attackFacotr = new VFactor(difficultyAdjustData.MonsterAttackFactor,GlobalLogic.VALUE_1000);
                if (unitData.Type == ProtoTable.UnitTable.eType.BOSS)
                {
                    hpFactor = new VFactor(difficultyAdjustData.BossHPFactor,GlobalLogic.VALUE_1000);
                    attackFacotr = new VFactor(difficultyAdjustData.BossAttackFactor,GlobalLogic.VALUE_1000);
                }

                if (hpFactor <= VFactor.NewVFactor(GlobalLogic.VALUE_1000,(long)GlobalLogic.VALUE_1000))
                {
                    hpFactor = VFactor.NewVFactor(GlobalLogic.VALUE_1000,(long)GlobalLogic.VALUE_1000);
                }
                //Logger.LogErrorFormat("hpFactor={0}", hpFactor);


                attribute.SetMaxHP(attribute.GetMaxHP() * hpFactor);
                attribute.SetHP(attribute.GetMaxHP());
                //attribute.battleData.maxHp = (int)(attribute.battleData.maxHp * hpFactor);
                //attribute.battleData.hp = attribute.battleData.maxHp;

                attribute.battleData.attack = Mathf.Max(1, (attribute.battleData.attack * attackFacotr));
            }
        }

        //attribute.battleData.DebugPrint();
        if (attribute.GetHP() <= 0)
        {
            isBattleDataError = true;
        }
#if DEBUG_SETTING
		if (Global.Settings.isDebug)
		{
			if (Global.Settings.monsterHP > 0)
			{
				attribute.SetHP(Global.Settings.monsterHP);
				attribute.SetMaxHP(Global.Settings.monsterHP);
			}
		}
#endif
    }



    /// <summary>
    /// 在PVP中召唤兽的属性读取新的怪物表ID
    /// </summary>
    private ProtoTable.UnitTable GetSummonMonsterAttr(int originMonsterId)
    {
        var eventData = TriggerEventNew(BeEventSceneType.onChangeSummonMonsterAttr, new EventParam(){m_Int = originMonsterId});
        //int[] originMonsterIdArr = new int[1] { originMonsterId };
        //TriggerEvent(BeEventSceneType.onChangeSummonMonsterAttr, new object[] { originMonsterIdArr });
        if (eventData.m_Int == originMonsterId)
            return null;

        var monsterDataNew = TableManager.GetInstance().GetTableItem<ProtoTable.UnitTable>(eventData.m_Int);
        if (monsterDataNew == null)
        {
            Logger.LogErrorFormat("创建怪物失败，表中找不到ID为:{0}的怪物", eventData.m_Int);
            return null;
        }
        else
        {
            return monsterDataNew;
        }
    }

    public void ChangeMonsterAbility(BeActor monster, ProtoTable.UnitTable unitData, bool isSetIn = true)
    {
        for (int i = 0; i < unitData.AbilityChange.Count; ++i)
        {
            BeAbilityType at = (BeAbilityType)(int)unitData.AbilityChange[i];
            monster.stateController.SetAbilityEnable(at, !isSetIn);
            monster.stateController.SetBornAbility(at, !isSetIn);
        }
    }

    public void SetMonsterAiInfo(BeActor monster, ProtoTable.UnitTable data)
    {

        if (data != null)
        {
            BeAIManager aiManager = new BeActorAIManager();

            monster.InitAI(aiManager);

            if (aiManager != null)
            {
                aiManager.SetAIInfo(data);

#if UNITY_EDITOR && !LOGIC_SERVER
                var root = monster.m_pkGeActor.GetEntityNode(GeEntity.GeEntityNodeType.Root);
                var actorDebugInfo = root.AddComponent<ComActorInfoDebug>();
                actorDebugInfo.actor = monster;
#endif
            }

            monster.skillController.SetNeedCost(false);
        }
    }

    public void SetMonsterProtect(BeActor monster, ProtoTable.UnitTable data)
    {
        if (data.UseProtect > 0 && monster.protectManager != null)
        {
            VRate floatHurtPercent = data.ProtectFloatPercent > 0 ?  new VRate(data.ProtectFloatPercent) : (VRate)Global.Settings.defaultFloatHurt;
            VRate floatHurtPercent2 = data.ProtectFloatPercent2 > 0 ? new VRate(data.ProtectFloatPercent2) : (VRate)Global.Settings.defaultFloatLevelHurat;
            VRate groundHurtPercent = data.ProtectGroundPercent > 0 ? new VRate(data.ProtectGroundPercent) : (VRate)Global.Settings.defaultGroundHurt;
            VRate standHurtPercent = data.ProtectStandPercent > 0 ? new VRate(data.ProtectStandPercent) : (VRate)Global.Settings.defaultStandHurt;

            monster.protectManager.SetValue(floatHurtPercent, floatHurtPercent2, groundHurtPercent, standHurtPercent);

            monster.protectManager.SetEnable(true);
        }
    }


    private void AddBornBuff(BeActor monster, IList<int> buff, IList<int> buffInfo, IList<int> mechism)
    {
        if (null == monster)
        {
            return;
        }

        monster.delayCaller.DelayCall(30, () =>
        {
            //1
            _MonsterAddBornBuff(monster, buff);
        
            //2
            _MonsterAddBornBuffInfo(monster, buffInfo);

            //机制
            _MonsterAddBornMechanism(monster, mechism);
        });

    }

    private void _MonsterAddBornBuff(BeActor monster, IList<int> bornBuffs)
    {
        if (null == monster || null == bornBuffs)
        {
            return ;
        }

        int cnt = bornBuffs.Count;
        for (int i = 0; i < cnt; ++i)
        {
            int buffID = bornBuffs[i];
            if (buffID > 0)
            {
                int monsterLevel = monster.GetEntityData().level;
                monster.buffController.TryAddBuff(buffID, -1, monsterLevel);          //处理异抗问题将出生Buff等级默认设为怪物等级
            }
        }
    }

    private void _MonsterAddBornBuffInfo(BeActor monster, IList<int> bornBuffInfos)
    {
        if (null == monster || null == bornBuffInfos)
        {
            return ;
        }

        int cnt = bornBuffInfos.Count;

        for (int i = 0; i < cnt; ++i)
        {
            int buffInfoID = bornBuffInfos[i];
            if (buffInfoID > 0)
            {
                BuffInfoData buffInfo = new BuffInfoData(buffInfoID, monster.GetEntityData().level);
                if (buffInfo.condition <= BuffCondition.NONE)
                {
                    monster.buffController.TriggerBuffInfo(buffInfo);
                }
                else
                {
                    monster.buffController.AddTriggerBuff(buffInfo);
                }
            }
        }
    }

    private void _MonsterAddBornMechanism(BeActor monster, IList<int> bornMechanisms)
    {
        if (null == monster || null == bornMechanisms)
        {
            return ;
        }

        int cnt = bornMechanisms.Count;
        for (int i = 0; i < cnt; ++i)
        {
            var mid = bornMechanisms[i];
            if (mid > 0)
            {
                monster.AddMechanism(mid, monster.GetEntityData().level);
            }
        }
    }


#endregion

    public void ForceDoDeadEntityByOwner(BeEntity owner)
    {
        for (int i = 0; i < mEntitys.Count; ++i)
        {
            if (null == mEntitys[i])
            {
                continue;
            }

            if (null != mEntitys[i].GetEntityData() && mEntitys[i].GetEntityData().isPet)
            {
                continue;
            }

            if (mEntitys[i] != owner && mEntitys[i].GetOwner() == owner)
            {
                mEntitys[i].DoDead();
            }
        }
    }

#region Entity Count
    /// <summary>
    /// 该函数已被弃用  不建议继续使用
    /// </summary>
    /// <returns></returns>
    public List<BeEntity> GetEntities()
    {
        return mEntitys;
    }

    //获取实体列表  考虑到实体晚一帧添加进Entitys里面的情况
    public void GetEntitys2(List<BeEntity> entityList)
    {
        entityList.AddRange(mEntitys);
        if (mPendingArray.Count <= 0)
            return;
        for(int i=0;i< mPendingArray.Count; i++)
        {
            if (entityList.Contains(mPendingArray[i]))
                continue;
            entityList.Add(mPendingArray[i]);
        }
    }

    public List<BeEntity> GetFullEntities()
    {
        ForceUpdatePendingArray();
        return mEntitys;
    }


    public List<BeEntity> GetPendingEntities()
    {
        return mPendingArray;
    }

    public int GetEntityCount()
    {
        return mEntitys.Count;
    }

    public BeEntity GetEntityAt(int iIndex)
    {
        return mEntitys[iIndex];
    }
#endregion

#region Update
    public void Update(int iDeltaTime)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeScene.Update"))
        {
#endif
        _updateState(iDeltaTime);

        switch (state)
        {
            case BeSceneState.onPause:
                //Logger.LogErrorFormat("bescene onPause t:{0}", Time.realtimeSinceStartup);
                break;
            case BeSceneState.onReady:
            case BeSceneState.onClear:
            case BeSceneState.onFight:
            case BeSceneState.onFinish:
            case BeSceneState.onBulletTime:

                /////////////////////////////////////////


                ////////////////////////////////////////
                mDurtime += iDeltaTime;
                UpdateDelayCaller(iDeltaTime);
                UpdateEntities(iDeltaTime);
                UpdateRegions(iDeltaTime);
                if (mBattle != null && mBattle.LevelMgr != null)
                {
                    mBattle.LevelMgr.Update(iDeltaTime);
                }
#if !LOGIC_SERVER
                if (state == BeSceneState.onClear)
                {
                    UpdateShowArrowForDoor(iDeltaTime);
                }
#endif

                UpdateTimer(iDeltaTime);


                break;
        }

#if ENABLE_PROFILER
        }
#endif
    }

    public void UpdateCheckGlobalLogicValues(int deltaTime)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeScene.UpdateCheckGlobalLogicValues"))
        {
#endif
        if (isBattleDataError)
            return;

        checkBattleDataErrorAcc += deltaTime;
        if (true || checkBattleDataErrorAcc >= GlobalLogic.VALUE_20000)
        {
            //checkBattleDataErrorAcc -= GlobalLogic.VALUE_20000;

            if (GlobalLogic.GetTotalSum() != 253400)
            {
                isBattleDataError = true;

            }


        }
#if ENABLE_PROFILER
        }
#endif
    }

    public void UpdateGraphic(int iDeltaTime)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeScene.UpdateGraphic"))
        {
#endif

#if !LOGIC_SERVER
        switch (state)
        {
            case BeSceneState.onPause:
			//Logger.LogErrorFormat("bescene onPause t:{0}", Time.realtimeSinceStartup);
                break;
            case BeSceneState.onReady:
            case BeSceneState.onClear:
            case BeSceneState.onFight:
            case BeSceneState.onFinish:
            case BeSceneState.onBulletTime:
                //写死boss死亡5秒慢动作动画（死亡特效就是5秒时长）
            if (state == BeSceneState.onBulletTime && ReplayServer.GetInstance() != null && !ReplayServer.GetInstance().IsReplay())
            {
                var curTime = Utility.GetTimeStamp();
                if (bossDeadTimeStamp > 0.0 && (curTime - bossDeadTimeStamp) >= 5.0)
                {
                    Time.timeScale = 1.0f;
                    bossDeadTimeStamp = -1;
                }
            }

                /////////////////////////////////////////
                if (isBattleDataError)
			{
				SystemNotifyManager.SysNotifyMsgBoxOK("战斗数据异常！");

				isBattleDataError = false;

				mDelayCaller.DelayCall(GlobalLogic.VALUE_2000, ()=>{
					ClientSystemManager.GetInstance()._QuitToLoginImpl();	
				});
			}

			UpdateCheckGlobalLogicValues(iDeltaTime);
			/////////////////////////////////////////


                UpdateEntitiesGraphic(iDeltaTime);
                UpdateShowArrowForMonster(iDeltaTime);
                mBattle.TrailManager.Update(iDeltaTime);
                UpdateBlindMask(iDeltaTime);

                if (Global.Settings.debugDrawBlock)
                {
                    if (mCurrentGeScene != null)
                        mCurrentGeScene.SetBlockData(mSceneData, mBlockInfo);
                }
                break;
        }
#endif
#if ENABLE_PROFILER
        }
#endif
    }

    void UpdateTimer(int delta)
    {
        // marked by ckm
        // if (this.mBattle != null && !BattleMain.IsModePvP(mBattle.GetBattleType()))
        //     return;    

        if (simpleTimer != null)
            simpleTimer.UpdateTimer(delta);


        if (pkTimer == null)
        {
            var battleUI = BattleUIHelper.GetBattleUIComponent<BattleUIPvp>();
            if (battleUI != null)
                pkTimer = battleUI.TimerController;
        }

        if (pkTimer != null)
        {
            pkTimer.UpdateTimer(delta);
        }

    }

    void UpdateDelayCaller(int delta)
    {
        if (mDelayCaller != null)
        {
            mDelayCaller.Update(delta);
        }
    }

    public void ForceUpdatePendingArray()
    {
        mEntitys.AddRange(mPendingArray);
        mPendingArray.Clear();
    }

    private BeObject mHell;
    public BeObject CreateHellDestruct(Battle.DungeonHellInfo info)
    {
        if (info == null)
        {
            return null;
        }

        if (info.state == Battle.eDungeonHellState.End)
        {
            return null;
        }

        switch (info.mode)
        {
            case DungeonHellMode.Normal:
            case DungeonHellMode.Hard:
                VInt3 pos = VInt3.zero;

                //if (sceneData._hellbirthposition != null)
                //{
                //    pos = new Vec3(sceneData._hellbirthposition.position);
                //}

                if (sceneData.GetBirthPosition() != null)
                {
                    pos = new VInt3(sceneData.GetBirthPosition().GetPosition());
                }

                mHell = CreateDestruct(3017, pos, Color.white, VInt.one);
                mHell.SetCanBeAttacked(false);
                mHell.RegisterEventNew(BeEventType.onDead, eventParam => { mHell = null; });
                return mHell;
        }

        return mHell;
    }

    bool CheckEntityCanRemoveFlag(BeEntity item)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeScene.CheckEntityCanRemoveFlag"))
        {
#endif
        if (item.GetLifeState() == (int)EntityLifeState.ELS_CANREMOVE)
        {
            var actor = item as BeActor;
            if (actor != null)
            {
                if (actor.m_iCamp == (int)ProtoTable.UnitTable.eCamp.C_ENEMY)
                {
                    //_deadMonster(actor);
                }
                else if (actor.isLocalActor)
                {
                    return false;
                }
            }

            // 这个时候Entity已经没法Update了-。-
            if (item.m_iRemoveTime <= 0)
            {
                if (actor != null && actor.m_iCamp == (int)ProtoTable.UnitTable.eCamp.C_ENEMY)
                {
                    //_removeMonster(actor);
                }

                //item.OnRemove();
                //if (item.dontDelete)
                //	mDeadBody.Add(item);

                //item.ClearEvent();
                return true;
            }
        }
        return false;
#if ENABLE_PROFILER
        }
#endif
    }


    bool CheckEntityCanRemove(BeEntity item)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeScene.CheckEntityCanRemove"))
        {
#endif
        if (item.GetLifeState() == (int)EntityLifeState.ELS_CANREMOVE)
        {
            var actor = item as BeActor;
            if (actor != null)
            {
                if (actor.m_iCamp == (int)ProtoTable.UnitTable.eCamp.C_ENEMY)
                {
                    _deadMonster(actor);
                }
                else if (actor.isLocalActor)
                {
                    return false;
                }
            }

            // 这个时候Entity已经没法Update了-。-
            if (item.m_iRemoveTime <= 0)
            {
                if (actor != null && actor.m_iCamp == (int)ProtoTable.UnitTable.eCamp.C_ENEMY)
                {
                    _removeMonster(actor);
                }
                else if (item is BeObject)
                {
                    _removeDestruct();
                }

                item.OnRemove();
                if (item.dontDelete)
                    mDeadBody.Add(item);

                item.ClearEvent();

                if ((item as BeProjectile) != null)
                {
                    BeProjectilePool.PutProjectile(item as BeProjectile);
                }

                return true;
            }
        }
        return false;
#if ENABLE_PROFILER
        }
#endif
    }

    void UpdateEntitiesGraphic(int iDeltaTime)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeScene.UpdateEntitiesGraphic"))
        {
#endif
        for (int i = 0; i < mEntitys.Count; ++i)
        {
            var entity = mEntitys[i];
            if (null == entity) continue;
            entity.UpdateGraphic(iDeltaTime);
        }

        for (int i = 0; i < mTempSaveEntitys.Count; ++i)
        {
            var entity = mTempSaveEntitys[i];
            if (null == entity) continue;
            entity.UpdateGraphic(iDeltaTime);
        }
#if ENABLE_PROFILER
        }
#endif
    }
    void UpdateEntities(int iDeltaTime)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeScene.UpdateEntities"))
        {
#endif
        if (mPendingArray.Count > 0)
        {
            mEntitys.AddRange(mPendingArray);
            mPendingArray.Clear();
        }


        bool disableAI = false;
#if DEBUG_REPORT_ROOT
		disableAI = DebugSettings.GetInstance().DisableAI;
#endif

        bool bDirty = false;
        for (int i = 0; i < mEntitys.Count; ++i)
        {
            var entity = mEntitys[i];
            if (null == entity) continue;

            if (state != BeSceneState.onReady)
            {
                if (!disableAI && entity.hasAI && !entity.pauseAI && mIsTickAI)
                    entity.aiManager.Update(iDeltaTime);
            }

            entity.Update(iDeltaTime);

            if (state != BeSceneState.onReady)
            {
                if (!disableAI && entity.hasAI && !entity.pauseAI && mIsTickAI)
                    entity.aiManager.PostUpdate(iDeltaTime);
            }

            if (entity.GetLifeState() == (int)EntityLifeState.ELS_CANREMOVE)
            {
                entity.m_iRemoveTime -= iDeltaTime;
            }

            if (CheckEntityCanRemoveFlag(entity))
                bDirty = true;
        }

        if (bDirty)
        {
            _EntityRemoveAll(false,CheckEntityCanRemove);
        }

        for (int i = 0; i < mEntitys.Count; ++i)
        {
            var entity = mEntitys[i];
            if (null != entity)
                entity.PostUpate();
        }



        UpdateCheckMosnterClear(iDeltaTime);
#if ENABLE_PROFILER
        }
#endif
    }

 

    void _EntityRemoveAll(bool bForceUpdatePending,Predicate<BeEntity> match)
    {
        if(bForceUpdatePending)
        {
            ForceUpdatePendingArray();
        }

        for (int i = mEntitys.Count - 1; i >= 0; i--)
        {
            var entity = mEntitys[i];
            if (entity != null && match(entity))
            {
                mEntitysMap.Remove(entity.GetPID());
                mEntitys.RemoveAt(i);
            }
        }
    }

    bool CheckRegionCanRemove(BeRegionBase item)
    {
        return item.canRemove;
    }

    void UpdateRegions(int deltaTime)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeScene.UpdateRegions"))
        {
#endif
        if(mPendingRegion.Count > 0)
        {
            mRegions.AddRange(mPendingRegion);
            mPendingRegion.Clear();
        }

        bool bDirty = false;
        for (int i = 0; i < mRegions.Count; ++i)
        {
            var region = mRegions[i];
            if (!CheckRegionCanRemove(region))
                region.Update(deltaTime);
            else
                bDirty = true;
        }

        if (bDirty)
            _RemoveRegion(false,CheckRegionCanRemove);
#if ENABLE_PROFILER
        }
#endif
    }

    void _RemoveRegion(bool bForceUpdatePending,Predicate<BeRegionBase> match)
    {
        if(bForceUpdatePending && mPendingRegion.Count > 0)
        {
            mRegions.AddRange(mPendingRegion);
            mPendingRegion.Clear();
        }
        mRegions.RemoveAll(match);
    }

#endregion

#region Block Layer

    public DGrid CalGridByPosition(VInt3 position)
    {
        //!!Grid 计算这里需要验证
        int logicX = (position.x - logicXSize.x) / logicGrild.x;
        int logicZ = (position.y - logicZSize.x) / logicGrild.y;
        //logicZ -= 1;

        return new DGrid(logicX, logicZ);
    }

    public VInt3 CalPositionByGrid(DGrid grid)
    {
        return new VInt3(
            grid.x * logicGrild.x + logicXSize.x,
            (grid.y + 1) * logicGrild.y + logicZSize.x - logicGrild.y / 2,
            0);

    }

    // 格子化坐标
    public VInt3 CalPositionToGridPosition(VInt3 position)
    {
        return CalPositionByGrid(CalGridByPosition(position));
    }
    
    public bool IsInBlockPlayer(DGrid grid)
    {
        int logicX = grid.x;
        int logicZ = grid.y;

        var blockLayer = mBlockInfo;//sceneData._blocklayer;
        //var blockLayer = sceneData._blocklayer;

        int pos = logicZ * logicWidth + logicX;

        if (logicX < 0 || logicX >= logicWidth ||
            logicZ < 0 || logicZ >= logicHeight)
        {
            return true;
        }

        return blockLayer[pos] >= 1;
    }
    public bool IsInBlockPlayer(VInt3 position)
    {
        DGrid grid = CalGridByPosition(position);

        return IsInBlockPlayer(grid);
    }

    public byte InEventArea(DGrid grid)
    {
        if (mEventAreaInfo == null)
        {
            return 0;
        }

        int logicX = grid.x;
        int logicZ = grid.y;

        if (logicX < 0 || logicX >= logicWidth ||
            logicZ < 0 || logicZ >= logicHeight)
        {
            return 0;
        }

        int pos = logicZ * logicWidth + logicX;

        if (mEventAreaInfo.Length <= pos)
        {
            return 0;
        }

        return mEventAreaInfo[pos];
    }
    public byte InEventArea(VInt3 position)
    {
        DGrid grid = CalGridByPosition(position);

        return InEventArea(grid);
    }

    public bool IsInLogicScene(VInt3 position)
    {
        if (position.x <= logicXSize.y && position.x >= logicXSize.x && 
            position.y <= logicZSize.y && position.y >= logicZSize.x)
        {
            return true;
        }

        return false;
    }
    
    public void SetBlock(DGrid grid, bool isBlock)
    {
        //Logger.LogErrorFormat("set block ({0},{1}) {2}", grid.x, grid.y, isBlock);

        var blockLayer = mBlockInfo;//sceneData._blocklayer;
        //var blockLayer = sceneData._blocklayer;

        int logicX = grid.x;
        int logicZ = grid.y;

        if (logicX < 0 || logicX >= logicWidth ||
           logicZ < 0 || logicZ >= logicHeight)
        {
            return;
        }

        int pos = logicZ * logicWidth + logicX;
        if (isBlock)
            blockLayer[pos] = 1;
        else
            blockLayer[pos] = 0;
    }

    protected static readonly byte[] DEFAULT_BLOCK_DATA = new byte[] { 1 };
    protected byte[] _GetBlockData(int iResID, out int width, out int height)
    {
        string modelDataRes = FBModelDataSerializer.GetBlockDataResPath(iResID);
        DModelData modelData = null;
        if (!string.IsNullOrEmpty(modelDataRes))
        {
#if USE_FB
            FBModelDataSerializer.LoadFBModelData(Path.Combine(Utility.kRawDataPrefix, Path.ChangeExtension(modelDataRes, Utility.kRawDataExtension)), out modelData);
#else
            modelData = AssetLoader.instance.LoadRes(modelDataRes, typeof(DModelData)).obj as DModelData;
#endif
        }

        if (null != modelData)
        {
            width = modelData.blockGridChunk.gridWidth;
            height = modelData.blockGridChunk.gridHeight;
            return modelData.blockGridChunk.gridBlockData;
        }
        else
        {
            width = 1;
            height = 1;
            return DEFAULT_BLOCK_DATA;
        }
    }

    public void SetBlockLayer(int iResID, VInt3 pos, bool block = true)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeScene.SetBlockLayer"))
        {
#endif
        DGrid blockRect = new DGrid(GlobalLogic.VALUE_5, GlobalLogic.VALUE_10);
        int width;
        int height;

        byte[] blocks = _GetBlockData(iResID, out width, out height);

        bool hasBlock = true;
        if (width == 1 && height == 1)
        {
            hasBlock = false;
            //没有阻挡信息直接返回
            return;
        }
        else
        {
            blockRect.x = width;
            blockRect.y = height;
        }


        DGrid grid = CalGridByPosition(pos);
        int logicX = grid.x;
        int logicZ = grid.y;

        int startX = logicX - blockRect.x / 2 + 1;
        int startZ = logicZ - blockRect.y / 2 + 1;

        for (int i = startX; i < startX + blockRect.x; ++i)
        {
            for (int j = startZ; j < startZ + blockRect.y; ++j)
            {

                if (hasBlock)
                {
                    int index = (i - startX) + (j - startZ) * width;
                    //Logger.LogErrorFormat("index:{0}", index);
                    bool value = blocks[index] == 1;

                    if (value)
                    {
                        if (block)
                            SetBlock(new DGrid(i, j), true);
                        else
                            SetBlock(new DGrid(i, j), false);
                    }
                }
                else
                {
                    SetBlock(new DGrid(i, j), block);
                }
            }
        }

#if ENABLE_PROFILER
        }
#endif
    }
 


    int arrowAcc = 0;
    int arrowUpdateInterval = 1000;
    public void UpdateShowArrowForMonster(int delta)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeScene.UpdateShowArrowForMonster"))
        {
#endif
        arrowAcc += delta;
        if (arrowAcc > arrowUpdateInterval)
        {
            arrowAcc -= arrowUpdateInterval;

            BattlePlayer player = mBattle.dungeonPlayerManager.GetMainPlayer();
            if (player != null && player.playerActor!=null)
            {
                for (int i = 0; i < mEntitys.Count; ++i)
                {
                    var item = mEntitys[i] as BeActor;
                    if (item != null)
                    {
                        if (item.m_iCamp != player.playerActor.m_iCamp &&
                            item.GetEntityData() != null &&
                            item.GetEntityData().type != (int)ProtoTable.UnitTable.eType.EGG &&
                            !item.GetEntityData().isPet &&
                            item.GetEntityData().type != (int)ProtoTable.UnitTable.eType.SKILL_MONSTER)
                        {
                            if (!IsInScreen(item.GetPosition().vec3) && !item.stateController.IsInvisible())
                            {
                                TraceActor(item);
                                return;
                            }
                        }
                    }
                }
            }

            TraceActor(null);
        }
#if ENABLE_PROFILER
        }
#endif
    }

    public void ChooseADoorToChance()
    {
        TransportDoorType door = TransportDoorType.None;

        if (mBattle.GetBattleType() == BattleType.Hell && 0 == mBattle.dungeonManager.GetDungeonDataManager().IsHellAreaVisited())
        {
            traceDoorPos = mBattle.dungeonManager.GetDungeonDataManager().CurrentGuideHellPosition(ref door);
        }
        else
        {
            traceDoorPos = mBattle.dungeonManager.GetDungeonDataManager().CurrentGuidePosition(ref door);
        }
    }

    public TransportDoorType GetChanceDoorType()
    {
        TransportDoorType door = TransportDoorType.None;
        mBattle.dungeonManager.GetDungeonDataManager().CurrentGuidePosition(ref door);
        return door;
    }

    public VInt3 GetDoorPosition()
    {
        VInt3 pos = VInt3.zero;
        ChooseADoorToChance();
        if (traceDoorPos != VInt3.zero )
        {
            pos = traceDoorPos;
            pos.z = VInt.zeroDotOne.i;
            pos.x = Mathf.Clamp(pos.x, logicXSize.x, logicXSize.y);
            pos.y = Mathf.Clamp(pos.y, logicZSize.x, logicZSize.y);
        }

        return pos;
    }

    //获取场景中心点
    public VInt3 GetSceneCenterPosition()
    {
        VInt3 pos = VInt3.zero;
        int width = Mathf.Abs(logicXSize.y - logicXSize.x);
        int height = Mathf.Abs(logicZSize.y - logicZSize.x);
        pos.x = logicXSize.x + width / 2;
        pos.y = logicZSize.x + height / 2;
        pos.z = VInt.zeroDotOne.i;
        return pos;
    }

    public void UpdateShowArrowForDoor(int delta)
    {
        arrowAcc += delta;
        if (arrowAcc > arrowUpdateInterval)
        {
            arrowAcc -= arrowUpdateInterval;
            
            if (traceDoorPos != VInt3.zero)
            {
                var pos = traceDoorPos;
                pos.z = VInt.zeroDotOne.i;
                pos.x = Mathf.Clamp(pos.x, logicXSize.x, logicXSize.y);
                pos.y = Mathf.Clamp(pos.y, logicZSize.x, logicZSize.y);
                if (!IsInScreen(pos.vec3))
                {
                    TracePosition(pos.vec3, true);
                }
                else
                {
                    FinishTraceDoor();
                }
            }
        }
    }

    public void FinishTraceDoor()
    {
        TracePosition(Vec3.zero, true, true);
    }

    public void StartBlindMask()
    {
        isBlind = true;
        var battleUI = BattleUIHelper.GetBattleUIComponent<BattleUICommon>();
        if (battleUI != null)
            battleUI.SetBlindMask(true);
    }

    public void StopBlindMask()
    {
        isBlind = false;
        var battleUI = BattleUIHelper.GetBattleUIComponent<BattleUICommon>();
        if (battleUI != null)
            battleUI.SetBlindMask(false);
    }

    public void UpdateBlindMask(int delta)
    {
        if (!isBlind)
            return;
#if !LOGIC_SERVER
        BattlePlayer player = BattleMain.instance.GetLocalPlayer();

        if (player != null)
        {
            BeActor actor = player.playerActor;
            Vector3 gePos = actor.GetGePosition();
            //gePos.y += 2f;
            Vector2 pos = currentGeScene.WorldPosToScreenPos(gePos);

            var battleUI = BattleUIHelper.GetBattleUIComponent<BattleUICommon>();
            if (battleUI != null)
            {
                battleUI.SetBlindMaskPosition(pos);
            }
        }
#endif
    }

    public void TraceActor(BeActor target)
    {
#if !SERVER_LOGIC
        var battleUI = BattleUIHelper.GetBattleUIComponent<BattleUICommon>();

        if (target == null || target.IsDead())
        {
            if (battleUI != null)
                battleUI.ShowArrow(false);
            return;
        }
        if (target.m_pkGeActor == null) return;
        Vector3 headPos = new Vector3(target.GetPosition().vector3.x, target.m_pkGeActor.GetOverHeadPosition().y, target.GetPosition().vector3.z);
        TracePosition(target.GetPosition().vec3,false,false, headPos);
#endif
    }

    public void TracePosition(Vec3 pos, bool withGo = false, bool forceRemove = false,Vector3 headPos = default(Vector3))
    {
#if !SERVER_LOGIC

        var battleUI = BattleUIHelper.GetBattleUIComponent<BattleUICommon>();

        Vector2 screenPos = currentGeScene.WorldPosToScreenPos(pos.vector3());

        Vector2 screenPos2 = currentGeScene.WorldPosToScreenPos(headPos);

        float yPos = headPos == default(Vector3) ? screenPos.y : screenPos2.y;
        //Logger.LogErrorFormat("screenpos ({0},{1})", screenPos.x, screenPos.y);
        if (forceRemove ||
            screenPos.x > 0 && screenPos.x <= Screen.width &&
            yPos > 0 && yPos <= Screen.height)
        {
            if (battleUI != null) 
            {
                battleUI.ShowArrow(false, 0, true, withGo);
            }
        }
        else
        {
            Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2f);
            bool right = true;
            screenCenter.x = Screen.width;

            if (screenPos.x < 0)
            {
                right = false;
                screenCenter.x = 0;
            }


            if (battleUI != null)
            {
                battleUI.ShowArrow(true, screenPos.y / Screen.height, right, withGo);
            }
        }

#endif

    }

    public bool IsInScreen(Vec3 pos)
    {
        Vector2 screenPos = currentGeScene.WorldPosToScreenPos(new Vector3(pos.x, pos.z, pos.y));
        return (screenPos.x > 0 && screenPos.x <= Screen.width &&
                screenPos.y > 0 && screenPos.y <= Screen.height);
    }

#endregion

#region Debug Function

    public void DeleteEnemeyActorById(int resId)
    {
        for (int i = 0; i < mEntitys.Count; ++i)
        {
            var item = mEntitys[i];
            if (item.m_iCamp == 1 && item.m_iResID == resId)
            {
                item.DoDead();
                break;
            }
        }
    }

    public void ClearAllMonsters()
    {
        for (int j = 0; j < mEntitys.Count; j++)
        {
            var leftItem = mEntitys[j] as BeActor;
            if (leftItem != null &&
                !leftItem.IsDead() &&
                leftItem.m_iCamp == (int)ProtoTable.UnitTable.eCamp.C_ENEMY &&
                leftItem.GetEntityData().type != (int)ProtoTable.UnitTable.eType.SKILL_MONSTER &&
                leftItem.GetEntityData().type != (int)ProtoTable.UnitTable.eType.EGG
            )
            {
                leftItem.GetEntityData().SetHP(-1);
                leftItem.JudgeDead();
                leftItem.DoDead();
            }
        }
    }
    
    public void RemoveAllActorWithout(BeActor withoutActor = null)
    {
        for (int i = 0; i < mEntitys.Count; i++)
        {
            var actor = mEntitys[i] as BeActor;
            if(actor != null && actor != withoutActor)
            {
                actor.SetIsDead(true);
                actor.OnDead();
                actor.SetLifeState(EntityLifeState.ELS_CANREMOVE);
            }
        }
    }

    public void RemoveAllProjectiles()
    {
        for (int i = 0; i < mEntitys.Count; i++)
        {
            var projectile = mEntitys[i] as BeProjectile;
            if (projectile != null)
            {
                projectile.SetLifeState(EntityLifeState.ELS_CANREMOVE);
            }
        }
    }
    
    public void ClearAllCharacter()
    {
        for (int i = 0; i < mEntitys.Count; ++i)
        {
            var item = mEntitys[i];
            if (item != null && item.m_iCamp == 1)
            {
                item.DoHurt(int.MaxValue);
                item.DoDead();
            }
        }
    }

    //已经被删掉的对象
    public bool HasEntity(BeEntity entity)
    {
        return mEntitys.Contains(entity);
    }

    public void InitFriendActor(VInt3 pos)
    {
        for (int i = 0; i < mEntitys.Count; ++i)
        {
            BeActor actor = mEntitys[i] as BeActor;
            if (actor != null && !actor.IsDead() && actor.GetEntityData().isSummonMonster)
            {
                if (actor.actionManager != null)
                    actor.actionManager.StopAll();

                actor.SetPosition(pos);

                if (actor.isFloating)
                {
                    actor.RestoreFloating(false);
                }
            }

        }
    }

    public bool GetSummonBySummoner(List<BeActor> monsters, BeActor summoner, bool checkPendingList = false)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeScene.GetSummonBySummoner"))
        {
#endif
        if (monsters == null)
            return false;

        monsters.Clear();

        var entityList = checkPendingList ? mPendingArray : mEntitys;

        //List<BeActor> monsters = new List<BeActor>();
        for (int i = 0; i < entityList.Count; ++i)
        {
            BeActor monster = entityList[i] as BeActor;

            if (null == monster || (monster != null && monster.IsDead()))
            {
                continue;
            }

            BeEntityData entityData = monster.GetEntityData();
            if (null == entityData)
            {
                continue;
            }

            if (entityData.isSummonMonster && (monster.GetOwner() as BeActor) == summoner && !entityData.isPet)
            {
                monsters.Add(monster);
            }
        }

        return true;
        // return monsters;
#if ENABLE_PROFILER
        }
#endif
    }

    public int GetSummonCountByID(int mid, BeActor owner = null)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeScene.GetSummonCountByID"))
        {
#endif
        //List<BeActor> monsters = new List<BeActor>();

        int count = 0;
        for (int i = 0; i < mEntitys.Count; ++i)
        {
            BeActor monster = mEntitys[i] as BeActor;

            if (null == monster || (monster != null && monster.IsDead()))
            {
                continue;
            }

            BeEntityData entityData = monster.GetEntityData();

            if (null == entityData)
            {
                continue;
            }

            if (entityData.isSummonMonster && entityData.MonsterIDEqual(mid))
            {
                if (owner != null)
                {
                    if (monster.GetOwner() == owner)
                    {
                        //monsters.Add(monster);
                        count++;
                    }
                }
                else
                {
                    //monsters.Add(monster);
                    count++;
                }
            }
        }

        return count;//monsters.Count;
#if ENABLE_PROFILER
        }
#endif
    }

    //获取怪物数量By怪物ID和阵营
    public int GetMonsterCountByIdCamp(int monsterId,int camp)
    {
        int count = 0;
        List<BeActor> monsterList = GamePool.ListPool<BeActor>.Get();
        FindMonsterByIDAndCamp(monsterList, monsterId, camp);
        count = monsterList.Count;
        GamePool.ListPool<BeActor>.Release(monsterList);
        return count;
    }

    public int GetMonsterCount()
    {
        int count = 0;
        for (int i = 0; i < mEntitys.Count; ++i)
        {
            if ((mEntitys[i] as BeActor) == null || mEntitys[i].IsDead())
                continue;

            BeActor monster = (mEntitys[i] as BeActor);
            if (monster.IsMonster())
                count++;
        }

        for (int i = 0; i < mPendingArray.Count; ++i)
        {
            if ((mPendingArray[i] as BeActor) == null || mPendingArray[i].IsDead())
                continue;

            BeActor monster = (mPendingArray[i] as BeActor);
            if (monster.IsMonster())
                count++;
        }
        return count;
    }

    public BeEntity GetEntityByPID(int pid)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeScene.GetEntityByPID"))
        {
#endif
        BeEntity ret = null;
        mEntitysMap.TryGetValue(pid, out ret);
        return ret;
#if ENABLE_PROFILER
        }
#endif
    }

    public BeEntity GetEntityByResId(int resId,BeActor owner = null)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeScene.GetEntityByResId"))
        {
#endif
        BeEntity ret = null;
        for (int i = 0; i < mEntitys.Count; ++i)
        {
            var entity = mEntitys[i];
            if (owner != null)
            {
                if (entity.GetOwner() != null && entity.GetOwner() != owner)
                    continue;
            }
            if (entity != null && !entity.IsDead() && entity.m_iResID == resId)
                return entity;
        }
        return ret;
#if ENABLE_PROFILER
        }
#endif
    }

    public void GetEntitysByResId(List<BeEntity> entitys,int resId)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeScene.GetEntitysByResId"))
        {
#endif
        for(int i = 0; i < mEntitys.Count; ++i)
        {
            var entity = mEntitys[i];
            if(entity != null && !entity.IsDead() && entity.m_iResID == resId)
            {
                entitys.Add(entity);
            }
        }
#if ENABLE_PROFILER
        }
#endif
    }

    public bool FindMonsterByID1(List<BeActor> monsters, int mid,bool isEnemy = true)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeScene.FindMonsterByID"))
        {
#endif
        if (monsters == null || mid == 0)
            return false;

        // List<BeActor> monsters = new List<BeActor>();
        for (int i = 0; i < mEntitys.Count; ++i)
        {
            if ((mEntitys[i] as BeActor) == null || mEntitys[i].IsDead())
                continue;

            BeActor monster = (mEntitys[i] as BeActor);
            if (isEnemy)
            {
                if (monster.IsMonster() && (monster.GetEntityData().MonsterIDEqual(mid) || mid == 0))
                {
                    monsters.Add(monster);
                }
            }
            else
            {
                if (monster.attribute != null && monster.attribute.isMonster && monster.GetCamp() == (int)ProtoTable.UnitTable.eCamp.C_HERO && (monster.GetEntityData().MonsterIDEqual(mid) || mid == 0))
                {
                    monsters.Add(monster);
                }
            }
        }

        return true;
#if ENABLE_PROFILER
        }
#endif
    }

    public bool FindMonsterByID(List<BeActor> monsters, int mid,bool isEnemy = true)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeScene.FindMonsterByID"))
        {
#endif
        if (monsters == null || mid == 0)
            return false;

        monsters.Clear();

        // List<BeActor> monsters = new List<BeActor>();
        for (int i = 0; i < mEntitys.Count; ++i)
        {
            if ((mEntitys[i] as BeActor) == null || mEntitys[i].IsDead())
                continue;

            BeActor monster = (mEntitys[i] as BeActor);
            if (isEnemy)
            {
                if (monster.IsMonster() && (monster.GetEntityData().MonsterIDEqual(mid) || mid == 0))
                {
                    monsters.Add(monster);
                }
            }
            else
            {
                if (monster.attribute != null && monster.attribute.isMonster && monster.GetCamp() == (int)ProtoTable.UnitTable.eCamp.C_HERO && (monster.GetEntityData().MonsterIDEqual(mid) || mid == 0))
                {
                    monsters.Add(monster);
                }
            }
        }

        return true;
#if ENABLE_PROFILER
        }
#endif
    }
	
	//不要用于寻找攻击目标 可以用于寻找自己或者队友召唤的怪物
    public bool FindMonsterByIDAndCamp(List<BeActor> monsters, int mid,int camp)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeScene.FindMonsterByIDAndCamp"))
        {
#endif
        if (monsters == null || mid == 0)
            return false;

        monsters.Clear();
        
        for (int i = 0; i < mEntitys.Count; ++i)
        {
            if ((mEntitys[i] as BeActor) == null || mEntitys[i].IsDead())
                continue;

            BeActor monster = (mEntitys[i] as BeActor);
            if (monster.GetCamp() == camp && (monster.GetEntityData().MonsterIDEqual(mid) || mid == 0))
                monsters.Add(monster);
        }

        return true;
#if ENABLE_PROFILER
        }
#endif
    }
    public void FindAllMonsters(List<BeActor> targets, BeActor attacker, bool friend = false, IEntityFilter filter = null)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeScene.FindAllMonsters"))
        {
#endif
        int firstIndex = -1;
        targets.Clear();
        for (int i = 0; i < mEntitys.Count; ++i)
        {
            if ((mEntitys[i] as BeActor) == null)
                continue;

            if (mEntitys[i].IsDead())
                continue;

            if (!friend && attacker != null && (attacker == mEntitys[i] || mEntitys[i].m_iCamp == attacker.m_iCamp))
                continue;

            if (friend && attacker != null && mEntitys[i].m_iCamp != attacker.m_iCamp)
                continue;

            if ((mEntitys[i] as BeActor).IsSkillMonster())
                continue;

            if (!mEntitys[i].stateController.CanBeTargeted())
                continue;
            if (firstIndex == -1)
            {
                firstIndex = i;
            }
            if (filter == null || filter.isFit(mEntitys[i]))
            {
                targets.Add(mEntitys[i] as BeActor);
            }
        }
        if (filter != null && filter.isUseDefault() && firstIndex >= 0 && targets.Count <= 0)
        {
            if (filter != null)
            {
                if (firstIndex < mEntitys.Count)
                {
                    targets.Add(mEntitys[firstIndex] as BeActor);
                }
            }
        }
#if ENABLE_PROFILER
        }
#endif
    }
    public BeActor FindMonsterByID(int mid)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeScene.FindMonsterByID"))
        {
#endif
        if (mid == 0)
            return null;

        // List<BeActor> monsters = new List<BeActor>();
        for (int i = 0; i < mEntitys.Count; ++i)
        {
            if ((mEntitys[i] as BeActor) == null || mEntitys[i].IsDead())
                continue;

            BeActor monster = (mEntitys[i] as BeActor);

            if (monster.attribute != null && monster.attribute.isMonster && monster.GetEntityData().MonsterIDEqual(mid))
            {
                return monster;
            }

        }

        return null;
#if ENABLE_PROFILER
        }
#endif
    }

    public bool FindMainActor(List<BeActor> list)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeScene.FindMainActor"))
        {
#endif
        if (list == null)
            return false;

        list.Clear();

        // List<BeActor> monsters = new List<BeActor>();
        List<BattlePlayer> playerList = mBattle.dungeonPlayerManager.GetAllPlayers();
        for (int i = 0; i < playerList.Count; ++i)
        {
            BeActor actor = playerList[i].playerActor;
            if (actor==null || actor.IsDead())
                continue;

            if (actor.isMainActor)
                list.Add(actor);
        }

        return true;
#if ENABLE_PROFILER
        }
#endif
    }

    public BeActor FindNearestRangeTarget(VInt3 pos, BeActor attacker, VInt radius, List<BeActor> inList = null)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeScene.FindNearestRangeTarget"))
        {
#endif
        BeActor target = null;
        int min = int.MaxValue;

        VInt2 center = new VInt2(pos.x, pos.y);

        for (int i = 0; i < mEntitys.Count; ++i)
        {
            if ((mEntitys[i] as BeActor) == null || mEntitys[i].IsDead())
                continue;

            if (attacker == mEntitys[i] || mEntitys[i].m_iCamp == attacker.m_iCamp)
                continue;

            if ((mEntitys[i] as BeActor).IsSkillMonster())
                continue;

            if (!mEntitys[i].stateController.CanBeTargeted())
                continue;

            if (inList != null && inList.Contains(mEntitys[i] as BeActor))
                continue;


            var entityPos = mEntitys[i].GetPosition();
            VInt2 point = new VInt2(entityPos.x, entityPos.y);

            int distance = (center - point).magnitude;
            if (distance <= radius && distance < min)
            {
                target = mEntitys[i] as BeActor;
                min = distance;
            }
        }

        return target;
#if ENABLE_PROFILER
        }
#endif
    }


    public bool FindTargets(List<BeActor> targets, BeActor attacker, VInt radius, bool friend = false, IEntityFilter filter = null)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeScene.FindTargets"))
        {
#endif
        if (targets == null)
            return false;
        return FindTargetsByEntity(targets, attacker, radius, friend, filter);
#if ENABLE_PROFILER
        }
#endif
    }

    public bool FindTargetsByEntity(List<BeActor> targets, BeEntity attacker, VInt radius, bool friend = false, IEntityFilter filter = null)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeScene.FindTargetsByEntity"))
        {
#endif
        targets.Clear();
        //List<BeActor> targets = new List<BeActor>();
        if (attacker == null || attacker.IsDead())
            return false;

        var pos = attacker.GetPosition();
        VInt2 center = new VInt2(pos.x, pos.y);
        int firstIndex = -1;
        for (int i = 0; i < mEntitys.Count; ++i)
        {
            if ((mEntitys[i] as BeActor) == null)
                continue;

            if (mEntitys[i].IsDead())
                continue;

            if (!friend && (attacker == mEntitys[i] || mEntitys[i].m_iCamp == attacker.m_iCamp))
                continue;

            if (friend && mEntitys[i].m_iCamp != attacker.m_iCamp)
                continue;

            if ((mEntitys[i] as BeActor).IsSkillMonster())
                continue;

            if (!mEntitys[i].stateController.CanBeTargeted())
                continue;


            var entityPos = mEntitys[i].GetPosition();
            VInt2 point = new VInt2(entityPos.x, entityPos.y);

            int distance = (center - point).magnitude;

            if (distance <= radius)
            {
                if (firstIndex == -1)
                {
                    firstIndex = i;
                }
                if (filter == null || filter.isFit(mEntitys[i]))
                    targets.Add(mEntitys[i] as BeActor);
            }
        }
        if (filter != null && filter.isUseDefault() && firstIndex >= 0 && targets.Count <= 0)
        {
            if (filter != null)
            {
                if (firstIndex < mEntitys.Count)
                {
                    targets.Add(mEntitys[firstIndex] as BeActor);
                }
            }
        }
        return true;
#if ENABLE_PROFILER
        }
#endif
    }

    public List<BeActor> GetFilterTarget(List<BeActor> targets, IEntityFilter filter = null,bool judgeDead = true)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeScene.GetFilterTarget"))
        {
#endif
        targets.Clear();
        for (int i = 0; i < mEntitys.Count; ++i)
        {
            BeActor actor = mEntitys[i] as BeActor;
            if (actor == null)
                continue;

            if (judgeDead && actor.IsDead())
                continue;

            if (filter != null && filter.isFit(actor))
                targets.Add(actor);

        }

        for (int i = 0; i < mPendingArray.Count; ++i)
        {
            BeActor actor = mPendingArray[i] as BeActor;
            if (actor == null)
                continue;

            if (judgeDead && actor.IsDead())
                continue;

            if (filter != null && filter.isFit(actor))
                targets.Add(actor);

        }
        return targets;
#if ENABLE_PROFILER
        }
#endif
    }


    //找到最近的敌人 beactor
    public BeActor FindTarget(BeActor attacker, VInt radius)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeScene.FindTarget"))
        {
#endif
        if (attacker == null || attacker.IsDead())
            return null;

        var pos = attacker.GetPosition();
        VInt2 center = new VInt2(pos.x, pos.y);

        BeActor target = null;
        int minDistance = int.MaxValue;

        for (int i = 0; i < mEntitys.Count; ++i)
        {
            if ((mEntitys[i] as BeActor) == null || attacker == mEntitys[i] || mEntitys[i].IsDead() || mEntitys[i].m_iCamp == attacker.m_iCamp)
                continue;
            if (!mEntitys[i].stateController.CanBeTargeted())
                continue;

            var entityPos = mEntitys[i].GetPosition();
            VInt2 point = new VInt2(entityPos.x, entityPos.y);
 
            int distance = (center - point).magnitude;
            if (distance <= radius && distance < minDistance)
            {
                var actor = mEntitys[i] as BeActor;
                if (!actor.IsSkillMonster())
                {
                    minDistance = distance;
                    target = actor;
                }
            }
        }

        return target;
#if ENABLE_PROFILER
        }
#endif
    }



    /// <summary>
    /// 找到X轴面向的距离内的敌人
    /// </summary>
    /// <param name="targets"></param>
    /// <param name="attacker"></param>
    /// <param name="xDis"></param>
    /// <returns></returns>
    public bool FindFaceTargetsX(List<BeActor> targets, BeActor attacker, VInt xDis)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeScene.FindFaceTargetsX"))
        {
#endif
        targets.Clear();
        if (attacker == null || attacker.IsDead())
            return false;
        var pos = attacker.GetPosition();

        for (int i = 0; i < mEntitys.Count; ++i)
        {
            BeActor actor = mEntitys[i] as BeActor;
            if (actor == null)
                continue;

            if (actor.IsDead())
                continue;

            if ((attacker == actor || actor.m_iCamp == attacker.m_iCamp))
                continue;

            if (actor.IsSkillMonster())
                continue;

            if (!actor.stateController.CanBeTargeted())
                continue;

            var actorPos = actor.GetPosition();

            if (attacker.GetFace() && (pos.x - actorPos.x)>= 0 && (pos.x - actorPos.x) <= xDis)
            {
                targets.Add(actor);
            }
            else if(!attacker.GetFace() && (actorPos.x - pos.x) >= 0 && (actorPos.x - pos.x) <= xDis)
            {
                targets.Add(actor);
            }
        }
        return true;
#if ENABLE_PROFILER
        }
#endif
    }

    /// <summary>
    /// 寻找自己召唤的所有怪物
    /// </summary>
    /// <param name="summonList"></param>
    /// <returns></returns>
    public bool FindSummonMonster(List<BeActor> summonList,BeEntity owner)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeScene.FindSummonMonster"))
        {
#endif
        if (summonList == null)
            return false;
        summonList.Clear();
        for (int i = 0; i < mEntitys.Count; ++i)
        {
            if ((mEntitys[i] as BeActor) == null)
                continue;
            if (mEntitys[i].IsDead())
                continue;
            if (mEntitys[i] == owner)
                continue;
            if (mEntitys[i].GetOwner() == null || mEntitys[i].GetOwner() != owner)
                continue;
            summonList.Add((BeActor)mEntitys[i]);
        }
        return true;
#if ENABLE_PROFILER
        }
#endif
    }

    public BeActor FindNearestFacedTarget(BeActor attacker, VInt2 distance)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeScene.FindNearestFacedTarget"))
        {
#endif
        // List<BeActor> targets = new List<BeActor>();
        List<BeActor> targets = GamePool.ListPool<BeActor>.Get();

        var pos = attacker.GetPosition();
        VInt2 center = new VInt2(pos.x, pos.y);


        DBox aabb = new DBox();
        if (attacker.GetFace())
        {
            aabb._min.x = center.x - distance.x;
            aabb._max.x = center.x;
        }
        else
        {
            aabb._min.x = center.x;
            aabb._max.x = center.x + distance.x;
        }

        aabb._min.y = center.y - distance.y;
        aabb._max.y = center.y + distance.y;

        for (int i = 0; i < mEntitys.Count; ++i)
        {
            BeActor target = mEntitys[i] as BeActor;


            if (target != null && target != attacker && !target.IsDead() && target.m_iCamp != attacker.m_iCamp && !target.IsSkillMonster())
            {
                VInt2 tp = new VInt2(target.GetPosition().x, target.GetPosition().y);

                if (!target.stateController.CanBeTargeted())
                    continue;

                if (aabb.containPoint(ref tp))
                    targets.Add(target);
            }
        }

        BeActor retTarget = null;
        int minLen = int.MaxValue;
        for (int i = 0; i < targets.Count; ++i)
        {
            int dis = Mathf.Abs(targets[i].GetPosition().x - attacker.GetPosition().x);
            if (dis < minLen)
            {
                minLen = dis;
                retTarget = targets[i];
            }
        }

        GamePool.ListPool<BeActor>.Release(targets);

        return retTarget;
#if ENABLE_PROFILER
        }
#endif
    }
    
    /// <summary>
    /// 根据优先级获取目标
    /// </summary>
    public BeActor FindTargetByPriority(BeActor attacker, VInt radius)
    {
        List<BeActor> targetList = GamePool.ListPool<BeActor>.Get();
        FindTargets(targetList, attacker, radius);
        BeActor target = null;
        if (targetList != null && targetList.Count > 0)
        {
            for (int i = 0; i < targetList.Count; i++)
            {
                var actor = targetList[i];
                if (!actor.stateController.CanBeHit())
                    continue;
                if (target == null)
                {
                    target = actor;
                    continue;
                }
                var targetPrioritty = BeUtility.GetActorPriority(actor);
                var listPrioritty = BeUtility.GetActorPriority(actor);
                if (listPrioritty > targetPrioritty)
                {
                    target = actor;
                }
                else if (listPrioritty == targetPrioritty)
                {
                    var targetDis = target.GetDistance(attacker);
                    var listDis = actor.GetDistance(attacker);
                    if (listDis < targetDis)
                    {
                        target = actor;
                    }
                }
            }
        }
        GamePool.ListPool<BeActor>.Release(targetList);
        return target;
    }

    public BeActor GetResentmentActor(bool high = true)
    {
        List<BeActor> list = GamePool.ListPool<BeActor>.Get();
        FindMainActor(list);
        if (list.Count == 0)
        {
            GamePool.ListPool<BeActor>.Release(list);
            return null;
        }
        SortResentmentList(list);      
        BeActor actor = high ? list[0] : list[list.Count - 1];
        GamePool.ListPool<BeActor>.Release(list);
        return actor;
    }

    public void SortResentmentList(List<BeActor> list)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeScene.SortResentmentList"))
        {
#endif
        list.Sort((a, b) =>
        {
            Mechanism2004 mechanism_a = a.GetMechanism(5300) as Mechanism2004;
            Mechanism2004 mechanism_b = a.GetMechanism(5300) as Mechanism2004;
            if (mechanism_a != null && mechanism_b != null)
            {
                if (mechanism_a.IsBetray() && !mechanism_b.IsBetray())
                    return -1;
                if (!mechanism_a.IsBetray() && mechanism_b.IsBetray())
                    return 1;
                else
                {
                    int aValue = mechanism_a.GetResentmentValue();
                    int bValue = mechanism_b.GetResentmentValue();
                    if (aValue > bValue)
                        return -1;
                    if (aValue < bValue)
                        return 1;
                    if (aValue == bValue)
                    {
                        return a.GetPID().CompareTo(b.GetPID());
                    }
                }
            }
            return a.GetPID().CompareTo(b.GetPID());
        });
#if ENABLE_PROFILER
        }
#endif
    }

    public bool FindFaceTargets(List<BeEntity> targets, BeActor attacker, VInt2 distance, VFactor yReduceFactor, VInt xReduce)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeScene.FindFaceTargets"))
        {
#endif
        //List<BeEntity> targets = new List<BeEntity>();
        if (targets == null)
            return false;

        targets.Clear();

        var pos = attacker.GetPosition();
        VInt2 center = new VInt2(pos.x, pos.y);


        DBox aabb = new DBox();
        if (attacker.GetFace())
        {
            aabb._min.x = center.x - distance.x;
            aabb._max.x = center.x + xReduce.i;
        }
        else
        {
            aabb._min.x = center.x - xReduce.i;
            aabb._max.x = center.x + distance.x;
        }

        aabb._min.y = center.y - distance.y;
        aabb._max.y = center.y + distance.y * (VFactor.one - yReduceFactor);

        for (int i = 0; i < mEntitys.Count; ++i)
        {
            var target = mEntitys[i] as BeActor;
            var constructObject = mEntitys[i] as BeObject;


            if ((mEntitys[i] as BeObject) != null ||
                ((mEntitys[i] as BeActor) != null && target != attacker && !target.IsDead() && target.m_iCamp != attacker.m_iCamp && !(mEntitys[i] as BeActor).IsSkillMonster()))
            {
                VInt2 tp = new VInt2(mEntitys[i].GetPosition().x, mEntitys[i].GetPosition().y);

                if (!mEntitys[i].stateController.CanBeTargeted())
                    continue;

                if (aabb.containPoint(ref tp))
                    targets.Add(mEntitys[i]);
            }
        }

        return true;
#if ENABLE_PROFILER
        }
#endif
    }

    public bool IsEnemyClear(BeActor attacker)
    {
        for (int i = 0; i < mEntitys.Count; ++i)
        {
            var target = mEntitys[i] as BeActor;
            if (target != null && target != attacker && !target.IsDead() && target.m_iCamp != attacker.m_iCamp)
                return false;
        }

        return true;
    }

    public bool FindActorInRange(List<BeActor> list, VInt3 pos, VInt radius, int camp = -1, int monsterID = 0)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeScene.FindActorInRange"))
        {
#endif
        if (list == null)
            return false;
        list.Clear();
        // List<BeActor> list = new List<BeActor>();
        for (int i = 0; i < mEntitys.Count; ++i)
        {
            var actor = mEntitys[i] as BeActor;
            if(actor == null)
            {
                continue;
            }

            VInt dis = (pos - actor.GetPosition()).magnitude;
            if (actor != null &&
                !actor.IsDead() &&
                /*actor.m_iCamp == camp && */
                dis <= radius &&
                actor.stateController.CanBeTargeted())
            {
                if (camp > -1 && actor.GetCamp() != camp)
                    continue;

                if (monsterID != 0 && BeUtility.IsMonsterIDEqual(monsterID, actor.GetEntityData().simpleMonsterID))
                {
                    list.Add(actor);
                }
                else if (monsterID == 0 && !actor.IsSkillMonster())
                {
                    list.Add(actor);
                }
            }
        }

        return true;
#if ENABLE_PROFILER
        }
#endif
    }

    public bool FindMainActorInRange(List<BeActor> list, VInt3 pos, VInt radius)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeScene.FindMainActorInRange"))
        {
#endif
        if (list == null)
            return false;
        list.Clear();
        List<BattlePlayer> playerList = mBattle.dungeonPlayerManager.GetAllPlayers();
        for (int i = 0; i < playerList.Count; ++i)
        {
            BeActor actor = playerList[i].playerActor;
            if (actor == null || actor.IsDead())
                continue;

            VInt dis = (pos - actor.GetPosition()).magnitude;
            if (actor != null &&
                !actor.IsDead() &&
                dis <= radius && actor.isMainActor)
            {
                list.Add(actor);
            }
        }      

        return true;
#if ENABLE_PROFILER
        }
#endif
    }

    public void FindTargets(List<BeActor> list, BeActor attacker, bool isFriend, IEntityFilter filter)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeScene.FindTargets"))
        {
#endif
        if (list == null || filter == null)
            return;
        list.Clear();
        for (int i = 0; i < mEntitys.Count; ++i)
        {
            var actor = mEntitys[i] as BeActor;
            if (actor == null)
            {
                continue;
            }
            if (!isFriend && (attacker == mEntitys[i] || mEntitys[i].m_iCamp == attacker.m_iCamp))
                continue;

            if (isFriend && mEntitys[i].m_iCamp != attacker.m_iCamp)
                continue;

            if ((mEntitys[i] as BeActor).IsSkillMonster())
                continue;

            if (!mEntitys[i].stateController.CanBeTargeted())
                continue;
            if (filter.isFit(actor))
            {
                list.Add(actor);
            }
        }
        return;
#if ENABLE_PROFILER
        }
#endif
    }
    
    public BeActor FindActorById(int pid)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeScene.FindActorById Pid"))
        {
#endif
            for (int i = 0; i < mEntitys.Count; ++i)
            {
                var actor = mEntitys[i] as BeActor;
                if (actor != null && !actor.IsDead())
                {
                    if (pid != 0 && pid == actor.GetPID())
                    {
                        return actor;
                    }
                }
            }
            return null;
#if ENABLE_PROFILER
        }
#endif
    }
    
    public bool FindActorById(List<BeActor> list, int monsterID)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeScene.FindActorById"))
        {
#endif
        if (list == null)
            return false;
        list.Clear();
        for (int i = 0; i < mEntitys.Count; ++i)
        {
            var actor = mEntitys[i] as BeActor;
            if (actor != null && !actor.IsDead())
            {
                if (monsterID != 0 && BeUtility.IsMonsterIDEqual(monsterID, actor.GetEntityData().simpleMonsterID))
                {
                    list.Add(actor);
                }
            }
        }
        return true;
#if ENABLE_PROFILER
        }
#endif
    }

    //通过ID查找怪物 考虑到怪物晚一帧添加进Entitys里面的情况
    public bool FindActorById2(List<BeActor> list, int monsterID, bool canBeTarget = false)
    {
#if ENABLE_PROFILER
        using (new UWAProfilerNode("[tm]BeScene.FindActorById2"))
        {
#endif
        if (list == null)
            return false;
        list.Clear();
        for (int i = 0; i < mEntitys.Count; ++i)
        {
            var actor = mEntitys[i] as BeActor;
            if (actor != null && !actor.IsDead())
            {
                if (monsterID != 0 && BeUtility.IsMonsterIDEqual(monsterID, actor.GetEntityData().simpleMonsterID))
                {
                    if (canBeTarget)
                    {
                        if (actor.stateController != null && actor.stateController.CanBeTargeted())
                        {
                            list.Add(actor);
                        }
                    }
                    else
                    {
                        list.Add(actor);
                    }
                }
            }
        }

        if (mPendingArray.Count > 0)
        {
            for (int i = 0; i < mPendingArray.Count; i++)
            {
                var actor2 = mPendingArray[i] as BeActor;
                if (actor2 != null && !actor2.IsDead())
                {
                    if (monsterID != 0 && BeUtility.IsMonsterIDEqual(monsterID, actor2.GetEntityData().simpleMonsterID))
                    {
                        if (canBeTarget)
                        {
                            if (actor2.stateController != null && actor2.stateController.CanBeTargeted())
                            {
                                list.Add(actor2);
                            }
                        }
                        else
                        {
                            list.Add(actor2);
                        }
                    }
                }
            }
        }
        return true;
#if ENABLE_PROFILER
        }
#endif
    }

    public bool FindGroup(List<BeActor> targets, BeActor self)
    {
        targets.Clear();
        int groupID = self.GetEntityData().groupID;

        if (groupID == 0)
            return true;

        List<BeActor> list = GamePool.ListPool<BeActor>.Get();
        FindAllMonsters(list, self, true);
        for (int i = 0; i < list.Count; i++)
        {
            var actor = list[i];
            if (actor == null)
                continue;
            if (actor == self)
                continue;
            if (actor.IsDead())
                continue;
            if (!actor.stateController.CanBeTargeted())
                continue;
            if (actor.GetCamp() != self.GetCamp())
                continue;
            if (actor.GetEntityData().groupID == groupID)
                targets.Add(actor);
        }

        GamePool.ListPool<BeActor>.Release(list);
        return true;
    }

    /// <summary>
    /// 判断怪物是否还有存活的
    /// </summary>
    /// <param name="monsterId"></param>
    /// <returns></returns>
    public bool CheckMonsterAlive(int monsterId)
    {
        for (int i = 0; i < mEntitys.Count; ++i)
        {
            var actor = mEntitys[i] as BeActor;
            if (actor != null && !actor.IsDead())
            {
                if (monsterId != 0 && BeUtility.IsMonsterIDEqual(monsterId, actor.GetEntityData().simpleMonsterID))
                {
                    return true;
                }
            }
        }
        return false;
    }

    //在角色当前位置坐标的一定半径范围内随机选取一个坐标点(该坐标用于逻辑)
    public VInt3 GetLogicPosInRange(BeActor actor,int radius)
    {
        VInt3 pos = actor.GetPosition();
        int x = FrameRandom.InRange(pos.x - radius, pos.x + radius);
        int y = FrameRandom.InRange(pos.y - radius, pos.y + radius);
        return new VInt3(x, y, pos.z);
    }
    
    //在场景中随机取一个点
    public VInt3 GetRandomPos(int searchCount = 10)
    {
        VInt3 pos = VInt3.zero;
        int curCount = 0;
        DGrid newGrid = new DGrid();
        DGrid gridX = CalGridByPosition(new VInt3(logicXSize.x, logicZSize.x, 0));
        DGrid gridY = CalGridByPosition(new VInt3(logicXSize.y, logicZSize.y, 0));
        do
        {
            curCount++;
            int newGridX = FrameRandom.InRange(gridX.x, gridY.x);
            int newGridY = FrameRandom.InRange(gridX.y, gridY.y);
            newGrid = new DGrid(newGridX, newGridY);
            pos = CalPositionByGrid(newGrid);
            if (!IsInBlockPlayer(pos))
                break;
        } while (curCount < searchCount);
        return pos;
    }

    //在某个目标在X轴坐标点两边选取一个不在阻挡里面的点
    public VInt3 GetPosInXAxis(BeActor target, int searchCount = 10)
    {
        VInt3 pos = target.GetPosition();
        int offset = 0;
        int curCount = 0;
        do
        {
            curCount++;
            offset = curCount % 2 == 0 ? -1 : 1;
            pos.x += offset * curCount * GlobalLogic.VALUE_10000;
            if (!IsInBlockPlayer(pos))
                break;
        } while (curCount < searchCount);
        return pos;
    }

    public bool FindBoss(List<BeActor> list)
    {
        if (list == null)
            return false;
        list.Clear();
        //  List<BeActor> list = new List<BeActor>();
        for (int i = 0; i < mEntitys.Count; ++i)
        {
            var actor = mEntitys[i] as BeActor;
            if (actor != null && !actor.IsDead() && actor.IsBoss())
                list.Add(actor);
        }

        return true;
    }

    private BeSceneState mPrePauseState = BeSceneState.onNone;

    public void PauseLogic()
    {
        //现在流程里暂停逻辑不能重复进入，这里暂且退出
        if (state == BeSceneState.onPause)
            return;
        mPrePauseState = state;
        state = BeSceneState.onPause;

        if (mCurrentGeScene != null)
        {
            mCurrentGeScene.PauseLogic();
        }
    }

    public void ResumeLogic()
    {
        state = mPrePauseState;
        mPrePauseState = BeSceneState.onNone;

        if (mCurrentGeScene != null)
        {
            mCurrentGeScene.ResumeLogic();
        }
    }


    public void Pause(bool pauseAnimation = true)
    {
        if (!pauseAnimation)
        {
            //Time.timeScale = 0.65f;
            return;
        }

        //角色
        for (int i = 0; i < mEntitys.Count; ++i)
        {
            var entity = mEntitys[i];
            
            if (entity != null && !entity.IsPause() && entity.m_pkGeActor != null)
            {
                entity.m_pkGeActor.Pause();
            }
        }

        //场景特效
        if (currentGeScene != null)
        {
            var effectMgr = currentGeScene.GetEffectManager();
            if (effectMgr != null)
            {
                effectMgr.Pause(GeEffectType.EffectUnique);
                effectMgr.Pause(GeEffectType.EffectMultiple);
                effectMgr.Pause(GeEffectType.EffectGlobal);
            }
        }
    }

    public void Resume(bool pauseAnimation = true)
    {
        if (!pauseAnimation)
        {
            //Time.timeScale = 1.0f;
            return;
        }

        //角色
        for (int i = 0; i < mEntitys.Count; ++i)
        {
            var entity = mEntitys[i];
                
            if (!entity.IsPause())
            {
                entity.m_pkGeActor.Resume();
            }
        }

        //场景特效
        var effectMgr = currentGeScene.GetEffectManager();
        if (effectMgr != null)
        {
            effectMgr.Resume(GeEffectType.EffectUnique);
            effectMgr.Resume(GeEffectType.EffectMultiple);
            effectMgr.Resume(GeEffectType.EffectGlobal);
        }
    }

    /// <summary>
    /// 是否有玩家在释放觉醒技能
    /// </summary>
    public bool HavePlayerUseAwakeSkill()
    {
        if (mBattle == null)
            return false;
        if (mBattle.dungeonPlayerManager == null)
            return false;
        var players = mBattle.dungeonPlayerManager.GetAllPlayers();
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].playerActor == null)
                continue;
            if (!players[i].playerActor.IsCastingSkill())
                continue;
            var curSkill = players[i].playerActor.skillController.GetCurrentSkill();
            if (curSkill != null && curSkill.skillData != null && curSkill.skillData.SkillCategory == 4)
                return true;
        }
        return false;
    }

    #endregion
}
