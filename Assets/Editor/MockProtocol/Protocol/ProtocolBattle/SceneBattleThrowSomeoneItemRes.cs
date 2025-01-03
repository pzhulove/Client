using System;
using System.Text;

namespace Mock.Protocol
{

	public class SceneBattleThrowSomeoneItemRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 508921;
		public UInt32 Sequence;

		public UInt32 retCode;

		public UInt64 attackID;

		public UInt64 targetID;

		public UInt64 itemGuid;

		public UInt32 itemDataID;

		public string targetName;

		public UInt32 param;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, retCode);
			BaseDLL.encode_uint64(buffer, ref pos_, attackID);
			BaseDLL.encode_uint64(buffer, ref pos_, targetID);
			BaseDLL.encode_uint64(buffer, ref pos_, itemGuid);
			BaseDLL.encode_uint32(buffer, ref pos_, itemDataID);
			byte[] targetNameBytes = StringHelper.StringToUTF8Bytes(targetName);
			BaseDLL.encode_string(buffer, ref pos_, targetNameBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_uint32(buffer, ref pos_, param);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref retCode);
			BaseDLL.decode_uint64(buffer, ref pos_, ref attackID);
			BaseDLL.decode_uint64(buffer, ref pos_, ref targetID);
			BaseDLL.decode_uint64(buffer, ref pos_, ref itemGuid);
			BaseDLL.decode_uint32(buffer, ref pos_, ref itemDataID);
			UInt16 targetNameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref targetNameLen);
			byte[] targetNameBytes = new byte[targetNameLen];
			for(int i = 0; i < targetNameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref targetNameBytes[i]);
			}
			targetName = StringHelper.BytesToString(targetNameBytes);
			BaseDLL.decode_uint32(buffer, ref pos_, ref param);
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
