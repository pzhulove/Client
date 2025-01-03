using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  任命
	/// </summary>
	[AdvancedInspector.Descriptor(" 任命", " 任命")]
	public class TeamCopyAppointmentReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 1100047;
		public UInt32 Sequence;
		/// <summary>
		///  任命玩家
		/// </summary>
		[AdvancedInspector.Descriptor(" 任命玩家", " 任命玩家")]
		public UInt64 playerId;
		/// <summary>
		///  职位
		/// </summary>
		[AdvancedInspector.Descriptor(" 职位", " 职位")]
		public UInt32 post;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, playerId);
			BaseDLL.encode_uint32(buffer, ref pos_, post);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref playerId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref post);
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
