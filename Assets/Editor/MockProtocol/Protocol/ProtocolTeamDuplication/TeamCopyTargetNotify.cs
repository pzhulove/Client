using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  目标通知
	/// </summary>
	[AdvancedInspector.Descriptor(" 目标通知", " 目标通知")]
	public class TeamCopyTargetNotify : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 1100021;
		public UInt32 Sequence;
		/// <summary>
		///  小队目标
		/// </summary>
		[AdvancedInspector.Descriptor(" 小队目标", " 小队目标")]
		public TeamCopyTarget squadTarget = null;
		/// <summary>
		///  团队目标
		/// </summary>
		[AdvancedInspector.Descriptor(" 团队目标", " 团队目标")]
		public TeamCopyTarget teamTarget = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			squadTarget.encode(buffer, ref pos_);
			teamTarget.encode(buffer, ref pos_);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			squadTarget.decode(buffer, ref pos_);
			teamTarget.decode(buffer, ref pos_);
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
