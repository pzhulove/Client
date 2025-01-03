using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.Runtime.InteropServices;
using System.ComponentModel;

[AttributeUsage(AttributeTargets.Class)]
public class DSFFrameEventTypeAttribute : Attribute
{
    public DSFFrameEventTypeAttribute(string name)
    {
        this._name = name;
    }

    protected string _name;

    public string name
    {
        get { return _name; }
    }
}

#if UNITY_EDITOR 
public class DSFFrameEventTypes
{
    public struct TypeEnum
    {
        public System.Type type;
        public DSFFrameEventTypeAttribute attribute;
    }

    public static TypeEnum[] types  = null;
    public static string[] showList = null;
     
    public static void Check()
    {
        if(types == null)
        {
            List<TypeEnum> datas = new List<TypeEnum>();
            List<string> shows = new List<string>();

            Assembly asmScripts = Assembly.GetAssembly(typeof(DSkillFrameEvent));
            if (asmScripts != null)
            {
                System.Type[] typs = asmScripts.GetTypes();

                for (int i = 0; i < typs.Length; ++i)
                {
                    System.Object[] cas = typs[i].GetCustomAttributes(typeof(DSFFrameEventTypeAttribute), false);
                    if (cas.Length > 0)
                    {
                        DSFFrameEventTypeAttribute feta = cas[0] as DSFFrameEventTypeAttribute;
                        TypeEnum t = new TypeEnum();
                        t.type = typs[i];
                        t.attribute = feta;
                        datas.Add(t);
                        shows.Add(feta.name);
                    }

                }
            }

            types = datas.ToArray();
            showList = shows.ToArray();
        }
    }  
}
#endif

[System.Serializable]
public class DSkillFrameEvent
{
    public string name;
    public int startframe;
    public int length = 1;


    public virtual void copyFrameEvent(DSkillFrameEvent src)
    {
        this.name = src.name;
        this.startframe = src.startframe;
        this.length = src.length;
    }
    //add
    /*
    1 grap
    2 state stack
    3 frame tag
    */
//     public int frameType;//frame类型
//     //抓取
//     public int grapOp;
// 
//     //状态栈
//     public int                  op;
//     public int                  state;
//     public int                  idata1;
//     public int                  idata2;
//     public int                  fdata1;
//     public int                  fdata2;
//     public int                  statetag;
// 
//     //帧tag
//     public int                  frameTag;
}

public enum DSkillPropertyModifyType
{
    [Description("X轴速度")]
    SPEED_X = 1,

    [Description("Y轴速度")]
    SPEED_Y,

    [Description("Z轴速度")]
    SPEED_Z,

    [Description("X轴加速度")]
    SPEED_XACC,

    [Description("Y轴加速度")]
    SPEED_YACC,

    [Description("Z轴加速度")]
    SPEED_ZACC,

    [Description("Z轴加速度(新)")]
    SPEED_ZACC_NEW
}

//属性操作按下摇杆反方向时的处理
public enum DModifyXBackward
{
    [Description("继续朝着当前方向前进")]
    NONE = 1,       //继续朝着当前方向前进

    [Description("停止")]
    STOP,          //停止

    [Description("朝着反方向移动 ")]
    BACKMOVE,      //朝着反方向移动  
}

[System.Serializable]
[StructLayout(LayoutKind.Explicit)]
public struct SUnion
{
    [FieldOffset(0)]
    public bool _bool;
    [FieldOffset(0)]
    public float _float;
    [FieldOffset(0)]
    public int _int;
    [FieldOffset(0)]
    public Quaternion _quat;
    [FieldOffset(0)]
    public uint _uint;
    [FieldOffset(0)]
    public Vector3 _vec3;
}
//速度修正
[System.Serializable]
[DSFFrameEventType("属性修改")]
public class DSkillPropertyModify : DSkillFrameEvent
{
    public int tag;
    public DSkillPropertyModifyType modifyfliter;
    public float value;
    public float valueAcc;
    public float movedValue;
    public float movedValueAcc;
    public DModifyXBackward modifyXBackward;
    public float movedYValue;
    public float movedYValueAcc;
    public SUnion svalue;
	public bool jumpToTargetPos;
    public bool joyStickControl;
    public bool eachFrameModify;
    public bool useMovedYValue;

