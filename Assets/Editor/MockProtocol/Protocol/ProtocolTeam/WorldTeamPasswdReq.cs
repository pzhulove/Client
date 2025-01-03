using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  队伍如果有密码发起请求密码
	/// </summary>
	[AdvancedInspector.Descriptor(" 队伍如果有密码发起请求密码", " 队伍如果有密码发起请求密码")]
	public class WorldTeamPasswdReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601612;
		public UInt32 Sequence;
		/// <summary>
		/// 目标
		/// </summary>
		[AdvancedInspector.Descriptor("目标", "目标")]
		public UInt64 target;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, target);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref target);
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
