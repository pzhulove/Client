using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// client->world 购买夺宝商品请求
	/// </summary>
	[AdvancedInspector.Descriptor("client->world 购买夺宝商品请求", "client->world 购买夺宝商品请求")]
	public class PayingGambleReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 707901;
		public UInt32 Sequence;
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
		///  是否购入全部剩余份数(1:是,0:否)
		/// </summary>
		[AdvancedInspector.Descriptor(" 是否购入全部剩余份数(1:是,0:否)", " 是否购入全部剩余份数(1:是,0:否)")]
		public byte bBuyAll;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
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
