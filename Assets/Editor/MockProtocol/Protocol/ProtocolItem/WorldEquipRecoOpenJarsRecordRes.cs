using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	/// world->client 装备回收开罐子记录返回
	/// </summary>
	[AdvancedInspector.Descriptor("world->client 装备回收开罐子记录返回", "world->client 装备回收开罐子记录返回")]
	public class WorldEquipRecoOpenJarsRecordRes : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream, global::Protocol.IGetMsgID
	{
		[AdvancedInspector.Descriptor("ID", "")]
		public const UInt32 MsgID = 600905;
		public UInt32 Sequence;

		public OpenJarRecord[] records = null;

		#region METHOD
		public UInt32 GetMsgID()
		{
			return MsgID;
		}

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)records.Length);
			for(int i = 0; i < records.Length; i++)
			{
				records[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			UInt16 recordsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref recordsCnt);
			records = new OpenJarRecord[recordsCnt];
			for(int i = 0; i < records.Length; i++)
			{
				records[i] = new OpenJarRecord();
				records[i].decode(buffer, ref pos_);
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