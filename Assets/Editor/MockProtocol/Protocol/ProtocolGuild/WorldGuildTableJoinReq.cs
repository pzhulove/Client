using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  请求加入圆桌会议
	/// </summary>
	[AdvancedInspector.Descriptor(" 请求加入圆桌会议", " 请求加入圆桌会议")]
	public class WorldGuildTableJoinReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601937;
		public UInt32 Sequence;
		/// <summary>
		///  位置
		/// </summary>
		[AdvancedInspector.Descriptor(" 位置", " 位置")]
		public byte seat;
		/// <summary>
		///  是不是协助
		/// </summary>
		[AdvancedInspector.Descriptor(" 是不是协助", " 是不是协助")]
		public byte type;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, seat);
			BaseDLL.encode_int8(buffer, ref pos_, type);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref seat);
			BaseDLL.decode_int8(buffer, ref pos_, ref type);
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
