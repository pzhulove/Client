using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  client->world 账号商店商品购买请求
	/// </summary>
	[AdvancedInspector.Descriptor(" client->world 账号商店商品购买请求", " client->world 账号商店商品购买请求")]
	public class WorldAccountShopItemBuyReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 608803;
		public UInt32 Sequence;
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
		///  购买商品数量
		/// </summary>
		[AdvancedInspector.Descriptor(" 购买商品数量", " 购买商品数量")]
		public UInt32 buyShopItemNum;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
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
