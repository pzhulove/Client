using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  公会地下城boss剩余血量
	/// </summary>
	[AdvancedInspector.Descriptor(" 公会地下城boss剩余血量", " 公会地下城boss剩余血量")]
	public class WorldGuildDungeonBossOddBlood : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 608521;
		public UInt32 Sequence;

		public UInt64 bossOddBlood;

		public UInt64 bossTotalBlood;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, bossOddBlood);
			BaseDLL.encode_uint64(buffer, ref pos_, bossTotalBlood);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref bossOddBlood);
			BaseDLL.decode_uint64(buffer, ref pos_, ref bossTotalBlood);
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
