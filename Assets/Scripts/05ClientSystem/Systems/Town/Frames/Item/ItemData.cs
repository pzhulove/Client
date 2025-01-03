using UnityEngine;
using System;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using System.ComponentModel;
using EItemType = ProtoTable.ItemTable.eType;
using EItemQuality = ProtoTable.ItemTable.eColor;
using EItemBindAttr = ProtoTable.ItemTable.eOwner;
using EITemThirdType = ProtoTable.ItemTable.eThirdType;
using EItemUseType = ProtoTable.ItemTable.eCanUse;
using EItemUseLimitType = ProtoTable.ItemTable.eUseLimiteType;
using ProtoTable;
using Protocol;

namespace GameClient
{
    public enum EEquipAttributeType
    {
        EAT_ENCHANTMENTCARD = 0,//附魔卡
        EAT_BEAD,               //宝珠
        EAT_INSCRIPTION,       //铭文
        EAT_BEADADDITIVE        //宝珠附加属性
    }
    public enum EEquipType
    {
        ET_COMMON = 0, //普通（可强化的装备）
        ET_BREATH, //带气息（未激活的装备）
        ET_REDMARK,//红字（已激活的装备可增幅）
    }

    /// <summary>
    /// 装备增幅属性类型
    /// </summary>
    public enum EGrowthAttrType
    {
        GAT_NONE = 0,
        GAT_STRENGTH,       //力量
        GAT_INTELLIGENCE,   //智力
        GAT_STAMINA,        //体力
        GAT_SPIRIT,         //精神
    }
    public enum EquipStrMod
    {
        WpStrenthMod,   //武器强化系数
        WpColorQaMod,   //武器品质系数a
        WpColorQbMod,   //武器品质系数b
        ArmStrenthMod,  //防具强化系数
        ArmColorQaMod,  //防具品质系数a
        ArmColorQbMod,  //防具品质系数b
        JewStrenthMod,  //首饰强化系数
        JewColorQaMod,  //首饰品质系数a
        JewColorQbMod,  //首饰品质系数b
    }

    public enum EPackageType
    {
        Invalid = 0,    // 无效

        Equip = 1,      // 装备包裹
        Material,       // 材料包裹
        Consumable,     // 消耗品包裹
        Task,           // 任务道具包裹
        Fashion,        // 时装包裹
        WearEquip,      // 已装备的包裹
        WearFashion,    // 已装备的时装包裹
        Storage,        // 仓库（账号仓库)
        EnergyStone,    // 能量石
        Title = 10,     // 称号
        Pet,            // 宠物包裹
        EquipRecovery,  // 装备回收

        RoleStorage,   // 角色仓库

        Inscription,    // 铭文包裹 

        Bxy,   // 辟邪玉包裹
        Sinan,  // 司南包裹

        Count,
    }

    public enum EServerProp
    {
        IRP_NONE = 0,

        [MapEnum(EEquipProp.Strenth)]
        IRP_STR,        //力量
        [MapEnum(EEquipProp.Stamina)]
        IRP_STA,        //体力
        [MapEnum(EEquipProp.Intellect)]
        IRP_INT,        //智力
        [MapEnum(EEquipProp.Spirit)]
        IRP_SPR,        //精神
        [MapEnum(EEquipProp.HPMax)]
        IRP_HPMAX,      //生命最大值
        [MapEnum(EEquipProp.MPMax)]
        IRP_MPMAX,      //魔力最大值
        [MapEnum(EEquipProp.HPRecover)]
        IRP_HPREC,      //生命恢复
        [MapEnum(EEquipProp.MPRecover)]
        IRP_MPREC,      //魔力恢复
        [MapEnum(EEquipProp.HitRate)]
        IRP_HIT,        //命中率
        [MapEnum(EEquipProp.AvoidRate)]
        IRP_DEX,        //回避率
        [MapEnum(EEquipProp.PhysicCritRate)]
        IRP_PHYCRT,     //物理暴击
        [MapEnum(EEquipProp.MagicCritRate)]
        IRP_MGCCRT,     //魔法暴击
        [MapEnum(EEquipProp.AttackSpeedRate)]
        IRP_ATKSPD,     //攻速
        [MapEnum(EEquipProp.FireSpeedRate)]
        IRP_RDYSPD,     //施放速度
        [MapEnum(EEquipProp.MoveSpeedRate)]
        IRP_MOVSPD,     //移动速度
        [MapEnum(EEquipProp.Jump)]
        IRP_JUMP,       //跳跃力
        [MapEnum(EEquipProp.Spasticity)]
        IRP_HITREC,     //硬直

        IRP_LIGHT = 22,       //光属性攻击
        IRP_FIRE = 23,      //火属性攻击
        IRP_ICE = 24,      //冰属性攻击
        IRP_DARK = 25,      //暗属性攻击

        [MapEnum(EEquipProp.LightAttack)]
        IRP_LIGHT_ATK = 26, //光属性强化
        [MapEnum(EEquipProp.FireAttack)]
        IRP_FIRE_ATK = 27,  //火属性强化
        [MapEnum(EEquipProp.IceAttack)]
        IRP_ICE_ATK = 28,  //冰属性强化
        [MapEnum(EEquipProp.DarkAttack)]
        IRP_DARK_ATK = 29,  //暗属性强化
        [MapEnum(EEquipProp.LightDefence)]
        IRP_LIGHT_DEF = 30, //光属性抗性
        [MapEnum(EEquipProp.FireDefence)]
        IRP_FIRE_DEF = 31,  //火属性抗性
        [MapEnum(EEquipProp.IceDefence)]
        IRP_ICE_DEF = 32,  //冰属性抗性
        [MapEnum(EEquipProp.DarkDefence)]
        IRP_DARK_DEF = 33,  //暗属性抗性
        [MapEnum(EEquipProp.abnormalResist1)]
        IRP_GDKX = 34,    //感电抗性
        [MapEnum(EEquipProp.abnormalResist2)]
        IRP_CXKX = 35,    //出血抗性
        [MapEnum(EEquipProp.abnormalResist3)]
        IRP_ZSKX = 36,    //灼烧抗性
        [MapEnum(EEquipProp.abnormalResist4)]
        IRP_ZDKX = 37,    //中毒抗性
        [MapEnum(EEquipProp.abnormalResist5)]
        IRP_SMKX = 38,    //失明抗性
        [MapEnum(EEquipProp.abnormalResist6)]
        IRP_XYKX = 39,    //眩晕抗性
        [MapEnum(EEquipProp.abnormalResist7)]
        IRP_SHKX = 40,    //石化抗性
        [MapEnum(EEquipProp.abnormalResist8)]
        IRP_BDKX = 41,    //冰冻抗性
        [MapEnum(EEquipProp.abnormalResist9)]
        IRP_SLKX = 42,    //睡眠抗性
        [MapEnum(EEquipProp.abnormalResist10)]
        IRP_HLKX = 43,    //混乱抗性
        [MapEnum(EEquipProp.abnormalResist11)]
        IRP_SFKX = 44,    //束缚抗性
        [MapEnum(EEquipProp.abnormalResist12)]
        IRP_JSKX = 45,    //减速抗性
        [MapEnum(EEquipProp.abnormalResist13)]
        IRP_ZZKX = 46,    //诅咒抗性
        [MapEnum(EEquipProp.AbormalResist)]
        IRP_YKXZ = 47,    //所有异常抗性
        [MapEnum(EEquipProp.Independence)]
        IRP_INDEPENDENCE = 48,//独立攻击力

        IRP_MAX,
    }

    public class MapEnum : Attribute
    {
        public EEquipProp Prop;
        public MapEnum(EEquipProp prop)
        {
            this.Prop = prop;
        }
    }

    //镶嵌宝珠数据
    public class PrecBead
    {
        public int index;
        public int type;
        public int preciousBeadId;
        public int randomBuffId;
        public int pickNumber;//宝珠摘取次数
        public int beadReplaceNumber;//宝珠置换次数
    }

    /// <summary>
    /// 附魔卡数据
    /// </summary>
    public class PrecEnchantmentCard
    {
        public int iEnchantmentCardID;
        public int iEnchantmentCardLevel;
    }

    public enum EEquipProp
    {
        Invalid = -1,

        /// <summary>
        /// 物理攻击
        /// </summary>
        [PropAttribute("物理攻击", "Atk")]
        PhysicsAttack,

        /// <summary>
        /// 魔法攻击
        /// </summary>
        [PropAttribute("魔法攻击", "MagicAtk")]
        MagicAttack,

        /// <summary>
        /// 物理防御
        /// </summary>
        [PropAttribute("物理防御", "Def")]
        PhysicsDefense,

        /// <summary>
        /// 魔法防御
        /// </summary>
        [PropAttribute("魔法防御", "MagicDef")]
        MagicDefense,

        /// <summary>
        /// 力量
        /// </summary>
        [PropAttribute("力量", "Strenth")]
        Strenth,

        /// <summary>
        /// 智力
        /// </summary>
        [PropAttribute("智力", "Intellect")]
        Intellect,

        /// <summary>
        /// 精神
        /// </summary>
        [PropAttribute("精神", "Spirit")]
        Spirit,

        /// <summary>
        /// 体力
        /// </summary>
        [PropAttribute("体力", "Stamina")]
        Stamina,

        /// <summary>
        /// 物理技能耗蓝
        /// </summary>
        [PropAttribute("物理技能耗蓝", "PhySkillMp", true)]
        PhysicsSkillMPChange,

        /// <summary>
        /// 物理技能冷却
        /// </summary>
        [PropAttribute("物理技能冷却", "PhySkillCd", true)]
        PhysicsSkillCDChange,

        /// <summary>
        /// 魔法技能耗蓝
        /// </summary>
        [PropAttribute("魔法技能耗蓝", "MagSkillMp", true)]
        MagicSkillMPChange,

        /// <summary>
        /// 魔法技能冷却
        /// </summary>
        [PropAttribute("魔法技能冷却", "MagSkillCd", true)]
        MagicSkillCDChange,

        /// <summary>
        /// 生命最大值
        /// </summary>
        [PropAttribute("生命最大值", "HPMax")]
        HPMax,

        /// <summary>
        /// 魔力最大值
        /// </summary>
        [PropAttribute("魔力最大值", "MPMax")]
        MPMax,

        /// <summary>
        /// 生命恢复
        /// </summary>
        [PropAttribute("生命恢复", "HPRecover")]
        HPRecover,

        /// <summary>
        /// 魔力恢复
        /// </summary>
        [PropAttribute("魔力恢复", "MPRecover")]
        MPRecover,

        /// <summary>
        /// 攻击速度
        /// </summary>
        [PropAttribute("攻速等级", "AttackSpeedRate")]
        AttackSpeedRate,

        /// <summary>
        /// 施放速度
        /// </summary>
        [PropAttribute("施法等级", "FireSpeedRate")]
        FireSpeedRate,

        /// <summary>
        /// 移动速度
        /// </summary>
        [PropAttribute("移速等级", "MoveSpeedRate")]
        MoveSpeedRate,

        /// <summary>
        /// 所有异常状态抗性
        /// </summary>
        [PropAttribute("所有异常状态抗性", "AbormalResist")]
        AbormalResist,

        /// <summary>
        /// 各种异常状态抗性
        /// </summary>
        [PropAttribute("各种异常状态抗性", "AbormalResists")]
        AbormalResists,

        /// <summary>
        /// 攻击附带属性
        /// </summary>
        [PropAttribute("攻击附带属性", "Elements")]
        Elements,

        /// <summary>
        /// 光属性强化
        /// </summary>
        [PropAttribute("光属性强化", "LightAttack")]
        LightAttack,

        /// <summary>
        /// 火属性强化
        /// </summary>
        [PropAttribute("火属性强化", "FireAttack")]
        FireAttack,

        /// <summary>
        /// 冰属性强化
        /// </summary>
        [PropAttribute("冰属性强化", "IceAttack")]
        IceAttack,

        /// <summary>
        /// 暗属性强化
        /// </summary>
        [PropAttribute("暗属性强化", "DarkAttack")]
        DarkAttack,

        /// <summary>
        /// 光属性抗性
        /// </summary>
        [PropAttribute("光属性抗性", "LightDefence")]
        LightDefence,

        /// <summary>
        /// 火属性抗性
        /// </summary>
        [PropAttribute("火属性抗性", "FireDefence")]
        FireDefence,

        /// <summary>
        /// 冰属性抗性
        /// </summary>
        [PropAttribute("冰属性抗性", "IceDefence")]
        IceDefence,

        /// <summary>
        /// 暗属性抗性
        /// </summary>
        [PropAttribute("暗属性抗性", "DarkDefence")]
        DarkDefence,

        /// <summary>
        /// 命中率
        /// </summary>
        [PropAttribute("命中等级", "HitRate")]
        HitRate,

        /// <summary>
        /// 闪避率
        /// </summary>
        [PropAttribute("闪避等级", "AvoidRate")]
        AvoidRate,

        /// <summary>
        /// 物理暴击率
        /// </summary>
        [PropAttribute("物暴等级", "PhysicCrit")]
        PhysicCritRate,

        /// <summary>
        /// 魔法暴击率
        /// </summary>
        [PropAttribute("魔暴等级", "MagicCrit")]
        MagicCritRate,

        /// <summary>
        /// 硬直
        /// </summary>
        [PropAttribute("硬直", "Spasticity")]
        Spasticity,

        /// <summary>
        /// 跳跃力
        /// </summary>
        [PropAttribute("跳跃力", "Jump")]
        Jump,

        /// <summary>
        /// 城镇移动速度
        /// </summary>
        [PropAttribute("城镇移速等级", "TownMoveSpeedRate")]
        TownMoveSpeedRate,

        /// <summary>
        /// 无视防御物攻
        /// </summary>
        [PropAttribute("无视防御物攻", "")]
        IgnorePhysicsAttackRate,

        /// <summary>
        /// 无视防御魔攻
        /// </summary>
        [PropAttribute("无视防御魔攻", "")]
        IgnoreMagicAttackRate,

        /// <summary>
        /// 无视防御物攻
        /// </summary>
        [PropAttribute("无视防御物攻","")]
        IgnorePhysicsAttack,

        /// <summary>
        /// 无视防御魔攻
        /// </summary>
        [PropAttribute("无视防御魔攻", "")]
        IgnoreMagicAttack,

        /// <summary>
        /// 物理伤害减少
        /// </summary>
        [PropAttribute("物理伤害减少", "", true)]
        IgnorePhysicsDefenseRate,

        /// <summary>
        /// 魔法伤害减少
        /// </summary>
        [PropAttribute("魔法伤害减少", "", true)]
        IgnoreMagicDefenseRate,

        /// <summary>
        /// 物理伤害减少
        /// </summary>
        [PropAttribute("物理伤害减少", "",true)]
        IgnorePhysicsDefense,

        /// <summary>
        /// 魔法伤害减少
        /// </summary>
        [PropAttribute("魔法伤害减少", "", true)]
        IgnoreMagicDefense,

        /// <summary>
        /// 感电抗性
        /// </summary>
        [PropAttribute("感电抗性", "abnormalResist1")]
        abnormalResist1,

        /// <summary>
        /// 出血抗性
        /// </summary>
        [PropAttribute("出血抗性", "abnormalResist2")]
        abnormalResist2,

        /// <summary>
        /// 灼烧抗性
        /// </summary>
        [PropAttribute("灼烧抗性", "abnormalResist3")]
        abnormalResist3,

        /// <summary>
        /// 中毒抗性
        /// </summary>
        [PropAttribute("中毒抗性", "abnormalResist4")]
        abnormalResist4,

        /// <summary>
        /// 失明抗性
        /// </summary>
        [PropAttribute("失明抗性", "abnormalResist5")]
        abnormalResist5,

        /// <summary>
        /// 眩晕抗性
        /// </summary>
        [PropAttribute("眩晕抗性", "abnormalResist6")]
        abnormalResist6,

        /// <summary>
        /// 石化抗性
        /// </summary>
        [PropAttribute("石化抗性", "abnormalResist7")]
        abnormalResist7,

        /// <summary>
        /// 冰冻抗性
        /// </summary>
        [PropAttribute("冰冻抗性", "abnormalResist8")]
        abnormalResist8,

        /// <summary>
        /// 睡眠抗性
        /// </summary>
        [PropAttribute("睡眠抗性", "abnormalResist9")]
        abnormalResist9,

        /// <summary>
        /// 混乱抗性
        /// </summary>
        [PropAttribute("混乱抗性", "abnormalResist10")]
        abnormalResist10,

        /// <summary>
        /// 束缚抗性
        /// </summary>
        [PropAttribute("束缚抗性", "abnormalResist11")]
        abnormalResist11,

        /// <summary>
        /// 减速抗性
        /// </summary>
        [PropAttribute("减速抗性", "abnormalResist12")]
        abnormalResist12,

        /// <summary>
        /// 诅咒抗性
        /// </summary>
        [PropAttribute("诅咒抗性", "abnormalResist13")]
        abnormalResist13,

        /// <summary>
        /// 侵蚀抗性
        /// </summary>
        [PropAttribute("侵蚀抗性", "ResistMagic")]
        resistMagic,

        /// <summary>
        /// 固定攻击(独立攻击力)
        /// </summary>
        [PropAttribute("固定攻击", "Independce")]
        Independence,

        /// <summary>
        /// 强化（激化）固定攻击
        /// </summary>
        [PropAttribute("固定攻击", "")]
        IngoreIndependence,

        Count,
    }

    public class PropAttribute : Attribute
    {
        public PropAttribute(string desc,string fieldName,bool bInverse = false)
        {
            this.desc = desc;
            this.fieldName = fieldName;
            this.bInverse = bInverse;
        }
        public string desc;
        public string fieldName;
        public bool bInverse;
    }

    public class MapIndex : Attribute
    {
        public int Index;
        public MapIndex(int index)
        {
            this.Index = index;
        }
    }

    public enum EEquipWearSlotType
    {
        EquipInvalid = 0,

        [MapIndex(5)]
        [Description("item_slot_equip_weapon")]
        EquipWeapon,

        [MapIndex(0)]
        [Description("item_slot_equip_head")]
        EquipHead,

        [MapIndex(1)]
        [Description("item_slot_equip_chest")]
        EquipChest,

        [MapIndex(2)]
        [Description("item_slot_equip_belt")]
        EquipBelt,

        [MapIndex(3)]
        [Description("item_slot_equip_leg")]
        EquipLeg,

        [MapIndex(4)]
        [Description("item_slot_equip_boot")]
        EquipBoot,

        [MapIndex(6)]
        [Description("item_slot_equip_ring")]
        EquipRing,

        [MapIndex(7)]
        [Description("item_slot_equip_necklase")]
        EquipNecklase,

        [MapIndex(8)]
        [Description("item_slot_equip_bracelet")]
        Equipbracelet,

        [MapIndex(9)]
        [Description("item_slot_equip_title")]
        Equiptitle,

        [MapIndex(14)]
        [Description("item_slot_secondequip_weapon")]
        SecondEquipWeapon,

        // 辟邪玉[披风]
        [MapIndex(10)]
        [Description("item_slot_equip_cloak")]
        // Equipcloak,
        Equipcloak = 199,
        
        [MapIndex(11)]
        [Description("item_slot_equip_shirt")]
        Equipshirt,
        [MapIndex(12)]
        [Description("item_slot_equip_glove")]
        Equipglove,
        [MapIndex(13)]
        [Description("item_slot_equip_accessories")]
        Equipaccessories,

        [MapIndex(98)]
        [Description("item_slot_equip_assist1")]
        Equipassist1 = 99,   

        [MapIndex(99)]
        [Description("item_slot_equip_assist2")]
        Equipassist2 = 100,

        [MapIndex(100)]
        [Description("item_slot_equip_assist3")]
        Equipassist3 = 101,
        // EquipMax,
        EquipMax = 999,
    }

public enum EFashionWearNewSlotType
    {
        Invalid = 0,

        [MapIndex(0)]
        [Description("item_slot_fashion_head")]
        Head,       //头部

        [MapIndex(1)]
        [Description("item_slot_fashion_upper_body")]
        UpperBody,  //衣服


        [MapIndex(2)]
        [Description("item_slot_fashion_chest")]
        Chest,  //头饰(策划要求ui上显示成头饰) add by qxy 2019-03-19

        [MapIndex(3)]
        [Description("item_slot_fashion_waist")]
        Waist,  //首饰(策划要求ui上显示成首饰) add by qxy 2019-03-19


        [MapIndex(4)]
        [Description("item_slot_fashion_lower_body")]
        LowerBody,  //裤子

        [MapIndex(5)]
        [Description("item_slot_fashion_weapon")]
        Weapon, //武器

        [MapIndex(6)]
        [Description("item_slot_fashion_halo")]
        Halo,   //翅膀

        [MapIndex(7)]
        [Description("item_slot_fashion_auras")]
        Auras,   //光环

        Max,
    }

    public enum EFashionWearSlotType
    {
        Invalid = 0,

        [MapIndex(2)]
        [Description("item_slot_fashion_halo")]
        Halo,

        [MapIndex(3)]
        [Description("item_slot_fashion_head")]
        Head,       // 头部(原先叫头饰) by WangBo 2019.03.19

        [MapIndex(1)]
        [Description("item_slot_fashion_waist")]
        Waist,    // 腰饰(老时装还是腰饰没有外观，新时装用来表示首饰并且有外观) by WangBo 2019.03.19

        [MapIndex(4)]
        [Description("item_slot_fashion_upper_body")]
        UpperBody,

        [MapIndex(5)]
        [Description("item_slot_fashion_lower_body")]
        LowerBody,

        [MapIndex(0)]
        [Description("item_slot_fashion_chest")]
        Chest,    // 胸饰(老时装还是胸饰没有外观，新时装用来表示头饰并且有外观) by WangBo 2019.03.19

        [MapIndex(6)]
        [Description("item_slot_fashion_weapon")]
        Weapon, //武器

        [MapIndex(7)]
        [Description("item_slot_fashion_auras")]
        Auras, //光环

        Max,
    }

    #region 给凯哥用的结构
    public class EquipPropAttribute : Attribute
    {
        public EEquipProp Prop;
        public EquipPropAttribute(EEquipProp prop)
        {
            this.Prop = prop;
        }
    }

    public class ItemProperty
    {
        public ulong guid;
        //道具ID
        public CrypticInt32 itemID;
        //强化等级
        public int strengthen;
        //装备包裹槽位
        public int grid;

        public Dictionary<int, int> triggerBuffCDRemain = null;

        [EquipProp(EEquipProp.HPMax)]
        public CrypticInt32 maxHp;

        [EquipProp(EEquipProp.MPMax)]
        public CrypticInt32 maxMp;

        [EquipProp(EEquipProp.HPRecover)]
        public CrypticInt32 hpRecover;

        [EquipProp(EEquipProp.MPRecover)]
        public CrypticInt32 mpRecover;

        [EquipProp(EEquipProp.Stamina)]
        public CrypticInt32 baseSta;

        [EquipProp(EEquipProp.Strenth)]
        public CrypticInt32 baseAtk;

        [EquipProp(EEquipProp.Intellect)]
        public CrypticInt32 baseInt;

        [EquipProp(EEquipProp.Spirit)]
        public CrypticInt32 baseSpr;

        [EquipProp(EEquipProp.PhysicsAttack)]
        public CrypticInt32 attack;

        [EquipProp(EEquipProp.MagicAttack)]
        public CrypticInt32 magicAttack;

        [EquipProp(EEquipProp.PhysicsDefense)]
        public CrypticInt32 defence;

        [EquipProp(EEquipProp.MagicDefense)]
        public CrypticInt32 magicDefence;

        [EquipProp(EEquipProp.AttackSpeedRate)]
        public CrypticInt32 attackSpeed;

        [EquipProp(EEquipProp.FireSpeedRate)]
        public CrypticInt32 spellSpeed;

        [EquipProp(EEquipProp.MoveSpeedRate)]
        public CrypticInt32 moveSpeed;

        [EquipProp(EEquipProp.PhysicCritRate)]
        public CrypticInt32 ciriticalAttack;

        [EquipProp(EEquipProp.MagicCritRate)]
        public CrypticInt32 ciriticalMagicAttack;

        [EquipProp(EEquipProp.HitRate)]
        public CrypticInt32 dex;

        [EquipProp(EEquipProp.AvoidRate)]
        public CrypticInt32 dodge;

        //[EquipPropAttribute(EEquipProp.Invalid)]
        public CrypticInt32 frozen;                  //僵值

        [EquipProp(EEquipProp.Spasticity)]
        public CrypticInt32 hard;

        [EquipPropAttribute(EEquipProp.AbormalResist)]
        public CrypticInt32 abnormalResist;

        [EquipProp(EEquipProp.Jump)]
        public CrypticInt32 jumpForce;

        [EquipProp(EEquipProp.PhysicsSkillMPChange)]
        public CrypticInt32 mpCostReduceRate;

        [EquipProp(EEquipProp.MagicSkillMPChange)]
        public CrypticInt32 mpCostReduceRateMagic;

        [EquipProp(EEquipProp.PhysicsSkillCDChange)]
        public CrypticInt32 cdReduceRate;

        [EquipProp(EEquipProp.MagicSkillCDChange)]
        public CrypticInt32 cdReduceRateMagic;

        [EquipProp(EEquipProp.IgnorePhysicsAttackRate)]
        public CrypticInt32 attackAddRate;

        [EquipProp(EEquipProp.IgnoreMagicAttackRate)]
        public CrypticInt32 magicAttackAddRate;

        [EquipProp(EEquipProp.IgnorePhysicsAttack)]
        public CrypticInt32 ignoreDefAttackAdd;

