using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// 请求聊天链接信息
	/// </summary>
	[AdvancedInspector.Descriptor("请求聊天链接信息", "请求聊天链接信息")]
	public class WorldChatLinkDataReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 600802;
		public UInt32 Sequence;

		public byte type;

		public UInt64 uid;

		public UInt32 queryType;

		public UInt32 zoneId;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, type);
			BaseDLL.encode_uint64(buffer, ref pos_, uid);
			BaseDLL.encode_uint32(buffer, ref pos_, queryType);
			BaseDLL.encode_uint32(buffer, ref pos_, zoneId);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref type);
			BaseDLL.decode_uint64(buffer, ref pos_, ref uid);
			BaseDLL.decode_uint32(buffer, ref pos_, ref queryType);
			BaseDLL.decode_uint32(buffer, ref pos_, ref zoneId);
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