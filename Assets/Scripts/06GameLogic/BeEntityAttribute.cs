
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;
using GameClient;
using ProtoTable;
using Protocol;

public enum AttackResult
{
    MISS,
    NORMAL,
    CRITICAL,
}
	
public enum AttackType
{
	NONE = 0,
	//物理/魔法
	PHYSIC,
	MAGIC,
	//远/近,属性
	NEAR,
	FAR,
}

public enum MagicElementType
{
    NONE = 0,
    [UIProperty("光属性", "")]
    LIGHT,	//光
    [UIProperty("火属性", "")]
    FIRE,	//火
    [UIProperty("冰属性", "")]
    ICE,	//冰
    [UIProperty("暗属性", "")]
    DARK,//暗
    MAX
}

public struct AddDamageInfo
{
	public AddDamageInfo(int v, int at = 0)
	{
		value = v;
		attackType = (AttackType)at;
	}
	public CrypticInt32 value;
	public AttackType attackType;
}


public class DisplayAttribute
{
    public float maxHp;
    public float maxMp;
    public float hpRecover;
    public float mpRecover;

    public float baseSta;                 //基础体力
    public float baseAtk;                 //基础攻击
    public float baseInt;                 //基础智力
    public float baseIndependence;        //基础独立攻击力
    public float baseSpr;                 //基础精神

    public float attack;                  //物理攻击
    public float magicAttack;             //魔法攻击
    public float defence;                 //物理防御
    public float magicDefence;            //魔法防御
    public float attackSpeed;             //攻击速度
    public float spellSpeed;              //释放速度
    public float moveSpeed;               //移动速度
    public float ciriticalAttack;         //物理暴击
    public float ciriticalMagicAttack;    //魔法暴击
    public float dex;                     //命中
    public float dodge;                   //闪避
    public float frozen;                  //僵值
    public float hard;                    //硬值
    public float resistMagic;               //抗魔值

	public int hp;
	public int mp;

    //这里添加以后要做的属性
    public float lightAttack;         // 光属性攻击
    public float fireAttack;          // 火属性攻击
    public float iceAttack;           // 冰属性攻击
    public float darkAttack;          // 暗属性攻击
    public float lightDefence;        // 光属性抗性
    public float fireDefence;         // 火属性抗性
    public float iceDefence;          // 冰属性抗性
    public float darkDefence;         // 暗属性抗性
     
    //这里用于判断是红还是绿 =0 白色 >0绿色 <0红色
    public Dictionary<string, int> attachValue = new Dictionary<string, int>();
    public DisplayAttribute()
    {
		var names = Enum.GetNames(typeof(AttributeType));
		for(int i=0; i<names.Length; ++i)
        //foreach(var name in names)
        {
			var name = names[i];
            attachValue.Add(name, 0);
        }
    }

}

public enum AttributeType
{
    [UIProperty("HP最大值", "+{0}")]
    maxHp = 0,
    [UIProperty("MP最大值", "+{0}")]
    maxMp,
    [UIProperty("HP回复", "+{0}")]
    hpRecover,
    [UIProperty("MP回复", "+{0}")]
    mpRecover,
    [UIProperty("物理攻击", "+{0}")]
    attack,
    [UIProperty("魔法攻击", "+{0}")]
    magicAttack,
    [UIProperty("物理防御", "+{0}")]
    defence,
    [UIProperty("魔法防御", "+{0}")]
    magicDefence,
    [UIProperty("攻击速度", "+{0}%")]
    attackSpeed,
    [UIProperty("施放速度", "+{0}%")]
    spellSpeed,
    [UIProperty("移动速度", "+{0}%")]
    moveSpeed,
    [UIProperty("物理爆击", "+{0}%")]
    ciriticalAttack,
    [UIProperty("魔法爆击", "+{0}%")]
    ciriticalMagicAttack,
    [UIProperty("命中率", "+{0}%")]
    dex,                //命中      
    [UIProperty("闪避", "+{0}%")]
    dodge,              //闪避     
    [UIProperty("僵值", "+{0}")]
    frozen,             //僵值
    [UIProperty("硬值", "+{0}")]
    hard,               //硬值

    [UIProperty("异常状态抗性", "+{0}")]
    abnormalResist,     //异常抗性
    [UIProperty("暴击伤害百分比", "+{0}%")]
    criticalPercent,    //暴击伤害百分比
    [UIProperty("物理技能CD缩减百分比", "+{0}%")]
    cdReduceRate,       //物理技能cd缩短百分比（1000）
    [UIProperty("魔法技能CD缩减百分比", "+{0}%")]
    cdReduceRateMagic,  //魔法技能cd缩短百分比（1000）

    [UIProperty("物理技能mp消耗缩减百分比", "+{0}%")]
    mpCostReduceRate,       //物理技能mp缩短百分比（1000）
    [UIProperty("魔法技能mp消耗缩减百分比", "+{0}%")]
    mpCostReduceRateMagic,	//魔法技能mp缩短百分比（1000）

    //
    [UIProperty("物理攻击增加百分比", "+{0}%")]
    attackAddRate,
    [UIProperty("魔法攻击增加百分比", "+{0}%")]
    magicAttackAddRate,
    [UIProperty("无视物理防御攻击力", "+{0}")]
    ignoreDefAttackAdd,
    [UIProperty("无视魔法防御攻击力", "+{0}")]
    ignoreDefMagicAttackAdd,

    [UIProperty("物理攻击减免百分比", "-{0}%", false)]
    attackReduceRate,
    [UIProperty("魔法攻击减免百分比", "-{0}%", false)]
    magicAttackReduceRate,
    [UIProperty("物理攻击减免固定值", "-{0}", false)]
    attackReduceFix,
    [UIProperty("魔法攻击减免固定值", "-{0}", false)]
    magicAttackReduceFix,

    [UIProperty("物理防御增加百分比", "-{0}%", false)]
    defenceAddRate,     //物理防御增加百分比
    [UIProperty("魔法防御增加百分比", "-{0}%", false)]
    magicDefenceAddRate,//魔法防御增加百分比

    [UIProperty("无视物理防御百分比", "+{0}%", false)]
    ingnoreDefRate,//无视物理防御百分比
    [UIProperty("无视魔法防御百分比", "+{0}%", false)]
    ingnoreMagicDefRate,//无视魔法防御百分比

    //一级属性
    [UIProperty("力量", "+{0}")]
    baseAtk,            //基础攻击
    [UIProperty("智力", "+{0}")]
    baseInt,            //基础智力
    [UIProperty("体力", "+{0}")]
    baseSta,            //基础体力
    [UIProperty("独立攻击力", "+{0}")]
    baseIndependence,     //基础独立攻击力     
    [UIProperty("精神", "+{0}")]
    baseSpr,            //基础精神
    [UIProperty("强化固定攻击力", "+{0}")]
    ingoreIndependence, //固定攻击力

    hpGrow,             //hp成长
    mpGrow,             //mp成长
    atkGrow,            //基础攻击成长
    intGrow,            //基础智力成长
    staGrow,            //基础体力成长
    sprGrow,            //基础精神成长
    hardGrow,           //基础硬值成长

    sta,
    spr,

    resistMagic,        //抗魔值

