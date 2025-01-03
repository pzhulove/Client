using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
 
[System.Serializable]
public class ShapeBox
{
    public Vector2 size = new Vector2(1,1);
    public Vector2 center = new Vector2(0,0);
}

[System.Serializable]
public class BaseDecisionBox
{
    public ShapeBox[] boxs = new ShapeBox[0];
    [HideInInspector]
    public bool hasHit;
    [HideInInspector]
    public bool blockToggle;
}

[System.Serializable]
public class HurtDecisionBox : BaseDecisionBox
{
    public float zDim = 1;
    public int damage;
    public int hurtID;//伤害效果表的ID
}

[System.Serializable]
public class DefenceDecisionBox : BaseDecisionBox
{
    public int type;
}

public enum EffectPlayType
{
    [Description("特效自己的存在时间")]
    EffectTime = 0,

    [Description("指定的时间")]
    GivenTime,
}

public enum EffectAttachPoint
{
    Global = 0,
    Orign,
    Head,
    LeftHand,
    RightHand
}

public enum VisiableFliter
{
    HurtBox = 1 << 0,
    DefBox  = 1 << 1,
    Effect  = 1 << 2,
}

public enum VisiableFliterAll
{
    All = VisiableFliter.HurtBox | VisiableFliter.DefBox | VisiableFliter.Effect
}

public class TransformParam
{
    public Vector3      localPosition;
    public Quaternion   localRotation;
    public Vector3      localScale;
}
public enum EffectTimeType
{
    [Description("每次都重新创建")]
    SYNC_ANIMATION = 0,

    [Description("只会存在一个")]
    GLOBAL_ANIMATION,

    [Description("全局存在")]
    BUFF
}

[System.Serializable]
public class EffectsFrames
{
    public string            name;
    public int               startFrames;
    public int               endFrames;
    public string            attachname;
    public EffectPlayType    playtype;
    public EffectTimeType    timetype;
    public float             time;
    public GameObject        effectGameObjectPrefeb;
    public DAssetObject      effectAsset;
    public EffectAttachPoint attachPoint;
    public Vector3           localPosition = Vector3.zero;
    public Quaternion        localRotation = Quaternion.identity;
    public Vector3           localScale = Vector3.one;
    public int               effecttype;
    public bool              loop = false;
    public bool              loopLoop = true;//如果技能循环，这个特效要不要循环
    public bool              onlyLocalSee = false;   //特效只有本地玩家才能看到
    public bool              useActorSpeed = false;  //特效播放速度是否受角色攻速影响

    public void Copy(EffectsFrames src)
    {
        name = src.name;
        startFrames = src.startFrames;
        endFrames = src.endFrames;
        attachname = src.attachname;
        playtype = src.playtype;
        timetype = src.timetype;
        time = src.time;
        effectGameObjectPrefeb = src.effectGameObjectPrefeb;
        effectAsset = src.effectAsset;
        attachPoint = src.attachPoint;
        localPosition = src.localPosition;
        localRotation = src.localRotation;
        localScale = src.localScale;
        effecttype = src.effecttype;
        useActorSpeed = src.useActorSpeed;
    }

    [System.NonSerialized]
    static readonly public EffectsFrames Default = new EffectsFrames();

#if UNITY_EDITOR
    [System.NonSerialized]
    public DSkillEditor.DEditorEffects effects = new DSkillEditor.DEditorEffects();

    public EffectsFrames()
    {
        effects.SetData(this);
    }

    public void Check()
    {
        if(effects == null)
        {
            effects = new DSkillEditor.DEditorEffects();
            effects.SetData(this);
        }
    }

    public float AdjustFloat(float value)
    {
        if( Mathf.Abs(value) < Vector3.kEpsilon )
        {
            if(value < 0.0f)
            {
                return -Vector3.kEpsilon;
            }
            else
            {
                return Vector3.kEpsilon;
            }
        }

        return value;
    }
    public void AdjustScale()
    {
        localScale.x = AdjustFloat(localScale.x);
        localScale.y = AdjustFloat(localScale.y);
        localScale.z = AdjustFloat(localScale.z);
    }
#endif
}
 
 
public enum EntityType
{
    Bullet = 0,
    LogicEntity,
}

//子弹偏移用
public enum OffsetType
{
    OFFSET_NONE = 0, //没有偏移
    OFFSET_VERTICAL, //
    OFFSET_ANGLE
}


[System.Serializable]
public struct ShockInfo
{
    public float shockTime;
    public float shockSpeed;
    public float shockRangeX;
    public float shockRangeY;
}

public enum AxisType
{
    X,
    Y,
    Z
}

[System.Serializable]
public struct RandomLaunchInfo
{
	public int num;
	public bool isNumRand;
	public Vector2 numRandRange;
	public float interval;
	public float rangeRadius;
    public bool isFullScene;            //是否全场景随机取点
}

