using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  world->client 账号商店商品购买返回
	/// </summary>
	[AdvancedInspector.Descriptor(" world->client 账号商店商品购买返回", " world->client 账号商店商品购买返回")]
	public class WorldAccountShopItemBuyRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 608804;
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
		///  购买的商品id
		/// </summary>
		[AdvancedInspector.Descriptor(" 购买的商品id", " 购买的商品id")]
		public UInt32 buyShopItemId;
		/// <summary>
		///  购买数量
		/// </summary>
		[AdvancedInspector.Descriptor(" 购买数量", " 购买数量")]
		public UInt32 buyShopItemNum;
		/// <summary>
		///  账号剩余购买数量
		/// </summary>
		[AdvancedInspector.Descriptor(" 账号剩余购买数量", " 账号剩余购买数量")]
		public UInt32 accountRestBuyNum;
		/// <summary>
		///  角色剩余购买数量
		/// </summary>
		[AdvancedInspector.Descriptor(" 角色剩余购买数量", " 角色剩余购买数量")]
		public UInt32 roleRestBuyNum;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
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