    COUNT,
}
	
 public class BattleData
 {
	public BeEntity owner;
 
	public FrameRandomImp FrameRandom
    {
        get
        {
#if !SERVER_LOGIC 

            if(owner.FrameRandom == null)
            {
                return BeSkill.randomForTown;
            }


 #endif
            
            return owner.FrameRandom;
        }
    }


	//public CrypticInt32 hp;
	protected CrypticInt32 _hp;

	public int hp{
		get{
			return _hp;
		}

		set
		{
			_hp = value;
		}
	}
	public CrypticInt32 mp;
	public CrypticInt32 maxHp;//经过体力计算最终血量（如果没有抗魔值，那么这个值和fMaxHp一样。如果有抗魔值，那么这个值是经过抗魔值计算的。）
	public CrypticInt32 maxMp;
	public CrypticInt32 hpRecover;
	public CrypticInt32 mpRecover;
	public CrypticInt32 hpReduce; //hp每秒减
	public CrypticInt32 mpReduce; //mp每秒减
	//public float hpScale = (float)(GlobalLogic.VALUE_1);
	public CrypticInt32 hpScale;


    //以下为一级属性
	public CrypticInt32 baseSta; //基础体力，显示用体力
	public CrypticInt32 baseAtk; //基础攻击  显示用力量
	public CrypticInt32 baseInt; //基础智力  显示用智力
	public CrypticInt32 baseSpr; //基础精神  显示用精神
    public CrypticInt32 baseIndependence;//独立攻击力


    //二级属性
    public CrypticInt32 _maxHp;
	public CrypticInt32 _maxMp;
	public CrypticInt32 _hpRecover;
	public CrypticInt32 _mpRecover;              
	public CrypticInt32 attack;                  //物理攻击
	public CrypticInt32 magicAttack;             //魔法攻击
	public CrypticInt32 defence;                 //物理防御
	public CrypticInt32 magicDefence;            //魔法防御
	public CrypticInt32 attackSpeed;             //攻击速度，百分比显示
	public CrypticInt32 spellSpeed;              //释放速度，百分比显示
	public CrypticInt32 moveSpeed;               //移动速度，百分比显示
	public CrypticInt32 ciriticalAttack;         //物理暴击，百分比显示
	public CrypticInt32 ciriticalMagicAttack;    //魔法暴击，百分比显示
	public CrypticInt32 dex;                     //命中    ,百分比显示
	public CrypticInt32 dodge;                   //闪避    ,百分比显示
	public CrypticInt32 frozen;                  //僵值
	public CrypticInt32 hard;                    //硬值
    public CrypticInt32 initDefence;             //初始防御 怪物用，加上出生buff后的
    public CrypticInt32 initMagicDefence;             //初始防御 怪物用，加上出生buff后的

    //增加的属性
    //武器强化
    public CrypticInt32 attackAddRate;           //*1000  物理攻击增加百分比
	public CrypticInt32 magicAttackAddRate;      //*1000  魔法攻击增加百分比
	public CrypticInt32 ignoreDefAttackAdd;      //       无视防御攻击增加
	public CrypticInt32 ignoreDefMagicAttackAdd; //       无视防御魔法攻击增加
    public CrypticInt32 ingoreIndependence;      //固定攻击力
    //防具强化
    public CrypticInt32 attackReduceRate;        //物理攻击减免百分比
	public CrypticInt32 magicAttackReduceRate;   //魔法攻击减免百分比
	public CrypticInt32 attackReduceFix;         //物理攻击减免固定值
	public CrypticInt32 magicAttackReduceFix;    //魔法攻击减免固定值
    public CrypticInt32 ingoreDefRate;           //无视物理防御百分比
    public CrypticInt32 ingoreMagicDefRate;      //无视魔法防御百分比


    public CrypticInt32 abnormalResist;          //异常抗性
	public CrypticInt32 criticalPercent;         //暴击伤害百分比

	public CrypticInt32 cdReduceRate;            //技能cd缩短百分比（1000）
	public CrypticInt32 cdReduceRateMagic;
	public CrypticInt32 mpCostReduceRate;		//mp消耗虽短百分比(1000)
	public CrypticInt32 mpCostReduceRateMagic;


	public CrypticInt32 defenceAddRate;
	public CrypticInt32 magicDefenceAddRate;

	public CrypticInt32 attachHurtRate;          //附加伤害百分比
	public CrypticInt32 attachHurtFix;           //附加伤害固定值

    //成长
	public CrypticInt32 hpGrow;         //hp成长
	public CrypticInt32 mpGrow;         //mp成长
	public CrypticInt32 atkGrow;        //基础攻击成长
	public CrypticInt32 intGrow;        //基础智力成长
	public CrypticInt32 staGrow;        //基础体力成长
	public CrypticInt32 sprGrow;        //基础精神成长
	public CrypticInt32 mpRecoverGrow;  //mp恢复成长
	public CrypticInt32 hardGrow;       //基础硬值成长 


	public List<AddDamageInfo> attachAddDamageFix = new List<AddDamageInfo>();//附加伤害固定值
	public List<AddDamageInfo> attachAddDamagePercent = new List<AddDamageInfo>();//附加伤害百分比
	public List<AddDamageInfo> addDamageFix = new List<AddDamageInfo>();//增加伤害固定值
	public List<AddDamageInfo> addDamagePercent = new List<AddDamageInfo>();//增加伤害百分比
    public List<AddDamageInfo> skillAddDamagePercent = new List<AddDamageInfo>();//技能增加伤害百分比
    public List<AddDamageInfo> skillAddMagicDamagePercent = new List<AddDamageInfo>();//技能增加魔法伤害百分比

    public List<AddDamageInfo> reduceDamagePercent = new List<AddDamageInfo>();//减伤百分比
	public List<AddDamageInfo> reduceDamageFix = new List<AddDamageInfo>();//减伤固定值
    public List<AddDamageInfo> reduceExtraDamagePercent = new List<AddDamageInfo>();//附加减伤百分比
    public List<AddDamageInfo> reduceMeiyingDamagePercent = new List<AddDamageInfo>();//魅影减伤百分比
    public List<AddDamageInfo> reduceGeDangDamagePercent = new List<AddDamageInfo>();//格挡减伤百分比

    public List<AddDamageInfo> reflectDamagePercent = new List<AddDamageInfo>();//反伤百分比
	public List<AddDamageInfo> reflectDamageFix = new List<AddDamageInfo>();//反伤固定值

    // 武器buf附魔元素
    public int[] magicELements = new int[(int)(MagicElementType.MAX)];

    //属强
	public CrypticInt32[] magicElementsAttack = new CrypticInt32[(int)(MagicElementType.MAX)];
	//属抗
	public CrypticInt32[] magicElementsDefence = new CrypticInt32[(int)(MagicElementType.MAX)];
	//异抗,13种
	public CrypticInt32[] abnormalResists = new CrypticInt32[(int)BeAbilityType.CURSE - (int)BeAbilityType.FLASH + 1];
    //抗魔值
    public CrypticInt32 resistMagic;

    private long totalDamage = 0;
    private bool m_NeedChangeSta = true;

    public BattleData()
    {
        hpScale = GlobalLogic.VALUE_1000;
    }

    //计算后的
    //随机[-v% v%] * value
    private double RandmonValue(double value, int valueRange)
    {
        if (Global.Settings.isDebug && Global.Settings.damageNoRange)
        {
            return value;
        }
        else
        {
            // int rv = FrameRandom.InRange(0, valueRange * GlobalLogic.VALUE_2) - valueRange;
            // value *= (GlobalLogic.VALUE_1 + rv / (double)(GlobalLogic.VALUE_100));

            return value;
        }
        
    }

    public void AddDamage(int damage, BeEntity attacker, BeEntity owner)
    {
        totalDamage += damage;
        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.BattleDataUpdate);
    }

    public long GetTotalDamage()
    {
        return totalDamage;
    }


    public double _baseSta
    {
        get
        {
            return baseSta / (double)(GlobalLogic.VALUE_1000);
        }
    }
    public double _baseAtk
    {
        get
        {
            return baseAtk / (double)(GlobalLogic.VALUE_1000);
        }
    }
    public double _baseInt
    {
        get
        {
            return baseInt / (double)(GlobalLogic.VALUE_1000);
        }
    }
    public double _baseSpr
    {
        get
        {
            return baseSpr / (double)(GlobalLogic.VALUE_1000);
        }
    }
    public double _baseIndependence
    {
        get
        {
            return (baseIndependence + ingoreIndependence) / (double)(GlobalLogic.VALUE_1000);
        }
    }


    //buff改变sta的情况
    public int sta
    {
        get
        {
            return baseSta;
        }
        set
        {
            //体力是否需要改变
            if (!m_NeedChangeSta)
            {
                return;
            }

            baseSta = value;
            double curHpRate = hp / (double)maxHp;

            //自动计算hp的变化
            maxHp = fMaxHp;
            hp = IntMath.Double2Int(curHpRate * maxHp);

            //更新hp恢复
            hpRecover = fHpRecoer;
            if(owner != null)
            {
                owner.GetEntityData().ChangeMaxHpByResist();
            }
        }
    }

    //buff改变spr的情况
    public int spr
    {
        get
        {
            return baseSpr;
        }
        set
        {
            baseSpr = value;
            double curMpRate = mp / (double)maxMp;
            //自动计算mp的变化
            maxMp = fMaxMp;
            mp = IntMath.Double2Int(curMpRate * maxMp);

            mpRecover = fMpRecover;
        }
    }


    private const long kFloatBit = 100;

    public int displayAttack  //显示用物理攻击力
    {
        get
        {
            double att = (attack * (GlobalLogic.VALUE_1 + _baseAtk / (double)(GlobalLogic.VALUE_250)) * (GlobalLogic.VALUE_1 + attackAddRate / (double)(GlobalLogic.VALUE_1000)) + ignoreDefAttackAdd);
            //return (int)Math.Round(att, MidpointRounding.AwayFromZero);
            return IntMath.Float2IntWithFixed(att, GlobalLogic.VALUE_1, kFloatBit, MidpointRounding.AwayFromZero);
        }
    }

    public double fAttack
    {

        get
        {
			return (RandmonValue(attack, GlobalLogic.VALUE_10) * (GlobalLogic.VALUE_1 + _baseAtk / (double)(GlobalLogic.VALUE_250)) * (GlobalLogic.VALUE_1 + attackAddRate / (double)(GlobalLogic.VALUE_1000)) + ignoreDefAttackAdd);
        }
    }

    public  int displayMagicAttack //显示用魔法攻击力
    {
        get
        {
            double matt = (magicAttack * (GlobalLogic.VALUE_1 + _baseInt / (double)(GlobalLogic.VALUE_250)) * (GlobalLogic.VALUE_1 + magicAttackAddRate / (double)(GlobalLogic.VALUE_1000)) + ignoreDefMagicAttackAdd);

            return IntMath.Float2IntWithFixed(matt, GlobalLogic.VALUE_1, kFloatBit, MidpointRounding.AwayFromZero);
            //return (int)Math.Round(matt, MidpointRounding.AwayFromZero);
        }
    }

    public double fMagicAttack
    {
        get
        {
			return (RandmonValue(magicAttack, GlobalLogic.VALUE_10) * (GlobalLogic.VALUE_1 + _baseInt / (double)(GlobalLogic.VALUE_250)) * (GlobalLogic.VALUE_1 + magicAttackAddRate / (double)(GlobalLogic.VALUE_1000)) + ignoreDefMagicAttackAdd);
        }
    }

    public int fMaxHp           //显示用HP
    {
        get
        {
            double staProtectedValue = _baseSta;
            if (staProtectedValue <= -250d)
            {
                staProtectedValue = -240d;
            }
            double tmp = _maxHp * (GlobalLogic.VALUE_1 + _baseSta / (double)(GlobalLogic.VALUE_250)) * (hpScale/(double)(GlobalLogic.VALUE_1000));
            return IntMath.Float2IntWithFixed(tmp, GlobalLogic.VALUE_1, kFloatBit, MidpointRounding.AwayFromZero);
            //return (int)Math.Round(tmp, MidpointRounding.AwayFromZero);
        }
    }

    public int fMaxMp           //显示用MP
    {
        get
        {
            double sprProtectedValue = _baseSpr;
            if (sprProtectedValue <= -250d)
            {
                sprProtectedValue = -240d;
            }
            double tmp = _maxMp * (GlobalLogic.VALUE_1 + _baseSpr / (double)(GlobalLogic.VALUE_250));
            return   IntMath.Float2IntWithFixed(tmp, GlobalLogic.VALUE_1, kFloatBit, MidpointRounding.AwayFromZero);
            
            //return (int)Math.Round(tmp, MidpointRounding.AwayFromZero);
        }
    }

    public int fDefence         //显示用物理防御
    {
        get
        {
			double tmp = (defence + _baseSta * GlobalLogic.VALUE_5) * (GlobalLogic.VALUE_1 + defenceAddRate/(double)(GlobalLogic.VALUE_1000));
            int ret =  IntMath.Float2IntWithFixed(tmp, GlobalLogic.VALUE_1, kFloatBit, MidpointRounding.AwayFromZero);
            return Math.Max(0, ret);
            //return (int)Math.Round(tmp, MidpointRounding.AwayFromZero);
        }
    }

    public int fMagicDefence    //显示用魔法防御
    {
        get
        {
			double tmp = (magicDefence + _baseSpr * GlobalLogic.VALUE_5) * (GlobalLogic.VALUE_1 + magicDefenceAddRate/(double)(GlobalLogic.VALUE_1000));
            int ret = IntMath.Float2IntWithFixed(tmp, GlobalLogic.VALUE_1, kFloatBit, MidpointRounding.AwayFromZero);
            return Math.Max(0, ret);
            //return (int)Math.Round(tmp, MidpointRounding.AwayFromZero);
        }
    }

    public int fHpRecoer        //显示用HP回复
    {
        get
        {
            double tmp = _hpRecover * (GlobalLogic.VALUE_1 + _baseSta / (double)(GlobalLogic.VALUE_250));
            return IntMath.Float2IntWithFixed(tmp, GlobalLogic.VALUE_1, kFloatBit, MidpointRounding.AwayFromZero);
            //return (int)Math.Round(tmp, MidpointRounding.AwayFromZero);
        }
    }

    public int fMpRecover       //显示用MP回复
    {
        get
        {
            double tmp = _mpRecover * (GlobalLogic.VALUE_1 + _baseSpr / (double)(GlobalLogic.VALUE_250));
            return IntMath.Float2IntWithFixed(tmp, GlobalLogic.VALUE_1, kFloatBit,  MidpointRounding.AwayFromZero);
            //return (int)Math.Round(tmp, MidpointRounding.AwayFromZero);
        }
    }	

    public double fAbnormalResist
    {
        get
        {
            return abnormalResist / 1.2d;
        }
    }


	public void ChangeMaxHP(int value)
	{
		double curHPRate = hp / (double)maxHp;

		_maxHp += value;
		maxHp = this.fMaxHp;

		hp = IntMath.Double2Int(curHPRate * maxHp);
    }

	public void ChangeMaxMP(int value)
	{
		double curMPRate = mp / (double)maxMp;

		_maxMp += value;
		maxMp = this.fMaxMp;

		mp = IntMath.Double2Int(curMPRate * maxMp);
	}

    public void DebugPrint()
    {
        Utility.PrintType(typeof(BattleData), this);
    }

	public string GetDebugPrint()
	{
		return Utility.GetTypeInfoString(typeof(BattleData), this);
	}

    /// <summary>
    /// 设置Sta是否需要改变标志
    /// </summary>
    public void SetNeedChangeSta(bool flag)
    {
        m_NeedChangeSta = flag;
    }
    
    /// <summary>
    /// 设置Sta是否需要改变标志
    /// </summary>
    public bool GetNeedChangeSta()
    {
	    return m_NeedChangeSta;
    }

    public void RefreshMpInfo()
    {
        double curMpRate = mp / (double)maxMp;
        //自动计算mp的变化
        maxMp = fMaxMp;
        mp = IntMath.Double2Int(curMpRate * maxMp);

        mpRecover = fMpRecover;
    }

	#region SETGET
	public int GetValue(AttributeType attributeType){
		int value=0;
		switch(attributeType)
		{
		case AttributeType.maxHp:
			value = this._maxHp;
			break;
		case AttributeType.maxMp:
			value = this._maxMp;
			break;
		case AttributeType.hpRecover:
			value = this._hpRecover;
			break;
		case AttributeType.mpRecover:
			value = this._mpRecover;
			break;
		case AttributeType.attack:
			value = this.attack;
			break;
		case AttributeType.magicAttack:
			value = this.magicAttack;
			break;
		case AttributeType.defence:
			value = this.defence;
			break;
		case AttributeType.magicDefence:
			value = this.magicDefence;
			break;
		case AttributeType.attackSpeed:
			value = this.attackSpeed;
			break;
		case AttributeType.spellSpeed:
			value = this.spellSpeed;
			break;
		case AttributeType.moveSpeed:
			value = this.moveSpeed;
			break;
		case AttributeType.ciriticalAttack:
			value = this.ciriticalAttack;
			break;
		case AttributeType.ciriticalMagicAttack:
			value = this.ciriticalMagicAttack;
			break;
        case AttributeType.ingnoreDefRate:
            value = this.ingoreDefRate;
            break;
        case AttributeType.ingnoreMagicDefRate:
            value = this.ingoreMagicDefRate;
            break;
        case AttributeType.dex:
		value = this.dex;
			break;
		case AttributeType.dodge:
			value = this.dodge;
			break;
		case AttributeType.frozen:
			value = this.frozen;
			break;
		case AttributeType.hard:
			value = this.hard;
			break;
		case AttributeType.abnormalResist:
			value = this.abnormalResist;
			break;
		case AttributeType.criticalPercent:
			value = this.criticalPercent;
			break;
		case AttributeType.cdReduceRate:
			value = this.cdReduceRate;
			break;
		case AttributeType.cdReduceRateMagic:
			value = this.cdReduceRateMagic;
			break;
		case AttributeType.mpCostReduceRate:
			value = this.mpCostReduceRate;
			break;
		case AttributeType.mpCostReduceRateMagic:
			value = this.mpCostReduceRateMagic;
			break;
		case AttributeType.attackAddRate:
			value = this.attackAddRate;
			break;
		case AttributeType.magicAttackAddRate:
			value = this.magicAttackAddRate;
			break;
		case AttributeType.ignoreDefAttackAdd:
			value = this.ignoreDefAttackAdd;
			break;
		case AttributeType.ignoreDefMagicAttackAdd:
			value = this.ignoreDefMagicAttackAdd;
			break;
		case AttributeType.attackReduceRate:
			value = this.attackReduceRate;
			break;
		case AttributeType.magicAttackReduceRate:
			value = this.magicAttackReduceRate;
			break;
		case AttributeType.attackReduceFix:
			value = this.attackReduceFix;
			break;
		case AttributeType.magicAttackReduceFix:
			value = this.magicAttackReduceFix;
			break;
		case AttributeType.defenceAddRate:
			value = this.defenceAddRate;
			break;
		case AttributeType.magicDefenceAddRate:
			value = this.magicDefenceAddRate;
			break;
		case AttributeType.baseAtk:
			value = this.baseAtk;
			break;
		case AttributeType.baseInt:
			value = this.baseInt;
			break;
		case AttributeType.baseSta:
			value = this.baseSta;
			break;
		case AttributeType.baseSpr:
			value = this.baseSpr;
			break;
       case AttributeType.baseIndependence:
            value = this.baseIndependence;
            break;
		case AttributeType.hpGrow:
			value = this.hpGrow;
			break;
		case AttributeType.mpGrow:
			value = this.mpGrow;
			break;
		case AttributeType.atkGrow:
			value = this.atkGrow;
			break;
		case AttributeType.intGrow:
			value = this.intGrow;
			break;
		case AttributeType.staGrow:
			value = this.staGrow;
			break;
		case AttributeType.sprGrow:
			value = this.sprGrow;
			break;
		case AttributeType.hardGrow:
			value = this.hardGrow;
			break;
		case AttributeType.sta:
			value = this.sta;
			break;
		case AttributeType.spr:
			value = this.spr;
			break;
        case AttributeType.resistMagic:
            value = this.resistMagic;
            break;
        case AttributeType.ingoreIndependence:
            value = this.ingoreIndependence;
            break;
        }
		return value;
	}

	public void SetValue(AttributeType attributeType, int value, bool add=false){
		switch(attributeType)
		{
		case AttributeType.maxHp:
			if(add)
				this._maxHp += value;
			else
				this._maxHp = value;
			break;
		case AttributeType.maxMp:
			if(add)
				this._maxMp += value;
			else
				this._maxMp = value;
			break;
		case AttributeType.hpRecover:
			if(add)
				this._hpRecover += value;
			else
				this._hpRecover = value;
			break;
		case AttributeType.mpRecover:
			if(add)
				this._mpRecover += value;
			else
				this._mpRecover = value;
			break;
		case AttributeType.attack:
			if(add)
				this.attack += value;
			else
				this.attack = value;
			break;
		case AttributeType.magicAttack:
			if(add)
				this.magicAttack += value;
			else
				this.magicAttack = value;
			break;
		case AttributeType.defence:
			if(add)
				this.defence += value;
			else
				this.defence = value;
			break;
		case AttributeType.magicDefence:
			if(add)
				this.magicDefence += value;
			else
				this.magicDefence = value;
			break;
		case AttributeType.attackSpeed:
			if(add)
				this.attackSpeed += value;
			else
				this.attackSpeed = value;
			break;
		case AttributeType.spellSpeed:
			if(add)
				this.spellSpeed += value;
			else
				this.spellSpeed = value;
			break;
		case AttributeType.moveSpeed:
			if(add)
				this.moveSpeed += value;
			else
				this.moveSpeed = value;
			break;
		case AttributeType.ciriticalAttack:
			if(add)
				this.ciriticalAttack += value;
			else
				this.ciriticalAttack = value;
			break;
		case AttributeType.ciriticalMagicAttack:
			if(add)
				this.ciriticalMagicAttack += value;
			else
				this.ciriticalMagicAttack = value;
			break;
		case AttributeType.dex:
			if(add)
				this.dex += value;
			else
				this.dex = value;
			break;
		case AttributeType.dodge:
			if(add)
				this.dodge += value;
			else
				this.dodge = value;
			break;
		case AttributeType.frozen:
			if(add)
				this.frozen += value;
			else
				this.frozen = value;
			break;
		case AttributeType.hard:
			if(add)
				this.hard += value;
			else
				this.hard = value;
			break;
		case AttributeType.abnormalResist:
			if(add)
				this.abnormalResist += value;
			else
				this.abnormalResist = value;
			break;
		case AttributeType.criticalPercent:
			if(add)
				this.criticalPercent += value;
			else
				this.criticalPercent = value;
			break;
		case AttributeType.cdReduceRate:
			if(add)
				this.cdReduceRate += value;
			else
				this.cdReduceRate = value;
			break;
		case AttributeType.cdReduceRateMagic:
			if(add)
				this.cdReduceRateMagic += value;
			else
				this.cdReduceRateMagic = value;
			break;
		case AttributeType.mpCostReduceRate:
			if(add)
				this.mpCostReduceRate += value;
			else
				this.mpCostReduceRate = value;
			break;
		case AttributeType.mpCostReduceRateMagic:
			if(add)
				this.mpCostReduceRateMagic += value;
			else
				this.mpCostReduceRateMagic = value;
			break;
		case AttributeType.attackAddRate:
			if(add)
				this.attackAddRate += value;
			else
				this.attackAddRate = value;
			break;
		case AttributeType.magicAttackAddRate:
			if(add)
				this.magicAttackAddRate += value;
			else
				this.magicAttackAddRate = value;
			break;
		case AttributeType.ignoreDefAttackAdd:
			if(add)
				this.ignoreDefAttackAdd += value;
			else
				this.ignoreDefAttackAdd = value;
			break;
		case AttributeType.ignoreDefMagicAttackAdd:
			if(add)
				this.ignoreDefMagicAttackAdd += value;
			else
				this.ignoreDefMagicAttackAdd = value;
			break;
		case AttributeType.attackReduceRate:
			if(add)
				this.attackReduceRate += value;
			else
				this.attackReduceRate = value;
			break;
		case AttributeType.magicAttackReduceRate:
			if(add)
				this.magicAttackReduceRate += value;
			else
				this.magicAttackReduceRate = value;
			break;
        case AttributeType.ingnoreDefRate:
            if (add)
                this.ingoreDefRate += value;
            else
                this.ingoreDefRate = value;
            break;
        case AttributeType.ingnoreMagicDefRate:
            if (add)
                this.ingoreMagicDefRate += value;
            else
                this.ingoreMagicDefRate = value;
            break;
        case AttributeType.attackReduceFix:
		    if(add)
			    this.attackReduceFix += value;
		    else
			    this.attackReduceFix = value;
		    break;
		case AttributeType.magicAttackReduceFix:
			if(add)
				this.magicAttackReduceFix += value;
			else
				this.magicAttackReduceFix = value;
			break;
		case AttributeType.defenceAddRate:
			if(add)
				this.defenceAddRate += value;
			else
				this.defenceAddRate = value;
			break;
		case AttributeType.magicDefenceAddRate:
			if(add)
				this.magicDefenceAddRate += value;
			else
				this.magicDefenceAddRate = value;
			break;
		case AttributeType.baseAtk:
                {
                    int lastAtk = baseAtk.ToInt();
                    if (add)
                        this.baseAtk += value;
                    else
                        this.baseAtk = value;
                    if (owner != null && lastAtk != baseAtk)
                    {
                        //owner.TriggerEvent(BeEventType.onAttrChange, new object[] { AttributeType.baseAtk });
                        owner.TriggerEventNew(BeEventType.onAttrChange, new EventParam(){m_Int = (int) AttributeType.baseAtk});
                    }
                }
			break;
		case AttributeType.baseInt:
                {
                    int lastInt = baseInt.ToInt();
                    if (add)
                        this.baseInt += value;
                    else
                        this.baseInt = value;
                    if (owner != null && lastInt != baseInt)
                    {
                        //owner.TriggerEvent(BeEventType.onAttrChange, new object[] { AttributeType.baseInt });
                        owner.TriggerEventNew(BeEventType.onAttrChange, new EventParam(){m_Int = (int) AttributeType.baseInt});
                    }
                }
			break;
        case AttributeType.baseIndependence:
            {
                    int lastInt = baseIndependence.ToInt();
                    if (add)
                        this.baseIndependence += value;
                    else
                        this.baseIndependence = value;
                    if (owner != null && lastInt != baseInt)
                    {
                        //owner.TriggerEvent(BeEventType.onAttrChange, new object[] { AttributeType.baseIndependence });
                        owner.TriggerEventNew(BeEventType.onAttrChange, new EventParam(){m_Int = (int) AttributeType.baseIndependence});
                    }
                }
                break;
         case AttributeType.ingoreIndependence:
                {
                    int lastInt = ingoreIndependence.ToInt();
                    if (add)
                        this.ingoreIndependence += value;
                    else
                        this.ingoreIndependence = value;
                    if (owner != null && lastInt != baseInt)
                    {
                        //owner.TriggerEvent(BeEventType.onAttrChange, new object[] { AttributeType.ingoreIndependence });
                        owner.TriggerEventNew(BeEventType.onAttrChange, new EventParam(){m_Int = (int) AttributeType.ingoreIndependence});
                    }
                }
                break;
            case AttributeType.baseSta:
                {
                    int lastSta = baseSta.ToInt();
                    if (add)
                        this.baseSta += value;
                    else
                        this.baseSta = value;
                    if (owner != null && lastSta != baseSta)
                    {
                        //owner.TriggerEvent(BeEventType.onAttrChange, new object[] { AttributeType.baseSta });
                        owner.TriggerEventNew(BeEventType.onAttrChange, new EventParam(){m_Int = (int) AttributeType.baseSta});
                    }
                }
			break;
		case AttributeType.baseSpr:
                {
                    int lastSpr = baseSpr.ToInt();
                    if (add)
                        this.baseSpr += value;
                    else
                        this.baseSpr = value;
                    if (owner != null && lastSpr != baseSpr)
                    {
                        //owner.TriggerEvent(BeEventType.onAttrChange, new object[] { AttributeType.baseSpr });
                        owner.TriggerEventNew(BeEventType.onAttrChange, new EventParam(){m_Int = (int) AttributeType.baseSpr});
                    }
                }
			break;
		case AttributeType.hpGrow:
			if(add)
				this.hpGrow += value;
			else
				this.hpGrow = value;
			break;
		case AttributeType.mpGrow:
			if(add)
				this.mpGrow += value;
			else
				this.mpGrow = value;
			break;
		case AttributeType.atkGrow:
			if(add)
				this.atkGrow += value;
			else
				this.atkGrow = value;
			break;
		case AttributeType.intGrow:
			if(add)
				this.intGrow += value;
			else
				this.intGrow = value;
			break;
		case AttributeType.staGrow:
			if(add)
				this.staGrow += value;
			else
				this.staGrow = value;
			break;
		case AttributeType.sprGrow:
			if(add)
				this.sprGrow += value;
			else
				this.sprGrow = value;
			break;
		case AttributeType.hardGrow:
			if(add)
				this.hardGrow += value;
			else
				this.hardGrow = value;
			break;
		case AttributeType.sta:
			if (add)
				this.sta += value;
			else
				this.sta = value;
			break;
		case AttributeType.spr:
			if (add)
				this.spr += value;
			else
				this.spr = value;
			break;
        case AttributeType.resistMagic:
            if (add)
                this.resistMagic += value;
            else
                this.resistMagic = value;
            break;
        }

	}
	#endregion

 }


