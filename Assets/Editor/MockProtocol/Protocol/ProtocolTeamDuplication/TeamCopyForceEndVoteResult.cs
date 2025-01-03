using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  强制投票结果
	/// </summary>
	[AdvancedInspector.Descriptor(" 强制投票结果", " 强制投票结果")]
	public class TeamCopyForceEndVoteResult : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 1100075;
		public UInt32 Sequence;
		/// <summary>
		///  返回结果(0成功，非0失败)
		/// </summary>
		[AdvancedInspector.Descriptor(" 返回结果(0成功，非0失败)", " 返回结果(0成功，非0失败)")]
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
