using System;
using System.Text;

namespace Mock.Protocol
{

	public class DungeonPlayerRaceEndInfo : global::UnityEngine.ScriptableObject, global::Mock.Protocol.IMockProtocol, global::Protocol.IProtocolStream
	{

		public UInt64 roleId;

		public byte pos;

		public byte score;

		public byte[] md5 = new byte[16];

		public UInt16 beHitCount;

		public UInt64 bossDamage;
		/// <summary>
		///  玩家剩余血量
		/// </summary>
		[AdvancedInspector.Descriptor(" 玩家剩余血量", " 玩家剩余血量")]
		public UInt32 playerRemainHp;
		/// <summary>
		///  玩家剩余蓝量
		/// </summary>
		[AdvancedInspector.Descriptor(" 玩家剩余蓝量", " 玩家剩余蓝量")]
		public UInt32 playerRemainMp;
		/// <summary>
		///  BOSS1的ID
		/// </summary>
		[AdvancedInspector.Descriptor(" BOSS1的ID", " BOSS1的ID")]
		public UInt32 boss1ID;
		/// <summary>
		///  BOSS2的ID
		/// </summary>
		[AdvancedInspector.Descriptor(" BOSS2的ID", " BOSS2的ID")]
		public UInt32 boss2ID;
		/// <summary>
		///  BOSS1的剩余血量
		/// </summary>
		[AdvancedInspector.Descriptor(" BOSS1的剩余血量", " BOSS1的剩余血量")]
		public UInt32 boss1RemainHp;
		/// <summary>
		///  BOSS2的剩余血量
		/// </summary>
		[AdvancedInspector.Descriptor(" BOSS2的剩余血量", " BOSS2的剩余血量")]
		public UInt32 boss2RemainHp;

		#region METHOD

		public void encode(byte[] buffer, ref int pos_)
		{
			BaseDLL.encode_uint64(buffer, ref pos_, roleId);
			BaseDLL.encode_int8(buffer, ref pos_, pos);
			BaseDLL.encode_int8(buffer, ref pos_, score);
			for(int i = 0; i < md5.Length; i++)
			{
				BaseDLL.encode_int8(buffer, ref pos_, md5[i]);
			}
			BaseDLL.encode_uint16(buffer, ref pos_, beHitCount);
			BaseDLL.encode_uint64(buffer, ref pos_, bossDamage);
			BaseDLL.encode_uint32(buffer, ref pos_, playerRemainHp);
			BaseDLL.encode_uint32(buffer, ref pos_, playerRemainMp);
			BaseDLL.encode_uint32(buffer, ref pos_, boss1ID);
			BaseDLL.encode_uint32(buffer, ref pos_, boss2ID);
			BaseDLL.encode_uint32(buffer, ref pos_, boss1RemainHp);
			BaseDLL.encode_uint32(buffer, ref pos_, boss2RemainHp);
		}

		public void decode(byte[] buffer, ref int pos_)
		{
			BaseDLL.decode_uint64(buffer, ref pos_, ref roleId);
			BaseDLL.decode_int8(buffer, ref pos_, ref pos);
			BaseDLL.decode_int8(buffer, ref pos_, ref score);
			for(int i = 0; i < md5.Length; i++)
			{
				BaseDLL.decode_int8(buffer, ref pos_, ref md5[i]);
			}
			BaseDLL.decode_uint16(buffer, ref pos_, ref beHitCount);
			BaseDLL.decode_uint64(buffer, ref pos_, ref bossDamage);
			BaseDLL.decode_uint32(buffer, ref pos_, ref playerRemainHp);
			BaseDLL.decode_uint32(buffer, ref pos_, ref playerRemainMp);
			BaseDLL.decode_uint32(buffer, ref pos_, ref boss1ID);
			BaseDLL.decode_uint32(buffer, ref pos_, ref boss2ID);
			BaseDLL.decode_uint32(buffer, ref pos_, ref boss1RemainHp);
			BaseDLL.decode_uint32(buffer, ref pos_, ref boss2RemainHp);
		}


		#endregion

	}

}
