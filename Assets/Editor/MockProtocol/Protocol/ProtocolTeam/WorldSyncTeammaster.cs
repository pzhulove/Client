using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  同步新队长
	/// </summary>
	[AdvancedInspector.Descriptor(" 同步新队长", " 同步新队长")]
	public class WorldSyncTeammaster : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601609;
		public UInt32 Sequence;
		/// <summary>
		///  队长ID
		/// </summary>
		[AdvancedInspector.Descriptor(" 队长ID", " 队长ID")]
		public UInt64 master;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, master);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref master);
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