        [EquipProp(EEquipProp.IgnoreMagicAttack)]
        public CrypticInt32 ignoreDefMagicAttackAdd;

        [EquipProp(EEquipProp.IgnorePhysicsDefenseRate)]
        public CrypticInt32 attackReduceRate;

        [EquipProp(EEquipProp.IgnoreMagicDefenseRate)]
        public CrypticInt32 magicAttackReduceRate;

        [EquipProp(EEquipProp.IgnorePhysicsDefense)]
        public CrypticInt32 attackReduceFix;

        [EquipProp(EEquipProp.IgnoreMagicDefense)]
        public CrypticInt32 magicAttackReduceFix;

        [EquipProp(EEquipProp.Independence)]
        public CrypticInt32 independence;

        [EquipProp(EEquipProp.IngoreIndependence)]
        public CrypticInt32 ingoreIndependence;

        public IList<int> attachBuffIDs;
        public IList<int> attachMechanismIDs;

        public IList<int> attachPVPBuffIDs;
        public IList<int> attachPVPMechanismIDs;

        /// <summary>
        /// 武器附带攻击属性
        /// </summary>
        public int[] magicElements = null;//new int[(int)MagicElementType.MAX];
        /// <summary>
        /// 属强
        /// </summary>
        public CrypticInt32[] magicElementsAttack = new CrypticInt32[(int)(MagicElementType.MAX)];
        /// <summary>
        /// 属抗
        /// </summary>
        public CrypticInt32[] magicElementsDefence = new CrypticInt32[(int)(MagicElementType.MAX)];
        /// <summary>
        /// 异抗,13种
        /// </summary>
        public CrypticInt32[] abnormalResists = new CrypticInt32[Global.ABNORMAL_COUNT];
        /// <summary>
        /// 抗魔值
        /// </summary>
        public CrypticInt32 resistMagic;

        public void SaveTriggerBuffCDRemain(int buffInfoID, int cdRemain)
        {
            if (triggerBuffCDRemain == null)
                triggerBuffCDRemain = new Dictionary<int, int>();
            if (!triggerBuffCDRemain.ContainsKey(buffInfoID))
            {
                triggerBuffCDRemain.Add(buffInfoID, cdRemain);
                //Logger.LogErrorFormat("record cd remain:{0} {1}", buffInfoID, cdRemain);
            }
            else
            {
                //Logger.LogErrorFormat("record cd remain:{0} {1}", buffInfoID, cdRemain);
                triggerBuffCDRemain[buffInfoID] = cdRemain;
            }
                
        }

        public int GetTriggerBuffCDRemain(int buffInfoID)
        {
            if (triggerBuffCDRemain == null)
                return 0;

            if (triggerBuffCDRemain.ContainsKey(buffInfoID))
            {
                int tmp = triggerBuffCDRemain[buffInfoID];
                triggerBuffCDRemain.Remove(buffInfoID);
                //Logger.LogErrorFormat("get cd remain:{0} {1}", buffInfoID, tmp);
                return tmp;
            }
                 

            return 0;
        }

        public void DebugPrint()
		{
			Utility.PrintType(typeof(ItemProperty), this);
		}

		public int GetValue(AttributeType attributeType){
			int value=0;
			switch(attributeType)
			{
			case AttributeType.maxHp:
				value = this.maxHp;
				break;
            case AttributeType.baseIndependence:
                value = this.independence;
                break;
            case AttributeType.ingoreIndependence:
                value = this.ingoreIndependence;
                break;
			case AttributeType.maxMp:
				value = this.maxMp;
				break;
			case AttributeType.hpRecover:
				value = this.hpRecover;
				break;
			case AttributeType.mpRecover:
				value = this.mpRecover;
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
/*			case AttributeType.criticalPercent:
				value = this.criticalPercent;
				break;*/
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
/*			case AttributeType.defenceAddRate:
				value = this.defenceAddRate;
				break;
			case AttributeType.magicDefenceAddRate:
				value = this.magicDefenceAddRate;
				break;*/
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
			}
			return value;
		}
    }
    #endregion

    public class RenewalInfo
    {
        public int nDay;
        public int nCostID;
        public int nCostCount;
    }

    // TODO
    public class ItemData:IItemDataModel
    {
        public enum IncomeType
        {
            IT_GOLD = ProtoTable.ItemTable.eSubType.GOLD,
            IT_TICKET = ProtoTable.ItemTable.eSubType.POINT,
            IT_UNCOLOR = 300000106,
            IT_COLOR = 300000105,
            IT_PROTECTED = 200000310,
            IT_DEATHCOIN = 600000004,
            IT_PKCOIN = 600000005,
            IT_GROWTHPROTECTED = 300000207,
        }

        public object userData = null;

        public static int unlimitedUseTimes = 99999999;

        /// <summary>
        /// 表数据是否已初始化
        /// </summary>
        public bool IsTableDataInited = false;

        /// <summary>
        /// GUID
        /// </summary>
        public ulong GUID { get; set; }
        /// <summary>
        /// 表ID
        /// </summary>
        public int TableID {
            get
            {
                if (this.mTableData != null)
                {
                    return this.mTableData.ID;
                }

                return 0;
            }
        }

        /// <summary>
        /// 类型
        /// </summary> 
        public EItemType Type { get; set; }
        /// <summary>
        /// 包裹类型
        /// </summary>
        public EPackageType PackageType { get; set; }
        //背包的格子索引(角色仓库中分页用)
        public int GridIndex;
        /// <summary>
        /// 子类型
        /// </summary>
        public int SubType { get; set; }
        /// <summary>
        /// 三类别
        /// </summary>
        public EITemThirdType ThirdType { get; set; }
        /// <summary>
        /// 可装备的槽位类型
        /// </summary>
        public EEquipWearSlotType EquipWearSlotType { get; set; }
        /// <summary>
        /// 可装备的槽位类型
        /// </summary>
        public EFashionWearSlotType FashionWearSlotType { get; set; }
        /// <summary>
        /// 堆叠数量
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 用做界面显示用
        /// </summary>
        public int ShowCount;

        /// <summary>
        /// 最大可堆叠数量
        /// </summary>
        public int MaxStackCount;
        /// <summary>
        /// 品级（紫色-神器）
        /// </summary>
        public EItemQuality Quality { get; set; }
        /// <summary>
        /// 品级2 对应道具表Color2
        /// </summary>
        public int Quality2;
        /// <summary>
        /// 子品级（上级-68%）
        /// </summary>
        public int SubQuality;

        /// <summary>
        /// 附魔卡阶段
        /// </summary>
        public int EnchantmentCardStage;

        public int ItemTradeNumber;     //在拍卖行中已经交易的次数
        public bool IsTreasure;     //是否为珍品
        public bool IsShowTreasureFlagInTipFrame; //首先是珍品，并且是在拍卖行中，才在TipFrame中的ComItem中显示珍品
        public uint AuctionCoolTimeStamp; //交易之后的冷却时间
        public bool isPVP = false;

        public EEquipType EquipType { get; set; }//装备类型
        public EGrowthAttrType GrowthAttrType; //装备增幅属性类型
        public int GrowthAttrNum; //装备增幅属性值

        /// <summary>
        /// 图标
        /// </summary>
        public string Icon {
            get
            {
                if (this.mTableData != null)
                {
                    return this.mTableData.Icon;
                }
                return "";
            }
        }

        public int EffectType {
			get
			{
                if (this.mTableData == null) {
                    return -1;
                }
				if (this.mTableData.Inlaidhole2 == "" || this.mTableData.Inlaidhole2 == string.Empty || string.IsNullOrEmpty(this.mTableData.Inlaidhole2.Trim()))
				{
                    return -1;
				}
                return int.Parse(this.mTableData.Inlaidhole2);

            }
        }

        /// <summary>
        /// 名字
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 绑定属性
        /// </summary>
        public EItemBindAttr BindAttr { get; set; }
        /// <summary>
        /// 是否封装
        /// </summary>
        public bool Packing { get; set; }
        /// <summary>
        /// 可重新封装的次数
        /// </summary>
        public int RePackTime;
        /// <summary>
        /// 已经封装的次数
        /// </summary>
        public int iPackedTimes;
        /// <summary>
        /// 最大封装次数
        /// </summary>
        public int iMaxPackTime;
        /// <summary>
        /// 使用等级限制
        /// </summary>
        public int LevelLimit { get; set; }
        /// <summary>
        /// 使用最大等级限制
        /// </summary>
        public int MaxLevelLimit;
        /// <summary>
        /// 使用职业限制
        /// </summary>
        public List<int> OccupationLimit;
        /// <summary>
        /// 趣味描述
        /// </summary>
        public string Description
        {
            get
            {
                if (this.mTableData != null)
                {
                    return GetDescriptionOrDoubleCheckDesc(this.mTableData.Description);
                }
                return "";
            }
        }
        /// <summary>
        /// 出处描述
        /// </summary>
        public string SourceDescription
        {
            get
            {
                if (this.mTableData != null)
                {
                    return this.mTableData.ComeDesc;
                }
                return "";
            }
        }
        /// <summary>
        /// 效果描述
        /// </summary>
        public string EffectDescription
        {
            get
            {
                if (this.mTableData != null)
                {
                    return this.mTableData.EffectDescription;
                }
                return "";
            }
        }
        /// <summary>
        /// 使用类型
        /// </summary>
        public EItemUseType UseType;
        public int RecoScore;               //装备回收积分

        /// <summary>
        /// 装备转移石头ID
        /// </summary>
        public int TransferStone
        {
            get; set;
        }

        public bool HasTransfered
        {
            get
            {
                return TransferStone != 0;
            }
        }

        private bool canSell;
        public bool CanSell
        {
            set { canSell = value; }

            get
            {
                //不可出售
                if (canSell == false)
                    return false;
                
                //副武器
                if (isInSidePack == true)
                    return false;

                //在未启用的装备方案中
                if (IsItemInUnUsedEquipPlan == true)
                    return false;

                return true;
            }
        }

        private bool isLease;
        /// <summary>
        /// 是否是租赁装备标识
        /// </summary>
        public bool IsLease
        {
            get { return isLease; }
            set
            {
                isLease = value;
            }
        }

        public bool isInSidePack
        {
            get { return SwitchWeaponDataManager.GetInstance().IsInSidePack(this); }
        }


        private bool _isItemInUnUsedEquipPlan;
        //装备在未启用的装备方案的标志(只对背包中未穿戴的装备有效;穿戴在身上的道具，默认为false)
        public bool IsItemInUnUsedEquipPlan
        {
            set { _isItemInUnUsedEquipPlan = value; }
            get
            {
                //不在装备方案，直接返回false
                if(_isItemInUnUsedEquipPlan == false)
                {
                    return _isItemInUnUsedEquipPlan;
                }
                else
                {
                    //在装备方案中
                    //装备方案没有打开
                    if (EquipPlanUtility.IsEquipPlanOpenedByServer() == false)
                    {
                        return false;
                    }
                    else
                    {
                        return _isItemInUnUsedEquipPlan;
                    }
                }
            }
        }

        /// <summary>
        /// 共享CD组ID
        /// </summary>
        public int CDGroupID { get; set; }
        /// <summary>
        /// CD（固定值）
        /// </summary>
        public int CD;
        /// <summary>
        /// 剩余时长（固定值）
        /// </summary>
        public int FixTimeLeft;
        /// <summary>
        /// 失效时刻（时间戳）
        /// </summary>
        public int DeadTimestamp;
        /// <summary>
        /// 续费信息
        /// </summary>
        public List<RenewalInfo> arrRenewals;
        /// <summary>
        /// 商店货币ID
        /// </summary>
        public int PriceItemID;
        /// <summary>
        /// 商店价格
        /// </summary>
        public int Price;

        private int strengthenLevel;
        /// <summary>
        /// 强化等级
        /// </summary>
        public int StrengthenLevel
        {
            get { return strengthenLevel; }
            set
            {
                strengthenLevel = value;
                //如果是附魔卡
                if (SubType == (int)ItemTable.eSubType.EnchantmentsCard)
                {
                    mPrecEnchantmentCard = new PrecEnchantmentCard
                    {
                        iEnchantmentCardID = 0,
                        iEnchantmentCardLevel = strengthenLevel
                    };
                }
            }
        }

        private bool isTimeLimit = false;
        /// <summary>
        /// 是否显示限时图标，仅礼包道具使用该字段
        /// </summary>
        public bool IsTimeLimit
        {
            get { return isTimeLimit; }
            set { isTimeLimit = value; }
        }

        private bool canDecompse;
        public bool CanDecompose
        {
            set { canDecompse = value; }
            get
            {
                //不可分解
                if (canDecompse == false)
                    return false;

                //副武器
                if (isInSidePack == true)
                    return false;

                //在未启用的装备方案中
                if (IsItemInUnUsedEquipPlan == true)
                    return false;

                return true;
            }
        }
        /// <summary>
        /// 基础攻击速度(千分比)
        /// </summary>
        public int BaseAttackSpeedRate;
        /// <summary>
        /// 所属套装ID
        /// </summary>
        public int SuitID;                  


        /// <summary>
        /// 最终装备评分（新）
        /// </summary>
        public int finalRateScore { set; get; }

        /// <summary>
        /// 使用次数限制类型
        /// </summary>
        public EItemUseLimitType useLimitType;
        /// <summary>
        /// 使用次数限制值
        /// </summary>
        public int useLimitValue;
        /// <summary>
        /// 礼包ID
        /// </summary>
        public int PackID;                  

        /// <summary>
        /// 如果物品在使用时需要弹出一个二次确认框,则此string有内容.
        /// </summary>
        public string DoubleCheckWindowDesc {
            get
            {
                if (this.mTableData != null)
                { 
                    return GetDescriptionOrDoubleCheckDesc(this.mTableData.doubleCheckDesc);
                }

                return null;
            }

        }

        /// <summary>
        /// 新需求  消耗道具描述显示经验值
        /// </summary>
        /// <param name="sDesc"></param>
        /// <returns></returns>
        private string GetDescriptionOrDoubleCheckDesc(string sDesc)
        {
            //新需求 显示经验数量
            if (mTableData.DescriptionLink != 0)
            {
                string desc = "";
                int expCount = 0;
                var levelAdapterTable = TableManager.GetInstance().GetTableItem<LevelAdapterTable>(mTableData.DescriptionLink);
                if (levelAdapterTable != null)
                {
                    PropertyInfo info = levelAdapterTable.GetType().GetProperty(string.Format("Level{0}", PlayerBaseData.GetInstance().Level), (BindingFlags.Instance | BindingFlags.Public));
                    if (info != null)
                    {
                        expCount = (int)info.GetValue(levelAdapterTable, null);
                    }
                    desc = string.Format(sDesc, expCount);
                }

                return desc;
            }

            return sDesc;
        }

        public bool bLocked { get; set; }
        public bool bFashionItemLocked { get; set; } // 时装锁

        public void RefreshRateScore()
		{
            if (SubType == (int)ItemTable.eSubType.WEAPON)
            {
                finalRateScore = CalculateRateScore();
            }
            else
            {
                if (GUID == 0 && TableID != 0)
                {
                    finalRateScore = CalculateRateScoreNew();
                }
            }
        }

        /// <summary>
        /// 冒险团评分规则计算装备评分
        /// </summary>
        /// <param name="itemData"></param>
        /// <returns></returns>
       public int CalculateRateScoreNew()
        {
            int itemScore = 0;
            int baseScore = 0;

            switch (Type)
            {
                case EItemType.EQUIP:

                    if (SubType == (int)ItemTable.eSubType.ST_BXY_EQUIP)
                    {
                        break;
                    }

                    EquipBaseScoreModTable mEquipBaseScoreModTable = ItemDataManager.GetInstance().GetEquipBaseScoreModTable((int)SubType, (int)Quality);
                    if (mEquipBaseScoreModTable == null)
                    {
                        Logger.LogErrorFormat("calu item score err,EquipBaseScoreModTable not find subtype:{0},quality:{1}", SubType, Quality);
                        return 0;
                    }

                    //攻击力评分
                    int attackScoreData = ItemDataManager.GetInstance()._GetDataByType(EquipScoreTable.eType.ATTACK);
                    int attackScore = (int)((LevelLimit * mEquipBaseScoreModTable.AttrMod2 / 1000f + mEquipBaseScoreModTable.AttrMod1 / 1000f) * attackScoreData / 1000f);

                    //四维评分
                    int powerScoreData = ItemDataManager.GetInstance()._GetDataByType(EquipScoreTable.eType.POWER);
                    int powerScore = (int)((mEquipBaseScoreModTable.PowerMod1 / 1000f + Mathf.Ceil(LevelLimit / 5f) * mEquipBaseScoreModTable.PowerMod2 / 1000f + LevelLimit % 5 * mEquipBaseScoreModTable.PowerMod3 / 1000f) * powerScoreData / 1000f);

                    //装备防御评分
                    int reduceScoreData = ItemDataManager.GetInstance()._GetDataByType(EquipScoreTable.eType.REDUCE);
                    int eqDefScore = (int)((LevelLimit + mEquipBaseScoreModTable.DefMod1 / 1000f) * mEquipBaseScoreModTable.DefMod2 / 1000f * reduceScoreData / 1000f);

                    //辅助装备强化评分
                    int assistEqStrengScore = 0;
                    if ((SubType == (int)ItemTable.eSubType.ST_ASSIST_EQUIP
                    || SubType == (int)ItemTable.eSubType.ST_MAGICSTONE_EQUIP
                    || SubType == (int)ItemTable.eSubType.ST_EARRINGS_EQUIP) && strengthenLevel > 0)
                    {
                        AssistEqStrengFouerDimAddTable mAssistEqStrengFouerDimAddTable = null;
                        Dictionary<int, object> dicts = TableManager.instance.GetTable<AssistEqStrengFouerDimAddTable>();
                        if (dicts != null)
                        {
                            var iter = dicts.GetEnumerator();
                            while (iter.MoveNext())
                            {
                                AssistEqStrengFouerDimAddTable adt = iter.Current.Value as AssistEqStrengFouerDimAddTable;
                                if (adt == null)
                                {
                                    continue;
                                }

                                if (adt.Strengthen != strengthenLevel)
                                {
                                    continue;
                                }

                                if ((int)adt.Color != (int)Quality)
                                {
                                    continue;
                                }

                                if (adt.Color2 == TableData.Color2)
                                {
                                    continue;
                                }

                                if (adt.Lv != LevelLimit)
                                {
                                    continue;
                                }

                                mAssistEqStrengFouerDimAddTable = adt;
                                break;
                            }

                            if (mAssistEqStrengFouerDimAddTable != null)
                            {
                                assistEqStrengScore = mAssistEqStrengFouerDimAddTable.EqScore;
                            }
                        }
                    }

                    //增幅评分
                    int enhanceEqScore = 0;
                    if (EquipType == EEquipType.ET_REDMARK)
                    {
                        EquipEnhanceAttributeTable mEquipEnhanceAttributeTable = null;
                        Dictionary<int, object> dicts1 = TableManager.instance.GetTable<EquipEnhanceAttributeTable>();
                        if (dicts1 != null)
                        {
                            var iter = dicts1.GetEnumerator();
                            while (iter.MoveNext())
                            {
                                EquipEnhanceAttributeTable adt = iter.Current.Value as EquipEnhanceAttributeTable;
                                if (adt == null)
                                {
                                    continue;
                                }

                                if (adt.EnhanceLevel != strengthenLevel)
                                {
                                    continue;
                                }

                                if (adt.Quality != (int)Quality)
                                {
                                    continue;
                                }

                                if (adt.Level != LevelLimit)
                                {
                                    continue;
                                }

                                mEquipEnhanceAttributeTable = adt;
                                break;
                            }

                            if (mEquipEnhanceAttributeTable != null)
                            {
                                enhanceEqScore = mEquipEnhanceAttributeTable.eqScore;
                            }
                        }
                    }

                    //装备基础评分
                    baseScore = attackScore + powerScore + eqDefScore + assistEqStrengScore + enhanceEqScore;

                    int qualityLvCoefficient = 0;
                    if (SubQuality == 100)
                    {
                        //var systemValueTable = TableManager.GetInstance().GetTableItem<SystemValueTable>(SystemValueTable.eType2.SVT_MAX_QUALITY_LEVEL_SCORE_COEFFICIENT);
                        qualityLvCoefficient = 110;
                    }
                    else
                    {
                        qualityLvCoefficient = SubQuality + 1;
                    }

                    int calcVar1 = 0;
                    if (SubQuality == 100)
                    {
                        calcVar1 = baseScore * qualityLvCoefficient / 100;
                    }
                    else
                    {
                        calcVar1 = baseScore * 20 / 100 * qualityLvCoefficient / 100 + baseScore * 80 / 100;
                    }

                    int eqStrengWsAttackScore = 0;   //装备强化无视攻击评分
                    int eqStrengWsDefScore = 0;      //装备强化无视防御评分
                    int eqStrengDecHurtScore = 0;	//装备强化减伤评分

                    //装备强化无视攻击评分, 装备强化减伤评分(当装备为防具类型和首饰时计算)
                    if (IsDefend() || IsOrnaments())
                    {
                        //装备强化无视攻击评分
                        if (strengthenLevel >= mEquipBaseScoreModTable.StrengthMod2.Count)
                        {

                        }
                        else
                        {
                            int scoreData = ItemDataManager.GetInstance()._GetDataByType(EquipScoreTable.eType.DISPHYREDUCE);
                            eqStrengWsAttackScore = (int)((LevelLimit + mEquipBaseScoreModTable.StrenthQualityMod1 / 1000f) / 8f * (mEquipBaseScoreModTable.StrengthMod2[strengthenLevel] / 1000f) * mEquipBaseScoreModTable.StrenthQualityMod2 / 1000f * scoreData / 1000f);
                        }
                        
                        //装备强化减伤评分
                        if (strengthenLevel >= mEquipBaseScoreModTable.StrengthMod3.Count)
                        {
                           
                        }
                        else
                        {
                            int reduceScoreData2 = ItemDataManager.GetInstance()._GetDataByType(EquipScoreTable.eType.REDUCE);
                            eqStrengDecHurtScore = (int)((200 * LevelLimit * mEquipBaseScoreModTable.StrengthMod3[strengthenLevel] / 1000f / (1 - mEquipBaseScoreModTable.StrengthMod3[strengthenLevel] / 1000f)) * reduceScoreData2 / 1000f);
                        }

                    }

                    //装备强化无视防御评分（当装备为武器类型时计算）
                    if (IsWeapon())
                    {
                        if (strengthenLevel >= mEquipBaseScoreModTable.StrengthMod1.Count)
                        {
                          
                        }
                        else
                        {

                            int scoreData = ItemDataManager.GetInstance()._GetDataByType(EquipScoreTable.eType.DISPHYATTACK);
                            eqStrengWsDefScore = (int)((LevelLimit + mEquipBaseScoreModTable.StrenthQualityMod1 / 1000f) / 8f * (mEquipBaseScoreModTable.StrengthMod1[strengthenLevel] / 1000f) * (mEquipBaseScoreModTable.StrenthQualityMod2 / 1000f) * scoreData / 1000f);
                        }

                    }

                    //附加属性评分
                    int itemAdditionalScore = 0;
                    var equipAttrTable = TableManager.GetInstance().GetTableItem<EquipAttrTable>(TableID);
                    if (equipAttrTable != null)
                    {
                        int fujiaScoreData = ItemDataManager.GetInstance()._GetDataByType(EquipScoreTable.eType.FUJIA);
                        itemAdditionalScore = (int)(equipAttrTable.AttachRateScore * fujiaScoreData / 1000f);
                    }

                    //附魔卡评分
                    int magicCardScore = 0;
                    if (precEnchantmentCard != null && precEnchantmentCard.iEnchantmentCardID > 0)
                    {
                        var magicCardTable = TableManager.GetInstance().GetTableItem<MagicCardTable>(precEnchantmentCard.iEnchantmentCardID);
                        if (magicCardTable != null)
                        {
                            magicCardScore = magicCardTable.Score + magicCardTable.UpAddScore * precEnchantmentCard.iEnchantmentCardLevel;
                        }
                    }

                    //宝珠评分
                    int preciousBeadScore = 0;
                    if (mPreciousBeadMountHole != null)
                    {
                        for (int i = 0; i < mPreciousBeadMountHole.Length; i++)
                        {
                            var precBead = mPreciousBeadMountHole[i];
                            if (precBead == null)
                            {
                                continue;
                            }

                            if (precBead.preciousBeadId <= 0) 
                            {
                                continue;
                            }

                            var beadTable = TableManager.GetInstance().GetTableItem<BeadTable>(mPreciousBeadMountHole[0].preciousBeadId);
                            if (beadTable == null)
                            {
                                continue;
                            }

                            preciousBeadScore += beadTable.Score;
                        }
                    }

                    //铭文评分
                    int preciousInscriptionScore = 0;
                    if (mInscriptionHoles != null)
                    {
                        for (int i = 0; i < mInscriptionHoles.Count; i++)
                        {
                            InscriptionHoleData holeData = mInscriptionHoles[i];
                            if (holeData == null)
                            {
                                continue;
                            }

                            if (holeData.IsOpenHole == false)
                            {
                                continue;
                            }

                            if (holeData.InscriptionId <= 0)
                            {
                                continue;
                            }

                            var inscriptionTable = TableManager.GetInstance().GetTableItem<InscriptionTable>(holeData.InscriptionId);
                            if (inscriptionTable == null)
                            {
                                continue;
                            }

                            preciousInscriptionScore += inscriptionTable.Score;
                        }
                    }

                    itemScore = calcVar1 + eqStrengWsAttackScore + eqStrengWsDefScore + eqStrengDecHurtScore + itemAdditionalScore + magicCardScore + preciousBeadScore + preciousInscriptionScore;

                    break;
                case EItemType.FASHION:
                case EItemType.FUCKTITTLE:

                    EquipBaseScoreTable mEquipBaseScoreTable = null;
                    Dictionary<int, object> dicts2 = TableManager.instance.GetTable<EquipBaseScoreTable>();
                    if (dicts2 != null)
                    {
                        var iter = dicts2.GetEnumerator();
                        while (iter.MoveNext())
                        {
                            EquipBaseScoreTable adt = iter.Current.Value as EquipBaseScoreTable;
                            if (adt == null)
                            {
                                continue;
                            }

                            if (adt.Type != (int)Type)
                            {
                                continue;
                            }

                            if (adt.SubType != SubType)
                            {
                                continue;
                            }

                            if (adt.ThirdType != (int)ThirdType)
                            {
                                continue;
                            }

                            if (adt.NeedLevel != LevelLimit)
                            {
                                continue;
                            }

                            if (adt.Color != (int)Quality)
                            {
                                continue;
                            }

                            if (adt.Color2 != Quality2)
                            {
                                continue;
                            }

                            if (adt.SuitId != SuitID)
                            {
                                continue;
                            }

                            mEquipBaseScoreTable = adt;
                            break;
                        }
                    }

                    if (mEquipBaseScoreTable != null)
                    {  //基础评分
                        baseScore = mEquipBaseScoreTable.Score;
                    }
                  
                    if (Type == EItemType.FASHION)
                    {
                        itemScore = baseScore;
                    }
                    else if (Type == EItemType.FUCKTITTLE)
                    {
                        //附加属性评分
                        int itemAdditionalScore2 = 0;
                        var equipAttrTable2 = TableManager.GetInstance().GetTableItem<EquipAttrTable>(TableID);
                        if (equipAttrTable2 != null)
                        {
                            itemAdditionalScore2 = equipAttrTable2.AttachRateScore;
                        }

                        //宝珠评分
                        int preciousBeadScore2 = 0;
                        if (mPreciousBeadMountHole != null 
                            && mPreciousBeadMountHole.Length > 0
                            && mPreciousBeadMountHole[0] != null
                            && mPreciousBeadMountHole[0].preciousBeadId > 0)
                        {
                            var beadTable = TableManager.GetInstance().GetTableItem<BeadTable>(mPreciousBeadMountHole[0].preciousBeadId);
                            if (beadTable != null)
                            {
                                preciousBeadScore2 = beadTable.Score;
                            }
                        }

                        itemScore = baseScore + itemAdditionalScore2 + preciousBeadScore2;
                    }
                    break;
            }

            int finalScore = (int)itemScore;

            return finalScore;
        }