public class BeEntityData
{
	FrameRandomImp FrameRandom
	{
		get{
			return owner.FrameRandom;
		}
	}

    public string name;            //名字
	public CrypticInt32 level;               //等级
    public int professtion;         //职业
    public int battleDataID;        //战斗包ID
	public int jobAttribute;		//职业属性0物理，1魔法
    public VInt weight { get; set; }              //体重
    public VInt backupWeight;
    public int type { get; set; }       //类型
	public Dictionary<int, CrypticInt32> skillLevelInfo; //技能等级信息
    public bool skillMonsterCanBeAttack;//技能实现的怪物是否能被攻击
    public bool autoFightNeedAttackFirst; //自动战斗是否要优先打这个怪
    public int exp { get; set; }        // 怪物死亡获得经验
	public double hpScale = (double)(GlobalLogic.VALUE_1);
	public int camp = 0;

    public BattleData battleData = new BattleData();

    public int enhancedRadius;//Y轴的宽度半径（用于被击判断）

    //无色晶体的数量
    //TODO 数据对接
    public int crystalNum = int.MaxValue;

    public int normalAttackID { get; set; }
    public int jumpAttackID { get; set; }
    public int runAttackID { get; set; }
    public int getupIDRand { get; set; }
    public int getupID { get; set; }

    public int hitIDRand { get; set; }

    public int hitID { get; set; }

    public int jumpAttackCount;
	public int deafaultWeaponTag;

    //public float criticalDamageFactor = 1.5f;
	public CrypticInt32 criticalDamageFactor = GlobalLogic.VALUE_1500;

    public BeEntity _owner;
	public BeEntity owner
	{
		set{
			_owner = value;
			battleData.owner = value;
		}

		get
		{
			return _owner;
		}
	}

    public List<ItemProperty> itemProperties = null;
    public List<ItemProperty> backupWeapons = new List<ItemProperty>();
    public ItemProperty currentWeapon = null;

    public bool isMonster = false;
    public bool isSummonMonster = false;
	public bool isPet = false;
	public bool isSpecialAPC = false;
	public bool isShowSkillSpeech = false;

	public CrypticInt32 walkAnimationSpeedPercent = GlobalLogic.VALUE_100;

    public int monsterID = 0;
	public float overHeadHeight = 0f;
	public float buffOriginHeight = 0f;
	public ProtoTable.UnitTable monsterData = null;
	public int simpleMonsterID = 0;
	public MonsterIDData monsterIDData = null;

	public bool adjustBalance = false;
	public int height = 0;

    public int groupID = 0;//怪物组ID，0表示没有组，被击时同组怪物进入战斗。

    public int GetCrystalNum()
    {
        return crystalNum;
    }

    public void SetCrystalNum(int num)
    {
        crystalNum = num;
    }

    public void ModifyCrystalessNum(int num)
    {
        crystalNum += num;
        crystalNum = Mathf.Max(0, crystalNum);
    }


    public static DisplayAttribute GetMonsterAttributeForDisplay(BeEntityData attribute)
    {
        var bd = attribute.battleData;

        DisplayAttribute da = new DisplayAttribute
        {
            maxHp = bd.fMaxHp,
            maxMp = bd.fMaxMp,
            hpRecover = bd.fHpRecoer,
            mpRecover = bd.fMpRecover,
            baseSta = IntMath.Double2Int(bd._baseSta),
            baseAtk = IntMath.Double2Int(bd._baseAtk),
            baseInt = IntMath.Double2Int(bd._baseInt),
            baseSpr = IntMath.Double2Int(bd._baseSpr),
            baseIndependence = IntMath.Double2Int(bd._baseIndependence),
            attack = bd.displayAttack,
            magicAttack = bd.displayMagicAttack,
            defence = bd.fDefence,
            magicDefence = bd.fMagicDefence,
            attackSpeed = (bd.attackSpeed) / (float)(GlobalLogic.VALUE_1000) * GlobalLogic.VALUE_100,
            spellSpeed = (bd.spellSpeed) / (float)(GlobalLogic.VALUE_1000) * GlobalLogic.VALUE_100,
            moveSpeed = (bd.moveSpeed) / (float)(GlobalLogic.VALUE_1000) * GlobalLogic.VALUE_100,
            ciriticalAttack = (bd.ciriticalAttack) / (float)(GlobalLogic.VALUE_1000) * GlobalLogic.VALUE_100,
            ciriticalMagicAttack = (bd.ciriticalMagicAttack) / (float)(GlobalLogic.VALUE_1000) * GlobalLogic.VALUE_100,
            dex = (bd.dex) / (float)(GlobalLogic.VALUE_1000) * GlobalLogic.VALUE_100,
            dodge = (bd.dodge) / (float)(GlobalLogic.VALUE_1000) * GlobalLogic.VALUE_100,
            frozen = bd.frozen,
            hard = bd.hard
        };
		return da;
	}

