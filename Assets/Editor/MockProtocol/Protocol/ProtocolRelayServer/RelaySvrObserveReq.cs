using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  观战请求
	/// </summary>
	[AdvancedInspector.Descriptor(" 观战请求", " 观战请求")]
	public class RelaySvrObserveReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 1300022;
		public UInt32 Sequence;
		/// <summary>
		///  账号ID
		/// </summary>
		[AdvancedInspector.Descriptor(" 账号ID", " 账号ID")]
		public UInt32 accid;
		/// <summary>
		///  角色ID
		/// </summary>
		[AdvancedInspector.Descriptor(" 角色ID", " 角色ID")]
		public UInt64 roleId;
		/// <summary>
		///  比赛ID
		/// </summary>
		[AdvancedInspector.Descriptor(" 比赛ID", " 比赛ID")]
		public UInt64 raceId;
		/// <summary>
		///  开始帧
		/// </summary>
		[AdvancedInspector.Descriptor(" 开始帧", " 开始帧")]
		public UInt32 startFrame;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, accid);
			BaseDLL.encode_uint64(buffer, ref pos_, roleId);
			BaseDLL.encode_uint64(buffer, ref pos_, raceId);
			BaseDLL.encode_uint32(buffer, ref pos_, startFrame);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref accid);
			BaseDLL.decode_uint64(buffer, ref pos_, ref roleId);
			BaseDLL.decode_uint64(buffer, ref pos_, ref raceId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref startFrame);
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