        /// <summary>
        /// 是否是防具
        /// </summary>
        /// <returns></returns>
        private bool IsDefend()
        {
            bool isDefend = false;

            if (SubType == (int)ItemTable.eSubType.HEAD ||
                SubType == (int)ItemTable.eSubType.CHEST ||
                SubType == (int)ItemTable.eSubType.BELT ||
                SubType == (int)ItemTable.eSubType.LEG ||
                SubType == (int)ItemTable.eSubType.BOOT)
            {
                isDefend = true;
            }

            return isDefend;
        }

        /// <summary>
        /// 是否是首饰
        /// </summary>
        /// <returns></returns>
        private bool IsOrnaments()
        {
            bool isOrnaments = false;
            if (SubType == (int)ItemTable.eSubType.RING ||
                SubType == (int)ItemTable.eSubType.NECKLASE||
                SubType == (int)ItemTable.eSubType.BRACELET)
            {
                isOrnaments = true;
            }

            return isOrnaments;
        }

        /// <summary>
        /// 是否是武器
        /// </summary>
        /// <returns></returns>
        private bool IsWeapon()
        {
            bool isWeapon = false;
            if (SubType == (int)ItemTable.eSubType.WEAPON)
            {
                isWeapon = true;
            }

            return isWeapon;
        }

        /// <summary>
        /// 装备评分
        /// </summary>
        /// <returns></returns>
		int CalculateRateScore()
		{
			var propFactors = Global.Settings.equipPropFactors;
			float score = 0;

			ItemProperty ip = GetPropertyForEquipScore();

			//Type type = typeof(ItemProperty);

			//属性分
			for(int i=0; i<(int)AttributeType.hpGrow; ++i)
			{
				AttributeType at = (AttributeType)(i);

				string field = at.ToString();

				if (propFactors.ContainsKey(field))
				{ 
					//var fieldInfo = type.GetField(field);
					//if (fieldInfo != null)
					{
						int value = ip.GetValue(at);//(int)fieldInfo.GetValue(ip);
						if (value != 0)
						{
							//Logger.LogErrorFormat("[RATE_SCORE]field:{0} value:{1} factor:{2}", field, (value / (float)Global.equipPropExtra[field]), propFactors[field]);
							if (Global.equipPropExtra[field] == 10)
								score += propFactors[field] * (value / (float)Global.equipPropExtra[field]) * Mathf.Max(5, LevelLimit)/10f;
							else
								score += propFactors[field] * (value / (float)Global.equipPropExtra[field]);
						}
							
					}
                }
                else
                {
                    //独立攻击力
                    if (at == AttributeType.baseIndependence)
                    {
                        int value = ip.GetValue(at);//(int)fieldInfo.GetValue(ip);
                        if (value != 0)
                        {
                           int baseIndependenceData = ItemDataManager.GetInstance()._GetDataByType(EquipScoreTable.eType.Independent);
                            score += baseIndependenceData / 1000.0f * (value / 1000.0f);
                        }
                    }
                }
			}
           
            // 新增属性分数计算
            EquipScoreTable.eType[] elementTypes = 
            {
                EquipScoreTable.eType.None, EquipScoreTable.eType.Light, EquipScoreTable.eType.Fire,
                EquipScoreTable.eType.Ice, EquipScoreTable.eType.Dark
            };
            score += _CalculateLFIDAtkScore(ip.magicElements, elementTypes);

            EquipScoreTable.eType[] ElementAttackTypes = 
            {
                EquipScoreTable.eType.None, EquipScoreTable.eType.LightAttack, EquipScoreTable.eType.FireAttack,
                EquipScoreTable.eType.IceAttack, EquipScoreTable.eType.DarkAttack
            };
            score += _CalculateLFIDScore(ip.magicElementsAttack, ElementAttackTypes);

            EquipScoreTable.eType[] ElementDefenceTypes = 
            {
                EquipScoreTable.eType.None, EquipScoreTable.eType.LightDefence, EquipScoreTable.eType.FireDefence,
                EquipScoreTable.eType.IceDefence, EquipScoreTable.eType.DarkDefence
            };
            score += _CalculateLFIDScore(ip.magicElementsDefence, ElementDefenceTypes);

            EquipScoreTable.eType[] AllabnormalResistsTypes =
            {
                EquipScoreTable.eType.GDKX, EquipScoreTable.eType.CXKX, EquipScoreTable.eType.ZSKX, EquipScoreTable.eType.ZDKX,
                EquipScoreTable.eType.SMKX, EquipScoreTable.eType.XYKX, EquipScoreTable.eType.SHKX, EquipScoreTable.eType.BDKX, 
                EquipScoreTable.eType.SLKX, EquipScoreTable.eType.HLKX, EquipScoreTable.eType.SFKX, EquipScoreTable.eType.JSKX,
                EquipScoreTable.eType.ZZKX
            };
            score += _CalculateAllabnormalResistsScore(ip.abnormalResist, ip.abnormalResists, AllabnormalResistsTypes);

            //类型系数
            var factor = 1f;
            //这里以前获得第三类型装备评分系数的局限性
			if (Global.Settings.quipThirdTypeFactors.ContainsKey(ThirdType.ToString()))
			{
				//Logger.LogErrorFormat("[RATE_SCORE]部位:{0} factor:{1}", ThirdType.ToString(), Global.Settings.quipThirdTypeFactors[ThirdType.ToString()]);
				factor = Global.Settings.quipThirdTypeFactors[ThirdType.ToString()];
			}
			else
			{
			    factor = GetEquipScoreValueByThirdType();
			}
            score *= factor;

            //装备技能附加分
            if(TableData != null)
            {
                var pdata = TableManager.GetInstance().GetTableItem<EquipAttrTable>(TableData.EquipPropID);
                if (pdata != null && pdata.AttachRateScore != 0)
                {
                    score += pdata.AttachRateScore;
                }
            }

            //装备铭文附加分
            if (InscriptionHoles != null)
            {
                for (int i = 0; i < InscriptionHoles.Count; i++)
                {
                    var holeData = InscriptionHoles[i];
                    if (holeData == null)
                    {
                        continue;
                    }

                    if (holeData.InscriptionId <= 0)
                    {
                        continue;
                    }

                    var inscriptionTable = TableManager.GetInstance().GetTableItem<InscriptionTable>(holeData.InscriptionId);
                    if (inscriptionTable == null)
                    {
                        continue;
                    }

                    score += inscriptionTable.Score;
                }
            }

            //Logger.LogErrorFormat("score:{0}", score);

            int finalScore = (int)Mathf.Max(1f, score);

			return finalScore;
		}

        private float GetEquipScoreValueByThirdType()
        {
            //特殊处理，因为以前第三类型装备评分设计的局限性，没有涉及到OFG类型的装备评分系数
            int equipScoreValue = 1000;
            if (ThirdType == EITemThirdType.OFG)
            {
                equipScoreValue = ItemDataManager.GetInstance()._GetDataByType(EquipScoreTable.eType.OFG);
            }
            if(ThirdType == EITemThirdType.EAST_STICK)
            {
                equipScoreValue = ItemDataManager.GetInstance()._GetDataByType(EquipScoreTable.eType.EAST_STICK);
            }
            if (ThirdType == EITemThirdType.SICKLE)
            {
                equipScoreValue = ItemDataManager.GetInstance()._GetDataByType(EquipScoreTable.eType.SICKLE);
            }
            if (ThirdType == EITemThirdType.TOTEM)
            {
                equipScoreValue = ItemDataManager.GetInstance()._GetDataByType(EquipScoreTable.eType.TOTEM);
            }
            if (ThirdType == EITemThirdType.AXE)
            {
                equipScoreValue = ItemDataManager.GetInstance()._GetDataByType(EquipScoreTable.eType.AXE);
            }
            if (ThirdType == EITemThirdType.BEADS)
            {
                equipScoreValue = ItemDataManager.GetInstance()._GetDataByType(EquipScoreTable.eType.BEADS);
            }
            if (ThirdType == EITemThirdType.CROSS)
            {
                equipScoreValue = ItemDataManager.GetInstance()._GetDataByType(EquipScoreTable.eType.CROSS);
            }
            if (equipScoreValue <= 0)
                return 1f;
            return equipScoreValue / 1000.0f;
        }

        // 计算光火冰暗相关属性评分
        float _CalculateLFIDAtkScore(int[] elements, EquipScoreTable.eType[] ElementTypes)
        {
            float score = 0.0f;

            int ieleNum = 0;
            float feleMaxvalue = 0.0f;

            for (int i = 0; i < elements.Length; i++)
            {
                if (elements[i] <= 0)
                {
                    continue;
                }

                float temp = ItemDataManager.GetInstance()._GetDataByType(ElementTypes[i]);

                if (temp > feleMaxvalue)
                {
                    feleMaxvalue = temp;
                }

                ieleNum++;
            }

            score = (feleMaxvalue / 1000.0f) * (1 + (ieleNum - 1) * 0.1f);

            return score;
        }

        float _CalculateLFIDScore(CrypticInt32[] elements, EquipScoreTable.eType[] ElementTypes)
        {
            float score = 0.0f;

            int MaxValue = 0;
            int iMaxValueIndex = 0;
            int ieleNum = 0;

            for (int i = 0; i < elements.Length; i++)
            {
                if (elements[i] == 0)
                {
                    continue;
                }

                if(elements[i] > MaxValue)
                {
                    MaxValue = elements[i];
                    iMaxValueIndex = i;
                }

                ieleNum++;
            }

            score = MaxValue * (ItemDataManager.GetInstance()._GetDataByType(ElementTypes[iMaxValueIndex]) / 1000.0f) * (1 + (ieleNum - 1) * 0.1f);

            return score;
        }

        float _CalculateAllabnormalResistsScore(CrypticInt32 abnormalResist, CrypticInt32[] abnormalResists, EquipScoreTable.eType[] ElementTypes)
        {
            float score = 0.0f;

            int MaxValue = 0;
            int iMaxValueIndex = 0;
            int ieleNum = 0;

            for(int i = 0; i < abnormalResists.Length; i++)
            {
                if(abnormalResists[i] == 0)
                {
                    continue;
                }

                if(abnormalResists[i] > MaxValue)
                {
                    MaxValue = abnormalResists[i];
                    iMaxValueIndex = i;
                }

                ieleNum++;
            }

            if (abnormalResist > 0)
            {
                MaxValue += abnormalResist;
                ieleNum = ElementTypes.Length;
            }

            score = MaxValue * (ItemDataManager.GetInstance()._GetDataByType(ElementTypes[iMaxValueIndex]) / 1000.0f) * (1 + (ieleNum - 1) * 0.1f);

            return score;
        }

        // ComItem表现相关的属性
        public bool IsSelected;                 // 是否被选中

        int iFashionFreeTimes;
        public int FashionFreeTimes
        {
            get
            {
                return iFashionFreeTimes;
            }
            set
            {
                iFashionFreeTimes = value;
            }
        }

        // 时装基础属性(这是新拆分出来的)
        int iFashionBaseAttributeID;
        public int FashionBaseAttributeID
        {
            get
            {
                return iFashionBaseAttributeID;
            }
            set
            {
                if (Type == EItemType.FASHION)
                {
                    iFashionBaseAttributeID = value;
                }
                else
                {
                    iFashionBaseAttributeID = 0;
                }
            }
        }

        // 时装重选属性ID(属性加成原先居然是写在set里了)
        int iFashionAttributeID;
        public int FashionAttributeID
        {
            get
            {
                return iFashionAttributeID;
            }
            set
            {
                if(Type == EItemType.FASHION)
                {
                    iFashionAttributeID = value;
                }
                else
                {
                    iFashionAttributeID = 0;
                }
            }
        }

        public bool HasFashionAttribute
        {
            get
            {
                var fashionAttributeItem = TableManager.GetInstance().GetTableItem<ProtoTable.EquipAttrTable>(iFashionAttributeID);
                return null != fashionAttributeItem && fashionAttributes != null && fashionAttributes.Count > 1;
            }
        }
        public ProtoTable.EquipAttrTable CurFashionAttribute
        {
            get
            {
                return TableManager.GetInstance().GetTableItem<ProtoTable.EquipAttrTable>(iFashionAttributeID);
            }
        }
        public List<ProtoTable.EquipAttrTable> fashionAttributes = null;

        #region 宝珠相关(单局战斗专用 系统勿改)
        PrecBead[] mPrecBeadBattle;
        public PrecBead[] PrecBeadBattle
        {
            get { return mPrecBeadBattle; }
            set
            {
                mPrecBeadBattle = value;
                if (mPrecBeadBattle != null)
                {
                    if (BeadProp != null)
                        BeadProp.ResetProperties();

                    for (int i = 0; i < mPrecBeadBattle.Length; i++)
                    {
                        var precBeadBattle = mPrecBeadBattle[i];
                        if (precBeadBattle == null)
                            continue;
                        var beadItem = TableManager.GetInstance().GetTableItem<BeadTable>(precBeadBattle.preciousBeadId);
                        if (beadItem != null)
                            ExtraEquipAttributeAdded(EEquipAttributeType.EAT_BEAD, beadItem.PropType, beadItem.PropValue, BeadProp, beadItem.BuffInfoIDPve, beadItem.BuffInfoIDPvp);
                        //MagicCardIDAndBeadCardID(beadItem.PropType, beadItem.PropValue, beadItem.BuffInfoIDPve, beadItem.BuffInfoIDPvp, BeadProp);
                        int randomBuffId = precBeadBattle.randomBuffId;
                        if (!BeadProp.attachBuffIDs.Contains(randomBuffId))
                        {
                            BeadProp.attachBuffIDs.Add(randomBuffId);
                            BeadProp.attachPVPBuffIDs.Add(randomBuffId);
                        }
                    }
                }
            }
        }
        #endregion

        PrecBead[] mPreciousBeadMountHole;
        public PrecBead[] PreciousBeadMountHole
        {
            get
            {
                return mPreciousBeadMountHole;
            }
            set
            {
                mPreciousBeadMountHole = value;
                bEquipIsInsetBead = false;
                if (mPreciousBeadMountHole != null)
                {
                    if (BeadProp != null)
                    {
                        BeadProp.ResetProperties();
                    }
                    
                    for (int i = 0; i < mPreciousBeadMountHole.Length; i++)
                    {
                        var mPreciousBeadHole = mPreciousBeadMountHole[i];
                        if (mPreciousBeadHole == null)
                        {
                            continue;
                        }
                        
                        var beadItem = TableManager.GetInstance().GetTableItem<BeadTable>((int)mPreciousBeadHole.preciousBeadId);
                        if (beadItem != null)
                        {
                            //MagicCardIDAndBeadCardID(beadItem.PropType, beadItem.PropValue, beadItem.BuffInfoIDPve, beadItem.BuffInfoIDPvp, BeadProp);
                            ExtraEquipAttributeAdded(EEquipAttributeType.EAT_BEAD, beadItem.PropType, beadItem.PropValue, BeadProp, beadItem.BuffInfoIDPve, beadItem.BuffInfoIDPvp);
                            bEquipIsInsetBead = true;
                        }

                        BeadRandomBuff mBeadRandomBuffTable = BeadCardManager.GetInstance().GetBeadRandomBuffData(mPreciousBeadHole.randomBuffId);
                        if (mBeadRandomBuffTable != null)
                        {
                            //BeadAdditiveAttribute(mBeadRandomBuffTable.PropType, mBeadRandomBuffTable.PropValue, mPreciousBeadHole.randomBuffId, BeadProp);
                            ExtraEquipAttributeAdded(EEquipAttributeType.EAT_BEADADDITIVE, mBeadRandomBuffTable.PropType, mBeadRandomBuffTable.PropValue, BeadProp, null, null, mPreciousBeadHole.randomBuffId);
                        }
                       
                    }
                }
            }
        }

        int iBeadAdditiveAttributeBuffID;
        /// <summary>
        /// 宝珠附加属性BuffID
        /// </summary>
        public int BeadAdditiveAttributeBuffID
        {
            get { return iBeadAdditiveAttributeBuffID; }
            set { iBeadAdditiveAttributeBuffID = value; }
        }

        private int iBeadPickNumber;
        /// <summary>
        /// 宝珠摘取次数
        /// </summary>
        public int BeadPickNumber
        {
            get { return iBeadPickNumber; }
            set { iBeadPickNumber = value; }
        }

        private int iBeadReplaceNumber;
        /// <summary>
        /// 宝珠置换次数
        /// </summary>
        public int BeadReplaceNumber
        {
            get { return iBeadReplaceNumber; }
            set { iBeadReplaceNumber = value; }
        }

        bool bEquipIsInsetBead = false;
        /// <summary>
        /// 记录装备是否镶嵌了宝珠
        /// </summary>
        public bool BEquipIsInsetBead
        {
            get { return bEquipIsInsetBead; }
            set { bEquipIsInsetBead = value; }
        }
        
        private PrecEnchantmentCard precEnchantmentCard;
        /// <summary>
        /// 附魔卡数据
        /// </summary>
        public PrecEnchantmentCard mPrecEnchantmentCard
        {
            get { return precEnchantmentCard; }
            set
            {
                precEnchantmentCard = value;
                if (precEnchantmentCard != null)
                {
                    var magicItem = TableManager.GetInstance().GetTableItem<ProtoTable.MagicCardTable>((int)precEnchantmentCard.iEnchantmentCardID);
                    if (magicItem != null)
                    {
                        MagicProp.ResetProperties();
                        //EnchantmentCardAttribute(magicItem, precEnchantmentCard.iEnchantmentCardLevel, MagicProp);
                        ExtraEquipAttributeAdded(EEquipAttributeType.EAT_ENCHANTMENTCARD, magicItem.PropType, magicItem.PropValue,MagicProp);
                    }
                }
            }
        }

