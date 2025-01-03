using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  上报单局校验数据
	/// </summary>
	[AdvancedInspector.Descriptor(" 上报单局校验数据", " 上报单局校验数据")]
	public class RelaySvrFrameChecksumRequest : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 1300011;
		public UInt32 Sequence;
		/// <summary>
		///  帧序号
		/// </summary>
		[AdvancedInspector.Descriptor(" 帧序号", " 帧序号")]
		public UInt32 frame;
		/// <summary>
		///  帧校验值
		/// </summary>
		[AdvancedInspector.Descriptor(" 帧校验值", " 帧校验值")]
		public UInt32 checksum;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, frame);
			BaseDLL.encode_uint32(buffer, ref pos_, checksum);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref frame);
			BaseDLL.decode_uint32(buffer, ref pos_, ref checksum);
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
