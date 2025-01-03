using System;
using System.Text;

namespace Protocol
{
	/// <summary>
	/// 仓库名字类型
	/// </summary>
	public enum StorageNameType
	{
		/// <summary>
		/// 仓库名字开始
		/// </summary>
		STORAGE_NAME_BEGIN = 0,
		/// <summary>
		/// 仓库名字结束
		/// </summary>
		STORAGE_NAME_END = 99,
		/// <summary>
		/// 账号仓库名字开始
		/// </summary>
		ACCOUNT_STORAGE_NAME_BEGIN = 100,
		/// <summary>
		/// 账号仓库名字结束
		/// </summary>
		ACCOUNT_STORAGE_NAME_END = 199,
	}

	/// <summary>
	/// 扩展类型
	/// </summary>
	public enum ItemEnlargeType
	{
		/// <summary>
		/// 扩展包裹
		/// </summary>
		ITEM_ENLARGE_TYPE_PACKAGE = 0,
		/// <summary>
		/// 仓库标签
		/// </summary>
		ITEM_ENLARGE_TYPE_STORAGE = 1,
		/// <summary>
		/// 账号仓库标签
		/// </summary>
		ITEM_ENLARGE_TYPE_ACCOUNT_STORAGE = 2,
	}

	/// <summary>
	///  宝珠镶嵌孔类型
	/// </summary>
	public enum PreciousBeadMountHoleType
	{
		/// <summary>
		///  初始值
		/// </summary>
		PBMHT_NONE = 0,
		/// <summary>
		///  普通
		/// </summary>
		PBMHT_COMMON = 1,
		/// <summary>
		///  多彩
		/// </summary>
		PBMHT_COLOUR = 2,
		/// <summary>
		///  最大值
		/// </summary>
		PBMHT_MAX = 3,
	}

	/// <summary>
	///  商城购买获得物类型
	/// </summary>
	public enum MallBuyGotType
	{
		/// <summary>
		///  无
		/// </summary>
		MALL_BUY_GOT_TYPE_NONE = 0,
		/// <summary>
		///  地精纪念币
		/// </summary>
		MALL_BUY_GOT_TYPE_GNOME_COIN = 1,
	}

	/// <summary>
	///  我的夺宝状态
	/// </summary>
	public enum GambingMineStatus
	{
		/// <summary>
		///  初始值
		/// </summary>
		GMS_INIT = 0,
		/// <summary>
		///  夺宝成功
		/// </summary>
		GMS_SUCCESS = 1,
		/// <summary>
		///  夺宝失败
		/// </summary>
		GMS_FAILE = 2,
		/// <summary>
		///  等待揭晓
		/// </summary>
		GMS_WAIT = 3,
	}

	/// <summary>
	/// 夺宝商品状态
	/// </summary>
	public enum GambingItemStatus
	{
		GIS_INVLID = 0,
		/// <summary>
		///  准备
		/// </summary>
		GIS_PREPARE = 1,
		/// <summary>
		///  在售
		/// </summary>
		GIS_SELLING = 2,
		/// <summary>
		///  售完
		/// </summary>
		GIS_SOLD_OUE = 3,
		/// <summary>
		///  开奖
		/// </summary>
		GIS_LOTTERY = 4,
		/// <summary>
		///  下架
		/// </summary>
		GIS_OFF_SALE = 5,
	}

	/// <summary>
	/// 商城玩家绑定礼包激活条件
	/// </summary>
	public enum MallGiftPackActivateCond
	{
		/// <summary>
		/// 无
		/// </summary>
		INVALID = 0,
		/// <summary>
		/// 强化到10
		/// </summary>
		STRENGEN_TEN = 1,
		/// <summary>
		/// 强化资源不足	
		/// </summary>
		STRENGEN_NO_RESOURCE = 2,
		/// <summary>
		/// 品级调整箱不足
		/// </summary>
		NO_QUILTY_ADJUST_BOX = 3,
		/// <summary>
		/// 疲劳不足，且背包中无疲劳药水
		/// </summary>
		NO_FATIGUE = 4,
		/// <summary>
		/// 刷深渊门票不足	
		/// </summary>
		NO_HELL_TICKET = 5,
		/// <summary>
		/// 刷远古门票不足	
		/// </summary>
		NO_ANCIENT_TICKET = 6,
		/// <summary>
		/// 死亡
		/// </summary>
		DIE = 7,
		/// <summary>
		/// 强化装备碎掉,推送10级装备
		/// </summary>
		STRENGEN_BROKE_TEN = 8,
		/// <summary>
		/// 强化装备碎掉,推送15级装备
		/// </summary>
		STRENGEN_BROKE_FIFTEEN = 9,
		/// <summary>
		/// 强化装备碎掉,推送20级装备
		/// </summary>
		STRENGEN_BROKE_TWENTY = 10,
		/// <summary>
		/// 强化装备碎掉,推送25级装备
		/// </summary>
		STRENGEN_BROKE_TWENTY_FIVE = 11,
		/// <summary>
		/// 强化装备碎掉,推送30级装备
		/// </summary>
		STRENGEN_BROKE_THIRTY = 12,
		/// <summary>
		/// 强化装备碎掉,推送35级装备
		/// </summary>
		STRENGEN_BROKE_THIRTY_FIVE = 13,
		/// <summary>
		/// 强化装备碎掉,推送40级装备
		/// </summary>
		STRENGEN_BROKE_FORTY = 14,
		/// <summary>
		/// 强化装备碎掉,推送45级装备
		/// </summary>
		STRENGEN_BROKE_FORTY_FIVE = 15,
		/// <summary>
		/// 强化装备碎掉,推送50级装备
		/// </summary>
		STRENGEN_BROKE_FIFTY = 16,
	}

	/// <summary>
	/// 商城商品类型
	/// </summary>
	public enum MallGoodsType
	{
		/// <summary>
		/// 普通商品
		/// </summary>
		INVALID = 0,
		/// <summary>
		/// 人民币礼包：可每日刷新
		/// </summary>
		GIFT_DAILY_REFRESH = 1,
		/// <summary>
		/// 礼包：账号激活限制一次
		/// </summary>
		GIFT_ACTIVATE_ONCE = 2,
		/// <summary>
		/// 礼包：账号激活限制三次礼包
		/// </summary>
		GIFT_ACTIVATE_THREE_TIMES = 3,
		/// <summary>
		/// 普通商品：可多选一
		/// </summary>
		COMMON_CHOOSE_ONE = 4,
		/// <summary>
		/// 礼包：限时活动
		/// </summary>
		GIFT_ACTIVITY = 5,
		/// <summary>
		/// 礼包: 普通不刷新礼包
		/// </summary>
		GIFT_COMMON = 6,
		/// <summary>
		/// 礼包: 每日刷新
		/// </summary>
		GIFT_COMMON_DAILY_REFRESH = 7,
	}

	/// <summary>
	/// 商城礼包活动状态
	/// </summary>
	public enum MallGiftPackActivityState
	{
		/// <summary>
		/// 无效
		/// </summary>
		GPAS_INVALID = 0,
		/// <summary>
		/// 开放
		/// </summary>
		GPAS_OPEN = 1,
		/// <summary>
		/// 关闭
		/// </summary>
		GPAS_CLOSED = 2,
	}

	/// <summary>
	/// 私人订制触发状态
	/// </summary>
	public enum MallPersonalTailorActivateState
	{
		MPTAS_INVALID = 0,
		/// <summary>
		/// 已开启
		/// </summary>
		MPTAS_OPEN = 1,
		/// <summary>
		/// 已关闭
		/// </summary>
		MPTAS_CLOSED = 2,
		/// <summary>
		/// 玩家上线
		/// </summary>
		MPTAS_ONLINE = 3,
	}

	/// <summary>
	///  快速购买目标类型
	/// </summary>
	public enum QuickBuyTargetType
	{
		/// <summary>
		///  复活
		/// </summary>
		QUICK_BUY_REVIVE = 0,
		/// <summary>
		///  购买道具
		/// </summary>
		QUICK_BUY_ITEM = 1,
	}

	public enum FashionMergeResultType
	{
		FMRT_NORMAL = 1,
		FMRT_SPECIAL = 2,
	}

	public enum ItemCheckType
	{
		ICT_VALID = 0,
		/// <summary>
		///  有效
		/// </summary>
		ICT_ABOUT_TO_EXPIRE = 1,
		/// <summary>
		///  快要到期
		/// </summary>
		ICT_EXPIRE = 2,
		/// <summary>
		///  到期可续费
		/// </summary>
		ICT_NEED_DELETE = 3,
	}

	public enum ItemLockType
	{
		/// <summary>
		///  道具没锁
		/// </summary>
		ILT_ITEM_UNLOCK = 0,
		/// <summary>
		///  道具锁
		/// </summary>
		ILT_ITEM_LOCK = 1,
		/// <summary>
		///  租赁锁	
		/// </summary>
		ILT_LEASE_LOCK = 2,
		/// <summary>
		///  时装锁
		/// </summary>
		ILT_FASHION_LOCK = 8,
	}

	/// <summary>
	/// 宝珠升级类型
	/// </summary>
	public enum UpgradePrecType
	{
		/// <summary>
		///  未镶嵌
		/// </summary>
		UnMounted = 0,
		/// <summary>
		///  已镶嵌
		/// </summary>
		Mounted = 1,
	}

	/// <summary>
	/// 黑市商人类型
	/// </summary>
	public enum BlackMarketType
	{
		BmtInvalid = 0,
		/// <summary>
		///  固定价格
		/// </summary>
		BmtFixedPrice = 1,
		/// <summary>
		///  竞拍价格
		/// </summary>
		BmtAuctionPrice = 2,
	}

	public enum BlackMarketAuctionState
	{
		BmaisInvalid = 0,
		/// <summary>
		/// 可以竞拍
		/// </summary>
		BmaisCanAuction = 1,
		/// <summary>
		/// 已申请
		/// </summary>
		BmaisApplyed = 2,
		/// <summary>
		/// 已交易
		/// </summary>
		BmaisTransed = 3,
		BmaisMax = 4,
	}

	public enum MagicCardCompOneKeyEndReason
	{
		MCCER_NONE = 0,
		/// <summary>
		/// 次数达到最大
		/// </summary>
		MCCER_TIMES_MAX = 1,
		/// <summary>
		/// 背包满
		/// </summary>
		MCCER_ITEMPACK_FULL = 2,
		/// <summary>
		/// 货币不足
		/// </summary>
		MCCER_MONEY_INSUFF = 3,
		MCCER_MONEY_MAX = 4,
	}

	/// <summary>
	///  商城购买获得物类型
	/// </summary>
	public enum EquipSchemeType
	{
		/// <summary>
		///  无
		/// </summary>
		EQST_NONE = 0,
		/// <summary>
		/// 装备，称号
		/// </summary>
		EQST_EQUIP = 1,
	}

	/// <summary>
	///  装备转换类型
	/// </summary>
	public enum EquipConvertType
	{
		/// <summary>
		///  同套装
		/// </summary>
		EQCT_SAME = 1,
		/// <summary>
		///  跨套装
		/// </summary>
		EQCT_DIFF = 2,
	}

