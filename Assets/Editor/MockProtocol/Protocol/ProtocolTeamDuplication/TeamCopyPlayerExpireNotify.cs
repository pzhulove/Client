using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  玩家离线通知
	/// </summary>
	[AdvancedInspector.Descriptor(" 玩家离线通知", " 玩家离线通知")]
	public class TeamCopyPlayerExpireNotify : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 1100066;
		public UInt32 Sequence;

		public UInt64 playerId;

		public UInt64 expireTime;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, playerId);
			BaseDLL.encode_uint64(buffer, ref pos_, expireTime);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref playerId);
			BaseDLL.decode_uint64(buffer, ref pos_, ref expireTime);
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
