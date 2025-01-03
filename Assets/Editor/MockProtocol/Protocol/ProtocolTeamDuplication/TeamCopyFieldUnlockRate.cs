using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  据点解锁比例通知
	/// </summary>
	[AdvancedInspector.Descriptor(" 据点解锁比例通知", " 据点解锁比例通知")]
	public class TeamCopyFieldUnlockRate : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 1100067;
		public UInt32 Sequence;
		/// <summary>
		///  boss阶段
		/// </summary>
		[AdvancedInspector.Descriptor(" boss阶段", " boss阶段")]
		public UInt32 bossPhase;
		/// <summary>
		///  boss血量比例
		/// </summary>
		[AdvancedInspector.Descriptor(" boss血量比例", " boss血量比例")]
		public UInt32 bossBloodRate;
		/// <summary>
		///  据点id
		/// </summary>
		[AdvancedInspector.Descriptor(" 据点id", " 据点id")]
		public UInt32 fieldId;
		/// <summary>
		///  比例
		/// </summary>
		[AdvancedInspector.Descriptor(" 比例", " 比例")]
		public UInt32 unlockRate;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, bossPhase);
			BaseDLL.encode_uint32(buffer, ref pos_, bossBloodRate);
			BaseDLL.encode_uint32(buffer, ref pos_, fieldId);
			BaseDLL.encode_uint32(buffer, ref pos_, unlockRate);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref bossPhase);
			BaseDLL.decode_uint32(buffer, ref pos_, ref bossBloodRate);
			BaseDLL.decode_uint32(buffer, ref pos_, ref fieldId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref unlockRate);
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