public enum EntityFace
{
    [Description("不设置")]
    NONE = 0,

    [Description("左")]
    LEFT,

    [Description("右")]
    RIGHT
}

[System.Serializable]
public class EntityFrames
{
    public string       name;
    public int          resID;
    public EntityType   type;
    public bool randomResID = false;
    public int[] resIDList = new int[0];
    public int          startFrames;
    public GameObject   entityPrefab;
    public DAssetObject entityAsset;

    public Vector2        gravity;
    public float          speed;
    public float          angle;
	public bool 		  isAngleWithEffect = true;
    public Vector2        emitposition;
    public float emitPositionZ;

    public AxisType axisType = AxisType.Z;

    public float shockTime;
    public float shockSpeed;
    public float shockRangeX;
    public float shockRangeY;

    public bool isRotation = false;
    public float rotateSpeed;
    public float moveSpeed;
    public int rotateInitDegree;

    //public ShockInfo targetShock;
    public ShockInfo sceneShock;

    public int hitFallUP;
    public float forceY;

    public int hurtID;

    public float lifeTime;
    public bool hitThrough;
    public int hitCount = 1;
    public float distance = 999999;
	public bool attackCountExceedPlayExtDead = false;
    public bool hitGroundClick = false;

    public float delayDead = 0;

    public OffsetType offsetType;

    public TargetChooseType targetChooseType;
    public Vector2 range;
    public float boomerangeDistance = 2.5f;
    public float stayDuration;

	//抛物线
	public float paraSpeed = 5f;
	public float paraGravity = 18f;//Global.Settings.gravity;

    public Vector2 offset;

	public bool useRandomLaunch;
	public RandomLaunchInfo randomLaunchInfo;

    public bool onCollideDie;
    public bool onXInBlockDie;

    public bool changForceBehindOther;

    public int changeFace = 0;

    // 旋转追踪目标位置参数
    public float changeMaxAngle = 0;
    public float chaseTime = 0;

    public void Copy(EntityFrames src)
    {
        name = src.name;
        resID = src.resID;
        type = src.type;
        startFrames = src.startFrames;
        entityPrefab = src.entityPrefab;
        entityAsset = src.entityAsset;

        gravity = src.gravity;
        speed = src.speed;
        angle = src.angle;
        isAngleWithEffect = src.isAngleWithEffect;
        emitposition = src.emitposition;
        emitPositionZ = src.emitPositionZ;

        axisType = src.axisType;

        shockTime = src.shockTime;
        shockSpeed = src.shockSpeed;
        shockRangeX = src.shockRangeX;
        shockRangeY = src.shockRangeY;

        isRotation = src.isRotation;
        rotateSpeed = src.rotateSpeed;
        moveSpeed = src.moveSpeed;
        rotateInitDegree = src.rotateInitDegree;

        sceneShock = src.sceneShock;

        hitFallUP = src.hitFallUP;
        forceY = src.forceY;

        hurtID = src.hurtID;

        lifeTime = src.lifeTime;
        hitThrough = src.hitThrough;
        hitCount = src.hitCount;
        distance = src.distance;
        attackCountExceedPlayExtDead = src.attackCountExceedPlayExtDead;
        hitGroundClick = src.hitGroundClick;

        delayDead = src.delayDead;

        offsetType = src.offsetType;

        targetChooseType = src.targetChooseType;
        range = src.range;
        boomerangeDistance = src.boomerangeDistance;
        stayDuration = src.stayDuration;

        paraSpeed = src.paraSpeed;
        paraGravity = src.paraGravity;

        useRandomLaunch = src.useRandomLaunch;
        randomLaunchInfo = src.randomLaunchInfo;

        onCollideDie = src.onCollideDie;
        onXInBlockDie = src.onXInBlockDie;

        changForceBehindOther = src.changForceBehindOther;

        changeFace = src.changeFace;

        changeMaxAngle = src.changeMaxAngle;
        chaseTime = src.chaseTime;
    }

    public EntityFrames()
    {
#if UNITY_EDITOR
        entity = new DSkillEditor.DEditorEntity(this);
#endif
    }

#if UNITY_EDITOR
    [System.NonSerialized]
    public DSkillEditor.DEditorEntity entity;
#endif
}
[System.Serializable]
public class AnimationFrames
{
    public float            start;
    public string           anim;
    public float            blend = 0.1f;
    public WrapMode         mode;
    public float            speed = 1.0f;
    public AnimationClip    clip;

