using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  加入队伍返回
	/// </summary>
	[AdvancedInspector.Descriptor(" 加入队伍返回", " 加入队伍返回")]
	public class WorldJoinTeamRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601628;
		public UInt32 Sequence;
		/// <summary>
		///  返回码(ErrorCode)
		/// </summary>
		[AdvancedInspector.Descriptor(" 返回码(ErrorCode)", " 返回码(ErrorCode)")]
		public UInt32 result;

		public byte inTeam;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, result);
			BaseDLL.encode_int8(buffer, ref pos_, inTeam);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref result);
			BaseDLL.decode_int8(buffer, ref pos_, ref inTeam);
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
