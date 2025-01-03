using System;
using System.Text;

namespace Mock.Protocol
{

	public class SceneDungeonMonster : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public UInt32 id;

		public UInt32 pointId;

		public UInt32 typeId;

		public SceneDungeonDropItem[] dropItems = null;

		public byte[] prefixes = new byte[0];

		public UInt32 summonerId;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, id);
			BaseDLL.encode_uint32(buffer, ref pos_, pointId);
			BaseDLL.encode_uint32(buffer, ref pos_, typeId);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)dropItems.Length);
			for(int i = 0; i < dropItems.Length; i++)
			{
				dropItems[i].encode(buffer, ref pos_);
			}
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)prefixes.Length);
			for(int i = 0; i < prefixes.Length; i++)
			{
				BaseDLL.encode_int8(buffer, ref pos_, prefixes[i]);
			}
			BaseDLL.encode_uint32(buffer, ref pos_, summonerId);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref id);
			BaseDLL.decode_uint32(buffer, ref pos_, ref pointId);
			BaseDLL.decode_uint32(buffer, ref pos_, ref typeId);
			UInt16 dropItemsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref dropItemsCnt);
			dropItems = new SceneDungeonDropItem[dropItemsCnt];
			for(int i = 0; i < dropItems.Length; i++)
			{
				dropItems[i] = new SceneDungeonDropItem();
				dropItems[i].decode(buffer, ref pos_);
			}
			UInt16 prefixesCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref prefixesCnt);
			prefixes = new byte[prefixesCnt];
			for(int i = 0; i < prefixes.Length; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref prefixes[i]);
			}
			BaseDLL.decode_uint32(buffer, ref pos_, ref summonerId);
		}


		#endregion

	}

}