	/// <summary>
	/// 仓库名字信息
	/// </summary>
	public class StorageNameInfo : Protocol.IProtocolStream
	{
		public UInt16 type;
		public string name;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, type);
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint16(buffer, ref pos_, ref type);
				UInt16 nameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
				byte[] nameBytes = new byte[nameLen];
				for(int i = 0; i < nameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
				}
				name = StringHelper.BytesToString(nameBytes);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, type);
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint16(buffer, ref pos_, ref type);
				UInt16 nameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
				byte[] nameBytes = new byte[nameLen];
				for(int i = 0; i < nameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
				}
				name = StringHelper.BytesToString(nameBytes);
			}

			public int getLen()
			{
				int _len = 0;
				// type
				_len += 2;
				// name
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(name);
					_len += 2 + _strBytes.Length;
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  道具位置
	/// </summary>
	public class ItemPos : Protocol.IProtocolStream
	{
		/// <summary>
		///  包裹类型
		/// </summary>
		public byte type;
		/// <summary>
		///  格子索引
		/// </summary>
		public UInt32 index;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_uint32(buffer, ref pos_, index);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_uint32(buffer, ref pos_, ref index);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_uint32(buffer, ref pos_, index);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_uint32(buffer, ref pos_, ref index);
			}

			public int getLen()
			{
				int _len = 0;
				// type
				_len += 1;
				// index
				_len += 4;
				return _len;
			}
		#endregion

	}

	public class ItemRandProp : Protocol.IProtocolStream
	{
		public byte type;
		public UInt32 value;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_uint32(buffer, ref pos_, value);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_uint32(buffer, ref pos_, ref value);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_uint32(buffer, ref pos_, value);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_uint32(buffer, ref pos_, ref value);
			}

			public int getLen()
			{
				int _len = 0;
				// type
				_len += 1;
				// value
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  宝珠镶嵌孔
	/// </summary>
	public class PreciousBeadMountHole : Protocol.IProtocolStream
	{
		public byte index;
		/// <summary>
		/// 孔索引
		/// </summary>
		public byte type;
		/// <summary>
		/// 孔类型
		/// </summary>
		public UInt32 preciousBeadId;
		/// <summary>
		/// 镶嵌宝珠id
		/// </summary>
		public UInt32 buffId;
		/// <summary>
		/// 附加buff id
		/// </summary>
		public UInt32 auctionCoolTimeStamp;
		/// <summary>
		/// 宝珠拍卖行交易冷却时间戳(秒)
		/// </summary>
		public UInt32 extirpeCnt;
		/// <summary>
		/// 摘除次数
		/// </summary>
		public UInt32 replaceCnt;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, index);
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_uint32(buffer, ref pos_, preciousBeadId);
				BaseDLL.encode_uint32(buffer, ref pos_, buffId);
				BaseDLL.encode_uint32(buffer, ref pos_, auctionCoolTimeStamp);
				BaseDLL.encode_uint32(buffer, ref pos_, extirpeCnt);
				BaseDLL.encode_uint32(buffer, ref pos_, replaceCnt);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref index);
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_uint32(buffer, ref pos_, ref preciousBeadId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref buffId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref auctionCoolTimeStamp);
				BaseDLL.decode_uint32(buffer, ref pos_, ref extirpeCnt);
				BaseDLL.decode_uint32(buffer, ref pos_, ref replaceCnt);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, index);
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_uint32(buffer, ref pos_, preciousBeadId);
				BaseDLL.encode_uint32(buffer, ref pos_, buffId);
				BaseDLL.encode_uint32(buffer, ref pos_, auctionCoolTimeStamp);
				BaseDLL.encode_uint32(buffer, ref pos_, extirpeCnt);
				BaseDLL.encode_uint32(buffer, ref pos_, replaceCnt);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref index);
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_uint32(buffer, ref pos_, ref preciousBeadId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref buffId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref auctionCoolTimeStamp);
				BaseDLL.decode_uint32(buffer, ref pos_, ref extirpeCnt);
				BaseDLL.decode_uint32(buffer, ref pos_, ref replaceCnt);
			}

			public int getLen()
			{
				int _len = 0;
				// index
				_len += 1;
				// type
				_len += 1;
				// preciousBeadId
				_len += 4;
				// buffId
				_len += 4;
				// auctionCoolTimeStamp
				_len += 4;
				// extirpeCnt
				_len += 4;
				// replaceCnt
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 置换次数
	/// </summary>
	/// <summary>
	///  铭文镶嵌孔
	/// </summary>
	public class InscriptionMountHole : Protocol.IProtocolStream
	{
		public UInt32 index;
		/// <summary>
		/// 孔索引
		/// </summary>
		public UInt32 type;
		/// <summary>
		/// 孔类型
		/// </summary>
		public UInt32 inscriptionId;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, index);
				BaseDLL.encode_uint32(buffer, ref pos_, type);
				BaseDLL.encode_uint32(buffer, ref pos_, inscriptionId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref index);
				BaseDLL.decode_uint32(buffer, ref pos_, ref type);
				BaseDLL.decode_uint32(buffer, ref pos_, ref inscriptionId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, index);
				BaseDLL.encode_uint32(buffer, ref pos_, type);
				BaseDLL.encode_uint32(buffer, ref pos_, inscriptionId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref index);
				BaseDLL.decode_uint32(buffer, ref pos_, ref type);
				BaseDLL.decode_uint32(buffer, ref pos_, ref inscriptionId);
			}

			public int getLen()
			{
				int _len = 0;
				// index
				_len += 4;
				// type
				_len += 4;
				// inscriptionId
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 铭文id
	/// </summary>
	public class GemStone : Protocol.IProtocolStream
	{
		public UInt32 id;
		public byte level;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, id);
				BaseDLL.encode_int8(buffer, ref pos_, level);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
				BaseDLL.decode_int8(buffer, ref pos_, ref level);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, id);
				BaseDLL.encode_int8(buffer, ref pos_, level);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
				BaseDLL.decode_int8(buffer, ref pos_, ref level);
			}

			public int getLen()
			{
				int _len = 0;
				// id
				_len += 4;
				// level
				_len += 1;
				return _len;
			}
		#endregion

	}

	public class ItemMagicProperty : Protocol.IProtocolStream
	{
		public byte type;
		/// <summary>
		/// 1.随机属性，2.buff
		/// </summary>
		public UInt32 param1;
		/// <summary>
		/// 随机属性: 属性id，buff:buffid
		/// </summary>
		public UInt32 param2;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_uint32(buffer, ref pos_, param1);
				BaseDLL.encode_uint32(buffer, ref pos_, param2);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_uint32(buffer, ref pos_, ref param1);
				BaseDLL.decode_uint32(buffer, ref pos_, ref param2);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_uint32(buffer, ref pos_, param1);
				BaseDLL.encode_uint32(buffer, ref pos_, param2);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_uint32(buffer, ref pos_, ref param1);
				BaseDLL.decode_uint32(buffer, ref pos_, ref param2);
			}

			public int getLen()
			{
				int _len = 0;
				// type
				_len += 1;
				// param1
				_len += 4;
				// param2
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 随机属性: 属性值，buff:无用
	/// </summary>
	public class ItemMountedMagic : Protocol.IProtocolStream
	{
		public UInt32 magicCardId;
		/// <summary>
		/// 附魔卡id
		/// </summary>
		public byte level;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, magicCardId);
				BaseDLL.encode_int8(buffer, ref pos_, level);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref magicCardId);
				BaseDLL.decode_int8(buffer, ref pos_, ref level);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, magicCardId);
				BaseDLL.encode_int8(buffer, ref pos_, level);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref magicCardId);
				BaseDLL.decode_int8(buffer, ref pos_, ref level);
			}

			public int getLen()
			{
				int _len = 0;
				// magicCardId
				_len += 4;
				// level
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 附魔卡等级
	/// </summary>
	public class ItemReward : Protocol.IProtocolStream
	{
		public UInt32 id;
		public UInt32 num;
		public byte qualityLv;
		/// <summary>
		///  强化等级
		/// </summary>
		public byte strength;
		public UInt32 auctionCoolTimeStamp;
		/// <summary>
		///  装备类型，对应枚举EquipType
		/// </summary>
		public byte equipType;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, id);
				BaseDLL.encode_uint32(buffer, ref pos_, num);
				BaseDLL.encode_int8(buffer, ref pos_, qualityLv);
				BaseDLL.encode_int8(buffer, ref pos_, strength);
				BaseDLL.encode_uint32(buffer, ref pos_, auctionCoolTimeStamp);
				BaseDLL.encode_int8(buffer, ref pos_, equipType);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
				BaseDLL.decode_uint32(buffer, ref pos_, ref num);
				BaseDLL.decode_int8(buffer, ref pos_, ref qualityLv);
				BaseDLL.decode_int8(buffer, ref pos_, ref strength);
				BaseDLL.decode_uint32(buffer, ref pos_, ref auctionCoolTimeStamp);
				BaseDLL.decode_int8(buffer, ref pos_, ref equipType);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, id);
				BaseDLL.encode_uint32(buffer, ref pos_, num);
				BaseDLL.encode_int8(buffer, ref pos_, qualityLv);
				BaseDLL.encode_int8(buffer, ref pos_, strength);
				BaseDLL.encode_uint32(buffer, ref pos_, auctionCoolTimeStamp);
				BaseDLL.encode_int8(buffer, ref pos_, equipType);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
				BaseDLL.decode_uint32(buffer, ref pos_, ref num);
				BaseDLL.decode_int8(buffer, ref pos_, ref qualityLv);
				BaseDLL.decode_int8(buffer, ref pos_, ref strength);
				BaseDLL.decode_uint32(buffer, ref pos_, ref auctionCoolTimeStamp);
				BaseDLL.decode_int8(buffer, ref pos_, ref equipType);
			}

			public int getLen()
			{
				int _len = 0;
				// id
				_len += 4;
				// num
				_len += 4;
				// qualityLv
				_len += 1;
				// strength
				_len += 1;
				// auctionCoolTimeStamp
				_len += 4;
				// equipType
				_len += 1;
				return _len;
			}
		#endregion

	}

	public class OpenJarResult : Protocol.IProtocolStream
	{
		public UInt32 jarItemId;
		public UInt64 itemUid;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, jarItemId);
				BaseDLL.encode_uint64(buffer, ref pos_, itemUid);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref jarItemId);
				BaseDLL.decode_uint64(buffer, ref pos_, ref itemUid);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, jarItemId);
				BaseDLL.encode_uint64(buffer, ref pos_, itemUid);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref jarItemId);
				BaseDLL.decode_uint64(buffer, ref pos_, ref itemUid);
			}

			public int getLen()
			{
				int _len = 0;
				// jarItemId
				_len += 4;
				// itemUid
				_len += 8;
				return _len;
			}
		#endregion

	}

	public class ItemCD : Protocol.IProtocolStream
	{
		public byte groupid;
		public UInt32 endtime;
		public UInt32 maxtime;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, groupid);
				BaseDLL.encode_uint32(buffer, ref pos_, endtime);
				BaseDLL.encode_uint32(buffer, ref pos_, maxtime);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref groupid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref endtime);
				BaseDLL.decode_uint32(buffer, ref pos_, ref maxtime);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, groupid);
				BaseDLL.encode_uint32(buffer, ref pos_, endtime);
				BaseDLL.encode_uint32(buffer, ref pos_, maxtime);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref groupid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref endtime);
				BaseDLL.decode_uint32(buffer, ref pos_, ref maxtime);
			}

			public int getLen()
			{
				int _len = 0;
				// groupid
				_len += 1;
				// endtime
				_len += 4;
				// maxtime
				_len += 4;
				return _len;
			}
		#endregion

	}

	public class ItemInfo : Protocol.IProtocolStream
	{
		public UInt64 uid;
		public UInt32 num;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, uid);
				BaseDLL.encode_uint32(buffer, ref pos_, num);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref uid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref num);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, uid);
				BaseDLL.encode_uint32(buffer, ref pos_, num);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref uid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref num);
			}

			public int getLen()
			{
				int _len = 0;
				// uid
				_len += 8;
				// num
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  商城购买获得物信息
	/// </summary>
	public class MallBuyGotInfo : Protocol.IProtocolStream
	{
		/// <summary>
		///  购买获得物类型(MallBuyGotType)
		/// </summary>
		public byte buyGotType;
		/// <summary>
		///  购买获得物对应的道具id
		/// </summary>
		public UInt32 itemDataId;
		/// <summary>
		///  购买获得物数量
		/// </summary>
		public UInt32 buyGotNum;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, buyGotType);
				BaseDLL.encode_uint32(buffer, ref pos_, itemDataId);
				BaseDLL.encode_uint32(buffer, ref pos_, buyGotNum);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref buyGotType);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemDataId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref buyGotNum);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, buyGotType);
				BaseDLL.encode_uint32(buffer, ref pos_, itemDataId);
				BaseDLL.encode_uint32(buffer, ref pos_, buyGotNum);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref buyGotType);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemDataId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref buyGotNum);
			}

			public int getLen()
			{
				int _len = 0;
				// buyGotType
				_len += 1;
				// itemDataId
				_len += 4;
				// buyGotNum
				_len += 4;
				return _len;
			}
		#endregion

	}

	public class MallItemInfo : Protocol.IProtocolStream
	{
		public UInt32 id;
		public byte type;
		public byte subtype;
		public byte jobtype;
		public UInt32 itemid;
		public UInt32 itemnum;
		public UInt32 price;
		public UInt32 discountprice;
		public byte moneytype;
		public byte limit;
		public UInt16 limitnum;
		public byte gift;
		public UInt16 vipscore;
		public string icon;
		public UInt32 starttime;
		public UInt32 endtime;
		public UInt16 limittotalnum;
		public ItemReward[] giftItems = new ItemReward[0];
		public string giftName;
		public byte tagType;
		public UInt32 sortIdx;
		public UInt32 hotSortIdx;
		public UInt16 goodsSubType;
		public byte isRecommend;
		public byte isPersonalTailor;
		public UInt32 discountRate;
		public byte loginPushId;
		public string fashionImagePath;
		public string giftDesc;
		/// <summary>
		///  购买获得物信息
		/// </summary>
		public MallBuyGotInfo[] buyGotInfos = new MallBuyGotInfo[0];
		/// <summary>
		///  扩展参数
		/// </summary>
		public UInt32[] extParams = new UInt32[0];
		/// <summary>
		///  账号刷新类型 RefreshType
		/// </summary>
		public byte accountRefreshType;
		/// <summary>
		///  账号限购次数
		/// </summary>
		public UInt32 accountLimitBuyNum;
		/// <summary>
		///  账号剩余购买次数
		/// </summary>
		public UInt32 accountRestBuyNum;
		/// <summary>
		///  折扣券id
		/// </summary>
		public UInt32 discountCouponId;
		/// <summary>
		///  多倍积分倍率
		/// </summary>
		public byte multiple;
		/// <summary>
		///  多倍结束时间
		/// </summary>
		public UInt32 multipleEndTime;
		/// <summary>
		///  抵扣券
		/// </summary>
		public UInt32 deductionCouponId;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, id);
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_int8(buffer, ref pos_, subtype);
				BaseDLL.encode_int8(buffer, ref pos_, jobtype);
				BaseDLL.encode_uint32(buffer, ref pos_, itemid);
				BaseDLL.encode_uint32(buffer, ref pos_, itemnum);
				BaseDLL.encode_uint32(buffer, ref pos_, price);
				BaseDLL.encode_uint32(buffer, ref pos_, discountprice);
				BaseDLL.encode_int8(buffer, ref pos_, moneytype);
				BaseDLL.encode_int8(buffer, ref pos_, limit);
				BaseDLL.encode_uint16(buffer, ref pos_, limitnum);
				BaseDLL.encode_int8(buffer, ref pos_, gift);
				BaseDLL.encode_uint16(buffer, ref pos_, vipscore);
				byte[] iconBytes = StringHelper.StringToUTF8Bytes(icon);
				BaseDLL.encode_string(buffer, ref pos_, iconBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint32(buffer, ref pos_, starttime);
				BaseDLL.encode_uint32(buffer, ref pos_, endtime);
				BaseDLL.encode_uint16(buffer, ref pos_, limittotalnum);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)giftItems.Length);
				for(int i = 0; i < giftItems.Length; i++)
				{
					giftItems[i].encode(buffer, ref pos_);
				}
				byte[] giftNameBytes = StringHelper.StringToUTF8Bytes(giftName);
				BaseDLL.encode_string(buffer, ref pos_, giftNameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, tagType);
				BaseDLL.encode_uint32(buffer, ref pos_, sortIdx);
				BaseDLL.encode_uint32(buffer, ref pos_, hotSortIdx);
				BaseDLL.encode_uint16(buffer, ref pos_, goodsSubType);
				BaseDLL.encode_int8(buffer, ref pos_, isRecommend);
				BaseDLL.encode_int8(buffer, ref pos_, isPersonalTailor);
				BaseDLL.encode_uint32(buffer, ref pos_, discountRate);
				BaseDLL.encode_int8(buffer, ref pos_, loginPushId);
				byte[] fashionImagePathBytes = StringHelper.StringToUTF8Bytes(fashionImagePath);
				BaseDLL.encode_string(buffer, ref pos_, fashionImagePathBytes, (UInt16)(buffer.Length - pos_));
				byte[] giftDescBytes = StringHelper.StringToUTF8Bytes(giftDesc);
				BaseDLL.encode_string(buffer, ref pos_, giftDescBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)buyGotInfos.Length);
				for(int i = 0; i < buyGotInfos.Length; i++)
				{
					buyGotInfos[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)extParams.Length);
				for(int i = 0; i < extParams.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, extParams[i]);
				}
				BaseDLL.encode_int8(buffer, ref pos_, accountRefreshType);
				BaseDLL.encode_uint32(buffer, ref pos_, accountLimitBuyNum);
				BaseDLL.encode_uint32(buffer, ref pos_, accountRestBuyNum);
				BaseDLL.encode_uint32(buffer, ref pos_, discountCouponId);
				BaseDLL.encode_int8(buffer, ref pos_, multiple);
				BaseDLL.encode_uint32(buffer, ref pos_, multipleEndTime);
				BaseDLL.encode_uint32(buffer, ref pos_, deductionCouponId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_int8(buffer, ref pos_, ref subtype);
				BaseDLL.decode_int8(buffer, ref pos_, ref jobtype);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemnum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref price);
				BaseDLL.decode_uint32(buffer, ref pos_, ref discountprice);
				BaseDLL.decode_int8(buffer, ref pos_, ref moneytype);
				BaseDLL.decode_int8(buffer, ref pos_, ref limit);
				BaseDLL.decode_uint16(buffer, ref pos_, ref limitnum);
				BaseDLL.decode_int8(buffer, ref pos_, ref gift);
				BaseDLL.decode_uint16(buffer, ref pos_, ref vipscore);
				UInt16 iconLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref iconLen);
				byte[] iconBytes = new byte[iconLen];
				for(int i = 0; i < iconLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref iconBytes[i]);
				}
				icon = StringHelper.BytesToString(iconBytes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref starttime);
				BaseDLL.decode_uint32(buffer, ref pos_, ref endtime);
				BaseDLL.decode_uint16(buffer, ref pos_, ref limittotalnum);
				UInt16 giftItemsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref giftItemsCnt);
				giftItems = new ItemReward[giftItemsCnt];
				for(int i = 0; i < giftItems.Length; i++)
				{
					giftItems[i] = new ItemReward();
					giftItems[i].decode(buffer, ref pos_);
				}
				UInt16 giftNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref giftNameLen);
				byte[] giftNameBytes = new byte[giftNameLen];
				for(int i = 0; i < giftNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref giftNameBytes[i]);
				}
				giftName = StringHelper.BytesToString(giftNameBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref tagType);
				BaseDLL.decode_uint32(buffer, ref pos_, ref sortIdx);
				BaseDLL.decode_uint32(buffer, ref pos_, ref hotSortIdx);
				BaseDLL.decode_uint16(buffer, ref pos_, ref goodsSubType);
				BaseDLL.decode_int8(buffer, ref pos_, ref isRecommend);
				BaseDLL.decode_int8(buffer, ref pos_, ref isPersonalTailor);
				BaseDLL.decode_uint32(buffer, ref pos_, ref discountRate);
				BaseDLL.decode_int8(buffer, ref pos_, ref loginPushId);
				UInt16 fashionImagePathLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref fashionImagePathLen);
				byte[] fashionImagePathBytes = new byte[fashionImagePathLen];
				for(int i = 0; i < fashionImagePathLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref fashionImagePathBytes[i]);
				}
				fashionImagePath = StringHelper.BytesToString(fashionImagePathBytes);
				UInt16 giftDescLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref giftDescLen);
				byte[] giftDescBytes = new byte[giftDescLen];
				for(int i = 0; i < giftDescLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref giftDescBytes[i]);
				}
				giftDesc = StringHelper.BytesToString(giftDescBytes);
				UInt16 buyGotInfosCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref buyGotInfosCnt);
				buyGotInfos = new MallBuyGotInfo[buyGotInfosCnt];
				for(int i = 0; i < buyGotInfos.Length; i++)
				{
					buyGotInfos[i] = new MallBuyGotInfo();
					buyGotInfos[i].decode(buffer, ref pos_);
				}
				UInt16 extParamsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref extParamsCnt);
				extParams = new UInt32[extParamsCnt];
				for(int i = 0; i < extParams.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref extParams[i]);
				}
				BaseDLL.decode_int8(buffer, ref pos_, ref accountRefreshType);
				BaseDLL.decode_uint32(buffer, ref pos_, ref accountLimitBuyNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref accountRestBuyNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref discountCouponId);
				BaseDLL.decode_int8(buffer, ref pos_, ref multiple);
				BaseDLL.decode_uint32(buffer, ref pos_, ref multipleEndTime);
				BaseDLL.decode_uint32(buffer, ref pos_, ref deductionCouponId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, id);
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_int8(buffer, ref pos_, subtype);
				BaseDLL.encode_int8(buffer, ref pos_, jobtype);
				BaseDLL.encode_uint32(buffer, ref pos_, itemid);
				BaseDLL.encode_uint32(buffer, ref pos_, itemnum);
				BaseDLL.encode_uint32(buffer, ref pos_, price);
				BaseDLL.encode_uint32(buffer, ref pos_, discountprice);
				BaseDLL.encode_int8(buffer, ref pos_, moneytype);
				BaseDLL.encode_int8(buffer, ref pos_, limit);
				BaseDLL.encode_uint16(buffer, ref pos_, limitnum);
				BaseDLL.encode_int8(buffer, ref pos_, gift);
				BaseDLL.encode_uint16(buffer, ref pos_, vipscore);
				byte[] iconBytes = StringHelper.StringToUTF8Bytes(icon);
				BaseDLL.encode_string(buffer, ref pos_, iconBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint32(buffer, ref pos_, starttime);
				BaseDLL.encode_uint32(buffer, ref pos_, endtime);
				BaseDLL.encode_uint16(buffer, ref pos_, limittotalnum);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)giftItems.Length);
				for(int i = 0; i < giftItems.Length; i++)
				{
					giftItems[i].encode(buffer, ref pos_);
				}
				byte[] giftNameBytes = StringHelper.StringToUTF8Bytes(giftName);
				BaseDLL.encode_string(buffer, ref pos_, giftNameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, tagType);
				BaseDLL.encode_uint32(buffer, ref pos_, sortIdx);
				BaseDLL.encode_uint32(buffer, ref pos_, hotSortIdx);
				BaseDLL.encode_uint16(buffer, ref pos_, goodsSubType);
				BaseDLL.encode_int8(buffer, ref pos_, isRecommend);
				BaseDLL.encode_int8(buffer, ref pos_, isPersonalTailor);
				BaseDLL.encode_uint32(buffer, ref pos_, discountRate);
				BaseDLL.encode_int8(buffer, ref pos_, loginPushId);
				byte[] fashionImagePathBytes = StringHelper.StringToUTF8Bytes(fashionImagePath);
				BaseDLL.encode_string(buffer, ref pos_, fashionImagePathBytes, (UInt16)(buffer.Length - pos_));
				byte[] giftDescBytes = StringHelper.StringToUTF8Bytes(giftDesc);
				BaseDLL.encode_string(buffer, ref pos_, giftDescBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)buyGotInfos.Length);
				for(int i = 0; i < buyGotInfos.Length; i++)
				{
					buyGotInfos[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)extParams.Length);
				for(int i = 0; i < extParams.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, extParams[i]);
				}
				BaseDLL.encode_int8(buffer, ref pos_, accountRefreshType);
				BaseDLL.encode_uint32(buffer, ref pos_, accountLimitBuyNum);
				BaseDLL.encode_uint32(buffer, ref pos_, accountRestBuyNum);
				BaseDLL.encode_uint32(buffer, ref pos_, discountCouponId);
				BaseDLL.encode_int8(buffer, ref pos_, multiple);
				BaseDLL.encode_uint32(buffer, ref pos_, multipleEndTime);
				BaseDLL.encode_uint32(buffer, ref pos_, deductionCouponId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_int8(buffer, ref pos_, ref subtype);
				BaseDLL.decode_int8(buffer, ref pos_, ref jobtype);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemnum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref price);
				BaseDLL.decode_uint32(buffer, ref pos_, ref discountprice);
				BaseDLL.decode_int8(buffer, ref pos_, ref moneytype);
				BaseDLL.decode_int8(buffer, ref pos_, ref limit);
				BaseDLL.decode_uint16(buffer, ref pos_, ref limitnum);
				BaseDLL.decode_int8(buffer, ref pos_, ref gift);
				BaseDLL.decode_uint16(buffer, ref pos_, ref vipscore);
				UInt16 iconLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref iconLen);
				byte[] iconBytes = new byte[iconLen];
				for(int i = 0; i < iconLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref iconBytes[i]);
				}
				icon = StringHelper.BytesToString(iconBytes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref starttime);
				BaseDLL.decode_uint32(buffer, ref pos_, ref endtime);
				BaseDLL.decode_uint16(buffer, ref pos_, ref limittotalnum);
				UInt16 giftItemsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref giftItemsCnt);
				giftItems = new ItemReward[giftItemsCnt];
				for(int i = 0; i < giftItems.Length; i++)
				{
					giftItems[i] = new ItemReward();
					giftItems[i].decode(buffer, ref pos_);
				}
				UInt16 giftNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref giftNameLen);
				byte[] giftNameBytes = new byte[giftNameLen];
				for(int i = 0; i < giftNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref giftNameBytes[i]);
				}
				giftName = StringHelper.BytesToString(giftNameBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref tagType);
				BaseDLL.decode_uint32(buffer, ref pos_, ref sortIdx);
				BaseDLL.decode_uint32(buffer, ref pos_, ref hotSortIdx);
				BaseDLL.decode_uint16(buffer, ref pos_, ref goodsSubType);
				BaseDLL.decode_int8(buffer, ref pos_, ref isRecommend);
				BaseDLL.decode_int8(buffer, ref pos_, ref isPersonalTailor);
				BaseDLL.decode_uint32(buffer, ref pos_, ref discountRate);
				BaseDLL.decode_int8(buffer, ref pos_, ref loginPushId);
				UInt16 fashionImagePathLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref fashionImagePathLen);
				byte[] fashionImagePathBytes = new byte[fashionImagePathLen];
				for(int i = 0; i < fashionImagePathLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref fashionImagePathBytes[i]);
				}
				fashionImagePath = StringHelper.BytesToString(fashionImagePathBytes);
				UInt16 giftDescLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref giftDescLen);
				byte[] giftDescBytes = new byte[giftDescLen];
				for(int i = 0; i < giftDescLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref giftDescBytes[i]);
				}
				giftDesc = StringHelper.BytesToString(giftDescBytes);
				UInt16 buyGotInfosCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref buyGotInfosCnt);
				buyGotInfos = new MallBuyGotInfo[buyGotInfosCnt];
				for(int i = 0; i < buyGotInfos.Length; i++)
				{
					buyGotInfos[i] = new MallBuyGotInfo();
					buyGotInfos[i].decode(buffer, ref pos_);
				}
				UInt16 extParamsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref extParamsCnt);
				extParams = new UInt32[extParamsCnt];
				for(int i = 0; i < extParams.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref extParams[i]);
				}
				BaseDLL.decode_int8(buffer, ref pos_, ref accountRefreshType);
				BaseDLL.decode_uint32(buffer, ref pos_, ref accountLimitBuyNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref accountRestBuyNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref discountCouponId);
				BaseDLL.decode_int8(buffer, ref pos_, ref multiple);
				BaseDLL.decode_uint32(buffer, ref pos_, ref multipleEndTime);
				BaseDLL.decode_uint32(buffer, ref pos_, ref deductionCouponId);
			}

			public int getLen()
			{
				int _len = 0;
				// id
				_len += 4;
				// type
				_len += 1;
				// subtype
				_len += 1;
				// jobtype
				_len += 1;
				// itemid
				_len += 4;
				// itemnum
				_len += 4;
				// price
				_len += 4;
				// discountprice
				_len += 4;
				// moneytype
				_len += 1;
				// limit
				_len += 1;
				// limitnum
				_len += 2;
				// gift
				_len += 1;
				// vipscore
				_len += 2;
				// icon
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(icon);
					_len += 2 + _strBytes.Length;
				}
				// starttime
				_len += 4;
				// endtime
				_len += 4;
				// limittotalnum
				_len += 2;
				// giftItems
				_len += 2;
				for(int j = 0; j < giftItems.Length; j++)
				{
					_len += giftItems[j].getLen();
				}
				// giftName
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(giftName);
					_len += 2 + _strBytes.Length;
				}
				// tagType
				_len += 1;
				// sortIdx
				_len += 4;
				// hotSortIdx
				_len += 4;
				// goodsSubType
				_len += 2;
				// isRecommend
				_len += 1;
				// isPersonalTailor
				_len += 1;
				// discountRate
				_len += 4;
				// loginPushId
				_len += 1;
				// fashionImagePath
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(fashionImagePath);
					_len += 2 + _strBytes.Length;
				}
				// giftDesc
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(giftDesc);
					_len += 2 + _strBytes.Length;
				}
				// buyGotInfos
				_len += 2;
				for(int j = 0; j < buyGotInfos.Length; j++)
				{
					_len += buyGotInfos[j].getLen();
				}
				// extParams
				_len += 2 + 4 * extParams.Length;
				// accountRefreshType
				_len += 1;
				// accountLimitBuyNum
				_len += 4;
				// accountRestBuyNum
				_len += 4;
				// discountCouponId
				_len += 4;
				// multiple
				_len += 1;
				// multipleEndTime
				_len += 4;
				// deductionCouponId
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  夺宝参与者数据
	/// </summary>
	public class GambingParticipantInfo : Protocol.IProtocolStream
	{
		/// <summary>
		///  roleid
		/// </summary>
		public UInt64 participantId;
		/// <summary>
		///  参与者平台(英文)
		/// </summary>
		public string participantENPlatform;
		/// <summary>
		///  参与者平台(中文)
		/// </summary>
		public string participantPlatform;
		/// <summary>
		///  参与者服务器
		/// </summary>
		public string participantServerName;
		/// <summary>
		///  名字
		/// </summary>
		public string participantName;
		/// <summary>
		///  夺宝商品id
		/// </summary>
		public UInt32 gambingItemId;
		/// <summary>
		///  夺宝组id
		/// </summary>
		public UInt16 groupId;
		/// <summary>
		///  投入份数
		/// </summary>
		public UInt32 investCopies;
		/// <summary>
		///  投入货币id
		/// </summary>
		public UInt32 investMoneyId;
		/// <summary>
		///  投入货币数量
		/// </summary>
		public UInt32 investMoney;
		/// <summary>
		///  夺宝几率
		/// </summary>
		public string gambingRate;
		/// <summary>
		///  状态(对应枚举 GambingMineStatus)
		/// </summary>
		public byte status;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, participantId);
				byte[] participantENPlatformBytes = StringHelper.StringToUTF8Bytes(participantENPlatform);
				BaseDLL.encode_string(buffer, ref pos_, participantENPlatformBytes, (UInt16)(buffer.Length - pos_));
				byte[] participantPlatformBytes = StringHelper.StringToUTF8Bytes(participantPlatform);
				BaseDLL.encode_string(buffer, ref pos_, participantPlatformBytes, (UInt16)(buffer.Length - pos_));
				byte[] participantServerNameBytes = StringHelper.StringToUTF8Bytes(participantServerName);
				BaseDLL.encode_string(buffer, ref pos_, participantServerNameBytes, (UInt16)(buffer.Length - pos_));
				byte[] participantNameBytes = StringHelper.StringToUTF8Bytes(participantName);
				BaseDLL.encode_string(buffer, ref pos_, participantNameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint32(buffer, ref pos_, gambingItemId);
				BaseDLL.encode_uint16(buffer, ref pos_, groupId);
				BaseDLL.encode_uint32(buffer, ref pos_, investCopies);
				BaseDLL.encode_uint32(buffer, ref pos_, investMoneyId);
				BaseDLL.encode_uint32(buffer, ref pos_, investMoney);
				byte[] gambingRateBytes = StringHelper.StringToUTF8Bytes(gambingRate);
				BaseDLL.encode_string(buffer, ref pos_, gambingRateBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, status);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref participantId);
				UInt16 participantENPlatformLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref participantENPlatformLen);
				byte[] participantENPlatformBytes = new byte[participantENPlatformLen];
				for(int i = 0; i < participantENPlatformLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref participantENPlatformBytes[i]);
				}
				participantENPlatform = StringHelper.BytesToString(participantENPlatformBytes);
				UInt16 participantPlatformLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref participantPlatformLen);
				byte[] participantPlatformBytes = new byte[participantPlatformLen];
				for(int i = 0; i < participantPlatformLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref participantPlatformBytes[i]);
				}
				participantPlatform = StringHelper.BytesToString(participantPlatformBytes);
				UInt16 participantServerNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref participantServerNameLen);
				byte[] participantServerNameBytes = new byte[participantServerNameLen];
				for(int i = 0; i < participantServerNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref participantServerNameBytes[i]);
				}
				participantServerName = StringHelper.BytesToString(participantServerNameBytes);
				UInt16 participantNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref participantNameLen);
				byte[] participantNameBytes = new byte[participantNameLen];
				for(int i = 0; i < participantNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref participantNameBytes[i]);
				}
				participantName = StringHelper.BytesToString(participantNameBytes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref gambingItemId);
				BaseDLL.decode_uint16(buffer, ref pos_, ref groupId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref investCopies);
				BaseDLL.decode_uint32(buffer, ref pos_, ref investMoneyId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref investMoney);
				UInt16 gambingRateLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref gambingRateLen);
				byte[] gambingRateBytes = new byte[gambingRateLen];
				for(int i = 0; i < gambingRateLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref gambingRateBytes[i]);
				}
				gambingRate = StringHelper.BytesToString(gambingRateBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref status);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, participantId);
				byte[] participantENPlatformBytes = StringHelper.StringToUTF8Bytes(participantENPlatform);
				BaseDLL.encode_string(buffer, ref pos_, participantENPlatformBytes, (UInt16)(buffer.Length - pos_));
				byte[] participantPlatformBytes = StringHelper.StringToUTF8Bytes(participantPlatform);
				BaseDLL.encode_string(buffer, ref pos_, participantPlatformBytes, (UInt16)(buffer.Length - pos_));
				byte[] participantServerNameBytes = StringHelper.StringToUTF8Bytes(participantServerName);
				BaseDLL.encode_string(buffer, ref pos_, participantServerNameBytes, (UInt16)(buffer.Length - pos_));
				byte[] participantNameBytes = StringHelper.StringToUTF8Bytes(participantName);
				BaseDLL.encode_string(buffer, ref pos_, participantNameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint32(buffer, ref pos_, gambingItemId);
				BaseDLL.encode_uint16(buffer, ref pos_, groupId);
				BaseDLL.encode_uint32(buffer, ref pos_, investCopies);
				BaseDLL.encode_uint32(buffer, ref pos_, investMoneyId);
				BaseDLL.encode_uint32(buffer, ref pos_, investMoney);
				byte[] gambingRateBytes = StringHelper.StringToUTF8Bytes(gambingRate);
				BaseDLL.encode_string(buffer, ref pos_, gambingRateBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, status);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref participantId);
				UInt16 participantENPlatformLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref participantENPlatformLen);
				byte[] participantENPlatformBytes = new byte[participantENPlatformLen];
				for(int i = 0; i < participantENPlatformLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref participantENPlatformBytes[i]);
				}
				participantENPlatform = StringHelper.BytesToString(participantENPlatformBytes);
				UInt16 participantPlatformLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref participantPlatformLen);
				byte[] participantPlatformBytes = new byte[participantPlatformLen];
				for(int i = 0; i < participantPlatformLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref participantPlatformBytes[i]);
				}
				participantPlatform = StringHelper.BytesToString(participantPlatformBytes);
				UInt16 participantServerNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref participantServerNameLen);
				byte[] participantServerNameBytes = new byte[participantServerNameLen];
				for(int i = 0; i < participantServerNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref participantServerNameBytes[i]);
				}
				participantServerName = StringHelper.BytesToString(participantServerNameBytes);
				UInt16 participantNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref participantNameLen);
				byte[] participantNameBytes = new byte[participantNameLen];
				for(int i = 0; i < participantNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref participantNameBytes[i]);
				}
				participantName = StringHelper.BytesToString(participantNameBytes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref gambingItemId);
				BaseDLL.decode_uint16(buffer, ref pos_, ref groupId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref investCopies);
				BaseDLL.decode_uint32(buffer, ref pos_, ref investMoneyId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref investMoney);
				UInt16 gambingRateLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref gambingRateLen);
				byte[] gambingRateBytes = new byte[gambingRateLen];
				for(int i = 0; i < gambingRateLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref gambingRateBytes[i]);
				}
				gambingRate = StringHelper.BytesToString(gambingRateBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref status);
			}

			public int getLen()
			{
				int _len = 0;
				// participantId
				_len += 8;
				// participantENPlatform
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(participantENPlatform);
					_len += 2 + _strBytes.Length;
				}
				// participantPlatform
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(participantPlatform);
					_len += 2 + _strBytes.Length;
				}
				// participantServerName
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(participantServerName);
					_len += 2 + _strBytes.Length;
				}
				// participantName
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(participantName);
					_len += 2 + _strBytes.Length;
				}
				// gambingItemId
				_len += 4;
				// groupId
				_len += 2;
				// investCopies
				_len += 4;
				// investMoneyId
				_len += 4;
				// investMoney
				_len += 4;
				// gambingRate
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(gambingRate);
					_len += 2 + _strBytes.Length;
				}
				// status
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  夺宝商品信息
	/// </summary>
	public class GambingItemInfo : Protocol.IProtocolStream
	{
		/// <summary>
		///  商品id
		/// </summary>
		public UInt32 gambingItemId;
		/// <summary>
		///  商品数量
		/// </summary>
		public UInt32 gambingItemNum;
		/// <summary>
		///  排序
		/// </summary>
		public UInt16 sortId;
		/// <summary>
		///  道具表id
		/// </summary>
		public UInt32 itemDataId;
		/// <summary>
		///  花费货币id
		/// </summary>
		public UInt32 costMoneyId;
		/// <summary>
		///  单价(一份)
		/// </summary>
		public UInt32 unitPrice;
		/// <summary>
		///  剩余组数
		/// </summary>
		public UInt16 restGroups;
		/// <summary>
		///  总组数
		/// </summary>
		public UInt16 totalGroups;
		/// <summary>
		///  每份奖励
		/// </summary>
		public ItemReward[] rewardsPerCopy = new ItemReward[0];
		/// <summary>
		///  当前组id
		/// </summary>
		public UInt16 curGroupId;
		/// <summary>
		///  当前组状态(对应枚举 GambingItemStatus)
		/// </summary>
		public byte statusOfCurGroup;
		/// <summary>
		///  当前组已售份数
		/// </summary>
		public UInt32 soldCopiesInCurGroup;
		/// <summary>
		///  当前组总份数
		/// </summary>
		public UInt32 totalCopiesOfCurGroup;
		/// <summary>
		///  当前组开售时间
		/// </summary>
		public UInt32 sellBeginTime;
		/// <summary>
		///  我的夺宝数据
		/// </summary>
		public GambingParticipantInfo mineGambingInfo = new GambingParticipantInfo();

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, gambingItemId);
				BaseDLL.encode_uint32(buffer, ref pos_, gambingItemNum);
				BaseDLL.encode_uint16(buffer, ref pos_, sortId);
				BaseDLL.encode_uint32(buffer, ref pos_, itemDataId);
				BaseDLL.encode_uint32(buffer, ref pos_, costMoneyId);
				BaseDLL.encode_uint32(buffer, ref pos_, unitPrice);
				BaseDLL.encode_uint16(buffer, ref pos_, restGroups);
				BaseDLL.encode_uint16(buffer, ref pos_, totalGroups);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)rewardsPerCopy.Length);
				for(int i = 0; i < rewardsPerCopy.Length; i++)
				{
					rewardsPerCopy[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_uint16(buffer, ref pos_, curGroupId);
				BaseDLL.encode_int8(buffer, ref pos_, statusOfCurGroup);
				BaseDLL.encode_uint32(buffer, ref pos_, soldCopiesInCurGroup);
				BaseDLL.encode_uint32(buffer, ref pos_, totalCopiesOfCurGroup);
				BaseDLL.encode_uint32(buffer, ref pos_, sellBeginTime);
				mineGambingInfo.encode(buffer, ref pos_);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref gambingItemId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref gambingItemNum);
				BaseDLL.decode_uint16(buffer, ref pos_, ref sortId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemDataId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref costMoneyId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref unitPrice);
				BaseDLL.decode_uint16(buffer, ref pos_, ref restGroups);
				BaseDLL.decode_uint16(buffer, ref pos_, ref totalGroups);
				UInt16 rewardsPerCopyCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref rewardsPerCopyCnt);
				rewardsPerCopy = new ItemReward[rewardsPerCopyCnt];
				for(int i = 0; i < rewardsPerCopy.Length; i++)
				{
					rewardsPerCopy[i] = new ItemReward();
					rewardsPerCopy[i].decode(buffer, ref pos_);
				}
				BaseDLL.decode_uint16(buffer, ref pos_, ref curGroupId);
				BaseDLL.decode_int8(buffer, ref pos_, ref statusOfCurGroup);
				BaseDLL.decode_uint32(buffer, ref pos_, ref soldCopiesInCurGroup);
				BaseDLL.decode_uint32(buffer, ref pos_, ref totalCopiesOfCurGroup);
				BaseDLL.decode_uint32(buffer, ref pos_, ref sellBeginTime);
				mineGambingInfo.decode(buffer, ref pos_);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, gambingItemId);
				BaseDLL.encode_uint32(buffer, ref pos_, gambingItemNum);
				BaseDLL.encode_uint16(buffer, ref pos_, sortId);
				BaseDLL.encode_uint32(buffer, ref pos_, itemDataId);
				BaseDLL.encode_uint32(buffer, ref pos_, costMoneyId);
				BaseDLL.encode_uint32(buffer, ref pos_, unitPrice);
				BaseDLL.encode_uint16(buffer, ref pos_, restGroups);
				BaseDLL.encode_uint16(buffer, ref pos_, totalGroups);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)rewardsPerCopy.Length);
				for(int i = 0; i < rewardsPerCopy.Length; i++)
				{
					rewardsPerCopy[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_uint16(buffer, ref pos_, curGroupId);
				BaseDLL.encode_int8(buffer, ref pos_, statusOfCurGroup);
				BaseDLL.encode_uint32(buffer, ref pos_, soldCopiesInCurGroup);
				BaseDLL.encode_uint32(buffer, ref pos_, totalCopiesOfCurGroup);
				BaseDLL.encode_uint32(buffer, ref pos_, sellBeginTime);
				mineGambingInfo.encode(buffer, ref pos_);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref gambingItemId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref gambingItemNum);
				BaseDLL.decode_uint16(buffer, ref pos_, ref sortId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemDataId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref costMoneyId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref unitPrice);
				BaseDLL.decode_uint16(buffer, ref pos_, ref restGroups);
				BaseDLL.decode_uint16(buffer, ref pos_, ref totalGroups);
				UInt16 rewardsPerCopyCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref rewardsPerCopyCnt);
				rewardsPerCopy = new ItemReward[rewardsPerCopyCnt];
				for(int i = 0; i < rewardsPerCopy.Length; i++)
				{
					rewardsPerCopy[i] = new ItemReward();
					rewardsPerCopy[i].decode(buffer, ref pos_);
				}
				BaseDLL.decode_uint16(buffer, ref pos_, ref curGroupId);
				BaseDLL.decode_int8(buffer, ref pos_, ref statusOfCurGroup);
				BaseDLL.decode_uint32(buffer, ref pos_, ref soldCopiesInCurGroup);
				BaseDLL.decode_uint32(buffer, ref pos_, ref totalCopiesOfCurGroup);
				BaseDLL.decode_uint32(buffer, ref pos_, ref sellBeginTime);
				mineGambingInfo.decode(buffer, ref pos_);
			}

			public int getLen()
			{
				int _len = 0;
				// gambingItemId
				_len += 4;
				// gambingItemNum
				_len += 4;
				// sortId
				_len += 2;
				// itemDataId
				_len += 4;
				// costMoneyId
				_len += 4;
				// unitPrice
				_len += 4;
				// restGroups
				_len += 2;
				// totalGroups
				_len += 2;
				// rewardsPerCopy
				_len += 2;
				for(int j = 0; j < rewardsPerCopy.Length; j++)
				{
					_len += rewardsPerCopy[j].getLen();
				}
				// curGroupId
				_len += 2;
				// statusOfCurGroup
				_len += 1;
				// soldCopiesInCurGroup
				_len += 4;
				// totalCopiesOfCurGroup
				_len += 4;
				// sellBeginTime
				_len += 4;
				// mineGambingInfo
				_len += mineGambingInfo.getLen();
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  我的夺宝信息
	/// </summary>
	public class GambingMineInfo : Protocol.IProtocolStream
	{
		/// <summary>
		///  商品id
		/// </summary>
		public UInt32 gambingItemId;
		/// <summary>
		///  商品数量
		/// </summary>
		public UInt32 gambingItemNum;
		/// <summary>
		///  排序
		/// </summary>
		public UInt16 sortId;
		/// <summary>
		///  道具表id
		/// </summary>
		public UInt32 itemDataId;
		/// <summary>
		///  组id
		/// </summary>
		public UInt16 groupId;
		/// <summary>
		///  已售出份数
		/// </summary>
		public UInt32 soldCopies;
		/// <summary>
		///  总份数
		/// </summary>
		public UInt32 totalCopies;
		/// <summary>
		///  我的夺宝数据
		/// </summary>
		public GambingParticipantInfo mineGambingInfo = new GambingParticipantInfo();
		/// <summary>
		///  获得者夺宝数据
		/// </summary>
		public GambingParticipantInfo gainersGambingInfo = new GambingParticipantInfo();
		/// <summary>
		///  参与者夺宝数据
		/// </summary>
		public GambingParticipantInfo[] participantsGambingInfo = new GambingParticipantInfo[0];

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, gambingItemId);
				BaseDLL.encode_uint32(buffer, ref pos_, gambingItemNum);
				BaseDLL.encode_uint16(buffer, ref pos_, sortId);
				BaseDLL.encode_uint32(buffer, ref pos_, itemDataId);
				BaseDLL.encode_uint16(buffer, ref pos_, groupId);
				BaseDLL.encode_uint32(buffer, ref pos_, soldCopies);
				BaseDLL.encode_uint32(buffer, ref pos_, totalCopies);
				mineGambingInfo.encode(buffer, ref pos_);
				gainersGambingInfo.encode(buffer, ref pos_);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)participantsGambingInfo.Length);
				for(int i = 0; i < participantsGambingInfo.Length; i++)
				{
					participantsGambingInfo[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref gambingItemId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref gambingItemNum);
				BaseDLL.decode_uint16(buffer, ref pos_, ref sortId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemDataId);
				BaseDLL.decode_uint16(buffer, ref pos_, ref groupId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref soldCopies);
				BaseDLL.decode_uint32(buffer, ref pos_, ref totalCopies);
				mineGambingInfo.decode(buffer, ref pos_);
				gainersGambingInfo.decode(buffer, ref pos_);
				UInt16 participantsGambingInfoCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref participantsGambingInfoCnt);
				participantsGambingInfo = new GambingParticipantInfo[participantsGambingInfoCnt];
				for(int i = 0; i < participantsGambingInfo.Length; i++)
				{
					participantsGambingInfo[i] = new GambingParticipantInfo();
					participantsGambingInfo[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, gambingItemId);
				BaseDLL.encode_uint32(buffer, ref pos_, gambingItemNum);
				BaseDLL.encode_uint16(buffer, ref pos_, sortId);
				BaseDLL.encode_uint32(buffer, ref pos_, itemDataId);
				BaseDLL.encode_uint16(buffer, ref pos_, groupId);
				BaseDLL.encode_uint32(buffer, ref pos_, soldCopies);
				BaseDLL.encode_uint32(buffer, ref pos_, totalCopies);
				mineGambingInfo.encode(buffer, ref pos_);
				gainersGambingInfo.encode(buffer, ref pos_);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)participantsGambingInfo.Length);
				for(int i = 0; i < participantsGambingInfo.Length; i++)
				{
					participantsGambingInfo[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref gambingItemId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref gambingItemNum);
				BaseDLL.decode_uint16(buffer, ref pos_, ref sortId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemDataId);
				BaseDLL.decode_uint16(buffer, ref pos_, ref groupId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref soldCopies);
				BaseDLL.decode_uint32(buffer, ref pos_, ref totalCopies);
				mineGambingInfo.decode(buffer, ref pos_);
				gainersGambingInfo.decode(buffer, ref pos_);
				UInt16 participantsGambingInfoCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref participantsGambingInfoCnt);
				participantsGambingInfo = new GambingParticipantInfo[participantsGambingInfoCnt];
				for(int i = 0; i < participantsGambingInfo.Length; i++)
				{
					participantsGambingInfo[i] = new GambingParticipantInfo();
					participantsGambingInfo[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// gambingItemId
				_len += 4;
				// gambingItemNum
				_len += 4;
				// sortId
				_len += 2;
				// itemDataId
				_len += 4;
				// groupId
				_len += 2;
				// soldCopies
				_len += 4;
				// totalCopies
				_len += 4;
				// mineGambingInfo
				_len += mineGambingInfo.getLen();
				// gainersGambingInfo
				_len += gainersGambingInfo.getLen();
				// participantsGambingInfo
				_len += 2;
				for(int j = 0; j < participantsGambingInfo.Length; j++)
				{
					_len += participantsGambingInfo[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 夺宝组记录
	/// </summary>
	public class GambingGroupRecordData : Protocol.IProtocolStream
	{
		/// <summary>
		///  组id
		/// </summary>
		public UInt16 groupId;
		/// <summary>
		///  赢家平台(英文)
		/// </summary>
		public string gainerENPlatform;
		/// <summary>
		///  赢家平台
		/// </summary>
		public string gainerPlatform;
		/// <summary>
		///  赢家服务器
		/// </summary>
		public string gainerServerName;
		/// <summary>
		///  赢家id
		/// </summary>
		public UInt64 gainerId;
		/// <summary>
		///  赢家名字
		/// </summary>
		public string gainerName;
		/// <summary>
		///  赢家投入货币id
		/// </summary>
		public UInt32 investCurrencyId;
		/// <summary>
		///  赢家投入货币数量
		/// </summary>
		public UInt32 investCurrencyNum;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, groupId);
				byte[] gainerENPlatformBytes = StringHelper.StringToUTF8Bytes(gainerENPlatform);
				BaseDLL.encode_string(buffer, ref pos_, gainerENPlatformBytes, (UInt16)(buffer.Length - pos_));
				byte[] gainerPlatformBytes = StringHelper.StringToUTF8Bytes(gainerPlatform);
				BaseDLL.encode_string(buffer, ref pos_, gainerPlatformBytes, (UInt16)(buffer.Length - pos_));
				byte[] gainerServerNameBytes = StringHelper.StringToUTF8Bytes(gainerServerName);
				BaseDLL.encode_string(buffer, ref pos_, gainerServerNameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint64(buffer, ref pos_, gainerId);
				byte[] gainerNameBytes = StringHelper.StringToUTF8Bytes(gainerName);
				BaseDLL.encode_string(buffer, ref pos_, gainerNameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint32(buffer, ref pos_, investCurrencyId);
				BaseDLL.encode_uint32(buffer, ref pos_, investCurrencyNum);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint16(buffer, ref pos_, ref groupId);
				UInt16 gainerENPlatformLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref gainerENPlatformLen);
				byte[] gainerENPlatformBytes = new byte[gainerENPlatformLen];
				for(int i = 0; i < gainerENPlatformLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref gainerENPlatformBytes[i]);
				}
				gainerENPlatform = StringHelper.BytesToString(gainerENPlatformBytes);
				UInt16 gainerPlatformLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref gainerPlatformLen);
				byte[] gainerPlatformBytes = new byte[gainerPlatformLen];
				for(int i = 0; i < gainerPlatformLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref gainerPlatformBytes[i]);
				}
				gainerPlatform = StringHelper.BytesToString(gainerPlatformBytes);
				UInt16 gainerServerNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref gainerServerNameLen);
				byte[] gainerServerNameBytes = new byte[gainerServerNameLen];
				for(int i = 0; i < gainerServerNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref gainerServerNameBytes[i]);
				}
				gainerServerName = StringHelper.BytesToString(gainerServerNameBytes);
				BaseDLL.decode_uint64(buffer, ref pos_, ref gainerId);
				UInt16 gainerNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref gainerNameLen);
				byte[] gainerNameBytes = new byte[gainerNameLen];
				for(int i = 0; i < gainerNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref gainerNameBytes[i]);
				}
				gainerName = StringHelper.BytesToString(gainerNameBytes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref investCurrencyId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref investCurrencyNum);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, groupId);
				byte[] gainerENPlatformBytes = StringHelper.StringToUTF8Bytes(gainerENPlatform);
				BaseDLL.encode_string(buffer, ref pos_, gainerENPlatformBytes, (UInt16)(buffer.Length - pos_));
				byte[] gainerPlatformBytes = StringHelper.StringToUTF8Bytes(gainerPlatform);
				BaseDLL.encode_string(buffer, ref pos_, gainerPlatformBytes, (UInt16)(buffer.Length - pos_));
				byte[] gainerServerNameBytes = StringHelper.StringToUTF8Bytes(gainerServerName);
				BaseDLL.encode_string(buffer, ref pos_, gainerServerNameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint64(buffer, ref pos_, gainerId);
				byte[] gainerNameBytes = StringHelper.StringToUTF8Bytes(gainerName);
				BaseDLL.encode_string(buffer, ref pos_, gainerNameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint32(buffer, ref pos_, investCurrencyId);
				BaseDLL.encode_uint32(buffer, ref pos_, investCurrencyNum);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint16(buffer, ref pos_, ref groupId);
				UInt16 gainerENPlatformLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref gainerENPlatformLen);
				byte[] gainerENPlatformBytes = new byte[gainerENPlatformLen];
				for(int i = 0; i < gainerENPlatformLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref gainerENPlatformBytes[i]);
				}
				gainerENPlatform = StringHelper.BytesToString(gainerENPlatformBytes);
				UInt16 gainerPlatformLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref gainerPlatformLen);
				byte[] gainerPlatformBytes = new byte[gainerPlatformLen];
				for(int i = 0; i < gainerPlatformLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref gainerPlatformBytes[i]);
				}
				gainerPlatform = StringHelper.BytesToString(gainerPlatformBytes);
				UInt16 gainerServerNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref gainerServerNameLen);
				byte[] gainerServerNameBytes = new byte[gainerServerNameLen];
				for(int i = 0; i < gainerServerNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref gainerServerNameBytes[i]);
				}
				gainerServerName = StringHelper.BytesToString(gainerServerNameBytes);
				BaseDLL.decode_uint64(buffer, ref pos_, ref gainerId);
				UInt16 gainerNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref gainerNameLen);
				byte[] gainerNameBytes = new byte[gainerNameLen];
				for(int i = 0; i < gainerNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref gainerNameBytes[i]);
				}
				gainerName = StringHelper.BytesToString(gainerNameBytes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref investCurrencyId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref investCurrencyNum);
			}

			public int getLen()
			{
				int _len = 0;
				// groupId
				_len += 2;
				// gainerENPlatform
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(gainerENPlatform);
					_len += 2 + _strBytes.Length;
				}
				// gainerPlatform
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(gainerPlatform);
					_len += 2 + _strBytes.Length;
				}
				// gainerServerName
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(gainerServerName);
					_len += 2 + _strBytes.Length;
				}
				// gainerId
				_len += 8;
				// gainerName
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(gainerName);
					_len += 2 + _strBytes.Length;
				}
				// investCurrencyId
				_len += 4;
				// investCurrencyNum
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 夺宝记录
	/// </summary>
	public class GambingItemRecordData : Protocol.IProtocolStream
	{
		/// <summary>
		///  商品id
		/// </summary>
		public UInt32 gambingItemId;
		/// <summary>
		///  商品数量
		/// </summary>
		public UInt32 gambingItemNum;
		/// <summary>
		///  排序
		/// </summary>
		public UInt16 sortId;
		/// <summary>
		///  道具表id
		/// </summary>
		public UInt32 itemDataId;
		/// <summary>
		///  售罄时间
		/// </summary>
		public UInt32 soldOutTimestamp;
		/// <summary>
		///  组记录
		/// </summary>
		public GambingGroupRecordData[] groupRecordData = new GambingGroupRecordData[0];

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, gambingItemId);
				BaseDLL.encode_uint32(buffer, ref pos_, gambingItemNum);
				BaseDLL.encode_uint16(buffer, ref pos_, sortId);
				BaseDLL.encode_uint32(buffer, ref pos_, itemDataId);
				BaseDLL.encode_uint32(buffer, ref pos_, soldOutTimestamp);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)groupRecordData.Length);
				for(int i = 0; i < groupRecordData.Length; i++)
				{
					groupRecordData[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref gambingItemId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref gambingItemNum);
				BaseDLL.decode_uint16(buffer, ref pos_, ref sortId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemDataId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref soldOutTimestamp);
				UInt16 groupRecordDataCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref groupRecordDataCnt);
				groupRecordData = new GambingGroupRecordData[groupRecordDataCnt];
				for(int i = 0; i < groupRecordData.Length; i++)
				{
					groupRecordData[i] = new GambingGroupRecordData();
					groupRecordData[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, gambingItemId);
				BaseDLL.encode_uint32(buffer, ref pos_, gambingItemNum);
				BaseDLL.encode_uint16(buffer, ref pos_, sortId);
				BaseDLL.encode_uint32(buffer, ref pos_, itemDataId);
				BaseDLL.encode_uint32(buffer, ref pos_, soldOutTimestamp);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)groupRecordData.Length);
				for(int i = 0; i < groupRecordData.Length; i++)
				{
					groupRecordData[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref gambingItemId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref gambingItemNum);
				BaseDLL.decode_uint16(buffer, ref pos_, ref sortId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemDataId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref soldOutTimestamp);
				UInt16 groupRecordDataCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref groupRecordDataCnt);
				groupRecordData = new GambingGroupRecordData[groupRecordDataCnt];
				for(int i = 0; i < groupRecordData.Length; i++)
				{
					groupRecordData[i] = new GambingGroupRecordData();
					groupRecordData[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// gambingItemId
				_len += 4;
				// gambingItemNum
				_len += 4;
				// sortId
				_len += 2;
				// itemDataId
				_len += 4;
				// soldOutTimestamp
				_len += 4;
				// groupRecordData
				_len += 2;
				for(int j = 0; j < groupRecordData.Length; j++)
				{
					_len += groupRecordData[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	public class GiftSyncInfo : Protocol.IProtocolStream
	{
		public UInt32 id;
		public UInt32 itemId;
		public UInt32 itemNum;
		public byte[] recommendJobs = new byte[0];
		public UInt32 weight;
		public UInt32[] validLevels = new UInt32[0];
		/// <summary>
		/// 装备类型，对应枚举EquipType
		/// </summary>
		public byte equipType;
		public UInt32 strengthen;
		public byte isTimeLimit;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, id);
				BaseDLL.encode_uint32(buffer, ref pos_, itemId);
				BaseDLL.encode_uint32(buffer, ref pos_, itemNum);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)recommendJobs.Length);
				for(int i = 0; i < recommendJobs.Length; i++)
				{
					BaseDLL.encode_int8(buffer, ref pos_, recommendJobs[i]);
				}
				BaseDLL.encode_uint32(buffer, ref pos_, weight);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)validLevels.Length);
				for(int i = 0; i < validLevels.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, validLevels[i]);
				}
				BaseDLL.encode_int8(buffer, ref pos_, equipType);
				BaseDLL.encode_uint32(buffer, ref pos_, strengthen);
				BaseDLL.encode_int8(buffer, ref pos_, isTimeLimit);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemNum);
				UInt16 recommendJobsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref recommendJobsCnt);
				recommendJobs = new byte[recommendJobsCnt];
				for(int i = 0; i < recommendJobs.Length; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref recommendJobs[i]);
				}
				BaseDLL.decode_uint32(buffer, ref pos_, ref weight);
				UInt16 validLevelsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref validLevelsCnt);
				validLevels = new UInt32[validLevelsCnt];
				for(int i = 0; i < validLevels.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref validLevels[i]);
				}
				BaseDLL.decode_int8(buffer, ref pos_, ref equipType);
				BaseDLL.decode_uint32(buffer, ref pos_, ref strengthen);
				BaseDLL.decode_int8(buffer, ref pos_, ref isTimeLimit);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, id);
				BaseDLL.encode_uint32(buffer, ref pos_, itemId);
				BaseDLL.encode_uint32(buffer, ref pos_, itemNum);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)recommendJobs.Length);
				for(int i = 0; i < recommendJobs.Length; i++)
				{
					BaseDLL.encode_int8(buffer, ref pos_, recommendJobs[i]);
				}
				BaseDLL.encode_uint32(buffer, ref pos_, weight);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)validLevels.Length);
				for(int i = 0; i < validLevels.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, validLevels[i]);
				}
				BaseDLL.encode_int8(buffer, ref pos_, equipType);
				BaseDLL.encode_uint32(buffer, ref pos_, strengthen);
				BaseDLL.encode_int8(buffer, ref pos_, isTimeLimit);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemNum);
				UInt16 recommendJobsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref recommendJobsCnt);
				recommendJobs = new byte[recommendJobsCnt];
				for(int i = 0; i < recommendJobs.Length; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref recommendJobs[i]);
				}
				BaseDLL.decode_uint32(buffer, ref pos_, ref weight);
				UInt16 validLevelsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref validLevelsCnt);
				validLevels = new UInt32[validLevelsCnt];
				for(int i = 0; i < validLevels.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref validLevels[i]);
				}
				BaseDLL.decode_int8(buffer, ref pos_, ref equipType);
				BaseDLL.decode_uint32(buffer, ref pos_, ref strengthen);
				BaseDLL.decode_int8(buffer, ref pos_, ref isTimeLimit);
			}

			public int getLen()
			{
				int _len = 0;
				// id
				_len += 4;
				// itemId
				_len += 4;
				// itemNum
				_len += 4;
				// recommendJobs
				_len += 2 + 1 * recommendJobs.Length;
				// weight
				_len += 4;
				// validLevels
				_len += 2 + 4 * validLevels.Length;
				// equipType
				_len += 1;
				// strengthen
				_len += 4;
				// isTimeLimit
				_len += 1;
				return _len;
			}
		#endregion

	}

	public class GiftPackSyncInfo : Protocol.IProtocolStream
	{
		public UInt32 id;
		public UInt32 filterType;
		public Int32 filterCount;
		public GiftSyncInfo[] gifts = new GiftSyncInfo[0];
		public byte uiType;
		public string description;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, id);
				BaseDLL.encode_uint32(buffer, ref pos_, filterType);
				BaseDLL.encode_int32(buffer, ref pos_, filterCount);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)gifts.Length);
				for(int i = 0; i < gifts.Length; i++)
				{
					gifts[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_int8(buffer, ref pos_, uiType);
				byte[] descriptionBytes = StringHelper.StringToUTF8Bytes(description);
				BaseDLL.encode_string(buffer, ref pos_, descriptionBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
				BaseDLL.decode_uint32(buffer, ref pos_, ref filterType);
				BaseDLL.decode_int32(buffer, ref pos_, ref filterCount);
				UInt16 giftsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref giftsCnt);
				gifts = new GiftSyncInfo[giftsCnt];
				for(int i = 0; i < gifts.Length; i++)
				{
					gifts[i] = new GiftSyncInfo();
					gifts[i].decode(buffer, ref pos_);
				}
				BaseDLL.decode_int8(buffer, ref pos_, ref uiType);
				UInt16 descriptionLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref descriptionLen);
				byte[] descriptionBytes = new byte[descriptionLen];
				for(int i = 0; i < descriptionLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref descriptionBytes[i]);
				}
				description = StringHelper.BytesToString(descriptionBytes);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, id);
				BaseDLL.encode_uint32(buffer, ref pos_, filterType);
				BaseDLL.encode_int32(buffer, ref pos_, filterCount);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)gifts.Length);
				for(int i = 0; i < gifts.Length; i++)
				{
					gifts[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_int8(buffer, ref pos_, uiType);
				byte[] descriptionBytes = StringHelper.StringToUTF8Bytes(description);
				BaseDLL.encode_string(buffer, ref pos_, descriptionBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
				BaseDLL.decode_uint32(buffer, ref pos_, ref filterType);
				BaseDLL.decode_int32(buffer, ref pos_, ref filterCount);
				UInt16 giftsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref giftsCnt);
				gifts = new GiftSyncInfo[giftsCnt];
				for(int i = 0; i < gifts.Length; i++)
				{
					gifts[i] = new GiftSyncInfo();
					gifts[i].decode(buffer, ref pos_);
				}
				BaseDLL.decode_int8(buffer, ref pos_, ref uiType);
				UInt16 descriptionLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref descriptionLen);
				byte[] descriptionBytes = new byte[descriptionLen];
				for(int i = 0; i < descriptionLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref descriptionBytes[i]);
				}
				description = StringHelper.BytesToString(descriptionBytes);
			}

			public int getLen()
			{
				int _len = 0;
				// id
				_len += 4;
				// filterType
				_len += 4;
				// filterCount
				_len += 4;
				// gifts
				_len += 2;
				for(int j = 0; j < gifts.Length; j++)
				{
					_len += gifts[j].getLen();
				}
				// uiType
				_len += 1;
				// description
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(description);
					_len += 2 + _strBytes.Length;
				}
				return _len;
			}
		#endregion

	}

	public class ProtoShopItem : Protocol.IProtocolStream
	{
		/// <summary>
		/// 商店道具id
		/// </summary>
		public UInt32 shopItemId;
		/// <summary>
		/// 格子
		/// </summary>
		public byte grid;
		/// <summary>
		/// 剩余数量
		/// </summary>
		public Int16 restNum;
		/// <summary>
		/// 次数限制
		/// </summary>
		public Int16 limiteNum;
		/// <summary>
		/// vip等级
		/// </summary>
		public byte vipLv;
		/// <summary>
		/// vip折扣
		/// </summary>
		public byte vipDiscount;
		/// <summary>
		/// 折扣率
		/// </summary>
		public UInt32 discount;
		/// <summary>
		/// 租赁结束时间戳
		/// </summary>
		public UInt32 leaseEndTimeStamp;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, shopItemId);
				BaseDLL.encode_int8(buffer, ref pos_, grid);
				BaseDLL.encode_int16(buffer, ref pos_, restNum);
				BaseDLL.encode_int16(buffer, ref pos_, limiteNum);
				BaseDLL.encode_int8(buffer, ref pos_, vipLv);
				BaseDLL.encode_int8(buffer, ref pos_, vipDiscount);
				BaseDLL.encode_uint32(buffer, ref pos_, discount);
				BaseDLL.encode_uint32(buffer, ref pos_, leaseEndTimeStamp);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref shopItemId);
				BaseDLL.decode_int8(buffer, ref pos_, ref grid);
				BaseDLL.decode_int16(buffer, ref pos_, ref restNum);
				BaseDLL.decode_int16(buffer, ref pos_, ref limiteNum);
				BaseDLL.decode_int8(buffer, ref pos_, ref vipLv);
				BaseDLL.decode_int8(buffer, ref pos_, ref vipDiscount);
				BaseDLL.decode_uint32(buffer, ref pos_, ref discount);
				BaseDLL.decode_uint32(buffer, ref pos_, ref leaseEndTimeStamp);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, shopItemId);
				BaseDLL.encode_int8(buffer, ref pos_, grid);
				BaseDLL.encode_int16(buffer, ref pos_, restNum);
				BaseDLL.encode_int16(buffer, ref pos_, limiteNum);
				BaseDLL.encode_int8(buffer, ref pos_, vipLv);
				BaseDLL.encode_int8(buffer, ref pos_, vipDiscount);
				BaseDLL.encode_uint32(buffer, ref pos_, discount);
				BaseDLL.encode_uint32(buffer, ref pos_, leaseEndTimeStamp);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref shopItemId);
				BaseDLL.decode_int8(buffer, ref pos_, ref grid);
				BaseDLL.decode_int16(buffer, ref pos_, ref restNum);
				BaseDLL.decode_int16(buffer, ref pos_, ref limiteNum);
				BaseDLL.decode_int8(buffer, ref pos_, ref vipLv);
				BaseDLL.decode_int8(buffer, ref pos_, ref vipDiscount);
				BaseDLL.decode_uint32(buffer, ref pos_, ref discount);
				BaseDLL.decode_uint32(buffer, ref pos_, ref leaseEndTimeStamp);
			}

			public int getLen()
			{
				int _len = 0;
				// shopItemId
				_len += 4;
				// grid
				_len += 1;
				// restNum
				_len += 2;
				// limiteNum
				_len += 2;
				// vipLv
				_len += 1;
				// vipDiscount
				_len += 1;
				// discount
				_len += 4;
				// leaseEndTimeStamp
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneSynItem : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500905;
		public UInt32 Sequence;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
			}

			public void decode(byte[] buffer, ref int pos_)
			{
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
			}

			public int getLen()
			{
				int _len = 0;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneSyncItemProp : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500906;
		public UInt32 Sequence;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
			}

			public void decode(byte[] buffer, ref int pos_)
			{
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
			}

			public int getLen()
			{
				int _len = 0;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneNotifyDeleteItem : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500907;
		public UInt32 Sequence;
		public UInt64 uid;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, uid);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref uid);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, uid);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref uid);
			}

			public int getLen()
			{
				int _len = 0;
				// uid
				_len += 8;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneUseItem : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500901;
		public UInt32 Sequence;
		public UInt64 uid;
		public byte useAll;
		public UInt32 param1;
		public UInt32 param2;
		public UInt64 uid1;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, uid);
				BaseDLL.encode_int8(buffer, ref pos_, useAll);
				BaseDLL.encode_uint32(buffer, ref pos_, param1);
				BaseDLL.encode_uint32(buffer, ref pos_, param2);
				BaseDLL.encode_uint64(buffer, ref pos_, uid1);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref uid);
				BaseDLL.decode_int8(buffer, ref pos_, ref useAll);
				BaseDLL.decode_uint32(buffer, ref pos_, ref param1);
				BaseDLL.decode_uint32(buffer, ref pos_, ref param2);
				BaseDLL.decode_uint64(buffer, ref pos_, ref uid1);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, uid);
				BaseDLL.encode_int8(buffer, ref pos_, useAll);
				BaseDLL.encode_uint32(buffer, ref pos_, param1);
				BaseDLL.encode_uint32(buffer, ref pos_, param2);
				BaseDLL.encode_uint64(buffer, ref pos_, uid1);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref uid);
				BaseDLL.decode_int8(buffer, ref pos_, ref useAll);
				BaseDLL.decode_uint32(buffer, ref pos_, ref param1);
				BaseDLL.decode_uint32(buffer, ref pos_, ref param2);
				BaseDLL.decode_uint64(buffer, ref pos_, ref uid1);
			}

			public int getLen()
			{
				int _len = 0;
				// uid
				_len += 8;
				// useAll
				_len += 1;
				// param1
				_len += 4;
				// param2
				_len += 4;
				// uid1
				_len += 8;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneUseItemRet : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500902;
		public UInt32 Sequence;
		public UInt32 code;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			}

			public int getLen()
			{
				int _len = 0;
				// code
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneSellItem : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500903;
		public UInt32 Sequence;
		public UInt64 uid;
		public UInt16 num;
		public UInt64 uid1;
		public UInt16 num1;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, uid);
				BaseDLL.encode_uint16(buffer, ref pos_, num);

				BaseDLL.encode_uint64(buffer, ref pos_, uid1);
				BaseDLL.encode_uint16(buffer, ref pos_, num1);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref uid);
				BaseDLL.decode_uint16(buffer, ref pos_, ref num);

				BaseDLL.decode_uint64(buffer, ref pos_, ref uid1);
				BaseDLL.decode_uint16(buffer, ref pos_, ref num1);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, uid);
				BaseDLL.encode_uint16(buffer, ref pos_, num);

				BaseDLL.encode_uint64(buffer, ref pos_, uid1);
				BaseDLL.encode_uint16(buffer, ref pos_, num1);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref uid);
				BaseDLL.decode_uint16(buffer, ref pos_, ref num);

				BaseDLL.decode_uint64(buffer, ref pos_, ref uid1);
				BaseDLL.decode_uint16(buffer, ref pos_, ref num1);
			}

			public int getLen()
			{
				int _len = 0;
				// uid
				_len += 8;
				// num
				_len += 2;
				// uid1
				_len += 8;
				// num1
				_len += 2;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneSellItemRet : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500904;
		public UInt32 Sequence;
		public UInt32 code;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			}

			public int getLen()
			{
				int _len = 0;
				// code
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneEnlargePackage : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500908;
		public UInt32 Sequence;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
			}

			public void decode(byte[] buffer, ref int pos_)
			{
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
			}

			public int getLen()
			{
				int _len = 0;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneEnlargePackageRet : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500917;
		public UInt32 Sequence;
		public UInt32 code;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			}

			public int getLen()
			{
				int _len = 0;
				// code
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class ScenePushStorage : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500909;
		public UInt32 Sequence;
		public UInt64 uid;
		public UInt16 num;
		public ItemPos targetPos = new ItemPos();

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, uid);
				BaseDLL.encode_uint16(buffer, ref pos_, num);
				targetPos.encode(buffer, ref pos_);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref uid);
				BaseDLL.decode_uint16(buffer, ref pos_, ref num);
				targetPos.decode(buffer, ref pos_);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, uid);
				BaseDLL.encode_uint16(buffer, ref pos_, num);
				targetPos.encode(buffer, ref pos_);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref uid);
				BaseDLL.decode_uint16(buffer, ref pos_, ref num);
				targetPos.decode(buffer, ref pos_);
			}

			public int getLen()
			{
				int _len = 0;
				// uid
				_len += 8;
				// num
				_len += 2;
				// targetPos
				_len += targetPos.getLen();
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class ScenePushStorageRet : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500911;
		public UInt32 Sequence;
		public UInt32 code;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			}

			public int getLen()
			{
				int _len = 0;
				// code
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class ScenePullStorage : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500910;
		public UInt32 Sequence;
		public UInt64 uid;
		public UInt16 num;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, uid);
				BaseDLL.encode_uint16(buffer, ref pos_, num);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref uid);
				BaseDLL.decode_uint16(buffer, ref pos_, ref num);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, uid);
				BaseDLL.encode_uint16(buffer, ref pos_, num);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref uid);
				BaseDLL.decode_uint16(buffer, ref pos_, ref num);
			}

			public int getLen()
			{
				int _len = 0;
				// uid
				_len += 8;
				// num
				_len += 2;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class ScenePullStorageRet : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500912;
		public UInt32 Sequence;
		public UInt32 code;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			}

			public int getLen()
			{
				int _len = 0;
				// code
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneEnlargeStorage : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500913;
		public UInt32 Sequence;
		public byte itemEnlargeType;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, itemEnlargeType);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref itemEnlargeType);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, itemEnlargeType);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref itemEnlargeType);
			}

			public int getLen()
			{
				int _len = 0;
				// itemEnlargeType
				_len += 1;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneTrimItem : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500914;
		public UInt32 Sequence;
		public byte pack;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, pack);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref pack);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, pack);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref pack);
			}

			public int getLen()
			{
				int _len = 0;
				// pack
				_len += 1;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneTrimItemRet : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500915;
		public UInt32 Sequence;
		public UInt32 code;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			}

			public int getLen()
			{
				int _len = 0;
				// code
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneNotifyGetItem : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500916;
		public UInt32 Sequence;
		public UInt32 sourceType;
		public byte notify;
		public ItemReward[] items = new ItemReward[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, sourceType);
				BaseDLL.encode_int8(buffer, ref pos_, notify);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)items.Length);
				for(int i = 0; i < items.Length; i++)
				{
					items[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref sourceType);
				BaseDLL.decode_int8(buffer, ref pos_, ref notify);
				UInt16 itemsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref itemsCnt);
				items = new ItemReward[itemsCnt];
				for(int i = 0; i < items.Length; i++)
				{
					items[i] = new ItemReward();
					items[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, sourceType);
				BaseDLL.encode_int8(buffer, ref pos_, notify);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)items.Length);
				for(int i = 0; i < items.Length; i++)
				{
					items[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref sourceType);
				BaseDLL.decode_int8(buffer, ref pos_, ref notify);
				UInt16 itemsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref itemsCnt);
				items = new ItemReward[itemsCnt];
				for(int i = 0; i < items.Length; i++)
				{
					items[i] = new ItemReward();
					items[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// sourceType
				_len += 4;
				// notify
				_len += 1;
				// items
				_len += 2;
				for(int j = 0; j < items.Length; j++)
				{
					_len += items[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneEquipDecompose : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500918;
		public UInt32 Sequence;
		public UInt64[] uids = new UInt64[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)uids.Length);
				for(int i = 0; i < uids.Length; i++)
				{
					BaseDLL.encode_uint64(buffer, ref pos_, uids[i]);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 uidsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref uidsCnt);
				uids = new UInt64[uidsCnt];
				for(int i = 0; i < uids.Length; i++)
				{
					BaseDLL.decode_uint64(buffer, ref pos_, ref uids[i]);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)uids.Length);
				for(int i = 0; i < uids.Length; i++)
				{
					BaseDLL.encode_uint64(buffer, ref pos_, uids[i]);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 uidsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref uidsCnt);
				uids = new UInt64[uidsCnt];
				for(int i = 0; i < uids.Length; i++)
				{
					BaseDLL.decode_uint64(buffer, ref pos_, ref uids[i]);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// uids
				_len += 2 + 8 * uids.Length;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneEquipDecomposeRet : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500919;
		public UInt32 Sequence;
		public UInt32 code;
		public ItemReward[] getItems = new ItemReward[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)getItems.Length);
				for(int i = 0; i < getItems.Length; i++)
				{
					getItems[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
				UInt16 getItemsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref getItemsCnt);
				getItems = new ItemReward[getItemsCnt];
				for(int i = 0; i < getItems.Length; i++)
				{
					getItems[i] = new ItemReward();
					getItems[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)getItems.Length);
				for(int i = 0; i < getItems.Length; i++)
				{
					getItems[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
				UInt16 getItemsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref getItemsCnt);
				getItems = new ItemReward[getItemsCnt];
				for(int i = 0; i < getItems.Length; i++)
				{
					getItems[i] = new ItemReward();
					getItems[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// code
				_len += 4;
				// getItems
				_len += 2;
				for(int j = 0; j < getItems.Length; j++)
				{
					_len += getItems[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 抽到的道具
	/// </summary>
	[Protocol]
	public class SceneEquipStrengthen : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500920;
		public UInt32 Sequence;
		public UInt64 euqipUid;
		public byte useUnbreak;
		public UInt64 strTickt;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, euqipUid);
				BaseDLL.encode_int8(buffer, ref pos_, useUnbreak);
				BaseDLL.encode_uint64(buffer, ref pos_, strTickt);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref euqipUid);
				BaseDLL.decode_int8(buffer, ref pos_, ref useUnbreak);
				BaseDLL.decode_uint64(buffer, ref pos_, ref strTickt);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, euqipUid);
				BaseDLL.encode_int8(buffer, ref pos_, useUnbreak);
				BaseDLL.encode_uint64(buffer, ref pos_, strTickt);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref euqipUid);
				BaseDLL.decode_int8(buffer, ref pos_, ref useUnbreak);
				BaseDLL.decode_uint64(buffer, ref pos_, ref strTickt);
			}

			public int getLen()
			{
				int _len = 0;
				// euqipUid
				_len += 8;
				// useUnbreak
				_len += 1;
				// strTickt
				_len += 8;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneEquipStrengthenRet : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500921;
		public UInt32 Sequence;
		public UInt32 code;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			}

			public int getLen()
			{
				int _len = 0;
				// code
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// ------------商店start------------------------------
	/// </summary>
	[Protocol]
	public class SceneShopQuery : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500922;
		public UInt32 Sequence;
		public byte shopId;
		public byte cache;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, shopId);
				BaseDLL.encode_int8(buffer, ref pos_, cache);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref shopId);
				BaseDLL.decode_int8(buffer, ref pos_, ref cache);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, shopId);
				BaseDLL.encode_int8(buffer, ref pos_, cache);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref shopId);
				BaseDLL.decode_int8(buffer, ref pos_, ref cache);
			}

			public int getLen()
			{
				int _len = 0;
				// shopId
				_len += 1;
				// cache
				_len += 1;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneShopQueryRet : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500923;
		public UInt32 Sequence;
		public UInt32 code;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			}

			public int getLen()
			{
				int _len = 0;
				// code
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneShopBuy : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500924;
		public UInt32 Sequence;
		public byte shopId;
		public UInt32 shopItemId;
		public UInt16 num;
		public ItemInfo[] costExtraItems = new ItemInfo[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, shopId);
				BaseDLL.encode_uint32(buffer, ref pos_, shopItemId);
				BaseDLL.encode_uint16(buffer, ref pos_, num);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)costExtraItems.Length);
				for(int i = 0; i < costExtraItems.Length; i++)
				{
					costExtraItems[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref shopId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref shopItemId);
				BaseDLL.decode_uint16(buffer, ref pos_, ref num);
				UInt16 costExtraItemsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref costExtraItemsCnt);
				costExtraItems = new ItemInfo[costExtraItemsCnt];
				for(int i = 0; i < costExtraItems.Length; i++)
				{
					costExtraItems[i] = new ItemInfo();
					costExtraItems[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, shopId);
				BaseDLL.encode_uint32(buffer, ref pos_, shopItemId);
				BaseDLL.encode_uint16(buffer, ref pos_, num);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)costExtraItems.Length);
				for(int i = 0; i < costExtraItems.Length; i++)
				{
					costExtraItems[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref shopId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref shopItemId);
				BaseDLL.decode_uint16(buffer, ref pos_, ref num);
				UInt16 costExtraItemsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref costExtraItemsCnt);
				costExtraItems = new ItemInfo[costExtraItemsCnt];
				for(int i = 0; i < costExtraItems.Length; i++)
				{
					costExtraItems[i] = new ItemInfo();
					costExtraItems[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// shopId
				_len += 1;
				// shopItemId
				_len += 4;
				// num
				_len += 2;
				// costExtraItems
				_len += 2;
				for(int j = 0; j < costExtraItems.Length; j++)
				{
					_len += costExtraItems[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneShopBuyRet : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500925;
		public UInt32 Sequence;
		public UInt32 code;
		public UInt32 shopItemId;
		public UInt16 newNum;
		public UInt32 leaseEndTimeStamp;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
				BaseDLL.encode_uint32(buffer, ref pos_, shopItemId);
				BaseDLL.encode_uint16(buffer, ref pos_, newNum);
				BaseDLL.encode_uint32(buffer, ref pos_, leaseEndTimeStamp);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
				BaseDLL.decode_uint32(buffer, ref pos_, ref shopItemId);
				BaseDLL.decode_uint16(buffer, ref pos_, ref newNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref leaseEndTimeStamp);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
				BaseDLL.encode_uint32(buffer, ref pos_, shopItemId);
				BaseDLL.encode_uint16(buffer, ref pos_, newNum);
				BaseDLL.encode_uint32(buffer, ref pos_, leaseEndTimeStamp);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
				BaseDLL.decode_uint32(buffer, ref pos_, ref shopItemId);
				BaseDLL.decode_uint16(buffer, ref pos_, ref newNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref leaseEndTimeStamp);
			}

			public int getLen()
			{
				int _len = 0;
				// code
				_len += 4;
				// shopItemId
				_len += 4;
				// newNum
				_len += 2;
				// leaseEndTimeStamp
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneShopSync : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500926;
		public UInt32 Sequence;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
			}

			public void decode(byte[] buffer, ref int pos_)
			{
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
			}

			public int getLen()
			{
				int _len = 0;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneShopItemSync : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500927;
		public UInt32 Sequence;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
			}

			public void decode(byte[] buffer, ref int pos_)
			{
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
			}

			public int getLen()
			{
				int _len = 0;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneShopRefresh : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500932;
		public UInt32 Sequence;
		public byte shopId;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, shopId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref shopId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, shopId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref shopId);
			}

			public int getLen()
			{
				int _len = 0;
				// shopId
				_len += 1;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneShopRefreshRet : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500933;
		public UInt32 Sequence;
		public UInt32 code;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			}

			public int getLen()
			{
				int _len = 0;
				// code
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneShopRefreshNumReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500956;
		public UInt32 Sequence;
		public byte shopId;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, shopId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref shopId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, shopId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref shopId);
			}

			public int getLen()
			{
				int _len = 0;
				// shopId
				_len += 1;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneShopRefreshNumRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500957;
		public UInt32 Sequence;
		public byte shopId;
		public byte restRefreshNum;
		public byte maxRefreshNum;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, shopId);
				BaseDLL.encode_int8(buffer, ref pos_, restRefreshNum);
				BaseDLL.encode_int8(buffer, ref pos_, maxRefreshNum);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref shopId);
				BaseDLL.decode_int8(buffer, ref pos_, ref restRefreshNum);
				BaseDLL.decode_int8(buffer, ref pos_, ref maxRefreshNum);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, shopId);
				BaseDLL.encode_int8(buffer, ref pos_, restRefreshNum);
				BaseDLL.encode_int8(buffer, ref pos_, maxRefreshNum);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref shopId);
				BaseDLL.decode_int8(buffer, ref pos_, ref restRefreshNum);
				BaseDLL.decode_int8(buffer, ref pos_, ref maxRefreshNum);
			}

			public int getLen()
			{
				int _len = 0;
				// shopId
				_len += 1;
				// restRefreshNum
				_len += 1;
				// maxRefreshNum
				_len += 1;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneSyncMysticalMerchant : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501020;
		public UInt32 Sequence;
		/// <summary>
		/// 神秘商人id
		/// </summary>
		public UInt32 id;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, id);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, id);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
			}

			public int getLen()
			{
				int _len = 0;
				// id
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// ------------商店end------------------------------
	/// </summary>
	[Protocol]
	public class SceneOneKeyPushStorage : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500928;
		public UInt32 Sequence;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
			}

			public void decode(byte[] buffer, ref int pos_)
			{
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
			}

			public int getLen()
			{
				int _len = 0;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneOneKeyPushStorageRet : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500929;
		public UInt32 Sequence;
		public UInt32 code;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			}

			public int getLen()
			{
				int _len = 0;
				// code
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneEnlargeStorageRet : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500930;
		public UInt32 Sequence;
		public byte itemEnlargeType;
		public UInt32 code;
		public UInt32 curOpenNum;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, itemEnlargeType);
				BaseDLL.encode_uint32(buffer, ref pos_, code);
				BaseDLL.encode_uint32(buffer, ref pos_, curOpenNum);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref itemEnlargeType);
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
				BaseDLL.decode_uint32(buffer, ref pos_, ref curOpenNum);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, itemEnlargeType);
				BaseDLL.encode_uint32(buffer, ref pos_, code);
				BaseDLL.encode_uint32(buffer, ref pos_, curOpenNum);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref itemEnlargeType);
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
				BaseDLL.decode_uint32(buffer, ref pos_, ref curOpenNum);
			}

			public int getLen()
			{
				int _len = 0;
				// itemEnlargeType
				_len += 1;
				// code
				_len += 4;
				// curOpenNum
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneUpdateNewItem : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500931;
		public UInt32 Sequence;
		public byte pack;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, pack);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref pack);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, pack);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref pack);
			}

			public int getLen()
			{
				int _len = 0;
				// pack
				_len += 1;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneDungeonUseItem : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500934;
		public UInt32 Sequence;
		public UInt32 itemid;
		public UInt16 num;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, itemid);
				BaseDLL.encode_uint16(buffer, ref pos_, num);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemid);
				BaseDLL.decode_uint16(buffer, ref pos_, ref num);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, itemid);
				BaseDLL.encode_uint16(buffer, ref pos_, num);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemid);
				BaseDLL.decode_uint16(buffer, ref pos_, ref num);
			}

			public int getLen()
			{
				int _len = 0;
				// itemid
				_len += 4;
				// num
				_len += 2;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneSealEquipReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500937;
		public UInt32 Sequence;
		public UInt64 uid;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, uid);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref uid);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, uid);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref uid);
			}

			public int getLen()
			{
				int _len = 0;
				// uid
				_len += 8;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  装备uid
	/// </summary>
	[Protocol]
	public class SceneSealEquipRet : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500938;
		public UInt32 Sequence;
		public UInt32 code;
		/// <summary>
		/// 返回码
		/// </summary>
		/// <summary>
		/// 铭文ID数组
		/// </summary>
		public UInt32[] inscriptionIds = new UInt32[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)inscriptionIds.Length);
				for(int i = 0; i < inscriptionIds.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, inscriptionIds[i]);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
				UInt16 inscriptionIdsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref inscriptionIdsCnt);
				inscriptionIds = new UInt32[inscriptionIdsCnt];
				for(int i = 0; i < inscriptionIds.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref inscriptionIds[i]);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)inscriptionIds.Length);
				for(int i = 0; i < inscriptionIds.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, inscriptionIds[i]);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
				UInt16 inscriptionIdsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref inscriptionIdsCnt);
				inscriptionIds = new UInt32[inscriptionIdsCnt];
				for(int i = 0; i < inscriptionIds.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref inscriptionIds[i]);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// code
				_len += 4;
				// inscriptionIds
				_len += 2 + 4 * inscriptionIds.Length;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneCheckSealEquipReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500939;
		public UInt32 Sequence;
		public UInt64 uid;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, uid);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref uid);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, uid);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref uid);
			}

			public int getLen()
			{
				int _len = 0;
				// uid
				_len += 8;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  装备uid
	/// </summary>
	[Protocol]
	public class SceneCheckSealEquipRet : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500940;
		public UInt32 Sequence;
		public UInt32 code;
		/// <summary>
		/// 返回码
		/// </summary>
		public UInt32 matID;
		/// <summary>
		/// 材料ID
		/// </summary>
		public UInt16 needNum;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
				BaseDLL.encode_uint32(buffer, ref pos_, matID);
				BaseDLL.encode_uint16(buffer, ref pos_, needNum);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
				BaseDLL.decode_uint32(buffer, ref pos_, ref matID);
				BaseDLL.decode_uint16(buffer, ref pos_, ref needNum);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
				BaseDLL.encode_uint32(buffer, ref pos_, matID);
				BaseDLL.encode_uint16(buffer, ref pos_, needNum);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
				BaseDLL.decode_uint32(buffer, ref pos_, ref matID);
				BaseDLL.decode_uint16(buffer, ref pos_, ref needNum);
			}

			public int getLen()
			{
				int _len = 0;
				// code
				_len += 4;
				// matID
				_len += 4;
				// needNum
				_len += 2;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 需要材料数量
	/// </summary>
	[Protocol]
	public class SceneRandEquipQlvReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500941;
		public UInt32 Sequence;
		public UInt64 uid;
		/// <summary>
		/// 装备uid
		/// </summary>
		public byte bUsePoint;
		/// <summary>
		/// 是否使用绑点代替
		/// </summary>
		public byte usePerfect;
		

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, uid);
				BaseDLL.encode_int8(buffer, ref pos_, bUsePoint);
				BaseDLL.encode_int8(buffer, ref pos_, usePerfect);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref uid);
				BaseDLL.decode_int8(buffer, ref pos_, ref bUsePoint);
				BaseDLL.decode_int8(buffer, ref pos_, ref usePerfect);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, uid);
				BaseDLL.encode_int8(buffer, ref pos_, bUsePoint);
				BaseDLL.encode_int8(buffer, ref pos_, usePerfect);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref uid);
				BaseDLL.decode_int8(buffer, ref pos_, ref bUsePoint);
				BaseDLL.decode_int8(buffer, ref pos_, ref usePerfect);
			}

			public int getLen()
			{
				int _len = 0;
				// uid
				_len += 8;
				// bUsePoint
				_len += 1;
				// usePerfect
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 是否使用完美洗练卷
	/// </summary>
	[Protocol]
	public class SceneRandEquipQlvRet : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500942;
		public UInt32 Sequence;
		public UInt32 code;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			}

			public int getLen()
			{
				int _len = 0;
				// code
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 返回码
	/// </summary>
	/// <summary>
	/// 开罐子返回
	/// </summary>
	[Protocol]
	public class SceneUseMagicJarRet : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500943;
		public UInt32 Sequence;
		public UInt32 code;
		/// <summary>
		/// 返回码
		/// </summary>
		public UInt32 jarID;
		/// <summary>
		/// 罐子ID
		/// </summary>
		public OpenJarResult[] getItems = new OpenJarResult[0];
		/// <summary>
		/// 抽到的道具
		/// </summary>
		public ItemReward baseItem = new ItemReward();
		/// <summary>
		/// 保底道具
		/// </summary>
		public UInt32 getPointId;
		/// <summary>
		/// 获得积分id
		/// </summary>
		public UInt32 getPoint;
		/// <summary>
		/// 获得积分数量
		/// </summary>
		public UInt32 crit;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
				BaseDLL.encode_uint32(buffer, ref pos_, jarID);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)getItems.Length);
				for(int i = 0; i < getItems.Length; i++)
				{
					getItems[i].encode(buffer, ref pos_);
				}
				baseItem.encode(buffer, ref pos_);
				BaseDLL.encode_uint32(buffer, ref pos_, getPointId);
				BaseDLL.encode_uint32(buffer, ref pos_, getPoint);
				BaseDLL.encode_uint32(buffer, ref pos_, crit);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
				BaseDLL.decode_uint32(buffer, ref pos_, ref jarID);
				UInt16 getItemsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref getItemsCnt);
				getItems = new OpenJarResult[getItemsCnt];
				for(int i = 0; i < getItems.Length; i++)
				{
					getItems[i] = new OpenJarResult();
					getItems[i].decode(buffer, ref pos_);
				}
				baseItem.decode(buffer, ref pos_);
				BaseDLL.decode_uint32(buffer, ref pos_, ref getPointId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref getPoint);
				BaseDLL.decode_uint32(buffer, ref pos_, ref crit);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
				BaseDLL.encode_uint32(buffer, ref pos_, jarID);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)getItems.Length);
				for(int i = 0; i < getItems.Length; i++)
				{
					getItems[i].encode(buffer, ref pos_);
				}
				baseItem.encode(buffer, ref pos_);
				BaseDLL.encode_uint32(buffer, ref pos_, getPointId);
				BaseDLL.encode_uint32(buffer, ref pos_, getPoint);
				BaseDLL.encode_uint32(buffer, ref pos_, crit);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
				BaseDLL.decode_uint32(buffer, ref pos_, ref jarID);
				UInt16 getItemsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref getItemsCnt);
				getItems = new OpenJarResult[getItemsCnt];
				for(int i = 0; i < getItems.Length; i++)
				{
					getItems[i] = new OpenJarResult();
					getItems[i].decode(buffer, ref pos_);
				}
				baseItem.decode(buffer, ref pos_);
				BaseDLL.decode_uint32(buffer, ref pos_, ref getPointId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref getPoint);
				BaseDLL.decode_uint32(buffer, ref pos_, ref crit);
			}

			public int getLen()
			{
				int _len = 0;
				// code
				_len += 4;
				// jarID
				_len += 4;
				// getItems
				_len += 2;
				for(int j = 0; j < getItems.Length; j++)
				{
					_len += getItems[j].getLen();
				}
				// baseItem
				_len += baseItem.getLen();
				// getPointId
				_len += 4;
				// getPoint
				_len += 4;
				// crit
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 暴击倍数
	/// </summary>
	/// <summary>
	/// 罐子积分请求
	/// </summary>
	[Protocol]
	public class SceneJarPointReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500960;
		public UInt32 Sequence;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
			}

			public void decode(byte[] buffer, ref int pos_)
			{
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
			}

			public int getLen()
			{
				int _len = 0;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 罐子积分请求响应
	/// </summary>
	[Protocol]
	public class SceneJarPointRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500961;
		public UInt32 Sequence;
		public UInt32 goldPoint;
		public UInt32 magPoint;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, goldPoint);
				BaseDLL.encode_uint32(buffer, ref pos_, magPoint);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref goldPoint);
				BaseDLL.decode_uint32(buffer, ref pos_, ref magPoint);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, goldPoint);
				BaseDLL.encode_uint32(buffer, ref pos_, magPoint);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref goldPoint);
				BaseDLL.decode_uint32(buffer, ref pos_, ref magPoint);
			}

			public int getLen()
			{
				int _len = 0;
				// goldPoint
				_len += 4;
				// magPoint
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 附魔请求
	/// </summary>
	[Protocol]
	public class SceneAddMagicReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500944;
		public UInt32 Sequence;
		public UInt64 cardUid;
		/// <summary>
		/// 附魔卡uid
		/// </summary>
		public UInt64 itemUid;
		/// <summary>
		/// 附魔卡uid1
		/// </summary>
		public UInt64 itemUid1;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, cardUid);
				BaseDLL.encode_uint64(buffer, ref pos_, itemUid);
				BaseDLL.encode_uint64(buffer, ref pos_, itemUid1);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref cardUid);
				BaseDLL.decode_uint64(buffer, ref pos_, ref itemUid);
				BaseDLL.decode_uint64(buffer, ref pos_, ref itemUid1);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, cardUid);
				BaseDLL.encode_uint64(buffer, ref pos_, itemUid);
				BaseDLL.encode_uint64(buffer, ref pos_, itemUid1);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref cardUid);
				BaseDLL.decode_uint64(buffer, ref pos_, ref itemUid);
				BaseDLL.decode_uint64(buffer, ref pos_, ref itemUid1);
			}

			public int getLen()
			{
				int _len = 0;
				// cardUid
				_len += 8;
				// itemUid
				_len += 8;
				// itemUid1
				_len += 8;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 装备uid
	/// </summary>
	/// <summary>
	/// 附魔请求返回
	/// </summary>
	[Protocol]
	public class SceneAddMagicRet : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500945;
		public UInt32 Sequence;
		public UInt32 code;
		/// <summary>
		/// 返回码
		/// </summary>
		public UInt64 itemUid;
		/// <summary>
		/// 附魔的道具
		/// </summary>
		public UInt32 cardId;
		/// <summary>
		/// 附魔的附魔卡表ID
		/// </summary>
		public byte cardLev;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
				BaseDLL.encode_uint64(buffer, ref pos_, itemUid);
				BaseDLL.encode_uint32(buffer, ref pos_, cardId);
				BaseDLL.encode_int8(buffer, ref pos_, cardLev);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
				BaseDLL.decode_uint64(buffer, ref pos_, ref itemUid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref cardId);
				BaseDLL.decode_int8(buffer, ref pos_, ref cardLev);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
				BaseDLL.encode_uint64(buffer, ref pos_, itemUid);
				BaseDLL.encode_uint32(buffer, ref pos_, cardId);
				BaseDLL.encode_int8(buffer, ref pos_, cardLev);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
				BaseDLL.decode_uint64(buffer, ref pos_, ref itemUid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref cardId);
				BaseDLL.decode_int8(buffer, ref pos_, ref cardLev);
			}

			public int getLen()
			{
				int _len = 0;
				// code
				_len += 4;
				// itemUid
				_len += 8;
				// cardId
				_len += 4;
				// cardLev
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 附魔的附魔卡等级
	/// </summary>
	/// <summary>
	/// 附魔卡合成
	/// </summary>
	[Protocol]
	public class SceneMagicCardCompReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500946;
		public UInt32 Sequence;
		public UInt64 cardA;
		/// <summary>
		/// 附魔卡A
		/// </summary>
		public UInt64 cardB;

		public UInt32 flag;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, cardA);
				BaseDLL.encode_uint64(buffer, ref pos_, cardB);
				BaseDLL.encode_uint32(buffer, ref pos_, flag);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref cardA);
				BaseDLL.decode_uint64(buffer, ref pos_, ref cardB);
				BaseDLL.decode_uint32(buffer, ref pos_, ref flag);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, cardA);
				BaseDLL.encode_uint64(buffer, ref pos_, cardB);
				BaseDLL.encode_uint32(buffer, ref pos_, flag);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref cardA);
				BaseDLL.decode_uint64(buffer, ref pos_, ref cardB);
				BaseDLL.decode_uint32(buffer, ref pos_, ref flag);
			}

			public int getLen()
			{
				int _len = 0;
				// cardA
				_len += 8;
				// cardB
				_len += 8;
				// flag
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneGiveGiftReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501168;
		public UInt32 Sequence;
		public UInt64 roleid;
		/// <summary>
		/// 附魔卡A
		/// </summary>
		public UInt64 guid;

		public UInt32 count;

		public UInt32 isGold;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, roleid);
				BaseDLL.encode_uint64(buffer, ref pos_, guid);
				BaseDLL.encode_uint32(buffer, ref pos_, count);
				BaseDLL.encode_uint32(buffer, ref pos_, isGold);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref roleid);
				BaseDLL.decode_uint64(buffer, ref pos_, ref guid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref count);
				BaseDLL.decode_uint32(buffer, ref pos_, ref isGold);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, roleid);
				BaseDLL.encode_uint64(buffer, ref pos_, guid);
				BaseDLL.encode_uint32(buffer, ref pos_, count);
				BaseDLL.encode_uint32(buffer, ref pos_, isGold);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref roleid);
				BaseDLL.decode_uint64(buffer, ref pos_, ref guid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref count);
				BaseDLL.decode_uint32(buffer, ref pos_, ref isGold);
			}

			public int getLen()
			{
				int _len = 0;
				// cardA
				_len += 8;
				// cardB
				_len += 8;
				// flag
				_len += 4;
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneGiveGiftRet : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501169;
		public UInt32 Sequence;
		public UInt32 code;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			}

			public int getLen()
			{
				int _len = 0;
				// code
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 附魔卡B
	/// </summary>
	/// <summary>
	/// 附魔卡合成返回
	/// </summary>
	[Protocol]
	public class SceneMagicCardCompRet : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500947;
		public UInt32 Sequence;
		public UInt32 code;
		/// <summary>
		/// 返回码
		/// </summary>
		public UInt32 cardId;
		/// <summary>
		/// 合成的附魔卡id	
		/// </summary>
		public byte cardLev;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
				BaseDLL.encode_uint32(buffer, ref pos_, cardId);
				BaseDLL.encode_int8(buffer, ref pos_, cardLev);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
				BaseDLL.decode_uint32(buffer, ref pos_, ref cardId);
				BaseDLL.decode_int8(buffer, ref pos_, ref cardLev);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
				BaseDLL.encode_uint32(buffer, ref pos_, cardId);
				BaseDLL.encode_int8(buffer, ref pos_, cardLev);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
				BaseDLL.decode_uint32(buffer, ref pos_, ref cardId);
				BaseDLL.decode_int8(buffer, ref pos_, ref cardLev);
			}

			public int getLen()
			{
				int _len = 0;
				// code
				_len += 4;
				// cardId
				_len += 4;
				// cardLev
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 合成的附魔卡等级
	/// </summary>
	/// <summary>
	/// 添加宝珠请求
	/// </summary>
	[Protocol]
	public class SceneAddPreciousBeadReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500971;
		public UInt32 Sequence;
		/// <summary>
		///  宝珠uid
		/// </summary>
		public UInt64 preciousBeadUid;
		/// <summary>
		///  道具uid
		/// </summary>
		public UInt64 itemUid;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, preciousBeadUid);
				BaseDLL.encode_uint64(buffer, ref pos_, itemUid);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref preciousBeadUid);
				BaseDLL.decode_uint64(buffer, ref pos_, ref itemUid);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, preciousBeadUid);
				BaseDLL.encode_uint64(buffer, ref pos_, itemUid);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref preciousBeadUid);
				BaseDLL.decode_uint64(buffer, ref pos_, ref itemUid);
			}

			public int getLen()
			{
				int _len = 0;
				// preciousBeadUid
				_len += 8;
				// itemUid
				_len += 8;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 添加宝珠返回
	/// </summary>
	[Protocol]
	public class SceneAddPreciousBeadRet : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500972;
		public UInt32 Sequence;
		/// <summary>
		///  返回码
		/// </summary>
		public UInt32 code;
		/// <summary>
		///  宝珠id
		/// </summary>
		public UInt32 preciousBeadId;
		/// <summary>
		///  道具uid
		/// </summary>
		public UInt64 itemUid;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
				BaseDLL.encode_uint32(buffer, ref pos_, preciousBeadId);
				BaseDLL.encode_uint64(buffer, ref pos_, itemUid);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
				BaseDLL.decode_uint32(buffer, ref pos_, ref preciousBeadId);
				BaseDLL.decode_uint64(buffer, ref pos_, ref itemUid);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
				BaseDLL.encode_uint32(buffer, ref pos_, preciousBeadId);
				BaseDLL.encode_uint64(buffer, ref pos_, itemUid);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
				BaseDLL.decode_uint32(buffer, ref pos_, ref preciousBeadId);
				BaseDLL.decode_uint64(buffer, ref pos_, ref itemUid);
			}

			public int getLen()
			{
				int _len = 0;
				// code
				_len += 4;
				// preciousBeadId
				_len += 4;
				// itemUid
				_len += 8;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// ------------商城相关-----------------------
	/// </summary>
	/// <summary>
	/// 激活商城限时礼包请求
	/// </summary>
	[Protocol]
	public class WorldMallGiftPackActivateReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 602814;
		public UInt32 Sequence;
		/// <summary>
		///  对应枚举MallGiftPackActivateCond
		/// </summary>
		public byte giftPackActCond;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, giftPackActCond);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref giftPackActCond);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, giftPackActCond);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref giftPackActCond);
			}

			public int getLen()
			{
				int _len = 0;
				// giftPackActCond
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 激活条件
	/// </summary>
	/// <summary>
	/// 激活商城限时礼包返回
	/// </summary>
	[Protocol]
	public class WorldMallGiftPackActivateRet : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 602815;
		public UInt32 Sequence;
		public MallItemInfo[] items = new MallItemInfo[0];
		/// <summary>
		/// 一个礼包
		/// </summary>
		public UInt32 code;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)items.Length);
				for(int i = 0; i < items.Length; i++)
				{
					items[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_uint32(buffer, ref pos_, code);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 itemsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref itemsCnt);
				items = new MallItemInfo[itemsCnt];
				for(int i = 0; i < items.Length; i++)
				{
					items[i] = new MallItemInfo();
					items[i].decode(buffer, ref pos_);
				}
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)items.Length);
				for(int i = 0; i < items.Length; i++)
				{
					items[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_uint32(buffer, ref pos_, code);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 itemsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref itemsCnt);
				items = new MallItemInfo[itemsCnt];
				for(int i = 0; i < items.Length; i++)
				{
					items[i] = new MallItemInfo();
					items[i].decode(buffer, ref pos_);
				}
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			}

			public int getLen()
			{
				int _len = 0;
				// items
				_len += 2;
				for(int j = 0; j < items.Length; j++)
				{
					_len += items[j].getLen();
				}
				// code
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 错误码
	/// </summary>
	/// <summary>
	/// 同步商城礼包活动状态
	/// </summary>
	[Protocol]
	public class SyncWorldMallGiftPackActivityState : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 602817;
		public UInt32 Sequence;
		/// <summary>
		/// 对应枚举MallGiftPackActivityState
		/// </summary>
		public byte state;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, state);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref state);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, state);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref state);
			}

			public int getLen()
			{
				int _len = 0;
				// state
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 同步商城私人订制
	/// </summary>
	[Protocol]
	public class WorldSyncMallPersonalTailorState : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 602818;
		public UInt32 Sequence;
		/// <summary>
		/// 对应枚举MallGiftPackActivateCond
		/// </summary>
		public byte state;
		/// <summary>
		/// 最新触发的商品id
		/// </summary>
		public UInt32 goodsId;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, state);
				BaseDLL.encode_uint32(buffer, ref pos_, goodsId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref state);
				BaseDLL.decode_uint32(buffer, ref pos_, ref goodsId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, state);
				BaseDLL.encode_uint32(buffer, ref pos_, goodsId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref state);
				BaseDLL.decode_uint32(buffer, ref pos_, ref goodsId);
			}

			public int getLen()
			{
				int _len = 0;
				// state
				_len += 1;
				// goodsId
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 查询商城道具请求
	/// </summary>
	[Protocol]
	public class WorldMallQueryItemReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 602803;
		public UInt32 Sequence;
		/// <summary>
		/// 商城热门类索引,1-热门
		/// </summary>
		public byte tagType;
		/// <summary>
		/// 商城主页签
		/// </summary>
		public byte type;
		/// <summary>
		/// 商城子页签
		/// </summary>
		public byte subType;
		/// <summary>
		/// 货币类别
		/// </summary>
		public byte moneyType;
		/// <summary>
		/// 职业
		/// </summary>
		public byte occu;
		/// <summary>
		/// 本地数据更新时间
		/// </summary>
		public UInt32 updateTime;
		/// <summary>
		/// 是否私人订制
		/// </summary>
		public byte isPersonalTailor;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, tagType);
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_int8(buffer, ref pos_, subType);
				BaseDLL.encode_int8(buffer, ref pos_, moneyType);
				BaseDLL.encode_int8(buffer, ref pos_, occu);
				BaseDLL.encode_uint32(buffer, ref pos_, updateTime);
				BaseDLL.encode_int8(buffer, ref pos_, isPersonalTailor);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref tagType);
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_int8(buffer, ref pos_, ref subType);
				BaseDLL.decode_int8(buffer, ref pos_, ref moneyType);
				BaseDLL.decode_int8(buffer, ref pos_, ref occu);
				BaseDLL.decode_uint32(buffer, ref pos_, ref updateTime);
				BaseDLL.decode_int8(buffer, ref pos_, ref isPersonalTailor);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, tagType);
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_int8(buffer, ref pos_, subType);
				BaseDLL.encode_int8(buffer, ref pos_, moneyType);
				BaseDLL.encode_int8(buffer, ref pos_, occu);
				BaseDLL.encode_uint32(buffer, ref pos_, updateTime);
				BaseDLL.encode_int8(buffer, ref pos_, isPersonalTailor);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref tagType);
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_int8(buffer, ref pos_, ref subType);
				BaseDLL.decode_int8(buffer, ref pos_, ref moneyType);
				BaseDLL.decode_int8(buffer, ref pos_, ref occu);
				BaseDLL.decode_uint32(buffer, ref pos_, ref updateTime);
				BaseDLL.decode_int8(buffer, ref pos_, ref isPersonalTailor);
			}

			public int getLen()
			{
				int _len = 0;
				// tagType
				_len += 1;
				// type
				_len += 1;
				// subType
				_len += 1;
				// moneyType
				_len += 1;
				// occu
				_len += 1;
				// updateTime
				_len += 4;
				// isPersonalTailor
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 查询商城道具返回
	/// </summary>
	[Protocol]
	public class WorldMallQueryItemRet : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 602804;
		public UInt32 Sequence;
		/// <summary>
		/// 商城主页签
		/// </summary>
		public byte type;
		public MallItemInfo[] items = new MallItemInfo[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)items.Length);
				for(int i = 0; i < items.Length; i++)
				{
					items[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				UInt16 itemsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref itemsCnt);
				items = new MallItemInfo[itemsCnt];
				for(int i = 0; i < items.Length; i++)
				{
					items[i] = new MallItemInfo();
					items[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)items.Length);
				for(int i = 0; i < items.Length; i++)
				{
					items[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				UInt16 itemsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref itemsCnt);
				items = new MallItemInfo[itemsCnt];
				for(int i = 0; i < items.Length; i++)
				{
					items[i] = new MallItemInfo();
					items[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// type
				_len += 1;
				// items
				_len += 2;
				for(int j = 0; j < items.Length; j++)
				{
					_len += items[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 商城道具
	/// </summary>
	/// <summary>
	/// 购买商城道具请求
	/// </summary>
	[Protocol]
	public class WorldMallBuy : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 602801;
		public UInt32 Sequence;
		public UInt32 itemId;
		/// <summary>
		/// 商城道具ID
		/// </summary>
		public UInt16 num;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, itemId);
				BaseDLL.encode_uint16(buffer, ref pos_, num);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemId);
				BaseDLL.decode_uint16(buffer, ref pos_, ref num);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, itemId);
				BaseDLL.encode_uint16(buffer, ref pos_, num);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemId);
				BaseDLL.decode_uint16(buffer, ref pos_, ref num);
			}

			public int getLen()
			{
				int _len = 0;
				// itemId
				_len += 4;
				// num
				_len += 2;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 购买数量
	/// </summary>
	/// <summary>
	/// 购买商城道具返回
	/// </summary>
	[Protocol]
	public class WorldMallBuyRet : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 602802;
		public UInt32 Sequence;
		public UInt32 code;
		/// <summary>
		/// 返回码
		/// </summary>
		public UInt32 mallitemid;
		/// <summary>
		/// 商城道具id
		/// </summary>
		public Int32 restLimitNum;
		/// <summary>
		/// 剩余限购数,-1是没有限购
		/// </summary>
		/// <summary>
		/// 账号剩余购买次数
		/// </summary>
		public UInt32 accountRestBuyNum;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
				BaseDLL.encode_uint32(buffer, ref pos_, mallitemid);
				BaseDLL.encode_int32(buffer, ref pos_, restLimitNum);
				BaseDLL.encode_uint32(buffer, ref pos_, accountRestBuyNum);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
				BaseDLL.decode_uint32(buffer, ref pos_, ref mallitemid);
				BaseDLL.decode_int32(buffer, ref pos_, ref restLimitNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref accountRestBuyNum);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
				BaseDLL.encode_uint32(buffer, ref pos_, mallitemid);
				BaseDLL.encode_int32(buffer, ref pos_, restLimitNum);
				BaseDLL.encode_uint32(buffer, ref pos_, accountRestBuyNum);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
				BaseDLL.decode_uint32(buffer, ref pos_, ref mallitemid);
				BaseDLL.decode_int32(buffer, ref pos_, ref restLimitNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref accountRestBuyNum);
			}

			public int getLen()
			{
				int _len = 0;
				// code
				_len += 4;
				// mallitemid
				_len += 4;
				// restLimitNum
				_len += 4;
				// accountRestBuyNum
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 商城批量购买请求
	/// </summary>
	[Protocol]
	public class CWMallBatchBuyReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 602812;
		public UInt32 Sequence;
		public ItemReward[] items = new ItemReward[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)items.Length);
				for(int i = 0; i < items.Length; i++)
				{
					items[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 itemsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref itemsCnt);
				items = new ItemReward[itemsCnt];
				for(int i = 0; i < items.Length; i++)
				{
					items[i] = new ItemReward();
					items[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)items.Length);
				for(int i = 0; i < items.Length; i++)
				{
					items[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 itemsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref itemsCnt);
				items = new ItemReward[itemsCnt];
				for(int i = 0; i < items.Length; i++)
				{
					items[i] = new ItemReward();
					items[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// items
				_len += 2;
				for(int j = 0; j < items.Length; j++)
				{
					_len += items[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 商城批量购买返回
	/// </summary>
	[Protocol]
	public class SCMallBatchBuyRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 602813;
		public UInt32 Sequence;
		public UInt32 code;
		public UInt64[] itemUids = new UInt64[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)itemUids.Length);
				for(int i = 0; i < itemUids.Length; i++)
				{
					BaseDLL.encode_uint64(buffer, ref pos_, itemUids[i]);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
				UInt16 itemUidsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref itemUidsCnt);
				itemUids = new UInt64[itemUidsCnt];
				for(int i = 0; i < itemUids.Length; i++)
				{
					BaseDLL.decode_uint64(buffer, ref pos_, ref itemUids[i]);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)itemUids.Length);
				for(int i = 0; i < itemUids.Length; i++)
				{
					BaseDLL.encode_uint64(buffer, ref pos_, itemUids[i]);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
				UInt16 itemUidsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref itemUidsCnt);
				itemUids = new UInt64[itemUidsCnt];
				for(int i = 0; i < itemUids.Length; i++)
				{
					BaseDLL.decode_uint64(buffer, ref pos_, ref itemUids[i]);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// code
				_len += 4;
				// itemUids
				_len += 2 + 8 * itemUids.Length;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 请求商城礼包详情
	/// </summary>
	[Protocol]
	public class WorldMallQueryItemDetailReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 602805;
		public UInt32 Sequence;
		public UInt32 mallItemId;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, mallItemId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref mallItemId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, mallItemId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref mallItemId);
			}

			public int getLen()
			{
				int _len = 0;
				// mallItemId
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 商城道具id
	/// </summary>
	public class MallGiftDetail : Protocol.IProtocolStream
	{
		public UInt32 itemId;
		public UInt16 num;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, itemId);
				BaseDLL.encode_uint16(buffer, ref pos_, num);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemId);
				BaseDLL.decode_uint16(buffer, ref pos_, ref num);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, itemId);
				BaseDLL.encode_uint16(buffer, ref pos_, num);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemId);
				BaseDLL.decode_uint16(buffer, ref pos_, ref num);
			}

			public int getLen()
			{
				int _len = 0;
				// itemId
				_len += 4;
				// num
				_len += 2;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 请求商城礼包详情返回
	/// </summary>
	[Protocol]
	public class WorldMallQueryItemDetailRet : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 602806;
		public UInt32 Sequence;
		public MallGiftDetail[] details = new MallGiftDetail[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)details.Length);
				for(int i = 0; i < details.Length; i++)
				{
					details[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 detailsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref detailsCnt);
				details = new MallGiftDetail[detailsCnt];
				for(int i = 0; i < details.Length; i++)
				{
					details[i] = new MallGiftDetail();
					details[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)details.Length);
				for(int i = 0; i < details.Length; i++)
				{
					details[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 detailsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref detailsCnt);
				details = new MallGiftDetail[detailsCnt];
				for(int i = 0; i < details.Length; i++)
				{
					details[i] = new MallGiftDetail();
					details[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// details
				_len += 2;
				for(int j = 0; j < details.Length; j++)
				{
					_len += details[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 根据道具id获取商城道具请求
	/// </summary>
	[Protocol]
	public class WorldGetMallItemByItemIdReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 602821;
		public UInt32 Sequence;
		/// <summary>
		///  道具id(不是商城道具的id)
		/// </summary>
		public UInt32 itemId;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, itemId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, itemId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemId);
			}

			public int getLen()
			{
				int _len = 0;
				// itemId
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 根据道具id获取商城道具返回
	/// </summary>
	[Protocol]
	public class WorldGetMallItemByItemIdRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 602822;
		public UInt32 Sequence;
		/// <summary>
		///  错误码
		/// </summary>
		public UInt32 retCode;
		/// <summary>
		///  道具id
		/// </summary>
		public UInt32 itemId;
		/// <summary>
		///  映射的商城道具
		/// </summary>
		public MallItemInfo mallItem = new MallItemInfo();

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, retCode);
				BaseDLL.encode_uint32(buffer, ref pos_, itemId);
				mallItem.encode(buffer, ref pos_);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemId);
				mallItem.decode(buffer, ref pos_);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, retCode);
				BaseDLL.encode_uint32(buffer, ref pos_, itemId);
				mallItem.encode(buffer, ref pos_);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemId);
				mallItem.decode(buffer, ref pos_);
			}

			public int getLen()
			{
				int _len = 0;
				// retCode
				_len += 4;
				// itemId
				_len += 4;
				// mallItem
				_len += mallItem.getLen();
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  商城查询单个商品请求
	/// </summary>
	[Protocol]
	public class WorldMallQuerySingleItemReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 602823;
		public UInt32 Sequence;
		/// <summary>
		///  商城道具id
		/// </summary>
		public UInt32 mallItemId;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, mallItemId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref mallItemId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, mallItemId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref mallItemId);
			}

			public int getLen()
			{
				int _len = 0;
				// mallItemId
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  商城查询单个商品返回
	/// </summary>
	[Protocol]
	public class WorldMallQuerySingleItemRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 602824;
		public UInt32 Sequence;
		/// <summary>
		///  错误码
		/// </summary>
		public UInt32 retCode;
		/// <summary>
		///  商城道具信息
		/// </summary>
		public MallItemInfo mallItemInfo = new MallItemInfo();

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, retCode);
				mallItemInfo.encode(buffer, ref pos_);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
				mallItemInfo.decode(buffer, ref pos_);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, retCode);
				mallItemInfo.encode(buffer, ref pos_);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
				mallItemInfo.decode(buffer, ref pos_);
			}

			public int getLen()
			{
				int _len = 0;
				// retCode
				_len += 4;
				// mallItemInfo
				_len += mallItemInfo.getLen();
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  world->client 玩家拥有的商城购买获得物同步
	/// </summary>
	[Protocol]
	public class WorldPlayerMallBuyGotInfoSync : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 602825;
		public UInt32 Sequence;
		public MallBuyGotInfo mallBuyGotInfo = new MallBuyGotInfo();

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				mallBuyGotInfo.encode(buffer, ref pos_);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				mallBuyGotInfo.decode(buffer, ref pos_);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				mallBuyGotInfo.encode(buffer, ref pos_);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				mallBuyGotInfo.decode(buffer, ref pos_);
			}

			public int getLen()
			{
				int _len = 0;
				// mallBuyGotInfo
				_len += mallBuyGotInfo.getLen();
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  world->client 推送商城商品
	/// </summary>
	[Protocol]
	public class WorldPushMallItems : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 602826;
		public UInt32 Sequence;
		/// <summary>
		///  商城道具
		/// </summary>
		public MallItemInfo[] mallItems = new MallItemInfo[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)mallItems.Length);
				for(int i = 0; i < mallItems.Length; i++)
				{
					mallItems[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 mallItemsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref mallItemsCnt);
				mallItems = new MallItemInfo[mallItemsCnt];
				for(int i = 0; i < mallItems.Length; i++)
				{
					mallItems[i] = new MallItemInfo();
					mallItems[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)mallItems.Length);
				for(int i = 0; i < mallItems.Length; i++)
				{
					mallItems[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 mallItemsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref mallItemsCnt);
				mallItems = new MallItemInfo[mallItemsCnt];
				for(int i = 0; i < mallItems.Length; i++)
				{
					mallItems[i] = new MallItemInfo();
					mallItems[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// mallItems
				_len += 2;
				for(int j = 0; j < mallItems.Length; j++)
				{
					_len += mallItems[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 推荐页信息
	/// </summary>
	public class MallRecommendPageInfo : Protocol.IProtocolStream
	{
		/// <summary>
		/// id
		/// </summary>
		public UInt32 id;
		/// <summary>
		/// 商城商品id
		/// </summary>
		public UInt32 mallItemID;
		/// <summary>
		/// 商品所属商城类型
		/// </summary>
		public byte itemBelongMallType;
		/// <summary>
		/// 推荐类型
		/// </summary>
		public byte recommendType;
		/// <summary>
		/// 广告图路径
		/// </summary>
		public string adImagePath;
		/// <summary>
		/// 链接功能类型
		/// </summary>
		public byte linkFunctionType;
		/// <summary>
		/// 链接路径
		/// </summary>
		public string linkPath;
		/// <summary>
		/// 排序
		/// </summary>
		public UInt32 sortNum;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, id);
				BaseDLL.encode_uint32(buffer, ref pos_, mallItemID);
				BaseDLL.encode_int8(buffer, ref pos_, itemBelongMallType);
				BaseDLL.encode_int8(buffer, ref pos_, recommendType);
				byte[] adImagePathBytes = StringHelper.StringToUTF8Bytes(adImagePath);
				BaseDLL.encode_string(buffer, ref pos_, adImagePathBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, linkFunctionType);
				byte[] linkPathBytes = StringHelper.StringToUTF8Bytes(linkPath);
				BaseDLL.encode_string(buffer, ref pos_, linkPathBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint32(buffer, ref pos_, sortNum);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
				BaseDLL.decode_uint32(buffer, ref pos_, ref mallItemID);
				BaseDLL.decode_int8(buffer, ref pos_, ref itemBelongMallType);
				BaseDLL.decode_int8(buffer, ref pos_, ref recommendType);
				UInt16 adImagePathLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref adImagePathLen);
				byte[] adImagePathBytes = new byte[adImagePathLen];
				for(int i = 0; i < adImagePathLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref adImagePathBytes[i]);
				}
				adImagePath = StringHelper.BytesToString(adImagePathBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref linkFunctionType);
				UInt16 linkPathLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref linkPathLen);
				byte[] linkPathBytes = new byte[linkPathLen];
				for(int i = 0; i < linkPathLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref linkPathBytes[i]);
				}
				linkPath = StringHelper.BytesToString(linkPathBytes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref sortNum);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, id);
				BaseDLL.encode_uint32(buffer, ref pos_, mallItemID);
				BaseDLL.encode_int8(buffer, ref pos_, itemBelongMallType);
				BaseDLL.encode_int8(buffer, ref pos_, recommendType);
				byte[] adImagePathBytes = StringHelper.StringToUTF8Bytes(adImagePath);
				BaseDLL.encode_string(buffer, ref pos_, adImagePathBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, linkFunctionType);
				byte[] linkPathBytes = StringHelper.StringToUTF8Bytes(linkPath);
				BaseDLL.encode_string(buffer, ref pos_, linkPathBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint32(buffer, ref pos_, sortNum);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
				BaseDLL.decode_uint32(buffer, ref pos_, ref mallItemID);
				BaseDLL.decode_int8(buffer, ref pos_, ref itemBelongMallType);
				BaseDLL.decode_int8(buffer, ref pos_, ref recommendType);
				UInt16 adImagePathLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref adImagePathLen);
				byte[] adImagePathBytes = new byte[adImagePathLen];
				for(int i = 0; i < adImagePathLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref adImagePathBytes[i]);
				}
				adImagePath = StringHelper.BytesToString(adImagePathBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref linkFunctionType);
				UInt16 linkPathLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref linkPathLen);
				byte[] linkPathBytes = new byte[linkPathLen];
				for(int i = 0; i < linkPathLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref linkPathBytes[i]);
				}
				linkPath = StringHelper.BytesToString(linkPathBytes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref sortNum);
			}

			public int getLen()
			{
				int _len = 0;
				// id
				_len += 4;
				// mallItemID
				_len += 4;
				// itemBelongMallType
				_len += 1;
				// recommendType
				_len += 1;
				// adImagePath
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(adImagePath);
					_len += 2 + _strBytes.Length;
				}
				// linkFunctionType
				_len += 1;
				// linkPath
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(linkPath);
					_len += 2 + _strBytes.Length;
				}
				// sortNum
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  client->world 请求 拉取推荐页信息
	/// </summary>
	[Protocol]
	public class WorldMallRecommendPageListReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 602835;
		public UInt32 Sequence;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
			}

			public void decode(byte[] buffer, ref int pos_)
			{
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
			}

			public int getLen()
			{
				int _len = 0;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  world->client 响应 拉取推荐页信息
	/// </summary>
	[Protocol]
	public class WorldMallRecommendPageListRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 602836;
		public UInt32 Sequence;
		/// <summary>
		/// 推荐页信息
		/// </summary>
		public MallRecommendPageInfo[] pageInfos = new MallRecommendPageInfo[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)pageInfos.Length);
				for(int i = 0; i < pageInfos.Length; i++)
				{
					pageInfos[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 pageInfosCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref pageInfosCnt);
				pageInfos = new MallRecommendPageInfo[pageInfosCnt];
				for(int i = 0; i < pageInfos.Length; i++)
				{
					pageInfos[i] = new MallRecommendPageInfo();
					pageInfos[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)pageInfos.Length);
				for(int i = 0; i < pageInfos.Length; i++)
				{
					pageInfos[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 pageInfosCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref pageInfosCnt);
				pageInfos = new MallRecommendPageInfo[pageInfosCnt];
				for(int i = 0; i < pageInfos.Length; i++)
				{
					pageInfos[i] = new MallRecommendPageInfo();
					pageInfos[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// pageInfos
				_len += 2;
				for(int j = 0; j < pageInfos.Length; j++)
				{
					_len += pageInfos[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///   client->world 商城批量查询商品请求
	/// </summary>
	[Protocol]
	public class WorldMallQueryBatchItemReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 602837;
		public UInt32 Sequence;
		/// <summary>
		///  商城道具id
		/// </summary>
		public UInt32[] mallItemIds = new UInt32[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)mallItemIds.Length);
				for(int i = 0; i < mallItemIds.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, mallItemIds[i]);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 mallItemIdsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref mallItemIdsCnt);
				mallItemIds = new UInt32[mallItemIdsCnt];
				for(int i = 0; i < mallItemIds.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref mallItemIds[i]);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)mallItemIds.Length);
				for(int i = 0; i < mallItemIds.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, mallItemIds[i]);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 mallItemIdsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref mallItemIdsCnt);
				mallItemIds = new UInt32[mallItemIdsCnt];
				for(int i = 0; i < mallItemIds.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref mallItemIds[i]);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// mallItemIds
				_len += 2 + 4 * mallItemIds.Length;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  world->client 商城批量查询商品返回
	/// </summary>
	[Protocol]
	public class WorldMallQueryBatchItemRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 602838;
		public UInt32 Sequence;
		/// <summary>
		///  商城道具
		/// </summary>
		public MallItemInfo[] mallItemInfos = new MallItemInfo[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)mallItemInfos.Length);
				for(int i = 0; i < mallItemInfos.Length; i++)
				{
					mallItemInfos[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 mallItemInfosCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref mallItemInfosCnt);
				mallItemInfos = new MallItemInfo[mallItemInfosCnt];
				for(int i = 0; i < mallItemInfos.Length; i++)
				{
					mallItemInfos[i] = new MallItemInfo();
					mallItemInfos[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)mallItemInfos.Length);
				for(int i = 0; i < mallItemInfos.Length; i++)
				{
					mallItemInfos[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 mallItemInfosCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref mallItemInfosCnt);
				mallItemInfos = new MallItemInfo[mallItemInfosCnt];
				for(int i = 0; i < mallItemInfos.Length; i++)
				{
					mallItemInfos[i] = new MallItemInfo();
					mallItemInfos[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// mallItemInfos
				_len += 2;
				for(int j = 0; j < mallItemInfos.Length; j++)
				{
					_len += mallItemInfos[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 快速购买请求
	/// </summary>
	[Protocol]
	public class SceneQuickBuyReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 507101;
		public UInt32 Sequence;
		/// <summary>
		///  类型(对应枚举QuickBuyTargetType)
		/// </summary>
		public byte type;
		/// <summary>
		///  参数
		/// </summary>
		public UInt64 param1;
		public UInt64 param2;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_uint64(buffer, ref pos_, param1);
				BaseDLL.encode_uint64(buffer, ref pos_, param2);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_uint64(buffer, ref pos_, ref param1);
				BaseDLL.decode_uint64(buffer, ref pos_, ref param2);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_uint64(buffer, ref pos_, param1);
				BaseDLL.encode_uint64(buffer, ref pos_, param2);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_uint64(buffer, ref pos_, ref param1);
				BaseDLL.decode_uint64(buffer, ref pos_, ref param2);
			}

			public int getLen()
			{
				int _len = 0;
				// type
				_len += 1;
				// param1
				_len += 8;
				// param2
				_len += 8;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 快速购买返回
	/// </summary>
	[Protocol]
	public class SceneQuickBuyRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 507102;
		public UInt32 Sequence;
		public UInt32 result;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
			}

			public int getLen()
			{
				int _len = 0;
				// result
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 开罐子
	/// </summary>
	[Protocol]
	public class SceneUseMagicJarReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500948;
		public UInt32 Sequence;
		public UInt32 type;
		/// <summary>
		/// 开罐类型
		/// </summary>
		public byte combo;
		/// <summary>
		/// 是否连开
		/// </summary>
		/// <summary>
		///  运营活动id
		/// </summary>
		public UInt32 opActId;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, type);
				BaseDLL.encode_int8(buffer, ref pos_, combo);
				BaseDLL.encode_uint32(buffer, ref pos_, opActId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref type);
				BaseDLL.decode_int8(buffer, ref pos_, ref combo);
				BaseDLL.decode_uint32(buffer, ref pos_, ref opActId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, type);
				BaseDLL.encode_int8(buffer, ref pos_, combo);
				BaseDLL.encode_uint32(buffer, ref pos_, opActId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref type);
				BaseDLL.decode_int8(buffer, ref pos_, ref combo);
				BaseDLL.decode_uint32(buffer, ref pos_, ref opActId);
			}

			public int getLen()
			{
				int _len = 0;
				// type
				_len += 4;
				// combo
				_len += 1;
				// opActId
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneNotifyCostItem : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500949;
		public UInt32 Sequence;
		public UInt32 itemid;
		public byte quality;
		public UInt16 num;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, itemid);
				BaseDLL.encode_int8(buffer, ref pos_, quality);
				BaseDLL.encode_uint16(buffer, ref pos_, num);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemid);
				BaseDLL.decode_int8(buffer, ref pos_, ref quality);
				BaseDLL.decode_uint16(buffer, ref pos_, ref num);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, itemid);
				BaseDLL.encode_int8(buffer, ref pos_, quality);
				BaseDLL.encode_uint16(buffer, ref pos_, num);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemid);
				BaseDLL.decode_int8(buffer, ref pos_, ref quality);
				BaseDLL.decode_uint16(buffer, ref pos_, ref num);
			}

			public int getLen()
			{
				int _len = 0;
				// itemid
				_len += 4;
				// quality
				_len += 1;
				// num
				_len += 2;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 请求快捷使用关卡道具
	/// </summary>
	[Protocol]
	public class SceneQuickUseItemReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500950;
		public UInt32 Sequence;
		public byte idx;
		public UInt32 dungenid;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, idx);
				BaseDLL.encode_uint32(buffer, ref pos_, dungenid);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref idx);
				BaseDLL.decode_uint32(buffer, ref pos_, ref dungenid);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, idx);
				BaseDLL.encode_uint32(buffer, ref pos_, dungenid);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref idx);
				BaseDLL.decode_uint32(buffer, ref pos_, ref dungenid);
			}

			public int getLen()
			{
				int _len = 0;
				// idx
				_len += 1;
				// dungenid
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 请求快捷使用关卡道具返回
	/// </summary>
	[Protocol]
	public class SceneQuickUseItemRet : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500951;
		public UInt32 Sequence;
		public UInt32 code;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			}

			public int getLen()
			{
				int _len = 0;
				// code
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneFashionMergeReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500952;
		public UInt32 Sequence;
		public UInt64 leftid;
		/// <summary>
		///  时装A
		/// </summary>
		public UInt64 rightid;
		/// <summary>
		///  时装B
		/// </summary>
		public UInt64 composer;
		/// <summary>
		///  合成器
		/// </summary>
		public UInt32 skySuitID;
		/// <summary>
		///  选择的天空套套装ID
		/// </summary>
		public UInt32 selFashionID;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, leftid);
				BaseDLL.encode_uint64(buffer, ref pos_, rightid);
				BaseDLL.encode_uint64(buffer, ref pos_, composer);
				BaseDLL.encode_uint32(buffer, ref pos_, skySuitID);
				BaseDLL.encode_uint32(buffer, ref pos_, selFashionID);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref leftid);
				BaseDLL.decode_uint64(buffer, ref pos_, ref rightid);
				BaseDLL.decode_uint64(buffer, ref pos_, ref composer);
				BaseDLL.decode_uint32(buffer, ref pos_, ref skySuitID);
				BaseDLL.decode_uint32(buffer, ref pos_, ref selFashionID);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, leftid);
				BaseDLL.encode_uint64(buffer, ref pos_, rightid);
				BaseDLL.encode_uint64(buffer, ref pos_, composer);
				BaseDLL.encode_uint32(buffer, ref pos_, skySuitID);
				BaseDLL.encode_uint32(buffer, ref pos_, selFashionID);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref leftid);
				BaseDLL.decode_uint64(buffer, ref pos_, ref rightid);
				BaseDLL.decode_uint64(buffer, ref pos_, ref composer);
				BaseDLL.decode_uint32(buffer, ref pos_, ref skySuitID);
				BaseDLL.decode_uint32(buffer, ref pos_, ref selFashionID);
			}

			public int getLen()
			{
				int _len = 0;
				// leftid
				_len += 8;
				// rightid
				_len += 8;
				// composer
				_len += 8;
				// skySuitID
				_len += 4;
				// selFashionID
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneFashionMergeRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500953;
		public UInt32 Sequence;
		public Int32 result;
		public byte resultType;
		public UInt32 itemA;
		public Int32 numA;
		public UInt32 itemB;
		public Int32 numB;
		public UInt32 itemC;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int32(buffer, ref pos_, result);
				BaseDLL.encode_int8(buffer, ref pos_, resultType);
				BaseDLL.encode_uint32(buffer, ref pos_, itemA);
				BaseDLL.encode_int32(buffer, ref pos_, numA);
				BaseDLL.encode_uint32(buffer, ref pos_, itemB);
				BaseDLL.encode_int32(buffer, ref pos_, numB);
				BaseDLL.encode_uint32(buffer, ref pos_, itemC);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int32(buffer, ref pos_, ref result);
				BaseDLL.decode_int8(buffer, ref pos_, ref resultType);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemA);
				BaseDLL.decode_int32(buffer, ref pos_, ref numA);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemB);
				BaseDLL.decode_int32(buffer, ref pos_, ref numB);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemC);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int32(buffer, ref pos_, result);
				BaseDLL.encode_int8(buffer, ref pos_, resultType);
				BaseDLL.encode_uint32(buffer, ref pos_, itemA);
				BaseDLL.encode_int32(buffer, ref pos_, numA);
				BaseDLL.encode_uint32(buffer, ref pos_, itemB);
				BaseDLL.encode_int32(buffer, ref pos_, numB);
				BaseDLL.encode_uint32(buffer, ref pos_, itemC);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int32(buffer, ref pos_, ref result);
				BaseDLL.decode_int8(buffer, ref pos_, ref resultType);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemA);
				BaseDLL.decode_int32(buffer, ref pos_, ref numA);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemB);
				BaseDLL.decode_int32(buffer, ref pos_, ref numB);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemC);
			}

			public int getLen()
			{
				int _len = 0;
				// result
				_len += 4;
				// resultType
				_len += 1;
				// itemA
				_len += 4;
				// numA
				_len += 4;
				// itemB
				_len += 4;
				// numB
				_len += 4;
				// itemC
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneFashionMergeRecordReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501019;
		public UInt32 Sequence;
		public UInt32 handleType;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, handleType);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref handleType);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, handleType);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref handleType);
			}

			public int getLen()
			{
				int _len = 0;
				// handleType
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneEquipMakeReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500954;
		public UInt32 Sequence;
		public UInt32 equipId;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, equipId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref equipId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, equipId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref equipId);
			}

			public int getLen()
			{
				int _len = 0;
				// equipId
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneEquipMakeRet : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500955;
		public UInt32 Sequence;
		public UInt32 code;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			}

			public int getLen()
			{
				int _len = 0;
				// code
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneFashionAttributeSelectReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500958;
		public UInt32 Sequence;
		public UInt64 guid;
		public Int32 attributeId;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, guid);
				BaseDLL.encode_int32(buffer, ref pos_, attributeId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref guid);
				BaseDLL.decode_int32(buffer, ref pos_, ref attributeId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, guid);
				BaseDLL.encode_int32(buffer, ref pos_, attributeId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref guid);
				BaseDLL.decode_int32(buffer, ref pos_, ref attributeId);
			}

			public int getLen()
			{
				int _len = 0;
				// guid
				_len += 8;
				// attributeId
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneFashionAttributeSelectRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500959;
		public UInt32 Sequence;
		public UInt32 result;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
			}

			public int getLen()
			{
				int _len = 0;
				// result
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 开罐记录数据
	/// </summary>
	public class OpenJarRecord : Protocol.IProtocolStream
	{
		public string name;
		public UInt32 itemId;
		public UInt32 num;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint32(buffer, ref pos_, itemId);
				BaseDLL.encode_uint32(buffer, ref pos_, num);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 nameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
				byte[] nameBytes = new byte[nameLen];
				for(int i = 0; i < nameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
				}
				name = StringHelper.BytesToString(nameBytes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref num);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				byte[] nameBytes = StringHelper.StringToUTF8Bytes(name);
				BaseDLL.encode_string(buffer, ref pos_, nameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint32(buffer, ref pos_, itemId);
				BaseDLL.encode_uint32(buffer, ref pos_, num);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 nameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref nameLen);
				byte[] nameBytes = new byte[nameLen];
				for(int i = 0; i < nameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref nameBytes[i]);
				}
				name = StringHelper.BytesToString(nameBytes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref num);
			}

			public int getLen()
			{
				int _len = 0;
				// name
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(name);
					_len += 2 + _strBytes.Length;
				}
				// itemId
				_len += 4;
				// num
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 请求开罐记录
	/// </summary>
	[Protocol]
	public class WorldOpenJarRecordReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 600901;
		public UInt32 Sequence;
		public UInt32 jarId;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, jarId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref jarId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, jarId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref jarId);
			}

			public int getLen()
			{
				int _len = 0;
				// jarId
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 返回开罐记录
	/// </summary>
	[Protocol]
	public class WorldOpenJarRecordRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 600902;
		public UInt32 Sequence;
		public UInt32 jarId;
		public OpenJarRecord[] records = new OpenJarRecord[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, jarId);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)records.Length);
				for(int i = 0; i < records.Length; i++)
				{
					records[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref jarId);
				UInt16 recordsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref recordsCnt);
				records = new OpenJarRecord[recordsCnt];
				for(int i = 0; i < records.Length; i++)
				{
					records[i] = new OpenJarRecord();
					records[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, jarId);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)records.Length);
				for(int i = 0; i < records.Length; i++)
				{
					records[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref jarId);
				UInt16 recordsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref recordsCnt);
				records = new OpenJarRecord[recordsCnt];
				for(int i = 0; i < records.Length; i++)
				{
					records[i] = new OpenJarRecord();
					records[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// jarId
				_len += 4;
				// records
				_len += 2;
				for(int j = 0; j < records.Length; j++)
				{
					_len += records[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 设置关卡药水配置
	/// </summary>
	[Protocol]
	public class SceneSetDungeonPotionReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500964;
		public UInt32 Sequence;
		public UInt32 potionId;
		public byte pos;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, potionId);
				BaseDLL.encode_int8(buffer, ref pos_, pos);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref potionId);
				BaseDLL.decode_int8(buffer, ref pos_, ref pos);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, potionId);
				BaseDLL.encode_int8(buffer, ref pos_, pos);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref potionId);
				BaseDLL.decode_int8(buffer, ref pos_, ref pos);
			}

			public int getLen()
			{
				int _len = 0;
				// potionId
				_len += 4;
				// pos
				_len += 1;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneSetDungeonPotionRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500965;
		public UInt32 Sequence;
		public UInt32 code;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			}

			public int getLen()
			{
				int _len = 0;
				// code
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 续费时限道具
	/// </summary>
	[Protocol]
	public class SceneRenewTimeItemReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500966;
		public UInt32 Sequence;
		public UInt64 itemUid;
		public UInt32 duration;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, itemUid);
				BaseDLL.encode_uint32(buffer, ref pos_, duration);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref itemUid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref duration);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, itemUid);
				BaseDLL.encode_uint32(buffer, ref pos_, duration);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref itemUid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref duration);
			}

			public int getLen()
			{
				int _len = 0;
				// itemUid
				_len += 8;
				// duration
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneRenewTimeItemRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500967;
		public UInt32 Sequence;
		public UInt32 code;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			}

			public int getLen()
			{
				int _len = 0;
				// code
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  到期被删除
	/// </summary>
	public class TimeItem : Protocol.IProtocolStream
	{
		public UInt64 itemUid;
		public UInt32 state;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, itemUid);
				BaseDLL.encode_uint32(buffer, ref pos_, state);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref itemUid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref state);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, itemUid);
				BaseDLL.encode_uint32(buffer, ref pos_, state);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref itemUid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref state);
			}

			public int getLen()
			{
				int _len = 0;
				// itemUid
				_len += 8;
				// state
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  server->client 通知道具时限状态
	/// </summary>
	[Protocol]
	public class SCNotifyTimeItem : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500968;
		public UInt32 Sequence;
		public TimeItem[] items = new TimeItem[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)items.Length);
				for(int i = 0; i < items.Length; i++)
				{
					items[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 itemsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref itemsCnt);
				items = new TimeItem[itemsCnt];
				for(int i = 0; i < items.Length; i++)
				{
					items[i] = new TimeItem();
					items[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)items.Length);
				for(int i = 0; i < items.Length; i++)
				{
					items[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 itemsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref itemsCnt);
				items = new TimeItem[itemsCnt];
				for(int i = 0; i < items.Length; i++)
				{
					items[i] = new TimeItem();
					items[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// items
				_len += 2;
				for(int j = 0; j < items.Length; j++)
				{
					_len += items[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  client->server 
	/// </summary>
	[Protocol]
	public class CSOpenMagBoxReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500969;
		public UInt32 Sequence;
		public UInt64 itemUid;
		public byte isBatch;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, itemUid);
				BaseDLL.encode_int8(buffer, ref pos_, isBatch);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref itemUid);
				BaseDLL.decode_int8(buffer, ref pos_, ref isBatch);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, itemUid);
				BaseDLL.encode_int8(buffer, ref pos_, isBatch);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref itemUid);
				BaseDLL.decode_int8(buffer, ref pos_, ref isBatch);
			}

			public int getLen()
			{
				int _len = 0;
				// itemUid
				_len += 8;
				// isBatch
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  server->client 
	/// </summary>
	[Protocol]
	public class SCOpenMagBoxRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500970;
		public UInt32 Sequence;
		public UInt32 retCode;
		public OpenJarResult[] rewards = new OpenJarResult[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, retCode);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)rewards.Length);
				for(int i = 0; i < rewards.Length; i++)
				{
					rewards[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
				UInt16 rewardsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref rewardsCnt);
				rewards = new OpenJarResult[rewardsCnt];
				for(int i = 0; i < rewards.Length; i++)
				{
					rewards[i] = new OpenJarResult();
					rewards[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, retCode);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)rewards.Length);
				for(int i = 0; i < rewards.Length; i++)
				{
					rewards[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
				UInt16 rewardsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref rewardsCnt);
				rewards = new OpenJarResult[rewardsCnt];
				for(int i = 0; i < rewards.Length; i++)
				{
					rewards[i] = new OpenJarResult();
					rewards[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// retCode
				_len += 4;
				// rewards
				_len += 2;
				for(int j = 0; j < rewards.Length; j++)
				{
					_len += rewards[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  client->server 批量出售道具请求
	/// </summary>
	[Protocol]
	public class SceneSellItemBatReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500973;
		public UInt32 Sequence;
		public UInt64[] itemUids = new UInt64[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)itemUids.Length);
				for(int i = 0; i < itemUids.Length; i++)
				{
					BaseDLL.encode_uint64(buffer, ref pos_, itemUids[i]);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 itemUidsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref itemUidsCnt);
				itemUids = new UInt64[itemUidsCnt];
				for(int i = 0; i < itemUids.Length; i++)
				{
					BaseDLL.decode_uint64(buffer, ref pos_, ref itemUids[i]);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)itemUids.Length);
				for(int i = 0; i < itemUids.Length; i++)
				{
					BaseDLL.encode_uint64(buffer, ref pos_, itemUids[i]);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 itemUidsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref itemUidsCnt);
				itemUids = new UInt64[itemUidsCnt];
				for(int i = 0; i < itemUids.Length; i++)
				{
					BaseDLL.decode_uint64(buffer, ref pos_, ref itemUids[i]);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// itemUids
				_len += 2 + 8 * itemUids.Length;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  server->client 批量出售道具返回
	/// </summary>
	[Protocol]
	public class SceneSellItemBatRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500974;
		public UInt32 Sequence;
		public UInt32 code;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			}

			public int getLen()
			{
				int _len = 0;
				// code
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  client->server 抽奖请求
	/// </summary>
	[Protocol]
	public class SceneDrawPrizeReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501006;
		public UInt32 Sequence;
		/// <summary>
		///  抽奖表id
		/// </summary>
		public UInt32 id;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, id);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, id);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
			}

			public int getLen()
			{
				int _len = 0;
				// id
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  server->client 抽奖返回
	/// </summary>
	[Protocol]
	public class SceneDrawPrizeRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501007;
		public UInt32 Sequence;
		/// <summary>
		///  抽奖表id
		/// </summary>
		public UInt32 drawPrizeId;
		/// <summary>
		///  奖励id
		/// </summary>
		public UInt32 rewardId;
		/// <summary>
		///  返回错误码
		/// </summary>
		public UInt32 retCode;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, drawPrizeId);
				BaseDLL.encode_uint32(buffer, ref pos_, rewardId);
				BaseDLL.encode_uint32(buffer, ref pos_, retCode);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref drawPrizeId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref rewardId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, drawPrizeId);
				BaseDLL.encode_uint32(buffer, ref pos_, rewardId);
				BaseDLL.encode_uint32(buffer, ref pos_, retCode);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref drawPrizeId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref rewardId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
			}

			public int getLen()
			{
				int _len = 0;
				// drawPrizeId
				_len += 4;
				// rewardId
				_len += 4;
				// retCode
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// client->server 装备回收提交请求
	/// </summary>
	[Protocol]
	public class SceneEquipRecSubcmtReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501008;
		public UInt32 Sequence;
		/// <summary>
		/// 批量装备uid
		/// </summary>
		public UInt64[] itemUids = new UInt64[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)itemUids.Length);
				for(int i = 0; i < itemUids.Length; i++)
				{
					BaseDLL.encode_uint64(buffer, ref pos_, itemUids[i]);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 itemUidsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref itemUidsCnt);
				itemUids = new UInt64[itemUidsCnt];
				for(int i = 0; i < itemUids.Length; i++)
				{
					BaseDLL.decode_uint64(buffer, ref pos_, ref itemUids[i]);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)itemUids.Length);
				for(int i = 0; i < itemUids.Length; i++)
				{
					BaseDLL.encode_uint64(buffer, ref pos_, itemUids[i]);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 itemUidsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref itemUidsCnt);
				itemUids = new UInt64[itemUidsCnt];
				for(int i = 0; i < itemUids.Length; i++)
				{
					BaseDLL.decode_uint64(buffer, ref pos_, ref itemUids[i]);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// itemUids
				_len += 2 + 8 * itemUids.Length;
				return _len;
			}
		#endregion

	}

	public class EqRecScoreItem : Protocol.IProtocolStream
	{
		/// <summary>
		///  装备uid
		/// </summary>
		public UInt64 uid;
		/// <summary>
		///  积分
		/// </summary>
		public UInt32 score;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, uid);
				BaseDLL.encode_uint32(buffer, ref pos_, score);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref uid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref score);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, uid);
				BaseDLL.encode_uint32(buffer, ref pos_, score);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref uid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref score);
			}

			public int getLen()
			{
				int _len = 0;
				// uid
				_len += 8;
				// score
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// server->client 装备回收提交返回
	/// </summary>
	[Protocol]
	public class SceneEquipRecSubcmtRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501009;
		public UInt32 Sequence;
		/// <summary>
		/// 错误码
		/// </summary>
		public UInt32 code;
		/// <summary>
		/// 装备回收积分
		/// </summary>
		public EqRecScoreItem[] items = new EqRecScoreItem[0];
		/// <summary>
		/// 当前总积分
		/// </summary>
		public UInt32 score;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)items.Length);
				for(int i = 0; i < items.Length; i++)
				{
					items[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_uint32(buffer, ref pos_, score);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
				UInt16 itemsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref itemsCnt);
				items = new EqRecScoreItem[itemsCnt];
				for(int i = 0; i < items.Length; i++)
				{
					items[i] = new EqRecScoreItem();
					items[i].decode(buffer, ref pos_);
				}
				BaseDLL.decode_uint32(buffer, ref pos_, ref score);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)items.Length);
				for(int i = 0; i < items.Length; i++)
				{
					items[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_uint32(buffer, ref pos_, score);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
				UInt16 itemsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref itemsCnt);
				items = new EqRecScoreItem[itemsCnt];
				for(int i = 0; i < items.Length; i++)
				{
					items[i] = new EqRecScoreItem();
					items[i].decode(buffer, ref pos_);
				}
				BaseDLL.decode_uint32(buffer, ref pos_, ref score);
			}

			public int getLen()
			{
				int _len = 0;
				// code
				_len += 4;
				// items
				_len += 2;
				for(int j = 0; j < items.Length; j++)
				{
					_len += items[j].getLen();
				}
				// score
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// client->server 装备赎回请求
	/// </summary>
	[Protocol]
	public class SceneEquipRecRedeemReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501010;
		public UInt32 Sequence;
		/// <summary>
		/// 装备id
		/// </summary>
		public UInt64 equid;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, equid);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref equid);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, equid);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref equid);
			}

			public int getLen()
			{
				int _len = 0;
				// equid
				_len += 8;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// server->client 装备赎回返回
	/// </summary>
	[Protocol]
	public class SceneEquipRecRedeemRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501011;
		public UInt32 Sequence;
		/// <summary>
		/// 错误码
		/// </summary>
		public UInt32 code;
		/// <summary>
		/// 扣除积分
		/// </summary>
		public UInt32 consScore;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
				BaseDLL.encode_uint32(buffer, ref pos_, consScore);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
				BaseDLL.decode_uint32(buffer, ref pos_, ref consScore);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
				BaseDLL.encode_uint32(buffer, ref pos_, consScore);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
				BaseDLL.decode_uint32(buffer, ref pos_, ref consScore);
			}

			public int getLen()
			{
				int _len = 0;
				// code
				_len += 4;
				// consScore
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// client->server 装备回收积分提升请求
	/// </summary>
	[Protocol]
	public class SceneEquipRecUpscoreReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501012;
		public UInt32 Sequence;
		/// <summary>
		/// 装备uid
		/// </summary>
		public UInt64 equid;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, equid);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref equid);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, equid);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref equid);
			}

			public int getLen()
			{
				int _len = 0;
				// equid
				_len += 8;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// server->client 装备回收积分提升返回
	/// </summary>
	[Protocol]
	public class SceneEquipRecUpscoreRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501013;
		public UInt32 Sequence;
		/// <summary>
		/// 错误码
		/// </summary>
		public UInt32 code;
		/// <summary>
		/// 提升积分值
		/// </summary>
		public UInt32 upscore;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
				BaseDLL.encode_uint32(buffer, ref pos_, upscore);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
				BaseDLL.decode_uint32(buffer, ref pos_, ref upscore);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
				BaseDLL.encode_uint32(buffer, ref pos_, upscore);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
				BaseDLL.decode_uint32(buffer, ref pos_, ref upscore);
			}

			public int getLen()
			{
				int _len = 0;
				// code
				_len += 4;
				// upscore
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// client->server 装备赎回刷新时间戳请求
	/// </summary>
	[Protocol]
	public class SceneEquipRecRedeemTmReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501014;
		public UInt32 Sequence;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
			}

			public void decode(byte[] buffer, ref int pos_)
			{
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
			}

			public int getLen()
			{
				int _len = 0;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// server->client 装备赎回刷新时间戳返回
	/// </summary>
	[Protocol]
	public class SceneEquipRecRedeemTmRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501015;
		public UInt32 Sequence;
		/// <summary>
		/// 刷新时间戳
		/// </summary>
		public UInt64 timestmap;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, timestmap);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref timestmap);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, timestmap);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref timestmap);
			}

			public int getLen()
			{
				int _len = 0;
				// timestmap
				_len += 8;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// server->client 通知装备回收重置
	/// </summary>
	[Protocol]
	public class SceneEquipRecNotifyReset : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501016;
		public UInt32 Sequence;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
			}

			public void decode(byte[] buffer, ref int pos_)
			{
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
			}

			public int getLen()
			{
				int _len = 0;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// cliet->server 装备传家请求
	/// </summary>
	[Protocol]
	public class SceneEquipTransferReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501017;
		public UInt32 Sequence;
		/// <summary>
		/// 装备uid
		/// </summary>
		public UInt64 equid;
		/// <summary>
		/// 转移石uid
		/// </summary>
		public UInt64 transferId;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, equid);
				BaseDLL.encode_uint64(buffer, ref pos_, transferId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref equid);
				BaseDLL.decode_uint64(buffer, ref pos_, ref transferId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, equid);
				BaseDLL.encode_uint64(buffer, ref pos_, transferId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref equid);
				BaseDLL.decode_uint64(buffer, ref pos_, ref transferId);
			}

			public int getLen()
			{
				int _len = 0;
				// equid
				_len += 8;
				// transferId
				_len += 8;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// server->client 装备传家返回
	/// </summary>
	[Protocol]
	public class SceneEquipTransferRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501018;
		public UInt32 Sequence;
		/// <summary>
		/// 错误码
		/// </summary>
		public UInt32 code;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			}

			public int getLen()
			{
				int _len = 0;
				// code
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// client->world 装备回收开罐子记录请求
	/// </summary>
	[Protocol]
	public class WorldEquipRecoOpenJarsRecordReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 600904;
		public UInt32 Sequence;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
			}

			public void decode(byte[] buffer, ref int pos_)
			{
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
			}

			public int getLen()
			{
				int _len = 0;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// world->client 装备回收开罐子记录返回
	/// </summary>
	[Protocol]
	public class WorldEquipRecoOpenJarsRecordRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 600905;
		public UInt32 Sequence;
		public OpenJarRecord[] records = new OpenJarRecord[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)records.Length);
				for(int i = 0; i < records.Length; i++)
				{
					records[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 recordsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref recordsCnt);
				records = new OpenJarRecord[recordsCnt];
				for(int i = 0; i < records.Length; i++)
				{
					records[i] = new OpenJarRecord();
					records[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)records.Length);
				for(int i = 0; i < records.Length; i++)
				{
					records[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 recordsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref recordsCnt);
				records = new OpenJarRecord[recordsCnt];
				for(int i = 0; i < records.Length; i++)
				{
					records[i] = new OpenJarRecord();
					records[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// records
				_len += 2;
				for(int j = 0; j < records.Length; j++)
				{
					_len += records[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// client->world 购买夺宝商品请求
	/// </summary>
	[Protocol]
	public class PayingGambleReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 707901;
		public UInt32 Sequence;
		/// <summary>
		///  夺宝商品id
		/// </summary>
		public UInt32 gambingItemId;
		/// <summary>
		///  组id
		/// </summary>
		public UInt16 groupId;
		/// <summary>
		///  投入份数
		/// </summary>
		public UInt32 investCopies;
		/// <summary>
		///  是否购入全部剩余份数(1:是,0:否)
		/// </summary>
		public byte bBuyAll;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, gambingItemId);
				BaseDLL.encode_uint16(buffer, ref pos_, groupId);
				BaseDLL.encode_uint32(buffer, ref pos_, investCopies);
				BaseDLL.encode_int8(buffer, ref pos_, bBuyAll);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref gambingItemId);
				BaseDLL.decode_uint16(buffer, ref pos_, ref groupId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref investCopies);
				BaseDLL.decode_int8(buffer, ref pos_, ref bBuyAll);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, gambingItemId);
				BaseDLL.encode_uint16(buffer, ref pos_, groupId);
				BaseDLL.encode_uint32(buffer, ref pos_, investCopies);
				BaseDLL.encode_int8(buffer, ref pos_, bBuyAll);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref gambingItemId);
				BaseDLL.decode_uint16(buffer, ref pos_, ref groupId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref investCopies);
				BaseDLL.decode_int8(buffer, ref pos_, ref bBuyAll);
			}

			public int getLen()
			{
				int _len = 0;
				// gambingItemId
				_len += 4;
				// groupId
				_len += 2;
				// investCopies
				_len += 4;
				// bBuyAll
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// world->client 购买夺宝商品返回
	/// </summary>
	[Protocol]
	public class PayingGambleRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 707902;
		public UInt32 Sequence;
		/// <summary>
		///  返回码
		/// </summary>
		public UInt32 retCode;
		/// <summary>
		///  夺宝商品id
		/// </summary>
		public UInt32 gambingItemId;
		/// <summary>
		///  组id
		/// </summary>
		public UInt16 groupId;
		/// <summary>
		///  投入份数
		/// </summary>
		public UInt32 investCopies;
		/// <summary>
		///  花费货币id
		/// </summary>
		public UInt32 costCurrencyId;
		/// <summary>
		///  花费货币数
		/// </summary>
		public UInt32 costCurrencyNum;
		public GambingItemInfo itemInfo = new GambingItemInfo();

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, retCode);
				BaseDLL.encode_uint32(buffer, ref pos_, gambingItemId);
				BaseDLL.encode_uint16(buffer, ref pos_, groupId);
				BaseDLL.encode_uint32(buffer, ref pos_, investCopies);
				BaseDLL.encode_uint32(buffer, ref pos_, costCurrencyId);
				BaseDLL.encode_uint32(buffer, ref pos_, costCurrencyNum);
				itemInfo.encode(buffer, ref pos_);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
				BaseDLL.decode_uint32(buffer, ref pos_, ref gambingItemId);
				BaseDLL.decode_uint16(buffer, ref pos_, ref groupId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref investCopies);
				BaseDLL.decode_uint32(buffer, ref pos_, ref costCurrencyId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref costCurrencyNum);
				itemInfo.decode(buffer, ref pos_);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, retCode);
				BaseDLL.encode_uint32(buffer, ref pos_, gambingItemId);
				BaseDLL.encode_uint16(buffer, ref pos_, groupId);
				BaseDLL.encode_uint32(buffer, ref pos_, investCopies);
				BaseDLL.encode_uint32(buffer, ref pos_, costCurrencyId);
				BaseDLL.encode_uint32(buffer, ref pos_, costCurrencyNum);
				itemInfo.encode(buffer, ref pos_);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
				BaseDLL.decode_uint32(buffer, ref pos_, ref gambingItemId);
				BaseDLL.decode_uint16(buffer, ref pos_, ref groupId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref investCopies);
				BaseDLL.decode_uint32(buffer, ref pos_, ref costCurrencyId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref costCurrencyNum);
				itemInfo.decode(buffer, ref pos_);
			}

			public int getLen()
			{
				int _len = 0;
				// retCode
				_len += 4;
				// gambingItemId
				_len += 4;
				// groupId
				_len += 2;
				// investCopies
				_len += 4;
				// costCurrencyId
				_len += 4;
				// costCurrencyNum
				_len += 4;
				// itemInfo
				_len += itemInfo.getLen();
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// client->world 夺宝商品数据查询请求
	/// </summary>
	[Protocol]
	public class GambingItemQueryReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 707903;
		public UInt32 Sequence;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
			}

			public void decode(byte[] buffer, ref int pos_)
			{
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
			}

			public int getLen()
			{
				int _len = 0;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// world->client 夺宝商品数据查询返回
	/// </summary>
	[Protocol]
	public class GambingItemQueryRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 707904;
		public UInt32 Sequence;
		public GambingItemInfo[] gambingItems = new GambingItemInfo[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)gambingItems.Length);
				for(int i = 0; i < gambingItems.Length; i++)
				{
					gambingItems[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 gambingItemsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref gambingItemsCnt);
				gambingItems = new GambingItemInfo[gambingItemsCnt];
				for(int i = 0; i < gambingItems.Length; i++)
				{
					gambingItems[i] = new GambingItemInfo();
					gambingItems[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)gambingItems.Length);
				for(int i = 0; i < gambingItems.Length; i++)
				{
					gambingItems[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 gambingItemsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref gambingItemsCnt);
				gambingItems = new GambingItemInfo[gambingItemsCnt];
				for(int i = 0; i < gambingItems.Length; i++)
				{
					gambingItems[i] = new GambingItemInfo();
					gambingItems[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// gambingItems
				_len += 2;
				for(int j = 0; j < gambingItems.Length; j++)
				{
					_len += gambingItems[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// client->world 我的夺宝数据查询请求
	/// </summary>
	[Protocol]
	public class GambingMineQueryReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 707905;
		public UInt32 Sequence;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
			}

			public void decode(byte[] buffer, ref int pos_)
			{
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
			}

			public int getLen()
			{
				int _len = 0;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// world->client 我的夺宝数据查询返回
	/// </summary>
	[Protocol]
	public class GambingMineQueryRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 707906;
		public UInt32 Sequence;
		public GambingMineInfo[] mineGambingInfo = new GambingMineInfo[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)mineGambingInfo.Length);
				for(int i = 0; i < mineGambingInfo.Length; i++)
				{
					mineGambingInfo[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 mineGambingInfoCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref mineGambingInfoCnt);
				mineGambingInfo = new GambingMineInfo[mineGambingInfoCnt];
				for(int i = 0; i < mineGambingInfo.Length; i++)
				{
					mineGambingInfo[i] = new GambingMineInfo();
					mineGambingInfo[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)mineGambingInfo.Length);
				for(int i = 0; i < mineGambingInfo.Length; i++)
				{
					mineGambingInfo[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 mineGambingInfoCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref mineGambingInfoCnt);
				mineGambingInfo = new GambingMineInfo[mineGambingInfoCnt];
				for(int i = 0; i < mineGambingInfo.Length; i++)
				{
					mineGambingInfo[i] = new GambingMineInfo();
					mineGambingInfo[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// mineGambingInfo
				_len += 2;
				for(int j = 0; j < mineGambingInfo.Length; j++)
				{
					_len += mineGambingInfo[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// client->world 夺宝记录查询
	/// </summary>
	[Protocol]
	public class GambingRecordQueryReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 707907;
		public UInt32 Sequence;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
			}

			public void decode(byte[] buffer, ref int pos_)
			{
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
			}

			public int getLen()
			{
				int _len = 0;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// world->client 夺宝记录查询返回
	/// </summary>
	[Protocol]
	public class GambingRecordQueryRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 707908;
		public UInt32 Sequence;
		public GambingItemRecordData[] gambingRecordDatas = new GambingItemRecordData[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)gambingRecordDatas.Length);
				for(int i = 0; i < gambingRecordDatas.Length; i++)
				{
					gambingRecordDatas[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 gambingRecordDatasCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref gambingRecordDatasCnt);
				gambingRecordDatas = new GambingItemRecordData[gambingRecordDatasCnt];
				for(int i = 0; i < gambingRecordDatas.Length; i++)
				{
					gambingRecordDatas[i] = new GambingItemRecordData();
					gambingRecordDatas[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)gambingRecordDatas.Length);
				for(int i = 0; i < gambingRecordDatas.Length; i++)
				{
					gambingRecordDatas[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 gambingRecordDatasCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref gambingRecordDatasCnt);
				gambingRecordDatas = new GambingItemRecordData[gambingRecordDatasCnt];
				for(int i = 0; i < gambingRecordDatas.Length; i++)
				{
					gambingRecordDatas[i] = new GambingItemRecordData();
					gambingRecordDatas[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// gambingRecordDatas
				_len += 2;
				for(int j = 0; j < gambingRecordDatas.Length; j++)
				{
					_len += gambingRecordDatas[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// world->client 
	/// </summary>
	[Protocol]
	public class GambingLotterySync : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 707909;
		public UInt32 Sequence;
		/// <summary>
		///  获得者夺宝数据
		/// </summary>
		public GambingParticipantInfo gainersGambingInfo = new GambingParticipantInfo();
		/// <summary>
		///  参与者夺宝数据
		/// </summary>
		public GambingParticipantInfo[] participantsGambingInfo = new GambingParticipantInfo[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				gainersGambingInfo.encode(buffer, ref pos_);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)participantsGambingInfo.Length);
				for(int i = 0; i < participantsGambingInfo.Length; i++)
				{
					participantsGambingInfo[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				gainersGambingInfo.decode(buffer, ref pos_);
				UInt16 participantsGambingInfoCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref participantsGambingInfoCnt);
				participantsGambingInfo = new GambingParticipantInfo[participantsGambingInfoCnt];
				for(int i = 0; i < participantsGambingInfo.Length; i++)
				{
					participantsGambingInfo[i] = new GambingParticipantInfo();
					participantsGambingInfo[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				gainersGambingInfo.encode(buffer, ref pos_);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)participantsGambingInfo.Length);
				for(int i = 0; i < participantsGambingInfo.Length; i++)
				{
					participantsGambingInfo[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				gainersGambingInfo.decode(buffer, ref pos_);
				UInt16 participantsGambingInfoCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref participantsGambingInfoCnt);
				participantsGambingInfo = new GambingParticipantInfo[participantsGambingInfoCnt];
				for(int i = 0; i < participantsGambingInfo.Length; i++)
				{
					participantsGambingInfo[i] = new GambingParticipantInfo();
					participantsGambingInfo[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// gainersGambingInfo
				_len += gainersGambingInfo.getLen();
				// participantsGambingInfo
				_len += 2;
				for(int j = 0; j < participantsGambingInfo.Length; j++)
				{
					_len += participantsGambingInfo[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  client -> scene 礼包信息请求
	/// </summary>
	[Protocol]
	public class SceneGiftPackInfoReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 503211;
		public UInt32 Sequence;
		public UInt32[] giftPackIds = new UInt32[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)giftPackIds.Length);
				for(int i = 0; i < giftPackIds.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, giftPackIds[i]);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 giftPackIdsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref giftPackIdsCnt);
				giftPackIds = new UInt32[giftPackIdsCnt];
				for(int i = 0; i < giftPackIds.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref giftPackIds[i]);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)giftPackIds.Length);
				for(int i = 0; i < giftPackIds.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, giftPackIds[i]);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 giftPackIdsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref giftPackIdsCnt);
				giftPackIds = new UInt32[giftPackIdsCnt];
				for(int i = 0; i < giftPackIds.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref giftPackIds[i]);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// giftPackIds
				_len += 2 + 4 * giftPackIds.Length;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  scene -> client 礼包信息返回
	/// </summary>
	[Protocol]
	public class SceneGiftPackInfoRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 503212;
		public UInt32 Sequence;
		public GiftPackSyncInfo[] giftPacks = new GiftPackSyncInfo[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)giftPacks.Length);
				for(int i = 0; i < giftPacks.Length; i++)
				{
					giftPacks[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 giftPacksCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref giftPacksCnt);
				giftPacks = new GiftPackSyncInfo[giftPacksCnt];
				for(int i = 0; i < giftPacks.Length; i++)
				{
					giftPacks[i] = new GiftPackSyncInfo();
					giftPacks[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)giftPacks.Length);
				for(int i = 0; i < giftPacks.Length; i++)
				{
					giftPacks[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 giftPacksCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref giftPacksCnt);
				giftPacks = new GiftPackSyncInfo[giftPacksCnt];
				for(int i = 0; i < giftPacks.Length; i++)
				{
					giftPacks[i] = new GiftPackSyncInfo();
					giftPacks[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// giftPacks
				_len += 2;
				for(int j = 0; j < giftPacks.Length; j++)
				{
					_len += giftPacks[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  client->scene 同步抗魔值
	/// </summary>
	[Protocol]
	public class SceneSyncResistMagicReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501021;
		public UInt32 Sequence;
		public UInt32 resist_magic;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, resist_magic);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref resist_magic);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, resist_magic);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref resist_magic);
			}

			public int getLen()
			{
				int _len = 0;
				// resist_magic
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  client->scene装备加锁解锁
	/// </summary>
	[Protocol]
	public class SceneItemLockReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501025;
		public UInt32 Sequence;
		/// <summary>
		///  操作类型，0解锁，否则加锁
		/// </summary>
		public UInt32 opType;
		/// <summary>
		///  道具uid
		/// </summary>
		public UInt64 itemUid;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, opType);
				BaseDLL.encode_uint64(buffer, ref pos_, itemUid);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref opType);
				BaseDLL.decode_uint64(buffer, ref pos_, ref itemUid);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, opType);
				BaseDLL.encode_uint64(buffer, ref pos_, itemUid);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref opType);
				BaseDLL.decode_uint64(buffer, ref pos_, ref itemUid);
			}

			public int getLen()
			{
				int _len = 0;
				// opType
				_len += 4;
				// itemUid
				_len += 8;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  scene -> client装备加锁解锁返回
	/// </summary>
	[Protocol]
	public class SceneItemLockRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501026;
		public UInt32 Sequence;
		/// <summary>
		///  道具uid
		/// </summary>
		public UInt64 itemUid;
		/// <summary>
		///  返回值
		/// </summary>
		public UInt32 ret;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, itemUid);
				BaseDLL.encode_uint32(buffer, ref pos_, ret);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref itemUid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref ret);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, itemUid);
				BaseDLL.encode_uint32(buffer, ref pos_, ret);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref itemUid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref ret);
			}

			public int getLen()
			{
				int _len = 0;
				// itemUid
				_len += 8;
				// ret
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  client->scene 设置时装武器显示请求
	/// </summary>
	[Protocol]
	public class SceneSetFashionWeaponShowReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501027;
		public UInt32 Sequence;
		/// <summary>
		/// 是否显示时装武器(1:是,0:否)
		/// </summary>
		public byte isShow;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, isShow);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref isShow);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, isShow);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref isShow);
			}

			public int getLen()
			{
				int _len = 0;
				// isShow
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  scene->client 设置时装武器显示返回
	/// </summary>
	[Protocol]
	public class SceneSetFashionWeaponShowRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501028;
		public UInt32 Sequence;
		/// <summary>
		///  返回值
		/// </summary>
		public UInt32 ret;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, ret);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref ret);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, ret);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref ret);
			}

			public int getLen()
			{
				int _len = 0;
				// ret
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  client->scene 换装节活动时装合成请求
	/// </summary>
	[Protocol]
	public class SceneFashionChangeActiveMergeReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501029;
		public UInt32 Sequence;
		/// <summary>
		/// 被合时装对象id
		/// </summary>
		public UInt64 fashionId;
		/// <summary>
		/// 换装卷对象id（选择背包中数量最大的换装卷）
		/// </summary>
		public UInt64 ticketId;
		/// <summary>
		/// 选择必定合成时装道具id
		/// </summary>
		public UInt32 choiceComFashionId;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, fashionId);
				BaseDLL.encode_uint64(buffer, ref pos_, ticketId);
				BaseDLL.encode_uint32(buffer, ref pos_, choiceComFashionId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref fashionId);
				BaseDLL.decode_uint64(buffer, ref pos_, ref ticketId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref choiceComFashionId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, fashionId);
				BaseDLL.encode_uint64(buffer, ref pos_, ticketId);
				BaseDLL.encode_uint32(buffer, ref pos_, choiceComFashionId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref fashionId);
				BaseDLL.decode_uint64(buffer, ref pos_, ref ticketId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref choiceComFashionId);
			}

			public int getLen()
			{
				int _len = 0;
				// fashionId
				_len += 8;
				// ticketId
				_len += 8;
				// choiceComFashionId
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  scene->client 换装节活动时装合成返回
	/// </summary>
	[Protocol]
	public class SceneFashionChangeActiveMergeRet : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501030;
		public UInt32 Sequence;
		/// <summary>
		/// 结果
		/// </summary>
		public UInt32 ret;
		/// <summary>
		/// [1]:未合出进击的勇士时装 [2]合出进击的勇士时装
		/// </summary>
		public byte type;
		/// <summary>
		/// 合出普通时装id
		/// </summary>
		public UInt32 commonId;
		/// <summary>
		/// 合出的进击的勇士时装id
		/// </summary>
		public UInt32 advanceId;
		/// <summary>
		/// 套装全部合出
		/// </summary>
		public byte allMerged;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, ret);
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_uint32(buffer, ref pos_, commonId);
				BaseDLL.encode_uint32(buffer, ref pos_, advanceId);
				BaseDLL.encode_int8(buffer, ref pos_, allMerged);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref ret);
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_uint32(buffer, ref pos_, ref commonId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref advanceId);
				BaseDLL.decode_int8(buffer, ref pos_, ref allMerged);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, ret);
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_uint32(buffer, ref pos_, commonId);
				BaseDLL.encode_uint32(buffer, ref pos_, advanceId);
				BaseDLL.encode_int8(buffer, ref pos_, allMerged);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref ret);
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_uint32(buffer, ref pos_, ref commonId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref advanceId);
				BaseDLL.decode_int8(buffer, ref pos_, ref allMerged);
			}

			public int getLen()
			{
				int _len = 0;
				// ret
				_len += 4;
				// type
				_len += 1;
				// commonId
				_len += 4;
				// advanceId
				_len += 4;
				// allMerged
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  client->scene 强化券合成请求
	/// </summary>
	[Protocol]
	public class SceneStrengthenTicketSynthesisReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501031;
		public UInt32 Sequence;
		/// <summary>
		///  合成方案
		/// </summary>
		public UInt32 synthesisPlan;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, synthesisPlan);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref synthesisPlan);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, synthesisPlan);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref synthesisPlan);
			}

			public int getLen()
			{
				int _len = 0;
				// synthesisPlan
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  scene->client 强化券合成返回
	/// </summary>
	[Protocol]
	public class SceneStrengthenTicketSynthesisRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501032;
		public UInt32 Sequence;
		/// <summary>
		///  结果
		/// </summary>
		public UInt32 ret;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, ret);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref ret);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, ret);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref ret);
			}

			public int getLen()
			{
				int _len = 0;
				// ret
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  client->scene 强化券融合请求
	/// </summary>
	[Protocol]
	public class SceneStrengthenTicketFuseReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501037;
		public UInt32 Sequence;
		/// <summary>
		///  选择要融合的强化券
		/// </summary>
		public UInt64 pickTicketA;
		public UInt64 pickTicketB;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, pickTicketA);
				BaseDLL.encode_uint64(buffer, ref pos_, pickTicketB);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref pickTicketA);
				BaseDLL.decode_uint64(buffer, ref pos_, ref pickTicketB);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, pickTicketA);
				BaseDLL.encode_uint64(buffer, ref pos_, pickTicketB);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref pickTicketA);
				BaseDLL.decode_uint64(buffer, ref pos_, ref pickTicketB);
			}

			public int getLen()
			{
				int _len = 0;
				// pickTicketA
				_len += 8;
				// pickTicketB
				_len += 8;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  scene->client 强化券融合返回
	/// </summary>
	[Protocol]
	public class SceneStrengthenTicketFuseRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501038;
		public UInt32 Sequence;
		/// <summary>
		///  结果
		/// </summary>
		public UInt32 ret;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, ret);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref ret);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, ret);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref ret);
			}

			public int getLen()
			{
				int _len = 0;
				// ret
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// client->scene 装备镶嵌宝珠请求
	/// </summary>
	[Protocol]
	public class SceneMountPreciousBeadReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501033;
		public UInt32 Sequence;
		/// <summary>
		///  宝珠uid
		/// </summary>
		public UInt64 preciousBeadUid;
		/// <summary>
		///  道具uid
		/// </summary>
		public UInt64 itemUid;
		/// <summary>
		///  孔索引
		/// </summary>
		public byte holeIndex;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, preciousBeadUid);
				BaseDLL.encode_uint64(buffer, ref pos_, itemUid);
				BaseDLL.encode_int8(buffer, ref pos_, holeIndex);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref preciousBeadUid);
				BaseDLL.decode_uint64(buffer, ref pos_, ref itemUid);
				BaseDLL.decode_int8(buffer, ref pos_, ref holeIndex);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, preciousBeadUid);
				BaseDLL.encode_uint64(buffer, ref pos_, itemUid);
				BaseDLL.encode_int8(buffer, ref pos_, holeIndex);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref preciousBeadUid);
				BaseDLL.decode_uint64(buffer, ref pos_, ref itemUid);
				BaseDLL.decode_int8(buffer, ref pos_, ref holeIndex);
			}

			public int getLen()
			{
				int _len = 0;
				// preciousBeadUid
				_len += 8;
				// itemUid
				_len += 8;
				// holeIndex
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// scene->client 装备镶嵌宝珠返回
	/// </summary>
	[Protocol]
	public class SceneMountPreciousBeadRet : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501034;
		public UInt32 Sequence;
		/// <summary>
		///  结果
		/// </summary>
		public UInt32 code;
		/// <summary>
		///  宝珠id
		/// </summary>
		public UInt32 preciousBeadId;
		/// <summary>
		///  道具uid
		/// </summary>
		public UInt64 itemUid;
		/// <summary>
		///  孔索引
		/// </summary>
		public byte holeIndex;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
				BaseDLL.encode_uint32(buffer, ref pos_, preciousBeadId);
				BaseDLL.encode_uint64(buffer, ref pos_, itemUid);
				BaseDLL.encode_int8(buffer, ref pos_, holeIndex);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
				BaseDLL.decode_uint32(buffer, ref pos_, ref preciousBeadId);
				BaseDLL.decode_uint64(buffer, ref pos_, ref itemUid);
				BaseDLL.decode_int8(buffer, ref pos_, ref holeIndex);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
				BaseDLL.encode_uint32(buffer, ref pos_, preciousBeadId);
				BaseDLL.encode_uint64(buffer, ref pos_, itemUid);
				BaseDLL.encode_int8(buffer, ref pos_, holeIndex);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
				BaseDLL.decode_uint32(buffer, ref pos_, ref preciousBeadId);
				BaseDLL.decode_uint64(buffer, ref pos_, ref itemUid);
				BaseDLL.decode_int8(buffer, ref pos_, ref holeIndex);
			}

			public int getLen()
			{
				int _len = 0;
				// code
				_len += 4;
				// preciousBeadId
				_len += 4;
				// itemUid
				_len += 8;
				// holeIndex
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// client->scene 装备宝珠摘除请求
	/// </summary>
	[Protocol]
	public class SceneExtirpePreciousBeadReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501035;
		public UInt32 Sequence;
		/// <summary>
		///  道具uid
		/// </summary>
		public UInt64 itemUid;
		/// <summary>
		///  孔索引
		/// </summary>
		public byte holeIndex;
		/// <summary>
		///  杵id
		/// </summary>
		public UInt32 pestleId;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, itemUid);
				BaseDLL.encode_int8(buffer, ref pos_, holeIndex);
				BaseDLL.encode_uint32(buffer, ref pos_, pestleId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref itemUid);
				BaseDLL.decode_int8(buffer, ref pos_, ref holeIndex);
				BaseDLL.decode_uint32(buffer, ref pos_, ref pestleId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, itemUid);
				BaseDLL.encode_int8(buffer, ref pos_, holeIndex);
				BaseDLL.encode_uint32(buffer, ref pos_, pestleId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref itemUid);
				BaseDLL.decode_int8(buffer, ref pos_, ref holeIndex);
				BaseDLL.decode_uint32(buffer, ref pos_, ref pestleId);
			}

			public int getLen()
			{
				int _len = 0;
				// itemUid
				_len += 8;
				// holeIndex
				_len += 1;
				// pestleId
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// scene->clinet 装备宝珠摘除返回
	/// </summary>
	[Protocol]
	public class SceneExtirpePreciousBeadRet : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501036;
		public UInt32 Sequence;
		/// <summary>
		///  结果
		/// </summary>
		public UInt32 code;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			}

			public int getLen()
			{
				int _len = 0;
				// code
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  client->scene 升级宝珠请求
	/// </summary>
	[Protocol]
	public class SceneUpgradePreciousbeadReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501039;
		public UInt32 Sequence;
		/// <summary>
		/// 类型 [0]:未镶嵌 [1]:已镶嵌
		/// </summary>
		public byte mountedType;
		/// <summary>
		/// 宝珠Guid 类型0时设置
		/// </summary>
		public UInt64 precGuid;
		/// <summary>
		/// 装备guid 类型1时设置
		/// </summary>
		public UInt64 equipGuid;
		/// <summary>
		/// 装备宝珠孔索引 类型1时设置
		/// </summary>
		public byte eqPrecHoleIndex;
		/// <summary>
		/// 宝珠id 类型1时设置
		/// </summary>
		public UInt32 precId;
		/// <summary>
		/// 选择消耗宝珠id 
		/// </summary>
		public UInt32 consumePrecId;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, mountedType);
				BaseDLL.encode_uint64(buffer, ref pos_, precGuid);
				BaseDLL.encode_uint64(buffer, ref pos_, equipGuid);
				BaseDLL.encode_int8(buffer, ref pos_, eqPrecHoleIndex);
				BaseDLL.encode_uint32(buffer, ref pos_, precId);
				BaseDLL.encode_uint32(buffer, ref pos_, consumePrecId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref mountedType);
				BaseDLL.decode_uint64(buffer, ref pos_, ref precGuid);
				BaseDLL.decode_uint64(buffer, ref pos_, ref equipGuid);
				BaseDLL.decode_int8(buffer, ref pos_, ref eqPrecHoleIndex);
				BaseDLL.decode_uint32(buffer, ref pos_, ref precId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref consumePrecId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, mountedType);
				BaseDLL.encode_uint64(buffer, ref pos_, precGuid);
				BaseDLL.encode_uint64(buffer, ref pos_, equipGuid);
				BaseDLL.encode_int8(buffer, ref pos_, eqPrecHoleIndex);
				BaseDLL.encode_uint32(buffer, ref pos_, precId);
				BaseDLL.encode_uint32(buffer, ref pos_, consumePrecId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref mountedType);
				BaseDLL.decode_uint64(buffer, ref pos_, ref precGuid);
				BaseDLL.decode_uint64(buffer, ref pos_, ref equipGuid);
				BaseDLL.decode_int8(buffer, ref pos_, ref eqPrecHoleIndex);
				BaseDLL.decode_uint32(buffer, ref pos_, ref precId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref consumePrecId);
			}

			public int getLen()
			{
				int _len = 0;
				// mountedType
				_len += 1;
				// precGuid
				_len += 8;
				// equipGuid
				_len += 8;
				// eqPrecHoleIndex
				_len += 1;
				// precId
				_len += 4;
				// consumePrecId
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  scene->client 升级宝珠返回
	/// </summary>
	[Protocol]
	public class SceneUpgradePreciousbeadRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501040;
		public UInt32 Sequence;
		/// <summary>
		/// 返回码
		/// </summary>
		public UInt32 retCode;
		/// <summary>
		/// 类型 [0]:未镶嵌 [1]:已镶嵌
		/// </summary>
		public byte mountedType;
		/// <summary>
		/// 装备guid 类型1时设置
		/// </summary>
		public UInt64 equipGuid;
		/// <summary>
		/// 升级成功后宝珠id
		/// </summary>
		public UInt32 precId;
		/// <summary>
		/// 附加buff id
		/// </summary>
		public UInt32 addBuffId;
		/// <summary>
		/// 升级后新的宝珠uid
		/// </summary>
		public UInt64 newPrebeadUid;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, retCode);
				BaseDLL.encode_int8(buffer, ref pos_, mountedType);
				BaseDLL.encode_uint64(buffer, ref pos_, equipGuid);
				BaseDLL.encode_uint32(buffer, ref pos_, precId);
				BaseDLL.encode_uint32(buffer, ref pos_, addBuffId);
				BaseDLL.encode_uint64(buffer, ref pos_, newPrebeadUid);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
				BaseDLL.decode_int8(buffer, ref pos_, ref mountedType);
				BaseDLL.decode_uint64(buffer, ref pos_, ref equipGuid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref precId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref addBuffId);
				BaseDLL.decode_uint64(buffer, ref pos_, ref newPrebeadUid);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, retCode);
				BaseDLL.encode_int8(buffer, ref pos_, mountedType);
				BaseDLL.encode_uint64(buffer, ref pos_, equipGuid);
				BaseDLL.encode_uint32(buffer, ref pos_, precId);
				BaseDLL.encode_uint32(buffer, ref pos_, addBuffId);
				BaseDLL.encode_uint64(buffer, ref pos_, newPrebeadUid);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
				BaseDLL.decode_int8(buffer, ref pos_, ref mountedType);
				BaseDLL.decode_uint64(buffer, ref pos_, ref equipGuid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref precId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref addBuffId);
				BaseDLL.decode_uint64(buffer, ref pos_, ref newPrebeadUid);
			}

			public int getLen()
			{
				int _len = 0;
				// retCode
				_len += 4;
				// mountedType
				_len += 1;
				// equipGuid
				_len += 8;
				// precId
				_len += 4;
				// addBuffId
				_len += 4;
				// newPrebeadUid
				_len += 8;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  宝珠替换请求
	/// </summary>
	[Protocol]
	public class SceneReplacePreciousBeadReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501042;
		public UInt32 Sequence;
		/// <summary>
		///  道具uid
		/// </summary>
		public UInt64 itemUid;
		/// <summary>
		///  宝珠uid
		/// </summary>
		public UInt64 preciousBeadUid;
		/// <summary>
		///  孔索引
		/// </summary>
		public byte holeIndex;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, itemUid);
				BaseDLL.encode_uint64(buffer, ref pos_, preciousBeadUid);
				BaseDLL.encode_int8(buffer, ref pos_, holeIndex);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref itemUid);
				BaseDLL.decode_uint64(buffer, ref pos_, ref preciousBeadUid);
				BaseDLL.decode_int8(buffer, ref pos_, ref holeIndex);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, itemUid);
				BaseDLL.encode_uint64(buffer, ref pos_, preciousBeadUid);
				BaseDLL.encode_int8(buffer, ref pos_, holeIndex);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref itemUid);
				BaseDLL.decode_uint64(buffer, ref pos_, ref preciousBeadUid);
				BaseDLL.decode_int8(buffer, ref pos_, ref holeIndex);
			}

			public int getLen()
			{
				int _len = 0;
				// itemUid
				_len += 8;
				// preciousBeadUid
				_len += 8;
				// holeIndex
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  宝珠替换返回
	/// </summary>
	[Protocol]
	public class SceneReplacePreciousBeadRet : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501043;
		public UInt32 Sequence;
		/// <summary>
		/// 返回码
		/// </summary>
		public UInt32 retCode;
		/// <summary>
		///  宝珠id
		/// </summary>
		public UInt32 preciousBeadId;
		/// <summary>
		///  道具uid
		/// </summary>
		public UInt64 itemUid;
		/// <summary>
		///  孔索引
		/// </summary>
		public byte holeIndex;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, retCode);
				BaseDLL.encode_uint32(buffer, ref pos_, preciousBeadId);
				BaseDLL.encode_uint64(buffer, ref pos_, itemUid);
				BaseDLL.encode_int8(buffer, ref pos_, holeIndex);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
				BaseDLL.decode_uint32(buffer, ref pos_, ref preciousBeadId);
				BaseDLL.decode_uint64(buffer, ref pos_, ref itemUid);
				BaseDLL.decode_int8(buffer, ref pos_, ref holeIndex);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, retCode);
				BaseDLL.encode_uint32(buffer, ref pos_, preciousBeadId);
				BaseDLL.encode_uint64(buffer, ref pos_, itemUid);
				BaseDLL.encode_int8(buffer, ref pos_, holeIndex);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
				BaseDLL.decode_uint32(buffer, ref pos_, ref preciousBeadId);
				BaseDLL.decode_uint64(buffer, ref pos_, ref itemUid);
				BaseDLL.decode_int8(buffer, ref pos_, ref holeIndex);
			}

			public int getLen()
			{
				int _len = 0;
				// retCode
				_len += 4;
				// preciousBeadId
				_len += 4;
				// itemUid
				_len += 8;
				// holeIndex
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  ************************神器罐子活动协议**************
	/// </summary>
	/// <summary>
	///  活动神器罐购买次数请求
	/// </summary>
	[Protocol]
	public class SceneArtifactJarBuyCntReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501046;
		public UInt32 Sequence;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
			}

			public void decode(byte[] buffer, ref int pos_)
			{
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
			}

			public int getLen()
			{
				int _len = 0;
				return _len;
			}
		#endregion

	}

	public class ArtifactJarBuy : Protocol.IProtocolStream
	{
		/// <summary>
		///  罐子id
		/// </summary>
		public UInt32 jarId;
		/// <summary>
		///  购买次数
		/// </summary>
		public UInt32 buyCnt;
		/// <summary>
		///  总次数
		/// </summary>
		public UInt32 totalCnt;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, jarId);
				BaseDLL.encode_uint32(buffer, ref pos_, buyCnt);
				BaseDLL.encode_uint32(buffer, ref pos_, totalCnt);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref jarId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref buyCnt);
				BaseDLL.decode_uint32(buffer, ref pos_, ref totalCnt);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, jarId);
				BaseDLL.encode_uint32(buffer, ref pos_, buyCnt);
				BaseDLL.encode_uint32(buffer, ref pos_, totalCnt);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref jarId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref buyCnt);
				BaseDLL.decode_uint32(buffer, ref pos_, ref totalCnt);
			}

			public int getLen()
			{
				int _len = 0;
				// jarId
				_len += 4;
				// buyCnt
				_len += 4;
				// totalCnt
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  活动神器罐购买次数返回
	/// </summary>
	[Protocol]
	public class SceneArtifactJarBuyCntRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501047;
		public UInt32 Sequence;
		public ArtifactJarBuy[] artifactJarBuyList = new ArtifactJarBuy[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)artifactJarBuyList.Length);
				for(int i = 0; i < artifactJarBuyList.Length; i++)
				{
					artifactJarBuyList[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 artifactJarBuyListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref artifactJarBuyListCnt);
				artifactJarBuyList = new ArtifactJarBuy[artifactJarBuyListCnt];
				for(int i = 0; i < artifactJarBuyList.Length; i++)
				{
					artifactJarBuyList[i] = new ArtifactJarBuy();
					artifactJarBuyList[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)artifactJarBuyList.Length);
				for(int i = 0; i < artifactJarBuyList.Length; i++)
				{
					artifactJarBuyList[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 artifactJarBuyListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref artifactJarBuyListCnt);
				artifactJarBuyList = new ArtifactJarBuy[artifactJarBuyListCnt];
				for(int i = 0; i < artifactJarBuyList.Length; i++)
				{
					artifactJarBuyList[i] = new ArtifactJarBuy();
					artifactJarBuyList[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// artifactJarBuyList
				_len += 2;
				for(int j = 0; j < artifactJarBuyList.Length; j++)
				{
					_len += artifactJarBuyList[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  神器罐活动抽奖记录请求
	/// </summary>
	[Protocol]
	public class GASArtifactJarLotteryRecordReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 700901;
		public UInt32 Sequence;
		/// <summary>
		///  罐子ID
		/// </summary>
		public UInt32 jarId;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, jarId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref jarId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, jarId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref jarId);
			}

			public int getLen()
			{
				int _len = 0;
				// jarId
				_len += 4;
				return _len;
			}
		#endregion

	}

	public class ArtifactJarLotteryRecord : Protocol.IProtocolStream
	{
		/// <summary>
		///  服务器名字
		/// </summary>
		public string serverName;
		/// <summary>
		///  玩家名字
		/// </summary>
		public string playerName;
		/// <summary>
		///  记录时间
		/// </summary>
		public UInt64 recordTime;
		/// <summary>
		///  道具id
		/// </summary>
		public UInt32 itemId;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				byte[] serverNameBytes = StringHelper.StringToUTF8Bytes(serverName);
				BaseDLL.encode_string(buffer, ref pos_, serverNameBytes, (UInt16)(buffer.Length - pos_));
				byte[] playerNameBytes = StringHelper.StringToUTF8Bytes(playerName);
				BaseDLL.encode_string(buffer, ref pos_, playerNameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint64(buffer, ref pos_, recordTime);
				BaseDLL.encode_uint32(buffer, ref pos_, itemId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 serverNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref serverNameLen);
				byte[] serverNameBytes = new byte[serverNameLen];
				for(int i = 0; i < serverNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref serverNameBytes[i]);
				}
				serverName = StringHelper.BytesToString(serverNameBytes);
				UInt16 playerNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref playerNameLen);
				byte[] playerNameBytes = new byte[playerNameLen];
				for(int i = 0; i < playerNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref playerNameBytes[i]);
				}
				playerName = StringHelper.BytesToString(playerNameBytes);
				BaseDLL.decode_uint64(buffer, ref pos_, ref recordTime);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				byte[] serverNameBytes = StringHelper.StringToUTF8Bytes(serverName);
				BaseDLL.encode_string(buffer, ref pos_, serverNameBytes, (UInt16)(buffer.Length - pos_));
				byte[] playerNameBytes = StringHelper.StringToUTF8Bytes(playerName);
				BaseDLL.encode_string(buffer, ref pos_, playerNameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint64(buffer, ref pos_, recordTime);
				BaseDLL.encode_uint32(buffer, ref pos_, itemId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 serverNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref serverNameLen);
				byte[] serverNameBytes = new byte[serverNameLen];
				for(int i = 0; i < serverNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref serverNameBytes[i]);
				}
				serverName = StringHelper.BytesToString(serverNameBytes);
				UInt16 playerNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref playerNameLen);
				byte[] playerNameBytes = new byte[playerNameLen];
				for(int i = 0; i < playerNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref playerNameBytes[i]);
				}
				playerName = StringHelper.BytesToString(playerNameBytes);
				BaseDLL.decode_uint64(buffer, ref pos_, ref recordTime);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemId);
			}

			public int getLen()
			{
				int _len = 0;
				// serverName
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(serverName);
					_len += 2 + _strBytes.Length;
				}
				// playerName
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(playerName);
					_len += 2 + _strBytes.Length;
				}
				// recordTime
				_len += 8;
				// itemId
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  神器罐活动抽奖记录返回
	/// </summary>
	[Protocol]
	public class GASArtifactJarLotteryRecordRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 700902;
		public UInt32 Sequence;
		/// <summary>
		///  罐子ID
		/// </summary>
		public UInt32 jarId;
		/// <summary>
		///  记录列表
		/// </summary>
		public ArtifactJarLotteryRecord[] lotteryRecordList = new ArtifactJarLotteryRecord[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, jarId);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)lotteryRecordList.Length);
				for(int i = 0; i < lotteryRecordList.Length; i++)
				{
					lotteryRecordList[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref jarId);
				UInt16 lotteryRecordListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref lotteryRecordListCnt);
				lotteryRecordList = new ArtifactJarLotteryRecord[lotteryRecordListCnt];
				for(int i = 0; i < lotteryRecordList.Length; i++)
				{
					lotteryRecordList[i] = new ArtifactJarLotteryRecord();
					lotteryRecordList[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, jarId);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)lotteryRecordList.Length);
				for(int i = 0; i < lotteryRecordList.Length; i++)
				{
					lotteryRecordList[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref jarId);
				UInt16 lotteryRecordListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref lotteryRecordListCnt);
				lotteryRecordList = new ArtifactJarLotteryRecord[lotteryRecordListCnt];
				for(int i = 0; i < lotteryRecordList.Length; i++)
				{
					lotteryRecordList[i] = new ArtifactJarLotteryRecord();
					lotteryRecordList[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// jarId
				_len += 4;
				// lotteryRecordList
				_len += 2;
				for(int j = 0; j < lotteryRecordList.Length; j++)
				{
					_len += lotteryRecordList[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  神器罐活动抽奖通知
	/// </summary>
	[Protocol]
	public class GASArtifactJarLotteryNotify : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 700904;
		public UInt32 Sequence;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
			}

			public void decode(byte[] buffer, ref int pos_)
			{
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
			}

			public int getLen()
			{
				int _len = 0;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  神器罐抽奖配置请求
	/// </summary>
	[Protocol]
	public class GASArtifactJarLotteryCfgReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 700905;
		public UInt32 Sequence;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
			}

			public void decode(byte[] buffer, ref int pos_)
			{
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
			}

			public int getLen()
			{
				int _len = 0;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  神器罐子奖励配置
	/// </summary>
	public class ArtifactJarLotteryCfg : Protocol.IProtocolStream
	{
		/// <summary>
		///  罐子id
		/// </summary>
		public UInt32 jarId;
		/// <summary>
		///  奖励列表
		/// </summary>
		public ItemReward[] rewardList = new ItemReward[0];

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, jarId);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)rewardList.Length);
				for(int i = 0; i < rewardList.Length; i++)
				{
					rewardList[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref jarId);
				UInt16 rewardListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref rewardListCnt);
				rewardList = new ItemReward[rewardListCnt];
				for(int i = 0; i < rewardList.Length; i++)
				{
					rewardList[i] = new ItemReward();
					rewardList[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, jarId);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)rewardList.Length);
				for(int i = 0; i < rewardList.Length; i++)
				{
					rewardList[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref jarId);
				UInt16 rewardListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref rewardListCnt);
				rewardList = new ItemReward[rewardListCnt];
				for(int i = 0; i < rewardList.Length; i++)
				{
					rewardList[i] = new ItemReward();
					rewardList[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// jarId
				_len += 4;
				// rewardList
				_len += 2;
				for(int j = 0; j < rewardList.Length; j++)
				{
					_len += rewardList[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  神器罐活动抽奖记录返回
	/// </summary>
	[Protocol]
	public class GASArtifactJarLotteryCfgRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 700906;
		public UInt32 Sequence;
		public ArtifactJarLotteryCfg[] artifactJarCfgList = new ArtifactJarLotteryCfg[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)artifactJarCfgList.Length);
				for(int i = 0; i < artifactJarCfgList.Length; i++)
				{
					artifactJarCfgList[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 artifactJarCfgListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref artifactJarCfgListCnt);
				artifactJarCfgList = new ArtifactJarLotteryCfg[artifactJarCfgListCnt];
				for(int i = 0; i < artifactJarCfgList.Length; i++)
				{
					artifactJarCfgList[i] = new ArtifactJarLotteryCfg();
					artifactJarCfgList[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)artifactJarCfgList.Length);
				for(int i = 0; i < artifactJarCfgList.Length; i++)
				{
					artifactJarCfgList[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 artifactJarCfgListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref artifactJarCfgListCnt);
				artifactJarCfgList = new ArtifactJarLotteryCfg[artifactJarCfgListCnt];
				for(int i = 0; i < artifactJarCfgList.Length; i++)
				{
					artifactJarCfgList[i] = new ArtifactJarLotteryCfg();
					artifactJarCfgList[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// artifactJarCfgList
				_len += 2;
				for(int j = 0; j < artifactJarCfgList.Length; j++)
				{
					_len += artifactJarCfgList[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	public class BlackMarketAuctionInfo : Protocol.IProtocolStream
	{
		/// <summary>
		///  唯一id
		/// </summary>
		public UInt64 guid;
		/// <summary>
		///  回购物品id
		/// </summary>
		public UInt32 back_buy_item_id;
		/// <summary>
		///  回购类型
		/// </summary>
		public byte back_buy_type;
		/// <summary>
		///  固定收购价格
		/// </summary>
		public UInt32 price;
		/// <summary>
		///  竞拍开始时间
		/// </summary>
		public UInt32 begin_time;
		/// <summary>
		///  竞拍结束时间
		/// </summary>
		public UInt32 end_time;
		/// <summary>
		///  推荐价格
		/// </summary>
		public UInt32 recommend_price;
		/// <summary>
		///  价格下限
		/// </summary>
		public UInt32 price_lower_limit;
		/// <summary>
		///  价格上限
		/// </summary>
		public UInt32 price_upper_limit;
		/// <summary>
		///  状态(BlackMarketAuctionState)
		/// </summary>
		public byte state;
		/// <summary>
		///  竞拍角色id
		/// </summary>
		public UInt64 auctioner_guid;
		/// <summary>
		///  竞拍者名字
		/// </summary>
		public string auctioner_name;
		/// <summary>
		///  报价
		/// </summary>
		public UInt32 auction_price;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, guid);
				BaseDLL.encode_uint32(buffer, ref pos_, back_buy_item_id);
				BaseDLL.encode_int8(buffer, ref pos_, back_buy_type);
				BaseDLL.encode_uint32(buffer, ref pos_, price);
				BaseDLL.encode_uint32(buffer, ref pos_, begin_time);
				BaseDLL.encode_uint32(buffer, ref pos_, end_time);
				BaseDLL.encode_uint32(buffer, ref pos_, recommend_price);
				BaseDLL.encode_uint32(buffer, ref pos_, price_lower_limit);
				BaseDLL.encode_uint32(buffer, ref pos_, price_upper_limit);
				BaseDLL.encode_int8(buffer, ref pos_, state);
				BaseDLL.encode_uint64(buffer, ref pos_, auctioner_guid);
				byte[] auctioner_nameBytes = StringHelper.StringToUTF8Bytes(auctioner_name);
				BaseDLL.encode_string(buffer, ref pos_, auctioner_nameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint32(buffer, ref pos_, auction_price);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref guid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref back_buy_item_id);
				BaseDLL.decode_int8(buffer, ref pos_, ref back_buy_type);
				BaseDLL.decode_uint32(buffer, ref pos_, ref price);
				BaseDLL.decode_uint32(buffer, ref pos_, ref begin_time);
				BaseDLL.decode_uint32(buffer, ref pos_, ref end_time);
				BaseDLL.decode_uint32(buffer, ref pos_, ref recommend_price);
				BaseDLL.decode_uint32(buffer, ref pos_, ref price_lower_limit);
				BaseDLL.decode_uint32(buffer, ref pos_, ref price_upper_limit);
				BaseDLL.decode_int8(buffer, ref pos_, ref state);
				BaseDLL.decode_uint64(buffer, ref pos_, ref auctioner_guid);
				UInt16 auctioner_nameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref auctioner_nameLen);
				byte[] auctioner_nameBytes = new byte[auctioner_nameLen];
				for(int i = 0; i < auctioner_nameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref auctioner_nameBytes[i]);
				}
				auctioner_name = StringHelper.BytesToString(auctioner_nameBytes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref auction_price);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, guid);
				BaseDLL.encode_uint32(buffer, ref pos_, back_buy_item_id);
				BaseDLL.encode_int8(buffer, ref pos_, back_buy_type);
				BaseDLL.encode_uint32(buffer, ref pos_, price);
				BaseDLL.encode_uint32(buffer, ref pos_, begin_time);
				BaseDLL.encode_uint32(buffer, ref pos_, end_time);
				BaseDLL.encode_uint32(buffer, ref pos_, recommend_price);
				BaseDLL.encode_uint32(buffer, ref pos_, price_lower_limit);
				BaseDLL.encode_uint32(buffer, ref pos_, price_upper_limit);
				BaseDLL.encode_int8(buffer, ref pos_, state);
				BaseDLL.encode_uint64(buffer, ref pos_, auctioner_guid);
				byte[] auctioner_nameBytes = StringHelper.StringToUTF8Bytes(auctioner_name);
				BaseDLL.encode_string(buffer, ref pos_, auctioner_nameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint32(buffer, ref pos_, auction_price);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref guid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref back_buy_item_id);
				BaseDLL.decode_int8(buffer, ref pos_, ref back_buy_type);
				BaseDLL.decode_uint32(buffer, ref pos_, ref price);
				BaseDLL.decode_uint32(buffer, ref pos_, ref begin_time);
				BaseDLL.decode_uint32(buffer, ref pos_, ref end_time);
				BaseDLL.decode_uint32(buffer, ref pos_, ref recommend_price);
				BaseDLL.decode_uint32(buffer, ref pos_, ref price_lower_limit);
				BaseDLL.decode_uint32(buffer, ref pos_, ref price_upper_limit);
				BaseDLL.decode_int8(buffer, ref pos_, ref state);
				BaseDLL.decode_uint64(buffer, ref pos_, ref auctioner_guid);
				UInt16 auctioner_nameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref auctioner_nameLen);
				byte[] auctioner_nameBytes = new byte[auctioner_nameLen];
				for(int i = 0; i < auctioner_nameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref auctioner_nameBytes[i]);
				}
				auctioner_name = StringHelper.BytesToString(auctioner_nameBytes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref auction_price);
			}

			public int getLen()
			{
				int _len = 0;
				// guid
				_len += 8;
				// back_buy_item_id
				_len += 4;
				// back_buy_type
				_len += 1;
				// price
				_len += 4;
				// begin_time
				_len += 4;
				// end_time
				_len += 4;
				// recommend_price
				_len += 4;
				// price_lower_limit
				_len += 4;
				// price_upper_limit
				_len += 4;
				// state
				_len += 1;
				// auctioner_guid
				_len += 8;
				// auctioner_name
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(auctioner_name);
					_len += 2 + _strBytes.Length;
				}
				// auction_price
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  同步黑市商人类型
	/// </summary>
	[Protocol]
	public class WorldBlackMarketSyncType : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 609001;
		public UInt32 Sequence;
		/// <summary>
		///  商人类型(BlackMarketType)
		/// </summary>
		public byte type;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, type);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, type);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
			}

			public int getLen()
			{
				int _len = 0;
				// type
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  请求竞拍列表
	/// </summary>
	[Protocol]
	public class WorldBlackMarketAuctionListReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 609002;
		public UInt32 Sequence;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
			}

			public void decode(byte[] buffer, ref int pos_)
			{
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
			}

			public int getLen()
			{
				int _len = 0;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  请求竞拍列表返回
	/// </summary>
	[Protocol]
	public class WorldBlackMarketAuctionListRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 609003;
		public UInt32 Sequence;
		public BlackMarketAuctionInfo[] items = new BlackMarketAuctionInfo[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)items.Length);
				for(int i = 0; i < items.Length; i++)
				{
					items[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 itemsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref itemsCnt);
				items = new BlackMarketAuctionInfo[itemsCnt];
				for(int i = 0; i < items.Length; i++)
				{
					items[i] = new BlackMarketAuctionInfo();
					items[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)items.Length);
				for(int i = 0; i < items.Length; i++)
				{
					items[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 itemsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref itemsCnt);
				items = new BlackMarketAuctionInfo[itemsCnt];
				for(int i = 0; i < items.Length; i++)
				{
					items[i] = new BlackMarketAuctionInfo();
					items[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// items
				_len += 2;
				for(int j = 0; j < items.Length; j++)
				{
					_len += items[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 请求竞拍
	/// </summary>
	[Protocol]
	public class WorldBlackMarketAuctionReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 609004;
		public UInt32 Sequence;
		/// <summary>
		/// 竞拍item项guid
		/// </summary>
		public UInt64 auction_guid;
		/// <summary>
		/// 装备id
		/// </summary>
		public UInt64 item_guid;
		/// <summary>
		/// 竞拍价格
		/// </summary>
		public UInt32 auction_price;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, auction_guid);
				BaseDLL.encode_uint64(buffer, ref pos_, item_guid);
				BaseDLL.encode_uint32(buffer, ref pos_, auction_price);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref auction_guid);
				BaseDLL.decode_uint64(buffer, ref pos_, ref item_guid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref auction_price);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, auction_guid);
				BaseDLL.encode_uint64(buffer, ref pos_, item_guid);
				BaseDLL.encode_uint32(buffer, ref pos_, auction_price);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref auction_guid);
				BaseDLL.decode_uint64(buffer, ref pos_, ref item_guid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref auction_price);
			}

			public int getLen()
			{
				int _len = 0;
				// auction_guid
				_len += 8;
				// item_guid
				_len += 8;
				// auction_price
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 竞拍返回
	/// </summary>
	[Protocol]
	public class WorldBlackMarketAuctionRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 609005;
		public UInt32 Sequence;
		/// <summary>
		/// 结果
		/// </summary>
		public UInt32 code;
		/// <summary>
		/// 竞拍的商品
		/// </summary>
		public BlackMarketAuctionInfo item = new BlackMarketAuctionInfo();

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
				item.encode(buffer, ref pos_);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
				item.decode(buffer, ref pos_);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
				item.encode(buffer, ref pos_);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
				item.decode(buffer, ref pos_);
			}

			public int getLen()
			{
				int _len = 0;
				// code
				_len += 4;
				// item
				_len += item.getLen();
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 取消竞拍请求
	/// </summary>
	[Protocol]
	public class WorldBlackMarketAuctionCancelReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 609006;
		public UInt32 Sequence;
		/// <summary>
		/// 竞拍item项guid
		/// </summary>
		public UInt64 auction_guid;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, auction_guid);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref auction_guid);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, auction_guid);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref auction_guid);
			}

			public int getLen()
			{
				int _len = 0;
				// auction_guid
				_len += 8;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 取消竞拍返回
	/// </summary>
	[Protocol]
	public class WorldBlackMarketAuctionCancelRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 609007;
		public UInt32 Sequence;
		/// <summary>
		/// 结果
		/// </summary>
		public UInt32 code;
		/// <summary>
		/// 取消竞拍的商品
		/// </summary>
		public BlackMarketAuctionInfo item = new BlackMarketAuctionInfo();

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
				item.encode(buffer, ref pos_);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
				item.decode(buffer, ref pos_);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
				item.encode(buffer, ref pos_);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
				item.decode(buffer, ref pos_);
			}

			public int getLen()
			{
				int _len = 0;
				// code
				_len += 4;
				// item
				_len += item.getLen();
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 通知客户端重新拉去列表
	/// </summary>
	[Protocol]
	public class WorldBlackMarketNotifyRefresh : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 609008;
		public UInt32 Sequence;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
			}

			public void decode(byte[] buffer, ref int pos_)
			{
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
			}

			public int getLen()
			{
				int _len = 0;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 装备升级请求
	/// </summary>
	[Protocol]
	public class SceneEquieUpdateReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501048;
		public UInt32 Sequence;
		public UInt64 equipUid;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, equipUid);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref equipUid);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, equipUid);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref equipUid);
			}

			public int getLen()
			{
				int _len = 0;
				// equipUid
				_len += 8;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 装备升级返回
	/// </summary>
	[Protocol]
	public class SceneEquieUpdateRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501049;
		public UInt32 Sequence;
		public UInt32 code;
		public UInt64 equipUid;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
				BaseDLL.encode_uint64(buffer, ref pos_, equipUid);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
				BaseDLL.decode_uint64(buffer, ref pos_, ref equipUid);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
				BaseDLL.encode_uint64(buffer, ref pos_, equipUid);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
				BaseDLL.decode_uint64(buffer, ref pos_, ref equipUid);
			}

			public int getLen()
			{
				int _len = 0;
				// code
				_len += 4;
				// equipUid
				_len += 8;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 充值推送购买
	/// </summary>
	public class RechargePushItem : Protocol.IProtocolStream
	{
		public UInt32 pushId;
		public UInt32 itemId;
		public UInt32 itemCount;
		public UInt32 buyTimes;
		public UInt32 maxTimes;
		public UInt32 price;
		public UInt32 discountPrice;
		/// <summary>
		/// 截至时间戳
		/// </summary>
		public UInt32 validTimestamp;
		public UInt32 lastTimestamp;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, pushId);
				BaseDLL.encode_uint32(buffer, ref pos_, itemId);
				BaseDLL.encode_uint32(buffer, ref pos_, itemCount);
				BaseDLL.encode_uint32(buffer, ref pos_, buyTimes);
				BaseDLL.encode_uint32(buffer, ref pos_, maxTimes);
				BaseDLL.encode_uint32(buffer, ref pos_, price);
				BaseDLL.encode_uint32(buffer, ref pos_, discountPrice);
				BaseDLL.encode_uint32(buffer, ref pos_, validTimestamp);
				BaseDLL.encode_uint32(buffer, ref pos_, lastTimestamp);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref pushId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemCount);
				BaseDLL.decode_uint32(buffer, ref pos_, ref buyTimes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref maxTimes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref price);
				BaseDLL.decode_uint32(buffer, ref pos_, ref discountPrice);
				BaseDLL.decode_uint32(buffer, ref pos_, ref validTimestamp);
				BaseDLL.decode_uint32(buffer, ref pos_, ref lastTimestamp);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, pushId);
				BaseDLL.encode_uint32(buffer, ref pos_, itemId);
				BaseDLL.encode_uint32(buffer, ref pos_, itemCount);
				BaseDLL.encode_uint32(buffer, ref pos_, buyTimes);
				BaseDLL.encode_uint32(buffer, ref pos_, maxTimes);
				BaseDLL.encode_uint32(buffer, ref pos_, price);
				BaseDLL.encode_uint32(buffer, ref pos_, discountPrice);
				BaseDLL.encode_uint32(buffer, ref pos_, validTimestamp);
				BaseDLL.encode_uint32(buffer, ref pos_, lastTimestamp);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref pushId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemCount);
				BaseDLL.decode_uint32(buffer, ref pos_, ref buyTimes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref maxTimes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref price);
				BaseDLL.decode_uint32(buffer, ref pos_, ref discountPrice);
				BaseDLL.decode_uint32(buffer, ref pos_, ref validTimestamp);
				BaseDLL.decode_uint32(buffer, ref pos_, ref lastTimestamp);
			}

			public int getLen()
			{
				int _len = 0;
				// pushId
				_len += 4;
				// itemId
				_len += 4;
				// itemCount
				_len += 4;
				// buyTimes
				_len += 4;
				// maxTimes
				_len += 4;
				// price
				_len += 4;
				// discountPrice
				_len += 4;
				// validTimestamp
				_len += 4;
				// lastTimestamp
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 获取充值推送道具列表请求
	/// </summary>
	[Protocol]
	public class WorldGetRechargePushItemsReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 602827;
		public UInt32 Sequence;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
			}

			public void decode(byte[] buffer, ref int pos_)
			{
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
			}

			public int getLen()
			{
				int _len = 0;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 获取充值推送道具列表返回
	/// </summary>
	[Protocol]
	public class WorldGetRechargePushItemsRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 602828;
		public UInt32 Sequence;
		public UInt32 retCode;
		public RechargePushItem[] itemVec = new RechargePushItem[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, retCode);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)itemVec.Length);
				for(int i = 0; i < itemVec.Length; i++)
				{
					itemVec[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
				UInt16 itemVecCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref itemVecCnt);
				itemVec = new RechargePushItem[itemVecCnt];
				for(int i = 0; i < itemVec.Length; i++)
				{
					itemVec[i] = new RechargePushItem();
					itemVec[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, retCode);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)itemVec.Length);
				for(int i = 0; i < itemVec.Length; i++)
				{
					itemVec[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
				UInt16 itemVecCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref itemVecCnt);
				itemVec = new RechargePushItem[itemVecCnt];
				for(int i = 0; i < itemVec.Length; i++)
				{
					itemVec[i] = new RechargePushItem();
					itemVec[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// retCode
				_len += 4;
				// itemVec
				_len += 2;
				for(int j = 0; j < itemVec.Length; j++)
				{
					_len += itemVec[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 购买充值推送道具请求
	/// </summary>
	[Protocol]
	public class WorldBuyRechargePushItemsReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 602829;
		public UInt32 Sequence;
		public UInt32 pushId;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, pushId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref pushId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, pushId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref pushId);
			}

			public int getLen()
			{
				int _len = 0;
				// pushId
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 购买充值推送道具返回
	/// </summary>
	[Protocol]
	public class WorldBuyRechargePushItemsRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 602830;
		public UInt32 Sequence;
		public UInt32 retCode;
		public UInt32 pushId;
		public RechargePushItem[] itemVec = new RechargePushItem[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, retCode);
				BaseDLL.encode_uint32(buffer, ref pos_, pushId);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)itemVec.Length);
				for(int i = 0; i < itemVec.Length; i++)
				{
					itemVec[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
				BaseDLL.decode_uint32(buffer, ref pos_, ref pushId);
				UInt16 itemVecCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref itemVecCnt);
				itemVec = new RechargePushItem[itemVecCnt];
				for(int i = 0; i < itemVec.Length; i++)
				{
					itemVec[i] = new RechargePushItem();
					itemVec[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, retCode);
				BaseDLL.encode_uint32(buffer, ref pos_, pushId);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)itemVec.Length);
				for(int i = 0; i < itemVec.Length; i++)
				{
					itemVec[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
				BaseDLL.decode_uint32(buffer, ref pos_, ref pushId);
				UInt16 itemVecCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref itemVecCnt);
				itemVec = new RechargePushItem[itemVecCnt];
				for(int i = 0; i < itemVec.Length; i++)
				{
					itemVec[i] = new RechargePushItem();
					itemVec[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// retCode
				_len += 4;
				// pushId
				_len += 4;
				// itemVec
				_len += 2;
				for(int j = 0; j < itemVec.Length; j++)
				{
					_len += itemVec[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  物品寄存请求
	/// </summary>
	[Protocol]
	public class SceneItemDepositReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501050;
		public UInt32 Sequence;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
			}

			public void decode(byte[] buffer, ref int pos_)
			{
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
			}

			public int getLen()
			{
				int _len = 0;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  寄存物品
	/// </summary>
	public class depositItem : Protocol.IProtocolStream
	{
		/// <summary>
		///  guid
		/// </summary>
		public UInt64 guid;
		/// <summary>
		///  创建时间
		/// </summary>
		public UInt32 createTime;
		/// <summary>
		///  奖励物品
		/// </summary>
		public ItemReward itemReward = new ItemReward();

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, guid);
				BaseDLL.encode_uint32(buffer, ref pos_, createTime);
				itemReward.encode(buffer, ref pos_);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref guid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref createTime);
				itemReward.decode(buffer, ref pos_);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, guid);
				BaseDLL.encode_uint32(buffer, ref pos_, createTime);
				itemReward.encode(buffer, ref pos_);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref guid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref createTime);
				itemReward.decode(buffer, ref pos_);
			}

			public int getLen()
			{
				int _len = 0;
				// guid
				_len += 8;
				// createTime
				_len += 4;
				// itemReward
				_len += itemReward.getLen();
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  物品寄存返回
	/// </summary>
	[Protocol]
	public class SceneItemDepositRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501051;
		public UInt32 Sequence;
		/// <summary>
		///  过期时间
		/// </summary>
		public UInt32 expireTime;
		/// <summary>
		///  物品列表
		/// </summary>
		public depositItem[] depositItemList = new depositItem[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, expireTime);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)depositItemList.Length);
				for(int i = 0; i < depositItemList.Length; i++)
				{
					depositItemList[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref expireTime);
				UInt16 depositItemListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref depositItemListCnt);
				depositItemList = new depositItem[depositItemListCnt];
				for(int i = 0; i < depositItemList.Length; i++)
				{
					depositItemList[i] = new depositItem();
					depositItemList[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, expireTime);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)depositItemList.Length);
				for(int i = 0; i < depositItemList.Length; i++)
				{
					depositItemList[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref expireTime);
				UInt16 depositItemListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref depositItemListCnt);
				depositItemList = new depositItem[depositItemListCnt];
				for(int i = 0; i < depositItemList.Length; i++)
				{
					depositItemList[i] = new depositItem();
					depositItemList[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// expireTime
				_len += 4;
				// depositItemList
				_len += 2;
				for(int j = 0; j < depositItemList.Length; j++)
				{
					_len += depositItemList[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  物品寄存领取请求
	/// </summary>
	[Protocol]
	public class SceneItemDepositGetReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501052;
		public UInt32 Sequence;
		public UInt64 guid;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, guid);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref guid);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, guid);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref guid);
			}

			public int getLen()
			{
				int _len = 0;
				// guid
				_len += 8;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  物品寄存领取返回
	/// </summary>
	[Protocol]
	public class SceneItemDepositGetRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501053;
		public UInt32 Sequence;
		public UInt32 retCode;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, retCode);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, retCode);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
			}

			public int getLen()
			{
				int _len = 0;
				// retCode
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  物品寄存领取到期
	/// </summary>
	[Protocol]
	public class SceneItemDepositExpire : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501059;
		public UInt32 Sequence;
		/// <summary>
		///  剩余过期时间
		/// </summary>
		public UInt32 oddExpireTime;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, oddExpireTime);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref oddExpireTime);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, oddExpireTime);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref oddExpireTime);
			}

			public int getLen()
			{
				int _len = 0;
				// oddExpireTime
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// client->scene附魔卡升级请求
	/// </summary>
	[Protocol]
	public class SceneMagicCardUpgradeReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501055;
		public UInt32 Sequence;
		public UInt64 upgradeUid;
		public UInt64[] materialItemVec = new UInt64[0];
		public UInt64 equipUid;
		public UInt32 cardId;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, upgradeUid);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)materialItemVec.Length);
				for(int i = 0; i < materialItemVec.Length; i++)
				{
					BaseDLL.encode_uint64(buffer, ref pos_, materialItemVec[i]);
				}
				BaseDLL.encode_uint64(buffer, ref pos_, equipUid);
				BaseDLL.encode_uint32(buffer, ref pos_, cardId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref upgradeUid);
				UInt16 materialItemVecCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref materialItemVecCnt);
				materialItemVec = new UInt64[materialItemVecCnt];
				for(int i = 0; i < materialItemVec.Length; i++)
				{
					BaseDLL.decode_uint64(buffer, ref pos_, ref materialItemVec[i]);
				}
				BaseDLL.decode_uint64(buffer, ref pos_, ref equipUid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref cardId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, upgradeUid);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)materialItemVec.Length);
				for(int i = 0; i < materialItemVec.Length; i++)
				{
					BaseDLL.encode_uint64(buffer, ref pos_, materialItemVec[i]);
				}
				BaseDLL.encode_uint64(buffer, ref pos_, equipUid);
				BaseDLL.encode_uint32(buffer, ref pos_, cardId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref upgradeUid);
				UInt16 materialItemVecCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref materialItemVecCnt);
				materialItemVec = new UInt64[materialItemVecCnt];
				for(int i = 0; i < materialItemVec.Length; i++)
				{
					BaseDLL.decode_uint64(buffer, ref pos_, ref materialItemVec[i]);
				}
				BaseDLL.decode_uint64(buffer, ref pos_, ref equipUid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref cardId);
			}

			public int getLen()
			{
				int _len = 0;
				// upgradeUid
				_len += 8;
				// materialItemVec
				_len += 2 + 8 * materialItemVec.Length;
				// equipUid
				_len += 8;
				// cardId
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// scene->client附魔卡升级返回
	/// </summary>
	[Protocol]
	public class SceneMagicCardUpgradeRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501056;
		public UInt32 Sequence;
		public UInt32 code;
		public UInt32 cardTypeId;
		public byte cardLev;
		public UInt64 cardGuid;
		public UInt64 equipUid;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
				BaseDLL.encode_uint32(buffer, ref pos_, cardTypeId);
				BaseDLL.encode_int8(buffer, ref pos_, cardLev);
				BaseDLL.encode_uint64(buffer, ref pos_, cardGuid);
				BaseDLL.encode_uint64(buffer, ref pos_, equipUid);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
				BaseDLL.decode_uint32(buffer, ref pos_, ref cardTypeId);
				BaseDLL.decode_int8(buffer, ref pos_, ref cardLev);
				BaseDLL.decode_uint64(buffer, ref pos_, ref cardGuid);
				BaseDLL.decode_uint64(buffer, ref pos_, ref equipUid);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
				BaseDLL.encode_uint32(buffer, ref pos_, cardTypeId);
				BaseDLL.encode_int8(buffer, ref pos_, cardLev);
				BaseDLL.encode_uint64(buffer, ref pos_, cardGuid);
				BaseDLL.encode_uint64(buffer, ref pos_, equipUid);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
				BaseDLL.decode_uint32(buffer, ref pos_, ref cardTypeId);
				BaseDLL.decode_int8(buffer, ref pos_, ref cardLev);
				BaseDLL.decode_uint64(buffer, ref pos_, ref cardGuid);
				BaseDLL.decode_uint64(buffer, ref pos_, ref equipUid);
			}

			public int getLen()
			{
				int _len = 0;
				// code
				_len += 4;
				// cardTypeId
				_len += 4;
				// cardLev
				_len += 1;
				// cardGuid
				_len += 8;
				// equipUid
				_len += 8;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// client->scene附魔卡一键合成请求
	/// </summary>
	[Protocol]
	public class SceneMagicCardCompOneKeyReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501057;
		public UInt32 Sequence;
		public byte isCompWhite;
		public byte isCompBlue;
		public byte autoUseGold;
		public byte compNotBind;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, isCompWhite);
				BaseDLL.encode_int8(buffer, ref pos_, isCompBlue);
				BaseDLL.encode_int8(buffer, ref pos_, autoUseGold);
				BaseDLL.encode_int8(buffer, ref pos_, compNotBind);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref isCompWhite);
				BaseDLL.decode_int8(buffer, ref pos_, ref isCompBlue);
				BaseDLL.decode_int8(buffer, ref pos_, ref autoUseGold);
				BaseDLL.decode_int8(buffer, ref pos_, ref compNotBind);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, isCompWhite);
				BaseDLL.encode_int8(buffer, ref pos_, isCompBlue);
				BaseDLL.encode_int8(buffer, ref pos_, autoUseGold);
				BaseDLL.encode_int8(buffer, ref pos_, compNotBind);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref isCompWhite);
				BaseDLL.decode_int8(buffer, ref pos_, ref isCompBlue);
				BaseDLL.decode_int8(buffer, ref pos_, ref autoUseGold);
				BaseDLL.decode_int8(buffer, ref pos_, ref compNotBind);
			}

			public int getLen()
			{
				int _len = 0;
				// isCompWhite
				_len += 1;
				// isCompBlue
				_len += 1;
				// autoUseGold
				_len += 1;
				// compNotBind
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// server->client 附魔卡一键合成返回
	/// </summary>
	[Protocol]
	public class SceneMagicCardCompOneKeyRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501058;
		public UInt32 Sequence;
		public UInt32 code;
		public byte endReason;
		public UInt32 compTimes;
		public UInt32 consumeBindGolds;
		public UInt32 comsumeGolds;
		public ItemReward[] items = new ItemReward[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
				BaseDLL.encode_int8(buffer, ref pos_, endReason);
				BaseDLL.encode_uint32(buffer, ref pos_, compTimes);
				BaseDLL.encode_uint32(buffer, ref pos_, consumeBindGolds);
				BaseDLL.encode_uint32(buffer, ref pos_, comsumeGolds);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)items.Length);
				for(int i = 0; i < items.Length; i++)
				{
					items[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
				BaseDLL.decode_int8(buffer, ref pos_, ref endReason);
				BaseDLL.decode_uint32(buffer, ref pos_, ref compTimes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref consumeBindGolds);
				BaseDLL.decode_uint32(buffer, ref pos_, ref comsumeGolds);
				UInt16 itemsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref itemsCnt);
				items = new ItemReward[itemsCnt];
				for(int i = 0; i < items.Length; i++)
				{
					items[i] = new ItemReward();
					items[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
				BaseDLL.encode_int8(buffer, ref pos_, endReason);
				BaseDLL.encode_uint32(buffer, ref pos_, compTimes);
				BaseDLL.encode_uint32(buffer, ref pos_, consumeBindGolds);
				BaseDLL.encode_uint32(buffer, ref pos_, comsumeGolds);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)items.Length);
				for(int i = 0; i < items.Length; i++)
				{
					items[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
				BaseDLL.decode_int8(buffer, ref pos_, ref endReason);
				BaseDLL.decode_uint32(buffer, ref pos_, ref compTimes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref consumeBindGolds);
				BaseDLL.decode_uint32(buffer, ref pos_, ref comsumeGolds);
				UInt16 itemsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref itemsCnt);
				items = new ItemReward[itemsCnt];
				for(int i = 0; i < items.Length; i++)
				{
					items[i] = new ItemReward();
					items[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// code
				_len += 4;
				// endReason
				_len += 1;
				// compTimes
				_len += 4;
				// consumeBindGolds
				_len += 4;
				// comsumeGolds
				_len += 4;
				// items
				_len += 2;
				for(int j = 0; j < items.Length; j++)
				{
					_len += items[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// client->server 装备增幅
	/// </summary>
	[Protocol]
	public class SceneEquipEnhanceUpgrade : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501060;
		public UInt32 Sequence;
		/// <summary>
		///  装备uid
		/// </summary>
		public UInt64 euqipUid;
		/// <summary>
		///  是否使用保护劵
		/// </summary>
		public byte useUnbreak;
		/// <summary>
		///  使用的增幅券
		/// </summary>
		public UInt64 strTickt;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, euqipUid);
				BaseDLL.encode_int8(buffer, ref pos_, useUnbreak);
				BaseDLL.encode_uint64(buffer, ref pos_, strTickt);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref euqipUid);
				BaseDLL.decode_int8(buffer, ref pos_, ref useUnbreak);
				BaseDLL.decode_uint64(buffer, ref pos_, ref strTickt);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, euqipUid);
				BaseDLL.encode_int8(buffer, ref pos_, useUnbreak);
				BaseDLL.encode_uint64(buffer, ref pos_, strTickt);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref euqipUid);
				BaseDLL.decode_int8(buffer, ref pos_, ref useUnbreak);
				BaseDLL.decode_uint64(buffer, ref pos_, ref strTickt);
			}

			public int getLen()
			{
				int _len = 0;
				// euqipUid
				_len += 8;
				// useUnbreak
				_len += 1;
				// strTickt
				_len += 8;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneEquipEnhanceUpgradeRet : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501061;
		public UInt32 Sequence;
		/// <summary>
		///  返回值
		/// </summary>
		public UInt32 code;
		/// <summary>
		///  返还材料
		/// </summary>
		public ItemReward[] matNums = new ItemReward[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)matNums.Length);
				for(int i = 0; i < matNums.Length; i++)
				{
					matNums[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
				UInt16 matNumsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref matNumsCnt);
				matNums = new ItemReward[matNumsCnt];
				for(int i = 0; i < matNums.Length; i++)
				{
					matNums[i] = new ItemReward();
					matNums[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)matNums.Length);
				for(int i = 0; i < matNums.Length; i++)
				{
					matNums[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
				UInt16 matNumsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref matNumsCnt);
				matNums = new ItemReward[matNumsCnt];
				for(int i = 0; i < matNums.Length; i++)
				{
					matNums[i] = new ItemReward();
					matNums[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// code
				_len += 4;
				// matNums
				_len += 2;
				for(int j = 0; j < matNums.Length; j++)
				{
					_len += matNums[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 气息装备清除气息
	/// </summary>
	[Protocol]
	public class SceneEquipEnhanceClear : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501062;
		public UInt32 Sequence;
		/// <summary>
		///  装备uid
		/// </summary>
		public UInt64 euqipUid;
		/// <summary>
		/// 使用的材料道具id
		/// </summary>
		public UInt32 stuffID;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, euqipUid);
				BaseDLL.encode_uint32(buffer, ref pos_, stuffID);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref euqipUid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref stuffID);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, euqipUid);
				BaseDLL.encode_uint32(buffer, ref pos_, stuffID);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref euqipUid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref stuffID);
			}

			public int getLen()
			{
				int _len = 0;
				// euqipUid
				_len += 8;
				// stuffID
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneEquipEnhanceClearRet : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501063;
		public UInt32 Sequence;
		/// <summary>
		///  返回值
		/// </summary>
		public UInt32 code;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			}

			public int getLen()
			{
				int _len = 0;
				// code
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// scene->client气息装备激活变成红字装备
	/// </summary>
	[Protocol]
	public class SceneEquipEnhanceRed : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501064;
		public UInt32 Sequence;
		/// <summary>
		///  装备uid
		/// </summary>
		public UInt64 euqipUid;
		/// <summary>
		/// 使用的材料道具id
		/// </summary>
		public UInt32 stuffID;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, euqipUid);
				BaseDLL.encode_uint32(buffer, ref pos_, stuffID);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref euqipUid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref stuffID);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, euqipUid);
				BaseDLL.encode_uint32(buffer, ref pos_, stuffID);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref euqipUid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref stuffID);
			}

			public int getLen()
			{
				int _len = 0;
				// euqipUid
				_len += 8;
				// stuffID
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneEquipEnhanceRedRet : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501065;
		public UInt32 Sequence;
		/// <summary>
		///  返回值
		/// </summary>
		public UInt32 code;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			}

			public int getLen()
			{
				int _len = 0;
				// code
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// scene->client红字装备属性转化
	/// </summary>
	[Protocol]
	public class SceneEquipEnhanceChg : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501066;
		public UInt32 Sequence;
		/// <summary>
		///  装备uid
		/// </summary>
		public UInt64 euqipUid;
		/// <summary>
		///  转化路线
		/// </summary>
		public byte enhanceType;
		/// <summary>
		/// 使用的材料道具id
		/// </summary>
		public UInt32 stuffID;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, euqipUid);
				BaseDLL.encode_int8(buffer, ref pos_, enhanceType);
				BaseDLL.encode_uint32(buffer, ref pos_, stuffID);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref euqipUid);
				BaseDLL.decode_int8(buffer, ref pos_, ref enhanceType);
				BaseDLL.decode_uint32(buffer, ref pos_, ref stuffID);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, euqipUid);
				BaseDLL.encode_int8(buffer, ref pos_, enhanceType);
				BaseDLL.encode_uint32(buffer, ref pos_, stuffID);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref euqipUid);
				BaseDLL.decode_int8(buffer, ref pos_, ref enhanceType);
				BaseDLL.decode_uint32(buffer, ref pos_, ref stuffID);
			}

			public int getLen()
			{
				int _len = 0;
				// euqipUid
				_len += 8;
				// enhanceType
				_len += 1;
				// stuffID
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneEquipEnhanceChgRet : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501067;
		public UInt32 Sequence;
		/// <summary>
		///  返回值
		/// </summary>
		public UInt32 code;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			}

			public int getLen()
			{
				int _len = 0;
				// code
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// scene->client装备增幅材料合成
	/// </summary>
	[Protocol]
	public class SceneEnhanceMaterialCombo : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501068;
		public UInt32 Sequence;
		/// <summary>
		///  目标ID
		/// </summary>
		public UInt32 goalId;
		/// <summary>
		///  目标数量
		/// </summary>
		public UInt32 goalNum;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, goalId);
				BaseDLL.encode_uint32(buffer, ref pos_, goalNum);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref goalId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref goalNum);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, goalId);
				BaseDLL.encode_uint32(buffer, ref pos_, goalNum);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref goalId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref goalNum);
			}

			public int getLen()
			{
				int _len = 0;
				// goalId
				_len += 4;
				// goalNum
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneEnhanceMaterialComboRet : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501069;
		public UInt32 Sequence;
		/// <summary>
		///  返回值
		/// </summary>
		public UInt32 code;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			}

			public int getLen()
			{
				int _len = 0;
				// code
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 同步飞升状态
	/// </summary>
	[Protocol]
	public class SceneSyncFlyUpStatus : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501070;
		public UInt32 Sequence;
		/// <summary>
		/// FlyUpStatus
		/// </summary>
		public byte status;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, status);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref status);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, status);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref status);
			}

			public int getLen()
			{
				int _len = 0;
				// status
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 铭文开孔
	/// </summary>
	[Protocol]
	public class SceneEquipInscriptionOpenReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501075;
		public UInt32 Sequence;
		/// <summary>
		///  装备uid
		/// </summary>
		public UInt64 guid;
		/// <summary>
		///  孔索引
		/// </summary>
		public UInt32 index;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, guid);
				BaseDLL.encode_uint32(buffer, ref pos_, index);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref guid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref index);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, guid);
				BaseDLL.encode_uint32(buffer, ref pos_, index);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref guid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref index);
			}

			public int getLen()
			{
				int _len = 0;
				// guid
				_len += 8;
				// index
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneEquipInscriptionOpenRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501076;
		public UInt32 Sequence;
		/// <summary>
		///  装备uid
		/// </summary>
		public UInt64 guid;
		/// <summary>
		///  孔索引
		/// </summary>
		public UInt32 index;
		/// <summary>
		///  返回值
		/// </summary>
		public UInt32 code;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, guid);
				BaseDLL.encode_uint32(buffer, ref pos_, index);
				BaseDLL.encode_uint32(buffer, ref pos_, code);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref guid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref index);
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, guid);
				BaseDLL.encode_uint32(buffer, ref pos_, index);
				BaseDLL.encode_uint32(buffer, ref pos_, code);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref guid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref index);
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			}

			public int getLen()
			{
				int _len = 0;
				// guid
				_len += 8;
				// index
				_len += 4;
				// code
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 铭文镶嵌
	/// </summary>
	[Protocol]
	public class SceneEquipInscriptionMountReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501077;
		public UInt32 Sequence;
		/// <summary>
		///  装备uid
		/// </summary>
		public UInt64 guid;
		/// <summary>
		///  孔索引
		/// </summary>
		public UInt32 index;
		/// <summary>
		///  铭文uid
		/// </summary>
		public UInt64 inscriptionGuid;
		/// <summary>
		///  铭文id
		/// </summary>
		public UInt32 inscriptionId;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, guid);
				BaseDLL.encode_uint32(buffer, ref pos_, index);
				BaseDLL.encode_uint64(buffer, ref pos_, inscriptionGuid);
				BaseDLL.encode_uint32(buffer, ref pos_, inscriptionId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref guid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref index);
				BaseDLL.decode_uint64(buffer, ref pos_, ref inscriptionGuid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref inscriptionId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, guid);
				BaseDLL.encode_uint32(buffer, ref pos_, index);
				BaseDLL.encode_uint64(buffer, ref pos_, inscriptionGuid);
				BaseDLL.encode_uint32(buffer, ref pos_, inscriptionId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref guid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref index);
				BaseDLL.decode_uint64(buffer, ref pos_, ref inscriptionGuid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref inscriptionId);
			}

			public int getLen()
			{
				int _len = 0;
				// guid
				_len += 8;
				// index
				_len += 4;
				// inscriptionGuid
				_len += 8;
				// inscriptionId
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneEquipInscriptionMountRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501078;
		public UInt32 Sequence;
		/// <summary>
		///  装备uid
		/// </summary>
		public UInt64 guid;
		/// <summary>
		///  孔索引
		/// </summary>
		public UInt32 index;
		/// <summary>
		///  铭文uid
		/// </summary>
		public UInt64 inscriptionGuid;
		/// <summary>
		///  铭文id
		/// </summary>
		public UInt32 inscriptionId;
		/// <summary>
		///  返回值
		/// </summary>
		public UInt32 code;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, guid);
				BaseDLL.encode_uint32(buffer, ref pos_, index);
				BaseDLL.encode_uint64(buffer, ref pos_, inscriptionGuid);
				BaseDLL.encode_uint32(buffer, ref pos_, inscriptionId);
				BaseDLL.encode_uint32(buffer, ref pos_, code);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref guid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref index);
				BaseDLL.decode_uint64(buffer, ref pos_, ref inscriptionGuid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref inscriptionId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, guid);
				BaseDLL.encode_uint32(buffer, ref pos_, index);
				BaseDLL.encode_uint64(buffer, ref pos_, inscriptionGuid);
				BaseDLL.encode_uint32(buffer, ref pos_, inscriptionId);
				BaseDLL.encode_uint32(buffer, ref pos_, code);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref guid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref index);
				BaseDLL.decode_uint64(buffer, ref pos_, ref inscriptionGuid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref inscriptionId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			}

			public int getLen()
			{
				int _len = 0;
				// guid
				_len += 8;
				// index
				_len += 4;
				// inscriptionGuid
				_len += 8;
				// inscriptionId
				_len += 4;
				// code
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 铭文摘取
	/// </summary>
	[Protocol]
	public class SceneEquipInscriptionExtractReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501079;
		public UInt32 Sequence;
		/// <summary>
		///  装备uid
		/// </summary>
		public UInt64 guid;
		/// <summary>
		///  孔索引
		/// </summary>
		public UInt32 index;
		/// <summary>
		///  铭文id
		/// </summary>
		public UInt32 inscriptionId;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, guid);
				BaseDLL.encode_uint32(buffer, ref pos_, index);
				BaseDLL.encode_uint32(buffer, ref pos_, inscriptionId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref guid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref index);
				BaseDLL.decode_uint32(buffer, ref pos_, ref inscriptionId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, guid);
				BaseDLL.encode_uint32(buffer, ref pos_, index);
				BaseDLL.encode_uint32(buffer, ref pos_, inscriptionId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref guid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref index);
				BaseDLL.decode_uint32(buffer, ref pos_, ref inscriptionId);
			}

			public int getLen()
			{
				int _len = 0;
				// guid
				_len += 8;
				// index
				_len += 4;
				// inscriptionId
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneEquipInscriptionExtractRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501080;
		public UInt32 Sequence;
		/// <summary>
		///  装备uid
		/// </summary>
		public UInt64 guid;
		/// <summary>
		///  孔索引
		/// </summary>
		public UInt32 index;
		/// <summary>
		///  铭文id
		/// </summary>
		public UInt32 inscriptionId;
		/// <summary>
		///  返回值
		/// </summary>
		public UInt32 code;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, guid);
				BaseDLL.encode_uint32(buffer, ref pos_, index);
				BaseDLL.encode_uint32(buffer, ref pos_, inscriptionId);
				BaseDLL.encode_uint32(buffer, ref pos_, code);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref guid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref index);
				BaseDLL.decode_uint32(buffer, ref pos_, ref inscriptionId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, guid);
				BaseDLL.encode_uint32(buffer, ref pos_, index);
				BaseDLL.encode_uint32(buffer, ref pos_, inscriptionId);
				BaseDLL.encode_uint32(buffer, ref pos_, code);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref guid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref index);
				BaseDLL.decode_uint32(buffer, ref pos_, ref inscriptionId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			}

			public int getLen()
			{
				int _len = 0;
				// guid
				_len += 8;
				// index
				_len += 4;
				// inscriptionId
				_len += 4;
				// code
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 铭文合成
	/// </summary>
	[Protocol]
	public class SceneEquipInscriptionSynthesisReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501081;
		public UInt32 Sequence;
		/// <summary>
		///  材料id
		/// </summary>
		public UInt32[] itemIDVec = new UInt32[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)itemIDVec.Length);
				for(int i = 0; i < itemIDVec.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, itemIDVec[i]);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 itemIDVecCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref itemIDVecCnt);
				itemIDVec = new UInt32[itemIDVecCnt];
				for(int i = 0; i < itemIDVec.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref itemIDVec[i]);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)itemIDVec.Length);
				for(int i = 0; i < itemIDVec.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, itemIDVec[i]);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 itemIDVecCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref itemIDVecCnt);
				itemIDVec = new UInt32[itemIDVecCnt];
				for(int i = 0; i < itemIDVec.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref itemIDVec[i]);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// itemIDVec
				_len += 2 + 4 * itemIDVec.Length;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneEquipInscriptionSynthesisRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501082;
		public UInt32 Sequence;
		/// <summary>
		/// 合成出的材料
		/// </summary>
		public ItemReward[] items = new ItemReward[0];
		/// <summary>
		///  返回值
		/// </summary>
		public UInt32 retCode;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)items.Length);
				for(int i = 0; i < items.Length; i++)
				{
					items[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_uint32(buffer, ref pos_, retCode);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 itemsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref itemsCnt);
				items = new ItemReward[itemsCnt];
				for(int i = 0; i < items.Length; i++)
				{
					items[i] = new ItemReward();
					items[i].decode(buffer, ref pos_);
				}
				BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)items.Length);
				for(int i = 0; i < items.Length; i++)
				{
					items[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_uint32(buffer, ref pos_, retCode);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 itemsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref itemsCnt);
				items = new ItemReward[itemsCnt];
				for(int i = 0; i < items.Length; i++)
				{
					items[i] = new ItemReward();
					items[i].decode(buffer, ref pos_);
				}
				BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
			}

			public int getLen()
			{
				int _len = 0;
				// items
				_len += 2;
				for(int j = 0; j < items.Length; j++)
				{
					_len += items[j].getLen();
				}
				// retCode
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 铭文碎裂
	/// </summary>
	[Protocol]
	public class SceneEquipInscriptionDestroyReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501083;
		public UInt32 Sequence;
		/// <summary>
		///  装备uid
		/// </summary>
		public UInt64 guid;
		/// <summary>
		///  孔索引
		/// </summary>
		public UInt32 index;
		/// <summary>
		///  铭文id
		/// </summary>
		public UInt32 inscriptionId;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, guid);
				BaseDLL.encode_uint32(buffer, ref pos_, index);
				BaseDLL.encode_uint32(buffer, ref pos_, inscriptionId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref guid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref index);
				BaseDLL.decode_uint32(buffer, ref pos_, ref inscriptionId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, guid);
				BaseDLL.encode_uint32(buffer, ref pos_, index);
				BaseDLL.encode_uint32(buffer, ref pos_, inscriptionId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref guid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref index);
				BaseDLL.decode_uint32(buffer, ref pos_, ref inscriptionId);
			}

			public int getLen()
			{
				int _len = 0;
				// guid
				_len += 8;
				// index
				_len += 4;
				// inscriptionId
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneEquipInscriptionDestroyRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501084;
		public UInt32 Sequence;
		/// <summary>
		///  装备uid
		/// </summary>
		public UInt64 guid;
		/// <summary>
		///  孔索引
		/// </summary>
		public UInt32 index;
		/// <summary>
		///  铭文id
		/// </summary>
		public UInt32 inscriptionId;
		/// <summary>
		///  返回值
		/// </summary>
		public UInt32 code;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, guid);
				BaseDLL.encode_uint32(buffer, ref pos_, index);
				BaseDLL.encode_uint32(buffer, ref pos_, inscriptionId);
				BaseDLL.encode_uint32(buffer, ref pos_, code);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref guid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref index);
				BaseDLL.decode_uint32(buffer, ref pos_, ref inscriptionId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, guid);
				BaseDLL.encode_uint32(buffer, ref pos_, index);
				BaseDLL.encode_uint32(buffer, ref pos_, inscriptionId);
				BaseDLL.encode_uint32(buffer, ref pos_, code);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref guid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref index);
				BaseDLL.decode_uint32(buffer, ref pos_, ref inscriptionId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			}

			public int getLen()
			{
				int _len = 0;
				// guid
				_len += 8;
				// index
				_len += 4;
				// inscriptionId
				_len += 4;
				// code
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 赠送项信息
	/// </summary>
	public class FriendPresentInfo : Protocol.IProtocolStream
	{
		/// <summary>
		///  唯一id
		/// </summary>
		public UInt64 guid;
		/// <summary>
		///  好友id
		/// </summary>
		public UInt64 friendId;
		/// <summary>
		///  好友职业
		/// </summary>
		public byte friendOcc;
		/// <summary>
		///  好友等级
		/// </summary>
		public UInt16 friendLev;
		/// <summary>
		///  好友名字
		/// </summary>
		public string friendname;
		/// <summary>
		///  在线状态
		/// </summary>
		public byte isOnline;
		/// <summary>
		///  被赠送数量
		/// </summary>
		public UInt32 beSendedTimes;
		/// <summary>
		///  被赠送上限
		/// </summary>
		public UInt32 beSendedLimit;
		/// <summary>
		///  赠送数量
		/// </summary>
		public UInt32 sendTimes;
		/// <summary>
		///  赠送上限
		/// </summary>
		public UInt32 sendLimit;
		/// <summary>
		///  被赠送总次数
		/// </summary>
		public UInt32 sendedTotalTimes;
		/// <summary>
		///  被赠送总次数上限
		/// </summary>
		public UInt32 sendedTotalLimit;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, guid);
				BaseDLL.encode_uint64(buffer, ref pos_, friendId);
				BaseDLL.encode_int8(buffer, ref pos_, friendOcc);
				BaseDLL.encode_uint16(buffer, ref pos_, friendLev);
				byte[] friendnameBytes = StringHelper.StringToUTF8Bytes(friendname);
				BaseDLL.encode_string(buffer, ref pos_, friendnameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, isOnline);
				BaseDLL.encode_uint32(buffer, ref pos_, beSendedTimes);
				BaseDLL.encode_uint32(buffer, ref pos_, beSendedLimit);
				BaseDLL.encode_uint32(buffer, ref pos_, sendTimes);
				BaseDLL.encode_uint32(buffer, ref pos_, sendLimit);
				BaseDLL.encode_uint32(buffer, ref pos_, sendedTotalTimes);
				BaseDLL.encode_uint32(buffer, ref pos_, sendedTotalLimit);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref guid);
				BaseDLL.decode_uint64(buffer, ref pos_, ref friendId);
				BaseDLL.decode_int8(buffer, ref pos_, ref friendOcc);
				BaseDLL.decode_uint16(buffer, ref pos_, ref friendLev);
				UInt16 friendnameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref friendnameLen);
				byte[] friendnameBytes = new byte[friendnameLen];
				for(int i = 0; i < friendnameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref friendnameBytes[i]);
				}
				friendname = StringHelper.BytesToString(friendnameBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref isOnline);
				BaseDLL.decode_uint32(buffer, ref pos_, ref beSendedTimes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref beSendedLimit);
				BaseDLL.decode_uint32(buffer, ref pos_, ref sendTimes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref sendLimit);
				BaseDLL.decode_uint32(buffer, ref pos_, ref sendedTotalTimes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref sendedTotalLimit);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, guid);
				BaseDLL.encode_uint64(buffer, ref pos_, friendId);
				BaseDLL.encode_int8(buffer, ref pos_, friendOcc);
				BaseDLL.encode_uint16(buffer, ref pos_, friendLev);
				byte[] friendnameBytes = StringHelper.StringToUTF8Bytes(friendname);
				BaseDLL.encode_string(buffer, ref pos_, friendnameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, isOnline);
				BaseDLL.encode_uint32(buffer, ref pos_, beSendedTimes);
				BaseDLL.encode_uint32(buffer, ref pos_, beSendedLimit);
				BaseDLL.encode_uint32(buffer, ref pos_, sendTimes);
				BaseDLL.encode_uint32(buffer, ref pos_, sendLimit);
				BaseDLL.encode_uint32(buffer, ref pos_, sendedTotalTimes);
				BaseDLL.encode_uint32(buffer, ref pos_, sendedTotalLimit);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref guid);
				BaseDLL.decode_uint64(buffer, ref pos_, ref friendId);
				BaseDLL.decode_int8(buffer, ref pos_, ref friendOcc);
				BaseDLL.decode_uint16(buffer, ref pos_, ref friendLev);
				UInt16 friendnameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref friendnameLen);
				byte[] friendnameBytes = new byte[friendnameLen];
				for(int i = 0; i < friendnameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref friendnameBytes[i]);
				}
				friendname = StringHelper.BytesToString(friendnameBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref isOnline);
				BaseDLL.decode_uint32(buffer, ref pos_, ref beSendedTimes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref beSendedLimit);
				BaseDLL.decode_uint32(buffer, ref pos_, ref sendTimes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref sendLimit);
				BaseDLL.decode_uint32(buffer, ref pos_, ref sendedTotalTimes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref sendedTotalLimit);
			}

			public int getLen()
			{
				int _len = 0;
				// guid
				_len += 8;
				// friendId
				_len += 8;
				// friendOcc
				_len += 1;
				// friendLev
				_len += 2;
				// friendname
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(friendname);
					_len += 2 + _strBytes.Length;
				}
				// isOnline
				_len += 1;
				// beSendedTimes
				_len += 4;
				// beSendedLimit
				_len += 4;
				// sendTimes
				_len += 4;
				// sendLimit
				_len += 4;
				// sendedTotalTimes
				_len += 4;
				// sendedTotalLimit
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class WorldGetItemFriendPresentInfosReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 609701;
		public UInt32 Sequence;
		/// <summary>
		///  道具id
		/// </summary>
		public UInt32 dataId;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, dataId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref dataId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, dataId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref dataId);
			}

			public int getLen()
			{
				int _len = 0;
				// dataId
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class WorldGetItemFriendPresentInfosRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 609702;
		public UInt32 Sequence;
		/// <summary>
		///  道具id
		/// </summary>
		public UInt32 dataId;
		/// <summary>
		///  赠送数据
		/// </summary>
		public FriendPresentInfo[] presentInfos = new FriendPresentInfo[0];
		/// <summary>
		///  被赠送总次数
		/// </summary>
		public UInt32 recvedTotal;
		/// <summary>
		///  被赠送总次数上限
		/// </summary>
		public UInt32 recvedTotalLimit;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, dataId);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)presentInfos.Length);
				for(int i = 0; i < presentInfos.Length; i++)
				{
					presentInfos[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_uint32(buffer, ref pos_, recvedTotal);
				BaseDLL.encode_uint32(buffer, ref pos_, recvedTotalLimit);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref dataId);
				UInt16 presentInfosCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref presentInfosCnt);
				presentInfos = new FriendPresentInfo[presentInfosCnt];
				for(int i = 0; i < presentInfos.Length; i++)
				{
					presentInfos[i] = new FriendPresentInfo();
					presentInfos[i].decode(buffer, ref pos_);
				}
				BaseDLL.decode_uint32(buffer, ref pos_, ref recvedTotal);
				BaseDLL.decode_uint32(buffer, ref pos_, ref recvedTotalLimit);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, dataId);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)presentInfos.Length);
				for(int i = 0; i < presentInfos.Length; i++)
				{
					presentInfos[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_uint32(buffer, ref pos_, recvedTotal);
				BaseDLL.encode_uint32(buffer, ref pos_, recvedTotalLimit);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref dataId);
				UInt16 presentInfosCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref presentInfosCnt);
				presentInfos = new FriendPresentInfo[presentInfosCnt];
				for(int i = 0; i < presentInfos.Length; i++)
				{
					presentInfos[i] = new FriendPresentInfo();
					presentInfos[i].decode(buffer, ref pos_);
				}
				BaseDLL.decode_uint32(buffer, ref pos_, ref recvedTotal);
				BaseDLL.decode_uint32(buffer, ref pos_, ref recvedTotalLimit);
			}

			public int getLen()
			{
				int _len = 0;
				// dataId
				_len += 4;
				// presentInfos
				_len += 2;
				for(int j = 0; j < presentInfos.Length; j++)
				{
					_len += presentInfos[j].getLen();
				}
				// recvedTotal
				_len += 4;
				// recvedTotalLimit
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class WorldItemFriendPresentReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 609703;
		public UInt32 Sequence;
		/// <summary>
		///  道具guid
		/// </summary>
		public UInt64 itemId;
		/// <summary>
		///  道具类型id
		/// </summary>
		public UInt32 itemTypeId;
		/// <summary>
		///  赠送好友id
		/// </summary>
		public UInt64 friendId;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, itemId);
				BaseDLL.encode_uint32(buffer, ref pos_, itemTypeId);
				BaseDLL.encode_uint64(buffer, ref pos_, friendId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref itemId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemTypeId);
				BaseDLL.decode_uint64(buffer, ref pos_, ref friendId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, itemId);
				BaseDLL.encode_uint32(buffer, ref pos_, itemTypeId);
				BaseDLL.encode_uint64(buffer, ref pos_, friendId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref itemId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemTypeId);
				BaseDLL.decode_uint64(buffer, ref pos_, ref friendId);
			}

			public int getLen()
			{
				int _len = 0;
				// itemId
				_len += 8;
				// itemTypeId
				_len += 4;
				// friendId
				_len += 8;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class WorldItemFriendPresentRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 609704;
		public UInt32 Sequence;
		/// <summary>
		/// 返回码
		/// </summary>
		public UInt32 retCode;
		/// <summary>
		///  道具guid
		/// </summary>
		public UInt64 itemId;
		/// <summary>
		///  道具类型id
		/// </summary>
		public UInt32 itemTypeId;
		/// <summary>
		///  更新的赠送信息
		/// </summary>
		public FriendPresentInfo presentInfos = new FriendPresentInfo();

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, retCode);
				BaseDLL.encode_uint64(buffer, ref pos_, itemId);
				BaseDLL.encode_uint32(buffer, ref pos_, itemTypeId);
				presentInfos.encode(buffer, ref pos_);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
				BaseDLL.decode_uint64(buffer, ref pos_, ref itemId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemTypeId);
				presentInfos.decode(buffer, ref pos_);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, retCode);
				BaseDLL.encode_uint64(buffer, ref pos_, itemId);
				BaseDLL.encode_uint32(buffer, ref pos_, itemTypeId);
				presentInfos.encode(buffer, ref pos_);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
				BaseDLL.decode_uint64(buffer, ref pos_, ref itemId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemTypeId);
				presentInfos.decode(buffer, ref pos_);
			}

			public int getLen()
			{
				int _len = 0;
				// retCode
				_len += 4;
				// itemId
				_len += 8;
				// itemTypeId
				_len += 4;
				// presentInfos
				_len += presentInfos.getLen();
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  宝珠转换请求
	/// </summary>
	[Protocol]
	public class SceneBeadConvertReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501090;
		public UInt32 Sequence;
		/// <summary>
		///  转换的宝珠
		/// </summary>
		public UInt64 beadGuid;
		/// <summary>
		///  使用的材料(0使用金币)
		/// </summary>
		public UInt64 materialGuid;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, beadGuid);
				BaseDLL.encode_uint64(buffer, ref pos_, materialGuid);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref beadGuid);
				BaseDLL.decode_uint64(buffer, ref pos_, ref materialGuid);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, beadGuid);
				BaseDLL.encode_uint64(buffer, ref pos_, materialGuid);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref beadGuid);
				BaseDLL.decode_uint64(buffer, ref pos_, ref materialGuid);
			}

			public int getLen()
			{
				int _len = 0;
				// beadGuid
				_len += 8;
				// materialGuid
				_len += 8;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  宝珠转换返回
	/// </summary>
	[Protocol]
	public class SceneBeadConvertRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501091;
		public UInt32 Sequence;
		public UInt32 retCode;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, retCode);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, retCode);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
			}

			public int getLen()
			{
				int _len = 0;
				// retCode
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  同一背包移动道具到指定位置
	/// </summary>
	[Protocol]
	public class SceneMoveItem : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500962;
		public UInt32 Sequence;
		/// <summary>
		///  道具guid
		/// </summary>
		public UInt64 itemId;
		/// <summary>
		///  道具数量
		/// </summary>
		public UInt16 num;
		/// <summary>
		///  道具目标位置
		/// </summary>
		public ItemPos targetPos = new ItemPos();

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, itemId);
				BaseDLL.encode_uint16(buffer, ref pos_, num);
				targetPos.encode(buffer, ref pos_);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref itemId);
				BaseDLL.decode_uint16(buffer, ref pos_, ref num);
				targetPos.decode(buffer, ref pos_);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, itemId);
				BaseDLL.encode_uint16(buffer, ref pos_, num);
				targetPos.encode(buffer, ref pos_);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref itemId);
				BaseDLL.decode_uint16(buffer, ref pos_, ref num);
				targetPos.decode(buffer, ref pos_);
			}

			public int getLen()
			{
				int _len = 0;
				// itemId
				_len += 8;
				// num
				_len += 2;
				// targetPos
				_len += targetPos.getLen();
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class SceneMoveItemRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 500963;
		public UInt32 Sequence;
		public UInt32 code;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
			}

			public int getLen()
			{
				int _len = 0;
				// code
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 装备方案信息
	/// </summary>
	public class EquipSchemeInfo : Protocol.IProtocolStream
	{
		/// <summary>
		/// 唯一id
		/// </summary>
		public UInt64 guid;
		/// <summary>
		/// 类型
		/// </summary>
		public byte type;
		/// <summary>
		/// 方案id
		/// </summary>
		public UInt32 id;
		/// <summary>
		/// 是否穿戴
		/// </summary>
		public byte weared;
		/// <summary>
		/// 装备id
		/// </summary>
		public UInt64[] equips = new UInt64[0];

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, guid);
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_uint32(buffer, ref pos_, id);
				BaseDLL.encode_int8(buffer, ref pos_, weared);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)equips.Length);
				for(int i = 0; i < equips.Length; i++)
				{
					BaseDLL.encode_uint64(buffer, ref pos_, equips[i]);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref guid);
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
				BaseDLL.decode_int8(buffer, ref pos_, ref weared);
				UInt16 equipsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref equipsCnt);
				equips = new UInt64[equipsCnt];
				for(int i = 0; i < equips.Length; i++)
				{
					BaseDLL.decode_uint64(buffer, ref pos_, ref equips[i]);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, guid);
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_uint32(buffer, ref pos_, id);
				BaseDLL.encode_int8(buffer, ref pos_, weared);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)equips.Length);
				for(int i = 0; i < equips.Length; i++)
				{
					BaseDLL.encode_uint64(buffer, ref pos_, equips[i]);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref guid);
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
				BaseDLL.decode_int8(buffer, ref pos_, ref weared);
				UInt16 equipsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref equipsCnt);
				equips = new UInt64[equipsCnt];
				for(int i = 0; i < equips.Length; i++)
				{
					BaseDLL.decode_uint64(buffer, ref pos_, ref equips[i]);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// guid
				_len += 8;
				// type
				_len += 1;
				// id
				_len += 4;
				// weared
				_len += 1;
				// equips
				_len += 2 + 8 * equips.Length;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// scene->client	同步装备方案数据
	/// </summary>
	[Protocol]
	public class SceneEquipSchemeSync : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501087;
		public UInt32 Sequence;
		public EquipSchemeInfo[] schemes = new EquipSchemeInfo[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)schemes.Length);
				for(int i = 0; i < schemes.Length; i++)
				{
					schemes[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 schemesCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref schemesCnt);
				schemes = new EquipSchemeInfo[schemesCnt];
				for(int i = 0; i < schemes.Length; i++)
				{
					schemes[i] = new EquipSchemeInfo();
					schemes[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)schemes.Length);
				for(int i = 0; i < schemes.Length; i++)
				{
					schemes[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 schemesCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref schemesCnt);
				schemes = new EquipSchemeInfo[schemesCnt];
				for(int i = 0; i < schemes.Length; i++)
				{
					schemes[i] = new EquipSchemeInfo();
					schemes[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// schemes
				_len += 2;
				for(int j = 0; j < schemes.Length; j++)
				{
					_len += schemes[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// client->scene   穿戴装备方案请求
	/// </summary>
	[Protocol]
	public class SceneEquipSchemeWearReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501088;
		public UInt32 Sequence;
		/// <summary>
		/// 类型id
		/// </summary>
		public byte type;
		/// <summary>
		/// 方案id
		/// </summary>
		public UInt32 id;
		/// <summary>
		/// 是否同步方案
		/// </summary>
		public byte isSync;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_uint32(buffer, ref pos_, id);
				BaseDLL.encode_int8(buffer, ref pos_, isSync);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
				BaseDLL.decode_int8(buffer, ref pos_, ref isSync);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_uint32(buffer, ref pos_, id);
				BaseDLL.encode_int8(buffer, ref pos_, isSync);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
				BaseDLL.decode_int8(buffer, ref pos_, ref isSync);
			}

			public int getLen()
			{
				int _len = 0;
				// type
				_len += 1;
				// id
				_len += 4;
				// isSync
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// scene->client   穿戴装备方案返回
	/// </summary>
	[Protocol]
	public class SceneEquipSchemeWearRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501089;
		public UInt32 Sequence;
		/// <summary>
		/// 结果
		/// </summary>
		public UInt32 code;
		/// <summary>
		/// 类型id
		/// </summary>
		public byte type;
		/// <summary>
		/// 方案id
		/// </summary>
		public UInt32 id;
		/// <summary>
		/// 是否同步方案
		/// </summary>
		public byte isSync;
		/// <summary>
		/// 到期的装备
		/// </summary>
		public UInt64[] overdueIds = new UInt64[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_uint32(buffer, ref pos_, id);
				BaseDLL.encode_int8(buffer, ref pos_, isSync);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)overdueIds.Length);
				for(int i = 0; i < overdueIds.Length; i++)
				{
					BaseDLL.encode_uint64(buffer, ref pos_, overdueIds[i]);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
				BaseDLL.decode_int8(buffer, ref pos_, ref isSync);
				UInt16 overdueIdsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref overdueIdsCnt);
				overdueIds = new UInt64[overdueIdsCnt];
				for(int i = 0; i < overdueIds.Length; i++)
				{
					BaseDLL.decode_uint64(buffer, ref pos_, ref overdueIds[i]);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_uint32(buffer, ref pos_, id);
				BaseDLL.encode_int8(buffer, ref pos_, isSync);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)overdueIds.Length);
				for(int i = 0; i < overdueIds.Length; i++)
				{
					BaseDLL.encode_uint64(buffer, ref pos_, overdueIds[i]);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_uint32(buffer, ref pos_, ref id);
				BaseDLL.decode_int8(buffer, ref pos_, ref isSync);
				UInt16 overdueIdsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref overdueIdsCnt);
				overdueIds = new UInt64[overdueIdsCnt];
				for(int i = 0; i < overdueIds.Length; i++)
				{
					BaseDLL.decode_uint64(buffer, ref pos_, ref overdueIds[i]);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// code
				_len += 4;
				// type
				_len += 1;
				// id
				_len += 4;
				// isSync
				_len += 1;
				// overdueIds
				_len += 2 + 8 * overdueIds.Length;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  装备转换请求
	/// </summary>
	[Protocol]
	public class SceneEquipConvertReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501092;
		public UInt32 Sequence;
		/// <summary>
		///  装换类型(EquipConvertType)
		/// </summary>
		public byte type;
		/// <summary>
		///  原装备id
		/// </summary>
		public UInt64 sourceEqGuid;
		/// <summary>
		///  目标装备类型id
		/// </summary>
		public UInt32 targetEqTypeId;
		/// <summary>
		///  转换器guid
		/// </summary>
		public UInt64 convertorGuid;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_uint64(buffer, ref pos_, sourceEqGuid);
				BaseDLL.encode_uint32(buffer, ref pos_, targetEqTypeId);
				BaseDLL.encode_uint64(buffer, ref pos_, convertorGuid);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_uint64(buffer, ref pos_, ref sourceEqGuid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref targetEqTypeId);
				BaseDLL.decode_uint64(buffer, ref pos_, ref convertorGuid);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_uint64(buffer, ref pos_, sourceEqGuid);
				BaseDLL.encode_uint32(buffer, ref pos_, targetEqTypeId);
				BaseDLL.encode_uint64(buffer, ref pos_, convertorGuid);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_uint64(buffer, ref pos_, ref sourceEqGuid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref targetEqTypeId);
				BaseDLL.decode_uint64(buffer, ref pos_, ref convertorGuid);
			}

			public int getLen()
			{
				int _len = 0;
				// type
				_len += 1;
				// sourceEqGuid
				_len += 8;
				// targetEqTypeId
				_len += 4;
				// convertorGuid
				_len += 8;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  装备转换返回
	/// </summary>
	[Protocol]
	public class SceneEquipConvertRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501093;
		public UInt32 Sequence;
		public UInt32 retCode;
		public UInt64 eqGuid;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, retCode);
				BaseDLL.encode_uint64(buffer, ref pos_, eqGuid);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
				BaseDLL.decode_uint64(buffer, ref pos_, ref eqGuid);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, retCode);
				BaseDLL.encode_uint64(buffer, ref pos_, eqGuid);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
				BaseDLL.decode_uint64(buffer, ref pos_, ref eqGuid);
			}

			public int getLen()
			{
				int _len = 0;
				// retCode
				_len += 4;
				// eqGuid
				_len += 8;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  植树节种树请求
	/// </summary>
	[Protocol]
	public class SceneActivePlantReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501094;
		public UInt32 Sequence;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
			}

			public void decode(byte[] buffer, ref int pos_)
			{
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
			}

			public int getLen()
			{
				int _len = 0;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  植树节种树返回
	/// </summary>
	[Protocol]
	public class SceneActivePlantRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501095;
		public UInt32 Sequence;
		public UInt32 retCode;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, retCode);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, retCode);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
			}

			public int getLen()
			{
				int _len = 0;
				// retCode
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///   植树节鉴定请求
	/// </summary>
	[Protocol]
	public class SceneActivePlantAppraReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501096;
		public UInt32 Sequence;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
			}

			public void decode(byte[] buffer, ref int pos_)
			{
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
			}

			public int getLen()
			{
				int _len = 0;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  植树节鉴定返回
	/// </summary>
	[Protocol]
	public class SceneActivePlantAppraRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501097;
		public UInt32 Sequence;
		public UInt32 retCode;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, retCode);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, retCode);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
			}

			public int getLen()
			{
				int _len = 0;
				// retCode
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///   解锁角色仓库请求
	/// </summary>
	[Protocol]
	public class SceneUnlockRoleStorageReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501098;
		public UInt32 Sequence;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
			}

			public void decode(byte[] buffer, ref int pos_)
			{
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
			}

			public int getLen()
			{
				int _len = 0;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  解锁角色仓库返回
	/// </summary>
	[Protocol]
	public class SceneUnlockRoleStorageRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501099;
		public UInt32 Sequence;
		public UInt32 retCode;
		public UInt32 curOpenNum;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, retCode);
				BaseDLL.encode_uint32(buffer, ref pos_, curOpenNum);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
				BaseDLL.decode_uint32(buffer, ref pos_, ref curOpenNum);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, retCode);
				BaseDLL.encode_uint32(buffer, ref pos_, curOpenNum);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
				BaseDLL.decode_uint32(buffer, ref pos_, ref curOpenNum);
			}

			public int getLen()
			{
				int _len = 0;
				// retCode
				_len += 4;
				// curOpenNum
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///   client->scene 商店批量查询商品请求
	/// </summary>
	[Protocol]
	public class SceneShopQueryBatchItemReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501100;
		public UInt32 Sequence;
		/// <summary>
		///  商店商品id
		/// </summary>
		public UInt32[] shopItemIds = new UInt32[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)shopItemIds.Length);
				for(int i = 0; i < shopItemIds.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, shopItemIds[i]);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 shopItemIdsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref shopItemIdsCnt);
				shopItemIds = new UInt32[shopItemIdsCnt];
				for(int i = 0; i < shopItemIds.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref shopItemIds[i]);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)shopItemIds.Length);
				for(int i = 0; i < shopItemIds.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, shopItemIds[i]);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 shopItemIdsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref shopItemIdsCnt);
				shopItemIds = new UInt32[shopItemIdsCnt];
				for(int i = 0; i < shopItemIds.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref shopItemIds[i]);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// shopItemIds
				_len += 2 + 4 * shopItemIds.Length;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  scene->client 商店批量查询商品返回
	/// </summary>
	[Protocol]
	public class SceneShopQueryBatchItemRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501101;
		public UInt32 Sequence;
		/// <summary>
		///  商品列表
		/// </summary>
		public ProtoShopItem[] shopItemList = new ProtoShopItem[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)shopItemList.Length);
				for(int i = 0; i < shopItemList.Length; i++)
				{
					shopItemList[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 shopItemListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref shopItemListCnt);
				shopItemList = new ProtoShopItem[shopItemListCnt];
				for(int i = 0; i < shopItemList.Length; i++)
				{
					shopItemList[i] = new ProtoShopItem();
					shopItemList[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)shopItemList.Length);
				for(int i = 0; i < shopItemList.Length; i++)
				{
					shopItemList[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 shopItemListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref shopItemListCnt);
				shopItemList = new ProtoShopItem[shopItemListCnt];
				for(int i = 0; i < shopItemList.Length; i++)
				{
					shopItemList[i] = new ProtoShopItem();
					shopItemList[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// shopItemList
				_len += 2;
				for(int j = 0; j < shopItemList.Length; j++)
				{
					_len += shopItemList[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  client->scene 请求 设置仓库名字
	/// </summary>
	[Protocol]
	public class SceneSetStorageNameReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501801;
		public UInt32 Sequence;
		public UInt16 storageType;
		public string storageName;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, storageType);
				byte[] storageNameBytes = StringHelper.StringToUTF8Bytes(storageName);
				BaseDLL.encode_string(buffer, ref pos_, storageNameBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint16(buffer, ref pos_, ref storageType);
				UInt16 storageNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref storageNameLen);
				byte[] storageNameBytes = new byte[storageNameLen];
				for(int i = 0; i < storageNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref storageNameBytes[i]);
				}
				storageName = StringHelper.BytesToString(storageNameBytes);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, storageType);
				byte[] storageNameBytes = StringHelper.StringToUTF8Bytes(storageName);
				BaseDLL.encode_string(buffer, ref pos_, storageNameBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint16(buffer, ref pos_, ref storageType);
				UInt16 storageNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref storageNameLen);
				byte[] storageNameBytes = new byte[storageNameLen];
				for(int i = 0; i < storageNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref storageNameBytes[i]);
				}
				storageName = StringHelper.BytesToString(storageNameBytes);
			}

			public int getLen()
			{
				int _len = 0;
				// storageType
				_len += 2;
				// storageName
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(storageName);
					_len += 2 + _strBytes.Length;
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  scene->client 返回 设置仓库名字
	/// </summary>
	[Protocol]
	public class SceneSetStorageNameRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501802;
		public UInt32 Sequence;
		public UInt32 result;
		public UInt16 storageType;
		public string storageName;

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
				BaseDLL.encode_uint16(buffer, ref pos_, storageType);
				byte[] storageNameBytes = StringHelper.StringToUTF8Bytes(storageName);
				BaseDLL.encode_string(buffer, ref pos_, storageNameBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				BaseDLL.decode_uint16(buffer, ref pos_, ref storageType);
				UInt16 storageNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref storageNameLen);
				byte[] storageNameBytes = new byte[storageNameLen];
				for(int i = 0; i < storageNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref storageNameBytes[i]);
				}
				storageName = StringHelper.BytesToString(storageNameBytes);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
				BaseDLL.encode_uint16(buffer, ref pos_, storageType);
				byte[] storageNameBytes = StringHelper.StringToUTF8Bytes(storageName);
				BaseDLL.encode_string(buffer, ref pos_, storageNameBytes, (UInt16)(buffer.Length - pos_));
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				BaseDLL.decode_uint16(buffer, ref pos_, ref storageType);
				UInt16 storageNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref storageNameLen);
				byte[] storageNameBytes = new byte[storageNameLen];
				for(int i = 0; i < storageNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref storageNameBytes[i]);
				}
				storageName = StringHelper.BytesToString(storageNameBytes);
			}

			public int getLen()
			{
				int _len = 0;
				// result
				_len += 4;
				// storageType
				_len += 2;
				// storageName
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(storageName);
					_len += 2 + _strBytes.Length;
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  scene->client 同步仓库名字
	/// </summary>
	[Protocol]
	public class SceneStorageNameSync : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 501803;
		public UInt32 Sequence;
		public StorageNameInfo[] infos = new StorageNameInfo[0];

		#region METHOD
			public UInt32 GetMsgID()
			{
				return MsgID;
			}

			public UInt32 GetSequence()
			{
				return Sequence;
			}

			public void SetSequence(UInt32 sequence)
			{
				Sequence = sequence;
			}

			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)infos.Length);
				for(int i = 0; i < infos.Length; i++)
				{
					infos[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 infosCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref infosCnt);
				infos = new StorageNameInfo[infosCnt];
				for(int i = 0; i < infos.Length; i++)
				{
					infos[i] = new StorageNameInfo();
					infos[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)infos.Length);
				for(int i = 0; i < infos.Length; i++)
				{
					infos[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 infosCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref infosCnt);
				infos = new StorageNameInfo[infosCnt];
				for(int i = 0; i < infos.Length; i++)
				{
					infos[i] = new StorageNameInfo();
					infos[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// infos
				_len += 2;
				for(int j = 0; j < infos.Length; j++)
				{
					_len += infos[j].getLen();
				}
				return _len;
			}
		#endregion

	}

}