    public void Copy(AnimationFrames src)
    {
        start = src.start;
        anim = src.anim;
        blend = src.blend;
        mode = src.mode;
        speed = src.speed;
        clip = src.clip;
    }
}
[System.Serializable]
public class EntityAttachFrames
{
    public string           name;
    public int              resID;
    public float            start;
    public float            end;
    public string           attachName;
    public GameObject       entityPrefab;
    public DAssetObject     entityAsset;
    public TransformParam   trans;
    public AnimationFrames[] animations = new AnimationFrames[0];

    public void Copy(EntityAttachFrames src)
    {
        name = src.name;
        resID = src.resID;
        start = src.start;
        end = src.end;
        attachName = src.attachName;
        entityPrefab = src.entityPrefab;
        entityAsset = src.entityAsset;
        trans = src.trans;

        List<AnimationFrames> af = new List<AnimationFrames>();

        for(int i = 0; i < src.animations.Length; ++i)
        {
            AnimationFrames frame = new AnimationFrames();
            frame.Copy(src.animations[i]);
            af.Add(frame);
        }

        animations = af.ToArray();
    }

#if UNITY_EDITOR
    [System.NonSerialized]
    public DSkillEditor.DEditorAttach attach;

    public EntityAttachFrames()
    {
        attach = new DSkillEditor.DEditorAttach(this); 
    }
#endif
}

public enum TriggerNextPhaseType
{
    [Description("无")]
    NONE = 0,

    [Description("上升到最高")]
    UPSTOP,             

    [Description("接触地面")]
    TOUCHGROUND,        

    [Description("松开技能按钮")]
    RELEASE_BUTTON,     

    [Description("再次点击技能按钮")]
    PRESS_AGAIN         
}


[System.Serializable]
public class ChargeConfig
{
    public int repeatPhase;
    public int changePhase;
    public int switchPhaseID;
    public float chargeDuration;
    public float chargeMinDuration;
    public string effect;
    public string locator;
    public int buffInfo;                    //蓄力完成时 添加一个BuffInfo
    public bool playBuffAni;                //蓄力完成时 是否播放霸体动画
    public Vector3 effectPos;   //蓄力特效初始位置

    public ChargeConfig DoCopy()
    {
        var newConfig = new ChargeConfig();
        newConfig.repeatPhase = this.repeatPhase;
        newConfig.changePhase = this.changePhase;
        newConfig.switchPhaseID = this.switchPhaseID;
        newConfig.chargeDuration = this.chargeDuration;
        newConfig.chargeMinDuration = this.chargeMinDuration;
        newConfig.effect = this.effect;
        newConfig.locator = this.locator;
        newConfig.buffInfo = this.buffInfo;
        newConfig.playBuffAni = this.playBuffAni;
        newConfig.effectPos = this.effectPos;

        return newConfig;
    }
}

[System.Serializable]
public class OperationConfig
{
    public int changePhase;
    public int[] changeSkillIDs;

    public OperationConfig DoCopy()
    {
        var config = new OperationConfig();
        config.changePhase = this.changePhase;
        config.changeSkillIDs = new int[this.changeSkillIDs.Length];
        for (int i=0; i<this.changeSkillIDs.Length; ++i)
            config.changeSkillIDs[i] = this.changeSkillIDs[i];

        return config;
    }
}

[System.Serializable]
public class SkillJoystickConfig
{
    public SkillJoystickMode mode;
    public bool notDisplayLineEffect;
    public bool dontRemoveJoystick;
    public string effectName;
    public int effectMoveRadius;
    public Vector3 effectMoveSpeed;

    public SkillJoystickConfig DoCopy()
    {
        var config = new SkillJoystickConfig();
        config.mode = this.mode;
        config.notDisplayLineEffect = this.notDisplayLineEffect;
        config.effectName = this.effectName;
        config.effectMoveRadius = this.effectMoveRadius;
        config.effectMoveSpeed = this.effectMoveSpeed;
        config.dontRemoveJoystick = this.dontRemoveJoystick;
        return config;
    }
}

public enum SkillAction
{
    CHANGE_ANIMATION = 0,
    DISPOSE_SKILL
}

[System.Serializable]
public class SkillEvent
{
    public EventCommand eventType;
    public SkillAction eventAction;
    public string paramter;
    public int workPhase;//生效的阶段，0表示整个技能阶段有效
    public string attachActionName;  //挂件动画对应的技能配置文件名称
}

