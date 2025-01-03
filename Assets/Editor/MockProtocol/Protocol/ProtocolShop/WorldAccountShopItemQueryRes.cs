using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  world->client 账号商店商品查询返回
	/// </summary>
	[AdvancedInspector.Descriptor(" world->client 账号商店商品查询返回", " world->client 账号商店商品查询返回")]
	public class WorldAccountShopItemQueryRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 608802;
		public UInt32 Sequence;
		/// <summary>
		///  错误码
		/// </summary>
		[AdvancedInspector.Descriptor(" 错误码", " 错误码")]
		public UInt32 resCode;
		/// <summary>
		///  查询索引
		/// </summary>
		[AdvancedInspector.Descriptor(" 查询索引", " 查询索引")]
		public AccountShopQueryIndex queryIndex = null;
		/// <summary>
		///  商品集
		/// </summary>
		[AdvancedInspector.Descriptor(" 商品集", " 商品集")]
		public AccountShopItemInfo[] shopItems = null;
		/// <summary>
		///  下一波商品上架时间
		/// </summary>
		[AdvancedInspector.Descriptor(" 下一波商品上架时间", " 下一波商品上架时间")]
		public UInt32 nextGroupOnSaleTime;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
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

		public UInt32 GetSequence()
		{
			return Sequence;
		}

		public void SetSequence(UInt32 sequence)
		{
			Sequence = sequence;
		}

		#endregion

	}

}
