using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DBoxImp
{
    public DBox  vBox;
    //public DBox  vWorldBox;
   // public DBox3 vWorld3Box;
}

public class DBoxImp2
{
    //public DBox  vBox;
    public DBox  vWorldBox;
}

public class BDDBoxData
{
    public List<DBoxImp> vBox = new List<DBoxImp>();
    public int hurtID;
    /*
    0:不指定
    1:敌人
    2:友方
    */
    public int hurtType = 1;

	public float zDim/* = 3.0f;*/
	{
		set {
			//encryptedZDim = IntMath.Float2Int(value * GlobalLogic.VALUE_1000);
            encryptedZDim = new VInt(value);
        }
	}

    public VInt zDimInt
    {
        get{
            return encryptedZDim;
        }
    }

	private VInt encryptedZDim;
}

public class BDGrabData
{
    public float posx;
    public float posy;
    public float endForcex;
    public float endForcey;
    public bool hitSpreadOut;    // 是否击散
    public int duraction;
    public int endForceType;
    public int action; //被抓取时候的动作
    public int grabNum;
    public float grabMoveSpeed;
	public bool quickPressDismis;
    public bool notGrabBati;     //不能抓取霸体玩家
    public bool notGrabGeDang;   //不能抓取格挡玩家
    public bool notUseGrabSetPos;//不使用抓取配置中的位置更新
    public bool notGrabToBlock;  //不能抓取进阻挡
    public int buffInfoId;       //给抓取玩家添加的BuffInfoId（抓取结束后移除)
    public int buffInfoIdToSelf; //抓取判定到目标以后给自己添加的BuffInfoId（抓取结束后移除)
    public int buffInfoIDToOther;//抓取判定到目标以后给目标添加的BuffInfoId
    public BDGrabData()
    {
        posx = 0;
        posy = 0;
        endForcex = 0;
        endForcey = 0;
        duraction = 0;
        endForceType = 0; //0 普通 1 float
        grabNum = 0;
        grabMoveSpeed = 0;
        hitSpreadOut = false;
        action =(int) ActionType.ActionType_HURT;
    }
}

// public class BDMoveData
// {
//     public float fMoveSpeed;
// }

public class BDEntityActionFrameData
{
    //防御框
    public BDDBoxData pDefenseData;
    //攻击框
    public BDDBoxData pAttackData;
    //public BDMoveData pMoveData;
    public List<BDEventBase> pEvents = new List<BDEventBase>();

    //攻击范围
    //public VInt2 kRange;
    //public VInt2 kGrapPosition;

    public SeFlag kFlag = new SeFlag();

    public BDEntityActionFrameData()
    {
        //kRange.x = VInt.NewVInt(-10,1).i;
        //kRange.y = VInt.NewVInt(10,1).i;
    }
}


public struct BeAnimationFrame
{
    public float start;
    public string anim;
    public float speed;
}

public class BeAttachFrames
{
    public string name;
    public int resID;
    public float start;
    public float end;
    public string attachName;
    public GameObject entityPrefab;
    public DAssetObject entityAsset;
    public BeAnimationFrame[] animations = new BeAnimationFrame[0];

}

public class BDEntityActionInfo
{
    public int weaponType;
    public int weaponTag;

    public string key;
    public string moveName;
    public int    skillID;
    public bool relatedAttackSpeed = false;
    public int attackSpeed = 0;
    public string actionName;
    public int   iLogicFramesNum;
    public VFactor fLogicFrameDeltaTime;
    public int  fRealFramesTime;
    //public float fFramesSpeed;
    public bool  bLoop;
    public GameObject hitEffect;
    public DAssetObject hitEffectAsset;
    public int hitEffectInfoTableId;
    //public UnityEngine.Object hitSFX;
	public int hitSFXID;
	public ProtoTable.SoundTable hitSFXIDData = null;
    public int skillTotalTime;
    public float animationspeed;

    public int[] skillPhases = new int[0];

    public TriggerNextPhaseType triggerType;

    //public int iLogicStartFrame;
    //public int iLogicEndFrame;
//    public int iActionType;
    //public float fActionHurtLevel;
//    public float fActionForcex;
    //public float fActionForcey;
    public float hurtTime;
    //public bool hurtPause;
    //public float hurtPauseTime;

    public int comboStartFrame;
    public int comboSkillID;

    //public int skillPriority;

    //public SeFlag exFlag = new SeFlag(0);

    public BDGrabData grabData = Global.gStaticGrabData;

    public bool useSpellBar;
	public float spellBarTime;

    public List<BDEntityActionFrameData>    vFramesData = new List<BDEntityActionFrameData>();
    //public List<BDDBoxData>                 vDecisionBoxData;
    public List<BeAttachFrames>             vAttachFrames = new List<BeAttachFrames>();

    public bool useCharge = false;
    public ChargeConfig chargeConfig = Global.gStaticChargeData;

    public bool useSpecialOperation = false;
    public OperationConfig operationConfig = Global.gStaticOpConfig;
    public bool useSelectSeatJoystick = false;      //使用选择玩家摇杆

    public int actionSelectPhase;
    public int[] actionSelect = new int[0];
    public string[] actionIconPath = new string[0];

    public SkillJoystickConfig skillJoystickConfig = Global.gStaticJoyConfig;

    public SkillEvent[] skillEvents = null;

	public bool cameraRestore;
	public float cameraRestoreTime;

    //函数指针
    //public delegate void Del(BDEntityActionFrameData state, DSkillFrameEvent frameEvent);

    //public bool bAnimLoop;


    public BDEntityActionInfo(string k)
    {
        key = k;
        //fActionHurtLevel = 60.0f;
        //fFramesSpeed = 1.0f;
       // fActionForcex = 4;
       // fActionForcey = 0;
//        iActionType = 0;
        bLoop = false;
        //bAnimLoop = false;
    }

    void SetVector3(ref Vector3 dest, FBSkillData.Vector3 source)
    {
        dest.x = source.X;
        dest.y = source.Y;
        dest.z = source.Z;
    }

    void SetVec3(ref Vec3 dest, FBSkillData.Vector3 source)
    {
        dest.x = source.X;
        dest.y = source.Y;
        dest.z = source.Z;
    }

    void SetVector2(ref Vector2 dest, FBSkillData.Vector2 source)
    {
        dest.x = source.X;
        dest.y = source.Y;
    }

    void SetQuan(ref Quaternion dest, FBSkillData.Quaternion source)
    {
        dest.x = source.X;
        dest.y = source.Y;
        dest.z = source.Z;
        dest.w = source.W;
    }

    void SetEffectsFrames(ref EffectsFrames effectsFrames, FBSkillData.EffectsFrames source)
    {
        effectsFrames.name = source.Name;
        effectsFrames.startFrames = source.StartFrames;
        effectsFrames.endFrames = source.EndFrames;
        effectsFrames.attachname = source.Attachname;
        effectsFrames.playtype = (EffectPlayType)source.Playtype;
        effectsFrames.timetype = (EffectTimeType)source.Timetype;
        effectsFrames.time = source.Time;
        effectsFrames.effectAsset = new DAssetObject(source.EffectAsset);
        effectsFrames.attachPoint = (EffectAttachPoint)source.AttachPoint;
        SetVector3(ref effectsFrames.localPosition, source.LocalPosition);
        SetQuan(ref effectsFrames.localRotation, source.LocalRotation);
        SetVector3(ref effectsFrames.localScale, source.LocalScale);
        effectsFrames.effecttype = source.Effecttype;
        effectsFrames.loop = source.Loop;
        effectsFrames.loopLoop = source.LoopLoop;
        effectsFrames.onlyLocalSee = source.OnlyLocalSee;
    }

    void SetEntityFrames(ref EntityFrames dest, FBSkillData.EntityFrames source)
    {
        dest.name = source.Name;
        dest.resID = source.ResID;
        dest.randomResID = source.RandomResID;
        dest.resIDList = new int[source.ResIDListLength];
        for (int i = 0; i < source.ResIDListLength; i++)
        {
            dest.resIDList[i] = source.GetResIDList(i);
        }
        dest.type = (EntityType)source.Type;
        dest.startFrames = source.StartFrames;
        dest.entityAsset = new DAssetObject(source.EntityAsset);
        SetVector2(ref dest.gravity, source.Gravity);
        dest.speed = source.Speed;
        dest.angle = source.Angle;
        dest.isAngleWithEffect = source.IsAngleWithEffect;
        SetVector2(ref dest.emitposition, source.Emitposition);
        dest.emitPositionZ = source.EmitPositionZ;
        dest.axisType = (AxisType)source.AxisType;
        dest.shockTime = source.ShockTime;
        dest.shockSpeed = source.ShockSpeed;
        dest.shockRangeX = source.ShockRangeX;
        dest.shockRangeY = source.ShockRangeY;
        dest.isRotation = source.IsRotation;
        dest.rotateSpeed = source.RotateSpeed;
        dest.moveSpeed = source.MoveSpeed;
        dest.rotateInitDegree = source.RotateInitDegree;

        dest.sceneShock = new ShockInfo();
        var sourceShock = source.SceneShock;
        dest.sceneShock.shockRangeX = source.ShockRangeX;
        dest.sceneShock.shockRangeY = source.ShockRangeY;
        dest.sceneShock.shockSpeed = source.ShockSpeed;
        dest.sceneShock.shockTime = source.ShockTime;

        dest.hitFallUP = source.HitFallUP;
        dest.forceY = source.ForceY;
        dest.hurtID = source.HurtID;
        dest.lifeTime = source.LifeTime;
        dest.hitThrough = source.HitThrough;
        dest.hitCount = source.HitCount;
        dest.distance = source.Distance;
		dest.attackCountExceedPlayExtDead = source.AttackCountExceedPlayExtDead;
        dest.hitGroundClick = source.HitGroundClick;
        dest.delayDead = source.DelayDead;
        dest.offsetType = (OffsetType)source.OffsetType;
        dest.targetChooseType = (TargetChooseType)source.TargetChooseType;
        SetVector2(ref dest.range, source.Range);
        dest.boomerangeDistance = source.BoomerangeDistance;
        dest.stayDuration = source.StayDuration;
        dest.paraSpeed = source.ParaSpeed;
        dest.paraGravity = source.ParaGravity;
        dest.useRandomLaunch = source.UseRandomLaunch;
        dest.randomLaunchInfo = new RandomLaunchInfo();
        var rli = source.RandomLaunchInfo;
        dest.randomLaunchInfo.num = rli.Num;
        dest.randomLaunchInfo.isNumRand = rli.IsNumRand;
        SetVector2(ref dest.randomLaunchInfo.numRandRange, rli.NumRandRange);
        dest.randomLaunchInfo.interval = rli.Interval;
        dest.randomLaunchInfo.rangeRadius = rli.RangeRadius;
        dest.randomLaunchInfo.isFullScene = rli.IsFullScene;

        dest.onCollideDie = source.OnCollideDie;
        dest.onXInBlockDie = source.OnXInBlockDie;
        dest.changForceBehindOther = source.ChangeForceBehindOther;

        dest.changeFace = source.ChangeFace;

        dest.changeMaxAngle = source.ChangeMaxAngle;
        dest.chaseTime = source.ChaseTime;
    }