        /// <summary>
        /// 额外的装备属性加成（附魔卡，宝珠，铭文等）
        /// </summary>
        /// <param name="eEquipAttributeType">装备属性类型</param>
        /// <param name="PropTypeList">属性类型</param>
        /// <param name="PropValueList">属性值</param>
        /// <param name="equipProp"></param>
        /// <param name="PveBuffIDList"></param>
        /// <param name="PvpBuffIDList"></param>
        /// <param name="randomBuffID">随机Buff</param>
        private void ExtraEquipAttributeAdded(
            EEquipAttributeType eEquipAttributeType, 
            IList<int> PropTypeList, 
            IList<int> PropValueList,
            EquipProp equipProp,
            IList<int> PveBuffIDList = null, 
            IList<int> PvpBuffIDList = null,
            int randomBuffID = 0)
        {
            MagicCardTable magicCardTable = null;

            if (eEquipAttributeType == EEquipAttributeType.EAT_ENCHANTMENTCARD && precEnchantmentCard != null)
            {
                var magicItem = TableManager.GetInstance().GetTableItem<MagicCardTable>(precEnchantmentCard.iEnchantmentCardID);
                if (magicItem != null)
                {
                    magicCardTable = magicItem;
                }
            }

            // 保证函数调用之前的属性数据都是初始化好的，因为该函数内部统一采用"+="进行属性加成，不再会有"="赋值重置数据的操作
            if (PropTypeList.Count == PropValueList.Count)
            {
                for (int i = 0; i < PropTypeList.Count; ++i)
                {
                    var id = PropTypeList[i];
                    int pveProValue = 0;
                    int pvpProValue = 0;

                    //目前只有附魔卡区分Pve、Pvp, 附魔卡取的是表格数据
                    if (eEquipAttributeType == EEquipAttributeType.EAT_ENCHANTMENTCARD)
                    {
                        if(magicCardTable != null && precEnchantmentCard != null)
                        {
                            int CardLevel = precEnchantmentCard.iEnchantmentCardLevel;

                            pveProValue = (magicCardTable.PropValue[i] + CardLevel * magicCardTable.UpValue[i]);
                            pvpProValue = (magicCardTable.PropValue_PVP[i] + CardLevel * magicCardTable.UpValue_PVP[i]);
                        }
                    }
                    else
                    {
                        pveProValue = PropValueList[i];
                    }

                    if (id > (int)EServerProp.IRP_NONE && id <= (int)EServerProp.IRP_HITREC)
                    {
                        EServerProp eEServerProp = (EServerProp)id;
                        MapEnum mapEnum = Utility.GetEnumAttribute<EServerProp, MapEnum>(eEServerProp);

                        if (mapEnum != null)
                        {
                            if (eEquipAttributeType == EEquipAttributeType.EAT_ENCHANTMENTCARD)
                            {  
                                if (!isPVP)
                                {
                                    equipProp.props[(int)mapEnum.Prop] += pveProValue;
                                }
                                else
                                {
                                    equipProp.props[(int)mapEnum.Prop] += pvpProValue;
                                }
                            }
                            else
                            {
                                equipProp.props[(int)mapEnum.Prop] += pveProValue;
                            }
                        }
                    }
                    /*
                    * 物攻(18)，魔攻(19)，物防(20)，魔放(21)
                   */
                    else if (id >= 18 && id <= 21)
                    {
                        var e = (EEquipProp)(id - 18);

                        if (e >= EEquipProp.PhysicsAttack && e <= EEquipProp.MagicDefense)
                        {
                            if (eEquipAttributeType == EEquipAttributeType.EAT_ENCHANTMENTCARD)
                            {
                                if (!isPVP)
                                {
                                    equipProp.props[(int)e] += pveProValue;
                                }
                                else
                                {
                                    equipProp.props[(int)e] += pvpProValue;
                                }
                            }
                            else
                            {
                                equipProp.props[(int)e] += pveProValue;
                            }
                        }
                    }
                    else if (id >= (int)EServerProp.IRP_LIGHT && id <= (int)EServerProp.IRP_DARK)
                    {
                        // 宝珠附加属性不计算这一项
                        if(eEquipAttributeType != EEquipAttributeType.EAT_BEADADDITIVE)
                        {
                            // 这里永远为1，为避免重复计算，保险起见这里用“=”赋值
                            if (eEquipAttributeType == EEquipAttributeType.EAT_ENCHANTMENTCARD)
                            {
                                if(magicCardTable != null)
                                {
                                    if (magicCardTable.PropValue[i] > 0 && magicCardTable.PropValue[i] < (int)MagicElementType.MAX)
                                    {
                                        // 不管是pve还是pvp，赋值都是1，所以不再写 if (!isPVP)的判断了
                                        equipProp.magicElements[magicCardTable.PropValue[i]] = 1;
                                    }
                                }
                            }
                            else
                            {
                                if (pveProValue > 0 && pveProValue < (int)MagicElementType.MAX)
                                {
                                    equipProp.magicElements[pveProValue] = 1;
                                }
                            }
                        }
                    }
                    else if (id >= (int)EServerProp.IRP_LIGHT_ATK && id <= (int)EServerProp.IRP_DARK_ATK)
                    {
                        if (eEquipAttributeType == EEquipAttributeType.EAT_ENCHANTMENTCARD)
                        {
                            if(!isPVP)
                            {
                                if (pveProValue != 0)
                                {
                                    equipProp.magicElementsAttack[id - (int)EServerProp.IRP_LIGHT_ATK + 1] += pveProValue;
                                }
                            }
                            else
                            {
                                if (pvpProValue != 0)
                                {
                                    equipProp.magicElementsAttack[id - (int)EServerProp.IRP_LIGHT_ATK + 1] += pvpProValue;
                                }
                            }
                        }
                        else
                        {
                            if (pveProValue != 0)
                            {
                                equipProp.magicElementsAttack[id - (int)EServerProp.IRP_LIGHT_ATK + 1] += pveProValue;
                            }
                        }
                    }
                    else if (id >= (int)EServerProp.IRP_LIGHT_DEF && id <= (int)EServerProp.IRP_DARK_DEF)
                    {
                        if (eEquipAttributeType == EEquipAttributeType.EAT_ENCHANTMENTCARD)
                        {
                            if (!isPVP)
                            {
                                if(pveProValue != 0)
                                {
                                    equipProp.magicElementsDefence[id - (int)EServerProp.IRP_LIGHT_DEF + 1] += pveProValue;
                                }                       
                            }
                            else
                            {
                                if(pvpProValue != 0)
                                {
                                    equipProp.magicElementsDefence[id - (int)EServerProp.IRP_LIGHT_DEF + 1] += pvpProValue;
                                }     
                            }
                        }
                        else
                        {
                            if(pveProValue != 0)
                            {
                                equipProp.magicElementsDefence[id - (int)EServerProp.IRP_LIGHT_DEF + 1] += pveProValue;
                            }
                        }
                    }
                    else if (id >= (int)EServerProp.IRP_GDKX && id <= (int)EServerProp.IRP_ZZKX)
                    {
                        // 宝珠附加属性不计算这一项
                        if (eEquipAttributeType == EEquipAttributeType.EAT_ENCHANTMENTCARD)
                        {
                            if (!isPVP)
                            {
                                if (pveProValue != 0)
                                {
                                    equipProp.abnormalResists[id - (int)EServerProp.IRP_GDKX] += pveProValue;
                                }
                            }
                            else
                            {
                                if (pvpProValue != 0)
                                {
                                    equipProp.abnormalResists[id - (int)EServerProp.IRP_GDKX] += pvpProValue;
                                }
                            }
                        }
                        else if (eEquipAttributeType == EEquipAttributeType.EAT_BEAD || eEquipAttributeType == EEquipAttributeType.EAT_INSCRIPTION)
                        {
                            if(pveProValue != 0)
                            {
                                equipProp.abnormalResists[id - (int)EServerProp.IRP_GDKX] += pveProValue;
                            } 
                        }
                    }
                    else if (id == (int)EServerProp.IRP_YKXZ)
                    {
                        // 宝珠附加属性不计算这一项
                        if (eEquipAttributeType == EEquipAttributeType.EAT_ENCHANTMENTCARD)
                        {
                            if (!isPVP)
                            {
                                if(pveProValue != 0)
                                {
                                    equipProp.props[(int)EEquipProp.AbormalResist] += pveProValue;
                                }
                            }
                            else
                            {
                                if(pvpProValue != 0)
                                {
                                    equipProp.props[(int)EEquipProp.AbormalResist] += pvpProValue;
                                }
                            }
                        }
                        else if (eEquipAttributeType == EEquipAttributeType.EAT_BEAD || eEquipAttributeType == EEquipAttributeType.EAT_INSCRIPTION)
                        {
                            if(pveProValue != 0)
                            {
                                equipProp.props[(int)EEquipProp.AbormalResist] += pveProValue;
                            }  
                        }
                    }
                    else if (id == (int)EServerProp.IRP_INDEPENDENCE)
                    {
                        if (eEquipAttributeType == EEquipAttributeType.EAT_ENCHANTMENTCARD)
                        {
                            if (!isPVP)
                            {
                                if (pveProValue != 0)
                                {
                                    equipProp.props[(int)EEquipProp.Independence] += pveProValue;
                                }
                            }
                            else
                            {
                                if (pvpProValue != 0)
                                {
                                    equipProp.props[(int)EEquipProp.Independence] += pvpProValue;
                                }
                            }
                        }
                        else
                        {
                            if (pveProValue != 0)
                            {
                                equipProp.props[(int)EEquipProp.Independence] += pveProValue;
                            }
                        }
                    }
                }
            }
            
            if (eEquipAttributeType == EEquipAttributeType.EAT_ENCHANTMENTCARD)
            {
                // 目前只有附魔卡是可以升级的（不排除以后宝珠，铭文也可以升级）,升级后的附魔卡取对应等级的buff，并且不继承0级的基础buff
                if (precEnchantmentCard != null && magicCardTable != null)
                {
                    int CardLevel = precEnchantmentCard.iEnchantmentCardLevel;

                    //从零开始//附魔卡附带属性pve
                    if (CardLevel > 0 && CardLevel <= magicCardTable.UpBuffID.Count)
                    {
                        AddAttachPveBuffIID(equipProp, magicCardTable.UpBuffID[CardLevel - 1]);
                    }
                    else
                    {
                        for (int i = 0; i < magicCardTable.BuffID.Count; i++)
                        {
                            AddAttachPveBuffIID(equipProp, magicCardTable.BuffID[i]);
                        }
                    }

                    //从零开始//附魔卡附带属性pvp
                    if(CardLevel > 0 && CardLevel <= magicCardTable.UpBuffID_PVP.Count)
                    {
                        AddAttachPvpBuffIID(equipProp, magicCardTable.UpBuffID_PVP[CardLevel - 1]);
                    }
                    else
                    {
                        for (int i = 0; i < magicCardTable.BuffID_PVP.Count; i++)
                        {
                            AddAttachPvpBuffIID(equipProp, magicCardTable.BuffID_PVP[i]);
                        }
                    }
                }
            }
            else if (eEquipAttributeType == EEquipAttributeType.EAT_BEAD)
            {
                // 宝珠附带的Buff
                if (PveBuffIDList != null)
                {
                    for (int i = 0; i < PveBuffIDList.Count; i++)
                    {
                        AddAttachPveBuffIID(equipProp, PveBuffIDList[i]);
                    }
                }

                if (PvpBuffIDList != null)
                {
                    for (int i = 0; i < PvpBuffIDList.Count; i++)
                    {
                        AddAttachPvpBuffIID(equipProp, PvpBuffIDList[i]);
                    }
                }
            }
            else if (eEquipAttributeType == EEquipAttributeType.EAT_BEADADDITIVE)
            {
                if (PropTypeList.Count == 0 || PropValueList.Count == 0 || PropTypeList.Count != PropValueList.Count)
                {
                    AddAttachPveBuffIID(equipProp, randomBuffID);
                    AddAttachPvpBuffIID(equipProp, randomBuffID);
                }
            }
            else if (eEquipAttributeType == EEquipAttributeType.EAT_INSCRIPTION)
            {
                if (PveBuffIDList != null)
                {
                    for (int i = 0; i < PveBuffIDList.Count; i++)
                    {
                        AddAttachPveBuffIID(equipProp, PveBuffIDList[i]);
                    }
                }
            }
        }
        
        // bxy buffId by ckm
        public void AddAttachBxyBuffIID(EquipProp equipProp, int oldBuffId, int newBuffId)
        {
            if(equipProp == null)
            {
                return;
            }

            if (equipProp.attachBuffIDs != null)
            {
                if(oldBuffId != 0)
                {
                    equipProp.attachBuffIDs.Remove(oldBuffId);
                }
                if(newBuffId != 0)
                {
                    equipProp.attachBuffIDs.Add(newBuffId);
                }
            }

            if (equipProp.attachPVPBuffIDs != null)
            {
                if(oldBuffId != 0)
                {
                    equipProp.attachPVPBuffIDs.Remove(oldBuffId);
                }
                if(newBuffId != 0)
                {
                    equipProp.attachPVPBuffIDs.Add(newBuffId);
                }
            }
        }

        void AddAttachPveBuffIID(EquipProp equipProp, int buffId)
        {
            if (equipProp != null && equipProp.attachBuffIDs != null)
            {
                if (equipProp.attachBuffIDs.Contains(buffId) == false)
                {
                    equipProp.attachBuffIDs.Add(buffId);
                }
            }
        }

        void AddAttachPvpBuffIID(EquipProp equipProp, int buffId)
        {
            if (equipProp != null && equipProp.attachPVPBuffIDs != null)
            {
                if (equipProp.attachPVPBuffIDs.Contains(buffId) == false)
                {
                    equipProp.attachPVPBuffIDs.Add(buffId);
                }
            }
        }

        /// <summary>
        /// 附魔卡属性和buff
        /// </summary>
        /// <param name="magicCardTable"></param>
        /// <param name="item"></param>
        void EnchantmentCardAttribute(MagicCardTable magicCardTable,int enchantmentCardLevel,EquipProp magicProp)
        {
            if (magicCardTable.PropType.Count == magicCardTable.PropValue.Count)
            {
                for (int i = 0; i < magicCardTable.PropType.Count; ++i)
                {
                    var id = magicCardTable.PropType[i];
                    var proValue = (magicCardTable.PropValue[i] + enchantmentCardLevel * magicCardTable.UpValue[i]);
                    var pvpValue = (magicCardTable.PropValue_PVP[i] + enchantmentCardLevel * magicCardTable.UpValue_PVP[i]);
                    if (id > (int)EServerProp.IRP_NONE && id <= (int)EServerProp.IRP_HITREC)
                    {
                        EServerProp eEServerProp = (EServerProp)id;
                        MapEnum mapEnum = Utility.GetEnumAttribute<EServerProp, MapEnum>(eEServerProp);
                        if (mapEnum != null)
                        {
                            if (!isPVP)
                            {
                                magicProp.props[(int)mapEnum.Prop] = proValue;
                            }
                            else
                            {
                                magicProp.props[(int)mapEnum.Prop] = pvpValue;
                            }
                        }
                    }
                    /*
                     * 物攻(18)，魔攻(19)，物防(20)，魔放(21)
                    */
                    else if (id >= 18 && id <= 21)
                    {
                        var e = (EEquipProp)(id - 18);
                        if (e >= EEquipProp.PhysicsAttack && e <= EEquipProp.MagicDefense)
                        {
                            if (!isPVP)
                            {
                                magicProp.props[(int)e] = proValue;
                            }
                            else
                            {
                                magicProp.props[(int)e] = pvpValue;
                            }
                        }
                    }
                    else if (id >= (int)EServerProp.IRP_LIGHT && id <= (int)EServerProp.IRP_DARK)
                    {
                        if (magicCardTable.PropValue[i] > 0 && magicCardTable.PropValue[i] < (int)MagicElementType.MAX)
                            magicProp.magicElements[magicCardTable.PropValue[i]] = 1;
                    }
                    else if (id >= (int)EServerProp.IRP_LIGHT_ATK && id <= (int)EServerProp.IRP_DARK_ATK)
                    {
                        if (magicCardTable.PropValue[i] != 0)
                        {
                            if (!isPVP)
                            {
                                magicProp.magicElementsAttack[id - (int)EServerProp.IRP_LIGHT_ATK + 1] = proValue;
                            }
                            else
                            {
                                magicProp.magicElementsAttack[id - (int)EServerProp.IRP_LIGHT_ATK + 1] = pvpValue;
                            }
                        }
                    }
                    else if (id >= (int)EServerProp.IRP_LIGHT_DEF && id <= (int)EServerProp.IRP_DARK_DEF)
                    {
                        if (magicCardTable.PropValue[i] != 0)
                        {
                            if (!isPVP)
                            {
                                magicProp.magicElementsDefence[id - (int)EServerProp.IRP_LIGHT_DEF + 1] = proValue;
                            }
                            else
                            {
                                magicProp.magicElementsDefence[id - (int)EServerProp.IRP_LIGHT_DEF + 1] = pvpValue;
                            }
                        }
                    }
                    else if (id >= (int)EServerProp.IRP_GDKX && id <= (int)EServerProp.IRP_ZZKX)
                    {
                        if (magicCardTable.PropValue[i] != 0)
                        {
                            if (!isPVP)
                            {
                                magicProp.abnormalResists[id - (int)EServerProp.IRP_GDKX] = proValue;
                            }
                            else
                            {
                                magicProp.abnormalResists[id - (int)EServerProp.IRP_GDKX] = pvpValue;
                            }
                        }
                    }
                    else if (id == (int)EServerProp.IRP_YKXZ)
                    {
                        if (magicCardTable.PropValue[i] != 0)
                        {
                            if (!isPVP)
                            {
                                magicProp.props[(int)EEquipProp.AbormalResist] = proValue;
                            }
                            else
                            {
                                magicProp.props[(int)EEquipProp.AbormalResist] = pvpValue;
                            }
                        }
                    }
                }
            }

            
            if (enchantmentCardLevel > 0 && 
                enchantmentCardLevel <= magicCardTable.UpBuffID.Count &&
                enchantmentCardLevel <= magicCardTable.UpBuffID_PVP.Count)
            {
                //从零开始//附魔卡附带属性pve
                if (magicCardTable.UpBuffID.Count > 0)
                {
                    if (magicProp.attachBuffIDs.Contains(magicCardTable.UpBuffID[enchantmentCardLevel - 1]) == false)
                    {
                        magicProp.attachBuffIDs.Add(magicCardTable.UpBuffID[enchantmentCardLevel - 1]);
                    }
                }

                //从零开始//附魔卡附带属性pvp
                if (magicCardTable.UpBuffID_PVP.Count > 0)
                {
                    if (magicProp.attachPVPBuffIDs.Contains(magicCardTable.UpBuffID_PVP[enchantmentCardLevel - 1]) == false)
                    {
                        magicProp.attachPVPBuffIDs.Add(magicCardTable.UpBuffID_PVP[enchantmentCardLevel - 1]);
                    }
                }
                
            }
            else
            {
                //pve
                if (magicCardTable.BuffID != null)
                {
                    for (int i = 0; i < magicCardTable.BuffID.Count; i++)
                    {
                        if (magicProp.attachBuffIDs.Contains(magicCardTable.BuffID[i]) == false)
                        {
                            magicProp.attachBuffIDs.Add(magicCardTable.BuffID[i]);
                        }
                    }
                }

                //pvp
                if (magicCardTable.BuffID_PVP != null)
                {
                    for (int i = 0; i < magicCardTable.BuffID_PVP.Count; i++)
                    {
                        if (magicProp.attachPVPBuffIDs.Contains(magicCardTable.BuffID_PVP[i]) == false)
                        {
                            magicProp.attachPVPBuffIDs.Add(magicCardTable.BuffID_PVP[i]);
                        }
                    }
                }
            }
        }

        //装备身上镶嵌的宝珠添加附加属性的Buff
        void BeadAdditiveAttribute(IList<int> iPropType, IList<int> iPropValue,int randomBuffID, EquipProp equipProp)
        {
            if (iPropType.Count == iPropValue.Count && iPropType.Count != 0 && iPropValue.Count != 0)
            {
                for (int i = 0; i < iPropType.Count; ++i)
                {
                    var id = iPropType[i];
                    if (id > (int)EServerProp.IRP_NONE && id <= (int)EServerProp.IRP_HITREC)
                    {
                        EServerProp eEServerProp = (EServerProp)id;
                        MapEnum mapEnum = Utility.GetEnumAttribute<EServerProp, MapEnum>(eEServerProp);
                        if (mapEnum != null)
                        {
                            equipProp.props[(int)mapEnum.Prop] += iPropValue[i];
                        }
                    } /*
                     * 物攻(18)，魔攻(19)，物防(20)，魔放(21)
                    */
                    else if (id >= 18 && id <= 21)
                    {
                        var e = (EEquipProp)(id - 18);
                        if (e >= EEquipProp.PhysicsAttack && e <= EEquipProp.MagicDefense)
                            equipProp.props[(int)e] += iPropValue[i];
                    }
                    else if (id >= (int)EServerProp.IRP_LIGHT_ATK && id <= (int)EServerProp.IRP_DARK_ATK)
                    {
                        if (iPropValue[i] != 0)
                            equipProp.magicElementsAttack[id - (int)EServerProp.IRP_LIGHT_ATK + 1] += iPropValue[i];
                    }
                    else if (id >= (int)EServerProp.IRP_LIGHT_DEF && id <= (int)EServerProp.IRP_DARK_DEF)
                    {
                        if (iPropValue[i] != 0)
                            equipProp.magicElementsDefence[id - (int)EServerProp.IRP_LIGHT_DEF + 1] += iPropValue[i];
                    }
                }
            }
            else
            {
                if (equipProp.attachBuffIDs.Contains(randomBuffID) == false)
                {
                    equipProp.attachBuffIDs.Add(randomBuffID);
                }
                
                if (equipProp.attachPVPBuffIDs.Contains(randomBuffID) == false)
                {
                    equipProp.attachPVPBuffIDs.Add(randomBuffID);
                }
            }
        }

        void MagicCardIDAndBeadCardID(IList<int> iPropType, IList<int> iPropValue, IList<int> iPveBuffID, IList<int> iPvpBuffID,EquipProp equipProp)
        {
            if (iPropType.Count == iPropValue.Count)
            {
                for (int i = 0; i < iPropType.Count; ++i)
                {
                    var id = iPropType[i];
                    if (id > (int)EServerProp.IRP_NONE && id <= (int)EServerProp.IRP_HITREC)
                    {
                        EServerProp eEServerProp = (EServerProp)id;
                        MapEnum mapEnum = Utility.GetEnumAttribute<EServerProp, MapEnum>(eEServerProp);
                        if (mapEnum != null)
                        {
                            equipProp.props[(int)mapEnum.Prop] = iPropValue[i];
                        }
                    }
                    /*
                     * 物攻(18)，魔攻(19)，物防(20)，魔放(21)
                    */
                    else if (id >= 18 && id <= 21)
                    {
                        var e = (EEquipProp)(id - 18);
                        if (e >= EEquipProp.PhysicsAttack && e <= EEquipProp.MagicDefense)
                            equipProp.props[(int)e] = iPropValue[i];
                    }
                    else if (id >= (int)EServerProp.IRP_LIGHT && id <= (int)EServerProp.IRP_DARK)
                    {
                        if (iPropValue[i] > 0 && iPropValue[i] < (int)MagicElementType.MAX)
                            equipProp.magicElements[iPropValue[i]] = 1;
                    }
                    else if (id >= (int)EServerProp.IRP_LIGHT_ATK && id <= (int)EServerProp.IRP_DARK_ATK)
                    {
                        if (iPropValue[i] != 0)
                            equipProp.magicElementsAttack[id - (int)EServerProp.IRP_LIGHT_ATK + 1] = iPropValue[i];
                    }
                    else if (id >= (int)EServerProp.IRP_LIGHT_DEF && id <= (int)EServerProp.IRP_DARK_DEF)
                    {
                        if (iPropValue[i] != 0)
                            equipProp.magicElementsDefence[id - (int)EServerProp.IRP_LIGHT_DEF + 1] = iPropValue[i];
                    }
                    else if (id >= (int)EServerProp.IRP_GDKX && id <= (int)EServerProp.IRP_ZZKX)
                    {
                        if (iPropValue[i] != 0)
                            equipProp.abnormalResists[id - (int)EServerProp.IRP_GDKX] = iPropValue[i];
                    }
                    else if (id == (int)EServerProp.IRP_YKXZ)
                    {
                        if (iPropValue[i] != 0)
                            equipProp.props[(int)EEquipProp.AbormalResist] = iPropValue[i];
                    }
                }
            }
            
            // 宝珠附带的Buff
            if (iPveBuffID != null)
            {
                for (int i = 0; i < iPveBuffID.Count; i++)
                {
                    equipProp.attachBuffIDs.Add(iPveBuffID[i]);
                }
            }

            if (iPvpBuffID != null)
            {
                for (int i = 0; i < iPvpBuffID.Count; i++)
                {
                    equipProp.attachPVPBuffIDs.Add(iPvpBuffID[i]);
                }
            }
        }

        private List<InscriptionHoleData> mInscriptionHoles;
        /// <summary>
        /// 铭文孔
        /// </summary>
        public List<InscriptionHoleData> InscriptionHoles
        {
            get { return mInscriptionHoles; }
            set
            {
                mInscriptionHoles = value;
                if (mInscriptionHoles != null)
                {
                    if (IncriptionProp != null)
                    {
                        IncriptionProp.ResetProperties();
                    }

                    for (int i = 0; i < mInscriptionHoles.Count; i++)
                    {
                        if (mInscriptionHoles[i] == null)
                        {
                            continue;
                        }
                        int incriptionId = mInscriptionHoles[i].InscriptionId;

                        var inscriptionTable = TableManager.GetInstance().GetTableItem<InscriptionTable>(incriptionId);
                        if (inscriptionTable != null)
                        {
                            //IncriptionAttrIntoTheBattle(inscriptionTable.PropType, inscriptionTable.PropValue, inscriptionTable.BuffID,IncriptionProp);
                            ExtraEquipAttributeAdded(EEquipAttributeType.EAT_INSCRIPTION, inscriptionTable.PropType, inscriptionTable.PropValue, IncriptionProp, inscriptionTable.BuffID);
                        }
                    }
                }
            }
        }

        void IncriptionAttrIntoTheBattle(IList<int> iPropType, IList<int> iPropValue, IList<int> iPveBuffID, EquipProp equipProp)
        {
            if (iPropType.Count == iPropValue.Count)
            {
                for (int i = 0; i < iPropType.Count; ++i)
                {
                    var id = iPropType[i];
                    if (id > (int)EServerProp.IRP_NONE && id <= (int)EServerProp.IRP_HITREC)
                    {
                        EServerProp eEServerProp = (EServerProp)id;
                        MapEnum mapEnum = Utility.GetEnumAttribute<EServerProp, MapEnum>(eEServerProp);
                        if (mapEnum != null)
                        {
                            equipProp.props[(int)mapEnum.Prop] = iPropValue[i];
                        }
                    }
                    /*
                     * 物攻(18)，魔攻(19)，物防(20)，魔放(21)
                    */
                    else if (id >= 18 && id <= 21)
                    {
                        var e = (EEquipProp)(id - 18);
                        if (e >= EEquipProp.PhysicsAttack && e <= EEquipProp.MagicDefense)
                            equipProp.props[(int)e] = iPropValue[i];
                    }
                    else if (id >= (int)EServerProp.IRP_LIGHT && id <= (int)EServerProp.IRP_DARK)
                    {
                        if (iPropValue[i] > 0 && iPropValue[i] < (int)MagicElementType.MAX)
                            equipProp.magicElements[iPropValue[i]] = 1;
                    }
                    else if (id >= (int)EServerProp.IRP_LIGHT_ATK && id <= (int)EServerProp.IRP_DARK_ATK)
                    {
                        if (iPropValue[i] != 0)
                            equipProp.magicElementsAttack[id - (int)EServerProp.IRP_LIGHT_ATK + 1] = iPropValue[i];
                    }
                    else if (id >= (int)EServerProp.IRP_LIGHT_DEF && id <= (int)EServerProp.IRP_DARK_DEF)
                    {
                        if (iPropValue[i] != 0)
                            equipProp.magicElementsDefence[id - (int)EServerProp.IRP_LIGHT_DEF + 1] = iPropValue[i];
                    }
                    else if (id >= (int)EServerProp.IRP_GDKX && id <= (int)EServerProp.IRP_ZZKX)
                    {
                        if (iPropValue[i] != 0)
                            equipProp.abnormalResists[id - (int)EServerProp.IRP_GDKX] = iPropValue[i];
                    }
                    else if (id == (int)EServerProp.IRP_YKXZ)
                    {
                        if (iPropValue[i] != 0)
                            equipProp.props[(int)EEquipProp.AbormalResist] = iPropValue[i];
                    }
                }
            }
            
            if (iPveBuffID != null)
            {
                for (int i = 0; i < iPveBuffID.Count; i++)
                {
                    equipProp.attachBuffIDs.Add(iPveBuffID[i]);
                }
            }
        }

#if OLD_EQUIP_PROP
        public EquipProp BaseProp = new EquipProp();
        public EquipProp RandamProp = new EquipProp();
        public EquipProp MagicProp = new EquipProp();
        public EquipProp BeadProp = new EquipProp();
#else
        public EquipProp BaseProp
        {
            get
            {
                if (mBaseProp == null)
                {
                    this.mBaseProp = new EquipProp();
                }

                return this.mBaseProp;
            }
            set { this.mBaseProp = value; }
        }

        private EquipProp mBaseProp;

        public EquipProp RandamProp
        {
            get
            {
                if (mRandamProp == null && (this.Type == EItemType.EQUIP || this.Type == EItemType.FASHION || this.Type == EItemType.FUCKTITTLE))
                {
                    this.mRandamProp = new EquipProp();
                }

                return this.mRandamProp;
            }
            set { this.mRandamProp = value; }
        }
        private EquipProp mRandamProp;

        public EquipProp MagicProp
        {
            get
            {
                if (mMagicProp == null && (this.Type == EItemType.EQUIP || this.Type == EItemType.FASHION || this.Type == EItemType.FUCKTITTLE || this.Type == EItemType.EXPENDABLE))
                {
                    this.mMagicProp = new EquipProp();
                }

                return this.mMagicProp;
            }
            set { this.mMagicProp = value; }
        }
        private EquipProp mMagicProp;

        public EquipProp BeadProp
        {
            get
            {
                if (mBeadProp == null && (this.Type == EItemType.EQUIP || this.Type == EItemType.FASHION || this.Type == EItemType.FUCKTITTLE || this.Type == EItemType.EXPENDABLE))
                {
                    this.mBeadProp = new EquipProp();
                }

                return this.mBeadProp;
            }
            set { this.mBeadProp = value; }
        }
        private EquipProp mBeadProp;