    public override void copyFrameEvent(DSkillFrameEvent src)
    {
        base.copyFrameEvent(src);
        var data = src as DSkillPropertyModify;
        tag = data.tag;
        modifyfliter = data.modifyfliter;
        value = data.value;
        valueAcc = data.valueAcc;
        movedValue = data.movedValue;
        movedValueAcc = data.movedValueAcc;
        modifyXBackward = data.modifyXBackward;
        movedYValue = data.movedYValue;
        movedYValueAcc = data.movedYValueAcc;
        svalue = data.svalue;
        jumpToTargetPos = data.jumpToTargetPos;
        joyStickControl = data.joyStickControl;
        eachFrameModify = data.eachFrameModify;
        useMovedYValue = data.useMovedYValue;
    }
}

//帧标签
public enum DSFFrameTags
{
    TAG_NEWDAMAGE = 1 << 0,             //重置伤害
    TAG_LOCKZSPEED = 1 << 1,            //锁定Z轴
    TAG_LOCKZSPEEDFREE = 1 << 2,        //解锁z轴
	TAG_IGNORE_GRAVITY = 1 << 3,        //忽略重力
	TAG_RESTORE_GRAVITY = 1 << 4,       //重置重力
    TAG_SET_TARGET_POS_XY = 1 << 5,     //将自己的位置设置成攻击对象当前的位置的XY
    TAG_CURFRAME = 1 << 6,              //标识当前帧事件
    TAG_CHANGEFACE = 1<< 7,             //在这一帧进行转向
    TAG_CHANGEFACEBYDIR = 1 << 8,       //在这一帧根据玩家摇杆方向判断是否需要转向 
    TAG_REMOVEEFFECT = 1 << 9,          //移除特效
    TAG_STARTCHECKHIT = 1 << 10,        //开始检测攻击
    TAG_STARTDEALSKIPPHASE = 1 << 11,   //判定是否攻击命中 命中则切换到下个阶段
    TAG_NAME_HIDE = 1 << 12,            //隐藏名字板
    TAG_NAME_SHOW = 1 << 13,            //显示名字板
    TAG_SHADOW_HIDE = 1 << 14,          //隐藏影子
    TAG_SHADOW_SHOW = 1 << 15,          //显示影子
    TAG_HPBAR_HIDE = 1 << 16,           //隐藏血条
    TAG_HPBAR_SHOW = 1 << 17,           //显示血条
    TAG_LOOKAT_TARGET = 1 << 18,        //转向攻击目标
    TAG_REMOVE_MECHANISM = 1 << 19,     //删除机制
    TAG_REMOVE_BUFF = 1 << 20,          //删除Buff
    TAG_OPEN_2ND_STATE = 1 << 21,       //开启第二形态
    TAG_CLOSE_2ND_STATE = 1 << 22,      //关闭第二形态

}
[System.Serializable]
[DSFFrameEventType("帧标签")]
public class DSkillFrameTag : DSkillFrameEvent
{
    public DSFFrameTags tag;
    public string tagFlag;                //用于标识当前技能帧（技能阶段ID+标记ID)               
}

//sfx播放
[System.Serializable]
public class DSkillSfx : DSkillFrameEvent
{
    public UnityEngine.Object soundClip;
    public DAssetObject soundClipAsset;
    public bool loop = false;
	public int soundID;
    public bool useActorSpeed;
    public bool phaseFinishDelete;
    public bool finishDelete;
    public float volume =1;

    public override void copyFrameEvent(DSkillFrameEvent src)
    {
        base.copyFrameEvent(src);
        var data = src as DSkillSfx;
        soundClip = data.soundClip;
        soundClipAsset = data.soundClipAsset;
        loop = data.loop;
        soundID = data.soundID;
        useActorSpeed = data.useActorSpeed;
        phaseFinishDelete = data.phaseFinishDelete;
        finishDelete = data.finishDelete;
        volume = data.volume;
    }
}