    string GetSafeString(string value)
    {
        if(string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        return value;
    }
    public bool InitWithFlatBuffer(FBSkillData.FBSkillData dataRes)
    {
        if (dataRes == null)
        {
            return false;
        }
        moveName = GetSafeString(dataRes.MoveName);
        skillID = dataRes.SkillID;

        actionName = GetSafeString(dataRes.AnimationName);
        relatedAttackSpeed = dataRes.RelatedAttackSpeed;
        attackSpeed = dataRes.AttackSpeed;
        iLogicFramesNum = dataRes.TotalFrames;
        hitEffect = null;
        hitEffectAsset = new DAssetObject(dataRes.HitEffect);
        hitEffectInfoTableId = 0;
        //hitSFX = null;
        hitSFXID = dataRes.HitSFXID;
        if (hitSFXID > 0)
            hitSFXIDData = TableManager.GetInstance().GetTableItem<ProtoTable.SoundTable>(hitSFXID);
 //       iActionType = dataRes.HurtType;
        //fActionForcex = dataRes.Forcex;
        //fActionForcey = dataRes.Forcey;
        animationspeed = dataRes.AnimationSpeed;

        if (dataRes.HurtTime > 0)
            hurtTime = dataRes.HurtTime;
        // if (dataRes.HurtPause == 1)
        //     hurtPause = true;
        //hurtPauseTime = dataRes.HurtPauseTime;
        if (dataRes.Skilltime > 0)
            skillTotalTime =  IntMath.Float2Int(dataRes.Skilltime,1000);
        //skillPriority = dataRes.SkillPriority;
        fLogicFrameDeltaTime = new VFactor(1,dataRes.Fps); //0.016f;//1.0f / 60.0f; //0.019f;//hard code 60fps
        // TODO *1000 convert to int 
        fRealFramesTime = (iLogicFramesNum - 1) * 1000 / dataRes.Fps;
        //fFramesSpeed = 1.0f;

        if (dataRes.IsLoop != 0)
            bLoop = true;

        if (dataRes.LoopAnimation)
        {
            //bAnimLoop = dataRes.LoopAnimation;
        }

        comboStartFrame = dataRes.CmboStartFrame;
        comboSkillID = dataRes.CmboSkillID;

        int skillPhaseLength = dataRes.SkillPhasesLength;
        if (skillPhaseLength > 0)
        {
            skillPhases = new int[skillPhaseLength];

            for (int ct = 0; ct < skillPhaseLength; ++ct)
            {
                skillPhases[ct] = dataRes.GetSkillPhases(ct);
            }
        }

        this.triggerType = (TriggerNextPhaseType)dataRes.TriggerType;

        useSpellBar = dataRes.UeSpellBar;
        if (useSpellBar)
        {
            spellBarTime = dataRes.SellBarTime;
        }

        useCharge = dataRes.IsCharge;
        if (useCharge)
        {
            var source = dataRes.ChargeConfig;
            this.chargeConfig = new ChargeConfig();

            chargeConfig.repeatPhase = source.RepeatPhase;
            chargeConfig.changePhase = source.ChangePhase;
            chargeConfig.switchPhaseID = source.SwitchPhaseID;
            chargeConfig.chargeDuration = source.ChargeDuration;
            chargeConfig.chargeMinDuration = source.ChargeMinDuration;
            chargeConfig.effect = source.Effect;
            chargeConfig.locator = source.Locator;
            chargeConfig.buffInfo = source.BuffInfo;
            chargeConfig.playBuffAni = source.PlayBuffAni;
        }

        useSpecialOperation = dataRes.IsSpeicalOperate;
        if (useSpecialOperation)
        {
            this.operationConfig = new OperationConfig();

            var source = dataRes.OperationConfig;
            operationConfig.changePhase = source.ChangePhase;
            var len = source.ChangeSkillIDsLength;
            operationConfig.changeSkillIDs = new int[len];
            for (int ct = 0; ct < len; ++ct)
            {
                operationConfig.changeSkillIDs[ct]
                = source.GetChangeSkillIDs(ct);
            }
        }
        useSelectSeatJoystick = dataRes.IsUseSelectSeatJoystick;
        //if (useSelectSeatJoystick)
        {
            skillJoystickConfig = new SkillJoystickConfig();
            var source = dataRes.SkillJoystickConfig;
            skillJoystickConfig.mode = (SkillJoystickMode)source.Mode;
            skillJoystickConfig.effectName = GetSafeString(source.EffectName);
            skillJoystickConfig.notDisplayLineEffect = source.NotDisplayLineEffect;
            skillJoystickConfig.dontRemoveJoystick = source.DontRemoveJoystick;
            skillJoystickConfig.effectMoveRadius = source.EffectMoveRadius;
            SetVector3(ref skillJoystickConfig.effectMoveSpeed, source.EffectMoveSpeed);
        }

        int actionSelectLength = dataRes.ActionSelectLength;
        if (actionSelectLength > 0)
        {
            actionSelectPhase = dataRes.ActionSelectPhase;
            actionSelect = new int[actionSelectLength];
            for (int i = 0; i < actionSelectLength; i++)
            {
                actionSelect[i] = dataRes.GetActionSelect(i);
            }
        }

        {
            var skillEventsLength = dataRes.SkillEventsLength;
            skillEvents = new SkillEvent[skillEventsLength];
            for (int ct = 0; ct < skillEventsLength; ++ct)
            {
                var skillitem = dataRes.GetSkillEvents(ct);
                skillEvents[ct] = new SkillEvent();
                skillEvents[ct].eventType = (EventCommand)skillitem.EventType;
                skillEvents[ct].eventAction = (SkillAction)skillitem.EventAction;
                skillEvents[ct].paramter = skillitem.Paramter;
                skillEvents[ct].workPhase = skillitem.WorkPhase;
            }
        }

        cameraRestore = dataRes.CameraRestore;
        cameraRestoreTime = dataRes.CameraRestoreTime;

        if (dataRes.GrabNum >= 1)
        {
            this.grabData = new BDGrabData();

            //抓取信息
            grabData.endForcex = dataRes.GrabEndForceX;
            grabData.endForcey = dataRes.GrabEndForceY;
            grabData.hitSpreadOut = dataRes.HitSpreadOut;
            grabData.posx = dataRes.GrabPosx;
            grabData.posy = dataRes.GrabPosy;
            grabData.grabNum = dataRes.GrabNum;
            grabData.grabMoveSpeed = dataRes.GrabMoveSpeed;

            var grabtime = dataRes.GrabTime;
            if (grabtime > 0)
                grabData.duraction = IntMath.Float2Int(grabtime * 1000);

            grabData.endForceType = dataRes.GrabEndEffectType;
            var grabAction = dataRes.GrabAction;
            if (grabAction != 0)
            {
                grabData.action = grabAction;
            }
            grabData.quickPressDismis = dataRes.GrabSupportQuickPressDismis;
            grabData.notGrabBati = dataRes.NotGrabBati;
            grabData.notGrabGeDang = dataRes.NotGrabGeDang;
            grabData.notUseGrabSetPos = dataRes.NotUseGrabSetPos;
            grabData.notGrabToBlock = dataRes.NotGrabToBlock;
            grabData.buffInfoId = dataRes.BuffInfoId;
            grabData.buffInfoIdToSelf = dataRes.BuffInfoIdToSelf;
            grabData.buffInfoIDToOther = dataRes.BuffInfoIdToOther;
        }


        
        //框
        for (int iFrame = 0; iFrame < iLogicFramesNum; ++iFrame)
        {
            BDEntityActionFrameData fdata = new BDEntityActionFrameData();

            //攻击框
            fdata.pAttackData = new BDDBoxData();
            var hurtBox = dataRes.GetHurtBlocks(iFrame);
            var boxLength = hurtBox.BoxsLength;
            if (hurtBox != null && boxLength > 0)
            {
                fdata.pAttackData.zDim = hurtBox.ZDim;
                fdata.pAttackData.hurtID = hurtBox.HurtID;

                if (fdata.pAttackData.hurtID > 0)
                {
                    //设置攻击框的类型
                    var hurtData = TableManager.GetInstance().GetTableItem<ProtoTable.EffectTable>(fdata.pAttackData.hurtID);
                    if (hurtData != null)
                    {
                        fdata.pAttackData.hurtType = (int)hurtData.EffectTargetType;
                    }
                    else
                    {
                        Logger.LogErrorFormat("技能配置文件{0}的攻击框没有{1}的触发效果ID", dataRes._name, fdata.pAttackData.hurtID);
                    }
                }

                for (int iHurt = 0; iHurt < boxLength; ++iHurt)
                {
                    var shapeBox = hurtBox.GetBoxs(iHurt);
                    var sizeX = Mathf.Abs(shapeBox.Size.X);
                    var sizeY = Mathf.Abs(shapeBox.Size.Y);
                    var CenterX = shapeBox.Center.X;
                    var CenterY = shapeBox.Center.Y;

                    DBoxImp boximp = new DBoxImp();
                    boximp.vBox._min.x = VInt.Float2VIntValue(CenterX - sizeX / 2.0f);
                    boximp.vBox._min.y = VInt.Float2VIntValue(CenterY - sizeY / 2.0f);
                    boximp.vBox._max.x = VInt.Float2VIntValue(CenterX + sizeX / 2.0f);
                    boximp.vBox._max.y = VInt.Float2VIntValue(CenterY + sizeY / 2.0f);

                    fdata.pAttackData.vBox.Add(boximp);
                }
            }

            //防御框
            fdata.pDefenseData = new BDDBoxData();
            var defBox = dataRes.GetDefenceBlocks(iFrame);
            var defBoxLength = defBox.BoxsLength;
            if (defBox != null && defBoxLength > 0)
            {
                for (int i = 0; i < defBoxLength; ++i)
                {
                    var shapeBox = defBox.GetBoxs(i);
                    var shapeBoxSizeX = Mathf.Abs(shapeBox.Size.X);
                    var shapeBoxSizeY = Mathf.Abs(shapeBox.Size.Y);
                    var CenterX = shapeBox.Center.X;
                    var CenterY = shapeBox.Center.Y;
                    DBoxImp box = new DBoxImp();
                    box.vBox._min.x = VInt.Float2VIntValue(CenterX - shapeBoxSizeX / 2.0f);
                    box.vBox._min.y = VInt.Float2VIntValue(CenterY - shapeBoxSizeY / 2.0f);
                    box.vBox._max.x = VInt.Float2VIntValue(CenterX + shapeBoxSizeX / 2.0f);
                    box.vBox._max.y = VInt.Float2VIntValue(CenterY + shapeBoxSizeY / 2.0f);

                    fdata.pDefenseData.vBox.Add(box);
                }
            }

            vFramesData.Add(fdata);
        }


        //插入事件帧
        //特效
        var effectFrameCount = dataRes.EffectFramesLength;
        if (effectFrameCount > 0)
        {
            for (int i = 0; i < effectFrameCount; ++i)
            {
                var effectData = dataRes.GetEffectFrames(i);
                EffectsFrames frames = new EffectsFrames();
                SetEffectsFrames(ref frames, effectData);
                BDPlayEffect effectEvent = new BDPlayEffect(frames);
                int frame = frames.startFrames;
                if (frame >= 0 && frame < vFramesData.Count)
                {
                    BDEntityActionFrameData frameData = vFramesData[frame];
                    if (frameData != null)
                    {
                        frameData.pEvents.Add(effectEvent);
                    }
                }

            }
        }


        //entity
        var enityFramesLen = dataRes.EntityFramesLength;
        if (enityFramesLen > 0)
        {
            for (int i = 0; i < enityFramesLen; ++i)
            {
                var entityData = dataRes.GetEntityFrames(i);
                EntityFrames entityFrame = new EntityFrames();
                SetEntityFrames(ref entityFrame, entityData);
                BDGenProjectile entityEvent = new BDGenProjectile(entityFrame);

                int frame = entityData.StartFrames;
                if (frame >= vFramesData.Count)
                {
                    continue;
                }
                BDEntityActionFrameData frameData = vFramesData[frame];
                if (frameData != null)
                {
                    frameData.pEvents.Add(entityEvent);
                }
            }
        }

        //帧事件
        var frameTagsLen = dataRes.FrameTagsLength;
        if (frameTagsLen > 0)
        {
            //foreach(var frameEvent in dataRes.frameTags)
            for (int i = 0; i < frameTagsLen; ++i)
            {
                var frameEvent = dataRes.GetFrameTags(i);
                int frame = frameEvent.Startframe;
                if (frame < 0 || frame >= vFramesData.Count)
                {
                    //Logger.LogErrorFormat("FrameTag {0}: startFrame is out of array, if you split the animation to two animation, please check two animation's frame tag config", dataRes.animationName);
                    continue;
                }

                BDEntityActionFrameData frameData = vFramesData[frame];

                int tag = (int)frameEvent.Tag;
                if (BeUtility.CheckHaveTag(tag, (int)DSFFrameTags.TAG_NEWDAMAGE))
                {
                    frameData.kFlag.SetFlag((int)DSFFrameTags.TAG_NEWDAMAGE);
                }
                else if (BeUtility.CheckHaveTag(tag, (int)DSFFrameTags.TAG_LOCKZSPEED))
                {
                    frameData.kFlag.SetFlag((int)DSFFrameTags.TAG_LOCKZSPEED);
                }
                else if (BeUtility.CheckHaveTag(tag, (int)DSFFrameTags.TAG_LOCKZSPEEDFREE))
                {
                    frameData.kFlag.SetFlag((int)DSFFrameTags.TAG_LOCKZSPEEDFREE);
                }
                else if (BeUtility.CheckHaveTag(tag, (int)DSFFrameTags.TAG_IGNORE_GRAVITY))
                {
                    frameData.kFlag.SetFlag((int)DSFFrameTags.TAG_IGNORE_GRAVITY);
                }
                else if (BeUtility.CheckHaveTag(tag, (int)DSFFrameTags.TAG_RESTORE_GRAVITY))
                {
                    frameData.kFlag.SetFlag((int)DSFFrameTags.TAG_RESTORE_GRAVITY);
                }
                else if (BeUtility.CheckHaveTag(tag, (int)DSFFrameTags.TAG_SET_TARGET_POS_XY))
                {
                    frameData.kFlag.SetFlag((int)DSFFrameTags.TAG_SET_TARGET_POS_XY);
                }
                else if (BeUtility.CheckHaveTag(tag, (int)DSFFrameTags.TAG_CURFRAME))
                {
                    frameData.kFlag.SetFlag((int)DSFFrameTags.TAG_CURFRAME, frameEvent.TagFlag);
                }
                else if (BeUtility.CheckHaveTag(tag, (int)DSFFrameTags.TAG_CHANGEFACE))
                {
                    frameData.kFlag.SetFlag((int)DSFFrameTags.TAG_CHANGEFACE);
                }
                else if (BeUtility.CheckHaveTag(tag, (int)DSFFrameTags.TAG_CHANGEFACEBYDIR))
                {
                    frameData.kFlag.SetFlag((int)DSFFrameTags.TAG_CHANGEFACEBYDIR);
                }
                else if (BeUtility.CheckHaveTag(tag, (int)DSFFrameTags.TAG_REMOVEEFFECT))
                {
                    frameData.kFlag.SetFlag((int)DSFFrameTags.TAG_REMOVEEFFECT, frameEvent.TagFlag);
                }
                else if (BeUtility.CheckHaveTag(tag, (int)DSFFrameTags.TAG_STARTCHECKHIT))
                {
                    frameData.kFlag.SetFlag((int)DSFFrameTags.TAG_STARTCHECKHIT);
                }
                else if (BeUtility.CheckHaveTag(tag, (int)DSFFrameTags.TAG_STARTDEALSKIPPHASE))
                {
                    frameData.kFlag.SetFlag((int)DSFFrameTags.TAG_STARTDEALSKIPPHASE);
                }

                else if (BeUtility.CheckHaveTag(tag, (int)DSFFrameTags.TAG_NAME_HIDE))
                {
                    frameData.kFlag.SetFlag((int)DSFFrameTags.TAG_NAME_HIDE);
                }
                else if (BeUtility.CheckHaveTag(tag, (int)DSFFrameTags.TAG_NAME_SHOW))
                {
                    frameData.kFlag.SetFlag((int)DSFFrameTags.TAG_NAME_SHOW);
                }
                else if (BeUtility.CheckHaveTag(tag, (int)DSFFrameTags.TAG_SHADOW_HIDE))
                {
                    frameData.kFlag.SetFlag((int)DSFFrameTags.TAG_SHADOW_HIDE);
                }
                else if (BeUtility.CheckHaveTag(tag, (int)DSFFrameTags.TAG_SHADOW_SHOW))
                {
                    frameData.kFlag.SetFlag((int)DSFFrameTags.TAG_SHADOW_SHOW);
                }
                else if (BeUtility.CheckHaveTag(tag, (int)DSFFrameTags.TAG_HPBAR_HIDE))
                {
                    frameData.kFlag.SetFlag((int)DSFFrameTags.TAG_HPBAR_HIDE);
                }
                else if (BeUtility.CheckHaveTag(tag, (int)DSFFrameTags.TAG_HPBAR_SHOW))
                {
                    frameData.kFlag.SetFlag((int)DSFFrameTags.TAG_HPBAR_SHOW);
                }
                else if (BeUtility.CheckHaveTag(tag, (int)DSFFrameTags.TAG_LOOKAT_TARGET))
                {
                    frameData.kFlag.SetFlag((int)DSFFrameTags.TAG_LOOKAT_TARGET);
                }
                else if (BeUtility.CheckHaveTag(tag, (int)DSFFrameTags.TAG_REMOVE_BUFF))
                {
                    frameData.kFlag.SetFlag((int)DSFFrameTags.TAG_REMOVE_BUFF, frameEvent.TagFlag);
                }
                else if (BeUtility.CheckHaveTag(tag, (int)DSFFrameTags.TAG_REMOVE_MECHANISM))
                {
                    frameData.kFlag.SetFlag((int)DSFFrameTags.TAG_REMOVE_MECHANISM, frameEvent.TagFlag);
                }
                else if (BeUtility.CheckHaveTag(tag, (int)DSFFrameTags.TAG_OPEN_2ND_STATE))
                {
                    frameData.kFlag.SetFlag((int)DSFFrameTags.TAG_OPEN_2ND_STATE);
                }
                else if (BeUtility.CheckHaveTag(tag, (int)DSFFrameTags.TAG_CLOSE_2ND_STATE))
                {
                    frameData.kFlag.SetFlag((int)DSFFrameTags.TAG_CLOSE_2ND_STATE);
                }
            }
        }

        //抓取
        var frameGrapLen = dataRes.FrameGrapLength;
        if (frameGrapLen > 0)
        {
            for (int k = 0; k < frameGrapLen; ++k)
            {
                var frameEvent = dataRes.GetFrameGrap(k);
                BDEventBase instance = null;
                int frame = frameEvent.Startframe;

                if (frame < vFramesData.Count)
                {
                    var op = (DSFGrapOp)frameEvent.Op;
                    BDEntityActionFrameData frameData = vFramesData[frame];
                    if (op == DSFGrapOp.GRAP_INTERRUPT ||
                        op == DSFGrapOp.GRAP_CHANGE_TARGETPOS ||
                        op == DSFGrapOp.GRAP_STOPCHANGE_TARGETPOS ||
                        op == DSFGrapOp.GRAP_CHANGE_TARGETACTION ||
                        op == DSFGrapOp.GRAP_CHANGE_TARGETROTATION)
                    {
                        Vec3 pos = new Vec3();
                        SetVec3(ref pos, frameEvent.TargetPos);
                        BDSkillSuspend suspend = new BDSkillSuspend((int)op, new VInt3(pos), frameEvent.FaceGraber, (int)frameEvent.TargetAction, frameEvent.TargetAngle);
                        instance = suspend;
                    }
                    else if (op == DSFGrapOp.GRAP_JUDGE ||
                            op == DSFGrapOp.GRAP_JUDGE_EXECUTE ||
                            op == DSFGrapOp.GRAP_JUDGE_SKIP_PHASE
                        )
                    {
                        var len = frameEvent.Length;
                        for (int i = 0; i < len; ++i)
                        {
                            if ((frame + i) < vFramesData.Count)
                            {
                                BDEntityActionFrameData tmpFrameData = vFramesData[frame + i];

                                tmpFrameData.kFlag.SetFlag((int)op);
                            }
                        }

                    }
                    else
                    {
                        frameData.kFlag.SetFlag((int)op);
                    }

                    if (instance != null)
                    {
                        frameData.pEvents.Add(instance);
                    }
                }
            }
        }

        //状态操作
        var stateOpLen = dataRes.StateopLength;
        if (stateOpLen > 0)
        {
            for (int i = 0; i < stateOpLen; ++i)
            //foreach(DSkillFrameStateOp frameEvent in dataRes.stateop)
            {
                var frameEvent = dataRes.GetStateop(i);
                BDEventBase instance = null;

                int frame = frameEvent.Startframe;
                if (frame < vFramesData.Count)
                {
                    BDEntityActionFrameData frameData = vFramesData[frame];
                    BDStateStackOP statckOP = new BDStateStackOP(
                        (int)frameEvent.Op,
                        (int)frameEvent.State,
                        frameEvent.Idata1,
                        frameEvent.Idata2,
                        frameEvent.Fdata1,
                        (int)frameEvent.Statetag);
                    instance = statckOP;

                    if (instance != null)
                    {
                        frameData.pEvents.Add(instance);
                    }
                }
            }
        }

        //属性修改
        var properModifyLen = dataRes.ProperModifyLength;
        if (properModifyLen > 0)
        {
            //foreach(DSkillPropertyModify frameEvent in dataRes.properModify)
            for (int i = 0; i < properModifyLen; ++i)
            {
                var frameEvent = dataRes.GetProperModify(i);
                BDEventBase instance = null;

                int frame = frameEvent.Startframe;
                for (int j = 0; j < frameEvent.Length && frame + j < vFramesData.Count; j++)
                {
                    BDEntityActionFrameData frameData = vFramesData[frame + j];
                    BDModifySpeed speed = new BDModifySpeed(
                        frameEvent.Tag,
                        (DSkillPropertyModifyType)frameEvent.Modifyfliter,
                        frameEvent.Value,
                        frameEvent.MovedValue,
                        frameEvent.JumpToTargetPos,
                        frameEvent.JoystickControl,
                        frameEvent.ValueAcc,
                        frameEvent.MovedValueAcc,
                        (DModifyXBackward)frameEvent.ModifyXBackward,
                        frameEvent.EachFrameModify,
                        frameEvent.UseMovedYValue,
                        frameEvent.MovedYValue,
                        frameEvent.MovedYValueAcc
                    );
                    instance = speed;

                    if (instance != null)
                        frameData.pEvents.Add(instance);
                }
            }
        }

        //camera效果
        var shockLen = dataRes.ShocksLength;
        if (shockLen > 0)
        {
            //foreach (DSkillFrameEventSceneShock frameEvent in dataRes.shocks)
            for (int i = 0; i < shockLen; ++i)
            {
                var frameEvent = dataRes.GetShocks(i);
                BDEventBase instance = null;

                int frame = frameEvent.Startframe;
                if (frame < vFramesData.Count)
                {
                    BDEntityActionFrameData frameData = vFramesData[frame];
                    BDSceneShock shock = null;
                    if (frameEvent.IsNew)
                    {
                        shock = shock = new BDSceneShock(frameEvent.Time, frameEvent.Num, frameEvent.Xrange, frameEvent.Yrange, frameEvent.Decelerate, frameEvent.Xreduce, frameEvent.Yreduce, frameEvent.Mode, frameEvent.Radius);                       
                    }
                    else
                    {
                        shock = new BDSceneShock(
                          frameEvent.Time, frameEvent.Speed, frameEvent.Xrange, frameEvent.Yrange);

                    }
                    instance = shock;

                    if (instance != null)
                        frameData.pEvents.Add(instance);
                }
            }
        }

        //移动控制
        var walkControlLen = dataRes.WalkControlLength;
        if (walkControlLen > 0)
        {
            //foreach(DSkillWalkControl frameEvent in dataRes.walkControl)
            for (int i = 0; i < walkControlLen; ++i)
            {
                var frameEvent = dataRes.GetWalkControl(i);
                BDEventBase instance = null;
                int frame = frameEvent.Startframe;
                if (frame < vFramesData.Count)
                {
                    BDEntityActionFrameData frameData = vFramesData[frame];
                    BDSkillWalkControl walk = new BDSkillWalkControl((SkillWalkMode)frameEvent.WalkMode, frameEvent.WalkSpeedPercent, frameEvent.UseSkillSpeed, frameEvent.WalkSpeedPercent2);

                    instance = walk;
                    if (instance != null)
                        frameData.pEvents.Add(instance);


                    /*                    if (frameEvent.walkMode == SkillWalkMode.FACEDIR || frameEvent.walkMode == SkillWalkMode.FREE)
                                        {
                                            BDSkillWalkControl walkStop = new BDSkillWalkControl(SkillWalkMode.FORBID, 0);
                                            frameData = vFramesData[frame+frameEvent.length-1];
                                            frameData.pEvents.Add(walkStop);
                                        }*/
                }
            }
        }

        //camera move
        var cameraMovesLen = dataRes.CameraMovesLength;
        if (cameraMovesLen > 0)
        {
            for (int i = 0; i < cameraMovesLen; ++i)
            //foreach(DSkillCameraMove frameEvent in dataRes.cameraMoves)
            {
                var frameEvent = dataRes.GetCameraMoves(i);
                BDEventBase instance = null;
                int frame = frameEvent.Startframe;
                if (frame >= 0 && frame < vFramesData.Count)
                {
                    BDEntityActionFrameData frameData = vFramesData[frame];
                    BDSkillCameraMove move = new BDSkillCameraMove(frameEvent.Offset, frameEvent.Duraction);
                    instance = move;
                    if (instance != null)
                        frameData.pEvents.Add(instance);
                }
            }
        }

        //sfx播放
        var sfxLen = dataRes.SfxLength;
        if (sfxLen > 0)
        {
            //foreach(var frameEvent in dataRes.sfx)
            for (int i = 0; i < sfxLen; ++i)
            {
                var frameEvent = dataRes.GetSfx(i);
                BDEventBase instance = null;
                int frame = frameEvent.Startframe;
                if (frame < vFramesData.Count)
                {
                    var frameData = vFramesData[frame];
                    var sfxEvent = new BDSkillSfx(
                        new DAssetObject(frameEvent.SoundClipAsset),
                        frameEvent.SoundID, frameEvent.Loop);

                    instance = sfxEvent;

                    if (instance != null)
                        frameData.pEvents.Add(instance);
                }
            }
        }

        //帧事件
        var frameEffectsLen = dataRes.FrameEffectsLength;
        if (frameEffectsLen > 0)
        {
            for (int i = 0; i < frameEffectsLen; ++i)
            {
                var frameEvent = dataRes.GetFrameEffects(i);
                BDEventBase instance = null;
                int frame = frameEvent.Startframe;
                if (frame < vFramesData.Count)
                {
                    var frameData = vFramesData[frame];
                    var duration = IntMath.Float2Int(frameEvent.BuffTime * 1000);
                    if (frameEvent.PhaseDelete)
                        duration = -1;
                    var effectEvent = new BDSkillFrameEffect(frameEvent.EffectID, duration, frameEvent.UseBuffAni,
                    frameEvent.UsePause, frameEvent.PauseTime, frameEvent.FinishDelete,frameEvent.FinishDeleteAll, frameEvent.MechanismId);

                    instance = effectEvent;

                    if (instance != null)
                        frameData.pEvents.Add(instance);
                }
            }
        }

        //
        var actionsLen = dataRes.ActionsLength;
        if (actionsLen > 0)
        {
            for (int i = 0; i < actionsLen; ++i)
            {
                var frameEvent = dataRes.GetActions(i);

                BDSkillAction instance = null;
                int frame = frameEvent.Startframe;
                if (frame < vFramesData.Count)
                {
                    Vec3 pos = new Vec3();
                    SetVec3(ref pos, frameEvent.DeltaPos);
                    var frameData = vFramesData[frame];
                    var action = new BDSkillAction(
                        (BeActionType)frameEvent.ActionType, frameEvent.Duration,
                        frameEvent.DeltaScale, new VInt3(pos), frameEvent.IgnoreBlock);
                    instance = action;
                    if (instance != null)
                        frameData.pEvents.Add(instance);
                }
            }
        }
        //挂件
        var attachFrameLen = dataRes.AttachFramesLength;
        for (int i = 0; i < attachFrameLen; ++i)
        {
            var attf = dataRes.GetAttachFrames(i);
            if (attf != null)
            {
                BeAttachFrames batf = new BeAttachFrames();
                batf.name = attf.Name;
                batf.resID = attf.ResID;
                batf.start = attf.Start;
                batf.end = attf.End;
                batf.entityPrefab = null;
                batf.entityAsset = new DAssetObject(attf.EntityAsset);
                batf.attachName = attf.AttachName;

                List<BeAnimationFrame> anims = new List<BeAnimationFrame>();
                var len = attf.AnimationsLength;
                for (int j = 0; j < len; ++j)
                {
                    BeAnimationFrame temp = new BeAnimationFrame();
                    var cur = attf.GetAnimations(j);
                    temp.anim = cur.Anim;
                    temp.start = cur.Start;
                    temp.speed = cur.Speed;
                    anims.Add(temp);
                }

                batf.animations = anims.ToArray();
                vAttachFrames.Add(batf);
            }
        }

        var buffsLength = dataRes.BuffsLength;
        if (buffsLength > 0)
        {
            for (int i = 0; i < buffsLength; ++i)
            {
                var buffSum = dataRes.GetBuffs(i);
                if (buffSum != null)
                {
                    BDEventBase instance = null;
                    int frame = buffSum.Startframe;
                    if (frame < vFramesData.Count)
                    {
                        var frameData = vFramesData[frame];
                        var duration = IntMath.Float2Int(buffSum.BuffTime * 1000);
                        if (buffSum.PhaseDelete || buffSum.FinishDeleteAll)
                            duration = -1;

                        List<int> buffInfoList = new List<int>();
                        var buffInfoLength = buffSum.BuffInfoListLength;
                        for (int j = 0; j < buffInfoLength; ++j)
                        {
                            buffInfoList.Add(buffSum.GetBuffInfoList(j));
                        }

                        var effectEvent = new BDAddBuffInfoOrBuff(buffSum.BuffID, buffInfoList, duration, buffSum.PhaseDelete, buffSum.FinishDeleteAll, buffSum.Level, buffSum.LevelBySkill);

                        instance = effectEvent;
                        if (instance != null)
                            frameData.pEvents.Add(instance);
                    }
                }
            }
        }

        var summonsLength = dataRes.SummonsLength;
        if (summonsLength > 0)
        {
            for (int i = 0; i < summonsLength; ++i)
            {
                var buffSum = dataRes.GetSummons(i);
                if (buffSum != null)
                {
                    BDEventBase instance = null;
                    int frame = buffSum.Startframe;
                    if (frame < vFramesData.Count)
                    {
                        var frameData = vFramesData[frame];
                        
                        List<int> type2List = new List<int>();
                        var type2Length = buffSum.PosType2Length;
                        for (int j = 0; j < type2Length; ++j)
                        {
                            type2List.Add(buffSum.GetPosType2(j));
                        }
                        var effectEvent = new BDDoSummon(buffSum.SummonID, buffSum.SummonLevel, buffSum.LevelGrowBySkill, buffSum.SummonNum, buffSum.PosType, type2List, buffSum.IsSameFace);

                        instance = effectEvent;
                        if (instance != null)
                            frameData.pEvents.Add(instance);
                    }
                }
            }
        }

        for (int i = 0; i < dataRes.MechanismsLength; ++i)
        {
            var data = dataRes.GetMechanisms(i);
            if (data != null)
            {
                BDEventBase instance = null;
                int frame = data.Startframe;
                if (frame < vFramesData.Count)
                {
                    var frameData = vFramesData[frame];
                    var duration = IntMath.Float2Int(data.Time * GlobalLogic.VALUE_1000);
                    if (data.PhaseDelete)
                        duration = -1;
                    var effectEvent = new BDAddMechanism(data.Id, duration, data.Level, data.LevelBySkill, data.PhaseDelete, data.FinishDeleteAll);

                    instance = effectEvent;
                    if (instance != null)
                        frameData.pEvents.Add(instance);
                }
            }
        }

        return true;
    }

	private static bool CheckSkillNeedLoad(List<string> skillCfgList, SkillFileName sf/*, bool isCommonSkill, string skillName*/)
    {

	//	bool find = false;
		//var list = skillCfgList;

		//for(int i=0; i<list.Count; ++i)
		{
			string filename = (string)sf.fullPath;

            //默认加载技能配置文件的时候只加载Pve的技能配置文件
			if (sf.isPvp)
				return false;

            if (sf.isChiji)
                return false;

			if (skillCfgList != null)
			{
				if (!sf.isCommon && sf.folderName.Length>0/* && !configList.Contains(list[i].lastName)*/)
				{
					bool find = false;
					for(int k=0; k<skillCfgList.Count; ++k)
					{
						if (string.Compare(sf.folderName, skillCfgList[k], true)==0)
						{
							find = true;
							break;
						}
					}
					if (!find)
						return false;
				}

			}
		}

		return true;

		/*
        if (skillCfgList == null || isCommonSkill || string.IsNullOrEmpty(skillName) == true)
        {
            //没有技能配置列表，或者是公用技能，或者技能的信息名为空，就默认都加载
            return true;
        }

        //技能在配置列表里就加载
        for (int k = 0; k < skillCfgList.Count; ++k)
        {
            if (string.Compare(skillName, skillCfgList[k], true) == 0)
            {
                return true;
            }
        }*/

        return false;
    }

#if !LOGIC_SERVER
    private static void ProcessNewLoadedSkillInfo(BDEntityActionInfo info, string skillFullPath,
        bool bPreLoadAction, bool collectAnimationNames,
        List<string> animationNames)
    {
        if (bPreLoadAction)
        {
            GameClient.PreloadManager.PreloadActionInfo(info,null,null);
        }

        if (collectAnimationNames && animationNames != null && !animationNames.Contains(info.actionName))
        {
            animationNames.Add(info.actionName);
        }
        BeActionFrameMgr.AddCached(skillFullPath, info);
    }
#else
    private static void ProcessNewLoadedSkillInfo(BDEntityActionInfo info, string skillFullPath,
       bool bPreLoadAction, bool collectAnimationNames,
       List<string> animationNames, BeActionFrameMgr frameMgr,SkillFileListCache fileCache)
    {
        if (bPreLoadAction)
        {
            GameClient.PreloadManager.PreloadActionInfo(info, frameMgr, fileCache);
        }

        if (collectAnimationNames && animationNames != null && !animationNames.Contains(info.actionName))
        {
            animationNames.Add(info.actionName);
        }
        frameMgr.AddCached(skillFullPath, info);
    }
#endif


    public static bool SaveLoad(BattleType battleType, string path, List<string> skillCfgList/*can be null*/,
            bool bPreLoadAction, bool collectAnimationNames, List<BDEntityActionInfo> loadedSkillList,
            List<string> animationNames
#if LOGIC_SERVER
        , BeActionFrameMgr frameMgr, SkillFileListCache fileCache, List<int> types = null)
#else
        , List<int> types = null)
#endif
    {
        try
        {
#if LOGIC_SERVER
            return Load(battleType, path, skillCfgList, bPreLoadAction, collectAnimationNames, loadedSkillList, animationNames, frameMgr, fileCache, null, types);
#else
            return Load(battleType, path, skillCfgList, bPreLoadAction, collectAnimationNames, loadedSkillList, animationNames, null, types);
#endif
        }
        catch (Exception e)
        {
            Logger.LogError("SkillTable Load Error" + e.Message + e.StackTrace);
        }

        return false;
    }


#if LOGIC_SERVER
    private static bool Load(BattleType battleType, string path, List<string> skillCfgList/*can be null*/,
    bool bPreLoadAction, bool collectAnimationNames, List<BDEntityActionInfo> loadedSkillList,
    List<string> animationNames, BeActionFrameMgr frameMgr, SkillFileListCache fileCache, BeEntity entity = null, List<int> types = null)
#else
    private static bool Load(BattleType battleType, string path, List<string> skillCfgList/*can be null*/,
    bool bPreLoadAction, bool collectAnimationNames, List<BDEntityActionInfo> loadedSkillList,
	List<string> animationNames, BeEntity entity = null,List<int> types = null)
#endif
    {
        //path = "Data/SkillData/Bullet_Object/Bullet_Gunman_gongji_zuolun";

        if (string.IsNullOrEmpty(path))
        {
            return false;
        }

        if (loadedSkillList != null)
        {
            loadedSkillList.Clear();
        }

		byte[] buffData = null;

#if !USE_FB && !SKILL_USE_FB
        UnityEngine.Object obj = null;
#else
        System.Text.StringBuilder builder = StringBuilderCache.Acquire();
        string folderName = Utility.GetPathLastName(path);
        //string fileListName = path + "/" + folderName + "_FileList";
        builder.AppendFormat("{0}/{1}_FileList_bin.bytes", path, folderName);
        string filebindName = builder.ToString();

#if SKILL_USE_FB
            filebindName = filebindName.Replace("SkillData", "SkillData_fb");
#endif

        //Console.Write(filepath + "\n");

#if LOGIC_SERVER
        string filepath = System.IO.Path.Combine(Utility.kRawDataPrefix, System.IO.Path.ChangeExtension(filebindName, Utility.kRawDataExtension)).ToLower();
		if(System.IO.File.Exists(filepath))
			buffData = System.IO.File.ReadAllBytes(filepath);
        //Logger.LogErrorFormat("skill load" + filepath + "\n");
#else
		UnityEngine.Object obj = AssetLoader.instance.LoadRes(filebindName).obj;
		StringBuilderCache.Release(builder);

		if (obj != null)
		{
			var binAsset = obj as TextAsset;

			if (binAsset == null)
			{
			Logger.LogErrorFormat("FlatBuffer Data is not TextAsset {0}", path);
			return false;
			}

			buffData = binAsset.bytes;
		}

#endif


#endif

#if LOGIC_SERVER || SKILL_USE_FB
        if (buffData != null)
        {
			FlatBuffers.ByteBuffer buffer = new FlatBuffers.ByteBuffer(buffData);

            if (!FBSkillData.FBSkillDataCollection.FBSkillDataCollectionBufferHasIdentifier(buffer))
            {
                Logger.LogErrorFormat("FlatBuffer Data formatIndentifier False {0}", path);
                return false;
            }

            var dataCollection = FBSkillData.FBSkillDataCollection.GetRootAsFBSkillDataCollection(buffer);

            var list = fileCache != null ? fileCache.GetCachedWithoutNew(path) : null;// SkillFileListCache.GetCachedWithoutNew(path);
            int len = dataCollection.CollectionLength;

            if (list == null)
            {
                list = new List<SkillFileName>();
                for (int i = 0; i < len; ++i)
                {
                    var current = dataCollection.GetCollection(i);
                    list.Add(new SkillFileName(current.Path, path/*, current.IsCommon, current.Type*/));
                }

                for(int i=0; i< list.Count; ++i)
                {
                    int index = BeUtility.FindPvP(list, i);
                    if (index > -1)
				    {
                        list[i].pvpPath = list[index].fullPath;
                        list[i].pvpIndexForFB = index;
                    }
                    
                    int chijiIndex = BeUtility.FindChiji(list, i);
                    if (chijiIndex > -1)
                    {
                        list[i].chijiPath = list[chijiIndex].fullPath;
                        list[i].chijiIndexForFB = chijiIndex;
                    }

                    list[i].indexForFB = i;
                }
                if (fileCache != null)
                    {
                        fileCache.AddCache(path, list);
                    }
                //SkillFileListCache.AddCache(path, list);
            }

            for (int i = 0; i < list.Count; ++i)
            {
                var current = list[i];
                if (types != null && list[i].weaponType != 0 && !types.Contains(list[i].weaponType))
                {
                    continue;
                }
                string filename = (string)current.fullPath;

				if (CheckSkillNeedLoad(skillCfgList, current/*current.isCommon, current.lastName*/))
                {
					//Logger.LogErrorFormat("before:{0}", current.fullPath);

                    string key = list[i].fullPath;
                    int index = list[i].indexForFB;
					if (BattleMain.IsModePvP(battleType) && list[i].pvpPath != null)
					{
                        key = list[i].pvpPath;
                        index = list[i].pvpIndexForFB;
                    }
                    else if(BattleMain.IsModeChiji(battleType) && list[i].chijiPath != null)
                    {
                        key = list[i].chijiPath;
                        index = list[i].chijiIndexForFB;
                    }

                    //Logger.LogErrorFormat("after:{0}", key);

                    BDEntityActionInfo info = null;
#if LOGIC_SERVER
                    if(frameMgr != null)
                        info = frameMgr.GetCached(key);
#else
                    info = BeActionFrameMgr.GetCached(key);
#endif

                    if (info == null)
                    {
                        info = new BDEntityActionInfo(key);
                        var data = dataCollection.GetCollection(index);

                        if (data == null)
                        {
                            continue;
                        }
                        var rawData = data.Data;
                        if (rawData == null)
                        {
                            continue;
                        }

                        if (!info.InitWithFlatBuffer(rawData))
                        {
                            Logger.LogErrorFormat("加载技能文件失败:{0}", key);
                            continue;
                        }
                        else
                        {
                            Logger.Log("load sill config " + filename + " succeed!!!! move name:" + rawData.MoveName);
                        }

                        info.weaponType = current.weaponType;
#if LOGIC_SERVER
                        ProcessNewLoadedSkillInfo(info, key, bPreLoadAction, collectAnimationNames, animationNames,frameMgr, fileCache);
#else
                        ProcessNewLoadedSkillInfo(info, key, bPreLoadAction, collectAnimationNames, animationNames);
#endif
                    }

                    if (loadedSkillList != null)
                    {
                        loadedSkillList.Add(info);
                    }
                }
            }
        }
        else
        {
            return false;
        }
#else
        {
            var list = SkillFileListCache.GetCached(path);
            if (list == null)
            {
                Logger.LogWarningFormat("can't find the filelist with path {0}", path);
                return false;
            }

            if(types != null)
            {
                list = SkillFileNameListFilter(list, types);
            }

            for (int i = 0; i < list.Count; ++i)
            {
                var current = list[i];
                string filename = (string)current.fullPath;

                if (CheckSkillNeedLoad(skillCfgList, current/*current.isCommon, current.lastName*/))
                {
                    string key = list[i].fullPath;
					if (BattleMain.IsModePvP(battleType) && list[i].pvpPath != null)
                    {
                        key = list[i].pvpPath;
                    }
                    else if (BattleMain.IsModeChiji(battleType) && list[i].chijiPath != null)
                    {
                        key = list[i].chijiPath;
                    }

                    BDEntityActionInfo info = BeActionFrameMgr.GetCached(key);

                    if (info == null)
                    {
                        UnityEngine.Object skillRes = null;

                        if (DebugSettings.instance.EnableDSkillDataCache)
                            skillRes = BeActionFrameMgr.GetSkillObjectCache(key);
                        else
                        {
#if TEST_SILLFILE_LOAD
                            info = GameClient.GameFrameWork.instance.GetGlobalSkillConfig(key);
                            //if (info != null)
                            //    Logger.LogErrorFormat("load from global skill config map:{0}", key);
#endif
                        }

                        if (info == null)
                        {
                            skillRes = AssetLoader.instance.LoadRes(key, typeof(DSkillData)).obj;
                            DSkillData data = skillRes as DSkillData;
                            info = new BDEntityActionInfo(key);
                            if (!info.InitWithDataRes(data))
                            {
                                Logger.LogErrorFormat("加载技能文件失败:{0}", key);
                                continue;
                            }
                            else
                            {
                                Logger.Log("load sill config " + filename + " succeed!!!! move name:" + data.moveName);
                            }

                            data = null;
                        }

                        info.weaponType = current.weaponType;

                        ProcessNewLoadedSkillInfo(info, key, bPreLoadAction, collectAnimationNames, animationNames);
                    }

                    if (loadedSkillList != null)
                    {
                        loadedSkillList.Add(info);
                    }
                }
            }
        }
#endif
                        return true;
    }

