using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  重连帧数据
	/// </summary>
	[AdvancedInspector.Descriptor(" 重连帧数据", " 重连帧数据")]
	public class RelaySvrReconnectFrameData : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 1300014;
		public UInt32 Sequence;

		public byte finish;

		public Frame[] frames = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, finish);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)frames.Length);
			for(int i = 0; i < frames.Length; i++)
			{
				frames[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref finish);
			UInt16 framesCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref framesCnt);
			frames = new Frame[framesCnt];
			for(int i = 0; i < frames.Length; i++)
			{
				frames[i] = new Frame();
				frames[i].decode(buffer, ref pos_);
			}
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
