using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  请求公会宣战报名
	/// </summary>
	[AdvancedInspector.Descriptor(" 请求公会宣战报名", " 请求公会宣战报名")]
	public class WorldGuildChallengeReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601959;
		public UInt32 Sequence;

		public byte terrId;

		public UInt32 itemId;

		public UInt32 itemNum;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, terrId);
			BaseDLL.encode_uint32(buffer, ref pos_, itemId);
			BaseDLL.encode_uint32(buffer, ref pos_, itemNum);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref terrId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref itemId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref itemNum);
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
