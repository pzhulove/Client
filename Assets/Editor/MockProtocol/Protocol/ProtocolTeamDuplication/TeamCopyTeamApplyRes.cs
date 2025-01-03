using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  申请入团返回
	/// </summary>
	[AdvancedInspector.Descriptor(" 申请入团返回", " 申请入团返回")]
	public class TeamCopyTeamApplyRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 1100010;
		public UInt32 Sequence;

		public UInt32 retCode;

		public UInt32 teamId;

		public UInt64 expireTime;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, retCode);
			BaseDLL.encode_uint32(buffer, ref pos_, teamId);
			BaseDLL.encode_uint64(buffer, ref pos_, expireTime);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
			BaseDLL.decode_uint32(buffer, ref pos_, ref teamId);
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
