using System;
using System.Text;

namespace Protocol
{
	/// <summary>
	///  账号商店查询索引
	/// </summary>
	public class AccountShopQueryIndex : Protocol.IProtocolStream
	{
		/// <summary>
		///  查询的商店id
		/// </summary>
		public UInt32 shopId;
		/// <summary>
		///  查询的页签类别
		/// </summary>
		public byte tabType;
		/// <summary>
		///  查询的职业类别
		/// </summary>
		public byte jobType;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, shopId);
				BaseDLL.encode_int8(buffer, ref pos_, tabType);
				BaseDLL.encode_int8(buffer, ref pos_, jobType);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref shopId);
				BaseDLL.decode_int8(buffer, ref pos_, ref tabType);
				BaseDLL.decode_int8(buffer, ref pos_, ref jobType);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, shopId);
				BaseDLL.encode_int8(buffer, ref pos_, tabType);
				BaseDLL.encode_int8(buffer, ref pos_, jobType);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref shopId);
				BaseDLL.decode_int8(buffer, ref pos_, ref tabType);
				BaseDLL.decode_int8(buffer, ref pos_, ref jobType);
			}

			public int getLen()
			{
				int _len = 0;
				// shopId
				_len += 4;
				// tabType
				_len += 1;
				// jobType
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  账号商店商品信息
	/// </summary>
	public class AccountShopItemInfo : Protocol.IProtocolStream
	{
		/// <summary>
		///  所属商店id
		/// </summary>
		public UInt32 shopId;
		/// <summary>
		///  商品id
		/// </summary>
		public UInt32 shopItemId;
		/// <summary>
		///  商品名称
		/// </summary>
		public string shopItemName;
		/// <summary>
		///  页签类别
		/// </summary>
		public byte tabType;
		/// <summary>
		///  职业类别
		/// </summary>
		public byte jobType;
		/// <summary>
		///  上架道具
		/// </summary>
		public UInt32 itemDataId;
		/// <summary>
		///  上架数量
		/// </summary>
		public UInt32 itemNum;
		/// <summary>
		///  购买消耗道具
		/// </summary>
		public ItemReward[] costItems = new ItemReward[0];
		/// <summary>
		///  账号刷新类型 RefreshType
		/// </summary>
		public byte accountRefreshType;
		/// <summary>
		///  账号刷新时间点
		/// </summary>
		public string accountRefreshTimePoint;
		/// <summary>
		///  账号限购数量
		/// </summary>
		public UInt32 accountLimitBuyNum;
		/// <summary>
		///  账号剩余购买数量
		/// </summary>
		public UInt32 accountRestBuyNum;
		/// <summary>
		///  账号购买记录下次刷新时间
		/// </summary>
		public UInt32 accountBuyRecordNextRefreshTimestamp;
		/// <summary>
		///  角色刷新类型 RefreshType
		/// </summary>
		public byte roleRefreshType;
		/// <summary>
		///  角色刷新时间点
		/// </summary>
		public string roleRefreshTimePoint;
		/// <summary>
		///  角色购买记录下次刷新时间
		/// </summary>
		public UInt32 roleBuyRecordNextRefreshTimestamp;
		/// <summary>
		///  角色限购数量
		/// </summary>
		public UInt32 roleLimitBuyNum;
		/// <summary>
		///  角色剩余购买数量
		/// </summary>
		public UInt32 roleRestBuyNum;
		/// <summary>
		///  扩展条件
		/// </summary>
		public UInt32 extensibleCondition;
		/// <summary>
		///  是否需要预览按钮
		/// </summary>
		public byte needPreviewFunc;

		#region METHOD


			public void encode(byte[] buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, shopId);
				BaseDLL.encode_uint32(buffer, ref pos_, shopItemId);
				byte[] shopItemNameBytes = StringHelper.StringToUTF8Bytes(shopItemName);
				BaseDLL.encode_string(buffer, ref pos_, shopItemNameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, tabType);
				BaseDLL.encode_int8(buffer, ref pos_, jobType);
				BaseDLL.encode_uint32(buffer, ref pos_, itemDataId);
				BaseDLL.encode_uint32(buffer, ref pos_, itemNum);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)costItems.Length);
				for(int i = 0; i < costItems.Length; i++)
				{
					costItems[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_int8(buffer, ref pos_, accountRefreshType);
				byte[] accountRefreshTimePointBytes = StringHelper.StringToUTF8Bytes(accountRefreshTimePoint);
				BaseDLL.encode_string(buffer, ref pos_, accountRefreshTimePointBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint32(buffer, ref pos_, accountLimitBuyNum);
				BaseDLL.encode_uint32(buffer, ref pos_, accountRestBuyNum);
				BaseDLL.encode_uint32(buffer, ref pos_, accountBuyRecordNextRefreshTimestamp);
				BaseDLL.encode_int8(buffer, ref pos_, roleRefreshType);
				byte[] roleRefreshTimePointBytes = StringHelper.StringToUTF8Bytes(roleRefreshTimePoint);
				BaseDLL.encode_string(buffer, ref pos_, roleRefreshTimePointBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint32(buffer, ref pos_, roleBuyRecordNextRefreshTimestamp);
				BaseDLL.encode_uint32(buffer, ref pos_, roleLimitBuyNum);
				BaseDLL.encode_uint32(buffer, ref pos_, roleRestBuyNum);
				BaseDLL.encode_uint32(buffer, ref pos_, extensibleCondition);
				BaseDLL.encode_int8(buffer, ref pos_, needPreviewFunc);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref shopId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref shopItemId);
				UInt16 shopItemNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref shopItemNameLen);
				byte[] shopItemNameBytes = new byte[shopItemNameLen];
				for(int i = 0; i < shopItemNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref shopItemNameBytes[i]);
				}
				shopItemName = StringHelper.BytesToString(shopItemNameBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref tabType);
				BaseDLL.decode_int8(buffer, ref pos_, ref jobType);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemDataId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemNum);
				UInt16 costItemsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref costItemsCnt);
				costItems = new ItemReward[costItemsCnt];
				for(int i = 0; i < costItems.Length; i++)
				{
					costItems[i] = new ItemReward();
					costItems[i].decode(buffer, ref pos_);
				}
				BaseDLL.decode_int8(buffer, ref pos_, ref accountRefreshType);
				UInt16 accountRefreshTimePointLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref accountRefreshTimePointLen);
				byte[] accountRefreshTimePointBytes = new byte[accountRefreshTimePointLen];
				for(int i = 0; i < accountRefreshTimePointLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref accountRefreshTimePointBytes[i]);
				}
				accountRefreshTimePoint = StringHelper.BytesToString(accountRefreshTimePointBytes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref accountLimitBuyNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref accountRestBuyNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref accountBuyRecordNextRefreshTimestamp);
				BaseDLL.decode_int8(buffer, ref pos_, ref roleRefreshType);
				UInt16 roleRefreshTimePointLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref roleRefreshTimePointLen);
				byte[] roleRefreshTimePointBytes = new byte[roleRefreshTimePointLen];
				for(int i = 0; i < roleRefreshTimePointLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref roleRefreshTimePointBytes[i]);
				}
				roleRefreshTimePoint = StringHelper.BytesToString(roleRefreshTimePointBytes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref roleBuyRecordNextRefreshTimestamp);
				BaseDLL.decode_uint32(buffer, ref pos_, ref roleLimitBuyNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref roleRestBuyNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref extensibleCondition);
				BaseDLL.decode_int8(buffer, ref pos_, ref needPreviewFunc);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, shopId);
				BaseDLL.encode_uint32(buffer, ref pos_, shopItemId);
				byte[] shopItemNameBytes = StringHelper.StringToUTF8Bytes(shopItemName);
				BaseDLL.encode_string(buffer, ref pos_, shopItemNameBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_int8(buffer, ref pos_, tabType);
				BaseDLL.encode_int8(buffer, ref pos_, jobType);
				BaseDLL.encode_uint32(buffer, ref pos_, itemDataId);
				BaseDLL.encode_uint32(buffer, ref pos_, itemNum);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)costItems.Length);
				for(int i = 0; i < costItems.Length; i++)
				{
					costItems[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_int8(buffer, ref pos_, accountRefreshType);
				byte[] accountRefreshTimePointBytes = StringHelper.StringToUTF8Bytes(accountRefreshTimePoint);
				BaseDLL.encode_string(buffer, ref pos_, accountRefreshTimePointBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint32(buffer, ref pos_, accountLimitBuyNum);
				BaseDLL.encode_uint32(buffer, ref pos_, accountRestBuyNum);
				BaseDLL.encode_uint32(buffer, ref pos_, accountBuyRecordNextRefreshTimestamp);
				BaseDLL.encode_int8(buffer, ref pos_, roleRefreshType);
				byte[] roleRefreshTimePointBytes = StringHelper.StringToUTF8Bytes(roleRefreshTimePoint);
				BaseDLL.encode_string(buffer, ref pos_, roleRefreshTimePointBytes, (UInt16)(buffer.Length - pos_));
				BaseDLL.encode_uint32(buffer, ref pos_, roleBuyRecordNextRefreshTimestamp);
				BaseDLL.encode_uint32(buffer, ref pos_, roleLimitBuyNum);
				BaseDLL.encode_uint32(buffer, ref pos_, roleRestBuyNum);
				BaseDLL.encode_uint32(buffer, ref pos_, extensibleCondition);
				BaseDLL.encode_int8(buffer, ref pos_, needPreviewFunc);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref shopId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref shopItemId);
				UInt16 shopItemNameLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref shopItemNameLen);
				byte[] shopItemNameBytes = new byte[shopItemNameLen];
				for(int i = 0; i < shopItemNameLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref shopItemNameBytes[i]);
				}
				shopItemName = StringHelper.BytesToString(shopItemNameBytes);
				BaseDLL.decode_int8(buffer, ref pos_, ref tabType);
				BaseDLL.decode_int8(buffer, ref pos_, ref jobType);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemDataId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref itemNum);
				UInt16 costItemsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref costItemsCnt);
				costItems = new ItemReward[costItemsCnt];
				for(int i = 0; i < costItems.Length; i++)
				{
					costItems[i] = new ItemReward();
					costItems[i].decode(buffer, ref pos_);
				}
				BaseDLL.decode_int8(buffer, ref pos_, ref accountRefreshType);
				UInt16 accountRefreshTimePointLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref accountRefreshTimePointLen);
				byte[] accountRefreshTimePointBytes = new byte[accountRefreshTimePointLen];
				for(int i = 0; i < accountRefreshTimePointLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref accountRefreshTimePointBytes[i]);
				}
				accountRefreshTimePoint = StringHelper.BytesToString(accountRefreshTimePointBytes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref accountLimitBuyNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref accountRestBuyNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref accountBuyRecordNextRefreshTimestamp);
				BaseDLL.decode_int8(buffer, ref pos_, ref roleRefreshType);
				UInt16 roleRefreshTimePointLen = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref roleRefreshTimePointLen);
				byte[] roleRefreshTimePointBytes = new byte[roleRefreshTimePointLen];
				for(int i = 0; i < roleRefreshTimePointLen; i++)
				{
					BaseDLL.decode_int8(buffer, ref pos_, ref roleRefreshTimePointBytes[i]);
				}
				roleRefreshTimePoint = StringHelper.BytesToString(roleRefreshTimePointBytes);
				BaseDLL.decode_uint32(buffer, ref pos_, ref roleBuyRecordNextRefreshTimestamp);
				BaseDLL.decode_uint32(buffer, ref pos_, ref roleLimitBuyNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref roleRestBuyNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref extensibleCondition);
				BaseDLL.decode_int8(buffer, ref pos_, ref needPreviewFunc);
			}

			public int getLen()
			{
				int _len = 0;
				// shopId
				_len += 4;
				// shopItemId
				_len += 4;
				// shopItemName
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(shopItemName);
					_len += 2 + _strBytes.Length;
				}
				// tabType
				_len += 1;
				// jobType
				_len += 1;
				// itemDataId
				_len += 4;
				// itemNum
				_len += 4;
				// costItems
				_len += 2;
				for(int j = 0; j < costItems.Length; j++)
				{
					_len += costItems[j].getLen();
				}
				// accountRefreshType
				_len += 1;
				// accountRefreshTimePoint
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(accountRefreshTimePoint);
					_len += 2 + _strBytes.Length;
				}
				// accountLimitBuyNum
				_len += 4;
				// accountRestBuyNum
				_len += 4;
				// accountBuyRecordNextRefreshTimestamp
				_len += 4;
				// roleRefreshType
				_len += 1;
				// roleRefreshTimePoint
				{
					byte[] _strBytes = StringHelper.StringToUTF8Bytes(roleRefreshTimePoint);
					_len += 2 + _strBytes.Length;
				}
				// roleBuyRecordNextRefreshTimestamp
				_len += 4;
				// roleLimitBuyNum
				_len += 4;
				// roleRestBuyNum
				_len += 4;
				// extensibleCondition
				_len += 4;
				// needPreviewFunc
				_len += 1;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  client->world 账号商店商品查询请求
	/// </summary>
	[Protocol]
	public class WorldAccountShopItemQueryReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 608801;
		public UInt32 Sequence;
		/// <summary>
		///  查询索引
		/// </summary>
		public AccountShopQueryIndex queryIndex = new AccountShopQueryIndex();

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
				queryIndex.encode(buffer, ref pos_);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				queryIndex.decode(buffer, ref pos_);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				queryIndex.encode(buffer, ref pos_);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				queryIndex.decode(buffer, ref pos_);
			}

			public int getLen()
			{
				int _len = 0;
				// queryIndex
				_len += queryIndex.getLen();
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  world->client 账号商店商品查询返回
	/// </summary>
	[Protocol]
	public class WorldAccountShopItemQueryRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 608802;
		public UInt32 Sequence;
		/// <summary>
		///  错误码
		/// </summary>
		public UInt32 resCode;
		/// <summary>
		///  查询索引
		/// </summary>
		public AccountShopQueryIndex queryIndex = new AccountShopQueryIndex();
		/// <summary>
		///  商品集
		/// </summary>
		public AccountShopItemInfo[] shopItems = new AccountShopItemInfo[0];
		/// <summary>
		///  下一波商品上架时间
		/// </summary>
		public UInt32 nextGroupOnSaleTime;

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
				BaseDLL.encode_uint32(buffer, ref pos_, resCode);
				queryIndex.encode(buffer, ref pos_);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)shopItems.Length);
				for(int i = 0; i < shopItems.Length; i++)
				{
					shopItems[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_uint32(buffer, ref pos_, nextGroupOnSaleTime);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref resCode);
				queryIndex.decode(buffer, ref pos_);
				UInt16 shopItemsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref shopItemsCnt);
				shopItems = new AccountShopItemInfo[shopItemsCnt];
				for(int i = 0; i < shopItems.Length; i++)
				{
					shopItems[i] = new AccountShopItemInfo();
					shopItems[i].decode(buffer, ref pos_);
				}
				BaseDLL.decode_uint32(buffer, ref pos_, ref nextGroupOnSaleTime);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, resCode);
				queryIndex.encode(buffer, ref pos_);
				BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)shopItems.Length);
				for(int i = 0; i < shopItems.Length; i++)
				{
					shopItems[i].encode(buffer, ref pos_);
				}
				BaseDLL.encode_uint32(buffer, ref pos_, nextGroupOnSaleTime);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref resCode);
				queryIndex.decode(buffer, ref pos_);
				UInt16 shopItemsCnt = 0;
				BaseDLL.decode_uint16(buffer, ref pos_, ref shopItemsCnt);
				shopItems = new AccountShopItemInfo[shopItemsCnt];
				for(int i = 0; i < shopItems.Length; i++)
				{
					shopItems[i] = new AccountShopItemInfo();
					shopItems[i].decode(buffer, ref pos_);
				}
				BaseDLL.decode_uint32(buffer, ref pos_, ref nextGroupOnSaleTime);
			}

			public int getLen()
			{
				int _len = 0;
				// resCode
				_len += 4;
				// queryIndex
				_len += queryIndex.getLen();
				// shopItems
				_len += 2;
				for(int j = 0; j < shopItems.Length; j++)
				{
					_len += shopItems[j].getLen();
				}
				// nextGroupOnSaleTime
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  client->world 账号商店商品购买请求
	/// </summary>
	[Protocol]
	public class WorldAccountShopItemBuyReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 608803;
		public UInt32 Sequence;
		/// <summary>
		///  查询索引
		/// </summary>
		public AccountShopQueryIndex queryIndex = new AccountShopQueryIndex();
		/// <summary>
		///  购买的商品id
		/// </summary>
		public UInt32 buyShopItemId;
		/// <summary>
		///  购买商品数量
		/// </summary>
		public UInt32 buyShopItemNum;

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
				queryIndex.encode(buffer, ref pos_);
				BaseDLL.encode_uint32(buffer, ref pos_, buyShopItemId);
				BaseDLL.encode_uint32(buffer, ref pos_, buyShopItemNum);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				queryIndex.decode(buffer, ref pos_);
				BaseDLL.decode_uint32(buffer, ref pos_, ref buyShopItemId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref buyShopItemNum);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				queryIndex.encode(buffer, ref pos_);
				BaseDLL.encode_uint32(buffer, ref pos_, buyShopItemId);
				BaseDLL.encode_uint32(buffer, ref pos_, buyShopItemNum);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				queryIndex.decode(buffer, ref pos_);
				BaseDLL.decode_uint32(buffer, ref pos_, ref buyShopItemId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref buyShopItemNum);
			}

			public int getLen()
			{
				int _len = 0;
				// queryIndex
				_len += queryIndex.getLen();
				// buyShopItemId
				_len += 4;
				// buyShopItemNum
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///  world->client 账号商店商品购买返回
	/// </summary>
	[Protocol]
	public class WorldAccountShopItemBuyRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 608804;
		public UInt32 Sequence;
		/// <summary>
		///  错误码
		/// </summary>
		public UInt32 resCode;
		/// <summary>
		///  查询索引
		/// </summary>
		public AccountShopQueryIndex queryIndex = new AccountShopQueryIndex();
		/// <summary>
		///  购买的商品id
		/// </summary>
		public UInt32 buyShopItemId;
		/// <summary>
		///  购买数量
		/// </summary>
		public UInt32 buyShopItemNum;
		/// <summary>
		///  账号剩余购买数量
		/// </summary>
		public UInt32 accountRestBuyNum;
		/// <summary>
		///  角色剩余购买数量
		/// </summary>
		public UInt32 roleRestBuyNum;

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
				BaseDLL.encode_uint32(buffer, ref pos_, resCode);
				queryIndex.encode(buffer, ref pos_);
				BaseDLL.encode_uint32(buffer, ref pos_, buyShopItemId);
				BaseDLL.encode_uint32(buffer, ref pos_, buyShopItemNum);
				BaseDLL.encode_uint32(buffer, ref pos_, accountRestBuyNum);
				BaseDLL.encode_uint32(buffer, ref pos_, roleRestBuyNum);
			}

			public void decode(byte[] buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref resCode);
				queryIndex.decode(buffer, ref pos_);
				BaseDLL.decode_uint32(buffer, ref pos_, ref buyShopItemId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref buyShopItemNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref accountRestBuyNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref roleRestBuyNum);
			}

			public void encode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.encode_uint32(buffer, ref pos_, resCode);
				queryIndex.encode(buffer, ref pos_);
				BaseDLL.encode_uint32(buffer, ref pos_, buyShopItemId);
				BaseDLL.encode_uint32(buffer, ref pos_, buyShopItemNum);
				BaseDLL.encode_uint32(buffer, ref pos_, accountRestBuyNum);
				BaseDLL.encode_uint32(buffer, ref pos_, roleRestBuyNum);
			}

			public void decode(MapViewStream buffer, ref int pos_)
			{
				BaseDLL.decode_uint32(buffer, ref pos_, ref resCode);
				queryIndex.decode(buffer, ref pos_);
				BaseDLL.decode_uint32(buffer, ref pos_, ref buyShopItemId);
				BaseDLL.decode_uint32(buffer, ref pos_, ref buyShopItemNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref accountRestBuyNum);
				BaseDLL.decode_uint32(buffer, ref pos_, ref roleRestBuyNum);
			}

			public int getLen()
			{
				int _len = 0;
				// resCode
				_len += 4;
				// queryIndex
				_len += queryIndex.getLen();
				// buyShopItemId
				_len += 4;
				// buyShopItemNum
				_len += 4;
				// accountRestBuyNum
				_len += 4;
				// roleRestBuyNum
				_len += 4;
				return _len;
			}
		#endregion

	}

	/// <summary>
	///   client->world 账号商店批量查询商品请求
	/// </summary>
	[Protocol]
	public class WorldAccountShopQueryBatchItemReq : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 608805;
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
	///  world->client 账号批量查询商品返回
	/// </summary>
	[Protocol]
	public class WorldAccountShopQueryBatchItemRes : Protocol.IProtocolStream, Protocol.IGetMsgID
	{
		public const UInt32 MsgID = 608806;
		public UInt32 Sequence;
		/// <summary>
		///  商品列表
		/// </summary>
		public AccountShopItemInfo[] shopItemList = new AccountShopItemInfo[0];

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
				shopItemList = new AccountShopItemInfo[shopItemListCnt];
				for(int i = 0; i < shopItemList.Length; i++)
				{
					shopItemList[i] = new AccountShopItemInfo();
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
				shopItemList = new AccountShopItemInfo[shopItemListCnt];
				for(int i = 0; i < shopItemList.Length; i++)
				{
					shopItemList[i] = new AccountShopItemInfo();
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

}
