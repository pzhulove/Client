using System;
using System.Text;

namespace Mock.Protocol
{

	public class GateEnterGameReq : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 300306;
		public UInt32 Sequence;

		public UInt64 roleId;

		public byte option;

		public string city;

		public UInt32 inviter;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, roleId);
			BaseDLL.encode_int8(buffer, ref pos_, option);
			byte[] cityBytes = StringHelper.StringToUTF8Bytes(city);
			BaseDLL.encode_string(buffer, ref pos_, cityBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_uint32(buffer, ref pos_, inviter);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref roleId);
			BaseDLL.decode_int8(buffer, ref pos_, ref option);
			UInt16 cityLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref cityLen);
			byte[] cityBytes = new byte[cityLen];
			for(int i = 0; i < cityLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref cityBytes[i]);
			}
			city = StringHelper.BytesToString(cityBytes);
			BaseDLL.decode_uint32(buffer, ref pos_, ref inviter);
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