//帧效果
[System.Serializable]
public class DSkillFrameEffect : DSkillFrameEvent
{
    public int effectID;
    public float buffTime;
    public bool phaseDelete = false;//阶段结束删除
	public bool finishDelete = true;//是否在技能结束(中断)删除膝撞霸体buff
    public bool finishDeleteAll = false;//是否在技能结束(中断)删除帧效果所有buff
    public bool useBuffAni = true;//是否使用buff动画（霸体）
    public bool usePause = false;
    public float pauseTime;
    public int mechanismId;     //机制ID

    public override void copyFrameEvent(DSkillFrameEvent src)
    {
        var data = src as DSkillFrameEffect;
        base.copyFrameEvent(src);
        effectID = data.effectID;
        buffTime = data.buffTime;
        phaseDelete = data.phaseDelete;
        finishDelete = data.finishDelete;
        finishDeleteAll = data.finishDeleteAll;
        useBuffAni = data.finishDeleteAll;
        usePause = data.usePause;
        pauseTime = data.pauseTime;
        mechanismId = data.mechanismId;
    }
}

//摄像机偏移
[System.Serializable]
public class DSkillCameraMove : DSkillFrameEvent
{
    public float offset;
    public float duraction;
}

//移动控制
[System.Serializable]
public class DSkillWalkControl : DSkillFrameEvent
{
    public SkillWalkMode walkMode;
    public float walkSpeedPercent;
    public bool useSkillSpeed;
    public float walkSpeedPercent2;
}

public enum DSFGrapOp
{
    [Description("抓取判定")]
    GRAP_JUDGE = 1,   //抓取判定

    [Description("抓取执行")]
    GRAP_EXECUTE = 1 << 1, //抓取执行

    [Description("抓取释放")]
    GRAP_RELEASE = 1 << 2,  //抓取释放

    [Description("抓取中断")]
    GRAP_INTERRUPT = 1 << 3,//抓取中断

    [Description("抓取并跳到下一阶段标记")]
    GRAP_JUDGE_SKIP_PHASE = 1 << 29,//抓取并跳到下一阶段

    [Description("抓取并且马上执行")]
    GRAP_JUDGE_EXECUTE = 1 << 30,//抓取并马上执行

    [Description("调整被抓取者的位置")]
    GRAP_CHANGE_TARGETPOS = 1 << 4,//调整被抓取者的位置

    [Description("停止调整抓取者的位置")]
    GRAP_STOPCHANGE_TARGETPOS = 1 << 5,//停止调整抓取者的位置

    [Description("被抓取者的目标行为")]
    GRAP_CHANGE_TARGETACTION = 1 << 6,//改变被抓取者的动作

    [Description("被抓取者的目标旋转角度")]
    GRAP_CHANGE_TARGETROTATION = 1 << 7,//改变被抓取者的旋转
}

//抓取帧
[System.Serializable]
[DSFFrameEventType("抓取帧")]
public class DSkillFrameGrap : DSkillFrameEvent
{
    public DSFGrapOp op;
    public bool faceGraber;         //是否面向被抓取者
    public Vec3 targetPos;
    public ActionType targetAction; //改变抓取者的动作
    public int targetAngle;

#if UNITY_EDITOR
    [NonSerialized]
    public DEditorObj obj = null;

    public DSkillFrameGrap()
    {
        obj = new DEditorObj(this);
    }
#endif
}

//状态操作
public enum DSFEntityStates
{
    //对应游戏里的
    IDLE    = 0,
    ATTACK  = 1,
    RUN     = 3,
    WALK    = 2,
    HURT    = 4,
    JUMP    = 5,
    JUMPBACK = 6,
    FALL    = 9,
    CASTSKILL = 14
}

public enum DSFEntityStateOp
{
    Push = 0,
    Pop,
    Clear,
    Locomote
}

public enum DSFEntityStateTag
{
    CLEARSPEED = 1,
    ACTIONHANDLE = 2,
    MOVECTRL = 4,
    GRAPRELEASE = 8
}

[System.Serializable]
[DSFFrameEventType("状态操作帧")]
public class DSkillFrameStateOp : DSkillFrameEvent
{
    public DSFEntityStateOp     op;
    public DSFEntityStates      state;
    public int                  idata1;
    public int                  idata2;
    public float                fdata1;
    public float                fdata2;
    public DSFEntityStateTag    statetag;
}

