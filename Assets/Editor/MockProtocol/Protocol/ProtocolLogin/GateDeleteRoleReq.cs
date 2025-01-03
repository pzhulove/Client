using System;
using System.Text;

namespace Mock.Protocol
{

	public class GateDeleteRoleReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 300304;
		public UInt32 Sequence;

		public UInt64 roldId;

		public string deviceId;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, roldId);
			byte[] deviceIdBytes = StringHelper.StringToUTF8Bytes(deviceId);
			BaseDLL.encode_string(buffer, ref pos_, deviceIdBytes, (UInt16)(buffer.Length - pos_));
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref roldId);
			UInt16 deviceIdLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref deviceIdLen);
			byte[] deviceIdBytes = new byte[deviceIdLen];
			for(int i = 0; i < deviceIdLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref deviceIdBytes[i]);
			}
			deviceId = StringHelper.BytesToString(deviceIdBytes);
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
