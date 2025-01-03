using UnityEngine;
using System.Collections;


public enum BeStateType
{
    STAND       = 1,
    WALK        = 1 << 1,
    JUMP        = 1 << 2,
    SKILL       = 1 << 3,
    FALLGROUND  = 1 << 4,   //倒地
    FLOAT       = 1 << 5,   //浮空
    GRAB        = 1 << 6,   //抓取
    DEAD        = 1 << 7,
	BLOCK		= 1 << 8, //格挡
}

public enum BeBuffStateType
{
    BATI        = 1,
    INVINCIBLE  = 1 << 1,
    FLASH       = 1 << 2,
    BLEEDING    = 1 << 3,
    BURN        = 1 << 4,
    POISON      = 1 << 5,
    BLIND       = 1 << 6,
    STUN        = 1 << 7,
    STONE       = 1 << 8,
    FROZEN      = 1 << 9,
    SLEEP       = 1 << 10,
    CONFUNSE    = 1 << 11,
    STRAIN      = 1 << 12,
    SPEED_DOWN  = 1 << 13,
    CURSE       = 1 << 14,
	BLOCK 		= 1 << 15,//格挡
    DEAD_PROTECT = 1 << 16,//死亡保护
}

/*
0:不可以
1:可以
*/
public enum BeAbilityType
{
    //基本能力
    MOVE        = 1,    //可以移动
    ATTACK      = 2,    //可以攻击
    FALLGROUND  = 3,    //可以倒地
    FLOAT       = 4,    //可以浮空
    BEGRAB        = 5,    //可以被抓取
    BEBREAK       = 6,    //可以被破招
	BEHIT		  = 7,   //be hit
	BETARGETED	  = 8,  //能不能被选为攻击对象

    //异常状态能力
    FLASH       = 10,   //感电
    BLEEDING    = 11,   //流血
    BURN        = 12,   //燃烧
    POISON      = 13,   //中毒
    BLIND       = 14,   //失明
    STUN        = 15,   //眩晕
    STONE       = 16,   //石化
    FROZEN      = 17,   //冰冻
    SLEEP       = 18,   //睡眠
    CONFUNSE    = 19,   //混乱
    STRAIN      = 20,   //束缚
    SPEED_DOWN  = 21,   //减速
    CURSE       = 22,   //诅咒

    BLOCK       = 23,   //受阻挡
    GRAVITY     = 24,   //受重力影响
	CHANGE_FACE = 25,  //可以转向
	DUPLICATE	= 26,  //可以分身
	X_MOVE		= 27,  //可以x轴移动
	Y_MOVE		= 28,  //可以y轴移动
	Z_MOVE		= 29,  //可以z轴移动
	ADD_BUFF	= 30, //可以上buff

    HAVE_HURT = 31,//默认不免伤

    CHAOS = 32,     //混乱状态 不能攻击队友           (默认是不拥有这个能力)
    BEHIT_NODAMAGE = 33,  //能被攻击 但是不造成伤害   (默认是不拥有这个能力)
    IGNOREFAR = 34,      //免疫远程伤害                    (默认是不拥有这个能力)
    BEBREAK_FORCE = 35,    //可以被破招(强制）
    CANADDABNORMALBUFF = 36,//可以上异常buff
    CANNOTBE_SUCKED = 37,//不能被黑洞机制吸过去 (默认不拥有这个能力)
    CANBEHITONLYNEAR = 38,   //只能被近身攻击伤害 (默认不拥有这个能力)
    CANNOTGRAP = 39,//不可以抓取
    ARROW_INVISIBLE = 40,//箭头消失（PVP隐身）
    NOTREMOVE_TRANSDOOR = 41,//过门的时候不移除
    CANATTACKFRIEND = 42,     //可以攻击队友的能力           (默认是不拥有这个能力)
    SUPER_BATI = 43,         //超级霸体（buff的强控不能控制）
    CANPAUSE = 44,       //可以顿帧
    CHANGE_ACTION = 45,
    ATTACK_FRIEND_ENEMY = 46, //可以攻击友方和敌方
    CAN_HIT_BACK = 47,   //可以背击(表中填了表示不能被背击)
    CAN_UPDATEX = 48,   //X轴位置需要刷新(默认不填表示需要刷新)
    CAN_UPDATEY = 49,   //Y轴位置需要刷新(默认不填表示需要刷新)
    CAN_UPDATEZ = 50,   //Z轴位置需要刷新(默认不填表示需要刷新)

    CAN_DO_SKILL_CMD = 51, //能否接技能帧协议(程序内部使用 无需提供出来)
    COUNT = 52,
}
	
