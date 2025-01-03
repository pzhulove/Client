using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  *************************************************************
	/// </summary>
	/// <summary>
	///  阶段同步
	/// </summary>
	[AdvancedInspector.Descriptor(" 阶段同步", " 阶段同步")]
	public class BetHorsePhaseSycn : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 708309;
		public UInt32 Sequence;
		/// <summary>
		///  赌马阶段同步(BetHorsePhaseType)
		/// </summary>
		[AdvancedInspector.Descriptor(" 赌马阶段同步(BetHorsePhaseType)", " 赌马阶段同步(BetHorsePhaseType)")]
		public UInt32 phaseSycn;
		/// <summary>
		///  时间戳
		/// </summary>
		[AdvancedInspector.Descriptor(" 时间戳", " 时间戳")]
		public UInt32 stamp;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, phaseSycn);
			BaseDLL.encode_uint32(buffer, ref pos_, stamp);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref phaseSycn);
			BaseDLL.decode_uint32(buffer, ref pos_, ref stamp);
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
