using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  请求客服系统签名
	/// </summary>
	[AdvancedInspector.Descriptor(" 请求客服系统签名", " 请求客服系统签名")]
	public class WorldCustomServiceSignReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 600816;
		public UInt32 Sequence;
		/// <summary>
		///  加密信息
		/// </summary>
		[AdvancedInspector.Descriptor(" 加密信息", " 加密信息")]
		public string info;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			byte[] infoBytes = StringHelper.StringToUTF8Bytes(info);
			BaseDLL.encode_string(buffer, ref pos_, infoBytes, (UInt16)(buffer.Length - pos_));
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 infoLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref infoLen);
			byte[] infoBytes = new byte[infoLen];
			for(int i = 0; i < infoLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref infoBytes[i]);
			}
			info = StringHelper.BytesToString(infoBytes);
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
