using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  客服系统签名返回
	/// </summary>
	[AdvancedInspector.Descriptor(" 客服系统签名返回", " 客服系统签名返回")]
	public class WorldCustomServiceSignRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 600817;
		public UInt32 Sequence;
		/// <summary>
		///  结果
		/// </summary>
		[AdvancedInspector.Descriptor(" 结果", " 结果")]
		public UInt32 result;
		/// <summary>
		///  签名
		/// </summary>
		[AdvancedInspector.Descriptor(" 签名", " 签名")]
		public string sign;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, result);
			byte[] signBytes = StringHelper.StringToUTF8Bytes(sign);
			BaseDLL.encode_string(buffer, ref pos_, signBytes, (UInt16)(buffer.Length - pos_));
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref result);
			UInt16 signLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref signLen);
			byte[] signBytes = new byte[signLen];
			for(int i = 0; i < signLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref signBytes[i]);
			}
			sign = StringHelper.BytesToString(signBytes);
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
