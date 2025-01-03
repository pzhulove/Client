using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  公会地下城状态同步
	/// </summary>
	[AdvancedInspector.Descriptor(" 公会地下城状态同步", " 公会地下城状态同步")]
	public class WorldGuildDungeonStatusSync : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 608509;
		public UInt32 Sequence;
		/// <summary>
		///  状态
		/// </summary>
		[AdvancedInspector.Descriptor(" 状态", " 状态")]
		public UInt32 dungeonStatus;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, dungeonStatus);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref dungeonStatus);
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