    public static DisplayAttribute GetActorAttributeForDisplay(BeEntityData attribute)
    {
        if (attribute == null)
            return null;
        int occuID = attribute.professtion;

        var jobData = TableManager.GetInstance().GetTableItem<ProtoTable.JobTable>(occuID);
        if (jobData == null)
        {
            Logger.LogErrorFormat("找不到职业ID为{0}的职业", occuID);
            return null;
        }

        var fightData = TableManager.GetInstance().GetTableItem<ProtoTable.FightPackageTable>(jobData.FightID);
        var bd = attribute.battleData;

        DisplayAttribute da = new DisplayAttribute
        {
            maxHp = attribute.GetMaxHP(),
            maxMp = attribute.GetMaxMP(),
            hpRecover = (int)bd.fHpRecoer,
            mpRecover = (int)bd.fMpRecover,
            baseSta = (int)bd._baseSta,
            baseAtk = (int)bd._baseAtk,
            baseInt = (int)bd._baseInt,
            baseSpr = (int)bd._baseSpr,
            baseIndependence = (int)bd._baseIndependence,
            attack = (int)bd.displayAttack,
            magicAttack = (int)bd.displayMagicAttack,
            defence = (int)bd.fDefence,
            magicDefence = (int)bd.fMagicDefence,
            attackSpeed = (bd.attackSpeed - fightData.AttackSpeed) / (float)(GlobalLogic.VALUE_1000) * GlobalLogic.VALUE_100,
            spellSpeed = (bd.spellSpeed - fightData.SpellSpeed) / (float)(GlobalLogic.VALUE_1000) * GlobalLogic.VALUE_100,
            moveSpeed = (bd.moveSpeed - fightData.MoveSpeed) / (float)(GlobalLogic.VALUE_1000) * GlobalLogic.VALUE_100,
            ciriticalAttack = (bd.ciriticalAttack - fightData.PhysicalCritical) / (float)(GlobalLogic.VALUE_1000) * GlobalLogic.VALUE_100,
            ciriticalMagicAttack = (bd.ciriticalMagicAttack - fightData.MagicCritical) / (float)(GlobalLogic.VALUE_1000) * GlobalLogic.VALUE_100,
            dex = (bd.dex - fightData.HitRate) / (float)(GlobalLogic.VALUE_1000) * GlobalLogic.VALUE_100,
            dodge = (bd.dodge - fightData.MissRate) / (float)(GlobalLogic.VALUE_1000) * GlobalLogic.VALUE_100,
            frozen = bd.frozen,
            hard = bd.hard,

            resistMagic = bd.resistMagic,

            lightAttack = bd.magicElementsAttack[1],
            fireAttack = bd.magicElementsAttack[2],
            iceAttack = bd.magicElementsAttack[3],
            darkAttack = bd.magicElementsAttack[4],
            lightDefence = bd.magicElementsDefence[1],
            fireDefence = bd.magicElementsDefence[2],
            iceDefence = bd.magicElementsDefence[3],
            darkDefence = bd.magicElementsDefence[4]
        };

        BeEntityData attribute2 = new BeEntityData(attribute.GetLevel(), attribute.battleDataID, null);

		attribute2.InitBattleData();
		attribute2.CalculateLevelGrow(attribute.GetLevel());
		attribute2.PostInit();


		for (int i = 0; i < (int)AttributeType.hpGrow; ++i)
		{
			AttributeType at = (AttributeType)(i);

			int curValue = attribute.battleData.GetValue(at); //attribute.GetValue(at, bd.GetType(), bd);
			int tableValue = attribute2.battleData.GetValue(at);//attribute.GetValue(at, bd.GetType(), attribute2.battleData);
			//Logger.LogErrorFormat("attribute:{0} curValue:{1} fightValue:{2}", at, curValue, tableValue);
		

			string typeName = Global.GetAttributeString(at);//Enum.GetName(typeof(AttributeType), at);
			if (da.attachValue.ContainsKey(typeName))
			{
				da.attachValue[typeName] = curValue - tableValue;
			}
		}
		

        //attribute.CalculateAttachPropery(equipAttach, da);
        return da;
    }

    /*******************************************************************/

    public static BeEntityData GetActorAttribute(int level, int fightID, List<ItemProperty> equip, Dictionary<int, int> skillLevelInfo, List<int> passiveBuffIDs = null,List<ItemProperty> sideEquips = null,BeEntity owner = null)
    {
        BeEntityData attribute = new BeEntityData(level, fightID, skillLevelInfo);
		
        attribute.InitBattleData();
        attribute.CalculateLevelGrow(level);
        attribute.CalculatePassiveBuffIDs(passiveBuffIDs);
        attribute.CalculateEquipmentAdd(equip,sideEquips);

        attribute.PostInit(owner);


        //attribute.battleData.DebugPrint();
        return attribute;
    }
    /*******************************************************************/

    public BeEntityData(int level, int battledataid, Dictionary<int, int> info)
    {
        this.level = level;
        battleDataID = battledataid;
        
		if (info == null)
		{
			return;
			//Logger.LogErrorFormat("info is nULL!!!!!!");
		}

		skillLevelInfo = new Dictionary<int, CrypticInt32>();
		Dictionary<int, int>.Enumerator enumerator = info.GetEnumerator();

		while(enumerator.MoveNext())
		{
			int skillID = (int)enumerator.Current.Key;//skillinfo.Key;
			int skillLevel = (int)enumerator.Current.Value;//skillinfo.Value;

			if (!skillLevelInfo.ContainsKey(skillID))
				skillLevelInfo.Add(skillID, skillLevel);
		}

		//skillLevelInfo = info;
    }

    public void InitBattleData()
    {
        if (battleDataID <= 0)
            return;

        //读取初始值
        var data = TableManager.GetInstance().GetTableItem<ProtoTable.FightPackageTable>(battleDataID);
        if (data != null)
        {
            battleData._maxHp = data.HP;
            battleData._maxMp = data.MP;
 
            battleData._hpRecover = data.HPRecover;
            battleData._mpRecover = data.MPRecover;
            battleData.attack = data.PhysicAttack;
            battleData.magicAttack = data.MagicAttack;
            battleData.defence = data.PhysicDefence;
            battleData.magicDefence = data.MagicDefence;
            battleData.ciriticalAttack = data.PhysicalCritical;
            battleData.ciriticalMagicAttack = data.MagicCritical;
            battleData.frozen = data.StarkValue;
            battleData.hard = data.HardValue;
            battleData.moveSpeed = data.MoveSpeed;
            battleData.dex = data.HitRate; 
            battleData.dodge = data.MissRate;
            battleData.attackSpeed = data.AttackSpeed;
            battleData.spellSpeed = data.SpellSpeed;

            battleData.baseAtk = data.Power;
            battleData.baseInt = data.Intellect;
            battleData.baseSpr = data.Spirit;
            battleData.baseSta = data.Streangth;

            battleData.hpGrow = data.HPLevel;
            battleData.mpGrow = data.MPLevel;
            battleData.atkGrow = data.PowerLevel;
            battleData.intGrow = data.IntellectLevel;
            battleData.sprGrow = data.SpiritLevel;
            battleData.staGrow = data.StrengthLevel;
			battleData.mpRecoverGrow = data.MPRecoverLevel;

            battleData.magicElementsAttack[(int)MagicElementType.LIGHT] = data.LightAttack;
            battleData.magicElementsAttack[(int)MagicElementType.FIRE] = data.FireAttack;
            battleData.magicElementsAttack[(int)MagicElementType.ICE] = data.IceAttack;
            battleData.magicElementsAttack[(int)MagicElementType.DARK] = data.DarkAttack;

            battleData.magicElementsDefence[(int)MagicElementType.LIGHT] = data.LightDefence;
            battleData.magicElementsDefence[(int)MagicElementType.FIRE] = data.FireDefence;
            battleData.magicElementsDefence[(int)MagicElementType.ICE] = data.IceDefence;
            battleData.magicElementsDefence[(int)MagicElementType.DARK] = data.DarkDefence;

            battleData.abnormalResist = data.AbormalResist;
            SetAbnormalResists(BeUtility.ParseAbnormalResistString(data.AbormalResists));

        }
	
    }



    public int GetAttributeValue(AttributeType at)
    {
		return battleData.GetValue(at);
    }

    public void SetAttributeValue(AttributeType at, int value, bool add=false)
    {
		battleData.SetValue(at, value, add);
    }
    
    public void PostInit(BeEntity entity = null)
    {
        if(entity != null)
            owner = entity;
        
        battleData.maxHp = battleData.fMaxHp;
        battleData.maxMp = battleData.fMaxMp;

        battleData.hp = battleData.maxHp;
        battleData.mp = battleData.maxMp;

        battleData.hpRecover = battleData.fHpRecoer;
        battleData.mpRecover = battleData.fMpRecover;

        //根据抗魔值修改最大血量
        ChangeMaxHpByResist();
    }

    public void CalculateLevelGrow(int level)
    {
        if (level > GlobalLogic.VALUE_1)
        {
            battleData._maxHp += (level - GlobalLogic.VALUE_1) * battleData.hpGrow;
            battleData._maxMp += (level - GlobalLogic.VALUE_1) * battleData.mpGrow;
            battleData.baseAtk += (level - GlobalLogic.VALUE_1) * battleData.atkGrow;
            battleData.baseInt += (level - GlobalLogic.VALUE_1) * battleData.intGrow;
            battleData.baseSpr += (level - GlobalLogic.VALUE_1) * battleData.sprGrow;
            battleData.baseSta += (level - GlobalLogic.VALUE_1) * battleData.staGrow;
            battleData.hard += (level - GlobalLogic.VALUE_1) * battleData.hardGrow;
			battleData._mpRecover += (level - GlobalLogic.VALUE_1) * battleData.mpRecoverGrow;
        }
    }

    public void CalculatePassiveBuffIDs(List<int> passiveBuffIDs)
    {
        if (passiveBuffIDs == null)
            return;

        for(int i=0; i<passiveBuffIDs.Count; ++i)
        {
            if (passiveBuffIDs[i] == 0)
                continue;

            var buffData = TableManager.GetInstance().GetTableItem<ProtoTable.BuffTable>(passiveBuffIDs[i]);
            if (buffData == null)
                return;

            battleData._maxHp += TableManager.GetValueFromUnionCell(buffData.maxHp, 1);
            battleData._maxMp += TableManager.GetValueFromUnionCell(buffData.maxMp, 1);
            battleData.baseAtk += TableManager.GetValueFromUnionCell(buffData.baseAtk, 1);
            battleData.baseInt += TableManager.GetValueFromUnionCell(buffData.baseInt, 1);
            battleData.baseSpr += TableManager.GetValueFromUnionCell(buffData.spr, 1);
            battleData.baseSta += TableManager.GetValueFromUnionCell(buffData.sta, 1);
        }
    }

    void CalculateEquipmentAdd(List<ItemProperty> equipmentsProperty,List<ItemProperty> sideEquipsProperty)
    {
        itemProperties = equipmentsProperty;

        if (equipmentsProperty == null)
            return;
        
		for(int j=0; j<equipmentsProperty.Count; ++j)
		{
            var itemID = equipmentsProperty[j].itemID;
            var itemData = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(itemID);
            if (itemData != null)
            {
                var slot = (EEquipWearSlotType)itemData.SubType;
                if (slot == EEquipWearSlotType.EquipWeapon)
                {
                    currentWeapon = equipmentsProperty[j];
                }
            }
            AddEquipment(equipmentsProperty[j]);
        }

        if (sideEquipsProperty == null)
            return;
        for (int i = 0; i < sideEquipsProperty.Count; i++)
        {
            backupWeapons.Add(sideEquipsProperty[i]);
        }

#if DEBUG_SETTING
       if (Global.Settings.isDebug && Global.Settings.isGiveEquips && Utility.IsStringValid(Global.Settings.switchEquipList))
       {
            var tokens = Global.Settings.switchEquipList.Split(',');
            List<int> equipIDs = new List<int>();
            for (int i = 0; i < tokens.Length; ++i)
            {
                int eid = Convert.ToInt32(tokens[i]);
                ItemData item = GameClient.ItemDataManager.CreateItemDataFromTable(eid);
                ItemProperty ip = item.GetBattleProperty();
                ip.itemID = eid;

                //ItemData item = GameClient.ItemDataManager.CreateItemDataFromTable((int)eid);
                //ItemProperty ip = item.GetBattleProperty(10);
                //ip.itemID = eid;
                backupWeapons.Add(ip);
            }
        }
#else
#endif
    }

    public void AddEquipment(ItemProperty property)
    {
        _DealEquipment(property);
    }

    public void RemoveEquipment(ItemProperty property)
    {
        _DealEquipment(property, false);
    }

    void _DealEquipment(ItemProperty property, bool add=true)
    {
      //  var property = equipmentsProperty[j];

        for (int i = 0; i < (int)AttributeType.hpGrow; ++i)
        {
            AttributeType at = (AttributeType)(i);

            int value = property.GetValue(at);
            if (value != 0)
            {
                if (add)
                    SetAttributeValue(at, value, true);
                else
                    SetAttributeValue(at, -value, true);
            }
        }

        for (int i = 1; i < (int)MagicElementType.MAX; ++i)
        {
            if (property.magicElements == null)
            {
                continue;
            }
            if (property.magicElements[i] > 0)
            {
                if (add)
                    battleData.magicELements[i] += property.magicElements[i];
                else
                    battleData.magicELements[i] -= property.magicElements[i];
            }
                
        }

        for (int i = 1; i < (int)MagicElementType.MAX; ++i)
        {
            if (property.magicElementsAttack[i] != 0)
            {
                if (add)
                    battleData.magicElementsAttack[i] += property.magicElementsAttack[i];
                else
                    battleData.magicElementsAttack[i] -= property.magicElementsAttack[i];
            }
                
        }

        for (int i = 1; i < (int)MagicElementType.MAX; ++i)
        {
            if (property.magicElementsDefence[i] != 0)
            {
                if (add)
                    battleData.magicElementsDefence[i] += property.magicElementsDefence[i];
                else
                    battleData.magicElementsDefence[i] -= property.magicElementsDefence[i];
            }
                
        }

        for (int i = 0; i < (int)Global.ABNORMAL_COUNT; ++i)
        {
            if (property.abnormalResists[i] != 0)
            {
                if (add)
                    battleData.abnormalResists[i] += property.abnormalResists[i];
                else
                    battleData.abnormalResists[i] -= property.abnormalResists[i];
            }
                
        }

        if (property.resistMagic != 0)
        {
            if (add)
                battleData.resistMagic += property.resistMagic;
            else
                battleData.resistMagic -= property.resistMagic;
        }
    }

//     void CalculateAttachPropery(List<ItemProperty> attachProperty, DisplayAttribute displayAttribute)
//     {
//         if (attachProperty == null)
//             return;
// 
// 		for(int k=0; k<attachProperty.Count; ++k)
// 		{
// 			var property = attachProperty[k];
// 			//Type type = typeof(ItemProperty);
// 
// 			for (int i = 0; i < (int)AttributeType.hpGrow; ++i)
// 			{
// 				AttributeType at = (AttributeType)(i);
// 
// 				int value = property.GetValue(at);//GetValue(at, type, property);
// 				if (value != 0)
// 				{
// 					string typeName = Global.GetAttributeString(at);
// 					if (displayAttribute.attachValue.ContainsKey(typeName))
// 					{
// 						displayAttribute.attachValue[typeName] += value;
// 					}
// 				}
// 			}
// 		}
//     }