    private static List<SkillFileName> SkillFileNameListFilter(List<SkillFileName> fileList, List<int> weaponTypes)
    {
        List<SkillFileName> tempList = new List<SkillFileName>();
        if (weaponTypes != null)
        {
            if (fileList != null)
            {
                for (int i = 0; i < fileList.Count; ++i)
                {
                    if (fileList[i].weaponType == 0 || weaponTypes.Contains(fileList[i].weaponType))
                    {
                        tempList.Add(fileList[i]);
                    }
                }
            }
        }
        return tempList;
    }

    public bool InitWithDataRes(DSkillData dataRes)
    {
        if(dataRes == null)
        {
            return false;
        }

        moveName = dataRes.moveName;
        skillID  = dataRes.skillID;
        relatedAttackSpeed = dataRes.relatedAttackSpeed;
        attackSpeed = dataRes.attackSpeed;
        actionName = dataRes.animationName;
        iLogicFramesNum = dataRes.totalFrames;
        hitEffect = dataRes.goHitEffect;
        hitEffectAsset = dataRes.goHitEffectAsset;
        hitEffectInfoTableId = dataRes.hitEffectInfoTableId;
        //hitSFX = dataRes.goSFX;
		hitSFXID = dataRes.hitSFXID;
		if (hitSFXID > 0)
			hitSFXIDData = TableManager.GetInstance().GetTableItem<ProtoTable.SoundTable>(hitSFXID);
//       iActionType = dataRes.hurtType;
        //fActionForcex = dataRes.forcex;
        //fActionForcey = dataRes.forcey;
        animationspeed = dataRes.animationSpeed;

        if (dataRes.hurtTime > 0)
            hurtTime = dataRes.hurtTime;
        // if (dataRes.hurtPause == 1)
        //     hurtPause = true;
        //hurtPauseTime = dataRes.hurtPauseTime;
        if (dataRes.skilltime > 0)
            skillTotalTime = IntMath.Float2Int(dataRes.skilltime,1000);
        //skillPriority = dataRes.skillPriority;
        fLogicFrameDeltaTime = new VFactor(1,dataRes.fps); //0.016f;//1.0f / 60.0f; //0.019f;//hard code 60fps
        // TODO *1000 convert to int 
        fRealFramesTime = (iLogicFramesNum - 1) * 1000 / dataRes.fps;
        //fFramesSpeed = 1.0f;

        if (dataRes.isLoop != 0)
            bLoop = true;

        if(dataRes.loopAnimation)
        {
            //bAnimLoop = dataRes.loopAnimation;
        }

        comboStartFrame = dataRes.comboStartFrame;
        comboSkillID = dataRes.comboSkillID;

        if (dataRes.skillPhases.Length > 0)
        {
            skillPhases = new int[dataRes.skillPhases.Length];

            dataRes.skillPhases.CopyTo(skillPhases, 0);
        }

        this.triggerType = dataRes.triggerType;

        useSpellBar = dataRes.useSpellBar;
		if (useSpellBar)
		{
			spellBarTime = dataRes.spellBarTime;
		}
			
        useCharge = dataRes.isCharge;
        if (useCharge)
        {
            this.chargeConfig = dataRes.chargeConfig.DoCopy();
        }

        useSpecialOperation = dataRes.isSpeicalOperate;
        if (useSpecialOperation)
        {
            this.operationConfig = dataRes.operationConfig.DoCopy();
        }
        useSelectSeatJoystick = dataRes.isUseSelectSeatJoystick;

        if (dataRes.skillJoystickConfig.mode != SkillJoystickMode.NONE)
        {

            skillJoystickConfig = dataRes.skillJoystickConfig.DoCopy();
        }
        
        if (dataRes.actionSelect.Length > 0)
        {
            actionSelectPhase = dataRes.actionSelectPhase;
            actionSelect = new int[dataRes.actionSelect.Length];
            for (int i = 0; i < dataRes.actionSelect.Length; i++)
            {
                actionSelect[i] = dataRes.actionSelect[i];
            }
        }
        if (dataRes.actionIconPath.Length > 0)
        {
            actionIconPath = new string[dataRes.actionIconPath.Length];
            for (int i = 0; i < dataRes.actionIconPath.Length; i++)
            {
                actionIconPath[i] = dataRes.actionIconPath[i];
            }
        }

        skillEvents = dataRes.skillEvents;


		cameraRestore = dataRes.cameraRestore;
		cameraRestoreTime = dataRes.cameraRestoreTime;

        if (dataRes.grabNum >= 1)
        {
            this.grabData = new BDGrabData();
            //抓取信息
            grabData.endForcex = dataRes.grabEndForceX;
            grabData.endForcey = dataRes.grabEndForceY;
            grabData.hitSpreadOut = dataRes.hitSpreadOut;
            grabData.posx = dataRes.grabPosx;
            grabData.posy = dataRes.grabPosy;
            grabData.grabNum = dataRes.grabNum;
            grabData.grabMoveSpeed = dataRes.grabMoveSpeed;
            if (dataRes.grabTime > 0)
                grabData.duraction = IntMath.Float2Int(dataRes.grabTime * GlobalLogic.VALUE_1000);

            grabData.endForceType = dataRes.grabEndEffectType;
            if (dataRes.grabAction != 0)
            {
                grabData.action = dataRes.grabAction;
            }
            grabData.quickPressDismis = dataRes.grabSupportQuickPressDismis;
            grabData.notGrabBati = dataRes.notGrabBati;
            grabData.notGrabGeDang = dataRes.notGrabGeDang;
            grabData.notUseGrabSetPos = dataRes.notUseGrabSetPos;
            grabData.notGrabToBlock = dataRes.notGrabToBlock;
            grabData.buffInfoId = dataRes.buffInfoId;
            grabData.buffInfoIdToSelf = dataRes.buffInfoIdToSelf;
            grabData.buffInfoIDToOther = dataRes.buffInfoIDToOther;
        }


        
        //框
        for (int iFrame = 0; iFrame < iLogicFramesNum; ++iFrame)
        {
            BDEntityActionFrameData fdata = new BDEntityActionFrameData();

            //攻击框
            fdata.pAttackData = new BDDBoxData();
            HurtDecisionBox hurtBox = dataRes.HurtBlocks[iFrame];
            if(hurtBox != null && hurtBox.boxs.Length > 0)
            {
                fdata.pAttackData.zDim = hurtBox.zDim;
                fdata.pAttackData.hurtID = hurtBox.hurtID;

                
                if (fdata.pAttackData.hurtID > 0)
                {
                    //设置攻击框的类型
                    var hurtData = TableManager.GetInstance().GetTableItem<ProtoTable.EffectTable>(fdata.pAttackData.hurtID);
                    if (hurtData != null)
                    {
                        fdata.pAttackData.hurtType = (int)hurtData.EffectTargetType;
                    }
                    else
                    {
                        Logger.LogErrorFormat("技能配置文件{0} id: {1} 的攻击框没有{2}的触发效果ID", dataRes.name, dataRes.skillID, fdata.pAttackData.hurtID);
                    }
                }
                
                

                for (int iHurt = 0; iHurt < hurtBox.boxs.Length; ++iHurt)
                {
                    ShapeBox shapeBox = hurtBox.boxs[iHurt];
                    shapeBox.size.x = Mathf.Abs(shapeBox.size.x);
                    shapeBox.size.y = Mathf.Abs(shapeBox.size.y);
                    DBoxImp boximp = new DBoxImp();
                    boximp.vBox._min.x = VInt.Float2VIntValue(shapeBox.center.x - shapeBox.size.x / 2.0f);
                    boximp.vBox._min.y = VInt.Float2VIntValue(shapeBox.center.y - shapeBox.size.y / 2.0f);
                    boximp.vBox._max.x = VInt.Float2VIntValue(shapeBox.center.x + shapeBox.size.x / 2.0f);
                    boximp.vBox._max.y = VInt.Float2VIntValue(shapeBox.center.y + shapeBox.size.y / 2.0f);

                    fdata.pAttackData.vBox.Add(boximp);
                }
            }

            //防御框
            fdata.pDefenseData = new BDDBoxData();
            DefenceDecisionBox defBox = dataRes.DefenceBlocks[iFrame];
            if (defBox != null && defBox.boxs.Length > 0)
            {
                for(int i=0; i<defBox.boxs.Length; ++i)
                {
                    ShapeBox shapeBox = defBox.boxs[i];
                    shapeBox.size.x = Mathf.Abs(shapeBox.size.x);
                    shapeBox.size.y = Mathf.Abs(shapeBox.size.y);
                    DBoxImp box = new DBoxImp();
                    box.vBox._min.x = VInt.Float2VIntValue(shapeBox.center.x - shapeBox.size.x / 2.0f);
                    box.vBox._min.y = VInt.Float2VIntValue(shapeBox.center.y - shapeBox.size.y / 2.0f);
                    box.vBox._max.x = VInt.Float2VIntValue(shapeBox.center.x + shapeBox.size.x / 2.0f);
                    box.vBox._max.y = VInt.Float2VIntValue(shapeBox.center.y + shapeBox.size.y / 2.0f);

                    fdata.pDefenseData.vBox.Add(box);
                }
            }

            vFramesData.Add(fdata);
        }


#if !LOGIC_SERVER
        //插入事件帧
        //特效
        if (dataRes.effectFrames != null && dataRes.effectFrames.Length > 0)
        {
            for (int i = 0; i < dataRes.effectFrames.Length; ++i)
            {
                EffectsFrames effectData = dataRes.effectFrames[i];
                BDPlayEffect effectEvent = new BDPlayEffect(effectData);
                int frame = effectData.startFrames;

                if(frame >= 0 && frame < vFramesData.Count)
                {
                    BDEntityActionFrameData frameData = vFramesData[frame];
                    if (frameData != null)
                    {
                        frameData.pEvents.Add(effectEvent);
                    }
                }
                
            }
        }
#endif

        //entity
        if (dataRes.entityFrames != null && dataRes.entityFrames.Length > 0)
        {
            for (int i = 0; i < dataRes.entityFrames.Length; ++i)  {
                EntityFrames entityData = dataRes.entityFrames[i];

                BDGenProjectile entityEvent = new BDGenProjectile(entityData);

                int frame = entityData.startFrames;
                if(frame >= vFramesData.Count)
                {
                    continue;
                }
                BDEntityActionFrameData frameData = vFramesData[frame];
                if (frameData != null)
                {
                    frameData.pEvents.Add(entityEvent);
                }
            }
        }

        //帧事件
        if (dataRes.frameTags != null && dataRes.frameTags.Length > 0)
        {
            //foreach(var frameEvent in dataRes.frameTags)
			for(int i=0; i<dataRes.frameTags.Length; ++i)
            {
				var frameEvent = dataRes.frameTags[i];
                int frame = frameEvent.startframe;
                if (frame < 0 || frame >= vFramesData.Count)
                {
                    //Logger.LogErrorFormat("FrameTag {0}: startFrame is out of array, if you split the animation to two animation, please check two animation's frame tag config", dataRes.animationName);
                    continue;
                }

                BDEntityActionFrameData frameData = vFramesData[frame];
                int tag = (int)frameEvent.tag;
                if (BeUtility.CheckHaveTag(tag, (int)DSFFrameTags.TAG_NEWDAMAGE))
                {
                    frameData.kFlag.SetFlag((int)DSFFrameTags.TAG_NEWDAMAGE);
                }
                else if (BeUtility.CheckHaveTag(tag, (int)DSFFrameTags.TAG_LOCKZSPEED))
                {
                    frameData.kFlag.SetFlag((int)DSFFrameTags.TAG_LOCKZSPEED);
                }
                else if (BeUtility.CheckHaveTag(tag, (int)DSFFrameTags.TAG_LOCKZSPEEDFREE))
                {
                    frameData.kFlag.SetFlag((int)DSFFrameTags.TAG_LOCKZSPEEDFREE);
                }
				else if (BeUtility.CheckHaveTag(tag, (int)DSFFrameTags.TAG_IGNORE_GRAVITY))
				{
					frameData.kFlag.SetFlag((int)DSFFrameTags.TAG_IGNORE_GRAVITY);
				}
				else if (BeUtility.CheckHaveTag(tag, (int)DSFFrameTags.TAG_RESTORE_GRAVITY))
				{
					frameData.kFlag.SetFlag((int)DSFFrameTags.TAG_RESTORE_GRAVITY);
				}
				else if (BeUtility.CheckHaveTag(tag, (int)DSFFrameTags.TAG_SET_TARGET_POS_XY))
				{
					frameData.kFlag.SetFlag((int)DSFFrameTags.TAG_SET_TARGET_POS_XY);
                }
                else if (BeUtility.CheckHaveTag(tag, (int)DSFFrameTags.TAG_CURFRAME))
                {
                    frameData.kFlag.SetFlag((int)DSFFrameTags.TAG_CURFRAME, frameEvent.tagFlag);
                }
                else if (BeUtility.CheckHaveTag(tag, (int)DSFFrameTags.TAG_CHANGEFACE))
                {
                    frameData.kFlag.SetFlag((int)DSFFrameTags.TAG_CHANGEFACE);
                }
                else if (BeUtility.CheckHaveTag(tag, (int)DSFFrameTags.TAG_CHANGEFACEBYDIR))
                {
                    frameData.kFlag.SetFlag((int)DSFFrameTags.TAG_CHANGEFACEBYDIR);
                }
                else if (BeUtility.CheckHaveTag(tag, (int)DSFFrameTags.TAG_REMOVEEFFECT))
                {
                    frameData.kFlag.SetFlag((int)DSFFrameTags.TAG_REMOVEEFFECT, frameEvent.tagFlag);
                }
                else if (BeUtility.CheckHaveTag(tag, (int)DSFFrameTags.TAG_STARTCHECKHIT))
                {
                    frameData.kFlag.SetFlag((int)DSFFrameTags.TAG_STARTCHECKHIT);
                }
                else if (BeUtility.CheckHaveTag(tag, (int)DSFFrameTags.TAG_STARTDEALSKIPPHASE))
                {
                    frameData.kFlag.SetFlag((int)DSFFrameTags.TAG_STARTDEALSKIPPHASE);
                }

                else if (BeUtility.CheckHaveTag(tag, (int)DSFFrameTags.TAG_NAME_HIDE))
                {
                    frameData.kFlag.SetFlag((int)DSFFrameTags.TAG_NAME_HIDE);
                }
                else if (BeUtility.CheckHaveTag(tag, (int)DSFFrameTags.TAG_NAME_SHOW))
                {
                    frameData.kFlag.SetFlag((int)DSFFrameTags.TAG_NAME_SHOW);
                }
                else if (BeUtility.CheckHaveTag(tag, (int)DSFFrameTags.TAG_SHADOW_HIDE))
                {
                    frameData.kFlag.SetFlag((int)DSFFrameTags.TAG_SHADOW_HIDE);
                }
                else if (BeUtility.CheckHaveTag(tag, (int)DSFFrameTags.TAG_SHADOW_SHOW))
                {
                    frameData.kFlag.SetFlag((int)DSFFrameTags.TAG_SHADOW_SHOW);
                }
                else if (BeUtility.CheckHaveTag(tag, (int)DSFFrameTags.TAG_HPBAR_HIDE))
                {
                    frameData.kFlag.SetFlag((int)DSFFrameTags.TAG_HPBAR_HIDE);
                }
                else if (BeUtility.CheckHaveTag(tag, (int)DSFFrameTags.TAG_HPBAR_SHOW))
                {
                    frameData.kFlag.SetFlag((int)DSFFrameTags.TAG_HPBAR_SHOW);
                }
                else if (BeUtility.CheckHaveTag(tag, (int)DSFFrameTags.TAG_LOOKAT_TARGET))
                {
                    frameData.kFlag.SetFlag((int)DSFFrameTags.TAG_LOOKAT_TARGET);
                }
                else if (BeUtility.CheckHaveTag(tag, (int)DSFFrameTags.TAG_REMOVE_BUFF))
                {
                    frameData.kFlag.SetFlag((int)DSFFrameTags.TAG_REMOVE_BUFF, frameEvent.tagFlag);
                }
                else if (BeUtility.CheckHaveTag(tag, (int)DSFFrameTags.TAG_REMOVE_MECHANISM))
                {
                    frameData.kFlag.SetFlag((int)DSFFrameTags.TAG_REMOVE_MECHANISM, frameEvent.tagFlag);
                }
                else if (BeUtility.CheckHaveTag(tag, (int)DSFFrameTags.TAG_OPEN_2ND_STATE))
                {
                    frameData.kFlag.SetFlag((int)DSFFrameTags.TAG_OPEN_2ND_STATE);
                }
                else if (BeUtility.CheckHaveTag(tag, (int)DSFFrameTags.TAG_CLOSE_2ND_STATE))
                {
                    frameData.kFlag.SetFlag((int)DSFFrameTags.TAG_CLOSE_2ND_STATE);
                }
            }
        }

        //抓取
        if (dataRes.frameGrap != null && dataRes.frameGrap.Length > 0)
        {
            
			for(int k=0; k<dataRes.frameGrap.Length; ++k)
            //foreach (DSkillFrameGrap frameEvent in dataRes.frameGrap)
            {
				var frameEvent = dataRes.frameGrap[k];
                BDEventBase instance = null;
                int frame = frameEvent.startframe;
                if (frame < vFramesData.Count)
                {
                    BDEntityActionFrameData frameData = vFramesData[frame];
                    if (frameEvent.op == DSFGrapOp.GRAP_INTERRUPT ||
                        frameEvent.op == DSFGrapOp.GRAP_CHANGE_TARGETPOS ||
                        frameEvent.op == DSFGrapOp.GRAP_STOPCHANGE_TARGETPOS ||
                        frameEvent.op == DSFGrapOp.GRAP_CHANGE_TARGETACTION ||
                        frameEvent.op == DSFGrapOp.GRAP_CHANGE_TARGETROTATION)
                    {
                        BDSkillSuspend suspend = new BDSkillSuspend((int)frameEvent.op,new VInt3(frameEvent.targetPos), frameEvent.faceGraber, (int)frameEvent.targetAction, frameEvent.targetAngle);
                        instance = suspend;
                    }
//                     else if (frameEvent.op == DSFGrapOp.GRAP_RELEASE)
//                     {
//                         exFlag.SetFlag((int)DSFEntityStateTag.GRAPRELEASE);
//                     }
                    else if (frameEvent.op == DSFGrapOp.GRAP_JUDGE || 
                            frameEvent.op == DSFGrapOp.GRAP_JUDGE_EXECUTE ||
                            frameEvent.op == DSFGrapOp.GRAP_JUDGE_SKIP_PHASE
                        )
                    {
                        for(int i=0; i<frameEvent.length; ++i)
                        {
                            if ((frame + i) < vFramesData.Count)
                            {
                                BDEntityActionFrameData tmpFrameData = vFramesData[frame + i];

                                tmpFrameData.kFlag.SetFlag((int)frameEvent.op);
                            }
                        }
                            
                    }
                    else
                    {
                        frameData.kFlag.SetFlag((int)frameEvent.op);
                    }

                    if (instance != null)
                    {
                        frameData.pEvents.Add(instance);
                    }
                }
            }
        }

        //状态操作
        if (dataRes.stateop != null && dataRes.stateop.Length > 0)
        {
			for(int i=0; i<dataRes.stateop.Length; ++i)
            //foreach(DSkillFrameStateOp frameEvent in dataRes.stateop)
            {
				var frameEvent = dataRes.stateop[i];
                BDEventBase instance = null;

                int frame = frameEvent.startframe;
                if (frame < vFramesData.Count)
                {
                    BDEntityActionFrameData frameData = vFramesData[frame];
                    BDStateStackOP statckOP = new BDStateStackOP(
                        (int)frameEvent.op, 
                        (int)frameEvent.state, 
                        frameEvent.idata1,
                        frameEvent.idata2, 
                        frameEvent.fdata1, 
                        (int)frameEvent.statetag);
                    instance = statckOP;

                    if (instance != null)
                    {
                        frameData.pEvents.Add(instance);
                    }
                }
            }
        }

        //属性修改
        if (dataRes.properModify != null && dataRes.properModify.Length > 0)
        {
            //foreach(DSkillPropertyModify frameEvent in dataRes.properModify)
			for(int i=0; i<dataRes.properModify.Length; ++i)
            {
				var frameEvent = dataRes.properModify[i];
                BDEventBase instance = null;

                int frame = frameEvent.startframe;
                for (int j = 0; j < frameEvent.length && frame + j < vFramesData.Count; j++)
                {
                    BDEntityActionFrameData frameData = vFramesData[frame + j];
                    BDModifySpeed speed = new BDModifySpeed(
                        frameEvent.tag,
                        frameEvent.modifyfliter,
                        frameEvent.value,
                        frameEvent.movedValue,
                        frameEvent.jumpToTargetPos,
                        frameEvent.joyStickControl,
                        frameEvent.valueAcc,
                        frameEvent.movedValueAcc,
                        frameEvent.modifyXBackward,
                        frameEvent.eachFrameModify,
                        frameEvent.useMovedYValue,
                        frameEvent.movedYValue,
                        frameEvent.movedYValueAcc
                    );
                    instance = speed;

                    if (instance != null)
                        frameData.pEvents.Add(instance);
                }
            }
        }

#if !LOGIC_SERVER
        //camera效果
        if (dataRes.shocks != null && dataRes.shocks.Length > 0)
        {
           	//foreach (DSkillFrameEventSceneShock frameEvent in dataRes.shocks)
			for(int i=0; i<dataRes.shocks.Length; ++i)
            {
				var frameEvent = dataRes.shocks[i];
                BDEventBase instance = null;

                int frame = frameEvent.startframe;
                if (frame < vFramesData.Count)
                {
                    BDEntityActionFrameData frameData = vFramesData[frame];
                    BDSceneShock shock = null;
                    if (frameEvent.isNew)
                    {
                        shock = new BDSceneShock(frameEvent.time, frameEvent.num, frameEvent.xrange, frameEvent.yrange, frameEvent.decelerate, frameEvent.xreduce, frameEvent.yreduce, frameEvent.mode, frameEvent.radius);
                    }
                    else
                    {
                        shock = new BDSceneShock(frameEvent.time, frameEvent.speed, frameEvent.xrange, frameEvent.yrange);
                    }

                    instance = shock;

                    if (instance != null)
                        frameData.pEvents.Add(instance);
                }
            }
        }
#endif

        //移动控制
        if (dataRes.walkControl != null && dataRes.walkControl.Length > 0)
        {
            //foreach(DSkillWalkControl frameEvent in dataRes.walkControl)
			for(int i=0; i<dataRes.walkControl.Length; ++i)
            {
				var frameEvent = dataRes.walkControl[i];
                BDEventBase instance = null;
                int frame = frameEvent.startframe;
                if (frame < vFramesData.Count)
                {
                    BDEntityActionFrameData frameData = vFramesData[frame];
                    BDSkillWalkControl walk = new BDSkillWalkControl(frameEvent.walkMode, frameEvent.walkSpeedPercent, frameEvent.useSkillSpeed, frameEvent.walkSpeedPercent2);

                    instance = walk;
                    if (instance != null)
                        frameData.pEvents.Add(instance);


/*                    if (frameEvent.walkMode == SkillWalkMode.FACEDIR || frameEvent.walkMode == SkillWalkMode.FREE)
                    {
                        BDSkillWalkControl walkStop = new BDSkillWalkControl(SkillWalkMode.FORBID, 0);
                        frameData = vFramesData[frame+frameEvent.length-1];
                        frameData.pEvents.Add(walkStop);
                    }*/
                }
            }
        }

        //camera move
        if (dataRes.cameraMoves != null && dataRes.cameraMoves.Length > 0)
        {
			for(int i=0; i<dataRes.cameraMoves.Length; ++i)
            //foreach(DSkillCameraMove frameEvent in dataRes.cameraMoves)
            {
				var frameEvent = dataRes.cameraMoves[i];
                BDEventBase instance = null;
                int frame = frameEvent.startframe;
                if (frame >= 0 && frame < vFramesData.Count) 
                {
                    BDEntityActionFrameData frameData = vFramesData[frame];
                    BDSkillCameraMove move = new BDSkillCameraMove(frameEvent.offset, frameEvent.duraction);
                    instance = move;
                    if (instance != null)
                        frameData.pEvents.Add(instance);
                }
            }
        }

#if !LOGIC_SERVER
        //sfx播放
        if (dataRes.sfx != null && dataRes.sfx.Length > 0)
        {
            //foreach(var frameEvent in dataRes.sfx)
			for(int i=0; i<dataRes.sfx.Length; ++i)
            {
				var frameEvent = dataRes.sfx[i];
                BDEventBase instance = null;
                int frame = frameEvent.startframe;
                if (frame < vFramesData.Count)
                {
                    var frameData = vFramesData[frame];
					var sfxEvent = new BDSkillSfx(frameEvent.soundClipAsset, frameEvent.soundID, frameEvent.loop, frameEvent.useActorSpeed, frameEvent.phaseFinishDelete, frameEvent.finishDelete, frameEvent.volume);

                    instance = sfxEvent;

                    if (instance != null)
                        frameData.pEvents.Add(instance);
                }
            }
        }
#endif

        //帧事件
        if (dataRes.frameEffects != null && dataRes.frameEffects.Length > 0)
        {
            //foreach(var frameEvent in dataRes.frameEffects)
			for(int i=0; i<dataRes.frameEffects.Length; ++i)
            {
				var frameEvent = dataRes.frameEffects[i];
                BDEventBase instance = null;
                int frame = frameEvent.startframe;
                if (frame < vFramesData.Count)
                {
                    var frameData = vFramesData[frame];
                    var duration = IntMath.Float2Int(frameEvent.buffTime * GlobalLogic.VALUE_1000);
                    if (frameEvent.phaseDelete)
                        duration = -1;
                    var effectEvent = new BDSkillFrameEffect(frameEvent.effectID, duration, frameEvent.useBuffAni, frameEvent.usePause, frameEvent.pauseTime, frameEvent.finishDelete,frameEvent.finishDeleteAll, frameEvent.mechanismId);

                    instance = effectEvent;

                    if (instance != null)
                        frameData.pEvents.Add(instance);
                }
            }
        }

		if (dataRes.actions != null && dataRes.actions.Length > 0)
		{
			for(int i=0; i<dataRes.actions.Length; ++i)
			{
				DActionData frameEvent = dataRes.actions[i];

				BDSkillAction instance = null;
				int frame = frameEvent.startframe;
				if (frame < vFramesData.Count)
				{
					var frameData = vFramesData[frame];
					var action = new BDSkillAction(frameEvent.actionType, frameEvent.duration, frameEvent.deltaScale, new VInt3(frameEvent.deltaPos), frameEvent.ignoreBlock);
					instance = action;
					if (instance != null)
						frameData.pEvents.Add(instance);
				}
			}
		}

#if !LOGIC_SERVER
        //挂件
        if(dataRes.attachFrames != null)
        {
            for(int i = 0; i < dataRes.attachFrames.Length; ++i)
            {
                EntityAttachFrames attf = dataRes.attachFrames[i];

                if( attf != null )
                {
                    BeAttachFrames batf = new BeAttachFrames();
                    batf.name = attf.name;
                    batf.resID = attf.resID;
                    batf.start = attf.start;
                    batf.end = attf.end;
                    batf.entityPrefab = attf.entityPrefab;
                    batf.entityAsset = attf.entityAsset;
                    batf.attachName = attf.attachName;

                    List<BeAnimationFrame> anims = new List<BeAnimationFrame>();

                    for(int j = 0; j < attf.animations.Length; ++ j)
                    {
                        BeAnimationFrame temp = new BeAnimationFrame();
                        temp.anim = attf.animations[j].anim;
                        temp.start = attf.animations[j].start;
                        temp.speed = attf.animations[j].speed;
                        anims.Add(temp);
                    }

                    batf.animations = anims.ToArray();
                    vAttachFrames.Add(batf);
                }
            }
        }
#endif
      
        if (dataRes.buffs != null && dataRes.buffs.Length > 0)
        {
            //foreach(var frameEvent in dataRes.frameEffects)
            for (int i = 0; i < dataRes.buffs.Length; ++i)
            {
                var frameEvent = dataRes.buffs[i];
                BDEventBase instance = null;
                int frame = frameEvent.startframe;
                if (frame < vFramesData.Count)
                {
                    var frameData = vFramesData[frame];
                    var duration = IntMath.Float2Int(frameEvent.buffTime * GlobalLogic.VALUE_1000);
                    if (frameEvent.phaseDelete || frameEvent.finishDeleteAll) 
                        duration = -1;
                    var effectEvent = new BDAddBuffInfoOrBuff(frameEvent.buffID, new List<int>(frameEvent.buffInfoList), duration, frameEvent.phaseDelete, frameEvent.finishDeleteAll, frameEvent.level, frameEvent.levelBySkill);

                    instance = effectEvent;

                    if (instance != null)
                        frameData.pEvents.Add(instance);
                }
            }
        }
        if (dataRes.summons != null && dataRes.summons.Length > 0)
        {
            //foreach(var frameEvent in dataRes.frameEffects)
            for (int i = 0; i < dataRes.summons.Length; ++i)
            {
                var frameEvent = dataRes.summons[i];
                BDEventBase instance = null;
                int frame = frameEvent.startframe;
                if (frame < vFramesData.Count)
                {
                    var frameData = vFramesData[frame];
                    var effectEvent = new BDDoSummon(frameEvent.summonID, frameEvent.summonLevel,frameEvent.levelGrowBySkill, frameEvent.summonNum, frameEvent.posType, new List<int>(frameEvent.posType2), frameEvent.isSameFace);

                    instance = effectEvent;

                    if (instance != null)
                        frameData.pEvents.Add(instance);
                }
            }
        }
        if (dataRes.mechanisms != null && dataRes.mechanisms.Length > 0)
        {
            for (int i = 0; i < dataRes.mechanisms.Length; ++i)
            {
                var frameEvent = dataRes.mechanisms[i];
                BDEventBase instance = null;
                int frame = frameEvent.startframe;
                if (frame < vFramesData.Count)
                {
                    var frameData = vFramesData[frame];
                    var duration = IntMath.Float2Int(frameEvent.time * GlobalLogic.VALUE_1000);
                    if (frameEvent.phaseDelete)
                        duration = -1;
                    var effectEvent = new BDAddMechanism(frameEvent.id, duration, frameEvent.level, frameEvent.levelBySkill, frameEvent.phaseDelete, frameEvent.finishDeleteAll);

                    instance = effectEvent;

                    if (instance != null)
                        frameData.pEvents.Add(instance);
                }
            }
        }
        return true;
    }
}

