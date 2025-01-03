using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  通知吃鸡杂项数据
	/// </summary>
	[AdvancedInspector.Descriptor(" 通知吃鸡杂项数据", " 通知吃鸡杂项数据")]
	public class BattleNotifyChijiMisc : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 2200022;
		public UInt32 Sequence;
		/// <summary>
		///  移动包发送间隔(ms)
		/// </summary>
		[AdvancedInspector.Descriptor(" 移动包发送间隔(ms)", " 移动包发送间隔(ms)")]
		public UInt32 moveIntervalMs;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, moveIntervalMs);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref moveIntervalMs);
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
