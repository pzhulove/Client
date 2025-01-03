using System;
using System.Collections.Generic;
///////删除linq
using System.Text;
using System.Reflection;

namespace Protocol
{
    public enum EItemProperty
    {
        EP_INVALID,         //无效属性
        EP_NUM,             //叠加数	UInt16
        EP_BIND,            //绑定		UInt8
        EP_PACK,            //包裹		UInt8
        EP_GRID,            //包裹格子	UInt32
        EP_PHY_ATK,         //物理攻击		UInt32
        EP_MAG_ATK,         //法术攻击
        EP_PHY_DEF,         //物理防御		UInt32
        EP_MAG_DEF,			//法术防御		UInt32
        EP_STR,             //力量		UInt32
        EP_STAMINA,         //体力		UInt32
        EP_INTELLECT,       //智力		UInt32
        EP_SPIRIT,          //精力		UInt32
        EP_QUALITYLV,       //品级
        EP_QUALITY,         //装备品质	UInt8 EquipQuality
        EP_STRENGTHEN,      //强化等级	UInt8
        EP_RANDATTR,        //随机属性	格式：[PkValueID(UInt8) + ModPkValueType(UInt8) + Value(Int32)]...[0(UInt8)]
        EP_DAYUSENUM,         //每日剩余使用次数		UInt32
        //EP_GEMSTONE,        //宝石      格式: [pos(UInt8 1-5)][ItemID(UInt32)][Level(UInt8)]...[0(UInt8)]
        EP_ADDMAGIC,        //附魔		格式：[符文孔等级(uInt8)][ItemID(UInt32)][Level(UInt8)]
        EP_PARAM1,          //灵活字段1 UInt32
        EP_PARAM2,          //灵活字段2	UInt32
        EP_POWER,           //战斗力	UInt32
        EP_DEADLINE,        //失效时间	UInt32	永久有效的道具值为0
        EP_PRICE,           //价格		UInt32	玩家设置的价格
        EP_STRFAILED,       //强化失败次数	UInt16
        EP_SEAL_STATE,      //是否封装 UInt8
        EP_SEAL_COUNT,      //封装次数 UInt32
        EP_DIS_PHYATK,      //无视物理攻击
        EP_DIS_MAGATK,      //无视法术攻击
        EP_DIS_PHYDEF,          //无视物理防御
        EP_DIS_MAGDEF,			//无视法术防御
		EP_VALUE_SCORE = 35,			//装备评分
        EP_IA_FASHION_ATTRID = 37,  //时装属性ID
        EP_FASHION_ATTR_SELNUM = 38,
        EP_PHYDEF_PERCENT = 39, //强化物理防御百分比
        EP_MAGDEF_PERCENT = 40, //强化魔法防御百分比
        EP_ADDBEAD,         //宝珠
        EP_STRPROP_LIGHT,		//光属强
        EP_STRPROP_FIRE,		//火属强
        EP_STRPROP_ICE,			//冰属强
        EP_STRPROP_DARK,		//暗属强
        EP_DEFPROP_LIGHT,		//光属抗
        EP_DEFPROP_FIRE,		//火属抗
        EP_DEFPROP_ICE,			//冰属抗
        EP_DEFPROP_DARK,		//暗属抗
        EP_ABNORMAL_RESISTS_TOTAL,	//异抗总
        EP_EAR_FLASH,		//异抗感电
        EP_EAR_BLEEDING,		//异抗出血
        EP_EAR_BURN,		//异抗灼烧
        EP_EAR_POISON,		//异抗中毒
        EP_EAR_BLIND,		//异抗失明
        EP_EAR_STUN,		//异抗晕眩
        EP_EAR_STONE,		//异抗石化
        EP_EAR_FROZEN,		//异抗冰冻
        EP_EAR_SLEEP,		//异抗睡眠
        EP_EAR_CONFUNSE,		//异抗混乱
        EP_EAR_STRAIN,		//异抗束缚
        EP_EAR_SPEED_DOWN,		//异抗减速
        EP_EAR_CURSE,		//异抗诅咒
		EP_TRANSFER_STONE,    //转移石itemid
        EP_RECO_SCORE,       //装备回收所得积分
        EP_LOCK_ITEM,       //锁住道具
		EP_PRECIOUSBEAD_HOLES, //宝珠镶嵌孔
		EP_AUCTION_COOL_TIMESTAMP, //拍卖行交易冷却时间
		EP_IS_TREAS,		//是否是珍品
        EP_BEAD_EXTIRPE_CNT, // 宝珠摘除次数
		EP_BEAD_REPLACE_CNT, // 宝珠置换次数
		EP_TABLE_ID, //物品ID
        EP_EQUIP_TYPE,       //装备类型:普通0/带气息1/红字2
	    EP_ENHANCE_TYPE,     //红字装备增幅类型:力量0/智力1/体力2/精神3
        EP_ENHANCE_NUM,      //红字装备增幅数值
        EP_ENHANCE_FAILED,   //红字装备增幅失败次数
		EA_AUCTION_TRANS_NUM, //拍卖行交易次数
		EP_INSCRIPTION_HOLES,	//铭文镶嵌孔
		EP_INDEPENDATK,			//独立攻击力
		EP_INDEPENDATK_STRENG,	//独立攻击力强化
		EP_SUBTYPE,	//子类型
    };

