using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  通知成员上下线
	/// </summary>
	[AdvancedInspector.Descriptor(" 通知成员上下线", " 通知成员上下线")]
	public class WorldSyncTeamMemberStatus : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601605;
		public UInt32 Sequence;
		/// <summary>
		///  队员ID
		/// </summary>
		[AdvancedInspector.Descriptor(" 队员ID", " 队员ID")]
		public UInt64 id;
		/// <summary>
		///  状态掩码（对应枚举TeamMemberStatusMask）
		/// </summary>
		[AdvancedInspector.Descriptor(" 状态掩码（对应枚举TeamMemberStatusMask）", " 状态掩码（对应枚举TeamMemberStatusMask）")]
		public byte statusMask;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, id);
			BaseDLL.encode_int8(buffer, ref pos_, statusMask);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref id);
			BaseDLL.decode_int8(buffer, ref pos_, ref statusMask);
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
