using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  创建队伍返回
	/// </summary>
	[AdvancedInspector.Descriptor(" 创建队伍返回", " 创建队伍返回")]
	public class WorldCreateTeamRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601627;
		public UInt32 Sequence;
		/// <summary>
		///  返回码(ErrorCode)
		/// </summary>
		[AdvancedInspector.Descriptor(" 返回码(ErrorCode)", " 返回码(ErrorCode)")]
		public UInt32 result;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, result);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref result);
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