    public class Item : StreamObject
    {
        public override bool GetStructProperty(FieldInfo field, byte[] buffer, ref int pos, int length)
        {
          
            if (field.FieldType == typeof(ItemRandProp[]))
            {
                byte num = 0;
                BaseDLL.decode_int8(buffer, ref pos, ref num);
                for (int i = 0; i < num; ++i)
                {  
                    ItemRandProp val = new ItemRandProp();
                    val.decode(buffer, ref pos);
                    randProps[i] = val;
                }
                return true;
            }
            else if (field.FieldType == typeof(GemStone[]))
            {
                byte val = 0;
                BaseDLL.decode_int8(buffer, ref pos, ref val);
               
                return true;
            }
            else if (field.FieldType == typeof(ItemMagicProperty[]))
            {
//                 ItemMagicProperty val = new ItemMagicProperty();
//                 byte num = 0;
//               BaseDLL.decode_int8(buffer, ref pos, ref num);
//                 for (int i = 0; i < num; ++i)
//               {
//                     val.decode(buffer, ref pos);
//                     magicProps[i] = val;
//                 }

                return true;
            }
			 else if (field.FieldType == typeof(ItemMountedMagic))
            {
                ItemMountedMagic data = new ItemMountedMagic();
                data.decode(buffer, ref pos);
                mountedMagic = data;
                return true;
            }
			else if (field.FieldType == typeof(PreciousBeadMountHole[]))
			{
				byte num = 0;
                BaseDLL.decode_int8(buffer, ref pos, ref num);
				for (int i = 0; i < num; ++i)
                {
					PreciousBeadMountHole hole = new PreciousBeadMountHole();
					hole.decode(buffer, ref pos);
					preciousBeadHoles[i] = hole;
				}					
                 return true;
			}
			else if (field.FieldType == typeof(InscriptionMountHole[]))
			{
				short num = 0;
                BaseDLL.decode_int16(buffer, ref pos, ref num);

                if (num > MAX_INSCRIPTION_MOUNTHOLE_NUM)
                {
                    num = MAX_INSCRIPTION_MOUNTHOLE_NUM;
                }

                inscriptionHoles = new InscriptionMountHole[num];
                
                for (int i = 0; i < num; ++i)
                {
					InscriptionMountHole hole = new InscriptionMountHole();
					hole.decode(buffer, ref pos);
					inscriptionHoles[i] = hole;
				}					
                 return true;
			}

            return false;
        }

        public static byte MAX_EQUIPATTR_NUM = 3;
        public static byte MAX_EQUIP_GEM_HOLE = 4;
        public static byte MAX_MAGICPROP_NUM = 5;
		public static byte MAX_PRECBEAD_MOUNTHOLE_NUM = 2;
		public static byte MAX_INSCRIPTION_MOUNTHOLE_NUM = 2;
		
