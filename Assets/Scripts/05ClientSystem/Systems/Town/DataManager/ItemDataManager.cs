using System;
using System.Collections;
using System.Collections.Generic;
using Network;
using Protocol;
using System.Reflection;
using EItemQuality = ProtoTable.ItemTable.eColor;
using EItemType = ProtoTable.ItemTable.eType;
using EItemSubType = ProtoTable.ItemTable.eSubType;
using EItemBindAttr = ProtoTable.ItemTable.eOwner;
///////删除linq
using ProtoTable;
using System.Text.RegularExpressions;
using UnityEngine.Assertions;

namespace GameClient
{
    public class ItemSimpleData
    {
        public int ItemID;
        public int Count;
        public string Name; // 只用在公会仓库抽奖,其他地方不用
        public int level;//只用在同套装备转换和跨套装备转换，其他地方不用

        public ItemSimpleData()
        {
        }

        public ItemSimpleData(int id, int count)
        {
            this.ItemID = id;
            this.Count = count;
        }
    }

    public class EPackageComparer : IEqualityComparer<EPackageType>
    {
        public bool Equals(EPackageType x, EPackageType y)
        {
            return x == y;
        }
        public int GetHashCode(EPackageType x)
        {
            return (int)x;
        }
    }

    public class EItemTypeComparer : IEqualityComparer<EItemType>
    {
        public bool Equals(EItemType x, EItemType y)
        {
            return x == y;
        }
        public int GetHashCode(EItemType x)
        {
            return (int)x;
        }
    }

    public class EItemSubComparer : IEqualityComparer<EItemSubType>
    {
        public bool Equals(EItemSubType x, EItemSubType y)
        {
            return x == y;
        }
        public int GetHashCode(EItemSubType x)
        {
            return (int)x;
        }
    }

    public class ItemDataManager : DataManager<ItemDataManager>
    {
        /// <summary>
        /// 拥有的道具，key：道具GUID，Value：道具对象
        /// </summary>
        protected Dictionary<ulong, ItemData> m_itemsDict = new Dictionary<ulong, ItemData>();

        /// <summary>
        /// 以背包类型为键值，建立的快速道具索引表，key：背包类型，Value：道具GUID列表
        /// </summary>
        protected Dictionary<EPackageType, List<ulong>> m_itemPackageTypesDict = new Dictionary<EPackageType, List<ulong>>(new EPackageComparer());

        /// <summary>
        /// 以道具主类型为键值，建立的快速道具索引表，key：道具主类型，Value：道具GUID列表
        /// </summary>
        protected Dictionary<EItemType, List<ulong>> m_itemTypesDict = new Dictionary<EItemType, List<ulong>>(new EItemTypeComparer());

        // 背包里道具个数, Key : 道具TableID, Value : 个数
        protected Dictionary<int, int> m_ItemNumDict = new Dictionary<int, int>();

        /// <summary>
        /// 以共享CD组ID为键值，建立的快速道具索引表，key：CD组ID，Value：道具GUID列表
        /// </summary>
        protected Dictionary<int, List<ulong>> m_itemCDGroupDict = new Dictionary<int, List<ulong>>();

        /// <summary>
        /// 以道具ID为键值，保存道具是否弹出二次确认界面
        /// </summary>
        protected Dictionary<int, bool> mItemDoubleCheckNeedShow = new Dictionary<int, bool>();

        /// <summary>
        /// CD组对象列表，用于存储CD组时间信息
        /// </summary>
        protected List<ItemCD> m_arrItemCDs = new List<ItemCD>();


        static Regex ms_equip_attr_change_reg = new Regex(@"(\-?\d*\.?\d+)(%?)");

        /// <summary>
        /// 存储常用的道具表格数据
        /// </summary>
        protected Dictionary<int, ItemData> m_commonTableItemDict = new Dictionary<int, ItemData>();

        /// <summary>
        /// 货币类型和道具ID的映射
        /// </summary>
        protected Dictionary<EItemSubType, int> m_moneyTypeIDDict = new Dictionary<EItemSubType, int>(new EItemSubComparer());

        // 拍卖行的主类型分类
        protected Dictionary<EItemType, Dictionary<EItemSubType, List<int>>> m_AuctionMainTypeIDDict = new Dictionary<EItemType, Dictionary<EItemSubType, List<int>>>(new EItemTypeComparer());

        //MagicJarFrameData m_magicJarData = null;
        
        /// <summary>
        /// 记录每个包裹，新道具数量
        /// </summary>
        protected UInt16[] m_packageHasNew = new UInt16[(int)EPackageType.Count];

        /// <summary>
        /// 时限道具更新时间间隔
        /// </summary>
        float m_fItemUpdateInterval = 0.0f;
        /// <summary>
        /// 时限道具列表
        /// </summary>
        List<ItemData> m_arrTimeLessItems = new List<ItemData>();

        List<int> m_aiAddPropertys = new List<int>();
        public List<int> AddPropertys
        {
            get { return m_aiAddPropertys; }
        }

        public const int iSwitchSecondWeaponId = 150;       //主武器变成副武器时param1的值
        public const int iSwitchWeaponId = 1;               //副武器切换成主武器时param1的值

        /// <summary>
        /// 使用虚空通行证是否弹提示
        /// </summary>
        public bool bUseVoidCrackTicketIsPlayPrompt = false;

        private bool bCalResistMagicValue = true; // 是否需要计算抗魔值并发送给服务器（特定的频繁调用情形下，该计算比较费，换个时机调用）

        public delegate void OnAddNewItem(List<Item> items);
        public delegate void OnRemoveItem(ItemData data);
        public delegate void OnUpdateItem(List<Item> items);
        public OnAddNewItem onAddNewItem;
        public OnRemoveItem onRemoveItem;
        public OnUpdateItem onUpdateItem;

        private DisplayAttribute beforeDisplayAttribute = null;

        /// <summary>
        /// 通过tableID创建缺省道具对象 TODO：参见道具系统文档，待优化1
        /// </summary>
        /// <param name="tableId"></param>
        /// <param name="subQuality">品质级别</param>
        /// <param name="strengthLevel">强化等级</param>
        /// <returns></returns>
        public static ItemData CreateItemDataFromTable(int tableId, int subQuality = 100, int strengthLevel = 0)
        {
            ItemData itemData = new ItemData(tableId);
            _InitTableData(itemData);

            if (itemData.IsTableDataInited == false)
            {
                return null;
            }

            itemData.SubQuality = subQuality;
            itemData.StrengthenLevel = strengthLevel;
 
            if(itemData.Type == EItemType.FASHION)
            {
                // 注意后续的属性加成都要用“+=”,不能再直接用“=”给覆盖掉数值了,因为无法保证以后没有人会在_InitTableData()里进行一些默认的属性加成
                // 基础属性
                EquipProp BaseEquipProp = EquipProp.CreateFromTable(itemData.FashionBaseAttributeID);
                if (BaseEquipProp != null)
                {
                    itemData.BaseProp += BaseEquipProp;
                }

                // 重选属性           
                EquipProp equipProp = EquipProp.CreateFromTable(itemData.FashionAttributeID);
                if (equipProp != null)
                {
                    itemData.BaseProp += equipProp;
                }
            }
            
            #region EquipQLValueTable
            EquipQLValueTable tableData = _GetEquipQLValue(itemData);
            if (tableData != null)
            {
                float fAtkDef = tableData.AtkDef / 1000.0f;
                float fPerfectAtkDef = tableData.PerfectAtkDef / 1000.0f;
                float fFourDimensional = tableData.FourDimensional / 1000.0f;
                float fPerfectFourDimensional = tableData.PerfectFourDimensional / 1000.0f;
                float fIndependentResists = tableData.IndependentResists / 1000.0f;
                float fPerfectIndependentResists = tableData.PerfectIndependentResists / 1000.0f;
                itemData.BaseProp.props[(int)EEquipProp.PhysicsAttack] =
                    _CalculateRealProp(itemData.BaseProp.props[(int)EEquipProp.PhysicsAttack] * 1000, fAtkDef, fPerfectAtkDef, subQuality) / 1000;
                itemData.BaseProp.props[(int)EEquipProp.MagicAttack] =
                    _CalculateRealProp(itemData.BaseProp.props[(int)EEquipProp.MagicAttack] * 1000, fAtkDef, fPerfectAtkDef, subQuality) / 1000;
                itemData.BaseProp.props[(int)EEquipProp.PhysicsDefense] =
                    _CalculateRealProp(itemData.BaseProp.props[(int)EEquipProp.PhysicsDefense] * 1000, fAtkDef, fPerfectAtkDef, subQuality) / 1000;
                itemData.BaseProp.props[(int)EEquipProp.MagicDefense] =
                    _CalculateRealProp(itemData.BaseProp.props[(int)EEquipProp.MagicDefense] * 1000, fAtkDef, fPerfectAtkDef, subQuality) / 1000;
                itemData.BaseProp.props[(int)EEquipProp.Strenth] =
                    _CalculateRealProp(itemData.BaseProp.props[(int)EEquipProp.Strenth], fFourDimensional, fPerfectFourDimensional, subQuality);
                itemData.BaseProp.props[(int)EEquipProp.Intellect] =
                    _CalculateRealProp(itemData.BaseProp.props[(int)EEquipProp.Intellect], fFourDimensional, fPerfectFourDimensional, subQuality);
                itemData.BaseProp.props[(int)EEquipProp.Spirit] =
                    _CalculateRealProp(itemData.BaseProp.props[(int)EEquipProp.Spirit], fFourDimensional, fPerfectFourDimensional, subQuality);
                itemData.BaseProp.props[(int)EEquipProp.Stamina] =
                    _CalculateRealProp(itemData.BaseProp.props[(int)EEquipProp.Stamina], fFourDimensional, fPerfectFourDimensional, subQuality);
                itemData.BaseProp.props[(int)EEquipProp.Independence] =
                    _CalculateRealProp(itemData.BaseProp.props[(int)EEquipProp.Independence], fIndependentResists, fPerfectIndependentResists, subQuality);

                #region AddElement
                //增加另外三种属性的计算：属性强化，属性抗性，异常属性抗性
                //属性强化计算
                //参数：属性强化系数，完美属性强化系数
                float fStrProp = tableData.StrProp / 1000.0f;
                float fPerfectStrProp = tableData.PerfectStrProp / 1000.0f;
                for (int i = 1; i < (int)MagicElementType.MAX; i++)
                {
                    itemData.BaseProp.magicElementsAttack[i] = _CalculateRealPropDefense(itemData.BaseProp.magicElementsAttack[i],
                            fStrProp,
                            fPerfectStrProp,
                            subQuality);
                }
                //属性抗性计算
                //参数：属性抗性系数，完美属性抗性系数
                float fDefProp = tableData.DefProp / 1000.0f;
                float fPerfectDefProp = tableData.PerfectDefProp / 1000.0f;
                for (int i = 1; i < (int)MagicElementType.MAX; i++)
                {
                    itemData.BaseProp.magicElementsDefence[i] = _CalculateRealPropDefense(itemData.BaseProp.magicElementsDefence[i],
                        fDefProp,
                        fPerfectDefProp,
                        subQuality);
                }

                //异常抗性计算
                //参数：异常抗性系数，完美异常抗性系数. 0-Global.ABNORMAL_COUNT;
                float fAbnormalResists = tableData.AbnormalResists / 1000.0f;
                float fPerfectAbnormalResists = tableData.PerfectAbnormalResists / 1000.0f;
                for (int i = 0; i < Global.ABNORMAL_COUNT; i++)
                {
                    itemData.BaseProp.abnormalResists[i] = _CalculateRealPropDefense(itemData.BaseProp.abnormalResists[i],
                        fAbnormalResists,
                        fPerfectAbnormalResists,
                        subQuality);
                }

                itemData.BaseProp.props[(int)EEquipProp.AbormalResist] =
                   _CalculateRealPropDefense(itemData.BaseProp.props[(int)EEquipProp.AbormalResist], fAbnormalResists, fPerfectAbnormalResists, subQuality);
                #endregion
            }
            #endregion

            #region IgnoreAttack

            var ignorePhysicsAttack = 0;
            var ignoreMagicAttack = 0;
            CalculateIgnroeAttack(itemData, out ignorePhysicsAttack, out ignoreMagicAttack);
            itemData.BaseProp.props[(int) EEquipProp.IgnorePhysicsAttack] = ignorePhysicsAttack;
            itemData.BaseProp.props[(int) EEquipProp.IgnoreMagicAttack] = ignoreMagicAttack;

            //强化固定攻击
            var ingnoreIndependenceAttack = 0;
            CalculateIngoreIndependenceAttack(itemData, out ingnoreIndependenceAttack);
            itemData.BaseProp.props[(int)EEquipProp.IngoreIndependence] = ingnoreIndependenceAttack;
            #endregion

            itemData.RefreshRateScore();

            return itemData;
        }

        static EquipQLValueTable _GetEquipQLValue(ItemData a_itemData)
        {
            EquipQLValueTable.ePart eType = EquipQLValueTable.ePart.NONE;
            if (a_itemData.Type == EItemType.EQUIP)
            {
                if (a_itemData.SubType == (int)EItemSubType.WEAPON)
                {
                    eType = EquipQLValueTable.ePart.WEAPON;
                }
                else if (a_itemData.ThirdType == ItemTable.eThirdType.CLOTH)
                {
                    eType = EquipQLValueTable.ePart.CLOTH;
                }
                else if (a_itemData.ThirdType == ItemTable.eThirdType.HEAVY)
                {
                    eType = EquipQLValueTable.ePart.HEAVY;
                }
                else if (a_itemData.ThirdType == ItemTable.eThirdType.SKIN)
                {
                    eType = EquipQLValueTable.ePart.LEATHER;
                }
                else if (a_itemData.ThirdType == ItemTable.eThirdType.PLATE)
                {
                    eType = EquipQLValueTable.ePart.PLATE;
                }
                else if (a_itemData.ThirdType == ItemTable.eThirdType.LIGHT)
                {
                    eType = EquipQLValueTable.ePart.LIGHT;
                }
                else if (a_itemData.SubType == (int)EItemSubType.RING || 
                    a_itemData.SubType == (int)EItemSubType.NECKLASE || 
                    a_itemData.SubType == (int)EItemSubType.BRACELET)
                {
                    eType = EquipQLValueTable.ePart.JEWELRY;
                }
                else if(a_itemData.IsAssistEquip())
                {
                    eType = EquipQLValueTable.ePart.ASSIST;
                }
            }
            
            return TableManager.GetInstance().GetTableItem<EquipQLValueTable>((int)eType);
        }

        #region PropDefence
        //属性计算公式
        //光火冰暗抗性

        //如果value为0，则直接返回0.
        //基础数值
        //Int32 base = (Int32)(value* baseRatio + value* (1 - baseRatio) * fQlRate); 
        //value : 装备属性表配的基础值
        //baseRatio：Z-装备品级数值表 的strProp字段
        //fQlRate：(float) ql / 100.00f;   ql是品质值

        //完美数值
        //perfect = (Int32)(bPerfect * value * perRatio);
        //bPerfect ： 完美 1 不是完美 0
        //perRatio ： Z-装备品级数值表 的PerfectStrProp字段
        //修正perfect：
        //if (bPerfect) { perfect = (perfect == 0) ? 1 : perfect; }  
        //（完美品时候 perfect最小值为1）
        //if (perfect > 0 && value< 0) { perfect *= -1; }

        //最终值   
        // value =  base + perfect

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="baseRatio"></param>
        /// <param name="perfectRate"></param>
        /// <param name="quality"></param>
        /// <returns></returns>
        static int _CalculateRealPropDefense(float value, float baseRatio, float perfectRate, int quality)
        {
            //基础数值为0，直接返回0
            if(value <= 0.0f)
            {
                return 0;
            }

            float qualityRate = (float)quality / 100.0f;
            bool isPerfect = true;
            //表中的数据quality默认为100， isPerfect = true;
            if (quality < 100)
            {
                isPerfect = false;
            }

            //基础数值
            int baseValue = (int)(value * baseRatio + value * (1 - baseRatio) * qualityRate);

            //完美数值
            int perfectValue = (int)((isPerfect == true ? 1 : 0) * value * perfectRate);
            if(isPerfect)
            {
                perfectValue = (perfectValue == 0) ? 1 : perfectValue;
            }
            if(perfectValue > 0 && value < 0)
            {
                perfectValue = -perfectValue;
            }

            return baseValue + perfectValue;
        }
        #endregion

        private static void CalculateIngoreIndependenceAttack(ItemData itemData,out int ignoreIndependAttack)
        {
            ignoreIndependAttack = 0;

            if (itemData == null)
            {
                return;
            }

            if (itemData.SubType != (int)ItemTable.eSubType.WEAPON)
            {
                return;
            }

            //与StrengthenLevel相关，如果StrengthenLevel不大于0，则无意义
            if (itemData.StrengthenLevel <= 0)
                return;

            var equipStrModIndAtkTable = TableManager.GetInstance().GetTableItem<ProtoTable.EquipStrModIndAtkTable>(1);
            if (equipStrModIndAtkTable == null)
                return;

            var itemLevel = itemData.LevelLimit;
            var itemStrengthLevel = itemData.StrengthenLevel;
            var itemQulity = (int)itemData.Quality;

            if (equipStrModIndAtkTable.WpColorQaMod_1.Count <= 0 || itemQulity <= 0 || itemQulity > equipStrModIndAtkTable.WpColorQaMod_1.Count)
                return;
            var qAQulityRate = equipStrModIndAtkTable.WpColorQaMod_1[itemQulity - 1] / 100.0f;

            if (equipStrModIndAtkTable.WpColorQbMod_1.Count <= 0 || itemQulity <= 0 || itemQulity > equipStrModIndAtkTable.WpColorQbMod_1.Count)
                return;
            var qBQualityRate = equipStrModIndAtkTable.WpColorQbMod_1[itemQulity - 1] / 100.0f;

            if (equipStrModIndAtkTable.WpStrenthMod_1.Count <= 0 || itemStrengthLevel <= 0 || itemStrengthLevel > equipStrModIndAtkTable.WpStrenthMod_1.Count)
                return;
            var equipStrengthRate = equipStrModIndAtkTable.WpStrenthMod_1[itemStrengthLevel - 1] / 100.0f;

            if (equipStrModIndAtkTable.EquipMod.Count <= 0 || itemStrengthLevel <= 0)
                return;
            var equipModRate = equipStrModIndAtkTable.EquipMod[0] * 0.01;

            // m_DisPhyIndependence = (int)((((itemData.LevelLimit + wpClQaModIndependence) * 0.125) * wpStrModIndependence * wpClQbModIndependence * wpPhyModIndependence * 1.1));
            //if (m_DisPhyIndependence < 1) m_DisPhyIndependence = 1;

            ignoreIndependAttack = (int)(((itemLevel + qAQulityRate) * 0.125) * equipStrengthRate * qBQualityRate * equipModRate * 1.1) * 1000;
            if (ignoreIndependAttack < 1)
            {
                ignoreIndependAttack = 1;
            }
        }

        private static void CalculateIgnroeAttack(ItemData itemData,
            out int ignorePhysicsAttack,
            out int ignoreMagicAttack)
        {
            ignorePhysicsAttack = 0;
            ignoreMagicAttack = 0;

            if(itemData == null)
                return;

            //与StrengthenLevel相关，如果StrengthenLevel不大于0，则无意义
            if (itemData.StrengthenLevel <= 0)
                return;

            var equipStrModTable = TableManager.GetInstance().GetTableItem<ProtoTable.EquipStrModTable>(1);
            if(equipStrModTable == null)
                return;

            var itemLevel = itemData.LevelLimit;
            var itemStrengthLevel = itemData.StrengthenLevel;

            var itemQulity = (int)itemData.Quality;
            if (equipStrModTable.WpColorQaMod.Count <= 0 || itemQulity <= 0 || itemQulity > equipStrModTable.WpColorQaMod.Count)
                return;
            var qAQulityRate = equipStrModTable.WpColorQaMod[itemQulity - 1] / 100.0f;

            if (equipStrModTable.WpColorQbMod.Count <= 0 || itemQulity <= 0 || itemQulity > equipStrModTable.WpColorQbMod.Count) 
                return;
            var qBQualityRate = equipStrModTable.WpColorQbMod[itemQulity - 1] / 100.0f;

            if (equipStrModTable.WpStrenthMod.Count <= 0 || itemStrengthLevel <= 0 || itemStrengthLevel > equipStrModTable.WpStrenthMod.Count)
                return;
            var equipStrengthRate = equipStrModTable.WpStrenthMod[itemStrengthLevel - 1] / 100.0f;

            #region BodyRateList
            //部位系数，由ItemData中的ThirdType字段对应equipStrModTable中相应的字段
            IList<int> bodyRateList = null;
            switch (itemData.ThirdType)
            {
                case ItemTable.eThirdType.HUGESWORD:
                    bodyRateList = equipStrModTable.HugeSword;
                    break;
                case ItemTable.eThirdType.KATANA:
                    bodyRateList = equipStrModTable.Katana;
                    break;
                case ItemTable.eThirdType.SHORTSWORD:
                    bodyRateList = equipStrModTable.ShortSword;
                    break;
                case ItemTable.eThirdType.BEAMSWORD:
                    bodyRateList = equipStrModTable.BeamSword;
                    break;
                case ItemTable.eThirdType.BLUNT:
                    bodyRateList = equipStrModTable.Blunt;
                    break;
                case ItemTable.eThirdType.REVOLVER:
                    bodyRateList = equipStrModTable.Revolver;
                    break;
                case ItemTable.eThirdType.CROSSBOW:
                    bodyRateList = equipStrModTable.CrossBow;
                    break;
                case ItemTable.eThirdType.HANDCANNON:
                    bodyRateList = equipStrModTable.HandCannon;
                    break;
                case ItemTable.eThirdType.RIFLE:
                    bodyRateList = equipStrModTable.AutoRifle;
                    break;
                case ItemTable.eThirdType.PISTOL:
                    bodyRateList = equipStrModTable.AutoPistal;
                    break;
                case ItemTable.eThirdType.STAFF:
                    bodyRateList = equipStrModTable.MagicStick;
                    break;
                case ItemTable.eThirdType.WAND:
                    bodyRateList = equipStrModTable.Twig;
                    break;
                case ItemTable.eThirdType.SPEAR:
                    bodyRateList = equipStrModTable.Pike;
                    break;
                case ItemTable.eThirdType.STICK:
                    bodyRateList = equipStrModTable.Stick;
                    break;
                case ItemTable.eThirdType.BESOM:
                    bodyRateList = equipStrModTable.Besom;
                    break;
                case ItemTable.eThirdType.GLOVE:
                    bodyRateList = equipStrModTable.Glove;
                    break;
                case ItemTable.eThirdType.BIKAI:
                    bodyRateList = equipStrModTable.Bikai;
                    break;
                case ItemTable.eThirdType.CLAW:
                    bodyRateList = equipStrModTable.Claw;
                    break;
                case ItemTable.eThirdType.OFG:
                    bodyRateList = equipStrModTable.Ofg;
                    break;
                case ItemTable.eThirdType.EAST_STICK:
                    bodyRateList = equipStrModTable.East_stick;
                    break;
                case ItemTable.eThirdType.SICKLE:
                    bodyRateList = equipStrModTable.SICKLE;
                    break;
                case ItemTable.eThirdType.TOTEM:
                    bodyRateList = equipStrModTable.TOTEM;
                    break;
                case ItemTable.eThirdType.AXE:
                    bodyRateList = equipStrModTable.AXE;
                    break;
                case ItemTable.eThirdType.BEADS:
                    bodyRateList = equipStrModTable.BEADS;
                    break;
                case ItemTable.eThirdType.CROSS:
                    bodyRateList = equipStrModTable.CROSS;
                    break;
                default:
                    break;
            }
            #endregion

            //包含物理和魔法系数，分别为[0][1]
            if(bodyRateList == null || bodyRateList.Count < 2)
                return;

            //计算公式：减伤 = max（[（武器等级+品级系数A）/8]*强化系数*品级系数B*各部位系数*110%，强化Lv*1）
            var physicsAttackResult = (itemLevel + qAQulityRate) * 0.125f * equipStrengthRate * qBQualityRate *
                                        (bodyRateList[0] / 100.0f) * 1.1f;
            var finalPhysicsAttackResult = (int) (physicsAttackResult + 0.5f);  //四舍五入取整
            ignorePhysicsAttack = (finalPhysicsAttackResult > itemStrengthLevel)
                ? finalPhysicsAttackResult
                : itemStrengthLevel;

            var magicAttackResult = (itemLevel + qAQulityRate) * 0.125f * equipStrengthRate * qBQualityRate *
                                      (bodyRateList[1] / 100.0f) * 1.1f;
            var finalMagicAttackResult = (int) (magicAttackResult + 0.5f); //四舍五入取整
            ignoreMagicAttack = (finalMagicAttackResult > itemStrengthLevel)
                ? finalMagicAttackResult
                : itemStrengthLevel;

            return;
        }

        static int _CalculateRealProp(int a_nBaseValue, float a_fBaseRatio, float a_fPerRatio, int a_nQuality)
        {
            if (a_nBaseValue <= 0)
            {
                return 0;
            }

            int nValue = (int)(a_nBaseValue * a_fBaseRatio + a_nBaseValue * (1.0f - a_fBaseRatio) * (float)a_nQuality/100.0f);
            if (a_nQuality >= 100)
            {
                int nPerfectValue = (int)(a_nBaseValue * a_fPerRatio);
                if (nPerfectValue < 1000)
                {
                    nPerfectValue = 1000;
                }
                nValue += nPerfectValue;
            }
            return nValue;
        }

        public sealed override EEnterGameOrder GetOrder()
        {
            return EEnterGameOrder.ItemDataManager;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public sealed override void Initialize()
        {
            Clear();
            _BindNetMessage();

            InitData();

            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.FatigueChanged,_OnFatigueChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.OnCountValueChange, _OnCountValueChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.BuffListChanged, OnSyncResistMagicValueByBuffChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.BuffRemoved, OnSyncResistMagicValueByBuffChanged);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ContinueProcessStart, OnContinueProcessStart);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ContinueProcessFinish, OnContinueProcessFinish);
            UIEventSystem.GetInstance().RegisterEventHandler(EUIEventID.ContinueProcessReset, OnContinueProcessReset);

