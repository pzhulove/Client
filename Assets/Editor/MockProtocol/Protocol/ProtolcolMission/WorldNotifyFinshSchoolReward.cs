using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// world->client 出师成功奖励展示
	/// </summary>
	[AdvancedInspector.Descriptor("world->client 出师成功奖励展示", "world->client 出师成功奖励展示")]
	public class WorldNotifyFinshSchoolReward : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 601771;
		public UInt32 Sequence;

		public UInt32 giftId;

		public UInt64 masterId;

		public UInt64 discipleId;

		public string masterName;

		public string discipleName;

		public UInt32 discipleGrowth;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, giftId);
			BaseDLL.encode_uint64(buffer, ref pos_, masterId);
			BaseDLL.encode_uint64(buffer, ref pos_, discipleId);
			byte[] masterNameBytes = StringHelper.StringToUTF8Bytes(masterName);
			BaseDLL.encode_string(buffer, ref pos_, masterNameBytes, (UInt16)(buffer.Length - pos_));
			byte[] discipleNameBytes = StringHelper.StringToUTF8Bytes(discipleName);
			BaseDLL.encode_string(buffer, ref pos_, discipleNameBytes, (UInt16)(buffer.Length - pos_));
			BaseDLL.encode_uint32(buffer, ref pos_, discipleGrowth);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref giftId);
			BaseDLL.decode_uint64(buffer, ref pos_, ref masterId);
			BaseDLL.decode_uint64(buffer, ref pos_, ref discipleId);
			UInt16 masterNameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref masterNameLen);
			byte[] masterNameBytes = new byte[masterNameLen];
			for(int i = 0; i < masterNameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref masterNameBytes[i]);
			}
			masterName = StringHelper.BytesToString(masterNameBytes);
			UInt16 discipleNameLen = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref discipleNameLen);
			byte[] discipleNameBytes = new byte[discipleNameLen];
			for(int i = 0; i < discipleNameLen; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref discipleNameBytes[i]);
			}
			discipleName = StringHelper.BytesToString(discipleNameBytes);
			BaseDLL.decode_uint32(buffer, ref pos_, ref discipleGrowth);
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