        public EquipProp IncriptionProp
        {
            get
            {
                if (mInscriptionProp == null && (this.Type == EItemType.EQUIP || this.Type == EItemType.FASHION || this.Type == EItemType.FUCKTITTLE || this.Type == EItemType.EXPENDABLE))
                {
                    this.mInscriptionProp = new EquipProp();
                }

                return mInscriptionProp;
            }
            set
            {
                mInscriptionProp = value;
            }
        }

        private EquipProp mInscriptionProp;
#endif
        /// <summary>
        /// 是否新数据
        /// </summary>
        public bool IsNew { get; set; }

        public class QualityInfo
        {
            public QualityInfo(EItemQuality quality, Color col, string colStr, string desc, string background, string titleBG, string titleBG2, string tipTitleBackGround)
            {
                Quality = quality;
                Col = col;
                ColStr = colStr;
                Desc = desc;
                Background = background;
                TitleBG = titleBG;
                TitleBG2 = titleBG2;
                TipTitleBackGround = tipTitleBackGround;
            }

            /// <summary>
            /// tip顶部图片路径
            /// </summary>
            public string TipTitleBackGround;

            /// <summary>
            /// 品质枚举
            /// </summary>
            public EItemQuality Quality;

            /// <summary>
            /// 品质对应颜色值 
            /// </summary>
            public Color Col;

            /// <summary>
            /// 品质对应颜色RGBA值, 16进制颜色码
            /// </summary>
            public string ColStr;

            /// <summary>
            /// 品质对应中文描述
            /// </summary>
            public string Desc;

            /// <summary>
            /// 品质框
            /// </summary>
            public string Background;

            /// <summary>
            /// tip标题背景
            /// </summary>
            public string TitleBG;

            /// <summary>
            /// tip标题背景2
            /// </summary>
            public string TitleBG2;
        }

        public static QualityInfo GetQualityInfo(EItemQuality type,bool bGrey = false)
        {
            for (int i = 0; i < ms_qualityInfos.Length; ++i)
            {
                if (ms_qualityInfos[i].Quality == type)
                {
                    if(type == EItemQuality.WHITE && bGrey)
                    {
                        return ms_grey_quality;
                    }
                    return ms_qualityInfos[i];
                }
            }
            return null;
        }

        //由Color1 和 color2 共同决定颜色：两种特殊颜色：1|1灰色，5|3玫红色。其他颜色正常
        public static QualityInfo GetQualityInfo(EItemQuality type, int color2)
        {

            for (int i = 0; i < ms_qualityInfos.Length; i++)
            {
                if (ms_qualityInfos[i].Quality == type)
                {
                    //color1 = 1, color2 = 1 表示灰色
                    if (type == EItemQuality.WHITE && color2 == 1)
                    {
                        return ms_grey_quality;
                    }
                    //color1 = 5, color2 = 3 表示红
                    if (type == EItemQuality.PINK && color2 == 3)
                    {
                        return ms_rosered_quality;
                    }

                    //color2 部位1 或者3，只有color1 决定
                    return ms_qualityInfos[i];
                }
            }
            return null;
        }


        static protected QualityInfo ms_grey_quality = new QualityInfo(EItemQuality.WHITE, new Color(0.666f, 0.6f, 0.6f, 0.6f), "#bbbbbbff", "灰色",
                    "UI/Image/NewPacked/Common_Item.png:Common_Pinzhi01",
                    "UIFlatten/Image/Packed/pck_UI_Common00.png:UI_Tongyong_Lanzhuang_Di",
                    "UIFlatten/Image/Packed/pck_UI_Common00.png:UI_Common_ListItem02",
                    "UI/Image/Packed/p_UI_Common00.png:UI_Tongyong_Tubiao_Huise_Di");

        //需要设置颜色和边框的地址
        static protected QualityInfo ms_rosered_quality = new QualityInfo(EItemQuality.PINK, 
            new Color(0.733f, 0.133f, 0.066f, 0.6f), 
            "#ff4444ff", 
            "红",
            "UI/Image/NewPacked/Common_Item.png:Common_Pinzhi05",
            "UIFlatten/Image/Packed/pck_UI_Common00.png:UI_Tongyong_Fenzhuang_Di",
            "UIFlatten/Image/Packed/pck_UI_Common00.png:UI_Tongyong_Fenzhuang01",
            "UI/Image/Packed/p_UI_Common00.png:UI_Tongyong_Tubiao_Pink_Di");
        
        static protected QualityInfo[] ms_qualityInfos =
        {
			new QualityInfo(EItemQuality.WHITE, new Color(0.666f, 0.6f, 0.6f, 0.6f), "#bbbbbbff", "白"  ,
                    "UI/Image/NewPacked/Common_Item.png:Common_Pinzhi01", 
                    "UIFlatten/Image/Packed/pck_UI_Common00.png:UI_Tongyong_Lanzhuang_Di", 
                    "UIFlatten/Image/Packed/pck_UI_Common00.png:UI_Common_ListItem02",
                    "UI/Image/Packed/p_UI_Common00.png:UI_Tongyong_Tubiao_Huise_Di"),

			new QualityInfo(EItemQuality.BLUE, new Color(0.133f,0.4f,0.733f, 0.6f), "#4488ddff", "蓝"    ,
                    "UI/Image/NewPacked/Common_Item.png:Common_Pinzhi03", 
                    "UIFlatten/Image/Packed/pck_UI_Common00.png:UI_Tongyong_Lanzhuang_Di", 
                    "UIFlatten/Image/Packed/pck_UI_Common00.png:UI_Tongyong_Lanzhuang01",
                    "UI/Image/Packed/p_UI_Common00.png:UI_Tongyong_Tubiao_Blue_Di"),

			new QualityInfo(EItemQuality.PURPLE, new Color(0.533f,0.066f,0.733f, 0.6f), "#aa44eeff", "紫"  ,
                    "UI/Image/NewPacked/Common_Item.png:Common_Pinzhi04", 
                    "UIFlatten/Image/Packed/pck_UI_Common00.png:UI_Tongyong_Di_04", 
                    "UIFlatten/Image/Packed/pck_UI_Common00.png:UI_Tongyong_Zizhuang01",
                    "UI/Image/Packed/p_UI_Common00.png:UI_Tongyong_Tubiao_Purple_Di"),

			new QualityInfo(EItemQuality.PINK, new Color(1f,0.333f,0.733f, 1f), "#FF55BBFF", "粉"    ,
                    "UI/Image/NewPacked/Common_Item.png:Common_Pinzhi05", 
                    "UIFlatten/Image/Packed/pck_UI_Common00.png:UI_Tongyong_Fenzhuang_Di", 
                    "UIFlatten/Image/Packed/pck_UI_Common00.png:UI_Tongyong_Fenzhuang01",
                    "UI/Image/Packed/p_UI_Common00.png:UI_Tongyong_Tubiao_Pink_Di"),

			new QualityInfo(EItemQuality.YELLOW, new Color(0.8f,0.466f,0.066f, 0.6f), "#ee9922ff", "橙"  ,
                    "UI/Image/NewPacked/Common_Item.png:Common_Pinzhi06", 
                    "UIFlatten/Image/Packed/pck_UI_Common00.png:UI_Tongyong_Di_03", 
                    "UIFlatten/Image/Packed/pck_UI_Common00.png:UI_Tongyong_Chengzhuang01",
                    "UI/Image/Packed/p_UI_Common00.png:UI_Tongyong_Tubiao_Orange_Di"),

			new QualityInfo(EItemQuality.GREEN, new Color(0.2f,0.6f,0.2f, 0.6f), "#33bb33ff", "绿"    ,
                    "UI/Image/NewPacked/Common_Item.png:Common_Pinzhi02", 
                    "UIFlatten/Image/Packed/pck_UI_Common00.png:UI_Tongyong_Lvzhuang_Di", 
                    "UIFlatten/Image/Packed/pck_UI_Common00.png:UI_Tongyong_Lvzhuang01",
                    "UI/Image/Packed/p_UI_Common00.png:UI_Tongyong_Tubiao_Green_Di"),
        };



        static EEquipProp[] ms_attachAttrIDs =
        {
            EEquipProp.HPMax,
            EEquipProp.MPMax,
            EEquipProp.HPRecover,
            EEquipProp.MPRecover,
            EEquipProp.AttackSpeedRate,
            EEquipProp.FireSpeedRate,
            EEquipProp.MoveSpeedRate,
            EEquipProp.AbormalResist,
            EEquipProp.AbormalResists,
            EEquipProp.Elements,
            EEquipProp.LightAttack,
            EEquipProp.FireAttack,
            EEquipProp.IceAttack,
            EEquipProp.DarkAttack,
            EEquipProp.LightDefence,
            EEquipProp.FireDefence,
            EEquipProp.IceDefence,
            EEquipProp.DarkDefence,
            EEquipProp.HitRate,
            EEquipProp.AvoidRate,
            EEquipProp.PhysicCritRate,
            EEquipProp.MagicCritRate,
            EEquipProp.Spasticity,
            EEquipProp.Jump,
            EEquipProp.TownMoveSpeedRate,
        };

        private ItemTable mTableData;
        public  ItemTable TableData
        {
            get { return mTableData; }
            set { mTableData = value; }
        }


        public ItemData(int tableId)
        {
            this.mTableData = TableManager.GetInstance().GetTableItem<ItemTable>(tableId);
            if (this.mTableData == null)
            {
                //Logger.LogError("Can't find item with id :" + tableId);
                return;
            }

            this.Name = this.mTableData.Name;
        }

        public bool IsCooling()
        {
            Protocol.ItemCD itemCD = ItemDataManager.GetInstance().GetItemCD(CDGroupID);
            if (itemCD != null)
            {
                double dFinishTime = (double)itemCD.endtime;
                double dTimeLeft = dFinishTime - TimeManager.GetInstance().GetServerDoubleTime();
                return dTimeLeft >= 0;
            }
            return false;
        }