public class DSkillData : ScriptableObject
{
    public string           _name;
    public int              skillID;
    public int              skillPriority = 1;
    public int[]            skillPhases = new int[0]; //技能阶段
    public bool             relatedAttackSpeed = false;
    public int              attackSpeed = 1000;
    public int              isLoop = 0;
    public bool             notLoopLastFrame = false;
    public bool             loopAnimation = false;
    public string           hitEffect = "";
    public GameObject       goHitEffect = null;//被击特效
    public DAssetObject     goHitEffectAsset;
    public int              hitEffectInfoTableId = 0;
    public Object goSFX = null;//被击音效
    public DAssetObject goSFXAsset;
	public int 				hitSFXID;
    public int              hurtType;
    public float            hurtTime;
    public int              hurtPause;
    public float            hurtPauseTime;
    public float              forcex;
    public float              forcey;
    public string           description;
    public GameObject       characterPrefab;
    public DAssetObject     characterAsset;
    public int fps           = 60;
    public string           animationName;
    public string           moveName;
    public WrapMode         wrapMode;
    public float            interpolationSpeed  = 0;
    public float            animationSpeed      = 1;
    public int              totalFrames         = 15;
    public int              startUpFrames       = 5;
    public int              activeFrames        = 5;
    public int              recoveryFrames      = 5;
    public bool             useSpellBar = false;
	public float 			spellBarTime = 0f;

    public int              comboStartFrame;
    public int              comboSkillID;
    public float            skilltime;

	public bool cameraRestore;
	public float cameraRestoreTime;

    //grab info
    public float    grabPosx;
    public float    grabPosy;
    public float    grabEndForceX;
    public float    grabEndForceY;
    public bool     hitSpreadOut;
    public float    grabTime;
    public int      grabEndEffectType;//0 hurt, 1 fall
    public int      grabAction;
    public int      grabNum;
    public float    grabMoveSpeed;
	public bool 	grabSupportQuickPressDismis;
    public bool     notGrabBati;             //不能抓取霸体玩家
    public bool     notGrabGeDang;           //不能抓取格挡玩家
    public bool     notUseGrabSetPos;        //不使用抓取配置中的位置更新
    public bool     notGrabToBlock;          //不能抓取进阻挡
    public int      buffInfoId;              //给抓取玩家添加的BuffInfoId（抓取结束后移除)
    public int      buffInfoIdToSelf;        //抓取判定到目标以后给自己添加的BuffInfoId（抓取结束后移除)
    public int buffInfoIDToOther;      //抓取判定到目标以后给目标添加的BuffInfoId
    //charge
    public bool isCharge = false;
    public ChargeConfig chargeConfig;

    //speicalOperation
    public bool isSpeicalOperate = false;   
    public OperationConfig operationConfig;
    public bool isUseSelectSeatJoystick = false;        //使用选择玩家技能摇杆

    public int actionSelectPhase;
    public int[] actionSelect = new int[0];
    public string[] actionIconPath = new string[0];

    //skill joystick
    public SkillJoystickConfig skillJoystickConfig;

    //事件
    public SkillEvent[] skillEvents = new SkillEvent[0];

    public TriggerNextPhaseType triggerType;

    public HurtDecisionBox[]    HurtBlocks          = new HurtDecisionBox[0];
    public DefenceDecisionBox[] DefenceBlocks       = new DefenceDecisionBox[0];
    public EntityAttachFrames[] attachFrames        = new EntityAttachFrames[0];
    public EffectsFrames[]      effectFrames        = new EffectsFrames[0];
    public EntityFrames[]       entityFrames        = new EntityFrames[0];

    public DSkillFrameTag[]     frameTags           = new DSkillFrameTag[0];
    public DSkillFrameGrap[]    frameGrap           = new DSkillFrameGrap[0];
    public DSkillFrameStateOp[]     stateop             = new DSkillFrameStateOp[0];
    public DSkillPropertyModify[]   properModify        = new DSkillPropertyModify[0];
    public DSkillFrameEventSceneShock[] shocks          = new DSkillFrameEventSceneShock[0];
    public DSkillSfx[] sfx                              = new DSkillSfx[0];
    public DSkillFrameEffect[] frameEffects             = new DSkillFrameEffect[0];
    public DSkillCameraMove[] cameraMoves               = new DSkillCameraMove[0];
    public DSkillWalkControl[] walkControl              = new DSkillWalkControl[0];
	public DActionData[] actions 						= new DActionData[0];

    public DSkillBuff[] buffs                           = new DSkillBuff[0];
    public DSkillSummon[] summons                       = new DSkillSummon[0];
    public DSkillMechanism[] mechanisms                 = new DSkillMechanism[0];

#if UNITY_EDITOR
    [System.NonSerialized]
    static public VisiableFliter vfliter = (VisiableFliter)VisiableFliterAll.All;

    public static bool CheckVFliter(VisiableFliter vfliter, VisiableFliter mask)
    {
        return (vfliter & mask)!= 0;
    }

    [System.NonSerialized]
    static public float time = 0.0f;
    [System.NonSerialized]
    static public bool  play = false;

    [System.NonSerialized]
    static public GameObject root;

    [System.NonSerialized]
    static public GameObject actor;

    [System.NonSerialized]
    static public GameObject attach;
#endif
};