public enum GrabState
{
	NONE = 0,
    GRABBING,           //正在抓取
    ENDGRABBING,        //结束抓取
	WILL_BEGRAB,        //开始被抓取
	BEING_GRAB,         //正在被抓取
}

public class BeStateControl {

	protected CrypticInt32[] ability = new CrypticInt32[(int)BeAbilityType.COUNT];//buff改变的状态

    public CrypticInt32[] Ability
    {
        get
        {
            return ability;
        }
    }

	protected CrypticInt32[] bornAbility = new CrypticInt32[(int)BeAbilityType.COUNT];//出生状态

    protected SeFlag stateFlag = new SeFlag(0);
    protected SeFlag buffStateFlag = new SeFlag(0);

	protected GrabState grabState;

    BeEntity owner;

    public BeStateControl(BeEntity owner)
    {
        this.owner = owner;
		Reset ();
    }

	public void Reset()
	{
		for(int i=0; i<(int)BeAbilityType.COUNT; ++i)
		{
			ability[i] = 1;
			bornAbility[i] = 1;
		}
	}

    public bool HasState(BeStateType target)
    {
        return stateFlag.HasFlag((int)target);
    }

    public void SetState(BeStateType target, bool clear=false)
    {
        if (clear)
            stateFlag.ClearFlag((int)target);
        else
            stateFlag.SetFlag((int)target);
    }

    public bool HasBuffState(BeBuffStateType target)
    {
        return buffStateFlag.HasFlag((int)target);
    }

    public void SetBuffState(BeBuffStateType target, bool clear=false)
    {
        if (clear) { 
            buffStateFlag.ClearFlag((int)target);
        //    Logger.LogErrorFormat("clear Buff State {0}", target);
        }
        else { 
            buffStateFlag.SetFlag((int)target);
      //      Logger.LogErrorFormat("set buff state {0}", target);
        }
    }

    public void ResetMoveAbility()
    {
        ability[(int)BeAbilityType.MOVE] = 1;
    }

    public void ResetAttackAbility()
    {
        ability[(int)BeAbilityType.ATTACK_FRIEND_ENEMY] = 1;
    }
    public bool HasAbility(BeAbilityType abilityType)
    {
        return ability[(int)abilityType] > 0;
    }

	public bool HasBornAbility(BeAbilityType abilityType)
	{
		return bornAbility[(int)abilityType] > 0;
	}

	public void SetBornAbility(BeAbilityType abilityType, bool enable)
	{
		if (enable)
			bornAbility[(int)abilityType]++;
		else
			bornAbility[(int)abilityType]--;
	}

    public void SetAbilityEnable(BeAbilityType abilityType, bool enable)
    {
       
        //Logger.LogErrorFormat("set ability {0} value {1} name:{2}", abilityType, enable, owner.GetName());
		if (enable)
        	ability[(int)abilityType]++;
		else
			ability[(int)abilityType]--;

    }

	public void SetGrabState(GrabState state)
	{
		grabState = state;
	}

	public bool WillBeGrab()
	{
		return grabState == GrabState.WILL_BEGRAB;
	}

    //正在抓取
    public bool IsGrabbing()
    {
        return grabState == GrabState.GRABBING;
    }

	public bool IsBeingGrab()
	{
		return grabState == GrabState.BEING_GRAB;
	}

    public bool IsEndGrab()
    {
        return grabState == GrabState.ENDGRABBING;
    }

    public bool CanBeGrab()
    {
        bool cannotFlag = !HasAbility(BeAbilityType.BEGRAB);

        return !cannotFlag;
    }

    /// <summary>
    /// 强制不能被破招
    /// </summary>
    /// <returns>默认返回false 能够被强制破霸体</returns>
    public bool CanBeForceBreakAction()
    {
        return HasAbility(BeAbilityType.BEBREAK_FORCE);
    }

    /// <summary>
    /// 是否能被破招 霸体状态下不能被破招
    /// </summary>
    /// <returns>默认返回true</returns>
    public bool CanBeBreakAction()
    {
        bool cannotFlag = !HasAbility(BeAbilityType.BEBREAK);

        return !cannotFlag && CanBeHit();
    }

	public bool CanBeFloat()
	{
		bool cannotFlag = !HasAbility (BeAbilityType.FLOAT);
		return !cannotFlag;
	}

	public bool CanMove()
	{
		return HasAbility(BeAbilityType.MOVE);
	}

	public bool CanAttack()
	{
		return HasAbility(BeAbilityType.ATTACK);
	}