    private bool _isAvoidDamage(ProtoTable.EffectTable.eAvoidDamageType type, BeEntity target)
    {
        switch (type)
        {
            case ProtoTable.EffectTable.eAvoidDamageType.AV_FACINGAWAY:
                return owner.IsFacingAway(target);
            default:
                return false;
        }
    }


    public AttackResult GetAttackResult(BeEntityData targetData, ProtoTable.EffectTable.eDamageType type, int hurtid)
    {
        var hurtData = TableManager.GetInstance().GetTableItem<ProtoTable.EffectTable>(hurtid);
        
		VRate dexRatio = ((int)battleData.dex - (int)targetData.battleData.dodge);// / (float)(GlobalLogic.VALUE_1000);
         
		VRate ciriticalRatio = (VRate)((type == ProtoTable.EffectTable.eDamageType.PHYSIC ? (int)battleData.ciriticalAttack  : (int)battleData.ciriticalMagicAttack));
        //ciriticalRatio /= (float)(GlobalLogic.VALUE_1000);
		
        //触发效果表附加的暴击率
        int skillLevel = GetSkillLevel(hurtData.SkillID);

        //VRate attachCrtitcalRate = TableManager.GetValueFromUnionCell(hurtData.AttachCritical, skillLevel) / (float)(GlobalLogic.VALUE_10);
		VRate attachCrtitcalRate = TableManager.GetValueFromUnionCell(hurtData.AttachCritical, skillLevel);
		
        ciriticalRatio += attachCrtitcalRate;

        //修改触发效果表中读取到的伤害数值
        //var eventParam = DataStructPool.EventParamPool.Get();
        //eventParam.m_Int = hurtid;
        //eventParam.m_Rate = ciriticalRatio;
        //owner.TriggerEventNew(BeEventType.onReplaceHurtTableCiriticalData, eventParam);
        //ciriticalRatio = eventParam.m_Rate;
        //DataStructPool.EventParamPool.Release(eventParam);

        var param = owner.TriggerEventNew(BeEventType.onReplaceHurtTableCiriticalData, new EventParam(){ m_Int = hurtid, m_Rate = ciriticalRatio });
        ciriticalRatio = param.m_Rate;

        //技能提升的命中率和暴击率
        DealAttachHitRate(ref dexRatio, ref ciriticalRatio, hurtData.SkillID);


        //百分百不miss的情况
        if (!hurtData.IsCanMiss)
        {
            dexRatio = VRate.one;
        }

        // 这里认为AvoidDamage优先级高于`百分百不Miss`
        if (null != hurtData && _isAvoidDamage(hurtData.AvoidDamageType, targetData.owner))
        {
            dexRatio = 0;
        }

		if (FrameRandom.Range1000() <= dexRatio)
        {
			if (FrameRandom.Range1000() <= ciriticalRatio)
            {
                return AttackResult.CRITICAL;
            }
            else
            {
                return AttackResult.NORMAL;
            }
        }
        else
        {
            return AttackResult.MISS;
        }
    }
    //计算僵直
    public int GetHurtFrozenTime(BeEntityData targetData, int hurtid)
    {
        var hurtData = TableManager.GetInstance().GetTableItem<ProtoTable.EffectTable>(hurtid);
        int skillFrozen = 0;
        if (hurtData != null)
        {
            skillFrozen = TableManager.GetValueFromUnionCell(hurtData.HardValue, GetSkillLevel(hurtData.SkillID));

            /*int[] hardValueArray = new int[1];
            hardValueArray[0] = skillFrozen;
            owner.TriggerEvent(BeEventType.onChangeHardValue, new object[] { hurtid, hardValueArray });
            skillFrozen = hardValueArray[0];*/
            
            var ret = owner.TriggerEventNew(BeEventType.onChangeHardValue, new EventParam{m_Int = hurtid, m_Int2 = skillFrozen});
            skillFrozen = ret.m_Int2;
        }

        DealAttachFroze(ref skillFrozen, hurtData.SkillID);

		VFactor f = new VFactor(TableManager.instance.gst.frozenPercent,GlobalLogic.VALUE_10000);
        int frozenTime = (battleData.frozen - targetData.battleData.hard + skillFrozen) * f; 
		//Utility.I2FloatBy10000(TableManager.instance.gst.frozenPercent);
        frozenTime = Mathf.Max(0, frozenTime);
        //Logger.Log("GetHurtFrozenTime:" + frozenTime);

        return frozenTime;
    }

    public VInt GetFrozenDisMax(int hurtid)
    {
        var hurtData = TableManager.GetInstance().GetTableItem<ProtoTable.EffectTable>(hurtid);
        int frozenDisMax = hurtData.FrozenDistanceMax;
        
        return VInt.NewVInt(frozenDisMax, GlobalLogic.VALUE_1000);
    }
    public MagicElementType GetAttackElementType(int hurtID)
    {
        var hurtData = TableManager.GetInstance().GetTableItem<ProtoTable.EffectTable>(hurtID);
        if (hurtData == null)
        {
            return MagicElementType.NONE;
        }

        var param = owner.TriggerEventNew(BeEventType.onChangeMagicElement, new EventParam(){ m_Int = hurtData.MagicElementType , m_Int2 = hurtID});
        MagicElementType attackElementType = (MagicElementType)param.m_Int;

        //攻击属性
        //int[] magicElementTypeArr = new int[2];
        //magicElementTypeArr[0] = hurtData.MagicElementType;
        //magicElementTypeArr[1] = hurtID;
        //owner.TriggerEvent(BeEventType.onChangeMagicElement, new object[] { magicElementTypeArr });
        //MagicElementType attackElementType = (MagicElementType)magicElementTypeArr[0];

        if (attackElementType == MagicElementType.NONE && hurtData.MagicElementISuse)
        {
            attackElementType = GetOwnAttackElement();
        }
        else if(attackElementType == MagicElementType.MAX)
        {
            return MagicElementType.NONE;
        }

        return attackElementType;
    }
    
    public int GetDamage(BeEntityData targetData, int hurtID, AttackResult damageType)
    {
        var hurtData = TableManager.GetInstance().GetTableItem<ProtoTable.EffectTable>(hurtID);
        if (hurtData == null)
        {
            return 0;
        }

        int skillLevel = GetSkillLevel(hurtData.SkillID);

        ProtoTable.EffectTable.eDamageType attackType = (ProtoTable.EffectTable.eDamageType)hurtData.DamageType;

		bool isPvPMode = BattleMain.IsModePvP(owner.battleType);

        int fixDamage = 0;
        VPercent hurtRate = VPercent.zero;

        //吃鸡模式下替换触发效果表数值
        if (BattleMain.IsChijiNeedReplaceHurtId(hurtID, owner.battleType))
        {
            var chijiEffectMapData = TableManager.instance.GetTableItem<ChijiEffectMapTable>(hurtID);
            fixDamage = TableManager.GetValueFromUnionCell(chijiEffectMapData.DamageFixedValuePVP, skillLevel);
            hurtRate = TableManager.GetValueFromUnionCell(chijiEffectMapData.DamageRatePVP, skillLevel);
        }
        else
        {
            fixDamage = TableManager.GetValueFromUnionCell(isPvPMode ? hurtData.DamageFixedValuePVP : hurtData.DamageFixedValue, skillLevel);
            hurtRate = TableManager.GetValueFromUnionCell(isPvPMode ? hurtData.DamageRatePVP : hurtData.DamageRate, skillLevel);
        }

        //修改触发效果表中读取到的伤害数值
        //var eventParam = DataStructPool.EventParamPool.Get();
        //eventParam.m_Int = hurtID;
        //eventParam.m_Int2 = fixDamage;
        //eventParam.m_Percent = hurtRate;
        //eventParam.m_Obj = targetData.owner;
        //owner.TriggerEventNew(BeEventType.onReplaceHurtTableDamageData, eventParam);
        //fixDamage = eventParam.m_Int2;
        //hurtRate = eventParam.m_Percent;
        //DataStructPool.EventParamPool.Release(eventParam);

        var param = owner.TriggerEventNew(BeEventType.onReplaceHurtTableDamageData, new EventParam(){ m_Int = hurtID, m_Int2 = fixDamage, m_Percent = hurtRate, m_Obj = targetData.owner });
        fixDamage = param.m_Int2;
        hurtRate = param.m_Percent;

        if (isSpecialAPC)
		{
			fixDamage = 0;
			hurtRate = TableManager.GetValueFromUnionCell(hurtData.DamageRateAPC, skillLevel);
		}

        var attackElementType = GetAttackElementType(hurtID);

        //处理技能增加的
        DealAttachDamage(ref fixDamage, ref hurtRate, hurtData.SkillID);

        int damage1 = GetSkillDamage(attackType, targetData, hurtRate, attackElementType);
        int damage2 = GetSkillFixDamage(attackType, targetData, fixDamage, attackElementType);

        // Logger.LogError("normal damage:" + damage1 + " fix damage:" + damage2);

        /*int[] damageArray = new int[2];
        damageArray[0] = GlobalLogic.VALUE_1000;
        damageArray[1] = GlobalLogic.VALUE_1000;
        owner.TriggerEvent(BeEventType.onChangeDamage, new object[] { hurtID, damageArray });
        if(targetData != null && targetData.owner != null)
        {
            targetData.owner.TriggerEvent(BeEventType.onBeHitChangeDamage, new object[] { hurtID, damageArray, hurtData });
        }
        damage1 *= VFactor.NewVFactor(damageArray[0], GlobalLogic.VALUE_1000);
        damage2 *= VFactor.NewVFactor(damageArray[1], GlobalLogic.VALUE_1000);*/
        int damageChange1 = GlobalLogic.VALUE_1000;
        int damageChange2 = GlobalLogic.VALUE_1000;
        var ret = owner.TriggerEventNew(BeEventType.onChangeDamage, new EventParam{m_Int = hurtID, m_Int2 = damageChange1, m_Int3 = damageChange2});
        damageChange1 = ret.m_Int2;
        damageChange2 = ret.m_Int3;
        if(targetData != null && targetData.owner != null)
        {
	        var ret2 = targetData.owner.TriggerEventNew(BeEventType.onBeHitChangeDamage, new EventParam{m_Int = hitID, m_Int2 = damageChange1, m_Int3 = damageChange2, m_Obj = hurtData});
	        damageChange1 = ret2.m_Int2;
	        damageChange2 = ret2.m_Int3;
        }
        damage1 *= VFactor.NewVFactor(damageChange1, GlobalLogic.VALUE_1000);
        damage2 *= VFactor.NewVFactor(damageChange2, GlobalLogic.VALUE_1000);
        
        int totalDamage = damage1 + damage2;

        if (damageType == AttackResult.CRITICAL)
        {
            /*m_CriticalPercentNew[0] = battleData.criticalPercent;
            int[] criticalPercentNew = m_CriticalPercentNew;
            owner.TriggerEvent(BeEventType.onChangeSummonMonsterAddCritiDamage, new object[] { criticalPercentNew });

            totalDamage = (totalDamage * ((VRate)(criticalDamageFactor + criticalPercentNew[0])).factor);*/
            int percent = battleData.criticalPercent;
            var criticalRet = owner.TriggerEventNew(BeEventType.onChangeSummonMonsterAddCritiDamage, new EventParam(){m_Int = percent});
            percent = criticalRet.m_Int;
            totalDamage = (totalDamage * ((VRate)(criticalDamageFactor + percent)).factor);
            //   Logger.LogErrorFormat("cirical:{0}", totalDamage);
        }

		//攻击力修正(天平)
		if (adjustBalance)
		{

			if (level < targetData.level)
			{
				//!! 找胡戈确认
				double damageFactor = Math.Pow((Math.Min(targetData.level, GlobalLogic.VALUE_50) / Math.Min(level, GlobalLogic.VALUE_50)), (double)Global.Settings.pkDamageAdjustFactor); 
				totalDamage = IntMath.Double2Int(totalDamage * damageFactor);
			}

			//Logger.LogErrorFormat("pvp damageFactor:{0}", damageFactor);
			var item = TableManager.GetInstance().GetTableItem<ProtoTable.PkHPLevelAdjustTable>(level);
            if (owner != null && owner.CurrentBeBattle != null && owner.CurrentBeBattle.PkRaceType == (int)Protocol.RaceType.ChiJi)
            {
                totalDamage = totalDamage * VRate.Factor(item.Attackfactor_chiji);
            }
            else
            {
                totalDamage = (totalDamage * VRate.Factor(item.Attackfactor));
            }

            var data2 = TableManager.GetInstance().GetTableItem<ProtoTable.PkHPProfessionAdjustTable>(professtion);
            if (data2 != null)
            {
                if (owner != null && owner.CurrentBeBattle != null)
                {
                    if (owner.CurrentBeBattle.PkRaceType == (int)Protocol.RaceType.ScoreWar)
                    {
                        totalDamage = totalDamage * VRate.Factor(data2.DamageFactor_3v3);
                    }
                    else if (owner.CurrentBeBattle.PkRaceType == (int)Protocol.RaceType.ChiJi)
                    {
                        totalDamage = totalDamage * VRate.Factor(data2.DamageFactor_chiji);
                    }
                }
                else
                {
                    totalDamage = totalDamage * VRate.Factor(data2.DamageFactor);
                }
            }

            
		}
			
        totalDamage = Mathf.Max(GlobalLogic.VALUE_1, totalDamage);

        return totalDamage;
    }

    public void DealAttachDamage(ref int fixDamage, ref VPercent hurtRate, int skillID)
    {
        if (owner != null && (owner as BeActor) != null)
        {
            BeActor actor = owner as BeActor;

            BeSkill skill = actor.GetSkill(skillID);
            if (skill != null)
            {
				//fixDamage = (int)((fixDamage + skill.attackAddFix) *(GlobalLogic.VALUE_1 + skill.attackAddRate));
				fixDamage = (fixDamage + skill.attackAddFix) * (VRate.one + skill.attackAddRate).factor;
				//hurtRate = (hurtRate + skill.attackAdd / (float)(GlobalLogic.VALUE_100)) *(GlobalLogic.VALUE_1 + skill.attackAddRate);
				//!! attackadd 百分比 转为 VPercent
				hurtRate = (hurtRate.i + VPercent.interPercent2VPercent(skill.attackAdd)) * (VRate.one + skill.attackAddRate).factor;
			}
        }
    }

