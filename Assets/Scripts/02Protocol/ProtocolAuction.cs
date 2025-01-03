using System;
using System.Text;

namespace Protocol
{
	public enum AuctionType
	{
		Item = 0,
		Gold = 1,
	}

	public enum AuctionSortType
	{
		/// <summary>
		///  按价格升序
		/// </summary>
		PriceAsc = 0,
		/// <summary>
		///  按价格降序
		/// </summary>
		PriceDesc = 1,
	}

	public enum AuctionSellDuration
	{
		/// <summary>
		///  24小时
		/// </summary>
		Hour_24 = 0,
		/// <summary>
		///  48小时
		/// </summary>
		Hour_48 = 1,
	}

	public enum AuctionMainItemType
	{
		/// <summary>
		///  无效
		/// </summary>
		AMIT_INVALID = 0,
		/// <summary>
		///  武器
		/// </summary>
		AMIT_WEAPON = 1,
		/// <summary>
		///  防具
		/// </summary>
		AMIT_ARMOR = 2,
		/// <summary>
		///  首饰
		/// </summary>
		AMIT_JEWELRY = 3,
		/// <summary>
		///  消耗品
		/// </summary>
		AMIT_COST = 4,
		/// <summary>
		///  材料
		/// </summary>
		AMIT_MATERIAL = 5,
		/// <summary>
		///  其它
		/// </summary>
		AMIT_OTHER = 6,
		/// <summary>
		///  数量
		/// </summary>
		AMIT_NUM = 7,
	}

	/// <summary>
	///  拍卖行刷新原因
	/// </summary>
	public enum AuctionRefreshReason
	{
		/// <summary>
		///  购买
		/// </summary>
		SRR_BUY = 0,
		/// <summary>
		///  上架
		/// </summary>
		SRR_SELL = 1,
		/// <summary>
		///  下架
		/// </summary>
		SRR_CANCEL = 2,
		/// <summary>
		///  抢购
		/// </summary>
		SRR_RUSY_BUY = 3,
		/// <summary>
		///  扫货
		/// </summary>
		SRR_SYS_RECOVERY = 4,
	}

	/// <summary>
	///  商品拍卖状态
	/// </summary>
	public enum AuctionGoodState
	{
		/// <summary>
		///  无效
		/// </summary>
		AGS_INVALID = 0,
		/// <summary>
		///  公示
		/// </summary>
		AGS_PUBLIC = 1,
		/// <summary>
		///  上架
		/// </summary>
		AGS_ONSALE = 2,
	}

	/// <summary>
	/// 拍卖行关注操作
	/// </summary>
	public enum AuctionAttentOpType
	{
		/// <summary>
		/// 关注
		/// </summary>
		ATOT_ATTENT = 1,
		/// <summary>
		/// 取消关注
		/// </summary>
		ATOT_CANCEL = 2,
	}

	/// <summary>
	/// 拍卖行关注类型
	/// </summary>
	public enum AuctionAttentType
	{
		/// <summary>
		/// 非关注
		/// </summary>
		ATT_NOTATTENT = 0,
		/// <summary>
		/// 关注
		/// </summary>
		ATT_ATTENT = 1,
	}

