using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if LOGIC_SERVER
public class GlobalSetting
#else
public class GlobalSetting : ScriptableObject
#endif
{ 
	public bool isUsingSDK
    {
        get 
        {
            return SDKChannel.NONE != sdkChannel &&
                   SDKChannel.COUNT != sdkChannel;
        }
    }


	public bool isTestTeam = false;

    public SDKChannel mainSDKChannel = SDKChannel.NONE;
	public bool isPaySDKDebug = false;
	public SDKChannel sdkChannel = SDKChannel.XY;
	public bool isBanShuVersion = false;
	public bool isDebug = true;
	public bool isLogRecord = false;
	public bool isRecordPVP = false;
    public bool showDebugBox = false;
    public bool isRecordAB = false;
    public int frameLock = 60;
    public float fallgroundHitFactor = 0.5f;
    public float hitTime = 0.15f;
    public float deadWhiteTime = 0.2f;
    public string defaultHitEffect = "Effects/Common/Sfx/Hit/Prefab/Eff_hit";
    public string defaultProjectileHitEffect = "Effects/Hero_Gunman/Common/Pistolbullet/Prefab/Eff_Pistol_Bullet_Hit";
    public string defualtHitSfx = "Sound/kezhangoumai";
    public string defualtChannel = "none";
    public int tolerate = 6;
    
    public Vec3 _walkSpeed = new Vec3(2, 4, 0); //* */
    public Vec3 walkSpeed
    {
        get
        {
            return  Utility.IRepeate2Vector(TableManager.instance.gst.walkSpeed);
        }
        set
        {

        }
    }
    public Vec3 _runSpeed = new Vec3(4, 4, 0); //* */
    public Vec3 runSpeed
    {
        get
        {
            return  Utility.IRepeate2Vector(TableManager.instance.gst.runSpeed);
        }
        set
        {

        }
    }
    public Vec3 townWalkSpeed = new Vec3(2, 4, 0); //* */
    public Vec3 townRunSpeed = new Vec3(4, 4, 0); //* */
    public float townActionSpeed = 1.0f;
    public bool townPlayerRun = true;
    public int minHurtTime = 150; //* */
	public int maxHurtTime = 600; //* */
    public float frozenPercent = 0.5f; //* */
    public Vector2 jumpBackSpeed = new Vector2(-5, 5);//* */
    public float jumpForce = 20.0f; 
    public float clickForce = 14.0f;
    public float snapDuration = 0.3f;
	public float _dunFuTime = 3.0f;
    public float drift = 3.0f;
    public int logicFrameStepDelta = 0;
    public uint logicFrameStep = 2;
    public float gateReconnectTimeOut = 3.5f;  //gateserver 断线超时时间
    public int gateReconnectSendTryCount = 3;  //gateserver 断线重连次数
    public float relayReconnectTimeOut = 3.5f;  //relayserver 断线超时时间
    public int relayReconnectSendTryCount = 3;   //relayserver 断线重连次数

    /// <summary>
    /// 追帧的时候
    /// 客户端每一帧，最多执行的逻辑帧数目
    /// </summary>
    public int frameSyncMaxUpdateCount = 8;

    public float switchWeaponTime
    {
        get
        {
            return Utility.I2FloatBy1000(TableManager.instance.gst.SwitchWeaponCD);
        }
        set
        {

        }
    }

    public float dunFuTime
    {
        get
        {
            return  Utility.I2FloatBy1000(TableManager.instance.gst.dunFuTime);
        }
        set
        {

        }
    }
	public float _pvpDunFuTime = 1.5f;
    public float pvpDunFuTime
    {
        get
        {
            return  Utility.I2FloatBy1000(TableManager.instance.gst.pvpDunFuTime);
        }
        set
        {

        }
    }
	public int PVPHPScale = 10; //* */
    public int TestLevel = 1;
	public int testPlayerNum = 1;
    public bool showBattleInfoPanel = false;
	public int defaultMonsterID = 1000;
	public float _monsterWalkSpeedFactor = 1.0f;
    public VFactor monsterWalkSpeedFactor
    {
        get
        {
            return  new VFactor(TableManager.instance.gst.monsterWalkSpeedFactor,GlobalLogic.VALUE_1000);
        }
        set
        {

        }
    }
	public float _monsterSightFactor = 1.0f;
    public float monsterSightFactor
    {
        get
        {
            return  Utility.I2FloatBy1000(TableManager.instance.gst.monsterSightFactor);
        }
        set
        {

        }
    }

    public bool enableAssetInstPool = false;

    public bool enemyHasAI = true;
    public bool isCreateMonsterLocal = false;
	public bool isGiveEquips = false;
	public string equipList = "";
    public string switchEquipList = "";

    public bool isAnimationInto = false;
    public bool isGuide = false;

#if UNITY_EDITOR || ROBOT_TEST
    public bool CloseLoginPushFrame = false;//登录弹窗
#endif

    public bool displayHUD = false;
    public bool CloseTeamCondition = false;
    public bool isLocalDungeon = false;
    public int localDungeonID = -1;

    public bool recordResFile = false;
    public bool profileAssetLoad = false;

    public float _gravity = 50.0f; //* */
    public float gravity {
        get{
            return TableManager.instance.gst.gravity;
        }
        set{

        }
    }

    public float _fallGravityReduceFactor = 1.0f; //* */

    public float fallGravityReduceFactor
    {
        get{
            return Utility.I2FloatBy10000(TableManager.instance.gst.fallGravityReduceFactor);
        }
        set
        {

        }
    }

    public bool skillHasCooldown = true;

    public string scenePath = "Data/SceneData/Town_Aierwen";

    public int ipSelectedIndex = 0;

    public int iSingleCharactorID = 1;

    public Vector2 cameraInRange = new Vector2(1f, 1f);

    public InputManager.ButtonMode buttonType = InputManager.ButtonMode.NORMAL;

    public float _defaultFloatHurt = 0.2f;             //* */
    public float defaultFloatHurt
    {
        get
        {
            return  Utility.I2FloatBy10000(TableManager.instance.gst.defaultFloatHurt);
        }
        set
        {

        }
    }
    public float _defaultFloatLevelHurat = 0.05f;     //* */
     public float defaultFloatLevelHurat
    {
        get
        {
            return  Utility.I2FloatBy10000(TableManager.instance.gst.defaultFloatLevelHurat);
        }
        set
        {

        }
    }
    public float _defaultGroundHurt = 0.15f;          //* */
     public float defaultGroundHurt
    {
        get
        {
            return  Utility.I2FloatBy10000(TableManager.instance.gst.defaultGroundHurt);
        }
        set
        {

        }
    }
	public float _defaultStandHurt = 0.2f;            //* */
    public float defaultStandHurt
    {
        get
        {
            return  Utility.I2FloatBy10000(TableManager.instance.gst.defaultStandHurt);
        }
        set
        {

        }
    }
	public float _fallProtectGravityAddFactor = 2.0f; //* */
    public float fallProtectGravityAddFactor
    {
        get
        {
            return  Utility.I2FloatBy1000(TableManager.instance.gst.fallProtectGravityAddFactor);
        }

        set
        {

        }
    }

	public int _protectClearDuration = 1500;         //* */
    public float protectClearDuration
    {
        get
        {
            return  TableManager.instance.gst.protectClearDuration;
        }
        set
        {

        }
    }

    public float bgmStart = 1.0f;
    public float bgmTown = 1.0f;
    public float bgmBattle = 1.0f;

    public float _zDimFactor = 1.0f;              //* */
    public VFactor zDimFactor
    {
        get
        {
            return new VFactor(TableManager.instance.gst.zDimFactor,GlobalLogic.VALUE_1000);
        }
        set
        {

        }
    }

    public float bullteScale = 0.2f;
    public int bullteTime = 1000;

    public GameClient.EClientSystem startSystem = GameClient.EClientSystem.Login;

    public string[] loggerFilter = new string[4] {".", ".", ".", "."};
    public bool showDialog = true;

    public Vector3 avatarLightDir = new Vector3(1, 1, 0);
    public Vector3 shadowLightDir = new Vector3(-0.65f, -1, 0.5f);

    public Vector3 startVel = new Vector3(10, 20, -20);
    public Vector3 endVel = new Vector3(0, 0, 0);
    public Vector3 offset = new Vector3(3f, 0, 0);
    public float TimeAccerlate = 20f;
	public int TotalTime = 2000;
	public int TotalDist = 100;

    public bool heightAdoption = true;
    public float battleCameraNearClip = -10;
    public float battleCameraFarClip = 100;
    public float townCameraNearClip = -1;
    public float townCameraFarClip = 100;


    public bool debugDrawBlock = false;

    public bool loadFromPackage = false;
    public bool enableHotFix = false;
    public bool hotFixUrlDebug = false; 

	//废弃
    public int REVIVE_SHOCK_SKILLID = 9998;

	public Vector2 rollSpeed = new Vector2(7, 0);
	public float rollRand = 0.3f;
	public float normalRollRand = 0.1f;

    public Texture2D globalRamp = null;
	//PVP天平
	public float _pkDamageAdjustFactor = 0.7f;
    public float pkDamageAdjustFactor
    {
        get
        {
            return  Utility.I2FloatBy1000(TableManager.instance.gst.pkDamageAdjustFactor);
        }
        set
        {

        }
    }
	public float _pkHPAdjustFactor = 0.7f;
    public float pkHPAdjustFactor
    {
        get
        {
            return  Utility.I2FloatBy1000(TableManager.instance.gst.pkHPAdjustFactor);
        }
        set
        {

        }
    }
	public bool pkUseMaxLevel = true;

	public BattleRunMode battleRunMode = BattleRunMode.RUN;
	public bool hasDoubleRun = false;
    public bool useNewHurtAction = false;
    public bool useNewGravity = false;
    public int[] speedAnchorArray = new int[3] { 0, 800, 100000 };
    public int[] gravityRateArray = new int[3] { 1000, 500, 1000 };

    public int playerHP = 0;
	public int playerRebornHP = 0;
	public int monsterHP = 0;

	public Vec3 playerPos = Vec3.zero;
	public float transportDoorRadius = 1.0f;//传送门半径

	public float petXMovingDis = 1.5f;
	public float petXMovingv1 = 2.0f;
	public float petXMovingv2 = 5.0f;//max
	public float petYMovingDis = 1.5f;
	public float petYMovingv1 = 1f;
	public float petYMovingv2 = 3.0f;//max

    [System.Serializable]
    public class Address {
        public string name;
        public string ip;
        public ushort port;
        public uint id;
        
        public override string ToString() {
            return name;
        }
    }

    public string serverListUrl = "http://192.168.2.22:8765";
    
    public Address[] serverList = new Address[] {
        new Address{ip="192.168.0.156", port=8444}
    };

    public string IPAddress {
        get {
            return serverList[ipSelectedIndex].ip;            
        }
    }
    
    public ushort IPPort {
        get {
            return serverList[ipSelectedIndex].port;
        }
    }

    //战斗细节相关
	public bool aiHotReload = false;
	public float charScale = 1.0f;
    public bool damageNoRange = false;
    public bool sceneDark = false;

	public ShockData monsterBeHitShockData = new ShockData(0.2f, 70f, 0.03f, 0, 0);
	public ShockData playerBeHitShockData = new ShockData(0.2f, 70f, 0.03f, 0, 0);
	public ShockData playerSkillCDShockData = new ShockData(0.2f, 70f, 0.03f, 0, 0);
	//从高掉落到地上会震动
	public ShockData playerHighFallTouchGroundShockData = new ShockData(0.3f, 100f, 0.03f, 0, 0);
	public float highFallHight = 7.0f;

	public string critialDeadEffect = "_NEW_RESOURCES/Effects/Common/Prefab/Skill_Common_hit";
	public string immediateDeadEffect = "Effects/Common/Sfx/Siwang/Eff_die_yiji";
	public string normalDeadEffect = "Effects/Common/Sfx/Siwang/Eff_Common_die_guo";

    public bool enableEffectLimit = true;
    public int effectLimitCount = 10;

	public int immediateDeadHPPercent = 30;
	//废弃
	public bool openBossShow = false;
	public float shooterFitPercent = 0.5f;

    //传送门特效名称前缀
    public string doorEffPrefix = "eff|tx";

    //练习场机器人AI设置相关
    public bool isTrainingAIOpen = false;
    public int trainingAIConfigId = 0;          //对应机器人AI表ID
    public int trainingRobotId = 0;             //对应机器人配置表ID

    //随从
    public Vector3 disappearDis = new Vector3(-2, 0, 5);
	public float keepDis = 2.0f;
	public float accompanyRunTime = .5f;

	//怪物AI
	public int _aiWanderRange = 2;           //* */
    public int aiWanderRange
    {
        get
        {
            return TableManager.instance.gst.aiWanderRange;
        }
        set
        {

        }
    }	
    public int _aiWAlkBackRange = 2;         //* */
    public int aiWAlkBackRange
    {
        get
        {
            return   TableManager.instance.gst.aiWAlkBackRange;
        }
        set
        {

        }
    }	
	public int _aiMaxWalkCmdCount = 2;       //* */
    public int aiMaxWalkCmdCount
    {
        get
        {
            return   TableManager.instance.gst.aiMaxWalkCmdCount;
        }
        set
        {

        }
    }	
	public int _aiMaxWalkCmdCount_RANGED = 2;    //* */
    public int aiMaxWalkCmdCount_RANGED
    {
        get
        {
            return   TableManager.instance.gst.aiMaxWalkCmdCount_RANGED;
        }
        set
        {

        }
    }	
	public int _aiMaxIdleCmdcount = 1;           //* */
    public int aiMaxIdleCmdcount
    {
        get
        {
            return   TableManager.instance.gst.aiMaxIdleCmdcount;
        }
        set
        {

        }
    }	
	public int _aiSkillAttackPassive = 30;       //* */
    public int aiSkillAttackPassive
    {
        get
        {
            return   TableManager.instance.gst.aiSkillAttackPassive;
        }
        set
        {

        }
    }	

	public float _monsterGetupBatiFactor = 0.3f;  //* */
    public float monsterGetupBatiFactor
    {
        get
        {
            return  Utility.I2FloatBy1000(TableManager.instance.gst.monsterGetupBatiFactor);
        }
        set
        {

        }
    }
	public float _degangBackDistance = 0.5f;     //* */
    public float degangBackDistance
    {
        get
        {
            return  Utility.I2FloatBy1000(TableManager.instance.gst.degangBackDistance);
        }
        set
        {

        }
    }

	//自动战斗
	public int _afThinkTerm = 100;
    public int afThinkTerm
    {
        get
        {
            return   TableManager.instance.gst.afThinkTerm;
        }
        set
        {

        }
    }	
	public int _afFindTargetTerm = 100;
     public int afFindTargetTerm
    {
        get
        {
            return   TableManager.instance.gst.afFindTargetTerm;
        }
        set
        {

        }
    }	
	public int _afChangeDestinationTerm = 300;
     public int afChangeDestinationTerm
    {
        get
        {
            return   TableManager.instance.gst.afChangeDestinationTerm;
        }
        set
        {

        }
    }	
	public int _autoCheckRestoreInterval = 2000;
     public int autoCheckRestoreInterval
    {
        get
        {
            return   TableManager.instance.gst.autoCheckRestoreInterval;
        }
        set
        {

        }
    }	
	public bool forceUseAutoFight = false;
	public bool canUseAutoFight = true;
	public bool canUseAutoFightFirstPass = false;
	public bool loadAutoFight = false;

	//跳攻的最低高度设置
	public float jumpAttackLimitHeight = 0.7f;
    public int JumpAttackLimitHeight
    {
        get { return IntMath.Float2IntWithFixed(jumpAttackLimitHeight); }
    }

    //技能中断时间
    public float skillCancelLimitTime = 1f;

	//摇杆配置
	public int doublePressCheckDuration = 500;
	public ActionType walkAction = ActionType.ActionType_WALK;
	public ActionType runAction = ActionType.ActionType_RUN;
	public float walkAniFactor = 1.0f;
	public float runAniFactor = 1.0f;
	public bool changeFaceStop = true;

	public Vec3 _monsterWalkSpeed = new Vec3(2.2f, 4.5f, 0f);//* */
    public Vec3 monsterWalkSpeed
    {
        get
        {
            return  Utility.IRepeate2Vector(TableManager.instance.gst.monsterWalkSpeed);
        }
        set
        {

        }
    }
	public Vec3 _monsterRunSpeed = new Vec3(3.8f, 6f, 0f);//* */
    public Vec3 monsterRunSpeed
    {
        get
        {
            return  Utility.IRepeate2Vector(TableManager.instance.gst.monsterRunSpeed);
        }
        set
        {

        }
    }

	public int tableLoadStep = 3;
	public int loadingProgressStepInEditor = 100;
	public int loadingProgressStep = 10;

	//pvp录像
	public string pvpDefaultSesstionID = null;
    public string mPveRecordPath = null;

    //宠物测试
    public int petID = 0;
	public int petLevel = 1;
	public int petHunger = 10;
	public int petSkillIndex = 0;
	//测试
	public bool testFashionEquip = false;

    public string serverCodePath = "";


    #region equipRate
    public Dictionary<string, float> equipPropFactors = new Dictionary<string, float>(){};

	public float[] equipPropFactorValues = new float[] {
		0.2f,
		0.2f,
		0.5f,
		0.5f,
		1f,
		1f,
		0.1f,
		0.1f,
		2f,
		2f,
		2f,
		2f,
		2f,
		2f,
		2f,
		0.5f,
		0.5f,

		0.8f,
		0.8f,
		0.8f,
		0.8f,

		2f,
		2f,
		2f,
		2f,
	};

	public Dictionary<string, float> quipThirdTypeFactors = new Dictionary<string, float>(){};

	public float[] quipThirdTypeFactorValues = new float[] {
		1f,
		0.95f,
		1,
		1,
		1,
		1.2f,
		1,
		1.2f,
		1,
		1,
		1,
		1,
		1,
		1,
		3f,
		1.7f,
		1,
		1,
		1
	};
#endregion

#region gameTestSetting
    [System.Serializable]
    public class QualityAdjust
    {
        public bool bIsOpen = false;
        public float fInterval = 1.0f;
        public int iTimes = 50;

        [HideInInspector]
        public Vector2 mScroll;

        [System.NonSerialized]
        bool bDirty = false;
        public bool Dirty
        {
            get
            {
                return bDirty;
            }
            set
            {
                bDirty = value;
                OnQualityDirty(value);
            }
        }

        [System.NonSerialized]
        List<GameClient.ItemData> kEquipments = new List<GameClient.ItemData>();
        public List<GameClient.ItemData> Equipments
        {
            get
            {
                return kEquipments;
            }
        }

        [HideInInspector]
        [System.NonSerialized]
        public List<bool> toggles = new List<bool>();

        [System.NonSerialized]
        int iSelectedIndex = -1;
        public int SelectedIndex
        {
            get
            {
                return iSelectedIndex;
            }
            set
            {
                iSelectedIndex = value;
            }
        }

        public void OnQualityDirty(bool bDirty)
        {
            kEquipments.Clear();
            toggles.Clear();
            if (bDirty)
            {
                var uids = GameClient.ItemDataManager.GetInstance().GetItemsByPackageType(GameClient.EPackageType.Equip);
                for (int i = 0; i < uids.Count; ++i)
                {
                    var itemData = GameClient.ItemDataManager.GetInstance().GetItem(uids[i]);
                    if (itemData != null)
                    {
                        kEquipments.Add(itemData);
                        toggles.Add(false);
                    }
                }

                uids = GameClient.ItemDataManager.GetInstance().GetItemsByPackageType(GameClient.EPackageType.WearEquip);
                for (int i = 0; i < uids.Count; ++i)
                {
                    var itemData = GameClient.ItemDataManager.GetInstance().GetItem(uids[i]);
                    if (itemData != null && itemData.Type != ProtoTable.ItemTable.eType.FUCKTITTLE)
                    {
                        kEquipments.Add(itemData);
                        toggles.Add(false);
                    }
                }

                for(int i = 0; i < kEquipments.Count; ++i)
                {
                    if(kEquipments[i].SubQuality >= 100)
                    {
                        toggles.RemoveAt(i);
                        kEquipments.RemoveAt(i);
                        --i;
                    }
                }
            }
        }
    }
    public QualityAdjust qualityAdjust = new QualityAdjust();

	public bool IsTestTeam()
	{
#if DEBUG_SETTING
		return isDebug && isTestTeam;
#endif
		return false;
	}
#endregion

    public float petDialogLife = 5.0f;
    public float petDialogShowInterval = 10.0f;
    public float petSpecialIdleInterval = 15.0f;
    public int notifyItemTimeLess = 2 * 24 * 3600;
}