    public void DealAttachHitRate(ref VRate hitRate, ref VRate criticalHitRate, int skillID)
    {
        if (owner != null && (owner as BeActor) != null)
        {
            BeActor actor = owner as BeActor;

            BeSkill skill = actor.GetSkill(skillID);
            if (skill != null)
            {
                hitRate += skill.hitRateAdd;
                criticalHitRate += skill.criticalHitRateAdd;
            }
        }
    }

    public void DealAttachFroze(ref int froze, int skillID)
    {
        if (owner != null && (owner as BeActor) != null)
        {
            BeActor actor = owner as BeActor;

            BeSkill skill = actor.GetSkill(skillID);
            if (skill != null)
            {
                froze = (froze * (VRate.one + skill.hardAddRate).factor);
            }
        }
    }
    

	bool CanApplyDamageModify(
		AddDamageInfo damageInfo,
		ProtoTable.EffectTable.eDamageType damageType,  
		ProtoTable.EffectTable.eDamageDistanceType damageDistanceType
	)
	{
		if (damageInfo.attackType == AttackType.NONE)
			return true;

		switch(damageInfo.attackType)
		{
		case AttackType.PHYSIC:
			return damageType == ProtoTable.EffectTable.eDamageType.PHYSIC;
		case AttackType.MAGIC:
			return damageType == ProtoTable.EffectTable.eDamageType.MAGIC;
		case AttackType.FAR:
			return damageDistanceType == ProtoTable.EffectTable.eDamageDistanceType.FAR;
		case AttackType.NEAR:
			return damageDistanceType == ProtoTable.EffectTable.eDamageDistanceType.NEAR;
		}

		return false;
	}

    private List<AddDamageInfo>[] m_AddDamageList = new List<AddDamageInfo>[4]; 

	/*
	返回附加跟增加后的最终伤害值，并且返回List附加伤害值
	*/
	public int GetAttachDamages(
		BeEntity target, 
		int damage, 
		ProtoTable.EffectTable.eDamageType damageType,  
		ProtoTable.EffectTable.eDamageDistanceType damageDistanceType,
		ref int damage2, 
		List<int> attachValues)
	{
		int finalDamage = damage;

        //召唤兽的Buff附加伤害数值受召唤师的影响
        ResetAddDamageArr(2);
        ResetAddDamageArr(3);
        List<AddDamageInfo> addDamageFixFixNew = m_AddDamageList[2];
        List<AddDamageInfo> addDamagePercentNew = m_AddDamageList[3];
        addDamageFixFixNew.AddRange(battleData.addDamageFix);
        addDamagePercentNew.AddRange(battleData.addDamagePercent);
        //owner.TriggerEvent(BeEventType.onChangeSummonMonsterAddDamage, new object[] { addDamageFixFixNew, addDamagePercentNew });
        owner.TriggerEventNew(BeEventType.onChangeSummonMonsterAddDamage, new EventParam(){m_Obj = addDamageFixFixNew, m_Obj2 = addDamagePercentNew});

        //职业增伤
        int skillAddDamageMax = 0;
        if (damageType == EffectTable.eDamageType.MAGIC)
        {
            for (int i = 0; i < battleData.skillAddMagicDamagePercent.Count; i++)
            {
                int value = IntMath.Double2Int(damage * (battleData.skillAddMagicDamagePercent[i].value / (double)(GlobalLogic.VALUE_1000)));
                if (value > skillAddDamageMax)
                    skillAddDamageMax = value;
            }

        }
        else if (damageType == EffectTable.eDamageType.PHYSIC)
        {
            for (int i = 0; i < battleData.skillAddDamagePercent.Count; i++)
            {
                int value = IntMath.Double2Int(damage * (battleData.skillAddDamagePercent[i].value / (double)(GlobalLogic.VALUE_1000)));
                if (value > skillAddDamageMax)
                    skillAddDamageMax = value;
            }
        }
        int addDamageMax = 0;
		//计算增加伤害固定值
		for(int i=0; i< addDamageFixFixNew.Count; ++i)
		{
			if (addDamageFixFixNew[i].value > addDamageMax)
				addDamageMax = addDamageFixFixNew[i].value;
		}
			
		//计算增加伤害百分比
		for(int i=0; i< addDamagePercentNew.Count; ++i)
		{
			int value = IntMath.Double2Int(damage * (addDamagePercentNew[i].value/(double)(GlobalLogic.VALUE_1000)));
			if (value > addDamageMax)
				addDamageMax = value;
		}

		finalDamage += addDamageMax + skillAddDamageMax;

		if (addDamageMax > 0)
		{
			Logger.LogWarningFormat("增加伤害：normal: {0} attach values:{1}", damage, addDamageMax);
		}
			
		//减伤
		if (target.GetEntityData() != null)
		{
			var entityData = target.GetEntityData();

			int reduceDamage = 0;
			for(int i=0; i<entityData.battleData.reduceDamageFix.Count; ++i)
			{
				AddDamageInfo dInfo = entityData.battleData.reduceDamageFix[i];

				if (CanApplyDamageModify(dInfo, damageType, damageDistanceType))
					reduceDamage += dInfo.value;
			}


            int totalReduceDamagePercent = 0;
            int maxReduceDamagePercent = 0;
            for (int i = 0; i < entityData.battleData.reduceDamagePercent.Count; ++i)
            {
                AddDamageInfo dInfo = entityData.battleData.reduceDamagePercent[i];

                if (CanApplyDamageModify(dInfo, damageType, damageDistanceType))
                {
                    if (BeClientSwitch.FunctionIsOpen(ClientSwitchType.GeDangNewAlgorithms))
                    {
                        totalReduceDamagePercent += dInfo.value;
                    }
                    else
                    {
                        //如果是格挡
                        if (dInfo.value >= 400)
                        {
                            maxReduceDamagePercent = Mathf.Max(dInfo.value, maxReduceDamagePercent);
                        }
                        else
                            totalReduceDamagePercent += dInfo.value;
                    }
                    //reduceDamage += IntMath.Double2Int(dInfo.value / (double)(GlobalLogic.VALUE_1000) * damage);
                }
					
			}
            int extraTotalReduceDamagePercent = 0;
            for (int i = 0; i < entityData.battleData.reduceExtraDamagePercent.Count; ++i)
            {
                AddDamageInfo dInfo = entityData.battleData.reduceExtraDamagePercent[i];

                if (CanApplyDamageModify(dInfo, damageType, damageDistanceType))
                {
                    extraTotalReduceDamagePercent += dInfo.value;
                }
            }

            int totalReduceMeiyingPercent = 0;
            for (int i = 0; i < entityData.battleData.reduceMeiyingDamagePercent.Count; ++i)
            {
                AddDamageInfo dInfo = entityData.battleData.reduceMeiyingDamagePercent[i];

                if (CanApplyDamageModify(dInfo, damageType, damageDistanceType))
                {
                    totalReduceMeiyingPercent += dInfo.value;
                }
            }

            int maxGeDangReduceDamagePercent = 0;
            if (BeClientSwitch.FunctionIsOpen(ClientSwitchType.GeDangNewAlgorithms))
            {
                for (int i = 0; i < entityData.battleData.reduceGeDangDamagePercent.Count; ++i)
                {
                    AddDamageInfo dInfo = entityData.battleData.reduceGeDangDamagePercent[i];

                    if (CanApplyDamageModify(dInfo, damageType, damageDistanceType))
                    {
                        maxGeDangReduceDamagePercent = Mathf.Max(dInfo.value, maxGeDangReduceDamagePercent);
                    }
                }
            }
            totalReduceDamagePercent = Mathf.Clamp(totalReduceDamagePercent + maxReduceDamagePercent, 0, 990);
            //初始减伤
            int value = IntMath.Double2Int(totalReduceDamagePercent / (double)(GlobalLogic.VALUE_1000) * damage);
            reduceDamage += value;
            finalDamage -= reduceDamage;
            //额外减伤
            value = damage - value;
            extraTotalReduceDamagePercent = Mathf.Clamp(extraTotalReduceDamagePercent, 0, 990);
            int reduceExtraDamage = IntMath.Double2Int(extraTotalReduceDamagePercent / (double)(GlobalLogic.VALUE_1000) * (value));
            finalDamage -= reduceExtraDamage;
            value = value - reduceExtraDamage;
            //魅影减伤
            totalReduceMeiyingPercent = Mathf.Clamp(totalReduceMeiyingPercent, 0, 990);
            int reduceMeiyingDamage = IntMath.Double2Int(totalReduceMeiyingPercent / (double)(GlobalLogic.VALUE_1000) * (value));
            finalDamage -= reduceMeiyingDamage;
            value = value - reduceMeiyingDamage;
            //格挡减伤
            maxGeDangReduceDamagePercent = Mathf.Clamp(maxGeDangReduceDamagePercent, 0, 990);
            int reduceGeDangeDamage = IntMath.Double2Int(maxGeDangReduceDamagePercent / (double)(GlobalLogic.VALUE_1000) * (value));
            finalDamage -= reduceGeDangeDamage;
            finalDamage = Mathf.Max(finalDamage, 0);
            if (reduceDamage > 0)
			{
				Logger.LogWarningFormat("减伤：normal: {0} reduce values:{1}", damage, reduceDamage);
			}
		}


		damage2 = finalDamage;

        //召唤兽的Buff附加伤害数值受召唤师的影响
        ResetAddDamageArr(0);
        ResetAddDamageArr(1);
        List<AddDamageInfo> attachAddDamageFixNew = m_AddDamageList[0];
        List<AddDamageInfo> attachAddDamagePercentNew = m_AddDamageList[1];
        attachAddDamageFixNew.AddRange(battleData.attachAddDamageFix);
        attachAddDamagePercentNew.AddRange(battleData.attachAddDamagePercent);
        //owner.TriggerEvent(BeEventType.onChangeSummonMonsterAttach, new object[] {attachAddDamageFixNew, attachAddDamagePercentNew });
        owner.TriggerEventNew(BeEventType.onChangeSummonMonsterAttach, new EventParam{m_Obj = attachAddDamageFixNew, m_Obj2 = attachAddDamagePercentNew});

        //附加伤害
        for (int i=0; i< attachAddDamageFixNew.Count; ++i)
		{
			AddDamageInfo dInfo = attachAddDamageFixNew[i];

			if (CanApplyDamageModify(dInfo, damageType, damageDistanceType))
			{
				finalDamage += dInfo.value;
				attachValues.Add(attachAddDamageFixNew[i].value);
			}
		}

		for(int i=0; i< attachAddDamagePercentNew.Count; ++i)
		{
			AddDamageInfo dInfo = attachAddDamagePercentNew[i];

			if (CanApplyDamageModify(dInfo, damageType, damageDistanceType))
			{
				int value = IntMath.Double2Int(damage * (dInfo.value/(double)(GlobalLogic.VALUE_1000)));
				attachValues.Add(value);
				finalDamage += value;
			}
		}


		if(attachValues.Count > 0)
		{
			string v = "";
			for(int i=0; i<attachValues.Count; ++i)
				v += attachValues[i] + ",";

			Logger.LogWarningFormat("附加伤害：normal: {0} attach values:{1}", damage, v);
		}

        //伤害受到抗魔值影响
        finalDamage *= GetResistMagicRate() + 1;

        return finalDamage;
	}

	public int GetReflectDamages(int damage)
	{
		int reflectDamage = 0;
		for(int i=0; i<battleData.reflectDamageFix.Count; ++i)
		{
			reflectDamage += battleData.reflectDamageFix[i].value;
		}

		for(int i=0; i<battleData.reflectDamagePercent.Count; ++i)
		{
			reflectDamage += IntMath.Double2Int(battleData.reflectDamagePercent[i].value * damage /(double)(GlobalLogic.VALUE_1000) );
		}

		return reflectDamage;
	}

	/*
	 * 已经废弃
	*/
    public int GetAttachDamage(int damage)
    {
        //float attachRate = ;
        return Math.Max(0, IntMath.Double2Int((damage * battleData.attachHurtRate) / (double)(GlobalLogic.VALUE_1000) ) + battleData.attachHurtFix);
    }

    public int GetSkillDamage(ProtoTable.EffectTable.eDamageType attackType, BeEntityData targetData, VPercent skillPercent, MagicElementType attackElementType = MagicElementType.NONE)
    {
        double damage = 0;
        if (attackType == ProtoTable.EffectTable.eDamageType.PHYSIC)
            damage = CalculateDamage(targetData, attackElementType);
        else if (attackType == ProtoTable.EffectTable.eDamageType.MAGIC)
            damage = CalculateMagicDamage(targetData, attackElementType);
		
		int idamage = IntMath.Double2Int(damage);
		idamage *= skillPercent.precent;
        //damage *= skillPercent.precent.single;
        return Math.Max(0, idamage);
    }

    public int GetSkillFixDamage(ProtoTable.EffectTable.eDamageType type, BeEntityData targetData, int fixDamage, MagicElementType attackElementType = MagicElementType.NONE)
    {
        double damage = 0;
        if (type == ProtoTable.EffectTable.eDamageType.PHYSIC)
            damage = CalculateFixDamage(targetData, fixDamage, attackElementType);
        else if (type == ProtoTable.EffectTable.eDamageType.MAGIC)
            damage = CalculateFixMagicDamage(targetData, fixDamage, attackElementType);

        return Math.Max(0,IntMath.Double2Int(damage));
    }

	private double GetElementFactor(BeEntityData targetData, MagicElementType attackElementType = MagicElementType.NONE)
    {
        double elementFactor = GlobalLogic.VALUE_1;
        if (attackElementType != MagicElementType.NONE)
        {
            int elementDefence = targetData.GetMagicElementDefence(attackElementType);
            /*int[] array = new int[] { elementDefence};
            if (owner != null)
            {
                owner.TriggerEvent(BeEventType.OnChangeAttributeDefence, new object[] { array});
                elementDefence = array[0];
            }*/
            if (owner != null)
            {
	            var ret = owner.TriggerEventNew(BeEventType.OnChangeAttributeDefence, new EventParam{m_Int = elementDefence});
	            elementDefence = ret.m_Int;
            }
            elementFactor = (GlobalLogic.VALUE_1 + (GetMagicElementAttack(attackElementType) - elementDefence) / 220.0d);
        }

        return elementFactor;
    }

