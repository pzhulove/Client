using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  请求离开队伍
	/// </summary>
	[AdvancedInspector.Descriptor(" 请求离开队伍", " 请求离开队伍")]
	public class WorldLeaveTeamReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601603;
		public UInt32 Sequence;
		/// <summary>
		///  踢人或自己
		/// </summary>
		[AdvancedInspector.Descriptor(" 踢人或自己", " 踢人或自己")]
		public UInt64 id;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, id);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref id);
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