	public class AuctionQueryCondition : Protocol.IProtocolStream
	{
		/// <summary>
		///  拍卖类型
		/// </summary>
		public byte type;
		/// <summary>
		///  商品状态(AuctionGoodState)
		/// </summary>
		public byte goodState;
		/// <summary>
		///  物品主类型(AuctionMainItemType)
		/// </summary>
		public byte itemMainType;
		/// <summary>
		///  物品子类型
		/// </summary>
		public UInt32[] itemSubTypes = new UInt32[0];
		/// <summary>
		///  排除物品子类型
		/// </summary>
		public UInt32[] excludeItemSubTypes = new UInt32[0];
		/// <summary>
		///  物品ID
		/// </summary>
		public UInt32 itemTypeID;
		/// <summary>
		///  品质
		/// </summary>
		public byte quality;
		/// <summary>
		///  最低物品等级
		/// </summary>
		public byte minLevel;
		/// <summary>
		///  最高物品等级
		/// </summary>
		public byte maxLevel;
		/// <summary>
		///  最低强化等级
		/// </summary>
		public byte minStrength;
		/// <summary>
		///  最高强化等级
		/// </summary>
		public byte maxStrength;
		/// <summary>
		///  排序方式（对应枚举AuctionSortType）
		/// </summary>
		public byte sortType;
		/// <summary>
		///  页数
		/// </summary>
		public UInt16 page;
		/// <summary>
		///  每页物品数量
		/// </summary>
		public byte itemNumPerPage;
		/// <summary>
		///  强化卷强化等级
		/// </summary>
		public UInt32 couponStrengthToLev;
		/// <summary>
		///  是否关注[0]非关注,[1]关注,枚举(AuctionAttentType)
		/// </summary>
		public byte attent;
		/// <summary>
		///  职业
		/// </summary>
		public byte[] occus = new byte[0];

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_int8(buffer, ref pos_, goodState);
				BaseDLL.encode_int8(buffer, ref pos_, itemMainType);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)itemSubTypes.Length);
				for(int i = 0; i < itemSubTypes.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, itemSubTypes[i]);
				}
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)excludeItemSubTypes.Length);
				for(int i = 0; i < excludeItemSubTypes.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, excludeItemSubTypes[i]);
				}
				BaseDLL.encode_uint32(buffer, ref pos_, itemTypeID);
				BaseDLL.encode_int8(buffer, ref pos_, quality);
				BaseDLL.encode_int8(buffer, ref pos_, minLevel);
				BaseDLL.encode_int8(buffer, ref pos_, maxLevel);
				BaseDLL.encode_int8(buffer, ref pos_, minStrength);
				BaseDLL.encode_int8(buffer, ref pos_, maxStrength);
				BaseDLL.encode_int8(buffer, ref pos_, sortType);
				BaseDLL.encode_uint16(buffer, ref pos_, page);
				BaseDLL.encode_int8(buffer, ref pos_, itemNumPerPage);
				BaseDLL.encode_uint32(buffer, ref pos_, couponStrengthToLev);
				BaseDLL.encode_int8(buffer, ref pos_, attent);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)occus.Length);
				for(int i = 0; i < occus.Length; i++)
				{
					BaseDLL.encode_int8(buffer, ref pos_, occus[i]);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_int8(buffer, ref pos_, ref goodState);
				BaseDLL.decode_int8(buffer, ref pos_, ref itemMainType);
				UInt16 itemSubTypesCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref itemSubTypesCnt);
				itemSubTypes = new UInt32[itemSubTypesCnt];
				for(int i = 0; i < itemSubTypes.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref itemSubTypes[i]);
				}
				UInt16 excludeItemSubTypesCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref excludeItemSubTypesCnt);
				excludeItemSubTypes = new UInt32[excludeItemSubTypesCnt];
				for(int i = 0; i < excludeItemSubTypes.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref excludeItemSubTypes[i]);
				}
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemTypeID);
				BaseDLL.decode_int8(buffer, ref pos_, ref quality);
				BaseDLL.decode_int8(buffer, ref pos_, ref minLevel);
				BaseDLL.decode_int8(buffer, ref pos_, ref maxLevel);
				BaseDLL.decode_int8(buffer, ref pos_, ref minStrength);
				BaseDLL.decode_int8(buffer, ref pos_, ref maxStrength);
				BaseDLL.decode_int8(buffer, ref pos_, ref sortType);
				BaseDLL.decode_uint16(buffer, ref pos_, ref page);
				BaseDLL.decode_int8(buffer, ref pos_, ref itemNumPerPage);
				BaseDLL.decode_uint32(buffer, ref pos_, ref couponStrengthToLev);
				BaseDLL.decode_int8(buffer, ref pos_, ref attent);
				UInt16 occusCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref occusCnt);
				occus = new byte[occusCnt];
				for(int i = 0; i < occus.Length; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref occus[i]);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_int8(buffer, ref pos_, goodState);
				BaseDLL.encode_int8(buffer, ref pos_, itemMainType);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)itemSubTypes.Length);
				for(int i = 0; i < itemSubTypes.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, itemSubTypes[i]);
				}
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)excludeItemSubTypes.Length);
				for(int i = 0; i < excludeItemSubTypes.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, excludeItemSubTypes[i]);
				}
				BaseDLL.encode_uint32(buffer, ref pos_, itemTypeID);
				BaseDLL.encode_int8(buffer, ref pos_, quality);
				BaseDLL.encode_int8(buffer, ref pos_, minLevel);
				BaseDLL.encode_int8(buffer, ref pos_, maxLevel);
				BaseDLL.encode_int8(buffer, ref pos_, minStrength);
				BaseDLL.encode_int8(buffer, ref pos_, maxStrength);
				BaseDLL.encode_int8(buffer, ref pos_, sortType);
				BaseDLL.encode_uint16(buffer, ref pos_, page);
				BaseDLL.encode_int8(buffer, ref pos_, itemNumPerPage);
				BaseDLL.encode_uint32(buffer, ref pos_, couponStrengthToLev);
				BaseDLL.encode_int8(buffer, ref pos_, attent);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)occus.Length);
				for(int i = 0; i < occus.Length; i++)
				{
					BaseDLL.encode_int8(buffer, ref pos_, occus[i]);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_int8(buffer, ref pos_, ref goodState);
				BaseDLL.decode_int8(buffer, ref pos_, ref itemMainType);
				UInt16 itemSubTypesCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref itemSubTypesCnt);
				itemSubTypes = new UInt32[itemSubTypesCnt];
				for(int i = 0; i < itemSubTypes.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref itemSubTypes[i]);
				}
				UInt16 excludeItemSubTypesCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref excludeItemSubTypesCnt);
				excludeItemSubTypes = new UInt32[excludeItemSubTypesCnt];
				for(int i = 0; i < excludeItemSubTypes.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref excludeItemSubTypes[i]);
				}
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemTypeID);
				BaseDLL.decode_int8(buffer, ref pos_, ref quality);
				BaseDLL.decode_int8(buffer, ref pos_, ref minLevel);
				BaseDLL.decode_int8(buffer, ref pos_, ref maxLevel);
				BaseDLL.decode_int8(buffer, ref pos_, ref minStrength);
				BaseDLL.decode_int8(buffer, ref pos_, ref maxStrength);
				BaseDLL.decode_int8(buffer, ref pos_, ref sortType);
				BaseDLL.decode_uint16(buffer, ref pos_, ref page);
				BaseDLL.decode_int8(buffer, ref pos_, ref itemNumPerPage);
				BaseDLL.decode_uint32(buffer, ref pos_, ref couponStrengthToLev);
				BaseDLL.decode_int8(buffer, ref pos_, ref attent);
				UInt16 occusCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref occusCnt);
				occus = new byte[occusCnt];
				for(int i = 0; i < occus.Length; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref occus[i]);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// type
				_len += 1;
				// goodState
				_len += 1;
				// itemMainType
				_len += 1;
				// itemSubTypes
				_len += 2 + 4 * itemSubTypes.Length;
				// excludeItemSubTypes
				_len += 2 + 4 * excludeItemSubTypes.Length;
				// itemTypeID
				_len += 4;
				// quality
				_len += 1;
				// minLevel
				_len += 1;
				// maxLevel
				_len += 1;
				// minStrength
				_len += 1;
				// maxStrength
				_len += 1;
				// sortType
				_len += 1;
				// page
				_len += 2;
				// itemNumPerPage
				_len += 1;
				// couponStrengthToLev
				_len += 4;
				// attent
				_len += 1;
				// occus
				_len += 2 + 1 * occus.Length;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  拍卖行道具基本信息（类型，数量）
	/// </summary>
	public class AuctionItemBaseInfo : Protocol.IProtocolStream
	{
		/// <summary>
		///  道具ID
		/// </summary>
		public UInt32 itemTypeId;
		/// <summary>
		///  道具数量
		/// </summary>
		public UInt32 num;
		/// <summary>
		///  是否是珍品
		/// </summary>
		public byte isTreas;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, itemTypeId);
				BaseDLL.encode_uint32(buffer, ref pos_, num);
				BaseDLL.encode_int8(buffer, ref pos_, isTreas);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemTypeId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref num);
				BaseDLL.decode_int8(buffer, ref pos_, ref isTreas);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, itemTypeId);
				BaseDLL.encode_uint32(buffer, ref pos_, num);
				BaseDLL.encode_int8(buffer, ref pos_, isTreas);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemTypeId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref num);
				BaseDLL.decode_int8(buffer, ref pos_, ref isTreas);
			}

			public int getLen()
			{
				int _len = 0;
				// itemTypeId
				_len += 4;
				// num
				_len += 4;
				// isTreas
				_len += 1;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class WorldAuctionListReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 603901;
		public UInt32 Sequence;
		public AuctionQueryCondition cond = new AuctionQueryCondition();

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
				cond.encode(buffer, ref pos_);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				cond.decode(buffer, ref pos_);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				cond.encode(buffer, ref pos_);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				cond.decode(buffer, ref pos_);
			}

			public int getLen()
			{
				int _len = 0;
				// cond
				_len += cond.getLen();
				return _len;
			}
		#endregion

	}

	public class AuctionBaseInfo : Protocol.IProtocolStream
	{
		/// <summary>
		///  唯一id
		/// </summary>
		public UInt64 guid;
		public UInt32 price;
		public byte pricetype;
		public UInt32 num;
		public UInt32 itemTypeId;
		public UInt32 strengthed;
		public UInt32 itemScore;
		public UInt32 publicEndTime;
		public byte isTreas;
		public UInt64 owner;
		public byte attent;
		public UInt32 attentNum;
		public UInt32 duetime;
		public UInt32 onSaleTime;
		public UInt32 beadbuffid;
		public byte isAgainOnsale;
		public byte equipType;
		public byte enhanceType;
		public UInt32 itemDueTime;
		public UInt32 enhanceNum;
		public UInt32 itemTransNum;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, guid);
				BaseDLL.encode_uint32(buffer, ref pos_, price);
				BaseDLL.encode_int8(buffer, ref pos_, pricetype);
				BaseDLL.encode_uint32(buffer, ref pos_, num);
				BaseDLL.encode_uint32(buffer, ref pos_, itemTypeId);
				BaseDLL.encode_uint32(buffer, ref pos_, strengthed);
				BaseDLL.encode_uint32(buffer, ref pos_, itemScore);
				BaseDLL.encode_uint32(buffer, ref pos_, publicEndTime);
				BaseDLL.encode_int8(buffer, ref pos_, isTreas);
				BaseDLL.encode_uint64(buffer, ref pos_, owner);
				BaseDLL.encode_int8(buffer, ref pos_, attent);
				BaseDLL.encode_uint32(buffer, ref pos_, attentNum);
				BaseDLL.encode_uint32(buffer, ref pos_, duetime);
				BaseDLL.encode_uint32(buffer, ref pos_, onSaleTime);
				BaseDLL.encode_uint32(buffer, ref pos_, beadbuffid);
				BaseDLL.encode_int8(buffer, ref pos_, isAgainOnsale);
				BaseDLL.encode_int8(buffer, ref pos_, equipType);
				BaseDLL.encode_int8(buffer, ref pos_, enhanceType);
				BaseDLL.encode_uint32(buffer, ref pos_, itemDueTime);
				BaseDLL.encode_uint32(buffer, ref pos_, enhanceNum);
				BaseDLL.encode_uint32(buffer, ref pos_, itemTransNum);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref guid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref price);
				BaseDLL.decode_int8(buffer, ref pos_, ref pricetype);
				BaseDLL.decode_uint32(buffer, ref pos_, ref num);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemTypeId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref strengthed);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemScore);
				BaseDLL.decode_uint32(buffer, ref pos_, ref publicEndTime);
				BaseDLL.decode_int8(buffer, ref pos_, ref isTreas);
				BaseDLL.decode_uint64(buffer, ref pos_, ref owner);
				BaseDLL.decode_int8(buffer, ref pos_, ref attent);
				BaseDLL.decode_uint32(buffer, ref pos_, ref attentNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref duetime);
				BaseDLL.decode_uint32(buffer, ref pos_, ref onSaleTime);
				BaseDLL.decode_uint32(buffer, ref pos_, ref beadbuffid);
				BaseDLL.decode_int8(buffer, ref pos_, ref isAgainOnsale);
				BaseDLL.decode_int8(buffer, ref pos_, ref equipType);
				BaseDLL.decode_int8(buffer, ref pos_, ref enhanceType);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemDueTime);
				BaseDLL.decode_uint32(buffer, ref pos_, ref enhanceNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemTransNum);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, guid);
				BaseDLL.encode_uint32(buffer, ref pos_, price);
				BaseDLL.encode_int8(buffer, ref pos_, pricetype);
				BaseDLL.encode_uint32(buffer, ref pos_, num);
				BaseDLL.encode_uint32(buffer, ref pos_, itemTypeId);
				BaseDLL.encode_uint32(buffer, ref pos_, strengthed);
				BaseDLL.encode_uint32(buffer, ref pos_, itemScore);
				BaseDLL.encode_uint32(buffer, ref pos_, publicEndTime);
				BaseDLL.encode_int8(buffer, ref pos_, isTreas);
				BaseDLL.encode_uint64(buffer, ref pos_, owner);
				BaseDLL.encode_int8(buffer, ref pos_, attent);
				BaseDLL.encode_uint32(buffer, ref pos_, attentNum);
				BaseDLL.encode_uint32(buffer, ref pos_, duetime);
				BaseDLL.encode_uint32(buffer, ref pos_, onSaleTime);
				BaseDLL.encode_uint32(buffer, ref pos_, beadbuffid);
				BaseDLL.encode_int8(buffer, ref pos_, isAgainOnsale);
				BaseDLL.encode_int8(buffer, ref pos_, equipType);
				BaseDLL.encode_int8(buffer, ref pos_, enhanceType);
				BaseDLL.encode_uint32(buffer, ref pos_, itemDueTime);
				BaseDLL.encode_uint32(buffer, ref pos_, enhanceNum);
				BaseDLL.encode_uint32(buffer, ref pos_, itemTransNum);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref guid);
				BaseDLL.decode_uint32(buffer, ref pos_, ref price);
				BaseDLL.decode_int8(buffer, ref pos_, ref pricetype);
				BaseDLL.decode_uint32(buffer, ref pos_, ref num);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemTypeId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref strengthed);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemScore);
				BaseDLL.decode_uint32(buffer, ref pos_, ref publicEndTime);
				BaseDLL.decode_int8(buffer, ref pos_, ref isTreas);
				BaseDLL.decode_uint64(buffer, ref pos_, ref owner);
				BaseDLL.decode_int8(buffer, ref pos_, ref attent);
				BaseDLL.decode_uint32(buffer, ref pos_, ref attentNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref duetime);
				BaseDLL.decode_uint32(buffer, ref pos_, ref onSaleTime);
				BaseDLL.decode_uint32(buffer, ref pos_, ref beadbuffid);
				BaseDLL.decode_int8(buffer, ref pos_, ref isAgainOnsale);
				BaseDLL.decode_int8(buffer, ref pos_, ref equipType);
				BaseDLL.decode_int8(buffer, ref pos_, ref enhanceType);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemDueTime);
				BaseDLL.decode_uint32(buffer, ref pos_, ref enhanceNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemTransNum);
			}

			public int getLen()
			{
				int _len = 0;
				// guid
				_len += 8;
				// price
				_len += 4;
				// pricetype
				_len += 1;
				// num
				_len += 4;
				// itemTypeId
				_len += 4;
				// strengthed
				_len += 4;
				// itemScore
				_len += 4;
				// publicEndTime
				_len += 4;
				// isTreas
				_len += 1;
				// owner
				_len += 8;
				// attent
				_len += 1;
				// attentNum
				_len += 4;
				// duetime
				_len += 4;
				// onSaleTime
				_len += 4;
				// beadbuffid
				_len += 4;
				// isAgainOnsale
				_len += 1;
				// equipType
				_len += 1;
				// enhanceType
				_len += 1;
				// itemDueTime
				_len += 4;
				// enhanceNum
				_len += 4;
				// itemTransNum
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class WorldAuctionListQueryRet : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 603902;
		public UInt32 Sequence;
		public byte type;
		public AuctionBaseInfo[] data = new AuctionBaseInfo[0];
		public UInt32 curPage;
		public UInt32 maxPage;
		/// <summary>
		///  商品状态(AuctionGoodState)
		/// </summary>
		public byte goodState;
		/// <summary>
		///  是否关注[0]非关注,[1]关注,枚举(AuctionAttentType)
		/// </summary>
		public byte attent;

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
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)data.Length);
				for(int i = 0; i < data.Length; i++)
				{
					data[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_uint32(buffer, ref pos_, curPage);
				BaseDLL.encode_uint32(buffer, ref pos_, maxPage);
				BaseDLL.encode_int8(buffer, ref pos_, goodState);
				BaseDLL.encode_int8(buffer, ref pos_, attent);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				UInt16 dataCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref dataCnt);
				data = new AuctionBaseInfo[dataCnt];
				for(int i = 0; i < data.Length; i++)
				{
					data[i] = new AuctionBaseInfo();
					data[i].decode(buffer, ref pos_);
				}
				BaseDLL.decode_uint32(buffer, ref pos_, ref curPage);
				BaseDLL.decode_uint32(buffer, ref pos_, ref maxPage);
				BaseDLL.decode_int8(buffer, ref pos_, ref goodState);
				BaseDLL.decode_int8(buffer, ref pos_, ref attent);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)data.Length);
				for(int i = 0; i < data.Length; i++)
				{
					data[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_uint32(buffer, ref pos_, curPage);
				BaseDLL.encode_uint32(buffer, ref pos_, maxPage);
				BaseDLL.encode_int8(buffer, ref pos_, goodState);
				BaseDLL.encode_int8(buffer, ref pos_, attent);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				UInt16 dataCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref dataCnt);
				data = new AuctionBaseInfo[dataCnt];
				for(int i = 0; i < data.Length; i++)
				{
					data[i] = new AuctionBaseInfo();
					data[i].decode(buffer, ref pos_);
				}
				BaseDLL.decode_uint32(buffer, ref pos_, ref curPage);
				BaseDLL.decode_uint32(buffer, ref pos_, ref maxPage);
				BaseDLL.decode_int8(buffer, ref pos_, ref goodState);
				BaseDLL.decode_int8(buffer, ref pos_, ref attent);
			}

			public int getLen()
			{
				int _len = 0;
				// type
				_len += 1;
				// data
				_len += 2;
				for(int j = 0; j < data.Length; j++)
				{
					_len += data[j].getLen();
				}
				// curPage
				_len += 4;
				// maxPage
				_len += 4;
				// goodState
				_len += 1;
				// attent
				_len += 1;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class WorldAuctionSelfListReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 603904;
		public UInt32 Sequence;
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

	[Protocol]
	public class WorldAuctionSelfListRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 603905;
		public UInt32 Sequence;
		public byte type;
		public AuctionBaseInfo[] data = new AuctionBaseInfo[0];

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
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)data.Length);
				for(int i = 0; i < data.Length; i++)
				{
					data[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				UInt16 dataCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref dataCnt);
				data = new AuctionBaseInfo[dataCnt];
				for(int i = 0; i < data.Length; i++)
				{
					data[i] = new AuctionBaseInfo();
					data[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)data.Length);
				for(int i = 0; i < data.Length; i++)
				{
					data[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				UInt16 dataCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref dataCnt);
				data = new AuctionBaseInfo[dataCnt];
				for(int i = 0; i < data.Length; i++)
				{
					data[i] = new AuctionBaseInfo();
					data[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// type
				_len += 1;
				// data
				_len += 2;
				for(int j = 0; j < data.Length; j++)
				{
					_len += data[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class WorldAuctionRequest : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 603906;
		public UInt32 Sequence;
		/// <summary>
		///  拍卖类型
		/// </summary>
		public byte type;
		/// <summary>
		///  拍卖道具id
		/// </summary>
		public UInt64 id;
		/// <summary>
		///  拍卖道具类型id
		/// </summary>
		public UInt32 typeId;
		/// <summary>
		///  数量
		/// </summary>
		public UInt32 num;
		/// <summary>
		///  价格
		/// </summary>
		public UInt32 price;
		/// <summary>
		///  持续时间(AuctionSellDuration)
		/// </summary>
		public byte duration;
		/// <summary>
		///  强化等级
		/// </summary>
		public byte strength;
		/// <summary>
		///  宝珠buffid
		/// </summary>
		public UInt32 beadbuffId;
		/// <summary>
		///  是否重新上架
		/// </summary>
		public byte isAgain;
		/// <summary>
		///  重新上架guid
		/// </summary>
		public UInt64 auctionGuid;
		/// <summary>
		///  红字装备增幅类型 无效0/力量1/智力2/体力3/精神4
		/// </summary>
		public byte enhanceType;

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
				BaseDLL.encode_uint64(buffer, ref pos_, id);
				BaseDLL.encode_uint32(buffer, ref pos_, typeId);
				BaseDLL.encode_uint32(buffer, ref pos_, num);
				BaseDLL.encode_uint32(buffer, ref pos_, price);
				BaseDLL.encode_int8(buffer, ref pos_, duration);
				BaseDLL.encode_int8(buffer, ref pos_, strength);
				BaseDLL.encode_uint32(buffer, ref pos_, beadbuffId);
				BaseDLL.encode_int8(buffer, ref pos_, isAgain);
				BaseDLL.encode_uint64(buffer, ref pos_, auctionGuid);
				BaseDLL.encode_int8(buffer, ref pos_, enhanceType);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
				BaseDLL.decode_uint32(buffer, ref pos_, ref typeId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref num);
				BaseDLL.decode_uint32(buffer, ref pos_, ref price);
				BaseDLL.decode_int8(buffer, ref pos_, ref duration);
				BaseDLL.decode_int8(buffer, ref pos_, ref strength);
				BaseDLL.decode_uint32(buffer, ref pos_, ref beadbuffId);
				BaseDLL.decode_int8(buffer, ref pos_, ref isAgain);
				BaseDLL.decode_uint64(buffer, ref pos_, ref auctionGuid);
				BaseDLL.decode_int8(buffer, ref pos_, ref enhanceType);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_uint64(buffer, ref pos_, id);
				BaseDLL.encode_uint32(buffer, ref pos_, typeId);
				BaseDLL.encode_uint32(buffer, ref pos_, num);
				BaseDLL.encode_uint32(buffer, ref pos_, price);
				BaseDLL.encode_int8(buffer, ref pos_, duration);
				BaseDLL.encode_int8(buffer, ref pos_, strength);
				BaseDLL.encode_uint32(buffer, ref pos_, beadbuffId);
				BaseDLL.encode_int8(buffer, ref pos_, isAgain);
				BaseDLL.encode_uint64(buffer, ref pos_, auctionGuid);
				BaseDLL.encode_int8(buffer, ref pos_, enhanceType);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
				BaseDLL.decode_uint32(buffer, ref pos_, ref typeId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref num);
				BaseDLL.decode_uint32(buffer, ref pos_, ref price);
				BaseDLL.decode_int8(buffer, ref pos_, ref duration);
				BaseDLL.decode_int8(buffer, ref pos_, ref strength);
				BaseDLL.decode_uint32(buffer, ref pos_, ref beadbuffId);
				BaseDLL.decode_int8(buffer, ref pos_, ref isAgain);
				BaseDLL.decode_uint64(buffer, ref pos_, ref auctionGuid);
				BaseDLL.decode_int8(buffer, ref pos_, ref enhanceType);
			}

			public int getLen()
			{
				int _len = 0;
				// type
				_len += 1;
				// id
				_len += 8;
				// typeId
				_len += 4;
				// num
				_len += 4;
				// price
				_len += 4;
				// duration
				_len += 1;
				// strength
				_len += 1;
				// beadbuffId
				_len += 4;
				// isAgain
				_len += 1;
				// auctionGuid
				_len += 8;
				// enhanceType
				_len += 1;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class WorldAuctionBuy : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 603908;
		public UInt32 Sequence;
		public UInt64 id;
		public UInt32 num;

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
				BaseDLL.encode_uint64(buffer, ref pos_, id);
				BaseDLL.encode_uint32(buffer, ref pos_, num);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
				BaseDLL.decode_uint32(buffer, ref pos_, ref num);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, id);
				BaseDLL.encode_uint32(buffer, ref pos_, num);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
				BaseDLL.decode_uint32(buffer, ref pos_, ref num);
			}

			public int getLen()
			{
				int _len = 0;
				// id
				_len += 8;
				// num
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class WorldAuctionCancel : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 603909;
		public UInt32 Sequence;
		public UInt64 id;

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
				BaseDLL.encode_uint64(buffer, ref pos_, id);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, id);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
			}

			public int getLen()
			{
				int _len = 0;
				// id
				_len += 8;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class WorldAuctionNotifyRefresh : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 603911;
		public UInt32 Sequence;
		/// <summary>
		///  拍卖行类型
		/// </summary>
		public byte type;
		/// <summary>
		///  原因
		/// </summary>
		public byte reason;
		/// <summary>
		///  拍卖行id
		/// </summary>
		public UInt64 auctGuid;

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
				BaseDLL.encode_int8(buffer, ref pos_, reason);
				BaseDLL.encode_uint64(buffer, ref pos_, auctGuid);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_int8(buffer, ref pos_, ref reason);
				BaseDLL.decode_uint64(buffer, ref pos_, ref auctGuid);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_int8(buffer, ref pos_, reason);
				BaseDLL.encode_uint64(buffer, ref pos_, auctGuid);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_int8(buffer, ref pos_, ref reason);
				BaseDLL.decode_uint64(buffer, ref pos_, ref auctGuid);
			}

			public int getLen()
			{
				int _len = 0;
				// type
				_len += 1;
				// reason
				_len += 1;
				// auctGuid
				_len += 8;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class WorldAuctionQueryItemReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 603916;
		public UInt32 Sequence;
		public UInt64 id;

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
				BaseDLL.encode_uint64(buffer, ref pos_, id);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, id);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
			}

			public int getLen()
			{
				int _len = 0;
				// id
				_len += 8;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class WorldAuctionQueryItemRet : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 603917;
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
	public class WorldAuctionRecommendPriceReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 603918;
		public UInt32 Sequence;
		public byte type;
		public UInt32 itemTypeId;

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
				BaseDLL.encode_uint32(buffer, ref pos_, itemTypeId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemTypeId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_uint32(buffer, ref pos_, itemTypeId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemTypeId);
			}

			public int getLen()
			{
				int _len = 0;
				// type
				_len += 1;
				// itemTypeId
				_len += 4;
				return _len;
			}
		#endregion

	}

	[Protocol]
	public class WorldAuctionRecommendPriceRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 603919;
		public UInt32 Sequence;
		public byte type;
		public UInt32 itemTypeId;
		public UInt32 price;

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
				BaseDLL.encode_uint32(buffer, ref pos_, itemTypeId);
				BaseDLL.encode_uint32(buffer, ref pos_, price);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemTypeId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref price);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_uint32(buffer, ref pos_, itemTypeId);
				BaseDLL.encode_uint32(buffer, ref pos_, price);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemTypeId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref price);
			}

			public int getLen()
			{
				int _len = 0;
				// type
				_len += 1;
				// itemTypeId
				_len += 4;
				// price
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  查询拍卖行道具数量
	/// </summary>
	[Protocol]
	public class WorldAuctionItemNumReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 603920;
		public UInt32 Sequence;
		public AuctionQueryCondition cond = new AuctionQueryCondition();

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
				cond.encode(buffer, ref pos_);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				cond.decode(buffer, ref pos_);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				cond.encode(buffer, ref pos_);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				cond.decode(buffer, ref pos_);
			}

			public int getLen()
			{
				int _len = 0;
				// cond
				_len += cond.getLen();
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  返回拍卖行道具数量
	/// </summary>
	[Protocol]
	public class WorldAuctionItemNumRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 603921;
		public UInt32 Sequence;
		/// <summary>
		///  商品状态(AuctionGoodState)
		/// </summary>
		public byte goodState;
		public AuctionItemBaseInfo[] items = new AuctionItemBaseInfo[0];

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
				BaseDLL.encode_int8(buffer, ref pos_, goodState);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)items.Length);
				for(int i = 0; i < items.Length; i++)
				{
					items[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref goodState);
				UInt16 itemsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref itemsCnt);
				items = new AuctionItemBaseInfo[itemsCnt];
				for(int i = 0; i < items.Length; i++)
				{
					items[i] = new AuctionItemBaseInfo();
					items[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, goodState);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)items.Length);
				for(int i = 0; i < items.Length; i++)
				{
					items[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref goodState);
				UInt16 itemsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref itemsCnt);
				items = new AuctionItemBaseInfo[itemsCnt];
				for(int i = 0; i < items.Length; i++)
				{
					items[i] = new AuctionItemBaseInfo();
					items[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// goodState
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
	///  刷新拍卖行时间请求
	/// </summary>
	[Protocol]
	public class SceneAuctionRefreshReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 503901;
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
	///  刷新拍卖行时间返回
	/// </summary>
	[Protocol]
	public class SceneAuctionRefreshRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 503902;
		public UInt32 Sequence;
		/// <summary>
		///  结果
		/// </summary>
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
	///  购买拍卖行栏位请求
	/// </summary>
	[Protocol]
	public class SceneAuctionBuyBoothReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 503903;
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
	///  购买拍卖行栏位返回
	/// </summary>
	[Protocol]
	public class SceneAuctionBuyBoothRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 503904;
		public UInt32 Sequence;
		/// <summary>
		///  结果
		/// </summary>
		public UInt32 result;
		/// <summary>
		///  栏位数
		/// </summary>
		public UInt32 boothNum;

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
				BaseDLL.encode_uint32(buffer, ref pos_, boothNum);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				BaseDLL.decode_uint32(buffer, ref pos_, ref boothNum);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, result);
				BaseDLL.encode_uint32(buffer, ref pos_, boothNum);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref result);
				BaseDLL.decode_uint32(buffer, ref pos_, ref boothNum);
			}

			public int getLen()
			{
				int _len = 0;
				// result
				_len += 4;
				// boothNum
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 查询道具价格情况(近期交易平均价, 其它玩家正在出售价格)
	/// </summary>
	[Protocol]
	public class WorldAuctionQueryItemPricesReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 603922;
		public UInt32 Sequence;
		/// <summary>
		///  拍卖行类型
		/// </summary>
		public byte type;
		/// <summary>
		///  物品类型id
		/// </summary>
		public UInt32 itemTypeId;
		/// <summary>
		///  物品强化等级
		/// </summary>
		public UInt32 strengthen;
		/// <summary>
		///  宝珠附加buffid
		/// </summary>
		public UInt32 beadbuffid;
		/// <summary>
		/// 红字装备增幅类型:无效0/力量1/智力2/体力3/精神4
		/// </summary>
		public byte enhanceType;

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
				BaseDLL.encode_uint32(buffer, ref pos_, itemTypeId);
				BaseDLL.encode_uint32(buffer, ref pos_, strengthen);
				BaseDLL.encode_uint32(buffer, ref pos_, beadbuffid);
				BaseDLL.encode_int8(buffer, ref pos_, enhanceType);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemTypeId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref strengthen);
				BaseDLL.decode_uint32(buffer, ref pos_, ref beadbuffid);
				BaseDLL.decode_int8(buffer, ref pos_, ref enhanceType);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_uint32(buffer, ref pos_, itemTypeId);
				BaseDLL.encode_uint32(buffer, ref pos_, strengthen);
				BaseDLL.encode_uint32(buffer, ref pos_, beadbuffid);
				BaseDLL.encode_int8(buffer, ref pos_, enhanceType);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemTypeId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref strengthen);
				BaseDLL.decode_uint32(buffer, ref pos_, ref beadbuffid);
				BaseDLL.decode_int8(buffer, ref pos_, ref enhanceType);
			}

			public int getLen()
			{
				int _len = 0;
				// type
				_len += 1;
				// itemTypeId
				_len += 4;
				// strengthen
				_len += 4;
				// beadbuffid
				_len += 4;
				// enhanceType
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 道具价格情况返回
	/// </summary>
	[Protocol]
	public class WorldAuctionQueryItemPricesRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 603923;
		public UInt32 Sequence;
		/// <summary>
		///  拍卖行类型
		/// </summary>
		public byte type;
		/// <summary>
		///  物品类型id
		/// </summary>
		public UInt32 itemTypeId;
		/// <summary>
		///  物品强化等级
		/// </summary>
		public UInt32 strengthen;
		/// <summary>
		///  近期平均交易价格
		/// </summary>
		public UInt32 averagePrice;
		/// <summary>
		///  目前在售的价格最低的同样道具
		/// </summary>
		public AuctionBaseInfo[] actionItems = new AuctionBaseInfo[0];
		/// <summary>
		///  近期可见平均交易价格(非珍品)
		/// </summary>
		public UInt32 visAverPrice;
		/// <summary>
		///  鏈�灏忎环鏍?
		/// </summary>
		public UInt32 minPrice;
		/// <summary>
		///  鏈�澶т环鏍?
		/// </summary>
		public UInt32 maxPrice;
		/// <summary>
		///  推荐价格
		/// </summary>
		public UInt32 recommendPrice;

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
				BaseDLL.encode_uint32(buffer, ref pos_, itemTypeId);
				BaseDLL.encode_uint32(buffer, ref pos_, strengthen);
				BaseDLL.encode_uint32(buffer, ref pos_, averagePrice);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)actionItems.Length);
				for(int i = 0; i < actionItems.Length; i++)
				{
					actionItems[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_uint32(buffer, ref pos_, visAverPrice);
				BaseDLL.encode_uint32(buffer, ref pos_, minPrice);
				BaseDLL.encode_uint32(buffer, ref pos_, maxPrice);
				BaseDLL.encode_uint32(buffer, ref pos_, recommendPrice);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemTypeId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref strengthen);
				BaseDLL.decode_uint32(buffer, ref pos_, ref averagePrice);
				UInt16 actionItemsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref actionItemsCnt);
				actionItems = new AuctionBaseInfo[actionItemsCnt];
				for(int i = 0; i < actionItems.Length; i++)
				{
					actionItems[i] = new AuctionBaseInfo();
					actionItems[i].decode(buffer, ref pos_);
				}
				BaseDLL.decode_uint32(buffer, ref pos_, ref visAverPrice);
				BaseDLL.decode_uint32(buffer, ref pos_, ref minPrice);
				BaseDLL.decode_uint32(buffer, ref pos_, ref maxPrice);
				BaseDLL.decode_uint32(buffer, ref pos_, ref recommendPrice);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_uint32(buffer, ref pos_, itemTypeId);
				BaseDLL.encode_uint32(buffer, ref pos_, strengthen);
				BaseDLL.encode_uint32(buffer, ref pos_, averagePrice);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)actionItems.Length);
				for(int i = 0; i < actionItems.Length; i++)
				{
					actionItems[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_uint32(buffer, ref pos_, visAverPrice);
				BaseDLL.encode_uint32(buffer, ref pos_, minPrice);
				BaseDLL.encode_uint32(buffer, ref pos_, maxPrice);
				BaseDLL.encode_uint32(buffer, ref pos_, recommendPrice);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemTypeId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref strengthen);
				BaseDLL.decode_uint32(buffer, ref pos_, ref averagePrice);
				UInt16 actionItemsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref actionItemsCnt);
				actionItems = new AuctionBaseInfo[actionItemsCnt];
				for(int i = 0; i < actionItems.Length; i++)
				{
					actionItems[i] = new AuctionBaseInfo();
					actionItems[i].decode(buffer, ref pos_);
				}
				BaseDLL.decode_uint32(buffer, ref pos_, ref visAverPrice);
				BaseDLL.decode_uint32(buffer, ref pos_, ref minPrice);
				BaseDLL.decode_uint32(buffer, ref pos_, ref maxPrice);
				BaseDLL.decode_uint32(buffer, ref pos_, ref recommendPrice);
			}

			public int getLen()
			{
				int _len = 0;
				// type
				_len += 1;
				// itemTypeId
				_len += 4;
				// strengthen
				_len += 4;
				// averagePrice
				_len += 4;
				// actionItems
				_len += 2;
				for(int j = 0; j < actionItems.Length; j++)
				{
					_len += actionItems[j].getLen();
				}
				// visAverPrice
				_len += 4;
				// minPrice
				_len += 4;
				// maxPrice
				_len += 4;
				// recommendPrice
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// scene->client 通知异常交易
	/// </summary>
	[Protocol]
	public class SceneNotifyAbnormalTransaction : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 503905;
		public UInt32 Sequence;
		/// <summary>
		///  bool值(false(0):关闭通知，true(1):开启通知)
		/// </summary>
		public byte bNotify;

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
				BaseDLL.encode_int8(buffer, ref pos_, bNotify);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref bNotify);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, bNotify);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref bNotify);
			}

			public int getLen()
			{
				int _len = 0;
				// bNotify
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// client->scene 异常交易记录查询
	/// </summary>
	[Protocol]
	public class SceneAbnormalTransactionRecordReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 503906;
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
	/// scene->client 异常交易记录返回
	/// </summary>
	[Protocol]
	public class SceneAbnormalTransactionRecordRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 503907;
		public UInt32 Sequence;
		/// <summary>
		///  冻结金类型
		/// </summary>
		public UInt32 frozenMoneyType;
		/// <summary>
		///  冻结金额
		/// </summary>
		public UInt32 frozenAmount;
		/// <summary>
		///  异常交易时间
		/// </summary>
		public UInt32 abnormalTransactionTime;
		/// <summary>
		///  返还期
		/// </summary>
		public UInt32 backDeadline;
		/// <summary>
		///  已返还金额
		/// </summary>
		public UInt32 backAmount;
		/// <summary>
		///  已返还日
		/// </summary>
		public UInt32 backDays;

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
				BaseDLL.encode_uint32(buffer, ref pos_, frozenMoneyType);
				BaseDLL.encode_uint32(buffer, ref pos_, frozenAmount);
				BaseDLL.encode_uint32(buffer, ref pos_, abnormalTransactionTime);
				BaseDLL.encode_uint32(buffer, ref pos_, backDeadline);
				BaseDLL.encode_uint32(buffer, ref pos_, backAmount);
				BaseDLL.encode_uint32(buffer, ref pos_, backDays);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref frozenMoneyType);
				BaseDLL.decode_uint32(buffer, ref pos_, ref frozenAmount);
				BaseDLL.decode_uint32(buffer, ref pos_, ref abnormalTransactionTime);
				BaseDLL.decode_uint32(buffer, ref pos_, ref backDeadline);
				BaseDLL.decode_uint32(buffer, ref pos_, ref backAmount);
				BaseDLL.decode_uint32(buffer, ref pos_, ref backDays);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, frozenMoneyType);
				BaseDLL.encode_uint32(buffer, ref pos_, frozenAmount);
				BaseDLL.encode_uint32(buffer, ref pos_, abnormalTransactionTime);
				BaseDLL.encode_uint32(buffer, ref pos_, backDeadline);
				BaseDLL.encode_uint32(buffer, ref pos_, backAmount);
				BaseDLL.encode_uint32(buffer, ref pos_, backDays);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref frozenMoneyType);
				BaseDLL.decode_uint32(buffer, ref pos_, ref frozenAmount);
				BaseDLL.decode_uint32(buffer, ref pos_, ref abnormalTransactionTime);
				BaseDLL.decode_uint32(buffer, ref pos_, ref backDeadline);
				BaseDLL.decode_uint32(buffer, ref pos_, ref backAmount);
				BaseDLL.decode_uint32(buffer, ref pos_, ref backDays);
			}

			public int getLen()
			{
				int _len = 0;
				// frozenMoneyType
				_len += 4;
				// frozenAmount
				_len += 4;
				// abnormalTransactionTime
				_len += 4;
				// backDeadline
				_len += 4;
				// backAmount
				_len += 4;
				// backDays
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// 拍卖行交易记录
	/// </summary>
	public class AuctionTransaction : Protocol.IProtocolStream
	{
		public byte type;
		public UInt32 item_id;
		public UInt32 item_num;
		public UInt32 item_score;
		public UInt32 unit_price;
		public UInt32 verify_end_time;
		public byte enhance_type;
		public byte strength;
		public byte qualitylv;
		public UInt32 trans_time;
		public UInt32 beadbuffId;
		public UInt32 mountBeadId;
		public UInt32 mountBeadBuffId;
		public UInt32 mountMagicCardId;
		public byte mountMagicCardLv;
		public byte equipType;
		public UInt32 enhanceNum;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_uint32(buffer, ref pos_, item_id);
				BaseDLL.encode_uint32(buffer, ref pos_, item_num);
				BaseDLL.encode_uint32(buffer, ref pos_, item_score);
				BaseDLL.encode_uint32(buffer, ref pos_, unit_price);
				BaseDLL.encode_uint32(buffer, ref pos_, verify_end_time);
				BaseDLL.encode_int8(buffer, ref pos_, enhance_type);
				BaseDLL.encode_int8(buffer, ref pos_, strength);
				BaseDLL.encode_int8(buffer, ref pos_, qualitylv);
				BaseDLL.encode_uint32(buffer, ref pos_, trans_time);
				BaseDLL.encode_uint32(buffer, ref pos_, beadbuffId);
				BaseDLL.encode_uint32(buffer, ref pos_, mountBeadId);
				BaseDLL.encode_uint32(buffer, ref pos_, mountBeadBuffId);
				BaseDLL.encode_uint32(buffer, ref pos_, mountMagicCardId);
				BaseDLL.encode_int8(buffer, ref pos_, mountMagicCardLv);
				BaseDLL.encode_int8(buffer, ref pos_, equipType);
				BaseDLL.encode_uint32(buffer, ref pos_, enhanceNum);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_uint32(buffer, ref pos_, ref item_id);
				BaseDLL.decode_uint32(buffer, ref pos_, ref item_num);
				BaseDLL.decode_uint32(buffer, ref pos_, ref item_score);
				BaseDLL.decode_uint32(buffer, ref pos_, ref unit_price);
				BaseDLL.decode_uint32(buffer, ref pos_, ref verify_end_time);
				BaseDLL.decode_int8(buffer, ref pos_, ref enhance_type);
				BaseDLL.decode_int8(buffer, ref pos_, ref strength);
				BaseDLL.decode_int8(buffer, ref pos_, ref qualitylv);
				BaseDLL.decode_uint32(buffer, ref pos_, ref trans_time);
				BaseDLL.decode_uint32(buffer, ref pos_, ref beadbuffId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref mountBeadId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref mountBeadBuffId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref mountMagicCardId);
				BaseDLL.decode_int8(buffer, ref pos_, ref mountMagicCardLv);
				BaseDLL.decode_int8(buffer, ref pos_, ref equipType);
				BaseDLL.decode_uint32(buffer, ref pos_, ref enhanceNum);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_uint32(buffer, ref pos_, item_id);
				BaseDLL.encode_uint32(buffer, ref pos_, item_num);
				BaseDLL.encode_uint32(buffer, ref pos_, item_score);
				BaseDLL.encode_uint32(buffer, ref pos_, unit_price);
				BaseDLL.encode_uint32(buffer, ref pos_, verify_end_time);
				BaseDLL.encode_int8(buffer, ref pos_, enhance_type);
				BaseDLL.encode_int8(buffer, ref pos_, strength);
				BaseDLL.encode_int8(buffer, ref pos_, qualitylv);
				BaseDLL.encode_uint32(buffer, ref pos_, trans_time);
				BaseDLL.encode_uint32(buffer, ref pos_, beadbuffId);
				BaseDLL.encode_uint32(buffer, ref pos_, mountBeadId);
				BaseDLL.encode_uint32(buffer, ref pos_, mountBeadBuffId);
				BaseDLL.encode_uint32(buffer, ref pos_, mountMagicCardId);
				BaseDLL.encode_int8(buffer, ref pos_, mountMagicCardLv);
				BaseDLL.encode_int8(buffer, ref pos_, equipType);
				BaseDLL.encode_uint32(buffer, ref pos_, enhanceNum);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_uint32(buffer, ref pos_, ref item_id);
				BaseDLL.decode_uint32(buffer, ref pos_, ref item_num);
				BaseDLL.decode_uint32(buffer, ref pos_, ref item_score);
				BaseDLL.decode_uint32(buffer, ref pos_, ref unit_price);
				BaseDLL.decode_uint32(buffer, ref pos_, ref verify_end_time);
				BaseDLL.decode_int8(buffer, ref pos_, ref enhance_type);
				BaseDLL.decode_int8(buffer, ref pos_, ref strength);
				BaseDLL.decode_int8(buffer, ref pos_, ref qualitylv);
				BaseDLL.decode_uint32(buffer, ref pos_, ref trans_time);
				BaseDLL.decode_uint32(buffer, ref pos_, ref beadbuffId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref mountBeadId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref mountBeadBuffId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref mountMagicCardId);
				BaseDLL.decode_int8(buffer, ref pos_, ref mountMagicCardLv);
				BaseDLL.decode_int8(buffer, ref pos_, ref equipType);
				BaseDLL.decode_uint32(buffer, ref pos_, ref enhanceNum);
			}

			public int getLen()
			{
				int _len = 0;
				// type
				_len += 1;
				// item_id
				_len += 4;
				// item_num
				_len += 4;
				// item_score
				_len += 4;
				// unit_price
				_len += 4;
				// verify_end_time
				_len += 4;
				// enhance_type
				_len += 1;
				// strength
				_len += 1;
				// qualitylv
				_len += 1;
				// trans_time
				_len += 4;
				// beadbuffId
				_len += 4;
				// mountBeadId
				_len += 4;
				// mountBeadBuffId
				_len += 4;
				// mountMagicCardId
				_len += 4;
				// mountMagicCardLv
				_len += 1;
				// equipType
				_len += 1;
				// enhanceNum
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// client->world 拉取珍品交易记录
	/// </summary>
	[Protocol]
	public class WorldAuctionGetTreasTransactionReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 603924;
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
	/// world->client 拉取珍品交易记录返回
	/// </summary>
	[Protocol]
	public class WorldAuctionGetTreasTransactionRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 603925;
		public UInt32 Sequence;
		/// <summary>
		/// 寄售列表
		/// </summary>
		public AuctionTransaction[] sales = new AuctionTransaction[0];
		/// <summary>
		/// 购买列表
		/// </summary>
		public AuctionTransaction[] buys = new AuctionTransaction[0];

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
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)sales.Length);
				for(int i = 0; i < sales.Length; i++)
				{
					sales[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)buys.Length);
				for(int i = 0; i < buys.Length; i++)
				{
					buys[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 salesCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref salesCnt);
				sales = new AuctionTransaction[salesCnt];
				for(int i = 0; i < sales.Length; i++)
				{
					sales[i] = new AuctionTransaction();
					sales[i].decode(buffer, ref pos_);
				}
				UInt16 buysCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref buysCnt);
				buys = new AuctionTransaction[buysCnt];
				for(int i = 0; i < buys.Length; i++)
				{
					buys[i] = new AuctionTransaction();
					buys[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)sales.Length);
				for(int i = 0; i < sales.Length; i++)
				{
					sales[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)buys.Length);
				for(int i = 0; i < buys.Length; i++)
				{
					buys[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 salesCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref salesCnt);
				sales = new AuctionTransaction[salesCnt];
				for(int i = 0; i < sales.Length; i++)
				{
					sales[i] = new AuctionTransaction();
					sales[i].decode(buffer, ref pos_);
				}
				UInt16 buysCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref buysCnt);
				buys = new AuctionTransaction[buysCnt];
				for(int i = 0; i < buys.Length; i++)
				{
					buys[i] = new AuctionTransaction();
					buys[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// sales
				_len += 2;
				for(int j = 0; j < sales.Length; j++)
				{
					_len += sales[j].getLen();
				}
				// buys
				_len += 2;
				for(int j = 0; j < buys.Length; j++)
				{
					_len += buys[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// client->world 抢购购买拍卖道具
	/// </summary>
	[Protocol]
	public class WorldAuctionRusyBuy : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 603926;
		public UInt32 Sequence;
		public UInt64 id;
		public UInt32 num;

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
				BaseDLL.encode_uint64(buffer, ref pos_, id);
				BaseDLL.encode_uint32(buffer, ref pos_, num);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
				BaseDLL.decode_uint32(buffer, ref pos_, ref num);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, id);
				BaseDLL.encode_uint32(buffer, ref pos_, num);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref id);
				BaseDLL.decode_uint32(buffer, ref pos_, ref num);
			}

			public int getLen()
			{
				int _len = 0;
				// id
				_len += 8;
				// num
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// world->client
	/// </summary>
	[Protocol]
	public class WorldAuctionSyncPubPageIds : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 603928;
		public UInt32 Sequence;
		/// <summary>
		/// 拍卖行结构表id
		/// </summary>
		public UInt32[] pageIds = new UInt32[0];
		/// <summary>
		/// 拍卖行屏蔽道具列表
		/// </summary>
		public UInt32[] shieldItemList = new UInt32[0];

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
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)pageIds.Length);
				for(int i = 0; i < pageIds.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, pageIds[i]);
				}
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)shieldItemList.Length);
				for(int i = 0; i < shieldItemList.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, shieldItemList[i]);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				UInt16 pageIdsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref pageIdsCnt);
				pageIds = new UInt32[pageIdsCnt];
				for(int i = 0; i < pageIds.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref pageIds[i]);
				}
				UInt16 shieldItemListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref shieldItemListCnt);
				shieldItemList = new UInt32[shieldItemListCnt];
				for(int i = 0; i < shieldItemList.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref shieldItemList[i]);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)pageIds.Length);
				for(int i = 0; i < pageIds.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, pageIds[i]);
				}
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)shieldItemList.Length);
				for(int i = 0; i < shieldItemList.Length; i++)
				{
					BaseDLL.encode_uint32(buffer, ref pos_, shieldItemList[i]);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				UInt16 pageIdsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref pageIdsCnt);
				pageIds = new UInt32[pageIdsCnt];
				for(int i = 0; i < pageIds.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref pageIds[i]);
				}
				UInt16 shieldItemListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref shieldItemListCnt);
				shieldItemList = new UInt32[shieldItemListCnt];
				for(int i = 0; i < shieldItemList.Length; i++)
				{
					BaseDLL.decode_uint32(buffer, ref pos_, ref shieldItemList[i]);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// pageIds
				_len += 2 + 4 * pageIds.Length;
				// shieldItemList
				_len += 2 + 4 * shieldItemList.Length;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// client->world 关注
	/// </summary>
	[Protocol]
	public class WorldAuctionAttentReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 603929;
		public UInt32 Sequence;
		/// <summary>
		/// 拍卖行商品id
		/// </summary>
		public UInt64 autionGuid;

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
				BaseDLL.encode_uint64(buffer, ref pos_, autionGuid);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref autionGuid);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint64(buffer, ref pos_, autionGuid);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint64(buffer, ref pos_, ref autionGuid);
			}

			public int getLen()
			{
				int _len = 0;
				// autionGuid
				_len += 8;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// world->client 关注返回
	/// </summary>
	[Protocol]
	public class WorldActionAttentRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 603930;
		public UInt32 Sequence;
		/// <summary>
		/// 结果
		/// </summary>
		public UInt32 code;
		/// <summary>
		/// 拍卖行物品信息
		/// </summary>
		public AuctionBaseInfo aution = new AuctionBaseInfo();

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
				aution.encode(buffer, ref pos_);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
				aution.decode(buffer, ref pos_);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
				aution.encode(buffer, ref pos_);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
				aution.decode(buffer, ref pos_);
			}

			public int getLen()
			{
				int _len = 0;
				// code
				_len += 4;
				// aution
				_len += aution.getLen();
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// client->world 查询道具价格情况(最低出售价格列表)
	/// </summary>
	[Protocol]
	public class WorldAuctionQueryItemPriceListReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 603931;
		public UInt32 Sequence;
		/// <summary>
		///  拍卖行类型
		/// </summary>
		public byte type;
		/// <summary>
		///  物品出售状态 [1]:公示 [2]:上架
		/// </summary>
		public byte auctionState;
		/// <summary>
		///  物品类型id
		/// </summary>
		public UInt32 itemTypeId;
		/// <summary>
		///  物品强化等级
		/// </summary>
		public UInt32 strengthen;
		/// <summary>
		/// 红字装备增幅类型:无效0/力量1/智力2/体力3/精神4
		/// </summary>
		public byte enhanceType;

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
				BaseDLL.encode_int8(buffer, ref pos_, auctionState);
				BaseDLL.encode_uint32(buffer, ref pos_, itemTypeId);
				BaseDLL.encode_uint32(buffer, ref pos_, strengthen);
				BaseDLL.encode_int8(buffer, ref pos_, enhanceType);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_int8(buffer, ref pos_, ref auctionState);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemTypeId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref strengthen);
				BaseDLL.decode_int8(buffer, ref pos_, ref enhanceType);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_int8(buffer, ref pos_, auctionState);
				BaseDLL.encode_uint32(buffer, ref pos_, itemTypeId);
				BaseDLL.encode_uint32(buffer, ref pos_, strengthen);
				BaseDLL.encode_int8(buffer, ref pos_, enhanceType);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_int8(buffer, ref pos_, ref auctionState);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemTypeId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref strengthen);
				BaseDLL.decode_int8(buffer, ref pos_, ref enhanceType);
			}

			public int getLen()
			{
				int _len = 0;
				// type
				_len += 1;
				// auctionState
				_len += 1;
				// itemTypeId
				_len += 4;
				// strengthen
				_len += 4;
				// enhanceType
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// world->client 道具价格情况返回
	/// </summary>
	[Protocol]
	public class WorldAuctionQueryItemPriceListRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 603932;
		public UInt32 Sequence;
		/// <summary>
		///  拍卖行类型
		/// </summary>
		public byte type;
		/// <summary>
		///  物品出售状态 [1]:公示 [2]:上架
		/// </summary>
		public byte auctionState;
		/// <summary>
		///  物品类型id
		/// </summary>
		public UInt32 itemTypeId;
		/// <summary>
		///  物品强化等级
		/// </summary>
		public UInt32 strengthen;
		/// <summary>
		///  目前在售的价格最低的同样道具
		/// </summary>
		public AuctionBaseInfo[] actionItems = new AuctionBaseInfo[0];

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
				BaseDLL.encode_int8(buffer, ref pos_, auctionState);
				BaseDLL.encode_uint32(buffer, ref pos_, itemTypeId);
				BaseDLL.encode_uint32(buffer, ref pos_, strengthen);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)actionItems.Length);
				for(int i = 0; i < actionItems.Length; i++)
				{
					actionItems[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_int8(buffer, ref pos_, ref auctionState);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemTypeId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref strengthen);
				UInt16 actionItemsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref actionItemsCnt);
				actionItems = new AuctionBaseInfo[actionItemsCnt];
				for(int i = 0; i < actionItems.Length; i++)
				{
					actionItems[i] = new AuctionBaseInfo();
					actionItems[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, type);
				BaseDLL.encode_int8(buffer, ref pos_, auctionState);
				BaseDLL.encode_uint32(buffer, ref pos_, itemTypeId);
				BaseDLL.encode_uint32(buffer, ref pos_, strengthen);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)actionItems.Length);
				for(int i = 0; i < actionItems.Length; i++)
				{
					actionItems[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref type);
				BaseDLL.decode_int8(buffer, ref pos_, ref auctionState);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemTypeId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref strengthen);
				UInt16 actionItemsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref actionItemsCnt);
				actionItems = new AuctionBaseInfo[actionItemsCnt];
				for(int i = 0; i < actionItems.Length; i++)
				{
					actionItems[i] = new AuctionBaseInfo();
					actionItems[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// type
				_len += 1;
				// auctionState
				_len += 1;
				// itemTypeId
				_len += 4;
				// strengthen
				_len += 4;
				// actionItems
				_len += 2;
				for(int j = 0; j < actionItems.Length; j++)
				{
					_len += actionItems[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// client->world 查询拍卖行指定附魔卡各等级数量请求
	/// </summary>
	[Protocol]
	public class WorldAuctionQueryMagicOnsalesReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 603933;
		public UInt32 Sequence;
		public UInt32 itemTypeId;

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
				BaseDLL.encode_uint32(buffer, ref pos_, itemTypeId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemTypeId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, itemTypeId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemTypeId);
			}

			public int getLen()
			{
				int _len = 0;
				// itemTypeId
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  寄存物品
	/// </summary>
	public class AuctionMagicOnsale : Protocol.IProtocolStream
	{
		/// <summary>
		///  等级
		/// </summary>
		public byte strength;
		/// <summary>
		///  数量
		/// </summary>
		public UInt32 num;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, strength);
				BaseDLL.encode_uint32(buffer, ref pos_, num);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref strength);
				BaseDLL.decode_uint32(buffer, ref pos_, ref num);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_int8(buffer, ref pos_, strength);
				BaseDLL.encode_uint32(buffer, ref pos_, num);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref strength);
				BaseDLL.decode_uint32(buffer, ref pos_, ref num);
			}

			public int getLen()
			{
				int _len = 0;
				// strength
				_len += 1;
				// num
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// world->client 
	/// </summary>
	[Protocol]
	public class WorldAuctionQueryMagicOnsalesRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 603934;
		public UInt32 Sequence;
		public UInt32 code;
		public AuctionMagicOnsale[] magicOnsales = new AuctionMagicOnsale[0];

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
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)magicOnsales.Length);
				for(int i = 0; i < magicOnsales.Length; i++)
				{
					magicOnsales[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
				UInt16 magicOnsalesCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref magicOnsalesCnt);
				magicOnsales = new AuctionMagicOnsale[magicOnsalesCnt];
				for(int i = 0; i < magicOnsales.Length; i++)
				{
					magicOnsales[i] = new AuctionMagicOnsale();
					magicOnsales[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)magicOnsales.Length);
				for(int i = 0; i < magicOnsales.Length; i++)
				{
					magicOnsales[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
				UInt16 magicOnsalesCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref magicOnsalesCnt);
				magicOnsales = new AuctionMagicOnsale[magicOnsalesCnt];
				for(int i = 0; i < magicOnsales.Length; i++)
				{
					magicOnsales[i] = new AuctionMagicOnsale();
					magicOnsales[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// code
				_len += 4;
				// magicOnsales
				_len += 2;
				for(int j = 0; j < magicOnsales.Length; j++)
				{
					_len += magicOnsales[j].getLen();
				}
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// client->world 查询道具拍卖行交易记录请求
	/// </summary>
	[Protocol]
	public class WorldAuctionQueryItemTransListReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 603935;
		public UInt32 Sequence;
		public UInt32 itemTypeId;
		public UInt32 strengthen;
		public byte enhanceType;
		public UInt32 beadBuffId;

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
				BaseDLL.encode_uint32(buffer, ref pos_, itemTypeId);
				BaseDLL.encode_uint32(buffer, ref pos_, strengthen);
				BaseDLL.encode_int8(buffer, ref pos_, enhanceType);
				BaseDLL.encode_uint32(buffer, ref pos_, beadBuffId);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemTypeId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref strengthen);
				BaseDLL.decode_int8(buffer, ref pos_, ref enhanceType);
				BaseDLL.decode_uint32(buffer, ref pos_, ref beadBuffId);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, itemTypeId);
				BaseDLL.encode_uint32(buffer, ref pos_, strengthen);
				BaseDLL.encode_int8(buffer, ref pos_, enhanceType);
				BaseDLL.encode_uint32(buffer, ref pos_, beadBuffId);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemTypeId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref strengthen);
				BaseDLL.decode_int8(buffer, ref pos_, ref enhanceType);
				BaseDLL.decode_uint32(buffer, ref pos_, ref beadBuffId);
			}

			public int getLen()
			{
				int _len = 0;
				// itemTypeId
				_len += 4;
				// strengthen
				_len += 4;
				// enhanceType
				_len += 1;
				// beadBuffId
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	/// world->client  查询道具拍卖行交易记录返回
	/// </summary>
	[Protocol]
	public class WorldAuctionQueryItemTransListRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 603936;
		public UInt32 Sequence;
		public UInt32 code;
		public UInt32 itemTypeId;
		public UInt32 strengthen;
		public byte enhanceType;
		public UInt32 beadBuffId;
		public AuctionTransaction[] transList = new AuctionTransaction[0];

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
				BaseDLL.encode_uint32(buffer, ref pos_, itemTypeId);
				BaseDLL.encode_uint32(buffer, ref pos_, strengthen);
				BaseDLL.encode_int8(buffer, ref pos_, enhanceType);
				BaseDLL.encode_uint32(buffer, ref pos_, beadBuffId);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)transList.Length);
				for(int i = 0; i < transList.Length; i++)
				{
					transList[i].encode(buffer, ref pos_);
				}
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemTypeId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref strengthen);
				BaseDLL.decode_int8(buffer, ref pos_, ref enhanceType);
				BaseDLL.decode_uint32(buffer, ref pos_, ref beadBuffId);
				UInt16 transListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref transListCnt);
				transList = new AuctionTransaction[transListCnt];
				for(int i = 0; i < transList.Length; i++)
				{
					transList[i] = new AuctionTransaction();
					transList[i].decode(buffer, ref pos_);
				}
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, code);
				BaseDLL.encode_uint32(buffer, ref pos_, itemTypeId);
				BaseDLL.encode_uint32(buffer, ref pos_, strengthen);
				BaseDLL.encode_int8(buffer, ref pos_, enhanceType);
				BaseDLL.encode_uint32(buffer, ref pos_, beadBuffId);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)transList.Length);
				for(int i = 0; i < transList.Length; i++)
				{
					transList[i].encode(buffer, ref pos_);
				}
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref code);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemTypeId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref strengthen);
				BaseDLL.decode_int8(buffer, ref pos_, ref enhanceType);
				BaseDLL.decode_uint32(buffer, ref pos_, ref beadBuffId);
				UInt16 transListCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref transListCnt);
				transList = new AuctionTransaction[transListCnt];
				for(int i = 0; i < transList.Length; i++)
				{
					transList[i] = new AuctionTransaction();
					transList[i].decode(buffer, ref pos_);
				}
			}

			public int getLen()
			{
				int _len = 0;
				// code
				_len += 4;
				// itemTypeId
				_len += 4;
				// strengthen
				_len += 4;
				// enhanceType
				_len += 1;
				// beadBuffId
				_len += 4;
				// transList
				_len += 2;
				for(int j = 0; j < transList.Length; j++)
				{
					_len += transList[j].getLen();
				}
				return _len;
			}
		#endregion

	}

}