    private double CalculateDamage(BeEntityData targetData, MagicElementType attackElementType = MagicElementType.NONE)
    {
        double defenceReduce = GlobalLogic.VALUE_1 - targetData.GetAttackReduce(level,battleData);

        double damage = battleData.fAttack * defenceReduce * GetElementFactor(targetData, attackElementType) + battleData.ignoreDefAttackAdd * (GlobalLogic.VALUE_1 - targetData.battleData.attackReduceRate / (double)(GlobalLogic.VALUE_10000)) - targetData.battleData.attackReduceFix;

        damage = Math.Max(0, damage);
        return damage;
    }

    private double CalculateMagicDamage(BeEntityData targetData, MagicElementType attackElementType = MagicElementType.NONE)
    {
        double defenceReduce = GlobalLogic.VALUE_1 - targetData.GetMagicAttackReduce(level,battleData);

        double damage = battleData.fMagicAttack * defenceReduce * GetElementFactor(targetData, attackElementType) + battleData.ignoreDefMagicAttackAdd * (GlobalLogic.VALUE_1 - targetData.battleData.magicAttackAddRate / (double)(GlobalLogic.VALUE_10000)) - targetData.battleData.magicAttackReduceFix;

        damage = Math.Max(0, damage);
        return damage;
    }   

    private double CalculateFixDamage(BeEntityData targetData, int fixDamage, MagicElementType attackElementType = MagicElementType.NONE)
    {
        double damage = fixDamage * (GlobalLogic.VALUE_1 + battleData._baseAtk / (double)(GlobalLogic.VALUE_250)) * (GlobalLogic.VALUE_1 + battleData._baseIndependence / (double)(GlobalLogic.VALUE_1500)) * (GlobalLogic.VALUE_1 - targetData.GetAttackReduce(level,battleData)) * GetElementFactor(targetData, attackElementType);
        damage = Math.Max(0, damage);
        return damage;
    }

    private double CalculateFixMagicDamage(BeEntityData targetData, int fixDamage, MagicElementType attackElementType = MagicElementType.NONE)
    {
		double damage = fixDamage * (GlobalLogic.VALUE_1 + battleData._baseInt / (double)(GlobalLogic.VALUE_250)) * (GlobalLogic.VALUE_1 + battleData._baseIndependence / (double)(GlobalLogic.VALUE_1500)) * (GlobalLogic.VALUE_1 - targetData.GetMagicAttackReduce(level,battleData)) * GetElementFactor(targetData, attackElementType);
        damage = Math.Max(0, damage);
        return damage;
    }

    public double GetAttackReduce(int attackerLevel, BattleData attackerAttr)
    {
		if (BattleMain.IsModePvP(owner.battleType))
			attackerLevel = level;
        double ingoreDefRateRate = attackerAttr.ingoreDefRate / (double)(GlobalLogic.VALUE_1000);
        if (ingoreDefRateRate < 0.0d)
        {
            ingoreDefRateRate = 0.0d;
        }
        if (ingoreDefRateRate > 1.0d)
        {
            ingoreDefRateRate = 1.0d;
        }
        double ingoreDefValue = (double)(GlobalLogic.VALUE_1) - ingoreDefRateRate;

        double ret = (double)(GlobalLogic.VALUE_1) - (((double)(GlobalLogic.VALUE_1) - battleData.fDefence * ingoreDefValue / (double)(attackerLevel * GlobalLogic.VALUE_200 + battleData.fDefence * ingoreDefValue)) * ((double)(GlobalLogic.VALUE_1) - battleData.attackReduceRate / (double)(GlobalLogic.VALUE_10000)));
        if (ret < 0.0d)
            ret = 0.0d;
        if (ret > 1.0d)
        {
            ret = 1.0d;
        }

        return ret;
    }

    public double GetMagicAttackReduce(int attackerlevel, BattleData attackerAttr)
    {
		if (BattleMain.IsModePvP(owner.battleType))
			attackerlevel = level;

        double ingoreMagicDefRateRate = attackerAttr.ingoreMagicDefRate / (double)(GlobalLogic.VALUE_1000);
        if (ingoreMagicDefRateRate < 0.0d)
        {
            ingoreMagicDefRateRate = 0.0d;
        }
        if (ingoreMagicDefRateRate > 1.0d)
        {
            ingoreMagicDefRateRate = 1.0d;
        }
        double ingoreMagicDefValue = (double)(GlobalLogic.VALUE_1) - ingoreMagicDefRateRate;
        double ret = (double)(GlobalLogic.VALUE_1) - (((double)(GlobalLogic.VALUE_1) - battleData.fMagicDefence * ingoreMagicDefValue / (double)(attackerlevel * GlobalLogic.VALUE_200 + battleData.fMagicDefence * ingoreMagicDefValue)) * ((double)(GlobalLogic.VALUE_1) - battleData.magicAttackAddRate / (double)(GlobalLogic.VALUE_10000)));
        if (ret < 0.0d)
            ret = 0.0d;
        if (ret > 1.0d)
        {
            ret = 1.0d;
        }

        return ret;
    }

    public int GetAbnormalValue(BuffType type)
    {
        return battleData.abnormalResists[(int)(type - BuffType.FLASH)] + battleData.abnormalResist;
    }

    //设置抗魔值
    public void SetResistMagic(int resistMagic)
    {
        if (isMonster && camp != (int)ProtoTable.UnitTable.eCamp.C_HERO)
            return;
        battleData.resistMagic = resistMagic;
    }

    //获取抗魔值
    public int GetResistMagic()
    {
        return battleData.resistMagic;
    }

    //抗魔值对攻击力和生命值的影响
    public VFactor GetResistMagicRate()
    {
        if (isMonster && camp != (int)ProtoTable.UnitTable.eCamp.C_HERO)
            return VFactor.zero;
        int dungeonResistMagic = BeUtility.GetDungeonMagicValue(owner); 
        if (dungeonResistMagic == 0)
            return VFactor.zero;
        VFactor impactValue = new VFactor(battleData.resistMagic, dungeonResistMagic) - VFactor.one;
        VFactor min = new VFactor(-700,GlobalLogic.VALUE_1000);
        VFactor max = new VFactor(200,GlobalLogic.VALUE_1000);
        if(impactValue <= min)
        {
            return min;
        }
        else if (impactValue >= max)
        {
            return max;
        }
        return impactValue;
    }

    //设置决斗场机器人数据
    public void SetAIRobotData(ItemProperty item)
    {
        ItemTable itemTable = TableManager.instance.GetTableItem<ItemTable>(item.itemID);
        if (itemTable == null)
            return;
        SetAIRobotIgnoreDefAttackAdd(item, itemTable);
        SetAIRobotAttackReduceFix(item, itemTable);
        SetAIRobotMagicAttackReduceFix(item, itemTable);
    }

    //设置无视物防攻击力和魔防攻击力
    private void SetAIRobotIgnoreDefAttackAdd(ItemProperty item,ItemTable itemTable)
    {
        if (!BeUtility.IsWeapon(itemTable.SubType))
            return;
        int wpStrMod = BeUtility.GetEquipStrModByStrength(item.strengthen, EquipStrMod.WpStrenthMod);
        int wpClQaMod = BeUtility.GetEquipStrModByColor((int)itemTable.Color, EquipStrMod.WpColorQaMod);
        int wpClQbMod = BeUtility.GetEquipStrModByColor((int)itemTable.Color, EquipStrMod.WpColorQbMod);

        VFactor addValueA =VFactor.NewVFactor(itemTable.NeedLevel * GlobalLogic.VALUE_100 + wpClQaMod,GlobalLogic.VALUE_100);
        VFactor addValueB = addValueA * VFactor.NewVFactor(125, GlobalLogic.VALUE_1000);
        VFactor addValueC = addValueB * VFactor.NewVFactor(wpStrMod, GlobalLogic.VALUE_100);
        VFactor addValueD = addValueC * VFactor.NewVFactor(wpClQbMod, GlobalLogic.VALUE_100);
        VFactor addValueE = addValueD * VFactor.NewVFactor(1100, GlobalLogic.VALUE_1000);

        int addValue = addValueE.integer;
        battleData.ignoreDefAttackAdd += addValue;
        battleData.ignoreDefMagicAttackAdd += addValue;
    }

    //设置物理攻击减免固定值
    private void SetAIRobotAttackReduceFix(ItemProperty item, ItemTable itemTable)
    {
        if (!BeUtility.IsArmy(itemTable.SubType))
            return;
        int ArmStrenthMod = BeUtility.GetEquipStrModByStrength(item.strengthen, EquipStrMod.ArmStrenthMod);
        int ArmColorQaMod = BeUtility.GetEquipStrModByColor((int)itemTable.Color, EquipStrMod.ArmColorQaMod);
        int ArmColorQbMod = BeUtility.GetEquipStrModByColor((int)itemTable.Color, EquipStrMod.ArmColorQbMod);

        VFactor addValueA = VFactor.NewVFactor(itemTable.NeedLevel * GlobalLogic.VALUE_100 + ArmColorQaMod, GlobalLogic.VALUE_100);
        VFactor addValueB = addValueA * VFactor.NewVFactor(125, GlobalLogic.VALUE_1000);
        VFactor addValueC = addValueB * VFactor.NewVFactor(ArmStrenthMod, GlobalLogic.VALUE_100);
        VFactor addValueD = addValueC * VFactor.NewVFactor(ArmColorQbMod, GlobalLogic.VALUE_100);

        int addValue = addValueD.integer;
        battleData.attackReduceFix += addValue;
    }

    //设置魔法攻击减免固定值
    private void SetAIRobotMagicAttackReduceFix(ItemProperty item, ItemTable itemTable)
    {
        if (!BeUtility.IsJewelry(itemTable.SubType))
            return;
        int JewStrenthMod = BeUtility.GetEquipStrModByStrength(item.strengthen, EquipStrMod.JewStrenthMod);
        int JewColorQaMod = BeUtility.GetEquipStrModByColor((int)itemTable.Color, EquipStrMod.JewColorQaMod);
        int JewColorQbMod = BeUtility.GetEquipStrModByColor((int)itemTable.Color, EquipStrMod.JewColorQbMod);

        VFactor addValueA = VFactor.NewVFactor(itemTable.NeedLevel * GlobalLogic.VALUE_100 + JewColorQaMod, GlobalLogic.VALUE_100);
        VFactor addValueB = addValueA * VFactor.NewVFactor(125, GlobalLogic.VALUE_1000);
        VFactor addValueC = addValueB * VFactor.NewVFactor(JewStrenthMod, GlobalLogic.VALUE_100);
        VFactor addValueD = addValueC * VFactor.NewVFactor(JewColorQbMod, GlobalLogic.VALUE_100);

        int addValue = addValueD.integer;
        battleData.magicAttackReduceFix += addValue;
    }

    //异常状态触发几率
    //!! 以后几率都使用 1000为单位的值
    public bool CanAddAbnormalState(VRate abnormalRate, int abnormalLevel, BuffType buffType)
    {
		double abf = GetAbnormalValue(buffType) / 1.2d / GlobalLogic.VALUE_100;

        double tmp = (GlobalLogic.VALUE_1 - abf);
        tmp = Math.Max(0d, tmp);

		int rate = 0;
		if (abnormalLevel <= level)
			rate = IntMath.Double2Int(abnormalRate.i * (GlobalLogic.VALUE_1 + (abnormalLevel - level) * 0.05d) * tmp);
		else
			rate = IntMath.Double2Int(abnormalRate.i * tmp);

        //rate *= GlobalLogic.VALUE_1000;

		int randValue = FrameRandom.Range1000();

		return randValue < rate;
    }

    //异常状态伤害计算
    public int GetAbnormalDamage(int abnormalAttack, int attackCount=0, BeActor attacker=null)
    {
		double abnormalReduce = 0;

        //使用面板物理攻击和魔法攻击进行计算 并且添加服务器开关
        double fDefence = battleData.fDefence;
        double fMagicDefence = battleData.fMagicDefence;
        if (attacker != null && attacker.GetEntityData() != null)
        {
            double ingoreDefRateRate = attacker.GetEntityData().battleData.ingoreDefRate / (double)(GlobalLogic.VALUE_1000);
            if (ingoreDefRateRate < 0.0d)
            {
                ingoreDefRateRate = 0.0d;
            }
            if (ingoreDefRateRate > 1.0d)
            {
                ingoreDefRateRate = 1.0d;
            }
            double ingoreDefValue = (double)(GlobalLogic.VALUE_1) - ingoreDefRateRate;
            fDefence *= ingoreDefValue;

            double ingoreMagicDefRateRate = attacker.GetEntityData().battleData.ingoreMagicDefRate / (double)(GlobalLogic.VALUE_1000);
            double ingoreMagicDefValue = (double)(GlobalLogic.VALUE_1) - ingoreMagicDefRateRate;
            if (ingoreMagicDefValue < 0.0d)
            {
                ingoreMagicDefValue = 0.0d;
            }
            if (ingoreMagicDefValue > 1.0d)
            {
                ingoreMagicDefValue = 1.0d;
            }
            fMagicDefence *= ingoreMagicDefValue;

        }
        abnormalReduce = GlobalLogic.VALUE_1 - (fDefence + fMagicDefence) / (double)(fDefence + fMagicDefence + 800 * level);

        double factorAdd = 0;
		if (attacker != null && attacker.GetEntityData() != null)
		{
			factorAdd = (attacker.GetEntityData().battleData._baseAtk + attacker.GetEntityData().battleData._baseInt) / (double)(GlobalLogic.VALUE_500);
		}

        double damage = 0;
        if (attackCount == 0)
        {
            //适用于感电
			damage = abnormalAttack * (GlobalLogic.VALUE_1 + factorAdd) * abnormalReduce; 
        }
        else
        {
            //适用于出血，中毒，灼伤等
			damage = abnormalAttack * (GlobalLogic.VALUE_1 + factorAdd) * abnormalReduce / attackCount;
        }

        //抗魔值带入计算
        int finalDamage = IntMath.Double2Int(damage);
        if (attacker != null && attacker.attribute != null)
        {
            finalDamage *= attacker.attribute.GetResistMagicRate() + 1;
        }

        Logger.LogWarningFormat("GetAbnormalDamage abnormalAttack {0}---> damage {1}", abnormalAttack, damage);

        return Mathf.Max(0, finalDamage);
    }

    public int GetSkillLevel(int skillID)
    {
        int level = GlobalLogic.VALUE_1;
        if (skillLevelInfo.ContainsKey(skillID))
        {
            level = skillLevelInfo[skillID];
        }

        return level;
    }

