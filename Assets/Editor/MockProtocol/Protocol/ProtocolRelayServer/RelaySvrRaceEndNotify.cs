using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  通知比赛结束
	/// </summary>
	[AdvancedInspector.Descriptor(" 通知比赛结束", " 通知比赛结束")]
	public class RelaySvrRaceEndNotify : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 1300009;
		public UInt32 Sequence;
		/// <summary>
		///  结束原因（对应枚举RaceEndReason）
		/// </summary>
		[AdvancedInspector.Descriptor(" 结束原因（对应枚举RaceEndReason）", " 结束原因（对应枚举RaceEndReason）")]
		public byte reason;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, reason);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref reason);
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
