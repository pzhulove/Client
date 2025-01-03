using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  团队列表请求
	/// </summary>
	[AdvancedInspector.Descriptor(" 团队列表请求", " 团队列表请求")]
	public class TeamCopyTeamListReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 1100007;
		public UInt32 Sequence;
		/// <summary>
		///  队伍模式
		/// </summary>
		[AdvancedInspector.Descriptor(" 队伍模式", " 队伍模式")]
		public UInt32 teamModel;
		/// <summary>
		///  请求页
		/// </summary>
		[AdvancedInspector.Descriptor(" 请求页", " 请求页")]
		public UInt32 pageNum;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, teamModel);
			BaseDLL.encode_uint32(buffer, ref pos_, pageNum);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref teamModel);
			BaseDLL.decode_uint32(buffer, ref pos_, ref pageNum);
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