    public void OnHPChange(int value)
    {
		if (value < 0 && owner != null && owner.stateController!=null && !owner.stateController.CanHurt())
		{
			Logger.LogWarningFormat("{0} have NO HURT!!!!value:{1}", owner.GetName(), value);
			return;
		}
			
		battleData.hp += value;
		battleData.hp = Mathf.Clamp(battleData.hp, 0, battleData.maxHp);

		if (owner != null)
		{
			//owner.TriggerEvent(BeEventType.onHPChange, new object[] { value });
			owner.TriggerEventNew(BeEventType.onHPChange, new EventParam(){m_Int = value});

			owner.JudgeDead();

		}
    }  

    public void OnMPChange(int value)
    {
		battleData.mp += value;
		battleData.mp = Mathf.Clamp(battleData.mp, 0, battleData.maxMp);
    }

	public VRate GetCDReduce(SkillMaigcType type)
	{
        int value = 0;
        if (type == SkillMaigcType.MAGIC)
            value = battleData.cdReduceRateMagic;
        else if (type == SkillMaigcType.PHYSIC)
            value = battleData.cdReduceRate;
        else
            value = (jobAttribute == 0 ? battleData.cdReduceRate : battleData.cdReduceRateMagic);

        return -value;
	}

	public VRate GetMPCostReduce(SkillMaigcType type)
	{
        int value = 0;
        if (type == SkillMaigcType.MAGIC)
            value = battleData.mpCostReduceRateMagic;
        else if (type == SkillMaigcType.PHYSIC)
            value = battleData.mpCostReduceRate;
        else
            value = (jobAttribute == 0 ? battleData.mpCostReduceRate : battleData.mpCostReduceRateMagic);

        return -value;
	}

    public int GetLevel()
    {
        return level;
    }

	public int GetMP()
	{
		return battleData.mp;
	}

	public int GetHP()
	{
		return battleData.hp;
	}

	public int GetMaxHP()
	{
		return battleData.maxHp;
	}

	public int GetMaxMP()
	{
		return battleData.maxMp;
	}

	public VFactor GetMPRate()
	{
		return new VFactor(battleData.mp,battleData.maxMp);
	}

	public VFactor GetHPRate()
	{
		return new VFactor(battleData.hp,battleData.maxHp);
	}

	public void ChangeHPReduce(int value)
	{
		//Logger.LogErrorFormat("ChangeHPReduce {0} => {1}", battleData.hpReduce, battleData.hpReduce+value);

		battleData.hpReduce += value;
		battleData.hpReduce = Mathf.Max(battleData.hpReduce, 0);
	}

	public void ChangeMPReduce(int value)
	{
		battleData.mpReduce += value;
		battleData.mpReduce = Mathf.Max(battleData.mpReduce, 0);
	}

    //以后buff加成写在这
    public int GetAttackSpeed()
    {
		return battleData.attackSpeed;
    }

    public int GetMoveSpeed()
    {
		return battleData.moveSpeed;
    }

	public int GetSpellSpeed()
	{
		return battleData.spellSpeed;
	}

	public void UpdateLevel(int skillId, int level)
	{
		if (skillLevelInfo.ContainsKey(skillId))
		{
			skillLevelInfo[skillId] = level;
		}
	}

	public void AdjustHPForPvP(int ourLevel, int targetLevel, int ourPro, int targetPro, int pkRaceType=0)
    {
        double hpScale = 1;

		int useLevel = Global.Settings.pkUseMaxLevel?Math.Max(ourLevel, targetLevel):ourLevel;
		var item = TableManager.GetInstance().GetTableItem<ProtoTable.PkHPLevelAdjustTable>(useLevel);
		if (item != null)
		{
            if (pkRaceType == (int)Protocol.RaceType.ChiJi/* owner != null && owner.CurrentBeBattle != null && owner.CurrentBeBattle.PkRaceType == (int)Protocol.RaceType.ChiJi*/)
            {
                hpScale = item.factor_chiji / (double)(GlobalLogic.VALUE_1000);
            }
            else
            {
                hpScale = item.factor / (double)(GlobalLogic.VALUE_1000);
            }
		}

        double value = TableManager.GetInstance().GetPKHPAdjustFactor(ourPro, pkRaceType /* owner != null && owner.CurrentBeBattle != null ? owner.CurrentBeBattle.PkRaceType : 0*/);
		hpScale *= value;

		if (ourLevel < targetLevel)
		{
			hpScale *= Mathf.Pow(Math.Min(targetLevel, GlobalLogic.VALUE_50) / Math.Min(ourLevel, GlobalLogic.VALUE_50), Global.Settings.pkHPAdjustFactor);
		}

		//Logger.LogErrorFormat("AdjustHPForPvP levelScale:{0}, professionScale:{1}", item.factor/1000f, value);


		this.hpScale = hpScale;
			
		AdjustHPForPvP(hpScale);
    }

    public void AdjustHPForScufflePVP(int ourLevel,  int ourPro)
    {
        double hpScale = 1;


        var item = TableManager.GetInstance().GetTableItem<ProtoTable.PkHPLevelAdjustTable>(ourLevel);
        if (item != null)
        {
            hpScale = item.factor / (double)(GlobalLogic.VALUE_1000);
        }

        double value = TableManager.GetInstance().GetPKHPAdjustFactor(ourPro,0);
        hpScale *= value;

        this.hpScale = hpScale;

        AdjustHPForPvP(hpScale);
    }

    public void AdjustHPForPvP(double hpScale)
	{
		battleData.hpScale = IntMath.Double2Int(hpScale * GlobalLogic.VALUE_1000);
		double finalHP = battleData.maxHp * hpScale;

		SetMaxHP(IntMath.Double2Int(finalHP));
		SetHP(GetMaxHP());
	}

	public bool MonsterIDEqual(int mid)
	{
		return BeUtility.IsMonsterIDEqual(mid, monsterID);
	}

	public void SetHP(int value)
	{
		battleData.hp = value;
	}
    public void SetMP(int value)
    {
        battleData.mp = value;
    }
    public void SetMaxHP(int value)
    {
        battleData.maxHp = value;
    }

    public void ChangeMaxHp(int value)
    {
        battleData.ChangeMaxHP(value);
        ChangeMaxHpByResist();
    }

    //根据抗魔值改变最大血量
    public void ChangeMaxHpByResist()
    {
        if (owner == null)
            return;
        if (battleData.hp == 0)
            return;
        VFactor rate = GetResistMagicRate();
        double curHPRate = battleData.hp / (double)battleData.maxHp;
        battleData.maxHp = battleData.fMaxHp * (rate + 1);
        battleData.hp = IntMath.Double2Int(curHPRate * battleData.maxHp);
    }

    /// <summary>
    /// 初始化增加伤害数组
    /// </summary>
    private void ResetAddDamageArr(int index)
    {
        if (m_AddDamageList[index] == null)
        {
            m_AddDamageList[index] = new List<AddDamageInfo>();
        }
        else
        {
            m_AddDamageList[index].Clear();
        }
    }

    #region MagicElementType
    public void SetAbnormalResists(int[] inData, bool add=false)
    {
        for(int i=0; i<Global.ABNORMAL_COUNT; ++i)
        {
            if (inData[i] == 0)
                continue;

            if (add)
                battleData.abnormalResists[i] += inData[i];
            else
                battleData.abnormalResists[i] = inData[i];

        }
    }


    public void SetMagicElementTypes(IList<int> arr, bool isAdd = true)
    {
        int count = arr.Count;
        if (count < 1)
            return;

        int[] magicELements = battleData.magicELements;

        int value = isAdd?1:-1;
        int max = (int)(MagicElementType.MAX);

        for (int i = 0; i < count; i++)
        {
            int curMagicElementType = arr[i];
            if (curMagicElementType > 0 && curMagicElementType < max)
            {
                magicELements[curMagicElementType] += value;
            }
        }
    }

    public bool HasMagicElementType(int curType)
    {

        if (curType <= (int)MagicElementType.NONE || curType >= (int)MagicElementType.MAX)
            return false;

        return battleData.magicELements[curType] > 0;
    }

    //获得属强
    public int GetMagicElementAttack(MagicElementType type)
    {
        if (type > MagicElementType.NONE && type < MagicElementType.MAX)
            return battleData.magicElementsAttack[(int)type];

        return 0;
    }

    //获得属抗
    public int GetMagicElementDefence(MagicElementType type)
    {
        if (type > MagicElementType.NONE && type < MagicElementType.MAX)
            return battleData.magicElementsDefence[(int)type];

        return 0;
    }

    public MagicElementType GetOwnAttackElement()
    {
        MagicElementType ret = MagicElementType.NONE;

        int maxElementValue = -1;

        for (int i = 1; i < (int)MagicElementType.MAX; ++i)
        {
            int v = battleData.magicELements[i];
            int attack = battleData.magicElementsAttack[i];
            if (v > 0 && attack > maxElementValue)
            {
                maxElementValue = attack;
                ret = (MagicElementType)(i);
            }
        }

        return ret;
    }

    //获取装备添加的属性类型
    public void GetOwnerEquipElement(List<int> magicElementTypeList)
    {
        for (int i = 1; i < (int)MagicElementType.MAX; ++i)
        {
            int v = battleData.magicELements[i];
            if(v > 0)
            {
                magicElementTypeList.Add(i);
            }
        }
    }

    #endregion

    #region WEAPON

    public int GetWeaponTagImp(int itemID)
    {
        int weaponType = 0;
        var itemData = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(itemID);
        if (itemData != null)
        {
            var slot = (EEquipWearSlotType)itemData.SubType;
            if (slot == EEquipWearSlotType.EquipWeapon)
            {
                weaponType = (int)itemData.Tag;
            }
        }

        return weaponType;
    }
    public int GetWeaponTypeImp(int itemID)
    {
        int weaponType = 0;
        var itemData = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(itemID);
        if(itemData != null)
        {
            var slot = (EEquipWearSlotType)itemData.SubType;
            if (slot == EEquipWearSlotType.EquipWeapon)
            {
                weaponType = (int)itemData.ThirdType;
            }
        }
        return weaponType;
    }

    public int GetWeaponType()
    {
        int weaponType = 0;
        if (currentWeapon == null)
            return weaponType;

        var itemID = currentWeapon.itemID;
        var itemData = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(itemID);
        if (itemData != null)
        {
            var slot = (EEquipWearSlotType)itemData.SubType;
            if (slot == EEquipWearSlotType.EquipWeapon)
            {
                weaponType = (int)itemData.ThirdType;
            }
        }

        return weaponType;
    }

    public int GetWeaponTag()
    {
        int weaponType = 0;
        if (currentWeapon == null)
            return weaponType;

        var itemID = currentWeapon.itemID;
        weaponType = GetWeaponTagImp(itemID);

        return weaponType;
    }

    public int GetWeaponItemID()
    {
        int weaponID = 0;
        if (currentWeapon == null)
            return weaponID;

        return currentWeapon.itemID;
    }

    public bool CanChangeWeapon()
    {
        return GetWeaponItemID() != 0 && backupWeapons.Count > 0;
    }

    public int GetBackupEquipItemID()
    {
        int id = 0;
        if (backupWeapons.Count > 0)
            id = backupWeapons[0].itemID;
        return id;
    }

    public void ChangeWeapon(int index)
    {
        if (currentWeapon == null)
            return;

        var bu = backupWeapons[index];
        backupWeapons.RemoveAt(index);

        backupWeapons.Add(currentWeapon);

        if (itemProperties != null)
            itemProperties.Remove(currentWeapon);
        RemoveEquipment(currentWeapon);

        currentWeapon = bu;
        if (itemProperties != null)
            itemProperties.Insert(0, currentWeapon);
        AddEquipment(currentWeapon);
        ChangeMaxHpByResist();
    }

    public List<int> GetBackupWeaponTags()
    {
        List<int> tags = new List<int>();
        for(int i=0; i<backupWeapons.Count; ++i)
        {
            int tag = GetWeaponTagImp(backupWeapons[i].itemID);
            tags.Add(tag);
        }

        return tags;
    }

    public List<int> GetBackupWeaponTypes()
    {
        List<int> types = new List<int>();
        for(int i = 0; i < backupWeapons.Count; ++i)
        {
            int type = GetWeaponTypeImp(backupWeapons[i].itemID);
            types.Add( type); 
        }

        return types;
    }
    public void GetWeaponTagAndWeaponType(ref int tag,ref int weaponType)
    {
        weaponType = 0;
        tag = 0;
        if (currentWeapon == null)
            return;
        var itemID = currentWeapon.itemID;
        GetWeaponTagAndWeaponTypImp(itemID,ref tag, ref weaponType);
    }
    public void GetWeaponTagAndWeaponTypImp(int itemID,ref int tag ,ref int weaponType)
    {
        weaponType = 0;
        tag = 0;
        var itemData = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(itemID);
        if (itemData != null)
        {
            var slot = (EEquipWearSlotType)itemData.SubType;
            if (slot == EEquipWearSlotType.EquipWeapon)
            {
                tag = (int)itemData.Tag;
                weaponType = (int)itemData.ThirdType;
            }
        }
    }
    public void GetBackupWeaponTypesAndTags(List<int> weaponTypes,List<int> tags)
    {
        weaponTypes.Clear();
        tags.Clear();
        for (int i = 0; i < backupWeapons.Count; ++i)
        {
            int tag = 0;
            int weaponType = 0;
            GetWeaponTagAndWeaponTypImp(backupWeapons[i].itemID,ref tag,ref weaponType);
            tags.Add(tag);
            weaponTypes.Add(weaponType);
        }
    }
    #endregion
    #region 切换装备
    /// <summary>
    /// 添加装备
    /// </summary>
    public void AddEquip(ItemProperty item)
    {
        itemProperties.Add(item);
    }

    /// <summary>
    /// 移除装备
    /// </summary>
    public void RemoveEquip(ItemProperty item)
    {
        if (item == null)
            return;
        itemProperties.Remove(item);
    }

    /// <summary>
    /// 通过GUID获取已穿戴的装备
    /// </summary>
    public ItemProperty GetWearEquipByGUID(ulong guid)
    {
        for(int i=0;i< itemProperties.Count; i++)
        {
            if (itemProperties[i].guid == guid)
                return itemProperties[i];
        }
        return null;
    }
    #endregion
}