        public bool IsEquiped()
        {
            if (
                ((Type == EItemType.FUCKTITTLE || Type == EItemType.EQUIP) && ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.WearEquip).Contains(GUID)) ||
                (Type == EItemType.FASHION && ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.WearFashion).Contains(GUID))
                )
            {
                return true;
            }
            return false;
        }

        // 给道具弹窗用的接口 add by qxy 2019-07-08 
        public bool CheckBetterThanEquip()
        {
            if (CanEquip() && (PackageType != EPackageType.WearEquip && PackageType != EPackageType.WearFashion && PackageType != EPackageType.Storage && PackageType != EPackageType.RoleStorage))
            {
                bool bHasEquipt = false;
                List<ulong> equips = null;
                if (Type == EItemType.EQUIP || Type == EItemType.FUCKTITTLE)
                {
                    equips = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.WearEquip);
                }
                else if (Type == EItemType.FASHION)
                {
                    equips = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.WearFashion);
                }
                for (int i = 0; equips != null && i < equips.Count; ++i)
                {
                    var item = ItemDataManager.GetInstance().GetItem(equips[i]);
                    if (Type == EItemType.EQUIP || Type == EItemType.FUCKTITTLE)
                    {
                        if (item != null && item.EquipWearSlotType == EquipWearSlotType)
                        {
                            bHasEquipt = true;
                            ProtoTable.ItemTable itemOther = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>((int)TableID);
                            ProtoTable.ItemTable itemSelf = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>((int)item.TableID);
                            if (itemOther != null && itemSelf != null)
                            {
                                //如果是防具，优先判定护甲精通
                                if (item.EquipWearSlotType >= EEquipWearSlotType.EquipHead && item.EquipWearSlotType <= EEquipWearSlotType.EquipBoot)
                                {
                                    // 惩罚装备肯定不能推荐
                                    if (EquipMasterDataManager.GetInstance().IsPunish(PlayerBaseData.GetInstance().JobTableID, (int)Quality, (int)EquipWearSlotType, (int)ThirdType))
                                    {
                                        return false;
                                    }
                                    if (PlayerBaseData.IsJobChanged()) // 转职了
                                    {
                                        if (LevelLimit != item.LevelLimit)
                                        {
                                            // 新规则 董洽定的 规则就很简单，判断玩家身上穿的装备，如果是专精装备，则只会推荐评分更高的专精装备，如果身上不是专精装备，则推荐所有评分高的装备
                                            // 若身上穿戴的防具是专精护甲，则新获得的装备只推荐专精护甲类型，且评分高于身上的装备中，评分最高的那件
                                            if (EquipMasterDataManager.GetInstance().IsMaster(PlayerBaseData.GetInstance().JobTableID, (int)item.ThirdType)
                                                && EquipMasterDataManager.GetInstance().IsMaster(PlayerBaseData.GetInstance().JobTableID, (int)ThirdType)
                                                && finalRateScore > item.finalRateScore)
                                            {
                                                return true;
                                            }

                                            // 若身上穿戴的防具不是专精护甲，则如果新获得的装备如果评分更高，则推荐
                                            if (!EquipMasterDataManager.GetInstance().IsMaster(PlayerBaseData.GetInstance().JobTableID, (int)item.ThirdType)
                                                && finalRateScore > item.finalRateScore)
                                            {
                                                return true;
                                            }
                                        }
                                        else if (LevelLimit == item.LevelLimit)
                                        {
                                            // 新获得的装备等级等于身上的，若身上穿戴的防具是专精护甲，则新获得的装备只推荐专精护甲类型，且评分高于身上的装备中，评分最高的那件
                                            if (EquipMasterDataManager.GetInstance().IsMaster(PlayerBaseData.GetInstance().JobTableID, (int)item.ThirdType)
                                                && EquipMasterDataManager.GetInstance().IsMaster(PlayerBaseData.GetInstance().JobTableID, (int)ThirdType)
                                                && finalRateScore > item.finalRateScore)
                                            {
                                                return true;
                                            }

                                            // 新获得的装备等级等于身上的，若身上穿戴的防具不是专精护甲，则新获得的装备如果是专精护甲，且品质大于等于身上的，则推荐
                                            if (!EquipMasterDataManager.GetInstance().IsMaster(PlayerBaseData.GetInstance().JobTableID, (int)item.ThirdType)
                                                && EquipMasterDataManager.GetInstance().IsMaster(PlayerBaseData.GetInstance().JobTableID, (int)ThirdType)
                                                && Quality >= item.Quality)
                                            {
                                                return true;
                                            }

                                            // 新获得的装备等级等于身上的，若身上穿戴的防具不是专精护甲，则新获得的装备如果也不是专精护甲，且评分大于身上的，则推荐
                                            if (!EquipMasterDataManager.GetInstance().IsMaster(PlayerBaseData.GetInstance().JobTableID, (int)item.ThirdType)
                                                && !EquipMasterDataManager.GetInstance().IsMaster(PlayerBaseData.GetInstance().JobTableID, (int)ThirdType)
                                                && finalRateScore > item.finalRateScore)
                                            {
                                                return true;
                                            }
                                        }
                                    }
                                    else // 没有转职
                                    {
                                        return finalRateScore > item.finalRateScore;
                                    }
                                }
                                else
                                {

//                                  称号：	
//                                  品质高于已穿戴的或无称号，新获得时弹窗        
//                                  品质等于已穿戴的，且评分＞对应部位已穿戴的，新获得时弹窗

                                    if(Type == EItemType.FUCKTITTLE)
                                    {
                                        if(Quality > item.Quality)
                                        {
                                            return true;
                                        }

//                                         if(Quality == item.Quality)
//                                         {
//                                             return finalRateScore > item.finalRateScore;
//                                         }
                                    }
                                    else
                                    {
                                        return finalRateScore > item.finalRateScore;
                                    }
                                }
                            }
                        }
                    }
                    else if (Type == EItemType.FASHION)
                    {
                        if (item != null && item.FashionWearSlotType == FashionWearSlotType)
                        {
                            bHasEquipt = true;
                            ProtoTable.ItemTable itemOther = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>((int)TableID);
                            ProtoTable.ItemTable itemSelf = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>((int)item.TableID);
                            if (itemOther != null && itemSelf != null)
                            {
                                return finalRateScore > item.finalRateScore;
                            }
                        }
                    }
                }

                // 对应的槽位上没有装备
                if (!bHasEquipt)
                {
                    // 没有转职可以推荐
                    if (!PlayerBaseData.IsJobChanged())
                    {
                        return true;
                    }

                    // 转职了且不是惩罚装备也可以推荐
                    if (!(EquipMasterDataManager.GetInstance().IsPunish(PlayerBaseData.GetInstance().JobTableID, (int)Quality, (int)EquipWearSlotType, (int)ThirdType)))
                    {
                        return true;
                    }
                    return false;
                }
            }
            return false;
        }
        public bool IsBetterThanEquip()
        {
            if(CanEquip() && (PackageType != EPackageType.WearEquip && PackageType != EPackageType.WearFashion && PackageType != EPackageType.Storage && PackageType != EPackageType.RoleStorage))
            {
                bool bHasEquipt = false;
                List<ulong> equips = null;
                if (Type == EItemType.EQUIP || Type == EItemType.FUCKTITTLE)
                {
                    equips = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.WearEquip);
                }
                else if(Type == EItemType.FASHION)
                {
                    equips = ItemDataManager.GetInstance().GetItemsByPackageType(EPackageType.WearFashion);
                }

                for(int i = 0; equips != null && i < equips.Count; ++i)
                {
                    var item = ItemDataManager.GetInstance().GetItem(equips[i]);

                    if(Type == EItemType.EQUIP || Type == EItemType.FUCKTITTLE)
                    {
                        if (item != null && item.EquipWearSlotType == EquipWearSlotType)
                        {
                            bHasEquipt = true;
                            ProtoTable.ItemTable itemOther = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>((int)TableID);
                            ProtoTable.ItemTable itemSelf = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>((int)item.TableID);
                            if (itemOther != null && itemSelf != null)
                            {
                                //如果是防具，优先判定护甲精通
                                if (item.EquipWearSlotType >= EEquipWearSlotType.EquipHead && item.EquipWearSlotType <= EEquipWearSlotType.EquipBoot)
                                {
                                    if(EquipMasterDataManager.GetInstance().IsPunish(PlayerBaseData.GetInstance().JobTableID,(int)Quality,(int)EquipWearSlotType,(int)ThirdType))
                                    {
                                        return false;
                                    }

                                    //优先权越低越好
                                    int iNowMasterPriority = EquipMasterDataManager.GetInstance().GetMasterPriority(PlayerBaseData.GetInstance().JobTableID, (int)Quality, (int)EquipWearSlotType, (int)ThirdType);
                                    int iEquipedMasterPriority = EquipMasterDataManager.GetInstance().GetMasterPriority(PlayerBaseData.GetInstance().JobTableID, (int)item.Quality, (int)item.EquipWearSlotType, (int)item.ThirdType);
                                    if(iEquipedMasterPriority != iNowMasterPriority)
                                    {
                                        return iNowMasterPriority < iEquipedMasterPriority;
                                    }

                                    return finalRateScore > item.finalRateScore;
                                }
                                else
                                {
                                    return finalRateScore > item.finalRateScore;
                                }
                            }
                        }
                    }
                    else if(Type == EItemType.FASHION)
                    {
                        if(item != null && item.FashionWearSlotType == FashionWearSlotType)
                        {
                            bHasEquipt = true;
                            ProtoTable.ItemTable itemOther = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>((int)TableID);
                            ProtoTable.ItemTable itemSelf = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>((int)item.TableID);
                            if (itemOther != null && itemSelf != null)
                            {
                                return finalRateScore > item.finalRateScore;
                            }
                        }
                    }
                }
                if(!bHasEquipt)
                {
                    int iNowMasterPriority = EquipMasterDataManager.GetInstance().GetMasterPriority(PlayerBaseData.GetInstance().JobTableID, (int)Quality, (int)EquipWearSlotType, (int)ThirdType);
                    if (Type == EItemType.EQUIP && iNowMasterPriority == 2)
                    {
                        return false;
                    }
                    return true;
                }
            }
            return false;
        }

        public bool IsLevelValid()
        {
            if (PlayerBaseData.GetInstance().Level < LevelLimit)
            {
                return false;
            }
            return true;
        }

        public bool IsLevelFit()
        {
            if (PlayerBaseData.GetInstance().Level < LevelLimit)
            {
                return false;
            }
            return true;
        }

        public bool IsPreLevelFit(int iPreLv)
        {
            if(iPreLv > 1 && iPreLv - 1 >= LevelLimit)
            {
                return true;
            }

            return false;
        }

        public bool IsOccupationFit()
        {
            if (OccupationLimit.Count > 0)
            {
                bool find = false;
                for (int i = 0; i < OccupationLimit.Count; ++i)
                {
                    int nJob = OccupationLimit[i];
                    if (nJob > 0)
                    {
                        if (PlayerBaseData.GetInstance().ActiveJobTableIDs.Contains(nJob))
                        {
                            find = true;
                        }
                    }
                    else if (nJob < 0)
                    {
                        if (nJob == -1)
                        {
                            // -1 表示所有转职后的职业
                            if (PlayerBaseData.GetInstance().ActiveJobTableIDs.Count > 1)
                            {
                                find = true;
                            }
                        }
                        else
                        {
                            // -n 表示职业ID为n的职业，转职后的所有职业
                            ProtoTable.JobTable table = TableManager.instance.GetTableItem<ProtoTable.JobTable>(-nJob);
                            if (table != null)
                            {
                                for (int j = 0; j < table.ToJob.Count; ++j)
                                {
                                    if (PlayerBaseData.GetInstance().ActiveJobTableIDs.Contains(table.ToJob[j]))
                                    {
                                        find = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    if (find)
                    {
                        break;
                    }
                }
                if (find == false)
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsWearSoltEqual(ItemData data)
        {
            if (
                (data.Type == EItemType.FUCKTITTLE && data.EquipWearSlotType == EquipWearSlotType) ||
                (data.Type == EItemType.EQUIP && Type == EItemType.EQUIP && data.EquipWearSlotType == EquipWearSlotType) ||
                (data.Type == EItemType.FASHION && Type == EItemType.FASHION && data.FashionWearSlotType == FashionWearSlotType)
                )
            {
                return true;
            }
            return false;
        }

        public bool IsItemInAuctionPackage()
        {
            if (PackageType == EPackageType.Equip)
            {
                return true;
            }
            else if (PackageType == EPackageType.Material)
            {
                return true;
            }
            else if (PackageType == EPackageType.Consumable)
            {
                return true;
            }
            else if (PackageType == EPackageType.Task)
            {
                return true;
            }
            else if (PackageType == EPackageType.Fashion)
            {
                return true;
            }
            else if (PackageType == EPackageType.Title)
            {
                return true;
            }
            else if (PackageType == EPackageType.Bxy)
            {
                return true;
            }
            else if (PackageType == EPackageType.Sinan)
            {
                return true;
            }

            return false;
        }

        public bool CanGiftUse()
        {
            if(SubType != (int)ProtoTable.ItemTable.eSubType.GiftPackage)
            {
                return false;
            }

            if(UseType == ProtoTable.ItemTable.eCanUse.CanNot)
            {
                return false;
            }

            if(!IsLevelFit())
            {
                return false;
            }

            if(!IsOccupationFit())
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 是否可交易给其他人
        /// </summary>
        public bool CanTrade()
        {
            if (BindAttr == EItemBindAttr.NOTBIND)
            {
                return true;
            }
            else
            {
                if (Packing)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 是否有交易的可能性
        /// </summary>
        public bool HasTradePossibility()
        {
            if (BindAttr == EItemBindAttr.NOTBIND)
            {
                return true;
            }
            else
            {
                if (Packing)
                {
                    return true;
                }
                else
                {
                    if(RePackTime > 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool IsNeedPacking()
        {
            if(!Packing && RePackTime > 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 是否可放入账号仓库
        /// </summary>
        public bool CanStore()
        {
            if (isInSidePack)
                return false;

            if (BindAttr == EItemBindAttr.NOTBIND || BindAttr == EItemBindAttr.ACCBIND)
            {
                return true;
            }
            else if (BindAttr == EItemBindAttr.ROLEBIND)
            {
                if (Packing || HasTransfered)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 当前是否能装备
        /// </summary>
        public bool CanEquip()
        {
//             if(PackageType == EPackageType.Storage)
//             {
//                 return false;
//             }

            if (
                (Type == EItemType.FUCKTITTLE && EquipWearSlotType > EEquipWearSlotType.EquipInvalid && EquipWearSlotType < EEquipWearSlotType.EquipMax) ||
                (Type == EItemType.EQUIP && EquipWearSlotType > EEquipWearSlotType.EquipInvalid && EquipWearSlotType < EEquipWearSlotType.EquipMax) ||
                (Type == EItemType.FASHION && FashionWearSlotType > EFashionWearSlotType.Invalid && FashionWearSlotType < EFashionWearSlotType.Max)
                )
            {
                if (IsLevelFit() && IsOccupationFit())
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 将来是否可以装备（有无装备的可能）
        /// </summary>
        public bool WillCanEquip()
        {
            if (
                (Type == EItemType.FUCKTITTLE && EquipWearSlotType > EEquipWearSlotType.EquipInvalid && EquipWearSlotType < EEquipWearSlotType.EquipMax) ||
                (Type == EItemType.EQUIP && EquipWearSlotType > EEquipWearSlotType.EquipInvalid && EquipWearSlotType < EEquipWearSlotType.EquipMax) ||
                (Type == EItemType.FASHION && FashionWearSlotType > EFashionWearSlotType.Invalid && FashionWearSlotType < EFashionWearSlotType.Max)
                )
            {
                if (IsOccupationFit())
                {
                    return true;
                }
            }

            return false;
        }

        public CrypticInt32[] GetDiffPropBaseOn(ItemData baseOn)
        {
            EquipProp diffProp;
            diffProp = GetEquipProp();
            if (baseOn != null)
            {
                diffProp -= baseOn.GetEquipProp();
            }

            return diffProp.props;
        }

        public ItemProperty GetBattleProperty(int strengthen = 0)
        {
            EquipProp prop = GetEquipProp();

            if(prop != null)
            {
                return prop.ToItemProp(StrengthenLevel, (int)EquipWearSlotType, GrowthAttrType, GrowthAttrNum);
            }

            return new ItemProperty();
        }

        /// <summary>
        /// 装备评分的基础属性不包含铭文的属性加成，所以单独写出一个函数
        /// </summary>
        /// <returns></returns>
        public ItemProperty GetPropertyForEquipScore()
        {
            return GetEquipPropForEquipScore().ToItemProp(StrengthenLevel, (int)EquipWearSlotType, GrowthAttrType, GrowthAttrNum);
        }

        /// <summary>
        /// 该函数计算的是所有基础属性加成的总值，用于战斗
        /// </summary>
        /// <returns></returns>
        public EquipProp GetEquipProp()
        {
            EquipProp prop = BaseProp + RandamProp + MagicProp + BeadProp + IncriptionProp;
            prop.props[(int)EEquipProp.AttackSpeedRate] += BaseAttackSpeedRate;
            return prop;
        }

        /// <summary>
        /// 铭文的属性加成在计算装备评分时，不带入基础属性内，铭文的评分单独计算，所以与该函数GetEquipProp()分开处理
        /// </summary>
        /// <returns></returns>
        public EquipProp GetEquipPropForEquipScore()
        {
            EquipProp prop = BaseProp;

            prop += RandamProp;
            prop += MagicProp;
            prop += BeadProp;

            prop.props[(int)EEquipProp.AttackSpeedRate] += BaseAttackSpeedRate;
            return prop;
        }

        public QualityInfo GetQualityInfo()
        {
            for (int i = 0; i < ms_qualityInfos.Length; ++i)
            {
                QualityInfo info = ms_qualityInfos[i];
                if (info.Quality == Quality)
                {
                    if(info.Quality == EItemQuality.WHITE)
                    {
                        var tableItem = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(TableID);
                        if(null != tableItem && tableItem.Color2 == 1)
                        {
                            return ms_grey_quality;
                        }
                    }
                    //粉红色中的玫红
                    if (info.Quality == EItemQuality.PINK)
                    {
                        var tableItem = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(TableID);
                        if (null != tableItem && tableItem.Color2 == 3)
                        {
                            return ms_rosered_quality;
                        }
                    }

                    return info;
                }
            }
            return null;
        }

        public string GetQualityTipTitleBackGround()
        {
            QualityInfo qf = GetQualityInfo();
            if (null == qf)
            {
                return string.Empty;
            }

            return qf.TipTitleBackGround;
        }


        public string GetColorName(string a_strFormat = "", bool a_bWithStrengthLevel = false)
        {
            string strContent = string.Empty;
            if (a_bWithStrengthLevel && StrengthenLevel > 0)
            {
                strContent = string.Format("+{0}{1}", StrengthenLevel, Name);
            }
            else
            {
                strContent = Name;
            }

            ItemData.QualityInfo qualityInfo = GetQualityInfo();
            if (string.IsNullOrEmpty(a_strFormat) == false)
            {
                return TR.Value("common_color_text", qualityInfo.ColStr, string.Format(a_strFormat, strContent));
            }
            else
            {
                return TR.Value("common_color_text", qualityInfo.ColStr, strContent);
            }
        }

        public string GetColorLevel()
        {
            if (LevelLimit > 0)
            {
                ItemData.QualityInfo qualityInfo = GetQualityInfo();
                return TR.Value("common_color_text", qualityInfo.ColStr, LevelLimit);
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// a_bStartCountdown == true   => 已经开始倒计时
        /// -- a_timeLeft > 0           => 道具还有效
        /// -- a_timeLeft <= 0          => 道具已失效
        /// a_bStartCountdown == false  => 没有开始倒计时
        /// -- a_timeLeft > 0           => 有时长限制
        /// -- a_timeLeft <= 0          => 没有时长限制
        /// </summary>
        public void GetTimeLeft(out int a_timeLeft, out bool a_bStartCountdown)
        {
            if (DeadTimestamp <= 0)
            {
                if (CanRenewal())
                {
                    // 可以续费的道具，如果没有到期时间，则表示已经永久续费
                    // 等价于没有时长限制
                    a_timeLeft = 0;
                }
                else
                {
                    a_timeLeft = FixTimeLeft;
                }
                a_bStartCountdown = false;
                if (a_timeLeft > 0)
                {
                    a_bStartCountdown = true;
                }
            }
            else
            {
                a_timeLeft = DeadTimestamp - (int)TimeManager.GetInstance().GetServerTime();
                a_bStartCountdown = true;
            }
        }

        /// <summary>
        /// 判断限时道具是否过期
        /// </summary>
        /// <param name="a_timeLeft"></param>
        /// <param name="a_bStartCountdown"></param>
        public void GetLimitTimeLeft(out int a_timeLeft, out bool a_bStartCountdown)
        {
            if (DeadTimestamp > 0)
            {
                a_timeLeft = DeadTimestamp - (int)TimeManager.GetInstance().GetServerTime();
                a_bStartCountdown = true;
            }
            else
            {
                a_timeLeft = 0;
                a_bStartCountdown = false;
            }
        }

        public bool CanRenewal()
        {
            return arrRenewals != null && arrRenewals.Count > 0;
        }

        public int GetMaxUseTime()
        {
            int nMaxUseTime = 0;
            if (useLimitType == EItemUseLimitType.NOLIMITE)
            {
                nMaxUseTime = ItemData.unlimitedUseTimes;
            }
            else if (useLimitType == EItemUseLimitType.DAYLIMITE)
            {
                nMaxUseTime = useLimitValue;
            }
            else if (useLimitType == EItemUseLimitType.VIPLIMITE)
            {
                nMaxUseTime =(int)Utility.GetCurVipLevelPrivilegeData(useLimitValue);
            }
            else if (useLimitType == EItemUseLimitType.TEAMCOPYLIMITE)
            {
                nMaxUseTime = useLimitValue;
            }

            if (nMaxUseTime < 0)
            {
                nMaxUseTime = 0;
            }

            return nMaxUseTime;
        }

        /// <summary>
        /// 获得当前剩余使用次数，ItemData.unlimitedUseTimes表示无穷次
        /// </summary>
        /// <returns></returns>
        public int GetCurrentRemainUseTime()
        {
            int nRemainTime = GetMaxUseTime() - GetCurrentUseTime();
            if (nRemainTime < 0)
            {
                nRemainTime = 0;
            }
            return nRemainTime;
        }

        /// <summary>
        /// 得到当前团本通行证剩余使用次数
        /// </summary>
        /// <returns></returns>
        public int GetCurrentTeamCopyLimiteRemainUseTime()
        {
            int nRemainTime = GetMaxUseTime() - GetCurrentTeamCopyLimiteUseTime();
            if (nRemainTime < 0)
            {
                nRemainTime = 0;
            }
            return nRemainTime;
        }

        /// <summary>
        /// 得到当前团本通行证使用次数
        /// </summary>
        /// <returns></returns>
        public int GetCurrentTeamCopyLimiteUseTime()
        {
            return CountDataManager.GetInstance().GetCount(string.Format("{0}{1}", CounterKeys.COUNTER_ITEM_WEEKUSE_PRE, TableID));
        }

        public int GetCurrentUseTime()
        {
            return CountDataManager.GetInstance().GetCount(
                string.Format("{0}{1}", CounterKeys.COUNTER_ITEM_DAYUSE_PRE, TableID)
                );
        }

        // 是否是辅助装备
        public bool IsAssistEquip()
        {
            if(TableData == null)
            {
                return false;
            }
            return (TableData.SubType == ItemTable.eSubType.ST_ASSIST_EQUIP 
                || TableData.SubType == ItemTable.eSubType.ST_EARRINGS_EQUIP 
                || TableData.SubType == ItemTable.eSubType.ST_MAGICSTONE_EQUIP);
        }

        // 是否是辟邪玉 by ckm
        public bool IsBxyEquip()
        {
            if(TableData == null)
            {
                return false;
            }
            return (TableData.SubType == ItemTable.eSubType.ST_BXY_EQUIP);
        }

        // 获取四维基础属性强化加成值
        public int GetBaseFourAttrStrengthenAddUpValue(int strengthen)
        {
            if(!IsAssistEquip()) // 目前只有辅助装备有基础属性加强
            {
                return 0;
            }
            if(this.TableData == null)
            {
                return 0;
            }
            Dictionary<int, object> dicts = TableManager.instance.GetTable<AssistEqStrengFouerDimAddTable>();
            if (dicts != null)
            {
                var iter = dicts.GetEnumerator();
                while (iter.MoveNext())
                {
                    AssistEqStrengFouerDimAddTable adt = iter.Current.Value as AssistEqStrengFouerDimAddTable;
                    if (adt == null)
                    {
                        continue;
                    }                   
                    if(adt.Strengthen == strengthenLevel && (int)adt.Color == (int)this.TableData.Color && adt.Color2 == this.TableData.Color2 && adt.Lv == this.TableData.NeedLevel)
                    {
                        return adt.StrengNum / 1000; // 注意 表格中此字段为float，转表工具会将表中的值乘上1000转为int值 例如 表格中填写2，则最终会变为2000
                    }
                }
            }
            return 0;
        }
        public List<GiftTable> GetGifts()
        {
            if (PackID > 0)
            {
                GiftPackTable table = TableManager.GetInstance().GetTableItem<GiftPackTable>(PackID);
                if (table != null)
                {
                    List<GiftTable> arrGifts = new List<GiftTable>();
                    switch (table.FilterType)
                    {
                        case GiftPackTable.eFilterType.None:
                        case GiftPackTable.eFilterType.Custom:
                        case GiftPackTable.eFilterType.Random:
                            {
                                arrGifts = TableManager.GetInstance().GetGiftTableData(table.ID);
                                break;
                            }
                        case GiftPackTable.eFilterType.CustomWithJob:
                        case GiftPackTable.eFilterType.Job:
                            {
                                var giftDataList = TableManager.GetInstance().GetGiftTableData(table.ID);
                                for (int i = 0; i < giftDataList.Count; ++i)
                                {
                                    GiftTable giftTable = giftDataList[i];
                                    if (giftTable != null && giftTable.RecommendJobs.Contains(PlayerBaseData.GetInstance().JobTableID))
                                    {
                                        arrGifts.Add(giftTable);
                                    }
                                }
                                break;
                            }
                    }
                    return arrGifts;
                }
            }

            return null;
        }

        #region descs
        public string GetItemTypeDesc()
        {
            string typeName = " ";
            ProtoTable.ItemTable itemInfo = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>((int)TableID);
            if (itemInfo != null)
            {
                typeName = itemInfo.TypeName;
            }

            if (string.IsNullOrEmpty(typeName))
            {
                typeName = " ";
            }

            return typeName;
        }

        public string GetSubTypeDesc()
        {
            string subTypeName = " ";
            ProtoTable.ItemTable itemInfo = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>((int)TableID);
            if (itemInfo != null)
            {
                subTypeName = itemInfo.SubTypeName;
            }

            if(string.IsNullOrEmpty(subTypeName))
            {
                subTypeName = " ";
            }

            return subTypeName;
        }

        public string GetThirdTypeDesc()
        {
            string thirdTypeName = " ";
            ProtoTable.ItemTable itemInfo = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>((int)TableID);
            if (itemInfo != null)
            {
                thirdTypeName = itemInfo.ThirdTypeName;
            }

            if (string.IsNullOrEmpty(thirdTypeName))
            {
                thirdTypeName = " ";
            }
            return thirdTypeName;
        }

        public string GetBeadTypeDesc()
        {
            string sDesc = "";
            BeadTable mBeadTable = TableManager.GetInstance().GetTableItem<BeadTable>((int)TableID);
            if (mBeadTable != null)
            {
                if (mBeadTable.BeadType == 2)
                {
                    sDesc = TR.Value("bead_colorFul_des");
                }
                else
                {
                    sDesc = TR.Value("bead_nomal_des");
                }
            }
            return sDesc;
        }

        public string GetWearSoltTypeDesc()
        {
            if (EquipWearSlotType != EEquipWearSlotType.EquipInvalid)
            {
                return TR.Value(EquipWearSlotType.GetDescription());
            }
            else if (FashionWearSlotType != EFashionWearSlotType.Invalid)
            {
                return TR.Value(FashionWearSlotType.GetDescription());
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// 装备描述
        /// 武器：类型   速度（攻速用现在武器类型换算，比如巨剑是0.91，太刀是1.08）					
        /// 防具：部位   类型					
        /// 首饰：部位					
        /// </summary>
        public string GetEquipTypeDesc()
        {
            ProtoTable.ItemTable itemInfo = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>((int)TableID);
            if (null == itemInfo)
            {
                return string.Empty;
            }

            if (itemInfo.Type != ItemTable.eType.EQUIP)
            {
                return string.Empty;
            }

            //  WEAPON:1:武器
            //  HEAD:2:头部
            //  CHEST:3:胸
            //  BELT:4:腰带
            //  LEG:5:护腿
            //  BOOT:6:鞋子
            //  RING:7:戒指
            //  NECKLASE:8:项链
            //  BRACELET:9:手镯

            switch (itemInfo.SubType)
            {
                case ItemTable.eSubType.WEAPON:
                    return string.Format("{0}\t{1}", GetThirdTypeDesc(), GetWeaponAttackSpeedDesc());
                case ItemTable.eSubType.HEAD:
                case ItemTable.eSubType.CHEST:
                case ItemTable.eSubType.BELT:
                case ItemTable.eSubType.LEG:
                case ItemTable.eSubType.BOOT:
                    return string.Format("{0}-{1}", GetSubTypeDesc(), GetThirdTypeDesc());
                case ItemTable.eSubType.RING:
                case ItemTable.eSubType.NECKLASE:
                case ItemTable.eSubType.BRACELET:
                    return string.Format("{0}", GetSubTypeDesc());
            }

            return GetSubTypeDesc();
        }

        public Color GetEquipGradeColor()
        {
            //粗糙
            if (SubQuality <= 20)
            {
                return new Color(95 / 255.0f, 89 / 255.0f, 83 / 255.0f);
            } //普通
            else if (SubQuality <= 40)
            {
                return new Color(63 / 255.0f, 100 / 255.0f, 147 / 255.0f);
            } //优秀
            else if (SubQuality <= 60)
            {
                return new Color(127 / 255.0f, 54 / 255.0f, 158 / 255.0f);
            } //精良
            else if (SubQuality <= 80)
            {
                return new Color(172 / 255.0f, 51 / 255.0f, 113 / 255.0f);
            } //卓越
            else if (SubQuality < 100)
            {
                return new Color(190 / 255.0f, 113 / 255.0f, 37 / 255.0f);
            } //完美
            else
            {
                return new Color(238 / 255.0f, 190 / 255.0f, 21 / 255.0f);
            }
        }

        /// <summary>
        /// 装备品级
        /// </summary>
        public string GetEquipGradeDesc()
        {
            #region SubQuality
            if (SubQuality <= 20)
            {
                return TR.Value("tip_grade_lower_most", SubQuality);
            }
            else if (SubQuality <= 40)
            {
                return TR.Value("tip_grade_lower", SubQuality);
            }
            else if (SubQuality <= 60)
            {
                return TR.Value("tip_grade_middle", SubQuality);
            }
            else if (SubQuality <= 80)
            {
                return TR.Value("tip_grade_high", SubQuality);
            }
            else if (SubQuality < 100)
            {
                return TR.Value("tip_grade_high_most", SubQuality);
            }
            else
            {
                return TR.Value("tip_grade_perfect", SubQuality);
            }
            #endregion
        }

        /// <summary>
        /// 装备品级（洗炼系统使用）
        /// </summary>
        /// <returns></returns>
        public string GetEquipmentGradeDesc()
        {
            if (SubQuality <= 20)
            {
                return TR.Value("tip_grade_lower_most_1", SubQuality);
            }
            else if (SubQuality <= 40)
            {
                return TR.Value("tip_grade_lower_1", SubQuality);
            }
            else if (SubQuality <= 60)
            {
                return TR.Value("tip_grade_middle_1", SubQuality);
            }
            else if (SubQuality <= 80)
            {
                return TR.Value("tip_grade_high_1", SubQuality);
            }
            else if (SubQuality < 100)
            {
                return TR.Value("tip_grade_high_most_1", SubQuality);
            }
            else
            {
                return TR.Value("tip_grade_perfect_1", SubQuality);
            }
        }

        /// <summary>
        /// 道具品质
        /// </summary>
        public string GetQualityDesc()
        {
            QualityInfo qualityInfo = GetQualityInfo();
            if (qualityInfo != null)
            {
                return string.Format("<color={0}>{1}</color>", qualityInfo.ColStr, qualityInfo.Desc);
            }
            return "";
        }

        /// <summary>
        /// 绑定状态
        /// </summary>
         public string GetBindStateDesc()
        {
            string ret = string.Empty;

            if (BindAttr == EItemBindAttr.NOTBIND)
            {
                ret = TR.Value("tip_color_yellow", TR.Value("tip_no_bind"));
            }
            else if (BindAttr == EItemBindAttr.ROLEBIND)
            {
                if (Packing == true)
                {
                    ret = TR.Value("tip_color_yellow", TR.Value("tip_packing"));
                }
                else
                {
                    ret = TR.Value("tip_color_yellow", TR.Value("tip_role_bind"));
                }
            }
            else if (BindAttr == EItemBindAttr.ACCBIND)
            {
                if (Packing == true)
                {
                    ret = TR.Value("tip_color_yellow", TR.Value("tip_packing"));
                }
                else
                {
                    ret = TR.Value("tip_color_yellow", TR.Value("tip_account_bind"));
                }
            }

  			if (HasTransfered)
            {
                ret = TR.Value("tip_color_yellow", TR.Value("tip_equip_transfer"));
            }

            //ret += string.Format(" {0}", GetRepackTimeDesc());

            return ret;
        }

        /// <summary>
        /// 绑定归属状态
        /// </summary>
        public string GetBindStateOwnerDesc()
        {
            return string.Empty;

            switch (BindAttr)
            {
                case EItemBindAttr.NOTBIND:
                    return TR.Value("tip_can_sell", RePackTime);
                case EItemBindAttr.ACCBIND:
                    return TR.Value("tip_account_bind", RePackTime);
                case EItemBindAttr.ROLEBIND:
					if (!HasTransfered)
                    {
						return TR.Value("tip_role_bind", RePackTime);
					}
					break;
            }

            return string.Empty;
        }

        /// <summary>
        /// 剩余封装次数
        /// </summary>
        public string GetRepackTimeDesc()
        {
            if (Type== ProtoTable.ItemTable.eType.EQUIP||Type== ProtoTable.ItemTable.eType.FUCKTITTLE)
            {
                if (RePackTime > 0)
                {
                    return TR.Value("tip_repack_time", RePackTime);
                }
                else if (RePackTime == 0)
                {
                    return TR.Value("tip_no_repack_time");
                }
            }
            return " ";
        }


        /// <summary>
        /// 最大堆叠数量
        /// </summary>
        public string GetMaxStackCountDesc()
        {
            if (MaxStackCount > 1)
            {
                return TR.Value("tip_max_stack_count", MaxStackCount);
            }
            return " ";
        }

        /// <summary>
        /// 每日使用次数限制
        /// </summary>
        public string GetUseTimePerDayDesc()
        {
            return "";
        }

        /// <summary>
        /// 等级限制属性
        /// </summary>
        public string GetLevelLimitDesc()
        {
            string desc = " ";
            if (LevelLimit > 0)
            {
                string color_format = LevelLimit <= PlayerBaseData.GetInstance().Level ? "tip_color_normal" : "tip_color_bad";
                desc = TR.Value(color_format, TR.Value("tip_level_limit", LevelLimit));
            }
            return desc;
        }

        /// <summary>
        /// 消耗品等级限制
        /// </summary>
        /// <returns></returns>
        public string GetExpendableLimitDesc()
        {
            string desc = "";
            if (LevelLimit > 0)
            {
                string color_format = LevelLimit <= PlayerBaseData.GetInstance().Level ? "tip_color_good" : "tip_color_bad";
                desc = TR.Value(color_format, TR.Value("equipforge_tip_level_limit", LevelLimit));
            }
            return desc;
        }

        /// <summary>
        /// 得到仓库描述
        /// </summary>
        /// <returns></returns>
        public string GetStoreDesc()
        {
            string desc = "";

            if (CanStore())
            {
                desc = TR.Value("can_put_storage");
            }
            else
            {
                desc = TR.Value("cannot_put_storage");
            }

            return desc;
        }
		
		 /// <summary>
        /// 得到装备可升级描述
        /// </summary>
        /// <returns></returns>
        public string GetEquipUpgradeDesc()
        {
            string desc = "";

            bool isFind = EquipUpgradeDataManager.GetInstance().FindEquipUpgradeTableID(TableID);
            if (isFind)
            {
                desc = "可升级";
            }

            return desc;
        }

        /// <summary>
        /// 经验丸的限制属性
        /// </summary>
        /// <returns></returns>
        public string GetExperiencePillLevelLimitDesc()
        {
            string desc = " ";
            if (LevelLimit > 0)
            {
                string color_format = ((LevelLimit <= PlayerBaseData.GetInstance().Level ) && (PlayerBaseData.GetInstance().Level <= MaxLevelLimit))? "tip_color_good" : "tip_color_bad";
                if (LevelLimit == MaxLevelLimit)
                {
                    desc = TR.Value(color_format, TR.Value("tip_level_limit_2", LevelLimit));
                }
                else
                {
                    desc = TR.Value(color_format, TR.Value("tip_level_limit_maxlimit", LevelLimit, MaxLevelLimit));
                }
            }
            return desc;
        }

        /// <summary>
        /// 装备评分
        /// </summary>
		public string GetRateScoreDesc()
		{
            if (finalRateScore >= 0)
            {
                return TR.Value("tip_rate_score", finalRateScore);
            }
            else
            {
                return string.Empty;
            }
		}

        //交易冷却时间
        public string GetItemAuctionCoolTimeDesc()
        {
            if (AuctionCoolTimeStamp <= 0 || AuctionCoolTimeStamp <= TimeManager.GetInstance().GetServerTime())
            {
                return string.Empty;
            }
            else
            {
                var coolTimeDes = CountDownTimeUtility.GetCoolDownTimeByDayHour(AuctionCoolTimeStamp,
                    TimeManager.GetInstance().GetServerTime());
                return string.Format("<color={0}>{1}</color>", TR.Value("tip_color_red_noparm"), TR.Value("auction_new_item_tip_cool_in_cd_time", coolTimeDes));
            }
            
        }

        /// <summary>
        /// 可交易次数
        /// </summary>
        /// <returns></returns>
        public string GetTransactionNumberDesc()
        {
            string desc = string.Empty;

            bool isFlag = ItemDataUtility.IsItemTradeLimitBuyNumber(this);
            if (isFlag == true)
            {
                desc = string.Format("可交易次数:{0}", ItemDataUtility.GetItemTradeLeftTime(this));
            }

            return desc;
        }

        /// <summary>
        /// 属性重置免费次数
        /// </summary>
        public string GetFasionFreeTimesDesc()
        {
            if(FashionFreeTimes > 0)
            {
                return "";
            }

            return TR.Value("fashion_free_times_tips",FashionFreeTimes > 0 ? 0 : 1);
        }

        /// <summary>
        /// 职业限制属性
        /// </summary>
        public string GetOccupationLimitDesc()
        {
            string desc = "";
            if (OccupationLimit.Count > 0)
            {
                string color_format = IsOccupationFit() ? "tip_color_good" : "tip_color_bad";

                if (OccupationLimit.Contains(-1))
                {
                    desc = TR.Value(color_format, TR.Value("tip_job_limit_need_any_transfer"));
                }
                else
                {
                    for (int i = 0; i < OccupationLimit.Count; ++i)
                    {
                        if (OccupationLimit[i] < 0)
                        {
                            ProtoTable.JobTable table = TableManager.instance.GetTableItem<ProtoTable.JobTable>(-OccupationLimit[i]);
                            if (table != null)
                            {
                                if (string.IsNullOrEmpty(desc) == false)
                                {
                                    desc += "、";
                                }
                                desc += TR.Value("tip_job_limit_need_transfer", table.Name);
                            }
                        }
                        else
                        {
                            ProtoTable.JobTable table = TableManager.instance.GetTableItem<ProtoTable.JobTable>(OccupationLimit[i]);
                            if (table != null)
                            {
                                if (string.IsNullOrEmpty(desc) == false)
                                {
                                    desc += "、";
                                }
                                desc += table.Name;
                            }
                        }
                    }
                    desc = TR.Value(color_format, TR.Value("tip_job_limit", desc));
                }

            }
            return desc;
        }
        
        /// <summary>
        /// 得到剩余使用次数
        /// </summary>
        /// <returns></returns>
        public string GetRemainUseNumberDesc()
        {
            string desc = string.Empty;

            if (TableData.UseLinmit > 0)
            {
                int useNum = CountDataManager.GetInstance().GetCount(string.Format("item_fly_hell_gift_{0}", TableData.ID));
                int remainUseNum = TableData.UseLinmit - useNum;
                desc = string.Format("剩余使用次数:{0}", remainUseNum);
            }

            return desc;
        }

       

        /// <summary>
        /// 价格描述
        /// </summary>
        public string GetPriceDesc()
        {
            if (CanSell)
            {
                ItemData itemData = ItemDataManager.GetInstance().GetCommonItemTableDataByID(PriceItemID);
                if (itemData != null)
                {
                    return TR.Value("tip_price", Price, itemData.Name);
                }
            }
            return "";
        }
        
        /// <summary>
        /// 使用次数描述
        /// </summary>
        public string GetUseTimeDesc()
        {
            if (GetMaxUseTime() == unlimitedUseTimes)
            {
                return string.Empty;
            }
            else
            {
                //如果使用限制类型是团本通行证
                if (useLimitType == EItemUseLimitType.TEAMCOPYLIMITE)
                {
                    return TR.Value("tip_use_time_week", GetCurrentTeamCopyLimiteRemainUseTime(), GetMaxUseTime());
                }
                else
                {
                    return TR.Value("tip_use_time", GetCurrentRemainUseTime(), GetMaxUseTime());
                }
            }
        }


        /// <summary>
        /// 剩余时间，过期时间
        /// </summary>
        public string GetTimeLeftDescByDay()
        {
            int timeLeft;
            bool bStartCountdown;
            GetTimeLeft(out timeLeft, out bStartCountdown);
            if(timeLeft > 0)
            {
                if(timeLeft > 86400)
                {
                    int iDays = timeLeft / 86400;
                    return string.Format("{0}天", iDays);
                }

                if(timeLeft > 3600)
                {
                    int iHours = timeLeft / 3600;
                    return string.Format("{0}小时", iHours);
                }

                if(timeLeft > 60)
                {
                    int iMinutes = timeLeft / 60;
                    return string.Format("{0}分", iMinutes);
                }

                return string.Format("{0}秒", timeLeft);
            }
            else
            {
                if (bStartCountdown)
                {
                    return TR.Value("tip_color_bad", TR.Value("tip_time_left_invalid"));
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// 时限道具的剩余时间
        /// </summary>
        public string GetTimeLeftDesc()
        {
            int timeLeft;
            bool bStartCountdown;
            GetTimeLeft(out timeLeft, out bStartCountdown);
            if (timeLeft > 0)
            {
                int second = 0;
                int minute = 0;
                int hour = 0;
                int day = 0;
                second = timeLeft % 60;
                int temp = timeLeft / 60;
                if (temp > 0)
                {
                    minute = temp % 60;
                    temp = temp / 60;
                    if (temp > 0)
                    {
                        hour = temp % 24;
                        day = temp / 24;
                    }
                }

                string value = "";
                if (day > 0)
                {
                    value += string.Format("{0}天", day);
                }
                if (hour > 0)
                {
                    value += string.Format("{0}小时", hour);
                }
                if (minute > 0)
                {
                    value += string.Format("{0}分", minute);
                }
                if (second > 0)
                {
                    value += string.Format("{0}秒", second);
                }
                string desc = TR.Value("tip_time_left", value);
                string color_format = timeLeft > 86400 ? "tip_color_normal" : "tip_color_bad";
                return TR.Value(color_format, desc);
            }
            else
            {
                if (bStartCountdown)
                {
                    return TR.Value("tip_color_bad", TR.Value("tip_time_left_invalid"));
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// 无效日期
        /// </summary>
        public string GetDeadTimestampDesc()
        {
            if (DeadTimestamp > 0)
            {
                int timeLeft;
                bool bStartCountdown;
                GetTimeLeft(out timeLeft, out bStartCountdown);
                if (bStartCountdown && timeLeft <= 0)
                {
                    int nDeleteTime = DeadTimestamp + 
                        TableManager.GetInstance().GetTableItem<SystemValueTable>((int)SystemValueTable.eType.SVT_TIME_ITEM_DELETE_INTERVAL).Value * 86400;
                    System.DateTime time = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
                    time = time.AddSeconds(nDeleteTime);
                    return TR.Value("tip_color_gray2", TR.Value("tip_item_delete_timestrmp", time.ToString(TR.Value("tip_timestrmp"))));
                }
                else
                {
                    System.DateTime time = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
                    time = time.AddSeconds(DeadTimestamp);
                    return TR.Value("tip_color_gray2", TR.Value("tip_item_dead_timestrmp", time.ToString(TR.Value("tip_timestrmp"))));
                }
            }
            return "";
        }

        /// <summary>
        /// 冷却时间
        /// </summary>
        public string GetCoolTimeDesc()
        {
            if (CD > 0)
            {
                int second = 0;
                int minute = 0;
                int hour = 0;
                int day = 0;
                second = CD % 60;
                int temp = CD / 60;
                if (temp > 0)
                {
                    minute = temp % 60;
                    temp = temp / 60;
                    if (temp > 0)
                    {
                        hour = temp % 24;
                        day = temp / 24;
                    }
                }

                string value = "";
                if (day > 0)
                {
                    value += string.Format("{0}天", day);
                }
                if (hour > 0)
                {
                    value += string.Format("{0}时", hour);
                }
                if (minute > 0)
                {
                    value += string.Format("{0}分", minute);
                }
                if (second > 0)
                {
                    value += string.Format("{0}秒", second);
                }
                return TR.Value("tip_cool_time", value);
            }
            return "";
        }

        public float _GetGetStrengthenDescs(EEquipProp eEEquipProp)
        {
            float floatValue = BaseProp.props[(int)eEEquipProp] * 1.0f / EquipPropRate.propRates[(int)eEEquipProp];

            switch(eEEquipProp)
            {
                case EEquipProp.IgnorePhysicsDefenseRate:
                case EEquipProp.IgnoreMagicDefenseRate:
                    floatValue = (floatValue * 100.0f + 0.50f);
                    break;
            }
            //int intValue = Mathf.FloorToInt(floatValue);
            return floatValue;
        }

        public int ConvertAttrByValue(EEquipProp eEEquipProp,int iValue)
        {
            float floatValue = iValue * 1.0f / EquipPropRate.propRates[(int)eEEquipProp];
            int intValue = Mathf.FloorToInt(floatValue);
            return intValue;
        }

        public float ConvertAttrByValue(EEquipProp eEEquipProp, float iValue)
        {
            float floatValue = iValue / EquipPropRate.propRates[(int)eEEquipProp];
            return floatValue;
        }

        public string GetBaseAttrStrengthenDesc(EEquipProp eEquipProp)
        {
            if(eEquipProp != EEquipProp.Strenth 
                && eEquipProp != EEquipProp.Intellect 
                && eEquipProp != EEquipProp.Spirit 
                && eEquipProp != EEquipProp.Stamina)
            {
                return "";
            }
            string desc = "";
            int addUp = GetBaseFourAttrStrengthenAddUpValue(strengthenLevel);
            int oldValue = BaseProp.props[(int)eEquipProp];
            BaseProp.props[(int)eEquipProp] = addUp * EquipPropRate.propRates[(int)eEquipProp];
            string temp = BaseProp.GetPropFormatStr(eEquipProp);
            if (string.IsNullOrEmpty(temp) == false)
            {
                if (EquipType == EEquipType.ET_REDMARK)
                {
                    desc = (TR.Value("tip_growth_effect", StrengthenLevel, temp));
                }
                else if (EquipType == EEquipType.ET_COMMON)
                {
                    desc = TR.Value("tip_strengthen_effect", StrengthenLevel, temp);
                }
            }
            BaseProp.props[(int)eEquipProp] = oldValue;
            return desc;
        }
        /// <summary>
        /// 气息装备描述
        /// </summary>
        /// <returns></returns>
        public string GetBreathEquipDesc()
        {
            string content = string.Empty;
            if (EquipType == EEquipType.ET_BREATH)
            {
                content = TR.Value("tip_growth_breath_desc");
            }
            return content;
        }

        /// <summary>
        /// 强化属性
        ///
        /// </summary>
        public List<string> GetStrengthenDescs()
        {
            List<string> descs = new List<string>();

            if (StrengthenLevel > 0 || EquipType == EEquipType.ET_REDMARK)
            {
                string temp;

                if (EquipType == EEquipType.ET_REDMARK && StrengthenLevel >= 0)
                {
                    string attrDes = string.Format("{0}+{1}", EquipGrowthDataManager.GetInstance().GetGrowthAttrDesc(GrowthAttrType), GrowthAttrNum);
                    descs.Add (TR.Value("growth_tip_effect_decs", StrengthenLevel, attrDes));
                }

                temp = BaseProp.GetPropFormatStr(EEquipProp.IgnorePhysicsAttack);
                if (string.IsNullOrEmpty(temp) == false)
                {
                    if (EquipType == EEquipType.ET_REDMARK)
                    {
                        descs.Add(TR.Value("tip_growth_effect", StrengthenLevel, temp));
                    }
                    else if (EquipType == EEquipType.ET_COMMON)
                    {
                        descs.Add(TR.Value("tip_strengthen_effect", StrengthenLevel, temp));
                    }
                }

                temp = BaseProp.GetPropFormatStr(EEquipProp.IgnorePhysicsAttackRate);
                if (string.IsNullOrEmpty(temp) == false)
                {
                    if (EquipType == EEquipType.ET_REDMARK)
                    {
                        descs.Add(TR.Value("tip_growth_effect", StrengthenLevel, temp));
                    }
                    else if (EquipType == EEquipType.ET_COMMON)
                    {
                        descs.Add(TR.Value("tip_strengthen_effect", StrengthenLevel, temp));
                    }
                }

                temp = BaseProp.GetPropFormatStr(EEquipProp.IgnoreMagicAttack);
                if (string.IsNullOrEmpty(temp) == false)
                {
                    if (EquipType == EEquipType.ET_REDMARK)
                    {
                        descs.Add(TR.Value("tip_growth_effect", StrengthenLevel, temp));
                    }
                    else if (EquipType == EEquipType.ET_COMMON)
                    {
                        descs.Add(TR.Value("tip_strengthen_effect", StrengthenLevel, temp));
                    }
                }

                temp = BaseProp.GetPropFormatStr(EEquipProp.IgnoreMagicAttackRate);
                if (string.IsNullOrEmpty(temp) == false)
                {
                    if (EquipType == EEquipType.ET_REDMARK)
                    {
                        descs.Add(TR.Value("tip_growth_effect", StrengthenLevel, temp));
                    }
                    else if (EquipType == EEquipType.ET_COMMON)
                    {
                        descs.Add(TR.Value("tip_strengthen_effect", StrengthenLevel, temp));
                    }
                }

                //固定攻击
                temp = BaseProp.GetPropFormatStr(EEquipProp.IngoreIndependence);
                if (string.IsNullOrEmpty(temp) == false)
                {
                    if (EquipType == EEquipType.ET_REDMARK)
                    {
                        descs.Add(TR.Value("tip_growth_effect", StrengthenLevel, temp));
                    }
                    else if (EquipType == EEquipType.ET_COMMON)
                    {
                        descs.Add(TR.Value("tip_strengthen_effect", StrengthenLevel, temp));
                    }
                }

                temp = BaseProp.GetPropFormatStr(EEquipProp.IgnorePhysicsDefense);
                if (string.IsNullOrEmpty(temp) == false)
                {
                    if (EquipType == EEquipType.ET_REDMARK)
                    {
                        descs.Add(TR.Value("tip_growth_effect", StrengthenLevel, temp));
                    }
                    else if (EquipType == EEquipType.ET_COMMON)
                    {
                        descs.Add(TR.Value("tip_strengthen_effect", StrengthenLevel, temp));
                    }
                }

                temp = BaseProp.GetPropFormatStr(EEquipProp.IgnorePhysicsDefenseRate);
                if (string.IsNullOrEmpty(temp) == false)
                {
                    if (EquipType == EEquipType.ET_REDMARK)
                    {
                        descs.Add(TR.Value("tip_growth_effect", StrengthenLevel, temp));
                    }
                    else if (EquipType == EEquipType.ET_COMMON)
                    {
                        descs.Add(TR.Value("tip_strengthen_effect", StrengthenLevel, temp));
                    }
                }

                temp = BaseProp.GetPropFormatStr(EEquipProp.IgnoreMagicDefense);
                if (string.IsNullOrEmpty(temp) == false)
                {
                    if (EquipType == EEquipType.ET_REDMARK)
                    {
                        descs.Add(TR.Value("tip_growth_effect", StrengthenLevel, temp));
                    }
                    else if (EquipType == EEquipType.ET_COMMON)
                    {
                        descs.Add(TR.Value("tip_strengthen_effect", StrengthenLevel, temp));
                    }
                }

                temp = BaseProp.GetPropFormatStr(EEquipProp.IgnoreMagicDefenseRate);
                if (string.IsNullOrEmpty(temp) == false)
                {
                    if (EquipType == EEquipType.ET_REDMARK)
                    {
                        descs.Add(TR.Value("tip_growth_effect", StrengthenLevel, temp));
                    }
                    else if (EquipType == EEquipType.ET_COMMON)
                    {
                        descs.Add(TR.Value("tip_strengthen_effect", StrengthenLevel, temp));
                    }
                }

                // 辅助装备可以对基础属性进行强化
                if(IsAssistEquip() && StrengthenLevel > 0)
                {
                    temp = GetBaseAttrStrengthenDesc(EEquipProp.Strenth);
                    if (string.IsNullOrEmpty(temp) == false)
                    {
                        descs.Add(temp);
                    }

                    temp = GetBaseAttrStrengthenDesc(EEquipProp.Intellect);
                    if (string.IsNullOrEmpty(temp) == false)
                    {
                        descs.Add(temp);
                    }

                    temp = GetBaseAttrStrengthenDesc(EEquipProp.Spirit);
                    if (string.IsNullOrEmpty(temp) == false)
                    {
                        descs.Add(temp);
                    }

                    temp = GetBaseAttrStrengthenDesc(EEquipProp.Stamina);
                    if (string.IsNullOrEmpty(temp) == false)
                    {
                        descs.Add(temp);
                    }
                }
            }

            return descs;
        }

        //新的要求，四位属性和抗魔值放在一块
        public List<string> GetFourAttrAndResistMagicDescs()
        {
            var descs = GetFourAttrDescs();

            var resistMagicDescs = GetResistMagciDescs();
            if (string.IsNullOrEmpty(resistMagicDescs) == false)
            {
                descs.Add(resistMagicDescs);
            }

            return descs;
        }

        public string GetResistMagciDescs()
        {
            int resistMagicValue = ItemDataManager.GetInstance().GetItemResistMagicValue(TableID);
            if (resistMagicValue <= 0)
                return null;

            return TR.Value("resist_magic_title") + "+" + resistMagicValue;
        }

        /// <summary>
        /// 四维属性（力量，智力，精神，体力）
        /// </summary>
        ///       
        // 服务器发送的四维属性是加强后的值，tips上要显示加强前的值
        // 另外还有个规则 服务器发送的值是是真实的值的1000倍
        public List<string> GetFourAttrDescs()
        {
            List<string> descs = new List<string>();

            int StrenthValue = 0;
            int IntellectValue = 0;
            int SpiritValue = 0;
            int StaminaValue = 0;
            if (IsAssistEquip() && strengthenLevel > 0)
            {
                StrenthValue = BaseProp.props[(int)EEquipProp.Strenth];
                IntellectValue = BaseProp.props[(int)EEquipProp.Intellect];
                SpiritValue = BaseProp.props[(int)EEquipProp.Spirit];
                StaminaValue = BaseProp.props[(int)EEquipProp.Stamina];
                int addUp = GetBaseFourAttrStrengthenAddUpValue(strengthenLevel);                
                BaseProp.props[(int)EEquipProp.Strenth] -= addUp * EquipPropRate.propRates[(int)EEquipProp.Strenth];
                BaseProp.props[(int)EEquipProp.Intellect] -= addUp * EquipPropRate.propRates[(int)EEquipProp.Intellect];
                BaseProp.props[(int)EEquipProp.Spirit] -= addUp * EquipPropRate.propRates[(int)EEquipProp.Spirit];
                BaseProp.props[(int)EEquipProp.Stamina] -= addUp * EquipPropRate.propRates[(int)EEquipProp.Stamina];
            }
            string temp;
            if(BaseProp.props[(int)EEquipProp.Strenth] > 0)
            {
                temp = BaseProp.GetPropFormatStr(EEquipProp.Strenth);
                if (string.IsNullOrEmpty(temp) == false)
                {
                descs.Add(temp);
                }
            }
            if(BaseProp.props[(int)EEquipProp.Intellect] > 0)
            {
                temp = BaseProp.GetPropFormatStr(EEquipProp.Intellect);
                if (string.IsNullOrEmpty(temp) == false)
                {
                descs.Add(temp);
                }
            }

            if (BaseProp.props[(int)EEquipProp.Spirit] > 0)
            {
                temp = BaseProp.GetPropFormatStr(EEquipProp.Spirit);
                if (string.IsNullOrEmpty(temp) == false)
                {
                descs.Add(temp);
                }
            }

            if (BaseProp.props[(int)EEquipProp.Stamina] > 0)
            {
                temp = BaseProp.GetPropFormatStr(EEquipProp.Stamina);
                if (string.IsNullOrEmpty(temp) == false)
                {
                descs.Add(temp);
                }
            }
            if (IsAssistEquip() && strengthenLevel > 0)
            {
                BaseProp.props[(int)EEquipProp.Strenth] = StrenthValue;
                BaseProp.props[(int)EEquipProp.Intellect] = IntellectValue;
                BaseProp.props[(int)EEquipProp.Spirit] = SpiritValue;
                BaseProp.props[(int)EEquipProp.Stamina] = StaminaValue;
            }

            return descs;
        }

        /// <summary>
        /// 武器攻速
        /// </summary>
        public string GetWeaponAttackSpeedDesc()
        {
            string temp = "";
            if (EquipWearSlotType == EEquipWearSlotType.EquipWeapon)
            {
                // 徐鑫炜说要改成1010的 2018-01-12 04:37:45
                temp = TR.Value("tip_atk_speed_format", (1010 + BaseAttackSpeedRate) / 1000.0f);
                //if (BaseAttackSpeedRate <= -100)
                //{
                //    temp = TR.Value("tip_atk_speed_very_slow");
                //}
                //else if (BaseAttackSpeedRate < 0)
                //{
                //    temp = TR.Value("tip_atk_speed_slow");
                //}
                //else if (BaseAttackSpeedRate == 0)
                //{
                //    temp = TR.Value("tip_atk_speed_normal");
                //}
                //else if (BaseAttackSpeedRate < 100)
                //{
                //    temp = TR.Value("tip_atk_speed_quick");
                //}
                //else
                //{
                //    temp = TR.Value("tip_atk_speed_very_quick");
                //}
            }
            return temp;
        }

        /// <summary>
        /// 技能耗蓝变化，CD变化
        /// </summary>
        public List<string> GetSkillMPAndCDDescs()
        {
            List<string> descs = new List<string>();

            string temp;
            #region physic skill MP/CD 
            temp = BaseProp.GetPropFormatStr(EEquipProp.PhysicsSkillMPChange);
            if (string.IsNullOrEmpty(temp) == false)
            {
                descs.Add(temp);
            }

			temp = BaseProp.GetPropFormatStr(EEquipProp.PhysicsSkillCDChange);
			if (string.IsNullOrEmpty(temp) == false)
			{
				descs.Add(temp);
			}

            #endregion



            #region magic skill MP/CD 
            temp = BaseProp.GetPropFormatStr(EEquipProp.MagicSkillMPChange);
            if (string.IsNullOrEmpty(temp) == false)
            {
                descs.Add(temp);
            }
			temp = BaseProp.GetPropFormatStr(EEquipProp.MagicSkillCDChange);
			if (string.IsNullOrEmpty(temp) == false)
			{
				descs.Add(temp);
			}
            #endregion

            return descs;
        }

        /// <summary>
        /// 镶嵌部位
        /// </summary>
        public string GetBeadPartDesc()
        {
            if (SubType == (int)ItemTable.eSubType.Bead)
            {
                return TR.Value("tip_bead_part", BeadCardManager.GetInstance().GetCondition(TableID));
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary> 
        /// 宝珠镶嵌属性描述
        /// </summary> 
        public string GetBeadDescs()
        {
            string mDesc = "";

            if (PreciousBeadMountHole == null)
            {
                return mDesc;
            }

            for (int i = 0; i < PreciousBeadMountHole.Length; i++)
            {
                PrecBead mData = PreciousBeadMountHole[i];
                if (mData == null)
                {
                    continue;
                }

                var mBeadItemData = ItemDataManager.GetInstance().GetCommonItemTableDataByID(mData.preciousBeadId);
                if (mBeadItemData == null)
                {
                    continue;
                }

                mDesc = BeadCardManager.GetInstance().GetAttributesDesc(mData.preciousBeadId) + string.Format("（宝珠：{0}）", mBeadItemData.GetColorName());
                
                if (mData.randomBuffId > 0)
                {
                    mDesc += string.Format("\n附加属性:{0}", BeadCardManager.GetInstance().GetBeadRandomAttributesDesc(mData.randomBuffId));
                }

                break;
            }

            return mDesc;
        }

        /// <summary> 
        /// 宝珠镶嵌属性描述
        /// </summary> 
        public string GetBeadDescs1()
        {
            string mDesc = string.Empty;

            if (PreciousBeadMountHole == null)
            {
                return mDesc;
            }

            for (int i = 0; i < PreciousBeadMountHole.Length; i++)
            {
                PrecBead mData = PreciousBeadMountHole[i];
                if (mData == null)
                {
                    continue;
                }

                var mBeadItemData = ItemDataManager.GetInstance().GetCommonItemTableDataByID(mData.preciousBeadId);
                if (mBeadItemData == null)
                {
                    continue;
                }

                mDesc = BeadCardManager.GetInstance().GetAttributesDesc(mData.preciousBeadId);
                
                if (mData.randomBuffId > 0)
                {
                    mDesc += string.Format("\n附加属性:{0}", BeadCardManager.GetInstance().GetBeadRandomAttributesDesc(mData.randomBuffId));
                }

                break;
            }

            return mDesc;
        }

        /// <summary>
        /// 得到宝珠下级属性描述
        /// </summary>
        /// <returns></returns>
        public string GetBeadNextLevelArrtDescs()
        {
            string str = "";
            var mBeadTable = TableManager.GetInstance().GetTableItem<BeadTable>(TableID);
            if (mBeadTable != null)
            {
                if (mBeadTable.NextLevPrecbeadID != "")
                {
                    int mNextLevelBeadID = 0;
                    if (int.TryParse(mBeadTable.NextLevPrecbeadID, out mNextLevelBeadID))
                    {
                        if (mNextLevelBeadID > 0)
                        {
                            str = "下一级效果:" + BeadCardManager.GetInstance().GetAttributesDesc(mNextLevelBeadID);
                        }
                    }
                }
            }

            return str;
        }

        /// <summary>
        /// 得到宝珠附加属性描述
        /// </summary>
        /// <returns></returns>
        public string GetBeadAppendArrtDesce()
        {
            string str = "";
            if (BeadAdditiveAttributeBuffID > 0)
            {
                str = string.Format("附加属性:{0}", BeadCardManager.GetInstance().GetBeadRandomAttributesDesc(BeadAdditiveAttributeBuffID));
            }
            return str;
        }

        public string GetEnchantmentCardUpgradeDesce()
        {
            string str = string.Empty;
            var mMagicCardTable = TableManager.GetInstance().GetTableItem<MagicCardTable>(TableID);
            if (mMagicCardTable != null)
            {
                if (mMagicCardTable.MaxLevel > 0)
                {
                    str = string.Format("<color={0}>(Lv.{1}/{2})</color>", TR.Value("tip_color_normal_noparm"), mPrecEnchantmentCard.iEnchantmentCardLevel, mMagicCardTable.MaxLevel);
                }
                else
                {
                    str = string.Format("<color={0}>{1}</color>", TR.Value("Bead_red_color"), TR.Value("Beasd_DoNot_Upgrade"));
                }
            }

            return str;
        }

        public string GetBeadUpgradeDesce()
        {
            int beadMaxLevel = 0;
            var itemTable = TableManager.GetInstance().GetTableItem<ItemTable>(TableID);
            if (itemTable != null)
            {
                beadMaxLevel = itemTable.BeadLevel;
            }
            string str = "";
            var mBeadTable = TableManager.GetInstance().GetTableItem<BeadTable>(TableID);
            if (mBeadTable != null)
            {
                if (mBeadTable.Level == 1 && mBeadTable.NextLevPrecbeadID == "")
                {
                    str = string.Format("<color={0}>{1}</color>", TR.Value("Bead_red_color"), TR.Value("Bead_DoNot_Upgrade"));
                }
                else
                {
                    str = string.Format("<color={0}>升级({1}/{2})</color>", TR.Value("Bead_Orange_color"), (mBeadTable.Level - 1).ToString(),(beadMaxLevel - 1).ToString());
                }
            }

            return str;
        }

        /// <summary>
        /// 附魔部位
        /// </summary>
        public string GetMagicPartDesc()
        {
            if (SubType == (int)ProtoTable.ItemTable.eSubType.EnchantmentsCard)
            {
                return TR.Value("tip_magic_part", EnchantmentsCardManager.GetInstance().GetCondition(TableID));
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 附魔描述
        /// </summary>
        public string GetMagicDescs()
        {
            if (mPrecEnchantmentCard == null)
            {
                return string.Empty;
            }

            var enchantmentCardItemData = ItemDataManager.CreateItemDataFromTable(mPrecEnchantmentCard.iEnchantmentCardID);
            if (enchantmentCardItemData == null)
            {
                return string.Empty;
            }

            var magicItem = TableManager.GetInstance().GetTableItem<ProtoTable.MagicCardTable>(mPrecEnchantmentCard.iEnchantmentCardID);
            if (magicItem != null)
            {
                string magicItemName = string.Empty;
                {
                    var itemTable = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(mPrecEnchantmentCard.iEnchantmentCardID);
                    if (null != itemTable)
                    {
                        string levelDesc = string.Empty;
                        if (mPrecEnchantmentCard.iEnchantmentCardLevel > 0)
                        {
                            levelDesc = string.Format("+{0}", mPrecEnchantmentCard.iEnchantmentCardLevel);
                        }
                        magicItemName = TR.Value("tip_magic_attribute_desc", enchantmentCardItemData.GetColorName(), levelDesc,enchantmentCardItemData.GetQualityInfo().ColStr);
                    }
                }

                StringBuilder strBuilder = new StringBuilder(128);
                //strBuilder = strBuilder.Append(TR.Value("tip_magic_attr"));
                bool bFirst = true;
                List<string> ret = MagicProp.GetPropsFormatStr();
                for (int i = 0; i < ret.Count; ++i)
                {
                    if (bFirst)
                    {
                        strBuilder = strBuilder.AppendFormat("<color={1}>{0}</color>", ret[i], TR.Value("enchant_attribute_color"));
                        bFirst = false;
                    }
                    else
                    {
                        strBuilder = strBuilder.AppendFormat("\n<color={1}>{0}</color>", ret[i], TR.Value("enchant_attribute_color"));
                    }
                }

                if (magicItem.SkillAttributes.Length > 0)
                {
                    var splitStrings = magicItem.SkillAttributes.Split(new char[] { '\r', '\n' });
                    for (int i = 0; i < splitStrings.Length; ++i)
                    {
                        if (bFirst)
                        {
                            strBuilder = strBuilder.AppendFormat("<color={0}>{1}</color>", TR.Value("enchant_attribute_color"), splitStrings[i]);
                            bFirst = false;
                        }
                        else
                        {
                            strBuilder = strBuilder.AppendFormat("\n<color={0}>{1}</color>", TR.Value("enchant_attribute_color"), splitStrings[i]);
                        }
                    }
                }

                if (mPrecEnchantmentCard.iEnchantmentCardLevel > 0)
                {
                    var bufferitem = TableManager.GetInstance().GetTableItem<ProtoTable.BuffInfoTable>(magicItem.UpBuffID[mPrecEnchantmentCard.iEnchantmentCardLevel - 1]);
                    if (null != bufferitem)
                    {
                        if (bufferitem.Description.Count > 0)
                        {
                            if (bFirst)
                            {
                                strBuilder.AppendFormat("<color={0}>{1}</color>", TR.Value("enchant_attribute_color"), bufferitem.Description[0]);
                                bFirst = false;
                            }
                            else
                            {
                                strBuilder.AppendFormat("\n<color={0}>{1}</color>", TR.Value("enchant_attribute_color"), bufferitem.Description[0]);
                            }
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < magicItem.BuffID.Count; i++)
                    {
                        var bufferitem = TableManager.GetInstance().GetTableItem<ProtoTable.BuffInfoTable>(magicItem.BuffID[i]);
                        if (null != bufferitem)
                        {
                            if (bufferitem.Description.Count > 0)
                            {
                                if (bFirst)
                                {
                                    strBuilder.AppendFormat("<color={0}>{1}</color>", TR.Value("enchant_attribute_color"), bufferitem.Description[0]);
                                    bFirst = false;
                                }
                                else
                                {
                                    strBuilder.AppendFormat("\n<color={0}>{1}</color>", TR.Value("enchant_attribute_color"), bufferitem.Description[0]);
                                }
                            }
                        }
                    }
                }

                //if (strBuilder != null)
                //{
                  //  strBuilder.AppendFormat("{0}", magicItemName);
               // }

                return strBuilder.ToString();
            }
            else
            {
                return string.Empty;
            }
            
        }
		
		 /// <summary>
        /// 铭文镶嵌部位
        /// </summary>
        /// <returns></returns>
        public string InscriptionMosaicSlot()
        {
            if (SubType == (int)ProtoTable.ItemTable.eSubType.ST_INSCRIPTION)
            {
                return TR.Value("tip_inscription_part", InscriptionMosaicDataManager.GetInstance().GetInscriptionSlotDescription(TableID));
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 铭文适用职业描述
        /// </summary>
        /// <param name="inscriptionId"></param>
        /// <returns></returns>
        public string GetInscriptionApplicableToProfessionalDescription()
        {
            if (SubType == (int)ProtoTable.ItemTable.eSubType.ST_INSCRIPTION)
            {
                return InscriptionMosaicDataManager.GetInstance().GetInscriptionApplicableToProfessionalDescription(TableID);
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 得到铭文属性
        /// </summary>
        /// <returns></returns>
        public string GetInscriptionAttrDesc()
        {
            string desc = string.Empty;
            if (SubType == (int)ProtoTable.ItemTable.eSubType.ST_INSCRIPTION)
            {
                desc = InscriptionMosaicDataManager.GetInstance().GetInscriptionAttributesDesc(TableID);
                //desc += TR.Value("tip_inscription_desc", GetColorName());
            }

            return desc;
        }

        /// <summary>
        /// 随机属性描述
        /// </summary>
        public List<string> GetRandomAttrDescs()
        {
            //null判断
            if (RandamProp == null)
                return null;

            return RandamProp.GetPropsFormatStr();
        }

        /// <summary>
        /// 固有附加属性描述
        /// </summary>
        public List<string> GetAttachAttrDescs()
        {
            string temp;
            List<string> descs = new List<string>();

            for (int i = 0; i < ms_attachAttrIDs.Length; ++i)
            {
                if(ms_attachAttrIDs[i] == EEquipProp.AbormalResists)
                {
                    for(int j = 0; j < Global.ABNORMAL_COUNT; j++)
                    {
                        temp = BaseProp.GetPropFormatStr(ms_attachAttrIDs[i], j);

                        if (string.IsNullOrEmpty(temp) == false)
                        {
                            descs.Add(temp);
                        }
                    }
                }
                else if (ms_attachAttrIDs[i] == EEquipProp.Elements)
                {
                    for(int j=0; j<Global.ELEMENT_COUNT; ++j)
                    {
                        temp = BaseProp.GetPropFormatStr(ms_attachAttrIDs[i], j);
                        if (!string.IsNullOrEmpty(temp))
                            descs.Add(temp);
                    }
                }
                else
                {
                    temp = BaseProp.GetPropFormatStr(ms_attachAttrIDs[i]);
                    if (string.IsNullOrEmpty(temp) == false)
                    {
                        descs.Add(temp);
                    }
                }
            }

            return descs;
        }

        /// <summary>
        /// 得到武器攻击属性信息
        /// </summary>
        /// <returns></returns>
        public string GetWeaponAttackAttributeDescs()
        {
            string temp = "无";
            List<string> descs = new List<string>();

            for (int i = 0; i < ms_attachAttrIDs.Length; i++)
            {
                if (ms_attachAttrIDs[i] == EEquipProp.Elements)
                {
                    for (int j = 0; j < Global.ELEMENT_COUNT; ++j)
                    {
                        var str = BaseProp.GetPropFormatStr(ms_attachAttrIDs[i], j);
                        if (!string.IsNullOrEmpty(str))
                            descs.Add(str);

                        var str1 = MagicProp.GetPropFormatStr(ms_attachAttrIDs[i], j);
                        if (!string.IsNullOrEmpty(str1))
                        {
                            if (descs.Contains(str1))
                            {
                            }
                            else
                            {
                                descs.Add(str1);
                            }
                        }

                        var str2 = BeadProp.GetPropFormatStr(ms_attachAttrIDs[i], j);
                        if (!string.IsNullOrEmpty(str2))
                        {
                            if (descs.Contains(str2))
                            {
                            }
                            else
                            {
                                descs.Add(str2);
                            }
                        }

                        var str3 = IncriptionProp.GetPropFormatStr(ms_attachAttrIDs[i], j);
                        if (!string.IsNullOrEmpty(str3))
                        {
                            if (descs.Contains(str3))
                            {
                            }
                            else
                            {
                                descs.Add(str3);
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < descs.Count; i++)
            {
                string desc = descs[i].Substring(0, 3);

                if (i == 0)
                {
                    temp = desc;
                }
                else
                {
                    temp += string.Format("、{0}", desc);
                }
            }

            return temp;
        }

        public List<string> GetSinanBuffDescs()
        {
            List<string> descs = new List<string>();
            if(BaseProp.props[(int)EEquipProp.Strenth] > 0)
            {
                ProtoTable.BuffInfoTable buffInfoTable = TableManager.GetInstance().GetTableItem<ProtoTable.BuffInfoTable>(BaseProp.props[(int)EEquipProp.Strenth]);
                if (buffInfoTable != null)
                {
                    if (buffInfoTable.DescType == ProtoTable.BuffInfoTable.eDescType.Common)
                    {
                        descs.AddRange(buffInfoTable.Description);
                    }
                }
            }
            if(BaseProp.props[(int)EEquipProp.Intellect] > 0)
            {
                ProtoTable.BuffInfoTable buffInfoTable = TableManager.GetInstance().GetTableItem<ProtoTable.BuffInfoTable>(BaseProp.props[(int)EEquipProp.Intellect]);
                if (buffInfoTable != null)
                {
                    if (buffInfoTable.DescType == ProtoTable.BuffInfoTable.eDescType.Common)
                    {
                        descs.AddRange(buffInfoTable.Description);
                    }
                }
            }
            if(BaseProp.props[(int)EEquipProp.Spirit] > 0)
            {
                ProtoTable.BuffInfoTable buffInfoTable = TableManager.GetInstance().GetTableItem<ProtoTable.BuffInfoTable>(BaseProp.props[(int)EEquipProp.Spirit]);
                if (buffInfoTable != null)
                {
                    if (buffInfoTable.DescType == ProtoTable.BuffInfoTable.eDescType.Common)
                    {
                        descs.AddRange(buffInfoTable.Description);
                    }
                }
            }
            if(BaseProp.props[(int)EEquipProp.Stamina] > 0)
            {
                ProtoTable.BuffInfoTable buffInfoTable = TableManager.GetInstance().GetTableItem<ProtoTable.BuffInfoTable>(BaseProp.props[(int)EEquipProp.Stamina]);
                if (buffInfoTable != null)
                {
                    if (buffInfoTable.DescType == ProtoTable.BuffInfoTable.eDescType.Common)
                    {
                        descs.AddRange(buffInfoTable.Description);
                    }
                }
            }
            List<string> descs1 = new List<string>();
            for (int i = 0; i < descs.Count; ++i)
            {
                string temp = descs[i];
                if (string.IsNullOrEmpty(temp) == false)
                {
                    descs1.Add(TR.Value("tip_color_green", temp));
                }
            }
            return descs1;
        }

        /// <summary>
        /// 固有复杂属性描述
        /// </summary>
        public List<string> GetComplexAttrDescs()
        {
            string temp;
            List<string> descs = new List<string>();

            #region effect
            if (string.IsNullOrEmpty(EffectDescription) == false)
            {
                descs.Add(TR.Value("tip_color_gray1", EffectDescription));
            }
            #endregion

            #region buffSkill
            {
                List<EquipProp.BuffSkillInfo> buffSkillInfos = BaseProp.GetBuffSkillInfos();
                if (buffSkillInfos != null)
                {
                    for (int i = 0; i < buffSkillInfos.Count; ++i)
                    {
                        string strSkillDesc = string.Empty;
                        EquipProp.BuffSkillInfo buffSkillInfo = buffSkillInfos[i];
                        bool isJobMatch = PlayerBaseData.GetInstance().ActiveJobTableIDs.Contains(buffSkillInfo.jobID);
                        //descs.Add(TR.Value(isJobMatch ? "color_orange" : "color_grey", string.Format("[{0}]", buffSkillInfo.jobName)));
                        strSkillDesc += TR.Value(isJobMatch ? "color_orange" : "color_grey", string.Format("[{0}]", buffSkillInfo.jobName));
                        for (int j = 0; j < buffSkillInfo.skillDescs.Count; ++j)
                        {
                            temp = buffSkillInfo.skillDescs[j];
                            if (string.IsNullOrEmpty(temp) == false)
                            {
                                //descs.Add(TR.Value(isJobMatch ? "tip_color_green" : "color_grey", temp));
                                strSkillDesc += "\n";
                                strSkillDesc += TR.Value(isJobMatch ? "tip_color_green" : "color_grey", temp);
                            }
                        }
                        if (strSkillDesc.Length > 0)
                        {
                            descs.Add(strSkillDesc);
                        }
                    }
                }
            }
            #endregion

            #region buffOther
            {
                List<string> strList = BaseProp.GetBuffCommonDescs();
                if (strList != null)
                {
                    for (int i = 0; i < strList.Count; ++i)
                    {
                        temp = strList[i];
                        if (string.IsNullOrEmpty(temp) == false)
                        {
                            descs.Add(TR.Value("tip_color_green", temp));
                        }
                    }
                }
                if (string.IsNullOrEmpty(BaseProp.attachBuffDesc) == false)
                {
                    descs.Add(TR.Value("tip_color_green", BaseProp.attachBuffDesc));
                }
            }
            #endregion

            #region mechanism
            {
                List<string> strList = BaseProp.GetMechanismDescs();
                if (strList != null)
                {
                    for (int i = 0; i < strList.Count; ++i)
                    {
                        temp = strList[i];
                        if (string.IsNullOrEmpty(temp) == false)
                        {
                            descs.Add(TR.Value("tip_color_green", temp));
                        }
                    }
                }
                if (string.IsNullOrEmpty(BaseProp.attachMechanismDesc) == false)
                {
                    descs.Add(TR.Value("tip_color_green", BaseProp.attachMechanismDesc));
                }
            }
            #endregion

            return descs;
        }

        /// <summary>
        /// 特殊属性描述
        ///
        /// buff效果
        ///
        /// 机制效果
        /// </summary>
        public List<string> GetSepcialComplexAtrrDescs()
        {
            string temp;
            List<string> descs = new List<string>();

            #region effect
            if (string.IsNullOrEmpty(EffectDescription) == false)
            {
                descs.Add(TR.Value("tip_color_gray1", EffectDescription));
            }
            #endregion

            #region buffOther
            {
                List<string> strList = BaseProp.GetBuffCommonDescs();
                if (strList != null)
                {
                    for (int i = 0; i < strList.Count; ++i)
                    {
                        temp = strList[i];
                        if (string.IsNullOrEmpty(temp) == false)
                        {
                            descs.Add(TR.Value("color_green", temp));
                        }
                    }
                }
                if (string.IsNullOrEmpty(BaseProp.attachBuffDesc) == false)
                {
                    descs.Add(TR.Value("color_green", BaseProp.attachBuffDesc));
                }
            }
            #endregion

            #region mechanism
            {
                List<string> strList = BaseProp.GetMechanismDescs();
                if (strList != null)
                {
                    for (int i = 0; i < strList.Count; ++i)
                    {
                        temp = strList[i];
                        if (string.IsNullOrEmpty(temp) == false)
                        {
                            descs.Add(TR.Value("color_green", temp));
                        }
                    }
                }
                if (string.IsNullOrEmpty(BaseProp.attachMechanismDesc) == false)
                {
                    descs.Add(TR.Value("color_green", BaseProp.attachMechanismDesc));
                }
            }
            #endregion

            return descs;
        }

        /// <summary>
        /// 得到护甲精通、惩罚的描述
        /// </summary>
        /// <returns></returns>
        public string GetMasterAttrDes()
        {
            string strTitle = "";
            EquipProp equipProp = EquipMasterDataManager.GetInstance().GetEquipMasterProp(PlayerBaseData.GetInstance().JobTableID, this);
            if (equipProp != null)
            {
                if (EquipMasterDataManager.GetInstance().IsMaster(PlayerBaseData.GetInstance().JobTableID, (int)ThirdType))
                {
                    strTitle = string.Format("{0}{1}","护甲精通", TR.Value("tip_color_yellow1",
                    string.Format("[{0}]", EquipMasterDataManager.GetInstance().GetJobMasterDesc(PlayerBaseData.GetInstance().JobTableID, (int)ThirdType).title)) 
                    );
                }
                else
                {
                    var jobTable = TableManager.GetInstance().GetTableItem<JobTable>(PlayerBaseData.GetInstance().JobTableID);
                    if(jobTable == null)
                    {
                        return strTitle;
                    }

                    strTitle = string.Format("{0}{1}  {2}","护甲惩罚", TR.Value("color_black_white",
                    string.Format("[{0}]", EquipMasterDataManager.GetInstance().GetJobMasterDesc(PlayerBaseData.GetInstance().JobTableID, (int)ThirdType).title)),
                    TR.Value("tip_color_yellow1", string.Format("[{0}{1}精通]",jobTable.Name,jobTable.RecDefence)));
                }
            }

            return strTitle;
        }

        /// <summary>
        /// 某未开放功能
        /// </summary>
        public List<string> GetMasterAttrDescs(bool needHead = true)
        {
            EquipProp equipProp = EquipMasterDataManager.GetInstance().GetEquipMasterProp(PlayerBaseData.GetInstance().JobTableID, this);
            if (equipProp != null)
            {
                List<string> descs = equipProp.GetPropsFormatStr();
                if(needHead)
                {
                    if (EquipMasterDataManager.GetInstance().IsMaster(PlayerBaseData.GetInstance().JobTableID, (int)ThirdType))
                    {
                        string strTitle = TR.Value("tip_color_yellow1",
                        string.Format("[{0}]", EquipMasterDataManager.GetInstance().GetJobMasterDesc(PlayerBaseData.GetInstance().JobTableID, (int)ThirdType).title)
                        );
                        descs.Insert(0, strTitle);
                    }
                    else
                    {
                        string strTitle = TR.Value("color_black_white",
                        string.Format("[{0}]", EquipMasterDataManager.GetInstance().GetJobMasterDesc(PlayerBaseData.GetInstance().JobTableID, (int)ThirdType).title)
                        );
                        descs.Insert(0, strTitle);
                    }
                }
                return descs;
            }

            return new List<string>();
        }

        /// <summary>
        /// 装备描述
        /// </summary>
        public string GetDescription()
        {
            if (string.IsNullOrEmpty(Description))
            {
                return "";
            }
            else
            {
                return TR.Value("tip_color_gray3", Description);
            }
        }

        /// <summary>
        /// 装备来源描述
        /// </summary>
        public string GetSourceDescription()
        {
            if (string.IsNullOrEmpty(SourceDescription))
            {
                return "";
            }
            else
            {
                return TR.Value("tip_color_gray3", SourceDescription);
            }
        }

        /// <summary>
        /// 设置强化属性描述
        /// </summary>
        /// <param name="itemData"></param>
        /// <returns></returns>
        public ItemData SetStrengthenAttr(ItemData itemData)
        {
            var itemStrengthenAttr = ItemStrengthAttribute.Create(itemData.TableID);
            itemStrengthenAttr.SetStrength(itemData.StrengthenLevel);
            var data = itemStrengthenAttr.GetItemData();

            itemData.BaseProp.props[(int)EEquipProp.IgnorePhysicsAttack] = data.BaseProp.props[(int)EEquipProp.IgnorePhysicsAttack];
            itemData.BaseProp.props[(int)EEquipProp.IgnorePhysicsAttackRate] = data.BaseProp.props[(int)EEquipProp.IgnorePhysicsAttackRate];
            itemData.BaseProp.props[(int)EEquipProp.IgnoreMagicAttack] = data.BaseProp.props[(int)EEquipProp.IgnoreMagicAttack];
            itemData.BaseProp.props[(int)EEquipProp.IgnoreMagicAttackRate] = data.BaseProp.props[(int)EEquipProp.IgnoreMagicAttackRate];
            itemData.BaseProp.props[(int)EEquipProp.IngoreIndependence] = data.BaseProp.props[(int)EEquipProp.IngoreIndependence];
            itemData.BaseProp.props[(int)EEquipProp.IgnorePhysicsDefense] = data.BaseProp.props[(int)EEquipProp.IgnorePhysicsDefense];
            itemData.BaseProp.props[(int)EEquipProp.IgnorePhysicsDefenseRate] = data.BaseProp.props[(int)EEquipProp.IgnorePhysicsDefenseRate];
            itemData.BaseProp.props[(int)EEquipProp.IgnoreMagicDefense] = data.BaseProp.props[(int)EEquipProp.IgnoreMagicDefense];
            itemData.BaseProp.props[(int)EEquipProp.IgnoreMagicDefenseRate] = data.BaseProp.props[(int)EEquipProp.IgnoreMagicDefenseRate];

            return itemData;
        }
#endregion

        /// <summary>
        /// 检查装备是否可放入账号仓库，针对铭文,true表示能放入，false表示不能放入
        /// </summary>
        /// <returns></returns>
        public bool CheckEquipmentIsCanPutAccountWarehouse()
        {
            if (mInscriptionHoles == null)
            {
                return true;
            }

            bool find = false;

            for (int i = 0; i < mInscriptionHoles.Count; i++)
            {
                var inscriptionHoleData = mInscriptionHoles[i];
                if (inscriptionHoleData == null)
                {
                    continue;
                }

                if (inscriptionHoleData.InscriptionId > 0)
                {
                    find = true;
                    break;
                }
            }

            if (find == true)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 检查装备是否镶嵌了铭文
        /// </summary>
        /// <returns></returns>
        public bool CheckEquipIsMosaicInscription()
        {
            if (InscriptionHoles == null)
            {
                return false;
            }

            bool isMosaic = false;

            for (int i = 0; i < InscriptionHoles.Count; i++)
            {
                var inscriptionHoleData = InscriptionHoles[i];
                if (inscriptionHoleData == null)
                {
                    continue;
                }

                if (inscriptionHoleData.IsOpenHole == false)
                {
                    continue;
                }

                if (inscriptionHoleData.InscriptionId > 0)
                {
                    isMosaic = true;
                    break;
                }
            }

            return isMosaic;
        }

        #region BaseMainProp

        //得到基础属性的描述
        public List<string> GetBaseMainPropDescs()
        {

            var baseMainProp = BaseProp;
            if (baseMainProp == null)
                return null;
            List<string> descs = new List<string>();

            EEquipProp[] fourProps =
            {
                EEquipProp.PhysicsAttack,
                EEquipProp.MagicAttack,
                EEquipProp.PhysicsDefense,
                EEquipProp.MagicDefense,
                EEquipProp.Independence
            };

            int value = 0;
            var systemValueTable = TableManager.GetInstance().GetTableItem<SystemValueTable>(
                (int)SystemValueTable.eType3.SVT_Independent);

            if (systemValueTable != null)
            {
                value = systemValueTable.Value;
            }

            string temp;
            for (int i = 0; i < fourProps.Length; ++i)
            {
                temp = baseMainProp.GetPropFormatStr(fourProps[i]);
                if (string.IsNullOrEmpty(temp) == false)
                {
                    string desc = string.Empty;
                    if (fourProps[i] == EEquipProp.Independence)
                    {
                        desc = value > 0 ? string.Empty : TR.Value("tip_independence_pvpdesc");
                    }
                    
                    temp += desc;
                    descs.Add(temp);
                }
            }

            return descs;
        }

        #endregion

        /// <summary>
        /// 检查装备是否有镶嵌效果
        /// </summary>
        /// <returns></returns>
        public bool CheckEquipmentIsMosaicEffect()
        {
            if (PreciousBeadMountHole != null)
            {
                for (int i = 0; i < PreciousBeadMountHole.Length; i++)
                {
                    PrecBead mData = PreciousBeadMountHole[i];
                    if (mData == null)
                    {
                        continue;
                    }

                    var mBeadItemData = ItemDataManager.GetInstance().GetCommonItemTableDataByID(mData.preciousBeadId);
                    if (mBeadItemData == null)
                    {
                        continue;
                    }

                    return true;
                }
            }

            if(mPrecEnchantmentCard != null)
            {
                var enchantmentCardItemData = ItemDataManager.CreateItemDataFromTable(mPrecEnchantmentCard.iEnchantmentCardID);
                if(enchantmentCardItemData != null)
                {
                    return true;
                }
            }

            if(InscriptionHoles != null)
            {
                for (int i = 0; i < InscriptionHoles.Count; i++)
                {
                    var inscriptionHoleData = InscriptionHoles[i];
                    if (inscriptionHoleData == null)
                    {
                        continue;
                    }

                    if (inscriptionHoleData.IsOpenHole == false)
                    {
                        continue;
                    }

                    return true;
                }
            }

            return false;
        }

        public bool CheckEquipmentIsMosiacBead()
        {
            if (PreciousBeadMountHole != null)
            {
                for (int i = 0; i < PreciousBeadMountHole.Length; i++)
                {
                    PrecBead mData = PreciousBeadMountHole[i];
                    if (mData == null)
                    {
                        continue;
                    }

                    var mBeadItemData = ItemDataManager.GetInstance().GetCommonItemTableDataByID(mData.preciousBeadId);
                    if (mBeadItemData == null)
                    {
                        continue;
                    }

                    return true;
                }
            }

            return false;
        }

        public bool CheckEquipmentIsMosiacEnchantmentCard()
        {
            if (mPrecEnchantmentCard != null)
            {
                var enchantmentCardItemData = ItemDataManager.CreateItemDataFromTable(mPrecEnchantmentCard.iEnchantmentCardID);
                if (enchantmentCardItemData != null)
                {
                    return true;
                }
            }

            return false;
        }

        public bool CheckEquipmentIsMosiacInscription()
        {
            if (InscriptionHoles != null)
            {
                for (int i = 0; i < InscriptionHoles.Count; i++)
                {
                    var inscriptionHoleData = InscriptionHoles[i];
                    if (inscriptionHoleData == null)
                    {
                        continue;
                    }

                    if (inscriptionHoleData.IsOpenHole == false)
                    {
                        continue;
                    }

                    return true;
                }
            }

            return false;
        }
    }
}


