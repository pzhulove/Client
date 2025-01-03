using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// world->client 购买夺宝商品返回
	/// </summary>
	[AdvancedInspector.Descriptor("world->client 购买夺宝商品返回", "world->client 购买夺宝商品返回")]
	public class PayingGambleRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 707902;
		public UInt32 Sequence;
		/// <summary>
		///  返回码
		/// </summary>
		[AdvancedInspector.Descriptor(" 返回码", " 返回码")]
		public UInt32 retCode;
		/// <summary>
		///  夺宝商品id
		/// </summary>
		[AdvancedInspector.Descriptor(" 夺宝商品id", " 夺宝商品id")]
		public UInt32 gambingItemId;
		/// <summary>
		///  组id
		/// </summary>
		[AdvancedInspector.Descriptor(" 组id", " 组id")]
		public UInt16 groupId;
		/// <summary>
		///  投入份数
		/// </summary>
		[AdvancedInspector.Descriptor(" 投入份数", " 投入份数")]
		public UInt32 investCopies;
		/// <summary>
		///  花费货币id
		/// </summary>
		[AdvancedInspector.Descriptor(" 花费货币id", " 花费货币id")]
		public UInt32 costCurrencyId;
		/// <summary>
		///  花费货币数
		/// </summary>
		[AdvancedInspector.Descriptor(" 花费货币数", " 花费货币数")]
		public UInt32 costCurrencyNum;

		public GambingItemInfo itemInfo = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
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
