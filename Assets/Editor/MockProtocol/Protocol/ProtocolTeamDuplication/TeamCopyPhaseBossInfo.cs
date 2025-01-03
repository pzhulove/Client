using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  客户端上报阶段boss信息
	/// </summary>
	[AdvancedInspector.Descriptor(" 客户端上报阶段boss信息", " 客户端上报阶段boss信息")]
	public class TeamCopyPhaseBossInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 1100068;
		public UInt32 Sequence;
		/// <summary>
		///  比赛Id
		/// </summary>
		[AdvancedInspector.Descriptor(" 比赛Id", " 比赛Id")]
		public UInt64 raceId;
		/// <summary>
		///  角色Id
		/// </summary>
		[AdvancedInspector.Descriptor(" 角色Id", " 角色Id")]
		public UInt64 roleId;
		/// <summary>
		///  当前帧
		/// </summary>
		[AdvancedInspector.Descriptor(" 当前帧", " 当前帧")]
		public UInt32 curFrame;
		/// <summary>
		///  阶段
		/// </summary>
		[AdvancedInspector.Descriptor(" 阶段", " 阶段")]
		public UInt32 phase;
		/// <summary>
		///  boss血量百分比
		/// </summary>
		[AdvancedInspector.Descriptor(" boss血量百分比", " boss血量百分比")]
		public UInt32 bossBloodRate;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, raceId);
			BaseDLL.encode_uint64(buffer, ref pos_, roleId);
			BaseDLL.encode_uint32(buffer, ref pos_, curFrame);
			BaseDLL.encode_uint32(buffer, ref pos_, phase);
			BaseDLL.encode_uint32(buffer, ref pos_, bossBloodRate);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref raceId);
			BaseDLL.decode_uint64(buffer, ref pos_, ref roleId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref curFrame);
			BaseDLL.decode_uint32(buffer, ref pos_, ref phase);
			BaseDLL.decode_uint32(buffer, ref pos_, ref bossBloodRate);
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