            PlayerBaseData.GetInstance().onLevelChanged += OnLevelChanged;
            popUpConditions.Add(EItemType.EQUIP, new PopUpCondition()
            {
                iMinPlayerLv = 10,
                iMaxPlayerLv = 45,
                checkCallBack = (itemData) => 
                {
                    return true;
                },          
            });
            popUpConditions.Add(EItemType.FUCKTITTLE, new PopUpCondition()
            {
                iMinPlayerLv = 7,
                iMaxPlayerLv = 45,
                checkCallBack = (itemData) =>
                {
                    return true;
                },
            });
            popUpConditions.Add(EItemType.FASHION, new PopUpCondition()
            {
                iMinPlayerLv = 5,
                iMaxPlayerLv = 30,
                checkCallBack = (itemData) =>
                {
                    return true;
                },
            });
            popUpConditions.Add(EItemType.MATERIAL, new PopUpCondition() 
            {
                iMinPlayerLv = 10,
                iMaxPlayerLv = 0,
                checkCallBack = (itemData) =>
                {
                    return true;
                },
            });
            popUpConditions.Add(EItemType.EXPENDABLE, new PopUpCondition()
            {
                iMinPlayerLv = 10,
                iMaxPlayerLv = 0,
                checkCallBack = (itemData) =>
                {
                    if(itemData.TableData.SubType != EItemSubType.GiftPackage && itemData.TableData.SubType != EItemSubType.PetEgg)
                    {
                        return false;
                    }
                    return true;
                },
            });
        }

        public void InitData()
        {
            _clearData();

            _InitMoneyTableData();
            _InitEquipScore();
        }

        /// <summary>
        /// 清理
        /// </summary>
        public sealed override void Clear()
        {
            _UnBindNetMessage();
            _clearData();

            PlayerBaseData.GetInstance().onLevelChanged -= OnLevelChanged;

            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.FatigueChanged, _OnFatigueChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.OnCountValueChange, _OnCountValueChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.BuffListChanged, OnSyncResistMagicValueByBuffChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.BuffRemoved, OnSyncResistMagicValueByBuffChanged);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ContinueProcessStart, OnContinueProcessStart);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ContinueProcessFinish, OnContinueProcessFinish);
            UIEventSystem.GetInstance().UnRegisterEventHandler(EUIEventID.ContinueProcessReset, OnContinueProcessReset);

            popUpConditions.Clear();
            bCalResistMagicValue = true;
            bUseVoidCrackTicketIsPlayPrompt = false;
        }

        // 只删除数据，与道具有关的网络消息不能反注册，但是收到的无关事件触发的效果要视具体业务需求屏蔽掉 by wangbo 2019.01.29
        public void ClearChijiData()
        {
            m_itemsDict.Clear();
            m_ItemNumDict.Clear();
            m_itemPackageTypesDict.Clear();
            m_itemTypesDict.Clear();
            m_akNeedPopEquips.Clear();
            m_packageHasNew = new UInt16[(int)EPackageType.Count];
            m_arrItemCDs.Clear();
            m_fItemUpdateInterval = 0.0f;
            m_arrTimeLessItems.Clear();
        }

        private void _clearData()
        {
            m_itemsDict.Clear();

            m_ItemNumDict.Clear();
            m_itemPackageTypesDict.Clear();
            m_itemTypesDict.Clear();
            m_itemCDGroupDict.Clear();
            mItemDoubleCheckNeedShow.Clear();;
            m_akNeedPopEquips.Clear();
            //m_magicJarData = null;
            m_packageHasNew = new UInt16[(int)EPackageType.Count];

            m_commonTableItemDict.Clear();
            m_moneyTypeIDDict.Clear();
            m_AuctionMainTypeIDDict.Clear();

            m_arrItemCDs.Clear();

            m_fItemUpdateInterval = 0.0f;
            m_arrTimeLessItems.Clear();

            beforeDisplayAttribute = null;
        }

        /// <summary>
        /// 更新
        /// 更新时限道具
        /// </summary>
        /// <param name="a_fTime">每一帧的时间间隔</param>
        public sealed override void Update(float a_fTime)
        {
            m_fItemUpdateInterval -= a_fTime;
            if (m_fItemUpdateInterval <= 0.0f)
            {

                m_fItemUpdateInterval = 1.0f;
            }
        }

        void _SortTimeLessItems()
        {
            m_arrTimeLessItems.Sort((var1, var2) =>
            {
                int nTime1, nTime2;
                {
                    bool bStartCountdown;
                    var1.GetTimeLeft(out nTime1, out bStartCountdown);
                }
                {
                    bool bStartCountdown;
                    var2.GetTimeLeft(out nTime2, out bStartCountdown);
                }

                if (nTime1 <= 0)
                {
                    if (nTime2 > 0)
                    {
                        return -1;
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    if (nTime2 <= 0)
                    {
                        return 1;
                    }
                    else
                    {
                        return nTime1 - nTime2;
                    }
                }
            });
        }

        void _OnSCNotifyTimeItem(MsgDATA msg)
        {
            SCNotifyTimeItem ret = new SCNotifyTimeItem();
            ret.decode(msg.bytes);

            for(int i = 0; i < ret.items.Length; ++i)
            {
                var item = ret.items[i];
                if(null != item)
                {
                    m_arrTimeLessItems.RemoveAll(x =>
                    {
                        return x.GUID == item.itemUid;
                    });
                    var itemData = GetItem(item.itemUid);
                    if (null != itemData)
                    {
                        m_arrTimeLessItems.Add(itemData);
                    }

                    if (DeadLineReminderDataManager.GetInstance() != null)
                    {
                        DeadLineReminderDataManager.GetInstance().RemoveAll(item.itemUid);
                    }

                    if (null != itemData)
                    {

                        DeadLineReminderModel model = new DeadLineReminderModel()
                        {
                            type = DeadLineReminderType.DRT_LIMITTIMEITEM,
                            itemData = itemData
                        };

                        if (DeadLineReminderDataManager.GetInstance() != null)
                        {
                            DeadLineReminderDataManager.GetInstance().Add(model);
                        }
                    }

                }
            }

            _SortTimeLessItems();

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.TimeLessItemsChanged);

            DeadLineReminderDataManager.GetInstance().Sort();
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.DeadLineReminderChanged);
        }

        public void SendDeleteTimeLessItemsNotify()
        {
            for (int i = 0; i < m_arrTimeLessItems.Count; ++i)
            {
                SceneDeleteNotifyList kSend = new SceneDeleteNotifyList();
                kSend.notify.type = (int)NotifyType.NT_TIME_ITEM;
                kSend.notify.param = m_arrTimeLessItems[i].GUID;
                NetManager.Instance().SendCommand(ServerType.GATE_SERVER, kSend);
            }
            m_arrTimeLessItems.Clear();
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.TimeLessItemsChanged);
        }

        /// <summary>
        /// 获取时限道具列表
        /// </summary>
        /// <returns></returns>
        public List<ItemData> GetTimeLessItems()
        {
            return m_arrTimeLessItems;
        }

        /// <summary>
        /// 设置CD组信息
        /// </summary>
        /// <param name="a_arrCDs"></param>
        public void SetupItemCDs(List<ItemCD> a_arrCDs)
        {
            if (a_arrCDs != null)
            {
                for (int i = 0; i < a_arrCDs.Count; ++i)
                {
                    ItemCD source = a_arrCDs[i];
                    ItemCD target = m_arrItemCDs.Find(value => { return value.groupid == source.groupid; });
                    if (target != null)
                    {
                        target.endtime = source.endtime;
                        target.maxtime = source.maxtime;
                    }
                    else
                    {
                        m_arrItemCDs.Add(source);
                    }

                    Logger.LogProcessFormat("cd group end time:{0}", source.endtime);
                    Logger.LogProcessFormat("cd group max time:{0}", source.maxtime);
                }
            }
        }

        

        public int GetItemCount(int tableID)
        {
            Dictionary<ulong, ItemData> AllItems = ItemDataManager.GetInstance().GetAllPackageItems();
            if(AllItems == null)
            {
                Logger.LogErrorFormat("Can not Get AllItems from tableID IS is {0}",tableID);
                return 0;
            }
            var allitems = AllItems.GetEnumerator();

            int Count = 0;
            while (allitems.MoveNext())
            {
                if(allitems.Current.Value.PackageType != EPackageType.Storage && allitems.Current.Value.PackageType != EPackageType.RoleStorage)
                {
                    var tableIDTemp = allitems.Current.Value.TableID;

                    if (tableIDTemp == tableID)
                    {
                        Count += allitems.Current.Value.Count;
                    }
                }
                
            }
            return Count;
        }

        public int GetItemCountInPackage(int tableID)
        {
            Dictionary<ulong, ItemData> AllItems = ItemDataManager.GetInstance().GetAllPackageItems();
            if (AllItems == null)
            {
                Logger.LogErrorFormat("Can not Get AllItems from tableID IS is {0}", tableID);
                return 0;
            }
            var allitems = AllItems.GetEnumerator();

            int Count = 0;
            while (allitems.MoveNext())
            {
                if (allitems.Current.Value.PackageType != EPackageType.Storage && 
                    allitems.Current.Value.PackageType != EPackageType.WearEquip && 
                    allitems.Current.Value.PackageType != EPackageType.WearFashion &&
                    allitems.Current.Value.PackageType != EPackageType.RoleStorage)
                {
                    var tableIDTemp = allitems.Current.Value.TableID;

                    if (tableIDTemp == tableID)
                    {
                        Count += allitems.Current.Value.Count;
                    }
                }

            }
            return Count;
        }
        public int GetEqualFashionPriority(int tempType,int subType)
        {
            Dictionary<ulong, ItemData> AllItems = ItemDataManager.GetInstance().GetAllPackageItems();
            if (AllItems == null)
            {
                return -1;
            }
            var allitems = AllItems.GetEnumerator();

            int priority = 0;
            while (allitems.MoveNext())
            {
                if (allitems.Current.Value.PackageType == EPackageType.WearFashion)
                {
                    var tableIDTemp = allitems.Current.Value.TableID;

                    var equipmentRelationTableData = TableManager.GetInstance().GetTableItem<EquipmentRelationTable>(tableIDTemp);
                    if(equipmentRelationTableData != null && (int)equipmentRelationTableData.ItemType == tempType && (int)equipmentRelationTableData.SubType == subType)
                    {
                        priority = equipmentRelationTableData.Priority;
                    }
                }
            }
            return priority;
        }
        public ulong GetItemGUIDForType(int tableID , EPackageType packageType)
        {
            Dictionary<ulong, ItemData> AllItems = ItemDataManager.GetInstance().GetAllPackageItems();
            if (AllItems == null)
            {
                Logger.LogErrorFormat("Can not Get AllItems from tableID IS is {0}", tableID);
                return 0;
            }
            var allitems = AllItems.GetEnumerator();

            ulong guid = 0;
            while (allitems.MoveNext())
            {
                if (allitems.Current.Value.PackageType == packageType)
                {
                    var tableIDTemp = allitems.Current.Value.TableID;

                    if (tableIDTemp == tableID)
                    {
                        return allitems.Current.Value.GUID;
                    }
                }

            }
            return guid;
        }

        public List<ItemData> GetItemDataListBySubType(int thisSubType,EPackageType packageType )
        {
            Dictionary<ulong, ItemData> AllItems = ItemDataManager.GetInstance().GetAllPackageItems();
            if (AllItems == null)
            {
                return null;
            }
            var allitems = AllItems.GetEnumerator();

            List<ItemData> tempList = new List<ItemData>();
            while(allitems.MoveNext())
            {
                if(allitems.Current.Value.SubType == thisSubType && allitems.Current.Value.PackageType == packageType)
                {
                    tempList.Add(allitems.Current.Value);
                }
            }
            return tempList;
        }



        /// <summary>
        /// 获取所有的道具
        /// </summary>
        /// <returns></returns>
        public Dictionary<ulong, ItemData> GetAllPackageItems()
        {
            return m_itemsDict;
        }

        public List<ulong> GetPackageItems()
        {
            if (m_itemsDict == null)
            {
                return null;
            }

            List<ulong> guids = new List<ulong>();

            var emu = m_itemsDict.GetEnumerator();

            while(emu.MoveNext())
            {
                ItemData data = emu.Current.Value as ItemData; 

                if(data == null )
                {
                    continue;    
                }
            
                if(data.PackageType == EPackageType.WearEquip || data.PackageType == EPackageType.WearFashion || data.PackageType == EPackageType.Storage || data.PackageType == EPackageType.RoleStorage)
                {
                    continue; 
                }

                guids.Add(emu.Current.Key);
            }

            return guids;
        }

        /// <summary>
        /// 根据背包类型，获取道具GUID列表，然后再通过GetItem接口，遍历道具
        /// </summary>
        /// <param name="type">背包类型</param>
        /// <returns></returns>
        public List<ulong> GetItemsByPackageType(EPackageType type)
        {
            List<ulong> items = null;
            if (m_itemPackageTypesDict != null)
            {
                m_itemPackageTypesDict.TryGetValue(type, out items);
            }

            if (items == null)
            {
                return new List<ulong>();
            }
            return items;
        }

        //对PackageType对应的ItemGuidList进行更新（主要针对角色仓库)
        public void UpdateItemGuidListByPackageType(EPackageType type, List<ulong> itemGuidList)
        {
            if (itemGuidList == null)
                itemGuidList = new List<ulong>();

            if (m_itemPackageTypesDict != null)
                m_itemPackageTypesDict[type] = itemGuidList;
        }

        /// <summary>
        /// 根据道具主类型，获取道具GUID列表，然后再通过GetItem接口，遍历道具
        /// </summary>
        /// <param name="type">道具主类型</param>
        /// <returns></returns>
        public List<ulong> GetItemsByType(EItemType type)
        {
            List<ulong> items;
            m_itemTypesDict.TryGetValue(type, out items);
            if (items == null)
            {
                return new List<ulong>();
            }
            return items;
        }

        /// <summary>
        /// 根据道具主类型，道具子类型，获取道具GUID列表，然后再通过GetItem接口，遍历道具
        /// </summary>
        /// <param name="type">道具主类型</param>
        /// <param name="subType">道具子类型</param>
        /// <returns></returns>
        public List<ulong> GetItemsByPackageSubType(EPackageType type, ProtoTable.ItemTable.eSubType subType)
        {
            var list = GetItemsByPackageType(type);

            var ret = new List<ulong>();
            ret.AddRange(list);
            ret.RemoveAll(x =>
            {
                var data = GetItem(x);
                if (null != data)
                {
                    return data.SubType != (int)subType;
                }
                return true;
            });

            return ret;
        }

        public ulong GetMainWeapon()
        {
           return GetWearEquipBySlotType(EEquipWearSlotType.EquipWeapon);
        }

        public ulong GetBackWeapon()
        {
            return GetWearEquipBySlotType(EEquipWearSlotType.SecondEquipWeapon);
        }

        public List<ulong> GetOccupationFitEquip(List<ulong> list)
        {
            var ret = new List<ulong>();
            ret.AddRange(list);
            ret.RemoveAll(x =>
            {
                var data = GetItem(x);
                if (null != data)
                {
                    if (data.EquipType == EEquipType.ET_BREATH)
                    {
                        return true;
                    }

                    return !ShowInSideWeaponBag(data);
                }
                return true;
            });

            return ret;
        }

        /// <summary>
        /// 是否应该显示在副武器背包里面
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private bool ShowInSideWeaponBag(ItemData data)
        {
            return data.IsOccupationFit() && !data.isInSidePack;
        }

        /// <summary>
        /// 根据道具主类型、道具子类型、第三类型，获取道具GUID列表，然后再通过GetItem接口，遍历道具
        /// </summary>
        /// <param name="type">道具主类型</param>
        /// <param name="subType">道具子类型</param>
        /// <param name="thirdType">道具第三类型</param>
        /// <returns></returns>
        public List<ulong> GetItemsByPackageThirdType(EPackageType type, ProtoTable.ItemTable.eSubType subType, ProtoTable.ItemTable.eThirdType thirdType)
        {
            List<ulong> wait2FilterList = GetItemsByPackageSubType(type, subType);
            wait2FilterList.RemoveAll(
            x=>
            {
                var data = GetItem(x);
                if (null != data)
                {
                    return data.ThirdType != thirdType;
                }

                return true;
            });

            return wait2FilterList;
        }

        /// <summary>
        /// 根据道具主类型，第三类型,获取道具GUID列表，然后再通过GetItem接口，遍历道具
        /// </summary>
        /// <param name="type"></param>
        /// <param name="thirdType"></param>
        /// <returns></returns>
        public List<ulong> GetItemsByPackageThirdType(EPackageType type, ProtoTable.ItemTable.eThirdType thirdType)
        {
            var list = GetItemsByPackageType(type);

            var ret = new List<ulong>();
            ret.AddRange(list);
            ret.RemoveAll(x =>
            {
                var data = GetItem(x);
                if (null != data)
                {
                    return data.ThirdType != thirdType;
                }
                return true;
            });

            return ret;
        }

        /// <summary>
        /// 根据CD组ID，获取道具GUID列表，然后再通过GetItem接口，遍历道具
        /// </summary>
        /// <param name="a_nGroupID">CD组ID</param>
        /// <returns></returns>
        public List<ulong> GetItemsByCDGroup(int a_nGroupID)
        {
            List<ulong> items;
            m_itemCDGroupDict.TryGetValue(a_nGroupID, out items);
            if (items == null)
            {
                return new List<ulong>();
            }
            return items;
        }

        /// <summary>
        /// 根据时装槽位类型，获取道具GUID
        /// </summary>
        /// <param name="eTarget"></param>
        /// <returns></returns>
        public ulong GetFashionWearEquipBySlotType(EFashionWearSlotType eTarget)
        {
            var list = GetItemsByPackageType(EPackageType.WearFashion);
            for (int i = 0; i < list.Count; ++i)
            {
                var data = GetItem(list[i]);
                if (data != null && data.FashionWearSlotType == eTarget)
                {
                    return list[i];
                }
            }
            return 0;
        }

        /// <summary>
        /// 根据装备槽位类型，获取道具GUID
        /// </summary>
        /// <param name="eTarget">装备槽位类型</param>
        /// <returns></returns>
        public ulong GetWearEquipBySlotType(EEquipWearSlotType eTarget)
        {
            var list = GetItemsByPackageType(EPackageType.WearEquip);
            for (int i = 0; i < list.Count; ++i)
            {
                var data = GetItem(list[i]);
                if (data != null && data.EquipWearSlotType == eTarget)
                {
                    return list[i];
                }
            }
            return 0;
        }

        // 根据装备槽位类型，获取身上穿戴的某个道具
        public ItemData GetWearEquipDataBySlotType(EEquipWearSlotType eTarget)
        {
            var list = GetItemsByPackageType(EPackageType.WearEquip);
            for (int i = 0; i < list.Count; ++i)
            {
                var data = GetItem(list[i]);
                if (data != null && data.EquipWearSlotType == eTarget)
                {
                    return data;
                }
            }

            return null;
        }

        /// <summary>
        /// 根据道具GUID，获取道具对象
        /// </summary>
        /// <param name="id">道具GUID</param>
        /// <returns></returns>
        public ItemData GetItem(ulong id)
        {
            ItemData item = null;
            if (m_itemsDict.TryGetValue(id, out item))
            {
                return item;
            }
            return null;
        }

        /// <summary>
        /// 获取第一个表格ID为tableID的道具对象(注意这里遍历的是所有的道具，包括背包里的，身上穿的，仓库里的)
        /// </summary>
        /// <param name="tableID">道具表格ID</param>
        /// <returns></returns>
        public ItemData GetItemByTableID(int tableID, bool bIncludeStoreHouse = true/*是否包含仓库里的*/, bool bIncludeWearedItem = true/*是否包含身上穿戴的*/)
        {
            ItemTable tableData = TableManager.GetInstance().GetTableItem<ItemTable>(tableID);
            if (tableData == null)
            {
                return null;
            }

            List<ulong> itemGUIDs = GetItemsByType(tableData.Type);

            for (int i = 0; i < itemGUIDs.Count; ++i)
            {
                ItemData data = GetItem(itemGUIDs[i]);

                if(data == null || data.TableID != tableID)
                {
                    continue;
                }

                if(!bIncludeStoreHouse && (data.PackageType == EPackageType.Storage || data.PackageType == EPackageType.RoleStorage))
                {
                    continue;
                }

                if(!bIncludeWearedItem && (data.PackageType == EPackageType.WearEquip || data.PackageType == EPackageType.WearFashion))
                {
                    continue;
                }

                return data;
            }

            return null;
        }

        /// <summary>
        /// 根据表格ID，获取通用道具对象（缺省道具对象，非玩家拥有的道具）
        /// 经常需要用到的道具，才可以调用这个接口，并且不要去修改里面的数据
        /// </summary>
        /// <param name="a_nTableID">表格ID</param>
        /// <returns></returns>
        public ItemData GetCommonItemTableDataByID(int a_nTableID)
        {
            ItemData data = null;
            if (m_commonTableItemDict.TryGetValue(a_nTableID, out data))
            {
                return data;
            }
            else
            {
                data = CreateItemDataFromTable(a_nTableID);
                if (data == null)
                {
                    //Logger.LogErrorFormat("道具表不存在ID：{0}的道具", a_nTableID);
                    return data;
                }
                m_commonTableItemDict.Add((int)data.TableID, data);
                return data;
            }
        }

        /// <summary>
        /// 根据道具子类型a_eType，获取通用道具对象（缺省道具对象，非玩家拥有的道具）
        /// 不要去修改里面的数据
        /// </summary>
        /// <param name="a_eType">道具子类型</param>
        /// <returns></returns>
        public ItemData GetMoneyTableDataByType(EItemSubType a_eType)
        {
            ItemData data = null;
            int nTableID = 0;
            if (m_moneyTypeIDDict.TryGetValue(a_eType, out nTableID))
            {
                m_commonTableItemDict.TryGetValue(nTableID, out data);
            }
            return data;
        }

        /// <summary>
        /// 根据CD组ID获取CD组信息
        /// </summary>
        /// <param name="a_nID">CD组ID</param>
        /// <returns></returns>
        /// 
        int cachedID = ~0;
        ItemCD cachedItemCD = null;
        public ItemCD GetItemCD(int a_nID)
        {
            if (null == m_arrItemCDs || 0 == m_arrItemCDs.Count)
                return null;

            if(a_nID != cachedID || null == cachedItemCD)
            {
                cachedItemCD = _FindItemCDByID(a_nID);
                cachedID = a_nID;
            }

            return cachedItemCD;
        }

        protected ItemCD _FindItemCDByID(int id)
        {
            for(int i = 0;i < m_arrItemCDs.Count;i++)
            {
                if(m_arrItemCDs[i].groupid == id)
                {
                    return m_arrItemCDs[i];
                }
            }
            return null;
        }

        public bool TryDoUnBindItemCostHint(int a_tableID,int iNeedCount, UnityEngine.Events.UnityAction OnOKCallBack = null, UnityEngine.Events.UnityAction OnCancelCallBack = null)
        {
            var bindItem = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(a_tableID);
            if(bindItem == null)
            {
                return false;
            }

            ItemTable item = null;
            if (bindItem.SubType == EItemSubType.BindGOLD)
            {
                item = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(ItemDataManager.GetInstance().GetMoneyIDByType(ProtoTable.ItemTable.eSubType.GOLD));
            }
            else if(bindItem.SubType == EItemSubType.BindPOINT)
            {
                item = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>(ItemDataManager.GetInstance().GetMoneyIDByType(ProtoTable.ItemTable.eSubType.POINT));
            }
            if(item == null)
            {
                return false;
            }

            int iTotalCount = GetOwnedItemCount(a_tableID,true);
            int iCount = GetOwnedItemCount(a_tableID, false);

            if (!(iTotalCount >= iNeedCount && //item is enough
                iTotalCount - iCount < iNeedCount))// but bind item is not enough
            {
                return false;
            }

            SystemNotifyManager.SystemNotify(7005, OnOKCallBack, OnCancelCallBack, new object[] { bindItem.Name, item.Name });
            return true;
        }

        public int GetMoneyTypeID(byte bType)
        {
            // 金币
            if (bType == 1)
            {
                return 600000001;
            }

            // 无效的货币
            Logger.LogErrorFormat("服务器下发的货币id = {0},请在GetMoneyTypeID()转换对应货币的表格ID", bType);
            return 0;
        }

        /// <summary>
        /// 获取道具数量，道具有等价道具，如绑定金币的等价道具时金币
        /// 计算绑定金币的数量时，如果a_bNeedRelevance == true，那么会同时把金币的数量计算进去
        /// </summary>
        /// <param name="a_tableID">道具表格ID</param>
        /// <param name="a_bNeedRelevance">是否计算等价道具的数量</param>
        /// <returns></returns>
        public int GetOwnedItemCount(int a_tableID, bool a_bNeedRelevance = true)
        {
            int nCount = _GetOwnedItemCount(a_tableID);

            if (a_bNeedRelevance)
            {
                EqualItemTable equalTable = TableManager.GetInstance().GetTableItem<EqualItemTable>(a_tableID);
                if (equalTable != null)
                {
                    for (int i = 0; i < equalTable.EqualItemIDs.Count; ++i)
                    {
                        nCount += _GetOwnedItemCount(equalTable.EqualItemIDs[i]);
                    }
                }
            }

            // TODO 如果出现数据溢出，需要统一改一下，改成ulong

            return nCount;
        }

        /// <summary>
        /// 根据表格ID，获取道具名称
        /// </summary>
        /// <param name="a_tableID">表格ID</param>
        /// <returns></returns>
        public string GetOwnedItemName(int a_tableID)
        {
            ItemTable tableData = TableManager.GetInstance().GetTableItem<ItemTable>(a_tableID);
            if(tableData == null)
            {
                return "";
            }

            return tableData.Name;
        }
        
        /// <summary>
        /// 根据表格ID，获取道具ICon
        /// </summary>
        /// <param name="a_tableID">表格ID</param>
        /// <returns></returns>
        public string GetOwnedItemIconPath(int a_tableID)
        {
            ItemTable tableData = TableManager.GetInstance().GetTableItem<ItemTable>(a_tableID);
            if (tableData == null)
            {
                return "";
            }

            return tableData.Icon;
        }

        public List<int> GetAuctionItemListBaseFliter(EItemType itemType, EItemSubType itemSubType)
        {
            Dictionary<EItemSubType, List<int>> SubTypeDict = null;
            if (!m_AuctionMainTypeIDDict.TryGetValue(itemType, out SubTypeDict))
            {
                return null;
            }

            List<int> itemlist = null;
            if (!SubTypeDict.TryGetValue(itemSubType, out itemlist))
            {
                return null;
            }

            return itemlist;
        }

        public Dictionary<EItemSubType, List<int>> GetAuctionItemListByItemType(EItemType itemType)
        {
            Dictionary<EItemSubType, List<int>> subTypeDict = null;

            if (m_AuctionMainTypeIDDict.TryGetValue(itemType, out subTypeDict) == false)
                return null;

            return subTypeDict;
        }

        // 物品交易类型过滤器
        public bool TradeItemTypeFliter(List<EPackageType> TypeList, EPackageType ItemType)
        {
            for(int i = 0; i < TypeList.Count; i++)
            {
                if(TypeList[i] == ItemType)
                {
                    return true;
                }
            }

            return false;
        }

        // 物品交易品质过滤器
        public bool TradeItemQualityFliter(List<ItemTable.eColor> QualityList, ItemTable.eColor ItemColor)
        {
            for (int i = 0; i < QualityList.Count; i++)
            {
                if (QualityList[i] == ItemColor)
                {
                    return true;
                }
            }

            return false;
        }

        // 物品交易状态过滤器(判断该物品是否有交易的可能性，如果需要知道该物品实际究竟可不可以交易，要用CanTrade()这个接口)
        public bool TradeItemStateFliter(ItemData itemData) 
        {
            if (itemData.BindAttr != EItemBindAttr.NOTBIND && !itemData.Packing && itemData.RePackTime > 0)
            {
                return true;
            }

            return CanTrade(itemData);
        }

        // 判断是否有可交易的可能性,这个接口不需要具体的ItemData,只需要表格数据即可判断
        public bool TradeItemStateFliter(ItemTable item)
        {
            if(item == null)
            {
                return false;
            }

            if((item.Owner == EItemBindAttr.ROLEBIND || item.Owner == EItemBindAttr.ACCBIND) && item.SealMax <= 0)
            {
                return false;
            }

            return true;
        }

        // 该物品实际是否可交易
        public bool CanTrade(ItemData itemData)
        {
            if (itemData.BindAttr == EItemBindAttr.NOTBIND)
            {
                return true;
            }
            else
            {
                if (itemData.Packing)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 获取道具数量
        /// </summary>
        /// <param name="a_nTableID">表格ID</param>
        /// <returns></returns>
        int _GetOwnedItemCount(int a_nTableID)
        {
            int nCount = 0;
            ItemTable tableData = TableManager.GetInstance().GetTableItem<ItemTable>(a_nTableID);
            if (tableData != null)
            {
                if (tableData.SubType == ProtoTable.ItemTable.eSubType.GOLD)
                {
                    return (int)PlayerBaseData.GetInstance().Gold;
                }
                else if (tableData.SubType == ProtoTable.ItemTable.eSubType.BindGOLD)
                {
                    return (int)PlayerBaseData.GetInstance().BindGold;
                }
                else if (tableData.SubType == ProtoTable.ItemTable.eSubType.POINT)
                {
                    return (int)PlayerBaseData.GetInstance().Ticket;
                }
                else if (tableData.SubType == ProtoTable.ItemTable.eSubType.BindPOINT)
                {
                    return (int)PlayerBaseData.GetInstance().BindTicket;
                }
                else if (tableData.SubType == ProtoTable.ItemTable.eSubType.WARRIOR_SOUL)
                {
                    return (int)PlayerBaseData.GetInstance().WarriorSoul;
                }
                else if (tableData.SubType == ProtoTable.ItemTable.eSubType.DUEL_COIN)
                {
                    return (int)PlayerBaseData.GetInstance().uiPkCoin;
                }
                else if (tableData.SubType == EItemSubType.GuildContri)
                {
                    return PlayerBaseData.GetInstance().guildContribution;
                }
                else if (tableData.SubType == EItemSubType.GoldJarPoint)
                {
                    return (int)PlayerBaseData.GetInstance().GoldJarScore;
                }
                else if (tableData.SubType == EItemSubType.MagicJarPoint)
                {
                    return (int)PlayerBaseData.GetInstance().MagicJarScore;
                }
                else if (tableData.SubType ==EItemSubType.ST_APPOINTMENT_COIN)
                {
                    return (int)PlayerBaseData.GetInstance().AppoinmentCoin;
                }
                else if (tableData.SubType == EItemSubType.ST_MASTER_GOODTEACH_VALUE)
                {
                    return (int)PlayerBaseData.GetInstance().GoodTeacherValue;
                }
                else if (tableData.SubType == EItemSubType.ST_WEAPON_LEASE_TICKET)
                {
                    return (int)PlayerBaseData.GetInstance().WeaponLeaseTicket;
                }
                else if(tableData.SubType == EItemSubType.ST_GOLD_REWARD_VALUE)
                {
                    return (int)AdventureTeamDataManager.GetInstance().GetAdventureTeamBountyCount();
                }
                else if(tableData.SubType == EItemSubType.ST_BLESS_CRYSTAL_VALUE)
                {
                    return (int)AdventureTeamDataManager.GetInstance().GetAdventureTeamBlessCrystalCount();
                }
                else if (tableData.SubType == EItemSubType.ST_CHI_JI_COIN)
                {
                    return CountDataManager.GetInstance().GetCount(CounterKeys.CHIJI_SCORE);
                }
                else if(tableData.SubType == EItemSubType.ST_ZJSL_WZHJJJ_COIN)
                {
                    return CountDataManager.GetInstance().GetCount(CounterKeys.ZJSL_WZHJJJ_COIN);
                }
                else if (tableData.SubType == EItemSubType.ST_ZJSL_WZHGGG_COIN)
                {
                    return CountDataManager.GetInstance().GetCount(CounterKeys.ZJSL_WZHGGG_COIN);
                }
                else if (tableData.SubType == EItemSubType.ST_HIRE_COIN)
                {
                    return (int)AccountShopDataManager.GetInstance().GetAccountSpecialItemNum(AccountCounterType.ACC_COUNTER_HIRE_COIN);
                }
                else if (tableData.SubType == EItemSubType.ST_ZENGFU_ACTIVATE ||
                         tableData.SubType == EItemSubType.ST_ZENGFU_CLEANUP ||
                         tableData.SubType == EItemSubType.ST_ZENGFU_CREATE ||
                         tableData.SubType == EItemSubType.ST_ZENGFU_PROTECT ||
                         tableData.SubType == EItemSubType.ST_STRENGTHEN_PROTECT)
                {
                    List<ulong> itemGUIDs = GetItemsByType(tableData.Type);
                    for (int i = 0; i < itemGUIDs.Count; ++i)
                    {
                        ItemData data = GetItem(itemGUIDs[i]);
                        if (data != null && data.TableID == a_nTableID && data.PackageType != EPackageType.Storage && data.PackageType != EPackageType.RoleStorage)
                        {
                            int timeLeft;
                            bool bStartCountdown;
                            data.GetLimitTimeLeft(out timeLeft, out bStartCountdown);

                            //道具过期了，过滤掉
                            if (timeLeft <= 0 && bStartCountdown)
                            {
                                continue;
                            }

                            nCount += data.Count;
                        }
                    }
                }
                else if (tableData.SubType == EItemSubType.ST_MALL_POINT)
                {
                    return (int)PlayerBaseData.GetInstance().IntergralMallTicket;
                }
                else if (tableData.SubType == EItemSubType.ST_ADVENTURE_COIN)
                {
                    return (int)PlayerBaseData.GetInstance().adventureCoin;
                }
                else
                {
                    if(m_ItemNumDict.TryGetValue(a_nTableID, out nCount))
                    {
                        return nCount;
                    }
                }
            }
            return nCount;
        }

        /// <summary>
        /// 根据子类型得到道具总数（特殊需求绑定深渊票新加了限时道具，策划要跟非限时的道具数量显示一块）
        /// </summary>
        /// <param name="type"></param>
        /// <param name="subType"></param>
        /// <returns></returns>
        public int GetItemCountBySubType(EPackageType type,EItemSubType subType)
        {
            int iCount = 0;
            List<ulong> guidList = GetItemsByPackageType(type);
            for (int i = 0; i < guidList.Count; i++)
            {
                ulong guid = guidList[i];
                ItemData itemData = GetItem(guid);
                if(itemData == null)
                {
                    continue;
                }

                if(itemData.SubType != (int)subType)
                {
                    continue;
                }

                iCount += itemData.Count;
            }

            return iCount;
        }

        /// <summary>
        /// 根据道具子类型，获取道具表格ID
        /// </summary>
        /// <param name="a_eType">道具子类型</param>
        /// <returns></returns>
        public int GetMoneyIDByType(EItemSubType a_eType)
        {
            int nTableID = 0;
            m_moneyTypeIDDict.TryGetValue(a_eType, out nTableID);
            return nTableID;
        }

        /// <summary>
        /// 特定类型的背包是否有新道具
        /// </summary>
        /// <param name="type">背包类型</param>
        /// <returns></returns>
        public bool IsPackageHasNew(EPackageType type)
        {
            if (type > EPackageType.Invalid && type < EPackageType.Count)
            {
                return m_packageHasNew[(int)type] > 0 ? true : false;
            }

            return false;
        }

        /// <summary>
        /// 特定类型的背包，是否满了，如果a_eType == EPackageType.Invalid，那么任意一个背包满了，那返回true
        /// </summary>
        /// <param name="a_eType">背包类型</param>
        /// <returns></returns>
        public bool IsPackageFull(EPackageType a_eType = EPackageType.Invalid)
        {
            if (a_eType == EPackageType.Invalid)
            {
                return GetItemsByPackageType(EPackageType.Equip).Count >= PlayerBaseData.GetInstance().PackTotalSize[(int)EPackageType.Equip] ||
                    GetItemsByPackageType(EPackageType.Material).Count >= PlayerBaseData.GetInstance().PackTotalSize[(int)EPackageType.Material] ||
                    GetItemsByPackageType(EPackageType.Consumable).Count >= PlayerBaseData.GetInstance().PackTotalSize[(int)EPackageType.Consumable] ||
                    GetItemsByPackageType(EPackageType.Task).Count >= PlayerBaseData.GetInstance().PackTotalSize[(int)EPackageType.Task] ||
                    GetItemsByPackageType(EPackageType.Fashion).Count >= PlayerBaseData.GetInstance().PackTotalSize[(int)EPackageType.Fashion] ||
                    GetItemsByPackageType(EPackageType.Title).Count >= PlayerBaseData.GetInstance().PackTotalSize[(int)EPackageType.Title] ||
                    GetItemsByPackageType(EPackageType.Bxy).Count >= PlayerBaseData.GetInstance().PackTotalSize[(int)EPackageType.Bxy] ||
                    GetItemsByPackageType(EPackageType.Sinan).Count >= PlayerBaseData.GetInstance().PackTotalSize[(int)EPackageType.Sinan];
            }
            else
            {
                return GetItemsByPackageType(a_eType).Count >= PlayerBaseData.GetInstance().PackTotalSize[(int)a_eType];
            }
        }

        /// <summary>
        /// 设置道具状态为老的
        /// </summary>
        /// <param name="a_item">道具对象</param>
        public void NotifyItemBeOld(ItemData a_item)
        {
            if (a_item != null && a_item.IsNew)
            {
                a_item.IsNew = false;

                if (a_item.PackageType > EPackageType.Invalid && a_item.PackageType < EPackageType.Count)
                {
                    m_packageHasNew[(int)a_item.PackageType] -= 1;
                    _NotifyItemNewStateChanged();
                }
            }
        }

        /// <summary>
        /// 设置道具状态为新的
        /// </summary>
        /// <param name="a_item">道具对象</param>
        public void NotifyItemBeNew(ItemData a_item)
        {
            if (a_item != null && a_item.IsNew == false)
            {
                a_item.IsNew = true;

                if (a_item.PackageType > EPackageType.Invalid && a_item.PackageType < EPackageType.Count)
                {
                    m_packageHasNew[(int)a_item.PackageType] += 1;
                    _NotifyItemNewStateChanged();
                }
            }
        }

        //背包界面，对道具排序(装备和称号特殊处理)
        public void ArrangeItemsInPackageFrame(EPackageType type)
        {
            List<ulong> items;
            m_itemPackageTypesDict.TryGetValue(type, out items);
            if (items == null)
            {
                return;
            }

            items.Sort((x, y) =>
            {
                ItemData dataX = GetItem(x);
                ItemData dataY = GetItem(y);
                if (dataX == null || dataY == null) return 0;
                if (dataX.isInSidePack && !dataY.isInSidePack)
                    return -1;
                if (!dataX.isInSidePack && dataY.isInSidePack)
                    return 1;

                //装备和称号道具排序的时候，注意是否在未启用的装备方案中
                if (type == EPackageType.Equip || type == EPackageType.Title || type == EPackageType.Bxy)
                {
                    var itemXInUnSelectedEquipPlanFlag = dataX.IsItemInUnUsedEquipPlan;
                    var itemYInUnSelectedEquipPlanFlag = dataY.IsItemInUnUsedEquipPlan;

                    if (itemXInUnSelectedEquipPlanFlag == true && itemYInUnSelectedEquipPlanFlag == false)
                        return -1;
                    if (itemXInUnSelectedEquipPlanFlag == false && itemYInUnSelectedEquipPlanFlag == true)
                        return 1;
                }

                int comp = dataY.Quality.CompareTo(dataX.Quality);
                if (comp == 0)
                {
                    comp = dataY.TableID.CompareTo(dataX.TableID);
                    if (comp == 0)
                    {
                        comp = dataY.Count.CompareTo(dataX.Count);
                        //强化等级按照从小到大进行排序
                        if (comp == 0)
                        {
                            comp = dataX.StrengthenLevel.CompareTo(dataY.StrengthenLevel);
                        }
                    }
                }
                return comp;
            });
        }

        /// <summary>
        /// 对特定背包里的道具排序
        /// </summary>
        /// <param name="type">背包类型</param>
        public void ArrangeItems(EPackageType type)
        {
            List<ulong> items;
            m_itemPackageTypesDict.TryGetValue(type, out items);
            if (items == null)
            {
                return;
            }

            items.Sort((x, y) =>
            {
                ItemData dataX = GetItem(x);
                ItemData dataY = GetItem(y);
                if (dataX == null || dataY == null) return 0;
                if (dataX.isInSidePack && !dataY.isInSidePack)
                    return -1;
                if (!dataX.isInSidePack && dataY.isInSidePack)
                    return 1;

                int comp = dataY.Quality.CompareTo(dataX.Quality);
                if (comp == 0)
                {
                    comp = dataY.TableID.CompareTo(dataX.TableID);
                    if (comp == 0)
                    {
                        comp = dataY.Count.CompareTo(dataX.Count);
                        //强化等级按照从小到大进行排序
                        if (comp == 0)
                        {
                            comp = dataX.StrengthenLevel.CompareTo(dataY.StrengthenLevel);
                        }
                    }
                }
                return comp;
            });
        }

        /// <summary>
        /// 从仓库里取出道具到背包
        /// </summary>
        /// <param name="item">道具对象</param>
        /// <param name="count">道具数量</param>
        public void TakeItem(ItemData item, int count)
        {
            if (item != null)
            {

                var itemTableData = TableManager.GetInstance().GetTableItem<ItemTable>((int)item.TableID);
                if (itemTableData == null)
                {
                    Logger.LogErrorFormat("can not find ItemTableData when id is {0}", (int)item.TableID);
                }
                if (itemTableData.GetLimitNum != 0)
                {
                    if (itemTableData.GetLimitNum < ItemDataManager.GetInstance().GetItemCount((int)item.TableID) + count)
                    {
                        string name = itemTableData.Name;

                        object[] args = new object[1];
                        args[0] = name;

                        // SystemNotifyManager.SystemNotify(1102, _AgreeJoinTeam, _RejectJoinTeam, 30.0f, args);
                        SystemNotifyManager.SystemNotify(9103);
                        return;
                    }
                }

                ScenePullStorage msg = new ScenePullStorage();
                msg.uid = item.GUID;
                msg.num = (ushort)count;
                NetManager netMgr = NetManager.Instance();
                netMgr.SendCommand(ServerType.GATE_SERVER, msg);

                WaitNetMessageManager.GetInstance().Wait<ScenePullStorageRet>(msgRet =>
                {
                    if (msgRet.code != (uint)ProtoErrorCode.SUCCESS)
                    {
                        SystemNotifyManager.SystemNotify((int)msgRet.code);
                    }
                    else
                    {
                        UIEvent uiEvent = UIEventSystem.GetInstance().GetIdleUIEvent();
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ItemTakeSuccess, item.PackageType);
                    }
                });
            }
        }

        public void SendSellItem(ulong[] guids)
        {
            SceneSellItemBatReq kSend = new SceneSellItemBatReq();
            kSend.itemUids = guids;

            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, kSend);
        }

        //[MessageHandle(SceneSellItemBatRes.MsgID)]
        void _OnRecvSceneSellItemBatRes(MsgDATA msg)
        {
            SceneSellItemBatRes recv = new SceneSellItemBatRes();
            recv.decode(msg.bytes);

            if(recv.code != 0)
            {
                SystemNotifyManager.SystemNotify((int)recv.code);
            }
            else
            {
                SystemNotifyManager.SysNotifyTextAnimation(TR.Value("bat_sell_ok"));
            }
        }

        /// <summary>
        /// 出售道具，买到商店
        /// </summary>
        /// <param name="item">道具对象</param>
        /// <param name="Count">出售的道具数量</param>
        public void SellItem(ItemData item, int Count = 1)
        {
            if (item != null)
            {
                if (item.CanSell == false)
                {
                    Logger.LogError("物品不可出售！！");
                    return;
                }

                if (Count > item.Count)
                {
                    Logger.LogErrorFormat("最多可卖{0}个!!", item.Count);
                    return;
                }

                EPackageType packageType = item.PackageType;

                SceneSellItem msg = new SceneSellItem();
                msg.uid = item.GUID;
                msg.num = (ushort)Count;
                NetManager netMgr = NetManager.Instance();
                netMgr.SendCommand(ServerType.GATE_SERVER, msg);

                WaitNetMessageManager.GetInstance().Wait<SceneSellItemRet>(msgRet =>
                {
                    if (msgRet.code != (uint)ProtoErrorCode.SUCCESS)
                    {
                        SystemNotifyManager.SystemNotify((int)msgRet.code);
                    }
                    else
                    {
                        UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ItemSellSuccess, packageType);
                    }
                });
            }
        }

        public void SwitchWeapon(ItemData item, bool a_bUseAll = false, int a_nParam1 = 0, int a_nParam2 = 0)
        {
            if (item != null)
            {
                var find = GetItem(item.GUID);
                if (find == null)
                {
                    return;
                }
                if (item.UseType == ProtoTable.ItemTable.eCanUse.CanNot)
                {
                    SystemNotifyManager.SystemNotify(1007);
                    return;
                }

                if (item.SubType == (int)ProtoTable.ItemTable.eSubType.ExperiencePill)
                {
                    if (Utility.IsPlayerLevelFull(PlayerBaseData.GetInstance().Level))
                    {
                        SystemNotifyManager.SystemNotify(1234);
                        return;
                    }
                }

                if (item.useLimitType == ItemTable.eUseLimiteType.DAYLIMITE)
                {
                    if (item.GetCurrentRemainUseTime() <= 0)
                    {
                        SystemNotifyManager.SystemNotify(1226);
                        return;
                    }
                }
                else if (item.useLimitType == ItemTable.eUseLimiteType.VIPLIMITE)
                {
                    if (item.GetCurrentRemainUseTime() <= 0)
                    {
                        SystemNotifyManager.SystemNotify(1251, () =>
                        {
                            var frame = ClientSystemManager.GetInstance().OpenFrame<VipFrame>(FrameLayer.Middle) as VipFrame;
                            frame.OpenPayTab();

                            ItemTipManager.GetInstance().CloseAll();
                        });

                        return;
                    }
                }

                ItemData currentEquipted = null;
                ItemData target = null;
                bool isChange = false;
                if (item.EquipWearSlotType == EEquipWearSlotType.EquipWeapon)
                {
                    currentEquipted = item;
                    target = GetItem(GetWearEquipBySlotType(EEquipWearSlotType.SecondEquipWeapon));
                    a_nParam1 = iSwitchSecondWeaponId;
                    isChange = true;
                }
                else if (item.EquipWearSlotType == EEquipWearSlotType.SecondEquipWeapon)
                {
                    currentEquipted = item;
                    target = GetItem(GetWearEquipBySlotType(EEquipWearSlotType.EquipWeapon));
                    a_nParam1 = iSwitchWeaponId;
                    isChange = true;
                }

                if (isChange)
                {
                    SendUseItemMsg(item, currentEquipted, target, a_bUseAll, a_nParam1, a_nParam2);
                }
            }
        }

        /// <summary>
        /// 使用道具，许多功能都统一成使用道具，服务器判断具体使用类型
        /// 具体的类型：使用消耗品；穿戴卸下装备、时装；
        /// a_nParam1，a_nParam2为附加参数，不同情况，含义也不一样。目前的含义有：
        /// 1.开礼包，选择道具时，a_nParam1，a_nParam2表示高位，组合成一个64位的值，每一位表示是否选中该索引的道具
        /// </summary>
        /// <param name="item">道具对象</param>
        /// <param name="a_bUseAll">叠加道具，是否全部使用</param>
        /// <param name="a_nParam1">附加参数1</param>
        /// <param name="a_nParam2">附加参数2</param>
        public void UseItem(ItemData item, bool a_bUseAll = false, int a_nParam1 = 0, int a_nParam2 = 0)
        {
            if (item != null)
            {
                var find = GetItem(item.GUID);
                if (find == null)
                {
                    return;
                }
                if (item.UseType == ProtoTable.ItemTable.eCanUse.CanNot)
                {
                    SystemNotifyManager.SystemNotify(1007);
                    return;
                }

                if(item.SubType == (int)ProtoTable.ItemTable.eSubType.ExperiencePill)
                {
                    if(Utility.IsPlayerLevelFull(PlayerBaseData.GetInstance().Level))
                    {
                        SystemNotifyManager.SystemNotify(1234);
                        return;
                    }
                }

                if (item.useLimitType == ItemTable.eUseLimiteType.DAYLIMITE)
                {
                    if (item.GetCurrentRemainUseTime() <= 0)
                    {
                        SystemNotifyManager.SystemNotify(1226);
                        return;
                    }
                }
                else if (item.useLimitType == ItemTable.eUseLimiteType.VIPLIMITE)
                {
                    if (item.GetCurrentRemainUseTime() <= 0)
                    {
                        SystemNotifyManager.SystemNotify(1251, () =>
                        {
                            var frame = ClientSystemManager.GetInstance().OpenFrame<VipFrame>(FrameLayer.Middle) as VipFrame;
                            frame.OpenPayTab();

                            ItemTipManager.GetInstance().CloseAll();
                        });

                        return;
                    }
                }


                EPackageType packageType = item.PackageType;

                ItemData currentEquipted = null;
                ItemData target = null;
                if ((packageType == EPackageType.Equip ||
                    packageType == EPackageType.Fashion ||
                    packageType == EPackageType.Title ||
                    packageType == EPackageType.Bxy) && item.CanEquip())
                {

                   currentEquipted = ItemDataManager.GetInstance().GetItem(ItemDataManager.GetInstance().GetWearEquipBySlotType(item.EquipWearSlotType));
                    target = item;
                    if (item.ThirdType == ItemTable.eThirdType.BEAMSWORD)
                    {
                        if (PlayerBaseData.GetInstance().JobTableID != (int)ActorOccupation.SwordSoulMan)
                        {
                            SystemNotifyManager.SysNotifyFloatingEffect(TR.Value("beamsword_learn_skill"));
                        }
                    }
                }

                if((packageType == EPackageType.WearEquip ||
                    packageType == EPackageType.WearFashion))
                {
                    currentEquipted = item;
                    target = ItemDataManager.GetInstance().GetItem(ItemDataManager.GetInstance().GetWearEquipBySlotType(item.EquipWearSlotType));
                }

                // 飞升秘药特殊判断一下
                if (item.TableData.SubType == EItemSubType.ST_FLYUPITEM)
                {
                    PlayerBaseData.GetInstance().IsFlyUpState = true;
                }

                if (!string.IsNullOrEmpty(item.DoubleCheckWindowDesc) && !mItemDoubleCheckNeedShow.ContainsKey(item.TableID))
                {
                    ItemDoubleCheckData data = new ItemDoubleCheckData();
                    data.Desc = item.DoubleCheckWindowDesc;
                    data.mCallBack = isCloseNotify =>
                    {
                        if (isCloseNotify)
                        {
                            mItemDoubleCheckNeedShow.Add(item.TableID, true);
                        }

                        SendUseItemMsg(item, currentEquipted, target, a_bUseAll, a_nParam1, a_nParam2);
                    };
                    data.itemData = item;
                    ClientSystemManager.GetInstance().OpenFrame<ItemDoubleCheckFrame>(FrameLayer.Middle, data);
                }
                else
                {
                    SendUseItemMsg(item, currentEquipted, target, a_bUseAll, a_nParam1, a_nParam2);
                }
            }
        }

        public void UseItemWithoutDoubleCheck(ItemData item, bool a_bUseAll = false, int a_nParam1 = 0, int a_nParam2 = 0)
        {
            SendUseItemMsg(item, null, null, a_bUseAll, a_nParam1, a_nParam2);
        }

        void SendUseItemMsg(ItemData item, ItemData currentEquipted, ItemData target, bool a_bUseAll = false, int a_nParam1 = 0, int a_nParam2 = 0)
        {
            SceneUseItem msg = new SceneUseItem();
            msg.uid = item.GUID;
            msg.useAll = (byte)(a_bUseAll ? 1 : 0);
            msg.param1 = (uint)a_nParam1;
            msg.param2 = (uint)a_nParam2;
            NetManager netMgr = NetManager.Instance();
            netMgr.SendCommand(ServerType.GATE_SERVER, msg);
            Logger.LogProcessFormat("send use item, time:{0}", TimeManager.GetInstance().GetServerDoubleTime());

            if (item.Type == EItemType.EXPENDABLE && item.SubType == (int)EItemSubType.Jar)
            {
                _BindUseJarMsgHandle(item);
            }
            else
            {
                WaitNetMessageManager.GetInstance().Wait<SceneUseItemRet>(msgRet =>
                {
                    if (msgRet.code != (uint)ProtoErrorCode.SUCCESS)
                    {
                        if (item.TableData.SubType == EItemSubType.ST_FLYUPITEM)
                        {
                            PlayerBaseData.GetInstance().IsFlyUpState = false;
                        }
                        if(item.TableData.SubType==EItemSubType.ST_CONSUME_JAR_GIFT)//这个类型的道具提示，做特殊处理
                        {
                            var table = TableManager.GetInstance().GetTableItem<CommonTipsDesc>((int)msgRet.code);
                            ItemTable itemTable = TableManager.GetInstance().GetTableItem<ItemTable>(item.TableID);
                            if(table != null&&itemTable!=null&& itemTable.jarGiftConsumeItem.Count>=2)
                            {
                                int comsumItemID= itemTable.jarGiftConsumeItem[0];
                                int comsumItemNum = itemTable.jarGiftConsumeItem[1];

                                ItemTable comsumeItem = TableManager.GetInstance().GetTableItem<ItemTable>(comsumItemID);
                                if(comsumeItem!=null)
                                {
                                    string des = string.Format(table.Descs, comsumeItem.Name, comsumItemNum, itemTable.Name, comsumeItem.Name);
                                    SystemNotifyManager.SysNotifyTextAnimation(des);
                                }
                              
                            }
                        }
                        else
                        {
                            SystemNotifyManager.SystemNotify((int)msgRet.code);
                        }

                       

                      
                    }
                    else
                    {
                        Logger.Log("use item success!!!!");
                        _NotifyItemUseSuccess(item);

                        //if (target != null)
                        {
                            _OnPackageItemEquiped(currentEquipted, target);
                        }
                    }
                });
            }
        }

        /// <summary>
        /// 分解道具，可以分解单个，也可以分解多个
        /// </summary>
        /// <param name="a_arrGuilds">要分解的道具GUID列表</param>
        public void SendDecomposeItem(ulong[] a_arrGuilds,bool isDecomposeFashion = false)
        {
            if (a_arrGuilds == null)
            {
                return;
            }

            SceneEquipDecompose msg = new SceneEquipDecompose();
            msg.uids = a_arrGuilds;
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, msg);

            WaitNetMessageManager.GetInstance().Wait<SceneEquipDecomposeRet>(msgRet =>
            {
                if (msgRet.code != (uint)ProtoErrorCode.SUCCESS)
                {
                    SystemNotifyManager.SystemNotify((int)msgRet.code);
                }
                else
                {
                    DecomposeResultData data = new DecomposeResultData();
                    data.bSingle = a_arrGuilds.Length == 1;
                    data.arrItems = new List<ItemData>();
                    data.bIsDecomposeFashion = isDecomposeFashion;
                    for (int i = 0; i < msgRet.getItems.Length; ++i)
                    {
                        ItemData item = ItemDataManager.CreateItemDataFromTable((int)msgRet.getItems[i].id);
                        if (item != null)
                        {
                            item.Count = (int)msgRet.getItems[i].num;
                            data.arrItems.Add(item);
                        }
                    }

                    //分解的是时装
                    if (isDecomposeFashion == true)
                    {
                        data.arrItems.Sort((x, y) => 
                        {
                            if (x.Quality != y.Quality)
                            {
                                return y.Quality - x.Quality;
                            }

                            if (x.ThirdType != y.ThirdType)
                            {
                                return x.ThirdType - y.ThirdType;
                            }

                            return x.TableID - y.TableID;
                        });
                    }

                    ClientSystemManager.GetInstance().OpenFrame<DecomposeResultFrame>(FrameLayer.Middle, data);
                    ClientSystemManager.GetInstance().CloseFrame<FashionEquipDecomposeFrame>();
                }
            });
        }

        /// <summary>
        /// 道具续费，a_nTime<=0，表示永久续费，大于0表示，续费的天数
        /// </summary>
        /// <param name="a_item">道具对象</param>
        /// <param name="a_nTime">要续费的天数</param>
        public void RenewalItem(ItemData a_item, uint a_nTime)
        {
            SceneRenewTimeItemReq msg = new SceneRenewTimeItemReq();
            msg.itemUid = a_item.GUID;
            msg.duration = a_nTime;
            NetManager.Instance().SendCommand(ServerType.GATE_SERVER, msg);

            WaitNetMessageManager.GetInstance().Wait<SceneRenewTimeItemRes>(msgRet =>
            {
                if (msgRet.code != (uint)ProtoErrorCode.SUCCESS)
                {
                    SystemNotifyManager.SystemNotify((int)msgRet.code);
                }
                else
                {
                    if (a_item.CanEquip() && a_item.IsEquiped() == false)
                    {
                        SystemNotifyManager.SysNotifyMsgBoxOkCancel(
                            TR.Value("item_renewal_success_equip_ask", a_item.GetColorName()), 
                            ()=> { UseItem(a_item); }
                            );
                    }
                    else
                    {
                        SystemNotifyManager.SysNotifyTextAnimation(TR.Value("item_renewal_success"));
                    }

                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ItemRenewalSuccess);
                }
            });
        }

        public void OnLevelChanged(int iPreLv)
        {
            _OnLevelUpAddNewPopEquipments(iPreLv);
        }

        public void OnJobChanged()
        {
            m_akNeedPopEquips.Clear();
            _OnChangeJobAddNewPopEquipments();
        }

        /// <summary>
        /// 以网络消息数据，创建道具对象
        /// </summary>
        /// <param name="msgItemData"></param>
        /// <returns></returns>
        public ItemData CreateItemDataFromNet(Item msgItemData)
        {
            ItemData itemData = new ItemData((int)msgItemData.dataid);
            //itemData.IsNew = Convert.ToBoolean(msgItemData.param1);
            //itemData.TableID = (int)msgItemData.dataid;
            _InitTableData(itemData);
            if (itemData.IsTableDataInited == false)
            {
                return null;
            }
            
            // 后面如果还有客户端自己算的属性加成的话，一定要写在服务器数据赋值后面加.因为服务器赋值用的是“=”号
            if ((EPackageType)msgItemData.pack == EPackageType.WearEquip)
            {
                //特殊处理，因为臂章类型是99，穿戴臂章服务器返回过来的格子数是11，跟类型不匹配，暂时写死手动加上88
                if (itemData.SubType == (int)ItemTable.eSubType.ST_BXY_EQUIP)
                {
                    itemData.EquipWearSlotType = EEquipWearSlotType.Equipcloak;
                }
                else
                {
                    if (msgItemData.grid >= 11)
                    {
                        itemData.EquipWearSlotType = (EEquipWearSlotType)(msgItemData.grid + 88);
                    }
                    else
                    {
                        itemData.EquipWearSlotType = (EEquipWearSlotType)(msgItemData.grid + 1);
                    }
                }
            }
            itemData.GUID = msgItemData.uid;
            itemData.Count = msgItemData.num;
            itemData.ShowCount = msgItemData.num;
            itemData.Packing = (msgItemData.sealstate == 1);
            itemData.iPackedTimes = (int)msgItemData.sealcount;
            itemData.RePackTime = itemData.iMaxPackTime - itemData.iPackedTimes;
            itemData.PackageType = (EPackageType)msgItemData.pack;
            itemData.GridIndex = (int) msgItemData.grid;
            itemData.StrengthenLevel = msgItemData.strengthen;
            itemData.SubQuality = (int)msgItemData.qualitylv;
            itemData.ItemTradeNumber = (int) msgItemData.auctionTransNum;
            itemData.IsTreasure = msgItemData.isTreas == 1 ? true : false;
            itemData.AuctionCoolTimeStamp = msgItemData.auctionCoolTimeStamp;
            itemData.DeadTimestamp = (int)msgItemData.deadLine;        
            itemData.TransferStone = (int)msgItemData.transferStone;
            itemData.RecoScore = (int)msgItemData.recoScore;

            itemData.finalRateScore = (int)msgItemData.valueScore;

            itemData.bLocked = (msgItemData.lockItem & (int)ItemLockType.ILT_ITEM_LOCK) == (int)ItemLockType.ILT_ITEM_LOCK;
            itemData.IsLease = (msgItemData.lockItem & (int)ItemLockType.ILT_LEASE_LOCK) == (int)ItemLockType.ILT_LEASE_LOCK;
            itemData.bFashionItemLocked = (msgItemData.lockItem & (int)ItemLockType.ILT_FASHION_LOCK) == (int)ItemLockType.ILT_FASHION_LOCK;
            itemData.FashionFreeTimes = (int)msgItemData.fashionFreeSelNum;
 
            itemData.BaseProp.props[(int)EEquipProp.PhysicsAttack] = (int)msgItemData.phyatk;
            itemData.BaseProp.props[(int)EEquipProp.IgnorePhysicsAttack] = (int)msgItemData.disPhyAtk;
            itemData.BaseProp.props[(int)EEquipProp.IngoreIndependence] = (int)msgItemData.independAtkStreng;
            itemData.BaseProp.props[(int)EEquipProp.MagicAttack] = (int)msgItemData.magatk;
            itemData.BaseProp.props[(int)EEquipProp.IgnoreMagicAttack] = (int)msgItemData.disMagAtk;

            itemData.BaseProp.props[(int)EEquipProp.PhysicsDefense] = (int)msgItemData.phydef;
            itemData.BaseProp.props[(int)EEquipProp.IgnorePhysicsDefense] = (int)msgItemData.disPhyDef;
            itemData.BaseProp.props[(int)EEquipProp.IgnorePhysicsDefenseRate] = (int)msgItemData.disPhyDefRate;
            itemData.BaseProp.props[(int)EEquipProp.MagicDefense] = (int)msgItemData.magdef;
            itemData.BaseProp.props[(int)EEquipProp.IgnoreMagicDefense] = (int)msgItemData.disMagDef;
            itemData.BaseProp.props[(int)EEquipProp.IgnoreMagicDefenseRate] = (int)msgItemData.disMagDefRate;

            if (itemData.SubType != (int)ItemTable.eSubType.ST_BXY_EQUIP)
            {
                itemData.BaseProp.props[(int)EEquipProp.Strenth] = (int)msgItemData.strenth;
                itemData.BaseProp.props[(int)EEquipProp.Intellect] = (int)msgItemData.intellect;
                itemData.BaseProp.props[(int)EEquipProp.Spirit] = (int)msgItemData.spirit;
                itemData.BaseProp.props[(int)EEquipProp.Stamina] = (int)msgItemData.stamina;
                itemData.BaseProp.props[(int)EEquipProp.Independence] = (int)msgItemData.independAtk;//独立攻击力
            }
            else
            {
                if(msgItemData.strenth != 0)
                {
                    itemData.AddAttachBxyBuffIID(itemData.BaseProp, 0, (int)msgItemData.strenth);
                }
                if(msgItemData.intellect != 0)
                {
                    itemData.AddAttachBxyBuffIID(itemData.BaseProp, 0, (int)msgItemData.intellect);
                }
                if(msgItemData.spirit != 0)
                {
                    itemData.AddAttachBxyBuffIID(itemData.BaseProp, 0, (int)msgItemData.spirit);
                }
                if(msgItemData.stamina != 0)
                {
                    itemData.AddAttachBxyBuffIID(itemData.BaseProp, 0, (int)msgItemData.stamina);
                }
            }
            

            itemData.BaseProp.magicElementsAttack[(int)(MagicElementType.LIGHT)] = (int)msgItemData.strPropLight;
            itemData.BaseProp.magicElementsAttack[(int)(MagicElementType.FIRE)] = (int)msgItemData.strPropFire;
            itemData.BaseProp.magicElementsAttack[(int)(MagicElementType.ICE)] = (int)msgItemData.strPropIce;
            itemData.BaseProp.magicElementsAttack[(int)(MagicElementType.DARK)] = (int)msgItemData.strPropDark;

            itemData.BaseProp.magicElementsDefence[(int)(MagicElementType.LIGHT)] = (int)msgItemData.defPropLight;
            itemData.BaseProp.magicElementsDefence[(int)(MagicElementType.FIRE)] = (int)msgItemData.defPropFire;
            itemData.BaseProp.magicElementsDefence[(int)(MagicElementType.ICE)] = (int)msgItemData.defPropIce;
            itemData.BaseProp.magicElementsDefence[(int)(MagicElementType.DARK)] = (int)msgItemData.defPropDark;

            itemData.BaseProp.props[(int)EEquipProp.AbormalResist] = (int)msgItemData.abnormalResistsTotal;

            //abnormal resist total?
            itemData.BaseProp.abnormalResists[0] = (int)msgItemData.abnormalResistFlash;
            itemData.BaseProp.abnormalResists[1] = (int)msgItemData.abnormalResistBleeding;
            itemData.BaseProp.abnormalResists[2] = (int)msgItemData.abnormalResistBurn;
            itemData.BaseProp.abnormalResists[3] = (int)msgItemData.abnormalResistPoison;
            itemData.BaseProp.abnormalResists[4] = (int)msgItemData.abnormalResistBlind;
            itemData.BaseProp.abnormalResists[5] = (int)msgItemData.abnormalResistStun;
            itemData.BaseProp.abnormalResists[6] = (int)msgItemData.abnormalResistStone;
            itemData.BaseProp.abnormalResists[7] = (int)msgItemData.abnormalResistFrozen;
            itemData.BaseProp.abnormalResists[8] = (int)msgItemData.abnormalResistSleep;
            itemData.BaseProp.abnormalResists[9] = (int)msgItemData.abnormalResistConfunse;
            itemData.BaseProp.abnormalResists[10] = (int)msgItemData.abnormalResistStrain;
            itemData.BaseProp.abnormalResists[11] = (int)msgItemData.abnormalResistSpeedDown;
            itemData.BaseProp.abnormalResists[12] = (int)msgItemData.abnormalResistCurse;

            // 时装属性加成
            itemData.FashionAttributeID = (int)msgItemData.fashionAttributeID;

            // 注意：真实的时装道具的属性加成目前全部是客户端读表的，而且一直都是这么做的....,根本没有使用服务器数据，另外服务器的时装重选数据还都是错的
            if (itemData.Type == EItemType.FASHION)
            {
                // 重置BaseProp数据
                itemData.BaseProp = new EquipProp();

                // 这里一定要放在服务器下发的四维属性赋值以后再+=，因为服务器只下发重选属性，不下发基础时装属性，不然放在前面计算又会被服务器数据刷掉
                // 基础属性
                EquipProp BaseEquipProp = EquipProp.CreateFromTable(itemData.FashionBaseAttributeID);
                if (BaseEquipProp != null)
                {
                    itemData.BaseProp += BaseEquipProp;
                }

                // 重选属性
                EquipProp equipProp = EquipProp.CreateFromTable(itemData.FashionAttributeID);
                if (equipProp != null)
                {
                    itemData.BaseProp += equipProp;
                }
            }

            itemData.EquipType = (EEquipType)msgItemData.equipType;
            itemData.GrowthAttrType = (EGrowthAttrType)msgItemData.enhanceType;
            itemData.GrowthAttrNum = (int)msgItemData.enhanceNum;
            
            //   itemData.IsLease = (msgItemData.isLease == 1);
            //var magicPropertys = msgItemData.magicProps;

            if (itemData.SubType != (int)ProtoTable.ItemTable.eSubType.EnchantmentsCard)
            {
                itemData.mPrecEnchantmentCard = SwichPrecEnchantmentCard(msgItemData.mountedMagic);
            }

            if (itemData.SubType != (int)ItemTable.eSubType.Bead)
            {
                itemData.PreciousBeadMountHole = SwitchPrecBead(msgItemData.preciousBeadHoles);
            }
            else
            {
                itemData.BeadAdditiveAttributeBuffID = (int)msgItemData.param2;
                itemData.BeadPickNumber = (int)msgItemData.beadExtipreCnt;
                itemData.BeadReplaceNumber = (int)msgItemData.beadReplaceCnt;
            }

            if (itemData.Type == EItemType.EQUIP && itemData.Quality > ItemTable.eColor.PURPLE)
            {
                if (msgItemData.inscriptionHoles != null)
                {
                    bool bIsHasData = false;
                    for (int i = 0; i < msgItemData.inscriptionHoles.Length; i++)
                    {
                        if (msgItemData.inscriptionHoles[i] == null)
                        {
                            continue;
                        }

                        bIsHasData = true;
                        break;
                    }

                    if (bIsHasData)
                    {
                        itemData.InscriptionHoles = SwichInscriptionHoleData(msgItemData.inscriptionHoles,itemData);
                    }
                    else
                    {
                        itemData.InscriptionHoles = InscriptionMosaicDataManager.GetInstance().GetEquipmentInscriptionHoleData(itemData);
                    }
                }
            }

            for (int i = 0; i < msgItemData.randProps.Length; ++i)
            {
                ItemRandProp data = msgItemData.randProps[i];
                if (data == null)
                {
                    continue;
                }
                EServerProp prop = (EServerProp)data.type;
                Type type = prop.GetType();
                string name = System.Enum.GetName(type, prop);
                if (string.IsNullOrEmpty(name) == false)
                {
                    FieldInfo field = type.GetField(name);
                    MapEnum attribute = System.Attribute.GetCustomAttribute(field, typeof(MapEnum)) as MapEnum;
                    if (attribute != null)
                    {
                        int id = (int)attribute.Prop;
                        itemData.RandamProp.props[id] = (int)data.value;
                    }
                }
            }

            #region EquipPlanFlag
            //是否存在于未启用装备方案的标志
            itemData.IsItemInUnUsedEquipPlan = EquipPlanUtility.IsItemInUnUsedEquipPlanByItemData(itemData);
            #endregion

            return itemData;
        }

        public ItemData CreateItem(int id, int count)
        {
            ItemData itemData = new ItemData(id);
            //itemData.IsNew = Convert.ToBoolean(msgItemData.param1);
            //itemData.TableID = (int)msgItemData.dataid;
            _InitTableData(itemData);
            itemData.Count = count;
            return itemData;
        }

        /// <summary>
        /// 强制广播背包当前有没有满
        /// </summary>
        public void NotifyPackageFullState()
        {
            if (IsPackageFull())
            {
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PackageFull);
            }
            else
            {
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.PackageNotFull);
            }
        }

        #region private
        /// <summary>
        /// 注册使用罐子的返回的回调，罐子协议比较特殊，他是通过通用的使用道具的接口，所以有通用使用道具的返回
        /// 同时又有特殊的使用罐子的返回，用来处理罐子开启效果
        /// </summary>
        /// <param name="a_item"></param>
        void _BindUseJarMsgHandle(ItemData a_item)
        {
            WaitNetMessageManager.GetInstance().Wait<SceneUseItemRet>(msgRet =>
            {
                if (msgRet.code != (uint)ProtoErrorCode.SUCCESS)
                {
                    SystemNotifyManager.SystemNotify((int)msgRet.code);
                }
                else
                {
                    Logger.Log("use item success!!!!");
                    _NotifyItemUseSuccess(a_item);
                }
            });

            WaitNetMessageManager.GetInstance().Wait(SceneUseMagicJarRet.MsgID, (MsgDATA data) =>
            {
                if (data == null)
                {
                    return;
                }

                SceneUseMagicJarRet msgRet = new SceneUseMagicJarRet();

                int nPos = 0;
                msgRet.decode(data.bytes, ref nPos);
                List<Item> items = ItemDecoder.Decode(data.bytes, ref nPos, data.bytes.Length);

                if (msgRet.code == (uint)ProtoErrorCode.SUCCESS)
                {
                    RedPointDataManager.GetInstance().NotifyRedPointChanged();

                    List<JarBonus> arrBonus = new List<JarBonus>();

                    {
                        JarBonus jarBonus = new JarBonus();
                        jarBonus.nBonusID = 0;
                        jarBonus.item = ItemDataManager.CreateItemDataFromTable((int)msgRet.baseItem.id);
                        jarBonus.item.Count = (int)msgRet.baseItem.num;
                        jarBonus.bHighValue = false;
                        arrBonus.Add(jarBonus);
                    }

                    for (int i = 0; i < msgRet.getItems.Length; ++i)
                    {
                        OpenJarResult reward = msgRet.getItems[i];
                        JarBonus jarBonus = new JarBonus();
                        jarBonus.nBonusID = (int)reward.jarItemId;

                        ProtoTable.JarItemPool table = TableManager.GetInstance().GetTableItem<ProtoTable.JarItemPool>((int)reward.jarItemId);

                        ItemData itemData = null;
                        for (int j = 0; j < items.Count; ++j)
                        {
                            if (table.ItemID == items[j].dataid)
                            {
                                items[j].num -= (ushort)table.ItemNum;

                                //Logger.LogErrorFormat("jar result id:{0} num:{1} leftNum:{2}", table.ItemID, table.ItemNum, items[j].num);

                                itemData = ItemDataManager.GetInstance().CreateItemDataFromNet(items[j]);
                                itemData.Count = table.ItemNum; ;

                                if (items[j].num <= 0)
                                    items.RemoveAt(j);

                                break;
                            }
                        }

                        if (itemData == null)
                        {
                            //Logger.LogErrorFormat("jar not find!!!! result id:{0} num:{1} ", table.ItemID, table.ItemNum);
                            itemData = ItemDataManager.CreateItemDataFromTable((int)table.ItemID);
                            itemData.Count = table.ItemNum;
                        }

                        jarBonus.item = itemData;
                        jarBonus.bHighValue = table.ShowEffect == 1;
                        //jarBonus.bHighValue = true;
                        arrBonus.Add(jarBonus);

                    }

                    ShowItemsFrameData frameData = new ShowItemsFrameData();
                    frameData.data = JarDataManager.GetInstance().GetJarData((int)msgRet.jarID);
                    frameData.items = arrBonus;
                    frameData.buyInfo = null;
                    frameData.scoreItemData = ItemDataManager.CreateItemDataFromTable((int)msgRet.getPointId);
                    if (frameData.scoreItemData != null)
                    {
                        frameData.scoreItemData.Count = (int)msgRet.getPoint;
                    }
                    frameData.scoreRate = (int)msgRet.crit;

                    ClientSystemManager.GetInstance().OpenFrame<JarBuyResultFrame>(FrameLayer.Middle, frameData);
                }
            });
        }

        static void _CreateFashionAttributeItems(ItemData data)
        {
            if (data.TableData == null || data.TableData.EquipPropID == 0)
            {
                return;
            }

            var item = TableManager.GetInstance().GetTableItem<FashionAttributesConfigTable>(data.TableData.EquipPropID);
            if(null != item)
            {
                for(int i = 0;i < item.Attributes.Count; ++i)
                {
                    var attributeItem = TableManager.GetInstance().GetTableItem<EquipAttrTable>(item.Attributes[i]);
                    if(attributeItem != null)
                    {
                        if(data.fashionAttributes == null)
                        {
                            data.fashionAttributes = new List<EquipAttrTable>();
                        }
                        data.fashionAttributes.Add(attributeItem);
                    }
                }
            }
        }

        /// <summary>
        /// 初始化道具表格数据
        /// </summary>
        /// <param name="data">道具对象</param>
        static void _InitTableData(ItemData data)
        {
            if (data.IsTableDataInited == false)
            {
                ProtoTable.ItemTable tableData = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>((int)data.TableID);
                if (tableData == null)
                {
                    return;
                }

                data.GUID = 0;
                data.Type = tableData.Type;
                data.PackageType = EPackageType.Invalid;
                data.SubType = (int)tableData.SubType;
                data.ThirdType = tableData.ThirdType;
                if (data.Type == EItemType.EQUIP)
                {
                    data.EquipWearSlotType = (EEquipWearSlotType)tableData.SubType;
                    data.FashionWearSlotType = EFashionWearSlotType.Invalid;
                    if(tableData.SubType == EItemSubType.ST_ASSIST_EQUIP)
                    {
                        data.EquipWearSlotType = EEquipWearSlotType.Equipassist1;
                    }
                    else if(tableData.SubType == EItemSubType.ST_MAGICSTONE_EQUIP)
                    {
                        data.EquipWearSlotType = EEquipWearSlotType.Equipassist2;
                    }
                    else if(tableData.SubType == EItemSubType.ST_EARRINGS_EQUIP)
                    {
                        data.EquipWearSlotType = EEquipWearSlotType.Equipassist3;
                    }
                    else if(tableData.SubType == EItemSubType.ST_BXY_EQUIP)
                    {
                        // marked by ckm
                        data.EquipWearSlotType = EEquipWearSlotType.Equipcloak;
                    }
                }
                else if (data.Type == EItemType.FUCKTITTLE)
                {
                    data.EquipWearSlotType = (EEquipWearSlotType)tableData.SubType;
                    data.FashionWearSlotType = EFashionWearSlotType.Invalid;
                }
                else if (data.Type == EItemType.FASHION)
                {
                    data.EquipWearSlotType = EEquipWearSlotType.EquipInvalid;
                    if (tableData.SubType == ItemTable.eSubType.FASHION_WEAPON) //武器时装
                    {
                        data.FashionWearSlotType = EFashionWearSlotType.Weapon;
                    }
                    else if(tableData.SubType == EItemSubType.FASHION_AURAS) // 表格中的时装光环枚举是从后面追加的，不能直接减去10
                    {
                        data.FashionWearSlotType = EFashionWearSlotType.Auras;
                    }
                    else
                    {
                        data.FashionWearSlotType = (EFashionWearSlotType) (tableData.SubType - 10); //时装subtype值从11开始
                    }
                    _CreateFashionAttributeItems(data);
                }

                data.Count = 0;
                data.MaxStackCount = tableData.MaxNum;
                data.Quality = tableData.Color;
                data.Quality2 = tableData.Color2;
                data.SubQuality = 100;
                //data.Icon = tableData.Icon;
                //data.Name = tableData.Name;
                data.BindAttr = tableData.Owner;
                data.Packing = tableData.IsSeal;
                data.iPackedTimes = 0;
                data.iMaxPackTime = tableData.SealMax;
                data.RePackTime = data.iMaxPackTime - data.iPackedTimes;
                data.LevelLimit = tableData.NeedLevel;
                data.MaxLevelLimit = tableData.MaxLevel;
                data.OccupationLimit = new List<int>(tableData.Occu);
                _UpdateQccupationLimit(data.OccupationLimit);
                //data.Description = tableData.Description;
                //data.SourceDescription = tableData.ComeDesc;
                //data.EffectDescription = tableData.EffectDescription;
                data.UseType = tableData.CanUse;
                data.CanSell = tableData.CanTrade;
                data.CD = tableData.CoolTime;
                data.CDGroupID = tableData.CdGroup;
                data.useLimitType = tableData.UseLimiteType;
                data.useLimitValue = tableData.UseLimiteValue;
                data.FixTimeLeft = tableData.TimeLeft;
                if (data.SubType == (int)ProtoTable.ItemTable.eSubType.EnchantmentsCard)
                {
                    data.mPrecEnchantmentCard = new PrecEnchantmentCard
                    {
                        iEnchantmentCardID = data.TableID,
                        iEnchantmentCardLevel = 0,
                    };

                    var magicCardTable = TableManager.GetInstance().GetTableItem<MagicCardTable>(tableData.ID);
                    if(magicCardTable != null)
                    {
                        data.EnchantmentCardStage = magicCardTable.Stage;
                    }
                }
                else
                {
                    data.mPrecEnchantmentCard = new PrecEnchantmentCard
                    {
                        iEnchantmentCardID = 0,
                        iEnchantmentCardLevel = 0,
                    };
                }
                data.DeadTimestamp = tableData.ExpireTime;
                data.Price = tableData.Price;
                data.PriceItemID = tableData.SellItemID;
                data.StrengthenLevel = 0;
                data.CanDecompose = tableData.IsDecompose;
                data.SuitID = tableData.SuitID;
                data.PackID = tableData.PackageID;
                data.BaseAttackSpeedRate = tableData.BaseAttackSpeedRate;

                if(tableData.Type != EItemType.FASHION)
                {
                    EquipProp equipProp = EquipProp.CreateFromTable(tableData.EquipPropID);
                    if (equipProp != null)
                    {
                        data.BaseProp = equipProp;
                    }
                }
                else
                {
                    // 以前时装没有基础属性(DefaultAttribute_1)，现在时装也可以配置基础属性（光环）了,这里只给id初始化，属性加成挪到外面计算
                    // 对于真实的道具，正常装备可以在这里给BaseProp计算属性加成，在CreateItemFromNet里再用服务器数据刷掉，但是时装不行，
                    // 因为服务器只同步时装重选的四维属性，不同步时装的基础属性，对于假道具而言，虽然可以在这里处理属性加成，
                    // 但是由于CreateItemDataFromTable和CreateItemDataFromNet都在调用该函数，流程上就不统一了，所以假道具也要挪到外面去计算属性加成
                    var item = TableManager.GetInstance().GetTableItem<FashionAttributesConfigTable>(tableData.EquipPropID);
                    if(null != item)
                    {
                        data.FashionBaseAttributeID = item.DefaultAttribute_1;
                        data.FashionAttributeID = item.DefaultAttribute;
                    }
                }

                if (tableData.RenewInfo != null && tableData.RenewInfo.Count > 0)
                {
                    data.arrRenewals = new List<RenewalInfo>();
                    for (int i = 0; i < tableData.RenewInfo.Count; ++i)
                    {
                        if (string.IsNullOrEmpty(tableData.RenewInfo[i]))
                        {
                            continue;
                        }
                        string[] arrValues = tableData.RenewInfo[i].Split(',');
                        Assert.IsTrue(arrValues.Length >= 3);

                        RenewalInfo info = new RenewalInfo();
                        info.nDay = int.Parse(arrValues[0]);
                        info.nCostID = int.Parse(arrValues[1]);
                        info.nCostCount = int.Parse(arrValues[2]);
                        data.arrRenewals.Add(info);
                    }
                }

                data.IsSelected = false;
                data.IsNew = false;

                data.IsTableDataInited = true;
            }
        }

        /// <summary>
        /// 职业限制里，设计成，父职业限制了，子职业一定会限制，
        /// 该函数，就是过滤掉多余的子职业
        /// </summary>
        /// <param name="occupationLimit">职业限制列表</param>
        static void _UpdateQccupationLimit(List<int> occupationLimit)
        {
            occupationLimit.RemoveAll(data => { return data == 0; });
            //List<int> ignoreJobs = new List<int>();
            //for (int i = 0; i < occupationLimit.Count; ++i)
            //{
            //    _GetToJobs(ref ignoreJobs, occupationLimit[i]);
            //}

            //for (int i = 0; i < occupationLimit.Count; ++i)
            //{
            //    if (ignoreJobs.Contains(occupationLimit[i]))
            //    {
            //        occupationLimit.RemoveAt(i);
            //        i--;
            //    }
            //}
            
        }

        /// <summary>
        /// 获取特定职业的所有子职业
        /// </summary>
        /// <param name="toJobList">子职业列表</param>
        /// <param name="job">职业</param>
        static void _GetToJobs(ref List<int> toJobList, int job)
        {
            if (job <= 0)
            {
                return;
            }

            ProtoTable.JobTable table = TableManager.instance.GetTableItem<ProtoTable.JobTable>(job);
            if (table == null)
            {
                return;
            }

            for (int i = 0; i < table.ToJob.Count; ++i)
            {
                int toJob = table.ToJob[i];
                if (toJob > 0)
                {
                    if (toJobList.Contains(toJob) == false)
                    {
                        toJobList.Add(toJob);
                        _GetToJobs(ref toJobList, toJob);
                    }
                }
            }
        }

        /// <summary>
        /// 绑定网络消息
        /// </summary>
        void _BindNetMessage()
        {
            NetProcess.AddMsgHandler(SceneSellItemBatRes.MsgID, _OnRecvSceneSellItemBatRes);
            NetProcess.AddMsgHandler(SceneSynItem.MsgID, _OnAddItem);
            NetProcess.AddMsgHandler(SceneNotifyDeleteItem.MsgID, _OnRemoveItem);
            NetProcess.AddMsgHandler(SceneSyncItemProp.MsgID, _OnUpdateItem);
            NetProcess.AddMsgHandler(SceneNotifyGetItem.MsgID, _OnNotifyGetItem);
            NetProcess.AddMsgHandler(SceneNotifyCostItem.MsgID, _OnNotifyCostItem);
            NetProcess.AddMsgHandler(SCNotifyTimeItem.MsgID, _OnSCNotifyTimeItem);
        }

        /// <summary>
        /// 解绑网络消息
        /// </summary>
        void _UnBindNetMessage()
        {
            NetProcess.RemoveMsgHandler(SceneSellItemBatRes.MsgID, _OnRecvSceneSellItemBatRes);
            NetProcess.RemoveMsgHandler(SceneSynItem.MsgID, _OnAddItem);
            NetProcess.RemoveMsgHandler(SceneNotifyDeleteItem.MsgID, _OnRemoveItem);
            NetProcess.RemoveMsgHandler(SceneSyncItemProp.MsgID, _OnUpdateItem);
            NetProcess.RemoveMsgHandler(SceneNotifyGetItem.MsgID, _OnNotifyGetItem);
            NetProcess.RemoveMsgHandler(SceneNotifyCostItem.MsgID, _OnNotifyCostItem);
            NetProcess.RemoveMsgHandler(SCNotifyTimeItem.MsgID, _OnSCNotifyTimeItem);
        }

        /// <summary>
        /// 遍历道具表，初始化一些map，以便快速访问
        /// </summary>
        void _InitMoneyTableData()
        {
            var table = TableManager.GetInstance().GetTable<ProtoTable.ItemTable>();
            if (table != null)
            {
                var enumerator = table.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    var item = enumerator.Current.Value as ProtoTable.ItemTable;

                    if (item.Type == EItemType.INCOME)
                    {
                        m_commonTableItemDict.Add(item.ID, CreateItemDataFromTable(item.ID));

                        // 货币类型道具客户端的一种货币类型对应一个道具id,服务器有需求填两个道具，但是客户端没必要报错
                        if (!m_moneyTypeIDDict.ContainsKey(item.SubType))
                        {
                            m_moneyTypeIDDict.Add(item.SubType, item.ID);
                        }
                        else
                        {
                            //Logger.LogErrorFormat("m_moneyTypeIDDict alreay has:{0} id:{1}", item.SubType, item.ID);
                        }
                    }
                    else if(item.Type == EItemType.EQUIP || item.Type == EItemType.EXPENDABLE || item.Type == EItemType.MATERIAL || item.Type == EItemType.FUCKTITTLE)
                    {
                        if(!TradeItemStateFliter(item))
                        {
                            continue;
                        }

                        EItemType eTmpType = item.Type;
                        if (item.Type == EItemType.FUCKTITTLE)
                        {
                            eTmpType = EItemType.EQUIP;
                        }

                        Dictionary<EItemSubType, List<int>> SubTypeList = null;

                        if(!m_AuctionMainTypeIDDict.TryGetValue(eTmpType, out SubTypeList))
                        {
                            SubTypeList = new Dictionary<EItemSubType, List<int>>(new EItemSubComparer());

                            List<int> itemlist = new List<int>();
                            itemlist.Add(item.ID);

                            SubTypeList.Add(item.SubType, itemlist);

                            m_AuctionMainTypeIDDict.Add(eTmpType, SubTypeList);
                        }
                        else
                        {
                            List<int> itemlist = null;
                            if (!SubTypeList.TryGetValue(item.SubType, out itemlist))
                            {
                                itemlist = new List<int>();
                                itemlist.Add(item.ID);

                                SubTypeList.Add(item.SubType, itemlist);
                            }
                            else
                            {
                                itemlist.Add(item.ID);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 广播道具使用成功
        /// </summary>
        /// <param name="a_item"></param>
        void _NotifyItemUseSuccess(ItemData a_item)
        {
            if (_TryNotifyMagicJarUseSuccess(a_item) == false)
            {
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ItemUseSuccess, a_item);
            }
        }

        /// <summary>
        /// 广播罐子使用成功
        /// </summary>
        /// <param name="a_item"></param>
        /// <returns></returns>
        bool _TryNotifyMagicJarUseSuccess(ItemData a_item)
        {
//             if (m_magicJarData != null)
//             {
//                 if (m_magicJarData.ret.code != (uint)ProtoErrorCode.SUCCESS)
//                 {
//                     SystemNotifyManager.SystemNotify((int)m_magicJarData.ret.code);
//                 }
//                 else
//                 {
//                     m_magicJarData.item = a_item;
//                     ClientSystemManager.GetInstance().OpenFrame<MagicJarFrame>(FrameLayer.Top, m_magicJarData);
//                 }
// 
//                 m_magicJarData = null;
//                 return true;
//             }
//             else
            {
                return false;
            }
        }

        void _OnPackageItemEquiped(ItemData pre, ItemData aft)
        {
            ClientSystemGameBattle ChijiTown = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemGameBattle;
            if (ChijiTown != null)
            {
                return;
            }
            var data = _GetEquipmentPropChanges(pre, aft);
            if (data != null)
            {
                PopUpChangedAttrbutes(data);
            }
        }

        public EquipProp _GetSuitProp(EPackageType eEPackageType, ItemData pre, ItemData aft,bool bUsePre)
        {
            EquipProp ePresSuitProp = null;
            List<ulong> guids = new List<ulong>();
            var outGuids = GetItemsByPackageType(eEPackageType);
            if(null != outGuids)
            {
                guids.AddRange(outGuids);
            }

            if (null != guids)
            {
                if(bUsePre && null != aft)
                {
                    guids.RemoveAll(x => { return aft.GUID == x; });
                }

                if(!bUsePre && null != pre)
                {
                    guids.RemoveAll(x => { return pre.GUID == x; });
                }

                if(bUsePre && null != pre)
                {
                    if(pre.Type == EItemType.FASHION && eEPackageType == EPackageType.WearFashion ||
                        pre.Type == EItemType.EQUIP && eEPackageType == EPackageType.WearEquip)
                    {
                        guids.RemoveAll(x => { return pre.GUID == x; });
                        guids.Add(pre.GUID);
                    }
                }

                if(!bUsePre && null != aft)
                {
                    if (aft.Type == EItemType.FASHION && eEPackageType == EPackageType.WearFashion ||
                        aft.Type == EItemType.EQUIP && eEPackageType == EPackageType.WearEquip)
                    {
                        guids.RemoveAll(x => { return aft.GUID == x; });
                        guids.Add(aft.GUID);
                    }
                }
            }

            var list = GamePool.ListPool<int>.Get();
            for (int i = 0; i < guids.Count; ++i)
            {
                var itemData = GetItem(guids[i]);
                if (null != itemData)
                {
                    list.Add(itemData.TableID);
                }
            }

            ePresSuitProp = EquipSuitDataManager.GetInstance().GetEquipSuitBasePropByIDs(list);
            GamePool.ListPool<int>.Release(list);
            if (null == ePresSuitProp)
            {
                ePresSuitProp = new EquipProp();
            }
            return ePresSuitProp;
        }
        
        List<BetterEquipmentData> _GetEquipmentPropChanges(ItemData pre, ItemData aft)
        {
            List<BetterEquipmentData> data = new List<BetterEquipmentData>();
            var current = (null == aft) ? new EquipProp() : aft.GetEquipProp();
            current += _GetSuitProp(EPackageType.WearEquip, pre, aft, false);
            current += _GetSuitProp(EPackageType.WearFashion, pre, aft, false);

            current.props[(int)EEquipProp.LightAttack] = current.magicElementsAttack[1];
            current.props[(int)EEquipProp.FireAttack] = current.magicElementsAttack[2];
            current.props[(int)EEquipProp.IceAttack] = current.magicElementsAttack[3];
            current.props[(int)EEquipProp.DarkAttack] = current.magicElementsAttack[4];

            current.props[(int)EEquipProp.LightDefence] = current.magicElementsDefence[1];
            current.props[(int)EEquipProp.FireDefence] = current.magicElementsDefence[2];
            current.props[(int)EEquipProp.IceDefence] = current.magicElementsDefence[3];
            current.props[(int)EEquipProp.DarkDefence] = current.magicElementsDefence[4];

            current.props[(int)EEquipProp.abnormalResist1] = current.abnormalResists[0];
            current.props[(int)EEquipProp.abnormalResist2] = current.abnormalResists[1];
            current.props[(int)EEquipProp.abnormalResist3] = current.abnormalResists[2];
            current.props[(int)EEquipProp.abnormalResist4] = current.abnormalResists[3];
            current.props[(int)EEquipProp.abnormalResist5] = current.abnormalResists[4];
            current.props[(int)EEquipProp.abnormalResist6] = current.abnormalResists[5];
            current.props[(int)EEquipProp.abnormalResist7] = current.abnormalResists[6];
            current.props[(int)EEquipProp.abnormalResist8] = current.abnormalResists[7];
            current.props[(int)EEquipProp.abnormalResist9] = current.abnormalResists[8];
            current.props[(int)EEquipProp.abnormalResist10] = current.abnormalResists[9];
            current.props[(int)EEquipProp.abnormalResist11] = current.abnormalResists[10];
            current.props[(int)EEquipProp.abnormalResist12] = current.abnormalResists[11];
            current.props[(int)EEquipProp.abnormalResist13] = current.abnormalResists[12];



            var preProperty = (null == pre) ? new EquipProp() : pre.GetEquipProp();
            preProperty += _GetSuitProp(EPackageType.WearEquip, pre, aft, true);
            preProperty += _GetSuitProp(EPackageType.WearFashion, pre, aft, true);

            preProperty.props[(int)EEquipProp.LightAttack] = preProperty.magicElementsAttack[1];
            preProperty.props[(int)EEquipProp.FireAttack] = preProperty.magicElementsAttack[2];
            preProperty.props[(int)EEquipProp.IceAttack] = preProperty.magicElementsAttack[3];
            preProperty.props[(int)EEquipProp.DarkAttack] = preProperty.magicElementsAttack[4];

            preProperty.props[(int)EEquipProp.LightDefence] = preProperty.magicElementsDefence[1];
            preProperty.props[(int)EEquipProp.FireDefence] = preProperty.magicElementsDefence[2];
            preProperty.props[(int)EEquipProp.IceDefence] = preProperty.magicElementsDefence[3];
            preProperty.props[(int)EEquipProp.DarkDefence] = preProperty.magicElementsDefence[4];

            preProperty.props[(int)EEquipProp.abnormalResist1] = preProperty.abnormalResists[0];
            preProperty.props[(int)EEquipProp.abnormalResist2] = preProperty.abnormalResists[1];
            preProperty.props[(int)EEquipProp.abnormalResist3] = preProperty.abnormalResists[2];
            preProperty.props[(int)EEquipProp.abnormalResist4] = preProperty.abnormalResists[3];
            preProperty.props[(int)EEquipProp.abnormalResist5] = preProperty.abnormalResists[4];
            preProperty.props[(int)EEquipProp.abnormalResist6] = preProperty.abnormalResists[5];
            preProperty.props[(int)EEquipProp.abnormalResist7] = preProperty.abnormalResists[6];
            preProperty.props[(int)EEquipProp.abnormalResist8] = preProperty.abnormalResists[7];
            preProperty.props[(int)EEquipProp.abnormalResist9] = preProperty.abnormalResists[8];
            preProperty.props[(int)EEquipProp.abnormalResist10] = preProperty.abnormalResists[9];
            preProperty.props[(int)EEquipProp.abnormalResist11] = preProperty.abnormalResists[10];
            preProperty.props[(int)EEquipProp.abnormalResist12] = preProperty.abnormalResists[11];
            preProperty.props[(int)EEquipProp.abnormalResist13] = preProperty.abnormalResists[12];

            for (int i = 0; i < current.props.Length; ++i)
            {
                if (current.props[i] == 0 && preProperty.props[i] == 0)
                {
                    continue;
                }

                var enumAttr = Utility.GetEnumAttribute<EEquipProp, PropAttribute>((EEquipProp)i);
                if (enumAttr == null)
                {
                    continue;
                }

                //特殊处理，强化（激化）固定攻击力，过滤掉，在基础攻击力分支加上强化固定攻击力
                if (i == (int)EEquipProp.IngoreIndependence)
                {
                    continue;
                }

                float iValue = 0;
                string strValue = enumAttr.desc;
                string name = strValue;
                string curValue = "";
                string preValue = "";

                EquipmentDataState eEquipmentDataState = EquipmentDataState.PROPERTY_NO_CHANGE;

                if (i >= (int)EEquipProp.AttackSpeedRate && i <= (int)EEquipProp.MoveSpeedRate ||
                    i >= (int)EEquipProp.HitRate && i <= (int)EEquipProp.MagicCritRate ||
                    (i >= (int)EEquipProp.PhysicsSkillMPChange && i <= (int)EEquipProp.MagicSkillCDChange) ||
                    (i == (int)EEquipProp.TownMoveSpeedRate))
                {

                    iValue =(float) Math.Round((float)current.props[i]/10,1);//(float)current.props[i] / 10;

					curValue = string.Format("{0}%", (float)current.props[i] / 10);
                    preValue = string.Format("{0}%", (float)preProperty.props[i] / 10);

                    if (current.props[i] / 10 > preProperty.props[i] / 10)
                    {
                        if (!enumAttr.bInverse)
                        {
                            eEquipmentDataState = EquipmentDataState.PROPERTY_UP;
                        }
                        else
                        {
                            eEquipmentDataState = EquipmentDataState.PROPERTY_DOWN;
                        }
                    }
                    else if (current.props[i] / 10 < preProperty.props[i] / 10)
                    {
                        if (!enumAttr.bInverse)
                        {
                            eEquipmentDataState = EquipmentDataState.PROPERTY_DOWN;
                        }
                        else
                        {
                            eEquipmentDataState = EquipmentDataState.PROPERTY_UP;
                        }
                    }
                    else
                    {
                        eEquipmentDataState = EquipmentDataState.PROPERTY_NO_CHANGE;
                    }
                }
                else if (i >= (int)EEquipProp.Strenth && i <= (int)EEquipProp.Stamina)
                {
                    iValue = (float)Math.Round((float)current.props[i]/1000,1);// current.props[i] / 1000;
                    float preiValue = (float)Math.Round((float)preProperty.props[i] / 1000, 1);// preProperty.props[i] / 1000;

                    if (aft != null)
                    {
                        switch (aft.GrowthAttrType)
                        {
                            case EGrowthAttrType.GAT_NONE:
                                break;
                            case EGrowthAttrType.GAT_STRENGTH:
                                if (i == (int)EEquipProp.Strenth)
                                {
                                    iValue += aft.GrowthAttrNum;
                                }
                                break;
                            case EGrowthAttrType.GAT_INTELLIGENCE:
                                if (i == (int)EEquipProp.Intellect)
                                {
                                    iValue += aft.GrowthAttrNum;
                                }
                                break;
                            case EGrowthAttrType.GAT_STAMINA:
                                if (i == (int)EEquipProp.Stamina)
                                {
                                    iValue += aft.GrowthAttrNum;
                                }
                                break;
                            case EGrowthAttrType.GAT_SPIRIT:
                                if (i == (int)EEquipProp.Spirit)
                                {
                                    iValue += aft.GrowthAttrNum;
                                }
                                break;
                        }
                    }

                    if (pre != null)
                    {
                        switch (pre.GrowthAttrType)
                        {
                            case EGrowthAttrType.GAT_NONE:
                                break;
                            case EGrowthAttrType.GAT_STRENGTH:
                                if (i == (int)EEquipProp.Strenth)
                                {
                                    preiValue += pre.GrowthAttrNum;
                                }
                                break;
                            case EGrowthAttrType.GAT_INTELLIGENCE:
                                if (i == (int)EEquipProp.Intellect)
                                {
                                    preiValue += pre.GrowthAttrNum;
                                }
                                break;
                            case EGrowthAttrType.GAT_STAMINA:
                                if (i == (int)EEquipProp.Stamina)
                                {
                                    preiValue += pre.GrowthAttrNum;
                                }
                                break;
                            case EGrowthAttrType.GAT_SPIRIT:
                                if (i == (int)EEquipProp.Spirit)
                                {
                                    preiValue += pre.GrowthAttrNum;
                                }
                                break;
                        }
                    }
                   

                    curValue = string.Format("{0:F1}", iValue);
                    preValue = string.Format("{0:F1}", preiValue);

                    if (iValue > preiValue)
                    {
                        if (!enumAttr.bInverse)
                        {
                            eEquipmentDataState = EquipmentDataState.PROPERTY_UP;
                        }
                        else
                        {
                            eEquipmentDataState = EquipmentDataState.PROPERTY_DOWN;
                        }
                    }
                    else if (iValue < preiValue)
                    {
                        if (!enumAttr.bInverse)
                        {
                            eEquipmentDataState = EquipmentDataState.PROPERTY_DOWN;
                        }
                        else
                        {
                            eEquipmentDataState = EquipmentDataState.PROPERTY_UP;
                        }
                    }
                    else
                    {
                        eEquipmentDataState = EquipmentDataState.PROPERTY_NO_CHANGE;
                    }
                }
                else if (i == (int)EEquipProp.Independence)
                {
                    int currentvalue = (int)Math.Round((float)current.props[i] / 1000, 1);// current.props[i] / 1000;
                    int preiValue = (int)Math.Round((float)preProperty.props[i] / 1000, 1);// preProperty.props[i] / 1000;

                    currentvalue += (int)Math.Round((float)current.props[(int)EEquipProp.IngoreIndependence] / 1000, 1);
                    preiValue += (int)Math.Round((float)preProperty.props[(int)EEquipProp.IngoreIndependence] / 1000, 1);

                    curValue = string.Format("{0:F1}", currentvalue);
                    preValue = string.Format("{0:F1}", preiValue);

                    if (currentvalue > preiValue)
                    {
                        if (!enumAttr.bInverse)
                        {
                            eEquipmentDataState = EquipmentDataState.PROPERTY_UP;
                        }
                        else
                        {
                            eEquipmentDataState = EquipmentDataState.PROPERTY_DOWN;
                        }
                    }
                    else if (currentvalue < preiValue)
                    {
                        if (!enumAttr.bInverse)
                        {
                            eEquipmentDataState = EquipmentDataState.PROPERTY_DOWN;
                        }
                        else
                        {
                            eEquipmentDataState = EquipmentDataState.PROPERTY_UP;
                        }
                    }
                    else
                    {
                        eEquipmentDataState = EquipmentDataState.PROPERTY_NO_CHANGE;
                    }
                }
                else
                {
                    iValue = current.props[i];
                    curValue = string.Format("{0}", current.props[i]);
                    preValue = string.Format("{0}", preProperty.props[i]);

                    if (current.props[i] > preProperty.props[i])
                    {
                        if (!enumAttr.bInverse)
                        {
                            eEquipmentDataState = EquipmentDataState.PROPERTY_UP;
                        }
                        else
                        {
                            eEquipmentDataState = EquipmentDataState.PROPERTY_DOWN;
                        }
                    }
                    else if (current.props[i] < preProperty.props[i])
                    {
                        if (!enumAttr.bInverse)
                        {
                            eEquipmentDataState = EquipmentDataState.PROPERTY_DOWN;
                        }
                        else
                        {
                            eEquipmentDataState = EquipmentDataState.PROPERTY_UP;
                        }
                    }
                    else
                    {
                        eEquipmentDataState = EquipmentDataState.PROPERTY_NO_CHANGE;
                    }
                }

                if(eEquipmentDataState != EquipmentDataState.PROPERTY_NO_CHANGE)
                {
                    var curData = new BetterEquipmentData();
                    curData.CurData = curValue;
                    curData.PreData = preValue;
                    curData.name = name;
                    curData.DataState = eEquipmentDataState;

                    data.Add(curData);
                }
            }

            return data;
        }

        public void PopUpChangedAttrbutes(List<BetterEquipmentData> data)
        {
            for (int i = 0; i < data.Count; ++i)
            {
                if(string.Equals(data[i].PreData,data[i].CurData))
                {
                    continue;
                }

                var mathFirst = ms_equip_attr_change_reg.Match(data[i].PreData);
                var mathSecond = ms_equip_attr_change_reg.Match(data[i].CurData);
                if(string.IsNullOrEmpty(mathFirst.Groups[0].Value) ||
                    string.IsNullOrEmpty(mathSecond.Groups[0].Value))
                {
                    continue;
                }

                float fPreValue = 0.0f;
                float fCurValue = 0.0f;
                if(!float.TryParse(mathFirst.Groups[1].Value, out fPreValue) ||
                    !float.TryParse(mathSecond.Groups[1].Value, out fCurValue))
                {
                    continue;
                }
                float fDeltaValue = (float)Math.Round(fCurValue - fPreValue,1);//fCurValue - fPreValue;

                string formatString = "";
                if (fDeltaValue > 0.0f)
                {
                    formatString = string.Format("{0} +{1}{2}", data[i].name, Math.Abs(fDeltaValue), mathFirst.Groups[2].Value);
                }
                else
                {
                    formatString = string.Format("{0} -{1}{2}", data[i].name, Math.Abs(fDeltaValue), mathSecond.Groups[2].Value);
                }

                SystemNotifyManager.SysNotifyFloatingEffect(formatString, CommonTipsDesc.eShowMode.SI_QUEUE);
            }
        }

        //强化效果变更
        void _OnStrengthenLevelChanged(ItemData data)
        {
            if (data.EquipWearSlotType == EEquipWearSlotType.EquipWeapon)
            {
                ulong uwid = GetWearEquipBySlotType(EEquipWearSlotType.EquipWeapon);
                if (uwid > 0 && data.GUID == uwid)
                {
                    ClientSystemTown townSystem = ClientSystemManager.instance.CurrentSystem as ClientSystemTown;
                    if (townSystem != null)
                    {
                        townSystem.MainPlayer.ShowEquipStrengthenEffect(data.StrengthenLevel);
                    }
                }
            }
        }

        /// <summary>
        /// 添加道具，将拥有的道具组织到数据结构中；只更新数据，不更新UI展示
        /// </summary>
        /// <param name="data">新增的道具</param>
        /// <param name="isRealAddItemInLogic"></param>
        /// <returns></returns>
        
        bool _AddItem(ItemData data, bool isRealAddItemInLogic = true)
        {
            if (data != null && m_itemsDict.ContainsKey(data.GUID) == false)
            {
                m_itemsDict.Add(data.GUID, data);

                if (m_itemPackageTypesDict.ContainsKey(data.PackageType) == false)
                {
                    m_itemPackageTypesDict.Add(data.PackageType, new List<ulong>());
                }
                m_itemPackageTypesDict[data.PackageType].Add(data.GUID);

                if (data.PackageType > EPackageType.Invalid && data.PackageType < EPackageType.Count
                    && data.IsNew)
                {
                    m_packageHasNew[(int)data.PackageType] += 1;
                    _NotifyItemNewStateChanged();
                }

                if (m_itemTypesDict.ContainsKey(data.Type) == false)
                {
                    m_itemTypesDict.Add(data.Type, new List<ulong>());
                }
                m_itemTypesDict[data.Type].Add(data.GUID);

                if (m_itemCDGroupDict.ContainsKey(data.CDGroupID) == false)
                {
                    m_itemCDGroupDict.Add(data.CDGroupID, new List<ulong>());
                }
                m_itemCDGroupDict[data.CDGroupID].Add(data.GUID);

                if(data.PackageType != EPackageType.Storage && data.Type != EItemType.INCOME && data.PackageType != EPackageType.RoleStorage)
                {
                    int iNum = 0;
                    if (m_ItemNumDict.TryGetValue(data.TableID, out iNum))
                    {
                        m_ItemNumDict[data.TableID] = iNum + data.Count;
                    }
                    else
                    {
                        m_ItemNumDict.Add(data.TableID, data.Count);
                    }
                }

                // 吃鸡模式特殊处理
                ClientSystemGameBattle systemTown = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemGameBattle;
                if (systemTown != null)
                {
                    CitySceneTable scenedata = TableManager.GetInstance().GetTableItem<CitySceneTable>(systemTown.CurrentSceneID);
                    if (scenedata != null)
                    {
                        if(scenedata.SceneType == CitySceneTable.eSceneType.BATTLE && scenedata.SceneSubType == CitySceneTable.eSceneSubType.Battle &&                
                            !ChijiDataManager.GetInstance().SwitchingChijiSceneToPrepare)
                        {
#if UNITY_EDITOR
                            //Logger.LogErrorFormat("吃鸡时序测试----接收道具; item id = {0}, name = {1}", data.TableID, data.TableData.Name);
#endif
                            if (data.TableData.Type == EItemType.EXPENDABLE && data.TableData.SubType == EItemSubType.GiftPackage)
                            {
                                // 吃鸡道具自动使用
                                _AutoUseChijiItem(data, isRealAddItemInLogic);
                            }
                            else if(data.TableData.Type == EItemType.EQUIP)
                            {
                                // 吃鸡捡装备自动更新上去
                                _AutoEquipChijiEquipment(data, isRealAddItemInLogic);
                            }
                        }
                    }
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// 删除道具，根据GUID，删除对应的数据结构;只删除数据，不更新UI
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="isSendUiEvent"></param>
        /// <returns></returns>
        bool _RemoveItem(ulong guid)
        {
            ItemData data = null;
            m_itemsDict.TryGetValue(guid, out data);
            if (data != null)
            {
                if (data.PackageType > EPackageType.Invalid && data.PackageType < EPackageType.Count
                && data.IsNew)
                {
                    m_packageHasNew[(int)data.PackageType] -= 1;
                    _NotifyItemNewStateChanged();
                }

                if (data.PackageType != EPackageType.Storage && data.Type != EItemType.INCOME && data.PackageType != EPackageType.RoleStorage)
                {
                    int iNum = 0;
                    if (m_ItemNumDict.TryGetValue(data.TableID, out iNum))
                    {
                        iNum -= data.Count;
                        m_ItemNumDict[data.TableID] = iNum;
                    }
                }

                m_itemsDict.Remove(guid);
        
                m_itemPackageTypesDict[data.PackageType].Remove(guid);
                m_itemTypesDict[data.Type].Remove(guid);
                m_itemCDGroupDict[data.CDGroupID].Remove(guid);

                return true;
            }

            return false;
        }

        /// <summary>
        /// 广播道具数量发生变化
        /// </summary>
        /// <param name="a_nTableID"></param>
        void _NotifyItemCountChanged(int a_nTableID)
        {
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ItemCountChanged, a_nTableID);

            // 道具数量变化，魔法罐红点，背包红点等都有可能发生变化，所需需要强制刷新一下
            RedPointDataManager.GetInstance().NotifyRedPointChanged();
        }

        /// <summary>
        /// 广播道具新旧状态发生变化
        /// </summary>
        void _NotifyItemNewStateChanged()
        {
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ItemNewStateChanged);

            // 道具新旧状态变化，背包红点需要发生变化，所需需要强制刷新一下
            RedPointDataManager.GetInstance().NotifyRedPointChanged();
        }

        /// <summary>
        /// 添加道具的网络消息处理函数
        /// </summary>
        /// <param name="msg"></param>
        void _OnAddItem(MsgDATA msg)
        {
            int pos = 0;
            List<Item> items = ItemDecoder.Decode(msg.bytes, ref pos, msg.bytes.Length);
            byte isInit = 0;
            if (pos < msg.bytes.Length)
            {
                BaseDLL.decode_int8(msg.bytes, ref pos, ref isInit);
            }
            
            for (int i = 0; i < items.Count; ++i)
            {
                ItemData item = CreateItemDataFromNet(items[i]);
                if (item != null)
                {
                    //item.IsNew = true;
                    item.IsNew = (isInit == 0);

                    _AddItem(item);
                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnlyUpdateItemList);

                    _NotifyItemCountChanged((int)item.TableID);

                    if (item.DeadTimestamp > 0) // 限时道具剩余时间少于24小时后加入到限时道具列表界面中显示
                    {
                        uint serverTime = TimeManager.GetInstance().GetServerTime();
                        int leftTime = item.DeadTimestamp - (int)serverTime;
                        if (leftTime <= 24 * 60 * 60)
                        {                        
                            m_arrTimeLessItems.Add(item);
                            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.TimeLessItemsChanged);

                            DeadLineReminderModel model = new DeadLineReminderModel()
                            {
                                type = DeadLineReminderType.DRT_LIMITTIMEITEM,
                                itemData = item
                            };

                            if (DeadLineReminderDataManager.GetInstance() != null)
                            {
                                DeadLineReminderDataManager.GetInstance().Add(model);
                            }

                            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.DeadLineReminderChanged);
                        }
                    }

                    UIEventSystem.GetInstance()
                        .SendUIEvent(EUIEventID.OnItemInPackageAddedMessage, item.GUID, item.TableID);
                }
                else
                {
                    Logger.LogErrorFormat("item data tableid = {0} cannot find in itemtable!", items[i].dataid);
                }
            }

            //Logger.LogError("吃鸡时序测试---_addItem()");

            if (isInit != 0)
            {
                EquipSuitDataManager.GetInstance().InitSelfEquipSuits();
                EquipHandbookDataManager.GetInstance().InitSelfEquipData();
                EquipUpgradeDataManager.GetInstance().InitEquipUpgradeTable();
            }
            NotifyPackageFullState();

            _OnAddItem(items);
        }

        /// <summary>
        /// 删除道具的网络消息的处理函数
        /// </summary>
        /// <param name="msg"></param>
        void _OnRemoveItem(MsgDATA msg)
        {
            SceneNotifyDeleteItem msgDeleteItem = new SceneNotifyDeleteItem();
            msgDeleteItem.decode(msg.bytes);
            
            ItemData data = GetItem(msgDeleteItem.uid);

            _RemoveItem(msgDeleteItem.uid);
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnlyUpdateItemList);

            if (data == null) return;
            if (onRemoveItem != null && data != null)
            {
                onRemoveItem(data);
            }

            m_akNeedPopEquips = OnFilter(m_akNeedPopEquips, false);
            _RemoveLowScoreEquips(m_akNeedPopEquips);
            if (onNeedPopEquipsChanged != null)
            {
                onNeedPopEquipsChanged(m_akNeedPopEquips);
            }

            if (data.PackageType == EPackageType.WearEquip || data.PackageType == EPackageType.WearFashion)
            {
                EquipSuitDataManager.GetInstance().UpdateSelfEquipSuits(data, false);
            }
            if(data.PackageType == EPackageType.EquipRecovery)
            {
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.EquipRecivertDeleteItem);
            }

            NotifyPackageFullState();
            _NotifyItemCountChanged((int)data.TableID);

            if (DeadLineReminderDataManager.GetInstance() != null)
            {
                DeadLineReminderDataManager.GetInstance().RemoveAll(data.GUID);
            }
            
            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.DeadLineReminderChanged);

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.OnItemInPackageRemovedMessage, msgDeleteItem.uid);
        }

        /// <summary>
        /// 更新道具属性的网路消息的处理函数
        /// </summary>
        /// <param name="msg"></param>
        void _OnUpdateItem(MsgDATA msg)
        {
            isReset = true;
            List<ItemData> arrItemDatas = new List<ItemData>();
            int pos = 0;
            List<Item> items = ItemDecoder.Decode(msg.bytes, ref pos, msg.bytes.Length, true);

            //道具的背包属性是否更改
            bool isItemPropertyPackUpdate = false;

            for (int i = 0; i < items.Count; ++i)
            {
                Item msgItemData = items[i];
                ItemData itemData = GetItem(msgItemData.uid);

                if (itemData != null)
                {
                    ClientSystemGameBattle sysChiji = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemGameBattle;
                    if (sysChiji != null)
                    {
                        msgItemData.tableID = (uint)itemData.TableID;
                    }

                    arrItemDatas.Add(itemData);
                    for (int j = 0; j < msgItemData.dirtyFields.Count; ++j)
                    {
                        EItemProperty prop = (EItemProperty)(msgItemData.dirtyFields[j]);
                        switch (prop)
                        {
                            case EItemProperty.EP_NUM:
                                {
                                    bool bIsAdd = false;
                                    if (itemData.PackageType != EPackageType.Storage && itemData.Type != EItemType.INCOME && itemData.PackageType != EPackageType.RoleStorage)
                                    {
                                        int iNum = 0;
                                        if (m_ItemNumDict.TryGetValue(itemData.TableID, out iNum))
                                        {
                                            if (msgItemData.num > itemData.Count)
                                            {
                                                m_ItemNumDict[itemData.TableID] += (msgItemData.num - itemData.Count);
                                                bIsAdd = true;
                                            }
                                            else
                                            {
                                                m_ItemNumDict[itemData.TableID] -= (itemData.Count - msgItemData.num);
                                            }
                                        }
                                        else
                                        {
                                            m_ItemNumDict.Add(itemData.TableID, msgItemData.num);
                                        }
                                    }

                                    if (msgItemData.num > itemData.Count)
                                    {
                                        NotifyItemBeNew(itemData);
                                    }

                                    itemData.Count = msgItemData.num;

                                    _NotifyItemCountChanged((int)itemData.TableID);

                                    // 吃鸡模式特殊处理
                                    ClientSystemGameBattle systemTown = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemGameBattle;
                                    if (systemTown != null)
                                    {
                                        CitySceneTable scenedata = TableManager.GetInstance().GetTableItem<CitySceneTable>(systemTown.CurrentSceneID);
                                        if (scenedata != null)
                                        {
                                            if (scenedata.SceneType == CitySceneTable.eSceneType.BATTLE && scenedata.SceneSubType == CitySceneTable.eSceneSubType.Battle &&
                                                !ChijiDataManager.GetInstance().SwitchingChijiSceneToPrepare)
                                            {
                                                if (itemData.TableData.Type == EItemType.EXPENDABLE && itemData.TableData.SubType == EItemSubType.GiftPackage)
                                                {
                                                    // 吃鸡道具自动使用,并且对于可叠加道具来讲，只有在道具数量变多的时候才应该自动使用,变少的时候不应该发的
                                                    if(bIsAdd)
                                                    {
                                                        _AutoUseChijiItem(itemData);
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ItemPropertyChanged, itemData, prop);
                                    break;
                                }
							case EItemProperty.EP_TRANSFER_STONE:
                                {
                                    itemData.TransferStone = (int)msgItemData.transferStone;
                                }
                                break;
                            case EItemProperty.EP_RECO_SCORE:
                                {
                                    itemData.RecoScore = (int)msgItemData.recoScore;
                                    break;
                                }
                            case EItemProperty.EP_LOCK_ITEM:
                                {
                                    itemData.bLocked = (msgItemData.lockItem & (int)ItemLockType.ILT_ITEM_LOCK) == (int)ItemLockType.ILT_ITEM_LOCK;
                                    itemData.bFashionItemLocked = (msgItemData.lockItem & (int)ItemLockType.ILT_FASHION_LOCK) == (int)ItemLockType.ILT_FASHION_LOCK;
                                    break;
                                }
                            case EItemProperty.EP_PACK:
                            {
                                    isItemPropertyPackUpdate = true;            //包裹属性发生改变
                                    EPackageType oldPackageType = itemData.PackageType;
                                    //只删除数据，不更新UI
                                    _RemoveItem(itemData.GUID);
                                    itemData.PackageType = (EPackageType)(msgItemData.pack);

                                    // 其实该道具在概念上是属于背包的，但是用先remove再add的流程去处理的时候，在_AddItem()函数内部再去判断背包内有没有该道具就会失效
                                    //只添加数据，不更新UI
                                    _AddItem(itemData, false);

                                    if (itemData.PackageType == EPackageType.WearEquip || itemData.PackageType == EPackageType.WearFashion)
                                    {
                                        EquipSuitDataManager.GetInstance().UpdateSelfEquipSuits(itemData, true);
                                    }
                                    else if((itemData.PackageType == EPackageType.Equip || itemData.PackageType == EPackageType.Fashion) && (oldPackageType == EPackageType.WearEquip || oldPackageType == EPackageType.WearFashion))
                                    {
                                        EquipSuitDataManager.GetInstance().UpdateSelfEquipSuits(itemData, false);
                                    }

                                    //道具的背包属性发生改变
                                    itemData.IsItemInUnUsedEquipPlan = EquipPlanUtility.IsItemInUnUsedEquipPlanByItemData(itemData);

                                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ItemCountChanged,(int)itemData.TableID);

                                    break;
                                }
                            case EItemProperty.EP_GRID:
                                {   
                                    itemData.GridIndex = (int) msgItemData.grid;

                                    if (itemData.PackageType == EPackageType.WearEquip)
                                    {
                                        if (itemData.SubType == (int)ItemTable.eSubType.ST_BXY_EQUIP)
                                        {
                                            itemData.EquipWearSlotType = EEquipWearSlotType.Equipcloak;
                                        }
                                        else
                                        {
                                            //特殊处理，因为臂章类型是99，穿戴臂章服务器返回过来的格子数是11，跟类型不匹配，暂时写死手动加上88
                                            if (msgItemData.grid >= 11)
                                            {
                                                itemData.EquipWearSlotType = (EEquipWearSlotType)(msgItemData.grid + 88);
                                            }
                                            else
                                            {
                                                itemData.EquipWearSlotType = (EEquipWearSlotType)(msgItemData.grid + 1);
                                            }
                                        }
                                    }
                                    break;
                                }
                            case EItemProperty.EP_PHY_ATK:
                                {
                                    itemData.BaseProp.props[(int)EEquipProp.PhysicsAttack] = (int)msgItemData.phyatk;
                                    break;
                                }
                            case EItemProperty.EP_MAG_ATK:
                                {
                                    itemData.BaseProp.props[(int)EEquipProp.MagicAttack] = (int)msgItemData.magatk;
                                    break;
                                }
                            case EItemProperty.EP_PHY_DEF:
                                {
                                    itemData.BaseProp.props[(int)EEquipProp.PhysicsDefense] = (int)msgItemData.phydef;
                                    break;
                                }
                            case EItemProperty.EP_MAG_DEF:
                                {
                                    itemData.BaseProp.props[(int)EEquipProp.MagicDefense] = (int)msgItemData.magdef;
                                    break;
                                }
                            case EItemProperty.EP_STR:
                                {
                                    // by ckm
                                    if (itemData.SubType != (int)ItemTable.eSubType.ST_BXY_EQUIP)
                                    {
                                        itemData.BaseProp.props[(int)EEquipProp.Strenth] = (int)msgItemData.strenth;
                                    }
                                    else
                                    {
                                        AttachBuff2Bxy(itemData, (int)msgItemData.strenth);
                                    }
                                    break;
                                }
                            case EItemProperty.EP_STAMINA:
                                {
                                    if (itemData.SubType != (int)ItemTable.eSubType.ST_BXY_EQUIP)
                                    {
                                        itemData.BaseProp.props[(int)EEquipProp.Stamina] = (int)msgItemData.stamina;
                                    }
                                    else
                                    {
                                        AttachBuff2Bxy(itemData, (int)msgItemData.stamina);
                                    }
                                    break;
                                }
                            case EItemProperty.EP_INTELLECT:
                                {
                                    if (itemData.SubType != (int)ItemTable.eSubType.ST_BXY_EQUIP)
                                    {
                                        itemData.BaseProp.props[(int)EEquipProp.Intellect] = (int)msgItemData.intellect;
                                    }
                                    else
                                    {
                                        AttachBuff2Bxy(itemData, (int)msgItemData.intellect);
                                    }
                                    break;
                                }
                            case EItemProperty.EP_SPIRIT:
                                {
                                    if (itemData.SubType != (int)ItemTable.eSubType.ST_BXY_EQUIP)
                                    {
                                        itemData.BaseProp.props[(int)EEquipProp.Spirit] = (int)msgItemData.spirit;
                                    }
                                    else
                                    {
                                        AttachBuff2Bxy(itemData, (int)msgItemData.spirit);
                                    }
                                    break;
                                }
                            case EItemProperty.EP_QUALITYLV:
                                {
                                    itemData.SubQuality = (int)msgItemData.qualitylv;
                                  
                                    break;
                                }
                            case EItemProperty.EP_STRENGTHEN:
                                {
                                    itemData.StrengthenLevel = msgItemData.strengthen;
                                    _OnStrengthenLevelChanged(itemData);

                                    //如果是附魔卡
                                    if (itemData.SubType == (int)ItemTable.eSubType.EnchantmentsCard)
                                    {
                                        itemData.mPrecEnchantmentCard = new PrecEnchantmentCard()
                                        {
                                            iEnchantmentCardID = itemData.TableID,
                                            iEnchantmentCardLevel = msgItemData.strengthen
                                        };
                                    }

                                    break;
                                }
                            case EItemProperty.EP_ADDMAGIC:
                                {
                                    if (msgItemData.mountedMagic != null)
                                    {
                                        itemData.mPrecEnchantmentCard = SwichPrecEnchantmentCard(msgItemData.mountedMagic);
                                    }
                                    break;
                                }
                            case EItemProperty.EP_ADDBEAD:
                                {
                                    itemData.PreciousBeadMountHole = SwitchPrecBead(msgItemData.preciousBeadHoles);
                                    break;
                                }
                            case EItemProperty.EP_INSCRIPTION_HOLES:
                                {
                                    itemData.InscriptionHoles = SwichInscriptionHoleData(msgItemData.inscriptionHoles,itemData);
                                    break;
                                }
                            case EItemProperty.EP_SEAL_STATE:
                                {
                                    itemData.Packing = msgItemData.sealstate == 1 ? true : false;
                                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ItemPropertyChanged, itemData, prop);
                                    break;
                                }
                            case EItemProperty.EP_SEAL_COUNT:
                                {
                                    itemData.iPackedTimes = (int)msgItemData.sealcount;
                                    itemData.RePackTime = itemData.iMaxPackTime - itemData.iPackedTimes;
                                    break;
                                }
                            case EItemProperty.EP_DIS_PHYATK:
                                {
                                    itemData.BaseProp.props[(int)EEquipProp.IgnorePhysicsAttack] = (int)msgItemData.disPhyAtk;
                                    break;
                                }
                            case EItemProperty.EP_DIS_MAGATK:
                                {
                                    itemData.BaseProp.props[(int)EEquipProp.IgnoreMagicAttack] = (int)msgItemData.disMagAtk;
                                    break;
                                }
                            case EItemProperty.EP_INDEPENDATK_STRENG:
                                //强化（激化）固定攻击TODO
                                {
                                    itemData.BaseProp.props[(int)EEquipProp.IngoreIndependence] = (int)msgItemData.independAtkStreng;
                                    break;
                                }
                            case EItemProperty.EP_DIS_PHYDEF:
                                {
                                    itemData.BaseProp.props[(int)EEquipProp.IgnorePhysicsDefense] = (int)msgItemData.disPhyDef;
                                    break;
                                }
                            case EItemProperty.EP_DIS_MAGDEF:
                                {
                                    itemData.BaseProp.props[(int)EEquipProp.IgnoreMagicDefense] = (int)msgItemData.disMagDef;
                                    break;
                                }
                            case EItemProperty.EP_PHYDEF_PERCENT:
                                {
                                    itemData.BaseProp.props[(int)EEquipProp.IgnorePhysicsDefenseRate] = (int)msgItemData.disPhyDefRate;
                                    break;
                                }
                            case EItemProperty.EP_MAGDEF_PERCENT:
                                {
                                    itemData.BaseProp.props[(int)EEquipProp.IgnoreMagicDefenseRate] = (int)msgItemData.disMagDefRate;
                                    break;
                                }
                            case EItemProperty.EP_PARAM1:
                                {
                                    break;
                                }
                            case EItemProperty.EP_DAYUSENUM:
                                {
                                    //itemData.currentUseTime = (int)msgItemData.dayUseNum;
                                    break;
                                }
                            case EItemProperty.EP_IA_FASHION_ATTRID:
                                {
                                    if (itemData.Type == EItemType.FASHION)
                                    {
                                        // 时装属性重选要减去当前加成的值，再加上重选后的值，不能像之前那样在FashionAttributeID赋值的时候
                                        // 直接在set里把数值覆盖掉，因为现在时装有自己的基础属性加成,原先的做法会把时装的基础属性给抹去
                                        EquipProp CurEquipProp = EquipProp.CreateFromTable(itemData.FashionAttributeID);
                                        if (CurEquipProp != null)
                                        {
                                            if (itemData.BaseProp != null)
                                            {
                                                itemData.BaseProp -= CurEquipProp;
                                            }
                                        }

                                        EquipProp NewEquipProp = EquipProp.CreateFromTable((int)msgItemData.fashionAttributeID);
                                        if (NewEquipProp != null)
                                        {
                                            if (itemData.BaseProp != null)
                                            {
                                                itemData.BaseProp += NewEquipProp;
                                            }
                                        }
                                    }

                                    itemData.FashionAttributeID = (int)msgItemData.fashionAttributeID;
                                    break;
                                }
                            case EItemProperty.EP_FASHION_ATTR_SELNUM:
                                {
                                    itemData.FashionFreeTimes = (int)msgItemData.fashionFreeSelNum;
                                    break;
                                }
                            case EItemProperty.EP_DEADLINE:
                                {
                                    itemData.DeadTimestamp = (int)msgItemData.deadLine;

                                    if(/*itemData.TableData.Type == EItemType.MATERIAL && */itemData.DeadTimestamp > 0)
                                    {
                                        uint serverTime = TimeManager.GetInstance().GetServerTime();
                                        int leftTime = itemData.DeadTimestamp - (int)serverTime;
                                        if(leftTime <= 24 * 60 * 60)
                                        {
                                            m_arrTimeLessItems.RemoveAll(x =>
                                            {
                                                return x.GUID == itemData.GUID;
                                            });
                                            m_arrTimeLessItems.Add(itemData);
                                            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.TimeLessItemsChanged);

                                            if (DeadLineReminderDataManager.GetInstance() != null)
                                            {
                                                DeadLineReminderDataManager.GetInstance().RemoveAll(itemData.GUID);
                                            }

                                            DeadLineReminderModel model = new DeadLineReminderModel()
                                            {
                                                type = DeadLineReminderType.DRT_LIMITTIMEITEM,
                                                itemData = itemData
                                            };

                                            if (DeadLineReminderDataManager.GetInstance() != null)
                                            {
                                                DeadLineReminderDataManager.GetInstance().Add(model);
                                            }

                                            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.DeadLineReminderChanged);
                                        }
                                    }
                                    break;
                                }
                            case EItemProperty.EP_STRPROP_LIGHT:
                                {
                                    itemData.BaseProp.magicElementsAttack[(int)(MagicElementType.LIGHT)] = (int)msgItemData.strPropLight;
                                    break;
                                }
                            case EItemProperty.EP_STRPROP_FIRE:
                                {
                                    itemData.BaseProp.magicElementsAttack[(int)(MagicElementType.FIRE)] = (int)msgItemData.strPropFire;
                                    break;
                                }
                            case EItemProperty.EP_STRPROP_ICE:
                                {
                                    itemData.BaseProp.magicElementsAttack[(int)(MagicElementType.ICE)] = (int)msgItemData.strPropIce;
                                    break;
                                }
                            case EItemProperty.EP_STRPROP_DARK:
                                {
                                    itemData.BaseProp.magicElementsAttack[(int)(MagicElementType.DARK)] = (int)msgItemData.strPropDark;
                                    break;
                                }
                            case EItemProperty.EP_DEFPROP_LIGHT:
                                {
                                    itemData.BaseProp.magicElementsDefence[(int)(MagicElementType.LIGHT)] = (int)msgItemData.defPropLight;
                                    break;
                                }
                            case EItemProperty.EP_DEFPROP_FIRE:
                                {
                                    itemData.BaseProp.magicElementsDefence[(int)(MagicElementType.FIRE)] = (int)msgItemData.defPropFire;
                                    break;
                                }
                            case EItemProperty.EP_DEFPROP_ICE:
                                {
                                    itemData.BaseProp.magicElementsDefence[(int)(MagicElementType.ICE)] = (int)msgItemData.defPropIce;
                                    break;
                                }
                            case EItemProperty.EP_DEFPROP_DARK:
                                {
                                    itemData.BaseProp.magicElementsDefence[(int)(MagicElementType.DARK)] = (int)msgItemData.defPropDark;
                                    break;
                                }
                            case EItemProperty.EP_ABNORMAL_RESISTS_TOTAL:
                                {
                                    itemData.BaseProp.props[(int)EEquipProp.AbormalResist] = (int)msgItemData.abnormalResistsTotal;
                                    break;
                                }
                            case EItemProperty.EP_EAR_FLASH:
                                {
                                    itemData.BaseProp.abnormalResists[0] = (int)msgItemData.abnormalResistFlash;
                                    break;
                                }
                            case EItemProperty.EP_EAR_BLEEDING:
                                {
                                    itemData.BaseProp.abnormalResists[1] = (int)msgItemData.abnormalResistBleeding;
                                    break;
                                }
                            case EItemProperty.EP_EAR_BURN:
                                {
                                    itemData.BaseProp.abnormalResists[2] = (int)msgItemData.abnormalResistBurn;
                                    break;
                                }
                            case EItemProperty.EP_EAR_POISON:
                                {
                                    itemData.BaseProp.abnormalResists[3] = (int)msgItemData.abnormalResistPoison;
                                    break;
                                }
                            case EItemProperty.EP_EAR_BLIND:
                                {
                                    itemData.BaseProp.abnormalResists[4] = (int)msgItemData.abnormalResistBlind;
                                    break;
                                }
                            case EItemProperty.EP_EAR_STUN:
                                {
                                    itemData.BaseProp.abnormalResists[5] = (int)msgItemData.abnormalResistStun;
                                    break;
                                }
                            case EItemProperty.EP_EAR_STONE:
                                {
                                    itemData.BaseProp.abnormalResists[6] = (int)msgItemData.abnormalResistStone;
                                    break;
                                }
                            case EItemProperty.EP_EAR_FROZEN:
                                {
                                    itemData.BaseProp.abnormalResists[7] = (int)msgItemData.abnormalResistFrozen;
                                    break;
                                }
                            case EItemProperty.EP_EAR_SLEEP:
                                {
                                    itemData.BaseProp.abnormalResists[8] = (int)msgItemData.abnormalResistSleep;
                                    break;
                                }
                            case EItemProperty.EP_EAR_CONFUNSE:
                                {
                                    itemData.BaseProp.abnormalResists[9] = (int)msgItemData.abnormalResistConfunse;
                                    break;
                                }
                            case EItemProperty.EP_EAR_STRAIN:
                                {
                                    itemData.BaseProp.abnormalResists[10] = (int)msgItemData.abnormalResistStrain;
                                    break;
                                }
                            case EItemProperty.EP_EAR_SPEED_DOWN:
                                {
                                    itemData.BaseProp.abnormalResists[11] = (int)msgItemData.abnormalResistSpeedDown;
                                    break;
                                }
                            case EItemProperty.EP_EAR_CURSE:
                                {
                                    itemData.BaseProp.abnormalResists[12] = (int)msgItemData.abnormalResistCurse;
                                    break;
                                }
                            case EItemProperty.EP_PRECIOUSBEAD_HOLES:
                                {
                                    itemData.PreciousBeadMountHole = SwitchPrecBead(msgItemData.preciousBeadHoles);
                                    break;
                                }
                            case EItemProperty.EP_BEAD_EXTIRPE_CNT:
                                {
                                    itemData.BeadPickNumber = (int)msgItemData.beadExtipreCnt;
                                    break;
                                }
                            case EItemProperty.EP_BEAD_REPLACE_CNT:
                                {
                                    itemData.BeadReplaceNumber = (int)msgItemData.beadReplaceCnt;
                                    break;
                                }
                            case EItemProperty.EP_TABLE_ID:
                                {
                                    if (itemData.PackageType == EPackageType.WearEquip)
                                    {
                                        EquipSuitDataManager.GetInstance().UpdateSelfEquipSuits(itemData, false);
                                    }
                                    // by ckm
                                    ItemTable tableData1 = TableManager.GetInstance().GetTableItem<ItemTable>((int)msgItemData.tableID);
                                    itemData.TableData = tableData1;
                                    itemData.Name = itemData.TableData.Name;
                                    itemData.Price = itemData.TableData.Price;

                                    itemData.SuitID = itemData.TableData.SuitID;
                                    itemData.LevelLimit = itemData.TableData.NeedLevel;
                                    if (itemData.PackageType == EPackageType.WearEquip)
                                    {
                                        EquipSuitDataManager.GetInstance().UpdateSelfEquipSuits(itemData, true);
                                    }

                                    if (itemData.SubType == (int)ItemTable.eSubType.ST_BXY_EQUIP)
                                    {
                                        itemData.Quality = tableData1.Color;
                                    }

                                    ProtoTable.EquipAttrTable tableData = TableManager.GetInstance().GetTableItem<ProtoTable.EquipAttrTable>(itemData.TableData.EquipPropID);
                                    if (tableData != null)
                                    {
                                        itemData.BaseProp.TableData = tableData;
                                        itemData.BaseProp.props[(int)EEquipProp.PhysicsSkillMPChange] = new CrypticInt32(tableData.PhySkillMp);
                                        itemData.BaseProp.props[(int)EEquipProp.PhysicsSkillCDChange] = new CrypticInt32(tableData.PhySkillCd);
                                        itemData.BaseProp.props[(int)EEquipProp.MagicSkillMPChange] = new CrypticInt32(tableData.MagSkillMp);
                                        itemData.BaseProp.props[(int)EEquipProp.MagicSkillCDChange] = new CrypticInt32(tableData.MagSkillCd);
                                        itemData.BaseProp.props[(int)EEquipProp.HPMax] = new CrypticInt32(tableData.HPMax);
                                        itemData.BaseProp.props[(int)EEquipProp.MPMax] = new CrypticInt32(tableData.MPMax);
                                        itemData.BaseProp.props[(int)EEquipProp.HPRecover] = new CrypticInt32(tableData.HPRecover);
                                        itemData.BaseProp.props[(int)EEquipProp.MPRecover] = new CrypticInt32(tableData.MPRecover);
                                        itemData.BaseProp.props[(int)EEquipProp.AttackSpeedRate] = new CrypticInt32(tableData.AttackSpeedRate);
                                        itemData.BaseProp.props[(int)EEquipProp.FireSpeedRate] = new CrypticInt32(tableData.FireSpeedRate);
                                        itemData.BaseProp.props[(int)EEquipProp.MoveSpeedRate] = new CrypticInt32(tableData.MoveSpeedRate);
                                        itemData.BaseProp.props[(int)EEquipProp.HitRate] = new CrypticInt32(tableData.HitRate);
                                        itemData.BaseProp.props[(int)EEquipProp.AvoidRate] = new CrypticInt32(tableData.AvoidRate);
                                        itemData.BaseProp.props[(int)EEquipProp.PhysicCritRate] = new CrypticInt32(tableData.PhysicCrit);
                                        itemData.BaseProp.props[(int)EEquipProp.MagicCritRate] = new CrypticInt32(tableData.MagicCrit);
                                        itemData.BaseProp.props[(int)EEquipProp.Spasticity] = new CrypticInt32(tableData.Spasticity);
                                        itemData.BaseProp.props[(int)EEquipProp.Jump] = new CrypticInt32(tableData.Jump);
                                        itemData.BaseProp.props[(int)EEquipProp.TownMoveSpeedRate] = new CrypticInt32(tableData.TownMoveSpeedRate);
                                        itemData.BaseProp.props[(int)EEquipProp.resistMagic] = new CrypticInt32(tableData.ResistMagic);

                                        if (itemData.SubType != (int)ItemTable.eSubType.ST_BXY_EQUIP)
                                        {
                                            itemData.BaseProp.attachBuffIDs = new List<int>(tableData.AttachBuffInfoIDs);
                                            itemData.BaseProp.attachMechanismIDs = new List<int>(tableData.AttachMechanismIDs);
                                            itemData.BaseProp.attachPVPBuffIDs = new List<int>(tableData.PVPAttachBuffInfoIDs);
                                            itemData.BaseProp.attachPVPMechanismIDs = new List<int>(tableData.PVPAttachMechanismIDs);
                                        }
                                    }

                                    break;
                                }
                            case EItemProperty.EA_AUCTION_TRANS_NUM:
                                {
                                    itemData.ItemTradeNumber = (int)msgItemData.auctionTransNum;
                                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ItemPropertyChanged, itemData, prop);
                                }
                                break;
                            case EItemProperty.EP_IS_TREAS:
                                {
                                    itemData.IsTreasure = msgItemData.isTreas == 1;
                                    UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ItemPropertyChanged, itemData, prop);
                                }
                                break;
                            case EItemProperty.EP_EQUIP_TYPE:
                                {
                                    itemData.EquipType = (EEquipType)msgItemData.equipType;
                                }
                                break;
                            case EItemProperty.EP_ENHANCE_TYPE:
                                {
                                    itemData.GrowthAttrType = (EGrowthAttrType)msgItemData.enhanceType;
                                }
                                break;
                            case EItemProperty.EP_ENHANCE_NUM:
                                {
                                    itemData.GrowthAttrNum = (int)msgItemData.enhanceNum;
                                }
                                break;
                            case EItemProperty.EP_VALUE_SCORE:
                                {
                                    itemData.finalRateScore = (int)msgItemData.valueScore;
                                }
                                break;
                            case EItemProperty.EP_INDEPENDATK:
                                {
                                    itemData.BaseProp.props[(int)EEquipProp.Independence] = (int)msgItemData.independAtk;
                                }
                                break;
                            case EItemProperty.EP_SUBTYPE:
                                {
                                    itemData.SubType = (int)msgItemData.subtype;
                                }
                                break;
                            default:
                                {
                                    string name = prop.GetDescription();
                                    if (name == null)
                                    {
                                        name = " " + msgItemData.dirtyFields[j];
                                    }
                                    Logger.LogWarningFormat("Item prop  {0} In [_OnUpdateItem] do not disposed!!", name);
                                    break;
                                }
                        }
                    }
                }
            }

            //装备的Pack属性发生更新(可能同时存在多个道具的Pack属性发生改变，如装备方案，只发送一次事件
            //避免多次刷新)
            if (isItemPropertyPackUpdate == true)
            {
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ItemPropertyChanged, EItemProperty.EP_PACK);

                NotifyPackageFullState();
                //更新红点
                RedPointDataManager.GetInstance().NotifyRedPointChanged();
            }

            if (arrItemDatas.Count > 0)
            {
                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ItemsAttrChanged, arrItemDatas);
            }

            if (onUpdateItem != null)
            {
                onUpdateItem(items);
            }
            _OnUpdateItem(items);

            // 默认情况下都是可以计算的
            if(bCalResistMagicValue)
            {
                SyncMainPlayerBaseDataByUpdateItem(arrItemDatas);
            }
        }

        /// <summary>
        /// 服务器通知获得道具
        /// </summary>
        /// <param name="msg"></param>
        void _OnNotifyGetItem(MsgDATA msg)
        {
            // 出于等级飞升过程中获得道具都不再弹提示了
            if(PlayerBaseData.GetInstance().IsFlyUpState)
            {
                return;
            }

            try
            {
                int pos = 0;
                UInt32 itemSource = 0;
                byte notify = 0; //0:旧的, 1:新的   0是旧的展现形式  1是新的
                List<CustomDecoder.RewardItem> rewardItems = CustomDecoder.DecodeGetRewards(msg.bytes, ref pos, msg.bytes.Length, ref itemSource, ref notify);
                if (rewardItems == null)
                {
                    return;
                }

                Dictionary<int, object> dicts = TableManager.instance.GetTable<ItemNotifyGetTable>();

                if (dicts == null)
                {
                    Logger.LogError("TableManager.instance.GetTable<ItemNotifyGetTable>() failed");
                    return;
                }

                var iter = dicts.GetEnumerator();
                bool bFind = false;
                if (notify == 0)
                {
                    bFind = true;
                }
                else
                {
                    while (iter.MoveNext())
                    {
                        ItemNotifyGetTable table = iter.Current.Value as ItemNotifyGetTable;
                        if (table != null && table.ID == (int)itemSource)
                        {
                            bFind = true;
                            break;
                        }
                    }
                }

                if (!(ClientSystemManager.instance.CurrentSystem is ClientSystemBattle))
                {
                    bool itemCanUse = false;
                    for (int i = 0; i < rewardItems.Count; ++i)
                    {
                        var itemTableData = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>((int)rewardItems[i].ID);
                        if (itemTableData == null)
                        {
                            continue;
                        }

                        if (itemTableData.CanUse == ItemTable.eCanUse.UseOne || itemTableData.CanUse == ItemTable.eCanUse.UseTotal)
                        {
                            itemCanUse = true;
                        }

                        ClientSystemBattle system = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemBattle;
                        if (system == null)
                        {
                            if (!bFind)
                            {
                                ItemData itemData = ItemDataManager.CreateItemDataFromTable((int)rewardItems[i].ID);
                                if (itemData != null)
                                {
                                    itemData.Count = (int)rewardItems[i].Num;
                                    itemData.StrengthenLevel = rewardItems[i].strength;
                                    itemData.EquipType = (EEquipType)rewardItems[i].equipType;
                                ClientSystemGameBattle gamebattle = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemGameBattle;
                                if (gamebattle != null) // 吃鸡场景不弹界面
                                {
                                    if (itemData != null && itemData.ThirdType == ItemTable.eThirdType.ChijiGiftPackage)
                                    {
                                        continue;
                                    }

                                    string str = string.Format("{0} * {1}", itemTableData.Name, rewardItems[i].Num);
                                    SystemNotifyManager.SysNotifyFloatingEffect(str, CommonTipsDesc.eShowMode.SI_QUEUE, (int)rewardItems[i].ID);
                                }
                                else
                                {
                                    SystemNotifyManager.SysNotifyGetNewItemEffect(itemData);
                                }
                                }
                                else
                                {
                                    Logger.LogErrorFormat("CreateItemDataFromTable failed, id = {0}", rewardItems[i].ID);
                                }
                            }
                            else
                            {
                                string str = string.Format("{0} * {1}", itemTableData.Name, rewardItems[i].Num);
                                SystemNotifyManager.SysNotifyFloatingEffect(str, CommonTipsDesc.eShowMode.SI_QUEUE, (int)rewardItems[i].ID);
                            }
                        }
                        else
                        {
                            string str = string.Format("{0} * {1}", itemTableData.Name, rewardItems[i].Num);
                            SystemNotifyManager.SysNotifyFloatingEffect(str, CommonTipsDesc.eShowMode.SI_QUEUE, (int)rewardItems[i].ID);
                        }

                    }

                    if (itemCanUse)
                        TryOpenEquipmentChangedFrame();
                }

                UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ItemNotifyGet);
            }
            catch (System.Exception e)
            {
                Logger.LogError("_OnNotifyGetItem failed! Exception:" + e.ToString());
            }            
        }

        /// <summary>
        /// 服务器通知删除道具
        /// </summary>
        /// <param name="msg"></param>
        void _OnNotifyCostItem(MsgDATA msg)
        { 
            SceneNotifyCostItem ret = new SceneNotifyCostItem();
            ret.decode(msg.bytes);

            UIEventSystem.GetInstance().SendUIEvent(EUIEventID.ItemNotifyRemoved, ret.itemid, ret.num);
        }
        #endregion


        #region newEquipments hint  待整理
        /// <summary>
        /// 新装备功能提示
        /// </summary>
        /// 
        bool _TryRemoveFatigueDrug()
        {
            bool bNeedNotify = false;
            for (int i = 0; i < m_akNeedPopEquips.Count; ++i)
            {
                var itemData = GetItem(m_akNeedPopEquips[i]);
                if (itemData != null && itemData.SubType == (int)ProtoTable.ItemTable.eSubType.FatigueDrug)
                {
                    if (itemData.PackageType == EPackageType.Storage || itemData.PackageType == EPackageType.RoleStorage || itemData.GetCurrentRemainUseTime() <= 0 || PlayerBaseData.GetInstance().fatigue > m_iFatigueLimit)
                    {
                        m_akNeedPopEquips.RemoveAt(i--);
                        bNeedNotify = true;
                        continue;
                    }
                }
            }
            return bNeedNotify;
        }

        void _AutoUseChijiItem(ItemData item, bool isRealAddItemInLogic = true)
        {
            if (ChijiDataManager.GetInstance().SwitchingPrepareToChijiScene)
            {
                return;
            }

            if (!isRealAddItemInLogic)
            {
                return;
            }

            UseItem(item);
        }

        void _AutoEquipChijiEquipment(ItemData item, bool isRealAddItemInLogic = true)
        {
            if (ChijiDataManager.GetInstance().SwitchingPrepareToChijiScene)
            {
                return;
            }

            if(!isRealAddItemInLogic)
            {
                return;
            }

            bool AdaptSelfJob = IsItemJobAdaptToTargetJob(item.TableData, PlayerBaseData.GetInstance().JobTableID);

            if(!AdaptSelfJob)
            {
                return;
            }

            List<ulong> wearEquipIDs = GetItemsByPackageType(EPackageType.WearEquip);

            bool bFindWearEquip = false;

            for (int i = 0; i < wearEquipIDs.Count; ++i)
            {
                ItemData curItem = GetItem(wearEquipIDs[i]);

                if (curItem == null)
                {
                    continue;
                }

                if (curItem.EquipWearSlotType != item.EquipWearSlotType)
                {
                    continue;
                }

                bFindWearEquip = true;

                if (curItem.finalRateScore >= item.finalRateScore)
                {
                    break;
                }

                UseItem(item);

                break;
            }

            if(!bFindWearEquip)
            {
                UseItem(item);

                SystemNotifyManager.SysNotifyFloatingEffect(string.Format("已自动穿戴 {0}", item.GetColorName()));
            }
        }

        public bool IsItemJobAdaptToTargetJob(ItemTable ItemData, int TargetJobId)
        {
            if (ItemData == null || ItemData.Occu == null || ItemData.Occu.Length < 1)
            {
                return false;
            }

            // 装备的职业适配规则与技能的职业适配规则不一样,基础职业的装备，进阶职业也可以用，但技能不能行
            if (ItemData.Occu[0] == 0) // 适配所有职业
            {
                return true;
            }
            else if (ItemData.Occu[0] == -1)   // 仅适配进阶职业
            {
                JobTable TargetData = TableManager.GetInstance().GetTableItem<JobTable>(TargetJobId);
                if (TargetData == null)
                {
                    return false;
                }

                return TargetData.JobType > 0;
            }
            else // 适配具体职业
            {
                bool bIsBaseJob = false;

                JobTable FirstData = TableManager.GetInstance().GetTableItem<JobTable>(ItemData.Occu[0]);
                if (FirstData == null)
                {
                    return false;
                }

                if (FirstData.JobType == 0)
                {
                    bIsBaseJob = true;
                }

                // 如果只填基础职业
                if (bIsBaseJob)
                {
                    JobTable TargetData = TableManager.GetInstance().GetTableItem<JobTable>(TargetJobId);
                    if (TargetData == null)
                    {
                        return false;
                    }

                    if (TargetData.JobType == 0)
                    {
                        if (TargetData.ID == FirstData.ID)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        // 进阶职业也是可以用的
                        if (TargetData.prejob == FirstData.ID)
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < ItemData.Occu.Length; i++)
                    {
                        JobTable jobData = TableManager.GetInstance().GetTableItem<JobTable>(ItemData.Occu[i]);
                        if (jobData == null)
                        {
                            continue;
                        }

                        if (jobData.ID == TargetJobId)
                        {
                            return true;
                        }
                    }
                }
            }

            return true;
        }

        public object GetAddDrugUseTipFunc(ItemData item)
        {
            if(null == item)
            {
                return null;
            }

            var realItem = ItemDataManager.GetInstance().GetItem(item.GUID);
            if(null == realItem)
            {
                return null;
            }

            TipFuncButon tempFunc = null;
            if(item.SubType == (int)ProtoTable.ItemTable.eSubType.Hp ||
                item.SubType == (int)ProtoTable.ItemTable.eSubType.Mp ||
                item.SubType == (int)ProtoTable.ItemTable.eSubType.HpMp||
                item.SubType == (int)ProtoTable.ItemTable.eSubType.AttributeDrug)
            {
                tempFunc = new TipFuncButon();
                tempFunc.text = TR.Value("tip_drug_config");
                tempFunc.name = "drug_config";
                tempFunc.callback = _DrugConfig;
            }
            return tempFunc;
        }

        void _DrugConfig(ItemData item, object param1)
        {
            ClientSystemManager.instance.OpenFrame<ChapterBattlePotionSetFrame>();
            ItemTipManager.GetInstance().CloseAll();
        }

        bool ItemCanPopUp(ItemData itemData)
        {
            if(itemData == null)
            {
                return false;
            }

            if(itemData.TableData == null)
            {
                return false;
            }
            if(popUpConditions == null)
            {
                return false;
            }
            if(!popUpConditions.ContainsKey(itemData.TableData.Type))
            {
                return false;
            }
            PopUpCondition popUpCondition = popUpConditions[itemData.TableData.Type];
            if(popUpCondition == null)
            {
                return false;
            }
            if(popUpCondition.iMinPlayerLv > 0 && PlayerBaseData.GetInstance().Level < popUpCondition.iMinPlayerLv)
            {
                return false;
            }
            if(popUpCondition.iMaxPlayerLv > 0 && PlayerBaseData.GetInstance().Level > popUpCondition.iMaxPlayerLv)
            {
                return false;
            }
            if(popUpCondition.checkCallBack != null)
            {
                return popUpCondition.checkCallBack(itemData);
            }
            return true;
        }
        bool IsNeedAddToEquipChangedList(ulong id)
        {
            var itemData = GetItem(id);
            bool checkResult = ItemCanPopUp(itemData);            
            return itemData != null
                && !this.m_akNeedPopEquips.Contains(id) 
                && itemData.PackageType != EPackageType.Storage 
                && itemData.PackageType != EPackageType.RoleStorage
                && checkResult
                && itemData.PackageType!=EPackageType.EquipRecovery;
        }

        const int m_iFatigueLimit = 0;
        void _OnFatigueChanged(UIEvent uiEvent)
        {
            bool bNeedNotify = false;
            if (PlayerBaseData.GetInstance().fatigue <= m_iFatigueLimit)
            {
                var uids = GetItemsByPackageSubType(EPackageType.Consumable, EItemSubType.FatigueDrug);
                for(int i = 0;i < uids.Count; ++i)
                {
                    var itemData = GetItem(uids[i]);
                    if(itemData == null || itemData.PackageType == EPackageType.Storage || itemData.PackageType == EPackageType.RoleStorage || itemData.GetCurrentRemainUseTime() <= 0)
                    {
                        continue;
                    }

                    if(IsNeedAddToEquipChangedList(itemData.GUID))
                    {
                        m_akNeedPopEquips.Add(itemData.GUID);
                        bNeedNotify = true;
                    }
                }

                if(_TryRemoveFatigueDrug())
                {
                    bNeedNotify = true;
                }

                if (bNeedNotify)
                {
                    TryOpenEquipmentChangedFrame();
                }
            }
            else
            {
                if(_TryRemoveFatigueDrug())
                {
                    bNeedNotify = true;
                }
            }

            if (bNeedNotify)
            {
                if (onNeedPopEquipsChanged != null)
                {
                    onNeedPopEquipsChanged.Invoke(m_akNeedPopEquips);
                }
            }
        }

        void _OnCountValueChanged(UIEvent a_event)
        {
            _OnFatigueChanged(null);
        }

        void OnLevelChanged(int iPreLv, int iCurLv)
        {
            if(Utility.IsPlayerLevelFull(iCurLv))
            {
                if(_TryRemoveExperiencePill())
                {
                    if (onNeedPopEquipsChanged != null)
                    {
                        onNeedPopEquipsChanged.Invoke(m_akNeedPopEquips);
                    }
                }
            }
        }

        bool _TryRemoveExperiencePill()
        {
            bool bNeedNotify = false;
            for (int i = 0; i < m_akNeedPopEquips.Count; ++i)
            {
                var itemData = GetItem(m_akNeedPopEquips[i]);
                if(itemData == null)
                {
                    m_akNeedPopEquips.RemoveAt(i--);
                    bNeedNotify = true;
                    continue;
                }

                if (itemData.SubType == (int)ProtoTable.ItemTable.eSubType.ExperiencePill)
                {
                    m_akNeedPopEquips.RemoveAt(i--);
                    bNeedNotify = true;
                    continue;
                }
            }
            return bNeedNotify;
        }

        List<ulong> OnFilter(List<ulong> items, bool bNeedNew)
        {
            List<ulong> ret = new List<ulong>();
            ItemData outValue = null;
            for (int i = 0; i < items.Count; ++i)
            {
                if (m_itemsDict.TryGetValue(items[i], out outValue))
                {
                    if(outValue.PackageType == EPackageType.Storage || outValue.PackageType == EPackageType.RoleStorage)
                    {
                        continue;
                    }

                    if (outValue.isInSidePack)
                    {
                        continue;
                    }

                    var itemTable = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>((int)outValue.TableID);
                    if(itemTable == null)
                    {
                        continue;
                    }

                    //chenghao can only be unlocked one
                    //if(itemTable.Type == EItemType.FUCKTITTLE)
                    //{
                    //    if(TittleBookManager.GetInstance().HasBindedTitle(outValue.TableID))
                    //    {
                    //        continue;
                    //    }
                    //}

                    if (itemTable != null && (itemTable.Type == EItemType.EQUIP || itemTable.Type == EItemType.FUCKTITTLE ||
                        itemTable.Type == EItemType.FASHION) && (outValue.IsNew || !bNeedNew))
                    {
                        if (outValue.CheckBetterThanEquip() && outValue.PackageType != EPackageType.WearEquip)
                        {
                            ret.Add(items[i]);
                        }
                    }
                    else if (itemTable != null && (itemTable.SubType == ProtoTable.ItemTable.eSubType.GiftPackage))
                    {
                        if (outValue.CanGiftUse() && itemTable.EPrompt == ItemTable.eEPrompt.EPT_NEW_EQUIP)
                        {
                            ret.Add(items[i]);
                        }
                    }
                    else if (itemTable != null && (itemTable.EPrompt == ItemTable.eEPrompt.EPT_NEW_EQUIP)/* && (outValue.IsNew)*/)
                    {
                        if (outValue.IsLevelFit())
                        {
                            if(itemTable.SubType == EItemSubType.FatigueDrug)
                            {
                                if (PlayerBaseData.GetInstance().fatigue <= m_iFatigueLimit && outValue.GetCurrentRemainUseTime() > 0)
                                {
                                    ret.Add(items[i]);
                                }
                            }
                            else if(itemTable.SubType == EItemSubType.ExperiencePill)
                            {
                                //如果是经验丸 需求修改 判断当前等级大于需要等级并且小于最大等级才可加入到列表中 有问题@戚乐超
                                if(PlayerBaseData.GetInstance().Level >= itemTable.NeedLevel && PlayerBaseData.GetInstance().Level <= itemTable.MaxLevel)
                                {
                                    ret.Add(items[i]);
                                }
                            }
                            else
                            {
                                ret.Add(items[i]);
                            }
                        }
                    }
                    else if(itemTable.SubType == EItemSubType.PetEgg)
                    {
                        const int minPlayerLv = 16;
                        const int maxPlayerLv = 45;

                        // 宠物蛋推送对玩家等级还有额外的限制
                        if(PlayerBaseData.GetInstance().Level >= minPlayerLv && PlayerBaseData.GetInstance().Level <= maxPlayerLv)
                        {
                            int petID = Utility.GetPetID(outValue.TableID);
                            var petTable = TableManager.GetInstance().GetTableItem<PetTable>(petID);
                            if (petTable != null)
                            {
                                List<PetInfo> petlist = PetDataManager.GetInstance().GetOnUsePetList();
                                if (petlist != null)
                                {
                                    bool bFind = false;
                                    for (int j = 0; j < petlist.Count; j++)
                                    {
                                        PetTable petData = TableManager.GetInstance().GetTableItem<PetTable>((int)petlist[j].dataId);
                                        if (petData != null && petData.PetType == petTable.PetType)
                                        {
                                            bFind = true;
                                            if (petTable.Quality > petData.Quality)
                                            {
                                                ret.Add(items[i]);
                                            }
                                            break;
                                        }
                                    }
                                    if (!bFind)
                                    {
                                        ret.Add(items[i]);
                                    }
                                }
                            }
                        }                      
                    }
                }
            }
            ret.Sort();
            return ret;
        }

        void _OnLevelUpAddNewPopEquipments(int iPreLv)
        {
            var enumerator = m_itemsDict.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var current = enumerator.Current.Value;
                if (current.CheckBetterThanEquip() && !current.IsPreLevelFit(iPreLv))
                {
                    if (IsNeedAddToEquipChangedList(current.GUID))
                    {
                        m_akNeedPopEquips.Add(current.GUID);
                    }
                }
                else if (current.SubType == (int)ProtoTable.ItemTable.eSubType.GiftPackage)
                {
                    if (current.CanGiftUse() && !current.IsPreLevelFit(iPreLv))
                    {
                        if (IsNeedAddToEquipChangedList(current.GUID))
                        {
                            m_akNeedPopEquips.Add(current.GUID);
                        }
                    }
                }
            }
            _RemoveLowScoreEquips(m_akNeedPopEquips);
            if(onNeedPopEquipsChanged != null)
            {
                onNeedPopEquipsChanged(m_akNeedPopEquips);
            }
            TryOpenEquipmentChangedFrame();
        }

        void _OnFatigueChanged()
        {

        }

        void _OnChangeJobAddNewPopEquipments()
        {
            var enumerator = m_itemsDict.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var current = enumerator.Current.Value;
                if (current.CheckBetterThanEquip())
                {
                    if (IsNeedAddToEquipChangedList(current.GUID))
                    {
                        m_akNeedPopEquips.Add(current.GUID);
                    }
                }
                else if (current.SubType == (int)ProtoTable.ItemTable.eSubType.GiftPackage)
                {
                    if (current.CanGiftUse())
                    {
                        if (IsNeedAddToEquipChangedList(current.GUID))
                        {
                            m_akNeedPopEquips.Add(current.GUID);
                        }
                    }
                }
            }
            _RemoveLowScoreEquips(m_akNeedPopEquips);
            if (onNeedPopEquipsChanged != null)
            {
                onNeedPopEquipsChanged(m_akNeedPopEquips);
            }
            TryOpenEquipmentChangedFrame();
        }

        List<ulong> m_akNeedPopEquips = new List<ulong>();
        public delegate void OnNeedPopEquipsChanged(List<ulong> equipts);
        public OnNeedPopEquipsChanged onNeedPopEquipsChanged;
        Dictionary<EEquipWearSlotType, ItemData> m_akSlotMap = new Dictionary<EEquipWearSlotType, ItemData>();
        void _RemoveLowScoreEquips(List<ulong> akNeedPopEquips)
        {
            m_akSlotMap.Clear();
            //first to remove unexist equipments
            for (int i = 0; i < akNeedPopEquips.Count; ++i)
            {
                var itemData = GetItem(akNeedPopEquips[i]);
                if(itemData == null)
                {
                    akNeedPopEquips.RemoveAt(i--);
                    continue;
                }

                if(itemData.PackageType == EPackageType.Equip ||
                    itemData.PackageType == EPackageType.Title ||
                    itemData.PackageType == EPackageType.Bxy)
                {
                    if(!m_akSlotMap.ContainsKey(itemData.EquipWearSlotType))
                    {
                        m_akSlotMap.Add(itemData.EquipWearSlotType,itemData);
                    }
                    else if(m_akSlotMap[itemData.EquipWearSlotType].finalRateScore < itemData.finalRateScore)
                    {
                        m_akSlotMap[itemData.EquipWearSlotType] = itemData;
                    }
                    akNeedPopEquips.RemoveAt(i--);
                    continue;
                }
            }
            //加入映射装备
            if(m_akSlotMap.Count > 0)
            {
                var values = m_akSlotMap.Values.ToList();
                for(int i = 0; i < values.Count; ++i)
                {
                    akNeedPopEquips.Add(values[i].GUID);
                }
            }
        }
        // TODO 干掉
        public void _OnAddItem(List<Item> items)
        {
            List<ulong> itemLists = new List<ulong>();
            itemLists.Clear();
            for (int i = 0; i < items.Count; ++i)
            {
                itemLists.Add(items[i].uid);
            }

            var currentLists = OnFilter(itemLists, true);
            if (currentLists.Count > 0)
            {
                for(int i = 0;i < currentLists.Count;i++)
                {
                    if(IsNeedAddToEquipChangedList(currentLists[i]))
                    {
                        m_akNeedPopEquips.Add(currentLists[i]);
                    }
                }
                //m_akNeedPopEquips.AddRange(currentLists.AsEnumerable());
                _RemoveLowScoreEquips(m_akNeedPopEquips);
                if (onNeedPopEquipsChanged != null)
                {
                    onNeedPopEquipsChanged(m_akNeedPopEquips);
                }
            }

            //TryOpenEquipmentChangedFrame();

            if (onAddNewItem != null)
            {
                onAddNewItem(items);
            }
        }

        private bool isReset = true;
        private void AttachBuff2Bxy(ItemData itemData,int buffID)
        {
            if (isReset)
            {
                itemData.BaseProp.attachBuffIDs.Clear();
                itemData.BaseProp.attachPVPBuffIDs.Clear();
                isReset = false;
            }
            itemData.AddAttachBxyBuffIID(itemData.BaseProp, 0, buffID);
        }

        void _OnUpdateItem(List<Item> items)
        {
            ItemData outValue = null;
            bool bNeedRefresh = false;
            List<ulong> addItems = new List<ulong>();
            for (int i = 0; i < items.Count; ++i)
            {
                if (m_itemsDict.TryGetValue(items[i].uid, out outValue))
                {
                    var itemTable = TableManager.GetInstance().GetTableItem<ProtoTable.ItemTable>((int)outValue.TableID);
                    if (itemTable != null && (itemTable.Type == EItemType.FASHION || itemTable.Type == EItemType.EQUIP || itemTable.Type == EItemType.FUCKTITTLE) ||
                        itemTable != null && itemTable.EPrompt == ItemTable.eEPrompt.EPT_NEW_EQUIP)
                    {
                        bNeedRefresh = true;
                        break;
                    }
                }
            }

            if (bNeedRefresh)
            {
                m_akNeedPopEquips = OnFilter(m_akNeedPopEquips, false);
                _RemoveLowScoreEquips(m_akNeedPopEquips);
                if (onNeedPopEquipsChanged != null)
                {
                    onNeedPopEquipsChanged(m_akNeedPopEquips);
                }
            }
        }

        public List<ulong> NeedEquiptmentsID
        {
            get { return m_akNeedPopEquips; }
            set { NeedEquiptmentsID = value; }
        }

        public void AddSystemInvoke()
        {
            ClientSystemManager.GetInstance().OnSwitchSystemFinished.RemoveListener(OnSceneLoadFinish);

            ClientSystemManager.GetInstance().OnSwitchSystemFinished.AddListener(OnSceneLoadFinish);
        }

        void OnSceneLoadFinish()
        {
            TryOpenEquipmentChangedFrame();
        }

        delegate bool CheckItemPopUp(ItemData itemData);
        class PopUpCondition
        {
            public int iMinPlayerLv = 0;
            public int iMaxPlayerLv = 0;
            public CheckItemPopUp checkCallBack = null;
        }
        Dictionary<ItemTable.eType, PopUpCondition> popUpConditions = new Dictionary<EItemType, PopUpCondition>();
        void TryOpenEquipmentChangedFrame()
        {
            if (ClientSystem.IsTargetSystem<ClientSystemTown>())
            {
                if (m_akNeedPopEquips.Count > 0)
                {
                    if (/*PlayerBaseData.GetInstance().Level > 4 && */!ClientSystemManager.GetInstance().IsFrameOpen<EquipmentChangedFrame>())
                    {
                        ClientSystemTown systemTown = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
                        if (systemTown == null)
                            return;

                        CitySceneTable sceneData = TableManager.GetInstance().GetTableItem<CitySceneTable>(systemTown.CurrentSceneID);
                        if (sceneData == null)
                            return;

                        ClientSystemManager.GetInstance().OpenFrame<EquipmentChangedFrame>(FrameLayer.Bottom);
                    }
                }
            }
        }
        #endregion

        #region  //从装备评分表中读取数据刷新全局配置里面的数据

        public void _InitEquipScore()
        {
            _InitEquipBaseScoreModTable();
            _InitEquipIdDic();
            _RefreshGlobalByTable();
            var dic = Global.Settings.equipPropFactors;
            for (int i = 0; i < Global.equipPropName.Count; ++i)
            {
                var key = Global.equipPropName[i];
                if (dic.ContainsKey(key))
                {
                    dic[key] = Global.Settings.equipPropFactorValues[i];
                }
                else
                {
                    dic.Add(key, Global.Settings.equipPropFactorValues[i]);
                }
            }
            dic = Global.Settings.quipThirdTypeFactors;
            for (int i = 0; i < Global.equipThirdTypeNamesList.Count; ++i)
            {
                var key = Global.equipThirdTypeNamesList[i];
                if (dic.ContainsKey(key))
                {
                    dic[key] = Global.Settings.quipThirdTypeFactorValues[i];
                }
                else
                {
                    dic.Add(key, Global.Settings.quipThirdTypeFactorValues[i]);
                }
            }
        }

        protected Dictionary<EquipScoreTable.eType, int> m_EquipScoreValueDic = new Dictionary<EquipScoreTable.eType, int>();
        protected void _InitEquipIdDic()
        {
            var equipScoreTable = TableManager.GetInstance().GetTable<EquipScoreTable>();
            if (equipScoreTable == null)
                return;
            EquipScoreTable.eType type = 0;
            for (int i = 1; i <= equipScoreTable.Count; i++)
            {
                var data = TableManager.instance.GetTableItem<EquipScoreTable>(i);
                if (!m_EquipScoreValueDic.ContainsKey(data.Type))
                {
                    m_EquipScoreValueDic.Add(data.Type, data.Value);
                }
                else
                {
                    m_EquipScoreValueDic[type] = data.Value;
                }
            }
        }

        void _RefreshGlobalByTable()
        {
            //属性分值刷新
            int value = -1;
            for (int i = 0; i < Global.Settings.equipPropFactorValues.Length; i++)
            {
                switch (i)
                {
                    case 0:
                        value = _GetDataByType(EquipScoreTable.eType.HP);
                        break;
                    case 1:
                        value = _GetDataByType(EquipScoreTable.eType.MP);
                        break;
                    case 2:
                        value = _GetDataByType(EquipScoreTable.eType.HPRECVR);
                        break;
                    case 3:
                        value = _GetDataByType(EquipScoreTable.eType.MPRECVR);
                        break;
                    case 4:
                        value = _GetDataByType(EquipScoreTable.eType.PHYATK);
                        break;
                    case 5:
                        value = _GetDataByType(EquipScoreTable.eType.MAGATK);
                        break;
                    case 6:
                        value = _GetDataByType(EquipScoreTable.eType.PHYDEF);
                        break;
                    case 7:
                        value = _GetDataByType(EquipScoreTable.eType.MAGDEF);
                        break;
                    case 8:
                        value = _GetDataByType(EquipScoreTable.eType.ATKSPD);
                        break;
                    case 9:
                        value = _GetDataByType(EquipScoreTable.eType.MAGSPD);
                        break;
                    case 10:
                        value = _GetDataByType(EquipScoreTable.eType.MVSPD);
                        break;
                    case 11:
                        value = _GetDataByType(EquipScoreTable.eType.PHYCRT);
                        break;
                    case 12:
                        value = _GetDataByType(EquipScoreTable.eType.MAGCRT);
                        break;
                    case 13:
                        value = _GetDataByType(EquipScoreTable.eType.HIT);
                        break;
                    case 14:
                        value = _GetDataByType(EquipScoreTable.eType.MISS);
                        break;
                    case 15:
                        value = _GetDataByType(EquipScoreTable.eType.JZ);
                        break;
                    case 16:
                        value = _GetDataByType(EquipScoreTable.eType.YZ);
                        break;
                    case 17:
                        value = _GetDataByType(EquipScoreTable.eType.STR);
                        break;
                    case 18:
                        value = _GetDataByType(EquipScoreTable.eType.INT);
                        break;
                    case 19:
                        value = _GetDataByType(EquipScoreTable.eType.STAM);
                        break;
                    case 20:
                        value = _GetDataByType(EquipScoreTable.eType.SPR);
                        break;
                    case 21:
                        value = _GetDataByType(EquipScoreTable.eType.PHYDMGRDC);
                        break;
                    case 22:
                        value = _GetDataByType(EquipScoreTable.eType.MAGDMGRDC);
                        break;
                    case 23:
                        value = _GetDataByType(EquipScoreTable.eType.DISPHYATK);
                        break;
                    case 24:
                        value = _GetDataByType(EquipScoreTable.eType.DISMAGATK);
                        break;
                }
                if (value != -1)
                {
                    Global.Settings.equipPropFactorValues[i] = value / 1000.0f;
                }
            }

            //武器评分系数
            //与名字相对应
            int value2 = -1;
            for (int j = 0; j < Global.Settings.quipThirdTypeFactorValues.Length; j++)
            {
                switch (j)
                {
                    case 0:
                        value2 = _GetDataByType(EquipScoreTable.eType.HSWORD);
                        break;
                    case 1:
                        value2 = _GetDataByType(EquipScoreTable.eType.TD);
                        break;
                    case 2:
                        value2 = _GetDataByType(EquipScoreTable.eType.DJ);
                        break;
                    case 3:
                        value2 = _GetDataByType(EquipScoreTable.eType.GUANGJIAN);
                        break;
                    case 4:
                        //value2 = _GetDataByType(EquipScoreTable.eType.PHYATK);
                        break;
                    case 5:
                        value2 = _GetDataByType(EquipScoreTable.eType.ZL);
                        break;
                    case 6:
                        value2 = _GetDataByType(EquipScoreTable.eType.NUJIAN);
                        break;
                    case 7:
                        value2 = _GetDataByType(EquipScoreTable.eType.SP);
                        break;
                    case 8:
                        value2 = _GetDataByType(EquipScoreTable.eType.BUQIANG);
                        break;
                    case 9:
                        //value2 = _GetDataByType(EquipScoreTable.eType.MAGSPD);
                        break;
                    case 10:
                        value2 = _GetDataByType(EquipScoreTable.eType.FZ);
                        break;
                    case 11:
                        value2 = _GetDataByType(EquipScoreTable.eType.MZ);
                        break;
                    case 12:
                        value2 = _GetDataByType(EquipScoreTable.eType.SPEAR);
                        break;
                    case 13:
                        value2 = _GetDataByType(EquipScoreTable.eType.STICK);
                        break;
                    case 14:
                        value2 = _GetDataByType(EquipScoreTable.eType.BJ);
                        break;
                    case 15:
                        value2 = _GetDataByType(EquipScoreTable.eType.PJ);
                        break;
                    case 16:
                        value2 = _GetDataByType(EquipScoreTable.eType.QJ);
                        break;
                    case 17:
                        value2 = _GetDataByType(EquipScoreTable.eType.ZJ);
                        break;
                    case 18:
                        value2 = _GetDataByType(EquipScoreTable.eType.BA);
                        break;
                }
                if (value2!=-1)
                {
                    Global.Settings.quipThirdTypeFactorValues[j] = value2/1000.0f;
                }
            }
        }

        //根据类型获取Id 
        public int _GetDataByType(EquipScoreTable.eType type)
        {
            if (m_EquipScoreValueDic.ContainsKey(type))
            {
                return m_EquipScoreValueDic[type];
            }
            else
            {
                return -1;
            }
        }
        #endregion

        #region ResistMagicValue
        
        private bool IsItemBelongSuit(int itemId)
        {
            var itemTable = TableManager.GetInstance().GetTableItem<ItemTable>(itemId);
            if (itemTable == null)
                return false;

            if (itemTable.SuitID <= 0)
                return false;

            return true;
        }

        //单独一个装备的抗魔值
        public int GetItemResistMagicValue(int itemId)
        {
            //return 10;

            var itemTable = TableManager.GetInstance().GetTableItem<ItemTable>(itemId);
            if (itemTable == null)
                return 0;

            var equipPropId = itemTable.EquipPropID;
            var equipAttrItem = TableManager.GetInstance().GetTableItem<EquipAttrTable>(equipPropId);
            if (equipAttrItem == null)
                return 0;

            return equipAttrItem.ResistMagic;
        }

        //同步玩家的信息，当用户穿或者脱装备和title的时候
        // 这个接口在装备属性发生变化的时候会调用，用来计算ResistMagicValue，然后客户端同步给服务器
        // 该功能本身没有什么问题，没有考虑到在一些特定情形下会被频繁调用（比如装备疯狂洗练的时候），GC较高，性能消耗较大（并且是带有冰暗水火属性的装备GC才会较高，才能用Profiler测出来）
        // 所以要加一个优化，在疯狂洗练过程中，不需要实时的告诉服务器计算结果，只需要在洗练结束的时候通知服务器一次就够了
        private void SyncMainPlayerBaseDataByUpdateItem(List<ItemData> itemDataList)
        {
            if (itemDataList == null || itemDataList.Count <= 0)
            {
                return;
            }

            //同步魔法值
            var syncResistMagicValue = false;
            for (var i = 0; i < itemDataList.Count; i++)
            {
                var curItemData = itemDataList[i];
                if(curItemData == null)
                    continue;

                //如果更新的装备的附魔值存在，则同步
                var curItemResistMagicValue = GetItemResistMagicValue(curItemData.TableID);
                if (curItemResistMagicValue > 0)
                {
                    syncResistMagicValue = true;
                    break;
                }

                var isItemBelongSuit = IsItemBelongSuit(curItemData.TableID);
                if (isItemBelongSuit == true)
                {
                    syncResistMagicValue = true;
                    break;
                }
            }

            if(syncResistMagicValue == false)
                return;

            SendSyncResistMagicValueReq();
        }

        private void SendSyncResistMagicValueReq()
        {
            if(ClientSystemManager.GetInstance().CurrentSystem == null)
                return;

            var systemTown = ClientSystemManager.GetInstance().CurrentSystem as ClientSystemTown;
            if(systemTown == null)
                return;

            if(systemTown.MainPlayer == null)
                return;
            
            systemTown.MainPlayer.SyncResistMagicValue();
        }

        private void OnSyncResistMagicValueByBuffChanged(UIEvent uiEvent)
        {
            SendSyncResistMagicValueReq();
        }

        private void OnContinueProcessStart(UIEvent uiEvent)
        {
            bCalResistMagicValue = false;
        }

        private void OnContinueProcessReset(UIEvent uiEvent)
        {
            bCalResistMagicValue = true;
        }

        private void OnContinueProcessFinish(UIEvent uiEvent)
        {
            bCalResistMagicValue = true;

            // 疯狂洗练,连续强化，连续激化 结束的时候计算一次就好
            SendSyncResistMagicValueReq();
        }

        #endregion

        public PrecBead[] SwitchPrecBead(PreciousBeadMountHole[] data)
        {
            if (data == null)
                return null;
            PrecBead[] precBeadData = new PrecBead[data.Length];
            for(int i=0;i< data.Length; i++)
            {
                if (data[i] == null)
                    continue;
                precBeadData[i] = new PrecBead();
                precBeadData[i].index = (int)data[i].index;
                precBeadData[i].type = (int)data[i].type;
                int beadId = (int)data[i].preciousBeadId;
                precBeadData[i].preciousBeadId = beadId;
                precBeadData[i].randomBuffId = (int)data[i].buffId;
                precBeadData[i].pickNumber = (int)data[i].extirpeCnt;
                precBeadData[i].beadReplaceNumber = (int)data[i].replaceCnt;
            }
            return precBeadData;
        }

        public PrecEnchantmentCard SwichPrecEnchantmentCard(ItemMountedMagic itemMountedMagic)
        {
            PrecEnchantmentCard mPrecEnchantmentCard = new PrecEnchantmentCard();
            if (itemMountedMagic != null)
            {
                mPrecEnchantmentCard.iEnchantmentCardID = (int)itemMountedMagic.magicCardId;
                mPrecEnchantmentCard.iEnchantmentCardLevel = itemMountedMagic.level;
            }

            return mPrecEnchantmentCard;
        }

        #region MountMagicCardAndBead
        //镶嵌附魔卡数据
        public PrecEnchantmentCard MountMagicCardInItem(uint mountMagicCardId, byte mountMagicCardLevel)
        {
            PrecEnchantmentCard precEnchantmentCard = new PrecEnchantmentCard()
            {
                iEnchantmentCardID = (int) mountMagicCardId,
                iEnchantmentCardLevel = (int) mountMagicCardLevel,
            };
            return precEnchantmentCard;
        }

        //镶嵌宝珠的数据
        public PrecBead[] MountBeadInItem(uint mountBeadId, uint mountBeadBuffId)
        {
            PrecBead precBead = new PrecBead();
            precBead.preciousBeadId = (int) mountBeadId;
            precBead.randomBuffId = (int) mountBeadBuffId;

            PrecBead[] precBeadData = new PrecBead[] {precBead};
            return precBeadData;
        }
        #endregion

        public List<InscriptionHoleData> SwichInscriptionHoleData(InscriptionMountHole[] data, ItemData itemData)
        {
            if (data == null)
            {
                return null;
            }

            List<InscriptionHoleData> inscriptionHoleDatas = new List<InscriptionHoleData>();

            List<HoleData> holeDatas = InscriptionMosaicDataManager.GetInstance().GetEquipmentInscriptionHoleNumber(itemData);

            for (int i = 0; i < holeDatas.Count; i++)
            {
                if (holeDatas[i] == null)
                {
                    continue;
                }

                InscriptionHoleData holeData = new InscriptionHoleData();

                if (i < data.Length)
                {
                    holeData.Index = (int)data[i].index;
                    holeData.Type = (int)data[i].type;
                    holeData.InscriptionId = (int)data[i].inscriptionId;
                    holeData.IsOpenHole = true;
                }
                else
                {
                    holeData.Index = holeDatas[i].Index;
                    holeData.Type = holeDatas[i].Type;
                    holeData.InscriptionId = 0;
                    holeData.IsOpenHole = false;
                }

                inscriptionHoleDatas.Add(holeData);
            }

            return inscriptionHoleDatas;
        }

        #region 装备基础评分系数表格数据

        private List<EquipBaseScoreModTable> mEquipBaseScoreModTableList = new List<EquipBaseScoreModTable>();

        private void _InitEquipBaseScoreModTable()
        {
            if (mEquipBaseScoreModTableList == null)
            {
                mEquipBaseScoreModTableList = new List<EquipBaseScoreModTable>();
            }

            mEquipBaseScoreModTableList.Clear();

            var dict = TableManager.GetInstance().GetTable<EquipBaseScoreModTable>().GetEnumerator();
            while (dict.MoveNext())
            {
                var table = dict.Current.Value as EquipBaseScoreModTable;
                if (table == null)
                {
                    continue;
                }

                mEquipBaseScoreModTableList.Add(table);
            }
        }

        public EquipBaseScoreModTable GetEquipBaseScoreModTable(int subType,int quality)
        {
            EquipBaseScoreModTable table = null;
            for (int i = 0; i < mEquipBaseScoreModTableList.Count; i++)
            {
                var mEquipBaseScoreModTable = mEquipBaseScoreModTableList[i];
                if (mEquipBaseScoreModTable == null)
                {
                    continue;
                }

                if (subType != (int)mEquipBaseScoreModTable.ItemSubType)
                {
                    continue;
                }

                if (quality != (int)mEquipBaseScoreModTable.ItemQuality)
                {
                    continue;
                }

                table = mEquipBaseScoreModTable;
            }

            return table;
        }

        #endregion
    }
}