public class BDEntityRes
{
    public Dictionary<string, BDEntityActionInfo> _vkActionsMap = new Dictionary<string, BDEntityActionInfo>();
    public Dictionary<int, string> skillData = new Dictionary<int, string>();
    public TagActionInfo[] tagActionInfo = new TagActionInfo[10];
    protected string currentActionName = "";

    public void SetActionName(string s)
    {
        currentActionName = s;
    }

    public int GetSkillIDByActionName(string name)
    {
        if (string.IsNullOrEmpty(name))
            return 0;
        foreach(var item in skillData)
        {
            if (name.Equals(item.Value))
                return item.Key;
        }

        return 0;
    }
    
    public string GetCurrentActionName()
    {
        return currentActionName;
    }

    public bool HasAction(string key)
    {
        if (key == null)
            return false;
        return _vkActionsMap.ContainsKey(key);
    }

    public void Reset()
    {
        skillData.Clear();
        _vkActionsMap.Clear();
    }
    public class TagActionInfo
    {
        public Dictionary<string, BDEntityActionInfo> actionMap = new Dictionary<string, BDEntityActionInfo>();
        //key weaponType
        public Dictionary<int, List<string>> weaponTypeSkillDataInfo = new Dictionary<int, List<string>>();
        public void AddActionInfo(BDEntityActionInfo info)
        {
            if (!weaponTypeSkillDataInfo.ContainsKey(info.weaponType))
                weaponTypeSkillDataInfo.Add(info.weaponType, new List<string>());

            weaponTypeSkillDataInfo[info.weaponType].Add(info.moveName);

            if (!actionMap.ContainsKey(info.moveName))
                actionMap.Add(info.moveName, info);
        }