//振屏幕
[System.Serializable]
[DSFFrameEventType("震屏帧")]
public class DSkillFrameEventSceneShock : DSkillFrameEvent
{
    public float time;
    public float speed;
    public float xrange;
    public float yrange;

    public bool isNew;
    public int mode;
    public bool decelerate;
    public float xreduce;
    public float yreduce;
    public float radius;
    public int num;

    public override void copyFrameEvent(DSkillFrameEvent src)
    {
        base.copyFrameEvent(src);

        var data = src as DSkillFrameEventSceneShock;
        time = data.time;
        speed = data.speed;
        xrange = data.xrange;
        yrange = data.yrange;

        isNew = data.isNew;
        mode = data.mode;
        decelerate = data.decelerate;
        xreduce = data.xreduce;
        yreduce = data.yreduce;
        radius = data.radius;
        num = data.num;
    }
}

[System.Serializable]
public class DSkillFrameEventTest
{
    public SUnion bo;
    public SUnion vec;
    public SUnion quat;
}


public enum BeActionType
{
	MoveBy = 0,
	ScaleBy,
	MoveToSavedPosition,
    MoveTo,
    ScaleTo,
} 

[System.Serializable]
public class DActionData : DSkillFrameEvent
{
	public BeActionType actionType;
	public float duration;
	public float deltaScale;
	public Vec3 deltaPos;
    public bool ignoreBlock = true;//是否无视阻挡
}
public enum SummonPosType
{
    [Description("面前一个单位")]
    FACE = 0,
    [Description("原地")]
    ORIGIN = 1,
    [Description("面前一个单位不管阻挡")]
    FACE_FORCE = 2,
    [Description("面前没有阻挡的位置")]
    FACE_BLACK = 3,
}

[System.Serializable]
[DSFFrameEventType("AddBuffInfoOrBuff")]
public class DSkillBuff : DSkillFrameEvent
{
    public float buffTime;
    public int buffID;
    public bool phaseDelete;
    public int[] buffInfoList = new int[0];
    public bool finishDeleteAll = false;

    public int level = 1;
    public bool levelBySkill = false;

    public override void copyFrameEvent(DSkillFrameEvent src)
    {
        base.copyFrameEvent(src);

        var data = src as DSkillBuff;
        buffTime = data.buffTime;
        buffID = data.buffID;
        phaseDelete = data.phaseDelete;
        if (data.buffInfoList != null)
        {
            buffInfoList = new int[data.buffInfoList.Length];
            Array.Copy(data.buffInfoList, buffInfoList, data.buffInfoList.Length);
        }


        finishDeleteAll = data.finishDeleteAll;
        level = data.level;
        levelBySkill = data.levelBySkill;
    }
}

[System.Serializable]
[DSFFrameEventType("DoSummon")]
public class DSkillSummon : DSkillFrameEvent
{
    public int summonID;
    public int summonLevel;
    public bool levelGrowBySkill;
    public int summonNum;
    public int posType;
    public int[] posType2 = new int[0];
    public bool isSameFace = true;

    public override void copyFrameEvent(DSkillFrameEvent src)
    {
        base.copyFrameEvent(src);

        var data = src as DSkillSummon;
        summonID = data.summonID;
        summonLevel = data.summonLevel;
        levelGrowBySkill = data.levelGrowBySkill;
        summonNum = data.summonNum;
        posType = data.posType;
        if (data.posType2 != null)
        {
            posType2 = new int[data.posType2.Length];
            Array.Copy(data.posType2, posType2, data.posType2.Length);
        }
        isSameFace = data.isSameFace;
    }
}

[System.Serializable]
public class DSkillMechanism : DSkillFrameEvent
{
    public int id = 0;
    public float time = 0;
    public int level = 1;
    public bool levelBySkill = false;
    public bool phaseDelete = false;
    public bool finishDeleteAll = false;

    public override void copyFrameEvent(DSkillFrameEvent src)
    {
        base.copyFrameEvent(src);

        var data = src as DSkillMechanism;
        id = data.id;
        time = data.time;
        level = data.level;
        levelBySkill = data.levelBySkill;

        phaseDelete = data.phaseDelete;
        finishDeleteAll = data.finishDeleteAll;
    }
}
