using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  公会地下城boss死亡通知
	/// </summary>
	[AdvancedInspector.Descriptor(" 公会地下城boss死亡通知", " 公会地下城boss死亡通知")]
	public class WorldGuildDungeonBossDeadNotify : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 608512;
		public UInt32 Sequence;
		/// <summary>
		///  地下城id
		/// </summary>
		[AdvancedInspector.Descriptor(" 地下城id", " 地下城id")]
		public UInt32 dungeonId;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, dungeonId);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref dungeonId);
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
