using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  ***********************************************
	/// </summary>
	/// <summary>
	///  请求绑定或解绑设备
	/// </summary>
	[AdvancedInspector.Descriptor(" 请求绑定或解绑设备", " 请求绑定或解绑设备")]
	public class WorldBindDeviceReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 608408;
		public UInt32 Sequence;
		/// <summary>
		///  绑定类型，0解绑，否则绑定
		/// </summary>
		[AdvancedInspector.Descriptor(" 绑定类型，0解绑，否则绑定", " 绑定类型，0解绑，否则绑定")]
		public UInt32 bindType;
		/// <summary>
		///  设备ID，客户端没有id时，发送空字符
		/// </summary>
		[AdvancedInspector.Descriptor(" 设备ID，客户端没有id时，发送空字符", " 设备ID，客户端没有id时，发送空字符")]
		public string deviceID;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, bindType);
			byte[] deviceIDBytes = StringHelper.StringToUTF8Bytes(deviceID);
			BaseDLL.encode_string(buffer, ref pos_, deviceIDBytes, (UInt16)(buffer.Length - pos_));
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref bindType);
			UInt16 deviceIDLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref deviceIDLen);
			byte[] deviceIDBytes = new byte[deviceIDLen];
			for(int i = 0; i < deviceIDLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref deviceIDBytes[i]);
			}
			deviceID = StringHelper.BytesToString(deviceIDBytes);
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
