using System;
using System.Text;

namespace Mock.Protocol
{
	/// <summary>
	///  深渊波次信息
	/// </summary>
	[AdvancedInspector.Descriptor(" 深渊波次信息", " 深渊波次信息")]
	public class DungeonHellWaveInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public byte wave;

		public SceneDungeonMonster[] monsters = null;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_int8(buffer, ref pos_, wave);
			BaseDLL.encode_uint16(buffer, ref pos_, (UInt16)monsters.Length);
			for(int i = 0; i < monsters.Length; i++)
			{
				monsters[i].encode(buffer, ref pos_);
			}
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_int8(buffer, ref pos_, ref wave);
			UInt16 monstersCnt = 0;
			BaseDLL.decode_uint16(buffer, ref pos_, ref monstersCnt);
			monsters = new SceneDungeonMonster[monstersCnt];
			for(int i = 0; i < monsters.Length; i++)
			{
				monsters[i] = new SceneDungeonMonster();
				monsters[i].decode(buffer, ref pos_);
			}
		}


		#endregion

	}

}
