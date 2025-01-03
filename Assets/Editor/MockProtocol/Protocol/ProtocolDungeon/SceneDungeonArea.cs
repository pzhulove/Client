using System;
using System.Text;

namespace Mock.Protocol
{

	public class SceneDungeonArea : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public UInt32 id;

		public SceneDungeonMonster[] monsters = null;

		public SceneDungeonMonster[] destructs = null;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint32(buffer, ref pos_, id);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)monsters.Length);
			for(int i = 0; i < monsters.Length; i++)
			{
				monsters[i].encode(buffer, ref pos_);
			}
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)destructs.Length);
			for(int i = 0; i < destructs.Length; i++)
			{
				destructs[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint32(buffer, ref pos_, ref id);
			UInt16 monstersCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref monstersCnt);
			monsters = new SceneDungeonMonster[monstersCnt];
			for(int i = 0; i < monsters.Length; i++)
			{
				monsters[i] = new SceneDungeonMonster();
				monsters[i].decode(buffer, ref pos_);
			}
			UInt16 destructsCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref destructsCnt);
			destructs = new SceneDungeonMonster[destructsCnt];
			for(int i = 0; i < destructs.Length; i++)
			{
				destructs[i] = new SceneDungeonMonster();
				destructs[i].decode(buffer, ref pos_);
			}
		}


		#endregion

	}

}
