using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// client ->  world 同意，拒绝公会兼并申请，取消已同意的申请
	/// </summary>
	[AdvancedInspector.Descriptor("client ->  world 同意，拒绝公会兼并申请，取消已同意的申请", "client ->  world 同意，拒绝公会兼并申请，取消已同意的申请")]
	public class WorldGuildAcceptOrRefuseOrCancleMergerRequestReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601985;
		public UInt32 Sequence;

		public UInt64 guildId;
		/// <summary>
		/// 操作类型 0同意 1拒绝 2取消 3清空请求
		/// </summary>
		[AdvancedInspector.Descriptor("操作类型 0同意 1拒绝 2取消 3清空请求", "操作类型 0同意 1拒绝 2取消 3清空请求")]
		public byte opType;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, guildId);
			BaseDLL.encode_int8(buffer, ref pos_, opType);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref guildId);
			BaseDLL.decode_int8(buffer, ref pos_, ref opType);
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