        public BDEntityActionInfo GetActionInfo(string res)
        {
            return actionMap[res];
        }

        public bool HasWeaponType(int weaponType)
        {
            return weaponTypeSkillDataInfo.ContainsKey(weaponType);
        }
    }

    public void AddActionInfo(BDEntityActionInfo info, string path)
    {
        if (info.weaponTag == 0)
        {
            if (!_vkActionsMap.ContainsKey(info.moveName))
                _vkActionsMap.Add(info.moveName, info);
            else
                Logger.LogWarningFormat("已经有技能配置文件名字为:{0} file:{1}", info.moveName, path);

            if (info.skillID != 0)
            {
                if (!skillData.ContainsKey(info.skillID))
                    skillData.Add(info.skillID, info.moveName);
                else
                    Logger.LogWarningFormat("已经有skill id：{0} file:{1}", info.skillID, path);
            }

            return;
        }

        if (!HasTagActionInfo(info.weaponTag))
        {
            tagActionInfo[info.weaponTag] = new TagActionInfo();
        }

        tagActionInfo[info.weaponTag].AddActionInfo(info);
    }

    public void SetWeaponInfo(int tag, int type=0)
    {
        Logger.LogProcessFormat("SetWeaponInfo tag:{0} type:{1}", tag, type);

        if (!HasTagActionInfo(tag))
            return;

        var commonLst = tagActionInfo[tag].weaponTypeSkillDataInfo[0];
        if (commonLst != null)
        {
            ReplaceActionInfo(commonLst, tagActionInfo[tag]);
        }

        if (type != 0 && tagActionInfo[tag].HasWeaponType(type))
        {
            ReplaceActionInfo(tagActionInfo[tag].weaponTypeSkillDataInfo[type], tagActionInfo[tag]);
        }
    }

    public void ReplaceActionInfo(List<string> lst, TagActionInfo tagInfo)
    {
        for(int i=0; i<lst.Count; ++i)
        {
            var moveName = lst[i];
            var action = tagInfo.GetActionInfo(moveName);

            if (!_vkActionsMap.ContainsKey(moveName))
            {
                //Logger.LogErrorFormat("[WEAPON]ReplaceActionInfo add:{0} skillid:{1}", moveName, action.skillID);
                _vkActionsMap.Add(moveName, action);
            }
            else
            {
                //Logger.LogErrorFormat("[WEAPON]ReplaceActionInfo replace:{0} skillid:{1}", moveName, action.skillID);
                _vkActionsMap[moveName] = action;
            }

            if (!skillData.ContainsKey(action.skillID))
                skillData.Add(action.skillID, moveName);
            else
                skillData[action.skillID] = moveName;
        }
    }

    bool HasTagActionInfo(int tag)
    {
        return tagActionInfo[tag] != null;
    }


}