    public bool CanBeHit()
    {
        return HasAbility(BeAbilityType.BEHIT);
    }

    //是否被阻挡
    public bool BlockByScene()
    {
        return HasAbility(BeAbilityType.BLOCK);
    }

    public bool IgnoreGravity()
    {
        return !HasAbility(BeAbilityType.GRAVITY);
    }

	public bool CanTurnFace()
	{
		return HasAbility(BeAbilityType.CHANGE_FACE);
	}
	public bool CanDuplicate()
	{
		return HasAbility(BeAbilityType.DUPLICATE);
	}

	public bool CanMoveX()
	{
		return HasAbility(BeAbilityType.X_MOVE);
	}

	public bool CanMoveY()
	{
		return HasAbility(BeAbilityType.Y_MOVE);
	}

	public bool CanMoveZ()
	{
		return HasAbility(BeAbilityType.Z_MOVE);
	}

	public bool CanAddBuff()
	{
		return HasAbility(BeAbilityType.ADD_BUFF);
	}

	public bool CanHurt()
	{
		return HasAbility(BeAbilityType.HAVE_HURT);
	}

	public bool CanBeTargeted()
	{
		return HasAbility(BeAbilityType.BETARGETED);
	}

    //是否混乱状态 可以被队友攻击(默认返回false)
    public bool IsChaosState()
    {
        return !HasAbility(BeAbilityType.CHAOS);
    }

    //能被攻击 但是不造成伤害 只用于伤害数值计算(默认返回false)
    public bool CanBeHitNoDamage()
    {
        return !HasAbility(BeAbilityType.BEHIT_NODAMAGE);
    }
    
    //免疫远程伤害 默认返回false
    public bool IgnoreFarDamage()
    {
        return !HasAbility(BeAbilityType.IGNOREFAR);
    }

    //只能被近身攻击
    public bool CanBeHitOnlyNear()
    {
        return !HasAbility(BeAbilityType.CANBEHITONLYNEAR);
    }

    public bool CanGrab()
    {
        return !HasAbility(BeAbilityType.CANNOTGRAP);
    }

    public bool CanAddAbnormalBuffWithBornAbility(BuffType buffType)
    {
        BeAbilityType abilityType = (BeAbilityType)(buffType + ((int)BeAbilityType.FLASH - (int)BuffType.FLASH));

      //  Logger.LogErrorFormat("CanAddAbnormalBuff:bufftype:{0} beAbilityType:{1}", buffType, abilityType);

        return HasBornAbility(abilityType);

    }

    public bool CanAddAbnormalBuffAbility(BuffType buffType)
    {
        BeAbilityType abilityType = (BeAbilityType)(buffType + ((int)BeAbilityType.FLASH - (int)BuffType.FLASH));


        return HasAbility(abilityType);

    }


    //添加能力36 默认返回True
    public bool CanAddAbnormalBuff()
    {
        return HasAbility(BeAbilityType.CANADDABNORMALBUFF);
    }

    //不能被黑洞机制吸引过去 默认返回False
    public bool CanNotAbsorbByBlockHole()
    {
        return !HasAbility(BeAbilityType.CANNOTBE_SUCKED);
    }
    public bool HaveSuperBati()
    {
        return !HasAbility(BeAbilityType.SUPER_BATI);
    }
    public bool IsInvisible()
    {
        return !HasAbility(BeAbilityType.ARROW_INVISIBLE);
    }


    //过门时不移除 默认会移除
    public bool NotRemoveTransDoor()
    {
        return !HasAbility(BeAbilityType.NOTREMOVE_TRANSDOOR);
    }

    //可以攻击队友的能力(默认返回false)
    public bool CanAttackFriend()
    {
        return !HasAbility(BeAbilityType.CANATTACKFRIEND);
    }

    public bool CanAttackFriendAndEnemy()
    {
        return !HasAbility(BeAbilityType.ATTACK_FRIEND_ENEMY);
    }

    /// <summary>
    /// X轴位置是否需要刷新
    /// </summary>
    public bool CanUpdateX()
    {
        return HasAbility(BeAbilityType.CAN_UPDATEX);
    }

    /// <summary>
    /// Y轴位置是否需要刷新
    /// </summary>
    public bool CanUpdateY()
    {
        return HasAbility(BeAbilityType.CAN_UPDATEY);
    }

    /// <summary>
    /// Z轴位置是否需要刷新
    /// </summary>
    public bool CanUpdateZ()
    {
        return HasAbility(BeAbilityType.CAN_UPDATEZ);
    }
}