        public UInt64 uid;
        public UInt32 dataid;
        [SProperty((int)EItemProperty.EP_NUM)]
        public UInt16 num;
        [SProperty((int)EItemProperty.EP_BIND)]
        public byte bind;       
        [SProperty((int)EItemProperty.EP_PACK)]
        public byte pack;       
        [SProperty((int)EItemProperty.EP_GRID)]
        public UInt32 grid;       
        [SProperty((int)EItemProperty.EP_PHY_ATK)]
        public UInt32 phyatk;   
        [SProperty((int)EItemProperty.EP_MAG_ATK)]
        public UInt32 magatk;   
        [SProperty((int)EItemProperty.EP_PHY_DEF)]
        public UInt32 phydef;
        [SProperty((int)EItemProperty.EP_MAG_DEF)]
        public UInt32 magdef;
        [SProperty((int)EItemProperty.EP_STR)]
        public UInt32 strenth;  
        [SProperty((int)EItemProperty.EP_STAMINA)]
        public UInt32 stamina;  
        [SProperty((int)EItemProperty.EP_INTELLECT)]
        public UInt32 intellect;    
        [SProperty((int)EItemProperty.EP_SPIRIT)]
        public UInt32 spirit;   
        [SProperty((int)EItemProperty.EP_QUALITYLV)]
        public byte qualitylv;  
        [SProperty((int)EItemProperty.EP_QUALITY)]
        public byte quality;
        [SProperty((int)EItemProperty.EP_STRENGTHEN)]
        public byte strengthen;
        [SProperty((int)EItemProperty.EP_RANDATTR)]
        public ItemRandProp[] randProps = new ItemRandProp[MAX_EQUIPATTR_NUM];
        [SProperty((int)EItemProperty.EP_DAYUSENUM)]
        public UInt32 dayUseNum;
//         [SProperty((int)EItemProperty.EP_GEMSTONE)]
//         public GemStone[] gemStones = new GemStone[MAX_EQUIP_GEM_HOLE];
        [SProperty((int)EItemProperty.EP_PARAM1)]
        public UInt32 param1;   // 保留字段，现用来做是否是新道具标识
        [SProperty((int)EItemProperty.EP_PARAM2)]
        public UInt32 param2;
        [SProperty((int)EItemProperty.EP_DEADLINE)]
        public UInt32 deadLine;
        [SProperty((int)EItemProperty.EP_STRFAILED)]
        public UInt32 strFailedTimes;
        [SProperty((int)EItemProperty.EP_ADDMAGIC)]
        public ItemMountedMagic mountedMagic;
        [SProperty((int)EItemProperty.EP_SEAL_STATE)]
        public byte sealstate;
        [SProperty((int)EItemProperty.EP_SEAL_COUNT)]
        public UInt32 sealcount;
        [SProperty((int)EItemProperty.EP_DIS_PHYATK)]
        public UInt32 disPhyAtk;
        [SProperty((int)EItemProperty.EP_DIS_MAGATK)]
        public UInt32 disMagAtk;
        [SProperty((int)EItemProperty.EP_DIS_PHYDEF)]
        public UInt32 disPhyDef;
        [SProperty((int)EItemProperty.EP_DIS_MAGDEF)]
        public UInt32 disMagDef;
		[SProperty((int)EItemProperty.EP_VALUE_SCORE)]
        public UInt32 valueScore;
        [SProperty((int)EItemProperty.EP_IA_FASHION_ATTRID)]
        public UInt32 fashionAttributeID;
        [SProperty((int)EItemProperty.EP_FASHION_ATTR_SELNUM)]
        public UInt32 fashionFreeSelNum;
        [SProperty((int)EItemProperty.EP_PHYDEF_PERCENT)]
        public UInt32 disPhyDefRate;
        [SProperty((int)EItemProperty.EP_MAGDEF_PERCENT)]
        public UInt32 disMagDefRate;
        [SProperty((int)EItemProperty.EP_ADDBEAD)]
        public UInt32 beadCardId;
        [SProperty((int)EItemProperty.EP_STRPROP_LIGHT)]
        public UInt32 strPropLight;
        [SProperty((int)EItemProperty.EP_STRPROP_FIRE)]
        public UInt32 strPropFire;
        [SProperty((int)EItemProperty.EP_STRPROP_ICE)]
        public UInt32 strPropIce;
        [SProperty((int)EItemProperty.EP_STRPROP_DARK)]
        public UInt32 strPropDark;
        [SProperty((int)EItemProperty.EP_DEFPROP_LIGHT)]
        public UInt32 defPropLight;
        [SProperty((int)EItemProperty.EP_DEFPROP_FIRE)]
        public UInt32 defPropFire;
        [SProperty((int)EItemProperty.EP_DEFPROP_ICE)]
        public UInt32 defPropIce;
        [SProperty((int)EItemProperty.EP_DEFPROP_DARK)]
        public UInt32 defPropDark;
        [SProperty((int)EItemProperty.EP_ABNORMAL_RESISTS_TOTAL)]
        public UInt32 abnormalResistsTotal;
        [SProperty((int)EItemProperty.EP_EAR_FLASH)]
        public UInt32 abnormalResistFlash;
        [SProperty((int)EItemProperty.EP_EAR_BLEEDING)]
        public UInt32 abnormalResistBleeding;
        [SProperty((int)EItemProperty.EP_EAR_BURN)]
        public UInt32 abnormalResistBurn;
        [SProperty((int)EItemProperty.EP_EAR_POISON)]
        public UInt32 abnormalResistPoison;
        [SProperty((int)EItemProperty.EP_EAR_BLIND)]
        public UInt32 abnormalResistBlind;
        [SProperty((int)EItemProperty.EP_EAR_STUN)]
        public UInt32 abnormalResistStun;
        [SProperty((int)EItemProperty.EP_EAR_STONE)]
        public UInt32 abnormalResistStone;
        [SProperty((int)EItemProperty.EP_EAR_FROZEN)]
        public UInt32 abnormalResistFrozen;
        [SProperty((int)EItemProperty.EP_EAR_SLEEP)]
        public UInt32 abnormalResistSleep;
        [SProperty((int)EItemProperty.EP_EAR_CONFUNSE)]
        public UInt32 abnormalResistConfunse;
        [SProperty((int)EItemProperty.EP_EAR_STRAIN)]
        public UInt32 abnormalResistStrain;
        [SProperty((int)EItemProperty.EP_EAR_SPEED_DOWN)]
        public UInt32 abnormalResistSpeedDown;
        [SProperty((int)EItemProperty.EP_EAR_CURSE)]
        public UInt32 abnormalResistCurse;
		[SProperty((int)EItemProperty.EP_TRANSFER_STONE)]
        public UInt32 transferStone;
        [SProperty((int)EItemProperty.EP_RECO_SCORE)]
        public UInt32 recoScore;
        [SProperty((int)EItemProperty.EP_LOCK_ITEM)]
        public UInt32 lockItem;
		[SProperty((int)EItemProperty.EP_PRECIOUSBEAD_HOLES)]
        public PreciousBeadMountHole[] preciousBeadHoles = new PreciousBeadMountHole[MAX_PRECBEAD_MOUNTHOLE_NUM];
		[SProperty((int)EItemProperty.EP_AUCTION_COOL_TIMESTAMP)]
		public UInt32 auctionCoolTimeStamp;
		[SProperty((int)EItemProperty.EP_IS_TREAS)]
		public byte isTreas;
        [SProperty((int)EItemProperty.EP_BEAD_EXTIRPE_CNT)]
        public UInt32 beadExtipreCnt;
		[SProperty((int)EItemProperty.EP_BEAD_REPLACE_CNT)]
        public UInt32 beadReplaceCnt;
        [SProperty((int)EItemProperty.EP_TABLE_ID)]
        public UInt32 tableID;
        [SProperty((int)EItemProperty.EP_EQUIP_TYPE)]
        public byte equipType;
        [SProperty((int)EItemProperty.EP_ENHANCE_TYPE)]
        public byte enhanceType;
		[SProperty((int)EItemProperty.EP_ENHANCE_NUM)]
		public UInt32 enhanceNum;
		[SProperty((int)EItemProperty.EP_ENHANCE_FAILED)]
		public UInt32 enhanceFailed;
		[SProperty((int)EItemProperty.EA_AUCTION_TRANS_NUM)]
		public UInt32 auctionTransNum;
		[SProperty((int)EItemProperty.EP_INSCRIPTION_HOLES)]
		public InscriptionMountHole[] inscriptionHoles = null;
		[SProperty((int)EItemProperty.EP_INDEPENDATK)]
		public UInt32 independAtk;
		[SProperty((int)EItemProperty.EP_INDEPENDATK_STRENG)]
		public UInt32 independAtkStreng;
		[SProperty((int)EItemProperty.EP_SUBTYPE)]
		public byte subtype;
    }

    class ItemDecoder
    {
        public static List<Item> Decode(byte[] buffer, ref int pos, int length, bool isUpdate = false)
        {
            List<Item> items = new List<Item>();

            while (true)
            {
#if MG_TEST || UNITY_EDITOR
                if(buffer.Length < pos + 8)
                {
                    Logger.LogErrorFormat("error buffer length:{0} pos:{1}", buffer.Length, pos);
                    break;
                }
#endif

                UInt64 uid = 0;
                UInt32 dataid = 0;
                BaseDLL.decode_uint64(buffer, ref pos, ref uid);
                if (0 == uid) break;

                if (isUpdate == false)
                {
                    BaseDLL.decode_uint32(buffer, ref pos, ref dataid);
                }
               
                Item item = new Item();
                StreamObjectDecoder<Item>.DecodePropertys(ref item, buffer, ref pos, length);

                item.uid = uid;
                item.dataid = dataid;
                items.Add(item);
 
            }

            return items;
        }
    }

}
